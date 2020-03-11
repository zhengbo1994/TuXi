using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class LayerGroup
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "mode")]
        public string Mode { get; set; }
        [DataMember(Name = "workspace")]
        public WorkSpace Workspace { get; set; }
        [DataMember(Name = "publishables")]
        public PublishedSet LayersSet { get; set; }
        [DataMember(Name = "styles")]
        public StyleSet Styles { get; set; }
        [DataMember(Name = "bounds")]
        public BoundSet BoundSet { get; set; }
    }

    [DataContract]
    public class BoundSet
    {
        [DataMember(Name = "minx")]
        public string Minx { get; set; }
        [DataMember(Name = "maxx")]
        public string Maxx { get; set; }
        [DataMember(Name = "miny")]
        public string Miny { get; set; }
        [DataMember(Name = "maxy")]
        public string Maxy { get; set; }
        [DataMember(Name = "crs")]
        public object Crs { get; set; }
    }
}
