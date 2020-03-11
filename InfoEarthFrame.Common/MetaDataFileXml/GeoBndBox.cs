using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class GeoBndBox
    {
        [XmlElement]
        public string westBL { get; set; }
        [XmlElement]
        public string eastBL { get; set; }
        [XmlElement]
        public string northBL { get; set; }
        [XmlElement]
        public string southBL { get; set; }
    }
}
