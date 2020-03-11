'use strict';
/** 
  * 弹出框
  * by licx
*/

app.directive('micUiModal', function ($modal) {
    /* ****弹框封装函数****
    * @param {string} tpl     模板名称 （必填）
    * @param {string} ctrl    控制器名称（必填）
    * @param {string} size    弹框大小 size = 'sm' 或  size = 'lg'
    * @param {string} winCls  自定义样式名称（非必填）
    * @param {string} backdrop  控制遮罩层 true(默认),flase(无背景) "static"-背景是存在的，但点击模态窗口之外时，模态窗口不关闭。
    * @param {string} resolve 1.为一个对象，定义一个或多个成员并传递给$modal指定的控制器。如果需要传递一个object对象，需要使用angular.copy()。2.如果需要载入资源文件（如JS路径）则定义一个function,相当于routes的一个reslove属性;（非必填）
    * @param {string} openedFun 模态窗口打开之后执行的回调函数 （非必填）
    * @param {string} backFun 模态窗口关团时执行的回调函数，对应$modalInstance.close('这是关闭窗口回传的'); （非必填）
    */
    var popWin = function (tpl, ctrl, size, winCls, backdrop, resolve, openedFun) {
        if (backdrop === undefined || backdrop === null || backdrop === '' || backdrop !== 'static') { backdrop = true; }

        if (resolve === 'map') {
            resolve = loadSequences('openlayerjs', 'itellurojs');
        }
        var m = $modal.open({
            template: tpl,
            controller: ctrl,
            backdrop: backdrop,
            size: size,
            windowClass: winCls,
            resolve: resolve ? resolve : {}
        });
        m.opened.then(function (a) {//模态窗口打开之后执行的函数
            if (!openedFun||!angular.isFunction(openedFun)) {return;}
            openedFun(a);
        });
    };
    function loadSequences() {//加载JS
        var _args = arguments;
        return {
            deps: ['$ocLazyLoad', '$q',
			function ($ocLL, $q) {
			    var promise = $q.when(1);
			    for (var i = 0, len = _args.length; i < len; i++) {
			        promise = promiseThen(_args[i]);
			    }
			    return promise;

			    function promiseThen(_arg) {
			        if (typeof _arg == 'function')
			            return promise.then(_arg);
			        else
			            return promise.then(function () {
			                var nowLoad = requiredData(_arg);
			                if (!nowLoad)
			                    return $.error('Route resolve: Bad resource name [' + _arg + ']');
			                return $ocLL.load(nowLoad);
			            });
			    }

			    function requiredData(name) {
			        if (jsRequires.modules)
			            for (var m in jsRequires.modules)
			                if (jsRequires.modules[m].name && jsRequires.modules[m].name === name)
			                    return jsRequires.modules[m];
			        return jsRequires.scripts && jsRequires.scripts[name];
			    }
			}]
        };
    }
    return {
        restrict: 'E',
        template: '<div ng-click="openPopwinFun()" ng-transclude>' +
            '</div>',
        scope: {
            popwinTitle: '=?',
            popwinIcon: '=?',
            popwinmodal: '=?',
            resolve: '@?',
            templateUrl: '@?',
            template: '=?',
            size: '@?',
            winClass: '@?',
            backdrop: '=?',
            onOpened: '=?',
            onSubmit: '=?',
            onCancel: '=?',
            openPopwinFun: '=?',
            submitText: '@?',
            cancelText: '@?'
            },
        replace: true,
        transclude: true,
        controller: function ($scope, SweetAlert, $templateCache) {
			function _rdom() {
				return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
			}
            $scope.ctrl = 'popwinsCtrl_'+_rdom()+'_' + new Date().getTime();
            $scope.openPopwinFun = function () {
                var isShowSubmit = !!$scope.submitText;
                var isShowCancel = !!$scope.cancelText;

                var tmls = $scope.template ? $scope.template :
                    '    <!-- 表单 -->' +
                    '    <form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
                    $templateCache.get($scope.templateUrl) +
                    '        <div class="form-group" ng-if="' + (isShowSubmit || isShowCancel) + '" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
                    '            <button type="submit" ng-if="' + isShowSubmit + '" style="min-width: 80px; float: right; margin-left: 0.5em;" class="btn btn-wide btn-primary">' + $scope.submitText + '</button>' +
                    '            <a style="float: right; min-width: 80px;" ng-if="' + isShowCancel + '" class="btn btn-wide btn-o btn-default" ng-click="submitForm.cancel()">' + $scope.cancelText + '</a>' +
                    '        </div>' +
                    '    </form>';

                tmls = '<!-- Head -->' +
                    '<div class="modal-header" style="padding:10px">' +
                    '    <span class="font-title-middle" style="margin-bottom: 0px;" ng-class="icon">{{title}}</span>' +
                    '    <a class="my-pop-win-close" href="#" ng-click="closePop()"><i class="ti-close" tooltip="{{\'setting.close\'|translate}}"></i></a>' +
                    '</div>' +
                    '<!-- Body -->' +
                    '<div class="modal-body form-error">' +
                    tmls+
                    '</div>';
                popWin(tmls, $scope.ctrl, $scope.size, $scope.winClass, $scope.backdrop, $scope.resolve, $scope.onOpened);
            };
        },
        link: function ($scope, $element, $attributes) {            
            var mod = {
                ppl: $scope.popwinmodal,
                ttl: $scope.popwinTitle,
                icon: $scope.popwinIcon
            },
            onSubmit = $scope.onSubmit,
            onCancel = $scope.onCancel;

            $scope.$watch('popwinmodal', function (val) {
                mod.ppl = val;
            });
            $scope.$watch('popwinTitle', function (val) {
                mod.ttl = val;
            });
            $scope.$watch('popwinIcon', function (val) {
                mod.icon = val;
            });

            app.controller($scope.ctrl, ['$scope', '$modalInstance', 'SweetAlert', function ($scope, $modalInstance, SweetAlert) {
                $scope.$modalInstance = $modalInstance;

                $scope.popwinmodal = mod.ppl;
                $scope.title = mod.ttl;
                $scope.icon = mod.icon;

                $scope.$watch(function () {
                    return mod;
                }, function (val) {
                    $scope.title = val.ttl;
                    $scope.icon = val.icon;
                    $scope.popwinmodal = val.ppl;
                });

                $modalInstance.result.finally(function () {
                    $scope.submitForm.cancel();
                });

                //提交
                $scope.submitForm = {
                    submit: function (form) {
                        if (form.$invalid) {//表单数据校验
                            var field = null, firstError = null;
                            for (field in form) {
                                if (field[0] != '$') {
                                    if (firstError === null && !form[field].$valid) {
                                        firstError = form[field].$name;
                                    }
                                    if (form[field].$pristine) {
                                        form[field].$dirty = true;
                                    }
                                }
                            }
                            return;
                        } else {//校验通过提交
                            if (!!onSubmit&&angular.isFunction(onSubmit))
                                onSubmit($modalInstance, form);
                            else if (!!onSubmit && !!onSubmit.submit && angular.isFunction(onSubmit.submit))
                                onSubmit.submit($modalInstance, form);
                        }
                    },
                    cancel: function () {
                        if (!!onCancel&&angular.isFunction(onCancel))
                            onCancel($modalInstance);
                        else if (!!onSubmit && !!onSubmit.cancel && angular.isFunction(onSubmit.cancel)) {
                            onSubmit.cancel($modalInstance);
                            return;
                        }
                        $modalInstance.dismiss('cancel');
                    }
                };
                //关闭弹框
                $scope.closePop = function () {
                    $scope.submitForm.cancel();
                };
            }]);
        }
    }
});