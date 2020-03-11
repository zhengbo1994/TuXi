using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class ContDesc
    {
        [XmlElement]
        public string cntRasterImage { get; set; }
        [XmlElement]
        public string cntAttrDescFile { get; set; }
        [XmlElement]
        public string cntLayerName { get; set; }
        [XmlElement]
        public string cntFetTypes { get; set; }
        [XmlElement]
        public string cntAttrTpyList { get; set; }
    }
}
