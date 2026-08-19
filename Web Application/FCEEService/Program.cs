namespace FCEEService
{
	using System.Net;
	using System;
	using System.Configuration;
	using System.Collections.Generic;
	using System.Linq;
	using System.ServiceProcess;
	using System.Text;
	using System.Threading.Tasks;

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			var httpListener = new HttpListener();

			var url = ConfigurationManager.AppSettings["Url"] == null ? "http://*:8080/api/va/" :  ConfigurationManager.AppSettings["Url"];

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new FCEEServer(httpListener, url)
			};

			ServiceBase.Run(ServicesToRun);
		}
	}
}
