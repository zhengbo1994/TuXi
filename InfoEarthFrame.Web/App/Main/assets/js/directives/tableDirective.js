'use strict';
/** 
  * 网格控件
  * by LICX
*/
app.directive('micDataTables', function (MicTablesServer, $timeout) {
    return {
        restrict: 'E',
        template: function (el, attrs) {
            function retPage(tpl) {
                

                if (!angular.isDefined(attrs.pageing)) { return tpl; }
                return '<div>' + tpl + '<div style="height:34px;margin-top:5px;">' +
                    '    <pagination style="float:left" first-text="{{\'paging.home\'|translate}}" previous-text="{{\'paging.previous\'|translate}}" next-text="{{\'paging.next\'|translate}}" last-text="{{\'paging.last\'|translate}}" items-per-page="pageSize"' +
                    '                total-items="pageing.pageCounts" ng-model="pageIndex" max-size="maxSize" class="pagination-sm"' +
                    '                boundary-links="true" boundary-go="true" rotate="false" num-pages="totalPages" ng-change="pageChanged()"></pagination>' +
                    '    <ul class="pagination" style="float: left;margin: 4px 0 0 !important;">' +
                    '    <li><input type="text" style="margin-left: 10px;float: left;width:40px;border-radius: 4px !important;" ng-model="goPage" ng-keydown="pageChanged(goPage,$event)" /></li>' +
                    '    <li><a style="padding: 4px 10px;" ng-click="pageChanged(goPage)">GO</a></li>' +
                    '    <li>' +
                    '        <label style="float: left;border:none;margin-left: 5px;padding: 4px 10px;position: relative;line-height: 1.52857143;text-decoration: none;margin-bottom: 0px;">' +
                    '            <span translate="paging.word1">第</span>{{pageIndex}}<span translate="paging.word2">页</span>/' +
                    '            <span translate="paging.word3">共</span>{{totalPages}}<span translate="paging.word2">页</span>，' +
                    '            <span translate="paging.word4">总计</span>{{pageing.pageCounts}}<span translate="paging.word5">条记录</span>' +
                    '        </label>' +
                    '    </li></ul>' +
                    '</div></div>';
            }

            var tmp, c1 = '', cstr = '', c2 = ' table-striped property-table', tr,
            tbodyHeight = attrs.tbodyHeight ? attrs.tbodyHeight + 'px;' : 'auto;';
            if (angular.isDefined(attrs.tableBordered)) {
                c1 = ' ibordered';
                c2 = ' table-bordered  table-hover';
                cstr = 'ng-class="{\'table-selected\':row.selected}"';
            }
            c2 = angular.isDefined(attrs.tableClass) ? attrs.tableClass : c2;

            /***********************************返回动态表格---dynamic---**************************************************/
            if (angular.isDefined(attrs.dynamic)) {

                tmp = '<div class="mic-tables ' + c1 + '" >' +
                    '<div class="mic-tables-thdiv"><table class="th-tables table ' + c2 + '">' +
                    '   <thead>' +
                    '       <tr>' +
                    '           <th ng-repeat="item in datasets.th track by $index"><div style="text-align:left;margin:0;white-space:nowrap;word-wrap:normal;overflow:hidden;display:block;text-overflow:ellipsis;">{{item}}</div></th>' +
                    '       </tr>' +
                    '   </thead>' +
                    '</table></div>' +
                    '<div class="mic-tables-div" style="overflow-y: auto;overflow-x:hidden;height:' + tbodyHeight + '">' +
                    '   <table class="table ' + c2 + '">' +
                    '       <tbody>' +
                    '           <tr ' + cstr + ' ng-click="tableSelected(row,$event)" ng-repeat="row in datasets.td">' +
                    '                <td ng-repeat="col in row"><div style="text-align:left;margin:0;white-space:nowrap;word-wrap:normal;overflow:hidden;display:block;text-overflow:ellipsis;">' +
                    '                   <a ng-if="col.isclick" ng-click="col.click(col.name)">{{col.name}}</a>' +
                    '                   <span ng-if="!col.isclick">{{col.name}}</span>' +
                    '                </div></td>' +
                    '           </tr>' +
                    '       </tbody>' +
                    '   </table>' +
                    '</div>' +
                    '</div>';
                return tmp = retPage(tmp);;
            }
            /*************************************************************************************************/


            var micTabServ = new MicTablesServer({ attrs: attrs, cellFunName: 'cellClickFun', row: 'row', btnFunName: 'table_btn_click' });
            var backCell = micTabServ._createTableCell();
            var thead = backCell.thead;
            tmp = backCell.td;

            if (angular.isDefined(attrs.tableBtnHtml)) {
                var backBtn = micTabServ._createTableBtn();
                thead += backBtn.thead;
                tmp += backBtn.td;
            }

            var checkbox = '', checkbox1 = '';

            if (angular.isDefined(attrs.checkable)) {
                var idall = 'chall_' + new Date().getTime();
                checkbox = '<th style="width:37px;padding: 0px 8px;">' +
                      '<div style="text-align:left;margin:0;white-space:nowrap;word-wrap:normal;overflow:hidden;display:block;text-overflow:ellipsis;width:22px;"><div style="margin-top: 4px;" class="checkbox clip-check check-primary">' +
                      '	<input type="checkbox" id="' + idall + '" ng-checked="allischecked" ng-click="tableCheckClick()">' +
                      '        <label style="padding-left: 22px !important;margin-right: 0px !important;" for="' + idall + '"></label>' +
                      '</div></div>' +
                      '</th>';
                checkbox1 = '<td style="width:37px;padding: 0px 8px;">' +
                       '<div style="text-align:left;margin:0;white-space:nowrap;word-wrap:normal;overflow:hidden;display:block;text-overflow:ellipsis;width:22px"><div style="margin-top: 4px;" class="checkbox clip-check check-primary">' +
                       '	<input type="checkbox" id="ch_{{row.$$hashKey}}" ng-checked="row.ischecked" ng-click="tableCheckClick(row)">' +
                       '        <label style="padding-left: 22px !important;margin-right: 0px !important;" for="ch_{{row.$$hashKey}}"></label>' +
                       '</div></div>' +
                       '</td>';
            }
            //选中行标注当前的选中状态(蓝底白字的样式)
            if (angular.isDefined(attrs.chooseState)) {
                tr = '<tr ' + cstr + ' ng-click="tableSelected(row,$event,$index)" ng-class="{currentIndex:i===currentIndex}"   style="cursor:pointer" ng-repeat="(i,row) in datasets">' +
                                    checkbox1 + tmp +
                                    '</tr>';
            } else {
                tr = '<tr ' + cstr + ' ng-click="tableSelected(row,$event)"    style="cursor:pointer" ng-repeat="row in datasets">' +
                                   checkbox1 + tmp +
                                   '</tr>';
            }
            tmp = '<div class="mic-tables ' + c1 + '">' +
                    '<div class="mic-tables-thdiv"><table class="th-tables table ' + c2 + '">' +
                    '<thead>' +
                    '<tr>' + checkbox + '<th><div style="text-align:left;margin:0;white-space:nowrap;word-wrap:normal;overflow:hidden;display:block;text-overflow:ellipsis;">' + thead + '</div></th></tr>' +
                    '</thead>' +
                    '</table></div>' +
                    '<div class="mic-tables-div" style="overflow-y:auto;overflow-x:hidden;height:' + tbodyHeight + '">' +
                    '<table class="table ' + c2 + '">' +
                    '<tbody>' + tr +
                    '</tbody>' +
                    '</table>' +
                    '</div>' +
                    '</div>';
            tmp = retPage(tmp);
            return tmp;
        },
        scope: {
            datasets: '=?',
            checkedData: '=?',
            tableBtnParams: '=?',
            pageing: '=?',
            tbodyHeight: '=?',
            allischecked: '=?',
            onDetailClick: '=?',
            onCellClick: '=?',
            onPageChange: '=?',
            //一行选中后的事件
            onTdChecked: '=?',
            //全选选中后的事件
            onThChecked: '=?'
        },
        controller: ['$scope', '$element', function ($scope, $element) {
            //分页
            if (angular.isDefined($scope.pageing)) {
                $scope.maxSize = angular.isDefined($scope.pageing.maxSize) ? $scope.pageing.maxSize : 5;//页码个数显示数
                $scope.goPage = 1;//转到多少页
                $scope.pageCounts = $scope.pageing.pageCounts;//总条数
                $scope.pageIndex = $scope.pageing.pageIndex ? $scope.pageing.pageIndex : 1;//起始页
                $scope.pageSize = $scope.pageing.pageSize ? $scope.pageing.pageSize : 10;//每页显示条数mic-tables-div
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
                    $scope.pageing.pageIndex = $scope.pageIndex;
                    if (angular.isDefined($scope.allischecked)) {
                        $scope.allischecked = false;
                    }
                    $scope.onPageChange();
                };
            }
            $scope.$watch('datasets', function (d) {
                for (var k in $scope.datasets) {
                    if (!$scope.datasets[k].tableBtnParams)
                        $scope.datasets[k].tableBtnParams = angular.copy($scope.tableBtnParams);
                }
            }, true);

            $scope.$watch('pageing', function (d) {
                //console.log(d);
                if (typeof (d) !== "undefined") {
                    $scope.pageIndex = d.pageIndex;
                }
            }, true);

            //动态设置表格宽高度
            function setth() {
                $timeout(function () {
                    var th_arr = $element.find('table.th-tables>thead>tr>th');
                    var w = $element.find('div.mic-tables-div>table').width();

                    $element.find('div.mic-tables-div>table')[0].style.width = '100%';
                    $element.find('table.th-tables')[0].style.width = '100%';
                    //$element.find('div.mic-tables-div>table')[0].style.borderCollapse = 'collapse';
                    //$element.find('table.th-tables')[0].style.borderCollapse = 'collapse';
                    $element.find('div.mic-tables-div>table')[0].style.tableLayout = 'fixed';
                    $element.find('table.th-tables')[0].style.tableLayout = 'fixed';

                    if ($element.find('div.mic-tables')[0]) {
                        $element.find('div.mic-tables')[0].style.overflowY = 'hidden';
                        $element.find('div.mic-tables')[0].style.overflowX = 'hidden';
                    } else {
                        $element[0].style.overflowY = 'hidden';
                        $element[0].style.overflowX = 'hidden';
                    }


                    $element.find('.mic-tables-thdiv')[0].style.width = 'auto';
                    $element.find('.mic-tables-div')[0].style.width = 'auto';

                    var h = 0;
                    var e1 = $element.find('table:last');
                    var e2 = $element.find('div.mic-tables-div');
                    if (e1.height() > e2.height()) {
                        h = !$scope.tbodyHeight ? 0 : 17;
                    }
                    $element.find('.mic-tables-thdiv')[0].style.paddingRight = h + 'px';

                    var td_arr = $element.find('div.mic-tables-div>table>tbody>tr:first>td');
                    if (td_arr.length === 0) { return; }
                    for (var i = 0; i < th_arr.length; i++) {
                        if (td_arr[i].style.width) {
                            angular.element(th_arr[i]).css('width', td_arr[i].style.width);
                        } else {
                            var setw = td_arr[i].offsetWidth / w * 100;
                            angular.element(th_arr[i]).css('width', setw + '%');
                            angular.element(td_arr[i]).css('width', setw + '%');
                        }
                    }
                }, 26);
            }

            function setth11() {
                $timeout(function () {
                    var th_arr = $element.find('table.th-tables>thead>tr>th');
                    var w = $element.find('div.mic-tables-div>table').width();

                    $element.find('div.mic-tables-div>table')[0].style.width = '100%';
                    $element.find('table.th-tables')[0].style.width = '100%';

                    var td_arr = $element.find('div.mic-tables-div>table>tbody>tr:first>td');
                    if (td_arr.length === 0) { return };
                    for (var i = 0; i < th_arr.length; i++) {
                        if (td_arr[i].style.width) {
                            angular.element(th_arr[i]).css('width', td_arr[i].style.width);
                        } else {
                            var el1 = angular.element(th_arr[i]),
                                el2 = angular.element(td_arr[i]),
                                w1 = el1.width(),
                                w2 = el2.width(),

                                w3 = th_arr[i].innerText.length === 1 ? 26 : (th_arr[i].innerText.length === 2 ? th_arr[i].innerText.length * 22 : th_arr[i].innerText.length * 19),
                                w4 = td_arr[i].innerText.length === 1 ? 26 : (td_arr[i].innerText.length === 2 ? td_arr[i].innerText.length * 22 : td_arr[i].innerText.length * 19),

                                wid;

                            wid = w1 > w2 ? w1 : w2;
                            if (w3 > w4) {
                                wid = wid > w3 ? wid : w3;
                            } else {
                                wid = wid > w4 ? wid : w4;
                            }
                            el1.css('width', wid + 'px');
                            el2.css('width', wid + 'px');
                        }
                    }
                    $element.find('div.mic-tables-div>table')[0].style.tableLayout = 'fixed';
                    $element.find('table.th-tables')[0].style.tableLayout = 'fixed';

                    var w = $element.find('table:first').width();

                    $element.find('.mic-tables-thdiv')[0].style.width = w + 'px';

                    var e1 = $element.find('table:last');
                    var e2 = $element.find('div.mic-tables-div');
                    if (e1.height() > e2.height()) {
                        w += 17;
                    }
                    $element.find('.mic-tables-div')[0].style.width = w + 'px';
                }, 26);
            }



            function setth1() {//灾害点统计分析主页宽表格
                $timeout(function () {
                    var scrollEl = $element.find('.mic-tables-div')[0];
                    var tempArr = [];
                    var tbs = $element.find('table');
                    tbs.each(function (i, el) {
                        var node = angular.element(el).children().children().first().children();
                        for (var j = 0 ; j < node.length; j++) {
                            var item = node.get(j).childNodes[0];
                            var l = item.innerText.length * 13;
                            if (i === 1) {
                                var ls = angular.element(tempArr[j]).width();
                                if (l < ls) {
                                    l = ls;
                                } else {
                                    tempArr[j].style.width = l + 'px';
                                }
                                item.style.width = l + 'px';
                                continue;
                            }
                            tempArr.push(item);
                            item.style.width = l + 'px';
                        }
                        if (i === 1) {
                            var ft = $element.find('table:first');
                            if (ft.width() > $element.width()) {
                                var scrollWidth = scrollEl.offsetWidth - scrollEl.scrollWidth;
                                if (scrollWidth !== 0) { scrollWidth = 17 }
                                scrollEl.style.width = (ft.width() + scrollWidth) + '.33px';
                            }
                        }
                    });
                }, 26);
            }

            function setth2() {// 暂停使用
                $timeout(function () {
                    $element.find('div.mic-tables-div').css('height', $scope.tbodyHeight);
                    $element.find('div.mic-tables-div')[0].style.width = 'auto';

                    var first = $element.find('table:first>thead>tr>th');
                    var last = $element.find('table:last>tbody>tr:first>td');
                    if (last.length === 0) return;
                    if ($element.find('table:first').width() <= $element.width()) {
                        var wt = $element.find('table:first').width();
                        first.each(function (i, el1) {
                            var el11 = angular.element(el1);
                            var el22 = angular.element(last[i]);
                            var w11 = el11.width();
                            var w22 = el22.width();
                            if (w11 >= w22) {
                                el22[0].childNodes[0].style.width = w11 + 'px';
                            } else {
                                el11[0].childNodes[0].style.width = w22 + 'px';
                            }
                            var el2 = angular.element(last[i])[0];
                            var f = el2.offsetWidth / wt * 100.000000;
                            el1.style.width = f + '%';
                        });
                        $element.find('table.th-tables')[0].style.width = $element.find('div.mic-tables-div>table>').width() + 'px';
                        return;
                    }
                    first.each(function (i, el) {
                        var el1 = angular.element(el);
                        var el2 = angular.element(last[i]);
                        var w1 = el1.width();
                        var w2 = el2.width();
                        if (w1 >= w2) {
                            el2[0].childNodes[0].style.width = w1 + 'px';
                        } else {
                            el1[0].childNodes[0].style.width = w2 + 'px';
                        }
                    });
                    var h = 0;
                    var e1 = $element.find('table:last');
                    var e2 = $element.find('div.mic-tables-div');
                    if (e1.height() > e2.height()) {
                        h = 17;
                    }
                    $element.find('div.mic-tables-div')[0].style.width = ($element.find('table:first').width() + h) + 'px';

                    return;
                }, 26);
            }


            $scope.$watch(function () {
                var len = $element.find('div.mic-tables-div>table>tbody>tr:last>td').length;
                len += $element.find('div.mic-tables-div>table>tbody>tr').length;
                return $element.width() + window.innerWidth + len;
            }, function (newValue, oldValue) {
                if ($element.attr('dynamic') !== undefined) {//宽表格（有纵向滚动条）
                    setth11();
                } else {
                    setth();
                }
            });

            $scope.cellClickFun = function (row, key, event, bo) {
                if (angular.isFunction($scope.onDetailClick)) {
                    $scope.onDetailClick(row, key, event);
                }
                if (bo)
                    $timeout(function () {
                        if (event.target.localName !== 'td')
                            angular.element(event.target).parent().children().first().select();
                        else
                            angular.element(event.target).children().first().select();
                    }, 50);
            };


            //按钮的回调
            $scope.table_btn_click = function (row, index, name, evt) {

                row.tableBtnParams[index].click(row, name, evt);
            };

            $scope.keydownTableInput = function (row, evt, i) {
                if (evt.keyCode === 13) {
                    row.isShowInput[i] = false;
                }
            };
            var selected_branch;
            $scope.tableSelected = function (row, evt, index) {
                $scope.currentIndex = index;
                if ($scope.onCellClick && angular.isFunction($scope.onCellClick)) $scope.onCellClick(row, evt);
                if (row !== selected_branch) {
                    if (selected_branch != null) {
                        selected_branch.selected = false;
                    }
                    row.selected = true;
                    selected_branch = row;
                }
            };

            $scope.allischecked = !!$scope.allischecked;
            //表格勾选函数的回掉
            function retstr(str) {
                str = str.replace(',"ischecked":true', '').replace(',"ischecked":false', '').replace(',"selected":true', '').replace(',"selected":false', '');
                return str;
            }
            $scope.tableCheckClick = function (row) {
                if (row) {
                    row.ischecked = !row.ischecked;
                    if (row.ischecked) {
                        $scope.checkedData.push(row);
                        if ($scope.datasets.length === $scope.checkedData.length) {
                            $scope.allischecked = true;
                        }
                    }
                    else {
                        var arr = [];
                        for (var i = 0; i < $scope.checkedData.length; i++) {
                            //console.log(row);
                            //console.log($scope.checkedData[i]);
                            //console.log(angular.toJson(row) === angular.toJson($scope.checkedData[i]));
                            if (!!row.id && row.id === $scope.checkedData[i].id) {
                                $scope.checkedData.splice(i, 1);
                                break;
                            } else if (retstr(angular.toJson(row)) === retstr(angular.toJson($scope.checkedData[i]))) {
                                $scope.checkedData.splice(i, 1);
                                break;
                            }
                        }
                        if ($scope.checkedData.length === 0) {
                            $scope.allischecked = false;
                        }
                    }
                    if (typeof ($scope.onTdChecked) === "function") {
                        $timeout(function () { $scope.onTdChecked(row); }, 0);
                    }
                } else {
                    $scope.allischecked = !$scope.allischecked;
                    for (var i in $scope.datasets) {
                        $scope.datasets[i].ischecked = $scope.allischecked;
                    }
                    if ($scope.allischecked)
                        $scope.checkedData = angular.copy($scope.datasets);
                    else
                        $scope.checkedData = [];

                    if (typeof ($scope.onThChecked) === "function") {
                        $timeout(function () { $scope.onThChecked($scope.allischecked); }, 0);
                    }
                }
            };
        }],
        replace: true
    }
});