/*************************************************'
* Script.IncrementalDataMaintenance.sql file
* Use this file for include scripts for:
* 1. Insert data into a table that already has data (e.g. new entry into an already populated lookup table). It is required that the insert script verifies whether the inserting record does not exist).
* 2. Update the content of a record(s) present in a table
* 3. Delete records from a table 
**************************************************/
Print 'Add Export Result to transactionFieldType'
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM [lookup].[tblTransactionFieldType] WHERE TransactionFieldTypeCode = 'EXPORT_RESULT')
BEGIN
	BEGIN TRANSACTION
	INSERT INTO [lookup].[tblTransactionFieldType]
		(TransactionFieldTypeIndex, TransactionFieldTypeCode, TransactionFieldTypeName, TransactionFieldTypeGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		VALUES
		(7, 'TRANSACTION_FIELD_TYPE_MAX', 'TRANSACTION FIELD TYPE MAX', '958108EC-5579-4085-9C4C-9FA18597E903', N'01/01/2015 12:00:01 AM -04:00','Varec', N'01/01/2015 12:00:01 AM -04:00','Varec')	
	UPDATE [lookup].[tblTransactionFieldType] SET TransactionFieldTypeName = 'EXPORT RESULT' , TransactionFieldTypeCode = 'EXPORT_RESULT'
		WHERE TransactionFieldTypeIndex = 6 
	COMMIT TRANSACTION
END
GO

-- Remove duplicate entries in tblGroupToRight
Print 'Remove duplicate entries from tblGroupToRight'
GO
;WITH x(GroupToRightGuid, GroupGuid, LookupRightIndex, GroupId, CreatedDate) AS 
( 
	SELECT	[GroupToRightGuid]
			,a.[GroupGuid]
			,a.[LookupRightIndex], ag.groupid, a.createddate
		FROM [map].[tblGroupToRight] a 
		JOIN [dbo].[tblGroups] ag on a.groupguid=ag.groupguid
		WHERE EXISTS(
			SELECT *
				FROM [map].[tblGroupToRight] b 
				join [dbo].[tblGroups] bg on b.groupguid=bg.groupguid
			WHERE 
				ag.[Groupid] =bg.[Groupid]
				and a.[LookupRightIndex] = b.[LookupRightIndex]  
			HAVING COUNT(*) > 1)

)
DELETE FROM [map].[tblGroupToRight]
	WHERE createdDate > (SELECT MIN(CreatedDate) FROM x
		WHERE	[map].[tblGroupToRight].GroupGuid = GroupGuid 
			AND [map].[tblGroupToRight].[LookupRightIndex]=[LookupRightIndex])

GO
-- Remove duplicate entries in [tblGroupToTransactionAlias]
Print 'Remove duplicate entries from [tblGroupToTransactionAlias]'
GO
;WITH z AS (SELECT [GroupToTransactionAliasGuid]
	,ga.[GroupGuid], g.GroupId
	,ga.[TransactionAliasGuid], a.aliasname
	,ga.[LookupRightIndex]
	,ga.CreatedDate 
FROM [map].[tblGroupToTransactionAlias] ga
JOIN [dbo].[tblTransactionAliases] a ON ga.TransactionAliasGuid = a.TransactionAliasGuid
JOIN [dbo].[tblGroups] g ON g.GroupGuid = ga.GroupGuid
JOIN [lookup].[tblRight] r ON r.[RightIndex] = ga.[LookupRightIndex]
)
DELETE FROM [map].[tblGroupToTransactionAlias]
	WHERE createdDate > (SELECT MIN(CreatedDate) FROM z
		WHERE	[map].[tblGroupToTransactionAlias].GroupGuid = GroupGuid
			AND [map].[tblGroupToTransactionAlias].[TransactionAliasGuid] = [TransactionAliasGuid] 
			AND [map].[tblGroupToTransactionAlias].[LookupRightIndex]=[LookupRightIndex])
GO
-- Remove duplicate entries in [tblTransactionAliasToStatus]
Print 'Remove duplicate entries from [tblTransactionAliasToStatus]'
GO
;WITH z AS (SELECT [TransactionAliasToStatusGuid]
	  ,m.[TransactionAliasGuid] , a.AliasName
	  ,m.[LookupTransactionStatusIndex], s.TransactionStatusCode
	  ,m.CreatedDate
  FROM [map].[tblTransactionAliasToStatus] m
JOIN [dbo].[tblTransactionAliases] a ON m.TransactionAliasGuid = a.TransactionAliasGuid
JOIN [lookup].[tblTransactionStatus] s ON m.[LookupTransactionStatusIndex] = s.TransactionStatusIndex
)
DELETE FROM [map].[tblTransactionAliasToStatus]
	WHERE createdDate > (SELECT MIN(CreatedDate) FROM z
		WHERE	[map].[tblTransactionAliasToStatus].[TransactionAliasGuid] = [TransactionAliasGuid] 
			AND [map].[tblTransactionAliasToStatus].[LookupTransactionStatusIndex]=[LookupTransactionStatusIndex])

GO

-- Remove duplicate entries in [tblDataDictionaries]
Print 'Remove duplicate entries from [tblDataDictionaries]'
GO
;WITH z AS (SELECT d.[Key]
	  ,d.[Value]
	  ,d.[CreatedDate]
	  ,d.[DataDictionaryGuid]
	  ,d.[SiteGuid], s.ID as SIteID
  FROM [dbo].[tblDataDictionaries] d
  JOIN [dbo].[tblSites] s ON d.SiteGuid = s.SiteGuid
)
DELETE FROM [dbo].[tblDataDictionaries]
	WHERE createdDate > (SELECT MIN(CreatedDate) FROM z
		WHERE	[dbo].[tblDataDictionaries].SiteGuid = SiteGuid
			AND [dbo].[tblDataDictionaries].[Key] = [Key])
GO

-- Remove duplicate entries in [tblListViewFields]
Print 'Remove duplicate entries from [tblListViewFields]'
GO
BEGIN TRANSACTION
-- Remove ListView with ID = 1
DELETE FROM [dbo].[tblListViewFields] where ListViewGuid IN (select ListViewGuid FROM [dbo].[tblListViews] where id='1') 
DELETE FROM map.tblEntityListViewToSite where ListViewGuid IN (select ListViewGuid FROM [dbo].[tblListViews] where id='1') 
DELETE FROM [dbo].[tblListViews] where id='1'
GO

;WITH z AS (SELECT f.[ColumnOrder]
	  ,f.[CreatedDate]
	  ,f.[ListViewFieldGuid]
	  ,f.[LookupListViewFieldTypeIndex], ft.ListViewFieldTypeCode
	  ,f.[ListViewGuid], l.ID as ListViewID
	  ,c.[LedgerAggregateColumnGuid], c.ID as AgreggateColumnID
  FROM [dbo].[tblListViewFields] f
  JOIN [dbo].[tblListViews] l ON f.[ListViewGuid] = l.[ListViewGuid]
  JOIN [lookup].[tblListViewFieldType] ft ON f.[LookupListViewFieldTypeIndex]=ft.ListViewFieldTypeIndex
  JOIN [dbo].[tblLedgerAggregateColumns] c ON f.[LedgerAggregateColumnGuid]=c.[LedgerAggregateColumnGuid]
  WHERE LookupListViewFieldTypeIndex=6
)
DELETE FROM dbo.[tblListViewFields] WHERE CreatedDate > (SELECT MIN(CreatedDate) FROM z
		WHERE	[dbo].[tblListViewFields].[ListViewGuid] = [ListViewGuid]
			AND [dbo].[tblListViewFields].[LedgerAggregateColumnGuid] = [LedgerAggregateColumnGuid])
COMMIT TRANSACTION
GO

-- Remove duplicate entries in [tblTransactionAliasFields]
Print 'Remove duplicate entries from [tblTransactionAliasFields]'
GO
;WITH z AS (
SELECT f.[AliasID]
	  ,f.[DbName]
	  ,f.[DisplayOrder]
	  ,f.[DisplayName]
	  ,f.[CreatedDate]
	  ,f.[Required]
	  ,f.[Virtual]
	  ,f.[TransactionAliasFieldGuid]
	  ,f.[LookupTransactionFieldTypeIndex]
	  ,f.[TransactionAliasGuid], a.AliasName
	  ,f.[UserGroupGuid]
	  ,f.[DispatchField]
	  ,f.[ClearOnNew]
	  ,s.SIteGuid, s.ID as SiteID
  FROM [dbo].[tblTransactionAliasFields] f
  JOIN [dbo].[tblTransactionAliases] a ON f.[TransactionAliasGuid] = a.[TransactionAliasGuid]
  JOIN [dbo].[tblSites] s ON s.[SiteGuid] = a.[SiteGuid]
  )
DELETE FROM [dbo].[tblTransactionAliasFields]  FROM z  AS f JOIN [dbo].[tblTransactionAliasFields] t ON
t.[TransactionAliasFieldGuid] = f.[TransactionAliasFieldGuid] 
WHERE
f.CreatedDate > (SELECT MIN(CreatedDate) FROM z WHERE z.[DbName] = f.DBNAme AND z.AliasName = f.AliasName AND z.SiteID = f.SiteID) 
GO

-- Remove duplicate entries in [tblUserDataFieldTransactionAlias]
Print 'Remove duplicate entries from [tblUserDataFieldTransactionAlias]'
GO

BEGIN TRANSACTION
;WITH z AS (
SELECT u.[UserDataFieldTransactionAliasGuid]
	  ,u.[TransactionAliasGuid], a.AliasName
	  ,u.[SiteGuid], s.ID as SiteID
	  ,u.[Number]
	  ,u.[DisplayOrder]
	  ,u.[DisplayName]
	  ,u.[LookupUserDataTypeIndex]
	  ,u.[Required]
	  ,u.[UserGroupGuid]
	  ,u.[CreatedDate]
	  ,u.[DispatchField]
	  ,u.[ClearOnNew]
  FROM [dbo].[tblUserDataFieldTransactionAlias] u
  JOIN [dbo].[tblTransactionAliases] a ON u.[TransactionAliasGuid] = a.[TransactionAliasGuid]
  JOIN [dbo].[tblSites] s ON s.[SiteGuid] = a.[SiteGuid]
  )
DELETE FROM dbo.tblUserDataListValueTransactionAlias WHERE [UserDataFieldTransactionAliasGuid] IN (
SELECT u.[UserDataFieldTransactionAliasGuid]  FROM z  AS u JOIN [dbo].[tblUserDataFieldTransactionAlias] t ON
t.[UserDataFieldTransactionAliasGuid] = u.[UserDataFieldTransactionAliasGuid] 
WHERE
u.CreatedDate > (SELECT MIN(CreatedDate) FROM z WHERE z.[Number] = u.[Number] AND z.AliasName = u.AliasName AND z.SiteID = u.SiteID)) 


;WITH z AS (
SELECT u.[UserDataFieldTransactionAliasGuid]
	  ,u.[TransactionAliasGuid], a.AliasName
	  ,u.[SiteGuid], s.ID as SiteID
	  ,u.[Number]
	  ,u.[DisplayOrder]
	  ,u.[DisplayName]
	  ,u.[LookupUserDataTypeIndex]
	  ,u.[Required]
	  ,u.[UserGroupGuid]
	  ,u.[CreatedDate]
	  ,u.[DispatchField]
	  ,u.[ClearOnNew]
  FROM [dbo].[tblUserDataFieldTransactionAlias] u
  JOIN [dbo].[tblTransactionAliases] a ON u.[TransactionAliasGuid] = a.[TransactionAliasGuid]
  JOIN [dbo].[tblSites] s ON s.[SiteGuid] = a.[SiteGuid]
  )
DELETE FROM [dbo].[tblUserDataFieldTransactionAlias]  FROM z  AS u JOIN [dbo].[tblUserDataFieldTransactionAlias] t ON
t.[UserDataFieldTransactionAliasGuid] = u.[UserDataFieldTransactionAliasGuid] 
WHERE
u.CreatedDate > (SELECT MIN(CreatedDate) FROM z WHERE z.[Number] = u.[Number] AND z.AliasName = u.AliasName AND z.SiteID = u.SiteID) 
COMMIT TRANSACTION
GO


--
-- This will inform FMD to add Bsme specific menu items by calling BsmeWebApp.BSMEInterfaceTreeNav.GetMenuItems method.
--
Print 'Update IDiscoverAssemblies in tblConfigurationSetting'
GO
IF (NOT EXISTS(SELECT TOP 1 1 FROM [tblConfigurationSetting]
	WHERE SettingKey = 'IDiscoveryAssemblies' AND SettingValue LIKE '%BSMEWebApp%'))
	BEGIN
	IF (NOT EXISTS(SELECT TOP 1 1 FROM [tblConfigurationSetting]
		WHERE SettingKey = 'IDiscoveryAssemblies'))
		INSERT INTO [tblConfigurationSetting]
		([ConfigurationSettingGuid], [KeyType],[SettingKey],[SettingValue], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) 
		VALUES 
		('8C14D838-AF5B-4822-BAAF-A1461C5851B3', 'MULTI_SZ','IDiscoveryAssemblies','BSMEWEbApp', N'01/01/2015 12:00:01 AM -04:00', 'varec', N'01/01/2015 12:00:01 AM -04:00','varec')
	IF (NOT EXISTS(SELECT TOP 1 1 FROM [tblConfigurationSetting]
			WHERE SettingKey = 'IDiscoveryAssemblies' AND SettingValue LIKE '%BSMEWebApp%'))
		UPDATE [tblConfigurationSetting] SET SettingValue += ';BSMEWebApp'
			WHERE SettingKey = 'IDiscoveryAssemblies'
	END

UPDATE [dbo].[tblConfigurationSetting] SET SettingValue = 'BsmeBusinessObjects'
		WHERE SettingKey = 'AccountingEnterpriseInterface' AND ISNULL(SettingValue,'') = ''
UPDATE [dbo].[tblConfigurationSetting] SET SettingValue += ';BsmeBusinessObjects'
		WHERE SettingKey = 'AccountingEnterpriseInterface' AND SettingValue NOT LIKE '%BsmeBusinessObjects%'
UPDATE [dbo].[tblConfigurationSetting] SET SettingValue += ';BsmeBusinessServices'
		WHERE SettingKey = 'AccountingEnterpriseInterface' AND SettingValue NOT LIKE '%BsmeBusinessServices%'

GO

--
-- Add the BsmeBusinessObjects.dll which contains the custom BSME security rights.
--
IF ((SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ISecurityAssemblies' AND SettingValue LIKE ('%BsmeBusinessObjects%')) = 0)
BEGIN
	DECLARE @SecurityAssemblyStr NVARCHAR (1000)
	SELECT @SecurityAssemblyStr = SettingValue FROM tblConfigurationSetting WHERE SettingKey = 'ISecurityAssemblies'

	IF (LEN(@SecurityAssemblyStr) = 0 OR @SecurityAssemblyStr IS NULL)
	BEGIN
		SET @SecurityAssemblyStr = '';
	END
	ELSE
	BEGIN
		SET @SecurityAssemblyStr = @SecurityAssemblyStr + ';'
	END 

	SET @SecurityAssemblyStr = @SecurityAssemblyStr + 'BsmeBusinessObjects'
	UPDATE tblConfigurationSetting SET SettingValue = @SecurityAssemblyStr WHERE SettingKey = 'ISecurityAssemblies'
END
GO

--
-- Add the BsmeBusinessObjects.dll which contains the custom BSME security rights.
--
IF ((SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'TestSetResultFormURL' ) > 0)
BEGIN
	UPDATE tblConfigurationSetting SET SettingValue = 'BSMEWebApp/TestSetResultForm.aspx' WHERE SettingKey = 'TestSetResultFormURL'
END
ELSE
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('35DCB263-D2DB-4CEF-8DBC-967EA3338911', 'SZ', 'TestSetResultFormURL', 'BSMEWebApp/TestSetResultForm.aspx', N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
END
GO


--DECLARE @AdministratorGroupID nvarchar(64)
--DECLARE @AdministratorGroupGuid uniqueidentifier
--DECLARE @CreatedBy nvarchar(32)
--DECLARE @CreatedDate DateTimeOffset(7)
--SET @CreatedBy = N'Varec'
--SET @CreatedDate = GetUTCDate()
--SET @AdministratorGroupGuid =  CONVERT(uniqueidentifier,  N'FE4F6B8F-9474-47BB-B728-7AEB2C6B1FDA')
--SET @AdministratorGroupID = N'DLA Administrator';
--print 'IncrementaldataMaintenance: insert group Administrator'
----
---- This adds DLA Administrator Group.
---- 
--IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblGroups] WHERE GroupID=@AdministratorGroupID AND SiteGuid = CONVERT(uniqueidentifier, N'00000000-0000-0000-0000-000000000001')))
--	INSERT INTO [dbo].[tblGroups] ([GroupID], [GroupDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [GroupGuid], [SiteGuid]) 
--	VALUES 
--	(@AdministratorGroupID, @AdministratorGroupID, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy,@AdministratorGroupGuid, CONVERT(uniqueidentifier, N'00000000-0000-0000-0000-000000000001'))

--SELECT @AdministratorGroupGuid = [GroupGuid] FROM [dbo].[tblGroups] WHERE GroupID=@AdministratorGroupID AND SiteGuid = CONVERT(uniqueidentifier, N'00000000-0000-0000-0000-000000000001')

--IF (@AdministratorGroupGuid IS NOT NULL)
--BEGIN
--	IF (NOT EXISTS(SELECT TOP 1 1 FROM [map].[tblEntityUserGroupToSite] WHERE GroupGuID=@AdministratorGroupGuid AND SiteGuid = CONVERT(uniqueidentifier, N'00000000-0000-0000-0000-000000000001') AND [AssignedFromSiteGuid] = CONVERT(uniqueidentifier, N'00000000-0000-0000-0000-000000000001')))
--		INSERT INTO [map].[tblEntityUserGroupToSite] ([UserGroupToSiteGuid], [GroupGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) 
--		VALUES 
--		(CONVERT(uniqueidentifier, N'A112DAB8-54BA-4B55-984C-B9CEB022D2D5'), @AdministratorGroupGuid, CONVERT(uniqueidentifier, N'00000000-0000-0000-0000-000000000001'), @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy, CONVERT(uniqueidentifier, N'00000000-0000-0000-0000-000000000001'))
--	IF (NOT EXISTS(SELECT TOP 1 1 FROM [map].[tblGroupToRight] WHERE [LookupRightIndex] = 184 AND [GroupGuid] = @AdministratorGroupGuid))
--		INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
--		VALUES 
--		(CONVERT(uniqueidentifier, N'183941B7-5FA5-4A96-B4D8-31572E872158'), @AdministratorGroupGuid, 184, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy)
--	IF (NOT EXISTS(SELECT TOP 1 1 FROM [map].[tblGroupToRight] WHERE [LookupRightIndex] = 185 AND [GroupGuid] = @AdministratorGroupGuid))
--		INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
--		VALUES 
--		(CONVERT(uniqueidentifier, N'90341F08-EDF2-4273-9369-BB2D28125D14'), @AdministratorGroupGuid, 185, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy)
--END
--GO

-- Due to the main branch already having 181 and 182 used, we need to shift any BSM-E deployment that started BSM-E rights at 181, down 2 positions.

BEGIN
	DECLARE @newSecurityRights bit
	SET @newSecurityRights = 0

	print 'IncrementaldataMaintenance: insert right CONFIGURE_DLA_TEST'
	IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex=187)
	BEGIN
		INSERT INTO lookup.tblRight (RightGuid, RightIndex, RightCode, RightName, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		VALUES ('6AC5514A-718F-4A62-8BAE-ACA7082708D5', 187, 'CONFIGURE_DLA_TEST', 'CONFIGURE_DLA_TEST', N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')

		SET @newSecurityRights = 1
	END

	print 'IncrementaldataMaintenance: insert right CONFIGURE_WEB_LINKS'
	IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex=186)
	BEGIN
		INSERT INTO lookup.tblRight (RightGuid, RightIndex, RightCode, RightName, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		VALUES ('7CF2C317-035C-4954-9FCE-4E816F80FB5E', 186, 'CONFIGURE_WEB_LINKS', 'CONFIGURE_WEB_LINKS', N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')

	END

	print 'IncrementaldataMaintenance: insert right VIEW_MOVEMENT'
	IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightCode = 'VIEW_MOVEMENT')
	BEGIN
		INSERT INTO lookup.tblRight (RightGuid, RightIndex, RightCode, RightName, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		VALUES ('1B0C5638-3354-4CFB-9A25-608C4307B97B', 185, 'VIEW_MOVEMENT', 'VIEW_MOVEMENT', N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
	END

	print 'IncrementaldataMaintenance: insert right CONFIGURE_LOCATIONS'
	IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightCode = 'CONFIGURE_LOCATIONS')
	BEGIN
		INSERT INTO lookup.tblRight (RightGuid, RightIndex, RightCode, RightName, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		VALUES ('2C2AD419-3FFF-4483-8100-D1B5C289831A', 184, 'CONFIGURE_LOCATIONS', 'CONFIGURE_LOCATIONS', N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
	END

	-- Modifications on enum RIGHT may require new INSERT/UPDATE on table lookup.tblRights
	-- For example, if you add a new right it must be added to lookup.tblRights
	print 'IncrementaldataMaintenance: insert right MODIFY_UNOBTAINABLE'
	IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightCode = 'MODIFY_UNOBTAINABLE')
	BEGIN
		INSERT INTO lookup.tblRight (RightGuid, RightIndex, RightCode, RightName, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		VALUES ('5429AE4E-044C-45CA-8E07-994E036818C9', 183, 'MODIFY_UNOBTAINABLE', 'MODIFY_UNOBTAINABLE', N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
	END



	IF @newSecurityRights = 1
	BEGIN
		UPDATE [map].tblGroupToRight SET LookupRightIndex = 187, UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' WHERE LookupRightIndex = 185
		UPDATE [map].tblGroupToRight SET LookupRightIndex = 186, UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' WHERE LookupRightIndex = 184
		UPDATE [map].tblGroupToRight SET LookupRightIndex = 185, UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' WHERE LookupRightIndex = 183
		UPDATE [map].tblGroupToRight SET LookupRightIndex = 184, UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' WHERE LookupRightIndex = 182
		UPDATE [map].tblGroupToRight SET LookupRightIndex = 183, UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' WHERE LookupRightIndex = 181

		-- put these back to what the core is expecting
		UPDATE lookup.tblRight SET RightCode = 'VIEW_EXTERNAL_STATION', rightname = 'VIEW_EXTERNAL_STATION', rightguid = '93E2CBFD-320B-466D-AD76-FBEB6B73FBDC', UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' where RightIndex = 181
		UPDATE lookup.tblRight SET RightCode = 'MODIFY_EXTERNAL_STATION', rightname = 'MODIFY_EXTERNAL_STATION', rightguid = 'BA9D30CA-9642-4407-BCB6-0B65F4C31752', UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' where RightIndex = 182

		--update these to be the new right value
		UPDATE lookup.tblRight SET RightCode = 'MODIFY_UNOBTAINABLE', RightName = 'MODIFY_UNOBTAINABLE', rightguid = '5429AE4E-044C-45CA-8E07-994E036818C9', UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' WHERE RightIndex = 183
		UPDATE lookup.tblRight SET RightCode = 'CONFIGURE_LOCATIONS', RightName = 'CONFIGURE_LOCATIONS', rightguid = '2C2AD419-3FFF-4483-8100-D1B5C289831A', UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' WHERE RightIndex = 184
		UPDATE lookup.tblRight SET RightCode = 'VIEW_MOVEMENT', RightName = 'VIEW_MOVEMENT', rightguid = '1B0C5638-3354-4CFB-9A25-608C4307B97B', UpdatedDate = N'11/01/2015 12:00:01 AM -04:00' WHERE RightIndex = 185

	END

END
GO





print 'IncrementaldataMaintenance: configure shutdown settings'
IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ShutdownIfMaximumErrorCountExceededForLogs') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(CONVERT(uniqueidentifier, '09D6F03A-1205-4C74-A356-BDB2BDD805D9'), 'DWORD', 'ShutdownIfMaximumErrorCountExceededForLogs', 0, N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
END
ELSE IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ShutdownIfMaximumErrorCountExceededForLogs' AND SettingValue = 0) = 0
BEGIN
	UPDATE tblConfigurationSetting SET ConfigurationSettingGuid = CONVERT(uniqueidentifier, '09D6F03A-1205-4C74-A356-BDB2BDD805D9'), SettingValue = 0, UpdatedDate = N'01/01/2015 12:00:01 AM -04:00', UpdatedBy = 'Administrator'
	WHERE SettingKey = 'ShutdownIfMaximumErrorCountExceededForLogs'
END
GO

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'MaximumConsecutiveErrorCountForLogs') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(CONVERT(uniqueidentifier, 'F6CA5D2E-C3C5-456E-8BEA-381432E03799'), 'DWORD', 'MaximumConsecutiveErrorCountForLogs', 3, N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
END
ELSE IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'MaximumConsecutiveErrorCountForLogs' AND SettingValue = 3) = 0
BEGIN
	UPDATE tblConfigurationSetting SET ConfigurationSettingGuid = CONVERT(uniqueidentifier, 'F6CA5D2E-C3C5-456E-8BEA-381432E03799'), SettingValue = 3, UpdatedDate = N'01/01/2015 12:00:01 AM -04:00', UpdatedBy = 'Administrator'
	WHERE SettingKey = 'MaximumConsecutiveErrorCountForLogs'
END
GO

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ShutdownIfThresholdExceededForLogs') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(CONVERT(uniqueidentifier, 'A4C96C9B-B13E-4FFB-8DAA-6CD76A9DB13A'), 'DWORD', 'ShutdownIfThresholdExceededForLogs', 0, N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
END
ELSE IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ShutdownIfThresholdExceededForLogs' AND SettingValue = 0) = 0
BEGIN
	UPDATE tblConfigurationSetting SET ConfigurationSettingGuid = CONVERT(uniqueidentifier, 'A4C96C9B-B13E-4FFB-8DAA-6CD76A9DB13A'), SettingValue = 0, UpdatedDate = N'01/01/2015 12:00:01 AM -04:00', UpdatedBy = 'Administrator'
	WHERE SettingKey = 'ShutdownIfThresholdExceededForLogs'
END
GO

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ThreshholdPercentageForLogs') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(CONVERT(uniqueidentifier, 'E46906C4-1533-4D8D-8477-F43BF7B76968'), 'DWORD', 'ThreshholdPercentageForLogs', 1, N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
END
ELSE IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ThreshholdPercentageForLogs' AND SettingValue = 1) = 0
BEGIN
	UPDATE tblConfigurationSetting SET ConfigurationSettingGuid = CONVERT(uniqueidentifier, 'E46906C4-1533-4D8D-8477-F43BF7B76968'), SettingValue = 1, UpdatedDate = N'01/01/2015 12:00:01 AM -04:00', UpdatedBy = 'Administrator'
	WHERE SettingKey = 'ThreshholdPercentageForLogs'
END
GO

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'MaximumNumberOfRowsForLogs') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(CONVERT(uniqueidentifier, '18C109C6-0563-4565-A726-DD93674522D2'), 'DWORD', 'MaximumNumberOfRowsForLogs', 1, N'01/01/2015 12:00:01 AM -04:00', 'Administrator', N'01/01/2015 12:00:01 AM -04:00', 'Administrator')
END
ELSE IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'MaximumNumberOfRowsForLogs' AND SettingValue = 1) = 0
BEGIN
	UPDATE tblConfigurationSetting SET ConfigurationSettingGuid = CONVERT(uniqueidentifier, '18C109C6-0563-4565-A726-DD93674522D2'), SettingValue = 1, UpdatedDate = N'01/01/2015 12:00:01 AM -04:00', UpdatedBy = 'Administrator'
	WHERE SettingKey = 'MaximumNumberOfRowsForLogs'
END
GO


------------------------------------------------------------------------------------------------------
--	Bsme DB Changes from FMD 8 SP4
------------------------------------------------------------------------------------------------------






print 'IncrementaldataMaintenance: 8.0.4.20-039 WI-48150'
-- keeping for now
--	8.0.4.20-039 WI-48150 Remove Transaction Status from Sale and Defuel Transaction Aliases.sql
--
--Delete From [dbo].[tblTransactionAliasFields] where DisplayName='status' and TransactionAliasGuid in 
--(Select TransactionAliasGuid FROM dbo.tblTransactionAliases WHERE Aliasname in ('Sale','defuel')) -- Sale and Defuel

--go





print 'IncrementaldataMaintenance: 8.0.4.17210-009 WI-43648'
--GO
----
----	8.0.4.17210-009 WI-43648 Add Unobtainable right to Accounting group.sql
----	8.0.4.17210-011 WI-43717 Add Unobtainable right to Administrator group.sql
----	8.0.4.20-034 WI-46911 Assign View Movement Calendar permission to user groups.sql
----
--INSERT INTO [Map].[tblGroupToRight]
--([GroupToRightGuid], GroupGuid, LookupRightIndex, CreatedDate, CreatedBy)
--SELECT convert(uniqueidentifier, 
--				hashbytes('md5',	--ok
--						(
--						convert(varchar(36), 183)+
--						convert(varchar(36), [GroupGuid])))) as GroupToRightGuid,
--	groupGuid, 183 /*View Movement Calendar*/, GETUTCDATE(), 'Varec' FROM 
--[dbo].[tblGroups] g where GroupID in ('Administrator', 'Accounting', 'Cor') 
--AND NOT EXISTS(SELECT 1 FROM [Map].[tblGroupToRight]
--WHERE g.GroupGuid=GroupGuid AND 183=LookupRightIndex)
--GO





print 'Add Modify Unobtainable and Acces MFCS rights to Accounting'
GO
INSERT INTO  [map].[tblGroupToRight] 
([GroupToRightGuid], GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate,UpdatedBy)
	SELECT convert(uniqueidentifier, --ok
				hashbytes('md5',	
						(
						convert(varchar(36), RightIndex)+
						convert(varchar(36), [GroupGuid])))) as [GroupToRightGuid],
	GroupGuid, RightIndex, N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec' FROM  
 [lookup].[tblRight] r , [dbo].tblgroups g  
where groupid='accounting' AND RightCode IN ('MODIFY_UNOBTAINABLE','ACCESS_MFCS')
AND NOT EXISTS(SELECT 1 FROM [Map].[tblGroupToRight]
WHERE g.GroupGuid=GroupGuid AND rightIndex=LookupRightIndex)
GO

print 'Add Base export manual and send to ebs  rights to Accounting in Enterprise'
GO
INSERT INTO  [map].[tblGroupToRight] 
([GroupToRightGuid], GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate,UpdatedBy)
 SELECT convert(uniqueidentifier, 
				hashbytes('md5',	--ok
						(
						convert(varchar(36), RightIndex)+
						convert(varchar(36), [GroupGuid])))) as [GroupToRightGuid],
		GroupGuid, RightIndex, N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec' FROM  
 [lookup].[tblRight] r , [dbo].tblgroups g  
where groupid='accounting' AND RightCode IN ('BASE_EXPORT_MANUAL','SEND_TO_EBS')
AND NOT EXISTS(SELECT 1 FROM [Map].[tblGroupToRight]
WHERE g.GroupGuid=GroupGuid AND rightIndex=LookupRightIndex) AND @@Version LIKE '%Enterprise%'
GO

print 'Enable manual send enterprise transactions in Enterprise'
GO
IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblConfigurationSetting]
	WHERE SettingKey = 'ManuallySendEnterpriseTransactions') AND @@Version LIKE '%Enterprise%')
	INSERT INTO [dbo].[tblConfigurationSetting]
	([ConfigurationSettingGuid], [KeyType],[SettingKey],[SettingValue], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) 
	VALUES 
	('B23D4584-E446-4EA5-8B30-A95026D44DAD', 'DWORD','ManuallySendEnterpriseTransactions','1', N'01/01/2015 12:00:01 AM -04:00', 'varec', N'01/01/2015 12:00:01 AM -04:00','varec')

GO

print 'Switch to using transaction IssuePoint and IssuePointNumber fields'
GO
DECLARE @Now DateTime = GetUTCDate()
DECLARE @user NVARCHAR(100)='administrator'
DECLARE @SiteAdminSiteGuid			uniqueidentifier = (SELECT SiteGuid FROM dbo.tblSites WITH(NOLOCK) WHERE ID='SiteAdmin')
DECLARE @AliasName					nvarchar(32) = 'Sale'
DECLARE @TransactionAliasGuid		UniqueIdentifier = (SELECT TransactionAliasGuid FROM [dbo].[tblTransactionAliases] WITH(NOLOCK) 
	WHERE aliasName=@AliasName AND SiteGuid=@SiteAdminSiteGuid)
DECLARE @DisplayOrder				int = (SELECT DisplayOrder FROM [dbo].[tblTransactionAliasFields]  WITH(NOLOCK) WHERE TransactionALiasGuid=@TransactionAliasGuid AND DBName='InterfaceData01')
DECLARE @UserDataFieldTransactionAliasGuid	UniqueIdentifier = (SELECT UserDataFieldTransactionAliasGuid FROM [dbo].[tblUserDataFieldTransactionAlias] 
	WHERE SiteGuid=@SiteAdminSiteGuid AND TransactionALiasGuid=@TransactionAliasGuid AND DisplayName LIKE 'Issue P%' AND Number=12)
DECLARE @ListViewFieldGuid UniqueIdentifier =(SELECT ListViewFieldGuid FROM dbo.tblListViewFields WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid)


IF ( @TransactionAliasGuid IS NOT NULL )
BEGIN
	IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePoint'))
		INSERT INTO [dbo].[tblTransactionAliasFields]
		([TransactionAliasFieldGuid],[TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[UserGroupGuid])
		  VALUES
		('C69BFA0A-5D2C-4CCF-A46F-1BCFA083A371', @TransactionAliasGuid,	1,	'IssuePoint',		@DisplayOrder+1,	'Issue Pt',	@now,	@user,	@now,	@user,	0,	0,	NULL)
	IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber'))
		INSERT INTO [dbo].[tblTransactionAliasFields]
		([TransactionAliasFieldGuid],[TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[UserGroupGuid])
		  VALUES
		('33B58063-AEBB-46E7-A072-F4E63FFEBF99', @TransactionAliasGuid,	1,	'IssuePointNumber',		@DisplayOrder+3,	'Issue Pt Num',	@now,	@user,	@now,	@user,	0,	0,	NULL)
	UPDATE dbo.tblListViewFields SET updateddate=@now,UserDataFieldTransactionAliasGuid=null,LookupListViewFieldTypeIndex=2,
	TransactionAliasFieldGuid=(SELECT TransactionAliasFieldGuid  FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePoint') WHERE ListViewFieldGuid=@ListViewFieldGuid
	IF (@UserDataFieldTransactionAliasGuid IS NOT NULL)
			DELETE FROM [dbo].[tblUserDataFieldTransactionAlias] WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid

	SET @UserDataFieldTransactionAliasGuid	= (SELECT UserDataFieldTransactionAliasGuid FROM [dbo].[tblUserDataFieldTransactionAlias] 
		WHERE SiteGuid=@SiteAdminSiteGuid AND TransactionALiasGuid=@TransactionAliasGuid AND DisplayName Like 'Issue P% Num%' AND Number=13)
	SET @ListViewFieldGuid =(SELECT ListViewFieldGuid FROM dbo.tblListViewFields WHERE 	@UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid)
	UPDATE dbo.tblListViewFields SET  updateddate=@now,UserDataFieldTransactionAliasGuid=null,LookupListViewFieldTypeIndex=2,
	TransactionAliasFieldGuid=(SELECT TransactionAliasFieldGuid  FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber') WHERE ListViewFieldGuid=@ListViewFieldGuid
	IF (@UserDataFieldTransactionAliasGuid IS NOT NULL)
			DELETE FROM [dbo].[tblUserDataFieldTransactionAlias] WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid
END

SET @AliasName					 = 'Defuel'
SET @TransactionAliasGuid		 = (SELECT TransactionAliasGuid FROM [dbo].[tblTransactionAliases] WITH(NOLOCK) 
	WHERE aliasName=@AliasName AND SiteGuid=@SiteAdminSiteGuid)
SET @DisplayOrder  = (SELECT DisplayOrder FROM [dbo].[tblTransactionAliasFields]  WITH(NOLOCK) WHERE TransactionALiasGuid=@TransactionAliasGuid AND DBName='InterfaceData01')

IF (@TransactionAliasGuid IS NOT NULL )
BEGIN

	IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePoint'))
		INSERT INTO [dbo].[tblTransactionAliasFields]
		([TransactionAliasFieldGuid], [TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[UserGroupGuid])
		  VALUES
		('3A2F36E4-71AC-4527-A4B3-63ED647C2327', @TransactionAliasGuid,	1,	'IssuePoint',		@DisplayOrder+1,	'Issue Pt',	@now,	@user,	@now,	@user,	0,	0,	NULL)
	IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber'))
		INSERT INTO [dbo].[tblTransactionAliasFields]
		([TransactionAliasFieldGuid], [TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[UserGroupGuid])
		  VALUES
		('78A91E0E-4606-4A2C-A5D2-69B6C784AD44', @TransactionAliasGuid,	1,	'IssuePointNumber',		@DisplayOrder+3,	'Issue Pt Num',	@now,	@user,	@now,	@user,	0,	0,	NULL)

	SET @UserDataFieldTransactionAliasGuid	 = (SELECT UserDataFieldTransactionAliasGuid FROM [dbo].[tblUserDataFieldTransactionAlias] 
		WHERE SiteGuid=@SiteAdminSiteGuid AND TransactionALiasGuid=@TransactionAliasGuid AND DisplayName  LIKE 'Issue P%' AND Number=12)
	SET @ListViewFieldGuid =(SELECT ListViewFieldGuid FROM dbo.tblListViewFields WHERE 	@UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid)
	UPDATE dbo.tblListViewFields SET UserDataFieldTransactionAliasGuid=null,LookupListViewFieldTypeIndex=2,
	TransactionAliasFieldGuid=(SELECT TransactionAliasFieldGuid  FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePoint') WHERE ListViewFieldGuid=@ListViewFieldGuid
	IF (@UserDataFieldTransactionAliasGuid IS NOT NULL)
			DELETE FROM [dbo].[tblUserDataFieldTransactionAlias] WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid
	SET @UserDataFieldTransactionAliasGuid	= (SELECT UserDataFieldTransactionAliasGuid FROM [dbo].[tblUserDataFieldTransactionAlias] 
		WHERE SiteGuid=@SiteAdminSiteGuid AND TransactionALiasGuid=@TransactionAliasGuid AND DisplayName Like 'Issue P% Num%' AND Number=13)
	SET @ListViewFieldGuid =(SELECT ListViewFieldGuid FROM dbo.tblListViewFields WHERE 	@UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid)
	UPDATE dbo.tblListViewFields SET UserDataFieldTransactionAliasGuid=null,LookupListViewFieldTypeIndex=2,
	TransactionAliasFieldGuid=(SELECT TransactionAliasFieldGuid  FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber') WHERE ListViewFieldGuid=@ListViewFieldGuid
	IF (@UserDataFieldTransactionAliasGuid IS NOT NULL)
			DELETE FROM [dbo].[tblUserDataFieldTransactionAlias] WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid
END


SET @AliasName					 = 'Reissue'
SET @TransactionAliasGuid		 = (SELECT TransactionAliasGuid FROM [dbo].[tblTransactionAliases] WITH(NOLOCK) 
	WHERE aliasName=@AliasName AND SiteGuid=@SiteAdminSiteGuid)
SET @DisplayOrder  = (SELECT max(DisplayOrder) FROM [dbo].[tblTransactionAliasFields]  WITH(NOLOCK) WHERE TransactionALiasGuid=@TransactionAliasGuid AND DBName='InterfaceData01')

IF (@TransactionAliasGuid IS NOT NULL )
BEGIN

	IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber'))
		INSERT INTO [dbo].[tblTransactionAliasFields]
		([TransactionAliasFieldGuid], [TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[UserGroupGuid])
		  VALUES
		('44505B7D-9A9D-445D-91B3-44A07F14373A', @TransactionAliasGuid,	1,	'IssuePointNumber',		@DisplayOrder+3,	'Issue Pt Num',	@now,	@user,	@now,	@user,	0,	0,	NULL)


	SET @UserDataFieldTransactionAliasGuid	= (SELECT UserDataFieldTransactionAliasGuid FROM [dbo].[tblUserDataFieldTransactionAlias] 
		WHERE SiteGuid=@SiteAdminSiteGuid AND TransactionALiasGuid=@TransactionAliasGuid AND DisplayName Like 'Issue P% Num%' AND Number=13)
	SET @ListViewFieldGuid =(SELECT ListViewFieldGuid FROM dbo.tblListViewFields WHERE 	@UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid)
	UPDATE dbo.tblListViewFields SET UserDataFieldTransactionAliasGuid=null,LookupListViewFieldTypeIndex=2,
	TransactionAliasFieldGuid=(SELECT TransactionAliasFieldGuid  FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber') WHERE ListViewFieldGuid=@ListViewFieldGuid
	IF (@UserDataFieldTransactionAliasGuid IS NOT NULL)
			DELETE FROM [dbo].[tblUserDataFieldTransactionAlias] WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid
END

SET @AliasName					 = 'Commercial'
SET @TransactionAliasGuid		 = (SELECT TransactionAliasGuid FROM [dbo].[tblTransactionAliases] WITH(NOLOCK) 
	WHERE aliasName=@AliasName AND SiteGuid=@SiteAdminSiteGuid)
SET @DisplayOrder  = (SELECT max(DisplayOrder) FROM [dbo].[tblTransactionAliasFields]  WITH(NOLOCK) WHERE TransactionALiasGuid=@TransactionAliasGuid AND DBName='InterfaceData01')


IF (@TransactionAliasGuid IS NOT NULL )
BEGIN

	IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber'))
		INSERT INTO [dbo].[tblTransactionAliasFields]
		([TransactionAliasFieldGuid],[TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[UserGroupGuid])
		  VALUES
		('74218907-0435-4DEE-86E2-644BBF9CC210', @TransactionAliasGuid,	1,	'IssuePointNumber',		@DisplayOrder+3,	'Issue Pt Num',	@now,	@user,	@now,	@user,	0,	0,	NULL)


	SET @UserDataFieldTransactionAliasGuid	= (SELECT UserDataFieldTransactionAliasGuid FROM [dbo].[tblUserDataFieldTransactionAlias] 
		WHERE SiteGuid=@SiteAdminSiteGuid AND TransactionALiasGuid=@TransactionAliasGuid AND DisplayName Like 'Issue P% Num%' AND Number=13)
	SET @ListViewFieldGuid =(SELECT ListViewFieldGuid FROM dbo.tblListViewFields WHERE 	@UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid)
	UPDATE dbo.tblListViewFields SET UserDataFieldTransactionAliasGuid=null,LookupListViewFieldTypeIndex=2,
	TransactionAliasFieldGuid=(SELECT TransactionAliasFieldGuid  FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber') WHERE ListViewFieldGuid=@ListViewFieldGuid
	IF (@UserDataFieldTransactionAliasGuid IS NOT NULL)
			DELETE FROM [dbo].[tblUserDataFieldTransactionAlias] WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid
END

SET @AliasName					 = 'Recirculation'
SET @TransactionAliasGuid		 = (SELECT TransactionAliasGuid FROM [dbo].[tblTransactionAliases] WITH(NOLOCK) 
	WHERE aliasName=@AliasName AND SiteGuid=@SiteAdminSiteGuid)
SET @DisplayOrder  = (SELECT max(DisplayOrder) FROM [dbo].[tblTransactionAliasFields]  WITH(NOLOCK) WHERE TransactionALiasGuid=@TransactionAliasGuid)

IF (@TransactionAliasGuid IS NOT NULL )
BEGIN
	IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePoint'))
		INSERT INTO [dbo].[tblTransactionAliasFields]
		([TransactionAliasFieldGuid], [TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[UserGroupGuid])
		  VALUES
		('26674213-45E0-4C27-9268-C7FD97682611', @TransactionAliasGuid,	1,	'IssuePoint',		@DisplayOrder+1,	'Issue Pt',	@now,	@user,	@now,	@user,	0,	0,	NULL)
	IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber'))
		INSERT INTO [dbo].[tblTransactionAliasFields]
		([TransactionAliasFieldGuid], [TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[UserGroupGuid])
		  VALUES
		('342A191D-8DE2-4609-9158-E789D6EC6783', @TransactionAliasGuid,	1,	'IssuePointNumber',		@DisplayOrder+3,	'Issue Pt Num',	@now,	@user,	@now,	@user,	0,	0,	NULL)

	SET @UserDataFieldTransactionAliasGuid	 = (SELECT UserDataFieldTransactionAliasGuid FROM [dbo].[tblUserDataFieldTransactionAlias] 
		WHERE SiteGuid=@SiteAdminSiteGuid AND TransactionALiasGuid=@TransactionAliasGuid AND DisplayName LIKE 'Issue P%' AND Number=12)
	SET @ListViewFieldGuid =(SELECT ListViewFieldGuid FROM dbo.tblListViewFields WHERE 	@UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid)
	UPDATE dbo.tblListViewFields SET UserDataFieldTransactionAliasGuid=null,LookupListViewFieldTypeIndex=2,
	TransactionAliasFieldGuid=(SELECT TransactionAliasFieldGuid  FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePoint') WHERE ListViewFieldGuid=@ListViewFieldGuid
	IF (@UserDataFieldTransactionAliasGuid IS NOT NULL)
			DELETE FROM [dbo].[tblUserDataFieldTransactionAlias] WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid
	SET @UserDataFieldTransactionAliasGuid	= (SELECT UserDataFieldTransactionAliasGuid FROM [dbo].[tblUserDataFieldTransactionAlias] 
		WHERE SiteGuid=@SiteAdminSiteGuid AND TransactionALiasGuid=@TransactionAliasGuid AND DisplayName Like 'Issue P% Num%' AND Number=13)
	SET @ListViewFieldGuid =(SELECT ListViewFieldGuid FROM dbo.tblListViewFields WHERE 	@UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid)
	UPDATE dbo.tblListViewFields SET UserDataFieldTransactionAliasGuid=null,LookupListViewFieldTypeIndex=2,
	TransactionAliasFieldGuid=(SELECT TransactionAliasFieldGuid  FROM [dbo].[tblTransactionAliasFields] 
			WHERE TransactionAliasGuid =@TransactionAliasGuid AND DbName='IssuePointNumber') WHERE ListViewFieldGuid=@ListViewFieldGuid
	IF (@UserDataFieldTransactionAliasGuid IS NOT NULL)
			DELETE FROM [dbo].[tblUserDataFieldTransactionAlias] WHERE @UserDataFieldTransactionAliasGuid=UserDataFieldTransactionAliasGuid
END

GO

-- WI 52216 RBAC-Personnel user group permission right assignment change
Print 'Assign rights to Personnel user group.'
GO
DELETE FROM [map].[tblGroupToRight] 
	WHERE GroupGuid IN (SELECT GroupGuid FROM [dbo].[tblGroups] WHERE GroupId='Personnel') 
			AND LookupRightIndex NOT IN (SELECT RightIndex FROM [lookup].[tblRight] WHERE RightCode IN ('MODIFY_PERSONNEL_DATA','ACCESS_ONLINE_HELP'))
INSERT INTO  [map].[tblGroupToRight] (GroupToRightGuid, GroupGuid, LookupRIghtIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT convert(uniqueidentifier, 
				hashbytes('md5',	--ok
						(
						convert(varchar(36), RightIndex)+
						convert(varchar(36), [GroupGuid])))) as GroupToRightGuid, 
		GroupGuid, RightIndex, N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec' FROM [dbo].[tblGroups] g, [lookup].[tblRight] r
WHERE GroupId='Personnel' AND RightCode IN ('MODIFY_PERSONNEL_DATA','ACCESS_ONLINE_HELP')
AND NOT EXISTS (SELECT TOP 1 1 FROM [map].[tblGroupToRight]  WHERE GroupGuid=g.GroupGuid AND r.RightIndex=LookupRightIndex )
GO
-- WI 52218 RBAC - Training user group
Print 'Assign Modify Personnel Data to Training user group.'
GO
INSERT INTO  [map].[tblGroupToRight] (GroupToRightGuid, GroupGuid, LookupRIghtIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT convert(uniqueidentifier, 
				hashbytes('md5',	--ok
						(
						convert(varchar(36), RightIndex)+
						convert(varchar(36), [GroupGuid])))) as GroupToRightGuid, 
	GroupGuid, RightIndex, N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec' FROM [dbo].[tblGroups] g, [lookup].[tblRight] r
WHERE GroupId='Training' AND RightCode IN ('MODIFY_PERSONNEL_DATA')
AND NOT EXISTS (SELECT TOP 1 1 FROM [map].[tblGroupToRight]  WHERE GroupGuid=g.GroupGuid AND r.RightIndex=LookupRightIndex )
GO
-- WI 52288 Quality RBAC permissions incorrect
Print 'Remove View Auto Distribution Configuration and View Sites and Site Groups security rights from Quality Assurance user group'

GO
DELETE FROM [map].[tblGroupToRight] 
	WHERE GroupGuid IN (SELECT GroupGuid FROM [dbo].[tblGroups] WHERE GroupId='Quality Assurance') 
			AND LookupRightIndex IN (SELECT RightIndex FROM [lookup].[tblRight] WHERE RightCode IN ('VIEW_AUTO_DISTRIBUTION_CONFIGURATION','VIEW_SITES_AND_SITE_GROUPS'))
-- WI 52290 Quality Test Sets
Print 'Assign Configure DLA Test to Quality Assurance user group.'
GO
INSERT INTO  [map].[tblGroupToRight] (GroupToRightGuid, GroupGuid, LookupRIghtIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT  convert(uniqueidentifier, 
				hashbytes('md5',	--ok
						(
						convert(varchar(36), RightIndex)+
						convert(varchar(36), [GroupGuid])))) as GroupToRightGuid, 
		GroupGuid, RightIndex, N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec' FROM [dbo].[tblGroups] g, [lookup].[tblRight] r
WHERE GroupId='Quality Assurance' AND RightCode IN ('CONFIGURE_DLA_TEST')
AND NOT EXISTS (SELECT TOP 1 1 FROM [map].[tblGroupToRight]  WHERE GroupGuid=g.GroupGuid AND r.RightIndex=LookupRightIndex )
GO


Print 'Update configuration settings.'
GO
IF (NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblConfigurationSetting Where SettingKey = 'BSME_EbsInterfaceServiceExportLogPath'))
INSERT INTO dbo.tblConfigurationSetting (ConfigurationSettingGuid, KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES
('b74ff972-17b7-404f-9b1f-c29e977906ed', 'SZ', 'BSME_EbsInterfaceServiceExportLogPath', 'C:\Program Files (x86)\FuelsManager\SAPInterface\Logs\export', N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec')
ELSE
Update dbo.tblConfigurationSetting
Set SettingValue = 'C:\Program Files (x86)\FuelsManager\SAPInterface\Logs\export'
Where SettingKey = 'BSME_EbsInterfaceServiceExportLogPath'

IF (NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblConfigurationSetting Where SettingKey = 'BSME_EbsInterfaceServiceResultsLogPath'))
INSERT INTO dbo.tblConfigurationSetting (ConfigurationSettingGuid, KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES
('2f677956-14b4-4388-b5a2-40cd747d8189', 'SZ', 'BSME_EbsInterfaceServiceResultsLogPath', 'C:\Program Files (x86)\FuelsManager\SAPInterface\Logs\results', N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec')
ELSE
Update dbo.tblConfigurationSetting
Set SettingValue = 'C:\Program Files (x86)\FuelsManager\SAPInterface\Logs\results'
Where SettingKey = 'BSME_EbsInterfaceServiceResultsLogPath'

IF (NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblConfigurationSetting Where SettingKey = 'BSME_IdeEnterpriseUploadCertificateName'))
INSERT INTO dbo.tblConfigurationSetting (ConfigurationSettingGuid, KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES
('6862ecc0-8b25-45e0-8700-4ec0424d9a08', 'SZ', 'BSME_IdeEnterpriseUploadCertificateName', 'FMDE.J6F.dla.mil', N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec')
ELSE
Update dbo.tblConfigurationSetting
Set SettingValue = 'FMDE.J6F.dla.mil'
Where SettingKey = 'BSME_IdeEnterpriseUploadCertificateName'

IF (NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblConfigurationSetting Where SettingKey = 'BSME_TavDataPath'))
INSERT INTO dbo.tblConfigurationSetting (ConfigurationSettingGuid, KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES
('f9c5ed70-a2c9-44dd-8378-65135b8e0cb1', 'SZ', 'BSME_TavDataPath', 'C:\Program Files (x86)\FuelsManager\FMDataExchangeVDir\Interfaces\TAV Data\', N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec')
ELSE
Update dbo.tblConfigurationSetting
Set SettingValue = 'C:\Program Files (x86)\FuelsManager\FMDataExchangeVDir\Interfaces\TAV Data\'
Where SettingKey = 'BSME_TavDataPath'

IF (NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblConfigurationSetting Where SettingKey = 'BSME_TavXsdPath'))
INSERT INTO dbo.tblConfigurationSetting (ConfigurationSettingGuid, KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES
('1109287e-336c-4db6-9a42-8485e127fd3d', 'SZ', 'BSME_TavXsdPath', 'C:\Program Files (x86)\FuelsManager\FMDataExchangeVDir\Interfaces\Schema', N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec')
ELSE
Update dbo.tblConfigurationSetting
Set SettingValue = 'C:\Program Files (x86)\FuelsManager\FMDataExchangeVDir\Interfaces\Schema'
Where SettingKey = 'BSME_TavXsdPath'

IF (NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblConfigurationSetting Where SettingKey = 'CustomClientScriptName'))
INSERT INTO dbo.tblConfigurationSetting (ConfigurationSettingGuid, KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES
('de3640ff-5615-4872-bad6-ea88b787296e', 'SZ', 'CustomClientScriptName', '/FuelsManager/BSMECustomScript.js', N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec')
ELSE
Update dbo.tblConfigurationSetting
Set SettingValue = '/FuelsManager/BSMECustomScript.js'
Where SettingKey = 'CustomClientScriptName'

IF (NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblConfigurationSetting Where SettingKey = 'ThreshholdPercentageForLogs'))
INSERT INTO dbo.tblConfigurationSetting (ConfigurationSettingGuid, KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES
('E46906C4-1533-4D8D-8477-F43BF7B76968', 'DWORD', 'ThreshholdPercentageForLogs', '1', N'01/01/2015 12:00:01 AM -04:00', 'Varec', N'01/01/2015 12:00:01 AM -04:00', 'Varec')
ELSE
Update dbo.tblConfigurationSetting
Set SettingValue = '1'
Where SettingKey = 'ThreshholdPercentageForLogs'
GO

DELETE FROM dbo.tblConfigurationSetting where SettingKey = 'ThershholdPerctangeForLogs'
GO


IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblconfigurationsetting WHERE SettingKey='SecurityAlertReceipients')
 insert into dbo.tblconfigurationsetting (ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, 
createddate, createdby, updateddate,updatedby) 
values ('9048c384-9491-474e-8915-1372f952a5a2', 'SZ', 'SecurityAlertReceipients', NULL, N'01/01/2015 12:00:01 AM -04:00', 'varec', N'01/01/2015 12:00:01 AM -04:00', 'varec')

IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblconfigurationsetting WHERE SettingKey='SecurityAlertSubject')
insert into dbo.tblconfigurationsetting (ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, 
createddate, createdby, updateddate,updatedby) 
values ('4d613367-a86d-474f-8316-f3f1e75fb84b', 'SZ', 'SecurityAlertSubject', 'Database stored procedure/function change alert.', N'01/01/2015 12:00:01 AM -04:00', 'varec', N'01/01/2015 12:00:01 AM -04:00', 'varec')

IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblconfigurationsetting WHERE SettingKey='SecurityAlertFrom')
insert into dbo.tblconfigurationsetting (ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, 
createddate, createdby, updateddate,updatedby) 
values ('440aeaa1-88ca-4ad2-ac6f-5df4b591b37b', 'SZ', 'SecurityAlertFrom', NULL, N'01/01/2015 12:00:01 AM -04:00', 'varec', N'01/01/2015 12:00:01 AM -04:00', 'varec')

IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblconfigurationsetting WHERE SettingKey='SecurityAlertSmtpServer')
insert into dbo.tblconfigurationsetting (ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, 
createddate, createdby, updateddate,updatedby) 
values ('782578ae-f1d2-49ae-9d77-847b984ebc4e', 'SZ', 'SecurityAlertSmtpServer', NULL, N'01/01/2015 12:00:01 AM -04:00', 'varec', N'01/01/2015 12:00:01 AM -04:00', 'varec')

IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.tblconfigurationsetting WHERE SettingKey='SecurityAlertSmtpPort')
insert into dbo.tblconfigurationsetting (ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, 
createddate, createdby, updateddate,updatedby) 
values ('782578ae-f1d2-49ae-9d77-847b984ebc4f', 'DWORD', 'SecurityAlertSmtpPort', NULL, N'01/01/2015 12:00:01 AM -04:00', 'varec', N'01/01/2015 12:00:01 AM -04:00', 'varec')



GO




DECLARE @SiteAdminSiteGuid uniqueidentifier = CONVERT(uniqueidentifier,'00000000-0000-0000-0000-000000000001' )

if not exists(select 1 from dbo.tblDataDictionaries where SiteGuid = @SiteAdminSiteGuid and [KEY] = 'Provider Name')
BEGIN
	INSERT INTO dbo.tblDataDictionaries ([DataDictionaryGuid], SiteGuid, [Key], [Value], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values
		('3C31D9D6-AA9A-4A99-A521-E30FAA25192A', @SiteAdminSiteGuid, 'Provider Name', 'Description', N'01/01/2015 12:00:01 AM -04:00', 'Varec',N'01/01/2015 12:00:01 AM -04:00', 'Varec')
END

if not exists(select 1 from dbo.tblDataDictionaries where SiteGuid = @SiteAdminSiteGuid and [KEY] = 'Fuel Card Number')
BEGIN
	INSERT INTO dbo.tblDataDictionaries ([DataDictionaryGuid], SiteGuid, [Key], [Value], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values
		('69A08E2D-DA0A-4909-9DF3-271F1F1C6340', @SiteAdminSiteGuid, 'Fuel Card Number', 'Activity ID', N'01/01/2015 12:00:01 AM -04:00', 'Varec',N'01/01/2015 12:00:01 AM -04:00', 'Varec')
END
GO
-- WI 52646 - Vehicle appears twice in the Assigned Drop Down List Box
Print 'Map equipment type Railcar to to string railcar.'
GO
UPDATE [dbo].[tblDataDictionaries] SET [Value] = 'Railcar' WHERE [Key] = 'Railcar' AND [Value] = 'Vehicle'
GO

-- WI 52878 Maintenance User Group - Scheduler and Dispatch Access Missing
Print 'Remove Modify_Appointments right from Maintenance user group.'
GO
DELETE FROM [map].[tblGroupToRight] 
WHERE EXISTS (SELECT TOP 1 1 FROM [dbo].[tblGroups] g, [lookup].[tblRight] r
WHERE GroupId IN ('Maintenance') AND RightCode IN ('MODIFY_APPOINTMENTS')
AND [map].[tblGroupToRight].GroupGuid=g.GroupGuid AND r.RightIndex=[map].[tblGroupToRight].LookupRightIndex )
GO

-- WI 52880 - Dispatch RBAC - Cannot Modify Appointments
Print 'Add Modify_Person_Training and Execute_Quality_Tests rights to Dispatch user group.'
GO
INSERT INTO [Map].[tblGroupToRight]
(GroupToRightGuid, GroupGuid, LookupRightIndex, CreatedDate, CreatedBy)
SELECT convert(uniqueidentifier,
		hashbytes('md5',	--ok
			(convert(varchar(36), r.RightIndex)+
			convert(varchar(36), GroupGuid)))) as GroupToRightGuid,
groupGuid, r.RightIndex , N'01/01/2015 12:00:01 AM -04:00', 'Varec' FROM 
[dbo].[tblGroups] g,[lookup].[tblRight] r where GroupID in ('Dispatch') AND r.RightCode IN ('EXECUTE_QUALITY_TESTS','MODIFY_PERSON_TRAINING')
AND NOT EXISTS(SELECT 1 FROM [Map].[tblGroupToRight]
WHERE g.GroupGuid=GroupGuid AND r.RightIndex=LookupRightIndex)
GO

-- WI 52905 MFCS RBAC - Cannot View Ledger
Print 'Add MFCS user group to user groups that can view ledger.'
GO
INSERT INTO map.tblGroupToLedgerView ( 
					   GroupGuid, 
					   ListViewGuid, 
					   CreatedDate, 
					   CreatedBy, 
					   UpdatedDate, 
					   UpdatedBy, 
					  GroupToLedgerViewGuid
					  ) 
SELECT g.GroupGuid, ListViewGuid, 
					  N'01/01/2015 12:00:01 AM -04:00', 
					   'varec', 
					   N'01/01/2015 12:00:01 AM -04:00', 
					  'varec', 
					   convert(uniqueidentifier, 
					hashbytes('md5',	--ok
						(
						convert(varchar(36), ListViewGuid)+
						convert(varchar(36), g.GroupGuid)))) as GroupToLedgerViewGuid
					  FROM dbo.tblGroups g, (SELECT distinct ListViewGuid FROM [map].[tblGroupToLedgerView]) v
		WHERE GroupId='MFCS' 
		AND NOT EXISTS(SELECT TOP 1 1 FROM [map].[tblGroupToLedgerView] 
		WHERE g.GroupGuid=GroupGuid and v.ListViewGuid=ListViewGuid);

GO
		
-- WI 53205 - Administrator RBAC - Should Not Have Import Functions		
-- WI 52911 Administrator RBAC - Can See Ledger but Cannot Modify Anything
Print 'Remove Import transaction data, Undelete transaction data and modify error transactions right from Administrator user group.'
GO
DELETE FROM [map].[tblGroupToRight] 
WHERE EXISTS (SELECT TOP 1 1 FROM [dbo].[tblGroups] g, [lookup].[tblRight] r
WHERE GroupId IN ('Administrator') 
AND RightCode IN (	'ACCESS_ARTS', 
					'ACCESS_MFCS',
					'BASE_EXPORT',
					'BASE_EXPORT_MANUAL',
					'ENTERPRISE_EXPORT', 
					'EXECUTE_IMPORT_EXPORT', 
					'EXPORT_ENTERPRISE_DATA', 
					'IMPORT_ENTERPRISE_DATA',
					'INTERFACE_IMPORT',
					'MODIFY_ERROR_TRANSACTION', 
					'MODIFY_TRANSACTION_DATA', 
					'PERFORM_REVERSE_TRANSACTION',
					'RAPS_IMPORT', 
					'SEND_TO_EBS',
					'UNDELETE_TRANSACTION_DATA')
AND [map].[tblGroupToRight].GroupGuid=g.GroupGuid AND r.RightIndex=[map].[tblGroupToRight].LookupRightIndex )
GO

-- Allow administrator only to  view the transactions in Transaction Detail page
DELETE FROM [map].[tblGroupToTransactionAlias]  FROM
	[map].[tblGroupToTransactionAlias] ga 
	JOIN [dbo].[tblTransactionAliases] a ON ga.TransactionAliasGuid=a.TransactionAliasGuid
	JOIN [dbo].[tblGroups] g ON g.groupguid=ga.groupguid
  WHERE GroupId='Administrator' 

DELETE FROM m FROM map.tblUserToGroup m 
	JOIN dbo.tblGroups g ON m.GroupGuid = g.GroupGuid
	JOIN dbo.tblUsers u ON u.UserGuid=m.UserGuid
	WHERE userid='administrator' AND GroupID <> 'Administrator'

GO

INSERT INTO [map].[tblGroupToTransactionAlias] 
	( [GroupToTransactionAliasGuid], GroupGuid, TransactionAliasGuid,LookupRightIndex,createddate, createdby,updateddate,updatedby)
	SELECT convert(uniqueidentifier, --ok
				hashbytes('md5',	
						(convert(varchar(36),0)+
						convert(varchar(36), a.[TransactionAliasGuid])+
						convert(varchar(36), g.[GroupGuid])))),
		   g.[GroupGuid]
		  ,a.[TransactionAliasGuid]
		  ,0 AS[LookupRightIndex] --VIEW     
		  ,N'01/01/2015 12:00:01 AM -04:00'
		  ,'Varec'
		  ,N'01/01/2015 12:00:01 AM -04:00'
		  ,'Varec'
	FROM	[dbo].[tblTransactionAliases] a, 
			[dbo].[tblGroups] g 
	WHERE GroupId='Administrator' 
	AND NOT EXISTS(SELECT TOP 1 1 FROM [map].[tblGroupToTransactionAlias] WHERE
	GroupGuid=g.GroupGuid AND TransactionAliasGuid = a.[TransactionAliasGuid] AND LookupRightIndex=0)
GO 
 
		
-- WI 52946 RAPS RBAC - Cannot View Ledger/Modify Transactions
Print 'Remove Perform Reverse Transaction, Undelete transaction data and modify error transactions rights from RAPS user group.'
GO
DELETE FROM [map].[tblGroupToRight] 
WHERE EXISTS (SELECT TOP 1 1 FROM [dbo].[tblGroups] g, [lookup].[tblRight] r
WHERE GroupId IN ('RAPS') AND RightCode IN ('PERFORM_REVERSE_TRANSACTION', 'UNDELETE_TRANSACTION_DATA','MODIFY_ERROR_TRANSACTION')
AND [map].[tblGroupToRight].GroupGuid=g.GroupGuid AND r.RightIndex=[map].[tblGroupToRight].LookupRightIndex )
GO

-- WI 53107 MFCS RBAC - Cannot View/Modify Transactions
Print 'Allow MFCS user group to  edit the transactions in Transaction Detail page.'
GO

DELETE FROM [map].[tblGroupToTransactionAlias]  FROM
	[map].[tblGroupToTransactionAlias] ga 
	JOIN [dbo].[tblTransactionAliases] a ON ga.TransactionAliasGuid=a.TransactionAliasGuid
	JOIN [dbo].[tblGroups] g ON g.groupguid=ga.groupguid
  WHERE GroupId='MFCS' 
GO

INSERT INTO [map].[tblGroupToTransactionAlias] 
	( [GroupToTransactionAliasGuid], GroupGuid, TransactionAliasGuid,LookupRightIndex,createddate, createdby,updateddate,updatedby)
	SELECT convert(uniqueidentifier, --ok
				hashbytes('md5',	
						(convert(varchar(36),1)+
						convert(varchar(36), a.[TransactionAliasGuid])+
						convert(varchar(36), g.[GroupGuid])))),
		   g.[GroupGuid]
		  ,a.[TransactionAliasGuid]
		  ,(CASE WHEN a.aliasname = 'Movement' THEN 0 ELSE 1 END) AS[LookupRightIndex] --VIEW=0 only for movement, others MODIFY=1    
		  ,N'01/01/2015 12:00:01 AM -04:00'
		  ,'Varec'
		  ,N'01/01/2015 12:00:01 AM -04:00'
		  ,'Varec'
	FROM	[dbo].[tblTransactionAliases] a, 
			[dbo].[tblGroups] g 
	WHERE GroupId='MFCS' 
	AND NOT EXISTS(SELECT TOP 1 1 FROM [map].[tblGroupToTransactionAlias] WHERE
	GroupGuid=g.GroupGuid AND TransactionAliasGuid = a.[TransactionAliasGuid] AND LookupRightIndex=1)
GO 


-- WI 53036 Unobtainable within Product Configuration
Print 'Allow Unobtainable to be editable in Product Configuration.'
GO
BEGIN TRANSACTION
DECLARE @T TABLE (EntitySegmentTemplateGuid UNIQUEIDENTIFIER, SiteGroupGuid UNIQUEIDENTIFIER)

INSERT INTO @T (EntitySegmentTemplateGuid, SiteGroupGuid)
SELECT EntitySegmentTemplateGuid, SiteGuid
	FROM dbo.tblSites s, erv.tblEntitySegmentTemplate e
	WHERE s.SiteGroupFlag=1 AND e.EntityTypeId='Product'
	AND NOT EXISTS(SELECT TOP 1 1 FROM  [erv].[tblEntityRecordVersioningFieldConfig] t WHERE
		t.EntitySegmentTemplateGuid=e.EntitySegmentTemplateGuid AND s.SiteGuid=t.SiteGroupGuid 
		AND TargetField='UserData5') 

INSERT INTO [erv].[tblEntityRecordVersioningFieldConfig]
(FieldConfigGuid, EntitySegmentTemplateGuid, SiteGroupGuid, TargetField, IsExternalAttribute, ForwardControlMode,
CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT convert(uniqueidentifier, --ok
				hashbytes('md5',	
						('UserData5'+ --TargetField = Product Unobtainable
						convert(varchar(36), SiteGroupGuid)+ --
						convert(varchar(36), EntitySegmentTemplateGuid)))), 
						EntitySegmentTemplateGuid, 
						SiteGroupGuid, 'UserData5', 0, 'VersionSpecific', 
						N'01/01/2015 12:00:01 AM -04:00', 'Varec',  N'01/01/2015 12:00:01 AM -04:00', 'Varec'
	FROM @T 

DECLARE initCur CURSOR FOR SELECT EntitySegmentTemplateGuid, SiteGroupGuid    FROM  @T 

DECLARE @EntitySegmentTemplateGuid UNIQUEIDENTIFIER
DECLARE @SiteGroupGuid UNIQUEIDENTIFIER
OPEN initCur
FETCH FROM initCur INTO @EntitySegmentTemplateGuid, @SiteGroupGuid
WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC [erv].[usp_EnforceFLCChangesOnProductRecordVersioning]	@EntitySegmentTemplateGuid, @SiteGroupGuid, 'Varec', 'OFF_TO_ON'
	FETCH FROM initCur INTO @EntitySegmentTemplateGuid, @SiteGroupGuid
END
CLOSE initCur
DEALLOCATE initCur
COMMIT TRANSACTION
GO





/* {CheckPoint: Add dbo.tblBSMEEBSLastResult to the Sync Configuration Tables and Default Synchronization Profile} */

-- If the SyncScope - Levels should be created in the core dacpac to avoid future collisions for now.

-- If the SyncTable record for dbo.tblBSMEEBSLastResult does not exist, create it
IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] st WHERE st.[TableName] = N'dbo.tblBSMEEBSLastResult')
BEGIN
	-- dbo.tblBSMEEBSLastResult
	INSERT INTO [sync].[tblSyncTable]
			   ([SyncTableGuid],[TableName],[SyncDependencyGroupGuid],[LastSchemaDate]
			   ,[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[IsSiteFilteredFlag],[IsSiteFilteredOnDeleteFlag])
		 VALUES
			   (N'18a1459d-f02b-4b4c-b4c5-f3b3186f77a2',N'dbo.tblBSMEEBSLastResult',N'e50c8831-c1cd-4284-8737-a0c100d0a539', '2015-09-11 10:41:16.1678647 -04:00'
			   ,'2015-09-11 10:41:16.1678647 -04:00',N'Administrator','2015-09-11 10:41:16.1678647 -04:00',N'Administrator',1,0)
END

-- If the SyncTableToScopeMap record doesn't exist, create a new record and the supporting records in the child tables
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblBSMEEBSLastResult'
									AND sttsm.[ID] = N'tblBSMEEBSLastResult')
BEGIN
	-- Insert tblSyncTableToScopeMap record for dbo.tblBSMEEBSLastResult - Level9d
	INSERT [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'27201ae5-7246-47cc-b560-e3d382414a3e', N'tblBSMEEBSLastResult', N'e1edbf11-908a-4ca6-b1c4-0b769bbb136c', N'18a1459d-f02b-4b4c-b4c5-f3b3186f77a2', 0, 2, 0, 0, NULL, NULL, N'9/11/2015 11:44:43 AM -04:00', NULL, N'9/11/2015 11:44:43 AM -04:00', NULL)

	-- Insert tblSyncTableToScopeMapCommand record for dbo.tblBSMEEBSLastResult 
	INSERT [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3d7d35d3-839b-41dc-a9c3-f847d39378c5', N'27201ae5-7246-47cc-b560-e3d382414a3e', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblBSMEEBSLastResult', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblBSMEEBSLastResult', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblBSMEEBSLastResult', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblBSMEEBSLastResult', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblBSMEEBSLastResult', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblBSMEEBSLastResult', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblBSMEEBSLastResult', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblBSMEEBSLastResult', N'2015-09-11 11:44:43.0000000 -04:00', NULL, N'2015-09-11 11:44:43.0000000 -04:00', NULL)

	-- Insert tblSyncTableToScopeMapColumn records for dbo.tblBSMEEBSLastResult 
	INSERT [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9d2a715b-296e-4c40-acc5-1299f8c7542c', N'27201ae5-7246-47cc-b560-e3d382414a3e', N'TransactionGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'9/11/2015 11:44:43 AM -04:00', NULL, N'9/11/2015 11:44:43 AM -04:00', NULL)
	INSERT [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b373c389-316a-4bce-a53e-34aecde6e744', N'27201ae5-7246-47cc-b560-e3d382414a3e', N'ExportResultDetailGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'9/11/2015 11:44:43 AM -04:00', NULL, N'9/11/2015 11:44:43 AM -04:00', NULL)
	INSERT [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3ec1a3b2-1783-4f52-a532-98bf167c469f', N'27201ae5-7246-47cc-b560-e3d382414a3e', N'BsmeEbsLastResultGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'9/11/2015 11:44:43 AM -04:00', NULL, N'9/11/2015 11:44:43 AM -04:00', NULL)
	INSERT [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bc672952-d0c2-4f6a-be77-77d57f590ce5', N'27201ae5-7246-47cc-b560-e3d382414a3e', N'CreatedDate', 3, N'DateTimeOffset', 10, 34, 7, 1, 0, 0, N'9/11/2015 11:44:43 AM -04:00', NULL, N'9/11/2015 11:44:43 AM -04:00', NULL)
	INSERT [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f1d94994-19d3-46e1-a9ad-059b71b4d228', N'27201ae5-7246-47cc-b560-e3d382414a3e', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 1, 0, 0, N'9/11/2015 11:44:43 AM -04:00', NULL, N'9/11/2015 11:44:43 AM -04:00', NULL)
	INSERT [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a91f1270-4446-4fc3-a4c3-cf9386b8347f', N'27201ae5-7246-47cc-b560-e3d382414a3e', N'UpdatedDate', 5, N'DateTimeOffset', 10, 34, 7, 1, 0, 0, N'9/11/2015 11:44:43 AM -04:00', NULL, N'9/11/2015 11:44:43 AM -04:00', NULL)
	INSERT [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'aed17673-fa0c-428a-963f-1b35ec7965db', N'27201ae5-7246-47cc-b560-e3d382414a3e', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 1, 0, 0, N'9/11/2015 11:44:43 AM -04:00', NULL, N'9/11/2015 11:44:43 AM -04:00', NULL)
END

GO



--WI 54115
DELETE FROM [map].[tblGroupToRight] 
WHERE LookupRightIndex in ( 
116,117,	-- incoming truck data
154, 155,	-- price list
33, 34,		-- ticketing data
101, 102,	-- Wac
146, 147,	-- field level configurations
7			-- installed module status
)
GO

--WI 54104, 54103
DELETE gr 
FROM [map].[tblGroupToRight] gr
inner join tblgroups g on gr.groupguid  = g.GroupGuid
WHERE LookupRightIndex in ( 
105, -- base export
124 -- base export manual
)
and g.GroupID in ('Enterprise')

INSERT INTO 
[map].[tblGroupToRight] 
(GroupToRightGuid, GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT newid(), g.groupguid, r.lookuprightindex, N'01/01/2015 12:00:01 AM -04:00', 'Migration',N'01/01/2015 12:00:01 AM -04:00', 'Migration'
FROM tblgroups g 
CROSS APPLY (SELECT 105 AS 'LookupRightIndex' UNION SELECT 124) r
left join map.tblgrouptoRight gr ON g.GroupGuid = gr.GroupGuid AND gr.LookupRightIndex = r.LookupRightIndex
WHERE g.GroupID ='Limited Upload' and gr.LookupRIghtIndex is null
GO



-- 54329 - Web Dispatch Validations
PRINT 'Configure Web Dispatch'
GO
BEGIN TRANSACTION
INSERT INTO [dbo].[tblDispatchConfiguration] (
	 [DispatchConfigurationGuid]
      ,[SiteGuid]
      ,[ID]
      ,[DisplayCurrentTime]
      ,[DispatchDataRefreshPeriod]
      ,[TabularViewDisplayMilitaryDate]
      ,[QuantityNotZeroCheck]
      ,[ExactlyOneManagerCheck]
      ,[ExactlyOneOwnerCheck]
      ,[DispatchFuelAdditiveFlagCheck]
      ,[FastLogFuelAdditiveFlagCheck]
      ,[FillstandVolumeWithinToleranceCheck]
      ,[ReturnToBulkVolumeWithinToleranceCheck]
      ,[RecirculationVolumesGreaterThanZeroCheck]
      ,[OperatorIsInCheck]
      ,[OperatorNotAssignedCheck]
      ,[OperatorHasRequiredTrainingCheck]
      ,[OperatorTrainingNotExpiredCheck]
      ,[OperatorNotLockedOutCheck]
      ,[OperatorHasRequiredQualificationsCheck]
      ,[OperatorQualificationsNotExpiredCheck]
      ,[DefuelStatusCheck]
      ,[RefuelStatusCheck]
      ,[EquipmentFuelGradeCheck]
      ,[EquipmentNotLockedOutCheck]
      ,[EquipmentNotAssignedCheck]
      ,[EquipmentInServiceCheck]
      ,[TagLicenseNotExpiredCheck]
      ,[TestInspectionNotExpiredCheck]
      ,[QualityControlCheckupDateCheck]
      ,[CautionQualityTagCheck]
      ,[WarningQualityTagCheck]
      ,[DangerQualityTagCheck]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[EnableServiceRequests]
      ,[AutomaticRestartDelay]
      ,[EquipmentRequired]
      ,[PersonnelRequired]
      ,[FillToActualOrStandard]
      ,[OperationalWindowPastHours]
      ,[OperationalWindowFutureHours]
      ,[ShowGridLines]
      ,[StaticTimeDisplay]
      ,[UseArrivalTime]
      ,[UseStartTime]
      ,[UseStopTime]
      ,[FuelsManagerReportURL])
  SELECT 
	   [SiteGuid] AS [DispatchConfigurationGuid]
      ,[SiteGuid]
      ,'Dispatch Configuration' AS [ID]
      ,1 AS [DisplayCurrentTime]
      ,5 AS [DispatchDataRefreshPeriod]
      ,1 AS [TabularViewDisplayMilitaryDate]
      ,1 AS [QuantityNotZeroCheck]
      ,0 AS [ExactlyOneManagerCheck]
      ,0 AS [ExactlyOneOwnerCheck]
      ,1 AS [DispatchFuelAdditiveFlagCheck]
      ,1 AS [FastLogFuelAdditiveFlagCheck]
      ,1 AS [FillstandVolumeWithinToleranceCheck]
      ,1 AS [ReturnToBulkVolumeWithinToleranceCheck]
      ,1 AS [RecirculationVolumesGreaterThanZeroCheck]
      ,1 AS [OperatorIsInCheck]
      ,1 AS [OperatorNotAssignedCheck]
      ,1 AS [OperatorHasRequiredTrainingCheck]
      ,1 AS [OperatorTrainingNotExpiredCheck]
      ,1 AS [OperatorNotLockedOutCheck]
      ,1 AS [OperatorHasRequiredQualificationsCheck]
      ,1 AS [OperatorQualificationsNotExpiredCheck]
      ,1 AS [DefuelStatusCheck]
      ,1 AS [RefuelStatusCheck]
      ,1 AS [EquipmentFuelGradeCheck]
      ,1 AS [EquipmentNotLockedOutCheck]
      ,1 AS [EquipmentNotAssignedCheck]
      ,1 AS [EquipmentInServiceCheck]
      ,0 AS [TagLicenseNotExpiredCheck]
      ,1 AS [TestInspectionNotExpiredCheck]
      ,1 AS [QualityControlCheckupDateCheck]
      ,1 AS [CautionQualityTagCheck]
      ,1 AS [WarningQualityTagCheck]
      ,1 AS [DangerQualityTagCheck]
      ,N'01/01/2015 12:00:01 AM -04:00' AS [CreatedDate]
      ,'Varec' AS [CreatedBy]
      ,N'01/01/2015 12:00:01 AM -04:00' AS [UpdatedDate]
      ,'Varec' AS [UpdatedBy]
      ,1 AS [EnableServiceRequests]
      ,30 AS [AutomaticRestartDelay]
      ,1 AS [EquipmentRequired]
      ,1 AS [PersonnelRequired]
      ,1 AS [FillToActualOrStandard]
      ,8 AS [OperationalWindowPastHours]
      ,16 AS [OperationalWindowFutureHours]
      ,0 AS [ShowGridLines]
      ,0 AS [StaticTimeDisplay]
      ,1 AS [UseArrivalTime]
      ,1 AS [UseStartTime]
      ,1 AS [UseStopTime]
      ,'../FMReportWebMain/ReportLandingPage.aspx' AS [FuelsManagerReportURL]
	  FROM dbo.tblSites 
	  WHERE SiteGuid NOT IN (SELECT SiteGuid FROM  [dbo].[tblDispatchConfiguration])

INSERT INTO [map].[tblEntityDispatchConfigurationToSite]
(
	 [DispatchConfigurationToSiteGuid]
      ,[DispatchConfigurationGuid]
      ,[SiteGuid]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[AssignedFromSiteGuid]
	  )
SELECT  SiteGuid AS [DispatchConfigurationToSiteGuid]
      ,[DispatchConfigurationGuid]
      ,[SiteGuid]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,SiteGuid AS [AssignedFromSiteGuid] FROM [dbo].[tblDispatchConfiguration]
	  WHERE SiteGuid NOT IN (SELECT [SiteGuid] FROM [map].[tblEntityDispatchConfigurationToSite])
COMMIT TRANSACTION
GO

-- 55767 - Needed modifications to FMD v9 to enable Flightline functionality.
PRINT 'Configure Web Dispatch'
GO
BEGIN TRANSACTION

DECLARE @CreatedDate Date = GetUTCDate()	
DECLARE @CreatedBy NVarchar(16) = 'Administrator'	
			
			
IF (NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblGroups] WHERE GroupID='Handheld Operators' AND SiteGuid = '00000000-0000-0000-0000-000000000001'))
	INSERT INTO [dbo].[tblGroups] ([GroupID], [GroupDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [GroupGuid], [SiteGuid]) 
	VALUES  
	('Handheld Operators', 'Handheld Operators', @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy,'AECEFB60-E6DD-4D49-946E-3DC2FB1A02DE', '00000000-0000-0000-0000-000000000001' )

COMMIT TRANSACTION

GO
PRINT 'Update Sync batch count to 500 for transactions and exportresults (WI 56146)'
UPDATE [Sync].[tblSyncTableToScopeMap] SET [MaxBatchSegmentRowCount] = 500 WHERE [ID] IN ('tblTransactions','tblExportResults') and [MaxBatchSegmentRowCount] > 500
GO