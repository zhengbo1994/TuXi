using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoEarthFrame.Core;
using Abp.EntityFramework;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class LogRepository : InfoEarthFrameRepositoryBase<Log, String>, ILogRepository
    {
        public LogRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
