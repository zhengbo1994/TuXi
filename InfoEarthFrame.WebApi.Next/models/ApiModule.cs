
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InfoEarthFrame.WebApi.Next.Models
{

    public class ApiModule<T> where T:class
    {
        public ApiModule()
        {
            this.Items = new List<T>();
        }
        /// <summary>
        /// 模块名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement(ElementName ="Item")]
        public List<T> Items { get; set; }
    }
}
