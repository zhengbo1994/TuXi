using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.OperateLogApp.Dtos;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Core.Entities;
using System.Collections.Generic;

namespace InfoEarthFrame.Application.OperateLogApp
{
    public interface IOperateLogAppService : IApplicationService
    {
        Task<PagedResultOutput<OperateLogOutputDto>> GetPageListByCondition(QueryOperateLogInputDto input, int pageSize, int pageIndex);

        Task<PagedResultOutput<OperateLogOutputDto>> GetPageListByLayerID(string layerID, int pageSize, int pageIndex);

        Task<PagedResultOutput<SystemUserDto>> GetPageUserByAreaCode(QueryUserInputDto input, int pageSize, int pageIndex);

        List<string> GetAllUserCodeByUserCode(string userCode);

        OperateLogOutputDto WriteOperateLog(string LayerID, string userCode, int systemFunc, int operateType, int result, int resultDescribe, string LayerName);

        Task<PagedResultOutput<OperateLogOutputDto>> GetPageListByParamCondition(QueryOperateLogInputParamDto input, int pageIndex, int pageSize);
    }
}
