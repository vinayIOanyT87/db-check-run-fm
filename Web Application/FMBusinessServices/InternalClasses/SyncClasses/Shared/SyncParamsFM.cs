// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncParamsFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses
{
    /// <summary>
    /// The sync params fm.
    /// </summary>
    public static class SyncParamsFM
    {
        #region Public Properties

        /// <summary>
        /// /// Gets the sync parameter name for anchor value parameter.
        /// </summary>
        public static string SYNC_ANCHOR_VALUE_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncAnchorValueName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for batch count parameter.
        /// </summary>
        public static string SYNC_BATCH_COUNT_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncBatchCountName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for batch size parameter.
        /// </summary>
        public static string SYNC_BATCH_SIZE_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncBatchSizeName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for bypass  delete  extraction  parameter.
        /// </summary>
        public static string SYNC_BYPASS_DELETE_EXTRACTION_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncBypassDeleteExtractionName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for bypass  insert  update  extraction  parameter.
        /// </summary>
        public static string SYNC_BYPASS_INSERT_UPDATE_EXTRACTION_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncBypassInsertUpdateExtractionName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for client  id  binary  parameter.
        /// </summary>
        public static string SYNC_CLIENT_ID_BINARY_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncClientIDBinaryName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for client  id  hash  parameter.
        /// </summary>
        public static string SYNC_CLIENT_ID_HASH_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncClientIDHashName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for client  id  parameter.
        /// </summary>
        public static string SYNC_CLIENT_ID_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncClientIDName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for context  child  site  guid  list  parameter.
        /// </summary>
        public static string SYNC_CONTEXT_CHILD_SITE_GUID_LIST_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncContextChildSiteGuidListName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for context  child  site  guid  list  parameter.
        /// </summary>
        public static string SYNC_CONTEXT_SITE_GUID_LIST_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncContextSiteGuidListName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for context  child  site  guid  list  parameter.
        /// </summary>
        public static string SYNC_CONTEXT_SITE_ID_LIST_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncContextSiteIDListName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for context  site  guid  parameter.
        /// </summary>
        public static string SYNC_CONTEXT_SITE_GUID_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncContextSiteGuidName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for context  site  id  parameter.
        /// </summary>
        public static string SYNC_CONTEXT_SITE_ID_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncContextSiteIDName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for current  site  guid  parameter.
        /// </summary>
        public static string SYNC_CURRENT_SITE_GUID_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncCurrentSiteGuidName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for current  site  id  parameter.
        /// </summary>
        public static string SYNC_CURRENT_SITE_ID_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncCurrentSiteIDName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for current  table  parameter.
        /// </summary>
        public static string SYNC_CURRENT_TABLE_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncCurrentTableName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for force  write  parameter.
        /// </summary>
        public static string SYNC_FORCE_WRITE_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncForceWriteName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for group  name  parameter.
        /// </summary>
        public static string SYNC_GROUP_NAME_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncGroupName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for initialized parameter.
        /// </summary>
        public static string SYNC_INITIALIZED_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncInitializedName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for request type parameter.
        /// </summary>
        public static string SYNC_REQUEST_TYPE_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncRequestTypeName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for last  received  anchor  parameter.
        /// </summary>
        public static string SYNC_LAST_RECEIVED_ANCHOR_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncLastReceivedAnchorName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for max  received  anchor  parameter.
        /// </summary>
        public static string SYNC_MAX_RECEIVED_ANCHOR_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncMaxReceivedAnchorName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for new  received  anchor  parameter.
        /// </summary>
        public static string SYNC_NEW_RECEIVED_ANCHOR_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncNewReceivedAnchorName);
            }
        }

        /// <summary>
        /// Gets the sync parameter name for the max client anchor string parameter
        /// </summary>
        public static string SYNC_MAX_CLIENT_ANCHOR_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncMaxClientAnchorName);
            }
        }

        /// <summary>
        /// Gets the sync parameter name for the max server anchor string parameter
        /// </summary>
        public static string SYNC_MAX_SERVER_ANCHOR_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncMaxServerAnchorName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for originator  id  parameter.
        /// </summary>
        public static string SYNC_ORIGINATOR_ID_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncOriginatorIdName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for row  count  parameter.
        /// </summary>
        public static string SYNC_ROW_COUNT_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncRowCountName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for server  id  binary  parameter.
        /// </summary>
        public static string SYNC_SERVER_ID_BINARY_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncServerIDBinaryName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for server  id  parameter.
        /// </summary>
        public static string SYNC_SERVER_ID_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncServerIDName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for session  id  parameter.
        /// </summary>
        public static string SYNC_SESSION_ID_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncSessionIDName);
            }
        }

        /// <summary>
        /// Gets the sync_start_daterange_parameter.
        /// </summary>
        public static string SYNC_START_DATERANGE_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncStartDateRange);
            }
        }

        /// <summary>
        /// Gets the sync_end_daterange_parameter.
        /// </summary>
        public static string SYNC_END_DATERANGE_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncEndDateRange);
            }
        }

        /// <summary>
        /// Gets the sync_filter_by_daterange_parameter.
        /// </summary>
        public static string SYNC_FILTER_BY_DATERANGE_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncFilterByDateRange);
            }
        }

        /// <summary>
        /// Gets the sync_supported_columns_parameter.
        /// </summary>
        public static string SYNC_SUPPORTED_COLUMNS_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncSupportedColumns);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for table  name  parameter.
        /// </summary>
        public static string SYNC_TABLE_NAME_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncTableName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for table  received  anchor  parameter.
        /// </summary>
        public static string SYNC_TABLE_RECEIVED_ANCHOR_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncTableReceivedAnchorName);
            }
        }

        /// <summary>
        /// /// Gets the sync parameter name for table  sent  anchor  parameter.
        /// </summary>
        public static string SYNC_TABLE_SENT_ANCHOR_PARAMETER
        {
            get
            {
                return GetFormattedParameterString(SyncTableSentAnchorName);
            }
        }

		/// <summary>
		/// /// Gets the sync parameter name for first time sync option parameter.
		/// </summary>
		public static string SYNC_FIRST_TIME_SYNC_OPTION
		{
			get
			{
				return GetFormattedParameterString(SyncFirstTimeSyncOption);
			}
		}

		/// <summary>
		/// Gets the sync anchor value name.
		/// </summary>
		public static string SyncAnchorValueName
        {
            get
            {
                return "sync_anchor";
            }
        }

        /// <summary>
        /// Gets the sync batch count name.
        /// </summary>
        public static string SyncBatchCountName
        {
            get
            {
                return "sync_batch_count";
            }
        }

        /// <summary>
        /// Gets the sync batch size name.
        /// </summary>
        public static string SyncBatchSizeName
        {
            get
            {
                return "sync_batch_size";
            }
        }

        /// <summary>
        /// Gets the sync bypass delete extraction name.
        /// </summary>
        public static string SyncBypassDeleteExtractionName
        {
            get
            {
                return "sync_bypass_delete_extraction";
            }
        }

        /// <summary>
        /// Gets the sync bypass insert update extraction name.
        /// </summary>
        public static string SyncBypassInsertUpdateExtractionName
        {
            get
            {
                return "sync_bypass_insert_update_extraction";
            }
        }

        /// <summary>
        /// Gets the sync client id binary name.
        /// </summary>
        public static string SyncClientIDBinaryName
        {
            get
            {
                return "sync_client_id_binary";
            }
        }

        /// <summary>
        /// Gets the sync client id hash name.
        /// </summary>
        public static string SyncClientIDHashName
        {
            get
            {
                return "sync_client_id_hash";
            }
        }

        /// <summary>
        /// Gets the sync client id name.
        /// </summary>
        public static string SyncClientIDName
        {
            get
            {
                return "sync_client_id";
            }
        }

        /// <summary>
        /// Gets the sync context child site guid list name.
        /// </summary>
        public static string SyncContextChildSiteGuidListName
        {
            get
            {
                return "sync_context_child_site_guid_list";
            }
        }

        /// <summary>
        /// Gets the sync context site guid list name.
        /// </summary>
        public static string SyncContextSiteGuidListName
        {
            get
            {
                return "sync_context_site_guid_list";
            }
        }

        /// <summary>
        /// Gets the sync context site id list name.
        /// </summary>
        public static string SyncContextSiteIDListName
        {
            get
            {
                return "sync_context_site_id_list";
            }
        }

        /// <summary>
        /// Gets the sync context site guid name.
        /// </summary>
        public static string SyncContextSiteGuidName
        {
            get
            {
                return "sync_context_site_guid";
            }
        }

        /// <summary>
        /// Gets the sync context site id name.
        /// </summary>
        public static string SyncContextSiteIDName
        {
            get
            {
                return "sync_context_site_id";
            }
        }

        /// <summary>
        /// Gets the sync current site guid name.
        /// </summary>
        public static string SyncCurrentSiteGuidName
        {
            get
            {
                return "sync_current_site_guid";
            }
        }

        /// <summary>
        /// Gets the sync current site id name.
        /// </summary>
        public static string SyncCurrentSiteIDName
        {
            get
            {
                return "sync_current_site_id";
            }
        }

        /// <summary>
        /// Gets the sync current table name.
        /// </summary>
        public static string SyncCurrentTableName
        {
            get
            {
                return "sync_current_table";
            }
        }

        /// <summary>
        /// Gets the sync force write name.
        /// </summary>
        public static string SyncForceWriteName
        {
            get
            {
                return "sync_force_write";
            }
        }

        /// <summary>
        /// Gets the sync group name.
        /// </summary>
        public static string SyncGroupName
        {
            get
            {
                return "sync_group_name";
            }
        }

        /// <summary>
        /// Gets the sync initialized name.
        /// </summary>
        public static string SyncInitializedName
        {
            get
            {
                return "sync_initialized";
            }
        }

        /// <summary>
        /// Gets the sync request type name.
        /// </summary>
        public static string SyncRequestTypeName
        {
            get
            {
                return "sync_request_type";
            }
        }

        /// <summary>
        /// Gets the sync last received anchor name.
        /// </summary>
        public static string SyncLastReceivedAnchorName
        {
            get
            {
                return "sync_last_received_anchor";
            }
        }

        /// <summary>
        /// Gets the sync max received anchor name.
        /// </summary>
        public static string SyncMaxReceivedAnchorName
        {
            get
            {
                return "sync_max_received_anchor";
            }
        }

        /// <summary>
        /// Gets the sync max client anchor name.
        /// </summary>
        public static string SyncMaxClientAnchorName
        {
            get
            {
                return "sync_max_client_anchor";
            }
        }

        /// <summary>
        /// Gets the sync max server anchor name.
        /// </summary>
        public static string SyncMaxServerAnchorName
        {
            get
            {
                return "sync_max_server_anchor";
            }
        }

        /// <summary>
        /// Gets the sync new received anchor name.
        /// </summary>
        public static string SyncNewReceivedAnchorName
        {
            get
            {
                return "sync_new_received_anchor";
            }
        }

        /// <summary>
        /// Gets the sync originator id name.
        /// </summary>
        public static string SyncOriginatorIdName
        {
            get
            {
                return "sync_originator_id";
            }
        }

        /// <summary>
        /// Gets the sync row count name.
        /// </summary>
        public static string SyncRowCountName
        {
            get
            {
                return "sync_row_count";
            }
        }

        /// <summary>
        /// Gets the sync server id binary name.
        /// </summary>
        public static string SyncServerIDBinaryName
        {
            get
            {
                return "sync_server_id_binary";
            }
        }

        /// <summary>
        /// Gets the sync server id name.
        /// </summary>
        public static string SyncServerIDName
        {
            get
            {
                return "sync_server_id";
            }
        }

        /// <summary>
        /// Gets the sync session id name.
        /// </summary>
        public static string SyncSessionIDName
        {
            get
            {
                return "sync_session_id";
            }
        }

        /// <summary>
        /// Gets the sync start date range.
        /// </summary>
        public static string SyncStartDateRange
        {
            get
            {
                return "sync_start_daterange";
            }
        }

        /// <summary>
        /// Gets the sync end date range.
        /// </summary>
        public static string SyncEndDateRange
        {
            get
            {
                return "sync_end_daterange";
            }
        }

        /// <summary>
        /// Gets the sync filter by date range.
        /// </summary>
        public static string SyncFilterByDateRange
        {
            get
            {
                return "sync_filter_by_daterange";
            }
        }

        /// <summary>
        /// Gets the sync supported columns.
        /// </summary>
        public static string SyncSupportedColumns
        {
            get
            {
                return "sync_supported_columns";
            }
        }

        /// <summary>
        /// Gets the sync table name.
        /// </summary>
        public static string SyncTableName
        {
            get
            {
                return "sync_table_name";
            }
        }

        /// <summary>
        /// Gets the sync table received anchor name.
        /// </summary>
        public static string SyncTableReceivedAnchorName
        {
            get
            {
                return "sync_table_received_anchor";
            }
        }

        /// <summary>
        /// Gets the sync table sent anchor name.
        /// </summary>
        public static string SyncTableSentAnchorName
        {
            get
            {
                return "sync_table_sent_anchor";
            }
        }

		/// <summary>
		/// Gets the first time synchronization option.
		/// 0 (default) - Always synchronize
		/// 1 (bypass) - During initial synchronization, skip this table
		/// 2 (last batch) - During initial synchronization, only send down the last batch
		/// </summary>
		public static string SyncFirstTimeSyncOption
		{
			get
			{
				return "sync_first_time_sync_option";
			}
		}

		#endregion

		// =====================================================================================================
		#region Public Methods and Operators

		/// <summary>
		/// Returns a formatted sqlcommand parameter string.  (ie: parameterName =&gt; @parameterName)
		/// </summary>
		/// <param name="pParameterName">
		/// Name of the parameter to format.
		/// </param>
		/// <returns>
		/// The name of the parameter, prefixed with an @ symbol.
		/// </returns>
		public static string GetFormattedParameterString(string pParameterName)
        {
            return string.Format("@{0}", pParameterName);
        }

        #endregion
    }
}