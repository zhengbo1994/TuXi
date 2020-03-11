using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTelluro.GeologicMap.TopologyCheck
{
    public class TxLayerInfo
    {
        public string Guid = string.Empty;
        public string TxName = string.Empty;
        public string TjName = string.Empty;
        public string LayerName = string.Empty;
        public string GeoType = string.Empty;
        public string DataLayer = string.Empty;

        public TxLayerInfo()
        {
        }

        public TxLayerInfo(string guid, string txName, string tjName, string lyrName, string geoType, string dataLayer)
        {
            this.Guid = guid;
            this.TxName = txName;
            this.TjName = tjName;
            this.LayerName = lyrName;
            this.GeoType = geoType;
            this.DataLayer = dataLayer;
        }
    }
}
