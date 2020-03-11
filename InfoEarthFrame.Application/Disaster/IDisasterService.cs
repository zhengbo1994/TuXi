using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{

    public interface IDisasterService : IApplicationService
    {
        #region  自动生成
        Task<ListResultOutput<DisasterDto>> GetAllList();

        Task<ListResultOutput<DisasterDto>> GetPageList(QueryDisasterInput queryDto);

        Task<PagedResultOutput<DisasterDto>> GetPageAndCountList(QueryDisasterInput queryDto);

        Task<int> GetCount(QueryDisasterInput queryDto);

        Task<DisasterOutput> GetDetailById(int Id);

        Task<DisasterDto> Insert(DisasterInput input);

        Task<DisasterDto> Update(DisasterInput input);

        Task DeleteById(int Id);

        Task<DisasterDto> InertDisaster();

        #endregion

        #region  自定义

        #endregion
    }
}
