/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 表单设计器-组件
 * 陈小二
 */
(function ($, learun) {
    "use strict";

    var qdbData, qdbbTable = {}, qdataItem, qcurrentData;
    var _flagCode = 0;

    //获取组件行显示html
    function getComponentRowHtml(data) {
        var _html = '<div class="item_field_label"><span>' + data.name + '</span></div><div class="item_field_value">' + data.text + '</div><div class="item_field_remove"><i  title="移除控件" class="del fa fa-close"></i></div>';
        return _html;
    };
    //绑定数据表字段
    function setControlField(opt, itemdata) {
        var $html = $(".field_option");
        var _field = itemdata.field;
        if (opt.type == 0) {
            var _value = (_field == "" ? learun.createGuid() : _field);
            $html.find("#control_field").parents('.field_control').html('<input id="control_field" type="text" class="form-control" disabled data-text="' + _value + '" value="' + _value + '" />');
            itemdata.field = _value;
        }
        else {
            $html.find("#control_field").parents('.field_control').html('<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>');
            $html.find("#control_field").comboBox({
                data: opt.dbFields,
                id: "f_column",
                text: "f_remark",
                maxHeight: "300px",
                allowSearch: true
            });
            $html.find("#control_field").comboBoxSetValue(_field);
            $html.find('#control_field').change(function (e) {
                var value = $(this).attr('data-value');
                itemdata.field = value;
            });
        }
    };
    //设置验证方式
    function setValidatorType(itemdata) {
        var _data = [];
        _data.push({ id: "NotNull", text: "不能为空！" });
        _data.push({ id: "Num", text: "必须为数字！" });
        _data.push({ id: "NumOrNull", text: "数字或空！" });
        _data.push({ id: "Email", text: "必须为E-mail格式！" });
        _data.push({ id: "EmailOrNull", text: "E-mail格式或空！" });
        _data.push({ id: "EnglishStr", text: "必须为字符串！" });
        _data.push({ id: "EnglishStrOrNull", text: "字符串或空！" });
        _data.push({ id: "Phone", text: "必须电话格式！" });
        _data.push({ id: "PhoneOrNull", text: "电话格式或者空！" });
        _data.push({ id: "Fax", text: "必须为传真格式！" });
        _data.push({ id: "Mobile", text: "必须为手机格式！" });
        _data.push({ id: "MobileOrNull", text: "手机格式或者空！" });
        _data.push({ id: "MobileOrPhone", text: "电话格式或手机格式！" });
        _data.push({ id: "MobileOrPhoneOrNull", text: "电话格式或手机格式或空！" });
        _data.push({ id: "Uri", text: "必须为网址格式！" });
        _data.push({ id: "UriOrNull", text: "网址格式或空！" });

        $("#control_verify").comboBox({
            data: _data,
            id: "id",
            text: "text",
            maxHeight: "200px",
            allowSearch: true
        }).change(function (e) {
            var value = $(this).attr('data-value');
            itemdata.verify = value;
        });
        $("#control_verify").comboBoxSetValue(itemdata.verify);
    };
    function setValidatorTypeOnlyNotNull(itemdata) {
        var _data = [];
        _data.push({ id: "NotNull", text: "不能为空！" });
        $("#control_verify").comboBox({
            data: _data,
            id: "id",
            text: "text"
        }).change(function (e) {
            var value = $(this).attr('data-value');
            itemdata.verify = value;
        });
        $("#control_verify").comboBoxSetValue(itemdata.verify);
    };
    //设置数据源选项
    function setDataSource(itemdata) {
        var _data = [];
        _data.push({ id: "dataItem", text: "数据字典" });
        _data.push({ id: "dataTable", text: "数据库表" });

        $("#control_dataSource").comboBox({
            data: _data,
            id: "id",
            text: "text",
            description: ""
        }).change(function (e) {
            var value = $(this).attr('data-value');
            itemdata.dataSource = value;
            if (value == "dataItem") {
                $(".dataDb").hide();
                $(".dataItem").show();
            }
            else {
                setDbCtrl(itemdata);
                $(".dataItem").hide();
                $(".dataDb").show();
            }
        });
        $("#control_dataSource").comboBoxSetValue(itemdata.dataSource);
    };
    //设置默认值
    function setDefaultCtrl(itemdata) {
        $("#control_default").comboBox({
            description: "无则不填"
        }).change(function (e) {
            var value = $(this).attr('data-value');
            itemdata.defaultValue = value;

        });
        $("#control_default").comboBoxSetValue(itemdata.defaultValue);
    };
    //数据字典初始化
    function setDataItemCtrl(itemdata) {
        if (!!qdataItem) {
            init(qdataItem);
        }
        else {
            learun.getDataForm({
                type: "get",
                url: "../../SystemManage/DataItem/GetTreeJson",
                async: true,
                success: function (data) {
                    qdataItem = data;
                    init(data);
                }
            });
        }
        function init(data) {
            $("#control_dataItem").comboBoxTree({
                data: data,
                maxHeight: "180px",
                click: function (item) {
                    itemdata.dataItemCode = item.value;
                    itemdata.dataItemCodeId = item.id;
                    if (itemdata.dataSource == "dataItem") {
                        $("#control_default").comboBox({
                            url: "../../SystemManage/DataItemDetail/GetDataItemTreeJson?EnCode=" + itemdata.dataItemCode,
                            id: "value",
                            text: "text",
                            maxHeight: "200px",
                            description: "无则不填"
                        });
                        $("#control_default").comboBoxSetValue(itemdata.defaultValue);
                    }
                },
                allowSearch: true
            });
            $("#control_dataItem").comboBoxTreeSetValue(itemdata.dataItemCodeId);
        }
    };
    function setDistrict(itemdata) {
        $("#control_dataSourceProvince").comboBox({
            url: "../../SystemManage/Area/GetAreaJson",
            param: { areaId: itemdata.provinceId },
            id: "F_AreaCode",
            text: "F_AreaName",
            maxHeight: "200px",
            description: "全国"
        }).change(function (e) {
            var value = $(this).attr('data-value');
            itemdata.provinceId = value;
        });
        $("#control_dataSourceProvince").comboBoxSetValue(itemdata.provinceId);
    }
    //数据库选择初始化
    function setDbCtrl(itemdata) {
        function dbInit(data) {
            $("#control_db").unbind();
            $("#control_db").bind('change', function () {
                var dbId = $(this).attr('data-value');
                itemdata.dbId = dbId;
                if (!!qdbbTable[dbId]) {
                    dbtableInit(qdbbTable[dbId]);
                }
                else {
                    learun.getDataForm({
                        type: "get",
                        url: "../../SystemManage/DataBaseTable/GetTableListJson",
                        param: { dataBaseLinkId: dbId },
                        success: function (data) {
                            qdbbTable[dbId] = data;
                            dbtableInit(data);
                        }
                    });
                }
            });
            $("#control_db").comboBox({
                data: data,
                id: "F_DatabaseLinkId",
                text: "F_DBAlias",
                selectOne: (!!itemdata.dbId ? false : true),
                maxHeight: "140px",
                description: ""
            });
            $("#control_db").comboBoxSetValue(itemdata.dbId);
        }
        function dbtableInit(data) {
            //数据表
            $("#control_dbTable").unbind();
            $("#control_dbTable").bind('change', function () {
                var tableName = $(this).attr('data-value');
                itemdata.dbTable = tableName;
                //数据字段
                learun.getDataForm({
                    type: "get",
                    url: "../../SystemManage/DataBaseTable/GetTableFiledListJson",
                    param: { dataBaseLinkId: itemdata.dbId, tableName: tableName },
                    async: true,
                    success: function (filedData) {
                        dbFiledInit(filedData);
                    }
                });
            });
            $("#control_dbTable").comboBox({
                data: data,
                id: "f_name",
                text: "f_name",
                selectOne: (!!itemdata.dbTable ? false : true),
                maxHeight: "120px",
                description: "",
                allowSearch: true
            });
            $("#control_dbTable").comboBoxSetValue(itemdata.dbTable);
        }
        function dbFiledInit(filedData) {
            $("#control_dbFiledText").unbind();
            $("#control_dbFiledValue").unbind();
            $("#control_dbFiledText").bind('change', function () {
                itemdata.dbFiledText = $(this).attr('data-value');
                if (!!$('#control_dbFiledValue').attr('data-value')) {
                    itemdata.dbFiledValue = $('#control_dbFiledValue').attr('data-value');
                    if (itemdata.dataSource == "dataTable") {
                        $("#control_default").comboBox({
                            url: "../../SystemManage/DataSource/GetTableData?dbLinkId=" + itemdata.dbId + "&tableName=" + itemdata.dbTable,
                            id: itemdata.dbFiledValue.toLowerCase(),
                            text: itemdata.dbFiledText.toLowerCase(),
                            maxHeight: "200px",
                            description: "无则不填"
                        });
                        $("#control_default").comboBoxSetValue(itemdata.defaultValue);
                    }
                }
            });
            $("#control_dbFiledValue").bind('change', function () {
                itemdata.dbFiledValue = $(this).attr('data-value');
                if (!!$('#control_dbFiledText').attr('data-value')) {
                    itemdata.dbFiledText = $('#control_dbFiledText').attr('data-value');

                    if (itemdata.dataSource == "dataTable") {
                        $("#control_default").comboBox({
                            url: "../../SystemManage/DataSource/GetTableData?dbLinkId=" + itemdata.dbId + "&tableName=" + itemdata.dbTable,
                            id: itemdata.dbFiledValue.toLowerCase(),
                            text: itemdata.dbFiledText.toLowerCase(),
                            maxHeight: "200px",
                            description: "无则不填"
                        });
                        $("#control_default").comboBoxSetValue(itemdata.defaultValue);
                    }
                }
            });
            $("#control_dbFiledText").comboBox({
                data: filedData,
                id: "f_column",
                text: "f_column",
                selectOne: (!!itemdata.dbFiledText ? false : true),
                maxHeight: "100px",
                description: ""
            });
            $("#control_dbFiledValue").comboBox({
                data: filedData,
                id: "f_column",
                text: "f_column",
                selectOne: (!!itemdata.dbFiledValue ? false : true),
                maxHeight: "100px",
                description: ""
            });

            $("#control_dbFiledText").comboBoxSetValue(itemdata.dbFiledText);
            $("#control_dbFiledValue").comboBoxSetValue(itemdata.dbFiledValue);
        }
        //数据库
        if (!!qdbData) {
            dbInit(qdbData);
        }
        else {
            learun.getDataForm({
                type: "get",
                url: "../../SystemManage/DataBaseLink/GetListJson",
                async: true,
                success: function (data) {
                    qdbData = data;
                    dbInit(data);
                }
            });
        }

    };
    //设置组织单位数据级联
    function setBaseInfoRelation(itemdata, baseInfoRelation) {
        $("#control_relation").comboBox({
            data: baseInfoRelation,
            id: "id",
            text: "label",
            maxHeight: "200px",
            description: "无则不填"
        }).change(function (e) {
            var value = $(this).attr('data-value');
            itemdata.relation = value;
        });
        $("#control_relation").comboBoxSetValue(itemdata.relation);
    }

    //渲染表格部分
    //获取<font face="宋体">*</font>
    function getFontHtml(verify) {
        var res = "";
        switch (verify) {
            case "NotNull":
            case "Num":
            case "Email":
            case "EnglishStr":
            case "Phone":
            case "Fax":
            case "Mobile":
            case "MobileOrPhone":
            case "Uri":
                res = '<font face="宋体">*</font>';
                break;
        }
        return res;
    }
    function getTdValidatorHtml(verify) {
        var res = "";
        if (verify != "") {
            res = 'isvalid="yes" checkexpession="' + verify + '"';
        }
        return res;

    }

    learun.components = {
        text: {//文本框
            init: function () {
                var $html = $('<div class="item_row" data-type="text" ><i  class="fa fa-italic"></i>文本框</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "文本框",
                    type: "text",
                    field: "",
                    defaultValue: "",
                    verify: ""
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "文本框" }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>无样式的单行文本框</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"/></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">默认值<i title="仅在添加数据时默认填入" class="help fa fa-question-circle"></i></div>';
                _html += '<div class="field_control"><input id="control_default" type="text" class="form-control" placeholder="无则不填"/></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setValidatorType(itemdata);

                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_default').val(itemdata.defaultValue);

                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
                $html.find('#control_default').keyup(function (e) {
                    var value = $(this).val();
                    itemdata.defaultValue = value;
                });
            },
            renderTable: function (data) {//使用表单的时候渲染成table
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td class="formValue custmerTd" data-type="' + data.type + '"  data-value="' + data.field + '"   ><input id="' + data.id + '" type="text" class="form-control" ' + getTdValidatorHtml(data.verify) + ' /></td>';
                var $td = $(_html);
                $td.find('input').val(data.defaultValue);
                return $td;
            },
            validTable: function ($obj) {
                return $obj.Validform();
            },
            getValue: function ($obj) {//获取数据保存
                var $input = $obj.find('input');
                var point = {
                    type: "text",
                    value: $input.val(),
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                var $input = $td.find('input');
                $input.val(data.value);
            }
        },
        textarea: {//文本区
            init: function () {
                var $html = $('<div class="item_row" data-type="textarea" ><i class="fa fa-align-justify"></i>文本区</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "文本区",
                    type: "textarea",
                    field: "",
                    defaultValue: "",
                    height: "100px",
                    verify: ""
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "文本区" }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>无样式的多行文本框</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">字段高度</div>';
                _html += '<div class="field_control"><input id="control_height" type="text" class="form-control" value="100px"></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">默认值<i title="仅在添加数据时默认填入" class="help fa fa-question-circle"></i></div>';
                _html += '<div class="field_control"><input id="control_default" type="text" class="form-control" placeholder="无则不填"></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setValidatorType(itemdata);

                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_height').val(itemdata.height);
                $html.find('#control_default').val(itemdata.defaultValue);

                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
                $html.find('#control_height').change(function (e) {
                    var value = $(this).val();
                    itemdata.height = value;
                });
                $html.find('#control_default').keyup(function (e) {
                    var value = $(this).val();
                    itemdata.defaultValue = value;
                });

            },
            renderTable: function (data) {//使用表单的时候渲染成table
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td class="formValue custmerTd" data-type="' + data.type + '"  data-value="' + data.field + '"  ><textarea id="' + data.id + '"  class="form-control" ' + 'style="height: ' + data.height + ';"' + getTdValidatorHtml(data.verify) + ' /></td>';
                var $td = $(_html);
                $td.find('textarea').val(data.defaultValue);
                return $td;
            },
            validTable: function ($obj) {
                return $obj.Validform();
            },
            getValue: function ($obj) {//获取数据保存
                var $input = $obj.find('textarea');
                var point = {
                    type: "textarea",
                    value: $input.val(),
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                var $input = $td.find('textarea');
                $input.val(data.value);
            }
        },
        texteditor: {//编辑器
            init: function () {
                var $html = $('<div class="item_row" data-type="texteditor" ><i class="fa fa-edit"></i>编辑器</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "编辑器",
                    type: "texteditor",
                    field: "",
                    defaultValue: "",
                    height: "200px",
                    verify: ""
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "编辑器" }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>设置丰富文字样式的多行文本编辑区</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">字段高度</div>';
                _html += '<div class="field_control"><input id="control_height" type="text" class="form-control" value="200px"></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">默认值<i title="仅在添加数据时默认填入" class="help fa fa-question-circle"></i></div>';
                _html += '<div class="field_control"><input id="control_default" type="text" class="form-control" placeholder="无则不填"></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setValidatorTypeOnlyNotNull(itemdata);

                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_height').val(itemdata.height);
                $html.find('#control_default').val(itemdata.defaultValue);

                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
                $html.find('#control_height').change(function (e) {
                    var value = $(this).val();
                    itemdata.height = value;
                });
                $html.find('#control_default').keyup(function (e) {
                    var value = $(this).val();
                    itemdata.defaultValue = value;
                });
            },
            renderTable: function (data) {//使用表单的时候渲染成table
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td class="formValue custmerTd"  data-name="' + data.label + '"  data-type="' + data.type + '"  data-value="' + data.field + '"  data-verify="' + data.verify + '"  ><textarea id="' + data.id + '"  class="form-control" ' + ' /></td>';
                var $td = $(_html);
                var simditor = new Simditor({
                    textarea: $td.find('textarea'),
                    placeholder: '这里输入内容...',
                    toolbar: ['title', 'bold', 'italic', 'underline', 'strikethrough', 'color', '|', 'ol', 'ul', 'blockquote', 'table', '|', 'link', 'image', 'hr', '|', 'indent', 'outdent']
                });
                $td.find(".simditor .simditor-body").height(parseInt(data.height.replace(/px/g, ''))).css({ "overflow": "auto", "min-height": "0" });                $td[1].simditor = simditor;
                simditor.setValue(data.defaultValue);
                return $td;
            },
            validTable: function ($obj) {
                var verify = $obj.attr("data-verify");
                if (verify == "NotNull") {
                    var data = $obj[0].simditor.getValue();
                    if (data != "" && data != undefined && data != null) {
                        return true;
                    }
                    var name = $obj.attr("data-name");
                    learun.dialogTop({ msg: name + "不能为空！", type: "error" });
                    return false;
                }
                else {
                    return true;
                }

            },
            getValue: function ($obj) {//获取数据保存
                var data = $obj[0].simditor.getValue();
                var point = {
                    type: "texteditor",
                    value: data,
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                $td[0].simditor.setValue(data.value);
            }
        },
        radio: {//单选框
            init: function () {
                var $html = $('<div class="item_row" data-type="radio" ><i class="fa fa-circle-thin"></i>单选框</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "单选项",
                    type: "radio",
                    field: "",
                    defaultValue: "",
                    dataSource: "dataItem",
                    dataItemCode: "",
                    dbId: "",
                    dbTable: "",
                    dbFiledText: "",
                    dbFiledValue: ""
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "单选项" }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>显示设置数据，从中只可选择一项</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">默认值<i title="仅在添加数据时默认填入" class="help fa fa-question-circle"></i></div>';
                _html += '<div class="field_control"><div id="control_default" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">数据来源</div>';
                _html += '<div class="field_control"><div id="control_dataSource" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataItem">数据字典</div>';
                _html += '<div class="field_control dataItem"><div id="control_dataItem" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">数据库</div>';
                _html += '<div class="field_control dataDb"><div id="control_db" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">数据表</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbTable" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">显示字段</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbFiledText" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">保存字段</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbFiledValue" type="select" class="ui-select"></div></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setDataSource(itemdata);
                setDefaultCtrl(itemdata);
                setDataItemCtrl(itemdata);
                $html.find('#control_label').val(itemdata.label);


                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
            },
            renderTable: function (data) {//使用表单的时候渲染成table
                var _html = '<th class="formTitle">' + data.label + '</th>';
                _html += '<td class="formValue custmerTd" data-type="' + data.type + '"  data-value="' + data.field + '"   ><div class="radio" id="' + data.id + '" style="color: #95A0AA;"></div></td>';
                var $td = $(_html);

                if (data.dataSource == "dataItem") {
                    learun.getDataForm({
                        type: "get",
                        url: "../../SystemManage/DataItemDetail/GetDataItemTreeJson?EnCode=" + data.dataItemCode,
                        async: false,
                        success: function (dataItemdata) {
                            $.each(dataItemdata, function (i, item) {
                                var $point = $('<label><input name="' + data.id + '" value="' + item.value + '"' + '" type="radio">' + item.text + '</label>');
                                $td.find('div').append($point);
                            });
                            $td.find('input[value="' + data.defaultValue + '"]').attr('checked', 'checked');
                        }
                    });
                }
                else {
                    learun.getDataForm({
                        type: "get",
                        url: "../../SystemManage/DataSource/GetTableData?dbLinkId=" + data.dbId + "&tableName=" + data.dbTable,
                        async: false,
                        success: function (dataItemdata) {
                            $.each(dataItemdata, function (i, item) {
                                var $point = $('<label><input name="' + data.id + '" value="' + item[data.dbFiledValue.toLowerCase()] + '"' + '" type="radio">' + item[data.dbFiledText.toLowerCase()] + '</label>');
                                $td.find('div').append($point);
                            });
                            $td.find('input[value="' + data.defaultValue + '"]').attr('checked', 'checked');
                        }
                    });
                }
                return $td;
            },
            validTable: function ($obj) {
                return true;
            },
            getValue: function ($obj) {//获取数据保存
                var point = {
                    type: "radio",
                    value: $obj.find('input:checked').val(),
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                $td.find('input[value="' + data.value + '"]').attr('checked', 'checked');
            }
        },
        checkbox: {//多选框
            init: function () {
                var $html = $('<div class="item_row" data-type="checkbox" ><i class="fa fa-square-o"></i>多选框</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "多选项",
                    type: "checkbox",
                    field: "",
                    dataSource: "dataItem",
                    dataItemCode: "",
                    dbId: "",
                    dbTable: "",
                    dbFiledText: "",
                    dbFiledValue: ""
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "多选项" }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>显示设置数据，从中可以选择多项</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">数据来源</div>';
                _html += '<div class="field_control"><div id="control_dataSource" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataItem">数据字典</div>';
                _html += '<div class="field_control dataItem"><div id="control_dataItem" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">数据库</div>';
                _html += '<div class="field_control dataDb"><div id="control_db" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">数据表</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbTable" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">显示字段</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbFiledText" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">保存字段</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbFiledValue" type="select" class="ui-select"></div></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setDataSource(itemdata);
                setDataItemCtrl(itemdata);
                $html.find('#control_label').val(itemdata.label);

                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
            },
            renderTable: function (data) {//使用表单的时候渲染成table
                var _html = '<th class="formTitle">' + data.label + '</th>';
                _html += '<td class="formValue custmerTd" data-type="' + data.type + '" data-value="' + data.field + '" ><div class="checkbox" id="' + data.id + '" style="color: #95A0AA;"></div></td>';
                var $td = $(_html);

                if (data.dataSource == "dataItem") {
                    learun.getDataForm({
                        type: "get",
                        url: "../../SystemManage/DataItemDetail/GetDataItemTreeJson?EnCode=" + data.dataItemCode,
                        async: false,
                        success: function (dataItemdata) {
                            $.each(dataItemdata, function (i, item) {
                                var $point = $('<label><input name="' + data.id + '" value="' + item.value + '"' + '" type="checkbox">' + item.text + '</label>');
                                $td.find('div').append($point);
                            });
                        }
                    });
                }
                else {
                    learun.getDataForm({
                        type: "get",
                        url: "../../SystemManage/DataSource/GetTableData?dbLinkId=" + data.dbId + "&tableName=" + data.dbTable,
                        async: false,
                        success: function (dataItemdata) {
                            $.each(dataItemdata, function (i, item) {
                                var $point = $('<label><input name="' + data.id + '" value="' + item[data.dbFiledValue.toLowerCase()] + '"' + '" type="checkbox">' + item[data.dbFiledText.toLowerCase()] + '</label>');
                                $td.find('div').append($point);
                            });
                        }
                    });
                }
                return $td;
            },
            validTable: function ($obj) {
                return true;
            },
            getValue: function ($obj) {//获取数据保存
                var value = "";
                $obj.find('input:checked').each(function (i) {
                    if (value != "") {
                        value += ",";
                    }
                    value += $(this).val();
                });
                var point = {
                    type: "checkbox",
                    value: value,
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                var valueList = data.value.split(',');
                $.each(valueList, function (i, item) {
                    $td.find('input[value="' + item + '"]').attr('checked', 'checked');
                });
            }
        },
        select: {//下拉框
            init: function () {
                var $html = $('<div class="item_row" data-type="select" ><i class="fa fa-caret-square-o-right"></i>下拉框</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "下拉框",
                    type: "select",
                    field: "",
                    verify: "",
                    height: "100px",
                    dataSource: "dataItem",
                    dataItemCode: "",
                    dbId: "",
                    dbTable: "",
                    dbFiledText: "",
                    dbFiledValue: ""
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "下拉框" }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                //_html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>显示设置数据，从中只可选择一项</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">数据来源</div>';
                _html += '<div class="field_control"><div id="control_dataSource" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataItem">数据字典</div>';
                _html += '<div class="field_control dataItem"><div id="control_dataItem" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">数据库</div>';
                _html += '<div class="field_control dataDb"><div id="control_db" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">数据表</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbTable" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">显示字段</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbFiledText" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title dataDb">保存字段</div>';
                _html += '<div class="field_control dataDb"><div id="control_dbFiledValue" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">下拉框高度</div>';
                _html += '<div class="field_control"><input id="control_height" type="text" class="form-control" value="100px"></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setValidatorTypeOnlyNotNull(itemdata);
                setDataSource(itemdata);
                setDataItemCtrl(itemdata);
                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_height').val(itemdata.height);

                $html.find('#control_height').change(function (e) {
                    var value = $(this).val();
                    itemdata.height = value;
                });
                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
            },
            renderTable: function (data) {//使用表单的时候渲染成table
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td class="formValue custmerTd" data-type="' + data.type + '"  data-value="' + data.field + '" ><div id="' + data.id + '" type="select" class="ui-select"  ' + getTdValidatorHtml(data.verify) + ' ></div></td>';
                var $td = $(_html);
                if (data.dataSource == "dataItem") {
                    learun.getDataForm({
                        type: "get",
                        url: "../../SystemManage/DataItemDetail/GetDataItemTreeJson?EnCode=" + data.dataItemCode,
                        async: true,
                        success: function (dataItemdata) {
                            $td.find("#" + data.id).comboBox({
                                data: dataItemdata,
                                id: "value",
                                text: "text",
                                maxHeight: data.height,
                                allowSearch: true
                            });
                        }
                    });
                }
                else {
                    learun.getDataForm({
                        type: "get",
                        url: "../../SystemManage/DataSource/GetTableData?dbLinkId=" + data.dbId + "&tableName=" + data.dbTable,
                        async: true,
                        success: function (dataItemdata) {
                            $td.find("#" + data.id).comboBox({
                                data: dataItemdata,
                                id: data.dbFiledValue.toLowerCase(),
                                text: data.dbFiledText.toLowerCase(),
                                maxHeight: data.height,
                                allowSearch: true
                            });
                        }
                    });
                }
                return $td;
            },
            validTable: function ($obj) {
                return $obj.Validform();
            },
            getValue: function ($obj) {//获取数据保存
                var data = $obj.find('.ui-select').attr('data-value');
                var point = {
                    type: "select",
                    value: data,
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                $td.find('.ui-select').comboBoxSetValue(data.value);
            }
        },
        datetime: {//日期框
            init: function () {
                var $html = $('<div class="item_row" data-type="datetime" ><i class="fa fa-calendar"></i>日期框</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "日期框",
                    type: "datetime",
                    field: "",
                    defaultValue: "",
                    verify: "",
                    dateformat: "date"
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                var strText = (_rowJson.dateformat == 'date' ? '年-月-日' : '年-月-日 时:分');
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: strText }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>选择日期、时间控件</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">日期格式</div>';
                _html += '<div class="field_control"><select id="control_dateformat" class="form-control"><option value="date">仅日期</option><option value="datetime">日期和时间</option></select></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">默认值<i title="仅在添加数据时默认填入" class="help fa fa-question-circle"></i></div>';
                _html += '<div class="field_control"><select id="control_default" class="form-control"><option value="">无则不填</option><option value="Yesterday">昨天</option><option value="Today">今天</option><option value="Tomorrow">明天</option></select></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setValidatorTypeOnlyNotNull(itemdata);

                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_dateformat').val(itemdata.dateformat);
                $html.find('#control_default').val(itemdata.defaultValue);

                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
                $html.find('#control_dateformat').change(function (e) {
                    var value = $(this).val();
                    if (value == 'date') {
                        $itemRow.find('.item_field_value').html('年-月-日');
                    } else if (value == 'datetime') {
                        $itemRow.find('.item_field_value').html('年-月-日 时:分');
                    }
                    itemdata.dateformat = value;
                });
                $html.find('#control_default').change(function (e) {
                    var value = $(this).val();
                    itemdata.defaultValue = value;
                });
            },
            renderTable: function (data) {//使用表单的时候渲染成table
                var dateformat = data.dateformat == 'date' ? 'yyyy-MM-dd' : 'yyyy-MM-dd HH:mm';
                var datedefault = "";
                var datetime = new Date();
                switch (data.defaultValue) {
                    case "Yesterday":
                        datedefault = datetime.DateAdd('d', -1);
                        break;
                    case "Today":
                        datedefault = datetime.DateAdd('d', 0);
                        break;
                    case "Tomorrow":
                        datedefault = datetime.DateAdd('d', 1);;
                        break;
                }
                datedefault = formatDate(datedefault, dateformat.replace(/H/g, 'h'));
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td class="formValue custmerTd" data-type="' + data.type + '" data-value="' + data.field + '"   ><input value="' + datedefault + '" id="' + data.id + '"  readonly  " onClick="WdatePicker({dateFmt:\'' + dateformat + '\',qsEnabled:false,isShowClear:false,isShowOK:false,isShowToday:false})"  type="text" class="form-control input-datepicker"  ' + getTdValidatorHtml(data.verify) + ' /></td>';
                var $td = $(_html);
                return $td;
            },
            validTable: function ($obj) {
                return $obj.Validform();
            },
            getValue: function ($obj) {//获取数据保存
                var $input = $obj.find('input');
                var point = {
                    type: "datetime",
                    value: $input.val(),
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                var $input = $td.find('input');
                $input.val(data.value);
            }
        },
        image: {//图片
            init: function () {
                var $html = $('<div class="item_row" data-type="image" ><i class="fa fa-photo"></i>图片</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "上传图片",
                    type: "image",
                    field: "",
                    verify: "",
                    fileformat: "jpg,gif,png,bmp"

                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "上传图片/" + _rowJson.fileformat }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>上传图片数据</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">图片格式<i title=".jpg .gif .png .bmp" class="help fa fa-question-circle"></i></div>';
                _html += '<div class="field_control"><input id="control_fileformat" type="text" class="form-control" placeholder="如：jpg,gif,png,bmp"></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setValidatorTypeOnlyNotNull(itemdata);

                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_fileformat').val(itemdata.fileformat);
                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });

                $html.find('#control_fileformat').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_value').html('上传图片/' + value);
                    itemdata.fileformat = value;
                });
            },
            renderTable: function (data) {
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td class="formValue custmerTd" data-name="' + data.label + '"  data-type="' + data.type + '"  data-value="' + data.field + '"  data-verify="' + data.verify + '"  ><div id="' + data.id + '"></div></td>';
                var $td = $(_html);
                $td[0].callback = {
                    data: data,
                    fn: function () {
                        $td.find("#" + data.id).uploadifyEx({
                            url: "/Utility/UploadifyFile?DataItemEncode=SaveFilePath&DataItemName=FormFilePath",
                            btnName: "添加图片",
                            type: "uploadify",
                            fileTypeExts: data.fileformat
                        });
                    }
                };
                return $td;
            },
            makeCallback: function (callbackList) {
                $.each(callbackList, function (i, item) {
                    item.fn();
                });
            },
            validTable: function ($obj) {
                var verify = $obj.attr("data-verify");
                if (verify == "NotNull") {
                    var data = $obj.find('.uploadify').attr('data-value');
                    if (data != "" && data != undefined && data != null) {
                        return true;
                    }
                    var name = $obj.attr("data-name");
                    learun.dialogTop({ msg: name + "不能为空！", type: "error" });
                    return false;
                }
                else {
                    return true;
                }
            },
            getValue: function ($obj) {//获取数据保存
                var data = $obj.find('.uploadify').attr('data-value');
                var point = {
                    type: "image",
                    value: data,
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                $td.find('.uploadify').uploadifyExSet(data.value);
            }
        },
        upload: {//附件
            init: function () {
                var $html = $('<div class="item_row" data-type="upload" ><i class="fa fa-paperclip"></i>附件</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "上传文件",
                    type: "upload",
                    field: "",
                    verify: "",
                    fileformat: "doc,xls,ppt,pdf"
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "上传文件/" + _rowJson.fileformat }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>上传文件数据</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">文件格式<i title=".doc .xls .ppt .pdf " class="help fa fa-question-circle"></i></div>';
                _html += '<div class="field_control"><input id="control_fileformat" type="text" class="form-control" placeholder="如：doc,xls,ppt,pdf"></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setValidatorTypeOnlyNotNull(itemdata);

                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_fileformat').val(itemdata.fileformat);

                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
                $html.find('#control_fileformat').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_value').html('上传文件/' + value);
                    itemdata.fileformat = value;
                });
            },
            renderTable: function (data) {
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td class="formValue custmerTd" data-name="' + data.label + '"  data-type="' + data.type + '"  data-value="' + data.field + '"  data-verify="' + data.verify + '"   ><div id="' + data.id + '"></div></td>';
                var $td = $(_html);
                $td[0].callback = {
                    data: data,
                    fn: function () {
                        $td.find("#" + data.id).uploadifyEx({
                            url: "/Utility/UploadifyFile?DataItemEncode=SaveFilePath&DataItemName=FormFilePath",
                            btnName: "添加附件",
                            type: "uploadify",
                            fileTypeExts: data.fileformat
                        });
                    }
                };
                return $td;
            },
            makeCallback: function (callbackList) {
                $.each(callbackList, function (i, item) {
                    item.fn();
                });
            },
            validTable: function ($obj) {
                var verify = $obj.attr("data-verify");
                if (verify == "NotNull") {
                    var data = $obj.find('.uploadify').attr('data-value');
                    if (data != "" && data != undefined && data != null) {
                        return true;
                    }
                    var name = $obj.attr("data-name");
                    learun.dialogTop({ msg: name + "不能为空！", type: "error" });
                    return false;
                }
                else {
                    return true;
                }
            },
            getValue: function ($obj) {//获取数据保存
                var data = $obj.find('.uploadify').attr('data-value');
                var point = {
                    type: "upload",
                    value: data,
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                $td.find('.uploadify').uploadifyExSet(data.value);
            }
        },
        baseSelect: {//单位组织
            init: function () {
                var $html = $('<div class="item_row" data-type="baseSelect" ><i  class="fa fa-coffee"></i>单位组织</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "单位组织",
                    type: "baseSelect",
                    field: "",
                    verify: "",
                    baseType: "user",
                    relation: "",
                    height: "100px"
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                var strText = "";
                switch (_rowJson.baseType) {
                    case "user":
                        strText = "人员选择";
                        break;
                    case "department":
                        strText = "部门选择";
                        break;
                    case "organize":
                        strText = "公司选择";
                        break;
                    case "post":
                        strText = "岗位选择";
                        break;
                    case "job":
                        strText = "职位选择";
                        break;
                    case "role":
                        strText = "角色选择";
                        break;
                }
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "单位组织/" + strText }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>单位组织下拉选择框,支持级联</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">下拉高度</div>';
                _html += '<div class="field_control"><input id="control_height" type="text" class="form-control" value="100px"></div>';
                _html += '<div class="field_title">类型选择</div>';
                _html += '<div class="field_control"><select id="control_baseType" class="form-control"><option value="user">人员选择</option><option value="department">部门选择</option><option value="organize">公司选择</option><option value="post">岗位选择</option><option value="job">职位选择</option><option value="role">角色选择</option></select></div>';
                _html += '<div class="field_title">单位组织控件级联-上一级</div>';
                _html += '<div class="field_control"><div id="control_relation" type="select" class="ui-select"></div></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                var baseInfoRelation = [];
                $.each($itemRow[0].parentNode.childNodes, function (i, item) {
                    var data = $(item)[0].itemdata;
                    if (data.id != itemdata.id && itemdata.type == 'baseSelect') {
                        if (itemdata.baseType == "user" || itemdata.baseType == "job")//向上级联部门
                        {
                            if (data.baseType == "department") {
                                baseInfoRelation.push(data);
                            }
                        }
                        else if (itemdata.baseType != "organize") {
                            if (data.baseType == "organize") {
                                baseInfoRelation.push(data);
                            }
                        }
                    }
                });

                setControlField(opt, itemdata);
                setValidatorTypeOnlyNotNull(itemdata);
                setBaseInfoRelation(itemdata, baseInfoRelation);

                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_baseType').val(itemdata.baseType);
                $html.find('#control_height').val(itemdata.height);
                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
                $html.find('#control_baseType').change(function (e) {
                    var value = $(this).val();
                    var text = $(this).find('[value="' + value + '"]').text();
                    itemdata.baseType = value;
                    $itemRow.find('.item_field_value').html('单位组织/' + text);
                });
                $html.find('#control_height').change(function (e) {
                    var value = $(this).val();
                    itemdata.height = value;
                });
            },
            renderTable: function (data) {
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td class="formValue custmerTd" data-type="' + data.type + '"  data-value="' + data.field + '"  ><div id="' + data.id + '" type="select" class="ui-select"  ' + getTdValidatorHtml(data.verify) + ' ></div></td>';
                var $td = $(_html);
                var $select = $td.find('#' + data.id);
                $td[0].callback = {
                    data: data,
                    fn: null
                }
                switch (data.baseType) {
                    case "user"://用户
                        $td[0].callback.fn = function (param) {
                            var query = "";
                            var $obj;
                            if (!!param) {
                                query = "?departmentId=" + param;
                            }
                            if (!!data.relation && query == "") {
                                $obj = $select.comboBox({
                                    allowSearch: true
                                });
                            }
                            else {
                                $obj = $select.comboBox({
                                    url: "../../BaseManage/User/GetListJson" + query,
                                    id: "F_UserId",
                                    text: "F_RealName",
                                    title: "F_Account",
                                    maxHeight: data.height,
                                    allowSearch: true
                                });
                            }
                            return $obj;
                        }
                        break;
                    case "department"://部门
                        $td[0].callback.fn = function (param) {
                            var query = "";
                            var $obj;
                            if (!!param) {
                                query = "?organizeId=" + param;
                            }
                            if (!!data.relation && query == "") {
                                $obj = $select.comboBoxTree({
                                    allowSearch: true
                                });
                            }
                            else {
                                $obj = $select.comboBoxTree({
                                    url: "../../BaseManage/Department/GetTreeJson" + query,
                                    maxHeight: data.height,
                                    allowSearch: true
                                });
                            }
                            return $obj;
                        };
                        break;
                    case "organize"://公司
                        $td[0].callback.fn = function (param) {
                            var $obj = $select.comboBoxTree({
                                url: "../../BaseManage/Organize/GetTreeJson",
                                maxHeight: data.height,
                                allowSearch: true
                            });
                            return $obj;
                        };
                        break;
                    case "post"://岗位
                        $td[0].callback.fn = function (param) {
                            var query = "";
                            var $obj;
                            if (!!param) {
                                query = "?organizeId=" + param;
                            }
                            if (!!data.relation && query == "") {
                                $obj = $select.comboBox({
                                    allowSearch: true
                                });
                            }
                            else {
                                $obj = $select.comboBox({
                                    url: "../../BaseManage/Post/GetListJson" + query,
                                    id: "F_RoleId",
                                    text: "F_FullName",
                                    maxHeight: data.height,
                                    allowSearch: true
                                });
                            }
                            return $obj;
                        };
                        break;
                    case "job"://职位
                        $td[0].callback.fn = function (param) {
                            var query = "";
                            var $obj;
                            if (!!param) {
                                query = "?organizeId=" + param;
                            }
                            if (!!data.relation && query == "") {
                                $obj = $select.comboBox({
                                    allowSearch: true
                                });
                            }
                            else {
                                $obj = $select.comboBox({
                                    url: "../../BaseManage/Job/GetListJson" + query,
                                    id: "F_RoleId",
                                    text: "F_FullName",
                                    maxHeight: data.height,
                                    allowSearch: true
                                });
                            }
                            return $obj;
                        };
                        break;
                    case "role"://角色
                        $td[0].callback.fn = function (param) {
                            var query = "";
                            var $obj;
                            if (!!param) {
                                query = "?organizeId=" + param;
                            }
                            if (!!data.relation && query == "") {
                                $obj = $select.comboBox({
                                    allowSearch: true
                                });
                            }
                            else {
                                $obj = $select.comboBox({
                                    url: "../../BaseManage/Role/GetListJson" + query,
                                    id: "F_RoleId",
                                    text: "F_FullName",
                                    maxHeight: data.height,
                                    allowSearch: true
                                });
                            }
                            return $obj;
                        };
                        break;
                }
                return $td;
            },
            makeCallback: function (callbackList) {//处理各个组织单位间的级联关系
                var $objList = {};
                $.each(callbackList, function (i, item) {//初始化各个控件
                    if (!$objList[i]) {
                        $objList[i] = {
                            obj: "",
                            fnlist: []
                        };
                    }
                    if (!!item.data.relation) {
                        if (!$objList[item.data.relation]) {
                            $objList[item.data.relation] = {
                                obj: "",
                                fnlist: []
                            };
                        }
                        $objList[item.data.relation].fnlist.push(item.fn);
                    }
                    $objList[i].obj = item.fn();
                });
                //绑定级联
                $.each($objList, function (i, item) {
                    if (item.fnlist.length > 0) {
                        item.obj.bind('change', function () {
                            var value = $(this).attr('data-value');
                            $.each(item.fnlist, function (i, fn) {
                                fn(value);
                            });

                        });
                    }
                });
            },
            validTable: function ($obj) {
                return $obj.Validform();
            },
            getValue: function ($obj) {//获取数据保存
                var data = $obj.find('.ui-select').attr('data-value');
                var point = {
                    type: "baseSelect",
                    value: data,
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                $td.find('.ui-select').comboBoxSetValue(data.value);
            }
        },
        sysAreas: {//行政区划
            init: function () {
                var $html = $('<div class="item_row" data-type="sysAreas" ><i class="fa fa-caret-square-o-right"></i>行政区划</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "行政区划",
                    type: "sysAreas",
                    field: "",
                    verify: "",
                    provinceId: "",
                    relation: "",
                    height: "100px"
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "行政区划" }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">字段验证</div>';
                _html += '<div class="field_control"><div id="control_verify" type="select" class="ui-select"></div>';
                _html += '<div class="field_title">选择省级</div>';
                _html += '<div class="field_control"><div id="control_dataSourceProvince" type="select" class="ui-select"></div></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);
                setValidatorTypeOnlyNotNull(itemdata);
                setDistrict(itemdata);
                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_height').val(itemdata.height);

                $html.find('#control_height').change(function (e) {
                    var value = $(this).val();
                    itemdata.height = value;
                });
                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
            },
            renderTable: function (data) {
                var _tempCode = "";
                if (data.provinceId == "") {
                    _tempCode = "00000" + _flagCode;
                    _flagCode++;
                }
                else {
                    _tempCode = data.provinceId;
                }
                var _html = '<th class="formTitle">' + data.label + getFontHtml(data.verify) + '</th>';
                _html += '<td colspan="2" class="formValue custmerTd" data-type="' + data.type + '"  data-value="' + data.field + '"  ><div style="float: left; "><div id="F_ProvinceId' + _tempCode + '" type="select" class="ui-select" style="float: left;width: 84px;  margin-right: 1px;"></div><div id="F_CityId' + _tempCode + '" type="select" class="ui-select" style="float: left;width: 84px;  margin-right: 1px;"></div><div id="F_CountyId' + _tempCode + '" type="select" class="ui-select" style="float: left;width: 84px; width: 84px; margin-right: 1px;"></div></div><div style="float:left;right:5px; width: auto; margin-left: 1px;"><input id="F_Address' + _tempCode + '" type="text" class="form-control" style="width:57%;" /></div></td>';
                var $td = $(_html);
                var $select = $td.find('#' + data.id);
                $td[0].callback = {
                    data: data,
                    fn: null
                }
                $td[0].callback.fn = function () {
                    //省份
                    $("#F_ProvinceId" + _tempCode).ComboBox({
                        url: "../../SystemManage/Area/GetAreaJson",
                        param: { areaId: data.provinceId },
                        id: "F_AreaCode",
                        text: "F_AreaName",
                        description: "选择省",
                        height: "170px"
                    }).bind("change", function () {
                        var value = $(this).attr('data-value');
                        $("#F_CityId" + _tempCode).ComboBox({
                            url: "../../SystemManage/Area/GetAreaListJson",
                            param: { parentId: value },
                            id: "F_AreaCode",
                            text: "F_AreaName",
                            description: "选择市",
                            height: "170px"
                        });
                    });
                    //城市
                    $("#F_CityId" + _tempCode).ComboBox({
                        description: "选择市",
                        height: "170px"
                    }).bind("change", function () {
                        var value = $(this).attr('data-value');
                        if (value) {
                            $("#F_CountyId" + _tempCode).ComboBox({
                                url: "../../SystemManage/Area/GetAreaListJson",
                                param: { parentId: value },
                                id: "F_AreaCode",
                                text: "F_AreaName",
                                description: "选择县/区",
                                height: "170px"
                            });
                        }
                    });
                    //县/区
                    $("#F_CountyId" + _tempCode).ComboBox({
                        description: "选择县/区",
                        height: "170px"
                    });
                };
                return $td;
            },
            makeCallback: function (callbackList) {//处理各个组织单位间的级联关系
                var $objList = {};
                $.each(callbackList, function (i, item) {//初始化各个控件
                    if (!$objList[i]) {
                        $objList[i] = {
                            obj: "",
                            fnlist: []
                        };
                    }
                    if (!!item.data.relation) {
                        if (!$objList[item.data.relation]) {
                            $objList[item.data.relation] = {
                                obj: "",
                                fnlist: []
                            };
                        }
                        $objList[item.data.relation].fnlist.push(item.fn);
                    }
                    $objList[i].obj = item.fn();
                });
            },
            validTable: function ($obj) {
                return $obj.Validform();
            },
            getValue: function ($obj) {//获取数据保存
                var value = "";
                $obj.find('.ui-select').each(function (i) {
                    if (value != "") {
                        value += ",";
                    }
                    value += $(this).attr('data-value');
                });
                var txt = $obj.find('input').val();
                if (value != "") {
                    value += "," + txt;
                }
                var point = {
                    type: "sysAreas",
                    value: value,
                    field: $obj.attr('data-value')
                };
                return point;
            },
            setValue: function ($td, data) {//设置数据
                var valueList = data.value.split(',');
                $td.find('.ui-select').each(function (i) {
                    $(this).comboBoxSetValue(valueList[i]);
                });
                var $input = $td.find('input');
                $input.val(valueList[3]);
            }
        },
        currentInfo: {//当前的信息
            init: function () {
                var $html = $('<div class="item_row" data-type="currentInfo" ><i  class="fa fa-book"></i>当前信息</div>');
                return $html;
            },
            render: function ($itemRow) {
                var _rowJson = $.extend({
                    label: "当前信息",
                    type: "currentInfo",
                    field: "",
                    infoType: "user",
                }, $itemRow[0].itemdata);
                $itemRow[0].itemdata = _rowJson;
                var strText = "";
                switch (_rowJson.infoType) {
                    case "user":
                        strText = "当前用户";
                        break;
                    case "department":
                        strText = "当前部门";
                        break;
                    case "organize":
                        strText = "当前公司";
                        break;
                    case "date":
                        strText = "当前时间";
                        break;
                }
                $itemRow.html(getComponentRowHtml({ name: _rowJson.label, text: "当前信息/" + strText }));
            },
            property: function (opt, $itemRow) {
                var $html = $(".field_option");
                var _html = '';
                _html += '<div class="field_tips"><i class="fa fa-info-circle"></i><span>显示当前操作信息</span></div>';
                _html += '<div class="field_title">字段标识</div>';
                _html += '<div class="field_control"><div id="control_field" type="select" class="ui-select"></div></div>';
                _html += '<div class="field_title">字段说明</div>';
                _html += '<div class="field_control"><input id="control_label" type="text" class="form-control" placeholder="必填项"></div>';
                _html += '<div class="field_title">类型选择</div>';
                _html += '<div class="field_control"><select id="control_infoType" class="form-control"><option value="user">当前用户</option><option value="department">当前部门</option><option value="organize">当前公司</option><option value="date">当前时间</option></select></div>';
                $html.html(_html);
                var itemdata = $itemRow[0].itemdata;
                setControlField(opt, itemdata);

                $html.find('#control_label').val(itemdata.label);
                $html.find('#control_infoType').val(itemdata.infoType);


                $html.find('#control_label').keyup(function (e) {
                    var value = $(this).val();
                    $itemRow.find('.item_field_label').find('span').html(value);
                    itemdata.label = value;
                });
                $html.find('#control_infoType').change(function (e) {
                    var value = $(this).val();
                    var text = $(this).find('[value="' + value + '"]').text();
                    itemdata.infoType = value;
                    $itemRow.find('.item_field_value').html('当前信息/' + text);
                });
            },
            renderTable: function (data) {
                var _html = '<th class="formTitle">' + data.label + '</th>';
                _html += '<td class="formValue custmerTd" data-type="' + data.type + '"  data-value="' + data.field + '"  data-infoType="' + data.infoType + '"  ><input id="' + data.id + '"  readonly type="text" class="form-control"  /></td>';
                var $td = $(_html);
                var $input = $td.find('input');
                if (!!qcurrentData) {
                    switch (data.infoType) {
                        case "user":
                            $input.val(qcurrentData.userName);
                            $input.attr('data-value', qcurrentData.userId);
                            break;
                        case "department":
                            $input.val(top.learun.data.get(["department", qcurrentData.departmentId, "FullName"]));
                            $input.attr('data-value', qcurrentData.departmentId);
                            break;
                        case "organize":
                            $input.val(top.learun.data.get(["organize", qcurrentData.companyId, "FullName"]));
                            $input.attr('data-value', qcurrentData.companyId);
                            break;
                        case "date":
                            $input.val(qcurrentData.time);
                            break;
                    }
                }
                else {
                    learun.getDataForm({
                        type: "get",
                        url: "../../Utility/getCurrentInfo",
                        success: function (currentData) {
                            qcurrentData = currentData;
                            switch (data.infoType) {
                                case "user":
                                    $input.val(currentData.userName);
                                    $input.attr('data-value', currentData.userId);
                                    break;
                                case "department":
                                    $input.val(top.learun.data.get(["department", currentData.departmentId, "FullName"]));
                                    $input.attr('data-value', currentData.departmentId);
                                    break;
                                case "organize":
                                    $input.val(top.learun.data.get(["organize", currentData.companyId, "FullName"]));
                                    $input.attr('data-value', currentData.companyId);
                                    break;
                                case "date":
                                    $input.val(currentData.time);
                                    break;
                            }
                        }
                    });
                }
                return $td;
            },
            validTable: function ($obj) {
                return true;
            },
            getValue: function ($obj) {//获取数据保存
                var $input = $obj.find('input');
                var point = {
                    type: "currentInfo",
                    value: $input.attr('data-value'),
                    field: $obj.attr('data-value'),
                    infoType: $obj.attr('data-infoType')
                };
                if (point.infoType == "date") {
                    point.value = $input.val();
                }
                return point;
            },
            setValue: function ($td, data) {//设置数据


                var $input = $td.find('input');
                switch (data.infoType) {
                    case "user":
                        $input.val(top.learun.data.get(['user', data.value, "RealName"]));
                        $input.attr('data-value', data.value);
                        break;
                    case "department":
                        $input.val(top.learun.data.get(["department", data.value, "FullName"]));
                        $input.attr('data-value', data.value);
                        break;
                    case "organize":
                        $input.val(top.learun.data.get(["organize", data.value, "FullName"]));
                        $input.attr('data-value', data.value);
                        break;
                    case "date":
                        $input.val(data.value);
                        break;
                }
            }
        }
    }
})(window.jQuery, window.learun);