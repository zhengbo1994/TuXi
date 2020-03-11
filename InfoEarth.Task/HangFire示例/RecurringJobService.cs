using Hangfire;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Console;

namespace InfoEarth.Task
{
    public class RecurringJobService
    {
        //[RecurringJob("*/10 * * * *")]
        //[Queue("jobs")]
        //public void SimpleJob(PerformContext context)
        //{
        //    context.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "SimpleJob Running ...");

        //    var progressBar = context.WriteProgressBar();

        //    foreach (var i in Enumerable.Range(1, 50).ToList().WithProgress(progressBar))
        //    {
        //        context.SetTextColor(ConsoleTextColor.DarkRed);
        //        context.WriteLine(i);
        //    }
        //}

        //[RecurringJob("*/1 * * * *")]
        //[Queue("jobs")]
        //public void TestJob1(PerformContext context)
        //{
        //    context.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "TestJob1 Running ...");
        //}
        //[RecurringJob("*/2 * * * *", RecurringJobId = "TestJob2")]
        //[Queue("jobs")]
        //public void TestJob2(PerformContext context)
        //{
        //    context.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "TestJob2 Running ...");
        //}
        //[RecurringJob("*/2 * * * *", "China Standard Time", "jobs")]
        //public void TestJob3(PerformContext context)
        //{
        //    context.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "TestJob3 Running ...");
        //}
        //[RecurringJob("*/5 * * * *", "jobs")]
        //public void InstanceTestJob(PerformContext context)
        //{
        //    context.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "InstanceTestJob Running ...");
        //}

        //[RecurringJob("*/6 * * * *", "UTC", "jobs")]
        //public static void StaticTestJob(PerformContext context)
        //{
        //    context.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "StaticTestJob Running ...");
        //}
    }
}