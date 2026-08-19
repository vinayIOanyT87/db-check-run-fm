// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMExportServiceMainEntry.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The main entry point for the FMExport Service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System.ServiceProcess;
	using System.Windows.Forms;

	/// <summary>
	/// The main entry point for the FMExport Service
	/// </summary>
	public static class FMExportServiceMainEntry
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// <param name="args">
		/// Used to detect whether the service should run in debug mode
		/// </param>
		public static void Main(string[] args)
		{
			if (args.Length > 0 && args[0].ToLower() == "/debug")
			{
				var form = new FMExportServiceForm();
				Application.Run(form);
			}
			else
			{
				var servicesToRun = new ServiceBase[] { new FMExportService() };

				ServiceBase.Run(servicesToRun);
			}
		}
	}
}
