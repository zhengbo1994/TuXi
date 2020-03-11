/// <reference path="../openLayer/ol.js" />
/**
 基于OpenLayer的矢量编辑工具
 */
(function ($) {
    $.fn.mapVectorEditorCtl = function (settings) {
        //集合扩展
        Array.prototype.indexOfEx = function (feature) {
            for (var i = 0; i < this.length; i++) {
                var id1 = this[i].getId();
                var id2 = feature.getId();
                if (id1 == id2) {
                    return i;
                }
            }
            return -1;
        };
        Array.prototype.removeEx = function (feature) {
            try {
                var index = this.indexOfEx(feature);
                if (index > -1) {
                    this.splice(index, 1);
                }
            }
            catch (e) {
            }
        };
        //默认选项
        var defaluts = {
            //地图底图数据源集合
            baseLayerList: null,
            //iTelluro地图服务集合
            iTelluroLayerList: null,
            //矢量数据结合
            vectorDataList: null,
            //矢量图层集合
            vectorLayerList: null,
            //wms地图服务集合
            wmsLayerList: null,
            //wmts地图服务集合
            wmtsLayerList: null,
            //地图控件宽高
            mapHeight: 400,
            mapWidth: 800,
            //边界拐点坐标
            boundWKT: null,
            center: [118.4546, 36.4482],
            zoom: 3,
            extent: null,
            //要素改变事件
            featureChanged: null,
            customHtml: null,
            customZIndex: 10000
        };
        $.extend(defaluts, settings);

        //设置样式
        var me = $(this);
        var id = me.attr("id");
        if (id == null || id == "") {
            id = "mapVectorEditor" + new Date().getTime();
            me.attr("id", id);
        }
        me.height(defaluts.mapHeight);
        me.css({
            width: '100%',
            //height:$(window).height()-me.offset().top
        });
        //地图上的显示模块的Html
        var html = null;
        if (defaluts.customHtml) {
            html = defaluts.customHtml;
        } else {
            html = '<div class="ui-layout" id="map_layout">' +
               '           <a class="sidebar-showon" title="展开"><i class="fa fa-forward" style="color: #ccc;"></i></a>' +
               '           <div class="ui-layout-west">' +
               '               <a class="sidebar-shutup" title="收缩"><i class="fa fa-backward" style="color: #ccc;"></i></a>' +
               '               <div class="panel-Title"><span>图层树信息</span><button class="btn btn-default add-layer" title="添加图层"><i class="fa fa-plus"></i></button></div>' +
               '               <div id="itemTree"></div>' +
               '           </div>' +
               '           <div class="map_container">' +
               '               <div class="mapDiv"></div> ' +
               '               <div class="map_controller_btn" id="draggable">' +
               '                   <div class="drag_bar"><i class="fa fa-bars"></i><i class="fa fa-bars"></i><i class="fa fa-bars"></i><i class="fa fa-bars"></i></div>' +
               '                   <div class="btn_group">' +
               '                       <div class="btn_item map_polygon" style="border-radius: 5px 5px 0 0;">' +
               '                           <span class="map-editor-icon-31" style="width:30px;height:28px;text-align:center;"></span><p>多边形</p>' +
               '                           <span class="fa fa-caret-right" style="font-size:14px;position: absolute;right:3px;bottom:-2px;transform: rotate(45deg);"></span>' +
               '                       </div>' +
               '                       <ul class="btn_item_list">' +
               '                           <li><div class="map_point" style="overflow:hidden;"><span class="map-editor-icon-121"></span><p>点</p></div></li>' +
               '                           <li class="isYuanhua"><div class="map_line" style="overflow:hidden;"><span class="map-editor-icon-9"></span><p>线</p></div></li>' +
               '                           <li class="isYuanhua"><div class="map_polygon" style="overflow:hidden;"><span class="map-editor-icon-31"></span><p>多边形</p></div></li>' +
               '                           <li><div class="map_rect" style="overflow:hidden;"><span class="map-editor-icon-uniE919"></span><p>矩形</p></div></li>' +
               '                           <li><div class="map_circle" style="overflow:hidden;"><span class="fa fa-circle-thin" style="width:30px;height:28px;text-align:center;"></span><p>圆形</p></div></li>' +
               '                       </ul>' +
               '                   </div>' +
               '                   <div class="btn_group">' +
               '                       <div class="btn_item map_boxselect">' +
               '                           <span class="map-editor-icon-11"></span><p>框选</p>' +
               '                           <span class="fa fa-caret-right" style="font-size:14px;position: absolute;right:3px;bottom:-2px;transform: rotate(45deg);"></span>' +
               '                       </div>' +
               '                       <ul class="btn_item_list">' +
               '                           <li><div class="map_boxselect" style="overflow:hidden;"><span class="map-editor-icon-11"></span><p>框选</p></div></li>' +
               '                           <li><div class="map_cut" style="overflow:hidden;"><span class="map-editor-icon-uniE901"></span><p>裁剪</p></div></li>' +
               '                       </ul>' +
               '                   </div>' +
               '                   <div class="btn_group">' +
               '                       <div class="btn_item map_pointset" style="border-radius:0 0 5px 5px;">' +
               '                           <span class="ti-settings"></span><p>设置</p>' +
               //'                           <span class="fa fa-caret-right" style="font-size:14px;position: absolute;right:3px;bottom:-2px;transform: rotate(45deg);"></span>' +
               '                       </div>' +
               '                       <div class="btn_item_list">' +
               '                           <div class="map_setpoint" style="overflow:hidden;">' +
               '                               <p>设置点</p>' +
               '                               <div class="set_item">' +
               '                                   <label for="_longitude">经度</label>' +
               '                                   <input type="text" id="_longitude"/>' +
               '                               </div>' +
               '                               <div class="set_item">' +
               '                                   <label for="_latitude">纬度</label>' +
               '                                   <input type="text" id="_latitude"/>' +
               '                               </div>' +
               '                               <div class="set_item">' +
               '                                   <label for="icon-href">icon路径</label>' +
               '                                   <input type="text" id="icon-href" style="width:126px;"/>' +
               '                               </div>' +
               '                               <div class="set_item">' +
               '                                   <button class="btn btn-primary" id="show_point">显示</button>' +
               '                               </div>' +
               '                           </div>' +
               '                       </div>' +
               '                   </div>' +
               '               </div>' +
               '               <div class=" map_zoom ol-zoom ol-unselectable ol-control"><button data-toggle="tooltip" data-placement="top" title="放大" type="button" class="ol-zoom-in map-zoom-in" id="zoomIn"><i class="ti-plus"/></button><button data-toggle="tooltip" data-placement="right" title="缩小" type="button" class="ol-zoom-out map-zoom-out"><i class="ti-minus"/></button></div>' +
               '           </div>' +
               '       </div>'
        }
        me.html('');
        me.append(html);
        var count = me.find("#popup").length;
        if (count == 0) {
            var html = '<div class="ol-popup none" id="popup" style="display:none;">' + '<div id="popup-content"></div></div>';
            me.append(html);
        }
        html = null;
        if (defaluts.baseLayerList == null) {
            return;
        }

        //地图的全局变量
        var map = null, polyDraw, pointSelect, polyModify, polyDrag, boxSelect, snapTool, snapLayer, rectDraw;
        var tipLayer = null;
        var baseMap = defaluts.baseLayerList[0];
        var baseZJ = defaluts.baseLayerList[1];
        //选中的Feature
        var selectFeatures = [];
        //要素备份;
        var featuresBak = [];
        //图层数据源
        var vectorDrawSource = new ol.source.Vector({
            wrapX: false
        });
        var vectorDrawLayer = new ol.layer.Vector({
            source: vectorDrawSource
        });

        var vectorPointSource = new ol.source.Vector({
            wrapX: false
        });
        var vectorPonintLayer = new ol.layer.Vector({
            source: vectorPointSource
        });

        var rectSource = new ol.source.Vector({
            wrapX: false
        });
        var rectLayer = new ol.layer.Vector({
            source: rectSource,
            name: "rectLayer",
            style: new ol.style.Style({
                fill: new ol.style.Fill({
                    color: 'rgba(252, 216, 107, 0.35)'
                }),
                stroke: new ol.style.Stroke({
                    color: '#ffcc33',
                    width: 2
                }),
                image: new ol.style.Circle({
                    radius: 7,
                    fill: new ol.style.Fill({
                        color: '#ffcc33'
                    })
                })
            })
        });

        //其他变量
        var isSmooth = true, isEdit = false;
        var toolType = "Polygon";
        var lineWidth = 3;
        var breakSelect = false;

        //获取地图控件自定义的高度值
        var mapheight = defaluts.mapHeight;
        //设置地图容器
        var mapDiv = me.find("div.mapDiv");

        //动态的设置地图的高度
        mapDiv.css({
            height: $(window).height() - 60
        });

        //生成图层树节点
        var treeData = [];
        var dtNode = {
            id: 'baseLayer',
            text: '底图',
            value: 'baseLayer',
            parentnodes: '0',
            checkstate: 1,
            showcheck: true,
            isexpand: false,
            complete: true,
            hasChildren: false,
            ChildNodes: []
        };
        treeData.push(dtNode);
        var bzNode = {
            id: 'zjLayer',
            text: '底图注记',
            value: 'zjLayer',
            parentnodes: '0',
            checkstate: 1,
            showcheck: true,
            isexpand: false,
            complete: true,
            hasChildren: false,
            ChildNodes: []
        };
        treeData.push(bzNode);
        if (defaluts.boundWKT && defaluts.boundWKT.length > 0) {
            var cutNode = {
                id: 'cutLayer',
                text: '剪切范围',
                value: 'cutLayer',
                parentnodes: '0',
                checkstate: 1,
                showcheck: true,
                isexpand: false,
                complete: true,
                hasChildren: false,
                ChildNodes: []
            };
            treeData.push(cutNode);
        }
        if (defaluts.iTelluroLayerList && defaluts.iTelluroLayerList.length > 0) {
            for (var i = 0; i < defaluts.iTelluroLayerList.length; i++) {
                var node = {
                    id: uuid(),
                    text: defaluts.iTelluroLayerList[i].dataserverkey,
                    value: defaluts.iTelluroLayerList[i].dataserverkey,
                    parentnodes: '0',
                    checkstate: 1,
                    showcheck: true,
                    isexpand: false,
                    complete: true,
                    hasChildren: false,
                    ChildNodes: []
                };
                treeData.push(node);
            }
        }
        if (defaluts.wmsLayerList && defaluts.wmsLayerList.length > 0) {
            for (var i = 0; i < defaluts.wmsLayerList.length; i++) {
                var nodeWMS = {
                    id: uuid(),
                    text: defaluts.wmsLayerList[i].layerName,
                    value: defaluts.wmsLayerList[i].layerName,
                    parentnodes: '0',
                    checkstate: 1,
                    showcheck: true,
                    isexpand: false,
                    complete: true,
                    hasChildren: false,
                    ChildNodes: []
                };
                treeData.push(nodeWMS);
            }
        }
        if (defaluts.wmtsLayerList && defaluts.wmtsLayerList.length > 0) {
            for (var i = 0; i < defaluts.wmtsLayerList.length; i++) {
                var nodeWMTS = {
                    id: uuid(),
                    text: defaluts.wmtsLayerList[i].layerName,
                    value: defaluts.wmtsLayerList[i].layerName,
                    parentnodes: '0',
                    checkstate: 1,
                    showcheck: true,
                    isexpand: false,
                    complete: true,
                    hasChildren: false,
                    ChildNodes: []
                };
                treeData.push(nodeWMTS);
            }
        }
        if (defaluts.vectorLayerList && defaluts.vectorLayerList.length > 0) {
            for (var i = 0; i < defaluts.vectorLayerList.length; i++) {
                var nodeV = {
                    id: uuid(),
                    text: defaluts.vectorLayerList[i].layerName,
                    value: defaluts.vectorLayerList[i].layerName,
                    parentnodes: '0',
                    checkstate: 1,
                    showcheck: true,
                    isexpand: false,
                    complete: true,
                    hasChildren: false,
                    ChildNodes: []
                };
                treeData.push(nodeV);
            }
        }

        //加载图层树
        function GetTree() {
            var item = {
                height: defaluts.mapHeight - $("#itemTree").offset().top,
                showcheck: true,
                data: treeData,
                oncheckboxclick: function (item, status) {
                    var layerName = item.value;
                    var maplayers = map.getLayers().getArray();
                    maplayers.forEach(function (layer) {
                        if (layer.get("name") == layerName) {
                            if (status == 0) {
                                layer.setVisible(false);
                            } else {
                                layer.setVisible(true);
                            }
                        }
                    });
                }
            };
            //初始化
            $("#itemTree").treeview(item);
        };
        GetTree();
        //图层树显示隐藏
        var treeWidth = $("#map_layout").find(".ui-layout-west").width();
        $("#map_layout").find(".sidebar-shutup").click(function () {
            $("#map_layout").find(".ui-layout-west").animate({ marginLeft: -(treeWidth + treeWidth * 0.1) }, 300);
            $("#map_layout").find(".sidebar-showon").show();
            mapDiv.width(me.width());
            map.updateSize();
        });
        $("#map_layout").find(".sidebar-showon").click(function () {
            $("#map_layout").find(".ui-layout-west").animate({ marginLeft: 0 }, 300);
            $("#map_layout").find(".ui-layout-west").width(me.width() * 0.1);
            $("#map_layout").find(".sidebar-showon").hide();
            setTimeout(function () {
                mapDiv.width(me.width() * 0.9);
                map.updateSize();
            }, 300);
        });
        //添加图层弹窗
        $("#map_layout").find(".add-layer").click(function () {
            var _tab = '<div class="tabWraper">' +
                            '<ul class="nav nav-tabs" style="border-bottom: 1px solid #ddd;">' +
                                '<li><input type="radio" value="1" name="layer" checked=checked /><label>iTelluro服务<label/></li>' +
                                '<li><input type="radio" value="2" name="layer" /><label>GeoJSON矢量数据<label/></li>' +
                                '<li><input type="radio" value="3" name="layer" /><label>WMS服务<label/></li>' +
                            '</ul>' +
                       '</div>' +
                       '<div class="tab-content" style="padding:10px;"></div>';
            var tabContent1 = '<div class="tab-item"><label for="key-name"><i>服</i><i>务</i><i>key</i><i>名</i><i>称：</i></label><input type="text" id="key-name"/></div>' +
                              '<div class="tab-item"><label for="key-url"><i>服</i><i>务</i><i>地</i><i>址：</i></label><input type="text" id="key-url"/></div>' +
                              '<div class="tab-item"><label for="zero-grade"><i>零</i><i>级</i><i>大</i><i>小：</i></label><input type="number" id="zero-level-size"/></div>' +
                              '<div class="tab-item"><label for="key-url"><i>图</i><i>层</i><i>透</i><i>明</i><i>度：</i></label><input type="number" id="layer-opacity" value="1.0"/></div>';
            var tabContent2 = '<div class="tab-item"><label class="geo" for="key-name"><i>名</i><i>称：</i></label><input type="text" id="key-name"/></div>' +
                              '<div class="tab-item"><label class="geo" for="key-url"><i>路</i><i>径：</i></label><input type="text" id="key-url"/></div>';
            layer.open({
                type: 1,
                skin: 'layui-layer-rim',
                area: ['345px', '350px'],
                btn: ['确定', '取消'],
                title: '添加图层',
                content: _tab,
                success: function (layero, index) {//弹出回调，切换选项
                    var _radio = layero.find(".nav-tabs li").find("input[type='radio']");
                    var tabContent = layero.find(".tab-content");
                    showCurrentTab();
                    _radio.click(function () {
                        tabContent.html("");
                        showCurrentTab();
                    });
                    function showCurrentTab() {//单选切换相应表单的函数
                        for (var i = 0; i < _radio.length; i++) {
                            var radioState = $(_radio[i]).prop("checked");
                            var radioValue = $(_radio[i]).attr("value");
                            if (radioState) {
                                if (radioValue == 1) {
                                    tabContent.append(tabContent1);
                                } else if (radioValue == 2) {
                                    tabContent.append(tabContent2);
                                } else if (radioValue == 3) {
                                    console.log("待开发");
                                };
                            };
                        };
                    };
                },
                yes: function (index, layero) {//完成后回调，提交
                    var keyName = layero.find("#key-name").val();
                    var keyUrl = layero.find("#key-url").val();
                    var zeroLevelSize = layero.find("#zero-level-size").val();
                    var layerOpacity = layero.find("#layer-opacity").val();
                    var _ID = uuid();
                    var newNode = {
                        id: _ID,
                        text: keyName,
                        value: keyName,
                        parentnodes: '0',
                        checkstate: 1,
                        showcheck: true,
                        isexpand: false,
                        complete: true,
                        hasChildren: false,
                        ChildNodes: []
                    };
                    function creatNode() {
                        var _input = layero.find(".tab-content").find("input");
                        for (var i = 0; i < _input.length; i++) {//验证是否有未填项
                            var _val = $(_input[i]).val();
                            if (!_val) {
                                $(_input[i]).focus().css("outline-color", "red");
                                return;
                            } else {
                                $(_input[i]).css("outline-color", "");
                            };
                        };
                        for (var j = 0; j < treeData.length; j++) {
                            if (keyName === treeData[j].text) {//验证是否重名
                                $(_input[0]).focus().css("outline-color", "red").val("命名重复，请重设图层名称！");
                                return;
                            } else {
                                $(_input[0]).css("outline-color", "");
                            };
                        };
                        treeData.push(newNode);
                        GetTree();
                        layer.close(index);
                    };
                    if (layerOpacity < 0) {
                        layerOpacity = 0;
                    } else if (layerOpacity > 1) {
                        layerOpacity = 1;
                    };
                    var currentRadioValue = layero.find(".nav-tabs li").find("input[type='radio']:checked").attr("value");
                    if (currentRadioValue == 1) {//iTelluro服务
                        creatNode();//生成左侧图层树节点
                        var layers = [{ dataserverkey: keyName, url: keyUrl, tilesize: 512, zerolevelsize: zeroLevelSize, opacity: layerOpacity }];
                        addiTelluroLayer(layers);
                    } else if (currentRadioValue == 2) {//GeoJSON矢量数据
                        creatNode();//生成左侧图层树节点
                        var layers = [{ layerName: keyName, geoJsonUrl: keyUrl }];
                        addVectorLayer(layers);
                    } else if (currentRadioValue == 3) {
                        console.log("待开发");
                    };
                }
            });
        });

        /** 按钮事件开始**/

        function drawingType(type) {
            toolType = type;
            $(map.getTargetElement()).find("canvas").css("cursor", "url(ico/16s.ico),auto");
            drawShapes();
        }
        //鼠标tip
        var mouseTip = $('<div class="mouse-tip" style="border:solid 1px red;display:none;position:absolute;padding:0 2px;background:#fff;max-height:19px;overflow:hidden;"></div>');
        var successTip = $('<div style="display:none;position:absolute;cursor:pointer;"><span class="map_editend" data-toggle="tooltip" data-placement="bottom" title="结束画图"><span class="map-editor-icon-12"><span class="path1"></span><span class="path2"></span></span></span></div>');
        //开始画形状时的事件函数
        function drawStart(polyDraw, drawstart, mouseTip, toolType, tipContent, successTip) {
            polyDraw.on(drawstart, function (event) {
                if (toolType == "LineString") {
                    tipContent = "单击确定下一个点位置，双击完成";
                } else if (toolType == "Polygon") {
                    tipContent = "单击确定至少三个点，双击完成或移到开始点单击完成";
                } else if (toolType == "Box") {
                    tipContent = "拖拽确定长宽，单击完成";
                } else if (toolType == "Circle") {
                    tipContent = "拖拽确定半径，单击完成画图";
                }
                mouseTip.html(tipContent);//添加tip提示内容
                mouseTip.show();//显示提示框
                successTip.hide();//开始画时隐藏结束画图按钮
                mapDiv.unbind('click');//解除获取鼠标位置的点击事件
                me.find(".edit-box>.map_move").removeClass("click").find("i").css("color", "#ed625e");
            });
        };
        //画形状
        function drawShapes() {
            removeInteraction();
            clearPointLayer();
            reSet();
            mapContainer.append(mouseTip);
            mapContainer.append(successTip);
            var tipContent = null;
            //鼠标移动时确定tip位置
            mapDiv.bind("mousemove", function (e) {
                e = e || e.originalEvent;
                var top = e.clientY - $(e.target).offset().top + 20;
                var left = e.clientX - $(e.target).offset().left + 20;
                mouseTip.css({ "top": top, "left": left });
            });

            if (toolType == "Box") {
                var geoFun = ol.interaction.Draw.createBox();
                polyDraw = new ol.interaction.Draw({
                    source: vectorDrawSource,
                    type: "Circle",
                    geometryFunction: geoFun
                });
            } else {
                polyDraw = new ol.interaction.Draw({
                    source: vectorDrawSource,
                    type: toolType
                });
            };
            if (toolType == "Circle") {
                polyDraw.on('drawstart', function (event) {
                    var feat = event.feature;
                    var geometry = feat.getGeometry();
                    var listener = geometry.on('change', function (ev) {
                        var radius = geometry.getRadius();
                        //var sourceProj = map.getView().getProjection();
                        var center = geometry.getCenter();
                        var last = geometry.getLastCoordinate();
                        //center = ol.proj.transform(center, sourceProj, 'EPSG:4326');
                        //last = ol.proj.transform(last, sourceProj, 'EPSG:4326');
                        //var length = wgs84Sphere.haversineDistance(center, last);
                        var length = ol.sphere.getDistance(center, last, 6378137);
                        //length = wgs84Sphere.haversineDistance([108.8857,31.9726], [119.2216,40.9032]);
                        if (length > 100) {
                            length = (Math.round(length / 1000 * 100) / 100) + 'km';
                        } else {
                            length = (Math.round(length * 100) / 100) + 'm';
                        }
                        //var line = new ol.geom.LineString();
                        //line.setCoordinates([center, last]);

                        //var lineLen = line.getLength(line);
                        var popup_element = me.find("#popup")[0];
                        var popup_content = me.find("#popup-content")[0];
                        popup_element.style.display = 'block';
                        popup_content.innerHTML = length;
                        if (!tipLayer) {
                            tipLayer = new ol.Overlay(({
                                element: popup_element,
                                autoPan: true,
                                autoPanAnimation: {
                                    duration: 250
                                },
                                id: "tipLayer"
                            }));
                            map.addOverlay(tipLayer);
                        }
                        tipLayer.setPosition(center);
                    });
                });
            }
            //开始画形状时
            drawStart(polyDraw, 'drawstart', mouseTip, toolType, tipContent, successTip);
            //形状画完时
            polyDraw.on('drawend', function (event) {
                //设置圆滑默认样式
                var feat = event.feature;
                addFeature(feat);
                if (tipLayer) {
                    tipLayer.setPosition(null);
                };
                mouseTip.hide();
                successTip.show();
                me.find("[data-toggle='tooltip']").tooltip();

                //设置结束画图按钮位置
                mapDiv.bind('click', function (e) {
                    var top = e.clientY - $(e.target).offset().top;
                    var left = e.clientX - $(e.target).offset().left + 30;
                    var _top = (top + 40) > $(e.target).height() ? top - 40 : top;
                    var _left = (left + 40) > $(e.target).width() ? left - 70 : left;
                    successTip.css({ "top": _top, "left": _left });
                });
                //结束确认
                editorEnd();
            });
            map.addInteraction(polyDraw);
            addSnapTool();
            if (defaluts.featureChanged) {
                defaluts.featureChanged([feat], toolType, "Add");
            }
        };

        /** 选择要素**/
        function mapSelect() {
            breakSelect = false;
            removeInteraction();
            pointSelect = new ol.interaction.Select({
                layers: [vectorDrawLayer],
                toggleCondition: ol.events.condition.click
            });
            pointSelect.on('select', function (event) {
                if (breakSelect) {
                    return;
                }
                //设置选中
                selectFeatures = null;
                selectFeatures = event.selected;
                selectFeature();
                map.render();
            });
            map.addInteraction(pointSelect);
            map.render();
        };

        /** 框选 **/
        function mapBoxSelect() {
            successTip.hide();
            $(map.getTargetElement()).find("canvas").css("cursor", "crosshair");
            removeInteraction();
            boxSelect = new ol.interaction.DragBox({
            });
            boxSelect.on('boxend', function (event) {
                //设置选中
                selectFeatures = null;
                selectFeatures = [];
                var extent = boxSelect.getGeometry().getExtent();
                vectorDrawSource.forEachFeatureIntersectingExtent(extent, function (feature) {
                    selectFeatures.push(feature);
                });
                selectFeature();
                map.render();
                $(map.getTargetElement()).find("canvas").css("cursor", "");
                //弹框
                var layerTop = setLayerPosition(event)[0];
                var layerLeft = setLayerPosition(event)[1];
                afterMapBoxSelect(layerTop, layerLeft, selectFeatures);
            });
            map.addInteraction(boxSelect);
            map.render();
        };

        /*编辑形状*/
        function editShape() {
            removeInteraction();
            if (!selectFeatures || selectFeatures.length <= 0) {
                return;
            }
            isEdit = true;
            polyModify = new ol.interaction.Modify({
                features: new ol.Collection(selectFeatures)
            });
            polyModify.on('modifystart', function (event) {
                console.log(event);
            });
            polyModify.on('modifyend', function (event) {
                console.log(event);
                //画点图层
                var features = event.features.getArray();
                var isClear = true;
                if (tipLayer) {
                    tipLayer.setPosition(null);
                }
                for (var i = 0; i < features.length; i++) {
                    if (i != 0) {
                        isClear = false;
                    }
                    var geometry = features[i].getGeometry();
                    var coords = null;
                    try {
                        coords = geometry.getCoordinates();
                    } catch (e) {

                    }
                    var geoType = geometry.getType();
                    if (geoType == "Polygon") {
                        addPointToPointLayer(coords[0], isClear);
                    } else if (geoType == "LineString") {
                        addPointToPointLayer(coords, isClear);
                    } else if (geoType == "Point") {
                        addPointToPointLayer([[coords[0], coords[1]]], isClear);
                    } else if (geoType == "Circle" && geometry instanceof ol.geom.Circle) {
                        var center = geometry.getCenter();
                        addPointToPointLayer([center], isClear);
                    }
                };
                successTip.show();
            });
            map.addInteraction(polyModify);
            addSnapTool();
        };

        /** 结束编辑多边形**/
        function editorEnd() {
            var map_editend = me.find(".map_editend");
            map_editend.click(function () {
                isEdit = false;
                removeInteraction();
                clearPointLayer();
                //画点图层
                if (selectFeatures && selectFeatures.length > 0) {
                    for (var i = 0; i < selectFeatures.length; i++) {
                        var geometry = selectFeatures[i].getGeometry();
                        var coords = null;
                        try {
                            coords = geometry.getCoordinates();
                        } catch (e) {

                        }
                        var geoType = geometry.getType();
                        var smoothened = [];
                        if (geoType == "Polygon") {
                            smoothened = getCurvePoints(convertToSmoothPoint(coords[0]), 0.5, 20, false);//makeSmooth(coords[0], 3);
                            geometry.setCoordinates([convertToGeometryPoint(smoothened)]);
                        } else if (geoType == "LineString") {
                            smoothened = getCurvePoints(convertToSmoothPoint(coords), 0.5, 20, false);
                            geometry.setCoordinates(convertToGeometryPoint(smoothened));
                        }
                    }
                }
                if (defaluts.featureChanged) {
                    defaluts.featureChanged(selectFeatures, toolType, "Edit");
                }
                selectFeatures = null;
                reSetFromBak();
                successTip.hide();//隐藏结束画图按钮
                $(map.getTargetElement()).find("canvas").css("cursor", "");//恢复鼠标样式

                learun.dialogAlert({ msg: "页面要素绘制完毕，可修改要素进行保存！" });
            });
        };

        /** 删除feature**/
        function delFeature() {
            removeInteraction();
            clearPointLayer();
            if (!selectFeatures || selectFeatures.length <= 0) {
                return;
            }

            for (var i = 0; i < selectFeatures.length; i++) {
                vectorDrawSource.removeFeature(selectFeatures[i]);
                featuresBak.removeEx(selectFeatures[i]);
            }
            if (defaluts.featureChanged) {
                defaluts.featureChanged(selectFeatures, toolType, "Remove");
            }
            selectFeatures = null;
        }

        /** 移动feature**/
        function mapMove() {
            removeInteraction();
            clearPointLayer();
            selectFeatures = null;
            reSetFromBak();
            if (!polyDrag) {
                polyDrag = new vectorDrag.Drag();
            }
            map.addInteraction(polyDrag);
        };

        /** 剪切多边形**/
        function mapCut() {
            removeInteraction();
            clearPointLayer();
            selectFeatures = null;
            reSetFromBak();
            if (!defaluts.boundWKT || defaluts.boundWKT.length <= 0 || featuresBak.length <= 0) {
                return;
            }
            var wktJSTSReader = new jsts.io.WKTReader();
            var parser = new jsts.io.OL3Parser();
            var wktOLReader = new ol.format.WKT();
            var tmpFeatures = featuresBak;
            featuresBak = [];
            vectorDrawSource.clear();
            for (var j = 0; j < defaluts.boundWKT.length; j++) {
                var bound = wktJSTSReader.read(defaluts.boundWKT[j]);
                for (var i = 0; i < tmpFeatures.length; i++) {
                    var tmpCircle = null;
                    if (tmpFeatures[i].getGeometry() instanceof ol.geom.Circle) {
                        tmpCircle = tmpFeatures[i];
                        tmpFeatures[i] = createCircle(tmpFeatures[i]);
                    }
                    var featureWKT = wktOLReader.writeFeature(tmpFeatures[i]);
                    var featuretmmp = wktJSTSReader.read(featureWKT);
                    var isV = featuretmmp.isValid();
                    if (!isV) {
                        featuresBak.push(tmpFeatures[i]);
                        vectorDrawSource.addFeature(tmpFeatures[i]);
                        continue;
                    }
                    var cutFTS = bound.intersection(featuretmmp);
                    if (!cutFTS.isEmpty()) {
                        if (bound.contains(featuretmmp)) {
                            if (tmpCircle) {
                                featuresBak.push(tmpCircle);
                                vectorDrawSource.addFeature(tmpCircle);
                            } else {
                                featuresBak.push(tmpFeatures[i]);
                                vectorDrawSource.addFeature(tmpFeatures[i]);
                            }
                        }
                        else {
                            var num = cutFTS.getNumGeometries();
                            for (var k = 0; k < num; k++) {
                                var geo = cutFTS.getGeometryN(k);
                                var geoType = geo.getGeometryType();
                                if (geoType == 'Polygon' || geoType == "MultiPolygon" || geoType == "LineString" || geoType == "Point") {
                                    var feat = new ol.Feature({
                                        geometry: parser.write(geo)
                                    });
                                    feat.setStyle(tmpFeatures[i].getStyle());
                                    var id = uuid();
                                    feat.setId(id);
                                    featuresBak.push(feat);
                                    vectorDrawSource.addFeature(feat);
                                }
                            }
                        }
                    }
                    console.log(cutFTS);
                }
            }
        };

        //框选后弹框
        function afterMapBoxSelect(layerTop, layerLeft, selectFeatures) {
            if (_index) {
                layer.close(_index);//关闭前一个弹框
            };
            var featuresName = [];
            selectFeatures.forEach(function (feature, index) {
                var GeoType = feature.getGeometry().getType();
                if (GeoType == "Point") {
                    featuresName.push("点" + index);
                } else if (GeoType == "LineString") {
                    featuresName.push("线" + index);
                } else if (GeoType == "Polygon" || GeoType == "Circle" || GeoType == "Box") {
                    if (GeoType == "Circle") {
                        featuresName.push("圆" + index);
                    } else {
                        var Coordinates = feature.getGeometry().getCoordinates();
                        var FeatureName = Coordinates[0].length == 5 ? "矩形" : "多边形";
                        featuresName.push(FeatureName + index);
                    };
                };
            });
            var _html = '<ul class="feature_edit">' +
                            '<li><label for"feature_name">图形</label><input type="text" disabled="disabled" id="feature_name" /></li>' +
                            '<li><button class="btn btn-default btn-edit" id="edit_shape">编辑形状</button></li>' +
                            '<li  class="b_title">外观</li>' +
                            '<li class="fill-wrap">' +
                                '<div class="item_wrap fill-color"><label for"inner_color">填充</label><div class="color-fix" style="background: rgba(255, 255, 0, 0.8);"></div><div class="color-fix-x" style="display:none;"></div></div>' +
                                '<div class="item_wrap"><label for"inner_none" style="text-align:right;padding-right:20%;">无填充</label><input type="checkbox" id="is_fill" /></div>' +
                            '</li>' +
                            '<li>' +
                                '<div class="item_wrap side-color"><label for"border_color">描边</label><div class="color-fix" style="background:rgba(0,0,255,1.0)"></div></div>' +
                                '<div class="item_wrap"><input type="number" value="1" min="0" id="border_width" class="hover" style="width:100%;text-indent:5px;" /></div>' +
                            '</li>' +
                            '<li>' +
                                '<label for"opacity_num">透明度</label>' +
                                '<input type="range" value=100 min=0 max=100 step=10 id="opacity_range" />' +
                                '<input type="number" value=1 min=0 max=1 id="opacity_num" class="hover" style="text-center;" />' +
                            '</li>' +
                        '</ul>';
            layer.open({
                type: 1,
                skin: 'feature-style-edit',
                area: ['250px', 'auto'],
                btn: ['保存'],
                title: ['属性', 'text-align:left;'],
                shade: 0,
                offset: [layerTop, layerLeft],
                content: _html,
                success: function (layero, index) {
                    _index = index;
                    if (!selectFeatures || selectFeatures.length == 0) {
                        layer.close(index);
                    };
                    layero.find("#feature_name").val(featuresName.toString());
                    /** 初始化颜色框**/
                    colorBoxInit(layero);
                    /*透明度滑轨样式*/
                    setThumbStyle(layero);
                    /*有无填充*/
                    isFillingColor(layero)
                    /*编辑形状*/
                    layero.find("#edit_shape").click(function () {
                        editShape();
                        layer.close(index);
                    });
                },
                yes: function (index, layero) {
                    var isFillNone = layero.find("#is_fill").prop("checked");
                    var fillColor = layero.find(".fill-color>.color-fix").css("background-color");
                    var lineColor = layero.find(".side-color>.color-fix").css("background-color");
                    var lineWidth = parseInt(layero.find("#border_width").val()) < 0 ? 0 : parseInt(layero.find("#border_width").val());
                    var fillOpacity = parseFloat(layero.find("#opacity_num").val());
                    var _fillColor = fillColor.split(",");
                    if (!isFillNone) {
                        fillColor = 'rgba(' + parseInt(_fillColor[0].split("(")[1]) + ',' + parseInt(_fillColor[1]) + ',' + parseInt(_fillColor[2]) + ',' + fillOpacity + ')';
                    } else {
                        fillColor = 'rgba(' + parseInt(_fillColor[0].split("(")[1]) + ',' + parseInt(_fillColor[1]) + ',' + parseInt(_fillColor[2]) + ', 0)';
                    };
                    var Style = null;
                    selectFeatures.forEach(function (feature, index) {
                        var GeoType = feature.getGeometry().getType();
                        if (GeoType == "Point") {
                            Style = new ol.style.Style({
                                fill: new ol.style.Fill({
                                    color: 'rgba(255,255,0,0.8)'
                                }),
                                stroke: new ol.style.Stroke({
                                    color: 'rgba(0,0,255,1.0)',
                                    width: 1
                                }),
                                image: new ol.style.Circle({
                                    radius: 3,
                                    stroke: new ol.style.Stroke({
                                        color: lineColor,
                                        width: lineWidth
                                    }),
                                    fill: new ol.style.Fill({
                                        color: fillColor
                                    })
                                })
                            });
                        } else if (GeoType == "LineString") {
                            Style = new ol.style.Style({
                                stroke: new ol.style.Stroke({
                                    color: lineColor,
                                    width: lineWidth
                                })
                            });
                        } else {
                            Style = new ol.style.Style({
                                fill: new ol.style.Fill({
                                    color: fillColor
                                }),
                                stroke: new ol.style.Stroke({
                                    color: lineColor,
                                    width: lineWidth
                                })
                            });
                        };
                        feature.setStyle(Style);
                    });
                    layer.close(index);
                    removeColpick();
                    clearPointLayer();
                },
                cancel: function () {
                    removeColpick();
                    removeInteraction();
                    clearPointLayer();
                }
            });
        };

        //取色框销毁函数
        function removeColpick() {
            var _colpick = $(".colpick.colpick_full");
            for (var i = 0; i < _colpick.length; i++) {
                $(_colpick[i]).remove();
            };
        };

        /** 圆滑事件 **/
        var YH_box = '<div class="yh_wrap" style="position:absolute;"><div class="yh_inner"><input type="checkbox" id="set_YH" checked="checked" /><label for="set_YH">拐角圆滑</label></div></div>';
        var mapContainer = me.find(".map_container");
        mapContainer.append(YH_box);
        var set_YH = me.find("#set_YH");
        set_YH.click(function () {
            if (this.checked) {
                isSmooth = true;
            } else {
                isSmooth = false;
            }
            //console.log(isSmooth);
        });

        /** 设置点 **/
        me.find("#show_point").click(function () {
            var id = uuid();
            var longitude = me.find("#_longitude").val();
            var latitude = me.find("#_latitude").val();
            var pointIcon = me.find("#icon-href").val();
            showPoint(id, longitude, latitude, pointIcon);
        });

        /** 设置框显示按钮点击事件 **/
        function showPoint(id, longitude, latitude, pointIcon) {
            var stylePoint = null;
            //导航到该点
            if (longitude && latitude) {
                map.getView().animate({ center: [longitude, latitude], duration: 1500 });
            } else {
                return false;
            };
            //设置样式
            if (pointIcon) {
                stylePonit = new ol.style.Style({
                    fill: new ol.style.Fill({
                        color: 'rgba(255,255,0,0.8)'
                    }),
                    stroke: new ol.style.Stroke({
                        color: 'rgba(0,0,255,1.0)',
                        width: 1
                    }),
                    image: new ol.style.Icon({
                        src: pointIcon,
                        size: [30, 30],
                        scale: 0.8
                    })
                });
            } else {
                stylePonit = new ol.style.Style({
                    fill: new ol.style.Fill({
                        color: 'rgba(255,255,0,0.8)'
                    }),
                    stroke: new ol.style.Stroke({
                        color: 'rgba(0,0,255,1.0)',
                        width: 1
                    }),
                    image: new ol.style.Circle({
                        radius: 3,
                        stroke: new ol.style.Stroke({
                            color: 'rgba(0, 0, 0, 0.8)'
                        }),
                        fill: new ol.style.Fill({
                            color: 'rgba(255, 255, 0, 0.8)'
                        })
                    })
                });
            };
            var featurePonit = new ol.Feature();
            var geometryPonit = new ol.geom.Point([longitude, latitude]);
            var _id = id ? id : uuid();
            featurePonit.setId(_id);
            // geometryPonit.setCoordinates([longitude, latitude]);
            featurePonit.setGeometry(geometryPonit);
            featurePonit.setStyle(stylePonit);
            featuresBak.push(featurePonit);
            vectorDrawSource.addFeature(featurePonit);
        };
        /** 按钮事件结束**/

        /** 初始化提示**/
        me.find("[data-toggle='tooltip']").tooltip();

        /** 地图上的控件点击切换样式 **/
        var key = false, firstTime = 0, lastTime = 0, _t = null, _T = null;
        $(window).resize(function () {
            resetMapBtnPosition();
        });
        function resetMapBtnPosition() {
            me.find(".map_controller_btn").css("left", mapDiv.width() - 80);
        };
        me.find("#draggable").draggable({ handle: "div.drag_bar" });
        me.find(".map_controller_btn .drag_bar").mousedown(function () {
            var btnItemList = me.find(".map_controller_btn .btn_group .btn_item_list");
            if (me.find(".yh_wrap").is(":visible")) {
                me.find(".yh_wrap").hide();
            }
            if (btnItemList.is(":visible")) {
                btnItemList.hide();
            }
        });
        me.find(".map_controller_btn .btn_group").mouseenter(function () {
            clearTimeout(_t);
            clearTimeout(_T);
            $(this).find(".btn_item").addClass("active").find("span").css("color", "#fff");
            $(this).siblings().find(".btn_item").removeClass("active").find("span").css("color", "");
            $(this).find(".btn_item_list").show();
            $(this).find(".btn_item_list").css("left", -($(this).find(".btn_item_list").width() + 20));
            $(this).siblings().find(".btn_item_list").hide().find("li").removeClass("active").find("span").css("color", "");
            var yhBoxTop = $(this).find(".btn_item").offset().top - mapContainer.offset().top + 13.5;
            var yhBoxLeft = $(this).find(".btn_item").offset().left - mapContainer.offset().left - 100;
            if ($(this).find(".btn_item").hasClass("map_line") || $(this).find(".btn_item").hasClass("map_polygon")) {
                me.find(".yh_wrap").show().css({ "top": yhBoxTop, "left": yhBoxLeft });
            } else {
                me.find(".yh_wrap").hide();
            };
            if ($(this).find(".btn_item_list").is(":visible")) {
                me.find(".yh_wrap").hide();
            };
        });
        me.find(".map_controller_btn .btn_group").mouseleave(function () {
            var btnItem = $(this).find(".btn_item");
            var btnItemList = $(this).find(".btn_item_list");
            if (btnItem.hasClass("active")) {
                _T = setTimeout(function () {
                    me.find(".yh_wrap").hide();
                    btnItem.removeClass("active").find("span").css("color", "");
                }, 3000);
            };
            if (btnItemList.is(":visible")) {
                _t = setTimeout(function () {
                    btnItemList.hide();
                    btnItemList.find("li").removeClass("active").find("span").css("color", "");
                }, 3000);
            };
        });

        function accordingTypeToDraw(note) {
            if (note.hasClass("map_point")) {
                drawingType("Point");
                return;
            } else if (note.hasClass("map_line")) {
                drawingType("LineString");
                return;
            } else if (note.hasClass("map_polygon")) {
                drawingType("Polygon");
                return;
            } else if (note.hasClass("map_rect")) {
                drawingType("Box");
                return;
            } else if (note.hasClass("map_circle")) {
                drawingType("Circle");
                return;
            } else if (note.hasClass("map_boxselect")) {
                mapBoxSelect();
                return;
            } else if (note.hasClass("map_cut")) {
                mapCut();
                return;
            };
        }
        var mapEditorBtn = me.find(".map_controller_btn .btn_group").find(".btn_item_list li");
        mapEditorBtn.mouseenter(function () {
            $(this).addClass("active").find("span").css("color", "#fff");
            $(this).siblings().removeClass("active").find("span").css("color", "");
            if ($(this).hasClass("isYuanhua")) {
                var yhBoxTop = $(this).offset().top - mapContainer.offset().top + 4.5;
                var yhBoxLeft = $(this).offset().left - mapContainer.offset().left - 100;
                me.find(".yh_wrap").show().css({ "top": yhBoxTop, "left": yhBoxLeft });
            } else {
                me.find(".yh_wrap").hide();
            };
        });

        mapEditorBtn.click(function () {
            successTip.hide();
            var _btnItem = $(this).parent().siblings();
            var _eventClassName = $(this).children().attr("class");
            var _fontIconClassName = $(this).find("span").attr("class");
            var _btnName = $(this).find("p").text();
            _btnItem.removeAttr("class").attr("class", "btn_item").addClass("active").addClass(_eventClassName);
            _btnItem.find("span:first-child").attr("class", _fontIconClassName);
            _btnItem.find("p").text(_btnName);
            $(this).parent().hide();
            me.find(".yh_wrap").hide();
            accordingTypeToDraw($(this).children());
        });

        /** 初始化加载地图  **/
        (function loadMap() {
            if (map != undefined) {
                var layers = map.getLayers.getArray();
                layers.forEach(function (e) {
                    map.removeLayer(e);
                });
            };
            var zjLayer = "";
            //底图
            map = NewWebTilesMap(mapDiv[0], baseMap.zoomlevel, 180, -180, -90, 90, baseMap.dataserverkey, baseMap.url, baseMap.tilesize, baseMap.zerolevelsize, 'baseLayer', defaluts.extent);
            //加注记
            zjLayer = new iTelluro().newItelluroLayer(baseZJ.dataserverkey, baseZJ.url, baseZJ.tilesize, baseZJ.zerolevelsize, 'zjLayer', defaluts.extent);
            zjLayer.setZIndex(2);
            map.addLayer(zjLayer);
            vectorDrawLayer.setZIndex(defaluts.customZIndex + 100);
            map.addLayer(vectorDrawLayer);
            vectorPonintLayer.setZIndex(defaluts.customZIndex + 101);
            map.addLayer(vectorPonintLayer);
            rectLayer.setZIndex(defaluts.customZIndex + 500);
            map.addLayer(rectLayer);
            initMapControl();
            //加载自定义数据
            addDrawDataFeatrues(defaluts.vectorDataList, true);
            //加载自定义图层
            addiTelluroLayer(defaluts.iTelluroLayerList);
            //加载边界剪切坐标
            addCutBoundsToLayer();
            //加载矢量图层
            addVectorLayer(defaluts.vectorLayerList);
            addWMSCustomLayers(defaluts.wmsLayerList);
            addWMTSCustomLayer(defaluts.wmtsLayerList);
            //设置中心点
            if (defaluts.center) {
                map.getView().setCenter(defaluts.center);
            } else {
                map.getView().setCenter([94, 30]);
            }
            if (defaluts.zoom) {
                map.getView().setZoom(defaluts.zoom);
            }
            if (defaluts.extent) {
                map.getView().fit(defaluts.extent, map.getSize());
            }
        }());

        /** 添加地图上的一些控件  **/
        function initMapControl() {
            //2.1显示坐标
            map.addControl(new ol.control.MousePosition({
                className: 'map_mouse-position',
                undefinedHTML: '',
                projection: 'EPSG:4326',
                coordinateFormat: function (coordinate) {
                    return ol.coordinate.format(coordinate, '{x}, {y}', 4);
                }
            }));

            //2.2添加地图缩放
            var zoomIn = me.find("#zoomIn");
            var zoomOut = me.find(".map-zoom-out");
            zoomOut.click(function () {
                var zoom = map.getView().getZoom();
                map.getView().setZoom(zoom - 1);
            });
            zoomIn.click(function () {
                var zoom = map.getView().getZoom();
                map.getView().setZoom(zoom + 1);
            });
            //比例尺
            map.addControl(new ol.control.ScaleLine());
        };

        /** 添加要素  **/
        function addFeature(feat) {
            var id = uuid();
            feat.setId(id);
            if (isSmooth && toolType != "Point" && toolType != "Circle") {
                var geometry = feat.getGeometry();
                var coords = geometry.getCoordinates();
                var smoothened = [];
                if (toolType == "Polygon") {
                    smoothened = getCurvePoints(convertToSmoothPoint(coords[0]), 0.5, 20, false);//makeSmooth(coords[0], 3);
                    geometry.setCoordinates([convertToGeometryPoint(smoothened)]);
                } else if (toolType == "LineString") {
                    smoothened = getCurvePoints(convertToSmoothPoint(coords), 0.5, 20, false);
                    geometry.setCoordinates(convertToGeometryPoint(smoothened));
                }
            }
            var style = new ol.style.Style({
                fill: new ol.style.Fill({
                    color: 'rgba(255,255,0,0.8)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0,0,255,1.0)',
                    width: 1
                }),
                image: new ol.style.Circle({
                    radius: 3,
                    stroke: new ol.style.Stroke({
                        color: 'rgba(0, 0, 0, 0.8)'
                    }),
                    fill: new ol.style.Fill({
                        color: 'rgba(255, 255, 0, 0.8)'
                    })
                })
            });
            feat.setStyle(style);
            if (toolType == "LineString") {
                style.getStroke().setWidth(lineWidth);
                style.getStroke().setColor('rgba(251,127,67,1.0)');
            }
            featuresBak.push(feat);
        }

        /** 选中要素  **/
        function selectFeature() {
            //还原其他
            var features = vectorDrawSource.getFeatures();
            for (var i = 0; i < features.length; i++) {
                var feat = features[i];
                var style = feat.getStyle();
                style.getStroke().setLineDash(null);
            }
            clearPointLayer();
            if (!selectFeatures || selectFeatures.length <= 0) {
                return;
            }
            var isClear = true;
            for (var i = 0; i < selectFeatures.length; i++) {
                if (i != 0) {
                    isClear = false;
                }
                var feat = selectFeatures[i];
                var style = feat.getStyle();
                style.getStroke().setLineDash([10, 10]);
                //画点图层
                var geometry = selectFeatures[i].getGeometry();
                var coords = null;
                try {
                    coords = geometry.getCoordinates();
                } catch (e) {

                }
                var geoType = geometry.getType();
                if (geoType == "Polygon") {
                    addPointToPointLayer(coords[0], isClear);
                } else if (geoType == "LineString") {
                    addPointToPointLayer(coords, isClear);
                } else if (geoType == "Point") {
                    addPointToPointLayer([[coords[0], coords[1]]], isClear);
                } else if (geoType == "Circle" && geometry instanceof ol.geom.Circle) {
                    var radius = geometry.getRadius();
                    var center = geometry.getCenter();
                    addPointToPointLayer([center], isClear);
                }
            }
            map.render();
        }

        /** 重置要素  **/
        function reSet() {
            var features = vectorDrawSource.getFeatures();
            for (var i = 0; i < features.length; i++) {
                var feat = features[i];
                var style = feat.getStyle();
                style.getStroke().setLineDash(null);
            }
        }

        /** 从备份中重置**/
        function reSetFromBak() {
            vectorDrawSource.clear();
            for (var i = 0; i < featuresBak.length; i++) {
                var feat = featuresBak[i];
                var style = feat.getStyle();
                style.getStroke().setLineDash(null);
                vectorDrawSource.addFeature(featuresBak[i]);
            }
        }

        /** 移除交互类**/
        function removeInteraction() {
            if (polyDraw) {
                map.removeInteraction(polyDraw);
            }
            if (pointSelect) {
                map.removeInteraction(pointSelect);
            }
            if (polyModify) {
                map.removeInteraction(polyModify);
            }
            if (polyDrag) {
                map.removeInteraction(polyDrag);
            }
            if (boxSelect) {
                map.removeInteraction(boxSelect);
            }
            if (snapTool) {
                map.removeInteraction(snapTool);
            }
            boxSelect = null
            polyDraw = null;
            pointSelect = null;
            polyModify = null;
            polyDrag = null;
            snapTool = null;
        }

        /** 清空点的图层**/
        function clearPointLayer() {
            vectorPointSource.clear();
        }

        /** 添加自定义画的要素**/
        function addDrawDataFeatrues(featrues, isClear) {
            if (isClear) {
                featuresBak = [];
                vectorDrawSource.clear();
                selectFeatures = [];
            }
            var wktJSTSReader = new jsts.io.WKTReader();
            var parser = new jsts.io.OL3Parser();
            if (!featrues || featrues.length <= 0) {
                return;
            }
            for (var i = 0; i < featrues.length; i++) {
                var feature = new ol.Feature();
                var geometry = null;

                if (featrues[i].GeoType != "Circle") {
                    var tmp = wktJSTSReader.read(featrues[i].Coordinates);
                    geometry = parser.write(tmp.getGeometryN(0));
                } else {
                    var tmp = featrues[i].Coordinates.split(',')
                    var radius = parseFloat(tmp[0]);
                    var center = [parseFloat(tmp[1]), parseFloat(tmp[2])];
                    geometry = new ol.geom.Circle(center, radius);
                }
                feature.setGeometry(geometry);
                var style = new ol.style.Style({
                    fill: new ol.style.Fill({
                        color: featrues[i].FillColor
                    }),
                    stroke: new ol.style.Stroke({
                        color: featrues[i].LineColor,
                        width: featrues[i].LineWidth
                    }),
                    image: new ol.style.Circle({
                        radius: 3,
                        stroke: new ol.style.Stroke({
                            color: 'rgba(0, 0, 0, 0.8)'
                        }),
                        fill: new ol.style.Fill({
                            color: 'rgba(255, 255, 0, 0.8)'
                        })
                    })
                });
                feature.setStyle(style);
                var id = uuid();
                feature.setId(id);
                vectorDrawSource.addFeature(feature);
                featuresBak.push(feature);
            }
        }

        /** 添加自定义iTelluroLayer**/
        function addiTelluroLayer(layers) {
            if (!layers || layers.length <= 0) {
                return;
            }
            for (var i = 0; i < layers.length; i++) {
                var layer = new iTelluro().newItelluroLayer(layers[i].dataserverkey, layers[i].url, layers[i].tilesize, layers[i].zerolevelsize, layers[i].dataserverkey, defaluts.extent);
                if (!layers[i].zIndex) {
                    layer.setZIndex(defaluts.customZIndex + 10);
                } else {
                    layer.setZIndex(layers[i].zIndex);
                }
                if (layers[i].opacity) {
                    layer.setOpacity(layers[i].opacity);
                }
                map.addLayer(layer);
            }
        }

        /** 添加自定义wms**/
        function addWMSCustomLayers(layers) {
            if (!layers || layers.length <= 0) {
                return;
            }
            for (var i = 0; i < layers.length; i++) {
                var layer = new iTelluro().newTileByWMS(layers[i].url, null, layers[i].format, layers[i].dataserverkey, layers[i].layerName);
                if (!layers[i].zIndex) {
                    layer.setZIndex(defaluts.customZIndex + 10);
                } else {
                    layer.setZIndex(layers[i].zIndex);
                }
                if (layers[i].opacity) {
                    layer.setOpacity(layers[i].opacity);
                }
                map.addLayer(layer);
            }
        }

        /** 添加自定义wmts**/
        function addWMTSCustomLayer(layers) {
            if (!layers || layers.length <= 0) {
                return;
            }
            for (var i = 0; i < layers.length; i++) {
                var layer = new iTelluro().newTileByWMTS(layers[i].url, layers[i].dataserverkey, layers[i].tileMatrixSet, layers[i].tileMatrix, layers[i].format, layers[i].zoomlevel, layers[i].tileSize, layers[i].layerName, layers[i].extent);
                if (!layers[i].zIndex) {
                    layer.setZIndex(defaluts.customZIndex + 10);
                } else {
                    layer.setZIndex(layers[i].zIndex);
                }
                if (layers[i].opacity) {
                    layer.setOpacity(layers[i].opacity);
                }
                map.addLayer(layer);
            }
        }

        /** 添加点到点图层上**/
        function addPointToPointLayer(coords, isClear) {
            //点样式
            if (isClear) {
                clearPointLayer();
            }
            var stylePonit = new ol.style.Style({
                image: new ol.style.Circle({
                    radius: 4,
                    stroke: new ol.style.Stroke({
                        color: 'rgba(0, 0, 0, 0.8)'
                    }),
                    fill: new ol.style.Fill({
                        color: 'rgba(255, 0, 0, 1.0)'
                    })
                })
            });
            for (var j = 0; j < coords.length; j++) {
                var featurePonit = new ol.Feature();
                var geometryPonit = new ol.geom.Point(coords[j]);
                //geometryPonit.setCoordinates();
                featurePonit.setGeometry(geometryPonit);
                featurePonit.setStyle(stylePonit);
                vectorPointSource.addFeature(featurePonit);
            }
        }

        /** 添加剪切边界到地图上**/
        function addCutBoundsToLayer() {
            if (!defaluts.boundWKT || defaluts.boundWKT.length <= 0) {
                return;
            }
            var vectorBoundSource = new ol.source.Vector({
                wrapX: false
            });
            var vectorBoundLayer = new ol.layer.Vector({
                source: vectorBoundSource,
                name: "cutLayer"
            });
            var style = new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: 'rgba(200,200,10,1.0)',
                    width: 3
                })
            });
            var wktParse = new ol.format.WKT();
            for (var i = 0; i < defaluts.boundWKT.length; i++) {
                var feature = wktParse.readFeature(defaluts.boundWKT[i]);
                feature.setStyle(style);
                vectorBoundSource.addFeature(feature);
            }
            vectorBoundLayer.setZIndex(defaluts.customZIndex + 95);
            map.addLayer(vectorBoundLayer);

        }

        /** 添加自定义矢量图层**/
        function addVectorLayer(layers) {
            if (!layers || layers.length <= 0) {
                return;
            };
            var style = new ol.style.Style({
                stroke: new ol.style.Stroke({
                    width: 2,
                    color: 'rgba(0,0,255,1.0)'
                })
            });
            for (var i = 0; i < layers.length; i++) {
                //加载geojson数据
                var geoJsonLayer = new ol.layer.Vector({
                    source: new ol.source.Vector({
                        url: layers[i].geoJsonUrl,
                        format: new ol.format.GeoJSON()
                    }),
                    style: style,
                    name: layers[i].layerName
                });
                geoJsonLayer.setZIndex(defaluts.customZIndex + 95);
                snapLayer = geoJsonLayer;
                map.addLayer(geoJsonLayer);
            };
        };

        /*要素feature的编辑移动删除*/
        var editHtml = '<div class="edit-box" style="border:1px solid #ed625e;display:none;cursor:pointer;background:#fff;border-radius:3px;">' +
                           '<span class="map_delete" data-toggle="tooltip" data-placement="bottom" title="删除要素"><i class="fa fa-trash-o"></i></span>' +
                           '<span class="map_amend" data-toggle="tooltip" data-placement="bottom" title="修改要素"><i class="fa fa-pencil"></i></span>' +
                           '<span class="map_move" data-toggle="tooltip" data-placement="bottom" title="激活移动要素/取消停止移动"><i class="fa fa-arrows"></i></span>' +
                       '</div>';
        var editBox = $(editHtml);
        me.append(editBox);
        me.find("[data-toggle='tooltip']").tooltip();
        var onFeatrue = null, selectedFeature = null, isSelected = 0;
        //设置修改删除tip图层
        var editLayer = new ol.Overlay({
            element: editBox[0],
            positioning: 'top-right',
            //autoPan: true,
            //autoPanAnimation: {
            //    duration: 250
            //},
            stopEvent: false
        });
        map.addOverlay(editLayer);
        //鼠标移入移出feature事件
        map.on('pointermove', function (event) {
            if (event.dragging) { return; }
            var pixel = map.getEventPixel(event.originalEvent);
            var hit = map.hasFeatureAtPixel(pixel);
            onFeatrue = currentFeature(pixel);//鼠标移入的当前featrue
            var isOnFeatureSelected = setEditLayerPosition(onFeatrue).isSelectedFeature;
            if (isOnFeatureSelected) {
                editBox.show();
            } else {
                if (isSelected == 1) {
                    editBox.show().parent().show();
                } else {
                    editBox.hide();
                }
            };
            var isCurrentSelectedFeature = setEditLayerPosition(selectedFeature).isSelectedFeature;
            var position = isCurrentSelectedFeature ? setEditLayerPosition(selectedFeature).position : setEditLayerPosition(onFeatrue).position;
            editLayer.setPosition(position);
        });
        //通过鼠标坐标找当前feature
        function currentFeature(pixel) {
            var feature = map.forEachFeatureAtPixel(pixel, function (feature) {
                return feature;
            });
            return feature;
        };
        //设置修改删除tip位置
        function setEditLayerPosition(feature) {
            var position = null, isSelectedFeature = false;
            var Features = vectorDrawSource.getFeatures();
            if (Features.length > 0) {
                for (var i = 0; i < Features.length; i++) {
                    if (Features[i] === feature) {
                        isSelectedFeature = true;
                        var GeoType = Features[i].getGeometry().getType();
                        if (GeoType == "Point") {
                            position = Features[i].getGeometry().getCoordinates();
                        } else if (GeoType == "LineString") {
                            var _coordinates = Features[i].getGeometry().getCoordinates();
                            var num = Math.round(_coordinates.length / 2);
                            position = _coordinates[num];
                        } else if (GeoType == "Circle") {
                            position = Features[i].getGeometry().getCenter();
                        } else {
                            var extent = ol.extent.boundingExtent(Features[i].getGeometry().getCoordinates()[0]);
                            position = ol.extent.getCenter(extent);
                        };
                    };
                };
            };
            return { "position": position, "isSelectedFeature": isSelectedFeature };
        };
        //单击feature选中
        map.on("click", function (event) {
            if (isEdit) {
                return;
            }
            var pixel = map.getEventPixel(event.originalEvent);
            var featrue = currentFeature(pixel);//鼠标移入的当前featrue
            var position = setEditLayerPosition(onFeatrue).position;
            var isSelectedFeature = setEditLayerPosition(onFeatrue).isSelectedFeature;
            if (isSelectedFeature) {
                mapSelect();
                isSelected = 1;
                selectedFeature = featrue;
                me.find(".edit-box>.map_move").removeClass("click").find("i").css("color", "#ed625e");
            } else {
                isSelected = 0;
                selectedFeature = null;
                if (_index) {
                    layer.close(_index);
                    breakSelect = false;
                    clearPointLayer();
                }
            }
        });

        map.on('postcompose', function (event) {
            changeMapData(event.context);
        });

        var wktOLReaderALL = new ol.format.WKT();
        var boundPolygon = null;
        if (defaluts.boundWKT && defaluts.boundWKT[0].length > 0) {
            boundPolygon = wktOLReaderALL.readFeature(defaluts.boundWKT[0]).getGeometry();
        }
        //改变地图数据
        function changeMapData(context) {
            if (!boundPolygon) {
                return;
            }
            context.save();
            var coors = boundPolygon.getCoordinates();
            var pointArr = [];
            for (var i = 0; i < coors.length; i++) {
                var coorTmp = coors[i];
                var pointTmp = [];
                for (var j = 0; j < coorTmp.length; j++) {
                    pointTmp.push(map.getPixelFromCoordinate(coorTmp[j]));
                }
                pointArr.push(pointTmp);
            }
            context.globalCompositeOperation = "destination-in";
            context.beginPath();
            for (var i = 0; i < pointArr.length; i++) {
                var pointTmp = pointArr[i];
                for (var j = 0; j < pointTmp.length; j++) {
                    if (j == 0) {
                        context.moveTo(pointTmp[j][0], pointTmp[j][1]);
                    } else {
                        context.lineTo(pointTmp[j][0], pointTmp[j][1]);
                    }
                }
            }
            context.closePath();
            context.fillStyle = "#ff0000ff";
            context.fill();
            context.restore();
        }

        /*移动feature*/
        var _flag = true;
        me.find(".edit-box>.map_move").click(function () {
            if (pointSelect) {
                map.removeInteraction(pointSelect);
                isSelected = 0;
                selectedFeature = null;
            };
            if (_flag) {
                $(this).addClass("click").find("i").css("color", "#fff");
                mapMove();
                _flag = false;
            } else {
                $(this).removeClass("click").find("i").css("color", "#ed625e");
                $(map.getTargetElement()).find("canvas").css("cursor", "default");
                clearPointLayer();
                _flag = true;
            }
        });

        /*feature删除事件*/
        me.find(".edit-box>.map_delete").click(function () {
            if (pointSelect) {
                map.removeInteraction(pointSelect);
                isSelected = 0;
                selectedFeature = null;
            };
            removeInteraction();
            clearPointLayer();
            if (!selectFeatures || selectFeatures.length <= 0) {
                vectorDrawSource.removeFeature(onFeatrue);
                featuresBak.removeEx(onFeatrue);
            } else {
                delFeature();
            };
            $(this).parent().hide();
            successTip.hide();
        });

        /*feature修改事件*/
        var _index = 0;
        me.find(".edit-box>.map_amend").click(function (e) {
            if (_index) {
                layer.close(_index);//关闭前一个弹框
            };
            me.find(".edit-box>.map_move").removeClass("click").find("i").css("color", "#ed625e");
            var feature = onFeatrue;
            var currentFeature = selectFeatures && selectFeatures.length > 0 ? selectFeatures[0] : feature;
            //弹框位置
            var layerTop = setLayerPosition(e)[0];
            var layerLeft = setLayerPosition(e)[1];
            if ($(".feature-style-edit").is(':visible')) {
                return;
            };
            //获取当前要素属性
            var Fid = currentFeature.getId();
            var GeoType = currentFeature.getGeometry().getType();
            var style = currentFeature.getStyle();
            var isPoint = GeoType == "Point" ? "flex" : "none";
            var isNotPoint = GeoType !== "Point" ? "flex" : "none";
            var isBox = GeoType == "Point" || GeoType == "LineString" ? "none" : "flex";
            var LineColor = style.getStroke().getColor();
            var LineWidth = style.getStroke().getWidth();
            var FillColor, FillOpacity = 1;
            if (GeoType == "Point" || GeoType == "LineString") {
                breakSelect = true;
            };
            var _html = '<ul class="feature_edit">' +
                            '<li><label for"feature_name">图形</label><input type="text" disabled="disabled" id="feature_name" /></li>' +
                            '<li style="display:' + isPoint + ';"><label for"longitude">经度</label><input type="text" id="longitude" class="hover" /></li>' +
                            '<li style="display:' + isPoint + ';"><label for"latitude">纬度</label><input type="text" id="latitude" class="hover" /></li>' +
                            '<li><button class="btn btn-default btn-edit" id="edit_shape">编辑形状</button></li>' +
                            '<li style="display:' + isPoint + ';" class="b_title">icon路径</li>' +
                            '<li style="margin:10px 0;display:' + isPoint + ';"><label for"icon_url">路径</label><input type="text" id="icon_url" class="hover" /></li>' +
                            '<li  class="b_title" style="display:' + isNotPoint + ';">外观</li>' +
                            '<li class="fill-wrap" style="display:' + isBox + ';">' +
                                '<div class="item_wrap fill-color"><label for"inner_color">填充</label><div class="color-fix"></div><div class="color-fix-x" style="display:none;"></div></div>' +
                                '<div class="item_wrap"><label for"inner_none" style="text-align:right;padding-right:20%;">无填充</label><input type="checkbox" id="is_fill" /></div>' +
                            '</li>' +
                            '<li style="display:' + isNotPoint + ';">' +
                                '<div class="item_wrap side-color"><label for"border_color">描边</label><div class="color-fix" style="background:' + LineColor + '"></div></div>' +
                                '<div class="item_wrap"><input type="number" value="1" min="0" id="border_width" class="hover" style="width:100%;text-indent:5px;" /></div>' +
                            '</li>' +
                            '<li style="display:' + isBox + ';">' +
                                '<label for"opacity_num">透明度</label>' +
                                '<input type="range" value=100 min=0 max=100 step=10 id="opacity_range" />' +
                                '<input type="number" value=1 min=0 max=1 id="opacity_num" class="hover" style="text-center;" />' +
                            '</li>' +
                        '</ul>';
            layer.open({
                type: 1,
                skin: 'feature-style-edit',
                area: ['250px', 'auto'],
                btn: ['保存'],
                title: ['属性', 'text-align:left;'],
                shade: 0,
                offset: [layerTop, layerLeft],
                content: _html,
                success: function (layero, index) {
                    _index = index;
                    layero.find("#border_width").val(LineWidth);
                    if (GeoType == "Point") {
                        layero.find(".btn-edit").attr("id", "btn_PointShow").text("显示位置");
                        var _icon = style.getImage();
                        var _iconProtoFn = Object.keys(_icon.constructor.prototype);
                        var _iconValue = _iconProtoFn.indexOf("getSrc") > 0 ? _icon.getSrc() : "";
                        var _coords = currentFeature.getGeometry().getCoordinates();
                        layero.find("#feature_name").val("点");
                        layero.find("#longitude").val(_coords[0]);
                        layero.find("#latitude").val(_coords[1]);
                        layero.find("#icon_url").val(_iconValue);
                    } else if (GeoType == "LineString") {
                        layero.find("#feature_name").val("线");
                    } else if (GeoType == "Polygon" || GeoType == "Circle" || GeoType == "Box") {
                        if (GeoType == "Circle") {
                            layero.find("#feature_name").val("圆");
                        } else {
                            var Coordinates = currentFeature.getGeometry().getCoordinates();
                            var FeatureName = Coordinates[0].length == 5 ? "矩形" : "多边形";
                            layero.find("#feature_name").val(FeatureName);
                        };
                        FillColor = style.getFill().getColor();
                        var _FillColor = FillColor.split(",");
                        FillOpacity = parseFloat(_FillColor[3].split(")")[0]);
                        layero.find("#opacity_num").val(FillOpacity);
                        layero.find("#opacity_range").val(FillOpacity * 100);
                        if (FillOpacity == 0) {
                            FillColor = 'rgba(' + parseInt(_FillColor[0].split("(")[1]) + ',' + parseInt(_FillColor[1]) + ',' + parseInt(_FillColor[2]) + ', 0.8)';
                            layero.find("#is_fill").prop("checked", true);
                            layero.find(".fill-color>.color-fix").hide().siblings(".color-fix-x").show();
                        }
                        layero.find(".fill-color>.color-fix").css("background", FillColor);

                    };
                    /** 初始化颜色框**/
                    colorBoxInit(layero);
                    /*透明度滑轨样式*/
                    setThumbStyle(layero);
                    /*有无填充*/
                    isFillingColor(layero)
                    /*编辑形状*/
                    layero.find("#edit_shape").click(function () {
                        editShape();
                        layer.close(index);
                    });
                    /*显示点位置*/
                    layero.find("#btn_PointShow").click(function () {
                        vectorDrawSource.removeFeature(currentFeature);
                        featuresBak.removeEx(currentFeature);
                        var id = Fid;
                        var longitude = layero.find("#longitude").val();
                        var latitude = layero.find("#latitude").val();
                        var pointIcon = layero.find("#icon_url").val();
                        //导航到该点
                        showPoint(id, longitude, latitude, pointIcon);
                        layer.close(index);
                    });
                },
                yes: function (index, layero) {
                    var _iconPath = layero.find("#icon_url").val();
                    var isFillNone = layero.find("#is_fill").prop("checked");
                    var fillColor = layero.find(".fill-color>.color-fix").css("background-color");
                    var lineColor = layero.find(".side-color>.color-fix").css("background-color");
                    var lineWidth = parseInt(layero.find("#border_width").val()) < 0 ? 0 : parseInt(layero.find("#border_width").val());
                    var fillOpacity = parseFloat(layero.find("#opacity_num").val());

                    var _fillColor = fillColor.split(",");
                    if (!isFillNone) {
                        fillColor = 'rgba(' + parseInt(_fillColor[0].split("(")[1]) + ',' + parseInt(_fillColor[1]) + ',' + parseInt(_fillColor[2]) + ',' + fillOpacity + ')';
                    } else {
                        fillColor = 'rgba(' + parseInt(_fillColor[0].split("(")[1]) + ',' + parseInt(_fillColor[1]) + ',' + parseInt(_fillColor[2]) + ', 0)';
                    };
                    var Style = new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: fillColor
                        }),
                        stroke: new ol.style.Stroke({
                            color: lineColor,
                            width: lineWidth
                        })
                    });
                    if (GeoType == "Point") {
                        if (_iconPath) {
                            Style = new ol.style.Style({
                                image: new ol.style.Icon({
                                    src: _iconPath,
                                    size: [30, 30],
                                    scale: 0.8
                                }),
                                fill: new ol.style.Fill({
                                    color: 'rgba(255,255,0,0.8)'
                                }),
                                stroke: new ol.style.Stroke({
                                    color: 'rgba(0, 0, 0, 0.8)',
                                    width: 1
                                })
                            });
                        } else {
                            Style = new ol.style.Style({
                                fill: new ol.style.Fill({
                                    color: 'rgba(255,255,0,0.8)'
                                }),
                                stroke: new ol.style.Stroke({
                                    color: 'rgba(0,0,255,1.0)',
                                    width: 1
                                }),
                                image: new ol.style.Circle({
                                    radius: 3,
                                    stroke: new ol.style.Stroke({
                                        color: lineColor,
                                        width: lineWidth
                                    }),
                                    fill: new ol.style.Fill({
                                        color: fillColor
                                    })
                                })
                            });
                        };
                    } else if (GeoType == "LineString") {
                        Style.getStroke().setColor(lineColor);
                        Style.getStroke().setWidth(lineWidth);
                    };
                    currentFeature.setStyle(Style);

                    var wktParser = new ol.format.WKT();
                    var geom = currentFeature.getGeometry();
                    var wktString = wktParser.writeGeometry(geom);
                    var layerId = window.parent.currentLayer.id;
               
                    /*yuanxing add 201902181628*/
                    $.post({
                        url: Base.getApiUrl("/DataEditor/AddLayerElementAttribute"),
                        data: {
                            layerId: layerId,
                            values: [],
                            geometry: wktString
                        },
                        success: function (resp)
                        {
                            if (resp.code == 0) {

                                layer.close(index);
                                removeColpick();
                                clearPointLayer();
                                if (GeoType == "Point" || GeoType == "LineString") {
                                    breakSelect = false;
                                };
                            }
                            else {
                                alert('保存要素失败');
                            }
                        }
                    });
                },
                cancel: function () {
                    removeColpick();
                    removeInteraction();
                    clearPointLayer();
                    if (GeoType == "Point" || GeoType == "LineString") {
                        breakSelect = false;
                    };
                }
            });
        });
        /***********************************弹框内样式设置开始****************************************/
        /*设置弹框位置*/
        function setLayerPosition(e) {
            var mouseTop = e.clientY;
            var mouseLeft = e.clientX;
            var layerTop = (mouseTop - 300) < mapDiv.offset().top ? mapDiv.offset().top : mouseTop - 300;
            var layerLeft = (mouseLeft + 400) > (mapDiv.offset().left + mapDiv.width()) ? mouseLeft - 400 : mouseLeft + 150;
            if ((mouseTop + $(".feature-style-edit").height()) > (mapDiv.offset().top + mapDiv.height())) {
                layerTop = mapDiv.offset().top + mapDiv.height() - $(".feature-style-edit").height();
            };
            return [layerTop, layerLeft];
        };
        /** 初始化颜色框**/
        function colorBoxInit(element) {
            var _colorTd = element.find(".feature_edit").find(".color-fix");
            var colorpick = _colorTd.colpick({
                onSubmit: function (hsb, hex, rgb, el, bySetColor) {
                    var _rgbA = $(el).css("background-color").split(",");
                    var A = 0;
                    var CC = '';
                    if (_rgbA.length == 4) {
                        A = parseFloat(_rgbA[3].split(")")[0]);
                    } else {
                        A = 1.0;
                    };
                    if ($(el).hasClass("side-color")) {
                        CC = 'rgb(' + rgb.r + ',' + rgb.g + ',' + rgb.b + ')';
                    } else {
                        CC = 'rgba(' + rgb.r + ',' + rgb.g + ',' + rgb.b + ' ,' + A + ')';
                    };
                    $(el).css("background", CC);
                    $(el).colpickHide();
                }
            });
            $(".colpick.colpick_full").css("z-index", "99999");
            _colorTd.click(function () {
                if (!colorpick) {
                    return;
                };
                var color = $(this).css("background-color");
                var rgba = color.split(",");
                var R = parseInt(rgba[0].split("(")[1]);
                var G = parseInt(rgba[1]);
                if (rgba.length == 4) {
                    var B = parseInt(rgba[2]);
                } else {
                    var B = parseInt(rgba[2].split(")")[0]);
                };
                colorpick.colpickSetColor({ r: R, g: G, b: B });
            });
        };
        /*透明度滑轨样式*/
        function setThumbStyle(element) {
            var _rangeTrack = element.find("#opacity_range");
            _rangeTrack.css("background-size", parseInt(element.find("#opacity_num").val() * 100) + "% 100%");
            element.find("#opacity_num").on("input", function () {
                var _opacityNum = $(this).val();
                _rangeTrack.val(parseInt(_opacityNum * 100));
                _rangeTrack.css("background-size", parseInt(_opacityNum * 100) + "% 100%");
            });
            _rangeTrack.on("input", function () {
                var _rangeValue = $(this).val();
                $(this).css("background-size", _rangeValue + "% 100%");
                element.find("#opacity_num").val(parseFloat(_rangeValue / 100));
            });
        };
        /*有无填充*/
        function isFillingColor(element) {
            element.find("#is_fill").on("change", function () {
                var isFillNone = $(this).prop("checked");
                var colorFix = $(this).parent().siblings().find(".color-fix");
                if (isFillNone) {
                    colorFix.hide().siblings(".color-fix-x").show();
                    element.find("#opacity_num").val(0);
                    element.find("#opacity_range").val(0).css("background-size", "0% 100%");
                } else {
                    colorFix.show().siblings(".color-fix-x").hide();
                    element.find("#opacity_num").val(0.8);
                    element.find("#opacity_range").val(80).css("background-size", "80% 100%");
                }
            });
        };
        /***********************************弹框内样式设置结束****************************************/

        /** 添加依附工具**/
        function addSnapTool() {
            if (!snapLayer) {
                return;
            }
            if (snapTool) {
                map.removeInteraction(snapTool);
            }
            snapTool = new ol.interaction.Snap({
                source: snapLayer.getSource()
            });
            map.addInteraction(snapTool);
        };

        /** 生成Guid **/
        function uuid() {
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
        }

        /** 坐标装换 **/
        function convertToSmoothPoint(coords) {
            var resultArray = [];
            if (!coords || coords.length <= 0) {
                return resultArray;
            }

            for (var i = 0; i < coords.length; i++) {
                resultArray.push(coords[i][0]);
                resultArray.push(coords[i][1]);
            }
            return resultArray;
        }

        function convertToGeometryPoint(coords) {
            var resultArray = [];
            if (!coords || coords.length <= 0) {
                return resultArray;
            }

            for (var i = 0; i < coords.length; i += 2) {
                resultArray.push([coords[i], coords[i + 1]]);
            }
            return resultArray;
        }

        function createCircle(feature) {
            var featureC = new ol.Feature();
            var style = feature.getStyle();
            var radius = feature.getGeometry().getRadius();
            var center = feature.getGeometry().getCenter();
            var coords = [];
            for (var i = 0; i < 360; i++) {
                var hudu = (2 * Math.PI / 360) * i;
                var x = radius * Math.sin(hudu) + center[0];
                var y = radius * Math.cos(hudu) + center[1];
                coords.push([x, y]);
            }
            coords.push(coords[0]);
            var geometry = new ol.geom.Polygon([coords]);
            // geometry.setCoordinates([coords]);
            featureC.setGeometry(geometry);
            featureC.setStyle(style);
            return featureC;
        }

        /** 地图事件**/
        //map.on('singleclick', function (event, a) {
        //    //console.log(event);
        //    me.find('.map_set').removeClass("active").find("i").css("color", "");
        //    $(".map_container").find(".set_box").hide();
        //});

        //map.on('dblclick', function (event, a) {
        //    //console.log(event);
        //});

        //加载iTelluroServer图层
        function loadCustomLayerByiTelluro(layer, dataServerKey, url, tileSize, zeroLevelSize, layerName, zIndex, opacity) {
            if (!layer) {
                layer = new iTelluro().newItelluroLayer(dataServerKey, url, tileSize, zeroLevelSize, layerName, defaluts.extent);
                var _zIndex = zIndex ? zIndex : defaluts.customZIndex + 22;
                layer.setZIndex(_zIndex);
                if (opacity) {
                    layer.setOpacity(opacity);
                }
                map.getView().setZoom(0);
                map.addLayer(layer);
            } else {
                layer.setVisible(true);
            }
            return layer;
        };

        //加载WMTSServer图层
        function loadCustomLayerByWMTS(layer, dataServerKey, url, tileMatrixSet, tileMatrixid, format, zoomlevel, tileSize, layerName, extent, zIndex, opacity) {
            if (!layer) {
                layer = new iTelluro().newTileByWMTS(url, dataServerKey, tileMatrixSet, tileMatrixid, format, zoomlevel, tileSize, layerName, extent);
                var _zIndex = zIndex ? zIndex : defaluts.customZIndex + 22;
                layer.setZIndex(_zIndex);
                if (opacity) {
                    layer.setOpacity(opacity);
                }
                map.addLayer(layer);
            } else {
                layer.setVisible(true);
            }
            return layer;
        };

        //加载WMSServer图层
        function loadCustomLayerByWMS(layer, url, sld, format, dataServerKey, layerName, zIndex, opacity) {
            if (!layer) {
                layer = new iTelluro().newTileByWMS(url, sld, format, dataServerKey, layerName);
                var _zIndex = zIndex ? zIndex : defaluts.customZIndex + 22;
                layer.setZIndex(_zIndex);
                if (opacity) {
                    layer.setOpacity(opacity);
                }
                map.getView().setZoom(0);
                map.addLayer(layer);
            } else {
                layer.setVisible(true);
            }
            return layer;
        };

        /**加载GeoJson图层 **/
        function loadGeoJsonFile(geoJsonInfos) {
            remoceGeoJsonLayers();
            if (!geoJsonInfos || geoJsonInfos.length <= 0) {
                return;
            }
            for (var i = 0; i < geoJsonInfos.length; i++) {
                var geoJson = geoJsonInfos[i];
                var gjStyle = defualtJsonStyle;
                if (geoJson.geoJsonStyle) {
                    gjStyle = geoJson.geoJsonStyle;
                }
                //加载geojson数据
                var geoJsonLayer = new ol.layer.Vector({
                    source: new ol.source.Vector({
                        url: geoJson.geoJsonUrl,
                        format: new ol.format.GeoJSON()
                    }),
                    style: gjStyle,
                    name: "GeoJsonLayer_" + uuid()
                });

                map.addLayer(geoJsonLayer);
            }

        };

        //判断另一组矩形区域是否与1矩形区域有交集
        //rc1:[{west:114.23,south:33.22,east:114.30, north:33.42}...]  rc2:{west:113.23, south:30.22, east:115.38, north:39.42}
        function IsBIntersectRects(rc1, rc2) {
            var isIntersects;
            rc1.forEach(function (item) {
                var extent1 = [item.west, item.south, item.east, item.north];
                var extent2 = [rc2.west, rc2.south, rc2.east, rc2.north];
                isIntersects = ol.extent.intersects(extent1, extent2);
                return isIntersects;
            });
            return isIntersects;
        };
        //判断一矩形区域是否与另一组矩形区域有交集
        //rc1:{west:114.23,south:33.22,east:114.30, north:33.42}  rc2:{WESTLONGITUDE:113.23, SOUTHLATITUDE:30.22, EASTLONGITUDE:115.38, NORTHLATITUDE:39.42}
        function IsBIntersectRect(rc1, rc2) {
            var isIntersects;
            var extent1 = [rc1.west, rc1.south, rc1.east, rc1.north];
            var extent2 = [rc2.west, rc2.south, rc2.east, rc2.north];
            isIntersects = ol.extent.intersects(extent1, extent2);
            return isIntersects;
        };

        //定位到某个区域中心点
        //range:{west:113.23, south:30.22, east:115.38, north:39.42}
        function moveToAreaAtCenter(range) {
            var longitude = (range.west + range.east) / 2;
            var latitude = (range.north + range.south) / 2;
            map.getView().animate({ center: [longitude, latitude], duration: 1500 });
        };

        //图层闪烁
        function flickerLayer(layer) {
            var isHide = true, num = 1;
            flicker();
            function flicker() {
                isHide = !isHide;
                setTimeout(function () {
                    layer.setVisible(isHide);
                    num++;
                    if (num == 7) { return; };
                    flicker();
                }, 800);
            };
        };

        /**默认样式 **/
        var defualtJsonStyle = function (feature, resolution) {
            var text = feature.get('CLValue');
            if (!text) {
                text = "";
            }
            var style = new ol.style.Style({
                stroke: new ol.style.Stroke({
                    width: 1.5,
                    color: [0, 0, 255, 255]
                }
                    ),
                text: new ol.style.Text({
                    text: text,
                    fill: new ol.style.Fill({
                        color: '#000'
                    })
                })
            });

            return style;
        };

        //隐藏交互tip
        function hideTip() {
            successTip.hide();
            //editBox.hide();
        };

        //清空geojson图层
        function remoceGeoJsonLayers() {
            var maplayers = map.getLayers().getArray();
            maplayers.forEach(function (layer) {
                var ln = layer.get("name");
                if (ln) {
                    if (ln.indexOf("GeoJsonLayer_") >= 0) {
                        map.removeLayer(layer);
                    }
                }
            });
        };

        /** 加marker图片标注根据一组坐标和marker的图片地址来显示 **/
        function addMarker(markerArr) {
            //清空定为
            removeMarker();
            //清空标注
            removeCustomMarker();
            var features = [];
            //第一步设置style

            //第二部创建featuare并设置好在地图上的位置及样式
            markerArr.forEach(function (data, index) {
                var feature = new ol.Feature({
                    geometry: new ol.geom.Point(data.location),
                    id: index++
                });

                var style = new ol.style.Style({
                    image: new ol.style.Icon({
                        src: data.url
                    })
                });
                var html = '<div class="ol-popup none" id="popup">' + '<a href="#" id="popup-closer" class="ol-popup-closer"></a>' + '<div id="popup-content"></div></div>';
                feature.obj = data.popupHtml;
                //设置样式在样式中既可以设置图标
                feature.setStyle(style);
                //添加到之前创建的layer中
                features.push(feature);
            });
            //第三步source添加到layer
            var source = new ol.source.Vector({
                features: features
            });

            if (defaluts.isCluster) {
                var clusterSource = new ol.source.Cluster({
                    distance: parseInt(defaluts.distance, 10),
                    source: source
                });
                var styleCache = {};
                markerLayer = new ol.layer.Vector({
                    source: clusterSource,
                    style: function (feature) {
                        var features = feature.get('features');
                        var size = features.length;
                        var style = styleCache[size];
                        if (size > 1) {
                            if (!style) {
                                style = new ol.style.Style({
                                    image: new ol.style.Circle({
                                        radius: 10,
                                        stroke: new ol.style.Stroke({
                                            color: '#fff'
                                        }),
                                        fill: new ol.style.Fill({
                                            color: '#3399CC'
                                        })
                                    }),
                                    text: new ol.style.Text({
                                        text: size.toString(),
                                        fill: new ol.style.Fill({
                                            color: '#fff'
                                        })
                                    })
                                });
                                styleCache[size] = style;
                            }
                        }
                        else {
                            style = features[0].getStyle();
                        }
                        //var style_= new ol.style.Style({
                        //    image: style.image_,
                        //    text: new ol.style.Text({
                        //        text: text,
                        //        offsetX: defaluts.offsetX,
                        //        offsetY: defaluts.offsetY
                        //    })
                        //});
                        return style;
                    }
                });
            } else {
                markerLayer = new ol.layer.Vector({
                    source: source
                })
            }

            //第四步添加feature到对应layer的Source中
            //markerLayer.getSource().addFeature(feature);
            markerLayer.setZIndex(30);
            //第五步 layer添加到map
            map.addLayer(markerLayer);
        };

        /** 对外提供的方法 **/
        me[0].ve = {
            updateSize: function () {
                map.updateSize();
                map.render();
            },
            getFeatrues: function () {
                clearPointLayer();
                var result = [];
                var wktOLReader = new ol.format.WKT();
                var features = vectorDrawSource.getFeatures();
                for (var i = 0; i < features.length; i++) {
                    var feat = features[i];

                    var geometry = feat.getGeometry();
                    var coords = null;
                    try {
                        coords = wktOLReader.writeFeature(feat);
                    } catch (e) {

                    }
                    var geoType = geometry.getType();
                    var style = feat.getStyle();
                    var fillColor = style.getFill().getColor();
                    var lineColor = style.getStroke().getColor();
                    var lineWidth = style.getStroke().getWidth();
                    if (geoType == "Circle" && geometry instanceof ol.geom.Circle) {
                        var radius = geometry.getRadius();
                        var center = geometry.getCenter();
                        coords = radius + "," + center[0] + "," + center[1];
                    }

                    if (geoType == "Point") {
                        var _icon = style.getImage();
                        var _iconProtoFn = Object.keys(_icon.constructor.prototype);
                        var pointIcon = "";
                        if (_iconProtoFn.indexOf("getSrc") > 0) {
                            pointIcon = style.getImage().getSrc();
                        }
                        result.push({ Coordinates: coords, GeoType: geoType, FillColor: fillColor, LineColor: lineColor, LineWidth: lineWidth, PointIcon: pointIcon });
                    } else {
                        result.push({ Coordinates: coords, GeoType: geoType, FillColor: fillColor, LineColor: lineColor, LineWidth: lineWidth });
                    }
                }
                return result;
            },
            setVEHeight: function (mapHeight) {
                me.height(mapHeight);
                me.find(".mapDiv").height(mapHeight);
            },
            addDrawDataFeatrues: function (featrues, isClear) {
                addDrawDataFeatrues(featrues, isClear);
            },
            clearDrawDataFeatrues: function () {
                vectorDrawSource.clear();
            },
            clearRectLayer: function () {
                rectSource.clear();
            },
            drawCustomPolygon: function (polyWkt) {
                rectSource.clear();
                var wktJSTSReader = new jsts.io.WKTReader();
                var parser = new jsts.io.OL3Parser();
                var feature = new ol.Feature();
                var tmp = wktJSTSReader.read(polyWkt);
                var geometry = parser.write(tmp.getGeometryN(0));
                feature.setGeometry(geometry);
                rectSource.addFeature(feature);
            },
            customDraw: function (toolType, callBack) {
                rectSource.clear();
                rectDraw = null;
                if (!rectDraw) {
                    if (toolType == "Box") {
                        var geoFun = ol.interaction.Draw.createBox();
                        rectDraw = new ol.interaction.Draw({
                            source: rectSource,
                            type: "Circle",
                            geometryFunction: geoFun
                        });
                    } else {
                        rectDraw = new ol.interaction.Draw({
                            source: rectSource,
                            type: toolType
                        });
                    };
                    //形状画完时
                    rectDraw.on('drawend', function (event) {
                        var feat = event.feature;
                        callBack(feat, rectDraw)
                    });
                }
                map.addInteraction(rectDraw);
            },
            loadCustomLayerByiTelluro: function (layer, dataServerKey, url, tileSize, zeroLevelSize, layerName, zIndex, opacity) {
                loadCustomLayerByiTelluro(layer, dataServerKey, url, tileSize, zeroLevelSize, layerName, zIndex, opacity);
            },
            loadCustomLayerByWMTS: function (layer, dataServerKey, url, tileMatrixSet, tileMatrixid, format, zoomlevel, tileSize, layerName, extent, zIndex, opacity) {
                loadCustomLayerByWMTS(layer, dataServerKey, url, tileMatrixSet, tileMatrixid, format, zoomlevel, tileSize, layerName, extent, zIndex, opacity);
            },
            loadCustomLayerByWMS: function (layer, url, sld, format, dataServerKey, layerName, zIndex, opacity) {
                loadCustomLayerByWMS(layer, url, sld, format, dataServerKey, layerName, zIndex, opacity);
            },
            getMap: function () {
                return map;
            },
            resetMapBtnPosition: function () {
                me.find(".map_controller_btn").css("left", mapDiv.width() - 80);
            },
            loadGeoJsonFile: function (geoJsonInfos) {
                loadGeoJsonFile(geoJsonInfos);
            },
            IsBIntersectRects: function (rc1, rc2) {
                return IsBIntersectRects(rc1, rc2);
            },
            IsBIntersectRect: function (rc1, rc2) {
                return IsBIntersectRect(rc1, rc2);
            },
            moveToAreaAtCenter: function (range) {
                moveToAreaAtCenter(range);
            },
            flickerLayer: function (layer) {
                flickerLayer(layer);
            },
            removeInteraction: function () {
                removeInteraction();
            },
            clearPointLayer: function () {
                clearPointLayer();
            },
            hideTip: function () {
                hideTip();
            },
            addMarker: function (markerArr) {
                addMarker(markerArr);
            }
        };
        return me;
    };
    //地图改变大小刷新
    $.fn.updateSize = function () {
        if (this[0].ve) {
            this[0].ve.updateSize();
        }
    };
    //获取画的要素
    $.fn.getFeatrues = function () {
        if (this[0].ve) {
            return this[0].ve.getFeatrues();
        }
    };
    $.fn.setVEHeight = function (mapHeight) {
        if (this[0].ve) {
            return this[0].ve.setVEHeight(mapHeight);
        }
    };
    $.fn.addDrawDataFeatrues = function (featrues, isClear) {
        if (this[0].ve) {
            return this[0].ve.addDrawDataFeatrues(featrues, isClear);
        }
    };
    $.fn.clearDrawDataFeatrues = function () {
        if (this[0].ve) {
            return this[0].ve.clearDrawDataFeatrues();
        }
    };

    $.fn.drawCustomPolygon = function (polyWkt) {
        if (this[0].ve) {
            return this[0].ve.drawCustomPolygon(polyWkt);
        }
    };
    //框选
    $.fn.customDraw = function (toolType, callBack) {
        if (this[0].ve) {
            return this[0].ve.customDraw(toolType, callBack);
        }
    };
    //加载iTelluroServer图层
    $.fn.loadCustomLayerByiTelluro = function (layer, dataServerKey, url, tileSize, zeroLevelSize, layerName, zIndex, opacity) {
        if (this[0].ve) {
            this[0].ve.loadCustomLayerByiTelluro(layer, dataServerKey, url, tileSize, zeroLevelSize, layerName, zIndex, opacity);
        }
    };
    //加载WMTSServer图层
    $.fn.loadCustomLayerByWMTS = function (layer, dataServerKey, url, tileMatrixSet, tileMatrixid, format, zoomlevel, tileSize, layerName, extent, zIndex, opacity) {
        if (this[0].ve) {
            this[0].ve.loadCustomLayerByWMTS(layer, dataServerKey, url, tileMatrixSet, tileMatrixid, format, zoomlevel, tileSize, layerName, extent, zIndex, opacity);
        }
    };
    //加载WMSServer图层
    $.fn.loadCustomLayerByWMS = function (layer, url, sld, format, dataServerKey, layerName, zIndex, opacity) {
        if (this[0].ve) {
            this[0].ve.loadCustomLayerByWMS(layer, url, sld, format, dataServerKey, layerName, zIndex, opacity);
        }
    };

    $.fn.getMap = function () {
        if (this[0].ve) {
            return this[0].ve.getMap();
        }
    };
    $.fn.clearRectLayer = function () {
        if (this[0].ve) {
            return this[0].ve.clearRectLayer();
        }
    };
    $.fn.resetMapBtnPosition = function () {
        if (this[0].ve) {
            return this[0].ve.resetMapBtnPosition();
        }
    };
    $.fn.loadGeoJsonFile = function (geoJsonInfos) {
        if (this[0].ve) {
            this[0].ve.loadGeoJsonFile(geoJsonInfos);
        }
    };
    $.fn.IsBIntersectRects = function (rc1, rc2) {
        if (this[0].ve) {
            return this[0].ve.IsBIntersectRects(rc1, rc2);
        }
    };
    $.fn.IsBIntersectRect = function (rc1, rc2) {
        if (this[0].ve) {
            return this[0].ve.IsBIntersectRect(rc1, rc2);
        }
    };
    $.fn.moveToAreaAtCenter = function (range) {
        if (this[0].ve) {
            this[0].ve.moveToAreaAtCenter(range);
        }
    };
    $.fn.flickerLayer = function (layer) {
        if (this[0].ve) {
            this[0].ve.flickerLayer(layer);
        }
    };
    $.fn.removeInteraction = function () {
        if (this[0].ve) {
            this[0].ve.removeInteraction();
        }
    };
    $.fn.clearPointLayer = function () {
        if (this[0].ve) {
            this[0].ve.clearPointLayer();
        }
    };
    $.fn.hideTip = function () {
        if (this[0].ve) {
            this[0].ve.hideTip();
        }
    };
    $.fn.addMarker = function (markerArr) {
        if (this[0].ve) {
            this[0].ve.addMarker(markerArr);
        }
    };
})(jQuery);