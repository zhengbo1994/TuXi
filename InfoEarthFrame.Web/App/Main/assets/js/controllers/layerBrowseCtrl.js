'use strict';
/**
 * layerBrowseCtrl Controller
 */

app.controller('layerBrowseCtrl', ['$rootScope', '$scope', '$document', '$filter', 'SweetAlert', '$element', "$timeout", 'abp.services.app.layerContent', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.layerField', 'abp.services.app.dicDataCode', 'abp.services.app.attachment', 'abp.services.app.uploadFile', 'abp.services.app.tagReleation', 'abp.services.app.mapReleation', 'waitmask', 'abp.services.app.layerReadLog', 'abp.services.app.layerFieldDict', 'abp.services.app.dataStyle', 'abp.services.app.operateLog',
  function ($rootScope, $scope, $document, $filter, SweetAlert, $element, $timeout, layerContent, dataTag, dataType, layerField, dicDataCode, attachment, uploadFile, tagReleation, mapReleation, waitmask, layerReadLog, layerFieldDict, dataStyle, operateLog) {
      $rootScope.loginOut();
      $rootScope.homepageStyle = {};
      //把地图之外的所有元素隐藏掉
      $rootScope.app.layout.isNavbarFixed = false;
      $rootScope.app.isWholeScreen = true;

      //添加一行中数据类型列表查询ID
      var tdDataTypeID = "73160096-67a5-11e7-8eb2-005056bb1c7e";
      var styleTypeID = 'c755eeea-986d-11e7-90b1-005056bb1c7e';


      //listenToSocketIO("http://192.168.10.34:8890/SDMS_VALID");

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

      // 地图数据
      $scope.mapDataset = mapDataset;
      $scope.isLoadTianDiTu = parseInt(isLoadTianDiTu);
      //加载图层
      var layer;
      $scope.mapSerShow = function (item, type) {
          if (layer) {
              $scope.removeLayer(layer);
          }
          //WMS
          //layer = newLocalTilesByWMS(GeoServerUrl + '/wms', WorkSpace + ':' + item.mapEnName, 'image/png');
          //WMTS
          //layer = newLocalTilesByWMTS(GeoServerUrl + '/gwc/service/wmts', WorkSpace + ':' + item.layerAttrTable);
          
          //wfs
          if (type === "wfs") {
              var tmpurl = GeoServerUrl + "/wfs?service=WFS&version=1.1.0&request=GetFeature&typeNames=" + item.layerAttrTable + "&bbox=" + item.minX + "," + item.minY + "," + item.maxX + "," + item.maxY + ",EPSG:4214&srsName=EPSG:4214&outputFormat=application/json";
              var vectorSource = new ol.source.Vector({
                  format: new ol.format.GeoJSON(),
                  url: tmpurl,
                  strategy: ol.loadingstrategy.bbox
              });
              layer = new ol.layer.Vector({
                  source: vectorSource
              });
          }
          else {
              var format = 'image/png';
              layer = new ol.layer.Image({
                  source: new ol.source.ImageWMS({
                      ratio: 1,
                      url: GeoServerUrl + '/wms',
                      params: {
                          FORMAT: format,
                          VERSION: '1.1.0',
                          STYLES: item.layerDefaultStyleName ? item.layerDefaultStyleName : "",
                          //SLD_BODY: sld,
                          LAYERS: WorkSpace + ":" + item.layerAttrTable,
                      }
                  })
              });
          }
          
          layer.setZIndex(27);
          //layer.getSource().updateParams({ "_": new Date().getTime()+ Math.random() });
          var bounds = [];
          if (!!item.minX) {
              bounds = [item.minX, item.minY, item.maxX, item.maxY];//范围
          }
          else {
              bounds = [70, 0, 140, 60];//范围
          }
          $scope.map = $scope.addLayer(layer, bounds);
      }

      $scope.mapEditFun = {
          editStart: function () {
              $scope.mapSerShow($scope.layerParam, "wfs");
          },
          editEnd: function () {
              $scope.mapSerShow($scope.layerParam);
          },
          editSave: function () {
              console.log(1);
          },
      }

      $scope.activeShowBtn = 0;
      //打开图层浏览
      $scope.showLayerDetail = function () {
          getLayerField($scope.currentData.id);
          getLayerDataNum($scope.currentData.id);

          if ($scope.currentData.pagetype == "1") {
              $scope.layerParam.title = $filter('translate')('views.Layer.layerBro.checkrule');
              $scope.openAttrCheck();
          }
          else {
              $scope.activeShowBtn = 1;
          }
      }
      //关闭图层浏览窗口
      $scope.closeLayerWin = function () {
          $scope.activeShowBtn = 0;
      }


      /*--------------图层属性--------------start----*/
      //图层属性对象
      var layerDto = {
          Id: "", LayerName: "", DataType: "", LayerBBox: "", LayerType: "", LayerTag: "", LayerDesc: "", LayerAttrTable: "", LayerSpatialTable: "", LayerRefence: "",
          MaxX: "", MinX: "", MaxY: "", MinY: ""
      }

      $scope.layerParam = {
          title: "",
          //图层名称
          layerName: "",
          //图层类型
          dataType: "",
          dataTypeName: "",
          //空间参考
          spatialRefData: SpatialRefence,
          //分类
          layerTypeName: "",
          //图层对应的样式
          styleId: "",
          styleName: "",
          //标签
          tags: "",
          //表格
          tableData: [],
          tmpTableData: [],

          //编辑属性按钮
          tabBtnParam1: [{
              name: "属性检查规则", click: function (row, name, event) {
                  $scope.tdParamModal.newTdTitle = $filter('translate')('views.Layer.create.newField.edit');
                  dicDataCode.getDetailByTypeID(tdDataTypeID).success(function (data, statues) {
                      //console.log(data);
                      //console.log(row);

                      $scope.tdParamModal.tdId = row.tableId;
                      $scope.tdParamModal.firstname = row.name;
                      $scope.tdParamModal.tdDataTypeData = data.items;
                      for (var i in data.items) {
                          if (data.items[i].id === row.dataTypeID) {
                              $scope.tdParamModal.tdDataTypeSel.selected = data.items[i];
                              break;
                          }
                      }
                      $scope.tdParamModal.onSelDataType();

                      $scope.tdParamModal.length = row.length;
                      $scope.tdParamModal.precision = row.decimal;
                      $scope.tdParamModal.unit = row.unit;
                      $scope.tdParamModal.defaultVal = row.defaultVal;

                      if (row.inputCtrl == "T") {
                          $scope.tdParamModal.tdInputCtrlSel.selected = { label: $filter('translate')('views.Layer.create.newField.control.required'), value: 'T' };
                      } else if (row.inputCtrl == "F") {
                          $scope.tdParamModal.tdInputCtrlSel.selected = { label: $filter('translate')('views.Layer.create.newField.control.optional'), value: 'F' };
                      } else {
                          $scope.tdParamModal.tdInputCtrlSel.selected = { label: " ", value: '' };
                      }

                      $scope.tdParamModal.maxVal = row.maxVal;
                      $scope.tdParamModal.minVal = row.minVal;
                      $scope.tdParamModal.dataTypeCode = row.dataTypeCode;
                      $scope.tdParamModal.linkCode = row.linkCode;

                      if (row.inputFormat == "S") {
                          $scope.tdParamModal.tdInputFormatSel.selected = { label: $filter('translate')('views.Layer.create.newField.format.radio'), value: 'S' };
                      } else if (row.inputFormat == "M") {
                          $scope.tdParamModal.tdInputFormatSel.selected = { label: $filter('translate')('views.Layer.create.newField.format.multiple'), value: 'M' };
                      } else {
                          $scope.tdParamModal.tdInputFormatSel.selected = { label: " ", value: '' };
                      }

                      $scope.tdParamModal.dataSource = row.dataSource;
                      $scope.tdParamModal.formula = row.formula;
                      $scope.tdParamModal.empty = row.empty;
                      $scope.tdParamModal.describe = row.describe;
                      $scope.tdParamModal.dictData = row.dictData;

                      $scope.tdParamModal.submitText = $filter('translate')('setting.submit');
                      $timeout(function () { $scope.openTdFun(); }, 0);
                  });
              }
          }],
          //查看属性按钮
          tabBtnParam2: [{
              name: "详情", click: function (row, name, event) {
                  $scope.tdParamModal.newTdTitle = $filter('translate')('views.Layer.create.newField.detail');
                  dicDataCode.getDetailByTypeID(tdDataTypeID).success(function (data, statues) {
                      //console.log(data);
                      //console.log(row);

                      $scope.tdParamModal.tdId = row.tableId;
                      $scope.tdParamModal.firstname = row.name;
                      $scope.tdParamModal.tdDataTypeData = data.items;
                      for (var i in data.items) {
                          if (data.items[i].id === row.dataTypeID) {
                              $scope.tdParamModal.tdDataTypeSel.selected = data.items[i];
                              break;
                          }
                      }
                      $scope.tdParamModal.onSelDataType();

                      $scope.tdParamModal.length = row.length;
                      $scope.tdParamModal.precision = row.decimal;
                      $scope.tdParamModal.unit = row.unit;
                      $scope.tdParamModal.defaultVal = row.defaultVal;

                      if (row.inputCtrl == "T") {
                          $scope.tdParamModal.tdInputCtrlSel.selected = { label: $filter('translate')('views.Layer.create.newField.control.required'), value: 'T' };
                      } else if (row.inputCtrl == "F") {
                          $scope.tdParamModal.tdInputCtrlSel.selected = { label: $filter('translate')('views.Layer.create.newField.control.optional'), value: 'F' };
                      } else {
                          $scope.tdParamModal.tdInputCtrlSel.selected = { label: " ", value: '' };
                      }

                      $scope.tdParamModal.maxVal = row.maxVal;
                      $scope.tdParamModal.minVal = row.minVal;
                      $scope.tdParamModal.dataTypeCode = row.dataTypeCode;
                      $scope.tdParamModal.linkCode = row.linkCode;

                      if (row.inputFormat == "S") {
                          $scope.tdParamModal.tdInputFormatSel.selected = { label: $filter('translate')('views.Layer.create.newField.format.radio'), value: 'S' };
                      } else if (row.inputFormat == "M") {
                          $scope.tdParamModal.tdInputFormatSel.selected = { label: $filter('translate')('views.Layer.create.newField.format.multiple'), value: 'M' };
                      } else {
                          $scope.tdParamModal.tdInputFormatSel.selected = { label: " ", value: '' };
                      }

                      $scope.tdParamModal.dataSource = row.dataSource;
                      $scope.tdParamModal.formula = row.formula;
                      $scope.tdParamModal.empty = row.empty;
                      $scope.tdParamModal.describe = row.describe;
                      $scope.tdParamModal.dictData = row.dictData;
                      $scope.tdParamModal.submitText = "";
                      $timeout(function () { $scope.openTdFun(); }, 0);
                  });
              }
          }],
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
          },
          //图层属性信息
          layerParamsData: [],

          opened: function () { },
          submit: function (modalInstance, form) {
              $rootScope.loginOut();

              var addObj = angular.copy($scope.searchCondition);

              if (!$scope.layerParam.layerName) {
                  alertFun($filter('translate')('views.Layer.alertFun.field.layerPrompt1'), "", 'warning', '#007AFF');
                  return;
              }
              //字段属性
              var tableTmp = $scope.layerParam.tableData;
              //字典项
              var dictTmp = "[";
              for (var i = 0; i < tableTmp.length; i++) {
                  if (tableTmp[i].dictData.length > 0) {
                      for (var j = 0; j < tableTmp[i].dictData.length; j++) {
                          //var obj = { AttributeID: tableTmp[i].tableId, FieldDictName: tableTmp[i].dictData[j].text, FieldDictDesc: tableTmp[i].dictData[j].remark };
                          var obj = "{\"AttributeID\":\"" + tableTmp[i].tableId + "\",\"FieldDictName\":\"" + tableTmp[i].dictData[j].text + "\",\"FieldDictDesc\":\"" + tableTmp[i].dictData[j].remark + "\"}";

                          dictTmp = dictTmp + obj;
                          if (j < tableTmp[i].dictData.length - 1) {
                              dictTmp = dictTmp + ",";
                          }
                      }
                  }
              }
              dictTmp = dictTmp + "]";
              //console.log(dictTmp);

              var fieldDtoList = [];
              //添加图层的配置信息
              for (var i = 0; i < tableTmp.length; i++) {
                  var fieldTmpDto = angular.copy(LayerFieldDto);

                  fieldTmpDto.Id = tableTmp[i].tableId;
                  fieldTmpDto.LayerID = $scope.currentData.id;
                  //fieldTmpDto.LayerName = $scope.layerParam.layerName;
                  fieldTmpDto.AttributeName = tableTmp[i].name;
                  fieldTmpDto.AttributeDesc = '';
                  fieldTmpDto.AttributeType = tableTmp[i].dataTypeID;
                  fieldTmpDto.AttributeLength = tableTmp[i].length;
                  fieldTmpDto.AttributePrecision = tableTmp[i].decimal;
                  fieldTmpDto.AttributeInputCtrl = tableTmp[i].inputCtrl;
                  fieldTmpDto.AttributeInputMax = tableTmp[i].maxVal;
                  fieldTmpDto.AttributeInputMin = tableTmp[i].minVal;
                  fieldTmpDto.AttributeDefault = tableTmp[i].defaultVal;
                  fieldTmpDto.AttributeIsNull = tableTmp[i].empty;
                  fieldTmpDto.AttributeInputFormat = tableTmp[i].inputFormat;
                  fieldTmpDto.Remark = tableTmp[i].describe;
                  fieldTmpDto.AttributeUnit = tableTmp[i].unit;
                  fieldTmpDto.AttributeDataType = tableTmp[i].dataTypeCode;
                  fieldTmpDto.AttributeValueLink = tableTmp[i].linkCode;
                  fieldTmpDto.AttributeDataSource = tableTmp[i].dataSource;
                  fieldTmpDto.AttributeCalComp = tableTmp[i].formula;
                  fieldTmpDto.AttributeSort = i;
                  if (tableTmp[i].dictData.length > 0) {
                      fieldTmpDto.AttributeDict = angular.copy(dictTmp);
                  }

                  fieldDtoList = fieldDtoList.concat(fieldTmpDto);
              }
              //console.log(fieldDtoList);
              if ($scope.pageCounts > 0) {
                  $scope.attrCheckResModal.title = $filter('translate')('views.Layer.layerBro.attrCheck');
                  $scope.attrCheckResModal.layerName = $scope.layerParam.layerName;
                  $scope.attrCheckResModal.allnum = $scope.pageCounts;
                  $scope.attrCheckResModal.checkResult = $filter('translate')('views.Layer.layerBro.incheck');
                  var text = $filter('translate')('views.Layer.layerBro.startcheck');
                  $scope.attrCheckResModal.log = [text];
                  $scope.attrCheckResModal.statues = 0;
                  $scope.attrCheckResModal.times = Math.ceil($scope.pageCounts / 50);
                  $scope.attrCheckResModal.count = 0;
                  $scope.attrCheckResModal.checkResultStyle = {
                      "color": "#5b5b60"
                  }
                  $scope.attrCheckResModal.barStyle = {
                      'width': '0'
                  }
                  $scope.openAttrCheckRes();
                  modalInstance.close();

                  layerField.getNodeJSServerConfig().success(function (data, status) {
                      //console.log(data);
                      var datas = data.split('##');
                      var url = datas[0] + '/' + "SDMS_VALID";

                      //listenToSocketIO(url);

                      layerField.getMultiUpdateField($scope.currentData.id, $scope.layerParam.layerName, localStorage.getItem('infoearth_spacedata_userCode'), fieldDtoList).success(function (data, status) {
                          //console.log(data);
                          data = JSON.parse(data);
                          if (data.length > 0) {
                              $scope.attrCheckResModal.checkResult = data[0].result;
                              if ($scope.attrCheckResModal.checkResult === "通过" || $scope.attrCheckResModal.checkResult === "OK") {
                                  $scope.attrCheckResModal.checkResultStyle = {
                                      "color": "#00A710"
                                  }
                              }
                              else {
                                  $scope.attrCheckResModal.checkResultStyle = {
                                      "color": "#FF0000"
                                  }
                              }
                              $scope.attrCheckResModal.download = data[0].url;
                          }
                      }).error(function (data, status) {
                          console.log(data);
                      });
                  }).error(function (data, status) {
                      console.log(data);
                  });
              }
              else {
                  waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                  layerField.getMultiUpdateField($scope.currentData.id, $scope.layerParam.layerName, localStorage.getItem('infoearth_spacedata_userCode'), fieldDtoList).success(function (data, status) {
                      //console.log(data);
                      data = JSON.parse(data);
                      waitmask.onHideMask();
                      if (data[0].result === "通过") {
                          alertFun($filter('translate')('views.Layer.alertFun.set.setAttrCheckSucc'), '', 'success', '#007AFF');
                      }
                      else {
                          alertFun($filter('translate')('views.Layer.alertFun.set.setAttrCheckErr'), '', 'error', '#007AFF');
                      }
                      modalInstance.close();
                  }).error(function (data, status) {
                      //console.log(data);
                      alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                  });
              }
          },
          cancel: function () { }
      };

      var emptyTableData = {
          name: "", dataType: "", dataTypeID: "", length: "", decimal: "", unit: "", defaultVal: "", inputCtrl: "", maxVal: "", minVal: "",
          dataTypeCode: "", linkCode: "", inputFormat: "", dataSource: "", formula: "", empty: "", describe: "", tableId: "", dictData: []
      };
      //图层列表属性dto
      var LayerFieldDto = {
          Id: '', LayerID: '', AttributeName: '', AttributeDesc: '', AttributeType: '', AttributeLength: '', AttributePrecision: '', AttributeInputCtrl: '', AttributeInputMax: '',
          AttributeInputMin: '', AttributeDefault: '', AttributeIsNull: '', AttributeInputFormat: '', Remark: '', CreateDT: '', AttributeSort: 0, AttributeDict: ""
      }

      //获取图层属性的分页信息
      function getLayerAttrTable(pageSize, pageIndex) {
          $scope.tablePageData = [];

          layerContent.getLayerAttrTabledDetail($scope.currentData.id, pageSize, pageIndex).success(function (data, status) {
              //console.log(data);
              if (data != undefined && !!data) {
                  var pdata = JSON.parse(data);
                  var tableColoums = pdata.LayerField.split(",");
                  var tableVal = JSON.parse(pdata.DataTableJson).data;
                  if (tableColoums != undefined && tableColoums.length > 0 && tableVal != undefined && tableVal.length > 0) {
                      var fieldDtoList = [];
                      
                      for (var i = 0; i < tableVal.length; i++) {
                          var values = [];
                          for (var j = 0; j < tableColoums.length; j++) {
                              values.push(tableVal[i][tableColoums[j]] + ",," + getRandom());
                          }
                          fieldDtoList.push(values);
                      }
                      $scope.tablePageData = [tableColoums, fieldDtoList];
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
                  }
              }
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      $scope.tableWidth = {};

      //图层操作记录的分页
      $scope.maxSize2 = 2;//页码个数显示数
      $scope.goPage2 = 1;//转到多少页
      $scope.pageCounts2 = 0;//32;//总条数
      $scope.pageIndex2 = 1;//1;//起始页
      $scope.pageSize2 = 10;//10;//每页显示条数
      //分页的事件方法
      $scope.pageChanged2 = function (a, evt, index) {
          //console.log(a, evt, index);

          $timeout(function () {
              if (evt && evt.keyCode !== 13) { return; }//注：回车键为13
              if (a) {
                  a = parseInt(a);
                  if (isNaN(a) || a < 1 || a > $scope.totalPages2) {
                      $scope.goPage2 = $scope.pageIndex2;
                      return;
                  }
                  $scope.goPage2 = a;
                  $scope.pageIndex2 = a;
              }
              //console.log($scope.pageIndex);

              getRecordsPageList(index, $scope.pageSize2);
          }, 2000);
      };

      //图层属性信息的分页
      $scope.maxSize = 2;//页码个数显示数
      $scope.goPage = 1;//转到多少页
      $scope.pageCounts = 0;//32;//总条数
      $scope.pageIndex = 1;//1;//起始页
      $scope.pageSize = 10;//10;//每页显示条数
      //分页的事件方法
      $scope.pageChanged = function (a, evt, index) {
          //console.log(a, evt, index);

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
              //console.log($scope.pageIndex);

              getLayerAttrTable($scope.pageSize, index);
          }, 2000);
      };

      getLayerAttrTable($scope.pageSize, $scope.pageIndex);
      /*--------------图层属性--------------end----*/


      /*--------------设置样式弹窗--------------start----*/
      //设置样式
      $scope.setStyle = function () {
          if (!!$scope.layerParam.styleId) {
              $scope.styleModal.styleSelectData.id = $scope.layerParam.styleId;
          }
          $scope.styleModal.title = $filter('translate')('views.Style.setStyle');
          $scope.getStyleTypeTree();
          $scope.opensStyleModal();
      }
      //删除样式
      $scope.delStyle = function () {
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
                  setLayerStyle($scope.layerParam.id, "", function () {
                      $scope.layerParam.styleId = "";
                      $scope.styleModal.styleSelectData.id = "";
                      alertFun($filter('translate')('views.Style.delStyleSucc'), '', 'success', '#007AFF');
                  });
              }
          });
      }
      //编辑样式
      $scope.editStyle = function () {
          $scope.getStyleTypeTree();
          $rootScope.edit($scope.styleObj, null, function (data, status) {
              var obj = {
                  layerName: $scope.layerParam.layerName,
                  layerAttrTable: $scope.layerParam.layerAttrTable,
                  layerDefaultStyleName: $scope.layerParam.styleName,
                  minX: $scope.layerParam.minX,
                  maxX: $scope.layerParam.maxX,
                  minY: $scope.layerParam.minY,
                  maxY: $scope.layerParam.maxY
              };
              $scope.mapSerShow(obj);
          });
      }

      //设置样式弹窗
      $scope.styleModal = {
          title: "",
          id: '',
          layerid: '',
          styleArr: [],
          styleName: '',
          searchStyleText: '',
          styleTypeTreeData: [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }],
          styleTypeSelected: {},
          //选中分类查询
          onStyleTypeSelected: function (node) {
              //console.log(node);
              $scope.styleModal.searchStyleText = "";
              $scope.styleModal.pageIndex = 1;
              //刷新右侧列表
              setStyleInit($scope.styleModal.searchStyleText, node.id, $scope.styleModal.pageSize, $scope.styleModal.pageIndex);
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
              setStyleInit($scope.styleModal.searchStyleText, $scope.styleModal.styleTypeSelected.id, $scope.styleModal.pageSize, $scope.styleModal.pageIndex);
          },
          //搜索框查询样式
          searchStyle: function () {
              $scope.styleModal.pageIndex = 1;
              setStyleInit($scope.styleModal.searchStyleText, $scope.styleModal.styleTypeSelected.id, $scope.styleModal.pageSize, $scope.styleModal.pageIndex);
          },
          //新增样式
          addStyle: function () {
              $rootScope.addStyleWin(null, function (data, status) {
                  $scope.styleModal.pageIndex = 1;
                  $scope.styleModal.searchStyleText = "";
                  setStyleInit("", $scope.styleModal.styleTypeSelected.id, $scope.styleModal.pageSize, $scope.styleModal.pageIndex);
              }, $scope.layerParam.dataType);
          },

          open: function () { },
          submit: function (modalInstance, form) {
              if (!$scope.styleModal.styleSelectData.id) {
                  alertFun($filter('translate')('views.Style.setStylePrompt'), '', 'warning', '#007AFF');
                  return;
              }
              setLayerStyle($scope.layerParam.id, $scope.styleModal.styleSelectData.id, function () {
                  modalInstance.close();
                  alertFun($filter('translate')('views.Style.editStyleSucc'), '', 'success', '#007AFF');
              });
          },
          cancel: function () { }
      };

      //分页查询样式列表
      function setStyleInit(styleName, styleType, pageSize, pageIndex) {
          $scope.styleModal.styleData = [];

          dataStyle.getAllListPage({ StyleName: styleName, StyleType: styleType, StyleDataType: $scope.layerParam.dataType, CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }, pageSize, pageIndex).success(function (data, status) {
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

      /**
      * 修改图层对应样式
      * layerId 图层id
      * styleId 样式id
      * func 回调函数
      */
      function setLayerStyle(layerId, styleId, func) {
          //删除样式绑定时传入“##”作为标识
          styleId = !!styleId ? styleId : "##";
          layerContent.updateDefaultStyle(layerId, styleId, localStorage.getItem('infoearth_spacedata_userCode')).success(function (data, status) {
              //console.log(data);
              getLayer($scope.layerParam.id);
              if (typeof (func) === "function") {
                  func();
              }
          }).error(function (data, status) {
              //console.log(data);
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      /*--------------设置样式弹窗--------------end----*/


      /*--------------字段属性弹窗--------------start----*/
      $scope.tdParamModal = {
          //是否显示提交按钮,及文本
          submitText: "",
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
              if (!!this.tdDataTypeSel.selected.codeName && (this.tdDataTypeSel.selected.codeName !== '时间型' || this.tdDataTypeSel.selected.codeName !== 'DateTime')) {
                  this.readOnly1 = false;
                  this.isTimeType = false;

                  if (this.tdDataTypeSel.selected.codeName == '长整型' || this.tdDataTypeSel.selected.codeName !== 'Long Integer') {
                      this.length = 10;
                      this.precision = '';
                      this.readOnly2 = true;
                      this.readOnly3 = false;
                  }
                  if (this.tdDataTypeSel.selected.codeName == '短整型' || this.tdDataTypeSel.selected.codeName !== 'Short Integer') {
                      this.length = 5;
                      this.precision = '';
                      this.readOnly2 = true;
                      this.readOnly3 = false;
                  }
                  if (this.tdDataTypeSel.selected.codeName == '单浮点型' || this.tdDataTypeSel.selected.codeName !== 'Single Floating') {
                      this.length = 0;
                      this.precision = 0;
                      this.readOnly2 = false;
                      this.readOnly3 = false;
                  }
                  if (this.tdDataTypeSel.selected.codeName == '双浮点型' || this.tdDataTypeSel.selected.codeName !== 'Double Floating') {
                      this.length = 0;
                      this.precision = 0;
                      this.readOnly2 = false;
                      this.readOnly3 = false;
                  }
                  if (this.tdDataTypeSel.selected.codeName == '字符型' || this.tdDataTypeSel.selected.codeName !== 'Character') {
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
          readOnly3: true,
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
                  $scope.tdParamModal.tdInputFormatSel.selected = {};
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
              $scope.calCompModel.fieldName = angular.copy(this.firstname);
              $scope.calCompModel.fieldData = [].concat({ id: 0, value: this.firstname, type: this.tdDataTypeSel.selected.codeName });
              //console.log($scope.layerParam.tableData);
              if ($scope.layerParam.tableData.length > 0) {
                  for (var i = 0; i < $scope.layerParam.tableData.length; i++) {
                      if (this.firstname !== $scope.layerParam.tableData[i].name && $scope.layerParam.tableData[i].dataType !== "时间型") {
                          $scope.calCompModel.fieldData = $scope.calCompModel.fieldData.concat({ id: i + 1, value: $scope.layerParam.tableData[i].name, type: $scope.layerParam.tableData[i].dataType });
                      }
                  }
              }

              $scope.calCompModel.title = $filter('translate')('views.Layer.formulaWin.MAIN');
              $scope.calCompModel.calFuncData = [];
              $scope.calCompModel.checkType = 'number';
              $scope.calCompModel.changeType(1);
              $scope.calCompModel.calFormula = $scope.tdParamModal.formula;
              $scope.openCalCompFun();
          },
          //数据源
          dataSource: '',
          //说明
          remark: '',
          dictData: [],

          opened: function () { },
          submit: function (modalInstance, form) {
              var tmpData = angular.copy(emptyTableData);
              var inputData = angular.copy($scope.tdParamModal);
              if (!!inputData.firstname) {
                  if (!!inputData.tdDataTypeSel.selected) {
                      if (inputData.readOnly1 || !!inputData.length) {
                          if (inputData.firstname.toUpperCase() === "SID") {
                              alertFun($filter('translate')('views.Layer.alertFun.field.namePrompt1'), '', 'warning', '#007AFF');
                              return;
                          }
                          if (!compareLength(inputData.firstname.length, 10, "字段名称")) {
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
                          for (var i in $scope.layerParam.tableData) {
                              var compare = $scope.layerParam.tableData[i];
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

                                  $scope.layerParam.tableData = $scope.layerParam.tableData.concat(tmpData);

                                  $scope.layerParam.createFromText = true;
                                  $scope.layerParam.createFromFile = false;
                                  //console.log(form);
                                  //关闭弹出框
                                  modalInstance.close();
                              }
                              else {
                                  $scope.layerParam.tableData[isNew - 1].name = inputData.firstname;
                                  $scope.layerParam.tableData[isNew - 1].dataType = inputData.tdDataTypeSel.selected.codeName;
                                  $scope.layerParam.tableData[isNew - 1].dataTypeID = inputData.tdDataTypeSel.selected.id;
                                  $scope.layerParam.tableData[isNew - 1].length = isNaN(parseInt(inputData.length)) ? "" : parseInt(inputData.length);
                                  $scope.layerParam.tableData[isNew - 1].decimal = isNaN(parseInt(inputData.precision)) ? "" : parseInt(inputData.precision);
                                  $scope.layerParam.tableData[isNew - 1].unit = inputData.unit;
                                  $scope.layerParam.tableData[isNew - 1].defaultVal = inputData.defaultVal;
                                  if (!!inputData.tdInputCtrlSel.selected) {
                                      $scope.layerParam.tableData[isNew - 1].inputCtrl = inputData.tdInputCtrlSel.selected.value;
                                  }
                                  $scope.layerParam.tableData[isNew - 1].maxVal = inputData.maxVal;
                                  $scope.layerParam.tableData[isNew - 1].minVal = inputData.minVal;

                                  $scope.layerParam.tableData[isNew - 1].dataTypeCode = inputData.dataTypeCode;
                                  $scope.layerParam.tableData[isNew - 1].linkCode = inputData.linkCode;

                                  if (!!inputData.tdInputFormatSel.selected) {
                                      $scope.layerParam.tableData[isNew - 1].inputFormat = inputData.tdInputFormatSel.selected.value;
                                  }
                                  $scope.layerParam.tableData[isNew - 1].dataSource = inputData.dataSource;
                                  //$scope.layerParam.tableData[isNew - 1].formula = inputData.formula;
                                  $scope.layerParam.tableData[isNew - 1].formula = inputData.realFormula;
                                  $scope.layerParam.tableData[isNew - 1].empty = inputData.empty;
                                  $scope.layerParam.tableData[isNew - 1].describe = inputData.describe;
                                  $scope.layerParam.tableData[isNew - 1].dictData = inputData.dictData;
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
          cancel: function () { }
      };
      /*--------------字段属性弹窗--------------end----*/


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
              $scope.createDictModel.lastText = tr.text;
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

          dictManagerOpened: function () { },
          submit: function (a) {
              $scope.tdParamModal.dictData = angular.copy($scope.dictManagerModal.dictData);
              a.close();
          },
          cancel: function () { },
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
              '    <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" translate="setting.cancel" ng-click="submitForm.cancel()" style="float: right; min-width: 80px;">取消</a>' +
              '</div>' +
              '</form>'
      };
      /*--------------字典项管理弹窗--------------end----*/


      /*--------------创建计算公式弹窗--------------start----*/
      $scope.calCompModel = {
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

              //console.log(calFormulas);
              $scope.tdParamModal.realFormula = angular.copy(calFormulas);
              $scope.tdParamModal.formula = angular.copy($scope.calCompModel.calFormula);
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


      /*--------------新增字典项弹窗--------------start----*/
      $scope.createDictModel = {
          title: '',
          id: '',
          text: '',
          lastText: '',
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

      /*--------------属性检查结果弹窗--------------start----*/
      $scope.attrCheckResModal = {
          layerName: "",
          progress: '',
          barStyle: {},
          count: 0,
          times: 0,
          checkResult: "",
          download: "",
          allnum: "",
          log: [],
          status: "",
          checkResultStyle: {},
          open: function () { },
          submit: function (modalInstance, form) {
              modalInstance.close();
          },
          cancel: function () { },
          html: ""
      };
      /*--------------属性检查结果弹窗--------------end----*/
      getLayer($scope.currentData.id);
      //根据id查询图层的属性
      function getLayer(id, func) {
          layerContent.getDetailById(id).success(function (data, status) {
              //console.log(data);
              $scope.layerParam.id = data.id;
              $scope.layerParam.layerName = data.layerName;
              $scope.layerParam.dataType = data.dataType;
              $scope.layerParam.dataTypeName = data.dataTypeName;
              $scope.layerParam.layerType = data.layerType;
              $scope.layerParam.layerTypeName = data.layerTypeName;
              $scope.layerParam.tags = data.layerTag;
              $scope.layerParam.layerAttrTable = data.layerAttrTable;
              $scope.layerParam.layerDesc = data.layerDesc;
              $scope.layerParam.layerBBox = data.layerBBox;
              $scope.layerParam.styleId = data.layerDefaultStyle;
              $scope.layerParam.styleName = data.layerDefaultStyleName;
              $scope.layerParam.spatialRefData = data.layerRefence;
              $scope.layerParam.layerSpatialTable = data.layerSpatialTable;
              $scope.layerParam.minX = data.minX;
              $scope.layerParam.maxX = data.maxX;
              $scope.layerParam.minY = data.minY;
              $scope.layerParam.maxY = data.maxY;
              $scope.layerParam.uploadFileType = data.uploadFileType;

              $scope.mapSerShow(data);
              if (typeof (func) === "function") {
                  func();
              }
              $scope.styleObj = { id: data.layerDefaultStyle };
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      if ($scope.currentData.pagetype == "1") {
          //console.log($scope.currentData.id);
          $scope.pageTitle = 1;
      }
      if ($scope.currentData.pagetype == "2") {
          //根据usercode和图层ID获取导入通知
          //layerReadLog.getDetailByLayer({ LayerID: $scope.currentData.id }).success(function (data, status) {
          //    console.log(data);              
          //    $scope.layerParam.fileImportData = angular.copy(data.items);
          //    for (var i = 0; i < $scope.layerParam.fileImportData.length; i++) {
          //        $scope.layerParam.fileImportData[i].downloadUrl = "";
          //        if (!!$scope.layerParam.fileImportData[i].message && $scope.layerParam.fileImportData[i].message.indexOf("Excel##") > -1) {
          //            var tmpStr = $scope.layerParam.fileImportData[i].message.split("##");
          //            $scope.layerParam.fileImportData[i].message = tmpStr[0];
          //            $scope.layerParam.fileImportData[i].downloadUrl = tmpStr[1];
          //        }
          //    }
          //    //console.log($scope.layerParam.fileImportData);
          //}).error(function (data, status) {
          //    alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          //});
          getRecordsPageList(1, 10);
          $scope.pageTitle = 2;
      }

      //根据图层ID查询图层操作记录
      function getRecordsPageList(index, size) {
          operateLog.getPageListByLayerID($scope.currentData.id, size, index).success(function (data, status) {
              console.log(data);
              $scope.layerParam.fileImportData = angular.copy(data.items);
              //for (var i = 0; i < $scope.layerParam.fileImportData.length; i++) {
              //    $scope.layerParam.fileImportData[i].downloadUrl = "";
              //    if (!!$scope.layerParam.fileImportData[i].message && $scope.layerParam.fileImportData[i].message.indexOf("Excel##") > -1) {
              //        var tmpStr = $scope.layerParam.fileImportData[i].message.split("##");
              //        $scope.layerParam.fileImportData[i].message = tmpStr[0];
              //        $scope.layerParam.fileImportData[i].downloadUrl = tmpStr[1];
              //    }
              //}
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //根据ID查询图层配置数据
      function getLayerField(id) {
          $scope.layerParam.tableData = [];
          $scope.layerParam.tmpTableData = [];
          layerField.getDetailByLayerID(id).success(function (data, statues) {
              //console.log(data);

              //读取字典项数据
              layerFieldDict.getFieldDictByLayerID(id).success(function (data1, statues) {
                  //console.log(data1);
                  $scope.dictManagerModal.allDictData = angular.copy(data1.items);
                  for (var i in data.items) {

                      var tmpData = angular.copy(emptyTableData);
                      for (var j = 0; j < data1.items.length; j++) {
                          if (data1.items[j].attributeID === data.items[i].id) {
                              tmpData.dictData = tmpData.dictData.concat({ id: data1.items[j].id, text: data1.items[j].fieldDictName, remark: data1.items[j].fieldDictDesc });
                          }
                      }
                      tmpData.tableId = data.items[i].id;
                      tmpData.name = data.items[i].attributeName;
                      tmpData.dataType = data.items[i].codeName;
                      tmpData.dataTypeID = data.items[i].attributeType;
                      tmpData.length = data.items[i].attributeLength;
                      tmpData.decimal = data.items[i].attributePrecision;
                      tmpData.unit = data.items[i].attributeUnit;
                      tmpData.defaultVal = data.items[i].attributeDefault;
                      tmpData.inputCtrl = data.items[i].attributeInputCtrl;
                      tmpData.maxVal = data.items[i].attributeInputMax;
                      tmpData.minVal = data.items[i].attributeInputMin;
                      tmpData.inputFormat = data.items[i].attributeInputFormat;
                      tmpData.empty = data.items[i].attributeIsNull;
                      tmpData.describe = data.items[i].remark;
                      tmpData.dataTypeCode = data.items[i].attributeDataType;
                      tmpData.linkCode = data.items[i].attributeValueLink;
                      tmpData.dataSource = data.items[i].attributeDataSource;
                      tmpData.formula = data.items[i].attributeCalComp;
                      if ((/\{|\}/g).test(tmpData.formula)) {
                          tmpData.formula = tmpData.formula.replace(/\{|\}/g, '');
                      }

                      $scope.layerParam.tableData = $scope.layerParam.tableData.concat(tmpData);
                  }
                  //console.log($scope.layerParam.tableData);
                  $scope.layerParam.tmpTableData = angular.copy($scope.layerParam.tableData);
              }).error(function (data, status) {
                  alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
              });
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //根据图层id获取图层属性总条数
      function getLayerDataNum(id) {
          layerContent.getLayerDataCount(id).success(function (data, status) {
              //console.log(data);
              $scope.pageCounts = data;
          }).error(function (data, status) {
              alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
          });
      }

      //获取样式分类树
      $scope.getStyleTypeTree = function () {
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

      //function listenToSocketIO(moduleID) {
      //    //创建socketIO连接
      //    try {
      //        var iosocket = io.connect(moduleID);//连接SocketIO服务器，命名空间为ModuleID
      //        iosocket.on('connect', function () {
      //            iosocket.emit('subscribe', localStorage.getItem('infoearth_spacedata_userCode'));//监听一个频道，频道名称为username
      //            iosocket.on('receive', function (msg) {
      //                //console.log(msg);
      //                $scope.attrCheckResModal.count++;

      //                console.log($scope.attrCheckResModal.count);

      //                $scope.attrCheckResModal.statues += 50;
      //                if (!!msg) {
      //                    var msgs = msg.split('ErrorMsg:');

      //                    for (var i = 1; i < msgs.length; i++) {
      //                        $scope.attrCheckResModal.log.push('ErrorMsg:' + msgs[i]);
      //                        if ($scope.attrCheckResModal.log.length > 200) {
      //                            $scope.attrCheckResModal.log.splice(0, 1);
      //                        }
      //                    }
      //                    //保持滚动条在页面底部
      //                    //$timeout(function () {
      //                    //    var el = $('.layerbrowse-log');
      //                    //    el.scrollTop(el.children.length * 70);
      //                    //},100);
      //                }
      //                $timeout(function () {
      //                    $scope.attrCheckResModal.barStyle = {
      //                        'width': $scope.attrCheckResModal.count / $scope.attrCheckResModal.times * 100 + '%'
      //                    }
      //                    //console.log(msg.length, msg.indexOf("#finish#"));
      //                    if (msg.indexOf("#finish#") === msg.length - 8) {
      //                        $scope.attrCheckResModal.log[$scope.attrCheckResModal.log.length - 1] = $scope.attrCheckResModal.log[$scope.attrCheckResModal.log.length - 1].replace("#finish#", "");
      //                        $scope.attrCheckResModal.barStyle = {
      //                            'width': '100%'
      //                        }
      //                        $scope.attrCheckResModal.log.push($filter('translate')('views.Layer.layerBro.endcheck'));
      //                    }
      //                }, 100);
      //            });

      //            //断开重连
      //            iosocket.on('disconnect', function () {
      //                iosocket = io.connect(moduleID);
      //            });
      //            //服务端抛出异常
      //            iosocket.on('error', function (data) {
      //                alert($filter('translate')('views.Layer.alertFun.connectErr1') + data.toString());
      //            });
      //        });
      //    } catch (e) {
      //        alert($filter('translate')('views.Layer.alertFun.connectErr2'));
      //        return;
      //    }
      //}

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
  }]);

