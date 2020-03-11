using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTelluro.GeologicMap.TopologyCheck
{
    public class AttValueInfo
    {
        public string Guid = string.Empty;
        public string AttId = string.Empty;
        public string Value = string.Empty;
        public string Memo = string.Empty;
        public string ParentId = string.Empty;

        public AttValueInfo()
        {
        }

        public AttValueInfo(string guid, string attId, string value, string memo,string parentId)
        {
            this.Guid = guid;
            this.AttId = attId;
            this.Value = value;
            this.Memo = memo;
            this.ParentId = parentId;
        }
    }
}
