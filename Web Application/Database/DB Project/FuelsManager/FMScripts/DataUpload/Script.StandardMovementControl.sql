DECLARE @StandardMovementControlSiteGuid UNIQUEIDENTIFIER  = '00000000-0000-0000-0000-000000000001'

-- Strap Table is the first Standard Module Developed, for Single Site System Modules are ownership changed to single site
IF EXISTS (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid)
BEGIN
	SET @StandardMovementControlSiteGuid =  (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid);
END 


DECLARE @StandardMovementControlTemplateGuid UNIQUEIDENTIFIER = '03E2911F-3195-4BEF-98AB-E7292D4B5B7F'
DECLARE @StandardMovementControlProfileImageGuid UNIQUEIDENTIFIER = (SELECT PictureGuid FROM dbo.tblPictures WHERE Id = 'Blank Template')

MERGE dbo.tblPointTemplate AS Target
USING 
( SELECT 'Standard Movement Control' as [ID],
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
				@StandardMovementControlTemplateGuid as [PointTemplateGuid],
				@StandardMovementControlSiteGuid as [SiteGuid],
				@StandardMovementControlProfileImageGuid as [ProfileImageGuid],
				NULL as [DefaultDrawingGuid],
				NULL as [PointCommandStatus],
				NULL as [DeviceAlarmMaps],
				'2022-06-22' as [CreatedDate],
				'Administrator' as [CreatedBy],
				'2022-06-22' as [UpdatedDate],
				'Administrator' as [UpdatedBy]) 
AS Source
ON (Target.PointTemplateGuid = Source.PointTemplateGuid)
WHEN MATCHED THEN
	UPDATE SET		target.[ID] = source.[ID],
						target.[UpdatedDate]		= SYSDATETIMEOFFSET(),
						target.[UpdatedBy]  = source.[UpdatedBy]
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

IF ((SELECT COUNT(*) FROM tblPointTemplate WHERE ID = 'Standard Movement Control') = 1)
BEGIN
	DECLARE @StandardMovementControlPointTypeGuid UNIQUEIDENTIFIER = (SELECT ApplicationStringGuid FROM tblApplicationString WHERE ID = 'Movement Control')

	UPDATE tblPointTemplate SET PointTemplateTypeApplicationStringGuid = @StandardMovementControlPointTypeGuid WHERE ID = 'Standard Movement Control' AND PointTemplateTypeApplicationStringGuid IS NULL
END


--Create Tags

DECLARE @StandardMovementControlInitiateTagGuid UNIQUEIDENTIFIER = 'BB7C5A17-E4B7-4ED1-95FD-3B6B82017415'
DECLARE @StandardMovementControlStopTagGuid UNIQUEIDENTIFIER = '8884B142-42EA-45A0-B2CF-87E5E4D8119E'
DECLARE @StandardMovementControlMovementIdentifierTagGuid UNIQUEIDENTIFIER = '728F8010-A5F5-4500-A35F-4D919DBB4F73'

MERGE dbo.tblPointTemplateTag AS Target
USING 
(  SELECT 'Initiate' as [ID], 
						16 as [EngineeringUnitsType],
						255 as [EngineeringUnitsIndex],
						0 as [DecimalPlaces],
						255 as [ServerEngineeringUnitsIndex],
						'System.Boolean' as [ValueType],
						NULL as [Value],
						1.00 as [Maximum],
						0 as [Minimum],
						3 as [PointTagInputOutputTypeIndex],
						1 as [Input],
						0 as [AlarmStatus],
						0 as [ApplyPointTemplateEngineeringUnits],
						0 as [ApplyPointTemplateDecimalPlaces],
						0 as [ApplyPointTemplateMaximum],
						0 as [ApplyPointTemplateMinimum],
						@StandardMovementControlInitiateTagGuid as [PointTemplateTagGuid],
						@StandardMovementControlTemplateGuid as [PointTemplateGuid],
						null as [WellKnownIdentityGuid],
						0 as [AlarmsEnabled],
						0 as [InhibitInputOutputTypeConfiguration],
						0 as [InhibitOverride],
						1 as [Module],
                  0 as [Archived],
						'2023-06-19' as [CreatedDate],
						'Administrator' as [CreatedBy],
						'2023-06-19' as [UpdatedDate],
						'Administrator' as [UpdatedBy]	
		UNION ALL
		SELECT 'Stop',	16, 255, 0, 255, 'System.Boolean', NULL, 1.00, 0.0, 3, 1, 0, 0, 0, 0, 0, @StandardMovementControlStopTagGuid, @StandardMovementControlTemplateGuid, null, 0, 0, 0, 1, 0, '2023-06-19', 'Administrator', '2023-06-19', 'Administrator'
		UNION ALL
		SELECT 'Movement Identifier' , 16, 255, 0, 255,'System.String',NULL,0.0,0,5,1,0,0,0,0,0,@StandardMovementControlMovementIdentifierTagGuid ,@StandardMovementControlTemplateGuid,@MovementControlIdentifierWellKnownGuid,0,1,0,1,0,'2023-06-19','Administrator','2023-06-19','Administrator'

) 
AS Source
ON (Target.[PointTemplateGuid] = Source.[PointTemplateGuid] AND Target.PointTemplateTagGuid = Source.PointTemplateTagGuid)
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
						target.[EngineeringUnitsType] = source.[EngineeringUnitsType],
						target.[DecimalPlaces] = source.[DecimalPlaces],
						target.[ServerEngineeringUnitsIndex] = source.[ServerEngineeringUnitsIndex],
						target.[ValueType] = source.[ValueType],
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


--Create mapping of Movement Control Module to Point Template


DECLARE @MovementControlModuleToPointTemplateGuid UNIQUEIDENTIFIER = '5C6BD301-FB7D-43D8-BC4C-DAA381EAB1C0'

DECLARE @MovementControlModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>bb7c5a17-e4b7-4ed1-95fd-3b6b82017415</TagGuid>
      <ModuleParameter>Initiate</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8884b142-42ea-45a0-b2cf-87e5e4d8119e</TagGuid>
      <ModuleParameter>Stop</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>728f8010-a5f5-4500-a35f-4d919dbb4f73</TagGuid>
      <ModuleParameter>MovementIdentifier</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
  </PropertyToModules>
</ModuleToPointTemplateData>'


MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Movement Control' as [ID],
1 as [Order],
@MovementControlModuleToPointTemplateData AS [ModuleToPointTemplateData],
@MovementControlModuleToPointTemplateGuid as [ModuleToPointTemplateGuid],
@StandardMovementControlTemplateGuid as [PointTemplateGuid] ,
@MovementControlModuleGuid as [ModuleGuid],
'2023-06-20' as [CreatedDate] ,
'Administrator' as [CreatedBy],
'2023-06-20' as [UpdatedDate],
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
if(0 = (SELECT COUNT(*) FROM map.tblEntityPointTemplateToSite WHERE PointTemplateGuid = @StandardMovementControlTemplateGuid AND SiteGuid = @StandardMovementControlSiteGuid))
BEGIN
		INSERT INTO map.tblEntityPointTemplateToSite ([PointTemplateToSiteGuid],[PointTemplateGuid],[SiteGuid],[AssignedFromSiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('F6C2CBF8-2B23-4F74-861A-6539C7266D8A',@StandardMovementControlTemplateGuid,@StandardMovementControlSiteGuid,@StandardMovementControlSiteGuid,'2023-06-20','Administrator','2023-06-20','Administrator')
END 

