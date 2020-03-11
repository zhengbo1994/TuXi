var formRelationId = request('Id');
var dbData = {};

$(function () {
    $.pageFn.initialPage();
    $.pageFn.init();
});
(function ($) {
    var getData = function () {
        if (!!formRelationId) {
            //获取表单
            $.SetForm({
                url: "../../FormManage/FormModule/GetFormContentJson",
                param: { keyValue: formRelationId },
                success: function (data) {
                    pageGird.init(data);
                    $.pageFn.frmEntity = data;
                }
            });
        }
    }
    var pageGird = {
        selectedRowIndex: "",
        init: function (data) {
            var colModel = [];
            frmJson = JSON.parse(data.F_FrmContent);
            colModel.push({ label: "leaCustmerFormId", name: "leaCustmerFormId", hidden: true });
            $.each(frmJson.data, function (i, item) {
                $.each(item.fields, function (j, jitem) {
                    if (jitem.type != "upload" && jitem.type != "image") {
                        if (jitem.type == "radio" || jitem.type == "checkbox" || jitem.type == "select") {
                            var _point = {};
                            if (jitem.dataSource == "dataItem") {
                                var _point = {
                                    label: jitem.label, name: jitem.field, index: jitem.field, width: 120, align: "left",
                                    formatter: function (cellvalue, options, rowObject) {
                                        return top.learun.data.get(['dataItem', jitem.dataItemCode, cellvalue]);
                                    }
                                }
                            }
                            else {
                                if (dbData[jitem.dbId] == undefined) {
                                    dbData[jitem.dbId] = {}
                                }
                                if (dbData[jitem.dbId][jitem.dbTable] == undefined) {
                                    dbData[jitem.dbId][jitem.dbTable] = {}
                                    learun.getDataForm({
                                        type: "get",
                                        url: "../../SystemManage/DataSource/GetTableData?dbLinkId=" + jitem.dbId + "&tableName=" + jitem.dbTable,
                                        async: false,
                                        success: function (dataItemdata) {
                                            $.each(dataItemdata, function (i, dbdataitem) {
                                                dbData[jitem.dbId][jitem.dbTable][dbdataitem[jitem.dbFiledValue.toLowerCase()]] = dbdataitem[jitem.dbFiledText.toLowerCase()]
                                            });
                                        }
                                    });
                                }

                                var _point = {
                                    label: jitem.label, name: jitem.field, index: jitem.field, width: 120, align: "left",
                                    formatter: function (cellvalue, options, rowObject) {
                                        return dbData[jitem.dbId][jitem.dbTable][cellvalue];
                                    }
                                }
                            }
                            colModel.push(_point);
                        }
                        else if (jitem.type == "baseSelect") {
                            var _point = {
                                label: jitem.label, name: jitem.field, index: jitem.field, width: 120, align: "left",
                                formatter: function (cellvalue, options, rowObject) {
                                    var data = "";
                                    switch (data.baseType) {
                                        case "user"://用户
                                            data = top.learun.data.get(['user', cellvalue, "RealName"]);
                                            break;
                                        case "department"://部门
                                            data = top.learun.data.get(["department", cellvalue, "FullName"]);
                                            break;
                                        case "organize"://公司
                                            data = top.learun.data.get(["organize", cellvalue, "FullName"]);
                                            break;
                                        case "post"://岗位
                                            data = top.learun.data.get(["post", cellvalue, "FullName"]);
                                            break;
                                        case "job"://职位
                                            data = top.learun.data.get(["post", cellvalue, "FullName"]);
                                            break;
                                        case "role"://角色
                                            data = top.learun.data.get(["role", cellvalue, "FullName"]);
                                            break;
                                    }
                                    return data;
                                }
                            }
                            colModel.push(_point);
                        }
                        else if (jitem.type == "currentInfo") {
                            var _point = {
                                label: jitem.label, name: jitem.field, index: jitem.field, width: 120, align: "left",
                                formatter: function (cellvalue, options, rowObject) {
                                    var data = "";
                                    switch (jitem.infoType) {
                                        case "user":
                                            data = top.learun.data.get(['user', cellvalue, "RealName"])
                                            break;
                                        case "department":
                                            data = top.learun.data.get(["department", cellvalue, "FullName"]);
                                            break;
                                        case "organize":
                                            data = top.learun.data.get(["organize", cellvalue, "FullName"]);
                                            break;
                                        case "date":
                                            data = cellvalue;
                                            break;
                                    }
                                    return data;
                                }
                            }
                            colModel.push(_point);
                        }
                        else {
                            var _point = { label: jitem.label, name: jitem.field, index: jitem.field, width: 120, align: "left" }
                            colModel.push(_point);
                        }
                    }
                });
            });
            $("#gridTable").jqGrid({
                url: "../../FormManage/FormModule/GetInstancePageList?relationFormId=" + formRelationId,
                datatype: "json",
                height: $(window).height() - 139.5,
                autowidth: true,
                colModel: colModel,
                viewrecords: true,
                rowNum: 30,
                rowList: [30, 50, 100],
                pager: "#gridPager",
                sortname: 'F_CreateDate',
                rownumbers: true,
                shrinkToFit: false,
                gridview: true,
                onSelectRow: function () {
                    pageGird.selectedRowIndex = $("#" + this.id).getGridParam('selrow');
                },
                gridComplete: function () {
                    $("#" + this.id).setSelection(pageGird.selectedRowIndex, false);
                }
            });
            $("#btn_Search").click(function () {
                $("#gridTable").trigger('reloadGrid');
            });
        }
    }

    $.pageFn = {
        frmEntity: "",
        init: function () {
            getData();
        },
        initialPage: function () {
            //resize重设(表格、树形)宽高
            $(window).resize(function (e) {
                window.setTimeout(function () {
                    $('#gridTable').setGridWidth(($('.gridPanel').width()));
                    $("#gridTable").setGridHeight($(window).height() - 139.5);
                }, 200);
                e.stopPropagation();
            });
        }
    };
})(window.jQuery);
//新增
function btn_add() {
    dialogOpen({
        id: "InstanceForm",
        title: '新增',
        url: '/FormManage/FormModule/CustmerFormForm' + "?formRelationId=" + formRelationId,
        width: "800px",
        height: "600px",
        callBack: function (iframeId) {
            top.frames[iframeId].AcceptClick();
        }
    });
};
//编辑
function btn_edit() {
    var leaformId = $("#gridTable").jqGridRowValue("leaCustmerFormId");
    if (checkedRow(leaformId)) {
        dialogOpen({
            id: "Form",
            title: '编辑',
            url: '/FormManage/FormModule/CustmerFormForm?keyValue=' + leaformId + "&formRelationId=" + formRelationId,
            width: "800px",
            height: "600px",
            callBack: function (iframeId) {
                top.frames[iframeId].AcceptClick();
            }
        });
    }
}
//删除
function btn_delete() {
    var leaformId = $("#gridTable").jqGridRowValue("leaCustmerFormId");
    if (leaformId) {
        $.RemoveForm({
            url: "../../FormManage/FormModule/RemoveInstanceForm",
            param: { keyValue: leaformId, frmContentId: $.pageFn.frmEntity.F_Id },
            success: function (data) {
                $("#gridTable").trigger("reloadGrid");
            }
        });
    } else {
        dialogMsg('请选择需要删除的表单模板！', 0);
    }
}