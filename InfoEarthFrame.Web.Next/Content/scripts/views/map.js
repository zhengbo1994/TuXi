function initMap(target){
    var layerObj = {};
    var baseMap = [
        new ol.layer.Tile({
            source:new ol.source.XYZ({
                url:"http://t3.tianditu.com/DataServer?T=vec_w&x={x}&y={y}&l={z}&tk=7dbe9676a47872a92d1d86cc7c71f440"
            })
        }),
        new ol.layer.Tile({
            source:new ol.source.XYZ({
                url: "http://t3.tianditu.com/DataServer?T=cva_w&x={x}&y={y}&l={z}&tk=7dbe9676a47872a92d1d86cc7c71f440"
            })
        })
    ];
    var map = new ol.Map({
        target: target,
        layers: baseMap,
        view: new ol.View({
            center: [120.97, 30.59],
            minZoom: 2,
            zoom: 4,
            maxZoom: 14,
            projection: 'EPSG:4326',
        })
    });

    // 实例化比例尺控件
    var scaleLineControl = new ol.control.ScaleLine({
        // 设置比例尺单位为degrees、imperial、us、nautical或metric（度量单位）
        units: "metric"
    });
    map.addControl(scaleLineControl);

    var mousePosition = function (targetClass, targetId) {
        targetClass = targetClass ? targetClass : 'custom-mouse-position';
        targetId = targetId ? targetId : 'mouse-position';
        /*鼠标控件,鼠标移动显示坐标*/
        var mousePositionControl = new ol.control.MousePosition({
            coordinateFormat: ol.coordinate.createStringXY(2),
            projection: "EPSG:4326",
            className: targetClass,
            target: document.getElementById(targetId),
            undefinedHTML: "&nbsp"
        });
        map.addControl(mousePositionControl);
    }

    var addLayer = function(){
        var _iTelluroLayer = function (url,title, zeroLevelSize, tileSize) {
            tileSize = tileSize ? tileSize : 512;
            function tileUrlFunctionCallBack(tileCoord) {
                var z = tileCoord[0];
                var x = tileCoord[1];
                var y = tileCoord[2];

                var xStr4 = "0000" + x.toString();
                xStr4 = xStr4.substr(xStr4.length - 4, 4);
                var yStr4 = "0000" + y.toString();
                yStr4 = yStr4.substr(yStr4.length - 4, 4);

                var url = urlTemplate.replace('{z}', z.toString())
                    .replace('{y}', y)
                    .replace('{x}', x);
                return url
            }
            var urlTemplate = url+'?X={x}&Y={y}&L={z}&T='+title;
            var resolutions = new Array(22);
            for (var i = 0, ii = resolutions.length; i < ii; ++i) {
                resolutions[i] = zeroLevelSize / Math.pow(2, i) / tileSize;
            }
            var layer = new ol.layer.Tile({
                source: new ol.source.XYZ({
                    tileSize: tileSize,
                    tileUrlFunction: tileUrlFunctionCallBack,
                    projection: 'EPSG:4326',
                    tileGrid: new ol.tilegrid.TileGrid({
                        resolutions: resolutions,
                        tileSize: tileSize,
                        origin: [-180, -90]
                    })
                })
            });
            return layer;
        }


      var newTileByWMS = function (urlTemplate, sld, format, layers, layerName) {
            var layer = new ol.layer.Tile({
                source: new ol.source.TileWMS({
                    url: urlTemplate,
                    params: {
                        FORMAT: format,
                        LAYERS: layers,
                        // SLD_BODY: '',
                        //SLD: "http://tiger_roads.sld",
                        //SLD_BODY: sld,
                        STYLES:sld?sld:'',
                        TILED: true
                    },
                    serverType: 'geoserver'
                }),
                name: !!layerName ? layerName : ''
            });
            return layer;
        }


      function iTelluro(url, mapName, zeroLevelSize, serviceType) {
     
            if (!layerObj[mapName]) {
                if ((serviceType || '').toLowerCase() == "geoserver") {
                    layerObj[mapName] = newTileByWMS(url, '', "image/png", mapName, mapName)
                }
                else {
                     layerObj[mapName] = _iTelluroLayer(url, mapName, zeroLevelSize)
                }
                map.addLayer(layerObj[mapName]);
            }
        }

        return {
            iTelluro:iTelluro
        }
    }();

    var removeLayer = function (mapName) {
        if (mapName && layerObj[mapName]) {
        map.removeLayer(layerObj[mapName]);
        layerObj[mapName] = null;
        }
    }

    var interaction = function(){
        // var collection = new ol.Collection()
        var featureOverlay = new ol.layer.Vector({
            source: new ol.source.Vector({wrapX:false}),
            style: new ol.style.Style({
                fill: new ol.style.Fill({
                    color: 'rgba(255, 255, 255, 0.2)'
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
        featureOverlay.setMap(map);//添加在地图上的临时图层

        var drawFeature = null, modifyFeature = null, selectFeature = null;
        function draw(type){
            removeDraw();
            drawFeature = new ol.interaction.Draw({
                source:featureOverlay.getSource(),
                type:type
            });
            // drawFeature.on('drawend', function(){
            //     map.removeInteraction(drawFeature)
            // })
            map.addInteraction(drawFeature);

        }
        function removeDraw(){
            removeModify();
            removeSelect();
            map.removeInteraction(drawFeature);
        }


        function modify(){
            removeModify();
            modifyFeature = new ol.interaction.Modify({
                features: selectFeature.getFeatures(),
                deleteCondition: function(event) {
                    return  ol.events.condition.singleClick(event);
                }
            })
            map.addInteraction(modifyFeature);
        }
        function removeModify(){
            map.removeInteraction(modifyFeature);
        }

        function select(){
            removeDraw();
            removeSelect();
            selectFeature = new ol.interaction.Select({wrapX: false});
            map.addInteraction(selectFeature);
            selectFeature.on('select', function(e) {console.log(7777,e.target.getFeatures().getArray())
                // e.target.getFeatures().forEach(fea=>{

                //     console.log(fea.getGeometry().getCoordinates() );

                // })

            });
            modify();
        }
        function removeSelect(){
            map.removeInteraction(selectFeature);
        }

        return {
            draw:draw,
            removeDraw:removeDraw,
            modify: modify,
            removeModify:removeModify,
            select:select,
            removeSelect: removeSelect
        }
    }()

    return {
        map: map,
        mousePosition:mousePosition,
        addLayer:addLayer,
        removeLayer:removeLayer,
        interaction:interaction
    }
}