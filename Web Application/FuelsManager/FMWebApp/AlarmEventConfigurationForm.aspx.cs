/******************************************************************************
	FILE NAME:		AlarmEventConfigurationForm.aspx.cs
	PURPOSE:		Implementation of AlarmEventConfigurationForm

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:				Reason:
		----------	--------------	-------------------------------------------
		2007-02-00	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
		2007-03-12	Richard Panachida	Corrected the problem that the tab was not highlighted after
										being disabled.
*******************************************************************************/

using System;
using System.Collections.Generic;
using AjaxControlToolkit;
using FMBusinessObjects.DataObjects;
using FMCore;
using FMWebApp;

namespace FuelsManager.FMWebApp
{
    /// <summary>
	/// Summary description for AlarmEventConfigurationForm.
	/// </summary>
	public partial class AlarmEventConfigurationForm : FMFormBase, IMenuDiscovery
	{
		private const string ALARM_EVENT_CONFIG_TAB_SELECTION = "AlarmEventConfigTabSelection";

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_SYSTEM_ALARM_AND_EVENTS,
					RootMenuName = "Configuration",
					CategoryName = "System",
					ItemName = "Alarms & Events",
					NavigateUrl = "AlarmEventConfigurationForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					// Apply the Data dictionary to the tab headers
					foreach (TabPanel tab in this.tcAlarmEventConfig.Tabs)
					{
						tab.HeaderText = this.GetTranslatedText(tab.HeaderText);
					}

					if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("EmailGroups")) == false)
					{
						this.tcAlarmEventConfig.ActiveTabIndex = 2;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will either enable or disable controls.  It is called by
		/// the individual tabs associated to the company form.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls(bool enable)
		{
			this.tcAlarmEventConfig.HeaderEnabled = enable;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
