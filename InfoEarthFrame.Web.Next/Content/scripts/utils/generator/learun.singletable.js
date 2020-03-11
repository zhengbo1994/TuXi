/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 快速开发(单表开发)
 * 陈小二
 */
//(function ($) {
//    "use strict";
//    var step = {
//        one: {//选择数据表
//            data:{},
//            init: function () {
//                $("#txt_DataBase").ComboBox({
//                    url: "../../SystemManage/DataBaseLink/GetListJson",
//                    id: "F_DatabaseLinkId",
//                    text: "F_DBAlias",
//                    height: "400px",
//                    description:"",
//                    allowSearch: true,
//                    selectOne:true
//                });
//                var $gridTable = $("#gridTable");
//                $gridTable.jqGrid({
//                    url: "../../SystemManage/DataBaseTable/GetTableListJson",
//                    postData: { dataBaseLinkId: $("#txt_DataBase").attr('data-value'), keyword: $("#txt_Keyword").val() },
//                    datatype: "json",
//                    height: $(window).height() - 197,
//                    autowidth: true,
//                    colModel: [
//                        { label: "表名", name: "f_name", width: 260, align: "left", sortable: false },
//                        { label: '主键', name: 'f_pk', width: 150, align: "left", sortable: false },
//                        {
//                            label: "记录数", name: "f_sumrows", width: 100, align: "left", sortable: false,
//                            formatter: function (cellvalue, options, rowObject) {
//                                return cellvalue + "条";
//                            }
//                        },
//                        { label: "使用大小", name: "f_reserved", width: 100, align: "left", sortable: false },
//                        { label: "更新时间", name: "f_updatetime", width: 120, align: "left", sortable: false },
//                        { label: "说明", name: "f_tdescription", width: 120, align: "left", sortable: false }
//                    ],
//                    rowNum: "1000",
//                    rownumbers: true,
//                    shrinkToFit: false,
//                    gridview: true,
//                    subGrid: true,
//                    subGridRowExpanded: function (subgrid_id, row_id) {
//                        var tableName = $gridTable.jqGrid('getRowData', row_id)['f_name'];
//                        var subgrid_table_id = subgrid_id + "_t";
//                        $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table>");
//                        $("#" + subgrid_table_id).jqGrid({
//                            url: "../../SystemManage/DataBaseTable/GetTableFiledListJson",
//                            postData: { dataBaseLinkId: $("#txt_DataBase").attr('data-value'), tableName: tableName },
//                            datatype: "json",
//                            height: "100%",
//                            colModel: [
//                                { label: "列名", name: "f_column", index: "f_column", width: 250, sortable: false },
//                                { label: "数据类型", name: "f_datatype", index: "f_datatype", width: 120, align: "center", sortable: false },
//                                { label: "长度", name: "f_length", index: "f_length", width: 57, align: "center", sortable: false },
//                                {
//                                    label: "允许空", name: "f_isnullable", index: "f_isnullable", width: 58, align: "center", sortable: false,
//                                    formatter: function (cellvalue, options, rowObject) {
//                                        return cellvalue == 1 ? "<i class=\"fa fa-check-square-o\"></i>" : "<i class=\"fa fa-square-o\"></i>";
//                                    }
//                                },
//                                {
//                                    label: "标识", name: "f_identity", index: "f_identity", width: 58, align: "center", sortable: false,
//                                    formatter: function (cellvalue, options, rowObject) {
//                                        return cellvalue == 1 ? "<i class=\"fa fa-check-square-o\"></i>" : "<i class=\"fa fa-square-o\"></i>";
//                                    }
//                                },
//                                {
//                                    label: "主键", name: "f_key", index: "f_key", width: 57, align: "center", sortable: false,
//                                    formatter: function (cellvalue, options, rowObject) {
//                                        return cellvalue == 1 ? "<i class=\"fa fa-check-square-o\"></i>" : "<i class=\"fa fa-square-o\"></i>";
//                                    }
//                                },
//                                { label: "默认值", name: "f_default", index: "f_default", width: 120, align: "center", sortable: false },
//                                { label: "说明", name: "f_remark", index: "f_remark", width: 100, sortable: false }
//                            ],
//                            caption: "表字段信息",
//                            rowNum: "1000",
//                            rownumbers: true,
//                            shrinkToFit: false,
//                            gridview: true,
//                            hidegrid: false
//                        });
//                    }
//                });
//                //查询事件
//                $("#btn_Search").click(function () {
//                    $gridTable.jqGrid('setGridParam', {
//                        url: "../../SystemManage/DataBaseTable/GetTableListJson",
//                        postData: { dataBaseLinkId: $("#txt_DataBase").attr('data-value'), keyword: $("#txt_Keyword").val() },
//                    }).trigger('reloadGrid');
//                });
//            },
//            bind: function () {
//                step.one.data.tablePk = $("#gridTable").jqGridRowValue("f_pk");//主键
//                step.one.data.tableName = $("#gridTable").jqGridRowValue("f_name");//表名
//                step.one.data.tableDescription = $("#gridTable").jqGridRowValue("f_tdescription");//表的说明
//                step.one.data.dataBaseLinkId = $("#txt_DataBase").attr('data-value');
//                if (!step.one.data.tablePk) {
//                    learun.dialogTop({ msg: "请选择数据表", type: "error" });
//                    return false;
//                } else {
//                    step.two.init();
//                }
//                return true;
//            }
//        },
//        two: {//基本配置
//            init: function (isFirst) {
//                if (isFirst) {
//                    $('#OutputAreas').comboBox({
//                        url: "../../SystemManage/DataItemDetail/GetDataItemListJson",
//                        param: { EnCode: "AreaName" },
//                        id: "F_ItemValue",
//                        text: "F_ItemName",
//                        maxHeight: "200px",
//                        allowSearch: true
//                    });
//                }
//                else {
//                    $("#DataBaseTablePK").comboBox({
//                        url: "../../SystemManage/DataBaseTable/GetTableFiledListJson",
//                        param: { dataBaseLinkId: step.one.data.dataBaseLinkId, tableName: step.one.data.tableName },
//                        id: "f_column",
//                        text: "f_column",
//                        maxHeight: "350px"
//                    }).ComboBoxSetValue(step.one.data.tablePk);
//                    var name = step.one.data.tableName;
//                    $("#Description").val(step.one.data.tableDescription);
//                    $("#EntityClassName").val(name + "Entity").attr('tableName', name);
//                    $("#MapClassName").val(name + "Map");
//                    $("#ServiceClassName").val(name + "Service");
//                    $("#IServiceClassName").val(name + "IService");
//                    $("#BusinesClassName").val(name + "BLL");
//                    $("#ControllerName").val(name + "Controller");
//                    $("#IndexPageName").val(name + "Index");
//                    $("#FormPageName").val(name + "Form");
//                }
//            },
//            bind: function () {
//                //验证初始化配置表单
//                if (!$('#step-2').Validform()) {
//                    return false;
//                }
//                else {
//                    return true;
//                }
//            }
//        },
//        three: {//列表页面
//            data:{},
//            init: function () {
//                $(".editview").hover(function () {
//                    $(this).find('.editviewtitle').show();
//                }, function (e) {
//                    $(this).find('.editviewtitle').hide();
//                });
//                $(".sys_spec_text li").click(function () {
//                    if (!!$(this).hasClass("active")) {
//                        $(this).removeClass("active");
//                    } else {
//                        $(this).addClass("active").siblings("li");
//                    }
//                });
//                $("#editgrid").click(function () {//编辑列表
//                    dialogOpen({
//                        id: "Member",
//                        title: '绑定表格数据',
//                        url: '/GeneratorManage/SingleTable/EditGrid?dataBaseLinkId=' + step.one.data.dataBaseLinkId + '&tableName=' + step.one.data.tableName,
//                        width: "800px",
//                        height: "520px",
//                        callBack: function (iframeId) {
//                            top.frames[iframeId].AcceptClick(function (dataJson) {
//                                step.one.data.jqGirdDataJson = dataJson;
//                                $('.editviewbody').html("<table id='Binding_gridTable'></table><div id='Binding_gridPager'></div>");
//                                $("#Binding_gridTable").jqGrid({
//                                    unwritten: false,
//                                    datatype: "json",
//                                    height: 300,
//                                    autowidth: true,
//                                    colModel: dataJson.colModel,
//                                    viewrecords: true,
//                                    rowNum: 30,
//                                    rowList: [30, 50, 100],
//                                    pager: (dataJson.pager != true ? true : "#Binding_gridPager"),
//                                    sortname: '',
//                                    rownumbers: true,
//                                    shrinkToFit: false,
//                                    gridview: true
//                                });
//                            });
//                        }
//                    });
//                });
//                $('#btn_SetSearch').click(function () {//编辑查询方法
//                    dialogOpen({
//                        id: "EditSearch",
//                        title: '查询条件设置',
//                        url: '/GeneratorManage/SingleTable/EditSearch?dataBaseLinkId=' + step.one.data.dataBaseLinkId + '&tableName=' + step.one.data.tableName,
//                        width: "800px",
//                        height: "520px",
//                        callBack: function (iframeId) {
//                            top.frames[iframeId].AcceptClick(function (dataJson) {
//                                step.one.data.jqGirdDataJson = dataJson;
//                                $('.editviewbody').html("<table id='Binding_gridTable'></table><div id='Binding_gridPager'></div>");
//                                $("#Binding_gridTable").jqGrid({
//                                    unwritten: false,
//                                    datatype: "json",
//                                    height: 300,
//                                    autowidth: true,
//                                    colModel: dataJson.colModel,
//                                    viewrecords: true,
//                                    rowNum: 30,
//                                    rowList: [30, 50, 100],
//                                    pager: (dataJson.pager != true ? true : "#Binding_gridPager"),
//                                    sortname: '',
//                                    rownumbers: true,
//                                    shrinkToFit: false,
//                                    gridview: true
//                                });
//                            });
//                        }
//                    });
//                });
//            },
//            bind: function () {

