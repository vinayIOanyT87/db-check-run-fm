
IF  EXISTS (SELECT * FROM sys.all_objects WHERE type_desc = 'SQL_SCALAR_FUNCTION' AND object_id = OBJECT_ID(N'[archive].[udf_GetArchiveCutOffDate]')) 
	DROP FUNCTION [archive].[udf_GetArchiveCutOffDate]
GO

IF  EXISTS (SELECT * FROM sys.all_objects WHERE type_desc = 'SQL_SCALAR_FUNCTION' AND object_id = OBJECT_ID(N'[archive].[udf_GetAllScopesArchivingOnString]')) 
	DROP FUNCTION [archive].[udf_GetAllScopesArchivingOnString]
GO


IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetArchivedRecordKeyAtRowNumber]')) 
	DROP PROCEDURE [archive].[usp_GetArchivedRecordKeyAtRowNumber]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetAlarmAndEventLogKeyAtRowNumber]')) 
	DROP PROCEDURE [archive].[usp_GetAlarmAndEventLogKeyAtRowNumber]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetAuditLogKeyAtRowNumber]')) 
	DROP PROCEDURE [archive].[usp_GetAuditLogKeyAtRowNumber]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionKeyAtRowNumber]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionKeyAtRowNumber]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetAlarmAndEventLog]')) 
	DROP PROCEDURE [archive].[usp_GetAlarmAndEventLog]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetAuditLog]')) 
	DROP PROCEDURE [archive].[usp_GetAuditLog]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionWeightReadings]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionWeightReadings]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionTransportLineItems]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionTransportLineItems]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionUserData]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionUserData]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionSubLineItems]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionSubLineItems]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionSignature]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionSignature]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactions]')) 
	DROP PROCEDURE [archive].[usp_GetTransactions]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionPIDX]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionPIDX]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionNotes]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionNotes]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionLinks]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionLinks]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionLineItemUserData]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionLineItemUserData]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetTransactionLineItems]')) 
	DROP PROCEDURE [archive].[usp_GetTransactionLineItems]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetExportResults]')) 
	DROP PROCEDURE [archive].[usp_GetExportResults]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_GetExportResultDetails]')) 
	DROP PROCEDURE [archive].[usp_GetExportResultDetails]
GO



IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_LogArchivingSession]')) 
	DROP PROCEDURE [archive].[usp_LogArchivingSession]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_MarkArchivedBatchAsComplete]')) 
	DROP PROCEDURE [archive].[usp_MarkArchivedBatchAsComplete]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_PurgeProcessedAlarmAndEventLogRecords]')) 
	DROP PROCEDURE [archive].[usp_PurgeProcessedAlarmAndEventLogRecords]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_PurgeProcessedAuditLogRecords]')) 
	DROP PROCEDURE [archive].[usp_PurgeProcessedAuditLogRecords]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_PurgeProcessedTransactionRecords]')) 
	DROP PROCEDURE [archive].[usp_PurgeProcessedTransactionRecords]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_ResetProcessedRecordsTable]')) 
	DROP PROCEDURE [archive].[usp_ResetProcessedRecordsTable]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_DeleteArchivedAuditLogSyncTrackingRecords]')) 
	DROP PROCEDURE [archive].[usp_DeleteArchivedAuditLogSyncTrackingRecords]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_DeleteArchivedAlarmAndEventLogSyncTrackingRecords]')) 
	DROP PROCEDURE [archive].[usp_DeleteArchivedAlarmAndEventLogSyncTrackingRecords]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_DeleteArchivedTransationSyncTrackingRecords]')) 
	DROP PROCEDURE [archive].[usp_DeleteArchivedTransationSyncTrackingRecords]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_ReenableTriggersForOfflineArchiving]')) 
	DROP PROCEDURE [archive].[usp_ReenableTriggersForOfflineArchiving]
GO

IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[archive].[usp_DisableTriggersForOfflineArchiving]')) 
	DROP PROCEDURE [archive].[usp_DisableTriggersForOfflineArchiving]
GO


IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[archive].[tblAlarmAndEventLogLastProcessedRecords]')) 
	DROP TABLE [archive].[tblAlarmAndEventLogLastProcessedRecords]
GO

IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[archive].[tblAuditLogLastProcessedRecords]')) 
	DROP TABLE [archive].[tblAuditLogLastProcessedRecords]
GO

IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[archive].[tblTransactionLastProcessedRecords]')) 
	DROP TABLE [archive].[tblTransactionLastProcessedRecords]
GO

IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[archive].[tblArchiveScopeToTable]')) 
	DROP TABLE [archive].[tblArchiveScopeToTable]
GO

IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[archive].[tblArchiveScope]')) 
	DROP TABLE [archive].[tblArchiveScope]
GO

IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'archive') 
	DROP SCHEMA archive
GO