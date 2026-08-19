// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncSessions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncSessionLogs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.InternalClasses.SyncClasses;

	/// <summary>
    /// Summary description for SyncSessionLogs
    /// </summary>
    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SyncSessionLogs : ISyncSessionLogs
    {
        public SyncSessionLogs()
        {
        }

        #region Public Methods

        /// <summary>
        /// Operation to add a new SyncSession master record version.
        /// </summary>
        /// <param name="security">
        /// The caller's security context.
        /// </param>
        /// <param name="syncSessionLog">
        /// An instance of the <see cref="SyncSessionLogDO"/> that should be added.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/> identity for the newly added session log.
        /// </returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SyncSessionLogDO syncSessionLog)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncSessionLog == null)
            {
                throw new ArgumentNullException("syncSessionLog");
            }

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncSessionLog);

            syncSessionLog.CreatedDate = DateTimeOffset.Now;
            syncSessionLog.CreatedBy = security.UserID;
            syncSessionLog.UpdatedDate = syncSessionLog.CreatedDate;
            syncSessionLog.UpdatedBy = security.UserID;

            using (var dbi = new SyncSessionLogDBI(security.UserID))
            {
                dbi.Save(security, syncSessionLog);
            }

            return syncSessionLog.IdentityGuid;
        }

        /// <summary>
        /// The modify.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionLog">
        /// The sync session.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SyncSessionLogDO syncSessionLog)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncSessionLog == null)
            {
                throw new ArgumentNullException("syncSessionLog");
            }

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw (new Exception("Insufficient Rights"));
            }

            this.Validate(security, syncSessionLog);

            syncSessionLog.UpdatedDate = DateTimeOffset.Now;
            syncSessionLog.UpdatedBy = security.UserID;

            using (var dbi = new SyncSessionLogDBI(security.UserID))
            {
                dbi.Save(security, syncSessionLog);
            }
        }

        /// <summary>
        /// This method deletes the record associated with the specified identity GUID.
        /// </summary>
        /// <param name="security">
        /// The caller's current security context.
        /// </param>
        /// <param name="syncSessionLogGuid">
        /// The identity GUID of the session log record to purge.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the passed in security context is null.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the passed in identity GUID contains a value of Guid.Empty or if the specified security 
        /// context does not have perform synchronization rights.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid syncSessionLogGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncSessionLogGuid == Guid.Empty)
            {
                throw new ArgumentNullException("syncSessionLogGuid");
            }

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            using (var dbi = new SyncSessionLogDBI(security.UserID))
            {
                SyncSessionLogDO syncSession = dbi.Get(security, syncSessionLogGuid);

                if (null == syncSession)
                {
                    throw new Exception("syncSession Not Found");
                }
                else
                {
                    if (syncSession.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("syncSession Not Found");
                    }
                }

                dbi.Delete(security, syncSession, true);
            }
        }

        /// <summary>
        /// This method populates an instance of a <see cref="SyncSessionLogDO"/> object using the passed in identity GUID.
        /// </summary>
        /// <param name="security">
        /// The caller's current security context
        /// </param>
        /// <param name="syncSessionLogGuid">
        /// The identity GUID of the session log record to retrieve.
        /// </param>
        /// <returns>
        /// If found, a populated instance of a <see cref="SyncSessionLogDO"/> that represents the record associated with the specified GUID.
        /// </returns>
        public SyncSessionLogDO Get(SecurityClass security, Guid syncSessionLogGuid)
        {
            SyncSessionLogDO syncSessionLog = null;

            using (var dbi = new SyncSessionLogDBI(security.UserID))
            {
                syncSessionLog = dbi.Get(security, syncSessionLogGuid);
            }

            return syncSessionLog;
        }

        /// <summary>
        /// The get last sync date time.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>DateTimeOffset?</cref>
        ///     </see>
        ///     .
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1125:UseShorthandForNullableTypes", Justification = "Reviewed. Suppression is OK here.")]
        public System.Nullable<DateTimeOffset> GetLastSyncDateTime(SecurityClass security)
        {
            SyncSessionLogDO syncSessionLog = null;

            using (var dbi = new SyncSessionLogDBI(security.UserID))
            {
                syncSessionLog = dbi.GetLastSyncSession(security);
            }

            return (null != syncSessionLog) ? syncSessionLog.EndDate : null;
        }

        /// <summary>
        /// Returns a dictionary of unique remote nodes from the Synchronization Session Logs
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// List of remote node machine names from the synchronization logs
        /// </returns>
        public Dictionary<Guid,string> GetRemoteNodes(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            Dictionary<Guid, string> nodeDictionary;

            using (var dbi = new SyncSessionLogDBI(security.UserID))
            {
				nodeDictionary = dbi.GetRemoteNodes(security);
            }

			return nodeDictionary;
        }

	    public DataSet GetNodeHealthSummary(SecurityClass security, int nodeStatus)
	    {
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataSet ds = null;

		    using (var dbi = new SyncSessionLogDBI(security.UserID))
		    {
			    ds = dbi.GetNodeHealthSummary(security, nodeStatus);
		    }

		    return ds;
	    }

	    public DataSet GetNodeHealthSummaryWithOrder(SecurityClass security, string order, int nodeStatus)
	    {
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(order))
			{
				return this.GetNodeHealthSummary(security, nodeStatus);
			}

			DataSet ds = null;

			using (var dbi = new SyncSessionLogDBI(security.UserID))
			{
				ds = dbi.GetNodeHealthWithOrderSummary(security, order, nodeStatus);
			}

			return ds;
		}

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid">The node.</param>
		/// <param name="startDateTimeOffset">The start date time offset.</param>
		/// <param name="endDateTimeOffset">The end date time offset.</param>
		/// <returns>
		/// The <see cref="SyncSessionLogCollection" />.
		/// </returns>
        public SyncSessionLogCollection Enumerate(SecurityClass security, Guid syncNodeGuid, DateTimeOffset? startDateTimeOffset, DateTimeOffset? endDateTimeOffset, bool? withConflicts)
        {
			return this.EnumerateExt(security, syncNodeGuid, startDateTimeOffset, endDateTimeOffset, false, withConflicts);
        }

		/// <summary>
		/// The enumerate active.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid">The node.</param>
		/// <returns>
		/// The <see cref="SyncSessionLogCollection" />.
		/// </returns>
		public SyncSessionLogCollection EnumerateActive(SecurityClass security, Guid syncNodeGuid)
        {
			return this.EnumerateExt(security, syncNodeGuid, null, null, true, null);
        }

		/// <summary>
		/// The enumerate ext.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid">The node.</param>
		/// <param name="startDateTimeOffset">The start date time offset.</param>
		/// <param name="endDateTimeOffset">The end date time offset.</param>
		/// <param name="onlyActiveFlag">The only active flag.</param>
		/// <param name="withConflicts">The with conflicts.</param>
		/// <returns>
		/// The <see cref="SyncSessionLogCollection" />.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="System.Exception">Insufficient Rights</exception>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
        [SecurityCritical]
        public SyncSessionLogCollection EnumerateExt(SecurityClass security, Guid syncNodeGuid, DateTimeOffset? startDateTimeOffset, DateTimeOffset? endDateTimeOffset, bool? onlyActiveFlag, bool? withConflicts)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw new Exception("Insufficient Rights");
            }

            var syncSessionLogs = new SyncSessionLogCollection();

            using (var dbi = new SyncSessionLogDBI(security.UserID))
            {
                if (onlyActiveFlag.HasValue && !onlyActiveFlag.Value)
                {
                    syncSessionLogs.AddRange(dbi.GetList(security, syncNodeGuid, startDateTimeOffset, endDateTimeOffset, withConflicts));
                }
                else
                {
                    syncSessionLogs.AddRange(dbi.GetActiveSessionList(security));
                }
            }

            return syncSessionLogs;
        }

        public void CloseActiveSessions(SecurityClass security)
        {
            using (var dbi = new SyncSessionLogDBI(security.UserID))
            {
                dbi.CleanupActiveSessions(security);
            }

            return;
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

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncSessionLog">
        /// The sync session.
        /// </param>
        private void Validate(SecurityClass security, SyncSessionLogDO syncSessionLog)
        {
        }

        #endregion Validation
    }
}
