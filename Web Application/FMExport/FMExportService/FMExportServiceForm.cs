// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMExportServiceForm.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Used to run the FMExport Service in debug mode
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	/// <summary>
	/// Used to run the FMExportService Service in debug mode
	/// </summary>
	public partial class FMExportServiceForm : Form
	{
		private readonly FMExportService fmExportService;

		public FMExportServiceForm()
		{
			this.InitializeComponent();
			this.fmExportService = new FMExportService();
		}

		/// <summary>
		/// Fires when the start button is clicked
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void btnStart_Click(object sender, EventArgs e)
		{
			// If you don't start the service on a new thread, the WCF calls will not act concurrently.
			Task.Factory.StartNew(() => this.fmExportService.ProxyStart());

			this.btnStart.Enabled = false;
			this.btnStop.Enabled = true;
		}

		/// <summary>
		/// Fires when the stop button is clicked
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void btnStop_Click(object sender, EventArgs e)
		{
			this.fmExportService.ProxyStop();

			this.btnStart.Enabled = true;
			this.btnStop.Enabled = false;
		}
	}
}
