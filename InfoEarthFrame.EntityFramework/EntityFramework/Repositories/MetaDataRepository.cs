using Abp.EntityFramework;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class MetaDataRepository : InfoEarthFrameRepositoryBase<MetaData, String>, IMetaDataRepository
    {
        public MetaDataRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
