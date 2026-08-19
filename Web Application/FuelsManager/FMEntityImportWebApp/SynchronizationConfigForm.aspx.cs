// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationConfigForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SynchronizationConfigForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for SiteForm.
	/// </summary>
	public partial class SynchronizationConfigForm : FMAutoSubmitFormBase
	{
		private const string BasePath = "../FMWebApp";

		#region Properties
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

		/// <summary>
		/// Gets or sets the SyncServerConfiguration object from Session.
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

		/// <summary>
		/// Gets or sets the PeriodicSyncSettings object from Session.
		/// </summary>
		private SiteCollectionClass SessionPeriodicSyncSettings
		{
			get
			{
				if (this.Session[PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS] != null
					&& this.Session[PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS] is SiteCollectionClass)
				{
					return (SiteCollectionClass)this.Session[PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS];
				}

				return null;
			}
			set
			{
				this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS, value);
			}
		}

		/// <summary>
		/// Gets or sets the Synchronization Data Store ID value from Session.
		/// </summary>
		private string SessionSyncDataStoreID
		{
			get
			{
				if (this.Session[PageSessionKeyConstants.SYNC_DATA_STORE_ID] != null
					&& this.Session[PageSessionKeyConstants.SYNC_DATA_STORE_ID] is string)
				{
					return (string)this.Session[PageSessionKeyConstants.SYNC_DATA_STORE_ID];
				}

				return null;
			}
			set
			{
				this.Session.Add(PageSessionKeyConstants.SYNC_DATA_STORE_ID, value);
			}
		}

		/// <summary>
		/// Gets or sets the Synchronization Data Store Name value from Session.
		/// </summary>
		public string SessionSyncDataStoreName
		{
			get
			{
				if (this.Session[PageSessionKeyConstants.SYNC_DATA_STORE_NAME] != null
					&& this.Session[PageSessionKeyConstants.SYNC_DATA_STORE_NAME] is string)
				{
					return (string)this.Session[PageSessionKeyConstants.SYNC_DATA_STORE_NAME];
				}

				return null;
			}
			set
			{
				this.Session.Add(PageSessionKeyConstants.SYNC_DATA_STORE_NAME, value);
			}
		}
		#endregion Properties

		#region Methods and Operators
		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the site form.
		/// </summary>
		/// <param name="enable">
		/// A value of True will enable the Ok button if modification rights were granted, otherwise; False will disable the Ok button.
		/// </param>
		public void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS)
				|| this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS)
				|| this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS))
			{
				this.OK.Enabled = enable;
			}

			this.Cancel.Enabled = enable;

			this.tcSyncConfigTabs.HeaderEnabled = enable;
		}

		/// <summary>
		/// Update all object(s) in session with any data the user has entered on the page
		/// </summary>
		public void UpdateData()
		{
		}

		/// <summary>
		/// Populate the fields on the screen with data
		/// </summary>
		private void UpdateView()
		{
			try
			{
				// Get the stored Synchronization Settings
				SyncClientConfigurationDO syncClientConfig = FMChannelHelper.MakeCall<ISyncClientConfigurations, SyncClientConfigurationDO>((config) => config.Get(this.Security));
				syncClientConfig.Changed = false;
				
				this.SessionSyncClientConfig = syncClientConfig;

				SyncServerConfigurationDO syncServerConfig = FMChannelHelper.MakeCall<ISyncServerConfigurations, SyncServerConfigurationDO>((config) => config.Get(this.Security));
				syncServerConfig.Changed = false;
				
				this.SessionSyncServerConfig = syncServerConfig;

				if (null == this.SessionSyncClientConfig)
				{
					this.SessionSyncClientConfig = new SyncClientConfigurationDO();
				}

				if (null == this.SessionSyncServerConfig)
				{
					this.SessionSyncServerConfig = new SyncServerConfigurationDO();
				}

				if (null == this.SessionPeriodicSyncSettings)
				{
					SiteCollectionClass siteCollection;
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																			 x =>
																			 x.Get(
																					this.Security,
																					this.Security.SiteGuid,
																					getMemberSites: false,
																					getSchedulesAndProcessVariables: false,
																					bGetAssociatedAliases: false));
					if (site.SiteGroup)
					{
						if (site.IsAdminSite)
						{
							siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																			 x =>
																			 x.Enumerate(this.Security));
						}
						else
						{
							siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																			 x =>
																			 x.EnumerateByParentSite(this.Security, this.Security.SiteGuid));
							siteCollection.Add(site);
						}
					}
					else
					{
						siteCollection = new SiteCollectionClass();
						siteCollection.Add(site);
					}

					this.SessionPeriodicSyncSettings = siteCollection;
				}

				// Get the one time Synchronization Data Store Identification
				var existingDataStoreID = FMChannelHelper.MakeCall<IConfigurationSettings, string>((x) => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid));

				if (string.IsNullOrEmpty(existingDataStoreID))
				{
					existingDataStoreID = Guid.NewGuid().ToString();
				}

				this.SessionSyncDataStoreID = existingDataStoreID;
				this.DataStoreIDTextBox.Text = this.SessionSyncDataStoreID;

				var existingDataStoreName = FMChannelHelper.MakeCall<IConfigurationSettings, string>((x) => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName));

				// If this was already initialized then don't let them change it.
				if (string.IsNullOrEmpty(existingDataStoreName))
				{
					this.DataStoreNameTextBox.ReadOnly = false;
					this.DataStoreNameTextBox.Enabled = true;

					Random rand = new Random();
					existingDataStoreName = string.Format("{0}_{1}", Environment.MachineName, rand.Next(1000, 9999));
				}
				else
				{
					this.DataStoreNameTextBox.ReadOnly = true;
					this.DataStoreNameTextBox.Enabled = false;
				}

				this.SessionSyncDataStoreName = existingDataStoreName;
				this.DataStoreNameTextBox.Text = this.SessionSyncDataStoreName;
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion Methods and Operators

		#region Page Events and Overrides
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
			base.OnInit(e);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
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
		/// <exception cref="Exception">
		/// Throws an exception for Insufficient Rights if the user does not have ANY View rights for Client, Server and Periodic Synchronization Settings.
		/// </exception>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
								this.ClearSessionErrors();

				if (!this.Security.HasRight(RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS)
					&& !this.Security.HasRight(RIGHT.VIEW_SYNC_CONFIG_SERVER_SETTINGS)
					&& !this.Security.HasRight(RIGHT.VIEW_SYNC_CONFIG_SITE_SETTINGS))
				{
					throw new Exception("Insufficient Rights");
				}
				
				if (!this.Page.IsPostBack)
				{
					this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS);
					this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SERVER_SETTINGS);
					this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS);
					this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS_MODIFIED);
					this.Session.Remove(PageSessionKeyConstants.SYNC_DATA_STORE_ID);
					this.Session.Remove(PageSessionKeyConstants.SYNC_DATA_STORE_NAME);

					this.UpdateView();
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS)
					&& !this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS)
					&& !this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS))
				{
					this.OK.Enabled = false;
				}

				this.tpClientSyncSettings.Visible = false;
				this.tpEnterpriseSyncSettings.Visible = false;
				this.tpSiteSyncSettingsPage.Visible = false;

				if (this.Security.HasRight(RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS)
					|| this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS))
				{
					this.tpClientSyncSettings.Visible = true;
					this.tpClientSyncSettings.HeaderText = this.GetTranslatedText("Client Settings");
				}

				if (this.Security.HasRight(RIGHT.VIEW_SYNC_CONFIG_SERVER_SETTINGS)
					|| this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS))
				{
					this.tpEnterpriseSyncSettings.Visible = true;
					this.tpEnterpriseSyncSettings.HeaderText = this.GetTranslatedText("Enterprise Settings");
				}

				if (this.Security.HasRight(RIGHT.VIEW_SYNC_CONFIG_SITE_SETTINGS)
					|| this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS))
				{
					this.tpSiteSyncSettingsPage.Visible = true;
					this.tpSiteSyncSettingsPage.HeaderText = this.GetTranslatedText("Site Settings");
				}

				// We could be coming back from another page
				if (this.Session["SynchronizationConfig.TabIndex"] != null)
				{
					this.tcSyncConfigTabs.ActiveTabIndex = (int)this.Session["SynchronizationConfig.TabIndex"];
					this.Session.Remove("SynchronizationConfig.TabIndex");
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion Page Events and Overrides

		#region Control  Event Handlers
		/// <summary>
		/// The cancel_ command.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS);
			this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SERVER_SETTINGS);
			this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS);

			this.Session.Remove(PageSessionKeyConstants.SYNC_DATA_STORE_ID);
			this.Session.Remove(PageSessionKeyConstants.SYNC_DATA_STORE_NAME);

			this.Redirect( BasePath + "/FuelsManagerForm.aspx" );
		}

		/// <summary>
		/// The o k_ command.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				if (this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS)
					|| this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS))
				{
					if (!string.IsNullOrEmpty(this.SessionSyncDataStoreID))
					{
						// Don't let them blank it out from here.
						if (string.IsNullOrEmpty(this.DataStoreIDTextBox.Text))
						{
							this.DataStoreIDTextBox.Text = this.SessionSyncDataStoreID;
						}

						var existingDataStoreID = FMChannelHelper.MakeCall<IConfigurationSettings, string>((x) => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid));

						// We will only set this one time; only when the existing value is not initialized;
						if (string.IsNullOrEmpty(existingDataStoreID))
						{
							FMChannelHelper.MakeCall<IConfigurationSettings>(
								(x) => x.Modify(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid, this.SessionSyncDataStoreID));
						}
					}

					if (!string.IsNullOrEmpty(this.SessionSyncDataStoreName))
					{
						// Don't let them blank it out from here.
						if (string.IsNullOrEmpty(this.DataStoreNameTextBox.Text))
						{
							this.DataStoreNameTextBox.Text = this.SessionSyncDataStoreName;
						}
						else if (!this.DataStoreNameTextBox.Text.Equals(this.SessionSyncDataStoreName))
						{
							this.SessionSyncDataStoreName = this.DataStoreNameTextBox.Text;
						}

						var existingDataStoreName = FMChannelHelper.MakeCall<IConfigurationSettings, string>((x) => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName));

						// Originally We will only set this one time; only when the existing value is not initialized;
						// Revised to permit resetting.  This is because now the Site/Site Group is a prefix to the DataStoreName
						// and we must account for the possiblity that it isn't entered properly and needs to be corrected.
						// Also must change the SyncNodeGuid in case sync was performed and sessions are logged.
						if (string.IsNullOrEmpty(existingDataStoreName)
						|| !existingDataStoreName.Equals(this.SessionSyncDataStoreName))
						{
							FMChannelHelper.MakeCall<IConfigurationSettings>(
								(x) => x.Modify(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName, this.SessionSyncDataStoreName));

							this.SessionSyncDataStoreID = Guid.NewGuid().ToString(); ;
							this.DataStoreIDTextBox.Text = this.SessionSyncDataStoreID;


							FMChannelHelper.MakeCall<IConfigurationSettings>(
								(x) => x.Modify(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid, this.SessionSyncDataStoreID));


						}
					}
				}

				if (this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS))
				{
					// Update the Client Configuration Settings
					SyncClientConfigurationDO syncClientConfig = this.SessionSyncClientConfig;

					if (null != syncClientConfig)
					{
						this.ClientSyncSettingsPage.UpdateData();

						// If there is no identity guid, we are creating a new record, otherwise, we're modifying an existing record
						if (syncClientConfig.IdentityGuid == Guid.Empty)
						{
							syncClientConfig.IdentityGuid = FMChannelHelper.MakeCall<ISyncClientConfigurations, Guid>((x) => x.Add(this.Security, syncClientConfig));
						}
						else
						{
							FMChannelHelper.MakeCall<ISyncClientConfigurations>((x) => x.Modify(this.Security, syncClientConfig));
						}
					}
				}

				if (this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS))
				{
					// Update the Server Configuration Settings
					SyncServerConfigurationDO syncServerConfig = this.SessionSyncServerConfig;

					if (null != syncServerConfig)
					{
						this.EnterpriseSyncSettingsPage.UpdateData();

						// If there is no identity guid, we are creating a new record, otherwise, we're modifying an existing record
						if (syncServerConfig.IdentityGuid == Guid.Empty)
						{
							syncServerConfig.IdentityGuid = FMChannelHelper.MakeCall<ISyncServerConfigurations, Guid>((x) => x.Add(this.Security, syncServerConfig));
						}
						else
						{
							FMChannelHelper.MakeCall<ISyncServerConfigurations>((x) => x.Modify(this.Security, syncServerConfig));
						}
					}
				}

				if (this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS))
				{
					// Update the Periodic Synchronization Configuration Settings
					SiteCollectionClass siteList = this.SessionPeriodicSyncSettings;

					if (null != siteList)
					{
						// Check each one and see if the periodic sync configuration has changed, only update Sites that have changed.
						foreach (SiteClass site in siteList)
						{
							FMChannelHelper.MakeCall<ISites>((x) => { x.Modify(this.Security, DATA_TYPE.SYNCCONFIG, site, false); });
						}

						this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS_MODIFIED);
					}
				}

				//this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS);
				//this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SERVER_SETTINGS);
				//this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS);

				// Refresh the menubar only if the configuration changes enabled or disabled specific menu options.
				if (this.ClientSyncSettingsPage.ShouldRefreshMenuBarFlag)
				{
					this.ucFMMenuBar.Refresh();
				}

				this.Context.ApplicationInstance.CompleteRequest();
			
				this.Redirect( BasePath + "/FuelsManagerForm.aspx" );
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion Control  Event Handlers
	}
}
