
-- DELETE TAGS FORMERLY IN Standard Movement Node Vol
DELETE FROM tblPointTemplateTag WHERE PointTemplateTagGuid
IN ('005857F3-6A0B-45BB-8C2D-65BE0073FCB1',
'438E2509-FE01-40CB-BD42-12CF81B82193',
'160C8CD9-7650-43D2-A766-A1D936A112B3',
'72AB75AC-878D-4E8D-BC5A-B4AEE0BB4778',
'9F30B72C-8E1E-4F09-9A82-9B95715CE930',
'998E70B1-0062-4E15-865D-10B7A3A899CF',
'7EF0DD02-7DC4-41AF-9EA1-6B55143FDA6B',
'64EA5144-7781-4A45-93BD-AFA2AF12042E',
-- Also the Totalizer tags; the existing Volume Gross Observed and Volume Net Standard tags serve this purpose
'A6B61138-ABAE-42EB-858E-3AE0286DB487', -- Totalizer Gross
'13EEF134-58F9-4134-8E8E-28D1E99CEBEF', -- Totalizer Net
'AC7B1D75-D7A7-4DEB-A035-A21CB1965DEC'  -- Duplicated Density Observed tag
)

DECLARE @StandardVolumeSiteGuid UNIQUEIDENTIFIER  = '00000000-0000-0000-0000-000000000001'

-- Strap Table is the first Standard Module Developed, for Single Site System Modules are ownership changed to single site
IF EXISTS (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid)
BEGIN
	SET @StandardVolumeSiteGuid =  (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid);
END 



DECLARE @StandardVolumeTemplateGuid UNIQUEIDENTIFIER = '3C7895BF-8A90-40CB-AC3B-04FD089B438B'
DECLARE @StandardVolumeProfileImageGuid UNIQUEIDENTIFIER = (SELECT PictureGuid FROM dbo.tblPictures WHERE Id = 'Dial Template')


