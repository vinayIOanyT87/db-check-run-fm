// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrainingSummaryTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.TrainingWebApp
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	/// FuelsManager menu implementation for this web app
	/// </summary>
	public class TrainingSummaryTreeNav : FMFormBase, IMenuDiscovery
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
		/// List of menu items to be displayed.
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x040) != 0x40)
                    return null;
            }
            else
            {
                // check if this option is set in the hardware key
                if ((options & 0x2000000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (!siteGroup
			    && (security.HasRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS)
			        || security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS) || security.HasRight(RIGHT.MODIFY_PERSON_TRAINING)))
			{
				const string TrainingSummaryAppUrl = "../TrainingWebApp/";

				items.Add(
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.OPERATIONS_TRAINING_TRAINING_SUMMARY, 
							RootMenuName = "Operations", 
							CategoryName = "Training", 
							ItemName = "Training Summary", 
							NavigateUrl = TrainingSummaryAppUrl + "TrainingSummary.aspx", 
							ApplyDataDictionary = ApplyDataDictionary.Apply, 
							SortOrder = 2
						});

				if (security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS) || security.HasRight(RIGHT.MODIFY_PERSON_TRAINING)
				    || security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
				{
					items.Add(
						new FMMenuItem
							{
								MenuItemType = FMMenuItemType.OPERATIONS_TRAINING_TRAINING_ASSIGNMENTS, 
								RootMenuName = "Operations", 
								CategoryName = "Training", 
								ItemName = "Training Assignments", 
								NavigateUrl = TrainingSummaryAppUrl + "TrainingAssignments.aspx", 
								ApplyDataDictionary = ApplyDataDictionary.Apply, 
								SortOrder = 1
							});
				}
			}

			return items;
		}

		#endregion
	}
}