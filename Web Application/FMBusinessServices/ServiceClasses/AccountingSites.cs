// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountingSites.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AccountingSites type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;

	/// <summary>
	/// The AccountingSites object is used to gather and organize site information for accounting purposes.
	/// </summary>
	public class AccountingSites : IAccountingSites
	{
		#region Public Methods and Operators

		/// <summary>
		/// This method will load the site information for the current given site and determine if the site is a site group or not. 
		/// If it is a site group, then it will get all the children site names.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="siteGuid">The GUID of the site to load.</param>
		/// <returns>An AccountingSite object containing site information for accounting purposes.</returns>
		public AccountingSite LoadSiteInfo(SecurityClass security, Guid siteGuid)
		{
			return this.LocalLoadSiteInfo(security, siteGuid,true);
		}

		/// <summary>
		/// This method will load the site information for the current given site and determine if the site is a site group or not. 
		/// If it is a site group, then it will get all the children site names.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="siteGuid">The GUID of the site to load.</param>
		/// <returns>An AccountingSite object containing site information for accounting purposes.</returns>
		public AccountingSite LoadSiteInfoNoCompanies(SecurityClass security, Guid siteGuid)
		{
			return this.LocalLoadSiteInfo(security, siteGuid,false);
		}
		#endregion

		#region Methods

		/// <summary>
		/// This method will retrieve and load an array list of companies that the user has permissions to view. If the list from the security object is empty, then the user has permissions to view all the data. In this case the flag will be set to true. Otherwise, it is set to false and the array will contain the companies the user can view.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="accountingSite">
		/// The accounting Site.
		/// </param>
		private void AccountingSites_RetrieveUsersCompanies(SecurityClass security, AccountingSite accountingSite)
		{
			// Ignore getting the user companies if the client has flag set
			// to false.
			if (accountingSite.GetUserCompanies == false)
			{
				return;
			}

			var timer = new StopWatch(StopWatch.Appnames.AccountingBLL, "AccountingSite.RetrieveUsersCompanies()");

			var companies = new CompaniesClass();
			accountingSite.UserCompanyList.Clear();
			accountingSite.UserCompanyList.AddRange(companies.GetCompanyGuidList(security, byGroupCompanies: true, localize: false));

			timer.Stop();
		}

		/// <summary>
		/// This method will load the site information for the current given site and determine if the site is a site group or not. 
		/// If it is a site group, then it will get all the children site names.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="siteGuid">The GUID of the site to load.</param>
		/// <param name="loadCompanies">A boolean indicating the callers desire to load the company info.</param>
		/// <returns>An AccountingSite object containing site information for accounting purposes.</returns>
		private AccountingSite LocalLoadSiteInfo(SecurityClass security, Guid siteGuid, bool loadCompanies)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (siteGuid == null)
			{
				throw new ArgumentNullException(nameof(siteGuid));
			}

			var accountingSite = new AccountingSite();
			var timer = new StopWatch(StopWatch.Appnames.AccountingBLL, "AccountingSite.loadSiteInfo()");
			var sites = new SitesClass();

			accountingSite.CurrentSite = sites.Get(security, siteGuid, false, false, false);
			accountingSite.CurrentSiteName = accountingSite.CurrentSite.ID;
			accountingSite.CurrentSiteGuid = accountingSite.CurrentSite.SiteGuid;
			accountingSite.Security = security;

			// If the current site and the login site are the same, there is not need to get the site again
			if (siteGuid == security.LoginSiteGuid)
			{
				accountingSite.LoginSite = accountingSite.CurrentSite;
			}
			else
			{
				accountingSite.LoginSite = sites.Get(security, security.LoginSiteGuid, false, false, false);
			}

			// If the current site is a site group, then grab all the children
			// sites and save the site names into an array.
			if (accountingSite.IsSiteGroup)
			{
				SiteCollectionClass siteChildList = sites.EnumerateLimitSiteMemberByParentSite(security, siteGuid);

				foreach (SiteClass site in siteChildList)
				{
					accountingSite.SiteList.Add(new Site(site.ID, site.SiteGuid));
				}

				accountingSite.SiteList.Add(new Site(accountingSite.CurrentSiteName, accountingSite.CurrentSiteGuid));
			}
			else
			{
				// Place the current site into the array list.
				accountingSite.SiteList.Add(new Site(accountingSite.CurrentSiteName, accountingSite.CurrentSiteGuid));
			}

			// Create all the unit conversions from SI to new value.
			accountingSite.CreateFromSiObjects();
			if (loadCompanies)
			{
				this.AccountingSites_RetrieveUsersCompanies(security, accountingSite);				
			}

			timer.Stop();
			return accountingSite;
		}


		#endregion
	}
}