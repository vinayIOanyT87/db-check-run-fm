-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmTemplate
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblAlarmTemplate]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AlarmTemplateGuid uniqueidentifier,
@InputTemplateTagGuid uniqueidentifier,
@ID nvarchar(256),
@Enabled bit,
@AlarmCategoryApplicationStringGuid uniqueidentifier,
@Order int,
@NotAlarmState nvarchar(100),
@Comment nvarchar(256),
@ShelvedStartTimeStamp datetimeoffset(7),
@ShelvedEndTimeStamp datetimeoffset(7),
@ShelvedOneShot bit,
@ShelvedBy nvarchar(100),
@Suppressed bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@AlarmStateTemplateTagGuid uniqueidentifier,
@ExclusiveAlarm bit,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblAlarmTemplate varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAlarmTemplate] AS existingData
        USING (SELECT @AlarmTemplateGuid 'AlarmTemplateGuid',@InputTemplateTagGuid 'InputTemplateTagGuid',@ID 'ID',@Enabled 'Enabled',@AlarmCategoryApplicationStringGuid 'AlarmCategoryApplicationStringGuid',@Order 'Order',@NotAlarmState 'NotAlarmState',@Comment 'Comment',@ShelvedStartTimeStamp 'ShelvedStartTimeStamp',@ShelvedEndTimeStamp 'ShelvedEndTimeStamp',@ShelvedOneShot 'ShelvedOneShot',@ShelvedBy 'ShelvedBy',@Suppressed 'Suppressed',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@AlarmStateTemplateTagGuid 'AlarmStateTemplateTagGuid',@ExclusiveAlarm 'ExclusiveAlarm'
                ) AS remoteChanges ([AlarmTemplateGuid],[InputTemplateTagGuid],[ID],[Enabled],[AlarmCategoryApplicationStringGuid],[Order],[NotAlarmState],[Comment],[ShelvedStartTimeStamp],[ShelvedEndTimeStamp],[ShelvedOneShot],[ShelvedBy],[Suppressed],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmStateTemplateTagGuid],[ExclusiveAlarm])
        ON (existingData.[AlarmTemplateGuid] = remoteChanges.[AlarmTemplateGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [InputTemplateTagGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InputTemplateTagGuid'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[InputTemplateTagGuid] ELSE remoteChanges.[InputTemplateTagGuid] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[Enabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Enabled'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[Enabled] ELSE remoteChanges.[Enabled] END
                       ,[AlarmCategoryApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmCategoryApplicationStringGuid'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[AlarmCategoryApplicationStringGuid] ELSE remoteChanges.[AlarmCategoryApplicationStringGuid] END
                       ,[Order] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Order'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[Order] ELSE remoteChanges.[Order] END
                       ,[NotAlarmState] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NotAlarmState'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[NotAlarmState] ELSE remoteChanges.[NotAlarmState] END
                       ,[Comment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Comment'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[Comment] ELSE remoteChanges.[Comment] END
                       ,[ShelvedStartTimeStamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShelvedStartTimeStamp'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[ShelvedStartTimeStamp] ELSE remoteChanges.[ShelvedStartTimeStamp] END
                       ,[ShelvedEndTimeStamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShelvedEndTimeStamp'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[ShelvedEndTimeStamp] ELSE remoteChanges.[ShelvedEndTimeStamp] END
                       ,[ShelvedOneShot] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShelvedOneShot'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[ShelvedOneShot] ELSE remoteChanges.[ShelvedOneShot] END
                       ,[ShelvedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShelvedBy'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[ShelvedBy] ELSE remoteChanges.[ShelvedBy] END
                       ,[Suppressed] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Suppressed'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[Suppressed] ELSE remoteChanges.[Suppressed] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[AlarmStateTemplateTagGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmStateTemplateTagGuid'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[AlarmStateTemplateTagGuid] ELSE remoteChanges.[AlarmStateTemplateTagGuid] END
                       ,[ExclusiveAlarm] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExclusiveAlarm'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN existingData.[ExclusiveAlarm] ELSE remoteChanges.[ExclusiveAlarm] END

        WHEN NOT MATCHED THEN
            INSERT ([AlarmTemplateGuid],[InputTemplateTagGuid],[ID],[Enabled],[AlarmCategoryApplicationStringGuid],[Order],[NotAlarmState],[Comment],[ShelvedStartTimeStamp],[ShelvedEndTimeStamp],[ShelvedOneShot],[ShelvedBy],[Suppressed],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmStateTemplateTagGuid],[ExclusiveAlarm])
                VALUES (@AlarmTemplateGuid,@InputTemplateTagGuid,@ID,@Enabled,@AlarmCategoryApplicationStringGuid,@Order,@NotAlarmState,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Comment'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN NULL ELSE @Comment END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShelvedStartTimeStamp'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN NULL ELSE @ShelvedStartTimeStamp END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShelvedEndTimeStamp'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN NULL ELSE @ShelvedEndTimeStamp END),@ShelvedOneShot,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShelvedBy'), @sync_supported_columns_tblAlarmTemplate)) WHEN 0 THEN NULL ELSE @ShelvedBy END),@Suppressed,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@AlarmStateTemplateTagGuid,@ExclusiveAlarm)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmTemplateGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmTemplateGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmTemplateGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAlarmTemplate] WHERE AlarmTemplateGuid = @AlarmTemplateGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

