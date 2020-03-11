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
        name:"drawCLayerName",
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
            })
        })
    });

    ol.control.Control.call(this, {
        element: element,
    });

};

ol.inherits(ol.control.MeasureTool, ol.control.Control);

var draw; // global so we can remove it later
ol.control.MeasureTool.prototype.mapmeasure = function (typeSelect, callback) {
    if (draw) {
        map.removeInteraction(draw); //移除之前的绘制对象
    }
    var _self = this,
     source = _self.source,
     vector = _self.vector,
     //wgs84Sphere = new ol.Sphere(_self.sphereradius),
     sketch,
     helpTooltipElement,
     measureTooltipElement,
     measureTooltip;
    source.clear();  //一次只能画一个
    //map.getOverlays().clear();
    clearMeasureTooltip();//清除之前所画图形点tooltip
    _self.getExtent = [];

    var formatLength = function (line) {
        var length;
        if (typeSelect.check) {
            var coordinates = line.getCoordinates();
            length = 0;
            var sourceProj = map.getView().getProjection();
            for (var i = 0, ii = coordinates.length - 1; i < ii; ++i) {
                var c1 = ol.proj.transform(coordinates[i], sourceProj, 'EPSG:4326');
                var c2 = ol.proj.transform(coordinates[i + 1], sourceProj, 'EPSG:4326');
                length += ol.sphere.getDistance(c1, c2, 6378137);
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
            area = Math.abs(ol.sphere.getArea(geom));
        } else {
            var sourceProj = map.getView().getProjection();
            var geom = (polygon.clone().transform(  /** @type {ol.geom.Polygon} */
                sourceProj, 'EPSG:3857'));
            if (bo) {
                area =Math.pow(geom.getRadius(),2) * Math.PI;
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

    //主要函数
    function addInteraction() {
        var value = typeSelect.value, type = 'LineString', geometryFunction, maxPoints;
        var switchType = {
            'circle': function () {
                type = "Circle";
            },
            'box': function () {   //参考API的做法
                type = "LineString";
                maxPoints = 2;
                geometryFunction = function (coordinates, geometry) {
                  
                    var start = coordinates[0];
                    var end = coordinates[1];
                    if (!geometry) {
                        geometry = new ol.geom.Polygon([
                          [start, [start[0], end[1]], end, [end[0], start[1]], start]
                        ]);
                    } else {
                        geometry.setCoordinates([
                          [start, [start[0], end[1]], end, [end[0], start[1]], start]
                        ]);
                    }
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
                type = "Circle";
                geometryFunction = ol.interaction.Draw.createRegularPolygon(4);
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

        createMeasureTooltip();
        createHelpTooltip();
        var listener;

        //开始画形状
        draw.on('drawstart',
          function (evt) {
              sketch = evt.feature;
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
                          output = formatArea(geom, true);
                          tooltipCoord = geom.getCenter();
                      }
                      measureTooltipElement.innerHTML = output;
                      measureTooltip.setPosition(tooltipCoord);
                  } catch (e) {
                      map.removeInteraction(draw);
                  }
              });
          }, this);

        //形状结束
        draw.on('drawend',
            function (evt) {
                _self.getExtent.push(evt.feature.getGeometry().getExtent());
                measureTooltipElement.className = 'tooltip-map tooltip-static';
                measureTooltip.setOffset([0, -7]);
                sketch = null;
                measureTooltipElement = null;
                createMeasureTooltip();
                ol.Observable.unByKey(listener);
                if (!callback) {
                    return;
                } else {
                    if (typeof callback === "function") {
                        callback(evt.feature, draw);
                    }
                };
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
        if (measureTooltipElement) {
            measureTooltipElement.parentNode.removeChild(measureTooltipElement);
        }
        var timesTamp = Date.parse(new Date());
        var ID = "drawCV" + timesTamp;
        measureTooltipElement = document.createElement('div');
        measureTooltipElement.className = 'tooltip-map tooltip-measure';
        measureTooltip = new ol.Overlay({
            element: measureTooltipElement,
            offset: [0, -15],
            positioning: 'bottom-center',
            id: ID
        });
        map.addOverlay(measureTooltip);
    }

    function clearMeasureTooltip() {
        map.getOverlays().getArray().slice(0).forEach(function (overlay) {
            var layerId = overlay.getId();
            if (layerId && layerId.indexOf("drawCV") !== -1) {
                map.removeOverlay(overlay);
            }
        })
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
