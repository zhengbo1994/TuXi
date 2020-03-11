'use strict';
/** 
  * 地图控件
  * 
*/
app.directive('openMaps', ['$document', 'SweetAlert', '$timeout', '$state', '$filter', function ($document, SweetAlert, $timeout, $state, $filter) {
    /* ****载入图层的封装函数****
    * @param {obj}    map 自定义的基础地图全局变量
    * @param {obj}    topName 自定义的基础地图标注全局变量
    * @param {obj}    layer 自定义的GIS图层全局变量
    * @param {bool}   ck checkbox的bool值
    * @param {string} dataServerKey 请求package的key值.
    * @param {string} url URL address of tiles' provider.
    * @param {number} tileSize 瓦片大小.
    * @param {number} zeroLevelSize  零级大小.
    * @return {obj} return  layer 对象
    */
    var loadLayer = function (map, topName, layer, ck, dataServerKey, url, tileSize, zeroLevelSize) {
        if (!layer) {
            layer = new iTelluro().newItelluroLayer(dataServerKey, url, tileSize, zeroLevelSize);
            layer.setZIndex(22);
            map.addLayer(layer);
        }
        if (!ck) {
            map.removeLayer(layer);
            layer = null;
        }
        return layer;
    };
    return {
        restrict: 'E',
        template: function (el, attr) {
            var treeTpl = '', gisbtn = '', maptools = '', maptoolgroup = '';

            if (!!angular.isDefined(attr.gislayerable)) {
                treeTpl = '<div class="gislayerpop hide" style="min-width:150px;border-radius:5px;overflow-y:auto;background-color:#fff;position:absolute;right:10px;top:42px;z-index:999999;border:solid 1px rgba(0, 0, 0, 0.07);"><div style="padding:15px;border-bottom:1px solid rgba(0, 0, 0, 0.07);"><h4 style="margin-bottom: 0px;" >图层设置</h4></div><abn-tree ng-if="!dataset.gisLayer===false" tree-data="dataset.gisLayer" icon-leaf="ti-empty" on-checked="onChecked" expand-level="2"></abn-tree></div>';
                gisbtn = '<div class="map-btngroup-div"><span class="map_gislayer" tooltip="GIS图层" tooltip-append-to-body style="border-radius: 0;"><i class="ti-layers-alt"></i></span></div>' + '<b></b>';
            }

            if (!!angular.isDefined(attr.mapToolGroup)) {
                maptoolgroup = '    <div class="mapControllerBtn" ng-if="!!mapSearchTool" > ' +
                      '        <!--地图查询工具栏-->' + gisbtn +
                      '        <div class="map-btngroup-div" ng-click="measuremap(\'point\',\'map_point\')"><span class="map_point" style="border-radius: 0;"><i class="ti-hand-point-up"></i><span translate="maps.tools.point">点选</span></span></div>' +
                      '<b></b>'+
                      '        <div class="map-btngroup-div" ng-click="measuremap(\'square\',\'map_square\')"><span class="map_square" style="border-radius: 0;"><i class="mdi mdi-crop-square"></i>{{\'maps.tools.square\'|translate}}</span></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" ng-click="measuremap(\'circle\',\'map_circle\')"><span class="map_circle" style="border-radius: 0;"><i class="mdi mdi-checkbox-blank-circle-outline"></i>{{\'maps.tools.circle\'|translate}}</span></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" ng-click="clearMap(\'map_clear\')"><span class="map_clear" style="border-radius: 0;"><i class="ti-trash"></i>{{\'maps.tools.clean\'|translate}}</span></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" style="width: 111px;" ng-click="showExtraTool()">' +
                      '            <span class="map_wrench" style="border-radius: 0; border-right: none; padding-left: 5px;"><i class="mdi-wrench"></i><span translate="maps.tools.MAIN">工具</span></span><i ng-class="{\'mdi-chevron-down\':!isShowExtraTool,\'mdi-chevron-up\':isShowExtraTool}" style="color: #5b5b60;"></i>' +
                      '        </div>' +
                      '        <div ng-show="isShowExtraTool" style="position: absolute; width: 111px; top: 38px; right: 0; box-shadow: 1px 2px 1px rgba(0,0,0,.15); background-color: #fff;" >' +
                      '            <ul style="list-style: none; padding: 0; margin: 0;">' +
                      '            <li><div class="map-btngroup-div" ng-click="measuremap(\'line\',\'map_length\')" style="width: 100%;"><span class="map_length" style="border-radius: 0;"><i class="ti-ruler"></i><span translate="maps.tools.distance">距离</span></span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="measuremap(\'area\',\'map_area\')" style="width: 100%;"><span class="map_area" style="border-radius: 0;"><i class="ti-ruler-alt-2"></i><span translate="maps.tools.area">面积</span></span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="reset(\'map_reset\')" style="width: 100%;"><span class="map_reset" style="border-radius: 0;"><i class="ti-back-right"></i><span translate="maps.tools.reset">复位</span></span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="clickFull()" style="width: 100%;"><span class="map_fullScreen" style="border-radius: 0;"><i class="ti-fullscreen"></i><span translate="maps.tools.full">全屏</span></span></div></li>' +
                      '        </ul></div>' +
                      '    </div>' +
                      '    <div class="mapControllerBtn" ng-if="!!mapEditTool" > ' +
                      '        <!--地图编辑工具栏-->' +
                      '        <div class="map-btngroup-div" style="width: 105px;" ng-click="showEditTool(\'start\')">' +
                      '            <span class="map_wrench" style="border-radius: 0; border-right: none; padding-left: 5px;"><i class="mdi-wrench"></i>矢量编辑</span><i ng-class="{\'mdi-chevron-down\':!isShowEditTool.start,\'mdi-chevron-up\':isShowEditTool.start}" style="color: #5b5b60;"></i>' +
                      '        </div>' +
                      '        <div ng-show="isShowEditTool.start" style="position: absolute; width: 105px; top: 38px; left: 0; z-index: 1000; box-shadow: 1px 2px 1px rgba(0,0,0,.15); background-color: #fff;" >' +
                      '            <ul style="list-style: none; padding: 0; margin: 0;">' +
                      '            <li><div class="map-btngroup-div" ng-click="editLayerStart()" style="width: 100%;"><span class="map_length" style="border-radius: 0; padding-right: 9px;"><i class="mdi-book-open-page-variant"></i>开始编辑</span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="editLayerSave()" style="width: 100%;"><span class="map_area" style="border-radius: 0; padding-right: 9px;""><i class="mdi-content-save"></i>保存</span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="editLayerEnd()" style="width: 100%;"><span class="map_reset" style="border-radius: 0; padding-right: 9px;""><i class="mdi-book-variant"></i>结束编辑</span></div></li>' +
                      '        </ul></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" ng-click="clickFull()" ng-disabled="!isEditStart" style="width: 105px;"><span class="" style="border-radius: 0;"><i class="mdi-note-plus"></i>创建要素</span></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" style="width: 105px;" ng-click="showEditTool(\'edit\')" ng-disabled="!isEditStart">' +
                      '            <span class="map_wrench" style="border-radius: 0; border-right: none; padding-left: 5px;"><i class="mdi-pencil-box"></i>编辑要素</span><i ng-class="{\'mdi-chevron-down\':!isShowEditTool.edit,\'mdi-chevron-up\':isShowEditTool.edit}" style="color: #5b5b60;"></i>' +
                      '        </div>' +
                      '        <div ng-show="isShowEditTool.edit" style="position: absolute; width: 105px; top: 38px; left: 210px; z-index: 1000; box-shadow: 1px 2px 1px rgba(0,0,0,.15); background-color: #fff;" >' +
                      '            <ul style="list-style: none; padding: 0; margin: 0;">' +
                      '            <li><div class="map-btngroup-div" ng-click="clickFull()" style="width: 100%;"><span class="map_length" style="border-radius: 0; padding-right: 9px;"><i class="mdi-crop-rotate"></i>形变</span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="clickFull()" style="width: 100%;"><span class="map_area" style="border-radius: 0; padding-right: 9px;""><i class="mdi-image-filter-tilt-shift"></i>节点操作</span></div></li>' +
                      '        </ul></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" style="width: 105px;" ng-click="showEditTool(\'del\')" ng-disabled="!isEditStart">' +
                      '            <span class="map_wrench" style="border-radius: 0; border-right: none; padding-left: 5px;"><i class="mdi-delete"></i>删除要素</span><i ng-class="{\'mdi-chevron-down\':!isShowEditTool.del,\'mdi-chevron-up\':isShowEditTool.del}" style="color: #5b5b60;"></i>' +
                      '        </div>' +
                      '        <div ng-show="isShowEditTool.del" style="position: absolute; width: 105px; top: 38px; right: 80px; z-index: 1000; box-shadow: 1px 2px 1px rgba(0,0,0,.15); background-color: #fff;" >' +
                      '            <ul style="list-style: none; padding: 0; margin: 0;">' +
                      '            <li><div class="map-btngroup-div" ng-click="clickFull()" style="width: 100%;"><span class="map_length" style="border-radius: 0; padding-right: 9px;"><i class="mdi-cursor-default"></i>选择</span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="clickFull()" style="width: 100%;"><span class="map_area" style="border-radius: 0; padding-right: 9px;""><i class="mdi-delete-forever"></i>删除</span></div></li>' +
                      '        </ul></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" style="width: 111px;" ng-click="showEditTool(\'extra\')">' +
                      '            <span class="map_wrench" style="border-radius: 0; border-right: none; padding-left: 5px;"><i class="mdi-wrench"></i><span translate="maps.tools.MAIN">工具</span></span><i ng-class="{\'mdi-chevron-down\':!isShowEditTool.extra,\'mdi-chevron-up\':isShowEditTool.extra}" style="color: #5b5b60;"></i>' +
                      '        </div>' +
                      '        <div ng-show="isShowEditTool.extra" style="position: absolute; width: 111px; top: 38px; right: 0; box-shadow: 1px 2px 1px rgba(0,0,0,.15); background-color: #fff;" >' +
                      '            <ul style="list-style: none; padding: 0; margin: 0;">' +
                      '            <li><div class="map-btngroup-div" ng-click="measuremap(\'line\',\'map_length\')" style="width: 100%;"><span class="map_length" style="border-radius: 0;"><i class="ti-ruler"></i><span translate="maps.tools.distance">距离</span></span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="measuremap(\'area\',\'map_area\')" style="width: 100%;"><span class="map_area" style="border-radius: 0;"><i class="ti-ruler-alt-2"></i><span translate="maps.tools.area">面积</span></span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="reset(\'map_reset\')" style="width: 100%;"><span class="map_reset" style="border-radius: 0;"><i class="ti-back-right"></i><span translate="maps.tools.reset">复位</span></span></div></li>' +
                      '            <li><div class="map-btngroup-div" ng-click="clickFull()" style="width: 100%;"><span class="map_fullScreen" style="border-radius: 0;"><i class="ti-fullscreen"></i><span translate="maps.tools.full">全屏</span></span></div></li>' +
                      '        </ul></div>' +
                      '    </div>' +
                      '    <div class="mapControllerBtn" ng-if="!mapSearchTool&&!mapEditTool" > ' +
                      '        <!--地图一般工具栏-->' + gisbtn +
                      '        <div class="map-btngroup-div" ng-click="measuremap(\'line\',\'map_length\')"><span class="map_length" style="border-radius: 0;"><i class="ti-ruler"></i>{{\'maps.tools.distance\'|translate}}</span></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" ng-click="measuremap(\'area\',\'map_area\')"><span class="map_area" style="border-radius: 0;"><i class="ti-ruler-alt-2"></i>{{\'maps.tools.area\'|translate}}</span></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" ng-click="reset(\'map_reset\')"><span class="map_reset" style="border-radius: 0;"><i class="ti-back-right"></i>{{\'maps.tools.reset\'|translate}}</span></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" ng-click="clearMap(\'map_clear\')"><span class="map_clear" style="border-radius: 0;"><i class="ti-trash"></i>{{\'maps.tools.clean\'|translate}}</span></div>' +
                      '<b></b>' +
                      '        <div class="map-btngroup-div" ng-click="clickFull()"><span class="map_fullScreen" style="border-radius: 0;"><i class="ti-fullscreen"></i>{{\'maps.tools.full\'|translate}}</span></div>' +
                      '    </div>';
            }
            //地图上的显示模块
            var tep = '<div class="map_container">' + treeTpl +
                      '    <div class="mapDiv"></div> ' +
                      '    <div class="mapDivBtn"> ' +
                      '        <span ng-click="changeMap(true)">地图</span>' +
                      '        <span ng-click="changeMap(false)">卫星</span>' +
                      '    </div>' + maptoolgroup +
                      '    <div class="ol-unselectable ol-control"><a class="ol-zoom-in map-zoom-in"><i class="ti-plus"/></a><a tooltip-placement="bottom" class="ol-zoom-out map-zoom-out"><i class="ti-minus"/></a></div>' +
                      '</div>'
            return tep;
        },
        replace: true,
        scope: {
            dataset: "=?",      /*地图数据*/
            mapheight: "=?",    /*地图自定义高度*/
            mapwidth: "=?",    /*地图自定义宽度*/
            measuremap: '=?',   /*画方画圆方法*/
            clearDraw: '=?',    /*清除画方画圆留下的印迹*/
            measureBefore: '=?',/*画点之前执行的方法*/
            measureAfter: '=?',/*画点之后执行的方法*/
            getExtent: '=?',    /*坐标*/
            addTagging: '=?',   /*添加坐标标注type="MultiPolygon" ,"Point"*/
            addChart: '=?',     /*添加图表*/
            addMarker: '=?',    /*添加图片标注*/
            addLocation: '=?',   /*实现定位*/
            toLocation: '=?',    /*定位*/
            map: '=?',   /*map对象*/
            addLayer: '=?',   /*添加自定义图层，调用示例：addLayer(layer对象,[0,0,0,0])  return map*/
            removeLayer: '=?',   /*删除自定义图层，调用示例：addLayer(layer对象,)*/
            //isShowTools: '=?',    /*是否显示map工具栏*/
            mapSearchTool: '=?',    /*是否显示map查询工具栏*/
            mapEditTool: '=?',    /*是否显示map编辑工具栏*/
            mapEditFun: '=?',     /*图层编辑用到的方法集合*/
            isLine: '=?',    /*是否显示测量距离*/
            isArea: '=?',    /*是否显示测量面积*/
            isReset: '=?',    /*是否显示复位*/
            isClear: '=?',   /*是否显示清理图层*/
            clearAfter: '=?',/*清理图层之后执行的方法*/
            isFullscreen: '=?',    /*是否显示全屏*/
            isZoominorout: '=?',    /*是否显示缩放*/
            isLoadInternetTianDiTu: '=?'    /*是否加载外网天地图*/
        },
        link: function (scope, element, attr) {
            //全局变量
            var map, vector = new ol.layer.Vector(), topName, chkId, select, dragBox, selectedFeatures, MeasureTool, isAgain = false, vectorLayer, lyr = {}, typeSelect = {}, vectorSource, locationLayer;
            //console.log(scope.dataset);
            if (scope.isLoadInternetTianDiTu == -1) {
                //无底图
                scope.dataset = {
                    baseMap: { map: {}, note: {} },
                    statelliteMap: { map: {}, note: {} }
                };
            }
            var baseMap_map = scope.dataset ? scope.dataset.baseMap.map : {};
            var baseMap_note = scope.dataset ? scope.dataset.baseMap.note : {};
            var statellite_map = scope.dataset ? scope.dataset.statelliteMap.map : {};
            var statellite_note = scope.dataset ? scope.dataset.statelliteMap.note : {};
            if (!!angular.isDefined(attr.mapToolGroup) && angular.isDefined(attr.mapSearchTool)) {
                scope.isShowExtraTool = false;
            }
            //获取地图控件自定义的高度值
            var mapheight = angular.isUndefined(scope.mapheight) ? angular.element(window).height() - 145 : scope.mapheight;
            var mapwidth = angular.isUndefined(scope.mapwidth) ? angular.element(window).width() : scope.mapwidth;
            /*--公开的添加和删除自定义图层方法----------------start-*/
            scope.addLayer = function (layer, bounds) {
                if (map === undefined) {
                    return scope.addLayer(layer, bounds);
                }
                map.addLayer(layer);
                
                if (bounds && bounds.length === 4) {
                    bounds[0] = parseFloat(bounds[0]);
                    bounds[1] = parseFloat(bounds[1]);
                    bounds[2] = parseFloat(bounds[2]);
                    bounds[3] = parseFloat(bounds[3]);
                    map.getView().fit(bounds, map.getSize());
                }
                return map;
            };
            scope.removeLayer = function (layer) {
                map.removeLayer(layer);
            };
            /*--公开的添加和删除自定义图层方法----------------end-*/

            typeSelect.check = false;

            //用来解决用户地图框选按钮重复点击事件
            scope.isAgain = false;

            //设置地图容器
            var mapDiv = element.find("div.mapDiv");
            var tree = element.find("div.gislayerpop");

            //动态的设置地图的高度
            scope.$watch('mapheight', function (val, old) {
                if (scope.isMapFullScreen !== 1) {
                    //console.log(val, old);
                    mapheight = val;
                    mapDiv.css({ height: val });
                    tree.css({ maxHeight: val - 50 });
                }
            });
            scope.$watch('mapwidth', function (val, old) {
                mapwidth = !!val ? val : angular.element(window).width();
                mapDiv.css({ width: mapwidth });
            });
            mapDiv.css({ height: mapheight, width: mapwidth });
            tree.css({ maxHeight: mapheight - 50 });
            
            /** 1.设置起始显示视图  **/
            function loadMap() {
                //console.log(baseMap_map);
                if (map != undefined) {
                    var layers = map.getLayers().array_;
                    angular.forEach(layers, function (e) {
                        map.removeLayer(e);
                    });
                };
                //起始底图
                var zjLayer;
                if (!!scope.isLoadInternetTianDiTu) {
                    //外网天地图
                    map = new TianDiTu().NewWebTilesMap(mapDiv[0], 20, 180, -180, -90, 90, 'vec_c', 'http://t0.tianditu.com/DataServer', 256, 36, 'baseLayer');
                    zjLayer = new TianDiTu().newWebVectorLabelLayer(null, 'zjLayer');
                } else {
                    map = NewWebTilesMap(mapDiv[0], baseMap_map.zoomlevel, 180, -180, -90, 90, baseMap_map.dataserverkey, baseMap_map.url, baseMap_map.tilesize, baseMap_map.zerolevelsize, 'baseLayer');
                    //加注记
                    zjLayer = new iTelluro().newItelluroLayer(baseMap_note.dataserverkey, baseMap_note.url, baseMap_note.tilesize, baseMap_note.zerolevelsize, 'zjLayer');
                }
                
                zjLayer.setZIndex(18);
                map.addLayer(zjLayer);
                changeColor(0, 1);
                initMap();
                scope.map = map;
            };
            /********10.读取后台地图数据******************/
            if (!scope.dataset) {
                
            } else {
                loadMap();
            };

            /** 2.添加地图上的一些控件  **/
            function initMap() {
                map.getView().setCenter([94, 30]);
                //2.1显示坐标
                map.addControl(new ol.control.MousePosition({
                    className: 'map_mouse-position',
                    undefinedHTML: '',
                    projection: 'EPSG:4326',
                    coordinateFormat: function (coordinate) {
                        return ol.coordinate.format(coordinate, '{x}, {y}', 4);
                    }
                }));
                
                $timeout(function () { //2.2添加全屏控件
                    map.addControl(new ol.control.FullScreen(
                        {
                            element: element.find("span.map_fullScreen")[0],
                            exitFullScreenBackFun: function () {
                                mapDiv.css({ height: mapheight });
                            }
                        }
                     ));
                }, 500);

                //2.3添加地图缩放
                if (scope.isZoominorout) {
                    map.addControl(new ol.control.Zoom({ element: { zoomIn: element.find("a.map-zoom-in")[0], zoomOut: element.find("a.map-zoom-out")[0] } }));
                }
                //2.4比例尺
                map.addControl(new ol.control.ScaleLine());
                //2.5调用工具（画圆|画方|画多边形)
                MeasureTool = new ol.control.MeasureTool();
                MeasureTool.vector.setZIndex(30);
                map.addLayer(MeasureTool.vector);
                map.addControl(MeasureTool);
            };
            /** 3.图层切换------remove掉以前的layer设置新的layer **/
            scope.changeMap = function (flag) {
                if (flag) {
                    changeColor(0, 1);
                    setLayer(baseMap_map, baseMap_note);
                } else {
                    changeColor(1, 0);
                    setLayer(statellite_map, statellite_note);
                }
            };
            /** 4.图层切换具体实现------remove掉以前的layer设置新的layer **/
            function setLayer(mapdata, notedata) {
                var newBaseLayer, newZjLayer;
                removeAllLayer({ base: "baseLayer", zj: "zjLayer" });
                if (!!scope.isLoadInternetTianDiTu) {
                    //外网天地图
                    newBaseLayer = new TianDiTu().newWebImageLayer(null, 'baseLayer');
                    newZjLayer = new TianDiTu().newWebImageLabelLayer(null, 'zjLayer');
                } else {
                    //底图
                    newBaseLayer = new iTelluro().newItelluroLayer(mapdata.dataserverkey, mapdata.url, mapdata.tilesize, mapdata.zerolevelsize, 'baseLayer');
                    //加注记   
                    newZjLayer = new iTelluro().newItelluroLayer(notedata.dataserverkey, notedata.url, notedata.tilesize, notedata.zerolevelsize, 'zjLayer');
                }

                newBaseLayer.setZIndex(0);
                map.addLayer(newBaseLayer);

                newZjLayer.setZIndex(18);
                map.addLayer(newZjLayer);
            };
            /** 5.根据name值切换移除底图 **/
            function removeAllLayer(obj) {
                var arr = map.getLayers().array_;
                var len = arr.length;
                for (var i = 0; i < len; i++) {
                    if (!arr[i]) { return; }
                    if (arr[i].get("name") === obj.base || arr[i].get("name") === obj.zj)
                        map.removeLayer(arr[i]);
                }
            };

            /** 6.加marker图片标注根据一组坐标和marker的图片地址来显示 **/
            scope.addMarker = function (markerArr) {
                var features = [];
                var url = markerArr[0].url;

                //第三步source添加到layer
                var markerLayer = new ol.layer.Vector({
                    source: new ol.source.Vector()
                });
                //第一步设置style
                var style = new ol.style.Style({
                    image: new ol.style.Icon({
                        src: url,
                        anchor: [1, 1]
                    })
                });
                //第二部创建featuare并设置好在地图上的位置及样式
                angular.forEach(markerArr, function (data, index) {
                    var feature = new ol.Feature({
                        geometry: new ol.geom.Point(data.location)
                    });
                    feature.obj = { "id": index };
                    //设置样式在样式中既可以设置图标
                    feature.setStyle(style);
                    //添加到之前创建的layer中
                    features.push(feature);
                    //第四步添加feature到对应layer的Source中
                    markerLayer.getSource().addFeature(feature);
                    markerLayer.setZIndex(20);
                });
                //第五步 layer添加到map
                map.addLayer(markerLayer);
            };

            /** 7./地图定位---给设置了一个单独的矢量图层-locationLayer 清除的时候主要清除它 **/
            scope.addLocation = function (locationArr) {
                removeMarker();//清除上一次的位置
                //第一步设置style
                var style = new ol.style.Style({
                    image: new ol.style.Icon({
                        src: locationArr[0].url,
                        anchor: [1, 1]
                    })
                });
                //第二部创建featuare并设置好在地图上的位置及样式。
                var feature = new ol.Feature({
                    geometry: new ol.geom.Point([locationArr[0].longitude, locationArr[0].latitude]),
                });
                feature.setStyle(style);

                //第四步source添加到layer
                locationLayer = new ol.layer.Vector({
                    source: new ol.source.Vector()
                });
                //第三步添加feature到Source中
                locationLayer.getSource().addFeature(feature);
                locationLayer.setZIndex(20);
                //第五步layer添加到map中
                map.addLayer(locationLayer);
                //设置中心点
                map.getView().setCenter([locationArr[0].longitude, locationArr[0].latitude]);
            };
            /** 8.监听地图点击事件 **/
            map.on("click", function (evt) {
                var pixel = map.getEventPixel(evt.originalEvent);
                var feature = map.forEachFeatureAtPixel(pixel, function (feature) {
                    return feature;
                });
                var coordinate = evt.coordinate;
                if (feature && feature.obj) {
                    var popupElement = '<div class="popup" style="width:300px;height:300px;background:#fff;border-radius:10px;position:relative;top:20px;left:10px;">' +
               '    <div class="popup-header" style="display:flex;height:40px;line-height:40px;padding:10px 0 20px 10px;border-bottom:1px solid #E5E5E5;"><h3 style="font-size:15px;width:90%;height:100%;">灾害点信息</h3><a href="#" style="width:10%;height:30px;line-height:30px;display:block;text-align:center;" class="ol-popup-closer"><i class="ti-close" tooltip="关闭"></i></a></div>' +
               '    <div class="popup-content" style="margin:10px 0 0 0;">' +
               '        <div style="border-bottom:1px solid #E5E5E5;width:90%;height:40px;line-height:40px;margin:5px auto;"><span>监测设备名称 :</span><span>栖霞翠屏街道黄燕底崩塌隐患</span></div>' +
               '        <div style="border-bottom:1px solid #E5E5E5;width:90%;height:40px;line-height:40px;margin:5px auto;"><span>监测设备编号 :</span><span>' + feature.obj.id + '</span></div>' +
               '        <div style="border-bottom:1px solid #E5E5E5;width:90%;height:40px;line-height:40px;margin:5px auto;"><span>经度:</span><span>120.87255556</span></div>' +
               '        <div style="border-bottom:1px solid #E5E5E5;width:90%;height:40px;line-height:40px;margin:5px auto;"><span>纬度:</span><span>37.26122222</span></div>' +
               '    </div>' +
                '   <div class="disInfo" style="width:40%;height:30px;line-height:30px;text-align:center;margin:10px 0 0 20px;border:1px solid #E5E5E5;"><a  href="javascript:void(0)" style="text-decoration: none;" class="popup-info">灾害点详细信息</a></div>' +
               '</div>';
                    var popup_element = angular.element(popupElement)[0];
                    var info_popup = new ol.Overlay(({
                        element: popup_element,
                        autoPan: true,
                        autoPanAnimation: {
                            duration: 250
                        }
                    }));
                    map.addOverlay(info_popup);
                    info_popup.setPosition(coordinate);
                    angular.element(".popup").find(".ol-popup-closer").on('click', function (evt) {
                        evt.preventDefault();
                        info_popup.setPosition(undefined);
                    });
                    angular.element(".popup").find(".popup-info").on('click', function (evt) {
                        $state.go("appPages.secondDetail.page", {});
                    });
                } else {
                    return;
                }
            });
            //移除图层上所有覆盖物
            function removeAllOverlay(map) {
                map.getOverlays().getArray().slice(0).forEach(function (overlay) {
                    map.removeOverlay(overlay);
                })
            };
            //清除定位的marker
            function removeMarker() {
                if (locationLayer) {
                    locationLayer.getSource().clear(locationLayer.getSource().getFeatures());
                }
            };
            /** 8.地图上加图表 **/
            scope.addChart = function (option, position) {
                removeAllOverlay(map);
                var chartHtmlTemplate =
                   '<div class="popup" style="width:400px;height:300px;background:#fff;border-radius:10px;padding:10px;box-shdow:10px 5px #eee;">' +
                   '<div style="position:absolute;top:-20px;left:-20px;width:0px;height:0px;border:20px solid transparent;border-bottom-color:red;transform:rotate(-45deg);"></div>' +
                   '    <div id="popupContent" style="width:400px;height:300px;">' +
                   '    </div>' +
                   '</div>';
                var popup_element = angular.element(chartHtmlTemplate);
                var popupContent = popup_element.find('#popupContent');

                var myChart = echarts.init(popupContent[0]);

                var chartPopup = new ol.Overlay(({
                    element: popup_element[0],
                    autoPan: true,
                    positioning: 'top-left',
                    autoPanAnimation: {
                        duration: 250
                    }
                }));
                chartPopup.setPosition(position);
                map.addOverlay(chartPopup);
                var chartOption = {};
                for (var key in option) {
                    chartOption[key] = option[key];
                }
                myChart.setOption(chartOption);
            };

            for (var i in lyr) {
                if (!lyr[i]) {
                    continue;
                }
                if (chkId !== i) {
                    map.addLayer(lyr[i]);
                }
            }
            if (lyr[chkId]) {
                map.addLayer(lyr[chkId]);
            }
            //操作树
            scope.onChecked = function (node) {
                if (!node.isLeaf) {
                    recursions(node.children);
                } else {
                    chkId = node.uid;
                    lyr[node.uid] = loadLayer(map, topName, lyr[node.uid], node.checked, node.dataserverkey, node.url, node.tilesize, node.zerolevelsize);
                }
            };
            //递归
            function recursions(arr) {
                for (var i in arr) {
                    if (!arr[i].isLeaf) {
                        recursions(arr[i].children);
                    } else {
                        chkId = arr[i].uid;
                        lyr[arr[i].uid] = loadLayer(map, topName, lyr[arr[i].uid], arr[i].checked, arr[i].dataserverkey, arr[i].url, arr[i].tilesize, arr[i].zerolevelsize);
                    }
                }
            };
            
            //距离测量|面积测量|画圆|画方|画点
            scope.measuremap = function (measure, className) {
                var spanobj = element.find("div.mapControllerBtn").find("span." + className);

                if (spanobj.css('color') !== 'rgb(0, 122, 255)') {
                    if ((measure === "point" || measure === "square" || measure === "circle") && typeof (scope.measureBefore) == 'function') {
                        if (!scope.measureBefore()) {
                            return;
                        }
                    }

                    //确定当前操作目标
                    scope.clearDraw();
                    typeSelect.value = measure;
                    MeasureTool.mapmeasure(typeSelect, function () {
                        if (measure === "point") {
                            if (typeof (scope.measureBefore) == 'function') {
                                if (!scope.measureBefore()) {
                                    return;
                                }
                            }
                            removeAllOverlay(map);
                            removeMarker();
                            clearMeasureVector();
                        }
                        else if (measure === "square" || measure === "circle") {
                            if (typeof (scope.measureBefore) == 'function') {
                                if (!scope.measureBefore()) {
                                    return;
                                }
                            }
                            //removeAllOverlay(map);
                            removeMarker();
                            clearMeasureVector();
                        }
                    }, function (draw) {
                        scope.drawOpt = draw;

                        //双向绑定赋值
                        scope.getExtent = MeasureTool.getExtent;
                        //console.log(scope.getExtent);

                        if ((measure === "point" || measure === "square" || measure === "circle") && typeof (scope.measureAfter) == 'function') {
                            scope.measureAfter(scope.getExtent, measure);
                        }
                    });
                    if (!className) return;
                    currentState(className);
                } else {
                    spanobj.css("color", "#5b5b60");
                    spanobj.find("i").css("color", "#5b5b60");
                    if (!!scope.drawOpt) {
                        map.removeInteraction(scope.drawOpt);
                    }
                    removeAllOverlay(map);
                    removeMarker();
                    clearMeasureVector();
                    map.removeLayer(vectorLayer);
                    if (typeof (scope.clearAfter) == 'function') {
                        scope.clearAfter();
                    }
                }

                if (!(measure === "point" || measure === "square" || measure === "circle")) {
                    if (typeof (scope.clearAfter) == 'function') {
                        scope.clearAfter();
                    }
                }
                scope.isShowExtraTool = false;
            };
            function clearMeasureVector() {
                var maplayers = map.getLayers().array_;
                angular.forEach(maplayers, function (obj) {
                    //console.log(obj);
                    if (obj.getZIndex() === 30) {
                        obj.getSource().clear();
                    }
                });
            };
            //清空图层
            scope.clearDraw = function () {
                //var features = MeasureTool.vector.getSource().getFeatures();
                //MeasureTool.vector.getSource().clear(features);
                removeAllOverlay(map);
                removeMarker();
                clearMeasureVector();
            };
            //地图按钮样式切换实现
            function changeColor(index0, index1) {
                element.find(".mapDivBtn").find("span").eq(index0).addClass("map_active");
                element.find(".mapDivBtn").find("span").eq(index1).removeClass("map_active");
            }
            //标注当前界面状态
            function currentState(className) {
                var spanobj = element.find("div.mapControllerBtn").find("span." + className);
                element.find("div.mapControllerBtn").find("span").css("color", "#5b5b60");
                element.find("div.mapControllerBtn").find("i").css("color", "#5b5b60");
                spanobj.css("color", "#007AFF");
                spanobj.find("i").css("color", "#007AFF");
            }
            //地图上清理图层：
            scope.clearMap = function (className) {
                if (!!scope.drawOpt) {
                    map.removeInteraction(scope.drawOpt);
                }
                currentState(className);
                removeAllOverlay(map);
                removeMarker();
                clearMeasureVector();
                map.removeLayer(vectorLayer);
                if (typeof (scope.clearAfter) == 'function') {
                    scope.clearAfter();
                }
                scope.isShowExtraTool = false;
            }

            //------------------------------------------图层编辑------------------------------
            scope.isShowEditTool = {
                start: false,
                create: false,
                edit: false,
                del: false,
                extra: false
            }
            scope.isEditStart = false;
            //展示地图编辑工具栏
            scope.showEditTool = function (tools) {
                for (var i in scope.isShowEditTool) {
                    if (tools === i) {
                        scope.isShowEditTool[i] = !scope.isShowEditTool[i];
                    }
                    else {
                        scope.isShowEditTool[i] = false;
                    }
                }
            }
            //开始编辑
            scope.editLayerStart = function () {
                if (!scope.isEditStart) {
                    scope.isEditStart = true;
                    if (angular.isDefined(scope.mapEditFun)) {
                        scope.mapEditFun.editStart();
                    }
                }
            }
            //结束编辑
            scope.editLayerEnd = function () {
                if (!!scope.isEditStart) {
                    scope.isEditStart = false;
                    if (angular.isDefined(scope.mapEditFun)) {
                        scope.mapEditFun.editEnd();
                    }
                }
            }
            //保存编辑
            scope.editLayerSave = function () {
                if (angular.isDefined(scope.mapEditFun)) {
                    scope.mapEditFun.editSave();
                }
            }

            //------------------------------------------图层编辑------------------------------


            //展示地图一般工具栏
            scope.showExtraTool = function () {
                scope.isShowExtraTool = !scope.isShowExtraTool;
            }
            //复位
            scope.reset = function (cls) {
                //console.log(scope.getExtent);
                if (!!scope.getExtent && scope.getExtent.length > 1) {
                    map.getView().setCenter(scope.getExtent);
                }
                else {
                    map.getView().setCenter([116.4, 39.9]);
                }
                currentState(cls);
                if (typeof (scope.clearAfter) == 'function') {
                    scope.clearAfter();
                }
                scope.isShowExtraTool = false;
            };
            //定位
            scope.toLocation = function (arr) {
                if (!!arr && arr.length === 2) {
                    map.getView().setCenter(arr);
                }
            }

            //解决全屏BUG
            scope.isMapFullScreen = 0;
            scope.clickFull = function () {
                //mapheight = element.find("div.mapDiv")[0].offsetHeight;
                mapDiv.css({ height: '100%' });
                scope.isMapFullScreen = 1;
                scope.isShowExtraTool = false;
            };

            scope.$watch(isFullScreen, function (newValue, oldValue) {
                //console.log(newValue, oldValue);
                if (newValue == 0 && oldValue == 1) {
                    scope.isMapFullScreen = 0;
                    scope.mapheight = mapheight;
                    //console.log(mapheight, scope.mapheight);
                }
            });

            //判断地图是否全屏
            function isFullScreen() {
                if (!!scope.isMapFullScreen) {
                    var explorer = window.navigator.userAgent.toLowerCase();
                    if (explorer.indexOf('chrome') > -1) {
                        if (document.body.scrollHeight === window.screen.height && document.body.scrollWidth === window.screen.width) {
                            return 1;
                        } else {
                            return 0;
                        }
                    }
                    else {
                        if (window.outerHeight === screen.height && window.outerWidth === screen.width) {
                            return 1;
                        } else {
                            return 0;
                        }
                    }
                }
                return 0;
            }

            //拐点坐标标注
            scope.addTagging = function (type, coordinates, color, bordercolor) {
                var geometry;

                if (type === 'MultiPolygon') {
                    var tmpObj = new ol.format.WKT();
                    geometry = tmpObj.readGeometry(coordinates);
                } else if (type === 'MultiLineString') {
                    geometry = new ol.geom.MultiLineString(coordinates);
                } else if (type === 'Point') {
                    geometry = new ol.geom.Point(coordinates);
                } else { return; }

                vectorSource = new ol.source.Vector({});

                vectorLayer = new ol.layer.Vector({
                    source: vectorSource
                });
                vectorLayer.setZIndex(40);
                map.addLayer(vectorLayer);

                var iconFeature = new ol.Feature({
                    geometry: geometry,
                    name: '拐点坐标标注',
                    population: 4000,
                    rainfall: 500
                }),
                iconStyle = new ol.style.Style({
                    fill: new ol.style.Fill({
                        color: color ? color : 'rgba(252, 216, 107, 0.35)'
                    }),
                    stroke: new ol.style.Stroke({
                        color: bordercolor ? bordercolor : '#ffcc33',
                        width: 2
                    }),
                    image: new ol.style.Circle({
                        radius: 7,
                        fill: new ol.style.Fill({
                            color: color ? color : '#ffcc33'
                        })
                    })
                });
                iconFeature.setStyle(iconStyle);
                vectorSource.addFeature(iconFeature);
                return vectorLayer;
            };

            /********树的显示与隐藏******************/
            var isAncestorOrSelf = function (element, target) {
                var parent = element;
                while (parent.length > 0) {
                    if (parent[0] === target[0]) {
                        return true;
                    }
                    parent = parent.parent();
                }
                return false;
            };
            var closeOnOuterClicks = function (e) {
                var elem = element.find('div.gislayerpop');
                if (angular.element(e.target).hasClass('map_gislayer') || angular.element(e.target).parent().hasClass('map_gislayer')) {
                    elem.removeClass('hide');
                    currentState('map_gislayer');
                    return;
                }
                if (!isAncestorOrSelf(angular.element(e.target), elem)) {
                    elem.addClass('hide');
                }
            };
            if (angular.isDefined(attr.gislayerable)) {
                $document.on('click', closeOnOuterClicks);
            }
            scope.$on('$destroy', function () {
                $document.off('click', closeOnOuterClicks);
            });
        }
    };
}]);