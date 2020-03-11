using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class PrcessStep
    {
        [XmlElement]
        public string stepDesc { get; set; }
    }
}
