using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Data;
using System.IO;

namespace InfoEarthFrame.Common
{
    /// <summary>
    /// 写Excel的实用类，NPOI 是POI项目的.NET 版本，是一个开源的用来读写Excel、WORD等微软OLE2组件文档的项目，不依赖于office，如果用Microsoft.Office.Interop.Excel，容易出错，尤其是安装了不同版本的office软件
    /// </summary>
    public class NpoiExcelUtility
    {
        private HSSFWorkbook _workBook = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xlsPath">xls保存路径</param>
        public NpoiExcelUtility()
        {
            _workBook = new HSSFWorkbook();
        }

        /// <summary>
        /// 将DataTable保存到sheet里
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheet"></param>
        private void DataTableToExcel(DataTable dt, ISheet sheet)
        {
            ICellStyle style = _workBook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle colStyle = _workBook.CreateCellStyle();
            colStyle.Alignment = HorizontalAlignment.Left;
            colStyle.VerticalAlignment = VerticalAlignment.Center;
            IFont font = _workBook.CreateFont();
            font.Color = NPOI.HSSF.Util.HSSFColor.LightBlue.Index;
            colStyle.SetFont(font);

            //列名
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sheet.SetDefaultColumnStyle(i, style);

                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ToString());

                cell.CellStyle = colStyle;
            }
            //内容
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                row = sheet.CreateRow(i);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    object obj = dt.Rows[i - 1][j];
                    if (obj != null)
                    {
                        ICell cell = row.CreateCell(j);
                        if (obj is double || obj is float || obj is int || obj is long || obj is decimal)
                        {
                            cell.SetCellValue(Convert.ToDouble(obj));
                        }
                        else if (obj is bool)
                        {
                            cell.SetCellValue((bool)obj);
                        }
                        else
                        {
                            cell.SetCellValue(obj.ToString());
                        }
                    }
                }
            }
            //一下方法会报异常，可能是改NPOI版本的问题，之前老的版本不会报错，这里暂时注释掉
            //for (int i = 0; i < dt.Columns.Count; i++)
            //{
            //    sheet.AutoSizeColumn(i);
            //}
        }

        /// <summary>
        /// 保存Excel
        /// </summary>
        public bool SaveExcel(string xlsPath)
        {
            try
            {
                FileStream file = new FileStream(xlsPath, FileMode.Create);
                _workBook.Write(file);
                file.Close();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public MemoryStream GetStream()
        {
            MemoryStream stream = new MemoryStream();
            _workBook.Write(stream);
            return stream;
        }

        /// <summary>
        /// 创建Sheet
        /// </summary>
        /// <param name="sheetName">sheet名称</param>
        /// <param name="tbl">DataTable数据表，当行数大于65536时，自动分割成几个sheet，sheet名称为sheetName_i</param>
        public void CreatExcelSheet(string sheetName, DataTable tbl)
        {
            string sName = this.CheckSheetName(sheetName);

            int rowMax = 65535;
            int intNum = tbl.Rows.Count / rowMax;
            int remainder = tbl.Rows.Count % rowMax;

            for (int i = 0; i < intNum; i++)
            {
                DataTable subTbl = tbl.Clone();
                for (int j = 0; j < 65535; j++)
                {
                    int rowIndex = i * rowMax + j;
                    subTbl.Rows.Add(tbl.Rows[rowIndex].ItemArray);
                }
                string subSheet = sName + "_" + (i + 1);
                ISheet sheet = _workBook.CreateSheet(subSheet);
                this.DataTableToExcel(subTbl, sheet);
            }
            if (remainder > 0)
            {
                DataTable subTbl = tbl.Clone();
                for (int j = 0; j < remainder; j++)
                {
                    int rowIndex = intNum * rowMax + j;
                    subTbl.Rows.Add(tbl.Rows[rowIndex].ItemArray);
                }
                string subSheet = sName + "_" + (intNum + 1);
                if (intNum < 1)
                {
                    subSheet = sName;
                }
                ISheet sheet = _workBook.CreateSheet(subSheet);
                this.DataTableToExcel(subTbl, sheet);
            }
        }

        /// <summary>
        /// 检查sheet名称是否合法，并去掉不合法字符
        /// </summary>
        /// <param name="sheetName"></param>
        private string CheckSheetName(string sheetName)
        {
            string rlt = sheetName;
            string[] illegalChars = { "*", "?", "\"", @"\", "/" };
            for (int i = 0; i < illegalChars.Length; i++)
            {
                rlt = rlt.Replace(illegalChars[i], "");
            }
            return rlt;
        }

        /// <summary>
        ///  检查xls路径是否合法，并去掉不合法字符
        /// </summary>
        /// <param name="filePath"></param>
        private string CheckFilePath(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string ext = Path.GetExtension(filePath);

            string[] illegalChars = { ":", "*", "?", "\"", "<", ">", "|", @"\", "/" };
            for (int i = 0; i < illegalChars.Length; i++)
            {
                fileName = fileName.Replace(illegalChars[i], "");
            }
            string rlt = Path.Combine(dir, fileName + ext);
            return rlt;
        }
    }
}