//            }
//        },
//        four: {//表单页面
//            init: function () {

//            },
//            bind: function () {

//            }
//        },
//        five: {//代码查看
//            init: function () {

//            },
//            bind: function () {

//            }
//        },
//        six: {//自动创建
//            init: function () {

//            },
//            bind: function () {

//            }
//        }
//    };
//    var singleTable = {
//        initialPage: function () {//初始化页面元素
//            $("#step-2").css("overflow-y", "auto").height($(window).height() - 94);
//            $("#step-3").css("overflow-y", "auto").height($(window).height() - 85);
//            $(".editviewbodybackground").width($(window).width() - 40).height($(window).height() - 160);
//            $("#showCodeAreas").height($(window).height() - 131);

//            //加载导向
//            $('#wizard').wizard().on('change', function (e, data) {
//                var $finish = $("#btn_finish");
//                var $next = $("#btn_next");
//                if (data.direction == "next") {
//                    switch (data.step) {
//                        case 1:
//                            return step.one.bind();
//                            break;
//                        case 2:
//                            return step.two.bind();
//                            break;
//                        case 3:
//                            BindingForm();
//                            break;
//                        case 4:
//                            LookCode()
//                            break;
//                        case 5:
//                            CreateCode()
//                            $finish.removeAttr('disabled');
//                            $next.attr('disabled', 'disabled');
//                            //发布功能事件
//                            $("#publish_btn").click(function () {
//                                $(this).attr('moduleForm', '1');
//                                $("#publish_panel").show().animate({ top: 50, speed: 2000 });
//                                $("#ModuleEnCode").val($("#EntityClassName").attr('tableName'));
//                                $("#ModuleFullName").val($("#Description").val());
//                                return false;
//                            });
//                            //选取功能图标
//                            $("#ModuleIcon").next('.input-button').click(function () {
//                                dialogOpen({
//                                    id: "SelectIcon",
//                                    title: '选取图标',
//                                    url: '/AuthorizeManage/Module/Icon?ControlId=ModuleIcon',
//                                    width: "1000px",
//                                    height: "600px",
//                                    btn: false
//                                })
//                            })
//                            break;
//                        default:
//                            break;
//                    }
//                } else {
//                    $finish.attr('disabled', 'disabled');
//                    $next.removeAttr('disabled');
//                }
//            });
//            step.one.init();
//            step.two.init(true);
//            step.three.init();
//        }
//    };

