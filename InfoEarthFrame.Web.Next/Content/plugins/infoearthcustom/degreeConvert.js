/// <reference path="../../jquery/jquery-2.0.3.min.js" />
(function ($) {
    $.fn.degreeConvertControl = function (settings) {
        //默认选项
        var defaluts =
            {
                degree: 113.75,
            };
        $.extend(defaluts, settings);
        var me = $(this);
        me.empty();
        var id = $(this).attr("id");
        var id = me.attr("id");
        if (id == null || id == "") {
            id = "degreeConvert" + new Date().getTime();
            me.attr("id", id);
        }

        me.height(defaluts.height);
        me.width(defaluts.width);
        //初始化的Html
        var html = '<div>' +
        '<input type="text" id="degree" style="width:100px;" />' +
        '<span>°</span>' +
        '<input type="text" id="minute" style="width:100px;" />' +
        '<span>′</span>' +
        '<input type="text" id="second" style="width:100px;" />' +
        '<span>″</span>' +
        '</div>';

        me.append(html);
        if (defaluts.degree != "")
        {
            var _degree = Math.floor(defaluts.degree);//度
            var _minute = Math.floor((defaluts.degree - _degree) * 60);//分
            var _second = Math.round((defaluts.degree - _degree) * 3600 % 60);//秒
            me.find("#degree").val(_degree);
            me.find("#minute").val(_minute);
            me.find("#second").val(_second);
        }
        
        function GetDegree() {
            var _degree = "";
            _degree = Math.abs(me.find("#degree").val());
            var _minute = Math.abs(me.find("#minute").val()) / 60;
            var _second = Math.abs(me.find("#second").val()) / 3600;
            _degree = _degree + (_minute + _second);
            return _degree;
        };
        me[0].d = {
            GetDegree: function () {
                return GetDegree();
            }
        };
        return me;
    }
    $.fn.GetDegree = function () {
        if (this[0].d) {
            return this[0].d.GetDegree();
        }
        return null;
    };
})(jQuery);