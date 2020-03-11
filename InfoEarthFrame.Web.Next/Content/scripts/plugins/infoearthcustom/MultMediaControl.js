/// <reference path="../../utils/MultMedia/multMedia.js" />
(function ($) {
    $.fn.MultMediaControl = function (settings) {
        //默认选项
        var defaluts =
            {
                divId: "divPoumian",
            };
        $.extend(defaluts, settings);
        var me = $(this);
        me.empty();
        var id = $(this).attr("id");
        var id = me.attr("id");
        if (id == null || id == "") {
            id = "multMedia" + new Date().getTime();
            me.attr("id", id);
        }
        //初始化的Html
        var html = '<div id="' + defaluts.divId + '" style="float:left;width:95%;">' +
                '</div>' +
                '<div style="float:right;width:5%; margin-top:3px;margin-bottom:3px;">' +
                    '<a id="lr-add" class="btn btn-default" onclick="cfupfile(\''+defaluts.divId+'\')"><i class="fa fa-plus"></i>添加</a>' +
                '</div>';

        me.append(html);
        me[0].d = {
            SaveFileInfo: function (belongGuid) {
                return SaveFileInfo(belongGuid);
            },
            ShowMedia: function (divId, ClassID, moduleID, belongGuid) {
                return showMedia(divId, ClassID, moduleID, belongGuid);
            }
        };
        return me;
    }
    $.fn.SaveFileInfo = function (belongGuid) {
        if (this[0].d) {
            return this[0].d.SaveFileInfo(belongGuid);
        }
        return null;
    };
    $.fn.ShowMedia = function (divId, ClassID, moduleID, belongGuid) {
        if (this[0].d) {
            return this[0].d.ShowMedia(divId, ClassID, moduleID, belongGuid);
        }
        return null;
    };
})(jQuery);