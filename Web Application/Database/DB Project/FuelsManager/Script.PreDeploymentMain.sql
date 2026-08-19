/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/*
--IF 'DEFENSE' = '$(FMSolution)'
IF '$(FMSolution)' = 'DEFENSE'
BEGIN
	-- CHECKS IF DATABASE EXISTS BY PINGING dbo.tblSites table
	IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_CATALOG='tblSites')
		:r .\FMScripts\Defense\PreScripts\Script.Defense.Pre.MigrationPatch.00001.sql
END
*/

-- Remove existing data from tracking tables that are being removed from synchronization to avoid triggering data loss warnings.
:r .\FMScripts\PreScripts\Script.IncrementalDataMaintenance.sql


-- Make sure CLR support is turned on
Exec sp_configure 'clr enabled', 1
GO
reconfigure
GO
sp_dbcmptlevel '$(DatabaseName)', '110';-- 110 = SQL 2012
GO
ALTER DATABASE [$(DatabaseName)] SET TRUSTWORTHY ON
GO
IF EXISTS (SELECT name FROM master.sys.server_principals WHERE name = 'sa')
				Begin 
					ALTER AUTHORIZATION ON DATABASE::[$(DatabaseName)] to sa
				end
GO


-- Check for need to upgrade MinorCorrectionMethod to LocalMinorCorrectionMethodIndex
create table #UpgradeNotifications
(
	UpgradeType nvarchar(50),
	Upgrade bit
	)

if exists (select * 
			from sys.tables t 
				inner join sys.columns c on t.object_id = c.object_id
				inner join sys.schemas s on t.schema_id = s.schema_id
			where s.name = 'dbo'
				and t.name = 'tblProducts'
				and c.name = 'MinorCorrectionMethod')
begin
	insert into #UpgradeNotifications values('UpgradeMinorCorrection',1)
end


-- rename PK in tracking tables to match naming standard
IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblSRMAdaptorIATAToSite'))   
				AND type = 'PK'
				AND name = 'PK_track_tblSRMAdaptorIATAToSite' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblSRMAdaptorIATAToSite', 'PK_track_tblSRMAdaptorIATAToSite_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblProcessVariableFlowControlledAdditiveInputPermissive'))   
				AND type = 'PK'
				AND name = 'PK_track_tblProcessVariableFlowControlledAdditiveInputPermissive' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblProcessVariableFlowControlledAdditiveInputPermissive', 'PK_track_tblProcessVariableFlowControlledAdditiveInputPermissive_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblProcessVariableFlowControlledAdditiveOutputPermissive'))   
				AND type = 'PK'
				AND name = 'PK_track_tblProcessVariableFlowControlledAdditiveOutputPermissive' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblProcessVariableFlowControlledAdditiveOutputPermissive', 'PK_track_tblProcessVariableFlowControlledAdditiveOutputPermissive_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblProcessVariableExternalMeterOutputPermissive'))   
				AND type = 'PK'
				AND name = 'PK_track_tblProcessVariableExternalMeterOutputPermissive' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblProcessVariableExternalMeterOutputPermissive', 'PK_track_tblProcessVariableExternalMeterOutputPermissive_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblProcessVariableExternalMeterInputPermissive'))   
				AND type = 'PK'
				AND name = 'PK_track_tblProcessVariableExternalMeterInputPermissive' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblProcessVariableExternalMeterInputPermissive', 'PK_track_tblProcessVariableExternalMeterInputPermissive_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblProcessVariableOffloadExternalMeter'))   
				AND type = 'PK'
				AND name = 'PK_track_tblProcessVariableOffloadExternalMeter' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblProcessVariableOffloadExternalMeter', 'PK_track_tblProcessVariableOffloadExternalMeter_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblQualificationPersonLicenseToStation'))   
				AND type = 'PK'
				AND name = 'PK_track_tblQualificationPersonLicenseToStation' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblQualificationPersonLicenseToStation', 'PK_track_tblQualificationPersonLicenseToStation_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblDispatchGrid'))   
				AND type = 'PK'
				AND name = 'PK_track_tblDispatchGrid' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblDispatchGrid', 'PK_track_tblDispatchGrid_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblMobileDevice'))   
				AND type = 'PK'
				AND name = 'PK_track_tblMobileDevice' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblMobileDevice', 'PK_track_tblMobileDevice_ChangeIndex'
END   

IF EXISTS ( SELECT 1  
				FROM sys.objects  
				WHERE parent_object_id = (OBJECT_ID('track.tblApplicationStringToPointCategory'))   
				AND type = 'PK'
				AND name = 'PK_track_tblApplicationStringToPointCategory' )
BEGIN
	EXEC sp_rename 'track.PK_track_tblApplicationStringToPointCategory', 'PK_track_tblApplicationStringToPointCategory_ChangeIndex'
END   


