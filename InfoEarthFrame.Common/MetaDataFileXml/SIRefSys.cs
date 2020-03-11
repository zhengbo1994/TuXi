using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class SIRefSys
    {
        [XmlElement]
        public string refSysName { get; set; }
    }
}
