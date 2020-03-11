/**
 * those are for pulling tiles from Itelluro tiles source address, which would return a new layer or map that is access to openlayer3.
 * refer to http://openlayers.org
 */

function iTelluro() {

    var _renderer = "dom"; //canvas,dom,webgl

    //
    // map

    /**
     * @param {string} id The container for the map.
     * @param {number} numLevels The maximum zoom level used to determine the resolution constraint.
     * @param {number} dsEast  MaxX extent coordinates.
     * @param {number} dsWest MinX extent coordinates
     * @param {number} dsSouth MinY extent coordinates
     * @param {numbere} dsNorth MaxY extent coordinates
     * @param {string} dataServerKey Source Package Key of tiles' provider.
     * @param {string} url URL address of tiles' provider.
     * @param {number} tileSize The pixel of tile.
     * @param {number} zeroLevelSize  The size of zero level tile.
     * @param {string} layerName   图层名
     * @return {ol.layer.Tile} return map
     */
    this.NewWebTilesMap = function (id, numLevels, dsEast, dsWest, dsSouth, dsNorth,
                                    dataServerKey, url, tileSize, zeroLevelSize, layerName) {
        var layer1 = this.newItelluroLayer(dataServerKey, url, tileSize, zeroLevelSize, layerName);

        var view = new ol.View({
            center: [(dsEast + dsWest) / 2, (dsNorth + dsSouth) / 2], zoom: 0, minZoom: 0, maxZoom: numLevels,
            projection: 'EPSG:4326',
            maxResolution: zeroLevelSize / tileSize
        });

        var map = new ol.Map({
            layers: [layer1],
            target: id,
            view: view,
            renderer: _renderer
        });

        return map;
    };

    /**
     * @param {string} id The container for the map.
     * @param {number} numLevels The maximum zoom level used to determine the resolution constraint.
     * @param {number} dsEast  MaxX extent coordinates.
     * @param {number} dsWest MinX extent coordinates
     * @param {number} dsSouth MinY extent coordinates
     * @param {number} dsNorth MaxY extent coordinates
     * @param {string} root Local root directory of tiles' files.
     * @param {number} tileSize The pixel of tile.
     * @param {number} zeroLevelSize  The size of zero level tile.
     * @param {string} layerName   图层名
     * @return {ol.layer.Tile} return map
     */
    this.NewLocalTilesMap = function (id, numLevels, dsEast, dsWest, dsSouth, dsNorth,
                                      root, tileSize, zeroLevelSize, zoom) {

        var layer1 = this.newLocalTilesLayer(root, tileSize, zeroLevelSize);

        var view = new ol.View({
            center: [(dsEast + dsWest) / 2, (dsNorth + dsSouth) / 2], zoom: 0, minZoom: 0, maxZoom: numLevels,
            projection: 'EPSG:4326',
            maxResolution: zeroLevelSize / tileSize
        });

        var map = new ol.Map({
            layers: [layer1],
            target: id,
            view: view,
            renderer: _renderer,
            zoom: zoom
        });

        return map;
    };


    //
    // layer

    /**
     * @param {string} dataServerKey The package key address of tiles' provider.
     * @param {string} url URL address of tiles' provider.
     * @param {number} tileSize The pixel of tile.
     * @param {number} zeroLevelSize  The size of zero level tile.
     * @param {string} layerName   图层名
     * @return {ol.layer.Tile} return layer
     */
    this.newItelluroLayer = function (dataServerKey, url, tileSize, zeroLevelSize, layerName) {
        if (url.indexOf("?") > 0)
            url += "&T={t}";
        else
            url += "?T={t}";
        url = url.replace('{t}', dataServerKey);
        return this.newWebTilesLayer(url, tileSize, zeroLevelSize, layerName);
    };

    /**
     * @param {string} root Local root directory of tiles' files.
     * @param {number} tileSize The pixel of tile.
     * @param {number} zeroLevelSize  The size of zero level tile.
     * @param {string} layerName   图层名
     * @return {ol.layer.Tile} return layer
     */
    this.newLocalTilesLayer = function (root, tileSize, zeroLevelSize, layerName) {
        var imageType = "png";
        root += "/{z}/{y}/{y}_{x}." + imageType;
        return this.newTilesUrlLayer(root, tileSize, zeroLevelSize, layerName);
    };

    /**
     * @param {string} url URL address of tiles' provider.
     * @param {number} tileSize The pixel of tile.
     * @param {number} zeroLevelSize  The size of zero level tile.
     * @param {string} layerName   图层名
     * @return {ol.layer.Tile} return layer
     */
    this.newWebTilesLayer = function (url, tileSize, zeroLevelSize, layerName) {
        url = encodeURI(url);
        if (url.indexOf("?") > 0)
            url += "&L={z}&X={x}&Y={y}";
        else
            url += "?L={z}&X={x}&Y={y}";
        return this.newTilesUrlLayer(url, tileSize, zeroLevelSize, layerName);
    };

    /**
     * @param {string} urlTemplate URL template. Must include {x}, {y} or {-y}.
     * @param {number} tileSize The pixel of tile.
     * @param {number} zeroLevelSize  The size of zero level tile.
     * @param {string} layerName   图层名
     * @return {ol.layer.Tile} return layer
     */
    this.newTilesUrlLayer = function (urlTemplate, tileSize, zeroLevelSize, layerName) {
        function tileUrlFunctionCallBack(tileCoord, pixelRatio, projection) {
            var z = tileCoord[0];
            var x = tileCoord[1];
            var y = tileCoord[2];

            var xStr4 = "0000" + x.toString();
            xStr4 = xStr4.substr(xStr4.length - 4, 4);
            var yStr4 = "0000" + y.toString();
            yStr4 = yStr4.substr(yStr4.length - 4, 4);

            var url = urlTemplate.replace('{z}', z.toString())
                .replace('{y}', yStr4)
                .replace('{y}', yStr4)
                .replace('{x}', xStr4)
                .replace('{x}', xStr4);
            return url
        }

        var resolutions = new Array(22);
        for (var i = 0, ii = resolutions.length; i < ii; ++i) {
            resolutions[i] = zeroLevelSize / Math.pow(2, i) / tileSize;
        }
        var layer = new ol.layer.Tile({
            source: new ol.source.XYZ({
                tileSize: tileSize,
                tileUrlFunction: tileUrlFunctionCallBack,
                projection: 'EPSG:4326',
                //tilePixelRatio: tileSize / 256,
                tileGrid: new ol.tilegrid.TileGrid({
                    resolutions: resolutions,
                    tileSize: tileSize,
                    origin: [-180, -90]
                })
            }),
            name: !!layerName ? layerName : ''
        });
        return layer;
    }


    /**
     * @param {string} urlTemplate URL template.
     * @param {sld}  example :  '<?xml version="1.0" encoding="UTF-8"?><sld: .....'
     * @param {string} format  example : 'image/png'
     * @param {string} layers   example :  'tiger:tiger_roads',
	 * @param {string} layerName   图层名
     * @return {ol.layer.Tile} return layer
     */
    this.newTileByWMS = function (urlTemplate, sld, format, layers, layerName) {
        var layer = new ol.layer.Tile({
            source: new ol.source.TileWMS({
                url: urlTemplate,
                params: {
                    FORMAT: format,
                    LAYERS: layers,
                    // SLD_BODY: '',
                    //SLD: "http://tiger_roads.sld",
                    SLD_BODY: sld,
                    TILED: true
                },
                serverType: 'geoserver'
            }),
            name: !!layerName ? layerName : ''
        });
        return layer;
    }


    /**
    * @param {string} url
    * @param {string} layers         example : 'gisdatamanage:dfgdsfgdsfg'
    * @param {string} tileMatrixSet 切片方案  example : 'EPSG:4326'        暂时没用2017-8
    * @param {string} tileMatrixid  切片等级名称  example : 'EPSG:4326:7'  暂时没用2017-8
    * @param {string} format         格式 example : 'image/png'
    * @param {string} zoomlevel  
    * @param {string} layerName   图层名
    * @return {ol.layer.Tile} return layer
    */
    this.newTileByWMTS = function (url, layers, tileMatrixSet, tileMatrixid, format, zoomlevel, layerName) {
        //console.log('Debug:   ', url, layers, tileMatrixSet, tileMatrixid, format, zoomlevel);

        var proj = 'EPSG:4326';
        var lev = !!zoomlevel ? zoomlevel : 15;

        var projection = new ol.proj.get(proj);
        var projectionExtent = projection.getExtent();


        var size = 0.703125;//ol.extent.getWidth(projectionExtent) / 256;

        var resolutions = new Array(lev);
        var matrixIds = new Array(lev);
        for (var i = 0; i < matrixIds.length; i++) {
            resolutions[i] = size / Math.pow(2, i);
            matrixIds[i] = proj + ':' + i;
        }
        //console.log('Debug: resolutions  ', resolutions);

        var layer = new ol.layer.Tile({
            source: new ol.source.WMTS({
                url: url,
                layer: layers,
                format: format,
                matrixSet: proj,
                projection: projection,
                tileGrid: new ol.tilegrid.WMTS({
                    resolutions: resolutions,
                    matrixIds: matrixIds,
                    tileSize: [256, 256],
                    extent: [-180.0, -90.0, 180.0, 90.0],
                    origin: new ol.extent.getTopLeft(projectionExtent)
                }),
                wrapX: true
            }),
            name: !!layerName ? layerName : ''
        });
        return layer;
    }


}

