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
    public interface IKneeCoordinateAppService : IApplicationService
    {
        KneeCoordinateOutput GeoCoordinateList(KneeCoordinateInput input);
    }
}
