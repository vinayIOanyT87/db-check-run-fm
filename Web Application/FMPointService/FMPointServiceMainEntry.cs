

namespace FMPointService
{
	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.ServiceProcess;
	using System.Windows.Forms;

	public static class FMPointServiceMainEntry
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		public static void Main(string[] args)
		{
			bool debug = false;
			string host = null;
			string port = null;
			string affinity = null;

			foreach (string arg in args)
			{
				if (arg.ToLower() == "/debug"
				|| arg.ToLower() == "-debug")
				{
					debug = true;
				}

				if (arg.ToLower().StartsWith("/h")
				|| arg.ToLower().StartsWith("-h"))
				{
					if (arg.Length > 2)
					{
						host = arg.Substring(2);
					}
				}

				if (arg.ToLower().StartsWith("/p")
				|| arg.ToLower().StartsWith("-p"))
				{
					if (arg.Length > 2)
					{
						port = arg.Substring(2);
					}
				}

				if(arg.ToLower().StartsWith("/a")
				|| arg.ToLower().StartsWith("-a"))
				{
					affinity = arg.Substring(2);
				}
			}

			if(affinity != null)
			{
				affinity = affinity.ToLower().Replace("0x", "");
				int result;
				if (int.TryParse(affinity,
									System.Globalization.NumberStyles.AllowHexSpecifier,
									null,
									out result))
				{
					var intPtrAffinity = new System.IntPtr(result);
					System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = intPtrAffinity;
				}
			}

				//Process thisProc = Process.GetCurrentProcess();
				//thisProc.PriorityClass = ProcessPriorityClass.RealTime;

				if (Environment.UserInteractive || debug)
			{
					bool displayWindows = true;
					try
					{
						displayWindows = bool.Parse(ConfigurationManager.AppSettings["DisplayWindows"]);
					}
					catch (Exception eadEx)
					{
						System.Console.WriteLine(eadEx.Message);
					}

					if (displayWindows)
					{
						FMPointServiceInProcForm form = new FMPointServiceInProcForm(host, port);
						Application.Run(form);
					}
					else
					{
						var pointService = new FMPointService(host, port);
						pointService.Start();
						while (true)
						{
								System.Threading.Thread.Sleep(5000);
						}
						//pointService.Stop();
					}
			}
			else
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					new FMPointService(host, port) 
				};
				ServiceBase.Run(ServicesToRun);
			}
		}
	}
}
