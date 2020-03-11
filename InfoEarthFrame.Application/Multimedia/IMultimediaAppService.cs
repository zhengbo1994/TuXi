using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using AutoMapper;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public interface IMultimediaAppService : IApplicationService
    {
        List<MultimediaOutput> GetAllList(ModuleInfoInput input);
        Task<string> Insert(MultimediaTypeInput input);
        Task<bool> Update(MultimediaTypeInput input);
        Task<bool> Delete(string id);
    }
}
