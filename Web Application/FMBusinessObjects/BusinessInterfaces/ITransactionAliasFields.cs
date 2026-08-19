// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionAliasFields.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface ITransactionAliasFields
	{
		#region Public Methods and Operators

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, TransactionAliasFieldClass transactionAliasField);

		/// <summary>
		///     Gets a list of TransactionAliasFieldClass objects from the database given the specified parameters.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="transactionAliasGuid">The asscoiated transaction alias identity Guid</param>
		/// <param name="type">The type of transaction field to retrieve</param>
		/// <param name="dispatchFields">If true then retrieve dispatch transaction fields</param>
		/// <param name="byUser">If true then retrieve fields associated with the current user</param>
		/// <returns>The specified list of TransactionAliasFieldClass objects</returns>
		[OperationContract]
		TransactionAliasFieldCollectionClass Enumerate(
														SecurityClass security,
														Guid transactionAliasGuid,
														TransactionFieldType type,
														bool dispatchFields,
														bool byUser);

		/// <summary>
		/// Gets a list of all the transaction alias fields based on the transaction alias GUID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="aliasGuid">The transaction alias GUID.</param>
		/// <param name="byUser">If true then retrieve fields associated with the current user</param>
		/// <returns>Returns all the transaction aliases field based on the Alias GUID.</returns>
		[OperationContract]
		TransactionAliasFieldCollectionClass EnumerateByAliasGuid(SecurityClass security, Guid aliasGuid, bool byUser);

		/// <summary>
		///     This method will return the list of alias fields to display.
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="fieldType">Contains the transaction alias field type.</param>
		/// <param name="transType">Contains the transaction type.</param>
		/// <returns>Returns a string array of alias field names.</returns>
		[OperationContract]
		List<string> EnumerateFields(SecurityClass security, TransactionFieldType fieldType, TransactionTypes transType);

		[OperationContract]
		TransactionAliasFieldClass Get(SecurityClass security, Guid identityGuid);

        /// <summary>
        /// Extended database attributes for all the fields defined for transactions and sub nodes
        /// </summary>
        /// <param name="userSecurity"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<TransactionAliasFieldExtendedAttributes> GetColumnDefinitionsForTransactions(SecurityClass userSecurity);


        [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, TransactionAliasFieldClass transactionAliasField);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyCollection(
							SecurityClass security,
							Guid transactionAliasGuid,
							string transactionAliasName,
							TransactionAliasFieldCollectionClass newFieldCollection,
							TransactionAliasFieldCollectionClass oldFieldCollection);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid identityGuid);
		#endregion
	}
}