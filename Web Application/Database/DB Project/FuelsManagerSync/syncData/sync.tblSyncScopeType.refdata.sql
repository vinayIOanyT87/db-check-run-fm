SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [sync].[tblSyncScopeType]'
PRINT ''

DECLARE @SyncScopeTypeInserted BIGINT
DECLARE @SyncScopeTypeUpdated BIGINT
DECLARE @SyncScopeTypeDeleted BIGINT

SET @SyncScopeTypeInserted = 0
SET @SyncScopeTypeUpdated = 0
SET @SyncScopeTypeDeleted = 0

DECLARE @tblSyncScopeTypeRefData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldID] NVARCHAR(80)
	,[ID] NVARCHAR(80)
	,[OldFriendlyName] NVARCHAR(100)
	,[FriendlyName] NVARCHAR(100)
	,[OldLongDescription] NVARCHAR(1024)
	,[LongDescription] NVARCHAR(1024)
	,[OldCreatedDate] DATETIMEOFFSET(7)
	,[CreatedDate] DATETIMEOFFSET(7)
	,[OldCreatedBy] [dbo].[udtUserID]
	,[CreatedBy] [dbo].[udtUserID]
	,[OldUpdatedDate] DATETIMEOFFSET(7)
	,[UpdatedDate] DATETIMEOFFSET(7)
	,[OldUpdatedBy] [dbo].[udtUserID]
	,[UpdatedBy] [dbo].[udtUserID]
);

; MERGE INTO [sync].[tblSyncScopeType] AS Target
USING (VALUES
	-- Create List of Sync Scope Types
	(1, N'Global', N'One Time', N'Scope is called before or after (sync order) Reference and Hosted Site Scopes are processed.', '2012-11-09 16:35:02.5620169 -05:00', N'dbo', '2012-11-09 16:35:02.5620169 -05:00', N'dbo')
	,(2, N'ReferenceOnly', N'Reference Sites', N'Scope used only for Reference Sites', '2012-11-09 16:35:02.5410148 -05:00', N'dbo', '2012-11-09 16:35:02.5410148 -05:00', N'dbo')
	,(3, N'HostedOnly', N'Hosted Sites', N'Scope used only for Hosted Sites', '2012-11-09 16:35:02.4640071 -05:00', N'dbo', '2012-11-09 16:35:02.4640071 -05:00', N'dbo')
	,(4, N'ReferenceAndHosted', N'BOTH', N'Applies to Reference and Hosted Sites', '2012-11-09 16:35:02.3389946 -05:00', N'dbo', '2012-11-09 16:35:02.3389946 -05:00', N'dbo')
) AS Source ([SyncScopeTypeIndex], [ID], [FriendlyName], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[SyncScopeTypeIndex] = Source.[SyncScopeTypeIndex])
WHEN MATCHED AND EXISTS (SELECT Target.[ID]
						, Target.[FriendlyName]
						, Target.[LongDescription]
						, Target.[CreatedDate]
						, Target.[CreatedBy]
						, Target.[UpdatedDate]
						, Target.[UpdatedBy] 
						EXCEPT 
						SELECT Source.[ID]
						, Source.[FriendlyName]
						, Source.[LongDescription]
						, Source.[CreatedDate]
						, Source.[CreatedBy]
						, Source.[UpdatedDate]
						, Source.[UpdatedBy] ) THEN
	UPDATE SET [ID] = Source.[ID]
				, [FriendlyName] = Source.[FriendlyName]
				, [LongDescription] = Source.[LongDescription]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([SyncScopeTypeIndex], [ID], [FriendlyName], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[SyncScopeTypeIndex], Source.[ID], Source.[FriendlyName], Source.[LongDescription], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[ID],
   inserted.[ID],
   deleted.[FriendlyName],
   inserted.[FriendlyName],
   deleted.[LongDescription],
   inserted.[LongDescription],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblSyncScopeTypeRefData;

SELECT @SyncScopeTypeInserted = COUNT(*) FROM @tblSyncScopeTypeRefData WHERE ActionType IN ( 'INSERT' );
SELECT @SyncScopeTypeUpdated = COUNT(*) FROM @tblSyncScopeTypeRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @SyncScopeTypeDeleted = COUNT(*) FROM @tblSyncScopeTypeRefData WHERE ActionType IN ( 'DELETE' )

IF (@SyncScopeTypeInserted = 0 AND @SyncScopeTypeUpdated = 0)
BEGIN
	PRINT '** No Changes Detected for [sync].[tblSyncScopeType] **'
	PRINT ''
END

IF (@SyncScopeTypeInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SyncScopeTypeInserted) + ' NEW RECORDS INSERTED INTO [sync].[tblSyncScopeType] **'
	PRINT ''
END

IF (@SyncScopeTypeUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SyncScopeTypeUpdated) + ' EXISTING RECORDS UPDATED IN [sync].[tblSyncScopeType] **'
	PRINT ''
	SELECT * FROM @tblSyncScopeTypeRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
