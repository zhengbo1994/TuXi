using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class LayerRequest
    {
        public LayerRequest(string name, string desiredSrs)
        {
            Layer = new TargetLayer() { Name = name, Srs = desiredSrs, IsEnabled = true, Advertised = true };
            Layer.Parameters = new LayerParameterSet();
            Layer.Parameters.Entries = new ParameterEntry[]{new ParameterEntry(){KeyValue = new string[]{ "USE_JAI_IMAGEREAD", "false"}},
                                                               new ParameterEntry(){KeyValue = new string[]{"USE_MULTITHREADING", "true"}},
                                                               new ParameterEntry(){KeyValue = new string[]{"SUGGESTED_TILE_SIZE", "256,256"}}};
            SrsSet set = new SrsSet() { Srs = new string[] { desiredSrs } };
            Layer.RequestSrs = set;
            Layer.ResponseSrs = set;
        }

        [DataMember(Name = "coverage")]
        private TargetLayer Layer { get; set; }
    }

    [DataContract]
    public class TargetLayer
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "requestSRS")]
        public SrsSet RequestSrs { get; set; }

        [DataMember(Name = "responseSRS")]
        public SrsSet ResponseSrs { get; set; }

        [DataMember(Name = "srs")]
        public string Srs { get; set; }

        [DataMember(Name = "enabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "parameters")]
        public LayerParameterSet Parameters { get; set; }

        [DataMember(Name = "advertised")]
        public bool Advertised { get; set; }
    }

    [DataContract]
    public class LayerParameterSet
    {
        [DataMember(Name = "entry")]
        public ParameterEntry[] Entries { get; set; }
    }

    [DataContract]
    public class ParameterEntry
    {
        [DataMember(Name = "string")]
        public string[] KeyValue { get; set; }
    }

    [DataContract]
    public class SrsSet
    {
        [DataMember(Name = "string")]
        public string[] Srs { get; set; }
    }
}
