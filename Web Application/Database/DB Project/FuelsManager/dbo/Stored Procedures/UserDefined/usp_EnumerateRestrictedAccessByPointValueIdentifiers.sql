/*****
*
* To improve performance usp_EnumerateRestrictedAccessByPointValueIdentifiers only returns results if
*  [View] = 0 OR Modify = 0  OR ExceedRange = 0 OR Override = 0 for a given PointValueIdentifier
* 
*****/
CREATE PROCEDURE [dbo].[usp_EnumerateRestrictedAccessByPointValueIdentifiers]
(
	@SiteGuid UniqueIdentifier,
	@UserGuid UniqueIdentifier,
	@PointValueIdentifiers utt_PointValueIdentifier READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		-- List of PointSettings with Attribute FMExposedSetting
		-- ENUM PointValueType.ValueType = 2 (Point)
				DECLARE @PointSettings Table(ID nvarchar(30), PropertyID nvarchar(30))

		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Point ID', 'PointId')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Point Description', 'Description')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Point Enabled', 'Enabled')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Product', 'ProductID')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Product Description', 'ProductDescription')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Site Name', 'SiteID')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Site Number', 'SiteNumber')

		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Level Units', 'LevelUnits')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Level Min', 'LevelMin')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Level Max', 'LevelMax')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Temperature Units', 'TemperatureUnits')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Temp Min', 'TempMin')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Temp Max', 'TempMax')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Volume Units', 'VolumeUnits')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Volume Min', 'VolumeMin')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Volume Max', 'VolumeMax')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Mass Units', 'MassUnits')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Mass Min', 'MassMin')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Mass Max', 'MassMax')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Density Units', 'DensityUnits')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Density Min', 'DensityMin')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Density Max', 'DensityMax')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Standard Density Units', 'StandardDensityUnits')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Standard Density Min', 'StandardDensityMin')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Standard Density Max', 'StandardDensityMax')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Volume Rate Units', 'VolumeRateUnits')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Pressure Units', 'PressureUnits')
		INSERT INTO @PointSettings (ID, PropertyID) VALUES ('Point Detail', 'PointDetailDrawingID')




		-- List of PointProperty with Attribute FMExposedSetting
		-- ENUM PointValueType.ValueType = 1 (SETTING)
		DECLARE @PropertySettings Table(SettingID nvarchar(30), ID nvarchar(60), PropertyID nvarchar(60))

		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Product Table', 'ProductTable')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Bottoms Table', 'BottomsTable')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Solids Table', 'SolidsTable')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Strap Temperauture', 'StrapTemperature')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Strap Density', 'StrapDensity')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Roof Landing Height', 'RoofLandingHeight')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Roof Floating Height', 'RoofFloatingHeight')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Datum Height', 'DatumHeight')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Roof Type', 'RoofType')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Strap Table', 'Roof Mass', 'RoofMass')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Tank Command Settings', 'Movement Alarm Differential', 'MovementAlarmDifferential')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Tank Transfer Settings', 'Transfer Advisory Time', 'TransferAdvisoryTime')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Installation Date', 'TankInstallationDate')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'CST Manufacture Date', 'CSTManufactureDate')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'CST Commission Date', 'CSTCommissionDate')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Geometry', 'TankGeometryEnumText')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Volume', 'TankVolume')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Height', 'TankHeight')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Radius', 'TankRadius')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Shell Thickness', 'TankShellThickness')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Lining Material', 'TankLiningMaterial')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Material', 'TankMaterialEnumText')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Expansion Coefficient', 'TankExpansionCoefficient')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Cathodic Protection Supported', 'CathodicProtectionSupported')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Overfill Protection Supported', 'OverfillProtectionSupported')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Spill Protection Supported', 'SpillProtectionSupported')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Tank Shell Insulated', 'TankShellInsulated')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Area Coefficient', 'AreaCoefficient')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'CST Manufacturer', 'CSTManufacturerName')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'CST Serial Number', 'CSTSerialNumber')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'CST Location Name', 'CSTLocationName')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'CST Latitude', 'CSTLatitude')
	   INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'CST Longitude', 'CSTLongitude')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Latitude Degrees', 'LatitudeDegrees')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Latitude Minutes', 'LatitudeMinutes')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Vessel', 'Latitude Seconds', 'LatitudeSeconds')

		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('VcfModuleSettings', 'Correction Commodity/Table', 'CorrectionCommodityOrTable')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('VcfModuleSettings', 'Correction Revision', 'CorrectionStandardRevision')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('VcfModuleSettings', 'Correction Standard/Organization', 'CorrectionStandardOrOrganization')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('VcfModuleSettings', 'Temperature Standard', 'BaseTemperature')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Leak Detection Settings', 'Gauge Type', 'GaugeType')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Leak Detection Settings', 'Leak Analysis Method', 'LeakAnalysisMethodString')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('Leak Detection Settings', 'Leak Analysis Type', 'LeakAnalysisTypeString')

		-- Movement Module Settings
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'PointId', 'PointId')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'CreatedBy', 'CreatedBy')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'Status', 'Status')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferStatus', 'TransferStatus')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'Product', 'Product')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferStartTime', 'TransferStartTime')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferStopTime', 'TransferStopTime')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'InitiationCount', 'InitiationCount')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'LevelProduct', 'LevelProduct')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'LevelWater', 'LevelWater')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'MassLiquid', 'MassLiquid')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TemperatureAmbient', 'TemperatureAmbient')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TemperatureDensity', 'TemperatureDensity')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TemperatureProduct', 'TemperatureProduct')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'DensityProductObserved', 'DensityProductObserved')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'DensityProductinAir', 'DensityProductinAir')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'DensityProductStandard', 'DensityProductStandard')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'DensityProductStandardinAir', 'DensityProductStandardinAir')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeCorrectionFactor', 'VolumeCorrectionFactor')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeGrossObserved', 'VolumeGrossObserved')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeGrossStandard', 'VolumeGrossStandard')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeNetStandard', 'VolumeNetStandard')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeTotalObserved', 'VolumeTotalObserved')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeWater', 'VolumeWater')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeRoofCorrection', 'VolumeRoofCorrection')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TankShellCorrection', 'TankShellCorrection')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeGrossObservedRate', 'VolumeGrossObservedRate')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeNetStandardRate', 'VolumeNetStandardRate')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeTotalObservedRate', 'VolumeTotalObservedRate')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData01', 'UserData01')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData02', 'UserData02')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData03', 'UserData03')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData04', 'UserData04')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData05', 'UserData05')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData06', 'UserData06')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData07', 'UserData07')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData08', 'UserData08')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData09', 'UserData09')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'UserData10', 'UserData10')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferredGOV', 'TransferredGOV')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferredNSV', 'TransferredNSV')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferredVolumeWater', 'TransferredVolumeWater')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferStartVolume', 'TransferStartVolume')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferMode', 'TransferMode')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferTarget', 'TransferTarget')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferLevelTarget', 'TransferLevelTarget')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferVolumeTarget', 'TransferVolumeTarget')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferTimeRemaining', 'TransferTimeRemaining')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferTimeCompleton', 'TransferTimeCompleton')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'Deviation', 'Deviation')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'PercentDeviation', 'PercentDeviation')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartTemperatureAmbient', 'StartTemperatureAmbient')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartDensityProductObserved', 'StartDensityProductObserved')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartDensityProductinAir', 'StartDensityProductinAir')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartDensityProductStandard', 'StartDensityProductStandard')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartDensityProductStandardinAir', 'StartDensityProductStandardinAir')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferStartLevel', 'TransferStartLevel')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartLevelWater', 'StartLevelWater')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartMassLiquid', 'StartMassLiquid')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartTankShellCorrection', 'StartTankShellCorrection')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartTemperatureDensity', 'StartTemperatureDensity')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartTemperatureProduct', 'StartTemperatureProduct')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartVolumeCorrectionFactor', 'StartVolumeCorrectionFactor')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferStartGOV', 'TransferStartGOV')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartVolumeRoofCorrection', 'StartVolumeRoofCorrection')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartVolumeGrossStandard', 'StartVolumeGrossStandard')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferStartNSV', 'TransferStartNSV')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartVolumeTotalObserved', 'StartVolumeTotalObserved')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferStartWaterVolume', 'TransferStartWaterVolume')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningTemperatureAmbient', 'OpeningTemperatureAmbient')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningDensityProductObserved', 'OpeningDensityProductObserved')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningDensityProductinAir', 'OpeningDensityProductinAir')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningDensityProductStandard', 'OpeningDensityProductStandard')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningDensityProductStandardinAir', 'OpeningDensityProductStandardinAir')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningLevelProduct', 'OpeningLevelProduct')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningLevelWater', 'OpeningLevelWater')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningMassLiquid', 'OpeningMassLiquid')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningTankShellCorrection', 'OpeningTankShellCorrection')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningTemperatureDensity', 'OpeningTemperatureDensity')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningTemperatureProduct', 'OpeningTemperatureProduct')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningVolumeCorrectionFactor', 'OpeningVolumeCorrectionFactor')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningVolumeGrossObserved', 'OpeningVolumeGrossObserved')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningVolumeGrossStandard', 'OpeningVolumeGrossStandard')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningVolumeNetStandard', 'OpeningVolumeNetStandard')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningVolumeRoofCorrection', 'OpeningVolumeRoofCorrection')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningVolumeTotalObserved', 'OpeningVolumeTotalObserved')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningVolumeWater', 'OpeningVolumeWater')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'TransferDirection', 'TransferDirection')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'IndividualNodeControl', 'IndividualNodeControl')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'Comment', 'Comment')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OrderNumber', 'OrderNumber')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'PlannedStartTime', 'PlannedStartTime')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'Type', 'Type')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'PercentBSW', 'PercentBSW')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartPercentBSW', 'StartPercentBSW')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningPercentBSW', 'OpeningPercentBSW')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'VolumeBSW', 'VolumeBSW')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'StartVolumeBSW', 'StartVolumeBSW')
		INSERT INTO @PropertySettings (SettingID, ID, PropertyID) VALUES ('MovementData', 'OpeningVolumeBSW', 'OpeningVolumeBSW')


		-- Get Point Access Groups assigned to User
		DECLARE @PointAccessGroupGuidTable TABLE(PointAccessGroupGuid UniqueIdentifier)

		INSERT INTO @PointAccessGroupGuidTable SELECT DISTINCT pagtug.PointAccessGroupGuid FROM map.tblUserToGroup utg
		INNER JOIN map.tblPointAccessGroupToUserGroup pagtug ON pagtug.UserGroupGuid = utg.GroupGuid
		INNER JOIN dbo.tblPointAccessGroup pag ON pag.PointAccessGroupGuid = pagtug.PointAccessGroupGuid AND pag.SiteGuid = utg.SiteGuid
		WHERE utg.SiteGuid = @SiteGuid AND utg.UserGuid = @UserGuid

		-- Get Points assigned to Point Access Groups
		CREATE TABLE tempdb.#PointTable (
				PointGuid UniqueIdentifier,
				PointAccessGroupGuid UniqueIdentifier
		 )
 
		 INSERT INTO #PointTable SELECT DISTINCT PointGuid, PointAccessGroupGuid FROM
		  (SELECT p.PointGuid, pagtpt.PointAccessGroupGuid FROM dbo.tblPoint p
			INNER JOIN map.tblPointAccessGroupToPointTemplate pagtpt ON pagtpt.PointTemplateGuid = p.PointTemplateGuid
			INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtpt.PointAccessGroupGuid
			WHERE p.SiteGuid = @SiteGuid
			UNION
			SELECT p.PointGuid, pagtp.PointAccessGroupGuid FROM dbo.tblPoint p
			INNER JOIN map.tblPointAccessGroupToPoint pagtp ON pagtp.PointGuid = p.PointGuid
			INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtp.PointAccessGroupGuid
			WHERE p.SiteGuid = @SiteGuid) s
   
		-- Get Value Information		
		CREATE TABLE tempdb.#ValueTable (
				PointValueGuid UniqueIdentifier,
				PointTemplateValueGuid UniqueIdentifier,
				PointValuePropertyId nvarchar(50),
				PointValueType tinyint,
				PointGuid UniqueIdentifier,
				PointTemplateGuid UniqueIdentifier,
				[View] bit,
				Modify bit,
				ExceedRange bit,
				Override bit
		)
		
		CREATE NONCLUSTERED INDEX [IX_ValueTable_PointValueIdentity] ON [tempdb].[#ValueTable](PointValueType ASC, PointValueGuid ASC, PointValuePropertyId ASC)

		-- Insert TAG values
		INSERT INTO #ValueTable
		SELECT pvi.Guid as PointValueGuid, 
		CASE WHEN pt.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' THEN NULL
		ELSE pt.PointTemplateTagGuid
		END as PointTemplateValueGuid,
		'', 0, p.PointGuid, p.PointTemplateGuid, 0, 0, 0, 00
		FROM @PointValueIdentifiers pvi
		INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = pvi.Guid
		INNER JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid 
		WHERE pvi.ValueType = 0;

		-- Insert Point values
		INSERT INTO #ValueTable
		SELECT pvi.Guid as PointValueGuid, p.PointTemplateGuid as PointTemplateValueGuid, pvi.PropertyId, 2, p.PointGuid, p.PointTemplateGuid, 0, 0, 0, 0 
		FROM @PointValueIdentifiers pvi
		INNER JOIN dbo.tblPoint p ON p.PointGuid = pvi.Guid
		WHERE pvi.ValueType = 2;

		-- Insert Settings values
		INSERT INTO #ValueTable
		SELECT pvi.Guid as PointValueGuid, pp.PointTemplatePropertyGuid as PointTemplateValueGuid, pvi.PropertyId, 1, p.PointGuid, p.PointTemplateGuid, 0, 0, 0, 0 
		FROM @PointValueIdentifiers pvi
		INNER JOIN dbo.tblPointProperty pp ON pp.PointPropertyGuid = pvi.Guid
		INNER JOIN dbo.tblPoint p ON p.PointGuid = pp.PointGuid 
		WHERE pvi.ValueType = 1;



		-- Update Value Information with Point Template Tag Items
		WITH ValueUpdates AS
		(SELECT vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType,
		SUM(CASE WHEN pagtt.[View] IS NULL OR pagtt.[View] = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS [View],
		SUM(CASE WHEN pagtt.Modify IS NULL OR pagtt.Modify = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS Modify,
		SUM(CASE WHEN pagtt.ExceedRange IS NULL OR pagtt.ExceedRange = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS ExceedRange,
		SUM(CASE WHEN pagtt.Override IS NULL OR pagtt.Override = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS Override
		FROM #ValueTable vt
		INNER JOIN #PointTable pt ON pt.PointGuid = vt.PointGuid
		LEFT JOIN map.tblPointAccessGroupToTag pagtt ON 0 = vt.PointValueType AND pagtt.TagGuid = vt.PointTemplateValueGuid AND pagtt.PointAccessGroupGuid = pt.PointAccessGroupGuid
		WHERE vt.PointValueType = 0
		GROUP BY vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType)
		UPDATE #ValueTable
		SET #ValueTable.[View] = CASE WHEN vu.[View] > 0 AND #ValueTable.PointTemplateValueGuid IS NOT NULL THEN CAST(1 AS BIT) ELSE #ValueTable.[View] END,
		#ValueTable.Modify = CASE WHEN vu.Modify > 0 AND #ValueTable.PointTemplateValueGuid IS NOT NULL THEN CAST(1 AS BIT) ELSE #ValueTable.Modify END,
		#ValueTable.ExceedRange = CASE WHEN vu.ExceedRange > 0 AND #ValueTable.PointTemplateValueGuid IS NOT NULL THEN CAST(1 AS BIT) ELSE #ValueTable.ExceedRange END,
		#ValueTable.Override = CASE WHEN vu.Override > 0  AND #ValueTable.PointTemplateValueGuid IS NOT NULL THEN CAST(1 AS BIT) ELSE #ValueTable.Override END
		FROM ValueUpdates vu WHERE #ValueTable.PointValueGuid = vu.PointValueGuid AND #ValueTable.PointValuePropertyId = vu.PointValuePropertyId AND #ValueTable.PointValueType = vu.PointValueType;

		-- Update Value Information with Point Tag Items
		WITH ValueUpdates AS
		(SELECT vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType,
		SUM(CASE WHEN pagtpt.[View] IS NULL OR pagtpt.[View] = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS [View],
		SUM(CASE WHEN pagtpt.Modify IS NULL OR pagtpt.Modify = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS Modify,
		SUM(CASE WHEN pagtpt.ExceedRange IS NULL OR pagtpt.ExceedRange = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS ExceedRange,
		SUM(CASE WHEN pagtpt.Override IS NULL OR pagtpt.Override = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS Override
		FROM #ValueTable vt
		INNER JOIN #PointTable pt ON pt.PointGuid = vt.PointGuid
		LEFT JOIN map.tblPointAccessGroupToPointTag pagtpt ON 0 = vt.PointValueType AND pagtpt.TagGuid = vt.PointValueGuid AND pagtpt.PointAccessGroupGuid = pt.PointAccessGroupGuid
		WHERE vt.PointValueType = 0
		GROUP BY vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType)
		UPDATE #ValueTable
		SET #ValueTable.[View] = CASE WHEN vu.[View] > 0 AND #ValueTable.PointTemplateValueGuid IS NULL THEN CAST(1 AS BIT) ELSE #ValueTable.[View] END,
		#ValueTable.Modify = CASE WHEN vu.Modify > 0 AND #ValueTable.PointTemplateValueGuid IS NULL THEN CAST(1 AS BIT) ELSE #ValueTable.Modify END,
		#ValueTable.ExceedRange = CASE WHEN vu.ExceedRange > 0 AND #ValueTable.PointTemplateValueGuid IS NULL THEN CAST(1 AS BIT) ELSE #ValueTable.ExceedRange END,
		#ValueTable.Override = CASE WHEN vu.Override > 0  AND #ValueTable.PointTemplateValueGuid IS NULL THEN CAST(1 AS BIT) ELSE #ValueTable.Override END
		FROM ValueUpdates vu WHERE #ValueTable.PointValueGuid = vu.PointValueGuid AND #ValueTable.PointValuePropertyId = vu.PointValuePropertyId AND #ValueTable.PointValueType = vu.PointValueType;


		-- Update value information with Point Setting items
		WITH ValueUpdates AS
		(SELECT vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType,
		SUM(CASE WHEN pagteps.[View] IS NULL OR pagteps.[View] = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS [View],
		SUM(CASE WHEN pagteps.Modify IS NULL OR pagteps.Modify = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS Modify,
		1 AS ExceedRange,
		1 AS Override
		FROM #ValueTable vt
		INNER JOIN #PointTable pt ON pt.PointGuid = vt.PointGuid
		LEFT JOIN map.tblPointAccessGroupToExposedPointSetting pagteps ON 2 = vt.PointValueType AND pagteps.PointSettingGuid = vt.PointTemplateValueGuid AND pagteps.PointAccessGroupGuid = pt.PointAccessGroupGuid AND vt.PointValuePropertyId = pagteps.PropertyID
		WHERE vt.PointValueType = 2
		GROUP BY vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType)
		UPDATE #ValueTable
		SET #ValueTable.[View] = CASE WHEN vu.[View] > 0 THEN CAST(1 AS BIT) ELSE #ValueTable.[View] END,
		#ValueTable.Modify = CASE WHEN vu.Modify > 0 THEN CAST(1 AS BIT) ELSE #ValueTable.Modify END,
		#ValueTable.ExceedRange = CASE WHEN vu.ExceedRange > 0 THEN CAST(1 AS BIT) ELSE #ValueTable.ExceedRange END,
		#ValueTable.Override = CASE WHEN vu.Override > 0 THEN CAST(1 AS BIT) ELSE #ValueTable.Override END
		FROM ValueUpdates vu WHERE #ValueTable.PointValueGuid = vu.PointValueGuid AND #ValueTable.PointValuePropertyId = vu.PointValuePropertyId AND #ValueTable.PointValueType = vu.PointValueType;

		-- Update value information for Point Property items
		WITH ValueUpdates AS
		(SELECT vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType,
		SUM(CASE WHEN pagteps.[View] IS NULL OR pagteps.[View] = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS [View],
		SUM(CASE WHEN pagteps.Modify IS NULL OR pagteps.Modify = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS Modify,
		1 AS ExceedRange,
		1 AS Override
		FROM #ValueTable vt
		INNER JOIN #PointTable pt ON pt.PointGuid = vt.PointGuid
		LEFT JOIN map.tblPointAccessGroupToExposedPropertySetting pagteps ON 1 = vt.PointValueType AND pagteps.PointSettingGuid = vt.PointTemplateValueGuid AND pagteps.PointAccessGroupGuid = pt.PointAccessGroupGuid AND vt.PointValuePropertyId = pagteps.PropertyID
		WHERE vt.PointValueType = 1
		GROUP BY vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType)
		UPDATE #ValueTable
		SET #ValueTable.[View] = CASE WHEN vu.[View] > 0 THEN CAST(1 AS BIT) ELSE #ValueTable.[View] END,
		#ValueTable.Modify = CASE WHEN vu.Modify > 0 THEN CAST(1 AS BIT) ELSE #ValueTable.Modify END,
		#ValueTable.ExceedRange = CASE WHEN vu.ExceedRange > 0 THEN CAST(1 AS BIT) ELSE #ValueTable.ExceedRange END,
		#ValueTable.Override = CASE WHEN vu.Override > 0 THEN CAST(1 AS BIT) ELSE #ValueTable.Override END
		FROM ValueUpdates vu WHERE #ValueTable.PointValueGuid = vu.PointValueGuid AND #ValueTable.PointValuePropertyId = vu.PointValuePropertyId AND #ValueTable.PointValueType = vu.PointValueType;

		-- Remove access if it a ENUM [PointValueType.Setting] and not in FMExposedSetting list
		UPDATE  v
		SET v.Modify = 0,v.[View] = 0
		FROM #ValueTable v
		WHERE PointValueType = 1 
		AND NOT EXISTS (SELECT 1 FROM @PropertySettings p WHERE v.PointValuePropertyId =  p.PropertyID )

		-- Remove access if it is a ENUM [PointValueType.Point] and not in FMExposedSetting list
		UPDATE  v
		SET v.Modify = 0,v.[View] = 0
		FROM #ValueTable v
		WHERE PointValueType = 2 
		AND NOT EXISTS (SELECT 1 FROM @PointSettings p WHERE v.PointValuePropertyId =  p.PropertyID )

		-- Only return items if access is restricted
		SELECT vt.PointValueGuid, vt.PointValuePropertyId, vt.PointValueType, vt.[View], vt.Modify, vt.ExceedRange, vt.Override FROM #ValueTable vt
		WHERE vt.[View] = CAST(0 AS BIT) OR vt.Modify = CAST(0 AS BIT) OR vt.ExceedRange = Cast(0 AS BIT) OR vt.Override = CAST(0 AS BIT) 
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;      
				      
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: usp_GetPointValueAccessEnumerateByPointValueIdentifiers' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END