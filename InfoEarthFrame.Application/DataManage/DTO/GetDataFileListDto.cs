using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Core;
using Newtonsoft.Json;

namespace InfoEarthFrame.Application
{
    public class GetDataFileListParamDto 
    {
        public string MainID { get; set; } // TBL_DATAMAIN 表 ID

        public string FolderName { get; set; }
    }

    public class GetDataFileListResultDto 
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string MainID { get; set; }
        public string FilePath { get; set; }
        public string FolderName { get; set; }

        [JsonProperty("name")]
        public string FileName { get; set; }
        public string Ext { get; set; }
        public DateTime? StorageTime { get; set; }

        public long FileLength { get; set; }

        [JsonProperty("pId")]
        public string ParentId { get; set; }

        public bool IsInGeoServer { get; set; }
       // public bool lay_is_open { get; set; }
    }
}
