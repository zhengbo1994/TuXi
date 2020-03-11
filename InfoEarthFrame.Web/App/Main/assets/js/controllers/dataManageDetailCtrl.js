/// <reference path="../../../../../PDFJSInNet/web/viewer.html" />
/**
 * Created by admin on 2016/8/8.
 */
app.controller('dataManageDetailCtrl', ["$scope", "$timeout", "$state", 'Upload', "$localStorage", '$stateParams', 'abp.services.app.dataManage', "$sce",
    function ($scope, $timeout, $state, Upload, $localStorage, $stateParams, dataManage, $sce) {
    $scope.detailId = $stateParams.fileId;
    $scope.detailName = $stateParams.fileName;
    $scope.metaDataID = $stateParams.metaDataId;
    var fileDto = {
        MainID: ""
    }
    fileDto.MainID = $scope.detailId;

    var loadClass = new Array();

    //*********************加载树开始*********************
    $scope.treeList = [{
        label: $scope.detailName,
        children:[{
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

    dataManage.getFileList(fileDto).success(function (data) {
        var temp = [];
        angular.forEach(data, function (val) {
            temp.push(treeFormat(val));
        });
        $scope.treeList = trans($scope.treeList, temp);

    });
    //*********************加载树结束*********************

    //$timeout(function () {
    //    //展开树的全部节点
    //    $scope.my_tree.expand_all();
    //}, 1000);

    //选择树的节点触发事件
    $scope.my_tree_handler = function (branch) {
        debugger;
        //$scope.docIframe = "PDFJSInNet/web/viewer.html?file=/TempFile/09.pdf";
        
        if (branch.level == 2 && branch.uid == 12) {
            $scope.docIframe = $sce.trustAsResourceUrl('/#viewImage/' + $scope.detailId);
        }

        if (branch.level == 3) {
            var type = branch.data;
            var sign = 0;

            for (var i = 0; i < loadClass.length; i++) {
                if (branch.uid == loadClass[i].id) {
                    if (type == ".docx" || type == ".doc" || type == ".xls" || type == ".xlsx") {
                        uploadTwice(loadClass[i].url);
                    }
                    else if (type == ".jpg" || type == ".png" || type == ".bmp" || type == ".gif") {
                        $scope.docIframe = $sce.trustAsResourceUrl(loadClass[i].url);
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

                        if (type == ".docx" || type == ".doc") {
                            uploadTwice(response.data);
                        }
                        else if (type == ".jpg" || type == ".png") {
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
            
        }
        else if (branch.level == 2 && branch.label == "元数据") {
            $scope.docIframe = $sce.trustAsResourceUrl("/#dataForDetail/" + $scope.metaDataID);
        }
    }

    function uploadTwice(path) {
        alert(path);
        $.post("/Handle/OfficeOnlineConfig.ashx", { filepath: path }, function (data) {
            //console.log(data);
            $timeout(function () {
                $scope.docIframe = $sce.trustAsResourceUrl(data);
            }, 100);
            
        });
    }
}]);
