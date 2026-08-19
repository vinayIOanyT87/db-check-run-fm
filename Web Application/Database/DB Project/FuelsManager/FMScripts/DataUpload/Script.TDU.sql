
-- DELETE TAGS FORMERLY IN Standard Movement Node Vol
--DELETE FROM tblPointTemplateTag WHERE PointTemplateTagGuid
--IN ('')

DECLARE @TDUSiteGuid UNIQUEIDENTIFIER  = '00000000-0000-0000-0000-000000000001'

-- Strap Table is the first Standard Module Developed, for Single Site System Modules are ownership changed to single site
IF EXISTS (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid)
BEGIN
	SET @TDUSiteGuid =  (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid);
END 



DECLARE @TDUTemplateGuid UNIQUEIDENTIFIER = '186348C4-C81F-4BC0-8A9E-5ABB9579885A'
DECLARE @TDUProfileImageGuid UNIQUEIDENTIFIER = (SELECT PictureGuid FROM dbo.tblPictures WHERE Id = 'Dial Template')


MERGE dbo.tblPointTemplate AS Target
USING 
( SELECT 'TDU' as [ID],
				'TDU Control Point' as [Description],
				1 as [Standard],
				NULL as [ExecutionInterval],
				27 as [LevelUnitIndex] ,
				2 as [TemperatureUnitIndex],
				191 as [DensityUnitIndex],
				82 as [PressureUnitIndex] ,
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
				284.0 as [TemperatureMaximum],
				-40.0 as [TemperatureMinimum],
				100 as [DensityMaximum],
				0 as [DensityMinimum],
				30.00 as [PressureMaximum],
				0 as [PressureMinimum],
				1000.00 as [VolumetricFlowMaximum],
				-1000.00 as [VolumetricFlowMinimum],
				210000.00 as [VolumeMaximum],
				0 as [VolumeMinimum],
				10000000 as [MassMaximum],
				0 as [MassMinimum],
				10 as [VelocityMaximum],
				-10 as [VelocityMinimum],
				3000 as [MassFlowMaximum],
				-3000 as [MassFlowMinimum],
				@TDUTemplateGuid as [PointTemplateGuid],
				@TDUSiteGuid as [SiteGuid],
				@TDUProfileImageGuid as [ProfileImageGuid],
				NULL as [DefaultDrawingGuid],
				'<PointCommandStatus xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <CommandStatusLists>
	<PointCommandStatusList>
	  <CommandStatusListGuid>b2337084-c5c3-fccf-9930-d166c72123af</CommandStatusListGuid>
	  <ID>Training Command</ID>
	  <CommandStatusList>
		<CommandStatusElement>
		  <Key>Start</Key>
		  <Value>146</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Cancel</Key>
		  <Value>326</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Stop</Key>
		  <Value>149</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Process</Key>
		  <Value>221</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Undefined</Key>
		  <Value>0</Value>
		</CommandStatusElement>
	  </CommandStatusList>
	</PointCommandStatusList>
	<PointCommandStatusList>
	  <CommandStatusListGuid>5077bbb1-65c8-da1f-4304-7c14e3522130</CommandStatusListGuid>
	  <ID>Hart Search Command</ID>
	  <CommandStatusList>
		<CommandStatusElement>
		  <Key>Start</Key>
		  <Value>146</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Disable</Key>
		  <Value>54</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Enable</Key>
		  <Value>61</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Suspend</Key>
		  <Value>347</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Search</Key>
		  <Value>346</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Searching</Key>
		  <Value>348</Value>
		</CommandStatusElement>
	  </CommandStatusList>
	</PointCommandStatusList>
	<PointCommandStatusList>
	  <CommandStatusListGuid>4ce60037-2623-4b60-e5a8-a4effff2dcda</CommandStatusListGuid>
	  <ID>Training Direction</ID>
	  <CommandStatusList>
		<CommandStatusElement>
		  <Key>Fill</Key>
		  <Value>222</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Empty</Key>
		  <Value>224</Value>
		</CommandStatusElement>
	  </CommandStatusList>
	</PointCommandStatusList>
	<PointCommandStatusList>
	  <CommandStatusListGuid>131d83fb-cba4-094e-cd45-fb5f195fb251</CommandStatusListGuid>
	  <ID>Training Status</ID>
	  <CommandStatusList>
		<CommandStatusElement>
		  <Key>In Progress</Key>
		  <Value>230</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Error</Key>
		  <Value>63</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Complete</Key>
		  <Value>232</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Executing</Key>
		  <Value>64</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Hold</Key>
		  <Value>81</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Cancel</Key>
		  <Value>326</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Stopped</Key>
		  <Value>228</Value>
		</CommandStatusElement>
		<CommandStatusElement>
		  <Key>Undefined</Key>
		  <Value>0</Value>
		</CommandStatusElement>
	  </CommandStatusList>
	</PointCommandStatusList>
  </CommandStatusLists>
</PointCommandStatus>' as [PointCommandStatus],
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


IF ((SELECT COUNT(*) FROM tblPointTemplate WHERE ID = 'TDU') = 1)
BEGIN
	DECLARE @TduPointTypeGuid UNIQUEIDENTIFIER
	SELECT @TduPointTypeGuid = ApplicationStringGuid FROM tblApplicationString WHERE ID = 'System'

	UPDATE tblPointTemplate SET PointTemplateTypeApplicationStringGuid = @TduPointTypeGuid WHERE ID = 'TDU' AND PointTemplateTypeApplicationStringGuid IS NULL
END


--Create Tags

DECLARE @TduNotepadTagGuid UNIQUEIDENTIFIER = 'B7208B3A-5233-7226-F694-B9EC10DD4CB8'
DECLARE @TduTrainCommTagGuid UNIQUEIDENTIFIER = '8817CA5F-5360-78CD-36FA-E27F150A4DB0'
DECLARE @TduTrCurrIndexTagGuid UNIQUEIDENTIFIER = 'BF851720-EA89-63CF-9C37-FDBAE622D663'
DECLARE @TduTrDirectionTagGuid UNIQUEIDENTIFIER = '8A6FC66E-9C9C-8CDD-6BBA-A98FD5A34577'
DECLARE @TduTrEndVolTagGuid UNIQUEIDENTIFIER = '7A89ABCE-3792-DEC8-2244-3E057E224396'
DECLARE @TduTrInitialVolTagGuid UNIQUEIDENTIFIER = '015AA21D-5011-BF49-1E7B-B0505D03E942'
DECLARE @TduTrNextVolTagGuid UNIQUEIDENTIFIER = 'A4DC706F-8606-2A2D-AF6C-4EEB3541C8BF'
DECLARE @TduTrRelaxVolTagGuid UNIQUEIDENTIFIER = '53933321-77A3-743E-59D0-868C04754B17'
DECLARE @TduTrStatusTagGuid UNIQUEIDENTIFIER = 'E482B754-7FC3-E9F0-B2FE-DB2A1E2ED5FC'
DECLARE @TduTrTempTagGuid UNIQUEIDENTIFIER = '56CD9C1D-2C8E-7428-FA01-9A7E48CB924E'
DECLARE @TduTrTkNumberTagGuid UNIQUEIDENTIFIER = '5E0FFB68-E900-B798-75C2-927D6042978A'
DECLARE @TduTrVolumeTagGuid UNIQUEIDENTIFIER = '8DD84845-13E8-EBB1-B134-73BA8BA2B25B'
DECLARE @TduSearchHartCommandTagGuid UNIQUEIDENTIFIER = '34B93CBA-001C-34B4-002D-0D2A4020FF80'
DECLARE @TduSearchHartTkNumTagGuid UNIQUEIDENTIFIER = 'FD100787-1D4D-B2B9-2553-A4BD08864ADD'
DECLARE @TduTduFirmwareTagGuid UNIQUEIDENTIFIER = '1170CCE1-93BF-4CF0-BD7E-4F542583336B'
DECLARE @TduTduStatusTagGuid UNIQUEIDENTIFIER = 'D59C5E63-0447-8BAC-70F0-E5719DEBD5CB'
DECLARE @TduTduTempTagGuid UNIQUEIDENTIFIER = '892CABCF-14CF-4AC2-243E-9145CFE5A998'
DECLARE @TduTduVoltageTagGuid UNIQUEIDENTIFIER = '894CCDE6-E173-9CCA-EA42-B92AA2A1CD21'
DECLARE @TduTrPressureTagGuid UNIQUEIDENTIFIER = '850514A2-3577-12CF-2895-2A14CC2A422E'

-- No alarms


MERGE dbo.tblPointTemplateTag AS Target
USING 
(  SELECT 'Notepad' as [ID], 
						16 as [EngineeringUnitsType],
						255 as [EngineeringUnitsIndex],
						0 as [DecimalPlaces],
						255 as [ServerEngineeringUnitsIndex],
						'System.String' as [ValueType],
						NULL as [Value],
						0 as [Maximum],
						0 as [Minimum],
						1 as [PointTagInputOutputTypeIndex],
						1 as [Input],
						0 as [AlarmStatus],
						1 as [ApplyPointTemplateEngineeringUnits],
						1 as [ApplyPointTemplateDecimalPlaces],
						1 as [ApplyPointTemplateMaximum],
						1 as [ApplyPointTemplateMinimum],
						@TduNotepadTagGuid as [PointTemplateTagGuid],
						@TDUTemplateGuid as [PointTemplateGuid],
						@TduNotepadWellKnownGuid as [WellKnownIdentityGuid],
						0 as [AlarmsEnabled],
						0 as [InhibitInputOutputTypeConfiguration],
						0 as [InhibitOverride],
						0 as [Module],
						1 as [Archived],
						'2023-11-03' as [CreatedDate],
						'Administrator' as [CreatedBy],
						'2023-11-03' as [UpdatedDate],
						'Administrator' as [UpdatedBy]	
		UNION ALL
		SELECT 'TrainComm' ,16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference','<PointCommandStatusListReference><PointCommandStatusListGuid>b2337084-c5c3-fccf-9930-d166c72123af</PointCommandStatusListGuid><CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" /><CurrentKey /></PointCommandStatusListReference>',0,0,1,0,0,0,0,0,0,@TduTrainCommTagGuid ,@TduTemplateGuid,@TduTrainCommWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrCurrIndex' ,16,255,0,255,'System.UInt16',NULL,4,0,3,1,0,0,0,0,0,@TduTrCurrIndexTagGuid,@TDUTemplateGuid,@TduTrCurrIndexWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrDirection' ,16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference','<PointCommandStatusListReference><PointCommandStatusListGuid>4ce60037-2623-4b60-e5a8-a4effff2dcda</PointCommandStatusListGuid><CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" /><CurrentKey /></PointCommandStatusListReference>',0,0,3,1,0,0,0,0,0,@TduTrDirectionTagGuid,@TDUTemplateGuid,@TduTrDirectionWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrEndVol' ,5,46,2,46,'System.Double',NULL,10000,0.0,1,0,0,0,0,0,0,@TduTrEndVolTagGuid,@TDUTemplateGuid,@TduTrEndVolWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrInitialVol' ,5,46,2,46,'System.Double',NULL,10000,0.0,1,0,0,0,0,0,0,@TduTrInitialVolTagGuid,@TDUTemplateGuid,@TduTrInitialVolWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrNextVol' ,5,46,2,46,'System.Double',NULL,10000.00,0.0,3,1,0,0,0,0,0,@TduTrNextVolTagGuid,@TDUTemplateGuid,@TduTrNextVolWellKnownGuid ,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrRelaxVol' ,5,46,2,46,'System.Double',NULL,10000.0,0,1,0,0,0,0,0,0,@TduTrRelaxVolTagGuid ,@TDUTemplateGuid,@TduTrRelaxVolWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrStatus' ,16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference','<PointCommandStatusListReference><PointCommandStatusListGuid>131d83fb-cba4-094e-cd45-fb5f195fb251</PointCommandStatusListGuid><CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" /><CurrentKey /></PointCommandStatusListReference>',0,0,3,1,0,0,0,0,0,@TduTrStatusTagGuid ,@TDUTemplateGuid,@TduTrStatusWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrTemp' ,1,2,2,2,'System.Double',NULL,300,-300,3,1,0,0,0,0,0,@TduTrTempTagGuid ,@TDUTemplateGuid,@TduTrTempWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrTkNumber' ,16,255,0,255,'System.UInt16',NULL,65355,0,1,0,0,0,0,0,0,@TduTrTkNumberTagGuid ,@TDUTemplateGuid,@TduTrTkNumberWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrVolume' ,5,46,2,46,'System.Double',NULL,10000.0,0,3,1,0,0,0,0,0,@TduTrVolumeTagGuid ,@TDUTemplateGuid,@TduTrVolumeWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'SearchHartCommand' ,16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference','<PointCommandStatusListReference><PointCommandStatusListGuid>5077bbb1-65c8-da1f-4304-7c14e3522130</PointCommandStatusListGuid><CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" /><CurrentKey /></PointCommandStatusListReference>',0,0,1,0,0,0,0,0,0,@TduSearchHartCommandTagGuid ,@TDUTemplateGuid,@TduSearchHartCommandWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'SearchHartTkNum' ,16,255,0,255,'System.UInt16',NULL,65535,0,1,0,0,0,0,0,0,@TduSearchHartTkNumTagGuid ,@TDUTemplateGuid,@TduSearchHartTkNumWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TDUFirmware' ,16,255,0,255,'System.String',NULL,0,0,3,1,0,0,0,0,0,@TduTduFirmwareTagGuid ,@TDUTemplateGuid,@TduTduFirmwareWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TDUStatus' ,16,255,0,255,'System.UInt16',NULL,65535,0,3,1,0,0,0,0,0,@TduTduStatusTagGuid,@TDUTemplateGuid,@TduTduStatusWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TDUTemp' ,1,1,2,1,'System.Double',NULL,148.888888889,-184.444444444,3,1,0,0,0,0,0,@TduTduTempTagGuid,@TDUTemplateGuid,@TduTduTempWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TDUVoltage' ,14,221,2,221,'System.Double',NULL,10000,0,3,1,0,0,0,0,0,@TduTduVoltageTagGuid,@TDUTemplateGuid,@TduTduVoltageWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
		UNION ALL
		SELECT 'TrPressure' ,7,82,2,82,'System.Double',NULL,200,-20,3,1,0,0,0,0,0,@TduTrPressureTagGuid,@TDUTemplateGuid,@TduTrPressureWellKnownGuid,0,0,0,0,1,'2023-11-03','Administrator','2023-11-03','Administrator'
) 
AS Source
ON (Target.[PointTemplateGuid] = Source.[PointTemplateGuid] AND Target.PointTemplateTagGuid = Source.PointTemplateTagGuid)
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
						target.[EngineeringUnitsType] = source.[EngineeringUnitsType],
						target.[DecimalPlaces] = source.[DecimalPlaces],
						target.[ServerEngineeringUnitsIndex] = source.[ServerEngineeringUnitsIndex],
						target.[ValueType] = source.[ValueType],
						target.[Value] = (CASE
							WHEN source.[ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' THEN source.[Value]
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



--Entity Assignment
if(0 = (SELECT COUNT(*) FROM map.tblEntityPointTemplateToSite WHERE PointTemplateGuid = @TDUTemplateGuid AND SiteGuid = @TDUSiteGuid))
BEGIN
		INSERT INTO map.tblEntityPointTemplateToSite ([PointTemplateToSiteGuid],[PointTemplateGuid],[SiteGuid],[AssignedFromSiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('e4c06c57-be57-4207-9f21-375b5fb888ea',@TDUTemplateGuid,@TDUSiteGuid,@TDUSiteGuid,'2022-06-22','Administrator','2022-06-22','Administrator')
END 

