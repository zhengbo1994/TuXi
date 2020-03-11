using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.LayerReadLogApp.Dtos;

namespace InfoEarthFrame.Application.LayerReadLogApp
{
	public interface ILayerReadLogAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<LayerReadLogDto>> GetAllList();

		Task<LayerReadLogOutputDto> GetDetailById(string id);

        ListResultOutput<LayerReadLogOutputDto> GetDetailByLayer(LayerReadLogOutputDto input);

        string GetAllListByPage(int PageSize, int PageIndex);

		Task<LayerReadLogDto> Insert(LayerReadLogInputDto input);

		Task<LayerReadLogDto> Update(LayerReadLogInputDto input);

		Task Delete(string id);

        Task<bool> UpdataMsgReadStatus(string user);

        string GetNodeJSServerConfig();
    
		#endregion
	}
}

