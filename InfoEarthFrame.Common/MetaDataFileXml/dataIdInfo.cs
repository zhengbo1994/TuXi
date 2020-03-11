using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    public class dataIdInfo
    {
        [XmlElement]
        public string dataChar
        {
            get;
            set;
        }

        [XmlElement]
        public string idAbs
        {
            get;
            set;
        }

        [XmlElement]
        public string idPurp
        {
            get;
            set;
        }

        [XmlElement]
        public string idStatus
        {
            get;
            set;
        }

        [XmlElement]
        public string imageID
        {
            get;
            set;
        }

        [XmlElement]
        public string dataLangCode
        {
            get;
            set;
        }

        [XmlElement]
        public string dataRpType
        {
            get;
            set;
        }

        [XmlElement]
        public string dataScale
        {
            get;
            set;
        }

        [XmlElement]
        public string tpCat
        {
            get;
            set;
        }

        [XmlElement]
        public string geoId
        {
            get;
            set;
        }

        private idCitation _idCitation;
        [XmlElement]
        public idCitation idCitation
        {
            get
            {
                if (_idCitation == null)
                {
                    _idCitation = new idCitation();
                }
                return _idCitation;
            }
            set
            {
                _idCitation = value;
            }
        }

        private GeoBndBox _GeoBndBox;
        [XmlElement]
        public GeoBndBox GeoBndBox
        {
            get
            {
                if (_GeoBndBox == null)
                {
                    _GeoBndBox = new GeoBndBox();
                }
                return _GeoBndBox;
            }
            set
            {
                _GeoBndBox = value;
            }
        }

        private TempExtent _TempExtent;
        [XmlElement]
        public TempExtent TempExtent
        {
            get
            {
                if (_TempExtent == null)
                {
                    _TempExtent = new TempExtent();
                }
                return _TempExtent;
            }
            set
            {
                _TempExtent = value;
            }
        }

        private VerExtent _VerExtent;
        [XmlElement]
        public VerExtent VerExtent
        {
            get
            {
                if (_VerExtent == null)
                {
                    _VerExtent = new VerExtent();
                }
                return _VerExtent;
            }
            set
            {
                _VerExtent = value;
            }
        }

        private idPoC _idPoC;
        [XmlElement]
        public idPoC idPoC
        {
            get
            {
                if (_idPoC == null)
                {
                    _idPoC = new idPoC();
                }
                return _idPoC;
            }
            set
            {
                _idPoC = value;
            }
        }

        //private KeyWords _KeyWords;
        //[XmlElement]
        //public KeyWords KeyWords
        //{
        //    get
        //    {
        //        if (_KeyWords == null)
        //        {
        //            _KeyWords = new KeyWords();
        //        }
        //        return _KeyWords;
        //    }
        //    set
        //    {
        //        _KeyWords = value;
        //    }
        //}
        [XmlElement(ElementName = "KeyWords")]
        public List<KeyWords> KeyWordsList { get; set; }

     

        private BrowGraph _BrowGraph;
        [XmlElement]
        public BrowGraph BrowGraph
        {
            get
            {
                if (_BrowGraph == null)
                {
                    _BrowGraph = new BrowGraph();
                }
                return _BrowGraph;
            }
            set
            {
                _BrowGraph = value;
            }
        }

        private Consts _Consts;
        [XmlElement]
        public Consts Consts
        {
            get
            {
                if (_Consts == null)
                {
                    _Consts = new Consts();
                }
                return _Consts;
            }
            set
            {
                _Consts = value;
            }
        }

        private Format _Format;
        [XmlElement]
        public Format Format
        {
            get
            {
                if (_Format == null)
                {
                    _Format = new Format();
                }
                return _Format;
            }
            set
            {
                _Format = value;
            }
        }


        private MaintInfo _MaintInfo;
        [XmlElement]
        public MaintInfo MaintInfo
        {
            get
            {
                if (_MaintInfo == null)
                {
                    _MaintInfo = new MaintInfo();
                }
                return _MaintInfo;
            }
            set
            {
                _MaintInfo = value;
            }
        }
    }
}
