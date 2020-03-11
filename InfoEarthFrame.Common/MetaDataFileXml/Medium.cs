using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class Medium
    {
        [XmlElement]
        public string medNote { get; set; }
        [XmlElement]
        public string medName { get; set; }
    }
}