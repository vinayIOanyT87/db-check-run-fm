// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInstaller.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The installer for the FMExport Service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System.ComponentModel;
	using System.Configuration.Install;

	/// <summary>
	/// The installer for the FMExport Service
	/// </summary>
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectInstaller"/> class.
		/// </summary>
		public ProjectInstaller()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// The after install handler.
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="args">The install event args.
		/// </param>
		private void AfterInstallHandler(object sender, InstallEventArgs args)
		{
		}
	}
}
