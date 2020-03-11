using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class LayerResponse
    {
        [DataMember(Name = "featureType")]
        public Layer Layer { get; set; }

    }
}
