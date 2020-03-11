/// <reference path="ImportKneecoordinate.html" />
/// <reference path="ImportKneecoordinate.html" />
/// <reference path="ImportKneecoordinate.html" />
(function ($) {
    $.fn.kneecoordinateCtl = function () {
        var _coordSys = "";
        var _isProjectiveGeo = "";
        //默认选项
        var defaluts =
            {
                CoordSys: null,//坐标系标准
                IsProjectiveGeo: true,//是否为投影坐标
                InitialData: 800//坐标列表
            };
        //$.extend(defaluts, settings);
        var _knee = $(this);
        var thisBody = _knee[0].ownerDocument.body;
        var _html = '<div><div>拐点坐标</div><div class="btn-group"><a id="_xi_an80" class="btn btn-default" >&nbsp;西安80</a><a id="_beijing54" class="btn btn-default" >&nbsp;北京54</a><a id="_guojia2000" class="btn btn-default">&nbsp;国家2000</a><a id="_wgs84" class="btn btn-default">&nbsp;WGS84</a></div>&nbsp;&nbsp;&nbsp;&nbsp;<div class="btn-group"><a id="_dili" class="btn btn-default" >&nbsp;地理</a><a id="_touying" class="btn btn-default" >&nbsp;投影</a></div></div><div><textarea id="txtKneecoordinate" class="form-control"></textarea></div>';
        _knee.append(_html);
        var _xi_an80 = _knee.find("#_xi_an80");
        _xi_an80.click(function () {
            _coordSys = "西安80坐标系";
            $("#_xi_an80").removeAttr("style");
            $("#_beijing54").removeAttr("style");
            $("#_guojia2000").removeAttr("style");
            $("#_wgs84").removeAttr("style");
            $(this).css("background-color","#CCCCCC");//gray
        });
        var _beijing54 = _knee.find("#_beijing54");
        _beijing54.click(function () {
            _coordSys = "北京54坐标系";
            $("#_xi_an80").removeAttr("style");
            $("#_beijing54").removeAttr("style");
            $("#_guojia2000").removeAttr("style");
            $("#_wgs84").removeAttr("style");
            $(this).css("background-color", "#CCCCCC");//gray
        });
        var _guojia2000 = _knee.find("#_guojia2000");
        _guojia2000.click(function () {
            _coordSys = "国家2000坐标系";
            $("#_xi_an80").removeAttr("style");
            $("#_beijing54").removeAttr("style");
            $("#_guojia2000").removeAttr("style");
            $("#_wgs84").removeAttr("style");
            $(this).css("background-color", "#CCCCCC");//gray
        });
        var _wgs84 = _knee.find("#_wgs84");
        _wgs84.click(function () {
            _coordSys = "WGS84坐标系";
            $("#_xi_an80").removeAttr("style");
            $("#_beijing54").removeAttr("style");
            $("#_guojia2000").removeAttr("style");
            $("#_wgs84").removeAttr("style");
            $(this).css("background-color", "#CCCCCC");//gray
        });
        var _dili = _knee.find("#_dili");
        _dili.click(function () {
            _isProjectiveGeo = "false";
            $("#_dili").removeAttr("style");
            $("#_touying").removeAttr("style");
            $(this).css("background-color", "#CCCCCC");//gray
        });
        var _touying = _knee.find("#_touying");
        _touying.click(function () {
            _isProjectiveGeo = "true";
            $("#_dili").removeAttr("style");
            $("#_touying").removeAttr("style");
            $(this).css("background-color", "#CCCCCC");//gray
        });

        
        $("#inputKneeExcel").hide();
        var inputKnee = _knee.find("#txtKneecoordinate");
        inputKnee.click(function () {
            var text= inputKnee.val();
            learun.dialogOpen({
                id: "kneecoordinate",
                title: '拐点坐标',
                url: '/Content/scripts/plugins/Kneecoordinate/ImportKneecoordinate.html?kneecoordinate=' + text,
                width: "680px",
                height: "510px",
                isClose: true,
                btn: null,
                callBack: function (iframeId) {
                    top.frames[iframeId].AcceptClick();
                }
            });
        });

        function outputKneecoordinate()
        {
            var text = $("#txtKneecoordinate").val();
            var _output = "";
            var output =
            {
                CoordSys: _coordSys,//坐标系标准
                IsProjectiveGeo: _isProjectiveGeo,//是否为投影坐标
                InitialData: text//坐标列表
            };
            return output;
        }

        _knee[0].k = {
            outputKneecoordinate: function () {
                return outputKneecoordinate();
            }
        };
        _xi_an80.click();
        _touying.click();
        return _knee;
    };
    $.fn.outputKneecoordinate = function () {
        if (this[0].k) {
            return this[0].k.outputKneecoordinate();
        }
        return null;
    };
})(jQuery);