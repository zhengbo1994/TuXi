'use strict';
/**
 * dataInterfaceCtrl2 Controller
 */
app.controller('mapServicePageCtrl', ['$rootScope', '$scope', 'SweetAlert', '$filter', '$element', 'selfadapt', '$timeout', 'waitmask', 'abp.services.app.map', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.setSys', 'abp.services.app.serverInterface', '$http',
 function ($rootScope, $scope, SweetAlert, $filter, $element, selfadapt, $timeout, waitmask, mapSearch, dataTag, dataType, setSys, serverInterface, $http) {
     $rootScope.loginOut();
     $rootScope.homepageStyle = {};
     //调用实时随窗口高度的变化而改变页面内容高度的服务
     $scope.globalobj = $element;
     var unlink = selfadapt.anyChange($element);
     $scope.$on('$destroy', function () {
         unlink();
         selfadapt.showBodyScroll();
     });
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
     //是否展示链接
     $scope.showLink = false;
     //是否展示地图
     $scope.showMap = false;
     //是否展示参数输入框
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

     //地图图层
     var layer, map;
     function loadMaplayer(type, url) {
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
     }

     /*--------------页面左边--------------start----*/

     //树形数据
     $scope.typeTreeData3 = [
      {
          label: "OGC服务", children: [
                  { label: "WMS", name: "WMS", callFun: function () { getServerInterface1("WMS"); }, children: [] },
                  { label: "WMTS", name: "WMTS", callFun: function () { getServerInterface1("WMTS"); }, children: [] },
                  { label: "iTelluro", name: "iTelluro", callFun: function () { getServerInterface1("iTelluro"); }, children: [] },
                  { label: "WFS", name: "WFS", callFun: function () { getServerInterface1("WFS"); }, children: [] },
                  { label: "KML", name: "KML", callFun: function () { getServerInterface1("KML"); }, children: [] }
          ]
      }, {
          label: "数据接口", children: [
                  { label: "查询地图的图层api", name: "MapAPI", callFun: function () { getServerInterface2("MapAPI"); }, children: [] },
                  { label: "查询图层的属性api", name: "LayerAPI", callFun: function () { getServerInterface2("LayerAPI"); }, children: [] },
                  { label: "图层空间查询api", name: "SpaceAPI", callFun: function () { getServerInterface2("SpaceAPI"); }, children: [] }

          ]
      }];


     $scope.selectedData3 = "";//当前节点,点击树时自动赋值
     $scope.layerURL = "";

     //选中回调函数
     $scope.onSelected3 = function (node, getParentNodeBackFun) {
         if (!node.callFun) return;
         node.callFun();         
         clearData();
         $scope.params = {};
         $scope.searchServiceURL = "";

         $scope.showTable = false;
     };

     //请求后台获取服务数据
     function getServerInterface1(type) {
         $scope.currentLayerInfo = [];
         $scope.layerURL = "";
         $scope.showInput = false;
         $scope.showMap = true;

         //重置WFS图层
         $scope.showWFS = false;
         $scope.isShowChose = false;
         $scope.wfsLayerData = [];

         $timeout(setHeight, 200);
         $scope.showLink = false;
         serverInterface.getServerInterface(currentData.id, type).success(function (data, statues) {
             //console.log(data);
             $scope.layerURL = data.items[0].serverPath;
             $scope.responseP = false;
             $scope.currentLayerInfo = data.items;
             //console.log(url);
             
             if (type === 'WFS') {
                 $scope.showWFS = true;
                 $scope.isShowChose = true;
                 for (var i = 0; i < data.items[0].serverRequestParameters.items.length; i++) {
                     var tmpItem = data.items[0].serverRequestParameters.items[i];
                     if (tmpItem.parameterName === "typeNames") {
                         var tmpParaValue = tmpItem.parameterValue.split(', ');
                         for (var j = 0; j < tmpParaValue.length; j++) {
                             //var tmpName = tmpParaValue[j].replace("gisdatamanage:", "");
                             var tmpObj = {
                                 id: "gisdatamanageID" + j,
                                 value: tmpParaValue[j],
                                 ischecked: j == 0 ? true : false
                             };
                             $scope.wfsLayerData = $scope.wfsLayerData.concat(tmpObj);
                         }
                         //$scope.wfsCheckedOne($scope.wfsLayerData[0]);
                         break;
                     }
                 }
             }
             loadMaplayer(type, $scope.layerURL);
         }).error(function (data, statues) {
             console.log(data);
         });
         //if (type == "KML" || type == "WFS") {
         //    $scope.showMap = false;
         //    $scope.showLink = true;
         //    serverInterface.getServerInterface(currentData.id, type).success(function (data, statues) {
         //        //console.log(data);
         //        var url = data.items[0].serverPath;
         //        $scope.responseP = false;
         //        $scope.currentLayerInfo = data.items;
         //    });
         //}
         //else {
         //    $scope.showMap = true;
         //    $timeout(setHeight, 200);
         //    $scope.showLink = false;
         //    serverInterface.getServerInterface(currentData.id, type).success(function (data, statues) {
         //        //console.log(data);
         //        var url = data.items[0].serverPath;
         //        $scope.responseP = false;
         //        $scope.currentLayerInfo = data.items;
         //        loadMaplayer(type, url);
         //    });
         //}
     }
     //请求后台获取服务数据
     function getServerInterface2(type) {
         $scope.currentLayerInfo = [];
         $scope.showMap = false;
         $scope.showLink = true;
         $scope.showInput = true;
         $scope.showWFS = false;
         $scope.isShowChose = false;
         $scope.wfsLayerData = [];
         serverInterface.getDataInterface(currentData.id, type).success(function (data, statues) {
             //console.log(data);

             $scope.responseP = true;
             $scope.currentLayerInfo = data.items;
         });
     }

     //点击链接之后执行的方法
     $scope.serverPathClick = function (obj) {
         //console.log(obj);
         if (obj.serverName === 'KML') {
             //请求下载
             window.location.href = obj.serverPath;
         }
         else if (obj.serverName === "GetFeature") {
             //请求下载
             $scope.showWFS = true;
             $http.get(obj.serverPath).success(function (res, status) {
                 //console.log(res);

                 console.log(getXML(res));
             }).error(function (res, status) {
                 console.log(res);
                 alertFun($filter('translate')('views.Interface.info.requestFail'), '', 'error', '#007AFF');
             });
         }
         else {
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
         }
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
                 alertFun($filter('translate')('views.Interface.info.requestFail'), '', 'error', '#007AFF');
                 waitmask.onHideMask();
             });
         }

         $scope.realURL = url;
         //console.log($scope.realURL);
         getData($scope.realURL, $scope.pageIndex, $scope.pageSize);
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
         loadMaplayer("WFS", tmpParams);
     }

     //自适应高度  
     function setHeight() {
         var winH = angular.element(window).height();
         var contentH = (winH - 65 - 12 - 30 - 10);
         $scope.mapContentSty = {
             'height': contentH + 9 + 'px'
         };
         $scope.mapheight = contentH;
         //console.log($scope.mapheight);
         angular.element(".mapDiv").height(contentH);

     }
     //$timeout(setHeight, 200);
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
             waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
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
                     alertFun($filter('translate')('views.Interface.info.requestFail'), '', 'error', '#007AFF');
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
                     alertFun($filter('translate')('views.Interface.info.requestFail'), '', 'error', '#007AFF');
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


     //分页
     $scope.maxSize = 3;//页码个数显示数
     $scope.goPage = 1;//转到多少页
     $scope.pageCounts = 0;//32;//总条数
     $scope.pageIndex = 1;//1;//起始页
     $scope.pageSize = 10;//10;//每页显示条数

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
 }]);
