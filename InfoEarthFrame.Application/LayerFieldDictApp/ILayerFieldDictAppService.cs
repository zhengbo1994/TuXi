using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.LayerFieldDictApp.Dtos;

namespace InfoEarthFrame.Application.LayerFieldDictApp
{
	public interface ILayerFieldDictAppService : IApplicationService
	{
		#region 自动生成
		Task<ListResultOutput<LayerFieldDictDto>> GetAllList();

		Task<LayerFieldDictOutputDto> GetDetailById(string id);

		Task<LayerFieldDictDto> Insert(LayerFieldDictInputDto input);

        bool MultiInsert(List<LayerFieldDictInputDto> lstInput);

		Task<LayerFieldDictDto> Update(LayerFieldDictInputDto input);

        ListResultOutput<LayerFieldDictDto> GetFieldDictByLayerID(string layerID);

		Task Delete(string id);
		#endregion
	}
}

