using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using Abp.Application.Services;
using iTelluro.GeologicMap.TopologyCheck;
using System.IO.Compression;
using System.IO;
using InfoEarthFrame.Core;
using System.Drawing;
using System.Data.OleDb;
using System.Data;
using System.Xml;
using System.Reflection;
using OSGeo.GDAL;
using System.Data.OracleClient;
using iTelluro.ZoomifyTile;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using InfoEarthFrame.DataManage.DTO;
using System.Data.Entity;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Common;
using Newtonsoft.Json;
using InfoEarthFrame.Application.MapApp.Dtos;
using InfoEarthFrame.Application.OperateLogApp;
using InfoEarthFrame.Application.MapReleationApp;
using InfoEarthFrame.Application.LayerContentApp;
using InfoEarthFrame.Core.Repositories;

namespace InfoEarthFrame.Application
{
    public class DataManageAppService : ApplicationService, IDataManageAppService
    {

       // InfoEarthFrame.Data.IDatabase DBAccess = InfoEarthFrame.Data.Factory.GetDBAccess(ConfigurationManager.ConnectionStrings["Default"].ConnectionString, InfoEarthFrame.Data.AccessDBType.Oracle);

        //public readonly IDataConvertAppService DataConvert = new DataConvertAppService(null, null, null, null);

        public readonly InfoEarthFrame.Core.IDataMainRepository _DataMainRepository = null;
        public readonly IDataManageFileRepository _DataManageFileRepository = null;
        public readonly IGeologyMappingTypeRepository _GeologyMappingType = null;
        private readonly IOperateLogAppService _IOperateLogAppService;
        public readonly IMetaDataRepository _MetaDataRepository = null;
        private readonly IMapReleationAppService _MapReleationAppService;
        private readonly ILayerContentRepository _layerContentRepository;
        //public readonly IIdentificationRepository _IdentificationRepository = null;
        //public readonly ICitationRepository _CitationRepository = null;
        //public readonly IGeoBndBoxRepository _GeoBndBoxRepository = null;
        //public readonly ITempExtentRepository _TempExtentRepository = null;
        //public readonly IVerExtentRepository _VerExtentRepository = null;
        //public readonly IIdPoCRepository _IdPoCRepository = null;
        //public readonly IKeyWordsRepository _KeyWordsRepository = null;
        //public readonly IBrowGraphRepository _BrowGraphRepository = null;
        //public readonly IConstsRepository _ConstsRepository = null;
        //public readonly IFormatRepository _FormatRepository = null;
        //public readonly IMaintInfoRepository _MaintInfoRepository = null;
        //public readonly IDqInfoRepository _DqInfoRepository = null;
        //public readonly IRefSysInfoRepository _RefSysInfoRepository = null;
        //public readonly IDistInfoRepository _DistInfoRepository = null;
        //public readonly IConInfoRepository _ConInfoRepository = null;
        //public readonly IContactRepository _ContactRepository = null;
        //public readonly IContactpeopleRepository _ContactpeopleRepository = null;

        public readonly ILogRepository _LogRepository = null;
        // 元数据
        public readonly IMetaDataRepository _MetaData = null;

         private readonly IDataConvertAppService _dataConvertAppService;
        public DataManageAppService(Core.IDataMainRepository dataMainRepository,
            Core.IDataManageFileRepository dataManageFile, 
            Core.IGeologyMappingTypeRepository dataGeologyMappingType,
             IDataConvertAppService dataConvertAppService,
            Core.IMetaDataRepository metaDataRepository,
                   IOperateLogAppService iOperateLogAppService,
            IMapReleationAppService MapReleationAppService, ILayerContentRepository layerContentRepository)
           // Core.IIdentificationRepository identificationRepository, Core.ICitationRepository citationRepository, Core.IGeoBndBoxRepository geoBndBoxRepository,
          //  Core.ITempExtentRepository tempExtentRepository, Core.IVerExtentRepository verExtentRepository, Core.IIdPoCRepository idPoCRepository, Core.IKeyWordsRepository keyWordsRepository, Core.IBrowGraphRepository browGraphRepository,
          //  Core.IConstsRepository constsRepository, Core.IFormatRepository formatRepository, Core.IMaintInfoRepository maintInfoRepository, Core.IDqInfoRepository dqInfoRepository, Core.IRefSysInfoRepository refSysInfoRepository,
         //   Core.IDistInfoRepository distInfoRepository, Core.IConInfoRepository conInfoRepository, Core.IContactRepository contactRepository, Core.IContactpeopleRepository contactpeopleRepository, ILogRepository logRepository)
        {
            _DataMainRepository = dataMainRepository;
            _DataManageFileRepository = dataManageFile;
            _GeologyMappingType = dataGeologyMappingType;
            _dataConvertAppService = dataConvertAppService;
            _MetaDataRepository = metaDataRepository;
            this._IOperateLogAppService = iOperateLogAppService;
            this._MapReleationAppService = MapReleationAppService;
            this._layerContentRepository = layerContentRepository;
            //_IdentificationRepository = identificationRepository;
            //_CitationRepository = citationRepository;
            //_GeoBndBoxRepository = geoBndBoxRepository;
            //_TempExtentRepository = tempExtentRepository;
            //_VerExtentRepository = verExtentRepository;
            //_IdPoCRepository = idPoCRepository;
            //_KeyWordsRepository = keyWordsRepository;
            //_BrowGraphRepository = browGraphRepository;
            //_ConstsRepository = constsRepository;
            //_FormatRepository = formatRepository;
            //_MaintInfoRepository = maintInfoRepository;
            //_DqInfoRepository = dqInfoRepository;
            //_RefSysInfoRepository = refSysInfoRepository;
            //_DistInfoRepository = distInfoRepository;
            //_ConInfoRepository = conInfoRepository;
            //_ContactRepository = contactRepository;
            //_ContactpeopleRepository = contactpeopleRepository;

            //_LogRepository = logRepository;
        }

