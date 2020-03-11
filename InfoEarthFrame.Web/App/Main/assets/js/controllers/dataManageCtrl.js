/**
 * Created by admin on 2016/8/8.
 */
app.controller('dataManageCtrl', ["$rootScope", "$scope", "$http", "$timeout", "$state", "$log", "Upload", "$modal", "abp.services.app.geologyMappingType", "$localStorage", "abp.services.app.dataManage", "SweetAlert", "select",
function ($rootScope, $scope, $http, $timeout, $state, $log, Upload, $modal, geologyMappingType, $localStorage, dataManage, SweetAlert, select) {
    /*树结构对象赋值-----start----*/
    //左侧的树
    $scope.my_data = [];
    $scope.initialSelection = '';
    if ($rootScope.my_datas) {
        $scope.my_data = angular.copy($rootScope.my_datas);
        $scope.initialSelection = angular.copy($scope.my_data[0].label);
    }
    /*树结构对象赋值-----end------*/
    $scope.treeBranch = [];
    /*树结构已经在后台生成，此屏蔽部分在做完测试后请删除
    function treeFormat(item) {
        var treedata_avm = {};
        treedata_avm.label = item.className;
        treedata_avm.uid = item.id;
        treedata_avm.parent_uid = item.parentID;
        treedata_avm.level = item.paths.split('.').length;
        treedata_avm.sn = item.sn;
        treedata_avm.children = [];
        return treedata_avm;
    };
    function sortBySn(arr) {
        var temp = [];
        if (arr.length > 0) {
            arr.sort(function (a, b) {
                return a.sn - b.sn;
            });
            temp = arr;
            angular.forEach(arr, function (item) {
                item.children = sortBySn(item.children);
            });
        };
        return temp;
    };
    function transData(a, idStr, pidStr, childrenStr) {
        var r = [], hash = {}, id = idStr, pid = pidStr, children = childrenStr, i = 0, j = 0, len = a.length;
        for (; i < len; i++) {
            hash[a[i][id]] = a[i];
        };
        for (; j < len; j++) {
            var aVal = a[j], hashVP = hash[aVal[pid]];
            if (hashVP) {
                !hashVP[children] && (hashVP[children] = []);
                hashVP[children].push(aVal);
            } else {
                r.push(aVal);
            };
        };
        return r;
    };
    function getTree() {
        var temp = [];
        angular.forEach($scope.data, function (val) {
            temp.push(treeFormat(val));
        });
        $scope.my_data = transData(temp, 'uid', 'parent_uid', 'children');
        $scope.my_data = sortBySn($scope.my_data);
    }
    if (angular.isDefined($localStorage.treedata)) {
        $scope.data = $localStorage.treedata;
        getTree();
    } else {
        geologyMappingType.getGeologyMappingTypeList().success(function (data) {
            $scope.data = data;
            $localStorage.treedata = $scope.data;
            getTree();
        });
    };*/
    /*-----------------------文件上传部分开始-----------------------*/
    $scope.openUpload = function () {
        if ($scope.treeBranch == "" || $scope.treeBranch.length == 0 || $scope.treeBranch == null) {
            alert("没有选中任意一级节点");
        }
        else {
            var modalInstance = $modal.open({
                windowClass: 'conver-result',
                templateUrl: 'openUpload.html',
                controller: 'dataManageUploadCtrl',
                size: 'lg',
                resolve: {
                    items: function () {
                        return $scope.treeBranch;
                    },
                    allItems: function () {
                        return $rootScope.my_datas;
                    }
                }
            });
        }
    }

    /*-----------------------文件上传部分结束-----------------------*/

    $scope.file = { name: "" };
    $scope.startDate = moment().subtract(15, 'days').format('YYYY-MM-DD');
    $scope.endDate = moment().add(1, 'days').format('YYYY-MM-DD');
    $scope.$on("dataStorageStartTimeChange", function (event, data) {
        $scope.startDate = data.getFullYear() + "-" + (data.getMonth() + 1) + "-" + data.getDate();
    });
    $scope.$on("dataStorageEndTimeChange", function (event, data) {
        $scope.endDate = data.getFullYear() + "-" + (data.getMonth() + 1) + "-" + data.getDate();
    });
    $scope.params = {};
    $scope.currentPage = 1;
    $scope.pageSize = 10;
    $scope.totalItems = 10;
    $scope.thisId = "";

    //元数据页面跳转
    $scope.goState = function (id) {
        select.metaDataId = id;
        //window.open($state.href('app.dataManage.datasource', { dataId: id }));
        var modalInstance = $modal.open({
            templateUrl: 'datasource.html',
            controller: 'dataSourceCtrl',
            size: 'lg',
        });
    };
    //详细信息页面跳转
    $scope.goDetailPage = function (id, zipFileName, metaID) {
        $rootScope.progressAllShow = true;//显示跳转进度条
        select.setWebPage = {
            type: false,
            title: '详细信息',
            toLeftStyle: 'margin-left:210px',
            isTreeShow: true,
            isPanelHeadShow: true,
            showMetaData: false
        };

        select.fileId = id;
        select.fileName = zipFileName;
        select.metaDataId = metaID;
        var modalInstance = $modal.open({
            templateUrl: 'dataDetail.html',
            controller: 'dataDetailCtrl',
            size: 'lg',
        });
        //window.open($state.href('app.dataManage.dataManageDetail', { fileId: id, fileName: zipFileName, metaDataId: metaID }));
    }
    //数据编辑页面调用程序
    $scope.goDataPage = function (itemId) {// + "&" + userinfo.id + "&" + AccessTokenValues + "\\"
        var hhref = "geologicmap:\\\\" + itemId;
        window.location.href = hhref;
    }
    //专题制作页面调用程序
    $scope.goSpecialPage = function (itemId) {
        console.log(itemId);
        var hhref = "ThematicMapping:\\\\" + itemId;
        window.location.href = hhref;
    }
    //下载调用程序
    $scope.download = function (itemId) {
        SweetAlert.swal({
            title: "下载须知",
            text: "本网站所有内容由中国地质环境监测院发布，用户因使用本网站提供的所有内容（包括第三方服务）而引起的各类问题和责任事故不承担任何责任。",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "确定",
            cancelButtonText: "取消"
        }, function (isConfirm) {
            if (isConfirm) {

                dataManage.getZipFile(itemId).success(function (cons) {
                    var name = "/Handle/DownloadFile.ashx?filename=" + cons;
                    window.open(name);
                });
            } else {
                console.log("你取消了该操作！");
            }
        });
    };

    function getParams() {
        var params = {
            GeologyMappingTypeID: '',
            FileName: $scope.file.name,
            StartDate: '',
            EndDate: '',
            currentPage: $scope.currentPage,
            pageSize: $scope.pageSize
        };
        return params;
    };
    function getFieList(params) {
        dataManage.getDataMainList(params).success(function (data) {
            $scope.fileList = [];
            console.log(data);
            angular.forEach(data.rowList, function (item) {
                $scope.fileList.push(item);
            });
            $scope.totalItems = data.rowNum;
        });
    };
    $scope.my_tree_handler = function (branch) {
        console.log(branch);
        //创建遮罩
        abp.ui.block($('#app'));

        $scope.params = getParams();
        $scope.params.GeologyMappingTypeID = branch.id;
        $scope.thisId = branch.id;
        $scope.params.StartDate = $scope.startDate;
        $scope.params.EndDate = $scope.endDate;
        getFieList($scope.params);
        $scope.treeBranch = branch;

        setTimeout(function () {
            //去掉遮罩
            abp.ui.unblock($('#app'));
        }, 100);
    };
    $scope.signupDataSearchForm = function () {
        $scope.params = getParams();
        $scope.params.GeologyMappingTypeID = $scope.thisId;
        $scope.params.StartDate = $scope.startDate;
        $scope.params.EndDate = $scope.endDate;
        getFieList($scope.params);
    };
    $scope.pageChanged = function (currentPage) {
        $scope.params.currentPage = currentPage;
        getFieList($scope.params);
    };
    //删除
    $scope.selected = [];
    function updateSelected(action, id) {
        if (action == 'add' && $scope.selected.indexOf(id) == -1) {
            $scope.selected.push(id);
        };
        if (action == 'remove' && $scope.selected.indexOf(id) != -1) {
            $scope.selected.splice($scope.selected.indexOf(id), 1);
        };
    };
    $scope.updateSelection = function ($event, id) {
        var action = ($event.target.checked ? 'add' : 'remove');
        updateSelected(action, id);
    };
    $scope.isSelected = function (id) {
        return $scope.selected.indexOf(id) >= 0;
    };

    //删除按钮的方法
    $scope.deleteList = function () {
        console.log($scope.selected);
        if ($scope.selected.length) {
            SweetAlert.swal({
                title: "是否确认删除选中文件?",
                text: "你将不能恢复它们!",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "删除",
                cancelButtonText: "取消",
                closeOnConfirm: false,
                closeOnCancel: false
            }, function (isConfirm) {
                if (isConfirm) {
                    //创建遮罩
                    abp.ui.block($('#app'));
                    dataManage.deleteDataMain($scope.selected).success(function () {
                        setTimeout(function () {
                            //去掉遮罩
                            abp.ui.unblock($('#app'));
                        }, 100);
                        SweetAlert.swal("Deleted!", "文件已经被删除.", "success");
                        $scope.selected = [];
                        getFieList($scope.params);
                    }).error(function () {
                        setTimeout(function () {
                            //去掉遮罩
                            abp.ui.unblock($('#app'));
                        }, 100);
                        SweetAlert.swal("Error!", "删除失败", "error");
                    });
                } else {
                    SweetAlert.swal("Cancelled", "File is safe :)", "error");
                };
            });
        };
    };
}]);

