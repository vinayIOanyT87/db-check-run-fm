// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMBAddressConfiguration.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for FMAddressConfiguration dialog
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dispatch
{
	using System;
	using System.Configuration;
	using System.Windows.Forms;

	/// <summary>
	/// Code behind for FMAddressConfiguration dialog
	/// </summary>
	public partial class FMBAddressConfiguration : Form
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMBAddressConfiguration"/> class.
		/// </summary>
		public FMBAddressConfiguration()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Load" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			this.Text = string.Format("{0}FuelsManager Dispatch Server Address", "ARG0");

			var endPointAddress = ConfigurationManager.AppSettings["DispatchEndPointAddress"];

			if (string.IsNullOrEmpty(endPointAddress) == false)
			{
				this.EndPointAddressTextBox.Text = endPointAddress;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Shown" /> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnShown( EventArgs e )
		{
			base.OnShown( e );

			this.EndPointAddressTextBox.Focus();
			this.EndPointAddressTextBox.SelectAll();
		}

		/// <summary>
		/// Handles the Click event of the OKButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void OkButtonClick( object sender, EventArgs e )
		{
			try
			{
				if (string.IsNullOrEmpty(this.EndPointAddressTextBox.Text))
				{
					throw new ApplicationException("Dispatch server addresss cannot be blank.");
				}

				Configuration configuration = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );

				configuration.AppSettings.Settings.Remove("DispatchEndPointAddress");
				configuration.AppSettings.Settings.Add("DispatchEndPointAddress", this.EndPointAddressTextBox.Text);

				// Save the configuration file.
				configuration.Save( ConfigurationSaveMode.Modified );

				// Force a reload of a changed section.
				ConfigurationManager.RefreshSection( "appSettings" );

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, string.Format("{0}FuelsManager Dispatch", "ARG0"));
			}
		}

		/// <summary>
		/// Handles the Click event of the CancelButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void CancelButtonClick( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
