using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Core;

namespace InfoEarthFrame.Application
{
    public class UploadFileParamDto : EntityDto<string>
    {
        public string GeologyMappingTypeID { get; set; }
        public string ZipFilePath { get; set; }
        public int? Scale { get; set; }
        public int? Version { get; set; }
    }
}
