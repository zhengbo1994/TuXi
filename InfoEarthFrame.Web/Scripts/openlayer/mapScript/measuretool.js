/**
 * OpenLayers 3 MeasureTool Control.
 * See [the examples](./examples) for usage.
 * @constructor
 * @extends {ol.control.Control}
 * @param {Object} opt_options Control options, extends olx.control.ControlOptions adding:
 *                              **`tipLabel`** `String` - the button tooltip.
 */
ol.control.MeasureTool = function (opt_options) {

    var options = opt_options || {};
    this.sphereradius = options.sphereradius ?
      options.sphereradius : 6378137;
    this.mapListeners = [];

    // hiddenclass
    this.hiddenClassName = 'ol-control MeasureTool';
    if (ol.control.MeasureTool.isTouchDevice_()) {
        this.hiddenClassName += ' touch';
    }
    // shownClass
    this.shownClassName = this.hiddenClassName + ' shown';

    var element = document.createElement('div');
    element.className = this.hiddenClassName;

    this.source = new ol.source.Vector();
    this.vector = new ol.layer.Vector({
        source: this.source,
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: 'rgba(252, 216, 107, 0.35)'
            }),
            stroke: new ol.style.Stroke({
                color: '#ffcc33',
                width: 2
            }),
            image: new ol.style.Circle({
                radius: 7,
                fill: new ol.style.Fill({
                    color: '#ffcc33'
                })
            }),
            name:"layerName"
        })
    });

    ol.control.Control.call(this, {
        element: element,
    });

};

