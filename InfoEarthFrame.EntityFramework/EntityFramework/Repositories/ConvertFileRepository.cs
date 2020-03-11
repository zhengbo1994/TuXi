using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoEarthFrame.Core;
using Abp.EntityFramework;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class ConvertFileRepository : InfoEarthFrameRepositoryBase<ConvertFile, String>, IConvertFileRepository
    {
        public ConvertFileRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
