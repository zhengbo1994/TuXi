using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.DataTypeApp.Dtos;

namespace InfoEarthFrame.Application.DataTypeApp
{
	public interface IDataTypeAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<DataTypeDto>> GetAllList();

        ListResultOutput<DataTypeDto> GetAllListByDataType(string dataType);

		Task<DataTypeOutputDto> GetDetailById(string id);
        ListResultOutput<DataTypeDto> GetDetailByName(string name, string dataType);

		Task<DataTypeDto> Insert(DataTypeInputDto input);

		Task<DataTypeDto> Update(DataTypeInputDto input);

		Task Delete(string id);
		#endregion
	}
}