//    $(function () {
//        singleTable.initialPage();
//    });
//})(window.jQuery);

$(function () {
    initialPage();
    GetDataSource();
})
//初始化页面
function initialPage() {
    $("#step-2").css("overflow-y", "auto").height($(window).height() - 95);
    $("#step-3").css("overflow-y", "auto").height($(window).height() - 85);
    $(".editviewbodybackground").width($(window).width() - 40).height($(window).height() - 160);
    //所在区域
    $("#OutputAreas").ComboBox({
        description: "==请选择==",
        height: "200px"
    }).ComboBoxSetValue('BaseManage');
    //功能上级
    $("#ModuleParentId").ComboBoxTree({
        url: "../../AuthorizeManage/Module/GetTreeJson",
        description: "==请选择==",
        height: "300px",
        allowSearch: true
    });
    //加载导向
    $('#wizard').wizard().on('change', function (e, data) {
        var $finish = $("#btn_finish");
        var $next = $("#btn_next");
        if (data.direction == "next") {
            switch (data.step) {
                case 1:
                    var pk = $("#gridTable").jqGridRowValue("f_pk");
                    var name = $("#gridTable").jqGridRowValue("f_name");
                    var tdescription = $("#gridTable").jqGridRowValue("f_tdescription");
                    if (!pk) {
                        return false;
                    } else {
                        $("#DataBaseTablePK").ComboBox({
                            url: "../../SystemManage/DataBaseTable/GetTableFiledListJson",
                            param: { dataBaseLinkId: $("#txt_DataBase").val(), tableName: name },
                            id: "f_column",
                            text: "f_column",
                            height: "350px"
                        }).ComboBoxSetValue(pk);
                        $("#Description").val(tdescription);
                        $("#EntityClassName").val(name + "Entity").attr('tableName', name);
                        $("#MapClassName").val(name + "Map");
                        $("#ServiceClassName").val(name + "Service");
                        $("#IServiceClassName").val(name + "IService");
                        $("#BusinesClassName").val(name + "BLL");
                        $("#ControllerName").val(name + "Controller");
                        $("#IndexPageName").val(name + "Index");
                        $("#FormPageName").val(name + "Form");
                    }
                    break;
                case 2://绑定列表
                    //验证初始化配置表单
                    if (!$('#step-2').Validform()) {
                        return false;
                    }
                    BindingIndex();
                    break;
                case 3://绑定表单
                    BindingForm();
                    break;
                case 4://查看代码
                    LookCode()
                    break;
                case 5://创建代码
                    CreateCode()
                    $finish.removeAttr('disabled');
                    $next.attr('disabled', 'disabled');
                    //发布功能事件
                    $("#publish_btn").click(function () {
                        $(this).attr('moduleForm', '1');
                        $("#publish_panel").show().animate({ top: 50, speed: 2000 });
                        $("#ModuleEnCode").val($("#EntityClassName").attr('tableName'));
                        $("#ModuleFullName").val($("#Description").val());
                        return false;
                    });
                    //选取功能图标
                    $("#ModuleIcon").next('.input-button').click(function () {
                        dialogOpen({
                            id: "SelectIcon",
                            title: '选取图标',
                            url: '/AuthorizeManage/Module/Icon?ControlId=ModuleIcon',
                            width: "1000px",
                            height: "600px",
                            btn: false
                        })
                    })
                    break;
                default:
                    break;
            }
        } else {
            $finish.attr('disabled', 'disabled');
            $next.removeAttr('disabled');
        }
    });
    //完成
    $("#btn_finish").click(function () {
        if ($("#publish_btn").attr('moduleForm') == 1) {
            //自动创建功能菜单
            if (!$('#publish_panel').Validform()) {
                return false;
            }
            var baseConfig = $("#step-2").GetWebControls();
            baseConfig["DataBaseLinkId"] = $("#txt_DataBase").val();
            baseConfig["DataBaseTableName"] = $("#gridTable").jqGridRowValue("f_name");
            var postData = {
                EnCode: $("#ModuleEnCode").val(),
                FullName: $("#ModuleFullName").val(),
                ParentId: $("#ModuleParentId").attr('data-value'),
                Icon: $("#ModuleIcon").val(),
                Description: $("#ModuleDescription").val(),
                baseInfoJson: JSON.stringify(baseConfig)
            }
            $.SaveForm({
                url: "../../GeneratorManage/SingleTable/PublishModule",
                param: postData,
                loading: "正在发布功能...",
                success: function () {
                    dialogClose();
                }
            })
        } else {
            dialogClose();
        }
    });
}
/*=========选择数据源（begin）================================================================*/
function GetDataSource() {
    $.ajax({
        url: "../../SystemManage/DataBaseLink/GetListJson",
        type: "get",
        dataType: "json",
        async: false,
        success: function (data) {
            $.each(data, function (i) {
                $("#txt_DataBase").append($("<option title='" + data[i].F_DBName + "'></option>").val(data[i].F_DatabaseLinkId).html(data[i].F_DBAlias));
            });
        }
    });
    var $gridTable = $("#gridTable");
    $gridTable.jqGrid({
        url: "../../SystemManage/DataBaseTable/GetTableListJson",
        postData: { dataBaseLinkId: $("#txt_DataBase").val(), keyword: $("#txt_Keyword").val() },
        datatype: "json",
        height: $(window).height() - 255,
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
                postData: { dataBaseLinkId: $("#txt_DataBase").val(), tableName: tableName },
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
            postData: { dataBaseLinkId: $("#txt_DataBase").val(), keyword: $("#txt_Keyword").val() },
        }).trigger('reloadGrid');
    });
}
/*=========选择数据源（end）==================================================================*/

