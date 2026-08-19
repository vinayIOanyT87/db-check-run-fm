
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Summary description for CompanyMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CompanyMapsClass : IDependency, ICompanyMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, CompanyMapClass companyMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (companyMap == null)
			{
				throw new ArgumentNullException(nameof(companyMap));
			}

			Guid identityGuid = this.GetIdentityGuidByGuidsAndType(security,
															companyMap.AssignedToGuid,
															companyMap.AssignedGuid,
															companyMap.Type);
			if (identityGuid != Guid.Empty)
			{
				// Duplicates can be expected if Company is Carrier and ShipTo
				// and carries for itself
				if (companyMap.Type == COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP)
				{
					return identityGuid;
				}

				throw (new Exception("Company Map Exists"));
			}

			if (companyMap.Type == COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP)
			{
				if (Guid.Empty != this.GetIdentityGuidByMapID(security, companyMap.MapID))
				{
					throw (new Exception("Duplicate ID"));
				}
			}

			companyMap.SiteGuid = security.SiteGuid;
			companyMap.CreatedDate = DateTimeOffset.Now;
			companyMap.CreatedBy = security.UserID;
			companyMap.UpdatedDate = companyMap.CreatedDate;
			companyMap.UpdatedBy = security.UserID;
			companyMap.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return companyMap.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, CompanyMapClass companyMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (companyMap == null)
			{
				throw new ArgumentNullException(nameof(companyMap));
			}

			Guid identityGuid = this.GetIdentityGuidByGuidsAndType(security,
																companyMap.AssignedToGuid,
																companyMap.AssignedGuid,
																companyMap.Type);
			if (identityGuid != Guid.Empty
			&& identityGuid != companyMap.IdentityGuid)
			{
				throw (new Exception("Company Map Exists"));
			}

			if (companyMap.Type == COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP)
			{
				identityGuid = this.GetIdentityGuidByMapID(security, companyMap.MapID);
				if (identityGuid != Guid.Empty
				&& identityGuid != companyMap.IdentityGuid)
				{
					throw (new Exception("Duplicate ID"));
				}
			}

			companyMap.UpdatedDate = DateTimeOffset.Now;
			companyMap.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid, COMPANY_MAP_TYPE type)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			CompanyMapClass companyMap = this.Get(security, identityGuid, type);
			if (companyMap.IdentityGuid == Guid.Empty)
			{
				return;
			}

			DependenciesClass dependencies = new DependenciesClass(security);
			dependencies.Purge(security, companyMap);

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public CompanyMapClass Get(SecurityClass security, Guid identityGuid, COMPANY_MAP_TYPE type)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(type);
			companyMap.IdentityGuid = identityGuid;
			companyMap.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				companyMap.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return companyMap;
		}

		public CompanyMapCollectionClass GetLoadRackCompanyMapClasses(SecurityClass security, Guid billtoshipperidentityGuid)
		{
			// this is used by the loadrack and will return all of the map classes in one call instead of four
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var companyMapCollection = new CompanyMapCollectionClass();

			CompanyMapClass companyMapbilltoshipper = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
			companyMapbilltoshipper.IdentityGuid = billtoshipperidentityGuid;
			companyMapbilltoshipper.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMapbilltoshipper.SelectSQLMinimal(cmd);
				companyMapbilltoshipper.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
			companyMapCollection.Add(companyMapbilltoshipper);

			// get the SHIPPER_OWNER_MAP
			CompanyMapClass companyMapshippermap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
			companyMapshippermap.IdentityGuid = companyMapbilltoshipper.AssignedToGuid;
			companyMapshippermap.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMapshippermap.SelectSQLMinimal(cmd);
				companyMapshippermap.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
			companyMapCollection.Add(companyMapshippermap);


			// get the LOAD_OWNER_MANAGER_MAP
			CompanyMapClass companyMapownermap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
			companyMapownermap.IdentityGuid = companyMapshippermap.AssignedToGuid;
			companyMapownermap.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMapownermap.SelectSQLMinimal(cmd);
				companyMapownermap.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
			companyMapCollection.Add(companyMapownermap);

			return companyMapCollection;
		}

		public Guid GetIdentityGuidByGuidsAndType(SecurityClass security,
															Guid assignedToGuid,
															Guid assignedGuid,
															COMPANY_MAP_TYPE type)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(type);
			companyMap.SiteGuid = security.SiteGuid;
			companyMap.AssignedToGuid = assignedToGuid;
			companyMap.AssignedGuid = assignedGuid;

			DataSet set = null;
			DataSet set1 = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.SelectByGuidsAndTypeSQL(cmd, ContextUtil.IsInTransaction);

				set1 = this.ConsolidatedDA.GetDataSet(cmd, security);

				if(set1.Tables[0].Rows.Count == 0)
				{
					DataSet set2 = null;
					using (var cmd1 = new SqlCommand())
					{
						companyMap.SelectByGuidsAndTypeSQL(cmd1, ContextUtil.IsInTransaction,true);
						set2 = this.ConsolidatedDA.GetDataSet(cmd1, security);
					}
					set = set2;
				}
				else
					set = set1;

				companyMap.Load(set);
			}
			return companyMap.IdentityGuid;
		}

		/// <summary>
		/// This function is for getting the identity guid specifically
		/// of a Load ID to ShipTo map.  Based on that constraint, simplifications
		/// can be make
		/// 
		/// Personnel check is NOT made here.
		/// </summary>
		/// <param name="security">Valid security context</param>
		/// <param name="id">Load ID to look for</param>
		/// <returns>Identity guid of the mapping.  Guid.Empty if a mapping is not found</returns>
		public Guid GetIdentityGuidByMapID(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			Guid identityGuid = Guid.Empty;

			CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP);
			companyMap.SiteGuid = security.SiteGuid;
			companyMap.MapID = id;
			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.SelectIdentityGuidByTypeAndMapIdsql(cmd);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				if ((set?.Tables?.Count ?? 0) == 1)
				{
					DataTable identityGuidTable = set.Tables[0];

					// For a given site, the LoadID is supposed to be unique.  Therefore, we
					// should have only 0 (load ID not found) or 1 (load ID found).  Anything else
					// we'll return as not found.
					if (identityGuidTable.Rows.Count == 1)
					{
						identityGuid = DataObject.getValue(identityGuidTable.Rows[0]["CompanyPersonnelToShipToBillToGuid"], Guid.Empty);
					}
				}
			}
			return identityGuid;
		}

		public Guid GetOffLoadIdentityGuidByMapID(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

         Guid identityGuid = Guid.Empty;

         CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP);
			companyMap.SiteGuid = security.SiteGuid;
			companyMap.MapID = id;
			using (SqlCommand cmd = new SqlCommand())
			{
            companyMap.SelectIdentityGuidByTypeAndMapIdsql(cmd);
            DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

            if ((set?.Tables?.Count ?? 0) == 1)
            {
               DataTable identityGuidTable = set.Tables[0];

               // For a given site, the OffloadID is supposed to be unique.  Therefore, we
               // should have only 0 (load ID not found) or 1 (load ID found).  Anything else
               // we'll return as not found.
               if (identityGuidTable.Rows.Count == 1)
               {
                  identityGuid = DataObject.getValue(identityGuidTable.Rows[0]["CompanyPersonnelToSupplierOwnerGuid"], Guid.Empty);
               }
            }
         }
         return identityGuid;
		}

		/// <summary>
		/// This method will enumerate a company map collection based on the company map type.
		/// </summary>
		/// <param name="security">Contains the security info.</param>
		/// <param name="assignedToGuid">The assigned GUID.</param>
		/// <param name="mapType">The type of company mapping.</param>
		/// <returns>Returns a company map colllection of the mappings.</returns>
		public CompanyMapCollectionClass EnumerateByAssignedToGuidAndType(SecurityClass security, Guid assignedToGuid, COMPANY_MAP_TYPE mapType)
		{
			if (security.Equals(null))
			{
				throw new ArgumentNullException(nameof(security));
			}
			var companyMapCollection = new CompanyMapCollectionClass();
			companyMapCollection = EnumerateByAssignedToGuidAndTypeInternal(security, assignedToGuid, security.SiteGuid, mapType);

			return companyMapCollection;
		}


		/// <summary>
		/// Retrieves a company mapping collection for a given combination of AssignedToGuid, SiteGuid, and map type.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="assignedToGuid"></param>
		/// <param name="siteGuid"></param>
		/// <param name="mapType"></param>
		/// <returns></returns>
		private CompanyMapCollectionClass EnumerateByAssignedToGuidAndTypeInternal(SecurityClass security, Guid assignedToGuid, Guid siteGuid, COMPANY_MAP_TYPE mapType)
		{
			if (security.Equals(null))
			{
				throw new ArgumentNullException(nameof(security));
			}
			bool usingFieldLevelControl = false;
			var companyMap = CompanyMapClass.CreateCompanyMap(mapType);
			companyMap.SiteGuid = siteGuid;
			companyMap.AssignedToGuid = assignedToGuid;

			DataSet set = null;
			DataSet set1 = null;

			using (var cmd = new SqlCommand())
			{
				if (mapType == COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP)
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetCompanyAuthorizedCarrierToCompanyByAssignedToCompany";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@AssignedToCompanyGuid", SqlDbType.UniqueIdentifier);

					cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
					cmd.Parameters["@AssignedToCompanyGuid"].Value = assignedToGuid;
				}
				else if (mapType == COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP)
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetCompanyToUserGroupMappingsByUserGroup";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@AssignedToUserGroupGuid", SqlDbType.UniqueIdentifier);

					cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
					cmd.Parameters["@AssignedToUserGroupGuid"].Value = assignedToGuid;
				}
				else
				{
					usingFieldLevelControl = true;

					companyMap.EnumerateByAssignedToGuidAndTypeSQL(cmd, ContextUtil.IsInTransaction);
				}

				set1 = this.ConsolidatedDA.GetDataSet(cmd, security);

				if (usingFieldLevelControl &&
					set1.Tables[0].Rows.Count == 0)
				{
					// this could be where no field level control is being used but it is assigned so rerun
					//var cmd1 = new SqlCommand();
					DataSet set2 = null;
					using (var cmd1 = new SqlCommand())
					{
						companyMap.EnumerateByAssignedToGuidAndTypeSQL(cmd1, ContextUtil.IsInTransaction, true);
						set2 = this.ConsolidatedDA.GetDataSet(cmd1, security);
					}
					set = set2;
				}
				else
				{
					set = set1;
				}
			}

			var companyMapCollection = new CompanyMapCollectionClass();

			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				companyMap = CompanyMapClass.CreateCompanyMap(mapType);
				companyMap.Load(set);
				companyMapCollection.Add(companyMap);

				table.Rows.RemoveAt(0);
			}

			return companyMapCollection;
		}


		/// <summary>
		/// This method will enumerate a company map collection based on the company map type.
		/// </summary>
		/// <param name="security">Contains the security info.</param>
		/// <param name="assignedGuid">The assigned GUID.</param>
		/// <param name="mapType">The type of company mapping.</param>
		/// <returns>Returns a company map colllection of the mappings.</returns>
		public CompanyMapCollectionClass EnumerateByAssignedGuidAndType(SecurityClass security, Guid assignedGuid, COMPANY_MAP_TYPE mapType)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}
			var companyMapCollection = new CompanyMapCollectionClass();
			companyMapCollection = EnumerateByAssignedGuidAndTypeInternal(security, assignedGuid, security.SiteGuid, mapType);

			return companyMapCollection;
		}


		/// <summary>
		/// Retrieves a company mapping collection for a given combination of AssignedGuid, SiteGuid, and map type.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="assignedGuid">Assigned Guid</param>
		/// <param name="siteGuid">Target Site guid of the mappings</param>
		/// <param name="mapType">Company Mapping Type</param>
		/// <returns></returns>
		private CompanyMapCollectionClass EnumerateByAssignedGuidAndTypeInternal(SecurityClass security, Guid assignedGuid, Guid siteGuid, COMPANY_MAP_TYPE mapType)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			bool usingFieldLevelControl = false;
			CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(mapType);
			companyMap.SiteGuid = siteGuid;
			companyMap.AssignedGuid = assignedGuid;

			DataSet set = null;
			DataSet set1 = null;

			using (var cmd = new SqlCommand())
			{
				if (mapType == COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP)
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetCompanyAuthorizedCarrierToCompanyByCompany";

					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

					cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
					cmd.Parameters["@CompanyGuid"].Value = assignedGuid;
				}
				else
				{
					usingFieldLevelControl = true;
					companyMap.EnumerateByAssignedGuidAndTypeSQL(cmd, security);
				}

				set1 = this.ConsolidatedDA.GetDataSet(cmd, security);

				if (usingFieldLevelControl &&
					set1.Tables[0].Rows.Count == 0)
				{
					// this could be where no field level control is being used but it is assigned so rerun
					DataSet set2 = null;
					using (var cmd1 = new SqlCommand())
					{
						companyMap.EnumerateByAssignedGuidAndTypeSQL(cmd1, security, true);
						set2 = this.ConsolidatedDA.GetDataSet(cmd1, security);
					}
					set = set2;
				}
				else
				{
					set = set1;
				}
			}

			var companyMapCollection = new CompanyMapCollectionClass();
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				companyMap = CompanyMapClass.CreateCompanyMap(mapType);
				companyMap.Load(set);
				companyMapCollection.Add(companyMap);

				table.Rows.RemoveAt(0);
			}

			return companyMapCollection;
		}


		public CompanyMapCollectionClass EnumerateByType(SecurityClass security, COMPANY_MAP_TYPE type)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(type);
			companyMap.SiteGuid = security.SiteGuid;

			DataSet set = null;
			DataSet set1 = null;


			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.EnumerateByTypeSQL(cmd);
				set1 = this.ConsolidatedDA.GetDataSet(cmd, security);

				if (set1.Tables[0].Rows.Count == 0)
				{
					// this could be where no field level control is being used but it is assigned so rerun
					DataSet set2 = null;
					using (var cmd1 = new SqlCommand())
					{
						companyMap.EnumerateByTypeSQL(cmd1,true);
						set2 = this.ConsolidatedDA.GetDataSet(cmd1, security);
					}
					set = set2;
				}
				else
				{
					set = set1;
				}

			}

			CompanyMapCollectionClass companyMapCollection = new CompanyMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				companyMap = CompanyMapClass.CreateCompanyMap(type);
				companyMap.Load(set);

				companyMapCollection.Add(companyMap);
				table.Rows.RemoveAt(0);
			}

			return companyMapCollection;
		}


		/// <summary>
		/// The enumerate by site for user group mapping.
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
		public CompanyMapCollectionClass EnumerateBySiteForUserGroupMapping(SecurityClass security, Guid targetSiteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "map.usp_GetCompanyToUserGroupMapBySite ";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyMapCollection = new CompanyMapCollectionClass();

			var table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);
				companyMap.Load(set);

				companyMapCollection.Add(companyMap);
				table.Rows.RemoveAt(0);
			}

			return companyMapCollection;
		}

		/// <summary>
		/// The enumerate by site for user Authorized Carrier mapping.
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
		public CompanyMapCollectionClass EnumerateBySiteForAuthorizedCarrierMapping(SecurityClass security, Guid targetSiteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "map.usp_GetCompanyToAuthorizedCarrierBySite ";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyMapCollection = new CompanyMapCollectionClass();

			var table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
				companyMap.Load(set);
				companyMapCollection.Add(companyMap);
				table.Rows.RemoveAt(0);
			}

			return companyMapCollection;
		}

		/// <summary>
		/// The enumerate by site for user Carrier Customer ShipTo mapping.
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
		public CompanyMapCollectionClass EnumerateBySiteForCarrierCustomerShipToMapping(SecurityClass security, Guid targetSiteGuid)
		{
			const string SecurityExceptionText = "Security";
			if (security == null)
			{
				throw new ArgumentNullException(SecurityExceptionText);
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "map.usp_GetCompanyToCarrierCustomerShipToBySite ";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var table = set.Tables[0];
			var companyMapCollection = new CompanyMapCollectionClass();
			while (table.Rows.Count != 0)
			{
				var companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
				companyMap.Load(set);
				companyMapCollection.Add(companyMap);
				table.Rows.RemoveAt(0);
			}

			return companyMapCollection;
		}

		public List<Guid> EnumerateGroupMapsWithAllCompaniesAssigned(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT DISTINCT GroupGuid FROM map.tblCompanyCompanyToUserGroup WHERE CompanyGuid IS NULL";
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var table = set.Tables[0];
			var groupList = new List<Guid>();

			for (var index = 0; index < table.Rows.Count; ++index)
			{
				DataRow row = table.Rows[index];
				groupList.Add(new Guid(row["GroupGuid"].ToString()));
			}

			return groupList;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security, Guid identityGuid, string id, CompanyMapCollectionClass newCompanyMapCollection, CompanyMapCollectionClass existingCompanyMapCollection)
		{
			if (newCompanyMapCollection != null)
			{
				foreach (CompanyMapClass newCompanyMap in newCompanyMapCollection)
				{
					newCompanyMap.AssignedToGuid = identityGuid;
					newCompanyMap.AssignedToID = id;

					if (existingCompanyMapCollection != null)
					{
						int item = 0;
						foreach (CompanyMapClass existingCompanyMap in existingCompanyMapCollection)
						{
							if (existingCompanyMap.AssignedGuid == newCompanyMap.AssignedGuid)
							{
								break;
							}

							item++;
						}

						if (item == existingCompanyMapCollection.Count)
						{
							this.Add(security, newCompanyMap);
						}
						else
						{
							existingCompanyMapCollection.Remove(item);
						}
					}
					else
					{
						this.Add(security, newCompanyMap);
					}
				}
			}

			if (existingCompanyMapCollection != null)
			{
				foreach (CompanyMapClass existingCompanyMap in existingCompanyMapCollection)
				{
					this.Purge(security, existingCompanyMap.IdentityGuid, existingCompanyMap.Type);
				}
			}
		}

		//private void PurgeCollection(SecurityClass security, CompanyMapCollectionClass companyMapCollection)
		//{
		//	if (companyMapCollection != null)
		//	{
		//		foreach (CompanyMapClass associatedMap in companyMapCollection)
		//		{
		//			this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
		//		}
		//	}
		//}

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

			if (!preOperation && Object is CompanyRoleMapClass)
			{
				CompanyRoleMapClass companyRoleMap = (CompanyRoleMapClass)Object;
				if (companyRoleMap.Role == COMPANY_ROLE.CUSTOMER_SHIPTO)
				{
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

			var person = Object as PersonClass;
			if (person != null)
			{
				CompanyMapCollectionClass companyMapCollection = this.EnumerateByAssignedGuidAndType(security, person.MasterRecordGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP);
				foreach (CompanyMapClass companyMap in companyMapCollection)
				{
					this.Purge(security, companyMap.IdentityGuid, companyMap.Type);
				}
			}
			else
			{
				var companyRoleMap = Object as CompanyRoleMapClass;
				if (companyRoleMap != null)
				{
					CompanyMapCollectionClass companyMapCollection = null;

					// Remove CompanyMaps associated with Carrier Authorization
					if (companyRoleMap.Role == COMPANY_ROLE.CARRIER)
					{
						companyMapCollection = this.EnumerateByAssignedGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
					}
					else if (companyRoleMap.Role == COMPANY_ROLE.CUSTOMER_SHIPTO)
					{
						companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
					}

					if (companyMapCollection != null)
					{
						var companies = new CompaniesClass();
						CompanyClass company =  companies.Get(security, companyRoleMap.CompanyGuid);						
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							if ((company.HasRole(COMPANY_ROLE.CARRIER)) 
								&& (associatedMap.Type == COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP)
								&& (associatedMap.AssignedGuid == Guid.Empty))
                            {
								continue;
                            }
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}
					}

					// Remove CompanyMaps associated with Hierarchy
					if (companyRoleMap.Role == COMPANY_ROLE.MANAGER)
					{
						companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}

						companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}
					}
					else if (companyRoleMap.Role == COMPANY_ROLE.OWNER)
					{
						companyMapCollection = this.EnumerateByAssignedGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}

						companyMapCollection = this.EnumerateByAssignedGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}
					}
					else if (companyRoleMap.Role == COMPANY_ROLE.SHIPPER)
					{
						companyMapCollection = this.EnumerateByAssignedGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}

						companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}
					}
					else if (companyRoleMap.Role == COMPANY_ROLE.CUSTOMER_BILLTO)
					{
						companyMapCollection = this.EnumerateByAssignedGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}

						companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}
					}
					else if (companyRoleMap.Role == COMPANY_ROLE.CUSTOMER_SHIPTO)
					{
						companyMapCollection = this.EnumerateByAssignedGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}
					}
					else if (companyRoleMap.Role == COMPANY_ROLE.SUPPLIER)
					{
						companyMapCollection = this.EnumerateByAssignedGuidAndTypeInternal(security, companyRoleMap.CompanyGuid, companyRoleMap.SiteGuid, COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP);
						foreach (CompanyMapClass associatedMap in companyMapCollection)
						{
							this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
						}
					}
				}
				else
				{
					var companyMap = Object as CompanyMapClass;
					if (companyMap != null)
					{
						CompanyMapCollectionClass companyMapCollection = null;

						if (companyMap.Type == COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
						{
							companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyMap.IdentityGuid, companyMap.SiteGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
						}
						else if (companyMap.Type == COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
						{
							companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyMap.IdentityGuid, companyMap.SiteGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
						}
						else if (companyMap.Type == COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
						{
							companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyMap.IdentityGuid, companyMap.SiteGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);
						}
						else if (companyMap.Type == COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
						{
							companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyMap.IdentityGuid, companyMap.SiteGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP);
						}
						else if (companyMap.Type == COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
						{
							companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyMap.IdentityGuid, companyMap.SiteGuid, COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP);
						}
						else if (companyMap.Type == COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP)
						{
							companyMapCollection = this.EnumerateByAssignedToGuidAndTypeInternal(security, companyMap.IdentityGuid, companyMap.SiteGuid, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP);
						}

						if (companyMapCollection != null)
						{
							foreach (CompanyMapClass associatedMap in companyMapCollection)
							{
								this.Purge(security, associatedMap.IdentityGuid, associatedMap.Type);
							}
						}

						return;
					}
				}
			}
		}

		public CompanyMapClass GetLoadIdMapWithoutPersonnelCheck(SecurityClass security, Guid loadIDToCompanyShipToMapGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP);
			companyMap.IdentityGuid = loadIDToCompanyShipToMapGuid;
			companyMap.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.SelectSQLMinimal(cmd);
				companyMap.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return companyMap;
		}

		public CompanyMapClass GetMinimal(SecurityClass security, Guid identityGuid, COMPANY_MAP_TYPE type)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(type);
			companyMap.IdentityGuid = identityGuid;
			companyMap.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				companyMap.SelectSQLMinimal(cmd);
				companyMap.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return companyMap;
		}
	}
}
