namespace FuelsManager.Areas.AssetTrackingArea
{
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public class MVCAssetTrackingMenuDiscovery : IMenuDiscovery
	{
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
			var items = new List<FMMenuItem>();

			List<FMMenuItem> mapViewMenuItems		= this.GetViewMapMenu(security);
			FMMenuItem mapConfigurationMenuItem		= this.GetConfigurationMapMenu(security);
			FMMenuItem iconConfigurationMenuItem	= this.GetConfigurationIconMenu(security);
			FMMenuItem deviceConfigSummaryItem		= this.GetConfigurationDeviceMenu(security);

			if (mapViewMenuItems != null && mapViewMenuItems.Count > 0)
			{
				foreach (FMMenuItem menuItem in mapViewMenuItems)
				{
					items.Add(menuItem);
				}
			}

			if (iconConfigurationMenuItem != null)
			{
				items.Add(iconConfigurationMenuItem);
			}

			if (mapConfigurationMenuItem != null)
			{
				items.Add(mapConfigurationMenuItem);
			}

			if (deviceConfigSummaryItem != null)
			{
				items.Add(deviceConfigSummaryItem);
			}

			return items;
		}

		/// <summary>
		/// This method will return the FM Menu Item for viewing maps.
		/// </summary>
		/// <param name="security">The security object for checking rights.</param>
		/// <returns>Returns a list of menu item for viewing maps.</returns>
		private List<FMMenuItem> GetViewMapMenu(SecurityClass security)
		{
			// No need to check Modify rights in addition to this because the
			// HasRight() call will check the Modify right as an implied View right.
			if (security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				return null;
			}
			bool isDefense = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
         if (isDefense)
         {
            return null;
         }
         var mapConfigurationList =
							FMChannelHelper.MakeCall<IAssetTrackingMapConfigurations, List<AssetTrackingMapConfigurationClass>>(
																								x => x.Enumerate(security));

			if (mapConfigurationList == null || mapConfigurationList.Count < 1)
			{
				return null;
			}

			var mapMenuItemList = new List<FMMenuItem>();

			foreach (AssetTrackingMapConfigurationClass mapConfiguration in mapConfigurationList)
			{
				if (mapConfiguration.Active == false)
				{
					continue;
				}

				var mapMenuItem = new FMMenuItem
				                  {
					                  MenuItemType			= FMMenuItemType.MAP_MAPS,
					                  RootMenuName			= "Map",
					                  CategoryName			= "Maps",
					                  ItemName				= mapConfiguration.MapName,
									  NavigateUrl			= "../MenuBar/FMMenuBar.aspx?target=../AssetTrackingArea/AssetMaps/MapBase&MapName=" 
																+ mapConfiguration.MapName,
									  DynamicMenuItemGuid	= mapConfiguration.AssetTrackingMapConfigurationGuid,
					                  ApplyDataDictionary	= ApplyDataDictionary.Apply
				                  };

				mapMenuItemList.Add(mapMenuItem);
			}

			return mapMenuItemList;
		}

		/// <summary>
		/// This method will return the FM Menu Item for map configuration.
		/// </summary>
		/// <param name="security">The security object for checking rights.</param>
		/// <returns>Returns the menu item for map configuration.</returns>
		private FMMenuItem GetConfigurationMapMenu(SecurityClass security)
		{
         // No need to check Modify rights in addition to this because the
         // HasRight() call will check the Modify right as an implied View right.
         if (security.HasRight(RIGHT.VIEW_MAP_CONFIGURATION) == false
				&& security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION) == false)
			{
				return null;
			}
			bool isDefense = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
			if (isDefense)
			{
				return null;
			}
			return new FMMenuItem
			{
				MenuItemType	= FMMenuItemType.MAP_CONFIGURATION,
				RootMenuName	= "Map",
				CategoryName	= "Configuration",
				ItemName		= "Maps Configuration",
				NavigateUrl		= "../MenuBar/FMMenuBar.aspx?target=../AssetTrackingArea/AssetMapConfigurationSummary/MapConfigurationSummary",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			};
		}

		/// <summary>
		/// This method will return the FM Menu Item for map configuration.
		/// </summary>
		/// <param name="security">The security object for checking rights.</param>
		/// <returns>Returns the menu item for map configuration.</returns>
		private FMMenuItem GetConfigurationIconMenu(SecurityClass security)
		{
			// No need to check Modify rights in addition to this because the
			// HasRight() call will check the Modify right as an implied View right.
			if (security.HasRight(RIGHT.VIEW_ICON_CONFIGURATION) == false
				&& security.HasRight(RIGHT.MODIFY_ICON_CONFIGURATION) == false)
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.ICON_CONFIGURATION,
				RootMenuName = "Map",
				CategoryName = "Configuration",
				ItemName = "Icon Configuration",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AssetTrackingArea/AssetIconConfiguration/IconConfiguration",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			};
		}

		/// <summary>
		/// This method will return the FM Menu Item for asset tracking device configuration.
		/// </summary>
		/// <param name="security">The security object for checking rights.</param>
		/// <returns>Returns the menu item for device configuration.</returns>
		private FMMenuItem GetConfigurationDeviceMenu(SecurityClass security)
		{
			const string NavigationUrlToAssetTrackingDeviceSummary	= 
					"../MenuBar/FMMenuBar.aspx?target=../AssetTrackingArea/AssetDeviceConfigurationSummary/DeviceConfigurationSummary";

			if (security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES) == false
			    && security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) == false)
			{
				return null;
			}

			return new FMMenuItem 
							{
									MenuItemType		= FMMenuItemType.MAP_ASSET_TRACKING_DEVICE_CONFIG,
									RootMenuName		= "Assets",
									CategoryName		= "Equipment",
									ItemName			= "Asset Tracking Devices",
									NavigateUrl			= NavigationUrlToAssetTrackingDeviceSummary,
									ApplyDataDictionary = ApplyDataDictionary.Apply
							};
		}
	}
}