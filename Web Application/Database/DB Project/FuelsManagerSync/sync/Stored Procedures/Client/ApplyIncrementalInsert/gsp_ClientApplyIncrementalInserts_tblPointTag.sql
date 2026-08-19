-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTag
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblPointTag]
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
@Status bigint,
@Value xml,
@ServerTimeStamp datetimeoffset(7),
@SourceTimeStamp datetimeoffset(7),
@Maximum float,
@Minimum float,
@PointTagInputOutputTypeIndex int,
@LastPointTagInputOutputTypeIndex int,
@Input bit,
@AlarmStatus bit,
@ApplyPointEngineeringUnits bit,
@ApplyPointDecimalPlaces bit,
@ApplyPointMaximum bit,
@ApplyPointMinimum bit,
@OpcUaServerGuid uniqueidentifier,
@OpcUaBrowsePath nvarchar(250),
@OpcUaNamespaceUri nvarchar(250),
@OpcUaPublishingInterval int,
@OpcUaNodeId nvarchar(250),
@OpcUaIsReadable bit,
@OpcUaServerDataType int,
@OpcUaWriteHoldoffTime int,
@OpcUaWritePeriodicUpdateInterval int,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@PointTagGuid uniqueidentifier,
@PointGuid uniqueidentifier,
@PointTemplateTagGuid uniqueidentifier,
@AlarmsEnabled bit,
@InhibitInputOutputTypeConfiguration bit,
@InhibitOverride bit,
@Deadband float,
@Holdoff int,
@Archived bit,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
   ;   MERGE [dbo].[tblPointTag] AS existingData
        USING (SELECT @ID 'ID',@EngineeringUnitsType 'EngineeringUnitsType',@EngineeringUnitsIndex 'EngineeringUnitsIndex',@DecimalPlaces 'DecimalPlaces',@ServerEngineeringUnitsIndex 'ServerEngineeringUnitsIndex',@ValueType 'ValueType',@Status 'Status',@Value 'Value',@ServerTimeStamp 'ServerTimeStamp',@SourceTimeStamp 'SourceTimeStamp',@Maximum 'Maximum',@Minimum 'Minimum',@PointTagInputOutputTypeIndex 'PointTagInputOutputTypeIndex',@LastPointTagInputOutputTypeIndex 'LastPointTagInputOutputTypeIndex',@Input 'Input',@AlarmStatus 'AlarmStatus',@ApplyPointEngineeringUnits 'ApplyPointEngineeringUnits',@ApplyPointDecimalPlaces 'ApplyPointDecimalPlaces',@ApplyPointMaximum 'ApplyPointMaximum',@ApplyPointMinimum 'ApplyPointMinimum',@OpcUaServerGuid 'OpcUaServerGuid',@OpcUaBrowsePath 'OpcUaBrowsePath',@OpcUaNamespaceUri 'OpcUaNamespaceUri',@OpcUaPublishingInterval 'OpcUaPublishingInterval',@OpcUaNodeId 'OpcUaNodeId',@OpcUaIsReadable 'OpcUaIsReadable',@OpcUaServerDataType 'OpcUaServerDataType',@OpcUaWriteHoldoffTime 'OpcUaWriteHoldoffTime',@OpcUaWritePeriodicUpdateInterval 'OpcUaWritePeriodicUpdateInterval',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PointTagGuid 'PointTagGuid',@PointGuid 'PointGuid',@PointTemplateTagGuid 'PointTemplateTagGuid',@AlarmsEnabled 'AlarmsEnabled',@InhibitInputOutputTypeConfiguration 'InhibitInputOutputTypeConfiguration',@InhibitOverride 'InhibitOverride',@Deadband 'Deadband',@Holdoff 'Holdoff',@Archived 'Archived'
                ) AS remoteChanges ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],[Status],[Value],[ServerTimeStamp],[SourceTimeStamp],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[LastPointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointEngineeringUnits],[ApplyPointDecimalPlaces],[ApplyPointMaximum],[ApplyPointMinimum],[OpcUaServerGuid],[OpcUaBrowsePath],[OpcUaNamespaceUri],[OpcUaPublishingInterval],[OpcUaNodeId],[OpcUaIsReadable],[OpcUaServerDataType],[OpcUaWriteHoldoffTime],[OpcUaWritePeriodicUpdateInterval],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTagGuid],[PointGuid],[PointTemplateTagGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Deadband],[Holdoff],[Archived])
        ON (existingData.[PointTagGuid] = remoteChanges.[PointTagGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = remoteChanges.[ID]
                       ,[EngineeringUnitsType] = remoteChanges.[EngineeringUnitsType]
                       ,[EngineeringUnitsIndex] = remoteChanges.[EngineeringUnitsIndex]
                       ,[DecimalPlaces] = remoteChanges.[DecimalPlaces]
                       ,[ServerEngineeringUnitsIndex] = remoteChanges.[ServerEngineeringUnitsIndex]
                       ,[ValueType] = remoteChanges.[ValueType]
                       ,[Status] = 
                             CASE WHEN remoteChanges.[Status] & 0xFFFF0000 <> 1083113472 THEN 
					                   CASE WHEN remoteChanges.[PointTagInputOutputTypeIndex] = 1 THEN remoteChanges.[Status]
                                       WHEN remoteChanges.[Status] & 0xFFFF0000 = 9830400 THEN remoteChanges.[Status]
			                              WHEN remoteChanges.[ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND existingData.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') <> RemoteChanges.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') THEN remoteChanges.[Status]
			                              WHEN remoteChanges.[ValueType] = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND existingData.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') <> RemoteChanges.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') THEN remoteChanges.[Status]
                                       WHEN (existingData.[Status] & 0xFFFF0000 = 9830400) AND (remoteChanges.[Status] & 0x0FFF0000 <> 9830400) THEN remoteChanges.[Status]
                                       WHEN existingData.[Status] & 0xFFFF0000 = 9830400 THEN existingData.[Status]
                                       ELSE existingData.[Status]
									       END
		                            ELSE existingData.[Status]
						           END
                       ,[Value] = 
                             CASE WHEN remoteChanges.[Status] & 0xFFFF0000 <> 1083113472 THEN 
					                   CASE WHEN remoteChanges.[PointTagInputOutputTypeIndex] = 1 THEN remoteChanges.[Value]
                                       WHEN remoteChanges.[Status] & 0xFFFF0000 = 9830400 THEN remoteChanges.[Value]
			                              WHEN remoteChanges.[ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND existingData.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') <> RemoteChanges.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') THEN remoteChanges.[Value]
			                              WHEN remoteChanges.[ValueType] = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND existingData.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') <> RemoteChanges.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') THEN remoteChanges.[Value]
                                       WHEN (existingData.[Status] & 0xFFFF0000 = 9830400) AND (remoteChanges.[Status] & 0x0FFF0000 <> 9830400) THEN remoteChanges.[Value]
                                       WHEN existingData.[Status] & 0xFFFF0000 = 9830400 THEN existingData.[Value]
                                       ELSE existingData.[Value]
									       END
		                            ELSE existingData.[Value]
						           END
                       ,[ServerTimeStamp] = 
                             CASE WHEN remoteChanges.[Status] & 0xFFFF0000 <> 1083113472 THEN 
					                   CASE WHEN remoteChanges.[PointTagInputOutputTypeIndex] = 1 THEN remoteChanges.[ServerTimeStamp]
                                       WHEN remoteChanges.[Status] & 0xFFFF0000 = 9830400 THEN remoteChanges.[ServerTimeStamp]
			                              WHEN remoteChanges.[ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND existingData.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') <> RemoteChanges.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') THEN remoteChanges.[ServerTimeStamp]
			                              WHEN remoteChanges.[ValueType] = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND existingData.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') <> RemoteChanges.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') THEN remoteChanges.[ServerTimeStamp]
                                       WHEN (existingData.[Status] & 0xFFFF0000 = 9830400) AND (remoteChanges.[Status] & 0x0FFF0000 <> 9830400) THEN remoteChanges.[ServerTimeStamp]
                                       WHEN existingData.[Status] & 0xFFFF0000 = 9830400 THEN existingData.[ServerTimeStamp]
                                       ELSE existingData.[ServerTimeStamp]
									       END
		                            ELSE existingData.[ServerTimeStamp]
						           END
                       ,[SourceTimeStamp] = 
                             CASE WHEN remoteChanges.[Status] & 0xFFFF0000 <> 1083113472 THEN 
					                   CASE WHEN remoteChanges.[PointTagInputOutputTypeIndex] = 1 THEN remoteChanges.[SourceTimeStamp]
                                       WHEN remoteChanges.[Status] & 0xFFFF0000 = 9830400 THEN remoteChanges.[SourceTimeStamp]
			                              WHEN remoteChanges.[ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND existingData.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') <> RemoteChanges.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') THEN remoteChanges.[SourceTimeStamp]
			                              WHEN remoteChanges.[ValueType] = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND existingData.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') <> RemoteChanges.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') THEN remoteChanges.[SourceTimeStamp]
                                       WHEN (existingData.[Status] & 0xFFFF0000 = 9830400) AND (remoteChanges.[Status] & 0x0FFF0000 <> 9830400) THEN remoteChanges.[SourceTimeStamp]
                                       WHEN existingData.[Status] & 0xFFFF0000 = 9830400 THEN existingData.[SourceTimeStamp]
                                       ELSE existingData.[SourceTimeStamp]
									       END
		                            ELSE existingData.[SourceTimeStamp]
						           END
                       ,[Maximum] = remoteChanges.[Maximum]
                       ,[Minimum] = remoteChanges.[Minimum]
                       ,[PointTagInputOutputTypeIndex] = remoteChanges.[PointTagInputOutputTypeIndex]
                       ,[LastPointTagInputOutputTypeIndex] = remoteChanges.[LastPointTagInputOutputTypeIndex]
                       ,[Input] = remoteChanges.[Input]
                       ,[AlarmStatus] = remoteChanges.[AlarmStatus]
                       ,[ApplyPointEngineeringUnits] = remoteChanges.[ApplyPointEngineeringUnits]
                       ,[ApplyPointDecimalPlaces] = remoteChanges.[ApplyPointDecimalPlaces]
                       ,[ApplyPointMaximum] = remoteChanges.[ApplyPointMaximum]
                       ,[ApplyPointMinimum] = remoteChanges.[ApplyPointMinimum]
                       ,[OpcUaServerGuid] = remoteChanges.[OpcUaServerGuid]
                       ,[OpcUaBrowsePath] = remoteChanges.[OpcUaBrowsePath]
                       ,[OpcUaNamespaceUri] = remoteChanges.[OpcUaNamespaceUri]
                       ,[OpcUaPublishingInterval] = remoteChanges.[OpcUaPublishingInterval]
                       ,[OpcUaNodeId] = remoteChanges.[OpcUaNodeId]
                       ,[OpcUaIsReadable] = remoteChanges.[OpcUaIsReadable]
                       ,[OpcUaServerDataType] = remoteChanges.[OpcUaServerDataType]
                       ,[OpcUaWriteHoldoffTime] = remoteChanges.[OpcUaWriteHoldoffTime]
                       ,[OpcUaWritePeriodicUpdateInterval] = remoteChanges.[OpcUaWritePeriodicUpdateInterval]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[PointGuid] = remoteChanges.[PointGuid]
                       ,[PointTemplateTagGuid] = remoteChanges.[PointTemplateTagGuid]
                       ,[AlarmsEnabled] = remoteChanges.[AlarmsEnabled]
                       ,[InhibitInputOutputTypeConfiguration] = remoteChanges.[InhibitInputOutputTypeConfiguration]
                       ,[InhibitOverride] = remoteChanges.[InhibitOverride]
                       ,[Deadband] = remoteChanges.[Deadband]
                       ,[Holdoff] = remoteChanges.[Holdoff]
                       ,[Archived] = remoteChanges.[Archived]

        WHEN NOT MATCHED THEN
            INSERT ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],[Status],[Value],[ServerTimeStamp],[SourceTimeStamp],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointEngineeringUnits],[ApplyPointDecimalPlaces],[ApplyPointMaximum],[ApplyPointMinimum],[OpcUaServerGuid],[OpcUaBrowsePath],[OpcUaNamespaceUri],[OpcUaPublishingInterval],[OpcUaNodeId],[OpcUaIsReadable],[OpcUaServerDataType],[OpcUaWriteHoldoffTime],[OpcUaWritePeriodicUpdateInterval],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTagGuid],[PointGuid],[PointTemplateTagGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Deadband],[Holdoff],[Archived])
                VALUES (@ID,@EngineeringUnitsType,@EngineeringUnitsIndex,@DecimalPlaces,@ServerEngineeringUnitsIndex,@ValueType
                        ,CASE WHEN @Status & 0xFFFF0000 = 9830400 THEN @Status
                              WHEN @ValueType = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND @Value.value('(PointCommandStatusListReference/CurrentValue)[1]','nvarchar(max)') <> '' THEN 0
                              WHEN @ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND @Value.value('(DeviceAlarmMapReference/CurrentValue)[1]','nvarchar(max)') <> '' THEN 0
                              WHEN @PointTagInputOutputTypeIndex = 1 AND @Value IS NOT NULL AND @ValueType NOT IN ('FMBusinessObjects.DataObjects.PointCommandStatusListReference','FMBusinessObjects.DataObjects.DeviceAlarmMapReference') THEN 0 
                              WHEN @PointTagInputOutputTypeIndex = 2 AND @Status & 0xC0000000 = 0 THEN @Status
                              ELSE 0x80000000
                         END
                        ,CASE WHEN @PointTagInputOutputTypeIndex = 1 THEN @Value
                              ELSE CASE WHEN @Status & 0xFFFF0000 = 9830400 THEN @Value
                                        WHEN @PointTagInputOutputTypeIndex = 2 AND @Status & 0xC0000000 = 0 THEN @Value
                                        WHEN @ValueType IN ('FMBusinessObjects.DataObjects.DeviceAlarmMapReference', 'FMBusinessObjects.DataObjects.PointCommandStatusListReference') THEN @Value
                                        ELSE null
                                   END
				             END
                       ,CASE WHEN @PointTagInputOutputTypeIndex = 1 THEN @ServerTimeStamp
                              ELSE CASE WHEN @Status & 0xFFFF0000 = 9830400 THEN @ServerTimeStamp
                                        WHEN @PointTagInputOutputTypeIndex = 2 AND @Status & 0xC0000000 = 0 THEN @ServerTimeStamp
                                        WHEN @ValueType IN ('FMBusinessObjects.DataObjects.DeviceAlarmMapReference', 'FMBusinessObjects.DataObjects.PointCommandStatusListReference') THEN @ServerTimeStamp
                                        ELSE SWITCHOFFSET(@CreatedDate,0)
                                   END
				             END
                      ,CASE WHEN @PointTagInputOutputTypeIndex = 1 THEN @SourceTimeStamp
                              ELSE CASE WHEN @Status & 0xFFFF0000 = 9830400 THEN @SourceTimeStamp
                                        WHEN @PointTagInputOutputTypeIndex = 2 AND @Status & 0xC0000000 = 0 THEN @SourceTimeStamp
                                        WHEN @ValueType IN ('FMBusinessObjects.DataObjects.DeviceAlarmMapReference', 'FMBusinessObjects.DataObjects.PointCommandStatusListReference') THEN @SourceTimeStamp
                                        ELSE SWITCHOFFSET(@CreatedDate,0)
                                   END
				             END
							  ,@Maximum,@Minimum,@PointTagInputOutputTypeIndex,@Input,@AlarmStatus,@ApplyPointEngineeringUnits,@ApplyPointDecimalPlaces,@ApplyPointMaximum,@ApplyPointMinimum,@OpcUaServerGuid,@OpcUaBrowsePath,@OpcUaNamespaceUri,@OpcUaPublishingInterval,@OpcUaNodeId,@OpcUaIsReadable,@OpcUaServerDataType,@OpcUaWriteHoldoffTime,@OpcUaWritePeriodicUpdateInterval,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PointTagGuid,@PointGuid,@PointTemplateTagGuid,@AlarmsEnabled,@InhibitInputOutputTypeConfiguration,@InhibitOverride,@Deadband,@Holdoff,@Archived)
        ;



    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTagGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTagGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTagGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPointTag] WHERE PointTagGuid = @PointTagGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
