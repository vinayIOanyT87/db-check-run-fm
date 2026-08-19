// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationDataExportImportLog.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for MigrationDataExportImportLog
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Linq;
    using System.Security;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.InternalClasses;

    /// <summary>
    /// The migration data export import log.
    /// </summary>
    [SecuritySafeCriticalAttribute]
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class MigrationDataExportImportLog : IMigrationDataExportImportLog
    {
        #region Public Methods

        /// <summary>
        /// Operation to add a new <see cref="MigrationDataExportImportLogDO"/> record.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="dataObject">
        /// The data object.
        /// </param>
        /// <returns>
        /// The newly assigned <see cref="Guid"/> identity value for the record.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if any of the incoming parameters are null.
        /// </exception>
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, MigrationDataExportImportLogDO dataObject)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (dataObject == null)
            {
                throw new ArgumentNullException("dataObject");
            }

            this.Validate(security, dataObject);

            dataObject.CreatedDate = DateTimeOffset.Now;
            dataObject.CreatedBy = security.UserID;
            dataObject.UpdatedDate = dataObject.CreatedDate;
            dataObject.UpdatedBy = security.UserID;

            using (var dbi = new MigrationExportImportLogDBI(security.UserID))
            {
                dbi.Save(security, dataObject);
            }

            return dataObject.IdentityGuid;
        }

        /// <summary>
        /// Method to modify an existing data object.
        /// </summary>
        /// <param name="security">
        /// The current security context
        /// </param>
        /// <param name="dataObject">
        /// The migration import export data object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if any of the input parameters are null.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, MigrationDataExportImportLogDO dataObject)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (dataObject == null)
            {
                throw new ArgumentNullException("dataObject");
            }

            this.Validate(security, dataObject);

            dataObject.UpdatedDate = DateTimeOffset.Now;
            dataObject.UpdatedBy = security.UserID;

            using (var dbi = new MigrationExportImportLogDBI(security.UserID))
            {
                dbi.Save(security, dataObject);
            }
        }

        /// <summary>
        /// Purges the specified record from the database.
        /// </summary>
        /// <param name="security">
        /// The current security context
        /// </param>
        /// <param name="identityGuid">
        /// Unique identifier of the log entry to purge.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if any of the input parameters are null.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid identityGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (identityGuid == Guid.Empty)
            {
                throw new ArgumentNullException("identityGuid");
            }

            using (var dbi = new MigrationExportImportLogDBI(security.UserID))
            {
                MigrationDataExportImportLogDO migrationImportExportHistory = dbi.Get(security, identityGuid);

                if (null == migrationImportExportHistory)
                {
                    throw new Exception("Migration data export import log entry not found");
                }
                else
                {
                    if (migrationImportExportHistory.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("Migration data export import log entry not found");
                    }
                }

                dbi.Delete(security, migrationImportExportHistory, true);
            }
        }

        /// <summary>
        /// Method to get an instance of a log record that matches the specified identity.
        /// </summary>
        /// <param name="security">
        /// The current security context
        /// </param>
        /// <param name="identityGuid">
        /// The migration import export history GUID.
        /// </param>
        /// <returns>
        /// An instance of a populated <see cref="MigrationDataExportImportLogDO"/> object.
        /// </returns>
        public MigrationDataExportImportLogDO Get(SecurityClass security, Guid identityGuid)
        {
            MigrationDataExportImportLogDO migrationImportExportHistoryDo = null;

            using (var dbi = new MigrationExportImportLogDBI(security.UserID))
            {
                migrationImportExportHistoryDo = dbi.Get(security, identityGuid);
            }

            return migrationImportExportHistoryDo;
        }

        /// <summary>
        /// Enumerates all migration export and import history.
        /// </summary>
        /// <param name="security">
        /// The current security context
        /// </param>
        /// <returns>
        /// The <see cref="MigrationDataExportImportLogCollection"/>.
        /// </returns>
        public MigrationDataExportImportLogCollection Enumerate(SecurityClass security)
        {
            return this.EnumerateExt(security, null);
        }

        /// <summary>
        /// Enumerates the migration export and import history for a specific Site.
        /// </summary>
        /// <param name="security">
        /// The current security context
        /// </param>
        /// <param name="siteGuid">
        /// Indicates the identify of the Site for which the export / import log history should be returned for.
        /// </param>
        /// <returns>
        /// The <see cref="MigrationDataExportImportLogCollection"/>.
        /// </returns>
        public MigrationDataExportImportLogCollection EnumerateBySiteGuid(SecurityClass security, Guid siteGuid)
        {
            return this.EnumerateExt(security, siteGuid);
        }

        /// <summary>
        /// Enumerate extension method that can enumerate ALL migration export and import history or restrict the results to a specific Site.
        /// </summary>
        /// <param name="security">
        /// The current security context
        /// </param>
        /// <param name="siteGuid">
        /// An optional parameter that can be used to restrict the migration export / import log history to a specific Site.
        /// </param>
        /// <param name="limit">
        /// Indicates the maximum number of records to return.
        /// </param>
        /// <returns>
        /// An instance of a <see cref="MigrationDataExportImportLogCollection"/> collection containing zero or more records that matched the specified criteria.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if no security context is provided.
        /// </exception>
        [SecurityCritical]
        public MigrationDataExportImportLogCollection EnumerateExt(SecurityClass security, Guid? siteGuid, int limit = 0)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            var list = new MigrationDataExportImportLogCollection();

            using (var dbi = new MigrationExportImportLogDBI(security.UserID))
            {
                if (siteGuid.HasValue)
                {
                    list.AddRange(dbi.GetListBySiteGuid(security, siteGuid.Value));
                }
                else
                {
                    list.AddRange(dbi.GetList(security));
                }
            }

            return list;
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
            return rowVersion.Aggregate(string.Empty, (current, b) => current + b.ToString("X"));
        }

        #endregion Public Methods

        #region Validation

        /// <summary>
        /// Validates the passed in <see cref="MigrationDataExportImportLogDO"/> data object.
        /// </summary>
        /// <param name="security">
        /// The current <see cref="SecurityClass"/> context.
        /// </param>
        /// <param name="dataObject">
        /// The data object to validate.
        /// </param>
        private void Validate(SecurityClass security, MigrationDataExportImportLogDO dataObject)
        {
        }

        #endregion Validation
    }
}
