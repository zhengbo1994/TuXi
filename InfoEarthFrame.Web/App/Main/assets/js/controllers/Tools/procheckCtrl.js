'use strict';

app.controller('procheckCtrl', ['$scope', '$rootScope', '$http', '$timeout', '$compile', 'Upload', 'SweetAlert', '$interval', 'abp.services.app.dataConvert',
    function ($scope, $rootScope, $http, $timeout, $compile, Upload, SweetAlert, $interval, dataConvert) {
        $rootScope.loginOut();

    $scope.$prochecklog = [];
    var countLog = 1;
    var isCheck = 0;
    var tempFile = new Array();

    $scope.$watch('files', function (newValue, oldValue) {
        if (newValue != oldValue && newValue != "" && newValue != undefined && (oldValue == "" || oldValue == undefined)) {
            var tempValue = newValue.concat();
            var compareName = "";
            var isNotAll = 0;
            for (var i = 0; i < tempValue.length; i++) {
                var isSame = 0;
                var tempName1 = tempValue[i].name.split(".");
                compareName = tempName1[0];

                for (var j = 0; j < tempValue.length; j++) {
                    var tempName2 = tempValue[j].name.split(".");
                    if (compareName === tempName2[0]) {
                        isSame++;
                        tempValue.splice(j, 1);
                        j--;
                    }
                    if (isSame > 3 && j == tempValue.length - 1) {
                        i--;
                        break;
                    }
                    if (isSame < 4 && j == tempValue.length - 1) {
                        isNotAll++;
                        break;
                    }
                }
            }

            if (isNotAll > 0) {
                SweetAlert.swal({
                    title: "同名文件至少有四个！",
                    text: "请选择同名的所有文件进行上传，否则将无法进行后续操作！",
                    type: "error",
                    confirmButtonColor: "#007AFF"
                });
            }
            else {
                waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
                tempFile = [];
                $scope.$prochecklog = $scope.$prochecklog.concat((countLog++ + ".") + "开始上传文件...");
                firstUpload();
                isCheck = 0;
            }
        }
    });

    //选中文件之后直接上传，读取相关信息显示在页面的表格中
    function firstUpload() {
        $scope.$lyrData = [];
        Upload.upload({
            url: '/Handle/UpLoadFileHandle.ashx',
            file: $scope.files
        }).then(function (response) {
            $scope.files = [];
            var PhysicsFilePath = new Array();
            var j = 0;

            PhysicsFilePath = response.data.Filepath;

            for (var i = 0; i < PhysicsFilePath.length; i++) {
                var temp = new Array();
                temp = PhysicsFilePath[i].split(".");
                if (temp[temp.length - 1] == "shp") {
                    tempFile[j] = PhysicsFilePath[i];
                    j++;
                    $scope.$prochecklog = $scope.$prochecklog.concat((countLog++ + ".") + PhysicsFilePath[i]);
                }
            }

            if (tempFile == "" || tempFile == null) {
                SweetAlert.swal({
                    title: "shp文件丢失！",
                    type: "error",
                    confirmButtonColor: "#007AFF"
                });
            }
            else {
                dataConvert.getlyrTypeList(tempFile).success(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        var tempLyr = {
                            "name": "",
                            "class": "",
                            "tjname": "",
                            "tcname": "",
                            "status": "未检查"
                        };
                        var tempData = data[i].split(",");
                        tempLyr.name = tempData[0];
                        tempLyr.class = tempData[1];
                        tempLyr.tjname = tempData[2];
                        tempLyr.tcname = tempData[3];
                        $scope.$lyrData = $scope.$lyrData.concat(tempLyr);
                    }

                    $scope.$prochecklog = $scope.$prochecklog.concat((countLog++ + ".") + "上传文件完成。");
                    waitmask.onHideMask();
                }).error(function (data) {
                    //console.log(data);
                });
            }

        }, function (resp) {
            $scope.$prochecklog = $scope.$prochecklog.concat((countLog++ + ".") + "上传文件错误。");
            waitmask.onHideMask();
            
            //console.log('ERROR:' + resp.status);
            $scope.files = [];
        }, function (evt) {
        });
    }

    $scope.uploadTwice = function () {
        $rootScope.loginOut();
        if (tempFile == undefined || tempFile == "" || tempFile == null) {
            SweetAlert.swal({
                title: "请先添加文件！",
                type: "error",
                confirmButtonColor: "#007AFF"
            });
        }
        else if (isCheck != 0) {
            SweetAlert.swal({
                title: "上传的文件已经检查完毕！",
                type: "error",
                confirmButtonColor: "#007AFF"
            });
        }
        else {
            //创建遮罩
            waitmask.onShowMask($filter('translate')('views.Toolset.waitMask.Wait'), 300);
            $scope.$prochecklog = $scope.$prochecklog.concat((countLog++ + ".") + "开始检查文件...");

            dataConvert.dataCheck(tempFile, true).success(function (response, status) {
                isCheck++;
                for (var i = 0; i < $scope.$lyrData.length; i++) {
                    $scope.$lyrData[i].status = "检查通过";
                    $("#sample-table-1 tr:eq(" + (i + 1) + ") td:eq(5)").css('cssText', 'color:green !important');
                }

                if (response.checkInfoList.length > 0) {
                    for (var i = 0; i < response.checkInfoList.length; i++) {
                        var name = response.checkInfoList[i].fileName.split("\\");

                        for (var k = 0; k < $scope.$lyrData.length; k++) {
                            var tempname = $scope.$lyrData[k].name.split("\\");

                            if (name[name.length - 1] == tempname[tempname.length - 1]) {
                                var msg = response.checkInfoList[i].log[0]
                                if (msg === "不在检查范围内") {
                                    $scope.$lyrData[k].status = msg;
                                    $("#sample-table-1 tr:eq(" + (k + 1) + ") td:eq(5)").css('cssText', 'color:gray !important');
                                }
                                else {
                                    $scope.$lyrData[k].status = "检查不通过";
                                    $("#sample-table-1 tr:eq(" + (k + 1) + ") td:eq(5)").css('cssText', 'color:red !important');
                                }

                                $scope.$prochecklog = $scope.$prochecklog.concat((countLog++ + ".") + name[name.length - 1] + " ： ");
                                for (var j = 0; j < response.checkInfoList[i].log.length; j++) {
                                    $scope.$prochecklog = $scope.$prochecklog.concat(response.checkInfoList[i].log[j] + "。");
                                }
                            }
                        }
                    }
                }
                
                $scope.downloadHref = "/Handle/DownloadFile.ashx?filename=" + response.excelFile;
                var timeOut = $timeout(function () {
                    SweetAlert.swal({
                        title: "检查完成！",
                        type: "success",
                        confirmButtonColor: "#007AFF"
                    });
                    $scope.$prochecklog = $scope.$prochecklog.concat((countLog++ + ".") + "检查文件完成。");
                }, 1000);
                waitmask.onHideMask();
            }).error(function (data, status) {
                console.log(data);
                for (var i = 0; i < $scope.$lyrData.length; i++) {
                    $scope.$lyrData[i].status = "检查失败";
                    $("#sample-table-1 tr:eq(" + (i + 1) + ") td:eq(5)").css('cssText', 'color:red !important');
                }
                waitmask.onHideMask();
                SweetAlert.swal({
                    title: "检查失败！",
                    type: "error",
                    confirmButtonColor: "#007AFF"
                });
            });
        }
    }
}]);
