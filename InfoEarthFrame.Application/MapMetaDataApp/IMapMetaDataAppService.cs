using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.MapMetaDataApp.Dtos;

namespace InfoEarthFrame.Application.MapMetaDataApp
{
	public interface IMapMetaDataAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<MapMetaDataDto>> GetAllList();

		Task<MapMetaDataOutputDto> GetDetailById(string id);

		Task<MapMetaDataDto> Insert(MapMetaDataInputDto input);

		Task<MapMetaDataDto> Update(MapMetaDataInputDto input);

		Task Delete(string id);

        Task DeleteByMapID(string mapID);
		#endregion
	}
}

