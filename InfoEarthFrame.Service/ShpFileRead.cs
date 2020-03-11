using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Quartz;
using System.Configuration;
using System.Data;
using InfoEarthFrame.Common;
using System.IO;
using InfoEarthFrame.Common.ShpUtility;
using System.Collections;
using InfoEarthFrame.GeoServerRest;

namespace InfoEarthFrame.Server
{
    /// <summary>
    /// A sample job that just prints info on console for demostration purposes.
    /// </summary>
    public class ShpFileRead : IJob
    {
        #region [变量定义]

        private static readonly ILog logger = LogManager.GetLogger(typeof(ShpFileRead));
        private string _connStr = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        private string _GeoServerIp = string.Empty;
        private string _GeoServerPort = string.Empty;
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

        //private MySqlHelper mysql;
        GeoServer _GeoServer;
        private PostgrelVectorHelper postgis;
        #endregion

        /// <summary>
        /// 实现JOB接口
        /// </summary>
        /// <param name="context">sd</param>
        public void Execute(IJobExecutionContext context)
        {
            logger.Info("ShpFileReadJob running...");
            InitGeoServer();
            //mysql = new MySqlHelper();
            postgis = new PostgrelVectorHelper();
            _GeoServer = new GeoServer(_GeoServerIp, _GeoServerPort);
            //启动开始
            StartReadShpFile();
            logger.Info("ShpFileReadJob run finished.");
        }

        /// <summary>
        /// 初始化GeoServer配置
        /// </summary>
        public void InitGeoServer()
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
        }

