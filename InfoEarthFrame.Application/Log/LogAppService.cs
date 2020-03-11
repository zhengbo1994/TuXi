using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Extensions;
using iTelluro.SSO.WebServices.DistrictZone;
using iTelluro.SYS.Entity;

namespace InfoEarthFrame.Area
{
    public class LogAppService : ApplicationService, ILogAppService
    {
        public readonly Core.ILogRepository _LogRepository = null;

        public LogAppService(Core.ILogRepository logRepository)
        {
            _LogRepository = logRepository;
        }

        public void InsertLog(Core.Log entity)
        {
            _LogRepository.Insert(entity);
        }

        public List<Core.Log> GetUploadLog()
        {
            var list = _LogRepository.GetAll().OrderByDescending(a => a.CreateTime);
            //s.LogType == 1 && 
            return list.ToList();
        }

        public int  GetUploadLogNum()
        {
            return GetUploadLog().Count;
        }
        
        public void UpdateStatus(List<string> Ids)
        {
            foreach(string s in Ids)
            {
                Core.Log entity = _LogRepository.Get(s);
                entity.Other1 = "1";
                _LogRepository.Update(entity);
            }
        }
    }
}
