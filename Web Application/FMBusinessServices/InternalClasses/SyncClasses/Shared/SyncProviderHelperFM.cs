// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncProviderHelperFM.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the SyncProviderHelperFM type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Runtime.Remoting.Contexts;
	using System.Text;

	using FMBusinessObjects.DataObjects;

	using Microsoft.Synchronization.Data;

	public class SyncProviderHelperFM : IDisposable
	{
		public const int DEFAULT_SYNC_COMMAND_TIMEOUT = 1800;

		/// <summary>
		/// The client node type.
		/// </summary>
		public const string ClientNodeType = "Client";

		/// <summary>
		/// The server node type.
		/// </summary>
		public const string ServerNodeType = "Server";

		#region Attributes
		private bool _IsDisposed = false;
		#endregion Attributes

		#region Properties
		#endregion Properties

		#region Constructors / Destructors / Initializers
		public SyncProviderHelperFM()
		{
				InitializeViewModel();
		}
		~SyncProviderHelperFM()
		{
				Dispose(false);
		}
		private void InitializeViewModel()
		{
		}
		#endregion Constructors / Initializers

		#region Public Static Helper Methods
		/// <summary>
		/// Gets the maximum row versions.
		/// </summary>
		/// <param name="groupMetadata">The group metadata.</param>
		/// <param name="contextFM">The context fm.</param>
		/// <param name="syncScope">The synchronize scope.</param>
		/// <param name="syncContext">The synchronize context.</param>
		/// <param name="enterprise">if set to <c>true</c> [enterprise].</param>
		public static void GetMaxRowVersions(SyncGroupMetadata groupMetadata, SyncContextFM contextFM, SyncScopeDO syncScope, SyncContext syncContext, bool enterprise)
		{
			int tableIndex = 0;

			var removedRowsDictionary = new Dictionary<Guid, Dictionary<Guid, Guid>>();


			foreach (var tableMetadata in groupMetadata.TablesMetadata)
			{

				var syncTableToScopeMap = syncScope.SyncScopeTables.Find(x => tableMetadata.TableName.Substring(tableMetadata.TableName.IndexOf(".")+1) == x.ID);
				var dataTable = syncContext.DataSet.Tables[tableMetadata.TableName];
				if (syncTableToScopeMap != null && dataTable != null && dataTable.Columns.Contains("_RowVersion"))
				{
					long maxAnchor = 0;

					var syncTable = GetSyncTable(contextFM.Security, syncTableToScopeMap.SyncTableGuid);

					var syncTableToScopeMapColumnCollection = GetSyncTableColumns(contextFM.Security, syncTableToScopeMap.IdentityGuid);

					var primaryKeyColumnName = GetTablePrimaryKeyColumnName(syncTableToScopeMapColumnCollection);

					var removedPrimaryKeyDictionary = new Dictionary<Guid, Guid>();

					removedRowsDictionary.Add(syncTableToScopeMap.SyncTableGuid, removedPrimaryKeyDictionary);

					// Data may have been limited by top clause, determine maxAnchor
					if (!syncScope.SyncSinglePass
					|| tableIndex == 0
					|| syncContext.DataSet.Tables[groupMetadata.TablesMetadata[0].TableName] == null)
					{
						if(syncTableToScopeMap.MaxBatchSegmentRowCount > 0)
						{
							long maxInsertAnchor = 0;
							long maxUpdateAnchor = 0;
							long maxDeleteAnchor = 0;
							long rowCount = 0;

							var lastDataRowState = DataRowState.Added;

							foreach (DataRow row in dataTable.Rows)
							{
								if (lastDataRowState != row.RowState)
								{
									lastDataRowState = row.RowState;
									rowCount = 0;
								}

								long anchor = row.HasVersion(DataRowVersion.Default)
															? SyncDBI.ConvertRowVersion((byte[])row["_RowVersion"])
															: SyncDBI.ConvertRowVersion((byte[])row["_RowVersion", DataRowVersion.Original]);

								if (anchor > maxAnchor)
								{
									maxAnchor = anchor;
								}

								rowCount++;

								// Now we know that we have more records than what we can fit into a single batch segment.
								if (rowCount >= syncTableToScopeMap.MaxBatchSegmentRowCount)
								{
									if (row.RowState == DataRowState.Added)
									{
										maxInsertAnchor = anchor;
									}
									else if (row.RowState == DataRowState.Modified)
									{
										maxUpdateAnchor = anchor;
									}
									else
									{
										maxDeleteAnchor = anchor;
									}
								}
							}

							// Set maxAnchor to the lowest anchor for any extractor that returned MaxBatchSegmentRowCount
							if (maxInsertAnchor != 0 && maxAnchor > maxInsertAnchor)
							{
								maxAnchor = maxInsertAnchor;
							}

							if (maxUpdateAnchor != 0 && maxAnchor > maxUpdateAnchor)
							{
								maxAnchor = maxUpdateAnchor;
							}

							if (maxDeleteAnchor != 0 && maxAnchor > maxDeleteAnchor)
							{
								maxAnchor = maxDeleteAnchor;
							}

							if (maxInsertAnchor != 0 || maxUpdateAnchor != 0 || maxDeleteAnchor != 0)
							{
								contextFM.MaxBatchSegmentRowCountEncountered = true;

								// Remove any rows with anchor greater than maxAnchor, no need on initial sync as
								// update and delete extractors return no data
								if (contextFM.RequestType != SYNCREQUESTTYPE.INIT)
								{
									int rowIndex = 0;
									while (rowIndex < dataTable.Rows.Count)
									{
										long anchor = dataTable.Rows[rowIndex].HasVersion(DataRowVersion.Default)
																	? SyncDBI.ConvertRowVersion((byte[])dataTable.Rows[rowIndex]["_RowVersion"])
																	: SyncDBI.ConvertRowVersion((byte[])dataTable.Rows[rowIndex]["_RowVersion", DataRowVersion.Original]);

										Guid primaryKeyValue = Guid.Empty;

										if (!string.IsNullOrEmpty(primaryKeyColumnName))
										{
											primaryKeyValue = dataTable.Rows[rowIndex].HasVersion(DataRowVersion.Default)
												? (Guid)dataTable.Rows[rowIndex][primaryKeyColumnName]
												: (Guid) dataTable.Rows[rowIndex][primaryKeyColumnName,DataRowVersion.Original];
										}

										if (anchor > maxAnchor)
										{
											if (primaryKeyValue != Guid.Empty
											&& !removedPrimaryKeyDictionary.ContainsKey(primaryKeyValue))
											{
												removedPrimaryKeyDictionary.Add(primaryKeyValue, Guid.Empty);
											}

											dataTable.Rows.RemoveAt(rowIndex);
										}
										else
										{
											rowIndex++;
										}
									}
								}
							}
							else // result not limited by top clause
							{
								if (enterprise)
								{
									maxAnchor = contextFM.MaxEnterpriseSyncAnchor;
								}
								else
								{
									maxAnchor = contextFM.MaxClientSyncAnchor;
								}
							}
						}
						else // result not limited by top clause
						{
							if (enterprise)
							{
								maxAnchor = contextFM.MaxEnterpriseSyncAnchor;
							}
							else
							{
								maxAnchor = contextFM.MaxClientSyncAnchor;
							}
						}
					}

					// For SyncSinglePass - remove rows based upon parent table and foreign key
					Dictionary<Guid, Guid> parentRemovedPrimaryKeys = null;

					if (syncScope.SyncSinglePass
					&& tableIndex > 0
					&& syncContext.DataSet.Tables[groupMetadata.TablesMetadata[0].TableName] != null
					&& syncTable.ParentSyncTableGuid.HasValue
					&& !string.IsNullOrEmpty(syncTable.ParentForeignKeyColumnName)
					&& removedRowsDictionary.TryGetValue(syncTable.ParentSyncTableGuid.Value, out parentRemovedPrimaryKeys)
					&& parentRemovedPrimaryKeys.Count > 0)
					{
						int rowIndex = 0;

						while (rowIndex < dataTable.Rows.Count)
						{
							Guid? foreignKey = null;

							long anchor = dataTable.Rows[rowIndex].HasVersion(DataRowVersion.Default)
								? SyncDBI.ConvertRowVersion((byte[])dataTable.Rows[rowIndex]["_RowVersion"])
								: SyncDBI.ConvertRowVersion((byte[])dataTable.Rows[rowIndex]["_RowVersion", DataRowVersion.Original]);


							// Caution - We may not have the original parent foreign key value stored in some of the tracking tables, especially if they were deleted
							// prior to the introduction of this new SinglePass sync feature.  If we can't find it, we should let it delete by itself.
							if (dataTable.Rows[rowIndex].RowState == DataRowState.Deleted)
							{
								foreignKey = ((dataTable.Rows[rowIndex][syncTable.ParentForeignKeyColumnName, DataRowVersion.Original]) != DBNull.Value) ? (Guid?)(dataTable.Rows[rowIndex][syncTable.ParentForeignKeyColumnName, DataRowVersion.Original]) : (Guid?)null;
							}
							else
							{
								foreignKey = ((dataTable.Rows[rowIndex][syncTable.ParentForeignKeyColumnName]) != DBNull.Value) ? (Guid?)(dataTable.Rows[rowIndex][syncTable.ParentForeignKeyColumnName]) : (Guid?)null;
							}

							Guid primaryKeyValue = Guid.Empty;
							if (!string.IsNullOrEmpty(primaryKeyColumnName))
							{
								if (dataTable.Rows[rowIndex].RowState == DataRowState.Deleted)
								{
									primaryKeyValue = (Guid)dataTable.Rows[rowIndex][primaryKeyColumnName, DataRowVersion.Original];
								}
								else
								{
									primaryKeyValue = (Guid)dataTable.Rows[rowIndex][primaryKeyColumnName];
								}
							}


							if (foreignKey.HasValue)
							{
								// If this record's parent record was removed from the synchronization scope/session then this record should also be removed from
								// the current synchronization session.
								if (parentRemovedPrimaryKeys.ContainsKey(foreignKey.Value))
								{
									if (primaryKeyValue != Guid.Empty
									&& !removedPrimaryKeyDictionary.ContainsKey(primaryKeyValue))
									{
										removedPrimaryKeyDictionary.Add(primaryKeyValue, Guid.Empty);
									}

									dataTable.Rows.RemoveAt(rowIndex);
								}

								else
								{
									rowIndex++;
								}
							}
							else
							{
								rowIndex++;
							}
						}
					}

					else
					{

						// update anchor when end of scope
						if (!contextFM.MaxBatchSegmentRowCountEncountered)
						{
							if (enterprise)
							{
								maxAnchor = contextFM.MaxEnterpriseSyncAnchor;
							}
							else
							{
								maxAnchor = contextFM.MaxClientSyncAnchor;
							}
						}
					}

					// Update sync anchor if not SinglePass or Root Table or Root Table is empty
					// this will ensure that for single pass anchors are updated for tables that are extracted without #SyncTable such as tblPointTagAlarmStatus
					if (!syncScope.SyncSinglePass || tableIndex == 0 || syncContext.DataSet.Tables[groupMetadata.TablesMetadata[0].TableName] == null)
					{
						tableMetadata.LastReceivedAnchor.Anchor = SyncDBI.SerializeAnchorValue(maxAnchor);
					}

					dataTable.Columns.Remove("_RowVersion");
				}
				else
				{
					// No rows extracted, only update anchor when end of scope
					if (!syncScope.SyncSinglePass
					|| tableIndex == 0
					|| (!contextFM.MaxBatchSegmentRowCountEncountered && contextFM.SyncSinglePassPhase == SYNCSINGLEPASSPHASE.POSTROOT))
					{
						if (enterprise)
						{
							tableMetadata.LastReceivedAnchor.Anchor = SyncDBI.SerializeAnchorValue(contextFM.MaxEnterpriseSyncAnchor);
						}
						else
						{
							tableMetadata.LastReceivedAnchor.Anchor = SyncDBI.SerializeAnchorValue(contextFM.MaxClientSyncAnchor);
						}
					}
				}

				tableIndex++;
			}

			if (syncScope.SyncSinglePass)
			{
				if (!contextFM.MaxBatchSegmentRowCountEncountered)
				{
					if (contextFM.SyncSinglePassPhase == SYNCSINGLEPASSPHASE.SYNCROOT)
					{
						contextFM.SyncSinglePassPhase = SYNCSINGLEPASSPHASE.POSTROOT;
					}
					else
					{
						contextFM.SyncSinglePassPhase = SYNCSINGLEPASSPHASE.COMPLETE;
					}
				}

				// if during scope, root table returns rows switch back to SYNCROOT
				else if(syncContext.DataSet.Tables[groupMetadata.TablesMetadata[0].TableName] != null)
				{
					contextFM.SyncSinglePassPhase = SYNCSINGLEPASSPHASE.SYNCROOT;
				}
			}
		}

		public static string ApplyNodeTypeMask(string inputString, string maskValue)
		{
				string maskedOutput = inputString;

				if (inputString.Contains("[NodeType]"))
					maskedOutput = inputString.Replace("[NodeType]", maskValue);

				return (maskedOutput);
		}
		public static SyncTableDO GetSyncTable(SecurityClass security, Guid syncTableGuid)
		{
				SyncTableDO syncTable = null;

				using (SyncTableDBI dbi = new SyncTableDBI(security.UserID))
				{
					syncTable = dbi.Get(security, syncTableGuid, null);
				}

				if (null == syncTable)
					throw new Exception(string.Format("Unable to locate SyncTable information for specified SyncTable identifier: {0}", syncTableGuid));

				return (syncTable);
		}
		public static SyncTableToScopeMapCommandDO GetSyncTableCommands(SecurityClass security, Guid syncTableToScopeMapGuid)
		{
				SyncTableToScopeMapCommandDO syncCommands = null;

				using (SyncTableToScopeMapCommandDBI dbi = new SyncTableToScopeMapCommandDBI(security.UserID))
				{
					syncCommands = dbi.Get(security, syncTableToScopeMapGuid, null);
				}

				if (null == syncCommands)
					throw new Exception(string.Format("Unable to locate Sync Commands information for specified synchronization table to scope mapping identifier: {0}", syncTableToScopeMapGuid));

				return (syncCommands);
		}
		public static SyncTableToScopeMapColumnCollection GetSyncTableColumns(SecurityClass security, Guid syncTableToScopeMapGuid)
		{
				SyncTableToScopeMapColumnCollection syncColumns = null;

				using (SyncTableToScopeMapColumnDBI dbi = new SyncTableToScopeMapColumnDBI(security.UserID))
				{
					syncColumns = dbi.GetList(security, syncTableToScopeMapGuid);
				}

				if (null == syncColumns)
					throw new Exception(string.Format("Unable to locate Sync Columns for specified synchronization table to scope mapping identifier: {0}", syncTableToScopeMapGuid));

				return (syncColumns);
		}

		public static string GetTablePrimaryKeyColumnName(SyncTableToScopeMapColumnCollection columnCollection)
		{
				StringBuilder keyColumn = new StringBuilder();

				var sorted = from s in columnCollection where s.IsPrimaryKeyMemberFlag orderby s.ColumnIndex select s;

				foreach (var s in sorted)
				{
					keyColumn.Append(s.ColumnName);
				}

				return keyColumn.ToString();
		}

		public static string GetRecordKeyString(SyncTableToScopeMapColumnCollection columnCollection, DataRow row)
		{
				StringBuilder keyString = new StringBuilder();

				var sorted = from s in columnCollection where s.IsPrimaryKeyMemberFlag orderby s.ColumnIndex select s;

				foreach (var s in sorted)
				{
					if (row.RowState == DataRowState.Deleted)
					{
					keyString.Append(string.Format("{0}", row[s.ColumnName, DataRowVersion.Original]));
					}
					else
					{
					keyString.Append(string.Format("{0}", row[s.ColumnName]));
					}
				}

				return keyString.ToString();
		}

		public static string TranslateConflictErrorMessage(string tableName, string syncConflictMessage, out bool canAutoRetry)
		{
				string parsedMessage = syncConflictMessage;
				string tableNameOnly = SyncHelperFM.GetNamePartFromTableName(tableName, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME);

				canAutoRetry = false;

				// Remove any tbl prefix that we may have on the name.
				if (tableNameOnly.StartsWith("tbl"))
				{
					tableNameOnly = tableNameOnly.Substring(3);
				}

				if (!string.IsNullOrEmpty(syncConflictMessage))
				{
					// Ex: Cannot insert duplicate key row in object 'dbo.tblProducts'-- with unique index 'IXU_tblProducts_ProductID_SiteGuid'
					if (syncConflictMessage.Contains("Cannot insert duplicate key row")
						&& syncConflictMessage.Contains("with unique index"))
					{
						parsedMessage = string.Format(@"Duplicate {0} with the same ID already exists.", tableNameOnly);
					}
					else if (syncConflictMessage.Contains("conflicted with the FOREIGN KEY constraint"))
					{
						// Ex: The MERGE statement conflicted with the FOREIGN KEY constraint "FK_map_tblEntityProductToSite_ProductGuid". The conflict occurred in database "FuelsManagerDB", table "dbo.tblProducts", column 'ProductGuid'.
						// Ex: The MERGE statement conflicted with the FOREIGN KEY constraint "FK_map_tblProductToCompany_ProductIndex". The conflict occurred in database "FuelsManagerDB", table "dbo.tblProducts", column 'ProductGuid'.
						parsedMessage =
							"Entity references another that does not exist. Have all referenced entities been assigned to the target Site?";
						canAutoRetry = true;
					}
				}

				return parsedMessage;
		}

		/// <summary>
		/// The get conflict apply action message.
		/// </summary>
		/// <param name="action">
		/// The action.
		/// </param>
		/// <param name="forClientProvider"></param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GetConflictApplyActionMessage(ApplyAction action, bool forClientProvider)
		{
				string actionMessage;

				switch (action)
				{
					case ApplyAction.Continue:
						actionMessage = "Continue synchronization.";
						break;
					case ApplyAction.RetryApplyingRow:
						actionMessage = "Retry applying the change.";
						break;
					case ApplyAction.RetryWithForceWrite:
						if (forClientProvider)
						{
								actionMessage = "Retry with forced overwrite option. Server has a newer record.";
						}
						else
						{
								actionMessage = "Retry with forced overwrite option. Client has a newer record.";
						}

						break;
					default:
						actionMessage = "Retry during next synchronization session.";
						break;
				}

				return actionMessage;
		}

		/// <summary>
		/// The record sync conflict entry.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncSessionLogDetail">The sync session log detail.</param>
		/// <param name="syncConflictType">The sync conflict type.</param>
		/// <param name="targetNodeGuid">The target nod guid.</param>
		/// <param name="targetNodeName">The target nod guid.</param>
		/// <param name="tableName">The table name.</param>
		/// <param name="resyncAnchorMin">The re sync anchor min.</param>
		/// <param name="resyncAnchorMax">The re sync anchor max.</param>
		/// <param name="recordKey">The record key.</param>
		/// <param name="recordRowVersion">The record row version.</param>
		/// <param name="selectedAction">The selected action.</param>
		/// <param name="conflictErrorMessage">The conflict error message.</param>
		/// <param name="conflictResultMessage">The conflict result message.</param>
		/// <param name="commandText">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="parameters">The parameters.</param>
		public static void RecordSyncConflictEntry(
				SecurityClass security,
				SyncSessionScopeLogDO syncSessionLogDetail,
				SYNCCONFLICTTYPE syncConflictType,
				Guid targetNodeGuid,
				string targetNodeName,
				string tableName,
				long resyncAnchorMin,
				long resyncAnchorMax,
				string recordKey,
				long recordRowVersion,
				ApplyAction selectedAction,
				string conflictErrorMessage,
				string conflictResultMessage,
			string commandText,
			CommandType commandType,
			IDbTransaction transaction,
			Dictionary<string, object> parameters)
		{
				StringBuilder rowError = new StringBuilder();

				SyncRecordConflictDO existingConflict = new SyncRecordConflictDO();
				existingConflict.SyncConflictResolutionStatusIndex = (selectedAction == ApplyAction.RetryNextSync) ? SYNCCONFLICTRESOLUTIONSTATUS.AUTORETRY : SYNCCONFLICTRESOLUTIONSTATUS.PENDING;
				existingConflict.SyncConflictTypeIndex = syncConflictType;
				existingConflict.TableName = tableName;
				existingConflict.TargetNodeGuid = targetNodeGuid; // THIS SHOULD BE THE CLIENT NODE
				existingConflict.TargetNodeName = targetNodeName;
				existingConflict.RecordKey = recordKey;
				existingConflict.RecordRowVersion = recordRowVersion;
				existingConflict.ReSyncAnchorMin = resyncAnchorMin;
				existingConflict.ReSyncAnchorMax = resyncAnchorMax;

				rowError.AppendLine(
					string.Format(
						"Conflict Type: {0}{1}",
						SyncTypes.GetSyncConflictTypeString(existingConflict.SyncConflictTypeIndex),
						Environment.NewLine));

				if (!string.IsNullOrEmpty(conflictErrorMessage))
				{
					rowError.AppendLine(string.Format(
						"Conflict Error Message: {0}{1}",
						conflictErrorMessage,
						Environment.NewLine));
				}

				if (!string.IsNullOrEmpty(conflictResultMessage))
				{
					rowError.AppendLine(conflictResultMessage);
				}

			existingConflict.ConflictDescription = rowError.ToString();
			existingConflict.CommandText = commandText;
			existingConflict.CommandType = commandType;
			existingConflict.Parameters = parameters;
			
			using (var conflictDbi = new SyncRecordConflictDBI(security.UserID))
				{
			
				conflictDbi.Transaction = (SqlTransaction)transaction;
				conflictDbi.Save(security, syncSessionLogDetail, existingConflict);
				}
		}

		/// <summary>
		/// The get child sites for current site.
		/// </summary>
		/// <param name="siteList">
		/// The p site list.
		/// </param>
		/// <param name="currentSite">
		/// The p current site.
		/// </param>
		/// <returns>
		/// The <see cref="ArrayList"/>.
		/// </returns>
		public static ArrayList GetChildSitesForCurrentSite(SiteCollectionClass siteList, SiteClass currentSite)
		{
				var childSiteList = new ArrayList();

				bool pastCurrentSite = false;

				foreach (SiteClass sc in siteList)
				{
					if (!pastCurrentSite)
					{
						if (sc.ID == currentSite.ID)
						{
								pastCurrentSite = true;
						}

						continue;
					}

					if (!childSiteList.Contains(sc))
					{
						childSiteList.Add(sc);
					}
				}

				return childSiteList;
		}

		/// <summary>
		/// The get SQL DB type from string.
		/// </summary>
		/// <param name="sqlDbType">
		/// The SQL DB type.
		/// </param>
		/// <returns>
		/// The <see cref="SqlDbType"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Throws an exception if the incoming DB type is NULL.
		/// </exception>
		public static SqlDbType GetSqlDbTypeFromString(string sqlDbType)
		{
				if (sqlDbType == null)
				{
					throw new ArgumentNullException("sqlDbType");
				}

				return (SqlDbType)Enum.Parse(typeof(SqlDbType), sqlDbType, true);
		}
		#endregion Public Static Helper Methods

		#region Anchor Command

		/// <summary>
		/// Create a command to retrieve a new anchor value from
		/// the server. In this case, we use a timestamp value
		/// that is retrieved and stored in the client database.
		/// During each synchronization, the new anchor value and
		/// the last anchor value from the previous synchronization
		/// are used: the set of changes between these upper and
		/// lower bounds is synchronized.
		///
		/// SyncSession.SyncNewReceivedAnchor is a string constant; 
		/// you could also use @sync_new_received_anchor directly in 
		/// your queries.
		/// </summary>
		/// <returns></returns>
		public static IDbCommand CreateAnchorCommand(bool isBatching, string providerType)
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;

				if (isBatching)
				{
					if (providerType.Equals(
						SyncProviderHelperFM.ClientNodeType,
						StringComparison.InvariantCultureIgnoreCase))
					{
						cmd.CommandText = "sync.usp_GetNewClientBatchAnchor";
						cmd.Parameters.Add(SyncParamsFM.SYNC_MAX_CLIENT_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction =
								ParameterDirection.Input;
					}
					else
					{
						cmd.CommandText = "sync.usp_GetNewServerBatchAnchor";
						cmd.Parameters.Add(SyncParamsFM.SYNC_MAX_SERVER_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction =
								ParameterDirection.Input;
					}
					cmd.Parameters.Add(SyncParamsFM.SYNC_MAX_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.InputOutput;
					cmd.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
					cmd.Parameters.Add(SyncParamsFM.SYNC_BATCH_SIZE_PARAMETER, SqlDbType.Int);
					cmd.Parameters.Add(SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.Output;
					cmd.Parameters.Add(SyncParamsFM.SYNC_BATCH_COUNT_PARAMETER, SqlDbType.Int).Direction = ParameterDirection.InputOutput;
				}
				else
				{
					cmd.CommandText = "sync.usp_GetNewAnchor";
					cmd.Parameters.Add(SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.Output;
				}

				return (cmd);
		}

		public static IDbCommand CreateStaticAnchorCommand(bool isBatching, string providerType, long maxSyncAnchor)
		{
				var cmd = new SqlCommand();

				if (isBatching)
				{
					cmd.CommandType = CommandType.StoredProcedure;

					if (providerType.Equals(
						SyncProviderHelperFM.ClientNodeType,
						StringComparison.InvariantCultureIgnoreCase))
					{
						cmd.CommandText = "sync.usp_GetNewClientBatchAnchor";
						cmd.Parameters.Add(SyncParamsFM.SYNC_MAX_CLIENT_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction =
								ParameterDirection.Input;
					}
					else
					{
						cmd.CommandText = "sync.usp_GetNewServerBatchAnchor";
						cmd.Parameters.Add(SyncParamsFM.SYNC_MAX_SERVER_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction =
								ParameterDirection.Input;
					}
					cmd.Parameters.Add(SyncParamsFM.SYNC_MAX_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction =
						ParameterDirection.InputOutput;
					cmd.Parameters.Add(SyncParamsFM.SYNC_LAST_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt);
					cmd.Parameters.Add(SyncParamsFM.SYNC_BATCH_SIZE_PARAMETER, SqlDbType.Int);
					cmd.Parameters.Add(SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction =
						ParameterDirection.Output;
					cmd.Parameters.Add(SyncParamsFM.SYNC_BATCH_COUNT_PARAMETER, SqlDbType.Int).Direction =
						ParameterDirection.InputOutput;
				}
				else
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = string.Format("SELECT @sync_new_received_anchor = {0}", maxSyncAnchor);
					cmd.Parameters.Add(SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction =
						ParameterDirection.Output;
				}

				return (cmd);
		}

		/// <summary>
		/// Create a command to retrieve the current max anchor value (-1) from
		/// the system. 
		///
		/// SyncSession.SyncNewReceivedAnchor is a string constant; 
		/// you could also use @sync_new_received_anchor directly in 
		/// your queries.
		/// </summary>
		/// <returns></returns>
		public static IDbCommand CreateMaxAnchorCommand()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.CommandText = "sync.usp_GetNewAnchor";
				cmd.Parameters.Add(SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.Output;

				return (cmd);
		}

		/// <summary>
		/// Creates a Command used to select the last received anchor for a given table.
		/// </summary>
		/// <returns>IDbCommand</returns>
		/// <remarks>This method is used by client synchronization providers and depends on the "Anchor" table.</remarks>
		public static IDbCommand CreateSelectTableReceivedAnchorCommand()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sync.usp_SyncAnchorSelectLastReceivedAnchorByTableName";

				cmd.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar, 30);
				cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 256); // it can't auto-add it (because it operates at abstract level)
				cmd.Parameters.Add(SyncParamsFM.SYNC_TABLE_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.Output;

				return (cmd);
		}

		/// <summary>
		/// Creates a Command used to update the last received anchor for a given table.
		/// </summary>
		/// <returns>IDbCommand</returns>
		/// <remarks>This method is used by client synchronization providers and depends on the "Anchor" table.</remarks>
		public static IDbCommand CreateUpdateTableReceivedAnchorCommand()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sync.usp_SyncAnchorUpdateLastReceivedAnchorByTableName";

				cmd.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar, 30);
				cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 256); // it can't auto-add it (because it operates at abstract level)
				cmd.Parameters.Add(SyncParamsFM.SYNC_ANCHOR_VALUE_PARAMETER, SqlDbType.BigInt); // it can't auto-add it (because it operates at abstract level)

				return (cmd);
		}
		/// <summary>
		/// Creates a Command used to select the last received anchor for a given table.
		/// </summary>
		/// <returns>IDbCommand</returns>
		/// <remarks>This method is used by client synchronization providers and depends on the "Anchor" table.</remarks>
		public static IDbCommand CreateSelectTableReceivedAnchor2Command()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sync.usp_SyncAnchorSelectLastReceivedAnchor2ByTableName";

				cmd.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar, 30);
				cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 256); // it can't auto-add it (because it operates at abstract level)
				cmd.Parameters.Add(SyncParamsFM.SYNC_TABLE_RECEIVED_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.Output;

				return (cmd);
		}
		/// <summary>
		/// Creates a Command used to update the last received anchor for a given table.
		/// </summary>
		/// <returns>IDbCommand</returns>
		/// <remarks>This method is used by client synchronization providers and depends on the "Anchor" table.</remarks>
		public static IDbCommand CreateUpdateTableReceivedAnchor2Command()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sync.usp_SyncAnchorUpdateLastReceivedAnchor2ByTableName";

				cmd.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar, 30);
				cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 256); // it can't auto-add it (because it operates at abstract level)
				cmd.Parameters.Add(SyncParamsFM.SYNC_ANCHOR_VALUE_PARAMETER, SqlDbType.BigInt); // it can't auto-add it (because it operates at abstract level)

				return (cmd);
		}
		/// <summary>
		/// Creates a Command used to select the last sent anchor for a given table.
		/// </summary>
		/// <returns>IDbCommand</returns>
		/// <remarks>This method is used by client synchronization providers and depends on the "Anchor" table.</remarks>
		public static IDbCommand CreateSelectTableSentAnchorCommand()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sync.usp_SyncAnchorSelectLastSentAnchorByTableName";

				cmd.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar, 30);
				cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 256); // it can't auto-add it (because it operates at abstract level)
				cmd.Parameters.Add(SyncParamsFM.SYNC_TABLE_SENT_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.Output;

				return (cmd);
		}
		/// <summary>
		/// Creates a Command used to update the last sent anchor for a given table.
		/// </summary>
		/// <returns>IDbCommand</returns>
		/// <remarks>This method is used by client synchronization providers and depends on the "Anchor" table.</remarks>
		public static IDbCommand CreateUpdateTableSentAnchorCommand()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sync.usp_SyncAnchorUpdateLastSentAnchorByTableName";

				cmd.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar, 30);
				cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 256); // it can't auto-add it (because it operates at abstract level)
				cmd.Parameters.Add(SyncParamsFM.SYNC_ANCHOR_VALUE_PARAMETER, SqlDbType.BigInt); // it can't auto-add it (because it operates at abstract level)

				return (cmd);
		}
		/// <summary>
		/// Creates a Command used to select the last sent anchor2 for a given table.
		/// </summary>
		/// <returns>IDbCommand</returns>
		/// <remarks>This method is used by client synchronization providers and depends on the "Anchor" table.</remarks>
		public static IDbCommand CreateSelectTableSentAnchor2Command()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sync.usp_SyncAnchorSelectLastSentAnchor2ByTableName";

				cmd.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar, 30);
				cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 256); // it can't auto-add it (because it operates at abstract level)
				cmd.Parameters.Add(SyncParamsFM.SYNC_TABLE_SENT_ANCHOR_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.Output;
				return (cmd);
		}
		/// <summary>
		/// Creates a Command used to update the last sent anchor2 for a given table.
		/// </summary>
		/// <returns>IDbCommand</returns>
		/// <remarks>This method is used by client synchronization providers and depends on the "Anchor" table.</remarks>
		public static IDbCommand CreateUpdateTableSentAnchor2Command()
		{
				var cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sync.usp_SyncAnchorUpdateLastSentAnchor2ByTableName";

				cmd.Parameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, SqlDbType.NVarChar, 30);
				cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 256); // it can't auto-add it (because it operates at abstract level)
				cmd.Parameters.Add(SyncParamsFM.SYNC_ANCHOR_VALUE_PARAMETER, SqlDbType.BigInt); // it can't auto-add it (because it operates at abstract level)

				return (cmd);
		}
		#endregion // Anchor Command

		#region Handle Events

		public static void ChangesSelected(object sender, ChangesSelectedEventArgs e)
		{
				StringBuilder output = new StringBuilder();

				long totalUpdates = e.Context.GroupProgress.TotalUpdates / 2;

				output.AppendLine("Client ID: " + e.Session.ClientId);
				output.AppendLine("Changes selected for group " + e.GroupMetadata.GroupName);
				output.AppendLine("Inserts selected for group: " + e.Context.GroupProgress.TotalInserts.ToString());
				output.AppendLine("Updates selected for group: " + totalUpdates.ToString());
				output.AppendLine("Deletes selected for group: " + e.Context.GroupProgress.TotalDeletes.ToString());

				//Utility.WriteGreen(e.GroupMetadata.GroupName, output.ToString());
		}

		public static void ChangesApplied(object sender, ChangesAppliedEventArgs e)
		{
				StringBuilder output = new StringBuilder();

				long totalUpdates = e.Context.GroupProgress.TotalUpdates / 2;

				output.AppendLine("Client ID: " + e.Session.ClientId);
				output.AppendLine("Changes applied for group " + e.GroupMetadata.GroupName);
				output.AppendLine("Inserts applied for group: " + e.Context.GroupProgress.TotalInserts.ToString());
				output.AppendLine("Updates applied for group: " + totalUpdates.ToString());
				output.AppendLine("Deletes applied for group: " + e.Context.GroupProgress.TotalDeletes.ToString());

				//Utility.WriteBlue(e.GroupMetadata.GroupName, output.ToString());
		}

		// Here we can intercept changes before they are applied.
		// We can alter or prevent the changes from occuring.
		public static void ApplyingChanges(object sender, ApplyingChangesEventArgs e)
		{
				if (e == null || e.Changes == null || e.Changes.Tables == null)
					return;

				StringBuilder output = new StringBuilder();

				foreach (DataTable table in e.Changes.Tables)
				{
					// table name
					output.AppendLine(table.TableName);

					// column captions
					List<string> captions = GetCaptions(table.Columns);

					foreach (string caption in captions)
					{
						output.Append(caption + " ");
					}

					output.Append(Environment.NewLine);

					// row data for modified rows
					foreach (DataRow row in table.Rows)
					{
						// Only print modified rows (cant access deleted ones)
						if (row.RowState == DataRowState.Modified)
						{
								foreach (string caption in captions)
								{
									output.Append(row[caption] + " ");
								}

								output.Append(Environment.NewLine);
						}
					}

					output.Append(Environment.NewLine);
				}

				//Utility.WriteBlue(e.GroupMetadata.GroupName, output.ToString());
		}

		/// <summary>
		/// Get the column captions.
		/// </summary>
		private static List<string> GetCaptions(DataColumnCollection columns)
		{
				List<string> captions = new List<string>();

				foreach (DataColumn column in columns)
				{
					captions.Add(column.Caption);
				}

				return captions;
		}

		#endregion // Handle Events

		#region IDisposable Members
		public void Dispose()
		{
				Dispose(true);
				GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Dispose of any managed and unmanaged resources that we might control.  Only release managed resources if we were not called by the runtime Finalizer.
		/// </summary>
		/// <param name="disposing">True if we're explicitly called (meaning we're still a valid managed object); Else False if called by the .NET runtime Finalizer (it called our destructor)</param>
		protected void Dispose(bool disposing)
		{
				// If we fail to release our managed resources for some reason, we'll never set the _IsDisposed so when the Finalizer calls us, 
				// we'll still have an opportunity to come in here and clean up any unmanaged resources.
				//
				if (!_IsDisposed)
				{
					// If we are being asked to dispose ourselves, we can safely clean up any managed resources if needed.
					if (disposing)
					{
					}

					// We should always release un-managed resources (if we have any) regardless of the "disposing" value.
					//
					// Add un-managed resources here:
				}

				_IsDisposed = true;
		}
		#endregion IDisposable Members
	}
}