ol.inherits(ol.control.MeasureTool, ol.control.Control);
var draw; // global so we can remove it later
ol.control.MeasureTool.prototype.mapmeasure = function (typeSelect, drawstart, drawend) {
    var map = this.getMap();
    if (draw) {
        map.removeInteraction(draw); //移除之前的绘制对象
    }
    var _self = this,
     source = _self.source,
     vector = _self.vector,
     wgs84Sphere = new ol.Sphere(_self.sphereradius),
     sketch,
     helpTooltipElement,
     measureTooltipElement,
     measureTooltip;
     //source.clear(); //一次只能画一个
    //var map = this.getMap();
    //map.getOverlays().clear();
    //map.addLayer(vector);

    //map.getViewport().addEventListener('mouseout', function () {
    //    helpTooltipElement.classList.add('hidden');
    //});

    

    var formatLength = function (line) {
        var length;
        if (typeSelect.check) {
            var coordinates = line.getCoordinates();
            length = 0;
            var sourceProj = map.getView().getProjection();
            for (var i = 0, ii = coordinates.length - 1; i < ii; ++i) {
                var c1 = ol.proj.transform(coordinates[i], sourceProj, 'EPSG:4326');
                var c2 = ol.proj.transform(coordinates[i + 1], sourceProj, 'EPSG:4326');
                length += wgs84Sphere.haversineDistance(c1, c2);
            }
        } else {
            var sourceProj = map.getView().getProjection();
            var geom = /** @type {ol.geom.Polygon} */(line.clone().transform(
                sourceProj, 'EPSG:3857'));
            length = Math.round(geom.getLength() * 100) / 100;
        }
        var output;
        if (length > 100) {
            output = (Math.round(length / 1000 * 100) / 100) +
                ' ' + 'km';
        } else {
            output = (Math.round(length * 100) / 100) +
                ' ' + 'm';
        }
        return output;
    };
    var formatArea = function (polygon, bo) {
        var area;
        if (typeSelect.check) {
            var sourceProj = map.getView().getProjection();
            var geom = (polygon.clone().transform(   /** @type {ol.geom.Polygon} */
                sourceProj, 'EPSG:4326'));
            var coordinates = geom.getLinearRing(0).getCoordinates();
            area = Math.abs(wgs84Sphere.geodesicArea(coordinates));
        } else {
            var sourceProj = map.getView().getProjection();
            var geom = (polygon.clone().transform(  /** @type {ol.geom.Polygon} */
                sourceProj, 'EPSG:3857'));
            if (bo) {
                area = geom.getRadiusSquared_() * 3.1415926;
            } else {
                area = geom.getArea();
            }
        }
        var output;
        if (area > 10000) {
            output = (Math.round(area / 1000000 * 100) / 100) +
                ' ' + 'km<sup>2</sup>';
        } else {
            output = (Math.round(area * 100) / 100) +
                ' ' + 'm<sup>2</sup>';
        }
        return output;
    };
    //创建一个关闭X按钮
    var popupcloser = document.createElement('a');
    popupcloser.href = 'javascript:void(0);';
    popupcloser.classList.add('ol-popup-closer');
    popupcloser.classList.add('ti-close');
    //主要函数
    function addInteraction() {
        //if (!!$(popupcloser)) {
        //    $(popupcloser).click();
        //}
        var value = typeSelect.value, type = 'LineString', geometryFunction, maxPoints;
        var switchType = {
            'point': function () {
                type = "Point";
            },
            'circle': function () {
                type = "Circle";
            },
            'box': function () {   //参考API的做法
                type = "LineString";
                maxPoints = 2;
                geometryFunction = function (coordinates, geometry) {
                    if (!geometry) {
                        geometry = new ol.geom.Polygon(null);
                    }
                    var start = coordinates[0];
                    var end = coordinates[1];
                    geometry.setCoordinates([
                      [start, [start[0], end[1]], end, [end[0], start[1]], start]
                    ]);
                    return geometry;
                };
            },
            'area': function () {
                type = "Polygon";

            },
            'line': function () {
                type = "LineString";
            },
            'square': function () {
                //type = "Circle";
                //geometryFunction = ol.interaction.Draw.createRegularPolygon(4);
                type = "LineString";
                maxPoints = 2;
                geometryFunction = function (coordinates, geometry) {
                    if (!geometry) {
                        geometry = new ol.geom.Polygon(null);
                    }
                    var start = coordinates[0];
                    var end = coordinates[1];
                    geometry.setCoordinates([
                      [start, [start[0], end[1]], end, [end[0], start[1]], start]
                    ]);
                    return geometry;
                };
            }
        };
        if (typeof switchType[value] !== 'function') {
            return;
        }
        switchType[value]();

        draw = new ol.interaction.Draw({
            geometryFunction: geometryFunction,
            maxPoints: maxPoints,
            source: source,
            type: type,
            style: new ol.style.Style({
                fill: new ol.style.Fill({
                    color: 'rgba(255, 255, 255, 0.2)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.5)',
                    lineDash: [10, 10],
                    width: 2
                }),
                image: new ol.style.Circle({
                    radius: 5,
                    stroke: new ol.style.Stroke({
                        color: 'rgba(0, 0, 0, 0.7)'
                    }),
                    fill: new ol.style.Fill({
                        color: 'rgba(255, 255, 255, 0.2)'
                    })
                })
            })
        });
        map.addInteraction(draw);
        if (!!measureTooltipElement && !!measureTooltipElement.parentNode) {
            measureTooltipElement.parentNode.removeChild(measureTooltipElement);
        }
        createMeasureTooltip();
        createHelpTooltip();
        var listener;


        draw.on('drawstart',
          function (evt) {
              $('.tooltip-map.tooltip-static').remove();
              createMeasureTooltip();
              //console.log('drawstart', evt.feature, evt.coordinate);
              sketch = evt.feature;
              /** @type {ol.Coordinate|undefined} */
              var tooltipCoord = evt.coordinate;

              listener = sketch.getGeometry().on('change', function (ev) {
                  try {
                      var geom = ev.target;
                      var output;
                      if (geom instanceof ol.geom.Polygon) {
                          output = formatArea(geom);
                          tooltipCoord = geom.getInteriorPoint().getCoordinates();
                      } else if (geom instanceof ol.geom.LineString) {
                          output = formatLength(geom);
                          tooltipCoord = geom.getLastCoordinate();

                      } else if (geom instanceof ol.geom.Circle) {
                          output = formatArea(geom,true);
                          tooltipCoord = geom.getCenter();
                      }
                      measureTooltipElement.innerHTML = output;
                      measureTooltip.setPosition(tooltipCoord);
                  } catch (e) {
                      map.removeInteraction(draw);
                  }
              });
              drawstart ? drawstart() : '';
          }, this);
        draw.on('drawend',
            function (evt) {
                if (typeSelect.value === 'circle') {
                    var centerPoint = evt.feature.getGeometry().getCenter();
                    var distance = evt.feature.getGeometry().getRadius();
                    _self.getExtent = centerPoint.concat(distance);
                }
                else {
                    _self.getExtent = evt.feature.getGeometry().getExtent();
                }
               // measureTooltipElement.appendChild(popupcloser);
                measureTooltipElement.className = 'tooltip-map tooltip-static';
                if (typeSelect.value !== 'point') {
                    measureTooltip.setOffset([0, -35]);
                }
                sketch = null;
                measureTooltipElement = null;
                createMeasureTooltip();
                ol.Observable.unByKey(listener);
                //map.removeInteraction(draw);
                drawend ? drawend(draw) : '';
            }, this);


    }

    function createHelpTooltip() {
        if (helpTooltipElement) {
            helpTooltipElement.parentNode.removeChild(helpTooltipElement);
        }
        helpTooltipElement = document.createElement('div');
        helpTooltipElement.className = 'tooltip-map hidden';
    }
    function createMeasureTooltip() {
        if (!!measureTooltipElement && !!measureTooltipElement.parentNode) {
            measureTooltipElement.parentNode.removeChild(measureTooltipElement);
        }
        measureTooltipElement = document.createElement('div');
        measureTooltipElement.className = 'tooltip-map tooltip-measure';
        measureTooltip = new ol.Overlay({
            element: measureTooltipElement,
            offset: [0, -35],//提示框偏移画线的距离(x,y)
            positioning: 'top-center'//提示框在画线的哪个方位
        });
        map.addOverlay(measureTooltip);
    }

    //clear  
    popupcloser.onclick = function (e) {
        _self.getExtent = [];
        map.getOverlays().clear();
        var features = vector.getSource().getFeatures();
        vector.getSource().clear(features);
    };
    addInteraction();
};
/**
 * Show the MeasureTool.
 */
