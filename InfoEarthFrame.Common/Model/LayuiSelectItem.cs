using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common.Model
{
 
   public class LayuiSelectItem
    {
        public string name { get; set; }

        public object value { get; set; }

        public object tag { get; set; }
        public object tag1 { get; set; }

        public object tag2 { get; set; }

        public object tag3 { get; set; }

        public object tag4 { get; set; }

        public List<LayuiSelectItem> children { get; set; }
    }
}
