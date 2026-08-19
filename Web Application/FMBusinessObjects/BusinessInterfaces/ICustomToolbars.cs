// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICustomToolbars.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ICustomToolbars type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for CustomToolbars.  Provides a database interface for
	/// the CustomToolbarClass type.
	/// </summary>
	[ServiceContract]
	public interface ICustomToolbars
	{
		/// <summary>
		/// Adds a CustomToolbarClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbar">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, CustomToolbarClass customToolbar);

		/// <summary>
		///  Modifies an existing CustomToolbarClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbar">The object to modify in the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, CustomToolbarClass customToolbar);

		/// <summary>
		/// Deletes an existing CustomToolbarClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarGuid">The identity Guid of the object to delete from the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid customToolbarGuid);

		/// <summary>
		/// Gets an existing CustomToolbarClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="customToolbarGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified CustomToolbarClass object</returns>
		[OperationContract]
		CustomToolbarClass Get(SecurityClass security, Guid customToolbarGuid);

		/// <summary>
		/// Gets the identity Guid of a CustomToolbarClass object from the database given the ID
		/// and associated dispatch configuration identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the CustomToolbarClass object</param>
		/// <param name="dispatchConfigurationGuid">The asscoiated dispatch configuration identity Guid</param>
		/// <returns>The identity Guid of the specified CustomToolbarClass object</returns>
		[OperationContract]
		Guid GetIdentityGuidById(SecurityClass security, string id, Guid dispatchConfigurationGuid);

		/// <summary>
		/// Gets a list of CustomToolbarClass objects from the database given the associated
		/// dispatch configuration identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfigurationGuid">The asscoiated dispatch configuration identity Guid</param>
		/// <returns>The specified list of CustomToolbarClass objects</returns>
		[OperationContract]
		CustomToolbarCollectionClass Enumerate(SecurityClass security, Guid dispatchConfigurationGuid);

		/// <summary>
		/// Gets a list of CustomToolbarType objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of CustomToolbarType objects</returns>
		[OperationContract]
		CustomToolbarTypeList EnumerateToolbarTypes(SecurityClass security);

		/// <summary>
		/// Gets a list of CustomToolbarType objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of CustomToolbarType objects</returns>
		[OperationContract]
		CustomToolbarType EnumerateToolbarTypeById(SecurityClass security, string id);
	}
}
