'use strict';
/**
 * mapManagerCtrl Controller
 */
app.controller('styleManagerCtrl',
    ['$cookieStore', '$rootScope', '$scope', '$document', 'SweetAlert', '$element', 'selfadapt', "$timeout", 'abp.services.app.dataType', 'abp.services.app.dataTag', 'abp.services.app.dataStyle', 'abp.services.app.layerContent', 'waitmask', 'abp.services.app.layerField', 'abp.services.app.dicDataCode', '$filter',
        function ($cookieStore, $rootScope, $scope, $document, SweetAlert, $element, selfadapt, $timeout, dataType, dataTag, dataStyle, layerContent, waitmask, layerField, dicDataCode, $filter) {
            $rootScope.loginOut();
            $rootScope.homepageStyle = {};
            //调用实时随窗口高度的变化而改变页面内容高度的服务
            var unlink = selfadapt.anyChange($element);
            $scope.$on('$destroy', function () {
                unlink();
                selfadapt.showBodyScroll();
            });
            //初始化
            var dataTypeID = 'c755eeea-986d-11e7-90b1-005056bb1c7e';
            $scope.inputText = '';//搜索input
            $scope.typeNodeId = '';//树选中类型

            //图层的标签、分类的TYPE
            var layerTypeID = "a2faae61-6acd-11e7-87f3-005056bb1c7e";

            //模糊搜索
            $scope.searchStyle = function () {
                styleInit($scope.inputText, $scope.typeNodeId);
            }

            //分页初始信息
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
                //console.log({ MapName: $scope.inputText, MapType: $scope.typeNodeId });
                //调用AJAX
                styleInit($scope.inputText, $scope.typeNodeId);
            };

            /*--------------页面左边树形--------------start----*/

            //分类查询树
            $scope.typeTreeData = [{ label: $filter('translate')('setting.all'), children: [], id: '' }];

            // 接收切换语言的事件
            $scope.$on("LanguageChange", function () {
                $scope.typeTreeData[0].label = $filter('translate')('setting.all');
            })

            //当前节点,点击树时自动赋值
            $scope.selectedType = "";

            //选中分类查询
            $scope.onTypeSelected = function (node) {
                //console.log(node);
                $scope.inputText = "";
                $scope.typeNodeId = node.id;
                $scope.pageIndex = 1;
                //刷新右侧列表
                styleInit($scope.inputText, $scope.typeNodeId);
            };

            //分类树
            $scope.getTypeTree = function () {
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
                    $rootScope.typeTreeData = angular.copy($scope.typeTreeData);//给全局变量附值
                });
            }
            $scope.getTypeTree();
            $rootScope.getTypeTree = angular.copy($scope.getTypeTree);//给全局变量附值

            /*--------------页面左边树形--------------end----*/

            //分页查询样式列表
            function styleInit(styleName, styleType) {
                //console.log('Debug:   ', styleName, styleType, $scope.pageSize, $scope.pageIndex);

                dataStyle.getAllListPage({ StyleName: styleName, StyleType: styleType, CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }, $scope.pageSize, $scope.pageIndex).success(function (data, status) {
                    //console.log(data);
                    $scope.pageCounts = data.totalCount;
                    $scope.totalPages = Math.ceil($scope.pageCounts / $scope.pageSize);
                    $scope.styleData = data.items;
                }).error(function (data, status) {
                    //console.log(data);
                    $rootScope.alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                });
            }
            $scope.styleInit = styleInit;
            //console.log('typeTreeData', $scope.typeTreeData)

            //删除方法
            $scope.del = function (id) {
                SweetAlert.swal({
                    title: $filter('translate')('views.Style.alertFun.delete'),
                    text: $filter('translate')('views.Style.DeleteTips'),
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText:  $filter('translate')('setting.sure'),
                    cancelButtonText: $filter('translate')('setting.cancel')
                }, function (isConfirm) {
                    if (isConfirm) {
                        waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                        dataStyle.delete(id).success(function (data, statues) {
                            //console.log(data, statues);
                            waitmask.onHideMask();
                            if (data) {
                                $rootScope.alertFun($filter('translate')('views.Style.alertFun.DeleteSuccessfully'), '', 'success', '#007AFF');
                                styleInit($scope.inputText, $scope.typeNodeId, $scope.pageSize, $scope.pageIndex);
                            } else {
                                $rootScope.alertFun(('translate')('views.Style.alertFun.FailedDelete'), '', 'error', '#007AFF');
                            }
                        }).error(function (data, status) {
                            waitmask.onHideMask();
                            $rootScope.alertFun($filter('translate')('setting.error'), data.message, 'error', '#007AFF');
                        });
                    }
                });
            }


            //获取图层列表
            $scope.getAllLayer = function (typeId, tagId, name, size, index) {
                $scope.choseLayerModol.choseableLayer = [];
                layerContent.getPageListByName({ LayerName: name, LayerType: typeId, LayerTag: tagId, CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }, size, index).success(function (data, status) {
                    //console.log(data);
                    $scope.choseLayerModol.pageCounts = data.totalCount;
                    $scope.choseLayerModol.totalPages = Math.ceil($scope.choseLayerModol.pageCounts / $scope.choseLayerModol.pageSize);
                    $scope.choseLayerModol.choseableLayer = angular.copy(data.items);

                    for (var i in $scope.choseLayerModol.choseableLayer) {
                        $scope.choseLayerModol.choseableLayer[i].checked = false;
                        if (!!$scope.choseLayerModol.checkedData.id) {
                            if ($scope.choseLayerModol.choseableLayer[i].id === $scope.choseLayerModol.checkedData.id) {
                                $scope.choseLayerModol.choseableLayer[i].checked = true;
                            }
                        }
                    }
                });
            }

            //分类树
            function getLayerTypeTree() {
                $scope.choseLayerModol.typeTreeData[0].children = [];
                //分类查询树的数据
                dataType.getAllListByDataType(layerTypeID).success(function (data, statues) {
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
                            $scope.choseLayerModol.typeTreeData[0].children.push(each);
                        }
                    });
                    $scope.choseLayerModol.typeTreeData[0].children.forEach(function (each) {
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
            function getLayerTagTree() {
                //标签查询树的数据
                dataTag.getAllListByDataType(layerTypeID).success(function (data, statues) {
                    //console.log(data);
                    for (var i in data.items) {
                        var tempTagData = {};
                        tempTagData.id = data.items[i].id;
                        tempTagData.dictCodeID = data.items[i].dictCodeID;
                        tempTagData.label = data.items[i].tagName;
                        tempTagData.tagDesc = data.items[i].tagDesc;
                        tempTagData.children = [];

                        $scope.choseLayerModol.tagTreeData = $scope.choseLayerModol.tagTreeData.concat(tempTagData);
                    }
                });
            }
            /*--------------选择图层弹窗--------------end----*/

            
            //查看大图----------------- ___----————————————
            $scope.lookPitcure = function (imgsrc) {
                //图片查看器初始化
                var imgDom = angular.element("#dowebok")[0];
                var viewer = new Viewer(imgDom, {
                    "toolbar": false,
                });
                viewer.show();
            }

            /*--------------样式预览弹窗--------------end----*/


            //提示框
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
                    confirmButtonText:  $filter('translate')('setting.sure'),
                    cancelButtonText: $filter('translate')('setting.cancel')
                }, function (isConfirm) {
                    if (isConfirm) {
                        backFun();
                    }
                });
            };

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

            /*

            //检查样式内容的首尾
            function matchingXmlHeadAndEnd(content) {
                var firstStrs = content.substr(0, content.indexOf('>') + 1);
                var lastStrs = content.substr(content.lastIndexOf('<\/') + 2);
                if (firstStrs.indexOf('<') === 0 && lastStrs.indexOf('>') === lastStrs.length - 1) {
                    return true;
                }
                return false;
            }

            //预览样式
            function preStyle(styleName, data, sld) {

                var style = {
                    StyleName: styleName,
                    StyleContent: sld
                };
                dataStyle.insertGeoServerStyle(style).success(function (result, status) {
                    if (status == 200) {
                        $("#map").html("");
                        var format = 'image/png';
                        //var bounds = [$scope.addStyleModol.currentLayer.data.minX, $scope.addStyleModol.currentLayer.data.minY, $scope.addStyleModol.currentLayer.data.maxX, $scope.addStyleModol.currentLayer.data.maxY];//范围
                        var bounds = [data.minX, data.minY, data.maxX, data.maxY];//范围
                        var ImageMap = new ol.layer.Image({
                            source: new ol.source.ImageWMS({
                                ratio: 1,
                                url: GeoServerUrl + '/wms',
                                params: {
                                    FORMAT: format,
                                    VERSION: '1.1.0',
                                    STYLES: styleName,
                                    //SLD_BODY: sld,
                                    LAYERS: WorkSpace + ":" + data.layerAttrTable,
                                }
                            })
                        });
                        var projection = new ol.proj.Projection({
                            code: 'EPSG:4326',//投影编码
                            units: 'degrees',
                            axisOrientation: 'neu'
                        });
                        var map = new ol.Map({
                            controls: ol.control.defaults({
                                attribution: false
                            }).extend([
                                new ol.control.ScaleLine()
                            ]),
                            target: 'map',
                            layers: [
                                ImageMap
                            ],
                            view: new ol.View({
                                projection: projection
                            })
                        });
                        map.getView().fit(bounds, map.getSize());

                    }
                }).error(function (data) {

                });


                //console.log(map);
            }*/
        }]);

