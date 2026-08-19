/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/*
************************************
	DATA INSERT SECTION:
************************************
*/
PRINT '***************************************************************************'
PRINT '** PREPARE INITIALIZE / UPDATE STATIC REFERENCE DATA (i.e. lookup data)  **'
PRINT '***************************************************************************'
PRINT '** NOTE: Changes to existing data will appear below.  If a lookup index changed; '
PRINT '         You are responsible for adding the appropriate statements to the '
PRINT '         Script.IncrementalDataMaintenance.sql deployment script so existing '
PRINT '**       records are updated to use the new lookup index value. '
:r .\lookupData\lookup.tblStandardFieldType.refdata.sql
:r .\lookupData\lookup.tblRight.refdata.sql
:r .\lookupData\lookup.tblApplicationSringType.refdata.sql
:r .\lookupData\lookup.tblTransactionStatus.refdata.sql
:r .\lookupData\lookup.tblAccessibilities.refdata.sql
:r .\lookupData\lookup.tblStationInterfaceType.refdata.sql
:r .\lookupData\lookup.tblEdgeMessage.refdata.sql
:r .\ervData\erv.tblEntitySegmentTemplate.refdata.sql
:r .\ervData\erv.tblEntityExternalAttribute.refdata.sql
:r .\ervData\erv.tblProcessSettings.refdata.sql

PRINT '***************************************************************************'
PRINT '** FINISHED INITIALIZE / UPDATE STATIC REFERENCE DATA (i.e. lookup data) **'
PRINT '***************************************************************************'
:r .\FMScripts\DataUpload\Script.FirstTimeDataUpload.sql
:r .\FMScripts\DataUpload\Script.IncrementalDataMaintenance.sql

:r .\FMScripts\DataMigrationScripts\Script.MeterEquipmentMappingMigration.sql
:r .\FMScripts\DataMigrationScripts\Script.MigrateStandardTankPointsToSupportCST.sql


-- This should be done after all other post deployment scripts have been processed.
UPDATE dbo.tblConfigurationSetting SET SettingValue = '1' WHERE SettingKey='AuditEnabled'

/*
************************************
	REESTABLISH SYSTEM ACCOUNTS POTENTIALLY DROPPED BY THE PROCCESS
************************************
*/
IF (SELECT SERVERPROPERTY('EngineEdition')) <> 5 -- not SQL Azure Database
BEGIN
    -- Fix up [NT AUTHORITY\NETWORK SERVICE] Login / Database Mapping
    -- Create a SQL Login for NT AUTHORITY\NETWORK SERVICE if one doesn't already exist.
	EXEC sp_executesql N'IF NOT EXISTS (SELECT 1 FROM master.sys.server_principals WHERE name = N''NT AUTHORITY\NETWORK SERVICE'')
    CREATE LOGIN [NT AUTHORITY\NETWORK SERVICE] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]'

    -- Map NT AUTHORITY\NETWORK SERVICE to the target database if it doesn't already exist.
	EXEC sp_executesql N'IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N''NT AUTHORITY\NETWORK SERVICE'')
    CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[dbo]'

    -- Add the db_owner Role to the NT AUTHORITY\NETWORK SERVICE user if it doesn't already exist.
	EXEC sp_executesql N'IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N''NT AUTHORITY\NETWORK SERVICE'')
    EXEC sys.sp_addrolemember @rolename = N''db_owner'', @membername = N''NT AUTHORITY\NETWORK SERVICE'''

	EXEC sp_executesql N'EXEC sys.sp_addsrvrolemember @loginame = N''NT AUTHORITY\NETWORK SERVICE'', @rolename = N''sysadmin'''
	
    -- Fix up [NT AUTHORITY\SYSTEM] Login / Database Mapping
    -- Create a SQL Login for NT AUTHORITY\SYSTEM if one doesn't already exist.
	EXEC sp_executesql N'IF NOT EXISTS (SELECT 1 FROM master.sys.server_principals WHERE name = N''NT AUTHORITY\SYSTEM'')
    CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]'

    -- Map NT AUTHORITY\SYSTEM to the target database if it doesn't already exist.
	EXEC sp_executesql N'IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N''NT AUTHORITY\SYSTEM'')
    CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM] WITH DEFAULT_SCHEMA=[dbo]'

    -- Add the db_owner Role to the NT AUTHORITY\SYSTEM user if it doesn't already exist.
	EXEC sp_executesql N'IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N''NT AUTHORITY\SYSTEM'')
    EXEC sys.sp_addrolemember @rolename = N''db_owner'', @membername = N''NT AUTHORITY\SYSTEM'''

	EXEC sp_executesql N'EXEC sys.sp_addsrvrolemember @loginame = N''NT AUTHORITY\SYSTEM'', @rolename = N''sysadmin'''
END

