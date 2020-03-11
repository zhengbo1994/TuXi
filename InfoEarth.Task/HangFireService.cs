//using InfoEarth.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarth.Task
{
    public class HangFireService
    {
       // private Log.Log Logger = null;
        public HangFireService()
        {
            //Logger = LogFactory.GetLogger(this.GetType().Name);
        }

        public void Start()
        {
            string baseAddress = System.Configuration.ConfigurationManager.AppSettings["HostUrl"];
            Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress);
        }

        public void Stop()
        {
         
        }
    }
}
