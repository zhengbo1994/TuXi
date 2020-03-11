using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using AutoMapper;
using InfoEarthFrame.Core;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace InfoEarthFrame.Application
{
    [AbpAuthorize]
    public class HazardsTypeAppService : ApplicationService, IHazardsTypeAppService
    {
        private readonly IHazardsTypeRepository _iHazardstypeRepository;
        public HazardsTypeAppService(IHazardsTypeRepository iHazardstypeRepository)
        {
            _iHazardstypeRepository = iHazardstypeRepository;
        }


        public async Task<ListResultOutput<HazardsTypeDto>> GetAllList()
        {

            try
            {
                //var result = await _iHazardstypeRepository.GetAllListAsync();
                var result =  _iHazardstypeRepository.GetAllList();
                var outputList = new ListResultOutput<HazardsTypeDto>(result.MapTo<List<HazardsTypeDto>>());
                return outputList;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

    }
}
