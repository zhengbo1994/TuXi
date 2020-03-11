using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InfoEarthFrame.Application
{
    public class AreaOutput
    {
        [JsonProperty("children")]
        public List<AreaOutput> Children { get; set; }

        [XmlAttribute("Code")]
        [JsonProperty("id")]
        public string Code { get; set; }
 
        [XmlAttribute("Name")]
        [JsonProperty("label")]
        public string Label { get; set; }
    }

    public class Area
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Name")]
        public string Label { get; set; }
    }

    public class TownOutput
    {
        /// <summary>
        /// 树形显示的参数
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 树形显示的参数叶子节点
        /// </summary>
        public object[] Children { get; set; }
        public string DistrictCode { get; set; }
        
    }
}
