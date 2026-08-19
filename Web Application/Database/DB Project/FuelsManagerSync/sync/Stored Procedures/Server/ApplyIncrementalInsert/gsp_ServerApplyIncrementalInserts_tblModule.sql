-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblModule
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblModule]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(30),
@Description nvarchar(50),
@Standard bit,
@ModuleCalculation nvarchar(250),
@ModuleTypeName nvarchar(250),
@ModuleData xml,
@ModuleScript nvarchar(max),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@ModuleGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblModule varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblModule] AS existingData
        USING (SELECT @ID 'ID',@Description 'Description',@Standard 'Standard',@ModuleCalculation 'ModuleCalculation',@ModuleTypeName 'ModuleTypeName',@ModuleData 'ModuleData',@ModuleScript 'ModuleScript',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@ModuleGuid 'ModuleGuid',@SiteGuid 'SiteGuid'
                ) AS remoteChanges ([ID],[Description],[Standard],[ModuleCalculation],[ModuleTypeName],[ModuleData],[ModuleScript],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ModuleGuid],[SiteGuid])
        ON (existingData.[ModuleGuid] = remoteChanges.[ModuleGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[Description] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[Description] ELSE remoteChanges.[Description] END
                       ,[Standard] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Standard'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[Standard] ELSE remoteChanges.[Standard] END
                       ,[ModuleCalculation] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ModuleCalculation'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[ModuleCalculation] ELSE remoteChanges.[ModuleCalculation] END
                       ,[ModuleTypeName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ModuleTypeName'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[ModuleTypeName] ELSE remoteChanges.[ModuleTypeName] END
                       ,[ModuleData] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ModuleData'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[ModuleData] ELSE remoteChanges.[ModuleData] END
                       ,[ModuleScript] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ModuleScript'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[ModuleScript] ELSE remoteChanges.[ModuleScript] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblModule)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([ID],[Description],[Standard],[ModuleCalculation],[ModuleTypeName],[ModuleData],[ModuleScript],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ModuleGuid],[SiteGuid])
                VALUES (@ID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblModule)) WHEN 0 THEN NULL ELSE @Description END),@Standard,@ModuleCalculation,@ModuleTypeName,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ModuleData'), @sync_supported_columns_tblModule)) WHEN 0 THEN NULL ELSE @ModuleData END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ModuleScript'), @sync_supported_columns_tblModule)) WHEN 0 THEN NULL ELSE @ModuleScript END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblModule)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblModule)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblModule)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblModule)) WHEN 0 THEN NULL ELSE @UpdatedBy END),@ModuleGuid,@SiteGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ModuleGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ModuleGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ModuleGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblModule] WHERE ModuleGuid = @ModuleGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

