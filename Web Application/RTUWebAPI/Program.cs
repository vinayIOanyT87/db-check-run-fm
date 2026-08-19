using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Threading;

namespace RTUWebAPI
{
	internal class CustomWebHostService : WebHostService
	{

		public CustomWebHostService(IWebHost host) : base(host)
		{
			base.ServiceName = "VarecVeRTUeService";
			base.EventLog.Source = base.ServiceName;
			base.EventLog.Log = "Application";
		}

		protected override void OnStarting(string[] args)
		{
			// Log
			base.OnStarting(args);
		}

		protected override void OnStarted()
		{
			base.OnStarted();
		}

		protected override void OnStopping()
		{
			// Even more log
			base.OnStopping();
		}

		protected override void OnStopped()
		{
			base.OnStopped();
		}
	}

	public static class CustomWebHostWindowsServiceExtensions
	{
		public static void RunAsCustomService(this IWebHost host)
		{
			var webHostService = new CustomWebHostService(host);
			ServiceBase.Run(webHostService);
		}
	}

	public class Program
    {
        public static void Main(string[] args)
        {
				var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
				var directoryPath = Path.GetDirectoryName(exePath);

				var config = new ConfigurationBuilder()
					.SetBasePath(directoryPath)
					.AddJsonFile("hosting.json", optional: true)
					.Build();


				var host = CreateWebHostBuilder(args, config, directoryPath).Build();

				if (!args.Contains("-service"))
				{
					host.Run();
				}
				else
				{
					host.RunAsCustomService();
				}
			}

			public static IWebHostBuilder CreateWebHostBuilder(string[] args, IConfigurationRoot config, string directoryPath) =>
					WebHost.CreateDefaultBuilder(args)
						 .UseConfiguration(config)
						 .UseContentRoot(directoryPath)
 						 .UseStartup<Startup>();
    }
}
