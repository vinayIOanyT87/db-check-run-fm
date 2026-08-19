// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGasboyStations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Describes operations that can be performed by the External Station Service class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    using FuelsManager.Afss.BusinessObjects.DataObjects;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// Describes operations to support database operations for External Stations
    /// like adding, modifying, or deleting a record.
    /// </summary>
    [ServiceContract]
    public interface IGasboyStations
    {
        /// <summary>
        /// Get stations configured for the site, filtering by the filter text if it was provided
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <returns>Stations configured for the site, filtered by the filter text if it was provided</returns>
        [OperationContract]
        List<GasboyStation> Enumerate(SecurityClass security);

        /// <summary>
        /// Get all External Stations assigned or owned by the current site that partially match the ID provided
        /// </summary>
        /// <param name="security">Contains security information, like the site we're currently accessing to retrieve External Stations for</param>
        /// <param name="searchFilter">The ID to search for matches on</param>
        /// <returns>All External Stations assigned or owned by the current site that partially match the provided ID</returns>
        [OperationContract]
        List<GasboyStation> EnumerateAndFilter(SecurityClass security, string searchFilter);

        /// <summary>
        /// Retrieve the External Station identified by the provided guid
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies the External Station to retrieve</param>
        /// <returns>The External Station identified by the provided guid</returns>
        [OperationContract]
        GasboyStation Get(SecurityClass security, Guid externalStationGuid);

        /// <summary>
        /// Get the Identity Guid (Primary Key) of the External Station record identified by the provided ID
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationID">Identifies the External Station record to retrieve.</param>
        /// <returns>The Identity Guid (Primary Key) External Station record identified by the provided ID. Will return an empty guid if no match is found</returns>
        [OperationContract]
        Guid GetIdentityGuid(SecurityClass security, string externalStationID);

        /// <summary>
        /// Retrieve the External Station identified by the provided id
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationID">Identifies the External Station to retrieve</param>
        /// <returns>The External Station identified by the provided id</returns>
        [OperationContract]
        GasboyStation GetByID(SecurityClass security, string externalStationID);

		/// <summary>
		/// Retrieve the External Station identified by the provided Gasboy SiteCode
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationID">Identifies the External Station to retrieve</param>
		/// <returns>The External Station identified by the provided Gasboy SiteCode</returns>
		[OperationContract]
		GasboyStation GetBySiteCode(SecurityClass security, string externalStationID);

		/// <summary>
		/// Add a new External Station record to the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStation">The external station to add</param>
		/// <returns>The identity guid of the new External Station record</returns>
		[OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, GasboyStation externalStation);

        /// <summary>
        /// Modify the provided External Station in the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStation">The external station to modify</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass security, GasboyStation externalStation);

        /// <summary>
        /// Modify connection status information for the provided external stations. 
        /// </summary>
        /// <param name="security">Contains Security information</param>
        /// <param name="externalStations">External stations with updated connection status information</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void ModifyConnectionInformation(SecurityClass security, List<GasboyStation> externalStations);

        /// <summary>
        /// Delete the external station identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies the external station to delete</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass security, Guid externalStationGuid);

        /// <summary>
        /// Retrieve external station logs for the site matching the provided search parameters
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationGuid">The station to get logs for, or Guid.Empty for all stations</param>
        /// <param name="beginDate">The beginning date to get logs for</param>
        /// <param name="endDate">The end date to get logs for</param>
        /// <param name="logType">The type of logs to get, or null for all types</param>
        /// <returns>External station logs for the site matching the provided search parameters</returns>
        [OperationContract]
        List<GasboyStationLog> EnumerateLogs(SecurityClass security, Guid externalStationGuid, DateTimeOffset beginDate, DateTimeOffset endDate, ExternalStationLogType? logType);

        /// <summary>
        /// Get the external station log identified by the provided guid
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationLogGuid">Identifies the external station log to retrieve</param>
        /// <returns>The external station log identified by the provided guid</returns>
        [OperationContract]
        GasboyStationLog GetLog(SecurityClass security, Guid externalStationLogGuid);

        /// <summary>
        /// Get the gasboy station event record identified by the provided guid
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationLogGuid">Identifies the gasboy station event record to retrieve</param>
        /// <returns>The gasboy station event record identified by the provided guid</returns>
        [OperationContract]
        GasboyStationEvent GetGasboyStationEvent(SecurityClass security, Guid externalStationLogGuid);

        /// <summary>
        /// Add the provided log record to the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationLog">External Station Log object to add to the database</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void AddExternalStationLog(SecurityClass security, GasboyStationLog externalStationLog);

        /// <summary>
        /// Add the provided log records to the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationLogs">External Station Log objects to add to the database</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void AddExternalStationLogs(SecurityClass security, List<GasboyStationLog> externalStationLogs);

        /// <summary>
        /// Delete External Station Logs older than the specified number of days
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="maximumDaysToRetainLogs">Logs older than this will be deleted</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void PurgeExternalStationLogs(SecurityClass security, int maximumDaysToRetainLogs);

        /// <summary>
        /// Add the provided event record to the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationEvent">External Station Event object to add to the database</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void AddExternalStationEvent(SecurityClass security, GasboyStationEvent externalStationEvent);

        #region Product Mapping APIs

        /// <summary>
        /// Retrieve the External Station identified by the provided id
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies the External Station to retrieve</param>
        /// <param name="externalStationProduct">Identifies the Product defined in the External Station</param>
        /// <returns>The External Station identified by the provided id</returns>
        [OperationContract]
        GasboyStationProductMapping GetMappedProductByStationProductID(SecurityClass security, Guid externalStationGuid, string externalStationProduct);
        
        #endregion Product Mapping APIs

        /// <summary>
        /// Add a new External Station General Configuration record to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGeneralConfiguration">The External Station General Configuration record to add</param>
        /// <returns>The identity guid of the new External Station General Configuration record</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid AddGeneralConfiguration(SecurityClass security, GasboyStationGeneralConfiguration externalStationGeneralConfiguration);

        /// <summary>
        /// Modify the provided External Station General Configuration record in the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGeneralConfiguration">The External Station General Configuration record to modify</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void ModifyGeneralConfiguration(SecurityClass security, GasboyStationGeneralConfiguration externalStationGeneralConfiguration);

        /// <summary>
        /// Delete the external station identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGeneralConfigurationGuid">Identifies the xternal Station General Configuration record to delete</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void PurgeGeneralConfiguration(SecurityClass security, Guid externalStationGeneralConfigurationGuid);

        /// <summary>
        /// Get the general configuration record identified by the provided guid. 
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGeneralConfigurationGuid">Identifies the general configuration record to retrieve</param>
        /// <returns>The general configuration record identified by the provided guid, or null if it was not found</returns>
        [OperationContract]
        GasboyStationGeneralConfiguration GetGeneralConfiguration(SecurityClass security, Guid externalStationGeneralConfigurationGuid);

        /// <summary>
        /// Get the general configuration record identified by the provided site guid. There should only be one general configuration record per site
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="siteGuid">The site guid to retrieve the general configuration record for</param>
        /// <returns>The general configuration record identified by the provided site guid, or null if it was not found</returns>
        [OperationContract]
        GasboyStationGeneralConfiguration GetGeneralConfigurationBySiteGuid(SecurityClass security, Guid siteGuid);

        /// <summary>
        /// Get all External Station General Configuration records
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <returns>All External Station General Configuration records configured in the system</returns>
        [OperationContract]
        List<GasboyStationGeneralConfiguration> EnumerateGeneralConfigurations(SecurityClass security);

        /// <summary>
        /// Get all Gasboy Devices associated with the Site
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <returns>All Gasboy Devices configured in the system</returns>
        [OperationContract]
        List<GasboyDevice> EnumerateGasboyDevices(SecurityClass security, Guid externalStationGuid);
    }
}