        /// <summary>
        /// 添加 数据管理（主表）TBL_DATAMAIN
        /// </summary>
        /// <param name="entity">DataMain 实体</param>
        public DataMain InsertDataMain(DataMainDto model)
        {


            var entity = AutoMapper.Mapper.Map<DataMain>(model);
            try
            {
 
                entity.Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id;
                entity.StorageTime = DateTime.Now;

                //整个图件包上传的
                if (!string.IsNullOrEmpty(model.ZipFileName))
                {
                    entity.Status = 1;
                }

                var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();
                db.DataMain.Add(entity);

                var input = new MapInputDto
                {
                    CreateUserId = model.CreaterID,
                    CreateDT = DateTime.Now,
                    CreateUserName = model.Creater,
                    MapName = model.Name,
                    SpatialRefence = System.Configuration.ConfigurationManager.AppSettings["SpatialRefence"],
                    Id = entity.Id
                };
                MapEntity entity1 = new MapEntity
                {
                    Id = input.Id,
                    MapName = input.MapName,
                    MapBBox = input.MapBBox,
                    MapPublishAddress = input.MapPublishAddress,
                    MapStatus = input.MapStatus,
                    MapDesc = input.MapDesc,
                    MapType = input.MapType,
                    MapTag = input.MapTag,
                    PublishDT = input.PublishDT,
                    SortCode = input.SortCode,
                    EnabledMark = input.EnabledMark,
                    DeleteMark = input.DeleteMark,
                    CreateUserId = input.CreateUserId,
                    CreateUserName = input.CreateUserId,
                    CreateDT = DateTime.Now,
                    ModifyUserId = input.ModifyUserId,
                    ModifyUserName = input.ModifyUserName,
                    ModifyDate = input.ModifyDate,
                    MapScale = input.MapScale,
                    SpatialRefence = input.SpatialRefence,
                    MapLegend = input.MapLegend
                };
                ChineseConvert chn = new ChineseConvert();
                string mapEnName = chn.GetPinyinInitials(entity1.MapName);
                string lastIndex = (System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString());
                entity1.MapEnName = mapEnName + lastIndex;
                var query =  db.Map.Add(entity1);
                db.SaveChanges();
                try
                {
                    _IOperateLogAppService.WriteOperateLog(input.Id, input.ModifyUserId, 1002, 1101, 1201, 1411, "(" + input.MapName + ")");
                }
                catch (Exception ex)
                {
                    _IOperateLogAppService.WriteOperateLog(input.Id, input.ModifyUserId, 1002, 1101, 1202, 1412, "(" + input.MapName + ")");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
          //  var sql = _DataMainRepository.GenerateInsertSql(entity);

           // var flag = db.Database.ExecuteSqlCommand(sql) > 0;
            //if (!flag)
           // {
            //    return null;
          //  }

            return entity;
        }

        public DataMain UpdateDataMain(DataMainDto model)
        {
            var oldModel = GetDataMain(model.Id);
            if (oldModel == null)
            {
                throw new Exception("未找到[Id='" + model.Id + "']的数据");
            }

            oldModel.Scale = model.Scale;
            oldModel.Version = model.Version;
            oldModel.Name = model.Name;
            oldModel.MappingTypeID = model.MappingTypeID;

            var sql = _DataMainRepository.GenerateUpdateSql(oldModel);
            var db = _DataMainRepository.GetDbContext();
            var flag = db.Database.ExecuteSqlCommand(sql) > 0;
            if (!flag)
            {
                return null;
            }
            return oldModel;
        }

        /// <summary>
        /// 删除 数据管理（主表）TBL_DATAMAIN
        /// </summary>
        /// <param name="DataMainID">ID</param>
        [Abp.Domain.Uow.UnitOfWork(IsDisabled = true)]
        public bool DeleteDataMain(List<string> DataMainIDList)
        {
            //foreach (string delId in DataMainIDList)
            //{
            //    List<string> SQLList = new List<string>();
            //    DataTable delTable = DBAccess.GetDataSetFromExcuteCommand("select TABLE_NAME from user_tables where TABLE_NAME like 'TLB_SP_%'").Tables[0];
            //    DataTable fileTable = DBAccess.GetDataSetFromExcuteCommand("select \"Id\", \"FileName\" from TBL_DATAMANAGEFILE where \"MainID\" = '" + delId + "'").Tables[0];

            //    SQLList.Add("delete tbl_datamanagefile where \"MainID\" = '" + delId + "'"); // 删除文件
            //    SQLList.Add("delete tbl_datamain where \"Id\" = '" + delId + "'"); // 删除主表
            //    SQLList.Add("delete tbl_md_metadata where \"MainID\" = '" + delId + "'"); // 删除元数据
            //    SQLList.Add("delete TBL_LAYERMANAGER where \"DataMainID\" = '" + delId + "'"); // 删除元数据

            //    if (delTable != null && fileTable != null && delTable.Rows.Count > 0 && fileTable.Rows.Count > 0)
            //    {
            //        // 删除TLB_SP_XXXXXX
            //        for (int i = 0; i < fileTable.Rows.Count; i++)
            //        {
            //            string SPName = GetSP_XXX(delTable, fileTable.Rows[i]);
            //            if (SPName.Trim().Length == 6)
            //            {
            //                SQLList.Add("delete TLB_SP_" + SPName + " where \"MainID\" = '" + delId + "' AND \"FileID\" = '" + fileTable.Rows[i]["ID"] + "'");
            //            }
            //        }
            //    }

            //    DBAccess.ExcuteTransaction(SQLList);
            //}

            return true;
        }

        /// <summary>
        /// 添加 数据管理（文件） TBL_DATAMANAGEFILE
        /// </summary>
        /// <param name="entity">DataManageFile 实体</param>
        public void InsertDataManageFile(Core.DataManageFile entity)
        {
           // _DataManageFileRepository.Insert(entity);
        }

        /// <summary>
        /// 获得  数据管理（主表）TBL_DATAMAIN 列表
        /// </summary>
        /// <param name="param">参数DTO</ param>
        /// <returns>结果DTO</returns>
        public GetDataMainListResultDto GetDataMainList(GetDataMainListParamDto param)
        {
            GetDataMainListResultDto retDTO = new GetDataMainListResultDto();
            retDTO.RowList = new List<GetDataMainListResult>();
            retDTO.RowNum = 0;
            if (string.IsNullOrEmpty(param.GeologyMappingTypeID))
            {
                return retDTO;
            }
            IEnumerable<GetDataMainListResult> items = _DataMainRepository.GetAll().Select(s => new GetDataMainListResult()
            {
                Id = s.Id,
                MappingTypeID = s.MappingTypeID,
                Scale = s.Scale,// == null ? "" : "1：" + s.Scale.ToString() + "万",
                ScaleName = "",
                MetaDataID = "",
                MappingTypeName = "",
                ZipFileName = s.ZipFileName,
                Version = s.Version,
                VersionNo = s.VersionNo,
                StorageTime = s.StorageTime,
                Name = s.Name,
                Status=s.Status
                
            });

            // 条件过滤
            if (param.Name != null && param.Name.Trim().Length > 0)
            {
                items = items.Where(s => s.Name.ToUpper().IndexOf(param.Name.ToUpper()) != -1);
            }
            if (param.StartDate != null)
            {
                items = items.Where(s => s.StorageTime >= param.StartDate.Value.AddHours(-1));
            }
            if (param.EndDate != null)
            {
                items = items.Where(s => s.StorageTime <= param.EndDate.Value.AddDays(1).AddHours(-1));
            }
            if (param.GeologyMappingTypeID != null && param.GeologyMappingTypeID != "")
            {
     List<string> GMList=null;
     if (param.userId.ToLower() != "admin")
     {
         var sql = "select \"Id\" FROM" +
"	\"TBL_GEOLOGYMAPPINGTYPE\" T where  EXISTS(" +
"select \"LayerId\"  from \"TBL_GROUP_RIGHT\" r " +
" where r.\"GroupId\" in(" +
"select u.\"GroupId\"  from \"TBL_GROUP_USER\" u where u.\"UserId\"=\'" + param.userId + "\' and r.\"IsBrowse\"=1" +
"group by u.\"GroupId\" " +
") and t.\"Id\"=r.\"LayerId\"" +
")";
         GMList = _DataMainRepository.GetDbContext().Database.SqlQuery<string>(sql).ToList();
     }
     else
     {
         GMList = _GeologyMappingType.GetAll().Where(s => s.Paths.Contains(param.GeologyMappingTypeID)).Select(item => item.Id).ToList();
     }
                items = items.Where(s => GMList.Any(a => a == s.MappingTypeID));// s.MappingTypeID == param.GeologyMappingTypeID);
            }

            retDTO.RowNum=items.Count();

            // 设置元数据
            List<GetDataMainListResult> list = items.OrderByDescending(t => t.StorageTime).Skip((param.pageIndex - 1) * param.pageSize).Take(param.pageSize).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                GeologyMappingType gmTypeEntity = _GeologyMappingType.Get(list[i].MappingTypeID);
                if (gmTypeEntity != null)
                {
                    list[i].MappingTypeName = gmTypeEntity.ClassName;
                }

                //List<MetaData> tmpList = _MetaDataRepository.GetAll().ToList();
                //if (tmpList.Count > 0)
                //{
                //    tmpList = tmpList.Where(s => s.MainID == list[i].Id).ToList();
                //    if (tmpList.Count > 0)
                //    {
                //        list[i].MetaDataID = tmpList[0].Id;
                //    }
                //}

               // list[i].VersionName = list[i].Version == 1 ? "验收版" : "最终版";

                //if (list[i].Scale != null)
                //{
                //    string tempNum = "";
                //    string scaleNum = list[i].Scale.ToString();
                //    if (scaleNum.Length < 4)
                //    {
                //        tempNum = String.Copy(scaleNum);
                //    }
                //    else
                //    {
                //        for (int j = 3; j < scaleNum.Length; j += 3)
                //        {
                //            scaleNum = scaleNum.Insert((scaleNum.Length - j), " ");
                //            j++;
                //        }
                //        tempNum = String.Copy(scaleNum);
                //    }

                //    list[i].ScaleName = "1：" + tempNum;
                //}
                //if (list[i].ZipFileName != null)
                //{
                //    if (list[i].ZipFileName.LastIndexOf('.') != -1)
                //    {
                //        list[i].ZipFileName = list[i].ZipFileName.Remove(list[i].ZipFileName.LastIndexOf('.'));
                //    }
                //}
            }

            retDTO.RowList = list;

            return retDTO;
        }

        /// <summary>
        /// 获得 数据管理（文件） TBL_DATAMANAGEFILE
        /// </summary>
        /// <param name="param">参数DTO</param>
        /// <returns>列表</returns>
        public List<GetDataFileListResultDto> GetFileList(GetDataFileListParamDto param)
        {
            var db =(InfoEarthFrameDbContext) _DataManageFileRepository.GetDbContext();
            var ftpSite = ConfigContext.Current.FtpConfig["Package"].Site;
            var items = (from s in db.DataManageFile.Where(s => s.MainID == param.MainID && s.FolderName == param.FolderName)
                         join l in db.LayerContent.AsQueryable()
                        on s.Id equals l.Id into r
                        from row in r.DefaultIfEmpty()
                select new GetDataFileListResultDto
            {
                Id = s.Id,
                MainID = s.MainID,
                FolderName = s.FolderName,
                FileName = s.FileName,
                Ext = s.Ext,
                StorageTime = s.StorageTime,
                FilePath = s.FileData,
                FileLength = s.FileSize.Value,
                ParentId = s.ParentId,
                IsInGeoServer=!string.IsNullOrEmpty(row.Id)&&string.IsNullOrEmpty(s.ParentId)&&(!s.Id.EndsWith("geo500")&&!s.Id.EndsWith("geo-200"))
            }).OrderBy(t => t.FileName).ToList();

            // 过滤掉不可以显示的文件
            //string Exts = ConfigurationManager.AppSettings["DataManageFile"];

            //if (Exts.Length > 0)
            //{
            //    items = items.Where(s => Exts.ToUpper().IndexOf(s.Ext.ToUpper()) != -1);
            //}
            //List<GetDataFileListResultDto> list = items.ToList();

            //// 设置元数据编号
            //for (int i = 0; i < list.Count; i++)
            //{
            //    if (list[i].FolderName.IndexOf("元数据") != -1 && list[i].Ext.ToUpper() == ".XML")
            //    {
            //        List<MetaData> tmpList = _MetaDataRepository.GetAll().ToList();
            //        if (tmpList.Count > 0)
            //        {
            //            tmpList = tmpList.Where(s => s.FileID == list[i].Id).ToList();
            //        }
            //        if (tmpList.Count > 0)
            //        {
            //            list[i].MetaDataID = tmpList[0].Id;
            //        }
            //    }
            //}

            foreach (var item in items)
            {
                item.FilePath = ftpSite + "/" + (item.FilePath.Replace("\\", "/"));
            }
            return items;
        }

     //   /// <summary>
     //   /// 获得文件下载路径
     //   /// </summary>
     //   /// <param name="FileID"></param>
     //   /// <returns></returns>
     //   public string GetFile(string FileID)
     //   {
     //       DataManageFile entity = _DataManageFileRepository.Get(FileID);
     //       if (entity == null)
     //       {
     //           return "";
     //       }
     //       else
     //       {
     //           string FolderPath = ConfigurationManager.AppSettings["ConvertExprotPath"] + Guid.NewGuid().ToString() + "\\";
     //           if (Directory.Exists(FolderPath) == false)
     //           {
     //               Directory.CreateDirectory(FolderPath);
     //           }

     //           File.WriteAllBytes(FolderPath + entity.FileName, entity.FileData);

     //           return FolderPath + entity.FileName;
     //       }
     //   }

     //   /// <summary>
     //   /// 获得Zip文件的下载路径
     //   /// </summary>
     //   /// <param name="DataMainId"></param>
     //   /// <returns></returns>
     //   public string GetZipFile(string DataMainId)
     //   {
     //       var model = _DataMainRepository.Get(DataMainId);
     //       return ConfigurationManager.AppSettings["DataMainFile"] + model.FilePath;
     //   }

     //   /// <summary>
     //   /// 获得成果图目录
     //   /// </summary>
     //   /// <param name="DataMainId"></param>
     //   /// <returns></returns>
     //   public string GetResultChart(string DataMainId)
     //   {
     //       if (Directory.Exists(ConfigurationManager.AppSettings["ImagePath"] + DataMainId + "\\") == true)
     //       {
     //           return ConfigurationManager.AppSettings["ImagePath"] + DataMainId + "\\";
     //       }
     //       else
     //       {
     //           return "-1";
     //       }
     //   }
     //   public string GetResultChartFile(string DataMainId)
     //   {
     //       if (File.Exists(ConfigurationManager.AppSettings["ImagePath"] + DataMainId + ".zip") == true)
     //       {
     //           return ConfigurationManager.AppSettings["ImagePath"] + DataMainId + ".zip";
     //       }
     //       else
     //       {
     //           return "-1";
     //       }
     //   }

     //   /// <summary>
     //   /// 
     //   /// </summary>
     //   /// <param name="DataMainId"></param>
     //   /// <returns></returns>
     //   public string GetThumbnailTileGroup(string DataMainId, int type)
     //   {
     //       string path = "";
     //       if (type == 1)
     //       {
     //           path = ConfigurationManager.AppSettings["Thumbnail"];
     //       }
     //       else if (type == 2)
     //       {
     //           path = ConfigurationManager.AppSettings["ImagePath"];
     //       }
     //       string[] Directs = Directory.GetDirectories(path + DataMainId + "\\");
     //       if (Directs.Length > 0)
     //       {
     //           return Directs[0];
     //       }
     //       else
     //       {
     //           return "";
     //       }
     //   }

     //   public string GetThumbnailSize(string DataMainId, int type)
     //   {
     //       string path = "";
     //       if (type == 1)
     //       {
     //           path = ConfigurationManager.AppSettings["Thumbnail"];
     //       }
     //       else if (type == 2)
     //       {
     //           path = ConfigurationManager.AppSettings["ImagePath"];
     //       }

     //       if (File.Exists(path + DataMainId + "\\ImageProperties.xml") == true)
     //       {
     //           try
     //           {
     //               XmlDocument xmlDoc = new XmlDocument();
     //               xmlDoc.Load(path + DataMainId + "\\ImageProperties.xml");

     //               return xmlDoc.ChildNodes[0].Attributes["WIDTH"].Value + "," + xmlDoc.ChildNodes[0].Attributes["HEIGHT"].Value;
     //           }
     //           catch
     //           {
     //               return "";
     //           }
     //       }
     //       else
     //       {
     //           return "";
     //       }
     //   }

     //   protected int GetVersionNo(string MappingTypeID, string zipFileName, int version, int Scale)
     //   {

     //       //DataTable objDT = DBAccess.GetDataSetFromExcuteCommand("select \"VersionNo\" from TBL_DATAMAIN where \"MappingTypeID\" = '" + MappingTypeID + "' and \"ZipFileName\" = '" + zipFileName + "' and \"Version\" = " + version + " and \"Scale\" =" + Scale).Tables[0];


     //       //if (objDT.Rows.Count <= 0)
     //       //{
     //       //    return 1;
     //       //}
     //       //else
     //       //{
     //       //    return Convert.ToInt32(objDT.Rows[0][0]) + 1;
     //       //}
     //       return 1;
     //   }

     //   #region 上传文件
     //   /// <summary>
     //   /// 上传文件
     //   /// </summary>
     //   /// <param name="paramDto">参数DTO</param>
     //   /// <returns>错误列表，Count=0成功</returns>
     //   [Abp.Domain.Uow.UnitOfWork(IsDisabled = true)]
     //   public void UploadFile(UploadFileParamDto paramDto)
     //   {
     //       string LogID = Guid.NewGuid().ToString();
     //       Log LogEntity = null;
     //       try
     //       {
     //           //string LogID = Guid.NewGuid().ToString();



     //           List<string> ErrorInfo = new List<string>();
     //           //   Log LogEntity = null;
     //           ErrorInfo = null;
     //           FileInfo Zipfi = new FileInfo(paramDto.ZipFilePath);
     //           string FileName = Zipfi.Name;
     //           if (Zipfi.Name.LastIndexOf('.') != -1)
     //           {
     //               FileName = Zipfi.Name.Substring(0, Zipfi.Name.LastIndexOf('.'));
     //           }

     //           LogEntity = new Log();
     //           LogEntity.Id = LogID;
     //           LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //           LogEntity.LogType = (int)paramDto.Version;
     //           LogEntity.LogValue = FileName;
     //           LogEntity.LogContent = "正在上传";
     //           LogEntity.CreateTime = DateTime.Now;
     //           LogEntity.Other1 = "0";
     //           _LogRepository.Insert(LogEntity);

     //           if (File.Exists(paramDto.ZipFilePath) == false)
     //           {
     //               _LogRepository.Delete(LogID);
     //               LogEntity = new Log();
     //               LogEntity.Id = LogID;
     //               LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //               LogEntity.LogType = (int)paramDto.Version;
     //               LogEntity.LogValue = FileName;
     //               LogEntity.LogContent = "压缩文件不存在";
     //               LogEntity.CreateTime = DateTime.Now;
     //               LogEntity.Other1 = "0";
     //               _LogRepository.Insert(LogEntity);
     //               return;
     //           }


     //           // 解压缩
     //           string FolderPath = null;
     //           try
     //           {
     //               FolderPath = UnZip(paramDto.ZipFilePath);
     //           }
     //           catch (Exception ex)
     //           {
     //               _LogRepository.Delete(LogID);
     //               LogEntity = new Log();
     //               LogEntity.Id = LogID;
     //               LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //               LogEntity.LogType = (int)paramDto.Version;
     //               LogEntity.LogValue = FileName;
     //               LogEntity.LogContent = ex.Message;
     //               LogEntity.CreateTime = DateTime.Now;
     //               LogEntity.Other1 = "0";
     //               _LogRepository.Insert(LogEntity);
     //               return;
     //           }

     //           if (FolderPath == null)
     //           {
     //               _LogRepository.Delete(LogID);
     //               LogEntity = new Log();
     //               LogEntity.Id = LogID;
     //               LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //               LogEntity.LogType = (int)paramDto.Version;
     //               LogEntity.LogValue = FileName;
     //               LogEntity.LogContent = "压缩文件内没有内容";
     //               LogEntity.CreateTime = DateTime.Now;
     //               LogEntity.Other1 = "0";
     //               _LogRepository.Insert(LogEntity);
     //               return;
     //           }

     //           // 文件夹检查
     //           string FactorFolder = "";

     //           if (CheckFolder(FolderPath, out FactorFolder) == false)
     //           {
     //               _LogRepository.Delete(LogID);
     //               LogEntity = new Log();
     //               LogEntity.Id = LogID;
     //               LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //               LogEntity.LogType = (int)paramDto.Version;
     //               LogEntity.LogValue = FileName;
     //               LogEntity.LogContent = FactorFolder;
     //               LogEntity.CreateTime = DateTime.Now;
     //               LogEntity.Other1 = "0";
     //               _LogRepository.Insert(LogEntity);
     //               return;
     //           }

     //           // 文件转换和检查
     //           ErrorInfo = FileConvertAndCheck(FactorFolder);
     //           if (ErrorInfo.Count > 0)
     //           {
     //               _LogRepository.Delete(LogID);
     //               LogEntity = new Log();
     //               LogEntity.Id = LogID;
     //               LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //               LogEntity.LogType = (int)paramDto.Version;
     //               LogEntity.LogValue = FileName;
     //               LogEntity.LogContent = ErrorInfo[0];
     //               LogEntity.CreateTime = DateTime.Now;
     //               LogEntity.Other1 = "0";
     //               _LogRepository.Insert(LogEntity);
     //               return;
     //           }

     //           // 添加渲染字段


     //           try
     //           {
     //               string[] Files = Directory.GetFiles(FactorFolder + "\\专业图层", "*.shp", SearchOption.AllDirectories);
     //               foreach (string s in Files)
     //               {
     //                   iTelluro.DataTools.ShpToolSet.AddField(s);
     //               }
     //           }
     //           catch (Exception ex)
     //           {
     //               _LogRepository.Delete(LogID);
     //               LogEntity = new Log();
     //               LogEntity.Id = LogID;
     //               LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //               LogEntity.LogType = 1;
     //               LogEntity.LogValue = FileName;
     //               LogEntity.LogContent = "添加渲染字段错误：" + ex.Message;
     //               LogEntity.CreateTime = DateTime.Now;
     //               LogEntity.Other1 = "0";
     //               _LogRepository.Insert(LogEntity);
     //               return;
     //           }



     //           // 保存Zip文件
     //           DataMain DataMainEntity = SaveMainFile(paramDto);
     //           if (DataMainEntity == null)
     //           {
     //               _LogRepository.Delete(LogID);
     //               LogEntity = new Log();
     //               LogEntity.Id = LogID;
     //               LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //               LogEntity.LogType = (int)paramDto.Version;
     //               LogEntity.LogValue = FileName;
     //               LogEntity.LogContent = "保存ZIP文件错误";
     //               LogEntity.CreateTime = DateTime.Now;
     //               LogEntity.Other1 = "0";
     //               _LogRepository.Insert(LogEntity);
     //               return;
     //           }

     //           // 保存目录内文件
     //           ErrorInfo = SaveFolderFile(DataMainEntity.Id, FolderPath);
     //           if (ErrorInfo.Count > 0)
     //           {
     //               _LogRepository.Delete(LogID);
     //               LogEntity = new Log();
     //               LogEntity.Id = LogID;
     //               LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //               LogEntity.LogType = (int)paramDto.Version;
     //               LogEntity.LogValue = FileName;
     //               LogEntity.LogContent = ErrorInfo[0];
     //               LogEntity.CreateTime = DateTime.Now;
     //               LogEntity.Other1 = "0";
     //               _LogRepository.Insert(LogEntity);
     //               return;
     //           }

     //           _LogRepository.Delete(LogID);
     //           LogEntity = new Log();
     //           LogEntity.Id = LogID;
     //           LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //           LogEntity.LogType = (int)paramDto.Version;
     //           LogEntity.LogValue = FileName;
     //           LogEntity.LogContent = "文件上传成功";
     //           LogEntity.CreateTime = DateTime.Now;
     //           LogEntity.Other1 = "0";
     //           _LogRepository.Insert(LogEntity);
     //       }
     //       catch (Exception ex)
     //       {
     //           _LogRepository.Delete(LogID);
     //           LogEntity = new Log();
     //           LogEntity.Id = LogID;
     //           LogEntity.LogKey = paramDto.Scale.Value.ToString();
     //           LogEntity.LogType = (int)paramDto.Version;
     //           LogEntity.LogValue = new FileInfo(paramDto.ZipFilePath).Name;
     //           LogEntity.LogContent = ex.Message;
     //           LogEntity.CreateTime = DateTime.Now;
     //           LogEntity.Other1 = "0";
     //           _LogRepository.Insert(LogEntity);
     //           return;

     //       }
     //   }

     //   /// <summary>
     //   /// 文件转换和检查
     //   /// </summary>
     //   /// <param name="FolderPath">要素文件夹中路径</param>
     //   /// <returns>错误信息</returns>
     //   public List<string> FileConvertAndCheck(string FolderPath)
     //   {
     //       List<string> ErrorInfo = new List<string>();

     //   //    string[] Files = Directory.GetFiles(FolderPath + "\\专业图层", "*.*", SearchOption.AllDirectories);
     //   //    ConvertResult result = null;

     //   //    // 格式转换
     //   //    try
     //   //    {
     //   //        List<ConvertFileList> FCFileList = new List<ConvertFileList>();
     //   //        foreach (string s in Files)
     //   //        {
     //   //            FileInfo fi = new FileInfo(s);
     //   //            if (fi.Extension.ToUpper() == ".WP" || fi.Extension.ToUpper() == ".WL" || fi.Extension.ToUpper() == ".WT")
     //   //            {
     //   //                ConvertFileList cf = new ConvertFileList();
     //   //                cf.ID = Guid.NewGuid().ToString();
     //   //                cf.LogicFileName = fi.Name;
     //   //                cf.PhysicsFilePath = s;
     //   //                cf.ConvertResult = 0;
     //   //                cf.ConvertFilePath = "";
     //   //                cf.ConvertMsg = "";
     //   //                cf.SrcCoordName = "";
     //   //                cf.FileType =(int)DataFileType.FormatConvert;

     //   //                FCFileList.Add(cf);
     //   //            }
     //   //        }
     //   //        if (FCFileList.Count <= 0)
     //   //        {
     //   //            return new List<string>() { "格式转换：未找到图层文件" };
     //   //        }
     //   //        result = DataConvert.DataConvert(FCFileList, "", "", false);
     //   //    }
     //   //    catch (Exception ex)
     //   //    {
     //   //        ErrorInfo.Add("格式转换失败： " + ex.Message);
     //   //        return ErrorInfo;
     //   //    }

     //   //    // 坐标转换
     //   //    try
     //   //    {
     //   //        List<ConvertFileList> CCFileList = new List<ConvertFileList>();
     //   //        foreach (var f in result.fileList)
     //   //        {
     //   //            if (f.ConvertResult == 0)
     //   //            {
     //   //                ErrorInfo.Add("格式转换失败： " + f.ConvertMsg + "  文件：" + f.LogicFileName);
     //   //                return ErrorInfo;
     //   //            }

     //   //            FileInfo fi = new FileInfo(f.PhysicsFilePath);

     //   //            ConvertFileList cf = new ConvertFileList();
     //   //            cf.ID = Guid.NewGuid().ToString();
     //   //            cf.LogicFileName = fi.Name;
     //   //            cf.PhysicsFilePath = f.ConvertFilePath + ".shp";
     //   //            cf.ConvertResult = 0;
     //   //            cf.ConvertFilePath = fi.DirectoryName;
     //   //            cf.ConvertMsg = "";
     //   //            cf.SrcCoordName = "";
     //   //            cf.FileType = DataFileType.CoordinateConvert;

     //   //            CCFileList.Add(cf);
     //   //        }

     //   //        result = DataConvert.DataConvert(CCFileList, "", "Xian 1980", false);

     //   //    }
     //   //    catch (Exception ex)
     //   //    {
     //   //        ErrorInfo.Add("坐标转换失败： " + ex.Message);
     //   //        return ErrorInfo;
     //   //    }

     //   //    #region 投影转换
     //   //    // 投影转换
     //   //    try
     //   //    {
     //   //        List<ConvertFileList> PFileList = new List<ConvertFileList>();

     //   //        foreach (var f in result.fileList)
     //   //        {
     //   //            FileInfo fi = new FileInfo(f.ConvertFilePath);

     //   //            ConvertFileList cf = new ConvertFileList();
     //   //            cf.ID = Guid.NewGuid().ToString();
     //   //            cf.LogicFileName = fi.Name;
     //   //            cf.PhysicsFilePath = f.ConvertFilePath;
     //   //            cf.ConvertResult = 0;
     //   //            cf.ConvertFilePath = FolderPath;
     //   //            cf.ConvertMsg = "";
     //   //            cf.FileType = DataFileType.Projection;

     //   //            PFileList.Add(cf);
     //   //        }

     //   //        result = DataConvert.DataConvert(PFileList, "", "", false);
     //   //    }
     //   //    catch (Exception ex)
     //   //    {
     //   //        ErrorInfo.Add("投影转换失败： " + ex.Message);
     //   //        return ErrorInfo;
     //   //    }
     //   //    #endregion

     //   //    //// 属性检查
     //   //    //List<string> CheckFile = new List<string>();
     //   //    //foreach (var f in result.fileList)
     //   //    //{
     //   //    //    if (f.ConvertResult == 0)
     //   //    //    {
     //   //    //        ErrorInfo.Add("坐标转换失败： " + f.ConvertMsg + "  文件：" + f.PhysicsFilePath);
     //   //    //        return ErrorInfo;
     //   //    //    }

     //   //    //    CheckFile.Add(f.ConvertFilePath);
     //   //    //}
     //   //    //DataCheckResult dcResult = DataConvert.DataCheck(CheckFile, false);

     //   //    //foreach (var cl in dcResult.CheckInfoList)
     //   //    //{
     //   //    //    if (cl.Log.Count > 0)
     //   //    //    {
     //   //    //        FileInfo ff = new FileInfo(cl.FileName);
     //   //    //        ErrorInfo.Add(ff.Name + " " + cl.Log[0]);
     //   //    //    }
     //   //    //}

     //       return ErrorInfo;

     //   }

     ////   [Abp.Domain.Uow.UnitOfWork(IsDisabled = true)]
     //   protected DataMain SaveMainFile(UploadFileParamDto paramDto)
     //   {
     //       FileInfo mainFi = new FileInfo(paramDto.ZipFilePath);

     //       string FileName = mainFi.Name.Substring(0, mainFi.Name.LastIndexOf('.'));
     //       if (paramDto.Version.Value == 1)
     //       {
     //           FileName += "[验收版]_";
     //       }
     //       else
     //       {
     //           FileName += "[最终版]_";
     //       }
     //       FileName += DateTime.Now.ToString("yyyyMMddHHmmss");

     //       DataMain DataMainEntity = new DataMain();
     //       DataMainEntity.Id = Guid.NewGuid().ToString();
     //       DataMainEntity.MappingTypeID = paramDto.GeologyMappingTypeID;
     //       DataMainEntity.Scale = paramDto.Scale;
     //       DataMainEntity.StorageTime = DateTime.Now;
     //       DataMainEntity.Creater = "";
     //       DataMainEntity.FileSize = mainFi.Length;
     //       DataMainEntity.ZipFileName = mainFi.Name;
     //       DataMainEntity.FilePath = FileName + ".zip";//ReadToBytes(paramDto.ZipFilePath);
     //       DataMainEntity.Version = paramDto.Version.Value;
     //       DataMainEntity.VersionNo = GetVersionNo(paramDto.GeologyMappingTypeID, mainFi.Name, paramDto.Version.Value, paramDto.Scale.Value);
     //       var dir = ConfigurationManager.AppSettings["DataMainFile"];
     //       if (!Directory.Exists(dir))
     //       {
     //           Directory.CreateDirectory(dir);
     //       }
     //       File.Copy(paramDto.ZipFilePath, ConfigurationManager.AppSettings["DataMainFile"] + FileName + ".zip");
     //       //if (DataMainEntity.FileData == null)
     //       //{
     //       //    return null;
     //       //}
     //       //else
     //       //{
     //       InsertDataMain(DataMainEntity);
     //       return DataMainEntity;
     //     //  }
     //   }

     //   [Abp.Domain.Uow.UnitOfWork(IsDisabled = true)]
     //   protected List<string> SaveFolderFile(string DataMainID, string FolderPath)
     //   {
     //       //TODO:保存文件夹里面的文件
     //       List<string> ErrorInfo = new List<string>();

     //       //if (FolderPath[FolderPath.Length - 1] != '\\')
     //       //{
     //       //    FolderPath += "\\";
     //       //}

     //       //List<string> FileList = Directory.EnumerateFiles(FolderPath, "*.*", SearchOption.AllDirectories).Where(p => !p.EndsWith(".log") && !p.EndsWith("~")).ToList();
     //       //byte[] ImgBytes = null;
     //       //string MetaDataID = null;



     //       //foreach (string s in FileList)
     //       //{
     //       //    FileInfo fi = new FileInfo(s);
     //       //    string FolderLevel = s.Replace(FolderPath, "").Replace(fi.Name, "");
     //       //    if (FolderLevel.ToString().Length > 0 && FolderLevel[FolderLevel.Length - 1] == '\\')
     //       //    {
     //       //        FolderLevel = FolderLevel.Remove(FolderLevel.Length - 1);
     //       //    }

     //       //    //string saveFile = s;
     //       //    //string fileName = fi.Name;
     //       //    //string Ext = fi.Extension;


     //       //    if (FolderLevel.IndexOf("栅格图") != -1 && fi.Name.ToUpper().IndexOf("JPG") != -1)
     //       //    {
     //       //        #region 栅格图
     //       //        string tmpFilder = ConfigurationManager.AppSettings["ConvertExprotPath"] + Guid.NewGuid().ToString();
     //       //        Directory.CreateDirectory(tmpFilder);

     //       //        // 转换之后保存 到 元数据 里
     //       //        Bitmap ThumbnailImg = iTelluro.DataTools.Utility.Img.ImgExport.ExportThumbnail(s, 800, 600);
     //       //        if (ThumbnailImg != null) // 压缩图片成功
     //       //        {
     //       //            ThumbnailImg.Save(tmpFilder + "\\" + fi.Name);
     //       //            ImgBytes = ReadToBytes(tmpFilder + "\\" + fi.Name);
     //       //        }
     //       //        if (ImgBytes != null && MetaDataID != null) // 元数据已导入
     //       //        {
     //       //            // 更新静态浏览图为压缩图
     //       //        }

     //       //        #endregion
     //       //    }

     //       //    DataManageFile DataManageFileEntity = new DataManageFile();
     //       //    DataManageFileEntity.Id = Guid.NewGuid().ToString();
     //       //    DataManageFileEntity.MainID = DataMainID;
     //       //    DataManageFileEntity.StorageTime = DateTime.Now;
     //       //    DataManageFileEntity.FileName = fi.Name;
     //       //    DataManageFileEntity.Ext = fi.Extension;
     //       //    DataManageFileEntity.FileSize = (int)fi.Length;
     //       //    DataManageFileEntity.FolderName = FolderLevel;
     //       //    DataManageFileEntity.OrderBy = 9;
     //       //    DataManageFileEntity.FileData = ReadToBytes(s);
     //       //    if (DataManageFileEntity.FileData != null)
     //       //    {
     //       //        if (fi.Extension.ToUpper() == ".SHP")
     //       //        {
     //       //            DataManageFileEntity.OrderBy = GetShpSn(s);
     //       //        }


     //       //        InsertDataManageFile(DataManageFileEntity);
     //       //        if (fi.Extension.ToUpper() == ".DBF")
     //       //        {
     //       //            //List<string> list = SaveLayer(s, DataMainID, DataManageFileEntity.Id);
     //       //            //if (list.Count > 0)
     //       //            //{
     //       //            //    foreach (string m in list)
     //       //            //    {
     //       //            //        ErrorInfo.Add(m);
     //       //            //    }
     //       //            //}
     //       //        }
     //       //        else if (ConfigurationManager.AppSettings["ImageFile"].ToUpper().IndexOf(fi.Extension.ToUpper()) != -1)
     //       //        {
     //       //            try
     //       //            {
     //       //                Image img = Image.FromFile(s);
     //       //                if (img.Width > 1920 || img.Height > 1080)
     //       //                {
     //       //                    CreateThumbnailImg(s, DataManageFileEntity.Id);
     //       //                }
     //       //                else
     //       //                {
     //       //                    if (Directory.Exists(ConfigurationManager.AppSettings["Thumbnail"] + DataManageFileEntity.Id + "\\") == false)
     //       //                    {
     //       //                        Directory.CreateDirectory(ConfigurationManager.AppSettings["Thumbnail"] + DataManageFileEntity.Id + "\\");
     //       //                    }

     //       //                    if (img.Width >= 256 || img.Height >= 256)
     //       //                    {
     //       //                        ZoomifyProcess zp = new ZoomifyProcess(
     //       //                                                        s,
     //       //                                                        ConfigurationManager.AppSettings["Thumbnail"] + DataManageFileEntity.Id + "\\"
     //       //                                                        );
     //       //                        zp.Process();

     //       //                        ThumbnailProcess tp = new ThumbnailProcess(
     //       //                                                s,
     //       //                                                ConfigurationManager.AppSettings["Thumbnail"] + DataManageFileEntity.Id + "\\",
     //       //                                                new int[] { 1600, 1024, 512, 256 }
     //       //                                                );
     //       //                        tp.Process();
     //       //                    }
     //       //                    else
     //       //                    {
     //       //                        Directory.CreateDirectory(ConfigurationManager.AppSettings["Thumbnail"] + DataManageFileEntity.Id + "\\TileGroup0\\");

     //       //                        //string FName = fi.Name.Substring(0, fi.Name.IndexOf('.')) + "-" + img.Width;

     //       //                        File.Copy(s, ConfigurationManager.AppSettings["Thumbnail"] + DataManageFileEntity.Id + "\\" + fi.Name.Substring(0, fi.Name.IndexOf('.')) + "-" + img.Width + ".jpg");
     //       //                        File.Copy(s, ConfigurationManager.AppSettings["Thumbnail"] + DataManageFileEntity.Id + "\\TileGroup0\\0-0-0.jpg");

     //       //                        StreamWriter swXML = new StreamWriter(ConfigurationManager.AppSettings["Thumbnail"] + DataManageFileEntity.Id + "\\ImageProperties.xml");
     //       //                        swXML.Write("<IMAGE_PROPERTIES WIDTH=\"" + img.Width + "\" HEIGHT=\"" + img.Height + "\" NUMTILES=\"17\" NUMIMAGES=\"1\" VERSION=\"1.8\" TILESIZE=\"256\"/>");
     //       //                        swXML.Close();
     //       //                    }
     //       //                }
     //       //            }
     //       //            catch
     //       //            {
     //       //                CreateThumbnailImg(s, DataManageFileEntity.Id);
     //       //            }



     //       //        }
     //       //        else if (FolderLevel.IndexOf("元数据") != -1 && fi.Extension.ToUpper() == ".XML")
     //       //        {
     //       //            // 保存元文件
     //       //            try
     //       //            {

     //       //                IMetaDataAppService objMetaData = new MetaDataAppService(_DataMainRepository, _MetaDataRepository, _IdentificationRepository, _CitationRepository, _GeoBndBoxRepository, _TempExtentRepository, _VerExtentRepository,
     //       //            _IdPoCRepository, _KeyWordsRepository, _BrowGraphRepository, _ConstsRepository, _FormatRepository, _MaintInfoRepository, _DqInfoRepository, _RefSysInfoRepository, _DistInfoRepository, _ConInfoRepository, _ContactRepository,
     //       //            _ContactpeopleRepository, _GeologyMappingType);
     //       //                try
     //       //                {
     //       //                    MetaDataEntityAssembly EntityAssembly = objMetaData.LoadMetaDataFromXML(s, ImgBytes);
     //       //                    EntityAssembly.MetaDataEntity.MainID = DataMainID;
     //       //                    EntityAssembly.MetaDataEntity.FileID = DataManageFileEntity.Id;
     //       //                    MetaDataID = EntityAssembly.MetaDataEntity.Id;

     //       //                    objMetaData.InsertMetaData(EntityAssembly);
     //       //                }
     //       //                catch (Exception ex)
     //       //                {
     //       //                    throw new Exception("解析[" + s + "]发生错误，请仔细检查！详细信息：" + ex.Message);
     //       //                }

     //       //            }
     //       //            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
     //       //            {
     //       //                return new List<string>() { "保存元数据信息到数据库失败：" + ex.Message };
     //       //            }
     //       //        }
     //       //    }
     //       //    else
     //       //    {
     //       //        ErrorInfo.Add("读取文件错误：" + s);
     //       //        break;
     //       //    }
     //       //}

     //       return ErrorInfo;
     //   }

     //   protected int GetShpSn(string Shp)
     //   {
     //       FileInfo fi = new FileInfo(Shp);

     //       string FileName = fi.DirectoryName + "\\" + fi.Name.Substring(0, fi.Name.IndexOf('.'));

     //       if (File.Exists(FileName + ".WT") == true)
     //       {
     //           return 1;
     //       }
     //       else if (File.Exists(FileName + ".WL") == true)
     //       {
     //           return 2;
     //       }
     //       else if (File.Exists(FileName + ".WP") == true)
     //       {
     //           return 3;
     //       }
     //       else
     //       {
     //           return 9;
     //       }

     //   }
     //   protected void CreateThumbnailImg(string filePath, string FolderID)
     //   {
     //       string tmpFilder = ConfigurationManager.AppSettings["ConvertExprotPath"] + Guid.NewGuid().ToString();
     //       Directory.CreateDirectory(tmpFilder);

     //       FileInfo fi = new FileInfo(filePath);

     //       Bitmap ThumbnailImg = iTelluro.DataTools.Utility.Img.ImgExport.ExportThumbnail(filePath, 1920, 1080);
     //       if (ThumbnailImg != null) // 压缩图片成功
     //       {
     //           ThumbnailImg.Save(tmpFilder + "\\" + fi.Name);

     //           if (Directory.Exists(ConfigurationManager.AppSettings["Thumbnail"] + FolderID + "\\") == false)
     //           {
     //               Directory.CreateDirectory(ConfigurationManager.AppSettings["Thumbnail"] + FolderID + "\\");
     //           }

     //           string inputFile = tmpFilder + "\\" + fi.Name;
     //           string output = ConfigurationManager.AppSettings["Thumbnail"] + FolderID + "\\";
     //           ZoomifyProcess zp = new ZoomifyProcess(
     //                                           inputFile,
     //                                           output
     //                                           );
     //           zp.Process();

     //           ThumbnailProcess tp = new ThumbnailProcess(
     //                                   inputFile,
     //                                   output,
     //                                   new int[] { 1600, 1024, 512, 256 }
     //                                   );
     //           tp.Process();
     //       }
     //   }

     //   protected List<string> SaveLayer(string FilePath, string MainID, string FileID)
     //   {
     //       InfoEarthFrame.Data.IDatabase DBAccess = InfoEarthFrame.Data.Factory.GetDBAccess(ConfigurationManager.ConnectionStrings["Default"].ConnectionString, InfoEarthFrame.Data.AccessDBType.Oracle);

     //       List<string> ErrorInfo = new List<string>();

     //       string temPath = Path.GetTempPath();

     //       string conStr = "provider=microsoft.jet.oledb.4.0;data source=" + temPath + ";extended properties='dbase 5.0;hdr=false';OLE DB Services=-4";
     //       string cmdStr = "select * from temp.dbf";


     //       OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);

     //       try
     //       {
     //           conn.Open();

     //           File.Copy(FilePath, temPath + "temp.dbf", true);

     //           FileInfo fi = new FileInfo(FilePath);

     //           // 读取数据
     //           OleDbConnection con = new OleDbConnection(conStr);
     //           OleDbDataAdapter adapter = new OleDbDataAdapter(cmdStr, con);
     //           DataTable table = new DataTable();
     //           adapter.Fill(table);

     //           adapter.Dispose();
     //           con.Close();
     //           con.Dispose();

     //           if (table.Rows.Count <= 0)
     //           {
     //               return ErrorInfo;
     //           }

     //           // 判断表是否存在
     //           string Number = fi.Name.Substring(0, fi.Name.IndexOf('.'));
     //           Number = Number.Substring(Number.Length - 6);
     //           DataTable objDT = DBAccess.GetDataSetFromExcuteCommand("select 1 from user_tables where TABLE_NAME = 'TLB_SP_" + Number + "'").Tables[0];
     //           if (objDT == null || objDT.Rows.Count < 1 || objDT.Rows[0][0].ToString() != "1")
     //           {
     //               return ErrorInfo;
     //           }

     //           List<ColumnRealtion> Realtion = LoadColumns(table, Number);
     //           if (Realtion == null)
     //           {
     //               return ErrorInfo;
     //           }


     //           List<string> SQLList = new List<string>();
     //           for (int i = 0; i < table.Rows.Count; i++)
     //           {
     //               string BaseSQL = "insert into TLB_SP_" + Number + " (\"Id\",{0} \"MainID\", \"FileID\") values('" + Guid.NewGuid().ToString() + "',{1}'" + MainID + "','" + FileID + "')";
     //               string columns = "";
     //               string value = "";

     //               for (int r = 0; r < Realtion.Count; r++)
     //               {
     //                   columns += Realtion[r].ColumnName + ",";

     //                   if (Realtion[r].Index == -1)
     //                   {
     //                       value += "null,";
     //                   }
     //                   else
     //                   {
     //                       if (Realtion[r].IsNumber == true)
     //                       {
     //                           value += "'" + NumberFilter(table.Rows[i][Realtion[r].Index].ToString()) + "',";
     //                       }
     //                       else
     //                       {
     //                           value += "'" + table.Rows[i][Realtion[r].Index].ToString().Replace("'", "''") + "',";
     //                       }
     //                   }
     //               }
     //               OracleCommand cmd = new OracleCommand();
     //               cmd.Connection = conn;
     //               cmd.CommandType = CommandType.Text;
     //               cmd.CommandText = string.Format(BaseSQL, columns, value);

     //               int n = cmd.ExecuteNonQuery();

     //               //SQLList.Add(string.Format(BaseSQL, columns, value));
     //           }

     //           //if (DBAccess.ExcuteTransaction(SQLList) == false)
     //           //{
     //           //    ErrorInfo.Add("保存错误" );
     //           //}
     //           conn.Close();
     //       }
     //       catch (Exception ex)
     //       {
     //           ErrorInfo.Add(ex.Message);
     //           if (conn.State == ConnectionState.Open)
     //           {
     //               conn.Close();
     //           }
     //       }

     //       return ErrorInfo;
     //   }

     //   protected List<ColumnRealtion> LoadColumns(DataTable table, string DataTableNumber)
     //   {
     //       InfoEarthFrame.Data.IDatabase DBAccess = InfoEarthFrame.Data.Factory.GetDBAccess(ConfigurationManager.ConnectionStrings["Default"].ConnectionString, InfoEarthFrame.Data.AccessDBType.Oracle);

     //       DataTable objDT = DBAccess.GetDataSetFromExcuteCommand("select column_name, data_type  from user_tab_columns where  TABLE_NAME = 'TLB_SP_" + DataTableNumber + "'").Tables[0];
     //       if (objDT == null || objDT.Rows.Count <= 0)
     //       {
     //           return null;
     //       }

     //       List<ColumnRealtion> Realtion = new List<ColumnRealtion>();

     //       for (int i = 0; i < objDT.Rows.Count; i++)
     //       {
     //           if (objDT.Rows[i]["column_Name"].ToString().ToUpper() == "ID" || objDT.Rows[i]["column_Name"].ToString().ToUpper() == "MAINID" ||
     //               objDT.Rows[i]["column_Name"].ToString().ToUpper() == "FILEID")
     //           {
     //               continue;
     //           }

     //           bool bNumber = false;
     //           int index = -1;
     //           for (int j = 0; j < table.Columns.Count; j++)
     //           {
     //               if (objDT.Rows[i]["column_Name"].ToString().ToUpper() == table.Columns[j].ColumnName.ToUpper())
     //               {
     //                   if (objDT.Rows[i]["data_type"].ToString().ToUpper() == "NUMBER")
     //                   {
     //                       bNumber = true;
     //                   }
     //                   index = j;
     //                   break;
     //               }
     //           }



     //           Realtion.Add(new ColumnRealtion(objDT.Rows[i]["column_Name"].ToString(), bNumber, index));
     //       }

     //       return Realtion;
     //   }

     //   /// <summary>
     //   /// 检查文件夹，返回要素文件夹
     //   /// </summary>
     //   /// <param name="FolderPath">检测路径</param>
     //   /// <returns>要素文件夹路径，null为文件夹缺失</returns>
     //   protected bool CheckFolder(string FolderPath, out string FactorFolder)
     //   {
     //       FactorFolder = "";
     //       List<string> list = Directory.EnumerateDirectories(FolderPath, "1文档", SearchOption.AllDirectories).ToList();
     //       try
     //       {
     //           if (list.Count <= 0)
     //           {
     //               FactorFolder = "1文档 目录不存在";
     //               return false;
     //           }

     //           list = Directory.EnumerateDirectories(FolderPath, "2系统库", SearchOption.AllDirectories).ToList();
     //           if (list.Count <= 0)
     //           {
     //               FactorFolder = "2系统库 目录不存在";
     //               return false;
     //           }

     //           list = Directory.EnumerateDirectories(FolderPath, "3栅格图", SearchOption.AllDirectories).ToList();
     //           if (list.Count <= 0)
     //           {
     //               FactorFolder = "3栅格图 目录不存在";
     //               return false;
     //           }
     //           list = Directory.EnumerateDirectories(FolderPath, "4说明书", SearchOption.AllDirectories).ToList();
     //           if (list.Count <= 0)
     //           {
     //               FactorFolder = "4说明书 目录不存在";
     //               return false;
     //           }
     //           list = Directory.EnumerateDirectories(FolderPath, "5元数据", SearchOption.AllDirectories).ToList();
     //           if (list.Count <= 0)
     //           {
     //               FactorFolder = "5元数据 目录不存在";
     //               return false;
     //           }
     //           list = Directory.EnumerateDirectories(FolderPath, "7制图文件", SearchOption.AllDirectories).ToList();
     //           if (list.Count <= 0)
     //           {
     //               FactorFolder = "7制图文件 目录不存在";
     //               return false;
     //           }

     //           list = Directory.EnumerateDirectories(FolderPath + "\\6要素类文件", "专业图层", SearchOption.AllDirectories).ToList();
     //           if (list.Count <= 0)
     //           {
     //               FactorFolder = "6要素类文件\\专业图层 目录不存在";
     //               return false;
     //           }

     //           list = Directory.EnumerateDirectories(FolderPath, "6要素类文件", SearchOption.AllDirectories).ToList();
     //           if (list.Count <= 0)
     //           {
     //               FactorFolder = "6要素类文件 目录不存在";
     //               return false;
     //           }
     //       }
     //       catch (Exception ex)
     //       {
     //           FactorFolder = "上传的压缩包文件层级错误，或者命名不规范，请仔细检查，服务端返回信息:" + ex.Message;
     //           return false;
     //       }

     //       FactorFolder = list[0];
     //       return true;
     //       //foreach(string s in list)
     //       //{
     //       //    if(s.IndexOf("要素") != -1)
     //       //    {
     //       //        return s;
     //       //    }
     //       //}

     //       //foreach (string s in list)
     //       //{
     //       //    List<string> cList = Directory.EnumerateDirectories(s).ToList();
     //       //    foreach (string c in cList)
     //       //    {
     //       //        if (c.IndexOf("要素") != -1)
     //       //        {
     //       //            return c;
     //       //        }
     //       //    }
     //       //}

     //       //return null;
     //   }

     //   /// <summary>
     //   /// 解压缩
     //   /// </summary>
     //   /// <param name="ZipFilePath">压缩文件路径</param>
     //   /// <returns>解压缩后的目录</returns>
     //   protected string UnZip(string ZipFilePath)
     //   {
     //       string FolderPath = ConfigurationManager.AppSettings["ConvertExprotPath"] + Guid.NewGuid().ToString();
     //       System.IO.Directory.CreateDirectory(FolderPath);
     //       ZipFile.ExtractToDirectory(ZipFilePath, FolderPath);
     //       if (Directory.EnumerateDirectories(FolderPath).ToList().Count == 1)
     //       {
     //           return Directory.EnumerateDirectories(FolderPath).ToList()[0];
     //       }
     //       else if (Directory.EnumerateDirectories(FolderPath).ToList().Count <= 0)
     //       {
     //           return null;
     //       }
     //       else
     //       {
     //           return FolderPath;
     //       }
     //   }

     //   protected string NumberFilter(string number)
     //   {
     //       string ret = "";
     //       int dot = 0;
     //       foreach (char c in number)
     //       {
     //           if (c >= '0' && c <= '9')
     //           {
     //               ret += c;
     //           }
     //           else
     //           {
     //               if (c == '.' && dot == 0)
     //               {
     //                   dot = 1;
     //                   ret += c;
     //               }

     //               else
     //               {
     //                   break;
     //               }
     //           }
     //       }
     //       return ret;
     //   }

     //   protected byte[] ReadToBytes(string FilePath)
     //   {
     //       try
     //       {
     //           FileInfo fi = new FileInfo(FilePath);
     //           FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
     //           byte[] infbytes = new byte[(int)fi.Length];
     //           fs.Read(infbytes, 0, infbytes.Length);
     //           fs.Close();
     //           return infbytes;
     //       }
     //       catch
     //       {
     //           return null;
     //       }
     //   }

     //   public string GetSP_XXX(DataTable SPDataTable, DataRow dr)
     //   {
     //       string FileName = dr["FileName"].ToString();

     //       if (FileName.Length < 10)
     //       {
     //           return "";
     //       }

     //       int dotIndex = FileName.IndexOf('.');
     //       if (dotIndex == -1)
     //       {
     //           return "";
     //       }

     //       if (FileName.Substring(dotIndex + 1).ToUpper() != "SHP")
     //       {
     //           return "";
     //       }

     //       string SPName = FileName.Substring(dotIndex - 6, 6);

     //       for (int i = 0; i < SPDataTable.Rows.Count; i++)
     //       {
     //           if (SPDataTable.Rows[i]["TABLE_NAME"].ToString().ToUpper() == "TLB_SP_" + SPName)
     //           {
     //               return SPName;
     //           }
     //       }

     //       return "";
     //   }

     //   private PropertyInfo[] GetProperts(Type type)
     //   {
     //       object obj = Activator.CreateInstance(type);
     //       PropertyInfo[] propers = type.GetProperties(BindingFlags.Public | BindingFlags.Current);
     //       return propers;
     //   }

     //   private bool RunExe(string exeArgs)
     //   {
     //       Gdal.SetConfigOption("SHAPE_ENCODING", "");//支持中文
     //       Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO"); //指定路径名非U8编码 
     //       string transExePath = ConfigurationManager.AppSettings["Translate"];
     //       if (File.Exists(transExePath) == false)
     //       {
     //           return false;
     //       }
     //       Process process = new Process();
     //       ProcessStartInfo psStartInfo = new ProcessStartInfo(transExePath);
     //       psStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
     //       process.StartInfo = psStartInfo;
     //       psStartInfo.Arguments = exeArgs;
     //       process.Start();
     //       while (!process.HasExited)
     //       {
     //           System.Threading.Thread.Sleep(500);
     //       }
     //       if (process.ExitCode == 0)
     //       {
     //           return true;
     //       }
     //       else
     //       {
     //           return false;
     //       }
     //   }
     //   #endregion

     //   #region 统计
     //   /// <summary>
     //   /// 统计地质环境概况（线图）
     //   /// </summary>
     //   /// <returns>   
     //   /// 字符串List ,第1行年月，第2行完成，第3行发布 如
     //   /// 2016-11，2016-12，2017-01
     //   /// 5,2,8
     //   /// 2,0,1
     //   /// </returns>
     //   public List<string> SummarizeLine()
     //   {
     //       try
     //       {
     //           DataTable objDT = AllStat(DateTime.Now.AddMonths(-6), DateTime.Now);

     //           string Year = "";
     //           string AllItem = "";
     //           string ReleaseItem = "";

     //           for (int i = 0; i < objDT.Rows.Count; i++)
     //           {
     //               Year += objDT.Rows[i]["Year"].ToString() + "-" + objDT.Rows[i]["Month"].ToString() + ",";
     //               AllItem += objDT.Rows[i]["AllItem"] == null || objDT.Rows[i]["AllItem"].ToString() == "" ? "0" : objDT.Rows[i]["AllItem"].ToString() + ",";
     //               ReleaseItem += objDT.Rows[i]["ReleaseItem"] == null || objDT.Rows[i]["ReleaseItem"].ToString() == "" ? "0" : objDT.Rows[i]["ReleaseItem"].ToString() + ",";
     //           }

     //           if (Year.Length > 1)
     //           {
     //               Year = Year.Substring(0, Year.Length - 1);
     //           }

     //           if (AllItem.Length > 1)
     //           {
     //               AllItem = AllItem.Substring(0, AllItem.Length - 1);
     //           }

     //           if (ReleaseItem.Length > 1)
     //           {
     //               ReleaseItem = ReleaseItem.Substring(0, ReleaseItem.Length - 1);
     //           }

     //           return new List<string> { Year, AllItem, ReleaseItem };
     //       }
     //       catch
     //       {
     //           return null;
     //       }
     //   }

     //   /// <summary>
     //   /// 全国（概况）柱状图
     //   /// </summary>
     //   /// <returns>
     //   /// 字符串List ,第1行验收版，第2行最终版 如
     //   /// 地质灾害类:4,地下水类:0,矿山地质环境类:5,地质遗迹类:1,地质环境条件类:9
     //   /// 地质灾害类:2,地下水类:1,矿山地质环境类:0,地质遗迹类:0,地质环境条件类:6
     //   /// </returns>
     //   public List<string> CountrySummarize()
     //   {
     //       return SummarizeStat("702fd19f-4dc2-4490-a63b-bcd15593d4c7");
     //   }

     //   /// <summary>
     //   /// 区域（概况）柱状图
     //   /// </summary>
     //   /// <returns>
     //   /// 字符串List ,第1行验收版，第2行最终版 如
     //   /// 地质灾害类:4,地下水类:0,矿山地质环境类:5,地质遗迹类:1,地质环境条件类:9
     //   /// 地质灾害类:2,地下水类:1,矿山地质环境类:0,地质遗迹类:0,地质环境条件类:6
     //   /// </returns>
     //   public List<string> AreaSummarize()
     //   {
     //       return SummarizeStat("8ba5c7e9-6196-4bbd-a06b-39a0239c636e");
     //   }

     //   /// <summary>
     //   /// 省域（概况）柱状图
     //   /// </summary>
     //   /// <returns>
     //   /// 字符串List ,第1行验收版，第2行最终版 如
     //   /// 地质灾害类:4,地下水类:0,矿山地质环境类:5,地质遗迹类:1,地质环境条件类:9
     //   /// 地质灾害类:2,地下水类:1,矿山地质环境类:0,地质遗迹类:0,地质环境条件类:6
     //   /// </returns>
     //   public List<string> ProvinceSummarize()
     //   {
     //       return AreaTypeStat();
     //   }

     //   /// <summary>
     //   /// 省域（概况）地图
     //   /// </summary>
     //   /// <returns>
     //   /// 字符串List ,第1行各省数量 如
     //   /// 黑龙江省：3：702fd19f-4dc2-4490-a63b-bcd15593d4c7.ade5429f-ff11-438d-bb2b-2af2549c2a2f.068f95b9-0595-4d45-b79e-09a186f08e60，北京市：1：702fd19f-4dc2-4490-a63b-bcd15593d4c7.ade5429f-ff11-438d-bb2b-2af2549c2a2f.068f95b9-0595-4d45-b79e-09a186f08e60
     //   /// </returns>
     //   public List<string> ProvinceMap()
     //   {
     //       string ProvStr = "";
     //       DataTable objDT = AreaStat();
     //       for (int i = 0; i < objDT.Rows.Count; i++)
     //       {
     //           ProvStr += objDT.Rows[i]["ClassName"] + ":";
     //           if (objDT.Rows[i]["STATNUM"] == null || objDT.Rows[i]["STATNUM"].ToString() == "")
     //           {
     //               ProvStr += "0:";
     //           }
     //           else
     //           {
     //               ProvStr += objDT.Rows[i]["STATNUM"] + ":";
     //           }
     //           if (objDT.Rows[i]["PATHS"] == null || objDT.Rows[i]["PATHS"].ToString() == "")
     //           {
     //               ProvStr += "0,";
     //           }
     //           else
     //           {
     //               ProvStr += objDT.Rows[i]["PATHS"] + ",";
     //           }
     //       }

     //       if (ProvStr.Length > 1)
     //       {
     //           ProvStr = ProvStr.Substring(0, ProvStr.Length - 1);
     //       }

     //       return new List<string>() { ProvStr };
     //   }

     //   /// <summary>
     //   /// 省的详细信息
     //   /// </summary>
     //   /// <param name="Paths">路径，ProvinceMap返回值里获得</param>
     //   /// <returns>
     //   /// 字符串List ,第1行验收版，第2行最终版 如
     //   /// 地质灾害类:4,地下水类:0,矿山地质环境类:5,地质遗迹类:1,地质环境条件类:9
     //   /// 地质灾害类:2,地下水类:1,矿山地质环境类:0,地质遗迹类:0,地质环境条件类:6
     //   /// </returns>
     //   public List<string> ProvinceInfo(string Paths)
     //   {
     //       if (Paths == "0")
     //       {
     //           return new List<string>() { "地质环境条件类:0,地质灾害类:0,地下水类:0,矿山地质环境类:0,地质遗迹类:0", "地质环境条件类:0,地质灾害类:0,地下水类:0,矿山地质环境类:0,地质遗迹类:0" };
     //       }
     //       string[] Ids = Paths.Split('.');

     //       return SummarizeStat(Ids[Ids.Length - 1], 110);
     //   }

     //   //todo:SQL语句不能跨SQL Server平台
     //   protected DataTable AllStat(DateTime sDT, DateTime eDT)
     //   {
     //       string SQL = "select a.*, b.ReleaseItem from (";
     //       SQL += " select Year, Month, count(AllItem) AllItem from( select to_char(\"StorageTime\", 'yyyy')  Year,  to_char(\"StorageTime\", 'mm')  Month, Max(\"Id\") AllItem  from tbl_datamain group by \"MappingTypeID\",  \"ZipFileName\", \"Scale\",  to_char(\"StorageTime\", 'yyyy'),  to_char(\"StorageTime\", 'mm')) group by Year, Month";
     //       SQL += " ) a left join (";
     //       SQL += " select Year, Month, count(ReleaseItem) ReleaseItem from( select to_char(\"StorageTime\", 'yyyy')  Year, to_char(\"StorageTime\", 'mm')  Month, Max(\"Id\") ReleaseItem from tbl_datamain where \"ReleaseTime\" is not null group by \"MappingTypeID\", \"ZipFileName\", \"Scale\",  to_char(\"StorageTime\", 'yyyy'),  to_char(\"StorageTime\", 'mm')) group by Year, Month";
     //       SQL += " ) b on a.Year=b.Year AND a.Month=b.Month ";
     //       if (sDT.Year == eDT.Year)
     //       {
     //           SQL += " where a.Year=" + sDT.Year + " and a.Month >=" + sDT.Month + " and a.Month <=" + eDT.Month;
     //       }
     //       else
     //       {
     //           SQL += " where (a.Year=" + sDT.Year + " and a.Month >=" + sDT.Month + ") or (a.Year=" + eDT.Year + " and a.Month <=" + eDT.Month + ")";
     //       }

     //       SQL += " order by a.Year, a.Month";

     //       return DBAccess.GetDataSetFromExcuteCommand(SQL).Tables[0];

     //   }

     //   //todo:SQL语句不能跨SQL Server平台
     //   protected List<string> SummarizeStat(string Id, int len = 73)
     //   {

     //       //string Id = "";
     //       //if (type == 1)
     //       //{
     //       //    Id = "702fd19f-4dc2-4490-a63b-bcd15593d4c7";
     //       //}
     //       //else
     //       //{
     //       //    Id = "8ba5c7e9-6196-4bbd-a06b-39a0239c636e";
     //       //}

     //       string SQL = "select t1.StatSum, t1.LayerType, t2.\"ClassName\" from ";
     //       SQL += " ( select count(a.Id) StatSum, substr(b.\"Paths\", 0," + len + ") LayerType  from ";
     //       SQL += " ( select Max(\"Id\") Id, \"MappingTypeID\"  from tbl_datamain where  \"ReleaseTime\" is not null  and \"Version\"={0}   group by \"MappingTypeID\",  \"ZipFileName\", \"Scale\" ) a left join TBL_GEOLOGYMAPPINGTYPE b on a.\"MappingTypeID\" = b.\"Id\"  where ";
     //       SQL += " substr(b.\"Paths\", 0," + len + ") in ( ";
     //       SQL += " select \"Paths\" from TBL_GEOLOGYMAPPINGTYPE  where \"ParentID\"='" + Id + "')";
     //       SQL += " group by substr(b.\"Paths\", 0," + len + ")  ) t1 left join TBL_GEOLOGYMAPPINGTYPE t2 on t1.LayerType = t2.\"Paths\"";

     //       DataTable Version1DT = DBAccess.GetDataSetFromExcuteCommand(string.Format(SQL, "1")).Tables[0];
     //       DataTable Version2DT = DBAccess.GetDataSetFromExcuteCommand(string.Format(SQL, "2")).Tables[0];
     //       DataTable Result1DT = DBAccess.GetDataSetFromExcuteCommand(" select * from TBL_GEOLOGYMAPPINGTYPE  where \"ParentID\"='" + Id + "'").Tables[0];
     //       DataTable Result2DT = Result1DT.Copy();

     //       for (int i = 0; i < Result1DT.Rows.Count; i++)
     //       {
     //           Result1DT.Rows[i]["Sn"] = 0;
     //           Result2DT.Rows[i]["Sn"] = 0;
     //       }

     //       for (int i = 0; i < Result1DT.Rows.Count; i++)
     //       {
     //           for (int j = 0; j < Version1DT.Rows.Count; j++)
     //           {
     //               if (Version1DT.Rows[j]["LayerType"].ToString() == Result1DT.Rows[i]["Paths"].ToString())
     //               {
     //                   Result1DT.Rows[i]["Sn"] = Version1DT.Rows[j]["StatSum"];
     //                   break;
     //               }
     //           }

     //           for (int j = 0; j < Version2DT.Rows.Count; j++)
     //           {
     //               if (Version2DT.Rows[j]["LayerType"].ToString() == Result2DT.Rows[i]["Paths"].ToString())
     //               {
     //                   Result2DT.Rows[i]["Sn"] = Version2DT.Rows[j]["StatSum"];
     //                   break;
     //               }
     //           }
     //       }

     //       string Version1 = "";
     //       string Version2 = "";
     //       for (int i = 0; i < Result1DT.Rows.Count; i++)
     //       {
     //           Version1 += Result1DT.Rows[i]["ClassName"].ToString() + ":" + Result1DT.Rows[i]["Sn"].ToString() + ",";
     //       }
     //       for (int i = 0; i < Result2DT.Rows.Count; i++)
     //       {
     //           Version2 += Result2DT.Rows[i]["ClassName"].ToString() + ":" + Result2DT.Rows[i]["Sn"].ToString() + ",";
     //       }

     //       if (Version1.Length > 1)
     //       {
     //           Version1 = Version1.Remove(Version1.Length - 1);
     //       }
     //       if (Version2.Length > 1)
     //       {
     //           Version2 = Version2.Remove(Version2.Length - 1);
     //       }

     //       return new List<string> { Version1, Version2 };
     //   }

     //   //todo:SQL语句不能跨SQL Server平台
     //   protected List<string> AreaTypeStat()
     //   {
     //       DataTable AreaDT = DBAccess.GetDataSetFromExcuteCommand("select * from TBL_GEOLOGYMAPPINGTYPE t where \"ParentID\"='057bc40b-92f3-405d-b699-b02c24dc8b69'").Tables[0];


     //       string SQL = "select ClassName, count(NoId) StatSum from ";
     //       SQL += " (";
     //       SQL += " select Max(\"ClassName\")  ClassName, Max(\"Id\") NoId from";
     //       SQL += " (";
     //       SQL += " select t2.\"ClassName\", t1.* from (select a.*, b.\"ParentID\", b.\"Paths\" from tbl_datamain a left join tbl_geologymappingtype b on a.\"MappingTypeID\"=b.\"Id\" where a.\"ReleaseTime\" is not null and a.\"Version\"={0} ) t1 ";
     //       SQL += " left join tbl_geologymappingtype t2  on t1.\"ParentID\"=t2.\"Id\"";
     //       SQL += " where  t1.\"Paths\" like '057bc40b-92f3-405d-b699-b02c24dc8b69%'";
     //       SQL += " ) group by \"MappingTypeID\", \"Scale\", \"ZipFileName\"";
     //       SQL += " ) group by ClassName";

     //       DataTable Version1DT = DBAccess.GetDataSetFromExcuteCommand(string.Format(SQL, "1")).Tables[0];
     //       DataTable Version2DT = DBAccess.GetDataSetFromExcuteCommand(string.Format(SQL, "2")).Tables[0];

     //       DataTable Result1DT = DBAccess.GetDataSetFromExcuteCommand(" select \"ClassName\", max(\"Sn\") Sn  from TBL_GEOLOGYMAPPINGTYPE where \"Paths\" like '057bc40b-92f3-405d-b699-b02c24dc8b69%' AND length(\"Paths\") = 110 group by \"ClassName\"").Tables[0];
     //       DataTable Result2DT = Result1DT.Copy();

     //       for (int i = 0; i < Result1DT.Rows.Count; i++)
     //       {
     //           Result1DT.Rows[i]["Sn"] = 0;
     //           Result2DT.Rows[i]["Sn"] = 0;
     //       }

     //       for (int i = 0; i < Result1DT.Rows.Count; i++)
     //       {
     //           for (int j = 0; j < Version1DT.Rows.Count; j++)
     //           {
     //               if (Version1DT.Rows[j]["ClassName"].ToString() == Result1DT.Rows[i]["ClassName"].ToString())
     //               {
     //                   Result1DT.Rows[i]["Sn"] = Version1DT.Rows[j]["StatSum"];
     //                   break;
     //               }
     //           }

     //           for (int j = 0; j < Version2DT.Rows.Count; j++)
     //           {
     //               if (Version2DT.Rows[j]["ClassName"].ToString() == Result2DT.Rows[i]["ClassName"].ToString())
     //               {
     //                   Result2DT.Rows[i]["Sn"] = Version2DT.Rows[j]["StatSum"];
     //                   break;
     //               }
     //           }
     //       }

     //       string Version1 = "";
     //       string Version2 = "";
     //       for (int i = 0; i < Result1DT.Rows.Count; i++)
     //       {
     //           Version1 += Result1DT.Rows[i]["ClassName"].ToString() + ":" + Result1DT.Rows[i]["Sn"].ToString() + ",";
     //       }
     //       for (int i = 0; i < Result2DT.Rows.Count; i++)
     //       {
     //           Version2 += Result2DT.Rows[i]["ClassName"].ToString() + ":" + Result2DT.Rows[i]["Sn"].ToString() + ",";
     //       }

     //       if (Version1.Length > 1)
     //       {
     //           Version1 = Version1.Remove(Version1.Length - 1);
     //       }
     //       if (Version2.Length > 1)
     //       {
     //           Version2 = Version2.Remove(Version2.Length - 1);
     //       }

     //       return new List<string> { Version1, Version2 };

     //       #region
     //       //DataTable Result1DT = DBAccess.GetDataSetFromExcuteCommand(" select * from TBL_GEOLOGYMAPPINGTYPE  where \"ParentID\"='1a9034a9-31cc-460a-86a7-f4dd06141d8f'").Tables[0];
     //       //DataTable Result2DT = Result1DT.Copy();
     //       //for (int i = 0; i < Result1DT.Rows.Count; i++)
     //       //{
     //       //    Result1DT.Rows[i]["Sn"] = 0;
     //       //    Result2DT.Rows[i]["Sn"] = 0;
     //       //}

     //       //for(int i=0; i<AreaDT.Rows.Count; i++)
     //       //{
     //       //    DataTable Tmp1 = new DataTable();
     //       //    DataTable Tmp2 = new DataTable();
     //       //    SummarizeStat(AreaDT.Rows[i]["Id"].ToString(), out Tmp1, out Tmp2, 110);

     //       //    for(int j=0; j<Result1DT.Rows.Count; j++)
     //       //    {
     //       //        for(int k=0; k<Tmp1.Rows.Count; k++)
     //       //        {
     //       //            if (Result1DT.Rows[j]["ClassName"].ToString() == Tmp1.Rows[j]["ClassName"].ToString())
     //       //            {
     //       //                Result1DT.Rows[j]["Sn"] = Convert.ToInt32(Result1DT.Rows[j]["Sn"]) + Convert.ToInt32(Tmp1.Rows[j]["Sn"]);
     //       //                break;
     //       //            }
     //       //        }
     //       //    }

     //       //    for (int j = 0; j < Result2DT.Rows.Count; j++)
     //       //    {
     //       //        for (int k = 0; k < Tmp2.Rows.Count; k++)
     //       //        {
     //       //            if (Result2DT.Rows[j]["ClassName"].ToString() == Tmp2.Rows[j]["ClassName"].ToString())
     //       //            {
     //       //                Result2DT.Rows[j]["Sn"] = Convert.ToInt32(Result2DT.Rows[j]["Sn"]) + Convert.ToInt32(Tmp2.Rows[j]["Sn"]);
     //       //                break;
     //       //            }
     //       //        }
     //       //    }
     //       //}
     //       #endregion
     //   }

     //   //todo:SQL语句不能跨SQL Server平台
     //   protected DataTable AreaStat(string ParentID = "057bc40b-92f3-405d-b699-b02c24dc8b69", int len = 73)
     //   {

     //       //string SQL = "select STATNUM, \"ClassName\", Paths from";
     //       //SQL += " (";
     //       //SQL += " select Paths, count(Id) STATNUM from";
     //       //SQL += " (";
     //       //SQL += " select Max(a.\"Id\") as Id , a.\"ZipFileName\", a.\"Version\", a.\"Scale\", Max(substr(b.\"Paths\",0, " + len + ")) Paths from  tbl_datamain a left join tbl_geologymappingtype b on a.\"MappingTypeID\"=b.\"Id\" where a.\"ReleaseTime\" is not null and substr(b.\"Paths\",0, 36) = '057bc40b-92f3-405d-b699-b02c24dc8b69'";
     //       //SQL += " group by a.\"ZipFileName\", a.\"Version\", a.\"Scale\"";
     //       //SQL += " ) group by Paths";
     //       //SQL += " ) t1  right join (select * from TBL_GEOLOGYMAPPINGTYPE where \"ParentID\"='" + ParentID + "') t2 on t1.Paths = t2.\"Paths\"";

     //       //return DBAccess.GetDataSetFromExcuteCommand(SQL).Tables[0];
     //       return null;

     //   }

      //  #endregion


        public DataMain GetDataMain(string id)
        {
            return _DataMainRepository.FirstOrDefault(p => p.Id == id);
        }


        public bool UploadLayer(UploadLayerContext context)
        {
            context.Upload();

            //TODO:引入了Geoserver，出现了好多BUG,后面着重处理下。
            if (context.ErrorInfo.Count == 0||context.IsGeoServerError)
            {
                if (context.Files != null && context.Files.Any())
                {
                    var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();

        
                    var dic = new Dictionary<string, string>();
                    foreach (var item in context.Layers)
                    {
                        //var layer = AutoMapper.Mapper.Map<LayerContentEntity>(item);
                        //layer.Id = Guid.NewGuid().ToString();
                        //layer.CreateDT = DateTime.Now;
                        //db.LayerContent.Add(layer);

                        //TODO:layercontent不用插入,由空间数据管理插入
                        if (!dic.ContainsKey(item.LayerName))
                        {
                            dic.Add(item.LayerName, item.LayerContentId);
                        }
                    }

                     var layerId = "";
                     var layers = new List<string>();
                    foreach (var file in context.Files)
                    {
                        var dataFile = AutoMapper.Mapper.Map<DataManageFile>(file);
                        dataFile.Id=Guid.NewGuid().ToString();
                        dataFile.StorageTime = DateTime.Now;
                        dataFile.CreaterID = context.UploadUserId;
                         layerId = dic[file.FileName.Substring(0, file.FileName.LastIndexOf("."))];
                        if (file.Ext.ToLower() == ".shp")
                        {
                            dataFile.Id = layerId;
                            dataFile.ParentId = "";

                            var layer = db.LayerContent.FirstOrDefault(p => p.Id == layerId);
                            if (layer != null)
                            {
                                layers.Add(layer.Id);
                                layer.MainId = context.MainId;
                                db.Entry(layer).State = EntityState.Modified;

                                MapReleationEntity entity = new MapReleationEntity
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    MapID =context.MainId,
                                    DataConfigID = layer.Id,
                                    DataStyleID = "",
                                    ConfigDT = DateTime.Now
                                };

                                db.MapReleation.Add(entity);
                            }
                        }
                        else
                        {
                            dataFile.ParentId = layerId;
                        }
                        db.DataManageFile.Add(dataFile);
                    }

                    if (layers != null && layers.Count > 0)
                    {
                           var map =db.Map.FirstOrDefault(p=>p.Id==context.MainId);
                           if (map != null)
                           {
                               var list = db.LayerContent.Where(t => layers.Contains(t.Id)).ToList();
                               if (list != null)
                               {
                                   Dictionary<string, decimal?> bbox = new Dictionary<string, decimal?>();
                                   for (int i = 0; i < list.Count; i++)
                                   {
                                       if (i == 0)
                                       {
                                           bbox.Add("MaxX", list[i].MaxX);
                                           bbox.Add("MinX", list[i].MinX);
                                           bbox.Add("MaxY", list[i].MaxY);
                                           bbox.Add("MinY", list[i].MinY);
                                       }
                                       else
                                       {
                                           if (list[i].MaxX > bbox["MaxX"])
                                           {
                                               bbox["MaxX"] = list[i].MaxX;
                                           }
                                           if (list[i].MaxY > bbox["MaxY"])
                                           {
                                               bbox["MaxY"] = list[i].MaxY;
                                           }
                                           if (list[i].MinX < bbox["MinX"])
                                           {
                                               bbox["MinX"] = list[i].MinX;
                                           }
                                           if (list[i].MinY < bbox["MinY"])
                                           {
                                               bbox["MinY"] = list[i].MinY;
                                           }
                                       }
                                   }

                                   map.MaxX = bbox["MaxX"];
                                   map.MinX = bbox["MinX"];
                                   map.MaxY = bbox["MaxY"];
                                   map.MinY = bbox["MinY"];

                                   db.Entry(map).State = EntityState.Modified;
                               }
                           }
                    }

                    var flag= db.SaveChanges() > 0;
                    return flag;
                }
            }
            return context.ErrorCode == 0;
        }

