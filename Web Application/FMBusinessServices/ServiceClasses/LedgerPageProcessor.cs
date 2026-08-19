// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedgerPageProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LedgerPageProcessorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System.Collections;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;

	public class LedgerPageProcessorClass : ILedgerPageProcessor
	{
		#region Attributes
		protected SecurityClass Security;
		#endregion Attributes

		#region Public methods
		/// <summary>
		/// This method processes the ledger page request and returns a ledger
		/// page data object.
		/// </summary>
		/// <param name="sr">The service request object.</param>
		/// <returns>A LedgerPageDO object.</returns>
		public LedgerPageDO Process ( LedgerPageSR sr, AccountingSite accountingSite = null)
		{
			this.Security = sr.Security;

			LedgerPageDO ledgerPageDO = new LedgerPageDO ( );

			if (accountingSite == null)
			{
				AccountingSites accountingSites = new AccountingSites();
				accountingSite = accountingSites.LoadSiteInfoNoCompanies(sr.Security, sr.CurrentSiteGuid);
			}

			// Must perform the add product, manager, and owner methods
			// prior to retrieving transaction data.  The reason being is
			// the default values for the above are set and then are used
			// by the transaction retrieve query. In addition, the add
			// owner will set the single owner system flag.
			StopWatch timer = new StopWatch ( StopWatch.Appnames.AccountingBLL, "LedgerPageProcessor.GettingLists" );

			CompanyLists companyLists = new CompanyLists ( );
			companyLists.Enumerate ( this.Security, false );

			this.AddProductListToDO ( ref ledgerPageDO );

			ledgerPageDO.SingleOwnerSystem	= accountingSite.CurrentSite.EnforceSingleOwner;
			ledgerPageDO.OwnerList			= companyLists.GetCompanyList ( COMPANY_ROLE.OWNER );
			ledgerPageDO.ManagerList		= companyLists.GetCompanyList ( COMPANY_ROLE.MANAGER );

			timer.Stop ( );
			this.Security = null;

			return ledgerPageDO;
		}
		#endregion

		#region Private Methods
		///// <summary>
		///// This method will retrieve the manager list.
		///// </summary>
		///// <param name="ledgerPageDo"></param>
		//private void AddManagerListToDo(ref LedgerPageDO ledgerPageDo)
		//{
		//	var companies = new CompaniesClass();
		//	CompanyCollectionClass companyCollection = companies.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, false, false);
		//	ledgerPageDo.ManagerList = new ArrayList((from company in companyCollection
		//											  select company.ID).ToList());
		//}

		///// <summary>
		///// This method will retrieve the manager list.
		///// </summary>
		///// <param name="ledgerPageDo"></param>
		//private void AddOwnerListToDo(ref LedgerPageDO ledgerPageDo)
		//{
		//	var companies = new CompaniesClass();
		//	CompanyCollectionClass companyCollection = companies.EnumerateByRole(this.Security, COMPANY_ROLE.OWNER, false, false);
		//	ledgerPageDo.OwnerList = new ArrayList((from company in companyCollection
		//											  select company.ID).ToList());
		//}

		/// <summary>
		/// This method will access the shared components for a list
		/// products associated with a site group or site.
		/// </summary>
		/// <param name="ledgerPageDO">The ledger page DO.</param>
		private void AddProductListToDO ( ref LedgerPageDO ledgerPageDO )
		{
			// Retrieve product list from shared components and
			// add to the Ledger DO.
			SitesClass sites = new SitesClass ( );
			SiteClass site = sites.GetByMemberAndProcessVariables ( this.Security, this.Security.SiteGuid, false, false );

			ProductsClass products = new ProductsClass ( );
			ProductCollectionClass productCollection = products.EnumerateByFilterAndLocalize ( this.Security, null, false );

			ArrayList productList = new ArrayList ( );

			if (productCollection != null)
			{
				foreach (ProductClass product in productCollection)
				{
					if (product.InhibitAccounting)
					{
						continue;
					}

					if (( site.EnableAdditiveAccounting == false ) && ( product.ProductType == ProductType.AdditiveProduct ))
					{
						continue;
					}

					productList.Add ( product.ID );
				}
			}

			ledgerPageDO.ProductList = productList;
		}
		#endregion Private Methods
	}
}