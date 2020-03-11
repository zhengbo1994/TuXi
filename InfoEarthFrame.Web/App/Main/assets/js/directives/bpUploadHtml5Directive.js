'use strict';
/** 
  * 断点上传控件
  * by licx
*/

app.directive('bpUpload', function ($timeout, SweetAlert, $filter) {
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
        template: function () {
            var tmp = '<div style="border: solid 1px #eeeeee; padding: 20px;">' +
                '    <div class="bp-upload-btn col-sm-6">' +
                '        <i class="ti-upload" style=""></i>&nbsp;选择文件' +
                '    </div>' +
                '    <div class="col-sm-6">' +
                '        <button class="btn btn-primary btn-o font-title-btn" type="button" style="float: right; margin-left: 0.3%; margin-right: 1.2%;" ng-click="delAll()" ng-disabled="!total">\r\n' +
                '           <span class="glyphicon glyphicon-trash"></span> <span translate="views.Layer.create.newFrom.upload.deleteAll">删除所有</span>\r\n' +
                '        </button>\r\n' +
                '        <button class="btn btn-primary font-title-btn" type="button" style="float: right;" ng-click="uploadAll()" ng-disabled="!total || totalProgress==100 || hasUploadNum>=total">\r\n' +
                '           <span class="glyphicon glyphicon-upload"></span> <span translate="views.Layer.create.newFrom.upload.uploadAll">上传所有</span>\r\n' +
                '        </button>\r\n' +
                '    </div>' +
                '    <div class="col-sm-12">\r\n' +
                '        <div class="table-responsive">\r\n' +
                '            <table class="table">\r\n' +
                '                <thead class="font-title-small">\r\n' +
                '                    <tr>\r\n' +
                '                        <th width="50%" translate="views.Layer.create.newFrom.name">名称</th>\r\n' +
                '                        <th translate="views.Layer.create.newFrom.size">大小</th>\r\n' +
                '                        <th translate="views.Layer.create.newFrom.progress">上传进度</th>\r\n' +
                '                        <th class="text-center" translate="views.Layer.create.newFrom.status">状态</th>\r\n' +
                '                        <th translate="setting.operating">操作</th>\r\n' +
                '                    </tr>\r\n' +
                '                </thead>\r\n' +
                '                <tbody class="font-content-small">\r\n' +
                '                    <tr ng-repeat="(a,b) in queue">\r\n' +
                '                        <td>\r\n' +
                '                            {{ a }}\r\n' +
                '                        </td>\r\n' +
                '                        <td ng-show="true" nowrap>\r\n' +
				'                            <span ng-if="(b.size/1024/1024/1024|number:2)>1">{{ b.size/1024/1024/1024|number:2 }} GB</span>\r\n' +
                '                            <span ng-if="(b.size/1024/1024|number:2)>1">{{ b.size/1024/1024|number:2 }} MB</span>\r\n' +
                '                            <span ng-if="(b.size/1024|number:2)>1">{{ b.size/1024|number:2 }} KB</span>\r\n' +
                '                            <span ng-if="(b.size|number:2)>1">{{ b.size|number:2 }} B</span>\r\n' +
                '                        </td>\r\n' +
                '                        <td ng-show="true">\r\n' +
                '                            <div class="progress progress-xs margin-bottom-0">\r\n' +
                '                                <div class="progress-bar" role="progressbar" ng-style="{ \'width\': b.progress + \'%\' }"></div>\r\n' +
                '                            </div>\r\n' +
                '                        </td>\r\n' +
                '                        <td class="text-center">\r\n' +
                '                            <span ng-show="b.msg===\'上传成功\'"><i class="glyphicon glyphicon-ok"></i></span>\r\n' +
                '                            <span ng-show="b.msg===\'上传失败\'"><i class="glyphicon glyphicon-ban-circle"></i></span>\r\n' +
                //'                            <span ng-show="item.msg"><i class="glyphicon glyphicon-remove"></i></span>\r\n' +
                '                        </td>\r\n' +
                '                        <td nowrap>\r\n' +
                '                            <button type="button" class="btn btn-success btn-xs" ng-click="uploadOne(b)" ng-disabled="b.progress > 0 || b.msg===\'上传成功\' || b.msg===\'上传失败\'">\r\n' +
                '                                <span class="glyphicon glyphicon-upload"></span> <span translate="views.Layer.create.newFrom.upload.uploadOne">上传</span>\r\n' +
                '                            </button>\r\n' +
                '                            <button type="button" class="btn btn-danger btn-xs" ng-click="delOne(b)">\r\n' +
                '                                <span class="glyphicon glyphicon-trash"></span> <span translate="setting.delete">删除</span>\r\n' +
                '                            </button>\r\n' +
                '                        </td>\r\n' +
                '                    </tr>\r\n' +
                '                </tbody>\r\n' +
                '            </table>\r\n' +
                '        </div><span class="font-title-small" translate="views.Layer.create.newFrom.totalPro">上传总进度:</span>\r\n' +
                '        <div class="progress progress-xs margin-top-5 margin-bottom-20">\r\n' +
                '           <div class="progress-bar" role="progressbar" ng-style="{ \'width\': totalProgress + \'%\' }"></div>\r\n' +
                '        </div>\r\n' +
                '    </div>' +
                '<div style="clear: both;"></div>' +
                '</div>';
            return tmp;
        },
        scope: {
            //queue: '=?',

            del: '=?',
            totalProgress: '=?',
            url: '=?',

            fileType: '@?',
            fileSize: '=?',
            //上传文件之前执行的方法
            beforeAddfile: '=?',
            //上传文件之后执行的方法
            afterAddfile: '=?',
            //单个文件上传成功之后执行的事件，返回该文件信息
            onSuccess: '=?',
            //删除列表中的所有文件
            onDelAll: '=?',
            //删除列表中的单个文件
            onDelSingle: '=?'
        },
        replace: true,
        link: function (scope, element) {
            scope.queue = {};

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
                    id: element.children('.bp-upload-btn'),
                    innerHTML: $filter('translate')('views.Layer.create.newFrom.upload.select')
                }
            });

            uploader.on('beforeFileQueued', function (a, b) {
                //console.info('Debug: beforeFileQueued    ', a, b);
                if (typeof (scope.beforeAddfile) === 'function') {
                    scope.beforeAddfile();
                }
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
                scope.total++;
                totalProgress();
                $timeout(function () {
                    scope.queue[a.name] = { name: a.name, size: a.size, type: a.type, progress: 0, id: a.id, loaded: a.loaded, source: a.source, statusText: a.statusText };
                    //console.log(scope.queue);
                    if (typeof (scope.afterAddfile) === 'function') {
                        scope.afterAddfile();
                    }
                });
            });

            scope.totalProgress = 0;
            //单个进度
            uploader.on('uploadProgress', function (a, b) {
                $timeout(function () {
                    scope.queue[a.name].progress = Math.floor((b * 100));
                    totalProgress();
                });
            });

            //总体进度设置
            function totalProgress() {
                var tmpProgress = 0;
                for (var i in scope.queue) {
                    tmpProgress += scope.queue[i].progress;
                }
                scope.totalProgress = scope.total !== 0 ? Math.floor(tmpProgress / scope.total) : 0;
            }

            //上传所有
            scope.uploadAll = function () {
                uploader.upload();
            }

            //已经上传完成的文件数量和正在上传的文件数量之和
            var hasUploadNum = 0;
            //上传单个文件
            scope.uploadOne = function (file) {
                uploader.upload(file.id);
                var obj = uploader.getStats();
                hasUploadNum = obj.queueNum + obj.successNum;
            }

            //删除所有
            scope.delAll = function () {
                SweetAlert.swal({
                    title: $filter('translate')('views.Layer.create.newFrom.upload.confirmDelAll'),
                    text: "",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: $filter('translate')('setting.sure'),
                    cancelButtonText: $filter('translate')('setting.cancel')
                }, function (isConfirm) {
                    if (isConfirm) {
                        uploader.reset();
                        delete scope.queue;
                        scope.queue = {};
                        scope.total = 0;
                        scope.totalProgress = 0;
                        scope.onDelAll();
                    }
                });
            }

            //删除单个文件
            scope.delOne = function (file) {
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
                        uploader.removeFile(file.id, true);
                        delete scope.queue[file.name];
                        scope.total--;
                        totalProgress();
                        scope.onDelSingle(file);
                    }
                });
            }

            //设置返回队列参数
            function setQueue(k, ret) {
                $timeout(function () {
                    //console.log('Debug: ret   ', ret);
                    if (ret === 'error') {
                        scope.queue[k].msg = '上传失败';
                        scope.queue[k].progress = 0;
                        totalProgress();

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
                        scope.onSuccess(scope.queue[k]);
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
                            //scope.totalProgress = 100;
                        });
                    }
                };
                _ajax(GUID, arr[i], callback);
            });
        }
    }
});