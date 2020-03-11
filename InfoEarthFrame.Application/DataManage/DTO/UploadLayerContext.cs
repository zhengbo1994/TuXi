using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.DataManage.DTO
{
    public class UploadZipDto
    {
        /// <summary>
        /// 1：mapgis转shp文件；2：arcgis文件入库；3：mapgis文件入库
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 1：压缩文件；2：其他文件
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 本地文件路径，多个文件;间隔
        /// </summary>
        public string FilePath { get; set; }
    }


    public class UploadLayerContext : UploadFileContext
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(UploadLayerContext));
        private  Dictionary<string, IList<string>> _fileGroups;
        private readonly IDataConvertAppService _dataConvertAppService;
        public UploadLayerContext(IDataConvertAppService dataConvertAppService)
        {
            this._dataConvertAppService = dataConvertAppService;
        }
        /// <summary>
        /// 转换后的结果
        /// </summary>
        private ConvertResult ConvertedResult
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是GeoServer错误，如果是请忽略掉。这个错误太几把操蛋了。去死吧，操！
        /// </summary>
        public bool IsGeoServerError { get; set; }

        private List<FileInfo> _convertedFiles;
        protected override List<FileInfo> ConvertedFiles
        {
            get
            {
                if (_convertedFiles == null)
                {
                    if (ConvertedResult != null
                        && ConvertedResult.fileList != null && ConvertedResult.fileList.Any())
                    {
                        var dir = Path.GetDirectoryName(ConvertedResult.fileList[0].PhysicsFilePath);
                        _convertedFiles = GetShpFiles(dir);
                    }
                    else
                    {
                      _convertedFiles= GetShpFiles(SaveDirectory);
                    }
                }

                return _convertedFiles;
            }
        }

        private List<FileInfo> GetShpFiles(string directory)
        {
            var files = new DirectoryInfo(directory).GetFiles();
            var list = new List<FileInfo>();
            _fileGroups = new Dictionary<string, IList<string>>();
            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var ext = file.Extension.ToLower();
                    if (ext == ".shp" || ext == ".shx" || ext == ".dbf" || ext == ".prj")
                    {
                        list.Add(file);

                        var key=file.Name.ToLower().Replace(ext,"");
                        if (!_fileGroups.ContainsKey(key))
                        {
                            _fileGroups.Add(key, new List<string>{
                                file.FullName
                            });
                        }
                        else
                        {
                            _fileGroups[key].Add(file.FullName);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        ///解压zip或rar文件
        /// </summary>
        private void UnzipFile()
        {
            var ext = Path.GetExtension(SaveFilePath);
            if (ext.ToLower() == ".zip" || ext.ToLower() == ".rar")
            {
                RarOrZipUtil.DeCompress(SaveFilePath, SaveDirectory);
            }
        }


        private List<UploadLayerDto> _layers;
        public List<UploadLayerDto> Layers
        {
            get
            {
                if (_layers == null)
                {
                   _layers= new List<UploadLayerDto>();
                    var prjFiles = Files.Where(p => p.FileName.ToLower().EndsWith(".shp"));
                    if (prjFiles != null && prjFiles.Any())
                    {
                        foreach (var item in prjFiles)
                        {
                            _layers.Add(new UploadLayerDto
                            {
                                Id = Guid.NewGuid().ToString(),
                                LayerName = item.FileName.Substring(0, item.FileName.LastIndexOf(".")),
                                CreateBy = UploadUserId,
                                UploadFileType = 1,
                                MainID = MainId
                            });
                        }
                    }
                }
                return _layers;
            }
        }

  
        /// <summary>
        /// 文件转换和检查
        /// </summary>
        /// <param name="FolderPath">要素文件夹中路径</param>
        /// <returns>错误信息</returns>
        private void ConvertAndCheckFile(string FolderPath)
        {

            string[] files = Directory.GetFiles(FolderPath, "*.*", SearchOption.AllDirectories);

            // 格式转换
            try
            {
                ConvertedResult = DataConvertAppService.FormatConvert(files.Select(p => new ConvertFileList
                {
                    PhysicsFilePath = p
                }), ErrorInfo, _dataConvertAppService);
            }
            catch (Exception ex)
            {
                ErrorInfo.Add("格式转换失败： " + ex.Message);
                return;
            }
       
            // 坐标转换
            try
            {

                ConvertedResult = DataConvertAppService.CoordinateConvert(ConvertedResult.fileList, ErrorInfo, _dataConvertAppService);
            }
            catch (Exception ex)
            {
                ErrorInfo.Add("坐标转换失败： " + ex.Message);
                return;
            }
        
            #region 投影转换
            // 投影转换
            try
            {
                ConvertedResult = DataConvertAppService.ProjectionConvert(ConvertedResult.fileList, ErrorInfo, _dataConvertAppService);
            }
            catch (Exception ex)
            {
                ErrorInfo.Add("投影转换失败： " + ex.Message);
                return;
            }
           
            #endregion

            //// 属性检查
            //List<string> CheckFile = new List<string>();
            //foreach (var f in result.fileList)
            //{
            //    if (f.ConvertResult == 0)
            //    {
            //        ErrorInfo.Add("坐标转换失败： " + f.ConvertMsg + "  文件：" + f.PhysicsFilePath);
            //        return ErrorInfo;
            //    }

            //    CheckFile.Add(f.ConvertFilePath);
            //}
            //DataCheckResult dcResult = DataConvert.DataCheck(CheckFile, false);

            //foreach (var cl in dcResult.CheckInfoList)
            //{
            //    if (cl.Log.Count > 0)
            //    {
            //        FileInfo ff = new FileInfo(cl.FileName);
            //        ErrorInfo.Add(ff.Name + " " + cl.Log[0]);
            //    }
            //}

        }

        protected override void BeforeUploadToFtp()
        {
            UnzipFile();
            this.ConvertAndCheckFile(this.SaveDirectory);

            try
            {
                
                //删除上传文件
                if (!base.IsFullPackageUpload)
                {
                    File.Delete(SaveFilePath);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void AfterUploadToFtp()
        {
            if (_fileGroups != null && _fileGroups.Any())
            {
                var fileType = "2";
                var type = "2";
                foreach (var key in _fileGroups.Keys)
                {
                    var filePath = string.Join(";", _fileGroups[key]);
                    var dto = new UploadZipDto
                    {
                        FilePath = filePath,
                        FileType = fileType,
                        Type = type
                    };

                    try
                    {
                        var resp = HttpClinetHelper.PostResponseJson(ConfigContext.Current.DefaultConfig["Upload:SLWJRK"], JsonConvert.SerializeObject(dto));
                        _logger.Debug(resp);

                        //TODO:给laycontentId赋值
                        if (!resp.Contains("error"))
                        {
                            var model = JsonConvert.DeserializeObject<InfoEarthFrame.ServerInterfaceApp.ServerInterfaceAppService.UploadResult>(resp);
                            if (model != null)
                            {
                                var layer = Layers.FirstOrDefault(p => p.LayerName.ToLower() == key.ToLower());
                                if (layer != null && model.LayerContentEntity != null)
                                {
                                    layer.LayerContentId = model.LayerContentEntity.Id;
                                }
                                else
                                {
                                    layer.LayerContentId = Guid.NewGuid().ToString() + "_geo-200";

                                    this.ErrorInfo.Add("图层文件入库失败：" + model.Message + "，相关文件:" + string.Join("、", dto.FilePath.Split(';').Select(p => Path.GetFileName(p))) + "");
                                    this.IsGeoServerError = true;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(resp);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);

                        var layer = Layers.FirstOrDefault(p => p.LayerName.ToLower() == key.ToLower());
                        if (layer != null)
                        {
                            layer.LayerContentId = Guid.NewGuid().ToString() + "_geo500";
                        }
                        this.ErrorInfo.Add("图层文件入库出错，相关文件:" + string.Join("、", dto.FilePath.Split(';').Select(p => Path.GetFileName(p))) + ",错误信息:" + ex.Message);
                        this.IsGeoServerError = true;
                    }
                }
            }
        }

        protected override bool IsRenamePostedFile
        {
            get
            {
                return true;
            }
        }
    }
}
