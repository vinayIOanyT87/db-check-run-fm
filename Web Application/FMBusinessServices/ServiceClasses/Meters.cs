// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Meters.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Allows the user interface to interact with meter records in the system.
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
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

    /// <summary>
    /// Allows the user interface to interact with meter records in the system.
    /// </summary>
    [SecuritySafeCriticalAttribute]
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class MetersClass : IMeters
    {
        /// <summary>
        /// Allows database access.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// Construct a meters object
        /// </summary>
        public MetersClass()
        {
        }

        /// <summary>
        /// Add a record to the database to indicate a relationship between a tank and a meter
        /// </summary>
        /// <param name="security">Security information</param>
        /// <param name="meter">The meter to add a tank relationship for</param>
        /// <param name="tankGuid">The tank to relate the meter to</param>
        public void AddTankMap(SecurityClass security, MeterClass meter, Guid tankGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_METERS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            if (meter == null)
            {
                throw new ArgumentNullException("meter");
            }

            if (tankGuid == Guid.Empty)
            {
                throw new ApplicationException("Tank guid is required");
            }

            meter.CreatedDate = DateTimeOffset.Now;
            meter.CreatedBy = security.UserID;
            meter.UpdatedDate = meter.CreatedDate;
            meter.UpdatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                meter.InsertTankMapSql(cmd, tankGuid);
                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
		/// Add a record to the database to indicate a relationship between equipment and a meter
		/// </summary>
		/// <param name="security">Security information</param>
		/// <param name="meter">The meter to add an equipment relationship for</param>
		/// <param name="equipmentGuid">The MasterRecordGuid of the equipment to relate the meter to.</param>
		public void AddEquipmentMap(SecurityClass security, MeterClass meter, Guid equipmentGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_METERS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            if (meter == null)
            {
                throw new ArgumentNullException("meter");
            }

            if (equipmentGuid == Guid.Empty)
            {
                throw new ApplicationException("Equipment guid is required");
            }

            meter.CreatedDate = DateTimeOffset.Now;
            meter.CreatedBy = security.UserID;
            meter.UpdatedDate = meter.CreatedDate;
            meter.UpdatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                meter.InsertEquipmentMapSQL(cmd, equipmentGuid);
                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Add a new meter to the database
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="meter">The new meter to add</param>
        /// <returns>The primary key of the meter record that was entered into the database</returns>
        public Guid Add(SecurityClass security, MeterClass meter)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_METERS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            if (meter == null)
            {
                throw new ArgumentNullException("meter");
            }

            Validate(meter);

            if (GetIdentityGuid(security, meter.ID) != Guid.Empty)
            {
                throw new Exception("A meter with the same ID exists");
            }

            meter.SiteGuid = security.SiteGuid;
            meter.CreatedDate = DateTimeOffset.Now;
            meter.CreatedBy = security.UserID;
            meter.UpdatedDate = meter.CreatedDate;
            meter.UpdatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                string identityGuidParameterName = meter.InsertSQL(cmd);
                ConsolidatedDA.ExecuteQuery(security, cmd);
                meter.IdentityGuid = (Guid)cmd.Parameters[identityGuidParameterName].Value;
            }

            return meter.IdentityGuid;
        }

        /// <summary>
        /// Update a meter record in the database.
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="meter">The meter to modify</param>
        public void Modify(SecurityClass security, MeterClass meter)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (meter == null)
            {
                throw new ArgumentNullException("meter");
            }

            if (!security.HasRight(RIGHT.MODIFY_METERS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            Validate(meter);

            Guid existingIdentityGuid = GetIdentityGuid(security, meter.ID);

            if (existingIdentityGuid != Guid.Empty && existingIdentityGuid != meter.IdentityGuid)
            {
                throw (new Exception("A meter with the same ID exists"));
            }

            if (meter.IdentityGuid == Guid.Empty)
            {
                throw (new Exception("The meter was not found"));
            }

            meter.UpdatedDate = DateTimeOffset.Now;
            meter.UpdatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                meter.UpdateSQL(cmd);
                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Is the meter assigned to any product map record?
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="meterGuid">The primary key of the meter record to check</param>
        /// <returns>True if the meter is assigned to any product map record. False otherwise</returns>
        public bool AssignedToAnyProductMap(SecurityClass security, Guid meterGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (meterGuid == Guid.Empty)
            {
                throw new ApplicationException("Meter guid is required");
            }

            //Meters can be assigned to multiple types of product maps,
            //and each type is stored in a different database table.
            foreach (PRODUCT_MAP_TYPE mapType in ProductMapClass.GetMeterTypes())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    ProductMapClass productMap = new ProductMapClass();
                    productMap.Type = mapType;
                    productMap.EnumerateByMeterGuidSQL(cmd, meterGuid, security.SiteGuid);

                    DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                    DataTable table = set.Tables[0];
                    if (table.Rows.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Is the meter assigned to more than one product map record?
        /// </summary>
        /// <param name="security">Security information</param>
        /// <param name="meterGuid">The primary key of the meter record to check</param>
        /// <returns>True if the meter is assigned to more than one product map record</returns>
        public bool AssignedToMoreThanOneProductMap(SecurityClass security, Guid meterGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (meterGuid == Guid.Empty)
            {
                throw new ApplicationException("Meter guid is required");
            }

            int numberOfAssignments = 0;

            //Meters can be assigned to multiple types of product maps,
            //and each type is stored in a different database table.
            foreach (PRODUCT_MAP_TYPE mapType in ProductMapClass.GetMeterTypes())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    ProductMapClass productMap = new ProductMapClass();
                    productMap.Type = mapType;
                    productMap.EnumerateByMeterGuidSQL(cmd, meterGuid, security.SiteGuid);

                    DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                    DataTable table = set.Tables[0];
                    numberOfAssignments += table.Rows.Count;
                }
            }

            if (numberOfAssignments > 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get all of the meters that belong to a specific tank
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="tankGuid">The primary key of the tank to get meters for</param>
        /// <returns>A collection of meters assigned to a tank. May be empty if no meters are assigned to the tank</returns>
        public List<MeterClass> EnumerateByTank(SecurityClass security, Guid tankGuid)
        {
            List<MeterClass> meterCollection = new List<MeterClass>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (tankGuid == Guid.Empty)
            {
                throw new ApplicationException("Tank guid is required");
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                MeterClass meter = new MeterClass();
                meter.EnumerateByTank(cmd, tankGuid);

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    meter = new MeterClass();
                    meter.Load(set);
                    meterCollection.Add(meter);
                    table.Rows.RemoveAt(0);
                }
            }

            return meterCollection;
        }

        /// <summary>
		/// Get all of the meters that belong to a specific piece of equipment
		/// </summary>
		/// <param name="security">Security Information</param>
		/// <param name="equipmentGuid">The primary key of the equipment to get meters for</param>
		/// <returns>A collection of meters assigned to the equipment. May be empty if no meters are assigned to the equipment</returns>
		public List<MeterClass> EnumerateByEquipment(SecurityClass security, Guid equipmentGuid)
        {
            List<MeterClass> meterCollection = new List<MeterClass>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (equipmentGuid == Guid.Empty)
            {
                throw new ApplicationException("Equipment guid is required");
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                MeterClass meter = new MeterClass();
                meter.EnumerateByEquipment(cmd, equipmentGuid, security.SiteGuid);

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    meter = new MeterClass();
                    meter.Load(set);
                    meterCollection.Add(meter);
                    table.Rows.RemoveAt(0);
                }
            }

            return meterCollection;
        }

        /// <summary>
        /// Get the primary key of the meter record which matches the specified ID
        /// </summary>
        /// <param name="security">Security information</param>
        /// <param name="id">the ID to search for</param>
        /// <returns>The primary key of the meter record which matches the specified ID, or the empty guid if 
        /// a matching meter was not found</returns>
        public Guid GetIdentityGuid(SecurityClass security, string id)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            MeterClass meter = new MeterClass();
            meter.ID = id;
            meter.SiteGuid = security.SiteGuid;

            using (SqlCommand cmd = new SqlCommand())
            {
                meter.SelectByIDSQL(cmd);
                meter.Load(ConsolidatedDA.GetDataSet(cmd, security));
            }

            return meter.IdentityGuid;
        }

        /// <summary>
        /// Get the primary key of the meter record which matches the specified ID and is assigned to the specified load arm. 
        /// Matches component meters only.
        /// </summary>
        /// <param name="security">Security information</param>
        /// <param name="id">the ID to search for</param>
        /// <param name="loadArmGuid">the load arm to look for meters in</param>
        /// <returns>The primary key of the meter record which matches the specified ID, or the empty guid if 
        /// a matching meter was not found</returns>
        public Guid GetIdentityGuidForLoadArmComponentMeter(SecurityClass security, string id, Guid loadArmGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            MeterClass meter = new MeterClass();
            meter.ID = id;
            meter.SiteGuid = security.SiteGuid;

            using (SqlCommand cmd = new SqlCommand())
            {
                meter.SelectComponentMeterByIDAndLoadArmGuidSQL(cmd, loadArmGuid);
                meter.Load(ConsolidatedDA.GetDataSet(cmd, security));
            }

            return meter.IdentityGuid;
        }

        /// <summary>
        /// Retrieve a meter record by the primary key
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="identityGuid">The primary key of the meter record to search for</param>
        /// <returns>The meter matching the value provided, or null if one was not found</returns>
        public MeterClass Get(SecurityClass security, Guid identityGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            MeterClass meter = new MeterClass();
            meter.IdentityGuid = identityGuid;

            if (identityGuid != Guid.Empty)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    meter.SelectSQL(cmd);
                    if (meter.Load(ConsolidatedDA.GetDataSet(cmd, security)))
                    {
                        return meter;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Delete a meter from the system
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="meterGuid">the meter to delete</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid meterGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_METERS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            MeterClass meter = Get(security, meterGuid);

            if (meter.IdentityGuid != Guid.Empty)
            {
                // Delete any of the records which map a tank to this meter
                using (SqlCommand cmd = new SqlCommand())
                {
                    meter.DeleteTankMapForMeterSQL(cmd);
                    ConsolidatedDA.ExecuteQuery(security, cmd);
                }

                // Delete any of the records which map equipment to this meter
                using (SqlCommand cmd = new SqlCommand())
                {
                    meter.DeleteEquipmentMapForMeterSQL(cmd);
                    ConsolidatedDA.ExecuteQuery(security, cmd);
                }

                // delete the meter
                using (SqlCommand cmd = new SqlCommand())
                {
                    meter.PurgeSQL(cmd);
                    ConsolidatedDA.ExecuteQuery(security, cmd);
                }
            }
        }


        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public bool HasForeignKeyReference(SecurityClass security, Guid meterGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            try
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "usp_GetMeterForeignKeyReferences";
                    cmd.Parameters.AddWithValue("@MeterGuid", meterGuid);

                    var dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);

                    return dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Make sure that the meter is valid.
        /// This may partially duplicate some logic in the user interface, but
        /// we check anyway since we cannot guarantee that the methods in this class
        /// were called from the UI.
        /// 
        /// This method will throw if any errors are detected
        /// </summary>
        /// <param name="meter">a meter to check</param>
        private void Validate(MeterClass meter)
        {
            if (meter.NumberOfDigits <= 0)
            {
                throw new Exception("Meter number of digits must be greater than zero");
            }

            if (string.IsNullOrEmpty(meter.ID))
            {
                throw new Exception("Meter ID is required");
            }
        }

        /// <summary>
        /// Determine if the meter is in use in an accounting transaction. If it is, it may not be deleted 
        /// because it will violate a foreign key constraint.
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="meterGuid">The primary key of a meter to check</param>
        /// <returns>True if the meter is used in a transaction line item or sub line item. False otherwise</returns>
        private bool IsMeterUsedInTransaction(SecurityClass security, Guid meterGuid)
        {
            int transactionCount = 0;

            using (SqlCommand cmd = new SqlCommand())
            {
                //this stored procedure takes a long time to run
                //may need to speed up the query eventually
                cmd.CommandTimeout = 200;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionsSelectCountByMeterGuid";
                cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@MeterGuid"].Value = meterGuid;

                transactionCount = (int)ConsolidatedDA.GetDataSet(cmd, security).Tables[0].Rows[0][0];
            }

            if (transactionCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// List all of the meters in the system which belong to the current site
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <returns>A collection of meters containing all of those assigned to the current site</returns>
        public List<MeterClass> Enumerate(SecurityClass security)
        {
            return Enumerate(security, string.Empty, Guid.Empty);
        }

        /// <summary>
        /// List all of the meters in the system which belong the the current site, and filter by ID
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="meterIDFilterValue">the ID value to filter on</param>
        /// <returns>A collection of meters containing all of those assigned to the current site
        /// and where the meter ID partially matches the provided parameter</returns>
        public List<MeterClass> EnumerateAndFilter(SecurityClass security, string meterIDFilterValue)
        {
            return Enumerate(security, meterIDFilterValue, Guid.Empty);
        }

        /// <summary>
        /// List all of the meters in the system which belong the the current site and a specified asset (tank, equipment, load arm)
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="assetGuid">an asset to filter the results on</param>
        /// <returns>A collection of meters containing all of those assigned to the current site and which belong to the specified asset</returns>
        public List<MeterClass> EnumerateByAssetGuid(SecurityClass security, Guid assetGuid)
        {
            return Enumerate(security, string.Empty, assetGuid);
        }

        /// <summary>
        /// List all of the meters in the system which belong the the current site and a specified asset (tank, equipment, load arm), 
        /// and filter by the meter ID
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="assetGuid">an asset to filter the results on</param>
        /// <param name="meterIDFilterValue">the meter ID value to filter on</param>
        /// <returns>A collection of meters containing all of those assigned to the current site and which belong to the specified asset
        /// and match the provided ID value</returns>
        public List<MeterClass> EnumerateByAssetGuidAndFilter(SecurityClass security, Guid assetGuid, string meterIDFilterValue)
        {
            return Enumerate(security, meterIDFilterValue, assetGuid);
        }

        /// <summary>
        /// A private enumerate method which allows us to avoid duplicating code for every enumerate method
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="meterIDFilterValue">he meter ID value to filter on, or string.Empty if we aren't filtering on ID</param>
        /// <param name="assetGuid">an asset to filter the results on, or Guid.Empty if we aren't filtering on asset</param>
        /// <returns>A collection of meters matching the specified search criteria</returns>
        private List<MeterClass> Enumerate(SecurityClass security, string meterIDFilterValue, Guid assetGuid)
        {
            List<MeterClass> meterCollection = new List<MeterClass>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                MeterClass meter = new MeterClass();

                if (assetGuid != Guid.Empty && !string.IsNullOrEmpty(meterIDFilterValue))
                {
                    meter.EnumerateByAssetGuidAndFilter(cmd, security.SiteGuid, assetGuid, meterIDFilterValue);
                }
                else if (assetGuid != Guid.Empty)
                {
                    meter.EnumerateByAssetGuid(cmd, security.SiteGuid, assetGuid);
                }
                else if (!string.IsNullOrEmpty(meterIDFilterValue))
                {
                    meter.EnumerateAndFilter(cmd, security.SiteGuid, meterIDFilterValue);
                }
                else
                {
                    meter.Enumerate(cmd, security.SiteGuid);
                }

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    meter = new MeterClass();
                    meter.Load(set);
                    meterCollection.Add(meter);
                    table.Rows.RemoveAt(0);
                }
            }

            return meterCollection;
        }

        /// <summary>
        /// List all of equipment, tanks, and load arms in the system which belong to the current site
        /// and have a meter assigned to them
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <returns>A collection of meter assets (equipment, tanks, and load arms)</returns>
        public List<MeterAssetClass> EnumerateAssets(SecurityClass security)
        {
            return EnumerateAssetsAndFilter(security, string.Empty);
        }

        /// <summary>
        /// List all of equipment, tanks, and load arms in the system which belong to the current site
        /// and have a meter assigned to them and filter by the asset ID
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="assetIDFilterValue">The asset ID to filter on (partial match)</param>
        /// <returns>A collection of meters containing all of those assigned to the current site</returns>
        public List<MeterAssetClass> EnumerateAssetsAndFilter(SecurityClass security, string assetIDFilterValue)
        {
            return EnumerateAssets(security, assetIDFilterValue);
        }

        /// <summary>
        /// A private enumerate method which allows us to avoid duplicating code for every enumerate method
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="assetIDFilterValue">The asset ID to filter on (partial match). String.Empty if no filtering is desired</param>
        /// <returns>A collection of meters containing all of those assigned to the current site</returns>
        private List<MeterAssetClass> EnumerateAssets(SecurityClass security, string assetIDFilterValue)
        {
            List<MeterAssetClass> meterAssetCollection = new List<MeterAssetClass>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                MeterAssetClass meterAsset = new MeterAssetClass();
                meterAsset.SiteGuid = security.SiteGuid;

                if (!string.IsNullOrEmpty(assetIDFilterValue))
                {
                    meterAsset.EnumerateAndFilter(cmd, assetIDFilterValue);
                }
                else
                {
                    meterAsset.Enumerate(cmd);
                }

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    meterAsset = new MeterAssetClass();
                    meterAsset.Load(set);
                    meterAssetCollection.Add(meterAsset);
                    table.Rows.RemoveAt(0);
                }
            }

            return meterAssetCollection;
        }

        /// <summary>
        /// Returns a list of Meter Ids for meters attached to a list of asset Guids passed to this method.
        /// </summary>
        /// <param name="security">Security Information</param>
        /// <param name="assets">List of Equipments to search for attached meters</param>
        /// <returns>A list of meter Ids for meters attached to the list of equipments passed in to the method</returns>
        public List<string> GetMeterIdsByAssetGuids(SecurityClass security, List<EquipmentClass> assets)
        {
            //create the datatable
            DataTable dt = new DataTable();
            dt.Columns.Add("Guid", typeof(Guid));
            foreach (EquipmentClass ec in assets)
            {
                dt.Rows.Add(ec.IdentityGuid);
            }

            List<string> meterIds = new List<string>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GetMeterIdsByAssetGuids";
                cmd.Parameters.Add("@AssetGuids", SqlDbType.Structured);
                cmd.Parameters["@AssetGuids"].Value = dt;

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                DataTable table = set.Tables[0];

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    meterIds.Add(table.Rows[i].ItemArray[0].ToString());
                }
            }
            return meterIds;
        }
    }
}
