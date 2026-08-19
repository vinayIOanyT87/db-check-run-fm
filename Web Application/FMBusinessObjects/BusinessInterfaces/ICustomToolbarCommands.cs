// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICustomToolbarCommands.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ICustomToolbarCommands type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for CustomToolbarCommands.  Provides a database interface for
	/// the CustomToolbarCommandClass type.
	/// </summary>
	[ServiceContract]
	public interface ICustomToolbarCommands
	{
		/// <summary>
		/// Adds a CustomToolbarCommandClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarCommand">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, CustomToolbarCommandClass customToolbarCommand);

		/// <summary>
		/// Modifies an existing CustomToolbarCommandClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarCommand">The object to modify in the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, CustomToolbarCommandClass customToolbarCommand);

		/// <summary>
		/// Deletes an existing CustomToolbarCommandClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarCommandGuid">The identity Guid of the object to delete from the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid customToolbarCommandGuid);

		/// <summary>
		/// Gets an existing CustomToolbarCommandClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarCommandGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified CustomToolbarCommandClass object</returns>
		[OperationContract]
		CustomToolbarCommandClass Get(SecurityClass security, Guid customToolbarCommandGuid);

		/// <summary>
		/// Gets a list of CustomToolbarCommandClass objects from the database given the CustomToolbar identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarGuid">The asscoiated CustomToolbar identity Guid</param>
		/// <returns>The specified list of CustomToolbarCommandClass objects</returns>
		[OperationContract]
		CustomToolbarCommandCollectionClass Enumerate(SecurityClass security, Guid customToolbarGuid);

		/// <summary>
		/// Gets a list of CustomToolbarCommandType objects from the database given the CustomToolbar type.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="toolbarType">The CustomToolbar type</param>
		/// <returns>The specified list of CustomToolbarCommandType objects</returns>
		[OperationContract]
		CustomToolbarCommandTypeList EnumerateCommandTypes(SecurityClass security, int toolbarType);

		/// <summary>
		/// Gets a list of default CustomToolbarCommandType objects from the database given the CustomToolbar type.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="toolbarType">The CustomToolbar type</param>
		/// <returns>The specified list of default CustomToolbarCommandType objects</returns>
		[OperationContract]
		CustomToolbarCommandTypeList EnumerateDefaultCommandTypes(SecurityClass security, int toolbarType);

		/// <summary>
		/// Modify the list of CustomToolbarCommand objects asscoiated with a given CustomToolbar object.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarGuid">The asscoiated CustomToolbar identity Guid</param>
		/// <param name="customToolbarId">The asscoiated CustomToolbar ID</param>
		/// <param name="newCollection">The new list of CustomToolbarCommand objects</param>
		/// <param name="oldCollection">The old list of CustomToolbarCommand objects</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyCollection(
			SecurityClass security,
			Guid customToolbarGuid,
			string customToolbarId,
			CustomToolbarCommandCollectionClass newCollection,
			CustomToolbarCommandCollectionClass oldCollection);
	}
}
