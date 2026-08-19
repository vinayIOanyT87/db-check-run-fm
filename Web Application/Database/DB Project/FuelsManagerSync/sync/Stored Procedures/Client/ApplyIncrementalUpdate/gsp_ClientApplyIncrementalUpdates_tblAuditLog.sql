-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAuditLog
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblAuditLog]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@SessionID nvarchar(50),
@ActionID nvarchar(20),
@TypeID nvarchar(50),
@ID nvarchar(256),
@PropertyID nvarchar(50),
@NewValue nvarchar(max),
@OldValue nvarchar(max),
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
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblAuditLog] CT
                        WHERE CT.PK_AuditLogGuid = @AuditLogGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblAuditLog].[SessionID],[dbo].[tblAuditLog].[ActionID],[dbo].[tblAuditLog].[TypeID],[dbo].[tblAuditLog].[ID],[dbo].[tblAuditLog].[PropertyID],[dbo].[tblAuditLog].[NewValue],[dbo].[tblAuditLog].[OldValue],[dbo].[tblAuditLog].[CreatedDate],[dbo].[tblAuditLog].[CreatedBy],[dbo].[tblAuditLog].[ParentTypeID],[dbo].[tblAuditLog].[AuditLogGuid],[dbo].[tblAuditLog].[SiteGuid],[dbo].[tblAuditLog].[AuditedDate],[dbo].[tblAuditLog].[SourceNode],[dbo].[tblAuditLog].[AuditContext]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblAuditLog]
                        INNER JOIN [track].[tblAuditLog] CT
                            ON CT.PK_AuditLogGuid = [dbo].[tblAuditLog].[AuditLogGuid] 
                    WHERE CT.PK_AuditLogGuid = @AuditLogGuid
            ) MERGE existingData
            USING (SELECT @SessionID,@ActionID,@TypeID,@ID,@PropertyID,@NewValue,@OldValue,@CreatedDate,@CreatedBy,@ParentTypeID,@AuditLogGuid,@SiteGuid,@AuditedDate,@SourceNode,@AuditContext
                    ) AS remoteChanges ([SessionID],[ActionID],[TypeID],[ID],[PropertyID],[NewValue],[OldValue],[CreatedDate],[CreatedBy],[ParentTypeID],[AuditLogGuid],[SiteGuid],[AuditedDate],[SourceNode],[AuditContext])
            ON (existingData.[AuditLogGuid] = remoteChanges.[AuditLogGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END
    
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
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
