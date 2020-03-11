using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class conInfo
    {
        private ContDesc _ContDesc { get; set; }
        [XmlElement]
        public ContDesc ContDesc
        {
            get
            {
                if (_ContDesc == null)
                {
                    _ContDesc = new ContDesc();
                }
                return _ContDesc;
            }
            set
            {
                _ContDesc = value;
            }
        }
    }
}
