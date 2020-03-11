using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class LayerGroupStoresResponse
    {
        [DataMember(Name = "layerGroup")]
        public LayerGroupStoreSet LayerGroupStoresSet { get; set; }
    }

    [DataContract]
    public class LayerGroupStoreSet
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "workspace")]
        public WorkSpace Workspace { get; set; }

        [DataMember(Name = "publishables")]
        public PublishedSet LayersSet { get; set; }
    }

    [DataContract]
    public class PublishedSet
    {
        [DataMember(Name = "published")]
        public object Layers { get; set; }
    }
}
