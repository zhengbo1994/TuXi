using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class Lineage
    {
        private PrcessStep _PrcessStep;
        [XmlElement]
        public PrcessStep PrcessStep
        {
            get
            {
                if (_PrcessStep == null)
                {
                    _PrcessStep = new PrcessStep();
                }
                return _PrcessStep;
            }
            set
            {
                _PrcessStep = value;
            }
        }
    }
}
