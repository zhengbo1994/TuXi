using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class dqInfo
    {
        private Lineage _Lineage;
        [XmlElement]
        public Lineage Lineage
        {
            get
            {
                if (_Lineage == null)
                {
                    _Lineage = new Lineage();
                }
                return _Lineage;
            }
            set
            {
                _Lineage = value;
            }
        }

    }
}
