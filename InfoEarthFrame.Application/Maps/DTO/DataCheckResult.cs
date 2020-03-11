using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class DataCheckResult : EntityDto
    {
        public string ExcelFile { get; set; }
        public List<DataChackLog> CheckInfoList { get; set; }
    }

    public class DataChackLog
    {
        public string FileName { get; set; }
        public List<string> Log { get; set; }

        public DataChackLog(string fileName, List<string> log)
        {
            FileName = fileName;
            Log = log;
        }
    }
}
