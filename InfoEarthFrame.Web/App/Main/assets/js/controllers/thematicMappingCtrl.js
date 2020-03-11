var app = angular.module('LoginApp', []);

app.controller('thematicMappingCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {

    //$scope.inputFocus = function () {
    //    console.log(111111);
    //}, $scope.inputBlur = function (a) {
    //    if (a === 1 && !$scope.usercode) {
    //        alert("请填写用户名");
    //        $('#code').focus();
    //        return;
    //    }
    //    if (a === 2 && !$scope.password) {
    //        alert("请填写密码");
    //        $('#psd').focus();
    //        return;
    //    }
    //},
    setTimeout(function () {
       angular.element('#code').focus();
    }, 1);
    $scope.keypress = function (e) {
        if (e.keyCode === 13) {
            if (!$scope.usercode) {
                $('#code').focus();
                return;
            }
            if (!$scope.password) {
                $('#psd').focus();
                return;
            }
            $scope.loginIn();
        }
    };




    var wwwPath = window.document.location.href;
    var pathname = window.document.location.pathname;
    var pos = wwwPath.indexOf(pathname);
    var localhostPath = wwwPath.substring(0, pos);
    
    $scope.usercode = 'admin';
    $scope.password = 'infoearth';
    $scope.isRemember = '';
    $scope.isRewrite = 0;

    $scope.rewritePsd = function () {
        $scope.isRewrite = 1;
    }

    //登录的方法
    $scope.loginIn = function () {
        if (!$scope.usercode) {
            alert("请填写用户名");
            return;
        }
        if (!$scope.password) {
            alert("请填写密码");
            return;
        }
        //密码为输入值或cookie中取出来的值
        var psd = $scope.password;
        //如果用户不是管理员或者修改过密码，需要对密码加密后发出请求
        if ($scope.isRewrite > 0) {
            if ($scope.usercode !== "admin") {
                psd = hex_md5(encodeURI($scope.password));
            }
        }

        $http({
            method: 'POST',
            url: '/api/services/app/systemUser/GetDetailByNamePassword',
            data: { UserCode: $scope.usercode, Password: psd }
        }).success(function (val, status) {
            console.log(val);
            
            if (!!val.result.id) {
                if ($scope.isRemember) {
                    setCookie('infoearth_spacedata_remUserCode', encodeURI(val.result.userCode));
                    setCookie('infoearth_spacedata_password', encodeURI(val.result.password));
                    setCookie('infoearth_spacedata_remUserStatus', 1);
                }
                else {
                    setCookie('infoearth_spacedata_remUserCode', '');
                    setCookie('infoearth_spacedata_password', '');
                    setCookie('infoearth_spacedata_remUserStatus', 0);
                }
                setCookie('infoearth_spacedata_userCode', encodeURI(val.result.userCode));
                //console.log(getCookie('password'));
                window.location.href = localhostPath + "/#/app/home";
            }
            else {
                alert("用户名或密码错误，请重新输入!");
                $('#code').focus();
            }
        }).error(function (data, status) {
            console.log(data);
        });
    }

    //设置cookie
    function setCookie(c_name, value) {
        localStorage.setItem(c_name, value);
    }
    //读取cookies
    function getCookie(name) {
        return localStorage.getItem(name);
    }

    //自适应高度  
    function setHeight() {
        var winH = angular.element(window).height();
        var winW = angular.element(window).width();
        var loginH = angular.element("#loginWin").height();
        var loginW = angular.element("#loginWin").width();

        angular.element("#loginWin").css('top', (winH - loginH) / 2 - 15 + 'px');
        angular.element("#loginWin").css('left', (winW - loginW) / 2 - 15 + 'px');
    }
    setHeight();
    angular.element(window)[0].onresize = function () {
        setHeight();
    }

    $(document).ready(function () {
        if (!!parseInt(getCookie('infoearth_spacedata_remUserStatus'))) {
            //window.location.href = localhostPath + "/#/app/home";
            $scope.usercode = angular.copy(decodeURI(getCookie('infoearth_spacedata_remUserCode')));
            $scope.password = angular.copy(decodeURI(getCookie('infoearth_spacedata_password')));
            $scope.isRemember = true;
            $('#code').val($scope.usercode);
            $('#psd').val($scope.password);
            $('#check').attr('checked', true);
        }
    });
}]);