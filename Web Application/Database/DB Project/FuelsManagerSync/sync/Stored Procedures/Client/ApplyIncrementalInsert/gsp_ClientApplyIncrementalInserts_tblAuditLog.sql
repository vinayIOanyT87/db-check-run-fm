-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAuditLog
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblAuditLog]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@SessionID nvarchar(50),
@ActionID nvarchar(20),
@TypeID nvarchar(50),
@ID nvarchar(256),
@PropertyID nvarchar(50),
@NewValue nvarchar(2000),
@OldValue nvarchar(2000),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@ParentTypeID nvarchar(50),
@AuditLogGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@AuditedDate datetimeoffset(7),
@SourceNode nvarchar(256),
@AuditContext varbinary(128),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAuditLog] AS existingData
        USING (SELECT @SessionID 'SessionID',@ActionID 'ActionID',@TypeID 'TypeID',@ID 'ID',@PropertyID 'PropertyID',@NewValue 'NewValue',@OldValue 'OldValue',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@ParentTypeID 'ParentTypeID',@AuditLogGuid 'AuditLogGuid',@SiteGuid 'SiteGuid',@AuditedDate 'AuditedDate',@SourceNode 'SourceNode',@AuditContext 'AuditContext'
                ) AS remoteChanges ([SessionID],[ActionID],[TypeID],[ID],[PropertyID],[NewValue],[OldValue],[CreatedDate],[CreatedBy],[ParentTypeID],[AuditLogGuid],[SiteGuid],[AuditedDate],[SourceNode],[AuditContext])
        ON (existingData.[AuditLogGuid] = remoteChanges.[AuditLogGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate) THEN
            UPDATE SET [SessionID] = remoteChanges.[SessionID]
                       ,[ActionID] = remoteChanges.[ActionID]
                       ,[TypeID] = remoteChanges.[TypeID]
                       ,[ID] = remoteChanges.[ID]
                       ,[PropertyID] = remoteChanges.[PropertyID]
                       ,[NewValue] = remoteChanges.[NewValue]
                       ,[OldValue] = remoteChanges.[OldValue]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[ParentTypeID] = remoteChanges.[ParentTypeID]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[AuditedDate] = remoteChanges.[AuditedDate]
                       ,[SourceNode] = remoteChanges.[SourceNode]
                       ,[AuditContext] = remoteChanges.[AuditContext]

        WHEN NOT MATCHED THEN
            INSERT ([SessionID],[ActionID],[TypeID],[ID],[PropertyID],[NewValue],[OldValue],[CreatedDate],[CreatedBy],[ParentTypeID],[AuditLogGuid],[SiteGuid],[AuditedDate],[SourceNode],[AuditContext])
                VALUES (@SessionID,@ActionID,@TypeID,@ID,@PropertyID,@NewValue,@OldValue,@CreatedDate,@CreatedBy,@ParentTypeID,@AuditLogGuid,@SiteGuid,@AuditedDate,@SourceNode,@AuditContext)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AuditLogGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AuditLogGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AuditLogGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAuditLog] WHERE (CreatedDate >= @CreatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
