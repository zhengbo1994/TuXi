/*!
 * 版 本 LearunADMS V6.1.1.7 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 *兼容之前版本的js,不建议使用这些
 * 陈小二
 */
(function ($) {
    $.fn.ComboBox = function (options) {
        options.maxHeight = options.height;
        return $(this).comboBox(options);
    }
    $.fn.ComboBoxEx = function (options) {
        options.maxHeight = options.height;
        return $(this).comboBoxEx(options);
    }
    $.fn.ComboBoxSetValue = function (value) {
        return $(this).comboBoxSetValue(value);
    }
    $.fn.ComboBoxTree = function (options) {
        options.maxHeight = options.height;
        return $(this).comboBoxTree(options);
    }
    $.fn.ComboBoxTreeSetValue = function (value) {
        return $(this).comboBoxTreeSetValue(value);
    }
    $.fn.GetWebControls = function (keyValue) {
        return $(this).getWebControls(keyValue);
    };
    $.fn.SetWebControls = function (data) {
        $(this).setWebControls(data);
    }
    $.fn.Contextmenu = $.fn.contextmenu;
    $.fn.LeftListShowOfemail = $.fn.leftListShowOfemail;

    $.SaveForm = function (options) {
        learun.saveForm(options);
    }
    $.SetForm = function (options) {
        learun.setForm(options);
    }
    $.RemoveForm = function (options) {
        learun.removeForm(options);
    }
    $.ConfirmAjax = function (options) {
        learun.confirmAjax(options);
    }
    $.ExistField = function (controlId, url, param) {
        learun.existField(controlId, url, param);
    }
    $.getDataForm = function (options) {
        learun.getDataForm(options);
    }
})(window.jQuery);
Loading = function (bool, text) {
    learun.loading({ isShow: bool, text: text });
}
tabiframeId = function () {
    return learun.tabiframeId();
}
dialogTop = function (msg, type) {
    learun.dialogTop({ msg: msg, type: type });
}
dialogAlert = function (msg, type) {
    learun.dialogAlert({ msg: msg, type: type });
}
dialogMsg = function (msg, type) {
    learun.dialogMsg({ msg: msg, type: type });
}
dialogOpen = function (options) {
    learun.dialogOpen(options);
}
dialogContent = function (options) {
    learun.dialogContent(options);
}
dialogConfirm = function (msg, callBack) {
    learun.dialogConfirm({ msg: msg, callBack: callBack });
}
dialogClose = function () {
    learun.dialogClose();
}
reload = function () {
    location.reload();
    return false;
}
newGuid = function () {
    return learun.newGuid();
}
formatDate = function (v, format) {
    return learun.formatDate(v, format);
};
toDecimal = function (num) {
    return learun.toDecimal(num);
}
request = function (keyValue) {
    return learun.request(keyValue);
}
changeUrlParam = function (url, key, value) {
    return learun.changeUrlParam(url, key, value);
}
$.currentIframe = function () {
    return learun.currentIframe();
}
$.isbrowsername = function () {
    return learun.isbrowsername();
}
$.download = function (url, data, method) {
    learun.downFile({ url: url, data: data, method: method });
};
$.standTabchange = function (object, forid) {
    learun.changeStandTab({ obj: object, id: forid });
}
$.isNullOrEmpty = function (obj) {
    return learun.isNullOrEmpty(obj);
}
IsNumber = function (obj) {
    return learun.isNumber(obj);
}
IsMoney = function (obj) {
    return learun.isMoney(obj);
}
$.arrayClone = function (data) {
    return learun.arrayCopy(data);
}
$.windowWidth = function () {
    return $(window).width();
}
$.windowHeight = function () {
    return $(window).height();
}
checkedArray = function (id) {
    return learun.checkedArray(id);
}
checkedRow = function (id) {
    return learun.checkedRow(id);
}


$(function () {
    $(".ui-filter-text").click(function () {
        if ($(this).next('.ui-filter-list').is(":hidden")) {
            $(this).css('border-bottom-color', '#fff');
            $(".ui-filter-list").slideDown(10);
            $(this).addClass("active")
        } else {
            $(this).css('border-bottom-color', '#ccc');
            $(".ui-filter-list").slideUp(10);
            $(this).removeClass("active")
        }
    });
    $(".profile-nav li").click(function () {
        $(".profile-nav li").removeClass("active");
        $(".profile-nav li").removeClass("hover");
        $(this).addClass("active");
    }).hover(function () {
        if (!$(this).hasClass("active")) {
            $(this).addClass("hover");
        }
    }, function () {
        $(this).removeClass("hover");
    });
});


