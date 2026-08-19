// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentTypesClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Security;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using DataAccessLayer;
    using InternalClasses;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using IsolationLevel = System.Transactions.IsolationLevel;

    /// <summary>
	/// Service class implementation for equipment types.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class EquipmentTypesClass : IDependency, IEquipmentTypes
	{
		#region Constants and Fields

		/// <summary>
		/// The database access object.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators


		/// <summary>
		/// Adds the specified equipment type to the database.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="equipmentType">Type of the equipment.</param>
		/// <returns>The identity GUID of the newly added equipment type.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// security
		/// </exception>
		/// <exception cref="System.Exception">
		/// Access Denied
		/// </exception>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public Guid Add(SecurityClass security, EquipmentTypeClass equipmentType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (equipmentType == null)
			{
				throw new ArgumentNullException("equipmentType");
			}

			if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, equipmentType);

			equipmentType.SiteGuid = security.SiteGuid;
			equipmentType.CreatedDate = DateTimeOffset.Now;
			equipmentType.CreatedBy = security.UserID;
			equipmentType.UpdatedDate = equipmentType.CreatedDate;
			equipmentType.UpdatedBy = security.UserID;
			equipmentType.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				equipmentType.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(equipmentType);
			entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(
				security, equipmentType.IdentityGuid, equipmentType.ReqQualificationsCollection, null);
			qualificationMaps.ModifyCollection(security, equipmentType.IdentityGuid, equipmentType.ReqTrainingCollection, null);

			this.AddTanks(security, equipmentType);

			return equipmentType.IdentityGuid;
		}

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="filter">
		/// The filter.
		/// </param>
		/// <param name="order">
		/// The order.
		/// </param>
		/// <returns>
		/// The <see cref="EquipmentTypeCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public EquipmentTypeCollectionClass Enumerate(SecurityClass security, string filter, string order)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			    && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var equipmentType = new EquipmentTypeClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				equipmentType.EnumerateSQL(cmd, security, filter, order);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var equipmentTypeCollection = new EquipmentTypeCollectionClass();

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				equipmentType = new EquipmentTypeClass(site);
				equipmentType.Load(set);
				equipmentTypeCollection.Add(equipmentType);
				table.Rows.RemoveAt(0);
			}

			foreach (EquipmentTypeClass equipType in equipmentTypeCollection)
			{
				equipType.TankCollection = this.EnumerateByEquipmentType(
					security, 
					equipType.IdentityGuid, 
					equipType.GetAirplaneTankCapacityUnit(), 
					equipType.GetAirplaneTankCapacityDecimalPlaces());
			}

			return equipmentTypeCollection;
		}

		/// <summary>
		/// The enumerate by equipment type.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="equipmentTypeGuid">
		/// The equipment type GUID.
		/// </param>
		/// <param name="capacityUnits">
		/// The capacity units.
		/// </param>
		/// <param name="capacityDecimalPlaces">
		/// The capacity decimal places.
		/// </param>
		/// <returns>
		/// The <see cref="AirplaneTankCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public AirplaneTankCollectionClass EnumerateByEquipmentType(
			SecurityClass security, Guid equipmentTypeGuid, EngineeringUnit capacityUnits, int capacityDecimalPlaces)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES)
				&& !security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES))
			{
				throw new FMInsufficientRightsException();
			}

			var airplaneTank = new AirplaneTankClass(capacityUnits, capacityDecimalPlaces)
				{
					ParentGuid = equipmentTypeGuid
				};

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				airplaneTank.SelectByParentGuid(cmd, security, ContextUtil.IsInTransaction);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var airplaneTankCollection = new AirplaneTankCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				airplaneTank = new AirplaneTankClass(capacityUnits, capacityDecimalPlaces);
				airplaneTank.Load(set);
				airplaneTankCollection.Add(airplaneTank);
				table.Rows.RemoveAt(0);
			}

			return airplaneTankCollection;
		}

		/// <summary>
		/// The enumerate by equipment type.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="equipmentTypeGuid">
		/// The equipment type GUID.
		/// </param>
		/// <returns>
		/// The <see cref="AirplaneTankCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public AirplaneTankCollectionClass EnumerateByEquipmentType(SecurityClass security, Guid equipmentTypeGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			var airplaneTank = new AirplaneTankClass
				{
					ParentGuid = equipmentTypeGuid
				};

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				airplaneTank.SelectByParentGuid(cmd, security, ContextUtil.IsInTransaction);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var airplaneTankCollection = new AirplaneTankCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				airplaneTank = new AirplaneTankClass();
				airplaneTank.Load(set);
				airplaneTankCollection.Add(airplaneTank);
				table.Rows.RemoveAt(0);
			}

			return airplaneTankCollection;
		}

		/// <summary>
		/// The enumerate data set.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="filter">
		/// The filter.
		/// </param>
		/// <param name="order">
		/// The order.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public DataSet EnumerateDataSet(SecurityClass security, string filter, string order)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			    && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var equipmentType = new EquipmentTypeClass();
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				equipmentType.EnumerateSQL(cmd, security, filter, order);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			return set;
		}

		/// <summary>
		/// Gets the specified equipment type.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="equipmentTypeGuid">The equipment type GUID.</param>
		/// <returns>The requested equipment type or null.</returns>
		public EquipmentTypeClass Get(SecurityClass security, Guid equipmentTypeGuid)
		{
			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

			return this.Get(security, equipmentTypeGuid, site);
		}

		/// <summary>
		/// Gets the specified equipment type.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="equipmentTypeGuid">
		/// The equipment type GUID.
		/// </param>
		/// <param name="site">
		/// The Site.
		/// </param>
		/// <returns>
		/// The requested equipment type or null.
		/// </returns>
		public EquipmentTypeClass Get( SecurityClass security, Guid equipmentTypeGuid, SiteClass site )
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (site == null)
			{
				throw new ArgumentNullException("site");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES)
				&& !security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES))
			{
				throw new FMInsufficientRightsException();
			}

			var equipmentType = new EquipmentTypeClass(site)
				{
					IdentityGuid = equipmentTypeGuid
				};

			using (var cmd = new SqlCommand())
			{
				equipmentType.SelectSQL(cmd, ContextUtil.IsInTransaction);
				equipmentType.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			var qualificationMaps = new QualificationMapsClass();
			equipmentType.ReqQualificationsCollection = qualificationMaps.EnumerateByGuidAndType(
				security, equipmentTypeGuid, QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE, false);

			equipmentType.ReqTrainingCollection = qualificationMaps.EnumerateByGuidAndType(
				security, equipmentTypeGuid, QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_EQUIPMENT_TYPE, false);

			// Loading Airplane Tanks
			equipmentType.TankCollection = this.EnumerateByEquipmentType(
				security, 
				equipmentType.IdentityGuid, 
				equipmentType.GetAirplaneTankCapacityUnit(), 
				equipmentType.GetAirplaneTankCapacityDecimalPlaces());

			return equipmentType;
		}

		/// <summary>
		/// Gets the identity GUID from the equipment of the specified id.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="id">The ID.</param>
		/// <returns>The GUID.</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="System.Exception">Access Denied</exception>
		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_DISPATCH) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var equipmentType = new EquipmentTypeClass
				{
					ID = id, 
					SiteGuid = security.SiteGuid
				};

			using (var cmd = new SqlCommand())
			{
				equipmentType.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				equipmentType.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return equipmentType.IdentityGuid;
		}

		/// <summary>
		/// The import.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="equipmentType">
		/// The equipment type.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Equipment Type import exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass security, EquipmentTypeClass equipmentType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (equipmentType == null)
			{
				throw new ArgumentNullException("equipmentType");
			}

			var qualifications = new QualificationsClass();

			try
			{
				equipmentType.IdentityGuid = this.GetIdentityGuid(security, equipmentType.ID);

				// If the entity exists and is not owned by this site, do not update it.
				if (equipmentType.IdentityGuid != Guid.Empty
				    && this.Get(security, equipmentType.IdentityGuid).SiteGuid != security.SiteGuid)
				{
					return;
				}

				foreach (QualificationMapClass reqTraining in equipmentType.ReqTrainingCollection)
				{
					Guid qualificationGuid = qualifications.GetIdentityGuid(
						security, QUALIFICATION_TYPE.PERSON_QUALIFICATION, reqTraining.ID);
					if (qualificationGuid == Guid.Empty)
					{
						var training = new QualificationClass
							{
								ID = reqTraining.ID, 
								Type = QUALIFICATION_TYPE.PERSON_QUALIFICATION
							};

						qualificationGuid = qualifications.Add(security, training);
					}

					reqTraining.AssignedGuid = qualificationGuid;
				}

				foreach (QualificationMapClass reqQualifications in equipmentType.ReqQualificationsCollection)
				{
					Guid qualificationGuid = qualifications.GetIdentityGuid(
						security, QUALIFICATION_TYPE.PERSON_QUALIFICATION, reqQualifications.ID);
					if (qualificationGuid == Guid.Empty)
					{
						var qualification = new QualificationClass
							{
								ID = reqQualifications.ID,
								Type = QUALIFICATION_TYPE.PERSON_QUALIFICATION
							};

						qualificationGuid = qualifications.Add(security, qualification);
					}

					reqQualifications.AssignedGuid = qualificationGuid;
				}

				equipmentType.Product = Guid.Empty;

				if (string.IsNullOrEmpty(equipmentType.ProductId) == false)
				{
					var products = new ProductsClass();
					ProductClass product = products.GetByID(security, equipmentType.ProductId);

					if (product != null)
					{
						equipmentType.Product = product.IdentityGuid;
					}
				}

				if (equipmentType.IdentityGuid == Guid.Empty)
				{
					this.Add(security, equipmentType);
				}
				else
				{
					this.Modify(security, equipmentType);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("[EquipmentType Import Error ID] : " + equipmentType.ID + ", " + ex.Message);
			}
		}

		/// <summary>
		/// The modify.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="equipmentType">
		/// The equipment type.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Modify equipment type exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, EquipmentTypeClass equipmentType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if ( equipmentType == null )
			{
				throw new ArgumentNullException("equipmentType");
			}

			if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, equipmentType);

			EquipmentTypeClass oldEquipmentType = this.Get(security, equipmentType.IdentityGuid);

         if (oldEquipmentType.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Equipment Type Not Found");
			}

			equipmentType.UpdatedDate = DateTimeOffset.Now;
			equipmentType.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				equipmentType.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			this.PurgeTanks(security, oldEquipmentType);
			this.AddTanks(security, equipmentType);
			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, equipmentType.EntityType, equipmentType.IdentityGuid);

			if ( equipmentType.SiteGuid != oldEquipmentType.SiteGuid )
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(equipmentType);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}

			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(
												security,
												equipmentType.IdentityGuid,
												equipmentType.ReqQualificationsCollection, 
												oldEquipmentType.ReqQualificationsCollection);
			qualificationMaps.ModifyCollection(
									security, equipmentType.IdentityGuid, equipmentType.ReqTrainingCollection, oldEquipmentType.ReqTrainingCollection);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyOnlyQualificationsAndTrainings(SecurityClass security, EquipmentTypeClass equipmentType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (equipmentType == null)
			{
				throw new ArgumentNullException("equipmentType");
			}

			if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			EquipmentTypeClass oldEquipmentType = this.Get(security, equipmentType.IdentityGuid);

 
         if (oldEquipmentType.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Equipment Type Not Found");
			}


			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(
												security,
												equipmentType.IdentityGuid,
												equipmentType.ReqQualificationsCollection,
												oldEquipmentType.ReqQualificationsCollection);
			qualificationMaps.ModifyCollection(
									security, equipmentType.IdentityGuid, equipmentType.ReqTrainingCollection, oldEquipmentType.ReqTrainingCollection);
		}


		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="equipmentTypeGuid">
		/// The equipment type GUID.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Delete exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid equipmentTypeGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var equipments = new EquipmentsClass();
			EquipmentCollectionClass coll = equipments.EnumerateExt(security, equipmentTypeGuid);

			if (coll.Count > 0)
			{
				throw new Exception("Equipment Type cannot be deleted because it is associated to a piece of Equipment");
			}

			EquipmentTypeClass equipmentType = this.Get(security, equipmentTypeGuid);
			if (equipmentType.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Equipment Type Not Found");
			}

			this.PurgeTanks(security, equipmentType);

			// Purge any qualification maps
			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(
				security, equipmentType.IdentityGuid, null, equipmentType.ReqQualificationsCollection);
			qualificationMaps.ModifyCollection(security, equipmentType.IdentityGuid, null, equipmentType.ReqTrainingCollection);

			// Purge from EntityToSiteMap
			EntityToSiteMaps.RemoveAllMapsForEntity(security, ENTITY_TYPE.EQUIPMENT_TYPE, equipmentType.IdentityGuid);

			using (var cmd = new SqlCommand())
			{
				equipmentType.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#endregion

		#region Explicit Interface Methods

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

			if (preOperation && typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
			{
				var entityToSiteMap = (EntityToSiteMapClass)Object;

				if (entityToSiteMap.TypeID != ENTITY_TYPE.EQUIPMENT_TYPE)
				{
					return;
				}

				if (Guid.Empty != this.GetIdentityGuid(security, entityToSiteMap.ID))
				{
					throw new Exception("Equipment Type Exists - " + entityToSiteMap.ID);
				}
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

			// Purge Equipment Type Deleted/Undeleted
			var o = Object as SiteClass;
			if (o != null)
			{
				var site = o;
				EquipmentTypeCollectionClass equipmentTypeCollection = this.Enumerate(security, null, null);
				foreach (EquipmentTypeClass equipmentType in equipmentTypeCollection)
				{
					if (site.SiteGuid == equipmentType.SiteGuid)
					{
						this.Purge(security, equipmentType.IdentityGuid);
					}
				}
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

			var o = Object as SiteClass;
			if (o != null)
			{
				var site = o;
				EquipmentTypeCollectionClass equipmentTypeCollection = this.Enumerate(security, null, null);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (EquipmentTypeClass equipmentType in equipmentTypeCollection)
				{
					if (site.SiteGuid == equipmentType.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
							security, equipmentType.EntityType, equipmentType.IdentityGuid);
						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = equipmentType.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds the tanks to the database from the equipment type.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="equipmentType">Type of the equipment.</param>
		protected void AddTanks(SecurityClass security, EquipmentTypeClass equipmentType)
		{
			foreach (AirplaneTankClass airplaneTank in equipmentType.TankCollection)
			{
				airplaneTank.ParentGuid = equipmentType.IdentityGuid;
				airplaneTank.CreatedDate = DateTimeOffset.Now;
				airplaneTank.CreatedBy = security.UserID;
				airplaneTank.UpdatedDate = equipmentType.CreatedDate;
				airplaneTank.UpdatedBy = security.UserID;
				airplaneTank.IdentityGuid = Guid.NewGuid();

				using (var cmd = new SqlCommand())
				{
					airplaneTank.InsertSQL(cmd);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
		}

		/// <summary>
		/// Purges the tanks.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="equipmentType">Equipment type object.</param>
		protected void PurgeTanks(SecurityClass security, EquipmentTypeClass equipmentType)
		{
			foreach (AirplaneTankClass airplaneTank in equipmentType.TankCollection)
			{
				airplaneTank.ParentGuid = equipmentType.IdentityGuid;
				using (var cmd = new SqlCommand())
				{
					airplaneTank.PurgeSQL(cmd);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
		}

		/// <summary>
		/// Validates the specified equipment type.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="equipmentType">Type of the equipment.</param>
		/// <exception cref="System.Exception">ID Required</exception>
		private void Validate(SecurityClass security, EquipmentTypeClass equipmentType)
		{
			if (string.IsNullOrEmpty(equipmentType.ID))
			{
				throw new Exception("ID Required");
			}

			if (equipmentType.ID == "{None}" || equipmentType.ID == "{Unassigned}" || equipmentType.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + equipmentType.ID);
			}

			if (equipmentType.Description.Length > 50)
			{
				throw new Exception("Exceeded max length (50)");
			}

			Guid identityGuid = this.GetIdentityGuid(security, equipmentType.ID);
			if (identityGuid != Guid.Empty && identityGuid != equipmentType.IdentityGuid)
			{
				throw new Exception("Equipment Type Exists");
			}
		}

		#endregion
	}
}