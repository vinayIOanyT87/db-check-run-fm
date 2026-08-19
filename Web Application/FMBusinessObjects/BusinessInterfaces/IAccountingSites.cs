// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountingSites.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IAccountingSites type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for AccountingSites service 
	/// </summary>
	[ServiceContract]
	public interface IAccountingSites
	{
		#region Public Methods and Operators

		/// <summary>
		/// Loads the site info.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <returns>An AccountingSite object containing site information for accounting purposes.</returns>
		[OperationContract]
		AccountingSite LoadSiteInfo(SecurityClass security, Guid siteGuid);

		/// <summary>
		/// Loads the site info.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <returns>An AccountingSite object containing site information for accounting purposes.</returns>
		[OperationContract]
		AccountingSite LoadSiteInfoNoCompanies(SecurityClass security, Guid siteGuid);

		#endregion
	}
}