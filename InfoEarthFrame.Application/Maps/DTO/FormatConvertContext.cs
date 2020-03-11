using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Maps.DTO
{
    public class FormatConvertContext : BaseConvertContext
    {
        private readonly string _outputFormat;
        public FormatConvertContext(IDataConvertAppService dataConvertAppService, string mainId,string outputFormat)
            : base(dataConvertAppService, mainId)
        {
            this._outputFormat = outputFormat;
        }

        public override IList<ConvertFileList> ConvertFileList
        {
            get
            {
                return Directory.GetFiles(SaveDirectory, "*.*", SearchOption.AllDirectories).Select(p => new ConvertFileList
                 {
                     PhysicsFilePath = p,
                     ConvertKey = _outputFormat??"",
                      FileType=1
                 }).ToList();
            }
        }

        protected override void JustDoIt()
        {
            try
            {
                if (string.IsNullOrEmpty(_outputFormat))
                {
                    ConvertResult = DataConvertAppService.FormatConvert(ConvertFileList, ErrorInfo, ConvertService);
                }
                else
                {
                    ConvertResult = ConvertService.DataConvert(ConvertFileList.ToList(), "", "NULL", false);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override string RarFileDirectory
        {
            get
            {
                var currentFile=ConvertResult.fileList[0];
                if (currentFile.ConvertFilePath.StartsWith("\\"))
                {
                    return currentFile.ConvertFolder;
                }
                return currentFile.ConvertFilePath.Substring(0, currentFile.ConvertFilePath.LastIndexOf("\\"));
            }
        }
    }
}
