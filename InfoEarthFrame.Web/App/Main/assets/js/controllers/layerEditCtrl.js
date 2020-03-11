'use strict';
/**
 * dataInterfaceCtrl2 Controller
 */
app.controller('layerEditCtrl', ['$rootScope', '$scope', 'SweetAlert', '$element', '$timeout', 'waitmask', 'abp.services.app.map', 'abp.services.app.layerContent', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.setSys', 'abp.services.app.serverInterface', '$http',
 function ($rootScope, $scope, SweetAlert, $element, $timeout, waitmask, mapSearch, layerContent, dataTag, dataType, setSys, serverInterface, $http) {
     $rootScope.loginOut();
     $rootScope.homepageStyle = {};
     //把地图之外的所有元素隐藏掉
     $rootScope.app.layout.isNavbarFixed = false;
     $rootScope.app.isWholeScreen = true;

     // 获取url传值
     function GetQueryString(name, url) {
         var after = !!url ? url.split("?")[1] : window.location.hash.split("?")[1];
         if (after) {
             var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
             var r = after.match(reg);
             if (r != null) return decodeURIComponent(r[2]); return null;
         }
     }
     //接收url传值的对象
     var currentData = {};

     $scope.showInput = false;
     //是否展示数据列表
     $scope.showTable = false;
     $scope.showWFS = false;

     var currentData = {
         "id": GetQueryString("id"),
         "type": GetQueryString("type"),
         "flag": GetQueryString("flag"),
         "minX": GetQueryString("minX"),
         "minY": GetQueryString("minY"),
         "maxX": GetQueryString("maxX"),
         "maxY": GetQueryString("maxY"),
         "mapEnName": GetQueryString("mapEnName"),
     }
     $scope.initialSelection = currentData.type === 'MapAPI' ? '查询地图的图层api' : currentData.type;

     // 地图数据
     $scope.mapDataset = mapDataset;
     $scope.isLoadTianDiTu = parseInt(isLoadTianDiTu);

     //选择哪个地图服务
     $scope.serviceType = "";

     //地图图层
     var layer, map;
     function loadMaplayer(type, layerNum, url) {
         if ($scope.serviceType === type && layerNum) {
             return;
         }
         $scope.serviceType = type;
         var _url = url.split("?")[0];
         if (layer) {
             $scope.removeLayer(layer);
         }
         if (type === "WMS") {
             layer = newLocalTilesByWMS(_url, WorkSpace + ':' + currentData.mapEnName, 'image/png');
         }
         else if (type === "WMTS") {
             layer = newLocalTilesByWMTS(_url, WorkSpace + ':' + currentData.mapEnName);
         }
         else if (type === "iTelluro") {
             var dataServerKey = GetQueryString('T', url);
             //console.log('Debug:   ', dataServerKey);
             layer = new iTelluro().newItelluroLayer(dataServerKey, _url, 512, 36);
         }
         else if (type === "WFS") {
             var srsName = "";
             var tmpurl = "";
             var tmpParams = url.split('&');
             for (var i = 0; i < tmpParams.length; i++) {
                 if (tmpParams[i].indexOf("srsName=") === 0) {
                     srsName = tmpParams[i].replace("srsName=", "");
                 }
             }
             for (var i = 0; i < tmpParams.length; i++) {
                 if (tmpParams[i].indexOf("bbox=") === 0) {
                     tmpParams[i] = tmpParams[i] + "," + srsName;
                 }
             }
             tmpurl = tmpParams.join('&') + "&outputFormat=application/json";
             //console.log(tmpurl);
             var vectorSource = new ol.source.Vector({
                 format: new ol.format.GeoJSON(),
                 url: tmpurl,
                 strategy: ol.loadingstrategy.bbox
             });
             layer = new ol.layer.Vector({
                 source: vectorSource
             });
         }
         else if (type === "KML") {
             layer = new ol.layer.Vector({
                 source: new ol.source.Vector({
                     url: url,
                     format: new ol.format.KML()
                 })
             });
         }
         if (!layer) { return; }
         layer.setZIndex(27);
         var bounds = [currentData.minX, currentData.minY, currentData.maxX, currentData.maxY];
         map = $scope.addLayer(layer, bounds);
         map.updateSize();

         if (type === "WFS") {
             $timeout(function () {
                 $scope.vector = map.getLayers();
                 $scope.vector = $scope.vector.array_[$scope.vector.array_.length - 1];

                 var modifySource = $scope.vector.getSource();
                 interactions = {
                     select: new ol.interaction.Select(),
                     //modify: new ol.interaction.Modify({ features: modifySource.getFeaturesCollection() })
                 };
                 interactions.modify = new ol.interaction.Modify({ features: interactions.select.getFeatures() });
                 interactions.select.on('select', function (e) {
                     $scope.chosedFeature = [];
                     $scope.shapedFeature = [];
                     e.selected.forEach(function (f) {
                         $scope.chosedFeature.push(f);
                     });
                 });

                 //截断线之后执行的方法
                 $scope.vector.getSource().on("aftersplit", function (e) {
                     //splitting = false;
                     console.log(e);
                 });
             }, 500);
         }
     }

     //-------------------------------------测试添加feature------------start
     var testObj = {
         type: "Feature",
         id: "vh49z016d2119142523.1",
         geometry: {
             type: "Polygon",
             coordinates: [[[111.98545273, 30.66667985], [112, 30.66666667], [111.99999918, 30.54412475], [111.99999918, 30.54412475], [111.99913922, 30.54411886], [111.99760571, 30.54386334], [111.99620762, 30.54292247], [111.9949375, 30.54240415], [111.99336101, 30.54149295], [111.99071211, 30.54114819], [111.99071211, 30.54114819], [111.99269017, 30.54747434], [111.99296332, 30.55422193], [111.99226983, 30.55682831], [111.98887948, 30.56311307], [111.98732328, 30.56694559], [111.98717599, 30.56754435], [111.98403292, 30.57046994], [111.98125075, 30.57244204], [111.97846178, 30.5738198], [111.97688432, 30.57381843], [111.97617993, 30.57351115], [111.97423011, 30.57153504], [111.97352584, 30.57124685], [111.97247423, 30.57124591], [111.97109029, 30.57289318], [111.97003702, 30.57273886],
                 [111.96952828, 30.57229751], [111.96968656, 30.57076416], [111.97311413, 30.56770031], [111.97311072, 30.56739359], [111.97242658, 30.56693291], [111.97085619, 30.56756405], [111.96824746, 30.56938265], [111.9658392, 30.5715273], [111.9644372, 30.57351944], [111.96446409, 30.5739795], [111.96497282, 30.57442092], [111.97005773, 30.57461743], [111.97127269, 30.57551943], [111.97307129, 30.57966154], [111.97494449, 30.58656391], [111.97655954, 30.58993904], [111.97865678, 30.59331455], [111.97851206, 30.59408109], [111.97754816, 30.5945125], [111.97754816, 30.5945125], [111.97678023, 30.59485619], [111.97661114, 30.59593565], [111.97768014, 30.59746992], [111.98100926, 30.6005314], [111.98279154, 30.60313986], [111.98280422, 30.60498861], [111.98196399, 30.60792082],
                 [111.98236643, 30.61515941], [111.98187179, 30.61701121], [111.98188127, 30.61839768], [111.98310281, 30.61960367], [111.98451805, 30.62009583], [111.99069775, 30.62373695], [111.99074117, 30.6242101], [111.99070418, 30.62466129], [111.98865557, 30.62775022], [111.98863638, 30.63020499], [111.98779162, 30.63174425], [111.98691677, 30.63249742], [111.9862123, 30.63265504], [111.98374164, 30.63268906], [111.98127557, 30.63318327], [111.97936998, 30.63455617], [111.97956343, 30.63624262], [111.98112953, 30.63838106], [111.98116806, 30.64021913], [111.98082067, 30.640834], [111.97941782, 30.64176282], [111.97606996, 30.6420932], [111.97518908, 30.64227107], [111.97466394, 30.6427152], [111.97467767, 30.64424708], [111.97539062, 30.64514118], [111.9773369, 30.64591137],
                 [111.97927635, 30.64587791], [111.98173611, 30.64480729], [111.98596277, 30.64416359], [111.98667486, 30.64451384], [111.9870299, 30.64522716], [111.98703773, 30.64614555], [111.98598644, 30.64952114], [111.9837462, 30.6530587], [111.98359839, 30.65382514], [111.98519345, 30.65475093], [111.98621519, 30.6556427], [111.98639548, 30.65610057], [111.98663554, 30.65839493], [111.98750701, 30.65976617], [111.98749153, 30.66053167], [111.98681216, 30.66099598], [111.98474355, 30.66125103], [111.98474355, 30.66125103], [111.98416653, 30.6612912], [111.98292628, 30.66137754], [111.98292628, 30.66137754], [111.98254612, 30.66169825], [111.9821882, 30.66285244], [111.98224894, 30.66363223], [111.98242661, 30.66378405], [111.98280658, 30.66439349], [111.98545273, 30.66667985]]]
         },
         geometry_name: "geom",
         properties: {
             M2S_LEGEND: "000000820000000000000000",
             YSD: "0",
             SO: "Q3al",
             DSO: "Q↓3↑al",
             DSN: "新生界第四系更新统",
             ID: 1,
             QDFCF: "50",
             周长: 0.357065,
             YSEB: "0",
             YSC: "0",
             面积: 0.002089,
             CHFCAC: "HG160001",
             YSHB: "0"
         }
     };

     //获取高亮的拐点坐标
     $scope.addtestLayer = function () {
         var vLayer;
         var geom = testObj.geometry.type + "((";
         for (var i = 0; i < testObj.geometry.coordinates.length; i++) {
             var tmpArr = testObj.geometry.coordinates[i];
             for (var j = 0; j < tmpArr.length; j++) {
                 var oneArr = tmpArr[j][0] + " " + tmpArr[j][1];
                 geom += oneArr + ",";
             }
             geom = geom.replace(/,$/, ')');
             geom += ",";
         }
         geom = geom.replace(/,$/, ')');

         vLayer = addTagging(geom);
         return vLayer;
     }

     function addTagging(coordinates) {
         var tmpObj = new ol.format.WKT();
         var geometry = tmpObj.readGeometry(coordinates);

         var vectorSource = new ol.source.Vector({});

         var vectorLayer = new ol.layer.Vector({
             source: vectorSource
         });
         vectorLayer.setZIndex(40);
         map.addLayer(vectorLayer);

         var iconFeature = new ol.Feature({
             geometry: geometry,
             name: '拐点坐标标注',
             population: 4000,
             rainfall: 500
         });
         vectorSource.addFeature(iconFeature);
         return vectorLayer;
     };

     //-------------------------------------测试添加feature------------end

     //显示按钮的样式
     $scope.activeShowBtn = 0;
     $scope.qiehuanBorderStyle = {
         'padding-bottom': '9px'
     }
     //点击显示某一个弹窗
     $scope.showRightWin = function (num) {
         $scope.activeShowBtn = num;
         if (num === 1) {
             $scope.qiehuanBorderStyle = {
                 'padding-bottom': '9px',
             }
         }
         else if (num == 2) {
             $scope.closeLeftWin();
             $scope.isShowData = true;
             $scope.selectedData3 = "";
             $scope.showWFS = false;
             $scope.isShowChose = false;
             $scope.wfsLayerData = [];
             $scope.qiehuanBorderStyle = {
                 'padding-bottom': '12px',
             }
         }
     }

     //关闭上面的窗口
     $scope.closeWin = function () {
         $scope.activeShowBtn = 0;
         $scope.isShowData = false;
         $scope.qiehuanBorderStyle = {
             'padding-bottom': '9px',
         }
     }

     /*--------------页面左边--------------start----*/

     //树形数据
     $scope.typeTreeData3 = [{ label: "查询地图的图层api", name: "MapAPI", callFun: function () { getServerInterface2("MapAPI"); }, children: [] },
             { label: "查询图层的属性api", name: "LayerAPI", callFun: function () { getServerInterface2("LayerAPI"); }, children: [] },
             { label: "图层空间查询api", name: "SpaceAPI", callFun: function () { getServerInterface2("SpaceAPI"); }, children: [] }];
     $scope.selectedData3 = "";//当前节点,点击树时自动赋值

     //树节点点击事件
     $scope.onSelected3 = function (node, getParentNodeBackFun) {
         if (!node.callFun) return;
         node.callFun();
         clearData();
         $scope.params = {};
         $scope.showInput = false;
         $scope.searchServiceURL = "";
         $scope.showTable = false;
     };

     //请求后台获取服务数据
     $scope.getServerInterface1 = function (type) {
         $scope.OGCStyle = {
             "top": "100px"
         }
         if (type === "WFS") {
             $scope.OGCStyle = {
                 "top": "150px"
             }
         }
         $scope.isShowOGC = true;
         $scope.OGCTitle = type + "服务信息";
         
         $scope.currentLayerInfo = [];
         $scope.layerURL = "";

         //重置WFS图层
         $scope.showWFS = false;
         $scope.isShowChose = false;
         $scope.wfsLayerData = [];

         $scope.isShowData = false;
         $scope.showInput = false;

         serverInterface.getServerInterface(currentData.id, type).success(function (data, statues) {
             //console.log(data);
             $scope.layerURL = data.items[0].serverPath;
             $scope.currentLayerInfo = data.items;
             //console.log($scope.layerURL);
             
             if (type === 'WFS') {
                 layerContent.getAllListByMapID(currentData.id).success(function (data1, statues) {
                     $scope.showWFS = true;
                     for (var i = 0; i < data.items[0].serverRequestParameters.items.length; i++) {
                         var tmpItem = data.items[0].serverRequestParameters.items[i];
                         if (tmpItem.parameterName === "typeNames") {
                             var tmpParaValue = tmpItem.parameterValue.split(', ');
                             for (var j = 0; j < tmpParaValue.length; j++) {
                                 //var tmpName = tmpParaValue[j].replace("gisdatamanage:", "");
                                 var tmpObj = {
                                     id: data1.items[j].id,
                                     value: tmpParaValue[j],
                                     ischecked: j == 0 ? true : false
                                 };
                                 $scope.wfsLayerData = $scope.wfsLayerData.concat(tmpObj);
                             }
                             //$scope.wfsCheckedOne($scope.wfsLayerData[0]);
                             break;
                         }
                     }
                 }).error(function (data, statues) {
                     console.log(data);
                 });
             }
             loadMaplayer(type, 1, $scope.layerURL);
         }).error(function (data, statues) {
             console.log(data);
         });
     }

     //请求后台获取服务数据
     function getServerInterface2(type) {
         $scope.currentLayerInfo = [];
         serverInterface.getDataInterface(currentData.id, type).success(function (data, statues) {
             //console.log(data);
             $scope.currentLayerInfo = data.items;
         });
     }

     //关闭左侧窗口
     $scope.closeLeftWin = function () {
         $scope.isShowOGC = false;
     }

     //点击链接之后执行的方法
     $scope.serverPathClick = function (obj) {
         //console.log(obj);
         $scope.showInput = true;
         var serverPathPre = obj.serverPath.split('?')[0];
         var serverPathParam = obj.serverPath.split('?')[1];
         $scope.searchServiceURL = serverPathPre;
         $('#searchUrlInput').val($scope.searchServiceURL);
         var UrlParams = serverPathParam.split('&');
         var UrlParamObj = {};
         if (UrlParams.length > 0) {

             for (var i = 0; i < UrlParams.length; i++) {
                 var tmps = UrlParams[i].split('=');
                 if (!!tmps[0] && tmps.length > 0) {
                     UrlParamObj[tmps[0]] = angular.copy(tmps[1]);
                 }
             }
         }

         //console.log(UrlParamObj);
         $scope.params = angular.copy(UrlParamObj);
         $scope.showTable = false;
         $scope.tablePageData = [];
         //getData($scope.searchServiceURL);
         //console.log($scope.searchServiceURL);
     }

     /*--------------页面右边地图--------------start----*/

     $scope.searchServiceURL = "";
     $scope.params = {};
     $scope.tableData = [];
     $scope.tableWidth = {};
     $scope.tablePageData = [];
     $scope.realURL = "";
     $scope.mapContentSty = {};
     $scope.tableDataType = "";

     //根据输入框中的内容进行请求
     $scope.searchService = function () {
         //console.log('$scope.searchServiceURL------', $scope.searchServiceURL);
         clearData();
         //console.log($scope.params);
         var url = $scope.searchServiceURL + '?';
         for (var i in $scope.params) {
             if (!!i) {
                 url += i + '=' + $scope.params[i] + '&';
             }
         }
         url = url.substr(0, url.length - 1);
         if (url.indexOf('GetAttrByLayerName') > -1 || url.indexOf('GetAttrByCondition') > -1 || url.indexOf('GetAttrByPt') > -1 || url.indexOf('GetAttrByPtTolerane') > -1 || url.indexOf('GetAttrByRect') > -1) {
             var countURL = '';
             if (url.indexOf('GetAttrByLayerName') > -1) {
                 countURL = angular.copy(url).replace('GetAttrByLayerName', 'GetAttrByLayerNameCount');
                 url = angular.copy(url).replace('GetAttrByLayerName', 'GetAttrByLayerNamePage');
             }
             else if (url.indexOf('GetAttrByCondition') > -1) {
                 countURL = angular.copy(url).replace('GetAttrByCondition', 'GetAttrByConditionCount');
                 url = angular.copy(url).replace('GetAttrByCondition', 'GetAttrByConditionPage');
             }
             else if (url.indexOf('GetAttrByPtTolerane') > -1) {
                 countURL = angular.copy(url).replace('GetAttrByPtTolerane', 'GetAttrByPtToleraneCount');
                 url = angular.copy(url).replace('GetAttrByPtTolerane', 'GetAttrByPtToleranePage');
             }
             else if (url.indexOf('GetAttrByPt') > -1) {
                 countURL = angular.copy(url).replace('GetAttrByPt', 'GetAttrByPtCount');
                 url = angular.copy(url).replace('GetAttrByPt', 'GetAttrByPtPage');
             }
             else if (url.indexOf('GetAttrByRect') > -1) {
                 countURL = angular.copy(url).replace('GetAttrByRect', 'GetAttrByRectCount');
                 url = angular.copy(url).replace('GetAttrByRect', 'GetAttrByRectPage');
             }
             $http.get(countURL).success(function (res, status) {
                 //console.log(res);
                 if (!!res) {
                     res = JSON.parse(res);
                     //console.log(res);
                     $scope.pageCounts = Math.ceil(res[0].count);
                     $scope.totalPages = Math.ceil($scope.pageCounts / $scope.pageSize);
                     //console.log($scope.pageCounts, $scope.totalPages);
                 }
             }).error(function (res, status) {
                 console.log(res);
                 alertFun("请求失败!", '', 'error', '#007AFF');
                 waitmask.onHideMask();
             });
         }

         $scope.realURL = url;
         //console.log($scope.realURL);
         getData($scope.realURL, $scope.pageIndex, $scope.pageSize);
     }

     //关闭查询结果窗口
     $scope.closeResWin = function () {
         $scope.showInput = false;
     }

     //实时监听输入框的值
     $scope.searchChanged = function (val) {
         //console.log(decodeURI(encodeURI(val)));
         $scope.searchServiceURL = val;
     }

     $scope.paramChanged = function (a, b) {
         //console.log(a, b);
         $scope.params[a] = b;
     }
     
     //WFS选择图层
     $scope.hideChose = function () {
         $scope.isShowChose = !$scope.isShowChose;
     }
     $scope.wfsCheckedOne = function (a) {
         //console.log(a);
         if (a.ischecked) {
             return;
         }
         for (var i = 0; i < $scope.wfsLayerData.length; i++) {
             if (a.id === $scope.wfsLayerData[i].id) {
                 //console.log($scope.wfsLayerData[i]);
                 $scope.wfsLayerData[i].ischecked = !$scope.wfsLayerData[i].ischecked;
             }
             else {
                 $scope.wfsLayerData[i].ischecked = false;
             }
         }

         var tmpParams = $scope.layerURL.split('&');
         for (var i = 0; i < tmpParams.length; i++) {
             if (tmpParams[i].indexOf('typeNames=') === 0) {
                 tmpParams[i] = 'typeNames=' + a.value;
             }
         }
         tmpParams = tmpParams.join('&');
         //console.log(tmpParams);
         loadMaplayer("WFS", 0, tmpParams);
     }

     //自适应高度  
     function setHeight() {
         var winH = angular.element(window).height();
         var winW = angular.element(window).width();
         var contentH = (winH - 10 - 70 - 25);
         //angular.element(".mapDiv").height(contentH);
         $scope.mapheight = contentH;

         $('body').scrollTop(10);
         if ($('body').scrollTop() > 0) {
             winW += 17;
         }
         angular.element(".mapDiv").width(winW);
         $scope.mapwidth = winW;

         $scope.leftWinStyle = {
             'max-height': contentH - 250 + 'px'
         }
     }
     $timeout(setHeight, 300);
     angular.element(window)[0].onresize = function () {
         setHeight();
     }

     /*--------------页面右边地图--------------end----*/

     //重置数据
     function clearData() {
         $scope.tableData = [];
         $scope.tablePageData = [];
         $scope.pageCounts = 0;
         $scope.pageIndex = 1;
         $scope.goPage = 1;
     }

     //发送ajax请求
     function getData(url, index, size) {
         if (!!url) {
             waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
             if (url.indexOf('Page') > -1) {
                 url = url + '&pageSize=' + size + '&pageIndex=' + index;
                 //console.log(url);
                 $http.get(url).success(function (res, status) {
                     //console.log(res);

                     if (!!res) {
                         res = JSON.parse(res);
                         $scope.tablePageData = res.data;
                         //console.log($scope.tablePageData);
                         if (toString.call($scope.tablePageData[0]) === "[object Array]") {
                             $scope.tableDataType = "array";
                         }
                         else {
                             $scope.tableDataType = "object";
                         }

                         var num = 0;
                         for (var i in $scope.tablePageData[0]) {
                             num++;
                         }
                         $scope.tableWidth = {
                             "width": num * 120 + "px"
                         };

                         $scope.showTable = true;
                         waitmask.onHideMask();
                     }
                 }).error(function (res, status) {
                     console.log(res);
                     alertFun("请求失败!", '', 'error', '#007AFF');
                     waitmask.onHideMask();
                 });
             }
             else {
                 $http.get(url).success(function (res, status) {
                     //console.log(res);
                     var type = typeof (res);
                     //console.log(type);

                     if (!!res) {
                         if (type === "string") {
                             res = JSON.parse(res);
                             $scope.tableData = res.data;
                         }
                         if (type === "object") {
                             $scope.tableData = res.items;
                         }
                         var num = 0;
                         for (var i in $scope.tableData[0]) {
                             num++;
                         }
                         $scope.tableWidth = {
                             "width": num * 120 + "px"
                         };
                         $scope.pageCounts = $scope.tableData.length;
                         $scope.totalPages = Math.ceil($scope.pageCounts / $scope.pageSize);
                         getPageData($scope.pageIndex, $scope.pageSize);
                         //console.log($scope.tablePageData);
                         $scope.showTable = true;
                         waitmask.onHideMask();
                     }
                 }).error(function (res, status) {
                     console.log(res);
                     alertFun("请求失败!", '', 'error', '#007AFF');
                     waitmask.onHideMask();
                 });
             }
         }
     }

     //将请求回来的数据分页
     function getPageData(index, size) {
         var tmpArr = $scope.tableData.slice((index - 1) * size, index * size);
         $scope.tablePageData = [];
         
         var tmp = [];
         for (var i = 0; i < tmpArr.length; i++) {
             var objToArr = [];
             for (var j in tmpArr[i]) {
                 objToArr.push([j, tmpArr[i][j]]);
             }
             tmp.push(objToArr);
         }
         //console.log(tmp);
         $scope.tablePageData = angular.copy(tmp);
         if (toString.call($scope.tablePageData[0]) === "[object Array]") {
             $scope.tableDataType = "array";
         }
         else {
             $scope.tableDataType = "object";
         }
     }

     //页面初始化时加载WMS
     $scope.getServerInterface1('WMS');

     //分页
     $scope.maxSize = 3;//页码个数显示数
     $scope.goPage = 1;//转到多少页
     $scope.pageCounts = 0;//32;//总条数
     $scope.pageIndex = 1;//1;//起始页
     $scope.pageSize = 8;//10;//每页显示条数

     //分页的事件方法
     $scope.pageChanged = function (a, evt) {
         $timeout(function () {
             if (evt && evt.keyCode !== 13) { return; }//注：回车键为13
             if (a) {
                 a = parseInt(a);
                 if (isNaN(a) || a < 1 || a > $scope.totalPages) {
                     $scope.goPage = $scope.pageIndex;
                     return;
                 }
                 $scope.goPage = a;
                 $scope.pageIndex = a;
             }
             if ($scope.realURL.indexOf('Page') > -1) {
                 getData($scope.realURL, $scope.pageIndex, $scope.pageSize);
             }
             else {
                 getPageData($scope.pageIndex, $scope.pageSize);
             }
         }, 200);
     };

     //解析XML
     function getXML(xmlString) {
         var xmlDoc = null;

         if (!window.DOMParser && window.ActiveXObject) {   //window.DOMParser 判断是否是非ie浏览器

             var xmlDomVersions = ['MSXML.2.DOMDocument.6.0', 'MSXML.2.DOMDocument.3.0', 'Microsoft.XMLDOM'];
             for (var i = 0; i < xmlDomVersions.length; i++) {
                 try {
                     xmlDoc = new ActiveXObject(xmlDomVersions[i]);
                     xmlDoc.async = false;
                     xmlDoc.loadXML(xmlString); //loadXML方法载入xml字符串
                     break;
                 } catch (e) {
                     console.log(e);
                 }
             }
         }
             //支持Mozilla浏览器
         else if (window.DOMParser && document.implementation && document.implementation.createDocument) {
             try {
                 /* DOMParser 对象解析 XML 文本并返回一个 XML Document 对象。
                  * 要使用 DOMParser，使用不带参数的构造函数来实例化它，然后调用其 parseFromString() 方法
                  * parseFromString(text, contentType) 参数text:要解析的 XML 标记 参数contentType文本的内容类型
                  * 可能是 "text/xml" 、"application/xml" 或 "application/xhtml+xml" 中的一个。注意，不支持 "text/html"。
                  */
                 var domParser = new DOMParser();
                 xmlDoc = domParser.parseFromString(xmlString, 'text/xml');
             } catch (e) {
                 console.log(e);
             }
         }
         else {
             return null;
         }

         return xmlDoc;
     }

     function alertFun(title, text, type, color) {
         SweetAlert.swal({
             title: title,
             text: text,
             type: type,
             confirmButtonColor: color
         });
     }



     /*--------------图层编辑--------------end----*/
     $scope.showAllDraw = false;
     $scope.showAllEdit = false;
     $scope.chosedFeature = [];//选中的元素
     $scope.shapedFeature = [];//变形的元素
     $scope.newFeatures = [];//创建的元素
     $scope.deleteFeature = [];//删除的元素
     //添加
     $scope.addLayerEdit = function () {
         $scope.showAllDraw = true;
         $scope.showAllEdit = false;
     }
     //编辑
     $scope.showLayerEdit = function () {
         $scope.showAllEdit = true;
         $scope.showAllDraw = false;
     }
     //隐藏
     $scope.hideLayerEdit = function () {
         // deleteAll();
         setInteraction('none');
         if (typeof (shape_interaction) !== 'undefined' && !!shape_interaction) {
             map.removeInteraction(shape_interaction);
         }
         if (typeof ($scope.split) !== 'undefined' && !!$scope.split) {
             map.removeInteraction($scope.split);
         }
         $scope.newFeatures = [];
         $scope.chosedFeature = [];
         $scope.shapedFeature = [];
         $scope.deleteFeature = [];
         $scope.showAllDraw = false;
         $scope.showAllEdit = false;
     }
     //保存
     $scope.saveLayerEdit = function () {
         var layerId = "", values = [], geometry = [];
         for (var i = 0; i < $scope.wfsLayerData.length; i++) {
             if ($scope.wfsLayerData[i].ischecked) {
                 layerId = $scope.wfsLayerData[i].id;
                 break;
             }
         }
         //新增
         if ($scope.showAllDraw) {
             for (var j = 0; j < $scope.newFeatures.length; j++) {
                 var wktStr = getWktString($scope.newFeatures[j]);
                 console.log(wktStr);
                 serverInterface.addLayerElement(layerId, [], wktStr).success(function (data, status) {
                     console.log(data);
                 }).error(function (data, status) {
                     console.log(data);
                 });
             }
         }
         //编辑
         var editFeature;
         if ($scope.chosedFeature.length > 0) {
             editFeature = $scope.chosedFeature[0];
         }
         else if (!!$scope.shapedFeature) {
             editFeature = $scope.chosedFeature;
         }
         if (!!editFeature.length) {
             var valueObj = editFeature.getProperties();
             for (var i in valueObj) {
                 values.push(i + ":" + valueObj[i]);
             }
             geometry = getWktString(editFeature);
             var elementId = editFeature.getProperties().guid;//要素id
             serverInterface.updateLayerElement(layerId, elementId, values, geometry).success(function (data, status) {
                 console.log(data);
             }).error(function (data, status) {
                 console.log(data);
             });
         }
         //删除
         if ($scope.deleteFeature.length > 0) {
             
             for (var i = 0; i < $scope.deleteFeature.length; i++) {
                 var elementId = $scope.deleteFeature[i].getProperties().guid;//要素id
                 serverInterface.deleteLayerElement(layerId, elementId).success(function (data, status) {
                     console.log(data);
                 }).error(function (data, status) {
                     console.log(data);
                 });
             }
         }

         $scope.newFeatures = [];
         $scope.chosedFeature = [];
         $scope.shapedFeature = [];
         $scope.deleteFeature = [];
         setInteraction('none');
         if (typeof (shape_interaction) !== 'undefined' && !!shape_interaction) {
             map.removeInteraction(shape_interaction);
         }
         if (typeof ($scope.split) !== 'undefined' && !!$scope.split) {
             map.removeInteraction($scope.split);
         }
         $scope.showAllEdit = false;
         $scope.showAllDraw = false;
     }

     //获取wkt坐标
     function getWktString(feature) {
         var geo = feature.getGeometry();
         var coor = geo.getCoordinates();
         var type = geo.getType();
         var geom = "";
         if (type === "Point") {
             geom = "POINT(" + coor[0] + " " + coor[1] + ")";
         }
         else if (type === "LineString") {
             geom = "LINESTRING(";
             for (var k = 0; k < coor.length; k++) {
                 geom += coor[k][0] + " " + coor[k][1] + ",";
             }
             geom = geom.replace(/,$/, ')');
         }
         else {
             geom = "POLYGON((";
             for (var i = 0; i < coor.length; i++) {
                 var tmpArr = coor[i];
                 for (var j = 0; j < tmpArr.length; j++) {
                     var oneArr = tmpArr[j][0] + " " + tmpArr[j][1];
                     geom += oneArr + ",";
                 }
                 geom = geom.replace(/,$/, ')');
                 geom += ",";
             }
             geom = geom.replace(/,$/, ')');
         }
         return geom;
     }

     var interactions = {};
     
     //创建元素的方法
     $scope.draw = function (type) {
         setInteraction('none');
         if (typeof (shape_interaction) !== 'undefined' && !!shape_interaction) {
             map.removeInteraction(shape_interaction);
         }
         if (typeof ($scope.split) !== 'undefined' && !!$scope.split) {
             map.removeInteraction($scope.split);
         }
         map.removeInteraction(interactions.draw);

         if (type === "point") {
             interactions.draw = new ol.interaction.Draw({
                 source: $scope.vector.getSource(),
                 type: "Point"
             });
         }
         else if (type === "line") {
             interactions.draw = new ol.interaction.Draw({
                 source: $scope.vector.getSource(),
                 type: "LineString"
             });
             interactions.modify = new ol.interaction.Modify({ features: interactions.select.getFeatures() });             
         }
         else {
             interactions.draw = new ol.interaction.Draw({
                 source: $scope.vector.getSource(),
                 type: "Polygon"
             });
         }
         interactions.draw.on('drawend',
            function (evt) {
                $scope.newFeatures.push(evt.feature);
                //console.log($scope.newFeatures);
            }, this);
         setInteraction('draw');
     }

     // Activate modification
     var setInteraction = function (name) {
         if (!!interactions.select) {
             interactions.select.getFeatures().clear();
         }
         for (var i in interactions) {
             if (i == name) {
                 map.addInteraction(interactions[i]);
             }
             interactions[i].set("active", (i == name));
         }
         if (name == "modify") {
             map.addInteraction(interactions.select);
             interactions.select.set("active", true);
         }
     }
     
     function setHandleStyle() {
         if (!shape_interaction instanceof ol.interaction.Transform) return;
         if (1) {
             // Style the rotate handle
             var circle = new ol.style.RegularShape({
                 fill: new ol.style.Fill({ color: [255, 255, 255, 0.01] }),
                 stroke: new ol.style.Stroke({ width: 1, color: [0, 0, 0, 0.01] }),
                 radius: 8,
                 points: 10
             });
             shape_interaction.setStyle('rotate',
                 new ol.style.Style({
                     text: new ol.style.Text({
                         text: '\uf0e2',
                         font: "16px Fontawesome",
                         textAlign: "left",
                         fill: new ol.style.Fill({ color: 'red' })
                     }),
                     image: circle
                 }));
             // Center of rotation
             shape_interaction.setStyle('rotate0',
                 new ol.style.Style({
                     text: new ol.style.Text({
                         text: '\uf0e2',
                         font: "20px Fontawesome",
                         fill: new ol.style.Fill({ color: [255, 255, 255, 0.8] }),
                         stroke: new ol.style.Stroke({ width: 2, color: 'red' })
                     }),
                 }));
             // Style the move handle
             shape_interaction.setStyle('translate',
                 new ol.style.Style({
                     text: new ol.style.Text({
                         text: '\uf047',
                         font: "20px Fontawesome",
                         fill: new ol.style.Fill({ color: [255, 255, 255, 0.8] }),
                         stroke: new ol.style.Stroke({ width: 2, color: 'red' })
                     })
                 }));
         }
         else {
             shape_interaction.setDefaultStyle();
         }
         // Refresh
         shape_interaction.set('translate', shape_interaction.get('translate'));
     }

     //添加变形
     //变形的选项
     var shape_interaction = new ol.interaction.Transform({
         translate: true,//是否允许移动
         translateFeature: false,//false时只有选中中心的图标才能进行移动操作，选中其他地方只会移动地图
         scale: true,//是否允许放大缩小
         stretch: true,//在scale为true时才有意义，是否允许一边的放大缩小
         keepAspectRatio: false ? ol.events.condition.always : undefined,//选中的元素是否等比例放大缩小
         rotate: true,//是否允许旋转
     });
     var startangle = 0;
     var d = [0, 0];
     //开始点击移动，旋转时执行的事件
     shape_interaction.on(['rotatestart', 'translatestart'], function (e) {
         // Rotation
         //console.log('start');
         startangle = e.feature.get('angle') || 0;
         // Translation
         d = [0, 0];
     });
     //旋转
     shape_interaction.on('rotating', function (e) {
         //console.log("转");
         // $('#info').text("rotate: "+((e.angle*180/Math.PI -180)%360+180).toFixed(2)); 
         // Set angle attribute to be used on style !
         e.feature.set('angle', startangle - e.angle);
     });
     //移动位置
     shape_interaction.on('translating', function (e) {
         //console.log(1);
         d[0] += e.delta[0];
         d[1] += e.delta[1];
         // $('#info').text("translate: "+d[0].toFixed(2)+","+d[1].toFixed(2)); 
     });
     //形状变换
     shape_interaction.on('scaling', function (e) {
         //console.log(3);
         // $('#info').text("scale: "+e.scale[0].toFixed(2)+","+e.scale[1].toFixed(2)); 
     });
     //旋转，移动，形状变换鼠标放开时执行的事件
     shape_interaction.on(['rotateend', 'translateend', 'scaleend'], function (e) {
         //console.log(e);
         for (var i = 0; i < $scope.shapedFeature.length; i++) {
             var featureId = $scope.shapedFeature[i].getProperties().guid;
             if (featureId === e.feature.getProperties().guid) {
                 $scope.shapedFeature.splice(i, 1);
             }
             $scope.shapedFeature.push(e.feature);
         }
     });

     //变形
     $scope.shape = function () {
         setInteraction('none');
         $scope.shapedFeature = [];
         $scope.chosedFeature = [];
         if (typeof ($scope.split) !== 'undefined' && !!$scope.split) {
             map.removeInteraction($scope.split);
         }
         map.removeInteraction(shape_interaction);
         map.addInteraction(shape_interaction);
         setHandleStyle();
     }

     //选择
     $scope.chose = function () {
         setInteraction('none');
         $scope.shapedFeature = [];
         $scope.chosedFeature = [];
         if (typeof (shape_interaction) !== 'undefined' && !!shape_interaction) {
             map.removeInteraction(shape_interaction);
         }
         if (typeof ($scope.split) !== 'undefined' && !!$scope.split) {
             map.removeInteraction($scope.split);
         }
         map.removeInteraction(interactions.select);
         map.removeInteraction(interactions.modify);
         setInteraction('modify');
     }

     //切割
     $scope.cut = function () {
         setInteraction('none');
         if (typeof (shape_interaction) !== 'undefined' && !!shape_interaction) {
             map.removeInteraction(shape_interaction);
         }
         if (typeof ($scope.split) !== 'undefined' && !!$scope.split) {
             map.removeInteraction($scope.split);
         }
         // 切割
         $scope.split = new ol.interaction.Split({ sources: $scope.vector.getSource() });
         map.addInteraction($scope.split);
     }

     //删除一个元素
     $scope.deleteOne = function () {
         setInteraction('none');
         if (typeof (shape_interaction) !== 'undefined' && !!shape_interaction) {
             map.removeInteraction(shape_interaction);
         }
         if (typeof (interactions.draw) !== 'undefined' && !!interactions.draw) {
             map.removeInteraction(interactions.draw);
         }
         if (typeof ($scope.split) !== 'undefined' && !!$scope.split) {
             map.removeInteraction($scope.split);
         }
         map.removeInteraction(interactions.select);
         var selectInteraction = new ol.interaction.Select({
             source: $scope.vector.getSource()
         });
         selectInteraction.on('select', function (e) {
             e.selected.forEach(function (f) {
                 $scope.vector.getSource().removeFeature(f);
                 selectInteraction.getFeatures().clear();
             });
         });
         map.addInteraction(selectInteraction);
     }

     //删除一个元素
     $scope.deleteTwo = function () {
         if ($scope.chosedFeature.length < 1) {
             alertFun('请先选择一个元素！', '', 'warning', '#007AFF');
             return;
         }
         for (var i = 0; i < $scope.chosedFeature.length; i++) {
             $scope.vector.getSource().removeFeature($scope.chosedFeature[i]);
             interactions.select.getFeatures().clear();
             $scope.deleteFeature.push($scope.chosedFeature[i]);
             $scope.chosedFeature.splice(i, 1);
         }
     }

     $scope.choseOne = function () {
         if ($scope.chosedFeature.length < 1) {
             alertFun('请先选择一个元素！', '', 'warning', '#007AFF');
             return;
         }
         for (var i = 0; i < $scope.chosedFeature.length; i++) {
             var fea = $scope.chosedFeature[i];
             var points = fea.getProperties().geometry.getCoordinates();
             console.log(fea);
         }
     }

     //清除
     $scope.deleteAll = function () {
         setInteraction('none');
         if (typeof (shape_interaction) !== 'undefined' && !!shape_interaction) {
             map.removeInteraction(shape_interaction);
         }
         if (typeof (interactions.draw) !== 'undefined' && !!interactions.draw) {
             map.removeInteraction(interactions.draw);
         }
         if (typeof ($scope.split) !== 'undefined' && !!$scope.split) {
             map.removeInteraction($scope.split);
         }
         $scope.newFeatures = [];
         $scope.chosedFeature = [];
         $scope.shapedFeature = [];

         map.getOverlays().clear();
         var featureDe = $scope.vector.getSource().getFeatures();
         $scope.vector.getSource().clear(featureDe);
     }
 }]);
