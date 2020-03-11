using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoEarthFrame.Core;
using Abp.Domain.Repositories;

namespace InfoEarthFrame.Core
{
    public interface IElementTypeRepository : IRepository<ElementType, string>
    {
    }
}
