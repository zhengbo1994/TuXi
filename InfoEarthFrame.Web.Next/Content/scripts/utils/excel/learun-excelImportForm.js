/*!
 * 版 本 LearunADMS V6.1.2.0 (http://www.learun.cn)
 * Copyright 2011-2016 Learun, Inc.
 * excel导入页面
 * 陈小二
 */
(function ($) {
    "use strict";

    var templates;
    var dtData = {
        success: [],
        fail:[]
    };
    var jqData = [];
    var excelform = {
        init: function () {
            var moduleId = getInfoTop().$.cookie('currentmoduleId');
            var btnId = learun.request('btnId');
            templates = getInfoTop().learun.data.get(["excelImportTemplate", moduleId, "entitys", btnId, "data"]);
            console.log(templates);
            if (!!templates) {
                var $navTabs = $('.nav-tabs');
                var navTabsHtml = "",contentTabsHtml="";
                $.each(templates, function (i, item) {
                    navTabsHtml += '<li class="' + (i == 0 ? 'active' : '') + '"   data-value="' + i + '"><a >' + item.templateInfo.F_Name + '</a></li>';
                });
                excelform.initTemplate(0);
                $navTabs.html(navTabsHtml);
                $navTabs.find('li').on('click', excelform.clickTabs);
            }
            else {//获取不到模板数据
                learun.dialogTop({ msg: "获取不到模板数据", type: "error" });
                return false;
            }
            $("#btn_close").on('click', function () {
                learun.dialogClose();
            });

            $("#girdPager .prev").on('click', function () {
                var index = parseInt($(this).attr('data-value')) -1;
                if (index >= 0)
                {
                    excelform.jgGirdRendering(jqData, 0, 30);
                }
               
            });
            $("#girdPager .next").on('click', function () {
                var index = parseInt($(this).attr('data-value')) + 1;
                if (index >= 0) {
                    excelform.jgGirdRendering(jqData, 0, 30);
                }
            });
        },
        clickTabs: function () {
            var $li = $(this);
            if (!$li.hasClass('active')) {
                $('.nav-tabs .active').removeClass('active');
                $li.addClass('active');
                var id = $li.attr('data-value');
                excelform.initTemplate(id);
            }
        },
        initTemplate: function (i) {
            var data = templates[i];
            var colModel = [];
            $.each(data.filedsInfo, function (i, item) {
                if (item.F_RelationType != 1 && item.F_RelationType != 4 && item.F_RelationType != 5 && item.F_RelationType != 6 && item.F_RelationType != 7)
                {
                    var point = {
                        label: item.F_ColName,
                        name: item.F_ColName,
                        index: item.F_ColName,
                        width: 100,
                        align: 'left',
                        sortable: false
                    }
                    colModel.push(point);
                } 
            });
            excelform.initJqGird(colModel);
            excelform.initButton(data);
        },
        initButton: function (data) {
            $("#lr-upfile").uploadifyEx({
                url: "/Utility/ExecuteImportExcel?templateId=" + data.templateInfo.F_Id,
                btnName: "上传文件",
                type: "uploadify",
                height: 31,
                width: 90,
                oneFile:true,
                fileTypeExts: "xls,xlsx",
                fileSizeLimit:'100MB',
                onUploadSuccess: excelform.loadData
            });
            $('#lr-download').unbind();
            $('#lr-download').on('click', function () {
               
                learun.downFile({
                    url: "/Utility/DownExcelTemplate",
                    data: ("templateId=" + data.templateInfo.F_Id),
                    method: 'post'
                });
            });
        },
        initJqGird: function (colModel) {
            colModel.push({
                label: "状态",
                name: "learunColOk",
                index: "learunColOk",
                width: 60,
                align: 'center',
                sortable: false,
                formatter: function (cellvalue, options, rowObject) {
                    return cellvalue == 1 ? '<span >成功</span>' : '<span style="color:red">失败</span>';
                }
            });
            colModel.push({
                label: "描述",
                name: "learunColError",
                index: "learunColError",
                width: 200,
                align: 'left',
                sortable: false
            });
            $('.gridPanel').html(' <table id="gridTable"></table>');
            var $grid = $("#gridTable");
            $grid.jqGrid({
                unwritten: false,
                datatype: "local",
                height: $(window).height() - 214,
                autowidth: true,
                colModel: colModel,

                pager: false,
                rownumbers: true,
                shrinkToFit: false,
                gridview: true
            });
        },
        jgGirdRendering: function (data, index, pagenum) {
            var len = data.length;
            
            var flag = false;
            var begin = index * pagenum;
            var end = index * pagenum + 30;
            var j = 0;
            if (end > len)
            {
                end = len;
            }
            for (var i = begin; i < len; i++) {
                if (!flag) {
                    $("#gridTable").jqGrid('clearGridData');
                    flag = true;
                }
                $("#gridTable").jqGrid('addRowData', j, data[i]);
                j++;
            }
            if (flag)
            {
                var total = parseInt(len / pagenum) + ((len % pagenum) > 0 ? 1 : 0);
                var $girdPager = $('#girdPager');
                $girdPager.find('.num-total').text(total);
                $girdPager.find('.num-index').text(index + 1);
                $girdPager.find('.prev').attr('data-value', index);
                $girdPager.find('.next').attr('data-value', index);
            }
        },
        loadData: function (data) {
            console.log(data);
            try
            {
                window.parent.$("#gridTable").trigger("reloadGrid");
            }catch(e){}
            var len = data.Rows.length;
            dtData.success = [], dtData.fail = [];
            for (var i = 0; i < len; i++)
            {
                var point = data.Rows[i];
                point.rownum = i;
                if (point.learunColOk == "1") {
                    dtData.success.push(point);
                }
                else {
                    dtData.fail.push(point);
                }
            }

            var $girdPager = $('#girdPager');
            $girdPager.find('.num-all').text(len);
            $girdPager.find('.num-success').text(dtData.success.length);
            $girdPager.find('.num-fail').text(dtData.fail.length);

            jqData = dtData.fail.concat(dtData.success);
            excelform.jgGirdRendering(jqData, 0, 30);
        }
    };

    $(function () {
        excelform.init();
    });
})(window.jQuery, window.learun);