DECLARE @StandardNodeSiteGuid UNIQUEIDENTIFIER  = '00000000-0000-0000-0000-000000000001'

-- Strap Table is the first Standard Module Developed, for Single Site System Modules are ownership changed to single site
IF EXISTS (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid)
BEGIN
	SET @StandardNodeSiteGuid =  (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid);
END 



DECLARE @StandardNodeTemplateGuid UNIQUEIDENTIFIER = '42EDBDBD-C8FC-4B66-BB36-7EC0C969E378'
DECLARE @StandardNodeProfileImageGuid UNIQUEIDENTIFIER = (SELECT PictureGuid FROM dbo.tblPictures WHERE Id = 'Dial Template')


MERGE dbo.tblPointTemplate AS Target
USING 
( SELECT 'Standard Node' as [ID],
				'' as [Description],
				1 as [Standard],
				NULL as [ExecutionInterval],
				27 as [LevelUnitIndex] ,
				2 as [TemperatureUnitIndex],
				191 as [DensityUnitIndex],
				73 as [PressureUnitIndex] ,
				109 as [FlowUnitIndex],
				46 as [VolumeUnitIndex],
				64 as [MassUnitIndex],
				162 as [VelocityUnitIndex],
				132 as [MassFlowUnitIndex],
				0 as [LevelDecimalPlaces],
				2 as [TemperatureDecimalPlaces],
				2 as [DensityDecimalPlaces],
				2 as [PressureDecimalPlaces],
				2 as [FlowDecimalPlaces],
				2 as [VolumeDecimalPlaces],
				2 as [MassDecimalPlaces],
				2 as [VelocityDecimalPlaces],
				2 as [MassFlowDecimalPlaces],
				40 as [LevelMaximum],
				0 as [LevelMinimum],
				300.0 as [TemperatureMaximum],
				-300.0 as [TemperatureMinimum],
				100 as [DensityMaximum],
				0 as [DensityMinimum],
				30.00 as [PressureMaximum],
				0 as [PressureMinimum],
				1000.00 as [VolumetricFlowMaximum],
				-1000.00 as [VolumetricFlowMinimum],
				10000.00 as [VolumeMaximum],
				0 as [VolumeMinimum],
				10000000 as [MassMaximum],
				0 as [MassMinimum],
				10 as [VelocityMaximum],
				-10 as [VelocityMinimum],
				3000 as [MassFlowMaximum],
				-3000 as [MassFlowMinimum],
				@StandardNodeTemplateGuid as [PointTemplateGuid],
				@StandardNodeSiteGuid as [SiteGuid],
				@StandardNodeProfileImageGuid as [ProfileImageGuid],
				NULL as [DefaultDrawingGuid],
				'<PointCommandStatus xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <CommandStatusLists>
	<PointCommandStatusList>
	  <CommandStatusListGuid>7e5b9ce8-a66c-fa23-7fd9-8eadaa24c991</CommandStatusListGuid>
	  <ID>Reset</ID>
	  <CommandStatusList>
		<CommandStatusElement>
		  <Key>Reset</Key>
		  <Value>135</Value>
		</CommandStatusElement>
	  </CommandStatusList>
	</PointCommandStatusList>
  </CommandStatusLists>
</PointCommandStatus>' as [PointCommandStatus],
				NULL as [DeviceAlarmMaps],
				'2024-10-28' as [CreatedDate],
				'Administrator' as [CreatedBy],
				'2024-10-28' as [UpdatedDate],
				'Administrator' as [UpdatedBy]) 
