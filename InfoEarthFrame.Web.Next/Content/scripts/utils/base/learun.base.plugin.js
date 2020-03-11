/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * jquery扩展插件
 * 陈小二
 */
(function ($, learun) {
    "use strict";
    //iframe嵌套的点击事件中关闭下拉框
    if (window.parent && window.parent.document && window.parent.document.body) {
        window.parent.document.body.onclick = function () {
            document.body.click();
        };
    };
    

    //下拉框
    $.fn.comboBox = function (opt) {//下拉框
        var $select = $(this);
        var selectId = $select.attr('id');
        if (!selectId) {
            return false;
        }
        var opt = $.extend({
            //请选择
            description: "==请选择==",
            //字段
            id: "id",
            text: "text",
            title: "title",
            //展开最大高度
            maxHeight: null,
            //宽度
            width: null,
            //是否允许搜索
            allowSearch: false,
            //访问数据接口地址
            url: null,
            //访问数据接口参数
            param: null,
            //下拉选择数据
            data: null,
            //数据名称
            dataName: false,
            //默认选中第一个
            selectOne: false,
            //方法
            method: "GET",
            //支持多选
            multiple: $select.attr("multiples") != undefined ? true : false
        }, opt);
        var dom = {
            rendering: function () {
                if ($select.find('.ui-select-text').length == 0) {
                    $select.html("<div class=\"ui-select-text\" style='color:#999;'>" + opt.description + "</div>");
                }
                //渲染下拉选项框
                var optionHtml = "<div class=\"ui-select-option\">";
                optionHtml += "<div class=\"ui-select-option-content\" style=\"max-height: " + opt.maxHeight + "\"></div>";
                if (opt.allowSearch) {
                    optionHtml += "<div class=\"ui-select-option-search\"><input type=\"text\" class=\"form-control\" placeholder=\"搜索关键字\" /><span class=\"input-query\" title=\"Search\"><i class=\"fa fa-search\"></i></span></div>";
                }
                optionHtml += "</div>";
                var $optionHtml = $(optionHtml);
                $optionHtml.attr('id', selectId + '-option');

                var $obj = $("#" + selectId + "-option");
                if ($obj.length != undefined && $obj.length == 1) {
                    if (opt.maxHeight != null) {
                        $obj.find('.ui-select-option-content').css('max-height', opt.maxHeight);
                    }
                    if (opt.allowSearch) {
                        if ($obj.find('.ui-select-option-search').length != 1) {
                            $obj.append("<div class=\"ui-select-option-search\"><input type=\"text\" class=\"form-control\" placeholder=\"搜索关键字\" /><span class=\"input-query\" title=\"Search\"><i class=\"fa fa-search\"></i></span></div>");
                        }
                    }
                }
                else {
                    $('body').prepend($optionHtml);
                }

                return $("#" + selectId + "-option");
            },
            renderingData: function ($option, setting, searchValue) {
                if (setting.data != undefined && setting.data.length >= 0) {
                    var $_html = $('<ul></ul>');
                    if (setting.description) {
                        $_html.append('<li data-value="">' + setting.description + '</li>');
                    }
                    $.each(setting.data, function (i, row) {
                        var title = row[setting.title];
                        if (title == undefined) {
                            title = "";
                        }
                        if (searchValue != undefined) {
                            if (row[setting.text].indexOf(searchValue) != -1) {
                                $_html.append('<li data-value="' + row[setting.id] + '" title="' + title + '">' + row[setting.text] + '</li>');
                            }
                        }
                        else {
                            if (opt.multiple == true) {
                                var _checkbox = '<input type="checkbox" style="margin-right: 5px;margin-top: -1px;vertical-align: middle;" />';
                                $_html.append('<li data-value="' + row[setting.id] + '" data-text="' + row[setting.text] + '" title="' + title + '">' + _checkbox + row[setting.text] + '</li>');
                            } else {
                                $_html.append('<li data-value="' + row[setting.id] + '" title="' + title + '">' + row[setting.text] + '</li>');
                            }
                        }
                    });
                    $option.find('.ui-select-option-content').html($_html);
                    if (opt.multiple == true) {
                        $option.find('.fxBox').remove();
                        $option.find('.ui-select-option-content').parent().css("padding-bottom", "30px");
                        $option.find('.ui-select-option-content').parent().append('<div class="fxBox"><label class="fxLabel"><input type="checkbox" class="fxCheck" style="margin:0 5px;">反选</label>  <span class="yesBtn">确定</span></div>');
                    }
                    $option.find('li').css('padding', "0 5px");
                    $option.find('li').unbind();
                    $option.find('li').click(function (e) {
                        $select.attr("data-yesBtn","");
                        var $this = $(this);
                        if (opt.multiple == false) {
                            $select.attr("data-value", $this.attr('data-value')).attr("data-text", $this.text());
                            $select.find('.ui-select-text').html($this.text()).css('color', '#000');
                            $option.slideUp(150);
                            $select.trigger("change");
                        } else {
                            var _value = [];
                            var _text = [];
                            $this.parents("ul").find("input[type='checkbox']:checkbox:checked").each(function (i) {
                                var value = $(this).parents("li").attr("data-value");
                                var text = $(this).parents("li").attr("data-text");
                                _value.push(value)
                                _text.push(text)
                            });
                            $select.attr("data-value", String(_value));
                            $select.find('.ui-select-text').html(String(_text)).css('color', '#000');
                        }
                        e.stopPropagation();
                    }).hover(function (e) {
                        if (!$(this).hasClass('liactive')) {
                            $(this).toggleClass('on');
                        }
                        e.stopPropagation();
                    });
                    $option.find('.fxBox').click(function (e) {
                        e.stopPropagation();
                    });
                    $option.find('.fxCheck').click(function (e) {
                        $(this).attr("disabled", "disabled");
                        $(this).find(".fxCheck").attr("disabled", "disabled");
                        $option.find('input:not(.fxCheck)').click();
                        $(this).removeAttr("disabled");
                        $(this).find(".fxCheck").removeAttr("disabled");
                        e.stopPropagation();
                    });
                    //$option.find('.yesBtn').click(function (e) {
                    //    $select.attr("data-yesBtn", "yesBtn");
                    //    $select.click();
                    //});
                }
            },
            loadData: function () {
                if (!!opt.url) {
                    opt.data = learun.ajax.asyncGet({
                        url: opt.url,
                        data: opt.param,
                        type: opt.method
                    });
                    if (!!opt.dataName) {
                        opt.data = opt.data[opt.dataName];
                    }
                }
                else {
                    var $lilists = $select.find('li');
                    if ($lilists.length > 0) {
                        opt.data = [];
                        $lilists.each(function (e) {
                            var $li = $(this);
                            var point = {};
                            point[opt.id] = $li.attr('data-value');
                            point[opt.title] = $li.attr('title');
                            point[opt.text] = $li.html();
                            opt.data.push(point);
                        });
                    }
                }
            }
        };
        dom.loadData();
        var $option = dom.rendering();
        dom.renderingData($option, opt);
        var $yesBtn;
        if (opt.multiple) {
            $yesBtn = $option.find('.yesBtn');
            $yesBtn.bind("click", function (e) {
                if ($option.attr('data-show')) {
                    $option.hide();
                } else {
                    $option.slideUp(150);
                }
                $select.trigger("change");
            });
        }
        //操作搜索事件
        if (opt.allowSearch) {
            $option.find('.ui-select-option-search').find('input').bind("keypress", function (e) {
                if (event.keyCode == "13") {
                    var $this = $(this);
                    dom.renderingData($option, $this[0].opt, $this.val());
                }
            }).focus(function () {
                $(this).select();
            })[0]["opt"] = opt;
        }
        $select.unbind('click');
        $select.bind("click", function (e) {
            console.log(111);
            if ($select.attr('readonly') == 'readonly' || $select.attr('disabled') == 'disabled') {
                return false;
            }
            $(this).addClass('ui-select-focus');
            if ($option.is(":hidden")) {
                $select.find('.ui-select-option').hide();
                $('.ui-select-option').hide();

                var left = $select.offset().left;
                var top = $select.offset().top + 29;
                var width = $select.width();
                if (opt.width) {
                    width = opt.width;
                }
                if (($option.height() + top) < $(document).height()) {
                    $option.slideDown(150).css({ top: top, left: left, width: width });
                } else {
                    var _top = (top - $option.height() - 32)
                    $option.show().css({ top: _top, left: left, width: width });
                    $option.attr('data-show', true);
                }
                $option.css('border-top', '1px solid #ccc');
                $option.find('li').removeClass('liactive');
                if (!opt.multiple) {
                    $option.find('[data-value="' + $select.attr("data-value") + '"]').addClass('liactive');
                }
                $option.find('.ui-select-option-search').find('input').select();
            } else {
                if (!opt.multiple) {
                    if ($option.attr('data-show')) {
                        $option.hide();
                    } else {
                        $option.slideUp(150);
                    }
                } else {
                    $option.slideUp(150);
                };

            }
            e.stopPropagation();
        });
        $(document).click(function (e) {
            if (opt.multiple == false) {
                var e = e ? e : window.event;
                var tar = e.srcElement || e.target;
                if (!$(tar).hasClass('form-control')) {
                    if ($option.attr('data-show')) {
                        $option.hide();
                    } else {
                        $option.slideUp(150);
                    }
                    $select.removeClass('ui-select-focus');
                    e.stopPropagation();
                }
            } else {
                $option.slideUp(150);
            };
        });
        if (opt.selectOne) {
            if (!!opt.data) {
                $select.comboBoxSetValue(opt.data[0][opt.id]);
            }
        }
        return $select;
    };
    $.fn.comboBoxEx = function (opt) {//改善下拉框
        var $select = $(this);
        var selectId = $select.attr('id');
        if (!selectId) {
            return false;
        }
        var opt = $.extend({
            //请选择
            description: "==请选择==",
            //字段
            id: "id",
            text: "text",
            title: "title",
            //展开最大高度
            maxHeight: null,
            //宽度
            width: null,
            //是否允许搜索
            allowSearch: false,
            //访问数据接口地址
            url: null,
            //访问数据接口参数
            param: null,
            //下拉选择数据
            data: null,
            //数据名称
            dataName: false,
            //默认选中第一个
            selectOne: false,
            //方法
            method: "GET",
            //支持多选
            multiple: $select.attr("multiples") != undefined ? true : false
        }, opt);
        var dom = {
            rendering: function () {
                if ($select.find('.ui-select-text').length == 0) {
                    $select.html("<div class=\"ui-select-text\" style='color:#999;'>" + opt.description + "</div>");
                }
                //渲染下拉选项框
                var optionHtml = "<div class=\"ui-select-option\">";
                optionHtml += "<div class=\"ui-select-option-content\" style=\"max-height: " + opt.maxHeight + "\"></div>";
                if (opt.allowSearch) {
                    optionHtml += "<div class=\"ui-select-option-search\"><input type=\"text\" class=\"form-control\" placeholder=\"搜索关键字\" /><span class=\"input-query\" title=\"Search\"><i class=\"fa fa-search\"></i></span></div>";
                }
                optionHtml += "</div>";
                var $optionHtml = $(optionHtml);
                $optionHtml.attr('id', selectId + '-option');

                var $obj = $("#" + selectId + "-option");
                if ($obj.length != undefined && $obj.length == 1) {
                    if (opt.maxHeight != null) {
                        $obj.find('.ui-select-option-content').css('max-height', opt.maxHeight);
                    }
                    if (opt.allowSearch) {
                        if ($obj.find('.ui-select-option-search').length != 1) {
                            $obj.append("<div class=\"ui-select-option-search\"><input type=\"text\" class=\"form-control\" placeholder=\"搜索关键字\" /><span class=\"input-query\" title=\"Search\"><i class=\"fa fa-search\"></i></span></div>");
                        }
                    }
                }
                else {
                    $('body').prepend($optionHtml);
                }

                return $("#" + selectId + "-option");
            },
            renderingData: function ($option, setting, searchValue) {
                if (setting.data != undefined && setting.data.length >= 0) {
                    var $_html = $('<ul></ul>');
                    if (setting.description) {
                        $_html.append('<li data-value="">' + setting.description + '</li>');
                    }
                    $.each(setting.data, function (i, row) {
                        var title = row[setting.title];
                        if (title == undefined) {
                            title = "";
                        }
                        if (searchValue != undefined) {
                            if (row[setting.text].indexOf(searchValue) != -1) {
                                $_html.append('<li data-value="' + row[setting.id] + '" title="' + title + '">' + row[setting.text] + '</li>');
                            }
                        }
                        else {
                            if (opt.multiple == true) {
                                var _checkbox = '<input type="checkbox" style="margin-right: 5px;margin-top: -1px;vertical-align: middle;" class="checkboxEd"/>';
                                $_html.append('<li data-value="' + row[setting.id] + '" data-text="' + row[setting.text] + '" title="' + title + '">' + _checkbox + '<span style="display:inline-block;width:70%">' + row[setting.text] + '</span></li>');
                            } else {
                                $_html.append('<li data-value="' + row[setting.id] + '" title="' + title + '">' + '<span style="display:inline-block;width:70%">' + row[setting.text] + '</span></li>');
                            }
                        }
                    });
                    $option.find('.ui-select-option-content').html($_html);
                    if (opt.multiple == true) {
                        $option.find('.ui-select-option-content').parent().css("padding-bottom", "30px");
                        $option.find('.ui-select-option-content').parent().append('<div class="fxBox"><label class="fxLabel"><input type="checkbox" class="fxCheck" style="margin:0 5px;">反选</label>  <span class="yesBtn">确定</span></div>');
                    }
                    $option.find('li').css('padding', "0 5px");
                    $option.find('li').unbind();
                    $option.find('li>span').click(function (e) {
                        $select.attr("data-yesBtn", "");
                        var $this = $(this);
                        if (opt.multiple == false) {
                            $select.attr("data-value", $this.attr('data-value')).attr("data-text", $this.text());
                            $select.find('.ui-select-text').html($this.text()).css('color', '#000');
                            $option.slideUp(150);
                            $select.trigger("change");
                        } else {
                            var _value = [];
                            var _text = [];
                            var checkbox = $this.prev("input[type='checkbox']");
                            if (checkbox.prop("checked")) {
                                checkbox.prop("checked", false);
                            } else {
                                checkbox.prop("checked", true);
                            }
                            $this.parents("ul").find("input[type='checkbox']:checkbox:checked").each(function (i) {
                                var value = $(this).parents("li").attr("data-value");
                                var text = $(this).parents("li").attr("data-text");
                                _value.push(value)
                                _text.push(text)
                            });
                            $select.attr("data-value", String(_value));
                            $select.find('.ui-select-text').html(String(_text)).css('color', '#000');
                        }
                        e.stopPropagation();
                    })
                    $option.find('li').hover(function (e) {
                        if (!$(this).hasClass('liactive')) {
                            $(this).toggleClass('on');
                        }
                        e.stopPropagation();
                    });
                    $option.find('li').click(function (e) {
                        e.stopPropagation();
                    });
                    $option.find('.fxBox').click(function (e) {
                        e.stopPropagation();
                    });
                    $option.find('.checkboxEd').click(function (e) {
                        if ($(this).prop('checked')) {
                            $(this).prop('ckecked', true)
                        } else {
                            $(this).prop('ckecked', false)
                        }
                        var _value = [];
                        var _text = [];
                        $(this).parents("ul").find("input[type='checkbox']:checkbox:checked").each(function (i) {
                            var value = $(this).parents("li").attr("data-value");
                            var text = $(this).parents("li").attr("data-text");
                            _value.push(value)
                            _text.push(text)
                        });
                        $select.attr("data-value", String(_value));
                        $select.find('.ui-select-text').html(String(_text)).css('color', '#000');
                        e.stopPropagation();
                    });
                    $option.find('.fxCheck').click(function (e) {
                        $(this).attr("disabled", "disabled");
                        $(this).find(".fxCheck").attr("disabled", "disabled");
                        $option.find('input:not(.fxCheck)').click();
                        $(this).removeAttr("disabled");
                        $(this).find(".fxCheck").removeAttr("disabled");
                        e.stopPropagation();
                    });
                    //$option.find('.yesBtn').click(function (e) {
                    //    $select.attr("data-yesBtn", "yesBtn");
                    //    $select.click();
                    //});
                }
            },
            loadData: function () {
                if (!!opt.url) {
                    if (opt.method.toUpperCase() == "GET") {
                        opt.data = learun.ajax.asyncGet({
                            url: opt.url,
                            data: opt.param,
                            type: opt.method
                        });
                    } else {
                        opt.data = learun.ajax.asyncPost({
                            url: opt.url,
                            data: opt.param,
                            type: opt.method
                        });
                    }
                    if (!!opt.dataName) {
                        opt.data = opt.data[opt.dataName];
                    }
                }
                else {
                    var $lilists = $select.find('li');
                    if ($lilists.length > 0) {
                        opt.data = [];
                        $lilists.each(function (e) {
                            var $li = $(this);
                            var point = {};
                            point[opt.id] = $li.attr('data-value');
                            point[opt.title] = $li.attr('title');
                            point[opt.text] = $li.html();
                            opt.data.push(point);
                        });
                    }
                }
            }
        };
        dom.loadData();
        var $option = dom.rendering();
        dom.renderingData($option, opt);
        var $yesBtn;
        if (opt.multiple) {
            $yesBtn = $option.find('.yesBtn');
            $yesBtn.bind("click", function (e) {
                if ($option.attr('data-show')) {
                    $option.hide();
                } else {
                    $option.slideUp(150);
                }
                $select.trigger("change");
            });
        }
        //操作搜索事件
        if (opt.allowSearch) {
            $option.find('.ui-select-option-search').find('input').bind("keypress", function (e) {
                if (event.keyCode == "13") {
                    var $this = $(this);
                    dom.renderingData($option, $this[0].opt, $this.val());
                }
            }).focus(function () {
                $(this).select();
            })[0]["opt"] = opt;
        }
        $select.unbind('click');
        $select.bind("click", function (e) {
            if ($select.attr('readonly') == 'readonly' || $select.attr('disabled') == 'disabled') {
                return false;
            }
            $(this).addClass('ui-select-focus');
            if ($option.is(":hidden")) {
                $select.find('.ui-select-option').hide();
                $('.ui-select-option').hide();

                var left = $select.offset().left;
                var top = $select.offset().top + 29;
                var width = $select.width();
                if (opt.width) {
                    width = opt.width;
                }
                if (($option.height() + top) < $(document).height()) {
                    $option.slideDown(150).css({ top: top, left: left, width: width });
                } else {
                    var _top = (top - $option.height() - 32)
                    $option.show().css({ top: _top, left: left, width: width });
                    $option.attr('data-show', true);
                }
                $option.css('border-top', '1px solid #ccc');
                $option.find('li').removeClass('liactive');
               
                if (!opt.multiple) {
                    $option.find('[data-value="' + $select.attr("data-value") + '"]').addClass('liactive');
                }
                $option.find('.ui-select-option-search').find('input').select();
            } else {
                
                if (!opt.multiple) {
                    if ($option.attr('data-show')) {
                        $option.hide();
                    } else {
                        $option.slideUp(150);
                    }
                } else {
                   $option.slideUp(150);
                };
            }
            e.stopPropagation();
        });
        $(document).click(function (e) {
            if (opt.multiple == false) {
                var e = e ? e : window.event;
                var tar = e.srcElement || e.target;
                if (!$(tar).hasClass('form-control')) {
                    if ($option.attr('data-show')) {
                        $option.hide();
                    } else {
                        $option.slideUp(150);
                    }
                    $select.removeClass('ui-select-focus');
                    e.stopPropagation();
                }
            } else {
               $option.slideUp(150);
            };
        });
        if (opt.selectOne) {
            if (!!opt.data) {
                $select.comboBoxSetValue(opt.data[0][opt.id]);
            }
        }
        return $select;
    };
    $.fn.comboBoxSetValue = function (value) {
        if (learun.isNullOrEmpty(value)) {
            return;
        }
        var $select = $(this);
        var $option = $("#" + $select.attr('id') + "-option");
        $select.attr('data-value', value);
        var multiple = $select.attr("multiples") != undefined ? true : false;
        if (multiple == false) {
            var data_text = $option.find('ul').find('[data-value=' + value + ']').html();
            if (data_text) {
                $select.attr('data-text', data_text);
                $select.find('.ui-select-text').html(data_text).css('color', '#000');
                $option.find('ul').find('[data-value=' + value + ']').addClass('liactive')
            }
            $select.trigger("change");

        } else {
            var _text = [];
            var arr = value.split(",");
            for (var i = 0; i < arr.length; i++) {
                $option.find('ul').find("[data-value='" + arr[i] + "']").find("input[type='checkbox']").attr("checked", "checked");
                _text.push($option.find('ul').find("[data-value='" + arr[i] + "']").attr("data-text"))
            }
            $select.attr('data-text', String(_text));
            $select.find('.ui-select-text').html(String(_text)).css('color', '#000');
        }
        return $select;
    };
    //下拉框树形/6.1.2.0 搜索功能做了优化不用再去后台获取了
    $.fn.comboBoxTree = function (opt) {
        //opt参数：description,height,allowSearch,appendTo,click,url,param,method,icon
        var $select = $(this);
        var selectId = $select.attr('id');
        if (!selectId) {
            return false;
        }

        var opt = $.extend({
            //请选择
            description: "==请选择==",
            //字段
            id: "id",
            text: "text",
            title: "title",
            //展开最大高度
            maxHeight: null,
            //宽度
            width: null,
            //是否允许搜索
            allowSearch: false,
            //访问数据接口地址
            url: false,
            //访问数据接口参数
            param: null,
            //接口请求的方法
            method: "GET",
            //加载到哪个标签里面
            appendTo: null,
            //选择点击事件
            click: null,
            //是否移除图标
            icon: false,
            data: null,
            dataItemName: false
        }, opt);

        var dom = {
            rendering: function () {
                if ($select.find('.ui-select-text').length == 0) {
                    $select.html("<div class=\"ui-select-text\" style='color:#999;'>" + opt.description + "</div>");
                }
                //渲染下拉选项框
                var optionHtml = "<div class=\"ui-select-option\">";
                optionHtml += "<div class=\"ui-select-option-content\" style=\"max-height: " + opt.maxHeight + "\"></div>";
                if (opt.allowSearch) {
                    optionHtml += "<div class=\"ui-select-option-search\"><input type=\"text\" class=\"form-control\" placeholder=\"搜索关键字\" /><span class=\"input-query\" title=\"Search\"><i class=\"fa fa-search\"></i></span></div>";
                }
                optionHtml += "</div>";
                var $optionHtml = $(optionHtml);
                $optionHtml.attr('id', selectId + '-option');
                if (opt.appendTo) {
                    $(opt.appendTo).prepend($optionHtml);
                } else {
                    $('body').prepend($optionHtml);
                }
                return $("#" + selectId + "-option");
            },
            loadtreeview: function (setting, data) {
                $option_content.treeview({
                    onnodeclick: function (item) {
                        if (setting.click) {
                            var flag = "ok";
                            flag = setting.click(item);
                            if (flag == "false") {
                                return false;
                            }
                        }
                        $select.attr("data-value", item.id).attr("data-text", item.text);
                        $select.find('.ui-select-text').html(item.text).css('color', '#000');
                        $select.trigger("change");
                    },
                    height: setting.maxHeight,
                    data: data,
                    description: setting.description
                });
            },
            loadData: function (opt) {//统一路口加载数据
                var data = [];
                if (!!opt.data) {
                    data = opt.data;
                }
                else {
                    data = learun.ajax.asyncGet({
                        url: opt.url,
                        data: opt.param,
                        type: opt.method
                    });
                }
                if (opt.dataItemName) {
                    opt.data = [];
                    $.each(data, function (i, item) {
                        var _itemText = getInfoTop().learun.data.get(["dataItem", opt.dataItemName, item[opt.text]]);
                        if (_itemText != "") {
                            item[opt.text] = _itemText
                        }
                        opt.data.push(item);
                    });
                }
                else {
                    opt.data = data;
                }
            },
            //搜索方法
            searchData: function (data, keyword) {
                var pFlag = false;
                var childData = [];
                $.each(data, function (i, row) {
                    var item = {};
                    for (var ii in row) {
                        if (ii != "ChildNodes") {
                            item[ii] = row[ii];
                        }
                    }
                    var flag = false;
                    if (item.text.indexOf(keyword) != -1) {
                        flag = true;
                    }
                    if (item.hasChildren) {
                        item.ChildNodes = dom.searchData(row.ChildNodes, keyword);
                        if (item.ChildNodes.length > 0) {
                            flag = true;
                        }
                        else {
                            item.hasChildren = false;
                        }
                    }
                    if (flag) {
                        pFlag = true;
                        childData.push(item);
                    }
                });
                return childData;
            }
        };

        var $option = dom.rendering();
        var $option_content = $("#" + selectId + "-option").find('.ui-select-option-content');
        dom.loadData(opt);
        dom.loadtreeview(opt, opt.data);

        if (opt.allowSearch) {
            $option.find('.ui-select-option-search').find('input').bind("keypress", function (e) {
                if (event.keyCode == "13") {
                    var $this = $(this);
                    var value = $(this).val();
                    var data = dom.searchData($this[0].opt.data, value);
                    dom.loadtreeview($this[0].opt, data);
                }
            }).focus(function () {
                $(this).select();
            })[0]["opt"] = opt;
        }
        if (opt.icon) {
            $option.find('i').remove();
            $option.find('img').remove();
        }
        $select.find('.ui-select-text').unbind('click');
        $select.find('.ui-select-text').bind("click", function (e) {
            if ($select.attr('readonly') == 'readonly' || $select.attr('disabled') == 'disabled') {
                return false;
            }
            $(this).parent().addClass('ui-select-focus');
            if ($option.is(":hidden")) {
                $select.find('.ui-select-option').hide();
                $('.ui-select-option').hide();
                var left = $select.offset().left;
                var top = $select.offset().top + 29;
                var width = $select.width();
                if (opt.width) {
                    width = opt.width;
                }
                if (($option.height() + top) < $(window).height()) {
                    $option.slideDown(150).css({ top: top, left: left, width: width });
                } else {
                    var _top = (top - $option.height() - 32);
                    $option.show().css({ top: _top, left: left, width: width });
                    $option.attr('data-show', true);
                }
                $option.css('border-top', '1px solid #ccc');
                if (opt.appendTo) {
                    $option.css("position", "inherit")
                }
                $option.find('.ui-select-option-search').find('input').select();
            } else {
                if ($option.attr('data-show')) {
                    $option.hide();
                } else {
                    $option.slideUp(150);
                }
            }
            e.stopPropagation();
        });
        $select.find('li div').click(function (e) {
            var e = e ? e : window.event;
            var tar = e.srcElement || e.target;
            if (!$(tar).hasClass('bbit-tree-ec-icon')) {
                $option.slideUp(150);
                e.stopPropagation();
            }
        });
        $(document).click(function (e) {
            var e = e ? e : window.event;
            var tar = e.srcElement || e.target;
            if (!$(tar).hasClass('bbit-tree-ec-icon') && !$(tar).hasClass('form-control')) {
                if ($option.attr('data-show')) {
                    $option.hide();
                } else {
                    $option.slideUp(150);
                }
                $select.removeClass('ui-select-focus');
                e.stopPropagation();
            }
        });
        return $select;
    };
    $.fn.comboBoxTreeSetValue = function (value) {
        if (learun.isNullOrEmpty(value)) {
            return;
        }
        var $select = $(this);
        var $option_content = $("#" + $select.attr('id') + "-option").find('.ui-select-option-content');
        $option_content.find('ul').find('[data-value=' + value + ']').trigger('click');
        return $select;
    };
    //获取、设置表单数据
    $.fn.getWebControls = function (keyValue) {
        var reVal = "";
        $(this).find('input,select,textarea,.ui-select,.uploadify,.webUploader').each(function (r) {
            var el = $(this);
            var id = el.attr('id');
            if (el.attr('id') == "KQJD") {
                var dddd = el.attr('name');
            }
            var type = el.attr('type');
            switch (type) {
                case "checkbox":
                    if (id) {
                        if (el.is(":checked")) {
                            reVal += '"' + id + '"' + ':' + '"1",'
                        } else {
                            reVal += '"' + id + '"' + ':' + '"0",'
                        }
                    } else {
                        if (el.is(":checked")) {
                            if (reVal.indexOf('"' + el.attr('name') + '"') == -1) {
                                var chekcValue = "";
                                $("[name='" + el.attr('name') + "']").each(function () {
                                    if ($(this).is(":checked")) {
                                        chekcValue += $(this).attr('value') + ",";
                                    }
                                });
                                reVal += '"' + el.attr('name') + '"' + ':"' + chekcValue + '",';
                            }
                        }
                    }
                    break;
                case "radio":
                    if (el.is(":checked")) {
                        reVal += '"' + el.attr('name') + '"' + ':"' + el.attr('value') + '",'
                    }
                    break;
                case "select":
                    var value = el.attr('data-value');
                    if (value == "") {
                        //value = "&nbsp;";
                        value = "";
                    }
                    reVal += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
                    break;
                case "selectTree":
                    var value = el.attr('data-value');
                    if (value == "") {
                        //value = "&nbsp;";
                        value = "";
                    }
                    reVal += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
                    break;
                case "webUploader":
                case "uploadify":
                    var value = el.attr('data-value');
                    if (value == "" || value == undefined) {
                        //value = "&nbsp;";
                        value = "";
                    }
                    reVal += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
                    break;
                default:
                    var value = el.val();
                    if (value == "") {
                        //value = "&nbsp;";
                        value = "";
                    }
                    reVal += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
                    break;
            }
        });
        reVal = reVal.substr(0, reVal.length - 1);
        if (!keyValue) {
            reVal = reVal.replace(/&nbsp;/g, '');
        }
        reVal = reVal.replace(/\\/g, '\\\\');
        reVal = reVal.replace(/\n/g, '\\n');
        var postdata = jQuery.parseJSON('{' + reVal + '}');
        return postdata;
    };
    $.fn.setWebControls = function (data) {
        var $id = $(this)
        for (var key in data) {
            var id = $id.find('#' + key);
            if (id.attr('id')) {
                var type = id.attr('type');
                if (type) {
                    if (id.hasClass("input-datepicker") || id.hasClass("input-wdatepicker")) {
                        type = "datepicker";
                    }
                    var value = $.trim(data[key]).replace(/&nbsp;/g, '');
                    switch (type) {
                        case "checkbox":
                            if (value == 1) {
                                id.attr("checked", 'checked');
                            } else {
                                id.removeAttr("checked");
                            }
                            break;
                        case "select":
                            id.comboBoxSetValue(value);
                            break;
                        case "selectTree":
                            id.comboBoxTreeSetValue(value);
                            break;
                        case "datepicker":
                            id.val(formatDate(value, 'yyyy-MM-dd'));
                            break;
                        case "uploadify":
                        case "webUploader":
                            id.uploadifyExSet(value);
                        default:
                            id.val(value);
                            break;
                    }
                } else {
                    var value = $.trim(data[key]).replace(/&nbsp;/g, '');
                    if ($(id).get(0).tagName == "DIV") {
                        if (value.indexOf(",") == -1) {
                            $("input[name='" + key + "'][value=" + value + "]").attr("checked", 'checked');
                        }
                        else {
                            var checkValueData = value.split(',');
                            for (var i = 0; i < checkValueData.length; i++) {
                                $("input[name='" + key + "'][value=" + checkValueData[i] + "]").attr("checked", 'checked');
                            }
                        }
                    } else {
                        id.val(value);
                    }

                }
            }
        }
    };
    $.fn.getSysFormControls = function () {
        var postdata = [];
        $(this).find('[data-wfname]').each(function (r) {
            var $obj = $(this);
            var name = $obj.attr('data-wfname');
            var id = $obj.attr('id');
            var type = $obj.attr('type');
            if (id == undefined) {
                id = $obj.attr('data-id');
            }
            var girdId = $obj.attr('data-girdid');
            postdata.push({ "field": id, "label": name, 'type': type, 'girdId': girdId });
        });
        return postdata;
    };
    //右键菜单
    $.fn.conTextMenu = function () {
        var element = $(this);
        var oMenu = $('.contextmenu');
        $(document).click(function () {
            oMenu.hide();
        });
        $(document).mousedown(function (e) {
            if (3 == e.which) {
                oMenu.hide();
            }
        })
        var aUl = oMenu.find("ul");
        var aLi = oMenu.find("li");
        var showTimer = null, hideTimer = null;
        var i = 0;
        var maxWidth = null, maxHeight = 0;
        var aDoc = [document.documentElement.offsetWidth, document.documentElement.offsetHeight];
        oMenu.hide();
        for (i = 0; i < aLi.length; i++) {
            //为含有子菜单的li加上箭头
            aLi[i].getElementsByTagName("ul")[0] && (aLi[i].className = "sub");
            //鼠标移入
            aLi[i].onmouseover = function () {
                var oThis = this;
                var oUl = oThis.getElementsByTagName("ul");
                //鼠标移入样式
                oThis.className += " active";
                //显示子菜单
                if (oUl[0]) {
                    clearTimeout(hideTimer);
                    showTimer = setTimeout(function () {
                        for (i = 0; i < oThis.parentNode.children.length; i++) {
                            oThis.parentNode.children[i].getElementsByTagName("ul")[0] &&
                            (oThis.parentNode.children[i].getElementsByTagName("ul")[0].style.display = "none");
                        }
                        oUl[0].style.display = "block";
                        oUl[0].style.top = oThis.offsetTop + "px";
                        oUl[0].style.left = oThis.offsetWidth + "px";

                        //最大显示范围					
                        var maxWidth = aDoc[0] - oUl[0].offsetWidth;
                        var maxHeight = aDoc[1] - oUl[0].offsetHeight;

                        //防止溢出
                        maxWidth < getOffset.left(oUl[0]) && (oUl[0].style.left = -oUl[0].clientWidth + "px");
                        maxHeight < getOffset.top(oUl[0]) && (oUl[0].style.top = -oUl[0].clientHeight + oThis.offsetTop + oThis.clientHeight + "px")
                    }, 300);
                }
            };
            //鼠标移出	
            aLi[i].onmouseout = function () {
                var oThis = this;
                var oUl = oThis.getElementsByTagName("ul");
                //鼠标移出样式
                oThis.className = oThis.className.replace(/\s?active/, "");

                clearTimeout(showTimer);
                var hideTimer = setTimeout(function () {
                    for (i = 0; i < oThis.parentNode.children.length; i++) {
                        oThis.parentNode.children[i].getElementsByTagName("ul")[0] &&
                        (oThis.parentNode.children[i].getElementsByTagName("ul")[0].style.display = "none");
                    }
                }, 300);
            };
        }
        //自定义右键菜单
        $(element).bind("contextmenu", function () {
            var event = event || window.event;
            oMenu.show();
            oMenu.css('top', event.clientY + "px");
            oMenu.css('left', event.clientX + "px");
            //最大显示范围
            var maxWidth = aDoc[0] - oMenu.width();
            var maxHeight = aDoc[1] - oMenu.height();
            //防止菜单溢出
            if (oMenu.offset().top > maxHeight) {
                oMenu.css('top', maxHeight + "px");
            }
            if (oMenu.offset().left > maxWidth) {
                oMenu.css('left', maxWidth + "px");
            }
            return false;
        }).bind("click", function () {
            oMenu.hide();
        });
    };
    //翻页插件扩展
    $.fn.panginationEx = function (opt) {
        var $pager = $(this);
        if (!$pager.attr('id')) {
            return false;
        }
        var opt = $.extend({
            firstBtnText: '首页',
            lastBtnText: '尾页',
            prevBtnText: '上一页',
            nextBtnText: '下一页',
            showInfo: true,
            showJump: true,
            jumpBtnText: '跳转',
            showPageSizes: true,
            infoFormat: '{start} ~ {end}条，共{total}条',
            sortname: '',
            url: "",
            success: null,
            beforeSend: null,
            complete: null
        }, opt);
        var params = $.extend({ sidx: opt.sortname, sord: "asc" }, opt.params);
        opt.remote = {
            url: opt.url,  //请求地址
            params: params,       //自定义请求参数
            beforeSend: function (XMLHttpRequest) {
                if (opt.beforeSend != null) {
                    opt.beforeSend(XMLHttpRequest);
                }
            },
            success: function (result, pageIndex) {
                //回调函数
                //result 为 请求返回的数据，呈现数据
                if (opt.success != null) {
                    opt.success(result.rows, pageIndex);
                }
            },
            complete: function (XMLHttpRequest, textStatu) {
                if (opt.complete != null) {
                    opt.complete(XMLHttpRequest, textStatu);
                }
                //...
            },
            pageIndexName: 'page',    //请求参数，当前页数，索引从0开始
            pageSizeName: 'rows',     //请求参数，每页数量
            totalName: 'records'      //指定返回数据的总数据量的字段名
        };
        $pager.page(opt);
    };
    //发送邮件左侧列表项
    $.fn.leftListShowOfEmail = function (opt) {
        var $list = $(this);
        if (!$list.attr('id')) {
            return false;
        }
        $list.append('<ul  style="padding-top: 10px;"></ul>');
        var opt = $.extend({
            id: "id",
            name: "text",
            img: "fa fa-file-o",

        }, opt);
        $list.height(opt.height);
        $.ajax({
            url: opt.url,
            data: opt.param,
            type: "GET",
            dataType: "json",
            async: false,
            success: function (data) {
                $.each(data, function (i, item) {
                    var $_li = $('<li class="" data-value="' + item[opt.id] + '"  data-text="' + item[opt.name] + '" ><i class="' + opt.img + '" style="vertical-align: middle; margin-top: -2px; margin-right: 8px; font-size: 14px; color: #666666; opacity: 0.9;"></i>' + item[opt.name] + '</li>');
                    if (i == 0) {
                        $_li.addClass("active");
                    }
                    $list.find('ul').append($_li);
                });
                $list.find('li').click(function () {
                    var key = $(this).attr('data-value');
                    var value = $(this).attr('data-text');
                    $list.find('li').removeClass('active');
                    $(this).addClass('active');
                    opt.onnodeclick({ id: key, name: value });
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                dialogMsg(errorThrown, -1);
            }
        });
    };
    //权限按钮、列表数据列
    $.fn.authorizeButton = function () {
        var $element = $(this);
        $element.find('a.btn').attr('authorize', 'no');
        $element.find('ul.dropdown-menu').find('li').attr('authorize', 'no');
        if (learun.tabiframeId() != undefined) {
            var moduleId = learun.tabiframeId().substr(6);

            if (request('moduleId') != "") {
                moduleId = request('moduleId');
            }
            var data = getInfoTop().learun.data.get(["authorizeButton", moduleId]);
            if (data != undefined) {
                $.each(data, function (i) {
                    $element.find("#" + data[i].F_EnCode).attr('authorize', 'yes');
                });
            }
            $element.find('[authorize=no]').remove();
        }
    };
    $.fn.authorizeColModel = function () {
        var $element = $(this);
        var columnModel = $element.jqGrid('getGridParam', 'colModel');
        $.each(columnModel, function (i) {
            if (columnModel[i].name != "rn") {
                $element.hideCol(columnModel[i].name);
            }
        });
        var moduleId = tabiframeId().substr(6);
        var data = getInfoTop().learun.data.get(["authorizeColumn", moduleId]);
        if (data != undefined) {
            $.each(data, function (i) {
                $element.showCol(data[i].F_EnCode);
            });
        }
    };
    //jqgird
    $.fn.jqGridEx = function (opt) {
        var $jqGrid = $(this);
        var _selectedRowIndex;
        if (!$jqGrid.attr('id')) {
            return false;
        }
        var opt = $.extend({
            url: "",
            datatype: "json",
            height: $(window).height() - 139.5,
            autowidth: true,
            colModel: [],
            viewrecords: true,
            rowNum: 30,
            rowList: [30, 50, 100],
            pager: "#gridPager",
            sortname: 'F_CreateDate desc',
            rownumbers: true,
            shrinkToFit: false,
            gridview: true,
            onSelectRow: function () {
                _selectedRowIndex = $("#" + this.id).getGridParam('selrow');
            },
            gridComplete: function () {
                $("#" + this.id).setSelection(_selectedRowIndex, false);
            }
        }, opt);
        $jqGrid.jqGrid(opt);
    };
    $.fn.jqGridRowValue = function (code) {
        var $jgrid = $(this);
        var json = [];
        var selectedRowIds = $jgrid.jqGrid("getGridParam", "selarrrow");
        if (selectedRowIds != undefined && selectedRowIds != "") {
            var len = selectedRowIds.length;
            for (var i = 0; i < len ; i++) {
                var rowData = $jgrid.jqGrid('getRowData', selectedRowIds[i]);
                json.push(rowData[code]);
            }
        } else {
            var rowData = $jgrid.jqGrid('getRowData', $jgrid.jqGrid('getGridParam', 'selrow'));
            json.push(rowData[code]);
        }
        return String(json);
    };
    $.fn.jqGridRow = function () {
        var $jgrid = $(this);
        var json = [];
        var selectedRowIds = $jgrid.jqGrid("getGridParam", "selarrrow");
        if (selectedRowIds != "") {
            var len = selectedRowIds.length;
            for (var i = 0; i < len ; i++) {
                var rowData = $jgrid.jqGrid('getRowData', selectedRowIds[i]);
                json.push(rowData);
            }
        } else {
            var rowData = $jgrid.jqGrid('getRowData', $jgrid.jqGrid('getGridParam', 'selrow'));
            json.push(rowData);
        }
        return json;
    };
    //附件上传插件初始化
    $.fn.uploadifyEx = function (opt) {
        var $uploadifyEx = $(this);
        var uploadifyExId = $uploadifyEx.attr('id');
        if (!uploadifyExId) {
            return false;
        }

        var opt = $.extend({
            btnName: "上传附件",
            url: "",
            onUploadSuccess: false,
            cancel: false,
            height: 30,
            width: 90,
            type: "webUploader",
            fileTypeExts: "",
            fileSizeLimit: 0,
            oneFile: false,
            onFileDelete:false
        }, opt);

        if (opt.type == "uploadify") {
            $uploadifyEx.removeAttr("id");
            $uploadifyEx.html('<input id="' + uploadifyExId + '" type="file" />');
            $uploadifyEx = $('#' + uploadifyExId);

            if (opt.fileTypeExts == "") {
                opt.fileTypeExts = "*.jpeg;*.jpg;*.png; *.rar;*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.txt; *.xls; *.xlsx;";
            }
            else {
                opt.fileTypeExts = '*.' + opt.fileTypeExts.replace(/,/g, ';*.') + ';';
            }

            $uploadifyEx.uploadify({
                method: 'post',
                uploader: opt.url,
                swf: getInfoTop().contentPath + '/Content/scripts/plugins/uploadify/uploadify.swf',
                buttonText: opt.btnName,
                height: opt.height,
                width: opt.width,
                fileTypeExts: opt.fileTypeExts,//'*.avi;*.mp3;*.mp4;*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.psd; *.rar;*.zip;*.swf;*.log;*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.txt; *.xls; *.xlsx;',jsonccq  
                removeCompleted: false,
                fileSizeLimit: opt.fileSizeLimit,
                onSelect: function (file) {
                    if (opt.oneFile) {
                        $('#' + uploadifyExId + '-queue').find('.uploadify-queue-item').each(function () {
                            if ($(this).attr('id') != file.id) {
                                $(this).remove();
                            }
                        });
                    }

                    var $fileItem = $("#" + file.id);
                    $fileItem.prepend('<div style="float:left;width:50px;margin-right:2px;"><img src="/Content/images/filetype/' + file.type.replace('.', '') + '.png" style="width:40px;height:40px;" /></div>');
                    $fileItem.find('.cancel').find('a').html('<i class="fa fa-trash-o "></i>');
                    $fileItem.find('.cancel').find('a').attr('title', '删除');
                    $fileItem.hover(function () {
                        $(this).find('.cancel').find('a').show();
                    }, function () {
                        $(this).find('.cancel').find('a').hide();
                    });

                    $fileItem.find('.cancel').unbind();
                    $fileItem.find('.cancel').on('click', function () {
                        var fileId = $fileItem.attr("data-fileId");
                        $fileItem.remove();
                        if ($('#' + uploadifyExId + '-queue').find('.uploadify-queue-item').length == 0) {
                            $('#' + uploadifyExId + '-queue').hide();
                        }

                        var _dd = $("#" + uploadifyExId).attr("data-value");
                        _dd = learun.stringArray(_dd, fileId);
                        $("#" + uploadifyExId).attr("data-value", _dd);
                        if (opt.onFileDelete) {
                            opt.onFileDelete(fileId);
                        }
                        
                        if ($('#' + uploadifyExId + '-queue').find('.uploadify-queue-item').length == 0) {
                            $('#' + uploadifyExId + '-queue').hide();
                        }
                    });
                },
                onUploadSuccess: function (file, data) {
                    $("#" + file.id).find('.uploadify-progress').remove();
                    $("#" + file.id).find('.data').html(' 恭喜您，上传成功！');
                    $("#" + file.id).prepend('<a class="succeed" title="成功"><i class="fa fa-check-circle"></i></a>');

                    var dataJson = JSON.parse(data);

                    $("#" + file.id).attr("data-fileId", dataJson.fileId);

                    var _dd = $("#" + uploadifyExId).attr("data-value");
                    if (_dd != undefined && _dd != "" && _dd != "undefined") {
                        _dd += ",";
                    }
                    else {
                        _dd = "";
                    }

                    $("#" + uploadifyExId).attr("data-value", _dd + dataJson.fileId);

                    if (opt.onUploadSuccess) {
                        opt.onUploadSuccess(dataJson);
                    }
                },
                onUploadError: function (file, errorCode, errorMsg, errorStr) {
                    $("#" + file.id).removeClass('uploadify-error');
                    $("#" + file.id).find('.uploadify-progress').remove();
                    $("#" + file.id).find('.data').html(' 很抱歉，上传失败！' + errorStr);
                    $("#" + file.id).prepend('<span class="error" title="失败"><i class="fa fa-exclamation-circle"></i></span>');
                },
                onUploadStart: function () {
                    $('#' + uploadifyExId + '-queue').show();
                },
                onCancel: function (file) {
                }
            });
            $("#" + uploadifyExId + "-button").prepend('<i style="opacity: 0.6;" class="fa fa-cloud-upload"></i>&nbsp;');
            $('#' + uploadifyExId + '-queue').hide();
            $('#' + uploadifyExId).attr('type', 'uploadify');
        }
        else {//百度上传文件插件
            $uploadifyEx.attr('type', 'webUploader');
            $uploadifyEx.addClass('webUploader');
            $uploadifyEx.html('<div class="btns"><div id="' + uploadifyExId + '-btn" class="btnSelect" style="line-height:' + opt.height + 'px;height:' + (opt.height + 2) + 'px;" ><i style="opacity: 0.6;" class="fa fa-cloud-upload"></i>&nbsp;' + opt.btnName + '</div></div><div id="' + uploadifyExId + '-queue" class="uploadify-queue" style="display:none;"></div></div>');

            var uploader = WebUploader.create({
                auto: true,
                // swf文件路径
                swf: '/Content/scripts/plugins/webuploader/Uploader.swf',
                // 文件接收服务端。
                server: opt.url,
                // 选择文件的按钮。可选。
                // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                pick: {
                    id:'#' + uploadifyExId + "-btn",
                    multiple: !opt.oneFile
                },
                accept: {
                    extensions: opt.fileTypeExts
                },
                // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
                resize: false,

            });



            $uploadifyEx.find('.webuploader-pick').height(opt.height);
            $uploadifyEx.find('.webuploader-pick').width(opt.width);
            //
            uploader.on('startUpload', function (file) {
                var $list = $uploadifyEx.find('.uploadify-queue');
                $list.show();
            });
            // 当有文件被添加进队列的时候
            uploader.on('uploadStart', function (file) {

                var $list = $uploadifyEx.find('.uploadify-queue');
                if (opt.oneFile) {
                    $list.html("");
                }

                var $fileItem = $('<div id="' + file.id + '" class="uploadify-queue-item"></div>');
                $fileItem.append('<span class="fileName">' + file.name + ' (' + learun.countFileSize(file.size) + ')</span><span class="data"></span>');
                $fileItem.append('<div style="float:left;width:50px;margin-right:2px;"><img src="/Content/images/filetype/' + file.ext + '.png" style="width:40px;height:40px;" /></div>');

                $list.append($fileItem);
            });
            //当某一个文件开始触发
            uploader.on('uploadStart', function (file) {
                var $fileItem = $('#' + file.id);
                $fileItem.find('.data').html(" - 0%");
                $fileItem.append('<div class="uploadify-progress"><div class="uploadify-progress-bar" style="width:0%;"></div></div>');
            });
            //上传过程中触发，携带上传速度
            uploader.on('uploadProgress', function (file, percentage) {
                var $fileItem = $('#' + file.id);
                var percentage = percentage * 100 + '%';
                $fileItem.find('.data').html(" - " + percentage);
                $fileItem.find('.uploadify-progress-bar').css('width', percentage);
            });
            //上传成功后
            uploader.on('uploadSuccess', function (file, dataJson) {
                var $fileItem = $('#' + file.id);
                $fileItem.find('.uploadify-progress').remove();

                $fileItem.find('.data').html(' 恭喜您，上传成功！');
                $fileItem.attr("data-fileId", dataJson.fileId);
                $fileItem.prepend('<div class="cancel"><a title="删除" style="display: none;"><i class="fa fa-trash-o "></i></a></div>');
                $fileItem.prepend('<a class="succeed" title="成功"><i class="fa fa-check-circle"></i></a>');

                var _dd = $("#" + uploadifyExId).attr("data-value");
                if (_dd != undefined && _dd != "" && _dd != "undefined") {
                    _dd += ",";
                }
                else {
                    _dd = "";
                }

                $("#" + uploadifyExId).attr("data-value", _dd + dataJson.fileId);

                if (opt.onUploadSuccess) {
                    opt.onUploadSuccess(dataJson);
                }
            });
            //上传失败后
            uploader.on('uploadError', function (file, code) {
                var $fileItem = $('#' + file.id);
                $fileItem.removeClass('uploadify-error');
                $fileItem.find('.uploadify-progress').remove();
                $fileItem.find('.data').html(' 很抱歉，上传失败！');
                $fileItem.append('<div class="cancel"><a title="删除" style="display: none;"><i class="fa fa-trash-o "></i></a></div>');
                $fileItem.append('<span class="error" title="失败"><i class="fa fa-exclamation-circle"></i></span>');

            });
            //上传完成后触发jsonccq  
            uploader.on("uploadComplete", function (file) {
                var $list = $uploadifyEx.find('.uploadify-queue');
                var $fileItem = $('#' + file.id);
                $fileItem.hover(function () {
                    $(this).find('.cancel').find('a').show();
                }, function () {
                    $(this).find('.cancel').find('a').hide();
                });
                $fileItem.find('.cancel').unbind();
                $fileItem.find('.cancel').on('click', function () {
                    var fileId = $fileItem.attr("data-fileId");

                    $fileItem.remove();
                    if ($('#' + uploadifyExId + '-queue').find('.uploadify-queue-item').length == 0) {
                        $('#' + uploadifyExId + '-queue').hide();
                    }

                    var _dd = $("#" + uploadifyExId).attr("data-value");
                    _dd = learun.stringArray(_dd, fileId);
                    $("#" + uploadifyExId).attr("data-value", _dd);

                    if (opt.onFileDelete) {
                        opt.onFileDelete(fileId);
                    }
                    //learun.setForm(
                    //{
                    //    url: "/Utility/RemoveFile?fileId=" + fileId,
                    //    success: function (data) {
                    //        if (data.code == 1) {
                    //            uploader.removeFile(file);
                    //            $fileItem.remove();
                    //            if ($list.find('.uploadify-queue-item').length == 0) {
                    //                $list.hide();
                    //            }
                    //            var _dd = $("#" + uploadifyExId).attr("data-value");
                    //            _dd = learun.stringArray(_dd, fileId);
                    //            $("#" + uploadifyExId).attr("data-value", _dd);
                    //        }
                    //    }
                    //});
                    if ($list.find('.uploadify-queue-item').length == 0) {
                        $list.hide();
                    }
                });

            });
        }
    }

    //附件上传插件初始化
    $.fn.uploadifyFileEx = function (opt) {
        var $uploadifyEx = $(this);
        var uploadifyExId = $uploadifyEx.attr('id');
        if (!uploadifyExId) {
            return false;
        }

        var opt = $.extend({
            btnName: "上传附件",
            url: "",
            onUploadSuccess: false,
            cancel: false,
            height: 30,
            width: 90,
            type: "webUploader",
            fileTypeExts: "",
            oneFile: false,
            onFileDelete: false
        }, opt);

        if (opt.type == "uploadify") {
            $uploadifyEx.removeAttr("id");
            $uploadifyEx.html('<input id="' + uploadifyExId + '" type="file" />');
            $uploadifyEx = $('#' + uploadifyExId);

            if (opt.fileTypeExts == "") {
                opt.fileTypeExts = "*.jpeg;*.jpg;*.png; *.rar;*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.txt; *.xls; *.xlsx;";
            }
            else {
                opt.fileTypeExts = '*.' + opt.fileTypeExts.replace(/,/g, ';*.') + ';';
            }

            $uploadifyEx.uploadify({
                method: 'post',
                uploader: opt.url,
                swf: getInfoTop().contentPath + '/Content/scripts/plugins/uploadify/uploadify.swf',
                buttonText: opt.btnName,
                height: opt.height,
                width: opt.width,
                fileTypeExts: opt.fileTypeExts,//'*.avi;*.mp3;*.mp4;*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.psd; *.rar;*.zip;*.swf;*.log;*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.txt; *.xls; *.xlsx;',jsonccq  
                removeCompleted: false,
                fileSizeLimit: 0,
                onSelect: function (file) {
                    if (opt.oneFile) {
                        $('#' + uploadifyExId + '-queue').find('.uploadify-queue-item').each(function () {
                            //if ($(this).attr('id') != file.id) {
                            //    $(this).remove();
                            //}
                        });
                    }

                    var $fileItem = $("#" + file.id);
                    $fileItem.remove();
                    var _dd = $("#" + uploadifyExId).attr("data-value");
                    _dd = learun.stringArray(_dd, fileId);
                    $("#" + uploadifyExId).attr("data-value", _dd);
                },
                onUploadSuccess: function (file, data) {
                    $("#" + file.id).find('.uploadify-progress').remove();

                    var dataJson = JSON.parse(data);

                    if (opt.onUploadSuccess) {
                        opt.onUploadSuccess(dataJson);
                    }
                },
                onUploadError: function (file) {
                    $("#" + file.id).removeClass('uploadify-error');
                    $("#" + file.id).find('.uploadify-progress').remove();
                },
                onUploadStart: function () {
                    $('#' + uploadifyExId + '-queue').show();
                },
                onCancel: function (file) {
                }
            });
            //$("#" + uploadifyExId + "-button").prepend('<i style="opacity: 0.6;" class="fa fa-cloud-upload"></i>&nbsp;');
            $('#' + uploadifyExId + '-queue').hide();
            //$('#' + uploadifyExId).attr('type', 'uploadify');
        }
        else {//百度上传文件插件
            $uploadifyEx.attr('type', 'webUploader');
            $uploadifyEx.addClass('webUploader');
            $uploadifyEx.html('<div class="btns"><div id="' + uploadifyExId + '-btn" class="btnSelect" style="line-height:' + opt.height + 'px;height:' + (opt.height + 2) + 'px;" ><i style="opacity: 0.6;" class="fa fa-cloud-upload"></i>&nbsp;' + opt.btnName + '</div></div><div id="' + uploadifyExId + '-queue" class="uploadify-queue" style="display:none;"></div></div>');

            var uploader = WebUploader.create({
                auto: true,
                // swf文件路径
                swf: '/Content/scripts/plugins/webuploader/Uploader.swf',
                // 文件接收服务端。
                server: opt.url,
                // 选择文件的按钮。可选。
                // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                pick: {
                    id: '#' + uploadifyExId + "-btn",
                    multiple: !opt.oneFile
                },
                accept: {
                    extensions: opt.fileTypeExts
                },
                // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
                resize: false,

            });



            $uploadifyEx.find('.webuploader-pick').height(opt.height);
            $uploadifyEx.find('.webuploader-pick').width(opt.width);
            //
            uploader.on('startUpload', function (file) {
                var $list = $uploadifyEx.find('.uploadify-queue');
                $list.hide();
            });
            // 当有文件被添加进队列的时候
            uploader.on('uploadStart', function (file) {

                var $list = $uploadifyEx.find('.uploadify-queue');
                $list.hide();
                if (opt.oneFile) {
                    $list.html("");
                }

                var $fileItem = $('<div id="' + file.id + '" class="uploadify-queue-item"></div>');
                $fileItem.append('<span class="fileName">' + file.name + ' (' + learun.countFileSize(file.size) + ')</span><span class="data"></span>');
                $fileItem.append('<div style="float:left;width:50px;margin-right:2px;"><img src="/Content/images/filetype/' + file.ext + '.png" style="width:40px;height:40px;" /></div>');

                $list.append($fileItem);
            });
            //当某一个文件开始触发
            uploader.on('uploadStart', function (file) {
                var $fileItem = $('#' + file.id);
                $fileItem.find('.data').html(" - 0%");
                $fileItem.append('<div class="uploadify-progress"><div class="uploadify-progress-bar" style="width:0%;"></div></div>');
            });
            //上传过程中触发，携带上传速度
            uploader.on('uploadProgress', function (file, percentage) {
                var $fileItem = $('#' + file.id);
                var percentage = percentage * 100 + '%';
                $fileItem.find('.data').html(" - " + percentage);
                $fileItem.find('.uploadify-progress-bar').css('width', percentage);
            });
            //上传成功后
            uploader.on('uploadSuccess', function (file, dataJson) {
                var $fileItem = $('#' + file.id);
                var $list = $uploadifyEx.find('.uploadify-queue');
                $list.hide();
                $fileItem.find('.uploadify-progress').remove();

                if (opt.onUploadSuccess) {
                    opt.onUploadSuccess(dataJson);
                }
            });
            //上传失败后
            uploader.on('uploadError', function (file, code) {
                var $fileItem = $('#' + file.id);
                $fileItem.removeClass('uploadify-error');
                $fileItem.find('.uploadify-progress').remove();

            });
            //上传完成后触发jsonccq  
            uploader.on("uploadComplete", function (file) {
                uploader.removeFile(file);
                //$fileItem.remove();
                //if ($list.find('.uploadify-queue-item').length == 0) {
                //$list.hide();
                //}
                var _dd = $("#" + uploadifyExId).attr("data-value");
                _dd = learun.stringArray(_dd, fileId);
                $("#" + uploadifyExId).attr("data-value", _dd);
            });
        }
    }
    $.fn.uploadifyExSet = function (keyValue, opt) {
        var $uploadifyExSet = $(this);
        var uploadifyExSetId = $uploadifyExSet.attr('id');
        if (!uploadifyExSetId) {
            return false;
        }
        var $uploadifyExSetQueue = $('#' + uploadifyExSetId + '-queue');

        if ($uploadifyExSetQueue.length < 1) {
            $uploadifyExSetQueue = $('<div id="' + uploadifyExSetId + '-queue" class="uploadify-queue" style="display:none;"></div>');
            $uploadifyExSet.append($uploadifyExSetQueue);
        }

        learun.setForm(
        {
            url: "/Utility/GetFiles?fileIdList=" + keyValue,
            success: function (data) {
                $.each(data, function (id, item) {
                    $uploadifyExSetQueue.show();
                    var _html = '<div id="' + item.F_Id + '"  class="uploadify-queue-item olduploadify-queue-item" ><a class="succeed" title="成功"><i class="fa fa-check-circle"></i></a><div style="float:left;width:50px;margin-right:2px;"><img src="/Content/images/filetype/' + item.F_FileType + '.png" style="width:40px;height:40px;"></div>';
                    if (opt == undefined || opt.isRemove) {
                        _html += '<div class="cancel remove" data-fileId="' + item.F_Id + '"><a title="删除" style="display: none;"><i class="fa fa-trash-o "></i></a></div>';
                    }
                    if (opt == undefined || opt.isDown) {
                        _html += '<div class="cancel down" data-fileId="' + item.F_Id + '"><a title="下载" style="display: none;margin-right:10px;"><i class="fa fa-download"></i></a></div>';
                    }
                    _html += '<span class="fileName">' + item.F_FileName + '</span><span class="data"></span></div>';
                    $uploadifyExSetQueue.append(_html);
                });
            }
        });
        $uploadifyExSet.attr("data-value", keyValue);

        $uploadifyExSetQueue.find(".uploadify-queue-item").hover(function () {
            $(this).find('.cancel').find('a').show();
        }, function () {
            $(this).find('.cancel').find('a').hide();
        });

        $uploadifyExSetQueue.find(".olduploadify-queue-item").find('.remove').on('click', function () {
            var fileId = $(this).attr("data-fileId");
            learun.setForm(
            {
                url: "/Utility/RemoveFile?fileId=" + fileId,
                success: function (data) {
                    if (data.code == 1) {
                        $("#" + fileId).remove();
                        var _dd = $("#" + uploadifyExSetId).attr("data-value");
                        _dd = learun.stringArray(_dd, fileId);
                        $("#" + uploadifyExSetId).attr("data-value", _dd);

                        if ($('#' + uploadifyExSetId + '-queue').find('.uploadify-queue-item').length == 0) {
                            $('#' + uploadifyExSetId + '-queue').hide();
                        }
                    }
                }
            });
            if ($('#' + uploadifyExSetId + '-queue').find('.uploadify-queue-item').length == 0) {
                $('#' + uploadifyExSetId + '-queue').hide();
            }
        });
        $uploadifyExSetQueue.find(".olduploadify-queue-item").find('.down').on('click', function () {
            var fileId = $(this).attr("data-fileId");
            learun.downFile({ url: "/Utility/DownFile", data: ("fileId=" + fileId), method: 'post' });
        });
    }
    //对Date属性增加一个方法
    Date.prototype.DateAdd = function (strInterval, Number) {
        //y年 q季度 m月 d日 w周 h小时 n分钟 s秒 ms毫秒
        var dtTmp = this;
        switch (strInterval) {
            case 's': return new Date(Date.parse(dtTmp) + (1000 * Number));
            case 'n': return new Date(Date.parse(dtTmp) + (60000 * Number));
            case 'h': return new Date(Date.parse(dtTmp) + (3600000 * Number));
            case 'd': return new Date(Date.parse(dtTmp) + (86400000 * Number));
            case 'w': return new Date(Date.parse(dtTmp) + ((86400000 * 7) * Number));
            case 'q': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number * 3, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
            case 'm': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
            case 'y': return new Date((dtTmp.getFullYear() + Number), dtTmp.getMonth(), dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        }
    };
    //员工多选框
    $.fn.userSelectBox = function () {
        var $select = $(this);
        var selectId = $select.attr('id');
        if (!selectId) {
            return false;
        };
        var opt = $.extend({
            //请选择
            description: "==请选择==",
        }, opt);
        $select.on('click', function () {//绑定点击事件jsonccq  
            var data = $select.attr("data-value");
            learun.dialogOpen({
                id: "SelectUserIndex",
                title: '用户选择',
                url: '/BaseManage/User/SelectUserIndex?data=' + data,
                width: "800px",
                height: "520px",
                callBack: function (iframeId) {
                    getInfoTop().frames[iframeId].AcceptClick(function (data) {
                        var ids = [], names = [];
                        $.each(data, function (i, item) {
                            ids.push(item.id);
                            names.push(item.name);
                        });
                        $select.attr("data-value", String(ids)).attr("data-text", String(names));
                        $select.find('.ui-select-text').html(String(names)).css('color', '#000');
                    });
                }
            });
        });
    };

    $.fn.zTreeEx = function (opt) {
        //自定义属性
        var opt = $.extend({
            //鼠标单击
            onClick:false
        }, opt);
        //zTree 属性变量
        var setting = {
            data: {
                simpleData: {
                    enable: true
                }
            },
            view: {
                selectedMulti: false
            },
            callback: {
                onClick: opt.onClick
            }
        };
    }
})(window.jQuery, window.learun);