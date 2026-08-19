// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncSessionDetails.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncSessionScopeLogs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Security;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.InternalClasses;
    using FMBusinessServices.InternalClasses.SyncClasses;

	/// <summary>
    /// Summary description for SyncSessionScopeLogs
    /// </summary>
    [SecuritySafeCriticalAttribute]
    [ServiceKnownType(typeof(SYNCSITETYPE))]
    [ServiceKnownType(typeof(SyncSessionScopeLogDO))]
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class SyncSessionScopeLogs : ISyncSessionScopeLogs
    {
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        public SyncSessionScopeLogs()
        {
        }

        #region Public Methods

        /// <summary>
        /// Operation to add a new syncSessionScopeLog.
        /// </summary>
        /// <param name="security">
        /// Current security context.
        /// </param>
        /// <param name="syncSessionScopeLog">
        /// Synchronization tracking session scope record to add.
        /// </param>
        /// <returns>
        /// Identity <see cref="Guid"/> of the newly inserted synchronization session scope record.
        /// </returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SyncSessionScopeLogDO syncSessionScopeLog)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncSessionScopeLog == null)
            {
                throw new ArgumentNullException("syncSessionScopeLog");
            }

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncSessionScopeLog);

            syncSessionScopeLog.CreatedDate = DateTimeOffset.Now;
            syncSessionScopeLog.CreatedBy = security.UserID;
            syncSessionScopeLog.UpdatedDate = syncSessionScopeLog.CreatedDate;
            syncSessionScopeLog.UpdatedBy = security.UserID;

            using (var dbi = new SyncSessionScopeLogDBI(security.UserID))
            {
                dbi.Save(security, syncSessionScopeLog);
            }

            return syncSessionScopeLog.IdentityGuid;
        }

        /// <summary>
        /// The modify.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionScopeLog">
        /// The sync session scope log.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the incoming parameters are null.
        /// </exception>
        /// <exception cref="Exception">
        /// Throws an exception if the user does not have synchronization permissions.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SyncSessionScopeLogDO syncSessionScopeLog)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncSessionScopeLog == null)
            {
                throw new ArgumentNullException("syncSessionScopeLog");
            }

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncSessionScopeLog);

            syncSessionScopeLog.UpdatedDate = DateTimeOffset.Now;
            syncSessionScopeLog.UpdatedBy = security.UserID;

            using (var dbi = new SyncSessionScopeLogDBI(security.UserID))
            {
                dbi.Save(security, syncSessionScopeLog);
            }
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionScopeLogGuid">
        /// The sync session scope log GUID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the incoming parameters are null.
        /// </exception>
        /// <exception cref="Exception">
        /// Throws an exception if the user does not have synchronization permissions.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid syncSessionScopeLogGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            SyncSessionScopeLogDO syncSessionScopeLog = null;

            using (var dbi = new SyncSessionScopeLogDBI(security.UserID))
            {
                syncSessionScopeLog = dbi.Get(security, syncSessionScopeLogGuid);

                if (null == syncSessionScopeLog)
                {
                    throw new Exception("syncSessionScopeLog Not Found");
                }
                else
                {
                    if (syncSessionScopeLog.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("syncSessionScopeLog Not Found");
                    }
                }

                dbi.Delete(security, syncSessionScopeLog, true);
            }
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionScopeLogGuid">
        /// The sync session detail GUID.
        /// </param>
        /// <returns>
        /// A populated instance of a <see cref="SyncSessionScopeLogDO"/> object that represents the specified record.
        /// </returns>
        public SyncSessionScopeLogDO Get(SecurityClass security, Guid syncSessionScopeLogGuid)
        {
            SyncSessionScopeLogDO syncSessionScopeLog = null;

            using (var dbi = new SyncSessionScopeLogDBI(security.UserID))
            {
                syncSessionScopeLog = dbi.Get(security, syncSessionScopeLogGuid);
            }

            return syncSessionScopeLog;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionLogGuid">
        /// The synchronization tracking session GUID.
        /// </param>
        /// <param name="siteGuid">
        /// The site GUID to get the synchronization tracking session detail record for.
        /// </param>
        /// <returns>
        /// A populated instance of a <see cref="SyncSessionScopeLogDO"/> object that represents the specified record.
        /// </returns>
        public SyncSessionScopeLogDO GetBySiteGuid(SecurityClass security, Guid syncSessionLogGuid, Guid? siteGuid)
        {
            SyncSessionScopeLogDO syncSessionScopeLog = null;

            using (var dbi = new SyncSessionScopeLogDBI(security.UserID))
            {
                syncSessionScopeLog = dbi.GetBySiteGuid(security, syncSessionLogGuid, siteGuid);
            }

            return syncSessionScopeLog;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionLogGuid">
        /// The synchronization tracking session GUID.
        /// </param>
        /// <param name="siteGuid">
        /// The site GUID to get the synchronization tracking session detail record for.
        /// </param>
        /// <param name="scopeID">
        /// The scope ID.
        /// </param>
        /// <returns>
        /// A populated instance of a <see cref="SyncSessionScopeLogDO"/> object that represents the specified record.
        /// </returns>
        public SyncSessionScopeLogDO GetByCompositeKey(SecurityClass security, Guid syncSessionLogGuid, Guid? siteGuid, string scopeID)
        {
            SyncSessionScopeLogDO syncSessionScopeLog = null;

            using (var dbi = new SyncSessionScopeLogDBI(security.UserID))
            {
                syncSessionScopeLog = dbi.GetByCompositeKey(security, syncSessionLogGuid, siteGuid, scopeID);
            }

            return syncSessionScopeLog;
        }

        /// <summary>
        /// The enumerate.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionLogGuid">
        /// The sync session guid.
        /// </param>
        /// <returns>
        /// The <see cref="SyncSessionScopeLogCollection"/>.
        /// </returns>
        public SyncSessionScopeLogCollection Enumerate(SecurityClass security, Guid syncSessionLogGuid)
        {
            return this.EnumerateExt(security, syncSessionLogGuid);
        }

        /// <summary>
        /// The enumerate ext.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionLogGuid">
        /// The sync session guid.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="SyncSessionScopeLogCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [SecurityCritical]
        public SyncSessionScopeLogCollection EnumerateExt(SecurityClass security, Guid syncSessionLogGuid, int limit = 0)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncSessionLogGuid == null)
            {
                throw new ArgumentNullException("syncSessionLogGuid");
            }

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            var dataObjects = new SyncSessionScopeLogCollection();

            using (var dbi = new SyncSessionScopeLogDBI(security.UserID))
            {
                dataObjects.AddRange(dbi.GetListBySyncSession(security, syncSessionLogGuid));
            }

            return dataObjects;
        }

        /// <summary>
        /// The convert to string.
        /// </summary>
        /// <param name="rowVersion">
        /// The row version.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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

        private void Validate(SecurityClass security, SyncSessionScopeLogDO syncSessionScopeLog)
        {

        }

        #endregion Validation
    }
}
