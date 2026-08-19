/*************************************************'
* Script.IncrementalDataMaintenance.sql file
* Use this file for include scripts for:
* 1. Insert data into a table that already has data (e.g. new entry into an already populated lookup table). It is required that the insert script verifies whether the inserting record does not exist).
* 2. Update the content of a record(s) present in a table
* 3. Delete records from a table 
**************************************************/


IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceSendPollingInterval' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'DWORD', 'NSPA_SAPInterfaceServiceSendPollingInterval', 15, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceExportEnabled' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'DWORD', 'NSPA_SAPInterfaceServiceExportEnabled', 1, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceExportNumberOfRetries' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'DWORD', 'NSPA_SAPInterfaceServiceExportNumberOfRetries', 3, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceExportRetryInterval' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'DWORD', 'NSPA_SAPInterfaceServiceExportRetryInterval', 3, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceImportLogPath' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'SZ', 'NSPA_SAPInterfaceServiceImportLogPath', 'C:\temp', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceExportLogPath' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'SZ', 'NSPA_SAPInterfaceServiceExportLogPath', 'C:\temp', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceNMBSUserId' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'SZ', 'NSPA_SAPInterfaceServiceNMBSUserId', 'user', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceNMBSPassword' AND KeyType = 'SZ')
BEGIN
	DELETE FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceNMBSPassword' AND KeyType = 'SZ'
END 

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceNMBSPassword' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'PWD', 'NSPA_SAPInterfaceServiceNMBSPassword', 'k9nabX4lwcnY073bj+VNsg==', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceResultsImportUserId' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'SZ', 'NSPA_SAPInterfaceServiceResultsImportUserId', 'user', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END


IF EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceResultsImportPassword' AND KeyType = 'SZ')
BEGIN
	DELETE FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceResultsImportPassword' AND KeyType = 'SZ'
END 

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceResultsImportPassword' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'PWD', 'NSPA_SAPInterfaceServiceResultsImportPassword', 'k9nabX4lwcnY073bj+VNsg==', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceNMBSUri' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'SZ', 'NSPA_SAPInterfaceServiceNMBSUri', 'http://localhost:8080/NMBS.svc', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceNMBSConnectTimeout' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'DWORD', 'NSPA_SAPInterfaceServiceNMBSConnectTimeout', '60', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF ((SELECT COUNT(*) FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 1036 OR MenuItemTypeCode = 'ACCOUNTING_ERROR_SUMMARY') = 0)
BEGIN
	INSERT INTO lookup.tblMenuItemType
		(MenuItemTypeIndex, MenuItemTypeCode, MenuItemTypeName, MenuItemTypeGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		(1036, N'ACCOUNTING_ERROR_SUMMARY', N'ACCOUNTING_ERROR_SUMMARY', N'F63082F0-6465-444A-90EC-A6C2812DD2A2',  SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceMonthsArchiveRetention' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'DWORD', 'NSPA_SAPInterfaceServiceMonthsArchiveRetention', 3, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceExportAuthenticationMethod' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'SZ', 'NSPA_SAPInterfaceServiceExportAuthenticationMethod', 'Windows', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceExportSimulatorEnvironment' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'DWORD', 'NSPA_SAPInterfaceServiceExportSimulatorEnvironment', 0, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_SAPInterfaceServiceNMBSDomain' )
BEGIN
	INSERT tblConfigurationSetting  ( ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy	)
	SELECT newid(), 'SZ', 'NSPA_SAPInterfaceServiceNMBSDomain', 'NSPA', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator'
END

IF EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'NspaEnterprise' )
BEGIN
	DELETE FROM tblConfigurationSetting WHERE SettingKey = 'NspaEnterprise'
END

Declare @assem nvarchar(1000)
Declare @nspaDLL nvarchar(1000)

set @assem = (select SettingValue from dbo.tblConfigurationSetting where SettingKey = 'ISecurityAssemblies')
set @nspaDLL = 'NspaBusinessObjects.dll;'
set @assem = ISNULL(@assem, '')

if (charindex(@nspaDLL, @assem) <= 0)
	Update tblConfigurationSetting Set SettingValue = @assem + @nspaDLL where SettingKey = 'ISecurityAssemblies'

IF NOT EXISTS ( SELECT 1 
	FROM tblConfigurationSetting cs
	WHERE cs.SettingKey = 'IDiscoveryAssemblies'
	AND cs.SettingValue LIKE '%NspaInterfaceLibrary.dll%' )

BEGIN
	UPDATE tblConfigurationSetting 
	SET SettingValue = SettingValue + 'NspaInterfaceLibrary.dll;'
	WHERE SettingKey = 'IDiscoveryAssemblies'
END

set @assem = (select SettingValue from dbo.tblConfigurationSetting where SettingKey = 'IDiscoveryAssemblies')
set @nspaDLL = 'NspaWebApp.dll;'
set @assem = ISNULL(@assem, '')

if (charindex(@nspaDLL, @assem) <= 0)
	Update tblConfigurationSetting Set SettingValue = @assem + @nspaDLL where SettingKey = 'IDiscoveryAssemblies'

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS WHERE CONSTRAINT_NAME = 'CK_tblTransactions_DocumentNumberUniqueness')
BEGIN
	ALTER TABLE [dbo].[tblTransactions]
		ADD CONSTRAINT [CK_tblTransactions_DocumentNumberUniqueness]
		CHECK (([dbo].[udf_IsDocumentNumberUnique]([TransactionGuid],[SiteGuid],[TransactionAliasGuid],[ReversalType],[ConjoinTransID],[TransID],[DocumentNumber],[DeleteFlag])=(1)))
END
ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS WHERE CONSTRAINT_NAME = 'CK_tblTransactions_DocumentNumberUniqueness' AND CHECK_CLAUSE = '([dbo].[udf_IsDocumentNumberUnique]([TransactionGuid],[SiteGuid],[TransactionAliasGuid],[ReversalType],[ConjoinTransID],[TransID],[DocumentNumber],[DeleteFlag])=(1))')
BEGIN
	ALTER TABLE [dbo].[tblTransactions] 
		DROP CONSTRAINT [CK_tblTransactions_DocumentNumberUniqueness]

	ALTER TABLE [dbo].[tblTransactions]
		ADD CONSTRAINT [CK_tblTransactions_DocumentNumberUniqueness]
		CHECK (([dbo].[udf_IsDocumentNumberUnique]([TransactionGuid],[SiteGuid],[TransactionAliasGuid],[ReversalType],[ConjoinTransID],[TransID],[DocumentNumber],[DeleteFlag])=(1)))
END
