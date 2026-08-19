-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMessageLog
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblMessageLog]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@MessageLogGuid uniqueidentifier,
@CompanyGuid uniqueidentifier,
@MessageGuid uniqueidentifier,
@PersonnelGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblMessageLog varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblMessageLog] AS existingData
        USING (SELECT @CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@MessageLogGuid 'MessageLogGuid',@CompanyGuid 'CompanyGuid',@MessageGuid 'MessageGuid',@PersonnelGuid 'PersonnelGuid'
                ) AS remoteChanges ([CreatedDate],[CreatedBy],[MessageLogGuid],[CompanyGuid],[MessageGuid],[PersonnelGuid])
        ON (existingData.[MessageLogGuid] = remoteChanges.[MessageLogGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate) THEN
            UPDATE SET [CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[CompanyGuid] ELSE remoteChanges.[CompanyGuid] END
                       ,[MessageGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MessageGuid'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[MessageGuid] ELSE remoteChanges.[MessageGuid] END
                       ,[PersonnelGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PersonnelGuid'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[PersonnelGuid] ELSE remoteChanges.[PersonnelGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([CreatedDate],[CreatedBy],[MessageLogGuid],[CompanyGuid],[MessageGuid],[PersonnelGuid])
                VALUES (@CreatedDate,@CreatedBy,@MessageLogGuid,@CompanyGuid,@MessageGuid,@PersonnelGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageLogGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageLogGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageLogGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblMessageLog] WHERE (CreatedDate >= @CreatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

