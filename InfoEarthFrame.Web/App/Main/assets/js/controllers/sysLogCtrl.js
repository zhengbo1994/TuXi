'use strict';
/**
 * sysLogCtrl Controller
 */
app.controller('sysLogCtrl', ['$rootScope', '$scope', 'selfadapt', 'SweetAlert', '$element', '$http', 'abp.services.app.operateLog', '$filter', '$timeout',
function ($rootScope, $scope, selfadapt, SweetAlert, $element, $http, operateLog, $filter, $timeout) {
    $rootScope.loginOut();
    $rootScope.homepageStyle = {};

    //调用实时随窗口高度的变化而改变页面内容高度的服务
    var unlink = selfadapt.anyChange($element);
    $scope.$on('$destroy', function () {
        unlink();
        selfadapt.showBodyScroll();
    });

    // 获取请求的localhost
    var wwwPath = window.document.location.href;
    var pathname = window.document.location.pathname;
    var pos = wwwPath.indexOf(pathname);
    var localhostPath = wwwPath.substring(0, pos);

    $scope.isEnglish = language_English;

    //提示框
    function alertFun(title, text, type, color) {
        SweetAlert.swal({
            title: title,
            text: text,
            type: type,
            confirmButtonColor: color
        });
    }

    // 操作类型
    $scope.actionType = [
        { type: ' ', num: null },
        { type: $filter('translate')('views.System.SystemLog.New'), num: '1' },
        { type: $filter('translate')('views.System.SystemLog.Edit'), num: '2' },
        { type: $filter('translate')('views.System.SystemLog.Import'), num: '3' },
        { type: $filter('translate')('views.System.SystemLog.Emptied'), num: '4' },
        { type: $filter('translate')('views.System.SystemLog.Delete'), num: '5' },
        { type: $filter('translate')('views.System.SystemLog.Refresh'), num: '6' }
    ]

    // 系统功能
    $scope.systemFunc = [
        { func: ' ', num: null },
        { func: $filter('translate')('views.System.SystemLog.Layers'), num: '1' },
        { func: $filter('translate')('views.System.SystemLog.Maps'), num: '2' },
        { func: $filter('translate')('views.System.SystemLog.Style'), num: '3' }
    ]

    $scope.$on("LanguageChange", function () {
        // 操作类型
        $scope.actionType = [
            { type: ' ', num: null },
            { type: $filter('translate')('views.System.SystemLog.New'), num: '1' },
            { type: $filter('translate')('views.System.SystemLog.Edit'), num: '2' },
            { type: $filter('translate')('views.System.SystemLog.Import'), num: '3' },
            { type: $filter('translate')('views.System.SystemLog.Emptied'), num: '4' },
            { type: $filter('translate')('views.System.SystemLog.Delete'), num: '5' },
            { type: $filter('translate')('views.System.SystemLog.Refresh'), num: '6' }
        ]

        // 系统功能
        $scope.systemFunc = [
            { func: ' ', num: null },
            { func: $filter('translate')('views.System.SystemLog.Layers'), num: '1' },
            { func: $filter('translate')('views.System.SystemLog.Maps'), num: '2' },
            { func: $filter('translate')('views.System.SystemLog.Style'), num: '3' }
        ]
    })

    // 用户的区域id
    $scope.department = '';

    // 系统日志信息
    $scope.logData = [];

    // 搜索的用户名
    $scope.searchUserName = '';

    // 操作用户弹窗的modal
    $scope.inputModal = {
        modalTitle: '',
        pageing: {
            goPage: 1,
            pageCounts: 0,
            totalPages: 0,
            pageIndex: 1,
            pageSize: 2,
        },
        treeControl: {},
        searchName: '',
        resultName: '',
        chedckedData: [],
        departmentTreeData: [{ label: $filter('translate')('setting.all'), children: [], id: '' }],
        departmentSelected: function (selected) {
            console.log('selected', selected)
            if (selected.code) {
                $scope.department = selected.code;
            } else {
                $scope.department = '';
            }
            getSearchUser($scope.department, $scope.inputModal.searchName);
        },
        searchUserByName: function () {
            getSearchUser($scope.department, $scope.inputModal.searchName);
        },
        pageChanged: function (a, evt) {
            console.log(evt)
            if (evt && evt.keyCode !== 13) { return; }//注：回车键为13
            if (a) {
                a = parseInt(a);
                if (isNaN(a) || a < 1 || a > $scope.inputModal.pageing.totalPages) {
                    $scope.inputModal.pageing.goPage = $scope.inputModal.pageing.pageIndex;
                    return;
                }
                $scope.inputModal.pageing.goPage = a;
                $scope.inputModal.pageing.pageIndex = a;
            }
            getSearchUser($scope.department, $scope.inputModal.searchName);
        },
        onTdChecked: function () {
            console.log('chedckedData', $scope.inputModal.chedckedData)
        },
        tabBtnParams: [{
            name: "删除", click: function (row, name, event) {
                for (var i = 0; i < $scope.inputModal.chedckedData.length; i++) {
                    if ($scope.inputModal.chedckedData[i].id === row.id) {
                        $scope.inputModal.chedckedData.splice(i, 1);
                        break;
                    }
                }
                for (var j = 0; j < $scope.inputModal.resultName.length; j++) {
                    if ($scope.inputModal.resultName[j].id === row.id) {
                        $scope.inputModal.resultName[j].ischecked = false;
                        break;
                    }
                }
            }
        }]
    }

    // 模态框提交
    $scope.submitForm = function (modalInstance) {
        var arr = [];
        $scope.inputModal.chedckedData.forEach(function (item) {
            arr.push(item.userName);
        })
        $scope.searchUserName = arr.join(',');
        modalInstance.close();
    }

    // 模态框关闭
    $scope.cancel = function (modalInstance) {
        modalInstance.close();
        $scope.inputModal.searchName = '';
        $scope.inputModal.pageing = {
            goPage: 1,
            pageCounts: 0,
            totalPages: 0,
            pageIndex: 1,
            pageSize: 2,
        }
        $scope.inputModal.treeControl.select_branch({});
    }

    $scope.openedBack = function () {
        $scope.inputModal.modalTitle = $filter('translate')('views.System.SystemLog.User');

        $scope.inputModal.treeSelected = {};

        $scope.inputModal.departmentTreeData[0].label = $filter('translate')('setting.all');
    }

    // 分页
    $scope.maxSize = 5;//页码个数显示数
    $scope.goPage = 1;//转到多少页
    $scope.pageCounts = 0;//32;//总条数
    $scope.pageIndex = 1;//1;//起始页
    $scope.pageSize = 10;//10;//每页显示条数
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
        $scope.getSystemLog();
    };

    // 格式化时间
    Date.prototype.Format = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1, //月份 
            "d+": this.getDate(), //日 
            "h+": this.getHours(), //小时 
            "m+": this.getMinutes(), //分 
            "s+": this.getSeconds(), //秒 
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
            "S": this.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt))
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }

    // 获取n天前后的日期
    function fun_date(aa, type) {
        var date1 = new Date(),
        time1 = date1.getFullYear() + "-" + (date1.getMonth() + 1) + "-" + date1.getDate();//time1表示当前时间
        var date2 = new Date(date1);
        if (type === 'day') {
            date2.setDate(date1.getDate() + aa);
            var time2 = date2.getFullYear() + "-" + (date2.getMonth() + 1) + "-" + date2.getDate();
        } else if (type === 'month') {
            date2.setDate(date2.getMonth() + aa);
            var time2 = date2.getFullYear() + "-" + (date2.getMonth() + 1) + "-" + date2.getDate();
        }
        return time2
    }

    // 按钮设置时间
    $scope.setTime = function (str) {
        if (str === 'today') {
            $scope.betweenCalendar = [new Date(), new Date()];
        } else if (str === 'sevenDays') {
            var sevenDaysAgoTime = fun_date(-7, 'day');
            $scope.betweenCalendar = [new Date(sevenDaysAgoTime), new Date()];
        } else if (str === 'oneMonth') {
            var begin = new Date().setMonth((new Date().getMonth() - 1))
            var oneMonthAgoTime = new Date(begin)
            $scope.betweenCalendar = [new Date(oneMonthAgoTime), new Date()];
        } else if (str === 'threeMonth') {
            var begin = new Date().setMonth((new Date().getMonth() - 3))
            var threeMonthAgoTime = new Date(begin)
            $scope.betweenCalendar = [new Date(threeMonthAgoTime), new Date()];
        }
    }

    // 获取单位树信息
    var userCode = localStorage.getItem('infoearth_spacedata_userCode');
    $http({
        method: 'GET',
        url: localhostPath + '/api/services/app/area/GetAreaInfoByUserCode',
        params: { 'userCode': userCode }
    }).then(function successCallback(resp) {
        console.log()
        $scope.inputModal.departmentTreeData[0].children = resp.data;
        getDepartmentArr(resp.data);
    }, function errorCallback(err) {
    });

    function getSearchUser(id, name) {
        var param = {
            'AreaCode': id,
            'UserCode': userCode,
            'UserName': name
        }
        operateLog.getPageUserByAreaCode(param, $scope.inputModal.pageing.pageSize, $scope.inputModal.pageing.pageIndex).success(function (data, status) {
            console.log('data', data)
            $scope.inputModal.resultName = data.items;
            $scope.inputModal.pageing.pageCounts = data.totalCount;
            $scope.inputModal.pageing.totalPages = Math.ceil(data.totalCount / $scope.inputModal.pageing.pageSize);
            getDepartmentName($scope.selectedArr, data.items);
            initChecked($scope.inputModal.chedckedData, data.items);
        }).error(function (data, status) {
            console.log(data)
            alertFun($filter('translate')('views.System.alertFun.error'), data.message, 'error', '#007AFF');
        });
    }

    // 初始化选中项
    function initChecked(checked, person) {
        person.forEach(function (item) {
            checked.forEach(function (obj) {
                if (item.id === obj.id) {
                    item.ischecked = true;
                }
            })
        })
    }

    // 遍历找到对应的部门名称
    $scope.selectedArr = [];
    function getDepartmentArr(departmentArr) {
        if (Array.isArray(departmentArr)) {
            departmentArr.forEach(function (item) {
                $scope.selectedArr.push({ code: item.code, label: item.label });
                getDepartmentArr(item.children);
            })
        }
    }

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
    }

    // 获取系统日志
    $scope.getSystemLog = function () {
        // 操作用户      
        if ($scope.inputModal.chedckedData.length > 0) {
            $scope.QueryBy = [];
            $scope.inputModal.chedckedData.forEach(function (item) {
                $scope.QueryBy.push(item.id)
            })
            $scope.QueryBy = JSON.stringify($scope.QueryBy);
        } else {
            $scope.QueryBy = '';
        }

        $scope.StartTime = '';
        $scope.EndTime = '';
        if ($scope.betweenCalendar && $scope.betweenCalendar.length === 2) {
            $scope.StartTime = $scope.betweenCalendar[0].Format("yyyy-MM-dd") // 起始时间
            $scope.EndTime = $scope.betweenCalendar[1].Format("yyyy-MM-dd") // 结束时间
        }

        $scope.SystemFunc = $scope.systemFunc.selected ? $scope.systemFunc.selected.num : ''; //系统功能
        $scope.OperateType = $scope.actionType.selected ? $scope.actionType.selected.num : '';; // 操作类型

        var params = {
            'OperateId': userCode,
            'QueryId': $scope.QueryBy,
            'StartTime': $scope.StartTime,
            'EndTime': $scope.EndTime,
            'SystemFunc': $scope.SystemFunc,
            'OperateType': $scope.OperateType
        }

        operateLog.getPageListByCondition(params, $scope.pageSize, $scope.pageIndex).success(function (data, status) {
            console.log('data', data);
            data.items.forEach(function (item) {
                item.operateTime = new Date(item.operateTime).Format("yyyy-MM-dd hh:mm:ss");
            })
            $scope.logData = data.items;
            $scope.pageCounts = data.totalCount;
            $scope.totalPages = Math.ceil(data.totalCount / $scope.pageSize);
        }).error(function (data, status) {
            alertFun($filter('translate')('views.System.alertFun.error'), data.message, 'error', '#007AFF');
        });
    }
    $scope.getSystemLog();
}]);
