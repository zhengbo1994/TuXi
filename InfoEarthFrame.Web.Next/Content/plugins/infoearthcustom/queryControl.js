/// <reference path="../../jquery/jquery-2.0.3.min.js" />
(function ($) {
    $.fn.queryControl = function (settings) {
        //默认选项
        var defaluts =
            {
                databaseLinkId: "",
                tableName: "",   //数据库表名
                width: 300,      //宽度
                height: 200,      //高度
                valueIsVisible: true //输入框是否可见
            };
        $.extend(defaluts, settings);
        var me = $(this);
        me.empty();
        var id = $(this).attr("id");
        var id = me.attr("id");
        if (id == null || id == "") {
            id = "map" + new Date().getTime();
            me.attr("id", id);
        }

        me.height(defaluts.height);
        me.width(defaluts.width);
        var temphtml = "";
        var addBtnHeight;
        if (defaluts.valueIsVisible) {
            temphtml = '<input id="txt_Keyword" type="text" class="form-control auto_border" placeholder="请输入关键字" style="width: 100px; float: left;display:display;" />';
            addBtnHeight = 237;
        }
        else {
            temphtml = '<input id="txt_Keyword" type="text" class="form-control auto_border" placeholder="请输入关键字" style="width: 100px; float: left;display:none;" />';
            addBtnHeight = 137;
        }
        //初始化的Html
        var html = '<div id="divSearchCondition" style="height:'+defaluts.height+'px;overflow-y:auto;">' +
                   '<div style="margin-bottom:5px;">' +
                    '<a style="white-space: nowrap; font-weight: normal;line-height:34px;">查询条件：</a>' +
                   '<a id="btn_AddCondition" class="btn btn-default" style="margin-left:'+addBtnHeight+'px;"><i class="fa fa-plus"></i>&nbsp;</a><br/>' +
                   '</div>'+
                    '<div id="divCondition" style="margin-bottom:2px;height:30px;">' +
                    '<select id="Condition0" class="form-control auto_border" style="width: 60px; float: left;margin-right:2px;" disabled="true"><option value="none">无</option></select>' +
                   '<select id="switchWhere" class="form-control auto_border" style="width: 100px;;float:left;margin-right:2px;"></select>' +
                   '<select id="logic" class="form-control auto_border" style="float: left;width: 80px;margin-right:2px;">' +
                      ' <option value="Equal">等于</option>' +
                      '<option value="NotEqual">不等于</option>' +
                      '<option value="Greater">大于</option>' +
                      '<option value="GreaterThan">大于等于</option>' +
                      '<option value="Less">小于</option>' +
                      '<option value="LessThan">小于等于</option>' +
                      '<option value="Null">为空</option>' +
                      '<option value="NotNull">不为空</option>' +
                      '<option value="Like">包含</option>' +
                  '</select>' +temphtml+
                '</div></div>'

        me.append(html);
        var comboxData = [];
        var datetimeArr = [];
        var dicArr = [];
        var intArr = [];
        $.ajax({
            url: "../../SystemManage/DataBaseTable/GetTableFiledListJson",
            data: { dataBaseLinkId: defaluts.databaseLinkId, tableName: defaluts.tableName },
            type: "get",
            dataType: "json",
            async: false,
            success: function (json) {
                comboxData = json;
                $.each(json, function (i) {
                    if (json[i].f_datatype == "datetime") {
                        datetimeArr.push(json[i].f_column);
                    }
                    if (json[i].f_datatype == "int") {
                        intArr.push(json[i].f_column);
                    }
                    var arr = json[i].f_remark.split('#');
                    if (arr.length > 1 && arr[1]!="")
                        dicArr[json[i].f_column] = arr[1];
                    var field = json[i].f_column + "#" + json[i].f_datatype;
                    var remark = arr[0];
                    me.find("#switchWhere").append($("<option title=" + remark + "></option>").val(field).html(remark));
                });
            }
        });
        var cmbSwitchwhere = me.find("#switchWhere");
        cmbSwitchwhere.change(function () {
            var _txtDisplay = ''
            if (defaluts.valueIsVisible)
                _txtDisplay = 'style="width: 100px; float: left;display:display;" />';
            else
                _txtDisplay = 'style="width: 100px; float: left;display:none;" />';
            var _value = (me.find("#switchWhere").val().split("#"))[0];
            if ($.inArray(_value,datetimeArr) != -1) {
                me.find("#txt_Keyword").remove();
                var divsearch = me.find("#divCondition");
                divsearch.append('<input id="txt_Keyword" type="text"  class="form-control input-wdatepicker" onfocus="WdatePicker()" ' + _txtDisplay);
            } else if (dicArr[_value]!=undefined) {
                me.find("#txt_Keyword").remove();
                var divsearch = me.find("#divCondition");
                divsearch.append('<div id="txt_Keyword" type="select" class="ui-select"  ' + _txtDisplay);
                $(me.find("#txt_Keyword")).ComboBox({
                    url: "../../SystemManage/DataItemDetail/GetDataItemListJson",
                    param: { EnCode: dicArr[_value] },
                    id: "F_ItemValue",
                    text: "F_ItemName",
                    description: "==请选择==",
                    height: "200px"
                });
            } 
            else {
                me.find("#txt_Keyword").remove();
                var divsearch = me.find("#divCondition");
                divsearch.append('<input id="txt_Keyword" type="text"  class="form-control auto_border" placeholder="请输入关键字" ' + _txtDisplay);
            }
            if (me.find("#logic").val() == "Null" || me.find("#logic").val() == "NotNull") {
                me.find("#txt_Keyword").attr("readonly", true);
            } else {
                me.find("#txt_Keyword").attr("readonly", false);
            }
            if ($.inArray(_value, intArr) != -1) {
                $(me.find("#txt_Keyword")).keyup(function () {
                    $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                }).bind("paste", function () {  //CTR+V事件处理    
                    $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                }).css("ime-mode", "disabled"); //CSS设置输入法不可用    
            }
        });

        var cmbLogic = me.find("#logic");
        cmbLogic.change(function () {
            var _valueLogic = me.find("#logic").val();
            if (_valueLogic == "Null" || _valueLogic == "NotNull") {
                me.find("#txt_Keyword").attr("readonly", true);
            } else {
                me.find("#txt_Keyword").attr("readonly", false);
            }
        });

        var addBtn = me.find("#btn_AddCondition");
        addBtn.click(function () {
            var countDiv = me.find("#divSearchCondition").children('div');
            var flag = countDiv.length;
            var _html = "";
            if (defaluts.valueIsVisible)
                _html = "style='width: 100px; float: left;display:display;' /></div>"
            else
                _html = "style='width: 100px; float: left;display:none;' /></div>";
            var divHtml = "<div id='divCondition" + flag + "' style=';clear:both;margin-bottom:2px;;height:30px;'><select id='Condition" + flag + "' class='form-control auto_border' style='width: 60px; float: left;margin-right:2px;'><option value='And'>And</option><option value='Or'>Or</option></select><select id='switchWhere" + flag + "' class='form-control auto_border' style='width: 100px; float: left;margin-right:2px;'></select><select id='logic" + flag + "' class='form-control auto_border' style='width: 80px; float: left;margin-right:2px;'><option value='Equal'>等于</option><option value='NotEqual'>不等于</option><option value='Greater'>大于</option><option value='GreaterThan'>大于等于</option><option value='Less'>小于</option><option value='LessThan'>小于等于</option><option value='Null'>为空</option><option value='NotNull'>不为空</option><option value='Like'>包含</option></select><input id='txt_Keyword" + flag + "' type='text' class='form-control auto_border' placeholder='请输入关键字' " + _html;
            me.find("#divSearchCondition").append(divHtml);
            $.each(comboxData, function (i) {
                var arr = comboxData[i].f_remark.split('#');
                var field = comboxData[i].f_column + "#" + comboxData[i].f_datatype;
                var remark = arr[0];
                me.find("#switchWhere" + flag).append($("<option title=" + remark + "></option>").val(field).html(remark));
            });
            var _cmbSwitchwhereEx = me.find("#switchWhere" + flag);
            _cmbSwitchwhereEx.bind("change", function () {
                var _switchWhereEx = this.id;
                var _flagEx = _switchWhereEx.substring(11, _switchWhereEx.length);
                var _valueEx = (me.find("#switchWhere" + _flagEx).val().split("#"))[0];
                var divsearch = me.find("#divCondition" + _flagEx);
                var _txthtml = "";
                if (defaluts.valueIsVisible)
                    _txthtml = 'style="width: 100px; float: left;display:display;" />'
                else
                    _txthtml = 'style="width: 100px; float: left;display:none;" />'
                me.find("#txt_Keyword" + _flagEx).remove();
                if ($.inArray(_valueEx, datetimeArr) != -1) {
                    divsearch.append($('<input id="txt_Keyword' + _flagEx + '" type="text"  class="form-control input-wdatepicker" onfocus="WdatePicker()" '+_txthtml));
                } else if (dicArr[_valueEx] != undefined) {
                    divsearch.append($('<div id="txt_Keyword' + _flagEx + '" type="select" class="ui-select" '+_txthtml));
                    $(me.find("#txt_Keyword" + _flagEx)).ComboBox({
                        url: "../../SystemManage/DataItemDetail/GetDataItemListJson",
                        param: { EnCode: dicArr[_valueEx] },
                        id: "F_ItemValue",
                        text: "F_ItemName",
                        description: "==请选择==",
                        height: "200px"
                    });
                } else {
                    divsearch.append('<input id="txt_Keyword' + _flagEx + '" type="text" class="form-control auto_border" placeholder="请输入关键字" '+_txthtml)
                }
                var _keyword2 = me.find("#txt_Keyword" + _flagEx);
                var _logicValueEx2 = me.find("#logic" + _flagEx).val();
                if (_logicValueEx2 == "Null" || _logicValueEx2 == "NotNull") {
                    _keyword2.attr("readonly", true);
                } else {
                    _keyword2.attr("readonly", false);
                }
                if ($.inArray(_valueEx, intArr) != -1) {
                    $(me.find("#txt_Keyword" + _flagEx)).keyup(function () {
                        $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                    }).bind("paste", function () {  //CTR+V事件处理    
                        $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                    }).css("ime-mode", "disabled"); //CSS设置输入法不可用    
                }
            });
            var _cmbLogic = me.find("#logic" + flag);
            _cmbLogic.bind("change", function () {
                var _logicWhereEx = this.id;
                var _logicFlagEx = _logicWhereEx.substring(5, _logicWhereEx.length);
                var _logicValueEx = me.find("#logic" + _logicFlagEx).val();
                var _keyword = me.find("#txt_Keyword" + _logicFlagEx);
                if (_logicValueEx == "Null" || _logicValueEx == "NotNull") {
                    _keyword.attr("readonly", true);
                } else {
                    _keyword.attr("readonly", false);
                }
            });

        });
        function convertLogic(str) {
            var _logic = "";
            switch (str) {
                case "Equal":
                    _logic = " =";
                    break;
                case "NotEqual":
                    _logic = " <>";
                    break;
                case "Greater":
                    _logic = " >";
                    break;
                case "GreaterThan":
                    _logic = " >=";
                    break;
                case "Less":
                    _logic = " <";
                    break;
                case "LessThan":
                    _logic = " <=";
                    break;
                case "Null":
                    _logic = " is null";
                    break;
                case "NotNull":
                    _logic = " is not null";
                    break;
                case "Like":
                    _logic = " like";
                    break;
            }
            return _logic;
        };
        function GetData() {
            var data = [];
            var isDic = false;
            var dicCode = "";
            var txtwhere = (me.find("#switchWhere")).val();
            var arrwhere = txtwhere.split("#");
            if (dicArr[arrwhere[0]] != undefined) {
                isDic = true;
                dicCode = dicArr[arrwhere[0]];
            }
            data.push({ QueryField: arrwhere[0], QueryFieldName: me.find("#switchWhere option:selected").text(), QueryTJ: convertLogic((me.find("#logic")).val()), ControlType: arrwhere[1], QueryConChar: "", IsDic: isDic, DicCode: dicCode });
            var countDiv = (me.find("#divSearchCondition")).children('div');
            for (var i = 2; i <= countDiv.length - 1; i++) {
                var _txtwhere = (me.find("#switchWhere" + i)).val();
                var arrwhere2 = _txtwhere.split("#");
                if (dicArr[arrwhere2[0]] != undefined) {
                    isDic = true;
                    dicCode = dicArr[arrwhere2[0]];
                } else {
                    isDic = false;
                    dicCode = "";
                }
                data.push({ QueryField: arrwhere2[0], QueryFieldName: me.find("#switchWhere" + i + " option:selected").text(), QueryTJ: convertLogic((me.find("#logic"+i)).val()), ControlType: arrwhere2[1], QueryConChar: (me.find("#Condition" + i)).val(), IsDic: isDic, DicCode: dicCode });
            }
            return data;
        }

        function GetSql() {
            var strWhere = "";
            if (!defaluts.valueIsVisible)
                return "";
            strWhere += ((me.find("#switchWhere")).val().split("#"))[0];
            strWhere += convertLogic((me.find("#logic")).val());
            if (me.find("#txt_Keyword")[0].localName == "div") {
                strWhere += " '" + me.find("#txt_Keyword")[0].dataset.value + "'";
            } else {
                strWhere += " '" + (me.find("#txt_Keyword")).val() + "'";
            }
            var countDiv = (me.find("#divSearchCondition")).children('div');
            for (var i = 2; i <= countDiv.length-1; i++) {
                strWhere += " " + (me.find("#Condition" + i)).val();
                strWhere += " " + ((me.find("#switchWhere" + i)).val().split("#"))[0];
                strWhere += convertLogic((me.find("#logic" + i)).val());
                if ((me.find("#logic" + i)).val() == "Like")
                {
                    if (me.find("#txt_Keyword" + i)[0].localName == "div") {
                        strWhere += " '" + me.find("#txt_Keyword" + i)[0].dataset.value + "'";
                    } else {
                        strWhere += " '%" + (me.find("#txt_Keyword" + i)).val() + "%'";
                    }
                    
                }
                if ((me.find("#logic" + i)).val() != "Null" && (me.find("#logic" + i)).val() != "NotNull" && (me.find("#logic" + i)).val() != "Like") {
                    if (me.find("#txt_Keyword" + i)[0].localName == "div") {
                        strWhere += " '" + me.find("#txt_Keyword" + i)[0].dataset.value + "'";
                    } else {
                        strWhere += " '" + (me.find("#txt_Keyword" + i)).val() + "'";
                    }
                }
            }
            return strWhere;
        };
        me[0].d = {
            GetSql: function () {
                return GetSql();
            },
            GetData: function () {
                return GetData();
            }
        };
        return me;
    }
    $.fn.GetSql = function () {
        if (this[0].d) {
            return this[0].d.GetSql();
        }
        return null;
    };
    $.fn.GetData = function () {
        if (this[0].d) {
            return this[0].d.GetData();
        }
        return null;
    };
})(jQuery);