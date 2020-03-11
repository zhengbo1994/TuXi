using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class LayersResponse
    {
        [DataMember(Name = "featureTypes")]
        public LayerSet LayersSet { get; set; }

        [IgnoreDataMember]
        public IEnumerable<Layer> Layers
        {
            get { return (LayersSet != null && LayersSet.Layers != null) ? LayersSet.Layers.ToList() : new List<Layer>(); }
        }

        [DataContract]
        public class LayerSet
        {
            [DataMember(Name = "featureType")]
            public Layer[] Layers { get; set; }
        }
    }
}
