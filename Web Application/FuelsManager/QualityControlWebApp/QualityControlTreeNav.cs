// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityControlTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.QualityControlWebApp
{
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public class QualityControlTreeNav : IMenuDiscovery
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

            var items = new List<FMMenuItem>();

			// check the security permissions so we do not end up with this quality node with nothing under it
			if (security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) || security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
			    || security.HasRight(RIGHT.VIEW_QUALITY_TESTS) || security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
			    || security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) || security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
			    || security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) || security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			    || security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
			{
				// Add the sub nodes bases on individual security
				if (security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) || security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
				{
					// AddNode(MainNode, "Add Test Set Results", "../QualityControlWebApp/TestSetResultForm.aspx?MODE=ADD", true);
					items.Add(
						new FMMenuItem
							{
								MenuItemType = FMMenuItemType.OPERATIONS_QUALITY_ADD_TEST_SET_RESULTS, 
								RootMenuName = "Operations", 
								CategoryName = "Quality", 
								ItemName = "Add Test Set Results", 
								NavigateUrl = string.Format("{0}?MODE=ADD",this.TestSetResultFormURL(security)), 
								ApplyDataDictionary = ApplyDataDictionary.Apply, 
								SortOrder = 1
							});
				}

				if (security.HasRight(RIGHT.VIEW_QUALITY_TESTS) || security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				    || security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
				{
					// AddNode(MainNode, "Testing Results", "../QualityControlWebApp/TestResults.aspx", true);
					items.Add(
						new FMMenuItem
							{
								MenuItemType = FMMenuItemType.OPERATIONS_QUALITY_TESTING_RESULTS, 
								RootMenuName = "Operations", 
								CategoryName = "Quality", 
								ItemName = "Testing Results", 
								NavigateUrl = "../QualityControlWebApp/TestResults.aspx", 
								ApplyDataDictionary = ApplyDataDictionary.Apply, 
								SortOrder = 2
							});
				}

				if (security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD))
				{
					// AddNode(MainNode, "Add Quality Tag Record", "../QualityControlWebApp/QualityTagAddRecordForm.aspx?MODE=ADD", true);
					items.Add(
						new FMMenuItem
							{
								MenuItemType = FMMenuItemType.OPERATIONS_QUALITY_ADD_QUALITY_TAG_RECORD, 
								RootMenuName = "Operations", 
								CategoryName = "Quality", 
								ItemName = "Add Quality Tag Record", 
								NavigateUrl = "../QualityControlWebApp/QualityTagAddRecordForm.aspx?MODE=ADD", 
								ApplyDataDictionary = ApplyDataDictionary.Apply, 
								SortOrder = 3
							});
				}

				if (security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) || security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
				    || security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
				{
					// AddNode(MainNode, "Equipment Tags Summary", "../QualityControlWebApp/QualityTagLogForm.aspx", true);
					items.Add(
						new FMMenuItem
							{
								MenuItemType = FMMenuItemType.OPERATIONS_QUALITY_TAG_SUMMARY, 
								RootMenuName = "Operations", 
								CategoryName = "Quality", 
								ItemName = "Quality Tag Summary", 
								NavigateUrl = "../QualityControlWebApp/QualityTagLogForm.aspx", 
								ApplyDataDictionary = ApplyDataDictionary.Apply, 
								SortOrder = 4
							});
				}
			}

			return items;
		}

		protected string TestSetResultFormURL(SecurityClass security)
		{

			string testSetResultFormUrl =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(security, "TestSetResultFormURL"));
			if (string.IsNullOrEmpty(testSetResultFormUrl))
			{
				testSetResultFormUrl = "QualityControlWebApp/TestSetResultForm.aspx";
			}
			return "../" + testSetResultFormUrl;
		}



		#endregion
	}
}