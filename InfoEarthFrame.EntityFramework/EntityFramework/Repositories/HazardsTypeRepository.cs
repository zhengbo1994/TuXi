using Abp.EntityFramework;
using InfoEarthFrame.Core;

namespace InfoEarthFrame.EntityFramework.Repositories
{


    public class HazardsTypeRepository : InfoEarthFrameRepositoryBase<HazardsTypeEntity, string>, IHazardsTypeRepository
    {
        public HazardsTypeRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
           
             
        }
    }
}
