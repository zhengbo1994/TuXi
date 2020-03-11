using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class rpCntInfo
    {
        [XmlElement]
        public string cntFaxNum { get; set; }
        [XmlElement]
        public string cntDelPnt { get; set; }
        [XmlElement]
        public string city { get; set; }
        [XmlElement]
        public string adminArea { get; set; }
        [XmlElement]
        public string country { get; set; }
        [XmlElement]
        public string postCode { get; set; }
        [XmlElement]
        public string cntOnlineRes { get; set; }
        [XmlElement]
        public string cntPhone { get; set; }
        [XmlElement]
        public string eMailAddr { get; set; }
    }
}
