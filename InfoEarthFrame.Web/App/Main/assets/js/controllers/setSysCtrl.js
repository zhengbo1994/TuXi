'use strict';
/**
 * setSysCtrl Controller
 */
app.controller('setSysCtrl', ['$rootScope', '$scope', 'SweetAlert', '$element', 'selfadapt', '$timeout', 'abp.services.app.map', 'abp.services.app.dataTag', 'abp.services.app.dataType', 'abp.services.app.setSys', 'waitmask', '$filter',
    function ($rootScope, $scope, SweetAlert, $element, selfadapt, $timeout, mapSearch, dataTag, dataType, setSys, waitmask, $filter) {
        $rootScope.loginOut();
        $rootScope.homepageStyle = {};
        $rootScope.app.layout.isNavbarFixed = true;
        //图层的标签、分类的TYPE
        var overAllId = 'ba5ab799-6acd-11e7-87f3-005056bb1c7e';

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
        //图层类型查询的ID
        var layerDataTypeID = "1cfe51dd-67a3-11e7-8eb2-005056bb1c7e";
        //空间参考查询的ID
        var refTypeID = "2cf1d4ba-67ab-11e7-8eb2-005056bb1c7e";
        //数据类型列表查询ID
        var dataTableID = "73160096-67a5-11e7-8eb2-005056bb1c7e";

        setSys.getAppSettingAttribute('SpatialRefence').success(function (data, status) {
            //console.log(data);
            if (data) {
                $scope.spatialRef = data;
            }
        }).error(function (data, status) {
            console.log(data);
        });

        setSys.getAppSettingAttribute('isLoadTianDiTu').success(function (data, status) {
            //console.log(data);
            if (data == 1) {
                $scope.baseMap = "tianDiTu";
            }
            else if (data == 0) {
                $scope.baseMap = "iTelluro";
            }
            else {
                $scope.baseMap = "noBaseMap";
            }
        }).error(function (data, status) {
            console.log(data);
        });

        setSys.getAppSettingAttribute('GridSetName').success(function (data, status) {
            //console.log(data);
            if (data) {
                var strs = data.split(',');

                $scope.sectionName = strs[0];
                $scope.sectionNameSe = strs[1];
                if (strs[1] === "EPSG:4326") {
                    $scope.sectionNameSe = "WMTS";
                }
            }
        }).error(function (data, status) {
            console.log(data);
        });
        setSys.getAppSettingAttribute('ZoomStart').success(function (data, status) {
            //console.log(data);
            if (data) {
                $scope.sectionStart = data;
            }
        }).error(function (data, status) {
            console.log(data);
        });

        setSys.getAppSettingAttribute('ZoomStop').success(function (data, status) {
            //console.log(data);
            if (data) {
                $scope.sectionEnd = data;
                $scope.sectionEndLast = data;
            }
        }).error(function (data, status) {
            console.log(data);
        });

        setSys.getAppSettingAttribute('ZoomStopSecond').success(function (data, status) {
            //console.log(data);
            if (data) {
                $scope.sectionEndSe = data;
                $scope.sectionEndLastSe = data;
            }
        }).error(function (data, status) {
            console.log(data);
        });

        //切换空间参考系
        $scope.ch1 = function () {
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            //console.log($scope.spatialRef);
            var fileString = "";
            var epsg = "";

            if ($scope.spatialRef === "GCS_WGS_1984") {
                fileString = "WGS 1984";
                epsg = "EPSG:4326";
            }
            if ($scope.spatialRef === "GCS_Beijing_1954") {
                fileString = "Beijing 1954";
                epsg = "EPSG:4214";
            }
            if ($scope.spatialRef === "GCS_Xian_1980") {
                fileString = "Xian 1980";
                epsg = "EPSG:4490";
            }
            if ($scope.spatialRef === "GCS_China_Geodetic_Coordinate_System_2000") {
                fileString = "China Geodetic Coordinate System 2000";
                epsg = "EPSG:4610";
            }
            setConfig('SpatialRefence', $scope.spatialRef, function () {
                setConfig('SpatialRefenceFile', fileString, function () {
                    setConfig('EPSG', epsg, function () {
                        waitmask.onHideMask();
                    }, "设置空间参考系错误！");
                }, "设置空间参考系错误！");
            }, "设置空间参考系错误！");
        }

        //切换底图配置
        $scope.ch2 = function () {
            console.log($scope.baseMap);
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            if ($scope.baseMap == 'noBaseMap') {
                setConfig("isLoadTianDiTu", -1, function () { waitmask.onHideMask(); }, "设置底图错误！");
            }
            if ($scope.baseMap == 'tianDiTu') {
                setConfig("isLoadTianDiTu", 1, function () { waitmask.onHideMask(); }, "设置底图错误！");
            }
            if ($scope.baseMap == 'iTelluro') {
                $scope.iTelluroSetModel.title = "设置iTelluro参数";
                $scope.iTelluroSetModel.zoomlevel = "";
                $scope.iTelluroSetModel.dataserverkey = "";
                $scope.iTelluroSetModel.mapUrl = "";
                $scope.iTelluroSetModel.tilesize = "";
                $scope.iTelluroSetModel.zerolevelsize = "";

                setSys.getAppSettingAttribute('zoomlevel').success(function (data1, status) {
                    $scope.iTelluroSetModel.zoomlevel = data1;
                    setSys.getAppSettingAttribute('dataserverkey').success(function (data2, status) {
                        $scope.iTelluroSetModel.dataserverkey = data2;
                        setSys.getAppSettingAttribute('mapUrl').success(function (data3, status) {
                            $scope.iTelluroSetModel.mapUrl = data3;
                            setSys.getAppSettingAttribute('tilesize').success(function (data4, status) {
                                $scope.iTelluroSetModel.tilesize = data4;
                                setSys.getAppSettingAttribute('zerolevelsize').success(function (data5, status) {
                                    //console.log(data);
                                    $scope.iTelluroSetModel.zerolevelsize = data5;

                                    waitmask.onHideMask();
                                    $scope.openITelluroSetFun();
                                }).error(function (data, status) {
                                    waitmask.onHideMask();
                                    console.log(data);
                                });
                            }).error(function (data, status) {
                                waitmask.onHideMask();
                                console.log(data);
                            });
                        }).error(function (data, status) {
                            waitmask.onHideMask();
                            console.log(data);
                        });
                    }).error(function (data, status) {
                        waitmask.onHideMask();
                        console.log(data);
                    });
                }).error(function (data, status) {
                    waitmask.onHideMask();
                    console.log(data);
                });
            }
        }

        //查看iTelluro参数弹窗
        $scope.showITelluro = function () {
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            $scope.iTelluroSetModel.title = $filter('translate')('views.System.Setting.Basemap.ViewITelluro');

            setSys.getAppSettingAttribute('zoomlevel').success(function (data1, status) {
                $scope.iTelluroSetModel.zoomlevel = data1;
                setSys.getAppSettingAttribute('dataserverkey').success(function (data2, status) {
                    $scope.iTelluroSetModel.dataserverkey = data2;
                    setSys.getAppSettingAttribute('mapUrl').success(function (data3, status) {
                        $scope.iTelluroSetModel.mapUrl = data3;
                        setSys.getAppSettingAttribute('tilesize').success(function (data4, status) {
                            $scope.iTelluroSetModel.tilesize = data4;
                            setSys.getAppSettingAttribute('zerolevelsize').success(function (data5, status) {
                                //console.log(data);
                                $scope.iTelluroSetModel.zerolevelsize = data5;

                                waitmask.onHideMask();
                                $scope.openITelluroSetFun();
                            }).error(function (data, status) {
                                waitmask.onHideMask();
                                console.log(data);
                            });
                        }).error(function (data, status) {
                            waitmask.onHideMask();
                            console.log(data);
                        });
                    }).error(function (data, status) {
                        waitmask.onHideMask();
                        console.log(data);
                    });
                }).error(function (data, status) {
                    waitmask.onHideMask();
                    console.log(data);
                });
            }).error(function (data, status) {
                waitmask.onHideMask();
                console.log(data);
            });
        }

        //设置切片终止级数
        $scope.sectionChange = function (num) {
            if (num === 1 && $scope.sectionEndLast !== $scope.sectionEnd) {
                waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
                setConfig('ZoomStop', $scope.sectionEnd, function () {
                    $scope.isShowBtn1 = false;
                    waitmask.onHideMask();
                    $scope.sectionEndLast = angular.copy($scope.sectionEnd);
                }, "设置切片终止级数错误！");
            }
            if (num === 2 && $scope.sectionEndLastSe !== $scope.sectionEndSe) {
                waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
                setConfig('ZoomStopSecond', $scope.sectionEndSe, function () {
                    $scope.isShowBtn2 = false;
                    waitmask.onHideMask();
                    $scope.sectionEndLastSe = angular.copy($scope.sectionEndSe);
                }, "设置切片终止级数错误！");
            }
        }

        //修改切片终止级数
        $scope.isShowBtn1 = false;
        $scope.isShowBtn2 = false;
        $scope.$watch("sectionEnd", function (val, old) {
            if (val !== old && !!val && !!old) {
                //console.log(val);
                $scope.isShowBtn1 = true;
            }
        });
        $scope.$watch("sectionEndSe", function (val, old) {
            if (val !== old && !!val && !!old) {
                //console.log(val);
                $scope.isShowBtn2 = true;
            }
        });

        //设置iTelluro底图弹窗model
        $scope.iTelluroSetModel = {
            title: '',
            zoomlevel: '',
            dataserverkey: '',
            mapUrl: '',
            tilesize: '',
            zerolevelsize: '',
            html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
                '<div class="form-group" style="padding:15px;">\r\n' +
                '    <div class="col-sm-12" style="padding: 0; margin-bottom: 15px;">\r\n' +
                '        <label class="col-sm-3 font-title-little" style="padding-top: 8px; padding-left: 0;" translate="views.System.Setting.Basemap.Magnification">放大级数<span class="symbol required"></span></label>\r\n' +
                '        <input class="col-sm-9" type="text" ng-model="popwinmodal.zoomlevel" ng-disabled="popwinmodal.title===\'查看iTelluro参数\'" style="height: 34px;" />\r\n' +
                '    </div>\r\n' +
                '    <div class="col-sm-12" style="padding: 0; margin-bottom: 15px;">\r\n' +
                '        <label class="col-sm-3 font-title-little" style="padding-top: 8px; padding-left: 0;" translate="views.System.Setting.Basemap.KeyValue">Key值<span class="symbol required"></span></label>\r\n' +
                '        <input class="col-sm-9" type="text" ng-model="popwinmodal.dataserverkey" ng-disabled="popwinmodal.title===\'查看iTelluro参数\'" style="height: 34px;" />\r\n' +
                '    </div>\r\n' +
                '    <div class="col-sm-12" style="padding: 0; margin-bottom: 15px;">\r\n' +
                '        <label class="col-sm-3 font-title-little" style="padding-top: 8px; padding-left: 0;" translate="views.System.Setting.Basemap.LayersUrl">图层url<span class="symbol required"></span></label>\r\n' +
                '        <input class="col-sm-9" type="text" ng-model="popwinmodal.mapUrl" ng-disabled="popwinmodal.title===\'查看iTelluro参数\'" style="height: 34px;" />\r\n' +
                '    </div>\r\n' +
                '    <div class="col-sm-12" style="padding: 0; margin-bottom: 15px;">\r\n' +
                '        <label class="col-sm-3 font-title-little" style="padding-top: 8px; padding-left: 0;" translate="views.System.Setting.Basemap.SliceSize">切片大小<span class="symbol required"></span></label>\r\n' +
                '        <input class="col-sm-9" type="text" ng-model="popwinmodal.tilesize" ng-disabled="popwinmodal.title===\'查看iTelluro参数\'" style="height: 34px;" />\r\n' +
                '    </div>\r\n' +
                '    <div class="col-sm-12" style="padding: 0; margin-bottom: 15px;">\r\n' +
                '        <label class="col-sm-3 font-title-little" style="padding-top: 8px; padding-left: 0;" translate="views.System.Setting.Basemap.ZeroSize">零级大小<span class="symbol required"></span></label>\r\n' +
                '        <input class="col-sm-9" type="text" ng-model="popwinmodal.zerolevelsize" ng-disabled="popwinmodal.title===\'查看iTelluro参数\'" style="height: 34px;" />\r\n' +
                '    </div>\r\n' +
                '</div>' +
                '<div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
                '    <button class="btn btn-wide btn-primary font-title-btn" type="submit" ng-if="popwinmodal.title!==\'查看iTelluro参数\'" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.submit">提交</button>' +
                '    <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()" translate="setting.cancel">取消</a>' +
                '</div>' +
                '</form>'
        };

        //设置iTelluro底图提交  
        $scope.iTelluroSetForm = function (modalInstance) {
            //console.log($scope.iTelluroSetModel);
            if ($scope.iTelluroSetModel.title === "查看iTelluro参数") {
                return;
            }
            if (!$scope.iTelluroSetModel.zoomlevel) {
                alertFun($filter('translate')('views.System.alertFun.Level'), "", 'warning', '#007AFF');
                return;
            }
            if (!$scope.iTelluroSetModel.dataserverkey) {
                alertFun($filter('translate')('views.System.alertFun.Value'), "", 'warning', '#007AFF');
                return;
            }
            if (!$scope.iTelluroSetModel.mapUrl) {
                alertFun($filter('translate')('views.System.alertFun.layerUrl'), "", 'warning', '#007AFF');
                return;
            }
            if (!$scope.iTelluroSetModel.tilesize) {
                alertFun($filter('translate')('views.System.alertFun.sliceSize'), "", 'warning', '#007AFF');
                return;
            }
            if (!$scope.iTelluroSetModel.zerolevelsize) {
                alertFun($filter('translate')('views.System.alertFun.zeroSize'), "", 'warning', '#007AFF');
                return;
            }

            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);

            setConfig('zoomlevel', $scope.iTelluroSetModel.zoomlevel, function () {
                setConfig('dataserverkey', $scope.iTelluroSetModel.zoomlevel, function () {
                    setConfig('mapUrl', $scope.iTelluroSetModel.zoomlevel, function () {
                        setConfig('tilesize', $scope.iTelluroSetModel.zoomlevel, function () {
                            setConfig('zerolevelsize', $scope.iTelluroSetModel.zoomlevel, function () {
                                waitmask.onHideMask();
                                modalInstance.close();
                            }, "设置零级大小错误！");
                        }, "设置切片大小错误！");
                    }, "设置图层url错误！");
                }, "设置Key值错误！");
            }, "设置放大级数错误！");
        }

        //设置iTelluro底图取消
        $scope.cancelITelluroSet = function () {
            setSys.getAppSettingAttribute('isLoadTianDiTu').success(function (data, status) {
                //console.log(data);
                if (data == 1) {
                    $scope.baseMap = "tianDiTu";
                }
                if (data == 0) {
                    $scope.baseMap = "iTelluro";
                }
                if (data == -1) {
                    $scope.baseMap = "noBaseMap";
                }
            }).error(function (data, status) {
                console.log(data);
            });
        }

        //设置参数
        function setConfig(configName, value, successFun, errTitle) {
            setSys.updateAppSetting(configName, value).success(function (data, status) {
                console.log(data);
                successFun() || function () { };
            }).error(function (data, status) {
                console.log(data);
                waitmask.onHideMask();
                alertFun(!!errTitle ? errTitle : $filter('translate')('views.System.alertFun.error'), "", 'warning', '#007AFF');
            });
        }

        /*--------------页面左边树形--------------start----*/
        // 统计分析和已保存报表切换实现
        $scope.selectTab = function (state) {
            $scope.showtab1 = state;
            $scope.typeTreeQueryCtrl.select_branch($scope.typeTreeData[0]);
            $scope.onTypeSelected($scope.typeTreeData[0]);
        }

        //分类查询树
        $scope.typeTreeQueryCtrl = {};
        $scope.typeTreeData = [{
            label: $filter('translate')('setting.all'),
            children: [],
            id: ''
        }];

        // 接收切换语言的事件
        $scope.$on("LanguageChange", function () {
            $scope.typeTreeData[0].label = $filter('translate')('setting.all');
        })

        //当前节点,点击树时自动赋值
        $scope.typeSelected = {};
        //选中树节点回调函数
        $scope.onTypeSelected = function (node) {
            $scope.inputText = "";
            getMap('', node.id, null);
            $scope.tagTreeQueryCtrl.select_branch();
        };

        //标签查询树
        $scope.tagTreeQueryCtrl = {};
        $scope.tagTreeData = [];
        //当前节点,点击树时自动赋值
        $scope.tagSelected = {};
        //选中树节点回调函数
        $scope.onTagSelected = function (node) {
            $scope.inputText = "";
            getMap('', null, node.id);
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
                })
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


        /*--------------网格1数据--------------strart-*/

        //网格1数据
        $scope.myDatasets1 = [];
        //被复选框选中的数据
        $scope.checkedData1 = [];
        //设置分页
        $scope.pageing = { pageIndex: 1, pageSize: 10, pageCounts: 0 };
        //分页点击事件回调函数
        $scope.pageChanged = function () {
            getMap($scope.inputText, $scope.typeSelected.id, $scope.tagSelected.id);
        };
        //选中一行
        $scope.onTdChecked = function (row) {
            var isSame = 0;
            if (row.ischecked && $scope.myDatasets2.length >= 5) {
                row.ischecked = false;
                alertFun($filter('translate')('views.System.alertFun.LeastMaps'), "", 'warning', '#007AFF');
                return;
            }

            for (var i = 0; i < $scope.myDatasets2.length; i++) {
                if (row.id === $scope.myDatasets2[i].id) {
                    if (row.ischecked) {
                        isSame++;
                        break;
                    }
                    else {
                        $scope.myDatasets2.splice(i, 1);
                        break;
                    }
                }
            }

            if (isSame === 0 && row.ischecked) {
                $scope.myDatasets2 = $scope.myDatasets2.concat(row);
                $scope.myDatasets2[$scope.myDatasets2.length - 1].selected = false;
            }
            $scope.toSave();
        }

        /*--------------网格1数据--------------end-*/

        //搜索图层
        $scope.searchMap = function () {
            getMap($scope.inputText, $scope.typeSelected.id, $scope.tagSelected.id);
        }
        //根据条件查询地图数据
        function getMap(mapName, MapType, MapTag) {
            mapSearch.getPageListByName({ MapName: mapName, MapType: MapType, MapTag: MapTag, CreateUserId: localStorage.getItem('infoearth_spacedata_userCode') }, $scope.pageing.pageSize, $scope.pageing.pageIndex).success(function (data, statues) {
                !!data && ($scope.myDatasets1 = data.items, $scope.pageing.pageCounts = data.totalCount);

                for (var i = 0; i < $scope.myDatasets1.length; i++) {
                    for (var j = 0; j < $scope.myDatasets2.length; j++) {
                        if ($scope.myDatasets1[i].id === $scope.myDatasets2[j].id) {
                            $scope.myDatasets1[i].ischecked = true;
                            continue;
                        }
                    }
                }
            }).error(function (data, status) {
                console.log(data);
                alertFun($filter('translate')('views.System.alertFun.error'), data.message, 'error', '#007AFF');
            });
        }

        /*--------------网格2数据--------------strart-*/

        //网格2数据
        $scope.myDatasets2 = [];
        //被复选框选中的数据
        $scope.chedckedData2 = [];
        function loadTable2Data() {
            setSys.getList().success(function (data, statues) {
                if (data) {
                    $scope.myDatasets2 = angular.fromJson(data.json);
                    for (var i = 0; i < $scope.myDatasets2.length; i++) {
                        $scope.myDatasets2[i].tableBtnParams = "";
                        $scope.myDatasets2[i].selected = false;
                    }
                }
            });
        }
        loadTable2Data();

        //已选中列表的操作栏
        $scope.tabBtnParams = [{
            name: "删除",
            click: function (row, name, event) {
                for (var i = 0; i < $scope.myDatasets2.length; i++) {
                    if ($scope.myDatasets2[i].id === row.id) {
                        $scope.myDatasets2.splice(i, 1);
                        break;
                    }
                }
                for (var j = 0; j < $scope.myDatasets1.length; j++) {
                    if ($scope.myDatasets1[j].id === row.id) {
                        $scope.myDatasets1[j].ischecked = false;
                        break;
                    }
                }
                $scope.toSave();
            }
        }];
        /*--------------网格2数据--------------end-*/

        //保存
        $scope.toSave = function () {
            var json = angular.toJson($scope.myDatasets2);
            json = json.replace(/\,\"ischecked\":true/g, '');
            setSys.add({ Json: json }).success(function (data, statues) {
                //console.log(data);
            }).error(function (data, statues) {
                console.log(data);
                alertFun($filter('translate')('views.System.alertFun.error'), data.message, 'error', '#007AFF');
            });
        };

        function alertFun(title, text, type, color) {
            SweetAlert.swal({
                title: title,
                text: text,
                type: type,
                confirmButtonColor: color
            });
        }
    }]);
