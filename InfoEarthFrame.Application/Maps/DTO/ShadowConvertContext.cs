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
    public class ShadowConvertContext : BaseConvertContext
    {
        private readonly string _outputCoordName;
        public ShadowConvertContext(IDataConvertAppService dataConvertAppService, string mainId, string outputCoordName)
            : base(dataConvertAppService, mainId )
        {
            this._outputCoordName = outputCoordName;
        }

        public override IList<ConvertFileList> ConvertFileList
        {
            get
            {
                return Directory.GetFiles(SaveDirectory, "*.*", SearchOption.AllDirectories).Select(p => new ConvertFileList
                 {
                     PhysicsFilePath = p,
                     FileType = 3,
                     ConvertResult = 1,
                     ConvertFilePath = p
                 }).ToList();
            }
        }

        protected override void JustDoIt()
        {
            try
            {
                ConvertResult = DataConvertAppService.ProjectionConvert(ConvertFileList, ErrorInfo, ConvertService,_outputCoordName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
