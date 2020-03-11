(function ($) {
    $.fn.dictionaryControl = function (settings) {
        //默认选项
        var defaluts =
            {
                dicCode: "",//字典Code
                type: "",//类型：checkbox、radio
                wrap: {
                    interlaced: false,//是否每行换行,默认不换行
                    indexBr: "",//指定数字换行
                },
                QTHtml: {
                    html: "",//其他html
                    index: ""//在第几个后面
                },//其他html
            };
        $.extend(defaluts, settings);
        var _dictionnaryControl = $(this).empty();
        var id = $(this).attr("id");
        $.ajax({
            url: "../../SystemManage/DataItemDetail/GetDataItemTreeJson?EnCode=" + defaluts.dicCode,
            async: false,
            success: function (data) {
                var dataDMLX = eval(data);
                for (var i = 0; i < dataDMLX.length; i++) {
                    var dicHtml = "<label><input  type=\"" + defaluts.type + "\" name=\"" + id + "\" value=\"" + dataDMLX[i].value + "\" />" + dataDMLX[i].text + "</label>";
                    //每行换行
                    if (defaluts.wrap.interlaced) {
                        if (dataDMLX.length - 1 != i) {
                            dicHtml += "<Br />";
                        }
                    }
                    //指定行换行
                    if (!defaluts.wrap.interlaced && defaluts.wrap.indexBr) {
                        if (defaluts.wrap.indexBr == i) {
                            dicHtml += "<Br />";
                        }
                    }
                    //其他
                    if (defaluts.QTHtml.html) {
                        if (defaluts.QTHtml.index && defaluts.QTHtml.index - 1 == i) {
                            dicHtml += defaluts.QTHtml.html;

                        } else if (!defaluts.QTHtml.index && i == dataDMLX.length - 1) {
                            dicHtml += defaluts.QTHtml.html;
                        }
                        if (defaluts.indexBr && defaluts.indexBr == i && !defaluts.wrap.interlaced) {
                            dicHtml += "<Br />";
                        }
                       
                    }
                    _dictionnaryControl.append(dicHtml);

                }
            }, error: function (e) {
            },
            cache: false
        });
        $(this)[0].dic = {
            setDictionaryControlValue: function (value) {
                var name = $(this).attr('id');
                $("input[name='" + name + "']").removeAttr("checked");//清空绑定
                if (value.indexOf(",") == -1) {
                    $("input[name='" + name + "'][value=" + value + "]").prop("checked", true);
                }
                else {
                    var checkValueData = value.split(',');
                    for (var i = 0; i < checkValueData.length; i++) {
                        $("input[name='" + name + "'][value=" + checkValueData[i] + "]").prop("checked", true);
                    }
                }

            },
            getDictionaryControlValue:function () {
                var chekcValue = "";
                $("[name='" + $(this).attr('id') + "']").each(function () {
                    if ($(this).is(":checked")) {
                        chekcValue += $(this).attr('value') + ",";
                    }
                });
                return chekcValue;
            }
        };
        return _dictionnaryControl;
    };
    $.fn.setDictionaryControlValue = function (value) {
        if (this[0].dic) {
            this[0].dic.setDictionaryControlValue(value);
        }
    };
    $.fn.getDictionaryControlValue = function () {
        if (this[0].dic) {
            this[0].dic.getDictionaryControlValue();
        }
    };
})(jQuery);