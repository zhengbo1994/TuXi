using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.LayerContentApp.Dtos;
using System.Data;
using InfoEarthFrame.EntityFramework;

namespace InfoEarthFrame.Application.LayerContentApp
{
	public interface ILayerContentAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<LayerContentDto>> GetAllList();

        DataTypeCountOutput GetAllCountByDataType(string userCode);
        PagedResultOutput<LayerContentDto> GetAllListStatus(LayerContentInputDto input, int PageSize, int PageIndex);

        Task<int> GetAllCount(string userCode);

        ListResultOutput<LayerContentDto> GetAllListByName(LayerContentInputDto input);

        PagedResultOutput<LayerContentDto> GetPageListByName(LayerContentInputDto input, int PageSize, int PageIndex);

        ListResultOutput<LayerContentDto> GetAllListByMapID(string mapID);

        Task<LayerContentDto> GetDetailById(string id);

        DataTable GetLayerAttrTabledDetail(string layerID, int pageSize, int pageIndex,out int total);

		Task<LayerContentDto> Insert(LayerContentInputDto input);

		Task<LayerContentDto> Update(LayerContentInputDto input);

        Task<LayerContentDto> UpdateStatus(string layerID);

        Task<LayerContentDto> UpdateDataType(string layerContentID, string layerTypeID);

        LayerContentDto UpdateDefaultStyle(string layerID, string styleID, string user,InfoEarthFrameDbContext db=null);
        LayerImportValidDto ImportShpFileData(string layerID, string filePath, string user);

        string OutputTifFileData(string layerID);

        string OutputShpFileData(string layerID);

        int GetLayerDataCount(string layerID);

        bool Clear(string id, string user);

		bool Delete(string id, string user);

        bool DeleteLayer(string id);
        #endregion

        IList<LayerContentOutputDto> GetLayers(string mainId,string layerName);


    }
}

