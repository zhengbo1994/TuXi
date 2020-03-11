using System;
using System.Collections.Generic;
using System.Text;

namespace InfoEarthFrame.Common.Data
{
    public class VectorDataType : BaseDataType
    {
        private string _filename = null;

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public VectorDataType()
        {
            this.DataGuid = Guid.NewGuid().ToString();
        }

        public VectorDataType(string guid)
        {
            this.DataGuid = guid;
        }

        public VectorDataType(string guid, string filename)
            : this(guid)
        {
            this.Filename = filename;
        }

        public override void AddToGlobe()
        {
            throw new NotImplementedException();
        }
    }
}