AS Source
ON (Target.PointTemplateGuid = Source.PointTemplateGuid)
WHEN MATCHED THEN
	UPDATE SET		target.[ID] = source.[ID],
						target.[UpdatedDate]		= SYSDATETIMEOFFSET(),
						target.[UpdatedBy]  = source.[UpdatedBy],
						target.[PointCommandStatus] = source.[PointCommandStatus]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID], [Description], [Standard], [ExecutionInterval], [LevelUnitIndex], [TemperatureUnitIndex], [DensityUnitIndex], [PressureUnitIndex], [FlowUnitIndex],
				[VolumeUnitIndex], [MassUnitIndex], [VelocityUnitIndex], [MassFlowUnitIndex], [LevelDecimalPlaces], [TemperatureDecimalPlaces], [DensityDecimalPlaces],
				[PressureDecimalPlaces], [FlowDecimalPlaces], [VolumeDecimalPlaces], [MassDecimalPlaces], [VelocityDecimalPlaces], [MassFlowDecimalPlaces], [LevelMaximum],
				[LevelMinimum], [TemperatureMaximum], [TemperatureMinimum], [DensityMaximum], [DensityMinimum], [PressureMaximum], [PressureMinimum], [VolumetricFlowMaximum],
				[VolumetricFlowMinimum], [VolumeMaximum], [VolumeMinimum], [MassMaximum], [MassMinimum], [VelocityMaximum], [VelocityMinimum], [MassFlowMaximum],
				[MassFlowMinimum], [PointTemplateGuid], [SiteGuid], [ProfileImageGuid], [DefaultDrawingGuid], [PointCommandStatus], [DeviceAlarmMaps], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
	VALUES (Source.[ID], Source.[Description], Source.[Standard], Source.[ExecutionInterval], Source.[LevelUnitIndex], Source.[TemperatureUnitIndex], Source.[DensityUnitIndex], Source.[PressureUnitIndex], Source.[FlowUnitIndex],
				Source.[VolumeUnitIndex], Source.[MassUnitIndex], Source.[VelocityUnitIndex], Source.[MassFlowUnitIndex], Source.[LevelDecimalPlaces], Source.[TemperatureDecimalPlaces], Source.[DensityDecimalPlaces],
				Source.[PressureDecimalPlaces], Source.[FlowDecimalPlaces], Source.[VolumeDecimalPlaces], Source.[MassDecimalPlaces], Source.[VelocityDecimalPlaces], Source.[MassFlowDecimalPlaces], Source.[LevelMaximum],
				Source.[LevelMinimum], Source.[TemperatureMaximum], Source.[TemperatureMinimum], Source.[DensityMaximum], Source.[DensityMinimum], Source.[PressureMaximum], Source.[PressureMinimum], Source.[VolumetricFlowMaximum],
				Source.[VolumetricFlowMinimum], Source.[VolumeMaximum], Source.[VolumeMinimum], Source.[MassMaximum], Source.[MassMinimum], Source.[VelocityMaximum], Source.[VelocityMinimum], Source.[MassFlowMaximum],
				Source.[MassFlowMinimum], Source.[PointTemplateGuid], Source.[SiteGuid], Source.[ProfileImageGuid], Source.[DefaultDrawingGuid], Source.[PointCommandStatus], Source.[DeviceAlarmMaps], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy]);


IF ((SELECT COUNT(*) FROM tblPointTemplate WHERE ID = 'Standard Node') = 1)
BEGIN
	DECLARE @StandardNodePointTypeGuid UNIQUEIDENTIFIER
	SELECT @StandardNodePointTypeGuid = ApplicationStringGuid FROM tblApplicationString WHERE ID = 'System'

	UPDATE tblPointTemplate SET PointTemplateTypeApplicationStringGuid = @StandardNodePointTypeGuid WHERE ID = 'Standard Node' AND PointTemplateTypeApplicationStringGuid IS NULL
END


--Create Tags

DECLARE @StandardNodeTransferModeTagGuid UNIQUEIDENTIFIER = 'DA354855-777E-47C8-B932-4BD8C1F4BA8C'
DECLARE @StandardNodeTransferStatusTagGuid UNIQUEIDENTIFIER = 'F55A77AE-EB91-4FAA-B5F0-6BE8F780D741'
DECLARE @StandardNodeTransferStartTimeTagGuid UNIQUEIDENTIFIER = 'C4ADC176-6E56-46B3-A332-248C8A79B2BC'
DECLARE @StandardNodeTransferStopTimeTagGuid UNIQUEIDENTIFIER = 'E93A7E6F-9C32-4FA7-B706-726CF4063C45'


