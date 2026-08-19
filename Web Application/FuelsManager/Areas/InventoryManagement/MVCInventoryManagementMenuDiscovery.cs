namespace FuelsManager.Areas.InventoryManagement
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	public class MvcConfigMenuDiscovery : IMenuDiscovery
	{
		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems( SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
		{
			if ((word1 & 0x80) != 0x80)
				return null;

			var items = new List<FMMenuItem>();

			// TODO: Add security rights checks for menu items in this section.
			if (((word2 & 0x02) == 0x02) && (security.HasRight(RIGHT.VIEW_POINT_TEMPLATES)
			|| security.HasRight(RIGHT.VIEW_POINT_TEMPLATES)))
			{
				items.Add(
					new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_INVMGR_POINT_TEMPLATES,
						RootMenuName = "Configuration",
						CategoryName = "Inventory Management",
						ItemName = "Point Templates",
						NavigateUrl =
							"../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointTemplates/PointTemplatesIndex",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});
			}

			if (security.HasRight(RIGHT.VIEW_POINTS) || security.HasRight(RIGHT.ENABLE_POINTS) || security.HasRight(RIGHT.DISABLE_POINTS))
				items.Add(
				new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_INVMGR_POINTS,
					RootMenuName = "Configuration",
					CategoryName = "Inventory Management",
					ItemName = "Points",
					NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointsSummary/PointsSummaryView",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				});
				if (security.HasRight(RIGHT.VIEW_FCEE_DATA))
				{
					 items.Add(
					 new FMMenuItem
					 {
						  MenuItemType = FMMenuItemType.OPERATIONS_INVENTORY_MANAGEMENT_FCEE_MESSAGES,
						  RootMenuName = "Operations",
						  CategoryName = "Inventory Management",
						  ItemName = "FCEE Messages Summary",
						  NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../FCEE/FCEEMessagesSummary/FCEEMessagesSummaryView",
						  ApplyDataDictionary = ApplyDataDictionary.Apply
					 });
                items.Add(
					 new FMMenuItem
					 {
						  MenuItemType = FMMenuItemType.CONFIG_INVMGR_FCEE_MAPPINGS,
						  RootMenuName = "Configuration",
						  CategoryName = "Inventory Management",
						  ItemName = "FCEE Mappings",
						  NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../FCEE/FCEEMapping/FCEEMappingView",
						  ApplyDataDictionary = ApplyDataDictionary.Apply
					 });
                items.Add(
					 new FMMenuItem
					 {
						  MenuItemType = FMMenuItemType.CONFIG_INVMGR_FCE_DEVICE_SUMMARY,
						  RootMenuName = "Configuration",
						  CategoryName = "Inventory Management",
						  ItemName = "FCE Devices",
						  NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../FCEE/FCEDeviceSummary/FCEDeviceSummaryView",
						  ApplyDataDictionary = ApplyDataDictionary.Apply
					 });
					 if (security.HasRight(RIGHT.ROLLING_STOCK_IMPORT)){
						  items.Add(
						  new FMMenuItem
						  {
								MenuItemType = FMMenuItemType.CONFIG_INVMGR_ROLLING_STOCK_IMPORT,
								RootMenuName = "Operations",
								CategoryName = "Import/Export",
								ItemName = "Rolling Stock Import",
								NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/OfflineRollingStockImport/OfflineRollingStockImportView",
								ApplyDataDictionary = ApplyDataDictionary.Apply
						  }); }
            }
            if (((word2 & 0x02) == 0x02) && security.HasRight(RIGHT.VIEW_MODULE_LIBRARY))
				items.Add(
					  new FMMenuItem
					  {
						  MenuItemType = FMMenuItemType.CONFIG_INVMGR_MODULELIBRARY,
						  RootMenuName = "Configuration",
						  CategoryName = "Inventory Management",
						  ItemName = "Module Library",
						  NavigateUrl =
								 "../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/ModuleLibrary/ModuleLibraryView",
						  ApplyDataDictionary = ApplyDataDictionary.Apply
					  });

			if (security.HasRight(RIGHT.ACCESS_TAG_VIEWER))
				items.Add(
				new FMMenuItem
				{
					MenuItemType = FMMenuItemType.OPERATIONS_INVENTORY_MANAGEMENT_TAG_VIEWER,
					RootMenuName = "Operations",
					CategoryName = "Inventory Management",
					ItemName = "Tag Viewer",
					NavigateUrl =
						"../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/TagViewer/TagViewer",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				});


			if (security.HasRight(RIGHT.ACCESS_DRAW))
            items.Add(
                 new FMMenuItem
                {
                    MenuItemType = FMMenuItemType.CONFIG_INVMGR_DRAW_PROTO,
                    RootMenuName = "Configuration",
                    CategoryName = "Inventory Management",
                    ItemName = "Draw",
                    NavigateUrl =
                         "../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Draw/DrawIndex",
                    ApplyDataDictionary = ApplyDataDictionary.Apply
                });

			if (security.HasRight(RIGHT.VIEW_OPERATE_ONLY) 
				|| security.HasRight(RIGHT.OPERATE_VIEW_POINTS) 
				|| security.HasRight(RIGHT.OPERATE_VIEW_GRAPHICS)
				|| security.HasRight(RIGHT.OPERATE_USE_POINT_CALCULATOR) 
				|| security.HasRight(RIGHT.OPERATE_VIEW_TRENDS)
                || security.HasRight(RIGHT.OPERATE_VIEW_POINT_GROUPS)
                || security.HasRight(RIGHT.OPERATE_VIEW_POINT_HISTORY)
				|| security.HasRight(RIGHT.OPERATE_VIEW_IM_REPORTS)
                || security.HasRight(RIGHT.OPERATE_VIEW_ALARM_SUMMARY)
				|| security.HasRight(RIGHT.OPERATE_VIEW_ALARM_HISTORY))
			{
				items.Add(new FMMenuItem
							{
								MenuItemType = FMMenuItemType.OPERATIONS_INVENTORY_MANAGEMENT_OPERATE,
								RootMenuName = "Operations",
								CategoryName = "Inventory Management",
								ItemName = "Operate",
								NavigateUrl =
									"../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndex",
								ApplyDataDictionary = ApplyDataDictionary.Apply
            } );
			}

			if (security.HasRight(RIGHT.VIEW_PICTURE_SUMMARY))
				items.Add(
				new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_INVMGR_PICTURESUMMARY,
					RootMenuName = "Configuration",
					CategoryName = "Inventory Management",
					ItemName = "Picture Summary",
					NavigateUrl =
						"../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Picture/PictureSummary",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				} );

			var statistiscPageEnabled = AppSettingsHelper.GetKeyValue(
				"ShowIventoryManagementStatisticsPage",
				defaultValue: false);

			if (statistiscPageEnabled)
			{
				items.Add(
					new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_INVENTORY_MANAGEMENT_STATISTICS,
						RootMenuName = "Operations",
						CategoryName = "Inventory Management",
						ItemName = "Point Service Statistics",
						NavigateUrl =
							"../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Statistics/StatisticsSummary",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});
			}

			if (security.HasRight(RIGHT.VIEW_POINT_ACCESS_GROUP))
				items.Add(
				new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_INVMGR_POINT_ACCESS_CONFIGURATION,
					RootMenuName = "Configuration",
					CategoryName = "Inventory Management",
					ItemName = "Point Access",
					NavigateUrl =
						"../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointAccess/PointAccess",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				});

            if (security.HasRight(RIGHT.VIEW_OPERATE_STATISTICS))
                items.Add(
                new FMMenuItem
                {
                    MenuItemType = FMMenuItemType.OPERATIONS_INVENTORY_MANAGEMENT_OPERATE_STATISTICS,
                    RootMenuName = "Operations",
                    CategoryName = "Inventory Management",
                    ItemName = "Operate Statistics",
                    NavigateUrl =
                        "../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/OperateStatistics/Summary",
                    ApplyDataDictionary = ApplyDataDictionary.Apply
                });

            if (security.HasRight(RIGHT.IMPORT_ENTITIES))
            {
                items.Add(
                   new FMMenuItem
                   {
                       MenuItemType = FMMenuItemType.CONFIG_IMPORT_EXPORT_STRAP_TABLE_FILE_IMPORT,
                       RootMenuName = "Configuration",
                       CategoryName = "Import/Export",
                       ItemName = "Strap Table File Import",
                       NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/StrapTableFileImport/StrapTableFileImportView",
                       ApplyDataDictionary = ApplyDataDictionary.Apply
                   });


                foreach (var item in items)
				{
					if (item.NavigateUrl.Contains("&title=") == false)
					{
						//item.Description = item.ItemName;
						item.NavigateUrl += string.Format("&title={0}", item.ItemName);
               }


				
				}
			}

            return items;
		}
	}
}
