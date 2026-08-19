// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncServerConfigurationDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncServerConfigurationDBI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.InternalClasses.SyncClasses;

	/// <summary>
    /// Summary description for SyncServerConfigurationDBI.
    /// </summary>
    public class SyncServerConfigurationDBI : SyncDBI, IDisposable
    {
        #region Attributes
        #endregion Attributes

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncServerConfigurationDBI"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public SyncServerConfigurationDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods
        /// <summary>
        /// Gets the current <seealso cref="SyncServerConfigurationDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <returns>An instance of the current SyncServerConfigurationDO settings.</returns>
        public SyncServerConfigurationDO Get(SecurityClass security)
        {
            return this.GetEx(security, null);
        }

        /// <summary>
        /// Gets the current <seealso cref="SyncServerConfigurationDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <returns>An instance of the current SyncServerConfigurationDO settings.</returns>
        public SyncServerConfigurationDO Get(SecurityClass security, Guid identityGuid)
        {
            if (identityGuid == Guid.Empty)
            {
                throw new Exception("Identity parameter cannot be empty.");
            }

            return this.GetEx(security, identityGuid);
        }

        /// <summary>
        /// Saves the passed in <seealso cref="SyncServerConfigurationDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The server synchronization configuration settings to persist.</param>
        /// <returns>True unless an exception is encountered.</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, SyncServerConfigurationDO dataObject)
        {
            // Save the dataobject using a merge implementation
            this.Upsert(security, dataObject);

            return true;
        }

        /// <summary>
        /// Deletes the passed in <seealso cref="SyncServerConfigurationDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to delete.</param>
        /// <returns>True unless an exception is encountered.</returns>
        public bool Delete(SecurityClass security, SyncServerConfigurationDO dataObject, bool purge)
        {
            try
            {
                // Save the dataobject using a merge implementation
                this.Delete(security, dataObject);

                return true;
            }
            catch (Exception eX)
            {
                throw eX;
            }
        }
        #endregion Public Data Access Methods

        #region Private Persistence Methods
        /// <summary>
        /// Gets the current <seealso cref="SyncServerConfigurationDO"/> from the database by calling the internal Load method and processing the returned dataset.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="identityGuid">Contains the identity guid of a specific record to load.  If null is passed in, then the first available record is loaded.</param>
        /// <returns>If client configuration settings were found, returns a fully populated <seealso cref="SyncServerConfigurationDO"/>, otherwise; null.</returns>
        /// <remarks>Since the Server Node ID is considered to be applicable to Server and Server settings this method will include this in the returned instance.</remarks>
        private SyncServerConfigurationDO GetEx(SecurityClass security, System.Nullable<Guid> identityGuid)
        {
            DataSet ds = this.Load(security, identityGuid);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = ds.Tables[0].Rows[0];
            SyncServerConfigurationDO syncServerConfiguration = this.GetDataObjectFromDataRow(row);

            return syncServerConfiguration;
        }

        /// <summary>
        /// Loads the current <seealso cref="SyncServerConfigurationDO"/> from the database.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="identityGuid">Contains the identity guid of a specific record to load.  If null is passed in, then the first available record is loaded.</param>
        /// <returns>Returns a DataSet</returns>
        private DataSet Load(SecurityClass security, System.Nullable<Guid> identityGuid)
        {
            DataSet ds = null;

            using (var cmd = this.PrepareSelectStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);

                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return ds;
        }

        /// <summary>
        /// Updates an existing record with the current <seealso cref="SyncServerConfigurationDO"/> changes.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The server synchronization configuration settings to persist.</param>
        private void Upsert(SecurityClass security, SyncServerConfigurationDO dataObject)
        {
            using (var cmd = this.PrepareUpsertStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = (dataObject.IdentityGuid == Guid.Empty) ? (object)DBNull.Value : dataObject.IdentityGuid;
                cmd.Parameters["@AllowSynchronizationFlag"].Value = dataObject.AllowSynchronizationFlag;
                cmd.Parameters["@AcceptFMUserAuthenticationFlag"].Value = dataObject.AcceptFMUserAuthenticationFlag;
                cmd.Parameters["@AcceptClientCertificateAuthenticationFlag"].Value = dataObject.AcceptClientCertificateAuthenticationFlag;
                cmd.Parameters["@ClientSignatureRequiredForMessagesFlag"].Value = dataObject.ClientSignatureRequiredForMessagesFlag;
                cmd.Parameters["@ClientEncryptionRequiredForMessagesFlag"].Value = dataObject.ClientEncryptionRequiredForMessagesFlag;

				cmd.Parameters["@NodeHealthCriticalThresholdHours"].Value = dataObject.NodeHealthCriticalThresholdHours;
				cmd.Parameters["@NodeHealthCautionThresholdHours"].Value = dataObject.NodeHealthCautionThresholdHours;
                
				cmd.Parameters["@CreatedBy"].Value = security.UserID;
                cmd.Parameters["@UpdatedBy"].Value = security.UserID;

	            this.ConsolidatedDA.ExecuteQuery(security, cmd);

                Guid? retIdentityGuid = this.GetOutputValue<Guid>(cmd.Parameters["@NewRowGuid"], dataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
                {
                    dataObject.IdentityGuid = retIdentityGuid.Value;
                }

                dataObject.Changed = false;
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="SyncServerConfigurationDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The server synchronization configuration settings to delete.</param>
        private void Delete(SecurityClass security, SyncServerConfigurationDO dataObject)
        {
            using (var cmd = this.PrepareDeleteStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = dataObject.IdentityGuid;

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }
        #endregion Private Persistence Methods

        #region Override Implementations for Prepare Methods

        /// <summary>
        /// The prepare upsert statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        protected override SqlCommand PrepareUpsertStatement()
        {
            return this.CreateUpsertStatement();
        }

        /// <summary>
        /// The prepare select statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        protected override SqlCommand PrepareSelectStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_SyncServerConfigurationSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return cmd;
        }

        /// <summary>
        /// The prepare insert statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        protected override SqlCommand PrepareInsertStatement()
        {
            return this.CreateUpsertStatement();
        }

        /// <summary>
        /// The prepare update statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        protected override SqlCommand PrepareUpdateStatement()
        {
            return this.CreateUpsertStatement();
        }

        /// <summary>
        /// The prepare delete statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        protected override SqlCommand PrepareDeleteStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_SyncServerConfigurationDeleteByGuid";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return cmd;
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

            cmd.CommandText = "dbo.usp_SyncServerConfigurationSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@AllowSynchronizationFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@AcceptFMUserAuthenticationFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@AcceptClientCertificateAuthenticationFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@ClientSignatureRequiredForMessagesFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@ClientEncryptionRequiredForMessagesFlag", SqlDbType.Bit);

			cmd.Parameters.Add("@NodeHealthCriticalThresholdHours", SqlDbType.Int);
			cmd.Parameters.Add("@NodeHealthCautionThresholdHours", SqlDbType.Int); 

            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return cmd;
        }

        /// <summary>
        /// The get data object from data row.
        /// </summary>
        /// <param name="pRow">
        /// The p row.
        /// </param>
        /// <returns>
        /// The <see cref="SyncServerConfigurationDO"/>.
        /// </returns>
        private SyncServerConfigurationDO GetDataObjectFromDataRow(DataRow pRow)
        {
            SyncServerConfigurationDO syncServerConfiguration = new SyncServerConfigurationDO();

            syncServerConfiguration.IdentityGuid = DataObject.getGuid(pRow["SyncServerConfigurationGuid"]);
            syncServerConfiguration.AllowSynchronizationFlag = DataObject.getValue<bool>(pRow["AllowSynchronizationFlag"], false);
            syncServerConfiguration.AcceptFMUserAuthenticationFlag = DataObject.getValue<bool>(pRow["AcceptFMUserAuthenticationFlag"], false);
            syncServerConfiguration.AcceptClientCertificateAuthenticationFlag = DataObject.getValue<bool>(pRow["AcceptClientCertificateAuthenticationFlag"], false);
            syncServerConfiguration.ClientSignatureRequiredForMessagesFlag = DataObject.getValue<bool>(pRow["ClientSignatureRequiredForMessagesFlag"], false);
            syncServerConfiguration.ClientEncryptionRequiredForMessagesFlag = DataObject.getValue<bool>(pRow["ClientEncryptionRequiredForMessagesFlag"], false);
			
			syncServerConfiguration.NodeHealthCriticalThresholdHours = DataObject.getValue<int>(pRow["NodeHealthCriticalThresholdHours"], FMChannelHelper.DefaultNodeHealthCriticalThresholdHours);
			syncServerConfiguration.NodeHealthCautionThresholdHours = DataObject.getValue<int>(pRow["NodeHealthCautionThresholdHours"], FMChannelHelper.DefaultNodeHealthCautionThresholdHours);

            syncServerConfiguration.CreatedDate = DataObject.getValue<DateTimeOffset>(pRow["CreatedDate"], DateTimeOffset.Now);
            syncServerConfiguration.CreatedBy = DataObject.getString(pRow["CreatedBy"]);
            syncServerConfiguration.UpdatedDate = DataObject.getValue<DateTimeOffset>(pRow["UpdatedDate"], DateTimeOffset.Now);
            syncServerConfiguration.UpdatedBy = DataObject.getString(pRow["UpdatedBy"]);

            syncServerConfiguration.Changed = false;

            return syncServerConfiguration;
        }
        #endregion Private Support Methods
    }
}
