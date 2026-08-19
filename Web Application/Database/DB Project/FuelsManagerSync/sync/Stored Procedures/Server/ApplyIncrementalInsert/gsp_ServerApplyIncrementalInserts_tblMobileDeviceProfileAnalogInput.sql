-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMobileDeviceProfileAnalogInput
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblMobileDeviceProfileAnalogInput]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@MobileDeviceProfileAnalogInputGuid uniqueidentifier,
@MobileDeviceProfileGuid uniqueidentifier,
@LowLimit float,
@HighLimit float,
@ParameterA nvarchar(20),
@ParameterB nvarchar(20),
@ParameterC nvarchar(20),
@AnalogFormula nvarchar(50),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblMobileDeviceProfileAnalogInput varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblMobileDeviceProfileAnalogInput] AS existingData
        USING (SELECT @MobileDeviceProfileAnalogInputGuid 'MobileDeviceProfileAnalogInputGuid',@MobileDeviceProfileGuid 'MobileDeviceProfileGuid',@LowLimit 'LowLimit',@HighLimit 'HighLimit',@ParameterA 'ParameterA',@ParameterB 'ParameterB',@ParameterC 'ParameterC',@AnalogFormula 'AnalogFormula',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy'
                ) AS remoteChanges ([MobileDeviceProfileAnalogInputGuid],[MobileDeviceProfileGuid],[LowLimit],[HighLimit],[ParameterA],[ParameterB],[ParameterC],[AnalogFormula],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
        ON (existingData.[MobileDeviceProfileAnalogInputGuid] = remoteChanges.[MobileDeviceProfileAnalogInputGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [MobileDeviceProfileGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MobileDeviceProfileGuid'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[MobileDeviceProfileGuid] ELSE remoteChanges.[MobileDeviceProfileGuid] END
                       ,[LowLimit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowLimit'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[LowLimit] ELSE remoteChanges.[LowLimit] END
                       ,[HighLimit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HighLimit'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[HighLimit] ELSE remoteChanges.[HighLimit] END
                       ,[ParameterA] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParameterA'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[ParameterA] ELSE remoteChanges.[ParameterA] END
                       ,[ParameterB] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParameterB'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[ParameterB] ELSE remoteChanges.[ParameterB] END
                       ,[ParameterC] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParameterC'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[ParameterC] ELSE remoteChanges.[ParameterC] END
                       ,[AnalogFormula] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AnalogFormula'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[AnalogFormula] ELSE remoteChanges.[AnalogFormula] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

        WHEN NOT MATCHED THEN
            INSERT ([MobileDeviceProfileAnalogInputGuid],[MobileDeviceProfileGuid],[LowLimit],[HighLimit],[ParameterA],[ParameterB],[ParameterC],[AnalogFormula],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                VALUES (@MobileDeviceProfileAnalogInputGuid,@MobileDeviceProfileGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowLimit'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN NULL ELSE @LowLimit END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HighLimit'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN NULL ELSE @HighLimit END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParameterA'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN NULL ELSE @ParameterA END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParameterB'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN NULL ELSE @ParameterB END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParameterC'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN NULL ELSE @ParameterC END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AnalogFormula'), @sync_supported_columns_tblMobileDeviceProfileAnalogInput)) WHEN 0 THEN NULL ELSE @AnalogFormula END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfileAnalogInputGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfileAnalogInputGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfileAnalogInputGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblMobileDeviceProfileAnalogInput] WHERE MobileDeviceProfileAnalogInputGuid = @MobileDeviceProfileAnalogInputGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

