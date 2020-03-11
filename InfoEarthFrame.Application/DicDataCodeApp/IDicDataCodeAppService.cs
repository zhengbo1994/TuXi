using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.DicDataCodeApp.Dtos;

namespace InfoEarthFrame.Application.DicDataCodeApp
{
	public interface IDicDataCodeAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<DicDataCodeDto>> GetAllList();

        ListResultOutput<DicDataCodeDto> GetDetailByConn(string typeID, string keyWord);

        ListResultOutput<DicDataCodeDto> GetDetailByTypeID(string typeID);

		Task<DicDataCodeOutputDto> GetDetailById(string id);

		Task<DicDataCodeDto> Insert(DicDataCodeInputDto input);

		Task<DicDataCodeDto> Update(DicDataCodeInputDto input);

		Task Delete(string id);
		#endregion
	}
}

