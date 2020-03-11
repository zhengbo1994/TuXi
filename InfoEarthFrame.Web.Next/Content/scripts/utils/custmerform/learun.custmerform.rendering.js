/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 表单渲染器
 * 陈小二
 */
/*
 *formData 表单数据
 *data     实例数据
*/
(function ($, learun) {
    "use strict";

    var qopt={};
    var callback = {};
    var rendering = {
        init: function (opt, $this) {//渲染自定义表单
            if (typeof learun === 'undefined') {
                return false;
            }
            $this.html("");
            callback = {};
            var layout = {
                area: function (data) {//渲染区域
                    var $table = $('<table class="form" data-id="' + data.id + '"  data-type="' + data.type + '"></table>');
                    $table.css('margin-top', data.margintop + 'px');
                    return $table;
                },
                field: function ($table, data) {//渲染字段
                    var type = parseInt($table.attr('data-type'));
                    var $tr = $('<tr></tr>');
                    $table.append($tr);
                    $.each(data, function (i, item) {
                        var $td = learun.components[item.type].renderTable(item);
                        if ($tr.find('td').length >= type) {
                            $tr = $('<tr></tr>');
                            $table.append($tr);
                        }
                        if (!!$td[0].callback)
                        {
                            if (callback[$td[0].callback.data.type] == undefined) {
                                callback[$td[0].callback.data.type] = {};
                            }
                            callback[$td[0].callback.data.type][$td[0].callback.data.id] = $td[0].callback;//部分渲染函数需要在页面显示完全后执行才有效果
                        }
                        $tr.append($td);
                    });
                }
            };
            $.each(opt.formData, function (i, item) {
                var $table = layout.area(item.area);
                layout.field($table, item.fields);
                $this.append($table);
                $.each(callback, function (i, item) {//加载回调函数
                    learun.components[i].makeCallback(item);
                });
            });
        },
        get: function ($this) {
            var data = [];
            $this.find('.custmerTd').each(function () {
                var $obj = $(this);
                var type = $obj.attr('data-type');
                var point = learun.components[type].getValue($obj);
                if (point.value == undefined) {
                    point.vaule = "";
                }
                data.push(point);
            });
            return data;
        },
        set: function (opt, $this) {
            $.each(opt.data, function (i, item) {
                var $obj = $this.find('[data-value="' + item.field + '"]');
                learun.components[item.type].setValue($obj, item);
            });
        },
        valid: function ($this) {
            var validFlag = true;
            $this.find('.custmerTd').each(function () {
                var $obj = $(this);
                var type = $obj.attr('data-type');
                var flag = learun.components[type].validTable($obj);
                if (!flag)
                {
                    validFlag = false;
                    return flag
                }
            });
            return validFlag;
        }
    };

    //对外暴露接口
    $.fn.formRendering = function (type, opt) {
        var $this = $(this);
        qopt = $.extend(qopt, opt);
        switch (type)
        {
            case "init":
                rendering.init(qopt, $this);
                break;
            case "get":
                if (rendering.valid($this)) {
                    return rendering.get($this);
                }
                return false;
                break;
            case "set":
                rendering.set(qopt, $this);
                break;
        }
    }
})(window.jQuery, window.learun);