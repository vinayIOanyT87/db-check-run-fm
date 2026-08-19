// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppointmentWebAppTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Purpose:	Used by Shared Components left tree navigation to discover
//   the QualityControl tree structure.
//   Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA,
//   2007.  This file shall not be copied or reproduced in any form
//   without the express written consent of Varec, Inc.
//   Author(s):	B. Schaal
//   Version:	1.0.0  Current version
//   Modification History:
//   Date:			By:						Reason:
//   ----------  --------------------	----------------------------------
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.AppointmentWebApp
{
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	///   Provides a vehicle for generating nodes for Quality Control menus.
	/// </summary>
	public class AppointmentWebAppTreeNav : FMFormBase, IMenuDiscovery
	{
		#region Explicit Interface Methods

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session 
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group 
		/// </param>
		/// <param name="options">
		/// Hardware key options 
		/// </param>
		/// <returns>
		/// List of menu items to be displayed 
		/// </returns>
		List<FMMenuItem> IMenuDiscovery.GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x80) != 0x80)
                    return null;
            }
            else
            {
                if (!FMChannelHelper.MakeCall<IHardwareKey, bool>(hardwareKeyChannel => hardwareKeyChannel.IsDescKey()))
                {
                    return null;
                }
            }

            var menuItems = new List<FMMenuItem>();

			if (!siteGroup && (security.HasRight(RIGHT.VIEW_APPOINTMENTS) || security.HasRight(RIGHT.MODIFY_APPOINTMENTS)))
			{
				var schedulerSummaryMenuItem = new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_SCHEDULER_SCHEDULER_SUMMARY, 
						RootMenuName = "Operations", 
						CategoryName = "Scheduler", 
						ItemName = "Scheduler Summary", 
						NavigateUrl = string.Format("../AppointmentWebApp/{0}", "AppointmentSummary.aspx?MODE=NORMAL"), 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					};

				menuItems.Add(schedulerSummaryMenuItem);

				if (security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS))
				{
					var getTestScheduleMenuItem = new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_SCHEDULER_GET_TEST_SCHEDULE,
						RootMenuName = "Operations",
						CategoryName = "Scheduler",
						ItemName = "Scheduled/Overdue Tests",
						NavigateUrl = string.Format("../AppointmentWebApp/{0}", "AppointmentSummary.aspx?MODE=GETTEST"),
						ApplyDataDictionary = ApplyDataDictionary.Apply
					};
					menuItems.Add(getTestScheduleMenuItem);
				}				
			}

			return menuItems;
		}

		#endregion
	}
}
