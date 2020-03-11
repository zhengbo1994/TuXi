'use strict';
/**
 * mapManagerCtrl Controller
 */
app.controller('mapManagerCtrl',
    ['$rootScope', '$scope', '$document', '$sce', 'SweetAlert', '$element', '$filter', 'selfadapt', "$timeout", 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.map',
        'abp.services.app.dicDataCode', 'abp.services.app.tagReleation', 'abp.services.app.layerContent', 'abp.services.app.mapReleation', 'abp.services.app.dataStyle',
        'abp.services.app.attachment', 'abp.services.app.mapMetaData', 'waitmask',
  function ($rootScope, $scope, $document, $sce, SweetAlert, $element, $filter, selfadapt, $timeout, dataTag, dataType, map, dicDataCode, tagReleation, layerContent, mapReleation, dataStyle, attachment, mapMetaData, waitmask) {
      $rootScope.loginOut();
      $rootScope.homepageStyle = {};
      $rootScope.app.layout.isNavbarFixed = true;
      $rootScope.app.isWholeScreen = false;
      //调用实时随窗口高度的变化而改变页面内容高度的服务
      var unlink = selfadapt.anyChange($element);
      $scope.$on('$destroy', function () {
          unlink();
          selfadapt.showBodyScroll();
      });
      //初始化
      var overAllId = 'ba5ab799-6acd-11e7-87f3-005056bb1c7e';
      var layerAllId = 'a2faae61-6acd-11e7-87f3-005056bb1c7e';
      var spaceId = '2cf1d4ba-67ab-11e7-8eb2-005056bb1c7e';
      var scaleId = '46f30c2a-6cf2-11e7-9d4f-005056bb1c7e';
      var styleTypeID = 'c755eeea-986d-11e7-90b1-005056bb1c7e';
      $scope.inputText = '';//搜索input
      $scope.typeNodeId = '';//树选中类型
      $scope.tagNodeId = '';//树选中标签
      //地图分页初始信息
      $scope.maxSize = 5;//页码个数显示数
      $scope.goPage = 1;//转到多少页
      $scope.pageCounts = 0;//32;//总条数
      $scope.pageIndex = 1;//1;//起始页
      $scope.pageSize = 10;//10;//每页显示条数

      //以过滤图层分页初始信息
      $scope.maxSizeC = 5;//页码个数显示数
      $scope.goPageC = 1;//转到多少页
      $scope.pageCountsC = 0;//32;//总条数
      $scope.pageIndexC = 1;//1;//起始页
      $scope.pageSizeC = 10;//10;//每页显示条数

      ////////////////////////////////////////////////////////////////////////////////////////'首页'////////////////////////////////////////////////////////////////////////////////

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

      //标签列表--地图
      dataTag.getAllListByDataType(overAllId).success(function (data, statues) {

          //console.log(data);

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
      //模糊搜索
      $scope.searchMap = function () {
          mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
      }
      //地图删除
      $scope.deleteMap = function (id) {
          //console.log(id);
          SweetAlert.swal({
              title: $filter('translate')('views.Map.alertFun.confirmDelMap'),
              text: $filter('translate')('views.Map.alertFun.confirmDelMapPrompt'),
              type: "warning",
              showCancelButton: true,
              confirmButtonColor: "#DD6B55",
              confirmButtonText: $filter('translate')('setting.sure'),
              cancelButtonText: $filter('translate')('setting.cancel')
          }, function (isConfirm) {
              if (isConfirm) {
                  waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                  map.delete(id).success(function (data, statues) {
                      //console.log(data, statues);
                      waitmask.onHideMask();
                      if (data) {
                          alertFun($filter('translate')('views.Map.alertFun.mapDelSucc'), '', 'success', '#007AFF');
                          mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
                      } else {
                          alertFun($filter('translate')('views.Map.alertFun.mapDelErr'), '', 'error', '#007AFF');
                      }
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              }
          });
      }
      //地图新增
      $scope.addMap = function(){
          //比例尺列表
          //dicDataCode.getDetailByTypeID(scaleId).success(function (data, statues) {
          //    //console.log(data, statues);
          //    $scope.inputModal.scaleArr = data.items;
          //});
          $scope.inputModal.title = $filter('translate')('views.Map.create.MAIN');
          $scope.inputModal.mapDes = "";
          $scope.inputModal.mapName = "";
          $scope.inputModal.mapTag = "";
          $scope.inputModal.tagsInput = "";
          $scope.inputModal.tagsList = [];
          $scope.inputModal.spaceSel = {};
          $scope.inputModal.scaleSel = {};
          $scope.inputModal.spaceArr = [];
          $scope.inputModal.scaleArr = [];
          $scope.inputModal.tableLayers = [];
          $scope.inputModal.typePullDownTreeSelData = {};
          $scope.inputModal.currentType = '';
          
          $scope.inputModal.CreateUserId = localStorage.getItem('infoearth_spacedata_userCode');
          $scope.inputModal.CreateUserName = localStorage.getItem('infoearth_spacedata_userCode');
          //分类
          $scope.inputModal.typePullDownTreeData = JSON.parse(JSON.stringify($scope.typeTreeData[0].children));
          $scope.openAddMapFun();
      }
      //地图刷新函数
      function refreshMap(id) {
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
          mapReleation.refreshMap(id).success(function (data, statues) {
              waitmask.onHideMask();
              if (data) {
                  alertFun($filter('translate')('views.Map.alertFun.refreshOver'), '', 'success', '#007AFF');
              }
              else {
                  alertFun($filter('translate')('views.Map.alertFun.refreshErr'), '', 'error', '#007AFF');
              }
          }).error(function (data, status) {
              waitmask.onHideMask();
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }
      //地图刷新
      $scope.refreshMap = function (id) {
          SweetAlert.swal({
              title: $filter('translate')('views.Map.List.refresh'),
              text: $filter('translate')('views.Map.alertFun.confirmRef'),
              type: "info",
              showCancelButton: true,
              confirmButtonColor: "#DD6B55",
              confirmButtonText: $filter('translate')('setting.sure'),
              cancelButtonText: $filter('translate')('setting.cancel')
          }, function (isConfirm) {
              if (isConfirm) {
                  mapReleation.isExistTilesTask(id).success(function (data) {
                      if (data) {
                          SweetAlert.swal({
                              title: $filter('translate')('views.Map.List.refresh'),
                              text: $filter('translate')('views.Map.alertFun.confirmRef2'),
                              type: "info",
                              showCancelButton: true,
                              confirmButtonColor: "#DD6B55",
                              confirmButtonText: $filter('translate')('setting.sure'),
                              cancelButtonText: $filter('translate')('setting.cancel')
                          }, function (isConfirm) {
                              if (isConfirm) {
                                  refreshMap(id);
                              } else {
                                  return;
                              }
                          });
                      } else {
                          refreshMap(id);
                      }
                  }).error(function (data) {
                      alertFun($filter('translate')('setting.error'), 'error', '#007AFF');
                  });
              } else {
                  return;
              }
          });
      }
      //分页的事件方法
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
          //console.log({ MapName: $scope.inputText, MapType: $scope.typeNodeId, MapTag: $scope.tagNodeId });
          mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
      };

      /*--------------页面左边树形--------------start----*/
      // 切换 刷新数据
      $scope.selectTab = function (state) {
          $scope.showtab1 = state;
          $scope.treeQueryCtrl1.select_branch($scope.typeTreeData[0]);
          $scope.onTypeSelected($scope.typeTreeData[0]);
      }

      //分类查询树
      $scope.typeTreeData = [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }];
      $scope.layerTagTreeData = [];
      $scope.layerTypeTreeData = [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }];

      //当前节点,点击树时自动赋值
      $scope.selectedType = "";

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
      $scope.selectedTag = "";

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
      $scope.treeFlag = false;
      $scope.close = function () {
          if ($scope.treeFlag) {
              $scope.treeQueryCtrl1.expand_all();
              $scope.treeFlag = false;
          }
          else {
              $scope.treeQueryCtrl1.collapse_all();
              $scope.treeFlag = true;
          }
      }

      /*--------------页面左边树形--------------end----*/

      function mapInit(mapName, mapType, mapTag, pageSize, pageIndex) {

          $scope.mapData = [];
          map.getPageListByName({ MapName: mapName, MapType: mapType, MapTag: mapTag, CreateUserId: localStorage.getItem('infoearth_spacedata_userCode') }, pageSize, pageIndex).success(function (data, status) {
              //console.log(data);

              data.items.forEach(function (each) {
                  each.mapImgSrc = '/Thumbnail/map/' + each.mapEnName + '.png';
                  //each.mapImgSrc = GeoServerUrl + '/wms?service=WMS&version=1.1.0&request=GetMap&layers=' + WorkSpace + ':' + each.mapEnName + '&styles=&bbox=' + each.minX + ',' + each.minY + ',' + each.maxX + ',' + each.maxY + '&width=768&height=670&srs=EPSG:4326&format=image%2Fvnd.jpeg-png&_=' + $scope.randomCount;

                  if (each.mapTag.length > 20) {
                      each.mapTag = each.mapTag.substr(0, 20) + '...';
                  }
                  if (each.createDT != null) {
                      each.createDT = each.createDT.replace('T', ' ');
                  }
              });
              $scope.mapData = data.items;
              $scope.pageCounts = data.totalCount;
          });
      }



      ////////////////////////////////////////////////////////'标签管理'////////////////////////////////////////////////////////////////////////////////
      $scope.inputModalTag = {
          title: "",
          tagsInput: '',
          tagsList: [],
          delItems: function (a, b) {
              for (var i = 0; i < this.tagsList.length; i++) {
                  if ($scope.inputModalTag.tagsList[i] === a) {
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
          }
      }
      var inputModalTag = angular.copy($scope.inputModalTag);
      $scope.cancelTagManage = function () { //关闭或取消操作
          $scope.inputModalTag = angular.copy(inputModalTag);
      };

      //'标签管理'打开
      $scope.openTagManage = function () {
          $timeout(function () {//延时INPUT获取焦点
              $document.find('.mytagsfocus').focus();
          }, 450);
      };

      $scope.tagManageId = function (tag, id) {
          $scope.mapid = id;
          $scope.inputModalTag.title = $filter('translate')('setting.tags.manage');
          //2017-8-3
          var str = angular.copy(tag);
          if (!!str) {
              $scope.inputModalTag.tagsList = str.split(',');
          }

          $scope.openTagManagerFun();
      }
      //'标签管理' 标签input监听
      $scope.$watch('inputModalTag.tagsInput', function (val, old) {
          if (myBrowser() === 'Chrome') {
              return;
          }
          $document.find('.mytagsfocus').blur();
          $timeout(function () {
              $document.find('.mytagsfocus').focus();
              if (val.length > 20) {
                  $scope.inputModalTag.tagsInput = $scope.inputModalTag.tagsInput.substr(0, 20);
              }
              if ($scope.inputModalTag.tagsList.length === 5) {
                  $scope.inputModalTag.tagsInput = '';
                  return;
              }
              if (val.lastIndexOf(' ') !== -1) {
                  $timeout(function () {
                      $scope.inputModalTag.tagsInput = $.trim(val).replace(',', '');
                  });
              }
              if (val.lastIndexOf('，') === -1) return;
              $scope.inputModalTag.setdata(old);
              $scope.inputModalTag.tagsInput = old;
          });
      });

      //'标签管理'提交  
      $scope.tagManageForm = function (modalInstance) {

          if (!!$scope.inputModalTag.tagsInput) {
              alertFun($filter('translate')('views.Layer.alertFun.field.tagPrompt1'), $filter('translate')('views.Layer.alertFun.field.tagPrompt2'), "warning", "#007AFF");
              return;
          }
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);

          //更新地图与标签关系
          var joinstr = $scope.inputModalTag.tagsList.join(',');
          var input = {
              tagName: joinstr,
              dataID: overAllId,
              mapLayerID: $scope.mapid
          }
          insertTagReMap(input, function (data) {
              waitmask.onHideMask();
              alertFun($filter('translate')('views.Layer.alertFun.set.setTagSucc'), "", "success", "#007AFF");
              mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
              modalInstance.close();
          });
      }


      //////////////////////////////////////////////////////////////////////////////////////'分类管理'////////////////////////////////////////////////////////////////////////////////
      //'分类管理'打开
      $scope.inputModalType = {
          title: "",
          typePullDownTreeData: '',
          openCreatStylea: function () {
              $scope.creatTypeModal.title = $filter('translate')('setting.class.newClassTitle');
              $scope.opencreatstyle();
          },
          typePullDownTreeSelData: '',
          onComboChecked: function (a, b) {
              //console.log($scope.inputModalType.typePullDownTreeSelData);
              //console.log($scope.typeTreeData);
          }
      };
      var inputModalType = angular.copy($scope.inputModalType);
      $scope.cancelTypeManage = function () { //关闭或取消操作
          $scope.inputModalType = angular.copy(inputModalType);
      };
      $scope.openTypeManage = function () {
          //console.log("'分类管理'弹窗打开");
          $scope.inputModalType.typePullDownTreeData = JSON.parse(JSON.stringify($scope.typeTreeData[0].children));
      }
      $scope.typeManageId = function (type, id) {
          $scope.inputModalType.currentType = type;
          $scope.inputModalType.title = $filter('translate')('views.Layer.List.classSet');
          $scope.currentType = type;
          $scope.mapid = id;
          $scope.openTypeManageFun();
      }

      //'分类管理'提交
      $scope.typeManageForm = function (modalInstance) {
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
          //console.log($scope.mapid, $scope.inputModalType.typePullDownTreeSelData.id);
          //分类与地图关系保存
          map.updateDataType($scope.mapid, $scope.inputModalType.typePullDownTreeSelData.id).success(function (data, status) {
              //console.log(data);
              waitmask.onHideMask();
              alertFun($filter('translate')('views.Layer.alertFun.set.setClassSucc'), '', 'success', '#007AFF');
              mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
              modalInstance.close();
          }).error(function (data, status) {
              waitmask.onHideMask();
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }
      //////////////////////////////////////////////////////////////////////////////////////'创建分类'////////////////////////////////////////////////////////////////////////////////
      
      $scope.initCreatType = function () {
          $scope.creatTypeModal.classTreedata = [];//初始化
          var result = [];
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
                      result.push(each);
                  }
              });
              result.forEach(function (each) {
                  each.children = [];
                  arr.forEach(function (item) {
                      if (item.parentID == each.id) {
                          each.children.push(item);
                      }
                  });
              });
              $scope.creatTypeModal.classTreedata = angular.copy(result);
              //重载分类管理的树及首页分类树
              $scope.inputModalType.typePullDownTreeData = angular.copy(result);
              $scope.inputModal.typePullDownTreeData = angular.copy(result);
              $scope.typeTreeData[0].children = angular.copy(result);
          });
      }
      //创建分类的弹窗model
      $scope.nodeType = '';
      $scope.creatTypeModal = {
          title: '',
          classTreeSel: {},
          classTreedata: [],
          //新增父节点
          classAddPa: function () {
              //console.log($scope.creatTypeModal.classTreeSel)
              $scope.openInputText();
              $scope.nodeType = 'parent';
              $scope.openInputTextModal.title = $filter('translate')('setting.class.newParent');
              getNodeInputFocus("openInputTextCls");
          },
          //新增子节点
          classAddSon: function () {
              $scope.nodeType = 'children';
              if (!$scope.creatTypeModal.classTreeSel.id) {
                  alertFun($filter('translate')('views.Layer.alertFun.set.addNodePrompt1'), "", "warning", "#007AFF");
              } else {
                  if ($scope.creatTypeModal.classTreeSel.parentID != "0") {
                      alertFun($filter('translate')('views.Layer.alertFun.set.addNodePrompt2'), "", "warning", "#007AFF");
                  }
                  else {
                      $scope.openInputTextModal.typeName = '';
                      $scope.openInputTextModal.title = $filter('translate')('setting.class.newChild');
                      $scope.openInputText();
                      getNodeInputFocus("openInputTextCls");
                  }
              }
          },
          //编辑
          classEdit: function () {
              $scope.nodeType = 'edit';
              if (!$scope.creatTypeModal.classTreeSel.id) {
                  alertFun($filter('translate')('views.Layer.alertFun.set.editNodePrompt1'), "", "warning", "#007AFF");
              }
              else {
                  $scope.openInputTextModal.typeName = $scope.creatTypeModal.classTreeSel.label;
                  $scope.openInputTextModal.title = $filter('translate')('setting.class.editNode');
                  $scope.openInputText();
                  getNodeInputFocus("openInputTextCls");
              }
          },
          //删除
          classDel: function () {
              $scope.nodeType = 'delete';
              if (!$scope.creatTypeModal.classTreeSel.id) {
                  alertFun($filter('translate')('views.Layer.alertFun.set.delNodePrompt1'), "", "warning", "#007AFF");
              } else if ($scope.creatTypeModal.classTreeSel.label == $scope.currentType) {
                  alertFun($filter('translate')('views.Layer.alertFun.set.delNodePrompt2'), "", "warning", "#007AFF");
              } else {
                  SweetAlert.swal({
                      title: $filter('translate')('setting.delete'),
                      text: $filter('translate')('views.Layer.alertFun.set.delNodePrompt3'),
                      type: "warning",
                      showCancelButton: true,
                      confirmButtonColor: "#DD6B55",
                      confirmButtonText: $filter('translate')('setting.sure'),
                      cancelButtonText: $filter('translate')('setting.cancel')
                  }, function (isConfirm) {
                      if (isConfirm) {
                          dataType.delete($scope.creatTypeModal.classTreeSel.id).success(function (data, status) {
                              //console.log(data);
                              $scope.creatTypeModal.classTreeSel = {};
                              $scope.initCreatType();
                          });
                      }
                  });
              }
          },
          creatTypeOpened: function () {
              //console.log('创建分类打开');
              $scope.creatTypeModal.classTreedata = angular.copy($scope.typeTreeData[0].children);
          },
          createTypeSubmit: function (modalInstance, form) {
              //刷新首页树
              $scope.typeTreeData.children = [];
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
                          $scope.typeTreeData.children.push(each)
                      }
                  });
                  $scope.typeTreeData.children.forEach(function (each) {
                      each.children = [];
                      arr.forEach(function (item) {
                          if (item.parentID == each.id) {
                              each.children.push(item);
                          }
                      });
                  });
              });
              //关闭弹出框
              modalInstance.close();
          },
          html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            '<div class="form-group">' +
            '    <div class="popwin-top" style="padding: 0 10px 10px 10px; text-align: center;">' +
            '        <div class="btn-group" style="margin-bottom: 15px;">' +
            '            <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classAddPa()" translate="setting.class.newParent">' +
            '                新增父节点' +
            '            </a>' +
            '            <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classAddSon()" translate="setting.class.newChild">' +
            '                新增子节点' +
            '            </a>' +
            '            <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classEdit()" translate="setting.edit">' +
            '                编辑' +
            '            </a>' +
            '            <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classDel()" translate="setting.delete">' +
            '                删除' +
            '            </a>' +
            '        </div>' +
            '    </div>' +
            '    <div class="popwin-body" style="padding: 0 10px; max-height: 300px; overflow-y: auto;">' +
            '        <div style="border-top: solid 1px rgba(0, 0, 0, 0.07);"></div>' +
            '        <abn-tree class="font-title-btn" icon-leaf="" tree-data="popwinmodal.classTreedata" initial-selection="{{popwinmodal.classTreedata[0].label}}" selected-data="popwinmodal.classTreeSel" on-select="popwinmodal.onClaSelected" expand-level="2"></abn-tree>' +
            '        <div style="border-top: solid 1px rgba(0, 0, 0, 0.07);"></div>' +
            '    </div>' +
            '</div>' +
            '<div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '    <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.sure">确定</button>' +
            '</div>' +
            '</form>'
      }


      //选中树节点执行方法
      $scope.creatTypeModal.onClaSelected = function (node, getParentNodeBackFun) {
          //console.log("当前节点", node);
          var pnode = getParentNodeBackFun(node);
          //console.log("当前节点的父节点", pnode);
      }

      //输入分类
      $scope.openInputTextModal = {
          title: '',
          typeName: ''
      }

      //输入分类提交
      $scope.submitNode = function (modalInstance) {

          //新增分类的方法
          //obj    分类对象
          function insertClass(obj) {
              dataType.insert(obj).success(function (data, status) {
                  //console.log(data);
                  if (data) {
                      $scope.initCreatType();
                  }
                  else {
                      alertFun($filter('translate')('views.Map.alertFun.addClassErr'), '', 'error', '#007AFF');
                  }
                  modalInstance.close();
              }).error(function (data, status) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });;
          }

          if ($scope.openInputTextModal.typeName != '') {
              dataType.getDetailByName($scope.openInputTextModal.typeName, overAllId).success(function (data, statues) {
                  //console.log(data);
                  if (data.items.length > 0) {
                      alertFun($filter('translate')('views.Layer.alertFun.set.addClassPrompt'), '', 'warning', '#007AFF');
                  } else {
                      if ($scope.nodeType == 'parent') {
                          var dataP = { TypeName: $scope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: overAllId, ParentID: "0" }
                          //console.log(data);
                          insertClass(dataP);
                      } else if ($scope.nodeType == 'children') {
                          var dataC = { TypeName: $scope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: overAllId, ParentID: $scope.creatTypeModal.classTreeSel.id }
                          insertClass(dataC);
                      } else if ($scope.nodeType == 'edit') {
                          var dataE = { Id: $scope.creatTypeModal.classTreeSel.id, TypeName: $scope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: overAllId, ParentID: $scope.creatTypeModal.classTreeSel.parentID }
                          dataType.update(dataE).success(function (data, status) {
                              //console.log(data);
                              modalInstance.close();
                              $scope.initCreatType();
                          });
                      } else if ($scope.nodeType == 'delete') {

                      }
                      $scope.openInputTextModal.typeName = [];
                      $scope.creatTypeModal.classTreeSel = {};
                  }
              });
          } else {
              alertFun($filter('translate')('views.Layer.alertFun.set.inputContent'), '', 'warning', '#007AFF');
          }
      }


      //////////////////////////////////////////////////////////////////////////////////////'新建地图'////////////////////////////////////////////////////////////////////////////////
      //监听标签输入框
      $scope.$watch('inputModal.tagsInput', function (val, old) {
          //console.log(myBrowser());
          if (myBrowser() === 'Chrome') {
              return;
          }
          $document.find('.mytagsfocus').blur();
          $timeout(function () {
              $document.find('.mytagsfocus').focus();
              if (val.length > 20) {
                  $scope.inputModal.tagsInput = $scope.inputModal.tagsInput.substr(0, 20);
              }
              if ($scope.inputModal.tagsList.length === 5) {
                  $scope.inputModal.tagsInput = '';
                  return;
              }
              if (val.lastIndexOf(' ') !== -1) {
                  $timeout(function () {
                      $scope.inputModal.tagsInput = $.trim(val).replace(',', '');
                  });
              }
              if (val.lastIndexOf('，') === -1) return;
              $scope.inputModal.setdata(old);
              $scope.inputModal.tagsInput = old;
          });
      });

      $scope.inputModal = {
          title:"",
          openCreatStylea: function () {
              $scope.creatTypeModal.title = $filter('translate')('setting.class.newClassTitle');
              $scope.opencreatstyle();
          },
          //新建地图
          mapDes: '',
          mapName: '',
          CreateUserId: '',
          CreateUserName: '',
          //标签
          mapTag: '',
          tagsInput: '',
          tagsList: [],
          delItems: function (a, b) {
              for (var i = 0; i < this.tagsList.length; i++) {
                  if ($scope.inputModal.tagsList[i] === a) {
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

          spaceSel: {},
          scaleSel: {},
          spaceArr: [],
          scaleArr: [],
          up: function (arr, index) {
              swapItems(arr, index, index - 1);
          },
          down: function (arr, index) {
              swapItems(arr, index, index + 1);
          },
          //图层列表
          tableLayers: [],
          //设置图层样式
          setStyle: function (item) {
              //console.log(item);
              //图层id
              $scope.styleModal.title = $filter('translate')('views.Style.setStyle');
              $scope.styleModal.layerid = angular.copy(item.id);
              $scope.styleModal.styleSelectData = {};
              if (!!item.dataStyleID) {
                  $scope.styleModal.styleSelectData.id = angular.copy(item.dataStyleID);
              }
              $scope.styleModal.styleDataType = item.dataType;
              getStyleTypeTree();
              $scope.opensStyleModal();
          },
          //删除图层样式
          delStyle: function (item) {
              //console.log(item);
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
                      //console.log(mapReleaData);
                      for (var i = 0; i < $scope.inputModal.tableLayers.length; i++) {
                          if (item.id === $scope.inputModal.tableLayers[i].id) {
                              $scope.inputModal.tableLayers[i].dataStyleID = '';
                              $scope.inputModal.tableLayers[i].dataStyleName = '';
                          }
                      }
                  }
              });
          },
          dropComplete: function (index, data) {
              //console.log(index,data);
              var layerArr = [];
              var tmpArr = angular.copy($scope.inputModal.tableLayers);
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

              $scope.inputModal.tableLayers = angular.copy(tmpArr);
          },
          openAddLayerModel: function () {
              $scope.inputModalB.layerTo = [];
              $scope.inputModalB.layerTo = angular.copy($scope.inputModal.tableLayers);

              $scope.inputModalB.typeTreeData = JSON.parse(JSON.stringify($scope.layerTypeTreeData));
              $scope.inputModalB.tagTreeData = JSON.parse(JSON.stringify($scope.layerTagTreeData));
              $scope.inputModalB.title = $filter('translate')('views.Map.create.associate');
              $scope.addLayerModel();
          },
          spatialRefence: SpatialRefence,
          typePullDownTreeData: '',
          typePullDownTreeSelData: '',
          currentType: '',
          onSelected: function (node) {
              //console.log(node);
          }
      };
      $scope.openedBack = function () { };
      $scope.cancel = function () { };
      //排序
      var swapItems = function (arr, index1, index2) {
          arr[index1] = arr.splice(index2, 1, arr[index1])[0];
          return arr;
      }

      //'新建地图'→提交
      $scope.submitForm = function (modalInstance, form) {
          $rootScope.loginOut();

          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
          if ($scope.inputModal.mapName == '') {
              waitmask.onHideMask();
              alertFun($filter('translate')('views.Map.alertFun.mapPrompt1'), '', 'warning', '#007AFF');
          } else if ($scope.inputModal.typePullDownTreeSelData == '') {
              waitmask.onHideMask();
              alertFun($filter('translate')('views.Layer.alertFun.field.layerPrompt3'), '', 'warning', '#007AFF');
          } else if (!!$scope.inputModal.tagsInput) {
              waitmask.onHideMask();
              alertFun($filter('translate')('views.Layer.alertFun.field.tagPrompt2'), '', 'warning', '#007AFF');
          } else if ($scope.inputModal.tableLayers.length == 0) {
              waitmask.onHideMask();
              alertFun($filter('translate')('views.Map.alertFun.mapPrompt2'), '', 'warning', '#007AFF');
          }
          else {
              var mapInfo = {
                  Id: '',
                  MapName: $scope.inputModal.mapName,
                  MapType: $scope.inputModal.typePullDownTreeSelData.id,
                  MapTag: $scope.inputModal.tagsList.join(','),
                  SpatialRefence: $scope.inputModal.spatialRefence,
                  MapScale: typeof ($scope.inputModal.scaleSel.selected) === "undefined" ? '' : $scope.inputModal.scaleSel.selected.id,
                  MapDesc: $scope.inputModal.mapDes,
                  MapBBox: '',
                  MapPublishAddress: '',
                  MapStatus: '',
                  PublishDT: '',
                  SortCode: '',
                  EnabledMark: '',
                  DeleteMark: '',
                  CreateUserId: $scope.inputModal.CreateUserId,
                  CreateUserName: $scope.inputModal.CreateUserName,
                  CreateDT: '',
                  ModifyUserId: '',
                  ModifyUserName: '',
                  ModifyDate: ''
              };
              //console.log($scope.inputModal.tableLayers);
              map.insert(mapInfo).success(function (data, statues) {
                  //console.log(data, statues)
                  if (statues == 200) {
                      //保存标签与地图对应关系
                      //console.log($scope.inputModal.tagsList.join(','));
                      //console.log(overAllId, data.id);
                      var args = {
                          tagName: $scope.inputModal.tagsList.join(','),
                          dataID: overAllId,
                          mapLayerID: data.id
                      }
                      insertTagReMap(args, function (data1) {
                          //保存图层与地图的关系
                          var layersInfo = [];
                          $scope.inputModal.tableLayers.forEach(function (each, index) {
                              layersInfo.push({ MapID: data.id, DataConfigID: each.id, DataStyleID: each.dataStyleID, DataSort: index + 1 });
                          });
                          mapReleation.multiInsert(layersInfo).success(function (data2, statues2) {
                              //console.log(data2, statues2)
                              if (data2) {
                                  //发布地图
                                  mapReleation.publicMap(data.id).success(function (data3, statues3) {
                                      //console.log(data3, statues3)
                                      if (data3) {
                                          waitmask.onHideMask();
                                          alertFun($filter('translate')('views.Map.alertFun.addMapSucc'), '', 'success', '#007AFF');
                                          $scope.randomCount = (new Date()).getTime() + getRandom();

                                          mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
                                          modalInstance.close();
                                      }
                                  }).error(function (data, status) {
                                      waitmask.onHideMask();
                                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                                  });
                              }
                          }).error(function (data, status) {
                              waitmask.onHideMask();
                              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                          });
                      });

                  }
              }).error(function (data, status) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });
          }
      };

      ////////////////////////////////////////////////////////'新加图层'////////////////////////////////////////////////////////////////////////////////
      //新加图层modal
      $scope.inputModalB = {
          title: "",
          typeTreeData: [],
          tagTreeData: [],
          showtab: true,
          selectTab: function (state) {
              //console.log(state)
              $scope.inputModalB.showtab = state;
              $scope.inputModalB.typeTreeQueryCtrl.select_branch($scope.inputModalB.typeTreeData[0]);
              $scope.inputModalB.onTypeSelected($scope.inputModalB.typeTreeData[0]);
          },
          typeSelected: {},
          onTypeSelected: function (node) {
              //console.log(node);
              $scope.inputModalB.pageing.pageIndex = 1;
              $scope.inputModalB.layerName = "";
              $scope.layerInit(node.id, '', '', '', $scope.inputModalB.pageing.pageSize, 1);
              $scope.inputModalB.tagTreeQueryCtrl.select_branch();
          },
          tagSelected: {},
          onTagSelected: function (node) {
              //console.log(node);
              $scope.inputModalB.pageing.pageIndex = 1;
              $scope.inputModalB.layerName = "";
              $scope.layerInit('', node.id, '', '', $scope.inputModalB.pageing.pageSize, 1);
          },
          layerFrom: [],
          layerTo: [],
          checkedDataF: [],
          //选中一行
          onTdChecked: function (row) {
              var isSame = 0;
              for (var i = 0; i < $scope.inputModalB.layerTo.length; i++) {
                  if (row.id === $scope.inputModalB.layerTo[i].id) {
                      if (row.ischecked) {
                          isSame++;
                          break;
                      }
                      else {
                          $scope.inputModalB.layerTo.splice(i, 1);
                          break;
                      }
                  }
              }
              if (isSame === 0 && row.ischecked) {
                  $scope.inputModalB.layerTo = $scope.inputModalB.layerTo.concat(row);
              }
          },
          //选中全部
          onThChecked: function (allchecked) {
              for (var i = 0; i < $scope.inputModalB.layerFrom.length; i++) {
                  var isSame = 0;
                  for (var j = 0; j < $scope.inputModalB.layerTo.length; j++) {
                      if ($scope.inputModalB.layerTo[j].id === $scope.inputModalB.layerFrom[i].id) {
                          if (allchecked) {
                              isSame++;
                              break;
                          }
                          else {
                              $scope.inputModalB.layerTo.splice(j, 1);
                              break;
                          }
                      }
                  }
                  if (isSame === 0 && allchecked) {
                      $scope.inputModalB.layerTo = $scope.inputModalB.layerTo.concat($scope.inputModalB.layerFrom[i]);
                  }
              }
          },
          //已选中列表的操作栏
          tabBtnParams: [{
              name: "删除", click: function (row, name, event) {
                  for (var i = 0; i < $scope.inputModalB.layerTo.length; i++) {
                      if ($scope.inputModalB.layerTo[i].id === row.id) {
                          $scope.inputModalB.layerTo.splice(i, 1);
                          break;
                      }
                  }
                  for (var j = 0; j < $scope.inputModalB.layerFrom.length; j++) {
                      if ($scope.inputModalB.layerFrom[j].id === row.id) {
                          $scope.inputModalB.layerFrom[j].ischecked = false;
                          break;
                      }
                  }
              }
          }],

          selectedLayer: [],

          //分页点击事件回调函数
          pageChanged: function () {
              var typeId = typeof ($scope.inputModalB.typeSelected.id) === "undefined" ? '' : $scope.inputModalB.typeSelected.id;
              var tagId = typeof ($scope.inputModalB.tagSelected.id) === "undefined" ? '' : $scope.inputModalB.tagSelected.id;
              $scope.layerInit(typeId, tagId, $scope.inputModalB.layerName, "", $scope.inputModalB.pageing.pageSize, $scope.inputModalB.pageing.pageIndex);
          },
          pageing: { pageIndex: $scope.pageIndex, pageSize: $scope.pageSize, pageCounts: $scope.pageCounts, maxSize: 2 },
          layerName: '',
          //搜索图层
          searchLayerByName: function () {
              $scope.inputModalB.pageing.pageIndex = 1;
              var typeId = typeof ($scope.inputModalB.typeSelected.id) === "undefined" ? '' : $scope.inputModalB.typeSelected.id;
              var tagId = typeof ($scope.inputModalB.tagSelected.id) === "undefined" ? '' : $scope.inputModalB.tagSelected.id;
              $scope.layerInit(typeId, tagId, $scope.inputModalB.layerName, "", $scope.inputModalB.pageing.pageSize, 1);
          },
          open: function () { },
          submit: function (modalInstance, form) {
              $scope.inputModal.tableLayers = [];
              var arr = $scope.inputModalB.layerTo;
              var arrPoint = [[], [], [], []];

              for (var i = 0; i < arr.length; i++) {
                  //点
                  if (arr[i].dataType === "6b6941f1-67a3-11e7-8eb2-005056bb1c7e") {
                      arrPoint[0].push(arr[i]);
                  }
                  //线
                  if (arr[i].dataType === "7776934c-67a3-11e7-8eb2-005056bb1c7e") {
                      arrPoint[1].push(arr[i]);
                  }
                  //面
                  if (arr[i].dataType === "a2758dc0-67a3-11e7-8eb2-005056bb1c7e") {
                      arrPoint[2].push(arr[i]);
                  }
                  //影像图层
                  if (arr[i].dataType === "acf11b57-0626-4e49-b385-2e9a4195221c") {
                      arrPoint[3].push(arr[i]);
                  }
              }
              $scope.inputModal.tableLayers = [].concat(arrPoint[0], arrPoint[1], arrPoint[2], arrPoint[3]);
              for (var j = 0; j < $scope.inputModal.tableLayers.length; j++) {
                  $scope.inputModal.tableLayers[j].dataStyleID = $scope.inputModal.tableLayers[j].layerDefaultStyle;
                  $scope.inputModal.tableLayers[j].dataStyleName = $scope.inputModal.tableLayers[j].layerDefaultStyleName;
              }
              //console.log($scope.inputModal.tableLayers);
              //关闭弹出框
              modalInstance.close();
          },
          cancel: function () { }
      }

      $scope.layerInit = function (layerType, layerTag, layerName, mapId, PageSize, PageIndex) {
          $scope.inputModalB.layerFrom = [];
          //获取图层列表
          layerContent.getAllListStatus({ LayerType: layerType, LayerTag: layerTag, LayerName: layerName, LayerDesc: '', CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }, PageSize, PageIndex).success(function (data, statues) {
              //console.log(data);
              var tmpArr = angular.copy(data.items);
              for (var i = 0; i < tmpArr.length; i++) {
                  for (var j = 0; j < $scope.inputModalB.layerTo.length; j++) {
                      if (tmpArr[i].id === $scope.inputModalB.layerTo[j].id) {
                          tmpArr[i].ischecked = true;
                          continue;
                      }
                  }
              }
              $scope.inputModalB.layerFrom = angular.copy(tmpArr);
              //console.log($scope.inputModalB.layerFrom)
              //设置分页
              $scope.inputModalB.pageing.pageCounts = data.totalCount;
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      ////////////////////////////////////////////////////////'地图管理'////////////////////////////////////////////////////////////////////////////////

      //根据mapid查询已关联的图层列表
      function getListByMapId() {
          $scope.mapManageModal.mapManage_layers = [];
          mapReleation.getAllListByMapId($scope.mapid).success(function (data, status) {
              //console.log('11111111111111111111', $scope.mapid, data);    
              //console.log(data);

              $scope.mapManageModal.mapManage_layers = data.items;
          });
      }

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
          title: '',
          mapDes: '',
          mapName: '',
          CreateUserId: '',
          CreateUserName: '',
          mapTag: '',
          tagsInput: '',
          tagsList: [],
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
          spaceSel: {},
          scaleSel: {},
          spaceArr: [],
          scaleArr: [],
          mapManage_layers: [],
          mapPreUrl: '',
          xy: '',
          mapLegend: '',
          mapLegendName: '',
          up: function (arr, index) {
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              var layerArr = [];
              swapItems(arr, index, index - 1);
              arr.forEach(function (each, index) {
                  layerArr.push({ MapID: $scope.mapid, DataConfigID: each.dataConfigID, DataStyleID: each.dataStyleID, DataSort: index, name: each.dataStyleName })
              });
              //console.log(layerArr);
              //重新发布地图
              rePublishMap(layerArr);
          },
          down: function (arr, index) {
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              var layerArr = [];
              swapItems(arr, index, index + 1);
              arr.forEach(function (each, index) {
                  layerArr.push({ MapID: $scope.mapid, DataConfigID: each.dataConfigID, DataStyleID: each.dataStyleID, DataSort: index })
              });
              //console.log(layerArr);
              //重新发布地图
              rePublishMap(layerArr);
          },
          editLayersModel: function () {
              //获取地图对应的图层
              layerContent.getAllListByMapID($scope.mapid).success(function (data1, status1) {
                  //console.log('555', data1);
                  $scope.layerManageModal.layersD = data1.items;
                  $scope.layerManageModal.selectedLayer = goSelectedLayer(angular.copy(data1.items));
                  $scope.editLayerModel();
              }).error(function (data, status) {
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });
          },
          //设置样式
          setStyle: function (id, mapID, dataConfigID, dataStyleID, dataSort) {
              $scope.id = id;
              $scope.mapID = mapID;
              $scope.dataConfigID = dataConfigID;
              $scope.styleModal.title = $filter('translate')('views.Style.setStyle');
              $scope.styleModal.styleSelectData = {};
              $scope.styleModal.styleSelectData.id = angular.copy(dataStyleID);
              getStyleTypeTree();
              $scope.styleModal.layerDataSort = dataSort;
              $scope.opensStyleModal();
          },
          //删除样式
          delStyle: function (id, mapID, dataConfigID, dataSort) {
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
                      var mapReleaData = { Id: id, MapID: mapID, DataConfigID: dataConfigID, DataStyleID: null, DataSort: dataSort };
                      //console.log(mapReleaData);
                      mapReleation.update(mapReleaData).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                      }).success(function (data, status) {
                          //console.log(data);
                          //获取地图对应的图层
                          mapReleation.getAllListByMapId(mapID).success(function (data, status) {
                              //console.log(data);
                              var styleIdArr = [], layerIdArr = [];
                              data.items.forEach(function (each) {
                                  styleIdArr.push(each.dataStyleID);
                                  layerIdArr.push(each.dataConfigID);
                              });

                              var obj = {
                                  MapId: mapID,
                                  LayerStr: layerIdArr.join(','),
                                  StyleStr: styleIdArr.join(',')
                              };
                              //重新发布地图
                              mapReleation.changeStyleObject(obj).success(function (data1, status1) {
                                  //console.log(data1, status1);
                                  $scope.mapManageModal.mapManage_layers = data.items;
                                  map.getDetailById(mapID).success(function (map, statues) {
                                      //console.log(data);
                                      waitmask.onHideMask();
                                      alertFun($filter('translate')('views.Style.delStyleSucc'), '', 'success', '#007AFF');
                                      $scope.preMap(map);
                                  }).error(function (data, status) {
                                      waitmask.onHideMask();
                                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
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
          spatialRefence: SpatialRefence,
          typePullDownTreeSelData: '',
          currentType: '',
          onSelected: function (node) {
              //console.log(node);
              $scope.mapManageModal.typePullDownTreeSelData = node;
          },
          mapDataset: {
              baseMap: {
                  map: { zoomlevel: 14, dataserverkey: "", url: '', tilesize: 512, zerolevelsize: 36 },
                  note: { zoomlevel: 14, dataserverkey: "", url: '', tilesize: 512, zerolevelsize: 36 }
              },
              statelliteMap: {
                  map: { zoomlevel: 14, dataserverkey: "", url: '', tilesize: 512, zerolevelsize: 36 },
                  note: { zoomlevel: 14, dataserverkey: "", url: '', tilesize: 512, zerolevelsize: 36 }
              }
          },
          //tableChecked: '',
          //choseOne: function (id) {
          //    $scope.mapManageModal.tableChecked = id;
          //},
          //拖拽
          dropComplete: function (index, data) {
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
                      layerArr.push({ MapID: $scope.mapid, DataConfigID: each.dataConfigID, DataStyleID: each.dataStyleID, DataSort: index })
                  }
              });
              console.log(layerArr);

              //重新发布地图
              rePublishMap(layerArr);
          },
          
          //删除图层关联
          delLayer: function (data, index) {
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
                              layerArr.push({ MapID: $scope.mapid, DataConfigID: each.dataConfigID, DataStyleID: each.dataStyleID, DataSort: index })
                          });
                          //console.log(layerArr);

                          //重新发布地图
                          rePublishMap(layerArr);
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('views.Layer.alertFun.field.layerDelErr'), data.message, 'error', '#007AFF');
                      });
                  }
              });
          }
      }
      

      //地图管理打开
      $scope.openMapManage = function () {
          var data = $scope.typeTreeData[0].children;
          $scope.mapManageModal.typePullDownTreeData = JSON.parse(JSON.stringify(data));
          $scope.mapManageModal.mapPreUrl = [];
      }
      $scope.editMap = function (id, type, tag) {
          $scope.mapManageModal.title = $filter('translate')('views.Map.List.mapManage');
          $scope.mapid = id;
          $scope.mapManageModal.currentType = type; //分类
          $scope.mapManageModal.tagsInput = '';
          var str = angular.copy(tag);
          if (!!str) {
              $scope.mapManageModal.tagsList = str.split(',');
          }
          else {
              $scope.mapManageModal.tagsList = [];
          }

          //获取地图基本信息
          map.getDetailById(id).success(function (data, statues) {
              //console.log(data);
              if (statues == 200) {
                  $scope.mapManageModal.mapName = data.mapName;
                  $scope.mapManageModal.mapDes = data.mapDesc;
                  $scope.mapManageModal.CreateUserId = data.CreateUserId;
                  $scope.mapManageModal.CreateUserName = data.CreateUserName;
                  $scope.mapManageModal.xy = [data.minXName, data.minYName, data.maxXName, data.maxYName];
                  if (data.mapLegend && data.mapLegend != null && data.mapLegend != 'delete') {
                      $scope.mapManageModal.mapLegendName = data.mapLegend.split('/').pop();
                  } else {
                      $scope.mapManageModal.mapLegendName = null;
                  }
                  //$scope.mapManageModal.mapPreUrl = $sce.trustAsResourceUrl(GeoServerUrl + '/wms?service=WMS&version=1.1.0&request=GetMap&layers=' + WorkSpace + ':' + data.mapEnName + '&styles=&bbox=' + data.minX + ',' + data.minY + ',' + data.maxX + ',' + data.maxY + '&width=650&height=446&srs=EPSG:4326&format=application/openlayers')
                  //地图信息

                  $scope.preMap(data);
                  //比例尺
                  dicDataCode.getDetailByTypeID(scaleId).success(function (data2, statues2) {
                      //console.log(data2, statues2);
                      $scope.mapManageModal.scaleArr = data2.items;
                      $scope.mapManageModal.scaleArr.forEach(function (each) {
                          if (each.id == data.mapScale) {
                              $scope.mapManageModal.scaleSel.selected = angular.copy(each);
                          }
                      });
                  });
              }
          });

          //获取地图对应的图层
          getListByMapId();
          $scope.openMapManageFun();
      }
      var maps, layer;
      $scope.preMap = function (data) {
          if (layer) {
              $scope.mapManageModal.removeLayer(layer);
              maps.updateSize();
          }
          layer = newLocalTilesByWMS(GeoServerUrl + '/wms', WorkSpace + ':' + data.mapEnName, 'image/png');
          layer.getSource().updateParams({ "time": Date.now() });
          layer.setZIndex(30);
          var bounds = [data.minX, data.minY, data.maxX, data.maxY];//范围
          maps = $scope.mapManageModal.addLayer(layer, bounds);
          maps.updateSize();
          maps.getView().fit(bounds, maps.getSize());
          //maps.getView().setZoom(6);
      }
      //图例上传
      //$scope.mapManageModal.uploadLegend = function () {
      //    var formData = new FormData();
      //    var name = $('#legend').val();
      //    formData.append('name', name);
      //    formData.append('file', $('#legend')[0].files[0]);
      //    console.log(formData);
      //    $.ajax({
      //        url: 'upload',
      //        type: 'POST',
      //        data: formData,
      //        contentType: false,
      //        processData: false,
      //        success: function (data) {
      //            console.log(data);
      //            if (data.success == "true") {
      //                console.log(data.httpPath);
      //                $scope.mapManageModal.mapLegend = data.httpPath;
      //                //保存图例与地图关系
      //                map.updateMapLegend($scope.mapid, data.httpPath).success(function (data1, status1) {
      //                    console.log(data1, status1)
      //                    SweetAlert.swal({
      //                        title: "成功!",
      //                        text: "上传成功!",
      //                        type: "success",
      //                        confirmButtonColor: "#007AFF"
      //                    });
      //                    $scope.mapManageModal.mapLegendName = data.httpPath.split('/').pop();
      //                    var obj = document.getElementById('legend');
      //                    obj.outerHTML = obj.outerHTML;
      //                })

      //            } else {
      //                SweetAlert.swal({
      //                    title: "失败!",
      //                    text: "上传失败!",
      //                    type: "error",
      //                    confirmButtonColor: "#007AFF"
      //                });
      //            }
      //        }
      //    });
      //}
      ////图例删除
      //$scope.mapManageModal.deleteLegend = function () {
      //    //保存图例与地图关系
      //    map.updateMapLegend($scope.mapid,"delete").success(function (data1, status1) {
      //        console.log(data1);
      //        SweetAlert.swal({
      //            title: "删除成功!",
      //            text: "",
      //            type: "success",
      //            confirmButtonColor: "#007AFF"
      //        });
      //        $scope.mapManageModal.mapLegendName =null;
      //    });
      //}
      //提交地图管理
      $scope.mapManageForm = function (modalInstance, form) {
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);

          var mapInfo = {
              Id: $scope.mapid,
              MapName: $scope.mapManageModal.mapName,
              MapType: $scope.mapManageModal.typePullDownTreeSelData.id,
              MapTag: $scope.mapManageModal.tagsList.join(','),
              SpatialRefence: $scope.mapManageModal.spatialRefence,
              MapScale: typeof ($scope.mapManageModal.scaleSel.selected) === "undefined" ? '' : $scope.mapManageModal.scaleSel.selected.id,
              MapDesc: $scope.mapManageModal.mapDes,
              CreateUserId: $scope.mapManageModal.CreateUserId,
              CreateUserName: $scope.mapManageModal.CreateUserName
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
              insertTagReMap(args, function (data) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('views.Map.alertFun.editMapSucc'), "", "success", "#007AFF");
                  $scope.randomCount = (new Date()).getTime() + getRandom();
                  mapInit($scope.inputText, $scope.typeNodeId, $scope.tagNodeId, $scope.pageSize, $scope.pageIndex);
                  //location.reload([true]);  //强制页面刷新
                  modalInstance.close();
              });
          });
      }

      //修改图层之后重新发布地图
      function rePublishMap(layerArr) {
          mapReleation.multiUpdate($scope.mapid, layerArr).success(function (data1, status1) {
              //console.log(data1);
              if (data1) {
                  //发布地图
                  mapReleation.publicMap($scope.mapid).success(function (data3, statues3) {
                      //console.log(data3);
                      map.getDetailById($scope.mapid).success(function (data, statues) {
                          //console.log(data);
                          waitmask.onHideMask();
                          $scope.preMap(data);
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                      });
                  });
                  getListByMapId();
              }
              else {
                  map.getDetailById($scope.mapid).success(function (data, statues) {
                      //console.log(data);
                      waitmask.onHideMask();
                      alertFun($filter('translate')('views.Map.alertFun.publishErr'), "", "error", "#007AFF");
                      $scope.preMap(data);
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              }
          }).error(function (data, status) {
              waitmask.onHideMask();
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      ////////////////////////////////////////////////////////地图管理弹窗--'图层管理'////////////////////////////////////////////////////////////////////////////////

      //根据点线面来给选中的图层进行排序
      //arr   图层对象数组
      //return    经过排序的图层对象数组
      function goSelectedLayer(arr) {
          var arrPoint = [[], [], []];
          for (var i = 0; i < arr.length; i++) {
              //点
              if (arr[i].dataType === "6b6941f1-67a3-11e7-8eb2-005056bb1c7e") {
                  arrPoint[0].push({ MapID: $scope.mapid, DataConfigID: arr[i].id, DataSort: 0 });
              }
              //线
              if (arr[i].dataType === "7776934c-67a3-11e7-8eb2-005056bb1c7e") {
                  arrPoint[1].push({ MapID: $scope.mapid, DataConfigID: arr[i].id, DataSort: 0 });
              }
              //面
              if (arr[i].dataType === "a2758dc0-67a3-11e7-8eb2-005056bb1c7e") {
                  arrPoint[2].push({ MapID: $scope.mapid, DataConfigID: arr[i].id, DataSort: 0 });
              }
          }

          arrPoint = [].concat(arrPoint[0], arrPoint[1], arrPoint[2]);
          for (i = 0; i < arrPoint.length; i++) {
              arrPoint[i].DataSort = i + 1;
          }
          return arrPoint;
      }

      //图层管理
      $scope.layerManageModal = {
          tagTreeDataC: [],
          typeTreeDataC: [],
          treeFlagC: false,
          treeQueryCtrlC: {},
          selectTabC: function (state) {
              $scope.layerManageModal.showtabC = state;
              $scope.layerManageModal.treeQueryCtrlC.select_branch($scope.layerManageModal.typeTreeDataC[0]);
              $scope.layerManageModal.onTypeSelectedC($scope.layerManageModal.typeTreeDataC[0]);
          },
          typeSelectedC: {},
          onTypeSelectedC: function (node) {
              $scope.layerManageModal.pageingC.pageIndex = 1;
              $scope.layerManageModal.layerName = "";
              $scope.getAllLayer(node.id, '', '', $scope.mapid, $scope.pageSizeC, $scope.pageIndexC);
              $scope.layerManageModal.treeQueryCtrlD.select_branch();
          },
          tagSelectedC: {},
          onTagSelectedC: function (node) {
              $scope.layerManageModal.pageingC.pageIndex = 1;
              $scope.layerManageModal.layerName = "";
              $scope.getAllLayer('', node.id, '', $scope.mapid, $scope.pageSizeC, $scope.pageIndexC);
          },
          //展开/收起树
          closeC: function () {
              if ($scope.layerManageModal.treeFlagC) {
                  $scope.layerManageModal.treeQueryCtrlC.expand_all();
                  $scope.layerManageModal.treeFlagC = false;
              }
              else {
                  $scope.layerManageModal.treeQueryCtrlC.collapse_all();
                  $scope.layerManageModal.treeFlagC = true;
              }
          },

          layersC: [],
          layersD: [],
          checkedDataC: [],
          checkedDataD: [],

          selectedLayer: [],
          difDt: function (dts, val) {
              var ishav = false;
              for (var j in dts) {
                  if (dts[j].id === val.id) {
                      ishav = true;
                      break;
                  }
              }
              if (!ishav)
                  dts.push(val);
          },
          addLayersC: function () {

              for (var i = 0; i < $scope.layerManageModal.checkedDataC.length; i++) {
                  $scope.layerManageModal.checkedDataC[i].ischecked = false;
                  $scope.layerManageModal.checkedDataC[i].selected = false;

                  for (var k = 0; k < $scope.layerManageModal.layersC.length; k++) {
                      if ($scope.layerManageModal.checkedDataC[i].id === $scope.layerManageModal.layersC[k].id) {
                          $scope.layerManageModal.layersC.splice(k, 1);
                      }
                  }
                  $scope.layerManageModal.difDt($scope.layerManageModal.layersD, $scope.layerManageModal.checkedDataC[i]);
              }
              //console.log('222', $scope.layerManageModal.selectedLayer);
              for (var i = 0; i < $scope.layerManageModal.checkedDataD.length; i++) {
                  $scope.layerManageModal.checkedDataD[i].ischecked = false;
                  $scope.layerManageModal.checkedDataD[i].selected = false;
              }

              $scope.layerManageModal.checkedDataC = [];
              $scope.layerManageModal.checkedDataD = [];

              $scope.layerManageModal.selectedLayer = goSelectedLayer(angular.copy($scope.layerManageModal.layersD));
          },
          deleteLayersC: function () {
              for (var i = 0; i < $scope.layerManageModal.checkedDataD.length; i++) {
                  $scope.layerManageModal.checkedDataD[i].ischecked = false;
                  $scope.layerManageModal.checkedDataD[i].selected = false;

                  for (var j = 0; j < $scope.layerManageModal.selectedLayer.length; j++) {
                      if ($scope.layerManageModal.checkedDataD[i].id === $scope.layerManageModal.selectedLayer[j].DataConfigID) {
                          $scope.layerManageModal.selectedLayer.splice(j, 1);
                      }
                  }
                  for (var k = 0; k < $scope.layerManageModal.layersD.length; k++) {
                      if ($scope.layerManageModal.checkedDataD[i].id === $scope.layerManageModal.layersD[k].id) {
                          $scope.layerManageModal.layersD.splice(k, 1);
                      }
                  }
                  $scope.layerManageModal.difDt($scope.layerManageModal.layersC, $scope.layerManageModal.checkedDataD[i]);
              }
              $scope.layerManageModal.layersC = reItem($scope.layerManageModal.layersC);
              //console.log($scope.layerManageModal.selectedLayer);

              for (var i = 0; i < $scope.layerManageModal.checkedDataC.length; i++) {
                  $scope.layerManageModal.checkedDataC[i].ischecked = false;
                  $scope.layerManageModal.checkedDataC[i].selected = false;
              }
              $scope.layerManageModal.checkedDataC = [];
              $scope.layerManageModal.checkedDataD = [];
              $scope.layerManageModal.selectedLayer = goSelectedLayer(angular.copy($scope.layerManageModal.layersD));
          },
          pageChangedC: function () {
              var typeCId = typeof ($scope.layerManageModal.typeSelectedC.id) === "undefined" ? '' : $scope.layerManageModal.typeSelectedC.id;
              var tagCId = typeof ($scope.layerManageModal.tagSelectedC.id) === "undefined" ? '' : $scope.layerManageModal.tagSelectedC.id;
              $scope.getAllLayer(typeCId, tagCId, $scope.layerManageModal.layerName, $scope.mapid, $scope.layerManageModal.pageingC.pageSize, $scope.layerManageModal.pageingC.pageIndex);
          },
          //分页点击事件回调函数
          pageingC: { pageIndex: $scope.pageIndexC, pageSize: $scope.pageSizeC, pageCounts: $scope.pageCountsC, maxSize: 2 },
          layerName: '',
          //搜索图层
          searchLayerByName: function () {
              $scope.pageIndexC = 1;
              $scope.layerManageModal.pageingC.pageIndex = 1;
              var typeCId = typeof ($scope.layerManageModal.typeSelectedC.id) === "undefined" ? '' : $scope.layerManageModal.typeSelectedC.id;
              var tagCId = typeof ($scope.layerManageModal.tagSelectedC.id) === "undefined" ? '' : $scope.layerManageModal.tagSelectedC.id;
              $scope.getAllLayer(typeCId, tagCId, $scope.layerManageModal.layerName, $scope.mapid, $scope.pageSizeC, $scope.pageIndexC);
          }
      };
      var copyModC = angular.copy($scope.layerManageModal);
      $scope.cancelC = function () { //关闭或取消操作
          $scope.layerManageModal = angular.copy(copyModC);
      };
      //'图层管理'弹窗打开
      $scope.openEditLayer = function () {
          $scope.layerManageModal.typeTreeDataC = JSON.parse(JSON.stringify($scope.layerTypeTreeData));
          $scope.layerManageModal.tagTreeDataC = JSON.parse(JSON.stringify($scope.layerTagTreeData));
      }

      //获取图层列表
      $scope.getAllLayer = function (lyerType, layerTag, layerName, mapId, PageSize, PageIndex) {
          layerContent.getAllListStatus({ LayerType: lyerType, LayerTag: layerTag, LayerName: layerName, LayerDesc: mapId }, PageSize, PageIndex).success(function (data, statues) {
              //console.log('444', data);
              //console.log($scope.layerManageModal.layersD);
              var tmpArr = angular.copy(data.items);
              for (var i = 0; i < tmpArr.length; i++) {
                  for (var j = 0; j < $scope.layerManageModal.layersD.length; j++) {
                      if (tmpArr[i].id === $scope.layerManageModal.layersD[j].id) {
                          tmpArr.splice(i, 1);
                          continue;
                      }
                  }
              }
              //console.log(tmpArr);
              $scope.layerManageModal.layersC = angular.copy(tmpArr);
              //设置分页
              $scope.layerManageModal.pageingC.pageCounts = data.totalCount;
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      /*
       * 修改地图对应的图层
       * id    地图ID
       * obj    需要与地图关联的图层对象
       * fun    回调函数
      */
      function changeMapRelLayer(id, obj, fun) {

          //console.log(id, '------', obj);

          mapReleation.multiUpdate(id, obj).success(function (data, status) {
              //console.log('999', data);
              if (!data) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('views.Map.alertFun.reLayerErr'), '', 'error', '#007AFF');
              }
              fun();
          }).error(function (data, status) {
              waitmask.onHideMask();
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //'管理图层'提交
      $scope.submitEditLayer = function (modalInstance) {
          if ($scope.layerManageModal.layersD.length == 0) {
              alertFun($filter('translate')('views.Map.alertFun.mapPrompt2'), '', 'warning', '#007AFF');
          } else {
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              //渲染地图管理页面的图层列表

              mapReleation.getAllListByMapId($scope.mapid).success(function (data, status) {
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
                          layerArr.push({ MapID: $scope.mapid, DataConfigID: object.dataConfigID, DataStyleID: object.dataStyleID, DataSort: object.dataSort });
                      }
                      else {
                          layerArr.push({ MapID: $scope.mapid, DataConfigID: layer.DataConfigID, DataStyleID: null, DataSort: i });
                      }
                  });

                  //重新发布地图
                  mapReleation.multiUpdate($scope.mapid, layerArr).success(function (data1, status1) {
                      //console.log('002', data1);
                      if (data1) {
                          //发布地图
                          mapReleation.publicMap($scope.mapid).success(function (data3, statues3) {
                              //console.log('003', data3);
                              if (data3) {
                                  map.getDetailById($scope.mapid).success(function (data4, statues) {
                                      //console.log('004', data4);
                                      getListByMapId();

                                      $scope.preMap(data4);
                                      waitmask.onHideMask();
                                      //mapInit();
                                      modalInstance.close();
                                  }).error(function (data, status) {
                                      waitmask.onHideMask();
                                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
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
                          map.getDetailById($scope.mapid).success(function (data4, statues) {
                              //console.log(data4);
                              getListByMapId();
                              $scope.preMap(data4);
                              waitmask.onHideMask();
                              alertFun($filter('translate')('views.Map.alertFun.publishErr'), '', 'error', '#007AFF');
                              //mapInit();
                              modalInstance.close();
                          }).error(function (data, status) {
                              waitmask.onHideMask();
                              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
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
          }
      }

      ////////////////////////////////////////////////////////地图管理弹窗--'设置样式'////////////////////////////////////////////////////////////////////////////////
      //设置样式
      $scope.styleModal = {
          title: "",
          layerDataSort: '',
          layerid: '',
          styleArr: [],
          styleName: '',
          //点线面分类ID
          styleDataType: "",
          searchStyleText: '',
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
          }
      };
      var copyModD = angular.copy($scope.styleModal);
      $scope.cancelD = function () { //关闭或取消操作
          $scope.styleModal = angular.copy(copyModD);
      };
      //弹窗打开   编辑、预览事件写在数据绑定上
      $scope.openSetStyle = function () {
          //console.log('设置样式打开');
      }

      //分页查询样式列表
      function setStyleInit(styleName, styleType, styleDataType, pageSize, pageIndex) {
          $scope.styleModal.styleData = [];
          dataStyle.getAllListPage({ StyleName: styleName, StyleType: styleType, StyleDataType: styleDataType, CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }, pageSize, pageIndex).success(function (data, status) {
              //console.log(data);
              if (!!data) {
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
              }
          }).error(function (data, status) {
              //console.log(data);
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //提交
      $scope.setStyleForm = function (modalInstance, form) {

          if (!$scope.styleModal.styleSelectData.id) {
              alertFun($filter('translate')('views.Style.setStylePrompt'), '', 'warning', '#007AFF');
              return;
          }
          //图层管理的样式管理
          if (!!$scope.styleModal.layerDataSort) {
              //样式与图层关联
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              var mapReleaData = { Id: $scope.id, MapID: $scope.mapid, DataConfigID: $scope.dataConfigID, DataStyleID: $scope.styleModal.styleSelectData.id, DataSort: $scope.styleModal.layerDataSort };
              //console.log(mapReleaData);
              mapReleation.update(mapReleaData).error(function (data, status) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              }).success(function (data, status) {
                  //console.log(data);
                  //获取地图对应的图层
                  mapReleation.getAllListByMapId($scope.mapid).success(function (data, status) {
                      //console.log(data);
                      var styleIdArr = [], layerIdArr = [];

                      data.items.forEach(function (each) {
                          styleIdArr.push(each.dataStyleID);
                          layerIdArr.push(each.dataConfigID);
                      });
                      var obj = {
                          MapId: $scope.mapid,
                          LayerStr: layerIdArr.join(','),
                          StyleStr: styleIdArr.join(',')
                      };
                      //重新发布地图
                      mapReleation.changeStyleObject(obj).success(function (data1, status1) {
                          //console.log(data1, status1);
                          $scope.mapManageModal.mapManage_layers = data.items;
                          map.getDetailById($scope.mapid).success(function (map, statues) {
                              //console.log(data);
                              waitmask.onHideMask();
                              alertFun($filter('translate')('views.Style.setStyleSucc'), '', 'success', '#007AFF');
                              $scope.preMap(map);
                          }).error(function (data, status) {
                              waitmask.onHideMask();
                              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                          });
                          modalInstance.close();
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
              //新建地图的样式管理
          else {
              for (var i = 0; i < $scope.inputModal.tableLayers.length; i++) {
                  if ($scope.inputModal.tableLayers[i].id === $scope.styleModal.layerid) {
                      $scope.inputModal.tableLayers[i].dataStyleID = $scope.styleModal.styleSelectData.id;
                      $scope.inputModal.tableLayers[i].dataStyleName = $scope.styleModal.styleSelectData.styleName;
                      break;
                  }
              }
              modalInstance.close();
          }
      };


      /*
       * 保存标签与地图的联系
       * args    { tagName: joinstr, dataID: overAllId, mapLayerID: $scope.mapid }
       * fun    回调函数
      */
      function insertTagReMap(args, fun) {
          tagReleation.multiInsert(args).success(function (data, statues) {
              //console.log(data);
              if (!data) {
                  alertFun($filter('translate')('views.Layer.alertFun.set.setTagErr'), '', 'error', '#007AFF');
              }
              fun(data);
          }).error(function (data, status) {
              waitmask.onHideMask();
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //输入节点的input框获取焦点
      function getNodeInputFocus(classname) {
          $timeout(function () {
              do {
                  $("." + classname + " .font-title-little input").focus();
              }
              while (!$("." + classname)[0]);
          }, 200);
      }

      function alertFun(title, text, type, color) {
          SweetAlert.swal({
              title: title,
              text: text,
              type: type,
              confirmButtonColor: color
          });
      }
      //数组去重
      function reItem(arr) {
          var ret = [];
          for (var i = 0; i < arr.length; i++) {
              if (ret.indexOf(arr[i]) === -1) {
                  ret.push(arr[i]);
              }
          }
          return ret;
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

      //获取样式分类树
      function getStyleTypeTree() {
          $scope.styleModal.styleTypeTreeData.children = [];
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
          });
      }

  }]);


