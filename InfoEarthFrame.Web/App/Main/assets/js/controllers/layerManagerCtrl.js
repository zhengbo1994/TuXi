'use strict';
/**
 * layerManagerCtrl Controller
 */

app.controller('layerManagerCtrl', ['$rootScope', '$scope', '$document', '$filter', 'SweetAlert', '$element', 'selfadapt', "$timeout", 'abp.services.app.layerContent', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.layerField',
    'abp.services.app.dicDataCode', 'abp.services.app.attachment', 'abp.services.app.uploadFile', 'abp.services.app.tagReleation', 'abp.services.app.mapReleation', 'waitmask', 'abp.services.app.layerReadLog', 'abp.services.app.layerFieldDict',
  function ($rootScope, $scope, $document, $filter, SweetAlert, $element, selfadapt, $timeout, layerContent, dataTag, dataType, layerField, dicDataCode, attachment, uploadFile, tagReleation, mapReleation, waitmask, layerReadLog, layerFieldDict) {
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

      //图层的标签、分类的TYPE
      var dataTypeID = "a2faae61-6acd-11e7-87f3-005056bb1c7e";
      //图层类型查询的ID
      var layerDataTypeID = "1cfe51dd-67a3-11e7-8eb2-005056bb1c7e";
      //空间参考查询的ID
      var refTypeID = "2cf1d4ba-67ab-11e7-8eb2-005056bb1c7e";
      //添加一行中数据类型列表查询ID
      var tdDataTypeID = "73160096-67a5-11e7-8eb2-005056bb1c7e";

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


      //图层列表属性dto
      var LayerFieldDto = {
          Id: '',
          LayerID: '',
          AttributeName: '',
          AttributeDesc: '',
          AttributeType: '',
          AttributeLength: '',
          AttributePrecision: '',
          AttributeInputCtrl: '',
          AttributeInputMax: '',
          AttributeInputMin: '',
          AttributeDefault: '',
          AttributeIsNull: '',
          AttributeInputFormat: '',
          Remark: '',
          CreateDT: '',
          AttributeSort: 0
      }

      /*--------------页面左边树形--------------start----*/
      // 统计分析和已保存报表切换实现
      $scope.selectTab = function (state) {
          $scope.showtab1 = state;
          $scope.treeQueryCtrl1.select_branch($scope.typeTreeData[0]);
          $scope.onTypeSelected($scope.typeTreeData[0]);
      }

      //分类查询树
      $scope.typeTreeData = [{
          label: $filter('translate')('views.Layer.Query.class.all'),
          children: [],
          id: ''
      }];
      //当前节点,点击树时自动赋值
      $scope.selectedType = "";

      //选中树节点回调函数
      $scope.onTypeSelected = function (node) {
          //刷新右侧列表
          //console.log(node);

          $scope.layerData = [];
          $scope.inputText.inmodel = "";
          $scope.searchCondition.LayerName = "";
          $scope.searchCondition.LayerType = "";
          $scope.searchCondition.LayerTag = "";
          $scope.searchCondition.LayerType = node.id + "";
          getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
          $scope.treeQueryCtrl2.select_branch();
      };

      //标签查询树
      $scope.tagTreeData = [];
      //当前节点,点击树时自动赋值
      $scope.selectedTag = "";

      //选中树节点回调函数
      $scope.onTagSelected = function (node) {
          //刷新右侧列表
          //console.log(node);

          $scope.layerData = [];
          $scope.inputText.inmodel = "";
          $scope.searchCondition.LayerName = "";
          $scope.searchCondition.LayerType = "";
          $scope.searchCondition.LayerTag = "";
          $scope.searchCondition.LayerTag = node.id + "";
          getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
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

      //图层列表数据
      $scope.layerData = [];
      //搜索条件
      $scope.searchCondition = {
          Id: '',
          LayerName: '',
          DataType: '',
          LayerBBox: '',
          LayerType: '',
          LayerTag: '',
          LayerDesc: '',
          LayerAttrTable: '',
          LayerSpatialTable: '',
          LayerRefence: '',
          CreateDT: '',
          CreateBy: '',
          UploadFileType: '',
          UploadFileName: ''
      };

      //分页
      $scope.maxSize = 5;//页码个数显示数
      $scope.goPage = 1;//转到多少页
      $scope.pageCounts = 0;//32;//总条数
      $scope.pageIndex = 1;//1;//起始页
      $scope.pageSize = 10;//10;//每页显示条数
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
          getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
      };



      /*--------------页面右边表格--------------start----*/
      $scope.inputText = {
          Id: "text1",
          inmodel: '',
          defaultVal: '请输入图层名称查询'
      };

      //搜索图层
      $scope.searchLayer = function () {
          //分类查询
          if (!!$scope.selectedType) {
              $scope.searchCondition.LayerType = $scope.selectedType.id + "";
          }
          //标签查询
          if (!!$scope.selectedTag) {
              $scope.searchCondition.LayerTag = $scope.selectedTag.id + "";
          }
          $scope.searchCondition.LayerName = $scope.inputText.inmodel + '';
          getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
      }

      //导出图层
      $scope.export = function (id, type) {
          $rootScope.loginOut();
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
          if (type == undefined || type == "" || type == 1) {
              layerContent.outputShpFileData(id).success(function (data, status) {
                  console.log(data);
                  if (!!data) {
                      waitmask.onHideMask();
                      window.open(data);
                  }
                  else {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('views.Layer.alertFun.files.exportErr'), '', 'warning', '#007AFF');
                  }
              }).error(function (data, status) {
                  //console.log(data);
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'warning', '#007AFF');
              });
          }
          else {
              layerContent.outputTifFileData(id).success(function (data, status) {
                  console.log(data);
                  if (!!data) {
                      waitmask.onHideMask();
                      window.open(data);
                  }
                  else {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('views.Layer.alertFun.files.exportErr'), '', 'warning', '#007AFF');
                  }
              }).error(function (data, status) {
                  //console.log(data);
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'warning', '#007AFF');
              });
          }
      }

      /*--------------页面右边表格--------------end----*/

      
      /*--------------导入弹窗--------------start----*/
      $scope.importModal = {
          title: "",
          shpFile: {},

          LayerId: '',
          //传到后台的shp文件
          fileInput: [],
          fileListData: {},
          onLoadSuccess: function (ret) {
              //console.log(ret);
              var obj = { fKey: $scope.importModal.LayerId, physicalName: ret.physicalName, logicName: ret.logicName, fileSize: ret.size, extension: ret.extension, physicalPath: ret.physicalPath, httpPath: ret.httpPath };
              var filename = obj.physicalName.substring(0, obj.physicalName.length - obj.extension.length);

              if (angular.isArray($scope.importModal.shpFile[filename])) {
                  $scope.importModal.shpFile[filename].push(obj.extension);
              }
              else {
                  $scope.importModal.shpFile[filename] = [];
                  $scope.importModal.shpFile[filename].push(obj.extension);
              }
              if (obj.extension === ".shp") {
                  $scope.importModal.fileInput.push(obj.physicalName);
              }
              //console.log($scope.importModal.shpFile);
          },
          onDelAll: function () {
              //console.log('onDelAll');
              $scope.importModal.shpFile = {};
              $scope.importModal.fileInput = [];
          },
          del: function (backValue) {
              //console.log(backValue);

              //var name = backValue.file.name;
              var name = backValue.name;

              var filenames = name.split('.');
              var ext = "." + filenames[filenames.length - 1];
              var filename = name.substring(0, name.length - ext.length);

              if (!$scope.importModal.shpFile[filename]) {
                  return;
              }
              for (var i = 0; i < $scope.importModal.shpFile[filename].length; i++) {
                  if ($scope.importModal.shpFile[filename][i] === ext) {
                      $scope.importModal.shpFile[filename].splice(i, 1);
                      break;
                  }
              }
              for (var j = 0; j < $scope.importModal.fileInput.length; j++) {
                  if ($scope.importModal.fileInput[j] === name) {
                      $scope.importModal.fileInput.splice(j, 1);
                  }
              }

              //console.log($scope.importModal.shpFile);
              //console.log($scope.importModal.fileInput);
          }
      };
      $scope.importOpened = function () {
          //console.log('导入窗口');
      };
      function postFile(filename, k, len, modalInstance) {
          //console.log(filename);
          $rootScope.loginOut();
          var usercode = localStorage.getItem('infoearth_spacedata_userCode');
          layerContent.importShpFileData($scope.importModal.LayerId, filename, usercode).success(function (data, status) {
              //console.log(data);
              if (!data.status) {
                  waitmask.onHideMask();
                  alertFun(filename + $filter('translate')('views.Layer.alertFun.submitErr'), data.message, 'error', '#007AFF');
                  modalInstance.close();
                  return;
              }
              else {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('views.Layer.alertFun.files.afterImport1'), $filter('translate')('views.Layer.alertFun.files.afterImport2'), 'success', '#007AFF');
                  $scope.importModal.fileInput = [];
                  $scope.importModal.shpFile = {};
                  getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                  //关闭弹出框
                  modalInstance.close();
              }
              //console.log(status);
          }).error(function () {
              waitmask.onHideMask();
              alertFun(filename + $filter('translate')('views.Layer.alertFun.submitErr'), '', 'error', '#007AFF');
          });
      }

      $scope.importSubmit = function (modalInstance, form) {
          //console.log($scope.importModal);
          var fileObj = reItem($scope.importModal.fileInput);
          if (fileObj.length > 0) {
              //图层导入文件
              if (!!$scope.importModal.LayerId) {
                  if (fileObj.length > 1) {
                      alertFun($filter('translate')('views.Layer.alertFun.files.importPrompt1'), $filter('translate')('views.Layer.alertFun.files.importPrompt2'), 'warning', '#007AFF');
                      return;
                  }

                  for (var i in $scope.importModal.shpFile) {

                      if ($scope.importModal.shpFile[i].length === 0) {
                          continue;
                      }
                      var n = 0;
                      var tmpArr = reItem($scope.importModal.shpFile[i]);

                      for (var j = 0; j < tmpArr.length; j++) {
                          if (n == 4) {
                              break;
                          }
                          if (tmpArr[j] === ".shp" || tmpArr[j] === ".shx" || tmpArr[j] === ".dbf" || tmpArr[j] === ".prj") {
                              n++;
                          }
                      }

                      if (n < 4) {
                          alertFun(i + $filter('translate')('views.Layer.alertFun.files.importLack'), '', 'warning', '#007AFF');
                          return;
                      }
                  }

                  waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                  var len = fileObj.length;
                  for (var k = 0; k < len; k++) {
                      postFile(fileObj[k], k, len, modalInstance);
                  }
              }
              //从shp文件新增字段
              else {
                  if (fileObj.length > 1) {
                      alertFun($filter('translate')('views.Layer.alertFun.files.importPrompt1'), $filter('translate')('views.Layer.alertFun.files.importPrompt2'), 'warning', '#007AFF');
                      return;
                  }
                  for (var i in $scope.importModal.shpFile) {

                      if ($scope.importModal.shpFile[i].length === 0) {
                          continue;
                      }
                      var n = 0;
                      var tmpArr = reItem($scope.importModal.shpFile[i]);

                      for (var j = 0; j < tmpArr.length; j++) {
                          if (n == 4) {
                              break;
                          }
                          if (tmpArr[j] === ".shp" || tmpArr[j] === ".shx" || tmpArr[j] === ".dbf" || tmpArr[j] === ".prj") {
                              n++;
                          }
                      }

                      if (n < 4) {
                          alertFun(i + $filter('translate')('views.Layer.alertFun.files.importLack'), '', 'warning', '#007AFF');
                          return;
                      }
                  }
                  waitmask.onShowMask($filter('translate')('setting.waitText'), 200);

                  layerField.getLayerFieldByFileName(fileObj[0]).success(function (data, status) {
                      //console.log(data);
                      if (!data.isError) {
                          for (var i = 0; i < data.layerField.length; i++) {
                              var tmpData = angular.copy(emptyTableData);
                              tmpData.tableId = getRandom();
                              tmpData.name = data.layerField[i].attributeName;
                              tmpData.dataType = data.layerField[i].attributeTypeName;
                              tmpData.dataTypeID = data.layerField[i].attributeType;
                              tmpData.length = data.layerField[i].attributeLength;
                              tmpData.decimal = data.layerField[i].attributePrecision;

                              $scope.inputModal.tableData = $scope.inputModal.tableData.concat(tmpData);
                          }
                          $scope.inputModal.createFromText = false;
                          $scope.inputModal.createFromFile = true;
                          $scope.importModal.fileInput = [];
                          $scope.importModal.shpFile = {};
                          modalInstance.close();
                          waitmask.onHideMask();
                          //console.log($scope.inputModal.tableData);
                      }
                      else {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                      }
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              }
              
          }
          else {
              alertFun($filter('translate')('views.Layer.alertFun.files.beforeImport'), '', 'warning', '#007AFF');
          }
          
      };

      $scope.importCancel = function () {
          //console.log('导入');
      };

      //打开导入弹窗
      $scope.importOpenWin = function (id) {
          $scope.importModal.title = $filter('translate')('views.Layer.create.newFrom.upload.MAIN');
          $scope.importModal.LayerId = id;
          $scope.importModal.shpFile = {};
          $scope.importModal.fileListData = {};
          $scope.importModal.fileInput = [];
          $scope.openImportFun();
          //console.log($scope.importModal);
      }
      /*--------------导入弹窗--------------end----*/


      /*--------------新增矢量图层弹窗--------------start----*/
      //监听新增一行中小数位一栏
      $scope.$watch(function () {
          var obj = $scope.inputModal.createTdModal;
          return typeof (obj.tdDataTypeSel) == "undefined" ? false : obj.precision;
      }, function (val) {
          if (val === false) { return; }
          if (!$scope.inputModal.createTdModal.tdDataTypeSel.selected) { return; }

          var codename = $scope.inputModal.createTdModal.tdDataTypeSel.selected.codeName;
          if ((codename === '单浮点型' || codename === '双浮点型')) {
              val = val === '' ? 0 : val;
              $scope.inputModal.createTdModal.precision = parseInt(val);
          }
      });
      //监听标签输入框
      $scope.$watch('inputModal.tagsInput', function (val, old) {
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
          addLayerTitle: '',
          //newTdTitle: '',
          //是否禁用输入
          isDisabled: false,
          //手动创建一行
          createFromText: false,
          //从shp文件导入创建
          createFromFile: false,

          //图层名称
          layerName: "",
          //图层类型下拉框
          layerTypeSel: {},
          layerTypeData: [],
          //空间参考下拉框
          spatialRefData: SpatialRefence,
          //分类下拉框
          typePullDownTreeData: [],
          typePullDownTreeSelData: {},
          initialTypeTreeData: {},

          //打开创建分类弹窗
          createClaOpenWinIn: function () {
              //createClaOpenWin();
              $scope.creatTypeModal.title = $filter('translate')('setting.class.newClassTitle');
              $scope.creatTypeModal.classTreedata = angular.copy($scope.typeTreeData[0].children);
              //console.log($scope.creatTypeModal.classTreedata);
              $scope.opencreatstyle();
          },
          //标签
          tags: "",
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
          //表格
          tableData: [],

          //表格的操作按钮
          tabBtnParams: [{
              name: "编辑", click: function (row, name, event) {
                  //请在此用参数（row--当前行, name--按钮的名称, event--事件对象）自定义设置按钮显示与隐藏。
                  if (!$scope.inputModal.isDisabled) {
                      $scope.inputModal.createTdModal = angular.copy(createTdModelPub);
                      $scope.inputModal.createTdModal.newTdTitle = $filter('translate')('views.Layer.create.newField.edit');
                      dicDataCode.getDetailByTypeID(tdDataTypeID).success(function (data, statues) {
                          //console.log(data);

                          $scope.inputModal.createTdModal.tdId = row.tableId;
                          $scope.inputModal.createTdModal.firstname = row.name;
                          $scope.inputModal.createTdModal.tdDataTypeData = data.items;
                          for (var i in data.items) {
                              if (data.items[i].id === row.dataTypeID) {
                                  $scope.inputModal.createTdModal.tdDataTypeSel.selected = data.items[i];
                                  break;
                              }
                          }
                          $scope.inputModal.createTdModal.onSelDataType();

                          $scope.inputModal.createTdModal.length = row.length;
                          $scope.inputModal.createTdModal.precision = row.decimal;
                          $scope.inputModal.createTdModal.unit = row.unit;
                          $scope.inputModal.createTdModal.defaultVal = row.defaultVal;

                          if (row.inputCtrl == "T") {
                              $scope.inputModal.createTdModal.tdInputCtrlSel.selected = { label: $filter('translate')('views.Layer.create.newField.control.required'), value: 'T' };
                          } else if (row.inputCtrl == "F") {
                              $scope.inputModal.createTdModal.tdInputCtrlSel.selected = { label: $filter('translate')('views.Layer.create.newField.control.optional'), value: 'F' };
                          } else {
                              $scope.inputModal.createTdModal.tdInputCtrlSel.selected = { label: " ", value: '' };
                          }

                          $scope.inputModal.createTdModal.maxVal = row.maxVal;
                          $scope.inputModal.createTdModal.minVal = row.minVal;
                          $scope.inputModal.createTdModal.dataTypeCode = row.dataTypeCode;
                          $scope.inputModal.createTdModal.linkCode = row.linkCode;

                          if (row.inputFormat == "S") {
                              $scope.inputModal.createTdModal.tdInputFormatSel.selected = { label: $filter('translate')('views.Layer.create.newField.format.radio'), value: 'S' };
                          } else if (row.inputFormat == "M") {
                              $scope.inputModal.createTdModal.tdInputFormatSel.selected = { label: $filter('translate')('views.Layer.create.newField.format.multiple'), value: 'M' };
                          } else {
                              $scope.inputModal.createTdModal.tdInputFormatSel.selected = { label: " ", value: '' };
                          }

                          $scope.inputModal.createTdModal.dataSource = row.dataSource;
                          $scope.inputModal.createTdModal.formula = row.formula;
                          $scope.inputModal.createTdModal.empty = row.empty;
                          $scope.inputModal.createTdModal.describe = row.describe;
                          $scope.inputModal.createTdModal.dictData = row.dictData;

                          $scope.openNewTdFun();
                      });
                  }
              }
          }, {
              name: "删除", click: function (row, name, event) {
                  //console.log(row);
                  if (!$scope.inputModal.isDisabled) {
                      SweetAlert.swal({
                          title: $filter('translate')('setting.delete'),
                          text: $filter('translate')('setting.confirmDel') + "?",
                          type: "warning",
                          showCancelButton: true,
                          confirmButtonColor: "#DD6B55",
                          confirmButtonText: $filter('translate')('setting.sure'),
                          cancelButtonText: $filter('translate')('setting.cancel')
                      }, function (isConfirm) {
                          if (isConfirm) {
                              var th = $(event.target).parents('.mic-tables').find('th:last');
                              for (var i in $scope.inputModal.tableData) {
                                  if ($scope.inputModal.tableData[i].tableId == row.tableId) {
                                      $scope.inputModal.tableData.splice(i, 1);
                                      break;
                                  }
                              }
                              //修改宽度触发表格的自适应方法
                              var thWidth = th.css('width');
                              var widthNum = Math.ceil(thWidth.split('px')[0]);
                              //console.log(widthNum);
                              $timeout(function () {
                                  th.css('width', (widthNum - 1) + 'px');
                                  th.css('width', (widthNum + 1) + 'px');
                              });

                              if ($scope.inputModal.tableData.length === 0) {
                                  $scope.inputModal.createFromText = false;
                                  $scope.inputModal.createFromFile = false;
                              }
                          }
                      });
                  }
              }
          }],
          //清空table中的属性
          clearTd: function () {
              SweetAlert.swal({
                  title: $filter('translate')('views.Layer.create.emptied'),
                  text: $filter('translate')('views.Layer.create.emptyTip'),
                  type: "warning",
                  showCancelButton: true,
                  confirmButtonColor: "#DD6B55",
                  confirmButtonText: $filter('translate')('setting.sure'),
                  cancelButtonText: $filter('translate')('setting.cancel')
              }, function (isConfirm) {
                  if (isConfirm) {
                      $scope.inputModal.tableData = [];
                      $scope.inputModal.createFromText = false;
                      $scope.inputModal.createFromFile = false;
                  }
              });
          },
          //新增字段的弹窗model
          createTdModal: {},
          createTdOpenWinIn: function () {
              //查询数据类型列表
              $scope.inputModal.createTdModal = angular.copy(createTdModelPub);
              $scope.inputModal.createTdModal.newTdTitle = $filter('translate')('views.Layer.create.newField.create');
              $scope.inputModal.createTdModal.tdId = getRandom();
              dicDataCode.getDetailByTypeID(tdDataTypeID).success(function (data, statues) {
                  //console.log(data);
                  $scope.inputModal.createTdModal.tdDataTypeData = data.items;
              });
              
              $scope.openNewTdFun();
          },
          //从shp文件新增字段的弹窗
          createFromShpWin:function(){
              $scope.importModal.title = $filter('translate')('views.Layer.create.newFrom.upload.MAIN');
              $scope.importModal.LayerId = "";
              $scope.importModal.shpFile = {};
              $scope.importModal.fileListData = {};
              $scope.importModal.fileInput = [];
              $scope.openImportFun();
          },
          //选择shp文件解析生成table中的栏位
          onAllSuccess: function (obj) {
              //console.log(obj);
              for (var i in obj) {
                  if (obj[i].extension === ".shp") {
                      
                  }
              }
          },
          createTdOpened: function () {
          },
          createTdSubmit: function (modalInstance, form) {
              //console.log($scope.inputModal.createTdModal);
              var tmpData = angular.copy(emptyTableData);
              var inputData = $scope.inputModal.createTdModal;
              if (!!inputData.firstname) {
                  if (!!inputData.tdDataTypeSel.selected) {
                      if (inputData.readOnly1 || !!inputData.length) {
                          if (inputData.firstname.toUpperCase() === "SID") {
                              alertFun($filter('translate')('views.Layer.alertFun.field.namePrompt1'), '', 'warning', '#007AFF');
                              return;
                          }
                          if (!compareLength(inputData.firstname.length, 10, "字段名称")){
                              return;
                          }
                          //for(var i in inputData.tableHeadData){
                          //    var isTrue = compareLength(inputData.tableHeadData[i].text.length,inputData.tableHeadData[i].length,inputData.tableHeadData[i].label);
                          //    if(!isTrue){
                          //        return;
                          //    }
                          //}
                          if ((isNaN(parseInt(inputData.length)) && inputData.length != "") && inputData.tdDataTypeSel.selected.codeName !== '时间型') {
                              alertFun($filter('translate')('views.Layer.alertFun.field.lengthPrompt1'), '', 'warning', '#007AFF');
                              return;
                          }
                          if ((isNaN(parseInt(inputData.precision)) && inputData.precision != "") && (inputData.tdDataTypeSel.selected.codeName === '单浮点型' || inputData.tdDataTypeSel.selected.codeName === '双浮点型')) {
                              alertFun($filter('translate')('views.Layer.alertFun.field.precisionPrompt'), '', 'warning', '#007AFF');
                              return;
                          }
                          var isNew = 0;
                          var isRe = 0;
                          //判断是新增还是编辑
                          for (var i in $scope.inputModal.tableData) {
                              var compare = $scope.inputModal.tableData[i];
                              //判断字段名称是否重复
                              if (compare.name.toUpperCase() == inputData.firstname.toUpperCase() && compare.tableId !== inputData.tdId) {
                                  isRe++;
                                  break;
                              }
                              //如果tdId相同，进行编辑操作
                              if (compare.tableId === inputData.tdId) {
                                  isNew = parseInt(i) + 1;
                              }
                          }
                          if (!isRe) {
                              //如果没有tdId相同，进行新增操作
                              if (!isNew) {
                                  tmpData.tableId = inputData.tdId;
                                  tmpData.name = inputData.firstname;
                                  tmpData.dataType = inputData.tdDataTypeSel.selected.codeName;
                                  tmpData.dataTypeID = inputData.tdDataTypeSel.selected.id;
                                  tmpData.length = isNaN(parseInt(inputData.length)) ? "" : parseInt(inputData.length);
                                  tmpData.decimal = isNaN(parseInt(inputData.precision)) ? "" : parseInt(inputData.precision);
                                  tmpData.unit = inputData.unit;
                                  tmpData.defaultVal = inputData.defaultVal;

                                  if (!!inputData.tdInputCtrlSel.selected) {
                                      tmpData.inputCtrl = inputData.tdInputCtrlSel.selected.value;
                                  }

                                  tmpData.maxVal = inputData.maxVal;
                                  tmpData.minVal = inputData.minVal;
                                  tmpData.dataTypeCode = inputData.dataTypeCode;
                                  tmpData.linkCode = inputData.linkCode;

                                  if (!!inputData.tdInputFormatSel.selected) {
                                      tmpData.inputFormat = inputData.tdInputFormatSel.selected.value;
                                  }

                                  tmpData.dataSource = inputData.dataSource;
                                  //tmpData.formula = inputData.formula;
                                  tmpData.formula = inputData.realFormula;
                                  tmpData.empty = inputData.empty;
                                  tmpData.describe = inputData.describe;
                                  tmpData.dictData = inputData.dictData;

                                  $scope.inputModal.tableData = $scope.inputModal.tableData.concat(tmpData);

                                  $scope.inputModal.createFromText = true;
                                  $scope.inputModal.createFromFile = false;
                                  //console.log(form);
                                  //关闭弹出框
                                  modalInstance.close();
                              }
                              else {
                                  $scope.inputModal.tableData[isNew - 1].name = inputData.firstname;
                                  $scope.inputModal.tableData[isNew - 1].dataType = inputData.tdDataTypeSel.selected.codeName;
                                  $scope.inputModal.tableData[isNew - 1].dataTypeID = inputData.tdDataTypeSel.selected.id;
                                  $scope.inputModal.tableData[isNew - 1].length = isNaN(parseInt(inputData.length)) ? "" : parseInt(inputData.length);
                                  $scope.inputModal.tableData[isNew - 1].decimal = isNaN(parseInt(inputData.precision)) ? "" : parseInt(inputData.precision);
                                  $scope.inputModal.tableData[isNew - 1].unit = inputData.unit;
                                  $scope.inputModal.tableData[isNew - 1].defaultVal = inputData.defaultVal;
                                  if (!!inputData.tdInputCtrlSel.selected) {
                                      $scope.inputModal.tableData[isNew - 1].inputCtrl = inputData.tdInputCtrlSel.selected.value;
                                  }
                                  $scope.inputModal.tableData[isNew - 1].maxVal = inputData.maxVal;
                                  $scope.inputModal.tableData[isNew - 1].minVal = inputData.minVal;

                                  $scope.inputModal.tableData[isNew - 1].dataTypeCode = inputData.dataTypeCode;
                                  $scope.inputModal.tableData[isNew - 1].linkCode = inputData.linkCode;

                                  if (!!inputData.tdInputFormatSel.selected) {
                                      $scope.inputModal.tableData[isNew - 1].inputFormat = inputData.tdInputFormatSel.selected.value;
                                  }
                                  $scope.inputModal.tableData[isNew - 1].dataSource = inputData.dataSource;
                                  //$scope.inputModal.tableData[isNew - 1].formula = inputData.formula;
                                  $scope.inputModal.tableData[isNew - 1].formula = inputData.realFormula;
                                  $scope.inputModal.tableData[isNew - 1].empty = inputData.empty;
                                  $scope.inputModal.tableData[isNew - 1].describe = inputData.describe;
                                  $scope.inputModal.tableData[isNew - 1].dictData = inputData.dictData;
                                  //关闭弹出框
                                  modalInstance.close();
                              }
                          }
                          else {
                              alertFun($filter('translate')('views.Layer.alertFun.field.namePrompt2'), '', 'warning', '#007AFF');
                          }
                      }
                      else {
                          alertFun($filter('translate')('views.Layer.alertFun.field.lengthPrompt2'), '', 'warning', '#007AFF');
                      }
                  }
                  else {
                      alertFun($filter('translate')('views.Layer.alertFun.field.dataTypePrompt'), '', 'warning', '#007AFF');
                  }
              }
              else {
                  alertFun($filter('translate')('views.Layer.alertFun.field.namePrompt3'), '', 'warning', '#007AFF');
              }
          },
          createTdCancel: function () {
              //console.log('新增字段close');
          },
          //图层导入记录
          fileImportData: [],
          //弹出导入通知的结果信息
          seeMsg: function (status, msg) {
              if (status == 1) {
                  alertFun($filter('translate')('views.Layer.alertFun.files.importSuccess'), '', 'success', '#007AFF');
              }
              if (status == 2) {
                  alertFun($filter('translate')('views.Layer.alertFun.files.importErr'), msg, 'error', '#007AFF');
              }
          }
      };

      var emptyTableData = {
          name: "", dataType: "", dataTypeID: "", length: "", decimal: "", unit: "", defaultVal: "", inputCtrl: "", maxVal: "", minVal: "",
          dataTypeCode: "", linkCode: "", inputFormat: "", dataSource: "", formula: "", empty: "", describe: "", tableId: "", dictData: []
      };

      var copyMod = angular.copy($scope.inputModal);

      //提交数据操作
      $scope.submitForm = function (modalInstance, form) {
          //console.log($scope.inputModal.layerTypeSel.selected);
          //console.log($scope.inputModal.typePullDownTreeSelData);
          $rootScope.loginOut();

          var addObj = angular.copy($scope.searchCondition);
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
          if (!!$scope.inputModal.layerName) {
              if (!!$scope.inputModal.layerTypeSel.selected) {
                  if (!!$scope.inputModal.typePullDownTreeSelData.id) {
                      if (!!$scope.inputModal.tagsInput) {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('views.Layer.alertFun.field.tagPrompt1'), $filter('translate')('views.Layer.alertFun.field.tagPrompt2'), 'warning', '#007AFF');
                          return;
                      }

                      //字段属性
                      var tableTmp = $scope.inputModal.tableData;
                      //字典项
                      var dictTmp = [];
                      for (var i = 0; i < tableTmp.length; i++) {
                          if (tableTmp[i].dictData.length > 0) {
                              for (var j = 0; j < tableTmp[i].dictData.length; j++) {
                                  dictTmp = dictTmp.concat({ AttributeID: tableTmp[i].tableId, FieldDictName: tableTmp[i].dictData[j].text, FieldDictDesc: tableTmp[i].dictData[j].remark });
                              }
                          }
                      }

                      if (!!tableTmp[0]) {
                          //添加图层
                          addObj.LayerName = $scope.inputModal.layerName;
                          addObj.DataType = $scope.inputModal.layerTypeSel.selected.id;
                          addObj.LayerRefence = $scope.inputModal.spatialRefData;
                          addObj.LayerType = $scope.inputModal.typePullDownTreeSelData.id;
                          addObj.LayerTag = $scope.inputModal.tagsList.join(',');
                          addObj.CreateBy = localStorage.getItem('infoearth_spacedata_userCode');
                          addObj.UploadFileType = "1";

                          layerContent.insert(addObj).success(function (data, statues) {
                              //console.log(data);
                              if (statues === 200) {
                                  //添加字典项
                                  layerFieldDict.multiInsert(dictTmp).success(function (data2, statues) {
                                      //console.log(data2);

                                      var fieldDtoList = [];
                                      //添加图层的配置信息
                                      for (var i = 0; i < tableTmp.length; i++) {
                                          var fieldTmpDto = angular.copy(LayerFieldDto);

                                          fieldTmpDto.Id = tableTmp[i].tableId;
                                          fieldTmpDto.LayerID = data.id;
                                          fieldTmpDto.AttributeName = tableTmp[i].name;
                                          fieldTmpDto.AttributeDesc = '';
                                          fieldTmpDto.AttributeType = tableTmp[i].dataTypeID;
                                          fieldTmpDto.AttributeLength = tableTmp[i].length;
                                          fieldTmpDto.AttributePrecision = tableTmp[i].decimal;
                                          fieldTmpDto.AttributeInputCtrl = tableTmp[i].inputCtrl;
                                          fieldTmpDto.AttributeInputMax = !!tableTmp[i].maxVal ? tableTmp[i].maxVal : "";
                                          fieldTmpDto.AttributeInputMin = !!tableTmp[i].minVal ? tableTmp[i].minVal : "";
                                          fieldTmpDto.AttributeDefault = tableTmp[i].defaultVal;
                                          fieldTmpDto.AttributeIsNull = !!tableTmp[i].empty ? tableTmp[i].empty : "";
                                          fieldTmpDto.AttributeInputFormat = tableTmp[i].inputFormat;
                                          fieldTmpDto.Remark = !!tableTmp[i].describe ? tableTmp[i].describe : "";
                                          fieldTmpDto.AttributeUnit = tableTmp[i].unit;
                                          fieldTmpDto.AttributeDataType = tableTmp[i].dataTypeCode;
                                          fieldTmpDto.AttributeValueLink = !!tableTmp[i].linkCode ? tableTmp[i].linkCode : "";
                                          fieldTmpDto.AttributeDataSource = tableTmp[i].dataSource;
                                          fieldTmpDto.AttributeCalComp = tableTmp[i].formula;
                                          fieldTmpDto.AttributeSort = i;

                                          fieldDtoList = fieldDtoList.concat(fieldTmpDto);
                                      }
                                      layerField.multiInsert(fieldDtoList).success(function (data1, statues1) {
                                          //console.log(data1, statues1);
                                          if (data1) {
                                              //建表
                                              layerField.createTable(data.id).success(function (data2, statues2) {
                                                  //console.log(data2);
                                                  //保存标签与图层的联系
                                                  if (data2) {
                                                      var args = {
                                                          tagName: $scope.inputModal.tagsList.join(','),
                                                          dataID: dataTypeID,
                                                          mapLayerID: data.id
                                                      }
                                                      tagReleation.multiInsert(args).success(function (data, status) {
                                                          //console.log(data, status);
                                                          if (data) {
                                                              waitmask.onHideMask();
                                                              getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                                                              alertFun($filter('translate')('views.Layer.alertFun.field.addLayerSuccess'), "", 'success', '#007AFF');
                                                              //关闭弹出框
                                                              modalInstance.close();
                                                          }
                                                          else {
                                                              waitmask.onHideMask();
                                                              getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                                                              alertFun($filter('translate')('views.Layer.alertFun.field.addTagErr'), "", 'error', '#007AFF');
                                                              //关闭弹出框
                                                              modalInstance.close();
                                                          }
                                                      }).error(function (data, statues) {
                                                          waitmask.onHideMask();
                                                          console.log(data);
                                                          alertFun($filter('translate')('views.Layer.alertFun.field.addTagErr'), "", 'error', '#007AFF');
                                                      });
                                                  }
                                                  else {
                                                      //执行删除操作
                                                      tagReleation.deleteByMapID(data.id).success(function (d, statues) {
                                                          //console.log(data);
                                                          layerField.deleteByLayerID(data.id).success(function (d, statues) {
                                                              //console.log(data);
                                                              layerContent.delete(data.id).success(function (d, statues) {
                                                                  //console.log(data);
                                                                  layerContent.deleteLayer(data.id).success(function (d, statues) {
                                                                      getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                                                                  });
                                                              });
                                                          });
                                                      });
                                                      waitmask.onHideMask();
                                                      alertFun($filter('translate')('views.Layer.alertFun.field.addTableErr'), "", 'error', '#007AFF');
                                                      getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                                                      //关闭弹出框
                                                      modalInstance.close();
                                                  }
                                              }).error(function (data, statues) {
                                                  console.log(data);
                                                  waitmask.onHideMask();
                                                  alertFun($filter('translate')('views.Layer.alertFun.field.addTableErr'), "", 'error', '#007AFF');
                                              });
                                          }
                                          else {
                                              waitmask.onHideMask();
                                              alertFun($filter('translate')('views.Layer.alertFun.field.addFieldErr'), "", 'error', '#007AFF');
                                              getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                                              //关闭弹出框
                                              modalInstance.close();
                                          }
                                      });
                                  }).error(function (data2, statues) {
                                      console.log(data2);
                                      waitmask.onHideMask();
                                      alertFun($filter('translate')('views.Layer.alertFun.field.addFieldErr'), "", 'error', '#007AFF');
                                  });
                              }
                              else {
                                  waitmask.onHideMask();
                                  alertFun($filter('translate')('views.Layer.alertFun.field.addLayerErr'), "", 'error', '#007AFF');
                                  getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                                  //关闭弹出框
                                  modalInstance.close();
                              }
                          }).error(function (data, statues) {
                              console.log(data);
                              waitmask.onHideMask();
                              alertFun($filter('translate')('views.Layer.alertFun.field.addLayerErr'), "", 'error', '#007AFF');
                          });
                      }
                      else {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('views.Layer.alertFun.field.layerPrompt4'), "", 'warning', '#007AFF');
                      }
                  }
                  else {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('views.Layer.alertFun.field.layerPrompt3'), "", 'warning', '#007AFF');
                  }
              }
              else {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('views.Layer.alertFun.field.layerPrompt2'), "", 'warning', '#007AFF');
              }
          }
          else {
              waitmask.onHideMask();
              alertFun($filter('translate')('views.Layer.alertFun.field.layerPrompt1'), "", 'warning', '#007AFF');
          }
      };

      //关闭或取消操作
      $scope.cancel = function () {
          $scope.inputModal = angular.copy(copyMod);
      };
      //打开新增矢量图层弹窗
      $scope.addLayerOpenWin = function () {
          $scope.inputModal.addLayerTitle = $filter('translate')('views.Layer.create.vectorLayer');
          $scope.inputModal.isDisabled = false;
          $scope.inputModal.addlayerSelectDis = {
              "width": ""
          };
          //查询图层类型
          dicDataCode.getDetailByTypeID(layerDataTypeID).success(function (data, statues) {
              //console.log(data);
              for (var i = 0; i < data.items.length; i++) {
                  if (data.items[i].id === "acf11b57-0626-4e49-b385-2e9a4195221c") {
                      data.items.splice(i, 1);
                  }
              }
              $scope.inputModal.layerTypeData = data.items;
          });
          
          //查询分类
          $scope.inputModal.typePullDownTreeData = JSON.parse(JSON.stringify($scope.typeTreeData[0].children));
          
          $scope.openAddLayerFun();
      };
      //模态窗口打开时的回调函数
      $scope.openedBack = function () {

      };
      /*--------------新增矢量图层弹窗--------------end----*/


      /*--------------新增影像图层弹窗--------------start----*/
      //监听标签输入框
      $scope.$watch('imageLayerModal.tagsInput', function (val, old) {
          if (myBrowser() === 'Chrome') {
              return;
          }
          $document.find('.mytagsfocus').blur();
          $timeout(function () {
              $document.find('.mytagsfocus').focus();
              if (val.length > 20) {
                  $scope.imageLayerModal.tagsInput = $scope.imageLayerModal.tagsInput.substr(0, 20);
              }
              if ($scope.imageLayerModal.tagsList.length === 5) {
                  $scope.imageLayerModal.tagsInput = '';
                  return;
              }
              if (val.lastIndexOf(' ') !== -1) {
                  $timeout(function () {
                      $scope.imageLayerModal.tagsInput = $.trim(val).replace(',', '');
                  });
              }
              if (val.lastIndexOf('，') === -1) return;
              $scope.imageLayerModal.setdata(old);
              $scope.imageLayerModal.tagsInput = old;
          });
      });

      $scope.imageLayerModal = {
          title: "",
          //图层名称
          layerName: "",
          //图层类型
          layerType: $filter('translate')('views.Layer.create.imageLayer.Type'),
          //空间参考下拉框
          spatialRefData: SpatialRefence,
          //分类下拉框
          typePullDownTreeData: [],
          typePullDownTreeSelData: {},

          //打开创建分类弹窗
          createClaOpenWinIn: function () {
              $scope.creatTypeModal.title = $filter('translate')('setting.class.newClassTitle');
              $scope.creatTypeModal.classTreedata = angular.copy($scope.typeTreeData[0].children);
              //console.log($scope.creatTypeModal.classTreedata);
              $scope.opencreatstyle();
          },
          //标签
          tags: "",
          tagsInput: '',
          tagsList: [],
          delItems: function (a, b) {
              for (var i = 0; i < this.tagsList.length; i++) {
                  if ($scope.imageLayerModal.tagsList[i] === a) {
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

          //导入
          tmpFile: {},
          fileInput: [],
          onLoadSuccess: function (ret) {
              //console.log(ret);
              var obj = { fKey: $scope.imageLayerModal.LayerId, physicalName: ret.physicalName, logicName: ret.logicName, fileSize: ret.size, extension: ret.extension, physicalPath: ret.physicalPath, httpPath: ret.httpPath };
              var filename = obj.physicalName.substring(0, obj.physicalName.length - obj.extension.length);

              if (angular.isArray($scope.imageLayerModal.tmpFile[filename])) {
                  $scope.imageLayerModal.tmpFile[filename].push(obj.extension);
              }
              else {
                  $scope.imageLayerModal.tmpFile[filename] = [];
                  $scope.imageLayerModal.tmpFile[filename].push(obj.extension);
              }
              $scope.imageLayerModal.fileInput.push(obj.physicalName);

              //console.log($scope.imageLayerModal.tmpFile);
          },
          onDelAll: function () {
              $scope.imageLayerModal.tmpFile = {};
              $scope.imageLayerModal.fileInput = [];
          },
          del: function (backValue) {
              //console.log(backValue);

              //var name = backValue.file.name;
              var name = backValue.name;

              var filenames = name.split('.');
              var ext = "." + filenames[filenames.length - 1];
              var filename = name.substring(0, name.length - ext.length);

              if (!$scope.imageLayerModal.tmpFile[filename]) {
                  return;
              }
              for (var i = 0; i < $scope.imageLayerModal.tmpFile[filename].length; i++) {
                  if ($scope.imageLayerModal.tmpFile[filename][i] === ext) {
                      $scope.imageLayerModal.tmpFile[filename].splice(i, 1);
                      break;
                  }
              }
              for (var j = 0; j < $scope.imageLayerModal.fileInput.length; j++) {
                  if ($scope.imageLayerModal.fileInput[j] === name) {
                      $scope.imageLayerModal.fileInput.splice(j, 1);
                  }
              }

              //console.log($scope.imageLayerModal.tmpFile);
              //console.log($scope.imageLayerModal.fileInput);
          },

          opened: function () { },
          submit: function (modalInstance, form) {
              $rootScope.loginOut();

              var addObj = angular.copy($scope.searchCondition);
              if (!$scope.imageLayerModal.layerName) {
                  alertFun($filter('translate')('views.Layer.alertFun.field.layerPrompt1'), "", 'warning', '#007AFF');
                  return;
              }
              if (!$scope.imageLayerModal.typePullDownTreeSelData.id) {
                  alertFun($filter('translate')('views.Layer.alertFun.field.layerPrompt3'), "", 'warning', '#007AFF');
                  return;
              }
              if (!!$scope.imageLayerModal.tagsInput) {
                  alertFun($filter('translate')('views.Layer.alertFun.field.tagPrompt1'), $filter('translate')('views.Layer.alertFun.field.tagPrompt2'), 'warning', '#007AFF');
                  return;
              }
              var fileObj = reItem($scope.imageLayerModal.fileInput);
              console.log(fileObj);
              if (!fileObj.length) {
                  alertFun($filter('translate')('views.Layer.alertFun.imageFiles.importLack'), "", 'warning', '#007AFF');
                  return;
              }
              var numOfTif = 0;
              for (var i = 0; i < fileObj.length; i++) {
                  if (fileObj[i].indexOf(".tif") === fileObj[i].length - 4) {
                      numOfTif++;
                  }
              }
              if (numOfTif > 1) {
                  alertFun($filter('translate')('views.Layer.alertFun.imageFiles.importPrompt1'), $filter('translate')('views.Layer.alertFun.imageFiles.importPrompt2'), 'warning', '#007AFF');
                  return;
              }
              //添加图层
              addObj.LayerName = $scope.imageLayerModal.layerName;
              addObj.DataType = "acf11b57-0626-4e49-b385-2e9a4195221c";
              addObj.LayerRefence = $scope.imageLayerModal.spatialRefData;
              addObj.LayerType = $scope.imageLayerModal.typePullDownTreeSelData.id;
              addObj.LayerTag = $scope.imageLayerModal.tagsList.join(',');
              addObj.CreateBy = localStorage.getItem('infoearth_spacedata_userCode');
              addObj.UploadFileType = "2";
              addObj.UploadFileName = fileObj.join('*');

              //console.log(addObj);
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              //return;
              layerContent.insert(addObj).success(function (data, statues) {
                  //console.log(data);
                  if (statues === 200) {
                      var args = {
                          tagName: $scope.imageLayerModal.tagsList.join(','),
                          dataID: dataTypeID,
                          mapLayerID: data.id
                      }
                      tagReleation.multiInsert(args).success(function (data, status) {
                          //console.log(data, status);
                          if (data) {
                              waitmask.onHideMask();
                              getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                              alertFun($filter('translate')('views.Layer.alertFun.field.addLayerSuccess'), "", 'success', '#007AFF');
                              modalInstance.close();
                          }
                          else {
                              waitmask.onHideMask();
                              getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                              alertFun($filter('translate')('views.Layer.alertFun.field.addTagErr'), "", 'error', '#007AFF');
                              modalInstance.close();
                          }
                      }).error(function (data, statues) {
                          //console.log(data);
                          waitmask.onHideMask();
                          alertFun($filter('translate')('views.Layer.alertFun.field.addTagErr'), "", 'error', '#007AFF');
                          modalInstance.close();
                      });
                  }
                  else {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('views.Layer.alertFun.field.addLayerErr'), "", 'error', '#007AFF');
                      getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                      //关闭弹出框
                      modalInstance.close();
                  }
              }).error(function (data, statues) {
                  console.log(data);
                  waitmask.onHideMask();
                  alertFun($filter('translate')('views.Layer.alertFun.field.addLayerErr'), "", 'error', '#007AFF');
              });
          },
          cancel: function () { }
      };
      
      //打开新增影像图层弹窗
      $scope.addImageLayer = function () {
          $scope.imageLayerModal.title = $filter('translate')('views.Layer.create.imageLayer.MAIN');
          $scope.imageLayerModal.layerName = "";
          $scope.imageLayerModal.typePullDownTreeData = JSON.parse(JSON.stringify($scope.typeTreeData[0].children));
          $scope.imageLayerModal.typePullDownTreeSelData = {};
          $scope.imageLayerModal.tags = "";
          $scope.imageLayerModal.tagsInput = "";
          $scope.imageLayerModal.tagsList = [];
          $scope.imageLayerModal.tmpFile = {};
          $scope.imageLayerModal.fileInput = [];

          $scope.openImageLayerFun();
      };
      /*--------------新增影像图层弹窗--------------end----*/


      /*--------------分类设置弹窗--------------start----*/
      //'分类管理'打开
      $scope.inputModalType = {
          title: "",
          //下拉树
          typePullDownTreeData: [],
          typePullDownTreeSelData: {},
          initialTypeTreeData: {},
          //下拉树选中事件
          onComboSelected: function (a,b) {
              //console.log($scope.inputModalType.typePullDownTreeSelData);
          },
          //打开创建分类弹窗
          openCreatStylea: function () {
              $scope.creatTypeModal.title = $filter('translate')('setting.class.newClassTitle');
              $scope.creatTypeModal.classTreedata = angular.copy($scope.typeTreeData[0].children);
              //console.log($scope.creatTypeModal.classTreedata);
              $scope.opencreatstyle();
          },
          opened: function () { },
          submit: function (modalInstance) {
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              //console.log($scope.layerId, $scope.inputModalType.typePullDownTreeSelData);
              //分类与图层关系保存
              layerContent.updateDataType($scope.layerId, $scope.inputModalType.typePullDownTreeSelData.id).success(function (data, status) {
                  //console.log(data);
                  waitmask.onHideMask();
                  alertFun($filter('translate')('views.Layer.alertFun.set.setClassSucc'), "", 'success', '#007AFF');
                  getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                  modalInstance.close();
              }).error(function (data, status) {
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });
          },
          cancel: function () { }
      };

      $scope.typeManageId = function (type, id) {
          $scope.currentType = type;
          $scope.layerId = id;

          $scope.inputModalType.title = $filter('translate')('views.Layer.List.classSet');
          $scope.inputModalType.typePullDownTreeData = JSON.parse(JSON.stringify($scope.typeTreeData[0].children));
          $scope.inputModalType.typePullDownTreeSelData = {};
          $scope.inputModalType.initialTypeTreeData = {};

          var breakException = "hasSelected";
          try{
              $scope.inputModalType.typePullDownTreeData.forEach(function (each) {
                  if (each.id == $scope.currentType) {
                      $scope.inputModalType.initialTypeTreeData = angular.copy(each);
                      throw breakException;
                  }

                  var parentNode = each.children;
                  parentNode.forEach(function (item) {
                      if (item.id == $scope.currentType) {
                          $scope.inputModalType.initialTypeTreeData = angular.copy(item);
                          throw breakException;
                      }
                  });
              });
          } catch (e) {
              //console.log(e);
              //console.log($scope.inputModalType.initialTypeTreeData);
          }
          
          $scope.openClaSetFun();
      }

      /*--------------分类设置弹窗--------------end----*/


      /*--------------标签管理弹窗--------------start----*/
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
      };
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

      var inputModalTag = angular.copy($scope.inputModalTag);
      $scope.cancelTagManage = function () { //关闭或取消操作
          $scope.inputModalTag = angular.copy(inputModalTag);
      };

      //'标签管理'打开
      $scope.openTagManage = function () {
          //2017-8-3
          $timeout(function () {//延时INPUT获取焦点
              $document.find('.mytagsfocus').focus();
          }, 450);
      };
      $scope.tagManageId = function (tag, id) {
          //console.log(tag);
          $scope.layerId = id;
          $scope.inputModalTag.title = $filter('translate')('setting.tags.manage');
          //2017-8-3
          var str = angular.copy(tag);
          if (!!str) {
              $scope.inputModalTag.tagsList = str.split(',');
          }

          $scope.openTagManagerFun();
      }

      //'标签管理'提交  
      $scope.tagManageForm = function (modalInstance) {
          if (!!$scope.inputModalTag.tagsInput) {
              alertFun($filter('translate')('views.Layer.alertFun.field.tagPrompt1'), $filter('translate')('views.Layer.alertFun.field.tagPrompt2'), 'warning', '#007AFF');
              return;
          }
          waitmask.onShowMask($filter('translate')('setting.waitText'), 200);

          //更新地图与标签关系
          var joinstr = $scope.inputModalTag.tagsList.join(',');
          var input = {
              tagName: joinstr,
              dataID: dataTypeID,
              mapLayerID: $scope.layerId
          }
          tagReleation.multiInsert(input).success(function (data, status) {
              //console.log(data);
              waitmask.onHideMask();
              alertFun($filter('translate')('views.Layer.alertFun.set.setTagSucc'), "", 'success', '#007AFF');
              getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
              modalInstance.close();
          }).error(function (data, status) {
              waitmask.onHideMask();
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }
      /*--------------标签管理弹窗--------------end----*/


      /*--------------创建分类弹窗--------------start----*/
      //创建分类的弹窗model
      $scope.creatTypeModal = {
          title: "",
          classAddPa: function () { },
          classAddSon: function () { },
          classEdit: function () { },
          classDel: function () { },
          classTreedata: [],
          classTreeSel: {},
          onClaSelected: function () { },
          html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
              '<div class="form-group">' +
'                <div class="popwin-top" style="padding: 0 10px 10px 10px; text-align: center;">' +
'                    <div class="btn-group" style="margin-bottom: 15px;">' +
'                        <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classAddPa()" translate="setting.class.newParent">' +
'                            新增父节点' +
'                        </a>' +
'                        <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classAddSon()" translate="setting.class.newChild">' +
'                            新增子节点' +
'                        </a>' +
'                        <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classEdit()" translate="setting.edit">' +
'                            编辑' +
'                        </a>' +
'                        <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classDel()" translate="setting.delete">' +
'                            删除' +
'                        </a>' +
'                    </div>' +
'                </div>' +
'                <div class="popwin-body" style="padding: 0 10px; max-height: 300px; overflow-y: auto;">' +
'                    <div style="border-top: solid 1px rgba(0, 0, 0, 0.07);"></div>' +
'                    <abn-tree class="font-title-btn" tree-data="popwinmodal.classTreedata" initial-selection="{{popwinmodal.classTreedata[0].label}}" selected-data="popwinmodal.classTreeSel" icon-leaf="" on-select="popwinmodal.onClaSelected" expand-level="3"></abn-tree>' +
'                    <div style="border-top: solid 1px rgba(0, 0, 0, 0.07);"></div>' +
'                </div>' +
'            </div>' +
'        <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
'            <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.sure">确定</button>' +
'        </div>' +
'    </form>'
      }
      //初始化分类树
      $scope.initCreatType = function () {
          $scope.creatTypeModal.classTreedata = [];
          dataType.getAllListByDataType(dataTypeID).success(function (data, statues) {
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
                      $scope.creatTypeModal.classTreedata.push(each);
                  }
              });
              $scope.creatTypeModal.classTreedata.forEach(function (each) {
                  each.children = [];
                  arr.forEach(function (item) {
                      if (item.parentID == each.id) {
                          each.children.push(item);
                      }
                  })
              });
              $scope.typeTreeData[0].children = angular.copy($scope.creatTypeModal.classTreedata);
          });
      }
      //打开
      $scope.createTypeOpened = function () {
          //console.log('创建分类打开');
      }
      //提交
      $scope.createTypeSubmit = function (modalInstance, form) {
          //console.log($scope.creatTypeModal);
          $rootScope.loginOut();

          getTypeTree();
          
          $timeout(function () {
              $scope.inputModalType.typePullDownTreeData = JSON.parse(JSON.stringify($scope.typeTreeData[0].children));
              $scope.inputModal.typePullDownTreeData = JSON.parse(JSON.stringify($scope.typeTreeData[0].children));
          }, 500);

          //console.log($scope.inputModalType.typePullDownTreeData);
          //关闭弹出框
          modalInstance.close();
      }
      //取消
      $scope.createTypeCancel = function () { }

      //新增父节点
      $scope.nodeType = '';
      $scope.creatTypeModal.classAddPa = function () {
          $scope.openInputTextModal.typeName = '';
          $scope.openInputTextModal.title = $filter('translate')('setting.class.newParent');
          $scope.openInputText();
          $scope.nodeType = 'parent';
          getNodeInputFocus("openInputTextCls");
      }
      //新增子节点
      $scope.creatTypeModal.classAddSon = function () {
          $scope.nodeType = 'children';
          if (!$scope.creatTypeModal.classTreeSel.id) {
              alertFun($filter('translate')('views.Layer.alertFun.set.addNodePrompt1'), "", 'warning', '#007AFF');
          } else {
              if ($scope.creatTypeModal.classTreeSel.parentID != "0") {
                  alertFun($filter('translate')('views.Layer.alertFun.set.addNodePrompt2'), "", 'warning', '#007AFF');
              }
              else {
                  $scope.openInputTextModal.typeName = '';
                  $scope.openInputTextModal.title = $filter('translate')('setting.class.newChild');
                  $scope.openInputText();
                  getNodeInputFocus("openInputTextCls");
              }
          }
      }

      //编辑
      $scope.creatTypeModal.classEdit = function () {
          $scope.nodeType = 'edit';
          if (!$scope.creatTypeModal.classTreeSel.id) {
              alertFun($filter('translate')('views.Layer.alertFun.set.editNodePrompt1'), "", 'warning', '#007AFF');
          }
          else {
              $scope.openInputTextModal.typeName = $scope.creatTypeModal.classTreeSel.label;
              $scope.openInputTextModal.title = $filter('translate')('setting.class.editNode');
              $scope.openInputText();
              getNodeInputFocus("openInputTextCls");
          }
      }
      //删除
      $scope.creatTypeModal.classDel = function () {
          $scope.nodeType = 'delete';
          if (!$scope.creatTypeModal.classTreeSel.id) {
              alertFun($filter('translate')('views.Layer.alertFun.set.delNodePrompt1'), "", 'warning', '#007AFF');
          }
          else if ($scope.creatTypeModal.classTreeSel.id == $scope.currentType) {
              alertFun($filter('translate')('views.Layer.alertFun.set.delNodePrompt2'), "", 'warning', '#007AFF');
          }
          else {
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
                          $scope.initCreatType();
                          $scope.creatTypeModal.classTreeSel = {};
                      });
                  }
              });
          }
      }

      //选中树节点执行方法
      $scope.creatTypeModal.onClaSelected = function (node, getParentNodeBackFun) {
          //console.log("当前节点", node);
          var pnode = getParentNodeBackFun(node);
          //console.log("当前节点的父节点", pnode);
      }

      //填写分类弹窗model
      $scope.openInputTextModal = {
          typeName: '',
          title: '',
          submit: function (a) {
              $rootScope.loginOut();
              if (!$.trim($scope.openInputTextModal.typeName)) {
                  alertFun($filter('translate')('views.Layer.alertFun.set.inputContent'), "", 'warning', '#007AFF');
                  return;
              }
              dataType.getDetailByName($scope.openInputTextModal.typeName, dataTypeID).success(function (data, statues) {
                  if (!data) return;
                  if (data.items.length === 0) {
                      if ($scope.nodeType == 'parent') {
                          var data = { TypeName: $scope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: dataTypeID, ParentID: "0" }
                          //console.log(data);
                          dataType.insert(data).success(function (data, status) {
                              //console.log(data);
                              a.close();
                              $scope.initCreatType();
                          });
                      } else if ($scope.nodeType == 'children') {
                          var data = { TypeName: $scope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: dataTypeID, ParentID: $scope.creatTypeModal.classTreeSel.id }
                          //console.log(data);
                          dataType.insert(data).success(function (data, status) {
                              //console.log(data);
                              a.close();
                              $scope.initCreatType();
                          })
                      } else if ($scope.nodeType == 'edit') {
                          var data = { Id: $scope.creatTypeModal.classTreeSel.id, TypeName: $scope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: dataTypeID, ParentID: $scope.creatTypeModal.classTreeSel.parentID }
                          dataType.update(data).success(function (data, status) {
                              //console.log(data);
                              a.close();
                              $scope.initCreatType();
                          });
                      }
                      $scope.openInputTextModal.typeName = [];
                      $scope.creatTypeModal.classTreeSel = {};
                  }
                  else {
                      alertFun($filter('translate')('views.Layer.alertFun.set.addClassPrompt'), "", 'warning', '#007AFF');
                  }
              });
          },
          open: function () { },
          cancel: function () { },
          html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            '    <div class="form-group">' +
            '        <div style="padding-left: 25px;">' +
            '            <label class="col-sm-2 font-title-little" style="padding-top: 8px; padding-left: 0;">' +
            '                <span translate="setting.class.inputNode">输入节点</span><span class="symbol required"></span>' +
            '            </label>' +
            '            <input class="col-sm-9" type="text" ng-model="popwinmodal.typeName" style="height: 34px;" />' +
            '            <div style="clear: both;"></div>' +
            '        </div>' +
            '    </div>' +
            '    <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '        <button class="btn btn-wide btn-primary font-title-btn" type="submit" translate="setting.submit" style="min-width: 80px; float: right; margin-left: 0.5em;">提交</button>' +
            '        <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" translate="setting.cancel" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()">取消</a>' +
            '    </div>' +
            '</form>'
      }

      /*--------------创建分类弹窗--------------end----*/


      /*--------------新增字段弹窗--------------start----*/
      var createTdModelPub = {
          newTdTitle: '',
          //字段名称
          firstname: "",
          //这一行td的id
          tdId: '',
          notAllowedString: '~`!@#$%^&*()+=.\\|/?.,<>[]{}:;"\'~·！@#￥%……&*（）——-【】、“”‘’：；《》，。？',

          //需要填入的文本信息
          tableHeadData: [],

          //数据类型下拉框
          tdDataTypeData: [],
          tdDataTypeSel: {},
          //数据类型下拉框的选中事件
          onSelDataType: function () {
              //console.log(this.tdDataTypeSel);
              if (!!this.tdDataTypeSel.selected.codeName && this.tdDataTypeSel.selected.codeName !== '时间型') {
                  this.readOnly1 = false;
                  this.isTimeType = false;

                  if (this.tdDataTypeSel.selected.codeName == '长整型') {
                      this.length = 10;
                      this.precision = '';
                      this.readOnly2 = true;
                      this.readOnly3 = false;
                  }
                  if (this.tdDataTypeSel.selected.codeName == '短整型') {
                      this.length = 5;
                      this.precision = '';
                      this.readOnly2 = true;
                      this.readOnly3 = false;
                  }
                  if (this.tdDataTypeSel.selected.codeName == '单浮点型') {
                      this.length = 0;
                      this.precision = 0;
                      this.readOnly2 = false;
                      this.readOnly3 = false;
                  }
                  if (this.tdDataTypeSel.selected.codeName == '双浮点型') {
                      this.length = 0;
                      this.precision = 0;
                      this.readOnly2 = false;
                      this.readOnly3 = false;
                  }
                  if (this.tdDataTypeSel.selected.codeName == '字符型') {
                      this.length = 50;
                      this.precision = '';
                      this.readOnly2 = true;
                      this.readOnly3 = true;
                  }
              }
              else {
                  this.readOnly1 = true;
                  this.readOnly2 = true;
                  this.readOnly3 = true;
                  this.isTimeType = true;
                  this.length = '';
                  this.precision = '';
              }
          },
          //数据类型为时间类型
          isTimeType: true,
          //长度
          length: '',
          readOnly1: true,
          //小数位
          precision: '',
          readOnly2: true,
          //值域上下限
          readOnly3:true,
          //单位
          unit: '',
          //缺省值
          defaultVal: '',

          //输入控制下拉框
          tdInputCtrlData: [{ label: " ", value: '' }, { label: $filter('translate')('views.Layer.create.newField.control.required'), value: 'T' }, { label: $filter('translate')('views.Layer.create.newField.control.optional'), value: 'F' }],
          tdInputCtrlSel: {},
          //输入控制下拉框的选中事件
          onSelInputCtrl: function () {
              //console.log(this.tdInputCtrlSel);
              if (this.tdInputCtrlSel.selected.value === 'T') {
                  this.empty = 'F';
              }
              else {
                  this.empty = 'T';
              }
          },
          //值域上限
          inputMax: '',
          //值域下限
          inputMin: '',

          //数据分类代码
          dataTypeCode: '',
          dataTypeCodeChange: function () {
              if (this.dataTypeCode != 'T') {
                  this.tdInputFormatSel.selected = {};
              }
          },
          createDict: function (data, num) {
              //console.log(data);

              if (num == 1) {
                  $scope.dictManagerModal.title = $filter('translate')('views.Layer.dicWin.manage');
                  $scope.dictManagerModal.isEdit = true;
              }
              else {
                  $scope.dictManagerModal.title = $filter('translate')('views.Layer.dicWin.see');
                  $scope.dictManagerModal.isEdit = false;
              }
              $scope.dictManagerModal.dictData = angular.copy(data);
              $scope.dictManagerModal.firstname = this.firstname;
              $scope.openDictManager();
          },

          //输入格式下拉框
          tdInputFormatData: [{ label: " ", value: '' }, { label: $filter('translate')('views.Layer.create.newField.format.radio'), value: 'S' }, { label: $filter('translate')('views.Layer.create.newField.format.multiple'), value: 'M' }],
          tdInputFormatSel: {},
          //输入格式下拉框的选中事件
          onSelInputFormat: function () {
              //console.log(this.tdInputFormatSel);
          },
          //空值
          isNull: '',
          //文字值连接代码
          valueLink: '',
          //显示在页面的计算公式
          formula: "",
          //实际提交的计算公式
          realFormula: "",
          createCalComp: function () {
              $scope.calCompModel.title = $filter('translate')('views.Layer.formulaWin.MAIN');
              $scope.calCompModel.fieldName = angular.copy(this.firstname);
              $scope.calCompModel.fieldData = [].concat({ id: 0, value: this.firstname, type: this.tdDataTypeSel.selected.codeName });
              //console.log($scope.inputModal.tableData);
              if ($scope.inputModal.tableData.length > 0) {
                  for (var i = 0; i < $scope.inputModal.tableData.length; i++) {
                      if (this.firstname !== $scope.inputModal.tableData[i].name && $scope.inputModal.tableData[i].dataType !== "时间型") {
                          $scope.calCompModel.fieldData = $scope.calCompModel.fieldData.concat({ id: i + 1, value: $scope.inputModal.tableData[i].name, type: $scope.inputModal.tableData[i].dataType });
                      }
                  }
              }
              
              $scope.calCompModel.calFuncData = [];
              $scope.calCompModel.checkType = 'number';
              $scope.calCompModel.changeType(1);
              $scope.openCalCompFun();
          },
          //数据源
          dataSource: '',
          //说明
          remark: '',
          dictData: []
      };
      /*--------------新增字段弹窗--------------end----*/


      /*--------------字典项管理弹窗--------------start----*/
      $scope.dictManagerModal = {
          title: '',
          isEdit: false,
          firstname: '',
          addDict: function () {
              $scope.createDictModel.title = $filter('translate')('views.Layer.dicWin.newDic');
              $scope.createDictModel.id = '';
              $scope.createDictModel.text = '';
              $scope.createDictModel.remark = '';
              $scope.openCreateDictFun();
          },
          //一条字段的字典项
          dictData: [],
          //所有字典项
          allDictData: [],
          edit: function (tr) {
              $scope.createDictModel.title = $filter('translate')('views.Layer.dicWin.editDic');
              $scope.createDictModel.id = tr.id;
              $scope.createDictModel.text = tr.text;
              $scope.createDictModel.remark = tr.remark;
              $scope.openCreateDictFun();
          },
          del: function (id) {
              //console.log(id);
              for (var i = 0; i < $scope.dictManagerModal.dictData.length; i++) {
                  if ($scope.dictManagerModal.dictData[i].id === id) {
                      $scope.dictManagerModal.dictData.splice(i, 1);
                  }
              }
          },

          dictManagerOpened: function () {},
          submit: function (a) {
              $scope.inputModal.createTdModal.dictData = angular.copy($scope.dictManagerModal.dictData);
              a.close();
          },
          cancel: function () {},
          html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">\r\n' +
              '<div class="col-sm-12">\r\n' +
              '    <div class="panel panel-white panel-bottom0">\r\n' +
              '        <div class="panel-heading border-bottom" ng-if="!popwinmodal.isEdit">\r\n' +
              '            <span class="font-title-middle" translate="views.Layer.dicWin.data">字典数据</span><span class="font-title-middle">---{{popwinmodal.firstname}}</span>\r\n' +
              '        </div>\r\n' +
              '        <div class="panel-body panel-fixed" style="height: 470px;">\r\n' +
              '            <div style="margin-bottom: 10px;">\r\n' +
              '                <a class="btn btn-wide btn-default btn-o font-title-btn" href="javascript:;" ng-click="popwinmodal.addDict()" ng-if="!!popwinmodal.isEdit" translate="views.Layer.dicWin.new">\r\n' +
              '                    新增\r\n' +
              '                </a>\r\n' +
              '            </div>\r\n' +
              '            <div>\r\n' +
              '                <table class="table table-bordered font-content-small">\r\n' +
              '                    <thead>\r\n' +
              '                        <tr>\r\n' +
              '                            <th style="text-align: center; width: 55px;" translate="topbar.notification.serialnumber">序号</th>\r\n' +
              '                            <th style="text-align: center;" translate="views.Layer.dicWin.content">字典项内容</th>\r\n' +
              '                            <th style="text-align: center;" translate="views.Layer.dicWin.description">字典项说明</th>\r\n' +
              '                            <th style="text-align: center;" ng-if="!!popwinmodal.isEdit" translate="setting.operating">操作</th>\r\n' +
              '                        </tr>\r\n' +
              '                    </thead>\r\n' +
              '                    <tbody>\r\n' +
              '                        <tr ng-repeat="tr in popwinmodal.dictData">\r\n' +
              '                            <td style="text-align: center;">{{$index+1}}</td>\r\n' +
              '                            <td style="text-align: center;">{{tr.text}}</td>\r\n' +
              '                            <td style="text-align: center;">{{tr.remark}}</td>\r\n' +
              '                            <td style="text-align: center;" ng-if="!!popwinmodal.isEdit">\r\n' +
              '                                <a href="javascript:;" ng-click="popwinmodal.edit(tr)" style="border-right: solid 1px #000; padding-right: 4px;" translate="setting.edit">编辑</a>\r\n' +
              '                                <a href="javascript:;" ng-click="popwinmodal.del(tr.id)" translate="setting.delete">删除</a>\r\n' +
              '                            </td>\r\n' +
              '                        </tr>\r\n' +
              '                    </tbody>\r\n' +
              '                </table>\r\n' +
              '            </div>\r\n' +
              '        </div>\r\n' +
              '    </div>\r\n' +
              '</div>\r\n' +
              '<div style="clear: both;"></div>\r\n' +
              '<div class="form-group" ng-if="!!popwinmodal.isEdit" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
              '    <button class="btn btn-wide btn-primary font-title-btn" type="submit" translate="setting.sure" style="min-width: 80px; float: right; margin-left: 0.5em;">确定</button>' +
              '    <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" ng-click="submitForm.cancel()" translate="setting.cancel" style="float: right; min-width: 80px;">取消</a>' +
              '</div>' +
              '</form>'
      };
      /*--------------字典项管理弹窗--------------end----*/


      /*--------------新增字典项弹窗--------------start----*/
      $scope.createDictModel = {
          title: '',
          id: '',
          text: '',
          remark: '',

          openCreateDict: function () { },
          createDictForm: function (modalInstance, form) {
              if (!$scope.createDictModel.text) {
                  alertFun($filter('translate')('views.Layer.alertFun.set.inputDicContent'), "", 'warning', '#007AFF');
                  return;
              }
              for (var j = 0; j < $scope.dictManagerModal.dictData.length; j++) {
                  var tmpObj = $scope.dictManagerModal.dictData[j];
                  if (!!$scope.createDictModel.id) {
                      if ($scope.dictManagerModal.dictData[j].text === $scope.createDictModel.text && $scope.createDictModel.text !== $scope.createDictModel.lastText) {
                          alertFun($filter('translate')('views.Layer.alertFun.set.addDicPrompt1'), "", 'warning', '#007AFF');
                          return;
                      }
                  }
                  else {
                      if ($scope.dictManagerModal.dictData[j].text === $scope.createDictModel.text) {
                          alertFun($filter('translate')('views.Layer.alertFun.set.addDicPrompt1'), "", 'warning', '#007AFF');
                          return;
                      }
                  }
              }
              if (!!$scope.createDictModel.id) {
                  for (var i = 0; i < $scope.dictManagerModal.dictData.length; i++) {
                      if ($scope.dictManagerModal.dictData[i].id === $scope.createDictModel.id) {
                          $scope.dictManagerModal.dictData[i] = angular.copy({ id: $scope.createDictModel.id, text: $scope.createDictModel.text, remark: $scope.createDictModel.remark });
                          break;
                      }
                  }
              }
              else {
                  $scope.dictManagerModal.dictData = $scope.dictManagerModal.dictData.concat({ id: getRandom(), text: $scope.createDictModel.text, remark: $scope.createDictModel.remark })
              }
              //关闭弹出框
              modalInstance.close();
          },
          cancelCreateDict: function () { }
      };
      /*--------------新增字典项弹窗--------------end----*/

      /*--------------创建计算公式弹窗--------------start----*/
      $scope.calCompModel = {
          title: "",
          fieldData: [],
          checkType: '',
          calFuncData: [],
          //当前添加的这条字段的名称
          fieldName: '',
          //输入的计算公式值
          calFormula: '',
          //公式是否检验通过
          isChecked: false,
          //改变数据类型
          changeType: function (num) {
              if (num === 1) {
                  $scope.calCompModel.calFuncData = [{ id: 1, value: "Math.Max()", label: "Max(a,b)", remark: $filter('translate')('views.Layer.formulaWin.function.max') },
                      { id: 2, value: "Math.Min()", label: "Min(a,b)", remark: $filter('translate')('views.Layer.formulaWin.function.min') },
                      { id: 3, value: "Math.Abs()", label: "Abs(a)", remark: $filter('translate')('views.Layer.formulaWin.function.abs') },
                      { id: 4, value: "Math.Ceiling()", label: "Ceiling(a)", remark: $filter('translate')('views.Layer.formulaWin.function.ceiling') },
                      { id: 5, value: "Math.Floor()", label: "Floor(a)", remark: $filter('translate')('views.Layer.formulaWin.function.floor') }
                  ];
              }
              if (num === 2) {
                  $scope.calCompModel.calFuncData = [{ id: 1, value: "Substring()", label: "Substring(str1,str2)", remark: $filter('translate')('views.Layer.formulaWin.function.substring') },
                      { id: 2, value: "Replace()", label: "Replace(str1,str2)", remark: $filter('translate')('views.Layer.formulaWin.function.replace') }
                  ];
              }
          },
          //点击字段列表
          clickField: function (tr) {
              //console.log(tr);
              var posi = getCursortPosition($('.calcomp-content-textarea').get(0));
              insertAfterText($('.calcomp-content-textarea').get(0), tr.value);
              $scope.calCompModel.calFormula = $('.calcomp-content-textarea').val();
              $scope.calCompModel.isChecked = false;
          },
          //点击函数列表
          clickFunc: function (tr) {
              //console.log(tr);
              var posi = getCursortPosition($('.calcomp-content-textarea').get(0));
              insertAfterText($('.calcomp-content-textarea').get(0), tr.value);
              var setPosi = posi + tr.value.length - 1;
              setCaretPosition($('.calcomp-content-textarea').get(0), setPosi);

              $scope.calCompModel.calFormula = $('.calcomp-content-textarea').val();
              $scope.calCompModel.isChecked = false;
          },
          //点击符号按钮
          clickSymbol: function (str) {
              var posi = getCursortPosition($('.calcomp-content-textarea').get(0));
              insertAfterText($('.calcomp-content-textarea').get(0), str);
              $scope.calCompModel.calFormula = $('.calcomp-content-textarea').val();
              $scope.calCompModel.isChecked = false;
          },
          //手动向输入框中输入
          changeTextarea: function () {
              $scope.calCompModel.isChecked = false;
          },
          
          //公式校验
          check: function () {
              //console.log($scope.calCompModel.fieldData);
              waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
              var calFormulas = angular.copy($scope.calCompModel.calFormula);
              for (var c = 0; c < $scope.calCompModel.fieldData.length; c++) {
                  if ($scope.calCompModel.fieldData[c].type === "字符型") {
                      calFormulas = calFormulas.split($scope.calCompModel.fieldData[c].value).join("\"abcdefg\"");
                  }
                  else {
                      calFormulas = calFormulas.split($scope.calCompModel.fieldData[c].value).join("11");
                  }
              }
              calFormulas = encodeURI(calFormulas);
              calFormulas = calFormulas.replace(/\+/g, '%#');

              layerField.checkCalComp(calFormulas).success(function (data, status) {
                  //console.log(data, status);
                  waitmask.onHideMask();
                  if (data) {
                      alertFun($filter('translate')('views.Layer.alertFun.set.checkSucc'), "", 'success', '#007AFF');
                      $scope.calCompModel.isChecked = true;
                  }
                  else {
                      alertFun($filter('translate')('views.Layer.alertFun.set.checkErr'), "", 'warning', '#007AFF');
                  }
              }).error(function (data, status) {
                  console.log(data, status);
                  waitmask.onHideMask();
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });
          },
          //清空公式内容
          clear: function () {
              $scope.calCompModel.calFormula = "";
          },

          openCalComp: function () { },
          calCompForm: function (modalInstance, form) {
              if (!$scope.calCompModel.isChecked) {
                  alertFun($filter('translate')('views.Layer.alertFun.set.formulaCheckErr'), "", 'warning', '#007AFF');
                  return;
              }

              var calFormulas = angular.copy($scope.calCompModel.calFormula);
              for (var c = 0; c < $scope.calCompModel.fieldData.length; c++) {
                  calFormulas = calFormulas.split($scope.calCompModel.fieldData[c].value).join("{" + $scope.calCompModel.fieldData[c].value + "}");
              }
              
              console.log(calFormulas);
              $scope.inputModal.createTdModal.realFormula = angular.copy(calFormulas);
              $scope.inputModal.createTdModal.formula = angular.copy($scope.calCompModel.calFormula);
              //关闭弹出框
              modalInstance.close();
          },
          cancelCalComp: function () { }
      };
      
      /**
        * 在光标后插入文本
        *  textDom [JavaScript DOM String] 当前对象
        *  value [String] 要插入的文本
        */
      function insertAfterText(textDom, value) {
          var selectRange;
          if (document.selection) {
              // IE Support
              textDom.focus();
              selectRange = document.selection.createRange();
              selectRange.text = value;
              textDom.focus();
          } else if (textDom.selectionStart || textDom.selectionStart == '0') {
              // Firefox support
              var startPos = textDom.selectionStart;
              var endPos = textDom.selectionEnd;
              var scrollTop = textDom.scrollTop;
              textDom.value = textDom.value.substring(0, startPos) + value + textDom.value.substring(endPos, textDom.value.length);
              textDom.focus();
              textDom.selectionStart = startPos + value.length;
              textDom.selectionEnd = startPos + value.length;
              textDom.scrollTop = scrollTop;
          }
          else {
              textDom.value += value;
              textDom.focus();
          }
      }

      // 获取光标位置
      function getCursortPosition(textDom) {
          var cursorPos = 0;
          if (document.selection) {
              // IE Support
              textDom.focus();
              var selectRange = document.selection.createRange();
              selectRange.moveStart('character', -textDom.value.length);
              cursorPos = selectRange.text.length;
          } else if (textDom.selectionStart || textDom.selectionStart == '0') {
              // Firefox support
              cursorPos = textDom.selectionStart;
          }
          return cursorPos;
      }

      // 设置光标位置
      function setCaretPosition(textDom, pos) {
          if (textDom.setSelectionRange) {
              // IE Support
              textDom.focus();
              textDom.setSelectionRange(pos, pos);
          } else if (textDom.createTextRange) {
              // Firefox support
              var range = textDom.createTextRange();
              range.collapse(true);
              range.moveEnd('character', pos);
              range.moveStart('character', pos);
              range.select();
          }
      }

      /*--------------创建计算公式弹窗--------------start----*/


      //根据条件查询图层数据
      function getLayerDataBy(obj) {
          $scope.layerData = [];
          layerContent.getAllListByName(obj).success(function (data, statues) {
              //console.log(data);
              $scope.layerData = angular.copy(data.items);
              for (var i in $scope.layerData) {
                  $scope.layerData[i].imgUrl = '/Thumbnail/layer/' + $scope.layerData[i].layerAttrTable + '.png';
              }
          });
      }
      //新增图层数据
      function addLayerData(obj) {
          layerContent.insert(obj).success(function (data, statues) {
              getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
          });
      }
      //分页查询
      function getPageList(layerName, layerType, layerTag, pageIndex, pageSize) {
          //console.log(layerName);
          //console.log(layerType);
          //console.log(layerTag);
          $scope.layerData = [];
          layerContent.getPageListByName({ LayerName: layerName, LayerType: layerType, LayerTag: layerTag, CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }, pageSize, pageIndex).success(function (data, status) {
              //console.log(data);
              $scope.totalPages = Math.ceil(data.totalCount / $scope.pageSize);
              $scope.pageCounts = data.totalCount;
              $scope.layerData = angular.copy(data.items);
              for (var i in $scope.layerData) {
                  //$scope.layerData[i].imgUrl = GeoServerUrl + '/wms?service=WMS&version=1.1.0&request=GetMap&layers=' + WorkSpace + ':' +
                  //    $scope.layerData[i].layerAttrTable + '&styles=&bbox=' + $scope.layerData[i].minX + ',' + $scope.layerData[i].minY + ',' + $scope.layerData[i].maxX + ',' + $scope.layerData[i].maxY +
                  //    '&width=768&height=670&srs=EPSG:4326&format=image%2Fvnd.jpeg-png';
                  $scope.layerData[i].imgUrl = '/Thumbnail/layer/' + $scope.layerData[i].layerAttrTable + '.png';
              }
          });
      }
      //分类树
      function getTypeTree() {
          $scope.typeTreeData[0].children = [];
          //分类查询树的数据
          dataType.getAllListByDataType(dataTypeID).success(function (data, statues) {
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
      }
      //标签树
      function getTagTree() {
          //标签查询树的数据
          dataTag.getAllListByDataType(dataTypeID).success(function (data, statues) {
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
      }

      //清空图层数据
      $scope.clearLayerData = function (id) {
          //console.log(id);
          SweetAlert.swal({
              title: $filter('translate')('views.Layer.alertFun.field.confirmClearLayer'),
              text: $filter('translate')('views.Layer.alertFun.field.confirmClearLayerPrompt'),
              type: "warning",
              showCancelButton: true,
              confirmButtonColor: "#DD6B55",
              confirmButtonText: $filter('translate')('setting.sure'),
              cancelButtonText: $filter('translate')('setting.cancel')
          }, function (isConfirm) {
              if (isConfirm) {
                  waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                  layerContent.clear(id).success(function (data, statues) {
                      //console.log(data);
                      if (data) {
                          getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                          waitmask.onHideMask();
                          alertFun($filter('translate')('views.Layer.alertFun.field.layerClearSucc'), "", 'success', '#007AFF');
                      }
                      else {
                          waitmask.onHideMask();
                          alertFun($filter('translate')('views.Layer.alertFun.field.layerClearErr'), "", 'error', '#007AFF');
                      }
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              }
          });
      }
      //删除图层数据
      $scope.delLayerData = function (id) {
          //console.log(id);
          SweetAlert.swal({
              title: $filter('translate')('views.Layer.alertFun.field.confirmDelLayer'),
              text: $filter('translate')('views.Layer.alertFun.field.confirmDelLayerPrompt'),
              type: "warning",
              showCancelButton: true,
              confirmButtonColor: "#DD6B55",
              confirmButtonText: $filter('translate')('setting.sure'),
              cancelButtonText: $filter('translate')('setting.cancel')
          }, function (isConfirm) {
              if (isConfirm) {
                  waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                  mapReleation.getAllListBylayerID(id).success(function (data, statues) {
                      //console.log(data);
                      if (!data.items[0]) {
                          tagReleation.deleteByMapID(id).success(function (data, statues) {
                              //console.log(data);
                              layerField.deleteByLayerID(id).success(function (data, statues) {
                                  //console.log(data);
                                  layerContent.delete(id).success(function (data, statues) {
                                      //console.log(data);
                                      layerContent.deleteLayer(id).success(function (data, statues) {
                                          getPageList($scope.searchCondition.LayerName, $scope.searchCondition.LayerType, $scope.searchCondition.LayerTag, $scope.pageIndex, $scope.pageSize);
                                          if (data) {
                                              waitmask.onHideMask();
                                              alertFun($filter('translate')('views.Layer.alertFun.field.layerDelSucc'), "", 'success', '#007AFF');
                                          }
                                          else {
                                              waitmask.onHideMask();
                                              alertFun($filter('translate')('views.Layer.alertFun.field.layerDelSucc'), "", 'error', '#007AFF');
                                          }
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
                          }).error(function (data, status) {
                              waitmask.onHideMask();
                              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                          });
                      }
                      else {
                          var tmpMapName = "";
                          for (var i in data.items) {
                              tmpMapName += data.items[i].mapName;
                              if (i < data.items.length - 1) {
                                  tmpMapName += "、";
                              }
                          }
                          waitmask.onHideMask();
                          alertFun($filter('translate')('setting.delErr'), $filter('translate')('views.Layer.alertFun.field.layerDelErrPrompt') + tmpMapName, 'error', '#007AFF');
                      }
                  }).error(function (data, status) {
                      waitmask.onHideMask();
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              }
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

      //输入节点的input框获取焦点
      function getNodeInputFocus(classname) {
          $timeout(function () {
              do {
                  $("." + classname + " .font-title-little input").focus();
              }
              while (!$("." + classname)[0]);
          }, 200);
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
      
      //计算对象的长度
      function countObj(obj) {
          var count = 0;
          for (var i in obj) {
              count++;
          }
          return count;
      }

      //比较新增一行的长度
      function compareLength(length, lenCom, confirm) {
          if (length > lenCom) {
              alertFun(confirm + $filter('translate')('views.Layer.alertFun.field.tooLong'), "", 'warning', '#007AFF');
              return false;
          }
          return true;
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

      function alertFun(title, text, type, color) {
          SweetAlert.swal({
              title: title,
              text: text,
              type: type,
              confirmButtonColor: color
          });
      }

      function contains(arr, obj) {
          var i = arr.length;
          while (i--) {
              if (arr[i] === obj) {
                  return true
              }
          }
          return false;
      }

      getTypeTree();
      getTagTree();

      $scope.addLayerHtml = '    <form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
'<div class="form-group">\r\n' +
'    <div>\r\n' +
'        <div class="popwin-top" style="padding: 0 10px 15px 10px;">\r\n' +
'            <div class="" style="margin-bottom: 10px;">\r\n' +
'                <div class="col-sm-6">\r\n' +
'                    <div style="float:right; width: 100%;">\r\n' +
'                        <div class="col-sm-3">\r\n' +
'                            <label class="font-title-little" style="padding-top: 6px;"><span translate="views.Layer.create.name">图层名称</span><span class="symbol required"></span></label>\r\n' +
'                        </div>\r\n' +
'                        <div class="col-sm-9" style="padding-right: 14px; padding-left: 0;">\r\n' +
'                            <input type="text" style="width: 100%; height: 34px;" ng-model="popwinmodal.layerName" />\r\n' +
'                        </div>\r\n' +
'                    </div>\r\n' +
'                </div>\r\n' +
'                <div class="col-sm-6">\r\n' +
'                    <div style="float: right; padding-right: 10px; width: 100%;">\r\n' +
'                        <div class="col-sm-3">\r\n' +
'                            <label class="font-title-little" style="padding-top: 6px;"><span translate="views.Layer.create.type.MAIN">图层类型</span><span class="symbol required"></span></label>\r\n' +
'                        </div>\r\n' +
'                        <div class="col-sm-9" style="padding: 0;">\r\n' +
'                            <ui-select class="ui-select-width" ng-model="popwinmodal.layerTypeSel.selected" theme="select2" style="width: 100%;" >\r\n' +
'                                <ui-select-match>{{$select.selected.codeName}}</ui-select-match>\r\n' +
'                                <ui-select-choices class="ui-select-height" repeat="p in popwinmodal.layerTypeData| propsFilter: {codeName: $select.search}">\r\n' +
'                                    <div ng-bind-html="p.codeName | highlight: $select.search"></div>\r\n' +
'                                </ui-select-choices>\r\n' +
'                            </ui-select>\r\n' +
'                        </div>\r\n' +
'                    </div>\r\n' +
'                </div>\r\n' +
'                <div style="clear: both;"></div>\r\n' +
'            </div>\r\n' +
'            <div class="" style="margin-bottom: 10px;">\r\n' +
'                <div class="col-sm-6">\r\n' +
'                    <div style="float: right; width: 100%;">\r\n' +
'                        <div class="col-sm-3">\r\n' +
'                            <label class="font-title-little" style="padding-top: 6px;"><span translate="views.Layer.create.sr">空间参考</span><span class="symbol required"></span></label>\r\n' +
'                        </div>\r\n' +
'                        <div class="col-sm-9" style="padding-right: 14px; padding-left: 0;">\r\n' +
'                            <input type="text" ng-model="popwinmodal.spatialRefData" ng-disabled="true" style="width: 100%; height:34px;" readonly />\r\n' +
'                        </div>\r\n' +
'                    </div>\r\n' +
'                </div>\r\n' +
'                <div class="col-sm-6">\r\n' +
'                    <div style="float: right; padding-right: 10px; width: 100%;">\r\n' +
'                        <div class="col-sm-3">\r\n' +
'                            <label class="font-title-little" style="padding-top: 6px;"><span translate="setting.class.MAIN">分类</span><span class="symbol required"></span></label>\r\n' +
'                        </div>\r\n' +
'                        <div class="col-sm-9" style="padding: 0;">\r\n' +
'                            <abn-tree class="typePullDownTreeData" style="float: left; margin-right: 13px;" mic-combo-tree tree-data="popwinmodal.typePullDownTreeData"\r\n' +
'                                      icon-leaf="" selected-data="popwinmodal.typePullDownTreeSelData" on-select="popwinmodal.onComboSelected"\r\n' +
'                                      initial-selection="{{popwinmodal.initialTypeTreeData.label}}" expand-level="2">\r\n' +
'                            </abn-tree>\r\n' +
'                            <span class="btn btn-primary btn-o font-title-little" ng-click="popwinmodal.createClaOpenWinIn()" translate="setting.class.newClass">\r\n' +
'                                创建分类\r\n' +
'                            </span>\r\n' +
'                        </div>\r\n' +
'                    </div>\r\n' +
'                </div>\r\n' +
'                <div style="clear: both;"></div>\r\n' +
'            </div>\r\n' +
'            <div style="margin-bottom: 20px;">\r\n' +
'                <div class="col-sm-12">\r\n' +
'                    <div style="float: right; padding-right: 10px; padding-left: 0; width: 100%;">\r\n' +
'                        <div class="col-sm-1">\r\n' +
'                            <label class="font-title-little" style="padding-top: 6px;" translate="setting.tags.MAIN">标签</label>\r\n' +
'                        </div>\r\n' +
'                        <div class="col-sm-11" style="padding-left: 42px; padding-right: 15px;">\r\n' +
'                            <div style="border: solid 1px rgba(200, 199, 204, 0.62);" class="taginput col-sm-12">\r\n' +
'                                <span class="btn btn-default btn-xs datavalues ng-scope" ng-repeat="$item in popwinmodal.tagsList"\r\n' +
'                                      style="font-size: 13px; line-height: 1.35; margin-right: 4px; margin-bottom: 3px; padding: 2px 4px;">\r\n' +
'                                    <span class="close" style="line-height: 0.665432em;padding-left:4px;" ng-click="popwinmodal.delItems($item,$event)">×</span>\r\n' +
'                                    <span class="cs-placeholder mtl ng-binding">{{$item}}</span>\r\n' +
'                                </span>\r\n' +
'                                <input class="mytagsfocus" type="text" placeholder="{{\'setting.tags.tagPlacehold\'|translate}}"\r\n' +
'                                       ng-keydown="popwinmodal.keydown($event,popwinmodal.tagsInput)" ng-trim="true" ng-model="popwinmodal.tagsInput"\r\n' +
'                                       style="background-color: #FFF !important; height: 34px; border: none; padding: 0;width:63%;" />\r\n' +
'                            </div>\r\n' +
'                        </div>\r\n' +
'                    </div>\r\n' +
'                </div>\r\n' +
'                <div style="clear: both;"></div>\r\n' +
'            </div>\r\n' +
'            <div style="border-top: solid 1px rgba(0, 0, 0, 0.07);"></div>\r\n' +
'        </div>\r\n' +
'\r\n' +
'        <div class="popwin-body" style="padding: 0 20px;">\r\n' +
'            <div style="padding: 0 10px 10px 10px;">\r\n' +
'                <a class="btn btn-primary btn-o font-title-btn" ng-click="popwinmodal.createTdOpenWinIn()" ng-disabled="popwinmodal.createFromFile" translate="views.Layer.create.newField.create" style="float: left; margin-right: 20px;">\r\n' +
'                    新增字段\r\n' +
'                </a>\r\n' +
'                <a class="btn btn-primary btn-o font-title-btn" ng-click="popwinmodal.createFromShpWin()" ng-disabled="popwinmodal.createFromText||popwinmodal.createFromFile" translate="views.Layer.create.newFrom.MAIN" style="float: left;">\r\n' +
'                    从shp文件新建\r\n' +
'                </a>\r\n' +
'                <a class="btn btn-primary btn-o font-title-btn" ng-click="popwinmodal.clearTd()" translate="views.Layer.create.emptied" style="float: right; margin-right: 15px;">\r\n' +
'                    清空字段\r\n' +
'                </a>\r\n' +
'                <div style="clear: both;"></div>\r\n' +
'            </div>\r\n' +
'            <div class="" style="overflow: auto;">\r\n' +
'                <div style="width: 100%;">\r\n' +
'                    <mic-data-tables class="font-content-small" table-bordered tbody-height="200"\r\n' +
'                                    thead="{{\'views.Layer.create.newField.fieldName\'|translate}},{{\'views.Layer.create.newField.datatype.MAIN\'|translate}},{{\'views.Layer.create.newField.length\'|translate}},{{\'views.Layer.create.newField.decimal\'|translate}},{{\'views.Layer.create.newField.description\'|translate}}"\r\n' +
'                                    td-params="{\'name\':{\'type\':\'span\',\'isclick\':false},\'dataType\':{\'type\':\'span\',\'isclick\':false},\'length\':{\'type\':\'span\',\'isclick\':false},\'decimal\':{\'type\':\'span\',\'isclick\':false},\'describe\':{\'type\':\'span\',\'isclick\':false}}"\r\n' +
'                                    datasets="popwinmodal.tableData"\r\n' +
'                                    table-btn-html="[{\'name\':\'编辑\',\'text\':\'\',\'icon\':\'mdi-pencil\'},{\'name\':\'删除\',\'text\':\'\',\'icon\':\'mdi-window-close\'}]" table-btn-params="popwinmodal.tabBtnParams">\r\n' +
'                    </mic-data-tables>\r\n' +
'                </div>\r\n' +
'            </div>\r\n' +
'        </div>\r\n' +
'    </div>\r\n' +
'</div>'+
'        <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
'            <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" ng-disabled="popwinmodal.isDisabled" ng-if="!popwinmodal.isDisabled" translate="setting.submit">提交</button>' +
'            <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()" translate="setting.cancel">取消</a>' +
'        </div>' +
'    </form>';
  }]);

//设置默认图片
app.directive('errSrc', function () {
    return {
        link: function (scope, element, attrs) {
            element.bind('error', function () {
                //console.log(scope);
                //console.log(attrs);
                if (attrs.src != attrs.errSrc) {
                    if (attrs.errSrc === "点" || attrs.errSrc === "Point") {
                        attrs.$set('src', '/App/Main/assets/images/mapDian.png');
                    }
                    if (attrs.errSrc === "线" || attrs.errSrc === "Line") {
                        attrs.$set('src', '/App/Main/assets/images/mapXian.png');
                    }
                    if (attrs.errSrc === "面" || attrs.errSrc === "Polygon") {
                        attrs.$set('src', '/App/Main/assets/images/mapMian.png');
                    }
                    if (attrs.errSrc === "影像" || attrs.errSrc === "Image") {
                        attrs.$set('src', '/App/Main/assets/images/imageLayer.png');
                    }
                }
            });
        }
    }
});