/*
IF '$(DeploymentType)' = 'Enterprise'
BEGIN
	:r .\FMSCripts\Core\DeployTypeEnterprise\<Script_Name_Here>.sql
END
*/

/*
IF '$(DeploymentType)' = 'Base'
BEGIN
	:r .\FMSCripts\Core\DeployTypeBase\<Script_Name_Here>.sql
END
*/

-----------------------------------------------------------------------------
-- If upgrading from FMV10 we need to convert the menu items and rights
-- Also we need to convert a table unique to that version 
-----------------------------------------------------------------------------
IF (( SELECT TOP 1 version 
	from tblVersion
	ORDER BY VersionIndex DESC ) = '10.0.0.0' )
BEGIN

	-- mapping conversion for rights
	CREATE TABLE #rightconversion ( beforeLookupRightIndex int, afterLookupRightIndex int )
	INSERT #rightconversion
	SELECT 180, 300
	UNION
	SELECT 181, 301
	UNION
	SELECT 182, 302
	UNION
	SELECT 183, 303
	UNION
	SELECT 184, 304
	UNION
	SELECT 188, 305
	UNION
	SELECT 189, 306
	UNION
	SELECT 190, 307
	UNION
	SELECT 191, 308
	UNION
	SELECT 192, 309
	UNION
	SELECT 193, 310
	UNION
	SELECT 194, 311
	UNION
	SELECT 195, 312
	UNION
	SELECT 196, 313
	UNION
	SELECT 197, 314
	UNION
	SELECT 198, 315
	UNION
	SELECT 199, 316
	UNION
	SELECT 200, 317
	UNION
	SELECT 201, 318
	UNION
	SELECT 202, 319
	UNION
	SELECT 203, 320
	UNION
	SELECT 204, 321
	UNION
	SELECT 205, 322
	UNION
	SELECT 206, 323
	UNION
	SELECT 207, 324
	UNION
	SELECT 208, 325
	UNION
	SELECT 209, 326
	UNION
	SELECT 210, 327
	UNION
	SELECT 211, 328
	UNION
	SELECT 212, 329
	UNION
	SELECT 213, 330
	UNION
	SELECT 214, 331
	UNION
	SELECT 215, 332
	UNION
	SELECT 217, 333
	UNION
	SELECT 218, 334
	UNION
	SELECT 219, 335
	UNION
	SELECT 220, 336
	UNION
	SELECT 221, 337
	UNION
	SELECT 222, 338
	UNION
	SELECT 223, 339
	UNION
	SELECT 224, 340
	UNION
	SELECT 225, 341
	UNION
	SELECT 44, 342

	
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid],[GroupGuid],[LookupRightIndex],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
	SELECT GroupToRightGuid,
	GroupGuid,
	ISNULL( rc.afterLookupRightIndex, LookupRightIndex),
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy
	FROM map.tblGroupToRight_upgrade gr
	LEFT JOIN #rightconversion rc
	ON gr.LookupRightIndex = rc.beforeLookupRightIndex
	WHERE NOT EXISTS ( SELECT 1 FROM map.tblGroupToRight gr2 WHERE gr2.GroupGuid = gr.GroupGuid AND gr2.LookupRightIndex = ISNULL( rc.afterLookupRightIndex, gr.LookupRightIndex))

	DROP TABLE  #rightconversion
	DROP TABLE map.tblGroupToRight_upgrade

	-- mapping conversion for menu items
	CREATE TABLE #menuitemconversion ( beforeMenuItemTypeCode int, afterbeforeMenuItemTypeCode int )
	INSERT #menuitemconversion
	SELECT 4057, 4069
	UNION
	SELECT 4058, 4070
	UNION
	SELECT 4059, 4071
	UNION
	SELECT 4061, 4073
	UNION
	SELECT 4062, 4073
	UNION
	SELECT 4063, 4074
	UNION
	SELECT 4064, 4075
	UNION
	SELECT 4065, 4076
	UNION
	SELECT 4066, 4077
	UNION
	SELECT 7035, 7039
	UNION
	SELECT 7036, 7040
	UNION
	SELECT 7037, 7041
	UNION
	SELECT 7038, 7042

	INSERT INTO [dbo].[tblMenuFavorites] ([MenuFavoriteGuid],[UserGuid],[IsQuickLink],[CustomName],[DisplayOrder],[MenuItemType],[DynamicMenuItemGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
	SELECT MenuFavoriteGuid,
	UserGuid,
	IsQuickLink,
	CustomName,
	DisplayOrder,
	ISNULL( mic.afterbeforeMenuItemTypeCode, mu.MenuItemType),
	DynamicMenuItemGuid,
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy
	FROM dbo.tblMenuFavorites_upgrade mu
	LEFT JOIN #menuitemconversion mic
	ON mu.MenuItemType = mic.beforeMenuItemTypeCode
	WHERE NOT EXISTS ( SELECT 1 FROM [dbo].[tblMenuFavorites] gr2 WHERE gr2.[MenuFavoriteGuid] = mu.[MenuFavoriteGuid] AND gr2.MenuItemType = ISNULL( mic.afterbeforeMenuItemTypeCode, mu.MenuItemType))

	DROP TABLE  #menuitemconversion
	DROP TABLE dbo.tblMenuFavorites_upgrade

	-- update the ownership of the modules
	DECLARE @siteguid UNIQUEIDENtIFIER

	SELECT top 1 @siteguid = siteguid
	FROM tblsites
	WHERE enabled = 1
	AND sitegroupflag = 0

	UPDATE [map].[tblEntityModuleToSite]
	SET SiteGuid = @siteguid, 
	UpdatedDate = SYSDATETIMEOFFSET()
	WHERE SiteGuid = '00000000-0000-0000-0000-000000000001'


	UPDATE tblModule
	SET SiteGuid = @siteguid, 
	UpdatedDate = SYSDATETIMEOFFSET()
	WHERE SiteGuid = '00000000-0000-0000-0000-000000000001'

	-- Convert trends
	IF OBJECT_ID('map.tblTrendPenToTrend_upgrade', 'U') IS NOT NULL 
	BEGIN 

		INSERT INTO [map].[tblTrendPenToDetailTrend]
			   ([TrendPenToDetailTrendGuid]
			   ,[PointTemplateTagGuid]
			   ,[TrendGuid]
			   ,[PenColor]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy])
		SELECT [TrendPenToTrendGuid]
			   ,[PointTemplateTagGuid]
			   ,[TrendGuid]
			   ,[PenColor]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
		FROM map.tblTrendPenToTrend_upgrade
		WHERE [PointTemplateTagGuid] IS NOT NULL


		INSERT INTO [map].[tblTrendPenToPointTrend]
			   ([TrendPenToPointTrendGuid]
			   ,[PointTagGuid]
			   ,[TrendGuid]
			   ,[PenColor]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy])
		SELECT [TrendPenToTrendGuid]
			   ,[PointTagGuid]
			   ,[TrendGuid]
			   ,[PenColor]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
		FROM map.tblTrendPenToTrend_upgrade
		WHERE [PointTagGuid] IS NOT NULL

		DROP TABLE map.tblTrendPenToTrend_upgrade
	END

