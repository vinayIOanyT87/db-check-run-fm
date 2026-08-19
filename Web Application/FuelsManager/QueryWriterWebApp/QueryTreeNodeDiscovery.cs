// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryTreeNodeDiscovery.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the QueryTreeNodeDiscovery type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.QueryWriterWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	//using Microsoft.Web.Services2.Referral;

	/// <summary>
	/// Class for hosting methods to provide menu options for Query Writer.
	/// </summary>
	public class QueryTreeNodeDiscovery : IMenuDiscovery
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
                if ((word1 & 0x04) != 0x04)
                    return null;
            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (security.HasRight(RIGHT.MODIFY_QUERIES) == false && security.HasRight(RIGHT.VIEW_QUERIES) == false)
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.REPORTS_QUERY_WRITER_QUERIES,
						RootMenuName = "Reports",
						CategoryName = "Query Writer",
						ItemName = "Queries",
						NavigateUrl = "../QueryWriterWebApp/ManageQueriesForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply,
						SortOrder = 1
					});

			if (security.HasRight(RIGHT.MODIFY_QUERIES))
			{
				items.Add(
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.REPORTS_QUERY_WRITER_CREATE_NEW_QUERY,
							RootMenuName = "Reports",
							CategoryName = "Query Writer",
							ItemName = "Create New Query",
							NavigateUrl = "../QueryWriterWebApp/QueryDefinitionForm.aspx?Mode=New",
							ApplyDataDictionary = ApplyDataDictionary.Apply,
							SortOrder = 2
						});
			}



			QueryCollectionClass nodeQueries =
				FMChannelHelper.MakeCall<IQueries, QueryCollectionClass>(x => x.EnumerateQueryNodes(security));

			foreach (QueryClass query in nodeQueries)
			{
				string menuName = "Reports";
				string menuCategory = "Queries";

				string[] navPathArray = query.NavNodePath.Split('/');

				if (navPathArray.Length > 0)
				{
					menuName = navPathArray[0];
				}

				if (navPathArray.Length > 1)
				{
					menuCategory = navPathArray[1];
				}


				items.Add(
					new FMMenuItem
					{
						MenuItemType = FMMenuItemType.DYNAMIC_REPORT,
						RootMenuName = menuName,
						CategoryName = menuCategory,
						ItemName = query.QueryName,
						NavigateUrl = "../QueryWriterWebApp/QueryResultsForm.aspx?id=" + query.QueryStorageGuid,
						ApplyDataDictionary = ApplyDataDictionary.Apply,
						DynamicMenuItemGuid = query.QueryStorageGuid
					});

			}

			return items;
		}

		#endregion

	}

	/// <summary>
	/// Class for hosting methods to provide menu options for Query Writer.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
		Justification = "Reviewed. Suppression is OK here.")]
	public class QueryTreeNodeDiscoveryConfiguration : IMenuDiscovery
	{
		#region IMenuDiscovery Implementation

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
                if ((word1 & 0x04) != 0x04)
                    return null;
            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (security.HasRight(RIGHT.CONFIGURE_QUERIES) == false && security.HasRight(RIGHT.MODIFY_QUERIES) == false && security.HasRight(RIGHT.VIEW_QUERIES) == false)
			{
				return null;
			}

			if (security.HasRight(RIGHT.MODIFY_QUERIES) || security.HasRight(RIGHT.VIEW_QUERIES))
			{
				items.Add(
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.CONFIG_REPORTS_QUERIES_MANAGE_QUERIES,
							RootMenuName = "Configuration",
							CategoryName = "Reports/Queries",
							ItemName = "Manage Queries",
							NavigateUrl = "../QueryWriterWebApp/ManageQueriesForm.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						});
			}

			if (security.HasRight(RIGHT.CONFIGURE_QUERIES))
			{
				items.Add(
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.CONFIG_REPORTS_QUERIES_QUERY_SETTINGS,
							RootMenuName = "Configuration",
							CategoryName = "Reports/Queries",
							ItemName = "Query Settings",
							NavigateUrl = "../QueryWriterWebApp/QueryConfigurationSettings.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						});
			}

			return items;
		}

		#endregion
	}
}