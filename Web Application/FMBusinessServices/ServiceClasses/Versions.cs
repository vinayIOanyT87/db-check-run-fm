// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Versions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for Versions
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
    /// Summary description for Versions
    /// </summary>
    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class Versions : IVersions
    {
        public Versions()
        {
        }

        #region Public Methods
        /// <summary>
        /// This method utilizes an update stored procedure to update a limited number of columns in the database.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="version">
        /// An instance of the <see cref="VersionDO"/> object to modify.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// An exception will be thrown if the passed in <see cref="SecurityClass"/> is null or the <see cref="VersionDO"/> instance is null.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, VersionDO version)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            this.Validate(security, version);

            version.UpdatedDate = DateTimeOffset.Now;
            version.UpdatedBy = security.UserID;

            using (var dbi = new VersionDBI(security.UserID))
            {
                dbi.Save(security, version);
            }
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="versionGuid">
        /// The primary key <see cref="Guid"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// An exception will be thrown if the passed in <see cref="SecurityClass"/> is null or the identity <see cref="Guid"/> is empty.
        /// </exception>
        /// <exception cref="Exception">
        /// An exception will be thrown if we are unable to locate the specified Version record.
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid versionGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (versionGuid == Guid.Empty)
            {
                throw new ArgumentNullException("versionGuid");
            }

            using (var dbi = new VersionDBI(security.UserID))
            {
                VersionDO version = dbi.Get(security, versionGuid);

                if (null == version)
                {
                    throw new Exception("Version Not Found");
                }
                else
                {
                    if (version.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("Version Not Found");
                    }
                }

                dbi.Delete(security, version, true);
            }
        }

        /// <summary>
        /// The get current.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="VersionDO"/>.
        /// </returns>
        public VersionDO GetCurrent(SecurityClass security)
        {
            VersionDO version = null;

            using (var dbi = new VersionDBI(security.UserID))
            {
                version = dbi.GetCurrentVersion(security);
            }

            return version;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="versionGuid">
        /// The primary key <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="VersionDO"/>.
        /// </returns>
        public VersionDO Get(SecurityClass security, Guid versionGuid)
        {
            VersionDO version = null;

            using (var dbi = new VersionDBI(security.UserID))
            {
                version = dbi.Get(security, versionGuid);
            }

            return version;
        }

        /// <summary>
        /// The enumerate.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="VersionCollection"/>.
        /// </returns>
        public VersionCollection Enumerate(SecurityClass security)
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
        /// The <see cref="VersionCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SecurityCritical]
        public VersionCollection EnumerateExt(SecurityClass security, int limit = 0)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            var versions = new VersionCollection();

            using (var dbi = new VersionDBI(security.UserID))
            {
                versions.AddRange(dbi.GetList(security));
            }

            return versions;
        }

/*
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
        private void Validate(SecurityClass security, VersionDO version)
        {
        }

        #endregion Validation
    }
}
