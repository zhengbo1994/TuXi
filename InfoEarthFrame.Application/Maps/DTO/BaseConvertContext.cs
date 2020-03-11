using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfoEarthFrame.Maps.DTO
{
    public class BaseConvertContext:IConvertContext
    {
        private readonly IDataConvertAppService _dataConvertAppService;
        private readonly string _mainId;
        public BaseConvertContext(IDataConvertAppService dataConvertAppService,string mainId)
        {
            this._dataConvertAppService = dataConvertAppService;
            this._mainId = mainId;
        }
        public IDataConvertAppService ConvertService
        {
            get 
            { 
                return _dataConvertAppService;
            }
        }

        public string MainId
        {
            get 
            { 
                return _mainId; 
            }
        }

        public string SaveDirectory
        {
            get 
            { 
                return  Path.Combine(HttpContext.Current.Server.MapPath("~"), ConfigContext.Current.DefaultConfig["upload:tempdir"], MainId);
            }
        }

        private IList<string> _errorInfo;
        public IList<string> ErrorInfo
        {
            get
            {
                if (_errorInfo == null)
                {
                    _errorInfo = new List<string>();
                }
                return _errorInfo;
            }
        }

        private ConvertResult _convertResult;

        public ConvertResult ConvertResult
        {
            get
            {
                if (_convertResult == null)
                {
                    _convertResult = new ConvertResult
                    {
                        fileList = new List<ConvertFileList>()
                    };
                }
                return _convertResult;
            }
            set
            {
                _convertResult = value;
            }
        }

        public virtual IList<Core.ConvertFileList> ConvertFileList
        {
            get;
            set;
        }

     

        public string RarFileRelativePath
        {
            get;
            set;
        }

        public virtual string RarFileName
        {
            get
            {
                return MainId + ".rar";
            }
        }


        public virtual void BeforeConvert()
        {
            return;
        }

        protected virtual void JustDoIt()
        {

        }
        public virtual void Convert()
        {
            BeforeConvert();

            JustDoIt();

            AfterConvert();
        }

        public virtual void AfterConvert()
        {
            if (ConvertResult.fileList != null && ConvertResult.fileList.Any())
            {
                var rarFullPath = "";
                RarOrZipUtil.Compress(RarFileDirectory, RarFileName, out rarFullPath);
                RarFileRelativePath = rarFullPath;
            }
        }


        public bool IsSuccess
        {
            get {return !ErrorInfo.Any(); }
        }


        public virtual string RarFileDirectory
        {
            get
            {
                return Path.GetDirectoryName(ConvertResult.fileList[0].PhysicsFilePath);
            }
        }
    }
}
