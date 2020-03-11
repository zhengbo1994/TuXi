using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest.Model
{
    [DataContract]
    public class StylesResponse
    {
        [DataMember(Name = "styles")]
        public StyleSet StylesSet { get; set; }

        [IgnoreDataMember]
        public IEnumerable<Style> Styles
        {
            get { return (StylesSet != null && StylesSet.Styles != null) ? StylesSet.Styles.ToList() : new List<Style>(); }
        }
    }

    [DataContract]
    public class StyleSet
    {
        [DataMember(Name = "style")]
        public Style[] Styles { get; set; }
    }
}
