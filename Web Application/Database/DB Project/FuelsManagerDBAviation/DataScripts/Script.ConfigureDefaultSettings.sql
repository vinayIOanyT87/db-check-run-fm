-- For Aviation, we'll disable the change tracking triggers from tracking changes in the track tables.
-- If synchronization needs to be enabled at a later date, there is a script that can "prime" the
-- tracking tables in preparation for synchronization.
IF (SELECT COUNT(*) FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'SyncEnabled') = 0
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting]
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(N'603D5775-B8CE-4DB5-B842-CC07FF39B327', 'DWORD', 'SyncEnabled', '0', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END
ELSE
BEGIN
	UPDATE [dbo].[tblConfigurationSetting] SET [SettingValue] = '0' WHERE [SettingKey] = 'SyncEnabled'
END
