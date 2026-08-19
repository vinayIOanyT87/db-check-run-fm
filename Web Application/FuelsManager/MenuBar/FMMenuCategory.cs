// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="FMMenuCategory.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Holds the information for one category (sub-heading) in a drop-down menu.
//   The name (ItemName or DictionaryName) will appear as a heading on the dropdown
//   panel. The class is XML serializable so that
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
	///   Holds the information for one category (sub-heading) in a drop-down menu.
	///   The name (ItemName or DictionaryName) will appear as a heading on the dropdown
	///   panel. The class is XML serializable so that
	///   default information can be loaded at startup.
	/// </summary>
	[Serializable]
	public class FMMenuCategory
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMMenuCategory"/> class. 
		///   Default constructor
		/// </summary>
		public FMMenuCategory()
		{
			this.MenuItems = new List<FMMenuItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMMenuCategory"/> class. 
		/// Constructor
		/// </summary>
		/// <param name="categoryName">
		/// Value for CategoryName property 
		/// </param>
		public FMMenuCategory(string categoryName)
		{
			this.CategoryName = categoryName;
			this.MenuItems = new List<FMMenuItem>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		///   Gets or sets the name of the category
		/// </summary>
		public string CategoryName { get; set; }

		/// <summary>
		///   Gets or sets the name of the category, as loaded from current data dictionary,
		///   if applicable
		/// </summary>
		public string DictionaryName { get; set; }

		/// <summary>
		///   Gets or sets the limit of menu items to put in one column before creating
		///   a new column. If zero, then unlimited.
		/// </summary>
		public int MaxItemsPerColumn { get; set; }

		/// <summary>
		///   Gets or sets the collection of menu items in the category. This property is not
		///   serialized, because it is not needed for default menu information.
		/// </summary>
		[XmlIgnore]
		public List<FMMenuItem> MenuItems { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Retrieves the category name to show, based on whether or not to use
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

			return this.CategoryName;
		}

		#endregion
	}
}