app.filter('roleChange', function () {
    return function (role, str) {
        switch (role) {
            case "001":
                role = "内容提供者";
                break;
            case "002":
                role = "维护管理员";
                break;
            case "003":
                role = "所有者";
                break;
            case "004":
                role = "用户";
                break;
            case "005":
                role = "分发者";
                break;
            case "006":
                role = "生产者";
                break;
            case "007":
                role = "联系方式";
                break;
            case "008":
                role = "主要调查者";
                break;
            case "009":
                role = "处理者";
                break;
            case "010":
                role = "出版者";
                break;
            case "011":
                role = "作者";
                break;
            default:
                role = "未知";
                break;
        }
        return role;
    }
});

app.factory('select', ["$location", function ($location) {
    return {
        setWebPage: {
            title: '',
            toLeftStyle: 'margin-left:210px',
            isTreeShow: false,
            isPanelHeadShow: false,
            showMetaData: false
        },
        //id: $location.path().slice($location.path().lastIndexOf("/") + 1),
        getOptions: function (arr, data) {
            arr = [];
            angular.forEach(data, function (item) {
                var select = {};
                select.name = item.text;
                select.value = item.value;
                arr.push(select);
            });
            return arr;
        },
        getDefault: function (obj, data, arr) {
            obj = {};
            obj.selected = { name: data };
            angular.forEach(arr, function (item) {
                if (item.value == data) {
                    obj.selected.name = item.name;
                    obj.selected.value = item.value;
                } else if (item.name == data) {
                    obj.selected.name = item.name;
                    obj.selected.value = item.value;
                };
            });
            return obj;
        }
    };
}]);

//详细信息部分  $rootScope.progressAllShow = false;
app.controller('dataDetailCtrl', ['$rootScope', "$scope", "$modalInstance", "$modal", "$timeout", "$state", 'Upload', "$localStorage", '$stateParams', 'abp.services.app.dataManage', "$sce", "select", "SweetAlert",
    function ($rootScope, $scope, $modalInstance, $modal, $timeout, $state, Upload, $localStorage, $stateParams, dataManage, $sce, select, SweetAlert) {
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
        $scope.setWebPage = angular.copy(select.setWebPage);

        $scope.detailId = select.fileId;
        $scope.detailName = select.fileName;
        $scope.metaDataID = select.metaDataId;

        var fileDto = {
            MainID: $scope.detailId
        }
        //fileDto.MainID = $scope.detailId;

        var loadClass = new Array();

        $scope.showMetaData = $scope.setWebPage.showMetaData;
        $scope.showIframe = select.fileId ? true : false;
        $scope.editAble = false;
        //*********************加载树开始*********************
        $scope.treeList = [{
            label: $scope.detailName,
            children: [{
                label: '文档',
                uid: 11
            }, {
                label: '栅格图',
                uid: 12
            }, {
                label: '说明书',
                uid: 13
            }, {
                label: '元数据',
                uid: 14
            }, {
                label: '成果图',
                uid: 15
            }]
        }];
        function treeFormat(item) {
            var treedata_avm = {};
            treedata_avm.label = item.fileName;
            treedata_avm.uid = item.fileID;
            treedata_avm.data = item.ext;
            treedata_avm.level = 3;

            switch (item.folderName) {
                case "1文档":
                    treedata_avm.parent_uid = 11;
                    break;
                case "3栅格图":
                    treedata_avm.parent_uid = 12;
                    break;
                case "4说明书":
                    treedata_avm.parent_uid = 13;
                    break;
                    //case "5元数据":
                    //    treedata_avm.parent_uid = 14;
                    //    break;
                case "成果图":
                    treedata_avm.parent_uid = 15;
                    break;
                default:
                    break;
            }
            //treedata_avm.sn = item.sn;
            //treedata_avm.children = [];
            return treedata_avm;
        }

        function sortBySn(arr) {
            var temp = [];
            if (arr.length > 0) {
                arr.sort(function (a, b) {
                    return a.sn - b.sn;
                });
                temp = arr;
                angular.forEach(arr, function (item) {
                    item.children = sortBySn(item.children);
                });
            }
            return temp;
        }

        function transData(a, idStr, pidStr, childrenStr) {
            var r = [], hash = {}, id = idStr, pid = pidStr, children = childrenStr, i = 0, j = 0, len = a.length;
            for (; i < len; i++) {
                hash[a[i][id]] = a[i];
            }
            for (; j < len; j++) {
                var aVal = a[j], hashVP = hash[aVal[pid]];
                if (hashVP) {
                    !hashVP[children] && (hashVP[children] = []);
                    hashVP[children].push(aVal);
                } else {
                    r.push(aVal);
                }
            }
            return r;
        }

        function trans(tree, tempdata) {
            for (var i = 0; i < tempdata.length; i++) {
                switch (tempdata[i].parent_uid) {
                    case 11:
                        tree[0].children[0].children.push(tempdata[i]);
                        break;
                    case 12:
                        tree[0].children[1].children.push(tempdata[i]);
                        break;
                    case 13:
                        tree[0].children[2].children.push(tempdata[i]);
                        break;
                        //case 14:
                        //    tree[0].children[3].children.push(tempdata[i]);
                        //    break;
                    case 15:
                        tree[0].children[4].children.push(tempdata[i]);
                        break;
                    default:
                        break;
                }
            }
            return tree;
        }

        if (select.fileId) {
            dataManage.getFileList(fileDto).success(function (data) {
                //console.log(data);
                var temp = [];
                angular.forEach(data, function (val) {
                    if ($scope.setWebPage.type && val.ext === '.docx' && val.folderName === '4说明书') {
                        getFile(val.fileID, val.ext);
                        return;
                    } else {
                        temp.push(treeFormat(val));
                    }
                });
                if ($scope.setWebPage.type) { return; }
                $scope.treeList = trans($scope.treeList, temp);
                $rootScope.progressAllShow = false;
            });
        }


        //*********************加载树结束*********************

        //$timeout(function () {
        //    //展开树的全部节点
        //    $scope.my_tree.expand_all();
        //}, 1000);

        //选择树的节点触发事件
        $scope.my_tree_handler = function (branch) {
            //$scope.docIframe = "PDFJSInNet/web/viewer.html?file=/TempFile/09.pdf";

            if (branch.level == 3) {
                var type = branch.data;
                var sign = 0;

                $scope.showMetaData = false;
                $scope.showIframe = true;

                if (type == ".jpg" || type == ".png" || type == ".bmp" || type == ".gif" || type == ".JPG" || type == ".PNG" || type == ".BMP" || type == ".GIF") {
                    $scope.docIframe = $sce.trustAsResourceUrl('/#viewImage/' + branch.uid + "{1");
                }
                else {
                    for (var i = 0; i < loadClass.length; i++) {
                        if (branch.uid == loadClass[i].id) {
                            if (type == ".docx" || type == ".doc" || type == ".xls" || type == ".xlsx") {
                                uploadTwice(loadClass[i].url);
                            }
                            else if (type == ".pdf") {
                                $scope.docIframe = "PDFJSInNet/web/viewer.html?file=" + $sce.trustAsResourceUrl(loadClass[i].url);
                            }
                            else {
                            }
                            sign++;
                            break;
                        }
                    }

                    if (sign == 0) {
                        dataManage.getFile(branch.uid).success(function (data) {

                            Upload.upload({
                                url: '/Handle/FileAddressConversion.ashx',
                                data: { file: data }
                            }).then(function (response) {
                                //console.log(response);
                                var fileIsLoad = {
                                    id: branch.uid,
                                    url: response.data
                                }

                                if (type == ".docx" || type == ".doc" || type == ".xls" || type == ".xlsx") {
                                    uploadTwice(response.data);
                                }
                                else if (type == ".pdf") {
                                    $scope.docIframe = "PDFJSInNet/web/viewer.html?file=" + $sce.trustAsResourceUrl(response.data);
                                }
                                else {
                                }

                                loadClass.push(fileIsLoad);
                            }, function (resp) {
                                console.log('ERROR:' + resp.status);
                            });
                        }).error(function (data) {
                            console.log(data);
                        });
                    }
                }

            }
            else if (branch.level == 2 && branch.label == "元数据") {
                //$scope.docIframe = $sce.trustAsResourceUrl("/#dataForDetail/" + $scope.metaDataID);
                $scope.showMetaData = true;
                $scope.showIframe = false;
            }
            else if (branch.level == 2 && branch.label == "成果图") {
                dataManage.getResultChart($scope.detailId).success(function (data) {
                    if (data === "-1") {
                        SweetAlert.swal({
                            title: "没有成果图...",
                            text: "请联系管理员，谢谢！",
                            type: "warning",
                            confirmButtonColor: "#007AFF"
                        });
                        
                        $scope.showMetaData = false;
                        $scope.showIframe = false;
                        return;
                    }
                    else {
                        var tmp = data.split("\\");
                        var id = tmp[tmp.length - 2];
                        $scope.docIframe = $sce.trustAsResourceUrl('/#viewImage/' + id + "{2");

                        $scope.showMetaData = false;
                        $scope.showIframe = true;
                    }
                    
                }).error(function (data) {
                    SweetAlert.swal({
                        title: "错误！",
                        text: "请联系管理员，谢谢！",
                        type: "error",
                        confirmButtonColor: "#007AFF"
                    });
                    console.log(data);
                });
            }
        }

        function getFile(id, type) {
            dataManage.getFile(id).success(function (data) {
                console.log(data);
                Upload.upload({
                    url: '/Handle/FileAddressConversion.ashx',
                    data: { file: data }
                }).then(function (response) {
                    //console.log(response);
                    var fileIsLoad = {
                        id: id,
                        url: response.data
                    }

                    if (type == ".docx" || type == ".doc") {
                        uploadTwice(response.data);
                    }
                    else if (type == ".jpg" || type == ".png") {//window.open($state.href('app.dataManage.datasource', { dataId: id }));
                        $scope.docIframe = $sce.trustAsResourceUrl(response.data);
                    }
                    else if (type == ".pdf") {
                        $scope.docIframe = "PDFJSInNet/web/viewer.html?file=" + $sce.trustAsResourceUrl(response.data);
                    }
                    else {
                    }

                    loadClass.push(fileIsLoad);
                }, function (resp) {
                    console.log('ERROR:' + resp.status);
                });
            }).error(function (data) {
                console.log(data);
            });
        }

        function uploadTwice(path) {
            $.post("/Handle/OfficeOnlineConfig.ashx", { filepath: path }, function (data) {
                //console.log(data);
                $timeout(function () {
                    $rootScope.progressAllShow = false;
                    $scope.docIframe = $sce.trustAsResourceUrl(data);
                }, 100);

            });
        }
    }]);