        public async Task<bool> Delete(IEnumerable<string> ids)
        {
            var sql = "delete from \"TBL_DATAMANAGEFILE\" where \"Id\" in (" + string.Join(",", ids.Select(p => "'" + p + "'")) + ")";
            var db = _DataManageFileRepository.GetDbContext();
            var flag = false;
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    flag = await db.Database.ExecuteSqlCommandAsync(TransactionalBehavior.DoNotEnsureTransaction, sql) > 0;

                    sql = "delete from sdms_layer where id in (" + string.Join(",", ids.Select(p => "'" + p + "'")) + ")";
                    flag = await db.Database.ExecuteSqlCommandAsync(TransactionalBehavior.DoNotEnsureTransaction, sql) > 0;

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Commit();
                    throw;
                }
            }
          
            return flag;
        }


        public bool UploadFile(UploadFileContext context)
        {
            context.Upload();
            if (context.ErrorInfo.Count == 0)
            {
                if (context.Files != null && context.Files.Any())
                {
                    var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();
                    if (context.FolderName.Contains("栅格图") || context.FolderName.Contains("缩略图"))
                    {
                        var images = db.DataManageFile.Where(p => p.MainID == context.MainId && p.FolderName == context.FolderName).ToList();
                        images.ForEach(p =>
                        {
                            db.DataManageFile.Remove(p);
                        });
                    }




                    var thumbPath = "";
                    foreach (var file in context.Files)
                    {
                        var dataFile = AutoMapper.Mapper.Map<DataManageFile>(file);
                        dataFile.Id = Guid.NewGuid().ToString();
                        dataFile.StorageTime = DateTime.Now;
                        dataFile.CreaterID = context.UploadUserId;
                        db.DataManageFile.Add(dataFile);

                         thumbPath = file.FileData;
                    }
                    context.Tag = ConfigContext.Current.FtpConfig["Package"].Site + "/" + thumbPath.Replace("\\", "/");

                    if (context.FolderName.Contains("缩略图"))
                    {
                        
                        var main = db.DataMain.FirstOrDefault(p => p.Id == context.MainId);
                        if (main != null)
                        {
                            main.ImagePath = thumbPath;
                            db.Entry(main).State = EntityState.Modified;
                        }
                    }

                    //删除上传的文件
                    context.RemovePostedFiles();

                
                    var flag= db.SaveChanges() > 0;
                    return flag;
                }
            }
            return context.ErrorCode == 0;
        }


        public async Task<bool> Delete(PackageCategoryDto dto)
        {
            var sql = "delete from \"TBL_DATAMANAGEFILE\" where \"MainID\"='"+dto.MainId+"' AND \"FolderName\"='"+dto.FolderName+"'";
            var db = _DataManageFileRepository.GetDbContext();
            var flag = await db.Database.ExecuteSqlCommandAsync(sql) > 0;
            return flag;
        }


        public InfoEarthFrame.Common.Metadata GetMetaData(string mainId, string folderName)
        {
              var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();
               var data = new Metadata
                  {
                      MainId = mainId,
                      FolderName = folderName
                  };
              var file = db.DataManageFile.FirstOrDefault(p => p.MainID == mainId && p.FolderName == folderName);
              if (file == null)
              {
                  return data;
              }

              data = Metadata.GetInstance(mainId, folderName, file.FileData);
              if (file == null)
              {
                  data = new Metadata
                 {
                     MainId = mainId,
                     FolderName = folderName
                 };
                  return data;
              }

              var main = db.DataMain.FirstOrDefault(p => p.Id == mainId);
              if (main != null)
              {
               data.ThumbFile = ConfigContext.Current.FtpConfig["Package"].Site + "/" + (main.ImagePath??"").Replace("\\", "/");
              }

              data.MainId = mainId;
              data.FolderName = folderName;
              data.Keyword = string.Join("|", data.dataIdInfo.KeyWordsList.Select(p => p.keyword));
              data.CollectTime = data.dataIdInfo.TempExtent.begin + "-" + data.dataIdInfo.TempExtent.end;

              return data;
        }


        public bool SaveMetaData(Metadata context)
        {
            //保存文件到本地
            context.Save();

            //先将数据插入到数据库中
            var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();
            var xmls = db.DataManageFile.Where(p => p.MainID == context.MainId && p.FolderName == context.FolderName).ToList();
            xmls.ForEach(p =>
            {
                db.DataManageFile.Remove(p);
            });

            var entity = new DataManageFile
            {
                StorageTime = DateTime.Now,
                Ext = context.XmlFile.Extension,
                CreaterID = context.CreateUserId,
                FileData = context.XmlFtpPath,
                FileName = context.XmlFile.Name,
                MainID = context.MainId,
                FileSize = (int)context.XmlFile.Length,
                FolderName = context.FolderName,
                Id = Guid.NewGuid().ToString()
            };
            db.DataManageFile.Add(entity);

            var flag = db.SaveChanges() > 0;
            return flag;
        }

  
  
        public bool UploadPackage(UploadPackageContext context)
        {
            var currentCate = "";
            var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();
            var model = db.DataMain.FirstOrDefault(p => p.Id == context.MainId);
            try
            {
                context.UnzipFile();
                foreach (var cate in context.PackageCategory)
                {
                    currentCate = cate.Key;
                    var currentDir = new DirectoryInfo(Path.Combine(context.SaveDirectory,context.Name, cate.Value));
                    var files = currentDir.GetFiles();
                    if (cate.Value.Contains("地理图层")||cate.Value.Contains("专业图层"))
                    {
                    
                            var layerContext = new UploadLayerContext(_dataConvertAppService)
                            {
                                FolderName = cate.Value,
                                UploadUserId = context.UploadUserId,
                                MainId = context.MainId,
                                PostedFile = null,
                                IsFullPackageUpload = true,
                                SaveFilePath =currentDir.FullName
                            };

                            try
                            {
                                UploadLayer(layerContext);
                                if (layerContext.IsGeoServerError)
                                {
                                    throw new Exception();
                                }
                            }
                            catch (Exception ex)
                            {
                               // layerContext.ErrorInfo.Insert(0, ex.Message);
                                context.UploadFileResults.Add(new UploadFileResult
                                {
                                    Category = cate.Key,
                                    ErrorInfo = layerContext.ErrorInfo,
                                    ErrorCode =500
                                });

                                model.Status=3;
                            }
                    }
                    else
                    {
                        if (files != null && files.Any())
                        {
                            foreach (var file in files)
                            {
                                if (cate.Value.Contains("文档") || cate.Value.Contains("系统库") || cate.Value.Contains("栅格图")
                                    || cate.Value.Contains("说明书") || cate.Value.Contains("制图文件"))
                                {
                                    var fileContext = new UploadFileContext
                                    {
                                        FolderName = cate.Value,
                                        UploadUserId = context.UploadUserId,
                                        MainId = context.MainId,
                                        PostedFile = null,
                                        IsFullPackageUpload = true,
                                        SaveFilePath = file.FullName
                                    };
                                    try
                                    {
                                        UploadFile(fileContext);

                                
                                    }
                                    catch (Exception ex)
                                    {
                                      //  fileContext.ErrorInfo.Insert(0, ex.Message);
                                        context.UploadFileResults.Add(new UploadFileResult
                                        {
                                            Category = cate.Key,
                                            ErrorInfo = fileContext.ErrorInfo,
                                            ErrorCode = 500
                                        });

                                        model.Status = 3;
                                    }
                                }
                                else if (cate.Value.Contains("元数据"))
                                {
                                    var metaDataContext = new Metadata
                                    {
                                        CreateUserId = context.UploadUserId,
                                        FolderName = cate.Value,
                                        MainId = context.MainId,
                                        XmlFilePath = file.FullName
                                    };
                                    try
                                    {
                                        SaveMetaData(metaDataContext);
                                    }
                                    catch (Exception ex)
                                    {
                                        context.UploadFileResults.Add(new UploadFileResult
                                        {
                                            Category = cate.Key,
                                            ErrorInfo = new List<string> { ex.Message },
                                            ErrorCode = 500
                                        });

                                        model.Status = 3;
                                    }
                                }

                            }
                        }
                    }
                }

                model.Status = context.UploadFileResults.Any(p => p.ErrorCode < 0 || p.ErrorCode == 500) ? 3 : 2;
                model.Message = JsonConvert.SerializeObject(context.UploadFileResults);
                db.Entry(model).State = EntityState.Modified;
                var flag= db.SaveChanges() > 0;

                //TODO:服务发布
                //if (flag)
                //{
                //    currentCate = "发布地图服务";
                //    return _MapReleationAppService.PublicMap(context.MainId);
                //}

                return flag;
            }
            catch (Exception ex)
            {
                context.UploadFileResults.Add(new UploadFileResult
                {
                    Category = currentCate,
                    ErrorInfo = new List<string>{ex.Message},
                    ErrorCode =500
                });

                model.Status = 3;
                model.Message = JsonConvert.SerializeObject(context.UploadFileResults);
                db.Entry(model).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
        }


        public List<UploadFileResult> GetErrorMsg(string mainId)
        {
            var model = GetDataMain(mainId);
            if(model==null)
            {
              return new List<UploadFileResult>();
            }

            return JsonConvert.DeserializeObject<List<UploadFileResult>>(model.Message);
        }


        public bool ChangePackageStatus(string mainId)
        {
            var model = GetDataMain(mainId);
            if (model == null)
            {
                return false;
            }

            model.Status = null;
            var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();
            db.Entry(model).State = EntityState.Modified;
            return db.SaveChanges() > 0;
        }


        public bool RemovePackage(string mainId)
        {
            //TODO:删除可能要优化
            var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();

            var layerContents = db.LayerContent.Where(p => p.MainId == mainId).ToList();
            foreach (var item in layerContents)
            {
                var layerFields = db.LayerField.Where(p => p.LayerID == item.Id).ToList();
                foreach (var field in layerFields)
                {
                    db.LayerField.Remove(field);
                }
                db.LayerContent.Remove(item);
            }

            var layerFiles = db.DataManageFile.Where(p => p.MainID == mainId).ToList();
            foreach (var item in layerFiles)
            {
                db.DataManageFile.Remove(item);
            }


            //TBL_DATAMAIN
            var mainData = db.DataMain.Where(p => p.Id == mainId).ToList();
            foreach (var item in mainData)
            {
                db.DataMain.Remove(item);
            }


            //TBL_LAYERMANAGER
            var layerMaps = db.LayerManagers.Where(p => p.DataMainID == mainId).ToList();
            foreach (var item in layerMaps)
            {
                db.LayerManagers.Remove(item);
            }

            //sdms_map_releation
            var mapRefs = db.MapReleation.Where(p => p.MapID == mainId).ToList();
            foreach (var item in mapRefs)
            {
                db.MapReleation.Remove(item);
            }

            //sdms_map
            var maps = db.Map.Where(p => p.Id == mainId).ToList();
            foreach (var item in maps)
            {
                db.Map.Remove(item);
            }

            return db.SaveChanges() > 0;
        }

        public IList<DataMain> GetAllList(string mappingTypeId,string userId)
        {
            var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();
            var sql = "SELECT" +
"	*" +
" FROM" +
"	\"TBL_DATAMAIN\" M" +
" JOIN (" +
"	  SELECT" +
"		\"Id\"" +
" FROM" +
"	\"TBL_GEOLOGYMAPPINGTYPE\" T" +
" WHERE" +
"	EXISTS (" +
"		SELECT" +
"			\"LayerId\"" +
"		FROM" +
"			\"TBL_GROUP_RIGHT\" r" +
"		WHERE" +
"			r.\"GroupId\" IN (" +
"				SELECT" +
"					u.\"GroupId\"" +
"				FROM" +
"					\"TBL_GROUP_USER\" u" +
"				WHERE" +
"					u.\"UserId\" = \'" + userId + "\'" +
"				AND r.\"IsBrowse\" = 1 group BY u.\"GroupId\"" +
"			)" +
"		AND T .\"Id\" = r.\"LayerId\"" +
"	)" +
" and 	" +
"		\"Paths\" LIKE \'%" + mappingTypeId + "%\'" +
" ) T ON T .\"Id\" = M .\"MappingTypeID\"";
            if (userId.ToLower() == "admin")
            {
                sql = "SELECT" +
 "	*" +
 " FROM" +
 "	\"TBL_DATAMAIN\" M" +
 " JOIN (" +
 "	  SELECT" +
 "		\"Id\"" +
 " FROM" +
 "	\"TBL_GEOLOGYMAPPINGTYPE\" T" +
 " WHERE" +
 "		\"Paths\" LIKE \'%" + mappingTypeId + "%\'" +
 ") T ON T .\"Id\" = M .\"MappingTypeID\"";

            }
          
            var query = db.Database.SqlQuery<DataMain>(sql);

            return query.ToList();
        }

        public IList<DataMain> GetAllListByName(string name)
        {
            var db = (InfoEarthFrameDbContext)_DataMainRepository.GetDbContext();

            var query = db.DataMain.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = db.DataMain.Where(p => p.Name != null && p.Name.ToLower().Contains( name.ToLower()));
            }

            return query.ToList();
        }
    }
}
