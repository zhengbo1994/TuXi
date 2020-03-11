using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.MapReleationApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;
using InfoEarthFrame.GeoServerRest;
using System.Configuration;
using InfoEarthFrame.LayerFieldApp;
using InfoEarthFrame.Common;
using InfoEarthFrame.Application.OperateLogApp;
using InfoEarthFrame.EntityFramework;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using InfoEarthFrame.DataManage.DTO;
using System.Data.Entity;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Common;
using Newtonsoft.Json;
using InfoEarthFrame.Application.MapApp.Dtos;
using InfoEarthFrame.Application.OperateLogApp;
using InfoEarthFrame.Application.MapReleationApp;
using InfoEarthFrame.Core;
using System.IO;
using log4net;


namespace InfoEarthFrame.Application.MapReleationApp
{
    public class MapReleationAppService : IApplicationService, IMapReleationAppService
    {
        private readonly IMapReleationRepository _IMapReleationRepository;
        private readonly ILayerContentRepository _ILayerContentRepository;
        private readonly IDataStyleRepository _IDataStyleRepository;
        private readonly IMapRepository _IMapRepository;
        private readonly IOperateLogAppService _IOperateLogAppService;
        private readonly ILayerManagerAppService _layerManagerAppService;
        private readonly ILog _logger = LogManager.GetLogger(typeof(MapReleationAppService));
        /// <summary>
        /// 构造函数
        /// </summary>
        public MapReleationAppService(IMapReleationRepository iMapReleationRepository,
            ILayerContentRepository iLayerContentRepository,
            IDataStyleRepository iDataStyleRepository,
            IMapRepository iMapRepository,
            IOperateLogAppService iOperateLogAppService,
            ILayerManagerAppService layerManagerAppService)
        {
            _IMapReleationRepository = iMapReleationRepository;
            _ILayerContentRepository = iLayerContentRepository;
            _IDataStyleRepository = iDataStyleRepository;
            _IMapRepository = iMapRepository;
            _IOperateLogAppService = iOperateLogAppService;
            _layerManagerAppService = layerManagerAppService;
        }

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public async Task<ListResultOutput<MapReleationDto>> GetAllList()
        {
            try
            {
                //var query = await _IMapReleationRepository.GetAllListAsync();
                var query = _IMapReleationRepository.GetAllList();
                var list = new ListResultOutput<MapReleationDto>(query.MapTo<List<MapReleationDto>>());
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
        public async Task<MapReleationOutputDto> GetDetailById(string id)
        {
            try
            {
                var query = await _IMapReleationRepository.GetAsync(id);
                var result = query.MapTo<MapReleationOutputDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 判断是否有地图匹配
        /// </summary>
        /// <param name="styleID"></param>
        /// <returns></returns>
        public bool GetMapReleationByStyle(string styleID)
        {
            var list = _IMapReleationRepository.GetAll().Where(q => q.DataStyleID == styleID).ToList();

            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据MapID查询
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public ListResultOutput<MapReleationDto> GetAllListByMapId(string mapId)
        {
            try
            {
                var query = _IMapReleationRepository.GetAllList().Where(q => q.MapID == mapId).ToList().OrderBy(q => q.DataSort);
                var list = new ListResultOutput<MapReleationDto>(query.MapTo<List<MapReleationDto>>());
                if (list.Items.Count > 0)
                {
                    for (int i = 0; i < list.Items.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(list.Items[i].DataConfigID))
                        {
                            var layer = _ILayerContentRepository.Get(list.Items[i].DataConfigID);
                            list.Items[i].DataConfigName = (layer != null) ? layer.LayerName : "";
                        }
                        if (!string.IsNullOrWhiteSpace(list.Items[i].DataStyleID))
                        {
                            var style = _IDataStyleRepository.Get(list.Items[i].DataStyleID);
                            list.Items[i].DataStyleName = (style != null) ? style.StyleName : "";
                        }
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
        /// 根据MapID查询
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public ListResultOutput<MapReleationDto> GetAllListBylayerID(string layerId)
        {
            try
            {
                ListResultOutput<MapReleationDto> list = new ListResultOutput<MapReleationDto>();
                var query = _IMapReleationRepository.GetAllList().Where(q => q.DataConfigID == layerId);

                if (query.Count() > 0)
                {
                    list = new ListResultOutput<MapReleationDto>(query.MapTo<List<MapReleationDto>>());
                    if (list.Items.Count > 0)
                    {
                        for (int i = 0; i < list.Items.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(list.Items[i].MapID))
                            {
                                var map = _IMapRepository.Get(list.Items[i].MapID);
                                list.Items[i].MapName = (map != null) ? map.MapName : "";
                            }
                            if (!string.IsNullOrWhiteSpace(list.Items[i].DataConfigID))
                            {
                                var layer = _ILayerContentRepository.Get(list.Items[i].DataConfigID);
                                list.Items[i].DataConfigName = (layer != null) ? layer.LayerName : "";
                            }
                            if (!string.IsNullOrWhiteSpace(list.Items[i].DataStyleID))
                            {
                                var style = _IDataStyleRepository.Get(list.Items[i].DataStyleID);
                                list.Items[i].DataStyleName = (style != null) ? style.StyleName : "";
                            }
                        }
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
        /// 新增数据
        /// </summary>
        public MapReleationDto Insert(MapReleationInputDto input)
        {
            try
            {
                input.Id = Guid.NewGuid().ToString();
                MapReleationEntity entity = new MapReleationEntity
                {
                    Id = input.Id,
                    MapID = input.MapID,
                    DataConfigID = input.DataConfigID,
                    DataStyleID = input.DataStyleID,
                    DataSort = input.DataSort,
                    ConfigDT = input.ConfigDT,
                    ModifyDT = input.ModifyDT
                };
                var query = _IMapReleationRepository.Insert(entity);
                var result = entity.MapTo<MapReleationDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listInput"></param>
        /// <returns></returns>
        public bool MultiInsert(List<MapReleationInputDto> listInput)
        {
            try
            {
                string mapId = string.Empty;
                foreach (var input in listInput)
                {
                    MapReleationDto dto = Insert(input);
                    mapId = dto.MapID;
                }

                List<string> layers = listInput.Select(t => t.DataConfigID).ToList();

                UpdateMap(mapId, layers);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateMap(string mapId, List<string> layers)
        {
            if (layers != null && layers.Count > 0)
            {
                var map = _IMapRepository.Get(mapId);
                if (map != null)
                {
                    var list = _ILayerContentRepository.GetAll().Where(t => layers.Contains(t.Id)).ToList();
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

        public bool MultiUpdate(string mapId, List<MapReleationInputDto> listInput, string user)
        {
            try
            {
                _IMapReleationRepository.Delete(x => x.MapID == mapId);

                if (listInput != null)
                {
                    foreach (var input in listInput)
                    {
                        MapReleationDto dto = Insert(input);
                    }
                }

                List<string> layers = listInput.Select(t => t.DataConfigID).ToList();

                UpdateMap(mapId, layers);

                _IOperateLogAppService.WriteOperateLog(mapId, user, 1002, 1102, 1201, 1441, null);
                return true;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(mapId, user, 1002, 1102, 1202, 1442, null);
                return false;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task<MapReleationDto> Update(MapReleationInputDto input, string user)
        {
            try
            {
                //update by hu
                MapReleationEntity entity = new MapReleationEntity
                {
                    MapID = input.MapID,
                    DataConfigID = input.DataConfigID,
                    DataStyleID = input.DataStyleID,
                    DataSort = input.DataSort,
                    ConfigDT = input.ConfigDT,
                    ModifyDT = input.ModifyDT,
                    Id = input.Id
                };
                //var entity = _IMapReleationRepository.Get(input.Id);
                //if (entity != null)
                //{
                //    entity.MapID = input.MapID;
                //    entity.DataConfigID = input.DataConfigID;
                //    entity.DataStyleID = input.DataStyleID;
                //    //entity.DataSort = input.DataSort;
                //    entity.ConfigDT = input.ConfigDT;
                //    entity.ModifyDT = input.ModifyDT;
                //}
                var query = await _IMapReleationRepository.UpdateAsync(entity);
                var result = entity.MapTo<MapReleationDto>();

                _IOperateLogAppService.WriteOperateLog(input.MapID, user, 1002, 1102, 1201, 1431, null);
                return result;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(input.MapID, user, 1002, 1102, 1202, 1432, null);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task Delete(string id, string user)
        {
            try
            {
                await _IMapReleationRepository.DeleteAsync(id);
                _IOperateLogAppService.WriteOperateLog(id, user, 1002, 1105, 1201, 1421, null);
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(id, user, 1002, 1105, 1202, 1422, null);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> MultiDelete(List<MapReleationInputDto> listInput)
        {
            try
            {
                foreach (var input in listInput)
                {
                    await _IMapReleationRepository.DeleteAsync(q => q.DataConfigID == input.DataConfigID && q.MapID == input.MapID);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除地图关系表
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns></returns>
        public async Task DeleteByMapID(string mapID)
        {
            #region [删除地图关系表]

            await _IMapReleationRepository.DeleteAsync(q => q.MapID == mapID);

            #endregion
        }

        #endregion

        #region GeoServerRest

        class TempStyle
        {
            public string Id { get; set; }

            public string StyleName { get; set; }
        }
        /// <summary>
        /// 发布地图
        /// </summary>
        /// <param name="mapId">地图编号</param>
        /// <returns></returns>
        public bool PublicMap(string mapId)
        {
            var db = (InfoEarthFrameDbContext)_IMapReleationRepository.GetDbContext();
            var main = db.DataMain.FirstOrDefault(p => p.Id == mapId);
    
            var mapLayer = db.LayerManagers.FirstOrDefault(p => p.DataMainID == mapId);
            var query = (from a in db.MapReleation
                         join b in db.LayerContent on a.DataConfigID equals b.Id
                         join c in db.Map on a.MapID equals c.Id
                         where a.MapID == mapId
                         orderby a.DataSort
                         select new
                         {
                             a.Id,
                             a.DataStyleID,
                             b.LayerAttrTable,
                             c.MapEnName
                         }).ToList();


            var sql = "select distinct s.id,s.stylename from sdms_map_releation r join sdms_datastyle s on s.id=r.datastyleid";
    
            //var query2 = from a in _IMapReleationRepository.GetAll()
            //             join b in _IDataStyleRepository.GetAll() on a.DataStyleID equals b.Id
            //             select new
            //             {
            //                 a.Id,
            //                 b.StyleName
            //             };

            var list = db.Database.SqlQuery<TempStyle>(sql).ToList();
            List<string> targetLayers = new List<string>();
            List<string> styleLayers = new List<string>();
            string mapName = string.Empty;
            foreach (var item in query)
            {
                targetLayers.Add(item.LayerAttrTable);
                var style = list.Find(t => t.Id == item.DataStyleID);
                if (style != null)
                {
                    styleLayers.Add(style.StyleName);
                }
                else
                {
                    styleLayers.Add(string.Empty);
                }
                mapName = item.MapEnName;
            }
      
            var map = db.Map.FirstOrDefault(p => p.Id == mapId);
            GeoServerHelper geoHelp = new GeoServerHelper();

            var flag = geoHelp.AddLayerGroup(mapName, targetLayers, styleLayers);
     
            if (flag)
            {
                //往TBL_LAYERMANAGER插入数据
                if (mapLayer == null)
                {
                
                    db.LayerManagers.Add(new Core.Tbl_LayerManager
                    {
                        CreateTime = DateTime.Now,
                        DataMainID = mapId,
                        DATASERVERKEY = mapName,
                        Id = Guid.NewGuid().ToString(),
                        PICTYPE = "png",
                        SERVICETYPE = "GeoServer",
                        URL = ConfigContext.Current.DefaultConfig["geoserver:WMS"],
                        TEXT = main == null ? mapName : main.Name,
                    });
                }
                else
                {
         
                    mapLayer.CreateTime = DateTime.Now;
                 
                    mapLayer.DATASERVERKEY = mapName;
       
                    mapLayer.PICTYPE = "png";
            
                    mapLayer.SERVICETYPE = "GeoServer";
           
                    mapLayer.URL = ConfigContext.Current.DefaultConfig["geoserver:WMS"];
        
                    mapLayer.TEXT = main == null ? mapName : main.Name;
          
                    db.Entry(mapLayer).State = EntityState.Modified;
                }


     

            /*操作频繁，效率太低*/

            //string isAutoCache = ConfigurationManager.AppSettings.Get("IsAutoCache");
            //if (!string.IsNullOrEmpty(isAutoCache) && isAutoCache == "1")
            //{
            //    geoHelp.TerminatingTask(mapName);
            //    geoHelp.TileMap(map.MapEnName);
            //}

            #region [生成缩略图]
  
                string strBBox="";
                    strBBox = map.MinX.ToString() + "," + map.MinY.ToString() + "," + map.MaxX.ToString() + "," + map.MaxY.ToString();

 
            var thumbFilePath = GetThumbnial(map.MapEnName, strBBox);
      
            //上传缩略图到FTP
            var ftp = ConfigContext.Current.FtpConfig["Package"];
            if (ftp == null)
            {
                //FTP配置不存在
                throw new Exception("未找到[Name=Package]的FTP配置");
            }

            using (var client = new FtpHelper(ftp))
            {
                client.UploadFiles(new[] { thumbFilePath }, "Package\\" + mapId + "\\缩略图");
            }

  
                var images = db.DataManageFile.Where(p => p.MainID == mapId && p.FolderName == "缩略图").ToList();
                images.ForEach(p =>
                {
                    db.DataManageFile.Remove(p);
                });
     
                var fileName = Path.GetFileName(thumbFilePath);
                var file = new DataManageFile
                {
                    MainID = mapId,
                    Id = Guid.NewGuid().ToString(),
                    FolderName = "缩略图",
                    FileName = fileName,
                    Ext = Path.GetExtension(thumbFilePath),
                    FileSize = (int)new FileInfo(thumbFilePath).Length,
                    FileData = "Package\\" + mapId + "\\缩略图\\" + fileName,
                    StorageTime = DateTime.Now
                };
                db.DataManageFile.Add(file);

  
                main.ImagePath = file.FileData;
                db.Entry(main).State = EntityState.Modified;
                db.SaveChanges();
            }
            #endregion

            return true;

        }

        public string GetThumbnial(string name, string strBBox)
        {

            #region [生成缩略图]

            ThumbnailHelper tbh = new ThumbnailHelper();
            return tbh.CreateThumbnail(name, "map", strBBox);


            #endregion
        }

        /// <summary>
        /// 查询地图是否有切片任务
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public bool IsExistTilesTask(string mapId)
        {
            var query = _IMapRepository.FirstOrDefault(m => m.Id == mapId);
            if (query != null)
            {
                GeoServerHelper geoHelp = new GeoServerHelper();
                if (geoHelp.IsExistTilesTask(query.MapEnName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 刷新地图
        /// </summary>
        /// <param name="mapId">地图编号</param>
        /// <returns></returns>
        public bool RefreshMap(string mapId, string user)
        {
            var query = from a in _IMapReleationRepository.GetAll()
                        join b in _ILayerContentRepository.GetAll() on a.DataConfigID equals b.Id
                        join c in _IMapRepository.GetAll() on a.MapID equals c.Id
                        where a.MapID == mapId
                        orderby a.DataSort
                        select new
                        {
                            a.Id,
                            b.LayerAttrTable,
                            c.MapEnName
                        };

            var query2 = from a in _IMapReleationRepository.GetAll()
                         join b in _IDataStyleRepository.GetAll() on a.DataStyleID equals b.Id
                         select new
                         {
                             a.Id,
                             b.StyleName
                         };
            var list = query2.ToList();
            List<string> targetLayers = new List<string>();
            List<string> styleLayers = new List<string>();
            string mapName = string.Empty;
            foreach (var item in query)
            {
                targetLayers.Add(item.LayerAttrTable);
                var style = list.Find(t => t.Id == item.Id);
                if (style != null)
                {
                    styleLayers.Add(style.StyleName);
                }
                else
                {
                    styleLayers.Add(string.Empty);
                }
                mapName = item.MapEnName;
            }

            var map = _IMapRepository.Get(mapId);
            GeoServerHelper geoHelp = new GeoServerHelper();

            geoHelp.AddLayerGroup(mapName, targetLayers, styleLayers);
            geoHelp.TerminatingTask(mapName);
            geoHelp.TileMap(map.MapEnName);

            #region [生成缩略图]

            string strBBox = map.MinX.ToString() + "," + map.MinY.ToString() + "," + map.MaxX.ToString() + "," + map.MaxY.ToString();
            GetThumbnial(map.MapEnName, strBBox);

            #endregion

            _IOperateLogAppService.WriteOperateLog(mapId, user, 1002, 1106, 1201, 1451, null);
            return true;

        }

        /// <summary>
        /// 改变图层组样式
        /// </summary>
        /// <param name="mapid">地图id</param>
        /// <param name="layersId">图层id集合</param>
        /// <param name="stylesId">样式id集合</param>
        /// <returns></returns>
        public bool ChangeStyle(string mapid, string layerStr, string styleStr)
        {
            List<string> layersId = layerStr.Split(',').ToList();
            List<string> stylesId = styleStr.Split(',').ToList();

            GeoServerHelper geoHelp = new GeoServerHelper();
            MapEntity map = _IMapRepository.Get(mapid);
            if (map != null && layersId != null && stylesId != null && layersId.Count == stylesId.Count)
            {
                var list1 = _ILayerContentRepository.GetAll().Where(t => layersId.Contains(t.Id)).Select(t => new { id = t.Id, name = t.LayerAttrTable }).ToList();
                var list2 = _IDataStyleRepository.GetAll().Where(t => stylesId.Contains(t.Id)).Select(t => new { id = t.Id, name = t.StyleName }).ToList();

                Dictionary<string, string> dic = new Dictionary<string, string>();
                for (int i = 0; i < layersId.Count; i++)
                {
                    string lid = layersId[i];
                    string sid = stylesId[i];

                    var l1 = list1.Find(t => t.id == lid);
                    var s1 = list2.Find(t => t.id == sid);
                    if (l1 != null)
                    {
                        string sName = string.Empty;
                        if (s1 != null)
                        {
                            sName = s1.name;
                        }
                        if (!dic.ContainsKey(l1.name))
                        {
                            dic.Add(l1.name, sName);
                        }
                    }

                }
                geoHelp.ModifyLayerGroup(map.MapEnName, dic.Keys, dic.Values);

                #region [生成缩略图]

                string strBBox = map.MinX.ToString() + "," + map.MinY.ToString() + "," + map.MaxX.ToString() + "," + map.MaxY.ToString();
                GetThumbnial(map.MapEnName, strBBox);

                #endregion
            }

            return true;
        }

        /// <summary>
        /// 改变图层组样式
        /// </summary>
        /// <returns></returns>
        public bool ChangeStyleObject(StyleInputDto inputDto)
        {
            string mapid = inputDto.MapId;
            string layerStr = inputDto.LayerStr;
            string styleStr = inputDto.StyleStr;

            List<string> layersId = layerStr.Split(',').ToList();
            List<string> stylesId = styleStr.Split(',').ToList();

            GeoServerHelper geoHelp = new GeoServerHelper();
            MapEntity map = _IMapRepository.Get(mapid);
            if (map != null && layersId != null && stylesId != null && layersId.Count == stylesId.Count)
            {
                var list1 = _ILayerContentRepository.GetAll().Where(t => layersId.Contains(t.Id)).Select(t => new { id = t.Id, name = t.LayerAttrTable }).ToList();
                var list2 = _IDataStyleRepository.GetAll().Where(t => stylesId.Contains(t.Id)).Select(t => new { id = t.Id, name = t.StyleName }).ToList();

                Dictionary<string, string> dic = new Dictionary<string, string>();
                for (int i = 0; i < layersId.Count; i++)
                {
                    string lid = layersId[i];
                    string sid = stylesId[i];

                    var l1 = list1.Find(t => t.id == lid);
                    var s1 = list2.Find(t => t.id == sid);
                    if (l1 != null)
                    {
                        string sName = string.Empty;
                        if (s1 != null)
                        {
                            sName = s1.name;
                        }
                        if (!dic.ContainsKey(l1.name))
                        {
                            dic.Add(l1.name, sName);
                        }
                    }

                }
                geoHelp.ModifyLayerGroup(map.MapEnName, dic.Keys, dic.Values);

                #region [生成缩略图]

                string strBBox = map.MinX.ToString() + "," + map.MinY.ToString() + "," + map.MaxX.ToString() + "," + map.MaxY.ToString();
                GetThumbnial(map.MapEnName, strBBox);

                #endregion
            }

            return true;
        }

        #endregion
    }
}

