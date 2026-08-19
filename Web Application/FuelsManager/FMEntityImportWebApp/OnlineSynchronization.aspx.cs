// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineSynchronization.aspx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the OnlineSynchronization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.Configuration;
	using System.Globalization;
	using System.Text;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMSynchronizationCommon;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// The online synchronization.
	/// </summary>
	public partial class OnlineSynchronization : FMFormBase
	{
		#region Attributes
		/// <summary>
		/// The default auto refresh interval.
		/// </summary>
		private const int DefaultAutoRefreshInterval = -1;
		#endregion Attributes

		#region Properties
		/// <summary>
		/// Gets or sets the Client Sync Configuration object from Session.
		/// </summary>
		private SyncClientConfigurationDO SessionSyncClientConfig
		{
				get
				{
					var @do = this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS] as SyncClientConfigurationDO;
					return @do;
				}
			set
				{
					this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS, value);
				}
		}

		/// <summary>
		/// Gets or sets the SyncOnlineServiceState object from Session.
		/// </summary>
		private SyncServiceStateDO SyncServiceState
		{
				get
				{
					var @do = this.Session[PageSessionKeyConstants.SYNC_ONLINE_SERVICE_STATE] as SyncServiceStateDO;
					return @do;
				}
			set
				{
					this.Session.Add(PageSessionKeyConstants.SYNC_ONLINE_SERVICE_STATE, value);
				}
		}

		/// <summary>
		/// Gets or sets the SyncServiceBindingType information from Session.
		/// </summary>
		private string SyncServiceBindingType
		{
				get
				{
					var s = this.Session[PageSessionKeyConstants.SYNC_WINSERVICE_BINDING_TYPE] as string;
					return s;
				}
			set
				{
					this.Session.Add(PageSessionKeyConstants.SYNC_WINSERVICE_BINDING_TYPE, value);
				}
		}

		/// <summary>
		/// Gets or sets the SyncServiceBindingConfiguration information from Session.
		/// </summary>
		private string SyncServiceBindingConfiguration
		{
				get
				{
					return this.Session[PageSessionKeyConstants.SYNC_WINSERVICE_BINDING_CONFIGURATION] as string;
				}
			set
				{
					this.Session.Add(PageSessionKeyConstants.SYNC_WINSERVICE_BINDING_CONFIGURATION, value);
				}
		}

		/// <summary>
		/// Gets or sets the SyncServiceBindingEndPointAddress information from Session.
		/// </summary>
		private string SyncServiceBindingEndPointAddress
		{
				get
				{
					return this.Session[PageSessionKeyConstants.SYNC_WINSERVICE_BINDING_END_POINT_ADDRESS] as string;
				}
			set
				{
					this.Session.Add(PageSessionKeyConstants.SYNC_WINSERVICE_BINDING_END_POINT_ADDRESS, value);
				}
		}

		/// <summary>
		/// Gets or sets the sync request type.
		/// </summary>
		private SYNCREQUESTTYPE? SyncRequestType
		{
				get
				{
					return this.Session[PageSessionKeyConstants.SYNC_MANUAL_REQUEST_TYPE] as SYNCREQUESTTYPE?;
				}
			set
				{
					this.Session.Add(PageSessionKeyConstants.SYNC_MANUAL_REQUEST_TYPE, value);
				}
		}

		/// <summary>
		/// Gets or sets the sync site data object that will be used for the synchronization session
		/// </summary>
		private SyncSelectedSiteDO SelectedSyncSite
		{
				get
				{
					var @do = this.Session[PageSessionKeyConstants.SYNC_MANUAL_SELECTED_SITEID] as SyncSelectedSiteDO;
					if (@do != null)
					{
						return @do;
					}
					
				if (null != this.Security)
					{
						return new SyncSelectedSiteDO() { SiteID = this.Security.SiteID, SiteGuid = this.Security.SiteGuid };
					}
					
				return new SyncSelectedSiteDO();
				}
			set
				{
					this.Session.Add(PageSessionKeyConstants.SYNC_MANUAL_SELECTED_SITEID, value);
				}
		}

		/// <summary>
		/// Gets or sets the refresh interval.
		/// </summary>
		private int RefreshInterval 
		{
				get
				{
					if (this.Session[PageSessionKeyConstants.SYNC_MANUAL_AUTOREFRESH_INTERVAL] is int)
					{
						return (int)this.Session[PageSessionKeyConstants.SYNC_MANUAL_AUTOREFRESH_INTERVAL];
					}
					
				return DefaultAutoRefreshInterval;
				}
			set
				{
					this.Session.Add(PageSessionKeyConstants.SYNC_MANUAL_AUTOREFRESH_INTERVAL, value);
				}
		}
		#endregion Properties

		#region Methods and Operators

		/// <summary>
		/// Update all object(s) in session with any data the user has entered on the page
		/// </summary>
		public void UpdateData()
		{
				//this.SiteSyncSettingsPage.UpdateData();
		}

		/// <summary>
		/// Populate the fields on the screen with data
		/// </summary>
		private void UpdateView()
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					// Determine the current state of synchronization
					this.GetSecurity();

					// Get the stored Synchronization Settings
					SyncClientConfigurationDO syncClientConfig =
							FMChannelHelper.MakeCall<ISyncClientConfigurations, SyncClientConfigurationDO>(
								config => config.Get(this.Security));
					syncClientConfig.Changed = false;

					this.SessionSyncClientConfig = syncClientConfig;

					if (null != this.SessionSyncClientConfig)
					{
						StringBuilder syncMessage = new StringBuilder();

						// Determine if we need to perform an initial synchronization request, 
						// resynchronization request (post update) or a standard manual synchronization for deltas.
						this.SyncRequestType =
							FMChannelHelper.MakeCall<ISyncControllerProcessor, SYNCREQUESTTYPE>(
									syncControllerProcessorChannel =>
									syncControllerProcessorChannel.GetSynchronizationRequestType(this.Security));

						if (null == this.SyncRequestType)
						{
							this.SyncRequestType = SYNCREQUESTTYPE.MANUAL;
							syncMessage.Append("Defaulting to Manual synchronization request.");
						}

						// Determine which SiteId to use when performing the synchronization request.  
						// This is all dependent on the determine request type.
						this.SelectedSyncSite =
							FMChannelHelper.MakeCall<ISyncControllerProcessor, SyncSelectedSiteDO>(
								syncControllerProcessorChannel =>
								syncControllerProcessorChannel.GetSynchronizationSiteId(
										this.Security, this.SyncRequestType.Value));

						if (string.IsNullOrEmpty(this.SelectedSyncSite.SiteID))
						{
							throw new Exception("Error identifying Site / Site Group ID to synchronize.");
						}

						switch (this.SyncRequestType)
						{
							case SYNCREQUESTTYPE.INIT:
								syncMessage.Append("Performing first time synchronization for ");
								break;
							case SYNCREQUESTTYPE.RESYNC:
								syncMessage.Append("Resynchronization required for ");
								break;
							case SYNCREQUESTTYPE.MANUAL:
								break;
						}

						syncMessage.Append($@"Synchronizing Site / Site Group ID: '{this.SelectedSyncSite.SiteID}'");

						this.SyncMessage.Visible = true;
						this.SyncMessage.Text = syncMessage.ToString();
					}
				}

				this.UpdateSyncState();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		private void UpdateSyncState()
		{
				// Determine the current state of the synchronization service
				this.SyncServiceState =
					FMSyncServiceChannelHelper.MakeCall<ISynchronizationServices, SyncServiceStateDO>(
						x => x?.GetServiceState(this.Security));

				this.SynchronizeButton.Enabled = false;
				this.StopSynchronizationButton.Enabled = false;

				if (null == this.SyncServiceState)
				{
					this.SyncServiceState = new SyncServiceStateDO { SyncServiceState = SYNCSERVICESTATE.UNAVAILABLE };
				}

				switch (this.SyncServiceState.SyncServiceState)
				{
					case SYNCSERVICESTATE.READY:
						this.SynchronizeButton.Enabled = true;
						this.ServiceStatusIdleLabel.Visible = true;
						break;
					case SYNCSERVICESTATE.IN_PROGRESS:
						this.StopSynchronizationButton.Enabled = true;
						this.ServiceStatusInProgressLabel.Visible = true;
						break;
					case SYNCSERVICESTATE.DISABLED_LOCALLY:
						this.ServiceStatusDisabledLocallyLabel.Visible = true;
						break;
					case SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING:
					case SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING_SITE:
						this.ServiceStatusNotAcceptingLabel.Visible = true;
						break;
					default:
						this.ServiceStatusWindowsServiceUnavailable.Visible = true;
						break;
				}
		}
		#endregion Methods and Operators

		#region Page Events and Overrides
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
		/// Throws an exception if the Synchronization Service Binding Type, Configuration or EndPoint Address is missing from the application settings.
		/// </exception>
		protected void Page_Load(object sender, EventArgs e)
		{
				try
				{
					this.ClearSessionErrors();

					this.Response.CacheControl = "no-cache";
					this.Response.AddHeader("Pragma", "no-cache");
					this.Response.Expires = -1;


					this.GetSecurity();

					if (string.IsNullOrEmpty(this.SyncServiceBindingType))
					{
					this.SyncServiceBindingType = ConfigurationManager.AppSettings[FMSyncServiceChannelHelper.BindingTypeConfigKey];

						if (string.IsNullOrEmpty(this.SyncServiceBindingType))
						{
								throw new Exception(FMSyncServiceChannelHelper.BindingTypeConfigKey + " not found in configuration");
						}
					}

					if (string.IsNullOrEmpty(this.SyncServiceBindingConfiguration))
					{
						this.SyncServiceBindingConfiguration = ConfigurationManager.AppSettings[FMSyncServiceChannelHelper.BindingConfigurationConfigKey];

						if (string.IsNullOrEmpty(this.SyncServiceBindingConfiguration))
						{
								throw new Exception(FMSyncServiceChannelHelper.BindingConfigurationConfigKey + " not found in configuration");
						}
					}

					if (string.IsNullOrEmpty(this.SyncServiceBindingEndPointAddress))
					{
						this.SyncServiceBindingEndPointAddress = ConfigurationManager.AppSettings[FMSyncServiceChannelHelper.BindingEndpointAddressConfigKey];

						if (string.IsNullOrEmpty(this.SyncServiceBindingEndPointAddress))
						{
								throw new Exception(FMSyncServiceChannelHelper.BindingEndpointAddressConfigKey + " not found in configuration");
						}
					}

					this.ServiceStatusDisabledLocallyLabel.Visible = false;
					this.ServiceStatusIdleLabel.Visible = false;
					this.ServiceStatusInProgressLabel.Visible = false;
					this.ServiceStatusNotAcceptingLabel.Visible = false;
					this.ServiceStatusWindowsServiceUnavailable.Visible = false;
					this.SyncMessage.Visible = false;

					if (!this.Security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
					{
						this.SynchronizeButton.Enabled = false;
						this.StopSynchronizationButton.Enabled = false;

						throw new Exception("Insufficient Rights for Synchronization");
					}

					this.UpdateView();

					if (this.SyncServiceState != null)
					{
						if (this.SyncServiceState.SyncServiceState != SYNCSERVICESTATE.IN_PROGRESS)
						{
							this.RefreshInterval = 0;
						}
						else
						{
							this.RefreshInterval = 15;
						}

						if (this.RefreshInterval > 0)
						{
							this.Response.AppendHeader("Refresh", this.RefreshInterval.ToString(CultureInfo.InvariantCulture));
						}
					}

                this.ServiceStatusIdleLabel.Text = this.GetTranslatedText("Synchronization Idle");
                this.ServiceStatusInProgressLabel.Text = this.GetTranslatedText("Synchronization in Progress");
                this.ServiceStatusNotAcceptingLabel.Text = this.GetTranslatedText("Enterprise Server is currently not accepting synchronization requests");
                this.ServiceStatusDisabledLocallyLabel.Text = this.GetTranslatedText("Synchronization is locally disabled.  Check synchronization configuration settings");
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }
        #endregion Page Events and Overrides

		#region Control Event Handlers
		/// <summary>
		/// The synchronize button_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		public void SynchronizeButtonClick(object sender, EventArgs e)
		{
				try
				{
					this.GetSecurity();

				FMSyncServiceChannelHelper tmpHelper = new FMSyncServiceChannelHelper();
					var tmpConfig = tmpHelper.CreateChannelFactoryConfigInfo<ISynchronizationServices>();

					FMChannelFactory<ISynchronizationServices> syncServiceFactory = 
					new FMChannelFactory<ISynchronizationServices>(	tmpConfig);

					Func<ISynchronizationServices, SecurityClass, SyncSelectedSiteDO, byte[], SYNCREQUESTTYPE, bool> callback = (proxy,
																																					security,
																																					selectedSite,
																																					clientCert,
																																					requestType)
																																					=> proxy.ManuallyInitiate(security, selectedSite, clientCert, requestType);

					FMChannelHelper.MakeCall<ISynchronizationServices, bool>(syncServiceFactory, channelProxy => callback(channelProxy, this.Security, this.SelectedSyncSite, this.Request.ClientCertificate.Certificate, this.SyncRequestType ?? SYNCREQUESTTYPE.MANUAL));

					this.RefreshInterval = 15;

					this.UpdateView();

					this.Redirect(this.Request.RawUrl);
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
		}

		/// <summary>
		/// The stop synchronize button_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		/// <exception cref="Exception">
		/// </exception>
		public void StopSynchronizeButtonClick(object sender, EventArgs e)
		{
				try
				{
					this.GetSecurity();



				string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncServiceChannelHelper.BindingTypeConfigKey];

				if (string.IsNullOrEmpty(syncServiceBindingType))
				{
					throw new Exception(FMSyncServiceChannelHelper.BindingTypeConfigKey + " not found in configuration");
				}

				string syncServiceBindingConfiguration = ConfigurationManager.AppSettings[FMSyncServiceChannelHelper.BindingConfigurationConfigKey];

					if (string.IsNullOrEmpty(syncServiceBindingConfiguration))
					{
					throw new Exception(FMSyncServiceChannelHelper.BindingConfigurationConfigKey + " not found in configuration");
					}

				string syncServiceBindingEndPointAddress = ConfigurationManager.AppSettings[FMSyncServiceChannelHelper.BindingEndpointAddressConfigKey];

					if (string.IsNullOrEmpty(syncServiceBindingEndPointAddress))
					{
					throw new Exception(FMSyncServiceChannelHelper.BindingEndpointAddressConfigKey + " not found in configuration");
					}


					FMSyncServiceChannelHelper.MakeCall<ISynchronizationServices>(x => x.StopSynchronization(this.Security));
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
		}
		#endregion Control Event Handlers
	}
}