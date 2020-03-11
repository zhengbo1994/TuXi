using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class CoverageStore
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "href")]
        public string Href { get; set; }
    }

    public class ConverageRequest
    {
        public Coverage Coverage;
    }

    public class Coverage
    {
        public string Name { get; set; }

        public string Srs { get; set; }
    }
}
