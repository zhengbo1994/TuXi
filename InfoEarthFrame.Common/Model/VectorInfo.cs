using System;
using System.Collections.Generic;
using System.Text;

namespace InfoEarthFrame.Common.Data
{
    public class VectorInfo
    {
        // guid
        private string _guid;

        public string GUID
        {
            get { return _guid; }
            set { _guid = value; }
        }
        // 文件名
        private string _filename;

        public string FILENAME
        {
            get { return _filename; }
            set { _filename = value; }
        }
        // 表名
        private string _tablename;

        public string TABLENAME
        {
            get { return _tablename; }
            set { _tablename = value; }
        }
        // 数据类型
        private string _dataType;

        public string DATATYPE
        {
            get { return _dataType; }
            set { _dataType = value; }
        }
        // 空间参考WKT
        private string _spatialRef;

        public string SPATIALREF
        {
            get { return _spatialRef; }
            set { _spatialRef = value; }
        }
    }
}
