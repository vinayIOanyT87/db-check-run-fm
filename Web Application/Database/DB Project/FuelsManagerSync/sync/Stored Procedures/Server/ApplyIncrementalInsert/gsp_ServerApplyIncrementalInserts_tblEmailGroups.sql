-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEmailGroups
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblEmailGroups]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(80),
@AlwaysEnabled bit,
@StartTime datetimeoffset(7),
@EndTime datetimeoffset(7),
@CategoriesAndPriorities bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@EmailGroupGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblEmailGroups varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblEmailGroups] AS existingData
        USING (SELECT @ID 'ID',@AlwaysEnabled 'AlwaysEnabled',@StartTime 'StartTime',@EndTime 'EndTime',@CategoriesAndPriorities 'CategoriesAndPriorities',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@EmailGroupGuid 'EmailGroupGuid',@SiteGuid 'SiteGuid'
                ) AS remoteChanges ([ID],[AlwaysEnabled],[StartTime],[EndTime],[CategoriesAndPriorities],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EmailGroupGuid],[SiteGuid])
        ON (existingData.[EmailGroupGuid] = remoteChanges.[EmailGroupGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[AlwaysEnabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlwaysEnabled'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[AlwaysEnabled] ELSE remoteChanges.[AlwaysEnabled] END
                       ,[StartTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StartTime'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[StartTime] ELSE remoteChanges.[StartTime] END
                       ,[EndTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EndTime'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[EndTime] ELSE remoteChanges.[EndTime] END
                       ,[CategoriesAndPriorities] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CategoriesAndPriorities'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[CategoriesAndPriorities] ELSE remoteChanges.[CategoriesAndPriorities] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblEmailGroups)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([ID],[AlwaysEnabled],[StartTime],[EndTime],[CategoriesAndPriorities],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EmailGroupGuid],[SiteGuid])
                VALUES (@ID,@AlwaysEnabled,@StartTime,@EndTime,@CategoriesAndPriorities,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@EmailGroupGuid,@SiteGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EmailGroupGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EmailGroupGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EmailGroupGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblEmailGroups] WHERE EmailGroupGuid = @EmailGroupGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

