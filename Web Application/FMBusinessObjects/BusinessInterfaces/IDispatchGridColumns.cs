// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatchGridColumns.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDispatchGridColumns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for DispatchGridColumns.  Provides a database interface for
	/// the DispatchGridColumnClass type.
	/// </summary>
	[ServiceContract]
	public interface IDispatchGridColumns
	{
		/// <summary>
		/// Adds a DispatchGridColumnClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridColumn">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, DispatchGridColumnClass dispatchGridColumn);

		/// <summary>
		/// Modifies an existing DispatchGridColumnClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridColumn">The object to modify in the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, DispatchGridColumnClass dispatchGridColumn);

		/// <summary>
		/// Deletes an existing DispatchGridColumnClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridColumnGuid">The identity Guid of the object to delete from the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid dispatchGridColumnGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByUser(SecurityClass security, Guid userGuid);

		/// <summary>
		/// Gets an existing DispatchGridColumnClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridColumnGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified DispatchGridColumnClass object</returns>
		[OperationContract]
		DispatchGridColumnClass Get(SecurityClass security, Guid dispatchGridColumnGuid);

		/// <summary>
		/// Gets a list of DispatchGridColumnClass objects from the database given the dispatch grid identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridGuid">The asscoiated dispatch grid identity Guid</param>
		/// <returns>The specified list of DispatchGridColumnClass objects</returns>
		[OperationContract]
		DispatchGridColumnCollectionClass Enumerate(SecurityClass security, Guid dispatchGridGuid);

		/// <summary>
		/// Gets the list of DispatchGridColumnType objects from the database given the dispatch grid type
		/// and the default order flag.  If the default order flag is true the columns are retrieved in
		/// default order.  Otherwise the columns are retrieved in alphabetical order.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="gridType">The dispatch grid type</param>
		/// <param name="defaultOrder">The default order flag</param>
		/// <returns>The list of DispatchGridColumnType objects</returns>
		[OperationContract]
		DispatchGridColumnTypeList EnumerateColumnTypes(SecurityClass security, int gridType, bool defaultOrder);

		/// <summary>
		/// Modify the list of DispatchGridColumnClass objects asscoiated with a given dispatch grid object.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridGuid">The asscoiated dispatch grid identity Guid</param>
		/// <param name="dispatchGridId">The asscoiated dispatch grid ID</param>
		/// <param name="newCollection">The new list of DispatchGridColumnClass objects</param>
		/// <param name="oldCollection">The old list of DispatchGridColumnClass objects</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyCollection(
			SecurityClass security,
			Guid dispatchGridGuid,
			string dispatchGridId,
			DispatchGridColumnCollectionClass newCollection,
			DispatchGridColumnCollectionClass oldCollection);
	}
}