//元数据部分
app.controller('dataSourceCtrl', ["$scope", "$modalInstance", "$modal", "$timeout", "abp.services.app.metaData", "abp.services.app.dictionary", "select", "SweetAlert",
    function ($scope, $modalInstance, $modal, $timeout, metaData, dictionary, select, SweetAlert) {
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
        $scope.editAble = true;

        //元数据->保存
        $scope.save = function () {
            $scope.temp = select.metaData;
            var result = [];
            //元数据信息->空间参考系
            $scope.temp.refSysInfoEntity.refSysName = select.georefsysListed.selected.name;

            //标识信息->字符集
            $scope.temp.identificationEntity.mdChar = select.identificationCharset.selected.value;

            //标识信息->数据表示方式
            $scope.temp.identificationEntity.dataRpType = select.reppesentationListed.selected.value;

            //标识信息->状况
            $scope.temp.identificationEntity.idStatus = select.progressListed.selected.value;

            //标识信息->使用限制
            $scope.temp.constsEntity.useConsts = select.restrictionsListed.selected.value;

            //标识信息->安全等级
            $scope.temp.constsEntity.classification = select.classificationListed.selected.value;

            //标识信息->维护更新频率
            $scope.temp.maintInfoEntity.maintFreq = select.frequencyListed.selected.value;

            //标识信息->表达形式
            $scope.temp.dqInfoEntity.persForm = select.presentationListed.selected.value;

            //分发信息->分发介质名称
            $scope.temp.distInfoEntityList[0].medName = select.mediumnameListed.selected.value;

            //分发信息->分发服务类型
            $scope.temp.distInfoEntityList[0].medServerType = select.distsrvTypeListed.selected.value;

            //联系信息->职责
            $scope.temp.contactEntityList[0].role = select.roleListed.selected.value;

            //标识信息->专题类别
            var tpCat = [];
            var tpCatStr = "";
            angular.forEach(select.topiccategoryListed.selected, function (item) {
                tpCat.push(item.value);
            });
            tpCatStr = tpCat.toString();
            $scope.temp.identificationEntity.tpCat = tpCatStr;
            // 元数据信息
            metaData.updateMetaData($scope.temp.metaDataEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 元数据信息->空间参考系
            metaData.updateRefSysInfo($scope.temp.refSysInfoEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 元数据信息->数据质量信息
            metaData.updateDqInfo($scope.temp.dqInfoEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 标识信息
            metaData.updateIdentification($scope.temp.identificationEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 时间范围信息
            metaData.updateTempExtent($scope.temp.tempExtentEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 标识信息->数据集格式
            metaData.updateFormat($scope.temp.formatEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 标识信息->地理坐标范围信息
            metaData.updateGeoBndBox($scope.temp.geoBndBoxEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 标识信息->垂向范围信息
            metaData.updateVerExtent($scope.temp.verExtentEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 标识信息->数据集限制
            metaData.updateConsts($scope.temp.constsEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 标识信息->维护信息
            metaData.updateMaintInfo($scope.temp.maintInfoEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 标识信息->引用信息
            metaData.updateCitation($scope.temp.citationEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 静态浏览图
            metaData.updateBrowGraph($scope.temp.browGraphEntityList[0]).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 内容信息
            metaData.updateConInfo($scope.temp.conInfoEntityList[0]).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 分发信息
            metaData.updateDistInfo($scope.temp.distInfoEntityList[0]).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 联系单位
            metaData.updateContact($scope.temp.contactEntityList[0]).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            // 联系信息（单位）
            metaData.updateIdPoC($scope.temp.idPoCEntity).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });

            $timeout(function () {
                var temp = [];
                angular.forEach(result, function (item) {
                    if (item == "false") {
                        temp.push(item);
                    };
                });
                if (temp.length) {
                    SweetAlert.swal({
                        title: "保存失败!",
                        type: "error",
                        confirmButtonColor: "#DD6B55"
                    });
                } else {
                    SweetAlert.swal({
                        title: "保存成功!",
                        type: "success",
                        confirmButtonColor: "#007AFF"
                    });
                };
            }, 100);
            $modalInstance.dismiss('cancel');
        };
    }]);

//元数据模板
app.controller('metaDataCtrl', ["$rootScope", "$scope", "$modal", "$timeout", "$sce", "abp.services.app.metaData", "abp.services.app.dictionary", "select", "SweetAlert", "Upload",
function ($rootScope, $scope, $modal, $timeout, $sce, metaData, dictionary, select, SweetAlert, Upload) {
    //if(select.setWebPage.type === true){return;}

    $scope.oneAtATime = false;
    $scope.status = {
        isFirstOpen: true,
    };
    $scope.temp = {};

    //下拉框选项
    //元数据信息->空间参考系 
    dictionary.getGeorefsysCodeList().success(function (data) {
        $scope.georefsysList = select.getOptions($scope.georefsysList, data);
    });
    //标识信息->字符集 
    dictionary.getCharsetCodeList().success(function (data) {
        $scope.charsetList = select.getOptions($scope.charsetList, data);
    });
    //标识信息->类型 
    $scope.keywordList = [];
    dictionary.getKeywordTypeCodeCodeList().success(function (data) {
        $scope.keywordList = select.getOptions($scope.keywordList, data);
        select.keywordList = $scope.keywordList;
    });
    //标识信息->专题类别 
    $scope.topiccategoryList = [];
    dictionary.getTopiccategoryCodeList().success(function (data) {
        angular.forEach(data, function (item) {
            var temp = {};
            temp.id = item.id;
            temp.name = item.secCategoryName;
            temp.topCategory = item.topCategoryName;
            temp.value = item.topiccatCode;
            $scope.topiccategoryList.push(temp);
        });
    });
    //标识信息->数据表示方式
    dictionary.getReppesentationTypeCodeList().success(function (data) {
        $scope.reppesentationList = select.getOptions($scope.reppesentationList, data);
    });
    //标识信息->状况 
    dictionary.getProgressCodeList().success(function (data) {
        $scope.progressList = select.getOptions($scope.progressList, data);
    });
    //标识信息->使用限制 
    dictionary.getRestrictionsList().success(function (data) {
        $scope.restrictionsList = select.getOptions($scope.restrictionsList, data);
    });
    //标识信息->安全等级 
    dictionary.getClassificationList().success(function (data) {
        $scope.classificationList = select.getOptions($scope.classificationList, data);
    });
    //标识信息->维护更新频率 
    dictionary.getMaintenanFrequencyCodeList().success(function (data) {
        $scope.frequencyList = select.getOptions($scope.frequencyList, data);
    });
    //标识信息->表达形式 
    dictionary.getPresentationCodeCodeList().success(function (data) {
        $scope.presentationList = select.getOptions($scope.presentationList, data);
    });
    //分发信息->分发介质名称 
    dictionary.getMediumnameCodeList().success(function (data) {
        $scope.mediumnameList = select.getOptions($scope.mediumnameList, data);
    });
    //分发信息->分发服务类型 
    dictionary.getDistsrvTypeCodeList().success(function (data) {
        $scope.distsrvTypeList = select.getOptions($scope.distsrvTypeList, data);
    });
    //联系信息->职责 
    dictionary.getRoleCodeList().success(function (data) {
        $scope.roleList = select.getOptions($scope.roleList, data);
        select.roleList = $scope.roleList;
    });

    function getKeyword(obj, data, arr, str) {
        obj = { name: data };
        angular.forEach(arr, function (item) {
            if (item.value == data) {
                obj.name = item.name;
                obj.value = item.value;
                obj.keyword = str;
            } else if (item.name == data) {
                obj.name = item.name;
                obj.value = item.value;
                obj.keyword = str;
            };
        });
        return obj;
    };
    function getMultiple(obj, data, arr) {
        obj = { name: data };
        angular.forEach(arr, function (item) {
            if (item.value == data) {
                obj.name = item.name;
                obj.value = item.value;
            } else if (item.name == data) {
                obj.name = item.name;
                obj.value = item.value;
            };
        });
        return obj;
    };
    //获取页面所需数据 下拉框默认选项
    metaData.getMetaDataAll(select.metaDataId).success(function (data) {
        $rootScope.progressAllShow = false;
        $scope.temp = data;
        select.metaData = data;
        $scope.imgSrc = data.browGraphEntityList[0].bgFileName;

        //元数据信息->空间参考系 
        $scope.georefsysListed = select.getDefault($scope.georefsysListed, $scope.temp.refSysInfoEntity.refSysName, $scope.georefsysList);
        select.georefsysListed = $scope.georefsysListed;

        //标识信息->字符集 
        $scope.identificationCharset = select.getDefault($scope.identificationCharset, $scope.temp.identificationEntity.mdChar, $scope.charsetList);
        select.identificationCharset = $scope.identificationCharset;

        //标识信息->类型 
        $scope.keywordListed = {};
        $scope.keywordListed.selected = [];
        angular.forEach($scope.temp.keyWordsEntityList, function (item) {
            if (item.keyword === null) {
                item.keyword = "";
            }
            $scope.keywordListed.selected.push(getKeyword($scope.keywordListed.selected, item.keyTyp, $scope.keywordList, item.keyword));
        });
        //select.keywordListed = $scope.keywordListed;

        $scope.keyword = "";
        angular.forEach($scope.keywordListed.selected, function (item) {
            if (item.keyword === "") {
                item.keyword = "无";
            };
            var str = "";
            str = item.name + ":" + item.keyword;
            $scope.keyword += str + "\n";
        });

        //标识信息->专题类别 
        var tpCat = [];
        $scope.topiccategoryListed = {};
        $scope.topiccategoryListed.selected = [];
        tpCat = $scope.temp.identificationEntity.tpCat.split(",");
        angular.forEach(tpCat, function (item) {
            $scope.topiccategoryListed.selected.push(getMultiple($scope.topiccategoryListed.selected, item, $scope.topiccategoryList));
        });
        select.topiccategoryListed = $scope.topiccategoryListed;

        //标识信息->数据表示方式 
        $scope.reppesentationListed = select.getDefault($scope.reppesentationListed, $scope.temp.identificationEntity.dataRpType, $scope.reppesentationList);
        select.reppesentationListed = $scope.reppesentationListed;

        //标识信息->状况 
        $scope.progressListed = select.getDefault($scope.progressListed, $scope.temp.identificationEntity.idStatus, $scope.progressList);
        select.progressListed = $scope.progressListed;

        //标识信息->使用限制 
        $scope.restrictionsListed = select.getDefault($scope.restrictionsListed, $scope.temp.constsEntity.useConsts, $scope.restrictionsList);
        select.restrictionsListed = $scope.restrictionsListed;

        //标识信息->安全等级 
        $scope.classificationListed = select.getDefault($scope.classificationListed, $scope.temp.constsEntity.classification, $scope.classificationList);
        select.classificationListed = $scope.classificationListed;

        //标识信息->维护更新频率 
        $scope.frequencyListed = select.getDefault($scope.frequencyListed, $scope.temp.maintInfoEntity.maintFreq, $scope.frequencyList);
        select.frequencyListed = $scope.frequencyListed;

        //标识信息->表达形式 
        $scope.presentationListed = select.getDefault($scope.presentationListed, $scope.temp.dqInfoEntity.persForm, $scope.presentationList);
        select.presentationListed = $scope.presentationListed;

        //分发信息->分发介质名称 
        $scope.mediumnameListed = select.getDefault($scope.mediumnameListed, $scope.temp.distInfoEntityList[0].medName, $scope.mediumnameList);
        select.mediumnameListed = $scope.mediumnameListed;

        //分发信息->分发服务类型 
        $scope.distsrvTypeListed = select.getDefault($scope.distsrvTypeListed, $scope.temp.distInfoEntityList[0].medServerType, $scope.distsrvTypeList);
        select.distsrvTypeListed = $scope.distsrvTypeListed;

        //联系信息->职责 
        $scope.roleListed = select.getDefault($scope.roleListed, $scope.temp.contactEntityList[0].role, $scope.roleList);
        select.roleListed = $scope.roleListed;
    });

    //静态浏览图 
    $scope.showmediaIframe = false;
    $scope.showDelete = true;
    $scope.showBrowGraph = function (id) {
        metaData.getBrowGraphPath(id).success(function (data) {
            Upload.upload({
                url: '/Handle/FileAddressConversion.ashx',
                data: { file: data }
            }).then(function (response) {
                $scope.showmediaIframe = true;
                $scope.mediaIframe = $sce.trustAsResourceUrl(response.data);
            }, function (resp) {
                console.log('ERROR');
            });
        });
    };

    $scope.delBrowGraph = function (id) {
        $rootScope.progressAllShow = true;
        metaData.deleteBrowGraphById(id).success(function () {
            SweetAlert.swal({
                title: "删除成功！",
                type: "success",
                confirmButtonColor: "#007AFF"
            });
            metaData.getBrowGraphList(select.metaDataId).success(function (data) {
                console.log(data);
                $scope.temp.browGraphEntityList = data;
                $rootScope.progressAllShow = false;
            });
        }).error(function () {
            SweetAlert.swal({
                title: "删除失败！",
                type: "error",
                confirmButtonColor: "#007AFF"
            });
            $rootScope.progressAllShow = false;
        });
        
    }

    $scope.openImgManage = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addStaticImg.html',
            controller: 'staticImgCtrl',
            size: 'lg'
        });
    };

    $scope.openKeyWord = function () {
        //select.keywordList = $scope.keywordListed.selected;
        var modalInstance = $modal.open({
            templateUrl: 'keyWord.html',
            controller: 'keyWordCtrl',
            size: 'lg'
        });
    };

    $scope.openContactManage = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addContactInfo.html',
            controller: 'contactManageCtrl',
            size: 'lg'
        });
    };

    //联系信息 编辑
    $scope.edit = function (id) {
        select.editContactId = id;
        var modalInstance = $modal.open({
            templateUrl: 'editContactInfo.html',
            controller: 'contactManageCtrl',
            size: 'lg'
        });
    };

    //联系信息 删除
    $scope.delete = function (id) {
        SweetAlert.swal({
            title: "是否确认删除选中文件?",
            text: "你将不能恢复它们!",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "删除",
            cancelButtonText: "取消",
            closeOnConfirm: false,
            closeOnCancel: false
        }, function (isConfirm) {
            if (isConfirm) {
                //创建遮罩
                abp.ui.block($('#app'));
                metaData.deleteContactById(id).success(function () {
                    setTimeout(function () {
                        //去掉遮罩
                        abp.ui.unblock($('#app'));
                    }, 100);
                    SweetAlert.swal("Deleted!", "文件已经被删除.", "success");
                    metaData.getContactList(select.metaDataId).success(function (data) {
                        $scope.temp.contactEntityList = data;
                    });
                }).error(function () {
                    setTimeout(function () {
                        //去掉遮罩
                        abp.ui.unblock($('#app'));
                    }, 100);
                    SweetAlert.swal("Error!", "删除失败", "error");
                });
            } else {
                SweetAlert.swal("Cancelled", "File is safe :)", "error");
            };
        });
    };

    //刷新静态浏览图信息
    $rootScope.$on('refreshBrowGraph', function () {
        metaData.getBrowGraphList(select.metaDataId).success(function (data) {
            console.log(data);
            $scope.temp.browGraphEntityList = data;
            $rootScope.progressAllShow = false;
        });
    });

    //联系信息 刷新列表
    var deregisterEdit = $rootScope.$on("editContactInfo", function (event) {
        metaData.getContactList(select.metaDataId).success(function (data) {
            $scope.temp.contactEntityList = data;
        });
    });
    var deregisterAdd = $rootScope.$on("addContactInfo", function (event) {
        metaData.getContactList(select.metaDataId).success(function (data) {
            $scope.temp.contactEntityList = data;
        });
    });
    var deregisterKeyword = $rootScope.$on("keywordChange", function (event) {
        metaData.getKeyWordsList(select.metaDataId).success(function (data) {
            $scope.keywordListed = {};
            $scope.keywordListed.selected = [];
            angular.forEach(data, function (item) {
                if (item.keyword === null) {
                    item.keyword = "";
                }
                $scope.keywordListed.selected.push(getKeyword($scope.keywordListed.selected, item.keyTyp, $scope.keywordList, item.keyword));
            });

            $scope.keyword = "";
            angular.forEach($scope.keywordListed.selected, function (item) {
                if (item.keyword === "") {
                    item.keyword = "无";
                };
                var str = "";
                str = item.name + ":" + item.keyword;
                $scope.keyword += str + "\n";
            });
        });
    });
    var deregisterDelete = $rootScope.$on("deleteContact", function () {
        metaData.getContactList(select.metaDataId).success(function (data) {
            $scope.temp.contactEntityList = data;
        });
    });
    var deregisterContact = $rootScope.$on("editContact", function (event) {
        metaData.getContactList(select.metaDataId).success(function (data) {
            $scope.contact = data;
        });
    });
    $scope.$on("destory", function () {
        deregisterEdit();
        deregisterAdd();
        deregisterKeyword();
        deregisterContact();
    });
}]);

//关键词弹窗
app.controller('keyWordCtrl', ["$rootScope", "$scope", "$modalInstance", "$timeout", "Upload", "abp.services.app.metaData", "select", "SweetAlert",
    function ($rootScope, $scope, $modalInstance, $timeout, Upload, metaData, select, SweetAlert) {
        $scope.temp = select.keywordList;

        $scope.keywordList = select.keywordList;
        $scope.temp = select.metaData.keyWordsEntityList;
        arr = [];
        angular.forEach($scope.temp, function (val) {
            angular.forEach($scope.keywordList, function (item) {
                if (item.value == val.keyTyp) {
                    item.keyword = val.keyword;
                };
            });
        });
        $scope.delete = function (index) {
            $scope.keywordList.splice(index, 1);
        };

        $scope.save = function () {
            var result = [];
            var selected = [];
            angular.forEach($scope.keywordList, function (item) {
                var keyword = {};
                keyword.id = "";
                keyword.metaDataID = select.metaDataId;
                keyword.keyTyp = item.value;
                keyword.keyword = "";
                if (item.keyword) {
                    keyword.keyword = item.keyword;
                };
                selected.push(keyword);
            });
            metaData.deleteKeyWords(select.metaDataId).success(function () {
                result.push("true");
            }).error(function () {
                result.push("false");
            });
            if (selected.length) {
                angular.forEach(selected, function (item) {
                    metaData.insertKeyWords(item).success(function () {
                        result.push("true");
                    }).error(function () {
                        result.push("false");
                    });
                });
            };
            $timeout(function () {
                var temp = [];
                angular.forEach(result, function (item) {
                    if (item == "false") {
                        temp.push(item);
                    };
                });
                if (temp.length) {
                    SweetAlert.swal({
                        title: "保存失败!",
                        type: "error",
                        confirmButtonColor: "#DD6B55"
                    });
                } else {
                    SweetAlert.swal({
                        title: "保存成功!",
                        type: "success",
                        confirmButtonColor: "#007AFF"
                    });
                    $rootScope.$emit("keywordChange");
                };
            }, 100);
            $modalInstance.dismiss('cancel');
        };
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }])

//静态缩略图添加
app.controller('staticImgCtrl', ["$rootScope", "$scope", "$modalInstance", "$modal", "Upload", "abp.services.app.metaData", "select", "SweetAlert",
    function ($rootScope, $scope, $modalInstance, $modal, Upload, metaData, select, SweetAlert) {
        $scope.imgEntity = {
            id: '',
            MetaDataID: select.metaDataId,
            bgFileName: "",
            bgFileDesc: ""
        }
        
        $scope.upStaticImg = function (file) {
            if (file != "" && file != null && file != undefined) {
                var name = file.name.split(".");
                var tempName = name[name.length - 1];
                if (tempName == "jpg" || tempName == "JPG" || tempName == "png" || tempName == "PNG" || tempName == "gif" || tempName == "GIF" || tempName == "bmp" || tempName == "BMP") {
                    Upload.upload({
                        url: '/Handle/UpLoadFileHandle.ashx',
                        file: file
                    }).then(function (resp) {
                        console.log(resp);
                        $scope.imgEntity.bgFileName = resp.data.Filepath[0];
                        $scope.imageName = file.name;
                    }, function (resp) {
                        console.log('Error!');
                        console.log(resp);

                    }, function (evt) {

                    });
                }
                else {
                    alert("请上传图片文件！");
                }
            }
        };

        $scope.save = function () {
            console.log($scope.imgEntity);
            if ($scope.imgEntity.bgFileName == '' || $scope.imgEntity.bgFileName == null || $scope.imgEntity.bgFileName == undefined) {
                SweetAlert.swal({
                    title: "请添加图片文件!",
                    type: "error",
                    confirmButtonColor: "#007AFF"
                });
            }
            else if ($scope.imgEntity.bgFileDesc == '' || $scope.imgEntity.bgFileDesc == null || $scope.imgEntity.bgFileDesc == undefined) {
                SweetAlert.swal({
                    title: "请添加图片描述!",
                    type: "error",
                    confirmButtonColor: "#007AFF"
                });
            }
            else {
                $rootScope.progressAllShow = true;
                metaData.insertBrowGraph($scope.imgEntity).success(function () {
                    SweetAlert.swal({
                        title: "添加成功！",
                        type: "success",
                        confirmButtonColor: "#007AFF"
                    });
                    $rootScope.$emit('refreshBrowGraph');
                }).error(function () {
                    SweetAlert.swal({
                        title: "添加失败！",
                        type: "error",
                        confirmButtonColor: "#007AFF"
                    });
                    $rootScope.progressAllShow = false;
                });

                $modalInstance.dismiss('cancel');
            }
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
}]);

//联系信息管理
app.controller('contactManageCtrl', ["$rootScope", "$scope", "$modalInstance", "$modal", "abp.services.app.metaData", "select", "SweetAlert",
function ($rootScope, $scope, $modalInstance, $modal, metaData, select, SweetAlert) {
    $scope.openContact = function () {
        var modalInstance = $modal.open({
            templateUrl: 'openContact.html',
            controller: 'contactCtrl',
            size: 'lg'
        });
    };
    $scope.openEditContact = function (id) {
        var modalInstance = $modal.open({
            templateUrl: 'editContact.html',
            controller: 'editContactCtrl',
            size: 'lg'
        });
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.roleListed = {};
    $scope.roleListed.selected = { name: "内容提供者", value: "001" }
    $scope.roleList = select.roleList;
    $scope.contactInfo = {
        id: "",
        metaDataID: select.metaDataId,
        adminArea: "",
        city: "",
        cntDelPnt: "",
        cntFaxNum: "",
        cntKHH: "",
        cntOnlineRes: "",
        cntPhone: "",
        cntZZZS: "",
        country: "",
        eMailAddr: "",
        jbzs: "",
        postCode: "",
        role: "",
        peopleRole: "",
        rpIndName: "",
        rpOrgName: "",
        rpPosName: ""
    };
    var releaseChange = $rootScope.$on("contactInfoChange", function (event, data) {
        $scope.contactInfo = data;
    });
    var releaseAdd = $rootScope.$on("addContactInfo", function (event, data) {
        data[0].id = "";
        console.log(data[0]);
        $scope.contactInfo = data[0];
    });
    $scope.$on("destory", function () {
        releaseChange();
        releaseAdd();
    });
    //添加联系信息
    $scope.save = function () {
        $scope.contactInfo.role = $scope.roleListed.selected.value;
        metaData.insertContact($scope.contactInfo).success(function () {
            SweetAlert.swal({
                title: "添加成功!",
                type: "success",
                confirmButtonColor: "#007AFF"
            });
            $rootScope.$emit("addContactInfo");
        }).error(function () {
            SweetAlert.swal({
                title: "添加失败!",
                type: "error",
                confirmButtonColor: "#DD6B55"
            });
        });
        $modalInstance.close();
    };
    //编辑联系信息
    $scope.disabled = true;
    if (select.editContactId) {
        metaData.getContact(select.editContactId).success(function (data) {
            $scope.temp = data;
            $scope.dutyListed = select.getDefault($scope.dutyListed, $scope.temp.role, $scope.roleList);
        });
    };
    $scope.signup = function () {
        $scope.temp.role = $scope.dutyListed.selected.value;
        metaData.updateContact($scope.temp).success(function () {
            SweetAlert.swal({
                title: "修改成功!",
                type: "success",
                confirmButtonColor: "#007AFF"
            });
            $rootScope.$emit("editContactInfo");
        }).error(function () {
            SweetAlert.swal({
                title: "修改失败!",
                type: "error",
                confirmButtonColor: "#DD6B55"
            });
        });
        $modalInstance.close();
    };

}]);

//联系单位弹窗
app.controller('contactCtrl', ["$rootScope", "$scope", "$modalInstance", "$modal", "abp.services.app.metaData", "abp.services.app.dictionary", "select", "SweetAlert",
function ($rootScope, $scope, $modalInstance, $modal, metaData, dictionary, select, SweetAlert) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    metaData.getContactList(select.metaDataId).success(function (data) {
        $scope.contact = data;
    });
    //编辑
    $scope.edit = function (id) {
        select.contactId = id;
        console.log(id);
        var modalInstance = $modal.open({
            templateUrl: 'editContact.html',
            controller: 'editContactCtrl',
            size: 'lg'
        });
    };
    //删除
    $scope.delete = function (id) {
        SweetAlert.swal({
            title: "是否确认删除选中文件?",
            text: "你将不能恢复它们!",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "删除",
            cancelButtonText: "取消",
            closeOnConfirm: false,
            closeOnCancel: false
        }, function (isConfirm) {
            if (isConfirm) {
                //创建遮罩
                abp.ui.block($('#app'));
                metaData.deleteContactById(id).success(function () {
                    setTimeout(function () {
                        //去掉遮罩
                        abp.ui.unblock($('#app'));
                    }, 100);
                    SweetAlert.swal("Deleted!", "文件已经被删除.", "success");
                    metaData.getContactList(select.metaDataId).success(function (data) {
                        $scope.contact = data;
                    });
                    $rootScope.$emit("deleteContact");
                }).error(function () {
                    setTimeout(function () {
                        //去掉遮罩
                        abp.ui.unblock($('#app'));
                    }, 100);
                    SweetAlert.swal("Error!", "删除失败", "error");
                });
            } else {
                SweetAlert.swal("Cancelled", "File is safe :)", "error");
            };
        });
    };
    //添加
    $scope.add = function () {
        var modalInstance = $modal.open({
            templateUrl: 'newContact.html',
            controller: 'addContactCtrl',
            size: 'lg'
        });
        $modalInstance.dismiss('cancel');
    };
    var selected = [];
    $scope.select = function (item, index) {
        selected = [];
        selected.push(item);
        $scope.selectedRow = index;
    };

    $scope.signup = function () {
        $rootScope.$emit("addContactInfo", selected);
        $modalInstance.dismiss('cancel');
    };
    var deregisterEdit = $rootScope.$on("editContact", function (event) {
        metaData.getContactList(select.metaDataId).success(function (data) {
            $scope.contact = data;
        });
    });
    $scope.$on("destory", function () {
        deregisterEdit();
    });
}]);

//联系单位 编辑
app.controller('editContactCtrl', ["$rootScope", "$scope", "$modalInstance", "$modal", "abp.services.app.metaData", "select", "SweetAlert",
function ($rootScope, $scope, $modalInstance, $modal, metaData, select, SweetAlert) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.dutyList = select.roleList;
    metaData.getContact(select.contactId).success(function (data) {
        $scope.contact = data;
        $scope.dutyListed = select.getDefault($scope.dutyListed, $scope.contact.peopleRole, $scope.dutyList);
        $scope.adminArea = select.getDefault($scope.adminArea, $scope.contact.adminArea, $scope.adminAreaList);
    });
    $scope.signup = function () {
        $scope.contact.peopleRole = $scope.dutyListed.selected.value;
        metaData.updateContact($scope.contact).success(function () {
            SweetAlert.swal({
                title: "修改成功!",
                type: "success",
                confirmButtonColor: "#007AFF"
            });
            $rootScope.$emit("editContact");
        }).error(function () {
            SweetAlert.swal({
                title: "修改失败!",
                type: "error",
                confirmButtonColor: "#DD6B55"
            });
        });
        $modalInstance.close();
    };
    $scope.openContactPeople = function () {
        var modalInstance = $modal.open({
            templateUrl: 'openContactPeople.html',
            controller: 'contactPeopleCtrl',
            size: 'lg'
        });
    };
}]);

//联系单位 添加
app.controller('addContactCtrl', ["$rootScope", "$scope", "$modalInstance", "$modal", "abp.services.app.metaData", "select", "SweetAlert",
function ($rootScope, $scope, $modalInstance, $modal, metaData, select, SweetAlert) {
    $scope.contact = {
        id: "",
        metaDataID: select.metaDataId,
        adminArea: "",
        city: "",
        cntDelPnt: "",
        cntFaxNum: "",
        cntKHH: "",
        cntOnlineRes: "",
        cntPhone: "",
        cntZZZS: "",
        country: "",
        eMailAddr: "",
        jbzs: "",
        postCode: "",
        role: "",
        peopleRole: "",
        rpIndName: "",
        rpOrgName: "",
        rpPosName: ""
    };
    $scope.dutyListed = {};
    $scope.dutyListed.selected = { name: "内容提供者", value: "001" };
    $scope.adminArea = {};
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.dutyList = select.roleList;
    $scope.signup = function () {
        $scope.contact.peopleRole = $scope.dutyListed.selected.value;
        $rootScope.$emit("contactInfoChange", $scope.contact);
        $modalInstance.close();
    };
    $scope.openContactPeople = function () {
        var modalInstance = $modal.open({
            templateUrl: 'openContactPeople.html',
            controller: 'contactPeopleCtrl',
            size: 'lg'
        });
    };
}]);

//联系人弹窗
app.controller('contactPeopleCtrl', ["$rootScope", "$scope", "$modalInstance", "$modal", "abp.services.app.metaData", "select", "SweetAlert",
function ($rootScope, $scope, $modalInstance, $modal, metaData, select, SweetAlert) {
    //$scope.contactPeople = [{ name: "张礼中" }];
    metaData.getContactpeopleList(select.contactId).success(function (data) {
        $scope.contactPeople = data;
    });
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    //编辑
    $scope.edit = function (id) {
        select.contactPeopleId = id;
        var modalInstance = $modal.open({
            templateUrl: 'editContactPeople.html',
            controller: 'editContactPeopleCtrl',
            size: 'lg'
        });
    };
    //删除
    $scope.delete = function (id) {
        SweetAlert.swal({
            title: "是否确认删除选中文件?",
            text: "你将不能恢复它们!",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "删除",
            cancelButtonText: "取消",
            closeOnConfirm: false,
            closeOnCancel: false
        }, function (isConfirm) {
            if (isConfirm) {
                //创建遮罩
                abp.ui.block($('#app'));
                metaData.deleteContactpeopleById(id).success(function () {
                    setTimeout(function () {
                        //去掉遮罩
                        abp.ui.unblock($('#app'));
                    }, 100);
                    SweetAlert.swal("Deleted!", "文件已经被删除.", "success");
                    metaData.getContactpeopleList(select.contactId).success(function (data) {
                        $scope.contactPeople = data;
                    });
                }).error(function () {
                    setTimeout(function () {
                        //去掉遮罩
                        abp.ui.unblock($('#app'));
                    }, 100);
                    SweetAlert.swal("Error!", "删除失败", "error");
                });
            } else {
                SweetAlert.swal("Cancelled", "File is safe :)", "error");
            };
        });
    };
    //添加
    $scope.add = function () {
        var modalInstance = $modal.open({
            templateUrl: 'newContactPeople.html',
            controller: 'addContactPeopleCtrl',
            size: 'lg'
        });
    };
    var deregisterAdd = $rootScope.$on("addContactPeople", function (event) {
        metaData.getContactpeopleList(select.contactId).success(function (data) {
            $scope.contactPeople = data;
        });
    });
    var deregisterEdit = $rootScope.$on("editContactPeople", function (event) {
        metaData.getContactpeopleList(select.contactId).success(function (data) {
            $scope.contactPeople = data;
        });
    });
    $scope.$on("destory", function () {
        deregisterAdd();
        deregisterEdit();
    });
}]);

//联系人 编辑
app.controller('editContactPeopleCtrl', ["$rootScope", "$scope", "$modalInstance", "abp.services.app.metaData", "select", "SweetAlert",
function ($rootScope, $scope, $modalInstance, metaData, select, SweetAlert) {
    $scope.list = [
      { name: '男', value: '1' },
      { name: '女', value: '2' },
    ];
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    metaData.getContactpeople(select.contactPeopleId).success(function (data) {
        $scope.contactPeople = data;
        select.contactPeople = $scope.contactPeopel;
        $scope.adminArea = select.getDefault($scope.adminArea, $scope.contactPeople.province, $scope.adminAreaList);
        $scope.sex = select.getDefault($scope.sex, $scope.contactPeople.sex, $scope.list);
        console.log($scope.contactPeople);
    });
    $scope.signup = function () {
        $scope.contactPeople.sex = $scope.sex.selected.value;
        metaData.updateContactpeople($scope.contactPeople).success(function () {
            SweetAlert.swal({
                title: "修改成功!",
                type: "success",
                confirmButtonColor: "#007AFF"
            });
            $rootScope.$emit("editContactPeople");
        }).error(function () {
            SweetAlert.swal({
                title: "修改失败!",
                type: "error",
                confirmButtonColor: "#DD6B55"
            });
        });
        $modalInstance.close();
    };
}]);

//联系人 添加
app.controller('addContactPeopleCtrl', ["$rootScope", "$scope", "$modalInstance", "$modal", "abp.services.app.metaData", "select", "SweetAlert",
function ($rootScope, $scope, $modalInstance, $modal, metaData, select, SweetAlert) {
    $scope.contactPeople = {
        id: "",
        poCId: select.contactId,
        name: "",
        sex: "",
        phone: "",
        fax: "",
        address: "",
        city: "",
        province: "",
        country: "",
        zipCode: "",
        email: "",
        webSite: "",
        qq: "",
        msn: ""
    };
    $scope.list = [
      { name: '男', value: '1' },
      { name: '女', value: '2' },
    ];
    $scope.sex = {};
    $scope.sex.selected = { name: '男', value: '1' };
    $scope.adminArea = {};
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.signup = function () {
        $scope.contactPeople.sex = $scope.sex.selected.value;
        metaData.insertContactpeople($scope.contactPeople).success(function () {
            SweetAlert.swal({
                title: "添加成功!",
                type: "success",
                confirmButtonColor: "#007AFF"
            });
            $rootScope.$emit("addContactPeople");
        }).error(function () {
            SweetAlert.swal({
                title: "添加失败!",
                type: "error",
                confirmButtonColor: "#DD6B55"
            });
        });
        $modalInstance.close();
    };
}])

app.controller('dataManageTimeCtrl', function ($scope) {
    $scope.today = function () {
        $scope.dataStorageStartTime = moment().subtract(15, 'days').format('YYYY-MM-DD');
        $scope.dataStorageEndTime = moment().format('YYYY-MM-DD');
        $scope.dataCreateTime = moment().format('YYYY-MM-DD');
        $scope.dataGatherStartTime = moment().format('YYYY-MM-DD');
        $scope.dataGatherEndTime = moment().format('YYYY-MM-DD');
    };
    $scope.today();
    $scope.$watch('dataStorageStartTime', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.$emit("dataStorageStartTimeChange", newValue);
        };
    });
    $scope.$watch('dataStorageEndTime', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.$emit("dataStorageEndTimeChange", newValue);
        };
    });

    $scope.clear = function () {
        $scope.dataStorageStartTime = null;
        $scope.dataStorageEndTime = null;
        $scope.dt = null;
    };

    // Disable weekend selection
    $scope.disabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() === 0 || date.getDay() === 6));
    };

    $scope.toggleMin = function () {
        $scope.minDate = $scope.minDate ? null : new Date();
    };
    $scope.toggleMin();

    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.opened = !$scope.opened;
    };

    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1,
        showWeeks: false
    };

    $scope.formats = ['dd-MMMM-yyyy', 'yyyy-MM-dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[1];
});


