using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.OperateLogApp.Dtos;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Core.Entities;
using System.Collections.Generic;
using InfoEarthFrame.ShpFileReadLogApp.Dtos;

namespace InfoEarthFrame.ShpFileReadLogApp
{
    public interface IShpFileReadLogAppService:IApplicationService
    {
        Task<PagedResultOutput<ShpFileReadLogOutputDto>> GetGeologgerPageListByCondition(QueryShpFileReadLogInputParamDto input);
    }
}
