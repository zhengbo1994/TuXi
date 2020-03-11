using System;
using System.Collections.Generic;
using System.Text;

namespace InfoEarthFrame.Common.Data
{
    public abstract class BaseDataType
    {
        private string _dataGuid = null;

        public string DataGuid
        {
            get { return _dataGuid; }
            set { _dataGuid = value; }
        }

        private string _dataDescription = null;

        public string DataDescription
        {
            get { return _dataDescription; }
            set { _dataDescription = value; }
        }

        public abstract void AddToGlobe();
    }
}
