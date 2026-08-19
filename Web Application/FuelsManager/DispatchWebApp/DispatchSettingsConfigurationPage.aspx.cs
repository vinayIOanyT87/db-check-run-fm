// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchSettingsConfigurationPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchSettingsConfigurationPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Globalization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	///    Partial definition of the DispatchSettingsConfigurationPage class.  Provides functionality for the
	///    Dispatch Settings Configuration web page.
	/// </summary>
	public partial class DispatchSettingsConfigurationPage : FMFormBase, IEntityDiscovery
	{
		#region Explicit Interface Properties

		/// <summary>
		///    Gets a value indicating whether entity assignable.
		/// </summary>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		///    Gets the entity engine type.
		/// </summary>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IDispatchConfigurations);
			}
		}

		/// <summary>
		///    Gets the entity type.
		/// </summary>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.DISPATCH_CONFIGURATION;
			}
		}

		#endregion

		#region Explicit Interface Methods


		/// <summary>
		///    IEntityDiscovery EnumerateEntityMaps implementation
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="type">The entity assignment type</param>
		/// <returns>The list of entity to site map objects</returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			var dispatchConfigCollection = FMChannelHelper.MakeCall<IDispatchConfigurations, DispatchConfigurationCollectionClass>(
				dispatchConfigs => dispatchConfigs.Enumerate(security));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (DispatchConfigurationClass dispatchConfig in dispatchConfigCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == dispatchConfig.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != dispatchConfig.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != dispatchConfig.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(dispatchConfig);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		///    IEntityDiscovery GetIdentityGuid implementation
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The dispatch configuration ID</param>
		/// <returns>The identity Guid of the specified object</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IDispatchConfigurations, Guid>(
				dispatchConfigs => dispatchConfigs.GetIdentityGuidById(security, id));
		}

		/// <summary>
		///    IEntityDiscovery SetSiteGuid implementation
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="guid">The dispatch configuration Guid</param>
		/// <param name="siteGuid">The site Guid</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<IDispatchConfigurations>(
				dispatchConfigs =>
				{
					DispatchConfigurationClass dispatchConfig = dispatchConfigs.Get(security, guid);
					dispatchConfig.SiteGuid = siteGuid;
					dispatchConfigs.Modify(security, dispatchConfig);
				});
		}

		#endregion

		#region Methods

		/// <summary>
		///    Saves the current dispatch settings configuration to the database.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void ApplyButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				var dispatchConfig = (DispatchConfigurationClass)this.Session["DispatchConfiguration"];

				dispatchConfig.EnableServiceRequests = this.enableServiceRequestsCheckBox.Checked;

				int refreshPeriod = 0;
				bool validValue = int.TryParse(this.refreshPeriodTextBox.Text, out refreshPeriod) && refreshPeriod > 0;

				dispatchConfig.DispatchDataRefreshPeriod = validValue ?
					refreshPeriod : DispatchConfigurationClass.DefaultDataRefreshPeriod;
				this.refreshPeriodTextBox.Text = dispatchConfig.DispatchDataRefreshPeriod.ToString(CultureInfo.InvariantCulture);

				int automaticRestartDelay = 0;
				validValue = int.TryParse(this.automaticRestartDelayTextBox.Text, out automaticRestartDelay) && automaticRestartDelay > 0;

				dispatchConfig.AutomaticRestartDelay = validValue ?
					automaticRestartDelay : DispatchConfigurationClass.DefaultAutomaticRestartDelay;
				this.automaticRestartDelayTextBox.Text = dispatchConfig.AutomaticRestartDelay.ToString(CultureInfo.InvariantCulture);

				dispatchConfig.DisplayCurrentTime = this.displayCurrentTimeCheckBox.Checked;
				dispatchConfig.FuelsManagerReportURL = this.fuelsManagerReportUrlTextBox.Text;

				dispatchConfig.TabularViewDisplayMilitaryDate = this.tabularViewDisplayMilitaryDateCheckBox.Checked;
				dispatchConfig.UseArrivalTime = this.chkUseArrivalTime.Checked;
				dispatchConfig.UseStartTime = this.chkUseStartTime.Checked;
				dispatchConfig.UseStopTime = this.chkUseStopTime.Checked;
				dispatchConfig.ShowGridLines = this.ShowGridLinesCheckBox.Checked;
				dispatchConfig.StaticTimeDisplay = this.StaticTimeDisplayCheckBox.Checked;

				int pastHours;
				validValue = int.TryParse(this.operationalWindowPastHours.Text, out pastHours);
				dispatchConfig.OperationalWindowPastHours = validValue ? pastHours : DispatchConfigurationClass.DefaultOperationalWindowPastHours;
				this.operationalWindowPastHours.Text = dispatchConfig.OperationalWindowPastHours.ToString(CultureInfo.InvariantCulture);

				int futureHours;
				validValue = int.TryParse(this.operationalWindowFutureHours.Text, out futureHours);
				dispatchConfig.OperationalWindowFutureHours = validValue ? futureHours : DispatchConfigurationClass.DefaultOperationalWindowFutureHours;
				this.operationalWindowFutureHours.Text = dispatchConfig.OperationalWindowFutureHours.ToString(CultureInfo.InvariantCulture);

				dispatchConfig.FillToActualOrStandard = (FillToActualOrStandardType)this.FMRadioFillToActualStandard.SelectedIndex;

				// Add a new configuration if the current one does not exist in the database
				FMChannelHelper.MakeCall<IDispatchConfigurations>(
					dispatchConfigs =>
					{
						if (dispatchConfig.IdentityGuid == Guid.Empty)
						{
							dispatchConfig.IdentityGuid = dispatchConfigs.Add(this.Security, dispatchConfig);
						}
						else
						{
							dispatchConfigs.Modify(this.Security, dispatchConfig);
						}

						this.Session["DispatchConfiguration"] = dispatchConfigs.Get(this.Security, dispatchConfig.IdentityGuid);
					});
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Executes when the page is loaded.  Disables command buttons if security requirements are not satisfied.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					// Retrieve the current Dispatch Configuration from the database
					var dispatchConfig = new DispatchConfigurationClass();

					bool entityAssigned = false;

					FMChannelHelper.MakeCall<IDispatchConfigurations>(
						dispatchConfigs =>
						{
							Guid dispatchConfigGuid = dispatchConfigs.GetIdentityGuidBySiteIdAndAssigned(
									this.Security, this.Security.SiteGuid, DispatchConfigurationClass.DefaultId, true, out entityAssigned);

							if (dispatchConfigGuid != Guid.Empty)
							{
								dispatchConfig = dispatchConfigs.Get(this.Security, dispatchConfigGuid);
							}
						});

					if (entityAssigned || !this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.EnableControls(false);
					}

					this.enableServiceRequestsCheckBox.Checked = dispatchConfig.EnableServiceRequests;
					this.refreshPeriodTextBox.Text = dispatchConfig.DispatchDataRefreshPeriod.ToString(CultureInfo.InvariantCulture);
					this.automaticRestartDelayTextBox.Text = dispatchConfig.AutomaticRestartDelay.ToString(CultureInfo.InvariantCulture);
					this.displayCurrentTimeCheckBox.Checked = dispatchConfig.DisplayCurrentTime;
					this.fuelsManagerReportUrlTextBox.Text = dispatchConfig.FuelsManagerReportURL;
					this.tabularViewDisplayMilitaryDateCheckBox.Checked = dispatchConfig.TabularViewDisplayMilitaryDate;
					this.chkUseArrivalTime.Checked = dispatchConfig.UseArrivalTime;
					this.chkUseStartTime.Checked = dispatchConfig.UseStartTime;
					this.chkUseStopTime.Checked = dispatchConfig.UseStopTime;
					this.ShowGridLinesCheckBox.Checked = dispatchConfig.ShowGridLines;
					this.StaticTimeDisplayCheckBox.Checked = dispatchConfig.StaticTimeDisplay;
					this.operationalWindowPastHours.Text = dispatchConfig.OperationalWindowPastHours.ToString(CultureInfo.InvariantCulture);
					this.operationalWindowFutureHours.Text = dispatchConfig.OperationalWindowFutureHours.ToString(CultureInfo.InvariantCulture);
					this.FMRadioFillToActualStandard.SelectedIndex = (int)dispatchConfig.FillToActualOrStandard;

					this.Session["DispatchConfiguration"] = dispatchConfig;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Enables or disables all the data entry controls.
		/// </summary>
		/// <param name="enable">If true controls are enables otherwise they are disabled.</param>
		private void EnableControls(bool enable)
		{
			this.enableServiceRequestsCheckBox.Enabled = enable;
			this.refreshPeriodTextBox.Enabled = enable;
			this.automaticRestartDelayTextBox.Enabled = enable;
			this.displayCurrentTimeCheckBox.Enabled = enable;
			this.fuelsManagerReportUrlTextBox.Enabled = enable;
			this.tabularViewDisplayMilitaryDateCheckBox.Enabled = enable;
			this.chkUseArrivalTime.Enabled = enable;
			this.chkUseStartTime.Enabled = enable;
			this.chkUseStopTime.Enabled = enable;
			this.ShowGridLinesCheckBox.Enabled = enable;
			this.StaticTimeDisplayCheckBox.Enabled = enable;
			this.operationalWindowPastHours.Enabled = enable;
			this.operationalWindowFutureHours.Enabled = enable;
			this.applyButton.Enabled = enable;
			this.FMRadioFillToActualStandard.Enabled = enable;
		}

		#endregion
	}
}