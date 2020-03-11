using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;


namespace InfoEarthFrame.Common
{
    [XmlRoot]
    public class Metadata
    {
        private static readonly Cache Cache = System.Web.HttpRuntime.Cache;
        private static readonly object LockObj=new object();
        private static readonly Encoding CurrentEncoding = Encoding.GetEncoding("gb2312");

        /// <summary>
        /// 读取xml并获取对象
        /// </summary>
        /// <param name="ConfigContextKey"></param>
        /// <returns></returns>
        public static Metadata GetInstance(string mainId,string folderName,string ftpPath)
        {

            try
            {
                var key=mainId + "/" + folderName;
                var obj = Cache[key];
                if (obj == null)
                {
                    lock (LockObj)
                    {
                         obj = Cache[key];
                         if (obj == null)
                         { 
                            //从FTP下面读取
                             var ftpConfig=ConfigContext.Current.FtpConfig["Package"];
                             using (var client = new FtpHelper(ftpConfig))
                             {
                                 var localFilePath=Path.Combine(GetTempXmlDirectory(mainId,folderName), Path.GetFileName(ftpPath));
                                 client.Get(localFilePath, ftpPath);

                                 using (var fs =
                                     new FileStream(localFilePath,
                                         FileMode.Open))
                                 {
                                     using (var sr = new StreamReader(fs,CurrentEncoding))
                                     {
                                         var context =
                                            XmlConvert.XmlDeserialize<Metadata>(sr.ReadToEnd(), CurrentEncoding);
                                         Cache.Insert(key, context);
                                         return context;
                                     }
                                 }
                             }
                         }
                    }
                }

                return (Metadata)obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="folderName"></param>
        public void Save()
        {
            if (XmlFilePath.EndsWith("元数据.xml"))
            {
                using (var fs = new FileStream(XmlFilePath, FileMode.Create))
                {
                    using (var sr = new StreamWriter(fs, CurrentEncoding))
                    {
                        var str = InfoEarthFrame.Common.XmlConvert.XmlSerialize<Metadata>(this);
                        sr.Write(str);
                    }
                }
            }

            //上传文件到FTP
            var ftp = ConfigContext.Current.FtpConfig["Package"];
            if (ftp == null)
            {
                throw new Exception("未找到名称为[Package]的Ftp配置项");
            }

            using (var client = new FtpHelper(ftp))
            {
                var workingDir = "Package\\" + this.MainId + "\\" + this.FolderName;
                client.UploadFiles(new List<string> { XmlFile.FullName }, workingDir);
                XmlFtpPath = workingDir + "\\" + XmlFile.Name;
            }

            //缓存失效
            lock (LockObj)
            {
                var key = this.MainId + "/" + this.FolderName;
                Cache.Remove(key);
            }
        }

        protected  static string GetTempXmlDirectory(string mainId,string folderName)
        {
            var tempDir = Path.Combine(HttpContext.Current.Server.MapPath("~"), ConfigContext.Current.DefaultConfig["upload:tempdir"], mainId, folderName);
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            return tempDir;
        }
        [XmlElement]
        public string mdTitle { get; set; }

        [XmlElement]
        public string mdDataSt { get; set; }

        [XmlElement]
        public string mdChar { get; set; }


        [XmlElement]
        public string mdStanName { get; set; }

        [XmlElement]
        public string mdStanVer { get; set; }

        [XmlElement]
        public string mdLang { get; set; }

        private dataIdInfo _dataIdInfo;

        [XmlElement]
        public dataIdInfo dataIdInfo
        {
            get
            {
                if (_dataIdInfo == null)
                {
                    _dataIdInfo = new dataIdInfo();
                }

                return _dataIdInfo;
            }
            set { _dataIdInfo = value; }
        }

        private dqInfo _dqInfo;

        [XmlElement]
        public dqInfo dqInfo
        {
            get
            {
                if (_dqInfo == null)
                {
                    _dqInfo = new dqInfo();
                }

                return _dqInfo;
            }
            set { _dqInfo = value; }
        }

        private refSysInfo _refSysInfo;

        [XmlElement]
        public refSysInfo refSysInfo
        {
            get
            {
                if (_refSysInfo == null)
                {
                    _refSysInfo = new refSysInfo();
                }

                return _refSysInfo;
            }
            set { _refSysInfo = value; }
        }


        private distInfo _distInfo;

        [XmlElement]
        public distInfo distInfo
        {
            get
            {
                if (_distInfo == null)
                {
                    _distInfo = new distInfo();
                }

                return _distInfo;
            }
            set { _distInfo = value; }
        }

        private conInfo _conInfo;

        [XmlElement]
        public conInfo conInfo
        {
            get
            {
                if (_conInfo == null)
                {
                    _conInfo = new conInfo();
                }

                return _conInfo;
            }
            set { _conInfo = value; }
        }

        private mdContact _mdContact;

        [XmlElement]
        public mdContact mdContact
        {
            get
            {
                if (_mdContact == null)
                {
                    _mdContact = new mdContact();
                }

                return _mdContact;
            }
            set { _mdContact = value; }
        }

        private string _collectTime;

        [XmlIgnore]
        public string CollectTime
        {
            get
            {
                return _collectTime=dataIdInfo.TempExtent.begin + "到" + dataIdInfo.TempExtent.end;
            }
            set
            {
                _collectTime = value??"";

                var strs = _collectTime.Split('到');
                if (strs.Length == 2)
                {
                    dataIdInfo.TempExtent.begin = strs[0];
                    dataIdInfo.TempExtent.end = strs[1];
                }
            }
        }

        private string _keyword;
        [XmlIgnore]
        public string Keyword
        {
            get
            {
                return _keyword;
            }
            set {
                _keyword = value;
                var strs = (_keyword ?? "").Split('|');
                if (strs.Any())
                {
                    var list = new List<KeyWords>();
                    foreach (var item in strs)
                    {
                            var key = new KeyWords
                            {
                                keyTyp = item,
                                keyword = item
                            };
                            list.Add(key);
                    }
                    dataIdInfo.KeyWordsList = list;
                }
            }
        }


        [XmlIgnore]
        public string TopicCategory
        {
            get
            {
                return "";
            }
        }

        [XmlIgnore]
        public string MainId
        {
            get;
            set;
        }


        [XmlIgnore]
        public string FolderName
        {
            get;
            set;
        }

        [XmlIgnore]
        public FileInfo XmlFile
        {
            get
            {
                return new FileInfo(XmlFilePath);
            }
        }

        private string _xmlFilePath;
        [XmlIgnore]
        public string XmlFilePath
        {
            get {
                if (string.IsNullOrEmpty(_xmlFilePath))
                {
                 _xmlFilePath=   Path.Combine(GetTempXmlDirectory(this.MainId, this.FolderName), "元数据.xml");
                }
                return _xmlFilePath;
            }
            set
            {
                _xmlFilePath = value;
            }
        }

        
        [XmlIgnore]
        public string XmlFtpPath
        { 
           get;
            private set;
        }

        [XmlIgnore]
        public string CreateUserId
        {
            get;
             set;
        }

        [XmlIgnore]
        public List<ContactInfo> ContactDetails
        {
            get
            {
                var list = new List<ContactInfo>();
                list.Add(new ContactInfo
                {
                    DeptName = mdContact.rpOrgName,
                    NO = 1
                });
                return list;
            }
        }

        [XmlIgnore]
        public string ThumbFile
        {
            get;
            set;
        }
    }

    public class ContactInfo
    {
       public int NO{get;set;}

        public string DeptName{get;set;}

        public string Duty
        {
         get
         {
         return "内容提供者";
         }
        }
    }
}