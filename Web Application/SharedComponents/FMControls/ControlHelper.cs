// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ControlHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Web.UI;

	using FMBusinessObjects.BusinessInterfaces;

	/// <summary>
	/// Helper for controls to make WCF calls.
	/// </summary>
	internal class ControlHelper
	{
		/// <summary>
		/// Find the parent page which implements the ISharedWCFChannels interface
		/// </summary>
		/// <param name="currentPage">Curent page</param>
		/// <param name="parentPage">Found parent page</param>
		/// <returns>True if found</returns>
		internal static bool FindWCFParentPage(Page currentPage, out ISharedWCFChannels parentPage)
		{
			bool found = false;
			parentPage = null;

			if (currentPage != null)
			{
				Control tempPage = currentPage;

				do
				{
					var tempParentPage = tempPage as ISharedWCFChannels;

					// Check the interface match or not != null means match
					if (tempParentPage != null)
					{
						parentPage = tempParentPage;
						found = true;
					}

					tempPage = currentPage.Parent;					
				}
				while (tempPage != null);
			}

			return found;
		}
	}
}
