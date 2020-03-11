using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Core
{
    public interface IDisasterRepository : IRepository<DisasterEntity>
    {
        Task<List<DisasterEntity>> GetPageList(int  pageIndex,int pageSize);
    }
}
