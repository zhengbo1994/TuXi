'use strict';

app.controller('downListCtrl', ['$rootScope', '$scope', '$timeout', '$modal', '$http', 'abp.services.app.dataConvert', 'abp.services.app.log', 'abp.services.app.layerReadLog',
    function ($rootScope, $scope, $timeout, $modal, $http, dataConvert, log, layerReadLog) {
        $rootScope.loginOut();
        var conNum = 0;
        var logNum = 0;
        var msgNum = 0;
        $scope.moduleID = '';
        
        layerReadLog.getNodeJSServerConfig().success(function (data, status) {
            //console.log(data);
            $scope.moduleID = data;
            var dto = { CreateBy: localStorage.getItem('infoearth_spacedata_userCode') };
           
            layerReadLog.getDetailByLayer(dto).success(function (data, status) {
                //console.log(data);
                //listenToSocketIO();
            }).error(function (data, status) {
                console.log(data);
            });

            //$.ajax({
            //    url: abp.appPath + 'api/services/app/layerReadLog/GetDetailByLayer',
            //    type: 'POST',
            //    headers:{'Authorization':'Bearer '+AccessTokenValues},
            //    data: JSON.stringify(dto)
            //}).success(function (data, status) {
            //    console.log(data);
            //    listenToSocketIO();
            //});
        }).error(function (data, status) {
            console.log(data);
        });
        
        //下载通知数量userinfo.id
        dataConvert.getConvertFilesNum(localStorage.getItem('infoearth_spacedata_userCode')).success(function (data, status) {
            if (data != 0) {
                conNum = data;
            }
            $scope.allNum = conNum;

            layerReadLog.getDetailByLayer({ CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }).success(function (data, status) {
                //console.log(data);
                
                msgNum = data.items.length;
                $scope.allNum = msgNum + conNum;
            }).error(function (data, status) {
                //console.log(data);
                alertFun("错误!", data.message, 'error', '#007AFF');
            });

            //数据入库通知数量
            //log.getUploadLogNum().success(function (data) {
            //    if (data != 0) {
            //        logNum = data;
            //    }
                
            //    if (conNum + logNum > 0) {
            //        $scope.allNum = conNum + logNum;
            //    }
            //    else {
            //        $scope.allNum = "";
            //    }
            //});
        });

        //工具集转换成功后发出请求调用本方法刷新
        $scope.$onRootScope('$getFileNum', function () {
            dataConvert.getConvertFilesNum(localStorage.getItem('infoearth_spacedata_userCode')).success(function (data, status) {
                if (data != 0) {
                    conNum = data;
                }
                $scope.allNum = conNum + logNum;
            });
        });
        
        $scope.$onRootScope('$getMessageNum', function () {
            layerReadLog.getDetailByLayer({ CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }).success(function (data, status) {
                //console.log(data);

                msgNum = data.items.length;
                $scope.allNum = msgNum + conNum;
            }).error(function (data, status) {
                //console.log(data);
                alertFun("错误!", data.message, 'error', '#007AFF');
            });
        });

        //数据管理导入文件之后发出请求调用本方法刷新
        //$scope.$onRootScope('getLogNum', function () {
        //    $timeout(function () {
        //        //展开树的全部节点
        //        log.getUploadLogNum().success(function (data) {
        //            if (data != 0) {
        //                logNum = data;
        //            }

        //            if (conNum + logNum > 0) {
        //                $scope.allNum = conNum + logNum;
        //            }
        //            else {
        //                $scope.allNum = "";
        //            }
        //        });
        //    }, 2000);
            
        //})

        $scope.open = function () {
            $rootScope.loginOut();

            var modalInstance = $modal.open({
                windowClass: 'conver-result',
                templateUrl: 'downList.html',
                controller: 'downloadListCtrl',
                size: 'lg',
                resolve: {

                }
            });
        }

        //function listenToSocketIO() {
        //    //创建socketIO连接
        //    try {
        //        var iosocket = io.connect($scope.moduleID);//连接SocketIO服务器，命名空间为ModuleID
        //        iosocket.on('connect', function () {
        //            iosocket.emit('subscribe', localStorage.getItem('infoearth_spacedata_userCode'));//监听一个频道，频道名称为username
        //            iosocket.on('send', function (msg) {

        //                console.log(msg);

        //            });

        //            iosocket.on('disconnect', function () {
        //                iosocket = io.connect($scope.moduleID);
        //                //断开重连
        //            });

        //            iosocket.on('error', function (data) {
        //                alert('服务端抛出异常，异常信息:' + data.toString());
        //                //服务端抛出异常
        //            });
        //        });
        //    } catch (e) {
        //        alert("SocketIO服务器连接失败，请刷新后重试！");
        //        //$('#emergencymsg').html('*');
        //        //$('#warningmsg').html('*');
        //        return;
        //    }
        //}
}]);