END 



--UPDATE VERSION
-- From now on, only add the current version to the version table; there is no need to add the previous versions, and
-- only adding those that have been explicitly applied could help follow install history going forward.
IF NOT EXISTS(SELECT 1 FROM tblVersion WHERE [Version]='12.0.9.0')
BEGIN
	INSERT INTO tblVersion([Version],packageName,DateApplied,Comments,Check1,Check2,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
	VALUES ('12.0.9.0', 'StandardDatabase', SYSDATETIMEOFFSET(), 'FuelsManager 12.0.9.0', 0, 0, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

-- Update minor correction if going to new method
/*
if exists (select 1 from #UpgradeNotifications where UpgradeType = 'UpgradeMinorCorrection' and Upgrade = 1)
begin
	update [dbo].[tblProducts]
	set LookupMinorCorrectionMethodIndex = LookupMinorCorrectionMethodIndex + case LookupMajorCorrectionMethodIndex
																				when 0 then	0 -- none
																				when 1 then	0 -- none 1980
																				when 2 then	1 -- API C
																				when 3 then	1 -- API C 1980
																				when 4 then	12 -- API F
																				when 5 then	12 -- API F 1980
																				when 6 then	17 -- Polynomial F
																				when 7 then	17 -- Polynomial F 1980
																				when 8 then	18 -- LPG C
																				when 9 then	18 -- LPG C 1980
																				when 10 then	19 -- ASTM D1555 F
																				when 11 then	19 -- ASTM D1555 F 1980
																				when 12 then	19 -- ASTM D1555 C
																				when 13 then	19 -- ASTM D1555 C 1980
																				when 14 then	0 -- Japan none
																				when 15 then	1 -- Japan JIS 2249
																				when 16 then	30 -- Japan JIS 2250
																				when 17 then	19 -- Japan ASTM D1555
																				when 18 then	49 -- Japan ASTM D1250
																				when 19 then	35 -- Japan Chemical
																				when 20 then	37 -- Japan JIS 2249 Table
																				when 21 then	40 -- GBT
																				when 22 then	43 -- GOST
																				when 23 then	44 -- Asphalt
																				when 24 then	49 -- ASTM D1250 1952
																				when 25 then	50 -- ASTM Commodities 2004
																				when 26 then	19 -- ASTM D1555 F 2009
																				else	0
																				end
end
*/
