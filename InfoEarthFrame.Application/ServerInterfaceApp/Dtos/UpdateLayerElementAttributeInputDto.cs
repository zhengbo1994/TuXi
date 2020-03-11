using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ServerInterfaceApp.Dtos
{
   public class LayerElementAttributeInputDto
    {
       public string LayerId { get; set; }

       public string ElementId { get; set; }

       public string[] Values { get; set; }

       public string Geometry { get; set; }
    }
}
