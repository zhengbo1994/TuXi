using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class DefaultWorkSpaceResponse
    {
        [DataMember(Name = "workspace")]
        public WorkSpace WorkSpace { get; set; }
    }
}
