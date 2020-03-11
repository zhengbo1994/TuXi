using Infoearth.Util;
using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using InfoEarthFrame.DataManage.DTO;
using InfoEarthFrame.Maps.DTO;
using InfoEarthFrame.WebApi.Next.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Serialization;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class XmlDto
    {
        public string Xml { get; set; }
    }
    public class ToolsController : BaseApiController
    {
        private readonly IGeologyMappingTypeAppService _geologyMappingTypeAppService;
        private readonly IDataConvertAppService _dataConvertAppService;

        public ToolsController(IGeologyMappingTypeAppService geologyMappingTypeAppService,IDataConvertAppService dataConvertAppService)
        {
            this._geologyMappingTypeAppService = geologyMappingTypeAppService;
            this._dataConvertAppService = dataConvertAppService;
        }

        /// <summary>
        /// 导入图层种类（来源于老的oracle TBL_GEOLOGYMAPPINGTYPE 表）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IHttpActionResult ImportMappingType([FromBody]XmlDto dto)
        {
            var data = XmlConvert.XmlDeserialize<RowData>(dto.Xml, System.Text.Encoding.Default);
            if (data.Rows != null && data.Rows.Any())
            {
                foreach (var row in data.Rows)
                {
                    row.ClassName = HttpUtility.HtmlDecode(row.ClassName);
                }
            }
            var count = _geologyMappingTypeAppService.ImportMappingType(data);
            return Ok(GetResult(0, count));
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="mainId">文件夹ID</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult UploadFile(string mainId)
        {
            //判断有无文件
            var file = HttpContext.Current.Request.Files[0];
            if (file == null)
            {
                //没有上传文件
                return Ok(GetResult(-503));
            }


            var tempDir = Path.Combine(HttpContext.Current.Server.MapPath("~"), ConfigContext.Current.DefaultConfig["upload:tempdir"], mainId);
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            var name = file.FileName;
            var saveFilePath = Path.Combine(tempDir, name);
            file.SaveAs(saveFilePath);
            return Ok(GetResult(true));
        }

        /// <summary>
        /// 格式转换
        /// </summary>
        /// <param name="mainId">主数据ID</param>
        /// <param name="outputFormat">目标格式</param>
        /// <returns></returns>
         [ResponseType(typeof(ApiResult))]
        public IHttpActionResult FormatConversion([FromBody]string mainId, string outputFormat)
        {
            var context = new FormatConvertContext(_dataConvertAppService, mainId, outputFormat);
            context.Convert();
            return Ok(GetResult(context.IsSuccess, new
            {
                filePath = context.RarFileRelativePath,
                fileName = context.RarFileName
            }));
        }

        /// <summary>
        /// 坐标转换
        /// </summary>
         /// <param name="mainId">主数据</param>
        /// <returns></returns>
         [ResponseType(typeof(ApiResult))]
         public IHttpActionResult CoordinateConversion([FromBody]string mainId, string coordPoint)
         {
             var context = new CoordinateConvertContext(_dataConvertAppService, mainId, coordPoint);
             context.Convert();
             return Ok(GetResult(context.IsSuccess, new
             {
                 filePath = context.RarFileRelativePath,
                 fileName = context.RarFileName
             }));
         }

        /// <summary>
        /// 投影转换
        /// </summary>
         /// <param name="mainId">主数据</param>
         /// <param name="outputCoordName">坐标系</param>
        /// <returns></returns>
         [ResponseType(typeof(ApiResult))]
         public IHttpActionResult ShadowConversion([FromBody]string mainId, string outputCoordName)
         {
             var context = new ShadowConvertContext(_dataConvertAppService, mainId, outputCoordName);
             context.Convert();
             return Ok(GetResult(context.IsSuccess, new
             {
                 filePath = context.RarFileRelativePath,
                 fileName = context.RarFileName
             }));
         }
        /// <summary>
        ///  根据经纬度,比例尺计算标准图幅号
        /// </summary>
        /// <param name="lon">精度</param>
        /// <param name="lat">纬度</param>
        /// <param name="scale">比例尺</param>
        /// <returns></returns>

         [ResponseType(typeof(ApiResult))]
         public IHttpActionResult GetMapCode(double lon, double lat, int scale)
         {
             string mapCode = MapCode.GetMapCodeByScale(lon, lat, (MapScaleType)scale);
             return Ok(GetResult(0, mapCode));
         }


         /// <summary>
         /// 根据标准图幅获取范围
         /// </summary>
         /// <param name="mapcode">图幅号</param>
         /// <returns></returns>

         [ResponseType(typeof(ApiResult))]
         public IHttpActionResult GetMapRangeByMapCode(string mapcode)
         {
             MapRange mapRange = MapCode.GetMapRangeByMapCode(mapcode);
             return Ok(GetResult(0, mapRange));
         }
    }
}
