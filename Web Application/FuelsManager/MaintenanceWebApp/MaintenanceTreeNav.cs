
namespace FuelsManager.MaintenanceWebApp
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public class MaintenanceTreeNav : IMenuDiscovery 
	{
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
                if ((word2 & 0x10) != 0x10)
                    return null;
            }
            else
            {
                // check if this option is set in the hardware key
                if ((options & 0x4000000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)  ||
				 security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)  ||
				 security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) )
			{
				if (security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) )
				{
					items.Add(new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_MAINTENANCE_ADD_MAINTENANCE_RECORD,
						RootMenuName = "Operations",
						CategoryName = "Maintenance",
						ItemName = "Add Maintenance Record",
						NavigateUrl = "../MaintenanceWebApp/MaintenanceAddRecordForm.aspx?MODE=ADD",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});
				}

				items.Add(new FMMenuItem
				{
					MenuItemType = FMMenuItemType.OPERATIONS_MAINTENANCE_MAINTENANCE_LOG,
					RootMenuName = "Operations",
					CategoryName = "Maintenance",
					ItemName = "Maintenance Log",
					NavigateUrl = "../MaintenanceWebApp/MaintenanceLogForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				});
			}

			return items;
		}
	}
}