/*=========绑定表格（begin）==================================================================*/
var bindingTableJson = [];
function BindingIndex() {
    $(".editview").hover(function () {
        $(this).find('.editviewtitle').show();
    }, function (e) {
        $(this).find('.editviewtitle').hide();
    });
    $(".sys_spec_text li").click(function () {
        if (!!$(this).hasClass("active")) {
            $(this).removeClass("active");
        } else {
            $(this).addClass("active").siblings("li");
        }
    });
    $("#editgrid").click(function () {
        var dataBaseLinkId = $("#txt_DataBase").val();
        var tableName = $("#gridTable").jqGridRowValue("f_name");
        dialogOpen({
            id: "Member",
            title: '绑定表格数据',
            url: '/GeneratorManage/SingleTable/EditGrid?dataBaseLinkId=' + dataBaseLinkId + '&tableName=' + tableName,
            width: "800px",
            height: "520px",
            callBack: function (iframeId) {
                getInfoTop().frames[iframeId].AcceptClick(function (dataJson) {
                    bindingTableJson = dataJson;
                    $('.editviewbody').html("<table id='Binding_gridTable'></table><div id='Binding_gridPager'></div>");
                    $("#Binding_gridTable").jqGrid({
                        unwritten: false,
                        datatype: "json",
                        height: 300,
                        autowidth: true,
                        colModel: dataJson.colModel,
                        viewrecords: true,
                        rowNum: 30,
                        rowList: [30, 50, 100],
                        pager: (dataJson.pager != true ? true : "#Binding_gridPager"),
                        sortname: '',
                        rownumbers: true,
                        shrinkToFit: false,
                        gridview: true
                    });
                });
            }
        });
    })
}
/*=========绑定表格（end）====================================================================*/

