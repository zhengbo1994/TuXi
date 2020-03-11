using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class DataStoresResponse
    {
        [DataMember(Name = "dataStores")]
        public DataStoreSet DataStoresSet { get; set; }

        [IgnoreDataMember]
        public IEnumerable<DataStore> DataStores
        {
            get { return (DataStoresSet != null && DataStoresSet.DataStores != null) ? DataStoresSet.DataStores.ToList() : new List<DataStore>(); }
        }

        [DataContract]
        public class DataStoreSet
        {
            [DataMember(Name = "dataStore")]
            public DataStore[] DataStores { get; set; }
        }
    }
}
