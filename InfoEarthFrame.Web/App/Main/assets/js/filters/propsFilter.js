'use strict';

app.filter('propsFilter', function () {
    return function (items, props) {
        var out = [];
        if (angular.isArray(items) && !!props && angular.isObject(props)) {
            var keys = Object.keys(props);
            items.forEach(function (item) {
                var itemMatches = false;
                for (var i = 0; i < keys.length; i++) {
                    var prop = keys[i];
                    var text = props[prop].toLowerCase();
                    if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                        itemMatches = true;
                        break;
                    }
                }
                if (itemMatches) {
                    out.push(item);
                }
            });
        } else {
            out = items;
        }
        return out;
    };
});

//超过8个字符的字符串改写成...
app.filter('changeStr', function () {
    return function (data, str) {
        var tmpStr = angular.copy(data);
        if (tmpStr.length > 8) {
            tmpStr = tmpStr.substring(0, 7) + "...";
        }
        return tmpStr;
    }
});

//时间过滤
app.filter('dateFilter', function () {
    return function (text) {
        var date = "";
        if (!!text) {
            date = text.replace('T', ' ');
        }
        return date;
    }
});

//图层属性列表过滤
app.filter('tdFilter', function () {
    return function (text) {
        var date = "";
        if (!!text) {
            date = text.split(',,')[0];
        }
        return date;
    }
});

//图层导入消息状态过滤
app.filter('msgStatusFilter', function () {
    return function (text) {
        var data = "";
        if (language_English == 1) {
            if (text == 0) {
                data = "Waiting";
            }
            if (text == 1) {
                data = "Success";
            }
            if (text == 2) {
                data = "Failed";
            }
            if (text == 3) {
                data = "Importing";
            }
        }
        else {
            if (text == 0) {
                data = "等待导入";
            }
            if (text == 1) {
                data = "导入成功";
            }
            if (text == 2) {
                data = "导入失败";
            }
            if (text == 3) {
                data = "正在导入";
            }
        }
        return data;
    }
});

//类型过滤
app.filter('datatypeFilter', function () {
    return function (text) {
        //console.log(language_English);
        if (language_English == 1) {
            if (text === "6b6941f1-67a3-11e7-8eb2-005056bb1c7e") {
                text = "Point";
            }
            if (text === "7776934c-67a3-11e7-8eb2-005056bb1c7e") {
                text = "Line";
            }
            if (text === "a2758dc0-67a3-11e7-8eb2-005056bb1c7e") {
                text = "Polygon";
            }
        }
        else {
            if (text === "6b6941f1-67a3-11e7-8eb2-005056bb1c7e") {
                text = "点";
            }
            if (text === "7776934c-67a3-11e7-8eb2-005056bb1c7e") {
                text = "线";
            }
            if (text === "a2758dc0-67a3-11e7-8eb2-005056bb1c7e") {
                text = "面";
            }
        }
        return text;
    }
});

app.filter('changeSize', function () {
    return function (data, str) {
        if (data < 1024) {
            return data + ' B';
        }
        else if (data / 1024 < 1024) {
            return (data / 1024).toFixed(2) + ' KB';
        }
        else if (data / 1024 / 1024 < 1024) {
            return (data / 1024 / 1024).toFixed(2) + ' MB';
        }
        else {
            return (data / 1024 / 1024 / 1024).toFixed(2) + ' GB';
        }
    }
});

//日期过滤
app.filter('exdate', function () {
    return function (createTime, str) {
        var newdate = new Date(createTime);
        var showdate = newdate.getFullYear() + "/" + (newdate.getMonth() + 1) + "/" + newdate.getDate() + " " + newdate.getHours() + ":" + newdate.getMinutes() + ":" + newdate.getSeconds();
        return showdate;
    }
});

app.filter('changeDate', function () {
    return function (date, str) {
        var newdate = new Date();
        var showdate = newdate.getFullYear() + "/" + (newdate.getMonth() + 1) + "/" + newdate.getDate() + " " + newdate.getHours() + ":" + newdate.getMinutes() + ":" + newdate.getSeconds();
        return showdate;
    }
});

app.filter('fileChange', function () {
    return function (fileType, str) {
        switch (fileType) {
            case 1:
                fileType = language_English == 1 ? "Format conversion" : "格式转换";
                break;
            case 2:
                fileType = language_English == 1 ? "Coordinate conversion" : "坐标转换";
                break;
            case 3:
                fileType = language_English == 1 ? "Projection conversion" : "投影转换";
                break;
            default:
                fileType = language_English == 1 ? "unknow" : "未知";
                break;
        }
        return fileType;
    }
});

app.filter('exVers', function () {
    return function (logType, str) {
        switch (logType) {
            case 1:
                logType = language_English == 1 ? "Acceptance version" : "验收版";
                break;
            case 2:
                logType = language_English == 1 ? "Final version" : "最终版";
                break;
            default:
                logType = language_English == 1 ? "Version infomation error" : "版本信息有误";
                break;
        }
        return logType;
    }
});

app.filter('extension', function () {
    return function (name, str) {
        var arrName = name.split('\\');
        var extName = arrName[arrName.length - 1];
        return extName;
    }
});

