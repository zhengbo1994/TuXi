'use strict';
/**
 * setSysCtrl Controller
 */
app.controller('userManagerCtrl', ['$rootScope', '$scope', 'SweetAlert', '$element', 'selfadapt', '$timeout', 'abp.services.app.systemUser', '$cookieStore', '$localStorage', '$http', '$filter',
function ($rootScope, $scope, SweetAlert, $element, selfadapt, $timeout, systemUser, $cookieStore, $localStorage, $http, $filter) {
    $rootScope.loginOut();
    $rootScope.homepageStyle = {};
    //调用实时随窗口高度的变化而改变页面内容高度的服务
    var unlink = selfadapt.anyChange($element);
    $scope.$on('$destroy', function () {
        unlink();
        selfadapt.showBodyScroll();
    });
    //当前用户usercode
    var userId = localStorage.getItem('infoearth_spacedata_userCode');
    //搜索框的model
    $scope.searchInput = "";
    //用户信息
    $scope.personData = [];
    //搜索用对象
    $scope.searchData = {
        Id: '',
        UserName: '',
        UserCode: userId,
        UserSex: '',
        Password: '',
        TelPhone: '',
        Phone: '',
        Department: '',
        Position: ''
    }

    // 获取请求localhost
    var wwwPath = window.document.location.href;
    var pathname = window.document.location.pathname;
    var pos = wwwPath.indexOf(pathname);
    var localhostPath = wwwPath.substring(0, pos);
   
    //搜索用户
    $scope.searchUser = function () {
        $scope.searchData.UserName = $scope.searchInput;
        getDepartmentPerson($scope.searchData, $scope.pageSize, $scope.pageIndex);
    }

    //重置密码
    $scope.reset = function (tr) {
        $scope.resetPwModal.id = tr.id;
        $scope.resetPwModal.userName = tr.userName;
        $scope.resetPwModal.userCode = tr.userCode;
        $scope.resetPwModal.sex = tr.userSex;
        $scope.resetPwModal.password = tr.password;
        $scope.resetPwModal.telPhone = tr.telPhone;
        $scope.resetPwModal.phone = tr.phone;
        $scope.resetPwModal.department = tr.department;
        $scope.resetPwModal.position = tr.position;
        $scope.resetPwModal.remark = tr.remark;

        console.log($scope.resetPwModal);
        $scope.openResetPwFun();
    }

    //编辑
    $scope.edit = function (tr) {
        console.log('tr', tr)
        $scope.inputModal.departmentTreeData = angular.copy($scope.departmentTreeData);
        $scope.inputModal.addUserTitle = $filter('translate')('setting.edit');
        $scope.inputModal.isEdit = true;
        $scope.inputModal.id = tr.id;
        $scope.inputModal.userName = tr.userName;
        $scope.inputModal.userCode = tr.userCode;
        $scope.inputModal.sex = tr.userSex;
        $scope.inputModal.password = tr.password;
        $scope.inputModal.telPhone = tr.telPhone;
        $scope.inputModal.phone = tr.phone;
        $scope.inputModal.department = tr.department;
        if (tr.label) $scope.inputModal.label = tr.label;       
        $scope.inputModal.position = tr.position;
        $scope.inputModal.remark = tr.remark;
        $scope.openAddUserFun();
    }


    //删除
    $scope.del = function (id) {
        SweetAlert.swal({
            title: $filter('translate')('setting.delete'),
            text: $filter('translate')('views.System.alertFun.deleteUser'),
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: $filter('translate')('setting.sure'),
            cancelButtonText: $filter('translate')('setting.cancel')
        }, function (isConfirm) {
            if (isConfirm) {
                systemUser.delete(id).success(function (data, status) {
                    //console.log(data, status);
                    alertFun($filter('translate')('views.System.alertFun.DeleteSuccessfully'), '', 'success', '#007AFF');
                    getDepartmentPerson($scope.searchData, $scope.pageSize, $scope.pageIndex);
                }).error(function (data, status) {
                    console.log(data, status);
                    alertFun($filter('translate')('views.System.alertFun.error'), data.message, 'error', '#007AFF');
                });
            }
        });
    }

    // 接收切换语言的事件
    $scope.$on("LanguageChange", function () {
        $scope.departmentTreeData[0].label = $filter('translate')('setting.all');
    })

    //新增修改弹窗的model
    $scope.inputModal = {
        id: '',
        addUserTitle: '',
        userName: '',
        userCode: '',
        password: '',
        telPhone: '',
        phone: '',
        department: '',
        position: '',
        sex: '',
        remark: '',
        label: '',
        selected: {},
        departmentTreeData: '',
        isEdit: false,
        modalDepartmentSelected: function (selected) {
            $scope.inputModal.department = selected.code;
        }
    }
    var copyMod = angular.copy($scope.inputModal);
    //打开新增用户弹窗
    $scope.addUserOpenWin = function () {
        $scope.inputModal.addUserTitle = $filter('translate')('views.System.UserManagement.property.newusers');
        $scope.inputModal.isEdit = false;
        $scope.openAddUserFun();
    };
    //模态窗口打开时的回调函数
    $scope.openedBack = function () {
        $scope.inputModal.departmentTreeData = angular.copy($scope.departmentTreeData);
        $scope.inputModal.departmentTreeData[0].label = $filter('translate')('setting.all');
    };
    //提交数据操作
    $scope.submitForm = function (modalInstance, form) {
        if (!$scope.inputModal.userName) {
            alertFun($filter('translate')('views.System.alertFun.FillName'), "", 'warning', '#007AFF');
            return;
        }
        if (!$scope.inputModal.userCode) {
            alertFun($filter('translate')('views.System.alertFun.FillAccount'), "", 'warning', '#007AFF');
            return;
        }
        if (!$scope.inputModal.password) {
            alertFun($filter('translate')('views.System.alertFun.FillPassword'), "", 'warning', '#007AFF');
            return;
        }
        if ($scope.inputModal.userCode.indexOf("admin") > -1) {
            alertFun($filter('translate')('views.System.alertFun.NotContainAdmin'), "", 'warning', '#007AFF');
            return;
        }
        if (!$scope.inputModal.department) {
            alertFun($filter('translate')('views.System.alertFun.FillOrganization'), "", 'warning', '#007AFF');
            return;
        }
        var dto = {
            Id: $scope.inputModal.id,
            UserName: $scope.inputModal.userName,
            UserCode: $scope.inputModal.userCode,
            UserSex: $scope.inputModal.sex,
            Password: $scope.inputModal.password,
            TelPhone: $scope.inputModal.telPhone,
            Phone: $scope.inputModal.phone,
            Department: $scope.inputModal.department,
            Position: $scope.inputModal.position,
            Remark: $scope.inputModal.remark
        }
        //console.log(dto);

        if (!!dto.Id) {
            //编辑
            systemUser.update(dto).success(function (data, status) {
                //console.log(data, status);
                alertFun($filter('translate')('views.System.alertFun.EditSuccessful'), '', 'success', '#007AFF');
                modalInstance.close();
                getDepartmentPerson($scope.searchData, $scope.pageSize, $scope.pageIndex);
            }).error(function (data, status) {
                console.log(data, status);
                alertFun($filter('translate')('views.System.alertFun.error'), data.message, 'error', '#007AFF');
            });
        }
        else {
            //新增
            systemUser.getAllListByName({ UserName: $scope.inputModal.userCode }, $scope.pageSize, $scope.pageIndex).success(function (data, status) {
                //console.log(data, status);
                if (data.totalCount > 0) {
                    alertFun($filter('translate')('views.System.alertFun.Exists'), "", 'warning', '#007AFF');
                    return;
                }
                //dto.Id = '1';
                systemUser.insert(dto).success(function (data, status) {
                    //console.log(data, status);
                    // 每次新增用户前 要清空名字搜索内容
                    $scope.searchInput = '';
                    $scope.searchData.UserName = '';
                    alertFun($filter('translate')('views.System.alertFun.AddSuccessful'), '', 'success', '#007AFF');
                    modalInstance.close();
                    getDepartmentPerson($scope.searchData, $scope.pageSize, $scope.pageIndex);
                }).error(function (data, status) {
                    console.log(data, status);
                    alertFun($filter('translate')('views.System.alertFun.error'), data.message, 'error', '#007AFF');
                });
            });
        }
    }
    //关闭或取消操作
    $scope.cancel = function () {
        $scope.inputModal = angular.copy(copyMod);
    };

    //重置密码弹窗
    $scope.resetPwModal = {
        id: '',
        userCode: '',
        newPw: '',
        reNewPw: '',
        title: '',
        label1: '登录账号',
        label2: '新密码',
        label3: '确认密码'
    };
    var copyResetPwMod = angular.copy($scope.resetPwModal);

    $scope.openResetPw = function () {
        $scope.resetPwModal.title = $filter('translate')('views.System.UserManagement.tableOrModal.resetPassword');
    };
    //提交数据操作
    $scope.submitResetPw = function (modalInstance, form) {
        if (!$scope.resetPwModal.newPw) {
            alertFun($filter('translate')('views.System.alertFun.FillNewPassword'), "", 'warning', '#007AFF');
            return;
        }
        if (!$scope.resetPwModal.reNewPw) {
            alertFun($filter('translate')('views.System.alertFun.ConfirmPassword'), "", 'warning', '#007AFF');
            return;
        }
        if ($scope.resetPwModal.newPw !== $scope.resetPwModal.reNewPw) {
            alertFun($filter('translate')('views.System.alertFun.NotMatch'), "", 'warning', '#007AFF');
            return;
        }
        var dto = {
            Id: $scope.resetPwModal.id,
            UserCode: $scope.resetPwModal.userCode,
            Password: $scope.resetPwModal.newPw,
            UserName: $scope.resetPwModal.userName,
            UserSex: $scope.resetPwModal.sex,
            TelPhone: $scope.resetPwModal.telPhone,
            Phone: $scope.resetPwModal.phone,
            Department: $scope.resetPwModal.department,
            Position: $scope.resetPwModal.position,
            Remark: $scope.resetPwModal.remark
        }
        systemUser.update(dto).success(function (data, status) {
            //console.log(data, status);
            alertFun($filter('translate')('views.System.alertFun.ResetSuccessful'), '', 'success', '#007AFF');
            modalInstance.close();
            getDepartmentPerson($scope.searchData, $scope.pageSize, $scope.pageIndex);
        }).error(function (data, status) {
            console.log(data, status);
            alertFun($filter('translate')('views.System.alertFun.error'), data.message, 'error', '#007AFF');
        });
    }
    $scope.cancelResetPw = function () {
        $scope.resetPwModal = angular.copy(copyResetPwMod);
    };

    //分页
    $scope.maxSize = 5;//页码个数显示数
    $scope.goPage = 1;//转到多少页
    $scope.pageCounts = 0;//32;//总条数
    $scope.pageIndex = 1;//1;//起始页
    $scope.pageSize = 10;//10;//每页显示条数
    $scope.code = ''; // 判断是否点击查询树
    //分页的事件方法
    $scope.pageChanged = function (a, evt) {
        if (evt && evt.keyCode !== 13) { return; }//注：回车键为13
        if (a) {
            a = parseInt(a);
            if (isNaN(a) || a < 1 || a > $scope.totalPages) {
                $scope.goPage = $scope.pageIndex;
                return;
            }
            $scope.goPage = a;
            $scope.pageIndex = a;
        }
        getDepartmentPerson($scope.searchData, $scope.pageSize, $scope.pageIndex);
    };

    //提示框
    function alertFun(title, text, type, color) {
        SweetAlert.swal({
            title: title,
            text: text,
            type: type,
            confirmButtonColor: color
        });
    }

    // 获取单位树信息
    $scope.selectedArr = [];
    $scope.departmentTreeData = [{ label: $filter('translate')('setting.all'), children: [], id: '' }];
    $http({
        method: 'GET',
        url: localhostPath + '/api/services/app/area/GetAreaInfoByUserCode',
        params: { 'userCode': userId }
    }).then(function successCallback(resp) {
        console.log('resp', resp)
        getDepartmentArr(resp.data);
        $scope.departmentTreeData[0].children = resp.data;
        $scope.inputModal.departmentTreeData = angular.copy($scope.departmentTreeData);
        getDepartmentPerson($scope.searchData, $scope.pageSize, $scope.pageIndex);
    }, function errorCallback(err) {
        alertFun($filter('translate')('views.System.alertFun.error') + err, 'error', '#007AFF');
    });

    // 获取地方单位人员信息
    function getDepartmentPerson(searchData, size, index) {
        systemUser.getAllPageListByCondition(searchData, size, index).success(function (data, status) {
            $scope.personData = data.items;
            $scope.pageCounts = data.totalCount;
            $scope.totalPages = Math.ceil(data.totalCount / $scope.pageSize);
            getDepartmentName($scope.selectedArr, $scope.personData);
        });
    }

    $scope.departmentSelected = function (selected) {
        // 每次点击地方单位要清空名字搜索内容
        $scope.searchInput = '';
        $scope.searchData.UserName = '';
        if (selected.code) {
            $scope.searchData.Department = selected.code;            
        } else {
            $scope.searchData.Department = '';
        }
        getDepartmentPerson($scope.searchData, $scope.pageSize, $scope.pageIndex);
    }

    // 遍历找到对应的部门名称
    function getDepartmentName(department, person) {
        if (person.length > 0) {
            person.forEach(function (item) {
                department.forEach(function (obj) {
                    if (obj.code === item.department) {
                        item.label = obj.label;
                    }
                })
            })
        }
        console.log('personData', $scope.personData)
    }

    function getDepartmentArr(departmentArr) {
        if (Array.isArray(departmentArr)) {
            departmentArr.forEach(function (item) {
                $scope.selectedArr.push({ code: item.code, label: item.label });
                getDepartmentArr(item.children);
            })
        }
    }
}]);
