using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InfoEarthFrame.WebApi.Next.Models
{
  
    public class ApiAuthorize
    {
        /// <summary>
        /// 请求页面
        /// </summary>
        [XmlText]
        public string Page { get; set; }

        /// <summary>
        /// 是否需要令牌
        /// </summary>
        [XmlAttribute]
        public bool NeedToken { get; set; }
    }
}
