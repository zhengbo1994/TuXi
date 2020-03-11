(function () {
    var module,
      __indexOf = [].indexOf || function (item) { for (var i = 0, l = this.length; i < l; i++) { if (i in this && this[i] === item) return i; } return -1; };

    module = angular.module('angularBootstrapNavTree', []);

    module.directive('abnTree', [
      '$timeout', '$rootScope', 'MicTablesServer', function ($timeout, $rootScope, MicTablesServer) {
          return {
              restrict: 'E',
              template: function (element, attrs) {
                  var tmp = "";
                  if (angular.isDefined(attrs.micTableTree)) {
                      var micTabServ = new MicTablesServer({ attrs: attrs, cellFunName: 'table_item_onclick', row: 'item.branch', btnFunName: 'tree_tablebtn_click' });
                      var backCell = micTabServ._createTableCell();
                      var thead = backCell.thead;
                      tmp = backCell.td;


                      if (angular.isDefined(attrs.tableBtnHtml)) {
                          var backBtn = micTabServ._createTableBtn();
                          thead += backBtn.thead;
                          tmp += backBtn.td;
                      }

                      //console.log(tmp);

                      var tmp_fixedHeight = '<div class="abn-tree mic-treetables">' +
                                '<table class="th-treetables table table-bordered table-hover">' +
                                '<thead>' +
                                '<tr><th style="width:45px;">行号</th><th><div style="text-align:left;margin:0;white-space:nowrap;word-wrap:normal;overflow:hidden;display:block;">' + thead + '<div></th></tr>' +
                                '</thead>' +
                                '</table>' +
                                '<div class="mic-treetables-div" style="overflow-y: auto;height:' + attrs.tbodyHeight + 'px;">' +
                                '<table class="table table-bordered table-hover">' +
                                '<tbody class="nav nav-list nav-pills nav-stacked abn-tree ">' +
                                '<tr ng-repeat="item in tree_rows | filter:{visible:true} track by item.branch.uid" ng-animate="\'abn-tree-animate\'" ng-class="\'level-\' + {{ item.level }} +(item.branch.showCheckbox ? \'\' : \' \'+ (item.branch.selected ? (item.branch.isHidActive===true ? \'\':\' active\'):\'\')) + \' \' +item.classes.join(\' \')"  class="abn-tree-row" >' +
                                '  <td><span>{{item.index}}</span></td>' +
                                '  <td><div style="text-align:left;margin:0;white-space:nowrap;word-wrap:normal;overflow:hidden;display:block;"><a class="indented margin-right5" ><i ng-class=\"item.tree_icon\" ng-click=\"item.branch.expanded = !item.branch.expanded\" class=\"tree-icon\"/></a><a class="indented" ng-click="user_clicks_branch(item.branch,null,$event)" title="{{item.branch.label}}">{{item.branch.label}}</a></div></td>' +
                                tmp +
                                '</tr></tbody></table></div></div>';
                      if (attrs.tbodyHeight) { return tmp_fixedHeight; }

                      tmp = '<table class="table table-bordered table-hover">' +
                        '<thead><tr><th style="width:45px;">行号</th><th>' + thead + '</th></tr></thead>' +
                        '<tbody class="nav nav-list nav-pills nav-stacked abn-tree ">' +
                        '<tr ng-repeat="item in tree_rows | filter:{visible:true} track by item.branch.uid" ng-animate="\'abn-tree-animate\'" ng-class="\'level-\' + {{ item.level }} +(item.branch.showCheckbox ? \'\' : \' \'+ (item.branch.selected ? (item.branch.isHidActive===true ? \'\':\' active\'):\'\')) + \' \' +item.classes.join(\' \')"  class="abn-tree-row" >' +
                        '  <td><span>{{item.index}}</span></td>' +
                        '  <td><a class="indented margin-right5" ><i ng-class="item.tree_icon" ng-click="item.branch.expanded = !item.branch.expanded" class="tree-icon"/></a><a class="indented" ng-click="user_clicks_branch(item.branch,null,$event)" >{{item.branch.label}}</a></td>' +
                        tmp +
                        '</tr></tbody></table>';


                  } else {
                      if (angular.isDefined(attrs.othertoolable)) {
                          tmp = '<label style="margin-left:50px" ></label>' +
                              '<div id="treediv_{{row.branch.uid}}" ng-class="(row.branch.showFirstHtml ? \'checkbox info-check check-success checkbox-inline\' : \'no-check\')" class="indented " ng-click="user_clicks_branch(row.branch,1,$event)">' +
                              ' <input id="treeid_{{row.branch.uid}}" type="checkbox" ng-checked="(row.branch.firstHtml.checked1 ? \'checked\':\'\')">' +
                              ' <label ng-show="row.branch.showFirstHtml" ng-class="row.branch.isHidActive===true?\'\':row.branch.selected ? \'laber-color2\' : \'laber-color1\'" for="treeid_{{row.branch.uid}}" >下载</label>' +
                              '</div>' +
                              '<label style="margin-left:20px" ></label>' +
                              '<div id="treediv2_{{row.branch.uid}}" ng-class="(row.branch.showFirstHtml ? \'checkbox info-check check-success checkbox-inline\' : \'no-check\')" class="indented " ng-click="user_clicks_branch(row.branch,2,$event)">' +
                              ' <input id="treeid2_{{row.branch.uid}}" type="checkbox" ng-checked="(row.branch.firstHtml.checked2 ? \'checked\':\'\')">' +
                              ' <label ng-show="row.branch.showFirstHtml" ng-class="row.branch.isHidActive===true?\'\':row.branch.selected ? \'laber-color2\' : \'laber-color1\'" for="treeid2_{{row.branch.uid}}" >浏览</label>' +
                              '</div>';
                      }

                      tmp = '<ul class="nav nav-list nav-pills nav-stacked abn-tree">' +
                           ' <li ng-repeat="row in tree_rows | filter:{visible:true} track by row.branch.uid" ' +
                           '  ng-animate="\'abn-tree-animate\'" ' +
                           '  ng-class="\'level-\' + {{ row.level }} +(row.branch.showCheckbox ? \'\' : \' margin-sx5-a \'+ (row.branch.selected ? (row.branch.isHidActive===true ? \'\':\' active\'):\'\')) + \' \' +row.classes.join(\' \')" ' +
                           '  class="abn-tree-row" >' +
                           '     <a ng-click="user_clicks_branch(row.branch,null,$event)">' +
                           '          <i ng-class="row.tree_icon" ng-click="icon_click(row,$event)" ' +
                           '               class="indented tree-icon">' +
                           '          </i>' +
                           '          <div ng-if="row.branch.showCheckbox" id="treedivck_{{row.branch.uid}}" ' +
                           '               ng-class="row.branch.showCheckbox ? \'info-check check-primary checkbox-inline\' : \'no-check\'" ' +
                           '               ng-click="user_clicks_branch(row.branch,0,$event)">' +
                           '             <input id="treeidck_{{row.branch.uid}}" type="checkbox" ng-checked="row.branch.checked ? \'checked\':\'\'">' +
                           '             <label class="tree-label" ng-class="{\'blue-label\':row.branch.checked}" for="treeidck_{{row.branch.uid}}" >{{ row.label }}</label>' +
                           '         </div>' +
                           '         <span ng-if="!row.branch.showCheckbox" class="indented tree-label">{{ row.label }} </span>' +
                           tmp +
                           '     </a>' +
                           ' </li>' +
                           '</ul>';


                      if (angular.isDefined(attrs.micComboTree)) {
                          tmp = '<div class="trees-combos-select" ng-class="{\'trees-combos-act\':isCsActive}">' +
                              '<span class="trees-combospan" ng-click="isCsActive=!isCsActive">{{comboSpanText}}</span>' +
                              '<div class="trees-combobody">' +
                              tmp +
                              '</div></div>';
                      }
                  }
                  return tmp;
              },
              replace: true,
              scope: {
                  treeData: '=?',
                  onSelect: '=?',
                  onChecked: '=?',
                  initialSelection: '@',
                  treeControl: '=?',
                  selectedData: '=?',
                  tableBtnParams: '=?',
                  //替代双向绑定失效的treeControl，将它作为回调返回
                  returnCtrl: '=?'
              },
              link: function (scope, element, attrs) {
                  scope.treeData = !scope.treeData ? [] : scope.treeData;

                  if (element.attr('on-checked') !== undefined) { scope.selectedData = []; }
                  function set_comb_view() {
                      if (!angular.isDefined(attrs.micComboTree)) { return; }
                      var str = '';
                      for_each_branch(function (node) {
                          if (node.checked && node.children.length === 0)
                              str += node.label + ',';
                      });
                      str = str.substring(0, str.length - 1);;
                      scope.comboSpanText = str;
                  }
                  
                  if (angular.isDefined(attrs.micComboTree) && element.attr('on-checked') === undefined) {
                      scope.$watch('selectedData', function (v) {
                          //console.log(v);
                          scope.comboSpanText = v.label;
                          scope.isCsActive = false;
					  });
				  }
				  
                  //动态设置表格宽度

                  scope.$watch(function () {
                      var trw = element.find('div.mic-treetables-div>table>tbody>tr:first');
                      var tbd = element.find('div.mic-treetables-div>table>tbody');
                      return trw.length === 0 ? null : trw.width() + tbd.height();
                  }, function (w) {
                      if (!w) { return; }
                      $timeout(function () {
                          setTableWidth(w);
                      }, 260);

                  });
                  function setTableWidth(w) {
                      w = false;
                      if (!w) {
                          var td = element.find('div.mic-treetables-div>table>tbody>tr:first');
                          if (td.length === 0) { return; }
                          w = td.width();
                      }
                      var td_arr = element.find('div.mic-treetables-div>table>tbody>tr:first>td');
                      if (td_arr.length === 0) { return; }
                      var th_arr = element.find('table.th-treetables>thead>tr>th');
                      th_arr.each(function (i, el) {
                          var setw = td_arr[i].offsetWidth / w * 100;
                          if (i === 0) {
                              setw = 3;
                              angular.element(el).css('minWidth', '45px');
                              angular.element(td_arr[i]).css('minWidth', '45px');
                          } else {
                              var wid = angular.element(td_arr[i]).width() - 16;
                              angular.element(el.childNodes[0]).css('width', wid + 'px');
                          }
                          angular.element(el).css('width', setw + '%');
                          angular.element(td_arr[i]).css('width', setw + '%');
                      });
                      element.find('table.th-treetables').width(w);
                      element.find('table.th-treetables').css('width', w);
                  }

                  //操作栏按钮回调
                  if (angular.isDefined(attrs.micTableTree) && scope.tableBtnParams) {

                      scope.tree_tablebtn_click = function (branch, index, name, evt) {
                          if (branch !== selected_branch) {
                              if (selected_branch != null) {
                                  selected_branch.selected = false;
                              }
                              branch.selected = true;
                              selected_branch = branch;
                          }
                          branch.tableBtnParams[index].click(branch, name, evt);
                      };
                  }

                  var _isFirstLoad = true, error, expand_all_parents, expand_level, for_all_ancestors, for_each_branch, get_parent, n, on_treeData_change, select_branch, selected_branch, tree;
                  error = function (s) {
                      //console.log('ERROR:' + s);
                      debugger;
                      return void 0;
                  };
                  if (attrs.iconExpand == null) {
                      attrs.iconExpand = 'icon-plus  ti-plus';
                  }
                  if (attrs.iconCollapse == null) {
                      attrs.iconCollapse = 'icon-minus ti-minus';
                  }
                  if (attrs.iconLeaf == null) {
                      attrs.iconLeaf = 'icon-file  ti-file';
                  } else if (attrs.iconLeaf === "") {
                      attrs.iconLeaf = 'margin-left18';
                  }
                  if (attrs.expandLevel == null) {
                      attrs.expandLevel = '3';
                  }
                  expand_level = parseInt(attrs.expandLevel, 10);



                  if (scope.treeData.length == null) {
                      if (treeData.label != null) {
                          scope.treeData = [treeData];
                      } else {
                          alert('treeData should be an array of root branches');
                          return;
                      }
                  }

                  for_each_branch = function (f) {

                      var do_f, root_branch, _i, _len, _ref, _results;
                      do_f = function (branch, level) {
                          var child, _i, _len, _ref, _results;
                          f(branch, level);


                          if (_isFirstLoad && angular.isDefined(attrs.micTableTree) && scope.tableBtnParams && angular.isArray(scope.tableBtnParams)) {
                              branch.tableBtnParams = angular.copy(scope.tableBtnParams);
                          }


                          if (branch.children != null) {
                              _ref = branch.children;
                              _results = [];
                              for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                                  child = _ref[_i];
                                  _results.push(do_f(child, level + 1));
                              }
                              return _results;
                          }
                      };
                      _ref = scope.treeData;
                      _results = [];
                      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                          root_branch = _ref[_i];
                          _results.push(do_f(root_branch, 1));
                      }
                      return _results;
                  };
                  set_comb_view();
                  selected_branch = null;
                  select_branch = function (branch, $event) {
                      if (!branch) {
                          if (selected_branch != null) {
                              selected_branch.selected = false;
                          }
                          selected_branch = null;
                          return;
                      }
                      if (branch !== selected_branch) {
                          if (selected_branch != null) {
                              selected_branch.selected = false;
                          }
                          branch.selected = true;
                          selected_branch = branch;
                          expand_all_parents(branch);
                          scope.selectedData = branch;
                          if (branch.onSelect != null) {
                              return $timeout(function () {
                                  return branch.onSelect(branch, get_parent, $event);
                              });
                          } else {
                              if (scope.onSelect != null) {
                                  return $timeout(function () {
                                      return scope.onSelect(branch, get_parent, $event);
                                  });
                              }
                          }
                      }
                  };
                  scope.table_item_onclick = function (branch, n, $event) {
                      if (branch !== selected_branch) {
                          if (selected_branch != null) {
                              selected_branch.selected = false;
                          }
                          branch.selected = true;
                          selected_branch = branch;
                      }

                      scope.selectedData = branch;
                      if (scope.onSelect != null) {
                          return $timeout(function () {
                              return scope.onSelect(branch, n, $event);
                          });
                      }
                  };
                  scope.icon_click = function (row, event) {
                      row.branch.expanded = !row.branch.expanded;
                      
                      event = event ? event : window.event;
                      
                      if (event && event.stopPropagation) {
                          event.stopPropagation();
                      }
                      else {
                          event.cancelBubble = true;
                      }
                  };
                  scope.user_clicks_branch = function (branch, index, $event) {
                      if (index === 0) {
                          branch.checked = !branch.checked;
                          document.getElementById('treedivck_' + branch.uid).className = document.getElementById('treedivck_' + branch.uid).className.replace('infoAfter', '');

                          function delItemFromArr(branch) {
                              for (var x = 0; x < scope.selectedData.length; x++) {
                                  if (scope.selectedData[x].uid === branch.uid) {
                                      break;
                                  }
                              }
                              scope.selectedData.splice(x, 1);
                          }

                          var infoCheckChild = function (dt) {
                              for (var _i = 0; _i < dt.length; _i++) {
                                  dt[_i].checked = branch.checked;
                                  if (branch.checked && document.getElementById('treedivck_' + dt[_i].uid)) {
                                      document.getElementById('treedivck_' + dt[_i].uid).className = document.getElementById('treedivck_' + dt[_i].uid).className.replace('infoAfter', '');
                                  }
                                  if (branch.checked) {
                                      scope.selectedData.push(dt[_i]);
                                  } else {
                                      delItemFromArr(dt[_i]);
                                  }

                                  if (dt[_i].children.length > 0) {
                                      infoCheckChild(dt[_i].children);
                                  }
                              }
                          }
                          if (branch.children.length > 0) {
                              infoCheckChild(branch.children);
                          }
                          var infoCheckParent = function (node, isPnode) {
                              var bo1 = false, bo2 = false;
                              var Cnode = node.children;
                              for (var i = 0; i < Cnode.length; i++) {
                                  if (Cnode[i].checked) {
                                      bo1 = true;
                                  } else {
                                      bo2 = true;
                                  }
                              }

                              document.getElementById('treedivck_' + node.uid).className = document.getElementById('treedivck_' + node.uid).className.replace('infoAfter', '');
                              if (bo1 && bo2 || isPnode) {//半选
                                  node.checked = true;
                                  document.getElementById('treedivck_' + node.uid).className += ' infoAfter';
                                  delItemFromArr(node);
                              } else if (bo1 && !bo2) {//全选
                                  node.checked = true;
                                  if (isPnode !== undefined)
                                      scope.selectedData.push(node);
                              } else if (!bo1 && bo2) {//全不选
                                  node.checked = false;
                                  delItemFromArr(node);
                              }
                              if (get_parent(node)) {
                                  var b = false;
                                  var ret = get_parent(node).children;
                                  for (var k = 0; k < ret.length; k++) {
                                      var str = document.getElementById('treedivck_' + ret[k].uid).className;
                                      if (str.lastIndexOf('infoAfter') > -1) {
                                          b = true;
                                      }
                                  }
                                  infoCheckParent(get_parent(node), b);
                              }
                          }

                          if (branch.checked) {
                              scope.selectedData.push(branch);
                          } else {
                              delItemFromArr(branch);
                          }

                          if (get_parent(branch)) {
                              infoCheckParent(branch);
                          }

                          if (branch.onCheck != null) {
                              $timeout(function () {
                                  branch.onCheck(branch, get_parent);
                              });
                          } else if (scope.onChecked != null) {
                              $timeout(function () {
                                  scope.onChecked(branch, get_parent);
                              });
                          }
                          set_comb_view();
                          return;
                      } else if (index === 1) {
                          branch.firstHtml.checked1 = !branch.firstHtml.checked1;
                          $timeout(function () {
                              branch.firstHtml.onCheck1(branch);
                          });
                          return;
                      } else if (index === 2) {
                          branch.firstHtml.checked2 = !branch.firstHtml.checked2;
                          $timeout(function () {
                              branch.firstHtml.onCheck2(branch);
                          });
                          return;
                      }
                      if (branch !== selected_branch && !branch.showCheckbox || branch !== selected_branch && element[0].hasAttribute("mic-table-tree")) {
                          return select_branch(branch, $event);
                      }
                  };
                  get_parent = function (child) {
                      var parent;
                      parent = void 0;
                      if (child.parent_uid) {
                          for_each_branch(function (b) {
                              if (b.uid === child.parent_uid) {
                                  return parent = b;
                              }
                          });
                      }
                      return parent;
                  };
                  for_all_ancestors = function (child, fn) {
                      var parent;
                      parent = get_parent(child);
                      if (parent != null) {
                          fn(parent);
                          return for_all_ancestors(parent, fn);
                      }
                  };
                  expand_all_parents = function (child) {
                      return for_all_ancestors(child, function (b) {
                          return b.expanded = true;
                      });
                  };
                  scope.tree_rows = [];
                  on_treeData_change = function () {
                      $timeout(function () { setTableWidth(); }, 260);

                      var add_branch_to_list, root_branch, _i, _len, _ref, _results;
                      for_each_branch(function (b, level) {
                          if (!b.uid) {
                              return b.uid = "" + Math.random();
                          }
                      });
                      //console.log('UIDs are set.');
                      for_each_branch(function (b) {
                          var child, _i, _len, _ref, _results;
                          if (angular.isArray(b.children)) {
                              _ref = b.children;
                              _results = [];
                              for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                                  child = _ref[_i];
                                  _results.push(child.parent_uid = b.uid);
                              }
                              return _results;
                          }
                      });
                      scope.tree_rows = [];
                      for_each_branch(function (branch) {
                          var child, f;
                          if (branch.children) {
                              if (branch.children.length > 0) {
                                  f = function (e) {
                                      if (typeof e === 'string') {
                                          return {
                                              label: e,
                                              children: []
                                          };
                                      } else {
                                          return e;
                                      }
                                  };
                                  return branch.children = (function () {
                                      var _i, _len, _ref, _results;
                                      _ref = branch.children;
                                      _results = [];
                                      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                                          child = _ref[_i];
                                          _results.push(f(child));
                                      }
                                      return _results;
                                  })();
                              }
                          } else {
                              return branch.children = [];
                          }
                      });
                      add_branch_to_list = function (level, branch, visible) {
                          var child, child_visible, tree_icon, _i, _len, _ref, _results;
                          if (branch.expanded == null) {
                              branch.expanded = false;
                          }
                          if (branch.classes == null) {
                              branch.classes = [];
                          }
                          if (branch.isLeaf == null) {
                              branch.isLeaf = false;
                          }
                          if (!branch.noLeaf && (!branch.children || branch.children.length === 0)) {
                              tree_icon = attrs.iconLeaf;
                              branch.isLeaf = true;
                              if (__indexOf.call(branch.classes, "leaf") < 0) {
                                  branch.classes.push("leaf");
                              }
                          } else {
                              if (branch.expanded) {
                                  tree_icon = attrs.iconCollapse;
                              } else {
                                  tree_icon = attrs.iconExpand;
                              }
                          }
                          var tree_rows_index = scope.tree_rows.length + 1;
                          scope.tree_rows.push({
                              index: tree_rows_index,
                              level: level,
                              branch: branch,
                              label: branch.label,
                              isLeaf: branch.isLeaf,
                              classes: branch.classes,
                              tree_icon: tree_icon,
                              visible: visible
                          });
                          if (branch.children != null) {
                              _ref = branch.children;
                              _results = [];
                              for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                                  child = _ref[_i];
                                  child_visible = visible && branch.expanded;
                                  _results.push(add_branch_to_list(level + 1, child, child_visible));
                              }
                              return _results;
                          }
                      };
                      _ref = scope.treeData;
                      _results = [];
                      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                          root_branch = _ref[_i];
                          _results.push(add_branch_to_list(1, root_branch, true));
                      }
                      _isFirstLoad = false;
                      return _results;

                  };
                  scope.$watch('treeData', on_treeData_change, true);
                  if (attrs.initialSelection != null) {
                      for_each_branch(function (b) {
                          if (b.showCheckbox && b.checked) {
                              scope.selectedData = [];
                              scope.selectedData.push(b);
                          } else if (b.label === attrs.initialSelection) {
                              return $timeout(function () {
                                  //console.log(b);
                                  return select_branch(b);
                              });
                          }
                      });
                  }
                  n = scope.treeData.length;
                  //console.log('num root branches = ' + n);
                  for_each_branch(function (b, level) {
                      b.level = level;
                      return b.expanded = b.level < expand_level;
                  });

                  scope.treeControl = {};
                  if (scope.treeControl !== null) {
                      if (angular.isObject(scope.treeControl)) {
                          tree = scope.treeControl;
                          tree.expand_all = function () {
                              return for_each_branch(function (b, level) {
                                  return b.expanded = true;
                              });
                          };
                          tree.collapse_all = function () {
                              return for_each_branch(function (b, level) {
                                  return b.expanded = false;
                              });
                          };
                          tree.get_first_branch = function () {
                              n = scope.treeData.length;
                              if (n > 0) {
                                  return scope.treeData[0];
                              }
                          };
                          tree.select_first_branch = function () {
                              var b;
                              b = tree.get_first_branch();
                              return tree.select_branch(b);
                          };
                          tree.get_selected_branch = function () {
                              return selected_branch;
                          };
                          tree.get_parent_branch = function (b) {
                              return get_parent(b);
                          };
                          tree.select_branch = function (b) {
                              select_branch(b);
                              return b;
                          };
                          tree.get_children = function (b) {
                              return b.children;
                          };
                          tree.select_parent_branch = function (b) {
                              var p;
                              if (b == null) {
                                  b = tree.get_selected_branch();
                              }
                              if (b != null) {
                                  p = tree.get_parent_branch(b);
                                  if (p != null) {
                                      tree.select_branch(p);
                                      return p;
                                  }
                              }
                          };
                          tree.add_branch = function (parent, new_branch) {
                              if (parent != null) {
                                  parent.children.push(new_branch);
                                  parent.expanded = true;
                              } else {
                                  scope.treeData.push(new_branch);
                              }
                              return new_branch;
                          };
                          tree.add_root_branch = function (new_branch) {
                              tree.add_branch(null, new_branch);
                              return new_branch;
                          };
                          tree.expand_branch = function (b) {
                              if (b == null) {
                                  b = tree.get_selected_branch();
                              }
                              if (b != null) {
                                  b.expanded = true;
                                  return b;
                              }
                          };
                          tree.collapse_branch = function (b) {
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  b.expanded = false;
                                  return b;
                              }
                          };
                          tree.get_siblings = function (b) {
                              var p, siblings;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  p = tree.get_parent_branch(b);
                                  if (p) {
                                      siblings = p.children;
                                  } else {
                                      siblings = scope.treeData;
                                  }
                                  return siblings;
                              }
                          };
                          tree.get_next_sibling = function (b) {
                              var i, siblings;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  siblings = tree.get_siblings(b);
                                  n = siblings.length;
                                  i = siblings.indexOf(b);
                                  if (i < n) {
                                      return siblings[i + 1];
                                  }
                              }
                          };
                          tree.get_prev_sibling = function (b) {
                              var i, siblings;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              siblings = tree.get_siblings(b);
                              n = siblings.length;
                              i = siblings.indexOf(b);
                              if (i > 0) {
                                  return siblings[i - 1];
                              }
                          };
                          tree.select_next_sibling = function (b) {
                              var next;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  next = tree.get_next_sibling(b);
                                  if (next != null) {
                                      return tree.select_branch(next);
                                  }
                              }
                          };
                          tree.select_prev_sibling = function (b) {
                              var prev;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  prev = tree.get_prev_sibling(b);
                                  if (prev != null) {
                                      return tree.select_branch(prev);
                                  }
                              }
                          };
                          tree.get_first_child = function (b) {
                              var _ref;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  if (((_ref = b.children) != null ? _ref.length : void 0) > 0) {
                                      return b.children[0];
                                  }
                              }
                          };
                          tree.get_closest_ancestor_next_sibling = function (b) {
                              var next, parent;
                              next = tree.get_next_sibling(b);
                              if (next != null) {
                                  return next;
                              } else {
                                  parent = tree.get_parent_branch(b);
                                  return tree.get_closest_ancestor_next_sibling(parent);
                              }
                          };
                          tree.get_next_branch = function (b) {
                              var next;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  next = tree.get_first_child(b);
                                  if (next != null) {
                                      return next;
                                  } else {
                                      next = tree.get_closest_ancestor_next_sibling(b);
                                      return next;
                                  }
                              }
                          };
                          tree.select_next_branch = function (b) {
                              var next;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  next = tree.get_next_branch(b);
                                  if (next != null) {
                                      tree.select_branch(next);
                                      return next;
                                  }
                              }
                          };
                          tree.last_descendant = function (b) {
                              var last_child;
                              if (b == null) {
                                  debugger;
                              }
                              n = b.children.length;
                              if (n === 0) {
                                  return b;
                              } else {
                                  last_child = b.children[n - 1];
                                  return tree.last_descendant(last_child);
                              }
                          };
                          tree.get_prev_branch = function (b) {
                              var parent, prev_sibling;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  prev_sibling = tree.get_prev_sibling(b);
                                  if (prev_sibling != null) {
                                      return tree.last_descendant(prev_sibling);
                                  } else {
                                      parent = tree.get_parent_branch(b);
                                      return parent;
                                  }
                              }
                          };
                          tree.select_prev_branch = function (b) {
                              var prev;
                              if (b == null) {
                                  b = selected_branch;
                              }
                              if (b != null) {
                                  prev = tree.get_prev_branch(b);
                                  if (prev != null) {
                                      tree.select_branch(prev);
                                      return prev;
                                  }
                              }
                          };
                      }
                  }

                  if (scope.returnCtrl != null) {
                      return $timeout(function () {
                          //console.log(tree);
                          return scope.returnCtrl(tree);
                      });
                  }
              }
          };
      }
    ]);


}).call(this);
