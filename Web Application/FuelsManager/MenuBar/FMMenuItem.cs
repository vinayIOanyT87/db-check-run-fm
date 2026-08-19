// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="FMMenuItem.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Enumeration to designate whether data dictionary should be applied to the menu item.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///   Enumeration to designate whether data dictionary should be applied to the menu item.
	/// </summary>
	public enum ApplyDataDictionary
	{
		Apply, 

		DoNotApply
	}

	/// <summary>
	///   Holds information about a menu item that is displayed from one of the drop-down
	///   menus at the top of the page.
	/// </summary>
	[Serializable]
	public class FMMenuItem : ICloneable
	{
		#region Private Fields
		
		bool _OpenInSeparateTab = false;

		bool _IsEnabled = true;
		
		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMMenuItem"/> class. 
		///   Default constructor
		/// </summary>
		public FMMenuItem()
		{
			this.ApplyDataDictionary = ApplyDataDictionary.Apply;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets whether to apply the data dictionary to this menu item
		/// </summary>
		public ApplyDataDictionary ApplyDataDictionary { get; set; }

		/// <summary>
		/// Gets or sets the name of the category to which the item belongs
		/// </summary>
		public string CategoryName { get; set; }

		/// <summary>
		/// Gets or sets the data dictionary group prefix
		/// </summary>
		public string DataDictGroupPrefix { get; set; }

		/// <summary>
		/// Gets or sets the description to display in a Tool Tip for the link
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the name of the item, as loaded from current data dictionary, if applicable
		/// </summary>
		public string DictionaryName { get; set; }

		/// <summary>
		/// Gets or sets a Guid that, combined with MenuItemType, makes a unique identifier
		/// for the menu item. This will hold Guid.Empty unless the item is a
		/// dynamically created item, such as for an Add Transaction or a report
		/// </summary>
		public Guid DynamicMenuItemGuid { get; set; }

		/// <summary>
		/// Gets or sets the name of the item
		/// </summary>
		public string ItemName { get; set; }

		/// <summary>
		/// Gets or sets an identifier for the menu item that can be used for saving
		/// favorites and Quick Links.
		/// </summary>
		public FMMenuItemType MenuItemType { get; set; }

		/// <summary>
		/// Gets or sets the URL to which clicking the link should navigate
		/// </summary>
		public string NavigateUrl { get; set; }

		/// <summary>
		/// Gets or sets the name of the root menu.
		/// </summary>
		public string RootMenuName { get; set; }

		/// <summary>
		/// Gets or sets the sort order relative to other items in the category. If
		/// zero, then the item will appear after any items that have non-zero SortOrder.
		/// </summary>
		public int SortOrder { get; set; }

		/// <summary>
		/// Gets or sets whether clicking on a menu item should launch in a separate tab or in the iFrame.
		/// </summary>
		public bool OpenInSeparateTab
		{
			get
			{
				return _OpenInSeparateTab;
			}
			set
			{
				_OpenInSeparateTab = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the menu item is enbled or not.
		/// </summary>
		public bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				_IsEnabled = value;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///   Implementation of ICloneable.Clone(). Used for creating Favorite
		///   and Quick Link menu items.
		/// </summary>
		/// <returns> Cloned object </returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Retrieves the text to show, based on whether or not to use
		///   the data dictionary
		/// </summary>
		/// <param name="useDataDictionary">
		/// Whether to use data dictionary value 
		/// </param>
		/// <returns>
		/// Name to display 
		/// </returns>
		public string GetDisplayName(bool useDataDictionary)
		{
			if (useDataDictionary && !string.IsNullOrEmpty(this.DictionaryName))
			{
				return this.DictionaryName;
			}

			return this.ItemName;
		}

		#endregion
	}
}