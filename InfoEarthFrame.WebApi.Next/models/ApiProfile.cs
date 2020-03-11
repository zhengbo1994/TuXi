using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InfoEarthFrame.WebApi.Next.Models
{
    [XmlRoot(ElementName = "ApiProfile")]
    public class ApiProfile<T> where T:class
    {
        public ApiProfile()
        {
            this.Modules = new List<ApiModule<T>>();
        }

        [XmlElement(ElementName = "Module")]
        public List<ApiModule<T>> Modules { get; set; }
    }
}
