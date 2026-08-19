-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPoint
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblPoint]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(30),
@Description nvarchar(50),
@Enabled bit,
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
@PointGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@PointTemplateGuid uniqueidentifier,
@ProfileImageGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@Notes nvarchar(255),
@OverrideDefaultDrawingGuid uniqueidentifier,
@PointTemplateVersion int,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblPoint] AS existingData
        USING (SELECT @ID 'ID',@Description 'Description',@Enabled 'Enabled',@Standard 'Standard',@ExecutionInterval 'ExecutionInterval',@LevelUnitIndex 'LevelUnitIndex',@TemperatureUnitIndex 'TemperatureUnitIndex',@DensityUnitIndex 'DensityUnitIndex',@PressureUnitIndex 'PressureUnitIndex',@FlowUnitIndex 'FlowUnitIndex',@VolumeUnitIndex 'VolumeUnitIndex',@MassUnitIndex 'MassUnitIndex',@VelocityUnitIndex 'VelocityUnitIndex',@MassFlowUnitIndex 'MassFlowUnitIndex',@LevelDecimalPlaces 'LevelDecimalPlaces',@TemperatureDecimalPlaces 'TemperatureDecimalPlaces',@DensityDecimalPlaces 'DensityDecimalPlaces',@PressureDecimalPlaces 'PressureDecimalPlaces',@FlowDecimalPlaces 'FlowDecimalPlaces',@VolumeDecimalPlaces 'VolumeDecimalPlaces',@MassDecimalPlaces 'MassDecimalPlaces',@VelocityDecimalPlaces 'VelocityDecimalPlaces',@MassFlowDecimalPlaces 'MassFlowDecimalPlaces',@LevelMaximum 'LevelMaximum',@LevelMinimum 'LevelMinimum',@TemperatureMaximum 'TemperatureMaximum',@TemperatureMinimum 'TemperatureMinimum',@DensityMaximum 'DensityMaximum',@DensityMinimum 'DensityMinimum',@PressureMaximum 'PressureMaximum',@PressureMinimum 'PressureMinimum',@VolumetricFlowMaximum 'VolumetricFlowMaximum',@VolumetricFlowMinimum 'VolumetricFlowMinimum',@VolumeMaximum 'VolumeMaximum',@VolumeMinimum 'VolumeMinimum',@MassMaximum 'MassMaximum',@MassMinimum 'MassMinimum',@VelocityMaximum 'VelocityMaximum',@VelocityMinimum 'VelocityMinimum',@MassFlowMaximum 'MassFlowMaximum',@MassFlowMinimum 'MassFlowMinimum',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PointGuid 'PointGuid',@SiteGuid 'SiteGuid',@PointTemplateGuid 'PointTemplateGuid',@ProfileImageGuid 'ProfileImageGuid',@ProductGuid 'ProductGuid',@Notes 'Notes',@OverrideDefaultDrawingGuid 'OverrideDefaultDrawingGuid',@PointTemplateVersion 'PointTemplateVersion'
                ) AS remoteChanges ([ID],[Description],[Enabled],[Standard],[ExecutionInterval],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[VelocityUnitIndex],[MassFlowUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[VelocityDecimalPlaces],[MassFlowDecimalPlaces],[LevelMaximum],[LevelMinimum],[TemperatureMaximum],[TemperatureMinimum],[DensityMaximum],[DensityMinimum],[PressureMaximum],[PressureMinimum],[VolumetricFlowMaximum],[VolumetricFlowMinimum],[VolumeMaximum],[VolumeMinimum],[MassMaximum],[MassMinimum],[VelocityMaximum],[VelocityMinimum],[MassFlowMaximum],[MassFlowMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointGuid],[SiteGuid],[PointTemplateGuid],[ProfileImageGuid],[ProductGuid],[Notes],[OverrideDefaultDrawingGuid],[PointTemplateVersion])
        ON (existingData.[PointGuid] = remoteChanges.[PointGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = remoteChanges.[ID]
                       ,[Description] = remoteChanges.[Description]
                       ,[Enabled] = remoteChanges.[Enabled]
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
                       ,[PointTemplateGuid] = remoteChanges.[PointTemplateGuid]
                       ,[ProfileImageGuid] = remoteChanges.[ProfileImageGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]
                       ,[Notes] = remoteChanges.[Notes]
                       ,[OverrideDefaultDrawingGuid] = remoteChanges.[OverrideDefaultDrawingGuid]
                       ,[PointTemplateVersion] = remoteChanges.[PointTemplateVersion]

        WHEN NOT MATCHED THEN
            INSERT ([ID],[Description],[Enabled],[Standard],[ExecutionInterval],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[VelocityUnitIndex],[MassFlowUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[VelocityDecimalPlaces],[MassFlowDecimalPlaces],[LevelMaximum],[LevelMinimum],[TemperatureMaximum],[TemperatureMinimum],[DensityMaximum],[DensityMinimum],[PressureMaximum],[PressureMinimum],[VolumetricFlowMaximum],[VolumetricFlowMinimum],[VolumeMaximum],[VolumeMinimum],[MassMaximum],[MassMinimum],[VelocityMaximum],[VelocityMinimum],[MassFlowMaximum],[MassFlowMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointGuid],[SiteGuid],[PointTemplateGuid],[ProfileImageGuid],[ProductGuid],[Notes],[OverrideDefaultDrawingGuid],[PointTemplateVersion])
                VALUES (@ID,@Description,@Enabled,@Standard,@ExecutionInterval,@LevelUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@PressureUnitIndex,@FlowUnitIndex,@VolumeUnitIndex,@MassUnitIndex,@VelocityUnitIndex,@MassFlowUnitIndex,@LevelDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@PressureDecimalPlaces,@FlowDecimalPlaces,@VolumeDecimalPlaces,@MassDecimalPlaces,@VelocityDecimalPlaces,@MassFlowDecimalPlaces,@LevelMaximum,@LevelMinimum,@TemperatureMaximum,@TemperatureMinimum,@DensityMaximum,@DensityMinimum,@PressureMaximum,@PressureMinimum,@VolumetricFlowMaximum,@VolumetricFlowMinimum,@VolumeMaximum,@VolumeMinimum,@MassMaximum,@MassMinimum,@VelocityMaximum,@VelocityMinimum,@MassFlowMaximum,@MassFlowMinimum,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PointGuid,@SiteGuid,@PointTemplateGuid,@ProfileImageGuid,@ProductGuid,@Notes,@OverrideDefaultDrawingGuid,@PointTemplateVersion)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPoint] WHERE PointGuid = @PointGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
