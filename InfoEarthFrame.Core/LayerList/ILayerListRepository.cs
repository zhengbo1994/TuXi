using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Core
{
    public interface ILayerListRepository : IRepository
    {
        LayerPageData GetPageList(string userId,int PageIndex, int PageSize, Layer l);
    }
}
