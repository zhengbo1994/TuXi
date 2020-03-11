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
    public class MultimediaTypeRepository : InfoEarthFrameRepositoryBase<MultimediaTypeEntity, string>, IMultimediaTypeRepository
    {
        public MultimediaTypeRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
           
             
        }
    }
}
