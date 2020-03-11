using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;

//using Spire;
//using Spire.Doc;

namespace InfoEarthFrame.Common
{
    public class OfficeToPdf
    {
        private string sourcePath;
        private string targetPath;
        /// <summary>
        /// 构造函数
        /// </summary>
        ///<param name="sourcePath">源文件路径</param>
        ///<param name="targetPath">目标文件路径</param>
        /// <param name="fileExtension">源文件扩展名</param>
        public OfficeToPdf(string sourcePath, string targetPath, string fileExtension) {
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;
            if (fileExtension == ".doc" || fileExtension == ".docx")
                DOCConvertToPDF();
            else if (fileExtension == ".xls" || fileExtension == ".xlsx")
                XLSConvertToPDF();
            else if (fileExtension == ".ppt" || fileExtension == ".pptx")
                PPTConvertToPDF();


            //Document doc = new Document(sourcePath);
            //ToPdfParameterList topdfparam = new ToPdfParameterList();
            //{ 
                //AutoFitTableLayout=true,
                // CreateWordBookmarks=false,
                //  EmbeddedFontNameList=null,
                //   IsAtLast=false,
                //    IsEmbeddedAllFonts=false,
                //     IsHidden=false,
                //      PdfConformanceLevel=null,
                //       PrivateFontPaths=

            
            //};
            //doc.SaveToFile(targetPath, topdfparam);
            //doc.Close();

        }
        ///<summary>
        /// 把Word文件转换成为PDF格式文件
        ///</summary>
        private void DOCConvertToPDF()
        {
            Word.ApplicationClass application = new Word.ApplicationClass();
            Word.Documents docs= application.Documents;
             Type wordtype = application.GetType();
             Type docstype = docs.GetType();
            try
            {

                Word.Document document = (Word.Document)docstype.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, docs, new object[] { sourcePath, true, true });
                Type doctype = document.GetType();
                doctype.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, null, document, new object[] { targetPath, Word.WdSaveFormat.wdFormatPDF});   
            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                wordtype.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, application, null);
            }
            

            /*
            bool result = false;
            Word.WdExportFormat exportFormat = Word.WdExportFormat.wdExportFormatPDF;
            object paramMissing = Type.Missing;
            Word.ApplicationClass wordApplication = new Word.ApplicationClass();
            Word.Document wordDocument = null;
            try
            {
                object paramSourceDocPath = sourcePath;
                string paramExportFilePath = targetPath;

                Word.WdExportFormat paramExportFormat = exportFormat;
                bool paramOpenAfterExport = false;
                Word.WdExportOptimizeFor paramExportOptimizeFor = Word.WdExportOptimizeFor.wdExportOptimizeForPrint;
                Word.WdExportRange paramExportRange = Word.WdExportRange.wdExportAllDocument;
                int paramStartPage = 0;
                int paramEndPage = 0;
                Word.WdExportItem paramExportItem = Word.WdExportItem.wdExportDocumentContent;
                bool paramIncludeDocProps = true;
                bool paramKeepIRM = true;
                Word.WdExportCreateBookmarks paramCreateBookmarks = Word.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                bool paramDocStructureTags = true;
                bool paramBitmapMissingFonts = true;
                bool paramUseISO19005_1 = false;

                wordDocument = wordApplication.Documents.Open(
                ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                ref paramMissing, ref paramMissing, ref paramMissing,
                ref paramMissing, ref paramMissing, ref paramMissing,
                ref paramMissing, ref paramMissing, ref paramMissing,
                ref paramMissing, ref paramMissing, ref paramMissing,
                ref paramMissing);

                if (wordDocument != null)
                    wordDocument.ExportAsFixedFormat(paramExportFilePath,
                    paramExportFormat, paramOpenAfterExport,
                    paramExportOptimizeFor, paramExportRange, paramStartPage,
                    paramEndPage, paramExportItem, paramIncludeDocProps,
                    paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                    paramBitmapMissingFonts, paramUseISO19005_1,
                    ref paramMissing);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (wordDocument != null)
                {
                    wordDocument.Close(ref paramMissing, ref paramMissing, ref paramMissing);
                    wordDocument = null;
                }
                if (wordApplication != null)
                {
                    wordApplication.Quit(ref paramMissing, ref paramMissing, ref paramMissing);
                    wordApplication = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;*/
        }
        ///<summary>
        /// 把Excel文件转换成PDF格式文件
        ///</summary>
        ///<returns>true=转换成功</returns>
        private bool XLSConvertToPDF()
        {
            bool result = false;
            Excel.XlFixedFormatType targetType = Excel.XlFixedFormatType.xlTypePDF;
            object missing = Type.Missing;
            Excel.ApplicationClass application = null;
            Excel.Workbook workBook = null;
            try
            {
                application = new Excel.ApplicationClass();
                object target = targetPath;
                object type = targetType;
                workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing, missing, missing);

                workBook.ExportAsFixedFormat(targetType, target, Excel.XlFixedFormatQuality.xlQualityStandard, true, false, missing, missing, missing, missing);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }
        ///<summary>
        /// 把PowerPoint文件转换成PDF格式文件
        ///</summary>
        ///<returns>true=转换成功</returns>
        private bool PPTConvertToPDF()
        {
            bool result;
            PowerPoint.PpSaveAsFileType targetFileType = PowerPoint.PpSaveAsFileType.ppSaveAsPDF;
            object missing = Type.Missing;
            PowerPoint.ApplicationClass application = null;
            PowerPoint.Presentation persentation = null;
            try
            {
                application = new PowerPoint.ApplicationClass();
                persentation = application.Presentations.Open(sourcePath, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
                persentation.SaveAs(targetPath, targetFileType, Microsoft.Office.Core.MsoTriState.msoTrue);

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (persentation != null)
                {
                    persentation.Close();
                    persentation = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }
    }
}