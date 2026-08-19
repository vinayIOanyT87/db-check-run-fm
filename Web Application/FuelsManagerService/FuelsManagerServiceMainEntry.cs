// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerServiceMainEntry.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The main entry point for the FuelsManager Service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
    using System;
    using System.ServiceProcess;
	using System.Windows.Forms;

	/// <summary>
	/// The main entry point for the FuelsManager Service
	/// </summary>
	public static class FuelsManagerServiceMainEntry
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// <param name="args">
		/// Used to detect whether the service should run in debug mode
		/// </param>
		public static void Main(string[] args)
		{
            if ((Environment.UserInteractive) || (args.Length > 0 && args[0].ToLower() == "/debug"))
            {
				FuelsManagerServiceForm form = new FuelsManagerServiceForm();
				Application.Run(form);
			}
			else
			{
				ServiceBase[] servicesToRun = new ServiceBase[] 
					                              { 
						                              new FuelsManagerService() 
					                              };

				ServiceBase.Run(servicesToRun);
			}
		}
	}
}
