using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Core;

namespace InfoEarthFrame.Application
{
    //todo:EntityDto
    public class GetDataMainListParamDto 
    {
       public  string GeologyMappingTypeID {get; set;}
        public  string Name {get; set;}
        public  DateTime? StartDate {get; set;}
        public  DateTime? EndDate {get; set;}
        public  int pageIndex {get; set;}
        public  int pageSize {get; set;}

        public string userId { get; set; }
    }

    public class GetDataMainListResultDto : EntityDto<string>
    {
        public List<GetDataMainListResult> RowList { get; set; }
        public int RowNum { get; set; }
    }

    public class GetDataMainListResult : EntityDto<string>
    {
        public string MappingTypeID { get; set; }
        public string MappingTypeName { get; set; }
        public int? Scale { get; set; }
        public string ScaleName { get; set; }
        public string MetaDataID { get; set; }
        public string ZipFileName { get; set; }
        public int Version { get; set; }
        public string VersionName { get; set; }
        public int VersionNo { get; set; }
        public DateTime? StorageTime { get; set; }

        public string Name { get; set; }

        public int? Status { get; set; }
    }
}
