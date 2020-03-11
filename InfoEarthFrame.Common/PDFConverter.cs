using Aspose.Cells;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common
{
    public class PDFConverter
    {
        public static void Convert(string srcPath, string desPath)
        {
            var ext = Path.GetExtension(srcPath);
            switch (ext)
            {
                case ".doc":
                case ".docx":
                    Word2PDF(srcPath, desPath);
                    break;
                case ".xls":
                case ".xlsx":
                    Excel2PDF(srcPath, desPath);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// word转换成PDF
        /// </summary>
        /// <param name="srcPath">源路径</param>
        /// <param name="desPath">目标路径</param>
        private static void Word2PDF(string srcPath, string desPath)
        {
            Aspose.Words.Document doc = new Aspose.Words.Document(srcPath);
            doc.Save(desPath, Aspose.Words.SaveFormat.Pdf);
        }

        /// <summary>
        /// Excel转换成PDF
        /// </summary>
        /// <param name="srcPath">源路径</param>
        /// <param name="desPath">目标路径</param>
        private static void Excel2PDF(string srcPath, string desPath)
        {
            Workbook excel = new Workbook(srcPath);
            excel.Save(desPath, Aspose.Cells.SaveFormat.Pdf);
        }
    }
}
