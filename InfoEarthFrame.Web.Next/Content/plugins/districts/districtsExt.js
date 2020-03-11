/// <reference path="../../jquery/jquery-2.0.3.min.js" />
(function ($) {
    $.fn.districtsCtlExt = function (settings) {
        //默认选项
        var defaluts =
            {
                xzqhcode: "",//行政区划代码
                width:300,
                html:""
            };
        $.extend(defaluts, settings);
        var _district = $(this).empty();
        var _html;
        if (defaluts.html) {
            _html = defaluts.html;
        }
        else {
            _html = '<div id="districtsCtlInfoEarth" style="float: left; width: 255px;"><div id="F_ProvinceId" type="select" class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div><div id="F_CityId" type="select" class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div><div id="F_CountyId" type="select" class="ui-select" style="float: left; width: 84px; margin-right: 1px;"></div></div>';
        }
        _district.append(_html);
        var code = "";
        if (defaluts.xzqhcode) {
            code = defaluts.xzqhcode.replace(/0+$/g, "");
        }
        var selectProviceId = '';
        var selectCityId = '';
        var selectCounty = '';
        if (code.length >= 2) {
            selectProviceId = code.substr(0, 2) + "0000";
        }
        if (code.length >= 4) {
            selectCityId = code.substr(0, 4) + "00";
        }
        if (code.length >= 6) {
            selectCounty = code.substr(0, 6);
        }
        
        var districtsCtlInfoEarth = _district.find("#districtsCtlInfoEarth");
        var _provinceCommbox = _district.find("#F_ProvinceId");
        var _cityCommbox = _district.find("#F_CityId");
        var _countyCommbox = _district.find("#F_CountyId");
        if (!defaluts.html) {
            districtsCtlInfoEarth.width(defaluts.width);
            var comboxW = (defaluts.width - 9) / 3;
            _provinceCommbox.width(comboxW);
            _cityCommbox.width(comboxW);
            _countyCommbox.width(comboxW);
        }
       
        //var _addressText = _district.find("#F_Address");
        //省份
        _provinceCommbox.ComboBox({
            url: "../../SystemManage/Area/GetProvinceListJson",
            param: { parentId: "0",provinceIds:'' },
            id: "F_AreaCodeHistory",
            text: "F_AreaName",
            description: "选择省",
            //width: defaluts.comboboxwidth.toString()+"px",
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
                //width: defaluts.comboboxwidth.toString() + "px",
                height: "170px"
            });
            _countyCommbox.ComboBox({
                url: "../../SystemManage/Area/GetAreaListJson",
                param: { parentId: value },
                id: "F_AreaCodeHistory",
                text: "F_AreaName",
                description: "选择县/区",
                //width: defaluts.comboboxwidth.toString() + "px",
                height: "170px"
            });
        });
        //城市
        _cityCommbox.ComboBox({
            description: "选择市",
            height: "170px"
        }).bind("change", function () {
            var value = $(this).attr('data-value');
            _countyCommbox[0].innerHTML = "选择县/区";
            _countyCommbox.attr('data-value', '');
            if (!value) {
                value = "abcdef";
            }
            //if (value) {
                _countyCommbox.ComboBox({
                    url: "../../SystemManage/Area/GetAreaListJson",
                    param: { parentId: value },
                    id: "F_AreaCodeHistory",
                    text: "F_AreaName",
                    description: "选择县/区",
                    height: "170px"
                });
            //}
        });
        //县/区
        _countyCommbox.ComboBox({
            description: "选择县/区",
            height: "170px"
        });
        _provinceCommbox.comboBoxSetValue(selectProviceId);
        _cityCommbox.comboBoxSetValue(selectCityId);
        _countyCommbox.comboBoxSetValue(selectCounty);

        function setDistrictValue(xzqhcode)
        {
            if (!xzqhcode) {
                return;
            }
            var _province = _district.find("#F_ProvinceId");
            var _city = _district.find("#F_CityId");
            var _county = _district.find("#F_CountyId");

            var provinceid = "", cityid = "", countyid = "";
            if (xzqhcode.length >= 2) {
                provinceid = xzqhcode.substr(0, 2) + "0000";
            }
            if (xzqhcode.length >= 4) {
                cityid = xzqhcode.substr(0, 4) + "00";
            }
            if (xzqhcode.length >= 6) {
                countyid = xzqhcode.substr(0, 6);
            }

            _province.comboBoxSetValue(provinceid);
            _city.comboBoxSetValue(cityid);
            _county.comboBoxSetValue(countyid);
        }
        function getDistrictValue() {
            var districtInfo = {
                provinceId: "",
                provinceName: "",
                cityId: "",
                cityName: "",
                countyId: "",
                countyName: "",
                address: "",
                xzqhcode:""
            };
            var _province = _district.find("#F_ProvinceId");
            var _city = _district.find("#F_CityId");
            var _county = _district.find("#F_CountyId");
            districtInfo.provinceId = _province[0].dataset.value;
            districtInfo.provinceName = _province[0].dataset.text;
            districtInfo.cityId = _city[0].dataset.value;
            districtInfo.cityName = _city[0].dataset.text;
            districtInfo.countyId = _county[0].dataset.value;
            districtInfo.countyName = _county[0].dataset.text;
            if (districtInfo.provinceId) {
                districtInfo.xzqhcode = districtInfo.provinceId.substr(0, 2);
            }
            if (districtInfo.cityId) {
                districtInfo.xzqhcode += districtInfo.cityId.substr(2, 2);
            }
            if (districtInfo.countyId) {
                districtInfo.xzqhcode += districtInfo.countyId.substr(4, 2);
            }
            for (var i = districtInfo.xzqhcode.length; i < 6; i++) {
                districtInfo.xzqhcode += '0';
            }
            return districtInfo;
        }
        _district[0].dx = {
            setDistrictValue: function (xzqhcode) {
                setDistrictValue(xzqhcode)
            },
            getDistrictValue: function () {
                return getDistrictValue();
            }
        };
        return _district;
    };
    $.fn.setDistrictValue = function (xzqhcode) {
        if (this[0].dx) {
            this[0].dx.setDistrictValue(xzqhcode);
        }
        return null;
    };
    $.fn.getDistrictValue = function () {
        if (this[0].dx) {
            return this[0].dx.getDistrictValue();
        }
        return null;
    };
})(jQuery);