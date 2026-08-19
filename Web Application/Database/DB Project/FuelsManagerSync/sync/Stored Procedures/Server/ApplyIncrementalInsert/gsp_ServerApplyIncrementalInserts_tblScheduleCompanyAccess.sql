-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblScheduleCompanyAccess
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblScheduleCompanyAccess]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ScheduleCompanyAccessGuid uniqueidentifier,
@CompanyGuid uniqueidentifier,
@LookupDayOfWeekIndex int,
@Enabled bit,
@OpeningTime datetimeoffset(7),
@ClosingTime datetimeoffset(7),
@EndOfDayEnabled bit,
@EndOfDayTime datetimeoffset(7),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblScheduleCompanyAccess varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblScheduleCompanyAccess] AS existingData
        USING (SELECT @ScheduleCompanyAccessGuid 'ScheduleCompanyAccessGuid',@CompanyGuid 'CompanyGuid',@LookupDayOfWeekIndex 'LookupDayOfWeekIndex',@Enabled 'Enabled',@OpeningTime 'OpeningTime',@ClosingTime 'ClosingTime',@EndOfDayEnabled 'EndOfDayEnabled',@EndOfDayTime 'EndOfDayTime',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy'
                ) AS remoteChanges ([ScheduleCompanyAccessGuid],[CompanyGuid],[LookupDayOfWeekIndex],[Enabled],[OpeningTime],[ClosingTime],[EndOfDayEnabled],[EndOfDayTime],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
        ON (existingData.[ScheduleCompanyAccessGuid] = remoteChanges.[ScheduleCompanyAccessGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [CompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[CompanyGuid] ELSE remoteChanges.[CompanyGuid] END
                       ,[LookupDayOfWeekIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupDayOfWeekIndex'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[LookupDayOfWeekIndex] ELSE remoteChanges.[LookupDayOfWeekIndex] END
                       ,[Enabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Enabled'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[Enabled] ELSE remoteChanges.[Enabled] END
                       ,[OpeningTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OpeningTime'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[OpeningTime] ELSE remoteChanges.[OpeningTime] END
                       ,[ClosingTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ClosingTime'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[ClosingTime] ELSE remoteChanges.[ClosingTime] END
                       ,[EndOfDayEnabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EndOfDayEnabled'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[EndOfDayEnabled] ELSE remoteChanges.[EndOfDayEnabled] END
                       ,[EndOfDayTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EndOfDayTime'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[EndOfDayTime] ELSE remoteChanges.[EndOfDayTime] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

        WHEN NOT MATCHED THEN
            INSERT ([ScheduleCompanyAccessGuid],[CompanyGuid],[LookupDayOfWeekIndex],[Enabled],[OpeningTime],[ClosingTime],[EndOfDayEnabled],[EndOfDayTime],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                VALUES (@ScheduleCompanyAccessGuid,@CompanyGuid,@LookupDayOfWeekIndex,@Enabled,@OpeningTime,@ClosingTime,@EndOfDayEnabled,@EndOfDayTime,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblScheduleCompanyAccess)) WHEN 0 THEN NULL ELSE @UpdatedBy END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ScheduleCompanyAccessGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ScheduleCompanyAccessGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ScheduleCompanyAccessGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblScheduleCompanyAccess] WHERE ScheduleCompanyAccessGuid = @ScheduleCompanyAccessGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

