using System.Collections;
using System.Data;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

namespace InfoEarthFrame.Common
{
    public static class ExcelUtility
    {
        /// <summary>
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="cellStyle">样式</param>
        /// <param name="cellPosition">第列</param>
        /// <param name="cellValue">值</param>
        public static void SetCell(HSSFRow row, ICellStyle cellStyle, int cellPosition, string cellValue)
        {
            HSSFCell cell;
            row.Height = 400;
            cell = (HSSFCell)row.CreateCell(cellPosition);
            cell.CellStyle = cellStyle;
            //cell.CellStyle.ShrinkToFit = true;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cellValue);
        }

        /// <summary>
        ///     增加表头信息
        /// </summary>
        /// <param name="sheet">当前sheet</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="row">填充的数据</param>
        /// <param name="indentationIndex">缩进</param>
        /// <param name="hidden"></param>
        /// <param name="headerCellStyle"></param>
        public static void PrintHead(HSSFSheet sheet, ref int rowIndex, DataRow row, int indentationIndex,
                                     Hashtable hidden, ICellStyle headerCellStyle)
        {
            int left = 0;
            var headerRow = (HSSFRow)sheet.CreateRow(rowIndex);
            headerCellStyle.Alignment = HorizontalAlignment.Center;
            headerCellStyle.FillForegroundColor = HSSFColor.SkyBlue.Index;
            headerCellStyle.FillPattern = FillPattern.SolidForeground;
            headerCellStyle.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyle.Alignment = HorizontalAlignment.Center;

            string[] hidenColmunsName;
            if (hidden == null)
            {
                hidenColmunsName = null;
            }
            else
            {
                try
                {
                    hidenColmunsName = (string[])hidden[row.Table.TableName];
                }
                catch
                {
                    hidenColmunsName = null;
                }
            }
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                if (hidenColmunsName == null)
                {
                }
                else if (hidenColmunsName.Contains(row.Table.Columns[i].ColumnName))
                {
                    left++;
                    continue;
                }
                SetCell(headerRow, headerCellStyle, i + indentationIndex - left, row.Table.Columns[i].ColumnName);
            }
            rowIndex++;
        }

        /// <summary>
        ///     增加表的数据信息
        /// </summary>
        /// <param name="sheet">当前sheet</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="row">填充的数据</param>
        /// <param name="indentationIndex">缩进</param>
        /// <param name="hidden"></param>
        /// <param name="cellStyle"></param>
        public static void PrintData(HSSFSheet sheet, ref int rowIndex, DataRow row, int indentationIndex,
                                     Hashtable hidden, ICellStyle cellStyle)
        {
            int left = 0;
            var dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            string[] hidenColmunsName;
            if (hidden == null)
            {
                hidenColmunsName = null;
            }
            else
            {
                try
                {
                    hidenColmunsName = (string[])hidden[row.Table.TableName];
                }
                catch
                {
                    hidenColmunsName = null;
                }
            }
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                if (hidenColmunsName == null)
                {
                }
                else if (hidenColmunsName.Contains(row.Table.Columns[i].ColumnName))
                {
                    left++;
                    continue;
                }
                SetCell(dataRow, cellStyle, i + indentationIndex - left, row[i].ToString());
            }
            rowIndex++;
        }
    }
}