MERGE dbo.tblPointTemplateTag AS Target
USING 
(	SELECT 'Transfer Mode' as [ID], 
						16 as [EngineeringUnitsType],
						255 as [EngineeringUnitsIndex],
						0 as [DecimalPlaces],
						255 as [ServerEngineeringUnitsIndex],
						'FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode' as [ValueType],
						'<NodeTransferMode>Inactive</NodeTransferMode>' as [Value],
						0.00 as [Maximum],
						0.00 as [Minimum],
						1 as [PointTagInputOutputTypeIndex],
						0 as [Input],
						0 as [AlarmStatus],
						1 as [ApplyPointTemplateEngineeringUnits],
						1 as [ApplyPointTemplateDecimalPlaces],
						1 as [ApplyPointTemplateMaximum],
						1 as [ApplyPointTemplateMinimum],
						@StandardNodeTransferModeTagGuid as [PointTemplateTagGuid],
						@StandardNodeTemplateGuid as [PointTemplateGuid],
						@TransferModeWellKnownGuid as [WellKnownIdentityGuid],
						1 as [AlarmsEnabled],
						1 as [InhibitInputOutputTypeConfiguration],
						1 as [InhibitOverride],
						1 as [Module],
						0 as [Archived],
						'2024-10-30' as [CreatedDate],
						'Administrator' as [CreatedBy],
						'2024-10-30' as [UpdatedDate],
						'Administrator' as [UpdatedBy]	
		UNION ALL
		SELECT 'Transfer Status' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.NodeTransferStatus','<NodeTransferStatus>Inactive</NodeTransferStatus>',0.0,0.0,2,1,0,1,1,1,1,@StandardNodeTransferStatusTagGuid,@StandardNodeTemplateGuid,@TransferStatusWellKnownGuid,1,1,1,1,0,'2024-10-30','Administrator','2024-10-30','Administrator'
		UNION ALL
		SELECT 'Transfer Start Time' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@StandardNodeTransferStartTimeTagGuid ,@StandardNodeTemplateGuid,@TransferStartTimeWellKnownGuid,1,1,0,1,1,'2024-10-30','Administrator','2024-10-30','Administrator'
		UNION ALL
		SELECT 'Transfer Stop Time' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@StandardNodeTransferStopTimeTagGuid ,@StandardNodeTemplateGuid,@TransferStopTimeWellKnownGuid,1,1,1,1,1,'2024-10-30','Administrator','2024-10-30','Administrator'
) 
AS Source
ON (Target.[PointTemplateGuid] = Source.[PointTemplateGuid] AND Target.PointTemplateTagGuid = Source.PointTemplateTagGuid)
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
						target.[EngineeringUnitsType] = source.[EngineeringUnitsType],
						target.[ServerEngineeringUnitsIndex] = source.[ServerEngineeringUnitsIndex],
						target.[EngineeringUnitsIndex] = (CASE
						WHEN source.[EngineeringUnitsType] = 16 THEN source.[EngineeringUnitsIndex] -- Unit type is None/No Units
						ELSE target.[EngineeringUnitsIndex] -- All other unit types
						END),
						target.[DecimalPlaces] = source.[DecimalPlaces],
						target.[ValueType] = source.[ValueType],
						target.[Value] = (CASE
						WHEN source.[ValueType] = 'FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode' THEN source.[Value]
						WHEN source.[ValueType] = 'FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode' THEN source.[Value]
						WHEN source.[ValueType] = 'FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode' THEN source.[Value]
						WHEN source.[ValueType] = 'FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses' THEN source.[Value]
						WHEN source.[ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' THEN source.[Value]
						WHEN source.[ValueType] = 'System.Int32' THEN source.[Value]
						ELSE target.Value
						END),
						target.[PointTagInputOutputTypeIndex] = source.[PointTagInputOutputTypeIndex],
						target.[Input] = source.[Input],
						target.[AlarmStatus] = source.[AlarmStatus],
						target.[ApplyPointTemplateEngineeringUnits] = source.[ApplyPointTemplateEngineeringUnits],
						target.[ApplyPointTemplateDecimalPlaces] = source.[ApplyPointTemplateDecimalPlaces],
						target.[ApplyPointTemplateMaximum] = source.[ApplyPointTemplateMaximum],
						target.[ApplyPointTemplateMinimum] = source.[ApplyPointTemplateMinimum],
						target.[PointTemplateTagGuid] = source.[PointTemplateTagGuid],
						target.[PointTemplateGuid] = source.[PointTemplateGuid],
						target.[WellKnownIdentityGuid] = source.[WellKnownIdentityGuid],
						target.[InhibitInputOutputTypeConfiguration] = source.[InhibitInputOutputTypeConfiguration],
						target.[InhibitOverride] = source.[InhibitOverride],
						target.[Module] = source.[Module],
						target.[Archived] = source.[Archived],
						target.[UpdatedDate] = SYSDATETIMEOFFSET(),
						target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],
		[Value],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointTemplateEngineeringUnits],
		[ApplyPointTemplateDecimalPlaces],[ApplyPointTemplateMaximum],[ApplyPointTemplateMinimum],[PointTemplateTagGuid],
		[PointTemplateGuid],[WellKnownIdentityGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Module],[Archived],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
	VALUES (Source.[ID],Source.[EngineeringUnitsType],Source.[EngineeringUnitsIndex],Source.[DecimalPlaces],Source.[ServerEngineeringUnitsIndex],Source.[ValueType],
		Source.[Value],Source.[Maximum],Source.[Minimum],Source.[PointTagInputOutputTypeIndex],Source.[Input],Source.[AlarmStatus],Source.[ApplyPointTemplateEngineeringUnits],
		Source.[ApplyPointTemplateDecimalPlaces],Source.[ApplyPointTemplateMaximum],Source.[ApplyPointTemplateMinimum],Source.[PointTemplateTagGuid],
		Source.[PointTemplateGuid],Source.[WellKnownIdentityGuid],Source.[AlarmsEnabled],Source.[InhibitInputOutputTypeConfiguration],Source.[InhibitOverride],Source.[Module],Source.[Archived],Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy]);





