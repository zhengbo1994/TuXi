using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTelluro.GeologicMap.TopologyCheck
{
    public class LayerArgs
    {
        public string LayerPath = string.Empty;
        public string TxName = string.Empty;
        public string TjName = string.Empty;
        public string LayerName = string.Empty;
        public string DataLayer = string.Empty;

        public LayerArgs()
        {
        }

        public LayerArgs(string lyrPath, string txName, string tjName, string lyrName, string dataLayer)
        {
            this.LayerPath = lyrPath;
            this.TxName = txName;
            this.TjName = tjName;
            this.LayerName = lyrName;
            this.DataLayer = dataLayer;
        }
    }
}
