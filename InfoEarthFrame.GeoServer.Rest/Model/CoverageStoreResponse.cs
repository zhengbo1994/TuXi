using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class CoverageStoreResponse
    {
        [DataMember(Name = "coverageStores")]
        public CoverageStoreSet CoverageStoresSet { get; set; }

        [IgnoreDataMember]
        public IEnumerable<CoverageStore> CoverageStores
        {
            get { return (CoverageStoresSet != null && CoverageStoresSet.CoverageStores != null) ? CoverageStoresSet.CoverageStores.ToList() : new List<CoverageStore>(); }
        }

        [DataContract]
        public class CoverageStoreSet
        {
            [DataMember(Name = "coverageStore")]
            public CoverageStore[] CoverageStores { get; set; }
        }
    }
}
