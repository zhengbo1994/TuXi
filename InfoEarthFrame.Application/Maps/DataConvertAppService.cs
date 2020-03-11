using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Application.Services;
using iTelluro.GeologicMap.TopologyCheck;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;
using InfoEarthFrame.Core;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using iTelluro.DataTools.Utility.GIS;
using iTelluro.ZoomifyTile;
using OSGeo.OGR;
using System.Data.OleDb;
using InfoEarthFrame.Common.ShpUtility;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Common;
using log4net;
using Newtonsoft.Json;

namespace InfoEarthFrame.Application
{
    public class DataConvertAppService : ApplicationService, IDataConvertAppService
    {
        private string ErrorMsg = "";
        private string InfoMsg = "";

     //   public readonly InfoEarthFrame.Core.IConvertFileRepository _ConvertFileRepository = null;
       // private readonly IDicDataCodeRepository _IDicDataCodeRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DataConvertAppService));
        public static readonly IList<DicDataCodeEntity> OutputFormats = null;

        static DataConvertAppService()
        {
            OutputFormats = new List<DicDataCodeEntity>()
              {
               new DicDataCodeEntity{
                CodeName="GML",
                 CodeValue=".gml"
               },
                new DicDataCodeEntity{
                CodeName="GeoJSON",
                 CodeValue=".json"
               },
                 new DicDataCodeEntity{
                CodeName="ESRI Shapefile",
                 CodeValue=".shp"
               },
                 new DicDataCodeEntity{
                CodeName="CSV",
                 CodeValue=".csv"
               },
                      new DicDataCodeEntity{
                CodeName="KML",
                 CodeValue=".kml"
               },
                      new DicDataCodeEntity{
                CodeName="MapInfo File",
                 CodeValue=".tab"
               },
                       new DicDataCodeEntity{
                CodeName="DXF",
                 CodeValue=".dxf"
               }
              };
        }
        public DataConvertAppService(
           // InfoEarthFrame.Core.IConvertFileRepository convertFileRepository, IDicDataCodeRepository iDicDataCodeRepository
            )
        {
           // _ConvertFileRepository = convertFileRepository;
            //_IDicDataCodeRepository = iDicDataCodeRepository;
            //var convertExprotPath = ConfigurationManager.AppSettings["ConvertExprotPath"];
            //if (!Directory.Exists(convertExprotPath))
            //{
            //    Directory.CreateDirectory(convertExprotPath);
            //}
        }



        /// <summary>
        /// 获得文件列表
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<InfoEarthFrame.Core.ConvertFile> GetConvertFilesList(string UserID)
        {
            //try
            //{
            //    var list = _ConvertFileRepository.GetAllList().Where(s => ((s.UserID == UserID))).OrderByDescending(t => t.CreateTime);
            //    return list.ToList();
            //}
            //catch (Exception e)
            //{

            //    throw e;
            //}
            throw new NotImplementedException();
        }

