using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.MapApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;
using Abp.Domain.Uow;
using InfoEarthFrame.Common;
using InfoEarthFrame.LayerFieldApp;
using InfoEarthFrame.Application.LayerContentApp.Dtos;
using System.Configuration;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using InfoEarthFrame.Application.SystemUserApp;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Application.OperateLogApp;

namespace InfoEarthFrame.Application.MapApp
{
    public class MapAppService : ApplicationService, IMapAppService
    {
        #region 变量
        private readonly IMapRepository _IMapRepository;
        private readonly IDicDataCodeRepository _IDicDataCodeRepository;
        private readonly IMapReleationRepository _IMapReleationRepository;
        private readonly ITagReleationRepository _ITagReleationRepository;
        private readonly IMapMetaDataRepository _IMapMetaDataRepository;
        private readonly IDataTagRepository _IDataTagRepository;
        private readonly IDataTypeRepository _IDataTypeRepository;
        private readonly ILayerContentRepository _ILayerContentRepository;
        private readonly ILayerFieldRepository _ILayerFieldRepository;
        private readonly ISystemUserRepository _ISystemUserRepository;
        private readonly SystemUserAppService _SystemUserApp;
        private readonly IOperateLogAppService _IOperateLogAppService;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public MapAppService(
            IMapRepository iMapRepository, 
            IDicDataCodeRepository iDicDataCodeRepository, 
            IMapReleationRepository iMapReleationRepository, 
            ITagReleationRepository iTagReleationRepository, 
            IMapMetaDataRepository iMapMetaDataRepository, 
            IDataTagRepository iDataTagRepository, 
            IDataTypeRepository iDataTypeRepository,
            ILayerContentRepository iLayerContentRepository, 
            ILayerFieldRepository iLayerFieldRepository,
            ISystemUserRepository iSystemUserRepository,
            IOperateLogAppService iOperateLogAppService)
        {
            _IMapRepository = iMapRepository;
            _IDicDataCodeRepository = iDicDataCodeRepository;
            _IMapReleationRepository = iMapReleationRepository;
            _ITagReleationRepository = iTagReleationRepository;
            _IMapMetaDataRepository = iMapMetaDataRepository;
            _IDataTagRepository = iDataTagRepository;
            _IDataTypeRepository = iDataTypeRepository;
            _ILayerContentRepository = iLayerContentRepository;
            _ILayerFieldRepository = iLayerFieldRepository;
            _ISystemUserRepository = iSystemUserRepository;
            _SystemUserApp = new SystemUserAppService(_ISystemUserRepository);
            _IOperateLogAppService = iOperateLogAppService;
        }

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public async Task<ListResultOutput<MapDto>> GetAllList()
        {
            try
            {
                //var query = await _IMapRepository.GetAllListAsync();
                var query =  _IMapRepository.GetAllList();
                var list = new ListResultOutput<MapDto>(query.MapTo<List<MapDto>>());
                if (list.Items.Count > 0)
                {
                    for (int i = 0; i < list.Items.Count; i++)
                    {
                        if (list.Items[i].MapScale != null)
                        {
                            list.Items[i].MapScaleName = GetDetailCodeName(list.Items[i].MapScale);
                        }
                        if (list.Items[i].SpatialRefence != null)
                        {
                            list.Items[i].SpatialRefenceName = GetDetailCodeName(list.Items[i].SpatialRefence);
                        }
                        if (list.Items[i].MapType != null)
                        {
                            try
                            {
                                var mapType = _IDataTypeRepository.Get(list.Items[i].MapType);
                                list.Items[i].MapType = mapType.TypeName;
                            }
                            catch (Exception ex)
                            {
                                list.Items[i].MapType = "";
                            }
                        }
                        list.Items[i].MapTag = GetMultiTagNameByMapID(list.Items[i].Id);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 地图统计列表
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

                List<MapEntity> mapList = null;
                if (userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    #region 超级用户
                    mapList = _IMapRepository.GetAllList();
                    #endregion
                }
                else
                {
                    List<SystemUserDto> userQuery = _SystemUserApp.GetUserDataByUserCodeAsync(userCode).Result;
                    if (userQuery != null && userQuery.Count > 0)
                    {
                        var userids = userQuery.Select(s => s.UserCode).ToArray();
                        mapList = _IMapRepository.GetAllList(s => userids.Contains(s.CreateUserId));
                    }
                }

                //var mapList = _IMapRepository.GetAllList();

                //if (!string.IsNullOrEmpty(userCode))
                //{
                //    List<string> listUserCode = _IOperateLogAppService.GetAllUserCodeByUserCode(userCode);
                //    if (listUserCode != null)
                //    {
                //        if (listUserCode.Count != 0)
                //        {
                //            mapList = mapList.Where(s => listUserCode.Contains(s.CreateUserId)).ToList();
                //        }
                //    }
                //}

                var dataTypeChild = _IDataTypeRepository.GetAllList();
                var dataTypeParent = _IDataTypeRepository.GetAllList();

                //类型父子级集合
                var result = (from m in mapList
                              join c in dataTypeChild on m.MapType equals c.Id
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
        /// 获取地图总数
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

                List<MapEntity> query = null;
                if (userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    #region 超级用户
                    //query = await _IMapRepository.GetAllListAsync();
                    query =  _IMapRepository.GetAllList();
                    #endregion
                }
                else
                {
                    List<SystemUserDto> userQuery = await _SystemUserApp.GetUserDataByUserCodeAsync(userCode);
                    if (userQuery != null && userQuery.Count > 0)
                    {
                        var userids = userQuery.Select(s => s.UserCode).ToArray();
                        //query = await _IMapRepository.GetAllListAsync(s => userids.Contains(s.CreateUserId));
                        query =  _IMapRepository.GetAllList(s => userids.Contains(s.CreateUserId));
                    }
                }

                //var query = await _IMapRepository.GetAllListAsync();

                //if (!string.IsNullOrEmpty(userCode))
                //{
                //    List<string> listUserCode = _IOperateLogAppService.GetAllUserCodeByUserCode(userCode);
                //    if (listUserCode != null)
                //    {
                //        if (listUserCode.Count != 0)
                //        {
                //            query = query.Where(s => listUserCode.Contains(s.CreateUserId)).ToList();
                //        }
                //    }
                //}

                return query.Count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取地图列表(通过标签，类型，名称)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ListResultOutput<MapDto> GetAllListByName(MapInputDto input)
        {
            ListResultOutput<MapDto> list;
            try
            {
                string mapType = input.MapType, tag = input.MapTag, name = input.MapName, mapID = "", type = "";

                if (!string.IsNullOrEmpty(tag))
                {
                    mapID = GetMultiLayerIDByTag(tag);
                }

                if (!string.IsNullOrEmpty(mapType))
                {
                    type = GetMultiChildTypeByType(mapType);
                }

                if (string.IsNullOrWhiteSpace(input.CreateUserId))
                {
                    return null;
                }

                var mapUserData = GetDataByUserCodeAsync(input.CreateUserId, name, type, mapID).Result;
                
                var listMapType = _IDataTypeRepository.GetAll();
                var listMapRefence = _IDicDataCodeRepository.GetAll();
                var listMapScale = _IDicDataCodeRepository.GetAll();

                var query = (from l in mapUserData
                             join t in listMapType on l.MapType equals t.Id into tt
                             from de in tt.DefaultIfEmpty()
                             join r in listMapRefence on l.SpatialRefence equals r.Id into rr
                             from re in rr.DefaultIfEmpty()
                             join dt in listMapScale on l.MapScale equals dt.Id into dtt
                             from ldt in dtt.DefaultIfEmpty()
                             select new MapDto
                             {
                                 Id = l.Id,
                                 MapName = l.MapName,
                                 MapEnName = l.MapEnName,
                                 MapBBox = l.MapBBox,
                                 MapPublishAddress = l.MapPublishAddress,
                                 MapStatus = l.MapStatus,
                                 MapDesc = l.MapDesc,
                                 MapType = (de == null) ? "" : de.TypeName,
                                 MapTag = l.MapTag,
                                 PublishDT = l.PublishDT,
                                 SortCode = l.SortCode,
                                 EnabledMark = l.EnabledMark,
                                 DeleteMark = l.DeleteMark,
                                 CreateUserId = l.CreateUserId,
                                 CreateUserName = l.CreateUserName,
                                 CreateDT = l.CreateDT,
                                 ModifyUserId = l.ModifyUserId,
                                 ModifyUserName = l.ModifyUserName,
                                 ModifyDate = l.ModifyDate,
                                 MapScale = l.MapScale,
                                 MapScaleName = (ldt == null) ? "" : ldt.CodeName,
                                 SpatialRefence = l.SpatialRefence,
                                 SpatialRefenceName = (re == null) ? "" : re.CodeName,
                                 MaxY = l.MaxY,
                                 MinY = l.MinY,
                                 MinX = l.MinX,
                                 MaxX = l.MaxX,
                                 MapLegend = l.MapLegend,
                                 MaxDegLat = "",
                                 MinDegLat = "",
                                 MinDegLon = "",
                                 MaxDegLon = ""
                             }).OrderByDescending(x => x.CreateDT);

                var result = query.ToList();

                list = new ListResultOutput<MapDto>(result.MapTo<List<MapDto>>());

                return list;
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
        private async Task<List<MapEntity>> GetDataByUserCodeAsync(string userCode, string name, string mapType, string tag)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return null;
                }

                List<MapEntity> query = null;
                if (userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    #region 超级用户
                    //query = await _IMapRepository.GetAllListAsync();
                    query =  _IMapRepository.GetAllList();
                    #endregion
                }
                else
                {
                    List<SystemUserDto> userQuery = await _SystemUserApp.GetUserDataByUserCodeAsync(userCode);
                    if (userQuery != null && userQuery.Count > 0)
                    {
                        var userids = userQuery.Select(s => s.UserCode).ToArray();
                        //query = await _IMapRepository.GetAllListAsync(s => userids.Contains(s.CreateUserId));
                        query =  _IMapRepository.GetAllList(s => userids.Contains(s.CreateUserId));
                    }
                }
                if (query != null && query.Any())
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        query = query.Where(s => !string.IsNullOrWhiteSpace(s.MapName) && s.MapName.Contains(name)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(mapType))
                    {
                        query = query.Where(s => !string.IsNullOrWhiteSpace(s.MapType) && mapType.Contains(s.MapType)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        query = query.Where(s => !string.IsNullOrWhiteSpace(s.Id) && tag.Contains(s.Id)).ToList();
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
        /// 获取地图列表(通过标签，类型，名称)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PagedResultOutput<MapDto> GetPageListByName(MapInputDto input, int PageSize, int PageIndex)
        {
            try
            {
                string mapType = input.MapType, tag = input.MapTag, name = input.MapName, mapID = "", type = "";

                if (!string.IsNullOrEmpty(tag))
                {
                    mapID = GetMultiLayerIDByTag(tag);
                }

                if (!string.IsNullOrEmpty(mapType))
                {
                    type = GetMultiChildTypeByType(mapType);
                }

                if (string.IsNullOrWhiteSpace(input.CreateUserId))
                {
                    return null;
                }

                var mapUserData = GetDataByUserCodeAsync(input.CreateUserId, name, type, mapID).Result;

                var listMapType = _IDataTypeRepository.GetAll();
                var listMapRefence = _IDicDataCodeRepository.GetAll();
                var listMapScale = _IDicDataCodeRepository.GetAll();

                var query = (from l in mapUserData
                             join t in listMapType on l.MapType equals t.Id into tt
                             from de in tt.DefaultIfEmpty()
                             join r in listMapRefence on l.SpatialRefence equals r.Id into rr
                             from re in rr.DefaultIfEmpty()
                             join dt in listMapScale on l.MapScale equals dt.Id into dtt
                             from ldt in dtt.DefaultIfEmpty()
                             select new MapDto
                             {
                                 Id = l.Id,
                                 MapName = l.MapName,
                                 MapEnName = l.MapEnName,
                                 MapBBox = l.MapBBox,
                                 MapPublishAddress = l.MapPublishAddress,
                                 MapStatus = l.MapStatus,
                                 MapDesc = l.MapDesc,
                                 MapType = (de == null) ? "" : de.TypeName,
                                 MapTag = l.MapTag,
                                 PublishDT = l.PublishDT,
                                 SortCode = l.SortCode,
                                 EnabledMark = l.EnabledMark,
                                 DeleteMark = l.DeleteMark,
                                 CreateUserId = l.CreateUserId,
                                 CreateUserName = l.CreateUserName,
                                 CreateDT = Convert.ToDateTime(Convert.ToDateTime(l.CreateDT).ToString("yyyy-MM-ddTHH:mm:ss")),
                                 ModifyUserId = l.ModifyUserId,
                                 ModifyUserName = l.ModifyUserName,
                                 ModifyDate = l.ModifyDate,
                                 MapScale = l.MapScale,
                                 MapScaleName = (ldt == null) ? "" : ldt.CodeName,
                                 SpatialRefence = l.SpatialRefence,
                                 SpatialRefenceName = (re == null) ? "" : re.CodeName,
                                 MaxY = l.MaxY,
                                 MinY = l.MinY,
                                 MinX = l.MinX,
                                 MaxX = l.MaxX,
                                 MapLegend = l.MapLegend,
                                 MaxDegLat = "",
                                 MinDegLat = "",
                                 MinDegLon = "",
                                 MaxDegLon = ""
                             }).OrderByDescending(x => x.CreateDT);
                int count = query.Count();
                var result = query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();



                IReadOnlyList<MapDto> ir = new List<MapDto>();
                if (result != null && result.Count > 0)
                {
                    foreach (var dto in result)
                    {
                        dto.MapTag = GetMultiTagNameByMapID(dto.Id);
                        dto.MaxDegLat = (dto.MaxY != null) ? ConvertBBox(Convert.ToDouble(dto.MaxY)) : "";
                        dto.MinDegLat = (dto.MinY != null) ? ConvertBBox(Convert.ToDouble(dto.MinY)) : "";
                        dto.MaxDegLon = (dto.MaxX != null) ? ConvertBBox(Convert.ToDouble(dto.MaxX)) : "";
                        dto.MinDegLon = (dto.MinX != null) ? ConvertBBox(Convert.ToDouble(dto.MinX)) : "";
                    }
                    ir = result.MapTo<List<MapDto>>();
                }

                PagedResultOutput<MapDto> outputList = new PagedResultOutput<MapDto>(count, ir);

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
                string mapID = "";
                var query = _ITagReleationRepository.GetAllList().Where(q => q.DataTagID == TagID).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        mapID += item.MapID + ",";
                    }
                }
                if (mapID.Length > 0)
                {
                    mapID = mapID.Substring(0, mapID.Length - 1);
                }
                return mapID;
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
                string multiTypeID = typeID + ",";
                var query = _IDataTypeRepository.GetAllList().Where(q => q.ParentID == typeID).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        multiTypeID += item.Id + ",";
                    }
                }
                if (multiTypeID.Length > 0)
                {
                    multiTypeID = multiTypeID.Substring(0, multiTypeID.Length - 1);
                }
                return multiTypeID;
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

        public string GetDetailCodeName(string id)
        {
            try
            {
                var entity = _IDicDataCodeRepository.Get(id);
                if (entity.CodeName != null)
                {
                    return entity.CodeName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string GetMapType(string id)
        {
            try
            {
                var entity = _IDataTypeRepository.Get(id);
                if (entity.Id != null)
                {
                    return entity.TypeName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 验证是否有重复地图名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetMapNameExist(string name)
        {
            try
            {
                var entity = _IMapRepository.GetAll().Where(q => q.MapName == name).ToList();
                if (entity.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        public async Task<MapOutputDto> GetDetailById(string id)
        {
            try
            {
                var query = await _IMapRepository.GetAsync(id);
                var result = query.MapTo<MapOutputDto>();
                if (result.MapScale != null)
                {
                    result.MapScaleName = GetDetailCodeName(result.MapScale);
                }
                if (result.SpatialRefence != null)
                {
                    result.SpatialRefenceName = GetDetailCodeName(result.SpatialRefence);
                }
                if (result.MapType != null)
                {
                    result.MapType = GetMapType(result.MapType);
                }
                result.MaxYName = (result.MaxY != null) ? ConvertBBox(Convert.ToDouble(result.MaxY)) : "";
                result.MinYName = (result.MinY != null) ? ConvertBBox(Convert.ToDouble(result.MinY)) : "";
                result.MaxXName = (result.MaxX != null) ? ConvertBBox(Convert.ToDouble(result.MaxX)) : "";
                result.MinXName = (result.MinX != null) ? ConvertBBox(Convert.ToDouble(result.MinX)) : "";
                result.MapTag = GetMultiTagNameByMapID(id);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string ConvertBBox(double bbox)
        {
            if (double.IsNaN(bbox))
            {
                return "";
            }
            try
            {
                double d = Math.Abs(bbox);
                double m = (60 * (d - Math.Floor(d)));
                double s = (60 * (m - Math.Floor(m)));
                return string.Format("{0}°{1}'{2:f0}\"", (int)d * Math.Sign(bbox), (int)m, s);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        public async Task<MapDto> Insert(MapInputDto input)
        {
            try
            {
                input.Id = Guid.NewGuid().ToString();
                MapEntity entity = new MapEntity
                {
                    Id = input.Id,
                    MapName = input.MapName,
                    MapBBox = input.MapBBox,
                    MapPublishAddress = input.MapPublishAddress,
                    MapStatus = input.MapStatus,
                    MapDesc = input.MapDesc,
                    MapType = input.MapType,
                    MapTag = input.MapTag,
                    PublishDT = input.PublishDT,
                    SortCode = input.SortCode,
                    EnabledMark = input.EnabledMark,
                    DeleteMark = input.DeleteMark,
                    CreateUserId = input.CreateUserId,
                    CreateUserName = input.CreateUserId,
                    CreateDT = DateTime.Now,
                    ModifyUserId = input.ModifyUserId,
                    ModifyUserName = input.ModifyUserName,
                    ModifyDate = input.ModifyDate,
                    MapScale = input.MapScale,
                    SpatialRefence = input.SpatialRefence,
                    MapLegend = input.MapLegend
                };
                ChineseConvert chn = new ChineseConvert();
                string mapEnName = chn.GetPinyinInitials(entity.MapName);
                string lastIndex = (System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString());
                entity.MapEnName = mapEnName + lastIndex;
                var query = await _IMapRepository.InsertAsync(entity);
                var result = entity.MapTo<MapDto>();

                _IOperateLogAppService.WriteOperateLog(input.Id, input.ModifyUserId, 1002, 1101, 1201, 1411, "(" + input.MapName + ")");
                return result;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(input.Id, input.ModifyUserId, 1002, 1101, 1202, 1412, "(" + input.MapName + ")");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task<MapDto> Update(MapInputDto input)
        {
            try
            {
                await UpdateMapBBox(input.Id);

                var entity = _IMapRepository.Get(input.Id);
                //entity.MapName = input.MapName;
                //entity.MapBBox = input.MapBBox;
                entity.MapPublishAddress = input.MapPublishAddress;
                entity.MapStatus = input.MapStatus;
                entity.MapDesc = input.MapDesc;
                entity.MapType = input.MapType;
                entity.MapTag = input.MapTag;
                entity.PublishDT = input.PublishDT;
                entity.SortCode = input.SortCode;
                entity.EnabledMark = input.EnabledMark;
                entity.DeleteMark = input.DeleteMark;
                entity.CreateUserId = input.CreateUserId;
                entity.CreateUserName = input.CreateUserId;
                //entity.CreateDT = input.CreateDT;
                entity.ModifyUserId = input.ModifyUserId;
                entity.ModifyUserName = input.ModifyUserName;
                entity.ModifyDate = DateTime.Now;
                entity.MapScale = input.MapScale;
                entity.SpatialRefence = input.SpatialRefence;
                var query = await _IMapRepository.UpdateAsync(entity);
                var result = entity.MapTo<MapDto>();

                _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateUserId, 1002, 1102, 1201, 1431, null);
                return result;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateUserId, 1002, 1102, 1202, 1432, null);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新地图图例
        /// </summary>
        /// <param name="mapID"></param>
        /// <param name="legendUrlPath"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMapLegend(string mapID, string legendUrlPath)
        {
            try
            {
                var entity = _IMapRepository.Get(mapID);
                entity.MapLegend = legendUrlPath;
                var query = await _IMapRepository.UpdateAsync(entity);
                var result = query.MapTo<MapDto>();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task<MapDto> UpdateDataType(string mapID, string dataTypeID)
        {
            try
            {
                var input = _IMapRepository.Get(mapID);
                input.MapType = dataTypeID;
                var query = await _IMapRepository.UpdateAsync(input);
                var result = query.MapTo<MapDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateMapBBox(string mapID)
        {
            try
            {
                var list = _IMapReleationRepository.GetAll().Where(q => q.MapID == mapID).ToList();
                var map = _IMapRepository.Get(mapID);
                if (map != null && list != null && list.Count > 0)
                {
                    Dictionary<string, decimal?> bbox = new Dictionary<string, decimal?>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var layer = _ILayerContentRepository.Get(list[i].DataConfigID);

                        if (i == 0)
                        {
                            bbox.Add("MaxX", layer.MaxX);
                            bbox.Add("MinX", layer.MinX);
                            bbox.Add("MaxY", layer.MaxY);
                            bbox.Add("MinY", layer.MinY);
                        }
                        else
                        {
                            if (layer.MaxX > bbox["MaxX"])
                            {
                                bbox["MaxX"] = layer.MaxX;
                            }
                            if (layer.MaxY > bbox["MaxY"])
                            {
                                bbox["MaxY"] = layer.MaxY;
                            }
                            if (layer.MinX < bbox["MinX"])
                            {
                                bbox["MinX"] = layer.MinX;
                            }
                            if (layer.MinY < bbox["MinY"])
                            {
                                bbox["MinY"] = layer.MinY;
                            }
                        }
                    }
                    map.MaxX = bbox["MaxX"];
                    map.MinX = bbox["MinX"];
                    map.MaxY = bbox["MaxY"];
                    map.MinY = bbox["MinY"];

                    await _IMapRepository.UpdateAsync(map);


                    GeoServerHelper geoHelp = new GeoServerHelper();
                    string isAutoCache = ConfigurationManager.AppSettings.Get("IsAutoCache");
                    if (!string.IsNullOrEmpty(isAutoCache) && isAutoCache == "1")
                    {
                        geoHelp.TerminatingTask(map.MapEnName);
                        geoHelp.TileMap(map.MapEnName);
                    }

                    string strBBox = map.MinX.ToString() + "," + map.MinY.ToString() + "," + map.MaxX.ToString() + "," + map.MaxY.ToString();
                    #region [生成缩略图]

                    ThumbnailHelper tbh = new ThumbnailHelper();
                    string imagePath = tbh.CreateThumbnail(map.MapEnName, "map", strBBox);

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
                return true;
            }
            catch (Exception ex)
            {
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

                #region [删除地图关系表]

                _IMapReleationRepository.Delete(x => x.MapID == id);
                #endregion


                #region [删除GeoServer地图]

                DeleteMap(id);

                #endregion

                #region [删除地图元数据]
                _IMapMetaDataRepository.Delete(x => x.MapID == id);
                #endregion

                _IMapRepository.Delete(id);

                _IOperateLogAppService.WriteOperateLog(id, user, 1002, 1105, 1201, 1421, null);
                return true;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(id, user, 1002, 1105, 1202, 1422, null);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 删除地图
        /// </summary>
        /// <param name="mapName">地图名称</param>
        /// <param name="workSpace">工作区名称</param>
        public bool DeleteMap(string mapid)
        {
            MapEntity map = _IMapRepository.Get(mapid);
            if (map != null)
            {
                GeoServerHelper geoHelp = new GeoServerHelper();
                geoHelp.EmptyTiles(map.MapEnName);
                return geoHelp.DeleteLayerGroup(map.MapEnName);
            }
            return true;
        }


        public async Task<bool> InsertMapInfo(MapInfoDto input)
        {
            try
            {
                #region [地图主信息]

                input.mapDto.Id = Guid.NewGuid().ToString();
                MapEntity entity = new MapEntity
                {
                    Id = input.mapDto.Id,
                    MapName = input.mapDto.MapName,
                    MapBBox = input.mapDto.MapBBox,
                    MapPublishAddress = input.mapDto.MapPublishAddress,
                    MapStatus = input.mapDto.MapStatus,
                    MapDesc = input.mapDto.MapDesc,
                    MapType = input.mapDto.MapType,
                    MapTag = input.mapDto.MapTag,
                    PublishDT = input.mapDto.PublishDT,
                    SortCode = input.mapDto.SortCode,
                    EnabledMark = input.mapDto.EnabledMark,
                    DeleteMark = input.mapDto.DeleteMark,
                    CreateUserId = input.mapDto.CreateUserId,
                    CreateUserName = input.mapDto.CreateUserName,
                    CreateDT = DateTime.Now,
                    ModifyUserId = input.mapDto.ModifyUserId,
                    ModifyUserName = input.mapDto.ModifyUserName,
                    ModifyDate = input.mapDto.ModifyDate,
                    MapScale = input.mapDto.MapScale,
                    SpatialRefence = input.mapDto.SpatialRefence,
                    MapLegend = input.mapDto.MapLegend
                };
                ChineseConvert chn = new ChineseConvert();
                entity.MapEnName = chn.GetPinyinInitials(entity.MapName);
                var query = await _IMapRepository.InsertAsync(entity);

                #endregion

                #region [地图图层关系]

                if (input.listMapReleationDto.Count > 0)
                {
                    foreach (var item in input.listMapReleationDto)
                    {
                        MapReleationEntity mapReleation = new MapReleationEntity
                        {
                            Id = Guid.NewGuid().ToString(),
                            MapID = input.mapDto.Id,
                            DataConfigID = item.DataConfigID,
                            DataStyleID = item.DataStyleID,
                            DataSort = item.DataSort,
                            ConfigDT = item.ConfigDT,
                            ModifyDT = item.ModifyDT
                        };
                        var result = await _IMapReleationRepository.InsertAsync(mapReleation);
                    }
                }

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 选择图层获取属性值
        /// </summary>
        /// <param name="layerID">图层ID</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="tolerance">容差</param>
        /// <returns></returns>
        public string GetLayerAttrByLayerPt(string layerID, float lon, float lat, float? tolerance = null)
        {
            string strSQL1 = String.Empty;
            string strSQL2 = String.Empty;
            strSQL1 = "select {0},ST_Asewkt(geom) geom from {1} where ST_Contains(geom,ST_PointFromText('POINT(" + lon + " " + lat + ")'))";
            if (!tolerance.HasValue)
            {
                tolerance = float.Parse(ConfigurationManager.AppSettings["GISTolerance"]);
            }
            strSQL2 = "select  {0},ST_Asewkt(geom) as geom from {1} where st_dwithin(ST_GeomFromText('POINT(" + lon + " " + lat + ")'),geom," + tolerance.Value + ")=true";
            return GetLayerAttrByLayerID(layerID, "Pt", strSQL1, strSQL2);
        }

        /// <summary>
        /// 根据中心点查询图层属性信息(总数)
        /// </summary>
        /// <param name="layerID">图层ID</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <returns></returns>
        public string GetLayerAttrByPtTolerane(string layerID, float lon, float lat, float distance)
        {
            string strSQL1 = String.Empty;
            string strSQL2 = String.Empty;

            strSQL2 = "select {0},ST_Asewkt(geom) geom from {1} where st_distance_sphere(ST_GeomFromText('POINT(" + lon + " " + lat + ")'),geom)<=" + distance;

            return GetLayerAttrByLayerID(layerID, "PtTolerane", strSQL1, strSQL2);
        }

        /// <summary>
        /// 根据矩形查询图层属性信息
        /// </summary>
        /// <param name="layerID">图层ID</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <returns></returns>
        public string GetLayerAttrByRect(string layerID, float minLon, float minLat, float maxLon, float maxLat)
        {
            string strSQL1 = String.Empty;
            string strSQL2 = String.Empty;

            strSQL2 = "select {0},ST_Asewkt(geom) geom from {1} where ST_Contains(ST_MakeEnvelope('" + minLon + "','" + minLat + "','" + maxLon + "','" + maxLat + "'),geom) or ST_Crosses(ST_MakeEnvelope('" + minLon + "','" + minLat + "','" + maxLon + "','" + maxLat + "'),geom)";
            return GetLayerAttrByLayerID(layerID, "Rect", strSQL1, strSQL2);
        }

        /// <summary>
        /// 框选查询结果
        /// </summary>
        /// <param name="layerID">图层ID</param>
        /// <param name="queryType">查询类型</param>
        /// <param name="strSQL1">SQL1</param>
        /// <param name="strSQL2">SQL2</param>
        /// <returns></returns>
        public string GetLayerAttrByLayerID(string layerID, string queryType, string strSQL1, string strSQL2)
        {
            try
            {
                bool success = false;
                string result = string.Empty;
                StringBuilder sb = new StringBuilder();
                var layerList = layerID.Split(',');
                if (layerList != null)
                {
                    result += "[";
                    int intIndex = 0;
                    foreach (var item in layerList)
                    {
                        string layerAttrTable = string.Empty, layerDataType = string.Empty, layerName = string.Empty;
                        var layer = _ILayerContentRepository.GetAll();
                        var dataType = _IDicDataCodeRepository.GetAll();

                        var query = from a in layer
                                    join b in dataType
                            on a.DataType equals b.Id
                                    where a.Id == item
                                    select new
                                    {
                                        a.LayerName,
                                        a.LayerAttrTable,
                                        b.CodeValue
                                    };

                        if (query != null)
                        {
                            if (query.Count() > 0)
                            {
                                foreach (var it in query)
                                {
                                    layerName = it.LayerName;
                                    layerAttrTable = it.LayerAttrTable;
                                    layerDataType = it.CodeValue;
                                }
                            }
                        }

                        var field = _ILayerFieldRepository.GetAllList(q => q.LayerID == item);

                        StringBuilder str = new StringBuilder();
                        StringBuilder att = new StringBuilder();

                        str.Append("\"");
                        str.Append("sid");
                        str.Append("\"");
                        str.Append(",");
                        foreach (var ite in field)
                        {
                            str.Append("\"");
                            str.Append(ite.AttributeName);
                            str.Append("\"");
                            str.Append(",");
                            att.Append(ite.AttributeName);
                            att.Append("#");
                        }
                        if (str.Length > 0)
                        {
                            str.Length = str.Length - 1;
                            att.Length = att.Length - 1;
                        }

                        #region [图层属性取值]

                        string tableName = "public." + layerAttrTable;
                        string strSQL = string.Empty;

                        if (layerDataType.ToUpper() == "POLYGON" && queryType == "Pt")
                        {
                            strSQL = string.Format(strSQL1, str.ToString(), tableName);
                        }
                        else
                        {
                            strSQL = string.Format(strSQL2, str.ToString(), tableName); ;
                        }

                        PostgrelVectorHelper actal = new PostgrelVectorHelper();

                        DataTable dt = actal.GetData(strSQL);

                        #endregion

                        #region [组JSON串]

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            success = true;
                            result += "{";
                            string lay = layerAttrTable;
                            string layName = layerName;
                            string layID = item;
                            result += "\"" + "layer" + "\":" + "\"" + lay + "\"," + "\"" + "layerName" + "\":" + "\"" + layName + "\"," + "\"" + "layerID" + "\":" + "\"" + layID + "\",";
                            result += "\"" + "attr" + "\":" + "\"" + att + "\",";
                            string data = JsonConvert.SerializeObject(dt);
                            result += "\"" + "data" + "\":" + data;
                            result += "},";
                        }
                        #endregion

                        intIndex++;
                    }

                    if (success)
                        result = result.Substring(0, result.Length - 1);
                    result += "]";
                }

                if (success)
                    return result;
                else
                    return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private string GetLayerFields(string layerId)
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
    }
}

