SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [lookup].[tblAccessibilities]'
PRINT ''

DECLARE @AccessibilitiesRefDataInserted bigint
DECLARE @AccessibilitiesRefDataUpdated bigint
DECLARE @AccessibilitiesRefDataDeleted bigint

SET @AccessibilitiesRefDataInserted = 0
SET @AccessibilitiesRefDataUpdated = 0
SET @AccessibilitiesRefDataDeleted = 0

DECLARE @tblAccessibilitiessRefData TABLE
(
	[ActionType] VARCHAR (50),
	[AccessibilityGuid] [uniqueidentifier],
	[OldAccessibilityGuid] [uniqueidentifier],
	[ValueType] [nvarchar](8),
	[OldValueType] [nvarchar](8),
	[ValueRange] [nvarchar](1024),
	[OldValueRange] [nvarchar](1024),
	[SettingKey] [nvarchar](50),
	[OldSettingKey] [nvarchar](50),
	[DefaultSettingValue] [nvarchar](1024),
	[OldDefaultSettingValue] [nvarchar](1024),
	[DisplayName] [nvarchar](128),
	[OldDisplayName] [nvarchar](128),
	[Description] [nvarchar](1024),
	[OldDescription] [nvarchar](1024),
	[CreatedDate] [datetimeoffset](7),
	[OldCreatedDate] [datetimeoffset](7),
	[CreatedBy] [dbo].[udtUserID],
	[OldCreatedBy] [dbo].[udtUserID],
	[UpdatedDate] [datetimeoffset](7),
	[OldUpdatedDate] [datetimeoffset](7),
	[UpdatedBy] [dbo].[udtUserID],
	[OldUpdatedBy] [dbo].[udtUserID]
);

; MERGE INTO [lookup].[tblAccessibilities] AS Target
USING (VALUES
( N'F42650EB-29D8-4BDC-AFCA-B801D310577C', 'bool',	'true;false',			'Enabled',									'false',	'Enable accessibility features'						,	'Enables accessibility feautures. If false, all accessibility features are ignored.'		,	N'05/05/2020 00:00:0 AM -04:00','administrator',N'05/05/2020 00:00:0 AM -04:00','administrator' ),
( N'042CE730-7F6A-4857-A195-BD6E2663210A', 'bool',	'true;false',			'OutlineFocusedControls',				'true',	'Enable solid outlining of focused elements'		,	'When set to true the control that has the focus is shown with a solid border.'				,	N'05/05/2020 00:00:0 AM -04:00','administrator',N'05/05/2020 00:00:0 AM -04:00','administrator' ),
( N'007696F9-50E3-4772-87AA-0326EA554399', 'bool',	'true;false',			'EnablePleaseWaitSound',				'true',	'Enable -Please Wait- audio'							,	'When set to true delayed audio is played when Please Wait message is displayed.'			,	N'05/05/2020 00:00:0 AM -04:00','administrator',N'05/05/2020 00:00:0 AM -04:00','administrator' ),
( N'8467AC88-246B-4A3A-B0FC-18A9D61BEF3B', 'bool',	'true;false',			'EnableSessionTimeoutNotification',	'true',	'Enable session time out notification'				,	'When set to true a message displayed before user session ends.'									,	N'05/05/2020 00:00:0 AM -04:00','administrator',N'05/05/2020 00:00:0 AM -04:00','administrator' ),
( N'46762289-CF34-4A77-96DD-87F3785887EB', 'int',	'2;3;4;5;6;7;8;9;10','SessionTimeoutNotificationMinute',	'5',		'Minutes before session time out notification'	,	'User is notified set amount of minutes before session times out.'								,	N'05/05/2020 00:00:0 AM -04:00','administrator',N'05/05/2020 00:00:0 AM -04:00','administrator' ),
( N'737BCD44-6288-41EF-BABE-1E27223A9227', 'bool',	'true;false',			'EnableKeyboardForMenu',				'true',	'Enable keyboard access to menus'					,	'When set to true HOME key will move keyboard focus to first (left most) root menu item .Tab and back tab keys can be used to focus move between menu items. When ENTER key is pressed while focus is on a root menu item, first sub-menu item of the root menu item will receive the keyboard focus. When ENTER key is pressed on a sub-menu, referenced page will be requested.',N'05/05/2020 00:00:0 AM -04:00','administrator',N'05/05/2020 00:00:0 AM -04:00','administrator'),
( N'C045D949-7F26-4EED-84FC-986728FB0D3E', 'int',	'2;3;4;5;6;7',			'PleaseWaitAudioDelay',					'3',		'Please Wait audio delay'								,	'When Please Wait is displayed, the audio will play after specified amount of seconds.'	,	N'05/05/2020 00:00:0 AM -04:00','administrator',N'05/05/2020 00:00:0 AM -04:00','administrator')

) AS Source (	[AccessibilityGuid],
					[ValueType],
					[ValueRange],
					[SettingKey],
					[DefaultSettingValue],
					[DisplayName],
					[Description],
					[CreatedDate],
					[CreatedBy],
					[UpdatedDate],
					[UpdatedBy])
