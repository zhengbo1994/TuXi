/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 自定义表单和系统表单构建
 * 陈小二
 */
(function ($,learun) {
    "use strict";

    var step = {
        one: {//第一步基础信息设置
            dbId: "",
            dbTable: "",
            dbFields: [],
            init: function () {
                $('#step-1 .panel-body').height($(window).height() - 228);
                //表单类型
                $("#F_FrmCategory").comboBoxTree({
                    description: "==请选择==",
                    maxHeight: "291px",
                    param: { EnCode: "FormSort" },
                    url: "../../SystemManage/DataItemDetail/GetDataItemTreeJson",
                    allowSearch: true
                });
            },
            valid: function (type) {
                if (!$('#step-1').Validform()) {
                    return false;
                }
                if (type == 1) {
                    $('#step-2').custmerForm("updateDbFilds", {//初始化表单设计器
                        dbId: step.one.dbId,
                        dbTable: step.one.dbTable,
                        dbFields: step.one.dbFields
                    });
                }
            },
            setData: function (data) {
                $('#step-1').setWebControls(data);
            },
            getData: function () {
                return $('#step-1').getWebControls();
            },
            0: {//自定义不建表
                init: function () {
                    step.one.init();
                    $('.systemTable').remove();
                    $('#step-1 .message').text('(自定义表表单-不建表)');
                    $('#step-1 textarea').height($(window).height() - 360);
                }
            },
            1: {//自定义建表
                init: function () {
                    step.one.init();
                    $('#step-1 .message').text('(自定义表表单-建表)');

                    $('#step-1 textarea').height($(window).height() - 475);
                    $('.systemTable').show();

                    $("#F_FrmDbId").comboBoxTree({//数据库选择
                        maxHeight: "252px",
                        url: "../../SystemManage/DataBaseLink/GetTreeJson",
                        allowSearch: true
                    }).bind("change", function (item) {
                        var value = $(this).attr('data-value');
                        step.one.dbId = value;
                        //数据表
                        $("#F_FrmDbTable").comboBox({
                            description: "",
                            param: { dataBaseLinkId: value },
                            maxHeight: "215px",
                            allowSearch: true,
                            url: "../../SystemManage/DataBaseTable/GetTableListJson",
                            id: "f_name",
                            text: "f_name",
                            selectOne: true
                        });
                    });
                    //数据表
                    $("#F_FrmDbTable").comboBox({}).bind("change", function () {
                        var value = $(this).attr('data-value');
                        step.one.dbTable = value;
                        $.ajax({
                            url: "../../SystemManage/DataBaseTable/GetTableFiledListJson",
                            data: { dataBaseLinkId: step.one.dbId, tableName: value },
                            type: "GET",
                            dataType: "json",
                            async: false,
                            success: function (data) {
                                step.one.dbFields = [];
                                $.each(data, function (id, item) {
                                    item.f_remark = item.f_column + "(" + item.f_remark + ")";
                                    step.one.dbFields.push(item);
                                });
                                //数据表
                                $("#F_FrmDbPkey").comboBox({
                                    description: "",
                                    maxHeight: "185px",
                                    allowSearch: true,
                                    data: data,
                                    id: "f_column",
                                    text: "f_remark",
                                    selectOne: true
                                });
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                dialogMsg(errorThrown, -1);
                            }
                        });

                    });
                    //主键
                    $("#F_FrmDbPkey").comboBox({});
                }
            },
            2: {//系统表单
                init: function () {
                    step.one.init();
                    $('.systemTable').remove();
                    $('#step-1 .message').text('(系统表单)');
                    $('#step-1 textarea').height($(window).height() - 360);
                }
            }
        },
        two: {//字段设计
            isSystemFormLoaded: "btn",
            isSystemFormOk:false,
            systemFormUrl: "",
            systemFormfields: [],
            custmerFormInit: function (type, data) {//自定义表单初始化
                var custmerData;
                if (!!data) {
                    custmerData = JSON.parse(data.F_FrmContent).data;
                    step.one.dbId = data.F_FrmDbId;
                    step.one.dbTable = data.F_FrmDbTable;
                }
                $('#step-2').custmerForm("init", {//初始化表单设计器
                    height: $(window).height() - 87,
                    width: $(window).width(),
                    data: custmerData,
                    type: type,
                    dbId: step.one.dbId,
                    dbTable: step.one.dbTable,
                    dbFields: step.one.dbFields
                });
            },
            getData: function (type) {
                if (type != 2) {
                    return $('#step-2').custmerForm("get");
                }
                else {
                    var data = {
                        url: $('#F_url').val(),
                        fields: step.two.systemFormfields
                    }
                    return data;
                }
            },
            valid: function (type) {
                if (type != 2) {
                    return $('#step-2').custmerForm("get", { isValid: true });
                }
                else {//对系统表单页面进行验证
                    var url = $('#F_url').val();
                    if (url == "") {
                        learun.dialogTop({ msg: "请输入系统表单地址！", type: "error" });
                        return false;
                    }
                    else if (url != step.two.systemFormUrl) {
                        step.two.systemFormUrl = url;
                        step.two.isSystemFormLoaded = "next";
                        step.two.isSystemFormOk = false;
                        $('#systemFormiframe').attr('src', url);
                        return false;
                    }
                    else {
                        if (!step.two.isSystemFormOk) {
                            learun.dialogTop({ msg: "当前输入的系统表单地址无法被加载成功,请核实！", type: "error" });
                            return false;
                        }
                    }
                    return true;
                }
            },
            0: {//自定义不建表
                init: function (data) {
                    step.two.custmerFormInit(0, data);
                },
            },
            1: {//自定义建表
                init: function (data) {
                    step.two.custmerFormInit(1, data);
                }
            },
            2: {//系统表单
                init: function (data) {
                    $('#step-2 .panelBody').height($(window).height() - 146);
                    $('#step-2 .Selectsystemform').height($(window).height() - 146);
                    $('#step-2 .filedBody').height($(window).height() - 146).hide();
                    $('#step-2 .filedBody>ul').height($(window).height() - 186);
                    $('#step-2 .iframeBody').width($(window).width());
                    
                    var _iframe = document.getElementById('systemFormiframe');
                    if (_iframe.attachEvent) {
                        _iframe.attachEvent("onload", iframeLoad);
                    } else {
                        _iframe.onload = iframeLoad;
                    }

                    $('#loadSystemForm').on('click', function () {
                        var url =top.contentpath+ $('#F_url').val();
                        if (url != "") {
                            step.two.systemFormUrl = url;
                            step.two.isSystemFormLoaded = "btn";
                            step.two.isSystemFormOk = false;
                            $('#systemFormiframe').attr('src', url);
                        }
                    });
                    function iframeLoad(e) {
                        var url = $('#F_url').val();
                        if (url != "") {
                            learun.loading({ isShow: false });
                            step.two.systemFormfields = learun.getSystemFormFields('systemFormiframe');
                            if (!!step.two.systemFormfields) {
                                $('#step-2 .Selectsystemform').fadeOut();
                                if (step.two.systemFormfields.length > 0) {
                                    $('#step-2 .filedBody').fadeIn();
                                    $('#step-2 .iframeBody').width($(window).width()-200);
                                }
                                else {
                                    $('#step-2 .filedBody').fadeOut();
                                    $('#step-2 .iframeBody').width($(window).width());
                                }
                                var _html = "";
                                $.each(step.two.systemFormfields, function (id, item) {
                                    _html += '<li class="bbit-tree-node">';
                                    _html += '<div  title="' + item.field + '" class="bbit-tree-node-el bbit-tree-node-leaf">';
                                    _html += '<i class="fa fa-tags"></i>&nbsp;';
                                    _html += '<span class="bbit-tree-node-text">' + item.label + '</span></div></li>';

                                });
                                $('#systemFormFieldList').html(_html);
                                step.two.isSystemFormOk = true;
                                if (step.two.isSystemFormLoaded == "next") {
                                    $("#btn_next").trigger('click');
                                }
                            }
                            else {
                                if (step.two.isSystemFormLoaded != "init")
                                {
                                    learun.dialogTop({ msg: "系统表单无法加载！", type: "error" });
                                }
                                $('#step-2 .Selectsystemform').show();
                            }
                        }
                    }

                    if (!!data) {
                        var systemForm = JSON.parse(data.F_FrmContent);
                        if (systemForm.data.url != "" && systemForm.data.url != undefined && systemForm.data.url != null)
                        {
                            $('#F_url').val(systemForm.data.url);
                            step.two.isSystemFormLoaded = "init";
                            step.two.isSystemFormOk = false;
                            $('#systemFormiframe').attr('src', systemForm.data.url);
                        }
                    }
                }
            }
        }
    }

    var formBuilder = {
        keyValue: "",
        formType: 0,//表单类型0不建表；1建表；2系统表单
        init: function () {
            formBuilder.keyValue = learun.request('keyValue');
            //获取表单
            if (!!formBuilder.keyValue) {
                $('#createPanel').hide();
                //获取表单
                learun.setForm({
                    url: "../../FormManage/FormModule/GetEntityJson",
                    param: { keyValue: formBuilder.keyValue },
                    success: function (data) {
                        formBuilder.initByType(data.F_FrmType, data);
                    }
                });

            }
            formBuilder.bindButtonEvent();
            formBuilder.wizardInit();//加载导向

        },
        initByType: function (type,data) {//初始化
            formBuilder.formType = parseInt(type);
            step.one[formBuilder.formType].init();
            step.two[formBuilder.formType].init(data);
            if (!!data) {
                step.one.setData(data);
                //step.two.setData(ormBuilder.formType,data);
            }

            $('#createPanel').hide();
        },
        wizardInit: function () { //加载导向
            $('#wizard').wizard().on('change', function (e, data) {
                var $finish = $("#btn_finish");
                var $next = $("#btn_next");
                if (data.direction == "next") {
                    switch (data.step) {
                        case 1:
                            return step.one.valid(formBuilder.formType);
                            break;
                        case 2:
                            if (!step.two.valid(formBuilder.formType)) {
                                return false;
                            }
                            $finish.removeAttr('disabled');
                            $next.attr('disabled', 'disabled');
                            $('#btn_draft').attr('disabled', 'disabled');
                            break;
                        default:
                            break;
                    }
                } else {
                    $finish.attr('disabled', 'disabled');
                    $next.removeAttr('disabled');
                    $('#btn_caogao').removeAttr('disabled');
                }
            });
        },
        bindButtonEvent: function () {
            $('#createPanel .box').on('click', formBuilder.createFormButton);
            $('#btn_draft').on('click', formBuilder.draftButtton);
            $('#btn_finish').on('click', formBuilder.finishButtton);
        },
        createFormButton: function () {//选择类型创建表单
            var formType = $(this).attr('data-value');
            formBuilder.initByType(formType);
        },
        draftButtton: function () {//保存草稿
            if (!$('#step-1').Validform()) {
                return false;
            }
            var baseinfo = step.one.getData();
            var formIndo = {
                type: formBuilder.formType,
                dbId: baseinfo.F_FrmDbId,
                dbTable: baseinfo.F_FrmDbTable,
                dbPkey: baseinfo.F_FrmDbPkey,
                data: step.two.getData(formBuilder.formType)
            };
            baseinfo.F_FrmContent = JSON.stringify(formIndo);
            baseinfo.F_EnabledMark = 3;
            baseinfo.F_FrmType = formBuilder.formType;

            learun.saveForm({
                url: "../../FormManage/FormModule/SaveForm?keyValue=" + formBuilder.keyValue,
                param: baseinfo,
                loading: "正在保存数据...",
                success: function () {
                    $.currentIframe().$("#gridTable").trigger("reloadGrid");
                }
            });
        },
        finishButtton: function () {//完成按钮
            var baseinfo = step.one.getData();
            var formIndo = {
                type: formBuilder.formType,
                dbId: baseinfo.F_FrmDbId,
                dbTable: baseinfo.F_FrmDbTable,
                dbPkey:baseinfo.F_FrmDbPkey,
                data: step.two.getData(formBuilder.formType)
            };
            baseinfo.F_FrmContent = JSON.stringify(formIndo);
            baseinfo.F_EnabledMark = 1;
            baseinfo.F_FrmType = formBuilder.formType;

            learun.saveForm({
                url: "../../FormManage/FormModule/SaveForm?keyValue=" + formBuilder.keyValue,
                param: baseinfo,
                loading: "正在保存数据...",
                success: function () {
                    $.currentIframe().$("#gridTable").trigger("reloadGrid");
                }
            });
        }
    }

    $(function () {//启动路口
        formBuilder.init();
    });
})(window.jQuery,window.learun);