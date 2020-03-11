using Abp.EntityFramework;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class LayerManagerRepository : InfoEarthFrameRepositoryBase<Tbl_LayerManager, string>, ILayerManagerRepository
    {
        public LayerManagerRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Tbl_LayerManager>> GetPageList(string name,int PageIndex, int PageSize)
        {
            var query = GetAll();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.TEXT != null && p.TEXT.ToLower().Contains(name.ToLower()));
            }
            var list=query.OrderBy(q => q.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            return list;
        }
    }
}
