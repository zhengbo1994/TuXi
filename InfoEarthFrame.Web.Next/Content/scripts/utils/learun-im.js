/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * 陈小二
 */
(function () {
    "use strict";
    //图标闪乐
    var IMinterval;
    var IMIconflash = function (state) {
        var $obj = $("#icon_message");
        if (state == 1) {
            if ($('body').find('embed').length == 0) {
                $('body').append('<embed src="' + getInfoTop().contentPath + '/Content/images/video/5103.wav">');
            }
            else {
                $('embed').remove();
                $('body').append('<embed src="' + getInfoTop().contentPath + '/Content/images/video/5103.wav">');
            }
            IMinterval = setInterval(function () {
                if (!$obj.hasClass('_ok')) {
                    $obj.addClass('_ok');
                    $obj.hide();
                } else {
                    $obj.removeClass('_ok');
                    $obj.show();
                }
            }, 400);
        } else {
            $obj.removeClass('_ok');
            $obj.show();
            clearInterval(IMinterval);
        }
    };
    //服务端断开，移除消息窗体
    var IMRemove = function () {
        var $obj = $('#btn_message');
        var $messagewrap = $('.message-wrap');
        $obj.remove();
        $messagewrap.remove();
        getInfoTop().learun.dialogTop({ msg: "消息服务器连接不上", type: "error" });
    };

    var  iMSignalR = function (options) {
        var options = $.extend({
            url: "",//服务地址
            userId: "",//用户登录Id
            updateUserList: function (userAllList, onLineUserList) { },//刷新用户列表
            updateLastUser: function (userLasrList) { },//刷新最新联系人
            updateUserStatus: function (userId, isOnLine) { },//刷用户在线状态0离线。1在线
            revMessage: function (formUser, message, dateTime) { },//接收消息
            revGroupMessage: function (userId, toGroup, message, dateTime) { },//接收群组消息
            afterLinkSuccess: function () { },//连接服务成功后
            disconnected: function () { }//断开连接后
        }, options);

        var imMethod = {
            //组的操作
            IMCreateGroup: function (groupName, userIdList) { },//创建一个组
            IMUpdateGroupName: function (groupId, groupName) { },//更改组的名字
            IMAddGroupUserId: function (groupId, userId) { },//增加一个用户到组里
            IMRemoveGroupUserId: function (userGroupId) { },//从一个组里面移除
            //消息的操作
            IMSendToOne: function (toUser, message) { },//发送消息
            IMSendGroup: function (toUser, message) { },//发送消息群组
            IMUpdateMessageStatus: function (sendId) { },//更新消息状态
            IMGetMsgList: function (page, rownum, sendId, callback) { },//获取消息列表
            IMGetUnReadMsgNumList: function () { },//获取未读消息的条数
            //联系人操作
            IMUpdateLastUserByClient: function () { },//主动发起更新最近联系人列表
            //获取所有用户列表
            userAllList: {},
        };
        
        var imSignalR = {
            chat:false,
            init: function () {
                imSignalR.loadJs(function () {
                    //Set the hubs URL for the connection
                    $.connection.hub.url = options.url;
                    $.connection.hub.qs = { "userId": options.userId };
                    // Declare a proxy to reference the hub.
                    imSignalR.chat = $.connection.ChatsHub;
                    imSignalR.registerClient();
                    // 连接成功后注册服务器方法
                    $.connection.hub.start().done(function () {
                        imSignalR.registerServer();
                        options.afterLinkSuccess(imMethod);
                    });
                    //断开连接后
                    $.connection.hub.disconnected(function () {
                        options.disconnected();
                    });
                });
            },
            loadJs: function (callback) {
                //console.log(options.url);
                $.ajax({
                    url: options.url + "/hubs",
                    type: "get",
                    dataType: "text",
                    success: function (data) {
                        eval(data);
                        callback();
                    }
                });
            },
            registerClient: function () {
                if (imSignalR.chat)
                {
                    //接收消息
                    imSignalR.chat.client.RevMessage = function (formUser, message, dateTime) {
                        options.revMessage(formUser, message, dateTime);
                    }
                    //接收群消息
                    imSignalR.chat.client.RevGroupMessage = function (userId, toGroup, message, dateTime) {
                        options.revGroupMessage(userId, toGroup, message, dateTime);
                    }
                }
                
            },
            registerServer: function () {
                imMethod.IMCreateGroup = function (groupName, userIdList) {
                    return imSignalR.chat.server.createGroup(groupName, userIdList);
                };
                imMethod.IMUpdateGroupName = function (groupId, groupName) {
                    return imSignalR.chat.server.updateGroupName(groupId, groupName);
                };
                imMethod.IMAddGroupUserId = function (groupId, userId) {
                    return imSignalR.chat.server.addGroupUserId(groupId, userId);
                };
                imMethod.IMRemoveGroupUserId = function (userGroupId) {
                    return imSignalR.chat.server.removeGroupUserId(userGroupId);
                };

                imMethod.IMSendToOne = function (toUser, message) {
                    imSignalR.chat.server.imSendToOne(toUser, message);
                };
                imMethod.IMSendGroup = function (toUser, message) {
                    imSignalR.chat.server.imSendToGroup(toUser, message);
                };
                imMethod.IMUpdateMessageStatus = function (sendId) {
                    return imSignalR.chat.server.updateMessageStatus(sendId);
                };
                //获取与某用户的消息列表
                imMethod.IMGetMsgList = function (page, rownum, sendId, callback) {
                    var pagination = { rows: rownum, page: page, sidx: 'F_CreateDate', sord: 'desc' }
                    imSignalR.chat.server.getMsgList(pagination, sendId).done(function (resdata) {
                        var data = [];
                        for (var i = resdata.length-1; i >= 0; i--) {
                            resdata[i].F_CreateDate = formatDate(resdata[i].F_CreateDate, 'yyyy-MM-dd hh:mm');
                            data.push(resdata[i]);
                        }
                        callback(data);
                    });
                };
                //获取未读消息的条数
                imMethod.IMGetUnReadMsgNumList = function (callback) {
                    imSignalR.chat.server.getMsgNumList("0").done(function (resdata) {
                        callback(resdata);
                    });
                };

                imMethod.IMUpdateLastUserByClient = function () {
                    imSignalR.chat.server.imSendLastUser();
                }
            }
        }
        imSignalR.init();

        if (learun === undefined)
        {
            IMRemove();
        }
    }
    //即使通信
    $.imInit = function (options) {
        var $message_wrap = $(".message-wrap");
        var $message_window = $(".message-window");
        var imMethod;
        var defaults = {
            userId: "77653de4-d8e7-4903-9f2a-ee47ba3111da",//当前登录Id
            userName: "",                                   //当前用户名,
            toUserId: "",                                   //发送人Id
            toUserName: "",                                 //发送人
            windowId: "",   //当前窗口Id
            revMessage: GetMessage,
            afterLinkSuccess: function (method) { //连接成功后
                imMethod = method;
                //显示消息未读条数
                imMethod.IMGetUnReadMsgNumList(function (num) {
                    $message_wrap.find('.message-count').html(num);
                    $("#num1,#num").html(num);
                    if (parseInt(num) > 0) {
                        IMIconflash(1);
                    }
                });
                $("#testSendMessage").click(function () {
                    imMethod.IMSendToOne("9b058ead-8cd7-4559-bde7-32070f306dfb", "消息通知测试!");
                });
            },
            disconnected: function () {
                IMRemove();
            }
        };
        var options = $.extend(defaults, options);

        try {
            iMSignalR(options);
        }
        catch (e) {
            IMRemove();
            return false;
        }


        //打开即时聊天
        $("#btn_message").click(function () {
            $(this).hide()
            $message_wrap.show();
        })
        //关闭即时聊天
        $message_wrap.find('.message-close').click(function () {
            IMIconflash(0);
            $message_wrap.hide();
            $message_window.hide();
            $("#btn_message").show();
        })
        //导航切换（联系人、讨论组、最近回话）
        $message_wrap.find('.message-nav li').click(function () {
            var tab_Id = $(this).attr('id');
            switch (tab_Id) {
                case "nav-contact-tab":
                    $message_wrap.find("#message-contact-list").show();
                    $message_wrap.find("#message-group-list").hide();
                    $message_wrap.find("#message-last-list").hide();
                    $(this).find('span').hide();
                    break;
                case "nav-group-tab":
                    $message_wrap.find("#message-group-list").show();
                    $message_wrap.find("#message-contact-list").hide();
                    $message_wrap.find("#message-last-list").hide();
                    break;
                case "nav-last-tab":
                    $message_wrap.find("#message-last-list").show();
                    $message_wrap.find("#message-contact-list").hide();
                    $message_wrap.find("#message-group-list").hide();
                    $(this).find('span').show();
                    break;
                default:
                    break;

            }
            $(this).parents('ul').find('li').removeClass("active");
            $(this).addClass("active");
        });
        //搜索（同事、讨论组、最近回话）
        $message_wrap.find('.message-body-search').find('.search-text').keyup(function () {
            var $chatlist = $("#message-contact-list").find('.message-chatlist');
            var value = $(this).val();
            if (value != "") {
                $chatlist.show();
                $chatlist.prev('a').hide();
                window.setTimeout(function () {
                    $chatlist.find('li')
                     .hide()
                     .filter(":contains('" + (value) + "')")
                     .show();
                }, 200);
                $chatlist.find('li').hover(function () {
                    $(this).find('span').show();
                }, function () {
                    $(this).find('span').hide();
                })
            } else {
                $chatlist.hide();
                $chatlist.find('li').show();
                $chatlist.prev('a').show();
                if ($chatlist.prev('a').hasClass('active')) {
                    $chatlist.prev('a.active').next('ul').show();
                }
                $chatlist.find('li').find('span').hide();
            }
        }).keyup();


        //刷新最近联系人列表
        function UpDateLastUser(userLasrList) {
            var $ul = $(".message-group #message-last-list");
            var html = "";
            //显示消息未读条数
            imMethod.IMGetUnReadMsgNumList(function (num) {
                $message_wrap.find('.message-count').html(num);
                $("#num1,#num").html(num);
            });
            $ul.html(html);
            $.each(userLasrList, function (i, item) {

                var model = imMethod.userAllList[item.F_OtherId];
                var src = "off-line.png"
                if (model.F_UserOnLine == 1) {
                    src = "on-line.png"
                }
                html += '<li>';
                html += '<div class="message-oneface"><img src="' + getInfoTop().contentPath + '/Content/images/' + src + '" /></div>';
                html += '<div data-value="' + model.F_UserId + '" class="message-onename">' + model.F_RealName + ' <span>[' + model.F_DepartmentId + ']</span></div>';
                if (item.F_UnReadNum > 0) {
                    html += '<span class="message-count">' + item.F_UnReadNum + '</span>';
                }
                html += '</li>';
            });
            $ul.append(html);
            $ul.find('li').hover(function () {
                $(this).find('span').show();
            }, function () {
                $(this).find('span').hide();
            })
            //打开聊天窗口
            OpenChatWindow();
            //关闭聊天窗口
            CloseChatWindow();
        }
        //打开聊天窗口
        function OpenChatWindow() {
            $message_wrap.find('.message-chatlist li').click(function () {
                $message_window.show();
                var id = $(this).find('.message-onename').attr('data-value');
                var name = $(this).find('.message-onename').text();

                defaults.toUserId = id;
                defaults.toUserName = name;
                defaults.windowId = id;
                $message_window.find('.message-window-header .text').html('与 ' + name + ' 聊天中')
                $message_window.find('.message-window-chat').scrollTop($message_window.find('.message-window-chat')[0].scrollHeight);
                $message_window.find('.message-window-chat').find('.message-window-content').html('');

                //IMIconflash(0);
                imMethod.IMGetMsgList(1, 5, id, function (data) {
                    imMethod.IMUpdateMessageStatus(id);
                    $.each(data, function (i) {
                        GetMessage(data[i].F_SendId, data[i].F_Content, data[i].F_CreateDate, true);
                    });
                    //显示消息未读条数
                    imMethod.IMGetUnReadMsgNumList(function (num) {
                        $message_wrap.find('.message-count').html(num);
                        $("#num1,#num").html(num);
                    });
                    $(this).find('.message-count').html("");
                });
            });
        }
        //关闭聊天窗口
        function CloseChatWindow() {
            $message_window.find('.message-window-header .close').click(function () {
                $message_window.hide();
                defaults.windowId = "";
            })
        }
        SendMessage();
        //发消息
        function SendMessage() {
            var $textarea = $message_window.find('.message-window-send').find('textarea');
            $textarea.bind('keypress', function (event) {
                if (event.keyCode == "13") {
                    var sendText = $(this).val();
                    if (sendText) {
                        imMethod.IMSendToOne(options.toUserId, sendText);
                        $message_window.find('.message-window-send').html('<textarea autofocus placeholder="按回车发送消息,shift+回车换行"></textarea>');
                        SendMessage();
                    }
                }
            });
        }
        //收消息
        function GetMessage(fromUer, content, dateTime, flag) {
            if (fromUer != options.userId) {
                if (options.windowId == fromUer) {
                    var html = '<div class="left"><div class="author-name">';
                    html += '<img src="/Content/images/on-line.png" />';
                    html += '<small class="chat-text">' + options.toUserName + '</small>';
                    html += '<small class="chat-date">' + dateTime + '</small>';
                    html += '</div><div class="chat-message"><em></em>' + content + '</div></div>';
                    $message_window.find('.message-window-content').append(html)
                    $message_window.find('.message-window-chat').scrollTop($message_window.find('.message-window-chat')[0].scrollHeight);
                }
                if (flag == undefined) {
                    //统计未读消息总数量
                    if (options.windowId != fromUer) {
                        var num = parseInt($message_wrap.find('.message-count').html());
                        num += 1;
                        if (num > 99) {
                            num = 99;
                        }
                        $message_wrap.find('.message-count').html(num);
                        $("#num1,#num").html(num);
                        IMIconflash(1);
                    }
                    else {//如果窗口是打开的就判定为已读
                        imMethod.IMUpdateMessageStatus(fromUer);
                    }
                }
            } else {
                var html = '<div class="right"><div class="author-name">';
                html += '<small class="chat-date">2015-11-25 11:24</small>';
                html += '<small class="chat-text">' + options.userName + '</small>';
                html += '<img src="/Content/images/on-line.png" />';
                html += '</div>';
                html += '<div class="chat-message"><em></em>' + content + '</div></div>';
                $message_window.find('.message-window-content').append(html)
                $message_window.find('.message-window-chat').scrollTop($message_window.find('.message-window-chat')[0].scrollHeight);
            }
        }
    };
})(window.jQuery);

















