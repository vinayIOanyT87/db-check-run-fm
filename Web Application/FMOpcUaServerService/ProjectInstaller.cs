// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInstaller.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The installer for the FMOpcUaServerService
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMOpcUaServerService
{
	using System.ComponentModel;
	using System.Configuration.Install;

	/// <summary>
	///  The installer for the FMOpcUaServerService
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
	}
}