//导入文件弹出的窗口页面
app.controller('dataManageUploadCtrl', ['$rootScope', '$scope', '$http', "$timeout", '$modalInstance', 'items', 'allItems', 'abp.services.app.dataManage', 'Upload', 'SweetAlert',
    function ($rootScope, $scope, $http, $timeout, $modalInstance, items, allItems, dataManage, Upload, SweetAlert) {
        $scope.title = "上传文件";
        $scope.isProvince = ProvinceID !== "0";
        var fileDto = {
            GeologyMappingTypeID: "",
            ZipFilePath: "",
            Scale: "",
            Version: 1
        }

        //用来判断全国、区域、省域这组单选框选中哪个
        $scope.bigPlace = {
            "allCountry": "checked",
            "onePart": "",
            "oneProvince": ""
        }

        //数据项初始化
        $scope.isDisable = "false";
        $scope.provinceList = [];
        $scope.leibieList = [];
        $scope.nameList = [];
        $scope.provinceListed = {};
        $scope.leibieListed = {};
        $scope.nameListed = {};

        $scope.scaleList = [
            {
                label: "20 000 000",
                number: 20000000,
                level: "A"
            },
             {
                 label: "15 000 000",
                 number: 15000000,
                 level: "B"
             },
              {
                  label: "12 000 000",
                  number: 12000000,
                  level: "C"
              },
               {
                   label: "10 000 000",
                   number: 10000000,
                   level: "D"
               },
                {
                    label: "7 500 000",
                    number: 7500000,
                    level: "E"
                },
                 {
                     label: "6 000 000",
                     number: 6000000,
                     level: "F"
                 },
                       {
                           label: "5 000 000",
                           number: 5000000,
                           level: "G"
                       },
                              {
                                  label: "4 000 000",
                                  number: 4000000,
                                  level: "H"
                              },
                                 {
                                     label: "2 500 000",
                                     number: 2500000,
                                     level: "I"
                                 },
                                      {
                                          label: "2 000 000",
                                          number: 2000000,
                                          level: "J"
                                      },
            {
            label: "1 000 000",
            number: 1000000,
            level: "K"
        }, {
            label: "500 000",
            number: 500000,
            level: "L"
        }, {
            label: "250 000",
            number: 250000,
            level: "M"
        }, {
            label: "200 000",
            number: 200000,
            level: "N"
        }, {
            label: "100 000",
            number: 100000,
            level: "O"
        }, {
            label: "50 000",
            number: 50000,
            level: "P"
        }, {
            label: "25 000",
            number: 25000,
            level: "Q"
        }, {
            label: "10 000",
            number: 10000,
            level: "R"
        }, {
            label: "5 000",
            number: 5000,
            level: "S"
        }, {
            label: "2 000",
            number: 2000,
            level: "T"
        }, {
            label: "1 000",
            number: 1000,
            level: "U"
        }, {
            label: "500",
            number: 500,
            level: "V"
        }];
        $scope.scaleListed = {};

        $scope.editionList = [{
            label: "验收版",
            version: 1
        }, {
            label: "最终版",
            version: 2
        }];
        $scope.editionListed = {};

        /*-----------------------弹窗中单选框、下拉菜单数据绑定-----------------------*/
        if ($scope.isProvince) {
            $scope.leibieList = allItems;
            if (items.level === 1) {
                for (var i = 0; i < $scope.leibieList.length; i++) {
                    if (items.label == $scope.leibieList[i].label) {
                        $scope.leibieListed.selected = $scope.leibieList[i];
                        $scope.nameList = items.children;
                        break;
                    }
                }
            } else {
                for (var i = 0; i < $scope.leibieList.length; i++) {
                    if (items.pid == $scope.leibieList[i].id) {
                        $scope.leibieListed.selected = $scope.leibieList[i];
                        $scope.nameList = $scope.leibieList[i].children;
                        break;
                    }
                }
                for (var i = 0; i < $scope.nameList.length; i++) {
                    if (items.label == $scope.nameList[i].label) {
                        $scope.nameListed.selected = $scope.nameList[i];
                        break;
                    }
                }
            }

        } else {
            if (items.level == 1) {
                bigPlaceCheck(items);
                if (items.label != "省域") {
                    $scope.isDisable = "false";
                    $scope.leibieList = items.children;
                } else {
                    $scope.isDisable = "true";
                    $scope.provinceList = items.children;
                }

            }
            if (items.level == 2) {
                //绑定数据
                for (var i = 0; i < 3; i++) {
                    if (items.pid == allItems[i].id) {
                        bigPlaceCheck(allItems[i]);

                        if (allItems[i].label != "省域") {
                            $scope.leibieList = allItems[i].children;
                        } else {
                            $scope.isDisable = "true";
                            $scope.provinceList = allItems[i].children;

                        }
                    }
                }
                //判断选中的哪一个
                for (var j = 0; j < $scope.leibieList.length; j++) {
                    if (items.label == $scope.leibieList[j].label) {
                        $scope.leibieListed.selected = $scope.leibieList[j];
                        $scope.nameList = $scope.leibieListed.selected.children;
                    }
                }
                for (var k = 0; k < $scope.provinceList.length; k++) {
                    if (items.label == $scope.provinceList[k].label) {
                        $scope.provinceListed.selected = $scope.provinceList[k];
                        $scope.leibieList = $scope.provinceListed.selected.children;
                    }
                }
            }
            if (items.level == 3) {
                //绑定数据
                for (var i = 0; i < 3; i++) {
                    for (var j = 0; j < allItems[i].children.length; j++) {
                        var tempI = allItems[i].children;

                        if (items.pid == tempI[j].id) {
                            bigPlaceCheck(allItems[i]);

                            if (allItems[i].label != "省域") {
                                $scope.leibieList = tempI;
                                $scope.leibieListed.selected = $scope.leibieList[j];
                                $scope.nameList = $scope.leibieListed.selected.children;
                            } else {
                                $scope.isDisable = "true";
                                $scope.provinceList = tempI;
                                $scope.provinceListed.selected = $scope.provinceList[j];
                                $scope.leibieList = $scope.provinceListed.selected.children;
                            }
                        }
                    }
                }
                //判断选中的哪一个
                for (var k = 0; k < $scope.nameList.length; k++) {
                    if (items.label == $scope.nameList[k].label) {
                        $scope.nameListed.selected = $scope.nameList[k];
                    }
                }
                for (var m = 0; m < $scope.leibieList.length; m++) {
                    if (items.label == $scope.leibieList[m].label) {
                        $scope.leibieListed.selected = $scope.leibieList[m];
                        $scope.nameList = $scope.leibieListed.selected.children;
                    }
                }
            }
            if (items.level == 4) {
                $scope.isDisable = "true";
                //绑定数据
                for (var i = 0; i < 3; i++) {
                    for (var j = 0; j < allItems[i].children.length; j++) {
                        var tempI = allItems[i].children;

                        for (var k = 0; k < tempI[j].children.length; k++) {
                            if (items.pid == tempI[j].children[k].id) {
                                bigPlaceCheck(allItems[i]);
                                $scope.provinceList = tempI;
                                $scope.provinceListed.selected = $scope.provinceList[j];

                                $scope.leibieList = tempI[j].children;
                                $scope.leibieListed.selected = $scope.leibieList[k];
                                $scope.nameList = $scope.leibieList[k].children;
                            }
                        }
                    }
                }
                //判断选中的哪一个
                for (var m = 0; m < $scope.nameList.length; m++) {
                    if (items.label == $scope.nameList[m].label) {
                        $scope.nameListed.selected = $scope.nameList[m];
                    }
                }
            }
        }

        //单选框改变事件
        $scope.bigPlaceChange = function (event) {
            if (event.target.checked) {
                switch (event.target.value) {
                    case "1":
                        $scope.isDisable = "false";
                        $scope.nameList = [];
                        $scope.provinceList = [];
                        $scope.nameListed.selected = {};
                        $scope.provinceListed.selected = {};

                        $scope.leibieListed.selected = {};
                        for (var i = 0; i < allItems.length; i++) {
                            if (allItems[i].label === "全国") {
                                $scope.leibieList = allItems[i].children;
                            }
                        }
                        break;
                    case "2":
                        $scope.isDisable = "false";
                        $scope.nameList = [];
                        $scope.provinceList = [];
                        $scope.nameListed.selected = {};
                        $scope.provinceListed.selected = {};

                        $scope.leibieListed.selected = {};
                        for (var i = 0; i < allItems.length; i++) {
                            if (allItems[i].label === "区域") {
                                $scope.leibieList = allItems[i].children;
                            }
                        }
                        break;
                    case "3":
                        $scope.isDisable = "true";
                        $scope.nameList = [];
                        $scope.leibieList = [];
                        $scope.nameListed.selected = {};
                        $scope.leibieListed.selected = {};

                        $scope.provinceListed.selected = {};
                        for (var i = 0; i < allItems.length; i++) {
                            if (allItems[i].label === "省域") {
                                $scope.provinceList = allItems[i].children;
                            }
                        }
                        break;
                    default:
                        alert("ERROR!");
                        $modalInstance.dismiss('cancel');
                        break;
                }
            }
        }

        //图件类别下拉菜单的改变事件
        $scope.leibieChange = function (label) {
            $scope.nameList = label.children;
            $scope.nameListed.selected = {};
        }

        //省份下拉菜单的改变事件
        $scope.provinceChange = function (label) {
            $scope.leibieList = label.children;
            $scope.leibieListed.selected = {};
            $scope.nameList = [];
        }

        //传入第一级的数据，判断选中哪个单选框
        function bigPlaceCheck(theItem) {
 
            if (theItem.label == "全国") {
                $scope.bigPlace.allCountry = "true";
                $scope.bigPlace.onePart = "";
                $scope.bigPlace.oneProvince = "";
            }
            if (theItem.label == "区域") {
                $scope.bigPlace.onePart = "true";
                $scope.bigPlace.allCountry = "";
                $scope.bigPlace.oneProvince = "";
            }
            if (theItem.label == "省域") {
                $scope.bigPlace.oneProvince = "true";
                $scope.bigPlace.allCountry = "";
                $scope.bigPlace.onePart = "";
            }
        }

        /*-----------------------弹窗中单选框、下拉菜单数据绑定-----------------------*/


        /*-----------------------上传事件开始-----------------------*/
        
        $scope.$watch('files', function (newValue, oldValue) {
            if (newValue != oldValue && newValue != [] && newValue != undefined && (oldValue == [] || oldValue == undefined)) {
                if ($scope.nameListed.selected === undefined || $scope.nameListed.selected.pid === undefined) {
                    SweetAlert.swal({
                        title: "请选择要导入的图件名称！",
                        type: "error",
                        confirmButtonColor: "#007AFF"
                    });
                    newValue = null;
                    oldValue = null;
                }
                else {
                    if ($scope.scaleListed.selected === undefined || $scope.scaleListed.selected === {}) {
                        SweetAlert.swal({
                            title: "请选择比例尺分母！",
                            type: "error",
                            confirmButtonColor: "#007AFF"
                        });
                        newValue = null;
                        oldValue = null;
                    }
                    else {
                        if ($scope.editionListed.selected === undefined || $scope.editionListed.selected === {}) {
                            SweetAlert.swal({
                                title: "请选择版本！",
                                type: "error",
                                confirmButtonColor: "#007AFF"
                            });
                            newValue = null;
                            oldValue = null;
                        }
                        else {
                            var name = $scope.files.name.split(".");
                            if (name[name.length - 1] == "zip") {
                                //创建遮罩
                                abp.ui.setBusy($('.modal'));
                                abp.ui.block($('#app'));

                                fileDto.Scale = $scope.scaleListed.selected.number;
                                fileDto.GeologyMappingTypeID = $scope.nameListed.selected.id;
                                fileDto.Version = $scope.editionListed.selected.version;

                                Upload.upload({
                                    url: '/Handle/UpLoadFileHandle.ashx',
                                    file: $scope.files
                                }).then(function (resp) {

                                    //异步调用后台操作
                                    async.parallel([
                                    function (callback) {
                                        uploadTwice(resp);
                                    },
                                    function (callback) {
                                        //去掉遮罩
                                        abp.ui.clearBusy($('.modal'));
                                        abp.ui.unblock($('#app'));

                                        SweetAlert.swal({
                                            title: "上传完成！",
                                            text: "转换操作转入后台执行，请在消息通知中查看导入结果。",
                                            type: "success",
                                            confirmButtonColor: "#007AFF"
                                        });

                                        $modalInstance.dismiss('cancel');
                                        //刷新消息通知数量
                                        $scope.$emit('getLogNum');
                                    }], function (err, results) {
                                        console.log(err);
                                        console.log(results);
                                    });

                                }, function (resp) {
                                    console.log('Error!');
                                    console.log(resp);

                                    //去掉遮罩
                                    abp.ui.clearBusy($('.modal'));
                                    abp.ui.unblock($('#app'));
                                }, function (evt) {

                                });
                                
                                newValue = null;
                                oldValue = null;
                            }
                            else {
                                SweetAlert.swal({
                                    title: "请上传ZIP文件！",
                                    type: "error",
                                    confirmButtonColor: "#007AFF"
                                });
                                newValue = null;
                                oldValue = null;
                            }
                        }
                    }
                }
            }
        });


        function uploadTwice(res) {
            fileDto.ZipFilePath = res.data.Filepath[0];
            dataManage.uploadFile(fileDto).success(function () {
            }).error(function () {
            });
        }

        /*-----------------------上传事件结束-----------------------*/

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        }

    }]);

