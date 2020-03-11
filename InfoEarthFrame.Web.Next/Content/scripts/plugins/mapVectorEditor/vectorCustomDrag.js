/// <reference path="../openLayer/ol.js" />
/**
* 定义命名空间
*/
var vectorDrag = {};

vectorDrag.Drag = function () {
    ol.interaction.Pointer.call(this, {
        handleDownEvent: vectorDrag.Drag.prototype.handleDownEvent,
        handleDragEvent: vectorDrag.Drag.prototype.handleDragEvent,
        handleMoveEvent: vectorDrag.Drag.prototype.handleMoveEvent,
        handleUpEvent: vectorDrag.Drag.prototype.handleUpEvent
    });
    /**
     * @type {module:ol/pixel~Pixel}
     * @private
     */
    this.coordinate_ = null;

    /**
     * @type {string|undefined}
     * @private
     */
    this.cursor_ = 'pointer';

    /**
     * @type {module:ol/Feature~Feature}
     * @private
     */
    this.feature_ = null;

    /**
     * @type {string|undefined}
     * @private
     */
    this.previousCursor_ = undefined;

};
ol.inherits(vectorDrag.Drag, ol.interaction.Pointer);

/**
 * 鼠标按下事件
 */
vectorDrag.Drag.prototype.handleDownEvent = function (evt) {
    var map = evt.map;
    var feature = map.forEachFeatureAtPixel(evt.pixel,
      function (feature) {
          return feature;
      });
   
    if (!feature) {
        return;
    }
    var id = feature.getId();
    if (!id || id.indexOf('InfoEarth') < 0) {
        return;
    }
    if (feature) {
        this.coordinate_ = evt.coordinate;
        this.feature_ = feature;
    }

    return !!feature;
};

/**
 * 拖动事件
 */
vectorDrag.Drag.prototype.handleDragEvent = function (evt) {
    var map = evt.map;
    var feature = map.forEachFeatureAtPixel(evt.pixel,
      function (feature) {
          return feature;
      });
    if (!feature) {
        return;
    }
    var id = feature.getId();
    if (!id || id.indexOf('InfoEarth') < 0) {
        return;
    }
    var deltaX = evt.coordinate[0] - this.coordinate_[0];
    var deltaY = evt.coordinate[1] - this.coordinate_[1];

    var geometry = this.feature_.getGeometry();
    geometry.translate(deltaX, deltaY);

    this.coordinate_[0] = evt.coordinate[0];
    this.coordinate_[1] = evt.coordinate[1];
};


/**
 * 移动事件
 */
vectorDrag.Drag.prototype.handleMoveEvent = function (evt) {
    var map = evt.map;
    var element = evt.map.getTargetElement();
    var feature = map.forEachFeatureAtPixel(evt.pixel,
      function (feature) {
          return feature;
      });
    if (!feature) {
        if (this.previousCursor_ !== undefined) {
            element.style.cursor = this.previousCursor_;
            this.previousCursor_ = undefined;
        }
        return;
    }
    var id = feature.getId();
    if (!id || id.indexOf('InfoEarth') < 0) {
        if (this.previousCursor_ !== undefined) {
            element.style.cursor = this.previousCursor_;
            this.previousCursor_ = undefined;
        }
        return;
    } 
    if (this.cursor_) {
        if (feature) {
            if (element.style.cursor != this.cursor_) {
                this.previousCursor_ = element.style.cursor;
                element.style.cursor = this.cursor_;
            }
        } else if (this.previousCursor_ !== undefined) {
            element.style.cursor = this.previousCursor_;
            this.previousCursor_ = undefined;
        }
    }
};


/**
 * Up事件
 */
vectorDrag.Drag.prototype.handleUpEvent = function (evt) {
    this.coordinate_ = null;
    this.feature_ = null;
    var maplayers = evt.map.getOverlays().getArray();
    maplayers.forEach(function (layer) {
        if (layer.getId() == "tipLayer") {
            layer.setPosition(null);
        }
    });
    return false;
};

