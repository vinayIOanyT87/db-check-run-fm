using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Summary description for CompanyRoleMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CompanyRoleMapsClass : IDependency, ICompanyRoleMaps
	{
		private const string DbTriggerError001 = "DB_TRIGGER_ERROR_CompanyRoleMap_001";
		private const string DbTriggerErrorMsg001 = "Cannot have multiple owner roles due to Site Enforce Single Owner.";
		private const string DbTriggerError002 = "DB_TRIGGER_ERROR_CompanyRoleMap_002";
		private const string DbTriggerErrorMsg002 = "Cannot have multiple manager, owner, shipper, bill to roles due to Site Auto Create Company Hierarchy.";
		private const string DbTriggerError003 = "DB_TRIGGER_ERROR_CompanyRoleMap_003";
		private const string DbTriggerErrorMsg003 = "Cannot have multiple manager roles due to Site Enforce Single Owner.";

		#region Protected Data members
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
		#endregion



		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, CompanyRoleMapClass companyRoleMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (companyRoleMap == null)
			{
				throw new ArgumentNullException("companyRoleMap");
			}

			if (companyRoleMap.SiteGuid == Guid.Empty)
			{
				companyRoleMap.SiteGuid = security.SiteGuid;
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				companyRoleMap.SelectSQL(cmd);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count != 0)
				return;

			companyRoleMap.CreatedDate = DateTimeOffset.Now;
			companyRoleMap.CreatedBy = security.UserID;
			try
			{
				using (var cmd = new SqlCommand())
				{
					companyRoleMap.InsertSQL(cmd);
					ConsolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception except)
			{
				if (except.Message.IndexOf(DbTriggerError001, StringComparison.Ordinal) > -1)
				{
					throw new Exception(DbTriggerErrorMsg001);
				}
				
				if (except.Message.IndexOf(DbTriggerError002, StringComparison.Ordinal) > -1)
				{
					throw new Exception(DbTriggerErrorMsg002);
				}
				
				if (except.Message.IndexOf(DbTriggerError003, StringComparison.Ordinal) > -1)
				{
					throw new Exception(DbTriggerErrorMsg003);
				}
				
				throw;
			}


			var dependencies = new DependenciesClass(security);
			dependencies.Insert(security, companyRoleMap, false);

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByRole(SecurityClass security, Guid companyGuid, COMPANY_ROLE role)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var companyRoleMap = new CompanyRoleMapClass { CompanyGuid = companyGuid, Role = role, SiteGuid = security.SiteGuid };

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, companyRoleMap);

			using (var cmd = new SqlCommand())
			{
				companyRoleMap.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}


		/// <summary>
		/// This method will purge a row in the tblCompanyRoleMaps table based on the
		/// Site Guid, Company Guid, and Role within the CompanyRoleMap object.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="companyRoleMap"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, CompanyRoleMapClass companyRoleMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, companyRoleMap);

			using (var cmd = new SqlCommand())
			{
				companyRoleMap.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		public CompanyRoleMapCollectionClass EnumerateByCompany(SecurityClass security, Guid companyGuid)
		{
			return EnumerateByCompany2(security, security.SiteGuid, companyGuid);
		}

		public CompanyRoleMapCollectionClass EnumerateByCompany2(SecurityClass security, Guid targetSiteGuid, Guid companyGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var companyRoleMap = new CompanyRoleMapClass { CompanyGuid = companyGuid, SiteGuid = targetSiteGuid };
			DataSet set;
			using (var cmd = new SqlCommand())
			{
				companyRoleMap.EnumerateByCompanySQL(cmd);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			var companyRoleMapCollection = new CompanyRoleMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				companyRoleMap = new CompanyRoleMapClass();
				companyRoleMap.Load(set);
				companyRoleMapCollection.Add(companyRoleMap);
				table.Rows.RemoveAt(0);
			}

			return companyRoleMapCollection;
		}

		/// <summary>
		/// The enumerate by site for user role mapping.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetSiteGuid">
		/// The target site globally unique identifier.
		/// </param>
		/// <returns>
		/// The <see cref="CompanyMapCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Security is null
		/// </exception>
		public List<CompanyRoleMapClass> EnumerateBySiteForRoleMapping(SecurityClass security, Guid targetSiteGuid)
		{
			const string SecurityExceptionText = "security";
			if (security == null)
			{
				throw new ArgumentNullException(SecurityExceptionText);
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "map.usp_GetCompanyToRoleBySite ";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var list = new List<CompanyRoleMapClass>();

			var table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var companyRoleMap = new CompanyRoleMapClass();
				companyRoleMap.Load(set);
				list.Add(companyRoleMap);
				table.Rows.RemoveAt(0);
			}

			return list;
		}


		/// <summary>
		/// Enumerates the by criterion.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="inSiteGuid">The in site GUID.</param>
		/// <param name="inFindString">The in find string.</param>
		/// <param name="inCompanyGuid">The in company GUID.</param>
		/// <param name="inRole">The in role.</param>
		/// <param name="includeMemberSites">if set to <c>true</c> [include member sites].</param>
		/// <param name="sortKey">The sort key.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">Invalid Security</exception>
		/// <exception cref="System.Exception"></exception>
		public List<CompanyRoleMapClass> EnumerateByCriterion(SecurityClass security,
																													Guid inSiteGuid,
																													string inFindString,
																													Guid inCompanyGuid,
																													COMPANY_ROLE inRole,
																													bool includeMemberSites,
																													string sortKey)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var combineRoleHsh = new Hashtable();

			var list = new List<CompanyRoleMapClass>();
			var companyRoleMap = new CompanyRoleMapClass();

			try
			{
				DataSet dataSet;

				using (var cmd = new SqlCommand())
				{
					companyRoleMap.EnumerateByCriterionSQL(cmd, inSiteGuid,
													inFindString,
													inCompanyGuid,
													inRole,
													includeMemberSites,
													security.LoginSiteGuid);
					dataSet = ConsolidatedDA.GetDataSet(cmd, security);
				}

				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					foreach (DataRow row in table.Rows)
					{
						if (row != null)
						{
							companyRoleMap = new CompanyRoleMapClass();
							companyRoleMap.LoadByCriterionRow(row);

							string hashKey = companyRoleMap.CompanyGuid + "|" + companyRoleMap.SiteGuid;

							if (combineRoleHsh.Contains(hashKey))
							{
								var currentCompanyRoleMap = combineRoleHsh[hashKey] as CompanyRoleMapClass;
								currentCompanyRoleMap.JoinRoles(companyRoleMap.Role);
							}
							else
							{
								combineRoleHsh.Add(hashKey, companyRoleMap);
							}
						}
					}

					IDictionaryEnumerator enumerator = combineRoleHsh.GetEnumerator();

					while (enumerator.MoveNext())
					{
						list.Add((CompanyRoleMapClass)enumerator.Value);
					}

					// Sort the List either by the Company Name, Site, or Company ID.
					// The default sorting is Company ID.
					if (sortKey.ToUpper().Equals("NAME"))
					{
						list.Sort((class1, class2) => (Comparer<string>.Default.Compare(class1.CompanyName, class2.CompanyName)));
					}
					else if (sortKey.ToUpper().Equals("SITE"))
					{
						list.Sort((class1, class2) => (Comparer<string>.Default.Compare(class1.SiteID, class2.SiteID)));
					}
					else
					{
						list.Sort((class1, class2) => (Comparer<string>.Default.Compare(class1.CompanyID, class2.CompanyID)));
					}

					// Insert the Apply To All row at the beginning of the list.
					companyRoleMap = new CompanyRoleMapClass
					                 {
						                 CompanyID = "Apply To All",
						                 HasBillToRole = false,
						                 HasCarrierRole = false,
						                 HasManagerRole = false,
						                 HasOwnerRole = false,
						                 HasShipperRole = false,
						                 HasShipToRole = false,
						                 HasSupplierRole = false
					                 };

					list.Insert(0, companyRoleMap);

					// Insert a separator row between the Apply To All row and the rest of the rows.
					companyRoleMap = new CompanyRoleMapClass
					                 {
						                 CompanyID = "",
						                 HasBillToRole = false,
						                 HasCarrierRole = false,
						                 HasManagerRole = false,
						                 HasOwnerRole = false,
						                 HasShipperRole = false,
						                 HasShipToRole = false,
						                 HasSupplierRole = false
					                 };

					list.Insert(1, companyRoleMap);
				}
			}
			catch (Exception)
			{
				throw new Exception("Error retrieving or loading company role map data.");
			}

			//return companyRoleMapCollection;
			return list;
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

		}


		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
			{
				var entityToSiteMap = (EntityToSiteMapClass)Object;

				if ( entityToSiteMap.TypeID == ENTITY_TYPE.COMPANY )
				{
					Guid siteGuid = security.SiteGuid;
					security.SiteGuid = entityToSiteMap.SiteGuid;

					try
					{
						CompanyRoleMapCollectionClass roles = EnumerateByCompany(security, entityToSiteMap.SiteGuid);
						foreach (CompanyRoleMapClass role in roles)
						{
							Purge(security, role);
						}
					}
					finally
					{
						security.SiteGuid = siteGuid;
					}
				}
			}
		}
	}
}
