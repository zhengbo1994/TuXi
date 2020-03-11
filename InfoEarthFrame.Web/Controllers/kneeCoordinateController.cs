using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using InfoEarthFrame.Common;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using System;
using System.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace InfoEarthFrame.Web.Controllers
{
    //[Authorize]
    public class KneeCoordinateController : InfoEarthFrameControllerBase
    {
        [HttpPost]
        public ActionResult index()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            JsonResult ret = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            try
            {
                Response.ContentType = "text/plain";
                Response.Charset = "utf-8";
                HttpPostedFileBase file = Request.Files["file"];
                string filePath = Server.MapPath("\\file") + "\\";

                if (file != null)
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                }
                else
                {
                    dic.Add("success", "false");
                    dic.Add("message", "请选择需要上传的文件");
                    ret.Data = dic;
                    return ret;
                }

                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension.ToUpper() != ".XLS" && fileExtension.ToUpper() != ".XLSX")//&& fileExtension.ToUpper() != ".XLSM" && fileExtension.ToUpper() != ".XLSB")
                {
                    dic.Add("success", "false");
                    dic.Add("message", "上传的文件类型错误");
                    ret.Data = dic;
                    return ret;
                }

                var path = filePath + file.FileName;
                file.SaveAs(path);
                List<KneeCoordinateModal> list = new List<KneeCoordinateModal>();
                using (FileStream f = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(f);
                    ISheet sheet = workbook.GetSheetAt(0);
                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                    {
                        HSSFRow r = (HSSFRow)sheet.GetRow(i);
                        KneeCoordinateModal coordinate = new KneeCoordinateModal
                        {
                            CoordinateX = Convert.ToDouble(r.GetCell(0).ToString()),
                            CoordinateY = Convert.ToDouble(r.GetCell(1).ToString())
                        };
                        list.Add(coordinate);
                    }
                } 
                dic.Add("success", "true");
                dic.Add("message", "文件导入成功");
                dic.Add("data", list);
                System.IO.File.Delete(path);
            }
            catch (Exception exception)
            {
                dic = new Dictionary<string, object>();
                dic.Add("success", "false");
                dic.Add("message", exception.Message);
            }
            ret.Data = dic;
            return ret;
        }
    }
}