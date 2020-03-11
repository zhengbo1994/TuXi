using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class VerExtent
    {
        [XmlElement]
        public string vertMaxVal { get; set; }
        [XmlElement]
        public string vertUoM { get; set; }
        [XmlElement]
        public string vertDatum { get; set; }
        [XmlElement]
        public string vertMinVal { get; set; }
    }
}
