using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class LayerGroupsResponse
    {
        [DataMember(Name = "layerGroups")]
        public LayerGroupSet LayerGroupsSet { get; set; }

        [IgnoreDataMember]
        public IEnumerable<LayerGroup> LayerGroups
        {
            get { return (LayerGroupsSet != null && LayerGroupsSet.LayerGroups != null) ? LayerGroupsSet.LayerGroups.ToList() : new List<LayerGroup>(); }
        }
    }

    [DataContract]
    public class LayerGroupSet
    {
        [DataMember(Name = "layerGroup")]
        public LayerGroup[] LayerGroups { get; set; }
    }
}
