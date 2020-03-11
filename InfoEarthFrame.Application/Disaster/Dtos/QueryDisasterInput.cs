using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Application
{
    public class QueryDisasterInput :PageInfo
    {
        public string name { get; set; }


    }
}
