var map = null;
var vectorSource = null;
var vectorLayer = null;
var menu_overlay = null;
var lon = null
var lat = null;
var infoBox;
var box;
var popup;
var msg;
var label;
var isOpenForm = false;
var dragBox = null;
var select = null;
var selectedFeatures = null;
var Massage;
//窗体加载事件
$(function () {
    InitalizeMap();//初始化地图
    AddControl();//添加控件
    InitalizeGPS();
    InitalizeLabelInfo();
    InitalizeSelect();
    AddVector();
    AddLabel();
    lon = document.getElementById('labellon');
    lat = document.getElementById('labellat');
    Massage = document.getElementById('MassageShow');
});
//地图初始化
function InitalizeMap() {
    //var weburl="http://192.168.100.18/iTelluro.Server/Service/DOM/dom.ashx";
	//map = NewWebTilesMap("mapDiv", 4, 180, -180, -90, 90, "bmngs", weburl, 512, 36)
    var url = "http://121.199.72.208/iTelluro.Server.yaan/Service/DOM/dom.ashx"
    map = NewWebTilesMap("mapDiv", 4, 180, -180, -90, 90, "bmng", url, 512, 36)
    map.getView().setCenter([110, 30]);//设置起始显示视图
};
//地图框选初始化
function InitalizeSelect() {
    select = new ol.interaction.Select();
    map.addInteraction(select);

    selectedFeatures = select.getFeatures();

    dragBox = new ol.interaction.DragBox({
        condition: ol.events.condition.platformModifiekeyOnly
    });
    infoBox = document.getElementById('msg');
    box = document.getElementById('box');
    msg = new ol.Overlay({
        element: box,
        positioning: 'absolute',//'bottom-center',
        stopEvent: false
    });

    //框选结束事件
    dragBox.on('boxend', function (e) {
        var info = [];
        var extent = dragBox.getGeometry().getExtent();
        //获取框选范围的图层要素信息
        vectorSource.forEachFeatureIntersectingExtent(extent, function (feature) {
            selectedFeatures.push(feature);
            info.push(feature.get('name'));
        });
        if (info.length > 0) {
            infoBox.innerHTML = "标注名称：" + info.join('</br>标注名称：');
            msg.setPosition(extent);//设置信息框位置
            map.addOverlay(msg);
        }
    });
    //框选开始事件
    dragBox.on('boxstart', function (e) {
        selectedFeatures.clear();
        //infoBox.innerHTML = ' ';
        //map.removeOverlay(msg);
    });
};
//添加标注信息初始化
function InitalizeLabelInfo() {
    popup = document.getElementById('popup');
    label = new ol.Overlay({
        element: popup,
        positioning: 'center-center',//'bottom-center',
        stopEvent: false
    });
    map.addOverlay(label);

    map.addEventListener('click', function (evt) {
        lon.value = evt.coordinate[0];
        lat.value = evt.coordinate[1];
        var feature = map.forEachFeatureAtPixel(evt.pixel,
            function (feature) {
                return feature;
            });
        if (feature) {
            popup.innerHTML = feature.get('name');
            label.setPosition(evt.coordinate);
        }
        else {
            label.setPosition(undefined);
        }
    });
};
//地图定位初始化
function InitalizeGPS() {
    var lon = document.getElementById('lon').value;
    var lat = document.getElementById('lat').value;
    //判断输入的经度是否有效
    document.getElementById('lon').onkeyup = function () {
        Massage.innerHTML = '';
        if (!isNaN(this.value) && (this.value <= 180) && (this.value >= -180)) {
            lon = this.value;
        }
        else {
            lon = null;
            Massage.innerHTML = '* 经度无效，请重新输入！';
        }
    };
    //判断输入的纬度是否有效
    document.getElementById('lat').onkeyup = function () {
        Massage.innerHTML = '';
        if (!isNaN(this.value) && (this.value <= 90) && (this.value >= -90)) {
            lat = this.value;
        }
        else {
            lat = null;
            Massage.innerHTML = '* 纬度无效，请重新输入！';
        }
    };

    //定义定位单击事件
    document.getElementById('btnGPS').onclick = function () {
        var view = map.getView();
        //view.setCenter(ol.proj.transform([parseFloat(lon), parseFloat(lat)], 'EPSG:4326', 'EPSG:3857'));
        if (lat == null || lon == null) {
            Massage.innerHTML = '* 无法定位，请重新输入经纬度！';
        }
        else {
            view.setCenter([parseFloat(lon), parseFloat(lat)]);
            Show("GPS");
            //document.getElementById('GPS').style.display = "none";
        }
    };
};
//添加控件
function AddControl() {
    map.addControl(new ol.control.ZoomSlider());
    //添加缩放到当前视图滑动控件
    //map.addControl(new ol.control.ZoomToExtent());
    //添加全屏控件
    map.addControl(new ol.control.FullScreen());
    map.addControl(new ol.control.MousePosition({
        undefinedHTML: 'outside',
        projection: 'EPSG:4326',
        coordinateFormat: function (coordinate) {
            return ol.coordinate.format(coordinate, '{x}, {y}', 4);
        }
    })
     );
    map.addControl(new ol.control.ScaleLine());
};
//初始化右键菜单
function InitalizeMenu() {
    menu_overlay = new ol.Overlay({
        //map:map,
        element: document.getElementById("contextmenu_container"),
        positioning: 'center-center'
    });
    menu_overlay.setMap(map);
    $(map.getViewport()).on("contextmenu", function (e) {
        e.preventDefault();
        var coordinate = map.getEventCoordinate(e);
        menu_overlay.setPosition(coordinate);
    });
};
//添加标注
function AddLabel() {
    vectorSource = new ol.source.Vector({
    });
    vectorLayer = new ol.layer.Vector({
        source: vectorSource
    });
    map.addLayer(vectorLayer);
    //添加标注单击事件
    document.getElementById('btnAddLabel').onclick = function () {
        var lblname = document.getElementById('lblname').value;
        if (lblname.length > 0) {
            var iconFeature = new ol.Feature({
                geometry: new ol.geom.Point([lon.value, lat.value]),
                name: lblname,
                population: 4000,
                rainfall: 500
            });

            var iconStyle = new ol.style.Style({
                image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
                    anchor: [0.5, 46],
                    anchorXUnits: 'fraction',
                    anchorYUnits: 'pixels',
                    src: '../Image/icon.png',
                }))
            });
            iconFeature.setStyle(iconStyle);
            vectorSource.addFeature(iconFeature);
            Show("Label");
            //document.getElementById('Label').style.display = "none";
        }
        else {
            alert("标注名称不能为空");
        }
    };
};
//绘制网格线
function DrawGrid() {
    var graticuleLayer = new ol.Graticule({
        //map: map,
        strokeStyle: new ol.style.Stroke({
            color: 'rgba(255, 255, 255, 0.8)',
            width: 0.6
        }),
        targetSize: 100
    });
    graticuleLayer.setMap(map);
};
//加载json类型的矢量数据
function AddVector() {
    //点
    var point = new ol.layer.Vector({
        title: 'Earthquakes',
        source: new ol.source.Vector({
            projection: 'EPSG:4326',
            url: 'dataSource/7day.json',
            format: new ol.format.GeoJSON()
        }),
        style: new ol.style.Style({ image: new ol.style.Circle({ radius: 5, fill: new ol.style.Fill({ color: 'red' }) }) })
    });
    //线
    var line = new ol.layer.Vector({
        title: 'Line',
        source: new ol.source.Vector({
            url: 'dataSource/line-samples.geojson',
            format: new ol.format.GeoJSON()
        }),
    });
    //面
    var polygon = new ol.layer.Vector({
        source: new ol.source.Vector({
            url: 'dataSource/countries.geojson',
            format: new ol.format.GeoJSON()
        })
    });
    map.addLayer(polygon);
    map.addLayer(line);
    map.addLayer(point);
};
//图层管理功能模块
function ShowLayers() {
    //alert("Hello World!（功能暂未实现）");
    Show("Layers");
    LoadLayers();
};
function LoadLayers() {

};
//定位功能模块
function ShowGPS() {
    //var coordinate = menu_overlay.getPositioning();
    //menu_overlay.setPosition(undefined);
    //var GPSForm = new ol.Overlay({
    //    element: document.getElementById('GPS'),
    //    positioning: 'absolute',//'bottom-center',
    //    stopEvent: false
    //});
    //GPSForm.setMap(map);
    Show("GPS");
};

function ShowAddLabel() {
    Show("Label");
};

function ShowVectorEdit() {
    Show("VectorEdit");
};

function StartSelect() {
    isOpenForm = !isOpenForm;
    if (isOpenForm) {
        Massage.innerHTML = '开始框选...';
        map.addInteraction(dragBox);
    }
    else {
        map.removeInteraction(dragBox);
        Massage.innerHTML = '';
    }
};

function DelInfoBox() {
    map.removeOverlay(msg);
}

function Show(name) {
    var state = document.getElementById(name).style.display;
    if (state != "block") {
        document.getElementById(name).style.display = "block";

    }
    else {
        document.getElementById(name).style.display = "none";
    }
}