-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplateTag
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblPointTemplateTag]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(50),
@EngineeringUnitsType int,
@EngineeringUnitsIndex int,
@DecimalPlaces tinyint,
@ServerEngineeringUnitsIndex int,
@ValueType nvarchar(max),
@Value xml,
@Maximum float,
@Minimum float,
@PointTagInputOutputTypeIndex int,
@Input bit,
@AlarmStatus bit,
@ApplyPointTemplateEngineeringUnits bit,
@ApplyPointTemplateDecimalPlaces bit,
@ApplyPointTemplateMaximum bit,
@ApplyPointTemplateMinimum bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@PointTemplateTagGuid uniqueidentifier,
@PointTemplateGuid uniqueidentifier,
@WellKnownIdentityGuid uniqueidentifier,
@AlarmsEnabled bit,
@InhibitInputOutputTypeConfiguration bit,
@InhibitOverride bit,
@Module bit,
@Archived bit,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblPointTemplateTag] AS existingData
        USING (SELECT @ID 'ID',@EngineeringUnitsType 'EngineeringUnitsType',@EngineeringUnitsIndex 'EngineeringUnitsIndex',@DecimalPlaces 'DecimalPlaces',@ServerEngineeringUnitsIndex 'ServerEngineeringUnitsIndex',@ValueType 'ValueType',@Value 'Value',@Maximum 'Maximum',@Minimum 'Minimum',@PointTagInputOutputTypeIndex 'PointTagInputOutputTypeIndex',@Input 'Input',@AlarmStatus 'AlarmStatus',@ApplyPointTemplateEngineeringUnits 'ApplyPointTemplateEngineeringUnits',@ApplyPointTemplateDecimalPlaces 'ApplyPointTemplateDecimalPlaces',@ApplyPointTemplateMaximum 'ApplyPointTemplateMaximum',@ApplyPointTemplateMinimum 'ApplyPointTemplateMinimum',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PointTemplateTagGuid 'PointTemplateTagGuid',@PointTemplateGuid 'PointTemplateGuid',@WellKnownIdentityGuid 'WellKnownIdentityGuid',@AlarmsEnabled 'AlarmsEnabled',@InhibitInputOutputTypeConfiguration 'InhibitInputOutputTypeConfiguration',@InhibitOverride 'InhibitOverride',@Module 'Module',@Archived 'Archived'
                ) AS remoteChanges ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],[Value],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointTemplateEngineeringUnits],[ApplyPointTemplateDecimalPlaces],[ApplyPointTemplateMaximum],[ApplyPointTemplateMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateTagGuid],[PointTemplateGuid],[WellKnownIdentityGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Module],[Archived])
        ON (existingData.[PointTemplateTagGuid] = remoteChanges.[PointTemplateTagGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = remoteChanges.[ID]
                       ,[EngineeringUnitsType] = remoteChanges.[EngineeringUnitsType]
                       ,[EngineeringUnitsIndex] = remoteChanges.[EngineeringUnitsIndex]
                       ,[DecimalPlaces] = remoteChanges.[DecimalPlaces]
                       ,[ServerEngineeringUnitsIndex] = remoteChanges.[ServerEngineeringUnitsIndex]
                       ,[ValueType] = remoteChanges.[ValueType]
                       ,[Value] = remoteChanges.[Value]
                       ,[Maximum] = remoteChanges.[Maximum]
                       ,[Minimum] = remoteChanges.[Minimum]
                       ,[PointTagInputOutputTypeIndex] = remoteChanges.[PointTagInputOutputTypeIndex]
                       ,[Input] = remoteChanges.[Input]
                       ,[AlarmStatus] = remoteChanges.[AlarmStatus]
                       ,[ApplyPointTemplateEngineeringUnits] = remoteChanges.[ApplyPointTemplateEngineeringUnits]
                       ,[ApplyPointTemplateDecimalPlaces] = remoteChanges.[ApplyPointTemplateDecimalPlaces]
                       ,[ApplyPointTemplateMaximum] = remoteChanges.[ApplyPointTemplateMaximum]
                       ,[ApplyPointTemplateMinimum] = remoteChanges.[ApplyPointTemplateMinimum]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[PointTemplateGuid] = remoteChanges.[PointTemplateGuid]
                       ,[WellKnownIdentityGuid] = remoteChanges.[WellKnownIdentityGuid]
                       ,[AlarmsEnabled] = remoteChanges.[AlarmsEnabled]
                       ,[InhibitInputOutputTypeConfiguration] = remoteChanges.[InhibitInputOutputTypeConfiguration]
                       ,[InhibitOverride] = remoteChanges.[InhibitOverride]
                       ,[Module] = remoteChanges.[Module]
                       ,[Archived] = remoteChanges.[Archived]

        WHEN NOT MATCHED THEN
            INSERT ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],[Value],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointTemplateEngineeringUnits],[ApplyPointTemplateDecimalPlaces],[ApplyPointTemplateMaximum],[ApplyPointTemplateMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateTagGuid],[PointTemplateGuid],[WellKnownIdentityGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Module],[Archived])
                VALUES (@ID,@EngineeringUnitsType,@EngineeringUnitsIndex,@DecimalPlaces,@ServerEngineeringUnitsIndex,@ValueType,@Value,@Maximum,@Minimum,@PointTagInputOutputTypeIndex,@Input,@AlarmStatus,@ApplyPointTemplateEngineeringUnits,@ApplyPointTemplateDecimalPlaces,@ApplyPointTemplateMaximum,@ApplyPointTemplateMinimum,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PointTemplateTagGuid,@PointTemplateGuid,@WellKnownIdentityGuid,@AlarmsEnabled,@InhibitInputOutputTypeConfiguration,@InhibitOverride,@Module,@Archived)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPointTemplateTag] WHERE PointTemplateTagGuid = @PointTemplateTagGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
