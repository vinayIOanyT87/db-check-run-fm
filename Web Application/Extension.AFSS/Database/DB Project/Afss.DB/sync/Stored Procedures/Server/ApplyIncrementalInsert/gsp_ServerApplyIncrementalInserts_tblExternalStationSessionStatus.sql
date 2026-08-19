-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationSessionStatus
-- Description:	Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblExternalStationSessionStatus]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ExternalStationSessionStatusIndex int,
@ExternalStationSessionStatusCode nvarchar(100),
@ExternalStationSessionStatusName nvarchar(100),
@ExternalStationSessionStatusGuid uniqueidentifier,
@LongDescription nvarchar(1024),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblExternalStationSessionStatus varchar(8000)
AS
BEGIN
    ;   MERGE [lookup].[tblExternalStationSessionStatus] AS existingData
        USING (SELECT @ExternalStationSessionStatusIndex 'ExternalStationSessionStatusIndex',@ExternalStationSessionStatusCode 'ExternalStationSessionStatusCode',@ExternalStationSessionStatusName 'ExternalStationSessionStatusName',@ExternalStationSessionStatusGuid 'ExternalStationSessionStatusGuid',@LongDescription 'LongDescription',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate'
                ) AS remoteChanges ([ExternalStationSessionStatusIndex],[ExternalStationSessionStatusCode],[ExternalStationSessionStatusName],[ExternalStationSessionStatusGuid],[LongDescription],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
        ON (existingData.[ExternalStationSessionStatusIndex] = remoteChanges.[ExternalStationSessionStatusIndex])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ExternalStationSessionStatusCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExternalStationSessionStatusCode'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN existingData.[ExternalStationSessionStatusCode] ELSE remoteChanges.[ExternalStationSessionStatusCode] END
                       ,[ExternalStationSessionStatusName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExternalStationSessionStatusName'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN existingData.[ExternalStationSessionStatusName] ELSE remoteChanges.[ExternalStationSessionStatusName] END
                       ,[ExternalStationSessionStatusGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExternalStationSessionStatusGuid'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN existingData.[ExternalStationSessionStatusGuid] ELSE remoteChanges.[ExternalStationSessionStatusGuid] END
                       ,[LongDescription] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LongDescription'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN existingData.[LongDescription] ELSE remoteChanges.[LongDescription] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END

        WHEN NOT MATCHED THEN
            INSERT ([ExternalStationSessionStatusIndex],[ExternalStationSessionStatusCode],[ExternalStationSessionStatusName],[ExternalStationSessionStatusGuid],[LongDescription],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                VALUES (@ExternalStationSessionStatusIndex,@ExternalStationSessionStatusCode,@ExternalStationSessionStatusName,@ExternalStationSessionStatusGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LongDescription'), @sync_supported_columns_tblExternalStationSessionStatus)) WHEN 0 THEN NULL ELSE @LongDescription END),@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExternalStationSessionStatusIndex) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExternalStationSessionStatusIndex))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExternalStationSessionStatusIndex)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [lookup].[tblExternalStationSessionStatus] WHERE ExternalStationSessionStatusIndex = @ExternalStationSessionStatusIndex AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    DECLARE @minValidVersion BigInt
    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
