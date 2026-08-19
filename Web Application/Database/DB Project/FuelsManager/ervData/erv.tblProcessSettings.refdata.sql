SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [erv].[tblProcessSettings]'
PRINT ''

DECLARE @ProcessSettingsInserted bigint
DECLARE @ProcessSettingsUpdated bigint
DECLARE @ProcessSettingsDeleted bigint

SET @ProcessSettingsInserted = 0
SET @ProcessSettingsUpdated = 0
SET @ProcessSettingsDeleted = 0


DECLARE @tblProcessSettingsRefData TABLE
(
	[ActionType] VARCHAR (50),
	[OldInhibitGlobalFieldsProcessing] BIT,
	[InhibitGlobalFieldsProcessing] BIT,
	[OldCreatedDate] DATETIMEOFFSET(7),
	[CreatedDate] DATETIMEOFFSET(7),
	[OldCreatedBy] [dbo].[udtUserID],
	[CreatedBy] [dbo].[udtUserID],
	[OldUpdatedDate] DATETIMEOFFSET(7),
	[UpdatedDate] DATETIMEOFFSET(7),
	[OldUpdatedBy] [dbo].[udtUserID],
	[UpdatedBy] [dbo].[udtUserID]
);

MERGE INTO [erv].[tblProcessSettings] AS Target
USING (VALUES
	(1, 0, N'08/21/2025 3:59:56 PM -04:00', N'Administrator', N'08/21/2025 3:59:56 PM -04:00', N'Administrator')
 ) AS Source ([ProcessSettingsKey], [InhibitGlobalFieldsProcessing], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[ProcessSettingsKey] = Source.[ProcessSettingsKey])
WHEN MATCHED AND EXISTS (SELECT Target.[InhibitGlobalFieldsProcessing]
						, Target.[CreatedDate]
						, Target.[CreatedBy]
						, Target.[UpdatedDate]
						, Target.[UpdatedBy] 
						EXCEPT 
						SELECT Source.[InhibitGlobalFieldsProcessing]
						, Source.[CreatedDate]
						, Source.[CreatedBy]
						, Source.[UpdatedDate]
						, Source.[UpdatedBy]) THEN
	UPDATE SET [InhibitGlobalFieldsProcessing] = Source.[InhibitGlobalFieldsProcessing]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([ProcessSettingsKey], [InhibitGlobalFieldsProcessing], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[ProcessSettingsKey], Source.[InhibitGlobalFieldsProcessing], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[InhibitGlobalFieldsProcessing],
   inserted.[InhibitGlobalFieldsProcessing],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblProcessSettingsRefData;

SELECT @ProcessSettingsInserted = COUNT(*) FROM @tblProcessSettingsRefData WHERE ActionType IN ( 'INSERT' );
SELECT @ProcessSettingsUpdated = COUNT(*) FROM @tblProcessSettingsRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @ProcessSettingsDeleted = COUNT(*) FROM @tblProcessSettingsRefData WHERE ActionType IN ( 'DELETE' )

IF (@ProcessSettingsInserted = 0 AND @ProcessSettingsUpdated = 0)
BEGIN
	PRINT '** No Changes Detected for [erv].[tblProcessSettings] **'
	PRINT ''
END

IF (@ProcessSettingsInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ProcessSettingsInserted) + ' NEW RECORDS INSERTED INTO [erv].[tblProcessSettings] **'
	PRINT ''
END

IF (@ProcessSettingsUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ProcessSettingsUpdated) + ' EXISTING RECORDS UPDATED IN [erv].[tblProcessSettings] **'
	PRINT ''
	SELECT * FROM @tblProcessSettingsRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
