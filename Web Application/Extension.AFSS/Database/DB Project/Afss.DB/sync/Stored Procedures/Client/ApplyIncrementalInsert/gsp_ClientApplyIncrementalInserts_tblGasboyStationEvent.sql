-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyStationEvent
-- Description:	Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblGasboyStationEvent]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@GasboyStationEventGuid uniqueidentifier,
@ExternalStationLogGuid uniqueidentifier,
@EventID int,
@LookupGasboyEventErrorClassCodeIndex int,
@ErrorCode int,
@FleetID int,
@ObjectID int,
@LookupGasboyEventObjectTypeIndex int,
@DeviceName nvarchar(100),
@Field1 nvarchar(100),
@Field2 nvarchar(100),
@Field3 nvarchar(100),
@Field4 nvarchar(100),
@Field5 nvarchar(100),
@Field6 nvarchar(100),
@Field7 nvarchar(100),
@Field8 nvarchar(100),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    ;   MERGE [dbo].[tblGasboyStationEvent] AS existingData
        USING (SELECT @GasboyStationEventGuid 'GasboyStationEventGuid',@ExternalStationLogGuid 'ExternalStationLogGuid',@EventID 'EventID',@LookupGasboyEventErrorClassCodeIndex 'LookupGasboyEventErrorClassCodeIndex',@ErrorCode 'ErrorCode',@FleetID 'FleetID',@ObjectID 'ObjectID',@LookupGasboyEventObjectTypeIndex 'LookupGasboyEventObjectTypeIndex',@DeviceName 'DeviceName',@Field1 'Field1',@Field2 'Field2',@Field3 'Field3',@Field4 'Field4',@Field5 'Field5',@Field6 'Field6',@Field7 'Field7',@Field8 'Field8',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate'
                ) AS remoteChanges ([GasboyStationEventGuid],[ExternalStationLogGuid],[EventID],[LookupGasboyEventErrorClassCodeIndex],[ErrorCode],[FleetID],[ObjectID],[LookupGasboyEventObjectTypeIndex],[DeviceName],[Field1],[Field2],[Field3],[Field4],[Field5],[Field6],[Field7],[Field8],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
        ON (existingData.[GasboyStationEventGuid] = remoteChanges.[GasboyStationEventGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ExternalStationLogGuid] = remoteChanges.[ExternalStationLogGuid]
                       ,[EventID] = remoteChanges.[EventID]
                       ,[LookupGasboyEventErrorClassCodeIndex] = remoteChanges.[LookupGasboyEventErrorClassCodeIndex]
                       ,[ErrorCode] = remoteChanges.[ErrorCode]
                       ,[FleetID] = remoteChanges.[FleetID]
                       ,[ObjectID] = remoteChanges.[ObjectID]
                       ,[LookupGasboyEventObjectTypeIndex] = remoteChanges.[LookupGasboyEventObjectTypeIndex]
                       ,[DeviceName] = remoteChanges.[DeviceName]
                       ,[Field1] = remoteChanges.[Field1]
                       ,[Field2] = remoteChanges.[Field2]
                       ,[Field3] = remoteChanges.[Field3]
                       ,[Field4] = remoteChanges.[Field4]
                       ,[Field5] = remoteChanges.[Field5]
                       ,[Field6] = remoteChanges.[Field6]
                       ,[Field7] = remoteChanges.[Field7]
                       ,[Field8] = remoteChanges.[Field8]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]

        WHEN NOT MATCHED THEN
            INSERT ([GasboyStationEventGuid],[ExternalStationLogGuid],[EventID],[LookupGasboyEventErrorClassCodeIndex],[ErrorCode],[FleetID],[ObjectID],[LookupGasboyEventObjectTypeIndex],[DeviceName],[Field1],[Field2],[Field3],[Field4],[Field5],[Field6],[Field7],[Field8],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                VALUES (@GasboyStationEventGuid,@ExternalStationLogGuid,@EventID,@LookupGasboyEventErrorClassCodeIndex,@ErrorCode,@FleetID,@ObjectID,@LookupGasboyEventObjectTypeIndex,@DeviceName,@Field1,@Field2,@Field3,@Field4,@Field5,@Field6,@Field7,@Field8,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyStationEventGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyStationEventGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyStationEventGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyStationEvent] WHERE GasboyStationEventGuid = @GasboyStationEventGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    DECLARE @minValidVersion BigInt
    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
