// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRights.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface definition for rights class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// IRights interface definition
	/// </summary>
	[ServiceContract]
	public interface IRights
	{
		#region Public Methods and Operators

		/// <summary>
		/// Enumerates the rights for the user of the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A collection of rights of the user of the security class.</returns>
		[OperationContract]
		RightCollectionClass Enumerate(SecurityClass security);

		/// <summary>
		/// The enumerate by user by site.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="userGuid">
		/// The user guid.
		/// </param>
		/// <param name="siteGuid">
		/// The site guid.
		/// </param>
		/// <returns>
		/// The <see cref="RightCollectionClass"/>.
		/// </returns>
		[OperationContract]
		RightCollectionClass EnumerateByUserBySite(SecurityClass security, Guid userGuid, Guid siteGuid);

		/// <summary>
		/// Enumerates security rights for the specified group
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="groupGuid">The identity guid of the group to enumerate rights for.</param>
		/// <returns>A collection of rights assigned to the specified group.</returns>
		[OperationContract]
		RightCollectionClass EnumerateByGroup(SecurityClass security, Guid groupGuid);

		#endregion
	}
}
