/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 公共JS基础扩展库,处理自定表单,扩展属性,Excel导入导出配置
 * 陈小二
 */
(function ($, learun) {
    "use strict";

    function excelImportAddButton(btnObj) {
        learun.addToolbarButton({
            "id": btnObj.F_EnCode,
            "toolbar": ".toolbar",
            "icon": "fa fa-sign-in",
            "name": btnObj.F_FullName,
            "event": function () {
                learun.dialogOpen({
                    id: "ExcelImportForm",
                    title: '快速导入',
                    url: '/Utility/ExcelImportForm?btnId=' + btnObj.F_EnCode,
                    width: "1100px",
                    height: "700px",
                    btn: null,
                    callBack: function (iframeId) {
                        console.log("cbb");
                    }
                });
            }
        });
    };

    function excelExportAddButton(btnObj, template) {
        learun.addToolbarButton({
            "id": btnObj.F_EnCode,
            "toolbar": ".toolbar",
            "icon": "fa fa-sign-out",
            "name": btnObj.F_FullName,
            "event": function () {
                dialogOpen({
                    id: "ExcelIExportDialog",
                    title: template.F_Name,
                    url: '/Utility/ExcelExportForm?gridId=' + template.F_GridId + '&filename=' + template.F_Name,
                    width: "500px",
                    height: "380px",
                    callBack: function (iframeId) {
                        getInfoTop().frames[iframeId].AcceptClick();
                    }, btn: ['导出Excel', '关闭']
                });
            }
        });
    };

    $.extend(learun, {
        addToolbarButton: function (opt) {
            /*id 按钮Id,toolbar 工具条,name 按钮名称,icon 按钮图标,event 按钮事件*/
            if ($('#' + opt.id).length == 0) {
                var btngroup = $('#learun-excel-btn-list');
                if (btngroup.length == 0) {
                    $(opt.toolbar).append('<div class="btn-group" id="learun-excel-btn-list"><a id="' + opt.id + '" class="btn btn-default" ><i class="' + opt.icon + '"></i>&nbsp;' + opt.name + '</a></div>');

                }
                else {
                    btngroup.append('<a id="' + opt.id + '" class="btn btn-default" ><i class="' + opt.icon + '"></i>&nbsp;' + opt.name + '</a>');
                }
            }
            $('#' + opt.id).unbind();
            $('#' + opt.id).on('click', opt.event);
        },
        excel: {
            init: function () {
                var moduleId = getInfoTop().$.cookie('currentmoduleId');
                var moduleObj = getInfoTop().learun.data.get(["menu", moduleId]);
                if (moduleObj != "" && window.location.href.indexOf(moduleObj.F_UrlAddress) != -1)
                {
                    //初始化excel导入功能
                    var template = getInfoTop().learun.data.get(["excelImportTemplate", moduleId]);
                    if (!!template)
                    {
                        if (!!template.entitys) {
                            $.each(template.entitys, function (i, item) {
                                excelImportAddButton(item.btn);
                            });
                        }
                        else {
                            template["entitys"] = {};
                            $.each(template.keys, function (i, item) {
                                learun.getDataForm({
                                    url: "../../SystemManage/ExcelImportTemplate/GetFormJson",
                                    param: { keyValue: item },
                                    async: true,
                                    type: "get",
                                    success: function (data) {
                                        var btnObj = getInfoTop().learun.data.get(["authorizeButton", moduleId, function (v) { return v.F_ModuleButtonId == data.templateInfo.F_ModuleBtnId }]);
                                        if (!!btnObj) {
                                            excelImportAddButton(btnObj);
                                            if (!template.entitys[btnObj.F_EnCode]) {
                                                template.entitys[btnObj.F_EnCode] = {
                                                    btn:btnObj,
                                                    data:[]
                                                };
                                            }
                                            template.entitys[btnObj.F_EnCode].data.push(data);
                                        }
                                    }
                                });
                            });
                        }
                    }
                    //初始化excel导出功能
                    var exportTemplate = getInfoTop().learun.data.get(["excelExportTemplate", moduleId]);
                    if (!!exportTemplate) {
                        console.log(exportTemplate);
                        $.each(exportTemplate, function (i, item) {
                            var btnObj = getInfoTop().learun.data.get(["authorizeButton", moduleId, function (v) { return v.F_ModuleButtonId == item.F_ModuleBtnId }]);
                            excelExportAddButton(btnObj, item);
                        });
                    }
                }
            }
        }
    });
})(window.jQuery, window.learun);