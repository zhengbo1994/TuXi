using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common
{
    public class HttpResponseResult
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        private int? _code;


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

        [JsonProperty("data")]
        public object Data { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
