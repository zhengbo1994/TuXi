/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * excel导入模板配置
 * 陈小二
 */
(function ($, learun) {
    "use strict";
    var keyValue = request('keyValue');
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
                }).bind("change", function () {
                    $gridTable.jqGrid('setGridParam', {
                        url: "../../SystemManage/DataBaseTable/GetTableListJson",
                        postData: { dataBaseLinkId: $("#txt_DataBase").attr('data-value'), keyword: $("#txt_Keyword").val() },
                    }).trigger('reloadGrid');
                });
                var $gridTable = $("#gridTable");
                $gridTable.jqGrid({
                    url: "../../SystemManage/DataBaseTable/GetTableListJson",
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
                    $("#gridTableFlied").jqGrid('clearGridData');
                    step.two.init();
                }
                return true;
            }
        },
        two: {//模板配置
            init: function (isFirst, filedsInfo) {
                if (isFirst) {
                    //绑定功能
                    $("#F_ModuleId").comboBoxTree({
                        url: "../../AuthorizeManage/Module/GetTreeJson",
                        description: "==请选择==",
                        maxHeight: "300px",
                        allowSearch: true,
                        click: function (item) {
                            if (item.F_Target == "iframe") {
                                $(".tip_container").remove();
                                //绑定按钮
                                $("#F_ModuleBtnId").comboBox({
                                    url: "../../AuthorizeManage/ModuleButton/GetTreeListJson?moduleId=" + item.id,
                                    id: "F_ModuleButtonId",
                                    text: "F_FullName",
                                    maxHeight: "300px",
                                    allowSearch: true,
                                    dataName: "rows"
                                });
                            }
                            else {
                                learun.dialogTop({ msg: "请选择功能页面", type: "error" });
                                return "false";
                            }
                        }
                    });
                    //绑定按钮
                    $("#F_ModuleBtnId").comboBox({});
                    //错误处理机制
                    $("#F_ErrorType").comboBox({
                        data: [{ "id": 1, "text": "跳过" }, { "id": 0, "text": "终止" }],
                        id: "id",
                        text: "text",
                        selectOne: true,
                        description: "",
                    });
                    step.two.initJqGird();
                }
                else {
                    var nameArray = [];
                    learun.getDataForm({
                        type: "get",
                        url: "../../SystemManage/DataBaseTable/GetTableFiledTreeJson",
                        param: { dataBaseLinkId: step.one.data.dataBaseLinkId, tableName: step.one.data.tableName, nameId: String(nameArray) },
                        async: true,
                        success: function (data) {
                            $("#FormFieldTree").treeview({
                                height: $(window).height() -87,
                                showcheck: true,
                                data: data,
                                oncheckboxclick: function (item, status) {
                                    if (status == 1) {
                                        var rowdata = {
                                            F_FliedName: '<input name="F_FliedName" value="' + item.id + '" type="text" />',

                                            F_RelationType: '<input name="F_RelationType" value="0" type="text" />',
                                            F_DataItemEncode: '<input name="F_DataItemEncode" type="text" />',
                                            F_Value: '<input name="F_Value" type="text" />',
                                            F_DbId: '<input name="F_DbId" type="text" />',
                                            F_DbTable: '<input name="F_DbTable" type="text" />',
                                            F_DbSaveFlied: '<input name="F_DbSaveFlied" type="text" />',
                                            F_DbRelationFlied: '<input name="F_DbRelationFlied" type="text" />',

                                            F_FliedLabel: '<input name="F_FliedLabel" type="text" value="' + item.text + '" class="editable"  readonly/>',
                                            F_FiledType: '<input name="F_FiledType" type="text" class="editable" value="' + item.type + '"  readonly />',
                                            F_ColName: '<input name="F_ColName" type="text" class="editable" isvalid="yes" checkexpession="NotNull" />',
                                            F_OnlyOne: '<input name="F_OnlyOne" type="checkbox"   readonly />',
                                            F_Description: '<input name="F_Description" type="text" class="editable" value="无关联" readonly />',
                                            F_SetBtn: '<div class="editable jqbtn"><span data-value="0">上移</span><span data-value="1">下移</span><span data-value="2">设置</span></div>'

                                        }
                                        var $row = $("#gridTableFlied").find('[role="row"]');
                                        $("#gridTableFlied").jqGrid('addRowData', $row.length, rowdata);
                                        $('[aria-describedby="gridTableFlied_F_SetBtn"]').removeAttr('title');

                                        $('.jqbtn>span').unbind();
                                        $('.jqbtn>span').on('click', function () {
                                            var $this = $(this);
                                            var type = $this.attr('data-value');
                                            var $row = $this.parents('[role="row"]');
                                            var rowId = $this.parents('[role="row"]').attr('id');
                                            
                                            switch (type) {
                                                case "0"://上移
                                                    if ($row.index() != 1) {
                                                        $row.find('.jqgrid-rownum').text(parseInt(rowId) - 1);
                                                        $row.attr("id", parseInt(rowId)- 1);
                                                        $row.prev().find('.jqgrid-rownum').text(rowId);
                                                        $row.prev().attr("id", rowId);

                                                        $row.prev().before($row);
                                                    }
                                                    break;
                                                case "1"://下移
                                                    var len = $("#gridTableFlied").find('[role="row"]').length;
                                                    if ($row.index() != len) {
                                                        $row.find('.jqgrid-rownum').text(parseInt(rowId) + 1);
                                                        $row.attr("id", parseInt(rowId) + 1);
                                                        $row.next().find('.jqgrid-rownum').text(rowId);
                                                        $row.next().attr("id", rowId);
                                                        $row.next().after($row);
                                                    }
                                                    break;
                                                case "2"://设置

                                                    var F_FliedLabel = $row.find('[name="F_FliedLabel"]').val();
                                                    var F_RelationType = $row.find('[name="F_RelationType"]').val();
                                                    var F_DataItemEncode = $row.find('[name="F_DataItemEncode"]').val();
                                                    var F_Value = $row.find('[name="F_Value"]').val();
                                                    var F_DbId = $row.find('[name="F_DbId"]').val();
                                                    var F_DbTable = $row.find('[name="F_DbTable"]').val();
                                                    var F_DbSaveFlied = $row.find('[name="F_DbSaveFlied"]').val();
                                                    var F_DbRelationFlied = $row.find('[name="F_DbRelationFlied"]').val();

                                                    dialogOpen({
                                                        id: "SetFieldForm",
                                                        title: '设置字段关联属性【'+F_FliedLabel+"】",
                                                        url: encodeURI(encodeURI('/SystemManage/ExcelImportTemplate/SetFieldForm?F_FliedLabel=' + F_FliedLabel + "&F_RelationType=" + F_RelationType + "&F_Value=" + F_Value + "&F_DataItemEncode=" + F_DataItemEncode + "&F_DbId=" + F_DbId + "&F_DbTable=" + F_DbTable + "&F_DbSaveFlied=" + F_DbSaveFlied + "&F_RelationType=" + F_DbRelationFlied)),
                                                        width: "500px",
                                                        height: "360px",
                                                        callBack: function (iframeId) {
                                                            getInfoTop().frames[iframeId].AcceptClick(function (data) {
                                                                $row.find('[name="F_FliedLabel"]').val(data.F_FliedLabel);
                                                                $row.find('[name="F_RelationType"]').val(data.F_RelationType);
                                                                $row.find('[name="F_DataItemEncode"]').val(data.F_DataItemEncode);
                                                                $row.find('[name="F_Value"]').val(data.F_Value);
                                                                $row.find('[name="F_DbId"]').val(data.F_DbId);
                                                                $row.find('[name="F_DbTable"]').val(data.F_DbTable);
                                                                $row.find('[name="F_DbSaveFlied"]').val(data.F_DbSaveFlied);
                                                                $row.find('[name="F_DbRelationFlied"]').val(data.F_DbRelationFlied);

                                                                $row.find('[name="F_ColName"]').removeAttr('readonly');
                                                               
                                                                setDes($row, data);
                                                            });
                                                        }
                                                    });
                                                    break;

                                            }
                                        });
                                    } else if (status == 0) {
                                        $("#gridTableFlied").find('[role="row"]').each(function () {
                                            if ($(this).find('[name="F_FliedName"]').val() == item.id) {
                                                $("#gridTableFlied").jqGrid('delRowData', $(this).attr("id"));
                                                return true;
                                            }
                                        });
                                    }
                                }
                            });
                            if (!!filedsInfo) {//编辑加载的时候有数据直接加载赋值
                                $.each(filedsInfo, function (i, item) {
                                    $("#FormFieldTree").setCheckedNodeOne(item.F_FliedName);
                                    $("#gridTableFlied").find('[role="row"]').each(function () {
                                        var $row = $(this);
                                        if ($row.find('[name="F_FliedName"]').val() == item.F_FliedName) {
                                            $row.find('[name="F_ColName"]').val(item.F_ColName);
                                            if (item.F_OnlyOne == 1)
                                            {
                                                $row.find('[name="F_OnlyOne"]').attr('checked', 'checked');
                                            }
                                            $row.find('[name="F_DataItemEncode"]').val(item.F_DataItemEncode);
                                            $row.find('[name="F_DbId"]').val(item.F_DbId);
                                            $row.find('[name="F_DbRelationFlied"]').val(item.F_DbRelationFlied);
                                            $row.find('[name="F_DbSaveFlied"]').val(item.F_DbSaveFlied);
                                            $row.find('[name="F_DbTable"]').val(item.F_DbTable);
                                            $row.find('[name="F_RelationType"]').val(item.F_RelationType);
                                            $row.find('[name="F_Value"]').val(item.F_Value);
                                            setDes($row, item);
                                            return true;
                                        }
                                    });
                                });
                            }
                            else {
                                $.each(data[0].ChildNodes, function (i, item) {
                                    $("#FormFieldTree").setCheckedNodeOne(item.id);
                                });
                            }
                           
                        }
                    });
                }
            },
            initJqGird: function () {
                var $grid = $("#gridTableFlied");
                $grid.jqGrid({
                    unwritten: false,
                    datatype: "local",
                    height: $(window).height() - 156,
                    autowidth: true,
                    colModel: [
                        { label: '字段名', name: 'F_FliedName', hidden: true },

                        { label: '关联类型', name: 'F_RelationType', hidden: true },
                        { label: '数据字典编码', name: 'F_DataItemEncode', hidden: true },
                        { label: '固定数值', name: 'F_Value', hidden: true },
                        { label: '库ID', name: 'F_DbId', hidden: true },
                        { label: '表名', name: 'F_DbTable', hidden: true },
                        { label: '保存数据字段', name: 'F_DbSaveFlied', hidden: true },
                        { label: '对应字段', name: 'F_DbRelationFlied', hidden: true },

                        { label: '字段', name: "F_FliedLabel", width: 200, align: 'left', sortable: false, resizable: false },
                        { label: '数据类型', name: "F_FiledType", width: 60, align: 'left', sortable: false, resizable: false },
                        { label: 'Excel列名', name: 'F_ColName', width: 150, align: 'left', sortable: false, resizable: false },
                        { label: '唯一性', name: 'F_OnlyOne', width: 60, align: 'center', sortable: false, resizable: false },
                        { label: '描述', name: 'F_Description', width: 212, align: 'left', sortable: false, resizable: false },
                        { label: '操作', name: 'F_SetBtn', width: 110, align: 'center', sortable: false, resizable: false }
                        
                    ],
                    pager: false,
                    rownumbers: true,
                    shrinkToFit: false,
                    gridview: true
                });            
            }
        }
    };
    var excelSetting = {
        initialPage: function () {//初始化页面元素
            //加载导向
            $('#wizard').wizard().on('change', function (e, data) {
                var $finish = $("#btn_finish");
                var $next = $("#btn_next");
                if (data.direction == "next") {
                    switch (data.step) {
                        case 1:
                            if (step.one.bind()) {
                                $finish.removeAttr('disabled');
                                $next.attr('disabled', 'disabled');
                            }
                            else {
                                return false;
                            }
                            break;
                        default:
                            break;
                    }
                } else {
                    $finish.attr('disabled', 'disabled');
                    $next.removeAttr('disabled');
                }
            });
            step.one.init();
            step.two.init(true);
            //加载数据
            if (!!keyValue)
            {
                $("#btn_finish").removeAttr('disabled');
                $("#btn_next").attr('disabled', 'disabled');
                $("#btn_last").attr('disabled', 'disabled');

                $('#wizard li').removeClass('active');
                $('#wizard  [data-target="#step-1"]').remove();
                $('#wizard  [data-target="#step-2"]').addClass('active');
                $('#step-1').removeClass('active');
                $('#step-2').addClass('active');

                learun.setForm({
                    url: "../../SystemManage/ExcelImportTemplate/GetFormJson",
                    param: { keyValue: keyValue },
                    success: function (data) {
                        console.log(data);
                        step.one.data.tableName = data.templateInfo.F_DbTable;//表名
                        step.one.data.dataBaseLinkId = data.templateInfo.F_DbId;
                        $('#templateInfo').setWebControls(data.templateInfo);
                        step.two.init(false, data.filedsInfo);

                    }
                });
            }

            //完成按钮
            $('#btn_finish').on('click', function () {
                if (!$('#templateInfo').Validform()) {
                    return false;
                }
                //模板数据
                var templateInfo = $('#templateInfo').getWebControls();
                templateInfo.F_DbId = step.one.data.dataBaseLinkId;
                templateInfo.F_DbTable = step.one.data.tableName;
                //字段数据
                var filedsInfo = [];
                $('#gridTableFlied').find('[role="row"]').each(function () {
                    var $this = $(this);
                    if ($this.find('[name="F_FliedName"]').length > 0) {
                        var one = {
                            F_FliedName: $this.find('[name="F_FliedName"]').val(),
                            F_FiledType:$this.find('[name="F_FiledType"]').val(),
                            F_ColName: $this.find('[name="F_ColName"]').val(),
                            F_OnlyOne: ($this.find('[name="F_OnlyOne"]').is(':checked') ? 1 : 0),
                            F_RelationType:$this.find('[name="F_RelationType"]').val(),
                            F_DataItemEncode: $this.find('[name="F_DataItemEncode"]').val(),
                            F_Value: $this.find('[name="F_Value"]').val(),
                            F_DbId:$this.find('[name="F_DbId"]').val(),
                            F_DbTable:$this.find('[name="F_DbTable"]').val(),
                            F_DbSaveFlied:$this.find('[name="F_DbSaveFlied"]').val(),
                            F_DbRelationFlied:$this.find('[name="F_DbRelationFlied"]').val(),
                            F_SortCode: $this.attr('id')
                        };
                        if (!one.F_ColName) {
                            learun.dialogTop({ msg: "请填写Excel列名", type: "error" });
                        }
                        filedsInfo.push(one);
                    }
                });
                //保存数据
                learun.saveForm({
                    url: "../../SystemManage/ExcelImportTemplate/SaveForm?keyValue=" + keyValue,
                    param: { templateInfo: JSON.stringify(templateInfo), filedsInfo: JSON.stringify(filedsInfo) },
                    loading: "正在保存数据...",
                    success: function () {
                        try{
                            getInfoTop().learun.currentIframe().$("#gridTable").trigger("reloadGrid");
                        } catch (e) {
                            window.parent.$("#gridTable").trigger("reloadGrid");
                        }
                    }
                });
            });
        }
    };

    function setDes($row, data) {
        var type = Number(data.F_RelationType);
        switch (type) {
            case 0://无关联
                $row.find('[name="F_Description"]').val("无关联");
                break;
            case 1://GUID
                $row.find('[name="F_ColName"]').attr('readonly', 'readonly');
                $row.find('[name="F_ColName"]').val('GUID');
                $row.find('[name="F_Description"]').val("系统产生GUID");
                break;
            case 2://数据字典
                $row.find('[name="F_Description"]').val("关联数据字典/" + data.F_DataItemName);
                break;
            case 3://数据表
                $row.find('[name="F_Description"]').val("关联数据表/" + data.F_DbTable + "/" + data.F_DbRelationFlied + "/" + data.F_DbSaveFlied);
                break;
            case 4://固定值
                $row.find('[name="F_ColName"]').attr('readonly', 'readonly');
                $row.find('[name="F_ColName"]').val(data.F_Value);
                $row.find('[name="F_Description"]').val("固定数值");
                break;
            case 5://操作人ID
                $row.find('[name="F_ColName"]').attr('readonly', 'readonly');
                $row.find('[name="F_ColName"]').val('操作人ID');
                $row.find('[name="F_Description"]').val("获取导入时的用户ID");
                break;
            case 6://操作人名字
                $row.find('[name="F_ColName"]').attr('readonly', 'readonly');
                $row.find('[name="F_ColName"]').val('操作人名字');
                $row.find('[name="F_Description"]').val("获取导入时的用户名字");
                break;
            case 7://操作时间
                $row.find('[name="F_ColName"]').attr('readonly', 'readonly');
                $row.find('[name="F_ColName"]').val('操作时间');
                $row.find('[name="F_Description"]').val("获取导入时的时间");
                break;
        }

    }

    $(function () {
        excelSetting.initialPage();
    });
})(window.jQuery, window.learun);