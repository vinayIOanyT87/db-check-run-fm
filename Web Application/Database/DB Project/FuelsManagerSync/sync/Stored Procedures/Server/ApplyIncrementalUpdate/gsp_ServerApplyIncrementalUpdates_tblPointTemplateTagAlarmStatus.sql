-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplateTagAlarmStatus
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblPointTemplateTagAlarmStatus]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@PointTemplateTagAlarmStatusGuid uniqueidentifier,
@AlarmTestTemplateGuid uniqueidentifier,
@Acknowledged bit,
@AcknowledgedTimestamp datetimeoffset(7),
@AcknowledgedBy nvarchar(100),
@AcknowledgedComment nvarchar(max),
@Silenced bit,
@SilencedTimestamp datetimeoffset(7),
@SilencedBy nvarchar(100),
@AlarmTestFailed bit,
@AlarmTestFailedTimestamp datetimeoffset(7),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblPointTemplateTagAlarmStatus varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblPointTemplateTagAlarmStatus] CT
                        WHERE CT.PK_PointTemplateTagAlarmStatusGuid = @PointTemplateTagAlarmStatusGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblPointTemplateTagAlarmStatus].[PointTemplateTagAlarmStatusGuid],[dbo].[tblPointTemplateTagAlarmStatus].[AlarmTestTemplateGuid],[dbo].[tblPointTemplateTagAlarmStatus].[Acknowledged],[dbo].[tblPointTemplateTagAlarmStatus].[AcknowledgedTimestamp],[dbo].[tblPointTemplateTagAlarmStatus].[AcknowledgedBy],[dbo].[tblPointTemplateTagAlarmStatus].[AcknowledgedComment],[dbo].[tblPointTemplateTagAlarmStatus].[Silenced],[dbo].[tblPointTemplateTagAlarmStatus].[SilencedTimestamp],[dbo].[tblPointTemplateTagAlarmStatus].[SilencedBy],[dbo].[tblPointTemplateTagAlarmStatus].[AlarmTestFailed],[dbo].[tblPointTemplateTagAlarmStatus].[AlarmTestFailedTimestamp],[dbo].[tblPointTemplateTagAlarmStatus].[CreatedDate],[dbo].[tblPointTemplateTagAlarmStatus].[CreatedBy],[dbo].[tblPointTemplateTagAlarmStatus].[UpdatedDate],[dbo].[tblPointTemplateTagAlarmStatus].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblPointTemplateTagAlarmStatus]
                        INNER JOIN [track].[tblPointTemplateTagAlarmStatus] CT
                            ON CT.PK_PointTemplateTagAlarmStatusGuid = [dbo].[tblPointTemplateTagAlarmStatus].[PointTemplateTagAlarmStatusGuid] 
                    WHERE CT.PK_PointTemplateTagAlarmStatusGuid = @PointTemplateTagAlarmStatusGuid
            ) MERGE existingData
            USING (SELECT @PointTemplateTagAlarmStatusGuid,@AlarmTestTemplateGuid,@Acknowledged,@AcknowledgedTimestamp,@AcknowledgedBy,@AcknowledgedComment,@Silenced,@SilencedTimestamp,@SilencedBy,@AlarmTestFailed,@AlarmTestFailedTimestamp,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([PointTemplateTagAlarmStatusGuid],[AlarmTestTemplateGuid],[Acknowledged],[AcknowledgedTimestamp],[AcknowledgedBy],[AcknowledgedComment],[Silenced],[SilencedTimestamp],[SilencedBy],[AlarmTestFailed],[AlarmTestFailedTimestamp],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[PointTemplateTagAlarmStatusGuid] = remoteChanges.[PointTemplateTagAlarmStatusGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [AlarmTestTemplateGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestTemplateGuid'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[AlarmTestTemplateGuid] ELSE remoteChanges.[AlarmTestTemplateGuid] END
                       ,[Acknowledged] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Acknowledged'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[Acknowledged] ELSE remoteChanges.[Acknowledged] END
                       ,[AcknowledgedTimestamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedTimestamp'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[AcknowledgedTimestamp] ELSE remoteChanges.[AcknowledgedTimestamp] END
                       ,[AcknowledgedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedBy'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[AcknowledgedBy] ELSE remoteChanges.[AcknowledgedBy] END
                       ,[AcknowledgedComment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedComment'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[AcknowledgedComment] ELSE remoteChanges.[AcknowledgedComment] END
                       ,[Silenced] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Silenced'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[Silenced] ELSE remoteChanges.[Silenced] END
                       ,[SilencedTimestamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SilencedTimestamp'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[SilencedTimestamp] ELSE remoteChanges.[SilencedTimestamp] END
                       ,[SilencedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SilencedBy'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[SilencedBy] ELSE remoteChanges.[SilencedBy] END
                       ,[AlarmTestFailed] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestFailed'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[AlarmTestFailed] ELSE remoteChanges.[AlarmTestFailed] END
                       ,[AlarmTestFailedTimestamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestFailedTimestamp'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[AlarmTestFailedTimestamp] ELSE remoteChanges.[AlarmTestFailedTimestamp] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

            WHEN NOT MATCHED THEN
                INSERT ([PointTemplateTagAlarmStatusGuid],[AlarmTestTemplateGuid],[Acknowledged],[AcknowledgedTimestamp],[AcknowledgedBy],[AcknowledgedComment],[Silenced],[SilencedTimestamp],[SilencedBy],[AlarmTestFailed],[AlarmTestFailedTimestamp],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@PointTemplateTagAlarmStatusGuid,@AlarmTestTemplateGuid,@Acknowledged,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedTimestamp'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN NULL ELSE @AcknowledgedTimestamp END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedBy'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN NULL ELSE @AcknowledgedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedComment'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN NULL ELSE @AcknowledgedComment END),@Silenced,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SilencedTimestamp'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN NULL ELSE @SilencedTimestamp END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SilencedBy'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN NULL ELSE @SilencedBy END),@AlarmTestFailed,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestFailedTimestamp'), @sync_supported_columns_tblPointTemplateTagAlarmStatus)) WHEN 0 THEN NULL ELSE @AlarmTestFailedTimestamp END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagAlarmStatusGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagAlarmStatusGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagAlarmStatusGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPointTemplateTagAlarmStatus] WHERE PointTemplateTagAlarmStatusGuid = @PointTemplateTagAlarmStatusGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
