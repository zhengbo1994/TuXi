/// <reference path="../select2/select2.min.js" />
(function ($) {
    $.fn.comboxSearchControl = function (settings) {
        //默认选项
        var defaluts =
            {
                item: "Client_Trade",
                value:""
            };
        $.extend(defaluts, settings);
        var me = $(this);
        me.empty();
        var id = $(this).attr("id");
        var id = me.attr("id");
        if (id == null || id == "") {
            id = "comboxSearch" + new Date().getTime();
            me.attr("id", id);
        }

        me.height(defaluts.height);
        me.width(defaluts.width);
        //初始化的Html
        var html = '<div><select class="js-example-basi-single" name="state" style="width:250px;">' +
            '</select></div>';

        me.append(html);
        me.find(".js-example-basi-single").select2();
        $.ajax({
            url: '../../SystemManage/DataItemDetail/GetDataItemListJson?EnCode='+defaluts.item,
            async: false,
            success: function (json) {
                var arr = JSON.parse(json);
                var temp = "";
                for (var i = 0; i < arr.length; i++) {
                    temp += '<option value="' + arr[i].F_ItemDetailId + '">' + arr[i].F_ItemName + '</option>';
                }
                me.find(".js-example-basi-single").append(temp);
            }
        });
        if (defaluts.value != "")
        {
            me.find(".js-example-basi-single").val(defaluts.value).trigger("change");
        }
        function GetComboxValue() {
            var item = {
                value: "",
                text: ""
            };
            item.value = me.find(".js-example-basi-single").select2('val');
            item.text = me.find(".js-example-basi-single").find('option:selected').text();
            return item;
        };
        function SetComboxValue(value) {
            me.find(".js-example-basi-single").val(value).trigger("change");
        };
        me[0].d = {
            SetComboxValue: function (value) {
                return SetComboxValue(value);
            },
            GetComboxValue: function () {
                return GetComboxValue();
            }
        };
        return me;
    }
    $.fn.SetComboxValue = function (value) {
        if (this[0].d) {
            return this[0].d.SetComboxValue(value);
        }
        return null;
    };
    $.fn.GetComboxValue = function () {
        if (this[0].d) {
            return this[0].d.GetComboxValue();
        }
        return null;
    };
})(jQuery);