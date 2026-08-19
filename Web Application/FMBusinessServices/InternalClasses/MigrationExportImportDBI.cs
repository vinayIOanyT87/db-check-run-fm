// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationExportImportDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for MigrationExportImportDBI.
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
    /// The migration import export history log database interface.
    /// </summary>
    public class MigrationExportImportDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationExportImportDBI"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public MigrationExportImportDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods

        /// <summary>
        /// The get key mapping list for table.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public DataSet GetKeyMappingListForTable(SecurityClass security, string tableName)
        {
            DataSet ds = this.LoadKeyMappings(security, tableName);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            return ds;
        }

        #endregion Public Data Access Methods

        #region Private Persistence Methods

        /// <summary>
        /// The load key mappings.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private DataSet LoadKeyMappings(SecurityClass security, string tableName)
        {
            DataSet ds = null;

            using (var cmd = this.PrepareKeyMappingSelectStatement(tableName))
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@sync_context_site_guid"].Value = security.SiteGuid;
                parms["@sync_context_site_id"].Value = security.SiteID;

                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return ds;
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
            throw new NotImplementedException("Method not supported on this interface");
        }

        /// <summary>
        /// Create a command object bound to a select stored procedure and parameters.
        /// </summary>
        /// <returns>
        /// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
        /// </returns>
        protected override SqlCommand PrepareSelectStatement()
        {
            throw new NotImplementedException("Method not supported on this interface");
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
            throw new NotImplementedException("Method not supported on this interface");
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
            throw new NotImplementedException("Method not supported on this interface");
        }

        /// <summary>
        /// The prepare delete statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        protected override SqlCommand PrepareDeleteStatement()
        {
            throw new NotImplementedException("Method not supported on this interface");
        }
        #endregion Override Implementations for Prepare Methods

        #region Implementations for Custom Prepare Methods

        /// <summary>
        /// The prepare key mapping select statement.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        protected SqlCommand PrepareKeyMappingSelectStatement(string tableName)
        {
            var cmd = new SqlCommand();

            cmd.CommandText = string.Format("sync.gsp_CirrusUpgSelectIDToGuidMapping_{0}", tableName);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@sync_context_site_guid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@sync_context_site_id", SqlDbType.NVarChar, 30);

            return cmd;
        }
        #endregion Implementations for Custom Prepare Methods

        #region Private Support Methods

        #endregion Private Support Methods
    }
}
