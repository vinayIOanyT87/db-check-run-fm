namespace FMUAAlarmServer
{
	using System;
	using System.ServiceProcess;
	using System.Windows.Forms;

	
	static class FMUAServerMainEntry
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if ((Environment.UserInteractive) || (args.Length > 0 && args[0].ToLower() == "/debug"))
			{
                bool noUI = false;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "/no_ui")
                    {
                        noUI = true;
                        break;
                    }
                }
                if (noUI)
                {
                    var fuelsManagerService = new FMUAAlarmServerService();
                    fuelsManagerService.MyStart(args);
                    while (true)
                    {
                        System.Threading.Thread.Sleep(10000);
                    }
                }
                else
                {
                    FMUAAlarmServerForm form = new FMUAAlarmServerForm(args);
                    Application.Run(form);
                }
			}
			else
			{
                var fuelsManagerService = new FMUAAlarmServerService();
                ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] { fuelsManagerService };
				ServiceBase.Run(ServicesToRun);
			}

		}
	}
}
