// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatchConfigurations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDispatchConfigurations interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for DispatchConfigurations.  Provides a database interface for
	/// the DispatchConfigurationClass type.
	/// </summary>
	[ServiceContract]
	public interface IDispatchConfigurations
	{
		/// <summary>
		/// Adds a DispatchConfigurationClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfig">The object to add to the database</param>
		/// <returns>The identity Guid of the added record</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, DispatchConfigurationClass dispatchConfig);

		/// <summary>
		///  Modifies an existing DispatchConfigurationClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfig">The object to modify in the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, DispatchConfigurationClass dispatchConfig);

		/// <summary>
		/// Deletes an existing DispatchConfigurationClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfigGuid">The identity Guid of the object to delete from the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid dispatchConfigGuid);

		/// <summary>
		/// Gets an existing DispatchConfigurationClass object from the database given the identity Guid.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="dispatchConfigGuid">The identity Guid of the object to get from the database</param>
		/// <returns>The specified DispatchConfigurationClass object</returns>
		[OperationContract]
		DispatchConfigurationClass Get(SecurityClass security, Guid dispatchConfigGuid);

		/// <summary>
		/// Gets the identity Guid of a DispatchConfigurationClass object from the database given the ID.
		/// Assigned entities are given preference to owned entities when both exist with the specified ID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the DispatchConfigurationClass object</param>
		/// <returns>The identity Guid of the specified DispatchConfigurationClass object</returns>
		[OperationContract]
		Guid GetIdentityGuidById(SecurityClass security, string id);

		/// <summary>
		/// Gets the identity Guid of a DispatchConfigurationClass object from the database given the Site Guid and ID.
		/// Owned entities are given preference to assigned entities when both exist with the specified Site Guid and ID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="siteGuid">The Site Guid of the DispatchConfigurationClass object</param>
		/// <param name="id">The ID of the DispatchConfigurationClass object</param>
		/// <returns>The identity Guid of the specified DispatchConfigurationClass object</returns>
		[OperationContract]
		Guid GetIdentityGuidBySiteAndId(SecurityClass security, Guid siteGuid, string id);

		/// <summary>
		/// Gets the identity Guid of a DispatchConfigurationClass object from the database given the Site Guid and ID.  The parameter
		/// getAssignedEntityFirst is used to determine how to select between an owned and assigned entity when both exist.  If set
		/// to true and both entities exist the Guid of the assigned entity is returned otherwise the Guid of the owned entity is
		/// returned.  If only one entity exists then its Guid is returned whether it is an assigned or owned entity.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="siteGuid">The Site Guid of the DispatchConfigurationClass object</param>
		/// <param name="id">The ID of the DispatchConfigurationClass object</param>
		/// <param name="getAssignedEntityFirst">If an assigned entity exists get its identity Guid</param>
		/// <param name="entityAssigned">True if the returned identity Guid is from an assigned entity</param>
		/// <returns>The identity Guid of the specified DispatchConfigurationClass object</returns>
		[OperationContract]
		Guid GetIdentityGuidBySiteIdAndAssigned(SecurityClass security, Guid siteGuid, string id, bool getAssignedEntityFirst, out bool entityAssigned);

		/// <summary>
		/// Gets a list of DispatchConfigurationClass objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of DispatchConfigurationClass objects</returns>
		[OperationContract]
		DispatchConfigurationCollectionClass Enumerate(SecurityClass security);
	}
}
