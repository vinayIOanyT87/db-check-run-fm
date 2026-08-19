// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Companies.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Implemtation of the companies service class and ICompanies interface.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CompaniesClass : FMServiceBase, IDependency, ICompanies
	{
		#region Fields

		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Adds the specified company object.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="company">The company.</param>
		/// <returns>The identity guid of the newly added company object.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// Security
		/// or
		/// Company
		/// </exception>
		/// <exception cref="System.Exception">
		/// Access Denied
		/// or
		/// Company Exists
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, CompanyClass company)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (company == null)
			{
				throw new ArgumentNullException(nameof(company));
			}

			if (!security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.INTERFACE_IMPORT) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, company);

			if (this.GetIdentityGuid(security, company.ID) != Guid.Empty)
			{
				throw new Exception("Company Exists");
			}

			// Update Shipper, BillTo and ShipTo types.
			this.ModifyTypes(security, company);

			company.SiteGuid = security.SiteGuid;
			company.CreatedDate = DateTimeOffset.Now;
			company.CreatedBy = security.UserID;
			company.UpdatedDate = company.CreatedDate;
			company.UpdatedBy = security.UserID;
			company.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				company.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(company);
			entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

			this.UpdateRoles(security, company, null);
			this.UpdateAuthorizedCarriers(security, company, null);
			this.UpdateCarrierCustomersShipTo(security, company, null);
			this.UpdatePersonnel(security, company, null);
			this.UpdateEquipment(security, company, null);
			this.UpdateGroups(security, company, null);

			var productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(
				security, company.IdentityGuid, company.ID, false, company.AuthorizedProductCollection, null);
			productMaps.ModifyCollection(
				security, company.IdentityGuid, company.ID, false, company.SupplierAuthorizedProductCollection, null);
			productMaps.ModifyCollection(
				security, company.IdentityGuid, company.ID, false, company.UnavailableInventoryCollection, null);

			// added (IGO 04-Sep-2008)			
			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(security, company.IdentityGuid, company.CertificateAndPermitCollection, null);

			var schedules = new SchedulesClass();
			schedules.ModifyCollection(security, company.IdentityGuid, company.AccessScheduleCollection, null);

			return company.IdentityGuid;
		}

		public CompanyCollectionClass Enumerate(SecurityClass security)
		{
			return this.EnumerateExt(security);
		}

		public CompanyCollectionClass EnumerateAllRoles(SecurityClass security, bool byGroupCompanies)
		{
			// Check security - if not valid, an exception will be thrown
			this.CheckSecurity(security);

			// Get the list of companies
			var company = new CompanyClass { SiteGuid = security.SiteGuid };

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateAllRoles(cmd, security, byGroupCompanies);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			// Start a collection class
			var companyCollection = new CompanyCollectionClass();

			// Load the collection class from the data set
			foreach (DataRow row in set.Tables[0].Rows)
			{
				company = new CompanyClass
				{
					IdentityGuid = DataObject.getValue(row["CompanyGuid"], Guid.Empty),
					MasterRecordGuid = DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
					ID = DataObject.getValue(row["ID"], string.Empty),
					Code = DataObject.getValue(row["Code"], string.Empty)
				};

				var role = new CompanyRoleMapClass
				{
					Role = DataObject.getValue(row["LookupCompanyRoleIndex"], COMPANY_ROLE.NO_COMPANY_ROLE)
				};
				company.RoleCollection.Add(role);
				companyCollection.Add(company);
			}

			return companyCollection;
		}

		public CompanyCollectionClass EnumerateAuthorizedCustomerShipToForColumnValue(
			SecurityClass security, string column, string value, Guid carrierGuid)
		{
			this.CheckSecurity(security);

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateAuthorizedCustomerShipToForColumnValueSQL(cmd, security, column, value, carrierGuid);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyCollection = new CompanyCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				company = new CompanyClass(site);
				company.Load(set);
				companyCollection.Add(company);

				table.Rows.RemoveAt(0);
			}

			return companyCollection;
		}

		public CompanyCollectionClass EnumerateAuthorizedSupplierForColumnValue(
			SecurityClass security, string column, string value)
		{
			this.CheckSecurity(security);

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				company.EnumerateAuthorizedSupplierForColumnValueSQL(cmd, security, column, value);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyCollection = new CompanyCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				company = new CompanyClass(site);
				company.Load(set);
				companyCollection.Add(company);

				table.Rows.RemoveAt(0);
			}

			return companyCollection;
		}

		public CompanyCollectionClass EnumerateByCode(SecurityClass security, string companyCode, bool bLocalize)
		{
			this.CheckSecurity(security);

			SiteClass site = null;
			if (bLocalize)
			{
				var sites = new SitesClass();
				site = sites.GetByMemberAndProcessVariables(security, security.LoginSiteGuid, false, false);
			}

			var company = new CompanyClass { SiteGuid = security.SiteGuid, Code = companyCode };

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateByCodeSQL(cmd, security);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyCollection = new CompanyCollectionClass();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				company = bLocalize ? new CompanyClass(site) : new CompanyClass();

				company.Load(row);
				companyCollection.Add(company);
			}

			return companyCollection;
		}

		public CompanyCollectionClass EnumerateByRole(SecurityClass security, COMPANY_ROLE role, bool byGroupCompanies)
		{
			return this.EnumerateByRole(security, role, byGroupCompanies, true);
		}

		public CompanyCollectionClass GetEntriesForFieldGeneratorByRole(SecurityClass security, COMPANY_ROLE role, Guid transContextCompanyGuid, Guid fuelCardGuid, bool hideHiddenCompanies = false)
		{
			CompanyCollectionClass companyCollection = new CompanyCollectionClass();

			if (fuelCardGuid != Guid.Empty)
			{
				if (transContextCompanyGuid != Guid.Empty)
				{
					CompanyClass company = this.Get(security, transContextCompanyGuid, false);

					// Don't allow the company corresponding to the fuel card to be returned if it is hidden and we are hiding hidden companies. 
					// The transaction detail screen will set the company field with the value automatically anyway (this is OK), 
					// but we still need to make sure that it doesn't pop up in the list for autocomplete controls
					if (company != null && (!company.HiddenDate.HasValue || !hideHiddenCompanies))
					{
						companyCollection.Add(company);
					}

					return companyCollection;
				}
				else
				{
					FuelCardsClass fuelC = new FuelCardsClass();
					FuelCardClass fc = fuelC.Get(security, fuelCardGuid, false);
					Guid fuelCardCompanyGuid;

					switch (role)
					{
						case COMPANY_ROLE.CUSTOMER_BILLTO:
							fuelCardCompanyGuid = fc.BillToGuid;
							break;
						case COMPANY_ROLE.CUSTOMER_SHIPTO:
							fuelCardCompanyGuid = fc.ShipToGuid;
							break;
						case COMPANY_ROLE.MANAGER:
							fuelCardCompanyGuid = fc.ManagerGuid;
							break;
						case COMPANY_ROLE.OWNER:
							fuelCardCompanyGuid = fc.OwnerGuid;
							break;
						case COMPANY_ROLE.SHIPPER:
							fuelCardCompanyGuid = fc.ShipperGuid;
							break;
						default:
							fuelCardCompanyGuid = Guid.Empty;
							break;
					}

					if (fuelCardCompanyGuid != Guid.Empty)
					{
						CompanyClass company = this.Get(security, fuelCardCompanyGuid, false);

						// Don't allow the company corresponding to the fuel card to be returned if it is hidden and we are hiding hidden companies. 
						// The transaction detail screen will set the company field with the value automatically anyway (this is OK), 
						// but we still need to make sure that it doesn't pop up in the list for autocomplete controls
						if (company != null && (!company.HiddenDate.HasValue || !hideHiddenCompanies))
						{
							companyCollection.Add(company);
						}

						return companyCollection;
					}
				}
			}

			companyCollection = this.EnumerateByRole(security, role, false, false, hideHiddenCompanies);
			return companyCollection;
		}

		public CompanyCollectionClass EnumerateByRole(
			SecurityClass security, COMPANY_ROLE role, bool byGroupCompanies, bool bLocalize, bool hideHiddenCompanies = false)
		{
			SiteClass site = null;
			if (bLocalize)
			{
				var sites = new SitesClass();
				site = sites.GetByMemberAndProcessVariables(security, security.LoginSiteGuid, false, false);
			}

			return this.EnumerateByRole(security, role, byGroupCompanies, site, hideHiddenCompanies);
		}

		public CompanyCollectionClass EnumerateByRole(
			SecurityClass security, COMPANY_ROLE role, bool byGroupCompanies, SiteClass site, bool hideHiddenCompanies = false)
		{
			this.CheckSecurity(security);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateByRoleSQL(cmd, security, role, byGroupCompanies, hideHiddenCompanies);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyCollection = new CompanyCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				company = site != null ? new CompanyClass(site) : new CompanyClass();

				company.Load(set);
				companyCollection.Add(company);

				table.Rows.RemoveAt(0);
			}

			return companyCollection;
		}

		// **********************************************************************************************************************
		// This method will return a company object collection of the companies that meet the security, role,
		// filter, and by group companies criterion. This method is the same as the EnumerateByRole method
		// with the exception that the user has supplied a filter to narrow the search on the list of companies.
		// **********************************************************************************************************************
		public CompanyCollectionClass EnumerateByRoleAndFilter(
			SecurityClass security, COMPANY_ROLE role, string filter, bool byGroupCompanies)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				company.EnumerateByRoleAndFilterSQL(cmd, security, role, filter, byGroupCompanies);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var companyCollection = new CompanyCollectionClass();

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					company = new CompanyClass();
					company.Load(set);
					companyCollection.Add(company);

					table.Rows.RemoveAt(0);
				}

				return companyCollection;
			}
		}

		/// <summary>
		/// This method will return a data set containing specific company data for the
		///	company grid on the Companies Summary page. The data is based on the role and
		///	filter settings.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="role">
		/// The role.
		/// </param>
		/// <param name="filter">
		/// </param>
		/// <param name="byGroupCompanies">
		/// The by Group Companies.
		/// </param>
		/// <param name="hideHiddenCompanies">If true, only companies that are not hidden will be shown</param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		public DataSet EnumerateByRoleAndFilterCompanyGrid(
			SecurityClass security, COMPANY_ROLE role, string filter, bool byGroupCompanies, bool hideHiddenCompanies = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);


			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			var limits = new EnumerationLimits();
			int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY);

			using (var cmd = new SqlCommand())
			{
				company.EnumerateByRoleAndFilterCompanyGridSQL(cmd, security, role, filter, byGroupCompanies, limit, hideHiddenCompanies);
				return this.ConsolidatedDA.GetDataSet(cmd, security);
			}
		}

		/// <summary>
		/// This method is called by Company Select Form to get a list on companies. The Company
		///	will only be populated with 6 columns (ID, Index, Name, City, Address1, Address2,
		///	Code, and State).  If the user has specified a filter, then the result will be
		///	filtered. Returning a dataset is must faster to marshall than a collection of objects.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="role">
		/// </param>
		/// <param name="filter">
		/// </param>
		/// <param name="hideHiddenCompanies">If true, only companies that are not marked as hidden will be returned</param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		public DataSet EnumerateByRoleAndFilterCompanySelect(SecurityClass security, COMPANY_ROLE role, string filter, bool hideHiddenCompanies = false)
		{
			return this.EnumerateByRoleAndFilterCompanySelectAndLoadType(security, role, filter, false, hideHiddenCompanies);
		}

		public DataSet EnumerateByRoleAndFilterCompanySelectAndLoadType(
			SecurityClass security, COMPANY_ROLE role, string filter, bool loadTypes, bool hideHiddenCompanies = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			var limits = new EnumerationLimits();
			company.SetSelectLimit(limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY));

			using (var cmd = new SqlCommand())
			{
				company.EnumerateByRoleAndFilterCompanySelectSQL(cmd, security, role, filter, loadTypes, hideHiddenCompanies);
				DataSet dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				return dataSet;
			}
		}

		/// <summary>
		/// This method will return a data set containing specific company data for the
		///	company grid on the Companies Summary page.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="role">
		/// </param>
		/// <param name="byGroupCompanies">
		/// </param>
		/// <param name="hideHiddenCompanies">If true, only companies that are not marked as hidden will be returned</param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		public DataSet EnumerateByRoleCompanyGrid(SecurityClass security, COMPANY_ROLE role, bool byGroupCompanies, bool hideHiddenCompanies = false)
		{
			this.CheckSecurity(security);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			var limits = new EnumerationLimits();
			int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY);

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateByRoleCompanyGridSQL(cmd, security, role, byGroupCompanies, limit, hideHiddenCompanies);
				dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return dataSet;
		}

		public CompanyCollectionClass EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(SecurityClass security, COMPANY_ROLE[] roles, bool hideHiddenCompanies = false)
		{
			this.CheckSecurity(security);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateByRoleGetIDCodeTypesIdentityGuidOnlySQL(cmd, security, roles, hideHiddenCompanies);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyCollection = new CompanyCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				company = new CompanyClass();
				company.LoadIDCodeTypesIdentityGuid(set);
				companyCollection.Add(company);

				table.Rows.RemoveAt(0);
			}

			return companyCollection;
		}

		public CompanyCollectionClass EnumerateBySite(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

			var sites = new SitesClass();
			var site = sites.GetByMemberAndProcessVariables(security, security.LoginSiteGuid, false, false);

			var company = new CompanyClass();
			var limits = new EnumerationLimits();
			company.SetSelectLimit(limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY));

			company.SiteGuid = security.SiteGuid;
			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompaniesById";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 100);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@Id"].Value = DBNull.Value;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyCollection = new CompanyCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				company = new CompanyClass(site);
				company.Load(set);
				companyCollection.Add(company);

				table.Rows.RemoveAt(0);
			}

			return companyCollection;
		}

		/// <summary>
		/// This method will enumerate companies for all sites. It will
		/// return a minimum set of columns.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns></returns>
		public DataSet EnumerateCompaniesAllSites(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

            DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_EnumerateCompaniesAllSites";

				dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return dataSet;
		}

		public string[] EnumerateColumnForAuthorizedCustomerShipTo(SecurityClass security, Guid carrierGuid, string column)
		{
			this.CheckSecurity(security);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateColumnForAuthorizedCustomerShipToSQL(cmd, security, carrierGuid, column);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var result = new string[table.Rows.Count];
			int index = 0;
			foreach (DataRow row in table.Rows)
			{
				result[index++] = DataObject.getValue(row[column], string.Empty);
			}

			return result;
		}

		public string[] EnumerateColumnForAuthorizedSupplierOffLoadID(SecurityClass security, string column)
		{
			this.CheckSecurity(security);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				company.EnumerateAuthorizedSupplierForColumnValueSQL(cmd, security, column);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var result = new string[table.Rows.Count];
			int index = 0;
			foreach (DataRow row in table.Rows)
			{
				result[index++] = DataObject.getValue(row[column], string.Empty);
			}

			return result;
		}

		/// <summary>
		/// This method is called by Company Select Form to get a list on companies. The Company
		///	will only be populated with 6 columns (ID, Index, Name, City, Address1, Address2,
		///	Code, and State). Returning a DataSet which is a known object is much faster.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="role">
		/// </param>
		/// <param name="hideHiddenCompanies">If true, only companies that are not marked as hidden will be returned</param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		public DataSet EnumerateCompanySelectRole(SecurityClass security, COMPANY_ROLE role, bool hideHiddenCompanies = false)
		{
			return this.EnumerateCompanySelectRoleByLoadTypes(security, role, false, hideHiddenCompanies);
		}

		public DataSet EnumerateCompanySelectRoleByLoadTypes(SecurityClass security, COMPANY_ROLE role, bool loadTypes, bool hideHiddenCompanies = false)
		{
			this.CheckSecurity(security);

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			var limits = new EnumerationLimits();
			company.SetSelectLimit(limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY));

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateForCompanySelectRoleSQL(cmd, security, role, loadTypes, hideHiddenCompanies);
				dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return dataSet;
		}

		/// <summary>
		/// The enumerate ext 2 prime.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetSiteGuid">
		/// The target site globally unique identifier.
		/// </param>
		/// <param name="groupCompanies">
		/// The by group companies.
		/// </param>
		/// <param name="localize">
		/// The localize.
		/// </param>
		/// <param name="getExtendedInfo">
		/// The get extended info.
		/// </param>
		/// <returns>
		/// The <see cref="CompanyCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Security is null
		/// </exception>
		public CompanyCollectionClass EnumerateExt2Prime(
  SecurityClass security, Guid targetSiteGuid, bool groupCompanies = true, bool localize = true, bool getExtendedInfo = false)
		{
			const bool GetSchedulesFlag = false;
			const bool GetMemberSites = false;
			const string SecurityString = "Security";

			if (security == null)
			{
				throw new ArgumentNullException(SecurityString);
			}

			this.CheckSecurity(security);

			SiteClass site = null;

			if (localize)
			{
				var sites = new SitesClass();
				site = sites.GetByMemberAndProcessVariables(security, security.LoginSiteGuid, GetMemberSites, GetSchedulesFlag);
			}

			var companyClass = new CompanyClass();
			var limits = new EnumerationLimits();
			companyClass.SetSelectLimit(limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY));

			companyClass.SiteGuid = targetSiteGuid;
			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				companyClass.EnumerateSQL(cmd, security, groupCompanies);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			var companyCollection = new CompanyCollectionClass();

			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				companyClass = localize ? new CompanyClass(site) : new CompanyClass();

				companyClass.Load(set);
				companyCollection.Add(companyClass);
				table.Rows.RemoveAt(0);
			}

			if (getExtendedInfo)
			{
				// ***************
				// **  Get Roles
				// ***************
				var companyRoleMaps = new CompanyRoleMapsClass();
				var compCompRoleMapCollection = companyRoleMaps.EnumerateBySiteForRoleMapping(security, targetSiteGuid);
				CompanyRoleMapCollectionClass companyRoleMapCollection = null;
				CompanyClass companyPtr = null;
				foreach (var compCompRoleMap in compCompRoleMapCollection)
				{
					if (companyPtr == null || companyPtr.MasterRecordGuid != compCompRoleMap.CompanyGuid)
					{
						if (companyPtr != null)
						{
							companyPtr.RoleCollection = companyRoleMapCollection;
						}

						companyPtr = companyCollection.FindByMasterRecordGuid(compCompRoleMap.CompanyGuid);
						companyRoleMapCollection = new CompanyRoleMapCollectionClass();
					}

					companyRoleMapCollection.Add(compCompRoleMap);
				}

				if (companyPtr != null)
				{
					companyPtr.RoleCollection = companyRoleMapCollection;
				}

				// *******************
				// ** End Get Roles
				// ********************

				// *************************************
				// **  Get Authorized Carrier
				// **************************************
				var companyMaps = new CompanyMapsClass();
				var compCompMapCollection = companyMaps.EnumerateBySiteForAuthorizedCarrierMapping(security, targetSiteGuid);
				companyPtr = null;
				CompanyMapCollectionClass companyMapCollection = null;
				foreach (CompanyMapClass compCompMap in compCompMapCollection)
				{
					if (companyPtr == null || companyPtr.IdentityGuid != compCompMap.AssignedToGuid)
					{
						if (companyPtr != null)
						{
							companyPtr.AuthorizedCarrierCollection = companyMapCollection;
						}

						companyPtr = companyCollection.FindByGuid(compCompMap.AssignedToGuid);
						companyMapCollection = new CompanyMapCollectionClass();
					}

					companyMapCollection.Add(compCompMap);
				}

				if (companyPtr != null)
				{
					companyPtr.AuthorizedCarrierCollection = companyMapCollection;
				}

				// ***********************************
				// ** End Get Authorized Carrier
				// ***********************************

				// *************************************
				// **  Get Carrier Customer ShipTo
				// **************************************
				companyMaps = new CompanyMapsClass();
				compCompMapCollection = companyMaps.EnumerateBySiteForCarrierCustomerShipToMapping(security, targetSiteGuid);
				companyPtr = null;
				companyMapCollection = null;
				foreach (CompanyMapClass compCompMap in compCompMapCollection)
				{
					if (companyPtr == null || companyPtr.IdentityGuid != compCompMap.AssignedGuid)
					{
						if (companyPtr != null)
						{
							companyPtr.CarrierCustomerShipToCollection = companyMapCollection;
						}

						companyPtr = companyCollection.FindByGuid(compCompMap.AssignedGuid);
						companyMapCollection = new CompanyMapCollectionClass();
					}

					companyMapCollection.Add(compCompMap);
				}

				if (companyPtr != null)
				{
					companyPtr.CarrierCustomerShipToCollection = companyMapCollection;
				}

				// ***********************************
				// ** End Get Carrier Customer ShipTo
				// ***********************************

				// *************************************
				// ** Get User Group
				// **************************************
				companyMaps = new CompanyMapsClass();
				compCompMapCollection = companyMaps.EnumerateBySiteForUserGroupMapping(security, targetSiteGuid);
				companyPtr = null;
				companyMapCollection = null;
				foreach (CompanyMapClass compCompMap in compCompMapCollection)
				{
					if (companyPtr == null || companyPtr.MasterRecordGuid != compCompMap.AssignedGuid)
					{
						if (companyPtr != null)
						{
							companyPtr.GroupMapCollection = companyMapCollection;
						}

						CompanyClass com = this.Get(security, compCompMap.AssignedGuid, false, true); //Company-UserGroup mappings are captured using the Company MasterRecordGuids. Need to translate those mappings to the actual site record version (MasterRecord or ChildRecordVersion) being processed in the companyCollection list.
						companyPtr = companyCollection.FindByGuid(com.IdentityGuid);
						companyMapCollection = new CompanyMapCollectionClass();
					}

					companyMapCollection.Add(compCompMap);
				}

				if (companyPtr != null)
				{
					companyPtr.GroupMapCollection = companyMapCollection;
				}

				// ***********************************
				// ** End Get User Group
				// ***********************************

				// *************************************
				// ** Get Authorized Products
				// **************************************
				var productMaps = new ProductMapsClass();
				var prodCompMapCollection = productMaps.EnumerateByType(security, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP, true);
				companyPtr = null;
				ProductMapCollectionClass productMapCollection = null;
				foreach (var prodCompMap in prodCompMapCollection)
				{
					if (companyPtr == null || companyPtr.IdentityGuid != prodCompMap.AssignedToGuid)
					{
						if (companyPtr != null)
						{
							companyPtr.AuthorizedProductCollection = productMapCollection;
						}

						companyPtr = companyCollection.FindByGuid(prodCompMap.AssignedToGuid);
						productMapCollection = new ProductMapCollectionClass();
					}

					productMapCollection.Add(prodCompMap);
				}

				if (companyPtr != null)
				{
					companyPtr.AuthorizedProductCollection = productMapCollection;
				}

				// ***********************************
				// ** End Get Authorized Products
				// ***********************************

				// *************************************
				// ** Get Supplier Authorized Products
				// **************************************
				productMaps = new ProductMapsClass();
				prodCompMapCollection = productMaps.EnumerateByType(security, PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP, true);
				companyPtr = null;
				foreach (var prodCompMap in prodCompMapCollection)
				{
					companyPtr = companyCollection.FindByGuid(prodCompMap.AssignedToGuid);
					if (companyPtr == null)
					{
						continue;
					}
					if (companyPtr.SupplierAuthorizedProductCollection == null)
					{
						companyPtr.SupplierAuthorizedProductCollection = new ProductMapCollectionClass();
					}
					companyPtr.SupplierAuthorizedProductCollection.Add(prodCompMap);
				}

				// ***********************************
				// ** End Get Supplier Authorized Products
				// ***********************************

				// *************************************
				// ** Get Unavailable Inventory
				// **************************************
				productMaps = new ProductMapsClass();
				prodCompMapCollection = productMaps.EnumerateByType(security, PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP, true);
				companyPtr = null;
				foreach (var prodCompMap in prodCompMapCollection)
				{
					companyPtr = companyCollection.FindByGuid(prodCompMap.AssignedToGuid);
					if (companyPtr == null)
					{
						continue;
					}
					if (companyPtr.UnavailableInventoryCollection == null)
					{
						companyPtr.UnavailableInventoryCollection = new ProductMapCollectionClass();
					}
					companyPtr.UnavailableInventoryCollection.Add(prodCompMap);
				}

				// ***********************************
				// ** End Get Unavailable Inventory
				// ***********************************

				// *************************************
				// ** Get Drivers
				// **************************************
				//var personnel = new PersonnelClass();
				//var personCompMapCollection = personnel.Enumerate(security);
				//companyPtr = null;
				//PersonCollectionClass personMapCollection = null;
				//foreach (var personCompMap in personCompMapCollection)
				//{
				//	if (companyPtr == null || companyPtr.IdentityGuid != personCompMap.CompanyGuid)
				//	{
				//		if (companyPtr != null)
				//		{
				//				companyPtr.PersonnelCollection = personMapCollection;
				//		}

				//		companyPtr = companyCollection.FindByMasterRecordGuid(personCompMap.CompanyGuid);
				//		personMapCollection = new PersonCollectionClass();
				//	}

				//	personMapCollection.Add(personCompMap);
				//}

				//if (companyPtr != null)
				//{
				//	companyPtr.PersonnelCollection = personMapCollection;
				//}

				// ***********************************
				// ** End Get Drivers
				// ***********************************

				// *************************************
				// ** Get Equipment
				// **************************************
				var equipments = new EquipmentsClass();
				var equipCompMapCollection = equipments.Enumerate(security);
				companyPtr = null;
				foreach (var equipCompMap in equipCompMapCollection)
				{
					companyPtr = companyCollection.FindByGuid(equipCompMap.CompanyGuid);
					if (companyPtr == null)
					{
						continue;
					}
					if (companyPtr.EquipmentCollection == null)
					{
						companyPtr.EquipmentCollection = new EquipmentCollectionClass();
					}
					companyPtr.EquipmentCollection.Add(equipCompMap);
				}

				// ***********************************
				// ** End Get Equipment
				// ***********************************

				// ****************************************************
				// ** Get Qualification Certificate and Permit
				// ****************************************************
				var qualificationMaps = new QualificationMapsClass();
				var qualCompMapCollection = qualificationMaps.EnumerateCompanyCertificateAndPermitForExport(security, targetSiteGuid);
				companyPtr = null;
				foreach (var qualCompMap in qualCompMapCollection)
				{
					companyPtr = companyCollection.FindByGuid(qualCompMap.AssigneeGuid);
					if (companyPtr == null)
					{
						continue;
					}
					if(companyPtr.CertificateAndPermitCollection == null)
                    {
						companyPtr.CertificateAndPermitCollection = new QualificationMapCollectionClass();
					}
					companyPtr.CertificateAndPermitCollection.Add(qualCompMap);
				}

				// ************************************************
				// ** End Get Qualification Certificate and Permit
				// ************************************************

				// ****************************************************
				// ** Get Company Access Schedule
				// ****************************************************
				var schedules = new SchedulesClass();
				var companyAccessScheduleCollection = schedules.EnumerateCompanyAccessType(security, targetSiteGuid);
				companyPtr = null;
				ScheduleCollectionClass schedCollection = null;
				foreach (var sched in companyAccessScheduleCollection)
				{
					if (companyPtr == null || companyPtr.IdentityGuid != sched.EntityGuid)
					{
						if (companyPtr != null)
						{
							companyPtr.AccessScheduleCollection = schedCollection;
						}

						companyPtr = companyCollection.FindByGuid(sched.EntityGuid);
						schedCollection = new ScheduleCollectionClass();
					}

					schedCollection.Add(sched);
				}

				if (companyPtr != null)
				{
					companyPtr.AccessScheduleCollection = schedCollection;
				}

				foreach (var company in companyCollection.Where(company => company.HasRole(COMPANY_ROLE.CARRIER) && company.AccessScheduleCollection.Count == 0))
				{
					DAY_OF_WEEK[] dayOfWeek =
						{
							DAY_OF_WEEK.SUNDAY, DAY_OF_WEEK.MONDAY, DAY_OF_WEEK.TUESDAY, DAY_OF_WEEK.WEDNESDAY,
							DAY_OF_WEEK.THURSDAY, DAY_OF_WEEK.FRIDAY, DAY_OF_WEEK.SATURDAY
						};

					for (var item = 0; item < 7; item++)
					{
						var schedule = new ScheduleClass
						{
							Type = SCHEDULE_TYPE.COMPANY_ACCESS_TYPE,
							Day = (int)dayOfWeek[item],
							Enabled = true,
							EndOfDayEnabled = false
						};

						company.AccessScheduleCollection.Add(schedule);
					}
				}

				// ************************************************
				// ** End Get Company Access Schedule
				// ************************************************
			}

			return companyCollection;
		}

		/// <summary>
		/// The get extended info for company object.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="company">
		/// The company from the companies table.
		/// </param>
		/// <param name="hideHiddenProducts">If true, only products that are not hidden will be returned as part of the product collections</param>
		private void GetExtendedInfo(SecurityClass security, ref CompanyClass company, bool hideHiddenProducts = false)
		{
			var companyRoleMaps = new CompanyRoleMapsClass();
			company.RoleCollection = companyRoleMaps.EnumerateByCompany(security, company.MasterRecordGuid);

			// CompanyRoles are maintained for each AssignedTo sites separately from RecordVersioning, using a combination of the MasterRecordGuid and the AssignedToSiteGuid.
			var companyMaps = new CompanyMapsClass();
			var productMaps = new ProductMapsClass();

			if (company.HasRole(COMPANY_ROLE.CUSTOMER_SHIPTO))
			{
				company.AuthorizedCarrierCollection = companyMaps.EnumerateByAssignedToGuidAndType(
					security, company.IdentityGuid, COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
				company.AuthorizedProductCollection = productMaps.EnumerateByAssignedToGuidAndTypeInstr(
					security, company.IdentityGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP, true, hideHiddenProducts);
			}
			else
			{
				company.AuthorizedCarrierCollection = new CompanyMapCollectionClass();
				company.AuthorizedProductCollection = new ProductMapCollectionClass();
			}

			if (company.HasRole(COMPANY_ROLE.CARRIER))
			{
				company.CarrierCustomerShipToCollection = companyMaps.EnumerateByAssignedGuidAndType(
					security, company.IdentityGuid, COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
				company.AssignedPersonnelCollection = companyMaps.EnumerateByAssignedGuidAndType(
					security, company.IdentityGuid, COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY);
			}
			else
			{
				company.CarrierCustomerShipToCollection = new CompanyMapCollectionClass();
				company.AssignedPersonnelCollection = new CompanyMapCollectionClass();
			}
			company.AssignedPersonnelCollection.Sort(COMPANY_MAP_SORT_CRITERIA.ASSIGNEDTO);

			company.GroupMapCollection = companyMaps.EnumerateByAssignedGuidAndType(
				security, company.IdentityGuid, COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);

			company.SupplierAuthorizedProductCollection = company.HasRole(COMPANY_ROLE.SUPPLIER)
				? productMaps.EnumerateByAssignedToGuidAndTypeInstr(
						security, company.IdentityGuid, PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP, true, hideHiddenProducts)
				: new ProductMapCollectionClass();

			company.UnavailableInventoryCollection = company.HasRole(COMPANY_ROLE.OWNER)
				? productMaps.EnumerateByAssignedToGuidAndTypeInstr(
						security, company.IdentityGuid, PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP, true, hideHiddenProducts)
				: new ProductMapCollectionClass();

			// added (IGO 04-Sep-2008)

			// Add Products Assigned To Company Groups to which this company is assigned
			if (company.HasRole(COMPANY_ROLE.CUSTOMER_SHIPTO))
			{
				var companyMapCollection = companyMaps.EnumerateByAssignedGuidAndType(
					security, company.MasterRecordGuid, COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP);

				foreach (var authorizedProduct in companyMapCollection.Select(companyMap => productMaps.EnumerateByAssignedToGuidAndTypeInstr(
					security, companyMap.AssignedToGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP, true, hideHiddenProducts)).SelectMany(authorizedProductCollection => authorizedProductCollection))
				{
					var productInCollection = company.AuthorizedProductCollection.Any(prodMap => prodMap.AssignedID == authorizedProduct.AssignedID);
					if (!productInCollection)
					{
						company.AuthorizedProductCollection.Add(authorizedProduct);
					}
				}
			}

			var equipments = new EquipmentsClass();
			company.EquipmentCollection = equipments.EnumerateByCompany(security, company.IdentityGuid);

			var qualificationMaps = new QualificationMapsClass();
			company.CertificateAndPermitCollection = qualificationMaps.EnumerateByGuidAndType(
				security, company.IdentityGuid, QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY, false);

			// Get possible access schedule for the selected company. If the company is associated
			// to a carrier role and it does not have an access schedule, then create an access
			// schedule and associated it to the company object.
			if (!company.HasRole(COMPANY_ROLE.CARRIER)
				&& !company.HasRole(COMPANY_ROLE.SUPPLIER))
			{
				return;
			}

			var schedules = new SchedulesClass();
			company.AccessScheduleCollection = schedules.EnumerateByEntityGuidAndType(
				security, company.IdentityGuid, SCHEDULE_TYPE.COMPANY_ACCESS_TYPE);

			if (company.AccessScheduleCollection.Count != 0)
			{
				return;
			}

			DAY_OF_WEEK[] dayOfWeek =
				{
					DAY_OF_WEEK.SUNDAY, DAY_OF_WEEK.MONDAY, DAY_OF_WEEK.TUESDAY, DAY_OF_WEEK.WEDNESDAY,
					DAY_OF_WEEK.THURSDAY, DAY_OF_WEEK.FRIDAY, DAY_OF_WEEK.SATURDAY
				};

			for (var item = 0; item < 7; item++)
			{
				var schedule = new ScheduleClass
				{
					Type = SCHEDULE_TYPE.COMPANY_ACCESS_TYPE,
					Day = (int)dayOfWeek[item],
					Enabled = true,
					EndOfDayEnabled = false
				};

				company.AccessScheduleCollection.Add(schedule);
			}
		}


		public CompanyCollectionClass EnumerateExt2(
  SecurityClass security, Guid targetSiteGuid, bool byGroupCompanies = true, bool bLocalize = true, bool getExtendedInfo = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

			SiteClass site = null;

			if (bLocalize)
			{
				var sites = new SitesClass();
				site = sites.GetByMemberAndProcessVariables(security, security.LoginSiteGuid, false, false);
			}

			var company = new CompanyClass();
			var limits = new EnumerationLimits();
			company.SetSelectLimit(limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY));

			company.SiteGuid = targetSiteGuid;
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateSQL(cmd, security, byGroupCompanies);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyCollection = new CompanyCollectionClass();

			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				company = bLocalize ? new CompanyClass(site) : new CompanyClass();

				company.Load(set);
				if (getExtendedInfo)
				{
					this.GetExtendedInfo(security, ref company);
				}
				companyCollection.Add(company);

				table.Rows.RemoveAt(0);
			}
			return companyCollection;
		}

		public CompanyCollectionClass EnumerateExtPrime(
	SecurityClass security, bool byGroupCompanies = true, bool bLocalize = true, bool getExtendedInfo = false)
		{
			return this.EnumerateExt2Prime(security, security.SiteGuid, byGroupCompanies, bLocalize, getExtendedInfo);
		}

		public CompanyCollectionClass EnumerateExt(
			SecurityClass security, bool byGroupCompanies = true, bool bLocalize = true, bool getExtendedInfo = false)
		{
			return this.EnumerateExt2(security, security.SiteGuid, byGroupCompanies, bLocalize, getExtendedInfo);
		}

		public CompanyCollectionClass EnumerateHierarchialCustomerFromRole(
			SecurityClass security,
			COMPANY_ROLE role,
			string managerString,
			string ownerString,
			string shipperString,
			string billToString,
			string filter)
		{
			this.CheckSecurity(security);
			Guid identityGuid = Guid.Empty;
			Guid identityGuid1;
			Guid identityGuid2;

			var company = new CompanyClass { SiteGuid = security.SiteGuid };

			// get the guid of the passed in companies
			if (role == COMPANY_ROLE.OWNER)
			{
				identityGuid = this.GetIdentityGuid(security, managerString);
			}
			else if (role == COMPANY_ROLE.SHIPPER)
			{
				identityGuid1 = this.GetIdentityGuid(security, managerString);
				identityGuid2 = this.GetIdentityGuid(security, ownerString);
				var companyMaps = new CompanyMapsClass();
				identityGuid = companyMaps.GetIdentityGuidByGuidsAndType(
					security, identityGuid1, identityGuid2, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
			}
			else if (role == COMPANY_ROLE.CUSTOMER_BILLTO)
			{
				identityGuid1 = this.GetIdentityGuid(security, managerString);
				identityGuid2 = this.GetIdentityGuid(security, ownerString);
				var companyMaps = new CompanyMapsClass();
				identityGuid = companyMaps.GetIdentityGuidByGuidsAndType(
					security, identityGuid1, identityGuid2, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
				identityGuid1 = this.GetIdentityGuid(security, shipperString);
				identityGuid2 = companyMaps.GetIdentityGuidByGuidsAndType(
					security, identityGuid, identityGuid1, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
				identityGuid = identityGuid2;
			}
			else if (role == COMPANY_ROLE.CUSTOMER_SHIPTO)
			{
				identityGuid1 = this.GetIdentityGuid(security, managerString);
				identityGuid2 = this.GetIdentityGuid(security, ownerString);
				var companyMaps = new CompanyMapsClass();
				identityGuid = companyMaps.GetIdentityGuidByGuidsAndType(
					security, identityGuid1, identityGuid2, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
				identityGuid1 = this.GetIdentityGuid(security, shipperString);
				identityGuid2 = companyMaps.GetIdentityGuidByGuidsAndType(
					security, identityGuid, identityGuid1, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
				identityGuid = this.GetIdentityGuid(security, billToString);
				identityGuid1 = companyMaps.GetIdentityGuidByGuidsAndType(
					security, identityGuid2, identityGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
				identityGuid = identityGuid1;
			}

			using (var cmd = new SqlCommand())
			{
				company.EnumerateHierarchialCustomerFromRoleSQL(cmd, security, role, identityGuid, filter);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var companyCollection = new CompanyCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					company = new CompanyClass();

					company.Load(set);
					companyCollection.Add(company);

					table.Rows.RemoveAt(0);
				}

				return companyCollection;
			}
		}

		/// <summary>
		/// This method will return a dataset of companies based on the criterion. Returning
		///	a dataset is much faster than a collection of company objects.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="role">
		/// </param>
		/// <param name="managerString">
		/// </param>
		/// <param name="ownerString">
		/// </param>
		/// <param name="shipperString">
		/// </param>
		/// <param name="billToString">
		/// </param>
		/// <param name="filter">
		/// </param>
		/// <param name="hideHiddenCompanies">If true, only companies that are not marked as hidden will be returned</param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		public DataSet EnumerateHierarchialCustomerFromRoleCompanySelect(
			SecurityClass security,
			COMPANY_ROLE role,
			string managerString,
			string ownerString,
			string shipperString,
			string billToString,
			string filter,
			bool hideHiddenCompanies = false)
		{
			this.CheckSecurity(security);
			Guid identityGuid = Guid.Empty;
			Guid identityGuid1;
			Guid identityGuid2;

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			var limits = new EnumerationLimits();
			company.SetSelectLimit(limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY));

			var companyMaps = new CompanyMapsClass();

			// get the guid of the passed in companies
			switch (role)
			{
				case COMPANY_ROLE.OWNER:
					identityGuid = this.GetMasterRecordGuid(security, managerString);

					break;
				case COMPANY_ROLE.SHIPPER:
					identityGuid1 = this.GetMasterRecordGuid(security, managerString);
					identityGuid2 = this.GetMasterRecordGuid(security, ownerString);

					identityGuid = companyMaps.GetIdentityGuidByGuidsAndType(security, identityGuid1, identityGuid2, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);

					break;
				case COMPANY_ROLE.CUSTOMER_BILLTO:
					identityGuid1 = this.GetMasterRecordGuid(security, managerString);
					identityGuid2 = this.GetMasterRecordGuid(security, ownerString);

					identityGuid = companyMaps.GetIdentityGuidByGuidsAndType(security, identityGuid1, identityGuid2, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);

					identityGuid1 = this.GetMasterRecordGuid(security, shipperString);
					identityGuid2 = companyMaps.GetIdentityGuidByGuidsAndType(security, identityGuid, identityGuid1, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);

					identityGuid = identityGuid2;
					break;
				case COMPANY_ROLE.CUSTOMER_SHIPTO:
					identityGuid1 = this.GetMasterRecordGuid(security, managerString);
					identityGuid2 = this.GetMasterRecordGuid(security, ownerString);

					identityGuid = companyMaps.GetIdentityGuidByGuidsAndType(security, identityGuid1, identityGuid2, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);

					identityGuid1 = this.GetMasterRecordGuid(security, shipperString);
					identityGuid2 = companyMaps.GetIdentityGuidByGuidsAndType(security, identityGuid, identityGuid1, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);

					identityGuid = this.GetMasterRecordGuid(security, billToString);
					identityGuid1 = companyMaps.GetIdentityGuidByGuidsAndType(security, identityGuid2, identityGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
					identityGuid = identityGuid1;

					break;
				case COMPANY_ROLE.SUPPLIER:
					identityGuid1 = this.GetMasterRecordGuid(security, managerString);
					identityGuid2 = this.GetMasterRecordGuid(security, ownerString);

					identityGuid = companyMaps.GetIdentityGuidByGuidsAndType(security, identityGuid1, identityGuid2, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP);

					break;
				default:
					break;
			}

			using (var cmd = new SqlCommand())
			{
				company.EnumerateHierarchialCustomerFromRoleSQL(cmd, security, role, identityGuid, filter, hideHiddenCompanies);
				DataSet dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				return dataSet;
			}
		}

		public CompanyCollectionClass EnumerateUndelegated(SecurityClass security)
		{
			var companyCollection = new CompanyCollectionClass();
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetUndelegatedCompanies";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count > 0)
			{
				while (set.Tables[0].Rows.Count != 0)
				{
					DataRow row = set.Tables[0].Rows[0];
					var company = new CompanyClass
					{
						IdentityGuid = DataObject.getValue(row["CompanyGuid"], Guid.Empty),
						MasterRecordGuid =
												DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
						ID = DataObject.getValue(row["Id"], string.Empty),
						SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
						AssignedToSiteGuid =
												DataObject.getValue(row["SiteGuid"], Guid.Empty),
						AssignedFromSiteGuid =
												DataObject.getValue(row["AssignedFromSiteGuid"], Guid.Empty),
						AssignedFromSiteId =
												DataObject.getValue(row["AssignedFromSiteId"], string.Empty)
					};

					// This query is limited to master records, i.e. SiteOwner, AssignedFromSite, and AssignedToSite are the same.
					companyCollection.Add(company);
					set.Tables[0].Rows.RemoveAt(0);
				}
			}

			return companyCollection;
		}

		public void ExcelImportImpl(SecurityClass security, object instance)
		{
			var classInstance = instance as CompanyClass;
			if (classInstance != null)
			{
				this.Import(security, classInstance);
			}
		}

		public CompanyCollectionClass GetLoadRackCompanyClasses(SecurityClass security, CompanyMapCollectionClass companyMapClassCollection, Guid shipToBillToMapGuid, DateTimeOffset siteTimeNow, bool getExtendedInfo = true, bool hideHiddenProducts = false)
		{
			// returned order shipto,billto,shipper,owner,manager
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.LoginSiteGuid, false, false);

			CompanyCollectionClass companyCollectionClass = new CompanyCollectionClass();
			// shipto
			DataSet set;
			using (var cmd = new SqlCommand())
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompanyByGuid";

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@CompanyGuid"].Value = shipToBillToMapGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var shiptocompany = new CompanyClass(site);

			shiptocompany.Load(set);

			var companyMaps = new CompanyMapsClass();
			var productMaps = new ProductMapsClass();

			if (getExtendedInfo)
			{
				this.GetExtendedInfo(security, ref shiptocompany, hideHiddenProducts);
			}
			else
			{
				shiptocompany.AuthorizedCarrierCollection = companyMaps.EnumerateByAssignedToGuidAndType(
									security, shiptocompany.IdentityGuid, COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
				shiptocompany.AuthorizedProductCollection = productMaps.EnumerateByAssignedToGuidAndTypeInstr(
					security, shiptocompany.IdentityGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP, true, hideHiddenProducts);
			}
			shiptocompany._LastActivityDate.Value = siteTimeNow;
			companyCollectionClass.Add(shiptocompany);

			// billto
			using (var cmd = new SqlCommand())
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompanyByGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@CompanyGuid"].Value = companyMapClassCollection[0].AssignedGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			var billtocompany = new CompanyClass(site);

			billtocompany.Load(set);

			if (getExtendedInfo)
			{
				this.GetExtendedInfo(security, ref billtocompany, hideHiddenProducts);
			}

			billtocompany._LastActivityDate.Value = siteTimeNow;
			companyCollectionClass.Add(billtocompany);

			// shipper
			using (var cmd = new SqlCommand())
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompanyByGuid";

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@CompanyGuid"].Value = companyMapClassCollection[1].AssignedGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			var shippertocompany = new CompanyClass(site);

			shippertocompany.Load(set);

			if (getExtendedInfo)
			{
				this.GetExtendedInfo(security, ref shippertocompany, hideHiddenProducts);
			}

			shippertocompany._LastActivityDate.Value = siteTimeNow;
			companyCollectionClass.Add(shippertocompany);

			// owner
			using (var cmd = new SqlCommand())
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompanyByGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@CompanyGuid"].Value = companyMapClassCollection[2].AssignedGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			var ownercompany = new CompanyClass(site);

			ownercompany.Load(set);

			if (getExtendedInfo)
			{
				this.GetExtendedInfo(security, ref ownercompany, hideHiddenProducts);
			}

			ownercompany._LastActivityDate.Value = siteTimeNow;
			companyCollectionClass.Add(ownercompany);

			// manager
			using (var cmd = new SqlCommand())
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompanyByGuid";

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@CompanyGuid"].Value = companyMapClassCollection[2].AssignedToGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			var managercompany = new CompanyClass(site);

			managercompany.Load(set);

			if (getExtendedInfo)
			{
				this.GetExtendedInfo(security, ref managercompany, hideHiddenProducts);
			}

			managercompany._LastActivityDate.Value = siteTimeNow;
			companyCollectionClass.Add(managercompany);

			// set the time and store back in the database
			foreach(CompanyClass company in companyCollectionClass)
			{
				this.Modify(security, DATA_TYPE.DYNAMIC, company);
			}

			return companyCollectionClass;
		}

		public CompanyClass Get(SecurityClass security, Guid companyGuid, bool getExtendedInfo = true, bool hideHiddenProducts = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.LoginSiteGuid, false, false);

			var company = new CompanyClass(site);

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompanyByGuid";

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@CompanyGuid"].Value = companyGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			company.Load(set);

			if (getExtendedInfo)
			{
				this.GetExtendedInfo(security, ref company, hideHiddenProducts);
			}

			return company;
		}

		public CompanyClass GetBasicInfo(SecurityClass security, Guid companyGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompanyByGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
				cmd.Parameters["@CompanyGuid"].Value = companyGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataRow row = set.Tables[0].Rows[0];
			var company = new CompanyClass
			{
				IdentityGuid = DataObject.getValue(row["CompanyGuid"], Guid.Empty),
				MasterRecordGuid =
										DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
				ID = DataObject.getValue(row["Id"], string.Empty),
				SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty)
			};

			return company;
		}

		public CompanyClass GetById(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

			if (id == "{All}" || id == "{Unassigned}" || id == "{None}")
			{
				return null;
			}

			var company = new CompanyClass { ID = id };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				// Company.SelectByIDSQL(security, cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompaniesById";

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 100);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@Id"].Value = id;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			company.Load(set);
			return company;
		}

		/// <summary>
		/// This method will return a list of company Guids.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="byGroupCompanies">
		/// </param>
		/// <param name="localize">
		/// The b Localize.
		/// </param>
		/// <returns>
		/// The <see cref="ArrayList"/>.
		/// </returns>
		[TransactionFlow(TransactionFlowOption.Allowed)]
		public List<Guid> GetCompanyGuidList(SecurityClass security, bool byGroupCompanies, bool localize)
		{
			var companyGuids = new List<Guid>();
			const bool GetSchedulesFlag = false;

			this.CheckSecurity(security);

			if (localize)
			{
				var sites = new SitesClass();
				sites.Get(security, security.LoginSiteGuid, GetSchedulesFlag);
			}

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateSQLIDCodeIdentityGuidOnly(cmd, security, byGroupCompanies);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			foreach (DataRow row in set.Tables[0].Rows)
			{
				Guid companyGuid = DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty);
				companyGuids.Add(companyGuid);
			}

			return companyGuids;
		}

		/// <summary>
		/// This method will return a list of company IDs.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="byGroupCompanies">
		/// </param>
		/// <param name="bLocalize">
		/// The b Localize.
		/// </param>
		/// <returns>
		/// The <see cref="ArrayList"/>.
		/// </returns>
		[TransactionFlow(TransactionFlowOption.Allowed)]
		public ArrayList GetCompanyIDList(SecurityClass security, bool byGroupCompanies, bool bLocalize)
		{
			var companyIDs = new ArrayList();

			this.CheckSecurity(security);

			if (bLocalize)
			{
				var sites = new SitesClass();
				sites.Get(security, security.LoginSiteGuid, false);
			}

			var company = new CompanyClass { SiteGuid = security.SiteGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				company.EnumerateSQLIDCodeIdentityGuidOnly(cmd, security, byGroupCompanies);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			foreach (DataRow row in set.Tables[0].Rows)
			{
				string companyID = DataObject.getValue(row["ID"], string.Empty);
				companyIDs.Add(companyID);
			}

			return companyIDs;
		}

		public string GetCompanyToolTip(SecurityClass security, string id)
		{
			string result = string.Empty;
			CompanyClass company = this.GetById(security, id);
			if (company != null)
			{
				result = company.CompanyToolTip;
			}

			return result;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			Guid result = Guid.Empty;
			CompanyClass company = this.GetById(security, id);
			if (company != null)
			{
				result = company.IdentityGuid;
			}

			return result;
		}

		public Guid GetMasterRecordGuid(SecurityClass security, string id)
		{
			Guid result = Guid.Empty;
			CompanyClass company = this.GetById(security, id);
			if (company != null)
			{
				result = company.MasterRecordGuid;
			}

			return result;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass securityParam, CompanyClass company)
		{
			if (securityParam == null)
			{
				throw new ArgumentNullException(nameof(securityParam));
			}

			if (company == null)
			{
				throw new ArgumentNullException(nameof(company));
			}

			SecurityClass security = securityParam.Clone();

			var products = new ProductsClass();
			var additiveProfiles = new AdditiveProfiles();
			var qualifications = new QualificationsClass();
			var schedules = new SchedulesClass();
			var groups = new GroupsClass();
			var equipments = new EquipmentsClass();

			var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			try
			{
				CompanyClass existingCompany = this.GetById(security, company.ID);

				// If the entity exists and is not owned by this site, do not update it.
				if (existingCompany == null || 
					(existingCompany.IdentityGuid != Guid.Empty && existingCompany.SiteGuid != security.SiteGuid))
				{
					return;
				}

				company.IdentityGuid = existingCompany.IdentityGuid;
				company.MasterRecordGuid = existingCompany.MasterRecordGuid;

				foreach (CompanyMapClass authorizedCarrier in company.AuthorizedCarrierCollection)
				{
					Guid identityGuid = this.GetIdentityGuid(security, authorizedCarrier.AssignedID);
					if (identityGuid == Guid.Empty)
					{
						var carrier = new CompanyClass(site) { ID = authorizedCarrier.AssignedID };
						var role = new CompanyRoleMapClass { Role = COMPANY_ROLE.CARRIER };
						carrier.RoleCollection.Add(role);
						identityGuid = this.Add(security, carrier);
					}

					authorizedCarrier.AssignedGuid = identityGuid;
				}

				foreach (CompanyMapClass authorizedCustomer in company.CarrierCustomerShipToCollection)
				{
					Guid identityGuid = this.GetIdentityGuid(security, authorizedCustomer.AssignedToID);
					if (identityGuid == Guid.Empty)
					{
						var customer = new CompanyClass(site) { ID = authorizedCustomer.AssignedToID };
						var role = new CompanyRoleMapClass { Role = COMPANY_ROLE.CUSTOMER_SHIPTO };
						customer.RoleCollection.Add(role);
						identityGuid = this.Add(security, customer);
					}

					authorizedCustomer.AssignedToGuid = identityGuid;
				}

				var personnel = new PersonnelClass();
				foreach (CompanyMapClass assignedPerson in company.AssignedPersonnelCollection)
				{
					assignedPerson.AssignedToGuid = personnel.GetGuidByID(security, assignedPerson.AssignedToID);
					if (assignedPerson.AssignedToGuid.IsEmpty())
					{
						var person = new PersonClass { ID = assignedPerson.AssignedToID };
						assignedPerson.AssignedToGuid = personnel.Add(security, person);
					}
				}

				foreach (EquipmentClass equipment in company.EquipmentCollection)
				{
					equipment.IdentityGuid = equipments.GetIdentityGuid(security, equipment.ID);
					if (equipment.IdentityGuid == Guid.Empty)
					{
						equipment.IdentityGuid = equipments.Add(security, equipment);
					}
				}

				foreach (ProductMapClass authorizedProduct in company.AuthorizedProductCollection)
				{
					Guid identityGuid = products.GetIdentityGuid(security, authorizedProduct.AssignedID);
					if (identityGuid == Guid.Empty)
					{
						var product = new ProductClass(site)
						{
							ID = authorizedProduct.AssignedID,
							ProductType = ProductType.ComponentProduct
						};
						identityGuid = products.Add(security, product);
					}

					authorizedProduct.AssignedGuid = identityGuid;
					authorizedProduct.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP;

					if (string.IsNullOrEmpty(authorizedProduct.AdditiveProfileID) == false)
					{
						identityGuid = additiveProfiles.GetIdentityGuid(security, authorizedProduct.AdditiveProfileID);
						if (identityGuid == Guid.Empty)
						{
							var additiveProfile = new AdditiveProfileClass { ID = authorizedProduct.AdditiveProfileID };
							identityGuid = additiveProfiles.Add(security, additiveProfile);
						}

						authorizedProduct.AdditiveProfileGuid = identityGuid;
					}
				}

				foreach (ProductMapClass supplierAuthorizedProduct in company.SupplierAuthorizedProductCollection)
				{
					Guid identityGuid = products.GetIdentityGuid(security, supplierAuthorizedProduct.AssignedID);
					if (identityGuid == Guid.Empty)
					{
						var product = new ProductClass(site)
						{
							ID = supplierAuthorizedProduct.AssignedID,
							ProductType = ProductType.ComponentProduct
						};
						identityGuid = products.Add(security, product);
					}

					supplierAuthorizedProduct.AssignedGuid = identityGuid;
					supplierAuthorizedProduct.Type = PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP;
				}

				// added (IGO 04-Sep-2008)
				foreach (ProductMapClass unavailableInventory in company.UnavailableInventoryCollection)
				{
					Guid identityGuid = products.GetIdentityGuid(security, unavailableInventory.AssignedID);
					if (identityGuid == Guid.Empty)
					{
						var product = new ProductClass(site)
						{
							ID = unavailableInventory.AssignedID,
							ProductType = ProductType.ComponentProduct
						};
						identityGuid = products.Add(security, product);
					}

					unavailableInventory.AssignedGuid = identityGuid;
					unavailableInventory.Type = PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP;
				}

				foreach (QualificationMapClass certificateAndPermit in company.CertificateAndPermitCollection)
				{
					Guid qualificationGuid = qualifications.GetIdentityGuid(
						security, QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT, certificateAndPermit.ID);
					if (qualificationGuid.IsEmpty())
					{
						var qualification = new QualificationClass
						{
							ID = certificateAndPermit.ID,
							Type = QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT
						};
						qualificationGuid = qualifications.Add(security, qualification);
					}

					certificateAndPermit.AssignedGuid = qualificationGuid;
					certificateAndPermit.Type = QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY;
				}

				// Check the security groups assigned and remove any that do not exist in this system
				for (int index = company.GroupMapCollection.Count - 1; index >= 0; --index)
				{
					CompanyMapClass companyMap = company.GroupMapCollection[index];

					if (string.IsNullOrEmpty(companyMap.AssignedToID)
						|| (companyMap.AssignedToGuid = groups.GetIdentityGuid(security, companyMap.AssignedToID)) == Guid.Empty)
					{
						company.GroupMapCollection.RemoveAt(index);
					}
				}

				if (company.IdentityGuid == Guid.Empty)
				{
					this.Add(security, company);
				}
				else
				{
					foreach (ScheduleClass schedule in company.AccessScheduleCollection)
					{
						schedule.IdentityGuid = schedules.GetIdentityGuid(security, company.IdentityGuid, schedule);
					}

					this.Modify(security, DATA_TYPE.CONFIG, company);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("[Company Import Error ID] : " + company.ID + ", " + ex.Message);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DATA_TYPE type, CompanyClass company)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (company == null)
			{
				throw new ArgumentNullException(nameof(company));
			}

			if (!security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (type == DATA_TYPE.CONFIG)
			{
				CompanyClass oldCompany = this.Get(security, company.IdentityGuid);

				Guid identityGuid = this.GetIdentityGuid(security, company.ID);

				// Remove Authorized Products from Company Groups
				var newAuthorizedProductCollection = new ProductMapCollectionClass();
				foreach (ProductMapClass newAuthorizedProduct in company.AuthorizedProductCollection)
				{
					if (newAuthorizedProduct.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP)
					{
						newAuthorizedProductCollection.Add(newAuthorizedProduct);
					}
				}

				company.AuthorizedProductCollection = newAuthorizedProductCollection;

				this.Validate(security, company);

                
                if (identityGuid != Guid.Empty && identityGuid != company.IdentityGuid)
                {
                    throw new Exception("Company Exists");
                }

                
                if (oldCompany.IdentityGuid == Guid.Empty)
                {
                    throw new Exception("Company Not Found");
                }

				// Update Shipper, BillTo and ShipTo types.
				this.ModifyTypes(security, company);

				// Set UserData(list type) to defaults if they are blanks
				UserDataFieldsClass.SetDefaults(security, company.UserData, ENTITY_TYPE.COMPANY);

                // Remove Authorized Products from Company Groups
                var oldAuthorizedProductCollection = new ProductMapCollectionClass();
                foreach (ProductMapClass oldAuthorizedProduct in oldCompany.AuthorizedProductCollection)
                {
                    if (oldAuthorizedProduct.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP)
                    {
                        oldAuthorizedProductCollection.Add(oldAuthorizedProduct);
                    }
                }

				oldCompany.AuthorizedProductCollection = oldAuthorizedProductCollection;

				// Check for Locked Out
				if (oldCompany.LockedOut != company.LockedOut && company.LockedOut)
				{
					var alarmAndEventLogs = new AlarmAndEventLogsClass();
					alarmAndEventLogs.Add(security, company.LockOutEvent);
				}

				company.UpdatedDate = DateTimeOffset.Now;
				company.UpdatedBy = security.UserID;

				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
				if (company.SiteGuid != oldCompany.SiteGuid)
				{
					entityToSiteMaps.PurgeAll(security, ENTITY_TYPE.COMPANY, company.MasterRecordGuid);
				}

				using (var cmd = new SqlCommand())
				{
					company.UpdateSQL(cmd, type);
					this.ConsolidatedDA.ExecuteQuery(security, cmd);
				}

				EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
					security, company.EntityType, company.IdentityGuid);

				if (company.SiteGuid != oldCompany.SiteGuid)
				{
					// When we change ownership of a company from a child site to a parent site, we maintain 
					// the site assignment at the child site level but change ownership to the parent site.
					// When we change ownership of a company from a parent site to a child site, we change the ownership only
					// So, if we're changing ownership from a child site to a parent site, we want to maintain the roles that
					// existed at the child site because the assignment of the company to the child site will be preserved
					SitesClass sites = new SitesClass();
					SiteCollectionClass siteCollection = sites.EnumerateByChildSite(security, security.SiteGuid);

					if (siteCollection.Find(
							matchingSite =>
							matchingSite.IdentityGuid != security.SiteGuid
							&& matchingSite.IdentityGuid == company.SiteGuid) == null)
					{
						// Delete the company roles assigned at the current site.
						this.UpdateRoles(security, null, oldCompany);
					}

					oldCompany.RoleCollection = new CompanyRoleMapCollectionClass();

					// Create Entity to Site Map
					var newEntityToSiteMap = new EntityToSiteMapClass(company);
					Guid currentSiteContext = security.SiteGuid;
					//When changing ownership of an entity that supports Cascading Assignment, need to make sure that the base mapping is created with the AssignedFromSiteGuid being the same as the Owner Site Guid (and the AssignedToSiteGuid), and not be set with the Site Context Guid which in the case of a Change of Ownership would be different from the new Owner Site Guid.
					//The Security SiteGuid swap below effectively does so by supplying the EntityToSiteMaps.Add() operation with the correct SiteGuid to use to set the AssignedFromSiteGuid.
					security.SiteGuid = company.SiteGuid;
					entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
					security.SiteGuid = currentSiteContext;
				}

				// Verify that new ID will not conflict with EntityToSiteMaps
				else
				{
					foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
					{
						Guid siteGuid = security.SiteGuid;
						security.SiteGuid = entityToSiteMap.SiteGuid;
						CompanyClass compObj = this.GetById(security, company.ID);
						security.SiteGuid = siteGuid;
						if ((compObj != null) && (compObj.IdentityGuid != Guid.Empty)
							&& (compObj.MasterRecordGuid != entityToSiteMap.IdentityGuid))
						{
							throw new Exception("Company Exists");
						}
					}
				}

				if (!company.HasRole(COMPANY_ROLE.CUSTOMER_SHIPTO))
				{
					company.AuthorizedProductCollection.Clear();
					company.AuthorizedCarrierCollection.Clear();
				}

				// added (IGO 04-Sep-2008)
				if (!company.HasRole(COMPANY_ROLE.OWNER))
				{
					company.UnavailableInventoryCollection.Clear();
				}

				if (!company.HasRole(COMPANY_ROLE.CARRIER) && !company.HasRole(COMPANY_ROLE.SUPPLIER))
				{
					company.AccessScheduleCollection.Clear();
					company.SupplierAuthorizedProductCollection.Clear();
				}

				this.UpdateRoles(security, company, oldCompany);
				this.UpdateAuthorizedCarriers(security, company, oldCompany);
				this.UpdateCarrierCustomersShipTo(security, company, oldCompany);
				this.UpdateGroups(security, company, oldCompany);

				var productMaps = new ProductMapsClass();
				productMaps.ModifyCollection(
					security,
					company.IdentityGuid,
					company.ID,
					false,
					company.AuthorizedProductCollection,
					oldCompany.AuthorizedProductCollection);
				productMaps.ModifyCollection(
					security,
					company.IdentityGuid,
					company.ID,
					false,
					company.SupplierAuthorizedProductCollection,
					oldCompany.SupplierAuthorizedProductCollection);
				productMaps.ModifyCollection(
					security,
					company.IdentityGuid,
					company.ID,
					false,
					company.UnavailableInventoryCollection,
					oldCompany.UnavailableInventoryCollection); // added (IGO 04-Sep-2008)

				var qualificationMaps = new QualificationMapsClass();
				qualificationMaps.ModifyCollection(
					security, company.IdentityGuid, company.CertificateAndPermitCollection, oldCompany.CertificateAndPermitCollection);

				var schedules = new SchedulesClass();
				schedules.ModifyCollection(
					security, company.IdentityGuid, company.AccessScheduleCollection, oldCompany.AccessScheduleCollection);

				// Update Personnel and Update Equipment after PurgeEmptyModifyLog
				// as these operations result in modifications of Person and Equipment
				this.UpdatePersonnel(security, company, oldCompany);
				this.UpdateEquipment(security, company, oldCompany);
			}
			else
			{
				company.UpdatedDate = DateTimeOffset.Now;
				company.UpdatedBy = security.UserID;

				using (var cmd = new SqlCommand())
				{
					company.UpdateSQL(cmd, type);
					this.ConsolidatedDA.ExecuteQuery(security, cmd);
				}
			}

			this.PropagateUpdate(security, company);

			// TODO: Temporary commented out so that QA does not test change queue features.
			// ChangeQueueRecordsClass.ProcessChangeQueueRecords(security, ChangeQueueEventType.Modify, Company);
		}

		/// <summary>
		/// Propagates the latest updates made to a Company record to its child record versions.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="company">
		/// The company.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// security
		/// </exception>
		public void PropagateUpdate(SecurityClass security, CompanyClass company)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "erv.usp_PropagateCompanyRevisionByEntityRecordChange";
				cmd.Parameters.Add("@SourceCompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SourceCompanyGuid"].Value = company.IdentityGuid;
				this.ConsolidatedDA.ExecuteQuery(security, cmd);

				// Next, enqueue a replication of global changes up to a master record version.
				// if the change was made to a child record.
				if (company.IdentityGuid != company.MasterRecordGuid)
				{
					cmd.CommandText = "erv.usp_AddGlobalSpecificQueueRecord";
					cmd.Parameters.Clear();
					cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
					cmd.Parameters["@EntityTypeId"].Value = CompanyClass.EntityTypeID;
					cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@EntityGuid"].Value = company.IdentityGuid;
					cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100);
					cmd.Parameters["@UserId"].Value = security.UserID;
					this.ConsolidatedDA.ExecuteQuery(security, cmd);
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid companyGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			CompanyClass company = this.Get(security, companyGuid);
			if (company.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Company Not Found");
			}

			if (company.IdentityGuid != company.MasterRecordGuid)
			{
				throw new Exception("Cannot delete a Company child record version directly");
			}

			this.UpdateRoles(security, null, company);

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			entityToSiteMaps.PurgeAll(security, ENTITY_TYPE.COMPANY, company.MasterRecordGuid);


			this.UpdateGroups(security, null, company);

			var schedules = new SchedulesClass();
			schedules.ModifyCollection(security, company.IdentityGuid, null, company.AccessScheduleCollection);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandTimeout = 600;
				company.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#endregion

		#region Explicit Interface Methods

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

			var entityToSiteMap = Object as EntityToSiteMapClass;
			if (entityToSiteMap != null)
			{
				if (entityToSiteMap.TypeID != ENTITY_TYPE.COMPANY)
				{
					return;
				}

				if (!preOperation)
				{
					Guid currentSiteGuid = security.SiteGuid;
					try
					{
						security.SiteGuid = entityToSiteMap.AssignedFromSiteGuid;
						CompanyClass company = this.Get(security, entityToSiteMap.IdentityGuid, false);

						// Propogate Roles
						if (entityToSiteMap.SiteGuid != company.SiteGuid)
						{
							var companyRoleMaps = new CompanyRoleMapsClass();

							company.RoleCollection = companyRoleMaps.EnumerateByCompany(security, company.MasterRecordGuid);

							foreach (CompanyRoleMapClass role in company.RoleCollection)
							{
								role.SiteGuid = entityToSiteMap.SiteGuid;
								companyRoleMaps.Add(security, role);
							}
						}
					}
					finally
					{
						security.SiteGuid = currentSiteGuid;
					}
				}
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

			var site = Object as SiteClass;
			if (site != null)
			{
				CompanyCollectionClass companyCollection = this.EnumerateExt2(security, site.SiteGuid, false, true);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (CompanyClass company in companyCollection)
				{
					if (site.SiteGuid == company.SiteGuid && company.MasterRecordGuid == company.IdentityGuid)
					{
						this.Purge(security, company.IdentityGuid);
					}
					else
					{
						if (site.SiteGuid == company.SiteGuid && company.MasterRecordGuid != company.IdentityGuid)
						{
							var maps = new CompanyRoleMapsClass();
							var roleMaps = maps.EnumerateByCompany2(security, site.SiteGuid, company.MasterRecordGuid);
							foreach (var roleMap in roleMaps)
							{
								maps.Purge(security, roleMap);
							}
						}
						var entityToSiteMap = new EntityToSiteMapClass(company) { SiteGuid = site.SiteGuid };
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
			else
			{
				var map = Object as EntityToSiteMapClass;
				if (map != null && map.TypeID == ENTITY_TYPE.COMPANY)
				{
					// Delete any company role maps
					var maps = new CompanyRoleMapsClass();
					var roleMaps = maps.EnumerateByCompany2(security, map.SiteGuid, map.IdentityGuid);
					foreach (var roleMap in roleMaps)
					{
						maps.Purge(security, roleMap);
					}
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

			var site = Object as SiteClass;
			if (site != null)
			{
				CompanyCollectionClass companyCollection = this.EnumerateExt(security, false, true);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (CompanyClass company in companyCollection)
				{
					if (site.SiteGuid == company.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
							security, company.EntityType, company.IdentityGuid);
						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = company.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		#endregion

		#region Methods

		private void CheckSecurity(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				&& !security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				&& !security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_REPORTS) && !security.HasRight(RIGHT.MODIFY_REPORTS)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
				&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) && !security.HasRight(RIGHT.VIEW_PRODUCTS)
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.VIEW_USER_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
				&& !security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) && !security.HasRight(RIGHT.MODIFY_STANDING_OFFERS)
				&& !security.HasRight(RIGHT.INTERFACE_IMPORT) && !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
				&& !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) && !security.HasRight(RIGHT.CREATE_ORDERS)
				&& !security.HasRight(RIGHT.MODIFY_ORDERS) && !security.HasRight(RIGHT.VIEW_ORDERS)
				// Tanks enumerate companies, so allow users with tank rights to enumerate companies
				&& !security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
			&& !security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION)
			&& !security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION)
			&& !security.HasRight(RIGHT.IMPORT_TRANSACTION)
			&& !security.HasRight(RIGHT.VIEW_ALLOCATIONS) && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			{
				throw new FMInsufficientRightsException();
			}
		}

		private bool DoesGroupHaveAllCompanyAssignment(SecurityClass security, CompanyMapClass newGroupMap)
		{
			// Get the group object
			var groups = new GroupsClass();

			GroupClass group = groups.Get(security, newGroupMap.AssignedToGuid);

			if (group != null)
			{
				// Check to see if it has the All company assignment
				foreach (CompanyMapClass companyMap in group.CompanyMapCollection)
				{
					if (companyMap.AssignedGuid == Guid.Empty)
					{
						return true;
					}
				}
			}

			return false;
		}

		// Check if a Shipper, BillTo or ShipTo company type ID is assigned
		// and update the index.
		private void ModifyTypes(SecurityClass security, CompanyClass company)
		{
			var strings = new ApplicationStringsClass();

			// Do Shipper Type.
			if (company.ShipperTypeID != null && company.ShipperTypeID.ToUpper() == "{None}")
			{
				company.ShipperTypeID = "{None}";
			}

			if (company.ShipperTypeID != null && string.IsNullOrEmpty(company.ShipperTypeID) == false
				&& company.ShipperTypeID != "{None}")
			{
				Guid shipperTypeGuid = strings.GetIdentityGuid(security, STRING_TYPE.COMPANY_TYPE, company.ShipperTypeID);

				if (shipperTypeGuid == Guid.Empty)
				{
					var type = new ApplicationStringClass
					{
						Type = STRING_TYPE.COMPANY_TYPE,
						ID = company.ShipperTypeID
					};

					company.ShipperTypeApplicationStringGuid = strings.Add(security, type);
				}
				else
				{
					company.ShipperTypeApplicationStringGuid = shipperTypeGuid;
				}
			}
			else
			{
				company.ShipperTypeApplicationStringGuid = Guid.Empty;
			}

			// Do Customer BillTo Type.
			if (company.CustomerBillToTypeID != null && company.CustomerBillToTypeID.ToUpper() == "{None}")
			{
				company.CustomerBillToTypeID = "{None}";
			}

			if (company.CustomerBillToTypeID != null && string.IsNullOrEmpty(company.CustomerBillToTypeID) == false
				&& company.CustomerBillToTypeID != "{None}")
			{
				Guid billToTypeGuid = strings.GetIdentityGuid(security, STRING_TYPE.COMPANY_TYPE, company.CustomerBillToTypeID);

				if (billToTypeGuid == Guid.Empty)
				{
					var type = new ApplicationStringClass
					{
						Type = STRING_TYPE.COMPANY_TYPE,
						ID = company.CustomerBillToTypeID
					};

					company.CustomerBillToTypeApplicationStringGuid = strings.Add(security, type);
				}
				else
				{
					company.CustomerBillToTypeApplicationStringGuid = billToTypeGuid;
				}
			}
			else
			{
				company.CustomerBillToTypeApplicationStringGuid = Guid.Empty;
			}

			// Do Customer ShipTo Type.
			if (company.CustomerShipToTypeID != null && company.CustomerShipToTypeID.ToUpper() == "{None}")
			{
				company.CustomerShipToTypeID = "{None}";
			}

			if (company.CustomerShipToTypeID != null && string.IsNullOrEmpty(company.CustomerShipToTypeID) == false
				&& company.CustomerShipToTypeID != "{None}")
			{
				Guid shipToTypeGuid = strings.GetIdentityGuid(security, STRING_TYPE.COMPANY_TYPE, company.CustomerShipToTypeID);

				if (shipToTypeGuid == Guid.Empty)
				{
					var type = new ApplicationStringClass
					{
						Type = STRING_TYPE.COMPANY_TYPE,
						ID = company.CustomerShipToTypeID
					};

					company.CustomerShipToTypeApplicationStringGuid = strings.Add(security, type);
				}
				else
				{
					company.CustomerShipToTypeApplicationStringGuid = shipToTypeGuid;
				}
			}
			else
			{
				company.CustomerShipToTypeApplicationStringGuid = Guid.Empty;
			}
		}

		private void UpdateAuthorizedCarriers(SecurityClass security, CompanyClass newCompany, CompanyClass oldCompany)
		{
			var companyMaps = new CompanyMapsClass();

			if (newCompany != null)
			{
				foreach (CompanyMapClass newAuthorizedCarrier in newCompany.AuthorizedCarrierCollection)
				{
					newAuthorizedCarrier.AssignedToGuid = newCompany.IdentityGuid;
					newAuthorizedCarrier.AssignedToID = newCompany.ID;

					// Test for Self Assignment
					if ((newAuthorizedCarrier.AssignedGuid == Guid.Empty) && (newAuthorizedCarrier.AssignedID != "{All}"))
						{
						newAuthorizedCarrier.AssignedGuid = newCompany.IdentityGuid;
						newAuthorizedCarrier.AssignedID = newCompany.ID;
					}

					if (oldCompany != null)
					{
						int item = 0;
						foreach (CompanyMapClass existingAuthorizedCarrier in oldCompany.AuthorizedCarrierCollection)
						{
							if (existingAuthorizedCarrier.AssignedGuid == newAuthorizedCarrier.AssignedGuid)
							{
								break;
							}

							item++;
						}

						if (item == oldCompany.AuthorizedCarrierCollection.Count)
						{
							companyMaps.Add(security, newAuthorizedCarrier);
						}
						else
						{
							oldCompany.AuthorizedCarrierCollection.Remove(item);
						}
					}
					else
					{
						companyMaps.Add(security, newAuthorizedCarrier);
					}
				}
			}

			if (oldCompany != null)
			{
				foreach (CompanyMapClass existingAuthorizedCarrier in oldCompany.AuthorizedCarrierCollection)
				{
					companyMaps.Purge(security, existingAuthorizedCarrier.IdentityGuid, existingAuthorizedCarrier.Type);
				}
			}
		}

		private void UpdateCarrierCustomersShipTo(SecurityClass security, CompanyClass newCompany, CompanyClass oldCompany)
		{
			var companyMaps = new CompanyMapsClass();

			if (newCompany != null)
			{
				foreach (CompanyMapClass newCarrierCustomerShipTo in newCompany.CarrierCustomerShipToCollection)
				{
					newCarrierCustomerShipTo.AssignedGuid = newCompany.IdentityGuid;
					newCarrierCustomerShipTo.AssignedID = newCompany.ID;

					// Test for Self Assignment
					if (newCarrierCustomerShipTo.AssignedToGuid == Guid.Empty)
					{
						newCarrierCustomerShipTo.AssignedToGuid = newCompany.IdentityGuid;
						newCarrierCustomerShipTo.AssignedToID = newCompany.ID;
					}

					if (oldCompany != null)
					{
						int item = 0;
						foreach (CompanyMapClass existingCarrierCustomerShipTo in oldCompany.CarrierCustomerShipToCollection)
						{
							if (existingCarrierCustomerShipTo.AssignedToGuid == newCarrierCustomerShipTo.AssignedToGuid)
							{
								break;
							}

							item++;
						}

						if (item == oldCompany.CarrierCustomerShipToCollection.Count
							&& (newCarrierCustomerShipTo.AssignedGuid != newCompany.MasterRecordGuid
								|| newCarrierCustomerShipTo.AssignedToGuid != newCompany.IdentityGuid))
						{
							companyMaps.Add(security, newCarrierCustomerShipTo);
						}
						else if (oldCompany.CarrierCustomerShipToCollection.Count > item)
						{
							oldCompany.CarrierCustomerShipToCollection.Remove(item);
						}
					}
					else
					{
						companyMaps.Add(security, newCarrierCustomerShipTo);
					}
				}
			}

			if (oldCompany != null)
			{
				foreach (CompanyMapClass existingCarrierCustomerShipTo in oldCompany.CarrierCustomerShipToCollection)
				{
					if ((existingCarrierCustomerShipTo.AssignedGuid == Guid.Empty))
					{
						continue; //do not allow deletion of a ShipTo_to_{All}_Carriers mapping from a Carrier that falls in the {All} category.
					}
					companyMaps.Purge(security, existingCarrierCustomerShipTo.IdentityGuid, existingCarrierCustomerShipTo.Type);
				}
			}
		}

		private void UpdateEquipment(SecurityClass security, CompanyClass newCompany, CompanyClass oldCompany)
		{
			var equipments = new EquipmentsClass();

			if (newCompany != null)
			{
				foreach (EquipmentClass newEquipment in newCompany.EquipmentCollection)
				{
					if (oldCompany != null)
					{
						int item = 0;
						foreach (EquipmentClass existingEquipment in oldCompany.EquipmentCollection)
						{
							if (existingEquipment.IdentityGuid == newEquipment.IdentityGuid)
							{
								break;
							}

							item++;
						}

						if (item == oldCompany.EquipmentCollection.Count)
						{
							EquipmentClass equipment = equipments.Get(security, newEquipment.IdentityGuid);
							equipment.CompanyGuid = newCompany.MasterRecordGuid;
							equipment.CompanyID = newCompany.ID;
							equipments.Modify(security, equipment);
						}
						else
						{
							oldCompany.EquipmentCollection.RemoveAt(item);
						}
					}
					else
					{
						EquipmentClass equipment = equipments.Get(security, newEquipment.IdentityGuid);
						equipment.CompanyGuid = newCompany.MasterRecordGuid;
						equipment.CompanyID = newCompany.ID;
						equipments.Modify(security, equipment);
					}
				}
			}

			if (oldCompany != null)
			{
				foreach (EquipmentClass existingEquipment in oldCompany.EquipmentCollection)
				{
					EquipmentClass equipment = equipments.Get(security, existingEquipment.IdentityGuid);
					equipment.CompanyGuid = Guid.Empty;
					equipment.CompanyID = "{Unassigned}";
					equipments.Modify(security, equipment);
				}
			}
		}

		private void UpdateGroups(SecurityClass security, CompanyClass company, CompanyClass oldCompany)
		{
			var companyMaps = new CompanyMapsClass();

			if (company != null)
			{
				foreach (CompanyMapClass newGroupMap in company.GroupMapCollection)
				{
					newGroupMap.AssignedGuid = company.MasterRecordGuid;
					newGroupMap.AssignedID = company.ID;
					if (oldCompany != null)
					{
						int item = 0;
						foreach (CompanyMapClass existingGroupMap in oldCompany.GroupMapCollection)
						{
							if (existingGroupMap.AssignedToGuid == newGroupMap.AssignedToGuid)
							{
								break;
							}

							item++;
						}

						if (item == oldCompany.GroupMapCollection.Count)
						{
							if (this.DoesGroupHaveAllCompanyAssignment(security, newGroupMap) == false)
							{
								companyMaps.Add(security, newGroupMap);
							}
						}
						else
						{
							oldCompany.GroupMapCollection.Remove(item);
						}
					}
					else
					{
						if (this.DoesGroupHaveAllCompanyAssignment(security, newGroupMap) == false)
						{
							companyMaps.Add(security, newGroupMap);
						}
					}
				}
			}

			if (oldCompany != null)
			{
				foreach (CompanyMapClass existingGroupMap in oldCompany.GroupMapCollection)
				{
					// Don't delete the {All} company map
					if (existingGroupMap.AssignedGuid != Guid.Empty)
					{
						companyMaps.Purge(security, existingGroupMap.IdentityGuid, existingGroupMap.Type);
					}
				}
			}
		}

		private void UpdatePersonnel(SecurityClass security, CompanyClass newCompany, CompanyClass oldCompany)
		{
			var companyMaps = new CompanyMapsClass();

			if (newCompany != null)
			{
				foreach (CompanyMapClass newPerson in newCompany.AssignedPersonnelCollection)
				{
					if (oldCompany != null)
					{
						int item = 0;
						foreach (CompanyMapClass existingPerson in oldCompany.AssignedPersonnelCollection)
						{
							if (existingPerson.AssignedToGuid == newPerson.AssignedToGuid)
							{
								break;
							}

							item++;
						}

						if (item == oldCompany.AssignedPersonnelCollection.Count)
						{
							companyMaps.Add(security, newPerson);
						}
						else
						{
							oldCompany.AssignedPersonnelCollection.Remove(item);
						}
					}
					else
					{
						if (newPerson.AssignedGuid.IsEmpty())
						{
							newPerson.AssignedGuid = newCompany.IdentityGuid;
						}
						companyMaps.Add(security, newPerson);
					}
				}
			}

			if (oldCompany != null)
			{
				foreach (CompanyMapClass existingAssignedPerson in oldCompany.AssignedPersonnelCollection)
				{
					companyMaps.Purge(security, existingAssignedPerson.IdentityGuid, existingAssignedPerson.Type);
				}
			}
		}

		private void UpdateRoles(SecurityClass security, CompanyClass newCompany, CompanyClass oldCompany)
		{
			var companyRoleMaps = new CompanyRoleMapsClass();

			if (newCompany != null)
			{
				foreach (CompanyRoleMapClass newRole in newCompany.RoleCollection)
				{
					newRole.CompanyGuid = newCompany.MasterRecordGuid;

					// CompanyRoles are maintained for each AssignedTo sites separately from RecordVersioning, using a combination of the MasterRecordGuid and the AssignedToSiteGuid.
					newRole.SiteGuid = newCompany.SiteGuid;

					if (oldCompany != null)
					{
						int item = 0;
						foreach (CompanyRoleMapClass existingRole in oldCompany.RoleCollection)
						{
							if (existingRole.Role == newRole.Role)
							{
								break;
							}

							item++;
						}

						if (item == oldCompany.RoleCollection.Count)
						{
							companyRoleMaps.Add(security, newRole);
						}
						else
						{
							oldCompany.RoleCollection.RemoveAt(item);
						}
					}
					else
					{
						companyRoleMaps.Add(security, newRole);
					}
				}
			}

			if (oldCompany != null)
			{
				foreach (CompanyRoleMapClass role in oldCompany.RoleCollection)
				{
					companyRoleMaps.PurgeByRole(security, role.CompanyGuid, role.Role);
				}
			}
		}

		private void Validate(SecurityClass security, CompanyClass company)
		{
			if (string.IsNullOrEmpty(company.ID))
			{
				throw new ApplicationException("ID Required");
			}

			if (company.ExpirationDateTime.Value.Date == DateTimeOffset.MinValue.Date)
			{
				throw new ApplicationException("Expiration Date Required");
			}

			if (company.ID == "{None}" || company.ID == "{Unassigned}" || company.ID == "{All}")
			{
				throw new ApplicationException("ID is reserved key word " + company.ID);
			}

			this.ValidateUserData(security, company);
		}

		/// <summary>
		/// Specialized getter for a carrier for Load Rack
		/// Load Rack keeps separate objects for each role needed, so 
		/// Carrier object only needs the carrier info.  
		/// 
		/// Attempt to improve performance by only returning the bare
		/// minimum required information.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="companyGuid"></param>
		/// <returns></returns>
		public CompanyClass GetCarrierForLoadRack(SecurityClass security, Guid companyGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			this.CheckSecurity(security);

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.LoginSiteGuid, false, false);

			var company = new CompanyClass(site);

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				// Company.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetCompanyByGuid";

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@CompanyGuid"].Value = companyGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			company.Load(set);

			var companyRoleMaps = new CompanyRoleMapsClass();
			company.RoleCollection = companyRoleMaps.EnumerateByCompany(security, company.MasterRecordGuid);

			if (!company.HasRole(COMPANY_ROLE.CARRIER))
			{
				// If this company is not a carrier, we should just return null.  It's not valid.
				return null;
			}

			// CompanyRoles are maintained for each AssignedTo sites separately from RecordVersioning, using a combination of the MasterRecordGuid and the AssignedToSiteGuid.
			var companyMaps = new CompanyMapsClass();

			company.CarrierCustomerShipToCollection = companyMaps.EnumerateByAssignedGuidAndType(
				security, company.IdentityGuid, COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
			company.AssignedPersonnelCollection = companyMaps.EnumerateByAssignedGuidAndType(
				security, company.IdentityGuid, COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY);
			company.AssignedPersonnelCollection.Sort(COMPANY_MAP_SORT_CRITERIA.ASSIGNEDTO);

			company.AuthorizedCarrierCollection = new CompanyMapCollectionClass();
			company.AuthorizedProductCollection = new ProductMapCollectionClass();
			company.SupplierAuthorizedProductCollection = new ProductMapCollectionClass();
			company.UnavailableInventoryCollection = new ProductMapCollectionClass();

			company.GroupMapCollection = companyMaps.EnumerateByAssignedGuidAndType(
				security, company.IdentityGuid, COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);

			company.EquipmentCollection = new EquipmentCollectionClass();

			var qualificationMaps = new QualificationMapsClass();
			company.CertificateAndPermitCollection = qualificationMaps.EnumerateByGuidAndType(
				security, company.IdentityGuid, QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY, false);

			var schedules = new SchedulesClass();
			company.AccessScheduleCollection = schedules.EnumerateByEntityGuidAndType(
				security, company.IdentityGuid, SCHEDULE_TYPE.COMPANY_ACCESS_TYPE);

			if (company.AccessScheduleCollection.Count == 0)
			{

				DAY_OF_WEEK[] dayOfWeek =
					{
						DAY_OF_WEEK.SUNDAY, DAY_OF_WEEK.MONDAY, DAY_OF_WEEK.TUESDAY, DAY_OF_WEEK.WEDNESDAY,
						DAY_OF_WEEK.THURSDAY, DAY_OF_WEEK.FRIDAY, DAY_OF_WEEK.SATURDAY
					};

				for (var item = 0; item < 7; item++)
				{
					var schedule = new ScheduleClass
					{
						Type = SCHEDULE_TYPE.COMPANY_ACCESS_TYPE,
						Day = (int)dayOfWeek[item],
						Enabled = true,
						EndOfDayEnabled = false
					};

					company.AccessScheduleCollection.Add(schedule);
				}
			}

			return company;
		}

		#endregion
	}
}