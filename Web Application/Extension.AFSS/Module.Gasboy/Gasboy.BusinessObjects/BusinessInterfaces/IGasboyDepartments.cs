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
	public interface IGasboyDepartments
	{
		/// <summary>
		/// Get devices configured for the site, filtering by the filter text if it was provided
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <returns>Devices configured for the site, filtered by the filter text if it was provided</returns>
		[OperationContract]
		GasboyDepartmentCollection Enumerate(SecurityClass security);

		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="gasboyDepartmentGuid">The gasboy department unique identifier.</param>
		/// <returns>GasboyDepartment.</returns>
		[OperationContract]
		GasboyDepartment Get(SecurityClass security, Guid gasboyDepartmentGuid);
	}
}