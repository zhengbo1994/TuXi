(function ($) {
    $.fn.districtsCtl = function (settings) {
        var _coordSys = "";
        var _isProjectiveGeo = "";
        //默认选项
        var defaluts =
            {
                provinceId: "",//初始化省的个数
                selectProviceId: "",//选中省
                selectCityId: "",//选中市
                selectCounty: "",//选中县
                html: "",
                callBack:null//回调函数
            };
        $.extend(defaluts, settings);
        var _district = $(this).empty();
        var _html;
        if (defaluts.html) {
            _html = defaluts.html;
        }
        else {
            _html = '<div style="float: left; width: 255px;"><div id="PROVINCE" type="select" class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div><div id="CITY" type="select" class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div><div id="COUNTY" type="select" class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div></div>';
        }
        _district.append(_html);
        var _provinceCommbox = _district.find("#PROVINCE");
        var _cityCommbox = _district.find("#CITY");
        var _countyCommbox = _district.find("#COUNTY");
        //var _addressText = _district.find("#F_Address");
        //省份
        _provinceCommbox.ComboBox({
            url: "../../SystemManage/Area/GetProvinceListJson",
            param: { parentId: "0",provinceIds:defaluts.provinceId },
            id: "F_AreaCodeHistory",
            text: "F_AreaName",
            description: "选择省",
            height: "170px"
        }).bind("change", function () {
            var value = $(this).attr('data-value');
            _cityCommbox[0].innerHTML = "选择市";
            _cityCommbox.attr('data-value', '');
            _countyCommbox[0].innerHTML = "选择县/区";
            _countyCommbox.attr('data-value', '');
            _cityCommbox.ComboBox({
                url: "../../SystemManage/Area/GetAreaListJson",
                param: { parentId: value },
                id: "F_AreaCodeHistory",
                text: "F_AreaName",
                description: "选择市",
                height: "170px"
            });
            _countyCommbox.ComboBox({
                url: "../../SystemManage/Area/GetAreaListJson",
                param: { parentId: value },
                id: "F_AreaCodeHistory",
                text: "F_AreaName",
                description: "选择县/区",
                height: "170px"
            });

            if (defaluts.callBack && typeof (eval(defaluts.callBack)) == "function") {
                defaluts.callBack(value);
            }
        });
        //城市
        _cityCommbox.ComboBox({
            description: "选择市",
            height: "170px"
        }).bind("change", function () {
            var value = $(this).attr('data-value');
            _countyCommbox[0].innerHTML = "选择县/区";
            _countyCommbox.attr('data-value', '');
                _countyCommbox.ComboBox({
                    url: "../../SystemManage/Area/GetAreaListJson",
                    param: { parentId: value },
                    id: "F_AreaCodeHistory",
                    text: "F_AreaName",
                    description: "选择县/区",
                    height: "170px"
                });
                if (defaluts.callBack && typeof (eval(defaluts.callBack)) == "function") {
                    defaluts.callBack(value);
                }
        });
        //县/区
        _countyCommbox.ComboBox({
            description: "选择县/区",
            height: "170px"
        }).bind("change", function () {
            var value = $(this).attr('data-value');
            if (defaluts.callBack && typeof (eval(defaluts.callBack)) == "function") {
                defaluts.callBack(value);
            }
        });
        _provinceCommbox.comboBoxSetValue(defaluts.selectProviceId);
        _cityCommbox.comboBoxSetValue(defaluts.selectCityId);
        _countyCommbox.comboBoxSetValue(defaluts.selectCounty);
        //_addressText.val(defaluts.address);
        function setDistrictValue(provinceid, cityid, countyid, address)
        {
            var _province = _district.find("#PROVINCE");
            var _city = _district.find("#CITY");
            var _county = _district.find("#COUNTY");
            //var _address = _district.find("#F_Address");
            _province.comboBoxSetValue(provinceid);
            _city.comboBoxSetValue(cityid);
            _county.comboBoxSetValue(countyid);
            //_address.val(address);
        }
        function getDistrictValue() {
            var districtInfo = {
                provinceId: "",
                provinceName: "",
                cityId: "",
                cityName: "",
                countyId: "",
                countyName: "",
                address: ""
            };
            var _province = _district.find("#PROVINCE");
            var _city = _district.find("#CITY");
            var _county = _district.find("#COUNTY");
            districtInfo.provinceId = _province[0].dataset.value;
            districtInfo.provinceName = _province[0].dataset.text;
            districtInfo.cityId = _city[0].dataset.value;
            districtInfo.cityName = _city[0].dataset.text;
            districtInfo.countyId = _county[0].dataset.value;
            districtInfo.countyName = _county[0].dataset.text;
            //districtInfo.address = _district.find("#F_Address").val();
            return districtInfo;
        }
        _district[0].d = {
            setDistrictValue: function (provinceid, cityid, countyid, address) {
                setDistrictValue(provinceid, cityid, countyid, address)
            },
            getDistrictValue: function () {
                return getDistrictValue();
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
})(jQuery);