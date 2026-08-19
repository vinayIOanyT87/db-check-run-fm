
using System.Configuration;
using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace FMTransactionExportService
{
    static class FMTransactionExportServiceMainEntry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            bool debug = false;

            foreach (string arg in args)
            {
                if (arg.ToLower() == "/debug"
                || arg.ToLower() == "-debug")
                {
                    debug = true;
                }
            }

            if (Environment.UserInteractive || debug)
            {
                bool displayWindows = true;
                try
                {
                    displayWindows = bool.Parse(ConfigurationManager.AppSettings["DisplayWindows"]);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }

                if (displayWindows)
                {
                    FMTransactionExportServiceInProcForm form = new FMTransactionExportServiceInProcForm();
                    Application.Run(form);
                }
                else
                {
                    var exportService = new FMTransactionExportService();
                    exportService.Start();
                    while (true)
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                }
            }
            else
            {

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new FMTransactionExportService() };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}