app.controller('downloadListCtrl', ['$scope', '$http', '$modalInstance', 'SweetAlert', 'abp.services.app.dataConvert', 'abp.services.app.log', 'abp.services.app.layerReadLog',
    function ($scope, $http, $modalInstance, SweetAlert, dataConvert, log, layerReadLog) {
        $scope.isEyes = false;

        //根据用户ID获取通过工具集转换的下载列表文件
        dataConvert.getConvertFilesList(localStorage.getItem('infoearth_spacedata_userCode')).success(function (data, status) {
            //console.log(data);
            for (var i = 0; i < data.length; i++) {
                data[i].physicsFilePath = "/Handle/DownloadFile.ashx?filename=" + data[i].physicsFilePath;

                if (data[i].state === 0) {
                    data[i].styles = {
                        "font-weight": 900
                    }
                }
                else {
                    data[i].styles = {
                        "font-weight": "normal"
                    }
                }
            }
            $scope.fileListOne = angular.copy(data);
            $scope.totalItemsOne = data.length;
            $scope.currentPageOne = 1;
            $scope.pageChangedOne($scope.currentPageOne);
        }).error(function (data, status) {
            //console.log(data);
            alertFun("错误!", data.message, 'error', '#007AFF');
        });


        //将请求回来的下载通知数据分页
        $scope.pageChangedOne = function (one) {
            $scope.$toolDatas = $scope.fileListOne.slice((one - 1) * 10, one * 10);
        }
       
        //展示下载通知中的详细信息
        $scope.showDetail = function (element, e, fileNameList) {
            $scope.$detailFileName = fileNameList.split(",");

            var parentRight = element.target.getBoundingClientRect().left - $('.modal-lg .modal-content')[0].getBoundingClientRect().left;
            //var a = $(".detail-window .tooltip").find("table").width() + 40;
            //if (a == 140) {
            //    a = 200;
            //}
            var a = 200;
            $scope.detailTop = {
                "top": 70 + e * 37 + 72 + "px",
                "left": parentRight - a / 2 - 15 + "px"
            }

            $scope.isEyes = true;
        }

        $scope.itselfShow = function () {
            $scope.isEyes = true;
        }
        $scope.downDetail = function () {
            $scope.isEyes = false;
        }

        //下载通知中的下载操作
        $scope.changeStatus = function (da) {
            if (da.state === 0) {
                dataConvert.updateState(da.id).success(function () {
                }).error(function () {
                });
                da.styles = {
                    "font-weight": "normal"
                }
            }
            window.open(da.physicsFilePath);
        }


        //导入通知
        $scope.$importDatas = [];

        //导入通知的分页
        //$scope.maxSize2 = 5;//页码个数显示数
        //$scope.goPage2 = 1;//转到多少页
        //$scope.pageCounts2 = 0;//32;//总条数
        //$scope.pageIndex2 = 1;//1;//起始页
        //$scope.pageSize2 = 10;//10;//每页显示条数
        ////导入通知的分页方法
        //$scope.pageChanged2 = function (a, evt) {
        //    if (evt && evt.keyCode !== 13) { return; }//注：回车键为13
        //    if (a) {
        //        a = parseInt(a);
        //        if (isNaN(a) || a < 1 || a > $scope.totalPages2) {
        //            $scope.goPage2 = $scope.pageIndex2;
        //            return;
        //        }
        //        $scope.goPage2 = a;
        //        $scope.pageIndex2 = a;
        //    }

        //    getPageData2($scope.pageIndex2, $scope.pageSize2);
        //};

        //关闭消息通知弹窗
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        }

        //弹出导入通知的结果信息
        $scope.seeMsg = function (status, msg) {
            if (status == 1) {
                alertFun("导入成功!", '', 'success', '#007AFF');
            }
            if (status == 2) {
                alertFun("导入失败!", msg, 'error', '#007AFF');
            }
        }

        layerReadLog.updataMsgReadStatus(localStorage.getItem('infoearth_spacedata_userCode')).success(function (data, status) {
            //console.log(data);
            $scope.$emit('$getMessageNum', 'layerReadLog');
        }).error(function (data, stauts) {
            //console.log(data);
            alertFun("错误!", data.message, 'error', '#007AFF');
        });

        //导入通知的分页方法
        function getPageData2(index, size) {
            layerReadLog.getDetailByLayer({ CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }).success(function (data, status) {
                //console.log(data);
                //if (typeof (data) === "string") {
                //    data = JSON.parse(data);
                //}
                $scope.$importDatas = data.items;
                for (var i = 0; i < $scope.$importDatas.length; i++) {
                    $scope.$importDatas[i].downloadUrl = "";
                    if (!!$scope.$importDatas[i].message && $scope.$importDatas[i].message.indexOf("Excel##") > -1) {
                        var tmpStr = $scope.$importDatas[i].message.split("##");
                        $scope.$importDatas[i].message = tmpStr[0];
                        $scope.$importDatas[i].downloadUrl = tmpStr[1];
                    }
                }
            }).error(function (data, status) {
                //console.log(data);
                alertFun("查询导入通知失败!", data.message, 'error', '#007AFF');
            });
        }
        getPageData2($scope.pageIndex2, $scope.pageSize2);


        function alertFun(title, text, type, color) {
            SweetAlert.swal({
                title: title,
                text: text,
                type: type,
                confirmButtonColor: color
            });
        }

        //获取数据管理导入文件的结果记录
        //log.getUploadLog().success(function (data) {
        //    console.log(data);
        //    for (var i = 0; i < data.length; i++) {
        //        if (data[i].logContent != null) {
        //            if (data[i].logContent.indexOf("上传成功") > -1) {
        //                data[i].logResult = "入库成功";
        //            }
        //            else if (data[i].logContent.indexOf("正在上传") > -1) {
        //                data[i].logResult = "正在上传";
        //            }
        //            else {
        //                data[i].logResult = "入库失败";
        //            }

        //            if (data[i].other1 == "0") {
        //                data[i].styles = {
        //                    "font-weight": 900
        //                }
        //            }
        //            else {
        //                data[i].styles = {
        //                    "font-weight": "normal"
        //                }
        //            }

        //            if (data[i].logKey != null && data[i].logKey.length > 0) {
        //                var tempNum = "";
        //                var scaleNum = data[i].logKey;
        //                if (scaleNum.length < 4) {
        //                    tempNum = scaleNum.concat();
        //                }
        //                else {
        //                    for (var j = 3; j < scaleNum.length; j += 3) {
        //                        var weizhi = scaleNum.length - j;
        //                        var tmpfirst = scaleNum.substring(0, weizhi);
        //                        var tmplast = scaleNum.substring(weizhi, scaleNum.length);
        //                        scaleNum = tmpfirst.concat(" ");
        //                        scaleNum = scaleNum.concat(tmplast);
        //                        j++;
        //                    }
        //                    tempNum = scaleNum.concat();
        //                }

        //                data[i].logKey = "1：" + tempNum;
        //            }
        //        }
        //    }
        //    $scope.fileListTwo = data;
        //    $scope.totalItemsTwo = data.length;
        //    $scope.currentPageTwo = 1;
        //    $scope.$logDatas = $scope.fileListTwo.slice(0, 10);
        //}).error(function (data) {
        //    console.log(data);
        //});
        //$scope.pageChangedTwo = function (two) {
        //    $scope.$logDatas = $scope.fileListTwo.slice((two - 1) * 10, two * 10);
        //}

        //弹出入库结果的详细信息
        //$scope.showLog = function (index, da) {
        //    alert(da.logContent);
        //    if (da.other1 === "0") {
        //        var arr = new Array();
        //        arr.push(da.id);
        //        log.updateStatus(arr).success(function () {
        //        }).error(function () {
        //        });
        //        da.styles = {
        //            "font-weight": "normal"
        //        }
        //    }
        //}

}]);



