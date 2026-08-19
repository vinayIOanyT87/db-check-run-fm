CREATE PROCEDURE [sync].[usp_SyncPurgeLogs]
	@RemoteNodeGuid UniqueIdentifier,
	@MaximumDaysToRetainLogs int
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SessionLogsToDelete TABLE(
	SyncSessionLogGuid UniqueIdentifier)

	INSERT INTO @SessionLogsToDelete
	SELECT ssl.SyncSessionLogGuid FROM [sync].[tblSyncSessionLog] ssl
	WHERE ssl.RemoteNodeGuid = @RemoteNodeGuid
	AND ssl.UpdatedDate < DATEADD(day,-@MaximumDaysToRetainLogs,SYSDATETIMEOFFSET())

	DECLARE @SyncRecordConflictToSyncSessionScopeLogToDelete TABLE (
	SyncRecordConflictToSyncSessionScopeLogGuid UniqueIdentifier)

	INSERT INTO @SyncRecordConflictToSyncSessionScopeLogToDelete
	SELECT SyncRecordConflictToSyncSessionScopeLogGuid FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog]
	WHERE SyncSessionScopeLogGuid IN
	(SELECT SyncSessionScopeLogGuid FROM [sync].[tblSyncSessionScopeLog]
	WHERE SyncSessionLogGuid IN (SELECT SyncSessionLogGuid FROM @SessionLogsToDelete))

	DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] 
	WHERE SyncRecordConflictToSyncSessionScopeLogGuid IN
	(SELECT SyncRecordConflictToSyncSessionScopeLogGuid FROM @SyncRecordConflictToSyncSessionScopeLogToDelete)

	DELETE FROM [sync].[tblSyncRecordConflict] WHERE
	TargetNodeGuid = @RemoteNodeGuid
	AND SyncConflictResolutionStatusIndex <> 0
	AND SyncConflictResolutionStatusIndex <> 3
	AND UpdatedDate < DATEADD(day,-@MaximumDaysToRetainLogs,SYSDATETIMEOFFSET())
	AND SyncRecordConflictGuid NOT IN
	(SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog])

	DELETE FROM [sync].[tblSyncSessionScopeLog] 
	WHERE SyncSessionLogGuid IN
	(SELECT SyncSessionLogGuid FROM @SessionLogsToDelete)

	DELETE FROM [sync].[tblSyncSessionLog] 
	WHERE SyncSessionLogGuid IN
	(SELECT SyncSessionLogGuid FROM @SessionLogsToDelete)
END