//系统管理弹出的窗口页面
app.controller('imageConfigCtrl', ['$rootScope', '$scope', '$http', "$timeout", '$modalInstance', 'allItems', 'abp.services.app.indexs', 'abp.services.app.sysSetting', 'Upload', 'SweetAlert',
    function ($rootScope, $scope, $http, $timeout, $modalInstance, allItems, indexs, sysSetting, Upload, SweetAlert) {
        $scope.title = "系统管理";

        //数据项初始化
        var imageSearchDto = {
            MappingClassName: "",
            Scale: new Number(),
            Version: new Number()
        }

        var imageSaveDto = {
            Id: "",
            DataMainID: "",
            IsDisplay: new Number(),
            Creater: "",
            CreateDT:new Date(),
            IsExist: new Number()
        }

        $scope.hasChecked = 0;
        $scope.leibieList = [];
        $scope.leibieListed = {};

        $scope.scaleList = [{
            label: "1 000 000",
            number: 1000000,
            level: "K"
        }, {
            label: "500 000",
            number: 500000,
            level: "L"
        }, {
            label: "250 000",
            number: 250000,
            level: "M"
        }, {
            label: "200 000",
            number: 200000,
            level: "N"
        }, {
            label: "100 000",
            number: 100000,
            level: "O"
        }, {
            label: "50 000",
            number: 50000,
            level: "P"
        }, {
            label: "25 000",
            number: 25000,
            level: "Q"
        }, {
            label: "10 000",
            number: 10000,
            level: "R"
        }, {
            label: "5 000",
            number: 5000,
            level: "S"
        }, {
            label: "2 000",
            number: 2000,
            level: "T"
        }, {
            label: "1 000",
            number: 1000,
            level: "U"
        }, {
            label: "500",
            number: 500,
            level: "V"
        }];
        $scope.scaleListed = {};

        $scope.editionList = [{
            label: "验收版",
            version: 1
        }, {
            label: "最终版",
            version: 2
        }];
        $scope.editionListed = {};

        for (var i = 0; i < allItems.length; i++) {
            if (allItems[i].label === "全国") {
                $scope.leibieList = allItems[i].children;
            }
        }

        //判断是否选中该图片
        $scope.isSelected = function (isExist) {
            if (isExist === 1) {
                return true;
            }
            else {
                return false;
            }
        }

        //每次点击单选框即改变是否展示该图片的状态
        $scope.updateSelection = function (e, item) {
            if ($scope.hasChecked > 7 && e.target.checked) {
                SweetAlert.swal({
                    title: "最多只能展示8张图!",
                    type: "error",
                    confirmButtonColor: "#DD6B55"
                });
            }
            else {
                imageSaveDto.Id = item.sysSettingID;
                imageSaveDto.DataMainID = item.dataMainID;
                imageSaveDto.IsDisplay = (e.target.checked ? 1 : 0);
                imageSaveDto.IsExist = item.isExist;
                //保存修改
                sysSetting.saveSysSetting(imageSaveDto).success(function () {
                    //已经选择的图片查询
                    sysSetting.getHisSysSettingList().success(function (data) {
                        $scope.chosefileList = data;
                        $scope.hasChecked = $scope.chosefileList.length;
                    }).error(function (data) {
                        console.log(data);
                    });
                }).error(function () {
                    console.log(2);
                });
            }
        }

        //图件类别下拉菜单的改变事件
        //$scope.leibieChange = function (label) {
        //    $scope.nameList = label.children;
        //    $scope.nameListed.selected = {};
        //}
        $scope.nameListed = {name:null};
        //按照搜索条件查询
        $scope.signupImageSearchForm = function () {
            if ($scope.nameListed.name != "" && $scope.nameListed.name != null && $scope.nameListed.name != undefined) {
                imageSearchDto.MappingClassName = $scope.nameListed.name;
            }
            else if ($scope.leibieListed.selected != "" && $scope.leibieListed.selected != null && $scope.leibieListed.selected != undefined) {
                imageSearchDto.MappingClassName = $scope.leibieListed.selected.label;
            }
            else {
                imageSearchDto.MappingClassName = "";
            }
            
            if ($scope.scaleListed.selected != "" && $scope.scaleListed.selected != null && $scope.scaleListed.selected != undefined) {
                imageSearchDto.Scale = $scope.scaleListed.selected.number;
            }
            
            if ($scope.editionListed.selected != "" && $scope.editionListed.selected != null && $scope.editionListed.selected != undefined) {
                imageSearchDto.Version = $scope.editionListed.selected.version;
            }
            
            imageFind(imageSearchDto);
        };

        function imageFind(dto) {
            console.log(dto);
            sysSetting.getSysSettingList(imageSearchDto).success(function (data) {
                console.log(data);
                $scope.allfileList = data;
            }).error(function (data) {
                console.log(data);
            });
        }

        //弹出页面时自动执行一次全体查询
        sysSetting.getSysSettingList(imageSearchDto).success(function (data) {
            console.log(data);
            $scope.allfileList = data;
        }).error(function (data) {
            console.log(data);
        });


        //已经选择的图片查询
        sysSetting.getHisSysSettingList().success(function (data) {
            console.log(data);
            $scope.chosefileList = data;
            $scope.hasChecked = $scope.chosefileList.length;
        }).error(function (data) {
            console.log(data);
        });

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        }
    }]);

