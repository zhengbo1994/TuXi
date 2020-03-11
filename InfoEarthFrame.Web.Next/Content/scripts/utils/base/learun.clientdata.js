/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 客户端数据
 * 陈小二
 */
/*
 *organize----------------------公司
 *department--------------------部分
 *post--------------------------岗位                         
 *role--------------------------角色
 *userGroup---------------------用户组
 *user--------------------------用户
 *authorizeMenu-----------------授权菜单
 *authorizeButton---------------授权按钮
 *authorizeColumn---------------授权列表
 *menu--------------------------菜单
 *button------------------------按钮
 *dataItem----------------------数据字典
 *excelImportTemplate-----------excel导入模板
 *excelExportTemplate-----------excel导出模板
*/
(function ($, learun) {
    "use strict";
    var clientData = {};
    function get(key, data) {
        try {
            var res = "";
            var len = data.length;
            if (len == undefined) {
                res = data[key];
            }
            else {
                for (var i = 0; i < len; i++) {
                    if(key(data[i]))
                    {
                        res = data[i];
                        break;
                    }                }
            }
            return res;
        }
        catch (e) {
            console.log(e.message);
            return "";
        }
    }
    function excelImportTemplateFormat() {//excel导入模板数据格式化
        clientData.excelImportTemplate = clientData.excelImportTemplate || [];
        $.each(clientData.excelImportTemplate, function (i, item) {
            clientData.excelImportTemplate[i] = {
                keys: item
            }
        });
    }
    learun.data = {
        init: function (callback) {
            var isShoundLoad = isLoadData();
            if (isShoundLoad) {
                $.ajax({
                    url: contentPath + "/ClientData/GetClientDataJson",
                    type: "post",
                    dataType: "json",
                    async: false,
                    success: function (data) {
                        clientData = data;
                        excelImportTemplateFormat();
                        callback();
                        window.setTimeout(function () {
                            $('#ajax-loader').fadeOut();
                        }, 50);
                    }
                });
            } else {
                callback();
                window.setTimeout(function () {
                    $('#ajax-loader').fadeOut();
                }, 50);
            }
        },
        get: function (nameArray) {//[key,function (v) { return v.key == value }]
            if(!nameArray)
            {
                return "";
            }
            var len = nameArray.length;
            var res = "";
            var data = clientData;
            for (var i = 0; i < len; i++)
            {
                res = get(nameArray[i], data);
                if (res != "" && res != undefined) {
                    data = res;
                }
                else
                {
                    break;
                }
            }
            if (res == undefined || res == null)
            {
                res = "";
            }
            return res;
        }
    };
})(window.jQuery, window.learun);