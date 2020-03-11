using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class SCRefSys
    {
        [XmlElement]
        public string coodRSID { get; set; }
        [XmlElement]
        public string coodType { get; set; }
        [XmlElement]
        public string coodSID { get; set; }
        [XmlElement]
        public string parameter { get; set; }
    }
}
