// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncRecordConflicts.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncRecordConflicts
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
    /// Summary description for SyncRecordConflicts
    /// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class SyncRecordConflicts : ISyncRecordConflicts
    {
        /// <summary>
        /// The consolidated da.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        #region Public Methods

        /// <summary>
        /// Operation to add a new SyncRecordConflict master record version.
        /// </summary>
        /// <param name="security">
        /// The calling security context.
        /// </param>
        /// <param name="syncSessionScopeLog">
        /// The sync session detail.
        /// </param>
        /// <param name="syncRecordConflict">
        /// The sync record conflict.
        /// </param>
        /// <returns>
        /// The identity GUID <see cref="Guid"/> for the newly added record.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the passed in security context is null.
        /// </exception>
        /// <exception cref="Exception">
        /// Throws an exception if the passed in record to add is null or if the user does not have <see cref="RIGHT.VIEW_SYNC_CONFLICT_STATUS"/> rights.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SyncSessionScopeLogDO syncSessionScopeLog, SyncRecordConflictDO syncRecordConflict)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncRecordConflict == null)
            {
                throw new ArgumentNullException("syncRecordConflict");
            }

            if (!security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncRecordConflict);

            syncRecordConflict.CreatedDate = DateTimeOffset.Now;
            syncRecordConflict.CreatedBy = security.UserID;
            syncRecordConflict.UpdatedDate = syncRecordConflict.CreatedDate;
            syncRecordConflict.UpdatedBy = security.UserID;

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
                dbi.Save(security, syncSessionScopeLog, syncRecordConflict);
            }

            return syncRecordConflict.IdentityGuid;
        }

        /// <summary>
        /// Persists the modified <see cref="SyncRecordConflictDO"/> instance that's passed in.
        /// </summary>
        /// <param name="security">
        /// The calling security context.
        /// </param>
        /// <param name="syncRecordConflict">
        /// The sync record conflict.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the passed in security context is null.
        /// </exception>
        /// <exception cref="Exception">
        /// Throws an exception if the passed in record to add is null or if the user does not have <see cref="RIGHT.VIEW_SYNC_CONFLICT_STATUS"/> rights.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SyncRecordConflictDO syncRecordConflict)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncRecordConflict == null)
            {
                throw new ArgumentNullException("syncRecordConflict");
            }

			if (syncRecordConflict.IdentityGuid == null)
			{
				throw new ArgumentNullException("syncRecordConflict.IdentityGuid");
			}


            if (!security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncRecordConflict);

            syncRecordConflict.UpdatedDate = DateTimeOffset.Now;
            syncRecordConflict.UpdatedBy = security.UserID;

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
                dbi.Save(security, null, syncRecordConflict);
            }
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncRecordConflictGuid">
        /// The sync record conflict GUID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the security context is null.
        /// </exception>
        /// <exception cref="Exception">
        /// Throws an exception if the user does not have <see cref="RIGHT.VIEW_SYNC_CONFLICT_STATUS"/> rights or if the <see cref="SyncRecordConflictDO"/> record 
        /// could not be found for the passed in syncRecordConflictGuid.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid syncRecordConflictGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
            {
                throw new Exception("Insufficient Rights");
            }

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
                SyncRecordConflictDO pSyncRecordConflict = dbi.Get(security, syncRecordConflictGuid);

                if (null == pSyncRecordConflict)
                {
                    throw new Exception("SyncRecordConflict Not Found");
                }
                else
                {
                    if (pSyncRecordConflict.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("SyncRecordConflict Not Found");
                    }
                }

                dbi.Delete(security, pSyncRecordConflict, true);
            }
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncRecordConflictGuid">
        /// The sync record conflict GUID.
        /// </param>
        /// <returns>
        /// The <see cref="SyncRecordConflictDO"/>.
        /// </returns>
        public SyncRecordConflictDO Get(SecurityClass security, Guid syncRecordConflictGuid)
        {
            SyncRecordConflictDO syncProfile = null;

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
                syncProfile = dbi.Get(security, syncRecordConflictGuid);
            }

            return syncProfile;
        }

        /// <summary>
        /// The get by table and entity key.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="onlyUnresolved">
        /// The only unresolved.
        /// </param>
        /// <returns>
        /// The <see cref="SyncRecordConflictDO"/>.
        /// </returns>
        public SyncRecordConflictDO GetByTableAndEntityKey(SecurityClass security, string tableName, string entityKey, bool onlyUnresolved)
        {
            SyncRecordConflictDO syncProfile = null;

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
                syncProfile = dbi.GetByTableAndEntityKey(security, tableName, entityKey, onlyUnresolved);
            }

            return syncProfile;
        }

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>
		/// The <see cref="SyncRecordConflictCollection" />.
		/// </returns>
		public SyncRecordConflictCollection Enumerate(SecurityClass security)
        {
			return this.EnumerateExt(security);
        }

		/// <summary>
		/// The enumerate unresolved.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid">The sync node.</param>
		/// <returns>
		/// The <see cref="SyncRecordConflictCollection" />.
		/// </returns>
        public SyncRecordConflictCollection EnumerateUnresolved(SecurityClass security, Guid syncNodeGuid, Int64? maxRecords, Int64 startRowVersion)
        {
            return this.EnumerateUnresolvedExt(security, syncNodeGuid, maxRecords, startRowVersion);
        }

        /// <summary>
        /// The enumerate by status.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="conflictResolutionStatus">
        /// The conflict resolution status.
        /// </param>
        /// <param name="syncSessionLogGuid">
        /// The session detail GUID.
        /// </param>
        /// <returns>
        /// The <see cref="SyncRecordConflictCollection"/>.
        /// </returns>
        public SyncRecordConflictCollection EnumerateByStatus(SecurityClass security, SYNCCONFLICTRESOLUTIONSTATUS conflictResolutionStatus, Guid? syncSessionLogGuid)
        {
            return this.EnumerateByStatusExt(security, conflictResolutionStatus, syncSessionLogGuid);
        }

		/// <summary>
		/// The enumerate by session detail.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionLogGuid">
		/// The session detail GUID.
		/// </param>
		/// <returns>
		/// The <see cref="SyncRecordConflictCollection"/>.
		/// </returns>
		public SyncRecordConflictCollection EnumerateBySyncSessionLog(SecurityClass security, Guid syncSessionLogGuid, Int64? maxRecords, Int64 startRowVersion)
		{
			return this.EnumerateBySyncSessionLogExt(security, syncSessionLogGuid, maxRecords, startRowVersion);
		}
		
		/// <summary>
        /// The enumerate by session detail.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionScopeLogGuid">
        /// The session detail GUID.
        /// </param>
        /// <returns>
        /// The <see cref="SyncRecordConflictCollection"/>.
        /// </returns>
        public SyncRecordConflictCollection EnumerateBySyncSessionScopeLog(SecurityClass security, Guid syncSessionScopeLogGuid)
        {
            return this.EnumerateBySyncSessionScopeLogExt(security, syncSessionScopeLogGuid);
        }

		/// <summary>
		/// The enumerate ext.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>
		/// The <see cref="SyncRecordConflictCollection" />.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="System.Exception">Insufficient Rights</exception>
		/// <exception cref="ArgumentNullException">Throws an exception if the security context is null.</exception>
		/// <exception cref="Exception">Throws an exception if the user does not have <see cref="RIGHT.VIEW_SYNC_CONFLICT_STATUS" /> rights.</exception>
        [SecurityCritical]
        public SyncRecordConflictCollection EnumerateExt(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
            {
                throw new Exception("Insufficient Rights");
            }

            var syncConflicts = new SyncRecordConflictCollection();

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
				syncConflicts.AddRange(dbi.GetList(security, null));
            }

            return syncConflicts;
        }

		/// <summary>
		/// The enumerate unresolved ext.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid">The sync node.</param>
		/// <returns>
		/// The <see cref="SyncRecordConflictCollection" />.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="System.Exception">Insufficient Rights</exception>
		/// <exception cref="ArgumentNullException">Throws an exception if the security context is null.</exception>
		/// <exception cref="Exception">Throws an exception if the user does not have <see cref="RIGHT.VIEW_SYNC_CONFLICT_STATUS" /> rights.</exception>
        [SecurityCritical]
        public SyncRecordConflictCollection EnumerateUnresolvedExt(SecurityClass security, Guid syncNodeGuid, Int64? maxRecords, Int64 startRowVersion)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
            {
                throw new Exception("Insufficient Rights");
            }

            var syncConflicts = new SyncRecordConflictCollection();

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
                syncConflicts.AddRange(dbi.GetUnresolvedList(security, syncNodeGuid, maxRecords, startRowVersion));
            }

            return syncConflicts;
        }

        /// <summary>
        /// The enumerate by status ext.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="conflictResolutionStatus">
        /// The conflict resolution status.
        /// </param>
        /// <param name="syncSessionLogGuid">
        /// The session detail GUID.
        /// </param>
        /// <returns>
        /// The <see cref="SyncRecordConflictCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the security context is null.
        /// </exception>
        /// <exception cref="Exception">
        /// Throws an exception if the user does not have <see cref="RIGHT.VIEW_SYNC_CONFLICT_STATUS"/> rights.
        /// </exception>
        [SecurityCritical]
        public SyncRecordConflictCollection EnumerateByStatusExt(SecurityClass security, SYNCCONFLICTRESOLUTIONSTATUS conflictResolutionStatus, Guid? syncSessionLogGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
            {
                throw new Exception("Insufficient Rights");
            }

            var syncProfiles = new SyncRecordConflictCollection();

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
                syncProfiles.AddRange(dbi.GetByStatusList(security, conflictResolutionStatus, syncSessionLogGuid));
            }

            return syncProfiles;
        }
			/// <summary>
			/// The enumerate by sync session detail ext.
			/// </summary>
			/// <param name="security">
			/// The security.
			/// </param>
			/// <param name="syncSessionLogGuid">
			/// The session detail GUID.
			/// </param>
			/// <returns>
			/// The <see cref="SyncRecordConflictCollection"/>.
			/// </returns>
			/// <exception cref="ArgumentNullException">
			/// Throws an exception if the security context is null.
			/// </exception>
			/// <exception cref="Exception">
			/// Throws an exception if the user does not have <see cref="RIGHT.VIEW_SYNC_CONFLICT_STATUS"/> rights.
			/// </exception>
			[SecurityCritical]
			public SyncRecordConflictCollection EnumerateBySyncSessionLogExt(SecurityClass security, Guid syncSessionLogGuid, Int64? maxRecords, Int64 startRowVersion)
			{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				if (!security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
				{
					throw new Exception("Insufficient Rights");
				}

				var syncProfiles = new SyncRecordConflictCollection();

				using (var dbi = new SyncRecordConflictDBI(security.UserID))
				{
					syncProfiles.AddRange(dbi.GetSyncSessionLogList(security, syncSessionLogGuid, maxRecords, startRowVersion));
				}

				return syncProfiles;
			}

        /// <summary>
        /// The enumerate by sync session detail ext.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionScopeLogGuid">
        /// The session detail GUID.
        /// </param>
        /// <returns>
        /// The <see cref="SyncRecordConflictCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the security context is null.
        /// </exception>
        /// <exception cref="Exception">
        /// Throws an exception if the user does not have <see cref="RIGHT.VIEW_SYNC_CONFLICT_STATUS"/> rights.
        /// </exception>
        [SecurityCritical]
        public SyncRecordConflictCollection EnumerateBySyncSessionScopeLogExt(SecurityClass security, Guid syncSessionScopeLogGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
            {
                throw new Exception("Insufficient Rights");
            }

            var syncProfiles = new SyncRecordConflictCollection();

            using (var dbi = new SyncRecordConflictDBI(security.UserID))
            {
                syncProfiles.AddRange(dbi.GetSyncSessionScopeLogList(security, syncSessionScopeLogGuid));
            }

            return syncProfiles;
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
        private string ConvertToString(byte[] rowVersion)
        {
            string result = string.Empty;

            foreach (byte b in rowVersion)
            {
                result += b.ToString("X");
            }

            return result;
        }

        /// <summary>
        /// The get unresolved conflicts count.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="syncNodeGuid">The sync node.</param>
        /// <returns>
        /// The <see cref="long" />.
        /// </returns>
        public SyncRecordConflictCountDO GetUnresolvedConflictsCount(SecurityClass security, Guid? syncNodeGuid)
        {
            var syncRecordConflictCountDO = new SyncRecordConflictCountDO();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

			if (security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
			{

				using (var dbi = new SyncRecordConflictDBI(security.UserID))
				{
					syncRecordConflictCountDO = dbi.GetUnresolvedCount(security, syncNodeGuid);
				}
			}

            return syncRecordConflictCountDO;
        }


        #endregion Public Methods

        #region Validation

        /// <summary>
        /// Validates the <see cref="SyncRecordConflictDO"/> instance.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncRecordConflict">
        /// The sync record conflict.
        /// </param>
        /// <exception cref="Exception">
        /// Throws an exception if the table name or record key is missing from the <see cref="SyncRecordConflictDO"/> instance.
        /// </exception>
        private void Validate(SecurityClass security, SyncRecordConflictDO syncRecordConflict)
        {
            if (string.IsNullOrEmpty(syncRecordConflict.TableName))
            {
                throw new Exception("TableName required");
            }

            if (string.IsNullOrEmpty(syncRecordConflict.RecordKey))
            {
                throw new Exception("RecordKey required");
            }
        }

        #endregion Validation
    }
}