MERGE dbo.tblPointTemplate AS Target
USING 
( SELECT 'Standard Volume' as [ID],
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
				@StandardVolumeTemplateGuid as [PointTemplateGuid],
				@StandardVolumeSiteGuid as [SiteGuid],
				@StandardVolumeProfileImageGuid as [ProfileImageGuid],
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


IF ((SELECT COUNT(*) FROM tblPointTemplate WHERE ID = 'Standard Volume') = 1)
BEGIN
	DECLARE @StandardVolumePointTypeGuid UNIQUEIDENTIFIER
	SELECT @StandardVolumePointTypeGuid = ApplicationStringGuid FROM tblApplicationString WHERE ID = 'Meter'

	UPDATE tblPointTemplate SET PointTemplateTypeApplicationStringGuid = @StandardVolumePointTypeGuid WHERE ID = 'Standard Volume' AND PointTemplateTypeApplicationStringGuid IS NULL
END


--Create Tags

DECLARE @StandardVolumeVolumeGrossObservedTagGuid UNIQUEIDENTIFIER = 'ADC283D1-A9D0-42E2-B69F-D867B91C9B80'
DECLARE @StandardVolumeVolumeNetStandardTagGuid UNIQUEIDENTIFIER = '7A1DB105-DB02-4968-B17D-7D78B445AED9'
DECLARE @StandardVolumeVolumeGrossObservedRateTagGuid UNIQUEIDENTIFIER = 'F663C1BA-0B0A-460C-830F-C86070D7BB42'
DECLARE @StandardVolumeVolumeNetStandardRateTagGuid UNIQUEIDENTIFIER = '85CA2C60-3B5E-4FA2-938A-2541AC207EC3'
DECLARE @StandardVolumeTransferModeTagGuid UNIQUEIDENTIFIER = '9F5CB5A7-1C07-477A-A53D-E69A625DC7E1'
DECLARE @StandardVolumeTransferStatusTagGuid UNIQUEIDENTIFIER = 'BA9621C5-D9FB-41B6-885E-A9451D54ABA1'
DECLARE @StandardVolumeTransferTargetTagGuid UNIQUEIDENTIFIER = '29DA71C4-1A51-42EE-8DFB-C781BE5C7B2E'
DECLARE @StandardVolumeTransferredGOVTagGuid UNIQUEIDENTIFIER = '06BF336D-97E8-4AE6-A092-332731ACDF76'
DECLARE @StandardVolumeTransferredNSVTagGuid UNIQUEIDENTIFIER = 'BB42BC34-E90A-4067-B89E-FFFF9547F0E0'
DECLARE @StandardVolumeTransferredVolumeTagGuid UNIQUEIDENTIFIER = '945926ED-C944-4D5F-A6B6-22A64A660B70'
DECLARE @StandardVolumeTransferStartGOVTagGuid UNIQUEIDENTIFIER = '30A2BB23-DB8E-445A-BE09-32F7F1132F25'
DECLARE @StandardVolumeTransferStartNSVTagGuid UNIQUEIDENTIFIER = 'A8FDE285-C978-407D-A994-797AADF58C13'
DECLARE @StandardVolumeTransferStartVolumeTagGuid UNIQUEIDENTIFIER = '88F438F1-F4D2-4048-9C4B-D8A3B693C6DF'
DECLARE @StandardVolumeTransferTimeRemainingTagGuid UNIQUEIDENTIFIER = '85BD69C2-E54E-45AF-AB06-ACC17C1E8F76'
DECLARE @StandardVolumeTransferTimeCompletionTagGuid UNIQUEIDENTIFIER = '4F24D174-D533-4CBB-9654-D7D6DA332AFC'
DECLARE @StandardVolumeDescreteAlarmTagGuid UNIQUEIDENTIFIER = '7595F7E2-1490-4D22-BC43-68D09A3DCFDF'
DECLARE @StandardVolumeAdvisoryAlarmTagGuid UNIQUEIDENTIFIER = 'B39E729D-D26F-40EA-BD7E-8283E61E599F'
DECLARE @StandardVolumeTargetAlarmTagGuid UNIQUEIDENTIFIER = 'D3738551-BBB1-48A2-83F5-05BA5B4DFA15'
DECLARE @StandardVolumeReverseFlowAlarmTagGuid UNIQUEIDENTIFIER = '76C46A63-5C6B-4FE7-A69A-F95E8C5D3379'
DECLARE @StandardVolumeAdvisoryAlarmLimitTagGuid UNIQUEIDENTIFIER = 'E3CEB56C-2EB3-4D3A-966D-A1117B2993A7'
DECLARE @StandardVolumeTargetAlarmLimitTagGuid UNIQUEIDENTIFIER = '7C692270-E28D-450F-9BFF-00C50F439031' 
DECLARE @StandardVolumeReverseFlowAlarmLImitTagGuid UNIQUEIDENTIFIER = '4924CCAD-0580-4956-8458-DDC82E061F0A'
DECLARE @StandardVolumeMovementIdTagGuid UNIQUEIDENTIFIER = '72AB75AC-878D-4E8D-BC5A-B4AEE0BB4778'
DECLARE @StandardVolumeDensityProductObservedTagGuid UNIQUEIDENTIFIER = '75A69330-C70F-4BCC-B390-5C1905927D6E'
DECLARE @StandardVolumeDensityProductStandardTagGuid UNIQUEIDENTIFIER = '7AB31EB0-F82F-4E36-9DCB-5A125B1AD1D1'
DECLARE @StandardVolumeMassTagGuid UNIQUEIDENTIFIER = 'F1304099-C72D-452B-A857-9177F9DB4094'
DECLARE @StandardVolumeTemperatureProductTagGuid UNIQUEIDENTIFIER = '2A572C1B-3670-4D69-8AB2-303BBD4640A1'
DECLARE @StandardVolumeTransferStartTimeTagGuid UNIQUEIDENTIFIER = '9C1B1656-8C94-4E25-8862-E6A3B1E8A902'
DECLARE @StandardVolumeTransferStopTimeTagGuid UNIQUEIDENTIFIER = 'D4E548CA-40DA-4B79-A21A-C4D2EEC6C737'
DECLARE @StandardVolumeTransferVolumeTargetTagGuid UNIQUEIDENTIFIER = '902EF38D-5516-4BAB-951A-A0E0394D3DFC'

-- Temperature Alarm Tags

DECLARE @StandardVolumeTemperatureHiHiAlarmTagGuid UNIQUEIDENTIFIER = 'E8CF259C-7B32-4A67-BE9D-66EEF092E2A9'
DECLARE @StandardVolumeTemperatureHighAlarmTagGuid UNIQUEIDENTIFIER = 'DD4EE84A-9516-4079-83B6-0BBD7AC1FBCE'
DECLARE @StandardVolumeTemperatureLowAlarmTagGuid UNIQUEIDENTIFIER = '5FE58D2D-5555-4699-BBFD-B26960BECD3D'
DECLARE @StandardVolumeTemperatureLoLoAlarmTagGuid UNIQUEIDENTIFIER = '8F578FC4-90FC-46AE-B12C-A766AC11E389'

DECLARE @StandardVolumeTemperatureHiHiLimitTagGuid UNIQUEIDENTIFIER = 'B46E85FE-94D6-487D-A854-AD5C5A4239C3'
DECLARE @StandardVolumeTemperatureHighLimitTagGuid UNIQUEIDENTIFIER = 'AF23218B-5B85-4239-849E-04F31068A750'
DECLARE @StandardVolumeTemperatureLowLimitTagGuid UNIQUEIDENTIFIER = 'FF464E91-B3A6-4352-BF5A-C2B0B1762D9A'
DECLARE @StandardVolumeTemperatureLoLoLimitTagGuid UNIQUEIDENTIFIER = '8B83E9C9-5E17-4A3B-BA26-C66D67571A75'

DECLARE @StandardVolumeDensityHighAlarmTagGuid UNIQUEIDENTIFIER = 'D2ACD75F-6DDC-49C3-B34A-3A840EE0F770'
DECLARE @StandardVolumeDensityLowAlarmTagGuid UNIQUEIDENTIFIER = 'E11870F6-7781-4BF3-87D6-288B70477B44'

DECLARE @StandardVolumeDensityHighLimitTagGuid UNIQUEIDENTIFIER = '7EF0900B-1E76-4962-BC0E-931B316826D1'
DECLARE @StandardVolumeDensityLowLimitTagGuid UNIQUEIDENTIFIER = 'E318089F-4302-43E9-8269-C71F3EA81058'


-- Flow Rate NSV Alarm Tags
DECLARE @StandardVolumeVolumeNetStandardRateHighAlarmTagGuid		UNIQUEIDENTIFIER = 'A3C03791-F843-4EED-8653-F989FC1429A0'
DECLARE @StandardVolumeVolumeNetStandardRateHighAlarmLimitTagGuid	UNIQUEIDENTIFIER = 'D55E13D7-493F-40D4-B8F3-752C8EE18459'
DECLARE @StandardVolumeVolumeNetStandardReverseRateHighAlarmTagGuid		UNIQUEIDENTIFIER = '8532AFED-8F01-4DBC-9B17-1D9573162AAC'
DECLARE @StandardVolumeVolumeNetStandardReverseRateHighAlarmLimitTagGuid	UNIQUEIDENTIFIER = 'EB09BC8C-1FF8-478F-A52D-4279DEF4334E'

-- Flow Rate GOV Alarm Tag
DECLARE @StandardVolumeVolumeGrossObservedRateHighAlarmTagGuid			UNIQUEIDENTIFIER = 'EA83DF7B-170A-4FD4-8B75-BF77BA8B89AE'
DECLARE @StandardVolumeVolumeGrossObservedRateHighAlarmLimitTagGuid	UNIQUEIDENTIFIER = 'D4168185-83D9-478E-B537-0CE02ACFE329'
DECLARE @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTagGuid	UNIQUEIDENTIFIER = 'F2107301-2ACA-43BD-A389-5E47BF71ABDD'
DECLARE @StandardVolumeVolumeGrossObservedReverseRateHighAlarmLimitTagGuid	UNIQUEIDENTIFIER = '7CD2F70C-6ABD-413E-9AA4-964445B3563B'

-- TAC Fuels Tags
DECLARE @StandardVolumePulseRatioTagGuid	UNIQUEIDENTIFIER = '6D16C25A-7CF3-4BE5-8A05-F2544B23266E'
DECLARE @StandardVolumeResetTagGuid	UNIQUEIDENTIFIER = '1A82134A-98B9-43C9-86C2-EFA232BDEB19'
DECLARE @StandardVolumeFlowFilterTagGuid	UNIQUEIDENTIFIER = '0FDEECCA-BBC1-4A68-8879-9611294A1CDC'
DECLARE @PulseMeterNumberOfRollOversTagGuid	UNIQUEIDENTIFIER = '9CE844BE-4A53-4E31-BDE1-95EDE4D7D1C0'
DECLARE @PulseMeterLastValueTagGuid	UNIQUEIDENTIFIER = '1D4C53CD-41E2-49E3-8AAF-7A6A247A779A'
DECLARE @PulseMeterCurrentValueTagGuid	UNIQUEIDENTIFIER = 'C878E5A7-3EDF-470B-94F2-415D66A9A3A2'
DECLARE @PulseMeterVolumePerPulseTagGuid	UNIQUEIDENTIFIER = 'C9454F0E-A573-4D37-94FF-9C7E8BDD8DEE'
DECLARE @PulseMeterRollOverAmountTagGuid	UNIQUEIDENTIFIER = '6A5EC2CA-E2C9-4BDA-AD07-0B56B2372DA5'
DECLARE @PulseMeterLastReadWasRollOverTagGuid	UNIQUEIDENTIFIER = '1385627C-5BD4-4381-95F3-50D8C0C44507'

-- VCF Tags
DECLARE @StandardVolumeTemperatureDensityTemplateTagGuid UNIQUEIDENTIFIER = 'C6D3FE2B-B301-4E1F-ABA1-4E67BB214F99'
DECLARE @StandardVolumeTemperatureVaporTemplateTagGuid UNIQUEIDENTIFIER = '4CE264BE-116F-4C7D-A0D2-B8CB2ECE7747'
DECLARE @StandardVolumePressureVaporTemplateTagGuid UNIQUEIDENTIFIER = 'E959D2BC-83FA-4B41-A195-4D2029B69710'
DECLARE @StandardVolumeVolCorForTempTemplateTagGuid UNIQUEIDENTIFIER = 'EB7441A8-32B0-4116-A976-90A18B55FE13'
DECLARE @StandardVolumeVolCorForPressTemplateTagGuid UNIQUEIDENTIFIER = '3ECE1AAA-B613-4EE2-A320-B3FA5A314A95'
DECLARE @StandardVolumeVolCorForPressTempTemplateTagGuid UNIQUEIDENTIFIER = 'D4DA43C5-4437-4213-A0F8-3CE11D415AB7'
DECLARE @StandardVolumeVolumeCorrectionFactorTemplateTagGuid UNIQUEIDENTIFIER = '81E007E0-F325-438C-BBC4-B7667173EC6E'
DECLARE @StandardVolumeAPICorrectionErrorTemplateTagGuid UNIQUEIDENTIFIER = '4C8FDBE5-6617-4B17-B239-138559EE0DB3'
DECLARE @StandardVolumeDensityGaugeProductTemplateTagGuid UNIQUEIDENTIFIER = 'C96CA69D-FB49-4EEE-A1D5-E0377BDDE643'
DECLARE @StandardVolumeDensityInAirTemplateTagGuid UNIQUEIDENTIFIER = '15BD13CE-1B09-4E0B-8E72-8BC50A77CE46'
DECLARE @StandardVolumeStdDensityInAirTemplateTagGuid UNIQUEIDENTIFIER = '0FA5B3B2-ADBF-41BF-AE36-960F24C41273'
DECLARE @StandardVolumeVolumeCorrectionFactorUnroundedTagGuid uniqueidentifier = '4F02A4CF-A9C3-4B14-94D6-BAF528B62737'

MERGE dbo.tblPointTemplateTag AS Target
USING 
(  SELECT 'Volume Gross Observed' as [ID], 
						5 as [EngineeringUnitsType],
						46 as [EngineeringUnitsIndex],
						2 as [DecimalPlaces],
						46 as [ServerEngineeringUnitsIndex],
						'System.Double' as [ValueType],
						NULL as [Value],
						10000.00 as [Maximum],
						0 as [Minimum],
						3 as [PointTagInputOutputTypeIndex],
						1 as [Input],
						0 as [AlarmStatus],
						1 as [ApplyPointTemplateEngineeringUnits],
						1 as [ApplyPointTemplateDecimalPlaces],
						1 as [ApplyPointTemplateMaximum],
						1 as [ApplyPointTemplateMinimum],
						@StandardVolumeVolumeGrossObservedTagGuid as [PointTemplateTagGuid],
						@StandardVolumeTemplateGuid as [PointTemplateGuid],
						@VolumeGrossObservedWellKnownGuid as [WellKnownIdentityGuid],
						1 as [AlarmsEnabled],
						0 as [InhibitInputOutputTypeConfiguration],
						0 as [InhibitOverride],
						1 as [Module],
						1 as [Archived],
						'2022-06-22' as [CreatedDate],
						'Administrator' as [CreatedBy],
						'2022-06-22' as [UpdatedDate],
						'Administrator' as [UpdatedBy]	
		UNION ALL
		SELECT 'Volume Net Standard' ,5,46,2,46,'System.Double',NULL,10000.0,0,3,1,0,1,1,1,1,@StandardVolumeVolumeNetStandardTagGuid ,@StandardVolumeTemplateGuid,@VolumeNetStandardWellKnownGuid,1,0,0,1,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Rate' ,8,109,2,109,'System.Double',NULL,1000.0,-1000.0,3,1,0,1,1,1,1,@StandardVolumeVolumeGrossObservedRateTagGuid,@StandardVolumeTemplateGuid,@VolumeGrossObservedRateWellKnownGuid,1,0,0,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Rate' ,8,109,2,109,'System.Double',NULL,1000.0,-1000.0,3,1,0,1,1,1,1,@StandardVolumeVolumeNetStandardRateTagGuid,@StandardVolumeTemplateGuid,@VolumeNetStandardRateWellKnownGuid,1,0,0,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Mode' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode','<VolumeTransferMode>Inactive</VolumeTransferMode>',0.0,0.0,1,0,0,1,1,1,1,@StandardVolumeTransferModeTagGuid,@StandardVolumeTemplateGuid,@TransferModeWellKnownGuid,1,1,1,1,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Status' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses','<TransferStatuses>Inactive</TransferStatuses>',0.0,0.0,2,1,0,1,1,1,1,@StandardVolumeTransferStatusTagGuid,@StandardVolumeTemplateGuid,@TransferStatusWellKnownGuid,1,1,1,1,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Target' ,5,46,2,46,'System.Double',NULL,10000.00,0.0,1,0,0,1,1,1,1,@StandardVolumeTransferTargetTagGuid,@StandardVolumeTemplateGuid,@TransferTargetWellKnownGuid ,1,1,1,1,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Start GOV' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@StandardVolumeTransferStartGOVTagGuid ,@StandardVolumeTemplateGuid,@TransferStartVolumeGOVWellKnownGuid,1,1,1,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Start NSV' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@StandardVolumeTransferStartNSVTagGuid ,@StandardVolumeTemplateGuid,@TransferStartVolumeNSVWellKnownGuid,1,1,1,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Start Volume' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@StandardVolumeTransferStartVolumeTagGuid ,@StandardVolumeTemplateGuid,@TransferStartVolumeWellKnownGuid,1,1,1,1,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transferred GOV' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@StandardVolumeTransferredGOVTagGuid ,@StandardVolumeTemplateGuid,@TransferVolumeGOVWellKnownGuid,1,1,1,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transferred NSV' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@StandardVolumeTransferredNSVTagGuid ,@StandardVolumeTemplateGuid,@TransferVolumeNSVWellKnownGuid,1,1,1,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transferred Volume' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@StandardVolumeTransferredVolumeTagGuid ,@StandardVolumeTemplateGuid,@TransferVolumeWellKnownGuid,1,1,1,1,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transfer Time Remaining' ,16,255,0,255,'System.TimeSpan',NULL,0,0,2,1,0,1,1,1,1,@StandardVolumeTransferTimeRemainingTagGuid ,@StandardVolumeTemplateGuid,@TransferTimeRemainingWellKnownGuid,1,1,1,1,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Time Completion' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@StandardVolumeTransferTimeCompletionTagGuid ,@StandardVolumeTemplateGuid,@TransferTimeCompletionWellKnownGuid,1,1,1,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Discrete Alarm' ,16,255,0,255,'System.Int16',NULL,255.0,0,2,1,0,1,1,1,1,@StandardVolumeDescreteAlarmTagGuid,@StandardVolumeTemplateGuid,null,1,1,0,1,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Advisory Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeAdvisoryAlarmTagGuid,@StandardVolumeTemplateGuid,null,0,1,1,0,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Target Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeTargetAlarmTagGuid,@StandardVolumeTemplateGuid,null,0,1,1,0,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Reverse Flow Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeReverseFlowAlarmTagGuid,@StandardVolumeTemplateGuid,null,0,1,1,0,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Advisory Alarm Limit' ,16,255,0,255,'System.Int16','<short>1</short>',8192.0,0.0,1,1,0,1,1,1,1,@StandardVolumeAdvisoryAlarmLimitTagGuid,@StandardVolumeTemplateGuid,null,0,1,1,0,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Target Alarm Limit' ,16,255,0,255,'System.Int16','<short>2</short>',8192.0,0.0,1,1,0,1,1,1,1,@StandardVolumeTargetAlarmLimitTagGuid,@StandardVolumeTemplateGuid,null,0,1,1,0,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Reverse Flow Limit' ,16,255,0,255,'System.Int16','<short>4</short>',8192.0,0.0,1,1,0,1,1,1,1,@StandardVolumeReverseFlowAlarmLImitTagGuid,@StandardVolumeTemplateGuid,null,0,1,1,0,0,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Density Product Observed' ,11,191,2,191,'System.Double',NULL,100.0,0.0,2,1,0,1,1,1,1,@StandardVolumeDensityProductObservedTagGuid,@StandardVolumeTemplateGuid,@DensityProductObservedWellKnownGuid,1,0,0,0,1,'2022-07-08','Administrator','2022-07-08','Administrator'
		UNION ALL
		SELECT 'Density Product Standard' ,11,191,2,191,'System.Double',NULL,100.0,0.0,1,1,0,1,1,1,1,@StandardVolumeDensityProductStandardTagGuid,@StandardVolumeTemplateGuid,@DensityProductStandardWellKnownGuid ,1,0,0,0,1,'2022-07-08','Administrator','2022-07-08','Administrator'
		UNION ALL
		SELECT 'Mass Liquid' ,6,64,2,64,'System.Double',NULL,10000000.0,0,3,1,0,1,1,1,1,@StandardVolumeMassTagGuid ,@StandardVolumeTemplateGuid,@MassLiquidWellKnownGuid ,1,0,0,0,1,'2022-07-08','Administrator','2022-07-08','Administrator'
		UNION ALL
		SELECT 'Temperature Product' ,1,2,2,2,'System.Double',NULL,300.0,-300.0,1,1,0,1,1,1,1,@StandardVolumeTemperatureProductTagGuid,@StandardVolumeTemplateGuid,@TemperatureProductWellKnownGuid,1,0,0,1,1,'2022-07-08','Administrator','2022-07-08','Administrator'
		UNION ALL
		SELECT 'Transfer Start Time' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@StandardVolumeTransferStartTimeTagGuid ,@StandardVolumeTemplateGuid,@TransferStartTimeWellKnownGuid,1,1,0,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Stop Time' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@StandardVolumeTransferStopTimeTagGuid ,@StandardVolumeTemplateGuid,@TransferStopTimeWellKnownGuid,1,1,0,1,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Volume Target' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@StandardVolumeTransferVolumeTargetTagGuid ,@StandardVolumeTemplateGuid,@TransferVolumeTargetWellKnownGuid,1,1,1,1,0,'2015-02-04','Administrator','2015-02-04','Administrator'

		-- Temperature Alarm Tags
		UNION ALL
		SELECT 'Temperature Product Low Limit' ,1,2,2,2,'System.Double','<double>-240.0</double>',300.0,-300.0,1,1,0,1,1,1,1,@StandardVolumeTemperatureLowLimitTagGuid,@StandardVolumeTemplateGuid,@TemperatureProductLowWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product LoLo Limit' ,1,2,2,2,'System.Double','<double>-270.0</double>',300.0,-300.0,1,1,0,1,1,1,1,@StandardVolumeTemperatureLoLoLimitTagGuid,@StandardVolumeTemplateGuid,@TemperatureProductLoLoWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product High Limit' ,1,2,2,2,'System.Double','<double>240.0</double>',300.0,-300.0,1,1,0,1,1,1,1,@StandardVolumeTemperatureHighLimitTagGuid,@StandardVolumeTemplateGuid,@TemperatureProductHighWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product HiHi Limit' ,1,2,2,2,'System.Double','<double>270.0</double>',300.0,-300.0,1,1,0,1,1,1,1,@StandardVolumeTemperatureHiHiLimitTagGuid,@StandardVolumeTemplateGuid,@TemperatureProductHiHiWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'


		UNION ALL
		SELECT 'Temperature Product HiHi Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeTemperatureHiHiAlarmTagGuid,@StandardVolumeTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product High Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeTemperatureHighAlarmTagGuid,@StandardVolumeTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product Low Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeTemperatureLowAlarmTagGuid,@StandardVolumeTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product LoLo Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeTemperatureLoLoAlarmTagGuid,@StandardVolumeTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'

		-- Density Alarm Tags
		UNION ALL
		SELECT 'Density Product Observed Low Limit' ,11,191,2,191,'System.Double','<double>5.0</double>',100.0,0.0,1,1,0,1,1,1,1,@StandardVolumeDensityLowLimitTagGuid,@StandardVolumeTemplateGuid,@DensityProductLowWellKnownGuid ,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Density Product Observed High Limit' ,11,191,2,191,'System.Double','<double>95.0</double>',100.0,0.0,1,1,0,1,1,1,1,@StandardVolumeDensityHighLimitTagGuid,@StandardVolumeTemplateGuid,@DensityProductHighWellKnownGuid ,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'


		UNION ALL
		SELECT 'Density Product Observed High Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeDensityHighAlarmTagGuid,@StandardVolumeTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Density Product Observed Low Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@StandardVolumeDensityLowAlarmTagGuid,@StandardVolumeTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'


		-- Flow Rate NSV Alarm Tags
		UNION ALL
		SELECT 'Volume Net Standard Rate High Alarm', 15, 255, 2 ,255, 'System.String', NULL, 1000, 0, 2, 1, 0, 1, 1, 1, 1, @StandardVolumeVolumeNetStandardRateHighAlarmTagGuid, @StandardVolumeTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Rate High Limit', 8, 109, 2 ,109, 'System.Double', '<double>950</double>', 1000, 0, 1, 1, 0, 1, 1, 1, 0, @StandardVolumeVolumeNetStandardRateHighAlarmLimitTagGuid ,@StandardVolumeTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Reverse Rate High Alarm', 15, 255, 2 ,255, 'System.String', NULL, 1000, 0, 2, 1, 0, 1, 1, 1, 1, @StandardVolumeVolumeNetStandardReverseRateHighAlarmTagGuid, @StandardVolumeTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Reverse High Limit', 8, 109, 2 ,109, 'System.Double', '<double>-950</double>', 0, -1000, 1, 1, 0, 1, 1, 0, 1, @StandardVolumeVolumeNetStandardreverseRateHighAlarmLimitTagGuid ,@StandardVolumeTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'

		-- Flow Rate GOV Alarm Tags
		UNION ALL
		SELECT 'Volume Gross Observed Rate High Alarm', 15, 255, 2 ,255, 'System.String', NULL, 1000, 0, 2, 1, 0, 1, 1, 1, 1, @StandardVolumeVolumeGrossObservedRateHighAlarmTagGuid, @StandardVolumeTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Rate High Limit', 8, 109, 2 ,109, 'System.Double', '<double>950</double>', 1000, 0, 1, 1, 0, 1, 1, 1, 0, @StandardVolumeVolumeGrossObservedRateHighAlarmLimitTagGuid ,@StandardVolumeTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Reverse Rate High Alarm', 15, 255, 2 ,255, 'System.String', NULL, 1000, 0, 2, 1, 0, 1, 1, 1, 1, @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTagGuid, @StandardVolumeTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Reverse Rate High Limit', 8, 109, 2 ,109, 'System.Double', '<double>-950</double>', 0, -1000, 1, 1, 0, 1, 1, 0, 1, @StandardVolumeVolumeGrossObservedReverseRateHighAlarmLimitTagGuid ,@StandardVolumeTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'

		-- Tags for TAC Fuels
		UNION ALL
		SELECT 'Reset',16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference','<PointCommandStatusListReference><PointCommandStatusListGuid>7e5b9ce8-a66c-fa23-7fd9-8eadaa24c991</PointCommandStatusListGuid><CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" /><CurrentKey /></PointCommandStatusListReference>',0,0,1,0,0,1,1,1,1,@StandardVolumeResetTagGuid,@StandardVolumeTemplateGuid,null,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Flow Filter',16,255,6,255,'System.Int16',NULL,20,1,1,0,0,1,1,0,0,@StandardVolumeFlowFilterTagGuid,@StandardVolumeTemplateGuid,null,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Pulse Meter Number of Roll Overs',16,255,0,255,'System.Int16','<short>0</short>',99999999,0,2,0,0,1,1,0,0,@PulseMeterNumberOfRollOversTagGuid,@StandardVolumeTemplateGuid,@PulseMeterNumberOfRollOversWellKnownGuid,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Pulse Meter Last Value',16,255,0,255,'System.Int32','<int>0</int>',99999999,0,2,1,0,1,1,0,0,@PulseMeterLastValueTagGuid,@StandardVolumeTemplateGuid,@PulseMeterLastValueWellKnownGuid,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Pulse Meter Current Value',16,255,0,255,'System.Int32','<int>0</int>',99999999,0,3,1,0,1,1,0,0,@PulseMeterCurrentValueTagGuid,@StandardVolumeTemplateGuid,@PulseMeterCurrentValueWellKnownGuid,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Pulse Meter Volume Per Pulse',5,46,3,46,'System.Double','<double>0</double>',100,0,1,0,0,1,1,0,0,@PulseMeterVolumePerPulseTagGuid,@StandardVolumeTemplateGuid,@PulseMeterVolumePerPulseWellKnownGuid,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Pulse Meter Roll Over Amount',16,255,0,255,'System.Int32','<int>0</int>',99999999,0,1,1,0,1,1,0,0,@PulseMeterRollOverAmountTagGuid,@StandardVolumeTemplateGuid,@PulseMeterRollOverAmountWellKnownGuid,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Pulse Meter Last Read Was Roll Over',16,255,0,255,'System.Boolean','<boolean>false</boolean>',99999999,0,2,1,0,1,1,0,0,@PulseMeterLastReadWasRollOverTagGuid,@StandardVolumeTemplateGuid,@PulseMeterLastReadWasRollOverWellKnownGuid,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'

		-- Tags for VCF Module
		-- Temperature Density - input
		UNION ALL
		SELECT 'Temperature Density' ,1,2,2,2,'System.Double',NULL,300.0,-300.0,0,1,0,1,1,1,1,@StandardVolumeTemperatureDensityTemplateTagGuid,@StandardVolumeTemplateGuid,@TemperatureDensityWellKnownGuid,1,0,0,1,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Temperature Vapor - input
		UNION ALL
		SELECT 'Temperature Vapor' ,1,2,2,2,'System.Double',NULL,300.0,-300.0,1,1,0,1,1,1,1,@StandardVolumeTemperatureVaporTemplateTagGuid,@StandardVolumeTemplateGuid,null,1,0,0,1,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Density Product Observed - input
		UNION ALL
		SELECT 'Density Product in Air' ,11,191,2,191,'System.Double',NULL,100.0,0,2,1,0,1,1,1,1,@StandardVolumeDensityInAirTemplateTagGuid ,@StandardVolumeTemplateGuid,@DensityProductInAirWellKnownGuid,1,0,0,1,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		-- Density Product in Air Standard - input
		UNION ALL
		SELECT 'Density Product Standard in Air' ,11,191,2,191,'System.Double',NULL,100.0,0,2,1,0,1,1,1,1,@StandardVolumeStdDensityInAirTemplateTagGuid ,@StandardVolumeTemplateGuid,@DensityProductStandardInAirWellKnownGuid,1,0,0,1,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		-- Pressure Vapor - input
		UNION ALL
		SELECT 'Pressure Vapor' ,7,73,2,73,'System.Double',NULL,30.0,0.0,1,1,0,1,1,1,1,@StandardVolumePressureVaporTemplateTagGuid,@StandardVolumeTemplateGuid,@PressureVaporWellKnownGuid ,1,0,0,1,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Volume Correction for Temperature - ctl - output
		UNION ALL
		SELECT 'Volume Correction for Temperature' ,15,255,5,255,'System.Double',NULL,2.0,0.0,2,1,0,1,0,0,0,@StandardVolumeVolCorForTempTemplateTagGuid,@StandardVolumeTemplateGuid,null,1,0,0,1,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Volume Correction for Pressure - cpl - output
		UNION ALL
		SELECT 'Volume Correction for Pressure' ,15,255,5,255,'System.Double',NULL,2.0,0.0,2,1,0,1,0,0,0,@StandardVolumeVolCorForPressTemplateTagGuid,@StandardVolumeTemplateGuid,null,1,0,0,1,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Volume Correction for Temp and Press - ctpl - output
		UNION ALL
		SELECT 'Volume Correction for Temperature and Pressure' ,15,255,5,255,'System.Double',NULL,2.0,0.0,2,1,0,1,0,0,0,@StandardVolumeVolCorForPressTempTemplateTagGuid,@StandardVolumeTemplateGuid,null,1,0,0,1,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Volume Correction Factor - VCF UnRounded - output
		UNION ALL
		SELECT 'Volume Correction Factor' ,15,255,5,255,'System.Double',NULL,2.0,0.0,2,1,0,1,0,0,0,@StandardVolumeVolumeCorrectionFactorTemplateTagGuid,@StandardVolumeTemplateGuid,@VolumeCorrectionFactorWellKnownGuid,1,0,0,1,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- API Correction Error - output
		UNION ALL
		SELECT 'API Correction Error' ,16,255,0,255,'System.Boolean',NULL,1.0,0.0,2,1,0,1,1,1,1,@StandardVolumeAPICorrectionErrorTemplateTagGuid,@StandardVolumeTemplateGuid,null,1,0,0,1,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Density Gauge - input
		UNION ALL
		SELECT 'Density Product Gauge' ,11,191,2,191,'System.Double',NULL,100.0,0.0,1,1,0,1,1,1,1,@StandardVolumeDensityGaugeProductTemplateTagGuid,@StandardVolumeTemplateGuid,null ,1,0,0,1,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Volume Correction Factor Unrounded',15,255,9,255,'System.Double',NULL,2,0,2,1,0,1,0,0,0,@StandardVolumeVolumeCorrectionFactorUnroundedTagGuid,@StandardVolumeTemplateGuid,@VolumeCorrectionFactorUnroundedWellKnownGuid,1,0,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'
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
						WHEN source.[ValueType] = 'FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode' THEN source.[Value]
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

-- Create Alarms
DECLARE @StandardVolumeAlarmGroupCategoryGuid UNIQUEIDENTIFIER = '512ab266-b3b8-4a29-b8d9-594795cf63ed'


DECLARE @StandardVolumeAdvisoryAlarmGuid UNIQUEIDENTIFIER = '94B4A85A-66BB-4312-BD52-B2A62AF6725D'
DECLARE @StandardVolumeTargetAlarmGuid UNIQUEIDENTIFIER = '56074A87-7954-40BA-A237-4848EE881B5B'
DECLARE @StandardVolumeReverseFlowAlarmGuid UNIQUEIDENTIFIER = '8149F7F6-52AC-47BA-AFBC-9BCEF6D8C6DF'

DECLARE @StandardVolumeTemperatureHiHiAlarmTemplateGuid UNIQUEIDENTIFIER = '56449C60-4FC2-4D81-B302-9B1CDB0C41C9'
DECLARE @StandardVolumeTemperatureHighAlarmTemplateGuid UNIQUEIDENTIFIER = '97E4BAEA-29F2-42D5-A4E1-D62864DF0C17'
DECLARE @StandardVolumeTemperatureLowAlarmTemplateGuid UNIQUEIDENTIFIER = 'F82F6B30-DD09-4E6E-AEF8-4428BFD584D2'
DECLARE @StandardVolumeTemperatureLoLoAlarmTemplateGuid UNIQUEIDENTIFIER = '31C40846-4811-4BCE-AD50-C9AA720F0B1D'

DECLARE @StandardVolumeDensityHighAlarmTemplateGuid UNIQUEIDENTIFIER = 'B9AF2BFB-5FCB-41C8-92D0-30E9FEB4C348'
DECLARE @StandardVolumeDensityLowAlarmTemplateGuid UNIQUEIDENTIFIER = '6C799F62-D740-49D3-90F4-F66136F14080'

DECLARE @StandardVolumeVolumeNetStandardRateHighAlarmTemplateGuid	UNIQUEIDENTIFIER = 'F19D5780-DB56-437C-ADC9-C2E494F60ACB'
DECLARE @StandardVolumeVolumeNetStandardReverseRateHighAlarmTemplateGuid	UNIQUEIDENTIFIER = '0A7B2658-AEC2-424E-8C6F-A80F960BC0DB'

DECLARE @StandardVolumeVolumeGrossObservedRateHighAlarmTemplateGuid UNIQUEIDENTIFIER = 'F8BBC5A9-075E-4005-AA04-69FD1EF95CAA'
DECLARE @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTemplateGuid UNIQUEIDENTIFIER = 'E36C2037-4C17-4B8B-B4DF-8D97CDB91315'



MERGE dbo.tblAlarmTemplate AS Target
USING 
(  SELECT @StandardVolumeAdvisoryAlarmGuid AS [AlarmTemplateGuid]
			,@StandardVolumeDescreteAlarmTagGuid AS [InputTemplateTagGuid]
			,'Transfer Advisory Alarm' AS [ID]
			,1 AS [Enabled]
			,@StandardVolumeAlarmGroupCategoryGuid AS [AlarmCategoryApplicationStringGuid]
			,0 AS [Order]
			,'Normal' AS [NotAlarmState]
			,'Alarm Comment' AS [Comment]
			,null AS [ShelvedStartTimeStamp]
			,null AS [ShelvedEndTimeStamp]
			,0 AS [ShelvedOneShot]
			,0 AS [Suppressed]
			,'2022-06-22' as [CreatedDate]
			,'Administrator' as [CreatedBy]
			,'2022-06-22' as [UpdatedDate]
			,'Administrator' as [UpdatedBy]
			,@StandardVolumeAdvisoryAlarmTagGuid AS [AlarmStateTemplateTagGuid]
			,1 AS [ExclusiveAlarm]
	UNION ALL
	SELECT @StandardVolumeTargetAlarmGuid,@StandardVolumeDescreteAlarmTagGuid,'Transfer Target Alarm',1,@StandardVolumeAlarmGroupCategoryGuid,1,'Normal','Alarm Comment',null,null,0,0,'2022-06-22','Administrator','2022-06-22','Administrator',@StandardVolumeTargetAlarmTagGuid,1
	UNION ALL
	SELECT @StandardVolumeReverseFlowAlarmGuid,@StandardVolumeDescreteAlarmTagGuid,'Reverse Flow Alarm',1,@StandardVolumeAlarmGroupCategoryGuid,2,'Normal','Alarm Comment',null,null,0,0,'2022-06-22','Administrator','2022-06-22','Administrator',@StandardVolumeReverseFlowAlarmTagGuid,1
	UNION ALL
	SELECT @StandardVolumeTemperatureHiHiAlarmTemplateGuid,@StandardVolumeTemperatureProductTagGuid,'Temperature HiHi Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@StandardVolumeTemperatureHiHiAlarmTagGuid,1
	UNION ALL
	SELECT @StandardVolumeTemperatureHighAlarmTemplateGuid,@StandardVolumeTemperatureProductTagGuid,'Temperature High Alarm',1,@AlarmApplicationStringGuid,1,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@StandardVolumeTemperatureHighAlarmTagGuid,1
	UNION ALL
	SELECT @StandardVolumeTemperatureLowAlarmTemplateGuid,@StandardVolumeTemperatureProductTagGuid,'Temperature Low Alarm',1,@AlarmApplicationStringGuid,1,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@StandardVolumeTemperatureLowAlarmTagGuid,1
	UNION ALL
	SELECT @StandardVolumeTemperatureLoLoAlarmTemplateGuid,@StandardVolumeTemperatureProductTagGuid,'Temperature LoLo Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@StandardVolumeTemperatureLoLoAlarmTagGuid,1
	UNION ALL
	SELECT @StandardVolumeDensityHighAlarmTemplateGuid,@StandardVolumeDensityProductObservedTagGuid,'Density High Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@StandardVolumeDensityHighAlarmTagGuid,1
	UNION ALL
	SELECT @StandardVolumeDensityLowAlarmTemplateGuid,@StandardVolumeDensityProductObservedTagGuid,'Density Low Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@StandardVolumeDensityLowAlarmTagGuid,1
	UNION ALL
	SELECT @StandardVolumeVolumeNetStandardRateHighAlarmTemplateGuid, @StandardVolumeVolumeNetStandardRateTagGuid, 'Volume Net Standard Rate High Alarm', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @StandardVolumeVolumeNetStandardRateHighAlarmTagGuid, 1
	UNION ALL
	SELECT @StandardVolumeVolumeNetStandardReverseRateHighAlarmTemplateGuid, @StandardVolumeVolumeNetStandardRateTagGuid, 'Volume Net Standard Reverse Rate High Alarm', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @StandardVolumeVolumeNetStandardReverseRateHighAlarmTagGuid, 1
	UNION ALL
	SELECT @StandardVolumeVolumeGrossObservedRateHighAlarmTemplateGuid, @StandardVolumeVolumeGrossObservedRateTagGuid, 'Volume Gross Observed Rate High Alarm', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @StandardVolumeVolumeGrossObservedRateHighAlarmTagGuid, 1
	UNION ALL
	SELECT @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTemplateGuid, @StandardVolumeVolumeGrossObservedRateTagGuid, 'Volume Gross Observed Reverse Rate High Alarm', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTagGuid, 1

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


DECLARE @StandardVolumeAdvisoryAlarmTestGuid UNIQUEIDENTIFIER = 'B74939A0-8CBE-4FFE-AF0F-BCE211978DD1'
DECLARE @StandardVolumeTargetAlarmTestGuid UNIQUEIDENTIFIER = '5D9B979F-1534-49E3-B471-065AC8C1A887'
DECLARE @StandardVolumeReverseFlowAlarmTestGuid UNIQUEIDENTIFIER = 'E3E11A92-0172-4D1A-8BD7-C05AC18C672A'

DECLARE @StandardVolumeTemperatureProductLowAlarmTestGuid UNIQUEIDENTIFIER = '8FA16AB1-39BC-4B59-A376-30135E783D6D'
DECLARE @StandardVolumeTemperatureProductLoLoAlarmTestGuid UNIQUEIDENTIFIER = '5D1B6532-20E5-4A45-9312-409413D137CB'
DECLARE @StandardVolumeTemperatureProductHighAlarmTestGuid UNIQUEIDENTIFIER = '7CEF2800-B041-440F-913C-28A51EDE9FBC'
DECLARE @StandardVolumeTemperatureProductHiHiAlarmTestGuid UNIQUEIDENTIFIER = '444A8F0C-A179-4E4A-B111-3D1CEFB11D5F'

DECLARE @StandardVolumeDensityProductLowAlarmTestGuid UNIQUEIDENTIFIER = '18E229BA-0EE6-4CB7-80A0-F5CD8FB1004B'
DECLARE @StandardVolumeDensityProductHighAlarmTestGuid UNIQUEIDENTIFIER = '4C92D692-6DBB-410E-A2C9-59956D036BAD'

DECLARE @StandardVolumeVolumeNetStandardRateHighAlarmTestGuid UNIQUEIDENTIFIER = 'F772A45C-FAFE-4F64-BD7C-CCBE5D3C188F'
DECLARE @StandardVolumeVolumeNetStandardReverseRateHighAlarmTestGuid UNIQUEIDENTIFIER = '75C7324D-C705-4B0A-B826-250CA9BB4651'

DECLARE @StandardVolumeVolumeGrossObservedRateHighAlarmTestGuid UNIQUEIDENTIFIER = '0F0D4B25-C334-4F07-A459-C0CA59F0E8BA'
DECLARE @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTestGuid UNIQUEIDENTIFIER = '4D63DCE2-1470-4A76-92C8-8743531B9FDB'



MERGE dbo.tblAlarmTestTemplate AS Target
USING 
(  SELECT @StandardVolumeAdvisoryAlarmTestGuid AS [AlarmTestTemplateGuid]
		,@StandardVolumeAdvisoryAlarmGuid AS [AlarmTemplateGuid]
		,'Traansfer Advisory Test' AS [ID]
		,@StandardVolumeAdvisoryAlarmLimitTagGuid AS [LimitTemplateTagGuid]
		,0 AS [TagField]
		,@HighLowAlarmPriorityGuid AS [AlarmPriorityGuid]
		,@NormalUnacknowledgedAlarmPriorityGuid as [NormalUnacknowledgedAlarmPriorityGuid]
		,4 AS [TestType] -- enum TestTypeEnum { GreaterThan = 0, GreaterThanOrEqual = 1, LessThan = 2, LessThanOrEqual = 3, Equals = 4, NotEquals = 5 }
		,CONVERT(INT, CONVERT(VARBINARY, '0x01', 1))AS [BitMask]
		,1 AS [Enabled]
		,0 AS [Order]
		,'Transfer Advisory' AS [AlarmState]
		,0.00 AS [Holdoff]
		,'Transfer Advisory Alarm' AS [AlarmText]
		,null AS [HelpFile]
		,null AS [DrawingGuid]
		,'2022-06-22' as [CreatedDate]
		,'Administrator' as [CreatedBy]
		,'2022-06-22' as [UpdatedDate]
		,'Administrator' as [UpdatedBy]
		,0 as [BitwiseOperator]
		,0 as [TimedHoldOffInSeconds]
		UNION ALL
		SELECT @StandardVolumeTargetAlarmTestGuid,@StandardVolumeTargetAlarmGuid,'Transfer Target Test',@StandardVolumeTargetAlarmLimitTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x02', 1)),1,0,'Transfer Target',0.00,'Transfer Target Alarm',null,null,'2022-06-22','Administrator','2022-06-22','Administrator',0,0
		UNION ALL
		SELECT @StandardVolumeReverseFlowAlarmTestGuid,@StandardVolumeReverseFlowAlarmGuid,'Reverse Flow Test',@StandardVolumeReverseFlowAlarmLImitTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x04', 1)),1,0,'Reverse Flow',0.00,'Reverse Flow Alarm',null,null,'2022-06-22','Administrator','2022-06-22','Administrator',0,0
		UNION ALL
		SELECT @StandardVolumeTemperatureProductHiHiAlarmTestGuid,@StandardVolumeTemperatureHiHiAlarmTemplateGuid,'HiHi Test',@StandardVolumeTemperatureHiHiLimitTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'HiHi Alarm',0.00,'Temperature Product HiHi Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @StandardVolumeTemperatureProductHighAlarmTestGuid,@StandardVolumeTemperatureHighAlarmTemplateGuid,'High Test',@StandardVolumeTemperatureHighLimitTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'High Alarm',0.00,'Temperature Product High Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @StandardVolumeTemperatureProductLowAlarmTestGuid,@StandardVolumeTemperatureLowAlarmTemplateGuid,'Low Test',@StandardVolumeTemperatureLowLimitTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'Low Alarm',0.00,'Temperature Product Low Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @StandardVolumeTemperatureProductLoLoAlarmTestGuid,@StandardVolumeTemperatureLoLoAlarmTemplateGuid,'LoLo Test',@StandardVolumeTemperatureLoLoLimitTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'LoLo Alarm',0.00,'Temperature Product LoLo Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @StandardVolumeDensityProductHighAlarmTestGuid,@StandardVolumeDensityHighAlarmTemplateGuid,'High Test',@StandardVolumeDensityHighLimitTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'High Alarm',0.00,'Density Product Observed High Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @StandardVolumeDensityProductLowAlarmTestGuid,@StandardVolumeDensityLowAlarmTemplateGuid,'Low Test',@StandardVolumeDensityLowLimitTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'Low Alarm',0.00,'Density Product Observed Low Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @StandardVolumeVolumeNetStandardRateHighAlarmTestGuid, @StandardVolumeVolumeNetStandardRateHighAlarmTemplateGuid, 'Volume Net Standard Rate High Test', @StandardVolumeVolumeNetStandardRateHighAlarmLimitTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 1, -1, 1, 0, 'High Alarm', 0.00, 'Volume Net Standard Rate Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0
		UNION ALL
		SELECT @StandardVolumeVolumeNetStandardReverseRateHighAlarmTestGuid, @StandardVolumeVolumeNetStandardReverseRateHighAlarmTemplateGuid, 'Volume Net Standard Reverse Rate High Test', @StandardVolumeVolumeNetStandardReverseRateHighAlarmLimitTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 3, -1, 1, 0, 'Reverse High Alarm', 0.00, 'Volume Net Standard Reverse Rate Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0
		UNION ALL
		SELECT @StandardVolumeVolumeGrossObservedRateHighAlarmTestGuid, @StandardVolumeVolumeGrossObservedRateHighAlarmTemplateGuid, 'Volume Gross Observed Rate High Test', @StandardVolumeVolumeGrossObservedRateHighAlarmLimitTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 1, -1, 1, 0, 'High Alarm', 0.00, 'Volume Gross Observed Rate Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0
		UNION ALL
		SELECT @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTestGuid, @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTemplateGuid, 'Volume Gross Observed Reverse Rate High Test', @StandardVolumeVolumeGrossObservedReverseRateHighAlarmLimitTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 3, -1, 1, 0, 'Reverse High Alarm', 0.00, 'Volume Gross Observed Reverse Rate Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0

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


DECLARE @StandardVolumeAdvisoryAlarmStatusGuid UNIQUEIDENTIFIER = 'B66A7E4A-9B69-4BFD-9C86-BF2DF1AF1B19'
DECLARE @StandardVolumeTargetAlarmStatusGuid UNIQUEIDENTIFIER = 'E2A03B61-1A14-4C5E-895C-CAFEE9CD5C0C'
DECLARE @StandardVolumeReverseFlowAlarmStatusGuid UNIQUEIDENTIFIER = '851C9A3F-AF53-48BA-B59C-855B31C74030'

DECLARE @StandardVolumeTemperatureProductLowAlarmStatusGuid UNIQUEIDENTIFIER = 'A76C0A0D-79AA-4C93-8986-175F9F97496A'
DECLARE @StandardVolumeTemperatureProductLoLoAlarmStatusGuid UNIQUEIDENTIFIER = 'D1F5C91B-7332-4FFF-AF91-C06D17B49AE3'
DECLARE @StandardVolumeTemperatureProductHighAlarmStatusGuid UNIQUEIDENTIFIER = 'C8A89A34-9A66-4C1A-B328-D3CDCA845A2F'
DECLARE @StandardVolumeTemperatureProductHiHiAlarmStatusGuid UNIQUEIDENTIFIER = 'D22814DE-A9F1-481E-B865-8212A1ECE503'

DECLARE @StandardVolumeDensityProductLowAlarmStatusGuid UNIQUEIDENTIFIER = 'A2FEBB8E-1B6A-4ADB-90DC-DEDCE3EE313A'
DECLARE @StandardVolumeDensityProductHighAlarmStatusGuid UNIQUEIDENTIFIER = 'D3EA8210-74EB-46F2-9E8C-A3F9DA6E70DE'

DECLARE @StandardVolumeVolumeNetStandardRateHighAlarmStatusGuid UNIQUEIDENTIFIER = 'F489DDA6-4F0D-445B-BE23-B95A6B8DF704'
DECLARE @StandardVolumeVolumeNetStandardReverseRateHighAlarmStatusGuid UNIQUEIDENTIFIER = 'C92C7F9B-9F9C-49AF-B044-823CA94DBA8D'

DECLARE @StandardVolumeVolumeGrossObservedRateHighAlarmStatusGuid UNIQUEIDENTIFIER = '3D4A9934-3AE5-4AEA-B7E6-4C0F07E81B61'
DECLARE @StandardVolumeVolumeGrossObservedReverseRateHighAlarmStatusGuid UNIQUEIDENTIFIER = '41CAA0E9-BB17-48D0-AFCE-5DD2104804F9'



MERGE dbo.tblPointTemplateTagAlarmStatus AS Target
USING 
(  SELECT @StandardVolumeAdvisoryAlarmStatusGuid AS [PointTemplateTagAlarmStatusGuid],
	@StandardVolumeAdvisoryAlarmTestGuid AS[AlarmTestTemplateGuid],
	1 AS [Acknowledged],
	null AS [AcknowledgedTimestamp],
	null AS [AcknowledgedBy],
	null AS [AcknowledgedComment],
	1 AS [Silenced],
	null as [SilencedTimestamp],
	null as [SilencedBy],
	0 AS [AlarmTestFailed],
	null AS [AlarmTestFailedTimestamp],
	'2022-06-22' as [CreatedDate],
	'Administrator' as [CreatedBy],
	'2022-06-22' as [UpdatedDate],
	'Administrator' as [UpdatedBy]
		UNION ALL
		SELECT @StandardVolumeTargetAlarmStatusGuid,@StandardVolumeTargetAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT @StandardVolumeReverseFlowAlarmStatusGuid,@StandardVolumeReverseFlowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT @StandardVolumeTemperatureProductHiHiAlarmStatusGuid,@StandardVolumeTemperatureProductHiHiAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @StandardVolumeTemperatureProductHighAlarmStatusGuid,@StandardVolumeTemperatureProductHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @StandardVolumeTemperatureProductLowAlarmStatusGuid,@StandardVolumeTemperatureProductLowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @StandardVolumeTemperatureProductLoLoAlarmStatusGuid,@StandardVolumeTemperatureProductLoLoAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @StandardVolumeDensityProductHighAlarmStatusGuid,@StandardVolumeDensityProductHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @StandardVolumeDensityProductLowAlarmStatusGuid,@StandardVolumeDensityProductLowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @StandardVolumeVolumeNetStandardRateHighAlarmStatusGuid, @StandardVolumeVolumeNetStandardRateHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @StandardVolumeVolumeNetStandardReverseRateHighAlarmStatusGuid, @StandardVolumeVolumeNetStandardReverseRateHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @StandardVolumeVolumeGrossObservedRateHighAlarmStatusGuid, @StandardVolumeVolumeGrossObservedRateHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @StandardVolumeVolumeGrossObservedReverseRateHighAlarmStatusGuid, @StandardVolumeVolumeGrossObservedReverseRateHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'

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


--Create Volume Gross Observed Rate Settings Property

--Note : RateSettings Value is xml serialization of FMBusinessObjects RateModuleSettings

DECLARE @VolumeGrossObservedRateSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = 'A668316B-2963-41F9-AF8D-162EE2F5D255'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Rate Volume Gross Observed Settings' as [ID],
'FMBusinessObjects.DataObjects.RateModuleSettings' as [ValueType],
'<RateModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Deadband>0</Deadband>
  <StaleTimePeriodInSeconds>60</StaleTimePeriodInSeconds>
  <FlowCalculationType>Averaging</FlowCalculationType>
  <AveragingNumberSamples>4</AveragingNumberSamples>
  <AveragingSampleTimeSeconds>30</AveragingSampleTimeSeconds>
</RateModuleSettings>' as [Value], 
'2022-06-22' as [CreatedDate],
'Administrator' as [CreatedBy],
'2022-06-22' as [UpdatedDate],
'Administrator' [UpdatedBy],
@VolumeGrossObservedRateSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@StandardVolumeTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
						target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
						target.[PointTemplateGuid] = source.[PointTemplateGuid],
						target.[UpdatedDate] = SYSDATETIMEOFFSET(),
						target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

--Create mapping of Rate Module to Point Template

DECLARE @RateModuleVolumeGrossObservedToPointTemplateGuid UNIQUEIDENTIFIER = '9372451D-E630-436C-AFF8-6841F1DC5A41'

DECLARE @RateModuleVolumeGrossObservedToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolumeGrossObservedTagGuid) + '</TagGuid>
	  <ModuleParameter>Value</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolumeGrossObservedRateTagGuid) + '</TagGuid>
	  <ModuleParameter>Rate</ModuleParameter>
	</TagToModule>
  </TagToModules>
  <PropertyToModules>
	<PropertyToModule>
	  <PropertyGuid>' + CONVERT(NVARCHAR(36),@VolumeGrossObservedRateSettingsPointTemplatePropertyGuid) + '</PropertyGuid>
	  <PropertyName>Settings</PropertyName>
	</PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'


MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Rate Volume Gross Observed' as [ID],
3 as [Order],
@RateModuleVolumeGrossObservedToPointTemplateData AS [ModuleToPointTemplateData],
@RateModuleVolumeGrossObservedToPointTemplateGuid as [ModuleToPointTemplateGuid],
@StandardVolumeTemplateGuid as [PointTemplateGuid] ,
@RateModuleGuid as [ModuleGuid],
'2015-02-04' as [CreatedDate] ,
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
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

--Create Volume Gross Observed Rate Settings Property

--Note : RateSettings Value is xml serialization of FMBusinessObjects RateModuleSettings

DECLARE @VolumeNetStandardRateSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = 'B7A7C15F-18A9-4653-81F3-BD35AE33F44F'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Rate Volume Net Standard Settings' as [ID],
'FMBusinessObjects.DataObjects.RateModuleSettings' as [ValueType],
'<RateModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Deadband>0</Deadband>
  <StaleTimePeriodInSeconds>60</StaleTimePeriodInSeconds>
  <FlowCalculationType>Averaging</FlowCalculationType>
  <AveragingNumberSamples>4</AveragingNumberSamples>
  <AveragingSampleTimeSeconds>30</AveragingSampleTimeSeconds>
</RateModuleSettings>' as [Value], 
'2022-06-22' as [CreatedDate],
'Administrator' as [CreatedBy],
'2022-06-22' as [UpdatedDate],
'Administrator' [UpdatedBy],
@VolumeNetStandardRateSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@StandardVolumeTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
						target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
						target.[PointTemplateGuid] = source.[PointTemplateGuid],
						target.[UpdatedDate] = SYSDATETIMEOFFSET(),
						target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);



DECLARE @RateModuleVolumeNetStandardToPointTemplateGuid UNIQUEIDENTIFIER = '5D510AC7-8359-441F-B268-35DA3EB13613'

DECLARE @RateModuleVolumeNetStandardToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolumeNetStandardTagGuid) + '</TagGuid>
	  <ModuleParameter>Value</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolumeNetStandardRateTagGuid) + '</TagGuid>
	  <ModuleParameter>Rate</ModuleParameter>
	</TagToModule>
  </TagToModules>
  <PropertyToModules>
	<PropertyToModule>
	  <PropertyGuid>' + CONVERT(NVARCHAR(36),@VolumeNetStandardRateSettingsPointTemplatePropertyGuid) + '</PropertyGuid>
	  <PropertyName>Settings</PropertyName>
	</PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'


MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Rate Volume Net Standard' as [ID],
4 as [Order],
@RateModuleVolumeNetStandardToPointTemplateData AS [ModuleToPointTemplateData],
@RateModuleVolumeNetStandardToPointTemplateGuid as [ModuleToPointTemplateGuid],
@StandardVolumeTemplateGuid as [PointTemplateGuid] ,
@RateModuleGuid as [ModuleGuid],
'2015-02-04' as [CreatedDate] ,
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
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




--Create VolumeTransferModuleSettings Property
--Note : VolumeTransferModuleSettings Value is xml serialization of FMBusinessObjects VolumeTransferModuleSettings 
DECLARE @MVolumeTransferModuleSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '2060F1CA-2E48-4C94-84FE-82988FFDA0A4'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(
	SELECT 'Volume Transfer Settings' as [ID],
	'FMBusinessObjects.DataObjects.VolumeTransferModuleSettings' as [ValueType],
	'<VolumeTransferModuleSettings xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	  <TransferVolumeMode>GrossObservedVolume</TransferVolumeMode>
	  <TransferAdvisoryTime>15</TransferAdvisoryTime>
	  <CurrentTransferVolumeMode>GrossObservedVolume</CurrentTransferVolumeMode>
	</VolumeTransferModuleSettings>' as [Value],
	'2022-06-22' as [CreatedDate],
	'Administrator' as [CreatedBy],
	'2022-06-22' as [UpdatedDate],
	'Administrator' [UpdatedBy],
	@MVolumeTransferModuleSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
	@StandardVolumeTemplateGuid as [PointTemplateGuid]
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

DECLARE @VolumeTransferModuleToPointTemplateGuid UNIQUEIDENTIFIER = 'D45CC437-8B8A-4F67-87C5-F05A58F97542'

DECLARE @VolumeTransferModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>adc283d1-a9d0-42e2-b69f-d867b91c9b80</TagGuid>
      <ModuleParameter>VolumeGrossObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>f663c1ba-0b0a-460c-830f-c86070d7bb42</TagGuid>
      <ModuleParameter>VolumeGrossObservedRate</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7a1db105-db02-4968-b17d-7d78b445aed9</TagGuid>
      <ModuleParameter>VolumeNetStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>85ca2c60-3b5e-4fa2-938a-2541ac207ec3</TagGuid>
      <ModuleParameter>VolumeNetStandardRate</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9f5cb5a7-1c07-477a-a53d-e69a625dc7e1</TagGuid>
      <ModuleParameter>TransferMode</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ba9621c5-d9fb-41b6-885e-a9451d54aba1</TagGuid>
      <ModuleParameter>TransferStatus</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>29da71c4-1a51-42ee-8dfb-c781be5c7b2e</TagGuid>
      <ModuleParameter>TransferTarget</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>30a2bb23-db8e-445a-be09-32f7f1132f25</TagGuid>
      <ModuleParameter>TransferStartGOV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>a8fde285-c978-407d-a994-797aadf58c13</TagGuid>
      <ModuleParameter>TransferStartNSV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>88f438f1-f4d2-4048-9c4b-d8a3b693c6df</TagGuid>
      <ModuleParameter>TransferStartVolume</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>85bd69c2-e54e-45af-ab06-acc17c1e8f76</TagGuid>
      <ModuleParameter>TransferTimeRemaining</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>4f24d174-d533-4cbb-9654-d7d6da332afc</TagGuid>
      <ModuleParameter>TransferTimeCompletion</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>06bf336d-97e8-4ae6-a092-332731acdf76</TagGuid>
      <ModuleParameter>TransferredGOV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>bb42bc34-e90a-4067-b89e-ffff9547f0e0</TagGuid>
      <ModuleParameter>TransferredNSV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>945926ed-c944-4d5f-a6b6-22a64a660b70</TagGuid>
      <ModuleParameter>TransferredVolume</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7595F7E2-1490-4D22-BC43-68D09A3DCFDF</TagGuid>
      <ModuleParameter>TransferDiscreteAlarm</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9c1b1656-8c94-4e25-8862-e6a3b1e8a902</TagGuid>
      <ModuleParameter>TransferStartTime</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d4e548ca-40da-4b79-a21a-c4d2eec6c737</TagGuid>
      <ModuleParameter>TransferStopTime</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>902ef38d-5516-4bab-951a-a0e0394d3dfc</TagGuid>
      <ModuleParameter>TransferVolumeTarget</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>2060f1ca-2e48-4c94-84fe-82988ffda0a4</PropertyGuid>
      <PropertyName>VolumeTransferSettings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'


--Create VcfSettings Property
--Note : VcfSettings Value is xml serialization of FMBusinessObjects VcfModuleSettings 
DECLARE @StandardVolumeVcfSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '2B1CFEAD-A4DC-40E0-A53B-19005075463B'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Volume Correction' as [ID],
'FMBusinessObjects.DataObjects.VcfModuleSettings' as [ValueType],
'<VcfModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <DensityPressure>
	<EngineeringUnitsType>FmuPressure</EngineeringUnitsType>
	<Value>0</Value>
  </DensityPressure>
  <AlternateTemperature>
	<EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
	<Value>0</Value>
  </AlternateTemperature>
  <BaseTemperature>
	<EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
	<Value>60</Value>
  </BaseTemperature>
  <AlternateBasePressure>
	<EngineeringUnitsType>FmuPressure</EngineeringUnitsType>
	<Value>0</Value>
  </AlternateBasePressure>
  <K>
	<double>0</double>
	<double>0</double>
	<double>0</double>
	<double>0</double>
	<double>0</double>
  </K>
  <Alpha>0</Alpha>
  <UseProductObservedDensity>false</UseProductObservedDensity>
  <UseHydrometerCorrection>false</UseHydrometerCorrection>
  <ForceVcfTo4Digits>false</ForceVcfTo4Digits>
  <CorrectionMethodType>CORR_ASTM_COMM_2004</CorrectionMethodType>
  <CorrectionMethodSpecific>CORR_REFINED_PRODUCTS</CorrectionMethodSpecific>
</VcfModuleSettings>' as [Value], 
'2015-02-04' as [CreatedDate],
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
'Administrator' [UpdatedBy],
@StandardVolumeVcfSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@StandardVolumeTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Volume Transfer' as [ID],
5 as [Order],
@VolumeTransferModuleToPointTemplateData AS [ModuleToPointTemplateData],
@VolumeTransferModuleToPointTemplateGuid as [ModuleToPointTemplateGuid],
@StandardVolumeTemplateGuid as [PointTemplateGuid] ,
@VolumeTransferModuleGuid as [ModuleGuid],
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


DECLARE @StandardVolumeVcfModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeTemperatureProductTagGuid) + '</TagGuid>
	  <ModuleParameter>TemperatureProduct</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeTemperatureDensityTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>TemperatureDensity</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeTemperatureVaporTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>TemperatureVapor</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeDensityProductStandardTagGuid) + '</TagGuid>
	  <ModuleParameter>DensityStandard</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeDensityProductObservedTagGuid) + '</TagGuid>
	  <ModuleParameter>DensityObserved</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumePressureVaporTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>PressureVapor</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolCorForTempTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>VolumeCorrectionForTemperature</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolCorForPressTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>VolumeCorrectionForPressure</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolCorForPressTempTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>VolumeCorrectionForTemperatureandPressure</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolumeCorrectionFactorTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>VolumeCorrectionFactor</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVolumeCorrectionFactorUnroundedTagGuid) + '</TagGuid>
	  <ModuleParameter>VolumeCorrectionFactorUnrounded</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeAPICorrectionErrorTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>APICorrectionError</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeDensityInAirTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>DensityObservedInAir</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeStdDensityInAirTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>DensityStandardInAir</ModuleParameter>
	</TagToModule>
	<TagToModule>
	  <TagGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeDensityGaugeProductTemplateTagGuid) + '</TagGuid>
	  <ModuleParameter>DensityGauge</ModuleParameter>
	</TagToModule>
  </TagToModules>
  <PropertyToModules>
	<PropertyToModule>
	  <PropertyGuid>' + CONVERT(NVARCHAR(36),@StandardVolumeVcfSettingsPointTemplatePropertyGuid) + '</PropertyGuid>
	  <PropertyName>VcfSettings</PropertyName>
	</PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @TotalizerModuleToPointTemplateGuid UNIQUEIDENTIFIER = '2E34B3DE-8899-480C-8AD6-D7FB17FA507D'
