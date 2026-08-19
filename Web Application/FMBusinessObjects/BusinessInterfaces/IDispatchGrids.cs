// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatchGrids.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDispatchGrids type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for DispatchGrids.  Provides a database interface for
	/// the DispatchGridClass type.
	/// </summary>
	[ServiceContract]
	public interface IDispatchGrids
	{
		/// <summary>
		/// Adds a DispatchGridClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGrid">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, DispatchGridClass dispatchGrid);

		/// <summary>
		///  Modifies an existing DispatchGridClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGrid">The object to modify in the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, DispatchGridClass dispatchGrid);

		/// <summary>
		/// Deletes an existing DispatchGridClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridGuid">The identity Guid of the object to delete from the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid dispatchGridGuid);

		/// <summary>
		/// Gets an existing DispatchGridClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchGridGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified DispatchGridClass object</returns>
		[OperationContract]
		DispatchGridClass Get(SecurityClass security, Guid dispatchGridGuid);

		/// <summary>
		/// Gets the identity Guid of a DispatchGridClass object from the database given the ID
		/// and associated dispatch configuration identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the DispatchGridClass object</param>
		/// <param name="dispatchConfigurationGuid">The asscoiated dispatch configuration identity Guid</param>
		/// <returns>The identity Guid of the specified DispatchGridClass object</returns>
		[OperationContract]
		Guid GetIdentityGuidById(SecurityClass security, string id, Guid dispatchConfigurationGuid);

		/// <summary>
		/// Gets a list of DispatchGridClass objects from the database given the associated
		/// dispatch configuration identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfigurationGuid">The asscoiated dispatch configuration identity Guid</param>
		/// <returns>The specified list of DispatchGridClass objects</returns>
		[OperationContract]
		DispatchGridCollectionClass Enumerate(SecurityClass security, Guid dispatchConfigurationGuid);

		/// <summary>
		/// Gets a list of DispatchGridType objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of DispatchGridType objects</returns>
		[OperationContract]
		DispatchGridTypeList EnumerateGridTypes(SecurityClass security);
	}
}
