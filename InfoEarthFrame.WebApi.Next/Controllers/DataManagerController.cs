using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using InfoEarthFrame.DataManage.DTO;
using InfoEarthFrame.WebApi.Next.Models;
using Ionic.Zip;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class DataManagerController : BaseApiController
    {
        protected override string ModuleName
        {
            get
            {
                return "DataManager";
            }
        }

        private readonly IGeologyMappingTypeAppService _geologyMappingTypeAppService;
        private readonly IDataManageAppService _dataManageAppService;
        private readonly IDataConvertAppService _dataConvertAppService;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DataManagerController));
        public DataManagerController(IGeologyMappingTypeAppService geologyMappingTypeAppService,
            IDataManageAppService dataManageAppService,
            IDataConvertAppService dataConvertAppService )
        {
            this._geologyMappingTypeAppService = geologyMappingTypeAppService;
            this._dataManageAppService = dataManageAppService;
            this._dataConvertAppService=dataConvertAppService;
        }

        /// <summary>
        /// 获取图件分类树结构
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetMappingTree(string rootType = "")
        {
            var model = await _geologyMappingTypeAppService.GetMappingTree(CurrentUserId);
            var all = new List<GeologyMappingTypeOutput>{
                new GeologyMappingTypeOutput
            {
                Label =GetText(1000),
                 Id = "",
                Children = model
            }};

            if (!string.IsNullOrEmpty(rootType))
            {
                var current = new List<GeologyMappingTypeOutput>();
                all.ForEach(p =>
                {
                    p.Children.ForEach(m =>
                    {
                        if (rootType == "countory" && m.Label == "全国"
                            || rootType == "province" && m.Label == "省域"
                            || rootType == "area" && m.Label == "区域")
                        {

                            current = new List<GeologyMappingTypeOutput>{
                new GeologyMappingTypeOutput
            {
                   Label = m.Label,
      Id = "",
      Children = m.Children
            }};


                        }
                    });

                });

                return Ok(GetResult(0, current));
            }
            return Ok(GetResult(0, all));
        }

        /// <summary>
        /// 获取单个图件分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetType(string id)
        {
            var model = _geologyMappingTypeAppService.GetGeologyMappingType(id);
            return Ok(GetResult(0, model));
        }


        /// <summary>
        /// 获取分类名称状态
        /// </summary>
        /// <param name="parentId">父级分类ID</param>
        /// <param name="className">分类名称</param>
        /// <param name="oldClassName">原始分类名称（主要是用作比较）</param>
        /// <returns>-1000代表名称已存在</returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetClassNameStatus(string parentId, string className, string oldClassName)
        {
            var flag = true;
            if (string.IsNullOrEmpty(oldClassName)
              || (className.ToLower() != oldClassName.ToLower()))
            {
                flag = !_geologyMappingTypeAppService.IsClassNameExists(parentId, className);
            }
            return Ok(GetResult(flag ? 0 : -1000));
        }

        /// <summary>
        /// 新增或编辑分类
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult AddOrEditType([FromBody]GeologyMappingTypeDto dto)
        {
            if (string.IsNullOrEmpty(dto.Id))
            {
                var data = _geologyMappingTypeAppService.Insert(dto);
                return Ok(GetResult(data.Item1, data.Item2));
            }
            else
            {
                var data = _geologyMappingTypeAppService.Update(dto);
                return Ok(GetResult(data.Item1, data.Item2));
            }

        }

        /// <summary>
        /// 新增或编辑图件包
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult AddOrEditMainData([FromBody]DataMainDto dto)
        {
            try
            {
                
                if (string.IsNullOrEmpty(dto.Id)||string.IsNullOrEmpty(dto.CreaterID))
                {
                    dto.CreaterID = CurrentUserId;
                    var data = _dataManageAppService.InsertDataMain(dto);
                    return Ok(GetResult(data != null, data));
                }
                else
                {
                    var data = _dataManageAppService.UpdateDataMain(dto);
                    return Ok(GetResult(data != null, data));
                }
            }
            catch (Exception ex)
            {
                var current = ex;
                while (current != null && !string.IsNullOrEmpty(current.Message))
                {
                    if (current.Message.Contains("UK_TBL_DATAMAIN_Name")||current.Message.Contains("唯一约束"))
                    {
                        return Ok(GetResult(-1001));
                    }
                    current = current.InnerException;
                }
                throw;
            }
        }


        /// <summary>
        /// 获取图件包
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetMainData(string id)
        {
            var data = _dataManageAppService.GetDataMain(id);
            return Ok(GetResult(data != null, data));
        }

        /// <summary>
        /// 删除图件分类
        /// </summary>
        /// <param name="ids">主键字符串，多个请用,号隔开，例如:1,2,3,4</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> RemoveType([FromBody]string ids)
        {
            var idList = (ids ?? "").Split(',');
            var flag = await _geologyMappingTypeAppService.Delete(idList);
            return Ok(GetResult(flag));
        }

        /// <summary>
        /// 获取图件包列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        [ResponseType(typeof(LayuiGridResult))]
        public IHttpActionResult PostAllPageListByCondition([FromBody]GetDataMainListParamDto dto)
        {
            dto.userId = CurrentUserId;
            var result = _dataManageAppService.GetDataMainList(dto);
            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = result.RowList
                },
                Status = 0,
                Total = result.RowNum
            };
            return Ok(data);
        }

        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetChildType(string parentId)
        {
            var result = _geologyMappingTypeAppService.GetChildType(parentId);
            return Ok(GetResult(0, result));
        }

        /// <summary>
        /// 获取图件包内部树结构
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetFolderTree()
        {
            var data = ConfigContext.Current.PackageCategory.Where(p=>!p.Value.Contains("\\")).Select(p => new LayuiTreeItem
            {
                label = p.Key,
                id = p.Value,
            }).ToList();
            data.FirstOrDefault(p => p.label == "要素类文件").children =
                ConfigContext.Current.PackageCategory.Where(p => p.Value.Contains("\\")).Select(p => new LayuiTreeItem
                {
                    label = p.Key,
                    id = p.Value,
                }).ToList();

            var all = new List<LayuiTreeItem>
            {
               new LayuiTreeItem{
                children=data,
                 id="",
                  label=GetText(1002)
               }
            };
            return Ok(GetResult(0, all));
        }

        /// <summary>
        /// 获取上传文件
        /// </summary>
        /// <param name="mainId">主数据Id</param>
        /// <param name="folderName">目录全名</param>
        /// <returns></returns>

        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetUploadFiles(string mainId, string folderName)
        {
            folderName = HttpUtility.UrlDecode(folderName, System.Text.Encoding.UTF8).Replace(",", "\\");
            var data = _dataManageAppService.GetFileList(new GetDataFileListParamDto
            {
                FolderName = folderName,
                MainID = mainId
            }).OrderBy(p => p.ParentId);
            return Ok(GetResult(0,data));
        }


        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult UploadLayerFile(string mainId, string folderName)
        {
            folderName = HttpUtility.UrlDecode(folderName, System.Text.Encoding.UTF8).Replace(",", "\\");


            //判断有无文件
            var file = HttpContext.Current.Request.Files[0];
            if (file == null)
            {
                //没有上传文件
                return Ok(GetResult(-503));
            }

            var context = new UploadLayerContext(_dataConvertAppService)
            {
                FolderName = folderName,
                UploadUserId = CurrentUserId,
                MainId = mainId, 
                PostedFile=file
            };
            var flag=_dataManageAppService.UploadLayer(context);
            if (context.IsGeoServerError)
            {
                return Ok(GetResult(false, context.ErrorInfo));
            }

            return Ok(GetResult(flag,context.ErrorInfo));
        }


        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult UploadFile(string mainId, string folderName)
        {
            folderName = HttpUtility.UrlDecode(folderName, System.Text.Encoding.UTF8).Replace(",", "\\");
            //判断有无文件
            var file = HttpContext.Current.Request.Files[0];
            if (file == null)
            {
                //没有上传文件
                return Ok(GetResult(-503));
            }

            var context = new UploadFileContext
            {
                FolderName = folderName,
                UploadUserId = CurrentUserId,
                MainId = mainId,
                PostedFile = file
            };
            var flag = _dataManageAppService.UploadFile(context);
            if (folderName != "缩略图")
            {
                return Ok(GetResult(flag, context.ErrorInfo));
            }
            else {
                return Ok(GetResult(flag, context.Tag));
            }
        }

     

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="ids">主键字符串，多个请用,号隔开，例如:1,2,3,4</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> RemoveFile([FromBody]string ids)
        {
            var idList = (ids ?? "").Split(',');
            var flag = await _dataManageAppService.Delete(idList);
            return Ok(GetResult(flag));
        }


        /// <summary>
        /// 删除分类文件
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> RemovePackageCategory([FromBody]PackageCategoryDto model)
        {
            model.FolderName = HttpUtility.UrlDecode(model.FolderName, System.Text.Encoding.Default).Replace(",", "\\");
            var flag = await _dataManageAppService.Delete(model);
            return Ok(GetResult(flag));
        }


        /// <summary>
        /// 获取元数据信息
        /// </summary>
        /// <param name="mainId">主数据Id</param>
        /// <param name="folderName">目录全名</param>
        /// <returns></returns>

        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetMetaData(string mainId, string folderName)
        {
            folderName = HttpUtility.UrlDecode(folderName, System.Text.Encoding.UTF8).Replace(",", "\\");
            var data = _dataManageAppService.GetMetaData(mainId, folderName);
            return Ok(GetResult(0, data));
        }

        /// <summary>
        /// 保存元数据信息
        /// </summary>
        /// <returns></returns>

        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult SaveMetaData([FromBody]PackageCategoryDto model)
        {
            model.FolderName = HttpUtility.UrlDecode(model.FolderName,System.Text.Encoding.Default).Replace(",", "\\");
            var metaData=JsonConvert.DeserializeObject<InfoEarthFrame.Common.Metadata>(model.Data);
            metaData.MainId=model.MainId;
            metaData.FolderName=model.FolderName;
            metaData.CreateUserId = base.CurrentUserId;
            var flag = _dataManageAppService.SaveMetaData(metaData);
            return Ok(GetResult(flag));
        }

        /// <summary>
        /// GB2312转换成UTF8
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string gb2312_utf8(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            byte[] gb;
            gb = gb2312.GetBytes(text);
            gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb);
        }

        /// <summary>
        /// UTF8转换成GB2312
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string utf8_gb2312(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] utf;
            utf = utf8.GetBytes(text);
            utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
            //返回转换后的字符   
            return gb2312.GetString(utf);
        }

        /// <summary>
        /// 上传元数据文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult UploadMetaDataFile(string mainId, string folderName)
        {
            //判断有无文件
            var file = HttpContext.Current.Request.Files[0];
            if (file == null)
            {
                //没有上传文件
                return Ok(GetResult(-503));
            }

            var tempDir = Path.Combine(HttpContext.Current.Server.MapPath("~"), ConfigContext.Current.DefaultConfig["upload:tempdir"], mainId, folderName);
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            var name = file.FileName;
            var saveFilePath = Path.Combine(tempDir, name);
            file.SaveAs(saveFilePath);


            var metaDataContext = new Metadata
            {
                CreateUserId = CurrentUserId,
                FolderName = folderName,
                MainId = mainId,
                XmlFilePath = saveFilePath
            };
          
            var flag=_dataManageAppService.SaveMetaData(metaDataContext);
            return Ok(GetResult(flag));
        }

        /// <summary>
        /// 上传图件压缩包
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult UploadMainDataPackage()
        {
            var mainId = Guid.NewGuid().ToString();

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

            var name=file.FileName;
            var newFileName = file.FileName.Insert(file.FileName.LastIndexOf("."), "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
           var saveFilePath = Path.Combine(tempDir, newFileName);
           file.SaveAs(saveFilePath);
           
            var fileInfo=new FileInfo(saveFilePath);
            var data = new UploadFileDto
            {
                Ext = fileInfo.Extension,
                MainID = mainId,
                FileSize = fileInfo.Length,
                FileName = fileInfo.Name,
                Name = name.Substring(0,name.LastIndexOf("."))
            };

            return Ok(GetResult(0, data));
        }

        /// <summary>
        /// 处理图件压缩包
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult HandleMainDataPackage([FromBody]UploadPackagDto dto)
        {
            var context = new UploadPackageContext
            {
                MainId = dto.MainId,
                ZipFileName = dto.ZipFileName
            };
           var flag= _dataManageAppService.UploadPackage(context);
            return Ok(GetResult(flag, context.UploadFileResults));
        }


        /// <summary>
        /// 获取上传包错误信息
        /// </summary>
        /// <param name="mainId"></param>
        /// <returns></returns>
         [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetErrorMsg(string mainId)
        {
            var data = _dataManageAppService.GetErrorMsg(mainId);
            return Ok(GetResult(0, data));
        }


        /// <summary>
        /// 修改图件包状态
        /// </summary>
        /// <param name="mainId"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult ChangePackageStatus([FromBody] string mainId)
        {
            var flag = _dataManageAppService.ChangePackageStatus(mainId);
            return Ok(GetResult(flag));
        }

        /// <summary>
        /// 删除图件包
        /// </summary>
        /// <param name="mainId"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult RemovePackage([FromBody]string mainId)
        {
            var flag = _dataManageAppService.RemovePackage(mainId);
            return Ok(GetResult(flag));
        }
    }
}