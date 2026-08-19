// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Equipments.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implementation of the IEquipments services class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Security;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using DataAccessLayer;
    using InternalClasses;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using IsolationLevel = System.Transactions.IsolationLevel;

    /// <summary>
    /// Implementation of the IEquipments services class
    /// </summary>
    [SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class EquipmentsClass : FMServiceBase, IDependency, IEquipments
    {
        #region Constants and Fields
        /// <summary>
        /// The consolidated data access object for database access.
        /// </summary>
        private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		private const string CantDeleteMessageDueToTrx = "This equipment cannot be deleted because it is associated with {0} transaction(s).";
        private const string CantDeleteMessageDueToTestResuls = "This equipment cannot be deleted because it is associated with {0} test result(s).";
        private const string CantChangeIDMessageDueToTrx = "The ID cannot be changed because this equipment is associated with {0} transaction(s).";
        private const string CantDeleteMessageDueToMaintenance = "This equipment cannot be deleted because this equipment is associated with {0} maintenance record(s)."; 
        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// Operation to add a new Equipment master record version.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="equipment">The equipment.</param>
        /// <returns>The <see cref="Guid"/> of the newly added equipment.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        /// <exception cref="System.ApplicationException">Return To Service Date required if not in service</exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, EquipmentClass equipment)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (equipment == null)
            {
                throw new ArgumentNullException(nameof(equipment));
            }

            if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.INTERFACE_IMPORT)
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            this.Validate(security, equipment);

            this.UpdateCapacity(equipment);

			// Set UserData(list type) to defaults if they are blanks
			UserDataFieldsClass.SetDefaults(security, equipment.UserData, ENTITY_TYPE.EQUIPMENT);

            equipment.SiteGuid = security.SiteGuid;
            equipment.CreatedDate = DateTimeOffset.Now;
            equipment.CreatedBy = security.UserID;
            equipment.UpdatedDate = equipment.CreatedDate;
            equipment.UpdatedBy = security.UserID;

            using (var cmd = new SqlCommand())
            {
                equipment.InsertSQL(cmd);
                this.consolidatedDA.ExecuteQuery(security, cmd);
            }

            // Does the equipment have a meter assigned to it? Add it here so that the EquipmentGuid is available for mapping purposes
            if (equipment.Meter.Count > 0)
            {
                // If so, add the meter to the database and remember the meterGuid so we can use it when we insert the equipment.
                foreach (MeterClass meter in equipment.Meter)
                {
                    var meters = new MetersClass();
                    meters.Add(security, meter);
                    meters.AddEquipmentMap(security, meter, equipment.MasterRecordGuid);
                }
            }

            // Create Entity to Site Map
            var entityToSiteMaps = new EntityToSiteMaps();
            var entityToSiteMap = new EntityToSiteMapClass(equipment);
            entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

            this.UpdateCompartments(security, equipment, null);

            // If we are adding equipment that is out-of-service, we need to required
            // some information and actually save it in a maintenance record.
            if (equipment.InServiceFlag == false)
            {
                if (string.IsNullOrEmpty(equipment.ReturnToServiceDate))
                {
                    throw new ApplicationException("Return To Service Date required if not in service");
                }

                if (string.IsNullOrEmpty(equipment.StatusDescription))
                {
                    throw new ApplicationException("Status Description required if not in service");
                }

                var logs = new EquipmentMaintenanceLogsClass();

                var log = new EquipmentMaintenanceLogClass
                {
                    EquipmentGuid = equipment.IdentityGuid,
                    EquipmentID = equipment.ID,
                    EstReturnToServiceDate = equipment.ReturnToServiceDateObject.Value,
                    InServiceFlag = 0,
                    MaintenanceReasonGuid = equipment.StatusDescriptionGuid,
                    MaintenanceReason = equipment.StatusDescription,
                    EquipmentType = equipment.TypeClass
                };

                logs.Add(security, log);
            }

            var processVariables = new ProcessVariablesClass();
            equipment.VolumeProcessVariable.UnitGuid = equipment.MasterRecordGuid;
            processVariables.Add(security, equipment.VolumeProcessVariable);

            var qualificationMaps = new QualificationMapsClass();
            qualificationMaps.ModifyCollection(security, equipment.IdentityGuid, equipment.TestAndInspectionCollection, null);
            qualificationMaps.ModifyCollection(security, equipment.IdentityGuid, equipment.TagAndLicenseCollection, null);

            return equipment.IdentityGuid;
        }

        /// <summary>
        /// Enumerates equipment based on the current site specified in the security object.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not marked as hidden will be returned</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        public EquipmentCollectionClass Enumerate(SecurityClass security, bool hideHiddenEquipmentRecords = false)
        {
            return this.EnumerateExt(security, Guids.AllFilterGuid, hideHiddenEquipmentRecords: hideHiddenEquipmentRecords);
        }

        /// <summary>
        /// Enumerates equipment by company.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="companyGuid">The company GUID.</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not marked as hidden will be returned</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        public EquipmentCollectionClass EnumerateByCompany(SecurityClass security, Guid companyGuid, bool hideHiddenEquipmentRecords = false)
        {
            return this.EnumerateByCompanyAndLocalize(security, companyGuid, true, hideHiddenEquipmentRecords: hideHiddenEquipmentRecords);
        }

        /// <summary>
        /// Enumerates equipment by company and localize.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="companyGuid">The company GUID.</param>
        /// <param name="localize">if set to <c>true</c> [b localize].</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not marked as hidden will be returned</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        public EquipmentCollectionClass EnumerateByCompanyAndLocalize(SecurityClass security, Guid companyGuid, bool localize, bool hideHiddenEquipmentRecords = false)
        {
            var sites = new SitesClass();
            SiteClass site = null;

            if (localize)
            {
                site = sites.GetByMemberAndProcessVariables(
                    security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);
            }

            return this.EnumerateByCompanyAndSite(security, companyGuid, site, hideHiddenEquipmentRecords: hideHiddenEquipmentRecords);
        }

        /// <summary>
        /// Enumerates equipment by company and site.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="companyGuid">The company GUID.</param>
        /// <param name="site">The site.</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not marked as hidden will be returned</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public EquipmentCollectionClass EnumerateByCompanyAndSite(SecurityClass security, Guid companyGuid, SiteClass site, bool hideHiddenEquipmentRecords = false)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
                && !security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
                && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
                && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
                && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
                && !security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_DISPATCH) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)   && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_INCOMING_TRUCK_DATA)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByCompanyGuid";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@CompanyGuid"].Value = DBNull.Value;

                if (companyGuid != Guid.Empty)
                {
                    cmd.Parameters["@CompanyGuid"].Value = companyGuid;
                }

                if (hideHiddenEquipmentRecords)
                {
                    cmd.Parameters.Add("@HideHiddenEquipmentRecords", SqlDbType.Bit).Value = 1;
                }

                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var equipmentCollection = new EquipmentCollectionClass();

            DataTable table = set.Tables[0];
            while (table.Rows.Count != 0)
            {
	            EquipmentClass equipment = site != null ? new EquipmentClass(site) : new EquipmentClass();

                equipment.Load(set);
                //this.LoadAttachedMeters(equipment, security);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// Enumerates equipment by company get ID type only.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="companyGuid">The company GUID.</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public EquipmentCollectionClass EnumerateByCompanyGetIDTypeOnly(SecurityClass security, Guid companyGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
                && !security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
                && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
                && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
                && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
                && !security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)   && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateByCompanyGetIDTypeOnlySQL(cmd, security, companyGuid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByCompany";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyEquipmentId", SqlDbType.NVarChar, 30);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@CompanyGuid"].Value = companyGuid;
                cmd.Parameters["@CompanyEquipmentId"].Value = DBNull.Value;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var equipmentCollection = new EquipmentCollectionClass();

            DataTable table = set.Tables[0];

            while (table.Rows.Count != 0)
            {
                var equipment = new EquipmentClass();
                equipment.LoadIDType(set);
                //this.LoadAttachedMeters(equipment, security);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// Enumerates equipment by equipment guid
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="equipmentGuid">The equipment GUID.</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public EquipmentCollectionClass EnumerateByEquipment(SecurityClass security, Guid equipmentGuid)
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
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
                && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
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

            var equipment = new EquipmentClass
            {
                IdentityGuid = equipmentGuid
            };

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                equipment.EnumerateByEquipmentSQL(cmd);
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var equipmentCollection = new EquipmentCollectionClass();

            var sites = new SitesClass();
            SiteClass site = sites.GetByMemberAndProcessVariables(
                security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

            DataTable table = set.Tables[0];
            while (table.Rows.Count != 0)
            {
                equipment = new EquipmentClass(site);
                equipment.Load(set);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// This method will retrieve the associated equipment compartments for a given
        /// equipment.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="parentEquipmentGuid">The parent equipment Guid to search on.</param>
        /// <returns>Returns a collection of equipment of compartment types or empty collection.</returns>
        public Dictionary<string, Guid> GetEquipmentCompartmentGuids(SecurityClass security, Guid parentEquipmentGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            var equipmentCompartmentList = new Dictionary<string, Guid>();

            using (SqlCommand command = new SqlCommand())
            {
                var equipment = new EquipmentClass();
                equipment.GetEquipmentCompartmentGuidSql(command, parentEquipmentGuid);
                DataSet dataSet = this.consolidatedDA.GetDataSet(command, security);

                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach(DataRow row in dataSet.Tables[0].Rows)
                    {
                        Guid equipmentCompartmentGuid = row.IsNull("EquipmentGuid") ? Guid.Empty : (Guid)row["EquipmentGuid"];
                        string equipmentCompartmentId = row.IsNull("ID") ? string.Empty : (string)row["ID"];

                        if (equipmentCompartmentGuid != Guid.Empty)
                        {
                            equipmentCompartmentList.Add(equipmentCompartmentId, equipmentCompartmentGuid);
                        }
                    }
                }
            }

            return equipmentCompartmentList;
        }

        /// <summary>
        /// Enumerates equipment by fuel card.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="fuelCardGuid">The fuel card GUID.</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not marked as hidden will be returned</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public EquipmentCollectionClass EnumerateByFuelCard(SecurityClass security, Guid fuelCardGuid, bool hideHiddenEquipmentRecords = false)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.MODIFY_PRODUCTS) 
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) )
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateByFuelCardSQL(cmd, security, fuelCardGuid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByFuelCard";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@FuelCardGuid"].Value = fuelCardGuid;

                if (hideHiddenEquipmentRecords)
                {
                    cmd.Parameters.Add("@HideHiddenEquipmentRecords", SqlDbType.Bit).Value = 1;
                }

                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var equipmentCollection = new EquipmentCollectionClass();

            var sites = new SitesClass();
            SiteClass site = sites.GetByMemberAndProcessVariables(
                security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

            DataTable table = set.Tables[0];
            while (table.Rows.Count != 0)
            {
                var equipment = new EquipmentClass(site);
                equipment.Load(set);
                //this.LoadAttachedMeters(equipment, security);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// Enumerates equipment by managed fillstand.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        public EquipmentCollectionClass EnumerateByManagedFillstand(SecurityClass security)
        {
            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateByManagedFuelstand(cmd, security);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByManagedFuelstand";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@FillStandType", SqlDbType.Int);
                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@FillStandType"].Value = (int)EQUIPMENT_TYPE.FILLSTAND_TYPE;
                return this.EnumerateInternalStandard(security, cmd);
            }
        }

        /// <summary>
        /// Enumerates equipment by product.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="productGuid">The product GUID.</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public EquipmentCollectionClass EnumerateByProduct(SecurityClass security, Guid productGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateByProductSQL(cmd, security, productGuid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByProduct";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@ProductGuid"].Value = productGuid;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var equipmentCollection = new EquipmentCollectionClass();

            var sites = new SitesClass();
            SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

            DataTable table = set.Tables[0];
            while (table.Rows.Count != 0)
            {
                var equipment = new EquipmentClass(site);
                equipment.Load(set);
                //this.LoadAttachedMeters(equipment, security);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// Enumerates equipment by source.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        public EquipmentCollectionClass EnumerateBySource(SecurityClass security)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsBySource";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@HydrantCartType", SqlDbType.Int);
                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@HydrantCartType"].Value = (int)EQUIPMENT_TYPE.HYDRANT_CART_TYPE;
                return this.EnumerateInternalStandard(security, cmd);
            }
        }

        /// <summary>
        /// Enumerates equipment by type and filter and product.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="type">The type.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="productGuid">The product GUID.</param>
        /// <param name="excludeNonEditableCompanyGuid">if set to <c>true</c> [exclude non editable company GUID].</param>
        /// <param name="excludeNonEditableFuelCardGuid">if set to <c>true</c> [exclude non editable fuel card GUID].</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records not marked as hidden will be returned</param>
        /// <returns>
        /// A collection of enumerated equipment objects.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public EquipmentCollectionClass EnumerateByTypeAndFilterAndProduct(SecurityClass security, EQUIPMENT_TYPE type, string filter, Guid productGuid, bool excludeNonEditableCompanyGuid, bool excludeNonEditableFuelCardGuid, bool hideHiddenEquipmentRecords = false)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS))
            {
                throw new FMInsufficientRightsException();
            }

            var equipmentCollection = new EquipmentCollectionClass();

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateByTypeAndFilterAndProductSQL(cmd, security, Type, filter, productGuid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsBySearchFilter";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentType", SqlDbType.Int);
                cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 32);
                cmd.Parameters.Add("@ExcludeNonEditableCompanyGuid", SqlDbType.Bit);
                cmd.Parameters.Add("@ExcludeNonEditableFuelCardGuid", SqlDbType.Bit);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@EquipmentType"].Value = DBNull.Value;
                if (type != EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
                {
                    cmd.Parameters["@EquipmentType"].Value = (int)type;
                }

                cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
                if (productGuid != Guid.Empty)
                {
                    cmd.Parameters["@ProductGuid"].Value = productGuid;
                }

                cmd.Parameters["@SearchFilter"].Value = DBNull.Value;
                if (!string.IsNullOrEmpty(filter))
                {
                    if (filter.Length > 30)
                    {
                       filter = filter.Substring(0, 30);
                    }

                    filter = "%" + filter + "%";
                    filter = filter.ToUpper();
                    cmd.Parameters["@SearchFilter"].Value = filter;
                }

                cmd.Parameters["@ExcludeNonEditableCompanyGuid"].Value = excludeNonEditableCompanyGuid;
                cmd.Parameters["@ExcludeNonEditableFuelCardGuid"].Value = excludeNonEditableFuelCardGuid;

                if (hideHiddenEquipmentRecords)
                {
                    cmd.Parameters.Add("@HideHiddenEquipmentRecords", SqlDbType.Bit).Value = 1;
                }

                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var sites = new SitesClass();
            SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

            DataTable table = set.Tables[0];

            while (table.Rows.Count != 0)
            {
                var equipment = new EquipmentClass(site);
                equipment.Load(set);
                //this.LoadAttachedMeters(equipment, security);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// Enumerates equipment by equipment type and product.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="type">The type.</param>
        /// <param name="productGuid">The product GUID.</param>
        /// <param name="excludeNonEditableCompanyGuid">if set to <c>true</c> [exclude non editable company GUID].</param>
        /// <param name="excludeNonEditableFuelCardGuid">if set to <c>true</c> [exclude non editable fuel card GUID].</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not marked as hidden will be returned</param>
        /// <returns>
        /// A collection of enumerated equipment objects.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public EquipmentCollectionClass EnumerateByTypeAndProduct(
            SecurityClass security, EQUIPMENT_TYPE type, Guid productGuid, bool excludeNonEditableCompanyGuid, bool excludeNonEditableFuelCardGuid, bool hideHiddenEquipmentRecords = false)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            var equipmentCollection = new EquipmentCollectionClass();

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateByTypeAndProductSQL(cmd, security, Type, productGuid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByTypeAndProduct";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentType", SqlDbType.Int);
                cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ExcludeNonEditableCompanyGuid", SqlDbType.Bit);
                cmd.Parameters.Add("@ExcludeNonEditableFuelCardGuid", SqlDbType.Bit);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@EquipmentType"].Value = DBNull.Value;
                if (type != EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
                {
                    cmd.Parameters["@EquipmentType"].Value = (int)type;
                }

                cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
                if (productGuid != Guid.Empty)
                {
                    cmd.Parameters["@ProductGuid"].Value = productGuid;
                }

                cmd.Parameters["@ExcludeNonEditableCompanyGuid"].Value = excludeNonEditableCompanyGuid;
                cmd.Parameters["@ExcludeNonEditableFuelCardGuid"].Value = excludeNonEditableFuelCardGuid;

                if (hideHiddenEquipmentRecords)
                {
                    cmd.Parameters.Add("@HideHiddenEquipmentRecords", SqlDbType.Bit).Value = 1;
                }

                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var sites = new SitesClass();
            SiteClass site = sites.GetByMemberAndProcessVariables(
                security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

            DataTable table = set.Tables[0];
            while (table.Rows.Count != 0)
            {
                var equipment = new EquipmentClass(site);
                equipment.Load(set);
                //this.LoadAttachedMeters(equipment, security);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// Enumerates equipment by types company fuel card product and secondary storage1.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="types">The types.</param>
        /// <param name="companyGuid">The company GUID.</param>
        /// <param name="fuelCardGuid">The fuel card GUID.</param>
        /// <param name="productGuid">The product GUID.</param>
        /// <param name="secondaryStorage">The secondary storage.</param>
        /// <returns>A collection of enumerated equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public DataSet EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(
            SecurityClass security,
            EQUIPMENT_TYPE[] types,
            object companyGuid,
            object fuelCardGuid,
            object productGuid,
            object secondaryStorage)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) 
                && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
                && !security.HasRight(RIGHT.MODIFY_DISPATCH) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            var equipmentTypeList = new DataTable();
            equipmentTypeList.Columns.Add("EquipmentType", typeof(int));
            if (types != null && types.Length != 0)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (int i = 0; i < types.Length; i++)
                {
                    equipmentTypeList.Rows.Add((int)types[i]);
                }
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByFuelCardAndProduct";
                SqlParameter sqlParamEqTypeList = cmd.Parameters.AddWithValue("@EquipmentTypeList", equipmentTypeList);
                sqlParamEqTypeList.SqlDbType = SqlDbType.Structured;
                sqlParamEqTypeList.TypeName = "dbo.utt_EquipmentType";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@SecondaryStorageFlag", SqlDbType.Bit);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@CompanyGuid"].Value = DBNull.Value;
                if (companyGuid is Guid)
                {
                    cmd.Parameters["@CompanyGuid"].Value = (Guid)companyGuid;
                }

                cmd.Parameters["@FuelCardGuid"].Value = DBNull.Value;
                if (fuelCardGuid is Guid)
                {
                    cmd.Parameters["@FuelCardGuid"].Value = (Guid)fuelCardGuid;
                }

                cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
                if (productGuid is Guid)
                {
                    cmd.Parameters["@ProductGuid"].Value = (Guid)productGuid;
                }

                cmd.Parameters["@SecondaryStorageFlag"].Value = DBNull.Value;
                if (secondaryStorage is bool)
                {
                    cmd.Parameters["@SecondaryStorageFlag"].Value = (bool)secondaryStorage;
                }

                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            return set;
        }

        /// <summary>
        /// Enumerates equipment and return a data set
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="managedEquipmentOnly">if set to <c>true</c> [managed equipment only].</param>
        /// <param name="secondaryStorageOnly">if set to <c>true</c> [secondary storage only].</param>
        /// <param name="equipmentTypeGuid">The equipment type GUID.</param>
        /// <param name="equipmentType">Type of the equipment.</param>
        /// <param name="translatedUnassigned">The translated unassigned.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="isDefense">if set to <c>true</c> [is defense].</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not hidden will be returned</param>
        /// <returns>A dataset containing the enumerating equipment.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public DataSet EnumerateDataSet(
            SecurityClass security,
            bool managedEquipmentOnly,
            bool secondaryStorageOnly,
            Guid equipmentTypeGuid,
            EQUIPMENT_TYPE equipmentType,
            string translatedUnassigned,
            string filter,
            bool isDefense,
            bool hideHiddenEquipmentRecords = false,
            int limit = 1500)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
                && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;
            using (var cmd = new SqlCommand())
            {
                // Equipment.EquipmentlistEnumerateSQL(cmd, security, managedEquipmentOnly, secondaryStorageOnly, equipmentTypeGuid, equipmentType, translatedUnassigned, filter, isDefense);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipments";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ManagedEquipmentOnly", SqlDbType.Bit);
                cmd.Parameters.Add("@SecondaryStorageOnly", SqlDbType.Bit);
                cmd.Parameters.Add("@EquipmentTypeGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentType", SqlDbType.Int);
                cmd.Parameters.Add("@EquipmentTypeToIgnore", SqlDbType.Int);
                cmd.Parameters.Add("@UnassignedStr", SqlDbType.NVarChar, 30);
                cmd.Parameters.Add("@IdFilter", SqlDbType.NVarChar, 32);
                cmd.Parameters.Add("@DefenceDateFilter", SqlDbType.NVarChar, 30);
                cmd.Parameters.Add("@VolumeFilter", SqlDbType.NVarChar, 30);
                cmd.Parameters.Add("@ProductIdFilter", SqlDbType.NVarChar, 32);
                cmd.Parameters.Add("@ApplyUnassignedCompanyIdFilter", SqlDbType.Bit);
                cmd.Parameters.Add("@CompanyIdFilter", SqlDbType.NVarChar, 32);
                cmd.Parameters.Add("@CompanyEquipmentIdFilter", SqlDbType.NVarChar, 32);
                cmd.Parameters.Add("@Limit", SqlDbType.Int);

                cmd.Parameters["@TargetSiteGuid"].Value					= DBNull.Value;
                cmd.Parameters["@ManagedEquipmentOnly"].Value			= DBNull.Value;
                cmd.Parameters["@SecondaryStorageOnly"].Value			= DBNull.Value;
                cmd.Parameters["@EquipmentTypeGuid"].Value				= DBNull.Value;
                cmd.Parameters["@EquipmentType"].Value					= DBNull.Value;
                cmd.Parameters["@EquipmentTypeToIgnore"].Value			= DBNull.Value;
                cmd.Parameters["@UnassignedStr"].Value = DBNull.Value;
                cmd.Parameters["@IdFilter"].Value						= DBNull.Value;
                cmd.Parameters["@DefenceDateFilter"].Value				= DBNull.Value;
                cmd.Parameters["@VolumeFilter"].Value					= DBNull.Value;
                cmd.Parameters["@ProductIdFilter"].Value				= DBNull.Value;
                cmd.Parameters["@ApplyUnassignedCompanyIdFilter"].Value = DBNull.Value;
                cmd.Parameters["@CompanyIdFilter"].Value				= DBNull.Value;
                cmd.Parameters["@CompanyEquipmentIdFilter"].Value		= DBNull.Value;
                cmd.Parameters["@Limit"].Value                          = DBNull.Value;

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                if (managedEquipmentOnly)
                {
                    cmd.Parameters["@ManagedEquipmentOnly"].Value = 1;
                }

                if (secondaryStorageOnly)
                {
                    cmd.Parameters["@SecondaryStorageOnly"].Value = 1;
                }

                if (equipmentTypeGuid != Guids.AllFilterGuid)
                {
                    cmd.Parameters["@EquipmentTypeGuid"].Value = equipmentTypeGuid;
                }

                if (equipmentType != EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
                {
                    cmd.Parameters["@EquipmentType"].Value = (int)equipmentType;
                }

                cmd.Parameters["@EquipmentTypeToIgnore"].Value = (int)EQUIPMENT_TYPE.COMPARTMENT_TYPE;
				cmd.Parameters["@UnassignedStr"].Value = translatedUnassigned;

                if (hideHiddenEquipmentRecords)
                {
                    cmd.Parameters.Add("@HideHiddenEquipmentRecords", SqlDbType.Bit).Value = 1;
                }

                if (filter != string.Empty)
                {
                     if (filter.Length > 30)
                     {
                        filter = filter.Substring(0, 30);
                     }

                     cmd.Parameters["@IdFilter"].Value = "%" + filter + "%";

                    if (isDefense)
                    {
                        DateTimeOffset dt;
                        bool isDate = DateTimeOffset.TryParse(filter, out dt);

                        if (isDate)
                        {
                            cmd.Parameters["@DefenceDateFilter"].Value = filter;
                        }
                    }

                    float num;
                    bool isFloat = float.TryParse(filter, out num);

                    if (isFloat)
                    {
                        cmd.Parameters["@VolumeFilter"].Value = filter;
                    }

                    cmd.Parameters["@ProductIdFilter"].Value = "%" + filter + "%";

                    if (filter.Contains(translatedUnassigned))
                    {
                        cmd.Parameters["@ApplyUnassignedCompanyIdFilter"].Value = 1;
                    }
                    else
                    {
                        cmd.Parameters["@CompanyIdFilter"].Value = "%" + filter + "%";
                    }

                    if (!isDefense)
                    {
                        cmd.Parameters["@CompanyEquipmentIdFilter"].Value = "%" + filter + "%";
                    }
                }

                cmd.Parameters["@Limit"].Value = limit;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            return set;
        }

        /// <summary>
        /// Enumerates the ext.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="equipmentTypeGuid">The equipment type GUID.</param>
        /// <param name="managedEquipmentOnly">if set to <c>true</c> [managed equipment only].</param>
        /// <param name="secondaryStorageOnly">if set to <c>true</c> [secondary storage only].</param>
        /// <param name="equipmentType">Type of the equipment.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not marked as inactive will be returned</param>
        /// <returns>A collection of equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        [SecurityCritical]
        public EquipmentCollectionClass EnumerateExt(
            SecurityClass security,
            Guid equipmentTypeGuid,
            bool managedEquipmentOnly = false,
            bool secondaryStorageOnly = false,
            EQUIPMENT_TYPE equipmentType = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE,
            string filter = null,
            int limit = 0,
            bool hideHiddenEquipmentRecords = false)
        {
            return this.EnumerateExt2(
                security, security.SiteGuid, equipmentTypeGuid, managedEquipmentOnly, secondaryStorageOnly, equipmentType, filter, limit, hideHiddenEquipmentRecords: hideHiddenEquipmentRecords);
        }

        /// <summary>
        /// Enumerates the ext2.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="targetSiteGuid">The site that you want the Equipments from.</param>
        /// <param name="equipmentTypeGuid">The equipment type GUID.</param>
        /// <param name="managedEquipmentOnly">if set to <c>true</c> [managed equipment only].</param>
        /// <param name="secondaryStorageOnly">if set to <c>true</c> [secondary storage only].</param>
        /// <param name="equipmentType">Type of the equipment.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equipment records that are not marked as inactive will be returned</param>
        /// <returns>A collection of equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        [SecurityCritical]
        public EquipmentCollectionClass EnumerateExt2(
            SecurityClass security,
            Guid targetSiteGuid,
            Guid equipmentTypeGuid,
            bool managedEquipmentOnly = false,
            bool secondaryStorageOnly = false,
            EQUIPMENT_TYPE equipmentType = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE,
            string filter = null,
            int limit = 0,
            bool hideHiddenEquipmentRecords = false)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS) && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateSql(cmd, security, managedEquipmentOnly, secondaryStorageOnly, equipmentTypeGuid, equipmentType);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByType";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompartmentType", SqlDbType.Int);
                cmd.Parameters.Add("@ManagedEquipmentFlag", SqlDbType.Bit);
                cmd.Parameters.Add("@SecondaryStorageFlag", SqlDbType.Bit);
                cmd.Parameters.Add("@EquipmentTypeGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentType", SqlDbType.Int);

                cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
                cmd.Parameters["@CompartmentType"].Value = (int)EQUIPMENT_TYPE.COMPARTMENT_TYPE;
                cmd.Parameters["@ManagedEquipmentFlag"].Value = DBNull.Value;
                if (managedEquipmentOnly)
                {
                    cmd.Parameters["@ManagedEquipmentFlag"].Value = 1;
                }

                cmd.Parameters["@SecondaryStorageFlag"].Value = DBNull.Value;
                if (secondaryStorageOnly)
                {
                    cmd.Parameters["@SecondaryStorageFlag"].Value = 1;
                }

                cmd.Parameters["@EquipmentTypeGuid"].Value = DBNull.Value;
                if (equipmentTypeGuid != Guids.AllFilterGuid)
                {
                    cmd.Parameters["@EquipmentTypeGuid"].Value = equipmentTypeGuid;
                }

                cmd.Parameters["@EquipmentType"].Value = DBNull.Value;
                if (equipmentType != EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
                {
                    cmd.Parameters["@EquipmentType"].Value = (int)equipmentType;
                }

                if (hideHiddenEquipmentRecords)
                {
                    cmd.Parameters.Add("@HideHiddenEquipmentRecords", SqlDbType.Bit).Value = 1;
                }

                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var equipmentCollection = new EquipmentCollectionClass();

            var sites = new SitesClass();
            SiteClass site = sites.GetByMemberAndProcessVariables(
                security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

            DataTable table = set.Tables[0];

            var dataDictionaries = new DataDictionariesClass();
            string translatedUnassigned = dataDictionaries.Get(security.SiteGuid, "{Unassigned}");

            var hardwareKey = new HardwareKeyClass();
            bool defense = hardwareKey.IsDescKey();

            while (table.Rows.Count != 0)
            {
                var equipment = new EquipmentClass(site);
                equipment.Load(set);
                string companyId = equipment.CompanyID;
                string qualityDate = equipment.QCDate ?? string.Empty;
                string returnToServiceDate = equipment.ReturnToServiceDate ?? string.Empty;

                if (equipment.CompanyID == "{Unassigned}")
                {
                    companyId = translatedUnassigned;
                }

                string abbrevString = EngineeringUnits.GetUnitAbbreviation(equipment.VolumeUnits);

                if (!string.IsNullOrEmpty(filter))
                {
                    if (!equipment.ID.ToUpper().Contains(filter) && !equipment.Volume.ToUpper().Contains(filter)
                        && !abbrevString.ToUpper().Contains(filter) && !companyId.ToUpper().Contains(filter)
                        && !equipment.ProductID.ToUpper().Contains(filter))
                    {
                        if (!defense && !equipment.CompanyEquipmentID.ToUpper().Contains(filter))
                        {
                            table.Rows.RemoveAt(0);
                            continue;
                        }

                        if (defense && !qualityDate.ToUpper().Contains(filter) && !returnToServiceDate.ToUpper().Contains(filter))
                        {
                            table.Rows.RemoveAt(0);
                            continue;
                        }
                    }
                }

                if (limit <= 0 || equipmentCollection.Count < limit + 1)
                {
                    equipmentCollection.Add(equipment);
                }
                else
                {
                    break;
                }

                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// Enumerates the get ID type only.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>A collection of equipment objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public EquipmentCollectionClass EnumerateGetIDTypeOnly(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))

            {
                throw new FMInsufficientRightsException();
            }

            var equipment = new EquipmentClass();
            DataSet set;

            using (var cmd = new SqlCommand())
            {
                equipment.EnumerateGetIDTypeOnlySQL(cmd);
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var equipmentCollection = new EquipmentCollectionClass();
            DataTable table = set.Tables[0];

            while (table.Rows.Count != 0)
            {
                equipment = new EquipmentClass();
                equipment.LoadIDType(set);
                //this.LoadAttachedMeters(equipment, security);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        /// <summary>
        /// Enumerates the info.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns>An array of equipment info objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        public EquipmentInfo[] EnumerateInfo(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateInfoSQL(cmd, security);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetParentEquipments";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            EquipmentInfo[] equipmentInfo;

            if (set != null && set.Tables.Count != 0 && set.Tables[0].Rows.Count != 0)
            {
                equipmentInfo = new EquipmentInfo[set.Tables[0].Rows.Count];

                for (int index = 0; index < set.Tables[0].Rows.Count; index++)
                {
                    equipmentInfo[index] = new EquipmentInfo
                    {
                        ID = set.Tables[0].Rows[index]["ID"] as string,
                        Xref = set.Tables[0].Rows[index]["Xref"] as string,
                        siteGuid = (Guid)set.Tables[0].Rows[index]["SiteGuid"],
                        identityGuid = (Guid)set.Tables[0].Rows[index]["EquipmentGuid"],
                        masterRecordGuid = (Guid)set.Tables[0].Rows[index]["_MasterRecordGuid"],
                        AssignedToSiteGuid = (Guid)set.Tables[0].Rows[index]["AssignedToSiteGuid"],
                        AssignedFromSiteGuid = (Guid)set.Tables[0].Rows[index]["AssignedFromSiteGuid"],
                        AssignedFromSiteId = (string)set.Tables[0].Rows[index]["AssignedFromSiteId"]
                    };
                }
            }
            else
            {
                equipmentInfo = new EquipmentInfo[0];
            }

            return equipmentInfo;
        }

        /// <summary>
        /// Enumerates the info for undelegated equipment.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns>An array of equipment information objects.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        public EquipmentInfo[] EnumerateInfoUndelegated(SecurityClass security, bool excludeCompartments)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetUndelegatedEquipments";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ExcludeCompartments", SqlDbType.Bit);                

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@ExcludeCompartments"].Value = excludeCompartments;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            EquipmentInfo[] equipmentInfo;

            if (set != null && set.Tables.Count != 0 && set.Tables[0].Rows.Count != 0)
            {
                equipmentInfo = new EquipmentInfo[set.Tables[0].Rows.Count];

                for (int index = 0; index < set.Tables[0].Rows.Count; index++)
                {
                    equipmentInfo[index] = new EquipmentInfo
                    {
                        // This query is limited to master records, i.e. SiteOwner, AssignedFromSite, and AssignedToSite are the same.
                        ID = set.Tables[0].Rows[index]["ID"] as string,
                        siteGuid = (Guid)set.Tables[0].Rows[index]["SiteGuid"],
                        identityGuid = (Guid)set.Tables[0].Rows[index]["EquipmentGuid"],
                        masterRecordGuid = (Guid)set.Tables[0].Rows[index]["_MasterRecordGuid"],
                        AssignedToSiteGuid = (Guid)set.Tables[0].Rows[index]["SiteGuid"],
                        AssignedFromSiteGuid = (Guid)set.Tables[0].Rows[index]["AssignedFromSiteGuid"],
                        AssignedFromSiteId = (string)set.Tables[0].Rows[index]["AssignedFromSiteId"]
                    };
                }
            }
            else
            {
                equipmentInfo = new EquipmentInfo[0];
            }

            return equipmentInfo;
        }

        /// <summary>
        /// The enumerate info by types company fuel card product and secondary storage.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="types">
        /// The types.
        /// </param>
        /// <param name="companyGuid">
        /// The company GUID.
        /// </param>
        /// <param name="fuelCardGuid">
        /// The fuel card GUID.
        /// </param>
        /// <param name="productGuid">
        /// The product GUID.
        /// </param>
        /// <param name="secondaryStorage">
        /// The secondary storage.
        /// </param>
        /// <param name="hideHiddenEquipmentRecords">If true, only equippment records that are not marked as hidden will be returned</param>
        /// <returns>
        /// The <see cref="EquipmentInfo"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        public EquipmentInfo[] EnumerateInfoByTypesCompanyFuelCardProductAndSecondaryStorage(
            SecurityClass security,
            EQUIPMENT_TYPE[] types,
            object companyGuid,
            object fuelCardGuid,
            object productGuid,
            object secondaryStorage,
            bool hideHiddenEquipmentRecords = false)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) 
                && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
                && !security.HasRight(RIGHT.MODIFY_DISPATCH) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            var equipmentTypeList = new DataTable();
            equipmentTypeList.Columns.Add("EquipmentType", typeof(int));

            if (types != null && types.Length != 0)
            {
                foreach (EQUIPMENT_TYPE equipmentType in types)
                {
	                equipmentTypeList.Rows.Add((int)equipmentType);
                }
            }

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateInfoByTypesCompanyFuelCardProductAndSourceSQL(cmd, security, types, companyGuid, fuelCardGuid, productGuid, secondaryStorage);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByFuelCardAndProduct";
                SqlParameter sqlParamEqTypeList = cmd.Parameters.AddWithValue("@EquipmentTypeList", equipmentTypeList);
                sqlParamEqTypeList.SqlDbType = SqlDbType.Structured;
                sqlParamEqTypeList.TypeName = "dbo.utt_EquipmentType";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@SecondaryStorageFlag", SqlDbType.Bit);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@CompanyGuid"].Value = DBNull.Value;
                if (companyGuid is Guid)
                {
                    cmd.Parameters["@CompanyGuid"].Value = (Guid)companyGuid;
                }

                cmd.Parameters["@FuelCardGuid"].Value = DBNull.Value;
                if (fuelCardGuid is Guid)
                {
                    cmd.Parameters["@FuelCardGuid"].Value = (Guid)fuelCardGuid;
                }

                cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
                if (productGuid is Guid)
                {
                    cmd.Parameters["@ProductGuid"].Value = (Guid)productGuid;
                }

                cmd.Parameters["@SecondaryStorageFlag"].Value = DBNull.Value;
                if (secondaryStorage is bool)
                {
                    cmd.Parameters["@SecondaryStorageFlag"].Value = (bool)secondaryStorage;
                }

                if (hideHiddenEquipmentRecords)
                {
                    cmd.Parameters.Add("@HideHiddenEquipmentRecords", SqlDbType.Bit).Value = 1;
                }

                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            EquipmentInfo[] equipmentInfo;

            if (set != null && set.Tables.Count != 0 && set.Tables[0].Rows.Count != 0)
            {
                equipmentInfo = new EquipmentInfo[set.Tables[0].Rows.Count];

                for (int index = 0; index < set.Tables[0].Rows.Count; index++)
                {
                    equipmentInfo[index] = new EquipmentInfo
                    {
                        ID = set.Tables[0].Rows[index]["ID"] as string,
                        Xref = set.Tables[0].Rows[index]["Xref"] as string,
                        siteGuid = (Guid)set.Tables[0].Rows[index]["SiteGuid"],
                        identityGuid = (Guid)set.Tables[0].Rows[index]["EquipmentGuid"]
                    };
                }
            }
            else
            {
                equipmentInfo = new EquipmentInfo[0];
            }

            return equipmentInfo;
        }

        /// <summary>
        /// Enumerates the type of the makes by.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public string[] EnumerateMakesByType(SecurityClass security, EQUIPMENT_TYPE type)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateMakesByTypeSQL(cmd, security, Type);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentMakesByType";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentType", SqlDbType.Int);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@EquipmentType"].Value = (int)type;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            DataTable table = set.Tables[0];
            var makes = new ArrayList();
            foreach (DataRow row in table.Rows)
            {
                if (row.IsNull("Make"))
                {
                    continue;
                }

                makes.Add(row["Make"]);
            }

            return (string[])makes.ToArray(typeof(string));
        }

	    /// <summary>
	    /// The enumerate managed equipment.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <returns>
	    /// The <see cref="EquipmentCollectionClass"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    public EquipmentCollectionClass EnumerateManagedEquipment(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateManagedEquipmentSQL(cmd, security);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByManagedFlag";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ManagedEquipmentFlag", SqlDbType.Bit);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@ManagedEquipmentFlag"].Value = 1;

                return this.EnumerateInternalStandard(security, cmd);
            }
        }

	    /// <summary>
	    /// The enumerate managed equipment without quality tag.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <returns>
	    /// The <see cref="EquipmentCollectionClass"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    public EquipmentCollectionClass EnumerateManagedEquipmentWithoutQualityTag(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) 
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD))
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateManagedEquipmentWithoutQualityTagSQL(cmd, security);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByManagedFlag";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@ManagedEquipmentFlag", SqlDbType.Bit);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@ManagedEquipmentFlag"].Value = 1;

                return this.EnumerateInternalStandard(security, cmd);
            }
        }

	    /// <summary>
	    /// The enumerate models by type and make.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="type">
	    /// The type.
	    /// </param>
	    /// <param name="make">
	    /// The make.
	    /// </param>
	    /// <returns>
