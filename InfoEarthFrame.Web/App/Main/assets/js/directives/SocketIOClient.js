//2014/4/11 socketIO web 客户端 吴昊

$(function () {
    try {
        //"use strict";

        var pageUrl = location.href;
        var fetchWarningMsgCountUrl = "/AjaxHandler.ashx?class=AjaxYJPolicyDetailinfo&method=FindMonitorWarningMsgsCount";
        var dangerCaseMsgCountUrl = "/AjaxHandler.ashx?class=AjaxDangerList&method=FindDangerCaseMsgsCount";
        fetchWarningMsgCountUrl = getFullUrl(fetchWarningMsgCountUrl);
        dangerCaseMsgCountUrl = getFullUrl(dangerCaseMsgCountUrl);

        //从cookie中取出登录账户信息
        function getAccount() {
            var strToken = getCookie("Token");
            if (strToken == "" || strToken == "undefined") {
                return "admin";
            }
            else {
                var params = strToken.split(",");
                if (params.length > 0) {
                    return params[1];
                }
                else {
                    return "admin";
                }
            }
        }

        var username = getAccount();
        var sign = false;

        function init() {
            //通过当前页面唯一标志来确定初始化模块类目
            if (pageUrl != "") {
                if (pageUrl.indexOf("WarningAnalysis") >= 0 || pageUrl.indexOf("YJZH") >= 0) {
                    initwarningMsgs(username);
                    initDangerCaseMsgs(username);
                }
                else {
                    $("#dvMsgCount").css("display", "block");
                    initUnreadMessages(username);
                }
            }
        }

        function initwarningMsgs(username) {
            $.ajax({
                url: fetchWarningMsgCountUrl,
                data: { receiver: username, readstatus: "0" },
                async: true,
                success: function (jsonData) {
                    $('#warningmsg').html(jsonData);
                    //listenToSocketIO('MonitorWarningAnalysis');
                },
                error: function (e) {
                    $('#warningmsg').html('#');
                },
                cache: false
            });
        }

        function initDangerCaseMsgs(username) {
            $.ajax({
                url: dangerCaseMsgCountUrl,
                data: { receiver: username, readstatus: "0" },
                async: true,
                success: function (jsonData) {
                    $('#emergencymsg').html(jsonData);
                    //listenToSocketIO('EmergencyCommand');
                },
                error: function (e) {
                    $('#emergencymsg').html('#');
                },
                cache: false
            });
        }

        //加载首页未读信息
        function initUnreadMessages(username) {
            var url = "/AjaxHandler.ashx?class=AjaxDefault&method=GetUnreadMessage";
            //var receiver = $("#loginName").html();

            $.ajax({
                url: getFullUrl(url),
                data: { receiver: username, readStatus: "0" },
                success: function (data) {
                    if (data == "") {
                        alert("未读信息加载出错，请联系管理员！");
                        return;
                    }
                    else {
                        setUnreadMessages(data);
                        //listenToSocketIO("QxyjResultSign");
                        //listenToSocketIO("QxyjResultPublish");
                    }
                },
                error: function (e) {
                    console.log("未读信息加载出错，请联系管理员！");
                    //alert("未读信息加载出错，请联系管理员！");
                },
                cache: false
            });
        }

        function setUnreadMessages(data) {
            var arrMessageCount = data.split('|');

            var totalMessageCount = 0;
            $.each(arrMessageCount, function (index, val) {
                totalMessageCount += parseInt(val);
            })
            var html = '消息(<font color="red">' + totalMessageCount + '</font>)<i class="more2"></i>';
            $("#spnTotalMsgs").html(html);
            html = '未读待签批消息(<font color="red">' + arrMessageCount[0] + '</font>)条';
            $("#spnUnSignMsg").html(html);
            html = '未读待发布消息(<font color="red">' + arrMessageCount[1] + '</font>)条';
            $("#spnUnPublishMsg").html(html);
            html = '未读预警消息(<font color="red">' + arrMessageCount[2] + '</font>)条';
            $("#spnUnReadYJMsg").html(html);
            html = '未读险情消息(<font color="red">' + arrMessageCount[3] + '</font>)条';
            $("#spnUnReadXQMsg").html(html);
        }

        //function listenToSocketIO(moduleID) {
        //    //创建socketIO连接
        //    try {
        //        var iosocket = io.connect(SocketIOServer + '/' + moduleID);//连接SocketIO服务器，命名空间为ModuleID
        //        iosocket.on('connect', function () {
        //            iosocket.emit('subscribe', username);//监听一个频道，频道名称为username
        //            iosocket.on('send', function (msg) {
        //                switch (moduleID) {
        //                    case 'MonitorWarningAnalysis':
        //                        $('#warningmsg').html(msg.count);
        //                        break;
        //                    case 'EmergencyCommand':
        //                        $('#emergencymsg').html(msg.count);
        //                        break;
        //                    case 'QxyjResultSign':
        //                        $("#spnUnSignMsg").html('未读待签批消息(<font color="red">' + msg.count + '</font>)条');
        //                        break;
        //                    case 'QxyjResultPublish':
        //                        $("#spnUnPublishMsg").html('未读待发布消息(<font color="red">' + msg.count + '</font>)条');
        //                        break;
        //                    default:
        //                        alert("发现未知的module信息");
        //                        break;
        //                }
        //            });

        //            iosocket.on('disconnect', function () {
        //                iosocket = io.connect(SocketIOServer + '/' + moduleID);
        //                //断开重连
        //            });

        //            iosocket.on('error', function (data) {
        //                alert('服务端抛出异常，异常信息:' + data.toString());
        //                //服务端抛出异常
        //            });
        //        });
        //    } catch (e) {
        //        alert("SocketIO服务器连接失败，请刷新后重试！");
        //        //$('#emergencymsg').html('*');
        //        //$('#warningmsg').html('*');
        //        return;
        //    }
        //}

        init();

    }
    catch (E) {
        //alert(123);
    }

});
