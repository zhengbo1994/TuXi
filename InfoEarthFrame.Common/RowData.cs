using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    [XmlRoot(ElementName = "ROWDATA")]
    public class RowData
    {
        public RowData()
        {
            this.Rows = new List<Row>();
        }

        [XmlElement(ElementName = "ROW")]
        public List<Row> Rows { get; set; }
    }

    public class Row
    {
        public string Id { get; set; }

        public string ParentID { get; set; }

        public string Paths { get; set; }

        public int Sn { get; set; }

        public string ClassName { get; set; }
    }
}
