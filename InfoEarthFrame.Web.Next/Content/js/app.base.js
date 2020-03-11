var Base = {
    isAdmin: function () {
        return (Config["CurrentUserName"] || '').toLowerCase() == "admin";
    },
    hasPermession: function (buttonType,pageUrl) {
        var flag = false;
        $("button[data-permession-button],a[data-permession-button],li[data-permession-button]").each(function (index, element) {
            if (($(this).text().indexOf(buttonType) >= 0 || $(this).attr("data-permession-button").indexOf(buttonType) >= 0) && $(this).is(":disabled"))
            {
                flag = true;
                return false;
            }
        });

        return flag;
    },
    initPermession:function(pageUrl)
    {
        if (!Base.isAdmin()) {
            $.get({
                url: Base.getApiUrl("/Account/GetPagePermession?menuUrl=") + pageUrl,
                success: function (resp) {
                    var data = resp.data || [];

                    var $buttons = $("button[data-permession-button],a[data-permession-button],li[data-permession-button],div[data-permession-button]");
                    $buttons.each(function (index2, element2) {
                        var $current = $(this);

                        if (data.length > 0) {
                            $.each(data, function (index, element) {
                                if (element.HasPermession && ($current.text().indexOf(element.Name) >= 0 || $current.attr("data-permession-button").indexOf(element.Name) >= 0)) {
                                    $current.attr("disabled", false).removeClass("layui-btn-disabled")
                                    return false;
                                }
                                else {
                                    if (!($current.text().indexOf('搜索') >= 0 || $current.text().indexOf('刷新') >= 0)) {
                                        if ($current.is("li") || $current.is("a") || $current.is("div")) {
                                            $current.hide();
                                        }
                                        else {
                                            $current.attr("disabled", true).addClass("layui-btn-disabled");
                                        }
                                    }
                                }
                            });
                        }
                        else {
                            if (!($current.text().indexOf('搜索') >= 0 || $current.text().indexOf('刷新') >= 0)) {
                                if ($current.is("li") || $current.is("a") || $current.is("div")) {
                                    $current.hide();
                                }
                                else {
                                    $current.attr("disabled", true).addClass("layui-btn-disabled");
                                }
                            }
                        }
                    });
                }
            });
        }
    },
    getGuid:function() {
        var s = [];
        var hexDigits = "0123456789abcdef";
        for (var i = 0; i < 36; i++) {
            s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
        }
        s[14] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
        s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01
        s[8] = s[13] = s[18] = s[23] = "-";

        var uuid = s.join("");
        return uuid + "-InfoEarth";
    },
    getApiUrl: function (rawUrl) {
        return Config["BaseUrl"] + "/api" + rawUrl;
    },
    getText:function(value)
    {
        if (value == null)
        {
            return "";
        }

        return value;
    },
    viewFile: function (option) {
        $.get({
            url: option.getPDFFileUrl,
            data: {
                filePath: option.filePath
            },
            success: function (resp) {
                top.layui.showDlg({
                    title: option.fileName,
                    content: '/Content/PDFJS/web/viewer.html?path=' + resp.data,
                    area: ['100%', '100%'],
                    btn: []
                });
            }
        });
    },
    getTimeRange: function (id) {
        id = id.replace("#", "");
        var value = $("#" + id).val();
        if (!value) {
            return {
                StartDate: '',
                EndDate: ''
            };
        }

        var valuePart = value.split(LangConfig.CommonText.to);
        return {
            StartDate: valuePart[0],
            EndDate: valuePart[1]
        };
    },
    Converter: {
        dateTime: function (str) {
            str = (str || '').replace("T", " ");
            if (str.indexOf(".") != -1) {
                return str.substring(0, str.indexOf("."));
            }
            return str;
        }
    },
    MessageBox: {
        ok: function (msg) {
            layer.alert(msg, {
                icon: 1
            })
        },
        alert: function (msg) {
            layer.alert(msg)
        },
        warnning: function (msg) {
            layer.alert(msg, {
                icon: 0
            })
        },
        error: function (msg) {
            layer.alert(msg, {
                icon: 2
            })
        },
        confirm: function (msg, callback) {
            layer.confirm(msg, callback);
        }
    },
    DataContext: {
        OutputCoord:[{
            label: 'GCS_WGS_1984',
            value: 'WGS 1984'
        }, {
            label: 'GCS_Beijing_1954',
            value: 'Beijing 1954'
        }, {
            label: 'GCS_Xian_1980',
            value: 'Xian 1980'
        }, {
            label: 'CGCS_2000',
            value: 'China Geodetic Coordinate System 2000'
        }],
        KJCKX: [{ "value": "001", "text": "1954年北京坐标系", "id": null }, { "value": "002", "text": "1980年西安坐标系", "id": null }, { "value": "003", "text": "地方独立坐标系", "id": null }, { "value": "004", "text": "全球参考系", "id": null }, { "value": "005", "text": "IAG 1979年大地参照系", "id": null }, { "value": "006", "text": "世界大地坐标系", "id": null }, { "value": "007", "text": "2000中国大地坐标系统", "id": null }],
        ZFJ: [{ "value": "通用字符集转换格式8", "text": "004", "id": null }, { "value": "GB/T 15273第一部分", "text": "006", "id": null }, { "value": "GB/T 15273第一部分", "text": "006", "id": null }, { "value": "美国信息交换标准代码", "text": "025", "id": null }, { "value": "美国信息交换标准代码", "text": "025", "id": null }, { "value": "BIG5", "text": "028", "id": null }, { "value": "GB2312", "text": "029", "id": null }, { "value": "GB18030", "text": "030", "id": null }, { "value": "汉字内码扩展规范(GBK)", "text": "031", "id": null }, { "value": "HZ", "text": "032", "id": null }],
        SJBSFS: [{ "value": "001", "text": "矢量", "id": null }, { "value": "002", "text": "网格", "id": null }, { "value": "003", "text": "文本", "id": null }, { "value": "004", "text": "三角网", "id": null }, { "value": "005", "text": "立体模型", "id": null }, { "value": "006", "text": "视频", "id": null }, { "value": "007", "text": "矩阵", "id": null }, { "value": "008", "text": "数据表", "id": null }],
        ZK: [{ "value": "001", "text": "完成", "id": null }, { "value": "002", "text": "历史档案", "id": null }, { "value": "003", "text": "作废", "id": null }, { "value": "004", "text": "连续更新", "id": null }, { "value": "005", "text": "列入计划", "id": null }, { "value": "006", "text": "有需求", "id": null }, { "value": "007", "text": "正在建设中", "id": null }],
        SYXZ: [{ "value": "001", "text": "版权", "id": null }, { "value": "002", "text": "专利权", "id": null }, { "value": "003", "text": "正在申请专利权", "id": null }, { "value": "004", "text": "商标", "id": null }, { "value": "005", "text": "许可证", "id": null }, { "value": "006", "text": "知识产权", "id": null }, { "value": "007", "text": "限制", "id": null }, { "value": "008", "text": "其它限制", "id": null }],
        AQDJ: [{ "value": "0", "text": "公开", "id": null }, { "value": "1", "text": "国内", "id": null }, { "value": "2", "text": "内部", "id": null }, { "value": "3", "text": "秘密", "id": null }, { "value": "4", "text": "机密", "id": null }, { "value": "5", "text": "绝密", "id": null }, { "value": "6", "text": "未分级", "id": null }],
        BDXS: [{ "value": "001", "text": "数字文档", "id": null }, { "value": "002", "text": "硬拷贝文档", "id": null }, { "value": "003", "text": "数字影像", "id": null }, { "value": "004", "text": "硬拷贝影像", "id": null }, { "value": "005", "text": "数字地图", "id": null }, { "value": "006", "text": "硬拷贝地图", "id": null }, { "value": "007", "text": "数字模型", "id": null }, { "value": "008", "text": "硬拷贝模型", "id": null }, { "value": "009", "text": "数字剖面", "id": null }, { "value": "010", "text": "硬拷贝剖面", "id": null }, { "value": "011", "text": "数字表格", "id": null }, { "value": "012", "text": "硬拷贝表格", "id": null }, { "value": "013", "text": "数字视频", "id": null }, { "value": "014", "text": "硬拷贝视数字视频频", "id": null }],
        WHGXPV: [{ "value": "001", "text": "连续", "id": null }, { "value": "002", "text": "按日", "id": null }, { "value": "003", "text": "按周", "id": null }, { "value": "004", "text": "按月", "id": null }, { "value": "005", "text": "按季", "id": null }, { "value": "006", "text": "按半年", "id": null }, { "value": "007", "text": "按年", "id": null }, { "value": "008", "text": "按需求", "id": null }, { "value": "009", "text": "不固定", "id": null }, { "value": "010", "text": "无计划", "id": null }, { "value": "011", "text": "未知", "id": null }, { "value": "012", "text": "按旬", "id": null }, { "value": "013", "text": "每5天", "id": null }],
        FFJZMC: [{ "value": "001", "text": "只读光盘", "id": null }, { "value": "002", "text": "数字视频光盘", "id": null }, { "value": "003", "text": "数字视频只读光盘", "id": null }, { "value": "004", "text": "3.5”软盘", "id": null }, { "value": "006", "text": "7磁道磁带", "id": null }, { "value": "007", "text": "9磁道磁带", "id": null }, { "value": "008", "text": "3480盒式磁带", "id": null }, { "value": "009", "text": "3490盒式磁带", "id": null }, { "value": "010", "text": "3590盒式磁带", "id": null }, { "value": "011", "text": "4mm盒式磁带", "id": null }, { "value": "012", "text": "8mm盒式磁带", "id": null }, { "value": "013", "text": "0.25 盒式磁带", "id": null }, { "value": "014", "text": "数字线性磁带", "id": null }, { "value": "015", "text": "在线", "id": null }, { "value": "016", "text": "卫星", "id": null }, { "value": "017", "text": "电话", "id": null }, { "value": "018", "text": "硬拷贝", "id": null }, { "value": "019", "text": "硬盘", "id": null }, { "value": "020", "text": "U盘", "id": null }, { "value": "021", "text": "电子邮件", "id": null }, { "value": "022", "text": "可擦写光盘", "id": null }, { "value": "023", "text": "其它", "id": null }],
        FFFWLX: [{ "value": "ArcIMS", "text": "ArcIMS", "id": null }, { "value": "MapInfo", "text": "MapInfo", "id": null }],
        OutputFormat: [{ "id": "21e5a8f1-cdc4-11e7-a735-005056bb1c7e", "codeName": "GML", "codeValue": ".gml", "codeDesc": "Geography MarkupLanguate地理标记语言", "dataTypeID": "25159792-cdba-11e7-a735-005056bb1c7e", "codeSort": 9, "remark": null, "keywords": null }, { "id": "3e37fde0-cdc7-11e7-a735-005056bb1c7e", "codeName": "GeoJSON", "codeValue": ".json", "codeDesc": "geoJson标准文件", "dataTypeID": "25159792-cdba-11e7-a735-005056bb1c7e", "codeSort": 12, "remark": null, "keywords": null }, { "id": "83378c80-cdbd-11e7-a735-005056bb1c7e", "codeName": "ESRI Shapefile", "codeValue": ".shp", "codeDesc": "ArcGIS格式文件", "dataTypeID": "25159792-cdba-11e7-a735-005056bb1c7e", "codeSort": 1, "remark": null, "keywords": null }, { "id": "a6616231-cdc3-11e7-a735-005056bb1c7e", "codeName": "CSV", "codeValue": ".csv", "codeDesc": "CSV文件", "dataTypeID": "25159792-cdba-11e7-a735-005056bb1c7e", "codeSort": 8, "remark": null, "keywords": null }, { "id": "eb73c482-cdc5-11e7-a735-005056bb1c7e", "codeName": "KML", "codeValue": ".kml", "codeDesc": "地标文件", "dataTypeID": "25159792-cdba-11e7-a735-005056bb1c7e", "codeSort": 11, "remark": null, "keywords": null }, { "id": "a87e09fe-cdbd-11e7-a735-005056bb1c7e", "codeName": "MapInfo File", "codeValue": ".tab", "codeDesc": "MapInfo格式文件", "dataTypeID": "25159792-cdba-11e7-a735-005056bb1c7e", "codeSort": 2, "remark": null, "keywords": null }, { "id": "c817e346-cdca-11e7-a735-005056bb1c7e", "codeName": "DXF", "codeValue": ".dxf", "codeDesc": "DXF文件", "dataTypeID": "25159792-cdba-11e7-a735-005056bb1c7e", "codeSort": 19, "remark": null, "keywords": null }],
        scaleList: [{
            label: "20 000 000",
            number: 20000000,
            level: "A"
        },
        {
            label: "15 000 000",
            number: 15000000,
            level: "B"
        },
        {
            label: "12 000 000",
            number: 12000000,
            level: "C"
        },
        {
            label: "10 000 000",
            number: 10000000,
            level: "D"
        },
        {
            label: "7 500 000",
            number: 7500000,
            level: "E"
        },
        {
            label: "6 000 000",
            number: 6000000,
            level: "F"
        },
        {
            label: "5 000 000",
            number: 5000000,
            level: "G"
        },
        {
            label: "4 000 000",
            number: 4000000,
            level: "H"
        },
        {
            label: "2 500 000",
            number: 2500000,
            level: "I"
        },
        {
            label: "2 000 000",
            number: 2000000,
            level: "J"
        },
        {
            label: "1 000 000",
            number: 1000000,
            level: "K"
        },
        {
            label: "500 000",
            number: 500000,
            level: "L"
        },
        {
            label: "250 000",
            number: 250000,
            level: "M"
        },
        {
            label: "200 000",
            number: 200000,
            level: "N"
        },
        {
            label: "100 000",
            number: 100000,
            level: "O"
        },
        {
            label: "50 000",
            number: 50000,
            level: "P"
        },
        {
            label: "25 000",
            number: 25000,
            level: "Q"
        },
        {
            label: "10 000",
            number: 10000,
            level: "R"
        },
        {
            label: "5 000",
            number: 5000,
            level: "S"
        },
        {
            label: "2 000",
            number: 2000,
            level: "T"
        },
        {
            label: "1 000",
            number: 1000,
            level: "U"
        },
        {
            label: "500",
            number: 500,
            level: "V"
        }],
        version: [{
            label: "验收版",
            version: 1
        }, {
            label: "最终版",
            version: 2
        }],
        StyleDataType: {
            点: '6b6941f1-67a3-11e7-8eb2-005056bb1c7e',
            线: '7776934c-67a3-11e7-8eb2-005056bb1c7e',
            面: 'a2758dc0-67a3-11e7-8eb2-005056bb1c7e'
        },
        DXM: (function () {
            var data = [];
            var temp={
                点: '6b6941f1-67a3-11e7-8eb2-005056bb1c7e',
                线: '7776934c-67a3-11e7-8eb2-005056bb1c7e',
                面: 'a2758dc0-67a3-11e7-8eb2-005056bb1c7e'
            };
            for (var p in temp)
            {
                data.push({
                    id: temp[p],
                    value: temp[p],
                    text: p
                });
            }
            return data;
        })()
    }
}

window.ok = Base.MessageBox.ok;
window.alert = Base.MessageBox.alert;
window.warnning = Base.MessageBox.warnning;
window.error = Base.MessageBox.error;
window.confirm = Base.MessageBox.confirm;

Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
    }
    return -1;
};


Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
};