/**
 * @param {string} id The container for the map.
 * @param {number} numLevels The maximum zoom level used to determine the resolution constraint.
 * @param {number} dsEast  MaxX extent coordinates.
 * @param {number} dsWest MinX extent coordinates
 * @param {number} dsSouth MinY extent coordinates
 * @param {numbere} dsNorth MaxY extent coordinates
 * @param {string} sourcePackageKey Source Package Key of tiles' provider.
 * @param {string} url URL address of tiles' provider.
 * @param {number} tileSize The pixel of tile.
 * @param {number} zeroLevelSize  The size of zero level tile.
 * @param {string} layerName   图层名
 * @return {ol.layer.Tile} return map
 */
function NewWebTilesMap(id, numLevels, dsEast, dsWest, dsSouth, dsNorth,
                                sourcePackageKey, url, tileSize, zeroLevelSize, layerName) {
    var itelluro = new iTelluro();
    return itelluro.NewWebTilesMap(id, numLevels, dsEast, dsWest, dsSouth, dsNorth,
        sourcePackageKey, url, tileSize, zeroLevelSize, layerName);
}

/**
 * @param {string} root Local root directory of tiles' files.
 * @param {number} tileSize The pixel of tile.
 * @param {number} zeroLevelSize  The size of zero level tile.
 * @param {string} layerName   图层名
 * @return {ol.layer.Tile} return layer
 */
