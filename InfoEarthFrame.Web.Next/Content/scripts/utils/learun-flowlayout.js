/*!
 * 版 本 LearunADMS V6.1.1.7 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * LR215
 */
(function ($) {
    $.fn.flowDesigner = function (options) {
        var $flowDesigner = $(this);
        if (!$flowDesigner.attr('id')) {
            return false;
        }
        $flowDesigner.html("");
        var options = $.extend( {
            schemeContent: "",
            frmType: 0,//自定义表单0,系统表单1
            frmData: "",
            width: $(window).width(),
            height: $(window).height() + 2,
            OpenNode: function () { return false },
            OpenLine: function () { return false },
            NodeRemarks: {
                cursor: "选择指针",
                direct: "步骤连线",
                startround: "开始节点",
                endround: "结束节点",
                stepnode: "普通节点",
                shuntnode: "分流节点",
                confluencenode: "合流节点",
                group: "区域规划"
            },
            haveTool: true,
            toolBtns: ["startround", "endround", "stepnode", "shuntnode", "confluencenode"],
            isprocessing: false,
            nodeData: null,
            activityId: "",
            preview: 0
        }, options);
        var flowDesigner = $.createGooFlow($flowDesigner, {
            width: options.width,
            height: options.height,
            haveHead: true,
            headBtns: ["undo", "redo"],
            haveTool: options.haveTool,
            toolBtns: options.toolBtns,
            haveGroup: true,
            useOperStack: true
        });
        flowDesigner.setNodeRemarks(options.NodeRemarks);
        flowDesigner.loadData(options.schemeContent);
        OpenNode = options.OpenNode;
        OpenLine = options.OpenLine;
        //导出数据扩展方法
        //所有节点必须有进出线段
        //必须有开始结束节点（且只能为一个）
        //分流合流节点必须成对出现
        //分流合流节点必须一一对应且中间必须有且只能有一个普通节点
        //分流节点与合流节点之前的审核节点必须有且只能有一条出去和进来节点
        flowDesigner.exportDataEx = function () {
            var _data = flowDesigner.exportData();
            var _fromlines = {}, _tolines = {}, _nodes = {}, _fnodes = [], _hnodes = [], _startroundFlag = 0, _endroundFlag = 0;
            for (var i in _data.lines) {
                if (_fromlines[_data.lines[i].from] == undefined) {
                    _fromlines[_data.lines[i].from] = [];
                }
                _fromlines[_data.lines[i].from].push(_data.lines[i].to);

                if (_tolines[_data.lines[i].to] == undefined) {
                    _tolines[_data.lines[i].to] = [];
                }
                _tolines[_data.lines[i].to].push(_data.lines[i].from);
            }
            for (var j in _data.nodes) {
                var _node = _data.nodes[j];
                var _flag = false;
                switch (_node.type) {
                    case "startround":
                        _startroundFlag++;
                        if (_fromlines[_node.id] == undefined) {
                            dialogTop("开始节点无法流转到下一个节点", "error");
                            return -1;
                        }
                        break;
                    case "endround":
                        _endroundFlag++;
                        if (_tolines[_node.id] == undefined) {
                            dialogTop("无法流转到结束节点", "error");
                            return -1;
                        }
                        break;
                    case "stepnode":
                        _flag = true;
                        break;
                    case "shuntnode":
                        _flag = true;
                        _fnodes.push(_node.id);
                        break;
                    case "confluencenode":
                        _hnodes.push(_node.id);
                        _flag = true;
                        break;
                    default:
                        dialogTop("节点数据异常,请重新登录下系统！", "error");
                        return -1;
                        break;
                }
                if (_flag) {
                    if (_tolines[_node.id] == undefined) {
                        labellingRedNode(_node.id);
                        dialogTop("标注红色的节点没有【进来】的连接线段", "error");
                        return -1;
                    }
                    if (_fromlines[_node.id] == undefined) {
                        labellingRedNode(_node.id);
                        dialogTop("标注红色的节点没有【出去】的连接线段", "error");
                        return -1;
                    }
                }
                _nodes[_node.id] = _node;
            }
            if (_startroundFlag == 0) {
                dialogTop("必须有开始节点", "error");
                return -1;
            }

            if (_endroundFlag == 0) {
                dialogTop("必须有结束节点", "error");
                return -1;
            }

            if (_fnodes.length != _hnodes.length) {
                dialogTop("分流节点必须等于合流节点", "error");
                return -1;
            }
            for (var a in _fnodes) {
                var aNondeid = _fnodes[a];
                if (_fromlines[aNondeid].length == 1) {
                    labellingRedNode(aNondeid);
                    dialogTop("标注红色的分流节点不允许只有一条【出去】的线段", "error");
                    return -1;
                }
                var _hhnodeid = {};
                for (var b in _fromlines[aNondeid]) {
                    btoNode = _fromlines[aNondeid][b];
                    if (_nodes[btoNode].type == "stepnode") {
                        var _nextLine = _fromlines[_nodes[btoNode].id];

                        var _nextNode = _nodes[_nextLine[0]];
                        if (_nextNode.type != "confluencenode") {
                            labellingRedNode(_nodes[btoNode].id);
                            dialogTop("标注红色的普通节点下一个节点必须是合流节点", "error");
                            return -1;
                        }
                        else {
                            _hhnodeid[_nextLine[0]] = 0;
                            if (_hhnodeid.length > 1) {
                                labellingRedNode(aNondeid);
                                dialogTop("标注红色的分流节点与之对应的合流节点只能有一个", "error");
                                return -1;
                            }
                            if (_tolines[_nextLine[0]].length != _fromlines[aNondeid].length) {
                                labellingRedNode(_nextLine[0]);
                                dialogTop("标注红色的合流节点与之对应的分流节点只能有一个", "error");
                                return -1;
                            }
                        }
                        if (_nextLine.length > 1) {
                            labellingRedNode(_nodes[btoNode].id);
                            dialogTop("标注红色的节点只能有一条出去的线条【分流合流之间】", "error");
                            return -1;
                        }
                        else if (_tolines[_nodes[btoNode].id], length > 1) {
                            labellingRedNode(_nodes[btoNode].id);
                            dialogTop("标注红色的节点只能有一条进来的线条【分流合流之间】", "error");
                            return -1;
                        }
                    }
                    else {
                        labellingRedNode(aNondeid);
                        dialogTop("标注红色的分流节点必须经过一个普通节点到合流节点", "error");
                        return -1;
                    }
                }
            }
            return _data;
        }
        flowDesigner.SetNodeEx = function (id, data) {
            flowDesigner.setName(id, data.nodeMyName, "node", data);
        }
        flowDesigner.SetLineEx = function (id, data) {
            flowDesigner.setName(id, data.lineMyName, "line", data);
        }
        if (options.isprocessing)//如果是显示进程状态
        {
            var tipHtml = '<div style="position:absolute;left:10px;margin-top: 10px;padding:10px;border-radius:5px;background:rgba(0,0,0,0.05);z-index:1000;display:inline-block;">';
            tipHtml += '<div style="display: inline-block;"><i style="padding-right:5px;color:#5cb85c;" class="fa fa-flag"></i><span>已处理</span></div>';
            tipHtml += '<div style="display: inline-block;margin-left: 10px;"><i style="padding-right:5px;color:#5bc0de;" class="fa fa-flag"></i><span>正在处理</span></div>';
            tipHtml += '<div style="display: inline-block;margin-left: 10px;"><i style="padding-right:5px;color:#d9534f;" class="fa fa-flag"></i><span>不通过</span></div>';
            tipHtml += '<div style="display: inline-block;margin-left: 10px;"><i style="padding-right:5px;color:#f0ad4e;" class="fa fa-flag"></i><span>驳回</span></div>';
            tipHtml += '<div style="display: inline-block;margin-left: 10px;"><i style="padding-right:5px;color:#999;" class="fa fa-flag"></i><span>未处理</span></div></div>';

            $flowDesigner.find('.GooFlow_work .GooFlow_work_inner').css('background-image', 'none');
            $flowDesigner.find('td').css('color', '#fff');
            $flowDesigner.css('background', '#fff');
            $flowDesigner.find('.ico').remove();
            $flowDesigner.find('.GooFlow_item').css('border', '0px');
            $flowDesigner.append(tipHtml);
            $flowDesigner.find('.GooFlow_item.stepnode').css("background", "#999");
            $flowDesigner.find('.GooFlow_item.confluencenode').css("background", "#999");
            $.each(options.nodeData, function (i, item) {
                console.log(item);
                if (item.F_NodeType == "startround" || item.F_NodeType == "confluencenode") {
                    $flowDesigner.find("#" + item.F_NodeId).css("background", "#5cb85c");
                }
                else {
                    if (item.F_IsActivtyId == 1) {
                        $flowDesigner.find("#" + item.F_NodeId).css("background", "#5bc0de");//正在处理
                    }
                    switch (item.F_NodeSate) {//0 未处理 1 通过 2不通过 3驳回
                        case 2:
                            $flowDesigner.find("#" + item.F_NodeId).css("background", "#d9534f");//不通过
                            break;
                        case 1:
                            $flowDesigner.find("#" + item.F_NodeId).css("background", "#5cb85c");//通过
                            break;
                        case 3:
                            $flowDesigner.find("#" + item.F_NodeId).css("background", "#f0ad4e");//驳回
                            break;
                    }
                }
                if (item.F_NodeType == "startround") {
                    var _row = '<div style="text-align:left">';
                    _row += "<p>发起人：" + getInfoTop().learun.data.get(["user", item.F_CreateUserId, "RealName"]); + "</p>";
                    _row += "<p>处理时间：" + item.F_CreateDate + "</p>";

                    $flowDesigner.find('#' + item.F_NodeId).attr('data-toggle', 'tooltip');
                    $flowDesigner.find('#' + item.F_NodeId).attr('data-placement', 'bottom');
                    $flowDesigner.find('#' + item.F_NodeId).attr('title', _row);
                }
                else if (item.F_NodeSate != 0 && item.F_NodeSate != 10) {
                    var _row = '<div style="text-align:left">';
                    var tagname = {  "1": "通过", "2": "不通过" };
                    _row += "<p>处理人：" + getInfoTop().learun.data.get(["user", item.F_ModifyUserId, "RealName"]); + "</p>";
                    _row += "<p>结果：" + tagname[item.F_NodeSate] + "</p>";
                    _row += "<p>处理时间：" + item.F_ModifyDate + "</p>";
                    _row += "<p>处理意见：" + item.F_Description + "</p></div>";

                    $flowDesigner.find('#' + item.F_NodeId).attr('data-toggle', 'tooltip');
                    $flowDesigner.find('#' + item.F_NodeId).attr('data-placement', 'bottom');
                    $flowDesigner.find('#' + item.F_NodeId).attr('title', _row);
                }
                 
            });
            $('[data-toggle="tooltip"]').tooltip({ "html": true });
        }
        if (options.preview == 1) {
            preview();
        }

        //预览
        function preview() {
            var _frmitems = {};
            for (var i in options.frmData) {
                var _frmitem = options.frmData[i];
                _frmitems[_frmitem.control_field] = _frmitem.control_label;
            }
            var DataBaseLinkData = {};
            $.ajax({
                url: "../../SystemManage/DataBaseLink/GetListJson",
                type: "get",
                dataType: "json",
                async: false,
                success: function (data) {
                    for (var i in data) {
                        DataBaseLinkData[data[i].F_DatabaseLinkId] = data[i].F_DBAlias;
                    }
                }
            });

            var _nodeRejectType = { "0": "前一步", "1": "第一步", "2": "某一步", "3": "用户指定", "4": "不处理" };
            var _nodeDesignate = { "1": "所有成员", "2": "指定成员", "3": "发起者领导", "4": "前一步骤领导", "5": "发起者部门领导", "6": "发起者公司领导" };
            var _nodeConfluenceType = { "0": "所有步骤通过", "1": "一个步骤通过即可", "2": "按百分比计算" };
            $.each(options.schemeContent.nodes, function (i, item) {
                console.log(item);
                if (item.type != "startround" && item.type != "endround" && item.setInfo != undefined) {
                    var _popoverhtml = "";
                    _popoverhtml += '<div class="flow-portal-panel-title"><i class="fa fa-navicon"></i>&nbsp;&nbsp;基本信息</div>';
                    _popoverhtml += '<ul>';
                    _popoverhtml += '<li>节点标识:' + item.setInfo.nodeCode + '</li>';
                    _popoverhtml += '<li>驳回类型:' + _nodeRejectType[item.setInfo.nodeRejectType] + '</li>';
                    _popoverhtml += '<li>执行对象:' + item.setInfo.nodeSysFn + '</li>';
                    if (item.setInfo.nodeDes != "") { _popoverhtml += '<li>备注:' + item.setInfo.nodeDes + '</li>'; }
                    if (item.setInfo.nodeConfluenceType != undefined && item.setInfo.nodeConfluenceType != "") {
                        _popoverhtml += '<li>会签策略:' + _nodeConfluenceType[item.setInfo.nodeConfluenceType] + '</li>';
                        if (item.setInfo.nodeConfluenceType == 2) {
                            _popoverhtml += '<li>会签比例:' + item.setInfo.nodeConfluenceRate + '</li>';
                        }
                    }

                    _popoverhtml += '</ul>';

                    _popoverhtml += '<div class="flow-portal-panel-title"><i class="fa fa-navicon"></i>&nbsp;&nbsp;审核者</div>';
                    _popoverhtml += '<ul>';
                    _popoverhtml += '<li>类型:' + _nodeDesignate[item.setInfo.nodeDesignate] + '</li>';
                    if (item.setInfo.nodeDesignateData != undefined) {
                        var _rowstr = "";
                        for (var i in item.setInfo.nodeDesignateData.role) {
                            var _postitem = item.setInfo.nodeDesignateData.role[i];
                            _rowstr += ' <span class="label label-success">' + getInfoTop().learun.data.get(["role", _postitem, "FullName"]) + '</span>';
                            if (i == item.setInfo.nodeDesignateData.role.length - 1) {
                                _popoverhtml += '<li>角色:' + _rowstr + '</li>';
                            }
                        }

                        _rowstr = "";
                        for (var i in item.setInfo.nodeDesignateData.post) {
                            var _postitem = item.setInfo.nodeDesignateData.post[i];
                            _rowstr += ' <span class="label label-info">' + getInfoTop().learun.data.get(["post", _postitem, "FullName"]) + '</span>';
                            if (i == item.setInfo.nodeDesignateData.post.length - 1) {
                                _popoverhtml += '<li>岗位:' + _rowstr + '</li>';
                            }
                        }

                        _rowstr = "";
                        for (var i in item.setInfo.nodeDesignateData.usergroup) {
                            var _postitem = item.setInfo.nodeDesignateData.usergroup[i];
                            _rowstr += ' <span class="label label-warning">' + getInfoTop().learun.data.get(["userGroup", _postitem, "FullName"]) + '</span>';
                            if (i == item.setInfo.nodeDesignateData.usergroup.length - 1) {
                                _popoverhtml += '<li>用户组:' + _rowstr + '</li>';
                            }
                        }

                        _rowstr = "";
                        for (var i in item.setInfo.nodeDesignateData.user) {
                            var _postitem = item.setInfo.nodeDesignateData.user[i];
                            _rowstr += ' <span class="label label-danger">' + getInfoTop().learun.data.get(["user", _postitem, "FullName"]) + '</span>';
                            if (i == item.setInfo.nodeDesignateData.user.length - 1) {
                                _popoverhtml += '<li>用户:' + _rowstr + '</li>';
                            }
                        }
                    }
                    _popoverhtml += '</ul>';

                    var _row = "";
                    for (var i in item.setInfo.frmPermissionInfo) {
                        var _item = item.setInfo.frmPermissionInfo[i];
                        var _downtext = "";
                        if (_item.down) {
                            _downtext = ' | 可下载';
                        }
                        else if (_item.down != undefined) {
                            _downtext = ' | 不可下载';
                        }
                        _row += '<li>' + _frmitems[_item.fieldid] + ': ' + (_item.look ? '可查看' : '不可查看') + _downtext + '</li>';
                        if (i == item.setInfo.frmPermissionInfo.length - 1) {
                            _popoverhtml += '<div class="flow-portal-panel-title"><i class="fa fa-navicon"></i>&nbsp;&nbsp;权限分配</div>';
                            _popoverhtml += '<ul>';
                            _popoverhtml += _row;
                            _popoverhtml += '</ul>';
                        }
                    }

                    if (item.setInfo.nodeDbId != "" || item.setInfo.nodeSQL != "") {
                        _popoverhtml += '<div class="flow-portal-panel-title"><i class="fa fa-navicon"></i>&nbsp;&nbsp;执行SQL</div>';
                        _popoverhtml += '<ul>';
                        _popoverhtml += '<li>数据库:' + DataBaseLinkData[item.setInfo.nodeDbId] + '</li>';
                        _popoverhtml += '<li>SQL语句:' + item.setInfo.nodeSQL + '</li>';
                        _popoverhtml += '</ul>';
                    }

                    $flowDesigner.find('#' + item.id).attr('title', item.name);
                    $flowDesigner.find('#' + item.id).attr('data-toggle', 'popover');
                    $flowDesigner.find('#' + item.id).attr('data-placement', 'bottom');
                    $flowDesigner.find('#' + item.id).attr('data-content', _popoverhtml);
                }
            });
            $flowDesigner.find('.GooFlow_item').popover({ html: true });
        }

        function labellingRedNode(id) {
            $flowDesigner.find('.flow-labellingnode-red').removeClass('flow-labellingnode-red');
            $flowDesigner.find('#' + id).addClass('flow-labellingnode-red');
        }

        return flowDesigner;
    }
})(window.jQuery);


