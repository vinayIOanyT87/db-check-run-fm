-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestTankResults
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblTestTankResults]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@TestName nvarchar(80),
@Measurement nvarchar(50),
@TestDate datetimeoffset(7),
@DeleteFlag bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@PerformedBy nvarchar(100),
@Supervisor nvarchar(100),
@Flag01 bit,
@Flag02 bit,
@TestCode nvarchar(5),
@TestTankResultGuid uniqueidentifier,
@LookupTestSetStatusIndex int,
@TestSetTankResultGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTestTankResults varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTestTankResults] AS existingData
        USING (SELECT @TestName 'TestName',@Measurement 'Measurement',@TestDate 'TestDate',@DeleteFlag 'DeleteFlag',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PerformedBy 'PerformedBy',@Supervisor 'Supervisor',@Flag01 'Flag01',@Flag02 'Flag02',@TestCode 'TestCode',@TestTankResultGuid 'TestTankResultGuid',@LookupTestSetStatusIndex 'LookupTestSetStatusIndex',@TestSetTankResultGuid 'TestSetTankResultGuid'
                ) AS remoteChanges ([TestName],[Measurement],[TestDate],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PerformedBy],[Supervisor],[Flag01],[Flag02],[TestCode],[TestTankResultGuid],[LookupTestSetStatusIndex],[TestSetTankResultGuid])
        ON (existingData.[TestTankResultGuid] = remoteChanges.[TestTankResultGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [TestName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestName'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[TestName] ELSE remoteChanges.[TestName] END
                       ,[Measurement] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Measurement'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[Measurement] ELSE remoteChanges.[Measurement] END
                       ,[TestDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestDate'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[TestDate] ELSE remoteChanges.[TestDate] END
                       ,[DeleteFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[DeleteFlag] ELSE remoteChanges.[DeleteFlag] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[PerformedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PerformedBy'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[PerformedBy] ELSE remoteChanges.[PerformedBy] END
                       ,[Supervisor] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Supervisor'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[Supervisor] ELSE remoteChanges.[Supervisor] END
                       ,[Flag01] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag01'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[Flag01] ELSE remoteChanges.[Flag01] END
                       ,[Flag02] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag02'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[Flag02] ELSE remoteChanges.[Flag02] END
                       ,[TestCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestCode'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[TestCode] ELSE remoteChanges.[TestCode] END
                       ,[LookupTestSetStatusIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupTestSetStatusIndex'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[LookupTestSetStatusIndex] ELSE remoteChanges.[LookupTestSetStatusIndex] END
                       ,[TestSetTankResultGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestSetTankResultGuid'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN existingData.[TestSetTankResultGuid] ELSE remoteChanges.[TestSetTankResultGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([TestName],[Measurement],[TestDate],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PerformedBy],[Supervisor],[Flag01],[Flag02],[TestCode],[TestTankResultGuid],[LookupTestSetStatusIndex],[TestSetTankResultGuid])
                VALUES (@TestName,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Measurement'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN NULL ELSE @Measurement END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestDate'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN NULL ELSE @TestDate END),@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PerformedBy'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN NULL ELSE @PerformedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Supervisor'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN NULL ELSE @Supervisor END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag01'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN NULL ELSE @Flag01 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag02'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN NULL ELSE @Flag02 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestCode'), @sync_supported_columns_tblTestTankResults)) WHEN 0 THEN NULL ELSE @TestCode END),@TestTankResultGuid,@LookupTestSetStatusIndex,@TestSetTankResultGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestTankResultGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestTankResultGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestTankResultGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTestTankResults] WHERE TestTankResultGuid = @TestTankResultGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

