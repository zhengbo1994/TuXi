'use strict';
/**
 * mapManagerCtrl Controller
 */
app.controller('mapBrowseCtrl',
    ['$rootScope', '$scope', '$document', '$filter', 'SweetAlert', '$element', "$timeout", 'waitmask', 'abp.services.app.map', 'abp.services.app.dicDataCode', 'abp.services.app.mapReleation', 'abp.services.app.layerContent', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.tagReleation', 'abp.services.app.dataStyle',
  function ($rootScope, $scope, $document, $filter, SweetAlert, $element, $timeout, waitmask, map, dicDataCode, mapReleation, layerContent, dataTag, dataType, tagReleation, dataStyle) {
      $rootScope.loginOut();
      $rootScope.homepageStyle = {};
      //把地图之外的所有元素隐藏掉
      $rootScope.app.layout.isNavbarFixed = false;
      $rootScope.app.isWholeScreen = true;

      var scaleId = '46f30c2a-6cf2-11e7-9d4f-005056bb1c7e';
      var overAllId = 'ba5ab799-6acd-11e7-87f3-005056bb1c7e';
      var styleTypeID = 'c755eeea-986d-11e7-90b1-005056bb1c7e';
      var layerAllId = 'a2faae61-6acd-11e7-87f3-005056bb1c7e';

      $scope.maxSize = 5;//页码个数显示数
      $scope.goPage = 1;//转到多少页
      $scope.pageCounts = 0;//32;//总条数
      $scope.pageIndex = 1;//1;//起始页
      $scope.pageSize = 10;//10;//每页显示条数

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
      $scope.currentData = {
          "id": GetQueryString("id"),
          "pagetype": GetQueryString("type")
      }

      //----地图数据
      $scope.mapDataset = mapDataset;
      $scope.isLoadTianDiTu = parseInt(isLoadTianDiTu);
      //用于获取画方画圆后的坐标点
      $scope.getExtent = [];
      $scope.location = [];
      //map对象
      $scope.mapObj = null;

      //选择哪个地图服务
      $scope.mapServiceNum = 2;
      //加载地图
      var layer, legend = GeoServerWmsUrl + '?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=20&HEIGHT=20&LAYER=' + WorkSpace + ':';
      $scope.mapSerShow = function (mapIndex, serviceIndex, item) {
          if ($scope.mapServiceNum === serviceIndex && !mapIndex) {
              return;
          }
          //服务信息窗口
          $scope.isshowSerInfo = true;
          $scope.mapServiceNum = serviceIndex;
          $scope.serviceInfo = item;
          $scope.legend = legend + item.mapEnName;

          if (layer) {
              $scope.removeLayer(layer);
          }
          //加时间戳去缓存
          var urlTemplate = '?_' + new Date().getTime() + Math.random();

          if ($scope.mapServiceNum === 1) {
              //WMS
              layer = newLocalTilesByWMS(GeoServerUrl + '/wms', WorkSpace + ':' + item.mapEnName, 'image/png');
          }
          else if ($scope.mapServiceNum === 2) {
              //WMTS
              layer = newLocalTilesByWMTS(GeoServerUrl + '/gwc/service/wmts' + urlTemplate, WorkSpace + ':' + item.mapEnName);
          }
          else if ($scope.mapServiceNum === 3) {
              //iTelluro
              var tmpServiceUrl = GeoServerUrl.split('/')[0] + "/Service/GIS/gis.ashx";
              layer = new iTelluro().newItelluroLayer(item.mapName, tmpServiceUrl, 512, 36);
          }

          layer.setZIndex(27);
          //layer.getSource().updateParams({ "_": new Date().getTime()+ Math.random() });
          var bounds = [];
          if (!!$scope.serviceInfo.minX) {
              bounds = [$scope.serviceInfo.minX, $scope.serviceInfo.minY, $scope.serviceInfo.maxX, $scope.serviceInfo.maxY];//范围
          }
          else {
              bounds = [70, 0, 140, 60];//范围
          }
          var map = $scope.addLayer(layer, bounds);
          map.updateSize();
      }
      
      //根据跳转过来的mapManageModal.id查询地图信息
      if ($scope.currentData.pagetype == 1) {
          $scope.mapSearchTool = false;
          getMap($scope.currentData.id);
          $scope.pageTitle = 1;
      }
      else {
          //如果是地图浏览页面，给选择图层model赋值
          $scope.mapSearchTool = true;
          getMap($scope.currentData.id, {}, function () {
              $scope.choseLayerModel.layerData = angular.copy($scope.mapManageModal.mapManage_layers);
              //默认选中所有图层
              for (var i in $scope.choseLayerModel.layerData) {
                  $scope.choseLayerModel.layerData[i].ischecked = true;
              }
              $scope.choseLayerModel.ischecked1 = true;
          });
          $scope.pageTitle = 2;
      }

      //----页面右侧按钮组
      var imgDom = angular.element("#dowebok")[0];
      var viewer = new Viewer(imgDom, {
          "toolbar": false,
      });
      //查看大图
      $scope.lookPitcure = function (imgsrc) {
          viewer.show();
      }
      //显示按钮的样式
      $scope.activeShowBtn = 0;
      $scope.qiehuanBorderStyle = {
          'padding-bottom': '9px'
      }
      //点击显示某一个弹窗
      $scope.showRightWin = function (num) {
          $scope.activeShowBtn = num;
          var btnCssTop = angular.element('.container-fluid .righttag:eq(0)').css('top');
          var top = Math.ceil(btnCssTop.split('px')[0]);
          if (num === 1) {
              if ($scope.currentData.pagetype == 1) {
                  $scope.mapManageModal.title = $filter('translate')('views.Map.List.mapManage');
                  $scope.mapManageModal.typePullDownTreeData = angular.copy($scope.typeTreeData[0].children);
                  $scope.openMapManageFun();
              }
              $scope.qiehuanBorderStyle = {
                  'padding-bottom': '9px',
              }
          }
          else if (num == 2) {
              $scope.qiehuanBorderStyle = {
                  'padding-bottom': '12px',
              }
              top = top + 38;
              $scope.choseServiceWinStyle = {
                  'top': top + 'px'
              }
          }
      }
      //关闭上面的窗口
      $scope.closeMapWin = function () {
          $scope.activeShowBtn = 0;
          $scope.qiehuanBorderStyle = {
              'padding-bottom': '9px',
          }
      }
      //切换地图服务
      $scope.changeService = function (num) {
          $scope.mapSerShow(false, num, $scope.serviceList);
      }

      //打开绑定图层弹窗的按钮
      $scope.editLayer = function () {
          layerContent.getAllListByMapID($scope.currentData.id).success(function (data1, status1) {
              //console.log('555', data1);
              $scope.layerManageModal.layerTo = data1.items;
              $scope.layerManageModal.selectedLayer = goSelectedLayer(angular.copy(data1.items));
              $scope.layerManageModal.typeTreeData = angular.copy($scope.layerTypeTreeData);
              $scope.layerManageModal.tagTreeData = angular.copy($scope.layerTagTreeData);
              $scope.layerManageModal.title = $filter('translate')('views.Map.mapBro.selectLayer');
              $scope.editLayerModel();
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //地图分类树
      $scope.typeTreeData = [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }];
      //图层标签树
      $scope.layerTagTreeData = [];
      //图层分类树
      $scope.layerTypeTreeData = [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }];



      /*--------------地图管理弹窗--------------start----*/
      //监听标签输入框
      $scope.$watch('mapManageModal.tagsInput', function (val, old) {
          if (myBrowser() === 'Chrome') {
              return;
          }
          $document.find('.mytagsfocus').blur();
          $timeout(function () {
              $document.find('.mytagsfocus').focus();
              if (val.length > 20) {
                  $scope.mapManageModal.tagsInput = $scope.mapManageModal.tagsInput.substr(0, 20);
              }
              if ($scope.mapManageModal.tagsList.length === 5) {
                  $scope.mapManageModal.tagsInput = '';
                  return;
              }
              if (val.lastIndexOf(' ') !== -1) {
                  $timeout(function () {
                      $scope.mapManageModal.tagsInput = $.trim(val).replace(',', '');
                  });
              }
              if (val.lastIndexOf('，') === -1) return;
              $scope.mapManageModal.setdata(old);
              $scope.mapManageModal.tagsInput = old;
          });
      });
      
      var url = "http://192.168.100.18:8088/iTelluro.Server.TianDiTu/Service/DOM/dom.ashx";
      //地图管理modal
      $scope.mapManageModal = {
          title: "",
          id: "",
          mapName: '',
          mapEnName: '',
          createUserId: '',
          spatialRefence: SpatialRefence,
          //分类
          typePullDownTreeData: [],
          typePullDownTreeSelData: {},
          currentType: '',
          onSelected: function (node) {
              //console.log(node);
              $scope.mapManageModal.typePullDownTreeSelData = node;
          },
          //标签
          mapTag: '',
          tagsInput: '',
          tagsList: [],
          //标签输入框的方法
          delItems: function (a, b) {
              for (var i = 0; i < this.tagsList.length; i++) {
                  if ($scope.mapManageModal.tagsList[i] === a) {
                      this.tagsList.splice(i, 1);
                      angular.element(b.target.parentNode).remove();
                  }
              }
          },
          keydown: function (evt, val) {
              var self = this;
              if (this.tagsList.length === 5) {
                  if (evt.keyCode === 32) {
                      self.tagsInput = ',';
                  }
                  $timeout(function () {
                      self.tagsInput = '';
                  });
                  return;
              }
              if (evt.keyCode === 188) {
                  this.setdata(val);
              }
              else if (evt.keyCode === 32) {
                  this.setdata(val);
                  self.tagsInput = ',';
              }
          },
          setdata: function (val) {
              var self = this;
              var v = angular.copy(val);
              v = $.trim(v);
              if (!!v && self.isdiff(v)) {
                  v = v.replace(/ /g, '').replace(/，/g, '');
                  this.tagsList.push(v);
              }
              $timeout(function () {
                  self.tagsInput = '';
              });
          },
          isdiff: function (val) {
              for (var i in this.tagsList) {
                  if (this.tagsList[i] === val) {
                      return false;
                  }
              }
              return true;
          },
          //地图描述
          mapDesc: '',
          //边界范围
          minX: "",
          maxX: "",
          minY: "",
          maxY: "",
          xy: [],
          mapManage_layers: [],
          //图例
          mapLegend: '',
          mapLegendName: '',

          //设置样式
          setStyle: function (item) {
              //console.log(item);
              layerContent.getDetailById(item.dataConfigID).success(function (data, status) {
                  //console.log(data);
                  $scope.styleModal.id = item.id;
                  $scope.styleModal.dataConfigID = item.dataConfigID;
                  $scope.styleModal.styleSelectData = {};
                  $scope.styleModal.styleSelectData.id = !!item.dataStyleID ? item.dataStyleID : "";
                  $scope.styleModal.styleDataType = data.dataType;
                  $scope.styleModal.title = $filter('translate')('views.Style.setStyle');
                  $scope.getStyleTypeTree();
                  $scope.styleModal.layerDataSort = !!item.dataSort ? item.dataSort : 1;
                  $scope.opensStyleModal();
              }).error(function (data, status) {
                  //console.log(data);
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });
          },
          //编辑样式
          editStyle: function (id) {
              var styleObj = { id: id };
              $scope.getStyleTypeTree();
              $rootScope.edit(styleObj, null, function (data, status) {
                  var obj = {
                      mapEnName: $scope.mapManageModal.mapEnName,
                      mapName: $scope.mapManageModal.mapName,
                      minX: $scope.mapManageModal.minX,
                      maxX: $scope.mapManageModal.maxX,
                      minY: $scope.mapManageModal.minY,
                      maxY: $scope.mapManageModal.maxY
                  };
                  $scope.mapSerShow(true, $scope.mapServiceNum, obj);
              });
          },
          //删除样式
          delStyle: function (id, dataConfigID, dataSort) {
              SweetAlert.swal({
                  title: $filter('translate')('setting.delete'),
                  text: $filter('translate')('views.Style.DeleteTips'),
                  type: "warning",
                  showCancelButton: true,
                  confirmButtonColor: "#DD6B55",
                  confirmButtonText: $filter('translate')('setting.sure'),
                  cancelButtonText: $filter('translate')('setting.cancel')
              }, function (isConfirm) {
                  if (isConfirm) {
                      waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                      var mapReleaData = { Id: id, MapID: $scope.mapManageModal.id, DataConfigID: dataConfigID, DataStyleID: null, DataSort: dataSort };
                      //console.log(mapReleaData);
                      mapReleation.update(mapReleaData).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                      }).success(function (data, status) {
                          //console.log(data);
                          //获取地图对应的图层
                          mapReleation.getAllListByMapId($scope.mapManageModal.id).success(function (data, status) {
                              //console.log(data);
                              var styleIdArr = [], layerIdArr = [];
                              data.items.forEach(function (each) {
                                  styleIdArr.push(each.dataStyleID);
                                  layerIdArr.push(each.dataConfigID);
                              });

                              var obj = {
                                  MapId: $scope.mapManageModal.id,
                                  LayerStr: layerIdArr.join(','),
                                  StyleStr: styleIdArr.join(',')
                              };
                              //重新发布地图
                              mapReleation.changeStyleObject(obj).success(function (data1, status1) {
                                  //console.log(data1, status1);
                                  getMap($scope.mapManageModal.id, function () {
                                      waitmask.onHideMask();
                                      alertFun($filter('translate')('views.Style.delStyleSucc'), "", "success", "#007AFF");
                                      //$scope.randomCount = (new Date()).getTime() + getRandom();
                                      $scope.activeShowBtn = 0;
                                  });
                              }).error(function (data, status) {
                                  waitmask.onHideMask();
                                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                              });
                          }).error(function (data, status) {
                              waitmask.onHideMask();
                              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                          });
                      });
                  }
              });
          },

          opened: function () { },
          submit: function (modalInstance, form) {
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);

              var mapInfo = {
                  Id: $scope.mapManageModal.id,
                  MapName: $scope.mapManageModal.mapName,
                  MapType: $scope.mapManageModal.typePullDownTreeSelData.id,
                  MapTag: $scope.mapManageModal.tagsList.join(','),
                  SpatialRefence: $scope.mapManageModal.spatialRefence,
                  //MapScale: typeof ($scope.mapManageModal.scaleSel.selected) === "undefined" ? '' : $scope.mapManageModal.scaleSel.selected.id,
                  MapDesc: $scope.mapManageModal.mapDesc,
                  createUserId: $scope.mapManageModal.createUserId
              };

              //更新地图数据  图层与地图关系保存在‘图层管理’里
              map.update(mapInfo).success(function (data, status) {
                  //console.log(data);
                  //保存标签与地图对应关系
                  var args = {
                      tagName: $scope.mapManageModal.tagsList.join(','),
                      dataID: overAllId,
                      mapLayerID: data.id
                  }
                  tagReleation.multiInsert(args).success(function (data1, statues) {
                      //console.log(data);
                      if (!data1) {
                          alertFun($filter('translate')('views.Layer.alertFun.set.setTagErr'), '', 'error', '#007AFF');
                      }
                      getMap(mapInfo.Id, function () {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('views.Map.alertFun.editMapSucc'), "", "success", "#007AFF");
                          //$scope.randomCount = (new Date()).getTime() + getRandom();
                          $scope.activeShowBtn = 0;
                          modalInstance.close();
                      });
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              }).error(function (data, status) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });
          },
          cancel: function () {
              $scope.activeShowBtn = 0;
          }
      }
      /*--------------地图管理弹窗--------------end----*/


      /*--------------绑定图层弹窗--------------end----*/
      $scope.layerManageModal = {
          tagTreeData: [],
          typeTreeData: [],
          typeTreeQueryCtrl: {},
          showtab: true,
          selectTab: function (state) {
              $scope.layerManageModal.showtab = state;
              $scope.layerManageModal.typeTreeQueryCtrl.select_branch($scope.layerManageModal.typeTreeData[0]);
              $scope.layerManageModal.onTypeSelected($scope.layerManageModal.typeTreeData[0]);
          },
          typeSelected: {},
          onTypeSelected: function (node) {
              $scope.layerManageModal.pageing.pageIndex = 1;
              $scope.layerManageModal.layerName = "";
              $scope.getLayerFromMap(node.id, '', '', $scope.mapManageModal.id, $scope.layerManageModal.pageing.pageSize, 1);
              $scope.layerManageModal.tagTreeQueryCtrl.select_branch();
          },
          tagSelected: {},
          onTagSelected: function (node) {
              $scope.layerManageModal.pageing.pageIndex = 1;
              $scope.layerManageModal.layerName = "";
              $scope.getLayerFromMap('', node.id, '', $scope.mapManageModal.id, $scope.layerManageModal.pageing.pageSize, 1);
          },
          layerFrom: [],
          layerTo: [],
          checkedDataF: [],
          //选中一行
          onTdChecked: function (row) {
              var isSame = 0;
              for (var i = 0; i < $scope.layerManageModal.layerTo.length; i++) {
                  if (row.id === $scope.layerManageModal.layerTo[i].id) {
                      if (row.ischecked) {
                          isSame++;
                          break;
                      }
                      else {
                          $scope.layerManageModal.layerTo.splice(i, 1);
                          break;
                      }
                  }
              }
              if (isSame === 0 && row.ischecked) {
                  $scope.layerManageModal.layerTo = $scope.layerManageModal.layerTo.concat(row);
              }
          },
          //选中全部
          onThChecked: function (allchecked) {
              for (var i = 0; i < $scope.layerManageModal.layerFrom.length; i++) {
                  var isSame = 0;
                  for (var j = 0; j < $scope.layerManageModal.layerTo.length; j++) {
                      if ($scope.layerManageModal.layerTo[j].id === $scope.layerManageModal.layerFrom[i].id) {
                          if (allchecked) {
                              isSame++;
                              break;
                          }
                          else {
                              $scope.layerManageModal.layerTo.splice(j, 1);
                              break;
                          }
                      }
                  }
                  if (isSame === 0 && allchecked) {
                      $scope.layerManageModal.layerTo = $scope.layerManageModal.layerTo.concat($scope.layerManageModal.layerFrom[i]);
                  }
              }
          },
          //已选中列表的操作栏
          tabBtnParams: [{
              name: "删除", click: function (row, name, event) {
                  for (var i = 0; i < $scope.layerManageModal.layerTo.length; i++) {
                      if ($scope.layerManageModal.layerTo[i].id === row.id) {
                          $scope.layerManageModal.layerTo.splice(i, 1);
                          break;
                      }
                  }
                  for (var j = 0; j < $scope.layerManageModal.layerFrom.length; j++) {
                      if ($scope.layerManageModal.layerFrom[j].id === row.id) {
                          $scope.layerManageModal.layerFrom[j].ischecked = false;
                          break;
                      }
                  }
              }
          }],

          selectedLayer: [],

          //分页点击事件回调函数
          pageChanged: function () {
              var typeId = typeof ($scope.layerManageModal.typeSelected.id) === "undefined" ? '' : $scope.layerManageModal.typeSelected.id;
              var tagId = typeof ($scope.layerManageModal.tagSelected.id) === "undefined" ? '' : $scope.layerManageModal.tagSelected.id;
              $scope.getLayerFromMap(typeId, tagId, $scope.layerManageModal.layerName, $scope.mapManageModal.id, $scope.layerManageModal.pageing.pageSize, $scope.layerManageModal.pageing.pageIndex);
          },
          pageing: { pageIndex: $scope.pageIndex, pageSize: $scope.pageSize, pageCounts: $scope.pageCounts, maxSize: 2 },
          layerName: '',
          //搜索图层
          searchLayerByName: function () {
              $scope.layerManageModal.pageing.pageIndex = 1;
              var typeId = typeof ($scope.layerManageModal.typeSelected.id) === "undefined" ? '' : $scope.layerManageModal.typeSelected.id;
              var tagId = typeof ($scope.layerManageModal.tagSelected.id) === "undefined" ? '' : $scope.layerManageModal.tagSelected.id;
              $scope.getLayerFromMap(typeId, tagId, $scope.layerManageModal.layerName, $scope.mapManageModal.id, $scope.layerManageModal.pageing.pageSize, 1);
          },
          open: function () { },
          submit: function (modalInstance, form) {
              if ($scope.layerManageModal.layerTo.length == 0) {
                  alertFun($filter('translate')('views.Map.alertFun.mapPrompt2'), '', 'warning', '#007AFF');
                  return;
              }

              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              $scope.layerManageModal.selectedLayer = goSelectedLayer(angular.copy($scope.layerManageModal.layerTo));
              //渲染地图管理页面的图层列表
              mapReleation.getAllListByMapId($scope.mapManageModal.id).success(function (data, status) {
                  //console.log('001', data);
                  var layerArr = [];
                  var layers = $scope.layerManageModal.selectedLayer;
                  layers.forEach(function (layer, i) {
                      var object = null;
                      data.items.forEach(function (each, index) {
                          if (layer.DataConfigID == each.dataConfigID) {
                              object = each;
                          }
                      });
                      if (object != null) {
                          layerArr.push({ MapID: $scope.mapManageModal.id, DataConfigID: object.dataConfigID, DataStyleID: object.dataStyleID, DataSort: object.dataSort });
                      }
                      else {
                          layerArr.push({ MapID: $scope.mapManageModal.id, DataConfigID: layer.DataConfigID, DataStyleID: null, DataSort: i });
                      }
                  });

                  //重新发布地图
                  mapReleation.multiUpdate($scope.mapManageModal.id, layerArr, localStorage.getItem('infoearth_spacedata_userCode')).success(function (data1, status1) {
                      //console.log('002', data1);
                      if (data1) {
                          //发布地图
                          mapReleation.publicMap($scope.mapManageModal.id).success(function (data3, statues3) {
                              //console.log('003', data3);
                              if (data3) {
                                  getMap($scope.mapManageModal.id, function () {
                                      waitmask.onHideMask();
                                      modalInstance.close();
                                      alertFun($filter('translate')('views.Map.alertFun.changeBindLayerSucc'), '', 'success', '#007AFF');
                                  });
                              }
                              else {
                                  alertFun($filter('translate')('views.Map.alertFun.loadMapErr'), '', 'error', '#007AFF');
                              }
                          }).error(function (data, status) {
                              waitmask.onHideMask();
                              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                          });
                      }
                      else {
                          getMap($scope.mapManageModal.id, function () {
                              waitmask.onHideMask();
                              alertFun($filter('translate')('views.Map.alertFun.publishErr'), '', 'error', '#007AFF');
                              modalInstance.close();
                          });
                      }
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              }).error(function (data, status) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });
          },
          cancel: function () { }
      };

      //删除图层关联
      $scope.delLayer = function (data, index) {
          //console.log(data);
          SweetAlert.swal({
              title: $filter('translate')('setting.delete'),
              text: $filter('translate')('views.Map.alertFun.delOneLayer'),
              type: "warning",
              showCancelButton: true,
              confirmButtonColor: "#DD6B55",
              confirmButtonText: $filter('translate')('setting.sure'),
              cancelButtonText: $filter('translate')('setting.cancel')
          }, function (isConfirm) {
              if (isConfirm) {
                  waitmask.onShowMask($filter('translate')('setting.waitText'), 200);

                  mapReleation.delete(data.id).success(function (data, status) {
                      var layerArr = [];
                      var tmpArr = angular.copy($scope.mapManageModal.mapManage_layers);
                      tmpArr.splice(index, 1);
                      tmpArr.forEach(function (each, index) {
                          layerArr.push({ MapID: $scope.mapManageModal.id, DataConfigID: each.dataConfigID, DataStyleID: each.dataStyleID, DataSort: index })
                      });
                      //console.log(layerArr);

                      //重新发布地图
                      rePublishMap(layerArr, $scope.mapManageModal.id, function () {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('views.Layer.alertFun.field.layerDelSucc'), '', 'success', '#007AFF');
                      });
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('views.Layer.alertFun.field.layerDelErr'), data.message, 'error', '#007AFF');
                  });
              }
          });
      }
      //图层顺序↑1
      $scope.upLayer = function (arr, index) {
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
          var layerArr = [];
          swapItems(arr, index, index - 1);
          arr.forEach(function (each, index) {
              layerArr.push({ MapID: $scope.mapManageModal.id, DataConfigID: each.dataConfigID, DataStyleID: each.dataStyleID, DataSort: index })
          });
          //console.log(layerArr);
          //重新发布地图
          rePublishMap(layerArr, $scope.mapManageModal.id, function () {
              waitmask.onHideMask();
          });
      }
      //图层顺序↓1
      $scope.downLayer = function (arr, index) {
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
          var layerArr = [];
          swapItems(arr, index, index + 1);
          arr.forEach(function (each, index) {
              layerArr.push({ MapID: $scope.mapManageModal.id, DataConfigID: each.dataConfigID, DataStyleID: each.dataStyleID, DataSort: index })
          });
          //console.log(layerArr);
          //重新发布地图
          rePublishMap(layerArr, $scope.mapManageModal.id, function () {
              waitmask.onHideMask();
          });
      }
      //拖拽
      $scope.dropComplete = function (index, data) {
          //console.log(index, data);
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
          var layerArr = [];
          var tmpArr = angular.copy($scope.mapManageModal.mapManage_layers);
          var sortNum = 0;
          for (var i = 0; i < tmpArr.length; i++) {
              if (tmpArr[i].id === data.id) {
                  sortNum = i;
                  break;
              }
          }
          var startNum = index < sortNum ? index : sortNum;
          var endNum = index > sortNum ? index : sortNum;
          if (index < sortNum) {
              for (var i = endNum; i > startNum; i--) {
                  swapItems(tmpArr, i, i - 1);
              }
          }
          if (index > sortNum) {
              for (var i = startNum; i < endNum; i++) {
                  swapItems(tmpArr, i, i + 1);
              }
          }

          tmpArr.forEach(function (each, index) {
              if (typeof (each) !== 'undefined') {
                  layerArr.push({ MapID: $scope.mapManageModal.id, DataConfigID: each.dataConfigID, DataStyleID: each.dataStyleID, DataSort: index })
              }
          });
          //console.log(layerArr);

          //重新发布地图
          rePublishMap(layerArr, $scope.mapManageModal.id, function () {
              waitmask.onHideMask();
          });
      }

      function swapItems(arr, index1, index2) {
          arr[index1] = arr.splice(index2, 1, arr[index1])[0];
          return arr;
      }

      //修改图层之后重新发布地图
      function rePublishMap(layerArr, id, func) {
          mapReleation.multiUpdate(id, layerArr, localStorage.getItem('infoearth_spacedata_userCode')).success(function (data1, status1) {
              //console.log(data1);
              if (data1) {
                  //发布地图
                  mapReleation.publicMap(id).success(function (data3, statues3) {
                      //console.log(data3);
                      getMap(id, function () {
                          if (typeof (func) === "function") {
                              func();
                          }
                      });
                  });
              }
              else {
                  getMap(id, function () {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('views.Map.alertFun.publishErr'), "", "error", "#007AFF");
                  });
              }
          }).error(function (data, status) {
              waitmask.onHideMask();
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }
      /*--------------绑定图层弹窗--------------end----*/


      /*--------------设置样式弹窗--------------start----*/
      //设置样式
      $scope.styleModal = {
          title: '',
          id: '',
          //图层id
          dataConfigID: '',
          //图层排序
          layerDataSort: '',
          styleArr: [],
          styleName: '',
          //点线面分类ID
          styleDataType: "",
          searchStyleText: '',
          //样式分类树
          styleTypeTreeData: [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }],
          styleTypeSelected: {},
          //选中分类查询
          onStyleTypeSelected: function (node) {
              //console.log(node);
              $scope.styleModal.searchStyleText = "";
              $scope.styleModal.pageIndex = 1;
              //刷新右侧列表
              setStyleInit($scope.styleModal.searchStyleText, node.id, $scope.styleModal.styleDataType, $scope.styleModal.pageSize, $scope.styleModal.pageIndex);
          },
          //样式设置表格中的数据
          styleData: [],
          styleSelectData: {},
          choiceTR: function (tr) {
              //console.log(tr);
              $scope.styleModal.styleSelectData = {};
              if (tr.checked) {
                  for (var i in $scope.styleModal.styleData) {
                      if ($scope.styleModal.styleData[i].id !== tr.id) {
                          $scope.styleModal.styleData[i].checked = false;
                      }
                  }
                  $scope.styleModal.styleSelectData = angular.copy(tr);
              }
              //console.log($scope.styleModal.styleSelectData);
          },

          //样式表格分页
          maxSize: 4,//页码个数显示数
          goPage: 1,//转到多少页
          pageCounts: 0,//32;//总条数
          pageIndex: 1,//1;//起始页
          pageSize: 10,//10;//每页显示条数
          //分页的事件方法
          pageChanged: function (a, evt) {
              if (evt && evt.keyCode !== 13) { return; }//注：回车键为13
              if (a) {
                  a = parseInt(a);
                  if (isNaN(a) || a < 1 || a > $scope.styleModal.totalPages) {
                      $scope.styleModal.goPage = $scope.styleModal.pageIndex;
                      return;
                  }
                  $scope.styleModal.goPage = a;
                  $scope.styleModal.pageIndex = a;
              }
              //console.log({ MapName: $scope.inputText, MapType: $scope.typeNodeId });
              //调用AJAX
              setStyleInit($scope.styleModal.searchStyleText, $scope.styleModal.styleTypeSelected.id, $scope.styleModal.styleDataType, $scope.styleModal.pageSize, $scope.styleModal.pageIndex);
          },
          searchStyle: function () {
              $scope.styleModal.pageIndex = 1;
              setStyleInit($scope.styleModal.searchStyleText, $scope.styleModal.styleTypeSelected.id, $scope.styleModal.styleDataType, $scope.styleModal.pageSize, $scope.styleModal.pageIndex);
          },
          //添加样式
          addStyle: function () {
              $rootScope.addStyleWin(null, function (data, status) {
                  $scope.styleModal.pageIndex = 1;
                  $scope.styleModal.searchStyleText = "";
                  setStyleInit("", $scope.styleModal.styleTypeSelected.id, $scope.styleModal.styleDataType, $scope.styleModal.pageSize, $scope.styleModal.pageIndex);
              }, $scope.styleModal.styleDataType);
          },

          open: function () { },
          submit: function (modalInstance, form) {
              if (!$scope.styleModal.styleSelectData.id) {
                  alertFun($filter('translate')('views.Style.setStylePrompt'), '', 'warning', '#007AFF');
                  return;
              }
              //样式与图层关联
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              var mapReleaData = {
                  Id: $scope.styleModal.id,
                  MapID: $scope.mapManageModal.id,
                  DataConfigID: $scope.styleModal.dataConfigID,
                  DataStyleID: $scope.styleModal.styleSelectData.id,
                  DataSort: $scope.styleModal.layerDataSort
              };
              //console.log(mapReleaData);
              mapReleation.update(mapReleaData).error(function (data, status) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              }).success(function (data, status) {
                  //console.log(data);
                  //获取地图对应的图层
                  mapReleation.getAllListByMapId($scope.mapManageModal.id).success(function (data, status) {
                      //console.log(data);
                      var styleIdArr = [], layerIdArr = [];

                      data.items.forEach(function (each) {
                          styleIdArr.push(each.dataStyleID);
                          layerIdArr.push(each.dataConfigID);
                      });
                      var obj = {
                          MapId: $scope.mapManageModal.id,
                          LayerStr: layerIdArr.join(','),
                          StyleStr: styleIdArr.join(',')
                      };
                      //重新发布地图
                      mapReleation.changeStyleObject(obj).success(function (data1, status1) {
                          //console.log(data1, status1);
                          getMap($scope.mapManageModal.id, function () {
                              waitmask.onHideMask();
                              alertFun($filter('translate')('views.Style.setStyleSucc'), "", "success", "#007AFF");
                              modalInstance.close();
                          });
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                      });
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              });
          },
          cancel: function () { }
      };

      //分页查询样式列表
      function setStyleInit(styleName, styleType, styleDataType, pageSize, pageIndex) {
          $scope.styleModal.styleData = [];
          dataStyle.getAllListPage({ StyleName: styleName, StyleType: styleType, StyleDataType: styleDataType, CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }, pageSize, pageIndex).success(function (data, status) {
              //console.log(data);
              $scope.styleModal.pageCounts = data.totalCount;
              $scope.styleModal.totalPages = Math.ceil($scope.styleModal.pageCounts / $scope.styleModal.pageSize);
              $scope.styleModal.styleData = angular.copy(data.items);
              for (var i in $scope.styleModal.styleData) {
                  $scope.styleModal.styleData[i].checked = false;
                  if (!!$scope.styleModal.styleSelectData.id) {
                      if ($scope.styleModal.styleData[i].id === $scope.styleModal.styleSelectData.id) {
                          $scope.styleModal.styleData[i].checked = true;
                      }
                  }
              }

          }).error(function (data, status) {
              //console.log(data);
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }
      /*--------------设置样式弹窗--------------end----*/


      /*--------------选择图层窗口--------------start----*/
      //点选查出的数据条数
      $scope.dataNum = 0;
      //中心点
      $scope.centerPoint = [];
      //展示搜索结果窗口
      $scope.isShowRes = false;
      //展示选择图层窗口
      $scope.isShowChose = false;
      $scope.treectrlObj = {};

      //显示/隐藏选择图层的面板
      $scope.hideChose = function () {
          $scope.isShowChose = !$scope.isShowChose;
      }

      $scope.choseLayerModel = {
          //全选按钮的选择状态
          ischecked1: false,
          //可选择的图层列表
          layerData: [],
          layerId: "",
          //全选
          checkedAll: function () {
              if ($scope.choseLayerModel.ischecked1) {
                  for (var i in $scope.choseLayerModel.layerData) {
                      $scope.choseLayerModel.layerData[i].ischecked = false;
                  }
              }
              else {
                  for (var i in $scope.choseLayerModel.layerData) {
                      $scope.choseLayerModel.layerData[i].ischecked = true;
                  }
              }
              $scope.choseLayerModel.ischecked1 = !$scope.choseLayerModel.ischecked1;
          },
          //单选
          checkedOne: function (a) {
              var isAll = 0;
              for (var i in $scope.choseLayerModel.layerData) {
                  if (a.dataConfigID === $scope.choseLayerModel.layerData[i].dataConfigID) {
                      $scope.choseLayerModel.layerData[i].ischecked = !$scope.choseLayerModel.layerData[i].ischecked;
                  }
                  if ($scope.choseLayerModel.layerData[i].ischecked) {
                      isAll++;
                  }
              }
              if (isAll === $scope.choseLayerModel.layerData.length) {
                  $scope.choseLayerModel.ischecked1 = true;
              }
              else {
                  $scope.choseLayerModel.ischecked1 = false;
              }
          }
      };
      /*--------------选择图层窗口--------------end----*/


      /*--------------地图查询操作--------------start----*/
      $scope.measuremapBefore = function () {
          $scope.isShowLayerCh = true;
          $scope.isShowChose = true;
          $scope.choseLayerModel.layerId = "";
          for (var i = 0; i < $scope.choseLayerModel.layerData.length; i++) {
              if ($scope.choseLayerModel.layerData[i].ischecked) {
                  $scope.choseLayerModel.layerId += $scope.choseLayerModel.layerData[i].dataConfigID + ',';
              }
          }
          if (!$scope.choseLayerModel.layerId) {
              alertFun($filter('translate')('views.Map.alertFun.mapPrompt2'), '', 'warning', '#007AFF');
              return 0;
          }
          $scope.choseLayerModel.layerId = $scope.choseLayerModel.layerId.slice(0, $scope.choseLayerModel.layerId.length - 1);

          return 1;
      }

      //画点之后执行的方法
      $scope.measuremapAfter = function (val, measure) {
          //console.log(val, measure);
          $scope.dataNum = 0;
          $scope.layerTreeData = [];
          $scope.layerTableData = {};
          clearLayer();

          if (!!val && !!$scope.choseLayerModel.layerId) {
              $timeout(function () {
                  waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                  $scope.isShowChose = false;
                  if (measure == "point") {
                      $scope.getExtent = val.slice(0, 2);
                      var tmpLoca = [coorToAngle($scope.getExtent[0]), coorToAngle($scope.getExtent[1])];

                      $scope.location = tmpLoca.join(',');
                      //console.log(layerId, $scope.getExtent[0], $scope.getExtent[1]);
                      var resolution = $scope.mapObj.getView().getResolution();
                      var tolerance = resolution * 10;//10像素容差查询
                      //console.log(resolution);
                      map.getLayerAttrByLayerPt($scope.choseLayerModel.layerId, $scope.getExtent[0], $scope.getExtent[1], tolerance).success(function (data, status) {
                          //console.log(data);
                          getTableData(data);
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                      });
                  }
                  else if (measure == "square") {
                      $scope.getExtent = angular.copy(val);
                      $scope.location = [coorToAngle($scope.getExtent[0]), coorToAngle($scope.getExtent[1])].join(',') + ' ' + [coorToAngle($scope.getExtent[2]), coorToAngle($scope.getExtent[3])].join(',');
                      var minLon = $scope.getExtent[0] < $scope.getExtent[2] ? $scope.getExtent[0] : $scope.getExtent[2];
                      var maxLon = $scope.getExtent[0] > $scope.getExtent[2] ? $scope.getExtent[0] : $scope.getExtent[2];
                      var minLat = $scope.getExtent[1] < $scope.getExtent[3] ? $scope.getExtent[1] : $scope.getExtent[3];
                      var maxLat = $scope.getExtent[1] > $scope.getExtent[3] ? $scope.getExtent[1] : $scope.getExtent[3];
                      map.getLayerAttrByRect($scope.choseLayerModel.layerId, minLon, minLat, maxLon, maxLat, tolerance).success(function (data, status) {
                          //console.log(data);
                          getTableData(data);
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                      });
                  }
                  else if (measure == "circle") {
                      $scope.getExtent = angular.copy([val[0], val[1]]);
                      var distance = getDistanceByLat(val[1], val[0], val[1], val[0] + val[2]);
                      $scope.location = [coorToAngle(val[0]), coorToAngle(val[1])].join(',') + ' ' + val[2];
                      //return;
                      map.getLayerAttrByPtTolerane($scope.choseLayerModel.layerId, $scope.getExtent[0], $scope.getExtent[1], distance).success(function (data, status) {
                          //console.log(data);
                          getTableData(data);
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                      });
                  }
              }, 200);
          }
      }

      //将地图查询返回的数据转换成页面上的表格
      function getTableData(data) {
          if (!!data) {
              data = JSON.parse(data);
              //console.log(data);
              for (var i = 0; i < data.length; i++) {
                  var tmp = {};
                  tmp.label = data[i].layerName;
                  tmp.layerID = data[i].layerID;
                  tmp.children = [];
                  for (var j = 0; j < data[i].data.length; j++) {
                      var tmpChild = {
                          label: data[i].data[j].sid,
                          geom: data[i].data[j].geom,
                          data: {}
                      };
                      for (var k in data[i].data[j]) {
                          if (k !== 'geom' && k !== 'sid') {
                              tmpChild.data[k] = data[i].data[j][k];
                          }
                      }
                      tmp.children = tmp.children.concat(tmpChild);
                  }
                  $scope.dataNum = $scope.dataNum + data[i].data.length;
                  $scope.layerTreeData = $scope.layerTreeData.concat(tmp);
              }
              $timeout(function () {
                  $scope.returnCtrl();
                  if (!!$scope.treectrlObj) {
                      $scope.treectrlObj.expand_all();
                      $scope.treectrlObj.select_branch($scope.layerTreeData[0].children[0]);
                      //$scope.onLayerSelected($scope.layerTreeData[0].children[0]);
                  }
              }, 100);
          }
          //console.log($scope.layerTreeData);

          $scope.isShowRes = true;
          waitmask.onHideMask();
      }

      //获取树的treeCtrl
      $scope.returnCtrl = function (data) {
          //console.log(data);
          if (!!data) {
              $scope.treectrlObj = angular.copy(data);
          }
      }

      //清理地图之后执行的方法
      $scope.clearAfter = function () {
          $scope.getExtent = [];
          $scope.isShowLayerCh = false;
          clearLayer();
          clearData();
      }

      //坐标转换成度分秒的形式
      function coorToAngle(num) {
          if (typeof (num) !== "number") {
              return NaN;
          }
          var d = Math.abs(num);
          var m = (60 * (d - Math.floor(d)));
          var s = (60 * (m - Math.floor(m)));
          if (s.toString(10).length > 4) {
              s = s.toFixed(0);
          }
          return parseInt(d) + '°' + parseInt(m) + '\'' + s + '\"';
      }

      //根据坐标经纬度计算两点之间的距离
      function getDistanceByLat(lat1, lon1, lat2, lon2) {
          //console.log(lat1, lon1, lat2, lon2);
          var earth_radius = 6378.137;//地球半径(km)
          var radLat1 = rad(lat1);
          var radLat2 = rad(lat2);
          var a = radLat1 - radLat2;
          var b = rad(lon1) - rad(lon2);
          var s = 2 * Math.asin(Math.sqrt(Math.pow(Math.sin(a / 2), 2) + Math.cos(radLat1) * Math.cos(radLat2) * Math.pow(Math.sin(b / 2), 2)));
          s = Math.round(s * earth_radius * 10000) / 10;
          //console.log(s);

          return s;
      }
      //上面方法的配套计算
      function rad(d) {
          return d * Math.PI / 180.0;
      }

      /*--------------地图查询操作--------------end----*/

      
      /*--------------地图查询结果弹窗--------------start----*/
      //图层查询树
      $scope.layerTreeData = [];
      //当前节点,点击树时自动赋值
      $scope.selectedLayer = "";
      //图层对应的属性值
      $scope.layerTableData = {};

      var vectorSource = new ol.source.Vector({});
      var vectorLayer = new ol.layer.Vector({
          source: vectorSource
      });
      var vectorLayers = []

      //选中树节点回调函数
      $scope.onLayerSelected = function (node, par, evt) {
          //console.log(node, par, evt);
          //console.log(vectorLayer);

          clearLayer();
          $timeout(function () {
              if (!node.isLeaf) {
                  vectorLayers = [];
                  for (var i = 0; i < node.children.length; i++) {
                      if (!!node.children[i].geom) {
                          vectorLayers[i] = getTaggingCoor(node.children[i].geom);
                      }
                  }
                  //console.log(vectorLayers);
              }
              if (node.isLeaf) {
                  $scope.layerTableData = angular.copy(node.data);
                  vectorLayer = new ol.layer.Vector({
                      source: vectorSource
                  });
                  if (!!node.geom) {
                      vectorLayer = getTaggingCoor(node.geom);
                  }
                  //console.log(vectorLayer);
              }
          }, 200);
      };

      //关闭查询结果弹窗
      $scope.closeRes = function () {
          $scope.getExtent = [];
          clearLayer();
          clearData();
      }

      //获取高亮的拐点坐标
      function getTaggingCoor(geom) {
          var vLayer;
          var coor = [];
          var tmp0 = geom.split('(');
          var tmp1 = tmp0[tmp0.length - 1].split(')');
          var tmp2 = tmp1[0].split(',');
          if (tmp0[0].indexOf('POINT') > -1) {
              var tmp3 = tmp2[0].split(' ');
              vLayer = $scope.addTagging("Point", tmp3, '#00fffa', '#000');
              $scope.centerPoint = angular.copy(tmp3);
          }
          if (tmp0[0].indexOf('LINESTRING') > -1) {
              for (var i = 0; i < tmp2.length; i++) {
                  var tmp3 = tmp2[i].split(' ');
                  coor.push(tmp3);
              }
              var tmp4 = [];
              for (var j = 0; j < coor.length - 1; j++) {
                  tmp4.push([coor[j], coor[j + 1]]);
              }
              vLayer = $scope.addTagging("MultiLineString", tmp4, '#a7a', '#EA158B');
              $scope.centerPoint = angular.copy(coor[0]);
          }
          if (tmp0[0].indexOf('POLYGON') > -1) {
              vLayer = $scope.addTagging("MultiPolygon", geom, '#00fffa', '#000');
          }
          $scope.tolocation($scope.centerPoint);
          return vLayer;
      }

      /*--------------地图查询结果弹窗--------------end----*/


      //根据地图ID获取分页图层列表
      $scope.getLayerFromMap = function (lyerType, layerTag, layerName, mapId, PageSize, PageIndex) {
          layerContent.getAllListStatus({ LayerType: lyerType, LayerTag: layerTag, LayerName: layerName, LayerDesc: mapId, CreateBy: localStorage.getItem('infoearth_spacedata_userCode')}, PageSize, PageIndex).success(function (data, statues) {
              //console.log(data);
              var tmpArr = angular.copy(data.items);

              for (var i = 0; i < tmpArr.length; i++) {
                  for (var j = 0; j < $scope.layerManageModal.layerTo.length; j++) {
                      if (tmpArr[i].id === $scope.layerManageModal.layerTo[j].id) {
                          tmpArr[i].ischecked = true;
                          continue;
                      }
                  }
              }
              //console.log(tmpArr);
              $scope.layerManageModal.layerFrom = angular.copy(tmpArr);
              //设置分页
              $scope.layerManageModal.pageing.pageCounts = data.totalCount;
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //分类列表--地图
      dataType.getAllListByDataType(overAllId).success(function (data, statues) {
          //console.log(data);
          var arr = [];
          for (var i in data.items) {
              var tempTypeData = {};
              tempTypeData.id = data.items[i].id;
              tempTypeData.dictCodeID = data.items[i].dictCodeID;
              tempTypeData.label = data.items[i].typeName;
              tempTypeData.typeDesc = data.items[i].typeDesc;
              tempTypeData.parentID = data.items[i].parentID;
              tempTypeData.children = [];
              arr.push(tempTypeData);
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
      //分类列表--图层
      dataType.getAllListByDataType(layerAllId).success(function (data, statues) {
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
                  $scope.layerTypeTreeData[0].children.push(each);
              }
          });
          $scope.layerTypeTreeData[0].children.forEach(function (each) {
              each.children = [];
              arr.forEach(function (item) {
                  if (item.parentID == each.id) {
                      each.children.push(item);
                  }
              });
          });
      });

      //标签列表--图层
      dataTag.getAllListByDataType(layerAllId).success(function (data, statues) {
          //console.log(data);
          for (var i in data.items) {
              var tempTagData = {};
              tempTagData.id = data.items[i].id;
              tempTagData.dictCodeID = data.items[i].dictCodeID;
              tempTagData.label = data.items[i].tagName;
              tempTagData.tagDesc = data.items[i].tagDesc;
              tempTagData.children = [];

              $scope.layerTagTreeData.push(tempTagData);
          }
      });

      //获取样式分类树
      $scope.getStyleTypeTree = function() {
          $scope.styleModal.styleTypeTreeData[0].children = [];
          //分类查询树的数据
          dataType.getAllListByDataType(styleTypeID).success(function (data, statues) {
              //console.log(data);

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
                      $scope.styleModal.styleTypeTreeData[0].children.push(each);
                  }
              });
              $scope.styleModal.styleTypeTreeData[0].children.forEach(function (each) {
                  each.children = [];
                  arr.forEach(function (item) {
                      if (item.parentID == each.id) {
                          each.children.push(item);
                      }
                  })
              });
              $rootScope.typeTreeData = angular.copy($scope.styleModal.styleTypeTreeData);
          });
      }
      $rootScope.getTypeTree = angular.copy($scope.getStyleTypeTree);//给全局变量附值

      //根据地图ID查询地图信息
      function getMap(id, func1, func2) {
          map.getDetailById(id).success(function (data, statues) {
              //console.log(data);
              if (countObj(data) > 0) {
                  $scope.serviceList = angular.copy(data);
                  $scope.mapSerShow(true, $scope.mapServiceNum, $scope.serviceList);

                  $scope.mapManageModal.id = data.id;
                  $scope.mapManageModal.mapName = data.mapName;
                  $scope.mapManageModal.mapEnName = data.mapEnName;
                  $scope.mapManageModal.mapDesc = data.mapDesc;
                  $scope.mapManageModal.currentType = data.mapType; //分类
                  $scope.mapManageModal.mapTag = data.mapTag;
                  $scope.mapManageModal.tagsInput = '';
                  var str = angular.copy(data.mapTag);
                  if (!!str) {
                      $scope.mapManageModal.tagsList = str.split(',');
                  }
                  else {
                      $scope.mapManageModal.tagsList = [];
                  }
                  $scope.mapManageModal.createUserId = data.createUserId;
                  $scope.mapManageModal.minX = data.minX;
                  $scope.mapManageModal.minY = data.minY;
                  $scope.mapManageModal.maxX = data.maxX;
                  $scope.mapManageModal.maxY = data.maxY;
                  $scope.mapManageModal.xy = [data.minXName, data.minYName, data.maxXName, data.maxYName];
                  if (data.mapLegend && data.mapLegend != null && data.mapLegend != 'delete') {
                      $scope.mapManageModal.mapLegendName = data.mapLegend.split('/').pop();
                  } else {
                      $scope.mapManageModal.mapLegendName = null;
                  }
                  //获取地图对应的图层
                  getListByMapId(func2);

                  if (typeof (func1) === "function") {
                      func1();
                  }
              }
          }).error(function (data, status) {
              //console.log(val, status);
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //根据地图ID查询已关联的图层列表
      function getListByMapId(func) {
          $scope.mapManageModal.mapManage_layers = [];
          mapReleation.getAllListByMapId($scope.mapManageModal.id).success(function (data, status) {
              //console.log(data);
              $scope.mapManageModal.mapManage_layers = data.items;
              if (typeof (func) === "function") {
                  func();
              }
          }).error(function (data, status) {
              //console.log(data);
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //根据点线面来给选中的图层进行排序
      //arr   图层对象数组
      //return    经过排序的图层对象数组
      function goSelectedLayer(arr) {
          var arrPoint = [[], [], [], []];
          for (var i = 0; i < arr.length; i++) {
              //点
              if (arr[i].dataType === "6b6941f1-67a3-11e7-8eb2-005056bb1c7e") {
                  arrPoint[0].push({ MapID: $scope.mapManageModal.id, DataConfigID: arr[i].id, DataSort: 0 });
              }
              //线
              if (arr[i].dataType === "7776934c-67a3-11e7-8eb2-005056bb1c7e") {
                  arrPoint[1].push({ MapID: $scope.mapManageModal.id, DataConfigID: arr[i].id, DataSort: 0 });
              }
              //面
              if (arr[i].dataType === "a2758dc0-67a3-11e7-8eb2-005056bb1c7e") {
                  arrPoint[2].push({ MapID: $scope.mapManageModal.id, DataConfigID: arr[i].id, DataSort: 0 });
              }//影像
              if (arr[i].dataType === "acf11b57-0626-4e49-b385-2e9a4195221c") {
                  arrPoint[3].push({ MapID: $scope.mapManageModal.id, DataConfigID: arr[i].id, DataSort: 0 });
              }
          }

          arrPoint = [].concat(arrPoint[0], arrPoint[1], arrPoint[2], arrPoint[3]);
          for (i = 0; i < arrPoint.length; i++) {
              arrPoint[i].DataSort = i + 1;
          }
          return arrPoint;
      }

      //清理地图查询数据
      function clearData() {
          $scope.isShowRes = false;
          $scope.location = [];
          $scope.layerTreeData = [];
          $scope.selectedLayer = "";
          $scope.layerTableData = {};
      }

      //清理高亮区域
      function clearLayer() {
          if (vectorLayers.length > 0) {
              for (var i in vectorLayers) {
                  $scope.removeLayer(vectorLayers[i]);
              }
          }
          if (vectorLayer) {
              $scope.removeLayer(vectorLayer);
          }
      }

      //地图高度自适应
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
      }
      $timeout(setHeight, 300);
      angular.element(window)[0].onresize = function () {
          setHeight();
      }

      //提示框
      function alertFun(title, text, type, color) {
          SweetAlert.swal({
              title: title,
              text: text,
              type: type,
              confirmButtonColor: color
          });
      }
      
      $rootScope.alertFun = function (title, text, type, color) {
          SweetAlert.swal({
              title: title,
              text: text,
              type: type,
              confirmButtonColor: color
          });
      };
      $rootScope.alertConfirm = function (title, text, type, backFun) {
          SweetAlert.swal({
              title: title,
              text: text,
              type: type,
              showCancelButton: true,
              confirmButtonColor: "#DD6B55",
              confirmButtonText: $filter('translate')('setting.sure'),
              cancelButtonText: $filter('translate')('setting.cancel')
          }, function (isConfirm) {
              if (isConfirm) {
                  backFun();
              }
          });
      };

      //计算对象的长度
      function countObj(obj) {
          var count = 0;
          for (var i in obj) {
              count++;
          }
          return count;
      }
      //生成随机数
      function getRandom() {
          var s = [];
          var str = "0123456789abcdefghijklmnopqrstuvwxyz";
          for (var i = 0; i < 36; i++) {
              s[i] = str.substr(Math.floor(Math.random() * 0x10), 1);
          }
          s[14] = "4";
          s[19] = str.substr((s[19] & 0x3) | 0x8, 1);
          s[8] = s[13] = s[18] = s[23] = "-";
          var guid = s.join("");
          return guid;
      }
      //判断浏览器类型
      function myBrowser() {
          var userAgent = navigator.userAgent;

          var isOpera = userAgent.indexOf("Opera") > -1;
          if (isOpera) {
              return "Opera";
          }
          if (userAgent.indexOf("firefox") > -1) {
              return "firefox";
          }
          if (userAgent.indexOf("Chrome") > -1) {
              return "Chrome";
          }
          if (userAgent.indexOf("Safari") > -1) {
              return "Safari";
          }
          if (userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera) {
              return "IE";
          }
      }
  }]);