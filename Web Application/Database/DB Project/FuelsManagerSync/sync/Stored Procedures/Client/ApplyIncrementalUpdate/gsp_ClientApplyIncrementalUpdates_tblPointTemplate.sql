-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplate
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblPointTemplate]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(30),
@Description nvarchar(50),
@Standard bit,
@ExecutionInterval int,
@LevelUnitIndex int,
@TemperatureUnitIndex int,
@DensityUnitIndex int,
@PressureUnitIndex int,
@FlowUnitIndex int,
@VolumeUnitIndex int,
@MassUnitIndex int,
@VelocityUnitIndex int,
@MassFlowUnitIndex int,
@LevelDecimalPlaces tinyint,
@TemperatureDecimalPlaces tinyint,
@DensityDecimalPlaces tinyint,
@PressureDecimalPlaces tinyint,
@FlowDecimalPlaces tinyint,
@VolumeDecimalPlaces tinyint,
@MassDecimalPlaces tinyint,
@VelocityDecimalPlaces tinyint,
@MassFlowDecimalPlaces tinyint,
@LevelMaximum float,
@LevelMinimum float,
@TemperatureMaximum float,
@TemperatureMinimum float,
@DensityMaximum float,
@DensityMinimum float,
@PressureMaximum float,
@PressureMinimum float,
@VolumetricFlowMaximum float,
@VolumetricFlowMinimum float,
@VolumeMaximum float,
@VolumeMinimum float,
@MassMaximum float,
@MassMinimum float,
@VelocityMaximum float,
@VelocityMinimum float,
@MassFlowMaximum float,
@MassFlowMinimum float,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@PointTemplateGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@PointTemplateTypeApplicationStringGuid uniqueidentifier,
@ProfileImageGuid uniqueidentifier,
@DefaultDrawingGuid uniqueidentifier,
@PointCommandStatus xml,
@DeviceAlarmMaps xml,
@PointLogicScript nvarchar(max),
@Version int,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblPointTemplate] CT
                        WHERE CT.PK_PointTemplateGuid = @PointTemplateGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblPointTemplate].[ID],[dbo].[tblPointTemplate].[Description],[dbo].[tblPointTemplate].[Standard],[dbo].[tblPointTemplate].[ExecutionInterval],[dbo].[tblPointTemplate].[LevelUnitIndex],[dbo].[tblPointTemplate].[TemperatureUnitIndex],[dbo].[tblPointTemplate].[DensityUnitIndex],[dbo].[tblPointTemplate].[PressureUnitIndex],[dbo].[tblPointTemplate].[FlowUnitIndex],[dbo].[tblPointTemplate].[VolumeUnitIndex],[dbo].[tblPointTemplate].[MassUnitIndex],[dbo].[tblPointTemplate].[VelocityUnitIndex],[dbo].[tblPointTemplate].[MassFlowUnitIndex],[dbo].[tblPointTemplate].[LevelDecimalPlaces],[dbo].[tblPointTemplate].[TemperatureDecimalPlaces],[dbo].[tblPointTemplate].[DensityDecimalPlaces],[dbo].[tblPointTemplate].[PressureDecimalPlaces],[dbo].[tblPointTemplate].[FlowDecimalPlaces],[dbo].[tblPointTemplate].[VolumeDecimalPlaces],[dbo].[tblPointTemplate].[MassDecimalPlaces],[dbo].[tblPointTemplate].[VelocityDecimalPlaces],[dbo].[tblPointTemplate].[MassFlowDecimalPlaces],[dbo].[tblPointTemplate].[LevelMaximum],[dbo].[tblPointTemplate].[LevelMinimum],[dbo].[tblPointTemplate].[TemperatureMaximum],[dbo].[tblPointTemplate].[TemperatureMinimum],[dbo].[tblPointTemplate].[DensityMaximum],[dbo].[tblPointTemplate].[DensityMinimum],[dbo].[tblPointTemplate].[PressureMaximum],[dbo].[tblPointTemplate].[PressureMinimum],[dbo].[tblPointTemplate].[VolumetricFlowMaximum],[dbo].[tblPointTemplate].[VolumetricFlowMinimum],[dbo].[tblPointTemplate].[VolumeMaximum],[dbo].[tblPointTemplate].[VolumeMinimum],[dbo].[tblPointTemplate].[MassMaximum],[dbo].[tblPointTemplate].[MassMinimum],[dbo].[tblPointTemplate].[VelocityMaximum],[dbo].[tblPointTemplate].[VelocityMinimum],[dbo].[tblPointTemplate].[MassFlowMaximum],[dbo].[tblPointTemplate].[MassFlowMinimum],[dbo].[tblPointTemplate].[CreatedDate],[dbo].[tblPointTemplate].[CreatedBy],[dbo].[tblPointTemplate].[UpdatedDate],[dbo].[tblPointTemplate].[UpdatedBy],[dbo].[tblPointTemplate].[PointTemplateGuid],[dbo].[tblPointTemplate].[SiteGuid],[dbo].[tblPointTemplate].[PointTemplateTypeApplicationStringGuid],[dbo].[tblPointTemplate].[ProfileImageGuid],[dbo].[tblPointTemplate].[DefaultDrawingGuid],[dbo].[tblPointTemplate].[PointCommandStatus],[dbo].[tblPointTemplate].[DeviceAlarmMaps],[dbo].[tblPointTemplate].[PointLogicScript],[dbo].[tblPointTemplate].[Version]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblPointTemplate]
                        INNER JOIN [track].[tblPointTemplate] CT
                            ON CT.PK_PointTemplateGuid = [dbo].[tblPointTemplate].[PointTemplateGuid] 
                    WHERE CT.PK_PointTemplateGuid = @PointTemplateGuid
            ) MERGE existingData
            USING (SELECT @ID,@Description,@Standard,@ExecutionInterval,@LevelUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@PressureUnitIndex,@FlowUnitIndex,@VolumeUnitIndex,@MassUnitIndex,@VelocityUnitIndex,@MassFlowUnitIndex,@LevelDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@PressureDecimalPlaces,@FlowDecimalPlaces,@VolumeDecimalPlaces,@MassDecimalPlaces,@VelocityDecimalPlaces,@MassFlowDecimalPlaces,@LevelMaximum,@LevelMinimum,@TemperatureMaximum,@TemperatureMinimum,@DensityMaximum,@DensityMinimum,@PressureMaximum,@PressureMinimum,@VolumetricFlowMaximum,@VolumetricFlowMinimum,@VolumeMaximum,@VolumeMinimum,@MassMaximum,@MassMinimum,@VelocityMaximum,@VelocityMinimum,@MassFlowMaximum,@MassFlowMinimum,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PointTemplateGuid,@SiteGuid,@PointTemplateTypeApplicationStringGuid,@ProfileImageGuid,@DefaultDrawingGuid,@PointCommandStatus,@DeviceAlarmMaps,@PointLogicScript,@Version
                    ) AS remoteChanges ([ID],[Description],[Standard],[ExecutionInterval],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[VelocityUnitIndex],[MassFlowUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[VelocityDecimalPlaces],[MassFlowDecimalPlaces],[LevelMaximum],[LevelMinimum],[TemperatureMaximum],[TemperatureMinimum],[DensityMaximum],[DensityMinimum],[PressureMaximum],[PressureMinimum],[VolumetricFlowMaximum],[VolumetricFlowMinimum],[VolumeMaximum],[VolumeMinimum],[MassMaximum],[MassMinimum],[VelocityMaximum],[VelocityMinimum],[MassFlowMaximum],[MassFlowMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateGuid],[SiteGuid],[PointTemplateTypeApplicationStringGuid],[ProfileImageGuid],[DefaultDrawingGuid],[PointCommandStatus],[DeviceAlarmMaps],[PointLogicScript],[Version])
            ON (existingData.[PointTemplateGuid] = remoteChanges.[PointTemplateGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = remoteChanges.[ID]
                       ,[Description] = remoteChanges.[Description]
                       ,[Standard] = remoteChanges.[Standard]
                       ,[ExecutionInterval] = remoteChanges.[ExecutionInterval]
                       ,[LevelUnitIndex] = remoteChanges.[LevelUnitIndex]
                       ,[TemperatureUnitIndex] = remoteChanges.[TemperatureUnitIndex]
                       ,[DensityUnitIndex] = remoteChanges.[DensityUnitIndex]
                       ,[PressureUnitIndex] = remoteChanges.[PressureUnitIndex]
                       ,[FlowUnitIndex] = remoteChanges.[FlowUnitIndex]
                       ,[VolumeUnitIndex] = remoteChanges.[VolumeUnitIndex]
                       ,[MassUnitIndex] = remoteChanges.[MassUnitIndex]
                       ,[VelocityUnitIndex] = remoteChanges.[VelocityUnitIndex]
                       ,[MassFlowUnitIndex] = remoteChanges.[MassFlowUnitIndex]
                       ,[LevelDecimalPlaces] = remoteChanges.[LevelDecimalPlaces]
                       ,[TemperatureDecimalPlaces] = remoteChanges.[TemperatureDecimalPlaces]
                       ,[DensityDecimalPlaces] = remoteChanges.[DensityDecimalPlaces]
                       ,[PressureDecimalPlaces] = remoteChanges.[PressureDecimalPlaces]
                       ,[FlowDecimalPlaces] = remoteChanges.[FlowDecimalPlaces]
                       ,[VolumeDecimalPlaces] = remoteChanges.[VolumeDecimalPlaces]
                       ,[MassDecimalPlaces] = remoteChanges.[MassDecimalPlaces]
                       ,[VelocityDecimalPlaces] = remoteChanges.[VelocityDecimalPlaces]
                       ,[MassFlowDecimalPlaces] = remoteChanges.[MassFlowDecimalPlaces]
                       ,[LevelMaximum] = remoteChanges.[LevelMaximum]
                       ,[LevelMinimum] = remoteChanges.[LevelMinimum]
                       ,[TemperatureMaximum] = remoteChanges.[TemperatureMaximum]
                       ,[TemperatureMinimum] = remoteChanges.[TemperatureMinimum]
                       ,[DensityMaximum] = remoteChanges.[DensityMaximum]
                       ,[DensityMinimum] = remoteChanges.[DensityMinimum]
                       ,[PressureMaximum] = remoteChanges.[PressureMaximum]
                       ,[PressureMinimum] = remoteChanges.[PressureMinimum]
                       ,[VolumetricFlowMaximum] = remoteChanges.[VolumetricFlowMaximum]
                       ,[VolumetricFlowMinimum] = remoteChanges.[VolumetricFlowMinimum]
                       ,[VolumeMaximum] = remoteChanges.[VolumeMaximum]
                       ,[VolumeMinimum] = remoteChanges.[VolumeMinimum]
                       ,[MassMaximum] = remoteChanges.[MassMaximum]
                       ,[MassMinimum] = remoteChanges.[MassMinimum]
                       ,[VelocityMaximum] = remoteChanges.[VelocityMaximum]
                       ,[VelocityMinimum] = remoteChanges.[VelocityMinimum]
                       ,[MassFlowMaximum] = remoteChanges.[MassFlowMaximum]
                       ,[MassFlowMinimum] = remoteChanges.[MassFlowMinimum]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[PointTemplateTypeApplicationStringGuid] = remoteChanges.[PointTemplateTypeApplicationStringGuid]
                       ,[ProfileImageGuid] = remoteChanges.[ProfileImageGuid]
                       ,[DefaultDrawingGuid] = remoteChanges.[DefaultDrawingGuid]
                       ,[PointCommandStatus] = remoteChanges.[PointCommandStatus]
                       ,[DeviceAlarmMaps] = remoteChanges.[DeviceAlarmMaps]
                       ,[PointLogicScript] = remoteChanges.[PointLogicScript]
                       ,[Version] = remoteChanges.[Version]

            WHEN NOT MATCHED THEN
                INSERT ([ID],[Description],[Standard],[ExecutionInterval],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[VelocityUnitIndex],[MassFlowUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[VelocityDecimalPlaces],[MassFlowDecimalPlaces],[LevelMaximum],[LevelMinimum],[TemperatureMaximum],[TemperatureMinimum],[DensityMaximum],[DensityMinimum],[PressureMaximum],[PressureMinimum],[VolumetricFlowMaximum],[VolumetricFlowMinimum],[VolumeMaximum],[VolumeMinimum],[MassMaximum],[MassMinimum],[VelocityMaximum],[VelocityMinimum],[MassFlowMaximum],[MassFlowMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateGuid],[SiteGuid],[PointTemplateTypeApplicationStringGuid],[ProfileImageGuid],[DefaultDrawingGuid],[PointCommandStatus],[DeviceAlarmMaps],[PointLogicScript],[Version])
                    VALUES (@ID,@Description,@Standard,@ExecutionInterval,@LevelUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@PressureUnitIndex,@FlowUnitIndex,@VolumeUnitIndex,@MassUnitIndex,@VelocityUnitIndex,@MassFlowUnitIndex,@LevelDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@PressureDecimalPlaces,@FlowDecimalPlaces,@VolumeDecimalPlaces,@MassDecimalPlaces,@VelocityDecimalPlaces,@MassFlowDecimalPlaces,@LevelMaximum,@LevelMinimum,@TemperatureMaximum,@TemperatureMinimum,@DensityMaximum,@DensityMinimum,@PressureMaximum,@PressureMinimum,@VolumetricFlowMaximum,@VolumetricFlowMinimum,@VolumeMaximum,@VolumeMinimum,@MassMaximum,@MassMinimum,@VelocityMaximum,@VelocityMinimum,@MassFlowMaximum,@MassFlowMinimum,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PointTemplateGuid,@SiteGuid,@PointTemplateTypeApplicationStringGuid,@ProfileImageGuid,@DefaultDrawingGuid,@PointCommandStatus,@DeviceAlarmMaps,@PointLogicScript,@Version)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPointTemplate] WHERE PointTemplateGuid = @PointTemplateGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
