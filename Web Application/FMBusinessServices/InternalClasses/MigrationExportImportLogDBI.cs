// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationExportImportLogDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for MigrationExportImportLogDBI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.InternalClasses.SyncClasses;

	/// <summary>
    /// The migration import export history log database interface methods.
    /// </summary>
    public class MigrationExportImportLogDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationExportImportLogDBI"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public MigrationExportImportLogDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods

        /// <summary>
        /// The get list.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        /// </returns>
        public List<MigrationDataExportImportLogDO> GetList(SecurityClass security)
        {
            List<MigrationDataExportImportLogDO> list = new List<MigrationDataExportImportLogDO>();

            DataSet ds = this.Load(security, (Guid?)null, (Guid?)null, null);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return list;
            }

            list.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

            return list;
        }

        /// <summary>
        /// The all the migration import export activity for the specified site.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="siteGuid">
        /// The site Guid.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        /// </returns>
        public List<MigrationDataExportImportLogDO> GetListBySiteGuid(SecurityClass security, Guid siteGuid)
        {
            List<MigrationDataExportImportLogDO> list = new List<MigrationDataExportImportLogDO>();
            DataSet ds = this.Load(security, null, siteGuid, null);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return list;
            }

            list.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

            return list;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="identityGuid">
        /// The identity <see cref="Guid"/>.
        /// </param>
        /// <returns>
        /// The <see cref="MigrationDataExportImportLogDO"/>.
        /// </returns>
        public MigrationDataExportImportLogDO Get(SecurityClass security, Guid identityGuid)
        {
            DataSet ds = this.Load(security, identityGuid, null, null);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = ds.Tables[0].Rows[0];
            MigrationDataExportImportLogDO changeHistory = this.GetDataObjectFromDataRow(row);

            return changeHistory;
        }

        /// <summary>
        /// Saves the passed in <seealso cref="MigrationDataExportImportLogDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, MigrationDataExportImportLogDO dataObject)
        {
            // Save the dataobject using a merge implementation
            this.Merge(security, dataObject);

            return true;
        }

        /// <summary>
        /// Deletes the passed in <seealso cref="MigrationDataExportImportLogDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to delete.</param>
        /// <param name="purge">Permanently delete the specified synchronization session.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        public bool Delete(SecurityClass security, MigrationDataExportImportLogDO dataObject, bool purge)
        {
            try
            {
                // Delete the dataobject.
                this.Delete(security, dataObject);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Public Data Access Methods

        #region Private Persistence Methods
        /// <summary>
        /// Returns a DataSet containing the MigrationImportExportHistoryLog record containing the specified Primary Key.
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="identityGuid">
        /// Primary key value for the MigrationImportExportHistoryLog record to load.
        /// </param>
        /// <param name="siteGuid">
        /// The site Guid.
        /// </param>
        /// <param name="activityID">
        /// The activity ID.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private DataSet Load(SecurityClass security, Guid? identityGuid, Guid? siteGuid, string activityID)
        {
            DataSet ds = null;

            using (var cmd = this.PrepareSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);
                parms["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
                parms["@ActivityID"].Value = this.SetOptionalValue<string>(string.IsNullOrEmpty(activityID) ? null : activityID);

                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return ds;
        }

        /// <summary>
        /// Merges current <seealso cref="MigrationDataExportImportLogDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to update or insert.</param>
        private void Merge(SecurityClass security, MigrationDataExportImportLogDO dataObject)
        {
            using (var cmd = this.PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
                parms["@SiteGuid"].Value = dataObject.SiteGuid;
                parms["@ActivityID"].Value = dataObject.ActivityId;
                parms["@ActivityDescription"].Value = dataObject.ActivityDescription;
                parms["@ActivityStatus"].Value = dataObject.ActivityStatus;
                parms["@ClientIPAddress"].Value = dataObject.ClientIPAddress;
                parms["@PerformedBy"].Value = dataObject.PerformedBy;
                parms["@CreatedDate"].Value = dataObject.CreatedDate;
                parms["@CreatedBy"].Value = security.UserID;
                parms["@UpdatedBy"].Value = security.UserID;

                ConsolidatedDA.ExecuteQuery(security, cmd);

                Guid? retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
                {
                    dataObject.IdentityGuid = retIdentityGuid.Value;
                }
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="MigrationDataExportImportLogDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to persist.</param>
        private void Delete(SecurityClass security, MigrationDataExportImportLogDO dataObject)
        {
            using (var cmd = this.PrepareDeleteStatement())
            {
                SqlParameterCollection parms = null;
                parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }
        #endregion Private Persistence Methods

        #region Override Implementations for Prepare Methods

        /// <summary>
        /// Create a command object bound to a merge (insert/update) stored procedure and parameters.
        /// </summary>
        /// <returns>
        /// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
        /// </returns>
        protected override SqlCommand PrepareUpsertStatement()
        {
            return this.CreateMergeStatement();
        }

        /// <summary>
        /// Create a command object bound to a select stored procedure and parameters.
        /// </summary>
        /// <returns>
        /// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
        /// </returns>
        protected override SqlCommand PrepareSelectStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_MigrationExportImportLogSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ActivityID", SqlDbType.NVarChar, 30);

            return cmd;
        }

        /// <summary>
        /// Create a command object bound to a merge (insert/update) stored procedure and parameters.
        /// </summary>
        /// <returns>
        /// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
        /// </returns>
        /// <remarks>
        /// This method is provided to meet the required implementation of an Abstract Method.  All Insert / Update logic has been implemented as a Merge stored 
        /// procedure so Modify or Insert can be used interchangeably.
        /// </remarks>
        protected override SqlCommand PrepareInsertStatement()
        {
            return this.CreateMergeStatement();
        }

        /// <summary>
        /// Create a command object bound to a merge (insert/update) stored procedure and parameters.
        /// </summary>
        /// <returns>
        /// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
        /// </returns>
        /// <remarks>
        /// This method is provided to meet the required implementation of an Abstract Method.  All Insert / Update logic has been implemented as a Merge stored 
        /// procedure so Modify or Insert can be used interchangeably.
        /// </remarks>
        protected override SqlCommand PrepareUpdateStatement()
        {
            return this.CreateMergeStatement();
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

            cmd.CommandText = "dbo.usp_MigrationExportImportLogDeleteByGuid";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return cmd;
        }
        #endregion Override Implementations for Prepare Methods

        #region Private Support Methods

        /// <summary>
        /// The create merge statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        private SqlCommand CreateMergeStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_MigrationExportImportLogSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ActivityID", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@ActivityDescription", SqlDbType.NVarChar, 256);
            cmd.Parameters.Add("@ActivityStatus", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@PerformedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@ClientIPAddress", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return cmd;
        }

        /// <summary>
        /// The get data object from data row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="MigrationDataExportImportLogDO"/>.
        /// </returns>
        private MigrationDataExportImportLogDO GetDataObjectFromDataRow(DataRow row)
        {
            MigrationDataExportImportLogDO dataObject = new MigrationDataExportImportLogDO();

            dataObject.IdentityGuid = DataObject.getValue<Guid>(row["MigrationExportImportLogGuid"], Guid.Empty);
            dataObject.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
            dataObject.ActivityId = DataObject.getValue<string>(row["ActivityID"], string.Empty);
            dataObject.ActivityDescription = DataObject.getString(row["ActivityDescription"]);
            dataObject.ActivityStatus = DataObject.getString(row["ActivityStatus"]);
            dataObject.PerformedBy = DataObject.getString(row["PerformedBy"]);
            dataObject.ClientIPAddress = DataObject.getString(row["ClientIPAddress"]);
            dataObject.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            dataObject.CreatedBy = DataObject.getString(row["CreatedBy"]);
            dataObject.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now);
            dataObject.UpdatedBy = DataObject.getString(row["UpdatedBy"]);

            // If additional information is returned by the selected, get it.
            dataObject.SiteID = DataObject.getValue<string>(row["SiteID"], string.Empty);

            return dataObject;
        }
        #endregion Private Support Methods
    }
}
