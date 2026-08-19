
namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Security;
    using System.ServiceModel;
    using System.Transactions;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.InternalClasses;

    /// <summary>
    /// Summary description for SyncServerConfigurations
    /// </summary>
    [SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class SyncServerConfigurations : ISyncServerConfigurations
    {
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        #region Public Methods

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncServerConfiguration">
        /// The sync server configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, SyncServerConfigurationDO syncServerConfiguration)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncServerConfiguration == null)
            {
                throw new ArgumentNullException("syncServerConfiguration");
            }

            if (!security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncServerConfiguration);

            syncServerConfiguration.CreatedDate = DateTimeOffset.Now;
            syncServerConfiguration.CreatedBy = security.UserID;
            syncServerConfiguration.UpdatedDate = syncServerConfiguration.CreatedDate;
            syncServerConfiguration.UpdatedBy = security.UserID;

            using (var dbi = new SyncServerConfigurationDBI(security.UserID))
            {
                dbi.Save(security, syncServerConfiguration);
            }

            return syncServerConfiguration.IdentityGuid;
        }

        /// <summary>
        /// The modify.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncServerConfiguration">
        /// The sync server configuration.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, SyncServerConfigurationDO syncServerConfiguration)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (syncServerConfiguration == null)
            {
                throw new ArgumentNullException("syncServerConfiguration");
            }

            if (!security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS))
            {
                throw new Exception("Insufficient Rights");
            }

            this.Validate(security, syncServerConfiguration);

            syncServerConfiguration.UpdatedDate = DateTimeOffset.Now;
            syncServerConfiguration.UpdatedBy = security.UserID;

            using (var dbi = new SyncServerConfigurationDBI(security.UserID))
            {
                dbi.Save(security, syncServerConfiguration);
            }
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncServerConfigurationGuid">
        /// The sync server configuration guid.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid syncServerConfigurationGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS))
            {
                throw new Exception("Insufficient Rights");
            }

            SyncServerConfigurationDO syncServerConfiguration = null;

            using (var dbi = new SyncServerConfigurationDBI(security.UserID))
            {
                syncServerConfiguration = dbi.Get(security, syncServerConfigurationGuid);

                if (null == syncServerConfiguration)
                {
                    throw new Exception("SyncServerConfiguration Not Found");
                }
                else
                {
                    if (syncServerConfiguration.IdentityGuid == Guid.Empty)
                    {
                        throw new Exception("SyncServerConfiguration Not Found");
                    }
                }

                dbi.Delete(security, syncServerConfiguration, true);
            }
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="SyncServerConfigurationDO"/>.
        /// </returns>
        public SyncServerConfigurationDO Get(SecurityClass security)
        {
            SyncServerConfigurationDO syncServerConfiguration = null;

            using (var dbi = new SyncServerConfigurationDBI(security.UserID))
            {
                syncServerConfiguration = dbi.Get(security);
            }

            if (null == syncServerConfiguration)
            {
                syncServerConfiguration = new SyncServerConfigurationDO();
            }

            return syncServerConfiguration;
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

        private void Validate(SecurityClass pSecurity, SyncServerConfigurationDO pSyncServerConfiguration)
        {
        }

        #endregion Validation
    }
}