ON (Target.[AccessibilityGuid] = Source.[AccessibilityGuid])
WHEN MATCHED AND (Target.[ValueType] <> Source.[ValueType] 
					OR Target.[ValueRange] <> Source.[ValueRange]
					OR Target.[SettingKey] <> Source.[SettingKey]
					OR Target.[DefaultSettingValue] <> Source.[DefaultSettingValue]
					OR Target.[DisplayName] <> Source.[DisplayName]
					OR Target.[Description] <> Source.[Description]) THEN
	UPDATE SET 
				[ValueType] = Source.[ValueType]
				, [ValueRange] = Source.[ValueRange]
				, [SettingKey] = Source.[SettingKey]
				, [DefaultSettingValue] = Source.[DefaultSettingValue]
				, [DisplayName] = Source.[DisplayName]
				, [Description] = Source.[Description]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT (	[AccessibilityGuid],
				[ValueType],
				[ValueRange],
				[SettingKey],
				[DefaultSettingValue],
				[DisplayName],
				[Description],
				[CreatedDate],
				[CreatedBy],
				[UpdatedDate],
				[UpdatedBy])
		VALUES (	Source.[AccessibilityGuid],
					Source.[ValueType],
					Source.[ValueRange],
					Source.[SettingKey],
					Source.[DefaultSettingValue],
					Source.[DisplayName],
					Source.[Description],
					Source.[CreatedDate],
					Source.[CreatedBy],
					Source.[UpdatedDate],
					Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[AccessibilityGuid],
   inserted.[AccessibilityGuid],
   deleted.[ValueType],
   inserted.[ValueType],
   deleted.[ValueRange],
   inserted.[ValueRange],
   deleted.[SettingKey],
   inserted.[SettingKey],
   deleted.[DefaultSettingValue],
   inserted.[DefaultSettingValue],
   deleted.[DisplayName],
   inserted.[DisplayName],
   deleted.[Description],
   inserted.[Description],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblAccessibilitiessRefData;

SELECT @AccessibilitiesRefDataInserted = COUNT(*) FROM @tblAccessibilitiessRefData WHERE ActionType IN ( 'INSERT' );
SELECT @AccessibilitiesRefDataUpdated = COUNT(*) FROM @tblAccessibilitiessRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @AccessibilitiesRefDataDeleted = COUNT(*) FROM @tblAccessibilitiessRefData WHERE ActionType IN ( 'DELETE' )

IF (@AccessibilitiesRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @AccessibilitiesRefDataInserted) + ' NEW RECORDS INSERTED INTO [lookup].[tblAccessibilities] **'
	PRINT ''
END

IF (@AccessibilitiesRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @AccessibilitiesRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [lookup].[tblAccessibilities] **'
	PRINT ''
	SELECT * FROM @tblAccessibilitiessRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
