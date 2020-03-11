using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.DataTagApp.Dtos;

namespace InfoEarthFrame.Application.DataTagApp
{
	public interface IDataTagAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<DataTagDto>> GetAllList();

        ListResultOutput<DataTagDto> GetAllListByDataType(string dataType);

		Task<DataTagOutputDto> GetDetailById(string id);


		Task<DataTagDto> Insert(DataTagInputDto input);

		Task<DataTagDto> Update(DataTagInputDto input);

		Task Delete(string id);
		#endregion
	}
}