ol.control.MeasureTool.prototype.showPanel = function () {
    if (this.element.className != this.shownClassName) {
        this.element.className = this.shownClassName;
    }
};

/**
 * Hide the MeasureTool.
 */
ol.control.MeasureTool.prototype.hidePanel = function () {
    if (this.element.className != this.hiddenClassName) {
        this.element.className = this.hiddenClassName;
    }
};

/**
 * Set the map instance the control is associated with.
 * @param {ol.Map} map The map instance.
 */
ol.control.MeasureTool.prototype.setMap = function (map) {
    // Clean up listeners associated with the previous map
    for (var i = 0, key; i < this.mapListeners.length; i++) {
        this.getMap().unByKey(this.mapListeners[i]);
    }
    this.mapListeners.length = 0;
    // Wire up listeners etc. and store reference to new map
    ol.control.Control.prototype.setMap.call(this, map);
    if (map) {
        var this_ = this;
        this.mapListeners.push(map.on('pointerdown', function () {
            this_.hidePanel();
        }));
    }
};

/**
 * Generate a UUID
 * @returns {String} UUID
 *
 * Adapted from http://stackoverflow.com/a/2117523/526860
 */
ol.control.MeasureTool.uuid = function () {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

/**
* @private
* @desc Apply workaround to enable scrolling of overflowing content within an
* element. Adapted from https://gist.github.com/chrismbarr/4107472
*/
ol.control.MeasureTool.enableTouchScroll_ = function (elm) {
    if (ol.control.MeasureTool.isTouchDevice_()) {
        var scrollStartPos = 0;
        elm.addEventListener("touchstart", function (event) {
            scrollStartPos = this.scrollTop + event.touches[0].pageY;
        }, false);
        elm.addEventListener("touchmove", function (event) {
            this.scrollTop = scrollStartPos - event.touches[0].pageY;
        }, false);
    }
};

/**
 * @private
 * @desc Determine if the current browser supports touch events. Adapted from
 * https://gist.github.com/chrismbarr/4107472
 */
ol.control.MeasureTool.isTouchDevice_ = function () {
    try {
        document.createEvent("TouchEvent");
        return true;
    } catch (e) {
        return false;
    }
};

/**
 * @private
 * @desc Determine if the current browser supports touch events. Adapted from
 */
ol.control.MeasureTool.prototype.getPoint = function (point) {

    return point;
}
