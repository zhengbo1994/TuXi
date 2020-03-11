using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class CoverageStoreRequest
    {
        public CoverageStoreRequest(string workSpace, string coverageStoreName,string type)
        {
            CoverageStore = new TargetCoverageStore() { WorkSpace = workSpace, Name = coverageStoreName, Type=type, IsEnabled = true };
        }

        [DataMember(Name = "coverageStore")]
        private TargetCoverageStore CoverageStore { get; set; }

        [DataContract]
        public class TargetCoverageStore
        {

            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "type")]
            public string Type { get; set; }

            [DataMember(Name = "enabled")]
            public bool IsEnabled { get; set; }

            [DataMember(Name = "workspace")]
            public string WorkSpace { get; set; }

            [DataMember(Name = "__default")]
            public string Default { get; set; }

            [DataMember(Name = "url")]
            public string Url { get; set; }



        }
    }
}
