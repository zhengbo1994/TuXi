using InfoEarthFrame.Common;
using InfoEarthFrame.GeoServerRest.Model;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InfoEarthFrame.GeoServerRest
{
    public class GeoServer
    {
        public string GEOSERVER_USERNAME = "admin";
        public string GEOSERVER_PASSWORD = "geoserver";

        private readonly IDictionary<RequestMethod, string> _RequestMethodLookup = new Dictionary<RequestMethod, string>(){{RequestMethod.Delete, "DELETE"},
                                                                                                                          {RequestMethod.Get, "GET"},
                                                                                                                          {RequestMethod.Post, "POST"},
                                                                                                                          {RequestMethod.Put, "PUT"}};
        private readonly IDictionary<ContentType, string> _ContentTypeLookup = new Dictionary<ContentType, string>(){{ContentType.Json, "application/json"},
                                                                                                                     {ContentType.Xml, "text/xml"},
                                                                                                                     {ContentType.Html, "text/html"},
                                                                                                                     {ContentType.SldFile, "application/vnd.ogc.sld+xml" },
                                                                                                                     {ContentType.Zip, "application/zip"}};
        private readonly IDictionary<AcceptType, string> _AcceptTypeLookup = new Dictionary<AcceptType, string>(){{AcceptType.Json, "application/json"},
                                                                                                                     {AcceptType.Xml, "text/xml"},
                                                                                                                     {AcceptType.Html, "text/html"},
                                                                                                                     {AcceptType.Byte, "application/octet-stream"}};
        private readonly IDictionary<SrsType, string> _SrsLookup = new Dictionary<SrsType, string>()
        {
            {SrsType.Epsg4326, "EPSG:4326"},
            {SrsType.EPSG404000, "EPSG:404000"}
        };

        private readonly IDictionary<TaskState, string> _TaskStateLookup = new Dictionary<TaskState, string>()
        {
            {TaskState.Running, "running"},
            {TaskState.Pending, "pending"},
            {TaskState.All, "all"}
        };

        private static readonly ILog logger = LogManager.GetLogger(typeof(GeoServer));
        public string ServerIP { get; set; }
        public string ServerPort { get; set; }

        public GeoServer(string serverIP, string serverPort)
        {
            ServerIP = serverIP;
            ServerPort = serverPort;
        }

        public GeoServer(string serverIP, string serverPort, string serverUser, string serverPwd)
        {
            ServerIP = serverIP;
            ServerPort = serverPort;
            GEOSERVER_USERNAME = serverUser;
            GEOSERVER_PASSWORD = serverPwd;
        }

        #region 工作区
        /// <summary>
        /// 工作区是否存在
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <returns></returns>
        public bool IsExsitWorkSpace(string workSpace)
        {
            var list = GetWorkSpaces();
            foreach (var item in list)
            {
                if (item.Name == workSpace)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取所有工作区
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkSpace> GetWorkSpaces()
        {
            string requestString = RestServiceUrl + "/workspaces";
            object workSpaceResponse = null;
            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestString, RequestMethod.Get, typeof(WorkSpacesResponse), ref workSpaceResponse, ContentType.Json, AcceptType.Json);
            }
            catch (Exception ex)
            {
                throw new Exception("获取工作区异常！", ex);
            }

            if (workSpaceResponse is WorkSpacesResponse)
            {
                foreach (WorkSpace workspace in (workSpaceResponse as WorkSpacesResponse).WorkSpaces)
                {
                    workspace.GeoServer = this;
                    yield return workspace;
                }
            }
        }
        /// <summary>
        /// 获取默认工作区
        /// </summary>
        /// <returns></returns>
        public WorkSpace GetDefaultWorkSpace()
        {
            string requestString = RestServiceUrl + "/workspaces/default";
            object defaultWorkSpace = null;
            string status = String.Empty;

            try
            {
                status = SendRestRequest(requestString, RequestMethod.Get, typeof(DefaultWorkSpaceResponse), ref defaultWorkSpace, ContentType.Json, AcceptType.Json);
            }
            catch (Exception ex)
            {
                throw new Exception("获取默认工作区异常！", ex);
            }

            WorkSpace wksp = (defaultWorkSpace is DefaultWorkSpaceResponse) ? (defaultWorkSpace as DefaultWorkSpaceResponse).WorkSpace : null;
            if (wksp != null)
            {
                wksp.GeoServer = this;
            }
            return wksp;
        }
        /// <summary>
        /// 添加工作区
        /// </summary>
        /// <param name="workSpaceName">工作区名称</param>
        public bool AddWorkSpace(string workSpaceName)
        {
            string requestUrl = RestServiceUrl + "/workspaces";
            object newWorkspace = new WorkSpaceRequest(workSpaceName);

            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(WorkSpaceRequest), ref newWorkspace, ContentType.Json, AcceptType.Json);
                if (status == "Created")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("添加工作区失败，可能是工作区名称已存在引起的！", ex);
            }
        }
        /// <summary>
        /// 删除工作区
        /// </summary>
        /// <param name="workSpaceName">工作区名称</param>
        public bool DeleteWorkSpace(string workSpaceName)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpaceName + "?recurse=true";
            object workspaceRequest = null;

            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Delete, typeof(WorkSpaceRequest), ref workspaceRequest, ContentType.Json, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("删除工作区失败！", ex);
            }
        }
        #endregion

        #region 栅格数据
        public bool IsExsitCoverageStore(string workSpace, string coverStore)
        {
            var list = GetCoverageStores(workSpace);
            foreach (var item in list)
            {
                if (item.Name == coverStore)
                    return true;
            }
            return false;
        }

        public IEnumerable<CoverageStore> GetCoverageStores(string workSpace)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/coveragestores";
            object dataStoresResponse = null;
            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Get, typeof(CoverageStoreResponse), ref dataStoresResponse, ContentType.Json, AcceptType.Json);
            }
            catch (Exception ex)
            {
                throw new Exception("获取数据存储异常！", ex);
            }

            if (dataStoresResponse is CoverageStoreResponse)
            {
                foreach (CoverageStore store in (dataStoresResponse as CoverageStoreResponse).CoverageStores)
                {
                    yield return store;
                }
            }
        }

        public bool PutCoverageStore(string workSpace, string coverName, string tifPath)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/coveragestores/" + coverName + "/" + "file.geotiff";
            string status = string.Empty;

            object newDataStore = tifPath;
            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Put, typeof(string), ref newDataStore, tifPath.EndsWith(".zip") ? ContentType.Zip : ContentType.SldFile, AcceptType.Json);
                if (status == "Created")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("添加数据存储异常！", ex);
            }
        }

        public bool DeleteCoverageStore(string workSpace, string coverName)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/coveragestores/" + coverName + "?recurse=true";
            object request = null;

            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Delete, typeof(Object), ref request, ContentType.Json, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("删除数据存储异常！", ex);
            }
        }

        public bool UpdateStoreTitle(string workSpace, string coveragestoreName, string coverName, string title)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/coveragestores/" + coveragestoreName + "/coverages/" + coverName;
            string status = string.Empty;

            object newDataStore = "<coverage>";
            newDataStore += "<title>" + title + "</title>";
            newDataStore += "<parameters>";
            if (string.IsNullOrEmpty(ConfigHelper.TIFF_RemoveBackGroundColor))
            {
                //newDataStore += string.Format("<entry><string>InputTransparentColor</string><string>{0}</string></entry>", ConfigHelper.TIFF_RemoveBackGroundColor);
            }
            else
            {
                newDataStore += string.Format("<entry><string>InputTransparentColor</string><string>{0}</string></entry>", ConfigHelper.TIFF_RemoveBackGroundColor);
            }
            newDataStore += "<entry><string>SUGGESTED_TILE_SIZE</string><string>512,512</string></entry>";
            newDataStore += "</parameters>";
            newDataStore += "</coverage>";
            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Put, typeof(string), ref newDataStore, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("修改数据存储异常！", ex);
            }
        }

        public ConverageRequest GetConverageInfo(string workSpace, string coveragestoreName, string coverName)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/coveragestores/" + coveragestoreName + "/coverages/" + coverName;
            string status = string.Empty;
            object dataStoresResponse = null;
            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Get, typeof(string), ref dataStoresResponse, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    if (dataStoresResponse!=null)
                    {
                        var data = JsonConvert.DeserializeObject<ConverageRequest>(dataStoresResponse.ToString());
                        if (data != null && data.Coverage != null && !string.IsNullOrWhiteSpace(data.Coverage.Name))
                        {
                            return data;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("修改数据存储异常！", ex);
            }
        }
        #endregion

        #region 数据存储
        /// <summary>
        /// 数据存储是否存在
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="dataStore">数据存储名称</param>
        /// <returns></returns>
        public bool IsExsitDataStore(string workSpace, string dataStore)
        {
            var list = GetDataStores(workSpace);
            foreach (var item in list)
            {
                if (item.Name == dataStore)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取数据存储
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <returns></returns>
        public IEnumerable<DataStore> GetDataStores(string workSpace)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/datastores";
            object dataStoresResponse = null;
            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Get, typeof(DataStoresResponse), ref dataStoresResponse, ContentType.Json, AcceptType.Json);
            }
            catch (Exception ex)
            {
                throw new Exception("获取数据存储异常！", ex);
            }

            if (dataStoresResponse is DataStoresResponse)
            {
                foreach (DataStore store in (dataStoresResponse as DataStoresResponse).DataStores)
                {
                    yield return store;
                }
            }
        }
        /// <summary>
        /// 添加数据存储
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="storeName">数据存储名称</param>
        /// <param name="host">数据库主机地址</param>
        /// <param name="port">数据库端口号</param>
        /// <param name="dataBase">数据库名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="userPassword">密码</param>
        public bool AddDataStore(string workSpace, string storeName, string host, string port, string dataBase, string userName, string userPassword)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/datastores";
            string status = string.Empty;
            //object newDataStore = new DataStoreRequest(workSpace, storeName, host, port, dataBase, userName, userPassword, "postgis");

            object newDataStore = "<dataStore><name>" + storeName + "</name><connectionParameters>[Parameters]</connectionParameters></dataStore>";
            StringBuilder parameters = new StringBuilder();
            parameters.Append("<host>");
            parameters.Append(host);
            parameters.Append("</host>");
            parameters.Append("<port>");
            parameters.Append(port);
            parameters.Append("</port>");
            parameters.Append("<database>");
            parameters.Append(dataBase);
            parameters.Append("</database>");
            parameters.Append("<schema>public</schema>");
            parameters.Append("<user>");
            parameters.Append(userName);
            parameters.Append("</user>");
            parameters.Append("<passwd>");
            parameters.Append(userPassword);
            parameters.Append("</passwd>");
            parameters.Append("<dbtype>postgis</dbtype>");

            newDataStore = (newDataStore as string).Replace("[Parameters]", parameters.ToString());
            try
            {
                //status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(DataStoreRequest), ref newDataStore, ContentType.Json, AcceptType.Json);
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(string), ref newDataStore, ContentType.Html, AcceptType.Json);
                if (status == "Created")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("添加数据存储异常！", ex);
            }
        }
        /// <summary>
        /// 删除数据存储
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="dataStore">数据存储名称</param>
        /// <returns></returns>
        public bool DeleteDataStore(string workSpace, string dataStore)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/datastores/" + dataStore + "?recurse=true";
            object request = null;

            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Delete, typeof(Object), ref request, ContentType.Json, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("删除数据存储异常！", ex);
            }
        }
        #endregion

        #region 图层组
        /// <summary>
        /// 图层组是否存在
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="layerGroup">图层组名称</param>
        /// <returns></returns>
        public bool IsExsitLayerGroup(string layerGroup, string workSpace = null)
        {
            var list = GetLayerGroups(workSpace);
            foreach (var item in list)
            {
                if (item.Name == layerGroup)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取图层组
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <returns></returns>
        public IEnumerable<LayerGroup> GetLayerGroups(string workSpace = null)
        {
            string layerGroupPath = string.IsNullOrEmpty(workSpace) ? "/layergroups" : "/workspaces/" + workSpace + "/layergroups";
            string requestUrl = RestServiceUrl + layerGroupPath;
            object layerGroups = null;
            string status = string.Empty;
            Object temp = null;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Get, typeof(LayerGroupsResponse), ref layerGroups, ContentType.Json, AcceptType.Json);
                if (status == "OK")
                {
                    if (layerGroups is LayerGroupsResponse)
                    {
                        List<LayerGroup> list = new List<LayerGroup>();
                        string newRequestUrl = string.Empty;
                        foreach (LayerGroup layerGroup in (layerGroups as LayerGroupsResponse).LayerGroups)
                        {
                            newRequestUrl = requestUrl + "/" + layerGroup.Name;
                            string response = GetRestResponse(newRequestUrl, RequestMethod.Get, typeof(LayerGroupResponse), ref temp, ContentType.Json, AcceptType.Json);
                            response = response.Replace("\"null\"", null);
                            LayerGroupResponse groupResponse = JsonConvert.DeserializeObject<LayerGroupResponse>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                            if (groupResponse != null)
                            {
                                list.Add(groupResponse.LayerGroup);
                            }
                        }
                        return list;
                    }
                }
                return new List<LayerGroup>();
            }
            catch (Exception ex)
            {
                throw new Exception("获取图层组异常！", ex);
            }
        }

        /// <summary>
        /// 添加图层组
        /// </summary>
        /// <param name="layerGroupName">图层组名称</param>
        /// <param name="targetLayers">图层</param>
        /// <param name="targetStyles">样式</param>
        /// <param name="workSpace">工作区名称</param>
        public bool AddLayerGroup(string layerGroupName, IEnumerable<string> targetLayers, IEnumerable<string> targetStyles, string workSpace = null)
        {
            string layerGroupPath = string.IsNullOrEmpty(workSpace) ? "/layergroups" : "/workspaces/" + workSpace + "/layergroups";
            string requestUrl = RestServiceUrl + layerGroupPath;

            string status = string.Empty;

            Object requestPayload = "<layerGroup><name>" + layerGroupName + "</name><title>" + layerGroupName + "</title><layers>[LAYERS]</layers><styles>[STYLES]</styles></layerGroup>";
            StringBuilder layers = new StringBuilder();
            foreach (string targetLayer in targetLayers)
            {
                layers.Append("<layer>");
                layers.Append(targetLayer);
                layers.Append("</layer>");
            }
            StringBuilder styles = new StringBuilder();
            if (targetStyles != null)
            {
                foreach (string targetStyle in targetStyles)
                {
                    styles.Append("<style>");
                    styles.Append(targetStyle);
                    styles.Append("</style>");
                }
            }

            requestPayload = (requestPayload as String).Replace("[LAYERS]", layers.ToString());
            requestPayload = (requestPayload as String).Replace("[STYLES]", styles.ToString());

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(String), ref requestPayload, ContentType.Html, AcceptType.Json);
                if (status == "Created")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("添加图层组异常！", ex);
            }
        }
        /// <summary>
        /// 删除图层组
        /// </summary>
        /// <param name="layerGroup">图层组名称</param>
        /// <param name="workSpace">工作区名称</param>
        public bool DeleteLayerGroup(string layerGroup, string workSpace = null)
        {
            string layerGroupPath = string.IsNullOrEmpty(workSpace) ? "/layergroups/" + layerGroup : "/workspaces/" + workSpace + "/layergroups/" + layerGroup;
            string requestUrl = RestServiceUrl + layerGroupPath;
            Object obj = null;
            string status = String.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Delete, typeof(Object), ref obj, ContentType.Json, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("删除图层组异常！", ex);
            }
        }
        /// <summary>
        /// 修改图层组
        /// </summary>
        /// <param name="layerGroup">图层组名称</param>
        /// <param name="targetLayers">图层</param>
        /// <param name="workSpace">工作区名称</param>
        public bool ModifyLayerGroup(string layerGroup, IEnumerable<string> targetLayers, IEnumerable<string> targetStyles, string workSpace = null)
        {
            string layerGroupPath = string.IsNullOrEmpty(workSpace) ? "/layergroups/" + layerGroup : "/workspaces/" + workSpace + "/layergroups/" + layerGroup;
            string requestUrl = RestServiceUrl + layerGroupPath;

            string status = string.Empty;

            object requestPayload = "<layerGroup><name>" + layerGroup + "</name><layers>[LAYERS]</layers><styles>[STYLES]</styles></layerGroup>";
            StringBuilder layers = new StringBuilder();
            StringBuilder styles = new StringBuilder();
            foreach (string targetLayer in targetLayers)
            {
                layers.Append("<layer>");
                layers.Append(targetLayer);
                layers.Append("</layer>");
            }
            if (targetStyles != null)
            {
                foreach (string targetStyle in targetStyles)
                {
                    styles.Append("<style>");
                    styles.Append(targetStyle);
                    styles.Append("</style>");
                }
            }

            requestPayload = (requestPayload as String).Replace("[LAYERS]", layers.ToString());
            requestPayload = (requestPayload as String).Replace("[STYLES]", styles.ToString());

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Put, typeof(string), ref requestPayload, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("修改图层组异常！", ex);
            }
        }
        #endregion

        #region 图层
        /// <summary>
        /// 图层是否存在
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="layer">图层名称</param>
        /// <returns></returns>
        public bool IsExsitLayer(string layer, string workSpace, string dataStore)
        {
            var list = dataStore == null ? GetLayers(workSpace) : GetLayers(workSpace, dataStore);
            foreach (var item in list)
            {
                if (item.Name == layer)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="workSpaceName">工作区名称</param>
        /// <returns></returns>
        public IEnumerable<Layer> GetLayers(string workSpaceName)
        {
            //数据存储列表
            IEnumerable<DataStore> dataStores = GetDataStores(workSpaceName);
            IList<Layer> layers = new List<Layer>();

            foreach (DataStore dataStore in dataStores)
            {
                foreach (Layer layer in GetLayers(workSpaceName, dataStore.Name))
                {
                    layers.Add(layer);
                }
            }

            return layers;
        }
        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="dataStore">数据存储名称</param>
        /// <returns></returns>
        public IEnumerable<Layer> GetLayers(string workSpace, string dataStore)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/datastores/" + dataStore + "/featuretypes";
            object layers = null;
            string status = string.Empty;
            object temp = null;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Get, typeof(LayersResponse), ref layers, ContentType.Json, AcceptType.Json);
                if (status == "OK")
                {
                    if (layers is LayersResponse)
                    {
                        List<Layer> list = new List<Layer>();
                        string newRequestUrl = string.Empty;
                        foreach (Layer layer in (layers as LayersResponse).Layers)
                        {
                            try
                            {
                                newRequestUrl = requestUrl + "/" + layer.Name;
                                string response = GetRestResponse(newRequestUrl, RequestMethod.Get, typeof(LayerResponse), ref temp, ContentType.Json, AcceptType.Json);
                                response = response.Replace("\"null\"", null);
                                LayerResponse layerResponse = JsonConvert.DeserializeObject<LayerResponse>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                                if (layerResponse != null)
                                {
                                    list.Add(layerResponse.Layer);
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        return list;
                    }
                }
                return new List<Layer>();
            }
            catch (Exception ex)
            {
                throw new Exception("获取图层异常！", ex);
            }
        }
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="filedAndType">图层字段名称及类型</param>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="dataStore">数据存储名称</param>
        /// <param name="desiredSrs">编码</param>
        public bool AddLayer(string layerName, string title, Dictionary<string, string> filedAndType, string workSpace, string dataStore, string desiredSrs, string bBox = null)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/datastores/" + dataStore + "/featuretypes";
            string status = string.Empty;
            //object newLayer = new LayerRequest(layerName, _SrsLookup[desiredSrs]);
            object newLayer = "<featureType><name>" + layerName + "</name><nativeName>" + layerName + "</nativeName><title>" + title + "</title>[BBOX]<srs>" + desiredSrs + "</srs><attributes>[Parameters]</attributes></featureType>";
            StringBuilder parameters = new StringBuilder();
            foreach (KeyValuePair<string, string> item in filedAndType)
            {
                parameters.Append("<attribute>");
                parameters.Append("<name>" + item.Key + "</name>");
                parameters.Append("<binding>" + item.Value + "</binding>");
                parameters.Append("</attribute>");
            }
            #region old
            //parameters.Append("<attribute>");
            //parameters.Append("<name>objectid</name>");
            //parameters.Append("<binding>java.lang.Integer</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>area</name>");
            //parameters.Append("<binding>java.math.BigDecimal</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>perimeter</name>");
            //parameters.Append("<binding>java.math.BigDecimal</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>bou12_</name>");
            //parameters.Append("<binding>java.lang.Integer</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>bou12_id</name>");
            //parameters.Append("<binding>java.lang.Integer</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>code</name>");
            //parameters.Append("<binding>java.lang.Integer</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>name</name>");
            //parameters.Append("<binding>java.lang.String</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>shape_leng</name>");
            //parameters.Append("<binding>java.math.BigDecimal</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>shape_area</name>");
            //parameters.Append("<binding>java.math.BigDecimal</binding>");
            //parameters.Append("</attribute>");
            //parameters.Append("<attribute>");
            //parameters.Append("<name>geom</name>");
            //parameters.Append("<binding>com.vividsolutions.jts.geom.MultiPolygon</binding>");
            //parameters.Append("</attribute>");
            #endregion

            newLayer = (newLayer as string).Replace("[Parameters]", parameters.ToString());
            string bbox = string.Empty;
            if (string.IsNullOrEmpty(bBox))
            {
                bbox = string.Format("<nativeBoundingBox><minx>{0}</minx><maxx>{1}</maxx><miny>{2}</miny><maxy>{3}</maxy></nativeBoundingBox><latLonBoundingBox><minx>{0}</minx><maxx>{1}</maxx><miny>{2}</miny><maxy>{3}</maxy><crs>{4}</crs></latLonBoundingBox>", -1, 0, -1, 0, desiredSrs);
            }
            else
            {
                string[] list = bBox.Split(',');
                bbox = string.Format("<nativeBoundingBox><minx>{0}</minx><maxx>{1}</maxx><miny>{2}</miny><maxy>{3}</maxy></nativeBoundingBox><latLonBoundingBox><minx>{0}</minx><maxx>{1}</maxx><miny>{2}</miny><maxy>{3}</maxy><crs>{4}</crs></latLonBoundingBox>", list[0], list[1], list[2], list[3], desiredSrs);
            }
            newLayer = (newLayer as string).Replace("[BBOX]", string.Empty);

            try
            {
                //status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(LayerRequest), ref newLayer, ContentType.Json, AcceptType.Json);
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(String), ref newLayer, ContentType.Html, AcceptType.Json);
                logger.Debug("STATUS:" + status);
                if (status == "Created")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                
                throw new Exception("添加图层异常！", ex);
            }
        }
        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="dataStore">数据存储名称</param>
        /// <param name="layer">图层名称</param>
        /// <returns></returns>
        public bool DeleteLayer(string workSpace, string dataStore, string layer)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/datastores/" + dataStore + "/featuretypes/" + layer + "?recurse=true";
            Object request = null;

            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Delete, typeof(Object), ref request, ContentType.Json, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("删除图层异常！", ex);
            }
        }
        /// <summary>
        /// 修改图层样式
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="styles">样式</param>
        /// <returns></returns>
        public bool ModifyLayerStyle(string layerName, IEnumerable<string> styles, string defaultStyle = null)
        {
            string requestUrl = RestServiceUrl + "/layers/" + layerName;
            string status = string.Empty;
            object newLayer = "<layer><name>" + layerName + "</name><type>VECTOR</type>[DEFAULTSTYLE]<styles class=\"linked-hash-set\">[STYLES]</styles><resource class=\"featureType\"><name>" + layerName + "</name></resource></layer>";
            StringBuilder parameters = new StringBuilder();
            foreach (string item in styles)
            {
                parameters.Append("<style>");
                parameters.Append("<name>" + item + "</name>");
                parameters.Append("</style>");
            }

            newLayer = (newLayer as string).Replace("[STYLES]", parameters.ToString());
            if (!string.IsNullOrEmpty(defaultStyle))
            {
                newLayer = (newLayer as string).Replace("[DEFAULTSTYLE]", "<defaultStyle><name>" + defaultStyle + "</name></defaultStyle>");
            }
            else
            {
                newLayer = (newLayer as string).Replace("[DEFAULTSTYLE]", "");
            }
            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Put, typeof(String), ref newLayer, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("修改图层样式异常！", ex);
            }
        }

        /// <summary>
        /// 修改图层边界范围
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="dataStore">数据存储名称</param>
        /// <param name="desiredSrs">参考坐标系</param>
        /// <param name="bBox">边界范围（格式：minX,maxX,minY,maxY）</param>
        /// <returns></returns>
        public bool ModifyLayerBBox(string layerName, string title, string workSpace, string dataStore, string desiredSrs, string bBox)
        {
            string requestUrl = RestServiceUrl + "/workspaces/" + workSpace + "/datastores/" + dataStore + "/featuretypes/" + layerName;
            string status = string.Empty;
            object layer = "<featureType><name>" + layerName + "</name><nativeName>" + layerName + "</nativeName><title>" + title + "</title>[BBOX]<srs>" + desiredSrs + "</srs></featureType>";

            string bbox = string.Empty;
            if (string.IsNullOrEmpty(bBox))
            {
                bbox = string.Format("<nativeBoundingBox><minx>{0}</minx><maxx>{2}</maxx><miny>{1}</miny><maxy>{3}</maxy></nativeBoundingBox><latLonBoundingBox><minx>{0}</minx><maxx>{2}</maxx><miny>{1}</miny><maxy>{3}</maxy><crs>{4}</crs></latLonBoundingBox>", -1, -1, 0, 0, desiredSrs);
            }
            else
            {
                string[] list = bBox.Split(',');
                bbox = string.Format("<nativeBoundingBox><minx>{0}</minx><maxx>{2}</maxx><miny>{1}</miny><maxy>{3}</maxy></nativeBoundingBox><latLonBoundingBox><minx>{0}</minx><maxx>{2}</maxx><miny>{1}</miny><maxy>{3}</maxy><crs>{4}</crs></latLonBoundingBox>", list[0], list[1], list[2], list[3], desiredSrs);
            }
            layer = (layer as string).Replace("[BBOX]", bbox);

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Put, typeof(string), ref layer, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("修改图层边界范围异常！", ex);
            }
        }
        #endregion

        #region 样式
        /// <summary>
        /// 样式是否存在
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="styleName">样式名称</param>
        /// <returns></returns>
        public bool IsExsitStyle(string styleName, string workSpace = null)
        {
            var list = GetStyles(workSpace);
            foreach (var item in list)
            {
                if (item.Name == styleName)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取样式
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <returns></returns>
        public IEnumerable<Style> GetStyles(string workSpace = null)
        {
            string stylePath = string.IsNullOrEmpty(workSpace) ? "/styles" : "/workspaces/" + workSpace + "/styles";
            string requestUrl = RestServiceUrl + stylePath;
            object styles = null;
            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Get, typeof(StylesResponse), ref styles, ContentType.Json, AcceptType.Json);
            }
            catch (Exception ex)
            {
                throw new Exception("获取样式异常！", ex);
            }

            if (styles is StylesResponse)
            {
                foreach (Style style in (styles as StylesResponse).Styles)
                {
                    yield return style;
                }
            }
        }
        /// <summary>
        /// 添加样式
        /// </summary>
        /// <param name="styleName">样式名称</param>
        /// <param name="sldName">样式文件名称</param>
        /// <param name="sldPath">样式文件绝对路径</param>
        /// <param name="workSpace">工作区名称</param>
        public bool AddStyle(string styleName, string sldName, string sldPath, string workSpace = null)
        {
            string stylePath = string.IsNullOrEmpty(workSpace) ? "/styles" : "/workspaces/" + workSpace + "/styles";
            string requestUrl = RestServiceUrl + stylePath;
            string status = string.Empty;
            object requestPayload = "<style><name>" + styleName + "</name><filename>" + sldName + "</filename></style>";

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(string), ref requestPayload, ContentType.Html, AcceptType.Json);
                if (status == "Created")
                {
                    requestUrl += "/" + styleName;
                    requestPayload = sldPath;

                    status = SendRestRequest(requestUrl, RequestMethod.Put, typeof(string), ref requestPayload, ContentType.SldFile, AcceptType.Json);
                    if (status == "OK")
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("添加样式异常！", ex);
            }
        }

        /// <summary>
        /// 修改样式
        /// </summary>
        /// <param name="styleName">样式名称</param>
        /// <param name="sldPath">样式文件绝对路径</param>
        /// <param name="workSpace">工作区名称</param>
        public bool ModifyStyle(string styleName, string sldPath, string workSpace = null)
        {
            string stylePath = string.IsNullOrEmpty(workSpace) ? "/styles" : "/workspaces/" + workSpace + "/styles";
            string requestUrl = RestServiceUrl + stylePath + "/" + styleName;
            string status = string.Empty;
            object requestPayload = sldPath;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Put, typeof(string), ref requestPayload, ContentType.SldFile, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("修改样式异常！", ex);
            }
        }

        /// <summary>
        /// 删除样式
        /// </summary>
        /// <param name="style">样式名称</param>
        /// <param name="workSpace">工作区名称</param>
        public bool DeleteStyle(string style, string workSpace = null)
        {
            string stylePath = string.IsNullOrEmpty(workSpace) ? "/styles/" + style : "/workspaces/" + workSpace + "/styles/" + style;
            string requestUrl = RestServiceUrl + stylePath;
            Object obj = null;
            string status = String.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Delete, typeof(Object), ref obj, ContentType.Json, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("删除样式异常！", ex);
            }
        }
        #endregion

        #region 切片
        /// <summary>
        /// 图层绑定切片规则
        /// </summary>
        /// <param name="layerName">图层名称（工作区:图层名称）</param>
        /// <param name="gridSetNames">切片规则</param>
        /// <returns></returns>
        public bool BindLayerGridSet(string layerName, IEnumerable<string> gridSetNames)
        {
            string requestUrl = BaseServiceUrl + "/gwc/rest/layers/" + layerName + ".xml";
            string status = string.Empty;
            object newGridSet = "<GeoServerLayer><enabled>true</enabled><name>" + layerName + "</name><mimeFormats><string>image/png</string><string>image/jpeg</string></mimeFormats><gridSubsets>[GRIDSET]</gridSubsets><metaWidthHeight><int>4</int><int>4</int></metaWidthHeight><expireCache>0</expireCache><expireClients>0</expireClients><parameterFilters><styleParameterFilter><key>STYLES</key><defaultValue/></styleParameterFilter></parameterFilters><gutter>0</gutter></GeoServerLayer>";

            StringBuilder gridSets = new StringBuilder();

            if (gridSetNames == null || gridSetNames.Count() == 0)
            {
                return false;
            }
            else
            {
                foreach (string gridSetName in gridSetNames)
                {
                    gridSets.Append("<gridSubset><gridSetName>");
                    gridSets.Append(gridSetName);
                    gridSets.Append("</gridSetName></gridSubset>");
                }
            }
            newGridSet = (newGridSet as string).Replace("[GRIDSET]", gridSets.ToString());
            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(string), ref newGridSet, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("绑定图层切片规则异常！", ex);
            }
        }

        /// <summary>
        /// 获取图层所有绑定的切片规则
        /// </summary>
        /// <param name="layerName">图层名称（工作区:图层名称）</param>
        public IEnumerable<string> GetLayerGridSet(string layerName)
        {
            string requestUrl = BaseServiceUrl + "/gwc/rest/layers/" + layerName + ".xml";
            object gridSetResponse = null;
            string status = string.Empty;

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Get, typeof(string), ref gridSetResponse, ContentType.Html, AcceptType.Json);
            }
            catch (Exception ex)
            {
                throw new Exception("获取图层切片规则异常！", ex);
            }
            if (gridSetResponse == null)
            {
                yield return null;
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(gridSetResponse.ToString());
                XmlNodeList nodeList = doc.SelectNodes("/GeoServerLayer/gridSubsets/gridSubset");
                if (nodeList != null)
                {
                    foreach (XmlNode item in nodeList)
                    {
                        yield return item.ChildNodes[0].InnerText;
                    }
                }
            }
        }

        /// <summary>
        /// 获取图层所有的切片任务编号
        /// </summary>
        /// <param name="layerName">图层名称（工作区:图层名称）</param>
        /// <returns></returns>
        public IEnumerable<string> GetAllTilesTask(string layerName)
        {
            string requestUrl = BaseServiceUrl + "/gwc/rest/seed/" + layerName + ".xml";
            string status = string.Empty;
            object payload = "";

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Get, typeof(string), ref payload, ContentType.Html, AcceptType.Html);
            }
            catch (Exception ex)
            {
                throw new Exception("发送切片任务异常！", ex);
            }
            if (payload != null)
            {
                JObject json = JsonConvert.DeserializeObject(payload as string) as JObject;
                JArray array = json["long-array-array"] as JArray;
                foreach (var item in array)
                {
                    yield return item[3].ToString();
                }
            }
        }

        /// <summary>
        /// 发送重新切片任务
        /// </summary>
        /// <param name="layerName">图层名称（工作区:图层名称）</param>
        /// <param name="gridSetName">切片规则</param>
        /// <param name="zoomStart">切片开始等级</param>
        /// <param name="zoomStop">切片结束等级</param>
        /// <param name="threadCount">任务进程数量</param>
        /// <param name="bBox">切片范围（格式：minX,minY,maxX,maxY）</param>
        /// <returns></returns>
        public bool SendAllTilesTask(string layerName, string gridSetName, int zoomStart, int zoomStop, int threadCount = 1, string bBox = null)
        {
            string requestUrl = BaseServiceUrl + "/gwc/rest/seed/" + layerName + ".xml";
            string status = string.Empty;
            object newTask = "<seedRequest><name>" + layerName + "</name>[BBOX]<gridSetId>" + gridSetName + "</gridSetId><zoomStart>" + zoomStart + "</zoomStart><zoomStop>" + zoomStop + "</zoomStop><type>reseed</type><threadCount>" + threadCount + "</threadCount></seedRequest>";
            string strBBox = string.Empty;
            if (!string.IsNullOrEmpty(bBox))
            {
                string[] arrBBox = bBox.Split(',');
                if (arrBBox.Count() == 4)
                {
                    strBBox = string.Format("<bounds><coords><double>{0}</double><double>{1}</double><double>{2}</double><double>{3}</double></coords></bounds>", arrBBox[0], arrBBox[1], arrBBox[2], arrBBox[3]);
                }
            }
            newTask = (newTask as string).Replace("[BBOX]", strBBox);
            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(string), ref newTask, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("发送重新切片任务异常！", ex);
            }
        }

        /// <summary>
        /// 发送缺失切片任务
        /// </summary>
        /// <param name="layerName">图层名称（工作区:图层名称）</param>
        /// <param name="gridSetName">切片规则</param>
        /// <param name="zoomStart">切片开始等级</param>
        /// <param name="zoomStop">切片结束等级</param>
        /// <param name="threadCount">任务进程数量</param>
        /// <param name="bBox">切片范围（格式：minX,minY,maxX,maxY）</param>
        /// <returns></returns>
        public bool SendMissTilesTask(string layerName, string gridSetName, int zoomStart, int zoomStop, int threadCount = 1, string bBox = null)
        {
            string requestUrl = BaseServiceUrl + "/gwc/rest/seed/" + layerName + ".xml";
            string status = string.Empty;
            object newTask = "<seedRequest><name>" + layerName + "</name>[BBOX]<gridSetId>" + gridSetName + "</gridSetId><zoomStart>" + zoomStart + "</zoomStart><zoomStop>" + zoomStop + "</zoomStop><type>seed</type><threadCount>" + threadCount + "</threadCount></seedRequest>";
            string strBBox = string.Empty;
            if (!string.IsNullOrEmpty(bBox))
            {
                string[] arrBBox = bBox.Split(',');
                if (arrBBox.Count() == 4)
                {
                    strBBox = string.Format("<bounds><coords><double>{0}</double><double>{1}</double><double>{2}</double><double>{3}</double></coords></bounds>", arrBBox[0], arrBBox[1], arrBBox[2], arrBBox[3]);
                }
            }
            newTask = (newTask as string).Replace("[BBOX]", strBBox);
            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(string), ref newTask, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("发送缺失切片任务异常！", ex);
            }
        }

        /// <summary>
        /// 发送清空切片任务
        /// </summary>
        /// <param name="layerName">图层名称（工作区:图层名称）</param>
        /// <param name="gridSetName">切片规则</param>
        /// <param name="zoomStart">切片开始等级</param>
        /// <param name="zoomStop">切片结束等级</param>
        /// <param name="threadCount">任务进程数量</param>
        /// <param name="bBox">切片范围（格式：minX,minY,maxX,maxY）</param>
        /// <returns></returns>
        public bool SendEmptyTilesTask(string layerName, string gridSetName, int zoomStart, int zoomStop, int threadCount = 1, string bBox = null)
        {
            string requestUrl = BaseServiceUrl + "/gwc/rest/seed/" + layerName + ".xml";
            string status = string.Empty;
            object newTask = "<seedRequest><name>" + layerName + "</name>[BBOX]<gridSetId>" + gridSetName + "</gridSetId><zoomStart>" + zoomStart + "</zoomStart><zoomStop>" + zoomStop + "</zoomStop><type>truncate</type><threadCount>" + threadCount + "</threadCount></seedRequest>";
            string strBBox = string.Empty;
            if (!string.IsNullOrEmpty(bBox))
            {
                string[] arrBBox = bBox.Split(',');
                if (arrBBox.Count() == 4)
                {
                    strBBox = string.Format("<bounds><coords><double>{0}</double><double>{1}</double><double>{2}</double><double>{3}</double></coords></bounds>", arrBBox[0], arrBBox[1], arrBBox[2], arrBBox[3]);
                }
            }
            newTask = (newTask as string).Replace("[BBOX]", strBBox);
            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(string), ref newTask, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("发送清空切片任务异常！", ex);
            }
        }

        /// <summary>
        /// 结束切片任务
        /// </summary>
        /// <param name="layerName">图层名称（工作区:图层名称）</param>
        /// <param name="taskState">任务状态</param>
        /// <returns></returns>
        public bool SendTerminatingTask(string layerName, TaskState taskState)
        {
            string requestUrl = BaseServiceUrl + "/gwc/rest/seed/" + layerName + "";
            string status = string.Empty;
            object newTask = string.Format("kill_all={0}", _TaskStateLookup[taskState]);

            try
            {
                status = SendRestRequest(requestUrl, RequestMethod.Post, typeof(string), ref newTask, ContentType.Html, AcceptType.Json);
                if (status == "OK")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("结束切片任务异常！", ex);
            }
        }
        #endregion

        #region WMTS
        /// <summary>
        /// 获取WMTS图片
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public byte[] GetWMTSData(string requestUrl)
        {
            object payload = null;
            GetRestResponse(requestUrl, RequestMethod.Get, typeof(string), ref payload, ContentType.Html, AcceptType.Byte);
            if (payload != null)
            {
                return payload as byte[];
            }
            return null;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 获取Geoserver服务地址
        /// </summary>
        public string BaseServiceUrl
        {
            get
            {
                return GeoServerConstants.BaseUrl.Replace(GeoServerConstants.ServerIp, ServerIP).Replace(GeoServerConstants.ServerPort, ServerPort);
            }
        }

        /// <summary>
        /// 获取Rest服务地址
        /// </summary>
        public string RestServiceUrl
        {
            get
            {
                return GeoServerConstants.RestUrl.Replace(GeoServerConstants.ServerIp, ServerIP).Replace(GeoServerConstants.ServerPort, ServerPort);
            }
        }
        /// <summary>
        /// 验证GeoServer端口
        /// </summary>
        public bool IsValid
        {
            get
            {
                //IPGlobalProperties ipGlobal = IPGlobalProperties.GetIPGlobalProperties();
                //IPEndPoint[] tcpCon = ipGlobal.GetActiveTcpListeners();

                //foreach (IPEndPoint ipEnd in tcpCon)
                //{
                //    if (ipEnd.Port.ToString() == ServerPort)
                //    {
                //        return true;
                //    }
                //}
                return true;
            }
        }
        /// <summary>
        /// JSON转字符串
        /// </summary>
        public string GetJsonString(Object objToConvert)
        {
            string retVal = string.Empty;
            if (objToConvert != null)
            {
                Type objType = objToConvert.GetType();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(objType);

                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, objToConvert);
                    ms.Position = 0;
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        retVal = sr.ReadToEnd();
                    }
                }
            }
            return retVal;
        }

        public string XmlSerialize<T>(T obj)
        {
            string xmlString = string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                xmlSerializer.Serialize(ms, obj);
                xmlString = Encoding.UTF8.GetString(ms.ToArray());
            }
            return xmlString;
        }

        /// <summary>  
        /// XML String 反序列化成对象  
        /// </summary>  
        public T XmlDeserialize<T>(string xmlString)
        {
            T t = default(T);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                using (XmlReader xmlReader = XmlReader.Create(xmlStream))
                {
                    Object obj = xmlSerializer.Deserialize(xmlReader);
                    t = (T)obj;
                }
            }
            return t;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="objectType">请求类型</param>
        /// <param name="payload">返回数据</param>
        /// <param name="contentType">返回类型</param>
        /// <param name="acceptType">接收类型</param>
        /// <returns></returns>
        private string SendRestRequest(string requestUrl, RequestMethod method, Type objectType, ref object payload, ContentType contentType, AcceptType acceptType)
        {
            string statusCode = "Error";
            if (IsValid)
            {
                HttpWebRequest request = HttpWebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = _RequestMethodLookup[method];
                request.Credentials = new NetworkCredential(GEOSERVER_USERNAME, GEOSERVER_PASSWORD);
                request.ContentType = _ContentTypeLookup[contentType];

                if (contentType == ContentType.Json)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectType);
                    if (method == RequestMethod.Get)
                    {
                        request.Accept = _AcceptTypeLookup[acceptType];
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        statusCode = response.StatusDescription;
                        payload = serializer.ReadObject(response.GetResponseStream());
                        response.Close();
                    }
                    else if (method == RequestMethod.Post || method == RequestMethod.Delete || method == RequestMethod.Put)
                    {
                        serializer.WriteObject(request.GetRequestStream(), payload);
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        statusCode = response.StatusDescription;
                        response.Close();
                    }
                }
                else if (contentType == ContentType.Xml)
                {
                    request.ContentType = "text/xml";
                    if (method == RequestMethod.Get)
                    {

                    }
                    else if (method == RequestMethod.Post || method == RequestMethod.Delete || method == RequestMethod.Put)
                    {

                    }
                }
                else if (contentType == ContentType.SldFile || contentType == ContentType.Zip)
                {
                    if (method == RequestMethod.Put)
                    {
                        Stream reqStream = request.GetRequestStream();
                        FileStream fileStream = new FileStream(payload.ToString(), FileMode.Open, FileAccess.Read);
                        byte[] bytes = new byte[4096];
                        int bytesRead = fileStream.Read(bytes, 0, bytes.Length);
                        while (bytesRead > 0)
                        {
                            reqStream.Write(bytes, 0, bytesRead);
                            bytesRead = fileStream.Read(bytes, 0, bytes.Length);
                        }
                        fileStream.Close();
                        reqStream.Close();

                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        statusCode = response.StatusDescription;
                        response.Close();
                    }
                }
                else
                {
                    request.ContentType = "text/xml";
                    if (method == RequestMethod.Get)
                    {
                        request.Accept = _AcceptTypeLookup[acceptType];
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        statusCode = response.StatusDescription;
                        payload = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        response.Close();
                    }
                    else if (method == RequestMethod.Post || method == RequestMethod.Delete || method == RequestMethod.Put)
                    {
                        if (payload is string)
                        {
                            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(payload as string);

                            Stream reqStream = request.GetRequestStream();
                            reqStream.Write(bytes, 0, bytes.Length);
                            reqStream.Close();
                        }
                        else
                        {
                            throw new NotImplementedException("返回数据的类型错误！");
                        }

                        try
                        {
                             HttpWebResponse response =null;
                              response = (HttpWebResponse)request.GetResponse();
                            statusCode = response.StatusDescription;

                            logger.Debug("statusCode:"+statusCode);
                            response.Close();
                        }
                        catch (Exception ex)
                        {
                            logger.Debug("Request 出错，URL:" + requestUrl + "", ex);
                            throw;
                        }
                    }
                }
            }
            return statusCode;
        }
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="objectType">请求类型</param>
        /// <param name="payload">返回数据</param>
        /// <param name="contentType">返回类型</param>
        /// <param name="acceptType">接收类型</param>
        /// <returns></returns>
        private string GetRestResponse(string requestUrl, RequestMethod method, Type objectType, ref Object payload, ContentType contentType, AcceptType acceptType)
        {
            try
            {
                string retVal = "Error";

                if (IsValid)
                {
                    HttpWebRequest request = HttpWebRequest.Create(requestUrl) as HttpWebRequest;
                    request.Method = _RequestMethodLookup[method];
                    request.Credentials = new NetworkCredential(GEOSERVER_USERNAME, GEOSERVER_PASSWORD);
                    request.ContentType = _ContentTypeLookup[contentType];

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectType);
                    if (method == RequestMethod.Get)
                    {
                        request.Accept = _AcceptTypeLookup[acceptType];
                    }
                    else if (method == RequestMethod.Post)
                    {
                        serializer.WriteObject(request.GetRequestStream(), payload);
                    }

                    if (acceptType == AcceptType.Byte)
                    {
                        MemoryStream ms = new MemoryStream();
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        Stream resStream = response.GetResponseStream();
                        byte[] bytes = new byte[4096];
                        int bytesRead = resStream.Read(bytes, 0, bytes.Length);
                        while (bytesRead > 0)
                        {
                            ms.Write(bytes, 0, bytesRead);
                            bytesRead = resStream.Read(bytes, 0, bytes.Length);
                        }
                        response.Close();
                        ms.Flush();
                        ms.Seek(0L, SeekOrigin.Begin);
                        payload = ms.ToArray();
                        retVal = "OK";
                    }
                    else
                    {
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        retVal = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
