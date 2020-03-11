using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common
{
   public abstract  class PageInfo
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }

    }
}
