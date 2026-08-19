--[dbo].[tblSystemSettings]
SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [dbo].[tblSystemSettings]'
PRINT ''

DECLARE @SystemSettingsDataInserted bigint
DECLARE @SystemSettingsDataUpdated bigint
DECLARE @SystemSettingsDataDeleted bigint

SET @SystemSettingsDataInserted = 0
SET @SystemSettingsDataUpdated = 0
SET @SystemSettingsDataDeleted = 0

DECLARE @tblSystemSettingsData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldSKey] INT
    ,[SKey] INT
	,[OldSettingKey] [nvarchar](50)
	,[SettingKey] [nvarchar](50)
	,[OldSettingValue] [nvarchar](2000)
	,[SettingValue] [nvarchar](2000)
);

DECLARE @tblSystemSettings TABLE
(
    [SKey] INT
    ,[SettingKey] [nvarchar](50)
	,[SettingValue] [nvarchar](2000)
);

INSERT INTO @tblSystemSettings
(SKey, [SettingKey], [SettingValue])
VALUES 
(1,  N'DatabaseVersion', N'1.00'),
(2, N'PartitionSchemaTemplate', N'<Batch xmlns="http://schemas.microsoft.com/analysisservices/2003/engine">
		<Parallel>
			<Process xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:ddl2="http://schemas.microsoft.com/analysisservices/2003/engine/2" xmlns:ddl2_2="http://schemas.microsoft.com/analysisservices/2003/engine/2/2" xmlns:ddl100_100="http://schemas.microsoft.com/analysisservices/2008/engine/100/100" xmlns:ddl200="http://schemas.microsoft.com/analysisservices/2010/engine/200" xmlns:ddl200_200="http://schemas.microsoft.com/analysisservices/2010/engine/200/200" xmlns:ddl300="http://schemas.microsoft.com/analysisservices/2011/engine/300" xmlns:ddl300_300="http://schemas.microsoft.com/analysisservices/2011/engine/300/300">
				<Object>
					<DatabaseID>PUTDATABASEIDHERE</DatabaseID>
					<CubeID>PUTCUBEIDHERE</CubeID>
					<MeasureGroupID>PUTMEASUREGROUPIDHERE</MeasureGroupID>
					<PartitionID>PUTPARTITIONIDHERE</PartitionID>
				</Object>
				<Type>ProcessFull</Type>
				<WriteBackTableCreation>UseExisting</WriteBackTableCreation>
			</Process>
		</Parallel>
	</Batch>')

MERGE INTO [dbo].[tblSystemSettings] AS Target
USING 
(
	SELECT [SKey]
	, [SettingKey]
	, [SettingValue]
	FROM @tblSystemSettings
) AS Source ([SKey], [SettingKey], [SettingValue])
ON (Target.[SKey] = Source.[SKey])
WHEN MATCHED AND (ISNULL(Target.[SettingValue], '') <> ISNULL(Source.[SettingValue], '')) THEN
	UPDATE SET [SettingValue] = Source.[SettingValue]
WHEN NOT MATCHED THEN
	INSERT ([SKey], [SettingKey], [SettingValue])
		VALUES (Source.[SKey], Source.[SettingKey], source.[SettingValue])
OUTPUT
	$action AS ActionType,
	deleted.[SKey],
	inserted.[SKey],
	deleted.[SettingKey],
	inserted.[SettingKey],
	deleted.[SettingValue],
	inserted.[SettingValue]
INTO @tblSystemSettingsData;

SELECT @SystemSettingsDataInserted = COUNT(*) FROM @tblSystemSettingsData WHERE ActionType IN ( 'INSERT' );
SELECT @SystemSettingsDataUpdated = COUNT(*) FROM @tblSystemSettingsData WHERE ActionType IN ( 'UPDATE' )
SELECT @SystemSettingsDataDeleted = COUNT(*) FROM @tblSystemSettingsData WHERE ActionType IN ( 'DELETE' )

IF (@SystemSettingsDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SystemSettingsDataInserted) + ' NEW RECORDS INSERTED INTO [dbo].[tblSystemSettings] **'
	PRINT ''
END

IF (@SystemSettingsDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SystemSettingsDataUpdated) + ' EXISTING RECORDS UPDATED IN [dbo].[tblSystemSettings] **'
	PRINT ''
	SELECT * FROM @tblSystemSettingsData WHERE ActionType IN ( 'UPDATE' );
END