        public InfoEarthFrame.Core.ConvertFile GetConvertFiles(string Id)
        {
            //try
            //{
            //    return _ConvertFileRepository.Get(Id);
            //}
            //catch (Exception e)
            //{

            //    throw e;
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得文件数
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int GetConvertFilesNum(string UserID)
        {
            //try
            //{
            //    var list = _ConvertFileRepository.GetAllList().Where(s => ((s.UserID == UserID) && (s.STATE == 0))).ToList();
            //    return list.Count;
            //}
            //catch (Exception e)
            //{

            //    throw e;
            //}
            throw new NotImplementedException();
        }

        public void Insert(InfoEarthFrame.Core.ConvertFile entity)
        {
            //entity.STATE = 0;
            //_ConvertFileRepository.Insert(entity);
            throw new NotImplementedException();
        }

        public void UpdateState(string Id)
        {

            //InfoEarthFrame.Core.ConvertFile entity = _ConvertFileRepository.Get(Id);
            //entity.Id = Id;
            //entity.STATE = 1;
            //_ConvertFileRepository.Update(entity);
            throw new NotImplementedException();
        }

        /// <summary>
        /// 格式转换
        /// </summary>
        /// <param name="Files">文件列表</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="type">转换类型</param>
        /// <param name="IsZip">是否压缩</param>
        /// <returns>转换结果</returns>
        public ConvertResult DataConvert1(List<string> Files, string UserID, int type, string OutputCoordName, bool IsZip, string ConvertKey)
        {
            if (Files == null || Files.Count <= 0)
            {
                return new ConvertResult();
            }

            //DataFileType dfType;

            //if (type == 1)
            //{
            //    dfType = DataFileType.FormatConvert;
            //}
            //else if (type == 2)
            //{
            //    dfType = DataFileType.CoordinateConvert;
            //}
            //else
            //{
            //    dfType = DataFileType.Projection;
            //}
            List<ConvertFileList> fileList = new List<ConvertFileList>();
            foreach (string f in Files)
            {
                string[] ss = f.Split(',');
                if (ss.Length <= 0)
                {
                    return null;
                }

                FileInfo fi = new FileInfo(ss[0]);
                ConvertFileList cf = new ConvertFileList();
                cf.ID = Guid.NewGuid().ToString();
                cf.LogicFileName = fi.Name;
                cf.PhysicsFilePath = f;
                cf.FileType = type;
                cf.ConvertFilePath = "";
                cf.ConvertResult = 0;
                cf.ConvertMsg = "";
                cf.ConvertKey = ConvertKey;
                if (ss.Length >= 2)
                {
                    cf.SrcCoordName = "";
                }
                else
                {
                    cf.SrcCoordName = "";
                }
                fileList.Add(cf);
            }

            return DataConvert(fileList, UserID, OutputCoordName, IsZip);
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="IsZip"></param>
        /// <returns></returns>
        public ConvertResult DataConvert(List<ConvertFileList> fileList, string UserID, string OutputCoordName, bool IsZip)
        {


            if (fileList == null || fileList.Count <= 0)
            {
                return new ConvertResult();
            }

            string ConvertFolder = ConfigurationManager.AppSettings["ConvertExprotPath"] + Guid.NewGuid().ToString();
            string ConvertTypeName = "";


            string OutCoordFile = "";
            if (OutputCoordName.ToUpper() != "NULL")
            {
                OutCoordFile = GetCoordName(OutputCoordName);
            }

          //  var fileTypeList = _IDicDataCodeRepository.GetAllList().Where(q => q.DataTypeID == "25159792-cdba-11e7-a735-005056bb1c7e").ToList();

            foreach (ConvertFileList f in fileList)
            {
                FileInfo ff = new FileInfo(f.PhysicsFilePath);
                f.ID = Guid.NewGuid().ToString();
                f.LogicFileName = ff.Name;
                f.ConvertResult = 0;
                try
                {
                    // 检测文件是否存在
                    if (!File.Exists(f.PhysicsFilePath))
                    {
                        f.ConvertMsg = UtilityMessageConvert.Get("转换文件不存在！");
                        f.ConvertResult = 0;
                        continue;
                    }

                    FileInfo fi = null;
                    // 创建转换保存目录
                    Directory.CreateDirectory(ConvertFolder);
                    if (!Directory.Exists(ConvertFolder))
                    {
                        Directory.CreateDirectory(ConvertFolder);
                    }

                    string newFile = "";

                    // 转换文件
                    switch (f.FileType)
                    {
                        case 1:
                            ConvertTypeName = UtilityMessageConvert.Get("格式转换").Trim();

                            try
                            {
                                string[] Files = (string.IsNullOrEmpty(f.ConvertKey)) ? Mapgis2Arcgis(f.PhysicsFilePath) : FileFormatConvert(ff, f.ConvertKey, OutputFormats.ToList());
                                if (Files != null)
                                {
                                    foreach (string s in Files)
                                    {
                                        fi = new FileInfo(s);
                                        if (fi.Extension.ToUpper() != ".LOG")
                                        {
                                            newFile = ConvertFolder + "\\" + fi.Name;
                                            if (f.ConvertFilePath != "")
                                            {
                                                newFile = f.ConvertFilePath + "\\" + fi.Name;
                                            }

                                            File.Copy(s, ConvertFolder + "\\" + fi.Name, true);
                                            File.Copy(s, newFile, true);
                                        }

                                        try
                                        {
                                            File.Delete(s);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    if (f.ConvertFilePath != "")
                                    {
                                        f.ConvertFilePath = f.ConvertFilePath + "\\" + f.LogicFileName.Substring(0, f.LogicFileName.IndexOf('.'));
                                    }
                                    else
                                    {
                                        f.ConvertFilePath = ConvertFolder + "\\" + f.LogicFileName.Substring(0, f.LogicFileName.IndexOf('.'));
                                    }
                                    f.ConvertFolder = ConvertFolder;
                                    f.ConvertResult = 1;
                                    f.ConvertMsg = UtilityMessageConvert.Get("转换成功");
                                  //  _logger.Debug(f.LogicFileName + "转换成功");
                                }
                                else
                                {
                                    f.ConvertResult = 0;
                                    f.ConvertMsg = UtilityMessageConvert.Get("转换文件失败");
                                  //  _logger.Debug(f.LogicFileName + "转换文件失败");
                                }
                            }
                            catch (Exception ex)
                            {
                                f.ConvertResult = 0;
                                f.ConvertMsg = UtilityMessageConvert.Get("shp文件错误"); // ex.Message;
                              //  _logger.Debug(f.LogicFileName + "shp文件错误");
                            }
                            break;
                        case 2: // 坐标
                            ConvertTypeName = UtilityMessageConvert.Get("坐标转换").Trim();
                            fi = new FileInfo(f.PhysicsFilePath);
                            newFile = ConvertFolder + "\\" + fi.Name;
                            if (f.ConvertFilePath != "")
                            {
                                newFile = f.ConvertFilePath + "\\" + fi.Name;
                            }

                            if (f.CoordPoint != null && f.CoordPoint.Length > 0)
                            {
                                CoordTransformHelper ctf = new CoordTransformHelper();

                                if (f.CoordPoint[0].Contains(','))
                                {
                                    ErrorMsg = (!ctf.ControlPointTransform(f.PhysicsFilePath, newFile, f.CoordName, f.CoordPoint)) ? UtilityMessageConvert.Get("转换失败") : "";
                                }
                                else
                                {
                                    ErrorMsg = (!ctf.SevenParameterTransform(f.PhysicsFilePath, newFile, f.CoordName, f.CoordPoint)) ? UtilityMessageConvert.Get("转换失败") : "";
                                }
                            }
                            else //维持以前逻辑
                            {
                                VectorCoordTransform(f.PhysicsFilePath, OutCoordFile, newFile);
                            }
                            if (ErrorMsg.Trim().Length > 0 || InfoMsg.Trim() == UtilityMessageConvert.Get("转换失败"))
                            {
                                f.ConvertFilePath = "";
                                f.ConvertMsg = UtilityMessageConvert.Get("shp文件错误");// ErrorMsg != "" ? ErrorMsg : InfoMsg;
                                f.ConvertResult = 0;
                            }
                            else
                            {
                                f.ConvertFilePath = newFile;
                                f.ConvertResult = 1;
                                f.ConvertMsg = UtilityMessageConvert.Get("转换成功");
                            }
                            ErrorMsg = "";

                            break;
                        case 3: // 投影
                            ConvertTypeName = UtilityMessageConvert.Get("投影转换".Trim());

                            fi = new FileInfo(f.PhysicsFilePath);
                            newFile = ConvertFolder + "\\" + fi.Name;
                            if (f.ConvertFilePath != "")
                            {
                                newFile = f.ConvertFilePath + "\\" + fi.Name;
                            }
                           VectorCoordTransform(f.PhysicsFilePath, OutCoordFile, newFile);
                            //iTelluro.DataTools.PrjTransform.VectorCoordTransform(f.PhysicsFilePath, OutCoordFile, newFile, Error, Info);
                            if (ErrorMsg.Trim().Length > 0 || InfoMsg.Trim() == UtilityMessageConvert.Get("转换失败"))
                            {
                                f.ConvertFilePath = "";
                                f.ConvertMsg = UtilityMessageConvert.Get("shp文件错误"); // ErrorMsg != "" ? ErrorMsg : InfoMsg;
                                f.ConvertResult = 0;
                            }
                            else
                            {
                                f.ConvertFilePath = newFile;
                                f.ConvertResult = 1;
                                f.ConvertMsg = UtilityMessageConvert.Get("转换成功");
                            }
                            ErrorMsg = "";

                            break;
                        default:
                            f.ConvertMsg = "";
                            f.ConvertResult = 0;
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    f.ConvertMsg = ex.Message;
                    f.ConvertResult = 0;
                }
            }

            ConvertResult ret = new ConvertResult();
            ret.fileList = fileList;


            bool bAllFail = true;
            foreach (var v in fileList)
            {
                if (v.ConvertResult == 1)
                {
                    bAllFail = false;
                    break;
                }
            }



            if (IsZip == true && bAllFail == false)
            {
                string DownFile = ConvertTypeName.Replace(" ", "") + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".ZIP"; // Guid.NewGuid().ToString() + ".ZIP";
                // 压缩转换成功文件
                ZipFile.CreateFromDirectory(ConvertFolder, ConfigurationManager.AppSettings["ConvertExprotPath"] + DownFile);

                try
                {
                    Directory.Delete(ConvertFolder, true);
                }
                catch
                {

                }
                ret.ZipFileName = DownFile;

                InfoEarthFrame.Core.ConvertFile entity = new ConvertFile();
                entity.Id = Guid.NewGuid().ToString();
                entity.UserID = UserID;
                entity.PhysicsFilePath = ConfigurationManager.AppSettings["ConvertExprotPath"] + DownFile;
                FileInfo fi = new FileInfo(ConfigurationManager.AppSettings["ConvertExprotPath"] + DownFile);
                entity.FileName = ConvertTypeName + "_" + DateTime.Now.ToString("yyyyMMdd HH:mm");
                foreach (ConvertFileList r in ret.fileList)
                {
                    if (r.ConvertResult == 1)
                    {
                        FileInfo rFi = new FileInfo(r.PhysicsFilePath);
                        entity.ConvertFileNames += rFi.Name.Substring(0, rFi.Name.IndexOf('.')) + ",";
                    }
                }
                if (entity.ConvertFileNames.Length > 0)
                {
                    entity.ConvertFileNames = entity.ConvertFileNames.Remove(entity.ConvertFileNames.Length - 1);
                }
                entity.CreateTime = DateTime.Now;
                entity.FileType = ret.fileList[0].FileType;// FileTypeToNumber();

                try
                {
                    Insert(entity);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (IsZip == true)
            {
                // 添加空间参考
                for (int i = 0; i < fileList.Count; i++)
                {
                    if (fileList[i].ConvertResult == 1)
                    {
                        string ff = fileList[i].ConvertFilePath;
                        if (fileList[i].ConvertFilePath.Substring(fileList[i].ConvertFilePath.Length - 3).ToUpper() != "SHP")
                        {
                            ff += ".shp";
                        }
                        try
                        {

                            ShpReader sr = new ShpReader(ff);
                            string wkt = sr.GetSridWkt();
                            if (!string.IsNullOrEmpty(wkt))
                            {
                                SpatialReference srf = new SpatialReference(wkt);
                                string strHTML = srf.__str__().Replace("\n", "<br/>").Replace("[\"", ":").Replace("]", "");
                                fileList[i].CoordName = wkt.Substring(wkt.IndexOf("\"") + 1, wkt.IndexOf(",") - wkt.IndexOf("\"") - 2);
                                fileList[i].WKT = strHTML;
                            }
                        }
                        catch
                        {
                            fileList[i].CoordName = "";
                            fileList[i].WKT = "";
                        }
                    }
                }
            }

            return ret;
        }

        private int FileTypeToNumber(DataFileType type)
        {
            switch (type)
            {
                case DataFileType.FormatConvert:
                    return 1;
                case DataFileType.CoordinateConvert:
                    return 2;
                case DataFileType.Projection:
                    return 3;
                default:
                    return 0;
            }
        }

        private void Info(string info)
        {
            InfoMsg = info;
        }

        private void Error(string info)
        {
            ErrorMsg = info;
        }


        /// <summary>
        /// 根据名称获得空间参考文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetCoordName(string name)
        {
            string[] Files = Directory.GetFiles(ConfigurationManager.AppSettings["CoordPath"], "*.*", SearchOption.AllDirectories);
            foreach (string f in Files)
            {
                FileInfo fi = new FileInfo(f);
                if (fi.Name.IndexOf(name) != -1)
                {
                    return f;
                }
            }

            return "";
        }

        public string[] Mapgis2Arcgis(string filename)
        {
            List<string> filenames = new List<string>();

            //Process p = null;
            DateTime dtStart = DateTime.Now;
            object fileName = filename;
            if (fileName == null || string.IsNullOrEmpty(fileName.ToString()))
            {
                return null;
            }
            //else
            //{
            //    p = new Process();
            //}

            Environment.CurrentDirectory = ConfigurationManager.AppSettings["Map2ShpPath"];
            //设定程序名
            //p.StartInfo.FileName =  ConfigContext.Current.DefaultConfig["Map2ShpPath"] + "iTelluro.DataTools.Console.exe";
            //p.StartInfo.FileName = "cmd.exe";
            ////关闭Shell的使用
            //p.StartInfo.UseShellExecute = false;
            ////重定向标准输入
            //p.StartInfo.RedirectStandardInput = true;
            ////重定向标准输出
            //p.StartInfo.RedirectStandardOutput = true;
            ////重定向错误输出
            //p.StartInfo.RedirectStandardError = true;
            //设置不显示窗口
            string dir = Path.Combine(Path.GetDirectoryName(filename), "ShapeData"); ;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                _logger.Debug(ConfigurationManager.AppSettings["Map2ShpPath"] + "iTelluro.DataTools.Console.exe" + " Map2Shp -s:" + fileName + " -t:" + dir);
                //Process.EnterDebugMode(); // 等待完成
                Process.Start(ConfigurationManager.AppSettings["Map2ShpPath"] + "iTelluro.DataTools.Console.exe", " Map2Shp -s:" + fileName + " -t:" + dir).WaitForExit(30 * 1000);
            
                DirectoryInfo folder = new DirectoryInfo(dir);
                foreach (FileInfo file in folder.GetFiles())
                {
                    filenames.Add(file.FullName);
                }
                string[] filenameslst = (string[])filenames.ToArray();
                return filenameslst;
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                return null;
            }
            //p.StartInfo.CreateNoWindow = true;
            //p.StartInfo.Arguments = " Map2Shp -s:" + fileName + " -t:" + dir;


            //p.Start();
            //p.WaitForExit(1000 * 60);
            //if (p.ExitCode == 0)
            //{
            //    DirectoryInfo folder = new DirectoryInfo(dir);
            //    foreach (FileInfo file in folder.GetFiles())
            //    {
            //        filenames.Add(file.FullName);
            //    }
            //    string[]  filenameslst = (string[])filenames.ToArray();
            //    return filenameslst;

            //}
            //else
            //{
            //    return null;
            //}
        }

        public static ConvertResult FormatConvert(IEnumerable<ConvertFileList> files, IList<string> errorInfo, IDataConvertAppService dataConvertAppService)
        {

            //TODO:这里后面可能要考虑shp文件。如果是的话直接返回一个结果
            List<ConvertFileList> FCFileList = new List<ConvertFileList>();
            foreach (var s in files)
            {
                FileInfo fi = new FileInfo(s.PhysicsFilePath);
                if (fi.Extension.ToUpper() == ".WP" || fi.Extension.ToUpper() == ".WL" || fi.Extension.ToUpper() == ".WT")
                {
                    ConvertFileList cf = new ConvertFileList();
                    cf.ID = Guid.NewGuid().ToString();
                    cf.LogicFileName = fi.Name;
                    cf.PhysicsFilePath = s.PhysicsFilePath;
                    cf.ConvertResult = 0;
                    cf.ConvertFilePath = "";
                    cf.ConvertMsg = "";
                    cf.SrcCoordName = "";
                    cf.FileType = (int)DataFileType.FormatConvert;

                    FCFileList.Add(cf);

                    
                }

            }
            if (FCFileList.Count <= 0)
            {
                if (!files.Any(p => p.PhysicsFilePath.ToLower().Contains(".shp")))
                {
                    errorInfo.Add("格式转换：未找到图层文件");
                    return null;
                }

                return new ConvertResult
                {
                    fileList = files.Where(p => !p.PhysicsFilePath.ToLower().Contains( ".zip") && !p.PhysicsFilePath.ToLower().Contains(".rar")).ToList()
                };
            }
            return dataConvertAppService.DataConvert(FCFileList, "", "", false);
        }

        public static ConvertResult CoordinateConvert(IEnumerable<ConvertFileList> files,IList<string> errorInfo, IDataConvertAppService dataConvertAppService)
        {
            List<ConvertFileList> CCFileList = new List<ConvertFileList>();
            foreach (var f in files)
            {
                if (f.ConvertResult == 0)
                {
                    errorInfo.Add("格式转换失败： " + f.ConvertMsg + "  文件：" + f.LogicFileName);
                    return null;
                }

                FileInfo fi = new FileInfo(f.PhysicsFilePath);

                ConvertFileList cf = new ConvertFileList();
                cf.ID = Guid.NewGuid().ToString();
                cf.LogicFileName = fi.Name;
                cf.PhysicsFilePath = f.ConvertFilePath + ".shp";
                cf.ConvertResult = 0;
                cf.ConvertFilePath = fi.DirectoryName;
                cf.ConvertMsg = "";
                cf.SrcCoordName = "";
                cf.FileType = (int)DataFileType.CoordinateConvert;

                CCFileList.Add(cf);
            }
            return dataConvertAppService.DataConvert(CCFileList, "", "Xian 1980", false);
        }


        public static ConvertResult ProjectionConvert(IEnumerable<ConvertFileList> files, IList<string> errorInfo,IDataConvertAppService dataConvertAppService,string OutputCoordName="")
        {
            List<ConvertFileList> PFileList = new List<ConvertFileList>();

            foreach (var f in files)
            {
                FileInfo fi = new FileInfo(f.ConvertFilePath);

                ConvertFileList cf = new ConvertFileList();
                cf.ID = Guid.NewGuid().ToString();
                cf.LogicFileName = fi.Name;
                cf.PhysicsFilePath = f.ConvertFilePath;
                cf.ConvertResult = 0;
                cf.ConvertFilePath = fi.DirectoryName;
                cf.ConvertMsg = "";
                cf.FileType = (int)DataFileType.Projection;

                PFileList.Add(cf);
            }
            return dataConvertAppService.DataConvert(PFileList, "", OutputCoordName, false);
        }


        public string[] FileFormatConvert(FileInfo fileName, string convertKey, List<InfoEarthFrame.Core.Entities.DicDataCodeEntity> fileTypeList)
        {


            List<string> filenames = new List<string>();

            DateTime dtStart = DateTime.Now;
            object fileNameO = fileName.FullName;
            var fileType = fileTypeList.Where(q => q.CodeName.Equals(convertKey));
            string suffix = convertKey;
            if (fileType != null && fileType.Count() > 0)
            {
                var obj = fileType.FirstOrDefault();
                suffix = obj.CodeValue;
            }
            else
            {
                if (fileNameO == null || string.IsNullOrEmpty(fileNameO.ToString()))
                {
                    return null;
                }
            }

            //Environment.CurrentDirectory =  ConfigContext.Current.DefaultConfig["Map2ShpPath"];
            Environment.CurrentDirectory = AppDomain.CurrentDomain.RelativeSearchPath;
            string name = fileName.Name.Substring(0, fileName.Name.IndexOf('.')) + suffix;
            string dir = Path.Combine(fileName.DirectoryName, "ConvertData");
            string fileN = Path.Combine(dir, name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                //Process.EnterDebugMode(); // 等待完成
                string path = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "ogr2ogr.exe");
                Process.Start(path, " --config GDAL_FILENAME_IS_UTF8 NO -f \"" + convertKey + "\" \"" + fileN + "\" \"" + fileNameO + "\"").WaitForExit(30 * 1000);

                DirectoryInfo folder = new DirectoryInfo(dir);
                foreach (FileInfo file in folder.GetFiles())
                {
                    filenames.Add(file.FullName);
                }
                string[] filenameslst = (string[])filenames.ToArray();
                return filenameslst;
            }
            catch
            {
                return null;
            }
        }


        public List<string> GetCoordList(List<string> fileList)
        {
            List<string> ret = new List<string>();
            foreach (string s in fileList)
            {
                ShpReader sr = new ShpReader(s);
                string wkt = sr.GetSridWkt();
                if (!string.IsNullOrEmpty(wkt))
                {
                    SpatialReference srf = new SpatialReference(wkt);
                    string strHTML = srf.__str__().Replace("\n", "<br/>").Replace("[\"", ":").Replace("]", "");
                    string Lyr = wkt.Substring(wkt.IndexOf("\"") + 1, wkt.IndexOf(",") - wkt.IndexOf("\"") - 2);
                    ret.Add(s + "|" + Lyr + "|" + strHTML);
                }

            }

            return ret;
        }

        public List<string> GetlyrTypeList(List<string> fileList)
        {
            List<string> ret = new List<string>();
            foreach (string s in fileList)
            {
                string[] tempS = s.Split('\\');
                string Lyr = GetlyrTypeByFile(s);

                if (Lyr == "")
                {
                    ret.Add(s);
                }
                else
                {
                    ret.Add(tempS[tempS.Length - 1] + "," + Lyr.Replace("/", ","));
                }
            }

            return ret;
        }

        /// <summary>
        /// 数据检查 
        /// </summary>
        /// <param name="fileList">文件列表</param>
        /// <returns>转换信息</returns>
        public DataCheckResult DataCheck(List<string> fileList, bool IsExcel)
        {
            List<LayerArgs> _lyrArgsList = new List<LayerArgs>();

            if (fileList.Count <= 0)
            {
                return null;
            }

            List<DataChackLog> _checkLogs = new List<DataChackLog>();

            foreach (string s in fileList)
            {
                if (ExtCheck(s, "DataCheckExtension") == false)
                {
                    _checkLogs.Add(new DataChackLog(s, new List<string> { UtilityMessageConvert.Get("属性检查：文件类型错误") }));
                    continue;
                }


                string Lyr = GetlyrTypeByFile(s);

                //if (Lyr == "")
                //{
                //    Lyr = GetlyrTypeByFolder(s);
                //}


                if (Lyr == "")
                {
                    _checkLogs.Add(new DataChackLog(s, new List<string> { UtilityMessageConvert.Get("属性检查：不在检查范围内") }));
                    continue;
                }

                string[] arr = Lyr.Split('/');
                LayerArgs args = new LayerArgs()
                {
                    TxName = arr[0],
                    TjName = arr[1],
                    LayerName = arr[2],
                    LayerPath = s,
                    DataLayer = arr[3]
                };

                _lyrArgsList.Add(args);
            }

            DicInfoReader _dicReader = new DicInfoReader();

            List<string> errorInfo = new List<string>();

            for (int i = 0; i < _lyrArgsList.Count; i++)
            {
                try
                {
                    List<string> rlts = LayerAttChecker.CheckLayer(_lyrArgsList[i], _dicReader);
                    if (rlts != null && rlts.Count > 0)
                    {
                        _checkLogs.Add(new DataChackLog(_lyrArgsList[i].LayerPath, rlts));
                    }
                }
                catch (Exception ex)
                {
                    _checkLogs.Add(new DataChackLog(_lyrArgsList[i].LayerPath, new List<string> { "属性检查：" + ex.Message }));
                }
            }

            DataCheckResult ret = new DataCheckResult();
            ret.CheckInfoList = _checkLogs;

            if (IsExcel == true)
            {
                string ExcelFile = ExprotCheck(_checkLogs, _lyrArgsList);
                ret.ExcelFile = ExcelFile;
            }
            else
            {
                ret.ExcelFile = "";
            }

            return ret;
        }

        private string ExprotCheck(List<DataChackLog> info, List<LayerArgs> fileList)
        {
            var book = new XSSFWorkbook();
            var xlsTmpPath = System.AppDomain.CurrentDomain.BaseDirectory + "Templage\\CheckTemplate.xlsx";


            var file = new FileStream(xlsTmpPath, FileMode.Open, FileAccess.Read);
            var hssfworkbook = new XSSFWorkbook(file);
            var sheet = hssfworkbook.GetSheet("Sheet1");
            var guid = Guid.NewGuid().ToString();
            var saveFileName = ConfigurationManager.AppSettings["ConvertExprotPath"] + guid + ".xlsx";
            int currRow = 1;

            for (int i = 0; i < info.ToList().Count; i++)
            {
                for (int j = 0; j < fileList.Count; j++)
                {
                    if (fileList[j].LayerPath == info.ToList()[i].FileName)
                    {
                        FileInfo fi = new FileInfo(fileList[j].LayerPath);

                        var dataRow = sheet.CreateRow(currRow) as XSSFRow;
                        dataRow.CreateCell(0).SetCellValue(fi.Name);
                        dataRow.CreateCell(1).SetCellValue(fileList[j].TxName);
                        dataRow.CreateCell(2).SetCellValue(fileList[j].TjName);
                        dataRow.CreateCell(3).SetCellValue(fileList[j].LayerName);
                        if (info.ToList()[i].Log.Count > 0)
                        {
                            dataRow.CreateCell(4).SetCellValue(info.ToList()[i].Log[0]);
                        }
                        currRow++;
                        break;
                    }
                }

                for (int k = 1; k < info.ToList()[i].Log.Count; k++)
                {
                    var dataRow = sheet.CreateRow(currRow) as XSSFRow;
                    dataRow.CreateCell(0).SetCellValue("");
                    dataRow.CreateCell(1).SetCellValue("");
                    dataRow.CreateCell(2).SetCellValue("");
                    dataRow.CreateCell(3).SetCellValue("");
                    dataRow.CreateCell(4).SetCellValue(info.ToList()[i].Log[k]);

                    currRow++;
                }
            }

            sheet.ForceFormulaRecalculation = true;
            using (var fileWrite = new FileStream(saveFileName, FileMode.Create))
            {
                hssfworkbook.Write(fileWrite);
            }

            hssfworkbook = null;
            sheet = null;

            return saveFileName;
        }

        /// <summary>
        /// 按文件名获得
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private string GetlyrTypeByFile(string FilePath)
        {
            InfoEarthFrame.Data.IDatabase DBAccess = InfoEarthFrame.Data.Factory.GetDBAccess(ConfigurationManager.ConnectionStrings["Default"].ConnectionString, InfoEarthFrame.Data.AccessDBType.Oracle);

            string defaultValue = ""; // "地质环境条件类/中国地质环境分区图/地质环境分区";

            FileInfo fi = new FileInfo(FilePath);
            if (fi.Name.Length >= 8)
            {
                string code = fi.Name.Substring(3, 6);
                try
                {
                    int a = int.Parse(code);
                    if (a >= 101011 && a <= 502051)
                    {
                        //List<Core.ElementType> TypeList = _ElementTypeRepository.GetAll().Where(s => s.TCDM.IndexOf(a.ToString()) != -1).ToList();
                        //if (TypeList.Count > 0)
                        //{
                        //    return TypeList[0].TXName + "/" + TypeList[0].TJName + "/" + TypeList[0].LayerName;
                        //}
                        //else
                        //{
                        //    return defaultValue;
                        //}
                        DataTable objDT = DBAccess.GetDataSetFromExcuteCommand("select * from DIC_ElementType where TCDM like '%" + a + "%'").Tables[0];
                        if (objDT.Rows.Count > 0)
                        {
                            return objDT.Rows[0]["TXName"] + "/" + objDT.Rows[0]["TJName"] + "/" + objDT.Rows[0]["LayerName"] + "/" + objDT.Rows[0]["TCDM"];
                        }
                        else
                        {
                            return defaultValue;
                        }
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
                catch
                {
                    return defaultValue;
                }

            }
            return defaultValue;
        }

        /// <summary>
        /// 按目录获得
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private string GetlyrTypeByFolder(string FilePath)
        {
            InfoEarthFrame.Data.IDatabase DBAccess = InfoEarthFrame.Data.Factory.GetDBAccess(ConfigurationManager.ConnectionStrings["Default"].ConnectionString, InfoEarthFrame.Data.AccessDBType.Oracle);

            string defaultValue = ""; // "地质环境条件类/中国地质环境分区图/地质环境分区";
            string[] FolderCC = FilePath.Split('\\');

            foreach (string s in FolderCC)
            {
                string cf = ConvertFolderName(s);
                if (cf != "")
                {
                    //List<Core.ElementType> TypeList = _ElementTypeRepository.GetAll().Where(a => a.TJName.IndexOf(cf.ToString()) != -1).ToList();
                    //if (TypeList.Count > 0)
                    //{
                    //    return TypeList[0].TXName + "/" + TypeList[0].TJName + "/" + TypeList[0].LayerName;
                    //}
                    //else
                    //{
                    //    return defaultValue;
                    //}


                    DataTable objDT = DBAccess.GetDataSetFromExcuteCommand("select * from DIC_ElementType where \"TJName\" like '%" + cf + "%'").Tables[0];
                    if (objDT.Rows.Count > 0)
                    {
                        defaultValue = objDT.Rows[0]["TXName"] + "/" + objDT.Rows[0]["TJName"] + "/" + objDT.Rows[0]["LayerName"];
                    }
                }
            }

            return defaultValue;
        }

        private string ConvertFolderName(string folderName)
        {
            int start = -1;
            for (int i = 0; i < folderName.Length; i++)
            {
                if (folderName[i] >= '0' && folderName[i] <= '9')
                {
                    continue;
                }
                else
                {
                    start = i;
                    break;
                }
            }

            if (start != -1)
            {
                return folderName.Substring(start);
            }
            else
            {
                return "";
            }
        }

        private bool ExtCheck(string FilePath, string ConfigName)
        {
            string[] DataCheckExt = ConfigurationManager.AppSettings[ConfigName].Split(',');

            bool b = false;

            FileInfo fi = new FileInfo(FilePath);
            foreach (string ext in DataCheckExt)
            {
                if (fi.Extension.ToLower() == ext)
                {
                    b = true;
                    break;
                }
            }

            return b;
        }

        private void VectorCoordTransform(string physicsFilePath, string OutCoordFile, string newFile)
        {
            try
            {
                GdalHelp.InitGdal();

                ShpReader sr = new ShpReader(physicsFilePath);
                string csSrc = sr.GetSridWkt();
                string csDest = File.ReadAllText(OutCoordFile);
                iTelluro.DataTools.Utility.GIS.CoordTransform.Ogrtransform(csSrc, csDest, physicsFilePath, newFile);
                if (File.Exists(newFile))
                {
                    Info(UtilityMessageConvert.Get("转换成功"));
                }
                else
                {
                    Error(UtilityMessageConvert.Get("转换失败"));
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.LogException(ex);
                throw;
            }
        }
    }

    public class ConvertFileQuery
    {
        public string token { get; set; }

        //public string districtName { get; set; }
    }
}
