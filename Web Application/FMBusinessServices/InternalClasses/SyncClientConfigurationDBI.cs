namespace FMBusinessServices.InternalClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.InternalClasses.SyncClasses;

	/// <summary>
    /// Summary description for SyncClientConfigurationDBI.
    /// </summary>
    public class SyncClientConfigurationDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SyncClientConfigurationDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods
        /// <summary>
        /// Gets the current <seealso cref="SyncClientConfigurationDO"/> record.
        /// </summary>
        /// <param name="pSecurity">Contains security credentials</param>
        /// <returns>An instance of the current SyncClientConfigurationDO settings.</returns>
        public SyncClientConfigurationDO Get(SecurityClass pSecurity)
        {
            return (GetEx(pSecurity, null));
        }
        /// <summary>
        /// Gets the current <seealso cref="SyncClientConfigurationDO"/> record.
        /// </summary>
        /// <param name="pSecurity">Contains security credentials</param>
        /// <returns>An instance of the current SyncClientConfigurationDO settings.</returns>
        public SyncClientConfigurationDO Get(SecurityClass pSecurity, Guid pIdentityGuid)
        {
            if (pIdentityGuid == Guid.Empty)
                throw new Exception("Identity parameter cannot be empty.");

            return (GetEx(pSecurity, pIdentityGuid));
        }
        /// <summary>
        /// Saves the passed in <seealso cref="SyncClientConfigurationDO"/> record.
        /// </summary>
        /// <param name="pSecurity">Contains security credentials</param>
        /// <param name="pDataObject">The client synchronization configuration settings to persist.</param>
        /// <returns>True unless an exception is encountered.</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass pSecurity, SyncClientConfigurationDO pDataObject)
        {
            // Save the dataobject using a merge implementation
            Upsert(pSecurity, pDataObject);

            return (true);
        }
        /// <summary>
        /// Deletes the passed in <seealso cref="SyncClientConfigurationDO"/> record.
        /// </summary>
        /// <param name="pSecurity">Contains pSecurity credentials</param>
        /// <param name="pDataObject">The synchronization dependency group information to delete.</param>
        /// <returns>True unless an exception is encountered.</returns>
        public bool Delete(SecurityClass pSecurity, SyncClientConfigurationDO pDataObject, bool pPurge)
        {
            try
            {
                // Save the dataobject using a merge implementation
                Delete(pSecurity, pDataObject);

                return (true);
            }
            catch (Exception eX)
            {
                throw (eX);
            }
        }
        #endregion Public Data Access Methods

        #region Private Persistence Methods
        /// <summary>
        /// Gets the current <seealso cref="SyncClientConfigurationDO"/> from the database by calling the internal Load method and processing the returned dataset.
        /// </summary>
        /// <param name="pSecurity">Contains pSecurity credentials</param>
        /// <param name="pIdentityGuid">Contains the identity guid of a specific record to load.  If null is passed in, then the first available record is loaded.</param>
        /// <returns>If client configuration settings were found, returns a fully populated <seealso cref="SyncClientConfigurationDO"/>, otherwise; null.</returns>
        /// <remarks>Since the Server Node ID is considered to be applicable to Client and Server settings this method will include this in the returned instance.</remarks>
        private SyncClientConfigurationDO GetEx(SecurityClass pSecurity, System.Nullable<Guid> pIdentityGuid)
        {
            DataSet ds = Load(pSecurity, pIdentityGuid);

            if (ds.Tables[0].Rows.Count == 0)
                return (null);

            DataRow row = ds.Tables[0].Rows[0];
            SyncClientConfigurationDO syncClientConfiguration = GetDataObjectFromDataRow(row);

            return (syncClientConfiguration);
        }
        /// <summary>
        /// Loads the current <seealso cref="SyncClientConfigurationDO"/> from the database.
        /// </summary>
        /// <param name="pSecurity">Contains pSecurity credentials</param>
        /// <param name="pIdentityGuid">Contains the identity guid of a specific record to load.  If null is passed in, then the first available record is loaded.</param>
        /// <returns>Returns a DataSet</returns>
        private DataSet Load(SecurityClass pSecurity, System.Nullable<Guid> pIdentityGuid)
        {
            DataSet ds = null;

            using (var cmd = PrepareSelectStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(pIdentityGuid);

                ds = ConsolidatedDA.GetDataSet(cmd, pSecurity);
            }

            return (ds);
        }
        /// <summary>
        /// Updates an existing record with the current <seealso cref="SyncClientConfigurationDO"/> changes.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="pSecurity">Contains security credentials</param>
        /// <param name="pDataObject">The client synchronization configuration settings to persist.</param>
        private void Upsert(SecurityClass pSecurity, SyncClientConfigurationDO pDataObject)
        {
            using (var cmd = PrepareUpsertStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = (pDataObject.IdentityGuid == Guid.Empty) ? (object)DBNull.Value : pDataObject.IdentityGuid;
                cmd.Parameters["@RootSiteID"].Value = pDataObject.RootSiteID;
                cmd.Parameters["@EnterpriseURL"].Value = pDataObject.EnterpriseURL;
                cmd.Parameters["@SuspendSynchronizationFlag"].Value = pDataObject.SuspendSynchronizationFlag;
                cmd.Parameters["@ServerAuthUserName"].Value = pDataObject.ServerAuthUserName;
                cmd.Parameters["@ServerAuthPassword"].Value = UserClass.encode(pDataObject.ServerAuthPassword, Guids.SiteAdminGuid);
                cmd.Parameters["@ServerAuthDomain"].Value = pDataObject.ServerAuthDomain;
                cmd.Parameters["@ServerAuthClientCertificate"].Value = pDataObject.ServerAuthClientCertificate;
                cmd.Parameters["@FMAuthUserName"].Value = pDataObject.FMAuthUserName;
                cmd.Parameters["@FMAuthPassword"].Value = UserClass.encode(pDataObject.FMAuthPassword, Guids.SiteAdminGuid);
                cmd.Parameters["@FMAuthClientCertificate"].Value = pDataObject.FMAuthClientCertificate;
                cmd.Parameters["@MessageSecuritySigningCertificate"].Value = pDataObject.MessageSecuritySigningCertificate;
                cmd.Parameters["@MessageSecurityOfflineEncryptionCertificate"].Value = pDataObject.MessageSecurityOfflineEncryptionCertificate;
                cmd.Parameters["@MessageSecurityOfflineDecryptionCertificate"].Value = pDataObject.MessageSecurityOfflineDecryptionCertificate;

                cmd.Parameters["@ServiceMaximumRetryAttempts"].Value = pDataObject.ServiceMaximumRetryAttempts;
                cmd.Parameters["@ServiceRetryWaitTime"].Value = pDataObject.ServiceRetryWaitTime;

                cmd.Parameters["@CreatedBy"].Value = pSecurity.UserID;
                cmd.Parameters["@UpdatedBy"].Value = pSecurity.UserID;

                ConsolidatedDA.ExecuteQuery(pSecurity, cmd);

                System.Nullable<Guid> retIdentityGuid = this.GetOutputValue<Guid>(cmd.Parameters["@NewRowGuid"], pDataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != pDataObject.IdentityGuid)
                    pDataObject.IdentityGuid = retIdentityGuid.Value;

                pDataObject.Changed = false;
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="SyncClientConfigurationDO"/> record.
        /// </summary>
        /// <param name="pSecurity">Contains security credentials</param>
        /// <param name="pDataObject">The client synchronization configuration settings to delete.</param>
        private void Delete(SecurityClass pSecurity, SyncClientConfigurationDO pDataObject)
        {
            using (var cmd = PrepareDeleteStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = pDataObject.IdentityGuid;

                ConsolidatedDA.ExecuteQuery(pSecurity, cmd);
            }
        }
        #endregion Private Persistence Methods

        #region Override Implementations for Prepare Methods
        protected override SqlCommand PrepareUpsertStatement()
        {
            return (CreateUpsertStatement());
        }
        protected override SqlCommand PrepareSelectStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_SyncClientConfigurationSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return (cmd);
        }
        protected override SqlCommand PrepareInsertStatement()
        {
            return (CreateUpsertStatement());
        }
        protected override SqlCommand PrepareUpdateStatement()
        {
            return (CreateUpsertStatement());
        }
        protected override SqlCommand PrepareDeleteStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_SyncClientConfigurationDeleteByGuid";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return (cmd);
        }
        #endregion Override Implementations for Prepare Methods

        #region Private Support Methods

        /// <summary>
        /// The create upsert statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        private SqlCommand CreateUpsertStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_SyncClientConfigurationSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@RootSiteID", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@EnterpriseURL", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@SuspendSynchronizationFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@ServerAuthUserName", SqlDbType.NVarChar, 256);
            cmd.Parameters.Add("@ServerAuthPassword", SqlDbType.VarBinary, 256);
            cmd.Parameters.Add("@ServerAuthDomain", SqlDbType.NVarChar, 256);
            cmd.Parameters.Add("@ServerAuthClientCertificate", SqlDbType.NVarChar, 768);
            cmd.Parameters.Add("@FMAuthUserName", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@FMAuthPassword", SqlDbType.VarBinary, 256);
            cmd.Parameters.Add("@FMAuthClientCertificate", SqlDbType.NVarChar, 768);
            cmd.Parameters.Add("@MessageSecuritySigningCertificate", SqlDbType.NVarChar, 768);
            cmd.Parameters.Add("@MessageSecurityOfflineEncryptionCertificate", SqlDbType.NVarChar, 768);
            cmd.Parameters.Add("@MessageSecurityOfflineDecryptionCertificate", SqlDbType.NVarChar, 768);

            cmd.Parameters.Add("@ServiceMaximumRetryAttempts", SqlDbType.Int);
            cmd.Parameters.Add("@ServiceRetryWaitTime", SqlDbType.Int); 
            
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
            
            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return (cmd);
        }

        /// <summary>
        /// The get data object from data row.
        /// </summary>
        /// <param name="pRow">
        /// The p row.
        /// </param>
        /// <returns>
        /// The <see cref="SyncClientConfigurationDO"/>.
        /// </returns>
        private SyncClientConfigurationDO GetDataObjectFromDataRow(DataRow pRow)
        {
            SyncClientConfigurationDO syncClientConfiguration = new SyncClientConfigurationDO();

            syncClientConfiguration.IdentityGuid = DataObject.getGuid(pRow["SyncClientConfigurationGuid"]);
            syncClientConfiguration.RootSiteID = DataObject.getValue<string>(pRow["RootSiteID"], string.Empty);
            syncClientConfiguration.EnterpriseURL = DataObject.getValue<string>(pRow["EnterpriseURL"], string.Empty);
            syncClientConfiguration.SuspendSynchronizationFlag = DataObject.getValue<bool>(pRow["SuspendSynchronizationFlag"], false);
            syncClientConfiguration.ServerAuthUserName = DataObject.getValue<string>(pRow["ServerAuthUserName"], string.Empty);
            syncClientConfiguration.ServerAuthPassword = (DBNull.Value != pRow["ServerAuthPassword"]) ? UserClass.decode((byte[])pRow["ServerAuthPassword"], Guids.SiteAdminGuid) : string.Empty;
            syncClientConfiguration.ServerAuthDomain = DataObject.getValue<string>(pRow["ServerAuthDomain"], string.Empty);
            syncClientConfiguration.ServerAuthClientCertificate = DataObject.getValue<string>(pRow["ServerAuthClientCertificate"], string.Empty);
            syncClientConfiguration.FMAuthUserName = DataObject.getValue<string>(pRow["FMAuthUserName"], string.Empty);
            syncClientConfiguration.FMAuthPassword = (DBNull.Value != pRow["FMAuthPassword"]) ? UserClass.decode((byte[])pRow["FMAuthPassword"], Guids.SiteAdminGuid) : string.Empty;
            syncClientConfiguration.FMAuthClientCertificate = DataObject.getValue<string>(pRow["FMAuthClientCertificate"], string.Empty);
            syncClientConfiguration.MessageSecuritySigningCertificate = DataObject.getValue<string>(pRow["MessageSecuritySigningCertificate"], string.Empty);
            syncClientConfiguration.MessageSecurityOfflineEncryptionCertificate = DataObject.getValue<string>(pRow["MessageSecurityOfflineEncryptionCertificate"], string.Empty);
            syncClientConfiguration.MessageSecurityOfflineDecryptionCertificate = DataObject.getValue<string>(pRow["MessageSecurityOfflineDecryptionCertificate"], string.Empty);

            syncClientConfiguration.ServiceMaximumRetryAttempts = DataObject.getValue<int>(pRow["ServiceMaximumRetryAttempts"], FMChannelHelper.DefaultRetryAttempts);
            syncClientConfiguration.ServiceRetryWaitTime = DataObject.getValue<int>(pRow["ServiceRetryWaitTime"], FMChannelHelper.DefaultRetryWaitTime);

            syncClientConfiguration.CreatedDate = DataObject.getValue<DateTimeOffset>(pRow["CreatedDate"], DateTimeOffset.Now);
            syncClientConfiguration.CreatedBy = DataObject.getString(pRow["CreatedBy"]);
            syncClientConfiguration.UpdatedDate = DataObject.getValue<DateTimeOffset>(pRow["UpdatedDate"], DateTimeOffset.Now);
            syncClientConfiguration.UpdatedBy = DataObject.getString(pRow["UpdatedBy"]);

            syncClientConfiguration.Changed = false;

            return (syncClientConfiguration);
        }
        #endregion Private Support Methods
    }
}