DECLARE @NodeTransferModuleToPointTemplateGuid UNIQUEIDENTIFIER = '31931E2D-29A5-4D5C-A921-BE4CE9F3C6B6'

DECLARE @NodeTransferModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>da354855-777e-47c8-b932-4bd8c1f4ba8c</TagGuid>
      <ModuleParameter>TransferMode</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>f55a77ae-eb91-4faa-b5f0-6be8f780d741</TagGuid>
      <ModuleParameter>TransferStatus</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c4adc176-6e56-46b3-a332-248c8a79b2bc</TagGuid>
      <ModuleParameter>TransferStartTime</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>e93a7e6f-9c32-4fa7-b706-726cf4063c45</TagGuid>
      <ModuleParameter>TransferStopTime</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules />
</ModuleToPointTemplateData>'


MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Node Transfer' as [ID],
1 as [Order],
@NodeTransferModuleToPointTemplateData AS [ModuleToPointTemplateData],
@NodeTransferModuleToPointTemplateGuid as [ModuleToPointTemplateGuid],
@StandardNodeTemplateGuid as [PointTemplateGuid] ,
@NodeTransferModuleGuid as [ModuleGuid],
'2024-10-30' as [CreatedDate] ,
'Administrator' as [CreatedBy],
'2024-10-30' as [UpdatedDate],
'Administrator' as [UpdatedBy]
) 
AS Source
ON (Target.[ModuleToPointTemplateGuid] = Source.[ModuleToPointTemplateGuid] AND Target.[PointTemplateGuid] = Source.[PointTemplateGuid])
WHEN MATCHED THEN
	UPDATE SET 
						target.[ID] = source.[ID],
						target.[Order] = source.[Order],
						target.[ModuleToPointTemplateData] = source.[ModuleToPointTemplateData],
						target.[ModuleToPointTemplateGuid] = source.[ModuleToPointTemplateGuid],
						target.[PointTemplateGuid] = source.[PointTemplateGuid],
						target.[ModuleGuid] = source.[ModuleGuid],
						target.[UpdatedDate] = SYSDATETIMEOFFSET(),
						target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[Order],[ModuleToPointTemplateData],[ModuleToPointTemplateGuid],[PointTemplateGuid],[ModuleGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
	VALUES (source.[ID],source.[Order],source.[ModuleToPointTemplateData],source.[ModuleToPointTemplateGuid],source.[PointTemplateGuid],source.[ModuleGuid],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy]);


--Entity Assignment
if(0 = (SELECT COUNT(*) FROM map.tblEntityPointTemplateToSite WHERE PointTemplateGuid = @StandardNodeTemplateGuid AND SiteGuid = @StandardNodeSiteGuid))
BEGIN
		INSERT INTO map.tblEntityPointTemplateToSite ([PointTemplateToSiteGuid],[PointTemplateGuid],[SiteGuid],[AssignedFromSiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('40D1E65B-70D9-467B-8675-C702AC702304',@StandardNodeTemplateGuid,@StandardNodeSiteGuid,@StandardNodeSiteGuid,'2024-10-30','Administrator','2024-10-30','Administrator')
END 
