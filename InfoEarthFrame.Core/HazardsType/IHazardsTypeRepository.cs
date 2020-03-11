using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoEarthFrame.Core;

namespace InfoEarthFrame.Core
{
    public interface IHazardsTypeRepository : IRepository<HazardsTypeEntity,string>
    {
    }
}
