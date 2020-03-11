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
    public class CoordinateConvertContext : BaseConvertContext
    {
        private readonly string _coordPoint;
        public CoordinateConvertContext(IDataConvertAppService dataConvertAppService, string mainId, string coordPoint)
            : base(dataConvertAppService, mainId)
        {
            this._coordPoint = coordPoint??"";
        }

        public override IList<ConvertFileList> ConvertFileList
        {
            get
            {
                return Directory.GetFiles(SaveDirectory, "*.*", SearchOption.AllDirectories).Select(p => new ConvertFileList
                 {
                     PhysicsFilePath = p,
                     FileType = 2,
                     ConvertResult = 1,
                     ConvertFilePath=p,
                     CoordPoint=_coordPoint.Split(',')
                 }).ToList();
            }
        }

        protected override void JustDoIt()
        {
            try
            {
                ConvertResult = DataConvertAppService.CoordinateConvert(ConvertFileList, ErrorInfo, ConvertService);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
