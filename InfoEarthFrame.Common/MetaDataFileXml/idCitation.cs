using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class idCitation
    {
        [XmlElement]
        public string resTitle
        {
            get;
            set;
        }

        [XmlElement]
        public string resEd
        {
            get;
            set;
        }

        [XmlElement]
        public string resEdDate
        {
            get;
            set;
        }

        [XmlElement]
        public string Isbn
        {
            get;
            set;
        }

        [XmlElement]
        public string Issn
        {
            get;
            set;
        }

        [XmlElement]
        public string resDate
        {
            get;
            set;
        }



        [XmlElement]
        public string citRespParty
        {
            get;
            set;
        }

        [XmlElement]
        public string presForm
        {
            get; 
            set;
        }
    }
}
