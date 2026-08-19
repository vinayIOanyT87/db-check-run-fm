// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientSyncSettingsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ClientSyncSettingsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.Globalization;
	using System.Security;
	using System.Security.Cryptography.X509Certificates;
	using System.Text.RegularExpressions;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Code behind for ClientSyncSettingsPage that allows users to configure the synchronization settings when acting as the client node.
	/// </summary>
	public partial class ClientSyncSettingsPage : FMUserControlBase
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating whether should refresh menu bar flag.
		/// </summary>
		public bool ShouldRefreshMenuBarFlag { get; private set; }

		/// <summary>
		/// Gets or sets the Client Sync Configuration object from Session.
		/// </summary>
		private SyncClientConfigurationDO SessionSyncClientConfig
		{
			get
			{
				if (this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS] != null 
					&& this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS] is SyncClientConfigurationDO)
				{
					return (SyncClientConfigurationDO)this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS];
				}
				
				return null;
			}

			set
			{
				this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS, value);
			}
		}
		#endregion Properties

		#region Public Methods and Operators
		/// <summary>
		/// The update data.
		/// </summary>
		/// <exception cref="Exception">
		/// Throws an exception if the Site / Site Group ID field is SiteAdmin.  Currently we do not allow synchronization of the entire Site tree to a remote node.
		/// </exception>
		public void UpdateData()
		{
			try
			{
				string enterpriseUrl = null;
				bool suspendSync = false;

				if (!string.IsNullOrEmpty(this.SiteOrSiteGroupIDTextBox.Text))
				{
					if (this.SiteOrSiteGroupIDTextBox.Text.Equals(
						"SiteAdmin", StringComparison.CurrentCultureIgnoreCase))
					{
						throw new Exception("Specified Site/SiteGroup ID cannot be specified for synchronization.");
					}
				}

				// Put the values in the attributes so that they are not lost during the postback.
				this.ServerAuthPasswordTextBox.Attributes["value"] = GeneralConstants.PasswordPlaceholder;
				this.FuelsManagerAuthPasswordTextBox.Attributes["value"] = GeneralConstants.PasswordPlaceholder;

				SyncClientConfigurationDO syncClientConfig = this.SessionSyncClientConfig;

				if (null == syncClientConfig)
				{
					syncClientConfig = new SyncClientConfigurationDO();
					this.SessionSyncClientConfig = syncClientConfig;

					// Reference the one in the session.
					syncClientConfig = this.SessionSyncClientConfig;
				}
				else
				{
					// Get the original values from storage so that we can see if the UI will change.
					var originalSyncClientConfig = FMChannelHelper.MakeCall<ISyncClientConfigurations, SyncClientConfigurationDO>((x) => x.Get(this.Security));

					// If we had something originally, get the original values for the EnterpriseURL and the suspend flag.
					if (null != originalSyncClientConfig && originalSyncClientConfig.IdentityGuid != Guid.Empty)
					{
						enterpriseUrl = originalSyncClientConfig.EnterpriseURL;
						suspendSync = originalSyncClientConfig.SuspendSynchronizationFlag;
					}
				}

				syncClientConfig.RootSiteID = this.SiteOrSiteGroupIDTextBox.Text;
				syncClientConfig.EnterpriseURL = this.EnterpriseURLTextBox.Text;
				syncClientConfig.SuspendSynchronizationFlag = this.SuspendSyncCheckbox.Checked;
				syncClientConfig.ServerAuthUserName = this.ServerAuthUserNameTextBox.Text;

				if (this.ServerAuthPasswordTextBox.Text != GeneralConstants.PasswordPlaceholder)
				{
					syncClientConfig.ServerAuthPassword = this.ServerAuthPasswordTextBox.Text;
				}
				
				syncClientConfig.ServerAuthDomain = this.ServerAuthDomainNameTextBox.Text;
				syncClientConfig.ServerAuthClientCertificate = Regex.Replace(this.ServerAuthClientCertificateTextBox.Text, @"[^\da-zA-z]", string.Empty).ToUpper(); //sanitize certificate input
								
				syncClientConfig.FMAuthUserName = this.FuelsManagerAuthUserNameTextBox.Text;

				if (this.FuelsManagerAuthPasswordTextBox.Text != GeneralConstants.PasswordPlaceholder)
				{
					syncClientConfig.FMAuthPassword = this.FuelsManagerAuthPasswordTextBox.Text;
				}

				var certStore = new X509Store(StoreLocation.LocalMachine); //validate that the server certificate can be found
				certStore.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection certColl;

				certColl = certStore.Certificates.Find(X509FindType.FindByThumbprint, syncClientConfig.ServerAuthClientCertificate, true);
				if (certColl.Count == 0) //otherwise, it should be the subject name
				{
					certColl = certStore.Certificates.Find(X509FindType.FindBySubjectName, syncClientConfig.ServerAuthClientCertificate, true);
				}
				if (certColl.Count == 0) //notify the user if the certificate is still not found
				{
					throw new Exception("Server Authentication Certificate could not be found. Check the thumbprint or name, and verify the certificate was installed");
				}
				certStore.Close();

				syncClientConfig.FMAuthClientCertificate = Regex.Replace(this.FuelsManagerAuthClientCertificateTextBox.Text, @"[^\da-zA-z]", string.Empty).ToUpper(); //sanitize certificate input

				syncClientConfig.MessageSecuritySigningCertificate = this.MessageSecuritySigningCertificateTextBox.Text;
				syncClientConfig.MessageSecurityOfflineEncryptionCertificate = this.MessageSecurityOfflineEncryptionCertificateTextBox.Text;
				syncClientConfig.MessageSecurityOfflineDecryptionCertificate = this.MessageSecurityOfflineDecryptionCertificateTextBox.Text;

				var certStoreFM = new X509Store(StoreLocation.LocalMachine); //validate that the FM certificate can be found
				certStoreFM.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection certCol;

				certCol = certStoreFM.Certificates.Find(X509FindType.FindByThumbprint, syncClientConfig.FMAuthClientCertificate, true);
				if (certCol.Count == 0) //otherwise, it should be the subject name
				{
					certCol = certStoreFM.Certificates.Find(X509FindType.FindBySubjectName, syncClientConfig.FMAuthClientCertificate, true);
				}
				if (certCol.Count == 0) //notify the user if the certificate is still not found
				{
					throw new Exception("FM Authentication Certificate could not be found. Check the thumbprint or name, and verify the certificate was installed");
				}
				certStoreFM.Close();

				syncClientConfig.ServiceMaximumRetryAttempts =
					(!string.IsNullOrEmpty(this.EntepriseServiceMaxRetryAttemptTextBox.Text) && TypeHelper.IsNumeric(this.EntepriseServiceMaxRetryAttemptTextBox.Text))
					? Convert.ToInt32(this.EntepriseServiceMaxRetryAttemptTextBox.Text) : FMChannelHelper.DefaultRetryAttempts;

				syncClientConfig.ServiceRetryWaitTime =
					(!string.IsNullOrEmpty(this.EnterpriseServiceRetryWaitTimeTextBox.Text) && TypeHelper.IsNumeric(this.EnterpriseServiceRetryWaitTimeTextBox.Text))
					? Convert.ToInt32(this.EnterpriseServiceRetryWaitTimeTextBox.Text) : FMChannelHelper.DefaultRetryWaitTime;

				bool enterpriseUrlClearOrSet = (string.IsNullOrEmpty(enterpriseUrl) && !string.IsNullOrEmpty(this.EnterpriseURLTextBox.Text))
												|| (!string.IsNullOrEmpty(enterpriseUrl) && string.IsNullOrEmpty(this.EnterpriseURLTextBox.Text));

				bool syncSuspendedChanged = (suspendSync && !this.SuspendSyncCheckbox.Checked)
											  || (!suspendSync && this.SuspendSyncCheckbox.Checked);

				if (enterpriseUrlClearOrSet || syncSuspendedChanged)
				{
					this.ShouldRefreshMenuBarFlag = true;
				}
			}
			catch
			{
				throw; //this is intentional because the page is redirected and no error will pop if the error is handled here
			}
		}

		/// <summary>
		/// The update view.
		/// </summary>
		public void UpdateView()
		{
			try
			{
				var syncClientConfig = (SyncClientConfigurationDO)this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS];

				if (null != syncClientConfig)
				{
					this.SiteOrSiteGroupIDTextBox.Text = syncClientConfig.RootSiteID;
					this.EnterpriseURLTextBox.Text = syncClientConfig.EnterpriseURL;
					this.SuspendSyncCheckbox.Checked = syncClientConfig.SuspendSynchronizationFlag;
					this.ServerAuthUserNameTextBox.Text = syncClientConfig.ServerAuthUserName;
					this.ServerAuthPasswordTextBox.Attributes["value"] = GeneralConstants.PasswordPlaceholder;
					this.ServerAuthDomainNameTextBox.Text = syncClientConfig.ServerAuthDomain;
					this.ServerAuthClientCertificateTextBox.Text = syncClientConfig.ServerAuthClientCertificate;
					this.FuelsManagerAuthUserNameTextBox.Text = syncClientConfig.FMAuthUserName;
					this.FuelsManagerAuthPasswordTextBox.Attributes["value"] = GeneralConstants.PasswordPlaceholder;
					this.FuelsManagerAuthClientCertificateTextBox.Text = syncClientConfig.FMAuthClientCertificate;
					this.MessageSecuritySigningCertificateTextBox.Text = syncClientConfig.MessageSecuritySigningCertificate;
					this.MessageSecurityOfflineEncryptionCertificateTextBox.Text = syncClientConfig.MessageSecurityOfflineEncryptionCertificate;
					this.MessageSecurityOfflineDecryptionCertificateTextBox.Text = syncClientConfig.MessageSecurityOfflineDecryptionCertificate;

					this.EntepriseServiceMaxRetryAttemptTextBox.Text = syncClientConfig.ServiceMaximumRetryAttempts.ToString(CultureInfo.InvariantCulture);
					this.EnterpriseServiceRetryWaitTimeTextBox.Text = syncClientConfig.ServiceRetryWaitTime.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					this.SiteOrSiteGroupIDTextBox.Text = string.Empty;
					this.EnterpriseURLTextBox.Text = string.Empty;
					this.SuspendSyncCheckbox.Checked = false;
					this.ServerAuthUserNameTextBox.Text = string.Empty;
					this.ServerAuthPasswordTextBox.Text = string.Empty;
					this.ServerAuthDomainNameTextBox.Text = string.Empty;
					this.ServerAuthClientCertificateTextBox.Text = string.Empty;
					this.FuelsManagerAuthUserNameTextBox.Text = string.Empty;
					this.FuelsManagerAuthPasswordTextBox.Text = string.Empty;
					this.FuelsManagerAuthClientCertificateTextBox.Text = string.Empty;
					this.MessageSecuritySigningCertificateTextBox.Text = string.Empty;
					this.MessageSecurityOfflineEncryptionCertificateTextBox.Text = string.Empty;
					this.MessageSecurityOfflineDecryptionCertificateTextBox.Text = string.Empty;

					this.EntepriseServiceMaxRetryAttemptTextBox.Text = FMChannelHelper.DefaultRetryAttempts.ToString(CultureInfo.InvariantCulture);
					this.EnterpriseServiceRetryWaitTimeTextBox.Text = FMChannelHelper.DefaultRetryWaitTime.ToString(CultureInfo.InvariantCulture);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion Public Methods and Operators

		#region Page Event Handlers and Overrides
		/// <summary>
		/// Overrides the virtual <code>OnInit</code> framework method.  Any special page specific initialization can be performed here.
		/// </summary>
		/// <param name="e">
		/// Event arguments of type <see cref="EventArgs"/> for the <code>OnInit</code> method.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
            this.SiteOrSiteGroupIDTextBox.TextChanged += SiteOrSiteGroupIDTextBox_TextChanged;
			base.OnInit(e);
		}

        private void SiteOrSiteGroupIDTextBox_TextChanged(object sender, EventArgs e)
        {
			var parentForm = this.Page as SynchronizationConfigForm;
			if (parentForm != null)
			{
				var syncDataStoreName = parentForm.SessionSyncDataStoreName;
				syncDataStoreName = this.SiteOrSiteGroupIDTextBox.Text + "_" + syncDataStoreName.Substring(syncDataStoreName.IndexOf(Environment.MachineName));
				parentForm.SessionSyncDataStoreName = syncDataStoreName;
				parentForm.DataStoreNameTextBox.Text = syncDataStoreName;
			}
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        [SecurityCritical]
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.ShouldRefreshMenuBarFlag = false;

				if (!this.Page.IsPostBack)
				{
					this.UpdateView();
				}
				else
				{
					// During a postback event, we need to keep the contents of the password fields in the event they haven't been saved yet.
					this.ServerAuthPasswordTextBox.Attributes["value"] = this.ServerAuthPasswordTextBox.Text;
					this.FuelsManagerAuthPasswordTextBox.Attributes["value"] = this.FuelsManagerAuthPasswordTextBox.Text;
				}

				// this.Page.ClientScript.RegisterStartupScript(typeof(string), "highlightPwds", "");

				// If a successful complete synchronization has already taken place, then we will not allow the user to change the synchronization Site/SiteGroup ID.
				// This prevents nodes from cycling through Sites/SiteGroups and pulling down all data from the enterprise.

				if (FMFormBase.GetDataDictionaryFlag())
				{
					string newValue = DataDictionarySingleton.Get(this.Security.SiteGuid, "FuelsManager");
					this.FuelsManagerAuthSectionLabel.Text = newValue + " Authentication";
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion Page Event Handlers and Overrides
	}
}