function newLocalTilesLayer(root, tileSize, zeroLevelSize, layerName) {
    var itelluro = new iTelluro();
    return itelluro.newLocalTilesLayer(root, tileSize, zeroLevelSize, layerName);
}

/**
* @param {string} id The container for the map.
* @param {number} numLevels The maximum zoom level used to determine the resolution constraint.
* @param {number} dsEast  MaxX extent coordinates.
* @param {number} dsWest MinX extent coordinates
* @param {number} dsSouth MinY extent coordinates
* @param {number} dsNorth MaxY extent coordinates
* @param {string} root Local root directory of tiles' files.
* @param {number} tileSize The pixel of tile.
* @param {number} zeroLevelSize  The size of zero level tile.
* @param {string} layerName   图层名
* @return {ol.layer.Tile} return map
*/
function NewLocalTilesMap(id, numLevels, dsEast, dsWest, dsSouth, dsNorth,
                                  root, tileSize, zeroLevelSize, layerName) {
    var itelluro = new iTelluro();
    return itelluro.NewLocalTilesMap(root, tileSize, zeroLevelSize, layerName);
}


/**
 * @param {string} urlTemplate URL template.      (必选)
 * @param {string} layers   图层名称              (必选)
 * @param {string} format  example : 'image/png'
 * @param {sld}  example :  '<?xml version="1.0" encoding="UTF-8"?><sld: .....'
 * @param {string} layerName   图层名
 * @return {ol.layer.Tile} return layer
 */
function newLocalTilesByWMS(urlTemplate, layers, format, sld, layerName) {
    format = !format ? 'image/png' : format;
    var itelluro = new iTelluro();
    return itelluro.newTileByWMS(urlTemplate, sld, format, layers, layerName);
}

/**
 * @param {string} url URL template.      (必选)
 * @param {string} layers   图层名称      (必选)
 * @param {string} tileMatrixSet          (必选)
 * @param {string} tileMatrixid           (必选)
 * @param {string} format  example : 'image/png'
 * @param {string} zoomlevel  放大级数
 * @param {string} layerName   图层名
 * @return {ol.layer.Tile} return layer
 */
function newLocalTilesByWMTS(url, layers, tileMatrixSet, tileMatrixid, format, zoomlevel, layerName) {
    format = !format ? 'image/png' : format;
    var itelluro = new iTelluro();
    return itelluro.newTileByWMTS(url, layers, tileMatrixSet, tileMatrixid, format, zoomlevel, layerName);
}



