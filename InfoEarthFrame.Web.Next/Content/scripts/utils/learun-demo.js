/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * LR0101
 */
(function () {
    "use strict";

    var components = [
        {//选择员工控件
            "id": "lea-user-select",
            "text": "人员选择按钮",
            "value": "1",
            "img": "fa fa-user",
            "parentnodes": "0",
            "showcheck": false,
            "isexpand": true,
            "complete": true,
            "hasChildren": false,
            "html": ' <div id="lea-user-select"><div id="UserSelect" type="select" class="ui-select ui-select-focus"><div class="ui-select-text" style="color:#999;">选择人员</div></div></div>',
            "initFn": function () {
                $('#UserSelect').userSelectBox();
            }
        },
        {//上传文件插件
            "id": "lea-user-upfile",
            "text": "上传文件",
            "value": "1",
            "img": "fa fa-upload",
            "parentnodes": "0",
            "showcheck": false,
            "isexpand": true,
            "complete": true,
            "hasChildren": false,
            "html": '<div style="min-height:30px;" id="uploadifyDemo" ></div></div><div style="margin-top:20px;" id="uploader" ></div>',
            "initFn": function () {
                //uploadify版
                $("#uploadifyDemo").uploadifyEx({
                    url: "/Utility/UploadifyFile?DataItemEncode=SaveFilePath&DataItemName=DemoFilePath",
                    btnName: "【Uploadify版】添加附件",
                    height: 30,
                    width: 180,
                    type: "uploadify"
                });
                $("#uploader").uploadifyEx({
                    url: "/Utility/UploadifyFile?DataItemEncode=SaveFilePath&DataItemName=DemoFilePath",
                    btnName: "【WebUploader版】添加附件",
                    height: 30,
                    width: 200,
                    type: "webUploader"
                });
            }
        }
    ];
    
    var demoUI = {
        initial: function () {
            demoUI.initialPage();
            demoUI.bindControl();
        },
        initialPage: function () {
            //layout布局
            $('#layout').layout({
                applyDemoStyles: true,
                onresize: function () {
                    $(window).resize();
                }
            });
            $("#gridPanel").height($(window).height() - 40);
            //resize重设(表格、树形)宽高
            $(window).resize(function (e) {
                window.setTimeout(function () {
                    $("#gridPanel").height($(window).height() - 40);
                    $("#itemTree").setTreeHeight($(window).height() - 52);
                }, 200);
                e.stopPropagation();
            });
        },
        bindControl: function () {
            /*初始化左侧树形列表*/
            $("#itemTree").treeview({
                height: $(window).height() - 52,
                data: components,
                onnodeclick: function (item) {
                    $('#gridPanel').html(item.html);
                    if (item.initFn != null) {
                        item.initFn();
                    }
                }
            });
        }
    };

    $(function () {
        demoUI.initial();
    });

})(window.jQuery);