using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class DataStoreRequest
    {
        public DataStoreRequest(string workSpace, string storeName, string host, string port, string dataBase, string userName, string userPassword, string dbType)
        {
            DataStore = new TargetDataStore() { WorkSpace = workSpace, Name = storeName, Host = host, Port=port, DataBase=dataBase,Schema= "public", User=userName,Passwd=userPassword, DBType = dbType, IsEnabled = true };
        }

        [DataMember(Name = "dataStore")]
        private TargetDataStore DataStore { get; set; }

        [DataContract]
        public class TargetDataStore
        {
            [DataMember(Name = "workSpace")]
            public string WorkSpace { get; set; }

            [DataMember(Name = "storeName")]
            public string Name { get; set; }

            [DataMember(Name = "dbType")]
            public string DBType { get; set; }

            [DataMember(Name = "host")]
            public string Host { get; set; }
            [DataMember(Name = "port")]
            public string Port { get; set; }
            [DataMember(Name = "dataBase")]
            public string DataBase { get; set; }
            [DataMember(Name = "schema")]
            public string Schema { get; set; }
            [DataMember(Name = "userName")]
            public string User { get; set; }
            [DataMember(Name = "userPassword")]
            public string Passwd { get; set; }

            [DataMember(Name = "enabled")]
            public bool IsEnabled { get; set; }
        }
    }
}
