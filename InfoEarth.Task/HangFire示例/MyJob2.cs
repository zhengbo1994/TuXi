using System;
using Hangfire.Common;
using Hangfire.Console;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;

namespace InfoEarth.Task
{
	public class MyJob2 : IRecurringJob
	{
		class SimpleObject
		{
			public string Name { get; set; }
			public int Age { get; set; }
		}

		public void Execute(PerformContext context)
		{
			context.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+" MyJob2 Running ...");
			var intVal = context.GetJobData<int>("IntVal");
			var stringVal = context.GetJobData<string>("StringVal");
			var booleanVal = context.GetJobData<bool>("BooleanVal");
			var simpleObject = context.GetJobData<SimpleObject>("SimpleObject");
			context.SetJobData("IntVal", ++intVal);
			context.WriteLine("IntVal changed to "+intVal);
			context.SetJobData("NewIntVal", 99);
			var newIntVal = context.GetJobData<int>("NewIntVal");
		}
	}
}
