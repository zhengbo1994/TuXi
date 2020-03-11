using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.TagReleationApp.Dtos;
using InfoEarthFrame.TagReleationApp.Dtos;

namespace InfoEarthFrame.Application.TagReleationApp
{
	public interface ITagReleationAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<TagReleationDto>> GetAllList();

		Task<TagReleationOutputDto> GetDetailById(string id);

        string GetMultiTagNameByMapID(string mapLayerID);

		Task<TagReleationDto> Insert(TagReleationInputDto input);

        bool MultiInsert(TagDto tagDto);

		Task<TagReleationDto> Update(TagReleationInputDto input);

		Task Delete(string id);

        Task DeleteByMapID(string mapID);
		#endregion
	}
}

