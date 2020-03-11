using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class VerRS
    {
        [XmlElement]
        public string verRSID { get; set; }
    }
}
