using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;

namespace FuelsManager.Afss.ServiceProcess
{
    using global::AfssRICE.ServiceProcess;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                Application.Run(new ServiceUI(new AfssServiceProcess()));
            }
            else
            {
                ServiceBase.Run(new AfssServiceProcess());
            }
        }
    }
}
