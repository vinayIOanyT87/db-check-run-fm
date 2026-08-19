namespace FuelsManager.FMEntityImportWebApp
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public class EntityImportTreeNav : IMenuDiscovery
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
			var menuItems = new List<FMMenuItem>();
            bool addMenuOptions = false;

            if (useNewLicenseKey == 1)
            {
                addMenuOptions = true;
            }
            else
            {
                if (this.HasHardwareKey(options))
                {
                    addMenuOptions = true;
                }
            }


            if (addMenuOptions == true && this.HasImportAndOrExportEntitiesPermission(security))
			{
				const string EntityImportUrl = "../FMEntityImportWebApp/";

				if (this.HasImportEntitiesPermission(security))
				{
					var importMenuItem = new FMMenuItem
						{
							MenuItemType = FMMenuItemType.CONFIG_IMPORT_EXPORT_ENTITY_IMPORT,
							RootMenuName = "Configuration",
							CategoryName = "Import/Export",
							ItemName = "Entity Import",
                            NavigateUrl = EntityImportUrl + "EntityImportForm.aspx",
                            ApplyDataDictionary = ApplyDataDictionary.Apply
						};
					menuItems.Add(importMenuItem);
				}

				if (this.HasExportEntitiesPermission(security))
				{
					var exportMenuItem = new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_IMPORT_EXPORT_ENTITY_EXPORT,
						RootMenuName = "Configuration",
						CategoryName = "Import/Export",
						ItemName = "Entity Export",
                        NavigateUrl = EntityImportUrl + "EntityExportForm.aspx",
                        ApplyDataDictionary = ApplyDataDictionary.Apply
					};
					menuItems.Add(exportMenuItem);
				}
			}

			return menuItems;
		}

		/// <summary>
		/// This method will determine if the user has execute import/export permissions. If so,
		/// the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public bool HasImportExportExecutePermission(SecurityClass security)
		{
			return security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT);
		}

		public bool HasImportAndOrExportEntitiesPermission(SecurityClass security)
		{
			return security.HasRight(RIGHT.EXPORT_ENTITIES) ||
				security.HasRight(RIGHT.IMPORT_ENTITIES);
		}

		public bool HasImportEntitiesPermission(SecurityClass security)
		{
			return security.HasRight(RIGHT.IMPORT_ENTITIES);
		}

		/// <returns></returns>
		public bool HasExportEntitiesPermission(SecurityClass security)
		{
			return security.HasRight(RIGHT.EXPORT_ENTITIES);
		}

		/// <summary>
		/// This method will return true if there is a valid hardware key for Shared Components Configuration.
		/// Otherwise, it will return false. The key is located in the lower word of a 32 bit word 
		/// and the value is 0x4000.
		/// </summary>
		/// <returns></returns>
		public bool HasHardwareKey(uint Options)
		{
			bool hasKey = true;

			if ((Options & 0x4000) == 0)
			{
				hasKey = false;
			}

			return hasKey;
		}
	}
}

