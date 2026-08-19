-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmAndEventLog
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblAlarmAndEventLog]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@Source nvarchar(120),
@Alarm bit,
@ID nvarchar(120),
@AssociatedData nvarchar(max),
@CategoryID nvarchar(50),
@PriorityID nvarchar(50),
@Acknowledged bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@AlarmAndEventLogGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@SourceNode nvarchar(256),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblAlarmAndEventLog varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAlarmAndEventLog] AS existingData
        USING (SELECT @Source 'Source',@Alarm 'Alarm',@ID 'ID',@AssociatedData 'AssociatedData',@CategoryID 'CategoryID',@PriorityID 'PriorityID',@Acknowledged 'Acknowledged',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@AlarmAndEventLogGuid 'AlarmAndEventLogGuid',@SiteGuid 'SiteGuid',@SourceNode 'SourceNode'
                ) AS remoteChanges ([Source],[Alarm],[ID],[AssociatedData],[CategoryID],[PriorityID],[Acknowledged],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmAndEventLogGuid],[SiteGuid],[SourceNode])
        ON (existingData.[AlarmAndEventLogGuid] = remoteChanges.[AlarmAndEventLogGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [Source] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Source'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[Source] ELSE remoteChanges.[Source] END
                       ,[Alarm] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Alarm'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[Alarm] ELSE remoteChanges.[Alarm] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[AssociatedData] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssociatedData'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[AssociatedData] ELSE remoteChanges.[AssociatedData] END
                       ,[CategoryID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CategoryID'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[CategoryID] ELSE remoteChanges.[CategoryID] END
                       ,[PriorityID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PriorityID'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[PriorityID] ELSE remoteChanges.[PriorityID] END
                       ,[Acknowledged] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Acknowledged'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[Acknowledged] ELSE remoteChanges.[Acknowledged] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[SourceNode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceNode'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN existingData.[SourceNode] ELSE remoteChanges.[SourceNode] END

        WHEN NOT MATCHED THEN
            INSERT ([Source],[Alarm],[ID],[AssociatedData],[CategoryID],[PriorityID],[Acknowledged],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmAndEventLogGuid],[SiteGuid],[SourceNode])
                VALUES (@Source,@Alarm,@ID,@AssociatedData,@CategoryID,@PriorityID,@Acknowledged,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@AlarmAndEventLogGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceNode'), @sync_supported_columns_tblAlarmAndEventLog)) WHEN 0 THEN NULL ELSE @SourceNode END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmAndEventLogGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmAndEventLogGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmAndEventLogGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAlarmAndEventLog] WHERE AlarmAndEventLogGuid = @AlarmAndEventLogGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

