/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 快速开发(后台服务类快速开发)
 * 陈小二
 */
(function ($) {
    "use strict";
    var step = {
        one: {//选择数据表
            data: {},
            init: function () {
                $("#txt_DataBase").ComboBox({
                    url: "../../SystemManage/DataBaseLink/GetListJson",
                    id: "F_DatabaseLinkId",
                    text: "F_DBAlias",
                    height: "400px",
                    description: "",
                    allowSearch: true,
                    selectOne: true
                });
                var $gridTable = $("#gridTable");
                $gridTable.jqGrid({
                    url: "../../SystemManage/DataBaseTable/GetViewListJson",
                    postData: { dataBaseLinkId: $("#txt_DataBase").attr('data-value'), keyword: $("#txt_Keyword").val() },
                    datatype: "json",
                    height: $(window).height() - 197,
                    autowidth: true,
                    colModel: [
                        { label: "表名", name: "f_name", width: 260, align: "left", sortable: false },
                        { label: '主键', name: 'f_pk', width: 150, align: "left", sortable: false },
                        {
                            label: "记录数", name: "f_sumrows", width: 100, align: "left", sortable: false,
                            formatter: function (cellvalue, options, rowObject) {
                                return cellvalue + "条";
                            }
                        },
                        { label: "使用大小", name: "f_reserved", width: 100, align: "left", sortable: false },
                        { label: "更新时间", name: "f_updatetime", width: 120, align: "left", sortable: false },
                        { label: "说明", name: "f_tdescription", width: 120, align: "left", sortable: false }
                    ],
                    multiselect: true,
                    rowNum: "1000",
                    rownumbers: true,
                    shrinkToFit: false,
                    gridview: true,
                    subGrid: true,
                    subGridRowExpanded: function (subgrid_id, row_id) {
                        var tableName = $gridTable.jqGrid('getRowData', row_id)['f_name'];
                        var subgrid_table_id = subgrid_id + "_t";
                        $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table>");
                        $("#" + subgrid_table_id).jqGrid({
                            url: "../../SystemManage/DataBaseTable/GetTableFiledListJson",
                            postData: { dataBaseLinkId: $("#txt_DataBase").attr('data-value'), tableName: tableName },
                            datatype: "json",
                            height: "100%",
                            colModel: [
                                { label: "列名", name: "f_column", index: "f_column", width: 250, sortable: false },
                                { label: "数据类型", name: "f_datatype", index: "f_datatype", width: 120, align: "center", sortable: false },
                                { label: "长度", name: "f_length", index: "f_length", width: 57, align: "center", sortable: false },
                                {
                                    label: "允许空", name: "f_isnullable", index: "f_isnullable", width: 58, align: "center", sortable: false,
                                    formatter: function (cellvalue, options, rowObject) {
                                        return cellvalue == 1 ? "<i class=\"fa fa-check-square-o\"></i>" : "<i class=\"fa fa-square-o\"></i>";
                                    }
                                },
                                {
                                    label: "标识", name: "f_identity", index: "f_identity", width: 58, align: "center", sortable: false,
                                    formatter: function (cellvalue, options, rowObject) {
                                        return cellvalue == 1 ? "<i class=\"fa fa-check-square-o\"></i>" : "<i class=\"fa fa-square-o\"></i>";
                                    }
                                },
                                {
                                    label: "主键", name: "f_key", index: "f_key", width: 57, align: "center", sortable: false,
                                    formatter: function (cellvalue, options, rowObject) {
                                        return cellvalue == 1 ? "<i class=\"fa fa-check-square-o\"></i>" : "<i class=\"fa fa-square-o\"></i>";
                                    }
                                },
                                { label: "默认值", name: "f_default", index: "f_default", width: 120, align: "center", sortable: false },
                                { label: "说明", name: "f_remark", index: "f_remark", width: 100, sortable: false }
                            ],
                            caption: "表字段信息",
                            rowNum: "1000",
                            rownumbers: true,
                            shrinkToFit: false,
                            gridview: true,
                            hidegrid: false
                        });
                    }
                });
                //查询事件
                $("#btn_Search").click(function () {
                    $gridTable.jqGrid('setGridParam', {
                        url: "../../SystemManage/DataBaseTable/GetTableListJson",
                        postData: { dataBaseLinkId: $("#txt_DataBase").attr('data-value'), keyword: $("#txt_Keyword").val() },
                    }).trigger('reloadGrid');
                });
            },
            bind: function () {
                step.one.data.tablePk = $("#gridTable").jqGridRowValue("f_pk");//主键
                step.one.data.tableName = $("#gridTable").jqGridRowValue("f_name");//表名
                step.one.data.tableDescription = $("#gridTable").jqGridRowValue("f_tdescription");//表的说明
                step.one.data.dataBaseLinkId = $("#txt_DataBase").attr('data-value');
                if (!step.one.data.tablePk) {
                    learun.dialogTop({ msg: "请选择数据表", type: "error" });
                    return false;
                } else {
                    step.two.init();
                }
                return true;
            }
        },
        two: {//基本配置
            init: function (isFirst) {
                if (isFirst) {
                    $('#OutputAreas').comboBox({
                        url: "../../SystemManage/DataItemDetail/GetDataItemListJson",
                        param: { EnCode: "AreaName" },
                        id: "F_ItemValue",
                        text: "F_ItemName",
                        maxHeight: "200px",
                        description: "",
                        selectOne: true,
                        allowSearch: true
                    });
                }
                else {
                    var $tableList = $('#tableList');
                    var $tableList2 = $('#tableList2');
                    var tbalelist = step.one.data.tableName.split(",");
                    var tablePxList = step.one.data.tablePk.split(",");
                    var tableDescriptionList = step.one.data.tableDescription.split(",");
                    var _html = "";
                    $.each(tbalelist, function (i, tableName) {
                        var tablePk = tablePxList[i];
                        if (i == 0) {
                            step.two.set(tableName, tablePk, tableDescriptionList[i]);
                            _html += '<li data-value="' + i + '" class="active tableListOne"><i class="fa fa-file-text-o"></i>' + tableName + '</li>';
                        }
                        else {
                            _html += '<li data-value="' + i + '" class="tableListOne"><i class="fa fa-file-text-o"></i>' + tableName + '</li>';
                        }
                    });
                    $tableList.html(_html);
                    $tableList.find("li").unbind();
                    $tableList.find("li").on('click', function () {
                        $(".profile-nav li").removeClass("active");
                        $(".profile-nav li").removeClass("hover");
                        $(this).addClass("active");
                        var i = $(this).attr('data-value');
                        step.two.set(tbalelist[i], tablePxList[i], tableDescriptionList[i]);
                    }).hover(function () {
                        if (!$(this).hasClass("active")) {
                            $(this).addClass("hover");
                        }
                    }, function () {
                        $(this).removeClass("hover");
                    });
                    $tableList2.html(_html);
                    $tableList2.find("li").unbind();
                    $tableList2.find("li").on('click', function () {
                        $(".profile-nav li").removeClass("active");
                        $(".profile-nav li").removeClass("hover");
                        $(this).addClass("active");
                        var i = $(this).attr('data-value');
                        step.three.lookCode(i);
                    }).hover(function () {
                        if (!$(this).hasClass("active")) {
                            $(this).addClass("hover");
                        }
                    }, function () {
                        $(this).removeClass("hover");
                    });
                }
            },
            set: function (tableName, tablePk, tableDescription) {
                $("#Description").val(tableDescription);
                $("#DataBaseTablePK").val(tablePk);

                $("#EntityClassName").val(tableName + "Entity").attr('tableName', tableName);
                $("#MapClassName").val(tableName + "Map");
                $("#ServiceClassName").val(tableName + "Service");
                $("#IServiceClassName").val(tableName + "IService");
                $("#BusinesClassName").val(tableName + "BLL");
            },
            bind: function () {
                //验证初始化配置表单
                if (!$('#step-2').Validform()) {
                    return false;
                }
                else {
                    step.three.lookCode(0);
                    return true;
                }
            }
        },
        three: {//代码查看
            lookCode: function (i) {
                learun.loading({ isShow: true, text: "正在生成代码..." });
                window.setTimeout(function () {
                    var tableName = step.one.data.tableName.split(',')[i];
                    var postData = $('#baseconfig').getWebControls();
                    postData.DataBaseLinkId = step.one.data.dataBaseLinkId;
                    postData.DataBaseTableName = tableName;
                    postData.DataBaseTablePK = step.one.data.tablePk.split(',')[i];
                    postData.Description = step.one.data.tableDescription.split(',')[i];
                    postData.CreateUser = $('#CreateUser').val();
                    postData.CreateDate = $('#CreateDate').val();

                    postData.EntityClassName = tableName + "Entity";
                    postData.MapClassName = tableName + "Map";
                    postData.ServiceClassName = tableName + "Service";
                    postData.IServiceClassName = tableName + "IService";
                    postData.BusinesClassName = tableName + "BLL";

                    $.ajax({
                        url: "../../GeneratorManage/ServiceCodeForView/LookCode",
                        data: postData,
                        type: "post",
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            var id = $("#step-3 .nav-tabs li.active").attr('id');
                            $('#showCodeAreas').html('<textarea name="SyntaxHighlighter" class="brush: c-sharp;">' + data[id.substring(4)] + '</textarea>');
                            SyntaxHighlighter.highlight();
                            $("#step-3 .nav-tabs li").unbind();
                            $("#step-3 .nav-tabs li").click(function () {
                                var id = $(this).attr('id');
                                $('#showCodeAreas').html('<textarea name="SyntaxHighlighter" class="brush: c-sharp;">' + data[id.substring(4)] + '</textarea>');
                                SyntaxHighlighter.highlight();
                            });
                        },
                        complete: function () {
                            learun.loading({ isShow: false });
                        }
                    });

                }, 500);
            }
        },
        four: {//表单页面
            createCode: function () {
                learun.loading({ isShow: true, text: "正在创建代码..." });
                window.setTimeout(function () {
                    var postData = $('#baseconfig').getWebControls();
                    postData.DataBaseLinkId = step.one.data.dataBaseLinkId;
                    postData.DataBaseTableName = step.one.data.tableName;
                    postData.DataBaseTablePK = step.one.data.tablePk;
                    postData.Description = step.one.data.tableDescription;
                    postData.CreateUser = $('#CreateUser').val();
                    postData.CreateDate = $('#CreateDate').val();

                    $.ajax({
                        url: "../../GeneratorManage/ServiceCodeForView/CreateCode",
                        data: postData,
                        type: "post",
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            $(".drag-tip").show();
                            if (data.type == 1) {
                                $("#finish-msg").html(data.message).css("color", "#0FA74F");
                                $("#finish-msg").prev('i').attr('class', 'fa fa-check-circle').css("color", "#0FA74F");
                                $("#finish-msg").next('p').show();
                            } else {
                                $("#finish-msg").html(data.message).css("color", "#d9534f");
                                $("#finish-msg").prev('i').attr('class', 'fa fa-times-circle').css("color", "#d9534f");
                                $("#finish-msg").next('p').hide();
                            }
                        },
                        complete: function () {
                            learun.loading({ isShow: false });
                        }
                    });
                }, 500);
            }
        }
    };
    var singleTable = {
        initialPage: function () {//初始化页面元素
            $("#step-2 > div").css("overflow-y", "auto").height($(window).height() - 84);
            $("#tableList").css("overflow-y", "auto").height($(window).height() - 137);
            $("#tableList2").css("overflow-y", "auto").height($(window).height() - 137);
            $("#showCodeAreas").height($(window).height() - 131);

            //加载导向
            $('#wizard').wizard().on('change', function (e, data) {
                var $finish = $("#btn_finish");
                var $next = $("#btn_next");
                if (data.direction == "next") {
                    switch (data.step) {
                        case 1:
                            return step.one.bind();
                            break;
                        case 2:
                            return step.two.bind();
                            break;
                        case 3:
                            step.four.createCode();
                            $finish.removeAttr('disabled');
                            $next.attr('disabled', 'disabled');
                            break;                            
                        default:
                            break;
                    }
                } else {
                    $finish.attr('disabled', 'disabled');
                    $next.removeAttr('disabled');
                }
            });
            $("#btn_finish").click(function () {
                dialogClose();
            });
            step.one.init();
            step.two.init(true);
        }
    };
    $(function () {
        singleTable.initialPage();
    });
})(window.jQuery);