-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestDefinitions
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblTestDefinitions]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@TestName nvarchar(80),
@MeasurementUnit nvarchar(32),
@ValidationRule nvarchar(64),
@SampleSize float,
@TestCode nvarchar(5),
@TestMethod nvarchar(80),
@ProductID nvarchar(30),
@DeleteFlag bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@TestDefinitionGuid uniqueidentifier,
@OwnerSiteGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTestDefinitions] AS existingData
        USING (SELECT @TestName 'TestName',@MeasurementUnit 'MeasurementUnit',@ValidationRule 'ValidationRule',@SampleSize 'SampleSize',@TestCode 'TestCode',@TestMethod 'TestMethod',@ProductID 'ProductID',@DeleteFlag 'DeleteFlag',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@TestDefinitionGuid 'TestDefinitionGuid',@OwnerSiteGuid 'OwnerSiteGuid'
                ) AS remoteChanges ([TestName],[MeasurementUnit],[ValidationRule],[SampleSize],[TestCode],[TestMethod],[ProductID],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TestDefinitionGuid],[OwnerSiteGuid])
        ON (existingData.[TestDefinitionGuid] = remoteChanges.[TestDefinitionGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [TestName] = remoteChanges.[TestName]
                       ,[MeasurementUnit] = remoteChanges.[MeasurementUnit]
                       ,[ValidationRule] = remoteChanges.[ValidationRule]
                       ,[SampleSize] = remoteChanges.[SampleSize]
                       ,[TestCode] = remoteChanges.[TestCode]
                       ,[TestMethod] = remoteChanges.[TestMethod]
                       ,[ProductID] = remoteChanges.[ProductID]
                       ,[DeleteFlag] = remoteChanges.[DeleteFlag]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[OwnerSiteGuid] = remoteChanges.[OwnerSiteGuid]

        WHEN NOT MATCHED THEN
            INSERT ([TestName],[MeasurementUnit],[ValidationRule],[SampleSize],[TestCode],[TestMethod],[ProductID],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TestDefinitionGuid],[OwnerSiteGuid])
                VALUES (@TestName,@MeasurementUnit,@ValidationRule,@SampleSize,@TestCode,@TestMethod,@ProductID,@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TestDefinitionGuid,@OwnerSiteGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestDefinitionGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestDefinitionGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestDefinitionGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTestDefinitions] WHERE TestDefinitionGuid = @TestDefinitionGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
