using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class KeyWords
    {
        [XmlElement]
        public string keyTyp { get; set; }
        [XmlElement]
        public string keyword { get; set; }
    }
}