DECLARE @StandardVolumeVcfModuleToPointTemplateGuid UNIQUEIDENTIFIER = '0F22DB9F-BC0F-415B-9230-572CC68CC832'

DECLARE @TotalizerModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>ADC283D1-A9D0-42E2-B69F-D867B91C9B80</TagGuid>
      <ModuleParameter>VolumeGrossObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7A1DB105-DB02-4968-B17D-7D78B445AED9</TagGuid>
      <ModuleParameter>VolumeNetStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9CE844BE-4A53-4E31-BDE1-95EDE4D7D1C0</TagGuid>
      <ModuleParameter>PulseMeterNumberOfRollOvers</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>1D4C53CD-41E2-49E3-8AAF-7A6A247A779A</TagGuid>
      <ModuleParameter>PulseMeterLastValue</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>C878E5A7-3EDF-470B-94F2-415D66A9A3A2</TagGuid>
      <ModuleParameter>PulseMeterCurrentValue</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>C9454F0E-A573-4D37-94FF-9C7E8BDD8DEE</TagGuid>
      <ModuleParameter>PulseMeterVolumePerPulse</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>6A5EC2CA-E2C9-4BDA-AD07-0B56B2372DA5</TagGuid>
      <ModuleParameter>PulseMeterRollOverAmount</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>1385627C-5BD4-4381-95F3-50D8C0C44507</TagGuid>
      <ModuleParameter>PulseMeterLastReadWasRollOver</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>81E007E0-F325-438C-BBC4-B7667173EC6E</TagGuid>
      <ModuleParameter>VolumeCorrectionFactor</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules />
