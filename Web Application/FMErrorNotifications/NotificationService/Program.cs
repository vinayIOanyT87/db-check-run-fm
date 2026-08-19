using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMNotificationService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "/debug")
            {
                var form = new NotificationServiceForm();
                Application.Run(form);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new DirectoryWatcher()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
