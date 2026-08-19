// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpcUaServerServiceMainEntry.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMOpcUaServerService
{
	using System;
	using System.ServiceProcess;
	using System.Windows.Forms;

	static class OpcUaServerServiceMainEntry
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			if ((Environment.UserInteractive) || (args.Length > 0 && args[0].ToLower() == "/debug"))
			{
				FMOpcUaServerServiceForm form = new FMOpcUaServerServiceForm();
				Application.Run(form);
			}
			else
			{
			    var servicesToRun = new ServiceBase[] { new OpcUaServerService() };

			    ServiceBase.Run(servicesToRun);
			}
		}
	}
}
