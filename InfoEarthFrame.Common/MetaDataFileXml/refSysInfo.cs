using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class refSysInfo
    {
        private SIRefSys _SIRefSys;
        [XmlElement]
        public SIRefSys SIRefSys
        {
            get
            {
                if (_SIRefSys == null)
                {
                    _SIRefSys = new SIRefSys();
                }
                return _SIRefSys;
            }
            set
            {
                _SIRefSys = value;
            }
        }

        private SCRefSys _SCRefSys;
        [XmlElement]
        public SCRefSys SCRefSys
        {
            get
            {
                if (_SCRefSys == null)
                {
                    _SCRefSys = new SCRefSys();
                }
                return _SCRefSys;
            }
            set
            {
                _SCRefSys = value;
            }
        }

        private VerRS _VerRS;
        [XmlElement]
        public VerRS VerRS
        {
            get
            {
                if (_VerRS == null)
                {
                    _VerRS = new VerRS();
                }
                return _VerRS;
            }
            set
            {
                _VerRS = value;
            }
        }
    }
}
