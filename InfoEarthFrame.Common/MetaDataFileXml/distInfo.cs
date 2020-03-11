using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class distInfo
    {
        [XmlElement]
        public string onLineSrc { get; set; }
        [XmlElement]
        public string ordInstr { get; set; }
        private distorCont _distorCont { get; set; }
        [XmlElement]
        public distorCont distorCont
        {
            get
            {
                if (_distorCont == null)
                {
                    _distorCont = new distorCont();
                }
                return _distorCont;
            }
            set
            {
                _distorCont = value;
            }
        }

        private Medium _Medium { get; set; }
        [XmlElement]
        public Medium Medium
        {
            get
            {
                if (_Medium == null)
                {
                    _Medium = new Medium();
                }
                return _Medium;
            }
            set
            {
                _Medium = value;
            }
        }
    }
}
