// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFuelCardLimits.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Defines operations exposed by the Fuel Card Limits Functionality 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using FMBusinessObjects.DataObjects;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;

    /// <summary>
    /// Defines operations exposed by the Fuel Card Limits Functionality
    /// </summary>
    [ServiceContract]
    public interface IFuelCardLimits
    {
        /// <summary>
        /// Add a new Fuel Card Limit and any associated line items or fuel card mappings to the database.
        /// </summary>
        /// <param name="security">Contains Security Information.</param>
        /// <param name="fuelCardLimit">The fuel card limit record to add.</param>
        /// <returns>The identity guid (primary key) of the new Fuel Card Limit record.</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, FuelCardLimit fuelCardLimit);

        /// <summary>
        /// Update an existing Fuel Card Limit and any associated line items and fuel card mappings in the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimit">The fuel card limit record to update</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass security, FuelCardLimit fuelCardLimit);

        /// <summary>
        /// Delete the Fuel Card Limit record identified by the provided Guid and any associated records
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit record to delete.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass security, Guid fuelCardLimitGuid);

	    /// <summary>
	    /// Add a new Fuel Card Limit and any associated line items or fuel card mappings to the database.
	    /// </summary>
	    /// <param name="security">Contains Security Information.</param>
	    /// <param name="fuelCard">The fuel card object which contain a fuel card limit.</param>
	    [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddFuelCardToFuelCardLimitMapping(SecurityClass security, FuelCardClass fuelCard);

		/// <summary>
		/// This method will update an existing fuel card to fuel card limit mapping.
		/// </summary>
		/// <param name="security">Contains Security Information.</param>
		/// <param name="oldFuelCardLimitGuid">The previous fuel card limit GUID.</param>
		/// <param name="fuelCard">The updated fuel card.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyFuelCardToFuelCardLimitMapping(SecurityClass security, Guid oldFuelCardLimitGuid, FuelCardClass fuelCard);

        /// <summary>
        /// Get the Fuel Card Limit record identified by the provided Guid
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit record to retrieve.</param>
        /// <returns>The Fuel Card Limit record identified by the provided Guid. Will return null if no match is found</returns>
        [OperationContract]
        FuelCardLimit Get(SecurityClass security, Guid fuelCardLimitGuid);

        /// <summary>
        /// Get the Identity Guid (Primary Key) of the Fuel Card Limit record identified by the provided ID
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimitID">Identifies the fuel card limit record to retrieve.</param>
        /// <returns>The Identity Guid (Primary Key) Fuel Card Limit record identified by the provided ID. Will return an empty guid if no match is found</returns>
        [OperationContract]
        Guid GetIdentityGuid(SecurityClass security, string fuelCardLimitID);

        /// <summary>
        /// Get all Fuel Card Limits assigned or owned by the current site
        /// </summary>
        /// <param name="security">Contains security information, like the site we're currently accessing to retrieve fuel card limits for</param>
        /// <returns>All Fuel Card Limits assigned or owned by the current site</returns>
        [OperationContract]
        List<FuelCardLimit> Enumerate(SecurityClass security);

        [OperationContract]
        DataTable EnumerateForMobile(SecurityClass security);

        /// <summary>
        /// Get all Fuel Card Limits assigned or owned by the current site that partially match the ID provided
        /// </summary>
        /// <param name="security">Contains security information, like the site we're currently accessing to retrieve fuel card limits for</param>
        /// <param name="searchFilter">The ID to search for matches on</param>
        /// <returns>All Fuel Card Limits assigned or owned by the current site that partially match the provided ID</returns>
        [OperationContract]
        List<FuelCardLimit> EnumerateAndFilter(SecurityClass security, string searchFilter);

	    /// <summary>
	    /// Gets Fuel Card Limit assigned to a fuel card.
	    /// </summary>
	    /// <param name="security">Contains security information, like the site we're currently accessing to retrieve fuel card limit.</param>
	   /// <param name="fuelCardGuid">The fuel card GUID filter.</param>
	    /// <returns>Fuel Card Limit assigned to a fuel card.</returns>
	    [OperationContract]
	    FuelCardLimit EnumerateFuelCardLimitMappingsByFuelCardGuid(SecurityClass security, Guid fuelCardGuid);
    }
}
