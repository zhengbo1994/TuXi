using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public interface IHazardsTypeAppService : IApplicationService
    {
        Task<ListResultOutput<HazardsTypeDto>> GetAllList();
    }
}
