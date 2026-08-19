// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterpriseSyncSettingsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EnterpriseSyncSettingsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.Globalization;

	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for EnterpriseSyncSettingsPage.
	/// </summary>
	public partial class EnterpriseSyncSettingsPage : FMUserControlBase
	{
		#region Properties
		/// <summary>
		/// Get the Server Sync Configuration object from Session.
		/// </summary>
		private SyncServerConfigurationDO SessionSyncServerConfig
		{
			get
			{
				if (this.Session[PageSessionKeyConstants.SYNC_CONFIG_SERVER_SETTINGS] != null 
					&& this.Session[PageSessionKeyConstants.SYNC_CONFIG_SERVER_SETTINGS] is SyncServerConfigurationDO)
				{
					return (SyncServerConfigurationDO)this.Session[PageSessionKeyConstants.SYNC_CONFIG_SERVER_SETTINGS];
				}

				return null;
			}
			set
			{
				this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_SERVER_SETTINGS, value);
			}
		}
		#endregion Properties

		#region Public Methods and Operators
		/// <summary>
		/// Update the object in session with any data the user has entered on the page
		/// </summary>
		public void UpdateData()
		{
			try
			{
				var syncServerConfig = this.SessionSyncServerConfig;
				if (syncServerConfig == null)
				{
					syncServerConfig = new SyncServerConfigurationDO();
					this.SessionSyncServerConfig = syncServerConfig;

					// Reference the one in the session.
					syncServerConfig = this.SessionSyncServerConfig;
				}

				syncServerConfig.AllowSynchronizationFlag = this.EnableGlobalSynchronizationCheckBox.Checked;
				syncServerConfig.AcceptFMUserAuthenticationFlag = this.FuelsManagerAcceptUserIDCheckBox.Checked;
				syncServerConfig.AcceptClientCertificateAuthenticationFlag = this.FuelsManagerAcceptClientCertificateCheckBox.Checked;
				syncServerConfig.ClientSignatureRequiredForMessagesFlag = this.MessageSecurityClientSignatureRequiredCheckBox.Checked;
				syncServerConfig.ClientEncryptionRequiredForMessagesFlag = this.MessageSecurityClientEncryptionRequiredCheckBox.Checked;

				var nodeHealthCriticalThresholdHours = 
					(!string.IsNullOrEmpty(this.NodeHealthCriticalThresholdHoursTextBox.Text)
					&& TypeHelper.IsNumeric(this.NodeHealthCriticalThresholdHoursTextBox.Text))
						? Convert.ToInt32(this.NodeHealthCriticalThresholdHoursTextBox.Text) 
						: FMChannelHelper.DefaultNodeHealthCriticalThresholdHours;

				var nodeHealthCautionThresholdHours =
					(!string.IsNullOrEmpty(this.NodeHealthCautionThresholdHoursTextBox.Text)
					&& TypeHelper.IsNumeric(this.NodeHealthCautionThresholdHoursTextBox.Text))
						? Convert.ToInt32(this.NodeHealthCautionThresholdHoursTextBox.Text)
						: FMChannelHelper.DefaultNodeHealthCautionThresholdHours;

				if (nodeHealthCriticalThresholdHours <= nodeHealthCautionThresholdHours)
				{
					throw new Exception("Node health critical threshold hours must be greater than caution threshold hours.");
				}

				syncServerConfig.NodeHealthCriticalThresholdHours = nodeHealthCriticalThresholdHours;
				syncServerConfig.NodeHealthCautionThresholdHours = nodeHealthCautionThresholdHours;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Update the data displayed on the form
		/// </summary>
		public void UpdateView()
		{
			try
			{
				var syncServerConfig = (SyncServerConfigurationDO)this.Session[PageSessionKeyConstants.SYNC_CONFIG_SERVER_SETTINGS];

				if (null != syncServerConfig)
				{
					this.EnableGlobalSynchronizationCheckBox.Checked = syncServerConfig.AllowSynchronizationFlag;
					this.FuelsManagerAcceptUserIDCheckBox.Checked = syncServerConfig.AcceptFMUserAuthenticationFlag;
					this.FuelsManagerAcceptClientCertificateCheckBox.Checked = syncServerConfig.AcceptClientCertificateAuthenticationFlag;
					this.MessageSecurityClientSignatureRequiredCheckBox.Checked = syncServerConfig.ClientSignatureRequiredForMessagesFlag;
					this.MessageSecurityClientEncryptionRequiredCheckBox.Checked = syncServerConfig.ClientEncryptionRequiredForMessagesFlag;
					this.NodeHealthCriticalThresholdHoursTextBox.Text = syncServerConfig.NodeHealthCriticalThresholdHours.ToString(CultureInfo.InvariantCulture);
					this.NodeHealthCautionThresholdHoursTextBox.Text = syncServerConfig.NodeHealthCautionThresholdHours.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					this.EnableGlobalSynchronizationCheckBox.Checked = false;
					this.FuelsManagerAcceptUserIDCheckBox.Checked = false;
					this.FuelsManagerAcceptClientCertificateCheckBox.Checked = false;
					this.MessageSecurityClientSignatureRequiredCheckBox.Checked = false;
					this.MessageSecurityClientEncryptionRequiredCheckBox.Checked = false;
					this.NodeHealthCriticalThresholdHoursTextBox.Text = FMChannelHelper.DefaultNodeHealthCriticalThresholdHours.ToString(CultureInfo.InvariantCulture);
					this.NodeHealthCautionThresholdHoursTextBox.Text = FMChannelHelper.DefaultNodeHealthCautionThresholdHours.ToString(CultureInfo.InvariantCulture);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion Public Methods and Operators

		#region Page Event Handlers and Overrides
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// When the page loads, update the data displayed, but only if this is not a post back
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Not used</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.UpdateView();
				}

				if(FMFormBase.GetDataDictionaryFlag())
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