'use strict';
/** 
  * 断点上传控件(只有一个按钮)
  * by huqr
*/

app.directive('bpUploadBtn', function ($timeout, SweetAlert, $filter) {
    var _ajax = function (guid, fileName, backFun) {
        $.ajax({
            url: "/UploadForBreakpoint/FileMerge",
            data: { guid: guid, fileName: fileName },
            type: "POST",
            dataType: "json",
            headers: { Authorization: 'Bearer ' + AccessTokenValues },
            success: function (ret, status) {
                if (status === 'success') {
                    backFun(ret.result);
                }
            },
            error: function (msg, textStatus) {
                backFun('error');
            }
        });
    };


    return {
        restrict: 'E',
        template: function (a, b) {
            //var btnWord = !!b.btnWord && typeof (b.btnWord) === 'string' ? b.btnWord : '上传';

            var tmp = '<div class="font-title-btn">' + a.text() + '</div>';
            return tmp;
        },
        scope: {
            queue: '=?',
            //btnWord: '=?',
            mimeTypes: '@?',
            fileType: '@?',
            fileSize: '=?',
            onAllSuccess: '=?',
            onBeforeAdd: '=?',
            //onAfterAddingfile: '=?',
            //onAfterAddingall: '=?',
        },
        replace: true,
        link: function (scope, element) {
            scope.queue = {};
            //scope.btnWord = !!scope.btnWord && typeof (scope.btnWord) === 'string' ? scope.btnWord : '上传';
            var GUID = WebUploader.Base.guid();
            //并发上传（多线程上传）
            var uploader = WebUploader.create({
                //兼容老版本IE
                swf: '/Scripts/webuploader-0.1.5/Uploader.swf',
                // 文件接收服务端
                server: '/UploadForBreakpoint/AddFiles',
                // 开起分片上传
                chunked: true,
                //分片大小
                chunkSize: 2097152,
                //单个文件大小限制
                fileSingleSizeLimit: parseInt(scope.fileSize) > 0 ? parseInt(scope.fileSize) : undefined,
                //文件类型
                accept: {
                    mimeTypes: !scope.mimeTypes ? '' : scope.mimeTypes,
                    extensions: typeof (scope.fileType) === 'undefined' ? '' : scope.fileType
                },
                //允许自动重传次数
                chunkRetry: 5,
                //上传并发数
                threads: 5,
                formData: {
                    guid: GUID
                },
                // 选择文件的按钮
                pick: {
                    id: element[0],//element.children('.bp-upload-btn')
                    innerHTML: $filter('translate')('views.Layer.create.newFrom.upload.select')
                }
            });

            uploader.on('beforeFileQueued', function (a, b) {
                //console.info('Debug: beforeFileQueued    ', a, b);
                if (typeof (uploader.options.fileSingleSizeLimit) !== 'undefined' && a.size > uploader.options.fileSingleSizeLimit) {
                    SweetAlert.swal({
                        title: a.name + $filter('translate')('views.Toolset.CoordinateConversion.fileToobig'),
                        text: "",
                        type: "warning",
                        confirmButtonColor: "#007AFF"
                    });
                }
                if (uploader.options.accept[0].extensions !== "" && uploader.options.accept[0].extensions.indexOf(a.ext) === -1) {
                    SweetAlert.swal({
                        title: $filter('translate')('views.Toolset.CoordinateConversion.uploadPointType'),
                        text: $filter('translate')('views.Toolset.CoordinateConversion.pointFileType') + uploader.options.accept[0].extensions,
                        type: "warning",
                        confirmButtonColor: "#007AFF"
                    });
                }
            });

            //做上传失败重试
            var tmp = {}
            /*uploader.retry()*/

            scope.total = 0;
            //添加文件后触发
            uploader.on('fileQueued', function (a) {
                //用来触发文件上传 
                if (typeof (scope.onBeforeAdd) === 'function') {
                    scope.onBeforeAdd();
                }
                uploader.upload();
                scope.total++;
                $timeout(function () {
                    scope.queue[a.name] = { name: a.name, size: a.size, type: a.type, progress: 0, id: a.id, loaded: a.loaded, source: a.source, statusText: a.statusText };
                    //console.log(scope.queue);
                });
            });

            //设置返回队列参数
            function setQueue(k, ret) {
                $timeout(function () {
                    //console.log('Debug: ret   ', ret);
                    if (ret === 'error') {
                        scope.queue[k].msg = '上传失败';
                        scope.queue[k].progress = 0;
                        SweetAlert.swal({
                            title: scope.queue[k].name + "上传失败!",
                            text: "",
                            type: "error",
                            confirmButtonColor: "#007AFF"
                        });

                    } else {
                        scope.queue[k].msg = '上传成功';
                        scope.queue[k].progress = 100;
                        scope.queue[k].extension = ret.extension;
                        scope.queue[k].fileGuid = ret.fileGuid;
                        scope.queue[k].size = ret.fileSize;
                        scope.queue[k].httpPath = ret.httpPath;
                        scope.queue[k].logicName = ret.logicName;
                        scope.queue[k].physicalName = ret.physicalName;
                        scope.queue[k].physicalPath = ret.physicalPath;
                        
                    }
                });
            }

            var MergeArr = [];
            uploader.on('uploadSuccess', function (a, b) {
                MergeArr.push(a.name);
                //console.log(scope.queue);
            });
            uploader.on('uploadAccept', function (a, b) {
                //console.info('Debug: uploadAccept  ', a, b);
            });
            uploader.on('uploadError', function (a, b) {
                //console.error('Debug: Error  ', a, b);
            });

            uploader.on('uploadFinished', function () {
                var arr = angular.copy(MergeArr.reverse());
                MergeArr = [];
                var i = arr.length - 1;

                var callback = function (a) {
                    //设置返回值
                    setQueue(arr[i], a);
                    i--;
                    if (!!arr[i]) {
                        //递归上传
                        _ajax(GUID, arr[i], callback);
                    }
                    else if (i === -1) {
                        //全部上传成功
                        $timeout(function () {
                            SweetAlert.swal({
                                title: $filter('translate')('views.Style.alertFun.UploadSuccessfully'),
                                text: "",
                                type: "success",
                                confirmButtonColor: "#007AFF"
                            });
                            scope.onAllSuccess(scope.queue);
                            scope.queue = {};
                            uploader.reset();
                        });
                    }
                };
                _ajax(GUID, arr[i], callback);
            });
        }
    }
});