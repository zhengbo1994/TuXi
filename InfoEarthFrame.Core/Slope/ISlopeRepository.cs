using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Core
{
    public interface ISlopeRepository : IRepository<SlopeEntity, String>
    {
        Task<List<SlopeEntity>> GetPageList(int pageIndex, int pageSize, string disasterunitname, string location);
        Task<int> GetPageCount(int pageIndex, int pageSize, string disasterunitname, string location);
        Expression<Func<SlopeEntity, bool>> GetWhere(string disasterunitname, string location);

    }
}
