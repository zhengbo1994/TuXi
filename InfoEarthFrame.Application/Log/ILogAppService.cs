using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using iTelluro.SYS.Entity;

namespace InfoEarthFrame.Area
{
    public interface ILogAppService : IApplicationService
    {
        void InsertLog(Core.Log entity);
        List<Core.Log> GetUploadLog();
        int GetUploadLogNum();
        void UpdateStatus(List<string> Ids);
    }
}
