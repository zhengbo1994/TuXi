'use strict';
/**
 * homeCtrl Controller
 */
app.controller('homeCtrl', ['$rootScope', '$scope', '$filter', 'SweetAlert', '$element', 'selfadapt', "$timeout", 'abp.services.app.layerContent', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.layerField', 'abp.services.app.dicDataCode', 'abp.services.app.setSys', 'abp.services.app.map', 'abp.services.app.serverInterface',
  function ($rootScope, $scope, $filter, SweetAlert, $element, selfadapt, $timeout, layerContent, dataTag, dataType, layerField, dicDataCode, setSys, map, serverInterface) {
      $rootScope.loginOut();
      $rootScope.homepageStyle = {
          'padding-right': '10px',
          'padding-left': '10px',
      }
      //调用实时随窗口高度的变化而改变页面内容高度的服务
      var unlink = selfadapt.anyChange($element);
      $scope.$on('$destroy', function () {
          unlink();
          selfadapt.showBodyScroll();
      });

      //总数
      $scope.layerNumber = {
          "layerAll": "0",
          "mapAll": "0",
      }
      layerContent.getAllCount(localStorage.getItem('infoearth_spacedata_userCode')).success(function (a, b) {
          if (a) {
              $scope.layerNumber.layerAll = a;
          }
      });
      map.getAllCount(localStorage.getItem('infoearth_spacedata_userCode')).success(function (a, b) {
          if (a) {
              $scope.layerNumber.mapAll = a;
          }
      });

      // 地图数据
      function get_sld(layername) {
          var sld = '<?xml version="1.0" encoding="UTF-8"?>' +
          '<sld:StyledLayerDescriptor xmlns="http://www.opengis.net/sld" xmlns:sld="http://www.opengis.net/sld" xmlns:ogc="http://www.opengis.net/ogc" xmlns:gml="http://www.opengis.net/gml" version="1.0.0">' +
                  '<sld:UserLayer>' +
             '<sld:LayerFeatureConstraints>' +
              '<sld:FeatureTypeConstraint/>' +
              '</sld:LayerFeatureConstraints>' +
              '<sld:Name>' + layername + '</sld:Name>' + //举个例子而已，可以对sld中变量通过参数注入的形式修改，即动态的sld  
              '<sld:UserStyle>' +
              '<sld:Title/>' +
              '<sld:FeatureTypeStyle>' +
              '<sld:Name>group 0</sld:Name>' +
              '<sld:FeatureTypeName>Feature</sld:FeatureTypeName>' +
              '<sld:SemanticTypeIdentifier>generic:geometry</sld:SemanticTypeIdentifier>' +
              '<sld:SemanticTypeIdentifier>simple</sld:SemanticTypeIdentifier>' +
              '<sld:Rule>' +
              '<sld:Name>default rule</sld:Name>' +
              '<sld:PointSymbolizer>' +
              '<sld:Graphic>' +
              '<sld:Mark>' +
              '<sld:WellKnownName>circle</sld:WellKnownName>' +
              '<sld:Fill>' +
              '<sld:CssParameter name="fill">#FFFFFF</sld:CssParameter>' +
              '<sld:CssParameter name="fill-opacity">0.4</sld:CssParameter>' +
              '</sld:Fill>' +
              '<sld:Stroke>' +
              '<sld:CssParameter name="stroke">#0080FF</sld:CssParameter>' +
              '<sld:CssParameter name="stroke-width">1.2</sld:CssParameter>' +
              '</sld:Stroke>' +
              '</sld:Mark>' +
              '<sld:Size>5</sld:Size>' +
              '</sld:Graphic>' +
              '</sld:PointSymbolizer>' +
             '</sld:Rule>' +
              '</sld:FeatureTypeStyle>' +
              '</sld:UserStyle>' +
              '</sld:UserLayer>' +
              '</sld:StyledLayerDescriptor>';
          return sld;
      }

      $scope.mapDataset = mapDataset;
      $scope.isLoadTianDiTu = parseInt(isLoadTianDiTu);

      //选择哪个地图服务
      $scope.mapServiceNum = 2;

      //切换服务
      var layer, legend = GeoServerWmsUrl + '?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=20&HEIGHT=20&LAYER=' + WorkSpace + ':';
      $scope.mapSerShow = function (mapIndex, serviceIndex, item) {
          if ($scope.mapState === mapIndex && $scope.mapServiceNum === serviceIndex) {
              return;
          }
          //服务信息窗口
          $scope.isshowSerInfo = true;
          $scope.mapState = mapIndex;
          $scope.mapServiceNum = serviceIndex;
          $scope.serviceInfo = item;
          $scope.legend = legend + item.mapEnName;

          //console.log('Debug:   ',GeoServerUrl, $scope.legend);
          if (layer) {
              $scope.removeLayer(layer);
          }

          if ($scope.mapServiceNum === 1) {
              //WMS
              layer = newLocalTilesByWMS(GeoServerUrl + '/wms', WorkSpace + ':' + item.mapEnName, 'image/png');
          }
          else if ($scope.mapServiceNum === 2) {
              //WMTS
              layer = newLocalTilesByWMTS(GeoServerUrl + '/gwc/service/wmts', WorkSpace + ':' + item.mapEnName);
          }
          else if ($scope.mapServiceNum === 3) {
              //iTelluro
              var tmpServiceUrl = GeoServerUrl.split('/')[0] + "/Service/GIS/gis.ashx";
              layer = new iTelluro().newItelluroLayer(item.mapName, tmpServiceUrl, 512, 36);
          }
          
          layer.setZIndex(27);
          //layer.getSource().updateParams({ "_": new Date().getTime()+ Math.random() });
          var bounds = [$scope.serviceInfo.minX, $scope.serviceInfo.minY, $scope.serviceInfo.maxX, $scope.serviceInfo.maxY];//范围
          var map = $scope.addLayer(layer, bounds);
      }

      //切换加载地图显示
      $scope.mapTabNameSel = {};
      $scope.mapTabName = [];
      $scope.changeMap = function (li) {
          $scope.mapTabNameSel.selected = angular.copy(li);
          for (var i = 0; i < $scope.mapTabName.length; i++) {
              if ($scope.mapTabName[i].id === $scope.mapTabNameSel.selected.id) {
                  //console.log($scope.mapTabNameSel.selected);
                  $scope.mapSerShow(i, $scope.mapServiceNum, $scope.mapTabNameSel.selected);
                  break;
              }
          }
      }

      //切换地图服务
      $scope.changeService = function (num) {
          $scope.mapSerShow($scope.mapState, num, $scope.mapTabNameSel.selected);
      }

      //获取地图列表
      setSys.getList().success(function (data, statues) {
          if (data) {
              $scope.mapTabName = angular.fromJson(data.json);
              //console.log($scope.mapTabName);
              if ($scope.mapTabName.length > 0) {
                  //console.log($scope.mapTabName);
                  for (var i = 0; i < $scope.mapTabName.length; i++) {
                      $scope.mapTabName[i].mapImgSrc = '/Thumbnail/map/' + $scope.mapTabName[i].mapEnName + '.png';
                  }
                  $scope.mapSerShow(0, $scope.mapServiceNum, $scope.mapTabName[0]);
                  $scope.mapTabNameSel.selected = angular.copy($scope.mapTabName[0]);
              }
          }
      });

      //图例展示窗口控制
      $scope.closeMaplegend = function () {
          $scope.maplegend = {};
      }

      //服务信息窗口显隐控制
      $scope.isshowSerInfo = true;
      $scope.hideSerModel = function () {
          $scope.isshowSerInfo = false;
      }

      //显示按钮的样式
      $scope.activeShowBtn = 0;
      $scope.qiehuanBorderStyle = {
          'padding': '10px 0'
      }
      //切换显示弹窗
      $scope.showRightWin = function (num) {
          $scope.activeShowBtn = num;
          var btnCssTop = angular.element('.mainPageMap>.righttag:eq(0)').css('top');
          var top = Math.ceil(btnCssTop.split('px')[0]);
          if (num == 1) {
              $scope.qiehuanBorderStyle = {
                  'padding-top': '11px',
                  'padding-bottom': '10px',
              }
          }
          else if (num == 2) {
              $scope.qiehuanBorderStyle = {
                  'padding': '10px 0'
              }
              top = top + 38;
              $scope.choseMapWinStyle = {
                  'top': top + 'px'
              }
          }
          else if (num == 3) {
              $scope.qiehuanBorderStyle = {
                  'padding-top': '10px',
                  'padding-bottom': '11px',
              }
              top = top + 38 * 2;
              $scope.choseServiceWinStyle = {
                  'top': top + 'px'
              }
          }
      }
      //关闭上面的窗口
      $scope.closeMapWin = function () {
          $scope.activeShowBtn = 0;
          $scope.qiehuanBorderStyle = {
              'padding': '10px 0'
          }
      }

      //翻译tooltip的内容
      $scope.layerTooltip = $filter('translate')('views.home.charts.layer.tooltip');
      $scope.mapTooltip = $filter('translate')('views.home.charts.map.tooltip');
      
      //echart图表
      var chartArr = [];
      function loadEChart(el, dt, seriesName) {
          var arr = [1, 2, 3];
          var chart = echarts.init(el);
          chart.setOption({
              tooltip: {
                  trigger: 'item',
                  formatter: "{a} <br/>{b}: {c} ({d}%)"
              },
              //color: dt.colors,
              legend: {
                  orient: 'vertical',
                  type: 'scroll',
                  top: '42',
                  right: '0%',
                  //x: 'right',
                  y: 'right',
                  data: dt.names,
                  tooltip: {
                      trigger: 'item',
                      show: true,
                      formatter: function (data) {
                          return data.name + ":" + dt.data[dt.names.indexOf(data.name)].value;
                      }
                  }
              },
              series: [
                  {
                      name: seriesName,//'图层比例',
                      type: 'pie',
                      center: ['35%', '50%'],
                      radius: ['50%', '80%'],
                      avoidLabelOverlap: false,
                      label: {
                          normal: {
                              show: true,
                              position: 'outside',
                              formatter: "{c}",
                              textStyle: {
                                  fontSize: '15',
                                  fontWeight: 'bold',

                              }
                          },
                          emphasis: {
                              show: true,
                              position: 'outside',
                              formatter: "{c}",
                              textStyle: {
                                  fontSize: '15',
                                  fontWeight: 'bold',
                              }
                          }
                      },
                      labelLine: {
                          normal: {
                              show: true,
                              length: 10,
                              length2: 10,
                              lineStyle: {
                                  width: 1,
                                  type: "solid",
                                  color: "rgba(176,58,91,0.6)"
                              }
                          },
                          emphasis: {
                              show: true,
                              length: 10,
                              length2: 10,
                              lineStyle: {
                                  width: 1,
                                  type: "solid",
                                  color: "rgba(176,58,91,0.6)"
                              }
                          }
                      },
                      data: dt.data
                  }
              ]
          });
          chartArr.push(chart);
      }
      //图层的环形图
      function getLayerPie() {
          layerContent.getAllCountByDataType(localStorage.getItem('infoearth_spacedata_userCode')).success(function (a, b) {
              //console.log(a);

              var el = angular.element(".pie1")[0];
              loadEChart(el, a, '图层比例');
          });
      }
      
      //地图的环形图
      function getMapPie() {
          map.getAllCountByDataType(localStorage.getItem('infoearth_spacedata_userCode')).success(function (a, b) {
              //console.log(a);

              var el = angular.element(".pie2")[0];
              loadEChart(el, a, '地图比例');
          });
      }
      getLayerPie();
      getMapPie();

      //是否展示地图下方图表
      $scope.isDown = true;
      $scope.hideECharts = function () {
          $scope.isDown = !$scope.isDown;
          var winH = angular.element(window).height();
          var mapH = (winH - 65 - 10 - 36 - 14);
          if ($scope.isDown) {
              mapH = mapH - 200;
          }
          //console.log(mapH);

          $scope.mapheight = mapH;
          angular.element(".mapDiv").height(mapH);

          if ($scope.isDown) {
              var echartWid = ($('.layerNumber').width() - 10) / 2;
              $('.pie1').css('width', echartWid & 0.8 + 'px');
              $('.pie2').css('width', echartWid & 0.8 + 'px');
              getLayerPie();
              getMapPie();
          }
          
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
      
      //自适应高度
      function setHeight() {
          var winH = angular.element(window).height();
          var winW = angular.element(window).width();
          var mapH = (winH - 125);
          if ($scope.isDown) {
              mapH = mapH - 200;
          }
          $scope.mapheight = mapH;
          angular.element(".mapDiv").height(mapH);
          var rightConH = (mapH / 2) - 46;
          angular.element(".righttag .r-right .r-right-top").height(rightConH);

          if (winW >= 992) {
              $scope.mapwidth = winW - 212;
          }
          else {
              $scope.mapwidth = winW - 17;
          }
          angular.element(".mapDiv").width($scope.mapwidth);
      }
      $timeout(function () {
          setHeight();
      });
      angular.element(window)[0].onresize = function () {
          setHeight();
          for (var i in chartArr) {
              chartArr[i].resize();
          }
      }
  }]);
