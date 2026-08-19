using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.VisualBasic.ApplicationServices;

using System.Diagnostics;

namespace FMBackupUtilityConfiguration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm mainForm = new MainForm();
            SingleInstanceApplication.Run(mainForm, mainForm.StartupNextInstanceHandler);

//            Application.Run(new MainForm());
        }

        public class SingleInstanceApplication : WindowsFormsApplicationBase
        {
            private SingleInstanceApplication()
            {
                base.IsSingleInstance = true;
            }

            public static void Run(Form mainForm, StartupNextInstanceEventHandler startupHandler)
            {
                SingleInstanceApplication myApp = new SingleInstanceApplication();
                myApp.MainForm = mainForm;
                myApp.StartupNextInstance += startupHandler;

                if (!EventLog.SourceExists("Backup Utility Configuration")) 
                {
                    EventLog.CreateEventSource("Backup Utility Configuration", "Application");
                }

                try
                {
                    myApp.Run(Environment.GetCommandLineArgs());
                }
                catch (CantStartSingleInstanceException ex1)
                {
                    System.Diagnostics.Trace.WriteLine(ex1.Message);
//                    EventLog.WriteEntry(ex1.Message);

                    // Write an event to the event log.
                    EventLog.WriteEntry(
                        "Backup Utility Configuration",      // Registered event source
                        ex1.Message,        // Event entry message
                        EventLogEntryType.Information, // Event type
                        1,                             // Application specific ID
                        0,                             // Application specific category
                        new byte[] {10, 55, 200}       // Event data
                    );

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
//                    EventLog.WriteEntry(ex.Message);

                    // Write an event to the event log.
                    EventLog.WriteEntry(
                        "Backup Utility Configuration",      // Registered event source
                        ex.Message,        // Event entry message
                        EventLogEntryType.Information, // Event type
                        1,                             // Application specific ID
                        0,                             // Application specific category
                        new byte[] {10, 55, 200}       // Event data
                    );
                }
            }

        }
    }
}