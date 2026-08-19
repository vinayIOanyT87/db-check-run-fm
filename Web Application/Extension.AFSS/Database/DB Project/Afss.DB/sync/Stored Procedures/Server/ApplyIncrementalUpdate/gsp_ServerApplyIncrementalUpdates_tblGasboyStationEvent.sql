-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyStationEvent
-- Description:	Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblGasboyStationEvent]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblGasboyStationEvent varchar(8000)
AS
BEGIN
    DECLARE @wasDeleted int
    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyStationEvent]
                            INNER JOIN [track].[tblGasboyStationEvent] CT
                                ON CT.PK_GasboyStationEventGuid = [dbo].[tblGasboyStationEvent].[GasboyStationEventGuid] 
                        WHERE CT.PK_GasboyStationEventGuid = @GasboyStationEventGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblGasboyStationEvent].[GasboyStationEventGuid],[dbo].[tblGasboyStationEvent].[ExternalStationLogGuid],[dbo].[tblGasboyStationEvent].[EventID],[dbo].[tblGasboyStationEvent].[LookupGasboyEventErrorClassCodeIndex],[dbo].[tblGasboyStationEvent].[ErrorCode],[dbo].[tblGasboyStationEvent].[FleetID],[dbo].[tblGasboyStationEvent].[ObjectID],[dbo].[tblGasboyStationEvent].[LookupGasboyEventObjectTypeIndex],[dbo].[tblGasboyStationEvent].[DeviceName],[dbo].[tblGasboyStationEvent].[Field1],[dbo].[tblGasboyStationEvent].[Field2],[dbo].[tblGasboyStationEvent].[Field3],[dbo].[tblGasboyStationEvent].[Field4],[dbo].[tblGasboyStationEvent].[Field5],[dbo].[tblGasboyStationEvent].[Field6],[dbo].[tblGasboyStationEvent].[Field7],[dbo].[tblGasboyStationEvent].[Field8],[dbo].[tblGasboyStationEvent].[CreatedBy],[dbo].[tblGasboyStationEvent].[CreatedDate],[dbo].[tblGasboyStationEvent].[UpdatedBy],[dbo].[tblGasboyStationEvent].[UpdatedDate]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblGasboyStationEvent]
                        INNER JOIN [track].[tblGasboyStationEvent] CT
                            ON CT.PK_GasboyStationEventGuid = [dbo].[tblGasboyStationEvent].[GasboyStationEventGuid] 
                    WHERE CT.PK_GasboyStationEventGuid = @GasboyStationEventGuid
            ) MERGE existingData
            USING (SELECT @GasboyStationEventGuid,@ExternalStationLogGuid,@EventID,@LookupGasboyEventErrorClassCodeIndex,@ErrorCode,@FleetID,@ObjectID,@LookupGasboyEventObjectTypeIndex,@DeviceName,@Field1,@Field2,@Field3,@Field4,@Field5,@Field6,@Field7,@Field8,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate
                    ) AS remoteChanges ([GasboyStationEventGuid],[ExternalStationLogGuid],[EventID],[LookupGasboyEventErrorClassCodeIndex],[ErrorCode],[FleetID],[ObjectID],[LookupGasboyEventObjectTypeIndex],[DeviceName],[Field1],[Field2],[Field3],[Field4],[Field5],[Field6],[Field7],[Field8],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
            ON (existingData.[GasboyStationEventGuid] = remoteChanges.[GasboyStationEventGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ExternalStationLogGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExternalStationLogGuid'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[ExternalStationLogGuid] ELSE remoteChanges.[ExternalStationLogGuid] END
                       ,[EventID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EventID'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[EventID] ELSE remoteChanges.[EventID] END
                       ,[LookupGasboyEventErrorClassCodeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyEventErrorClassCodeIndex'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[LookupGasboyEventErrorClassCodeIndex] ELSE remoteChanges.[LookupGasboyEventErrorClassCodeIndex] END
                       ,[ErrorCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ErrorCode'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[ErrorCode] ELSE remoteChanges.[ErrorCode] END
                       ,[FleetID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FleetID'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[FleetID] ELSE remoteChanges.[FleetID] END
                       ,[ObjectID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ObjectID'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[ObjectID] ELSE remoteChanges.[ObjectID] END
                       ,[LookupGasboyEventObjectTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyEventObjectTypeIndex'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[LookupGasboyEventObjectTypeIndex] ELSE remoteChanges.[LookupGasboyEventObjectTypeIndex] END
                       ,[DeviceName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeviceName'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[DeviceName] ELSE remoteChanges.[DeviceName] END
                       ,[Field1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field1'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[Field1] ELSE remoteChanges.[Field1] END
                       ,[Field2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field2'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[Field2] ELSE remoteChanges.[Field2] END
                       ,[Field3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field3'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[Field3] ELSE remoteChanges.[Field3] END
                       ,[Field4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field4'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[Field4] ELSE remoteChanges.[Field4] END
                       ,[Field5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field5'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[Field5] ELSE remoteChanges.[Field5] END
                       ,[Field6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field6'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[Field6] ELSE remoteChanges.[Field6] END
                       ,[Field7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field7'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[Field7] ELSE remoteChanges.[Field7] END
                       ,[Field8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field8'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[Field8] ELSE remoteChanges.[Field8] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END

            WHEN NOT MATCHED THEN
                INSERT ([GasboyStationEventGuid],[ExternalStationLogGuid],[EventID],[LookupGasboyEventErrorClassCodeIndex],[ErrorCode],[FleetID],[ObjectID],[LookupGasboyEventObjectTypeIndex],[DeviceName],[Field1],[Field2],[Field3],[Field4],[Field5],[Field6],[Field7],[Field8],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                    VALUES (@GasboyStationEventGuid,@ExternalStationLogGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EventID'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @EventID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyEventErrorClassCodeIndex'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @LookupGasboyEventErrorClassCodeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ErrorCode'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @ErrorCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FleetID'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @FleetID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ObjectID'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @ObjectID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyEventObjectTypeIndex'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @LookupGasboyEventObjectTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeviceName'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @DeviceName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field1'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @Field1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field2'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @Field2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field3'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @Field3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field4'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @Field4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field5'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @Field5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field6'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @Field6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field7'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @Field7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Field8'), @sync_supported_columns_tblGasboyStationEvent)) WHEN 0 THEN NULL ELSE @Field8 END),@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate)
            ;
    END

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
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