#pragma warning disable 1584
	    /// The <see cref="string[]"/>.
#pragma warning restore 1584
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    public string[] EnumerateModelsByTypeAndMake(SecurityClass security, EQUIPMENT_TYPE type, string make)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // Equipment.EnumerateModelsByTypeAndMakeSQL(cmd, security, Type, Make);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentModelsByTypeAndMake";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentType", SqlDbType.Int);
                cmd.Parameters.Add("@Make", SqlDbType.NVarChar, 30);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@EquipmentType"].Value = (int)type;
                cmd.Parameters["@Make"].Value = make;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            DataTable table = set.Tables[0];
            var models = new ArrayList();
            foreach (DataRow row in table.Rows)
            {
                if (row.IsNull("Model"))
                {
                    continue;
                }

                models.Add(row["Model"]);
            }

            return (string[])models.ToArray(typeof(string));
        }

	    /// <summary>
	    /// The get.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="equipmentGuid">
		/// The equipment GUID.
	    /// </param>
	    /// <returns>
	    /// The <see cref="EquipmentClass"/>.
	    /// </returns>
	    public EquipmentClass Get(SecurityClass security, Guid equipmentGuid)
        {
            var sites = new SitesClass();
            SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

            return this.GetBySite(security, equipmentGuid, site);
        }

	    /// <summary>
	    /// The get basic info.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="equipmentGuid">
		/// The equipment GUID.
	    /// </param>
	    /// <param name="siteGuid">
		/// The site GUID.
	    /// </param>
	    /// <returns>
	    /// The <see cref="EquipmentClass"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    public EquipmentClass GetBasicInfo(SecurityClass security, Guid equipmentGuid, Guid siteGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            DataSet set;
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentByGuid";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
                cmd.Parameters["@EquipmentGuid"].Value = equipmentGuid;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            if (set.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = set.Tables[0].Rows[0];
            var equipment = new EquipmentClass
            {
                IdentityGuid = DataObject.getValue(row["EquipmentGuid"], Guid.Empty),
                MasterRecordGuid = DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
                ID = DataObject.getValue(row["Id"], string.Empty),
                SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty)
            };

            return equipment;
        }

	    /// <summary>
	    /// The get by company.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="companyGuid">
		/// The company GUID.
	    /// </param>
	    /// <param name="companyEquipmentID">
	    /// The company equipment ID.
	    /// </param>
	    /// <returns>
	    /// The <see cref="EquipmentClass"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    public EquipmentClass GetByCompany(SecurityClass security, Guid companyGuid, string companyEquipmentID)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
                && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
                && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByCompany";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyEquipmentId", SqlDbType.NVarChar, 30);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@CompanyGuid"].Value = companyGuid;
                cmd.Parameters["@CompanyEquipmentId"].Value = companyEquipmentID;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            if (set.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = set.Tables[0].Rows[0];
            var equipment = new EquipmentClass
            {
                IdentityGuid = DataObject.getValue(row["EquipmentGuid"], Guid.Empty),
                MasterRecordGuid = DataObject.getValue(row["MasterRecordGuid"], Guid.Empty),
                ID = DataObject.getValue(row["Id"], string.Empty),
                EquipmentTypeGuid = DataObject.getValue(row["EquipmentTypeGuid"], Guid.Empty),
                EqTypeName = DataObject.getValue(row["EqTypeName"], string.Empty)
            };

            return equipment;
        }

	    /// <summary>
	    /// The get by ID.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="id">
	    /// The ID.
	    /// </param>
	    /// <returns>
	    /// The <see cref="EquipmentClass"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    public EquipmentClass GetById(SecurityClass security, string id)
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
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
                && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) 
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.VIEW_MAPS)
				&& !security.HasRight(RIGHT.VIEW_PERSONNEL_DATA)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
                throw new FMInsufficientRightsException();
            }

            DataSet set;
            using (var cmd = new SqlCommand())
            {
                // Equipment.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByIdExt";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentId", SqlDbType.NVarChar, 30);
                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@EquipmentId"].Value = id;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            if (set.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = set.Tables[0].Rows[0];
            var equipment = new EquipmentClass
            {
                IdentityGuid = DataObject.getValue(row["EquipmentGuid"], Guid.Empty),
                MasterRecordGuid = DataObject.getValue(row["MasterRecordGuid"], Guid.Empty),
                ID = DataObject.getValue(row["Id"], string.Empty)
            };

            return equipment;
        }

	    /// <summary>
	    /// The get by site.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="equipmentGuid">
		/// The equipment GUID.
	    /// </param>
	    /// <param name="site">
	    /// The site.
	    /// </param>
	    /// <returns>
	    /// The <see cref="EquipmentClass"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    public EquipmentClass GetBySite(SecurityClass security, Guid equipmentGuid, SiteClass site)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
                && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
                && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) 
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.VIEW_DISPATCH) 
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
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

            var equipment = new EquipmentClass(site);

            // Equipment.IdentityGuid = equipmentGuid;
            using (var cmd = new SqlCommand())
            {
                // Equipment.SelectSQL(cmd, ContextUtil.IsInTransaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentByGuidExt";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@EquipmentGuid"].Value = equipmentGuid;

                equipment.Load(this.consolidatedDA.GetDataSet(cmd, security));
            }

            var qualificationMaps = new QualificationMapsClass();
            equipment.TestAndInspectionCollection = qualificationMaps.EnumerateByGuidAndType(
                security, equipment.IdentityGuid, QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT, false);

            equipment.TagAndLicenseCollection = qualificationMaps.EnumerateByGuidAndType(
                security, equipment.IdentityGuid, QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT, false);

            equipment.CompartmentCollection = this.EnumerateByEquipment(security, equipment.MasterRecordGuid);

            if (equipment.EquipmentTypeGuid != Guid.Empty)
            {
                var equipmentTypes = new EquipmentTypesClass();
                equipment.SetEquipmentType(equipmentTypes.Get(security, equipment.EquipmentTypeGuid));
            }

            var processVariables = new ProcessVariablesClass();

            var processVariableGuid = processVariables.GetIdentityGuid(
                security, PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV, 0, equipment.MasterRecordGuid, UNIT_TYPE.EQUIPMENT_UNIT);

            var volumeProcessVariable = processVariables.Get(security, processVariableGuid, UNIT_TYPE.EQUIPMENT_UNIT);

            if (volumeProcessVariable.IdentityGuid != Guid.Empty)
            {
                equipment.VolumeProcessVariable = volumeProcessVariable;
            }

            if (equipment.IdentityGuid != Guid.Empty)
            {
                var meters = new MetersClass();
                equipment.Meter = meters.EnumerateByEquipment(security, equipment.MasterRecordGuid);
            }

            return equipment;
        }

        public EquipmentClass GetByMeterGuid(SecurityClass security, Guid meterGuid)
        {
            Guid equipmentGuid = Guid.Empty;

            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
                && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
                && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
                && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
                && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
                && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
                && !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
                && !security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
                && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
                && !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
                && !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
                && !security.HasRight(RIGHT.VIEW_MAPS)
                && !security.HasRight(RIGHT.VIEW_PERSONNEL_DATA)
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                // Equipment.SelectByCardNumberAndEquipmentIDSQL(cmd, security);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT ISNULL((select EquipmentGuid from map.tblMeterToEquipment where MeterGuid = @MeterGuid), (select cast(cast(0 as binary) as uniqueidentifier))) As EquipmentGuid";
                cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@MeterGuid"].Value = meterGuid;

                var ds = this.consolidatedDA.GetDataSet(cmd, security);
                equipmentGuid = (Guid)ds.Tables[0].Rows[0]["EquipmentGuid"];
            }

            return this.Get(security, equipmentGuid);
        }

        /// <summary>
        /// Gets the identity GUID of the specified equipment
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="id">The id.</param>
        /// <returns>The guid of the specified equipment or Guid.Empty.</returns>
        public Guid GetIdentityGuid(SecurityClass security, string id)
        {
            Guid result = Guid.Empty;
            EquipmentClass equipment = this.GetById(security, id);
            if (equipment != null)
            {
                result = equipment.IdentityGuid;
            }

            return result;
        }

        /// <summary>
        /// Gets the identity GUID by card number and equipment ID.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="companyGuid">The company GUID.</param>
        /// <param name="truckCardNumber">The truck card number.</param>
        /// <returns>The guid of the specified equipment or Guid.Empty.</returns>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        public Guid GetIdentityGuidByCardNumberAndEquipmentID(SecurityClass security, Guid companyGuid, string truckCardNumber)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            var equipment = new EquipmentClass
            {
                CompanyGuid = companyGuid,
                TruckCardNumber = truckCardNumber,
                SiteGuid = security.SiteGuid
            };

            using (var cmd = new SqlCommand())
            {
                // Equipment.SelectByCardNumberAndEquipmentIDSQL(cmd, security);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByCompanyAndCardNumber";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@TruckCardNumber", SqlDbType.NVarChar, 32);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@CompanyGuid"].Value = companyGuid;
                cmd.Parameters["@TruckCardNumber"].Value = truckCardNumber;
                equipment.Load(this.consolidatedDA.GetDataSet(cmd, security));
            }

            return equipment.IdentityGuid;
        }

        public Guid GetIdentityGuidByTruckCardNumber(SecurityClass security, string truckCardNumber)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            var equipment = new EquipmentClass
            {
                TruckCardNumber = truckCardNumber,
                SiteGuid = security.SiteGuid
            };

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByTruckCardNumber";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@TruckCardNumber", SqlDbType.NVarChar, 32);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@TruckCardNumber"].Value = truckCardNumber;
                equipment.Load(this.consolidatedDA.GetDataSet(cmd, security));
            }

            return equipment.IdentityGuid;
        }

        /// <summary>
        /// The get identity GUID by company GUID and equipment ID.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="companyGuid">
        /// The company GUID.
        /// </param>
        /// <param name="companyEquipmentID">
        /// The company equipment ID.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        public Guid GetIdentityGuidByCompanyGuidAndEquipmentID(SecurityClass security, Guid companyGuid, string companyEquipmentID)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
                && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
                && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) )
            {
                throw new FMInsufficientRightsException();
            }

            DataSet set;
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentsByCompany";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@CompanyEquipmentId", SqlDbType.NVarChar, 30);

                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@CompanyGuid"].Value = companyGuid;
                cmd.Parameters["@CompanyEquipmentId"].Value = companyEquipmentID;
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            if (set.Tables[0].Rows.Count == 0)
            {
                return Guid.Empty;
            }

            DataRow row = set.Tables[0].Rows[0];
            return DataObject.getValue(row["EquipmentGuid"], Guid.Empty);
        }

	    /// <summary>
	    /// The get latest row version by source.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <returns>
	    /// The <see cref="string"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    public string GetLatestRowVersionBySource(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            string result = string.Empty;

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                // equipment.GetLatestRowVersionBySource(cmd, security);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetEquipmentLatestRowVersion";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@HydrantCartType", SqlDbType.Int);
                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                cmd.Parameters["@HydrantCartType"].Value = (int)EQUIPMENT_TYPE.HYDRANT_CART_TYPE;
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

	    /// <summary>
		/// The get master record GUID.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="id">
	    /// The ID.
	    /// </param>
	    /// <returns>
	    /// The <see cref="Guid"/>.
	    /// </returns>
	    public Guid GetMasterRecordGuid(SecurityClass security, string id)
        {
            Guid result = Guid.Empty;
            EquipmentClass equipment = this.GetById(security, id);
            if (equipment != null)
            {
                result = equipment.MasterRecordGuid;
            }

            return result;
        }

	    /// <summary>
	    /// The enumerate update versions.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <returns>
	    /// The <see cref="DataSet"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    public DataSet EnumerateUpdateVersionsForOpc(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            var equipment = new EquipmentClass();

            using (var sqlCommand = new SqlCommand())
            {
                equipment.EnumerateNotificationForOpcSQL(security, sqlCommand);
                return this.consolidatedDA.GetDataSet(sqlCommand, security);
            }
        }

		/// <summary>
		/// The enumerate update versions.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		/// <exception cref="Exception">
		/// </exception>
		public DataSet EnumerateUpdateVersions(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

            if (!security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_DISPATCH))
            {
                throw new FMInsufficientRightsException();
            }

            var equipment = new EquipmentClass();

            using (var sqlCommand = new SqlCommand())
            {
                sqlCommand.CommandText = equipment.EnumerateNotificationSQL(security);
                return this.consolidatedDA.GetDataSet(sqlCommand, security);
            }
        }

	    /// <summary>
	    /// The import.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="equipment">
	    /// The equipment.
	    /// </param>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Import(SecurityClass security, EquipmentClass equipment)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (equipment == null)
            {
                throw new ArgumentNullException(nameof(equipment));
            }

            SecurityClass securityClone = security.Clone();

            var companies = new CompaniesClass();
            var qualifications = new QualificationsClass();
            var products = new ProductsClass();

            try
            {
                equipment.IdentityGuid = this.GetIdentityGuid(securityClone, equipment.ID);

                // If the entity exists and is not owned by this site, do not update it.
                if (equipment.IdentityGuid != Guid.Empty && equipment.SiteGuid != securityClone.SiteGuid)
                {
                    return;
                }

                if (equipment.CompanyID != "{Unassigned}" && string.IsNullOrEmpty(equipment.CompanyID) == false)
                {
                    equipment.CompanyGuid = companies.GetIdentityGuid(securityClone, equipment.CompanyID);
                    if (equipment.CompanyGuid == Guid.Empty)
                    {
                        var carrier = new CompanyClass { ID = equipment.CompanyID };
                        var role = new CompanyRoleMapClass { Role = COMPANY_ROLE.CARRIER };
                        carrier.RoleCollection.Add(role);
                        equipment.CompanyGuid = companies.Add(securityClone, carrier);
                    }
                }

                if (equipment.ProductID != "{Unassigned}" && string.IsNullOrEmpty(equipment.ProductID) == false)
                {
                    equipment.ProductGuid = products.GetIdentityGuid(securityClone, equipment.ProductID);
                    if (equipment.ProductGuid == Guid.Empty)
                    {
                        var product = new ProductClass
                        {
                            ID = equipment.ProductID,
                            ProductType = ProductType.ComponentProduct
                        };

                        equipment.ProductGuid = products.Add(securityClone, product);
                    }
                }

                if (equipment.EquipmentTypeGuid == Guid.Empty && string.IsNullOrEmpty(equipment.TypeClass) == false)
                {
                    var equipmentTypes = new EquipmentTypesClass();
                    Guid equipmentTypeGuid = equipmentTypes.GetIdentityGuid(securityClone, equipment.TypeClass);

                    if (equipmentTypeGuid == Guid.Empty)
                    {
                        var equipmentType = new EquipmentTypeClass
                        {
                            ID = equipment.TypeClass,
                            Attribute = EQUIPMENT_TYPE.OTHER_TYPE
                        };

                        equipmentType.IdentityGuid = equipmentTypes.Add(securityClone, equipmentType);

                        equipment.SetEquipmentType(equipmentType);
                    }
                    else
                    {
                        equipment.SetEquipmentType(equipmentTypes.Get(securityClone, equipmentTypeGuid));
                    }
                }

                foreach (QualificationMapClass testAndInspection in equipment.TestAndInspectionCollection)
                {
                    Guid qualificationGuid = qualifications.GetIdentityGuid(
                        securityClone, QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION, testAndInspection.ID);
                    if (qualificationGuid == Guid.Empty)
                    {
                        var qualification = new QualificationClass
                        {
                            ID = testAndInspection.ID,
                            Type = QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION
                        };

                        qualificationGuid = qualifications.Add(securityClone, qualification);
                    }

                    testAndInspection.AssignedGuid = qualificationGuid;
                }

                foreach (QualificationMapClass tagAndLicense in equipment.TagAndLicenseCollection)
                {
                    Guid qualificationGuid = qualifications.GetIdentityGuid(
                        securityClone, QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE, tagAndLicense.ID);
                    if (qualificationGuid == Guid.Empty)
                    {
                        var qualification = new QualificationClass
                        {
                            ID = tagAndLicense.ID,
                            Type = QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE
                        };

                        qualificationGuid = qualifications.Add(securityClone, qualification);
                    }

                    tagAndLicense.AssignedGuid = qualificationGuid;
                }

                if (equipment.ExportUseFuelCard != null && string.IsNullOrEmpty(equipment.ExportUseFuelCard.ID) == false)
                {
                    var cards = new FuelCardsClass();

                    FuelCardClass card = cards.Get(
                        securityClone, cards.GetIdentityGuid(securityClone, equipment.ExportUseFuelCard.ID), false);

                    if (card.IdentityGuid != Guid.Empty)
                    {
                        equipment.FuelCardGuid = card.IdentityGuid;
                        equipment.FuelCardID = card.ID;
                    }
                    else
                    {
                        equipment.FuelCardGuid = cards.Add(securityClone, equipment.ExportUseFuelCard);
                        equipment.FuelCardID = equipment.ExportUseFuelCard.ID;
                    }
                }
                else if (equipment.FuelCardID != "{Unassigned}" && string.IsNullOrEmpty(equipment.FuelCardID) == false)
                {
                    var cards = new FuelCardsClass();

                    equipment.FuelCardGuid = cards.GetIdentityGuid(securityClone, equipment.FuelCardID);

                    if (equipment.FuelCardGuid == Guid.Empty)
                    {
                        var fuelCard = new FuelCardClass
                        {
                            ID = equipment.FuelCardID
                        };

                        equipment.FuelCardGuid = cards.Add(securityClone, fuelCard);
                    }
                }

                if (equipment.IdentityGuid == Guid.Empty)
                {
                    equipment.IdentityGuid = this.Add(securityClone, equipment);
                }
                else
                {
                    foreach (EquipmentClass compartment in equipment.CompartmentCollection)
                    {
                        compartment.IdentityGuid = this.GetIdentityGuid(securityClone, compartment.ID);
                    }

                    this.Modify(securityClone, equipment);
                }
            }
            catch (Exception e)
            {
                throw new Exception("[Equipment Import Error ID] : " + equipment.ID + ", reason: " + e.Message);
            }
        }

	    /// <summary>
	    /// The modify.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="equipment">
	    /// The equipment.
	    /// </param>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, EquipmentClass equipment)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

			if ( equipment == null )
            {
                throw new ArgumentNullException(nameof(equipment));
            }

            if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
                && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
            {
                throw new FMInsufficientRightsException();
            }

			this.Validate(security, equipment);

			this.UpdateCapacity(equipment);

			// Set UserData(list type) to defaults if they are blanks
			UserDataFieldsClass.SetDefaults(security, equipment.UserData, ENTITY_TYPE.EQUIPMENT);

			EquipmentClass oldEquipment = this.Get(security, equipment.IdentityGuid);
			
   
         if (oldEquipment.IdentityGuid == Guid.Empty)
         {
               throw new Exception("Equipment Not Found");
         }

			// only if the user is changing the ID
			if (equipment.ID.Equals(oldEquipment.ID, StringComparison.OrdinalIgnoreCase) == false)
			{
				// will throw exception when there is trx associated with current equipement if configured
				this.CheckAssociatedTrx(security, equipment.IdentityGuid, CantChangeIDMessageDueToTrx);
			}

            // Check for Locked Out
			if ( oldEquipment.LockedOut != equipment.LockedOut && equipment.LockedOut )
            {
                var alarmAndEventLogs = new AlarmAndEventLogsClass();
				alarmAndEventLogs.Add(security, equipment.LockOutEvent);
            }

            //// We need to determine if the meter record will be deleted before updating the equipment record.
            //// This is because if we delete the meter record first, we will violate a foreign key constraint.
            //// ModifyMeter will either add the meter, update the meter, or return true if the meter needs to be deleted.
            //bool deleteMeter = this.ModifyMeter(security, equipment);
            //Guid meterGuidToBeDeleted = Guid.Empty;


            //// If we detect that the meter will be deleted, save the meter's guid and null out the
            //// guid in the equipment record.
            //if (deleteMeter)
            //{
            //    meterGuidToBeDeleted = equipment.AssignedToMeterGuid;
            //    equipment.AssignedToMeterGuid = Guid.Empty;
            //}

            equipment.UpdatedDate = DateTimeOffset.Now;
			equipment.UpdatedBy = security.UserID;

            var entityToSiteMaps = new EntityToSiteMaps();

			if ( equipment.SiteGuid != oldEquipment.SiteGuid )
            {
				entityToSiteMaps.PurgeAllEquipmentMappings(security, equipment.MasterRecordGuid, false);
            }

            using (var cmd = new SqlCommand())
            {
				equipment.UpdateSQL(cmd);
                this.consolidatedDA.ExecuteQuery(security, cmd);
            }

			if ( equipment.SiteGuid != oldEquipment.SiteGuid )
            {
                // Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(equipment);
                Guid currentSiteContext = security.SiteGuid;

                // When changing ownership of an entity that supports Cascading Assignment, need to make sure that the base mapping is created with the AssignedFromSiteGuid being the same as the Owner Site Guid (and the AssignedToSiteGuid), and not be set with the Site Context Guid which in the case of a Change of Ownership would be different from the new Owner Site Guid.
                // The Security SiteGuid swap below effectively does so by supplying the EntityToSiteMaps.Add() operation with the correct SiteGuid to use to set the AssignedFromSiteGuid.
				security.SiteGuid = equipment.SiteGuid;
                //Add a self-assignment mapping to the owner site for the target equipment only, not to its compartments. The Compartments are handled separately further down in this same method. If we change the entity-to-site assignment of the children compartments here, it will cause an entity not found error when trying to retrieve the children compartments before updating them further down.
                entityToSiteMaps.AddEquipmentMapping(security, newEntityToSiteMap, false);
                security.SiteGuid = currentSiteContext;
            }
            var qualificationMaps = new QualificationMapsClass();

            qualificationMaps.ModifyCollection(
				security, equipment.IdentityGuid, equipment.TestAndInspectionCollection, oldEquipment.TestAndInspectionCollection);

            qualificationMaps.ModifyCollection(
				security, equipment.IdentityGuid, equipment.TagAndLicenseCollection, oldEquipment.TagAndLicenseCollection);

			// If the InServiceFlag or ReturnToServiceDate has changed then
			// the change needs to be saved in a new maintenance record.
			if (equipment.InServiceFlag != oldEquipment.InServiceFlag
				|| equipment.ReturnToServiceDate != oldEquipment.ReturnToServiceDate)
			{
				if (string.IsNullOrEmpty(equipment.ReturnToServiceDate))
				{
					throw new ApplicationException("Return To Service Date required");
				}

				if (string.IsNullOrEmpty(equipment.StatusDescription))
				{
					throw new ApplicationException("Status Description required");
				}

				var logs = new EquipmentMaintenanceLogsClass();

				var log = new EquipmentMaintenanceLogClass()
				{

					EquipmentGuid = equipment.IdentityGuid,
					EquipmentID = equipment.ID,
					EstReturnToServiceDate = equipment.ReturnToServiceDateObject.Value,
					InServiceFlag = equipment.InServiceFlag ? (byte)1 : (byte)0,
					MaintenanceReasonGuid = equipment.StatusDescriptionGuid,
					MaintenanceReason = equipment.StatusDescription,
					EquipmentType = equipment.TypeClass
				};

				logs.Add(security, log);
			}

            // The following Equipment attributes are not covered by Equipment Record Versioning: Compartments, Process Variables, and Meters.
			if ( equipment.IdentityGuid.Equals(equipment.MasterRecordGuid) )
            {
				this.UpdateCompartments(security, equipment, oldEquipment);

                var processVariables = new ProcessVariablesClass();
                var newProcessVariableCollection = new ProcessVariableCollectionClass
					{
						equipment.VolumeProcessVariable
					};

                var oldProcessVariableCollection = new ProcessVariableCollectionClass();
                if (oldEquipment.VolumeProcessVariable.IdentityGuid != Guid.Empty)
                {
                    oldProcessVariableCollection.Add(oldEquipment.VolumeProcessVariable);
                }

                processVariables.ModifyCollection(
					security, equipment.MasterRecordGuid, newProcessVariableCollection, oldProcessVariableCollection);

                //// Did we previously detect that a meter record needs to be deleted? 
                //// If so, it is now safe to delete it since the equipment record's meter guid has been nulled out. 
                //if (deleteMeter)
                //{
                //    var meters = new MetersClass();
                //    meters.Purge(security, meterGuidToBeDeleted);
                //}

                this.UpdateMeters(security, equipment, oldEquipment);
            }

            // Record Versioning does not apply to Compartments (Equipments with a ParentEquipmentGuid)
			if ( equipment.ParentEquipmentGuid == Guid.Empty )
            {
				if ( equipment.EquipmentTypeGuid != oldEquipment.EquipmentTypeGuid )
	            {
					this.ProcessEquipmentTypeChange(security, equipment);
	            }
	            else
	            {
					this.PropagateUpdate(security, equipment);
	            }
            }

			// TODO: Temporary commented out so that QA does not test change queue features.
            // ChangeQueueRecordsClass.ProcessChangeQueueRecords(security, ChangeQueueEventType.Modify, Equipment);
        }


        /// <summary>
        /// Propagates the latest updates made to an Equipment record to its child record versions.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="equipment">The equipment.</param>
        /// <exception cref="System.ArgumentNullException">security</exception>
        public void PropagateUpdate(SecurityClass security, EquipmentClass equipment)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_PropagateEquipmentRevisionByEntityRecordChange";
                cmd.Parameters.Add("@SourceEquipmentGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@SourceEquipmentGuid"].Value = equipment.IdentityGuid;
                this.consolidatedDA.ExecuteQuery(security, cmd);

                // Next, enqueue a replication of global changes up to a master record version.
                // if the change was made to a child record.
                if (equipment.IdentityGuid != equipment.MasterRecordGuid)
                {
                    cmd.CommandText = "erv.usp_AddGlobalSpecificQueueRecord";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                    cmd.Parameters["@EntityTypeId"].Value = EquipmentClass.ENTITY_TYPE_ID;
                    cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);
                    cmd.Parameters["@EntityGuid"].Value = equipment.IdentityGuid;
                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100);
                    cmd.Parameters["@UserId"].Value = security.UserID;
                    this.consolidatedDA.ExecuteQuery(security, cmd);
                }
            }
        }


        /// <summary>
        /// Responds to an EquipmentType change on an Equipment record and adjusts the child record versions accordingly.
        /// This operation also propagates all the other Equipment field changes, just like the PropagateUpdate() operation.
        /// Key assumption: EquipmentType is a filter field on the Equipment EntitySegmentTemplate, and as such is not configurable 
        /// in the Field Level Control configuration for Equipments, i.e. the EquipmentType of an Equipment record can only 
        /// be edited on the master record version.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="equipment">The equipment whose EquipmentType field has been modified</param>
        /// <exception cref="System.ArgumentNullException">security</exception>
        public void ProcessEquipmentTypeChange(SecurityClass security, EquipmentClass equipment)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_ProcessEquipmentTypeChange";
                cmd.Parameters.Add("@EntityRecordGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@EntityRecordGuid"].Value = equipment.IdentityGuid;
                this.consolidatedDA.ExecuteQuery(security, cmd);
            }
        }


        /// <summary>
        /// Purges the specified equipment.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="equipmentGuid">The equipment GUID.</param>
        /// <exception cref="System.ArgumentNullException">security</exception>
        /// <exception cref="System.Exception">Access Denied</exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid equipmentGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
            {
                throw new FMInsufficientRightsException();
            }

            EquipmentClass equipment = this.Get(security, equipmentGuid);
            if (equipment.IdentityGuid == Guid.Empty)
            {
                throw new Exception("Equipment Not Found");
            }

            if (equipment.IdentityGuid != equipment.MasterRecordGuid)
            {
                throw new Exception("Cannot delete an Equipment child record version directly");
            }

			// will throw exception when there is trx associated with current equipement if configured
			this.CheckAssociatedTrx(security, equipment.IdentityGuid, CantDeleteMessageDueToTrx);
            
            // will throw exception when there is test resuts associated with current equipement
            this.CheckTestResults(security, equipment.IdentityGuid, CantDeleteMessageDueToTestResuls);

            // will throw exception when there is maintenance records associated with current equipement
            this.CheckMaintenanceLog(security, equipment.IdentityGuid, CantDeleteMessageDueToMaintenance);

            // Purge from EntityToSiteMap
            var entityToSiteMaps = new EntityToSiteMaps();
            entityToSiteMaps.PurgeAllEquipmentMappings(security, equipment.MasterRecordGuid, false);

            // Purge any compartments
			   if(equipment.CompartmentCollection != null)
			   {
				   foreach (EquipmentClass compartment in equipment.CompartmentCollection)
				   {
					   this.Purge(security, compartment.IdentityGuid);
				   }
			   }
				
            if (equipment.VolumeProcessVariable.IdentityGuid != Guid.Empty)
            {
                var processVariables = new ProcessVariablesClass();
                processVariables.Purge(
                    security, equipment.VolumeProcessVariable.IdentityGuid, equipment.VolumeProcessVariable.UnitType);
            }

            // Purge any qualification maps
            var qualificationMaps = new QualificationMapsClass();
            qualificationMaps.ModifyCollection(security, equipment.IdentityGuid, null, equipment.TestAndInspectionCollection);
            qualificationMaps.ModifyCollection(security, equipment.IdentityGuid, null, equipment.TagAndLicenseCollection);

            // Purge any appointments
            var appointments = new AppointmentsClass();
            AppointmentCollectionClass appointmentCollection = appointments.EnumerateByAssetGuid(
                security, "Equipment", equipment.IdentityGuid);
            foreach (AppointmentClass appointment in appointmentCollection)
            {
                appointments.Purge(security, appointment.IdentityGuid);
            }

             this.UpdateMeters(security, null, equipment);
            using (var cmd = new SqlCommand())
            {
                equipment.PurgeSQL(cmd);
                this.consolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        #endregion

        #region Explicit Interface Methods
	    /// <summary>
	    /// The insert.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="Object">
	    /// The object.
	    /// </param>
	    /// <param name="preOperation">
	    /// The pre operation.
	    /// </param>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
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

	    /// <summary>
	    /// The purge.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="Object">
	    /// The object.
	    /// </param>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
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

            // Purge Equipment Deleted/Undeleted
		    var siteObject = Object as SiteClass;

	        if (siteObject != null)
            {
                var site = siteObject;
                EquipmentCollectionClass equipmentCollection = this.EnumerateExt2(security, site.SiteGuid, Guids.AllFilterGuid);
                var entityToSiteMaps = new EntityToSiteMaps();
                foreach (EquipmentClass equipment in equipmentCollection)
                {
                    if (site.SiteGuid == equipment.SiteGuid && equipment.MasterRecordGuid == equipment.IdentityGuid)
                    {
                        this.Purge(security, equipment.IdentityGuid);
                    }
                    else
                    {
                        var entityToSiteMap = new EntityToSiteMapClass(equipment) { SiteGuid = site.SiteGuid };
                        entityToSiteMaps.Purge(security, entityToSiteMap);
                    }
                }
            }
			else
	        {
	            var company = Object as CompanyClass;
	            if (company != null)
	            {
	                foreach (EquipmentClass equipment in company.EquipmentCollection)
	                {
	                    var fullEquipment = this.Get(security, equipment.MasterRecordGuid);
	                    fullEquipment.CompanyGuid = Guid.Empty;
	                    fullEquipment.CompanyID = "{Unassigned}";
	                    this.Modify(security, fullEquipment);
	                }
	            }
	            else
	            {
	                var entityToSiteMap = Object as EntityToSiteMapClass;
	                if (entityToSiteMap != null)
	                {
	                    // Modify Equipment to remove reference to this Company
	                    if ( entityToSiteMap.TypeID == ENTITY_TYPE.COMPANY )
	                    {
	                        EquipmentCollectionClass equipmentCollection;
	                        Guid siteGuid = security.SiteGuid;

	                        try
	                        {
	                            security.SiteGuid = entityToSiteMap.SiteGuid;
	                            equipmentCollection = this.EnumerateByCompany(security, entityToSiteMap.IdentityGuid);
	                        }
	                        finally
	                        {
	                            security.SiteGuid = siteGuid;
	                        }

	                        if (equipmentCollection != null)
	                        {
	                            foreach (EquipmentClass equipment in equipmentCollection)
	                            {
	                                if (equipment.SiteGuid != entityToSiteMap.SiteGuid)
	                                {
	                                    continue;
	                                }

	                                equipment.CompanyGuid = Guid.Empty;
	                                equipment.UpdatedDate = DateTimeOffset.Now;
	                                equipment.UpdatedBy = security.UserID;

	                                using (var cmd = new SqlCommand())
	                                {
	                                    equipment.UpdateSQL(cmd);
	                                    this.consolidatedDA.ExecuteQuery(security, cmd);
	                                }
	                            }
	                        }

	                        return;
	                    }

	                    if ( entityToSiteMap.TypeID == ENTITY_TYPE.PRODUCT )
	                    {
	                        security.SiteGuid = entityToSiteMap.SiteGuid;

	                        Guid siteGuid = security.SiteGuid;
	                        if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
	                            && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
	                        {
	                            throw new FMInsufficientRightsException();
	                        }

	                        security.SiteGuid = siteGuid;

	                        try
	                        {
	                            using (var cmd = new SqlCommand())
	                            {
	                                cmd.CommandText = "UPDATE tblEquipment SET ProductGuid = NULL" + ", UpdatedDate = @UpdatedDate "
	                                                  + ", UpdatedBy = @UpdatedBy " + " WHERE SiteGuid = @SiteGuid "
	                                                  + " AND ProductGuid = @ProductGuid ";

	                                cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
	                                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
	                                cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
	                                cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);

	                                cmd.Parameters["@UpdatedDate"].Value = DateTimeOffset.Now;
	                                cmd.Parameters["@UpdatedBy"].Value = security.UserID;
	                                cmd.Parameters["@SiteGuid"].Value = entityToSiteMap.SiteGuid;
	                                cmd.Parameters["@ProductGuid"].Value = entityToSiteMap.IdentityGuid;

	                                this.consolidatedDA.ExecuteQuery(security, cmd);
	                            }
	                        }
	                        catch (Exception e)
	                        {
	                            throw new Exception(e.Message);
	                        }
	                    }
	                }
	            }
	        }
        }

	    /// <summary>
	    /// The update.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="Object">
	    /// The object.
	    /// </param>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
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
                EquipmentCollectionClass equipmentCollection = this.Enumerate(security);
                var entityToSiteMaps = new EntityToSiteMaps();
                foreach (EquipmentClass equipment in equipmentCollection)
                {
                    if (site.SiteGuid == equipment.SiteGuid)
                    {
                        EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
                            security, equipment.EntityType, equipment.IdentityGuid);

                        foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
                        {
                            if (entityToSiteMap.SiteGuid != site.SiteGuid)
                            {
                                entityToSiteMap.ID = equipment.ID;
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
	    /// The enumerate internal standard.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="cmd">
	    /// The SQL Command.
	    /// </param>
	    /// <returns>
	    /// The <see cref="EquipmentCollectionClass"/>.
	    /// </returns>
	    /// <exception cref="ArgumentNullException">
	    /// </exception>
	    /// <exception cref="Exception">
	    /// </exception>
	    private EquipmentCollectionClass EnumerateInternalStandard(SecurityClass security, SqlCommand cmd)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
                && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))

            {
                throw new FMInsufficientRightsException();
            }

            DataSet set = this.consolidatedDA.GetDataSet(cmd, security);

            var equipmentCollection = new EquipmentCollectionClass();

            var sites = new SitesClass();
            SiteClass site = sites.GetByMemberAndProcessVariables(
                security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false);

            DataTable table = set.Tables[0];
            while (table.Rows.Count != 0)
            {
                var equipment = new EquipmentClass(site);
                equipment.Load(set);
                equipmentCollection.Add(equipment);
                table.Rows.RemoveAt(0);
            }

            return equipmentCollection;
        }

        ///// <summary>
        ///// Determine if the meter information belonging to this equipment record
        /////     needs to be inserted, updated, or deleted.
        ///// </summary>
        ///// <param name="security">
        ///// Security information
        ///// </param>
        ///// <param name="equipment">
        ///// The equipment record to examine meter information for
        ///// </param>
        ///// <returns>
        ///// True if the meter needs to be deleted. False otherwise.
        ///// </returns>
        //private bool ModifyMeter(SecurityClass security, EquipmentClass equipment)
        //{
        //    var meters = new MetersClass();

        //    // Does the equipment have a meter associated with it?
        //    if (equipment.Meter != null)
        //    {
        //        if (equipment.AssignedToMeterGuid != Guid.Empty)
        //        {
        //            // The meter record exists already, and should be updated.
        //            meters.Modify(security, equipment.Meter);
        //        }
        //        else
        //        {
        //            // The meter information is new and should be added.
        //            equipment.AssignedToMeterGuid = meters.Add(security, equipment.Meter);
        //        }
        //    }
        //    else if (equipment.AssignedToMeterGuid != Guid.Empty)
        //    {
        //        // We have to purge the meter after updating the equipment record to avoid violating a foreign key constraint.
        //        // Return true so we know to delete the meter later.
        //        return true;
        //    }

        //    return false;
        //}

        /// <summary>
        /// Updates the capacity.
        /// </summary>
        /// <param name="equipment">The equipment.</param>
        private void UpdateCapacity(EquipmentClass equipment)
        {
            if (!equipment.IsMultiCompartment)
            {
                return;
            }

            if (equipment.Type == EQUIPMENT_TYPE.COMPARTMENT_TYPE)
            {
                return;
            }

            EquipmentCollectionClass compartments = equipment.CompartmentCollection;

            equipment.SICapacity.SIValue = 0.0;

            if (compartments != null)
            {
                foreach (EquipmentClass compartment in compartments)
                {
                    equipment.SICapacity.SIValue += compartment.SICapacity.SIValue;
                }
            }
        }

		/// <summary>
		/// This method purges all existing equipment compartments and adds the new
		/// equipment compartments into the database.  The reason this method is used is that
		/// the keys are compartment sequence numbers. When a compartment item is delete the
		/// next in line receives the delete item's number. Because of this when the insert is
		/// performed the database throws a duplicate key error.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="equipment">The equipment.</param>
		/// <param name="oldEquipment">The old equipment.</param>
        private void UpdateCompartments(SecurityClass security, EquipmentClass equipment, EquipmentClass oldEquipment)
        {
            var newCompartments = equipment.CompartmentCollection;
	        EquipmentCollectionClass existingCompartments = null;
	        if (oldEquipment != null)
	        {
		        existingCompartments = oldEquipment.CompartmentCollection;
	        }

            if (newCompartments != null)
            {
				foreach(EquipmentClass newCompartment in newCompartments)
	            {
                    newCompartment.ID = equipment.ID + "_" + newCompartment.EquipmentSequence;
                    newCompartment.ParentEquipmentGuid = equipment.IdentityGuid;

					if (newCompartment.IdentityGuid == Guid.Empty)
					{
						Guid compartmentGuid = this.Add(security, newCompartment);
                        CopyEntityAssignment(security, compartmentGuid, equipment.IdentityGuid);
                    }

			        else if (existingCompartments != null)
			        {
				        int existingItem;

				        for (existingItem = 0; existingItem < existingCompartments.Count; existingItem++)
				        {
					        var existingCompartment = existingCompartments[existingItem];

							if (Int32.Parse(existingCompartment.EquipmentSequence) == Int32.Parse(newCompartment.EquipmentSequence)
							&& newCompartment.IdentityGuid != existingCompartment.IdentityGuid)
							{
								this.Purge(security, existingCompartment.IdentityGuid);
								this.Modify(security, newCompartment);
								break;
							}

					        if (newCompartment.IdentityGuid == existingCompartment.IdentityGuid)
					        {
						        this.Modify(security, newCompartment);
						        break;
					        }
				        }

				        if (existingItem == existingCompartments.Count)
				        {
							this.Modify(security, newCompartment);
						}
				        else
				        {
					        existingCompartments.RemoveAt(existingItem);
				        }
			        }
			        else
			        {
						this.Modify(security, newCompartment);
		            }
	            }
            }

			if (existingCompartments != null)
			{
				foreach (EquipmentClass existingCompartment in existingCompartments)
				{
					if (newCompartments?.Find(x => x.IdentityGuid == existingCompartment.IdentityGuid) == null)
					{
						this.Purge(security, existingCompartment.IdentityGuid);
					}
				}
			}
		}


        /// <summary>
        /// Applies the same entity-to-site assignment of a SourceEquipment to another Equipment
        /// </summary>
        /// <param name="targetCompartment"></param>
        /// <param name="sourceEquipment"></param>
        private void CopyEntityAssignment(SecurityClass security, Guid targetEquipmentGuid, Guid sourceEquipmentGuid)
        {
            EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
            EntityToSiteMapCollectionClass sourceEquipmentSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, ENTITY_TYPE.EQUIPMENT, sourceEquipmentGuid);
            EntityToSiteMapCollectionClass targetEquipmentSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, ENTITY_TYPE.EQUIPMENT, targetEquipmentGuid);            
            for (int i = 0; i < sourceEquipmentSiteMapCollection.Count; i++)
            {
                bool mappingFound = false;
                for (int j = 0; j < targetEquipmentSiteMapCollection.Count; j++)
                {
                    if (targetEquipmentSiteMapCollection[j].SiteGuid == sourceEquipmentSiteMapCollection[i].SiteGuid)
                    {
                        mappingFound = true;
                        break;
                    }
                }
                if (!mappingFound)
                {
                    EntityToSiteMapClass entityToSiteMap = sourceEquipmentSiteMapCollection[i];
                    entityToSiteMap.IdentityGuid = targetEquipmentGuid;
                    entityToSiteMap.ID = null;
                    entityToSiteMaps.AddEquipmentMapping(security, entityToSiteMap, false);
                }
            }
        }

        /// <summary>
		/// Compare the existing equipment's meters to the equipment we are adding, updating, or deleting
		/// and add, update, or delete meters appropriately.
		/// </summary>
		/// <param name="security">Security Information</param>
		/// <param name="equipment"> The equipment that is being updated, inserted, or deleted</param>
		/// <param name="oldEquipment"> The equipment as it existed in the database before the user's action</param>
		private void UpdateMeters(SecurityClass security, EquipmentClass equipment, EquipmentClass oldEquipment)
        {
            MetersClass meters = new MetersClass();

            // If the new equipment parameter was null, that means we're deleting the tank.
            // If it's not null, that means we may have to add, update, or delete meters.
            if (equipment != null)
            {
                foreach (MeterClass meter in equipment.Meter)
                {
                    if (oldEquipment != null)
                    {
                        // If the meter belonging to the new equipment has no identity guid, 
                        // it is new and needs to be added to the database
                        if (meter.IdentityGuid == Guid.Empty)
                        {
                            meters.Add(security, meter);
                            meters.AddEquipmentMap(security, meter, equipment.MasterRecordGuid);
                        }
                        else
                        {
                            // If we can find a meter with the same identity guid in the old tank's set of meters, 
                            // the meter needs to be updated. After the update we remove it from the old tank so we know not to delete it later
                            foreach (MeterClass oldMeter in oldEquipment.Meter)
                            {
                                if (oldMeter.IdentityGuid == meter.IdentityGuid)
                                {
                                    meters.Modify(security, meter);
                                    oldEquipment.Meter.Remove(oldMeter);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Delete any meters that are still present on the old equipment object, 
            // because they weren't found in the new tank.
            if (oldEquipment != null)
            {
                foreach (MeterClass oldMeter in oldEquipment.Meter)
                {
                    meters.Purge(security, oldMeter.IdentityGuid);
                }
            }
        }

        /// <summary>
        /// Validates the specified equipment.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="equipment">The equipment.</param>
        /// <exception cref="System.Exception">ID Required</exception>
        private void Validate(SecurityClass security, EquipmentClass equipment)
        {
            if (string.IsNullOrEmpty(equipment.ID))
            {
                throw new Exception("ID Required");
            }

            if (equipment.ID == "{None}" || equipment.ID == "{Unassigned}" || equipment.ID == "{All}")
            {
                throw new Exception("ID is reserved key word " + equipment.ID);
            }

            if (equipment.Notes.Length > 1000)
            {
                throw new Exception("Exceeded max length (1000)");
            }

            Guid equipmentGuid = this.GetIdentityGuid(security, equipment.ID);

            if (equipmentGuid != Guid.Empty && equipmentGuid != equipment.IdentityGuid)
            {
                throw new Exception("Equipment Exists");
            }

            if (!string.IsNullOrEmpty(equipment.TruckCardNumber))
            {
                equipmentGuid = this.GetIdentityGuidByTruckCardNumber(security, equipment.TruckCardNumber);
            }
 
            if(equipmentGuid != Guid.Empty && equipmentGuid != equipment.IdentityGuid)
            {
                throw new Exception("Duplicate Card Number");
            }

            EquipmentClass eqpObj = this.GetByCompany(security, equipment.CompanyGuid, equipment.CompanyEquipmentID);
            if (eqpObj != null && eqpObj.IdentityGuid != Guid.Empty && eqpObj.IdentityGuid != equipment.IdentityGuid)
            {
                throw new Exception("Duplicate Company Equipment ID");
            }

            var hardwareKey = new HardwareKeyClass();

            // Validate Equipment Type for non-BSME versions when equipment is not a compartment as indicated by non empty ParentEquipmentGuid
            if (equipment.ParentEquipmentGuid == Guid.Empty && !hardwareKey.IsDescKey())
            {
                if (equipment.EquipmentTypeGuid == Guid.Empty && !string.IsNullOrEmpty(equipment.TypeClass))
                {
                    var equipmentTypes = new EquipmentTypesClass();
                    equipment.EquipmentTypeGuid = equipmentTypes.GetIdentityGuid(security, equipment.TypeClass);
                }

                if (equipment.EquipmentTypeGuid == Guid.Empty)
                {
                    throw new Exception("Equipment Type is Required.");
                }
            }

            if (hardwareKey.IsDescKey() && string.IsNullOrEmpty(equipment.Xref))
            {
	            string id = equipment.ID.Trim();

	            equipment.Xref = id.Length < 4 ? id : id.Substring(id.Length - 4, 4);
            }

	        if (equipment.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
	        {
		        this.ValidateUserData(security, equipment);
	        }
        }

		/// <summary>
		/// Find out if the given equipment is associated with any trx, if so, throw an exception using the given error message formatstring
		/// </summary>
		/// <param name="security"></param>
		/// <param name="equipmentGuid"></param>
		/// <param name="errorMessageFormatString"></param>
		private void CheckAssociatedTrx(SecurityClass security, Guid equipmentGuid, string errorMessageFormatString)
		{
			var systemSettingBusObj = new SystemSettingsClass();

			SystemSettingClass systemSettingDataObj = systemSettingBusObj.Get(security);

			if (systemSettingDataObj.ProhibitUpdatingLinkedEquipment)
			{
				int count = this.NumOfAssociatedTrx(security, equipmentGuid);

				if (count > 0)
				{
					string errorMsg = string.Format(errorMessageFormatString, count);
					throw new Exception(errorMsg);
				}
			}
		}

		/// <summary>
		/// Find out how many transactions are associated with the given equipment
		/// </summary>
		/// <param name="security"></param>
		/// <param name="equipmentGuid"></param>
		/// <returns></returns>
		private int NumOfAssociatedTrx(SecurityClass security, Guid equipmentGuid)
		{
			int count = 0;
			try
			{
				DataSet result = this.consolidatedDA.GetDataSet(EquipmentClass.CountAssociatedTrxSQL(equipmentGuid), security);

				if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
				{
					count = (int) result.Tables[0].Rows[0][0];
				}

			}
			catch (Exception error)
			{
				throw (new Exception(error.Message));
			}
			return count;
		}

		private void CheckTestResults(SecurityClass security, Guid equipmentGuid, string errorMessageFormatString)
        {

            TestSetEquipmentResultsClass testResults = new TestSetEquipmentResultsClass();

            var results = testResults.EnumerateByEquipmentGuid(security, equipmentGuid);
            
            int count = results.Count;

            if (count > 0)
            {
                string errorMsg = string.Format(errorMessageFormatString, count);
                throw new Exception(errorMsg);
            }
        }


        private void CheckMaintenanceLog(SecurityClass security, Guid equipmentGuid, string errorMessageFormatString)
        {

            EquipmentMaintenanceLogsClass testResults = new EquipmentMaintenanceLogsClass();

            DataSet ds = testResults.GetDataSet(security, true, null, DateTimeOffset.Now, DateTimeOffset.Now, equipmentGuid);
            DataTable table = ds.Tables[0];

            int count = table.Rows.Count;

            if (count > 0)
            {
                string errorMsg = string.Format(errorMessageFormatString, count);
                throw new Exception(errorMsg);
            }
        }
        #endregion
    }
}