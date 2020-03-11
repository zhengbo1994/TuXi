using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.GeoServerRest;
using InfoEarthFrame.ServerInterfaceApp.Dtos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.IO;
using InfoEarthFrame.Common.ShpUtility;
using System.Collections;
using InfoEarthFrame.LayerFieldApp;
using Abp.Domain.Uow;
using System.Text.RegularExpressions;
using InfoEarthFrame.Core.Entities;
using System.Diagnostics;
using iTelluro.DataTools.Utility.DLG;
using InfoEarthFrame.GeoServerRest.Model;
using InfoEarthFrame.Application.LayerFieldApp.Dtos;
using Newtonsoft.Json;
using iTelluro.DataTools.Utility.Img;
using System.Net;
using System.Data.OracleClient;
using iTelluro.ZoomifyTile;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using InfoEarthFrame.DataManage.DTO;
using System.Data.Entity;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Common;
using Newtonsoft.Json;
using log4net;
using InfoEarthFrame.Application.OperateLogApp;
namespace InfoEarthFrame.ServerInterfaceApp
{
    public class ServerInterfaceAppService : IApplicationService, IServerInterfaceAppService
    {
        private readonly IMapReleationRepository _IMapReleationRepository;
        private readonly ILayerContentRepository _ILayerContentRepository;
        private readonly IDataStyleRepository _IDataStyleRepository;
        private readonly IMapRepository _IMapRepository;
        private readonly IDicDataTypeRepository _IDicDataTypeRepository;
        private readonly IDicDataCodeRepository _IDicDataCodeRepository;
        private readonly ILayerFieldRepository _ILayerFieldRepository;
        private readonly IDataTypeRepository _IDataTypeRepository;
        private readonly IDataTagRepository _IDataTagRepository;
        private readonly ITagReleationRepository _ITagReleationRepository;
        private readonly ILayerFieldDictRepository _ILayerFieldDictRepository;
        private readonly IOperateLogAppService _iOperateLogAppService;
        private string _GeoServerIp = string.Empty; 
        private string _GeoServerPort = string.Empty;
        private GeoServer _GeoServer;
        private string _GeoWorkSpace = string.Empty;
        private string _PublishAddress = string.Empty;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ServerInterfaceAppService));

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServerInterfaceAppService(IMapReleationRepository iMapReleationRepository, ILayerContentRepository iLayerContentRepository, IDataStyleRepository iDataStyleRepository,
            IMapRepository iMapRepository, IDicDataTypeRepository iDicDataTypeRepository, IDicDataCodeRepository iDicDataCodeRepository, ILayerFieldRepository iLayerFieldRepository,
            IDataTypeRepository iDataTypeRepository, IDataTagRepository iDataTagRepository, ITagReleationRepository iTagReleationRepository, ILayerFieldDictRepository iLayerFieldDictRepository,
            IOperateLogAppService iOperateLogAppService)
        {
            _IMapReleationRepository = iMapReleationRepository;
            _ILayerContentRepository = iLayerContentRepository;
            _IDataStyleRepository = iDataStyleRepository;
            _IMapRepository = iMapRepository;
            _IDicDataTypeRepository = iDicDataTypeRepository;
            _IDicDataCodeRepository = iDicDataCodeRepository;
            _ILayerFieldRepository = iLayerFieldRepository;
            _IDataTypeRepository = iDataTypeRepository;
            _IDataTagRepository = iDataTagRepository;
            _ITagReleationRepository = iTagReleationRepository;
            _ILayerFieldDictRepository = iLayerFieldDictRepository;
            _GeoServerIp = ConfigurationManager.AppSettings["GeoServerIp"];
            _GeoServerPort = ConfigurationManager.AppSettings["GeoServerPort"];
            _GeoWorkSpace = ConfigurationManager.AppSettings["GeoWorkSpace"];
            _GeoServer = new GeoServer(_GeoServerIp, _GeoServerPort);
            _PublishAddress = ConfigurationManager.AppSettings["PublishAddress"];
            this._iOperateLogAppService = iOperateLogAppService;
        }

        #region 数据接口
        public byte[] GetProcessFile(string mapName, string tileMatrix, string tileCol, string tileRow)
        {
            if (string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(tileMatrix) || string.IsNullOrEmpty(tileCol) || string.IsNullOrEmpty(tileRow))
            {
                return null;
            }
            else
            {
                var query = _IMapRepository.FirstOrDefault(m => m.MapName == mapName || m.MapName == mapName);
                if (query != null && !string.IsNullOrEmpty(query.MapEnName))
                {
                    string requestUrl = "";
                    requestUrl += string.Format("http://{0}:{1}/geoserver/gwc/service/{2}", _GeoServerIp, _GeoServerPort, "wmts");
                    requestUrl += "?service=WMTS";
                    requestUrl += "&version=1.0.0";
                    requestUrl += "&request=GetTile";
                    requestUrl += "&layer=" + (string.IsNullOrEmpty(_GeoWorkSpace) ? query.MapEnName : (_GeoWorkSpace + ":" + query.MapEnName));
                    requestUrl += "&tileRow=" + tileRow;
                    requestUrl += "&tileCol=" + tileCol;
                    requestUrl += "&tileMatrixSet=iTelluro";
                    requestUrl += "&tileMatrix=" + tileMatrix;
                    requestUrl += "&format=image/png";
                    byte[] buffer = _GeoServer.GetWMTSData(requestUrl);
                    return buffer;
                }

                return null;
            }
        }

        /// <summary>
        /// 根据地图名称查询图层列表
        /// </summary>
        /// <param name="mapName">地图名称</param>
        /// <returns></returns>
        public ListResultOutput<LayerOutputDto> GetLayersByMapName(string mapName)
        {
            try
            {
                var query = from a in _IMapReleationRepository.GetAll()
                            join b in _ILayerContentRepository.GetAll() on a.DataConfigID equals b.Id
                            join d in _IMapRepository.GetAll() on a.MapID equals d.Id
                            join f in _IDicDataCodeRepository.GetAll() on b.DataType equals f.Id into fi
                            from fii in fi.DefaultIfEmpty()
                            join e in _IDataTypeRepository.GetAll() on b.LayerType equals e.Id into ei
                            from eii in ei.DefaultIfEmpty()
                            join c in _IDataStyleRepository.GetAll() on a.DataStyleID equals c.Id into ci
                            from cii in ci.DefaultIfEmpty()
                            where d.MapName == mapName
                            orderby a.DataSort
                            select new LayerOutputDto
                            {
                                LayerId = b.Id,
                                LayerName = b.LayerName,
                                DataType = fii == null ? "" : fii.CodeName,
                                LayerBBox = b.LayerBBox,
                                LayerType = eii == null ? "" : eii.TypeName,
                                LayerTag = b.LayerTag,
                                LayerDesc = b.LayerDesc,
                                LayerRefence = b.LayerRefence,
                                CreateDT = b.CreateDT,
                                DataStyleName = cii == null ? "" : cii.StyleName,
                                DataSort = a.DataSort
                            };
                var list = query.ToList();
                foreach (var item in list)
                {
                    var queryTag = from a in _ITagReleationRepository.GetAll()
                                   join b in _IDataTagRepository.GetAll() on a.DataTagID equals b.Id
                                   where a.MapID == item.LayerId
                                   select new
                                   {
                                       b.TagName
                                   };
                    if (queryTag != null)
                    {
                        StringBuilder str = new StringBuilder();
                        foreach (var model in queryTag)
                        {
                            str.Append(model.TagName);
                            str.Append(",");
                        }
                        if (str.Length > 0)
                        {
                            str.Length = str.Length - 1;
                        }
                        item.LayerTag = str.ToString();
                    }
                }
                var result = new ListResultOutput<LayerOutputDto>(list.MapTo<List<LayerOutputDto>>());

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据图层名称查询图层所有字段
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public ListResultOutput<FieldOutputDto> GetLayerFieldNameList(string layerName)
        {
            var query = from a in _ILayerFieldRepository.GetAll()
                        join b in _ILayerContentRepository.GetAll()
                        on a.LayerID equals b.Id
                        where b.LayerName == layerName
                        orderby a.AttributeName
                        select new FieldOutputDto
                        {
                            FieldName = a.AttributeName,
                            FieldDesc = a.AttributeDesc,
                            FieldType = a.AttributeType,
                            FieldLength = a.AttributeLength,
                            FieldPrecision = a.AttributePrecision,
                            FieldInputCtrl = a.AttributeInputCtrl,
                            FieldInputMax = a.AttributeInputMax,
                            FieldInputMin = a.AttributeInputMin,
                            FieldDefaultValue = a.AttributeDefault,
                            FieldIsNull = a.AttributeIsNull,
                            FieldInputFormat = a.AttributeInputFormat,
                            FieldUnit = a.AttributeUnit,
                            FieldDataType = a.AttributeDataType,
                            FieldValueLink = a.AttributeValueLink,
                            FieldDataSource = a.AttributeDataSource,
                            FieldCalComp = a.AttributeCalComp,
                            FieldSort = a.AttributeSort,
                            Remark = a.Remark
                        };
            var result = new ListResultOutput<FieldOutputDto>(query.MapTo<List<FieldOutputDto>>());

            return result;
        }

        /// <summary>
        /// 根据图层名称查询完整空间数据
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public string GetGeoJsonByLayerName(string layerName)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select *,ST_AsGeoJson({0}) from {1} ", "geom", tableName);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据图层名称查询完整空间数据
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public string GetGeoJsonByLayerName(string mainId,string layerName)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName&&m.MainId==mainId);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select *,ST_AsGeoJson({0}) from {1} ", "geom", tableName);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 根据图层名称查询属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public string GetAttrByLayerName(string layerName)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} ", GetLayerFields(layer.Id), tableName);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据图层名称查询属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public string GetAttrByLayerNameCount(string layerName)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} ", GetLayerFields(layer.Id), tableName);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        string result = "[{" + "\"" + "count" + "\":" + "\"" + dt.Rows.Count + "\"}]";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据图层名称查询属性信息[分页]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        public string GetAttrByLayerNamePage(string layerName, int pageSize, int pageIndex)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where sid > {2} limit {3} ", GetLayerFields(layer.Id), tableName, (pageIndex - 1) * pageSize, pageSize);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据图层名称查询属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public string GetAttrByCondition(string layerName, string condition)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string strWhere = string.Empty;
                    if (!string.IsNullOrEmpty(condition))
                    {
                        string[] arr = condition.Replace("'", "").Replace("\"", "").Replace("{", "").Replace("}", "").Split(',');
                        foreach (string item in arr)
                        {
                            strWhere += " and \"" + item.Split(':')[0] + "\" like " + "'%" + item.Split(':')[1] + "%'";
                        }
                    }
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = "select " + GetLayerFields(layer.Id) + " from " + tableName + " where 1=1 " + strWhere;
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据图层名称查询属性信息(总数)
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public string GetAttrByConditionCount(string layerName, string condition)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string strWhere = string.Empty;
                    if (!string.IsNullOrEmpty(condition))
                    {
                        string[] arr = condition.Replace("'", "").Replace("\"", "").Replace("{", "").Replace("}", "").Split(',');
                        foreach (string item in arr)
                        {
                            strWhere += " and \"" + item.Split(':')[0] + "\" like " + "'%" + item.Split(':')[1] + "%'";
                        }
                    }
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = "select " + GetLayerFields(layer.Id) + " from " + tableName + " where 1=1 " + strWhere;
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        string result = "[{" + "\"" + "count" + "\":" + "\"" + dt.Rows.Count + "\"}]";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据图层名称查询属性信息[分页]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="condition">查询条件</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        public string GetAttrByConditionPage(string layerName, string condition, int pageSize, int pageIndex)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string strWhere = string.Empty;
                    if (!string.IsNullOrEmpty(condition))
                    {
                        string[] arr = condition.Replace("'", "").Replace("\"", "").Replace("{", "").Replace("}", "").Split(',');
                        foreach (string item in arr)
                        {
                            strWhere += " and \"" + item.Split(':')[0] + "\" like " + "'%" + item.Split(':')[1] + "%'";
                        }
                    }
                    string tableName = "public." + layer.LayerAttrTable;
                    string strPage = string.Format(" and sid > {0} limit {1}", (pageIndex - 1) * pageSize, pageSize);
                    string strSQL = "select " + GetLayerFields(layer.Id) + " from " + tableName + " where 1=1 " + strWhere + strPage;
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据点查询图层属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        public string GetAttrByPt(string layerName, float lon, float lat)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(geom,ST_PointFromText('POINT({2} {3})')) ", GetLayerFields(layer.Id), tableName, lon, lat);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据点查询图层属性信息(总数)
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        public string GetAttrByPtCount(string layerName, float lon, float lat)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(geom,ST_PointFromText('POINT({2} {3})')) ", GetLayerFields(layer.Id), tableName, lon, lat);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        string result = "[{" + "\"" + "count" + "\":" + "\"" + dt.Rows.Count + "\"}]";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据点查询图层属性信息[分页]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        public string GetAttrByPtPage(string layerName, float lon, float lat, int pageSize, int pageIndex)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(geom,ST_PointFromText('POINT({2} {3})')) and sid > {4} limit {5}", GetLayerFields(layer.Id), tableName, lon, lat, (pageIndex - 1) * pageSize, pageSize);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据中心点查询图层属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <returns></returns>
        public string GetAttrByPtTolerane(string layerName, float lon, float lat, float distance)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = "select " + GetLayerFields(layer.Id) + " from " + tableName + " where st_distance_sphere(ST_GeomFromText('POINT(" + lon + " " + lat + ")'),geom)<=" + distance;
                    //string strSQL = string.Format("select {0} from {1} where ST_Contains(geom,ST_PointFromText('POINT({2},{3})'),{4})", GetLayerFields(layer.Id), tableName, lon, lat, distance);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据中心点查询图层属性信息(总数)
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <returns></returns>
        public string GetAttrByPtToleraneCount(string layerName, float lon, float lat, float distance)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = "select " + GetLayerFields(layer.Id) + " from " + tableName + " where st_distance_sphere(ST_GeomFromText('POINT(" + lon + " " + lat + ")'),geom)<=" + distance;
                    //string strSQL = string.Format("select {0} from {1} where ST_Contains(geom,ST_PointFromText('POINT({2},{3})'),{4})", GetLayerFields(layer.Id), tableName, lon, lat, distance);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        string result = "[{" + "\"" + "count" + "\":" + "\"" + dt.Rows.Count + "\"}]";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据中心点查询图层属性信息[分页]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        public string GetAttrByPtToleranePage(string layerName, float lon, float lat, float distance, int pageSize, int pageIndex)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strPage = string.Format(" and sid > {0} limit {1}", (pageIndex - 1) * pageSize, pageSize);
                    string strSQL = "select " + GetLayerFields(layer.Id) + " from " + tableName + " where st_distance_sphere(ST_GeomFromText('POINT(" + lon + " " + lat + ")'),geom)<=" + distance + strPage;
                    //string strSQL = string.Format("select {0} from {1} where ST_Contains(geom,ST_PointFromText('POINT({2},{3})'),{4})", GetLayerFields(layer.Id), tableName, lon, lat, distance);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据矩形查询图层属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <returns></returns>
        public string GetAttrByRect(string layerName, float minLon, float minLat, float maxLon, float maxLat)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(ST_MakeEnvelope('{2}','{3}','{4}','{5}'),geom)", GetLayerFields(layer.Id), tableName, minLon, minLat, maxLon, maxLat);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据矩形查询图层属性信息(总数)
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <returns></returns>
        public string GetAttrByRectCount(string layerName, float minLon, float minLat, float maxLon, float maxLat)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(ST_MakeEnvelope('{2}','{3}','{4}','{5}'),geom)", GetLayerFields(layer.Id), tableName, minLon, minLat, maxLon, maxLat);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        string result = "[{" + "\"" + "count" + "\":" + "\"" + dt.Rows.Count + "\"}]";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据矩形查询图层属性信息[分页]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        public string GetAttrByRectPage(string layerName, float minLon, float minLat, float maxLon, float maxLat, int pageSize, int pageIndex)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(ST_MakeEnvelope('{2}','{3}','{4}','{5}'),geom) and sid > {6} limit {7}", GetLayerFields(layer.Id), tableName, minLon, minLat, maxLon, maxLat, (pageIndex - 1) * pageSize, pageSize);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据多边形生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="wkt"></param>
        /// <returns></returns>
        public string GetAttrByNGon(string layerName, string wkt)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(geom,ST_GeomFromText('{2}'))", GetLayerFields(layer.Id), tableName, wkt);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据多边形生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="wkt"></param>
        /// <returns></returns>
        public string GetAttrByNGonCount(string layerName, string wkt)
        {
            try
            {

                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(ST_GeomFromText('{2}'),geom)", GetLayerFields(layer.Id), tableName, wkt);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        string result = "[{" + "\"" + "count" + "\":" + "\"" + dt.Rows.Count + "\"}]";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 根据多边形生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="wkt"></param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页序号</param>
        /// <returns></returns>
        public string GetAttrByNGonPage(string layerName, string wkt, int pageSize, int pageIndex)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(ST_GeomFromText('{2}'),geom) and sid > {3} limit {4}", GetLayerFields(layer.Id), tableName, wkt, (pageIndex - 1) * pageSize, pageSize);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);
                    if (dt != null)
                    {
                        return DataTableConvertJson.DataTable2Json("data", dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据点生成图层文件
        /// </summary>
        /// <param name="layerId">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        public string GetShpFileByPt(string layerName, float lon, float lat)
        {
            string shpFilePath = "";
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    #region [图层属性取值]

                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0},ST_Asewkt(geom) geom from {1} where ST_Contains(geom,ST_PointFromText('POINT({2} {3})'))", GetLayerFields(layer.Id), tableName, lon, lat);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);

                    #endregion

                    if (dt.Rows.Count > 0)
                    {
                        shpFilePath = GetOutputShpFilePath(dt, layer);
                    }
                }
                return shpFilePath;
            }
            catch (Exception ex)
            {
                return shpFilePath;
            }
        }
        /// <summary>
        /// 根据中心点生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <returns></returns>
        public string GetShpFileByPtTolerane(string layerName, float lon, float lat, float distance)
        {
            string shpFilePath = "";
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    #region [图层属性取值]

                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = "select " + GetLayerFields(layer.Id) + " from " + tableName + " where st_distance_sphere(ST_GeomFromText('POINT(" + lon + " " + lat + ")'),geom)<=" + distance;
                    //string strSQL = string.Format("select {0} from {1} where ST_Contains(geom,ST_PointFromText('POINT({2},{3})'),{4})", GetLayerFields(layer.Id), tableName, lon, lat, distance);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);

                    #endregion

                    if (dt.Rows.Count > 0)
                    {
                        shpFilePath = GetOutputShpFilePath(dt, layer);
                    }
                }
                return shpFilePath;
            }
            catch (Exception ex)
            {
                return shpFilePath;
            }
        }
        /// <summary>
        /// 根据矩形生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <returns></returns>
        public string GetShpFileByRect(string layerName, float minLon, float minLat, float maxLon, float maxLat)
        {
            string shpFilePath = "";
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);
                if (layer != null)
                {
                    #region [图层属性取值]

                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0} from {1} where ST_Contains(ST_MakeEnvelope('{2}','{3}','{4}','{5}'),geom)", GetLayerFields(layer.Id), tableName, minLon, minLat, maxLon, maxLat);
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    DataTable dt = actal.GetData(strSQL);

                    #endregion

                    if (dt.Rows.Count > 0)
                    {
                        shpFilePath = GetOutputShpFilePath(dt, layer);
                    }
                }
                return shpFilePath;
            }
            catch (Exception ex)
            {
                return shpFilePath;
            }
        }

        /// <summary>
        /// 获取图层输出文件的路径
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public string GetOutputShpFilePath(DataTable dt, Core.Entities.LayerContentEntity layer)
        {
            string shpFilePath = "";
            if (dt.Rows.Count > 0)
            {
                #region [坐标参考]

                string wktReference = String.Empty;
                FindReferenceFile(ConfigurationManager.AppSettings["CoordPath"].ToString(), ref wktReference);

                #endregion

                #region [图层属性查询]

                var layerAttr = _ILayerFieldRepository.GetAll().Where(q => q.LayerID == layer.Id);
                var attrType = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "73160096-67a5-11e7-8eb2-005056bb1c7e");
                var query = (from l in layerAttr
                             join t in attrType on l.AttributeType equals t.Id into tt
                             from de in tt.DefaultIfEmpty()
                             select new
                             {
                                 AttributeName = l.AttributeName,
                                 AttributeType = (de == null) ? "" : de.CodeValue,
                                 AttributeLength = l.AttributeLength,
                                 AttributePrecision = l.AttributePrecision
                             });

                var result = query.ToList();

                #endregion

                #region [图层属性赋对象]

                List<AttributeModel> lstattr = new List<AttributeModel>();

                if (result.Count > 0)
                {
                    int index = 0;
                    foreach (var item in result)
                    {
                        index++;
                        AttributeModel attrm = new AttributeModel();
                        attrm.AttributeName = item.AttributeName;
                        attrm.AttributeType = Utility.DataTypeToGdalType(item.AttributeType);
                        attrm.AttributeWidth = (string.IsNullOrEmpty(item.AttributeLength)) ? 0 : int.Parse(item.AttributeLength);
                        attrm.AttributePrecision = (string.IsNullOrEmpty(item.AttributePrecision)) ? 0 : int.Parse(item.AttributePrecision);
                        attrm.AttributeApproxOK = index;
                        lstattr.Add(attrm);
                    }
                }

                #endregion

                #region [图层属性赋值]

                List<string> lstGeom = new List<string>();
                List<AttributeObj> lstAttrObj = new List<AttributeObj>();
                int fid = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    lstGeom.Add(dr["geom"].ToString());

                    AttributeObj attObj = new AttributeObj();
                    Dictionary<string, string> dicAttValue = new Dictionary<string, string>();
                    foreach (AttributeModel attributeM in lstattr)
                    {
                        attObj.OId = fid;
                        string cloumnName = attributeM.AttributeName;
                        string rowValue = dr[cloumnName].ToString().Trim();
                        dicAttValue.Add(cloumnName, rowValue.ToString());
                        fid++;
                    }
                    attObj.AttributeValue = dicAttValue;
                    lstAttrObj.Add(attObj);
                }

                #endregion

                #region [文件生成]

                string outFilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString() + ConfigurationManager.AppSettings["DownloadFile"].ToString()), "LayerShpFileExport");

                if (!Directory.Exists(outFilePath))
                {
                    Directory.CreateDirectory(outFilePath);
                }

                string layerTableName = layer.LayerAttrTable;

                string geoType = _IDicDataCodeRepository.Get(layer.DataType).CodeValue.ToUpper();
                geoType = (geoType != null) ? geoType : "";

                #region [创建图层文件夹]

                string layerFilePath = Path.Combine(outFilePath, layerTableName);
                if (!Directory.Exists(layerFilePath))
                {
                    Directory.CreateDirectory(layerFilePath);
                }

                #endregion

                string outFileFullname = Path.Combine(layerFilePath, layerTableName + ".shp");

                if (File.Exists(outFileFullname))
                {
                    File.Delete(Path.Combine(layerFilePath, layerTableName + ".shp"));
                    File.Delete(Path.Combine(layerFilePath, layerTableName + ".prj"));
                    File.Delete(Path.Combine(layerFilePath, layerTableName + ".dbf"));
                    File.Delete(Path.Combine(layerFilePath, layerTableName + ".shx"));
                }

                ShpWriter shpWriter = new ShpWriter(outFileFullname);
                bool success = shpWriter.DoExport(lstattr, Utility.GetGeoTypeByString(geoType), lstGeom, lstAttrObj, wktReference);

                if (success)
                {
                    string outFile = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, ConfigurationManager.AppSettings["DownloadFile"], "LayerShpFileExport");
                    string filePath = Path.Combine(outFile, layerTableName);
                    ZipHelper zip = new ZipHelper();
                    success = zip.ZipDir(filePath, filePath, 9);

                    if (success)
                    {
                        shpFilePath = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/" + ConfigurationManager.AppSettings["DownloadFile"].ToString() + "/" + "LayerShpFileExport" + "/" + layerTableName + ".zip";
                    }
                }

                #endregion
            }

            return shpFilePath;
        }

        /// <summary>
        /// 查找参考配置信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void FindReferenceFile(string path, ref string wktReference)
        {
            if (String.IsNullOrEmpty(wktReference))
            {
                string fileName = ConfigurationManager.AppSettings["SpatialRefenceFile"].ToString() + ".prj";
                DirectoryInfo fileFolder = new DirectoryInfo(@path);



                foreach (FileInfo nextFile in fileFolder.GetFiles())
                {
                    if (nextFile.Name == fileName)
                    {
                        wktReference = ReadFile(nextFile.FullName);
                        break;
                    }
                }

                if (String.IsNullOrEmpty(wktReference))
                {
                    foreach (DirectoryInfo NextFolder in fileFolder.GetDirectories())
                    {
                        FindReferenceFile(NextFolder.FullName, ref wktReference);
                    }

                }
            }
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public string ReadFile(string fileName)
        {
            string wktReference = "";
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                wktReference = sr.ReadLine();
                //while(wktReference != null)
                //{
                //    wktReference = sr.ReadLine();
                //}
                sr.Close();
                return wktReference;
            }
            catch (IOException ex)
            {
                return "";
            }
        }

        string GetLayerFields(string layerId)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                var query = from a in _ILayerContentRepository.GetAll()
                            join b in _ILayerFieldRepository.GetAll()
                            on a.Id equals b.LayerID
                            where a.Id == layerId
                            select new
                            {
                                b.AttributeName
                            };
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        str.Append("\"");
                        str.Append(item.AttributeName);
                        str.Append("\"");
                        str.Append(",");
                    }
                    if (str.Length > 0)
                    {
                        str.Length = str.Length - 1;
                    }
                }
                return str.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 导入图层json数据
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="dataType">数据操作类型</param>
        /// <param name="layerData">json图层数据</param>
        /// <returns>无异常返回空，有异常返回异常字符信息</returns>
        public string LayerDataImport(dynamic obje)
        {
            //string express = "1 * 5";
            //CSScriptHelper css = new CSScriptHelper();
            //object res = css.Calutrue(express);

            string strMultiSid = string.Empty;
            string layerName = Convert.ToString(obje.layerName), dataType = Convert.ToString(obje.dataType);
            DataTable dtData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(Convert.ToString(obje.layerData));
            string msg = string.Empty, resultMsg = string.Empty;
            bool status = true;
            try
            {

                #region [获取图层信息]

                var layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerName == layerName);

                var layerField = (from a in _ILayerContentRepository.GetAll()
                                  join b in _ILayerFieldRepository.GetAll()
                                  on a.Id equals b.LayerID
                                  where a.LayerName == layerName
                                  select new
                                  {
                                      Id = b.Id,
                                      LayerID = b.LayerID,
                                      AttributeName = b.AttributeName,
                                      AttributeDesc = b.AttributeDesc,
                                      AttributeType = b.AttributeType,
                                      AttributeLength = b.AttributeLength,
                                      AttributePrecision = b.AttributePrecision,
                                      AttributeInputCtrl = b.AttributeInputCtrl,
                                      AttributeInputMax = b.AttributeInputMax,
                                      AttributeInputMin = b.AttributeInputMin,
                                      AttributeDefault = b.AttributeDefault,
                                      AttributeIsNull = b.AttributeIsNull,
                                      AttributeInputFormat = b.AttributeInputFormat,
                                      AttributeUnit = b.AttributeUnit,
                                      AttributeCalComp = b.AttributeCalComp,
                                      AttributeDataSource = b.AttributeDataSource,
                                      AttributeDataType = b.AttributeDataType,
                                      AttributeValueLink = b.AttributeValueLink,
                                      Remark = b.Remark,
                                      AttributeSort = b.AttributeSort,
                                      CreateDT = b.CreateDT
                                  }).ToList();

                var fieldDict = _ILayerFieldDictRepository.GetAllList();

                Dictionary<string, decimal?> bbox = new Dictionary<string, decimal?>();
                bbox["MaxX"] = layer.MaxX;
                bbox["MaxY"] = layer.MaxY;
                bbox["MinX"] = layer.MinX;
                bbox["MinY"] = layer.MinY;

                #endregion

                #region [数据检验入库]


                if (dtData.Rows.Count > 0)
                {
                    //验证字段匹配
                    foreach (var item in layerField)
                    {
                        if (!dtData.Columns.Contains(item.AttributeName))
                        {
                            msg += "[" + item.AttributeName.ToString() + "]:" + "属性字段不匹配,";
                        }
                    }

                    string columnGeom = "geom", columnSid = "sid";
                    if (((dtData.Columns.Count != layerField.Count + 1) || !dtData.Columns.Contains(columnGeom)) && (dataType == "insert"))
                    {
                        msg += "[属性字段]:" + "属性字段个数不匹配,";
                    }
                    else if (((dtData.Columns.Count != layerField.Count + 2) || !dtData.Columns.Contains(columnGeom) || !dtData.Columns.Contains(columnSid)) && (dataType == "update"))
                    {
                        msg += "[属性字段]:" + "属性字段个数不匹配(须包含sid和geom),";
                    }
                    else if (!dtData.Columns.Contains(columnSid) && dataType == "delete")
                    {
                        msg += "[属性字段]:" + "属性字段不包含关键sid字段,";
                    }

                    if (string.IsNullOrEmpty(msg))
                    {
                        //MySqlHelper mysql = new MySqlHelper();
                        PostgrelVectorHelper postgis = new PostgrelVectorHelper();
                        string mySQLInsert1 = string.Empty;
                        string mySQLInsert2 = string.Empty;
                        string postGISSQL = string.Empty;
                        string mySQLDelete1 = string.Empty;
                        string mySQLDelete2 = string.Empty;
                        string postGISDelete = string.Empty;

                        int maxNum = 0;
                        int _count = 500;

                        string sqlStr = String.Format("select Max(SID) from {0}", layer.LayerAttrTable);
                        object obj = postgis.GetExecuteScalar(sqlStr);
                        maxNum = ((obj is System.DBNull) ? 0 : Convert.ToInt32(obj)) + 1;

                        for (int i = 0; i < dtData.Rows.Count; i++)
                        {
                            msg = "";
                            foreach (var item in layerField)
                            {
                                string fieldValue = dtData.Rows[i][item.AttributeName].ToString();

                                #region [数据类型验证]

                                if (fieldValue.Length > int.Parse(item.AttributeLength))
                                {
                                    msg += "[" + item.AttributeName + "]:" + "属性字段长度不匹配,";
                                }

                                int intValue;
                                float floatValue;
                                double doubleValue;
                                DateTime dtValue;
                                bool result = true;
                                switch (item.AttributeType)
                                {
                                    case "673e95ba-67a8-11e7-8eb2-005056bb1c7e":
                                        result = int.TryParse(fieldValue, out intValue);
                                        break;//长整型
                                    case "7c6aa917-67a8-11e7-8eb2-005056bb1c7e":
                                        result = int.TryParse(fieldValue, out intValue);
                                        break;//短整型
                                    case "8f553741-67a8-11e7-8eb2-005056bb1c7e":
                                        result = float.TryParse(fieldValue, out floatValue);
                                        break;//单浮点型
                                    case "9ffd11ea-67a8-11e7-8eb2-005056bb1c7e":
                                        result = double.TryParse(fieldValue, out doubleValue);
                                        break;//双浮点型
                                    case "ca945a49-67a8-11e7-8eb2-005056bb1c7e":
                                        result = DateTime.TryParse(fieldValue, out dtValue);
                                        break;//时间型
                                    case "afd042e0-67a8-11e7-8eb2-005056bb1c7e":
                                        result = true;
                                        break;
                                    default:
                                        result = false;
                                        break;//字符型
                                }

                                if (!result)
                                {
                                    msg += "[" + item.AttributeName + "]:" + "属性字段数据类型不匹配,";
                                }

                                #endregion

                                #region [数据规则验证]

                                if (dataType != "delete")
                                {


                                    if (!string.IsNullOrEmpty(item.AttributeIsNull.ToString()) || !string.IsNullOrEmpty(item.AttributeInputCtrl.ToString()) || !string.IsNullOrEmpty(item.AttributeInputFormat.ToString()) || !string.IsNullOrEmpty(item.AttributeDataType.ToString()) || (!string.IsNullOrEmpty(item.AttributeInputMax.ToString()) && !string.IsNullOrEmpty(item.AttributeInputMin.ToString()))
                                        || !string.IsNullOrEmpty(item.AttributeCalComp))
                                    {
                                        string dataTypeString = "673e95ba-67a8-11e7-8eb2-005056bb1c7e,7c6aa917-67a8-11e7-8eb2-005056bb1c7e,8f553741-67a8-11e7-8eb2-005056bb1c7e,9ffd11ea-67a8-11e7-8eb2-005056bb1c7e";
                                        if (!string.IsNullOrEmpty(item.AttributeIsNull.ToString()) && item.AttributeIsNull.ToString() == "F" && string.IsNullOrEmpty(fieldValue))
                                        {
                                            msg += "[" + item.AttributeName + "]" + ":与属性验证[空值]不能为空不符" + ",";
                                        }
                                        else if (!string.IsNullOrEmpty(item.AttributeDataType.ToString()) && item.AttributeDataType.ToString() == "T" && !string.IsNullOrEmpty(fieldValue))
                                        {
                                            var lstDict = fieldDict.Where(q => q.AttributeID == item.Id).ToList();
                                            int count = 0;

                                            if (!string.IsNullOrEmpty(item.AttributeInputFormat.ToString()) && item.AttributeInputFormat.ToString() == "S")
                                            {
                                                foreach (var dic in lstDict)
                                                {
                                                    if (dic.FieldDictName == fieldValue)
                                                    {
                                                        count++;
                                                    }
                                                }

                                                if (count != 1)
                                                {
                                                    msg += "[" + item.AttributeName + "]" + ":与属性验证[输入格式]单选不符" + ",";
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(item.AttributeInputFormat.ToString()) && item.AttributeInputFormat.ToString() == "M")
                                            {
                                                string[] value = fieldValue.Replace(",", ";").Replace("，", ";").Replace("；", ";").TrimEnd(';').Split(';');
                                                foreach (var dic in lstDict)
                                                {
                                                    for (int j = 0; j < value.Length; j++)
                                                    {
                                                        if (dic.FieldDictName.ToString() == value[j])
                                                        {
                                                            count++;
                                                        }
                                                    }
                                                }

                                                if (count <= 1 || count != value.Length)
                                                {
                                                    msg += "[" + item.AttributeName + "]" + ":与属性验证[输入格式]多选不符" + ",";
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(item.AttributeInputFormat.ToString()) && item.AttributeInputFormat.ToString() == "")
                                            {
                                                string[] value = fieldValue.Replace(",", ";").Replace("，", ";").Replace("；", ";").TrimEnd(';').Split(';');
                                                foreach (var dic in lstDict)
                                                {
                                                    for (int j = 0; j < value.Length; j++)
                                                    {
                                                        if (dic.FieldDictName.ToString() == value[j])
                                                        {
                                                            count++;
                                                        }
                                                    }
                                                }

                                                if (count == 0 || count != value.Length)
                                                {
                                                    msg += "[" + item.AttributeName + "]" + ":与属性验证[输入格式]不限制不符" + ",";
                                                }
                                            }
                                        }
                                        else if ((string.IsNullOrEmpty(item.AttributeDataType.ToString()) || item.AttributeDataType.ToString() == "F") && (!string.IsNullOrEmpty(item.AttributeInputMax.ToString()) && !string.IsNullOrEmpty(item.AttributeInputMin.ToString()))
                                    && dataTypeString.Contains(item.AttributeType.ToString()))
                                        {
                                            string intType = "673e95ba-67a8-11e7-8eb2-005056bb1c7e,7c6aa917-67a8-11e7-8eb2-005056bb1c7e";
                                            string floatType = "8f553741-67a8-11e7-8eb2-005056bb1c7e,9ffd11ea-67a8-11e7-8eb2-005056bb1c7e";
                                            if (intType.Contains(item.AttributeType.ToString()))
                                            {
                                                if (int.Parse(fieldValue) < int.Parse(item.AttributeInputMin.ToString()) || int.Parse(fieldValue) > int.Parse(item.AttributeInputMax.ToString()))
                                                {
                                                    msg += "[" + item.AttributeName + "]" + ":与属性验证[值域上限与值域下限]值不符" + ",";
                                                }
                                            }
                                            else if (floatType.Contains(item.AttributeType.ToString()))
                                            {
                                                if (decimal.Parse(fieldValue) < decimal.Parse(item.AttributeInputMin.ToString()) || decimal.Parse(fieldValue) > decimal.Parse(item.AttributeInputMax.ToString()))
                                                {
                                                    msg += "[" + item.AttributeName + "]" + ":与属性验证[值域上限与值域下限]值不符" + ",";
                                                }
                                            }
                                        }
                                        else if (!string.IsNullOrEmpty(item.AttributeCalComp) && !string.IsNullOrEmpty(fieldValue))
                                        {
                                            string express = item.AttributeCalComp;

                                            foreach (var field in layerField)
                                            {
                                                express = express.Replace("{" + field.AttributeName + "}", dtData.Rows[i][field.AttributeName].ToString());
                                            }

                                            CSScriptHelper css = new CSScriptHelper();
                                            object res = css.Calutrue(express);

                                            if (!fieldValue.Equals(res.ToString()))
                                            {
                                                if (res.ToString().Contains("#"))
                                                {
                                                    msg += "[" + item.AttributeName + "]" + ":与属性验证[公式]公式有异常" + ",";
                                                }
                                                else
                                                {
                                                    msg += "[" + item.AttributeName + "]" + ":与属性验证[公式]匹配值不符" + ",";
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }

                            #region [数据入库]

                            if (string.IsNullOrEmpty(msg))
                            {
                                #region [组SQL串]

                                string colStr1 = string.Empty;
                                string colStr2 = string.Empty;
                                string valueStr = string.Empty;

                                foreach (var item in layerField)
                                {
                                    string rowValue = dtData.Rows[i][item.AttributeName].ToString();
                                    colStr1 += "`" + item.AttributeName + "`" + ",";
                                    colStr2 += "\"" + item.AttributeName + "\"" + ",";
                                    if (item.AttributeType.ToString() == "afd042e0-67a8-11e7-8eb2-005056bb1c7e")
                                    {
                                        if (rowValue.Contains("'"))
                                        {
                                            string newValue = rowValue.Replace("'", "''");
                                            valueStr += String.Format("'{0}',", newValue);
                                        }
                                        else
                                        {
                                            valueStr += String.Format("'{0}',", rowValue);
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(rowValue))
                                        {
                                            valueStr += String.Format("{0},", rowValue);
                                        }
                                        else
                                        {
                                            valueStr += String.Format("'{0}',", rowValue);
                                        }
                                    }
                                }

                                colStr1 = colStr1.TrimEnd(',');
                                colStr2 = colStr2.TrimEnd(',');
                                valueStr = valueStr.TrimEnd(',');

                                #endregion

                                #region [MYSQL]

                                string mySQLGeomStr = String.Format("GEOMFROMTEXT('{0}')", dtData.Rows[i]["geom"].ToString());
                                string sid1 = (dataType != "insert") ? dtData.Rows[i]["sid"].ToString() : Guid.NewGuid().ToString();
                                string sid2 = Guid.NewGuid().ToString();
                                mySQLInsert1 += string.Format("('{0}',{1}),", sid1, valueStr);
                                mySQLInsert2 += string.Format("('{0}','{1}',{2}),", sid2, sid1, mySQLGeomStr);



                                if (dataType != "insert")
                                {
                                    mySQLDelete1 += string.Format("delete from {0} where sid='{1}' ;", layer.LayerAttrTable, dtData.Rows[i]["sid"].ToString());
                                    mySQLDelete2 += string.Format("delete from {0} where DataID='{1}' ;", layer.LayerSpatialTable, dtData.Rows[i]["sid"].ToString());
                                }

                                #endregion

                                #region [POSTGIS]

                                string sid = string.Format("{0}", maxNum + i);
                                string geomStr = String.Format("'{0}'", dtData.Rows[i]["geom"].ToString());

                                postGISSQL += string.Format("({0},'{1}',{2},{3}),", sid, sid1, geomStr, valueStr);

                                if (dataType != "insert")
                                {
                                    postGISDelete += string.Format("delete from public.{0} where guid='{1}' ;", layer.LayerAttrTable, dtData.Rows[i]["sid"].ToString());
                                }

                                #endregion

                                strMultiSid += sid + ",";

                                #region [批量处理]

                                if ((i % _count == 0) || (i == dtData.Rows.Count - 1))
                                {
                                    #region [POSTGIS]

                                    if (dataType != "insert")
                                    {
                                        postgis.ExceuteSQL(postGISDelete, string.Empty);
                                        postGISDelete = string.Empty;
                                    }

                                    if (dataType != "delete")
                                    {
                                        postGISSQL = postGISSQL.TrimEnd(',');
                                        string sqlInsert = String.Format("insert into {0}(sid,guid,geom,{1}) values{2}", layer.LayerAttrTable, colStr2, postGISSQL);

                                        postgis.ExceuteSQL(sqlInsert, string.Empty);
                                        postGISSQL = string.Empty;

                                    }

                                    #endregion

                                    #region [MYSQL]

                                    if (dataType != "insert")
                                    {
                                        //mysql.ExecuteNonQuery(mySQLDelete1);
                                        mySQLDelete1 = string.Empty;

                                        //postgis.ExceuteSQL(mySQLDelete2, string.Empty);
                                        //mysql.ExecuteNonQuery(mySQLDelete2);
                                        mySQLDelete2 = string.Empty;
                                    }

                                    if (dataType != "delete")
                                    {
                                        mySQLInsert1 = mySQLInsert1.TrimEnd(',');
                                        mySQLInsert2 = mySQLInsert2.TrimEnd(',');
                                        string sqlInsert1 = String.Format("insert into `{0}`(`sid`,{1}) values{2}", layer.LayerAttrTable, colStr1, mySQLInsert1);
                                        string sqlInsert2 = String.Format("insert into `{0}`(`sid`,`DataID`,`geom`) values{1}", layer.LayerSpatialTable, mySQLInsert2);

                                        //mysql.ExecuteNonQuery(sqlInsert1);
                                        mySQLInsert1 = string.Empty;

                                        //postgis.ExceuteSQL(sqlInsert2, string.Empty);
                                        //mysql.ExecuteNonQuery(sqlInsert2);
                                        mySQLInsert2 = string.Empty;
                                    }

                                    #endregion
                                }

                                #endregion
                            }

                            #endregion
                        }
                    }
                }

                #endregion

                #region [发布更新]

                if (string.IsNullOrEmpty(msg) && dataType != "delete")
                {
                    GeoServerHelper geoServerHelper = new GeoServerHelper();

                    #region [BBox更新]

                    PostgrelVectorHelper postgis = new PostgrelVectorHelper();
                    string strSQL = string.Format("select max(ST_XMax(geom)),max(ST_YMax(geom)),min(ST_XMin(geom)),min(ST_YMin(geom)) from public.{0}", layer.LayerAttrTable);
                    DataTable dt = postgis.getDataTable(strSQL);

                    layer.UploadStatus = "1";
                    if (dt.Rows.Count > 0)
                    {
                        layer.MaxX = (!string.IsNullOrEmpty(dt.Rows[0][0].ToString())) ? Convert.ToDecimal(dt.Rows[0][0]) : 0;
                        layer.MaxY = (!string.IsNullOrEmpty(dt.Rows[0][1].ToString())) ? Convert.ToDecimal(dt.Rows[0][1]) : 0;
                        layer.MinX = (!string.IsNullOrEmpty(dt.Rows[0][2].ToString())) ? Convert.ToDecimal(dt.Rows[0][2]) : 0;
                        layer.MinY = (!string.IsNullOrEmpty(dt.Rows[0][3].ToString())) ? Convert.ToDecimal(dt.Rows[0][3]) : 0;
                    }
                    //因为图层早已发布，此接口只为数据新增，无须重复发布
                    //geoServerHelper.PublicLayer(layer.LayerAttrTable, layer.LayerName, null, "");

                    var result2 = _ILayerContentRepository.Update(layer);

                    string strbbox = "";

                    if (layer.MinX != null)
                    {
                        strbbox = layer.MinX.ToString() + "," + layer.MinY.ToString() + "," + layer.MaxX.ToString() + "," + layer.MaxY.ToString();
                        string bboxStr = string.Format("{0},{1},{2},{3}", layer.MinX, layer.MinY, layer.MaxX, layer.MaxY);
                        geoServerHelper.ModifyLayerBBox(layer.LayerAttrTable, layer.LayerName, bboxStr);
                    }

                    #endregion

                    #region [变更部分BBox]

                    strMultiSid = strMultiSid.TrimEnd(',');
                    string strSQL2 = string.Format("select max(ST_XMax(geom)),max(ST_YMax(geom)),min(ST_XMin(geom)),min(ST_YMin(geom)) from public.{0} where sid in ({1})", layer.LayerAttrTable, strMultiSid);
                    DataTable dtReturn = postgis.getDataTable(strSQL2);

                    layer.UploadStatus = "1";
                    if (dtReturn.Rows.Count > 0)
                    {
                        bbox["MaxX"] = (!string.IsNullOrEmpty(dtReturn.Rows[0][0].ToString())) ? Convert.ToDecimal(dtReturn.Rows[0][0]) : 0;
                        bbox["MaxY"] = (!string.IsNullOrEmpty(dtReturn.Rows[0][1].ToString())) ? Convert.ToDecimal(dtReturn.Rows[0][1]) : 0;
                        bbox["MinX"] = (!string.IsNullOrEmpty(dtReturn.Rows[0][2].ToString())) ? Convert.ToDecimal(dtReturn.Rows[0][2]) : 0;
                        bbox["MinY"] = (!string.IsNullOrEmpty(dtReturn.Rows[0][3].ToString())) ? Convert.ToDecimal(dtReturn.Rows[0][3]) : 0;
                    }

                    #endregion

                    #region [缩略图下载]

                    ThumbnailHelper tbh = new ThumbnailHelper();
                    string imagePath = tbh.CreateThumbnail(layer.LayerAttrTable, "layer", strbbox);

                    #endregion

                    #region [地图刷缓存]

                    string layerID = layer.Id;
                    var releation = _IMapReleationRepository.GetAll();
                    var mapList = (from a in _ILayerContentRepository.GetAll()
                                   join b in _IMapReleationRepository.GetAll()
                                   on a.Id equals b.DataConfigID
                                   join c in _IMapRepository.GetAll()
                                   on b.MapID equals c.Id
                                   where a.Id == layerID && a.UploadStatus == "1"
                                   select new
                                   {
                                       MapEnName = c.MapEnName
                                   }).ToList();

                    GeoServerHelper geoHelp = new GeoServerHelper();

                    foreach (var map in mapList)
                    {
                        string strBbox = bbox["MinX"].ToString() + "," + bbox["MinY"].ToString() + "," + bbox["MaxX"].ToString() + "," + bbox["MaxY"].ToString();
                        geoHelp.TileMap(map.MapEnName, strBbox);
                    }

                    #endregion
                }

                #endregion

                return msg;
            }
            catch (Exception ex)
            {
                msg = "导入图层json数据发生异常";
            }

            if (!string.IsNullOrEmpty(msg))
                status = false;

            resultMsg = "[{" + "\"" + "status" + "\":" + "\"" + status + "\",\"" + "message" + "\":\"" + msg + "\"}]";
            return resultMsg;
        }

        #endregion

        #region 服务接口

        #region 服务
        /// <summary>
        /// 查询提供的服务接口
        /// </summary>
        /// <param name="mapIds">地图编号</param>
        /// <param name="serverType">服务类型</param>
        /// <returns></returns>
        public string GetServerInterfaceDictionaryByTypes(string mapIds, string serverTypes)
        {
            Dictionary<string, List<ServerInterfaceOutputDto>> dictionaryMapServer = new Dictionary<string, List<ServerInterfaceOutputDto>>();
            try
            {
                if (!string.IsNullOrWhiteSpace(mapIds) && !string.IsNullOrWhiteSpace(serverTypes))
                {
                    var maps = mapIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var query = _IMapRepository.GetAllList(m => maps.Contains(m.Id));
                    if (query != null && query.Count > 0)
                    {
                        var queryMapEnNames = query.Select(s => s.MapEnName).ToArray();
                        var idNameDictionary = query.ToDictionary(k => k.MapEnName, v => v.Id);
                        var queryDictionary = query.ToDictionary(k => k.Id, v => v);
                        var queryGroupDictionary = _GeoServer.GetLayerGroups(_GeoWorkSpace).Where(m => queryMapEnNames.Contains(m.Name)).ToDictionary(k => idNameDictionary[k.Name], v => v);

                        var queryLayers = from a in _IMapReleationRepository.GetAll()
                                          join b in _ILayerContentRepository.GetAll()
                                          on a.DataConfigID equals b.Id
                                          where mapIds.Contains(a.MapID)
                                          select new
                                          {
                                              a.MapID,
                                              b.LayerAttrTable
                                          };

                        Dictionary<string, List<string>> layerDictionary = null;
                        if (queryLayers != null && queryLayers.Count() > 0)
                        {
                            layerDictionary = queryLayers.GroupBy(s => s.MapID, v => v.LayerAttrTable).ToDictionary(k => k.Key, v => v.ToList());
                        }

                        List<ServerInterfaceOutputDto> mapList = null;
                        List<ServerInterfaceOutputDto> list = null;
                        var types = serverTypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var map in maps)
                        {
                            try
                            {
                                mapList = new List<ServerInterfaceOutputDto>();
                                foreach (var type in types)
                                {
                                    try
                                    {
                                        list = new List<ServerInterfaceOutputDto>();
                                        GetListByType(
                                            map,
                                            type,
                                            queryDictionary.ContainsKey(map) ? queryDictionary[map] : null,
                                            queryGroupDictionary.ContainsKey(map) ? queryGroupDictionary[map] : null,
                                            layerDictionary.ContainsKey(map) ? layerDictionary[map] : null,
                                            list);
                                        if (list != null)
                                        {
                                            mapList.Add(list.FirstOrDefault());
                                        }
                                    }
                                    catch { }
                                }
                                dictionaryMapServer.Add(map, mapList);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dictionaryMapServer = new Dictionary<string, List<ServerInterfaceOutputDto>>();
            }
            return dictionaryMapServer.ToJson();
        }

        private void GetListByType(string map, string type, MapEntity mapEntity, LayerGroup LayerGroup, List<string> layer, List<ServerInterfaceOutputDto> list)
        {
            switch (type.ToUpper())
            {
                case "WMS":
                    GetWMSInfo(map, mapEntity, LayerGroup, list);
                    break;
                case "WMTS":
                    GetWMTSInfo(map, mapEntity, LayerGroup, list);
                    break;
                case "ITELLURO":
                    GetiTelluroInfo(map, mapEntity, LayerGroup, list);
                    break;
                case "WFS":
                    GetWFSInfo(map, mapEntity, LayerGroup, layer, list);
                    break;
                case "KML":
                    GetKMLInfo(map, mapEntity, LayerGroup, list);
                    break;
            }
        }

        /// <summary>
        /// 查询提供的服务接口
        /// </summary>
        /// <param name="mapId">地图编号</param>
        /// <param name="serverType">服务类型</param>
        /// <returns></returns>
        public ListResultOutput<ServerInterfaceOutputDto> GetServerInterface(string mapId, string serverType)
        {

            List<ServerInterfaceOutputDto> list = new List<ServerInterfaceOutputDto>();
            if (!string.IsNullOrEmpty(mapId) && !string.IsNullOrEmpty(serverType))
            {
                var query = _IMapRepository.FirstOrDefault(m => m.Id == mapId);
                var queryGroup = _GeoServer.GetLayerGroups(_GeoWorkSpace).SingleOrDefault(m => m.Name == query.MapEnName);

                List<string> layer = null;
                if (serverType.ToUpper() == "WFS")
                {
                    var queryLayers = from a in _IMapReleationRepository.GetAll()
                                      join b in _ILayerContentRepository.GetAll()
                                      on a.DataConfigID equals b.Id
                                      where mapId.Equals(a.MapID)
                                      select new
                                      {
                                          b.LayerAttrTable
                                      };

                    if (queryLayers != null && queryLayers.Count() > 0)
                    {
                        layer = queryLayers.Select(s => s.LayerAttrTable).ToList();
                    }
                }

                GetListByType(mapId, serverType, query, queryGroup, layer, list);
            }
            var result = new ListResultOutput<ServerInterfaceOutputDto>(list.MapTo<List<ServerInterfaceOutputDto>>());
            return result;
        }

        void GetWMSInfo(string mapId, MapEntity query, LayerGroup queryGroup, List<ServerInterfaceOutputDto> list)
        {
            ServerInterfaceOutputDto modelServer = null;
            List<ParameterOutputDto> listParameter = null;
            ParameterOutputDto modelParameter = null;

            #region GetMap
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetMap";
            modelServer.ServerDesc = UtilityMessageConvert.Get("返回地图数据");
            modelServer.RequestType = "GET";
            modelServer.ServerPath = string.Format("http://{0}:{1}/geoserver[WORKSPACE]/{2}", _GeoServerIp, _GeoServerPort, "wms");
            modelServer.ServerPath = modelServer.ServerPath.Replace("[WORKSPACE]", string.IsNullOrEmpty(_GeoWorkSpace) ? "" : ("/" + _GeoWorkSpace));
            modelServer.ServerSort = 2;
            modelServer.ServerKeyWord = "WMS";

            listParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "service";
            modelParameter.ParameterValue = "WMS";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("服务类型");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "?service=WMS";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "version";
            modelParameter.ParameterValue = "1.0.0, 1.1.0, 1.1.1, 1.3.0, 2.0.0";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("版本号");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 2;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&version=1.1.0";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "request";
            modelParameter.ParameterValue = "GetMap";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("请求类型");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 3;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&request=GetMap";

            if (query != null && !string.IsNullOrEmpty(query.MapEnName))
            {

                modelParameter = new ParameterOutputDto();
                modelParameter.ParameterName = "layers";
                modelParameter.ParameterValue = string.IsNullOrEmpty(_GeoWorkSpace) ? query.MapEnName : (_GeoWorkSpace + ":" + query.MapEnName);
                modelParameter.ParameterDesc = UtilityMessageConvert.Get("地图名称");
                modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                modelParameter.ParameterSort = 4;
                listParameter.Add(modelParameter);
                modelServer.ServerPath += "&layers=" + modelParameter.ParameterValue;

                if (queryGroup != null)
                {
                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "bbox";
                    modelParameter.ParameterValue = string.Format("{0},{1},{2},{3}", queryGroup.BoundSet.Minx, queryGroup.BoundSet.Miny, queryGroup.BoundSet.Maxx, queryGroup.BoundSet.Maxy);
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("边界范围");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 6;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&bbox=" + modelParameter.ParameterValue;

                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "srs";
                    modelParameter.ParameterValue = queryGroup.BoundSet.Crs.ToString();
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("坐标参考系");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 7;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&srs=" + modelParameter.ParameterValue;
                }
            }

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "styles";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("空代表默认样式");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("地图样式");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 5;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&styles=";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "width";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("正整数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("宽度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 8;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&width=768";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "height";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("正整数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("高度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 9;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&height=670";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "format";
            modelParameter.ParameterValue = "image/png, image/png8, image/jpeg, image/vnd.jpeg-png, image/gif, image/tiff, image/tiff88, image/geotiff, image/geotiff8, image/svg, application/pdf, rss, kml, kmz, application/openlayers, application/json;type=utgrid";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("返回格式");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 10;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&format=image/png";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "transparent";
            modelParameter.ParameterValue = string.Format("true, false（{0}：false）", UtilityMessageConvert.Get("默认"));
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("是否透明");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 11;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "bgcolor";
            modelParameter.ParameterValue = string.Format("RRGGBB（{0}：FFFFFF）", UtilityMessageConvert.Get("默认"));
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("背景颜色");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 12;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "exceptions";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("文件格式（默认：application/vnd.ogc.se_xml）");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("异常文件格式");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 13;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "time";
            modelParameter.ParameterValue = "yyyy-MM-ddThh:mm:ss.SSSZ";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("日期");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 14;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "sld";
            modelParameter.ParameterValue = "URL";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("样式文件地址");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 15;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "sld_body";
            modelParameter.ParameterValue = "XML";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("样式文件内容");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 16;
            listParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion

            #region GetFeatureInfo

            #endregion
        }

        void GetWMTSInfo(string mapId, MapEntity query, LayerGroup queryGroup, List<ServerInterfaceOutputDto> list)
        {
            ServerInterfaceOutputDto modelServer = null;
            List<ParameterOutputDto> listParameter = null;
            ParameterOutputDto modelParameter = null;

            #region GetTile
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetTile";
            modelServer.ServerDesc = UtilityMessageConvert.Get("返回切片数据");
            modelServer.RequestType = "GET";
            modelServer.ServerPath = string.Format("http://{0}:{1}/geoserver/gwc/service/{2}", _GeoServerIp, _GeoServerPort, "wmts");
            modelServer.ServerSort = 1;
            modelServer.ServerKeyWord = "WMTS";

            listParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "service";
            modelParameter.ParameterValue = "WMTS";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("服务类型");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "?service=WMTS";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "version";
            modelParameter.ParameterValue = "1.0.0, 1.1.0, 1.1.1, 1.3.0, 2.0.0";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("版本号");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 2;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&version=1.1.0";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "request";
            modelParameter.ParameterValue = "GetTile";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("请求类型");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 3;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&request=GetTile";

            if (query != null && !string.IsNullOrEmpty(query.MapEnName))
            {
                modelParameter = new ParameterOutputDto();
                modelParameter.ParameterName = "layer";
                modelParameter.ParameterValue = string.IsNullOrEmpty(_GeoWorkSpace) ? query.MapEnName : (_GeoWorkSpace + ":" + query.MapEnName);
                modelParameter.ParameterDesc = UtilityMessageConvert.Get("地图名称");
                modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                modelParameter.ParameterSort = 4;
                listParameter.Add(modelParameter);
                modelServer.ServerPath += "&layer=" + modelParameter.ParameterValue;

                if (queryGroup != null)
                {
                    //行列计算
                    var tile4326MinCol = GetColFromLongitude(double.Parse(queryGroup.BoundSet.Minx), 360 / (2 * Math.Pow(2, 0)));
                    var tile4326MinRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Miny), 360 / (1 * Math.Pow(2, 0)));
                    var tile4326MaxCol = GetColFromLongitude(double.Parse(queryGroup.BoundSet.Maxx), 360 / (2 * Math.Pow(2, 21)));
                    var tile4326MaxRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Maxy), 360 / (1 * Math.Pow(2, 21)));

                    var tile900913MinCol = GetColFromLongitude(double.Parse(queryGroup.BoundSet.Minx), 360 / (1 * Math.Pow(2, 0)));
                    var tile900913MinRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Miny), 360 / (1 * Math.Pow(2, 0)));
                    var tile900913MaxCol = GetColFromLongitude(double.Parse(queryGroup.BoundSet.Maxx), 360 / (1 * Math.Pow(2, 30)));
                    var tile900913MaxRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Maxy), 360 / (1 * Math.Pow(2, 30)));

                    var tileiTelluroMinCol = GetColFromLongitude(double.Parse(queryGroup.BoundSet.Minx), 360 / (1 * Math.Pow(2, 0)));
                    var tileiTelluroMinRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Miny), 360 / (1 * Math.Pow(2, 0)));
                    var tileiTelluroMaxCol = GetColFromLongitude(double.Parse(queryGroup.BoundSet.Maxx), 360 / (1 * Math.Pow(2, 14)));
                    var tileiTelluroMaxRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Maxy), 360 / (1 * Math.Pow(2, 14)));

                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "tileRow";
                    modelParameter.ParameterValue = tile4326MinRow + "-" + tile4326MaxRow + ", " + tile900913MinRow + "-" + tile900913MaxRow + ", " + tileiTelluroMinRow + "-" + tileiTelluroMaxRow;
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("所在行");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 8;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&tileRow=" + GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Miny), 360 / (1 * Math.Pow(2, 7)));

                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "tileCol";
                    modelParameter.ParameterValue = tile4326MinCol + "-" + tile4326MaxCol + ", " + tile900913MinCol + "-" + tile900913MaxCol + ", " + tileiTelluroMinCol + "-" + tileiTelluroMaxCol;
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("所在列");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 9;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&tileCol=" + GetColFromLongitude(double.Parse(queryGroup.BoundSet.Minx), 360 / (2 * Math.Pow(2, 7)));
                }
            }

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "tileMatrixSet";
            modelParameter.ParameterValue = "EPSG:4326, EPSG:900913, iTelluro";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("切片方案");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 5;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&tileMatrixSet=EPSG:4326";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "tileMatrix";
            modelParameter.ParameterValue = "EPSG:4326:0-EPSG:4326:21, EPSG:900913:0-EPSG:900913:30";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("切片等级名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 6;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&tileMatrix=EPSG:4326:7";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "format";
            modelParameter.ParameterValue = "image/jpeg, image/png";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("返回格式");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 7;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&format=image/png";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "style";
            modelParameter.ParameterValue = "";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("样式");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 9;
            listParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion
        }

        void GetiTelluroInfo(string mapId, MapEntity query, LayerGroup queryGroup, List<ServerInterfaceOutputDto> list)
        {
            ServerInterfaceOutputDto modelServer = null;
            List<ParameterOutputDto> listParameter = null;
            ParameterOutputDto modelParameter = null;

            #region GetTile
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetTile";
            modelServer.ServerDesc = UtilityMessageConvert.Get("返回切片数据");
            modelServer.RequestType = "GET";
            modelServer.ServerPath = string.Format("http://{0}/Service/GIS/gis.ashx", _PublishAddress);
            modelServer.ServerSort = 1;
            modelServer.ServerKeyWord = "iTelluro";

            if (query != null && !string.IsNullOrEmpty(query.MapName))
            {
                listParameter = new List<ParameterOutputDto>();
                modelParameter = new ParameterOutputDto();
                modelParameter.ParameterName = "T";
                modelParameter.ParameterValue = query.MapName;
                modelParameter.ParameterDesc = UtilityMessageConvert.Get("地图名称");
                modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                modelParameter.ParameterSort = 1;
                listParameter.Add(modelParameter);
                modelServer.ServerPath += "?T=" + query.MapName;

                modelParameter = new ParameterOutputDto();
                modelParameter.ParameterName = "L";
                modelParameter.ParameterValue = "0-14";
                modelParameter.ParameterDesc = UtilityMessageConvert.Get("切片等级");
                modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                modelParameter.ParameterSort = 2;
                listParameter.Add(modelParameter);
                modelServer.ServerPath += "&L=7";

                if (queryGroup != null)
                {
                    //行列计算
                    var tileMinCol = GetColFromLongitude(double.Parse(queryGroup.BoundSet.Minx), 360 / (10 * Math.Pow(2, 0)));
                    var tileMinRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Miny), 360 / (5 * Math.Pow(2, 0)));
                    var tileMaxCol = GetColFromLongitude(double.Parse(queryGroup.BoundSet.Maxx), 360 / (10 * Math.Pow(2, 14)));
                    var tileMaxRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Maxy), 360 / (5 * Math.Pow(2, 14)));

                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "X";
                    modelParameter.ParameterValue = tileMinCol + "-" + tileMaxCol;
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("所在列");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 3;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&X=" + GetColFromLongitude(double.Parse(queryGroup.BoundSet.Minx), 360 / (10 * Math.Pow(2, 7)));

                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "Y";
                    modelParameter.ParameterValue = tileMinRow + "-" + tileMaxRow;
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("所在行");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 4;
                    listParameter.Add(modelParameter);
                    string tileRow = GetRowFromLatitude(double.Parse(queryGroup.BoundSet.Miny), 360 / (5 * Math.Pow(2, 7))).ToString();
                    int a1 = 180;
                    double b1 = 36;
                    double d1 = 0;
                    d1 = b1 / Math.Pow(2, 7);
                    int totalRow = Convert.ToInt32(a1 / d1) - 1;
                    tileRow = (totalRow - Convert.ToInt32(tileRow)).ToString();
                    modelServer.ServerPath += "&Y=" + tileRow;
                }
            }

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion
        }

        void GetWFSInfo(string mapId, MapEntity queryMap, LayerGroup queryGroup, List<string> query, List<ServerInterfaceOutputDto> list)
        {
            ServerInterfaceOutputDto modelServer = null;
            List<ParameterOutputDto> listParameter = null;
            ParameterOutputDto modelParameter = null;

            #region GetCapabilities

            #endregion

            #region DescribeFeatureType

            #endregion

            #region GetFeature
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetFeature";
            modelServer.ServerDesc = UtilityMessageConvert.Get("返回图层要素");
            modelServer.RequestType = "GET";
            modelServer.ServerPath = string.Format("http://{0}:{1}/geoserver[WORKSPACE]/{2}", _GeoServerIp, _GeoServerPort, "wfs");
            modelServer.ServerPath = modelServer.ServerPath.Replace("[WORKSPACE]", string.IsNullOrEmpty(_GeoWorkSpace) ? "" : ("/" + _GeoWorkSpace));
            modelServer.ServerSort = 1;
            modelServer.ServerKeyWord = "WFS";

            listParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "service";
            modelParameter.ParameterValue = "WFS";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("服务类型");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "?service=WFS";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "version";
            modelParameter.ParameterValue = "1.0.0, 1.1.0, 1.1.1, 1.3.0, 2.0.0";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("版本号");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 2;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&version=1.1.0";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "request";
            modelParameter.ParameterValue = "GetFeature";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("请求类型");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 3;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&request=GetFeature";

            //var query = from a in _IMapReleationRepository.GetAll()
            //            join b in _ILayerContentRepository.GetAll()
            //            on a.DataConfigID equals b.Id
            //            where a.MapID == mapId
            //            select new
            //            {
            //                b.LayerAttrTable
            //            };
            //StringBuilder strLayerNames = new StringBuilder();
            //if (query != null)
            //{
            //    foreach (var item in query)
            //    {
            //        strLayerNames.Append(string.IsNullOrEmpty(_GeoWorkSpace) ? item.LayerAttrTable : (_GeoWorkSpace + ":" + item.LayerAttrTable));
            //        strLayerNames.Append(", ");
            //    }
            //    if (strLayerNames.Length > 0)
            //    {
            //        strLayerNames.Length = strLayerNames.Length - 2;
            //    }
            //}

            string strLayerNames = string.Empty;
            if (query != null)
            {
                if (!string.IsNullOrWhiteSpace(_GeoWorkSpace))
                {
                    query = query.Select(s => _GeoWorkSpace + ":" + s).ToList();
                }
                strLayerNames = string.Join(",", query);
            }

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "typeNames";
            modelParameter.ParameterValue = strLayerNames.ToString();
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 4;
            listParameter.Add(modelParameter);
            if (query != null)
            {
                modelServer.ServerPath += "&typeNames=" + query.FirstOrDefault();
            }

            if (queryMap != null && !string.IsNullOrEmpty(queryMap.MapEnName))
            {
                if (queryGroup != null)
                {
                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "bbox";
                    modelParameter.ParameterValue = string.Format("{0},{1},{2},{3}", queryGroup.BoundSet.Minx, queryGroup.BoundSet.Miny, queryGroup.BoundSet.Maxx, queryGroup.BoundSet.Maxy);
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("边界范围");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 5;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&bbox=" + modelParameter.ParameterValue;

                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "srsName";
                    modelParameter.ParameterValue = queryGroup.BoundSet.Crs.ToString();
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("坐标参考系");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
                    modelParameter.ParameterSort = 6;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&srsName=" + modelParameter.ParameterValue;
                }
            }

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "featureID";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("要素编号");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("要素编号");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 7;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "count";
            modelParameter.ParameterValue = "";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("返回数量") + "(for 2.0.0)";
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 8;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "maxFeatures";
            modelParameter.ParameterValue = "";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("返回最大的数量");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 9;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "sortBy";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("属性名称");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("排序");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 10;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "propertyName";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("属性名称");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("指定返回的属性(多个','隔开)");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 11;
            listParameter.Add(modelParameter);

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "filter";
            modelParameter.ParameterValue = "";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("过滤条件");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 12;
            listParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion

            #region LockFeature

            #endregion

            #region Transaction

            #endregion

            #region 2.0 only

            #region GetPropertyValue

            #endregion

            #region GetFeatureWithLock

            #endregion

            #region CreateStoredQuery

            #endregion

            #region DropStoredQuery

            #endregion

            #region ListStoreQueries

            #endregion

            #region DescribeStoredQueries

            #endregion

            #endregion

            #region 1.0 only
            #region GetGMLObject

            #endregion
            #endregion
        }

        void GetKMLInfo(string mapId, MapEntity query, LayerGroup queryGroup, List<ServerInterfaceOutputDto> list)
        {
            #region GetMap
            List<ParameterOutputDto> listParameter = new List<ParameterOutputDto>();
            ParameterOutputDto modelParameter = null;
            ServerInterfaceOutputDto modelServer = null;
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "KML";
            modelServer.ServerDesc = UtilityMessageConvert.Get("返回KML文件");
            modelServer.RequestType = "GET";
            modelServer.ServerPath = string.Format("http://{0}:{1}/geoserver[WORKSPACE]/{2}", _GeoServerIp, _GeoServerPort, "wms");
            modelServer.ServerPath = modelServer.ServerPath.Replace("[WORKSPACE]", string.IsNullOrEmpty(_GeoWorkSpace) ? "" : ("/" + _GeoWorkSpace));
            modelServer.ServerSort = 1;
            modelServer.ServerKeyWord = "KML";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "service";
            modelParameter.ParameterValue = "WMS";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("服务类型");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "?service=WMS";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "version";
            modelParameter.ParameterValue = "1.0.0, 1.1.0, 1.1.1, 1.3.0, 2.0.0";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("版本号");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 2;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&version=1.1.0";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "request";
            modelParameter.ParameterValue = "GetMap";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("请求类型");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 3;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&request=GetMap";

            if (query != null && !string.IsNullOrEmpty(query.MapEnName))
            {
                modelParameter = new ParameterOutputDto();
                modelParameter.ParameterName = "layers";
                modelParameter.ParameterValue = string.IsNullOrEmpty(_GeoWorkSpace) ? query.MapEnName : (_GeoWorkSpace + ":" + query.MapEnName);
                modelParameter.ParameterDesc = UtilityMessageConvert.Get("地图名称");
                modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                modelParameter.ParameterSort = 4;
                listParameter.Add(modelParameter);
                modelServer.ServerPath += "&layers=" + modelParameter.ParameterValue;

                if (queryGroup != null)
                {
                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "bbox";
                    modelParameter.ParameterValue = string.Format("{0},{1},{2},{3}", queryGroup.BoundSet.Minx, queryGroup.BoundSet.Miny, queryGroup.BoundSet.Maxx, queryGroup.BoundSet.Maxy);
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("边界范围");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 6;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&bbox=" + modelParameter.ParameterValue;

                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = "srs";
                    modelParameter.ParameterValue = queryGroup.BoundSet.Crs.ToString();
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get("坐标参考系");
                    modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
                    modelParameter.ParameterSort = 7;
                    listParameter.Add(modelParameter);
                    modelServer.ServerPath += "&srs=" + modelParameter.ParameterValue;
                }
            }

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "styles";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("空代表默认样式");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("地图样式");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 5;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&styles=";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "width";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("正整数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("宽度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 8;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&width=768";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "height";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("正整数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("高度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 9;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&height=670";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "format";
            modelParameter.ParameterValue = "kml";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("返回格式");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 10;
            listParameter.Add(modelParameter);
            modelServer.ServerPath += "&format=kml";

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion
        }

        /// <summary>
        /// 根据经度计算列号
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        private int GetColFromLongitude(double longitude, double tileSize)
        {
            return (int)Math.Floor((Math.Abs(-180.0 - longitude) % 360) / tileSize);
        }
        /// <summary>
        /// 根据纬度计算行号
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        private int GetRowFromLatitude(double latitude, double tileSize)
        {
            return (int)Math.Floor((Math.Abs(-90.0 - latitude) % 180) / tileSize);
        }
        #endregion

        #region 数据
        /// <summary>
        /// 查询提供的数据接口
        /// </summary>
        /// <param name="mapId">地图编号</param>
        /// <param name="serverType">服务类型</param>
        /// <returns></returns>
        public ListResultOutput<ServerInterfaceOutputDto> GetDataInterface(string mapId, string serverType)
        {
            List<ServerInterfaceOutputDto> list = new List<ServerInterfaceOutputDto>();
            var queryMap = _IMapRepository.FirstOrDefault(m => m.Id == mapId);
            if (!string.IsNullOrEmpty(mapId) && !string.IsNullOrEmpty(serverType) && queryMap != null)
            {
                switch (serverType.ToUpper())
                {
                    case "MAPAPI":
                        GetMapInfo(mapId, queryMap.MapName, list);
                        break;

                    case "LAYERAPI":
                        GetLayerInfo(mapId, list);
                        break;

                    case "SPACEAPI":
                        GetSpaceInfo(mapId, list);
                        break;

                    default:
                        break;
                }
            }
            var result = new ListResultOutput<ServerInterfaceOutputDto>(list.MapTo<List<ServerInterfaceOutputDto>>());
            return result;
        }

        void GetMapInfo(string mapId, string mapName, List<ServerInterfaceOutputDto> list)
        {
            ServerInterfaceOutputDto modelServer = null;
            List<ParameterOutputDto> listRequestParameter = null;
            List<ParameterOutputDto> listResponseParameter = null;
            ParameterOutputDto modelParameter = null;

            #region GetLayersByMapId
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetLayersByMapName";
            modelServer.RequestType = "GET";
            modelServer.ServerDesc = UtilityMessageConvert.Get("根据地图名称查询图层列表");
            modelServer.ServerPath = string.Format("http://{0}/{1}", _PublishAddress, "api/services/app/serverInterface/GetLayersByMapName");
            modelServer.ServerSort = 1;
            modelServer.ServerKeyWord = "MapAPI";
            //请求参数
            listRequestParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "mapName";
            modelParameter.ParameterValue = mapName;
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("地图名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "?mapName=" + mapName;
            //返回参数
            listResponseParameter = new List<ParameterOutputDto>();
            Type type = typeof(LayerOutputDto);
            PropertyInfo[] propertyList = type.GetProperties();
            foreach (PropertyInfo item in propertyList)
            {
                ColumnAttribute columnAttr = Attribute.GetCustomAttribute(item, typeof(ColumnAttribute)) as ColumnAttribute;
                if (columnAttr != null)
                {
                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = columnAttr.ColumnName;
                    modelParameter.ParameterType = UtilityMessageConvert.Get(columnAttr.ColumnType);
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get(columnAttr.ColumnAlias);
                    listResponseParameter.Add(modelParameter);
                }
            }

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listRequestParameter.MapTo<List<ParameterOutputDto>>());
            modelServer.ServerResponseParameters = new ListResultOutput<ParameterOutputDto>(listResponseParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion
        }

        void GetLayerInfo(string mapId, List<ServerInterfaceOutputDto> list)
        {
            ServerInterfaceOutputDto modelServer = null;
            List<ParameterOutputDto> listRequestParameter = null;
            List<ParameterOutputDto> listResponseParameter = null;
            ParameterOutputDto modelParameter = null;

            #region GetLayerFieldNameList
            listRequestParameter = new List<ParameterOutputDto>();
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetLayerFieldNameList";
            modelServer.RequestType = "GET";
            modelServer.ServerDesc = UtilityMessageConvert.Get("根据图层名称查询图层所有字段");
            modelServer.ServerPath = string.Format("http://{0}/{1}", _PublishAddress, "api/services/app/serverInterface/GetLayerFieldNameList");
            modelServer.ServerSort = 1;
            modelServer.ServerKeyWord = "LayerAPI";
            //请求参数
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "layerName";
            modelParameter.ParameterValue = GetLayerNameList(mapId);
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "?layerName=" + modelParameter.ParameterValue.Split(',')[0];
            //返回参数
            //listResponseParameter = new List<ParameterOutputDto>();
            //modelParameter = new ParameterOutputDto();
            //modelParameter.ParameterName = "FieldName";
            //modelParameter.ParameterType = "字符串";
            //modelParameter.ParameterDesc = "字段名称";
            //listResponseParameter.Add(modelParameter);

            listResponseParameter = new List<ParameterOutputDto>();
            Type type = typeof(FieldOutputDto);
            PropertyInfo[] propertyList = type.GetProperties();
            foreach (PropertyInfo item in propertyList)
            {
                ColumnAttribute columnAttr = Attribute.GetCustomAttribute(item, typeof(ColumnAttribute)) as ColumnAttribute;
                if (columnAttr != null)
                {
                    modelParameter = new ParameterOutputDto();
                    modelParameter.ParameterName = columnAttr.ColumnName;
                    modelParameter.ParameterType = UtilityMessageConvert.Get(columnAttr.ColumnType);
                    modelParameter.ParameterDesc = UtilityMessageConvert.Get(columnAttr.ColumnAlias);
                    listResponseParameter.Add(modelParameter);
                }
            }

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listRequestParameter.MapTo<List<ParameterOutputDto>>());
            modelServer.ServerResponseParameters = new ListResultOutput<ParameterOutputDto>(listResponseParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion

            #region GetAttrByLayerName
            listRequestParameter = new List<ParameterOutputDto>();
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetAttrByLayerName";
            modelServer.RequestType = "GET";
            modelServer.ServerDesc = UtilityMessageConvert.Get("根据图层名称查询属性信息");
            modelServer.ServerPath = string.Format("http://{0}/{1}", _PublishAddress, "api/services/app/serverInterface/GetAttrByLayerName");
            modelServer.ServerSort = 2;
            modelServer.ServerKeyWord = "LayerAPI";
            //请求参数
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "layerName";
            modelParameter.ParameterValue = GetLayerNameList(mapId);
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "?layerName=" + modelParameter.ParameterValue.Split(',')[0];
            //返回参数
            listResponseParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "data";
            modelParameter.ParameterType = UtilityMessageConvert.Get("字符串");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层属性信息");
            listResponseParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listRequestParameter.MapTo<List<ParameterOutputDto>>());
            modelServer.ServerResponseParameters = new ListResultOutput<ParameterOutputDto>(listResponseParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion

            #region GetAttrByCondition
            listRequestParameter = new List<ParameterOutputDto>();
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetAttrByCondition";
            modelServer.RequestType = "GET";
            modelServer.ServerDesc = UtilityMessageConvert.Get("根据图层名称和条件查询属性信息");
            modelServer.ServerPath = string.Format("http://{0}/{1}", _PublishAddress, "api/services/app/serverInterface/GetAttrByCondition");
            modelServer.ServerSort = 3;
            modelServer.ServerKeyWord = "LayerAPI";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "layerName";
            modelParameter.ParameterValue = GetLayerNameList(mapId);
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "?layerName=" + modelParameter.ParameterValue.Split(',')[0];

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "condition";
            modelParameter.ParameterValue = "";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("条件");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("否");
            modelParameter.ParameterSort = 2;
            listRequestParameter.Add(modelParameter);

            //返回参数
            listResponseParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "data";
            modelParameter.ParameterType = UtilityMessageConvert.Get("字符串");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层属性信息");
            listResponseParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listRequestParameter.MapTo<List<ParameterOutputDto>>());
            modelServer.ServerResponseParameters = new ListResultOutput<ParameterOutputDto>(listResponseParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion
        }

        void GetSpaceInfo(string mapId, List<ServerInterfaceOutputDto> list)
        {
            ServerInterfaceOutputDto modelServer = null;
            List<ParameterOutputDto> listRequestParameter = null;
            List<ParameterOutputDto> listResponseParameter = null;
            ParameterOutputDto modelParameter = null;

            #region GetAttrByPt
            listRequestParameter = new List<ParameterOutputDto>();
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetAttrByPt";
            modelServer.RequestType = "GET";
            modelServer.ServerDesc = UtilityMessageConvert.Get("根据点查询图层属性信息");
            modelServer.ServerPath = string.Format("http://{0}/{1}", _PublishAddress, "api/services/app/serverInterface/GetAttrByPt");
            modelServer.ServerSort = 1;
            modelServer.ServerKeyWord = "SpaceAPI";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "layerName";
            modelParameter.ParameterValue = GetLayerNameList(mapId);
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "?layerName=" + modelParameter.ParameterValue.Split(',')[0];

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "lon";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("经度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 2;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&lon=110";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "lat";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("纬度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 3;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&lat=80";
            //返回参数
            listResponseParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "data";
            modelParameter.ParameterType = UtilityMessageConvert.Get("字符串");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层属性信息");
            listResponseParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listRequestParameter.MapTo<List<ParameterOutputDto>>());
            modelServer.ServerResponseParameters = new ListResultOutput<ParameterOutputDto>(listResponseParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion

            #region GetAttrByPtTolerane
            listRequestParameter = new List<ParameterOutputDto>();
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetAttrByPtTolerane";
            modelServer.RequestType = "GET";
            modelServer.ServerDesc = UtilityMessageConvert.Get("根据中心点查询图层属性信息");
            modelServer.ServerPath = string.Format("http://{0}/{1}", _PublishAddress, "api/services/app/serverInterface/GetAttrByPtTolerane");
            modelServer.ServerSort = 2;
            modelServer.ServerKeyWord = "SpaceAPI";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "layerName";
            modelParameter.ParameterValue = GetLayerNameList(mapId);
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "?layerName=" + modelParameter.ParameterValue.Split(',')[0];

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "lon";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("经度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 2;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&lon=110";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "lat";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("纬度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 3;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&lat=80";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "distance";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("半径");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 4;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&distance=10";
            //返回参数
            listResponseParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "data";
            modelParameter.ParameterType = UtilityMessageConvert.Get("字符串");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层属性信息");
            listResponseParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listRequestParameter.MapTo<List<ParameterOutputDto>>());
            modelServer.ServerResponseParameters = new ListResultOutput<ParameterOutputDto>(listResponseParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion

            #region GetAttrByRect
            listRequestParameter = new List<ParameterOutputDto>();
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetAttrByRect";
            modelServer.RequestType = "GET";
            modelServer.ServerDesc = UtilityMessageConvert.Get("根据矩形查询图层属性信息");
            modelServer.ServerPath = string.Format("http://{0}/{1}", _PublishAddress, "api/services/app/serverInterface/GetAttrByRect");
            modelServer.ServerSort = 3;
            modelServer.ServerKeyWord = "SpaceAPI";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "layerName";
            modelParameter.ParameterValue = GetLayerNameList(mapId);
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "?layerName=" + modelParameter.ParameterValue.Split(',')[0];

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "minLon";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("最小经度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 2;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&minLon=110";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "minLat";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("最小纬度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 3;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&minLat=70";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "maxLon";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("最大经度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 4;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&maxLon=130";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "maxLat";
            modelParameter.ParameterValue = UtilityMessageConvert.Get("双精度浮点数");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("最大纬度");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 5;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&maxLat=90";
            //返回参数
            listResponseParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "data";
            modelParameter.ParameterType = UtilityMessageConvert.Get("字符串");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层属性信息");
            listResponseParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listRequestParameter.MapTo<List<ParameterOutputDto>>());
            modelServer.ServerResponseParameters = new ListResultOutput<ParameterOutputDto>(listResponseParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);
            #endregion

            #region [GetAttrByNGon]

            listRequestParameter = new List<ParameterOutputDto>();
            modelServer = new ServerInterfaceOutputDto();
            modelServer.ServerName = "GetAttrByNGon";
            modelServer.RequestType = "GET";
            modelServer.ServerDesc = UtilityMessageConvert.Get("根据多边形框选查询图层属性信息");
            modelServer.ServerPath = string.Format("http://{0}/{1}", _PublishAddress, "api/services/app/serverInterface/GetAttrByNGon");
            modelServer.ServerSort = 1;
            modelServer.ServerKeyWord = "SpaceAPI";

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "layerName";
            modelParameter.ParameterValue = GetLayerNameList(mapId);
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层名称");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 1;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "?layerName=" + modelParameter.ParameterValue.Split(',')[0];

            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "wkt";
            modelParameter.ParameterValue = "POLYGON((lon1 lat1,lon2 lat2))";
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("多边形框选wkt字符串");
            modelParameter.ParameterIsMust = UtilityMessageConvert.Get("是");
            modelParameter.ParameterSort = 2;
            listRequestParameter.Add(modelParameter);
            modelServer.ServerPath += "&wkt=POLYGON((99.0000914694002 33.0001609295997,99.0000918162185 33.3335035352914))";

            //返回参数
            listResponseParameter = new List<ParameterOutputDto>();
            modelParameter = new ParameterOutputDto();
            modelParameter.ParameterName = "data";
            modelParameter.ParameterType = UtilityMessageConvert.Get("字符串");
            modelParameter.ParameterDesc = UtilityMessageConvert.Get("图层属性信息");
            listResponseParameter.Add(modelParameter);

            modelServer.ServerRequestParameters = new ListResultOutput<ParameterOutputDto>(listRequestParameter.MapTo<List<ParameterOutputDto>>());
            modelServer.ServerResponseParameters = new ListResultOutput<ParameterOutputDto>(listResponseParameter.MapTo<List<ParameterOutputDto>>());
            list.Add(modelServer);

            #endregion
        }

        string GetLayerNameList(string mapId)
        {
            var query = from a in _ILayerContentRepository.GetAll()
                        join b in _IMapReleationRepository.GetAll()
                        on a.Id equals b.DataConfigID
                        where b.MapID == mapId
                        select new
                        {
                            a.LayerName
                        };
            StringBuilder strLayers = new StringBuilder();
            if (query != null)
            {
                foreach (var item in query)
                {
                    strLayers.Append(item.LayerName);
                    strLayers.Append(", ");
                }
                if (strLayers.Length > 0)
                {
                    strLayers.Length = strLayers.Length - 2;
                }
            }
            return strLayers.ToString();
        }
        #endregion

        #region [地下水服务接口]

        /// <summary>
        /// 查询MapGIS转换ArcGIS文件下载
        /// </summary>
        /// <param name="fileName">mapgis压缩包文件名</param>
        /// <returns>返回文件压缩包下载路径</returns>
        public string GetMapGISToArcGIS(string fileName)
        {
            #region [屏弊]
            /*
            string outputFilePath = String.Empty;
            bool success = false;

            try
            {
                #region [路径处理]

                string fileFolder = fileName.Substring(0, fileName.IndexOf("."));
                string mapGISUnZipPath = Path.Combine(ConfigurationManager.AppSettings["UploadFilePath"].ToString(), "MapGIS");
                string mapGISFilePath = Path.Combine(mapGISUnZipPath, fileName);
                string mapGISFolderPath = Path.Combine(mapGISUnZipPath, fileFolder);
                string mapGISConvertFilePath = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, ConfigurationManager.AppSettings["DownloadFile"], fileFolder);
                string arcGISZipPath = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, ConfigurationManager.AppSettings["DownloadFile"], fileName);

                #endregion

                #region [解压文件]

                ZipHelper zip = new ZipHelper();

                //如果存在文件夹就先清理掉
                if (Directory.Exists(mapGISFolderPath))
                {
                    Directory.Delete(mapGISFolderPath, true);
                }
                success = zip.UnZip(mapGISFilePath, mapGISFolderPath);

                #endregion

                #region [转换文件]

                string[] fileNames = null;
                if (Directory.Exists(mapGISFolderPath) && success)
                {
                    GISFileConvertHelper fileConvert = new GISFileConvertHelper();
                    DirectoryInfo folder = new DirectoryInfo(mapGISFolderPath);
                    if (folder.GetFiles().Count() > 0)
                    {
                        foreach (FileInfo file in folder.GetFiles())
                        {
                            fileNames = fileConvert.MapgisToArcgis(file.FullName, mapGISConvertFilePath);
                        }
                    }
                }

                #endregion

                #region [加压输出]

                if (fileNames.Length > 0)
                {
                    //如果存在文件就先清理掉再压缩
                    if (File.Exists(arcGISZipPath))
                    {
                        File.Delete(arcGISZipPath);
                    }

                    success = zip.ZipDir(mapGISConvertFilePath, arcGISZipPath, 9);

                    if (success)
                    {
                        outputFilePath = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/" + ConfigurationManager.AppSettings["DownloadFile"].ToString() + "/" + fileName;
                    }
                }

                #endregion

                return outputFilePath;
            }
            catch (Exception ex)
            {
                return outputFilePath;
            }

            */
            #endregion

            return GisFileOperate(@fileName, "MapGIS", true, false);
        }

        /// <summary>
        /// ArcGIS文件入库
        /// </summary>
        /// <param name="fileName">arcgis压缩包文件名</param>
        /// <returns>返回成功提示</returns>
        public string GetArcGISToDB(string fileName)
        {
            #region [屏弊]
            /*
            string result = string.Empty;
            bool success = false;
            string Message = string.Empty;

            #region [路径处理]

            string gisFileType = "ArcGIS";
            string fileFolder = fileName.Substring(0, fileName.IndexOf("."));
            string arcGISUnZipPath = Path.Combine(ConfigurationManager.AppSettings["UploadFilePath"].ToString(), gisFileType);
            string arcGISFilePath = Path.Combine(arcGISUnZipPath, fileName);
            string arcGISFolderPath = Path.Combine(arcGISUnZipPath, fileFolder);
            string arcGISFolder = Path.Combine(gisFileType, fileFolder);

            #endregion

            #region [解压文件]

            ZipHelper zip = new ZipHelper();

            //如果存在文件夹就先清理掉
            if (Directory.Exists(arcGISFolderPath))
            {
                Directory.Delete(arcGISFolderPath, true);
            }
            success = zip.UnZip(arcGISFilePath, arcGISFolderPath);

            #endregion

            #region [解析文件]

            if (Directory.Exists(arcGISFolderPath))
            {
                var dicData = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "b9ef9c7c-67b3-11e7-8eb2-005056bb1c7e");
                string fileType = string.Empty;
                foreach (var item in dicData)
                {
                    fileType += "." + item.CodeValue + ";";
                }

                DirectoryInfo folder = new DirectoryInfo(arcGISFolderPath);
                string firstFileName = string.Empty;
                result += "[";
                foreach (FileInfo file in folder.GetFiles())
                {
                    string tempFileName = file.Name.Substring(0, file.Name.LastIndexOf("."));
                    if (!firstFileName.Equals(tempFileName) && !file.Extension.Contains(".log"))
                    {
                        result += "{";
                        string layerID = string.Empty, layerName = string.Empty;
                        bool status = true;
                        firstFileName = tempFileName;
                        success = IsFileExist(firstFileName, arcGISFolderPath, fileType);

                        if (!success)
                        {
                            status = false;
                            Message += firstFileName + ":" + "文件不完整;";
                        }
                        else
                        {
                            string shpFileName = Path.Combine(arcGISFolderPath, firstFileName + ".shp");

                            var listDataType = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "73160096-67a5-11e7-8eb2-005056bb1c7e").ToList();

                            var listGeomType = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "1cfe51dd-67a3-11e7-8eb2-005056bb1c7e").ToList();

                            #region [入库操作]

                            string message = GetShpFileInfo(shpFileName, firstFileName, listDataType, listGeomType, arcGISFolder, ref layerID, ref layerName);

                            #endregion

                            if (!string.IsNullOrEmpty(message))
                            {
                                status = false;
                                Message += firstFileName + ":" + message;
                            }
                            else
                            {
                                status = true;
                                Message += firstFileName + ":" + "文件已上传成功，文件解析后续交由后台异步处理，请在系统通知中确认;";
                            }
                        }
                        result += "\"" + "layerID" + "\":" + "\"" + layerID + "\"," + "\"" + "layerName" + "\":" + "\"" + layerName + "\"," + "\"" + "status" + "\":" + "\"" + status + "\"," + "\"" + "message" + "\":" + "\"" + Message + "\"";
                        result += "},";
                    }
                    else
                    {
                        continue;
                    }
                }
                result = result.TrimEnd(',');
                result += "]";
            }

            #endregion

            return result;
            */

            #endregion

            return GisFileOperate(@fileName, "ArcGIS", false, false);
        }

        /// <summary>
        /// MapGIS文件入库
        /// </summary>
        /// <param name="fileName">mapgis压缩包文件名</param>
        /// <returns></returns>
        public string GetMapGISToDB(string fileName)
        {
            #region [屏弊]
            /*
            string result = string.Empty;
            bool success = false;
            string Message = string.Empty;

            #region [路径处理]

            string gisFileType = "MapGIS";
            string fileFolder = fileName.Substring(0, fileName.IndexOf("."));
            string mapGISUnZipPath = Path.Combine(ConfigurationManager.AppSettings["UploadFilePath"].ToString(), gisFileType);
            string mapGISConvertFilePath = Path.Combine(ConfigurationManager.AppSettings["UploadFilePath"].ToString(), "ArcGIS", fileFolder);
            string mapGISFilePath = Path.Combine(mapGISUnZipPath, fileName);
            string mapGISFolderPath = Path.Combine(mapGISUnZipPath, fileFolder);
            string mapGISFolder = Path.Combine("ArcGIS", fileFolder);

            #endregion

            #region [解压文件]

            ZipHelper zip = new ZipHelper();

            //如果存在文件夹就先清理掉
            if (Directory.Exists(mapGISFolderPath))
            {
                Directory.Delete(mapGISFolderPath, true);
            }
            success = zip.UnZip(mapGISFilePath, mapGISFolderPath);

            #endregion

            #region [转换文件]

            string[] fileNames = null;
            if (Directory.Exists(mapGISFolderPath) && success)
            {
                //先清理已有文件夹
                if (Directory.Exists(mapGISConvertFilePath))
                {
                    Directory.Delete(mapGISConvertFilePath, true);
                }

                GISFileConvertHelper fileConvert = new GISFileConvertHelper();
                DirectoryInfo folder = new DirectoryInfo(mapGISFolderPath);
                if (folder.GetFiles().Count() > 0)
                {
                    foreach (FileInfo file in folder.GetFiles())
                    {
                        fileNames = fileConvert.MapgisToArcgis(file.FullName, mapGISConvertFilePath);
                    }
                }
            }

            #endregion

            #region [解析文件]

            if (Directory.Exists(mapGISConvertFilePath))
            {
                var dicData = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "b9ef9c7c-67b3-11e7-8eb2-005056bb1c7e");
                string fileType = string.Empty;
                foreach (var item in dicData)
                {
                    fileType += "." + item.CodeValue + ";";
                }

                DirectoryInfo folder = new DirectoryInfo(mapGISConvertFilePath);
                string firstFileName = string.Empty;
                result += "[";
                foreach (FileInfo file in folder.GetFiles())
                {
                    string tempFileName = file.Name.Substring(0, file.Name.LastIndexOf("."));
                    if (!firstFileName.Equals(tempFileName) && !file.Extension.Contains(".log"))
                    {
                        result += "{";
                        string layerID = string.Empty,layerName = string.Empty;
                        bool status = true;
                        firstFileName = tempFileName;
                        success = IsFileExist(firstFileName, mapGISConvertFilePath, fileType);

                        if (!success)
                        {
                            status = false;
                            Message += firstFileName + ":" + "文件不完整;";
                        }
                        else
                        {
                            string shpFileName = Path.Combine(mapGISConvertFilePath, firstFileName + ".shp");

                            var listDataType = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "73160096-67a5-11e7-8eb2-005056bb1c7e").ToList();

                            var listGeomType = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "1cfe51dd-67a3-11e7-8eb2-005056bb1c7e").ToList();

                            #region [入库操作]

                            string message = GetShpFileInfo(shpFileName, firstFileName, listDataType, listGeomType, mapGISFolder,ref layerID,ref layerName);

                            #endregion

                            if (!string.IsNullOrEmpty(message))
                            {
                                status = false;
                                Message += firstFileName + ":" + message;
                            }
                            else
                            {
                                status = true;
                                Message += firstFileName + ":" + "文件已上传成功，文件解析后续交由后台异步处理，请在系统通知中确认;";
                            }
                        }

                        result += "\"" + "layerID" + "\":" + "\"" + layerID + "\"," + "\"" + "layerName" + "\":" + "\"" + layerName + "\"," + "\"" + "status" + "\":" + "\"" + status + "\"," + "\"" + "message" + "\":" + "\"" + Message + "\"";
                        result += "},";
                    }
                    else
                    {
                        continue;
                    }
                }
                result = result.TrimEnd(',');
                result += "]";
            }

            #endregion

            return result;

            */
            #endregion

            return GisFileOperate(@fileName, "MapGIS", false, false);
        }

        #region [功能拆分]

        /// <summary>
        /// GIS文件操作(转换，入库)
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="gisFileType">文件类型</param>
        /// <param name="IsDownZip">是否需要下载</param>
        /// <param name="IsFilePath">是否文件路径</param>
        /// <returns></returns>
        public string GisFileOperate(string fileName, string gisFileType, bool IsDownZip, bool IsFilePath)
        {
            UploadResult ur = new UploadResult();
            LayerContentEntity entity = new LayerContentEntity();
            #region [初始化]

            string result = string.Empty, message = string.Empty, filePath = string.Empty;
            bool success = false;
            string[] fileNames = null;

            #endregion

            try
            {
                #region [路径处理]

                if (IsFilePath)
                {
                    filePath = fileName;
                    fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                }

                string fileFolder = fileName.Substring(0, fileName.IndexOf("."));
                string gisUnZipPath = Path.Combine(ConfigurationManager.AppSettings["UploadFilePath"].ToString(), gisFileType);
                string gisFolderPath = Path.Combine(gisUnZipPath, fileFolder);
                string gisFilePath = (IsFilePath) ? filePath : Path.Combine(gisUnZipPath, fileName);
                string gisConvertFilePath = (IsDownZip) ? Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, ConfigurationManager.AppSettings["DownloadFile"], fileFolder)
                                            : Path.Combine(ConfigurationManager.AppSettings["UploadFilePath"].ToString(), "ArcGIS", fileFolder);
                string gisZipPath = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, ConfigurationManager.AppSettings["DownloadFile"], fileName);
                string gisFolder = Path.Combine("ArcGIS", fileFolder);

                #endregion

                #region [解压文件]

                ZipHelper zip = new ZipHelper();

                //如果存在文件夹就先清理掉
                try
                {
                    if (Directory.Exists(gisFolderPath))
                    {
                        Directory.Delete(gisFolderPath, true);
                    }
                }
                catch { 
                
                }
                success = zip.UnZip(gisFilePath, gisFolderPath);

                #endregion
            
                #region [转换文件]

                if (Directory.Exists(gisFolderPath) && success && gisFileType == "MapGIS")
                {
                    //先清理已有文件夹
                    try
                    {
                        if (Directory.Exists(gisConvertFilePath))
                        {
                            Directory.Delete(gisConvertFilePath, true);
                        }
                    }
                    catch
                    { 
                    
                    }
                    GISFileConvertHelper fileConvert = new GISFileConvertHelper();
                    DirectoryInfo folder = new DirectoryInfo(gisFolderPath);
                    if (folder.GetFiles().Count() > 0)
                    {
                        foreach (FileInfo file in folder.GetFiles())
                        {
                            fileNames = fileConvert.MapgisToArcgis(file.FullName, gisConvertFilePath);
                        }
                    }
                }
         
                #endregion
              

                #region [解析文件]
        
        
               
                if (Directory.Exists(gisConvertFilePath) && !IsDownZip)
               {
                    var dicData = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "b9ef9c7c-67b3-11e7-8eb2-005056bb1c7e");
                    var listDataType = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "73160096-67a5-11e7-8eb2-005056bb1c7e").ToList();
                    var listGeomType = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "1cfe51dd-67a3-11e7-8eb2-005056bb1c7e").ToList();
               
                    string fileType = string.Empty;
                    foreach (var item in dicData)
                    {
                        fileType += "." + item.CodeValue + ";";
                    }

            

                    DirectoryInfo folder1 = new DirectoryInfo(gisConvertFilePath);
                    string firstFileName = string.Empty;


                    foreach (FileInfo file in folder1.GetFiles())
                    {
                    
                        string tempFileName = file.Name.Substring(0, file.Name.LastIndexOf("."));

                 
                        if (!firstFileName.Equals(tempFileName) && !file.Extension.Contains(".log"))
                        {
                            message = string.Empty;
                            string layerID = string.Empty, layerName = string.Empty;
                            bool status = true;
                            firstFileName = tempFileName;
                            success = IsFileExist(firstFileName, gisConvertFilePath, fileType);

                            if (!success)
                            {
                     
                                status = false;
                                message = firstFileName + ":" + "文件不完整;";

                                //ur.Success = status;
                                //ur.Message = message;
                                //ur.LayerContentEntity = entity;
                            }
                            else
                            {
                                string shpFileName = Path.Combine(gisConvertFilePath, firstFileName + ".shp");

                                #region [入库操作]

                                string msg = GetShpFileInfo(shpFileName, firstFileName, listDataType, listGeomType, gisFolder, ref entity);
                                #endregion

                                if (!string.IsNullOrEmpty(msg))
                                {
                                    status = false;
                                    message = firstFileName + ":" + msg;
                                }
                                else
                                {
                                    status = true;
                                    message = firstFileName + ":" + "文件已上传成功，文件解析后续交由后台异步处理，请在系统通知中确认;";
                                }
                                ur.Success = status;
                                ur.Message = msg;
                                ur.LayerContentEntity = entity;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                #endregion

                #region [加压输出]

                if (fileNames != null && fileNames.Length > 0 && IsDownZip)
                {
                    //如果存在文件就先清理掉再压缩
                    if (File.Exists(gisZipPath))
                    {
                        File.Delete(gisZipPath);
                    }

                    success = zip.ZipDir(gisConvertFilePath, gisZipPath, 9);

                    if (success)
                    {
                        result = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/" + ConfigurationManager.AppSettings["DownloadFile"].ToString() + "/" + fileName;
                    }
                    ur.DownUrl = result;
                }

                #endregion                
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            //ur.Message = message;
            //ur.Success = success;
            //ur.LayerContentEntity = entity;
            return JsonConvert.SerializeObject(ur);
        }

        #endregion

        #region [shp操作]

        /// <summary>
        /// 核对文件的完整性
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filePhysicPath"></param>
        /// <returns></returns>
        private bool IsFileExist(string fileName, string filePath, string fileType)
        {
            string[] arrFileType = fileType.TrimEnd(';').Split(';');

            foreach (var item in arrFileType)
            {
                string tmpFileName = fileName + item;
                string tmpFilePath = Path.Combine(filePath, tmpFileName);
                if (!File.Exists(tmpFilePath))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 匹配字典中的几何类型
        /// </summary>
        /// <param name="GeomType"></param>
        /// <param name="listGeomType"></param>
        /// <returns></returns>
        private string GetGeomType(string GeomType, List<InfoEarthFrame.Core.Entities.DicDataCodeEntity> listGeomType)
        {
            string geomTypeID = string.Empty;
            if (!string.IsNullOrEmpty(GeomType))
            {
                foreach (var item in listGeomType)
                {
                    if (GeomType.ToLower().Contains(item.CodeValue.ToLower()))
                    {
                        geomTypeID = item.Id;
                    }
                }
            }

            return geomTypeID;
        }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <param name="listDataType"></param>
        /// <param name="AttributeType"></param>
        /// <param name="AttributeWidth"></param>
        /// <returns></returns>
        private string GetFieldDataType(List<InfoEarthFrame.Core.Entities.DicDataCodeEntity> listDataType, string AttributeType, int AttributeWidth, ref string AttrType)
        {
            string AttributeTypeName = string.Empty;
            switch (AttributeType)
            {
                case "OFTDate":
                    AttributeTypeName = "时间型";
                    break;
                case "OFTDateTime":
                    AttributeTypeName = "时间型";
                    break;
                case "OFTInteger":
                    AttributeTypeName = "短整型";
                    break;
                case "OFTIntegerList":
                    AttributeTypeName = "长整型";
                    break;
                case "OFTReal":
                    if (AttributeWidth <= 13)
                    {
                        AttributeTypeName = "单浮点型";
                    }
                    else
                    {
                        AttributeTypeName = "双浮点型";
                    }
                    break;
                case "OFTString":
                    AttributeTypeName = "字符型";
                    break;
                case "OFTTime":
                    AttributeTypeName = "时间型";
                    break;
                default:
                    AttributeTypeName = "字符型";
                    break;
            }
            Predicate<InfoEarthFrame.Core.Entities.DicDataCodeEntity> dataTypeEntity = delegate (InfoEarthFrame.Core.Entities.DicDataCodeEntity entity)
            {
                return entity.CodeName.Equals(AttributeTypeName);
            };

            InfoEarthFrame.Core.Entities.DicDataCodeEntity dicEntity = listDataType.Find(dataTypeEntity);

            AttrType = dicEntity.CodeValue;

            return dicEntity.Id;
        }

        /// <summary>
        /// mySQL创建表
        /// </summary>
        /// <param name="tableName1"></param>
        /// <param name="tableName2"></param>
        /// <param name="lstAttr"></param>
        /// <returns></returns>
        private string MySqlTableCreateSql(string tableName1, string tableName2, List<AttributeModel> lstAttr)
        {
            string strSql = "";

            #region [图层业务表]

            strSql += "CREATE TABLE `" + tableName1 + "` (";
            strSql += "`sid` varchar(36) NOT NULL, ";
            foreach (var item in lstAttr)
            {
                string arrType = item.AttributeTypeName.ToLower();
                switch (arrType)
                {
                    case "text":
                        strSql += "`" + item.AttributeName + "` varchar(" + item.AttributeWidth + ") " + ",";
                        break;
                    case "long integer":
                        strSql += "`" + item.AttributeName + "` int(11)" + ",";
                        break;
                    case "short integer":
                        strSql += "`" + item.AttributeName + "` int(11)" + ",";
                        break;
                    case "float":
                        strSql += "`" + item.AttributeName + "` " + arrType + "(" + item.AttributeWidth + "," + item.AttributePrecision + ")" + ",";
                        break;
                    case "double":
                        strSql += "`" + item.AttributeName + "` " + arrType + "(" + item.AttributeWidth + "," + item.AttributePrecision + ")" + ",";
                        break;
                    case "date":
                        strSql += "`" + item.AttributeName + "`  datetime" + ",";
                        break;
                    default:
                        strSql += "";
                        break;
                }
            }
            strSql = strSql.TrimEnd(',');
            strSql += ", PRIMARY KEY (`sid`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";

            #endregion

            #region [空间参考表]

            strSql += "CREATE TABLE `" + tableName2 + "` (";
            strSql += "`sid` varchar(36) NOT NULL, ";
            strSql += "`DataID` varchar(36) NOT NULL, ";
            strSql += "`geom` GEOMETRY NOT NULL";

            strSql += ", PRIMARY KEY (`sid`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";

            #endregion

            return strSql;
        }

        /// <summary>
        /// PostGIS创建表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="lstAttr"></param>
        /// <returns></returns>
        public string PostGISTableCreateSql(string tableName, List<AttributeModel> lstAttr)
        {
            string config = ConfigurationManager.AppSettings["DataBase"].ToString();
            int startIndex = config.LastIndexOf(":");
            string dataBaseType = config.Substring(config.LastIndexOf(":") + 1);

            string strSql = string.Format("CREATE TABLE \"{0}\"(sid SERIAL primary key,guid varchar,", tableName);
            for (int i = 0; i < lstAttr.Count; i++)
            {
                string attrType = lstAttr[i].AttributeTypeName.ToLower();
                if (!string.IsNullOrEmpty(attrType))
                {
                    if (attrType == "double" || attrType == "float")
                    {
                        attrType = "double precision";
                    }
                    else if (attrType == "int" || attrType == "long integer" || attrType == "short integer")
                    {
                        attrType = "integer";
                    }
                    else if (attrType == "datetime")
                    {
                        attrType = "date";
                    }
                    else if (attrType == "text" || attrType == "varchar")
                    {
                        attrType = "varchar";
                    }
                    strSql += String.Format("\"{0}\" {1},", lstAttr[i].AttributeName, attrType);
                }
            }

            strSql = strSql.TrimEnd(',');
            strSql += String.Format(",geom geometry)");

            return strSql;
        }

       // [UnitOfWork(isTransactional: false)]
        /// <summary>
        /// 操作shp
        /// </summary>
        /// <param name="shpFilePath"></param>
        /// <param name="fileName"></param>
        /// <param name="listDataType"></param>
        /// <param name="listGeomType"></param>
        /// <returns></returns>
        public string GetShpFileInfo(string shpFilePath, string fileName, List<InfoEarthFrame.Core.Entities.DicDataCodeEntity> listDataType, List<InfoEarthFrame.Core.Entities.DicDataCodeEntity> listGeomType, string folderName, ref InfoEarthFrame.Core.Entities.LayerContentEntity layerEntity)
        {
            string result = string.Empty;
            try
            {
                InfoEarthFrame.Common.ShpUtility.ShpReader shpReader = new InfoEarthFrame.Common.ShpUtility.ShpReader(shpFilePath);
                // 检查矢量文件的有效性
                if (!shpReader.IsValidDataSource())
                {
                    return "上传文件解析异常(上传的矢量文件无效)";
                }
                else
                {
                    string csSrc = shpReader.GetSridWkt();
                    string SpatialRefence = System.Configuration.ConfigurationManager.AppSettings["SpatialRefence"].ToString();

                    if (!csSrc.Contains(SpatialRefence) && !csSrc.Contains(SpatialRefence.Replace("GCS_", "")))
                    {
                        return "上传文件解析异常(与系统默认坐标系不匹配)";
                    }
                    else
                    {
                        string tableName1 = String.Empty;
                        string tableName2 = String.Empty;

                        #region [图层配置]

                        ChineseConvert chn = new ChineseConvert();
                        string name = chn.GetPinyinInitials(fileName);
                        string lastIndex = (System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString());
                        int nameLength = (30 - lastIndex.Length) - 1;
                        tableName1 = "v" + ((name.Length > nameLength) ? name.Substring(0, nameLength) : name) + lastIndex;
                        tableName2 = "v" + ((name.Length > (nameLength - 7)) ? name.Substring(0, (nameLength - 7)) : name) + lastIndex + "_s";

                        string layerID = Guid.NewGuid().ToString();
                        Dictionary<string, double> listvalues = shpReader.GetLayerBBox();
                        layerEntity = new InfoEarthFrame.Core.Entities.LayerContentEntity
                        {
                            Id = layerID,
                            LayerName = fileName,
                            DataType = GetGeomType(shpReader.GetGeomType().ToUpper(), listGeomType),//点线面获取
                            LayerDesc = name,
                            LayerAttrTable = tableName1,
                            LayerSpatialTable = tableName2,
                            LayerRefence = SpatialRefence,
                            CreateBy = "admin",
                            CreateDT = DateTime.Now,
                            MaxX = listvalues.ContainsKey("MaxX") ? Convert.ToDecimal(listvalues["MaxX"]) : Decimal.Zero,
                            MaxY = listvalues.ContainsKey("MaxY") ? Convert.ToDecimal(listvalues["MaxY"]) : Decimal.Zero,
                            MinX = listvalues.ContainsKey("MinX") ? Convert.ToDecimal(listvalues["MinX"]) : Decimal.Zero,
                            MinY = listvalues.ContainsKey("MinY") ? Convert.ToDecimal(listvalues["MinY"]) : Decimal.Zero
                        };

                        var db = (InfoEarthFrameDbContext)_ILayerContentRepository.GetDbContext();

                        var layerE = db.LayerContent.Add(layerEntity);

                        List<AttributeModel> lstAttr = shpReader.GetOneFeatureAttributeModel(0);

                        List<AttributeModel> lstAttModel = new List<AttributeModel>();
                        foreach (var item in lstAttr)
                        {
                            string attrTypeID = string.Empty;
                            string attrType = string.Empty;
                            attrTypeID = GetFieldDataType(listDataType, item.AttributeType.ToString(), item.AttributeWidth, ref attrType);

                            AttributeModel model = new AttributeModel();
                            model.AttributeName = item.AttributeName;
                            model.AttributeTypeName = attrType;
                            model.AttributeWidth = item.AttributeWidth;
                            model.AttributePrecision = (item.AttributePrecision == 11) ? 6 : item.AttributePrecision;

                            InfoEarthFrame.Core.Entities.LayerFieldEntity layerFieldEntity = new InfoEarthFrame.Core.Entities.LayerFieldEntity
                            {
                                Id = Guid.NewGuid().ToString(),
                                LayerID = layerID,
                                AttributeName = item.AttributeName,
                                AttributeType = attrTypeID,
                                AttributeLength = item.AttributeWidth.ToString(),
                                AttributePrecision = (item.AttributePrecision.ToString() == "11") ? "6" : item.AttributePrecision.ToString(),
                                CreateDT = DateTime.Now
                            };
                           // var layerFE = _ILayerFieldRepository.Insert(layerFieldEntity);
                            db.LayerField.Add(layerFieldEntity);
                            lstAttModel.Add(model);
                        }

                        #endregion

                        #region [创建表]

                        bool success = true;

                        //MySqlHelper mysql = new MySqlHelper();
                        PostgrelVectorHelper postgis = new PostgrelVectorHelper();

                        success = postgis.ExceuteSQL(PostGISTableCreateSql(tableName1, lstAttModel), tableName1);
                        if (success)
                        {
                            //success = postgis.ExceuteSQL(PostGISTableCreateSql(tableName2, lstAttModel), tableName2);
                            //success = mysql.ExecuteNonQuery(MySqlTableCreateSql(tableName1, tableName2, lstAttModel));
                        }

                        #endregion

                        #region [处理数据]

                        /*
                        Dictionary<string, string> attr = shpReader.GetAttributeType();
                        Hashtable colAttr = new Hashtable();
                        List<String> shpAttr = new List<string>();

                        foreach (KeyValuePair<string, string> item in attr)
                        {
                            shpAttr.Add(item.Key);
                            colAttr.Add(item.Key, item.Value);
                        }

                        int pFeatureCount = shpReader.GetFeatureCount();

                        string mySQLInsert1 = string.Empty;
                        string mySQLInsert2 = string.Empty;
                        string postGISSQL = string.Empty;

                        int maxNum = 0;
                        int _count = 500;


                        for (int i = 0; i < pFeatureCount; i++)
                        {
                            string colStr = string.Empty;
                            string valueStr = string.Empty;

                            Dictionary<string, string> dicAttrValue = new Dictionary<string, string>();
                            if (pFeatureCount > 0)
                            {
                                dicAttrValue = shpReader.GetOneFeatureAttribute(i, shpAttr);
                            }

                            foreach (KeyValuePair<string, string> item in dicAttrValue)
                            {
                                colStr += "\"" + item.Key + "\"" + ",";
                                if (colAttr[item.Key].ToString() == "OFTString")
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

                            #region [POSTGIS]

                            string sid = string.Format("{0}", maxNum + i);
                            string geomStr = String.Format("'{0}'", shpReader.GetOneFeatureGeomWkt(i));

                            postGISSQL += string.Format("({0},{1},{2}),", sid, geomStr, valueStr);

                            #endregion

                            #region [MYSQL]

                            string mySQLGeomStr = String.Format("GEOMFROMTEXT('{0}')", shpReader.GetOneFeatureGeomWkt(i));
                            string sid1 = Guid.NewGuid().ToString();
                            string sid2 = Guid.NewGuid().ToString();
                            mySQLInsert1 += string.Format("('{0}',{1}),", sid1, valueStr);
                            mySQLInsert2 += string.Format("('{0}','{1}',{2}),", sid2, sid1, mySQLGeomStr);

                            #endregion

                            #region [批量处理]

                            if ((i % _count == 0) || (i == pFeatureCount - 1))
                            {
                                #region [POSTGIS]

                                postGISSQL = postGISSQL.TrimEnd(',');
                                string sqlInsert = String.Format("insert into {0}(sid,geom,{1}) values{2}", tableName1, colStr, postGISSQL);

                                postgis.ExceuteSQL(sqlInsert, string.Empty);
                                postGISSQL = string.Empty;

                                #endregion

                                #region [MYSQL]

                                mySQLInsert1 = mySQLInsert1.TrimEnd(',');
                                mySQLInsert2 = mySQLInsert2.TrimEnd(',');
                                string sqlInsert1 = String.Format("insert into `{0}`(`sid`,{1}) values{2}", tableName1, colStr, mySQLInsert1);
                                string sqlInsert2 = String.Format("insert into `{0}`(`sid`,`DataID`,`geom`) values{1}", tableName2, mySQLInsert2);

                                mysql.ExecuteNonQuery(sqlInsert1);
                                mySQLInsert1 = string.Empty;

                                mysql.ExecuteNonQuery(sqlInsert2);
                                mySQLInsert2 = string.Empty;

                                #endregion
                            }

                            #endregion
                        }
                        */

                        #endregion

                        #region [发布图层]

                        /*
                        Dictionary<string, double> bbox = shpReader.GetLayerBBox();

                        var layer = _ILayerContentRepository.Get(layerID);

                        GeoServerHelper geoServerHelper = new GeoServerHelper();
                        geoServerHelper.PublicLayer(tableName1, fileName, null, "");

                        layer.UploadStatus = "1";
                        if (bbox.Count > 0)
                        {
                            layer.MinX = Math.Min((decimal)bbox["MinX"], layer.MinX == null ? decimal.MaxValue : layer.MinX.Value);
                            layer.MaxX = Math.Max((decimal)bbox["MaxX"], layer.MaxX == null ? decimal.MinValue : layer.MaxX.Value);
                            layer.MinY = Math.Min((decimal)bbox["MinY"], layer.MinY == null ? decimal.MaxValue : layer.MinY.Value);
                            layer.MaxY = Math.Max((decimal)bbox["MaxY"], layer.MaxY == null ? decimal.MinValue : layer.MaxY.Value);
                        }

                        var result = _ILayerContentRepository.Update(layer);

                        string strbbox = "";

                        if (layer.MinX != null)
                        {
                            strbbox = layer.MinX.ToString() + "," + layer.MinY.ToString() + "," + layer.MaxX.ToString() + "," + layer.MaxY.ToString();
                            string bboxStr = string.Format("{0},{1},{2},{3}", layer.MinX, layer.MinY, layer.MaxX, layer.MaxY);
                            geoServerHelper.ModifyLayerBBox(tableName1, fileName, bboxStr);
                        }


                        ThumbnailHelper tbh = new ThumbnailHelper();
                        string imagePath = tbh.CreateThumbnail(layer.LayerAttrTable, "layer", strbbox);

                        */

                        #endregion

                        #region [服务处理解析]

                        if (success)
                        {
                            //string logID = Guid.NewGuid().ToString();
                            //string strSQL = "insert into sdms_layer_readlog(Id,LayerID,ShpFileName,FolderName,ReadStatus,CreateDT,CreateBy) values('" + logID + "','" + layerID + "','" + fileName + ".shp','" + folderName.Replace("\\", "#") + "','0','" + DateTime.Now.ToString() + "','admin');";

                            //mysql.ExecuteNonQuery(strSQL);

                            //postgis.ExceuteSQL(strSQL, string.Empty);
                        }

                        #endregion

                        #region [插入导入文件日志表]

                        string logID = Guid.NewGuid().ToString();
                        string strSQL = "insert into sdms_layer_readlog(Id,LayerID,ShpFileName,FolderName,ReadStatus,CreateDT,CreateBy) values('" + logID + "','" + layerID + "','" + fileName + ".shp','" + folderName.Replace("\\", "#") + "','0','" + DateTime.Now.ToString() + "','admin');";

                        //MySqlHelper mysql = new MySqlHelper();

                        //mysql.ExecuteNonQuery(strSQL);
                        postgis.ExceuteSQL(strSQL, string.Empty);
                        var flag = db.SaveChanges(); ;
                        if (flag > 0)
                        {
                            //调用Hangfire任务调度接口**
                            string HangfireIp = System.Configuration.ConfigurationManager.AppSettings["HangfireIp"];

                            string url = "http://" + HangfireIp + "/api/index/GetInfo";
                            Encoding encoding = Encoding.UTF8;
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            request.Method = "GET";
                            request.Accept = "text/html, application/xhtml+xml, */*";
                            request.ContentType = "application/json";

                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                            {
                                reader.ReadToEnd();
                            }
              
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return "";
        }

        public class UploadResult
        {
            public bool Success = false;
            public Core.Entities.LayerContentEntity LayerContentEntity;
            public string DownUrl = string.Empty;
            public string Message = string.Empty;
        }

        #endregion

        #endregion

        #region [其它特殊接口]

        /// <summary>
        /// ArcGIS文件入库
        /// </summary>
        /// <param name="filePath">arcgis压缩包物理路径</param>
        /// <returns>返回成功提示</returns>
        public string GetArcgisFileToDB(string filePath)
        {
            return GisFileOperate(@filePath, "ArcGIS", false, true);
        }

        #endregion

        /// <summary>
        /// 删除一个图层要素
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public bool DeleteLayerElement(string userCode,string layerId, string elementId)
        {
            try
            {
                var layer = _ILayerContentRepository.FirstOrDefault(m => m.Id == layerId);
                if (layer == null)
                {
                    layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerAttrTable == layerId);
                }
                if (layer != null)
                {
                    layerId = layer.Id;
                    string tableName1 = layer.LayerAttrTable;
                    string tableName2 = layer.LayerSpatialTable;
                    string strSQL = string.Format("delete from {0} where sid='{1}'", tableName1, elementId);
                    string strSQL2 = string.Format("delete from {0} where DataID='{1}'", tableName2, elementId);
                    string strSQL3 = string.Format("delete from {0} where guid='{1}'", tableName1, elementId);

                    PostgrelVectorHelper actal = new PostgrelVectorHelper();
                    bool isDel = actal.ExceuteSQL(strSQL3, tableName1);

                    //MySqlHelper mysql = new MySqlHelper();
                    //mysql.ExecuteNonQuery(strSQL);
                    //mysql.ExecuteNonQuery(strSQL2);

                    //转Postgre数据库
                    //actal.ExceuteSQL(strSQL, tableName1);
                    actal.ExceuteSQL(strSQL2, tableName2);

                    _iOperateLogAppService.WriteOperateLog(layerId, userCode, 1004, 1105, 1201, 1415, null);

                }
                else { return false; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 添加一个图层要素
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="values">列名:值</param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public bool AddLayerElement(string userCode,string layerId, string elementId,string[] values, string geometry)
        {
            try
            {
                return AddElement(userCode, layerId, elementId, values, geometry);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        private bool AddElement(string userCode, string layerId, string elementId, string[] values, string geometry)
        {
            var layer = _ILayerContentRepository.FirstOrDefault(m => m.Id == layerId);
            if (layer == null)
            {
                layer = _ILayerContentRepository.FirstOrDefault(m => m.LayerAttrTable == layerId);
            }
            if (layer != null)
            {
                layerId = layer.Id;
                List<LayerFieldEntity> fields = _ILayerFieldRepository.GetAll().Where(t => t.LayerID == layerId).ToList();
                if (fields != null && fields.Count > 0)
                {
                    string colStr1 = string.Empty;
                    string colStr2 = string.Empty;
                    string valueStr = string.Empty;

                    string sqlCInsert1 = string.Empty;
                    string sqlCInsert2 = string.Empty;
                    string sqlCInsert3 = string.Empty;

                    if (values != null && values.Any()) {
                        foreach (string value in values)
                        {
                            string[] vals = value.Split(new char[] { ';' });
                            if (vals.Length >= 2)
                            {
                                LayerFieldEntity field = fields.FirstOrDefault(t => t.AttributeName == vals[0]);
                                if (field != null)
                                {
                                    colStr1 += "`" + vals[0] + "`" + ",";
                                    colStr2 += "\"" + vals[0] + "\"" + ",";
                                    //字符型
                                    if (field.AttributeType == "afd042e0-67a8-11e7-8eb2-005056bb1c7e")
                                    {
                                        if (vals[1].Contains("'"))
                                        {
                                            string newValue = vals[1].Replace("'", "''");
                                            valueStr += String.Format("'{0}',", newValue);
                                        }
                                        else
                                        {
                                            valueStr += String.Format("'{0}',", vals[1]);
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(vals[1]))
                                        {
                                            valueStr += String.Format("{0},", vals[1]);
                                        }
                                        else
                                        {
                                            valueStr += String.Format("'{0}',", vals[1]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    colStr1 = colStr1.TrimEnd(',');
                    colStr2 = colStr2.TrimEnd(',');
                    valueStr = valueStr.TrimEnd(',');

                    string tableName1 = layer.LayerAttrTable;
                    string tableName2 = layer.LayerSpatialTable;
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();

                    if (!string.IsNullOrEmpty(colStr1))
                    {
                        string sqlStr = String.Format("select Max(SID) from {0}", tableName1);
                        object obj = actal.GetExecuteScalar(sqlStr);

                        int maxValue = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
                        int maxNum = maxValue + 1;

                        string sid1 = elementId;
                        string sid2 = Guid.NewGuid().ToString();
                        string sid3 = string.Format("{0}", maxNum + 1);

                        string geomStr1 = String.Format("GEOMFROMTEXT('{0}')", geometry);
                        string geomStr2 = String.Format("'{0}'", geometry);
                        if (string.IsNullOrEmpty(geometry))
                        {
                            geomStr1 = "''";
                            geomStr2 = "null";
                        }

                        sqlCInsert1 += string.Format("('{0}',{1})", sid1, valueStr);
                        sqlCInsert2 += string.Format("('{0}','{1}',{2})", sid2, sid1, geomStr1);

                        if (!string.IsNullOrEmpty(colStr2))
                        {
                            sqlCInsert3 += string.Format("({0},'{1}',{2},{3})", sid3, sid1, geomStr2, valueStr);

                            string sqlInsert3 = String.Format("insert into {0}(sid,guid,geom,{1}) values{2}", tableName1, colStr2, sqlCInsert3);
                            actal.ExceuteSQL(sqlInsert3, string.Empty);
                        }

                        //MySqlHelper mysql = new MySqlHelper();

                        string sqlInsert1 = String.Format("insert into `{0}`(`sid`,{1}) values{2}", tableName1, colStr1, sqlCInsert1);
                        string sqlInsert2 = String.Format("insert into `{0}`(`sid`,`DataID`,`geom`) values{1}", tableName2, sqlCInsert2);
                        //mysql.ExecuteNonQuery(sqlInsert1);
                        //mysql.ExecuteNonQuery(sqlInsert2);

                        //转Postgre数据库
                        actal.ExceuteSQL(sqlInsert2, string.Empty);

                    }
                    else
                    {
                        //TODO:新增要素成功入库但是图层加载的时候要素的style没有
                        string sqlStr = String.Format("select Max(SID) from {0}", tableName1);
                        object obj = actal.GetExecuteScalar(sqlStr);

                        int maxValue = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
                        int maxNum = maxValue + 1;

                        string sid1 = elementId;
                        string sid2 = Guid.NewGuid().ToString();
                        string sid3 = string.Format("{0}", maxNum + 1);

                        string geomStr1 = String.Format("GEOMFROMTEXT('{0}')", geometry);
                        string geomStr2 = String.Format("'{0}'", geometry);
                        if (string.IsNullOrEmpty(geometry))
                        {
                            geomStr1 = "''";
                            geomStr2 = "null";
                        }

                        sqlCInsert1 += string.Format("('{0}')", sid1);
                        sqlCInsert2 += string.Format("('{0}','{1}',{2})", sid2, sid1, geomStr1);
                        sqlCInsert3 += string.Format("({0},'{1}',{2})", sid3, sid1, geomStr2);

                        string sqlInsert3 = String.Format("insert into {0}(sid,guid,geom) values{1}", tableName1, sqlCInsert3);
                        actal.ExceuteSQL(sqlInsert3, string.Empty);

                        //MySqlHelper mysql = new MySqlHelper();

                        string sqlInsert1 = String.Format("insert into `{0}`(`sid`) values{1}", tableName1, sqlCInsert1);
                        string sqlInsert2 = String.Format("insert into `{0}`(`sid`,`DataID`,`geom`) values{1}", tableName2, sqlCInsert2);
                        //mysql.ExecuteNonQuery(sqlInsert1);
                        //mysql.ExecuteNonQuery(sqlInsert2);

                        actal.ExceuteSQL(sqlInsert2, string.Empty);

                    }
                }

                _iOperateLogAppService.WriteOperateLog(layerId, userCode, 1004, 1101, 1201, 1413, null);
            }
            else { return false; }
            return true;
        }

        /// <summary>
        /// 编辑一个图层要素
        /// </summary>
        public bool UpdateLayerElement(string userCode, string layerId, string elementId, string[] values, string geometry)
        {
            try
            {
                DeleteLayerElement(userCode,layerId, elementId);

                return AddElement(userCode,layerId, elementId, values, geometry);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        public bool UpdateLayerElementAttribute(string userCode, string layerId, string elementId, string[] values)
        {
            var layer = _ILayerContentRepository.FirstOrDefault(m => m.Id == layerId);
            if (layer == null)
            {
                layer=_ILayerContentRepository.FirstOrDefault(m => m.LayerAttrTable == layerId);
            }
            if (layer != null)
            {
                layerId = layer.Id;
                List<LayerFieldEntity> fields = _ILayerFieldRepository.GetAll().Where(t => t.LayerID == layerId).ToList();
                if (fields != null && fields.Count > 0)
                {
                    string colStr1 = string.Empty;
                    string colStr2 = string.Empty;

                    string sqlCInsert1 = string.Empty;
                    string sqlCInsert2 = string.Empty;
                    string sqlCInsert3 = string.Empty;

                    foreach (string value in values)
                    {
                        string valueStr = string.Empty;
                        string[] vals = value.Split(new char[] { ';' });
                        if (vals.Length >= 2)
                        {
                            LayerFieldEntity field = fields.FirstOrDefault(t => t.AttributeName == vals[0]);
                            if (field != null)
                            {
                                //字符型
                                if (field.AttributeType == "afd042e0-67a8-11e7-8eb2-005056bb1c7e")
                                {
                                    if (vals[1].Contains("'"))
                                    {
                                        string newValue = vals[1].Replace("'", "''");
                                        valueStr = String.Format("'{0}',", newValue);
                                    }
                                    else
                                    {
                                        valueStr = String.Format("'{0}',", vals[1]);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(vals[1]))
                                    {
                                        valueStr = String.Format("{0},", vals[1]);
                                    }
                                    else
                                    {
                                        valueStr = String.Format("'{0}',", vals[1]);
                                    }
                                }

                                colStr1 += "`" + vals[0] + "`" + "=" + valueStr ;
                                colStr2 += "\"" + vals[0] + "\"" + "=" + valueStr;
                            }
                        }
                    }

                    colStr1 = colStr1.TrimEnd(',');
                    colStr2 = colStr2.TrimEnd(',');

                    string tableName1 = layer.LayerAttrTable;
                    string tableName2 = layer.LayerSpatialTable;
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();

                    if (!string.IsNullOrEmpty(colStr1))
                    {
                        string sqlInsert3 = String.Format("update {0} set {1} where guid='{2}'", tableName1, colStr2, elementId);
                        actal.ExceuteSQL(sqlInsert3, string.Empty);

                        //MySqlHelper mysql = new MySqlHelper();
                        string sqlInsert2 = String.Format("update `{0}` set {1} where `DataID`='{2}'", tableName2, colStr1, elementId);
                        //mysql.ExecuteNonQuery(sqlInsert2);
                        actal.ExceuteSQL(sqlInsert2, string.Empty);

                    }
                    else { return false; }
                }

                _iOperateLogAppService.WriteOperateLog(layerId, userCode, 1004, 1102, 1201, 1414, null);
            }
            else { return false; }
            return true;
        }

        public bool UpdateLayerElementGeometry(string layerId, string elementId, string geometry)
        {
            var layer = _ILayerContentRepository.FirstOrDefault(m => m.Id == layerId);
            if (layer != null)
            {
                if (!string.IsNullOrEmpty(geometry))
                {
                    string tableName1 = layer.LayerAttrTable;
                    string tableName2 = layer.LayerSpatialTable;
                    PostgrelVectorHelper actal = new PostgrelVectorHelper();

                    string geomStr1 = String.Format("GEOMFROMTEXT('{0}')", geometry);
                    string geomStr2 = String.Format("'{0}'", geometry);

                    string sqlInsert3 = String.Format("update {0} set geom={1} where guid='{2}'", tableName1, geomStr2, elementId);
                    actal.ExceuteSQL(sqlInsert3, string.Empty);

                    //MySqlHelper mysql = new MySqlHelper();
                    string sqlInsert2 = String.Format("update `{0}` set `geom`={1} where `DataID`='{2}'", tableName2, geomStr1, elementId);
                    //mysql.ExecuteNonQuery(sqlInsert2);
                    actal.ExceuteSQL(sqlInsert2, string.Empty);
                }
                else { return false; }

            }
            return true;
        }

        public string[] Mapgis2Arcgis(string filename)
        {
            List<string> filenames = new List<string>();

            //Process p = null;
            DateTime dtStart = DateTime.Now;
            object fileName = filename;
            if (fileName == null || string.IsNullOrEmpty(fileName.ToString()))
            {
                return null;
            }
            //else
            //{
            //    p = new Process();
            //}

            Environment.CurrentDirectory = ConfigurationManager.AppSettings["Map2ShpPath"];
            //设定程序名
            //p.StartInfo.FileName = ConfigurationManager.AppSettings["Map2ShpPath"] + "iTelluro.DataTools.Console.exe";
            //p.StartInfo.FileName = "cmd.exe";
            ////关闭Shell的使用
            //p.StartInfo.UseShellExecute = false;
            ////重定向标准输入
            //p.StartInfo.RedirectStandardInput = true;
            ////重定向标准输出
            //p.StartInfo.RedirectStandardOutput = true;
            ////重定向错误输出
            //p.StartInfo.RedirectStandardError = true;
            //设置不显示窗口
            string dir = Path.Combine(Path.GetDirectoryName(filename), "ShapeData"); ;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                //Process.EnterDebugMode(); // 等待完成
                Process.Start(ConfigurationManager.AppSettings["Map2ShpPath"] + "iTelluro.DataTools.Console.exe", " Map2Shp -s:" + fileName + " -t:" + dir).WaitForExit(30 * 1000);

                DirectoryInfo folder = new DirectoryInfo(dir);
                foreach (FileInfo file in folder.GetFiles())
                {
                    filenames.Add(file.FullName);
                }
                string[] filenameslst = (string[])filenames.ToArray();
                return filenameslst;
            }
            catch
            {
                return null;
            }
            //p.StartInfo.CreateNoWindow = true;
            //p.StartInfo.Arguments = " Map2Shp -s:" + fileName + " -t:" + dir;


            //p.Start();
            //p.WaitForExit(1000 * 60);
            //if (p.ExitCode == 0)
            //{
            //    DirectoryInfo folder = new DirectoryInfo(dir);
            //    foreach (FileInfo file in folder.GetFiles())
            //    {
            //        filenames.Add(file.FullName);
            //    }
            //    string[]  filenameslst = (string[])filenames.ToArray();
            //    return filenameslst;

            //}
            //else
            //{
            //    return null;
            //}
        }

        public void UpdateBBOX(LayerContentEntity layer)
        {
            GeoServerHelper geoServerHelper = new GeoServerHelper();

            #region [BBox更新]

            PostgrelVectorHelper postgis = new PostgrelVectorHelper();
            string strSQL = string.Format("select max(ST_XMax(geom)),max(ST_YMax(geom)),min(ST_XMin(geom)),min(ST_YMin(geom)) from public.{0}", layer.LayerAttrTable);
            DataTable dt = postgis.getDataTable(strSQL);

            layer.UploadStatus = "1";
            if (dt.Rows.Count > 0)
            {
                layer.MaxX = (!string.IsNullOrEmpty(dt.Rows[0][0].ToString())) ? Convert.ToDecimal(dt.Rows[0][0]) : 0;
                layer.MaxY = (!string.IsNullOrEmpty(dt.Rows[0][1].ToString())) ? Convert.ToDecimal(dt.Rows[0][1]) : 0;
                layer.MinX = (!string.IsNullOrEmpty(dt.Rows[0][2].ToString())) ? Convert.ToDecimal(dt.Rows[0][2]) : 0;
                layer.MinY = (!string.IsNullOrEmpty(dt.Rows[0][3].ToString())) ? Convert.ToDecimal(dt.Rows[0][3]) : 0;
            }

            var result2 = _ILayerContentRepository.Update(layer);

            string strbbox = "";

            if (layer.MinX != null)
            {
                strbbox = layer.MinX.ToString() + "," + layer.MinY.ToString() + "," + layer.MaxX.ToString() + "," + layer.MaxY.ToString();
                string bboxStr = string.Format("{0},{1},{2},{3}", layer.MinX, layer.MinY, layer.MaxX, layer.MaxY);
                geoServerHelper.ModifyLayerBBox(layer.LayerAttrTable, layer.LayerName, bboxStr);
            }

            #endregion

            #region [缩略图下载]

            ThumbnailHelper tbh = new ThumbnailHelper();
            string imagePath = tbh.CreateThumbnail(layer.LayerAttrTable, "layer", strbbox);

            #endregion

        }

        /// <summary>
        /// 矢量裁剪
        /// </summary>
        /// <param name="sourceShp">原始shp文件</param>
        /// <param name="destShp">输出shp文件</param>
        /// <param name="cutshp">裁剪范围的shp文件</param>
        /// <returns></returns>
        public bool ClipShp(string sourceShp, string destShp, string cutshp)
        {
            bool isok = DLGClip.ClipShp(sourceShp, destShp, cutshp);
            return isok;
        }

        /// <summary>
        /// 矢量裁剪
        /// </summary>
        /// <param name="sourceShp">原始shp文件</param>
        /// <param name="destShp">输出shp文件</param>
        /// <param name="wktString">裁剪范围的WKT字符串</param>
        /// <returns></returns>
        public bool ClipShpByWKT(string sourceShp, string destShp, string wktString)
        {
            try
            {
                OSGeo.OGR.Geometry geom = OSGeo.OGR.Geometry.CreateFromWkt(wktString);
                bool isok = DLGClip.ClipShp(sourceShp, destShp, geom);
                return isok;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取行政区划信息
        /// </summary>
        /// <param name="codeStr">行政区划编码，多个以逗号间隔</param>
        /// <param name="level">级别，省、市、县</param>
        /// <returns></returns>
        public object GetDistrictWKTByCode(string codeStr, string level)
        {
            List<District> districts = new List<District>();
            if (!string.IsNullOrEmpty(codeStr))
            {
                string[] list = codeStr.Split(',');
                if (list != null && list.Length > 0 && list[0].Length == 6)
                {
                    if (list[0].EndsWith("0000"))
                    {
                        level = "省";
                    }
                    else if (list[0].EndsWith("00"))
                    {
                        level = "市";
                    }
                    else
                    {
                        level = "县";
                    }
                }
                DistrictSpatialProvider dsp = new DistrictSpatialProvider(level);

                districts = dsp.GetDistrictPolygon(list.ToList());
            }

            return districts;
        }

        /// <summary>
        /// 复制图层，属性数据不复制
        /// </summary>
        /// <param name="oldLayerId">图层编号</param>
        /// <param name="newLayerName">复制后的图层名称</param>
        /// <returns>新图层编号</returns>
        public string CloneLayer(CloneLayerDto dto)
        {
            try
            {
                string oldLayerId = dto.OldLayerId;
                string newLayerName = dto.NewLayerName;
                var entityLayerContent = _ILayerContentRepository.Get(oldLayerId);
                LayerContentEntity newLayerContent = new LayerContentEntity();
                newLayerContent.DataType = entityLayerContent.DataType;
                newLayerContent.CreateBy = entityLayerContent.CreateBy;
                newLayerContent.CreateDT = DateTime.Now;
                newLayerContent.LayerBBox = entityLayerContent.LayerBBox;
                newLayerContent.LayerDefaultStyle = entityLayerContent.LayerDefaultStyle;
                newLayerContent.LayerDesc = entityLayerContent.LayerDesc;
                newLayerContent.LayerTag = entityLayerContent.LayerTag;
                newLayerContent.LayerRefence = entityLayerContent.LayerRefence;
                newLayerContent.LayerType = entityLayerContent.LayerType;
                newLayerContent.MaxX = entityLayerContent.MaxX;
                newLayerContent.MaxY = entityLayerContent.MaxY;
                newLayerContent.MinX = entityLayerContent.MinX;
                newLayerContent.MinY = entityLayerContent.MinY;
                newLayerContent.UploadFileName = entityLayerContent.UploadFileName;
                newLayerContent.UploadFileType = entityLayerContent.UploadFileType;
                newLayerContent.UploadStatus = entityLayerContent.UploadStatus;
                if (newLayerContent != null && !string.IsNullOrEmpty(newLayerName))
                {
                    newLayerContent.Id = Guid.NewGuid().ToString();
                    newLayerContent.LayerName = newLayerName;

                    ChineseConvert chn = new ChineseConvert();
                    string name = chn.GetPinyinInitials(newLayerName);
                    string lastIndex = (System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString());
                    int nameLength = (30 - lastIndex.Length) - 1;
                    newLayerContent.LayerAttrTable = ((newLayerContent.UploadFileType == "1") ? "v" : "g") + ((name.Length > nameLength) ? name.Substring(0, nameLength) : name) + lastIndex;
                    newLayerContent.LayerSpatialTable = ((newLayerContent.UploadFileType == "1") ? "v" : "g") + ((name.Length > (nameLength - 7)) ? name.Substring(0, (nameLength - 7)) : name) + lastIndex + "_s";
                    //复制图层

                    if (_ILayerContentRepository.Insert(newLayerContent) != null)
                    {
                        var listLayerField = _ILayerFieldRepository.GetAllList().Where(t => t.LayerID == oldLayerId);
                        if (listLayerField != null && listLayerField.Count() > 0)
                        {
                            foreach (var itemLayerField in listLayerField)
                            {
                                string newLayerFieldId = Guid.NewGuid().ToString();
                                var listLayerFieldDict = _ILayerFieldDictRepository.GetAllList().Where(t => t.AttributeID == itemLayerField.Id);
                                if (listLayerFieldDict != null && listLayerFieldDict.Count() > 0)
                                {
                                    foreach (var itemLayerFieldDict in listLayerFieldDict)
                                    {
                                        //复制图层属性字典                                        
                                        LayerFieldDictEntity newLayerFieldDict = new LayerFieldDictEntity();
                                        newLayerFieldDict.Id = Guid.NewGuid().ToString();
                                        newLayerFieldDict.AttributeID = newLayerFieldId;
                                        newLayerFieldDict.FieldDictDesc = itemLayerFieldDict.FieldDictDesc;
                                        newLayerFieldDict.FieldDictName = itemLayerFieldDict.FieldDictName;
                                        _ILayerFieldDictRepository.Insert(newLayerFieldDict);
                                    }
                                }
                                //复制图层属性                                
                                LayerFieldEntity newLayerField = new LayerFieldEntity();
                                newLayerField.LayerID = newLayerContent.Id;
                                newLayerField.Id = newLayerFieldId;
                                newLayerField.AttributeCalComp = itemLayerField.AttributeCalComp;
                                newLayerField.AttributeDataSource = itemLayerField.AttributeDataSource;
                                newLayerField.AttributeDataType = itemLayerField.AttributeDataType;
                                newLayerField.AttributeDefault = itemLayerField.AttributeDefault;
                                newLayerField.AttributeDesc = itemLayerField.AttributeDesc;
                                newLayerField.AttributeInputCtrl = itemLayerField.AttributeInputCtrl;
                                newLayerField.AttributeInputFormat = itemLayerField.AttributeInputFormat;
                                newLayerField.AttributeInputMax = itemLayerField.AttributeInputMax;
                                newLayerField.AttributeInputMin = itemLayerField.AttributeInputMin;
                                newLayerField.AttributeIsNull = itemLayerField.AttributeIsNull;
                                newLayerField.AttributeLength = itemLayerField.AttributeLength;
                                newLayerField.AttributeName = itemLayerField.AttributeName;
                                newLayerField.AttributePrecision = itemLayerField.AttributePrecision;
                                newLayerField.AttributeSort = itemLayerField.AttributeSort;
                                newLayerField.AttributeType = itemLayerField.AttributeType;
                                newLayerField.AttributeUnit = itemLayerField.AttributeUnit;
                                newLayerField.AttributeValueLink = itemLayerField.AttributeValueLink;
                                newLayerField.Remark = itemLayerField.Remark;
                                newLayerField.CreateDT = DateTime.Now;
                                _ILayerFieldRepository.Insert(newLayerField);
                            }
                        }
                        //创建空间数据表
                        var query = GetDetailByLayerID(listLayerField.ToList(), newLayerContent.Id);
                        string strPostSQL = PostGISTableCreateSql(query, newLayerContent.LayerAttrTable);
                        PostgrelVectorHelper pvh = new PostgrelVectorHelper();
                        pvh.ExceuteSQL(strPostSQL, newLayerContent.LayerAttrTable);

                        return newLayerContent.Id;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 生成图层
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public string CreateLayer(CreateLayerDto dto)
        {
            try
            {
                var entityLayerContent = _ILayerContentRepository.Get(dto.LayerId);
                LayerContentEntity newLayerContent = new LayerContentEntity();
                newLayerContent.DataType = entityLayerContent.DataType;
                newLayerContent.CreateBy = entityLayerContent.CreateBy;
                newLayerContent.CreateDT = DateTime.Now;
                newLayerContent.LayerBBox = entityLayerContent.LayerBBox;
                newLayerContent.LayerDefaultStyle = entityLayerContent.LayerDefaultStyle;
                newLayerContent.LayerDesc = entityLayerContent.LayerDesc;
                newLayerContent.LayerTag = entityLayerContent.LayerTag;
                newLayerContent.LayerRefence = entityLayerContent.LayerRefence;
                newLayerContent.LayerType = entityLayerContent.LayerType;
                newLayerContent.MaxX = entityLayerContent.MaxX;
                newLayerContent.MaxY = entityLayerContent.MaxY;
                newLayerContent.MinX = entityLayerContent.MinX;
                newLayerContent.MinY = entityLayerContent.MinY;
                newLayerContent.UploadFileName = entityLayerContent.UploadFileName;
                newLayerContent.UploadFileType = entityLayerContent.UploadFileType;
                newLayerContent.UploadStatus = entityLayerContent.UploadStatus;
                if (newLayerContent != null && !string.IsNullOrEmpty(dto.NewLayerName))
                {
                    newLayerContent.Id = Guid.NewGuid().ToString();
                    newLayerContent.LayerName = dto.NewLayerName;

                    ChineseConvert chn = new ChineseConvert();
                    string name = chn.GetPinyinInitials(dto.NewLayerName);
                    string lastIndex = (System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString());
                    int nameLength = (30 - lastIndex.Length) - 1;
                    newLayerContent.LayerAttrTable = ((newLayerContent.UploadFileType == "1") ? "v" : "g") + ((name.Length > nameLength) ? name.Substring(0, nameLength) : name) + lastIndex;
                    newLayerContent.LayerSpatialTable = ((newLayerContent.UploadFileType == "1") ? "v" : "g") + ((name.Length > (nameLength - 7)) ? name.Substring(0, (nameLength - 7)) : name) + lastIndex + "_s";
                    //创建图层
                    if (_ILayerContentRepository.Insert(newLayerContent) != null)
                    {
                        //创建图层属性
                        var newLayerField = new LayerFieldEntity()
                        {
                            Id = Guid.NewGuid().ToString(),
                            LayerID = newLayerContent.Id,
                            AttributeName = dto.AttributeName
                        };
                        _ILayerFieldRepository.Insert(newLayerField);
                        //创建空间数据表
                        string strPostSQL = string.Format("CREATE TABLE {0}(sid SERIAL primary key,guid varchar,{1} varchar,geom geometry)", newLayerContent.LayerAttrTable, dto.AttributeName);
                        PostgrelVectorHelper pvh = new PostgrelVectorHelper();
                        pvh.ExceuteSQL(strPostSQL, newLayerContent.LayerAttrTable);

                        //插入数据
                        string insertSql = string.Format("insert into {0} (sid,guid,geom) select sid,guid,geom from {1}", newLayerContent.LayerAttrTable, entityLayerContent.LayerAttrTable);
                        pvh.ExceuteSQL(insertSql, string.Empty);
                        foreach (var element in dto.AttributeValues)
                        {
                            string updateSql = string.Format("update {0} set {1}='{2}' where guid='{3}'", newLayerContent.LayerAttrTable, dto.AttributeName, element.AttributeValue, element.ElementId);
                            pvh.ExceuteSQL(updateSql, string.Empty);
                        }

                        return newLayerContent.Id;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 发布地图
        /// </summary>
        /// <param name="mapDto">地图信息</param>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public MapEntity PublishMap(PublishMapDto mapDto)
        {
            try
            {
                var layerEntity = PublicLayer(mapDto.LayerName, mapDto.LayerType, mapDto.FileName);
                if (layerEntity == null)
                {
                    return null;
                }

                //保存地图数据
                string mapId = Guid.NewGuid().ToString();
                MapEntity entityMap = new MapEntity
                {
                    Id = mapId,
                    MapName = mapDto.MapName,
                    MapType = mapDto.MapType,
                    CreateDT = DateTime.Now,
                    SpatialRefence = ConfigurationManager.AppSettings["SpatialRefence"]
                };
                ChineseConvert chn = new ChineseConvert();
                string mapEnName = chn.GetPinyinInitials(entityMap.MapName);
                string lastIndex = (System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString());
                entityMap.MapEnName = mapEnName + lastIndex;
                var query = _IMapRepository.Insert(entityMap);

                //保存地图图层关系
                string releationId = Guid.NewGuid().ToString();
                MapReleationEntity entityMapReleation = new MapReleationEntity
                {
                    Id = releationId,
                    MapID = mapId,
                    DataConfigID = layerEntity.Id,
                    DataSort = 1,
                    ConfigDT = DateTime.Now,
                    ModifyDT = DateTime.Now
                };
                _IMapReleationRepository.Insert(entityMapReleation);

                //更新地图数据
                List<string> layers = new List<string>();
                layers.Add(layerEntity.Id);
                UpdateMap(entityMap, layers);

                //发布地图
                PublicMap(entityMap, layerEntity);

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 发布地图
        /// </summary>
        /// <param name="map">地图</param>
        /// <returns></returns>
        private bool PublicMap(MapEntity map, LayerContentEntity listLayer)
        {
            List<string> targetLayers = new List<string>();
            List<string> styleLayers = new List<string>();
            string mapName = string.Empty;
            targetLayers.Add(listLayer.LayerAttrTable);
            mapName = map.MapEnName;

            //var map = _IMapRepository.Get(mapId);
            GeoServerHelper geoHelp = new GeoServerHelper();

            geoHelp.AddLayerGroup(mapName, targetLayers, styleLayers);

            /*操作频繁，效率太低*/

            //string isAutoCache = ConfigurationManager.AppSettings.Get("IsAutoCache");
            //if (!string.IsNullOrEmpty(isAutoCache) && isAutoCache == "1")
            //{
            //    geoHelp.TerminatingTask(mapName);
            //    geoHelp.TileMap(map.MapEnName);
            //}

            #region [生成缩略图]

            string strBBox = map.MinX.ToString() + "," + map.MinY.ToString() + "," + map.MaxX.ToString() + "," + map.MaxY.ToString();
            GetThumbnial(map.MapEnName, strBBox);

            #endregion

            return true;

        }

        private bool GetThumbnial(string name, string strBBox)
        {

            #region [生成缩略图]

            ThumbnailHelper tbh = new ThumbnailHelper();
            string imagePath = tbh.CreateThumbnail(name, "map", strBBox);

            if (string.IsNullOrEmpty(imagePath))
            {
                return false;
            }
            else
            {
                return true;
            }

            #endregion
        }

        private void UpdateMap(MapEntity map, List<string> layers)
        {
            if (layers != null && layers.Count > 0)
            {
                if (map != null)
                {
                    var list = _ILayerContentRepository.GetAllList().Where(t => layers.Contains(t.Id)).ToList();
                    if (list != null)
                    {
                        Dictionary<string, decimal?> bbox = new Dictionary<string, decimal?>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (i == 0)
                            {
                                bbox.Add("MaxX", list[i].MaxX);
                                bbox.Add("MinX", list[i].MinX);
                                bbox.Add("MaxY", list[i].MaxY);
                                bbox.Add("MinY", list[i].MinY);
                            }
                            else
                            {
                                if (list[i].MaxX > bbox["MaxX"])
                                {
                                    bbox["MaxX"] = list[i].MaxX;
                                }
                                if (list[i].MaxY > bbox["MaxY"])
                                {
                                    bbox["MaxY"] = list[i].MaxY;
                                }
                                if (list[i].MinX < bbox["MinX"])
                                {
                                    bbox["MinX"] = list[i].MinX;
                                }
                                if (list[i].MinY < bbox["MinY"])
                                {
                                    bbox["MinY"] = list[i].MinY;
                                }
                            }
                        }

                        map.MaxX = bbox["MaxX"];
                        map.MinX = bbox["MinX"];
                        map.MaxY = bbox["MaxY"];
                        map.MinY = bbox["MinY"];

                        _IMapRepository.Update(map);
                    }
                }
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        private LayerContentEntity PublicLayer(string layerName, string layerType, string fileName)
        {
            try
            {
                string name = "";
                string UploadFileType = "2";
                string layerId = Guid.NewGuid().ToString();
                string layerAttrTable = string.Empty;
                string layerSpatialTable = string.Empty;
                if (!string.IsNullOrEmpty(layerName))
                {
                    ChineseConvert chn = new ChineseConvert();
                    name = chn.GetPinyinInitials(layerName);
                    string lastIndex = (System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString());
                    int nameLength = (30 - lastIndex.Length) - 1;
                    layerAttrTable = ((UploadFileType == "1") ? "v" : "g") + ((name.Length > nameLength) ? name.Substring(0, nameLength) : name) + lastIndex;
                    layerSpatialTable = ((UploadFileType == "1") ? "v" : "g") + ((name.Length > (nameLength - 7)) ? name.Substring(0, (nameLength - 7)) : name) + lastIndex + "_s";
                }
                LayerContentEntity entity = new LayerContentEntity
                {
                    Id = layerId,
                    LayerName = layerName,
                    DataType = layerType,
                    LayerDesc = name,
                    LayerAttrTable = layerAttrTable,
                    LayerSpatialTable = layerSpatialTable,
                    LayerRefence = ConfigurationManager.AppSettings["SpatialRefence"],
                    CreateDT = DateTime.Now,
                    UploadFileType = UploadFileType,
                    UploadFileName = fileName
                };
                if (UploadFileType == "1")
                {
                    var query = _ILayerContentRepository.Insert(entity);
                    return query;
                }
                else if (UploadFileType == "2")
                {
                    string GeoServerIp = ConfigurationManager.AppSettings["GeoServerIp"];
                    string GeoServerPort = ConfigurationManager.AppSettings["GeoServerPort"];
                    string GeoWorkSpace = ConfigurationManager.AppSettings["GeoWorkSpace"];
                    string UploadFilePath = ConfigurationManager.AppSettings["UploadFilePath"];

                    string zipFilePath = Path.Combine(UploadFilePath, layerAttrTable + ".zip");

                    GeoServer geoServer = new GeoServer(GeoServerIp, GeoServerPort);

                    var list = geoServer.GetCoverageStores(GeoWorkSpace);

                    //if (!input.UploadFileName.Contains("*"))
                    //{
                    //    geoServer.PutCoverageStore(GeoWorkSpace, input.LayerAttrTable, Path.Combine(UploadFilePath, input.UploadFileName));
                    //    geoServer.UpdateStoreTitle(GeoWorkSpace, input.LayerAttrTable, input.LayerAttrTable, input.LayerName);
                    //}
                    //else
                    //{
                    if (!Directory.Exists(Path.Combine(UploadFilePath, layerAttrTable)))
                    {
                        Directory.CreateDirectory(Path.Combine(UploadFilePath, layerAttrTable));
                    }
                    string tifPath = "";
                    foreach (string item in fileName.Split('*'))
                    {
                        if (File.Exists(Path.Combine(UploadFilePath, item)))
                        {
                            string newName = layerAttrTable + Path.GetExtension(item);
                            File.Copy(Path.Combine(UploadFilePath, item), Path.Combine(UploadFilePath, layerAttrTable, newName), true);
                            if (Path.GetExtension(item).ToLower() == ".tif" || Path.GetExtension(item).ToLower() == ".geotiff")
                            {
                                tifPath = Path.Combine(UploadFilePath, item);
                            }
                        }
                    }

                    if (File.Exists(tifPath))
                    {
                        try
                        {
                            RasterImgInfo imgInfo = new RasterImgInfo();
                            bool isok = imgInfo.Open(tifPath, true);
                            if (isok)
                            {
                                entity.MinX = Convert.ToDecimal(imgInfo.XLeft);
                                entity.MinY = Convert.ToDecimal(imgInfo.YBottom);
                                entity.MaxX = Convert.ToDecimal(imgInfo.XRight);
                                entity.MaxY = Convert.ToDecimal(imgInfo.YTop);

                                string csSrc = imgInfo.Coordsystem.WKT;
                                string SpatialRefence = System.Configuration.ConfigurationManager.AppSettings["SpatialRefence"].ToString();

                                if (!csSrc.Contains(SpatialRefence) && !csSrc.Contains(SpatialRefence.Replace("GCS_", "")))
                                {
                                    string msg = UtilityMessageConvert.Get("上传文件解析异常(与系统默认坐标系不匹配)");
                                    throw new Exception(msg);
                                }

                            }
                            imgInfo.Close();
                            imgInfo.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Abp.Logging.LogHelper.Logger.Error(ex.Message, ex);
                            throw ex;
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }

                    ZipHelper zip = new ZipHelper();

                    //ZipFile.CreateFromDirectory(Path.Combine(UploadFilePath, input.LayerAttrTable), zipFilePath);

                    bool success = zip.ZipDir(Path.Combine(UploadFilePath, layerAttrTable), zipFilePath);
                    if (success)
                    {
                        geoServer.PutCoverageStore(GeoWorkSpace, layerAttrTable, zipFilePath);
                        geoServer.UpdateStoreTitle(GeoWorkSpace, layerAttrTable, layerAttrTable, layerName);
                        string srs = string.Empty;
                        var data = geoServer.GetConverageInfo(GeoWorkSpace, layerAttrTable, layerAttrTable);
                        if (data != null)
                        {
                            srs = data.Coverage.Srs;
                        }
                        //生成缩略图
                        ThumbnailHelper tbh = new ThumbnailHelper();

                        string bboxStr = string.Format("{0},{1},{2},{3}", entity.MinX, entity.MinY, entity.MaxX, entity.MaxY);
                        string imagePath = tbh.CreateThumbnail(layerAttrTable, "layer", bboxStr, UploadFileType, srs);
                    }
                    else
                    {
                        throw new Exception();
                    }
                    //}
                    entity.UploadStatus = "1";
                    var query = _ILayerContentRepository.Insert(entity);
                    return query;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// PostGIS
        /// </summary>
        /// <param name="listDto"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string PostGISTableCreateSql(ListResultOutput<LayerFieldDto> listDto, string tableName)
        {

            string config = ConfigurationManager.AppSettings["DataBase"].ToString();
            int startIndex = config.LastIndexOf(":");
            string dataBaseType = config.Substring(config.LastIndexOf(":") + 1);

            string strSql = string.Format("CREATE TABLE \"{0}\"(sid SERIAL primary key,guid varchar,", tableName);
            for (int i = 0; i < listDto.Items.Count; i++)
            {
                string attrType = listDto.Items[i].AttributeType;
                if (!string.IsNullOrEmpty(listDto.Items[i].AttributeTypeName))
                {
                    if (listDto.Items[i].AttributeTypeName.ToLower() == "double" || listDto.Items[i].AttributeTypeName.ToLower() == "float")
                    {
                        listDto.Items[i].AttributeTypeName = "double precision";
                    }
                    else if (listDto.Items[i].AttributeTypeName.ToLower() == "int" || listDto.Items[i].AttributeTypeName.ToLower() == "long integer" || listDto.Items[i].AttributeTypeName.ToLower() == "short integer")
                    {
                        listDto.Items[i].AttributeTypeName = "integer";
                    }
                    else if (listDto.Items[i].AttributeTypeName.ToLower() == "datetime")
                    {
                        listDto.Items[i].AttributeTypeName = "date";
                    }
                    strSql += String.Format("\"{0}\" {1},", listDto.Items[i].AttributeName, listDto.Items[i].AttributeTypeName);
                }
            }

            strSql = strSql.TrimEnd(',');
            strSql += String.Format(",geom geometry)");

            return strSql;
        }

        /// <summary>
        /// 根据图层目录来查图层配置
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        private ListResultOutput<LayerFieldDto> GetDetailByLayerID(List<LayerFieldEntity> query, string layerID)
        {
            try
            {
                //var query = _ILayerFieldRepository.GetAllList().Where(q => q.LayerID == layerID).OrderBy(c => c.AttributeSort);
                List<LayerFieldDto> layerFields = query.MapTo<List<LayerFieldDto>>();

                List<string> attributeTypes = layerFields.Select(t => t.AttributeType).ToList();

                string config = ConfigurationManager.AppSettings["DataBase"].ToString();
                int startIndex = config.LastIndexOf(":");
                string dataBaseType = config.Substring(config.LastIndexOf(":") + 1);

                List<DicDataCodeEntity> dicDataCodeEntitys = _IDicDataCodeRepository.GetAllList().Where(q => q.DataTypeID == dataBaseType && attributeTypes.Contains(q.Keywords)).ToList();

                layerFields.ForEach(t =>
                {
                    DicDataCodeEntity entity = dicDataCodeEntitys.Find(m => m.Keywords == t.AttributeType);
                    if (entity != null && entity.CodeValue != null)
                    {
                        t.AttributeTypeName = entity.CodeValue;
                        t.CodeName = entity.CodeName;
                    }
                });

                var list = new ListResultOutput<LayerFieldDto>(layerFields);
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
}
