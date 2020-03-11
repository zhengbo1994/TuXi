using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InfoEarthFrame.WebApi.Next.Models
{
    public class ApiResult
    {
        private string _message;
        /// <summary>
        /// 返回信息
        /// </summary>
        [XmlText]
        [JsonProperty("message")]
        public string Message
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                {
                    _message = string.Empty;
                }
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        /// <summary>
        /// 返回值
        /// </summary>
        private int? _code;
       [XmlAttribute]
       [JsonProperty("code")]
        public int Code
        {
            get
            {
                if (!_code.HasValue)
                {
                    _code = -1;
                }
                return _code.Value;
            }
            set
            {
                _code = value;
            }
        }

        /// <summary>
        /// 额外信息
        /// </summary>
        [XmlElement]
        [JsonProperty("data")]
        public object Data { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
