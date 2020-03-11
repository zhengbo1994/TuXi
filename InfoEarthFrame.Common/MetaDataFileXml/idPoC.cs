using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class idPoC
    {
        [XmlElement]
        public string rpIndName { get; set; }
        [XmlElement]
        public string rpOrgName { get; set; }
        [XmlElement]
        public string rpPosName { get; set; }
        [XmlElement]
        public string role { get; set; }
        [XmlElement]
        private rpCntInfo _rpCntInfo { get; set; }
        public rpCntInfo rpCntInfo {
            get
            {
                if (_rpCntInfo == null)
                {
                    _rpCntInfo = new rpCntInfo();
                }
                return _rpCntInfo;
            }
            set
            {
                _rpCntInfo = value;
            }
        }
    }
}
