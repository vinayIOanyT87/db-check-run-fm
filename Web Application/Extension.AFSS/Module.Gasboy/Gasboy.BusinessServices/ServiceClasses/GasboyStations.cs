// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements operations to support database operations for External Stations
// like adding, modifying, or deleting a record.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.ServiceModel;
    using System.Text;
    using System.Transactions;
    using System.Xml;
    using System.Xml.Serialization;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.ServiceClasses;

    using FuelsManager.Afss.BusinessObjects.Constants;
    using FuelsManager.Afss.BusinessObjects.DataObjects;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
    using FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository;

    /// <summary>
    /// Implements operations to support database operations for External Stations
    /// like adding, modifying, or deleting a record.
    /// </summary>
    [ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
    public class GasboyStations : IGasboyStations
    {
        /// <summary>
        /// Allows database access.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// Used to transform raw external station transaction data into an object
        /// </summary>
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(GasboyStationTransaction));

        #region External Station Methods

        /// <summary>
        /// Get stations configured for the site, filtering by the filter text if it was provided
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <returns>Stations configured for the site, filtered by the filter text if it was provided</returns>
        public List<GasboyStation> Enumerate(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            List<GasboyStation> stations;

            using (var dbi = new GasboyStationDBI(security.UserID))
            {
                stations = dbi.GetList(security, security.SiteGuid, null);
            }

				foreach (GasboyStation station in stations)
				{
					if (station != null)
					{
						station.ProductMappings = this.EnumerateGasboyStationProductMappings(security, station.IdentityGuid);
					}
				}

			return stations;
        }

        /// <summary>
        /// Get all External Stations assigned or owned by the current site that partially match the ID provided
        /// </summary>
        /// <param name="security">Contains security information, like the site we're currently accessing to retrieve External Stations for</param>
        /// <param name="searchFilter">The ID to search for matches on</param>
        /// <returns>All External Stations assigned or owned by the current site that partially match the provided ID</returns>
        public List<GasboyStation> EnumerateAndFilter(SecurityClass security, string searchFilter)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            List<GasboyStation> stations;

            using (var dbi = new GasboyStationDBI(security.UserID))
            {
                stations = dbi.GetList(security, security.SiteGuid, searchFilter);
            }

            return stations;
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

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationGuid");
            }

            GasboyStation station;

            using (var dbi = new GasboyStationDBI(security.UserID))
            {
                station = dbi.Get(security, externalStationGuid);
            }

            if (station != null)
            {
                station.ProductMappings = this.EnumerateGasboyStationProductMappings(security, externalStationGuid);
            }

            return station;
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

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (string.IsNullOrEmpty(externalStationID))
            {
                throw new ArgumentException("externalStationID");
            }

            GasboyStation station;

            using (var dbi = new GasboyStationDBI(security.UserID))
            {
                station = dbi.GetByID(security, security.SiteGuid, externalStationID);
            }

            if (station != null)
            {
                station.ProductMappings = this.EnumerateGasboyStationProductMappings(security, station.IdentityGuid);
            }

            return station;
        }

		/// <summary>
		/// Retrieve the External Station identified by the provided id
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalSiteCode">Identifies the externalSiteCode to retrieve</param>
		/// <returns>The External Station identified by the provided id</returns>
		public GasboyStation GetBySiteCode(SecurityClass security, string externalSiteCode)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (string.IsNullOrEmpty(externalSiteCode))
			{
				throw new ArgumentException("externalSiteCode");
			}

			GasboyStation station;

			using (var dbi = new GasboyStationDBI(security.UserID))
			{
				station = dbi.GetBySiteCode(security, security.SiteGuid, externalSiteCode);
			}

			if (station != null)
			{
				station.ProductMappings = this.EnumerateGasboyStationProductMappings(security, station.IdentityGuid);
			}

			return station;
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

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
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

            using (var dbi = new GasboyStationDBI(security.UserID))
            {
                dbi.Insert(security, externalStation);
            }

            // Create a record mapping the external station to the current site
            EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
            EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(externalStation);
            entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

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

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStation == null)
            {
                throw new ArgumentNullException("externalStation");
            }

            this.Validate(externalStation);

            // Modify the security object's site guid in case the External Station's site is changing from an entity ownership change.
            // We want to perform the check in the site the external station will be owned by, not the site it's currently owned by.
            Guid siteGuid = security.SiteGuid;
            security.SiteGuid = externalStation.SiteGuid;

            Guid existingExternalStationGuid = this.GetIdentityGuid(security, externalStation.ID);

            // restore the site guid to its original value
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

            using (var dbi = new GasboyStationDBI(security.UserID))
            {
                dbi.Update(security, externalStation);
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
        /// Modify connection status information for the provided external stations. 
        /// </summary>
        /// <param name="security">Contains Security information</param>
        /// <param name="externalStations">External stations with updated connection status information</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void ModifyConnectionInformation(SecurityClass security, List<GasboyStation> externalStations)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStations == null)
            {
                throw new ArgumentNullException("externalStations");
            }

            foreach (GasboyStation externalStation in externalStations)
            {
                externalStation.UpdatedBy = security.UserID;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                GasboyStationDBI.UpdateConnectionInformationSQL(cmd, externalStations);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
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

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
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

                // Delete the External Station now that the mappings have been removed.
                using (var dbi = new GasboyStationDBI(security.UserID))
                {
                    dbi.Delete(security, externalStation);
                }
            }
            else
            {
                throw new Exception("The External Station to delete was not found");
            }
        }

        #endregion External Station Methods

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
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            List<GasboyStationLog> externalStationLogs = GasboyStationLogDBI.GetList(security, security.SiteGuid, externalStationGuid, beginDate, endDate, logType);

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

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationLogGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationLogGuid");
            }

            GasboyStationLog externalStationLog = GasboyStationLogDBI.Get(security, externalStationLogGuid);

            return externalStationLog;
        }

        /// <summary>
        /// Get the gasboy station event record identified by the provided guid
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationLogGuid">Identifies the gasboy station event record to retrieve</param>
        /// <returns>The gasboy station event record identified by the provided guid</returns>
        public GasboyStationEvent GetGasboyStationEvent(SecurityClass security, Guid externalStationLogGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationLogGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationLogGuid");
            }

            GasboyStationEvent gasboyStationEvent = GasboyStationEventDBI.Get(security, externalStationLogGuid);

            return gasboyStationEvent;
        }

        /// <summary>
        /// Add the provided log record to the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationLog">External Station Log entry to add to the database</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AddExternalStationLog(SecurityClass security, GasboyStationLog externalStationLog)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (externalStationLog == null)
            {
                throw new ArgumentNullException("externalStationLog");
            }

            var gasboyStationLogs = new List<GasboyStationLog> { externalStationLog };

            this.AddExternalStationLogs(security, gasboyStationLogs);
        }

        /// <summary>
        /// Add the provided log records to the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationLogs">External Station Log objects to add to the database</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AddExternalStationLogs(SecurityClass security, List<GasboyStationLog> externalStationLogs)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (externalStationLogs == null)
            {
                throw new ArgumentNullException("externalStationLogs");
            }

            List<GasboyStationEvent> gasboyStationEvents = new List<GasboyStationEvent>();
            foreach (GasboyStationLog externalStationLog in externalStationLogs)
            {
                Guid identityGuid = Guid.NewGuid();
                externalStationLog.IdentityGuid = identityGuid;
                externalStationLog.SiteGuid = security.SiteGuid;
                externalStationLog.CreatedBy = security.UserID;

                if (externalStationLog is GasboyStationEvent)
                {
                    GasboyStationEvent gasboyStationEvent = externalStationLog as GasboyStationEvent;
                    gasboyStationEvent.ExternalStationLogGuid = identityGuid;
                    gasboyStationEvents.Add(externalStationLog as GasboyStationEvent);
                }
            }

            GasboyStationLogDBI.Insert(security, externalStationLogs);

            if (gasboyStationEvents.Count > 0)
            {
                this.InsertGasboyStationEvents(security, gasboyStationEvents);
            }
        }

        /// <summary>
        /// Delete External Station Logs older than the specified number of days
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="maximumDaysToRetainLogs">Logs older than this will be deleted</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void PurgeExternalStationLogs(SecurityClass security, int maximumDaysToRetainLogs)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            GasboyStationLogDBI.Purge(security, maximumDaysToRetainLogs);
        }

        /// <summary>
        /// Add the provided gasboy station event records to the database.
        /// The event data is stored in a child table of the log table - 
        /// so this method should only be called after creating the log portion of the event data 
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="gasboyStationEvents">The events to add to the database</param>
        private void InsertGasboyStationEvents(SecurityClass security, List<GasboyStationEvent> gasboyStationEvents)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (gasboyStationEvents == null)
            {
                throw new ArgumentNullException("gasboyStationEvents");
            }

            GasboyStationEventDBI.Insert(security, gasboyStationEvents);
        }

        #endregion External Station Log Methods

        #region External Station Event Methods
        /// <summary>
        /// Add the provided event record to the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationEvent">External Station Event entry to add to the database</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AddExternalStationEvent(SecurityClass security, GasboyStationEvent externalStationEvent)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (externalStationEvent == null)
            {
                throw new ArgumentNullException("externalStationEvent");
            }

            var gasboyStationEvents = new List<GasboyStationEvent>();
            gasboyStationEvents.Add(externalStationEvent);
        }
        #endregion External Station Event Methods

        /// <summary>
        /// Check to make sure the Gasboy Station is valid
        /// </summary>
        /// <param name="gasboyStation">The Gasboy Station to check</param>
        private void Validate(GasboyStation gasboyStation)
        {
            if (string.IsNullOrEmpty(gasboyStation.ID))
            {
                throw new Exception("ID must be provided for a Gasboy Station");
            }

            if (!gasboyStation.SiteCode.HasValue)
            {
                throw new Exception("Site Code must be provided for a Gasboy Station");
            }
        }

        #region External Station Product Mapping Methods

        /// <summary>Translate the product defined in a station to a corresponding FuelsManager product.  If no mapping entry is found, pass it through.</summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationGuid">If not empty, identifies the external station to retrieve the product mapping for</param>
        /// <param name="externalStationProduct">If not empty, identifies the product defined in the external station to retrieve the corresponding FuelsManager product for.</param>
        /// <returns>All failed transactions for a specific site</returns>
        public GasboyStationProductMapping GetMappedProductByStationProductID(
            SecurityClass security,
            Guid externalStationGuid,
            string externalStationProduct)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            return GasboyStationProductMappingDBI.GetByExternalStationAndProductID(security, externalStationGuid, externalStationProduct);
        }

        /// <summary>
        /// Enumerate product mappings for the provided External Station
        /// </summary>
        /// <param name="security">Contains Security information</param>
        /// <param name="externalStationGuid">Identifies the external station to retrieve product mappings for</param>
        /// <returns>Product mappings for the provided External Station</returns>
        private List<GasboyStationProductMapping> EnumerateGasboyStationProductMappings(SecurityClass security, Guid externalStationGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationGuid");
            }

            List<GasboyStationProductMapping> externalStationProductMappings = GasboyStationProductMappingDBI.GetList(security, externalStationGuid);

            return externalStationProductMappings;
        }

        /// <summary>
        /// Retrieve an existing External Station Product Mapping from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMappingGuid">Identifies the External Station Product Mapping to retrieve</param>
        /// <returns>The External Station Product Mapping identified by the provided guid, or null if it was not found</returns>
        private GasboyStationProductMapping GetProductMapping(SecurityClass security, Guid externalStationProductMappingGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationProductMappingGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationProductMappingGuid");
            }

            GasboyStationProductMapping externalStationProductMapping = GasboyStationProductMappingDBI.Get(security, externalStationProductMappingGuid);

            return externalStationProductMapping;
        }

        /// <summary>
        /// Retrieve an existing External Station Product Mapping from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies the External Station Product Mapping to retrieve</param>
        /// <param name="id">The external station product ID to retrieve mappings for</param>
        /// <returns>The External Station Product Mapping identified by the provided guid, or null if it was not found</returns>
        private GasboyStationProductMapping GetProductMappingByExternalStationAndProductID(SecurityClass security, Guid externalStationGuid, string id)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationGuid");
            }

            GasboyStationProductMapping externalStationProductMapping =
                GasboyStationProductMappingDBI.GetByExternalStationAndProductID(security, externalStationGuid, id);

            return externalStationProductMapping;
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
                foreach (GasboyStationProductMapping externalStationProductMapping in newExternalStation.ProductMappings)
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
                        foreach (GasboyStationProductMapping oldGasboyStationProductMapping in oldExternalStation.ProductMappings)
                        {
                            if (oldGasboyStationProductMapping.IdentityGuid == externalStationProductMapping.IdentityGuid)
                            {
                                this.ModifyProductMapping(security, externalStationProductMapping);
                                oldExternalStation.ProductMappings.Remove(oldGasboyStationProductMapping);
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
                foreach (GasboyStationProductMapping oldGasboyStationProductMapping in oldExternalStation.ProductMappings)
                {
                    this.PurgeProductMapping(security, oldGasboyStationProductMapping.IdentityGuid);
                }
            }
        }

        /// <summary>
        /// Add an External Station Product Mapping to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMapping">The External Station Product Mapping to add</param>
        /// <returns>The identity guid (Primary Key) of the new External Station Product Mapping record</returns>
        private Guid AddProductMapping(SecurityClass security, GasboyStationProductMapping externalStationProductMapping)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
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

            GasboyStationProductMappingDBI.Insert(security, externalStationProductMapping);

            return externalStationProductMapping.IdentityGuid;
        }

        /// <summary>
        /// Update an existing External Station Product Mapping in the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMapping">The External Station Product Mapping to update</param>
        private void ModifyProductMapping(SecurityClass security, GasboyStationProductMapping externalStationProductMapping)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationProductMapping == null)
            {
                throw new ArgumentNullException("externalStationProductMapping");
            }

            this.ValidateProductMapping(security, externalStationProductMapping);

            externalStationProductMapping.UpdatedBy = security.UserID;

            GasboyStationProductMappingDBI.Modify(security, externalStationProductMapping);
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

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            GasboyStationProductMapping externalStationProductMapping = this.GetProductMapping(security, externalStationProductMappingGuid);

            if (externalStationProductMapping.IdentityGuid != Guid.Empty)
            {
                GasboyStationProductMappingDBI.Purge(security, externalStationProductMappingGuid);
            }
        }

        /// <summary>
        /// Make sure the data in the product mapping passes validation rules.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationProductMapping">The product mapping to check</param>
        private void ValidateProductMapping(SecurityClass security, GasboyStationProductMapping externalStationProductMapping)
        {
            if (string.IsNullOrEmpty(externalStationProductMapping.ID))
            {
                throw new Exception("The Station Product ID for a product mapping must be provided");
            }

            if (externalStationProductMapping.FuelsManagerProductMasterRecordGuid == Guid.Empty)
            {
                throw new Exception("The FuelsManager Product ID for a product mapping must be provided");
            }

            GasboyStationProductMapping matchingProductMapping = this.GetProductMappingByExternalStationAndProductID(
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

        #endregion External Station Product Mapping Methods

        #region External Station Device Methods

        /// <summary>Get a list of Gasboy Devices associated with the selected External Gasboy Station.</summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationGuid">If not empty, identifies the external station to retrieve the devices for</param>
        /// <returns>All gasboy devices for the specified station</returns>
        public List<GasboyDevice> EnumerateGasboyDevices(
            SecurityClass security,
            Guid externalStationGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            List<GasboyDevice> devices = null;

            using (var dbi = new GasboyDeviceDBI(security.UserID))
            {
                devices = dbi.GetList(security, security.SiteGuid, null, null);
            }

            return devices;
        }

        #endregion External Station Device Methods

        #region General Configuration Methods

        /// <summary>
        /// Add a new External Station General Configuration record to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGeneralConfiguration">The External Station General Configuration record to add</param>
        /// <returns>The identity guid of the new External Station General Configuration record</returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid AddGeneralConfiguration(SecurityClass security, GasboyStationGeneralConfiguration externalStationGeneralConfiguration)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGeneralConfiguration == null)
            {
                throw new ArgumentNullException("externalStationGeneralConfiguration");
            }

            // Make sure that there is not already a External Station General configuration record owned by this site.
            // There should only be one general configuration record per site
            GasboyStationGeneralConfiguration existingGeneralConfiguration = this.GetGeneralConfigurationBySiteGuid(security, security.SiteGuid);

            if (existingGeneralConfiguration != null && existingGeneralConfiguration.IdentityGuid != Guid.Empty)
            {
                throw new Exception("A General Configuration record already exists for this site");
            }

            externalStationGeneralConfiguration.IdentityGuid = Guid.NewGuid();
            externalStationGeneralConfiguration.SiteGuid = security.SiteGuid;
            externalStationGeneralConfiguration.CreatedBy = security.UserID;

            GasboyStationGeneralConfigurationDBI.Insert(security, externalStationGeneralConfiguration);

            return externalStationGeneralConfiguration.IdentityGuid;
        }

        /// <summary>
        /// Modify the provided External Station General Configuration record in the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGeneralConfiguration">The External Station General Configuration record to modify</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void ModifyGeneralConfiguration(SecurityClass security, GasboyStationGeneralConfiguration externalStationGeneralConfiguration)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGeneralConfiguration == null)
            {
                throw new ArgumentNullException("externalStationGeneralConfiguration");
            }

            GasboyStationGeneralConfiguration existingGeneralConfiguration = this.GetGeneralConfigurationBySiteGuid(security, security.SiteGuid);

            if (existingGeneralConfiguration != null && existingGeneralConfiguration.IdentityGuid != Guid.Empty
                && existingGeneralConfiguration.IdentityGuid != externalStationGeneralConfiguration.IdentityGuid)
            {
                throw new Exception("A different External Station General Configuration record already exists for this site");
            }

            GasboyStationGeneralConfiguration oldGeneralConfiguration = this.GetGeneralConfiguration(security, externalStationGeneralConfiguration.IdentityGuid);

            if (oldGeneralConfiguration == null || oldGeneralConfiguration.IdentityGuid == Guid.Empty)
            {
                throw new Exception("The External Station General Configuration record was not found");
            }

            externalStationGeneralConfiguration.UpdatedBy = security.UserID;

            GasboyStationGeneralConfigurationDBI.Update(security, externalStationGeneralConfiguration);
        }

        /// <summary>
        /// Delete the external station identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGeneralConfigurationGuid">Identifies the external Station General Configuration record to delete</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void PurgeGeneralConfiguration(SecurityClass security, Guid externalStationGeneralConfigurationGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            GasboyStationGeneralConfiguration externalStationGeneralConfiguration = this.GetGeneralConfiguration(security, externalStationGeneralConfigurationGuid);

            if (externalStationGeneralConfiguration.IdentityGuid != Guid.Empty)
            {
                GasboyStationGeneralConfigurationDBI.Purge(security, externalStationGeneralConfigurationGuid);
            }
            else
            {
                throw new Exception("The External Station General Configuration record to delete was not found");
            }
        }

        /// <summary>
        /// Get the general configuration record identified by the provided guid. 
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGeneralConfigurationGuid">Identifies the general configuration record to retrieve</param>
        /// <returns>The general configuration record identified by the provided guid, or null if it was not found</returns>
        public GasboyStationGeneralConfiguration GetGeneralConfiguration(SecurityClass security, Guid externalStationGeneralConfigurationGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (externalStationGeneralConfigurationGuid == Guid.Empty)
            {
                throw new ArgumentException("externalStationGeneralConfigurationGuid");
            }

            GasboyStationGeneralConfiguration stationGeneralConfiguration = GasboyStationGeneralConfigurationDBI.Get(security, externalStationGeneralConfigurationGuid);

            return stationGeneralConfiguration;
        }

        /// <summary>
        /// Get the general configuration record identified by the provided site guid. There should only be one general configuration record per site
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="siteGuid">The site guid to retrieve the general configuration record for</param>
        /// <returns>The general configuration record identified by the provided site guid, or null if it was not found</returns>
        public GasboyStationGeneralConfiguration GetGeneralConfigurationBySiteGuid(SecurityClass security, Guid siteGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            if (siteGuid == Guid.Empty)
            {
                throw new ArgumentException("siteGuid");
            }

            GasboyStationGeneralConfiguration stationGeneralConfiguration = GasboyStationGeneralConfigurationDBI.GetBySiteGuid(security, siteGuid);

            return stationGeneralConfiguration;
        }

        /// <summary>
        /// Get all External Station General Configuration records
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <returns>All External Station General Configuration records configured in the system</returns>
        public List<GasboyStationGeneralConfiguration> EnumerateGeneralConfigurations(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
            {
                throw new FMInsufficientRightsException();
            }

            List<GasboyStationGeneralConfiguration> stationGeneralConfigurations = GasboyStationGeneralConfigurationDBI.GetList(security);
            return stationGeneralConfigurations;
        }

        #endregion General Configuration Methods
    }
}