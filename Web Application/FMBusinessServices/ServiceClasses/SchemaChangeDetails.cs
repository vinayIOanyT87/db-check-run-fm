// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaChangeDetails.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SchemaChangeDetails
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
    /// Summary description for SchemaChangeDetails
    /// </summary>
    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SchemaChangeDetails : ISchemaChangeDetails
    {
        public SchemaChangeDetails()
        {
        }

        #region Public Methods

        /// <summary>
        /// Operation to add a new <see cref="SchemaChangeDetailDO"/> master record version.
        /// </summary>
        /// <param name="security">
        /// Current security context of the calling method.
        /// </param>
        /// <param name="schemaChangeDetail">
        /// An instance of the <see cref="SchemaChangeDetailDO"/> record that should be added.
        /// </param>
        /// <returns>
        /// The primary key <see cref="Guid"/> of the record that was just inserted.
        /// </returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SchemaChangeDetailDO schemaChangeDetail)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (schemaChangeDetail == null)
            {
                throw new ArgumentNullException("schemaChangeDetail");
            }

            this.Validate(security, schemaChangeDetail);

            schemaChangeDetail.CreatedDate = DateTimeOffset.Now;
            schemaChangeDetail.CreatedBy = security.UserID;
            schemaChangeDetail.UpdatedDate = schemaChangeDetail.CreatedDate;
            schemaChangeDetail.UpdatedBy = security.UserID;

            using (var dbi = new SchemaChangeDetailDBI(security.UserID))
            {
                dbi.Save(security, schemaChangeDetail);
            }

            return schemaChangeDetail.IdentityGuid;
        }

        /// <summary>
        /// This method utilizes a merge stored procedure to either insert or update the database depending on whether or not the specified record
        /// existed.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="schemaChangeDetail">
        /// An instance of the <see cref="SchemaChangeDetailDO"/> object to modify.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// An exception will be thrown if the passed in <see cref="SecurityClass"/> is null or the <see cref="SchemaChangeDetailDO"/> instance is null.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SchemaChangeDetailDO schemaChangeDetail)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (schemaChangeDetail == null)
            {
                throw new ArgumentNullException("schemaChangeDetail");
            }

            this.Validate(security, schemaChangeDetail);

            schemaChangeDetail.UpdatedDate = DateTimeOffset.Now;
            schemaChangeDetail.UpdatedBy = security.UserID;

            using (var dbi = new SchemaChangeDetailDBI(security.UserID))
            {
                dbi.Save(security, schemaChangeDetail);
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
        /// An exception will be thrown if the passed in <see cref="SecurityClass"/> is null or the identity <see cref="Guid"/> is empty.
        /// </exception>
        /// <exception cref="Exception">
        /// An exception will be thrown if we are unable to locate the specified Schema Change Detail record.
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

            using (var dbi = new SchemaChangeDetailDBI(security.UserID))
            {
                SchemaChangeDetailDO schemaChangeDetail = dbi.Get(security, identityGuid);

                if (null == schemaChangeDetail)
                {
                    throw new Exception("SchemaChangeDetail Not Found");
                }
                else
                {
                    if (schemaChangeDetail.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("SchemaChangeDetail Not Found");
                    }
                }

                dbi.Delete(security, schemaChangeDetail, true);
            }
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="identityGuid">
        /// The primary key <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="SchemaChangeDetailDO"/>.
        /// </returns>
        public SchemaChangeDetailDO Get(SecurityClass security, Guid identityGuid)
        {
            SchemaChangeDetailDO schemaChange = null;

            using (var dbi = new SchemaChangeDetailDBI(security.UserID))
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
        /// <param name="schemaChangeHistoryGuid">
        /// The schema Change History <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="SchemaChangeDetailCollection"/>.
        /// </returns>
        public SchemaChangeDetailCollection Enumerate(SecurityClass security, Guid schemaChangeHistoryGuid)
        {
            return this.EnumerateExt(security, schemaChangeHistoryGuid);
        }

        /// <summary>
        /// The enumerate ext.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="schemaChangeHistoryGuid">
        /// The schema Change History <see cref="Guid"/>.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="SchemaChangeDetailCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An exception will be thrown if the passed in <see cref="SecurityClass"/> is null.
        /// </exception>
        [SecurityCritical]
        public SchemaChangeDetailCollection EnumerateExt(SecurityClass security, Guid schemaChangeHistoryGuid, int limit = 0)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            var versions = new SchemaChangeDetailCollection();

            using (var dbi = new SchemaChangeDetailDBI(security.UserID))
            {
                versions.AddRange(dbi.GetList(security, schemaChangeHistoryGuid));
            }

            return versions;
        }

/*
        /// <summary>
        /// The convert to string.
        /// </summary>
        /// <param name="rowSchemaChangeDetail">
        /// The row version.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ConvertToString(System.Byte[] rowSchemaChangeDetail)
        {
            string result = string.Empty;

            foreach (byte b in rowSchemaChangeDetail)
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
        private void Validate(SecurityClass security, SchemaChangeDetailDO version)
        {
        }

        #endregion Validation
    }
}
