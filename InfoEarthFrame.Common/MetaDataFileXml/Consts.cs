using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class Consts
    {
        [XmlElement]
        public string accessConsts { get; set; }
        [XmlElement]
        public string useLimit { get; set; }
        [XmlElement(ElementName = "class")]
        public string class1 { get; set; }
        [XmlElement]
        public string useConsts { get; set; }
    }
}
