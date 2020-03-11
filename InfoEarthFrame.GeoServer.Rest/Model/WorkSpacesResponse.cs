using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class WorkSpacesResponse
    {
        [DataMember(Name = "workspaces")]
        public WorkSpaceSet WorkSpacesSet { get; set; }

        [IgnoreDataMember]
        public IEnumerable<WorkSpace> WorkSpaces
        {
            get { return (WorkSpacesSet != null && WorkSpacesSet.WorkSpaces != null) ? WorkSpacesSet.WorkSpaces.ToList() : new List<WorkSpace>(); }
        }

        [DataContract]
        public class WorkSpaceSet
        {
            [DataMember(Name = "workspace")]
            public WorkSpace[] WorkSpaces { get; set; }
        }
    }
}
