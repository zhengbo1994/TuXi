using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Core
{
    public interface ILayerManagerRepository : IRepository<Tbl_LayerManager, string>
    {
        Task<List<Tbl_LayerManager>> GetPageList(string name,int PageIndex, int PageSize);
    }
}
