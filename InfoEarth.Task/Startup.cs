using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Net.Http.Formatting;
using Hangfire;
using StackExchange.Redis;
using Hangfire.Console;
using Hangfire.RecurringJobExtensions;
using Hangfire.MySql;
using System.Transactions;

[assembly: OwinStartup(typeof(InfoEarth.Task.Startup))]
namespace InfoEarth.Task
{
    public class Startup
    {
        public static ConnectionMultiplexer Redis = null;
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
            );
            //将默认xml返回数据格式改为json
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("datatype", "json", "application/json"));
            app.UseWebApi(config);

            //HangFire配置
            bool isTaskRun = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IsTaskRun"]);
            if (!isTaskRun)
            {
                return;
            }

            string hangFireType = System.Configuration.ConfigurationManager.AppSettings["HangFireStoreType"];
            if (hangFireType == "SqlServer")
            {

                //SqlServer数据库
                GlobalConfiguration.Configuration
                    .UseSqlServerStorage("HangFireCon").UseConsole();
                //.UseRecurringJob(typeof(RecurringJobService)).UseDefaultActivator();


                //GlobalConfiguration.Configuration.UseRecurringJob("recurringjob.json");
            }
            else if (hangFireType == "MySql")
            {
                //MySql数据库
                MySqlStorage mySqlS = new MySqlStorage("HangFireCon", new MySqlStorageOptions
                {
                    TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(1),
                });
                GlobalConfiguration.Configuration
                   .UseStorage(mySqlS).UseConsole();//.UseRecurringJob(typeof(RecurringJobService)).UseDefaultActivator();
                //配置文件
                //x.UseRecurringJob("recurringjob.json");
            }
            else if (hangFireType == "Redis")
            {
                Redis = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.ConnectionStrings["HangFireCon"].ConnectionString);
                GlobalConfiguration.Configuration
                   .UseRedisStorage(Redis).UseConsole().
                   UseRecurringJob(typeof(RecurringJobService)).UseDefaultActivator();
            }
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() },
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                Queues = new[] { "default", "apis", "jobs" }
            });

            //测试
            //BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget"));

            //BackgroundJob.Schedule(() => Console.WriteLine("Delayed"), TimeSpan.FromDays(1));

            //RecurringJob.AddOrUpdate(() => Console.WriteLine("Daily Job"), Cron.Daily);
            //string cron = string.Empty;
            //RecurringJob.AddOrUpdate<IThumbnailInitialize>(string.Format("图层与地图初始化下载"), x => x.ExcuteJob(null), Cron.Daily, TimeZoneInfo.Local);


            BackgroundJob.Enqueue<ThumbnailInitialize>(x => x.ExcuteJob(null));
        }
    }
}
