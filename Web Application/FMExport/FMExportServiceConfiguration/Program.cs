// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the main program for the FMExport service configuration utility.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportServiceConfiguration
{
	using System;
	using System.Windows.Forms;

	/// <summary>
	/// The program.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}