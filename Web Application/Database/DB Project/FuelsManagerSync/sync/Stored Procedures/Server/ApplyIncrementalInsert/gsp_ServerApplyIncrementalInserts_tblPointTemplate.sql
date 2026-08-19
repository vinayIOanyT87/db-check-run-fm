-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplate
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblPointTemplate]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblPointTemplate varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblPointTemplate] AS existingData
        USING (SELECT @ID 'ID',@Description 'Description',@Standard 'Standard',@ExecutionInterval 'ExecutionInterval',@LevelUnitIndex 'LevelUnitIndex',@TemperatureUnitIndex 'TemperatureUnitIndex',@DensityUnitIndex 'DensityUnitIndex',@PressureUnitIndex 'PressureUnitIndex',@FlowUnitIndex 'FlowUnitIndex',@VolumeUnitIndex 'VolumeUnitIndex',@MassUnitIndex 'MassUnitIndex',@VelocityUnitIndex 'VelocityUnitIndex',@MassFlowUnitIndex 'MassFlowUnitIndex',@LevelDecimalPlaces 'LevelDecimalPlaces',@TemperatureDecimalPlaces 'TemperatureDecimalPlaces',@DensityDecimalPlaces 'DensityDecimalPlaces',@PressureDecimalPlaces 'PressureDecimalPlaces',@FlowDecimalPlaces 'FlowDecimalPlaces',@VolumeDecimalPlaces 'VolumeDecimalPlaces',@MassDecimalPlaces 'MassDecimalPlaces',@VelocityDecimalPlaces 'VelocityDecimalPlaces',@MassFlowDecimalPlaces 'MassFlowDecimalPlaces',@LevelMaximum 'LevelMaximum',@LevelMinimum 'LevelMinimum',@TemperatureMaximum 'TemperatureMaximum',@TemperatureMinimum 'TemperatureMinimum',@DensityMaximum 'DensityMaximum',@DensityMinimum 'DensityMinimum',@PressureMaximum 'PressureMaximum',@PressureMinimum 'PressureMinimum',@VolumetricFlowMaximum 'VolumetricFlowMaximum',@VolumetricFlowMinimum 'VolumetricFlowMinimum',@VolumeMaximum 'VolumeMaximum',@VolumeMinimum 'VolumeMinimum',@MassMaximum 'MassMaximum',@MassMinimum 'MassMinimum',@VelocityMaximum 'VelocityMaximum',@VelocityMinimum 'VelocityMinimum',@MassFlowMaximum 'MassFlowMaximum',@MassFlowMinimum 'MassFlowMinimum',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PointTemplateGuid 'PointTemplateGuid',@SiteGuid 'SiteGuid',@PointTemplateTypeApplicationStringGuid 'PointTemplateTypeApplicationStringGuid',@ProfileImageGuid 'ProfileImageGuid',@DefaultDrawingGuid 'DefaultDrawingGuid',@PointCommandStatus 'PointCommandStatus',@DeviceAlarmMaps 'DeviceAlarmMaps',@PointLogicScript 'PointLogicScript',@Version 'Version'
                ) AS remoteChanges ([ID],[Description],[Standard],[ExecutionInterval],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[VelocityUnitIndex],[MassFlowUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[VelocityDecimalPlaces],[MassFlowDecimalPlaces],[LevelMaximum],[LevelMinimum],[TemperatureMaximum],[TemperatureMinimum],[DensityMaximum],[DensityMinimum],[PressureMaximum],[PressureMinimum],[VolumetricFlowMaximum],[VolumetricFlowMinimum],[VolumeMaximum],[VolumeMinimum],[MassMaximum],[MassMinimum],[VelocityMaximum],[VelocityMinimum],[MassFlowMaximum],[MassFlowMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateGuid],[SiteGuid],[PointTemplateTypeApplicationStringGuid],[ProfileImageGuid],[DefaultDrawingGuid],[PointCommandStatus],[DeviceAlarmMaps],[PointLogicScript],[Version])
        ON (existingData.[PointTemplateGuid] = remoteChanges.[PointTemplateGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[Description] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[Description] ELSE remoteChanges.[Description] END
                       ,[Standard] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Standard'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[Standard] ELSE remoteChanges.[Standard] END
                       ,[ExecutionInterval] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExecutionInterval'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[ExecutionInterval] ELSE remoteChanges.[ExecutionInterval] END
                       ,[LevelUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[LevelUnitIndex] ELSE remoteChanges.[LevelUnitIndex] END
                       ,[TemperatureUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[TemperatureUnitIndex] ELSE remoteChanges.[TemperatureUnitIndex] END
                       ,[DensityUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[DensityUnitIndex] ELSE remoteChanges.[DensityUnitIndex] END
                       ,[PressureUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[PressureUnitIndex] ELSE remoteChanges.[PressureUnitIndex] END
                       ,[FlowUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[FlowUnitIndex] ELSE remoteChanges.[FlowUnitIndex] END
                       ,[VolumeUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VolumeUnitIndex] ELSE remoteChanges.[VolumeUnitIndex] END
                       ,[MassUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[MassUnitIndex] ELSE remoteChanges.[MassUnitIndex] END
                       ,[VelocityUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VelocityUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VelocityUnitIndex] ELSE remoteChanges.[VelocityUnitIndex] END
                       ,[MassFlowUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassFlowUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[MassFlowUnitIndex] ELSE remoteChanges.[MassFlowUnitIndex] END
                       ,[LevelDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[LevelDecimalPlaces] ELSE remoteChanges.[LevelDecimalPlaces] END
                       ,[TemperatureDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[TemperatureDecimalPlaces] ELSE remoteChanges.[TemperatureDecimalPlaces] END
                       ,[DensityDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[DensityDecimalPlaces] ELSE remoteChanges.[DensityDecimalPlaces] END
                       ,[PressureDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[PressureDecimalPlaces] ELSE remoteChanges.[PressureDecimalPlaces] END
                       ,[FlowDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[FlowDecimalPlaces] ELSE remoteChanges.[FlowDecimalPlaces] END
                       ,[VolumeDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VolumeDecimalPlaces] ELSE remoteChanges.[VolumeDecimalPlaces] END
                       ,[MassDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[MassDecimalPlaces] ELSE remoteChanges.[MassDecimalPlaces] END
                       ,[VelocityDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VelocityDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VelocityDecimalPlaces] ELSE remoteChanges.[VelocityDecimalPlaces] END
                       ,[MassFlowDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassFlowDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[MassFlowDecimalPlaces] ELSE remoteChanges.[MassFlowDecimalPlaces] END
                       ,[LevelMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[LevelMaximum] ELSE remoteChanges.[LevelMaximum] END
                       ,[LevelMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[LevelMinimum] ELSE remoteChanges.[LevelMinimum] END
                       ,[TemperatureMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[TemperatureMaximum] ELSE remoteChanges.[TemperatureMaximum] END
                       ,[TemperatureMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[TemperatureMinimum] ELSE remoteChanges.[TemperatureMinimum] END
                       ,[DensityMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[DensityMaximum] ELSE remoteChanges.[DensityMaximum] END
                       ,[DensityMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[DensityMinimum] ELSE remoteChanges.[DensityMinimum] END
                       ,[PressureMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[PressureMaximum] ELSE remoteChanges.[PressureMaximum] END
                       ,[PressureMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[PressureMinimum] ELSE remoteChanges.[PressureMinimum] END
                       ,[VolumetricFlowMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumetricFlowMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VolumetricFlowMaximum] ELSE remoteChanges.[VolumetricFlowMaximum] END
                       ,[VolumetricFlowMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumetricFlowMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VolumetricFlowMinimum] ELSE remoteChanges.[VolumetricFlowMinimum] END
                       ,[VolumeMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VolumeMaximum] ELSE remoteChanges.[VolumeMaximum] END
                       ,[VolumeMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VolumeMinimum] ELSE remoteChanges.[VolumeMinimum] END
                       ,[MassMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[MassMaximum] ELSE remoteChanges.[MassMaximum] END
                       ,[MassMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[MassMinimum] ELSE remoteChanges.[MassMinimum] END
                       ,[VelocityMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VelocityMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VelocityMaximum] ELSE remoteChanges.[VelocityMaximum] END
                       ,[VelocityMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VelocityMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[VelocityMinimum] ELSE remoteChanges.[VelocityMinimum] END
                       ,[MassFlowMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassFlowMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[MassFlowMaximum] ELSE remoteChanges.[MassFlowMaximum] END
                       ,[MassFlowMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassFlowMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[MassFlowMinimum] ELSE remoteChanges.[MassFlowMinimum] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[PointTemplateTypeApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointTemplateTypeApplicationStringGuid'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[PointTemplateTypeApplicationStringGuid] ELSE remoteChanges.[PointTemplateTypeApplicationStringGuid] END
                       ,[ProfileImageGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProfileImageGuid'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[ProfileImageGuid] ELSE remoteChanges.[ProfileImageGuid] END
                       ,[DefaultDrawingGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DefaultDrawingGuid'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[DefaultDrawingGuid] ELSE remoteChanges.[DefaultDrawingGuid] END
                       ,[PointCommandStatus] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointCommandStatus'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[PointCommandStatus] ELSE remoteChanges.[PointCommandStatus] END
                       ,[DeviceAlarmMaps] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeviceAlarmMaps'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[DeviceAlarmMaps] ELSE remoteChanges.[DeviceAlarmMaps] END
                       ,[PointLogicScript] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointLogicScript'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[PointLogicScript] ELSE remoteChanges.[PointLogicScript] END
                       ,[Version] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Version'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN existingData.[Version] ELSE remoteChanges.[Version] END

        WHEN NOT MATCHED THEN
            INSERT ([ID],[Description],[Standard],[ExecutionInterval],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[VelocityUnitIndex],[MassFlowUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[VelocityDecimalPlaces],[MassFlowDecimalPlaces],[LevelMaximum],[LevelMinimum],[TemperatureMaximum],[TemperatureMinimum],[DensityMaximum],[DensityMinimum],[PressureMaximum],[PressureMinimum],[VolumetricFlowMaximum],[VolumetricFlowMinimum],[VolumeMaximum],[VolumeMinimum],[MassMaximum],[MassMinimum],[VelocityMaximum],[VelocityMinimum],[MassFlowMaximum],[MassFlowMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateGuid],[SiteGuid],[PointTemplateTypeApplicationStringGuid],[ProfileImageGuid],[DefaultDrawingGuid],[PointCommandStatus],[DeviceAlarmMaps],[PointLogicScript],[Version])
                VALUES (@ID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @Description END),@Standard,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExecutionInterval'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @ExecutionInterval END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @LevelUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @TemperatureUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @DensityUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @PressureUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @FlowUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VolumeUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @MassUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VelocityUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VelocityUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassFlowUnitIndex'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @MassFlowUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @LevelDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @TemperatureDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @DensityDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @PressureDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @FlowDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VolumeDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @MassDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VelocityDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VelocityDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassFlowDecimalPlaces'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @MassFlowDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @LevelMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @LevelMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @TemperatureMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @TemperatureMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @DensityMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @DensityMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @PressureMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @PressureMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumetricFlowMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VolumetricFlowMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumetricFlowMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VolumetricFlowMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VolumeMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VolumeMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @MassMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @MassMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VelocityMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VelocityMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VelocityMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @VelocityMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassFlowMaximum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @MassFlowMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassFlowMinimum'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @MassFlowMinimum END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PointTemplateGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointTemplateTypeApplicationStringGuid'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @PointTemplateTypeApplicationStringGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProfileImageGuid'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @ProfileImageGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DefaultDrawingGuid'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @DefaultDrawingGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointCommandStatus'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @PointCommandStatus END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeviceAlarmMaps'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @DeviceAlarmMaps END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointLogicScript'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @PointLogicScript END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Version'), @sync_supported_columns_tblPointTemplate)) WHEN 0 THEN NULL ELSE @Version END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
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
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

