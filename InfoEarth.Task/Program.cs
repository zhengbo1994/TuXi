using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.ServiceConfigurators;
//using InfoEarth.Log;

namespace InfoEarth.Task
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                string instanceName = System.Configuration.ConfigurationManager.AppSettings["WindowsServiceName"];

                x.Service<HangFireService>(s =>
                {
                    ServiceConfigurator<HangFireService> s1 = s as ServiceConfigurator<HangFireService>;
                    s1.ConstructUsing(name => new HangFireService());
                    s1.WhenStarted(tc => tc.Start());
                    s1.WhenStopped(tc => tc.Stop());
                });

                //x.RunAsLocalService();
                x.RunAsLocalSystem();
                x.SetServiceName(instanceName);
                x.SetDisplayName(instanceName);
                x.SetDescription(instanceName);
            });
        }
    }
}

