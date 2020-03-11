using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.MapReleationApp.Dtos;

namespace InfoEarthFrame.Application.MapReleationApp
{
    public interface IMapReleationAppService : IApplicationService
    {
        #region 自动生成
        Task<ListResultOutput<MapReleationDto>> GetAllList();

        Task<MapReleationOutputDto> GetDetailById(string id);

        bool GetMapReleationByStyle(string styleID);

        ListResultOutput<MapReleationDto> GetAllListByMapId(string mapId);

        ListResultOutput<MapReleationDto> GetAllListBylayerID(string layerId);

        MapReleationDto Insert(MapReleationInputDto input);

        bool MultiInsert(List<MapReleationInputDto> listInput);

        Task<MapReleationDto> Update(MapReleationInputDto input, string user);

        Task<bool> MultiDelete(List<MapReleationInputDto> listInput);

        Task Delete(string id, string user);

        Task DeleteByMapID(string mapID);
        #endregion

        #region GeoServerRest
        /// <summary>
        /// 发布地图
        /// </summary>
        /// <param name="mapId">地图编号</param>
        /// <returns></returns>
        bool PublicMap(string mapId);

        /// <summary>
        /// 改变图层组样式
        /// </summary>
        /// <param name="mapid">地图id</param>
        /// <param name="layersId">图层id集合</param>
        /// <param name="stylesId">样式id集合</param>
        /// <returns></returns>
        bool ChangeStyle(string mapid, string layersId, string stylesId);

        bool ChangeStyleObject(StyleInputDto inputDto);

        /// <summary>
        /// 改变图层关系
        /// </summary>
        bool MultiUpdate(string mapId, List<MapReleationInputDto> listInput, string user);

        bool RefreshMap(string mapId, string user);

        /// <summary>
        /// 查询地图是否有切片任务
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        bool IsExistTilesTask(string mapId);
        #endregion
    }
}

