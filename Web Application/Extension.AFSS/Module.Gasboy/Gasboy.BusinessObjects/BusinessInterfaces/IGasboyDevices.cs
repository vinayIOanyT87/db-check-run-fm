// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGasboyDevices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Describes operations that can be performed by the Gasboy Device Service class
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
	/// Describes operations to support database operations for Gasboy Devices
	/// like adding, modifying, or deleting a record.
	/// </summary>
	[ServiceContract]
	public interface IGasboyDevices
	{
		/// <summary>
		/// Get devices configured for the site, filtering by the filter text if it was provided
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <returns>Devices configured for the site, filtered by the filter text if it was provided</returns>
		[OperationContract]
		List<GasboyDevice> Enumerate(SecurityClass security);

		/// <summary>
		/// Get devices configured for the site, including devices with a deleted status
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <returns>Devices configured for the site, filtered by the filter text if it was provided</returns>
		[OperationContract]
		List<GasboyDevice> EnumerateWithDeleted(SecurityClass security);

		/// <summary>
		/// Get all Gasboy Devices assigned or owned by the current site that partially match the ID provided
		/// </summary>
		/// <param name="security">Contains security information, like the site we're currently accessing to retrieve Gasboy Devices for</param>
		/// <param name="searchFilter">The ID to search for matches on</param>
		/// <returns>All Gasboy Devices assigned or owned by the current site that partially match the provided ID</returns>
		[OperationContract]
		List<GasboyDevice> EnumerateAndFilter(SecurityClass security, string searchFilter);

		/// <summary>
		/// Retrieve the Gasboy Device identified by the provided guid
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDeviceGuid">Identifies the Gasboy Device to retrieve</param>
		/// <returns>The Gasboy Device identified by the provided guid</returns>
		[OperationContract]
		GasboyDevice Get(SecurityClass security, Guid gasboyDeviceGuid);

		/// <summary>
		/// Retrieve the Gasboy Device identified by the provided guid
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="siteGuid">Identifies the site to retrieve the device from</param>
		/// <param name="departmentGuid">Identifies the department to retrieve the device from</param>
		/// <returns>The Gasboy Device identified by the provided guid</returns>
		[OperationContract]
		List<GasboyDevice> GetByDepartment(SecurityClass security,Guid siteGuid, Guid departmentGuid);

		/// <summary>
		/// Get the Identity Guid (Primary Key) of the Gasboy Device record identified by the provided Name
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="gasboyDeviceID">Identifies the Gasboy Device to retrieve</param>
		/// <param name="gasboyDeviceName">Identifies the Gasboy Device record to retrieve.</param>
		/// <returns>The Identity Guid (Primary Key) Gasboy Device record identified by the provided ID. Will return an empty guid if no match is found</returns>
		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, long? gasboyDeviceID, string gasboyDeviceName);

		/// <summary>
		/// Retrieve the Gasboy Device identified by the provided Id
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDeviceID">Identifies the Gasboy Device to retrieve</param>
		/// <param name="gasboyDeviceName">Identifies the Gasboy Device to retrieve</param>
		/// <returns>The Gasboy Device identified by the provided id</returns>
		[OperationContract]
		GasboyDevice GetByName(SecurityClass security, long? gasboyDeviceID, string gasboyDeviceName);

		/// <summary>
		/// Retrieve the Gasboy Device associated with the specified card number
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="siteGuid">Identifies the site to retrieve the device from</param>
		/// <param name="gasboyCardNumber">Card Number of the Gasboy Device to retreive</param>
		/// <returns>The Gasboy Device identified by the specified card number.</returns>
		[OperationContract]
		GasboyDevice GetByCardNumber(SecurityClass security, Guid siteGuid, string gasboyCardNumber);

		/// <summary>
		/// Add a new Gasboy Device record to the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDevice">The Gasboy Device to add</param>
		/// <returns>The identity guid of the new Gasboy Device record</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, GasboyDevice gasboyDevice);

		/// <summary>
		/// Modify the provided Gasboy Device in the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDevice">The Gasboy Device to add</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, GasboyDevice gasboyDevice);

		/// <summary>
		/// Delete the Gasboy Device identified by the provided guid from the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="gasboyDeviceGuid">Identifies the Gasboy Device to delete</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid gasboyDeviceGuid);
	}
}