</ModuleToPointTemplateData>'

MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Totalizer' as [ID],
2 as [Order],
@TotalizerModuleToPointTemplateData AS [ModuleToPointTemplateData],
@TotalizerModuleToPointTemplateGuid as [ModuleToPointTemplateGuid],
@StandardVolumeTemplateGuid as [PointTemplateGuid] ,
@TotalizerModuleGuid as [ModuleGuid],
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


MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Volume Correction' as [ID],
1 as [Order],
@StandardVolumeVcfModuleToPointTemplateData AS [ModuleToPointTemplateData],
@StandardVolumeVcfModuleToPointTemplateGuid as [ModuleToPointTemplateGuid],
@StandardVolumeTemplateGuid as [PointTemplateGuid] ,
@VcfModuleGuid as [ModuleGuid],
'2023-11-10' as [CreatedDate] ,
'Administrator' as [CreatedBy],
'2022-11-10' as [UpdatedDate],
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
if(0 = (SELECT COUNT(*) FROM map.tblEntityPointTemplateToSite WHERE PointTemplateGuid = @StandardVolumeTemplateGuid AND SiteGuid = @StandardVolumeSiteGuid))
BEGIN
		INSERT INTO map.tblEntityPointTemplateToSite ([PointTemplateToSiteGuid],[PointTemplateGuid],[SiteGuid],[AssignedFromSiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('26F5A603-6950-4D28-B975-29F37A79DC50',@StandardVolumeTemplateGuid,@StandardVolumeSiteGuid,@StandardVolumeSiteGuid,'2022-06-22','Administrator','2022-06-22','Administrator')
END 

-- Clear out no-longer-existing MovementNodeCommand type points (Transfer Command)
delete from map.tblPointAccessGroupToTag where tagguid in (select pointtemplatetagguid from tblPointTemplateTag where PointTemplateGuid = @StandardVolumeTemplateGuid and ValueType = 'FMBusinessObjects.DataObjects.CodedVariables.MovementNodeCommand')

delete from tblPointTemplateTag where PointTemplateGuid = @StandardVolumeTemplateGuid and ValueType = 'FMBusinessObjects.DataObjects.CodedVariables.MovementNodeCommand'
