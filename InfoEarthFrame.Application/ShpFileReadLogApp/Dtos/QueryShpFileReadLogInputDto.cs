using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ShpFileReadLogApp.Dtos
{
    public class QueryShpFileReadLogInputParamDto
    {
        public string Createby { get; set; }
        public int Readstatus { get; set; }
        public string Message { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
