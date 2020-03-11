using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.LayerFieldApp.Dtos;
using InfoEarthFrame.GeoServerRest;
using InfoEarthFrame.GeoServerRest.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.LayerFieldApp
{
    public class GeoServerHelper
    {
        private string _GeoServerIp = string.Empty;
        private string _GeoServerPort = string.Empty;
        private GeoServer _GeoServer;
        private string _GeoWorkSpace = string.Empty;
        private string _GeoDataStore = string.Empty;
        private string _PostGisHost = string.Empty;
        private string _PostGisPort = string.Empty;
        private string _PostGisDB = string.Empty;
        private string _PostGisUser = string.Empty;
        private string _PostGisPwd = string.Empty;

        private string _gridSetName = string.Empty;
        private string _zoomStart = string.Empty;
        private string _zoomStop = string.Empty;
        private string _epsg = string.Empty;
        private int _ThreadCount = 1;

        public GeoServerHelper()
        {
            _GeoServerIp = ConfigurationManager.AppSettings["GeoServerIp"];
            _GeoServerPort = ConfigurationManager.AppSettings["GeoServerPort"];
            _GeoWorkSpace = ConfigurationManager.AppSettings["GeoWorkSpace"];
            _GeoDataStore = ConfigurationManager.AppSettings["GeoDataStore"];
            _PostGisHost = ConfigurationManager.AppSettings["PostGisHost"];
            _PostGisPort = ConfigurationManager.AppSettings["PostGisPort"];
            _PostGisDB = ConfigurationManager.AppSettings["PostGisDB"];
            _PostGisUser = ConfigurationManager.AppSettings["PostGisUser"];
            _PostGisPwd = ConfigurationManager.AppSettings["PostGisPwd"];
            _gridSetName = ConfigurationManager.AppSettings["GridSetName"];
            _zoomStart = ConfigurationManager.AppSettings["ZoomStart"];
            _zoomStop = ConfigurationManager.AppSettings["ZoomStop"];
            _epsg = ConfigurationManager.AppSettings["EPSG"];
            if (string.IsNullOrEmpty(_epsg))
            {
                _epsg = "EPSG:4326";
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ThreadCount"]))
            {
                _ThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCount"]);
            }
            _GeoServer = new GeoServer(_GeoServerIp, _GeoServerPort);
        }

        #region GeoServerRest
        /// <summary>
        /// 发布图层
        /// </summary>
        /// <param name="layerId">图层编号</param>
        /// <returns>true：发布成功，false：发布失败</returns>
        public bool PublicLayer(string layerName, string title, ListResultOutput<LayerFieldDto> listDto, string layerType)
        {
            if (!_GeoServer.IsExsitWorkSpace(_GeoWorkSpace))
            {
                _GeoServer.AddWorkSpace(_GeoWorkSpace);
            }
            if (!_GeoServer.IsExsitDataStore(_GeoWorkSpace, _GeoDataStore))
            {
                _GeoServer.AddDataStore(_GeoWorkSpace, _GeoDataStore, _PostGisHost, _PostGisPort, _PostGisDB, _PostGisUser, _PostGisPwd);
            }
            Dictionary<string, string> filedAndType = new Dictionary<string, string>();
            for (int i = 0; i < listDto.Items.Count; i++)
            {
                filedAndType.Add(listDto.Items[i].AttributeName, GetGeoFiledType(listDto.Items[i].AttributeTypeName));
            }
            filedAndType.Add("guid", GetGeoFiledType("VARCHAR"));
            filedAndType.Add("geom", GetGeoFiledType(layerType));
            if (!_GeoServer.IsExsitLayer(layerName, _GeoWorkSpace, _GeoDataStore))
            {
                _GeoServer.AddLayer(layerName, title, filedAndType, _GeoWorkSpace, _GeoDataStore, _epsg);
                _GeoServer.ModifyLayerStyle(layerName, new List<string>(), GetDefaultStyle(layerType));
            }
            return true;
        }

        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="dataStore">数据存储名称</param>
        /// <param name="layer">图层名称</param>
        /// <returns>true：删除成功，false：删除失败</returns>
        public bool DeleteLayer(string layer)
        {
            try
            {
                if (_GeoServer.IsExsitLayer(layer, _GeoWorkSpace, _GeoDataStore))
                {
                    return _GeoServer.DeleteLayer(_GeoWorkSpace, _GeoDataStore, layer);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private string GetGeoFiledType(string filedType)
        {
            switch (filedType.ToUpper())
            {
                case "LONG INTEGER":
                case "SHORT INTEGER":
                case "INT":
                case "BIGINT":
                    return "java.lang.Integer";

                case "DOUBLE":
                    return "java.lang.Double";
                case "FLOAT":
                case "NUMERIC":
                    return "java.math.BigDecimal";

                case "TEXT":
                case "VARCHAR":
                    return "java.lang.String";

                case "DATETIME":
                case "DATE":
                    return "java.util.Date";

                case "点":
                    return "com.vividsolutions.jts.geom.Point";
                case "Point":
                    return "com.vividsolutions.jts.geom.Point";

                case "线": 
                    return "com.vividsolutions.jts.geom.MultiLineString";
                case "Line":
                    return "com.vividsolutions.jts.geom.MultiLineString";

                case "面":
                    return "com.vividsolutions.jts.geom.MultiPolygon";
                case "Polygon":
                    return "com.vividsolutions.jts.geom.MultiPolygon";

                default:
                    break;
            }
            return "java.lang.String";
        }

        private string GetDefaultStyle(string layerType)
        {
            switch (layerType)
            {
                case "点":
                    return "point";
                case "Point":
                    return "point";
                case "线":
                    return "line";
                case "Line":
                    return "line";
                case "面":
                    return "polygon";
                case "Polygon":
                    return "polygon";
                default:
                    break;
            }
            return string.Empty;
        }

        public void UpdateStyle(string styleName, string styleContent)
        {
            if (string.IsNullOrEmpty(styleContent))
            {
                return;
            }
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = Path.Combine(path, styleName + ".sld");

            File.WriteAllText(filepath, styleContent, Encoding.GetEncoding("GB2312"));

            List<Style> style = _GeoServer.GetStyles(_GeoWorkSpace).ToList();
            if (style != null && style.Exists(t => t.Name == styleName))
            {
                _GeoServer.ModifyStyle(styleName, filepath, _GeoWorkSpace);
            }
            else
            {
                _GeoServer.AddStyle(styleName, Path.GetFileName(filepath), filepath, _GeoWorkSpace);
            }
        }


        public void DeleteStyle(string styleName)
        {
            List<Style> style = _GeoServer.GetStyles(_GeoWorkSpace).ToList();
            if (style != null && style.Exists(t => t.Name == styleName))
            {
                _GeoServer.DeleteStyle(styleName, _GeoWorkSpace);
            }
        }

        public void ModifyLayerGroup(string layerGroup, IEnumerable<string> targetLayers, IEnumerable<string> targetStyles)
        {
            _GeoServer.ModifyLayerGroup(layerGroup, targetLayers, targetStyles, _GeoWorkSpace);
        }

        public void ModifyLayerBBox(string layerName, string title, string bBox)
        {
            _GeoServer.ModifyLayerBBox(layerName, title, _GeoWorkSpace, _GeoDataStore, _epsg, bBox);
        }

        public bool AddLayerGroup(string mapName, IEnumerable<string> targetLayers, IEnumerable<string> targetStyles)
        {
            if (!_GeoServer.IsExsitWorkSpace(_GeoWorkSpace))
            {
                _GeoServer.AddWorkSpace(_GeoWorkSpace);
            }
            if (_GeoServer.IsExsitLayerGroup(mapName, _GeoWorkSpace))
            {
                _GeoServer.DeleteLayerGroup(mapName, _GeoWorkSpace);
            }
          return  _GeoServer.AddLayerGroup(mapName, targetLayers, targetStyles, _GeoWorkSpace);
        }

        public bool DeleteLayerGroup(string layerGroup)
        {
            if (_GeoServer.IsExsitLayerGroup(layerGroup, _GeoWorkSpace))
            {
                return _GeoServer.DeleteLayerGroup(layerGroup, _GeoWorkSpace);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 图层绑定切片规则
        /// </summary>
        /// <param name="layerName">图层名称（工作区:图层名称）</param>
        /// <param name="gridSetNames">切片规则</param>
        /// <returns></returns>
        public bool TileMap(string mapName, string bbox = null)
        {
            mapName = _GeoWorkSpace + ":" + mapName;
            List<string> list = _GeoServer.GetLayerGridSet(mapName).ToList();
            //_GeoServer.BindLayerGridSet(mapName, list);


            int start = 0;
            int stop = 14;
            if (int.TryParse(_zoomStart, out start) && int.TryParse(_zoomStop, out stop))
            {
                foreach (var l in list)
                {
                    if (!string.IsNullOrEmpty(_gridSetName))
                    {
                        string[] listgrid = _gridSetName.Split(',');
                        if (listgrid.Contains(l))
                        {
                            _GeoServer.SendEmptyTilesTask(mapName, l, start, stop, _ThreadCount, bbox);
                            _GeoServer.SendAllTilesTask(mapName, l, start, stop, _ThreadCount, bbox);
                        }
                    }
                    else
                    {
                        _GeoServer.SendEmptyTilesTask(mapName, l, start, stop, _ThreadCount, bbox);
                        _GeoServer.SendAllTilesTask(mapName, l, start, stop, _ThreadCount, bbox);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 清空缓存切片
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public bool EmptyTiles(string mapName)
        {
            mapName = _GeoWorkSpace + ":" + mapName;
            List<string> list = _GeoServer.GetLayerGridSet(mapName).ToList();

            int start = 0;
            int stop = 14;
            if (int.TryParse(_zoomStart, out start) && int.TryParse(_zoomStop, out stop))
            {
                foreach (var l in list)
                {
                    if (!string.IsNullOrEmpty(_gridSetName))
                    {
                        string[] listgrid = _gridSetName.Split(',');
                        if (listgrid.Contains(l))
                        {
                            _GeoServer.SendEmptyTilesTask(mapName, l, start, stop, _ThreadCount);
                        }
                    }
                    else
                    {
                        _GeoServer.SendEmptyTilesTask(mapName, l, start, stop, _ThreadCount);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 查询地图是否有切片任务
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public bool IsExistTilesTask(string mapName)
        {
            mapName = _GeoWorkSpace + ":" + mapName;
            var list = _GeoServer.GetAllTilesTask(mapName);
            if (list != null && list.Count() > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 结束所有任务
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public bool TerminatingTask(string mapName)
        {
            if (IsExistTilesTask(mapName))
            {
                mapName = _GeoWorkSpace + ":" + mapName;
                return _GeoServer.SendTerminatingTask(mapName, TaskState.All);
            }
            return true;
        }

        #endregion
    }
}
