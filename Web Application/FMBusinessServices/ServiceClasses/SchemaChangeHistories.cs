// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaChangeHistories.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SchemaChangeHistories
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

    /// <summary>
    /// Summary description for SchemaChangeHistories
    /// </summary>
    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SchemaChangeHistories : ISchemaChangeHistories
    {
        public SchemaChangeHistories()
        {
        }

        #region Public Methods

        /// <summary>
        /// Operation to add a new SchemaChangeHistory master record version.
        /// </summary>
        /// <param name="security">
        /// Current security context of the calling method.
        /// </param>
        /// <param name="schemaChangeHistory">
        /// An instance of the <see cref="SchemaChangeHistoryDO"/> record that should be added.
        /// </param>
        /// <returns>
        /// The primary key <see cref="Guid"/> of the record that was just inserted.
        /// </returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SchemaChangeHistoryDO schemaChangeHistory)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (schemaChangeHistory == null)
            {
                throw new ArgumentNullException("schemaChangeHistory");
            }

            this.Validate(security, schemaChangeHistory);

            schemaChangeHistory.CreatedDate = DateTimeOffset.Now;
            schemaChangeHistory.CreatedBy = security.UserID;
            schemaChangeHistory.UpdatedDate = schemaChangeHistory.CreatedDate;
            schemaChangeHistory.UpdatedBy = security.UserID;

            using (var dbi = new SchemaChangeHistoryDBI(security.UserID))
            {
                dbi.Save(security, schemaChangeHistory);
            }

            return schemaChangeHistory.IdentityGuid;
        }

        /// <summary>
        /// This method utilizes a merge stored procedure to either insert or update the database depending on whether or not the specified record
        /// existed.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="schemaChangeHistory">
        /// An instance of the <see cref="SchemaChangeHistoryDO"/> object to modify.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// An exception will be thrown if the passed in <see cref="SecurityClass"/> is null or the <see cref="SchemaChangeHistoryDO"/> instance is null.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SchemaChangeHistoryDO schemaChangeHistory)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (schemaChangeHistory == null)
            {
                throw new ArgumentNullException("schemaChangeHistory");
            }

            this.Validate(security, schemaChangeHistory);

            schemaChangeHistory.UpdatedDate = DateTimeOffset.Now;
            schemaChangeHistory.UpdatedBy = security.UserID;

            using (var dbi = new SchemaChangeHistoryDBI(security.UserID))
            {
                dbi.Save(security, schemaChangeHistory);
            }
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="identityGuid">
        /// The primary key <see cref="Guid"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// An exception will be thrown if the passed in <see cref="SecurityClass"/> is null.
        /// </exception>
        /// <exception cref="Exception">
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

            using (var dbi = new SchemaChangeHistoryDBI(security.UserID))
            {
                SchemaChangeHistoryDO schemaChangeHistory = dbi.Get(security, identityGuid);

                if (null == schemaChangeHistory)
                {
                    throw new Exception("SchemaChangeHistory Not Found");
                }
                else
                {
                    if (schemaChangeHistory.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("SchemaChangeHistory Not Found");
                    }
                }

                dbi.Delete(security, schemaChangeHistory, true);
            }
        }

        /// <summary>
        /// Gets a copy of the <see cref="SchemaChangeHistoryDO"/> record that applies to the specified version number.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="versionNumber">
        /// A string representation of the version.
        /// </param>
        /// <returns>
        /// The <see cref="SchemaChangeHistoryDO"/>.
        /// </returns>
        public SchemaChangeHistoryDO GetByVersion(SecurityClass security, string versionNumber)
        {
            SchemaChangeHistoryDO schemaChange = null;

            using (var dbi = new SchemaChangeHistoryDBI(security.UserID))
            {
                schemaChange = dbi.Get(security, versionNumber);
            }

            return schemaChange;
        }

        /// <summary>
        /// Gets a copy of the <see cref="SchemaChangeHistoryDO"/> record that applies to the specified primary key.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="identityGuid">
        /// The primary key <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="SchemaChangeHistoryDO"/>.
        /// </returns>
        public SchemaChangeHistoryDO Get(SecurityClass security, Guid identityGuid)
        {
            SchemaChangeHistoryDO schemaChange = null;

            using (var dbi = new SchemaChangeHistoryDBI(security.UserID))
            {
                schemaChange = dbi.Get(security, identityGuid);
            }

            return schemaChange;
        }

        /// <summary>
        /// The enumerate.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="SchemaChangeHistoryCollection"/>.
        /// </returns>
        public SchemaChangeHistoryCollection Enumerate(SecurityClass security)
        {
            return this.EnumerateExt(security);
        }

        /// <summary>
        /// The enumerate ext.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="SchemaChangeHistoryCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An exception will be thrown if the passed in <see cref="SecurityClass"/> is null.
        /// </exception>
        [SecurityCritical]
        public SchemaChangeHistoryCollection EnumerateExt(SecurityClass security, int limit = 0)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            var versions = new SchemaChangeHistoryCollection();

            using (var dbi = new SchemaChangeHistoryDBI(security.UserID))
            {
                versions.AddRange(dbi.GetList(security));
            }

            return versions;
        }

/*
        /// <summary>
        /// The convert to string.
        /// </summary>
        /// <param name="rowSchemaChangeHistory">
        /// The row version.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ConvertToString(System.Byte[] rowSchemaChangeHistory)
        {
            string result = string.Empty;

            foreach (byte b in rowSchemaChangeHistory)
            {
                result += b.ToString("X");
            }

            return result;
        }
*/
        #endregion Public Methods

        #region Validation

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        private void Validate(SecurityClass security, SchemaChangeHistoryDO version)
        {
        }

        #endregion Validation
    }
}
