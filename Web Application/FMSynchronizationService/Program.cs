// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMSynchronizationService
{
    using System;
    using System.ServiceProcess;
    using System.Windows.Forms;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] 
            { 
                new FMSynchronizationService() 
            };

            if (Environment.UserInteractive)
            {
                Application.Run(new FMSynchronizationServiceUI((FMSynchronizationService)servicesToRun[0]));
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
