// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="FMMenuData.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Holds all the data needed by FMMenuBar to display its content, including
//   dropdown menus, QuickLinks, user and site information. The class is XML
//   serializable so that default information can be loaded at startup.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///   Holds all the data needed by FMMenuBar to display its content, including
	///   dropdown menus, QuickLinks, user and site information. The class is XML 
	///   serializable so that default information can be loaded at startup.
	/// </summary>
	[Serializable]
	public class FMMenuData
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMMenuData"/> class. 
		///   Default constructor
		/// </summary>
		public FMMenuData()
		{
			this.MenuRootItems = new List<FMMenuRootItem>();
			this.Favorites = new MenuFavoriteCollectionClass();
			this.QuickLinks = new MenuFavoriteCollectionClass();
			this.QuickLinksMenuItems = new List<FMMenuItem>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		///   Gets or sets a reference to the menu item most recently invoked
		/// </summary>
		[XmlIgnore]
		public FMMenuItem CurrentMenuItem { get; set; }

		/// <summary>
		///   Gets or sets the Location of help content that has not been translated via a data
		///   dictionary
		/// </summary>
		[XmlIgnore]
		public string DefaultHelpUrl { get; set; }

		/// <summary>
		///   Gets or sets a collection of data objects for the user's Favorites
		/// </summary>
		[XmlIgnore]
		public MenuFavoriteCollectionClass Favorites { get; set; }

		/// <summary>
		///   Gets the collection of menu items in the
		///   Favorites category of My Menu
		/// </summary>
		[XmlIgnore]
		public List<FMMenuItem> FavoritesMenuItems
		{
			get
			{
				return this.MenuRootItems[0].MenuCategories.Find(x => x.CategoryName == "Favorites").MenuItems;
			}
		}

		/// <summary>
		///   Gets or sets the root menu items, which in turn hold the entire hierarchy
		///   of categories and leaf menu items
		/// </summary>
		public List<FMMenuRootItem> MenuRootItems { get; set; }

		/// <summary>
		///   Gets or sets a collection of data objects for the user's Quick Links
		/// </summary>
		[XmlIgnore]
		public MenuFavoriteCollectionClass QuickLinks { get; set; }

		/// <summary>
		///   Gets or sets a collection of menu items used to display the user's Quick Links
		/// </summary>
		[XmlIgnore]
		public List<FMMenuItem> QuickLinksMenuItems { get; set; }

		/// <summary>
		///   Gets the collection of menu items in the
		///   Recent category of My Menu
		/// </summary>
		[XmlIgnore]
		public List<FMMenuItem> RecentMenuItems
		{
			get
			{
				return this.MenuRootItems[0].MenuCategories.Find(x => x.CategoryName == "Recent").MenuItems;
			}
		}

		/// <summary>
		///   Gets or sets Location of help content that has been translated via a data dictionary
		/// </summary>
		[XmlIgnore]
		public string TranslatedHelpUrl { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Add a menu item to the proper place in the hierarchy based on the values of
		///   the RootMenuName and CategoryName properties of the menu item
		/// </summary>
		/// <param name="menuItem">
		/// Item to add 
		/// </param>
		public void AddMenuItem(FMMenuItem menuItem)
		{
			// Check that menu item types aren't being used more than once
			if (this.GetMenuItem(menuItem.MenuItemType, menuItem.DynamicMenuItemGuid) != null)
			{
				throw new ApplicationException("Attempted to add duplicate menu item");
			}

			FMMenuRootItem menuRootItem = null;

			foreach (FMMenuRootItem findMenuRootItem in this.MenuRootItems)
			{
				if (findMenuRootItem.RootItemName == menuItem.RootMenuName)
				{
					menuRootItem = findMenuRootItem;
					break;
				}
			}

			if (menuRootItem == null)
			{
				// Root menus cannot be dynamically created. They must all appear in
				// DefaultMenuData.xml.
				// Commented throwing exception by Srini 02-15-23 as this is prematurely exiting from loading all other entries in the file
				// For TAS, we don't want to show Wingware root menu; one way to accomplish this is by commenting the entry from the DefaultMenuData.xml by SI
				// For all other Aviation systems, PO wants the root entry for Wingware be displayed
				//throw new ApplicationException("Root menu not found: " + menuItem.RootMenuName);
				return;
			}

			FMMenuCategory menuCategory = null;

			foreach (FMMenuCategory findMenuCategory in menuRootItem.MenuCategories)
			{
				if (findMenuCategory.CategoryName == menuItem.CategoryName)
				{
					menuCategory = findMenuCategory;
					break;
				}
			}

			// Create new category if not found
			if (menuCategory == null)
			{
				menuCategory = new FMMenuCategory(menuItem.CategoryName);
				menuRootItem.MenuCategories.Add(menuCategory);
			}

			menuCategory.MenuItems.Add(menuItem);
		}

		/// <summary>
		/// Get the URL for help content based on whether data dictionary is
		///   enabled.
		/// </summary>
		/// <param name="useDataDictionary">
		/// whether to look for translated content 
		/// </param>
		/// <returns>
		/// the URL 
		/// </returns>
		public string GetHelpUrl(bool useDataDictionary)
		{
			if (useDataDictionary && !string.IsNullOrEmpty(this.TranslatedHelpUrl))
			{
				return this.TranslatedHelpUrl;
			}

			return this.DefaultHelpUrl;
		}

		/// <summary>
		/// Retrieves a menu item from the hierarchy of menu items based on its unique
		///   identifiers. Favorites and Quick Links are not searched, because their items
		///   are duplicates of items in the main menus. Exceptions: Favorites config and
		///   Quick Links config.
		/// </summary>
		/// <param name="menuItemType">
		/// MenuItemTtype enum 
		/// </param>
		/// <param name="dynamicMenuItemGuid">
		/// Guid assigned to the menu item 
		/// </param>
		/// <returns>
		/// Menu item object, or null if not found 
		/// </returns>
		public FMMenuItem GetMenuItem(FMMenuItemType menuItemType, Guid dynamicMenuItemGuid)
		{
			FMMenuItem foundItem = null;

			// There is one menu item that doesn't appear in a menu per se: Quick Links configuration
			if (menuItemType == FMMenuItemType.QUICK_LINKS_CONFIG_QUICK_LINKS && dynamicMenuItemGuid == Guid.Empty)
			{
				return this.QuickLinksMenuItems[0];
			}

			// Search this separately b/c My Menu is excluded in loop below
			if (menuItemType == FMMenuItemType.MY_MENU_CONFIG_FAVORITES && dynamicMenuItemGuid == Guid.Empty)
			{
				foreach (FMMenuItem menuItem in this.FavoritesMenuItems)
				{
					if (menuItem.MenuItemType == menuItemType && menuItem.DynamicMenuItemGuid == dynamicMenuItemGuid)
					{
						return menuItem;
					}
				}
			}

			foreach (FMMenuRootItem menuRootItem in this.MenuRootItems)
			{
				if (menuRootItem.RootItemName != "My Menu")
				{
					foreach (FMMenuCategory menuCategory in menuRootItem.MenuCategories)
					{
						foreach (FMMenuItem menuItem in menuCategory.MenuItems)
						{
							if (menuItem.MenuItemType == menuItemType && menuItem.DynamicMenuItemGuid == dynamicMenuItemGuid)
							{
								foundItem = menuItem;
								break;
							}
						}

						if (foundItem != null)
						{
							break;
						}
					}

					if (foundItem != null)
					{
						break;
					}
				}
			}

			return foundItem;
		}

		/// <summary>
		/// Construct a string of the form "Root Menu &gt;&gt; Category &gt;&gt; Item Name" for the given menu item
		/// </summary>
		/// <param name="menuItemType">
		/// MenuItemTtype enum 
		/// </param>
		/// <param name="dynamicMenuItemGuid">
		/// Guid assigned to the menu item 
		/// </param>
		/// <param name="useDataDictionary">
		/// Whether to use data dictionary names 
		/// </param>
		/// <returns>
		/// Constructed string, or empty string if item not found 
		/// </returns>
		public string GetMenuItemPath(FMMenuItemType menuItemType, Guid dynamicMenuItemGuid, bool useDataDictionary)
		{
			// There is one menu item that doesn't appear in a menu per se: Quick Links configuration
			if (menuItemType == FMMenuItemType.QUICK_LINKS_CONFIG_QUICK_LINKS && dynamicMenuItemGuid == Guid.Empty)
			{
				return "Quick Links >> Quick Links Configuration";
			}

			// Search this separately b/c My Menu is excluded in loop below
			if (menuItemType == FMMenuItemType.MY_MENU_CONFIG_FAVORITES && dynamicMenuItemGuid == Guid.Empty)
			{
				foreach (FMMenuItem menuItem in this.FavoritesMenuItems)
				{
					if (menuItem.MenuItemType == menuItemType && menuItem.DynamicMenuItemGuid == dynamicMenuItemGuid)
					{
						return "My Menu >> Favorites >> " + menuItem.GetDisplayName(useDataDictionary);
					}
				}
			}

			foreach (FMMenuRootItem menuRootItem in this.MenuRootItems)
			{
				if (menuRootItem.RootItemName != "My Menu")
				{
					foreach (FMMenuCategory menuCategory in menuRootItem.MenuCategories)
					{
						foreach (FMMenuItem menuItem in menuCategory.MenuItems)
						{
							if (menuItem.MenuItemType == menuItemType && menuItem.DynamicMenuItemGuid == dynamicMenuItemGuid)
							{
								return menuRootItem.GetDisplayName(useDataDictionary) + " >> " + menuCategory.GetDisplayName(useDataDictionary)
								       + " >> " + menuItem.GetDisplayName(useDataDictionary);
							}
						}
					}
				}
			}

			return string.Empty;
		}

		/// <summary>
		///   Remove empty categories and root menu items. These can be empty
		///   when a user doesn't have permission to any of the menu items in them.
		/// </summary>
		public void RemoveEmptyMenus()
		{
			var rootItemsToRemove = new List<FMMenuRootItem>();

			foreach (FMMenuRootItem menuRootItem in this.MenuRootItems)
			{
				// Some menus are marked DisplayAlways, namely My Menu
				if (!menuRootItem.DisplayAlways)
				{
					var categoriesToRemove = new List<FMMenuCategory>();

					foreach (FMMenuCategory menuCategory in menuRootItem.MenuCategories)
					{
						if (menuCategory.MenuItems.Count == 0)
						{
							categoriesToRemove.Add(menuCategory);
						}
					}

					foreach (FMMenuCategory menuCategory in categoriesToRemove)
					{
						menuRootItem.MenuCategories.Remove(menuCategory);
					}

					if (menuRootItem.MenuCategories.Count == 0)
					{
						rootItemsToRemove.Add(menuRootItem);
					}
				}
			}

			foreach (FMMenuRootItem menuRootItem in rootItemsToRemove)
			{
                //if (menuRootItem.RootItemName == "Mobile")//WW-Dispatch")
                //{
                //    menuRootItem.IsEnabled = false;
                //}
                //else
                //{
                    this.MenuRootItems.Remove(menuRootItem);
                //}
            }
		}

		/// <summary>
		/// Sort menu items within categories if any have SortOrder &gt; 0.
		/// </summary>
		/// <param name="useDataDictionaryToSortItems">
		/// Whether to use data dictionary names to sort menu items 
		/// </param>
		public void SortMenuItems(bool useDataDictionaryToSortItems)
		{
			foreach (FMMenuRootItem menuRootItem in this.MenuRootItems)
			{
				foreach (FMMenuCategory menuCategory in menuRootItem.MenuCategories)
				{
					// Sort items such that items with SortOrder=0 go last, otherwise,
					// items with lower SortOrder come first. If SortOrders are equal,
					// then sort alphabetically
					menuCategory.MenuItems.Sort(
						(x, y) =>
						x.SortOrder == y.SortOrder
							? string.Compare(x.GetDisplayName(useDataDictionaryToSortItems), y.GetDisplayName(useDataDictionaryToSortItems), StringComparison.OrdinalIgnoreCase)
							: x.SortOrder == 0 ? y.SortOrder : y.SortOrder == 0 ? -1 : (x.SortOrder - y.SortOrder));
				}
			}
		}

		public void AttachCSRFToken(string rndTokenStr)
		{
			if (this.TranslatedHelpUrl != null && !this.TranslatedHelpUrl.Contains("CSRFToken="))
			{
				if (this.TranslatedHelpUrl.Contains("?"))
				{
					this.TranslatedHelpUrl += "&";
				}
				else
				{
					this.TranslatedHelpUrl += "?";
				}

				this.TranslatedHelpUrl += "CSRFToken=" + rndTokenStr;
			}
		}



		#endregion
	}
}