var app = angular.module('InfoearthApp', ['clip-two']);
app.run(['$rootScope', '$state', '$stateParams', '$cacheFactory', '$modal',
function ($rootScope, $state, $stateParams, $cacheFactory, $modal) {
    // Attach Fastclick for eliminating the 300ms delay between a physical tap and the firing of a click event on mobile browsers
    //FastClick.attach(document.body);

    // Set some reference to access them from any scope
    $rootScope.$state = $state;
    $rootScope.$stateParams = $stateParams;
    //注销
    //$rootScope.logOut = function () {
    //    abp.message.confirm('退出系统', '是否确认注销系统', function (a) {
    //        if (a) {
    //            window.location.href = "/Home/Logout";
    //        }
    //    });
    //};

    // GLOBAL APP SCOPE
    // set below basic information
    $rootScope.app = {
        name: '空间数据管理系统', // name of your project
        author: '武汉地大信息工程股份有限公司', // author's name or company name
        description: 'Infoeath Admin Template', // brief description
        version: '1.0.0', // current version
        year: ((new Date()).getFullYear()), // automatic current year (for copyright information)
        isMobile: (function () {// true if the browser is a mobile device
            var check = false;
            if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
                check = true;
            };
            return check;
        })(),
        layout: {
            isNavbarFixed: true, //true if you want to initialize the template with fixed header
            isSidebarFixed: true, // true if you want to initialize the template with fixed sidebar
            isSidebarClosed: false, // true if you want to initialize the template with closed sidebar
            isFooterFixed: false, // true if you want to initialize the template with fixed footer
            theme: 'theme-3', // indicate the theme chosen for your project
            logo: 'App/Main/assets/images/logo.png' // relative path of the project logo
        },
        isWholeScreen: false,
        user: localStorage.getItem('infoearth_spacedata_userCode')
    };
    //$rootScope.user = {
    //    name: '管理员',
    //    job: 'ng-Dev',
    //    picture: 'app/img/user/02.jpg'
    //};

    //没有登录
    $rootScope.loginOut = function () {
        if (!localStorage.getItem('infoearth_spacedata_userCode')) {
            var wwwPath = window.document.location.href;
            var pathname = window.document.location.pathname;
            var pos = wwwPath.indexOf(pathname);
            var localhostPath = wwwPath.substring(0, pos);
            if (language_English == 1) {
                window.location.href = localhostPath + "/loginEN.html";
            }
            else {
                window.location.href = localhostPath + "/login.html";
            }
        }
    }
}]);
// translate config 
app.config(['$translateProvider',
function ($translateProvider) {

    // prefix and suffix information  is required to specify a pattern
    // You can simply use the static-files loader with this pattern:
    $translateProvider.useStaticFilesLoader({
        prefix: 'App/Main/assets/i18n/',
        suffix: '.json'
    });

    // Since you've now registered more then one translation table, angular-translate has to know which one to use.
    // This is where preferredLanguage(langKey) comes in.
    $translateProvider.preferredLanguage('cn');

    // Store the language in the local storage
    $translateProvider.useLocalStorage('en');

}]);

// Angular-Loading-Bar
// configuration
app.config(['cfpLoadingBarProvider',
function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeBar = true;
    cfpLoadingBarProvider.includeSpinner = true;

}]);


//拦截器的注入
//app.config(['$httpProvider', function ($httpProvider) {
//    $httpProvider.interceptors.push('userInterceptor');
//}]);

//事件传播的拦截器
app.config(['$provide', function ($provide) {
    $provide.decorator('$rootScope', ['$delegate', function ($delegate) {
        Object.defineProperty($delegate.constructor.prototype, '$onRootScope', {
            value: function (name, listener) {
                var unsubscribe = $delegate.$on(name, listener);
                this.$on('$delegate', unsubscribe);
                return unsubscribe;
            },
            enumerable: false
        });
        return $delegate;
    }]);
}]);