'use strict';
/** 
  * 日历控件
  * by licx
*/

app.directive('micCalendars', function () {
    return {
        restrict: 'E',
        template: function (element, attr) {
            var el = '   <input type="text" style=" width:100%; height:34px;" ng-model="dateSelected" mobiscroll-calendar="option" mobiscroll-instance="democalendar" placeholder="Select a date..." />';
            if (angular.isDefined(attr.section)) {
                el = '   <input type="text" style=" width:100%; height:34px;" ng-model="dateSelected" mobiscroll-range="option" mobiscroll-instance="democalendarss" placeholder="Select a date..." />';
            }
            var tmp =
                '<div class="calendar-div">' + el +
                '   <span class="spanbtn">' +
                '       <i class="fa fa-calendar "></i>' +
                '   </span>' +
                '</div>';
            return tmp;
        },
        scope: {
            dateSelected: '=',
            dateFormat: '@?',
            min: '@?',
            max: '@?',
            onHide:'=?'
        },
        replace: true,
        controller: ['$scope', function ($scope) {
            $scope.option = {
                theme: 'bootstrap',//bootstrap material mobiscroll  android-holo
                lang: 'en',
                display: 'left',//显示方式  center  bubble
                controls: ['calendar'],//配置日期和时间
                showScrollArrows: true,
                showLabel: true,
                etOnTap: true,

                buttons: ["set", { text: "Today", handler: "now" }, "cancel"],

                //invalid: ['w0', 'w6', '5/1', '3/10', '12/24', '12/25'],//过滤日期
                dateFormat: $scope.dateFormat ? $scope.dateFormat : 'yyyy-mm-dd',
                timeFormat: 'HH:ii:ss',
                dateOrder: 'ymmdd',
                timeWheels: 'HHiiss',
                min: $scope.min ? $scope.min : new Date('1970'),
                max: $scope.max ? $scope.max : new Date('2525'),
                onCancel: function () {
                    //暂时没用到
                },
                onHide: function (a, b) {
                    $scope.onHide && angular.isFunction($scope.onHide) && $scope.onHide(b.getVal());
                },
                onShow: function (a, b) {
                    //console.log(a, b);
                    //暂时没用到
                },
            };
            $scope.democalendar = 'calendarTime';//可配置calendarTime calendarBasic   
            $scope.democalendarss = 'rangeBasic';




            // 手机版日期控件（滚轮型）
            //$scope.mydatetime = new Date('2016-10-01');
            //$scope.settings = {
            //    theme: 'bootstrap',//可配置  bootstrap  material
            //    lang: 'zh',
            //    display: 'center',//显示方式  center  bubble modal
            //    min: new Date('1900'),
            //    max: new Date('2222'),
            //    buttons: ["set", { text: "今天", handler: "now" }, "cancel"],
            //    //invalid: ['w0', 'w6', '5/1', '3/10', '12/24', '12/25'],//过滤日期
            //    dateFormat: 'yyyy-mm-dd',
            //    timeFormat: 'HH:ii:ss',
            //    dateOrder: 'ymmdd',
            //    timeWheels: 'HHiiss'
            //};
            //$scope.demo = 'calendarBasic';





            //$scope.mycalendarss = [new Date('2016-10-08'), new Date('2016-10-01')];





            
        }]
    }
});