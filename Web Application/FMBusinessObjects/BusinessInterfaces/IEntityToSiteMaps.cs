// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityToSiteMaps.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface definition for entity to site maps
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface definition for entity to site maps
	/// </summary>
	[ServiceContract]
	public interface IEntityToSiteMaps
	{
		#region Public Methods and Operators

		/// <summary>Adds the specified entity to site map.</summary>
		/// <param name="security">The security.</param>
		/// <param name="entityToSiteMap">The entity to site map.</param>
		/// <param name="engineTypeGuid">The engine type GUID.</param>
		/// <exception cref="System.ArgumentNullException">Security object null</exception>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, EntityToSiteMapClass entityToSiteMap, Guid engineTypeGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddEquipmentMapping(SecurityClass security, EntityToSiteMapClass entityToSiteMap, bool extendToCompartments);

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void AddList( SecurityClass security, List<EntityToSiteMapClass> addList, Guid entityEngineTypeGuid );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddEquipmentMappingList(SecurityClass security, List<EntityToSiteMapClass> addList, bool extendToCompartments);


		/// <summary>Enumerates maps by type ID and GUID.</summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="identityGuid">The identity GUID.</param>
		/// <returns>A collection of entity to site maps.</returns>
		[OperationContract]
		EntityToSiteMapCollectionClass EnumerateByTypeIDAndGuid(SecurityClass security, ENTITY_TYPE entityType, Guid identityGuid);

		/// <summary>
		/// Enumerates the by type ID and site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <returns> A collection of entity to site maps.</returns>
		[OperationContract]
		EntityToSiteMapCollectionClass EnumerateByTypeIDAndSiteGuid(
			SecurityClass security, ENTITY_TYPE entityType, Guid siteGuid);


		/// <summary>
		/// Enumerates the EntityMaps by Site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="siteGuid">The target AssignedTo site GUID.</param>
		/// <param name="excludeCompartments">Flag to indicate whether to exclude compartments or not from the query.</param>
		/// <returns> A collection of entity to site maps.</returns>
		[OperationContract]
        EntityToSiteMapCollectionClass EnumerateEntityMapsBySiteGuid(
            SecurityClass security, ENTITY_TYPE entityType, Guid assignedToSiteGuid, bool excludeCompartments);


		/// <summary>
		/// Enumerates the EntityMaps by Assigned From Site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="siteGuid">The target AssignedFrom site GUID.</param>
		/// <returns> A collection of entity to site maps.</returns>
		[OperationContract]
		EntityToSiteMapCollectionClass EnumerateEntityMapsByAssignedFromSiteGuid(
			SecurityClass security, ENTITY_TYPE entityType, Guid assignedFromSiteGuid);


		/// <summary>
		/// Enumerates the entity sites.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="selectedSiteGuid">The selected site GUID.</param>
		/// <param name="includeMembers">if set to <c>true</c> includes member sites.</param>
		/// <returns>A list of entity sites</returns>
		[OperationContract]
		List<KeyValuePair<Guid, string>> EnumerateEntitySites(
			SecurityClass security, Guid selectedSiteGuid, bool includeMembers);

		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="guid">The GUID.</param>
		/// <returns> A collection of entity to site maps.</returns>
		[OperationContract]
		EntityToSiteMapClass Get(SecurityClass security, ENTITY_TYPE entityType, Guid guid);

		/// <summary>
		/// Imports the specified entityToSiteMap.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityToSiteMap">Entity to Site Map.</param>
		/// <returns> A collection of entity to site maps.</returns>
		[OperationContract]
		void Import(SecurityClass security, EntityToSiteMapClass entityToSiteMap);

        /// <summary>
        /// Returns the EntityToSiteMap of a given entity record to a given site
        /// </summary>
        /// <param name="security">The security object</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="identityGuid">The guid of the entity record. For entity records under record versioning, this should be the MasterRecordGuid.</param>
        /// <param name="assignedToSiteGuid">The target AssignedTo site for which the mapping is to be retrieved.</param>
        /// <returns></returns>
        [OperationContract]
        EntityToSiteMapClass GetByRecordGuid(SecurityClass security, ENTITY_TYPE entityType, Guid identityGuid, Guid assignedToSiteGuid);


        /// <summary>
        /// Retrieve the Record Version specific fields of an entity record.
        /// </summary>
        /// <param name="security">The FuelsManager security object.
        /// </param>
        /// <param name="entityType">
        /// Entity Type of the entity record
        /// </param>
        /// <param name="masterRecordGuid">
        /// MaterRecordGuid of the entity record
        /// </param>
        /// <param name="flcMode">
        /// FLCMode for which to limit the query
        /// </param>
        /// <returns>
        /// A list of RecordVersioning fields/>.
        /// </returns>
        [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<string> GetRecordVersioningFields(SecurityClass security, ENTITY_TYPE entityType, Guid masterRecordGuid, string flcMode);

		/// <summary>
		/// This method will return true if the entity is assigned. Otherwise, it returns false.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <param name="assignedGuid">The assigned GUID.</param>
		/// <returns>
		///   <c>true</c> if the specified entity is assigned; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">Security object is null.</exception>
		[OperationContract]
		bool IsAssigned(SecurityClass security, ENTITY_TYPE entityType, Guid siteGuid, Guid assignedGuid);

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityToSiteMap">The entity to site map.</param>
		/// <exception cref="System.ArgumentNullException">Security object null.</exception>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, EntityToSiteMapClass entityToSiteMap);

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void PurgeList( SecurityClass security, List<EntityToSiteMapClass> purgeList);

		#endregion
	}
}