SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [sync].[tblSyncProfile]'
PRINT ''

DECLARE @SyncProfileInserted BIGINT
DECLARE @SyncProfileUpdated BIGINT
DECLARE @SyncProfileDeleted BIGINT

SET @SyncProfileInserted = 0
SET @SyncProfileUpdated = 0
SET @SyncProfileDeleted = 0

DECLARE @tblSyncProfileRefData TABLE
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


; MERGE INTO [sync].[tblSyncProfile] AS Target
USING (VALUES
	-- Create Default "Complete" Synchronization Profile
	(N'83912bbd-113c-4824-9406-6dc3fed36590', N'{Complete}', N'Complete Database Synchronization', N'Default Profile configured to synchronize all tables from the Enterprise Node to a remote node', '2012-11-15 08:25:06.3356616 -05:00', NULL, '2012-11-15 08:25:06.3356616 -05:00', NULL)
) AS Source ([SyncProfileGuid], [ID], [FriendlyName], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[SyncProfileGuid] = Source.[SyncProfileGuid])
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
	INSERT ([SyncProfileGuid], [ID], [FriendlyName], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[SyncProfileGuid], Source.[ID], Source.[FriendlyName], Source.[LongDescription], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy])
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
INTO @tblSyncProfileRefData;

SELECT @SyncProfileInserted = COUNT(*) FROM @tblSyncProfileRefData WHERE ActionType IN ( 'INSERT' );
SELECT @SyncProfileUpdated = COUNT(*) FROM @tblSyncProfileRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @SyncProfileDeleted = COUNT(*) FROM @tblSyncProfileRefData WHERE ActionType IN ( 'DELETE' )

IF (@SyncProfileInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SyncProfileInserted) + ' NEW RECORDS INSERTED INTO [sync].[tblSyncProfile] **'
	PRINT ''
END

IF (@SyncProfileUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SyncProfileUpdated) + ' EXISTING RECORDS UPDATED IN [sync].[tblSyncProfile] **'
	PRINT ''
	SELECT * FROM @tblSyncProfileRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
