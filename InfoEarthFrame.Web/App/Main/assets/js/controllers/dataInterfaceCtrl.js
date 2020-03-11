'use strict';
/**
 * testPage2Ctrl Controller
 */
app.controller('dataInterfaceCtrl', ['$rootScope', '$scope', 'SweetAlert', '$element', 'selfadapt', '$timeout', 'abp.services.app.map', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.setSys', 'abp.services.app.serverInterface',
   function ($rootScope, $scope, SweetAlert, $element, selfadapt, $timeout, mapSearch, dataTag, dataType, setSys, serverInterface) {
       $rootScope.loginOut();
       $rootScope.homepageStyle = {};
       //调用实时随窗口高度的变化而改变页面内容高度的服务
       $scope.globalobj = $element;
       var unlink = selfadapt.anyChange($element);
       $scope.$on('$destroy', function () {
           unlink();
           selfadapt.showBodyScroll();
       });
       //图层的标签、分类的TYPE
       var overAllId = 'ba5ab799-6acd-11e7-87f3-005056bb1c7e';

       var TypeDto = {
           Id: '',
           TypeName: '',
           TypeDesc: '',
           DictCodeID: '',
           ParentID: ''
       };
       var TagDto = {
           Id: '',
           TagName: '',
           TagDesc: '',
           DictCodeID: ''
       };
       //图层类型查询的ID
       var layerDataTypeID = "1cfe51dd-67a3-11e7-8eb2-005056bb1c7e";
       //空间参考查询的ID
       var refTypeID = "2cf1d4ba-67ab-11e7-8eb2-005056bb1c7e";
       //数据类型列表查询ID
       var dataTableID = "73160096-67a5-11e7-8eb2-005056bb1c7e";

       /*--------------页面左边树形--------------start----*/
       //分类查询树
       $scope.typeTreeData = [{
           label: '全部',
           children: [],
           id: ''
       }];

       //当前节点,点击树时自动赋值
       $scope.selectedType1 = "";

       //选中树节点回调函数
       $scope.onTypeSelected1 = function (node) {
           var str = node.label === '全部' ? '' : node.label;
           getLayerDataBy('', str, null);
       };




       //分类查询树的数据
       dataType.getAllListByDataType(overAllId).success(function (data, statues) {
           var arr = [];
           for (var i in data.items) {
               var tempTypeData = {};
               tempTypeData.id = data.items[i].id;
               tempTypeData.dictCodeID = data.items[i].dictCodeID;
               tempTypeData.label = data.items[i].typeName;
               tempTypeData.typeDesc = data.items[i].typeDesc;
               tempTypeData.parentID = data.items[i].parentID;
               tempTypeData.children = [];
               arr = arr.concat(tempTypeData);
           }
           arr.forEach(function (each) {
               if (each.parentID == 0) {
                   $scope.typeTreeData[0].children.push(each);
               }
           });
           $scope.typeTreeData[0].children.forEach(function (each) {
               each.children = [];
               arr.forEach(function (item) {
                   if (item.parentID == each.id) {
                       each.children.push(item);
                   }
               })
           });
       });

       /*--------------页面左边树形--------------end----*/

       /*--------------页面中间树形--------------start----*/

       //服务查询树
       $scope.typeTreeData2 = [];


       //根据条件查询图层数据
       function getLayerDataBy(mapName, typeName, tagName) {
           mapSearch.getAllListByName({ MapName: mapName, MapType: typeName, MapTag: tagName }).success(function (data, statues) {
               var dt = [];
               if (!!data) {
                   dt = data.items;
               };
               $scope.serviceList = dt;
               $scope.state = null;
               $scope.mapSerShow(0, dt[0]);
           });
       }

       /*--------------页面中间树形--------------end----*/

       /*--------------页面右边--------------start----*/
       // 地图数据
       $scope.mapDataset = mapDataset;
       $scope.isLoadTianDiTu = parseInt(isLoadTianDiTu);;

       //树形数据
       $scope.typeTreeData3 = [
        {
            label: "OGC服务", children: [
                    { label: "WMS", callFun: function () { getServerInterface1("WMS"); }, children: [] },
                    { label: "WMTS", callFun: function () { getServerInterface1("WMTS"); }, children: [] },
                    { label: "WFS", callFun: function () { getServerInterface1("WFS"); }, children: [] },
                    { label: "KML", callFun: function () { getServerInterface1("KML"); }, children: [] }
            ]
        }, {
            label: "数据接口", children: [
                    { label: "查询地图的图层api", callFun: function () { getServerInterface2("MapAPI"); }, children: [] },
                    { label: "查询图层的属性api", callFun: function () { getServerInterface2("LayerAPI"); }, children: [] },
                    { label: "图层空间查询api", callFun: function () { getServerInterface2("SpaceAPI"); }, children: [] }

            ]
        }];



       //切换服务
       var layer;
       $scope.currentData = {};
       $scope.mapSerShow = function (index, item) {
           if ($scope.state === index) {
               return;
           }
           $scope.state = index;
           $scope.currentData = item ? item : {};

           if ($scope.typeTreeData3[0].children[0].selected) {
               getServerInterface1("WMS");
           } else {
               $scope.treeQueryCtrl3.select_branch($scope.typeTreeData3[0].children[0]);
           }
           console.log(157, $scope.addLayer);
           //加载地图
           if (layer) {
               $scope.removeLayer(layer);
           }
           if (!item) return;
           layer = newLocalTilesByWMS(GeoServerUrl + '/wms', WorkSpace + ':' + item.mapEnName, 'image/png');

           layer.setZIndex(30);

           var bounds = [$scope.currentData.minX, $scope.currentData.minY, $scope.currentData.maxX, $scope.currentData.maxY];

           var map = $scope.addLayer(layer, bounds);
       }

       //请求后台获取服务数据
       function getServerInterface1(type) {
           $scope.currentLayerInfo = [];
           serverInterface.getServerInterface($scope.currentData.id, type).success(function (data, statues) {
               $scope.currentLayerInfo = data.items;
           });
       }
       //请求后台获取服务数据
       function getServerInterface2(type) {
           $scope.currentLayerInfo = [];
           serverInterface.getDataInterface($scope.currentData.id, type).success(function (data, statues) {
               $scope.currentLayerInfo = data.items;
           });
       }

       $scope.selectedData3 = "";//当前节点,点击树时自动赋值

       //选中回调函数
       $scope.onSelected3 = function (node, getParentNodeBackFun) {
           if (!node.callFun) return;
           node.callFun();
       };
       /*--------------页面右边--------------end----*/

       //自适应高度  
       function setHeight() {
           var winH = angular.element(window).height();
           var bottomH = angular.element(".pull-left").height();
           var contentH = (winH - 435 - bottomH);
           $scope.mapheight = contentH;
           angular.element(".mapDiv").height(contentH);
       }
       setHeight();
       angular.element(window)[0].onresize = function () {
           setHeight();
       }
   }]);
