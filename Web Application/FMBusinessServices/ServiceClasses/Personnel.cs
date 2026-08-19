// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Personnel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Security;
	using System.ServiceModel;
	using System.Text.RegularExpressions;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using IsolationLevel = System.Transactions.IsolationLevel;

	/// <summary>
	/// Service class for working with person objects.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class PersonnelClass : FMServiceBase, IDependency, IPersonnel
	{
		#region Fields

		/// <summary>
		/// The consolidated DA object for database access.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators


		/// <summary>
		/// Adds the specified person.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="person">The person.</param>
		/// <returns>The guid of the newly added person or Guid.Empty.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// security
		/// or
		/// person
		/// </exception>
		/// <exception cref="System.Exception">
		/// Access Denied
		/// or
		/// Personnel Exists
		/// or
		/// Duplicate Card Number
		/// or
		/// Duplicate Short Card Number
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PersonClass person)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (person == null)
			{
				throw new ArgumentNullException(nameof(person));
			}

			if (!security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, person);

			if (!this.GetGuidByID(security, person.ID).IsEmpty())
			{
				throw new Exception("Personnel Exists");
			}

			if (!this.GetGuidByCardNumber(security, person.CardNumber).IsEmpty())
			{
				throw new Exception("Duplicate Card Number");
			}

			if (!this.GetGuidByShortCardNumber(security, person.ShortCardNumber).IsEmpty())
			{
				throw new Exception("Duplicate Short Card Number");
			}

			// Set UserData(list type) to defaults if they are blanks
			UserDataFieldsClass.SetDefaults(security, person.UserData, ENTITY_TYPE.PERSONNEL);

			person.SiteGuid = security.SiteGuid;

			// A new person is always initially owned by the site adding it (ownership may be changed later)
			// Needed for PIN encryption
			person.MasterSiteGuid = security.SiteGuid;

			person.CreatedDate = DateTimeOffset.Now;
			person.CreatedBy = security.UserID;
			person.UpdatedDate = person.CreatedDate;
			person.UpdatedBy = security.UserID;
			this.consolidatedDA.ExecuteQuery(security, person.InsertSqlCommand);

			if (person.AssignedEquipmentID != "{Unassigned}")
			{
				var equipments = new EquipmentsClass();
				EquipmentClass eq = equipments.GetById(security, person.AssignedEquipmentID);

				if (eq != null)
				{
					person.AssignedEquipmentGuid = eq.MasterRecordGuid;
				}
			}
			else
			{
				person.AssignedEquipmentGuid = Guid.Empty;
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(person);
			entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

			this.UpdateRoles(security, person);

			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(security, person.IdentityGuid, person.QualificationCollection, null);
			qualificationMaps.ModifyCollection(security, person.IdentityGuid, person.LicenseCollection, null);
			qualificationMaps.ModifyCollection(security, person.IdentityGuid, person.TrainingCollection, null);

			var schedules = new SchedulesClass();
			schedules.ModifyCollection(security, person.IdentityGuid, person.AccessScheduleCollection, null);

			var companyMaps = new CompanyMapsClass();
			companyMaps.ModifyCollection(
				 security,
				 person.IdentityGuid,
				 person.ID,
				 person.AssignedCompaniesCollection,
				 null);

			return person.IdentityGuid;
		}

		/// <summary>
		/// Enumerates all personnel with respect to the security object site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="hideHiddenPersonnel">If true, only personnel records not marked as hidden will be returned</param>
		/// <returns>A collection of enumerated person objects.</returns>
		public PersonCollectionClass Enumerate(SecurityClass security, bool hideHiddenPersonnel = false)
		{
			return this.Enumerate2(security, security.SiteGuid, hideHiddenPersonnel);
		}

		/// <summary>
		/// Enumerates all personnel with respect to the security object site.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="targetSiteGuid">The identity guid of the target site.</param>
		/// <param name="hideHiddenPersonnel">If true, only personnel records not marked as hidden will be returned</param>
		/// <returns>A collection of enumerated person objects.</returns>
		public PersonCollectionClass Enumerate2(
			 SecurityClass security,
			 Guid targetSiteGuid,
			 bool hideHiddenPersonnel = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				 && !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD) && !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
				 && !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) && !security.HasRight(RIGHT.VIEW_TEST_ITEMS)
				 && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				 && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				 && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				 && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				 && !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				 && !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) && !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(
				 security,
				 targetSiteGuid,
				 getMemberSites: false,
				 getSchedulesAndProcessVariables: false);

			var limits = new EnumerationLimits();

			DataSet set;
			//using (SqlCommand cmd = Person.EnumerateSQL(security, limits.GetLimit(EnumerationLimits.EnumerationOptions.PERSON)))
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerate";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Limit", SqlDbType.Int);
				cmd.Parameters["@Limit"].Value = limits.GetLimit(EnumerationLimits.EnumerationOptions.PERSON);

				if (hideHiddenPersonnel)
				{
					cmd.Parameters.Add("@HideHiddenPersonnel", SqlDbType.Bit).Value = 1;
				}

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}
			PersonCollectionClass personCollection = new PersonCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var person = new PersonClass(site);
				person.Load(set);
				personCollection.Add(person);
				table.Rows.RemoveAt(0);
			}

			return personCollection;
		}

		/// <summary>
		/// Enumerates personnel by company.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="companyGuid">The company GUID.</param>
		/// <returns>A collection of person objects enumerated by company.</returns>
		/// <exception cref="System.ArgumentNullException">Security object invalid.</exception>
		/// <exception cref="System.Exception">Access Denied</exception>
		public PersonCollectionClass EnumerateByCompany(SecurityClass security, Guid companyGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				 && !security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				 && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
				 && !security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
				 && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
				 && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByCompany";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CompanyGuid"].Value = companyGuid;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var personCollection = new PersonCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var person = new PersonClass();
				person.Load(set);
				personCollection.Add(person);
				table.Rows.RemoveAt(0);
			}
			return personCollection;
		}

		/// <summary>
		/// Enumerates the by role
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="role">The role to enumerate.</param>
		/// <param name="hideHiddenPersonnel">If true, only personnel records not marked as hidden will be returned</param>
		/// <returns>A person collection class.</returns>
		public PersonCollectionClass EnumerateByRole(
			 SecurityClass security,
			 PERSON_ROLE role,
			 bool hideHiddenPersonnel = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_TEST_ITEMS) && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				 && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				 && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(
				 security,
				 security.SiteGuid,
				 getMemberSites: false,
				 getSchedulesAndProcessVariables: false);

			var limits = new EnumerationLimits();

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByRole";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Role", SqlDbType.Int);
				cmd.Parameters["@Role"].Value = (int)role;
				cmd.Parameters.Add("@StandardSelect", SqlDbType.Bit);
				cmd.Parameters["@StandardSelect"].Value = 1;
				cmd.Parameters.Add("@Limit", SqlDbType.Int);
				cmd.Parameters["@Limit"].Value = limits.GetLimit(EnumerationLimits.EnumerationOptions.PERSON);

				if (hideHiddenPersonnel)
				{
					cmd.Parameters.Add("@HideHiddenPersonnel", SqlDbType.Bit).Value = 1;
				}

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var personCollection = new PersonCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var person = new PersonClass(site);
				person.Load(set);
				personCollection.Add(person);
				table.Rows.RemoveAt(0);
			}
			return personCollection;
		}


		/// <summary>
		/// Enumerates the by role and companyGuid
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="role">The role to enumerate.</param>
		/// <param name="hideHiddenPersonnel">If true, only personnel records not marked as hidden will be returned</param>
		/// <param name="companyGuid">The companyGuid to filter</param>
		/// <returns>A person collection class.</returns>
		public PersonCollectionClass EnumerateByRoleAndCompanyGuid(
			 SecurityClass security,
			 PERSON_ROLE role,
			 Guid companyGuid,
			 bool hideHiddenPersonnel = false
			 )
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_TEST_ITEMS) && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				 && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				 && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(
				 security,
				 security.SiteGuid,
				 getMemberSites: false,
				 getSchedulesAndProcessVariables: false);

			var limits = new EnumerationLimits();

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByRoleAndCompanyGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Role", SqlDbType.Int);
				cmd.Parameters["@Role"].Value = (int)role;
				cmd.Parameters.Add("@StandardSelect", SqlDbType.Bit);
				cmd.Parameters["@StandardSelect"].Value = 1;
				cmd.Parameters.Add("@Limit", SqlDbType.Int);
				cmd.Parameters["@Limit"].Value = limits.GetLimit(EnumerationLimits.EnumerationOptions.PERSON);
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CompanyGuid"].Value = companyGuid;

				if (hideHiddenPersonnel)
				{
					cmd.Parameters.Add("@HideHiddenPersonnel", SqlDbType.Bit).Value = 1;
				}

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var personCollection = new PersonCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var person = new PersonClass(site);
				person.Load(set);
				personCollection.Add(person);
				table.Rows.RemoveAt(0);
			}
			return personCollection;
		}

		/// <summary>
		/// Enumerates the by role
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="role">The role to enumerate.</param>
		/// <returns>A dataset containing personnel records.</returns>
		public DataSet EnumerateByRole1(SecurityClass security, PERSON_ROLE role)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_TEST_ITEMS) && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				 && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				 && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;
			//using (SqlCommand cmd = Person.EnumerateByRoleSQL(security, Role, ContextUtil.IsInTransaction))
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByRole";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Role", SqlDbType.Int);
				cmd.Parameters["@Role"].Value = (int)role;
				cmd.Parameters.Add("@StandardSelect", SqlDbType.Bit);
				cmd.Parameters["@StandardSelect"].Value = 1;
				cmd.Parameters.Add("@Limit", SqlDbType.Int);
				cmd.Parameters["@Limit"].Value = 0;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}
			return set;
		}

		// **********************************************************************************************************************
		// This method will return a personnel object collection of the personnel that meet the security, role,
		// filter, and by group personnel criterion. This method is the same as the EnumerateByRole method
		// with the exception that the user has supplied a filter to narrow the search on the list of personnel.
		// **********************************************************************************************************************
		public PersonCollectionClass EnumerateByRoleAndFilter(
			 SecurityClass security,
			 PERSON_ROLE role,
			 string filter,
			 string order,
			 bool hideHiddenPersonnel = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				 && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var limits = new EnumerationLimits();

			DataSet set;
			//using (SqlCommand cmd = Person.EnumerateByRoleAndFilterSQL(security, Role, filter, order, limits.GetLimit(EnumerationLimits.EnumerationOptions.PERSON)))
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByRoleAndFilter";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Role", SqlDbType.Int);
				cmd.Parameters["@Role"].Value = (int)role;
				cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, -1);

				cmd.Parameters["@SearchFilter"].Value = filter != null ? "%" + filter.Trim().ToUpper() + "%" : (object)"%%";

				cmd.Parameters.Add("@OrderBy", SqlDbType.NVarChar, 200);

				cmd.Parameters["@OrderBy"].Value = order ?? (object)DBNull.Value;

				cmd.Parameters.Add("@Limit", SqlDbType.Int);
				cmd.Parameters["@Limit"].Value = limits.GetLimit(EnumerationLimits.EnumerationOptions.PERSON);

				if (hideHiddenPersonnel)
				{
					cmd.Parameters.Add("@HideHiddenPersonnel", SqlDbType.Bit).Value = 1;
				}

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var personCollection = new PersonCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var person = new PersonClass();
				person.Load(set);
				personCollection.Add(person);
				table.Rows.RemoveAt(0);
			}
			return personCollection;
		}

		public PersonCollectionClass EnumerateByRoleOrderByCompany(SecurityClass security, PERSON_ROLE role)
		{
			return this.EnumerateByRoleOrderByCompany(security, role, true);
		}

		public PersonCollectionClass EnumerateByRoleOrderByCompany(
			 SecurityClass security,
			 PERSON_ROLE role,
			 bool bLocalize)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				 && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				 && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			SiteClass site = null;

			if (bLocalize)
			{
				site = sites.GetByMemberAndProcessVariables(
					 security,
					 security.SiteGuid,
					 getMemberSites: false,
					 getSchedulesAndProcessVariables: false);
			}

			DataSet set;
			//using (SqlCommand cmd = Person.EnumerateByRoleOrderByCompanySQL(security, Role))
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByRole_SortByCompany";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Role", SqlDbType.Int);
				cmd.Parameters["@Role"].Value = (int)role;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var personCollection = new PersonCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				PersonClass person = bLocalize ? new PersonClass(site) : new PersonClass();
				person.Load(set);
				personCollection.Add(person);
				table.Rows.RemoveAt(0);
			}

			return personCollection;
		}

		public PersonCollectionClass EnumerateByRoleSortByName(SecurityClass security, PERSON_ROLE role)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_TEST_ITEMS) && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				 && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS) && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				 && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) && !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				 && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				 && !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				 && !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(
				 security,
				 security.SiteGuid,
				 getMemberSites: false,
				 getSchedulesAndProcessVariables: false);

			DataSet set;
			//using (SqlCommand cmd = Person.EnumerateByRoleSQLSortByName(security, Role))
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByRole_SortByName";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Role", SqlDbType.Int);
				cmd.Parameters["@Role"].Value = (int)role;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var personCollection = new PersonCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var person = new PersonClass(site);
				person.Load(set);
				personCollection.Add(person);
				table.Rows.RemoveAt(0);
			}
			return personCollection;
		}

		public Dictionary<string, PersonInfoClass> EnumerateInfoByRoleOrderByCompany(
			 SecurityClass security,
			 PERSON_ROLE role)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				 && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;
			//using (SqlCommand cmd = Person.EnumerateByRoleOrderByCompanySQL(security, Role))
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByRole_SortByCompany";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Role", SqlDbType.Int);
				cmd.Parameters["@Role"].Value = (int)role;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var personTable = new Dictionary<string, PersonInfoClass>();
			foreach (DataRow row in table.Rows)
			{
				var personInfo = new PersonInfoClass
				{
					ID = row["PersonID"] as string,
					CompanyID =
												 row.IsNull("CompanyID")
													  ? "{Unassigned}"
													  : row["CompanyID"] as string,
					CardNumber =
												 row.IsNull("CardNumber")
													  ? string.Empty
													  : row["CardNumber"] as string,
					IdentityGuid = (Guid)row["PersonnelGuid"]
				};

				personTable.Add(personInfo.ID, personInfo);
			}
			return personTable;
		}

		public PersonCollectionClass EnumerateUndelegated(SecurityClass security)
		{
			var personnelCollection = new PersonCollectionClass();
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetUndelegatedPersonnel";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count > 0)
			{
				while (set.Tables[0].Rows.Count != 0)
				{
					DataRow row = set.Tables[0].Rows[0];
					var person = new PersonClass
					{
						IdentityGuid = DataObject.getValue(row["PersonnelGuid"], Guid.Empty),
						MasterRecordGuid =
												DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
						ID = DataObject.getValue(row["PersonID"], string.Empty),
						SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
						AssignedToSiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
						AssignedFromSiteGuid =
												DataObject.getValue(row["AssignedFromSiteGuid"], Guid.Empty),
						AssignedFromSiteId =
												DataObject.getValue(row["AssignedFromSiteId"], string.Empty)
					};

					// This query is limited to master records, i.e. SiteOwner, AssignedFromSite, and AssignedToSite are the same.
					personnelCollection.Add(person);
					set.Tables[0].Rows.RemoveAt(0);
				}
			}

			return personnelCollection;
		}

		/// <summary>
		/// Enumerate all personnel for the site, retrieving only basic information like the ID, SiteGuid, MasterRecordGuid, and IdentityGuid
		/// </summary>
		/// <param name="security">Contains Security information</param>
		/// <returns>All personnel for the site, with basic information like the ID, SiteGuid, MasterRecordGuid, and IdentityGuid populated</returns>
		public PersonCollectionClass EnumerateBasicInformationOnly(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
			&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
			&& !security.HasRight(RIGHT.IMPORT_TRANSACTION))
			{
				throw new FMInsufficientRightsException();
			}

			var personnelCollection = new PersonCollectionClass();

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateBasic";
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set == null || set.Tables.Count <= 0 || set.Tables[0] == null)
			{
				return personnelCollection;
			}

			foreach (DataRow row in set.Tables[0].Rows)
			{
				var person = new PersonClass
				{
					IdentityGuid = DataObject.getValue(row["PersonnelGuid"], Guid.Empty),
					MasterRecordGuid =
											DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
					ID = DataObject.getValue(row["PersonID"], string.Empty),
					SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty)
				};

				personnelCollection.Add(person);
			}

			return personnelCollection;
		}

		public DataSet EnumerateUpdateVersions(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			using (var cmd = new SqlCommand())
			{
				var person = new PersonClass();
				cmd.CommandText = person.EnumerateNotificationSQL(security);
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				return this.consolidatedDA.GetDataSet(cmd, security);
			}
		}

		public PersonClass Get(SecurityClass security, Guid targetGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD) && !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
				 && !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
				 && !security.HasRight(RIGHT.VIEW_TEST_ITEMS) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				 && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				 && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH)
				 && !security.HasRight(RIGHT.CREATE_ORDERS) && !security.HasRight(RIGHT.VIEW_ORDERS)
				 && !security.HasRight(RIGHT.MODIFY_ORDERS) && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
				 && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
				 && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				 && !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				 && !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))

			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(
				 security,
				 security.SiteGuid,
				 getMemberSites: false,
				 getSchedulesAndProcessVariables: false);

			PersonClass person = new PersonClass(site);
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelByPersonnelGuids";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@PersonnelGuids1", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@PersonnelGuids1"].Value = targetGuid;
				cmd.Parameters.Add("@PersonnelGuids2", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@PersonnelGuids2"].Value = Guid.Empty;
				cmd.Parameters.Add("@InTransaction", SqlDbType.Bit);
				cmd.Parameters["@InTransaction"].Value = ContextUtil.IsInTransaction ? 1 : 0;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}
			person.Load(set);

			var personRoleMaps = new PersonRoleMapsClass();
			person.RoleCollection = personRoleMaps.EnumerateByPerson(security, targetGuid);

			var qualificationMaps = new QualificationMapsClass();
			person.QualificationCollection = qualificationMaps.EnumerateByGuidAndType(
				 security,
				 targetGuid,
				 QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON,
				 false);
			person.LicenseCollection = qualificationMaps.EnumerateByGuidAndType(
				 security,
				 targetGuid,
				 QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON,
				 false);
			person.TrainingCollection = qualificationMaps.EnumerateByGuidAndType(
				 security,
				 targetGuid,
				 QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON,
				 false);

			var companyMaps = new CompanyMapsClass();
			person.AssignedCompaniesCollection = companyMaps.EnumerateByAssignedToGuidAndType(
				 security,
				 person.IdentityGuid,
				 COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY);
			person.AssignedCompaniesCollection.Sort(COMPANY_MAP_SORT_CRITERIA.ASSIGNED);

			var schedules = new SchedulesClass();
			person.AccessScheduleCollection = schedules.EnumerateByEntityGuidAndType(
				 security,
				 person.IdentityGuid,
				 SCHEDULE_TYPE.PERSON_ACCESS_TYPE);

			if (person.AccessScheduleCollection.Count == 0)
			{
				DAY_OF_WEEK[] dayOfWeek =
				{
						  DAY_OF_WEEK.SUNDAY, DAY_OF_WEEK.MONDAY, DAY_OF_WEEK.TUESDAY,
						  DAY_OF_WEEK.WEDNESDAY, DAY_OF_WEEK.THURSDAY, DAY_OF_WEEK.FRIDAY,
						  DAY_OF_WEEK.SATURDAY
					 };

				for (int item = 0; item < 7; item++)
				{
					var schedule = new ScheduleClass
					{
						Type = SCHEDULE_TYPE.PERSON_ACCESS_TYPE,
						Day = (int)dayOfWeek[item],
						OpeningTime = { Value = TimeConverter.MinFMTime },
						ClosingTime = { Value = TimeConverter.MaxFMTime },
						Enabled = true
					};

					person.AccessScheduleCollection.Add(schedule);
				}
			}

			return person;
		}

		public PersonClass GetBasicInfo(SecurityClass security, Guid personnelGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelByGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@PersonnelGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
				cmd.Parameters["@PersonnelGuid"].Value = personnelGuid;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataRow row = set.Tables[0].Rows[0];
			var person = new PersonClass
			{
				IdentityGuid = DataObject.getValue(row["PersonnelGuid"], Guid.Empty),
				MasterRecordGuid = DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
				ID = DataObject.getValue(row["PersonID"], string.Empty),
				SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty)
			};

			return person;
		}

		public PersonClass GetByID(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				 && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				 && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			PersonClass person = new PersonClass();
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelByID";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
				cmd.Parameters["@ID"].Value = id;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}
			//Person.Load(ConsolidatedDA.GetDataSet(Person.SelectByIDSQL(security, ContextUtil.IsInTransaction), security));
			person.Load(set);
			return person;
		}

		public Guid GetGuidByCardNumber(SecurityClass security, string cardNumber)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING))
			{
				throw new FMInsufficientRightsException();
			}

			PersonClass person = new PersonClass();
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelByCardNumber";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@CardNumber1", SqlDbType.NVarChar, 30);
				cmd.Parameters["@CardNumber1"].Value = cardNumber;
				cmd.Parameters.Add("@CardNumber2", SqlDbType.NVarChar, 30);
				cmd.Parameters["@CardNumber2"].Value = string.Empty;
				cmd.Parameters.Add("@InTransaction", SqlDbType.Bit);
				cmd.Parameters["@InTransaction"].Value = ContextUtil.IsInTransaction ? 1 : 0;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}
			//Person.Load(ConsolidatedDA.GetDataSet(Person.SelectByCardNumberSQL(security, ContextUtil.IsInTransaction), security));
			person.Load(set);
			return person.IdentityGuid;
		}

		public Guid GetGuidByID(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				 && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				 && !security.HasRight(RIGHT.CREATE_ORDERS) && !security.HasRight(RIGHT.VIEW_ORDERS)
				 && !security.HasRight(RIGHT.MODIFY_ORDERS) && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
				 && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				 && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var person = this.GetByID(security, id);
			return person.IdentityGuid;
		}

		public Guid GetGuidByShortCardNumber(SecurityClass security, string shortCardNumber)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING))
			{
				throw new FMInsufficientRightsException();
			}

			PersonClass person = new PersonClass();
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelByShortCardNumber";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@ShortCardNumber1", SqlDbType.NVarChar, 30);
				cmd.Parameters["@ShortCardNumber1"].Value = shortCardNumber;
				cmd.Parameters.Add("@ShortCardNumber2", SqlDbType.NVarChar, 30);
				cmd.Parameters["@ShortCardNumber2"].Value = string.Empty;
				cmd.Parameters.Add("@InTransaction", SqlDbType.Bit);
				cmd.Parameters["@InTransaction"].Value = ContextUtil.IsInTransaction ? 1 : 0;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}
			person.Load(set);
			return person.IdentityGuid;
		}

		public string GetLatestRowVersionByRole(SecurityClass security, PERSON_ROLE role)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			string result = string.Empty;

			DataSet set;
			//using (SqlCommand cmd = person.GetLatestRowVersionByRole(security, Role, ContextUtil.IsInTransaction))
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByRole";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@Role", SqlDbType.Int);
				cmd.Parameters["@Role"].Value = (int)role;
				cmd.Parameters.Add("@StandardSelect", SqlDbType.Bit);
				cmd.Parameters["@StandardSelect"].Value = 0;
				cmd.Parameters.Add("@Limit", SqlDbType.Int);
				cmd.Parameters["@Limit"].Value = 0;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0)
			{
				DataRow row = set.Tables[0].Rows[0];
				result = DataObject.getString(row["RowVersionString"]);
			}

			if (string.IsNullOrEmpty(result))
			{
				result = "0";
			}

			return result;
		}

		public Guid GetMasterSiteGuidByID(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				 && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				 && !security.HasRight(RIGHT.CREATE_ORDERS) && !security.HasRight(RIGHT.VIEW_ORDERS)
				 && !security.HasRight(RIGHT.MODIFY_ORDERS) && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
				 && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				 && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var person = this.GetByID(security, id);
			return person.MasterSiteGuid;
		}

		public Guid GetMasterRecordGuid(SecurityClass security, string id)
		{
			Guid result = Guid.Empty;
			PersonClass person = this.GetByID(security, id);
			if (person != null)
			{
				result = person.MasterRecordGuid;
			}

			return result;
		}

		/// <summary>
		/// Gets the next short card number.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>The next short card number.</returns>
		public string GetNextShortCardNumber(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(
				 security,
				 security.SiteGuid,
				 getMemberSites: false,
				 getSchedulesAndProcessVariables: false);

			if (!site.UseShortCardNumber)
			{
				return "";
			}

			int startingShortCardNumber = Convert.ToInt32(site.StartingShortCardNumber);
			int nextShortCardNumber = startingShortCardNumber;

			DataSet set;
			//using (SqlCommand cmd = Person.EnumerateShortCardNumbersSQL(Security))
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_EnumerateShortCardNumbersSQL";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set != null && set.Tables.Count != 0 && set.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow row in set.Tables[0].Rows)
				{
					if (row.IsNull("ShortCardNumber"))
					{
						continue;
					}

					try
					{
						int shortCardNumber = Convert.ToInt32(row["ShortCardNumber"] as string);

						if (shortCardNumber >= nextShortCardNumber)
						{
							nextShortCardNumber = shortCardNumber + 1;
						}
					}
					catch (FormatException)
					{
						// Cannot evaluate numbers that are not numeric.  
					}
					catch (OverflowException)
					{
						// Can not evaluate based on an out-of-range number.  
					}
				}
			}

			return nextShortCardNumber.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Imports the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="person">The person.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Security
		/// or
		/// person
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass security, PersonClass person)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (person == null)
			{
				throw new ArgumentNullException(nameof(person));
			}

			SecurityClass securityClone = security.Clone();

			var companies = new CompaniesClass();
			var qualifications = new QualificationsClass();
			var schedules = new SchedulesClass();

			try
			{
				person.IdentityGuid = this.GetGuidByID(securityClone, person.ID);

				// If the entity exists and is not owned by this site, do not update it.
				if (!person.IdentityGuid.IsEmpty()
					 && this.Get(securityClone, person.IdentityGuid).SiteGuid != securityClone.SiteGuid)
				{
					return;
				}

				person.MasterSiteGuid = this.GetMasterSiteGuidByID(securityClone, person.ID);

				if (person.SupervisorID != string.Empty && person.SupervisorID != "{Unassigned}")
				{
					person.SupervisorGuid = this.GetMasterRecordGuid(securityClone, person.SupervisorID);
					if (person.SupervisorGuid.IsEmpty())
					{
						var supervisor = new PersonClass
						{
							ID = person.SupervisorID,
							FirstName = "firstname",
							LastName = "lastname"
						};

						// Need first & last names to be a valid person. Use dummy names for now. Will be updated later when that person is imported.
						var role = new PersonRoleMapClass { Role = PERSON_ROLE.SUPERVISOR_ROLE };
						supervisor.RoleCollection.Add(role);
						person.SupervisorGuid = this.Add(securityClone, supervisor);
					}
				}

				foreach (CompanyMapClass assignedCompany in person.AssignedCompaniesCollection)
				{
					assignedCompany.AssignedGuid = companies.GetMasterRecordGuid(securityClone, person.CompanyID);
					if (assignedCompany.AssignedGuid.IsEmpty())
					{
						var carrier = new CompanyClass { ID = assignedCompany.AssignedID };
						var role = new CompanyRoleMapClass { Role = COMPANY_ROLE.CARRIER };
						carrier.RoleCollection.Add(role);
						assignedCompany.AssignedGuid = companies.Add(security, carrier);
					}
				}

				foreach (QualificationMapClass license in person.LicenseCollection)
				{
					Guid licenseGuid = qualifications.GetIdentityGuid(
						 securityClone,
						 QUALIFICATION_TYPE.PERSON_LICENSE,
						 license.ID);
					if (licenseGuid.IsEmpty())
					{
						QualificationClass qualification =
							 person.FindQualificationInPayload(QUALIFICATION_TYPE.PERSON_LICENSE, license.ID)
							 ?? new QualificationClass { ID = license.ID, Type = QUALIFICATION_TYPE.PERSON_LICENSE };

						licenseGuid = qualifications.Add(securityClone, qualification);
					}

					license.AssignedGuid = licenseGuid;
					license.Type = QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON;
				}

				foreach (QualificationMapClass training in person.TrainingCollection)
				{
					Guid trainingGuid = qualifications.GetIdentityGuid(
						 securityClone,
						 QUALIFICATION_TYPE.PERSON_TRAINING,
						 training.ID);
					if (trainingGuid.IsEmpty())
					{
						QualificationClass qualification =
							 person.FindQualificationInPayload(QUALIFICATION_TYPE.PERSON_TRAINING, training.ID)
							 ?? new QualificationClass { ID = training.ID, Type = QUALIFICATION_TYPE.PERSON_TRAINING };

						trainingGuid = qualifications.Add(securityClone, qualification);
					}

					training.AssignedGuid = trainingGuid;
					training.Type = QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;
				}

				foreach (QualificationMapClass qualification in person.QualificationCollection)
				{
					Guid qualificationGuid = qualifications.GetIdentityGuid(
						 securityClone,
						 QUALIFICATION_TYPE.PERSON_QUALIFICATION,
						 qualification.ID);
					if (qualificationGuid.IsEmpty())
					{
						QualificationClass newQualification =
							 person.FindQualificationInPayload(QUALIFICATION_TYPE.PERSON_QUALIFICATION, qualification.ID)
							 ?? new QualificationClass
							 {
								 ID = qualification.ID,
								 Type = QUALIFICATION_TYPE.PERSON_QUALIFICATION
							 };

						qualificationGuid = qualifications.Add(securityClone, newQualification);
					}

					qualification.AssignedGuid = qualificationGuid;
					qualification.Type = QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON;
				}

				if (person.IdentityGuid.IsEmpty())
				{
					this.Add(securityClone, person);
				}
				else
				{
					foreach (ScheduleClass schedule in person.AccessScheduleCollection)
					{
						schedule.IdentityGuid = schedules.GetIdentityGuid(securityClone, person.IdentityGuid, schedule);
					}

					this.Modify(securityClone, DATA_TYPE.CONFIG, person);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("[Personnel Import Error ID] : " + person.ID + ", " + ex.Message);
			}
		}

		/// <summary>
		/// Modifies the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="type">The type.</param>
		/// <param name="person">The person.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DATA_TYPE type, PersonClass person)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (person == null)
			{
				throw new ArgumentNullException(nameof(person));
			}

			if (!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				if (type == DATA_TYPE.DYNAMIC && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
					 && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				{
					throw new FMInsufficientRightsException();
				}

				if (type == DATA_TYPE.CONFIG && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
					 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
					 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING))
				{
					throw new FMInsufficientRightsException();
				}
			}

			if (type == DATA_TYPE.CONFIG)
			{
				this.Validate(security, person);

				Guid targetGuid = this.GetGuidByID(security, person.ID);

				if (targetGuid.IsNotEmptyAndNotEqualTo(person.IdentityGuid))
				{
					throw new Exception("Personnel Exists");
				}

				targetGuid = this.GetGuidByCardNumber(security, person.CardNumber);
				if (targetGuid.IsNotEmptyAndNotEqualTo(person.IdentityGuid))
				{
					throw new Exception("Duplicate Card Number");
				}

				targetGuid = this.GetGuidByShortCardNumber(security, person.ShortCardNumber);
				if (targetGuid.IsNotEmptyAndNotEqualTo(person.IdentityGuid))
				{
					throw new Exception("Duplicate Short Card Number");
				}

				PersonClass oldPerson = this.Get(security, person.IdentityGuid);

				if (oldPerson.IdentityGuid.IsEmpty())
				{
					throw new Exception("Personnel Not Found");
				}

				// If the PIN Number is the dummy masked password text, 
				// it has not been modified by the user and the existing value should be preserved
				if (person.PINNumber == PersonClass.MaskedPasswordText)
				{
					person.PINNumber = oldPerson.PINNumber;
				}

				// Check for Locked Out
				if (oldPerson.LockedOut != person.LockedOut && person.LockedOut)
				{
					var alarmAndEventLogs = new AlarmAndEventLogsClass();
					alarmAndEventLogs.Add(security, person.LockOutEvent);
				}

				if (person.AssignedEquipmentID == "{Unassigned}")
				{
					if (!person.AssignedEquipmentGuid.IsEmpty())
					{
						person.AssignedEquipmentGuid = Guid.Empty;
					}
				}
				else
				{
					var equipments = new EquipmentsClass();
					EquipmentClass eq = equipments.GetById(security, person.AssignedEquipmentID);
					if (eq != null)
					{
						person.AssignedEquipmentGuid = eq.MasterRecordGuid;
					}
				}

				// Set UserData(list type) to defaults if they are blanks
				UserDataFieldsClass.SetDefaults(security, person.UserData, ENTITY_TYPE.PERSONNEL);

				person.UpdatedDate = DateTimeOffset.Now;
				person.UpdatedBy = security.UserID;

				var entityToSiteMaps = new EntityToSiteMaps();
				if (person.SiteGuid != oldPerson.SiteGuid)
				{
					// Purge from EntityToSiteMap
					entityToSiteMaps.PurgeAll(security, ENTITY_TYPE.PERSONNEL, person.MasterRecordGuid);
				}

				this.consolidatedDA.ExecuteQuery(security, person.UpdateSqlCommand(type));

				if (person.SiteGuid != oldPerson.SiteGuid)
				{
					// Create Entity to Site Map
					var newEntityToSiteMap = new EntityToSiteMapClass(person);

					Guid currentSiteContext = security.SiteGuid;

					////When changing ownership of an entity that supports Cascading Assignment, need to make sure that the base mapping is created with the AssignedFromSiteGuid being the same as the Owner Site Guid (and the AssignedToSiteGuid), and not be set with the Site Context Guid which in the case of a Change of Ownership would be different from the new Owner Site Guid.
					////The Security SiteGuid swap below effectively does so by supplying the EntityToSiteMaps.Add() operation with the correct SiteGuid to use to set the AssignedFromSiteGuid.
					security.SiteGuid = person.SiteGuid;
					entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
					security.SiteGuid = currentSiteContext;
				}

				this.UpdateRoles(security, person);

				var qualificationMaps = new QualificationMapsClass();
				qualificationMaps.ModifyCollection(
					 security,
					 person.IdentityGuid,
					 person.QualificationCollection,
					 oldPerson.QualificationCollection);
				qualificationMaps.ModifyCollection(
					 security,
					 person.IdentityGuid,
					 person.LicenseCollection,
					 oldPerson.LicenseCollection);
				qualificationMaps.ModifyCollection(
					 security,
					 person.IdentityGuid,
					 person.TrainingCollection,
					 oldPerson.TrainingCollection);

				var schedules = new SchedulesClass();
				schedules.ModifyCollection(
					 security,
					 person.IdentityGuid,
					 person.AccessScheduleCollection,
					 oldPerson.AccessScheduleCollection);

				var companyMaps = new CompanyMapsClass();
				companyMaps.ModifyCollection(
					 security,
					 person.IdentityGuid,
					 person.ID,
					 person.AssignedCompaniesCollection,
					 oldPerson.AssignedCompaniesCollection);
			}
			else
			{
				person.UpdatedDate = DateTimeOffset.Now;
				person.UpdatedBy = security.UserID;
				using (SqlCommand cmd = person.UpdateSqlCommand(type))
				{
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}

			this.PropagateUpdate(security, person);
		}

		/// <summary>
		/// Prepares a person object for export.
		/// </summary>
		/// <param name="security">
		/// The security object.
		/// </param>
		/// <param name="person">
		/// The person to prepare.
		/// </param>
		/// <returns>
		/// The prepared person class object.
		/// </returns>
		public PersonClass PrepareForExport(SecurityClass security, PersonClass person)
		{
			// Load the Training, Qualification, and License items so their 
			// information can be recreated if necessary
			person.QualificationExportPayload = new QualificationCollectionClass();

			var qualifications = new QualificationsClass();

			foreach (QualificationMapClass training in person.TrainingCollection)
			{
				QualificationClass qual = qualifications.Get(security, training.AssignedGuid);
				person.QualificationExportPayload.Add(qual);
			}

			foreach (QualificationMapClass qualification in person.QualificationCollection)
			{
				QualificationClass qual = qualifications.Get(security, qualification.AssignedGuid);
				person.QualificationExportPayload.Add(qual);
			}

			foreach (QualificationMapClass license in person.LicenseCollection)
			{
				QualificationClass qual = qualifications.Get(security, license.AssignedGuid);
				person.QualificationExportPayload.Add(qual);
			}

			return person;
		}

		/// <summary>
		/// Propagates the latest updates made to a Personnel record to its child record versions.
		/// After propagating updates to child record versions, enqueue a request to replicate
		/// global specific fields up to the master record
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="person">
		/// The Person whose changes are to be propagated.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// security
		/// </exception>
		public void PropagateUpdate(SecurityClass security, PersonClass person)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			using (var cmd = new SqlCommand())
			{
				// First propagate changes down to child record versions
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "erv.usp_PropagatePersonnelRevisionByEntityRecordChange";
				cmd.Parameters.Add("@SourcePersonnelGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SourcePersonnelGuid"].Value = person.IdentityGuid;
				this.consolidatedDA.ExecuteQuery(security, cmd);

				// Next, enqueue a replication of global changes up to a master record version.
				// if the change was made to a child record.
				if (person.IdentityGuid != person.MasterRecordGuid)
				{
					cmd.CommandText = "erv.usp_AddGlobalSpecificQueueRecord";
					cmd.Parameters.Clear();
					cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
					cmd.Parameters["@EntityTypeId"].Value = PersonClass.ENTITY_TYPE_ID;
					cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@EntityGuid"].Value = person.IdentityGuid;
					cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100);
					cmd.Parameters["@UserId"].Value = security.UserID;
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
		}

		/// <summary>
		/// Purges the specified person record.
		/// </summary>
		/// <param name="security">
		/// The security object.
		/// </param>
		/// <param name="targetGuid">
		/// The identity guid of the person record to purge.
		/// </param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid targetGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				 && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			PersonClass person = this.Get(security, targetGuid);
			if (person.IdentityGuid.IsEmpty())
			{
				throw new Exception("Personnel Not Found");
			}

			if (person.IdentityGuid != person.MasterRecordGuid)
			{
				throw new Exception("Cannot delete an Personnel child record version directly");
			}

			person.RoleCollection = null;
			this.UpdateRoles(security, person);

			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(security, person.IdentityGuid, null, person.QualificationCollection);
			qualificationMaps.ModifyCollection(security, person.IdentityGuid, null, person.LicenseCollection);
			qualificationMaps.ModifyCollection(security, person.IdentityGuid, null, person.TrainingCollection);

			var schedules = new SchedulesClass();
			schedules.ModifyCollection(security, person.IdentityGuid, null, person.AccessScheduleCollection);

			var companyMaps = new CompanyMapsClass();
			companyMaps.ModifyCollection(
				 security,
				 person.IdentityGuid,
				 person.ID,
				 null,
				 person.AssignedCompaniesCollection);

			var appointments = new AppointmentsClass();
			appointments.PurgeByAssetID(security, person.IdentityGuid, "Personnel");

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			entityToSiteMaps.PurgeAll(security, ENTITY_TYPE.PERSONNEL, person.MasterRecordGuid);

			this.consolidatedDA.ExecuteQuery(security, person.PurgeSQL);
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

			if (Object is SiteClass site)
			{
				PersonCollectionClass personCollection = this.Enumerate2(security, site.SiteGuid);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (PersonClass person in personCollection)
				{
					if (site.SiteGuid == person.SiteGuid && person.MasterRecordGuid == person.IdentityGuid)
					{
						this.Purge(security, person.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass(person) { SiteGuid = site.SiteGuid };
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
			else
			{
				if (Object is CompanyClass company)
				{
					// Remove Reference to Company
					PersonCollectionClass personCollection = this.EnumerateByCompany(security, company.IdentityGuid);
					foreach (PersonClass person in personCollection)
					{
						person.CompanyGuid = Guid.Empty;
						person.UpdatedDate = DateTimeOffset.Now;
						person.UpdatedBy = security.UserID;
						this.consolidatedDA.ExecuteQuery(security, person.UpdateSqlCommand(DATA_TYPE.CONFIG));
					}
				}
				else
				{
					var entityToSiteMap = Object as EntityToSiteMapClass;
					// Modify Person to remove reference to this Company
					if (entityToSiteMap?.TypeID == ENTITY_TYPE.COMPANY)
					{
						Guid tempSiteIndex = security.SiteGuid;
						security.SiteGuid = entityToSiteMap.SiteGuid;
						PersonCollectionClass personCollection = this.EnumerateByCompany(
							 security,
							 entityToSiteMap.IdentityGuid);
						security.SiteGuid = tempSiteIndex;

						foreach (PersonClass person in personCollection)
						{
							person.CompanyGuid = Guid.Empty;
							person.UpdatedDate = DateTimeOffset.Now;
							person.UpdatedBy = security.UserID;
							this.consolidatedDA.ExecuteQuery(security, person.UpdateSqlCommand(DATA_TYPE.CONFIG));
						}
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

			if (Object is SiteClass site)
			{
				PersonCollectionClass personCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (PersonClass person in personCollection)
				{
					if (site.SiteGuid == person.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection =
							 entityToSiteMaps.EnumerateByTypeIDAndGuid(security, person.EntityType, person.IdentityGuid);

						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = person.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
			else
			{
				if (Object is CompanyClass company)
				{
					// Remove reference to Company if Carrier Role removed
					if (!company.HasRole(COMPANY_ROLE.CARRIER))
					{
						PersonCollectionClass personCollection = this.EnumerateByCompany(security, company.IdentityGuid);
						foreach (PersonClass person in personCollection)
						{
							person.CompanyGuid = Guid.Empty;
							this.Modify(security, DATA_TYPE.CONFIG, person);
						}
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// Updates the roles of the specified person object.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="person">The person.</param>
		private void UpdateRoles(SecurityClass security, PersonClass person)
		{
			var personRoleMaps = new PersonRoleMapsClass();
			PersonRoleMapCollectionClass existingRoleCollection = personRoleMaps.EnumerateByPerson(
				 security,
				 person.IdentityGuid);
			PersonRoleMapCollectionClass newRoleCollection = person.RoleCollection;

			if (newRoleCollection != null)
			{
				foreach (PersonRoleMapClass newRole in newRoleCollection)
				{
					newRole.PersonGuid = person.IdentityGuid;
					int item = 0;
					foreach (PersonRoleMapClass existingRole in existingRoleCollection)
					{
						if (existingRole.Role == newRole.Role)
						{
							break;
						}

						item++;
					}

					if (item == existingRoleCollection.Count)
					{
						personRoleMaps.Add(security, newRole);
					}
					else
					{
						existingRoleCollection.RemoveAt(item);
					}
				}
			}

			foreach (PersonRoleMapClass personRole in existingRoleCollection)
			{
				if (personRole.Role == PERSON_ROLE.SUPERVISOR_ROLE)
				{
					// Any Personnel for which this Person is a supervisor
					// need to have there SupervisorIndex cleared.
					PersonCollectionClass personCollection = this.Enumerate(security);
					foreach (PersonClass subordinate in personCollection)
					{
						if (subordinate.SupervisorGuid == person.MasterRecordGuid)
						{
							var subordinateDetail = this.Get(security, subordinate.MasterRecordGuid);
							subordinateDetail.SupervisorGuid = Guid.Empty;
							subordinateDetail.SupervisorID = null;
							this.Modify(security, DATA_TYPE.CONFIG, subordinateDetail);
						}
					}
				}

				personRoleMaps.Purge(security, personRole.PersonGuid, personRole.Role);
			}
		}

		private void Validate(SecurityClass security, PersonClass person)
		{
			if (person.ID == string.Empty)
			{
				throw new Exception("ID Required");
			}

			if (person.ID == "{None}" || person.ID == "{Unassigned}" || person.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + person.ID);
			}

			if (person.ID.Length > 50)
			{
				throw new Exception("ID length must not exceed 50 characters.");
			}

			if (person.FirstName == string.Empty || person.LastName == string.Empty)
			{
				throw new Exception(person.ID + " - " + "Name Required");
			}

			if (person.RoleCollection.Count == 0)
			{
				throw new Exception(person.ID + " - " + "Role Required");
			}

			if (person.SupervisorGuid != Guid.Empty)
			{
				PersonClass supervisor = this.Get(security, person.SupervisorGuid);

				if (supervisor.IdentityGuid.IsEmpty())
				{
					throw new Exception(person.ID + " - " + "Supervisor Not Found");
				}

				int item = 0;
				foreach (PersonRoleMapClass personRoleMap in supervisor.RoleCollection)
				{
					if (personRoleMap.Role == PERSON_ROLE.SUPERVISOR_ROLE)
					{
						break;
					}

					item++;
				}

				if (item == supervisor.RoleCollection.Count)
				{
					throw new Exception(person.ID + " - " + "Supervisor Not Found");
				}
			}

			var hardwareKey = new HardwareKeyClass();

			// Verify the UserIndex
			if (person.UserGuid != Guid.Empty)
			{
				var users = new UsersClass();
				UserClass user = users.Get(security, person.UserGuid);
				if (user.IdentityGuid.IsEmpty())
				{
					throw new Exception(person.ID + " - " + "User Not Found");
				}
			}

			if (person.LastName.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in LastName must not contain comma or apostrophe.");
			}

			if (person.FirstName.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in FirstName must not contain comma or apostrophe.");
			}

			if (person.MiddleName.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in MiddleName must not contain comma or apostrophe.");
			}

			if (person.Department.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in Department must not contain comma or apostrophe.");
			}

			if (person.Phone1.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in Phone1 must not contain comma or apostrophe.");
			}

			if (person.Phone2.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in Phone2 must not contain comma or apostrophe.");
			}

			if (person.Title.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in Title must not contain comma or apostrophe.");
			}

			if (person.Country.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in Country must not contain comma or apostrophe.");
			}

			if (person.City.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in City must not contain comma or apostrophe.");
			}

			if (person.State.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in State must not contain comma or apostrophe.");
			}

			if (person.Address1.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in Address1 must not contain comma or apostrophe.");
			}

			if (person.Address2.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in Address2 must not contain comma or apostrophe.");
			}

			if (person.Department.IndexOfAny(new[] { ',', '\'' }) > -1)
			{
				throw new Exception("Value in Department must not contain comma or apostrophe.");
			}

			if (person.LaborRate1 >= 1000000 || person.LaborRate1 < 0)
			{
				throw new Exception("Value for Labor Rate 1 must be between 0 and 1000000.");
			}

			if (person.LaborRate2 >= 1000000 || person.LaborRate2 < 0)
			{
				throw new Exception("Value for Labor Rate 2 must be between 0 and 1000000.");
			}

			if (person.LaborRate3 >= 1000000 || person.LaborRate3 < 0)
			{
				throw new Exception("Value for Labor Rate 3 must be between 0 and 1000000.");
			}

			if (person.LaborRate4 >= 1000000 || person.LaborRate4 < 0)
			{
				throw new Exception("Value for Labor Rate 4 must be between 0 and 1000000.");
			}

			string[] emailparts = person.Email.Split('@');
			if (emailparts.Length == 2 && (emailparts[0].Length == 0 || emailparts[emailparts.Length - 1].Length == 0))
			{
				throw new Exception("Email value must contain at least one @ in proper location.");
			}

			var objAlphaNumeric = new Regex("[^-@._a-zA-Z0-9]");
			if (objAlphaNumeric.IsMatch(person.Email))
			{
				throw new Exception("Email value includes invalid characters.");
			}

			if (person.ID.Length > 50)
			{
				throw new Exception("ID field length exceeds its maximum limit of 50 characters.");
			}

			if (person.LastName.Length > 30)
			{
				throw new Exception("LastName field length exceeds its maximum limit of 30 characters.");
			}

			if (person.FirstName.Length > 20)
			{
				throw new Exception("FirstName field length exceeds its maximum limit of 20 characters.");
			}

			if (person.MiddleName.Length > 20)
			{
				throw new Exception("MiddleName field length exceeds its maximum limit of 20 characters.");
			}

			if (person.Address1.Length > 50)
			{
				throw new Exception("Address1 field length exceeds its maximum limit of 50 characters.");
			}

			if (person.Address2.Length > 50)
			{
				throw new Exception("Address2 field length exceeds its maximum limit of 50 characters.");
			}

			if (person.City.Length > 60)
			{
				throw new Exception("City field length exceeds its maximum limit of 60 characters.");
			}

			if (person.State.Length > 20)
			{
				throw new Exception("State field length exceeds its maximum limit of 20 characters.");
			}

			if (person.Zip.Length > 10)
			{
				throw new Exception("Zip field length exceeds its maximum limit of 10 characters.");
			}

			if (!hardwareKey.IsADFKey() && (person.Zip.Length < 5 && person.Zip.Length > 0))
			{
				throw new Exception("Zip field length must not be less than 5 characters.");
			}

			if (hardwareKey.IsADFKey() && (person.Zip.Length < 4 && person.Zip.Length > 0))
			{
				// JS20100908 WI-17505
				throw new Exception("Zip field length must not be less than 4 characters.");
			}

			if (person.Country.Length > 20)
			{
				throw new Exception("Country field length exceeds its maximum limit of 20 characters.");
			}

			if (person.Title.Length > 50)
			{
				throw new Exception("Title field length exceeds its maximum limit of 50 characters.");
			}

			if (person.Phone1.Length > 20)
			{
				throw new Exception("Phone1 field length exceeds its maximum limit of 20 characters.");
			}

			if (person.Phone2.Length > 20)
			{
				throw new Exception("Phone2 field length exceeds its maximum limit of 20 characters.");
			}

			if (person.Email.Length > 50)
			{
				throw new Exception("Email field length exceeds its maximum limit of 50 characters.");
			}

			if (person.SupervisionDate.Length > 10)
			{
				throw new Exception("SupervisionDate field length exceeds its maximum limit of 10 characters.");
			}

			if (person.SupervisionDate.Length < 4 && person.SupervisionDate.Length > 0)
			{
				throw new Exception("SupervisionDate field length must not be less than 4 characters.");
			}

			if (person.AssignmentDate.Length > 10)
			{
				throw new Exception("AssignmentDate field length exceeds its maximum limit of 10 characters.");
			}

			if (person.AssignmentDate.Length < 4 && person.AssignmentDate.Length > 0)
			{
				throw new Exception("AssignmentDate field length must not be less than 4 characters.");
			}

			this.ValidateUserData(security, person);
		}

		public DataSet EnumerateCardedInPersonnelPartTimeoutPeriod(SecurityClass security, DateTimeOffset timeOutPeriod)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				 && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
				 && !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetPersonnelEnumerateByCardedInAndDateTime";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters.Add("@TimeOutDate", SqlDbType.DateTimeOffset);
				cmd.Parameters["@TimeOutDate"].Value = timeOutPeriod;
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}
			return set;
		}
	}
}