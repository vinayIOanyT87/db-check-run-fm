// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportingTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportingTreeNav type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FuelsManager.FMWebApp;

	public class ReportingTreeNav : IMenuDiscovery
	{
		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
		{
			var menuItems = new List<FMMenuItem>();
			var helper = new ReportingTreeNavHelper();

			if (useNewLicenseKey == 1)
			{
				if ((word1 & 0x04) != 0x04)
					return null;
			}
			else
			{
				if (!helper.HasHardwareKey(options))
					return null;
			}

			if (helper.HasViewPermissions(security))
			{
				ReportConfigurationGroupListDO reportGroups = this.GetReportGroups(security);
				ReportConfigurationDetailListDO reportItems = this.GetReportDetails(security);

				// Ensure that our report configuration data exists.
				if (reportGroups == null || reportItems == null)
				{
					return menuItems;
				}

				// The order of the groups and reports in the menu should respect the order specified by the user when configuring reports and report groups
				// order the groups first
				IEnumerable<ReportConfigurationGroupDO> groupList = reportGroups.ReportGroupDOList.OrderBy(group => group.OrderNumber);
				List<ReportConfigurationDetailDO> detailList = reportItems.ReportDetailDOList;

				foreach (ReportConfigurationGroupDO group in groupList)
				{
					// Find all of the reports that belong to this group and order them too
					IEnumerable<ReportConfigurationDetailDO> groupReports = detailList.FindAll(report => (report.ReportGroupGuid == group.ReportGroupGuid) && (report.DWReportFlag == false))
						.OrderBy(report => report.OrderNumber);

					menuItems.AddRange(groupReports.Select((report, i) =>
						new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DYNAMIC_REPORT,
							RootMenuName = "Reports",
							CategoryName = group.GroupName,
							ItemName = report.ReportName,
							NavigateUrl = this.GetReportUrl(report.ReportGuid, security),
							DynamicMenuItemGuid = report.ReportGuid,
							Description = report.ReportDescription,
							ApplyDataDictionary = ApplyDataDictionary.Apply,
							SortOrder = i + 1 // it's i+1 because a SortOrder = 0 means it goes last 
						}));
				}
            bool isDataWarehouseKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDatawarehouseKey());
				if (isDataWarehouseKey)
				{
					foreach (ReportConfigurationGroupDO group in groupList)
					{
						// Find all of the dw reports that belong to this group and order them too
						IEnumerable<ReportConfigurationDetailDO> dwGroupReports = detailList.FindAll(report => (report.ReportGroupGuid == group.ReportGroupGuid) && (report.DWReportFlag == true))
							.OrderBy(report => report.OrderNumber);

						menuItems.AddRange(dwGroupReports.Select((report, i) =>
							new FMMenuItem
							{
								MenuItemType = FMMenuItemType.DYNAMIC_REPORT,
								RootMenuName = "Data Analytics",
								CategoryName = group.GroupName,
								ItemName = report.ReportName,
								NavigateUrl = this.GetReportUrl(report.ReportGuid, security),
								DynamicMenuItemGuid = report.ReportGuid,
								Description = report.ReportDescription,
								ApplyDataDictionary = ApplyDataDictionary.Apply,
								SortOrder = i + 1 // it's i+1 because a SortOrder = 0 means it goes last 
							}));
					}
				}

				// Find reports without a group
				IEnumerable<ReportConfigurationDetailDO> reportsWithoutAGroup = detailList.FindAll(report => (report.ReportGroupGuid == Guid.Empty) && (report.DWReportFlag == false))
					.OrderBy(report => report.OrderNumber);

				menuItems.AddRange(reportsWithoutAGroup.Select((report, i) =>
					new FMMenuItem
					{
						MenuItemType = FMMenuItemType.DYNAMIC_REPORT,
						RootMenuName = "Reports",
						CategoryName = "Uncategorized",
						ItemName = report.ReportName,
						NavigateUrl = this.GetReportUrl(report.ReportGuid, security),
						DynamicMenuItemGuid = report.ReportGuid,
						Description = report.ReportDescription,
						ApplyDataDictionary = ApplyDataDictionary.Apply,
						SortOrder = i + 1 // it's i+1 because a SortOrder = 0 means it goes last 
					}));

				var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.REPORTS_GENERAL_ALL_REPORTS,
					RootMenuName = "Reports",
					CategoryName = "General",
					ItemName = "All Reports",
					NavigateUrl = "../FMReportWebMain/FMReportDynamicSelectionPage.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

				menuItems.Add(menuItem);
			}

			return menuItems;
		}

		#endregion

		#region Methods

		/// <summary>
		///    Gets the report details.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A list of report menu items.</returns>
		private ReportConfigurationDetailListDO GetReportDetails(SecurityClass security)
		{
			var detailSr = new ReportConfigurationDetailSR
			{
				RequestType = ReportConfigurationDetailSR.RequestTypes.GET_ALL_NON_PRINT,
				Site = security.SiteID,
				CurrentSiteGuid = security.SiteGuid,
				Security = security
			};

			ReportConfigurationDetailListDO detailListDo =
				FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailListDO>(
																	 x =>
																	 x.GetAllNonPrint(detailSr)
																);

			return detailListDo;
		}

		/// <summary>
		///    Gets the report groups.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A list of report groups from configuration.</returns>
		private ReportConfigurationGroupListDO GetReportGroups(SecurityClass security)
		{
			var groupSr = new ReportConfigurationGroupSR
			{
				RequestType = ReportConfigurationGroupSR.RequestTypes.GET_ALL,
				Site = security.SiteID,
				CurrentSiteGuid = security.SiteGuid,
				Security = security
			};

			ReportConfigurationGroupListDO groupListDo =
				FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor, ReportConfigurationGroupListDO>(
																	 x =>
																	 x.GetAll(groupSr)
																);


			return groupListDo;
		}

		/// <summary>
		///    Gets the report URL.
		/// </summary>
		/// <param name="reportGuid">The report GUID.</param>
		/// <param name="security">The security.</param>
		/// <returns>
		///    A report URL for a menu item.
		/// </returns>
		private string GetReportUrl(Guid reportGuid, SecurityClass security)
		{
			if (reportGuid != Guid.Empty)
			{
				var detailSr = new ReportConfigurationDetailSR
				{
					RequestType = ReportConfigurationDetailSR.RequestTypes.GET,
					Site = security.SiteID,
					CurrentSiteGuid = security.SiteGuid
				};

				var detailDo = new ReportConfigurationDetailDO { ReportGuid = reportGuid, SiteGuid = security.SiteGuid };

				detailSr.ReportConfigurationDetailDO = detailDo;
				detailSr.Security = security;

				detailDo = FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailDO>(
																	 x =>
																	 x.GetConfiguration(detailSr)
																);


				// Concatenate the URL, directory, and report name.
				string reportName = detailDo.ReportPath.Replace(" ", "+");
				//var reportUrl = "../FMReporting/ReportLandingPage.aspx?ReportType=";
				string reportUrl = "../FMReportWebMain/ReportLandingPage.aspx?ReportType=";

				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsAviationProduct()))
				{
					string aviation = ((int)ReportTypesClass.ReportTypes.AVIATION_RPT).ToString(CultureInfo.InvariantCulture);
					reportUrl = reportUrl + aviation;
				}
				else
				{
					string oilAndGas = ((int)ReportTypesClass.ReportTypes.OIL_GAS_RPT).ToString(CultureInfo.InvariantCulture);
					reportUrl = reportUrl + oilAndGas;
				}

				reportUrl = reportUrl + "&IsDWReport=" + Convert.ToString(detailDo.DWReportFlag);

				reportUrl = reportUrl + "&ReportName=" + reportName;

				return reportUrl;
			}

			return string.Empty;
		}

		#endregion
	}
}