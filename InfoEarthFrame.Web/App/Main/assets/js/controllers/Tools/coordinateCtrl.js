'use strict';
app.controller('coordinateCtrl', ['$scope', '$rootScope', '$document', '$element', 'selfadapt', '$http', '$timeout', '$compile', '$modal', 'Upload', 'SweetAlert', 'abp.services.app.dataConvert', '$interval', 'waitmask', 'abp.services.app.setSys', '$filter',
    function ($scope, $rootScope, $document, $element, selfadapt, $http, $timeout, $compile, $modal, Upload, SweetAlert, dataConvert, $interval, waitmask, setSys, $filter) {
        $rootScope.loginOut();
        $rootScope.homepageStyle = {};
        //调用实时随窗口高度的变化而改变页面内容高度的服务
        var unlink = selfadapt.anyChange($element);
        $scope.$on('$destroy', function () {
            unlink();
            selfadapt.showBodyScroll();
        });

        var isCheck1 = 0, isCheck2 = 0;
        
        //切换tab
        $scope.showtab1 = true;
        $scope.selectTab = function (state) {
            $scope.showtab1 = state;
        }

        //供选择的坐标参考系
        $scope.outputTypeData = [{
            label: 'GCS_WGS_1984',
            value: 'WGS 1984'
        }, {
            label: 'GCS_Beijing_1954',
            value: 'Beijing 1954'
        }, {
            label: 'GCS_Xian_1980',
            value: 'Xian 1980'
        }, {
            label: 'CGCS_2000',
            value: 'China Geodetic Coordinate System 2000'
        }];

        //选中的坐标参考系
        $scope.outputType1 = {};
        $scope.outputType2 = {};

        //查询配置信息中的默认选中参考系
        for (var i = 0; i < $scope.outputTypeData.length; i++) {
            if ($scope.outputTypeData[i].value === SpatialRefenceFile) {
                $scope.outputType1.selected = angular.copy($scope.outputTypeData[i]);
                $scope.outputType2.selected = angular.copy($scope.outputTypeData[i]);
            }
        }

        //坐标点字符串
        $scope.CoordPoint = [];

        //7参数设置
        $scope.setParam1 = function () {
            $scope.openSevenParam();
        }
        //控制点参数设置
        $scope.setParam2 = function () {
            if ($scope.pointParam.tableData.length < 1) {
                var tmp = angular.copy(pointParamTableData);
                tmp.id = getRandom();
                $scope.pointParam.tableData.push(tmp);
            }
            $scope.openPointParam();
        }

        //解决第二个页面的上传文件按钮不能点击
        $scope.show2 = false;
        $scope.tabs = function (num) {            
            if (num === 1) {
                $scope.show2 = false;
            }
            else {
                $scope.show2 = true;
            }
        }

        // 上传文件按钮中英文切换
        //$scope.uploadFilesTxt = $filter('translate')('views.Toolset.FormatConversion.uploadFiles');
        //$scope.$on("LanguageChange", function () {
        //    $scope.uploadFilesTxt = $filter('translate')('views.Toolset.FormatConversion.uploadFiles');
        //});

        //需传入后台的对象元素
        var ConvertFileList = {
            ID: '',
            FileType: '',
            LogicFileName: '',
            PhysicsFilePath: '',
            ConvertFilePath: '',
            ConvertResult: 0,
            ConvertMsg: '',
            SrcCoordName: '',
            CoordName: '',
            WKT: '',
            ConvertKey: ''
        }
        //上传的文件列表
        $scope.fileDatas1 = [];
        $scope.fileDatas2 = [];
        //传入后台的数据列表
        $scope.fileList1 = [];
        $scope.fileList2 = [];

        //7参数--所有文件上传完之后执行的方法
        $scope.onAllSuccess1 = function (ret) {
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            //console.log(ret);
            isCheck1 = 0;
            $scope.fileDatas1 = [];
            $scope.fileList1 = [];
            var tmpData = [];
            for (var i in ret) {
                tmpData.push({
                    name: ret[i].physicalName,
                    size: ret[i].size,
                    type: ret[i].extension,
                    date: ret[i].source.lastModifiedDate,
                    url: ret[i].physicalPath
                });
            }
            
            var tmpRet = angular.copy(tmpData);
            var isNotAll = 0;
            var isSame = 0;
            for (var i = 0; i < tmpRet.length; i++) {
                var compareName = tmpRet[i].name.split('.')[0];
                for (var j = 0; j < tmpRet.length; j++) {
                    var compareName2 = tmpRet[i].name.split('.')[0];
                    if (compareName === compareName2) {
                        isSame++;
                        tmpRet.splice(j, 1);
                        j--;
                    }
                    if (isSame > 3 && j == tmpRet.length - 1) {
                        i--;
                        break;
                    }
                    if (isSame < 4 && j == tmpRet.length - 1) {
                        isNotAll++;
                        break;
                    }
                }
            }
            if (isNotAll > 0 || isSame < 4) {
                waitmask.onHideMask();
                alertFun($filter('translate')('views.Toolset.alertFun.FilesLeast'), $filter('translate')('views.Toolset.alertFun.SelectSameNameUpload'), 'warning', '#007AFF');
                return;
            }

            $scope.fileDatas1 = angular.copy(tmpData);
            var tmpUrl = [];
            for (var i = 0; i < $scope.fileDatas1.length; i++) {
                if ($scope.fileDatas1[i].name.indexOf('.shp') !== $scope.fileDatas1[i].name.length - 4) {
                    $scope.fileDatas1.splice(i, 1);
                    i--;
                    continue;
                }
                var One = angular.copy(ConvertFileList);
                One.PhysicsFilePath = $scope.fileDatas1[i].url;
                tmpUrl.push($scope.fileDatas1[i].url);
                $scope.fileList1.push(One);
            }

            if ($scope.fileList1.length < 1) {
                waitmask.onHideMask();
                alertFun($filter('translate')('views.Toolset.alertFun.shpMissing'), '', 'warning', '#007AFF');
                return;
            }
            //console.log(tmpUrl);

            //获取空间参考的详细信息
            dataConvert.getCoordList(tmpUrl).success(function (data) {
                //console.log(data);
                
                for (var i = 0; i < data.length; i++) {
                    var tempCoord = data[i].split("|");

                    if (tempCoord.length > 1) {
                        $scope.fileDatas1[i].coord = tempCoord[1];
                        $scope.fileDatas1[i].detailCoord = tempCoord[2];
                    }
                    else {
                        $scope.fileDatas1[i].coord = "";
                        $scope.fileDatas1[i].detailCoord = "";
                    }
                }
                waitmask.onHideMask();
            }).error(function (data) {
                //console.log(data);
                for (var i = 0; i < $scope.fileDatas1.length; i++) {
                    $scope.fileDatas1[i].coord = "";
                    $scope.fileDatas1[i].detailCoord = "";
                }
                waitmask.onHideMask();
            });
        }
        //进行转换
        $scope.uploadTwice1 = function () {
            $rootScope.loginOut();
            if ($scope.fileList1.length < 1) {
                alertFun($filter('translate')('views.Toolset.alertFun.AddFirst'), '', 'warning', '#007AFF');
                return;
            }
            if (isCheck1 != 0) {
                alertFun($filter('translate')('views.Toolset.alertFun.UploadedConverted'), '', 'warning', '#007AFF');
                return;
            }
            if (!$scope.outputType1.selected) {
                alertFun($filter('translate')('views.Toolset.alertFun.PleaseSRS'), '', 'warning', '#007AFF');
                return;
            }
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            for (var i = 0; i < $scope.fileList1.length; i++) {
                $scope.fileList1[i].CoordPoint = angular.copy($scope.CoordPoint),
                $scope.fileList1[i].CoordName = $scope.outputType1.selected.value;
                $scope.fileList1[i].FileType = 2;
            }
            dataConvert.dataConvert($scope.fileList1, localStorage.getItem('infoearth_spacedata_userCode'), $scope.outputType1.selected.value, true).success(function (response, status) {
                //console.log(response);

                isCheck1++;
                var isSuccess = 0;
                for (var i = 0; i < response.fileList.length; i++) {
                    if (response.fileList[i].convertMsg != $filter('translate')('views.Toolset.alertFun.ConversionSuccessful')) {
                        alert($filter('translate')('views.Toolset.alertFun.ConversionFailed_1') + (i + 1) + $filter('translate')('views.Toolset.alertFun.ConversionFailed_2'));
                        isSuccess++;
                        continue;
                    }
                }
                waitmask.onHideMask();
                if (isSuccess == 0) {
                    var timeOut = $timeout(function () {
                        alertFun($filter('translate')('views.Toolset.alertFun.ConversionSuccessful'), $filter('translate')('views.Toolset.alertFun.DownloadNotification'), 'success', '#007AFF');
                    }, 1000);
                }
                else {
                    $timeout.cancel(timeOut);
                }

                //发出刷新下载列表数量的请求
                $scope.$emit('$getFileNum', 'coordinate');

            }).error(function (response, status) {
                //console.log(response);
                waitmask.onHideMask();
                alertFun($filter('translate')('views.Toolset.alertFun.ConversionFailed'), '', 'error', '#007AFF');
            });
        }

        //7参数设置弹窗
        $scope.sevenParam = {
            title:'',
            dx: '',
            dy: '',
            dz: '',
            x: '',
            y: '',
            z: '',
            ppm: '',
            open: function () {
                $scope.sevenParam.title = $filter('translate')('views.Toolset.CoordinateConversion.SetParameters');
            },
            submit: function (modalInstance) {
                $scope.sevenParam.dx = isNull($scope.sevenParam.dx);
                $scope.sevenParam.dy = isNull($scope.sevenParam.dy);
                $scope.sevenParam.dz = isNull($scope.sevenParam.dz);
                $scope.sevenParam.x = isNull($scope.sevenParam.x);
                $scope.sevenParam.y = isNull($scope.sevenParam.y);
                $scope.sevenParam.z = isNull($scope.sevenParam.z);
                $scope.sevenParam.ppm = isNull($scope.sevenParam.ppm);
                $scope.CoordPoint = [];
                $scope.CoordPoint.push($scope.sevenParam.dx, $scope.sevenParam.dy, $scope.sevenParam.dz, $scope.sevenParam.x, $scope.sevenParam.y, $scope.sevenParam.z, $scope.sevenParam.ppm);
                
                modalInstance.close();
            },
            cancel: function () { }
        }

        //判断参数是否为空
        function isNull(str) {
            if (!str) {
                return "0";
            }
            return str;
        }


        //控制点参数--所有文件上传完之后执行的方法
        $scope.onAllSuccess2 = function (ret) {
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            //console.log(ret);
            isCheck2 = 0;
            $scope.fileDatas2 = [];
            $scope.fileList2 = [];
            var tmpData = [];
            for (var i in ret) {
                tmpData.push({
                    name: ret[i].physicalName,
                    size: ret[i].size,
                    type: ret[i].extension,
                    date: ret[i].source.lastModifiedDate,
                    url: ret[i].physicalPath
                });
            }

            var tmpRet = angular.copy(tmpData);
            var isNotAll = 0;
            var isSame = 0;
            for (var i in tmpRet) {
                var compareName = tmpRet[i].name.split('.')[0];
                for (var j = 0; j < tmpRet.length; j++) {
                    var compareName2 = tmpRet[i].name.split('.')[0];
                    if (compareName === compareName2) {
                        isSame++;
                        tmpRet.splice(j, 1);
                        j--;
                    }
                    if (isSame > 3 && j == tmpRet.length - 1) {
                        i--;
                        break;
                    }
                    if (isSame < 4 && j == tmpRet.length - 1) {
                        isNotAll++;
                        break;
                    }
                }
            }
            if (isNotAll > 0 || isSame < 4) {
                waitmask.onHideMask();
                alertFun($filter('translate')('views.Toolset.alertFun.FilesLeast'), $filter('translate')('views.Toolset.alertFun.SelectSameNameUpload'), 'warning', '#007AFF');
                return;
            }

            $scope.fileDatas2 = angular.copy(tmpData);
            var tmpUrl = [];
            for (var i = 0; i < $scope.fileDatas2.length; i++) {
                if ($scope.fileDatas2[i].name.indexOf('.shp') !== $scope.fileDatas2[i].name.length - 4) {
                    $scope.fileDatas2.splice(i, 1);
                    i--;
                    continue;
                }
                var One = angular.copy(ConvertFileList);
                One.PhysicsFilePath = $scope.fileDatas2[i].url;
                tmpUrl.push($scope.fileDatas2[i].url);
                $scope.fileList2.push(One);
            }

            if ($scope.fileList2.length < 1) {
                waitmask.onHideMask();
                alertFun($filter('translate')('views.Toolset.alertFun.shpMissing'), '', 'warning', '#007AFF');
                return;
            }
            //console.log(tmpUrl);

            //获取空间参考的详细信息
            dataConvert.getCoordList(tmpUrl).success(function (data) {
                //console.log(data);

                for (var i = 0; i < data.length; i++) {
                    var tempCoord = data[i].split("|");

                    if (tempCoord.length > 1) {
                        $scope.fileDatas2[i].coord = tempCoord[1];
                        $scope.fileDatas2[i].detailCoord = tempCoord[2];
                    }
                    else {
                        $scope.fileDatas2[i].coord = "";
                        $scope.fileDatas2[i].detailCoord = "";
                    }
                }
                waitmask.onHideMask();
            }).error(function (data) {
                //console.log(data);
                for (var i = 0; i < $scope.fileDatas2.length; i++) {
                    $scope.fileDatas2[i].coord = "";
                    $scope.fileDatas2[i].detailCoord = "";
                }
                waitmask.onHideMask();
            });
        }
        //进行转换
        $scope.uploadTwice2 = function () {
            $rootScope.loginOut();
            if ($scope.fileList2.length < 1) {
                alertFun($filter('translate')('views.Toolset.alertFun.AddFirst'), '', 'warning', '#007AFF');
                return;
            }
            if (isCheck2 != 0) {
                alertFun($filter('translate')('views.Toolset.alertFun.UploadedConverted'), '', 'warning', '#007AFF');
                return;
            }
            if (!$scope.outputType1.selected) {
                alertFun($filter('translate')('views.Toolset.alertFun.PleaseSRS'), '', 'warning', '#007AFF');
                return;
            }
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            for (var i = 0; i < $scope.fileList2.length; i++) {
                $scope.fileList2[i].CoordPoint = angular.copy($scope.CoordPoint),
                $scope.fileList2[i].CoordName = $scope.outputType2.selected.value;
                $scope.fileList2[i].FileType = 2;
            }

            //console.log($scope.fileList2);

            dataConvert.dataConvert($scope.fileList2, localStorage.getItem('infoearth_spacedata_userCode'), $scope.outputType2.selected.value, true).success(function (response, status) {
                //console.log(response);

                isCheck2++;
                var isSuccess = 0;
                for (var i = 0; i < response.fileList.length; i++) {
                    if (response.fileList[i].convertMsg != $filter('translate')('views.Toolset.alertFun.ConversionSuccessful')) {
                        alert($filter('translate')('views.Toolset.alertFun.ConversionFailed_1') + (i + 1) + $filter('translate')('views.Toolset.alertFun.ConversionFailed_2'));
                        isSuccess++;
                        continue;
                    }
                }
                if (isSuccess == 0) {
                    var timeOut = $timeout(function () {
                        alertFun($filter('translate')('views.Toolset.alertFun.ConversionSuccessful'), $filter('translate')('views.Toolset.alertFun.DownloadNotification'), 'success', '#007AFF');
                    }, 1000);
                }
                else {
                    $timeout.cancel(timeOut);
                }
                waitmask.onHideMask();

                //发出刷新下载列表数量的请求
                $scope.$emit('$getFileNum', 'coordinate');

            }).error(function (response, status) {
                //console.log(response);
                waitmask.onHideMask();
                alertFun($filter('translate')('views.Toolset.alertFun.ConversionFailed'), '', 'error', '#007AFF');
            });
        }

        //控制点参数设置弹窗
        $scope.pointParam = {
            title: '',
            tableData: [],
            addTips: '',
            deleteTips: '',
            add: function (tr) {
                var tmp = angular.copy(pointParamTableData);
                tmp.id = getRandom();
                for (var i = 0; i < $scope.pointParam.tableData.length; i++) {
                    if (tr.id === $scope.pointParam.tableData[i].id) {
                        $scope.pointParam.tableData.splice(i + 1, 0, tmp);
                        break;
                    }
                }
            },
            del: function (tr) {
                //console.log(tr);
                SweetAlert.swal({
                    title: $filter('translate')('setting.delete'),
                    text: $filter('translate')('setting.confirmDel'),
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: $filter('translate')('setting.sure'),
                    cancelButtonText: $filter('translate')('setting.cancel')
                }, function (isConfirm) {
                    if (isConfirm) {
                        for (var i = 0; i < $scope.pointParam.tableData.length; i++) {
                            if (tr.id === $scope.pointParam.tableData[i].id) {
                                $scope.pointParam.tableData.splice(i, 1);
                                break;
                            }
                        }
                    }
                });
            },
            open: function () {
                $scope.pointParam.title = $filter('translate')('views.Toolset.CoordinateConversion.ControlSetting');
                $scope.pointParam.addTips = $filter('translate')('views.Toolset.CoordinateConversion.AddLine');
                $scope.pointParam.deleteTips = $filter('translate')('views.Toolset.CoordinateConversion.DeleteLine');
            },
            submit: function (modalInstance) {
                $scope.CoordPoint = [];
                for (var i = 0 ; i < $scope.pointParam.tableData.length; i++) {
                    var tmp = $scope.pointParam.tableData[i];
                    if (!tmp.afterX || !tmp.afterY || !tmp.beforeX || !tmp.beforeY) {
                        alertFun($filter('translate')('views.Toolset.alertFun.setCompleted'), '', 'warning', '#007AFF');
                        return;
                    }
                    var tmpArr = [];
                    tmpArr.push(tmp.afterX, tmp.afterY, tmp.beforeX, tmp.beforeY);
                    $scope.CoordPoint.push(tmpArr.toString());
                }
                //console.log($scope.CoordPoint);
                modalInstance.close();
            },
            cancel: function () { }
        }

        //控制点参数设置弹窗中表格里面的数据类型
        var pointParamTableData = {
            id: '',
            beforeX: 0,
            beforeY: 0,
            beforeZ: 0,
            afterX: 0,
            afterY: 0,
            afterZ: 0
        }

        //展示空间参考中的详细信息
        $scope.showDetail = function (element, e, detailCoord) {
            detailCoord = detailCoord.replace(/"/g, "&nbsp");
            $('.tooltip-inner').html(detailCoord);

            var parentRight = element.target.getBoundingClientRect().left - $('#sidebar')[0].getBoundingClientRect().width;
            var a = parentRight - 151;

            $scope.detailTop = {
                "top": 185 + e * 35 + "px",
                "left": a + "px"
            }
            $scope.isSpaceEyes = true;
        }
        $scope.itselfShow = function () {
            $scope.isSpaceEyes = true;
        }
        $scope.downDetail = function () {
            $scope.isSpaceEyes = false;
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

        function alertFun(title, text, type, color) {
            SweetAlert.swal({
                title: title,
                text: text,
                type: type,
                confirmButtonColor: color
            });
        }
    }]);
