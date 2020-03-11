using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.LayerContentApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Common;
using InfoEarthFrame.Common.Data;
using System.Linq;
using AutoMapper;
using InfoEarthFrame.LayerFieldApp;
using InfoEarthFrame.Application.LayerFieldApp;
using Abp.Domain.Uow;
using System.IO;
using InfoEarthFrame.Common.ShpUtility;
using System.Collections;
using OSGeo.OGR;
using iTelluro.DataTools.Utility.GIS;
using System.Configuration;
using System.Linq.Expressions;
using System.Data;
using InfoEarthFrame.GeoServerRest;
using System.IO.Compression;
using InfoEarthFrame.Application.SystemUserApp;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using iTelluro.DataTools.Utility.Img;
using InfoEarthFrame.Application.OperateLogApp;
using System.Net;
using System.Text;
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


namespace InfoEarthFrame.Application.LayerContentApp
{

    public class LayerContentAppService : IApplicationService, ILayerContentAppService
    {
        #region 变量
        private readonly ILayerContentRepository _ILayerContentRepository;
        private readonly ILayerFieldRepository _ILayerFieldRepository;
        private readonly IDicDataCodeRepository _IDicDataCodeRepository;
        private readonly IDataTypeRepository _IDataTypeRepository;
        private readonly IMapReleationRepository _IMapReleationRepository;
        private readonly ITagReleationRepository _ITagReleationRepository;
        private readonly IDataTagRepository _IDataTagRepository;
        private readonly IDataStyleRepository _IDataStyleRepository;
        private readonly ILayerFieldAppService _layerFieldAppService;
        private readonly ISystemUserRepository _ISystemUserRepository;
        private readonly SystemUserAppService _SystemUserApp;
        private readonly IOperateLogAppService _IOperateLogAppService;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public LayerContentAppService(ILayerContentRepository iLayerContentRepository,
            ILayerFieldRepository ILayerFieldRepository,
            IDicDataCodeRepository iDicDataCodeRepository,
            IDataTypeRepository iDataTypeRepository,
            IMapReleationRepository iMapReleationRepository,
            ITagReleationRepository iTagReleationRepository,
            IDataTagRepository iDataTagRepository,
            ILayerFieldAppService layerFieldAppService,
            IDataStyleRepository iDataStyleRepository,
            ISystemUserRepository iSystemUserRepository,
            IOperateLogAppService iOperateLogAppService)
        {
            _ILayerContentRepository = iLayerContentRepository;
            _ILayerFieldRepository = ILayerFieldRepository;
            _IDicDataCodeRepository = iDicDataCodeRepository;
            _IDataTypeRepository = iDataTypeRepository;
            _IMapReleationRepository = iMapReleationRepository;
            _ITagReleationRepository = iTagReleationRepository;
            _layerFieldAppService = layerFieldAppService;
            _IDataTagRepository = iDataTagRepository;
            _IDataStyleRepository = iDataStyleRepository;
            _ISystemUserRepository = iSystemUserRepository;
            _SystemUserApp = new SystemUserAppService(_ISystemUserRepository);
            _IOperateLogAppService = iOperateLogAppService;
        }
        #endregion

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public async Task<ListResultOutput<LayerContentDto>> GetAllList()
        {
            try
            {
                //var query = await _ILayerContentRepository.GetAllListAsync();
                var query =  _ILayerContentRepository.GetAllList();
                var list = new ListResultOutput<LayerContentDto>(query.MapTo<List<LayerContentDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 图层统计列表
        /// </summary>
        /// <returns></returns>
        public DataTypeCountOutput GetAllCountByDataType(string userCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return null;
                }

                List<LayerContentEntity> layerList = null;
                if (userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    #region 超级用户
                    layerList = _ILayerContentRepository.GetAllList();
                    #endregion
                }
                else
                {
                    List<SystemUserDto> userQuery = _SystemUserApp.GetUserDataByUserCodeAsync(userCode).Result;
                    if (userQuery != null && userQuery.Count > 0)
                    {
                        var userids = userQuery.Select(s => s.UserCode).ToArray();
                        if (userids != null && userids.Any())
                        {
                            layerList = _ILayerContentRepository.GetAllList(s => userids.Contains(s.CreateBy));
                        }
                    }
                }

                //var layerList = _ILayerContentRepository.GetAllList();

                //if (!string.IsNullOrEmpty(userCode))
                //{
                //    List<string> listUserCode = _IOperateLogAppService.GetAllUserCodeByUserCode(userCode);
                //    if (listUserCode != null)
                //    {
                //        if (listUserCode.Count != 0)
                //        {
                //            layerList = layerList.Where(s => listUserCode.Contains(s.CreateBy)).ToList();
                //        }
                //    }
                //}

                var dataTypeChild = _IDataTypeRepository.GetAllList();
                var dataTypeParent = _IDataTypeRepository.GetAllList();

                //类型父子级集合
                var result = (from l in layerList
                              join c in dataTypeChild on l.LayerType equals c.Id
                              join p in dataTypeParent on c.ParentID equals p.Id into cp
                              from cpp in cp.DefaultIfEmpty()
                              group c by new
                              {
                                  DataTypeParentName = (cpp == null) ? c.TypeName : cpp.TypeName,
                                  DataTypeParentID = (cpp == null) ? c.Id : cpp.Id
                              } into g
                              select new
                              {
                                  DataType = g.Key.DataTypeParentID,
                                  DataTypeName = g.Key.DataTypeParentName,
                                  DataCount = g.Count()
                              });

                DataTypeCountOutput ret = new DataTypeCountOutput();
                ret.Data = new List<DataTypeCountDto>();
                ret.Colors = new List<string>();
                ret.Names = new List<string>();

                int i = 0, count = 0, cnt = result.Count();

                foreach (var item in result)
                {
                    ret.Data.Add(new DataTypeCountDto
                    {
                        Id = item.DataType,
                        Name = item.DataTypeName,
                        Value = item.DataCount
                    });
                    ret.Names.Add(item.DataTypeName);
                }
                return ret;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 获取所有数据
        /// </summary>
        public PagedResultOutput<LayerContentDto> GetAllListStatus(LayerContentInputDto input, int PageSize, int PageIndex)
        {
            try
            {
                string layerType = input.LayerType, layerTag = input.LayerTag, layerName = input.LayerName, mapID = input.LayerDesc;

                string listLayer = "", type = "", tag = "";

                List<LayerContentDto> list = new List<LayerContentDto>();

                if (!string.IsNullOrEmpty(layerType))
                {
                    type = GetMultiChildTypeByType(layerType);
                }

                if (!string.IsNullOrEmpty(layerTag))
                {
                    tag = GetMultiLayerIDByTag(layerTag);
                }

                if (string.IsNullOrWhiteSpace(input.CreateBy))
                {
                    return null;
                }

                var layerContent = GetDataByUserCodeAsync(input.CreateBy, layerName, type, tag, string.Empty, ConstHelper.UPLOAD_SUCCESS).Result;

                var listLayerType = _IDataTypeRepository.GetAll();
                var listlayerRefence = _IDicDataCodeRepository.GetAll();
                var listDataType = _IDicDataCodeRepository.GetAll();

                var query = (from l in layerContent
                             join t in listLayerType on l.LayerType equals t.Id into tt
                             from de in tt.DefaultIfEmpty()
                             join r in listlayerRefence on l.LayerRefence equals r.Id into rr
                             from re in rr.DefaultIfEmpty()
                             join dt in listDataType on l.DataType equals dt.Id into dtt
                             from ldt in dtt.DefaultIfEmpty()
                             select new LayerContentDto
                             {
                                 Id = l.Id,
                                 LayerName = l.LayerName,
                                 DataType = l.DataType,
                                 DataTypeName = (ldt == null) ? "" : ldt.CodeName,
                                 LayerBBox = l.LayerBBox,
                                 LayerType = l.LayerType,
                                 LayerTypeName = (de == null) ? "" : de.TypeName,
                                 LayerTag = l.LayerTag,
                                 LayerDesc = l.LayerDesc,
                                 LayerAttrTable = l.LayerAttrTable,
                                 LayerSpatialTable = l.LayerSpatialTable,
                                 LayerRefence = l.LayerRefence,
                                 LayerRefenceName = (re == null) ? "" : re.CodeName,
                                 CreateDT = l.CreateDT,
                                 MaxY = l.MaxY,
                                 MinY = l.MinY,
                                 MinX = l.MinX,
                                 MaxX = l.MaxX,
                                 UploadStatus = l.UploadStatus,
                                 CreateBy = l.CreateBy,
                                 LayerDefaultStyle = l.LayerDefaultStyle
                             }).OrderByDescending(x => x.CreateDT);

                int count = query.Count();
                var result = query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

                IReadOnlyList<LayerContentDto> ir;

                if (result != null && result.Count > 0)
                {

                    foreach (var item in result)
                    {
                        item.LayerTag = GetMultiTagNameByMapID(item.Id);
                        item.LayerDefaultStyleName = (!String.IsNullOrEmpty(item.LayerDefaultStyle)) ? GetDefaultStyleName(item.LayerDefaultStyle) : "";
                    }
                    ir = result.MapTo<List<LayerContentDto>>();
                }
                else
                {
                    ir = new List<LayerContentDto>();
                }
                PagedResultOutput<LayerContentDto> outputList = new PagedResultOutput<LayerContentDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region 根据当前登录人所在行政区划，获取对应行政区划下所有人员信息,在返回对应人员信息相关数据
        /// <summary>
        /// 根据当前登录人所在行政区划，获取对应行政区划下所有人员信息,在返回对应人员信息相关数据
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        private async Task<List<LayerContentEntity>> GetDataByUserCodeAsync(string userCode, string layerName, string layerType, string layerTag, string dataType, string uploadStatus)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return null;
                }

                List<LayerContentEntity> query = null;
                if (userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    #region 超级用户
                    //query = await _ILayerContentRepository.GetAllListAsync();
                    query = _ILayerContentRepository.GetAllList();
                    #endregion
                }
                else
                {
                    List<SystemUserDto> userQuery = await _SystemUserApp.GetUserDataByUserCodeAsync(userCode);
                    if (userQuery != null && userQuery.Count > 0)
                    {
                        var userids = userQuery.Select(s => s.UserCode).ToArray();
                        if (userids != null && userids.Any())
                        {
                            //query = await _ILayerContentRepository.GetAllListAsync(s => userids.Contains(s.CreateBy));
                            query =  _ILayerContentRepository.GetAllList(s => userids.Contains(s.CreateBy));
                        }
                    }
                }
                if (query != null && query.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(uploadStatus))
                    {
                        query = query.Where(s => !string.IsNullOrWhiteSpace(s.UploadStatus) && s.UploadStatus.Equals(uploadStatus)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(layerName))
                    {
                        query = query.Where(s => !string.IsNullOrWhiteSpace(s.LayerName) && s.LayerName.Contains(layerName)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(layerType))
                    {
                        query = query.Where(s => !string.IsNullOrWhiteSpace(s.LayerType) && layerType.Contains(s.LayerType)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(layerTag))
                    {
                        query = query.Where(s => !string.IsNullOrWhiteSpace(s.Id) && layerTag.Contains(s.Id)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(dataType))
                    {
                        query = query.Where(s => !string.IsNullOrWhiteSpace(s.DataType) && dataType.Contains(s.DataType)).ToList();
                    }
                }
                return query;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 返回图层总数
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetAllCount(string userCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return 0;
                }

                List<LayerContentEntity> layerList = null;
                if (userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    #region 超级用户
                    //layerList = await _ILayerContentRepository.GetAllListAsync();
                    layerList =  _ILayerContentRepository.GetAllList();
                    #endregion
                }
                else
                {
                    List<SystemUserDto> userQuery = await _SystemUserApp.GetUserDataByUserCodeAsync(userCode);
                    if (userQuery != null && userQuery.Count > 0)
                    {
                        var userids = userQuery.Select(s => s.UserCode).ToArray();
                        if (userids != null && userids.Any())
                        {
                            //layerList = await _ILayerContentRepository.GetAllListAsync(s => userids.Contains(s.CreateBy));
                            layerList =  _ILayerContentRepository.GetAllList(s => userids.Contains(s.CreateBy));
                        }
                    }
                }

                //var query = await _ILayerContentRepository.GetAllListAsync();

                //if (!string.IsNullOrEmpty(userCode))
                //{
                //    List<string> listUserCode = _IOperateLogAppService.GetAllUserCodeByUserCode(userCode);
                //    if (listUserCode != null)
                //    {
                //        if (listUserCode.Count != 0)
                //        {
                //            query = query.Where(s => listUserCode.Contains(s.CreateBy)).ToList();
                //        }
                //    }
                //}

                return layerList.Count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        public ListResultOutput<LayerContentDto> GetAllListByName(LayerContentInputDto input)
        {
            try
            {
                ListResultOutput<LayerContentDto> list;
                string layerType = input.LayerType, layerTag = input.LayerTag, layerName = input.LayerName;
                string type = "", tag = "";

                if (!string.IsNullOrEmpty(layerType))
                {
                    type = GetMultiChildTypeByType(layerType);
                }

                if (!string.IsNullOrEmpty(layerTag))
                {
                    tag = GetMultiLayerIDByTag(layerTag);
                }

                if (string.IsNullOrWhiteSpace(input.CreateBy))
                {
                    return null;
                }

                var layerContent = GetDataByUserCodeAsync(input.CreateBy, layerName, type, tag, string.Empty, string.Empty).Result;

                var listLayerType = _IDataTypeRepository.GetAll();
                var listlayerRefence = _IDicDataCodeRepository.GetAll();
                var listDataType = _IDicDataCodeRepository.GetAll();

                var query = (from l in layerContent
                             join t in listLayerType on l.LayerType equals t.Id into tt
                             from de in tt.DefaultIfEmpty()
                             join r in listlayerRefence on l.LayerRefence equals r.Id into rr
                             from re in rr.DefaultIfEmpty()
                             join dt in listDataType on l.DataType equals dt.Id into dtt
                             from ldt in dtt.DefaultIfEmpty()
                             select new LayerContentDto
                             {
                                 Id = l.Id,
                                 LayerName = l.LayerName,
                                 DataType = l.DataType,
                                 DataTypeName = (ldt == null) ? "" : ldt.CodeName,
                                 LayerBBox = l.LayerBBox,
                                 LayerType = l.LayerType,
                                 LayerTypeName = (de == null) ? "" : de.TypeName,
                                 LayerTag = l.LayerTag,
                                 LayerDesc = l.LayerDesc,
                                 LayerAttrTable = l.LayerAttrTable,
                                 LayerSpatialTable = l.LayerSpatialTable,
                                 LayerRefence = l.LayerRefence,
                                 LayerRefenceName = (re == null) ? "" : re.CodeName,
                                 CreateDT = l.CreateDT,
                                 MaxY = l.MaxY,
                                 MinY = l.MinY,
                                 MinX = l.MinX,
                                 MaxX = l.MaxX,
                                 UploadStatus = l.UploadStatus,
                                 CreateBy = l.CreateBy,
                                 LayerDefaultStyle = l.LayerDefaultStyle
                             }).OrderByDescending(x => x.CreateDT);

                var result = query.ToList();
                list = new ListResultOutput<LayerContentDto>(result.MapTo<List<LayerContentDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        public PagedResultOutput<LayerContentDto> GetPageListByName(LayerContentInputDto input, int PageSize, int PageIndex)
        {
            try
            {
                string layerType = input.LayerType, layerTag = input.LayerTag, layerName = input.LayerName, dataType = input.DataType;
                string type = "", tag = "";

                List<LayerContentDto> list = new List<LayerContentDto>();

                if (!string.IsNullOrEmpty(layerType))
                {
                    type = GetMultiChildTypeByType(layerType);
                }

                if (!string.IsNullOrEmpty(layerTag))
                {
                    tag = GetMultiLayerIDByTag(layerTag);
                }

                if (string.IsNullOrWhiteSpace(input.CreateBy))
                {
                    return null;
                }

                var layerContent = GetDataByUserCodeAsync(input.CreateBy, layerName, type, tag, dataType, string.Empty).Result;

                var listLayerType = _IDataTypeRepository.GetAll();
                var listlayerRefence = _IDicDataCodeRepository.GetAll();
                var listDataType = _IDicDataCodeRepository.GetAll();

                var query = (from l in layerContent
                             join t in listLayerType on l.LayerType equals t.Id into tt
                             from de in tt.DefaultIfEmpty()
                             join r in listlayerRefence on l.LayerRefence equals r.Id into rr
                             from re in rr.DefaultIfEmpty()
                             join dt in listDataType on l.DataType equals dt.Id into dtt
                             from ldt in dtt.DefaultIfEmpty()
                             select new LayerContentDto
                             {
                                 Id = l.Id,
                                 LayerName = l.LayerName,
                                 DataType = l.DataType,
                                 DataTypeName = (ldt == null) ? "" : ldt.CodeName,
                                 LayerBBox = l.LayerBBox,
                                 LayerType = l.LayerType,
                                 LayerTypeName = (de == null) ? "" : de.TypeName,
                                 LayerTag = l.LayerTag,
                                 LayerDesc = l.LayerDesc,
                                 LayerAttrTable = l.LayerAttrTable,
                                 LayerSpatialTable = l.LayerSpatialTable,
                                 LayerRefence = l.LayerRefence,
                                 LayerRefenceName = (re == null) ? "" : re.CodeName,
                                 CreateDT = Convert.ToDateTime(Convert.ToDateTime(l.CreateDT).ToString("yyyy-MM-ddTHH:mm:ss")),
                                 MaxY = l.MaxY,
                                 MinY = l.MinY,
                                 MinX = l.MinX,
                                 MaxX = l.MaxX,
                                 UploadStatus = l.UploadStatus,
                                 CreateBy = l.CreateBy,
                                 LayerDefaultStyle = l.LayerDefaultStyle,
                                 UploadFileType = l.UploadFileType,
                                 UploadFileName = l.UploadFileName
                             }).OrderByDescending(x => x.CreateDT);

                int count = query.Count();
                var result = query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

                IReadOnlyList<LayerContentDto> ir;
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        item.LayerTag = GetMultiTagNameByMapID(item.Id);
                    }
                    ir = result.MapTo<List<LayerContentDto>>();
                }
                else
                {
                    ir = new List<LayerContentDto>();
                }
                PagedResultOutput<LayerContentDto> outputList = new PagedResultOutput<LayerContentDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取标签对应的所有地图或图层ID字符串
        /// </summary>
        /// <param name="TagID"></param>
        /// <returns></returns>
        public string GetMultiLayerIDByTag(string TagID)
        {
            try
            {
                string layerID = "";
                var query = _ITagReleationRepository.GetAllList().Where(q => q.DataTagID == TagID).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        layerID += item.MapID + ",";
                    }
                }
                if (layerID.Length > 0)
                {
                    layerID = layerID.Substring(0, layerID.Length - 1);
                }
                return layerID;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取标签对应的所有地图或图层ID字符串
        /// </summary>
        /// <param name="TagID"></param>
        /// <returns></returns>
        public string GetMultiChildTypeByType(string typeID)
        {
            try
            {
                string multitypeid = typeID;
                var query = _IDataTypeRepository.GetAllList().Where(q => q.ParentID == typeID).ToList();
                if (query != null && query.Count > 0)
                {
                    foreach (var item in query)
                    {
                        var reustlt = GetMultiChildTypeByType(item.Id);
                        if (!string.IsNullOrWhiteSpace(reustlt))
                        {
                            multitypeid += "," + reustlt;
                        }
                        else
                        {
                            return typeID;
                        }
                    }
                }
                return multitypeid;
            }
            catch (Exception ex)
            {
                return typeID;
            }
        }

        /// <summary>
        /// 获取地图的所有标签描述
        /// </summary>
        /// <param name="mapLayerID"></param>
        /// <returns></returns>
        public string GetMultiTagNameByMapID(string mapLayerID)
        {
            try
            {
                string tagName = "";
                var query = _ITagReleationRepository.GetAllList().Where(q => q.MapID == mapLayerID).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var tag = _IDataTagRepository.Get(item.DataTagID);
                        tagName += tag.TagName + ",";
                    }
                }
                if (tagName.Length > 0)
                {
                    tagName = tagName.Substring(0, tagName.Length - 1);
                }
                return tagName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取样式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetDefaultStyleName(string id)
        {
            try
            {
                string styleName = "";
                var query = _IDataStyleRepository.Get(id);
                if (query != null)
                {
                    styleName = query.StyleName;
                }
                return styleName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 根据地图ID获取图层
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns></returns>
        public ListResultOutput<LayerContentDto> GetAllListByMapID(string mapID)
        {
            try
            {
                ListResultOutput<LayerContentDto> list;
                List<LayerContentDto> lisLcd = new List<LayerContentDto>();
                var mapRelation = _IMapReleationRepository.GetAll().Where(q => q.MapID == mapID).ToList();
                if (mapRelation != null && mapRelation.Count > 0)
                {
                    foreach (var item in mapRelation)
                    {
                        var layer = _ILayerContentRepository.Get(item.DataConfigID);
                        lisLcd.Add(layer.MapTo<LayerContentDto>());
                    }
                }
                list = new ListResultOutput<LayerContentDto>(lisLcd);
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        /// <param name="id">图层ID</param>
        /// <returns></returns>
        public async Task<LayerContentDto> GetDetailById(string id)
        {
             var db = (InfoEarthFrameDbContext)_ILayerContentRepository.GetDbContext();
            //var query = await _ILayerContentRepository.GetAsync(id);
             LayerContentEntity query = null;
             try
             {
                 query = _ILayerContentRepository.Get(id);
             }
             catch { 
             
             }
            if (query == null)
            {
                query = db.LayerContent.FirstOrDefault(p => p.LayerAttrTable == id);
            }
            var result = query.MapTo<LayerContentDto>();

            if (result != null)
            {
                try
                {
                    result.LayerTag = GetMultiTagNameByMapID(result.Id);
                    var dictCode = _IDicDataCodeRepository.Get(result.DataType);
                    result.DataTypeName = dictCode.CodeName;
                    var layerType = _IDataTypeRepository.Get(result.LayerType);
                    result.LayerTypeName = layerType.TypeName;
                    result.LayerDefaultStyleName = GetDefaultStyleName(result.LayerDefaultStyle);
                }
                catch (Exception ex)
                {
                    Abp.Logging.LogHelper.LogException(ex);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取图层属性表数据
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public DataTable GetLayerAttrTabledDetail(string layerID, int pageSize, int pageIndex,out int total)
        {
            string layerField = string.Empty;
            string querylayerField = string.Empty;
            try
            {
                var layer = _ILayerContentRepository.Get(layerID);
                var layerAttr = _ILayerFieldRepository.GetAllList().Where(q => q.LayerID == layerID).OrderBy(s => s.AttributeSort).ToList();
                foreach (var item in layerAttr)
                {
                    querylayerField += "\"" + item.AttributeName + "\",";
                    layerField += item.AttributeName + ",";
                }
                querylayerField += "guid,";
                layerField += "guid,";
                if (!string.IsNullOrEmpty(layerField))
                {
                    querylayerField = querylayerField.TrimEnd(',');
                    int fromNum = (pageIndex - 1) * pageSize;
                    //int endNum = pageIndex * pageSize;
                    //MySqlHelper mysql = new MySqlHelper();
                    PostgrelVectorHelper postgis = new PostgrelVectorHelper();
                    var sql = "select count(1) from " + layer.LayerAttrTable + "";
                    total = Convert.ToInt32(postgis.GetExecuteScalar(sql));
                     sql = "select " + querylayerField + " from " + layer.LayerAttrTable + "  ORDER BY SID DESC  limit " + pageSize + " OFFSET "+ fromNum;
                    //DataTable dt = mysql.ExecuteQuery(sql);
  
                    DataTable dt = postgis.getDataTable(sql);
                    return dt;
                   // strJson_dataTable = DataTableConvertJson.DataTable2Json("data", dt);
                   // strJson_result = Json.ToJson(new { LayerField = layerField, DataTableJson = strJson_dataTable });
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.LogException(ex);
                throw;
            }
            total = 0;
            return null;
        }

        [UnitOfWork(isTransactional: false)]
        public LayerImportValidDto ImportShpFileData(string layerID, string filePath, string user)
        {
            LayerImportValidDto liv = new LayerImportValidDto();
            bool success = true;
            string fileName = filePath;

            #region [屏弊实时导入功能]

            if (IsFileExist(filePath, ref filePath))
            {
                try
                {
                    ShpReader shpReader = new ShpReader(filePath);
                    // 检查矢量文件的有效性
                    if (!shpReader.IsValidDataSource())
                    {
                        liv.Status = false;
                        liv.Message = UtilityMessageConvert.Get("上传文件解析异常(上传的矢量文件无效)");
                        _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1202, 1323, null);
                    }
                    else
                    {
                        string csSrc = shpReader.GetSridWkt();
                        string SpatialRefence = System.Configuration.ConfigurationManager.AppSettings["SpatialRefence"].ToString();

                        if (!csSrc.Contains(SpatialRefence))
                        {
                            liv.Status = false;
                            liv.Message = UtilityMessageConvert.Get("上传文件解析异常(与系统默认坐标系不匹配)");
                            _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1202, 1324, null);
                        }
                        else
                        {
                            string tableName = string.Empty;
                            string layerType = string.Empty;
                            var layer = _ILayerContentRepository.Get(layerID);
                            if (layer.Id != null)
                            {
                                tableName = layer.LayerAttrTable;
                            }

                            var list = _ILayerFieldRepository.GetAll().Where(q => q.LayerID == layerID).ToList();
                            var layerAttr = list.Select(t => t.AttributeName).ToList();

                            //建表
                            Dictionary<string, string> attr = shpReader.GetAttributeType();
                            List<string> shpAttr = new List<string>();
                            Hashtable hashTable = new Hashtable();

                            foreach (KeyValuePair<string, string> item in attr)
                            {
                                if (layerAttr.Exists(t => t == item.Key))
                                {
                                    string lattr = layerAttr.Find(t => t == item.Key);
                                    shpAttr.Add(lattr);
                                    hashTable.Add(item.Key, item.Value);
                                }
                            }

                            GeoServerHelper geoServerHelper = new GeoServerHelper();

                            if (shpAttr.Count <= 0 || shpAttr.Count != layerAttr.Count || attr.Count != layerAttr.Count)
                            {
                                liv.Status = false;
                                liv.Message = UtilityMessageConvert.Get("上传文件解析异常(属性无匹配或只部分匹配)");
                                _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1202, 1325, null);
                            }
                            else
                            {
                                var listDataType = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "73160096-67a5-11e7-8eb2-005056bb1c7e").ToList();

                                #region [判断数据类型与长度]

                                List<AttributeModel> lstAttr = shpReader.GetOneFeatureAttributeModel(0);

                                foreach (AttributeModel item in lstAttr)
                                {
                                    #region [读取的shp属性]

                                    string dataType = item.AttributeType.ToString();
                                    item.AttributePrecision = (item.AttributePrecision == 11) ? 6 : item.AttributePrecision;

                                    switch (dataType)
                                    {
                                        case "OFTDate":
                                            item.AttributeTypeName = UtilityMessageConvert.Get("时间型");
                                            break;
                                        case "OFTDateTime":
                                            item.AttributeTypeName = UtilityMessageConvert.Get("时间型");
                                            break;
                                        case "OFTInteger":
                                            item.AttributeTypeName = UtilityMessageConvert.Get("短整型");
                                            break;
                                        case "OFTIntegerList":
                                            item.AttributeTypeName = UtilityMessageConvert.Get("长整型");
                                            break;
                                        case "OFTReal":
                                            if (item.AttributeWidth <= 13)
                                            {
                                                item.AttributeTypeName = UtilityMessageConvert.Get("单浮点型");
                                            }
                                            else
                                            {
                                                item.AttributeTypeName = UtilityMessageConvert.Get("双浮点型");
                                            }
                                            break;
                                        case "OFTString":
                                            item.AttributeTypeName = UtilityMessageConvert.Get("字符型");
                                            break;
                                        case "OFTTime":
                                            item.AttributeTypeName = UtilityMessageConvert.Get("时间型");
                                            break;
                                        default:
                                            item.AttributeTypeName = UtilityMessageConvert.Get("字符型");
                                            break;
                                    }
                                    Predicate<DicDataCodeEntity> dataTypeEntity = delegate (DicDataCodeEntity entity)
                                    {
                                        return entity.CodeName.Equals(item.AttributeTypeName);
                                    };

                                    DicDataCodeEntity dicEntity = listDataType.Find(dataTypeEntity);

                                    string AttributeType = string.Empty;
                                    if (dicEntity != null)
                                    {
                                        AttributeType = dicEntity.Id;
                                    }

                                    #endregion

                                    try
                                    {
                                        Predicate<LayerFieldEntity> layerFieldEntity = delegate (LayerFieldEntity entity)
                                        {
                                            return entity.AttributeName.Equals(item.AttributeName);
                                        };

                                        LayerFieldEntity layerField = list.Find(layerFieldEntity);

                                        if (layerField != null)
                                        {
                                            if (!layerField.AttributeLength.Equals(item.AttributeWidth.ToString()) || !layerField.AttributeType.Equals(AttributeType) || !layerField.AttributePrecision.Equals(item.AttributePrecision.ToString()))
                                            {
                                                success = false;
                                                liv.Status = false;
                                                liv.Message = UtilityMessageConvert.Get("图层属性信息匹配异常(长度＼精度＼数据类型)");
                                                _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1202, 1327, null);
                                            }
                                        }
                                        else
                                        {
                                            success = false;
                                            liv.Status = false;
                                            liv.Message = UtilityMessageConvert.Get("图层属性信息匹配异常");
                                            _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1202, 1328, null);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        success = false;
                                        liv.Status = false;
                                        liv.Message = UtilityMessageConvert.Get("图层属性信息匹配异常");
                                        _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1202, 1328, null);
                                    }
                                }

                                #endregion

                                if (success)
                                {
                                    #region [插入导入文件日志表]

                                    string logID = Guid.NewGuid().ToString();
                                    string strSQL = "insert into sdms_layer_readlog(Id,LayerID,ShpFileName,ReadStatus,CreateDT,CreateBy) values('" + logID + "','" + layerID + "','" + fileName + "','0','" + DateTime.Now.ToString() + "','" + user + "');";

                                    //MySqlHelper mysql = new MySqlHelper();

                                    //mysql.ExecuteNonQuery(strSQL);
                                    PostgrelVectorHelper postgis = new PostgrelVectorHelper();
                                    postgis.ExceuteSQL(strSQL, string.Empty);

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
                                    #endregion

                                    liv.Status = true;
                                    liv.Message = UtilityMessageConvert.Get("上传文件成功");
                                    _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1201, 1321, null);
                                }


                                /*
                                //PostgrelDBObject pdo = new PostgrelDBObject("gisdatamanage", "postgres", "123456", "192.168.1.63");
                                PostgrelVectorHelper dService = new PostgrelVectorHelper();
                                bool success = dService.ImportTable(tableName, shpReader, shpAttr, hashTable);
                                string msg = "";

                                if (success)
                                {
                                    //获取图层BBox
                                    Dictionary<string, double> bbox = new Dictionary<string, double>();

                                    success = ImportMysqlData(layer.LayerAttrTable, layer.LayerSpatialTable, shpReader, shpAttr, hashTable, ref bbox, ref msg);

                                    if (success)
                                    {
                                        var dic = _IDicDataCodeRepository.Get(layer.DataType);
                                        if (dic != null)
                                        {
                                            layerType = dic.CodeName;
                                        }
                                        var entitys = _layerFieldAppService.GetDetailByLayerID(layerID);

                                        GeoServerHelper geoServerHelper = new GeoServerHelper();
                                        //geoServerHelper.DeleteLayer(tableName);
                                        success = geoServerHelper.PublicLayer(tableName, layer.LayerName, entitys, layerType);

                                        if (success)
                                        {
                                            #region [更新状态和坐标]

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
                                                strbbox = layer.MinX.ToString()+","+layer.MinY.ToString() + ","+ layer.MaxX.ToString() +","+ layer.MaxY.ToString();
                                                string bboxStr = string.Format("{0},{1},{2},{3}", layer.MinX, layer.MinY, layer.MaxX, layer.MaxY);
                                                geoServerHelper.ModifyLayerBBox(tableName, bboxStr);
                                            }

                                            #region [生成缩略图]

                                            ThumbnailHelper tbh = new ThumbnailHelper();
                                            string imagePath = tbh.CreateThumbnail(layer.LayerAttrTable, "layer", strbbox);

                                            if(string.IsNullOrEmpty(imagePath))
                                            {
                                                liv.Status = false;
                                                liv.Message = "上传文件解析异常(下载缩略图)";
                                            }
                                            else
                                            {
                                                liv.Status = true;
                                                liv.Message = "上传文件解析数据保存成功";
                                            }

                                            #endregion

                                            #endregion
                                        }
                                        else
                                        {
                                            liv.Status = false;
                                            liv.Message = "上传文件解析异常(GeoServer地图发布异常)";
                                        }
                                    }
                                    else
                                    {
                                        liv.Status = false;
                                        liv.Message = "上传文件解析异常(MySQL:请查看SHP文件属性值类型或长度与系统建置是否匹配)";
                                    }
                                }
                                else
                                {
                                    liv.Status = false;
                                    liv.Message = "上传文件解析异常(postGIS:请查看SHP文件属性值类型或长度与系统建置是否匹配)";
                                }
                                 */
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    liv.Status = false;
                    liv.Message = UtilityMessageConvert.Get("上传文件解析异常");
                    Abp.Logging.LogHelper.LogException(ex);
                    _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1202, 1322, null);
                }
            }
            else
            {
                liv.Status = false;
                liv.Message = UtilityMessageConvert.Get("上传文件解析异常(上传必要文件不存在或不完整)");
                _IOperateLogAppService.WriteOperateLog(layerID, user, 1001, 1103, 1202, 1326, null);
            }

            return liv;


            #endregion
        }



        /// <summary>
        /// 判断上传文件是否完整
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsFileExist(string filePath, ref string filePhysicPath)
        {
            string name = filePath.Substring(0, filePath.LastIndexOf("."));

            string fileFullName = AppDomain.CurrentDomain.BaseDirectory + "\\" + "file";
            //string path = uploadFilePath + "/" + filePath.Substring(0, filePath.Length-4);

            filePhysicPath = Path.Combine(fileFullName, Path.GetFileName(filePath));
            var list = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == "b9ef9c7c-67b3-11e7-8eb2-005056bb1c7e").ToList();

            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    string fileName = name + "." + item.CodeValue;
                    string uploadFilePath = "";
                    if (!fileName.Contains(fileFullName))
                    {
                        uploadFilePath = Path.Combine(fileFullName, Path.GetFileName(fileName));
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
            //if (File.Exists(filePath))
            //{
            //MySqlHelper mysql = new MySqlHelper();
            //ShpReader shpReader = new ShpReader(filePath);
            // 检查矢量文件的有效性
            //if (!shpReader.IsValidDataSource())
            //{
            //    return false;
            //}
            int srid = shpReader.GetSrid();
            try
            {
                //获取图层BBox
                bbox = shpReader.GetLayerBBox();

                ////建表
                //Dictionary<string, string> attr = shpReader.GetAttributeType();
                //List<string> shpAttr = new List<string>();
                //Hashtable hashTable = new Hashtable();

                //foreach (KeyValuePair<string, string> item in attr)
                //{
                //    if (listAtrr.Exists(t => t.ToUpper() == item.Key.ToUpper()))
                //    {
                //        shpAttr.Add(item.Key);
                //        hashTable.Add(item.Key, item.Value);
                //    }
                //}

                //if (shpAttr.Count <= 0)
                //{
                //    return false;
                //}

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

                        Dictionary<string, string> attr1 = new Dictionary<string, string>();
                        if (pFeatureCount > 0)
                        {
                            attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
                        }
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

                        //string sqlInsert1 = String.Format("insert into `{0}`(`sid`,{1}) values('{2}',{3})", tableName1, colStr, sid1, valueStr);

                        //string sqlInsert2 = String.Format("insert into `{0}`(`sid`,`DataID`,`geom`) values('{1}','{2}',{3})", tableName2, sid2, sid1, geomStr);

                        //mysql.ExecuteNonQuery(sqlInsert1);
                        //mysql.ExecuteNonQuery(sqlInsert2);                                
                        sqlCInsert1 += string.Format("('{0}',{1}),", sid1, valueStr);
                        sqlCInsert2 += string.Format("('{0}','{1}',{2}),", sid2, sid1, geomStr);
                    }
                    catch (Exception ex)
                    {
                        Abp.Logging.LogHelper.LogException(ex);
                        //throw ex;
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
                Abp.Logging.LogHelper.LogException(ex);
                return false;
            }
            //}
            return true;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        public async Task<LayerContentDto> Insert(LayerContentInputDto input)
        {
            try
            {
                string name = "";
                input.Id = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(input.LayerName))
                {
                    ChineseConvert chn = new ChineseConvert();
                    name = chn.GetPinyinInitials(input.LayerName);
                    string lastIndex = (System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString());
                    int nameLength = (30 - lastIndex.Length) - 1;
                    input.LayerAttrTable = ((input.UploadFileType == "1") ? "v" : "g") + ((name.Length > nameLength) ? name.Substring(0, nameLength) : name) + lastIndex;
                    input.LayerSpatialTable = ((input.UploadFileType == "1") ? "v" : "g") + ((name.Length > (nameLength - 7)) ? name.Substring(0, (nameLength - 7)) : name) + lastIndex + "_s";
                }
                LayerContentEntity entity = new LayerContentEntity
                {
                    Id = input.Id,
                    LayerName = input.LayerName,
                    DataType = input.DataType,
                    LayerBBox = input.LayerBBox,
                    LayerType = input.LayerType,
                    LayerTag = input.LayerTag,
                    LayerDesc = name,
                    LayerAttrTable = input.LayerAttrTable,
                    LayerSpatialTable = input.LayerSpatialTable,
                    LayerRefence = input.LayerRefence,
                    MaxX = input.MaxX,
                    MinX = input.MinX,
                    MaxY = input.MaxY,
                    MinY = input.MinY,
                    CreateBy = input.CreateBy,
                    CreateDT = DateTime.Now,
                    UploadFileType = input.UploadFileType,
                    UploadFileName = input.UploadFileName
                };

                if (input.UploadFileType == "1")
                {
                    var query = await _ILayerContentRepository.InsertAsync(entity);
                    var result = entity.MapTo<LayerContentDto>();
                    _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateBy, 1001, 1101, 1201, 1313, "(" + input.LayerName + ")");
                    return result;
                }
                else if (input.UploadFileType == "2")
                {
                    string GeoServerIp = ConfigurationManager.AppSettings["GeoServerIp"];
                    string GeoServerPort = ConfigurationManager.AppSettings["GeoServerPort"];
                    string GeoWorkSpace = ConfigurationManager.AppSettings["GeoWorkSpace"];
                    string UploadFilePath = ConfigurationManager.AppSettings["UploadFilePath"];

                    string zipFilePath = Path.Combine(UploadFilePath, input.LayerAttrTable + ".zip");

                    GeoServer geoServer = new GeoServer(GeoServerIp, GeoServerPort);

                    var list = geoServer.GetCoverageStores(GeoWorkSpace);

                    //if (!input.UploadFileName.Contains("*"))
                    //{
                    //    geoServer.PutCoverageStore(GeoWorkSpace, input.LayerAttrTable, Path.Combine(UploadFilePath, input.UploadFileName));
                    //    geoServer.UpdateStoreTitle(GeoWorkSpace, input.LayerAttrTable, input.LayerAttrTable, input.LayerName);
                    //}
                    //else
                    //{
                    if (!Directory.Exists(Path.Combine(UploadFilePath, input.LayerAttrTable)))
                    {
                        Directory.CreateDirectory(Path.Combine(UploadFilePath, input.LayerAttrTable));
                    }
                    string tifPath = "";
                    foreach (string item in input.UploadFileName.Split('*'))
                    {
                        if (File.Exists(Path.Combine(UploadFilePath, item)))
                        {
                            string newName = input.LayerAttrTable + Path.GetExtension(item);
                            File.Copy(Path.Combine(UploadFilePath, item), Path.Combine(UploadFilePath, input.LayerAttrTable, newName), true);
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

                    bool success = zip.ZipDir(Path.Combine(UploadFilePath, input.LayerAttrTable), zipFilePath);
                    if (success)
                    {
                        geoServer.PutCoverageStore(GeoWorkSpace, input.LayerAttrTable, zipFilePath);
                        geoServer.UpdateStoreTitle(GeoWorkSpace, input.LayerAttrTable, input.LayerAttrTable, input.LayerName);
                        string srs = string.Empty;
                        var data = geoServer.GetConverageInfo(GeoWorkSpace, input.LayerAttrTable, input.LayerAttrTable);
                        if (data != null)
                        {
                            srs = data.Coverage.Srs;
                        }
                        //生成缩略图
                        ThumbnailHelper tbh = new ThumbnailHelper();

                        string bboxStr = string.Format("{0},{1},{2},{3}", entity.MinX, entity.MinY, entity.MaxX, entity.MaxY);
                        string imagePath = tbh.CreateThumbnail(input.LayerAttrTable, "layer", bboxStr, input.UploadFileType, srs);
                    }
                    else
                    {
                        throw new Exception();
                    }
                    //}
                    entity.UploadStatus = "1";
                    var query = await _ILayerContentRepository.InsertAsync(entity);
                    var result = entity.MapTo<LayerContentDto>();

                    _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateBy, 1001, 1101, 1201, 1315, "(" + input.LayerName + ")");

                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateBy, 1001, 1101, 1202, 1312, "(" + input.LayerName + ")");
                throw ex;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task<LayerContentDto> Update(LayerContentInputDto input)
        {
            try
            {
                LayerContentEntity entity = new LayerContentEntity
                {
                    Id = input.Id,
                    LayerName = input.LayerName,
                    DataType = input.DataType,
                    LayerBBox = input.LayerBBox,
                    LayerType = input.LayerType,
                    LayerTag = input.LayerTag,
                    LayerDesc = input.LayerDesc,
                    LayerAttrTable = input.LayerAttrTable,
                    LayerSpatialTable = input.LayerSpatialTable,
                    LayerRefence = input.LayerRefence,
                    MaxX = input.MaxX,
                    MinX = input.MinX,
                    MaxY = input.MaxY,
                    MinY = input.MinY
                    //,CreateDT = input.CreateDT
                };
                var query = await _ILayerContentRepository.UpdateAsync(entity);
                var result = entity.MapTo<LayerContentDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public async Task<LayerContentDto> UpdateStatus(string layerID)
        {
            try
            {
                var entity = _ILayerContentRepository.Get(layerID);

                if (entity != null)
                {
                    entity.UploadStatus = "1";
                }
                var query = await _ILayerContentRepository.UpdateAsync(entity);
                var result = entity.MapTo<LayerContentDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateBBox(string layerID, Dictionary<string, double> bbox)
        {
            try
            {
                var entity = _ILayerContentRepository.Get(layerID);
                if (entity != null && bbox.Count > 0)
                {
                    entity.MinX = (decimal)bbox["MinX"];
                    entity.MaxX = (decimal)bbox["MaxX"];
                    entity.MinY = (decimal)bbox["MinY"];
                    entity.MaxY = (decimal)bbox["MaxY"];
                }

                var query = await _ILayerContentRepository.UpdateAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置分类
        /// </summary>
        /// <param name="layerContentID">图层目录ＩＤ</param>
        /// <param name="DataTypeID">分类ＩＤ</param>
        /// <returns></returns>
        public async Task<LayerContentDto> UpdateDataType(string layerContentID, string layerTypeID)
        {
            try
            {
                var input = _ILayerContentRepository.Get(layerContentID);
                input.LayerType = layerTypeID;
                var query = await _ILayerContentRepository.UpdateAsync(input);
                var result = query.MapTo<LayerContentDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 设置图层的默认样式
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="styleID"></param>
        /// <returns></returns>
        public LayerContentDto UpdateDefaultStyle(string layerID, string styleID, string user, InfoEarthFrameDbContext db = null)
        {
            LayerContentEntity input = null;
            db = db ?? (InfoEarthFrameDbContext)_ILayerContentRepository.GetDbContext();
            try
            {
                input = db.LayerContent.FirstOrDefault(p => p.Id == layerID);
            }
            catch (Exception ex)
            { 
            
            }
            try
            {

                if (input == null)
                {
                  
                    input = db.LayerContent.FirstOrDefault(p => p.LayerAttrTable == layerID);
                    layerID=input.Id;
                }

                input.LayerDefaultStyle = (styleID.Contains("##")) ? "" : styleID;//清除默认样式匹配时，前端传两##

                var relation = db.MapReleation.FirstOrDefault(p => p.DataConfigID == layerID);
                if (relation != null)
                {
                    relation.DataStyleID = input.LayerDefaultStyle;
                    db.Entry(relation).State = EntityState.Modified;
                }

                db.Entry(input).State = EntityState.Modified;
                db.SaveChanges();
                var result = input.MapTo<LayerContentDto>();

                _IOperateLogAppService.WriteOperateLog(input.Id, user, 1001, 1102, 1201, 1351, null);
                return result;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(input.Id, user, 1001, 1102, 1202, 1352, null);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导出图层tiff文件
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public string OutputTifFileData(string layerID)
        {
            string tiffFilePath = string.Empty;
            try
            {
                var layer = _ILayerContentRepository.Get(layerID);
                if (layer != null)
                {
                    tiffFilePath = Path.Combine(ConfigHelper.UploadFilePath, string.Format("{0}.zip", layer.LayerAttrTable));
                    if (File.Exists(tiffFilePath))
                    {
                        return "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/" + tiffFilePath.Replace(AppDomain.CurrentDomain.BaseDirectory, string.Empty);
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 导出图层shp文件
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public string OutputShpFileData(string layerID)
        {
            string shpFilePath = "";
            try
            {
                var layer = _ILayerContentRepository.Get(layerID);
                if (layer != null)
                {
                    #region [图层属性取值]

                    string tableName = "public." + layer.LayerAttrTable;
                    string strSQL = string.Format("select {0},ST_Asewkt(geom) geom from {1} ", GetLayerFields(layer.Id), tableName);
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

                string layerTableName = layer.LayerName;

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
        /// 获取属性字符串
        /// </summary>
        /// <param name="layerId"></param>
        /// <returns></returns>
        private string GetLayerFields(string layerId)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
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

        private void FindReferenceFile(string path, ref string wktReference)
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

        private string ReadFile(string fileName)
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

        /// <summary>
        /// 获取图层业务表数据统计
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public int GetLayerDataCount(string layerID)
        {
            int count = 0;
            try
            {
                var entity = _ILayerContentRepository.Get(layerID);

                if (entity != null)
                {
                    string strSQL = "SELECT COUNT(1) num FROM " + entity.LayerAttrTable;

                    //MySqlHelper mySQL = new MySqlHelper();
                    //DataTable dt = mySQL.ExecuteQuery(strSQL);

                    PostgrelVectorHelper postgis = new PostgrelVectorHelper();
                    DataTable dt = postgis.getDataTable(strSQL);

                    if (dt.Rows.Count > 0)
                    {
                        count = int.Parse(dt.Rows[0]["num"].ToString());
                    }
                }

                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UnitOfWork(isTransactional: false)]
        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Clear(string id, string user)
        {
            try
            {
                var entity = GetDetailById(id);
                if (entity != null)
                {
                    #region [MySql]

                    string layerTable = entity.Result.LayerAttrTable;
                    string layerSpatial = entity.Result.LayerSpatialTable;
                    //执行清空逻辑
                    string strSql = "delete from @tableName1;delete from @tableName2";
                    strSql = strSql.Replace("@tableName1", layerTable);
                    strSql = strSql.Replace("@tableName2", layerSpatial);
                    //MySqlHelper mysql = new MySqlHelper();
                    //bool bSuccess = _ILayerFieldRepository.ExecuteSql(strSql, layerTable, layerSpatial);//执行超时错误
                    bool bSuccess = true;// mysql.ExecuteNonQuery(strSql);

                    #endregion

                    #region [PostGIS]


                    PostgrelVectorHelper pvh = new PostgrelVectorHelper();
                    string strPostSQL = string.Format("delete from {0}", entity.Result.LayerAttrTable);
                    bSuccess = pvh.ExceuteSQL(strPostSQL, layerTable);

                    #endregion

                    #region [更新BBox]

                    var layer = _ILayerContentRepository.Get(entity.Result.Id);
                    layer.MaxX = null;
                    layer.MaxY = null;
                    layer.MinX = null;
                    layer.MinY = null;
                    layer.UploadStatus = "0";

                    _ILayerContentRepository.Update(layer);

                    #endregion

                    #region [删除缩略图]

                    ThumbnailHelper thum = new ThumbnailHelper();
                    thum.DeleteThumbnail("layer", layerTable);

                    #endregion

                    _IOperateLogAppService.WriteOperateLog(id, user, 1001, 1104, 1201, 1331, null);
                    return bSuccess;
                }
                else
                {
                    _IOperateLogAppService.WriteOperateLog(id, user, 1001, 1104, 1202, 1332, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(id, user, 1001, 1104, 1202, 1332, null);
                return false;
            }
        }

        [UnitOfWork(IsDisabled = true)]
        /// <summary>
        /// 删除数据
        /// </summary>
        public bool Delete(string id, string user)
        {
            try
            {
                #region [删除标签关系数据]

                _ITagReleationRepository.Delete(x => x.MapID == id);

                #endregion

                #region[删除配置表数据]

                _ILayerFieldRepository.Delete(x => x.LayerID == id);

                #endregion

                _IOperateLogAppService.WriteOperateLog(id, user, 1001, 1105, 1201, 1341, null);
                return true;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(id, user, 1001, 1105, 1202, 1342, null);
                return false;
            }
        }

        [UnitOfWork(isTransactional: false)]
        public bool DeleteLayer(string id)
        {
            var entity = GetDetailById(id);
            if (entity != null)
            {
                #region [删除mysql图层业务数据表]

                string layerTable = entity.Result.LayerAttrTable;
                string layerSpatial = entity.Result.LayerSpatialTable;
                string strSql = "drop table @tableName1;drop table @tableName2";
                strSql = strSql.Replace("@tableName1", layerTable);
                strSql = strSql.Replace("@tableName2", layerSpatial);
                //MySqlHelper mysql = new MySqlHelper();
                ////bool bSucess = _ILayerFieldRepository.ExecuteSql(strSql, layerTable, layerSpatial);//执行超时错误
                bool bSucess = true;// mysql.ExecuteNonQuery(strSql);
                #endregion

                #region [删除postGIS图层业务数据表]

                if (bSucess)
                {
                    PostgrelVectorHelper pvh = new PostgrelVectorHelper();
                    string strPostSQL = string.Format("drop table {0}", entity.Result.LayerAttrTable);
                    bSucess = pvh.ExceuteSQL(strPostSQL, layerTable);
                }

                #endregion

                #region [GeoServer发布]

                GeoServerHelper geoHelp = new GeoServerHelper();

                geoHelp.DeleteLayer(entity.Result.LayerAttrTable);

                #endregion
            }

            _ILayerContentRepository.Delete(id);

            return true;
        }

        #endregion


        public IList<LayerContentOutputDto> GetLayers(string mainId,string layerName)
        {
            
            var db=(InfoEarthFrameDbContext)_ILayerContentRepository.GetDbContext();
            var sql="select * from (SELECT l.*,(case when u.\"UserName\" !='' then u.\"UserName\" else l.createby end) as CreateUser";
 sql+=" from sdms_layer l ";
 sql+=" left join sdms_user  u on u.\"Id\"=l.createby) t ";
 sql+=" where 1=1 and t.mainid='"+mainId+"' ";
 if (!string.IsNullOrEmpty(layerName))
 {
     sql += " and t.layername like '%"+layerName+"%' ";
 }
 sql+="order by t.createby desc";
var list= db.Database.SqlQuery<LayerContentOutputDto>(sql).ToList();


return list;
        }
    }
}