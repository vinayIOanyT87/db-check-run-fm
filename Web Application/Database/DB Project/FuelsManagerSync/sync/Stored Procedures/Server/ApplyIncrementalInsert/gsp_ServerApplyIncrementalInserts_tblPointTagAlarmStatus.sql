-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTagAlarmStatus
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblPointTagAlarmStatus]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@PointTagAlarmStatusGuid uniqueidentifier,
@AlarmTestGuid uniqueidentifier,
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
@sync_supported_columns_tblPointTagAlarmStatus varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblPointTagAlarmStatus] AS existingData
        USING (SELECT @PointTagAlarmStatusGuid 'PointTagAlarmStatusGuid',@AlarmTestGuid 'AlarmTestGuid',@Acknowledged 'Acknowledged',@AcknowledgedTimestamp 'AcknowledgedTimestamp',@AcknowledgedBy 'AcknowledgedBy',@AcknowledgedComment 'AcknowledgedComment',@Silenced 'Silenced',@SilencedTimestamp 'SilencedTimestamp',@SilencedBy 'SilencedBy',@AlarmTestFailed 'AlarmTestFailed',@AlarmTestFailedTimestamp 'AlarmTestFailedTimestamp',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy'
                ) AS remoteChanges ([PointTagAlarmStatusGuid],[AlarmTestGuid],[Acknowledged],[AcknowledgedTimestamp],[AcknowledgedBy],[AcknowledgedComment],[Silenced],[SilencedTimestamp],[SilencedBy],[AlarmTestFailed],[AlarmTestFailedTimestamp],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
        ON (existingData.[PointTagAlarmStatusGuid] = remoteChanges.[PointTagAlarmStatusGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [AlarmTestGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestGuid'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[AlarmTestGuid] ELSE remoteChanges.[AlarmTestGuid] END
                       ,[Acknowledged] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Acknowledged'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[Acknowledged] ELSE remoteChanges.[Acknowledged] END
                       ,[AcknowledgedTimestamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedTimestamp'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[AcknowledgedTimestamp] ELSE remoteChanges.[AcknowledgedTimestamp] END
                       ,[AcknowledgedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedBy'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[AcknowledgedBy] ELSE remoteChanges.[AcknowledgedBy] END
                       ,[AcknowledgedComment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedComment'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[AcknowledgedComment] ELSE remoteChanges.[AcknowledgedComment] END
                       ,[Silenced] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Silenced'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[Silenced] ELSE remoteChanges.[Silenced] END
                       ,[SilencedTimestamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SilencedTimestamp'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[SilencedTimestamp] ELSE remoteChanges.[SilencedTimestamp] END
                       ,[SilencedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SilencedBy'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[SilencedBy] ELSE remoteChanges.[SilencedBy] END
                       ,[AlarmTestFailed] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestFailed'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[AlarmTestFailed] ELSE remoteChanges.[AlarmTestFailed] END
                       ,[AlarmTestFailedTimestamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestFailedTimestamp'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[AlarmTestFailedTimestamp] ELSE remoteChanges.[AlarmTestFailedTimestamp] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

        WHEN NOT MATCHED THEN
            INSERT ([PointTagAlarmStatusGuid],[AlarmTestGuid],[Acknowledged],[AcknowledgedTimestamp],[AcknowledgedBy],[AcknowledgedComment],[Silenced],[SilencedTimestamp],[SilencedBy],[AlarmTestFailed],[AlarmTestFailedTimestamp],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                VALUES (@PointTagAlarmStatusGuid,@AlarmTestGuid,@Acknowledged,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedTimestamp'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN NULL ELSE @AcknowledgedTimestamp END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedBy'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN NULL ELSE @AcknowledgedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AcknowledgedComment'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN NULL ELSE @AcknowledgedComment END),@Silenced,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SilencedTimestamp'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN NULL ELSE @SilencedTimestamp END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SilencedBy'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN NULL ELSE @SilencedBy END),@AlarmTestFailed,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestFailedTimestamp'), @sync_supported_columns_tblPointTagAlarmStatus)) WHEN 0 THEN NULL ELSE @AlarmTestFailedTimestamp END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTagAlarmStatusGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTagAlarmStatusGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTagAlarmStatusGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPointTagAlarmStatus] WHERE PointTagAlarmStatusGuid = @PointTagAlarmStatusGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

