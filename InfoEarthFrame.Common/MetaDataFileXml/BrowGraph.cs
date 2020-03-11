using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class BrowGraph
    {
        [XmlElement]
        public string bgFileName { get; set; }
        [XmlElement]
        public string bgFileDesc { get; set; }
    }
}
