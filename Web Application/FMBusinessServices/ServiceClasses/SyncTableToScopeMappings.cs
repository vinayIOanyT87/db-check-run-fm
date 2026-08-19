// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncTableToScopeMappings.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncTableToScopeMappings
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Security;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.InternalClasses;
    using FMBusinessServices.InternalClasses.SyncClasses;

	/// <summary>
    /// Summary description for SyncTableToScopeMappings
    /// </summary>
    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SyncTableToScopeMappings : ISyncTableToScopeMappings
    {
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        public SyncTableToScopeMappings()
        {
        }

        #region Public Methods

        /// <summary>
        /// Operation to add a new SyncTableToScopeMap master record version.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="syncTableToScopeMap"></param>
        /// <returns></returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SyncTableToScopeMapDO syncTableToScopeMap)
        {
            if (security == null)
                throw new ArgumentNullException("security");

            if (syncTableToScopeMap == null)
                throw new ArgumentNullException("syncTableToScopeMap");

            if (!security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS))
            {
                throw (new Exception("Insufficient Rights"));
            }

            Validate(security, syncTableToScopeMap);

            syncTableToScopeMap.CreatedDate = DateTimeOffset.Now;
            syncTableToScopeMap.CreatedBy = security.UserID;
            syncTableToScopeMap.UpdatedDate = syncTableToScopeMap.CreatedDate;
            syncTableToScopeMap.UpdatedBy = security.UserID;

            using (var dbi = new SyncTableToScopeMapDBI(security.UserID))
            {
                dbi.Save(security, syncTableToScopeMap);
            }

            return (syncTableToScopeMap.IdentityGuid);
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SyncTableToScopeMapDO syncTableToScopeMap)
        {
            if (security == null)
                throw new ArgumentNullException("security");

            if (syncTableToScopeMap == null)
                throw new ArgumentNullException("syncTableToScopeMap");

            if (!security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS))
                throw (new Exception("Insufficient Rights"));

            Validate(security, syncTableToScopeMap);

            syncTableToScopeMap.UpdatedDate = DateTimeOffset.Now;
            syncTableToScopeMap.UpdatedBy = security.UserID;

            using (var dbi = new SyncTableToScopeMapDBI(security.UserID))
            {
                dbi.Save(security, syncTableToScopeMap);
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid syncTableToScopeMapGuid)
        {
            if (security == null)
                throw new ArgumentNullException("security");

            if (!security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS))
                throw (new Exception("Insufficient Rights"));

            SyncTableToScopeMapDO syncTableToScopeMap = null;

            using (var dbi = new SyncTableToScopeMapDBI(security.UserID))
            {
                syncTableToScopeMap = dbi.Get(security, syncTableToScopeMapGuid, null);

                if (null == syncTableToScopeMap)
                    throw (new Exception("SyncTableToScopeMap Not Found"));
                else
                {
                    if (syncTableToScopeMap.IdentityGuid == Guid.Empty)
                        throw (new Exception("SyncTableToScopeMap Not Found"));
                }

                dbi.Delete(security, syncTableToScopeMap, true);
            }
        }

        public SyncTableToScopeMapDO Get(SecurityClass security, Guid syncTableToScopeMapGuid)
        {
            SyncTableToScopeMapDO syncTableToScopeMap = null;

            using (var dbi = new SyncTableToScopeMapDBI(security.UserID))
            {
                syncTableToScopeMap = dbi.Get(security, syncTableToScopeMapGuid, null);
            }

            return (syncTableToScopeMap);
        }

        public SyncTableToScopeMapDO GetById(SecurityClass security, string id)
        {
            if (security == null)
                throw new ArgumentNullException("security");

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
                throw (new Exception("Insufficient Rights"));

            SyncTableToScopeMapDO syncTableToScopeMap = null;

            using (var dbi = new SyncTableToScopeMapDBI(security.UserID))
            {
                syncTableToScopeMap = dbi.Get(security, null, id);
            }

            return (syncTableToScopeMap);
        }


        public Guid GetIdentityGuid(SecurityClass security, string id)
        {
            Guid result = Guid.Empty;
            SyncTableToScopeMapDO syncTableToScopeMap = GetById(security, id);

            if (syncTableToScopeMap != null)
                result = syncTableToScopeMap.IdentityGuid;

            return (result);
        }

        public SyncTableToScopeMapCollection Enumerate(SecurityClass security, SyncScopeDO syncScope)
        {
            return EnumerateExt(security, syncScope);
        }

        [SecurityCritical]
        public SyncTableToScopeMapCollection EnumerateExt(SecurityClass security, SyncScopeDO syncScope, int pLimit = 0)
        {
            if (security == null)
                throw new ArgumentNullException("security");

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new FMInsufficientRightsException();
            }

            var syncTableToScopeMaps = new SyncTableToScopeMapCollection();

            using (var dbi = new SyncTableToScopeMapDBI(security.UserID))
            {
                syncTableToScopeMaps.AddRange(dbi.GetList(security, syncScope.IdentityGuid));
            }

            return (syncTableToScopeMaps);
        }

        public SyncTableToScopeMapCollection EnumerateForTable(SecurityClass security, SyncTableDO syncTable)
        {
            if (security == null)
                throw new ArgumentNullException("security");

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw (new Exception("Insufficient Rights"));
            }

            var syncTableToScopeMaps = new SyncTableToScopeMapCollection();

            // Currently stubbed out.  need to expand DBI class to allow retrieval of all SyncScopes where a SyncTable is mapped to.

            return (syncTableToScopeMaps);
        }

        private string ConvertToString(System.Byte[] rowVersion)
        {
            string result = string.Empty;

            foreach (byte b in rowVersion)
            {
                result += b.ToString("X");
            }

            return result;
        }
        #endregion Public Methods

        #region Validation

        private void Validate(SecurityClass security, SyncTableToScopeMapDO syncTableToScopeMap)
        {
            if (string.IsNullOrEmpty(syncTableToScopeMap.ID))
                throw new Exception("ID Required");

            if (syncTableToScopeMap.ID == "{Complete}")
                throw new Exception("ID is reserved key word " + syncTableToScopeMap.ID);

            if (syncTableToScopeMap.SyncScopeGuid == Guid.Empty)
                throw new Exception("A SyncScope must be specified.");

            if (syncTableToScopeMap.SyncTableGuid == Guid.Empty)
                throw new Exception("A SyncTable must be specified.");

            Guid pSyncTableToScopeMapGuid = GetIdentityGuid(security, syncTableToScopeMap.ID);
            if (pSyncTableToScopeMapGuid != Guid.Empty && pSyncTableToScopeMapGuid != syncTableToScopeMap.IdentityGuid)
                throw (new Exception("SyncTableToScopeMap Exists"));
        }

        #endregion Validation
    }
}
