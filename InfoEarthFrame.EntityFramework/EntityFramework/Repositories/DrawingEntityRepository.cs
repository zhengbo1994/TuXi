using Abp.EntityFramework;
using InfoEarthFrame.Core;
using InfoEarthFrame.DrawingOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class DrawingEntityRepository : InfoEarthFrameRepositoryBase<DrawingEntity, string>, IDrawingEntityRepository
    {
        public DrawingEntityRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
			:base(dbContextProvider)
		{
		}
    }
}
