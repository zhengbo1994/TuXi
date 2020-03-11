
(function ($) {
    $.fn.timeQuantumControl = function (settings) {
        //默认选项
        var defaluts =
            {
                startTime: "2018-1-23",//开始时间
                endTime: "2018-1-30" //结束时间
            };
        $.extend(defaluts, settings);
        var me = $(this);
        me.empty();
        var id = $(this).attr("id");
        var id = me.attr("id");
        if (id == null || id == "") {
            id = "timeQuantum" + new Date().getTime();
            me.attr("id", id);
        }
        //初始化的Html
        var html = '<div id="divTime">' +
        '<div style="margin-left:5px;">' +
            '<a style="white-space: nowrap; font-weight: normal;line-height:34px; float:left;">开始时间：</a>' +
            '<input id="dateTime_start" type="text" class="form-control input-wdatepicker" onfocus="WdatePicker()" style="width: 154px; float: left;" />' +
            '<a style="white-space: nowrap; font-weight: normal; line-height: 34px; float: left; margin-left:50px;">结束时间：</a>' +
            '<input id="dateTime_end" type="text" class="form-control input-wdatepicker" onfocus="WdatePicker()" style="width: 154px; float: left;" />' +
        '</div>' +
    '</div>';

        me.append(html);
        var comboxData = [];
        var dtStart = me.find("#dateTime_start");
        var dtEnd = me.find("#dateTime_end");
        dtStart.val(formatDate(defaluts.startTime, "yyyy-MM-dd"));
        dtEnd.val(formatDate(defaluts.endTime, "yyyy-MM-dd"));

        function GetDatetime() {
            var params = {
                startTime: "",
                endTime: ""
            };
            params.startTime = (me.find("#dateTime_start")).val();
            params.endTime = (me.find("#dateTime_end")).val()
            
            return params;
        };
        me[0].d = {
            GetDatetime: function () {
                return GetDatetime();
            }
        };
        return me;
    }
    $.fn.GetDatetime = function () {
        if (this[0].d) {
            return this[0].d.GetDatetime();
        }
        return null;
    };
})(jQuery);