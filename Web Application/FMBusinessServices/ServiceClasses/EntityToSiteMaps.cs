// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityToSiteMaps.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Implementation of the IEntityToSiteMaps service class
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class EntityToSiteMaps : IEntityToSiteMaps
	{
		#region Constants and Fields

		/// <summary>
		///	The database access object.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		/// <summary>Adds the specified entity to site map.</summary>
		/// <param name="security">The security.</param>
		/// <param name="entityToSiteMap">The entity to site map.</param>
		/// <param name="engineTypeGuid">The engine type GUID.</param>
		/// <exception cref="System.ArgumentNullException">Security object null</exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, EntityToSiteMapClass entityToSiteMap, Guid engineTypeGuid)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				if (entityToSiteMap == null)
				{
					throw new ArgumentNullException("entityToSiteMap");
				}

				if (engineTypeGuid == null)
				{
					throw new ArgumentNullException("engineTypeGuid");
				}

			var dependencies = new DependenciesClass( security );
			
			this.AddInternal(security, entityToSiteMap, dependencies, false);
		}


		/// <summary>
		/// Add an entity-to-site mapping for an Equipment. Equipment is handled separately from the other entities to support the automatic propagation of the mappings to the child Compartments of the target Equipment.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="entityToSiteMap"></param>
		/// <param name="extendToCompartments"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddEquipmentMapping(SecurityClass security, EntityToSiteMapClass entityToSiteMap, bool extendToCompartments)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (entityToSiteMap == null)
			{
				throw new ArgumentNullException("entityToSiteMap");
			}

			var dependencies = new DependenciesClass(security);

			this.AddInternal(security, entityToSiteMap, dependencies, extendToCompartments);
		}


		private void AddInternal(SecurityClass security, EntityToSiteMapClass entityToSiteMap, DependenciesClass dependencies, bool extendToCompartments)
		{
			EntityToSiteMapClass dbEntityToSiteMap = this.GetByRecordGuid(
				security,
				entityToSiteMap.TypeID,
				entityToSiteMap.IdentityGuid,
				entityToSiteMap.SiteGuid);

			if (dbEntityToSiteMap != null)
			{
				if (dbEntityToSiteMap.IdentityGuid != Guid.Empty)
				{
					return;
				}
			}

			Guid siteGuid = security.SiteGuid;

			security.SiteGuid = entityToSiteMap.SiteGuid;
			security.LoginSiteGuid = entityToSiteMap.SiteGuid;

			entityToSiteMap.CreatedDate = DateTimeOffset.Now;
			entityToSiteMap.CreatedBy = security.UserID;
			entityToSiteMap.UpdatedDate = entityToSiteMap.CreatedDate;
			entityToSiteMap.UpdatedBy = security.UserID;
			if (entityToSiteMap.AssignedFromSiteGuid == null || entityToSiteMap.AssignedFromSiteGuid == Guid.Empty)
			{
				entityToSiteMap.AssignedFromSiteGuid = siteGuid;
			}

			this.ValidateMapping(security, entityToSiteMap);

			dependencies.Insert(security, entityToSiteMap, true);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = this.GetMappingInsertQueryName(entityToSiteMap.TypeID);
				cmd.Parameters.Add("@EntityRecordGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@AssignedFromSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@AssignedToSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
				
				cmd.Parameters["@EntityRecordGuid"].Value = entityToSiteMap.IdentityGuid;
				cmd.Parameters["@AssignedFromSiteGuid"].Value = entityToSiteMap.AssignedFromSiteGuid;
				cmd.Parameters["@AssignedToSiteGuid"].Value = entityToSiteMap.SiteGuid;
				cmd.Parameters["@CreatedBy"].Value = entityToSiteMap.CreatedBy;
				cmd.Parameters["@CreatedDate"].Value = entityToSiteMap.CreatedDate;

				if (entityToSiteMap.TypeID == ENTITY_TYPE.EQUIPMENT)
				{
					cmd.Parameters.Add("@ExtendToCompartments", SqlDbType.Bit);
					cmd.Parameters["@ExtendToCompartments"].Value = extendToCompartments;
				}

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			dependencies.Insert(security, entityToSiteMap, false);

			this.UpdateChangeLog(security, entityToSiteMap, ChangeQueueEventType.Add);

			security.SiteGuid = siteGuid;
		}


		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void AddList( SecurityClass security, List<EntityToSiteMapClass> addList, Guid entityEngineTypeGuid )
		{
			var dependencies = new DependenciesClass( security );

			foreach ( var map in addList )
			{
				this.AddInternal( security, map, dependencies, false );
			}
		}


		/// <summary>
		/// Add a list of Equipment entity-to-site mappings. Equipment is handled separately from the other entities to support the automatic propagation of the mappings to the child Compartments of the target Equipment.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="addList"></param>
		/// <param name="entityEngineTypeGuid"></param>
		/// <param name="extendToCompartments"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddEquipmentMappingList(SecurityClass security, List<EntityToSiteMapClass> addList, bool extendToCompartments)
		{
			var dependencies = new DependenciesClass(security);

			foreach (var map in addList)
			{
				this.AddInternal(security, map, dependencies, extendToCompartments);
			}
		}


		private void ValidateMapping(SecurityClass security, EntityToSiteMapClass entityToSiteMap)
		{
			if (!entityToSiteMap.TypeID.IsEntityTypeSupportsIndividualEntityMapping())
				{					
					//- For entities mapped as a whole there can be only one EntityRecordGuid/OwnerSiteGuid that can be mapped from any given sitegroup, either the EntityRecordGuid/OwnerSiteGuid owned by the AssignedFrom sitegroup itself or the EntityRecordGuid/OwnerSiteGuid assigned to that AssignedFrom sitegroup, but not both.
					EntityToSiteMapCollectionClass entityToSiteMapCollection = EnumerateEntityMapsByAssignedFromSiteGuid(security, entityToSiteMap.TypeID, entityToSiteMap.SiteGuid);
					if ((entityToSiteMapCollection != null) && (entityToSiteMapCollection.Count > 0))
						throw new ApplicationException("Invalid Mapping. There are one or more " + EntityToSiteMapClass.GetEntityTypeID(entityToSiteMap.TypeID) + " mappings assigned from site " + entityToSiteMapCollection[0].AssignedFromSiteId + ".");

					//- Entities that are mapped as a whole do not support multiple assignments to the same site/sitegroup, even if the assignments have different RecordGuids/OwnerSitegroupGuid. E.g. cannot have more than one Data Dictionaries assigned/applied to a site.
					entityToSiteMapCollection = EnumerateEntityMapsBySiteGuid(security, entityToSiteMap.TypeID, entityToSiteMap.SiteGuid, false);
					if ((entityToSiteMapCollection != null) && (entityToSiteMapCollection.Count > 0))
						throw new ApplicationException("Invalid Mapping. There is already a " + EntityToSiteMapClass.GetEntityTypeID(entityToSiteMap.TypeID) + " mapping assigned to site " + entityToSiteMapCollection[0].SiteID + ".");
				}
		}

		/// <summary>Enumerates maps by type ID and GUID.</summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="identityGuid">The identity GUID.</param>
		/// <returns>A collection of entity to site maps.</returns>
		public EntityToSiteMapCollectionClass EnumerateByTypeIDAndGuid(
				SecurityClass security, ENTITY_TYPE entityType, Guid identityGuid)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				DataSet set;
				using (var cmd = new SqlCommand())
				{
					//entityToSiteMap.EnumerateByTypeIDAndGuidSQL(cmd);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = GetMappingReadQueryName(entityType, ContextUtil.IsInTransaction);
					cmd.Parameters.Add("@EntityRecordGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@AssignedToSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@IncludeChildrenSites", SqlDbType.Bit);
					cmd.Parameters["@EntityRecordGuid"].Value = identityGuid;
					cmd.Parameters["@AssignedToSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters["@IncludeChildrenSites"].Value = 1;
					set = this.consolidatedDA.GetDataSet(cmd, security);
				}
				EntityToSiteMapClass entityToSiteMap = null;
				EntityToSiteMapCollectionClass entityToSiteMapCollection = new EntityToSiteMapCollectionClass();
				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					entityToSiteMap = new EntityToSiteMapClass { TypeID = entityType };
					entityToSiteMap.Load(set);
					entityToSiteMapCollection.Add(entityToSiteMap);
					table.Rows.RemoveAt(0);
				}
				return entityToSiteMapCollection;
		}

		/// <summary>
		/// Enumerates the by type ID and site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <returns> A collection of entity to site maps.</returns>
		public EntityToSiteMapCollectionClass EnumerateByTypeIDAndSiteGuid(
				SecurityClass security, ENTITY_TYPE entityType, Guid siteGuid)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				return EnumerateEntityMapsBySiteGuid(security, entityType, siteGuid, false);
		}

		/// <summary>
		/// Enumerates the entity sites.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="selectedSiteGuid">The selected site GUID.</param>
		/// <param name="includeMembers">if set to <c>true</c> includes member sites.</param>
		/// <returns>A list of entity sites</returns>
		public List<KeyValuePair<Guid, string>> EnumerateEntitySites(SecurityClass security, Guid selectedSiteGuid, bool includeMembers)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				DataSet set;

				using (var cmd = new SqlCommand())
				{
					cmd.CommandText = (includeMembers
													? "WITH memberSites(c, p, t, l) AS " + " ( "
													+ " SELECT childSiteGuid, parentsiteGuid, parentsiteGuid, 0 FROM map.tblsitetosite WHERE childSiteGuid <> parentsiteGuid "
													+ " UNION ALL "
													+ " SELECT childSiteGuid, parentsiteGuid, t, 1 FROM map.tblsitetosite JOIN memberSites ON c = parentsiteGuid AND childSiteGuid <> parentsiteGuid "
													+ " ) "
													+ " SELECT DISTINCT c AS SiteGuid, ID FROM memberSites m JOIN tblSites s ON m.c = s.SiteGuid WHERE t = @SiteGuid UNION "
													: string.Empty) + " SELECT SiteGuid, ID FROM tblSites WHERE SiteGuid = @SiteGuid";

					cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

					cmd.Parameters["@SiteGuid"].Value = selectedSiteGuid;

					set = this.consolidatedDA.GetDataSet(cmd, security);
				}

				var sites = new List<KeyValuePair<Guid, string>>();

				DataTable table = set.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					var site = new KeyValuePair<Guid, string>(
						DataObject.getValue(row["SiteGuid"], Guid.Empty), DataObject.getValue(row["ID"], string.Empty));

					sites.Add(site);
				}

				return sites;
		}

		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="guid">The GUID.</param>
		/// <returns> A collection of entity to site maps.</returns>
		public EntityToSiteMapClass Get(SecurityClass security, ENTITY_TYPE entityType, Guid guid)
		{
				return GetByRecordGuid(security, entityType, guid, security.SiteGuid);
		}


		/// <summary>
		/// Returns the EntityToSiteMap of a given entity record to a given site
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="entityType">The type of the entity</param>
		/// <param name="identityGuid">The guid of the entity record. For entity records under record versioning, this should be the MasterRecordGuid.</param>
		/// <param name="assignedToSiteGuid">The target AssignedTo site for which the mapping is to be retrieved.</param>
		/// <returns></returns>
		public EntityToSiteMapClass GetByRecordGuid(SecurityClass security, ENTITY_TYPE entityType, Guid identityGuid, Guid assignedToSiteGuid)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				DataSet set;
				using (var cmd = new SqlCommand())
				{
					//entityToSiteMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = GetMappingReadQueryName(entityType, ContextUtil.IsInTransaction);
					cmd.Parameters.Add("@EntityRecordGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@AssignedToSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@IncludeChildrenSites", SqlDbType.Bit);
					cmd.Parameters["@EntityRecordGuid"].Value = identityGuid;
					cmd.Parameters["@AssignedToSiteGuid"].Value = assignedToSiteGuid;
					cmd.Parameters["@IncludeChildrenSites"].Value = 0;
					set = this.consolidatedDA.GetDataSet(cmd, security);
				}
				EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass();
				entityToSiteMap.Load(set);
				return entityToSiteMap;
		}


        /// <summary>
        /// Retrieve the RecordVersion specific fields of an entity record.
        /// </summary>
        /// <param name="security">The FuelsManager security object.
        /// </param>
        /// <param name="entityType">
        /// Entity Type of the entity record
        /// </param>
        /// <param name="masterRecordGuid">
        /// MaterRecordGuid of the entity record
        /// </param>
        /// <param name="flcMode">
        /// FLCMode for which to limit the query
        /// </param>
        /// <returns>
        /// A list of RecordVersioning fields/>.
        /// </returns>
        public List<string> GetRecordVersioningFields(SecurityClass security, ENTITY_TYPE entityType, Guid masterRecordGuid, string flcMode)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            List<string> lstVsFields = new List<string>();

            EntityToSiteMapClass entityToSiteMap = GetByRecordGuid(security, entityType, masterRecordGuid, security.SiteGuid);

            if ((entityToSiteMap == null) || (entityToSiteMap.AssignedFromSiteGuid == Guid.Empty))
            {
                return lstVsFields;
            }

            DataSet set;
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetRecordVersioningFields";
                cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@EntityMasterRecGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@AssignedFromSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@FieldLevelControlMode", SqlDbType.NVarChar, 40);

                cmd.Parameters["@EntityTypeId"].Value = entityType.ToString();
                cmd.Parameters["@EntityMasterRecGuid"].Value = masterRecordGuid;
                cmd.Parameters["@AssignedFromSiteGuid"].Value = entityToSiteMap.AssignedFromSiteGuid;
                cmd.Parameters["@FieldLevelControlMode"].Value = flcMode;

                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            DataTable table = set.Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string targetField = Convert.ToString(table.Rows[i]["InternalFieldName"]);
                if ((targetField == null) || (targetField.Length == 0))
                    targetField = Convert.ToString(table.Rows[i]["TargetField"]);
                lstVsFields.Add(targetField);
            }

            return lstVsFields;
        }

        /// <summary>
        /// This method will return true if the entity is assigned. Otherwise, it returns false.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="siteGuid">The site GUID.</param>
        /// <param name="assignedGuid">The assigned GUID.</param>
        /// <returns>
        ///	<c>true</c> if the specified entity is assigned; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Security object is null.</exception>
        public bool IsAssigned(SecurityClass security, ENTITY_TYPE entityType, Guid siteGuid, Guid assignedGuid)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}
				bool result = false;
				EntityToSiteMapClass dbEntityToSiteMap = GetByRecordGuid(security, entityType, assignedGuid, siteGuid);
				if ((dbEntityToSiteMap != null) && (dbEntityToSiteMap.IdentityGuid != Guid.Empty))
					result = true;
				return result;
		}

		/// <summary>
		/// Purges the list of entity maps.  All maps in list must be same entity type.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="purgeList"></param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void PurgeList( SecurityClass security, List<EntityToSiteMapClass> purgeList )
		{
			if (purgeList.Count > 0)
			{
				using (var cmd = new SqlCommand())
				{
					var dependencies = new DependenciesClass(security);
					this.PrepareCommandForPurge(cmd, purgeList[0].TypeID);

					foreach (var map in purgeList)
					{
						dependencies.Purge(security, map);
						this.PurgeInternal(security, cmd, map);
						this.UpdateChangeLog( security, map, ChangeQueueEventType.Purge );
					}
				}
			}
		}

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityToSiteMap">The entity to site map.</param>
		/// <exception cref="System.ArgumentNullException">Security object null.</exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, EntityToSiteMapClass entityToSiteMap)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				if (entityToSiteMap == null)
				{
					throw new ArgumentNullException("entityToSiteMap");
				}

				var dependencies = new DependenciesClass(security);
				dependencies.Purge(security, entityToSiteMap);

				using (var cmd = new SqlCommand())
				{
					this.PrepareCommandForPurge(cmd, entityToSiteMap.TypeID);
				this.PurgeInternal(security, cmd, entityToSiteMap);
				}

				this.UpdateChangeLog(security, entityToSiteMap, ChangeQueueEventType.Purge);
		}

		/// <summary>
		/// Prepares SqlCommand object for purge operation
		/// </summary>
		/// <param name="cmd">SqlCommand object to prepare.</param>
		/// <param name="typeId">The entity type to prepare purge for.</param>
		private void PrepareCommandForPurge(SqlCommand cmd, ENTITY_TYPE typeId)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = GetMappingPurgeQueryName( typeId );
			cmd.Parameters.Add( "@EntityRecordGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters.Add( "@AssignedToSiteGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters.Add( "@DeleteBaseMapping", SqlDbType.Bit );
		}

		/// <summary>
		/// Perform actual call to database for purge operation
		/// </summary>
		/// <param name="security"></param>
		/// <param name="cmd"></param>
		/// <param name="entityToSiteMap"></param>
		private void PurgeInternal(SecurityClass security, SqlCommand cmd, EntityToSiteMapClass entityToSiteMap)
		{
			cmd.Parameters["@EntityRecordGuid"].Value = entityToSiteMap.IdentityGuid;
			cmd.Parameters["@AssignedToSiteGuid"].Value = entityToSiteMap.SiteGuid;
			cmd.Parameters["@DeleteBaseMapping"].Value = 0;
			this.consolidatedDA.ExecuteQuery( security, cmd );
		}

		/// <summary>
		/// Deletes all the entity-to-site assignments associated with a given entity record, including the base assignment (assignment that an entity record maintains with its owner site/sitegroup).
		/// This operation is only supported for entity types that support Record Versioning.
		/// This operation will typically be called prior to deleting an entity record.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="entityMasterRecordGuid">The entity master record GUID.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeAll(SecurityClass security, ENTITY_TYPE entityType, Guid entityMasterRecordGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (entityMasterRecordGuid == null)
			{
				throw new ArgumentNullException("entityMasterRecordGuid");
			}
			PurgeAllInternall(security, entityType, entityMasterRecordGuid, true);
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeAllEquipmentMappings(SecurityClass security, Guid entityMasterRecordGuid, bool extendToCompartments)
		{
			PurgeAllInternall(security, ENTITY_TYPE.EQUIPMENT, entityMasterRecordGuid, extendToCompartments);
		}


		private void PurgeAllInternall(SecurityClass security, ENTITY_TYPE entityType, Guid entityMasterRecordGuid, bool extendToCompartments)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (entityMasterRecordGuid == null)
			{
				throw new ArgumentNullException("entityMasterRecordGuid");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;

				if (entityType == ENTITY_TYPE.EQUIPMENT)
				{
					cmd.CommandText = "map.usp_EquipmentToSiteDelete";
					cmd.Parameters.Add("@ExtendToCompartments", SqlDbType.Bit);
					cmd.Parameters["@ExtendToCompartments"].Value = extendToCompartments;
				}
				else if (entityType == ENTITY_TYPE.PRODUCT)
					cmd.CommandText = "map.usp_ProductToSiteDelete";
				else if (entityType == ENTITY_TYPE.COMPANY)
					cmd.CommandText = "map.usp_CompanyToSiteDelete";
				else if (entityType == ENTITY_TYPE.TRANSACTION_ALIAS)
					cmd.CommandText = "map.usp_TransactionAliasToSiteDelete";
				else if (entityType == ENTITY_TYPE.PERSONNEL)
					cmd.CommandText = "map.usp_PersonnelToSiteDelete";
				else
				{
					throw new ArgumentOutOfRangeException("entityType");
				}

				cmd.Parameters.Add("@EntityRecordGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@EntityRecordGuid"].Value = entityMasterRecordGuid;
				cmd.Parameters.Add("@AssignedToSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@AssignedToSiteGuid"].Value = DBNull.Value;
				cmd.Parameters.Add("@DeleteBaseMapping", SqlDbType.Bit);
				cmd.Parameters["@DeleteBaseMapping"].Value = 1;

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass security, EntityToSiteMapClass entityToSiteMap)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				if (entityToSiteMap == null)
				{
					throw new ArgumentNullException("entityToSiteMap");
				}

			SitesClass sites = new SitesClass();
			entityToSiteMap.SiteGuid = sites.GetIdentityGuid(security, entityToSiteMap.SiteID);
			entityToSiteMap.AssignedFromSiteGuid = security.SiteGuid;
			Guid engineTypeGuid = Guid.Empty;

			switch(entityToSiteMap.TypeID)
			{
				case ENTITY_TYPE.EQUIPMENT:
				{
					EquipmentsClass equipments = new EquipmentsClass();
					engineTypeGuid = equipments.GetType().GUID;
					entityToSiteMap.IdentityGuid = equipments.GetMasterRecordGuid(security, entityToSiteMap.ID);
					break;
				}

				case ENTITY_TYPE.FUEL_CARD:
				{
					FuelCardsClass fuelCards = new FuelCardsClass();
					engineTypeGuid = fuelCards.GetType().GUID;
					entityToSiteMap.IdentityGuid = fuelCards.GetIdentityGuid(security, entityToSiteMap.ID);
					break;
				}

				case ENTITY_TYPE.PRODUCT:
				{
					ProductsClass products = new ProductsClass();
					engineTypeGuid = products.GetType().GUID;
					entityToSiteMap.IdentityGuid = products.GetMasterRecordGuidFromID(security, entityToSiteMap.ID);
					break;
				}

				case ENTITY_TYPE.EQUIPMENT_TYPE:
				{
					EquipmentTypesClass equipmentTypes = new EquipmentTypesClass();
					engineTypeGuid = equipmentTypes.GetType().GUID;
					entityToSiteMap.IdentityGuid = equipmentTypes.GetIdentityGuid(security, entityToSiteMap.ID);
					break;
				}

				case ENTITY_TYPE.COMPANY:
				{
					CompaniesClass companies = new CompaniesClass();
					engineTypeGuid = companies.GetType().GUID;
					entityToSiteMap.IdentityGuid = companies.GetMasterRecordGuid(security, entityToSiteMap.ID);
					break;
				}

				case ENTITY_TYPE.PERSONNEL:
				{
					PersonnelClass personnel = new PersonnelClass();
					engineTypeGuid = personnel.GetType().GUID;
					entityToSiteMap.IdentityGuid = personnel.GetMasterRecordGuid(security, entityToSiteMap.ID);
					break;
				}

				case ENTITY_TYPE.STANDING_OFFER:
				{
					StandingOffersClass standingOffers = new StandingOffersClass();
					engineTypeGuid = standingOffers.GetType().GUID;
					entityToSiteMap.IdentityGuid = standingOffers.GetIdentityGuid(security, entityToSiteMap.ID);
					break;
				}

				case ENTITY_TYPE.IATA_CODE:
				{
					IATACodesClass iataCodes = new IATACodesClass();
					engineTypeGuid = iataCodes.GetType().GUID;
					entityToSiteMap.IdentityGuid = iataCodes.GetIdentityGuid(security, entityToSiteMap.ID);
					break;
				}

				default:
					throw new Exception("Import : Unsupported EntityType - " + entityToSiteMap.EntityType.ToString());

			}

			Add(security, entityToSiteMap, engineTypeGuid);

			return;
		}


		#endregion

		#region Methods

		/// <summary>
		/// Add a new mapping for the given object and engine Guid
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="srcObject">The SRC object.</param>
		/// <param name="engineTypeGuid">The engine type GUID.</param>
		internal static void AddNewMap(SecurityClass security, BaseDataObject srcObject, Guid engineTypeGuid)
		{
				var newMap = new EntityToSiteMapClass(srcObject);
				(new EntityToSiteMaps()).Add(security, newMap, engineTypeGuid);
		}

		/// <summary>
		/// Remove all mappings for the given type and object
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="targetGuid">The target GUID.</param>
		/// <exception cref="System.ArgumentNullException">Security object null.</exception>
		internal static void RemoveAllMapsForEntity(SecurityClass security, ENTITY_TYPE entityType, Guid targetGuid)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				if (targetGuid == Guid.Empty)
				{
					throw new ArgumentNullException("targetGuid");
				}

				// Purge from EntityToSiteMap
				var entityToSiteMaps = new EntityToSiteMaps();
				var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, entityType, targetGuid);

				foreach (var entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}
		}

		/// <summary>
		/// Updates the change log.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityToSiteMap">The entity to site map.</param>
		/// <param name="type">The change queue event type.</param>
		private void UpdateChangeLog(SecurityClass security, EntityToSiteMapClass entityToSiteMap, ChangeQueueEventType type)
		{
				BaseDataObject dataObject = null;

				if (entityToSiteMap.TypeID.Equals(ENTITY_TYPE.COMPANY))
				{
					dataObject = new CompanyClass();
				}
				else if (entityToSiteMap.TypeID.Equals(ENTITY_TYPE.EQUIPMENT))
				{
					dataObject = new EquipmentClass();
				}
				else if (entityToSiteMap.TypeID.Equals(ENTITY_TYPE.FUEL_CARD))
				{
					dataObject = new FuelCardClass();
				}
				else if (entityToSiteMap.TypeID.Equals(ENTITY_TYPE.PERSONNEL))
				{
					dataObject = new PersonClass();
				}
				else if (entityToSiteMap.TypeID.Equals(ENTITY_TYPE.PRODUCT))
				{
					dataObject = new ProductClass();
				}
				else if (entityToSiteMap.TypeID.Equals(ENTITY_TYPE.GROUP))
				{
					dataObject = new GroupClass();
				}
				else if (entityToSiteMap.TypeID.Equals(ENTITY_TYPE.TRANSACTION_ALIAS))
				{
					dataObject = new TransactionAliasClass();
				}

				if (dataObject != null)
				{
					dataObject.IdentityGuid = entityToSiteMap.IdentityGuid;
					dataObject.ID = entityToSiteMap.ID;
					dataObject.CreatedBy = security.UserID;
					dataObject.CreatedDate = DateTimeOffset.Now;
					dataObject.UpdatedBy = security.UserID;
					dataObject.UpdatedDate = DateTimeOffset.Now;

					Guid saveSiteGuid = security.SiteGuid;
					try
					{
						security.SiteGuid = entityToSiteMap.SiteGuid;

						// TODO: Temporary commented out so that QA does not test change queue features.
						// ChangeQueueRecordsClass.ProcessChangeQueueRecords(security, type, dataObject, false);
					}
					finally
					{
						security.SiteGuid = saveSiteGuid;
					}
				}
		}


		public EntityToSiteMapCollectionClass EnumerateEntityMapsBySiteGuid(SecurityClass security, ENTITY_TYPE entityType, Guid assignedToSiteGuid, bool excludeCompartments)
		{
				EntityToSiteMapCollectionClass entityToSiteMapCollection = new EntityToSiteMapCollectionClass();
				DataSet set;
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = this.GetMappingListToSiteReadQueryName(entityType);
					cmd.Parameters.Add("@AssignedToSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@AssignedToSiteGuid"].Value = assignedToSiteGuid;
					if (entityType == ENTITY_TYPE.EQUIPMENT)
					{
						cmd.Parameters.Add("@ExcludeCompartments", SqlDbType.Bit);
						cmd.Parameters["@ExcludeCompartments"].Value = excludeCompartments;
					}
					set = this.consolidatedDA.GetDataSet(cmd, security);
				}

				for (int i = 0; i < set.Tables[0].Rows.Count; i++)
				{
					DataRow row = set.Tables[0].Rows[i];
				    EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass
				                                           {
				                                               TypeID               = entityType,
				                                               ID                   = DataObject.getValue(row["EntityId"], string.Empty),
				                                               SiteGuid             = DataObject.getValue(row["AssignedToSiteGuid"], Guid.Empty),
				                                               SiteID               = DataObject.getValue(row["AssignedToSiteId"], string.Empty),
				                                               IdentityGuid         = DataObject.getValue(row["EntityRecordGuid"], Guid.Empty),
				                                               AssignedFromSiteGuid = DataObject.getValue(row["AssignedFromSiteGuid"], Guid.Empty),
				                                               AssignedFromSiteId   = DataObject.getValue(row["AssignedFromSiteId"], string.Empty)
				                                           };

				    if (entityType == ENTITY_TYPE.USER)
				    {
				        bool activeDirectoryUser = DataObject.getValue(row["ActiveDirectoryUser"], false);
				        if (activeDirectoryUser) entityToSiteMap.DisableSelection = true;
				    }

					entityToSiteMapCollection.Add(entityToSiteMap);
				}

				return entityToSiteMapCollection;
		}



		public EntityToSiteMapCollectionClass EnumerateEntityMapsByAssignedFromSiteGuid(SecurityClass security, ENTITY_TYPE entityType, Guid assignedFromSiteGuid)
		{
				EntityToSiteMapCollectionClass entityToSiteMapCollection = new EntityToSiteMapCollectionClass();
				DataSet set;
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = GetMappingListFromSiteReadQueryName(entityType);
					cmd.Parameters.Add("@AssignedFromSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@AssignedFromSiteGuid"].Value = assignedFromSiteGuid;
					set = this.consolidatedDA.GetDataSet(cmd, security);
				}

				for (int i = 0; i < set.Tables[0].Rows.Count; i++)
				{
					DataRow row = set.Tables[0].Rows[i];
					EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass();
					entityToSiteMap.TypeID = entityType;
					entityToSiteMap.ID = DataObject.getValue(row["EntityId"], string.Empty);
					entityToSiteMap.SiteGuid = DataObject.getValue(row["AssignedToSiteGuid"], Guid.Empty);
					entityToSiteMap.SiteID = DataObject.getValue(row["AssignedToSiteId"], string.Empty);
					entityToSiteMap.IdentityGuid = DataObject.getValue(row["EntityRecordGuid"], Guid.Empty);
					entityToSiteMap.AssignedFromSiteGuid = DataObject.getValue(row["AssignedFromSiteGuid"], Guid.Empty);
					entityToSiteMap.AssignedFromSiteId = DataObject.getValue(row["AssignedFromSiteId"], string.Empty);
					entityToSiteMapCollection.Add(entityToSiteMap);
				}

				return entityToSiteMapCollection;
		}


		private string GetMappingInsertQueryName(ENTITY_TYPE entityType)
		{
			switch (entityType)
			{
				case ENTITY_TYPE.ADDITIVE_PROFILE:
					return "map.usp_CreateAdditiveProfileToSiteMapping";
				case ENTITY_TYPE.ALARM_AND_EVENT:
					return "map.usp_CreateAlarmAndEventToSiteMapping";
				case ENTITY_TYPE.ALARM_EVENT_CATEGORY:
					return "map.usp_CreateAlarmAndEventCategoryToSiteMapping";
				case ENTITY_TYPE.ALARM_PRIORITY:
					return "map.usp_CreateAlarmPriorityToSiteMapping";
				case ENTITY_TYPE.ALLOCATION_GROUP:
					return "map.usp_CreateAllocationGroupToSiteMapping";
				case ENTITY_TYPE.APPOINTMENT_EQUIPMENT:
					return "map.usp_CreateAppointmentEquipmentToSiteMapping";
				case ENTITY_TYPE.APPOINTMENT_PERSONNEL:
					return "map.usp_CreateAppointmentPersonnelToSiteMapping";
				case ENTITY_TYPE.APPOINTMENT_TANK:
					return "map.usp_CreateAppointmentTankToSiteMapping";
				case ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE:
					return "map.usp_CreateAutoDistributionReasonCodeToSiteMapping";
				case ENTITY_TYPE.AUTODISTRIBUTION_RULE:
					return "map.usp_CreateAutoDistributionRuleToSiteMapping";
				case ENTITY_TYPE.COMPANY:
					return "map.usp_CreateCompanyToSiteMapping";  //RecordVersioning-aware query 
				case ENTITY_TYPE.QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT:
					return "map.usp_CreateCompanyCertificateAndPermitToSiteMapping";
				case ENTITY_TYPE.COMPANY_GROUP:
					return "map.usp_CreateCompanyGroupToSiteMapping";
				case ENTITY_TYPE.COMPANY_TYPE:
					return "map.usp_CreateCompanyTypeToSiteMapping";
				case ENTITY_TYPE.DATA_DICTIONARY:
					return "map.usp_CreateDataDictionaryToSiteMapping";
				case ENTITY_TYPE.DISPATCH_CONFIGURATION:
					return "map.usp_CreateDispatchConfigurationToSiteMapping";
				case ENTITY_TYPE.DOT_HAZARDOUS_MESSAGE:
					return "map.usp_CreateDotHazardousMessagesToSiteMapping";
				case ENTITY_TYPE.EMAIL_ADDRESS:
					return "map.usp_CreateEmailAddressToSiteMapping";
				case ENTITY_TYPE.EMAIL_GROUP:
					return "map.usp_CreateEmailGroupToSiteMapping";
				case ENTITY_TYPE.ENTRY_MESSAGE:
					return "map.usp_CreateEntryMessageToSiteMapping";
				case ENTITY_TYPE.EQUIPMENT:
					return "map.usp_CreateEquipmentToSiteMapping";	//RecordVersioning-aware query		
				case ENTITY_TYPE.EXTERNAL_STATION_DEVICE:
					return "map.usp_CreateExternalStationDeviceToSiteMapping";
				case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE:
					return "map.usp_CreateEquipmentTagAndLicenseToSiteMapping";
				case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION:
					return "map.usp_CreateEquipmentTestAndInspectionToSiteMapping";
				case ENTITY_TYPE.EQUIPMENT_TYPE:
					return "map.usp_CreateEquipmentTypeToSiteMapping";  //RecordVersioning-aware query 
				case ENTITY_TYPE.EXIT_MESSAGE:
					return "map.usp_CreateExitMessageToSiteMapping";
				case ENTITY_TYPE.EXTERNAL_STATION:
					return "map.gsp_EntityExternalStationToSiteInsertByPK";
				case ENTITY_TYPE.FOOTNOTE:
					return "map.usp_CreateFootNoteToSiteMapping";
				case ENTITY_TYPE.FUEL_CARD:
					return "map.usp_CreateFuelCardToSiteMapping";
				case ENTITY_TYPE.FUEL_CARD_LIMIT:
					return "map.usp_CreateFuelCardLimitToSiteMapping";
				case ENTITY_TYPE.FUEL_CARD_TYPE:
					return "map.usp_CreateFuelCardTypeToSiteMapping";
				case ENTITY_TYPE.IATA_CODE:
					return "map.usp_CreateIATACodeToSiteMapping";
				case ENTITY_TYPE.ASSET_TRACKING_DEVICE:
					return "map.usp_CreateAssetTrackingDeviceToSiteMapping";
				case ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION:
					return "map.usp_CreateAssetTrackingMapConfigurationToSiteMapping";
				case ENTITY_TYPE.LEDGER_AGGREGATE_COLUMN:
					return "map.usp_CreateLedgerAggregateColumnToSiteMapping";
				case ENTITY_TYPE.LEDGER_VIEW:
					return "map.usp_CreateLedgerViewToSiteMapping";
				case ENTITY_TYPE.LIST_VIEW:
					return "map.usp_CreateListViewToSiteMapping";
				case ENTITY_TYPE.MAINTENANCE_REASON:
					return "map.usp_CreateMaintenanceReasonToSiteMapping";
				case ENTITY_TYPE.MOBILE_DEVICE_PROFILE:
					return "map.usp_CreateMobileDeviceProfileToSiteMapping";
				case ENTITY_TYPE.PERSONNEL:
					return "map.usp_CreatePersonnelToSiteMapping";  //RecordVersioning-aware query
				case ENTITY_TYPE.QUALIFICATION_PERSON_LICENSE:
					return "map.usp_CreatePersonnelLicenseToSiteMapping";
				case ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION:
					return "map.usp_CreatePersonnelQualificationToSiteMapping";
				case ENTITY_TYPE.QUALIFICATION_PERSON_TRAINING:
					return "map.usp_CreatePersonnelTrainingToSiteMapping";
				case ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE:
					return "map.usp_CreateProcessVariableMessageToSiteMapping";
				case ENTITY_TYPE.PRODUCT:
					return "map.usp_CreateProductToSiteMapping";  //RecordVersioning-aware query
				case ENTITY_TYPE.PRODUCT_GROUP:
					return "map.usp_CreateProductGroupToSiteMapping";
				case ENTITY_TYPE.PRODUCT_MESSAGE:
					return "map.usp_CreateProductMessageToSiteMapping";
				case ENTITY_TYPE.QUALITY_TAG:
					return "map.usp_CreateQualityTagToSiteMapping";
				case ENTITY_TYPE.QUERY:
				case ENTITY_TYPE.QUERY_DEFAULT:
				case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
					return "map.usp_CreateQuerySettingToSiteMapping";
				case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
					return "map.usp_CreateReportConfigurationSettingsToSiteMapping";
				case ENTITY_TYPE.STANDING_OFFER:
					return "map.usp_CreateStandingOfferToSiteMapping";
				case ENTITY_TYPE.TEST:
					return "map.usp_CreateTestToSiteMapping";
				case ENTITY_TYPE.TEST_SET:
					return "map.usp_CreateTestSetToSiteMapping";
				case ENTITY_TYPE.TRANSACTION_ALIAS:
				case ENTITY_TYPE.TRANSACTION_ALIAS_NAME:
					return "map.usp_CreateTransactionAliasToSiteMapping";  //RecordVersioning-aware query
				case ENTITY_TYPE.USER:
					return "map.usp_UserToSiteInsert";
				case ENTITY_TYPE.USER_DATA_FIELD:
					return "map.usp_CreateUserDataToSiteMapping";
				case ENTITY_TYPE.GROUP:
					return "map.usp_UserGroupToSiteInsert";
				case ENTITY_TYPE.POINT_TEMPLATE:
					return "map.usp_CreatePointTemplateToSiteMapping";
				case ENTITY_TYPE.POINT_TEMPLATE_TYPE:
					return "map.usp_CreatePointTemplateTypeToSiteMapping";
				case ENTITY_TYPE.POINT_CATEGORY:
					return "map.usp_CreatePointCategoryToSiteMapping";
				case ENTITY_TYPE.MODULE:
					return "map.usp_CreateModuleToSiteMapping";

				default:
						System.Diagnostics.Debug.Assert(false, "Entity to site mapping query name not found.");
						return "Unknown";
			}
		}



		private string GetMappingReadQueryName(ENTITY_TYPE entityType, bool bInTransaction)
		{
				string queryName = null;
				switch (entityType)
				{
					case ENTITY_TYPE.ADDITIVE_PROFILE:
						{ queryName = "map.usp_GetAdditiveProfileToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.ALARM_AND_EVENT:
						{ queryName = "map.usp_GetAlarmAndEventToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.ALARM_EVENT_CATEGORY:
						{ queryName = "map.usp_GetAlarmAndEventCategoryToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.ALARM_PRIORITY:
						{ queryName = "map.usp_GetAlarmPriorityToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.ALLOCATION_GROUP:
						{ queryName = "map.usp_GetAllocationGroupToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.APPOINTMENT_EQUIPMENT:
						{ queryName = "map.usp_GetAppointmentEquipmentToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.APPOINTMENT_PERSONNEL:
						{ queryName = "map.usp_GetAppointmentPersonnelToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.APPOINTMENT_TANK:
						{ queryName = "map.usp_GetAppointmentTankToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE:
						{ queryName = "map.usp_GetAutoDistributionReasonCodeToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.AUTODISTRIBUTION_RULE:
						{ queryName = "map.usp_GetAutoDistributionRuleToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.COMPANY:
						{ queryName = "map.usp_GetCompanyToSiteByRecordGuid"; break; }  //RecordVersioning-aware query 
					case ENTITY_TYPE.QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT:
						{ queryName = "map.usp_GetCompanyCertificateAndPermitToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.COMPANY_GROUP:
						{ queryName = "map.usp_GetCompanyGroupToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.COMPANY_TYPE:
						{ queryName = "map.usp_GetCompanyTypeToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.DATA_DICTIONARY:
						{ queryName = "map.usp_GetDataDictionaryToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.DISPATCH_CONFIGURATION:
						{ queryName = "map.usp_GetDispatchConfigurationToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.DOT_HAZARDOUS_MESSAGE:
						{ queryName = "map.usp_GetDotHazardousMessageToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.EMAIL_ADDRESS:
						{ queryName = "map.usp_GetEmailAddressToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.EMAIL_GROUP:
						{ queryName = "map.usp_GetEmailGroupToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.ENTRY_MESSAGE:
						{ queryName = "map.usp_GetEntryMessageToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.EQUIPMENT:
						{ queryName = "map.usp_GetEquipmentToSiteByRecordGuid"; break; }	//RecordVersioning-aware query				
					case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE:
						{ queryName = "map.usp_GetEquipmentTagAndLicenseToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION:
						{ queryName = "map.usp_GetEquipmentTestAndInspectionToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.EQUIPMENT_TYPE:
						{ queryName = "map.usp_GetEquipmentTypeToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.EXIT_MESSAGE:
						{ queryName = "map.usp_GetExitMessageToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.EXTERNAL_STATION:
						{ queryName = "map.usp_GetExternalStationToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.EXTERNAL_STATION_DEVICE:
						{ queryName = "map.usp_GetExternalStationDeviceToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.FOOTNOTE:
						{ queryName = "map.usp_GetFootNoteToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.FUEL_CARD:
						{ queryName = "map.usp_GetFuelCardToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.FUEL_CARD_LIMIT:
						{ queryName = "map.usp_GetFuelCardLimitToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.FUEL_CARD_TYPE:
						{ queryName = "map.usp_GetFuelCardTypeToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.IATA_CODE:
						{ queryName = "map.usp_GetIATACodeToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.ASSET_TRACKING_DEVICE:
						{ queryName = "map.usp_GetAssetTrackingDeviceToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION:
						{ queryName = "map.usp_GetAssetTrackingMapConfigurationToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.LEDGER_AGGREGATE_COLUMN:
						{ queryName = "map.usp_GetLedgerAggregateColumnToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.LEDGER_VIEW:
						{ queryName = "map.usp_GetLedgerViewToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.LIST_VIEW:
						{ queryName = "map.usp_GetListViewToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.MAINTENANCE_REASON:
						{ queryName = "map.usp_GetMaintenanceReasonToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.MOBILE_DEVICE_PROFILE:
						{ queryName = "map.usp_GetMobileDeviceProfileToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.PERSONNEL:
						{ queryName = "map.usp_GetPersonnelToSiteByRecordGuid"; break; }  //RecordVersioning-aware query
					case ENTITY_TYPE.QUALIFICATION_PERSON_LICENSE:
						{ queryName = "map.usp_GetPersonnelLicenseToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION:
						{ queryName = "map.usp_GetPersonnelQualificationToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.QUALIFICATION_PERSON_TRAINING:
						{ queryName = "map.usp_GetPersonnelTrainingToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE:
						{ queryName = "map.usp_GetProcessVariableMessageToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.PRODUCT:
						{ queryName = "map.usp_GetProductToSiteByRecordGuid"; break; }  //RecordVersioning-aware query
					case ENTITY_TYPE.PRODUCT_GROUP:
						{ queryName = "map.usp_GetProductGroupToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.PRODUCT_MESSAGE:
						{ queryName = "map.usp_GetProductMessageToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.QUALITY_TAG:
						{ queryName = "map.usp_GetQualityTagToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.QUERY:
					case ENTITY_TYPE.QUERY_DEFAULT:
					case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
						{ queryName = "map.usp_GetQuerySettingToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
						{ queryName = "map.usp_GetReportConfigurationSettingsToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.STANDING_OFFER:
						{ queryName = "map.usp_GetStandingOfferToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.TEST:
						{ queryName = "map.usp_GetTestToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.TEST_SET:
						{ queryName = "map.usp_GetTestSetToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.TRANSACTION_ALIAS:
					case ENTITY_TYPE.TRANSACTION_ALIAS_NAME:
						{ queryName = "map.usp_GetTransactionAliasToSiteByRecordGuid"; break; }  //RecordVersioning-aware query
					case ENTITY_TYPE.USER:
						{ queryName = "map.usp_GetUserToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.USER_DATA_FIELD:
						{ queryName = "map.usp_GetUserDataToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.GROUP:
						{ queryName = "map.usp_GetUserGroupToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.POINT_TEMPLATE:
						{ queryName = "map.usp_GetPointTemplateToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.POINT_TEMPLATE_TYPE:
						{ queryName = "map.usp_GetPointTemplateTypeToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.POINT_CATEGORY:
						{ queryName = "map.usp_GetPointCategoryToSiteByRecordGuid"; break; }
					case ENTITY_TYPE.MODULE:
						{ queryName = "map.usp_GetModuleToSiteByRecordGuid"; break; }
					default:
						{
								System.Diagnostics.Debug.Assert(false, "Entity to site mapping query name not found.");
								break;
						}
				}
				return queryName;
		}



		private string GetMappingListToSiteReadQueryName(ENTITY_TYPE entityType)
		{
			switch (entityType)
			{
				case ENTITY_TYPE.ADDITIVE_PROFILE:
					return "map.usp_GetAdditiveProfileToSiteBySiteGuid";
				case ENTITY_TYPE.ALARM_AND_EVENT:
					return "map.usp_GetAlarmAndEventToSiteBySiteGuid";
				case ENTITY_TYPE.ALARM_EVENT_CATEGORY:
					return "map.usp_GetAlarmAndEventCategoryToSiteBySiteGuid";
				case ENTITY_TYPE.ALARM_PRIORITY:
					return "map.usp_GetAlarmPriorityToSiteBySiteGuid";
				case ENTITY_TYPE.ALLOCATION_GROUP:
					return "map.usp_GetAllocationGroupToSiteBySiteGuid";
				case ENTITY_TYPE.APPOINTMENT_EQUIPMENT:
					return "map.usp_GetAppointmentEquipmentToSiteBySiteGuid";
				case ENTITY_TYPE.APPOINTMENT_PERSONNEL:
					return "map.usp_GetAppointmentPersonnelToSiteBySiteGuid";
				case ENTITY_TYPE.APPOINTMENT_TANK:
					return "map.usp_GetAppointmentTankToSiteBySiteGuid";
				case ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE:
					return "map.usp_GetAutoDistributionReasonCodeToSiteBySiteGuid";
				case ENTITY_TYPE.AUTODISTRIBUTION_RULE:
					return "map.usp_GetAutoDistributionRuleToSiteBySiteGuid";
				case ENTITY_TYPE.COMPANY:
					return "map.usp_GetCompanyToSiteBySiteGuid";  //RecordVersioning-aware query 
				case ENTITY_TYPE.QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT:
					return "map.usp_GetCompanyCertificateAndPermitToSiteBySiteGuid";
				case ENTITY_TYPE.COMPANY_GROUP:
					return "map.usp_GetCompanyGroupToSiteBySiteGuid";
				case ENTITY_TYPE.COMPANY_TYPE:
					return "map.usp_GetCompanyTypeToSiteBySiteGuid";
				case ENTITY_TYPE.DATA_DICTIONARY:
					return "map.usp_GetDataDictionaryToSiteBySiteGuid";
				case ENTITY_TYPE.DISPATCH_CONFIGURATION:
					return "map.usp_GetDispatchConfigurationToSiteBySiteGuid";
				case ENTITY_TYPE.DOT_HAZARDOUS_MESSAGE:
					return "map.usp_GetDotHazardousMessageToSiteBySiteGuid";
				case ENTITY_TYPE.EMAIL_ADDRESS:
					return "map.usp_GetEmailAddressToSiteBySiteGuid";
				case ENTITY_TYPE.EMAIL_GROUP:
					return "map.usp_GetEmailGroupToSiteBySiteGuid";
				case ENTITY_TYPE.ENTRY_MESSAGE:
					return "map.usp_GetEntryMessageToSiteBySiteGuid";
				case ENTITY_TYPE.EQUIPMENT:
					return "map.usp_GetEquipmentToSiteBySiteGuid";	//RecordVersioning-aware query				
				case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE:
					return "map.usp_GetEquipmentTagAndLicenseToSiteBySiteGuid";
				case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION:
					return "map.usp_GetEquipmentTestAndInspectionToSiteBySiteGuid";
				case ENTITY_TYPE.EQUIPMENT_TYPE:
					return "map.usp_GetEquipmentTypeToSiteBySiteGuid";
				case ENTITY_TYPE.EXIT_MESSAGE:
					return "map.usp_GetExitMessageToSiteBySiteGuid";
				case ENTITY_TYPE.EXTERNAL_STATION:
					return "map.usp_GetExternalStationToSiteBySiteGuid";
				case ENTITY_TYPE.EXTERNAL_STATION_DEVICE:
					return "map.usp_GetExternalStationDeviceToSiteBySiteGuid";
				case ENTITY_TYPE.FOOTNOTE:
					return "map.usp_GetFootNoteToSiteBySiteGuid";
				case ENTITY_TYPE.FUEL_CARD:
					return "map.usp_GetFuelCardToSiteBySiteGuid";
				case ENTITY_TYPE.FUEL_CARD_LIMIT:
					return "map.usp_GetFuelCardLimitToSiteBySiteGuid";
				case ENTITY_TYPE.FUEL_CARD_TYPE:
					return "map.usp_GetFuelCardTypeToSiteBySiteGuid";
				case ENTITY_TYPE.IATA_CODE:
					return "map.usp_GetIATACodeToSiteBySiteGuid";
				case ENTITY_TYPE.ASSET_TRACKING_DEVICE:
					return "map.usp_GetAssetTrackingDeviceToSiteBySiteGuid";
				case ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION:
					return "map.usp_GetAssetTrackingMapConfigurationToSiteBySiteGuid";
				case ENTITY_TYPE.LEDGER_AGGREGATE_COLUMN:
					return "map.usp_GetLedgerAggregateColumnToSiteBySiteGuid";
				case ENTITY_TYPE.LEDGER_VIEW:
					return "map.usp_GetLedgerViewToSiteBySiteGuid";
				case ENTITY_TYPE.LIST_VIEW:
					return "map.usp_GetListViewToSiteBySiteGuid";
				case ENTITY_TYPE.MAINTENANCE_REASON:
					return "map.usp_GetMaintenanceReasonToSiteBySiteGuid";
				case ENTITY_TYPE.MOBILE_DEVICE_PROFILE:
						return "map.usp_GetMobileDeviceProfileToSiteBySiteGuid";
				case ENTITY_TYPE.PERSONNEL:
					return "map.usp_GetPersonnelToSiteBySiteGuid";  //RecordVersioning-aware query
				case ENTITY_TYPE.QUALIFICATION_PERSON_LICENSE:
					return "map.usp_GetPersonnelLicenseToSiteBySiteGuid";
				case ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION:
					return "map.usp_GetPersonnelQualificationToSiteBySiteGuid";
				case ENTITY_TYPE.QUALIFICATION_PERSON_TRAINING:
					return "map.usp_GetPersonnelTrainingToSiteBySiteGuid";
				case ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE:
					return "map.usp_GetProcessVariableMessageToSiteBySiteGuid";
				case ENTITY_TYPE.PRODUCT:
					return "map.usp_GetProductToSiteBySiteGuid";  //RecordVersioning-aware query
				case ENTITY_TYPE.PRODUCT_GROUP:
					return "map.usp_GetProductGroupToSiteBySiteGuid";
				case ENTITY_TYPE.PRODUCT_MESSAGE:
					return "map.usp_GetProductMessageToSiteBySiteGuid";
				case ENTITY_TYPE.QUALITY_TAG:
					return "map.usp_GetQualityTagToSiteBySiteGuid";
				case ENTITY_TYPE.QUERY:
				case ENTITY_TYPE.QUERY_DEFAULT:
				case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
					return "map.usp_GetQuerySettingToSiteBySiteGuid";
				case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
					return "map.usp_GetReportConfigurationSettingsToSiteBySiteGuid";
				case ENTITY_TYPE.STANDING_OFFER:
					return "map.usp_GetStandingOfferToSiteBySiteGuid";
				case ENTITY_TYPE.TEST:
					return "map.usp_GetTestToSiteBySiteGuid";
				case ENTITY_TYPE.TEST_SET:
					return "map.usp_GetTestSetToSiteBySiteGuid";
				case ENTITY_TYPE.TRANSACTION_ALIAS:
				case ENTITY_TYPE.TRANSACTION_ALIAS_NAME:
					return "map.usp_GetTransactionAliasToSiteBySiteGuid";  //RecordVersioning-aware query
				case ENTITY_TYPE.USER:
					return "map.usp_GetUserToSiteBySiteGuid";
				case ENTITY_TYPE.USER_DATA_FIELD:
					return "map.usp_GetUserDataToSiteBySiteGuid";
				case ENTITY_TYPE.GROUP:
					return "map.usp_GetUserGroupToSiteBySiteGuid";
				case ENTITY_TYPE.POINT_TEMPLATE:
					return "map.usp_GetPointTemplateToSiteBySiteGuid";
				case ENTITY_TYPE.POINT_TEMPLATE_TYPE:
					return "map.usp_GetPointTemplateTypeToSiteBySiteGuid";
				case ENTITY_TYPE.POINT_CATEGORY:
					return "map.usp_GetPointCategoryToSiteBySiteGuid";
				case ENTITY_TYPE.MODULE:
					return "map.usp_GetModuleToSiteBySiteGuid";

				default:
					System.Diagnostics.Debug.Assert(false, "Entity to site mapping query name not found.");
					return "Unknown";
			}
		}


		private string GetMappingListFromSiteReadQueryName(ENTITY_TYPE entityType)
		{
			switch (entityType)
				{
					case ENTITY_TYPE.ALARM_AND_EVENT:
						return "map.usp_GetAlarmAndEventToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.COMPANY:
						return "map.usp_GetCompanyToSiteByAssignedFromSiteGuid";  //RecordVersioning-aware query 
					case ENTITY_TYPE.DATA_DICTIONARY:
							return "map.usp_GetDataDictionaryToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.EQUIPMENT:
						return "map.usp_GetEquipmentToSiteByAssignedFromSiteGuid";  //RecordVersioning-aware query 
					case ENTITY_TYPE.EQUIPMENT_TYPE:
						return "map.usp_GetEquipmentTypeToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.EXTERNAL_STATION_DEVICE:
						return "map.usp_GetExternalStationDeviceToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.FUEL_CARD:
						return "map.usp_GetFuelCardToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.FUEL_CARD_TYPE:
						return "map.usp_GetFuelCardTypeToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.IATA_CODE:
						return "map.usp_GetIATACodeToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.ASSET_TRACKING_DEVICE:
						return "map.usp_GetAssetTrackingDeviceToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION:
						return "map.usp_GetAssetTrackingMapConfigurationToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.PERSONNEL:
						return "map.usp_GetPersonnelToSiteByAssignedFromSiteGuid";  //RecordVersioning-aware query 
					case ENTITY_TYPE.PRODUCT:
						return "map.usp_GetProductToSiteByAssignedFromSiteGuid";  //RecordVersioning-aware query 
					case ENTITY_TYPE.QUERY:
					case ENTITY_TYPE.QUERY_DEFAULT:
					case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
						return "map.usp_GetQuerySettingToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
						return "map.usp_GetReportConfigurationSettingsToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.STANDING_OFFER:
						return "map.usp_GetStandingOfferToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.USER_DATA_FIELD:
							return "map.usp_GetUserDataToSiteByAssignedFromSiteGuid";
					case ENTITY_TYPE.POINT_TEMPLATE:
						return "map.usp_GetPointTemplateToSiteBySiteGuid";
					case ENTITY_TYPE.POINT_TEMPLATE_TYPE:
						return "map.usp_GetPointTemplateTypeToSiteBySiteGuid";
					case ENTITY_TYPE.POINT_CATEGORY:
						return "map.usp_GetPointCategoryToSiteBySiteGuid";
					case ENTITY_TYPE.MODULE:
						return "map.usp_GetModuleToSiteBySiteGuid";

				default:
						System.Diagnostics.Debug.Assert(false, "Entity to site mapping query name not found.");
						return "Unknown";
				}
		}


		private string GetMappingPurgeQueryName(ENTITY_TYPE entityType)
		{
				switch (entityType)
				{
					case ENTITY_TYPE.ADDITIVE_PROFILE:
						return "map.usp_AdditiveProfileToSiteDelete";
					case ENTITY_TYPE.ALARM_AND_EVENT:
						return "map.usp_AlarmAndEventToSiteDelete";
					case ENTITY_TYPE.ALARM_EVENT_CATEGORY:
						return "map.usp_AlarmAndEventCategoryToSiteDelete";
					case ENTITY_TYPE.ALARM_PRIORITY:
						return "map.usp_AlarmPriorityToSiteDelete";
					case ENTITY_TYPE.ALLOCATION_GROUP:
						return "map.usp_AllocationGroupToSiteDelete";
					case ENTITY_TYPE.APPOINTMENT_EQUIPMENT:
						return "map.usp_AppointmentEquipmentToSiteDelete";
					case ENTITY_TYPE.APPOINTMENT_PERSONNEL:
						return "map.usp_AppointmentPersonnelToSiteDelete";
					case ENTITY_TYPE.APPOINTMENT_TANK:
						return "map.usp_AppointmentTankToSiteDelete";
					case ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE:
						return "map.usp_AutoDistributionReasonCodeToSiteDelete";
					case ENTITY_TYPE.AUTODISTRIBUTION_RULE:
						return "map.usp_AutoDistributionRuleToSiteDelete";
					case ENTITY_TYPE.COMPANY:
						return "map.usp_CompanyToSiteDelete";  //RecordVersioning-aware query 
					case ENTITY_TYPE.QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT:
						return "map.usp_CompanyCertificateAndPermitToSiteDelete";
					case ENTITY_TYPE.COMPANY_GROUP:
						return "map.usp_CompanyGroupToSiteDelete";
					case ENTITY_TYPE.COMPANY_TYPE:
						return "map.usp_CompanyTypeToSiteDelete";
					case ENTITY_TYPE.DATA_DICTIONARY:
						return "map.usp_DataDictionaryToSiteDelete";
					case ENTITY_TYPE.DISPATCH_CONFIGURATION:
						return "map.usp_DispatchConfigurationToSiteDelete";
					case ENTITY_TYPE.DOT_HAZARDOUS_MESSAGE:
						return "map.usp_DotHazardousMessageToSiteDelete";
					case ENTITY_TYPE.EMAIL_ADDRESS:
						return "map.usp_EmailAddressToSiteDelete";
					case ENTITY_TYPE.EMAIL_GROUP:
						return "map.usp_EmailGroupToSiteDelete";
					case ENTITY_TYPE.ENTRY_MESSAGE:
						return "map.usp_EntryMessageToSiteDelete";
					case ENTITY_TYPE.EQUIPMENT:
						return "map.usp_EquipmentToSiteDelete";	//RecordVersioning-aware query				
					case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE:
						return "map.usp_EquipmentTagAndLicenseToSiteDelete";
					case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION:
						return "map.usp_EquipmentTestAndInspectionToSiteDelete";
					case ENTITY_TYPE.EQUIPMENT_TYPE:
						return "map.usp_EquipmentTypeToSiteDelete";
					case ENTITY_TYPE.EXIT_MESSAGE:
						return "map.usp_ExitMessageToSiteDelete";
					case ENTITY_TYPE.EXTERNAL_STATION:
						return "map.usp_ExternalStationToSiteDelete";
					case ENTITY_TYPE.EXTERNAL_STATION_DEVICE:
						return "map.usp_ExternalStationDeviceToSiteDelete";
					case ENTITY_TYPE.FOOTNOTE:
						return "map.usp_FootNoteToSiteDelete";
					case ENTITY_TYPE.FUEL_CARD:
						return "map.usp_FuelCardToSiteDelete";
					case ENTITY_TYPE.FUEL_CARD_LIMIT:
						return "map.usp_FuelCardLimitToSiteDelete";
					case ENTITY_TYPE.FUEL_CARD_TYPE:
						return "map.usp_FuelCardTypeToSiteDelete";
					case ENTITY_TYPE.IATA_CODE:
						return "map.usp_IATACodeToSiteDelete";
					case ENTITY_TYPE.ASSET_TRACKING_DEVICE:
						return "map.usp_AssetTrackingDeviceToSiteDelete";
					case ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION:
						return "map.usp_AssetTrackingMapConfigurationToSiteDelete";
					case ENTITY_TYPE.LEDGER_AGGREGATE_COLUMN:
						return "map.usp_LedgerAggregateColumnToSiteDelete";
					case ENTITY_TYPE.LEDGER_VIEW:
						return "map.usp_LedgerViewToSiteDelete";
					case ENTITY_TYPE.LIST_VIEW:
						return "map.usp_ListViewToSiteDelete";
					case ENTITY_TYPE.MAINTENANCE_REASON:
						return "map.usp_MaintenanceReasonToSiteDelete";
					case ENTITY_TYPE.MOBILE_DEVICE_PROFILE:
						return "map.usp_MobileDeviceProfileToSiteDelete";
					case ENTITY_TYPE.PERSONNEL:
						return "map.usp_PersonnelToSiteDelete";  //RecordVersioning-aware query
					case ENTITY_TYPE.QUALIFICATION_PERSON_LICENSE:
						return "map.usp_PersonnelLicenseToSiteDelete";
					case ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION:
						return "map.usp_PersonnelQualificationToSiteDelete";
					case ENTITY_TYPE.QUALIFICATION_PERSON_TRAINING:
						return "map.usp_PersonnelTrainingToSiteDelete";
					case ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE:
						return "map.usp_ProcessVariableMessageToSiteDelete";
					case ENTITY_TYPE.PRODUCT:
						return "map.usp_ProductToSiteDelete";  //RecordVersioning-aware query
					case ENTITY_TYPE.PRODUCT_GROUP:
						return "map.usp_ProductGroupToSiteDelete";
					case ENTITY_TYPE.PRODUCT_MESSAGE:
						return "map.usp_ProductMessageToSiteDelete";
					case ENTITY_TYPE.QUALITY_TAG:
						return "map.usp_QualityTagToSiteDelete";
					case ENTITY_TYPE.QUERY:
					case ENTITY_TYPE.QUERY_DEFAULT:
					case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
						return "map.usp_QuerySettingToSiteDelete";
					case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
						return "map.usp_ReportConfigurationSettingsToSiteDelete";
					case ENTITY_TYPE.STANDING_OFFER:
						return "map.usp_StandingOfferToSiteDelete";
					case ENTITY_TYPE.TEST:
						return "map.usp_TestToSiteDelete";
					case ENTITY_TYPE.TEST_SET:
						return "map.usp_TestSetToSiteDelete";
					case ENTITY_TYPE.TRANSACTION_ALIAS:
					case ENTITY_TYPE.TRANSACTION_ALIAS_NAME:
						return "map.usp_TransactionAliasToSiteDelete";  //RecordVersioning-aware query
					case ENTITY_TYPE.USER:
						return "map.usp_UserToSiteDelete";
					case ENTITY_TYPE.USER_DATA_FIELD:
						return "map.usp_UserDataToSiteDelete";
					case ENTITY_TYPE.GROUP:
						return "map.usp_UserGroupToSiteDelete";
					case ENTITY_TYPE.POINT_TEMPLATE:
						return "map.usp_PointTemplateToSiteDelete";
					case ENTITY_TYPE.POINT_TEMPLATE_TYPE:
						return "map.usp_PointTemplateTypeToSiteDelete";
					case ENTITY_TYPE.POINT_CATEGORY:
						return "map.usp_PointCategoryToSiteDelete";
					case ENTITY_TYPE.MODULE:
					return "map.usp_ModuleToSiteDelete";
					default:
						System.Diagnostics.Debug.Assert(false, "Entity to site mapping query name not found.");
						return "Unknown";
				}
		}
		#endregion
	}
}