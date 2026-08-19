// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMMenuEngine.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   A singleton class (all members are static) that performs various processing tasks in support
//   of the FMMenuBar control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    /// <summary>
    ///   A singleton class (all members are static) that performs various processing tasks in support
    ///   of the FMMenuBar control.
    /// </summary>
    public class FMMenuEngine
	{
		#region Constants and Fields

		/// <summary>
		///   The name of a Session variable to hold permissions information about Transaction Aliases
		/// </summary>
		public const string SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION = "FMMenuEngine.AliasCollection";

		/// <summary>
		///   The maximum number of menu items to display in the Recent menu
		/// </summary>
		private const int MaxNumRecentItems = 12;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Add most recently clicked menu item to Favorites list
		/// </summary>
		/// <param name="security">
		/// Security object 
		/// </param>
		/// <param name="menuData">
		/// Menu Data object to modify 
		/// </param>
		/// <returns>
		/// New menu item that was created 
		/// </returns>
		public static FMMenuItem AddToFavoritesMenu(SecurityClass security, FMMenuData menuData)
		{
			FMMenuItem favoriteMenuItem = null;

			if (menuData.CurrentMenuItem != null)
			{
				if (menuData.CurrentMenuItem.MenuItemType == FMMenuItemType.QUICK_LINKS_CONFIG_QUICK_LINKS)
				{
					// Adding this as a favorite doesn't make a lot of sense, and messes up other stuff
					throw new ApplicationException("Quick Links Configuration cannot be added as a Favorite.");
				}

				if (
					menuData.FavoritesMenuItems.Find(
						x =>
						x.MenuItemType == menuData.CurrentMenuItem.MenuItemType
						&& x.DynamicMenuItemGuid == menuData.CurrentMenuItem.DynamicMenuItemGuid) != null)
				{
					throw new ApplicationException("Favorite already exists");
				}

				// Make a copy of the menu item object. Unlike the Recent list, we need to create
				// a new one, not just have a reference
				favoriteMenuItem = (FMMenuItem)menuData.CurrentMenuItem.Clone();

				// Allow two spaces for (Add) and (Config)
				menuData.FavoritesMenuItems.Insert(menuData.FavoritesMenuItems.Count - 2, favoriteMenuItem);

				// Create data object for DB storage
				var favorite = new MenuFavoriteClass { UserGuid = security.UserGuid, IsQuickLink = false };

				if (menuData.Favorites.Count == 0)
				{
					favorite.DisplayOrder = 1;
				}
				else
				{
					favorite.DisplayOrder = menuData.Favorites.Last().DisplayOrder + 1;
				}

				favorite.MenuItemType = favoriteMenuItem.MenuItemType;
				favorite.DynamicMenuItemGuid = favoriteMenuItem.DynamicMenuItemGuid;

				// Write to database
				FMChannelHelper.MakeCall<IMenuFavorites>(favoritesChannel => favoritesChannel.Add(security, favorite));
			}

			return favoriteMenuItem;
		}

		/// <summary>
		/// Add most recently clicked menu item to Quick Links bar
		/// </summary>
		/// <param name="security">
		/// Security object 
		/// </param>
		/// <param name="menuData">
		/// Menu Data object to modify 
		/// </param>
		/// <returns>
		/// New menu item that was created 
		/// </returns>
		public static FMMenuItem AddToQuickLinksMenu(SecurityClass security, FMMenuData menuData)
		{
			FMMenuItem quickLinkMenuItem = null;

			if (menuData.CurrentMenuItem != null)
			{
				if (menuData.CurrentMenuItem.MenuItemType == FMMenuItemType.MY_MENU_CONFIG_FAVORITES)
				{
					// Adding this doesn't make a lot of sense, and messes up other stuff
					throw new ApplicationException("Favorites Configuration cannot be added as a Quick Link.");
				}

				if (
					menuData.QuickLinksMenuItems.Find(
						x =>
						x.MenuItemType == menuData.CurrentMenuItem.MenuItemType
						&& x.DynamicMenuItemGuid == menuData.CurrentMenuItem.DynamicMenuItemGuid) != null)
				{
					throw new ApplicationException("Quick Link already exists");
				}

				// Make a copy of the menu item object. Unlike the Recent list, we need to create
				// a new one, not just have a reference
				quickLinkMenuItem = (FMMenuItem)menuData.CurrentMenuItem.Clone();

				// Allow space for Add button
				menuData.QuickLinksMenuItems.Insert(menuData.QuickLinksMenuItems.Count, quickLinkMenuItem);

				// Create data object for DB storage
				var quickLink = new MenuFavoriteClass
					{
						UserGuid = security.UserGuid, 
						IsQuickLink = true
					};

				if (menuData.QuickLinks.Count == 0)
				{
					quickLink.DisplayOrder = 1;
				}
				else
				{
					quickLink.DisplayOrder = menuData.QuickLinks.Last().DisplayOrder + 1;
				}

				quickLink.MenuItemType = quickLinkMenuItem.MenuItemType;
				quickLink.DynamicMenuItemGuid = quickLinkMenuItem.DynamicMenuItemGuid;

				// Write to database
				FMChannelHelper.MakeCall<IMenuFavorites>(favoritesChannel => favoritesChannel.Add(security, quickLink));
			}

			return quickLinkMenuItem;
		}

		/// <summary>
		/// Adds the current menu item that was clicked to the Recent list
		/// </summary>
		/// <param name="menuData">
		/// Menu Data object to modify 
		/// </param>
		/// <param name="menuItemType">
		/// The enuemrated menu item type.
		/// </param>
		/// <param name="dynamicMenuItemGuid">
		/// identifier of menu item to add 
		/// </param>
		public static void AddToRecentMenu(FMMenuData menuData, FMMenuItemType menuItemType, Guid dynamicMenuItemGuid)
		{
			if (menuData == null) 
			{  
				return; 
			}

			// Find menu item and store in class variable of FMMenuData
			menuData.CurrentMenuItem = menuData.GetMenuItem(menuItemType, dynamicMenuItemGuid);

			if (menuData.CurrentMenuItem == null)
			{
				throw new ApplicationException("Unable to find current menu item to add to Recent List");
			}

			// Remove old Recent entry before putting new one at top
			if (menuData.RecentMenuItems.Contains(menuData.CurrentMenuItem))
			{
				menuData.RecentMenuItems.Remove(menuData.CurrentMenuItem);
			}

			menuData.RecentMenuItems.Insert(0, menuData.CurrentMenuItem);

			// Limit Recent list
			if (menuData.RecentMenuItems.Count > MaxNumRecentItems)
			{
				menuData.RecentMenuItems.RemoveAt(menuData.RecentMenuItems.Count - 1);
			}
		}

		/// <summary>
		/// Loads the data that is needed for menus and Quick Links from classes' implementation of 
		///   IMenuDiscovery, from DefaultMenuData.xml, and from the database for Favorites and
		///   QuickLinks.
		/// </summary>
		/// <param name="security">
		/// Session security object 
		/// </param>
		/// <param name="isSiteGroup">
		/// Whether the currently selected site is a site group 
		/// </param>
		/// <param name="useDataDictionaryToSortItems">
		/// Whether to use data dictionary names to sort menu items 
		/// </param>
		/// <param name="exceptions">
		/// Exceptions that occurred while processing 
		/// </param>
		/// <returns>
		/// New FMMenuData object 
		/// </returns>
		//[DebuggerHidden]
		// Added DebuggerHidden attribute so this method does not constantly interrupt a debugging session
		public static FMMenuData LoadMenuData(
			SecurityClass security, 
			bool isSiteGroup, 
			bool useDataDictionaryToSortItems, 
			out List<KeyValuePair<string, Exception>> exceptions)
		{
			// Some exceptions are not fatal to the method, therefore, we may be sending back of list
			// of exceptions to report
			exceptions = new List<KeyValuePair<string, Exception>>();

			try
			{
				// we need to add a couple of cells for the menu loading calls
				// this is due to the new software license key format and the company not wanting to convert over completely to it for luzern
				uint options = 0;
				ushort word1 = 0;
				ushort word2 = 0;
                ushort useNewLicenseKey = 0;

				FMChannelHelper.MakeCall<IHardwareKey>(hardwareKeyChannel => { options = hardwareKeyChannel.GetOptionsCell(); });
				FMChannelHelper.MakeCall<IHardwareKey>(hardwareKeyChannel => { word1 = hardwareKeyChannel.GetWord1ValueLIN(); });
				FMChannelHelper.MakeCall<IHardwareKey>(hardwareKeyChannel => { word2 = hardwareKeyChannel.GetWord2ValueLIN(); });
                FMChannelHelper.MakeCall<IHardwareKey>(hardwareKeyChannel => { useNewLicenseKey = hardwareKeyChannel.GetUseNewLicenseFile(); });

                // Get the list of assemblies to probe for IMenuDiscovery implementations
                string discoveryAssem =
					FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_IDiscoveryAssemblies));

				if (string.IsNullOrEmpty(discoveryAssem))
				{
					return null;
				}

				// Parse the list of assemblies
				char[] separator = { ';' };
				string[] discoveryAssemList = discoveryAssem.Split(separator, StringSplitOptions.RemoveEmptyEntries);

				// Load default menu information, which includes the list of root menu items
				var menuDataSerializer = new XmlSerializer(typeof(FMMenuData));
				const string FileName = "FuelsManager.DefaultMenuData.xml";
				Assembly assembly = Assembly.GetExecutingAssembly();
				Stream stream = assembly.GetManifestResourceStream(FileName);
				var menuData = (FMMenuData)menuDataSerializer.Deserialize(stream);

				// Go through all the assemblies
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				foreach (string assemblyName in discoveryAssemList)
				{
					try
					{
						Assembly dll = null;

						if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
						{
							try
							{
								dll = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
							}
							catch
							{
								try
								{
									dll = Assembly.Load(assemblyName);
								}
								catch (Exception ex)
								{
									string message = "Assembly Load Error in Menu Load. " + ex.Message;
									FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
									continue;
								}
							}

							if (dll != null)
								AssemblyDictionary.Add(assemblyName.ToLower(), dll);
						}
						else
						{
							dll = AssemblyDictionary.Get(assemblyName.ToLower());
						}

						if (dll == null)
						{
							exceptions.Add(
								new KeyValuePair<string, Exception>(string.Empty, new Exception("Unable to load assembly : " + assemblyName)));
						}

						Type[] types;

						try
						{
							types = dll.GetTypes();
						}
						catch (ReflectionTypeLoadException ex)
						{
							string message = "Assembly Type Load Error in Menu Load. " + ex.Message;
							FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
							continue;
						}

						// For each type that implements IMenuDiscovery, call GetMenuItems() on it
						foreach (Type module in types)
						{
							Type discoveryInterface = module.GetInterface("IMenuDiscovery");

							if (discoveryInterface != null)
							{
								object engine = Activator.CreateInstance(module);
								var discovery = engine as IMenuDiscovery;

								if (discovery != null)
								{
									List<FMMenuItem> menuItems = discovery.GetMenuItems(security, isSiteGroup, word1, word2, useNewLicenseKey, options);

									if (menuItems != null && menuItems.Count > 0)
									{
										foreach (FMMenuItem menuItem in menuItems)
										{
											if (menuItem.ApplyDataDictionary == ApplyDataDictionary.Apply)
											{
												menuItem.DictionaryName = GetDataDictionaryValueByKey(
													security.SiteGuid,
													(string.IsNullOrWhiteSpace(menuItem.DataDictGroupPrefix)
															? string.Empty
															: menuItem.DataDictGroupPrefix + "|") + menuItem.ItemName);
											}
                                 if (menuItem.NavigateUrl.Contains("?target=") && string.IsNullOrWhiteSpace(menuItem.Description) == false)
                                 {
												menuData.TranslatedHelpUrl += "?title=" + menuItem.Description;
                     
											}

                                 menuData.AddMenuItem(menuItem);
										}
									}
								}
							}
						}
					}
					catch (Exception except)
					{
						exceptions.Add(new KeyValuePair<string, Exception>(assemblyName, except));
					}
				}

				menuData.RemoveEmptyMenus();
				menuData.SortMenuItems(useDataDictionaryToSortItems);

				// Apply Data Dictionary to root menu names and category names
				foreach (FMMenuRootItem rootItem in menuData.MenuRootItems)
				{
					rootItem.DictionaryName = GetDataDictionaryValueByKey(security.SiteGuid, rootItem.RootItemName);

					foreach (FMMenuCategory cat in rootItem.MenuCategories)
					{
						cat.DictionaryName = GetDataDictionaryValueByKey(security.SiteGuid, cat.CategoryName);
					}
				}

				// Load Favorites and Quick Links from database
				LoadFavorites(security, menuData);

				// Apply Data Dictionary to root menu names and category names
				FMChannelHelper.MakeCall<IDataDictionariesClass>(
					dataDictionariesChannel =>
					{
						foreach (FMMenuItem menuItem in menuData.FavoritesMenuItems)
						{
							menuItem.DictionaryName = dataDictionariesChannel.Get(security.SiteGuid, menuItem.ItemName);
						}
					});

				// Load info on Help location
				LoadHelpURLs(security, menuData);

				menuData.AttachCSRFToken(security.CSRFToken);

				return menuData;
			}
			catch (Exception ex)
			{
				if (exceptions == null)
				{
					exceptions = new List<KeyValuePair<string, Exception>>();
				}

				exceptions.Add(new KeyValuePair<string, Exception>(string.Empty, ex));

				return null;
			}
		}

		/// <summary>
		/// Refreshes Menu Data object by reloading everything, but preserving the Recent list
		/// </summary>
		/// <param name="oldMenuData">
		/// Old Menu Data object 
		/// </param>
		/// <param name="security">
		/// Security object 
		/// </param>
		/// <param name="isSiteGroup">
		/// Whether current site is a site group 
		/// </param>
		/// <param name="useDataDictionaryToSortItems">
		/// Whether to use data dictionary names to sort menu items 
		/// </param>
		/// <param name="exceptions">
		/// Exceptions that occurred while processing 
		/// </param>
		/// <returns>
		/// New Menu Data object 
		/// </returns>
		public static FMMenuData RefreshMenuData(
			FMMenuData oldMenuData, 
			SecurityClass security, 
			bool isSiteGroup, 
			bool useDataDictionaryToSortItems, 
			out List<KeyValuePair<string, Exception>> exceptions)
		{
			// Load it all from scratch
			FMMenuData newMenuData = LoadMenuData(security, isSiteGroup, useDataDictionaryToSortItems, out exceptions);

			// Now, add to the Recent list
			if (newMenuData != null)
			{
				foreach (FMMenuItem menuItem in oldMenuData.RecentMenuItems)
				{
                    if (menuItem != null)
                    {
                        var newMenuItem = newMenuData.GetMenuItem(menuItem.MenuItemType, menuItem.DynamicMenuItemGuid);
                        if (newMenuData != null)
                        {
                            newMenuData.RecentMenuItems.Add(newMenuItem);
                        }
                    }
				}
			}

			LoadHelpURLs(security, newMenuData);

			return newMenuData;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Loads Favorites and Quick Links from the database
		/// </summary>
		/// <param name="security">FuelsManager security object</param>
		/// <param name="menuData">Menu data objec to load.</param>
		private static void LoadFavorites(SecurityClass security, FMMenuData menuData)
		{
			MenuFavoriteCollectionClass favorites =
				FMChannelHelper.MakeCall<IMenuFavorites, MenuFavoriteCollectionClass>(
					favoritesChannel => favoritesChannel.EnumerateByUser(security, security.UserGuid));

			menuData.Favorites.Clear();
			menuData.QuickLinks.Clear();
			menuData.QuickLinksMenuItems.Clear();

			menuData.QuickLinksMenuItems.Add(
				new FMMenuItem
					{
						ApplyDataDictionary = ApplyDataDictionary.Apply,
						ItemName = "Add Quick Link",
						MenuItemType = FMMenuItemType.QUICK_LINKS_ADD_QUICK_LINK,
						NavigateUrl = "#",
						SortOrder = -1
					});

			foreach (MenuFavoriteClass favorite in favorites)
			{
				FMMenuItem origMenuItem = menuData.GetMenuItem(favorite.MenuItemType, favorite.DynamicMenuItemGuid);

				// The menu item might not exist, for example, if the user's permissions changed
				if (origMenuItem != null)
				{
					var favoriteMenuItem = (FMMenuItem)origMenuItem.Clone();
					favoriteMenuItem.SortOrder = favorite.DisplayOrder;
					if (!string.IsNullOrEmpty(favorite.CustomName))
					{
						favoriteMenuItem.ItemName = favorite.CustomName;
						favoriteMenuItem.DictionaryName = favorite.CustomName;
					}

					if (favorite.MenuItemType != FMMenuItemType.QUICK_LINKS_CONFIG_QUICK_LINKS)
					{
						if (favorite.IsQuickLink)
						{
							menuData.QuickLinks.Add(favorite);
							menuData.QuickLinksMenuItems.Add(favoriteMenuItem);
						}
						else
						{
							menuData.Favorites.Add(favorite);
							menuData.FavoritesMenuItems.Add(favoriteMenuItem);
						}
					}
				}
			}

			menuData.FavoritesMenuItems.Add(
			new FMMenuItem
			{
				ApplyDataDictionary = ApplyDataDictionary.Apply,
				ItemName = "(Add Favorite)",
				MenuItemType = FMMenuItemType.MY_MENU_ADD_FAVORITE,
				NavigateUrl = "#",
				SortOrder = 999
			});

			menuData.FavoritesMenuItems.Add(
				new FMMenuItem
				{
					ApplyDataDictionary = ApplyDataDictionary.Apply,
					ItemName = "(Configure Favorites)",
					MenuItemType = FMMenuItemType.MY_MENU_CONFIG_FAVORITES,
					NavigateUrl = "FavoritesSettingForm.aspx",
					SortOrder = 1000
				});

			menuData.FavoritesMenuItems.Add(
				new FMMenuItem
				{
					ApplyDataDictionary = ApplyDataDictionary.Apply,
					ItemName = "(Configure Quick Links)",
					MenuItemType = FMMenuItemType.QUICK_LINKS_CONFIG_QUICK_LINKS,
					NavigateUrl = "FavoritesSettingForm.aspx?QuickLinks=true",
					SortOrder = 1001
				});
		}

		/// <summary>
		/// Loads the URLs to be used for context-sensitive help: one default URL,
		///   and another for a translated (via data dictionary) version of the help
		///   that may be specified in tblSites.
		/// </summary>
		/// <param name="security">FuelsManager security object</param>
		/// <param name="menuData">Menu data object to load.</param>
		private static void LoadHelpURLs(SecurityClass security, FMMenuData menuData)
		{
			// Get default Help URL from tblConfigurationSetting
			menuData.DefaultHelpUrl =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_DefaultHelpURL));

			menuData.TranslatedHelpUrl = null;

			// Get Translated help URL from current site, or if that's empty and this
			// site uses the data dictionary owned by another site, then use that
			// site's value
			SiteClass currSite =
				FMChannelHelper.MakeCall<ISites, SiteClass>(
					sitesChannel => sitesChannel.Get(security, security.SiteGuid, false, false, false));
			if (!string.IsNullOrEmpty(currSite.TranslatedHelpURL))
			{
				menuData.TranslatedHelpUrl = currSite.TranslatedHelpURL;
			}
			else
			{
				EntityToSiteMapCollectionClass entityToSiteMapCollection =
					FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
						entityToSiteMapsChannel => entityToSiteMapsChannel.EnumerateByTypeIDAndGuid(
							security, ENTITY_TYPE.DATA_DICTIONARY, security.SiteGuid));

				if (entityToSiteMapCollection != null && entityToSiteMapCollection.Count > 0)
				{
					// Found data dictionary mapping from current site to another site
					Guid ownerSiteGuid = entityToSiteMapCollection[0].SiteGuid;
					SiteClass ownerSite =
						FMChannelHelper.MakeCall<ISites, SiteClass>(
							sitesChannel => sitesChannel.Get(security, ownerSiteGuid, false, false, false));
					if (!string.IsNullOrEmpty(ownerSite.TranslatedHelpURL))
					{
						menuData.TranslatedHelpUrl = ownerSite.TranslatedHelpURL;
					}
				}
			}
		}

		protected static string GetDataDictionaryValueByKey(Guid siteGuid, string value)
		{
			return DataDictionarySingleton.Get(siteGuid, value);
		}

		#endregion
	}
}