using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.DataManage.DTO
{
    public class PackageCategoryDto
    {
        public string MainId { get; set; }

        public string FolderName { get; set; }

        public string Data { get; set; }
    }


    public class UploadPackagDto
    {
        public string MainId { get; set; }

        public string ZipFileName { get; set; }
    }
}
