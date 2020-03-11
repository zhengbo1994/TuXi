/// <reference path="../openLayer/ol.js" />
function ShapeBase() {
};
ShapeBase.prototype = {
    constructor: ShapeBase,
    ShapeFeature: null,
    FillColor: '#123456',
    Opacity: 1,
    LineWidth: 2,
    LineColor: '#123456',
    SelectColor: '#123456',
    ShapeWKT:"",
    clone: function () {
        var obj = {};
        $.extend(obj, this);
    },
    toFeature: function () {
        if (!this.ShapeFeature) {
            return null;
        }

        var style = new ol.style.Style({
            fill: new ol.style.Fill({
                color: this.FillColor
            }),
            stroke: new ol.style.Stroke({
                color: this.LineColor,
                width: this.LineWidth
            })
        });
        this.ShapeFeature.setStyle(style);
    },
    toWKT: function () {
       
    },
    fromFeatrue: function (feature) {
        this.ShapeFeature = feature;
    }
  
};