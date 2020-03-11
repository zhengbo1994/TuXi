using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class WorkSpaceRequest
    {
        public WorkSpaceRequest(string workSpaceName)
        {
            WorkSpace = new TargetWorkSpace() { Name = workSpaceName };
        }

        [DataMember(Name = "workspace")]
        private TargetWorkSpace WorkSpace { get; set; }

        [DataContract]
        private class TargetWorkSpace
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }
        }
    }
}
