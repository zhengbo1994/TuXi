using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class MaintInfo
    {
        [XmlElement]
        public string upScpDesc { get; set; }
        [XmlElement]
        public string maintFreq { get; set; }
        private maintCont _maintCont { get; set; }
        [XmlElement]
        public maintCont maintCont {
            get
            {
                if (_maintCont == null)
                {
                    _maintCont = new maintCont();
                }
                return _maintCont;
            }
            set
            {
                _maintCont = value;
            }
        }
    }
}
