using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.MapApp.Dtos;
using InfoEarthFrame.Application.LayerContentApp.Dtos;

namespace InfoEarthFrame.Application.MapApp
{
	public interface IMapAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<MapDto>> GetAllList();
        DataTypeCountOutput GetAllCountByDataType(string userCode);

        Task<int> GetAllCount(string userCode);

        ListResultOutput<MapDto> GetAllListByName(MapInputDto input);

        PagedResultOutput<MapDto> GetPageListByName(MapInputDto input, int PageSize, int PageIndex);

		Task<MapOutputDto> GetDetailById(string id);

        bool GetMapNameExist(string name);

		Task<MapDto> Insert(MapInputDto input);

		Task<MapDto> Update(MapInputDto input);

        Task<bool> UpdateMapLegend(string mapID, string legendUrlPath);

        Task<MapDto> UpdateDataType(string mapID, string dataTypeID);

        Task<bool> UpdateMapBBox(string mapID);

        bool Delete(string id, string user);

        string GetLayerAttrByLayerPt(string layerID, float lon, float lat, float? tolerance = null);

        string GetLayerAttrByPtTolerane(string layerID, float lon, float lat, float distance);

        string GetLayerAttrByRect(string layerID, float minLon, float minLat, float maxLon, float maxLat);

		#endregion
	}
}

