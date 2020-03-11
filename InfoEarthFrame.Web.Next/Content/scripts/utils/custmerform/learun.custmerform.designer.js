/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 表单设计器
 * 陈小二
 * 6.1.2.0 支持多列布局，下拉，单选，多选关联数据字典和数据表，增加当前信息和组织单位控件，组织单位支持级联
 */
/*传入数据字段
 *height------------------高度
 *width-------------------宽度
 *type--------------------表单类型
 *dbId--------------------关联的库Id
 *dbTable-----------------关联的表名
 *dbFields-----------------关联数据库表字段
 *data:-------------------数据
 *isValid-----------------获取数据是否验证
*/
/*区域布局字段
 *id----------------------id
 *type--------------------类型：1列，2列。。。
 *margintop---------------向上间隔
 *sortCode----------------排序
*/
/*返回数据格式
 *area：{}
 *fields：[{itemrow},{itemrow}]
*/
(function ($, learun) {
    "use strict";

    var qopt = {
        height: 0,
        width:0,
        type: 0//默认不建表
    };
    var custmerform = {
        init: function (opt, $this) {
            var opt = $.extend(qopt, opt);
            qopt = opt;
            $this.html('<div class="filedLayout"></div><div class="areaLayout"></div><div class="previewLayout"></div>');
            var $areaLayout = $this.find('.areaLayout');
            var $filedLayout = $this.find('.filedLayout');
            var $previewLayout = $this.find('.previewLayout');
            custmerform.previewLayout.init(opt, $this);
            custmerform.areaLayout.init(opt, $areaLayout);
            custmerform.filedLayout.init(opt, $filedLayout);

            $previewLayout.find('.previewClosed i').trigger('click');
            $areaLayout.find('.btn_toFiledLayout').trigger('click');//切换到字段布局
        },
        get: function ($this, isValid) {//
            custmerform.filedLayout.getCurrentPanelData();//切换前保存数据
            var data = $this.find('.filedLayout .topToolBar .areaSelect')[0].formData;
            var resdata = [];
            var flag = true;
            var fieldbuff = {}, fieldsNum = 0;
            $.each(data, function (i, item) {
                if (!!isValid) {//如果需要验证
                    fieldsNum += item.fields.length;
                    $.each(item.fields, function (j, itemFiled) {
                        if (!!itemFiled.field) {
                            if (!!fieldbuff[itemFiled.field]) {
                                learun.dialogTop({ msg: "第" + (item.area.sortCode + 1) + "区域,字段【" + itemFiled.label + "】绑定数据库字段【" + itemFiled.field + "】有重复！", type: "error" });
                                flag = false;
                            }
                            else {
                                fieldbuff[itemFiled.field] = 1;
                            }
                        }
                        else {
                            learun.dialogTop({ msg: "第" + (item.area.sortCode + 1) + "区域,字段【" + itemFiled.label + "】没有绑定数据库字段！", type: "error" });
                            flag = false;
                        }
                        if (!flag) {
                            return false;
                        }
                    });
                }
                else {
                    fieldsNum = 1;
                }
                if (!flag) {
                    return false;
                }
                
                item.area = {
                    id: item.area.id,
                    type: item.area.type,
                    margintop: item.area.margintop,
                    sortCode: item.area.sortCode
                }
                resdata.push(item);
            });
            if (!flag) {//需要验证的情况下，如果字段没有绑定好，返回失败
                return flag;
            }
            if (fieldsNum == 0) {
                learun.dialogTop({ msg: "请布局字段！", type: "error" });
                return false;
            }
            return resdata;
        },
        updateFilds: function ($this, opt) {
            if (qopt.dbId != opt.dbId || qopt.dbTable != opt.dbTable) {
                qopt.dbId = opt.dbId;
                qopt.dbTable = opt.dbTable;
                qopt.dbFields = opt.dbFields;
                custmerform.filedLayout.getCurrentPanelData();//保存当前数据
                var data = $this.find('.filedLayout .topToolBar .areaSelect')[0].formData;
                $.each(data, function (i, item) {
                    $.each(item.fields, function (j, fieldItem) {
                        fieldItem.field = "";
                    });
                });
                $($this.find('#app_layout_list .item_row')[0]).trigger('click');
            }
            else if (qopt.dbFields.length == 0) {
                qopt.dbFields = opt.dbFields;
                $($this.find('#app_layout_list .item_row')[0]).trigger('click');
            }
        },
        previewLayout: {
            init: function (opt,$this) {
                var $previewLayout = $this.find('.previewLayout');
                $previewLayout.html('<div class="previewClosed"><i class="fa fa-close" title="关闭预览页" ></i></div><div id="custmerFormPreview" class="showPanel"></div>');
                $previewLayout.height(opt.height);
                $previewLayout.width(opt.width);
                $previewLayout.find('.showPanel').height(opt.height-21);
                $previewLayout.find('.previewClosed i').on('click', function () {
                    $previewLayout.animate({ opacity: 0, top: opt.height, 'z-index': -2000, speed: 2000 });
                });
            },
            rendering: function (data, $this) {//渲染预览的表单
                var $formShowPanel = $this.find('.previewLayout .showPanel');
                $formShowPanel.formRendering('init', { formData: data });
            }
        },
        areaLayout: {//表单区域布局
            init: function (opt, $areaLayout) {
                var layout = {
                    init: function (opt) {
                        var _html = '<div class="form_layout_panel">';
                        _html += '<div class="leftbody">';
                        _html += '<div class="topToolBar">';
                        _html += '<div class="btn-group">';
                        _html += '<a class="btn btn_areaAdd"><i class="fa fa-plus"></i>&nbsp;添加区域</a>';
                        _html += '<a class="btn btn_toFiledLayout"><i class="fa fa-list"></i>&nbsp;字段布局</a>';
                        _html += '</div>';
                        _html += '</div>';
                        _html += '<div class="showBar" ></div>';
                        _html += '</div>';
                        _html += '<div class="rightbody">';
                        _html += '<div class="set_title"><i class="fa fa-info-circle"></i><span>设置区域的属性</span></div>';
                        _html += '<div class="attr_title">显示方式</div>';
                        _html += '<div class="attr_control"><select class="form-control showType"><option value="1">1列</option><option value="2">2列</option><option value="3">3列</option><option value="4">4列</option><option value="5">5列</option><option value="6">6列</option></select></div>';
                        _html += '<div class="attr_title">间隔上一区域(px)</div>';
                        _html += '<div class="attr_control"><input type="number" class="form-control margintop" value="0" /></div>';
                        _html += '</div>';
                        _html += '</div>';

                        $areaLayout.html(_html);
                        $areaLayout.height(opt.height);
                        $areaLayout.width(opt.width);
                        $areaLayout.find('.form_layout_panel').height(opt.height);
                        $areaLayout.find('.leftbody').width(opt.windowW - 240);
                        $areaLayout.find('.showBar').height(opt.height - 30);
                        $areaLayout.find('.rightbody').height(opt.height);

                        $areaLayout.find('.btn_areaAdd').unbind();
                        $areaLayout.find('.btn_areaAdd').on('click', layout.addbtn);

                        $areaLayout.find('.btn_toFiledLayout').unbind();
                        $areaLayout.find('.btn_toFiledLayout').on('click', layout.totoFiledLayout);

                        layout.initAttributePanel();

                        if (!!opt.data) {
                            $.each(opt.data, function (i, item) {
                                layout.addArea(item.area);
                            });
                        }
                        if (!($areaLayout.find('.areaItem').length != 0)) {
                            $areaLayout.find('.btn_areaAdd').trigger('click');//添加一个区域
                        }
                    },
                    initAttributePanel: function () {
                        if (typeof learun === 'undefined') {
                            return false;
                        }
                        $areaLayout.find('.rightbody select,.rightbody input').bind('input propertychange', function () {
                            var $obj = $(this);
                            var _value = $obj.val();
                            var _type = $obj.hasClass('showType');
                            var $areaItem = $('#' + $areaLayout.find('.rightbody').attr('data-value'));
                            if (!!_value) {
                                if (_type) {
                                    $areaItem[0].areaItem.type = _value;
                                    $areaItem.find(".typeValue").html(_value);
                                }
                                else {
                                    $areaItem[0].areaItem.margintop = _value;
                                    $areaItem.find(".margintopValue").html(_value);
                                }
                            }
                        });
                    },
                    renderingAttributePanel: function (data, id) {//刷新属性数据
                        $areaLayout.find('.showType').val(data.type);
                        $areaLayout.find('.margintop').val(data.margintop);
                        $areaLayout.find('.rightbody').attr('data-value', id);
                    },
                    addbtn: function () {//增加区域
                        layout.addArea($areaLayout, {});
                    },
                    minusbtn: function () {//删除区域
                        var $areaItem = $(this).parents('.areaItem');
                        var list = $areaItem.parent().find('.areaItem');
                        if (list.length == 1) {
                            learun.dialogTop({ msg: "必须保留一个区域", type: "error" });
                        }
                        else {
                            if ($areaItem.hasClass('active')) {
                                var $other = $(list[0]);
                                if ($other.attr('id') == $areaItem.attr('id')) {
                                    var $other = $(list[1]);
                                }
                                $other.trigger('click');
                            }
                            $areaItem.remove();
                        }
                    },
                    clickItem: function () {//点击区域
                        var $this = $(this);
                        if (typeof learun === 'undefined') {
                            return false;
                        }
                        var id = $this.attr("id");
                        if (!$this.hasClass('active')) {
                            $areaLayout.find('.showBar').find('.areaItem').removeClass('active');
                            $this.addClass('active');
                            layout.renderingAttributePanel($this[0].areaItem, id);
                        }
                        else {
                            layout.renderingAttributePanel($this[0].areaItem, id);
                        }
                    },
                    totoFiledLayout: function () {
                        $areaLayout.animate({ opacity: 0, top: opt.height, 'z-index': -2000, speed: 2000 });
                        layout.refreshAreaData($areaLayout);
                    },
                    refreshAreaData: function ($areaLayout) {//刷新布局区域
                        var $select = $areaLayout.parent().find('.filedLayout .topToolBar .areaSelect');
                        var oldId = $select.val();
                        var oldData = {};
                        if ($select[0].formData == undefined) {
                            $select[0].formData = {};
                        }
                        else {
                            oldData = $.extend(true, {}, $select[0].formData);
                            $select[0].formData = {};
                        }
                        
                        var selectHtml = '';
                        var currentId;
                        $select.unbind();
                        $areaLayout.find('.areaItem').each(function (i) {//获取布局区域数据
                            var point = $(this)[0].areaItem;
                            if (i == 0) {
                                currentId = point.id;
                            }
                            point.sortCode = i;
                            selectHtml += '<option value="' + point.id + '">第' + (i + 1) + '区域(' + point.type + '列/向上间隔' + point.margintop + 'px)</option>';
                            $select[0].formData[point.id] = {
                                area: point,
                                fields: []
                            };
                            if (oldData[point.id] != undefined) {
                                $select[0].formData[point.id].fields = oldData[point.id].fields;
                                if (oldId == point.id) {
                                    currentId = oldId;
                                }
                            }
                        });
                        $select.html(selectHtml);
                        $select.on('change', custmerform.filedLayout.changeArea);
                        $select.val(currentId);
                        $select.trigger('change');
                    },
                    addArea: function (data) {
                        var _data = $.extend({
                            type: "1",
                            margintop: 0,
                            id: learun.createGuid()
                        }, data);
                        var $areaItem = $('<div class="areaItem active"  id="' + _data.id + '" >表单布局区域（<span class="typeValue">' + _data.type + '</span>列/向上间隔<span class="margintopValue">' + _data.margintop + '</span>px）<div class="areaItem_remove"><i title="移除区域" class="fa fa-close"></i></div></div>');
                        $areaItem[0].areaItem = _data;

                        layout.renderingAttributePanel($areaItem[0].areaItem, _data.id);

                        $areaItem.on('click', layout.clickItem);
                        $areaItem.find('.fa-close').on('click', layout.minusbtn);

                        $areaLayout.find('.showBar').find('.areaItem').removeClass('active');
                        $areaLayout.find('.showBar').append($areaItem);
                    }
                };
                layout.init(opt);
            },
            refreshShow: function () {//刷新区域显示
                $('.areaLayout .areaItem').each(function (i) {//获取布局区域数据
                    var point = $(this)[0].areaItem;
                    $(this).find('.typeValue').text(point.type);
                });
                $($('.areaLayout .areaItem')[0]).trigger('click');
            }
        },
        filedLayout: {//表单字段设计
            areaType: 1,
            init: function (opt, $filedLayout) {
                var layout = {
                    init: function ($filedLayout) {
                        layout.renderHtml($filedLayout);
                        layout.initHeight($filedLayout, opt);
                        layout.componentInit();

                        if (!!opt.data) {
                            var $select = $filedLayout.find(' .topToolBar .areaSelect');
                            $select[0].formData = {};
                            $.each(opt.data, function (i, item) {
                                $select[0].formData[item.area.id] = item;
                                $filedLayout.find(".guideareas").hide();
                            });
                        }
                    },
                    renderHtml: function ($filedLayout) {
                        var layoutHtml = '<div class="app_body"><div id="move_item_list" class="app_field"></div>';
                        //区域选择面板
                        layoutHtml += '<div class="topToolBar" ><select class="areaSelect"></select><select class="typeSelect" ><option value="1">1列</option><option value="2">2列</option><option value="3">3列</option><option value="4">4列</option><option value="5">5列</option><option value="6">6列</option></select><a id="leaAreaLayout" class="btn"><i class="fa fa-th"></i>&nbsp;区域布局</a><a id="leaformPreview" class="btn"><i class="fa fa-eye"></i>&nbsp;整体预览</a></div>';
                        //字段面板
                        layoutHtml += '<div id="app_layout_list" class="item_table connectedSortable"></div>';
                        //属性设置面板
                        layoutHtml += '<div id="app_layout_option" class="field_option"></div><div class="guideareas"></div>';
                        layoutHtml += '</div>';
                        $filedLayout.html(layoutHtml);
                        $('#leaAreaLayout').on('click', layout.switchToAreaLayout);
                        $filedLayout.find('.topToolBar .typeSelect').on('click', layout.changeType);
                        $('#leaformPreview').on('click', layout.previewForm);
                    },
                    changeType: function () {//改变显示的列数
                        var type = $(this).val();
                        var $select = $(this).parent().find('.areaSelect');
                        var currentId = $select.val();
                        var areaData = $select[0].formData[currentId].area;
                        if (areaData.type != type)
                        {
                            areaData.type = type;
                            $select.find('[value="' + currentId + '"]').html('第' + (areaData.sortCode + 1) + '区域(' + areaData.type + '列/向上间隔' + areaData.margintop + 'px)');;
                            $select.trigger('change');
                        }
                    },
                    switchToAreaLayout: function () {//切换到布局界面
                        custmerform.filedLayout.getCurrentPanelData();//切换前保存数据
                        $filedLayout.parent().find(".areaLayout").animate({ opacity: 1, top: 46, 'z-index': 2000, speed: 2000 });
                        custmerform.areaLayout.refreshShow();
                    },
                    previewForm: function () {//预览表单
                        //获取表单数据
                        //custmerform.filedLayout.getCurrentPanelData();//获取数据前保存下当前设计页的数据
                        var data = custmerform.get($filedLayout.parent());
                        custmerform.previewLayout.rendering(data, $filedLayout.parent());
                        $filedLayout.parent().find(".previewLayout").animate({ opacity: 1, top: 46, 'z-index': 2000, speed: 2000 });
                    },
                    initHeight: function ($filedLayout, opt) {
                        $filedLayout.find(".app_body").height(opt.height);
                        $filedLayout.find(".field_option").height(opt.height - 44).css('right',-240);
                        $filedLayout.find(".guideareas").height(opt.height - 63);
                        $filedLayout.find(".item_table").css("height", opt.height - 30);
                    },
                    componentInit: function () {
                        //初始化控件
                        $.each(learun.components, function (i, item) {
                            var $item = item.init();
                            $filedLayout.find('#move_item_list').append($item);
                        });

                        $("#move_item_list .item_row").draggable({
                            connectToSortable: "#app_layout_list",
                            helper: "clone",
                            revert: "invalid"
                        });
                        $("#app_layout_list").sortable({
                            opacity: 0.4,
                            delay: 300,
                            cursor: 'move',
                            placeholder: "ui-state-highlight",
                            stop: function (event, ui) {
                                var randomId = learun.createGuid();
                                var $itemRow = $(ui.item[0]);
                                var controltype = $itemRow.attr('data-type');
                                if (!!controltype) {//如果是第一次移入，需要对单元项进行初始化处理
                                    $itemRow.css({ width: ((100 / custmerform.filedLayout.areaType) + "%"), float: "left" });
                                    $itemRow.removeAttr('data-type');
                                    $itemRow[0].itemdata = {
                                        id:randomId
                                    }
                                    learun.components[controltype].render($itemRow);
                                    $itemRow.unbind('click');
                                    $itemRow.click(custmerform.filedLayout.itemRowClick);
                                    $itemRow.find('.item_field_remove i').unbind('click');
                                    $itemRow.find('.item_field_remove i').click(custmerform.filedLayout.itemRowRemoveClick);
                                    $itemRow.trigger("click");
                                }
                                else {
                                    $itemRow.trigger("click");
                                }
                            },
                            start: function (event, ui) {
                                $filedLayout.find(".guideareas").hide();
                                
                                $filedLayout.find(".ui-state-highlight").html('拖放控件到这里');
                                $filedLayout.find(".ui-state-highlight").css({ width: ((100 / custmerform.filedLayout.areaType) + "%"), float: "left" });
                                $filedLayout.find("#app_layout_list .item_row").removeClass('active');
                            },
                            out: function (event, ui) {
                                if (ui.helper != null) {
                                    var $items = $('#app_layout_list .item_row');
                                    if ($items.length <= 1) {
                                        if ($items.length == 1) {
                                            if ($items.find('.item_field_value').length == 0)
                                            {
                                                $(".field_option").animate({ right: -240, speed: 2000 });
                                                $("#app_layout_list").width(opt.width - 149);
                                                $filedLayout.find(".guideareas").show();
                                            }
                                        }
                                        else {
                                            $(".field_option").animate({ right: -240, speed: 2000 });
                                            $("#app_layout_list").width(opt.width - 149);
                                            $filedLayout.find(".guideareas").show();
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
                layout.init($filedLayout);
            },
            getCurrentPanelData: function () {//获取当前面板数据
                //获取区域字段布局数据
                var data = [];
                var $select = $('.filedLayout .topToolBar .areaSelect');
                var $app_layout_list = $('#app_layout_list');
                var currentId = $app_layout_list.attr('data-currentId');
                $('#app_layout_list .item_row').each(function (i) {
                    var itemdata = $(this)[0].itemdata;
                    itemdata.sortCode = i;
                    data.push(itemdata);
                });
                if (data.length > 0)
                {
                    if (!!$select[0].formData[currentId])
                    {
                        $select[0].formData[currentId].fields = data;
                    }
                }
            },
            changeArea: function () {
                custmerform.filedLayout.getCurrentPanelData();//切换前保存数据
                var value = $(this).val();
                var data = $(this)[0].formData[value];
                $('.filedLayout .topToolBar .typeSelect').val(data.area.type);
                custmerform.filedLayout.areaType = data.area.type;
                custmerform.filedLayout.renderFiledLayot(value, data.fields);
            },
            itemRowClick: function () {
                var $this = $(this);
                var $field_option = $(".field_option");
                $("#app_layout_list .item_row").removeClass('active').removeClass('activeerror');
                $("#app_layout_list").width(qopt.width - 389);
                $this.addClass('active');
                $field_option.animate({ right: 0, speed: 2000 }).show();
                learun.components[$this[0].itemdata.type].property(qopt, $this);
            },
            itemRowRemoveClick: function () {
                var $item_row = $(this).parents('.item_row');
                $item_row.remove();

                if ($('#app_layout_list .item_row').length == 0) {
                    $(".field_option").animate({ right: -240, speed: 2000 });
                    $("#app_layout_list").width(qopt.width - 149);
                    $(".guideareas").show();
                }
                else {
                    $($('#app_layout_list .item_row')[0]).trigger('click');
                }
            },
            renderFiledLayot: function (id, data) {//渲染字段布局
                var $app_layout_list = $('#app_layout_list');
                $app_layout_list.attr('data-currentId', id);
                $app_layout_list.html("");
                $.each(data, function (i, item) {
                    var $itemRow = $('<div class="item_row" style="display: block;"></div>');
                    $itemRow.css({ width: ((100 / custmerform.filedLayout.areaType) + "%"), float: "left" });
                    $itemRow[0].itemdata = item;
                    learun.components[item.type].render($itemRow);
                    $itemRow.unbind('click');
                    $itemRow.click(custmerform.filedLayout.itemRowClick);
                    $itemRow.find('.item_field_remove i').unbind('click');
                    $itemRow.find('.item_field_remove i').click(custmerform.filedLayout.itemRowRemoveClick);
                    $app_layout_list.append($itemRow);
                });
                $($app_layout_list.find('.item_row')[0]).trigger('click');
            }
        }
    }
    //对外暴露接口
    $.fn.custmerForm = function (type, opt) {
        var $this = $(this);
        if (!$this.attr('id')) {
            return false;
        }
        switch (type) {
            case "init": custmerform.init(opt, $this);
                break;
            case "get": return custmerform.get($this, (opt == undefined ? false : opt.isValid));
                break;
            case "updateDbFilds":
                custmerform.updateFilds($this, opt);
                break;
        }
    }
})(window.jQuery,window.learun);





