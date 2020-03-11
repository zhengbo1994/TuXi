/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * LR0101
 */
(function ($) {
    "use strict";
    //回车键
    document.onkeydown = function (e) {
        if (!e) e = window.event;
        if ((e.keyCode || e.which) == 13) {
            var btlogin = document.getElementById("btnlogin");
            btnlogin.click();
        }
    }
    var index = {
        initialPage: function () {
            var wHeight = $(window).height();
            var wWidth = $(window).width();
            var topHeight = (wHeight - 524) / 2;
            var leftWidth = (wWidth - 1024) / 2;
            $(".lr-login-top").css("margin-top", (topHeight - 56) + "px");
            $(".menu").css("top", (topHeight + 50) + "px");
            $(".lr-bg-left").width(leftWidth);
            $(".lr-bg-right").width(leftWidth);
        },
        dialogAlert: function (msg, type) {
            if (type == -1) {
                type = 2;
            }
            top.layer.alert(msg, {
                icon: type,
                title: "提示"
            });
        },
        formMessage: function (msg, type) {
            $('.login_tips').remove();
            $('.login_tips-succeed').remove();
            var _class = "login_tips";
            if (type == 1) {
                _class = "login_tips-succeed";
            }
            $('.lr-input-form').prepend('<div class="' + _class + '"><i class="fa fa-question-circle"></i>' + msg + '</div>');
        },
        formMessageRemove: function () {
            $('.login_tips').remove();
            $('.login_tips-succeed').remove();
        },
        loginForm: {//登录验证
            init: function () {
                //错误提示
                if (top.$.cookie('learun_login_error') != null) {
                    switch (top.$.cookie('learun_login_error')) {
                        case "Overdue":
                            index.formMessage('登录已超时,请重新登录');
                            break;
                        case "OnLine":
                            index.formMessage('您的帐号已在其它地方登录,请重新登录');
                            break;
                        case "noCache":
                            index.formMessage('缓存过期,请重新登录');
                            break;
                        case "-1":
                            index.formMessage('未知错误,请重新登录');
                            break;
                        default:
                            break;
                    }
                    top.$.cookie('learun_login_error', '', { path: "/", expires: -1 });
                }
                //是否自动登录
                if (top.$.cookie('learn_autologin') == 1) {
                    $("#autologin").attr("checked", 'true');
                    $("#username").val(top.$.cookie('learn_username'));
                    $("#password").val(top.$.cookie('learn_password'));
                    index.loginForm.checkLogin(1);
                }
                //设置下次自动登录
                $("#autologin").click(function () {
                    if (!$(this).attr('checked')) {
                        $(this).attr("checked", 'true');
                        top.$.cookie('learn_autologin', 1, { path: "/", expires: 7 });
                    } else {
                        $(this).removeAttr("checked");
                        top.$.cookie('learn_autologin', '', { path: "/", expires: -1 });
                        top.$.cookie('learn_username', '', { path: "/", expires: -1 });
                        top.$.cookie('learn_password', '', { path: "/", expires: -1 });
                    }
                });
                //主题风格
                var learn_UItheme = top.$.cookie('learn_UItheme');
                if (learn_UItheme) {
                    $("#UItheme").val(learn_UItheme);
                }
                //登录按钮事件
                $("#btnlogin").click(function () {
                    var $username = $("#username");
                    var $password = $("#password");
                    var $verifycode = $("#verifycode");
                    if ($username.val() == "") {
                        $username.focus();
                        index.formMessage('请输入账户或手机号或邮箱。');
                        return false;
                    } else if ($password.val() == "") {
                        $password.focus();
                        index.formMessage('请输入密码。');
                        return false;
                    } else if ($verifycode.val() == "") {
                        $verifycode.focus();
                        index.formMessage('请输入验证码。');
                        return false;
                    } else {
                        index.loginForm.checkLogin(0);
                    }
                });
                //点击切换验证码
                $("#login_verifycode").click(function () {
                    $("#verifycode").val('');
                    $("#login_verifycode").attr("src", contentPath + "/Login/VerifyCode?time=" + Math.random());
                });
                //切换注册表单
                $("#a_register").click(function () {
                    $("#lr-login-form").hide();
                    $("#lr-register-form").show();
                    $("#register_verifycode").attr("src", contentPath + "/Login/VerifyCode?time=" + Math.random());
                });
                //切换登录表单
                $("#a_login").click(function () {
                    $("#lr-login-form").show();
                    $("#lr-register-form").hide();
                    $("#login_verifycode").attr("src", contentPath + "/Login/VerifyCode?time=" + Math.random());
                });
            },
            checkLogin: function (autologin) {
                $("#btnlogin").addClass('active').attr('disabled', 'disabled');
                $("#btnlogin").find('span').hide();

                var username = $.trim($("#username").val());
                var password = $.trim($("#password").val());

                var verifycode = $.trim($("#verifycode").val());
                if (autologin == 1) {
                    if (top.$.cookie('learn_password') == "" || top.$.cookie('learn_password') == null) {
                        password = $.md5(password);
                    }
                }
                else {
                    password = $.md5(password);
                    top.$.cookie('learn_username', "", { path: "/", expires: 7 });
                    top.$.cookie('learn_password', "", { path: "/", expires: 7 });
                }
                $.ajax({
                    url: contentPath + "/Login/CheckLogin",
                    data: { username: $.trim(username), password: $.trim(password), verifycode: verifycode, autologin: autologin },
                    type: "post",
                    dataType: "json",
                    success: function (data) {
                        if (data.type == 1) {
                            if (top.$.cookie('learn_autologin') == 1) {
                                top.$.cookie('learn_username', $.trim(username), { path: "/", expires: 7 });
                                top.$.cookie('learn_password', $.trim(password), { path: "/", expires: 7 });
                            }
                            top.$.cookie('learn_UItheme', $("#UItheme option:selected").val(), { path: "/", expires: 30 });
                            window.location.href = contentPath + '/Home/Index';
                        } else {
                            if (data.message.length >= 30) {
                                index.dialogAlert(data.message, 0)
                            } else {
                                index.formMessage(data.message);
                            }
                            $("#btnlogin").removeClass('active').removeAttr('disabled');
                            $("#btnlogin").find('span').show();
                            $("#login_verifycode").trigger("click");
                        }
                    }
                });
            }
        },
        registerForm: {//注册账户
            init: function () {
                //手机号离开事件
                $("#txt_register_account").blur(function () {
                    if ($(this).val() != "" && !isMobile($(this).val())) {
                        $(this).focus();
                        index.formMessage('手机格式不正确,请输入正确格式的手机号码。');
                    }
                    else {
                        index.formMessageRemove();
                    }
                    function isMobile(obj) {
                        var reg = /^(\+\d{2,3}\-)?\d{11}$/;
                        if (!reg.test(obj)) {
                            return false;
                        } else {
                            return true;
                        }
                    }
                });
                //密码输入事件
                $("#txt_register_password").keyup(function () {
                    $(".passlevel").find('em').removeClass('bar');
                    var length = $(this).val().length;
                    if (length <= 8) {
                        $(".passlevel").find('em:eq(0)').addClass('bar');
                    } else if (length > 8 && length <= 12) {
                        $(".passlevel").find('em:eq(0)').addClass('bar');
                        $(".passlevel").find('em:eq(1)').addClass('bar');
                    } else if (length > 12) {
                        $(".passlevel").find('em').addClass('bar');
                    }
                })
                //注册按钮事件
                $("#btnregister").click(function () {
                    var $account = $("#txt_register_account");
                    var $code = $("#txt_register_code");
                    var $name = $("#txt_register_name");
                    var $password = $("#txt_register_password");
                    var $verifycode = $("#txt_register_verifycode");
                    if ($account.val() == "") {
                        $account.focus();
                        index.formMessage('请输入手机号。');
                        return false;
                    } else if ($code.val() == "") {
                        $code.focus();
                        index.formMessage('请输入短信验证码。');
                        return false;
                    } else if ($name.val() == "") {
                        $name.focus();
                        index.formMessage('请输入姓名。');
                        return false;
                    } else if ($password.val() == "") {
                        $password.focus();
                        index.formMessage('请输入密码。');
                        return false;
                    } else if ($verifycode.val() == "") {
                        $verifycode.focus();
                        index.formMessage('请输入图片验证码。');
                        return false;
                    } else {
                        $("#btnregister").addClass('active').attr('disabled', 'disabled');
                        $("#btnregister").find('span').hide();
                        $.ajax({
                            url: contentPath + "/Login/Register",
                            data: { mobileCode: $account.val(), securityCode: $code.val(), fullName: $name.val(), password: $.md5($password.val()), verifycode: $verifycode.val() },
                            type: "post",
                            dataType: "json",
                            success: function (data) {
                                if (data.type == 1) {
                                    alert('注册成功');
                                    window.location.href = contentPath + '/Login/Index';
                                } else {
                                    index.formMessage(data.message);
                                    $("#btnregister").removeClass('active').removeAttr('disabled');
                                    $("#btnregister").find('span').show();
                                    $("#register_verifycode").trigger("click");
                                }
                            }
                        });
                    }
                })
                //获取验证码
                $("#register_getcode").click(function () {
                    var $this = $(this);
                    $this.attr('disabled', 'disabled');
                    $.ajax({
                        url: contentPath + "/Login/GetSecurityCode",
                        data: { mobileCode: $("#txt_register_account").val() },
                        type: "get",
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            if (data.type == 1) {
                                index.formMessage('已向您的手机' + $("#txt_register_account").val() + '发送了一条验证短信。', 1);
                            } else {
                                $this.removeAttr('disabled');
                                alert(data.message);
                            }
                        }
                    });
                });
            }
        }
    };
    $(function () {
        index.initialPage();
        $(window).resize(function (e) {
            window.setTimeout(index.initialPage, 200);
            e.stopPropagation();
        });
        index.loginForm.init();
        index.registerForm.init();
        window.setTimeout(function () {
            $('#ajax-loader').fadeOut();
        }, 100);
    });
})(window.jQuery)
