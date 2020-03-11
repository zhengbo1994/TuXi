'use strict';
/**
 * mapManagerCtrl Controller
 */
app.controller('mapSearchCtrl',
    ['$rootScope', '$scope', '$document', 'SweetAlert', '$element', 'selfadapt', "$timeout", 'abp.services.app.dataType', 'abp.services.app.map', 'abp.services.app.mapReleation', 'waitmask',
  function ($rootScope, $scope, $document, SweetAlert, $element, selfadapt, $timeout, dataType, map, mapReleation, waitmask) {
      $rootScope.loginOut();
      $rootScope.homepageStyle = {};
      //调用实时随窗口高度的变化而改变页面内容高度的服务
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
      var currentData = {
          "id": GetQueryString("id"),
          "mapName": GetQueryString("mapName")
      }
      
      // 地图数据
      $scope.mapDataset = mapDataset;
      $scope.isLoadTianDiTu = parseInt(isLoadTianDiTu);

      //用于获取画方画圆后的坐标点
      $scope.getExtent = [];
      $scope.location = [];
      $scope.getMap = null;

      //点选查出的数据条数
      $scope.dataNum = 0;
      //中心点
      $scope.centerPoint = [];
      //展示搜索结果窗口
      $scope.isShowRes = false;
      //展示选择图层窗口
      $scope.isShowChose = false;
      $scope.treectrlObj = {};

      $scope.hideChose = function () {
          $scope.isShowChose = !$scope.isShowChose;
      }

      //选择图层窗口
      $scope.choseLayerModel = {
          val: '',
          ischecked1: false,
          layerData: [],
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

      //获取图层列表
      mapReleation.getAllListByMapId(currentData.id).success(function (data, status) {
          //console.log(data);
          $scope.choseLayerModel.layerData = angular.copy(data.items);
          //默认选中所有图层
          for (var i in $scope.choseLayerModel.layerData) {
              $scope.choseLayerModel.layerData[i].ischecked = true;
          }
          $scope.choseLayerModel.ischecked1 = true;
      }).error(function (data, status) {
          alertFun('错误!', data.message, 'error', '#007AFF');
      });

      var layerId = "";

      $scope.measuremapBefore = function () {
          $scope.isShowChose = true;
          layerId = "";
          for (var i = 0; i < $scope.choseLayerModel.layerData.length; i++) {
              if ($scope.choseLayerModel.layerData[i].ischecked) {
                  layerId += $scope.choseLayerModel.layerData[i].dataConfigID + ',';
              }
          }
          if (!layerId) {
              alertFun('请选择至少一个图层!', '', 'warning', '#007AFF');
              return 0;
          }
          layerId = layerId.slice(0, layerId.length - 1);

          return 1;
      }

      //画点之后执行的方法
      $scope.measuremapAfter = function (val, measure) {
          //console.log(val, measure);
          $scope.dataNum = 0;
          $scope.layerTreeData = [];
          $scope.layerTableData = {};
          clearLayer();
          
          if (!!val && !!layerId) {
              $timeout(function () {
                  waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
                  $scope.isShowChose = false;
                  if (measure == "point") {
                      $scope.getExtent = val.slice(0, 2);
                      var tmpLoca = [coorToAngle($scope.getExtent[0]), coorToAngle($scope.getExtent[1])];

                      $scope.location = tmpLoca.join(',');
                      //console.log(layerId, $scope.getExtent[0], $scope.getExtent[1]);
                      var resolution = $scope.getMap.getView().getResolution();
                      var tolerance = resolution * 10;//10像素容差查询
                      //console.log(resolution);
                      map.getLayerAttrByLayerPt(layerId, $scope.getExtent[0], $scope.getExtent[1], tolerance).success(function (data, status) {
                          //console.log(data);
                          getTableData(data);
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun('错误!', data.message, 'error', '#007AFF');
                      });
                  }
                  else if (measure == "square") {
                      $scope.getExtent = angular.copy(val);
                      $scope.location = [coorToAngle($scope.getExtent[0]), coorToAngle($scope.getExtent[1])].join(',') + ' ' + [coorToAngle($scope.getExtent[2]), coorToAngle($scope.getExtent[3])].join(',');
                      var minLon = $scope.getExtent[0] < $scope.getExtent[2] ? $scope.getExtent[0] : $scope.getExtent[2];
                      var maxLon = $scope.getExtent[0] > $scope.getExtent[2] ? $scope.getExtent[0] : $scope.getExtent[2];
                      var minLat = $scope.getExtent[1] < $scope.getExtent[3] ? $scope.getExtent[1] : $scope.getExtent[3];
                      var maxLat = $scope.getExtent[1] > $scope.getExtent[3] ? $scope.getExtent[1] : $scope.getExtent[3];
                      map.getLayerAttrByRect(layerId, minLon, minLat, maxLon, maxLat, tolerance).success(function (data, status) {
                          //console.log(data);
                          getTableData(data);
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun('错误!', data.message, 'error', '#007AFF');
                      });
                  }
                  else if (measure == "circle") {
                      $scope.getExtent = angular.copy([val[0], val[1]]);
                      var distance = getDistanceByLat(val[1], val[0], val[1], val[0] + val[2]);
                      $scope.location = [coorToAngle(val[0]), coorToAngle(val[1])].join(',') + ' ' + val[2];
                      //return;
                      map.getLayerAttrByPtTolerane(layerId, $scope.getExtent[0], $scope.getExtent[1], distance).success(function (data, status) {
                          //console.log(data);
                          getTableData(data);
                      }).error(function (data, status) {
                          waitmask.onHideMask();
                          alertFun('错误!', data.message, 'error', '#007AFF');
                      });
                  }
                  
              }, 200);
          }
      }

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
          clearLayer();
          clearData();
      }

      //关闭查询结果弹窗
      $scope.closeRes = function () {
          $scope.getExtent = [];
          clearLayer();
          clearData();
      }

      //切换服务
      var layer, legend = GeoServerWmsUrl + '?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=20&HEIGHT=20&LAYER=' + WorkSpace + ':';
      $scope.mapSerShow = function (index, item) {
          if ($scope.state === index) {
              return;
          }
          //服务信息窗口
          $scope.isshowSerInfo = true;
          $scope.state = index;
          $scope.serviceInfo = item;
          $scope.legend = legend + item.mapEnName;

          //console.log('Debug:   ',GeoServerUrl, $scope.legend);
          if (layer) {
              $scope.removeLayer(layer);
          }
          //WMS
          //layer = newLocalTilesByWMS(GeoServerUrl + '/wms', WorkSpace + ':' + item.mapEnName, 'image/png');
          //WMTS
          layer = newLocalTilesByWMTS(GeoServerUrl + '/gwc/service/wmts', WorkSpace + ':' + item.mapEnName);

          layer.setZIndex(27);
          //layer.getSource().updateParams({ "_": new Date().getTime()+ Math.random() });
          var bounds = [$scope.serviceInfo.minX, $scope.serviceInfo.minY, $scope.serviceInfo.maxX, $scope.serviceInfo.maxY];//范围
          var map = $scope.addLayer(layer, bounds);
      }

      //图片查看器初始化
      var imgDom = angular.element("#dowebok")[0];
      var viewer = new Viewer(imgDom, {
          "toolbar": false,
      });
      //查看大图
      $scope.lookPitcure = function (imgsrc) {
          viewer.show();
      }

      //根据跳转过来的mapid查询地图信息
      map.getDetailById(currentData.id).success(function (data, statues) {
          //console.log(data);
          if (countObj(data) > 0) {
              
              $scope.serviceList = angular.copy(data);
              $scope.mapSerShow(0, $scope.serviceList);
          }
      }).error(function (val, status) {
          //console.log(val, status);
      });

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
                      vectorLayers[i] = getTaggingCoor(node.children[i].geom);
                  }
                  //console.log(vectorLayers);
              }
              if (node.isLeaf) {
                  $scope.layerTableData = angular.copy(node.data);
                  vectorLayer = new ol.layer.Vector({
                      source: vectorSource
                  });
                  vectorLayer = getTaggingCoor(node.geom);
                  //console.log(vectorLayer);
              }
          }, 200);
      };

      //清理数据
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
          //if (tmp0[0].indexOf('MULTIPOLYGON') > -1) {
          //    var tmpAs = geom.split('(((')[1].split(')))')[0].split(")),((");
          //    for (var i = 0; i < tmpAs.length; i++) {
          //        var tmpDs = tmpAs[i].split('),(');
          //        for (var j = 0; j < tmpDs.length; j++) {
          //            var tmpEs = tmpDs[j].split(',');
          //            var tmpHs = [];
          //            for (var k = 0; k < tmpEs.length; k++) {
          //                var tmpFs = tmpEs[k].split(' ');
          //                tmpHs.push(tmpFs);
          //            }
          //            coor = coor.concat(tmpHs);
          //        }
          //    }
          //    //console.log(coor);

          //    vLayer = $scope.addTagging("MultiPolygon", coor, '#00fffa', '#000');
          //}
          //else if (tmp0[0].indexOf('POLYGON') > -1) {
          //    var tmpA = geom.split('((')[1].split('))')[0].split("),(");
          //    for (var j = 0; j < tmpA.length; j++) {
          //        var tmpB = tmpA[j].split(',');
          //        var tmpC = [];
          //        for (var k = 0; k < tmpB.length; k++) {
          //            var tmpD = tmpB[k].split(' ');
          //            tmpC.push(tmpD);
          //        }
          //        coor = coor.concat(tmpC);
          //    }

          //    vLayer = $scope.addTagging("MultiPolygon", coor, '#00fffa', '#000');
          //    $scope.centerPoint = angular.copy(coor[0]);
          //}
          //console.log(vLayer);
          //console.log($scope.centerPoint);
          $scope.tolocation($scope.centerPoint);
          return vLayer;
      }

      //根据坐标经纬度计算两点之间的距离
      function getDistanceByLat(lat1, lon1, lat2, lon2) {
          console.log(lat1, lon1, lat2, lon2);
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

      function rad(d) {
          return d * Math.PI / 180.0;
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

      //获取鼠标位置
      function getCrossBrowserElement() {
          var result = {
              x: 0,
              y: 0,
              relativeX: 0,
              relativeY: 0,
              currentDomId: ""
          };

          var mouseEvent = window.event;

          if (mouseEvent.pageX || mouseEvent.pageY) {
              result.x = mouseEvent.pageX;
              result.y = mouseEvent.pageY;
          }
          else if (mouseEvent.clientX || mouseEvent.clientY) {
              result.x = mouseEvent.clientX + document.body.scrollLeft +
                document.documentElement.scrollLeft;
              result.y = mouseEvent.clientY + document.body.scrollTop +
                document.documentElement.scrollTop;
          }

          result.relativeX = result.x;
          result.relativeY = result.y;

          if (mouseEvent.target) {
              var offEl = mouseEvent.target;
              var offX = 0;
              var offY = 0;
              if (typeof (offEl.offsetParent) != "undefined") {
                  while (offEl) {
                      offX += offEl.offsetLeft;
                      offY += offEl.offsetTop;
                      offEl = offEl.offsetParent;
                  }
              }
              else {
                  offX = offEl.x;
                  offY = offEl.y;
              }

              result.relativeX -= offX;
              result.relativeY -= offY;
          }
          result.currentDomId = mouseEvent.target.id;

          return result;
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
      //计算对象的长度
      function countObj(obj) {
          var count = 0;
          for (var i in obj) {
              count++;
          }
          return count;
      }

      //$scope.mapheight = angular.element(window).height() - 157;
      //自适应高度  
      function setHeight() {
          //var winH = angular.element(window).height();
          var winH = angular.element(".panel-body").height();
          var contentH = (winH - 5);
          //angular.element(".mapDiv").height(contentH);
          $scope.mapheight = contentH;
      }
      $timeout(setHeight, 300);
      angular.element(window)[0].onresize = function () {
          setHeight();
      }
  }]);


