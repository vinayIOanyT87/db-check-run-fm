// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginForm.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   A form which allows a user to login to FuelsManager
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMExportServiceConfiguration
{
	using System;
	using System.Windows.Forms;

	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FMExportService;

	/// <summary>
	/// A form which allows a user to login to FuelsManager
	/// </summary>
	public partial class LoginForm : Form
	{
		/// <summary>
		/// The security object used for interacting with FuelsManager
		/// </summary>
		private SecurityClass security;

		/// <summary>
		/// Construct the LoginForm
		/// </summary>
        public LoginForm()
        {
            this.InitializeComponent();
        }

		/// <summary>
		/// Exposes the security object used for interacting with FuelsManager
		/// </summary>
		public SecurityClass Security
		{
			get
			{
				return this.security;
			}
		}

		/// <summary>
		/// Attempt to login with the provided credentials
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void LoginButtonClick(object sender, EventArgs e)
		{
			try
			{
				bool changePassword;
				int daysUntilExpiration;
				this.security = null;

				var securityLoginRequest = new SecurityLoginRequest
				{
					CACEnabled = false,
					Password = this.PasswordTextBox.Text,
					SiteID = "SiteAdmin",
					UserID = this.UserNameTextBox.Text,
					TimeOut = -1
				};

				string token = FMChannelHelper.MakeCall<IFMExportService, string>(
					MainForm.BindingType,
					MainForm.BindingConfiguration,
					MainForm.FMExportServiceAddress,
						sites => sites.Login(out changePassword, out daysUntilExpiration, out this.security, securityLoginRequest));

				if (token != null && (token.StartsWith("User") || token.ToUpper().StartsWith("LOGIN FAILED")))
				{
					throw new Exception(token);
				}

				if (this.security == null)
				{
					throw new Exception(token);
				}

				if (!this.security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
				{
					throw new Exception("The user provided does not have sufficient rights to use the FuelsManager Export Configuration Utility");
				}

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch (Exception exception)
			{
				FMExportServiceLogger.Instance.LogError(exception.ToString());
				MessageBox.Show(this, exception.Message, this.Text);
			}      
		}
    }
}
