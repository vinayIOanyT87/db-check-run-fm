using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
    /// <summary>
    /// Interface for managing a collection of SyncDependencyGroup entities.
    /// </summary>
	[ServiceContract]
	public interface ISyncDependencyGroups
	{
        /// <summary>
        /// Add a new <see cref="SyncDependencyGroupDO"/> instance to the current collection.
        /// </summary>
        /// <param name="pSecurity">Security Context of the process or user performing the add.</param>
        /// <param name="pSyncDependencyGroup"><see cref="SyncDependencyGroupDO"/> instance to add.</param>
        /// <returns>Guid Identity of the added item.</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass pSecurity, SyncDependencyGroupDO pSyncDependencyGroup);

        /// <summary>
        /// Modify an existing <see cref="SyncDependencyGroupDO"/> instance within the collection.
        /// </summary>
        /// <param name="pSecurity">Security Context of the process or user submitting the changes.</param>
        /// <param name="pSyncDependencyGroup"><see cref="SyncDependencyGroupDO"/> instance to update.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass pSecurity, SyncDependencyGroupDO pSyncDependencyGroup);

        /// <summary>
        /// Deletes an existing <see cref="SyncDependencyGroupDO"/> instance from the collection.
        /// </summary>
        /// <param name="pSecurity">Security Context of the process or user performing the delete.</param>
        /// <param name="pSyncDependencyGroupGuid">The <see cref="SyncDependencyGroupDO.IdentityGuid"/> of the SyncDependencyGroup entry to remove.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass pSecurity, Guid pSyncDependencyGroupGuid);

        [OperationContract]
        EquipmentClass Get(SecurityClass pSecurity, Guid pSyncDependencyGroupGuid);

        [OperationContract]
        SyncDependencyGroupDO GetById(SecurityClass pSecurity, string pID);

		[OperationContract]
        Guid GetIdentityGuid(SecurityClass pSecurity, string ID);

		[OperationContract]
		SyncDependencyGroupCollection Enumerate(SecurityClass pSecurity);

		[OperationContract]
        SyncDependencyGroupCollection EnumerateExt(SecurityClass pSecurity, int limit = 0);
	}
}
