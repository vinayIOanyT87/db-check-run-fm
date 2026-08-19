@echo off

REM change the SERVER to the fully specified instance
REM if using a name instance, use (local)\<InstanceName> or .\<InstanceName>
REM if using default instance, use (local) or . or <MachineName>
set SERVER=(local)
REM change the DATABASE name to the *main* FuelsManagerDB, not the Archive database
set DATABASE=FuelsManagerDB

sqlcmd -S %SERVER% -d %DATABASE% -i archive.DeleteLiveDBComponents.sql > ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.ArchiveSchema.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.tblArchiveScope.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.tblArchiveScopeToTable.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.tblArchiveScope.refdata.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.tblArchiveScopeToTable.refdata.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.tblConfigurationSettingArchiveSetting.refdata.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.tblAlarmAndEventLogLastProcessedRecords.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.tblAuditLogLastProcessedRecords.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.tblTransactionLastProcessedRecords.sql >> ArchiveScripts.log

sqlcmd -S %SERVER% -d %DATABASE% -i archive.udf_GetArchiveCutOffDate.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.udf_GetAllScopesArchivingOnString.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetAlarmAndEventLog.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetAlarmAndEventLogKeyAtRowNumber.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetArchivedRecordKeyAtRowNumber.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetAuditLog.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetAuditLogKeyAtRowNumber.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetExportResultDetails.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetExportResults.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionKeyAtRowNumber.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionLineItems.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionLineItemUserData.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionLinks.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionNotes.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionPIDX.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactions.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionSignature.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionSubLineItems.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionTransportLineItems.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionUserData.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_GetTransactionWeightReadings.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_MarkArchivedBatchAsComplete.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_LogArchivingSession.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_PurgeProcessedAlarmAndEventLogRecords.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_PurgeProcessedAuditLogRecords.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_PurgeProcessedTransactionRecords.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_ResetProcessedRecordsTable.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_DeleteArchivedAuditLogSyncTrackingRecords.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_DeleteArchivedAlarmAndEventLogSyncTrackingRecords.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_DeleteArchivedTransationSyncTrackingRecords.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_ReenableTriggersForOfflineArchiving.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i archive.usp_DisableTriggersForOfflineArchiving.sql >> ArchiveScripts.log

sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionLineItems.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionLineItemUserData.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionNotes.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionPIDX.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactions.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionSignature.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionSubLineItems.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionTransportLineItems.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionUserData.sql >> ArchiveScripts.log
sqlcmd -S %SERVER% -d %DATABASE% -i dbo.trg_Audit_del_tblTransactionWeightReadings.sql >> ArchiveScripts.log
