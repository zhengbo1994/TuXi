using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public class TokenModal
    {
        public string Token { get; set; }
    }
    public class ProvincesQuery : TokenModal
    {
        public string DistrictName { get; set; }
    }

    public class CityQuery : TokenModal
    {
        public string ProviceCode { get; set; }
    }

    public class CountyQuery : TokenModal
    {
        public string CityCode { get; set; }
        public string[] CityCodes { get; set; }
    }

    public class TownsQuery : TokenModal
    {
        public string CountyCode { get; set; }
        public string[] CountyCodes { get; set; }
    }
}
