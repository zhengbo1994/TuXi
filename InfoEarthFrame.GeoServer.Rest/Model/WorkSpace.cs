using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    /// <summary>
    /// 工作区
    /// </summary>
    [DataContract]
    public class WorkSpace
    {
        private ObservableCollection<DataStore> _DataStores;
        private ObservableCollection<Layer> _Layers;
        private ObservableCollection<LayerGroup> _LayerGroups;
        private GeoServer _GeoServer;
        /// <summary>
        /// 工作区名称
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }
        /// <summary>
        /// 工作区地址
        /// </summary>
        [DataMember(Name = "href")]
        public string Href { get; set; }
        /// <summary>
        /// 工作区数据存储
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<DataStore> DataStores
        {
            get
            {
                if (_DataStores == null && _GeoServer != null)
                {
                    IEnumerable<DataStore> dataStores = _GeoServer.GetDataStores(Name);
                    _DataStores = new ObservableCollection<DataStore>(dataStores);
                }
                return _DataStores ?? new ObservableCollection<DataStore>();
            }
        }
        /// <summary>
        /// 工作区图层
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<Layer> Layers
        {
            get
            {
                if (_Layers == null && _GeoServer != null)
                {
                    IEnumerable<Layer> layers = _GeoServer.GetLayers(Name);
                    _Layers = new ObservableCollection<Layer>(layers);
                }
                return _Layers;
            }
        }
        /// <summary>
        /// 工作区图层组
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<LayerGroup> LayerGroups
        {
            get
            {
                if (_LayerGroups == null && _GeoServer != null)
                {
                    IEnumerable<LayerGroup> layerGroups = _GeoServer.GetLayerGroups(Name);
                    _LayerGroups = new ObservableCollection<LayerGroup>(layerGroups);
                }
                return _LayerGroups;
            }
        }

        public GeoServer GeoServer
        {
            get { return _GeoServer; }
            set { _GeoServer = value; }
        }
    }
}
