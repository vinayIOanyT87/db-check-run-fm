// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerSyncProvider.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SqlServerSyncProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses.Server
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.ServiceClasses;

    using Microsoft.Synchronization.Data;
    using Microsoft.Synchronization.Data.Server;

    /// <summary>
    /// The SQL Server sync provider.
    /// </summary>
    public abstract class SqlServerSyncProvider : DbServerSyncProvider, ISyncServerProviderFM
    {
        #region Attributes
        /// <summary>
        /// The is disposed.
        /// </summary>
        private bool isDisposed = false;

        #endregion Attributes

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerSyncProvider"/> class.
        /// </summary>
        protected SqlServerSyncProvider()
        {
            this.isDisposed = false;

            this.Context = null;
            this.Security = null;
        }

        #endregion Constructors/Destructors

        #region Properties

        #region Context property

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public SyncContextFM Context { get; set; }

        #endregion Context property

        #region Security property

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        public SecurityClass Security { get; set; }

        #endregion Security property

        #endregion Properties

        /// <summary>
        /// The on initialize synchronization.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncScope">
        /// The sync scope.
        /// </param>
        protected abstract void OnInitializeSynchronization(SecurityClass security, SyncScopeDO syncScope);

        #region SyncProvider Overrides - To ensure that the Session Information is Correct
        #endregion SyncProvider Overrides - To ensure that the Session Information is Correct

        #region Protected Synchronization Methods

        /// <summary>
        /// The initialize synchronization.
        /// </summary>
        /// <param name="security">
        /// The current security context of this synchronization session.
        /// </param>
        /// <param name="syncScope">
        /// The synchronization scope that defines which tables to include in this synchronization request.
        /// </param>
        /// <param name="context">
        /// The current FuelsManager synchronization context that defines the parameters for the current synchronization request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// A argument null exception will be thrown if any of the passed in parameters are null.
        /// </exception>
        protected void InitializeSynchronization(SecurityClass security, SyncScopeDO syncScope, SyncContextFM context)
        {
            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Info(1, "** Initializing Server Provider {0} **", this.GetType().ToString());
            }

            if (null == security)
            {
                throw new ArgumentNullException("security", @"Security context must be provided");
            }

            if (null == syncScope)
            {
                throw new ArgumentNullException("syncScope", @"A synchronization scope must be provided.");
            }

            if (null == context)
            {
                throw new ArgumentNullException("context", @"A FuelsManager synchronization context must be provided.");
            }

            // We should only create and open the connection once because we want our @@SPID to be 
            // consistent throughout the entire synchronization session.
            this.Connection = SyncDBI.CreateServerConnection();
            this.Connection.Open();

            this.Context = context;
            this.Security = security;

            this.SwitchContextSecuritySession(security);

            this.OnInitializeSynchronization(security, syncScope);

            this.BatchSize = this.Context.RecordsPerBatch;
        }

        #endregion Protected Synchronization Methods

        #region Private Synchronization Methods

        /// <summary>
        /// The switch context security session.
        /// </summary>
        private void SwitchContextSecuritySession(SecurityClass security)
        {
			if (null == security)
			{
				throw new ArgumentNullException("security", @"Security context must be provided");
			}

			if (null != this.Context 
				&& null != this.Context.Security)
            {
                this.Context.Security.Token = security.Token;
            }
        }

        private void RemoveColumnsNotSupportedByClient(DataTable dataTable, string clientSupportedColumnList)
        {
            List<string> supportedColumns = new List<string>();
            supportedColumns.AddRange(
                clientSupportedColumnList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            if (supportedColumns.Count == 0)
            {
                return;
            }

			List<string> tableColumnNames = new List<string>();

			foreach(DataColumn dataColumn in dataTable.Columns)
			{
				tableColumnNames.Add(dataColumn.ColumnName);
			}

            foreach (string columnName in tableColumnNames)
            {
				// The _RowVersion column will never be included in the Sync Table Column metadata because we can't actually synchronize this field and we don't want to
				// create parameters on the Apply stored procedures.  Since we need this column for batch operations; we can't remove it at this stage.
				if (columnName == "_RowVersion")
				{
					continue;
				}

				if (clientSupportedColumnList.ToLower().Contains(columnName.ToLower()))
                {
                    continue;
                }

                dataTable.Columns.Remove(dataTable.Columns[columnName]);
            }
        }
        #endregion Private Synchronization Methods

        #region Overrides

        /// <summary>
        /// The on selecting changes.
        /// </summary>
        /// <param name="e">
        /// The value.
        /// </param>
        protected override void OnSelectingChanges(SelectingChangesEventArgs e)
        {
			using (var command = new SqlCommand())
			{
				command.Connection = (SqlConnection)e.Connection;
				command.CommandType = CommandType.Text;
				command.CommandText = "CREATE TABLE #SyncTable (PK UniqueIdentifier, ChangeType CHAR(1))"
									  + " CREATE NONCLUSTERED INDEX [IX_SyncTable_ChangeType_PK] ON #SyncTable (ChangeType ASC, PK ASC)";
				command.ExecuteNonQuery();
			}


			base.OnSelectingChanges(e);
        }


        /// <summary>
        /// The on changes selected.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        protected override void OnChangesSelected(ChangesSelectedEventArgs value)
        {
            foreach (DataTable table in value.Context.DataSet.Tables)
            {
                if (this.Context.SupportedColumnsByTable.ContainsKey(table.TableName))
                {
                    StringBuilder supportedColumns = new StringBuilder();
                    foreach (SyncTableToScopeMapColumnDO columnInfo in this.Context.SupportedColumnsByTable[table.TableName])
                    {
                        if (supportedColumns.Length > 0)
                        {
                            supportedColumns.Append(",");
                        }

                        supportedColumns.Append(columnInfo.ColumnName);
                    }

                    this.RemoveColumnsNotSupportedByClient(table, supportedColumns.ToString());
                }
            }

            base.OnChangesSelected(value);
        }

        public override SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession)
        {
            return base.ApplyChanges(groupMetadata, dataSet, syncSession);
        }

        #endregion Overrides

        #region Disposable Pattern Implementation
        /// <summary>
        /// Disposes this Client Sync Provider instance 
        /// </summary>
        /// <param name="disposing">True if explicit finalization, false if through GC</param>
        protected override void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            try
            {
					if (disposing)
               {
					}

            }
            finally
            {
                this.isDisposed = true;
                base.Dispose(disposing);
            }
        }

        #endregion Disposable Pattern Implementation
    }
}
