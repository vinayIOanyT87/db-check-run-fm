-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAuditLog
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblAuditLog]
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblAuditLog varchar(8000)
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
                            OR (existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [SessionID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SessionID'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[SessionID] ELSE remoteChanges.[SessionID] END
                       ,[ActionID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ActionID'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[ActionID] ELSE remoteChanges.[ActionID] END
                       ,[TypeID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TypeID'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[TypeID] ELSE remoteChanges.[TypeID] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[PropertyID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PropertyID'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[PropertyID] ELSE remoteChanges.[PropertyID] END
                       ,[NewValue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NewValue'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[NewValue] ELSE remoteChanges.[NewValue] END
                       ,[OldValue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OldValue'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[OldValue] ELSE remoteChanges.[OldValue] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[ParentTypeID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParentTypeID'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[ParentTypeID] ELSE remoteChanges.[ParentTypeID] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[AuditedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AuditedDate'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[AuditedDate] ELSE remoteChanges.[AuditedDate] END
                       ,[SourceNode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceNode'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[SourceNode] ELSE remoteChanges.[SourceNode] END
                       ,[AuditContext] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AuditContext'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN existingData.[AuditContext] ELSE remoteChanges.[AuditContext] END

            WHEN NOT MATCHED THEN
                INSERT ([SessionID],[ActionID],[TypeID],[ID],[PropertyID],[NewValue],[OldValue],[CreatedDate],[CreatedBy],[ParentTypeID],[AuditLogGuid],[SiteGuid],[AuditedDate],[SourceNode],[AuditContext])
                    VALUES ((CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SessionID'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN NULL ELSE @SessionID END),@ActionID,@TypeID,@ID,@PropertyID,@NewValue,@OldValue,@CreatedDate,@CreatedBy,@ParentTypeID,@AuditLogGuid,@SiteGuid,@AuditedDate,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceNode'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN NULL ELSE @SourceNode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AuditContext'), @sync_supported_columns_tblAuditLog)) WHEN 0 THEN NULL ELSE @AuditContext END))
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
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