-- If upgrading from FMV10 we need to convert the menu items and rights.  
-- We also need to delete tracking information (they don't use sync) to avoid loss of data warnings
-- need to convert [map].[tblTrendPenToTrend], tblpoint and tblproduct

IF EXISTS (SELECT * 
   FROM INFORMATION_SCHEMA.TABLES 
   WHERE TABLE_SCHEMA = 'dbo' 
   AND TABLE_NAME = 'tblVersion') 
BEGIN
	IF (( SELECT TOP 1 version 
		from tblVersion
		ORDER BY VersionIndex DESC ) = '10.0.0.0' )
	BEGIN
		IF OBJECT_ID('map.tblGroupToRight_upgrade', 'U') IS NOT NULL 
			DROP TABLE map.tblGroupToRight_upgrade; 

		SELECT * 
		INTO map.tblGroupToRight_upgrade
		FROM map.tblGroupToRight

		-- we have to delete the records because some rights are going to be deleted and will cause referential integrity errors
		DELETE FROM map.tblGroupToRight

		IF OBJECT_ID('dbo.tblMenuFavorites_upgrade', 'U') IS NOT NULL 
			DROP TABLE dbo.tblMenuFavorites_upgrade; 

		SELECT * 
		INTO dbo.tblMenuFavorites_upgrade
		FROM dbo.tblMenuFavorites

		DELETE FROM dbo.tblMenuFavorites

		IF OBJECT_ID('map.tblTrendPenToTrend_upgrade', 'U') IS NOT NULL 
			DROP TABLE map.tblTrendPenToTrend_upgrade; 

		SELECT * 
		INTO map.tblTrendPenToTrend_upgrade
		FROM map.tblTrendPenToTrend

		DELETE FROM map.tblTrendPenToTrend

	UPDATE tblPointTemplateProperty
	SET Value = CONVERT(XML,replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(CONVERT( VARCHAR(MAX),value,1), 'FMU_NODIM','FmuNodim'),'FM_SiteUnits','FmuNone'),'Fmu_Temp','FmuTemp'),'FMU_VOLUME','FmuVolume'), 'Fmu_Mass','FmuMass'), 'Fmu_Length','FmuLength'), 'Fmu_Density','FmuDensity'), 'Fmu_None','FmuNone'), 'Fmu_Pressure','FmuPressure'), 'Fmu_Massflow','FmuMassflow'), 'Fmu_Velocity','FmuVelocity'), 'Fmu_Volflow','FmuVolflow'), 1)

	UPDATE tblPointproperty
	SET Value = CONVERT(XML,replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(CONVERT( VARCHAR(MAX),value,1), 'FMU_NODIM','FmuNodim'),'FM_SiteUnits','FmuNone'),'Fmu_Temp','FmuTemp'),'FMU_VOLUME','FmuVolume'), 'Fmu_Mass','FmuMass'), 'Fmu_Length','FmuLength'), 'Fmu_Density','FmuDensity'), 'Fmu_None','FmuNone'), 'Fmu_Pressure','FmuPressure'), 'Fmu_Massflow','FmuMassflow'), 'Fmu_Velocity','FmuVelocity'), 'Fmu_Volflow','FmuVolflow'), 1)

	UPDATE tblModule
	SET ModuleData = CONVERT(XML,replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(CONVERT( VARCHAR(MAX),ModuleData,1), 'FMU_NODIM','FmuNodim'),'FM_SiteUnits','FmuNone'),'Fmu_Temp','FmuTemp'),'FMU_VOLUME','FmuVolume'), 'Fmu_Mass','FmuMass'), 'Fmu_Length','FmuLength'), 'Fmu_Density','FmuDensity'), 'Fmu_None','FmuNone'), 'Fmu_Pressure','FmuPressure'), 'Fmu_Massflow','FmuMassflow'), 'Fmu_Velocity','FmuVelocity'), 'Fmu_Volflow','FmuVolflow'), 1)

	UPDATE tblproducts
	SET VCFModuleSettings = CONVERT(XML,replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(CONVERT( VARCHAR(MAX),VCFModuleSettings,1), 'FMU_NODIM','FmuNodim'),'FM_SiteUnits','FmuNone'),'Fmu_Temp','FmuTemp'),'FMU_VOLUME','FmuVolume'), 'Fmu_Mass','FmuMass'), 'Fmu_Length','FmuLength'), 'Fmu_Density','FmuDensity'), 'Fmu_None','FmuNone'), 'Fmu_Pressure','FmuPressure'), 'Fmu_Massflow','FmuMassflow'), 'Fmu_Velocity','FmuVelocity'), 'Fmu_Volflow','FmuVolflow'), 1)

	UPDATE map.tblModuleToPointTemplate
	SET ModuleToPointTemplateData = CONVERT(XML, replace(replace(replace(CONVERT( VARCHAR(MAX),ModuleToPointTemplateData,1),'VolumeCorrectionFactorForTemperature','VolumeCorrectionForTemperature'),'VolumeCorrectionFactorForPressureAndTemperature','VolumeCorrectionForTemperatureandPressure'),'VolumeCorrectionFactorForPressure','VolumeCorrectionForPressure'), 1)
	WHERE ID = 'Volume Correction' OR ID = 'Standard Tank Calculator'

	
		-- delete all rows in the tracking tables to avoid problems with possible loss of data THIS ONLY APPLIES TO v10 sites since they don't use SYNC
		DECLARE @sqlCommand VARCHAR(3000);
		DECLARE @TableName VARCHAR(128);

		-- get a cursor with a list of table names and their record counts
		DECLARE MyCursor CURSOR FAST_FORWARD
		FOR SELECT 	'track.' + t.name 
		from sys.tables t
		JOIN sys.schemas s
		ON t.schema_id = s.schema_id
		WHERE s.name = 'track'
		AND t.type = 'U'


		OPEN MyCursor;

		FETCH NEXT FROM MyCursor INTO @TableName;

		-- for each table name in the cursor, delete all records from that table:
		WHILE @@FETCH_STATUS = 0
			BEGIN
				SET @sqlCommand = 'DELETE FROM ' + @TableName;
				EXEC (@sqlCommand);
				FETCH NEXT FROM MyCursor INTO @TableName;
			END;

		CLOSE MyCursor;
		DEALLOCATE MyCursor;

		IF EXISTS( SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4067 ) 
		BEGIN 
			DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4067
		END

	END
END


