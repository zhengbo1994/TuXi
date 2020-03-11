
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoEarthFrame.Common
{
    public class LayuiGridResult 
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

         [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("rows")]
        public LayuiGridData Rows { get;set; }
    }

    public class LayuiGridData
    {
          [JsonProperty("item")]
        public object Items { get; set; }
    }
}