        /// <summary>
        /// 读取SHP文件
        /// </summary>
        public void StartReadShpFile()
        {
            DataTable dt = GetShpUploadFile();
            DataTable dtDataType = GetFileType();
            DataTable dtFieldDict = GetLayerAttributeDict();
            string msg = string.Empty;


            if (dt.Rows.Count > 0)
            {
                #region [更新状态]

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ReadShpFileStatus(dt.Rows[i]["Id"].ToString());
                }

                #endregion

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    msg = "";
                    DateTime dtStart = DateTime.Now;
                    bool success = ReadShpFile(dt.Rows[i], dtDataType, dtFieldDict, ref msg);
                    DateTime dtEnd = DateTime.Now;
                    UpdataShpFileRead(dt.Rows[i]["Id"].ToString(), success, msg, dtStart.ToString(), dtEnd.ToString());
                }
            }
        }

        #region [MySQL]

        #region [查询数据]

        /// <summary>
        /// 取待解析的SHP文件
        /// </summary>
        /// <returns></returns>
        public DataTable GetShpUploadFile()
        {
            //string strSQL = "select a.LayerID,a.ShpFileName,b.LayerAttrTable,b.LayerSpatialTable,b.MinX,b.MinY,b.MaxX,b.MaxY,b.UploadStatus,c.CodeName,a.Id,a.CreateBy,b.LayerName,a.FolderName from sdms_layer_readlog a,sdms_layer b,sdms_dictdatacode c where a.LayerID = b.Id and b.DataType = c.Id and a.ReadStatus = '0' ORDER BY a.CreateDT DESC LIMIT 0,5";
            //return mysql.ExecuteQuery(strSQL);
            string strSQL = "select a.LayerID,a.ShpFileName,b.LayerAttrTable,b.LayerSpatialTable,b.MinX,b.MinY,b.MaxX,b.MaxY,b.UploadStatus,c.CodeName,a.Id,a.CreateBy,b.LayerName,a.FolderName from sdms_layer_readlog a,sdms_layer b,sdms_dictdatacode c where a.LayerID = b.Id and b.DataType = c.Id and a.ReadStatus = '0' ORDER BY a.CreateDT DESC LIMIT 5";
            return postgis.getDataTable(strSQL);
        }

        /// <summary>
        /// 图层属性
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public DataTable GetLayerAttribute(string layerID)
        {
            //string strSQL = "select a.*,b.CodeValue as AttributeTypeName from sdms_layerfield a,sdms_dictdatacode b where a.AttributeType = b.Id and a.LayerID = '" + layerID + "' ";
            //return mysql.ExecuteQuery(strSQL);
            string strSQL = "select a.*,b.CodeValue as AttributeTypeName from sdms_layerfield a,sdms_dictdatacode b where a.AttributeType = b.Id and a.LayerID = '" + layerID + "' ";
            return postgis.getDataTable(strSQL);
        }

        /// <summary>
        /// 图层属性字典
        /// </summary>
        /// <returns></returns>
        public DataTable GetLayerAttributeDict()
        {
            string strSQL = "select a.* from sdms_layerfielddict a ";
            return postgis.getDataTable(strSQL);
            //return mysql.ExecuteQuery(strSQL);
        }

        /// <summary>
        /// asds
        /// </summary>
        /// <returns></returns>
        public DataTable GetFileType()
        {
            string strSQL = "select a.* from sdms_dictdatacode a where a.DataTypeID = 'b9ef9c7c-67b3-11e7-8eb2-005056bb1c7e' ";
            return postgis.getDataTable(strSQL);
            //return mysql.ExecuteQuery(strSQL);
        }

        public DataTable GetMapByRelLayer(string layerID)
        {
            string strSQL = "select b.MapEnName,b.MinX,b.MinY,b.MaxX,b.MaxY from sdms_map_releation a,sdms_map b where a.MapID = b.Id and a.DataConfigID = '" + layerID + "' ";
            return postgis.getDataTable(strSQL);
            //return mysql.ExecuteQuery(strSQL);
        }

        #endregion

        #region [更新数据]

        /// <summary>
        /// 更新文件读取状态
        /// </summary>
        /// <param name="logID">aas</param>
        /// <param name="status">aas</param>
        ///<param name="msg">aas</param>
        /// <returns></returns>
        public bool UpdataShpFileRead(string logID, bool status, string msg, string strStart, string strEnd)
        {
            string strSQL = "update sdms_layer_readlog set ";

            strSQL += "ReadStartDT = '" + strStart + "',ReadEndDT='" + strEnd + "',";

            if (!status)
            {
                strSQL += " readStatus = '2', message = '" + msg + "'";
            }
            else
            {
                strSQL += " readStatus = '1', message = '读取成功' ";
            }

            strSQL += ",MsgStatus = '0' where Id = '" + logID + "'";

            //return mysql.ExecuteNonQuery(strSQL);
            return postgis.ExceuteSQL(strSQL,string.Empty);
        }

        public bool ReadShpFileStatus(string logID)
        {
            string strSQL = "update sdms_layer_readlog set readStatus = '3'";

            strSQL += " where Id = '" + logID + "'";

            return postgis.ExceuteSQL(strSQL, string.Empty);
            //return mysql.ExecuteNonQuery(strSQL);
        }

        #endregion

        #endregion

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="dtFileType"></param>
        /// <param name="message"></param>
        public bool ReadShpFile(DataRow dr, DataTable dtFileType, DataTable dtFieldDict, ref string message)
        {
            string layerID = dr[0].ToString(), filePath = dr[1].ToString(), tableName1 = dr[2].ToString(), tableName2 = dr[3].ToString();
            string layerName = dr[12].ToString();
            string folderName = (dr[13] != null) ? dr[13].ToString().Replace("#", "\\") : "";
            bool status = false;
            if (IsFileExist(filePath, ref filePath, dtFileType, folderName))
            {
                try
                {
                    ShpReader shpReader = new ShpReader(filePath);
                    // 检查矢量文件的有效性
                    if (!shpReader.IsValidDataSource())
                    {
                        status = false;
                        message = UtilityMessageConvert.Get("上传文件解析异常(上传的矢量文件无效)");
                    }
                    else
                    {
                        string tableName = string.Empty;
                        string layerType = string.Empty;

                        DataTable dtAttr = GetLayerAttribute(layerID);

                        //建表
                        Dictionary<string, string> attr = shpReader.GetAttributeType();
                        List<string> shpAttr = new List<string>();
                        Hashtable hashTable = new Hashtable();

                        for (int i = 0; i < dtAttr.Rows.Count; i++)
                        {
                            string name = dtAttr.Rows[i]["AttributeName"].ToString();
                            if (attr.ContainsKey(name))
                            {
                                shpAttr.Add(name);
                                hashTable.Add(name, attr[name]);
                            }
                        }

                        if (shpAttr.Count <= 0 || shpAttr.Count != dtAttr.Rows.Count || attr.Count != dtAttr.Rows.Count)
                        {
                            status = false;
                            message = UtilityMessageConvert.Get("上传文件解析异常(属性无匹配或只部分匹配)");
                        }
                        else
                        {
                            #region [属性检查]

                            DataTable dtValidError = ValidLayerFieldData(shpReader, shpAttr, dtAttr, dtFieldDict);

                            #endregion

                            if (dtValidError.Rows.Count > 0)
                            {
                                NpoiExcelUtility excelUtility = new NpoiExcelUtility();
                                excelUtility.CreatExcelSheet(UtilityMessageConvert.Get("文件属性检查异常统计"), dtValidError);
                                string ExcelPath = Path.Combine(ConfigurationManager.AppSettings["DownloadFile"], "Excel");

                                if (!Directory.Exists(ExcelPath))
                                {
                                    Directory.CreateDirectory(ExcelPath);
                                }
                                ExcelPath = Path.Combine(ExcelPath, layerName + ".xls");
                                if (!File.Exists(ExcelPath))
                                {
                                    File.Delete(ExcelPath);
                                }
                                bool flag = excelUtility.SaveExcel(ExcelPath);

                                string url = "http://" + ConfigurationManager.AppSettings["PublishAddress"] + "/" + "DownloadFile" + "/" + "Excel" + "/" + layerName + ".xls";

                                status = false;

                                if (!flag)
                                {
                                    message = UtilityMessageConvert.Get("文件属性检查有异常，生成Excel有异常");
                                }
                                else
                                {
                                    message = UtilityMessageConvert.Get("文件属性检查有异常，详情请查看Excel") + "##" + url;
                                }
                            }
                            else
                            {
                                #region [数据入库]

                                //PostgrelVectorHelper dService = new PostgrelVectorHelper();

                                //bool success = dService.ImportTable(tableName1, shpReader, shpAttr, hashTable);

                                //if (success)
                                //{
                                //获取图层BBox
                                Dictionary<string, double> bbox = new Dictionary<string, double>();

                                bool success = ImportToDataBase(tableName1, tableName2, shpReader, ref bbox, shpAttr, hashTable);
                                //success = ImportMysqlData(tableName1, tableName2, shpReader, shpAttr, hashTable, ref bbox, ref message);

                                if (success)
                                {
                                    layerType = dr[9].ToString();

                                    #region [GeoServer操作]

                                    success = PublishGeoServer(tableName1, layerName, ref message, layerID, layerType);

                                    #endregion

                                    if (success)
                                    {
                                        #region [更新状态和坐标]

                                        string strUpdateSQL = "update sdms_layer set UploadStatus = '1'";
                                        string strBBox = string.Empty;

                                        if (bbox.Count > 0)
                                        {
                                            decimal MinX = Math.Min((decimal)bbox["MinX"], dr[4].ToString() == "" ? decimal.MaxValue : (decimal)dr[4]);
                                            decimal MaxX = Math.Max((decimal)bbox["MaxX"], dr[6].ToString() == "" ? decimal.MinValue : (decimal)dr[6]);
                                            decimal MinY = Math.Min((decimal)bbox["MinY"], dr[5].ToString() == "" ? decimal.MaxValue : (decimal)dr[5]);
                                            decimal MaxY = Math.Max((decimal)bbox["MaxY"], dr[7].ToString() == "" ? decimal.MinValue : (decimal)dr[7]);

                                            strUpdateSQL += ",MinX = " + MinX + ",MinY=" + MinY + ",MaxX=" + MaxX + ",MaxY=" + MaxY;
                                            strBBox = MinX.ToString() + "," + MinY.ToString() + "," + MaxX.ToString() + "," + MaxY.ToString();

                                            if (success && dr[4].ToString() != null)
                                            {
                                                string bboxStr = string.Format("{0},{1},{2},{3}", MinX.ToString(), MinY.ToString(), MaxX.ToString(), MaxY.ToString());
                                                success = _GeoServer.ModifyLayerBBox(tableName1, layerName, _GeoWorkSpace, _GeoDataStore, _epsg, bboxStr);
                                            }
                                        }

                                        if (success)
                                        {
                                            strUpdateSQL += " where Id='" + dr[0].ToString() + "'";
                                            //success = mysql.ExecuteNonQuery(strUpdateSQL);
                                            success = postgis.ExceuteSQL(strUpdateSQL,string.Empty);

                                            if (success)
                                            {
                                                #region [生成缩略图]

                                                ThumbnailHelper tbh = new ThumbnailHelper();
                                                string imagePath = tbh.CreateThumbnail(tableName1, "layer", strBBox);

                                                #endregion

                                                #region [图层所属的地图缩略图刷新]

                                                UpdateMapThumbnail(layerID);

                                                #endregion

                                                if (string.IsNullOrEmpty(imagePath))
                                                {
                                                    status = false;
                                                    message = UtilityMessageConvert.Get("上传文件解析异常(下载缩略图)");
                                                }
                                                else
                                                {
                                                    status = true;
                                                    message = UtilityMessageConvert.Get("上传文件解析数据保存成功");
                                                }

                                                string nodeMsg = string.Empty;
                                                nodeMsg += "[{";
                                                nodeMsg += "\"" + "layerID" + "\":\"" + layerID + "\",";
                                                nodeMsg += "\"" + "logID" + "\":\"" + dr["Id"].ToString() + "\",";
                                                nodeMsg += "\"" + "readStatus" + "\":\"";
                                                if (status)
                                                {
                                                    nodeMsg += UtilityMessageConvert.Get("导入成功");
                                                }
                                                else
                                                {
                                                    nodeMsg += UtilityMessageConvert.Get("导入失败");
                                                }
                                                nodeMsg += "\"";
                                                nodeMsg += "}]";

                                                #region [实时消息发送]

                                                //SendMsg(dr["CreateBy"].ToString(), nodeMsg);

                                                #endregion

                                            }
                                            else
                                            {
                                                status = false;
                                                message = UtilityMessageConvert.Get("上传文件解析异常(GeoServer地图发布异常)");
                                            }
                                        }
                                        else
                                        {
                                            status = false;
                                            message = UtilityMessageConvert.Get("上传文件解析异常(GeoServer地图发布异常)");
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        status = false;
                                        message = UtilityMessageConvert.Get("上传文件解析异常(GeoServer地图发布异常)");
                                    }
                                }
                                else
                                {
                                    status = false;
                                    message = UtilityMessageConvert.Get("上传文件解析异常(MySQL:请查看SHP文件属性值类型或长度与系统建置是否匹配)");
                                }
                                //}
                                //else
                                //{
                                //    status = false;
                                //    message = "上传文件解析异常(postGIS:请查看SHP文件属性值类型或长度与系统建置是否匹配)";
                                //}

                                #endregion
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    string test = ex.Message;
                    message = UtilityMessageConvert.Get("上传文件解析异常")+ test;
                }
            }
            else
            {
                status = false;
                message = UtilityMessageConvert.Get("上传文件解析异常(上传必要文件不存在或不完整)");
            }
            return status;
        }

        /// <summary>
        /// GeoServer图层发布
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="message"></param>
        /// <param name="layerID"></param>
        /// <param name="layerType"></param>
        /// <returns></returns>
        private bool PublishGeoServer(string tableName, string layerName, ref string message, string layerID, string layerType)
        {
            bool status = false, success = true;

            try
            {
                if (!_GeoServer.IsExsitLayer(tableName, _GeoWorkSpace, _GeoDataStore))
                {
                    #region [Publish Layer]

                    if (!_GeoServer.IsExsitWorkSpace(_GeoWorkSpace))
                    {
                        _GeoServer.AddWorkSpace(_GeoWorkSpace);
                    }
                    if (!_GeoServer.IsExsitDataStore(_GeoWorkSpace, _GeoDataStore))
                    {
                        _GeoServer.AddDataStore(_GeoWorkSpace, _GeoDataStore, _PostGisHost, _PostGisPort, _PostGisDB, _PostGisUser, _PostGisPwd);
                    }
                    Dictionary<string, string> filedAndType = new Dictionary<string, string>();

                    DataTable listDto = GetLayerAttribute(layerID);
                    for (int i = 0; i < listDto.Rows.Count; i++)
                    {
                        filedAndType.Add(listDto.Rows[i]["AttributeName"].ToString(), GetGeoFiledType(listDto.Rows[i]["AttributeTypeName"].ToString()));
                    }
                    filedAndType.Add("guid", GetGeoFiledType("VARCHAR"));
                    filedAndType.Add("geom", GetGeoFiledType(layerType));
                    if (!_GeoServer.IsExsitLayer(tableName, _GeoWorkSpace, _GeoDataStore))
                    {
                        success = _GeoServer.AddLayer(tableName, layerName, filedAndType, _GeoWorkSpace, _GeoDataStore, _epsg);

                        if (success)
                        {
                            success = _GeoServer.ModifyLayerStyle(tableName, new List<string>(), GetDefaultStyle(layerType));
                        }
                        else
                        {
                            status = false;
                            message = UtilityMessageConvert.Get("上传文件解析异常(GeoServer地图发布异常)");
                        }
                    }

                    #endregion

                    if (!success)
                    {
                        status = false;
                        message = UtilityMessageConvert.Get("上传文件解析异常(GeoServer地图发布异常)");
                    }
                    else
                    {
                        status = true;
                    }
                }
                else
                {
                    status = true;
                    message = UtilityMessageConvert.Get("上传文件成功");
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                status = false;
                return status;
            }
            return status;
        }

        /// <summary>
        /// 判断上传文件是否完整
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filePhysicPath"></param>
        /// <param name="dtDataType"></param>
        /// <returns></returns>
        public bool IsFileExist(string filePath, ref string filePhysicPath, DataTable dtDataType, string folderName)
        {
            string name = filePath.Substring(0, filePath.LastIndexOf("."));

            string fileFullName = System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"];
            filePhysicPath = Path.Combine(fileFullName, folderName, Path.GetFileName(filePath));

            if (dtDataType.Rows.Count > 0)
            {
                for (int i = 0; i < dtDataType.Rows.Count; i++)
                {
                    string fileName = name + "." + dtDataType.Rows[i]["CodeValue"];
                    string uploadFilePath = "";
                    if (!fileName.Contains(fileFullName))
                    {
                        uploadFilePath = Path.Combine(fileFullName, folderName, Path.GetFileName(fileName));
                    }
                    if (!File.Exists(uploadFilePath))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ImportMysqlData(string tableName1, string tableName2, ShpReader shpReader, List<string> shpAttr, Hashtable hashTable, ref Dictionary<string, double> bbox, ref string message)
        {
            int _count = 500;
            int srid = shpReader.GetSrid();
            try
            {
                //获取图层BBox
                bbox = shpReader.GetLayerBBox();

                //向表中添加数据
                int pFeatureCount = shpReader.GetFeatureCount();
                string sqlCInsert1 = string.Empty;
                string sqlCInsert2 = string.Empty;
                for (int i = 0; i < pFeatureCount; i++)
                {
                    string colStr = string.Empty;
                    try
                    {
                        string valueStr = string.Empty;

                        Dictionary<string, string> attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
                        foreach (KeyValuePair<string, string> item in attr1)
                        {
                            colStr += "`" + item.Key + "`" + ",";
                            if (hashTable[item.Key].ToString() == "OFTString")
                            {
                                if (item.Value.Contains("'"))
                                {
                                    string newValue = item.Value.Replace("'", "''");
                                    valueStr += String.Format("'{0}',", newValue);
                                }
                                else
                                {
                                    valueStr += String.Format("'{0}',", item.Value);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    valueStr += String.Format("{0},", item.Value);
                                }
                                else
                                {
                                    valueStr += String.Format("'{0}',", item.Value);
                                }
                            }
                        }
                        colStr = colStr.TrimEnd(',');
                        valueStr = valueStr.TrimEnd(',');

                        string sid1 = Guid.NewGuid().ToString();
                        string sid2 = Guid.NewGuid().ToString();

                        string geomStr = String.Format("GEOMFROMTEXT('{0}')", shpReader.GetOneFeatureGeomWkt(i));

                        sqlCInsert1 += string.Format("('{0}',{1}),", sid1, valueStr);
                        sqlCInsert2 += string.Format("('{0}','{1}',{2}),", sid2, sid1, geomStr);
                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                    if ((i % _count == 0) || (i == pFeatureCount - 1))
                    {
                        sqlCInsert1 = sqlCInsert1.TrimEnd(',');
                        sqlCInsert2 = sqlCInsert2.TrimEnd(',');
                        string sqlInsert1 = String.Format("insert into `{0}`(`sid`,{1}) values{2}", tableName1, colStr, sqlCInsert1);

                        string sqlInsert2 = String.Format("insert into `{0}`(`sid`,`DataID`,`geom`) values{1}", tableName2, sqlCInsert2);

                        //mysql.ExecuteNonQuery(sqlInsert1);
                        sqlCInsert1 = string.Empty;

                        //mysql.ExecuteNonQuery(sqlInsert2);
                        sqlCInsert2 = string.Empty;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 地图缩略图下载
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public bool UpdateMapThumbnail(string layerID)
        {
            try
            {
                DataTable dt = GetMapByRelLayer(layerID);

                if (dt.Rows.Count > 0)
                {
                    string strBBox = string.Empty;
                    ThumbnailHelper tbh = new ThumbnailHelper();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strBBox = dt.Rows[i]["MinX"].ToString() + "," + dt.Rows[i]["MinY"].ToString() + "," + dt.Rows[i]["MaxX"].ToString() + "," + dt.Rows[i]["MaxY"].ToString();
                        string imagePath = tbh.CreateThumbnail(dt.Rows[i]["MapEnName"].ToString(), "map", strBBox);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return false;
            }
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
                case "Point":
                    return "com.vividsolutions.jts.geom.Point";

                case "线":
                case "Line":
                    return "com.vividsolutions.jts.geom.MultiLineString";

                case "面":
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
                case "Point":
                    return "point";
                case "线":
                case "Line":
                    return "line";
                case "面":
                case "Polygon":
                    return "polygon";
                default:
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// 发布消息状态
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        //public bool SendMsg(string user, string msg)
        //{
        //    NodeJSHelper nodeMsg = new NodeJSHelper();
        //    string module = ConfigurationManager.AppSettings["NodeJSMsgModule"];
        //    return nodeMsg.SendMsg(module, user, msg);
        //}

        /// <summary>
        /// 属性字段规则验证
        /// </summary>
        /// <param name="shpReader"></param>
        /// <param name="shpAttr"></param>
        /// <param name="dtAttField"></param>
        /// <param name="dtAttFieldDict"></param>
        /// <returns></returns>
        private DataTable ValidLayerFieldData(ShpReader shpReader, List<string> shpAttr, DataTable dtAttField, DataTable dtAttFieldDict)
        {
            string msg = string.Empty;
            //向表中添加数据
            int pFeatureCount = shpReader.GetFeatureCount();
            DataTable dt = CreateErrorOutput(dtAttField);
            for (int i = 0; i < pFeatureCount; i++)
            {
                msg = "";
                DataRow drField = dt.NewRow();

                Dictionary<string, string> attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
                foreach (KeyValuePair<string, string> item in attr1)
                {
                    drField["FID"] = i.ToString();
                    drField[item.Key] = item.Value;

                    DataRow[] dr = dtAttField.Select("AttributeName='" + item.Key + "'");
                    foreach (DataRow drr in dr)
                    {
                        string dataTypeString = "673e95ba-67a8-11e7-8eb2-005056bb1c7e,7c6aa917-67a8-11e7-8eb2-005056bb1c7e,8f553741-67a8-11e7-8eb2-005056bb1c7e,9ffd11ea-67a8-11e7-8eb2-005056bb1c7e";
                        if (!string.IsNullOrEmpty(drr["AttributeIsNull"].ToString()) || !string.IsNullOrEmpty(drr["AttributeInputCtrl"].ToString()) || !string.IsNullOrEmpty(drr["AttributeInputFormat"].ToString()) || !string.IsNullOrEmpty(drr["AttributeDataType"].ToString()) || (!string.IsNullOrEmpty(drr["AttributeInputMax"].ToString()) && !string.IsNullOrEmpty(drr["AttributeInputMin"].ToString())) || !string.IsNullOrEmpty(drr["AttributeCalComp"].ToString()))
                        {
                            if (!string.IsNullOrEmpty(drr["AttributeIsNull"].ToString()) && drr["AttributeIsNull"].ToString() == "F" && string.IsNullOrEmpty(item.Value))
                            {
                                msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[空值]不能为空不符") + ",";
                            }
                            else if (!string.IsNullOrEmpty(drr["AttributeDataType"].ToString()) && drr["AttributeDataType"].ToString() == "T" && !string.IsNullOrEmpty(item.Value))
                            {
                                DataRow[] drDict = dtAttFieldDict.Select("AttributeID='" + drr["Id"] + "'");
                                string dictValue = string.Empty;
                                foreach (DataRow drValue in drDict)
                                {
                                    dictValue += drValue["FieldDictName"].ToString() + ",";
                                }
                                if (!string.IsNullOrEmpty(dictValue))
                                {
                                    dictValue = dictValue.TrimEnd(',');
                                }

                                int count = 0;

                                if (!string.IsNullOrEmpty(drr["AttributeInputFormat"].ToString()) && drr["AttributeInputFormat"].ToString() == "S")
                                {
                                    foreach (DataRow drrDict in drDict)
                                    {
                                        if (drrDict["FieldDictName"].ToString() == item.Value)
                                        {
                                            count++;
                                        }
                                    }

                                    if (count > 1)
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[输入格式]单选不符,字典值") + "[" + dictValue + "]" + ",";
                                    }
                                    else if (count < 1)
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[输入格式]单选不符,字典值") + "[" + dictValue + "]" + UtilityMessageConvert.Get("不符") + ",";
                                    }
                                }
                                else if (!string.IsNullOrEmpty(drr["AttributeInputFormat"].ToString()) && drr["AttributeInputFormat"].ToString() == "M")
                                {
                                    string fieldValue = item.Value.Replace(",", ";").Replace("，", ";").Replace("；", ";");
                                    fieldValue = fieldValue.TrimEnd(';');
                                    string[] value = fieldValue.Split(';');
                                    foreach (DataRow drrDict in drDict)
                                    {
                                        for (int j = 0; j < value.Length; j++)
                                        {
                                            if (drrDict["FieldDictName"].ToString() == value[j])
                                            {
                                                count++;
                                            }
                                        }
                                    }

                                    if (count <= 1 || value.Length == 1)
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[输入格式]多选不符,字典值") + "[" + dictValue + "]" + ",";
                                    }
                                    else if (count > 1 && count < value.Length)
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[输入格式]多选不符,字典值") + "[" + dictValue + "]" + UtilityMessageConvert.Get("不符") + ",";
                                    }
                                }
                                else if (!string.IsNullOrEmpty(drr["AttributeInputFormat"].ToString()) || drr["AttributeInputFormat"].ToString() == "")
                                {
                                    string fieldValue = item.Value.Replace(",", ";").Replace("，", ";").Replace("；", ";");
                                    fieldValue = fieldValue.TrimEnd(';');
                                    string[] value = fieldValue.Split(';');
                                    foreach (DataRow drrDict in drDict)
                                    {
                                        for (int j = 0; j < value.Length; j++)
                                        {
                                            if (drrDict["FieldDictName"].ToString() == value[j])
                                            {
                                                count++;
                                            }
                                        }
                                    }

                                    if (count == 0 || count != value.Length)
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[输入格式]不符,字典值") + "[" + dictValue + "]" + UtilityMessageConvert.Get("不符") + ",";
                                    }
                                }
                            }
                            else if ((string.IsNullOrEmpty(drr["AttributeDataType"].ToString()) || drr["AttributeDataType"].ToString() == "F") && (!string.IsNullOrEmpty(drr["AttributeInputMax"].ToString()) && !string.IsNullOrEmpty(drr["AttributeInputMin"].ToString()))
                                && dataTypeString.Contains(drr["AttributeType"].ToString()))
                            {
                                string intType = "673e95ba-67a8-11e7-8eb2-005056bb1c7e,7c6aa917-67a8-11e7-8eb2-005056bb1c7e";
                                string floatType = "8f553741-67a8-11e7-8eb2-005056bb1c7e,9ffd11ea-67a8-11e7-8eb2-005056bb1c7e";
                                if (intType.Contains(drr["AttributeType"].ToString()))
                                {
                                    if (int.Parse(item.Value) < int.Parse(drr["AttributeInputMin"].ToString()) || int.Parse(item.Value) > int.Parse(drr["AttributeInputMax"].ToString()))
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[值域上限") + ":" + drr["AttributeInputMax"].ToString() + UtilityMessageConvert.Get("与值域下限") + ":" + drr["AttributeInputMin"].ToString() + "]" + UtilityMessageConvert.Get("值不符") + ",";
                                    }
                                }
                                else if (floatType.Contains(drr["AttributeType"].ToString()))
                                {
                                    if (decimal.Parse(item.Value) < decimal.Parse(drr["AttributeInputMin"].ToString()) || decimal.Parse(item.Value) > decimal.Parse(drr["AttributeInputMax"].ToString()))
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[值域上限") + ":" + drr["AttributeInputMax"].ToString() + UtilityMessageConvert.Get("与值域下限") + ":" + drr["AttributeInputMin"].ToString() + "]" + UtilityMessageConvert.Get("值不符") + ",";
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(drr["AttributeCalComp"].ToString()) && !string.IsNullOrEmpty(item.Value))
                            {
                                string express = drr["AttributeCalComp"].ToString();

                                string strDataType = "673e95ba-67a8-11e7-8eb2-005056bb1c7e,7c6aa917-67a8-11e7-8eb2-005056bb1c7e,8f553741-67a8-11e7-8eb2-005056bb1c7e,9ffd11ea-67a8-11e7-8eb2-005056bb1c7e";
                                for (int j = 0; j < dtAttField.Rows.Count; j++)
                                {
                                    if (!strDataType.Contains(dtAttField.Rows[j]["AttributeType"].ToString()) && express.Contains("{" + dtAttField.Rows[j]["AttributeName"].ToString() + "}"))
                                    {
                                        express = express.Replace("{" + dtAttField.Rows[j]["AttributeName"].ToString() + "}", ("\"" + attr1[dtAttField.Rows[j]["AttributeName"].ToString()].ToString() + "\""));
                                    }
                                    else
                                    {
                                        express = express.Replace("{" + dtAttField.Rows[j]["AttributeName"].ToString() + "}", attr1[dtAttField.Rows[j]["AttributeName"].ToString()].ToString());
                                    }
                                }

                                CSScriptHelper css = new CSScriptHelper();
                                object res = css.Calutrue(express);

                                if (!item.Value.Equals(res.ToString()))
                                {
                                    string resValue = res.ToString();
                                    if (resValue.Contains("#"))
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[公式]公式有异常") + ",";
                                    }
                                    else
                                    {
                                        msg += "[" + item.Key + "]" + ":" + UtilityMessageConvert.Get("与属性验证[公式]计算值") + resValue + UtilityMessageConvert.Get("不符") + ",";
                                    }
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    drField["ErrorMsg"] = msg.TrimEnd(',');
                    dt.Rows.Add(drField);
                }
            }
            return dt;
        }

        /// <summary>
        /// 创建异常输出表
        /// </summary>
        /// <param name="dtAttField"></param>
        /// <returns></returns>
        private DataTable CreateErrorOutput(DataTable dtAttField)
        {
            DataTable dt = new DataTable();
            DataColumn dc;

            dc = new DataColumn("ErrorMsg", typeof(System.String));
            dt.Columns.Add(dc);

            dc = new DataColumn("FID", typeof(System.String));
            dt.Columns.Add(dc);

            for (int i = 0; i < dtAttField.Rows.Count; i++)
            {
                dc = new DataColumn(dtAttField.Rows[i]["AttributeName"].ToString(), typeof(System.String));
                dt.Columns.Add(dc);
            }

            return dt;
        }

        /// <summary>
        /// 执行入库
        /// </summary>
        /// <param name="tableName1">图层业务表</param>
        /// <param name="tableName2">空间数据表</param>
        /// <param name="shpReader">shp文件读取</param>
        /// <param name="bbox"></param>
        /// <param name="shpAttr"></param>
        /// <param name="hashTable"></param>
        /// <returns></returns>
        public bool ImportToDataBase(string tableName1, string tableName2, ShpReader shpReader, ref Dictionary<string, double> bbox, List<string> shpAttr, Hashtable hashTable)
        {
            int _count = 500;
            int srid = shpReader.GetSrid();
            try
            {
                //获取图层BBox
                bbox = shpReader.GetLayerBBox();

                //向表中添加数据
                int pFeatureCount = shpReader.GetFeatureCount();

                PostgrelVectorHelper dService = new PostgrelVectorHelper();
                string sqlStr = String.Format("select Max(SID) from {0}", tableName1);
                object obj = dService.GetExecuteScalar(sqlStr);

                int maxValue = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
                int maxNum = maxValue + 1;
                string sqlCStr = string.Empty;

                string sqlCInsert1 = string.Empty;
                string sqlCInsert2 = string.Empty;
                string sqlCInsert3 = string.Empty;
                for (int i = 0; i < pFeatureCount; i++)
                {
                    string colStr1 = string.Empty;
                    string colStr2 = string.Empty;
                    try
                    {
                        string valueStr = string.Empty;

                        Dictionary<string, string> attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
                        foreach (KeyValuePair<string, string> item in attr1)
                        {
                            colStr1 += "`" + item.Key + "`" + ",";
                            colStr2 += "\"" + item.Key + "\"" + ",";
                            if (hashTable[item.Key].ToString() == "OFTString")
                            {
                                if (item.Value.Contains("'"))
                                {
                                    string newValue = item.Value.Replace("'", "''");
                                    valueStr += String.Format("'{0}',", newValue);
                                }
                                else
                                {
                                    valueStr += String.Format("'{0}',", item.Value);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    valueStr += String.Format("{0},", item.Value);
                                }
                                else
                                {
                                    valueStr += String.Format("'{0}',", item.Value);
                                }
                            }
                        }
                        colStr1 = colStr1.TrimEnd(',');
                        colStr2 = colStr2.TrimEnd(',');
                        valueStr = valueStr.TrimEnd(',');

                        string sid1 = Guid.NewGuid().ToString();
                        string sid2 = Guid.NewGuid().ToString();
                        string sid3 = string.Format("{0}", maxNum + i);

                        string geomStr1 = String.Format("GEOMFROMTEXT('{0}')", shpReader.GetOneFeatureGeomWkt(i));
                        string geomStr2 = String.Format("'{0}'", shpReader.GetOneFeatureGeomWkt(i));

                        sqlCInsert1 += string.Format("('{0}',{1}),", sid1, valueStr);
                        sqlCInsert2 += string.Format("('{0}','{1}',{2}),", sid2, sid1, geomStr1);
                        sqlCInsert3 += string.Format("({0},'{1}',{2},{3}),", sid3, sid1, geomStr2, valueStr);
                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                    if ((i % _count == 0) || (i == pFeatureCount - 1))
                    {
                        sqlCInsert1 = sqlCInsert1.TrimEnd(',');
                        sqlCInsert2 = sqlCInsert2.TrimEnd(',');
                        string sqlInsert1 = String.Format("insert into `{0}`(`sid`,{1}) values{2}", tableName1, colStr1, sqlCInsert1);

                        string sqlInsert2 = String.Format("insert into `{0}`(`sid`,`DataID`,`geom`) values{1}", tableName2, sqlCInsert2);

                        //mysql.ExecuteNonQuery(sqlInsert1);
                        sqlCInsert1 = string.Empty;

                        //mysql.ExecuteNonQuery(sqlInsert2);
                        sqlCInsert2 = string.Empty;

                        sqlCInsert3 = sqlCInsert3.TrimEnd(',');
                        string sqlInsert3 = String.Format("insert into {0}(sid,guid,geom,{1}) values{2}", tableName1, colStr2, sqlCInsert3);
                        dService.ExceuteSQL(sqlInsert3, string.Empty);
                        sqlCInsert3 = string.Empty;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return false;
            }
        }
    }
}