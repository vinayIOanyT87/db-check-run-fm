// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncClientConfigurations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncClientConfigurations
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
    /// Summary description for SyncClientConfigurations
    /// </summary>
    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SyncClientConfigurations : ISyncClientConfigurations
    {
        #region Attributes
        #endregion Attributes

        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        public SyncClientConfigurations()
        {
        }

        #region Public Methods

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncClientConfiguration">
        /// The sync client configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SyncClientConfigurationDO syncClientConfiguration)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncClientConfiguration == null)
            {
                throw new ArgumentNullException("syncClientConfiguration");
            }

            if (!security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncClientConfiguration);

            syncClientConfiguration.SyncNodeGuid = SyncDBI.GetServerNodeID(security);

            syncClientConfiguration.CreatedDate = DateTimeOffset.Now;
            syncClientConfiguration.CreatedBy = security.UserID;
            syncClientConfiguration.UpdatedDate = syncClientConfiguration.CreatedDate;
            syncClientConfiguration.UpdatedBy = security.UserID;

            using (var dbi = new SyncClientConfigurationDBI(security.UserID))
            {
                dbi.Save(security, syncClientConfiguration);
            }

            return syncClientConfiguration.IdentityGuid;
        }

        /// <summary>
        /// The modify.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncClientConfiguration">
        /// The sync client configuration.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SyncClientConfigurationDO syncClientConfiguration)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncClientConfiguration == null)
            {
                throw new ArgumentNullException("syncClientConfiguration");
            }

            if (!security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncClientConfiguration);

            syncClientConfiguration.UpdatedDate = DateTimeOffset.Now;
            syncClientConfiguration.UpdatedBy = security.UserID;

            using (var dbi = new SyncClientConfigurationDBI(security.UserID))
            {
                dbi.Save(security, syncClientConfiguration);
            }
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncClientConfigurationGuid">
        /// The sync client configuration guid.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid syncClientConfigurationGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS))
            {
                throw new Exception("Insufficient Rights");
            }

            SyncClientConfigurationDO syncClientConfiguration = null;

            using (var dbi = new SyncClientConfigurationDBI(security.UserID))
            {
                syncClientConfiguration = dbi.Get(security, syncClientConfigurationGuid);

                if (null == syncClientConfiguration)
                {
                    throw new Exception("SyncClientConfiguration Not Found");
                }
                else
                {
                    if (syncClientConfiguration.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("SyncClientConfiguration Not Found");
                    }
                }

                dbi.Delete(security, syncClientConfiguration, true);
            }
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="SyncClientConfigurationDO"/>.
        /// </returns>
        public SyncClientConfigurationDO Get(SecurityClass security)
        {
            SyncClientConfigurationDO syncClientConfiguration = null;

            using (var dbi = new SyncClientConfigurationDBI(security.UserID))
            {
                syncClientConfiguration = dbi.Get(security);
            }

            if (null == syncClientConfiguration)
            {
                syncClientConfiguration = new SyncClientConfigurationDO();
            }

            return syncClientConfiguration;
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

        private void Validate(SecurityClass security, SyncClientConfigurationDO syncClientConfiguration)
        {
            if (syncClientConfiguration.RootSiteID == "SiteAdmin")
                throw new Exception("Invalid Site or Site Group ID [" + syncClientConfiguration.RootSiteID + "]");

            if (syncClientConfiguration.EnterpriseURL.Length > 512)
                throw new Exception("EntepriseURL Exceeded max length (512)");
        }

        #endregion Validation
    }
}
