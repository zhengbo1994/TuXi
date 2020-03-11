using Abp.Domain.Entities;
using Abp.EntityFramework;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class GeologyMappingTypeRepository : InfoEarthFrameRepositoryBase<GeologyMappingType, string>, IGeologyMappingTypeRepository
    {
        public GeologyMappingTypeRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {


        }
    }
}