/*=========绑定表单（begin）==================================================================*/
var bindingFormJson = [];
function BindingForm() {
    var dataBaseLinkId = $("#txt_DataBase").val();
    var tableName = $("#gridTable").jqGridRowValue("f_name");
    var nameArray = [];
    if (bindingFormJson) {
        $.each(bindingFormJson, function (i) {
            nameArray.push(bindingFormJson[i].ControlId)
        });
    }
    $("#FormFieldTree").treeview({
        height: 572,
        showcheck: true,
        url: "../../SystemManage/DataBaseTable/GetTableFiledTreeJson",
        param: { dataBaseLinkId: dataBaseLinkId, tableName: tableName, nameId: String(nameArray) },
        oncheckboxclick: function (item, status) {
            if (status == 1) {
                bindingFormJson.push({
                    ControlName: item.value,
                    ControlId: item.id,
                    ControlValidator: '',
                    ControlType: 'input'
                })
                //添加字段
                var $item = $('<div class="item_row" data-value="' + item.id + '"><div class="item_field_label"><span>' + item.value + '</span></div><div class="item_field_value"></div><div class="editviewtitle">编辑控件</div></div>');
                $("#Form_layout .item_table").append($item);
                $item.find('.editviewtitle').click(function () {
                    var Id = $item.attr('data-value');
                    btn_editcontrol(Id);
                })
                $item.hover(function () {
                    $(this).find('.editviewtitle').show();
                }, function (e) {
                    $(this).find('.editviewtitle').hide();
                });
            } else if (status == 0) {
                $.each(bindingFormJson, function (i) {
                    if (bindingFormJson[i].ControlId == item.id) {
                        bindingFormJson.splice(i, 1);
                        return false;
                    }
                });
                //删除字段
                $("#Form_layout .item_table").find('[data-value=' + item.id + ']').remove();
            }
        }
    });
    //表单控件可以拖动
    $("#Form_layout_list").sortable({
        handle: '.item_field_label',
        placeholder: "ui-state-highlight"
    });
    //点击切换表单类型（一排、二排）
    $("#FormshowType").find('a').click(function () {
        $("#FormshowType").find('a').removeClass('active')
        if ($(this).attr('id') == "FormType1") {
            $(this).addClass('active')
            $(".app_layout .item_row").css({ width: "100%", float: "left" })
        } else if ($(this).attr('id') == "FormType2") {
            $(this).addClass('active');
            $(".app_layout .item_row").css({ width: "50%", float: "left" })
        }
    });
    //编辑控件
    function btn_editcontrol(Id) {
        dialogOpen({
            id: "EditControl",
            title: '编辑控件',
            url: '/GeneratorManage/SingleTable/EditControl?controlId=' + Id,
            width: "450px",
            height: "360px",
            callBack: function (iframeId) {
                getInfoTop().frames[iframeId].AcceptClick(function (data) {
                    if (data.ControlColspan == 1) {
                        $("#Form_layout .item_table").find('[data-value=' + Id + ']').css({ width: "100%", float: "left" })
                    } else {
                        $("#Form_layout .item_table").find('[data-value=' + Id + ']').css({ width: "50%", float: "left" })
                    }
                    $("#Form_layout .item_table").find('[data-value=' + Id + ']').find('.item_field_label span').html(data.ControlName);
                    $.each(bindingFormJson, function (i) {
                        if (bindingFormJson[i].ControlId == Id) {
                            bindingFormJson[i] = data;
                            return false;
                        }
                    });
                });
            }
        });
    }
}
/*=========绑定表单（end）====================================================================*/

