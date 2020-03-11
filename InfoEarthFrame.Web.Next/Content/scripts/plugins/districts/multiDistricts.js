(function ($) {
    $.fn.mulDistrictsCtl = function (settings) {
        var _coordSys = "";
        var _isProjectiveGeo = "";
        //默认选项
        var defaluts =
            {
                provinceId: "",//初始化省的个数
                selectProviceId: "",//选中省
                selectCityId: "",//选中市
                selectCounty: "",//选中县
                html:""
            };
        $.extend(defaluts, settings);
        var _district = $(this).empty();
        var _html;
        if (defaluts.html) {
            _html = defaluts.html;
        }
        else {
            _html = " <table><tr><td class=\"formTitle\">省</td><td> <div id=\"PROVINCE\" multiples=\"true\" type=\"select\" class=\"ui-select\" style=\"float: left; width: 160px; margin-right: 1px;\"></div></td></tr>" +
           "<tr><td class=\"formTitle\">市</td><td><div id=\"CITY\" multiples=\"true\" type=\"select\" class=\"ui-select\" style=\"float: left; width: 160px; margin-right: 1px;\"></div></td></tr>" +
        "<tr><td class=\"formTitle\">区</td><td><div id=\"COUNTY\" multiples=\"true\" type=\"select\" class=\"ui-select\" style=\"float: left; width: 160px; margin-right: 1px;\"></div> </td></tr></table>";
          //  _html = '<div style="float: left; width: 255px;"><div id="PROVINCE" type="select" multiples class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div><div id="CITY" type="select" multiples class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div><div id="COUNTY" type="select" multiples class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div></div>';
        }
        _district.append(_html);
        var _provinceCommbox = _district.find("#PROVINCE");
        var _cityCommbox = _district.find("#CITY");
        var _countyCommbox = _district.find("#COUNTY");
        //var _addressText = _district.find("#F_Address");
        //省份初始化
        _provinceCommbox.ComboBoxEx({
            url: "../../SystemManage/Area/GetProvinceListJson",
            param: { codes: "0", provinceIds: defaluts.provinceId },
            id: "F_AreaCodeHistory",
            text: "F_AreaName",
            description: "选择省",
            height: "170px"
        }).bind("change", function () {
            var value = $(this).attr('data-value');
            if (!value) {
                _cityCommbox.ComboBoxEx({
                    height: "170px",
                    data:[]
                });
                _countyCommbox.ComboBoxEx({
                    height: "170px",
                    data: []
                });
                _provinceCommbox.find(".ui-select-text").text("选择省").css("color", "#999");
                _provinceCommbox.attr('data-value', '');
                _cityCommbox.find(".ui-select-text").text("选择市").css("color","#999");
                _cityCommbox.attr('data-value', '');
                _countyCommbox.find(".ui-select-text").text("选择县/区").css("color", "#999");
                _countyCommbox.attr('data-value', '');
                return;
            }
            _cityCommbox[0].innerHTML = "选择市";
            _cityCommbox.attr('data-value', '');
            _countyCommbox[0].innerHTML = "选择县/区";
            _countyCommbox.attr('data-value', '');
           
            _cityCommbox.ComboBoxEx({
                url: "../../SystemManage/Area/GetAllByParentCodes",
                param: { codes: value },
                id: "F_AreaCodeHistory",
                text: "F_AreaName",
                description: "选择市",
                height: "170px",
                method:"POST"
            });
            _countyCommbox.ComboBoxEx({
                url: "../../SystemManage/Area/GetAllByParentCodes",
                param: { codes: "" },
                id: "F_AreaCodeHistory",
                text: "F_AreaName",
                description: "选择县/区",
                height: "170px",
                method: "POST"
            });
        });
        var selectP = "";
        if (defaluts.selectProviceId!="") {
            selectP = defaluts.selectProviceId;
        }
        //市初始化
        _cityCommbox.ComboBoxEx({
            url: "../../SystemManage/Area/GetAllByParentCodes",
            param: { codes: selectP },
            id: "F_AreaCodeHistory",
            text: "F_AreaName",
            description: "选择市",
            height: "170px",
            method: "POST"
        }).bind("change", function () {
            var value = $(this).attr('data-value');
            //_countyCommbox[0].innerHTML = "选择县/区";
            _countyCommbox.find(".ui-select-text").text("选择县/区");
            _countyCommbox.attr('data-value', '');
            if (!value) {
                //_district.find("#CITY").find(".ui-select-text").text("选择市");
                //_district.find("#CITY").find(".ui-select-text").css("color", "#999");
                _countyCommbox.ComboBoxEx({
                    height: "170px",
                    data: []
                });
                _cityCommbox.find(".ui-select-text").text("选择市").css("color", "#999");
                _cityCommbox.attr('data-value', '');
                _countyCommbox.find(".ui-select-text").text("选择县/区").css("color", "#999");
                _countyCommbox.attr('data-value', '');
                return;
            }
            //if (value) {
                _countyCommbox.ComboBoxEx({
                    url: "../../SystemManage/Area/GetAllByParentCodes",
                    param: { codes: value },
                    id: "F_AreaCodeHistory",
                    text: "F_AreaName",
                    description: "选择县/区",
                    height: "170px",
                    method: "POST"
                });
            //}
        });

        var selectC = "";
        if (defaluts.selectCityId != "") {
            selectC = defaluts.selectCityId;
        }
        //县/区
        _countyCommbox.ComboBoxEx({
            url: "../../SystemManage/Area/GetAllByParentCodes",
            param: { codes: selectC },
            id: "F_AreaCodeHistory",
            text: "F_AreaName",
            description: "选择县/区",
            height: "170px",
            method: "POST"
        }).bind("change", function () {
            var value = $(this).attr('data-value');
            if (!value) {
                _district.find("#COUNTY").find(".ui-select-text").text("选择县/区");
                _district.find("#COUNTY").find(".ui-select-text").css("color", "#999");
            }
        });
        
        _provinceCommbox.comboBoxSetValue(defaluts.selectProviceId);
        _cityCommbox.comboBoxSetValue(defaluts.selectCityId);
        _countyCommbox.comboBoxSetValue(defaluts.selectCounty);
        function setDistrictValue(provinceid, cityid, countyid, address) {
            var _province = _district.find("#PROVINCE");
            var _city = _district.find("#CITY");
            var _county = _district.find("#COUNTY");
            _province.comboBoxSetValue(provinceid);
            _city.comboBoxSetValue(cityid);
            _county.comboBoxSetValue(countyid);
            //_address.val(address);
        };
        function getDistrictValue() {
            var districtInfo = {
                provinceId: "",
                provinceName: "",
                cityId: "",
                cityName: "",
                countyId: "",
                countyName: "",
                address: "",
                xzqhcode: ""
            };
            var _province = _district.find("#PROVINCE");
            var _city = _district.find("#CITY");
            var _county = _district.find("#COUNTY");
            districtInfo.provinceId = _province[0].dataset.value;
            districtInfo.provinceName = _province[0].dataset.text;
            districtInfo.cityId = _city[0].dataset.value;
            districtInfo.cityName = _city[0].textContent;
            districtInfo.countyId = _county[0].dataset.value;
            districtInfo.countyName = _county[0].textContent;
            //districtInfo.address = _district.find("#F_Address").val();
            if (districtInfo.provinceId) {
                districtInfo.xzqhcode = districtInfo.provinceId;
            }
            if (districtInfo.cityId) {
                districtInfo.xzqhcode = districtInfo.cityId;
            }
            if (districtInfo.countyId) {
                districtInfo.xzqhcode = districtInfo.countyId;
            }
            return districtInfo;
        };
        function resetDistrict() {
            _provinceCommbox.ComboBoxEx({
                url: "../../SystemManage/Area/GetProvinceListJson",
                param: { codes: "0", provinceIds: defaluts.provinceId },
                id: "F_AreaCodeHistory",
                text: "F_AreaName",
                description: "选择省",
                height: "170px"
            });
            _cityCommbox.ComboBoxEx({
                height: "170px",
                data: []
            });
            _countyCommbox.ComboBoxEx({
                height: "170px",
                data: []
            });
            _provinceCommbox.find(".ui-select-text").text("选择省").css("color", "#999");
            _provinceCommbox.attr('data-value', '');
            _cityCommbox.find(".ui-select-text").text("选择市").css("color", "#999");
            _cityCommbox.attr('data-value', '');
            _countyCommbox.find(".ui-select-text").text("选择县/区").css("color", "#999");
            _countyCommbox.attr('data-value', '');
                
        };
        _district[0].d = {
            setDistrictValue: function (provinceid, cityid, countyid, address) {
                setDistrictValue(provinceid, cityid, countyid, address)
            },
            getDistrictValue: function () {
                return getDistrictValue();
            },
            resetDistrict: function () {
                resetDistrict();
            }
        };
        return _district;
    };
    $.fn.setDistrictValue = function (provinceid, cityid, countyid, address) {
        if (this[0].d) {
            this[0].d.setDistrictValue(provinceid, cityid, countyid, address);
        }
        return null;
    };
    $.fn.getDistrictValue = function () {
        if (this[0].d) {
            return this[0].d.getDistrictValue();
        }
        return null;
    };
    $.fn.resetDistrict = function () {
        if (this[0].d) {
            this[0].d.resetDistrict();
        }
        return null;
    };
})(jQuery);