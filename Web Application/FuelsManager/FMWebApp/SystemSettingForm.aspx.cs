// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSettingForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemSettingForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for SystemSettingForm.
	/// </summary>
	public partial class SystemSettingForm : FMFormBase, IMenuDiscovery
	{
		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends upon Shared Components
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (security.HasRight(RIGHT.MODIFY_SYSTEM_SETTINGS) == false)
			{
				return null;
			}


			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ADMIN_SYSTEM_SYSTEM_SETTINGS,
						RootMenuName = "Administration",
						CategoryName = "System",
						ItemName = "System Settings",
						NavigateUrl = "SystemSettingForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					SystemSettingClass systemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(systemSettings => systemSettings.Get(this.Security));
					var configSetting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>
												(x => x.GetByKey(this.Security, ConfigurationSettingDOClass.Key_SingleSignOnMode));

					bool ssoMode = false;

					// This is so that the Login page will not try and auto login the domain user.
					if (configSetting != null
						&& string.IsNullOrEmpty(configSetting.SettingValue) == false
						&& configSetting.SettingValue == "1")
                    {
						ssoMode = true;
                    }

					this.ReportServerURLTextBox.Text = systemSetting.ReportServerUrl;

					// Instead of setting the text to the password, we add a default value to the text box
					// We then check the text of the text box against this value when the user saves the changes to see if a change was made
					// This is done to avoid having the password be visible in the page source.
					if (!string.IsNullOrEmpty(systemSetting.ReportServerPassword))
					{
						this.txtReportServerPassword.Attributes.Add("value", SystemSettingClass.MaskedPasswordText);
					}

					this.txtReportServerUserName.Text       = systemSetting.ReportServerUserName;
					this.StationMessageTimeoutTextBox.Text  = systemSetting.StationMessageTimeout.ToString();
					this.StationPromptTimeoutTextBox.Text   = systemSetting.StationPromptTimeout.ToString();
				    this.SsoModeCheckBox.Checked            = ssoMode;
				}
				else
				{
					// If the page ever posts back, we have to re-add the password the user entered or the value will be lost
					this.txtReportServerPassword.Attributes.Add("value", this.txtReportServerPassword.Text);	
				}

				if (this.Security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS) == false)
				{
					this.ConfigButton.Visible = false;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ConfigButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.Redirect("../MenuBar/FMMenuBar.aspx?target=../Config/ConfigurationSettings/ConfigurationSettingsIndex");
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
			this.ConfigButton.Command += this.ConfigButtonCommand;
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
			    var systemSetting = new SystemSettingClass
			                        {
			                            ReportServerUrl         = this.ReportServerURLTextBox.Text,
			                            ReportServerUserName    = this.txtReportServerUserName.Text,
			                            ReportServerPassword    = this.txtReportServerPassword.Text,
			                            StationMessageTimeout   = Convert.ToInt32(this.StationMessageTimeoutTextBox.Text),
			                            StationPromptTimeout    = Convert.ToInt32(this.StationPromptTimeoutTextBox.Text)
			                        };


			    FMChannelHelper.MakeCall<ISystemSettings>(systemSettings => systemSettings.Modify(this.Security, systemSetting));

				// Set the SSO mode in the configuration settings table.
				string settingValue = this.SsoModeCheckBox.Checked ? "1" : "0";
				FMChannelHelper.MakeCall<IConfigurationSettings>
												(x => x.Modify(this.Security, ConfigurationSettingDOClass.Key_SingleSignOnMode, settingValue));

				try
				{
					if (UsingLoadRack)
					{
						ILoadRackManager loadRackManager = this.GetLoadRackManager();
						loadRackManager.Modify(this.Security, typeof(SystemSettingClass), Guid.Empty);
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw socketExcept;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion
	}
}