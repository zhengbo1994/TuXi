using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class Layer
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "nativeName")]
        public string NativeName { get; set; }
        [DataMember(Name = "namespace")]
        public WorkSpace Workspace { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "srs")]
        public string Srs { get; set; }
        [DataMember(Name = "latLonBoundingBox")]
        public BoundSet BoundSet { get; set; }
        [DataMember(Name = "store")]
        public DataStore DataStore { get; set; }
        [DataMember(Name = "attributes")]
        public AttributesSet AttributesSet { get; set; }
    }

    [DataContract]
    public class AttributesSet
    {
        [DataMember(Name = "attribute")]
        public AttributeSet[] AttributeSet { get; set; }
    }

    [DataContract]
    public class AttributeSet
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "minOccurs")]
        public string MinOccurs { get; set; }
        [DataMember(Name = "maxOccurs")]
        public string MaxOccurs { get; set; }
        [DataMember(Name = "nillable")]
        public string Nillable { get; set; }
        [DataMember(Name = "binding")]
        public string Binding { get; set; }
    }
}
