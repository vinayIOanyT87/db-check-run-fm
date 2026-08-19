// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="FMMenuRootItem.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Holds the information for one drop-down menu. The title (ItemName or
//   DictionaryName) will appear on the menu bar. When a user hovers over it,
//   a panel full of links appears. The class is XML serializable so that
//   default information can be loaded at startup.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	/// <summary>
	///   Holds the information for one drop-down menu. The title (ItemName or
	///   DictionaryName) will appear on the menu bar. When a user hovers over it,
	///   a panel full of links appears. The class is XML serializable so that
	///   default information can be loaded at startup.
	/// </summary>
	[Serializable]
	public class FMMenuRootItem
	{
		#region private fields
		
		bool _IsEnabled = true;
		
		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMMenuRootItem"/> class. 
		///   Default constructor
		/// </summary>
		public FMMenuRootItem()
		{
			this.MenuCategories = new List<FMMenuCategory>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		///   Gets or sets CSS class for each column of menu items in the panel
		/// </summary>
		public string ColumnCssClass { get; set; }

		/// <summary>
		///   Gets or sets the name of the menu, as loaded from current data dictionary,
		///   if applicable
		/// </summary>
		public string DictionaryName { get; set; }

		/// <summary>
		///   Gets or sets a value indicating whether to always display this menu (and all the categories
		///   in it) regardless of whether the menu is empty or not
		/// </summary>
		public bool DisplayAlways { get; set; }

		/// <summary>
		///   Gets or sets the expected number of columns across on the panel. This is
		///   used for formatting.
		/// </summary>
		public int ExpectedNumColumns { get; set; }

		/// <summary>
		///   Gets or sets the collection of categories, which appear as headings within the
		///   dropdown panel, and which hold the leaf menu items
		/// </summary>
		public List<FMMenuCategory> MenuCategories { get; set; }

		/// <summary>
		///   Gets or sets the actual number of columns across on the panel. This is computed
		///   during rendering of the menu control
		/// </summary>
		[XmlIgnore]
		public int NumColumnPanels { get; set; }

		/// <summary>
		///   Gets or sets the CSS class for the dropdown panel
		/// </summary>
		public string PanelCssClass { get; set; }

		/// <summary>
		///   Gets or sets the name of the menu
		/// </summary>
		public string RootItemName { get; set; }

		/// <summary>
		/// Gets or sets the image src for the menu item when the mouse is over the menu items
		/// </summary>
		public string OnImageFileName { get; set; }

		/// <summary>
		/// Gets or sets the image src for the menu item when the mouse is not over the menu items
		/// </summary>
		public string OffImageFileName { get; set; }
		/// <summary>
		///   Gets or sets a value indicating whether to enabled or the this menu item
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
		/// Retrieves the menu name to show, based on whether or not to use
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

			return this.RootItemName;
		}

		#endregion
	}
}