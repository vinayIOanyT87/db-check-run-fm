// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncServerProviderHelperFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncServerProviderHelperFM type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses.Server
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using System.Text;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.InternalClasses;
    using FMBusinessServices.InternalClasses.SyncClasses;

    using Microsoft.Synchronization;
    using Microsoft.Synchronization.Data;
    using Microsoft.Synchronization.Data.Server;

    /// <summary>
    /// Helper class that exposes several static methods used by the <see cref="ServerSyncProviderFM"/> class
    /// to generate <see cref="SyncAdapter"/> instances and handle server side conflicts
    /// </summary>
    public partial class SyncServerProviderHelperFM : SyncProviderHelperFM
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncServerProviderHelperFM"/> class.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public SyncServerProviderHelperFM(SecurityClass security, SyncContextFM context)
            : base()
        {
        }

        #region Public Static Helper Methods

        /// <summary>
        /// The get sync adapter.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncTableToScopeMapping">
        /// The sync table to scope mapping.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="SyncAdapter"/>.
        /// </returns>
        public static SyncAdapter GetSyncAdapter(SecurityClass security, SyncTableToScopeMapDO syncTableToScopeMapping, SyncContextFM context, SqlConnection connection)
        {
            // Create a SyncAdapter for the table, and then define
            // the commands to synchronize changes:
            // * SelectIncrementalInsertsCommand, SelectIncrementalUpdatesCommand,
            //  and SelectIncrementalDeletesCommand are used to select changes
            //  from the server that the client provider then applies to the client.
            // * InsertCommand, UpdateCommand, and DeleteCommand are used to apply
            //  to the server the changes that the client provider has selected
            //  from the client.
            
            // Resolve the mapped table
            SyncTableDO syncTable = GetSyncTable(security, syncTableToScopeMapping.SyncTableGuid);

            // Resolve the mapped commands
            SyncTableToScopeMapCommandDO syncCommands = GetSyncTableCommands(security, syncTableToScopeMapping.IdentityGuid);

            SyncTableToScopeMapColumnCollection syncColumns = null;

            // Create the SyncAdapter.  
            // On the server, since we're using Stored Procedures, we have some flexibility in giving the SyncAdapter a table name that matches the client's
            // name so the SyncFramework can locate the correct Adapter to handle the client's requests.
            string tableName = string.IsNullOrEmpty(syncTableToScopeMapping.ClientTableNameOverride) ? syncTable.TableName : syncTableToScopeMapping.ClientTableNameOverride;

            // Instead of using the syncColumn list as defined on the server, we need to identify the columns that are supported by the client.  Retrieve this from the 
            // Context information.
            if (null == context.SupportedColumnsByTable[tableName])
            {
                syncColumns = SyncProviderHelperFM.GetSyncTableColumns(security, syncTableToScopeMapping.IdentityGuid); // default
            }
            else
            {
                syncColumns = context.SupportedColumnsByTable[tableName];
            }

            var syncAdapter = new SyncAdapter(tableName);

            // Select Inserts FROM the Server
            SqlCommand incrInserts = null;

            incrInserts = new SqlCommand();
            incrInserts.CommandTimeout = SyncProviderHelperFM.DEFAULT_SYNC_COMMAND_TIMEOUT;
            incrInserts.CommandText = SyncProviderHelperFM.ApplyNodeTypeMask(syncCommands.SelectIncrementalInserts, SyncProviderHelperFM.ServerNodeType);
            incrInserts.CommandType = CommandType.StoredProcedure;

            incrInserts.Parameters.Add(SyncParamsFM.SYNC_INITIALIZED_PARAMETER, SqlDbType.Bit);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_FILTER_BY_DATERANGE_PARAMETER, SqlDbType.Bit);

            incrInserts.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_PARAMETER, SqlDbType.UniqueIdentifier);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_SERVER_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);

            // if (syncTable.IsSiteFilteredFlag)
            // {
            //     incrInserts.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_PARAMETER, SqlDbType.UniqueIdentifier);
            //     incrInserts.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar);
            //     incrInserts.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_CHILD_SITE_GUID_LIST_PARAMETER, SqlDbType.NVarChar, 512);
            // }

            if (syncTable.IsSiteFilteredFlag)
            {
                if (!string.IsNullOrEmpty(context.SiteID))
                {
                    incrInserts.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, context.SiteID);
                }
                else
                {
                    incrInserts.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, DBNull.Value);
                }

                if (context.SiteGuid.HasValue && context.SiteGuid.Value != Guid.Empty)
                {
                    incrInserts.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_PARAMETER, context.SiteGuid);
                }
                else
                {
                    incrInserts.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_PARAMETER, DBNull.Value);
                }
            }

            incrInserts.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_LIST_PARAMETER, SqlDbType.NVarChar, 1024);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_LIST_PARAMETER, SqlDbType.NVarChar, 1024);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_TABLE_NAME_PARAMETER, SqlDbType.NVarChar);
            incrInserts.Parameters.Add(string.Format("{0}_{1}", SyncParamsFM.SYNC_BATCH_SIZE_PARAMETER, SyncHelperFM.GetNamePartFromTableName(tableName, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME)), SqlDbType.Int);
		    incrInserts.Parameters.Add(string.Format("{0}_{1}", SyncParamsFM.SYNC_FIRST_TIME_SYNC_OPTION, SyncHelperFM.GetNamePartFromTableName(tableName, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME)), SqlDbType.Int);
		    incrInserts.Parameters.Add(SyncParamsFM.SYNC_BYPASS_INSERT_UPDATE_EXTRACTION_PARAMETER, SqlDbType.Bit);
            incrInserts.Parameters.Add(SyncParamsFM.SYNC_REQUEST_TYPE_PARAMETER, SqlDbType.Int);
            incrInserts.Connection = connection;

            syncAdapter.SelectIncrementalInsertsCommand = incrInserts;

            // Apply Inserts TO the Server
            SqlCommand inserts = null;

            if (context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL || context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE)
            {
                inserts = new SqlCommand();
                inserts.CommandTimeout = SyncProviderHelperFM.DEFAULT_SYNC_COMMAND_TIMEOUT;
                inserts.CommandText = SyncProviderHelperFM.ApplyNodeTypeMask(syncCommands.ApplyIncrementalInserts, SyncProviderHelperFM.ServerNodeType);
                inserts.CommandType = CommandType.StoredProcedure;

                inserts.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);
                inserts.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_PARAMETER, SqlDbType.UniqueIdentifier);
                inserts.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
                inserts.Parameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                inserts.Parameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                inserts.Parameters.Add(SyncParamsFM.SYNC_FILTER_BY_DATERANGE_PARAMETER, SqlDbType.Bit);

                foreach (SyncTableToScopeMapColumnDO syncColumn in syncColumns)
                {
                    inserts.Parameters.Add(SyncParamsFM.GetFormattedParameterString(syncColumn.ColumnName), SyncProviderHelperFM.GetSqlDbTypeFromString(syncColumn.ColumnType));
                }

                inserts.Parameters.Add(SyncParamsFM.SYNC_ROW_COUNT_PARAMETER, SqlDbType.Int).Direction = ParameterDirection.Output;
                inserts.Parameters.Add(SyncParamsFM.SYNC_TABLE_NAME_PARAMETER, SqlDbType.NVarChar);
                inserts.Parameters.Add(string.Format("{0}_{1}", SyncParamsFM.SYNC_SUPPORTED_COLUMNS_PARAMETER, SyncHelperFM.GetNamePartFromTableName(tableName, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME)), SqlDbType.VarChar);
                inserts.Connection = connection;
            }

            syncAdapter.InsertCommand = inserts;

            // SELECT Updates FROM the Server
            SqlCommand incrUpdates = null;

            incrUpdates = new SqlCommand();
            incrUpdates.CommandTimeout = SyncProviderHelperFM.DEFAULT_SYNC_COMMAND_TIMEOUT;
            incrUpdates.CommandText = SyncProviderHelperFM.ApplyNodeTypeMask(syncCommands.SelectIncrementalUpdates, SyncProviderHelperFM.ServerNodeType);
            incrUpdates.CommandType = CommandType.StoredProcedure;

            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_INITIALIZED_PARAMETER, SqlDbType.Bit);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_FILTER_BY_DATERANGE_PARAMETER, SqlDbType.Bit);

            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_PARAMETER, SqlDbType.UniqueIdentifier);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_SERVER_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);

            if (syncTable.IsSiteFilteredFlag)
            {
                if (context.SiteGuid.HasValue && context.SiteGuid.Value != Guid.Empty)
                {
                    incrUpdates.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_PARAMETER, context.SiteGuid);
                }
                else
                {
                    incrUpdates.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_PARAMETER, DBNull.Value);
                }

                if (!string.IsNullOrEmpty(context.SiteID))
                {
                    incrUpdates.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, context.SiteID);
                }
                else
                {
                    incrUpdates.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, DBNull.Value);
                }
            }

            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_LIST_PARAMETER, SqlDbType.NVarChar, 1024);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_LIST_PARAMETER, SqlDbType.NVarChar, 1024);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_TABLE_NAME_PARAMETER, SqlDbType.NVarChar);
            incrUpdates.Parameters.Add(string.Format("{0}_{1}", SyncParamsFM.SYNC_BATCH_SIZE_PARAMETER, SyncHelperFM.GetNamePartFromTableName(tableName, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME)), SqlDbType.Int);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_BYPASS_INSERT_UPDATE_EXTRACTION_PARAMETER, SqlDbType.Bit);
            incrUpdates.Parameters.Add(SyncParamsFM.SYNC_REQUEST_TYPE_PARAMETER, SqlDbType.Int);

            incrUpdates.Connection = connection;

            syncAdapter.SelectIncrementalUpdatesCommand = incrUpdates;

            // Apply Updates TO the Server
            SqlCommand updates = null;

            if (context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL || context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE)
            {
                updates = new SqlCommand();
                updates.CommandTimeout = SyncProviderHelperFM.DEFAULT_SYNC_COMMAND_TIMEOUT;
                updates.CommandText = SyncProviderHelperFM.ApplyNodeTypeMask(syncCommands.ApplyIncrementalUpdates, SyncProviderHelperFM.ServerNodeType);
                updates.CommandType = CommandType.StoredProcedure;

                updates.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);
                updates.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_PARAMETER, SqlDbType.UniqueIdentifier);
                updates.Parameters.Add(SyncParamsFM.SYNC_FORCE_WRITE_PARAMETER, SqlDbType.Bit);
                updates.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
                updates.Parameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                updates.Parameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                updates.Parameters.Add(SyncParamsFM.SYNC_FILTER_BY_DATERANGE_PARAMETER, SqlDbType.Bit);

                foreach (SyncTableToScopeMapColumnDO syncColumn in syncColumns)
                {
                    updates.Parameters.Add(SyncParamsFM.GetFormattedParameterString(syncColumn.ColumnName), SyncProviderHelperFM.GetSqlDbTypeFromString(syncColumn.ColumnType));
                }

                updates.Parameters.Add(SyncParamsFM.SYNC_ROW_COUNT_PARAMETER, SqlDbType.Int).Direction = ParameterDirection.Output;
                updates.Parameters.Add(SyncParamsFM.SYNC_TABLE_NAME_PARAMETER, SqlDbType.NVarChar);
                updates.Parameters.Add(string.Format("{0}_{1}", SyncParamsFM.SYNC_SUPPORTED_COLUMNS_PARAMETER, SyncHelperFM.GetNamePartFromTableName(tableName, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME)), SqlDbType.VarChar);
                updates.Connection = connection;
            }

            syncAdapter.UpdateCommand = updates;

            // SELECT Deletes FROM the Server
            SqlCommand incrDeletes = null;

            if (context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL || context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_DELETE)
            {
                incrDeletes = new SqlCommand();
                incrDeletes.CommandTimeout = SyncProviderHelperFM.DEFAULT_SYNC_COMMAND_TIMEOUT;
                incrDeletes.CommandText = SyncProviderHelperFM.ApplyNodeTypeMask(syncCommands.SelectIncrementalDeletes, SyncProviderHelperFM.ServerNodeType);
                incrDeletes.CommandType = CommandType.StoredProcedure;

                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_INITIALIZED_PARAMETER, SqlDbType.Bit);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_FILTER_BY_DATERANGE_PARAMETER, SqlDbType.Bit);

                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_PARAMETER, SqlDbType.UniqueIdentifier);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_SERVER_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);

                if (context.SiteGuid.HasValue && context.SiteGuid.Value != Guid.Empty)
                {
                    incrDeletes.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_PARAMETER, context.SiteGuid);
                }
                else
                {
                    incrDeletes.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_PARAMETER, DBNull.Value);
                }

                if (!string.IsNullOrEmpty(context.SiteID))
                {
                    incrDeletes.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, context.SiteID);
                }
                else
                {
                    incrDeletes.Parameters.AddWithValue(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, DBNull.Value);
                }

                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_LIST_PARAMETER, SqlDbType.NVarChar, 1024);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_LIST_PARAMETER, SqlDbType.NVarChar, 1024);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_TABLE_NAME_PARAMETER, SqlDbType.NVarChar);
                incrDeletes.Parameters.Add(string.Format("{0}_{1}", SyncParamsFM.SYNC_BATCH_SIZE_PARAMETER, SyncHelperFM.GetNamePartFromTableName(tableName, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME)), SqlDbType.Int);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_BYPASS_DELETE_EXTRACTION_PARAMETER, SqlDbType.Bit);
                incrDeletes.Parameters.Add(SyncParamsFM.SYNC_REQUEST_TYPE_PARAMETER, SqlDbType.Int);

                incrDeletes.Connection = connection;
            }

            syncAdapter.SelectIncrementalDeletesCommand = incrDeletes;

            // Apply Deletes TO the Server
            SqlCommand deletes = null;

            if (context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL || context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_DELETE)
            {
                deletes = new SqlCommand();
                deletes.CommandTimeout = SyncProviderHelperFM.DEFAULT_SYNC_COMMAND_TIMEOUT;
                deletes.CommandText = SyncProviderHelperFM.ApplyNodeTypeMask(syncCommands.ApplyIncrementalDeletes, SyncProviderHelperFM.ServerNodeType);
                deletes.CommandType = CommandType.StoredProcedure;
                deletes.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_BINARY_PARAMETER, SqlDbType.Binary, 16);
                deletes.Parameters.Add(SyncParamsFM.SYNC_CLIENT_ID_PARAMETER, SqlDbType.UniqueIdentifier);
                deletes.Parameters.Add(SyncParamsFM.SYNC_FORCE_WRITE_PARAMETER, SqlDbType.Bit);
                deletes.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
                deletes.Parameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                deletes.Parameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                deletes.Parameters.Add(SyncParamsFM.SYNC_FILTER_BY_DATERANGE_PARAMETER, SqlDbType.Bit);

                foreach (SyncTableToScopeMapColumnDO syncColumn in syncColumns)
                {
                    if (syncColumn.IsPrimaryKeyMemberFlag)
                    {
                        deletes.Parameters.Add(
                            SyncParamsFM.GetFormattedParameterString(syncColumn.ColumnName),
                            SyncProviderHelperFM.GetSqlDbTypeFromString(syncColumn.ColumnType));
                    }
                }

                deletes.Parameters.Add(SyncParamsFM.SYNC_ROW_COUNT_PARAMETER, SqlDbType.Int).Direction = ParameterDirection.Output;
                deletes.Parameters.Add(SyncParamsFM.SYNC_TABLE_NAME_PARAMETER, SqlDbType.NVarChar);
                deletes.Connection = connection;
            }

            syncAdapter.DeleteCommand = deletes;

            // This command is used if @sync_row_count returns
            // 0 when changes are applied to the server.
            SqlCommand updateConflicts = null;

            if (context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL || context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE)
            {
                updateConflicts = new SqlCommand();
                updateConflicts.CommandTimeout = SyncProviderHelperFM.DEFAULT_SYNC_COMMAND_TIMEOUT;
                updateConflicts.CommandText = SyncProviderHelperFM.ApplyNodeTypeMask(syncCommands.SelectUpdateConflicts, SyncProviderHelperFM.ServerNodeType);
                updateConflicts.CommandType = CommandType.StoredProcedure;

                foreach (SyncTableToScopeMapColumnDO syncColumn in syncColumns)
                {
                    if (syncColumn.IsPrimaryKeyMemberFlag)
                    {
                        updateConflicts.Parameters.Add(
                            SyncParamsFM.GetFormattedParameterString(syncColumn.ColumnName),
                            SyncProviderHelperFM.GetSqlDbTypeFromString(syncColumn.ColumnType));
                    }
                }

                updateConflicts.Connection = connection;
            }

            syncAdapter.SelectConflictUpdatedRowsCommand = updateConflicts;

            // This command is used if the server provider cannot find
            // a row in the base table.
            SqlCommand deleteConflicts = null;

            if (context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL || context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_DELETE)
            {
                deleteConflicts = new SqlCommand();
                deleteConflicts.CommandTimeout = SyncProviderHelperFM.DEFAULT_SYNC_COMMAND_TIMEOUT;
                deleteConflicts.CommandText = SyncProviderHelperFM.ApplyNodeTypeMask(syncCommands.SelectDeleteConflicts, SyncProviderHelperFM.ServerNodeType);
                deleteConflicts.CommandType = CommandType.StoredProcedure;

                deleteConflicts.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
                deleteConflicts.Parameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                deleteConflicts.Parameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, SqlDbType.DateTimeOffset);
                deleteConflicts.Parameters.Add(SyncParamsFM.SYNC_FILTER_BY_DATERANGE_PARAMETER, SqlDbType.Bit);

                foreach (SyncTableToScopeMapColumnDO syncColumn in syncColumns)
                {
                    if (syncColumn.IsPrimaryKeyMemberFlag)
                    {
                        deleteConflicts.Parameters.Add(
                            SyncParamsFM.GetFormattedParameterString(syncColumn.ColumnName),
                            SyncProviderHelperFM.GetSqlDbTypeFromString(syncColumn.ColumnType));
                    }
                }

                deleteConflicts.Connection = connection;
            }

            syncAdapter.SelectConflictDeletedRowsCommand = deleteConflicts;

            return syncAdapter;
        }

        /// <summary>
        /// Event handler for the synchronization provider's ApplyChangeFailed event.  These are typically caused by synchronization conflicts or database errors while applying
        /// changes to the target database.
        /// </summary>
        /// <param name="syncContextFm">
        /// An instance of the current <see cref="SyncContextFM"/>.
        /// </param>
        /// <param name="syncSessionDetail">
        /// An instance of the current <see cref="SyncSessionLogDO"/> that applies to this synchronization session.
        /// </param>
        /// <param name="sender">
        /// Object that raised this event.
        /// </param>
        /// <param name="e">
        /// A populated instance of the <see cref="ApplyChangeFailedEventArgs"/> that contains information about the conflict, conflicting rows, etc.
        /// </param>
        public static void ApplyChangeFailed(SyncContextFM syncContextFm, SyncSessionScopeLogDO syncSessionDetail, object sender, ApplyChangeFailedEventArgs e)
        {
			var provider = sender as DbServerSyncProvider;

	        if (provider == null)
	        {
		        return;
	        }


            string tableName =
                e != null && e.TableMetadata != null && e.TableMetadata.TableName != null
                    ?
                    e.TableMetadata.TableName : "*Unknown*";

	        var syncAdapter = provider.SyncAdapters[tableName];

			if (syncAdapter == null)
	        {
		        return;
	        }

	        IDbCommand command = null;
	        if (e.Conflict.SyncStage == SyncStage.ApplyingDeletes)
	        {
		        command = syncAdapter.DeleteCommand;
	        }
			else if (e.Conflict.SyncStage == SyncStage.ApplyingInserts)
			{
				command = syncAdapter.InsertCommand;
			}
			else if (e.Conflict.SyncStage == SyncStage.ApplyingUpdates)
			{
				command = syncAdapter.UpdateCommand;
			}

	        if (command == null)
	        {
		        return;
	        }

	        var commandType = command.CommandType;
	        var commandText = command.CommandText;
	        var parameters = command.Parameters;
			var enumerator = parameters.GetEnumerator();
	        var parameterDictionary = new Dictionary<string, object>();

			while (enumerator.MoveNext() == true)
			{
				var sqlParameter = enumerator.Current as SqlParameter;
				if (sqlParameter != null)
				{
					parameterDictionary.Add(sqlParameter.ParameterName,sqlParameter.Value);
				}
			}


            SYNCCONFLICTTYPE conflictType = SyncHelperFM.ConvertSyncConflictType(e.Conflict.ConflictType);
            long resyncMinAnchor = 0;
            long resyncMaxAnchor = syncContextFm.MaxEnterpriseSyncAnchor;

            if (null != e.TableMetadata)
            {
                resyncMinAnchor = (long)SyncDBI.DeserializeAnchorValue(e.TableMetadata.LastReceivedAnchor.Anchor);
            }

            if (e.Conflict.ClientChange != null)
            {
                int clientChangeCount = e.Conflict.ClientChange.Rows.Count;
                int clientColumnCount = e.Conflict.ClientChange.Columns.Count;

                // We're attempting to apply changes from the Enterprise to the Client.
                if (clientChangeCount > 0)
                {
                    switch (e.Conflict.ConflictType)
                    {
                        case ConflictType.ClientDeleteServerUpdate:
                        case ConflictType.ClientUpdateServerDelete:
                            if (e.Conflict.ConflictType == ConflictType.ClientUpdateServerDelete)
                            {
                                // The Client tried to update (ClientUpdate) an entity the Server recently deleted (ServerDelete).  
                                // Note: We shouldn't see this type of conflict because the stored procedures are designed to UPSERT.
                                // If we did receive this conflict, being the server we need to let the server win by performing Continuing with the conflict.
                                // The server's delete will be going down to the client shortly.
                                e.Action = ApplyAction.Continue;
                            }
                            else
                            {
                                // If the Client tried to delete an entity (ClientDelete) the Server recently updated it (ServerUpdate), we 
                                // should probably ignore the client delete request since the server shows the record as being active.
                                // The record will be reintroduced into the client and they can delete it again if needed.
                                e.Action = ApplyAction.Continue;
                            }

                            break;

                        case ConflictType.ClientUpdateServerUpdate:
                        case ConflictType.ClientInsertServerInsert:
                            e.Action = ApplyAction.Continue;

                            for (int rowIndex = 0; rowIndex < clientChangeCount; rowIndex++)
                            {
                                DataRow conflictRow = e.Conflict.ClientChange.Rows[rowIndex];
                                long recordRowVersion = 0;

                                string primaryKeyColumn =
                                    SyncProviderHelperFM.GetTablePrimaryKeyColumnName(
                                        syncContextFm.SupportedColumnsByTable[tableName]);

                                string recordKey =
                                    SyncProviderHelperFM.GetRecordKeyString(
                                        syncContextFm.SupportedColumnsByTable[tableName],
                                        conflictRow);

                                // If the action has been set to RetryWithForceWrite, that means that something before this point has decided that the conflict can be 
                                // automatically resolved.
                                // Otherwise we need to record this conflict.
                                if (e.Action != ApplyAction.RetryWithForceWrite)
                                {
                                    if (conflictRow.Table.Columns.Contains("_RowVersion"))
                                    {
                                        recordRowVersion =
                                            BitConverter.ToInt64(
                                                DataObject.getOptionalVarBinary(conflictRow["_RowVersion"]),
                                                0);
                                    }

                                    bool canRetry = false;
                                    string errorMessage = SyncProviderHelperFM.TranslateConflictErrorMessage(
                                        tableName,
                                        e.Conflict.ErrorMessage,
                                        out canRetry);

                                    if (canRetry)
                                    {
                                        e.Action = ApplyAction.RetryNextSync;
                                    }
                                    else
                                    {
                                        e.Action = ApplyAction.Continue;
                                    }

                                    // Store the Record Conflict Record
                                    SyncProviderHelperFM.RecordSyncConflictEntry(
                                        syncContextFm.Security,
                                        syncSessionDetail,
                                        conflictType,
                                        syncContextFm.ClientID,
                                        syncContextFm.ClientName,
                                        tableName,
                                        resyncMinAnchor,
                                        resyncMaxAnchor,
                                        recordKey,
                                        recordRowVersion,
                                        e.Action,
                                        errorMessage,
                                        SyncProviderHelperFM.GetConflictApplyActionMessage(e.Action, true),
										commandText,
										commandType,
										command.Transaction,
										parameterDictionary);
;
                                }
                            }

                            break;

                        default:
                            for (int rowIndex = 0; rowIndex < clientChangeCount; rowIndex++)
                            {
                                DataRow conflictRow = e.Conflict.ClientChange.Rows[rowIndex];
                                long recordRowVersion = 0;

                                string recordKey = SyncProviderHelperFM.GetRecordKeyString(
                                    syncContextFm.SupportedColumnsByTable[tableName], conflictRow);

                                if (conflictRow.Table.Columns.Contains("_RowVersion"))
                                {
                                    recordRowVersion = BitConverter.ToInt64(DataObject.getOptionalVarBinary(conflictRow["_RowVersion"]), 0);
                                }

                                bool canRetry = false;
                                string errorMessage = SyncProviderHelperFM.TranslateConflictErrorMessage(
                                    tableName,
                                    e.Conflict.ErrorMessage,
                                    out canRetry);

                                if (canRetry)
                                {
                                    e.Action = ApplyAction.RetryNextSync;
                                }
                                else
                                {
                                    e.Action = ApplyAction.Continue;
                                }

								// Store the Record Conflict Record
								SyncProviderHelperFM.RecordSyncConflictEntry(
                                    syncContextFm.Security,
                                    syncSessionDetail,
                                    conflictType,
                                    syncContextFm.ClientID,
                                    syncContextFm.ClientName,
                                    tableName,
                                    resyncMinAnchor,
                                    resyncMaxAnchor,
                                    recordKey,
                                    recordRowVersion,
                                    e.Action,
                                    errorMessage,
                                    SyncProviderHelperFM.GetConflictApplyActionMessage(e.Action, true),
									commandText,
									commandType,
									command.Transaction,
									parameterDictionary);
                            }

                            break;
                    }
                }
            }

            // Utility.WriteYellow(e.TableMetadata.TableName, output.ToString());
        }

        #endregion Public Static Helper Methods

        #region Private Static Helper Methods

        #endregion Private Static Helper Methods
    }
}

