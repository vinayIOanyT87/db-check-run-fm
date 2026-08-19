// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdditiveProfiles.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IAdditiveProfiles type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
    using System.Data;
    using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for additive profiles service
	/// </summary>
	[ServiceContract]
	public interface IAdditiveProfiles
	{
		#region Public Methods and Operators

		/// <summary>
		/// Adds the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="additiveProfile">The additive profile.</param>
		/// <returns>The Guid of the newly added profile.</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AdditiveProfileClass additiveProfile);

		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A collection of additive profiles.</returns>
		[OperationContract]
		AdditiveProfileCollectionClass Enumerate(SecurityClass security);

		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="identityGuid">The identity GUID.</param>
        /// <param name="hideHiddenProducts">If true, only products that are not hidden will be returned in the additve collection</param>
		/// <returns>An additive profile class.</returns>
		[OperationContract]
		AdditiveProfileClass Get(SecurityClass security, Guid identityGuid, bool hideHiddenProducts = false);

		/// <summary>
		/// Gets the identity GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="id">The ID.</param>
		/// <returns>The identity Guid of the additive profile with ID.</returns>
		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string id);

		/// <summary>
		/// Modifies the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="additiveProfile">The additive profile.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AdditiveProfileClass additiveProfile);

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="identityGuid">The identity GUID.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid identityGuid);

		/// <summary>
		/// This interface will retrieve all the additive profiles at all sites.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a data set of all additive profiles at all sites.</returns>
		[OperationContract]
		DataSet EnumerateAdditiveProfilesAllSites(SecurityClass security);

		#endregion
	}
}