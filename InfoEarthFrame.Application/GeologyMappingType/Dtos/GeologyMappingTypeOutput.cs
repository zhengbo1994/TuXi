using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace InfoEarthFrame.Application
{    
    public class GeologyMappingTypeOutput 
    {
         public string Pid { get; set; }

         [JsonProperty("id")]
         public string Id { get; set; }

         [JsonProperty("label")]
         public string Label { get; set; }
         public string Paths { get; set; }
         public int Sn { get; set; }

          [JsonProperty("children")]
         public List<GeologyMappingTypeOutput> Children { get; set; }                
    }
}
