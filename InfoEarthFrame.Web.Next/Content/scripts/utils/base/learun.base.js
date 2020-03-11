/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 公共JS基础库
 * 陈小二
 */
var frameName = "武汉地大信息工程股份有限公司";
window.Sign = frameName;
function getInfoTop() {
    var parentF = window.self.parent;
    var selfF = window.self;
    //独立访问或者自身框架首页
    if (parentF == selfF && selfF.Sign == frameName) {
        //var test = selfF == window.top;
        //console.log(test);
        return selfF;
    }
    while (parentF != window.self) {
        try
        {
            //自身框架
            if (parentF == window.top && parentF.Sign == frameName) {
                //var test = parentF == window.top;
                //console.log(test);
                return parentF;
            }
            //其他框架
            if (parentF.Sign == undefined || parentF.Sign != frameName) {
                //var test = selfF == window.top;
                //console.log(test);
                return selfF;
            }
        }
        catch (e) {
            return selfF;
        }
        selfF = parentF;
        parentF = parentF.parent;
    }
}

function isLoadData() {
    try {
        //独立访问或者自身框架首页
        if (window.self == window.top || window.top.Sign != frameName) {
            return true;
        } else {
            return false;
        }
    }
    catch (e) {
        return true;
    }
}

window.learun = {};

