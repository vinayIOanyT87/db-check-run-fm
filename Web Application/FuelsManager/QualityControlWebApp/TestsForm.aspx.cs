// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.QualityControlWebApp
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FMWebApp;

	using global::FMWebApp;

	/// <summary>
	///     Summary description for TestsForm.
	/// </summary>
	public partial class TestsForm : FMFormBase, IMenuDiscovery
	{
		#region Public Methods and Operators

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
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x04) != 0x04)
                    return null;
            }
            else
            {
                // Depends Upon Quality Control. 
                if ((options & 0x2000) == 0)
                {
                    return null;
                }
            }

            if (!security.HasRight(RIGHT.MODIFY_TEST_ITEMS) && !security.HasRight(RIGHT.VIEW_TEST_ITEMS))
			{
				return null;
			}

			var items = new List<FMMenuItem>
			            {
				            new FMMenuItem
				            {
					            MenuItemType = FMMenuItemType.CONFIG_QUALITY_TESTS_AND_INSPECTIONS,
					            RootMenuName = "Configuration",
					            CategoryName = "Quality",
					            ItemName = "Tests and Inspections",
					            NavigateUrl =
						            "../QualityControlWebApp/TestsAndInspectionsForm.aspx",
					            ApplyDataDictionary = ApplyDataDictionary.Apply
				            },
				            new FMMenuItem
				            {
					            MenuItemType = FMMenuItemType.CONFIG_QUALITY_TESTS_SETS,
					            RootMenuName = "Configuration",
					            CategoryName = "Quality",
					            ItemName = "Test Sets",
					            NavigateUrl = "../QualityControlWebApp/TestSetsForm.aspx",
					            ApplyDataDictionary = ApplyDataDictionary.Apply
				            },
				            new FMMenuItem
				            {
					            MenuItemType = FMMenuItemType.CONFIG_QUALITY_QUALITY_TAGS,
					            RootMenuName = "Configuration",
					            CategoryName = "Quality",
					            ItemName = "Quality Tags",
					            NavigateUrl = "../QualityControlWebApp/QualityTagsForm.aspx",
					            ApplyDataDictionary = ApplyDataDictionary.Apply
				            }
			            };

			return items;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Put user code to initialize the page here
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		#endregion
	}
}