//将比例尺分母转换成比例尺正常显示
app.filter('exScale', function () {
    return function (Scale, str) {
        switch (Scale) {
            case 20000000:
                Scale = "1:20 000 000";
                break;
            case 15000000:
                Scale = "1:15 000 000";
                break;
            case 12000000:
                Scale = "1:12 000 000";
                break;
            case 10000000:
                Scale = "1:10 000 000";
                break;
            case 7500000:
                Scale = "1:7 500 000";
                break;
            case 6000000:
                Scale = "1:6 000 000";
                break;
            case 5000000:
                Scale = "1:5 000 000";
                break;
            case 4000000:
                Scale = "1:4 000 000";
                break;
            case 2500000:
                Scale = "1:2 500 000";
                break;
            case 1000000:
                Scale = "1:1 000 000";
                break;
            case 500000:
                Scale = "1:500 000";
                break;
            case 250000:
                Scale = "1:250 000";
                break;
            case 200000:
                Scale = "1:200 000";
                break;
            case 100000:
                Scale = "1:100 000";
                break;
            case 50000:
                Scale = "1:50 000";
                break;
            case 25000:
                Scale = "1:25 000";
                break;
            case 10000:
                Scale = "1:10 000";
                break;
            case 5000:
                Scale = "1:5 000";
                break;
            case 2000:
                Scale = "1:2 000";
                break;
            case 1000:
                Scale = "1:1 000";
                break;
            case 500:
                Scale = "1:500";
                break;
            default:
                Scale = "Wrong!";
                break;
        }
        return Scale;
    }
});

//将版本转换成版本汉字显示
app.filter('exVersion', function () {
    return function (Version, str) {
        switch (Version) {
            case 1:
                Version = "验收版";
                break;
            case 2:
                Version = "最终版";
                break;
            default:
                Version = "Wrong!";
                break;
        }
        return Version;
    }
}); 