using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class Format
    {
        [XmlElement]
        public string fomatName { get; set; }
        [XmlElement]
        public string formatVer { get; set; }
    }
}
