SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [sync].[tblSyncDependencyGroup]'
PRINT ''

DECLARE @SyncDependencyGroupInserted bigint
DECLARE @SyncDependencyGroupUpdated bigint
DECLARE @SyncDependencyGroupDeleted bigint

SET @SyncDependencyGroupInserted = 0
SET @SyncDependencyGroupUpdated = 0
SET @SyncDependencyGroupDeleted = 0

DECLARE @tblSyncDependencyGroupRefData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldID] NVARCHAR(80)
	,[ID] NVARCHAR(80)
	,[OldFriendlyName] NVARCHAR(100)
	,[FriendlyName] NVARCHAR(100)
	,[OldLongDescription] NVARCHAR(1024)
	,[LongDescription] NVARCHAR(1024)
	,[OldDependencyLevel] INT
	,[DependencyLevel] INT
	,[OldCreatedDate] DATETIMEOFFSET(7)
	,[CreatedDate] DATETIMEOFFSET(7)
	,[OldCreatedBy] [dbo].[udtUserID]
	,[CreatedBy] [dbo].[udtUserID]
	,[OldUpdatedDate] DATETIMEOFFSET(7)
	,[UpdatedDate] DATETIMEOFFSET(7)
	,[OldUpdatedBy] [dbo].[udtUserID]
	,[UpdatedBy] [dbo].[udtUserID]
);


; MERGE INTO [sync].[tblSyncDependencyGroup] AS Target
USING (VALUES
	-- Create List of SyncDependencyGroups
	(N'fcd07cf0-1692-4831-965f-1fd5d1dd421c', N'Level1', N'Lookup Data', N'Any system wide application reference or lookup tables that do not have any dependencies (foreign key relationships) on another table.', 1, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'c20b181d-d960-4206-9bed-3d8fa1a0aefa', N'Level2', N'Site List and Filtered Lookup Data', N'Site List and Lookup tables that are filtered/partitioned based on values in the AppCore Lookup Data.  Tables in this group must be able to have ALL foreign key relationships satisified by the AppCore reference tables.', 2, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'2ad7a4b9-68b1-45d8-aeaf-00938e96f22d', N'Level3', N'Site Mappings, Site Data, Internal Entity Mappings ', N'Site Mappings / Site Configuration Data / Data Dictionary To Site, Alarm and Event To Site, etc.. Maps', 3, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'64b99efe-9b71-4e01-ac04-1ded944ffa2b', N'Level4', N'Group Maps, Entity to Site Maps, User Data Config', N'Group Mappings / Entity Groups to Site Maps / Entity To Site Maps for Entities directly referenced by the Site.', 4, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'69eb8da5-c8af-42fa-be84-0e8bfa74104f', N'Level5', N'Level 5', N'Level 5', 5, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'20b7e0f1-3038-4f47-ac42-f448169cf303', N'Level6', N'Level 6', N'Level 6', 6, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'9e6ada63-3fc7-48a3-84a1-c2fef42b5d27', N'Level7', N'Level 7', N'Level 7', 7, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'ac471a77-0557-4f7f-9a83-1a0cec8aa696', N'Level8', N'Level 8', N'Level 8', 8, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'e50c8831-c1cd-4284-8737-a0c100d0a539', N'Level9', N'Level 9', N'Level 9', 9, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
	,(N'9b8fcb95-2913-4ffb-9539-4a55bb06d732', N'Level10', N'Level 10', N'Level 10', 10, N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator', N'2012-11-09 16:35:02.5620169 -05:00', N'Administrator')
) AS SOURCE ([SyncDependencyGroupGuid], [ID], [FriendlyName], [LongDescription], [DependencyLevel], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (TARGET.[SyncDependencyGroupGuid] = SOURCE.[SyncDependencyGroupGuid])
WHEN MATCHED AND EXISTS (SELECT Target.[ID]
						, Target.[FriendlyName]
						, Target.[LongDescription]
						, Target.[DependencyLevel]
						, Target.[CreatedDate]
						, Target.[CreatedBy]
						, Target.[UpdatedDate]
						, Target.[UpdatedBy] 
						EXCEPT 
						SELECT Source.[ID]
						, Source.[FriendlyName]
						, Source.[LongDescription]
						, Source.[DependencyLevel]
						, Source.[CreatedDate]
						, Source.[CreatedBy]
						, Source.[UpdatedDate]
						, Source.[UpdatedBy] ) THEN
	UPDATE SET [ID] = Source.[ID]
				, [FriendlyName] = Source.[FriendlyName]
				, [LongDescription] = Source.[LongDescription]
				, [DependencyLevel] = Source.[DependencyLevel]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([SyncDependencyGroupGuid], [ID], [FriendlyName], [LongDescription], [DependencyLevel], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[SyncDependencyGroupGuid], Source.[ID], Source.[FriendlyName], Source.[LongDescription], Source.[DependencyLevel], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[ID],
   inserted.[ID],
   deleted.[FriendlyName],
   inserted.[FriendlyName],
   deleted.[LongDescription],
   inserted.[LongDescription],
   deleted.[DependencyLevel],
   inserted.[DependencyLevel],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblSyncDependencyGroupRefData;

SELECT @SyncDependencyGroupInserted = COUNT(*) FROM @tblSyncDependencyGroupRefData WHERE ActionType IN ( 'INSERT' );
SELECT @SyncDependencyGroupUpdated = COUNT(*) FROM @tblSyncDependencyGroupRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @SyncDependencyGroupDeleted = COUNT(*) FROM @tblSyncDependencyGroupRefData WHERE ActionType IN ( 'DELETE' )

IF (@SyncDependencyGroupInserted = 0 AND @SyncDependencyGroupUpdated = 0)
BEGIN
	PRINT '** No Changes Detected for [sync].[tblSyncDependencyGroup] **'
	PRINT ''
END

IF (@SyncDependencyGroupInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SyncDependencyGroupInserted) + ' NEW RECORDS INSERTED INTO [sync].[tblSyncDependencyGroup] **'
	PRINT ''
END

IF (@SyncDependencyGroupUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SyncDependencyGroupUpdated) + ' EXISTING RECORDS UPDATED IN [sync].[tblSyncDependencyGroup] **'
	PRINT ''
	SELECT * FROM @tblSyncDependencyGroupRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
