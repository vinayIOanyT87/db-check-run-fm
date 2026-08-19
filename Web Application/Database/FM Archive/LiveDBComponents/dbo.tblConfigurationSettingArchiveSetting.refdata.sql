SET NOCOUNT ON

PRINT 'Processing Archiving-specific Static Reference Data for table [dbo].[tblConfigurationSetting]'
PRINT ''

DECLARE @ConfigurationSettingArchiveRefDataInserted bigint
DECLARE @ConfigurationSettingArchiveRefDataUpdated bigint
DECLARE @ConfigurationSettingArchiveRefDataDeleted bigint

SET @ConfigurationSettingArchiveRefDataInserted = 0
SET @ConfigurationSettingArchiveRefDataUpdated = 0
SET @ConfigurationSettingArchiveRefDataDeleted = 0

DECLARE @tblConfigurationSettingArchiveRefData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldConfigurationSettingGuid] UNIQUEIDENTIFIER
	,[ConfigurationSettingGuid] UNIQUEIDENTIFIER
    ,[OldKeyType] NVARCHAR (8)
    ,[KeyType] NVARCHAR (8)
	,[OldSettingKey] NVARCHAR (50)
    ,[SettingKey] NVARCHAR (50)
	,[OldSettingValue] NVARCHAR (1000)
    ,[SettingValue] NVARCHAR (1000)
    ,[OldCreatedDate] DATETIMEOFFSET (7)
    ,[CreatedDate] DATETIMEOFFSET (7)
    ,[OldCreatedBy] NVARCHAR (255)
    ,[CreatedBy] NVARCHAR (255)
    ,[OldUpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[OldUpdatedBy] NVARCHAR (255)
    ,[UpdatedBy] NVARCHAR (255)
    ,[_OldClusterIdx] BIGINT
	,[_ClusterIdx] BIGINT
);

DECLARE @tblConfigurationSettingArchiveData TABLE
(
    [ConfigurationSettingGuid] UNIQUEIDENTIFIER
	,[KeyType] NVARCHAR (8)
	,[SettingKey] NVARCHAR (50)
	,[SettingValue] NVARCHAR (1000)
    ,[CreatedDate] DATETIMEOFFSET (7)
    ,[CreatedBy] NVARCHAR (255)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedBy] NVARCHAR (255)	
);


INSERT INTO @tblConfigurationSettingArchiveData
(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
VALUES
('BDEFBF89-BA8E-468D-A419-519C0D4EC84D', 'DWORD', N'ArchiveRetentionPeriodInMonths', N'36', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')


; MERGE INTO [dbo].[tblConfigurationSetting] AS Target
USING 
(
	SELECT [ConfigurationSettingGuid], 
	[KeyType],
	[SettingKey],
	[SettingValue],
    [CreatedDate],
    [CreatedBy],
    [UpdatedDate],
    [UpdatedBy]
	FROM @tblConfigurationSettingArchiveData
) AS Source ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[ConfigurationSettingGuid] = Source.[ConfigurationSettingGuid])
WHEN MATCHED AND (Target.[KeyType] <> Source.[KeyType] 
					OR Target.[SettingKey] <> Source.[SettingKey]
					OR Target.[SettingValue] <> Source.[SettingValue]) THEN
	UPDATE SET [KeyType] = Source.[KeyType]
				, [SettingKey] = Source.[SettingKey]
				, [SettingValue] = Source.[SettingValue]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[ConfigurationSettingGuid], Source.[KeyType], Source.[SettingKey], Source.[SettingValue], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[ConfigurationSettingGuid],
   inserted.[ConfigurationSettingGuid],
   deleted.[KeyType],
   inserted.[KeyType],
   deleted.[SettingKey],
   inserted.[Settingkey],
   deleted.[SettingValue],
   inserted.[SettingValue],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy],
   deleted.[_ClusterIdx],
   inserted.[_ClusterIdx]
INTO @tblConfigurationSettingArchiveRefData;

SELECT @ConfigurationSettingArchiveRefDataInserted = COUNT(*) FROM @tblConfigurationSettingArchiveRefData WHERE ActionType IN ( 'INSERT' );
SELECT @ConfigurationSettingArchiveRefDataUpdated = COUNT(*) FROM @tblConfigurationSettingArchiveRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @ConfigurationSettingArchiveRefDataDeleted = COUNT(*) FROM @tblConfigurationSettingArchiveRefData WHERE ActionType IN ( 'DELETE' )

IF (@ConfigurationSettingArchiveRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ConfigurationSettingArchiveRefDataInserted) + ' NEW ARCHIVING-SPECIFIC RECORDS INSERTED INTO [dbo].[tblConfigurationSetting] **'
	PRINT ''
END

IF (@ConfigurationSettingArchiveRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ConfigurationSettingArchiveRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [dbo].[tblConfigurationSetting] **'
	PRINT ''
	SELECT * FROM @tblConfigurationSettingArchiveRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF