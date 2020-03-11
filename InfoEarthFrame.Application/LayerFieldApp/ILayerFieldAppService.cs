using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.LayerFieldApp.Dtos;

namespace InfoEarthFrame.Application.LayerFieldApp
{
    public interface ILayerFieldAppService : IApplicationService
    {
        #region 自动生成
        Task<ListResultOutput<LayerFieldDto>> GetAllList();

        Task<LayerFieldOutputDto> GetDetailById(string id);

        ListResultOutput<LayerFieldDto> GetDetailByLayerID(string layerID);

        ListResultOutput<LayerFieldDto> GetFieldDictByLayerID(string layerID);

        Task<LayerFieldDto> Insert(LayerFieldInputDto input);

        bool MultiInsert(List<LayerFieldInputDto> listInput);

        Task<LayerFieldDto> Update(LayerFieldComplexDto input);

        string GetMultiUpdateField(string layerID,string layerName,string user,List<LayerFieldComplexDto> listFieldInput);

        string GetNodeJSServerConfig();

        Task Delete(string id);

        Task DeleteByLayerID(string layerID);
        LayerFieldListDto GetLayerFieldByFileName(string shpFileName);

        #endregion

        bool CreateTable(string layerID);

        bool CheckCalComp(string InputCalComp);

        #region GeoServerRest
        /// <summary>
        /// 发布图层
        /// </summary>
        /// <param name="layerId">图层编号</param>
        /// <returns>true：发布成功，false：发布失败</returns>
        bool PublicLayer(string layerId, ListResultOutput<LayerFieldDto> listDto);
        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="workSpace">工作区名称</param>
        /// <param name="dataStore">数据存储名称</param>
        /// <param name="layer">图层名称</param>
        /// <returns>true：删除成功，false：删除失败</returns>
        bool DeleteLayer(string layer);

        #endregion
    }
}

