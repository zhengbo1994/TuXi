'use strict';
/**
 * dataInterfaceCtrl2 Controller
 */
app.controller('dataInterfaceCtrl2', ['$rootScope', '$scope', 'SweetAlert', '$element', '$filter', 'selfadapt', '$timeout', 'abp.services.app.map', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.setSys',

   function ($rootScope, $scope, SweetAlert, $element, $filter, selfadapt, $timeout, map, dataTag, dataType, setSys) {
       $rootScope.loginOut();
       $rootScope.homepageStyle = {};
       $rootScope.app.layout.isNavbarFixed = true;
       $rootScope.app.isWholeScreen = false;

       //调用实时随窗口高度的变化而改变页面内容高度的服务
       $scope.globalobj = $element;
       var unlink = selfadapt.anyChange($element);
       $scope.$on('$destroy', function () {
           unlink();
           selfadapt.showBodyScroll();
       });

       //地图的标签、分类的TYPE
       var overAllId = 'ba5ab799-6acd-11e7-87f3-005056bb1c7e';

       //分页
       $scope.maxSize = 5;//页码个数显示数
       $scope.goPage = 1;//转到多少页
       $scope.pageCounts = 0;//32;//总条数
       $scope.pageIndex = 1;//1;//起始页
       $scope.pageSize = 10;//10;//每页显示条数

       //分页的跳转事件
       $scope.pageChanged = function (a, evt) {
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
           $scope.pageIndex = $scope.pageIndex;
           mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
       };

       //搜索地图
       $scope.searchMap = function () {
           mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
       }

       /*--------------页面左边树形--------------start----*/
       //tab切换
       $scope.selectTab = function (state) {
           $scope.showtab1 = state;
           $scope.treeQueryCtrl1.select_branch($scope.typeTreeData[0]);
           $scope.onTypeSelected($scope.typeTreeData[0]);
       }

       //分类查询树
       $scope.typeTreeData = [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }];

       //当前节点,点击树时自动赋值
       $scope.typeSelectedData = "";

       //选中分类查询
       $scope.onTypeSelected = function (node) {
           //console.log(node);
           $scope.inputText = "";
           $scope.tagNodeId = "";
           $scope.typeNodeId = node.id;
           //刷新右侧列表
           mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
           $scope.treeQueryCtrl2.select_branch();
       };

       //标签查询树
       $scope.tagTreeData = [];
       //当前节点,点击树时自动赋值
       $scope.tagSelectedData = "";


       //选中标签查询
       $scope.onTagSelected = function (node) {
           //console.log(node);
           $scope.inputText = "";
           $scope.typeNodeId = "";
           $scope.tagNodeId = node.id;
           //刷新右侧列表
           mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
       };


       //实现树的折叠功能
       $scope.allChange = function (bo) {
           if (bo) {
               $scope.treeQueryCtrl1.expand_all();
               $scope.treeQueryCtrl2.expand_all();
           }
           else {
               $scope.treeQueryCtrl1.collapse_all();
               $scope.treeQueryCtrl2.collapse_all();
           }
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
               });
           });
       });
       //标签查询树的数据
       dataTag.getAllListByDataType(overAllId).success(function (data, statues) {
           for (var i in data.items) {
               var tempTagData = {};
               tempTagData.id = data.items[i].id;
               tempTagData.dictCodeID = data.items[i].dictCodeID;
               tempTagData.label = data.items[i].tagName;
               tempTagData.tagDesc = data.items[i].tagDesc;
               tempTagData.children = [];
               $scope.tagTreeData = $scope.tagTreeData.concat(tempTagData);
           }
       });

       /*--------------页面左边树形--------------end----*/

       //分页查询地图数据
       function mapInit(mapName, mapType, mapTag, pageSize, pageIndex) {

           $scope.mapServiceList = [];
           map.getPageListByName({ MapName: mapName, MapType: mapType, MapTag: mapTag, CreateUserId: localStorage.getItem('infoearth_spacedata_userCode') }, pageSize, pageIndex).success(function (data, status) {
               //console.log(data);

               data.items.forEach(function (each) {
                   each.mapImgSrc = '/Thumbnail/map/' + each.mapEnName + '.png';
                   //each.mapImgSrc = GeoServerUrl + '/wms?service=WMS&version=1.1.0&request=GetMap&layers=' + WorkSpace + ':' + each.mapEnName + '&styles=&bbox=' + each.minX + ',' + each.minY + ',' + each.maxX + ',' + each.maxY + '&width=768&height=670&srs=EPSG:4326&format=image%2Fvnd.jpeg-png';
                   if (each.mapTag.length > 20) {
                       each.mapTag = each.mapTag.substr(0, 20) + '...';
                   }
                   if (each.createDT != null) {
                       each.createDT = each.createDT.replace('T', ' ');
                   }
               });
               !!data && ($scope.mapServiceList = data.items, $scope.pageCounts = data.totalCount);
               $scope.pageCounts = data.totalCount;
           });
       }
   }]);
