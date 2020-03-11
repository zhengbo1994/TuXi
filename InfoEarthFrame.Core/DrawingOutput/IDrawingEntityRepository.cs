using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoEarthFrame.Core;
using InfoEarthFrame.DrawingOutput;

namespace InfoEarthFrame.Core
{
    public interface IDrawingEntityRepository : IRepository<DrawingEntity, string>
    {
    }
}
