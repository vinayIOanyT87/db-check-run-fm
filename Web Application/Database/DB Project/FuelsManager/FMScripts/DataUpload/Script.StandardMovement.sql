DECLARE @MovementSiteGuid UNIQUEIDENTIFIER  = '00000000-0000-0000-0000-000000000001'

-- Strap Table is the first Standard Module Developed, for Single Site System Modules are ownership changed to single site
IF EXISTS (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid)
BEGIN
	SET @MovementSiteGuid =  (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid);
END 


DECLARE @MovementTemplateGuid UNIQUEIDENTIFIER = '0FE444B2-920F-4572-AC60-31171C1F4763'
DECLARE @MovementProfileImageGuid UNIQUEIDENTIFIER = (SELECT PictureGuid FROM dbo.tblPictures WHERE Id = 'Dial Template')


MERGE dbo.tblPointTemplate AS Target
USING 
( SELECT 'Standard Movement' as [ID],
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
				@MovementTemplateGuid as [PointTemplateGuid],
				@MovementSiteGuid as [SiteGuid],
				@MovementProfileImageGuid as [ProfileImageGuid],
				NULL as [DefaultDrawingGuid],
				NULL as [PointCommandStatus],
				NULL as [DeviceAlarmMaps],
				'2022-06-30' as [CreatedDate],
				'Administrator' as [CreatedBy],
				'2022-06-30' as [UpdatedDate],
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


IF ((SELECT COUNT(*) FROM tblPointTemplate WHERE ID = 'Standard Movement') = 1)
BEGIN
	DECLARE @MovementPointTypeGuid UNIQUEIDENTIFIER
	SELECT @MovementPointTypeGuid = ApplicationStringGuid FROM tblApplicationString WHERE ID = 'Movement'

	UPDATE tblPointTemplate SET PointTemplateTypeApplicationStringGuid = @MovementPointTypeGuid WHERE ID = 'Standard Movement' AND PointTemplateTypeApplicationStringGuid IS NULL
END

--Create Tags
DECLARE @MovementPercentDeviationTagGuid UNIQUEIDENTIFIER = 'A95E83BA-CDD9-43C5-81BC-3DCF8145FFA0'
DECLARE @MovementPercentDeviationHighAlarmTagGuid UNIQUEIDENTIFIER = 'E8C66F94-0808-4C47-9F25-28670D050B3D'
DECLARE @MovementPercentDeviationLowAlarmTagGuid UNIQUEIDENTIFIER = '3F87E87E-6747-4B79-8E8D-6B6732D8375D'
DECLARE @MovementPercentDeviationHighAlarmLimitTagGuid UNIQUEIDENTIFIER = '20868E89-6330-4E90-A17B-C9E886BE7DCC'
DECLARE @MovementPercentDeviationLowAlarmLimitTagGuid UNIQUEIDENTIFIER = '6D186506-76BC-4AD5-A7DC-1DCC97A3B0C4'
DECLARE @MovementCommandTagGuid UNIQUEIDENTIFIER = '45F4AF52-126A-4836-A336-6CDE6D611E3B'
DECLARE @MovementStatusTagGuid UNIQUEIDENTIFIER = '065DA402-8A0C-4CDB-B64F-83B7B4C0D3ED'
DECLARE @MovementStartTimeTagGuid UNIQUEIDENTIFIER = '1113B77F-E421-4086-B535-5C7CF3D16922'
DECLARE @MovementStopTimeTagGuid UNIQUEIDENTIFIER = '955D3D56-B476-4B9A-9C8E-88A5B0D139A8'
DECLARE @MovementInitiationCountTagGuid UNIQUEIDENTIFIER = '4DCDC163-E055-417F-9016-9BB1913E730C'
DECLARE @MovementUserData01TagGuid UNIQUEIDENTIFIER = 'D37B1DF7-8843-445E-A6AB-939B79C50439'
DECLARE @MovementUserData02TagGuid UNIQUEIDENTIFIER = '3062BF34-C316-4951-97CD-4799C6A2D2F5'
DECLARE @MovementUserData03TagGuid UNIQUEIDENTIFIER = 'A4CB1964-6E6B-47D8-98E5-305D50EF7B96'
DECLARE @MovementUserData04TagGuid UNIQUEIDENTIFIER = '5884E398-CC5D-400B-BD6C-2ECD1CF81344'
DECLARE @MovementUserData05TagGuid UNIQUEIDENTIFIER = '2861430B-D630-4766-ABA6-A2BD35ED24A8'
DECLARE @MovementUserData06TagGuid UNIQUEIDENTIFIER = '1B12CCD4-8629-4AAE-BA69-745A486679BD'
DECLARE @MovementUserData07TagGuid UNIQUEIDENTIFIER = 'A430D660-C159-4BDD-A613-F09A66E34427'
DECLARE @MovementUserData08TagGuid UNIQUEIDENTIFIER = 'DD5F9748-1066-4626-8E00-20CAAF467714'
DECLARE @MovementUserData09TagGuid UNIQUEIDENTIFIER = '9DCB6542-113D-4A2F-AAA9-E4B9698935E5'
DECLARE @MovementUserData10TagGuid UNIQUEIDENTIFIER = '6F2A00B1-6C8D-49CC-8781-61BA9D7973D2'
DECLARE @MovementHistoryWrittenTimeTagGuid UNIQUEIDENTIFIER = 'D15E46AB-741D-4533-AEB9-95C1E98C9689'
DECLARE @MovementTransferredGOVTagGuid UNIQUEIDENTIFIER = '5CAA7F26-9A2A-4E67-A8F3-694BF5E2EF6B'
DECLARE @MovementTransferredNSVTagGuid UNIQUEIDENTIFIER = 'F021C476-325D-4CF8-A59F-95B6B136A483'
DECLARE @MovementTransferTimeRemainingTagGuid UNIQUEIDENTIFIER = '009F1EB8-4EE2-4B8F-AB75-7A066C1FECA0'
DECLARE @MovementInitiateIdentifierTagGuid UNIQUEIDENTIFIER = '64BE1F86-A923-4752-9902-D5BFF4711EC1'
DECLARE @MovementStopIdentifierTagGuid UNIQUEIDENTIFIER = 'CA09FCDF-81FF-4C52-8A1E-4B1867D0DF3F'
DECLARE @MovementDiscreteAlarmTagGuid uniqueidentifier = '322E377C-1995-4AFE-A1EA-AEAAC02D5C85'
DECLARE @MovementDiscreteAlarmStatusTagGuid uniqueidentifier = '88CD15B7-8ADF-4B87-97A2-CEC65917A871'
DECLARE @MovementControlLimitTagGuid uniqueidentifier = 'F3895AF8-12A4-401B-A21D-115F851DDA2A'
DECLARE @MovementDeviationTagGuid uniqueidentifier = '9A8866D0-FE08-456B-B494-7ED408863960'




MERGE dbo.tblPointTemplateTag AS Target
USING 
(  SELECT 'Percent Deviation' as [ID], 
						15 as [EngineeringUnitsType],
						255 as [EngineeringUnitsIndex],
						2 as [DecimalPlaces],
						255 as [ServerEngineeringUnitsIndex],
						'System.Double' as [ValueType],
						NULL as [Value],
						100.00 as [Maximum],
						-100.00 as [Minimum],
						5 as [PointTagInputOutputTypeIndex],
						1 as [Input],
						0 as [AlarmStatus],
						1 as [ApplyPointTemplateEngineeringUnits],
						1 as [ApplyPointTemplateDecimalPlaces],
						1 as [ApplyPointTemplateMaximum],
						1 as [ApplyPointTemplateMinimum],
						@MovementPercentDeviationTagGuid as [PointTemplateTagGuid],
						@MovementTemplateGuid as [PointTemplateGuid],
						@MovementPercentDeviationWellKnownGuid as [WellKnownIdentityGuid],
						1 as [AlarmsEnabled],
						1 as [InhibitInputOutputTypeConfiguration],
						0 as [InhibitOverride],
						1 as [Module],
                  1 as [Archived],
						'2022-06-30' as [CreatedDate],
						'Administrator' as [CreatedBy],
						'2022-06-30' as [UpdatedDate],
						'Administrator' as [UpdatedBy]	

		-- tags for Alarms
		UNION ALL
		SELECT 'Percent Deviation High Alarm Limit' ,15,255,2,255,'System.Double','<double>10.0</double>',100.0,0,1,1,0,1,1,1,1,@MovementPercentDeviationHighAlarmLimitTagGuid,@MovementTemplateGuid,null,1,0,0,0,0,'2022-06-30','Administrator','2022-06-30','Administrator'
		UNION ALL
		SELECT 'Percent Deviation Low Alarm Limit' ,15,255,2,255,'System.Double','<double>-10.0</double>',0.00,-100.0,1,1,0,1,1,1,1,@MovementPercentDeviationLowAlarmLimitTagGuid,@MovementTemplateGuid,null,1,0,0,0,0,'2022-06-30','Administrator','2022-06-30','Administrator'
		UNION ALL
		SELECT 'Percent Deviation High Alarm' ,15,255,0,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@MovementPercentDeviationHighAlarmTagGuid,@MovementTemplateGuid,null,0,1,1,0,0,'2022-06-30','Administrator','2022-06-30','Administrator'
		UNION ALL
		SELECT 'Percent Deviation Low Alarm' ,15,255,0,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@MovementPercentDeviationLowAlarmTagGuid,@MovementTemplateGuid,null,0,1,1,0,0,'2022-06-30','Administrator','2022-06-30','Administrator'

		-- tags for Movement Module
		UNION ALL
		SELECT 'Command' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.MovementCommand','<MovementCommand>Stop</MovementCommand>',5.0,0.0,1,0,0,1,1,1,1,@MovementCommandTagGuid,@MovementTemplateGuid,@MovementCommandWellKnownGuid,1,1,0,1,0,'2022-06-30','Administrator','2022-06-30','Administrator'
		UNION ALL
		SELECT 'Status' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.MovementStatus','<MovementStatus>Inactive</MovementStatus>',4.0,0.0,2,1,0,1,1,1,1,@MovementStatusTagGuid,@MovementTemplateGuid,@MovementStatusWellKnownGuid,1,1,0,1,0,'2022-06-30','Administrator','2022-06-30','Administrator'
		UNION ALL
		SELECT 'Transfer Start Time' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@MovementStartTimeTagGuid ,@MovementTemplateGuid,@TransferStartTimeWellKnownGuid,1,1,1,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Stop Time' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@MovementStopTimeTagGuid ,@MovementTemplateGuid,@TransferStopTimeWellKnownGuid,1,1,0,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Initiation Count' ,16,255,0,255,'System.Int16',NULL,32767.0,0.0,2,1,0,1,1,1,1,@MovementInitiationCountTagGuid,@MovementTemplateGuid,@InitiationCountWellKnownGuid,1,1,1,1,0,'2023-03-23','Administrator','2023-03-23','Administrator'
		UNION ALL
		SELECT 'Movement History Written Time',16,255,0,255,'System.DateTimeOffset',NULL,0,0,5,1,0,1,1,1,1,@MovementHistoryWrittenTimeTagGuid,@MovementTemplateGuid,@MovementHistoryWrittenTimeWellKnownGuid,0,1,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Transferred GOV' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,5,1,0,1,1,0,0,@MovementTransferredGOVTagGuid ,@MovementTemplateGuid,@TransferVolumeGOVWellKnownGuid,1,1,1,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transferred NSV' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,5,1,0,1,1,0,0,@MovementTransferredNSVTagGuid ,@MovementTemplateGuid,@TransferVolumeNSVWellKnownGuid,1,1,1,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Deviation' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,5,1,0,1,1,0,0,@MovementDeviationTagGuid ,@MovementTemplateGuid,@MovementDeviationWellKnownGuid,1,1,1,0,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Time Remaining' ,16,255,0,255,'System.TimeSpan',NULL,0,0,5,1,0,1,1,1,1,@MovementTransferTimeRemainingTagGuid ,@MovementTemplateGuid,@TransferTimeRemainingWellKnownGuid,1,1,1,1,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Initiate Identifier' ,16,255,0,255,'System.String',NULL,0.0,0.0,1,0,0,1,1,1,1,@MovementInitiateIdentifierTagGuid,@MovementTemplateGuid,@MovementInitiateIdentifierWellKnownGuid,0,1,1,1,0,'2022-06-30','Administrator','2022-06-30','Administrator'
		UNION ALL
		SELECT 'Stop Identifier' ,16,255,0,255,'System.String',NULL,0.0,0.0,1,0,0,1,1,1,1,@MovementStopIdentifierTagGuid,@MovementTemplateGuid,@MovementStopIdentifierWellKnownGuid,0,1,1,1,0,'2022-06-30','Administrator','2022-06-30','Administrator'
		UNION ALL
		SELECT 'Movement Discrete Alarm',16,255,0,255,'System.Int16',NULL,1,0,2,1,0,1,1,1,1,@MovementDiscreteAlarmTagGuid ,@MovementTemplateGuid,NULL,1,1,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Movement Discrete Alarm Status',16,255,0,255,'System.String',NULL,1,0,2,1,0,1,1,1,1,@MovementDiscreteAlarmStatusTagGuid,@MovementTemplateGuid,NULL,1,1,0,0,0,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Movement Control Limit',16,255,0,255,'System.Int16','<short>1</short>',1,0,1,1,0,1,1,1,1,@MovementControlLimitTagGuid,@MovementTemplateGuid,NULL,1,1,0,0,0,'2023-04-05','Administrator','2023-04-05','Administrator'

		-- tags for User Data
		UNION ALL
		SELECT 'User Data 01' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData01TagGuid,@MovementTemplateGuid,@UserData01WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 02' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData02TagGuid,@MovementTemplateGuid,@UserData02WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 03' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData03TagGuid,@MovementTemplateGuid,@UserData03WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 04' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData04TagGuid,@MovementTemplateGuid,@UserData04WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 05' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData05TagGuid,@MovementTemplateGuid,@UserData05WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 06' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData06TagGuid,@MovementTemplateGuid,@UserData06WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 07' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData07TagGuid,@MovementTemplateGuid,@UserData07WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 08' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData08TagGuid,@MovementTemplateGuid,@UserData08WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 09' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData09TagGuid,@MovementTemplateGuid,@UserData09WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'
		UNION ALL
		SELECT 'User Data 10' ,15,255,0,255,'System.String',NULL,1000,0,1,1,0,1,1,1,1,@MovementUserData10TagGuid,@MovementTemplateGuid,@UserData10WellKnownGuid,0,1,1,0,0,'2023-03-27','Administrator','2023-03-27','Administrator'

) 
AS Source
ON (Target.[PointTemplateGuid] = Source.[PointTemplateGuid] AND Target.PointTemplateTagGuid = Source.PointTemplateTagGuid)
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
						target.[EngineeringUnitsType] = source.[EngineeringUnitsType],
						target.[DecimalPlaces] = source.[DecimalPlaces],
						target.[EngineeringUnitsIndex] = source.[EngineeringUnitsIndex],
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

-- Create Alarms

DECLARE @MovementAlarmGroupCategoryGuid UNIQUEIDENTIFIER = '512ab266-b3b8-4a29-b8d9-594795cf63ed'

DECLARE @MovementPercentDeviationHighAlarmGuid UNIQUEIDENTIFIER = 'C11FBDEA-E91E-429D-823C-478FAD2E05F4'
DECLARE @MovementPercentDeviationLowAlarmGuid UNIQUEIDENTIFIER = '60D2D25C-D5A3-47FD-AD86-B45FF3D71823'
DECLARE @MovementDiscreteAlarmGuid UNIQUEIDENTIFIER = '2B22DA37-CA9D-4A32-94C6-6FF4863BFB91'


MERGE dbo.tblAlarmTemplate AS Target
USING 
(  SELECT @MovementPercentDeviationHighAlarmGuid AS [AlarmTemplateGuid]
			,@MovementPercentDeviationTagGuid AS [InputTemplateTagGuid]
			,'Percent Deviation High Alarm' AS [ID]
			,1 AS [Enabled]
			,@MovementAlarmGroupCategoryGuid AS [AlarmCategoryApplicationStringGuid]
			,0 AS [Order]
			,'Normal' AS [NotAlarmState]
			,'Alarm Comment' AS [Comment]
			,null AS [ShelvedStartTimeStamp]
			,null AS [ShelvedEndTimeStamp]
			,0 AS [ShelvedOneShot]
			,0 AS [Suppressed]
			,'2022-06-30' as [CreatedDate]
			,'Administrator' as [CreatedBy]
			,'2022-06-30' as [UpdatedDate]
			,'Administrator' as [UpdatedBy]
			,@MovementPercentDeviationHighAlarmTagGuid AS [AlarmStateTemplateTagGuid]
			,1 AS [ExclusiveAlarm]
	UNION ALL
	SELECT @MovementPercentDeviationLowAlarmGuid,@MovementPercentDeviationTagGuid,'Percent Deviation Low Alarm',1,@MovementAlarmGroupCategoryGuid,1,'Normal','Alarm Comment',null,null,0,0,'2022-06-30','Administrator','2022-06-30','Administrator',@MovementPercentDeviationLowAlarmTagGuid,1
	UNION ALL
	SELECT @MovementDiscreteAlarmGuid,@MovementDiscreteAlarmTagGuid,'Movement Discrete Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2023-04-05','Administrator','2023-04-05','Administrator',@MovementDiscreteAlarmStatusTagGuid,1
)

AS Source
ON (Target.[AlarmTemplateGuid] = Source.[AlarmTemplateGuid])

WHEN MATCHED THEN
UPDATE SET target.[InputTemplateTagGuid] = source.[InputTemplateTagGuid]
		,target.[ID] = source.[ID]
		,target.[AlarmCategoryApplicationStringGuid] = source.[AlarmCategoryApplicationStringGuid]
		,target.[Order] = source.[Order]
		,target.[NotAlarmState] = source.[NotAlarmState]
		,target.[UpdatedDate] = SYSDATETIMEOFFSET()
		,target.[UpdatedBy] = source.[UpdatedBy]
		,target.[AlarmStateTemplateTagGuid] = source.[AlarmStateTemplateTagGuid]
		,target.[ExclusiveAlarm] = source.[ExclusiveAlarm]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([AlarmTemplateGuid]
			,[InputTemplateTagGuid]
			,[ID]
			,[Enabled]
			,[AlarmCategoryApplicationStringGuid]
			,[Order]
			,[NotAlarmState]
			,[Comment]
			,[ShelvedStartTimeStamp]
			,[ShelvedEndTimeStamp]
			,[ShelvedOneShot]
			,[Suppressed]
			,[CreatedDate]
			,[CreatedBy]
			,[UpdatedDate]
			,[UpdatedBy]
			,[AlarmStateTemplateTagGuid]
			,[ExclusiveAlarm])
	VALUES
			(source.[AlarmTemplateGuid]
			,source.[InputTemplateTagGuid]
			,source.[ID]
			,source.[Enabled]
			,source.[AlarmCategoryApplicationStringGuid]
			,source.[Order]
			,source.[NotAlarmState]
			,source.[Comment]
			,source.[ShelvedStartTimeStamp]
			,source.[ShelvedEndTimeStamp]
			,source.[ShelvedOneShot]
			,source.[Suppressed]
			,source.[CreatedDate]
			,source.[CreatedBy]
			,source.[UpdatedDate]
			,source.[UpdatedBy]
			,source.[AlarmStateTemplateTagGuid]
			,source.[ExclusiveAlarm]);

DECLARE @MovementHighLowAlarmPriorityGuid UNIQUEIDENTIFIER = 'BA35E686-5CCE-402D-982B-18D45958CCB6'
DECLARE @MovementNormalUnacknowledgedAlarmPriorityGuid UNIQUEIDENTIFIER = '5B7D7344-7D3C-4CDE-A834-B5E2C8BFE11F'


DECLARE @MovementPercentDeviationHighAlarmTestGuid UNIQUEIDENTIFIER = '4901D126-9961-4FFF-9AD2-D61921D33A8E'
DECLARE @MovementPercentDeviationLowAlarmTestGuid UNIQUEIDENTIFIER = 'E06209E9-0993-4E6E-9CA0-AE9468E674F3'
DECLARE @MovementControlTestGuid UNIQUEIDENTIFIER = '76BFDA23-F871-433A-9147-C4849FC74DCC'

MERGE dbo.tblAlarmTestTemplate AS Target
USING 
(  SELECT @MovementPercentDeviationHighAlarmTestGuid AS [AlarmTestTemplateGuid]
		,@MovementPercentDeviationHighAlarmGuid AS [AlarmTemplateGuid]
		,'High Test' AS [ID]
		,@MovementPercentDeviationHighAlarmLimitTagGuid AS [LimitTemplateTagGuid]
		,0 AS [TagField]
		,@MovementHighLowAlarmPriorityGuid AS [AlarmPriorityGuid]
		,@MovementNormalUnacknowledgedAlarmPriorityGuid as [NormalUnacknowledgedAlarmPriorityGuid]
		,1 AS [TestType]
		,-1 AS [BitMask]
		,1 AS [Enabled]
		,0 AS [Order]
		,'High Alarm' AS [AlarmState]
		,0.00 AS [Holdoff]
		,'Movement Percent Deviation High Alarm' AS [AlarmText]
		,null AS [HelpFile]
		,null AS [DrawingGuid]
		,'2022-06-30' as [CreatedDate]
		,'Administrator' as [CreatedBy]
		,'2022-06-30' as [UpdatedDate]
		,'Administrator' as [UpdatedBy]
		,0 as [BitwiseOperator]
		,0 as [TimedHoldOffInSeconds]
		UNION ALL
		SELECT @MovementPercentDeviationLowAlarmTestGuid,@MovementPercentDeviationLowAlarmGuid,'Low Test',@MovementPercentDeviationLowAlarmLimitTagGuid,0,@MovementHighLowAlarmPriorityGuid,@MovementNormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'Low Alarm',0.00,'Movement Deviation Low Alarm',null,null,'2022-06-30','Administrator','2022-06-30','Administrator',0,0
		UNION ALL
		SELECT @MovementControlTestGuid,@MovementDiscreteAlarmGuid,'Movement Control Test',@MovementControlLimitTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x01', 1)),1,0,'Movement',0.00,'Movement Control Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0

) 
AS Source
ON (Target.[AlarmTestTemplateGuid] = Source.[AlarmTestTemplateGuid])
WHEN MATCHED THEN
UPDATE SET target.[AlarmTemplateGuid] = source.[AlarmTemplateGuid]
		,target.[ID] = source.[ID]
		,target.[LimitTemplateTagGuid] = source.[LimitTemplateTagGuid]
		,target.[TagField] = source.[TagField]
		,target.[AlarmPriorityGuid] = source.[AlarmPriorityGuid]
	   ,target.[NormalUnacknowledgedAlarmPriorityGuid] = source.[NormalUnacknowledgedAlarmPriorityGuid]
		,target.[TestType] = source.[TestType]
		,target.[BitMask] = source.[BitMask]
		,target.[Order] = source.[Order]
		,target.[AlarmState] = source.[AlarmState]
		,target.[AlarmText] = source.[AlarmText]
		,target.[UpdatedDate] = SYSDATETIMEOFFSET()
		,target.[UpdatedBy] = source.[UpdatedBy]
		,target.[BitwiseOperator] = source.[BitwiseOperator]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([AlarmTestTemplateGuid]
			,[AlarmTemplateGuid]
			,[ID]
			,[LimitTemplateTagGuid]
			,[TagField]
			,[AlarmPriorityGuid]
			,[NormalUnacknowledgedAlarmPriorityGuid]
			,[TestType]
			,[BitMask]
			,[Enabled]
			,[Order]
			,[AlarmState]
			,[Holdoff]
			,[AlarmText]
			,[HelpFile]
			,[DrawingGuid]
			,[CreatedDate]
			,[CreatedBy]
			,[UpdatedDate]
			,[UpdatedBy]
			,[BitwiseOperator]
			,[TimedHoldOffInSeconds])
	VALUES
			(source.[AlarmTestTemplateGuid]
			,source.[AlarmTemplateGuid]
			,source.[ID]
			,source.[LimitTemplateTagGuid]
			,source.[TagField]
			,source.[AlarmPriorityGuid]
			,source.[NormalUnacknowledgedAlarmPriorityGuid]
			,source.[TestType]
			,source.[BitMask]
			,source.[Enabled]
			,source.[Order]
			,source.[AlarmState]
			,source.[Holdoff]
			,source.[AlarmText]
			,source.[HelpFile]
			,source.[DrawingGuid]
			,source.[CreatedDate]
			,source.[CreatedBy]
			,source.[UpdatedDate]
			,source.[UpdatedBy]
			,source.[BitwiseOperator]
			,source.[TimedHoldOffInSeconds]);

DECLARE @MovementPercentDeviationHighAlarmStatusGuid UNIQUEIDENTIFIER = 'EEFCFB0D-2D58-4347-96A7-F1F66FA8FA0E'
DECLARE @MovementPercentDeviationLowAlarmStatusGuid UNIQUEIDENTIFIER = 'EE7CBDC8-2165-4BC6-BE83-E57E7592BAE8'
DECLARE @MovementControlAlarmStatusGuid UNIQUEIDENTIFIER = '5B0EFD83-6014-48B4-97EF-C10DCDE58ABA'

MERGE dbo.tblPointTemplateTagAlarmStatus AS Target
USING 
(  SELECT @MovementPercentDeviationHighAlarmStatusGuid AS [PointTemplateTagAlarmStatusGuid],
	@MovementPercentDeviationHighAlarmTestGuid AS[AlarmTestTemplateGuid],
	1 AS [Acknowledged],
	null AS [AcknowledgedTimestamp],
	null AS [AcknowledgedBy],
	null AS [AcknowledgedComment],
	1 AS [Silenced],
	null as [SilencedTimestamp],
	null as [SilencedBy],
	0 AS [AlarmTestFailed],
	null AS [AlarmTestFailedTimestamp],
	'2022-06-30' as [CreatedDate],
	'Administrator' as [CreatedBy],
	'2022-06-30' as [UpdatedDate],
	'Administrator' as [UpdatedBy]
		UNION ALL
		SELECT @MovementPercentDeviationLowAlarmStatusGuid,@MovementPercentDeviationLowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2022-06-30','Administrator','2022-06-30','Administrator'
		UNION ALL
		SELECT @MovementControlAlarmStatusGuid,@MovementControlTestGuid,1,null,null,null,1,null,null,0,null,'2022-06-30','Administrator','2022-06-30','Administrator'

) 
AS Source
ON (Target.[PointTemplateTagAlarmStatusGuid] = Source.[PointTemplateTagAlarmStatusGuid])
WHEN MATCHED THEN
UPDATE SET target.[AlarmTestTemplateGuid] = source.[AlarmTestTemplateGuid]
		,target.[UpdatedDate] = SYSDATETIMEOFFSET()
		,target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([PointTemplateTagAlarmStatusGuid]
			,[AlarmTestTemplateGuid]
			,[Acknowledged]
			,[AcknowledgedTimestamp]
			,[AcknowledgedBy]
			,[AcknowledgedComment]
			,[Silenced]
			,[SilencedTimestamp]
			,[SilencedBy]
			,[AlarmTestFailed]
			,[AlarmTestFailedTimestamp]
			,[CreatedDate]
			,[CreatedBy]
			,[UpdatedDate]
			,[UpdatedBy])
	VALUES
			(Source.[PointTemplateTagAlarmStatusGuid]
			,Source.[AlarmTestTemplateGuid]
			,Source.[Acknowledged]
			,Source.[AcknowledgedTimestamp]
			,Source.[AcknowledgedBy]
			,Source.[AcknowledgedComment]
			,Source.[Silenced]
			,Source.[SilencedTimestamp]
			,Source.[SilencedBy]
			,Source.[AlarmTestFailed]
			,Source.[AlarmTestFailedTimestamp]
			,Source.[CreatedDate]
			,Source.[CreatedBy]
			,Source.[UpdatedDate]
			,Source.[UpdatedBy]);


--Create MovementModuleSettings Property
--Note : MovementModuleSettings Value is xml serialization of FMBusinessObjects MovementModuleSettings 
DECLARE @MovementModuleSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = 'FC861EC7-89C7-4430-ABE2-7CAA8B9FBEC1'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(
	SELECT 'Movement Settings' as [ID],
	'FMBusinessObjects.DataObjects.MovementModuleSettings' as [ValueType],
	'<MovementModuleSettings>
	  <InterlockSourceDestinationSetpoints>true</InterlockSourceDestinationSetpoints>
	  <DeleteAfterCompletion>false</DeleteAfterCompletion>
	  <OrderNumber />
	  <Comment />
	  <HandGaugeData>false</HandGaugeData>
	  <SendToAccounting>false</SendToAccounting>
	  <Ticket />
	  <Printer />
	  <UseControlTagStartStop>false</UseControlTagStartStop>
	  <ControlTagGuid>00000000-0000-0000-0000-000000000000</ControlTagGuid>
	  <StopHaltBasedOnZeroFlow>false</StopHaltBasedOnZeroFlow>
	  <StartTimeBasedOnNonZeroFlow>false</StartTimeBasedOnNonZeroFlow>
	  <ZeroFlowHoldOffTime xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />
	  <SetPendingStatus>false</SetPendingStatus>
	  <PlannedStartDateTime xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />
	  <MovementNodeDataList />
	  <DeleteAfterStop>false</DeleteAfterStop>
     <Type>Transfer</Type>
	</MovementModuleSettings>' as [Value],
	'2022-06-22' as [CreatedDate],
	'Administrator' as [CreatedBy],
	'2022-06-22' as [UpdatedDate],
	'Administrator' [UpdatedBy],
	@MovementModuleSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
	@MovementTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
						target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
						target.[PointTemplateGuid] = source.[PointTemplateGuid],
						target.[UpdatedDate] = SYSDATETIMEOFFSET(),
						target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

--Create MovementData Property
--Note : MovementData Value is xml serialization of FMBusinessObjects MovementData 
DECLARE @MovementDataPointTemplatePropertyGuid UNIQUEIDENTIFIER = '5C760DDA-DCD6-4EF0-BE8A-AEDEDFA7A3EC'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(
	SELECT 'Movement Data' as [ID],
	'FMBusinessObjects.DataObjects.MovementData' as [ValueType],
	'<MovementData xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	  <PointId></PointId>
	</MovementData>' as [Value],
	'2022-06-22' as [CreatedDate],
	'Administrator' as [CreatedBy],
	'2022-06-22' as [UpdatedDate],
	'Administrator' [UpdatedBy],
	@MovementDataPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
	@MovementTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
						target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
						target.[PointTemplateGuid] = source.[PointTemplateGuid],
						target.[UpdatedDate] = SYSDATETIMEOFFSET(),
						target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);



DECLARE @MovementModuleToPointTemplateGuid UNIQUEIDENTIFIER = '4236145C-4945-46B1-A700-201B99258A2C'

DECLARE @MovementModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>A95E83BA-CDD9-43C5-81BC-3DCF8145FFA0</TagGuid>
      <ModuleParameter>PercentDeviation</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>45f4af52-126a-4836-a336-6cde6d611e3b</TagGuid>
      <ModuleParameter>Command</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>065da402-8a0c-4cdb-b64f-83b7b4c0d3ed</TagGuid>
      <ModuleParameter>Status</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>1113b77f-e421-4086-b535-5c7cf3d16922</TagGuid>
      <ModuleParameter>TransferStartTime</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>955d3d56-b476-4b9a-9c8e-88a5b0d139a8</TagGuid>
      <ModuleParameter>TransferStopTime</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>4dcdc163-e055-417f-9016-9bb1913e730c</TagGuid>
      <ModuleParameter>InitiationCount</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d15e46ab-741d-4533-aeb9-95c1e98c9689</TagGuid>
      <ModuleParameter>MovementHistoryWrittenTime</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>5caa7f26-9a2a-4e67-a8f3-694bf5e2ef6b</TagGuid>
      <ModuleParameter>TransferredGOV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>f021c476-325d-4cf8-a59f-95b6b136a483</TagGuid>
      <ModuleParameter>TransferredNSV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>009f1eb8-4ee2-4b8f-ab75-7a066c1feca0</TagGuid>
      <ModuleParameter>TransferTimeRemaining</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>64be1f86-a923-4752-9902-d5bff4711ec1</TagGuid>
      <ModuleParameter>InitiateIdentifier</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ca09fcdf-81ff-4c52-8a1e-4b1867d0df3f</TagGuid>
      <ModuleParameter>StopIdentifier</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>322e377c-1995-4afe-a1ea-aeaac02d5c85</TagGuid>
      <ModuleParameter>MovementDiscreteAlarm</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>fc861ec7-89c7-4430-abe2-7caa8b9fbec1</PropertyGuid>
      <PropertyName>MovementModuleSettings</PropertyName>
    </PropertyToModule>
    <PropertyToModule>
      <PropertyGuid>5c760dda-dcd6-4ef0-be8a-aededfa7a3ec</PropertyGuid>
      <PropertyName>MovementData</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'


MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Movement' as [ID],
1 as [Order],
@MovementModuleToPointTemplateData AS [ModuleToPointTemplateData],
@MovementModuleToPointTemplateGuid as [ModuleToPointTemplateGuid],
@MovementTemplateGuid as [PointTemplateGuid] ,
@MovementModuleGuid as [ModuleGuid],
'2022-06-22' as [CreatedDate] ,
'Administrator' as [CreatedBy],
'2022-06-22' as [UpdatedDate],
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
if(0 = (SELECT COUNT(*) FROM map.tblEntityPointTemplateToSite WHERE PointTemplateGuid = @MovementTemplateGuid AND SiteGuid = @MovementSiteGuid))
BEGIN
		INSERT INTO map.tblEntityPointTemplateToSite ([PointTemplateToSiteGuid],[PointTemplateGuid],[SiteGuid],[AssignedFromSiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('ED3F33B1-FB56-4C66-B87B-A46E80BEA60D',@MovementTemplateGuid,@MovementSiteGuid,@MovementSiteGuid,'2022-06-30','Administrator','2022-06-30','Administrator')
END 

