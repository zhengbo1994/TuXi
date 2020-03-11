using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.EntityFramework;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;


namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class DisasterRepository : InfoEarthFrameRepositoryBase<DisasterEntity>, IDisasterRepository
    {
        public DisasterRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
           
             
        }

        public  async Task<List<DisasterEntity>> GetPageList(int pageIndex, int pageSize)
        {
            var query = GetAll().OrderBy(lm => lm.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

           
            return query;
        }
    }
}
