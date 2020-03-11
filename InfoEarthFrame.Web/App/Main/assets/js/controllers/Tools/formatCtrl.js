'use strict';
app.controller('formatCtrl', ['$scope', '$rootScope', '$document', '$element', 'selfadapt', '$http', '$timeout', '$compile', "$modal", 'Upload', 'SweetAlert', 'abp.services.app.dataConvert', '$interval', 'waitmask', 'abp.services.app.dicDataCode', '$filter',
    function ($scope, $rootScope, $document, $element, selfadapt, $http, $timeout, $compile, $modal, Upload, SweetAlert, dataConvert, $interval, waitmask, dicDataCode, $filter) {
        debugger
        $rootScope.loginOut();
        $rootScope.homepageStyle = {};
        //调用实时随窗口高度的变化而改变页面内容高度的服务
        var unlink = selfadapt.anyChange($element);
        $scope.$on('$destroy', function () {
            unlink();
            selfadapt.showBodyScroll();
        });
        
        var isCheck1 = 0;
        var isCheck2 = 0;

        $scope.isEnglish = language_English;

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

        //切换tab
        $scope.showtab1 = true;
        if ($scope.isEnglish == 1) {
            $scope.showtab1 = false;
        }
        
        $scope.selectTab = function (state) {
            $scope.showtab1 = state;
        }

        //上传的文件列表
        $scope.fileDatas1 = [];
        $scope.fileDatas2 = [];
        //传入后台的数据列表
        $scope.fileList1 = [];
        $scope.fileList2 = [];
        
        $scope.onAllSuccess1 = function (ret) {
            debugger
            console.log(ret);
            isCheck1 = 0;
            $scope.fileDatas1 = [];
            $scope.fileList1 = [];
            for (var i in ret) {
                $scope.fileDatas1.push({
                    name: ret[i].physicalName,
                    size: ret[i].size,
                    type: ret[i].extension,
                    date: ret[i].source.lastModifiedDate,
                });
                var One = angular.copy(ConvertFileList);
                One.PhysicsFilePath = ret[i].physicalPath;

                $scope.fileList1.push(One);
            }
        }
        //开始格式转换
        $scope.uploadTwice1 = function () {
            debugger
            $rootScope.loginOut();
            if ($scope.fileList1.length < 1) {
                alertFun($filter('translate')('views.Toolset.alertFun.UploadFiles'), '', 'warning', '#007AFF');
                return;
            }
            if (isCheck1 != 0) {
                alertFun($filter('translate')('views.Toolset.alertFun.UploadedConverted'), $filter('translate')('views.Toolset.alertFun.reupload'), 'warning', '#007AFF');
                return;
            }
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            for (var i = 0; i < $scope.fileList1.length; i++) {
                $scope.fileList1[i].ConvertKey = "";
                $scope.fileList1[i].CoordName = "NULL";
                $scope.fileList1[i].FileType = 1;
            }
            //console.log($scope.fileList);

            dataConvert.dataConvert($scope.fileList1, localStorage.getItem('infoearth_spacedata_userCode'), "NULL", true).success(function (response, status) {
                isCheck1++;
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

                //发出刷新下载列表数量的请求
                $scope.$emit('$getFileNum', 'format');

                waitmask.onHideMask();
            }).error(function (response, status) {
                //console.log(response);
                waitmask.onHideMask();
                alertFun($filter('translate')('views.Toolset.alertFun.ConversionFailed'), '', 'error', '#007AFF');
            });
        }

        $scope.prompt = function () {
            alertFun($filter('translate')('views.Toolset.alertFun.SelectFormat'), '', 'warning', '#007AFF');
        }

        //选择输出文件格式
        $scope.outputType = "";
        $scope.choseOutType = function () {
            $scope.choseType.title = $filter('translate')('views.Toolset.FormatConversion.Tips_4');
            $scope.choseType.formatData = angular.copy($scope.formatType);
            $scope.openChoseTypeFun();
        }

        $scope.onAllSuccess2 = function (ret) {
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
                alertFun($filter('translate')('views.Toolset.alertFun.FilesLeast'), $filter('translate')('views.Toolset.alertFun.SelectSameNameUpload'), 'warning', '#007AFF');
                return;
            }

            $scope.fileDatas2 = angular.copy(tmpData);
            var tmpUrl = [];
            for (var i = 0; i < $scope.fileDatas2.length; i++) {
                if ($scope.fileDatas2[i].name.indexOf('.shp') !== $scope.fileDatas2[i].name.length - 4 && $scope.fileDatas2[i].name.indexOf('.tab') !== $scope.fileDatas2[i].name.length - 4) {
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
                alertFun($filter('translate')('views.Toolset.alertFun.missing'), '', 'warning', '#007AFF');
                return;
            }
        }

        //开始格式转换
        $scope.uploadTwice2 = function () {
            $rootScope.loginOut();
            if (!$scope.outputType) {
                alertFun($filter('translate')('views.Toolset.alertFun.SelectFirst'), '', 'warning', '#007AFF');
                return;
            }
            if ($scope.fileList2.length < 1) {
                alertFun($filter('translate')('views.Toolset.alertFun.UploadFiles'), '', 'warning', '#007AFF');
                return;
            }
            if (isCheck2 != 0) {
                alertFun($filter('translate')('views.Toolset.alertFun.UploadedConverted'), $filter('translate')('views.Toolset.alertFun.reupload'), 'warning', '#007AFF');
                return;
            }
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            for (var i = 0; i < $scope.fileList2.length; i++) {
                $scope.fileList2[i].ConvertKey = $scope.outputType;
                $scope.fileList2[i].CoordName = "NULL";
                $scope.fileList2[i].FileType = 1;
            }
            //console.log($scope.fileList);

            dataConvert.dataConvert($scope.fileList2, localStorage.getItem('infoearth_spacedata_userCode'), "NULL", true).success(function (response, status) {
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

                //发出刷新下载列表数量的请求
                $scope.$emit('$getFileNum', 'format');

                waitmask.onHideMask();
            }).error(function (response, status) {
                //console.log(response);
                waitmask.onHideMask();
                alertFun($filter('translate')('views.Toolset.alertFun.ConversionFailed'), '', 'error', '#007AFF');
            });
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

        //选择上传/输出格式弹窗
        $scope.choseType = {
            title: "",
            formatData: [],
            chosedOne: {},
            choseOne:function(row, key){
                //console.log(row, key);
                $scope.choseType.chosedOne = angular.copy(row);
                for (var i = 0; i < $scope.choseType.formatData.length; i++) {
                    if ($scope.choseType.formatData[i].id !== row.id) {
                        $scope.choseType.formatData[i].selected = false;
                    }
                }
            },
            open: function () { },
            submit: function (modalInstance) {
                if ($scope.choseType.title === $filter('translate')('views.Toolset.FormatConversion.Tips_4')) {
                    $scope.outputType = $scope.choseType.chosedOne.codeName;
                }
                
                modalInstance.close();
            },
            cancel: function () { }
        }

        $scope.formatType = [];
        //获取所有可选择的文件格式信息
        dicDataCode.getDetailByTypeID('25159792-cdba-11e7-a735-005056bb1c7e').success(function (data, status) {
            //console.log(data, status);
            $scope.formatType = angular.copy(data.items);
        }).error(function (data, status) {
            console.log(data, status);
        });

        function alertFun(title, text, type, color) {
            SweetAlert.swal({
                title: title,
                text: text,
                type: type,
                confirmButtonColor: color
            });
        }
    }]);

