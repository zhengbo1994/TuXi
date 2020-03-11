using Abp.EntityFramework;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class DataMainRepository : InfoEarthFrameRepositoryBase<DataMain, String>, InfoEarthFrame.Core.IDataMainRepository
    {
        public DataMainRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {


        }
    }
}
