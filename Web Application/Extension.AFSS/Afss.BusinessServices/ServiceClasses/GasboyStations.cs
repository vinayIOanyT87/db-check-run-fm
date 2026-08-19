// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements operations to support database operations for External Stations
// like adding, modifying, or deleting a record.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.GasboyRICE.BusinessServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.ServiceModel;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.ServiceClasses;

    using FuelsManager.GasboyRICE.BusinessObjects.BusinessInterfaces;
    using FuelsManager.GasboyRICE.BusinessObjects.DataObjects;

    /// <summary>
    /// Implements operations to support database operations for External Stations
    /// like adding, modifying, or deleting a record.
    /// </summary>
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class GasboyStations : IGasboyStation
    {
        /// <summary>
        /// Allows database access.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        #region External Station Methods

        /// <summary>
        /// Get stations configured for the site, filtering by the filter text if it was provided
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <returns>Stations configured for the site, filtered by the filter text if it was provided</returns>
        public List<GasboyStation> Enumerate(SecurityClass security)
        {          
            return this.EnumerateAndFilter(security, string.Empty);
        }

        /// <summary>
        /// Get all External Stations assigned or owned by the current site that partially match the ID provided
        /// </summary>
        /// <param name="security">Contains security information, like the site we're currently accessing to retrieve External Stations for</param>
        /// <param name="searchFilter">The ID to search for matches on</param>
        /// <returns>All External Stations assigned or owned by the current site that partially match the provided ID</returns>
        public List<GasboyStation> EnumerateAndFilter(SecurityClass security, string searchFilter)
        {
            List<GasboyStation> externalStations = new List<GasboyStation>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION) && !security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            GasboyStation externalStation = new GasboyStation
            {
                SiteGuid = security.SiteGuid,
                ID = searchFilter
            };

            using (SqlCommand cmd = new SqlCommand())
            {
                if (!string.IsNullOrEmpty(searchFilter))
                {
                    externalStation.EnumerateAndFilterSQL(cmd);
                }
                else
                {
                    externalStation.EnumerateSQL(cmd);
                }

                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count <= 0)
                {
                    return externalStations;
                }

                DataTable table = set.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    externalStation = new GasboyStation
                                          { 
                                              IdentityGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty),
                                              SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
                                              ID = DataObject.getValue(row["ID"], string.Empty),
                                              DoDAAC = DataObject.getValue(row["DoDAAC"], string.Empty),
                                              Status = DataObject.getValue(row["LookupExternalStationStatusIndex"], ExternalStationStatus.Inactive),
                                              LastConnectionAttempt = DataObject.getValue<DateTimeOffset?>(row["LastConnectionAttempt"], null),
                                              LastSuccessfulConnection = DataObject.getValue<DateTimeOffset?>(row["LastSuccessfulConnection"], null),
                                              LastTransactionID = DataObject.getValue<long?>(row["LastTransactionID"], null)
                                          };

                    externalStations.Add(externalStation);
                }
            }

            return externalStations;
        }

        /// <summary>
        /// Retrieve the External Station identified by the provided guid
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies the External Station to retrieve</param>
        /// <returns>The External Station identified by the provided guid</returns>
        public GasboyStation Get(SecurityClass security, Guid externalStationGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION) && !security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationGuid");
            }

            GasboyStation externalStation = new GasboyStation { IdentityGuid = externalStationGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStation.SelectSQL(cmd);

                if (!externalStation.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return null;
                }
            }

            externalStation.ProductMappings = this.EnumerateExternalStationProductMappings(security, externalStationGuid);

            return externalStation;
        }

        /// <summary>
        /// Retrieve the External Station identified by the provided id
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationID">Identifies the External Station to retrieve</param>
        /// <returns>The External Station identified by the provided id</returns>
        public GasboyStation GetByID(SecurityClass security, string externalStationID)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION) && !security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (string.IsNullOrEmpty(externalStationID))
            {
                throw new ArgumentException("externalStationID");
            }

            GasboyStation externalStation = new GasboyStation { ID = externalStationID, SiteGuid = security.SiteGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStation.SelectByIdSQL(cmd);
                if (!externalStation.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return null;
                }
            }

            externalStation.ProductMappings = this.EnumerateExternalStationProductMappings(security, externalStation.IdentityGuid);

            return externalStation.IdentityGuid == Guid.Empty ? null : externalStation;
        }

        /// <summary>
        /// Get the Identity Guid (Primary Key) of the External Station record identified by the provided ID
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationID">Identifies the External Station record to retrieve.</param>
        /// <returns>The Identity Guid (Primary Key) External Station record identified by the provided ID. Will return an empty guid if no match is found</returns>
        public Guid GetIdentityGuid(SecurityClass security, string externalStationID)
        {
            GasboyStation matchingExternalStation = this.GetByID(security, externalStationID);
            return matchingExternalStation == null ? Guid.Empty : matchingExternalStation.IdentityGuid;
        }

        /// <summary>
        /// Add a new External Station record to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStation">The external station to add</param>
        /// <returns>The identity guid of the new External Station record</returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, GasboyStation externalStation)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStation == null)
            {
                throw new ArgumentNullException("externalStation");
            }

            this.Validate(externalStation);

            // Make sure that there is not already a External Station assigned to or owned by this site
            // with the same ID 
            if (this.GetIdentityGuid(security, externalStation.ID) != Guid.Empty)
            {
                throw new Exception("An External Station with the same ID exists");
            }

            externalStation.IdentityGuid = Guid.NewGuid();
            externalStation.SiteGuid = security.SiteGuid;
            externalStation.CreatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStation.InsertSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd, ConsolidatedDAClass.Uniquifier);

                // Create a record mapping the external station to the current site
                EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
                EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(externalStation);
                entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);
            }

            this.UpdateProductMappings(security, null, externalStation);

            return externalStation.IdentityGuid;
        }

        /// <summary>
        /// Modify the provided External Station in the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStation">The external station to modify</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, GasboyStation externalStation)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStation == null)
            {
                throw new ArgumentNullException("externalStation");
            }

            this.Validate(externalStation);

            // Modify the security objects's site guid in case the External Station's site is changing from an entity ownership change.
            // We want to perform the check in the site the external station will be owned by, not the site it's currently owned by.
            Guid siteGuid = security.SiteGuid;
            security.SiteGuid = externalStation.SiteGuid;

            Guid existingExternalStationGuid = this.GetIdentityGuid(security, externalStation.ID);

            // restore the siteguid to its original value
            security.SiteGuid = siteGuid;

            if (existingExternalStationGuid != Guid.Empty && existingExternalStationGuid != externalStation.IdentityGuid)
            {
                throw new Exception("An External Station with the same ID exists");
            }

            GasboyStation oldExternalStation = this.Get(security, externalStation.IdentityGuid);

            if (oldExternalStation == null || oldExternalStation.IdentityGuid == Guid.Empty)
            {
                throw new Exception("The External Station was not found");
            }

            // If the password is the dummy masked password text, 
            // it has not been modified by the user and the existing value should be preserved
            if (externalStation.Password == GasboyStation.PasswordDefaultValue)
            {
                externalStation.Password = oldExternalStation.Password;
            }

            externalStation.UpdatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStation.UpdateSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }

            this.UpdateProductMappings(security, oldExternalStation, externalStation);

            if (externalStation.SiteGuid != oldExternalStation.SiteGuid)
            {
                EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
                EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
                    security, externalStation.EntityType, externalStation.IdentityGuid);

                // If the site changed,
                // Purge any records mapping the External Station to a site
                foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
                {
                    entityToSiteMap.ID = externalStation.ID;
                    entityToSiteMaps.Purge(security, entityToSiteMap);
                }

                // Create a new record mapping the External Station to the new site
                EntityToSiteMapClass newEntityToSiteMap = new EntityToSiteMapClass(externalStation);
                entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
            }
        }

        /// <summary>
        /// Delete the external station identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies the external station to delete</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid externalStationGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            GasboyStation externalStation = this.Get(security, externalStationGuid);

            if (externalStation.IdentityGuid != Guid.Empty)
            {
                // Delete any records mapping the External Station to a site
                EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
                EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
                    security, externalStation.EntityType, externalStation.IdentityGuid);

                foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
                {
                    entityToSiteMap.ID = externalStation.ID;
                    entityToSiteMaps.Purge(security, entityToSiteMap);
                }

                // Delete the External Station and any associated records (i.e. product mappings)
                using (SqlCommand cmd = new SqlCommand())
                {
                    externalStation.PurgeSQL(cmd);
                    this.ConsolidatedDA.ExecuteQuery(security, cmd);
                }
            }
            else
            {
                throw new Exception("The External Station to delete was not found");
            }
        }

        #endregion

        #region External Station Log Methods

        /// <summary>
        /// Retrieve external station logs for the site matching the provided search parameters
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationGuid">The station to get logs for, or Guid.Empty for all stations</param>
        /// <param name="beginDate">The beginning date to get logs for</param>
        /// <param name="endDate">The end date to get logs for</param>
        /// <param name="logType">The type of logs to get, or null for all types</param>
        /// <returns>External station logs for the site matching the provided search parameters</returns>
        public List<GasboyStationLog> EnumerateLogs(
            SecurityClass security,
            Guid externalStationGuid,
            DateTimeOffset beginDate,
            DateTimeOffset endDate,
            ExternalStationLogType? logType)
        {
            List<GasboyStationLog> externalStationLogs = new List<GasboyStationLog>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION) && !security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            GasboyStationLog externalStationLog = new GasboyStationLog
            {
                SiteGuid = security.SiteGuid
            };

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStationLog.EnumerateSQL(cmd, security.SiteGuid, externalStationGuid, beginDate, endDate, logType);

                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count <= 0)
                {
                    return externalStationLogs;
                }

                DataTable table = set.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    externalStationLog = new GasboyStationLog
                                             {
                                                 IdentityGuid = DataObject.getValue(row["ExternalStationLogGuid"], Guid.Empty),
                                                 SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
                                                 ID = DataObject.getValue(row["LogText"], string.Empty),
                                                 ExternalStationGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty),
                                                 ExternalStationID = DataObject.getValue(row["ExternalStationID"], string.Empty),
                                                 LogType = DataObject.getValue(row["LookupExternalStationLogTypeIndex"], ExternalStationLogType.ConnectionFailure),
                                                 CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now)
                                             };

                    externalStationLogs.Add(externalStationLog);
                }
            }

            return externalStationLogs;
        }

        /// <summary>
        /// Get the external station log identified by the provided guid
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationLogGuid">Identifies the external station log to retrieve</param>
        /// <returns>The external station log identified by the provided guid</returns>
        public GasboyStationLog GetLog(SecurityClass security, Guid externalStationLogGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION) && !security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationLogGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationLogGuid");
            }

            GasboyStationLog externalStationLog = new GasboyStationLog { IdentityGuid = externalStationLogGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStationLog.SelectSQL(cmd);

                if (!externalStationLog.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return null;
                }
            }

            return externalStationLog;
        }

        #endregion

        /// <summary>
        /// Check to make sure the External Station is valid
        /// </summary>
        /// <param name="externalStation">The External Station to check</param>
        private void Validate(GasboyStation externalStation)
        {
            if (string.IsNullOrEmpty(externalStation.ID))
            {
                throw new Exception("ID must be provided for an External Station");
            }
        }

        #region External Station Product Mapping Methods

        /// <summary>
        /// Enumerate product mappings for the provided External Station
        /// </summary>
        /// <param name="security">Contains Security information</param>
        /// <param name="externalStationGuid">Identifies the external station to retrieve product mappings for</param>
        /// <returns>Product mappings for the provided External Station</returns>
        private List<ExternalStationProductMapping> EnumerateExternalStationProductMappings(SecurityClass security, Guid externalStationGuid)
        {
            List<ExternalStationProductMapping> lineItems = new List<ExternalStationProductMapping>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION) && !security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationGuid");
            }

            ExternalStationProductMapping externalStationProductMapping = new ExternalStationProductMapping() { ExternalStationGuid = externalStationGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStationProductMapping.EnumerateSQL(cmd, externalStationGuid);
                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count <= 0)
                {
                    return lineItems;
                }

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    externalStationProductMapping = new ExternalStationProductMapping();
                    externalStationProductMapping.Load(set);
                    lineItems.Add(externalStationProductMapping);
                    table.Rows.RemoveAt(0);
                }
            }

            return lineItems;
        }

        /// <summary>
        /// Retrieve an existing External Station Product Mapping from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMappingGuid">Identifies the External Station Product Mapping to retrieve</param>
        /// <returns>The External Station Product Mapping identified by the provided guid, or null if it was not found</returns>
        private ExternalStationProductMapping GetProductMapping(SecurityClass security, Guid externalStationProductMappingGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION) && !security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationProductMappingGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationProductMappingGuid");
            }

            ExternalStationProductMapping externalStationProductMapping = new ExternalStationProductMapping() { IdentityGuid = externalStationProductMappingGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStationProductMapping.SelectSQL(cmd);

                if (externalStationProductMapping.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return externalStationProductMapping;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve an existing External Station Product Mapping from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies the External Station Product Mapping to retrieve</param>
        /// <param name="id">The external station product ID to retrieve mappings for</param>
        /// <returns>The External Station Product Mapping identified by the provided guid, or null if it was not found</returns>
        private ExternalStationProductMapping GetProductMappingByExternalStationAndProductID(SecurityClass security, Guid externalStationGuid, string id)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION) && !security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationGuid");
            }

            ExternalStationProductMapping externalStationProductMapping = new ExternalStationProductMapping() { ExternalStationGuid = externalStationGuid, ID = id };

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStationProductMapping.SelectByExternalStationAndProductIDSQL(cmd);

                if (externalStationProductMapping.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return externalStationProductMapping;
                }
            }

            return null;
        }

        /// <summary>
        /// Determine which product mappings need to be added, modified, or deleted by comparing
        /// the product mappings of the existing version of the external station and the new version of the external station
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="oldExternalStation">The existing version of the external station in the database</param>
        /// <param name="newExternalStation">The new version of the external station</param>
        private void UpdateProductMappings(SecurityClass security, GasboyStation oldExternalStation, GasboyStation newExternalStation)
        {
            // If the new external station parameter was null, that means we're deleting the external station.
            // If it's not null, that means we may have to add, update, or delete product mappings.
            if (newExternalStation != null)
            {
                foreach (ExternalStationProductMapping externalStationProductMapping in newExternalStation.ProductMappings)
                {
                    // If the product mapping belonging to the new external station has no identity guid, 
                    // it is new and needs to be added to the database
                    if (externalStationProductMapping.IdentityGuid == Guid.Empty)
                    {
                        externalStationProductMapping.ExternalStationGuid = newExternalStation.IdentityGuid;
                        this.AddProductMapping(security, externalStationProductMapping);
                    }
                    else if (oldExternalStation != null)
                    {
                        // If we can find a product mapping with the same identity guid in the old external station's set of product mappings, 
                        // the product mapping needs to be updated. After the update we remove it from the old external station so we know not to delete it later
                        foreach (ExternalStationProductMapping oldExternalStationProductMapping in oldExternalStation.ProductMappings)
                        {
                            if (oldExternalStationProductMapping.IdentityGuid == externalStationProductMapping.IdentityGuid)
                            {
                                this.ModifyProductMapping(security, externalStationProductMapping);
                                oldExternalStation.ProductMappings.Remove(oldExternalStationProductMapping);
                                break;
                            }
                        }
                    }
                }
            }

            // Delete any product mappings that are still present on the old external station object, 
            // because they weren't found in the new external station.
            if (oldExternalStation != null)
            {
                foreach (ExternalStationProductMapping oldExternalStationProductMapping in oldExternalStation.ProductMappings)
                {
                    this.PurgeProductMapping(security, oldExternalStationProductMapping.IdentityGuid);
                }
            }
        }

        /// <summary>
        /// Add an External Station Product Mapping to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMapping">The External Station Product Mapping to add</param>
        /// <returns>The identity guid (Primary Key) of the new External Station Product Mapping record</returns>
        private Guid AddProductMapping(SecurityClass security, ExternalStationProductMapping externalStationProductMapping)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationProductMapping == null)
            {
                throw new ArgumentNullException("externalStationProductMapping");
            }

            this.ValidateProductMapping(security, externalStationProductMapping);

            externalStationProductMapping.IdentityGuid = Guid.NewGuid();
            externalStationProductMapping.CreatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStationProductMapping.InsertSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }

            return externalStationProductMapping.IdentityGuid;
        }

        /// <summary>
        /// Update an existing External Station Product Mapping in the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMapping">The External Station Product Mapping to update</param>
        private void ModifyProductMapping(SecurityClass security, ExternalStationProductMapping externalStationProductMapping)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationProductMapping == null)
            {
                throw new ArgumentNullException("externalStationProductMapping");
            }

            this.ValidateProductMapping(security, externalStationProductMapping);

            externalStationProductMapping.UpdatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                externalStationProductMapping.UpdateSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Remove an existing External Station Product Mapping from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMappingGuid">Identifies the External Station Product Mapping to delete</param>
        private void PurgeProductMapping(SecurityClass security, Guid externalStationProductMappingGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            ExternalStationProductMapping externalStationProductMapping = this.GetProductMapping(security, externalStationProductMappingGuid);

            if (externalStationProductMapping.IdentityGuid != Guid.Empty)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    externalStationProductMapping.PurgeSQL(cmd);
                    this.ConsolidatedDA.ExecuteQuery(security, cmd);
                }
            }
        }


        /// <summary>
        /// Make sure the data in the product mapping passes validation rules.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMapping">The line item to check</param>
        private void ValidateProductMapping(SecurityClass security, ExternalStationProductMapping externalStationProductMapping)
        {
            ExternalStationProductMapping matchingProductMapping = this.GetProductMappingByExternalStationAndProductID(
                security,
                externalStationProductMapping.ExternalStationGuid,
                externalStationProductMapping.ID);

            if (matchingProductMapping != null
                && ((externalStationProductMapping.IdentityGuid != Guid.Empty && matchingProductMapping.IdentityGuid != externalStationProductMapping.IdentityGuid)
                    || (externalStationProductMapping.IdentityGuid == Guid.Empty && matchingProductMapping.IdentityGuid != Guid.Empty)))
            {
                throw new Exception("Duplicate External Station Product Mapping");
            }
        }

        #endregion
    }
}