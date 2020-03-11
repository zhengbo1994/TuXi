'use strict';

/**
 * Config for the router
 */
app.config(['$stateProvider', '$urlRouterProvider', '$controllerProvider', '$compileProvider', '$filterProvider', '$provide', '$ocLazyLoadProvider', 'JS_REQUIRES',
function ($stateProvider, $urlRouterProvider, $controllerProvider, $compileProvider, $filterProvider, $provide, $ocLazyLoadProvider, jsRequires) {

    app.controller = $controllerProvider.register;
    app.directive = $compileProvider.directive;
    app.filter = $filterProvider.register;
    app.factory = $provide.factory;
    app.service = $provide.service;
    app.constant = $provide.constant;
    app.value = $provide.value;

    // LAZY MODULES

    $ocLazyLoadProvider.config({
        debug: false,
        events: true,
        modules: jsRequires.modules
    });

    // APPLICATION ROUTES
    // For any unmatched url, redirect to /app/dashboard
    $urlRouterProvider.otherwise("/app/home");
    //
    // Set up the states
    $stateProvider.state('app', {
        url: "/app",
        templateUrl: "/App/Main/assets/views/app.html",
        resolve: loadSequence('modernizr', 'moment', 'angularMoment', 'uiSwitch', "propsFilter", 'perfect-scrollbar-plugin', 'toaster', 'ngAside', 'vAccordion', 'sweet-alert', 'oitozero.ngSweetAlert', 'jquery-mobile', 'downListCtrl', 'openlayerjs', 'itellurojs', 'measuretooljs', 'openMaps', 'socket', 'micUiModals', 'touchspin-plugin', 'tableDirective', 'btnUpload'),
        abstract: true
    }).state('app.layerManager', {
        url: "/layerManager",
        templateUrl: "/App/Main/assets/views/layerManager/page1.html",
        resolve: loadSequence("layerManagerCtrl", "angularBootstrapNavTree", "ui.select", "angularFileUpload", "webuploader", "bpUploadHtml", "bpUploadHtmlBtn"),
        title: '图层管理'
    }).state('app.layerBrowse', {
        url: "/layerBrowse",
        templateUrl: "/App/Main/assets/views/layerManager/page2.html",
        resolve: loadSequence("layerBrowseCtrl", "angularBootstrapNavTree", "ui.select", "angularFileUpload", "webuploader", "bpUploadHtmlBtn", "jquery-spectrum"),
        title: '图层浏览'
    }).state('app.mapManager', {
        url: "/mapManager",
        templateUrl: "/App/Main/assets/views/mapManager/page1.html",
        title: '地图管理',
        resolve: loadSequence("mapManagerCtrl", "angularBootstrapNavTree", "ui.select", "wang-editor", "angularFileUpload", 'ngDraggable')
    }).state('app.mapBrowse', {
        url: "/mapBrowse",
        templateUrl: "/App/Main/assets/views/mapManager/page2.html",
        resolve: loadSequence("mapBrowseCtrl", "angularBootstrapNavTree", "ui.select", 'viewer', 'ngDraggable', "jquery-spectrum", "webuploader"),
        title: '地图浏览'
    }).state('app.styleManager', {
        url: "/styleManager",
        templateUrl: "/App/Main/assets/views/styleManager/page.html",
        title: '样式管理',
        resolve: loadSequence("styleManagerCtrl", "angularBootstrapNavTree", "ui.select", "wang-editor", "angularFileUpload", 'webuploader', 'bpUploadHtmlBtn', 'jquery-spectrum')
    }).state('app.mapSearch', {
        url: "/mapSearch",
        templateUrl: "/App/Main/assets/views/mapSearch/page.html",
        title: '地图查询',
        resolve: loadSequence("mapSearchCtrl", "angularBootstrapNavTree", "ui.select", 'viewer')
    }).state('app.dataInterface', {
        url: "/dataInterface",
        templateUrl: "/App/Main/assets/views/dataInterface/page.html",
        title: '服务接口',
        resolve: loadSequence("dataInterfaceCtrl", 'angularBootstrapNavTree')
    }).state('app.home', {
        url: "/home",
        templateUrl: "/App/Main/assets/views/home.html",
        title: '首页',
        resolve: loadSequence("homeCtrl", 'echartJs', 'viewer', "ui.select")
    }).state('app.sysSet', {
        url: '/tools',
        template: '<div ui-view class="fade-in-up"></div>',
        title: '系统设置',
        ncyBreadcrumb: {
            label: '系统设置'
        }
    }).state('app.sysSet.setSys', {
        url: "/setSys",
        templateUrl: "/App/Main/assets/views/sysSet/page1.html",
        title: '设置',
        resolve: loadSequence("setSysCtrl", "angularBootstrapNavTree")
    }).state('app.sysSet.userManager', {
        url: "/userManager",
        templateUrl: "/App/Main/assets/views/sysSet/page2.html",
        title: '用户管理',
        resolve: loadSequence("userManagerCtrl", "angularBootstrapNavTree")
    }).state('app.sysSet.sysLog', {
        url: "/sysLog",
        templateUrl: "/App/Main/assets/views/sysSet/page3.html",
        title: '系统日志',
        resolve: loadSequence("sysLogCtrl", "ui.select", "angularBootstrapNavTree", "calendars")
    }).state('app.dataInterface2', {
        url: "/dataInterface2",
        templateUrl: "/App/Main/assets/views/dataInterface/page2.html",
        title: '服务接口',
        resolve: loadSequence("dataInterfaceCtrl2", 'propsFilter', 'angularBootstrapNavTree')
    }).state('app.mapService', {
        url: "/mapService",
        templateUrl: "/App/Main/assets/views/dataInterface/page3.html",
        title: '地图服务',
        resolve: loadSequence("mapServicePageCtrl", 'angularBootstrapNavTree')
    }).state('app.serviceDetail', {
        url: "/serviceDetail",
        templateUrl: "/App/Main/assets/views/dataInterface/page4.html",
        title: '服务详情',
        resolve: loadSequence("mapServiceDetailCtrl", 'angularBootstrapNavTree')
    }).state('app.layerEdit', {
        url: "/layerEdit",
        templateUrl: "/App/Main/assets/views/dataInterface/testEdit.html",
        title: '图层编辑',
        resolve: loadSequence('openlayerjs', 'itellurojs', 'measuretooljs', "layerEditCtrl", 'angularBootstrapNavTree', "ol3-ext")
    }).state('app.tools', {
        url: '/tools',
        template: '<div ui-view class="fade-in-up"></div>',
        title: '工具集',
        ncyBreadcrumb: {
            label: 'UI Elements'
        }
    }).state('app.thematicMapping', {
        url: '/ThematicMapping',
        cache: 'false',
        templateUrl: "App/Main/assets/views/ThematicMapping/page.html",
        title: '专题制作',
        resolve: loadSequence("dataInterfaceCtrl2", 'propsFilter', 'angularBootstrapNavTree')
    }).state('app.mapEditor', {
        url: '/ThematicMapping',
        cache: 'false',
        templateUrl: "App/Main/assets/views/MapEditor/page.html",
        title: '数据编辑',
        resolve: loadSequence("dataInterfaceCtrl2", 'propsFilter', 'angularBootstrapNavTree')
    }).state('app.dataManage', {
        url: '/dataManage',
        cache: 'false',
        templateUrl: "App/Main/assets/views/dataManage/page.html",
        title: '数据管理',
        resolve: loadSequence('ui.select', 'angularBootstrapNavTree'),
            //, 'dataManageCtrl'),
        ncyBreadcrumb: {
            label: '数据管理'
        }
    })
        .state('app.tools.coordinateConversion', {
        url: '/coordinateConversion',
        cache: 'false',
        templateUrl: "App/Main/assets/views/Tools/coordinateConversion.html",
        title: '坐标转换',
        resolve: loadSequence('coordinateCtrl', 'ngUploadCtrl', 'ngUploadMinCtrl', 'angularBootstrapNavTree', "ui.select", "webuploader", "bpUploadHtmlBtn"),
        ncyBreadcrumb: {
            label: '坐标转换'
        }
    }).state('app.tools.formatConversion', {
        url: '/formatConversion',
        cache: 'false',
        templateUrl: "App/Main/assets/views/Tools/formatConversion.html",
        title: '格式转换',
        resolve: loadSequence('formatCtrl', 'ngUploadCtrl', 'ngUploadMinCtrl', 'angularBootstrapNavTree', "webuploader", "bpUploadHtmlBtn"),
        ncyBreadcrumb: {
            label: '格式转换'
        }
    }).state('app.tools.shadowConversion', {
        url: '/shadowConversion',
        cache: 'false',
        templateUrl: "App/Main/assets/views/Tools/shadowConversion.html",
        title: '投影转换',
        resolve: loadSequence('shadowCtrl', 'ngUploadCtrl', 'ngUploadMinCtrl', 'angularBootstrapNavTree', "ui.select", "webuploader", "bpUploadHtmlBtn"),
        ncyBreadcrumb: {
            label: '投影转换'
        }
    }).state('app.tools.propertyCheck', {
        url: '/propertyCheck',
        cache: 'false',
        templateUrl: "App/Main/assets/views/Tools/propertyCheck.html",
        title: '属性检查',
        resolve: loadSequence('procheckCtrl', 'ngUploadCtrl', 'ngUploadMinCtrl'),
        ncyBreadcrumb: {
            label: '属性检查'
        }
    });
    // Login routes

    // Generates a resolve object previously configured in constant.JS_REQUIRES (config.constant.js)
    function loadSequence() {
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
}]);