/*=========查看代码（begin）==================================================================*/
var LookCodeJson = [];
function LookCode() {
    $.learunUIBase.loading({ isShow: true, text: "正在生成代码..." });
    window.setTimeout(function () {
        var baseConfig = $("#step-2").GetWebControls();
        baseConfig["DataBaseLinkId"] = $("#txt_DataBase").val();
        baseConfig["DataBaseTableName"] = $("#gridTable").jqGridRowValue("f_name");
        var gridInfoJson = {
            IsPage: bindingTableJson.pager,
        }
        var formJson = {
            width: $("#Formwidth").val(),
            height: $("#Formheight").val(),
            FormType: $("#FormshowType .active").attr('data-value')
        }
        $.ajax({
            url: "../../GeneratorManage/SingleTable/LookCode",
            data: {
                baseInfoJson: JSON.stringify(baseConfig),
                gridInfoJson: JSON.stringify(gridInfoJson),
                gridColumnJson: JSON.stringify(bindingTableJson.colModel),
                formInfoJson: JSON.stringify(formJson),
                formFieldJson: JSON.stringify(bindingFormJson),
            },
            type: "post",
            dataType: "json",
            async: false,
            success: function (data) {
                var dataJons = data;
                LookCodeJson = data;
                $('#showCodeAreas').html('<textarea name="SyntaxHighlighter" class="brush: c-sharp;">' + dataJons["entityCode"] + '</textarea>');
                SyntaxHighlighter.highlight();
                $("#step-5 .nav-tabs li").click(function () {
                    var id = $(this).attr('id');
                    $('#showCodeAreas').html('<textarea name="SyntaxHighlighter" class="brush: c-sharp;">' + dataJons[id.substring(4)] + '</textarea>');
                    SyntaxHighlighter.highlight();
                })
            },
            complete: function () {
                $.learunUIBase.loading({ isShow: false });
            }
        });
    }, 500);
}
/*=========查看代码（end）====================================================================*/

/*=========自动创建（begin）==================================================================*/
function CreateCode() {
    $.learunUIBase.loading({ isShow: true, text: "正在创建代码..." });
    window.setTimeout(function () {
        var baseConfig = $("#step-2").GetWebControls();
        $.ajax({
            url: "../../GeneratorManage/SingleTable/CreateCode",
            data: { baseInfoJson: JSON.stringify(baseConfig), strCode: encodeURIComponent(JSON.stringify(LookCodeJson)) },
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
                $.learunUIBase.loading({ isShow: false });
            }
        });
    }, 500);
}
/*=========自动创建（end）====================================================================*/