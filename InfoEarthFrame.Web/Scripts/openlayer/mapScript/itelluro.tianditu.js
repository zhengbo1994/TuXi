/**
 * those are for pulling tiles from tianditu tiles source address, which would return a new layer or map that is access to openlayer3.
 * refer to http://openlayers.org
 */
function TianDiTu() {
    var _renderer = "dom"; //canvas,dom,webgl
    var _url = "http://t0.tianditu.com/DataServer";

    //
    // map

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
     * @param {string} layerName   Í¼²ãÃû
     * @return {ol.layer.Tile} return map
     */
    this.NewWebTilesMap = function (id, numLevels, dsEast, dsWest, dsSouth, dsNorth,
                                    sourcePackageKey, url, tileSize, zeroLevelSize, layerName) {

        layer1 = this.newWebLayer(url, sourcePackageKey, layerName);

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
     * @param {string} imageType picture file extensions.
     * @param {string} layerName   Í¼²ãÃû
     * @return {ol.layer.Tile} return map
     */
    this.NewLocalTilesMap = function (id, numLevels, dsEast, dsWest, dsSouth, dsNorth,
                                      root, imageType, layerName) {

        layer1 = this.newLocalTilesLayer(root, imageType, layerName);

        var view = new ol.View({
            center: [(dsEast + dsWest) / 2, (dsNorth + dsSouth) / 2], zoom: 0, minZoom: 0, maxZoom: numLevels,
            projection: 'EPSG:4326',
            maxResolution: 180 / 256
        });

        var map = new ol.Map({
            layers: [layer1],
            target: id,
            view: view,
            renderer: _renderer
        });

        return map;
    };

    //
    // layer

    /**
     * @param {string} root Local root directory of tiles' files.
     * @param {string} imageType picture file extensions.
     * @param {string} layerName   Í¼²ãÃû
     * @return {ol.layer.Tile} return layer
     */
    this.newLocalTilesLayer = function (root, imageType, layerName) {
        root += "/{z}/{y}/{y}_{x}." + imageType;
        return this.newLayer(root, true, layerName);
    };


    /**
     * @param {string} url URL address of tiles' provider.
     * @param {string} layerName   Í¼²ãÃû
     */
    this.newWebImageLayer = function (url, layerName) {
        if (!url || url == "") url = _url;
        return this.newWebLayer(url, "img_c", layerName);
    };

    /**
     * @param {string} url URL address of tiles' provider.
     * @param {string} layerName   Í¼²ãÃû
     */
    this.newWebVectorLayer = function (url, layerName) {
        if (!url || url == "") url = _url;
        return this.newWebLayer(url, "vec_c", layerName);
    };

    /**
     * @param {string} url URL address of tiles' provider.
     * @param {string} layerName   Í¼²ãÃû
     */
    this.newWebVectorLabelLayer = function (url, layerName) {
        if (!url || url == "") url = _url;
        return this.newWebLayer(url, "cva_c", layerName);
    };

    /**
     * @param {string} url URL address of tiles' provider.
     * @param {string} layerName   Í¼²ãÃû
     */
    this.newWebImageLabelLayer = function (url, layerName) {
        if (!url || url == "") url = _url;
        return this.newWebLayer(url, "cia_c", layerName);
    };

    /**
     * @param {string} url URL address of tiles' provider.
     * @param {string} key tile type, vec_c vector, cva_c vector-label,img_c,cia_c  image-label
     * @param {string} layerName   Í¼²ãÃû
     */
    this.newWebLayer = function (url, key, layerName) {
        var tmp = (!key || key == "") ? "L={z}&X={x}&Y={y}" : "T=" + key + "&L={z}&X={x}&Y={y}";
        url += (url.indexOf("?") > 0) ? "&" : "?";
        url += tmp;
        return this.newLayer(url, false, layerName);
    };

    /**
     * @param {string} urlTemplate URL template. Must include {x}, {y} or {-y}.
     * @param {bool} local, true local tiles, false web tiles.
     * @param {string} layerName   Í¼²ãÃû
     * @example label http://t0.tianditu.com/DataServer?T=cva_c&x={x}&y={y}&l={z}
     * @example vector http://t0.tianditu.com/DataServer?T=vec_c&x={x}&y={y}&l={z}
     */
    this.newLayer = function (urlTemplate, local, layerName) {
        function TianDiTuUrlCallBack(tileCoord, pixelRatio, projection) {
            var z = tileCoord[0];
            var x = tileCoord[1];
            var y = -tileCoord[2]-1;

            var url = urlTemplate.replace('{z}', z.toString())
                .replace('{y}', y.toString())
                .replace('{x}', x.toString());
            return url;
        }

        function PaddingZero(num) {
            if (num < 1000) {
                var xStr4 = "0000" + num.toString();
                return xStr4.substr(xStr4.length - 4, 4);
            }

            return num.toString();
        }


        function TianDiTuLocalCallBack(tileCoord, pixelRatio, projection) {
            var zStr4 = tileCoord[0];
            var xStr4 = PaddingZero(tileCoord[1]);
            var yStr4 = PaddingZero(-tileCoord[2]);

            var url = urlTemplate.replace('{z}', zStr4)
                .replace('{y}', yStr4)
                .replace('{y}', yStr4)
                .replace('{x}', xStr4)
                .replace('{x}', xStr4);
            //console.log(url);
            return url
        }

        var tilesize = 256;
        var resolutions = new Array(22);
        for (var i = 0, ii = resolutions.length; i < ii; ++i) {
            resolutions[i] = 360 / Math.pow(2, i) / tilesize;
        }
        var layer = new ol.layer.Tile({
            source: new ol.source.XYZ({
                tileSize: tilesize,
                //url: urlTemplate,
                tileUrlFunction: local ? TianDiTuLocalCallBack : TianDiTuUrlCallBack,
                projection: 'EPSG:4326',
                tileGrid: new ol.tilegrid.TileGrid({
                    resolutions: resolutions,
                    tileSize: tilesize,
                    origin: [-180, 90]
                    //extent: [-180, -45, 180, 75]
                })
            }),
            name: !!layerName ? layerName : ''
        });


        return layer;
    };

}
