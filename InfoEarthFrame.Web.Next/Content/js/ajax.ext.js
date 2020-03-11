(function ($) {


    var AjaxRequest = {
        get: function (option) {
            option.type = "get";
            (option.data||{}).rdm = Math.random();

            AjaxRequest._invoke(option);
        },
        post: function (option) {
            option.type = "post";
            AjaxRequest._invoke(option);
        },
        _invoke: function (option) {

            var defaultOption = {
                type: "get",
                async: true,
                beforeSend: function (request) {
                    if (typeof layer != "undefined") {
                        layer.load();
                    }
                    request.setRequestHeader("Authorization", "Bearer " + $.cookie(Config["AccessTokenKey"]));
                    request.setRequestHeader("Lang", $.cookie(Config["CurrentLangKey"]) || 'zh-cn');
                },
                complete: function () {
                    if (typeof layer != "undefined") {
                        layer.closeAll("loading");
                    }
                },
                success: function (resp) {

                },
                error: function (xhr, status, type) {
                    if (xhr.status == 401) {
                        warnning('api没有权限');
                    }
                    else if (xhr.status == 500) {
                        //换一下成error
                        error('内部服务器错误');
                    }

                }
            }

            option = $.extend(defaultOption, option);

            $.ajax(option);
        }
    }

    $.get = AjaxRequest.get;
    $.post = AjaxRequest.post;
})(jQuery);
    
    
 