(function ($, learun) {
    "use strict";
    //基础方法
    $.extend(learun, {
        //初始化
        init: function (opt) {
            learun.theme.type = opt.themeType;
            learun.data.init(opt.callBack);
        },
        childInit: function (opt) {
            learun.data.init(function () { window.self.$('.toolbar').authorizeButton(); });
           
            learun.theme.setType();
           
            if (learun.excel != undefined)
            {
                learun.excel.init();
            }
            learun.ajaxLoading(false);
        },
        //皮肤主题
        theme: {
            type: "1",
            setType: function () {
                switch (getInfoTop().learun.theme.type) {
                    case "1"://经典版
                        $('body').addClass('uiDefault');
                        break;
                    case "2"://风尚版
                        $('body').addClass('uiLTE');
                        break;
                    case "3"://炫动版
                        $('body').addClass('uiWindows');
                        break;
                    case "4"://飞扬版
                        $('body').addClass('uiPretty');
                        break;
                    case "5"://地下水
                        $('body').addClass('uiWater');
                        break;
                }
            }
        },
        //加载提示
        loading: function (ops) {//加载动画显示与否
            var ajaxbg = getInfoTop().$("#loading_background,#loading_manage");
            if (ops.isShow) {
                ajaxbg.show();
            } else {
                if (getInfoTop().$("#loading_manage").attr('istableloading') == undefined) {
                    ajaxbg.hide();
                    getInfoTop().$(".ajax-loader").remove();
                }
            }
            if (!!ops.text) {
                getInfoTop().$("#loading_manage").html(ops.text);
            } else {
                getInfoTop().$("#loading_manage").html("正在拼了命为您加载…");
            }
            getInfoTop().$("#loading_manage").css("left", (getInfoTop().$('body').width() - getInfoTop().$("#loading_manage").width()) / 2 - 54);
            getInfoTop().$("#loading_manage").css("top", (getInfoTop().$('body').height() - getInfoTop().$("#loading_manage").height()) / 2);
        },
        ajaxLoading: function (isShow) {
            var $obj = $('#ajaxLoader');
            if (isShow) {
                $obj.show();
            }
            else {
                $obj.fadeOut();
            }
        },
        //获取窗口Id
        tabiframeId: function () {//tab窗口Id
            try{
                return getInfoTop().$(".LRADMS_iframe:visible").attr("id");
            }catch(e){
                return "";
            }
        },
        //获取当前窗口
        currentIframe: function () {
            try{
                if (getInfoTop().frames[learun.tabiframeId()].contentWindow) {
                    return getInfoTop().frames[learun.tabiframeId()].contentWindow;
                }
                else {
                    return getInfoTop().frames[learun.tabiframeId()];
                }
            } catch (e) {
                return getInfoTop().frames[learun.tabiframeId()];
            }
        },
        //获取iframe窗口
        getIframe: function (Id) {
            var obj = frames[Id];
            if (obj != undefined) {
                if (obj.contentWindow != undefined) {
                    return obj.contentWindow;
                }
                else {
                    return obj;
                }
            }
            else {
                return null;
            }
        },
        //刷新页面
        reload: function () {
            location.reload();
            return false;
        },
        //提示框
        dialogTop: function (opt) {
            $(".tip_container").remove();
            var bid = parseInt(Math.random() * 100000);
            $("body").prepend('<div id="tip_container' + bid + '" class="container tip_container"><div id="tip' + bid + '" class="mtip"><i class="micon"></i><span id="tsc' + bid + '"></span><i id="mclose' + bid + '" class="mclose"></i></div></div>');
            var $this = $(this);
            var $tip_container = $("#tip_container" + bid);
            var $tip = $("#tip" + bid);
            var $tipSpan = $("#tsc" + bid);
            //先清楚定时器
            clearTimeout(window.timer);
            //主体元素绑定事件
            $tip.attr("class", opt.type).addClass("mtip");
            $tipSpan.html(opt.msg);
            $tip_container.slideDown(300);
            //提示层隐藏定时器
            window.timer = setTimeout(function () {
                $tip_container.slideUp(300);
                $(".tip_container").remove();
            }, 4000);
            $("#tip_container" + bid).css("left", ($(window).width() - $("#tip_container" + bid).width()) / 2);
        },
        dialogOpen: function (opt) {
            learun.loading({ isShow: true });
            var opt = $.extend({
                id: null,
                title: '系统窗口',
                width: "100px",
                height: "100px",
                url: '',
                shade: 0.3,
                //maxmin:true,
                //anim: 1,
                //moveOut:true,
                btn: ['确认', '关闭'],
                callBack: null
            }, opt);
            var _url = opt.url;
            var _width = getInfoTop().learun.windowWidth() > parseInt(opt.width.replace('px', '')) ? opt.width : getInfoTop().learun.windowWidth() + 'px';
            var _height = getInfoTop().learun.windowHeight() > parseInt(opt.height.replace('px', '')) ? opt.height : getInfoTop().learun.windowHeight() + 'px';
            getInfoTop().layer.open({
                id: opt.id,
                type: 2,
                shade: opt.shade,
                title: opt.title,
                fix: false,
                //maxmin: opt.maxmin,
                //anim: opt.anim,
                //moveOut: opt.moveOut,
                area: [_width, _height],
                content: getInfoTop().contentPath + _url,
                btn: opt.btn,
                success: function (obj, index) {
                    //$(obj).ap
                    learun.loading({ isShow: false });
                },
                yes: function (index,layero) {
                    opt.callBack(opt.id);
                    //opt.callBack("layui-layer-iframe"+index);
                    if (opt.isClose != undefined && opt.isClose) {
                        getInfoTop().layer.close(index);
                    }
                }, cancel: function () {
                    if (opt.cancel != undefined) {
                        opt.cancel();
                    }
                    return true;
                }
            });
        },
        dialogContent: function (opt) {
            var opt = $.extend({
                id: null,
                title: '系统窗口',
                width: "100px",
                height: "100px",
                content: '',
                btn: ['确认', '关闭'],
                callBack: null
            }, opt);
            getInfoTop().layer.open({
                id: opt.id,
                type: 1,
                title: opt.title,
                fix: false,
                area: [opt.width, opt.height],
                success: function (obj, index) {
                    learun.loading({ isShow: false });
                },
                content: opt.content,
                btn: opt.btn,
                yes: function () {
                    opt.callBack(opt.id);
                    //opt.callBack("layui-layer-iframe" + index);
                }
            });
        },
        dialogAlert: function (opt) {
            if (opt.type == -1) {
                opt.type = 2;
            }
            getInfoTop().layer.alert(opt.msg, {
                icon: opt.type,
                title: "提示",
                success: function (obj, index) {
                    learun.loading({ isShow: false });
                }
            });
        },
        dialogConfirm: function (opt) {
            getInfoTop().layer.confirm(opt.msg, {
                icon: 7,
                title: "提示",
                btn: ['确认', '取消'],
                success: function (obj, index) {
                    learun.loading({ isShow: false });
                }
            }, function (index, layero) {
                opt.callBack(true);
                if (opt.isClose != undefined && opt.isClose) {
                    getInfoTop().layer.close(index);
                }
            }, function () {
                opt.callBack(false);
            });
        },
        dialogMsg: function (opt) {
            if (opt.type == -1) {
                opt.type = 2;
            }
            getInfoTop().layer.msg(opt.msg, { icon: opt.type, time: 4000, shift: 5 });
        },
        dialogClose: function () {
            try {
                var index = getInfoTop().layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                var $IsdialogClose = getInfoTop().$("#layui-layer" + index).find('.layui-layer-btn').find("#IsdialogClose");
                var IsClose = $IsdialogClose.is(":checked");
                if ($IsdialogClose.length == 0) {
                    IsClose = true;
                }
                if (IsClose) {
                    getInfoTop().layer.close(index);
                } else {
                    location.reload();
                }
            } catch (e) {
                alert(e)
            }
        },
        //下载文件
        downFile: function (opt) {
            if (opt.url && opt.data) {
                opt.data = typeof opt.data == 'string' ? opt.data : jQuery.param(opt.data);
                var inputs = '';
                $.each(opt.data.split('&'), function () {
                    var pair = this.split('=');
                    inputs += '<input type="hidden" name="' + pair[0] + '" value="' + pair[1] + '" />';
                });
                $('<form action="' + opt.url + '" method="' + (opt.method || 'post') + '">' + inputs + '</form>').appendTo('body').submit().remove();
            };
        },
        //获取url参数值
        request: function (keyValue) {
            //var search = decodeURI(location.search).slice(1);
            var search = (location.search).slice(1);
            var arr = search.split("&");
            for (var i = 0; i < arr.length; i++) {
                var ar = arr[i].split("=");
                if (ar[0] == keyValue) {
                    if (unescape(ar[1]) == 'undefined') {
                        return "";
                    } else {
                        return unescape(ar[1]);
                    }
                }
            }
            return "";
        },
        //改变url参数值
        changeUrlParam: function (url, key, value) {
            var newUrl = "";
            var reg = new RegExp("(^|)" + key + "=([^&]*)(|$)");
            var tmp = key + "=" + value;
            if (url.match(reg) != null) {
                newUrl = url.replace(eval(reg), tmp);
            } else {
                if (url.match("[\?]")) {
                    newUrl = url + "&" + tmp;
                }
                else {
                    newUrl = url + "?" + tmp;
                }
            }
            return newUrl;
        },
        //获取游览器名称
        getBrowserName: function () {
            var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串backcq   
            var isOpera = userAgent.indexOf("Opera") > -1;
            if (isOpera) {
                return "Opera"
            }; //判断是否Opera浏览器
            if (userAgent.indexOf("Firefox") > -1) {
                return "FF";
            } //判断是否Firefox浏览器
            if (userAgent.indexOf("Chrome") > -1) {
                if (window.navigator.webkitPersistentStorage == undefined) {
                    return "Edge";
                }
                if (window.navigator.webkitPersistentStorage.toString().indexOf('DeprecatedStorageQuota') > -1) {
                    return "Chrome";
                } else {
                    return "360";
                }
            }//判断是否Chrome浏览器//360浏览器
            if (userAgent.indexOf("Safari") > -1) {
                return "Safari";
            } //判断是否Safari浏览器
            if (userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera) {
                return "IE";
            }//判断是否IE浏览器
        },
        //改变树状tab状态
        changeStandTab: function (opt) {
            $(".standtabactived").removeClass("standtabactived");
            $(opt.obj).addClass("standtabactived");
            $('.standtab-pane').css('display', 'none');
            $('#' + opt.id).css('display', 'block');
        },
        //获取窗口宽
        windowWidth: function () {
            return $(window).width();
        },
        //获取窗口高度
        windowHeight: function () {
            return $(window).height();
        },
        //ajax通信方法
        ajax: {
            asyncGet: function (opt) {
                var data = null;
                var opt = $.extend({
                    type: "GET",
                    dataType: "json",
                    async: false,
                    cache: false,
                    success: function (d) {
                        data = d;
                    }
                }, opt);
                $.ajax(opt);
                return data;
            },
            asyncPost: function (opt) {
                var data = null;
                var opt = $.extend({
                    type: "POST",
                    dataType: "json",
                    async: false,
                    cache: false,
                    success: function (d) {
                        data = d;
                    }
                }, opt);
                $.ajax(opt);
                return data;
            }
        },
        //创建一个GUID
        createGuid: function () {
            var guid = "";
            for (var i = 1; i <= 32; i++) {
                var n = Math.floor(Math.random() * 16.0).toString(16);
                guid += n;
                if ((i == 8) || (i == 12) || (i == 16) || (i == 20)) guid += "-";
            }
            return guid;
        },
        //判断是否为空
        isNullOrEmpty: function (obj) {
            if ((typeof (obj) == "string" && obj == "") || obj == null || obj == undefined) {
                return true;
            }
            else {
                return false;
            }
        },
        //判断是否为数字
        isNumber: function (obj) {
            $("#" + obj).bind("contextmenu", function () {
                return false;
            });
            $("#" + obj).css('ime-mode', 'disabled');
            $("#" + obj).keypress(function (e) {
                if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                    return false;
                }
            });
        },
        //判断是否是金钱
        isMoney: function (obj) {
            $("#" + obj).bind("contextmenu", function () {
                return false;
            });
            $("#" + obj).css('ime-mode', 'disabled');
            $("#" + obj).bind("keydown", function (e) {
                var key = window.event ? e.keyCode : e.which;
                if (isFullStop(key)) {
                    return $(this).val().indexOf('.') < 0;
                }
                return (isSpecialKey(key)) || ((isNumber(key) && !e.shiftKey));
            });
            function isNumber(key) {
                return key >= 48 && key <= 57
            }
            function isSpecialKey(key) {
                return key == 8 || key == 46 || (key >= 37 && key <= 40) || key == 35 || key == 36 || key == 9 || key == 13
            }
            function isFullStop(key) {
                return key == 190 || key == 110;
            }
        },
        //判断图片是否存在
        isHasImg: function (pathImg) {
            var ImgObj = new Image();
            ImgObj.src = pathImg;
            if (ImgObj.fileSize > 0 || (ImgObj.width > 0 && ImgObj.height > 0)) {
                return true;
            } else {
                return false;
            }
        },
        //日期格式化yyyy-
        formatDate: function (v, format) {
            if (!v) return "";
            var d = v;
            if (typeof v === 'string') {
                if (v.indexOf("/Date(") > -1)
                    d = new Date(parseInt(v.replace("/Date(", "").replace(")/", ""), 10));
                else
                    d = new Date(Date.parse(v.replace(/-/g, "/").replace("T", " ").split(".")[0]));//.split(".")[0] 用来处理出现毫秒的情况，截取掉.xxx，否则会出错
            }
            var o = {
                "M+": d.getMonth() + 1,  //month
                "d+": d.getDate(),       //day
                "h+": d.getHours(),      //hour
                "m+": d.getMinutes(),    //minute
                "s+": d.getSeconds(),    //second
                "q+": Math.floor((d.getMonth() + 3) / 3),  //quarter
                "S": d.getMilliseconds() //millisecond
            };
            if (/(y+)/.test(format)) {
                format = format.replace(RegExp.$1, (d.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(format)) {
                    format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
                }
            }
            return format;
        },
        //转化成十进制
        toDecimal: function (num) {
            if (num == null) {
                num = "0";
            }
            num = num.toString().replace(/\$|\,/g, '');
            if (isNaN(num))
                num = "0";
            var sign = (num == (num = Math.abs(num)));
            num = Math.floor(num * 100 + 0.50000000001);
            var cents = num % 100;
            num = Math.floor(num / 100).toString();
            if (cents < 10)
                cents = "0" + cents;
            for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
                num = num.substring(0, num.length - (4 * i + 3)) + '' +
                        num.substring(num.length - (4 * i + 3));
            return (((sign) ? '' : '-') + num + '.' + cents);
        },
        //文件大小转换
        countFileSize: function (size) {
            if (size < 1024.00)
                return learun.toDecimal(size) + " 字节";
            else if (size >= 1024.00 && size < 1048576)
                return learun.toDecimal(size / 1024.00) + " KB";
            else if (size >= 1048576 && size < 1073741824)
                return learun.toDecimal(size / 1024.00 / 1024.00) + " MB";
            else if (size >= 1073741824)
                return learun.toDecimal(size / 1024.00 / 1024.00 / 1024.00) + " GB";
        },
        //数组复制
        arrayCopy: function (data) {
            return $.map(data, function (obj) {
                return $.extend(true, {}, obj);
            });
        },
        stringArray: function (str, strone) {
            var arrayObj = str.split(',');
            arrayObj.splice(arrayObj.indexOf(strone), 1);
            return String(arrayObj);
        },
        //检验是否选中行
        checkedRow: function (id) {
            var isOK = true;
            if (id == undefined || id == "" || id == 'null' || id == 'undefined') {
                isOK = false;
                learun.dialogMsg({ msg: '您没有选中任何数据项,请选中后再操作！', type: 0 });
            } else if (id.split(",").length > 1) {
                isOK = false;
                learun.dialogMsg({ msg: '很抱歉,一次只能选择一条记录！', type: 0 });
            }
            return isOK;
        },
        //表单操作
        saveForm: function (opt) {
            var opt = $.extend({
                url: "",
                param: [],
                type: "post",
                dataType: "json",
                loading: "正在处理数据...",
                success: null,
                error:null,
                close: true
            }, opt);
            learun.loading({ isShow: true, text: opt.loading });
            if ($('[name=__RequestVerificationToken]').length > 0) {
                opt.param["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
            }
            window.setTimeout(function () {
                $.ajax({
                    url: opt.url,
                    data: opt.param,
                    type: opt.type,
                    dataType: opt.dataType,
                    success: function (data) {
                        if (data.type == "3") {
                            learun.dialogAlert({ msg: data.message, type: -1 });
                        } else {
                            learun.loading({ isShow: false });
                            learun.dialogMsg({ msg: data.message, type: 1 });
                            opt.success(data);
                            if (opt.close == true) {
                                learun.dialogClose();
                            }
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (opt.error) {
                            opt.error(XMLHttpRequest, textStatus, errorThrown);
                        } else {
                            learun.loading({ isShow: false });
                            learun.dialogMsg({ msg: errorThrown, type: -1 });
                        }
                    },
                    beforeSend: function () {
                        learun.loading({ isShow: true, text: opt.loading });
                    },
                    complete: function () {
                        learun.loading({ isShow: false });
                    }
                });
            }, 500);
        },
        setForm: function (opt) {
            var opt = $.extend({
                url: "",
                param: [],
                type: "get",
                dataType: "json",
                success: null,
                async: false,
                cache: false
            }, opt);
            $.ajax({
                url: opt.url,
                data: opt.param,
                type: opt.type,
                dataType: opt.dataType,
                async: opt.async,
                success: function (data) {
                    if (data != null && data.type == "3") {
                        learun.dialogAlert({ msg: data.message, type: -1 });
                    } else {
                        opt.success(data);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    learun.dialogMsg({ msg: errorThrown, type: -1 });
                }, beforeSend: function () {
                    learun.loading({ isShow: true });
                },
                complete: function () {
                    learun.loading({ isShow: false });
                }
            });
        },
        removeForm: function (opt) {
            var opt = $.extend({
                msg: "注：您确定要删除吗？该操作将无法恢复",
                loading: "正在删除数据...",
                url: "",
                param: [],
                type: "post",
                dataType: "json",
                success: null
            }, opt);
            learun.dialogConfirm({
                msg: opt.msg,
                callBack: function (r) {
                    if (r) {
                        learun.loading({ isShow: true, text: opt.loading });
                        window.setTimeout(function () {
                            var postdata = opt.param;
                            if ($('[name=__RequestVerificationToken]').length > 0) {
                                postdata["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
                            }
                            $.ajax({
                                url: opt.url,
                                data: postdata,
                                type: opt.type,
                                dataType: opt.dataType,
                                success: function (data) {
                                    if (data.type == "3") {
                                        learun.dialogAlert({ msg: data.message, type: -1 });
                                    } else {
                                        learun.dialogMsg({ msg: data.message, type: 1 });
                                        opt.success(data);
                                    }
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    learun.loading({ isShow: false });
                                    learun.dialogMsg({ msg: errorThrown, type: -1 });
                                },
                                beforeSend: function () {
                                    learun.loading({ isShow: true, text: opt.loading });
                                },
                                complete: function () {
                                    learun.loading({ isShow: false });
                                }
                            });
                        }, 500);
                    }
                }
            });
        },
        confirmAjax: function (opt) {
            var opt = $.extend({
                msg: "提示信息",
                loading: "正在处理数据...",
                url: "",
                param: [],
                type: "post",
                dataType: "json",
                success: null
            }, opt);
            learun.dialogConfirm({
                msg: opt.msg,
                callBack: function (r) {
                    if (r) {
                        learun.loading({ isShow: true, text: opt.loading });
                        window.setTimeout(function () {
                            var postdata = opt.param;
                            if ($('[name=__RequestVerificationToken]').length > 0) {
                                postdata["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
                            }
                            $.ajax({
                                url: opt.url,
                                data: postdata,
                                type: opt.type,
                                dataType: opt.dataType,
                                success: function (data) {
                                    learun.loading({ isShow: false });
                                    if (data.type == "3") {
                                        learun.dialogAlert({ msg: data.message, type: -1 });
                                    } else {
                                        learun.dialogMsg({ msg: data.message, type: 1 });
                                        opt.success(data);
                                    }
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    learun.loading({ isShow: false });
                                    learun.dialogMsg({ msg: errorThrown, type: -1 });
                                },
                                beforeSend: function () {
                                    learun.loading({ isShow: true, text: opt.loading });
                                },
                                complete: function () {
                                    learun.loading({ isShow: false });
                                }
                            });
                        }, 200);
                    }
                }
            });
        },
        existField: function (controlId, url, param) {
            var $control = $("#" + controlId);
            if (!$control.val()) {
                return false;
            }
            var data = {
                keyValue: learun.request('keyValue')
            };
            data[controlId] = $control.val();
            var options = $.extend(data, param);
            $.ajax({
                url: url,
                data: options,
                type: "get",
                dataType: "text",
                async: false,
                success: function (data) {
                    if (data.toLocaleLowerCase() == 'false') {
                        ValidationMessage($control, '已存在,请重新输入');
                        $control.attr('fieldexist', 'yes');
                    } else {
                        $control.attr('fieldexist', 'no');
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    learun.dialogMsg({ msg: errorThrown, type: -1 });
                }
            });
        },
        getDataForm: function (opt) {
            var opt = $.extend({
                url: "",
                param: [],
                type: "post",
                dataType: "json",
                loading: "正在获取数据...",
                success: null,
                async: false,
                cache: false
            }, opt);
            learun.loading({ isShow: true, text: opt.loading });
            if ($('[name=__RequestVerificationToken]').length > 0) {
                opt.param["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
            }
            $.ajax({
                url: opt.url,
                data: opt.param,
                type: opt.type,
                dataType: opt.dataType,
                async: opt.async,
                success: function (data) {
                    if (data != null && data.type == "3") {
                        learun.dialogAlert({ msg: data.message, type: -1 });
                    } else {
                        opt.success(data);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    learun.dialogMsg({ msg: errorThrown, type: -1 });
                }, beforeSend: function () {
                    learun.loading({ isShow: true });
                },
                complete: function () {
                    learun.loading({ isShow: false });
                }
            });
        },
        //获取系统表单字段数据,如果需要对这些字段做权限管控才添加，不然不添加jsonccq  
        getSystemFormFields: function (Id) {
            var formIframe = learun.getIframe(Id);
            if (!!formIframe.$) {
                formIframe.$('body').find('[data-systemHideField]').hide();
                if (!!formIframe.getSystemFields) {
                    return formIframe.getSystemFields();//{ "field": id, "label": name, 'type': type }
                }
                else {
                    return [];
                }
            }
            else {
                return false;
            }
        },
        loadSystemForm: function (iframeId, url) {
            var _iframe = document.getElementById(iframeId);
            var _iframeLoaded = function () {
                var formIframe = learun.getIframe(iframeId);
                if (!!formIframe.$) {
                    formIframe.$('body').find('[data-systemHideField]').hide();
                }
                learun.loading({ "isShow": false });
            };
            if (_iframe.attachEvent) {
                _iframe.attachEvent("onload", _iframeLoaded);
            } else {
                _iframe.onload = _iframeLoaded;
            }
            $('#' + iframeId).attr('src', getInfoTop().contentPath + url);
        },
        getSystemFormData:function(iframeId)//获取系统表单数据
        {
            var formIframe = learun.getIframe(iframeId);
            if (!!formIframe && !!formIframe.$) {
                if (!!formIframe.getSystemData) {
                    return formIframe.getSystemData();//{ "field": id, "label": name, 'type': type }
                }
                else {
                    return [];
                }
            }
            else {
                return [];
            }
        },
        saveSystemFormData:function(iframeId,callback)
        {
            var formIframe = learun.getIframe(iframeId);
            if (!!formIframe.$) {
                if (!!formIframe.AcceptClick) {
                    formIframe.AcceptClick(callback);//{ "field": id, "label": name, 'type': type }
                }
            }
        },
        setSystemFormFieldsAuthrize: function (iframeId, item) {
            var formIframe = learun.getIframe(iframeId);
            if (!!formIframe.$) {
                if (!!formIframe.setSystemFieldsAuthorize) {
                    formIframe.setSystemFieldsAuthorize(item);//{ "field": id, "label": name, 'type': type }
                }
            }
        },
        //创建一个流程
        createProcess: function (postData, callBack) {
            postData.processId = learun.createGuid();
            postData["moduleId"] = getInfoTop().$.cookie('currentmoduleId');

            learun.getDataForm({
                url: "../../FlowManage/FlowLaunch/CreateProcessInstance",
                param: postData,
                loading: "正在创建流程",
                success: function () {
                    callBack(postData.processId);
                }
            });
        },
        //json数据操作
        jsonWhere: function (data, action) {
            if (action == null) return;
            var reval = new Array();
            $(data).each(function (i, v) {
                if (action(v)) {
                    reval.push(v);
                }
            })
            return reval;
        }
    });
})(window.jQuery, window.learun);
