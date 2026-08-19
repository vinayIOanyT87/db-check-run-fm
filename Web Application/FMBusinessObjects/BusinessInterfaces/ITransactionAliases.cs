// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionAliases.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ITransactionAliases type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Provides a database interface for the TransactionAliasClass and TransactionAliasNameClass types.
	/// </summary>
	[ServiceContract]
	public interface ITransactionAliases
	{
		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAlias">
		/// The transaction alias.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, TransactionAliasClass transactionAlias);

		/// <summary>
		/// The enumerate names only.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="byUser">
		/// The by user.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasNameCollectionClass"/>.
		/// </returns>
		[OperationContract]
		TransactionAliasNameCollectionClass EnumerateNamesOnly(SecurityClass security, bool byUser);

		/// <summary>
		/// Gets a list of TransactionAliasNameClass objects that are associated with
		/// transaction aliases that have the "IncludeInDispatch" flag set to true.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of TransactionAliasNameClass objects</returns>
		[OperationContract]
		TransactionAliasNameCollectionClass EnumerateDispatchAliasNames(SecurityClass security);

		/// <summary>
		/// Gets a list of transaction status codes that are associated with transaction
		/// aliases that have the "IncludeInDispatch" flag set to true.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of transaction status codes</returns>
		[OperationContract]
		List<string> EnumerateDispatchStatusCodes(SecurityClass security);

		/// <summary>
		/// The enumerate by trans type ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transTypeID">
		/// The trans type ID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasCollectionClass"/>.
		/// </returns>
		[OperationContract]
		TransactionAliasCollectionClass EnumerateByTransTypeID(SecurityClass security, TransactionTypes transTypeID);

		/// <summary>
		/// This method will return a collection of transaction aliases based on
		/// on the alias transaction group map.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a collection of transaction aliases.</returns>
		[OperationContract]
		TransactionAliasCollectionClass EnumerateByGroupMapsOnly(SecurityClass security);

		/// <summary>
		/// The get.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="identityGuid">
		/// The identity GUID.
		/// </param>
		/// <param name="byUser">
		/// The by user.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		[OperationContract]
		TransactionAliasClass Get(SecurityClass security, Guid identityGuid, bool byUser);

		/// <summary>
		/// The get identity GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string id);

		/// <summary>
		/// The get master record GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
        Guid GetMasterRecordGuid(SecurityClass security, string id);

		/// <summary>
		/// The get basic info.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAliasClassGuid">
		/// The transaction alias class GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		[OperationContract]
        TransactionAliasClass GetBasicInfo(SecurityClass security, Guid transactionAliasClassGuid, Guid siteGuid);

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasCollectionClass"/>.
		/// </returns>
		[OperationContract]
		TransactionAliasCollectionClass Enumerate(SecurityClass security);

		/// <summary>
		/// The modify.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAlias">
		/// The transaction alias.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, TransactionAliasClass transactionAlias);

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionAliasGuid">
		/// The transaction alias GUID.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid transactionAliasGuid);

		/// <summary>
		/// The import.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="alias">
		/// The alias.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Import(SecurityClass security, TransactionAliasClass alias);

		/// <summary>
		/// The enumerate un-delegated.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasCollectionClass"/>.
		/// </returns>
		[OperationContract]
        TransactionAliasCollectionClass EnumerateUndelegated(SecurityClass security);

		/// <summary>
		/// The get without alias fields.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="aliasGuid">
		/// The alias GUID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		[OperationContract]
		TransactionAliasClass GetWithoutAliasFields(SecurityClass security, Guid aliasGuid);

		/// <summary>
		/// The get if the user has edit permissions on the transaction alias.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="aliasGuid">
		/// The alias GUID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasClass"/>.
		/// </returns>
		[OperationContract]
		bool UserHasModifyPermissions(SecurityClass security, Guid aliasGuid);
	}
}
