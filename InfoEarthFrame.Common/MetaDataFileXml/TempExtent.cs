using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class TempExtent
    {
        [XmlElement]
        public string begin { get; set; }
        [XmlElement]
        public string end { get; set; }
    }
}
