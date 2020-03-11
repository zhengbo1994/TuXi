'use strict';
app.controller('shadowCtrl', ['$scope', '$document', '$element', 'selfadapt', '$rootScope', '$http', '$timeout', '$compile', '$modal', 'Upload', 'SweetAlert', 'abp.services.app.dataConvert', '$interval', 'waitmask', '$filter',
    function ($scope, $document, $element, selfadapt, $rootScope, $http, $timeout, $compile, $modal, Upload, SweetAlert, dataConvert, $interval, waitmask, $filter) {
        $rootScope.loginOut();
        $rootScope.homepageStyle = {};
        //调用实时随窗口高度的变化而改变页面内容高度的服务
        var unlink = selfadapt.anyChange($element);
        $scope.$on('$destroy', function () {
            unlink();
            selfadapt.showBodyScroll();
        });

        var isCheck = 0;

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
        $scope.outputType = {};

        //查询配置信息中的默认选中参考系
        for (var i = 0; i < $scope.outputTypeData.length; i++) {
            if ($scope.outputTypeData[i].value === SpatialRefenceFile) {
                $scope.outputType.selected = angular.copy($scope.outputTypeData[i]);
            }
        }
        
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
        $scope.fileDatas = [];
        //传入后台的数据列表
        $scope.fileList = [];

        //所有文件上传完之后执行的方法
        $scope.onAllSuccess = function (ret) {
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            //console.log(ret);
            isCheck = 0;
            $scope.fileDatas = [];
            $scope.fileList = [];
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

            $scope.fileDatas = angular.copy(tmpData);
            var tmpUrl = [];
            for (var i = 0; i < $scope.fileDatas.length; i++) {
                if ($scope.fileDatas[i].name.indexOf('.shp') !== $scope.fileDatas[i].name.length - 4) {
                    $scope.fileDatas.splice(i, 1);
                    i--;
                    continue;
                }
                var One = angular.copy(ConvertFileList);
                One.PhysicsFilePath = $scope.fileDatas[i].url;
                tmpUrl.push($scope.fileDatas[i].url);
                $scope.fileList.push(One);
            }

            if ($scope.fileList.length < 1) {
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
                        $scope.fileDatas[i].coord = tempCoord[1];
                        $scope.fileDatas[i].detailCoord = tempCoord[2];
                    }
                    else {
                        $scope.fileDatas[i].coord = "";
                        $scope.fileDatas[i].detailCoord = "";
                    }
                }
                waitmask.onHideMask();
            }).error(function (data) {
                //console.log(data);
                for (var i = 0; i < $scope.fileDatas.length; i++) {
                    $scope.fileDatas[i].coord = "";
                    $scope.fileDatas[i].detailCoord = "";
                }
                waitmask.onHideMask();
            });
        }
        //进行转换
        $scope.uploadTwice = function () {
            $rootScope.loginOut();
            if ($scope.fileList.length < 1) {
                alertFun($filter('translate')('views.Toolset.alertFun.AddFirst'), '', 'warning', '#007AFF');
                return;
            }
            if (isCheck != 0) {
                alertFun($filter('translate')('views.Toolset.alertFun.UploadedConverted'), '', 'warning', '#007AFF');
                return;
            }
            if (!$scope.outputType.selected) {
                alertFun($filter('translate')('views.Toolset.alertFun.PleaseSRS'), '', 'warning', '#007AFF');
                return;
            }
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            for (var i = 0; i < $scope.fileList.length; i++) {
                $scope.fileList[i].CoordName = $scope.outputType.selected.value;
                $scope.fileList[i].FileType = 3;
            }
            dataConvert.dataConvert($scope.fileList, localStorage.getItem('infoearth_spacedata_userCode'), $scope.outputType.selected.value, true).success(function (response, status) {
                console.log(response);

                isCheck++;
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

        $scope.lastuploadTwice = function () {
            $rootScope.loginOut();
            if (tempFile == undefined || tempFile == "" || tempFile == null) {
                alertFun($filter('translate')('views.Toolset.alertFun.AddFirst'), '', 'warning', '#007AFF');
            }
            else if (isCheck != 0) {
                alertFun($filter('translate')('views.Toolset.alertFun.UploadedConverted'), '', 'warning', '#007AFF');
            }
            else if ($scope.out_Coord == null || $scope.out_Coord == undefined || $scope.out_Coord == "") {
                alertFun($filter('translate')('views.Toolset.alertFun.PleaseSRS'), '', 'warning', '#007AFF');
            }
            else {
                waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);

                dataConvert.dataConvert1(tempFile, localStorage.getItem('infoearth_spacedata_userCode'), 3, $scope.out_Coord, true).success(function (response, status) {
                    isCheck++;
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
                    $scope.$emit('$getFileNum', 'shadow');
                }).error(function (response, status) {
                    //console.log(response);
                    waitmask.onHideMask();
                    alertFun($filter('translate')('views.Toolset.alertFun.ConversionFailed'), '', 'error', '#007AFF');
                });
            }
        }

        //展示空间参考中的详细信息
        $scope.showDetail = function (element, e, detailCoord) {
            detailCoord = detailCoord.replace(/"/g, "&nbsp");
            $('.tooltip-inner').html(detailCoord);

            var parentRight = element.target.getBoundingClientRect().left - $('#sidebar')[0].getBoundingClientRect().width;
            var a = parentRight - 165;

            $scope.detailTop = {
                "top": 153 + e * 35 + "px",
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

        function alertFun(title, text, type, color) {
            SweetAlert.swal({
                title: title,
                text: text,
                type: type,
                confirmButtonColor: color
            });
        }
    }]);
