-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestDefinitions
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblTestDefinitions]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTestDefinitions varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblTestDefinitions] CT
                        WHERE CT.PK_TestDefinitionGuid = @TestDefinitionGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblTestDefinitions].[TestName],[dbo].[tblTestDefinitions].[MeasurementUnit],[dbo].[tblTestDefinitions].[ValidationRule],[dbo].[tblTestDefinitions].[SampleSize],[dbo].[tblTestDefinitions].[TestCode],[dbo].[tblTestDefinitions].[TestMethod],[dbo].[tblTestDefinitions].[ProductID],[dbo].[tblTestDefinitions].[DeleteFlag],[dbo].[tblTestDefinitions].[CreatedDate],[dbo].[tblTestDefinitions].[CreatedBy],[dbo].[tblTestDefinitions].[UpdatedDate],[dbo].[tblTestDefinitions].[UpdatedBy],[dbo].[tblTestDefinitions].[TestDefinitionGuid],[dbo].[tblTestDefinitions].[OwnerSiteGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblTestDefinitions]
                        INNER JOIN [track].[tblTestDefinitions] CT
                            ON CT.PK_TestDefinitionGuid = [dbo].[tblTestDefinitions].[TestDefinitionGuid] 
                    WHERE CT.PK_TestDefinitionGuid = @TestDefinitionGuid
            ) MERGE existingData
            USING (SELECT @TestName,@MeasurementUnit,@ValidationRule,@SampleSize,@TestCode,@TestMethod,@ProductID,@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TestDefinitionGuid,@OwnerSiteGuid
                    ) AS remoteChanges ([TestName],[MeasurementUnit],[ValidationRule],[SampleSize],[TestCode],[TestMethod],[ProductID],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TestDefinitionGuid],[OwnerSiteGuid])
            ON (existingData.[TestDefinitionGuid] = remoteChanges.[TestDefinitionGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [TestName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestName'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[TestName] ELSE remoteChanges.[TestName] END
                       ,[MeasurementUnit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeasurementUnit'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[MeasurementUnit] ELSE remoteChanges.[MeasurementUnit] END
                       ,[ValidationRule] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ValidationRule'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[ValidationRule] ELSE remoteChanges.[ValidationRule] END
                       ,[SampleSize] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SampleSize'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[SampleSize] ELSE remoteChanges.[SampleSize] END
                       ,[TestCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestCode'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[TestCode] ELSE remoteChanges.[TestCode] END
                       ,[TestMethod] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestMethod'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[TestMethod] ELSE remoteChanges.[TestMethod] END
                       ,[ProductID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductID'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[ProductID] ELSE remoteChanges.[ProductID] END
                       ,[DeleteFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[DeleteFlag] ELSE remoteChanges.[DeleteFlag] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[OwnerSiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OwnerSiteGuid'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN existingData.[OwnerSiteGuid] ELSE remoteChanges.[OwnerSiteGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([TestName],[MeasurementUnit],[ValidationRule],[SampleSize],[TestCode],[TestMethod],[ProductID],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TestDefinitionGuid],[OwnerSiteGuid])
                    VALUES (@TestName,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeasurementUnit'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN NULL ELSE @MeasurementUnit END),@ValidationRule,@SampleSize,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestCode'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN NULL ELSE @TestCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestMethod'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN NULL ELSE @TestMethod END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductID'), @sync_supported_columns_tblTestDefinitions)) WHEN 0 THEN NULL ELSE @ProductID END),@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TestDefinitionGuid,@OwnerSiteGuid)
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
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
