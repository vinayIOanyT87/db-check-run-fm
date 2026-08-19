
namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Security;
    using System.ServiceModel;
    using System.Transactions;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using DataAccessLayer;
    using InternalClasses.SyncClasses;

    /// <summary>
    /// Summary description for SyncScopes
    /// </summary>
    [SecuritySafeCritical]
    [ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
    public class SyncScopes : ISyncScopes
    {
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        #region Public Methods

        /// <summary>
        /// Operation to add a new SyncScope master record version.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="syncScope"></param>
        /// <returns></returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SyncScopeDO syncScope)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncScope == null)
            {
                throw new ArgumentNullException("syncScope");
            }

            if (!security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS)
                && !security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncScope);

            security.CreatedDate = DateTimeOffset.Now;
            security.CreatedBy = security.UserID;
            security.UpdatedDate = syncScope.CreatedDate;
            security.UpdatedBy = security.UserID;

            using (var dbi = new SyncScopeDBI(security.UserID))
            {
                dbi.Save(security, syncScope);
            }

            return syncScope.IdentityGuid;
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SyncScopeDO syncScope)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncScope == null)
            {
                throw new ArgumentNullException("syncScope");
            }

            if (!security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS)
                && !security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncScope);

            syncScope.UpdatedDate = DateTimeOffset.Now;
            syncScope.UpdatedBy = security.UserID;

            using (var dbi = new SyncScopeDBI(security.UserID))
            {
                dbi.Save(security, syncScope);
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid syncScopeGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS)
                && !security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            SyncScopeDO syncScope = null;

            using (var dbi = new SyncScopeDBI(security.UserID))
            {
                syncScope = dbi.Get(security, syncScopeGuid, null);

                if (null == syncScope)
                {
                    throw new Exception("SyncScope Not Found");
                }
                else
                {
                    if (syncScope.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("SyncScope Not Found");
                    }
                }

                dbi.Delete(security, syncScope, true);
            }
        }

        public SyncScopeDO Get(SecurityClass security, Guid syncScopeGuid)
        {
            SyncScopeDO syncScope = null;

            using (var dbi = new SyncScopeDBI(security.UserID))
            {
                syncScope = dbi.Get(security, syncScopeGuid, null);
            }

            return syncScope;
        }

        public SyncScopeDO GetById(SecurityClass security, Guid pSyncProfileGuid, string pID)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS)
                && !security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            SyncScopeDO syncScope = null;

            using (var dbi = new SyncScopeDBI(security.UserID))
            {
                syncScope = dbi.Get(security, pSyncProfileGuid, pID);
            }

            return syncScope;
        }


        public Guid GetIdentityGuid(SecurityClass security, Guid pSyncProfileGuid, string pID)
        {
            Guid result = Guid.Empty;
            SyncScopeDO syncScope = GetById(security, pSyncProfileGuid, pID);

            if (syncScope != null)
            {
                result = syncScope.IdentityGuid;
            }

            return result;
        }

        public SyncScopeCollection Enumerate(SecurityClass security, SyncProfileDO pSyncProfile)
        {
            return EnumerateExt(security, pSyncProfile);
        }

        [SecurityCritical]
        public SyncScopeCollection EnumerateExt(SecurityClass security, SyncProfileDO pSyncProfile, int pLimit = 0)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS)
                && !security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            var syncScopes = new SyncScopeCollection();

            using (var dbi = new SyncScopeDBI(security.UserID))
            {
                syncScopes.AddRange(dbi.GetList(security, pSyncProfile.IdentityGuid));
            }

            return syncScopes;
        }

        private string ConvertToString(Byte[] rowVersion)
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

        private void Validate(SecurityClass security, SyncScopeDO syncScope)
        {
            if (string.IsNullOrEmpty(syncScope.ID))
                throw new Exception("ID Required");

            if (syncScope.ID == "{Complete}")
                throw new Exception("ID is reserved key word " + syncScope.ID);

            if (syncScope.FriendlyName.Length > 100)
                throw new Exception("Name Exceeded max length (100)");

            Guid syncScopeGuid = GetIdentityGuid(security, syncScope.SyncProfileGuid, syncScope.ID);
            if (syncScopeGuid != Guid.Empty && syncScopeGuid != syncScope.IdentityGuid)
                throw (new Exception("SyncScope Exists"));
        }

        #endregion Validation
    }
}
