SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [archive].[tblArchiveScope]'
PRINT ''

DECLARE @ArchiveScopeRefDataInserted bigint
DECLARE @ArchiveScopeRefDataUpdated bigint
DECLARE @ArchiveScopeRefDataDeleted bigint

SET @ArchiveScopeRefDataInserted = 0
SET @ArchiveScopeRefDataUpdated = 0
SET @ArchiveScopeRefDataDeleted = 0

DECLARE @tblArchiveScopeRefData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldArchiveScopeGuid] UNIQUEIDENTIFIER
	,[ArchiveScopeGuid] UNIQUEIDENTIFIER
    ,[OldScopeId] NVARCHAR (50)
    ,[ScopeId] NVARCHAR (50)
    ,[OldIsArchivingOn] BIT
    ,[IsArchivingOn] BIT
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

DECLARE @tblArchiveScopeData TABLE
(
    [ArchiveScopeGuid] UNIQUEIDENTIFIER
	,[ScopeId] NVARCHAR (50)
	,[IsArchivingOn] BIT
    ,[CreatedDate] DATETIMEOFFSET (7)
    ,[CreatedBy] NVARCHAR (255)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedBy] NVARCHAR (255)	
);


INSERT INTO @tblArchiveScopeData
(ArchiveScopeGuid, ScopeId, IsArchivingOn, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
VALUES
('349ABE3C-4521-41FD-A4E0-1C86874137FD', N'Transaction Tables', 1,  N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('345F252D-E5B1-4A6E-9E1A-FA74D5EA4C22', N'AlarmAndEvent Log', 1,  N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('75143700-5407-47C8-8C12-D17976B40F76', N'Audit Log', 1,  N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')


; MERGE INTO [archive].[tblArchiveScope] AS Target
USING 
(
	SELECT [ArchiveScopeGuid],
	[ScopeId],
	[IsArchivingOn],
    [CreatedDate],
    [CreatedBy],
    [UpdatedDate],
    [UpdatedBy]
	FROM @tblArchiveScopeData
) AS Source ([ArchiveScopeGuid], [ScopeId], [IsArchivingOn], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[ArchiveScopeGuid] = Source.[ArchiveScopeGuid])
WHEN MATCHED AND (Target.[ScopeId] <> Source.[ScopeId] 
					OR Target.[IsArchivingOn] <> Source.[IsArchivingOn]) THEN
	UPDATE SET [ScopeId] = Source.[ScopeId]
				, [IsArchivingOn] = Source.[IsArchivingOn]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([ArchiveScopeGuid], [ScopeId], [IsArchivingOn], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[ArchiveScopeGuid],Source.[ScopeId],Source.[IsArchivingOn],Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[ArchiveScopeGuid],
   inserted.[ArchiveScopeGuid],
   deleted.[ScopeId],
   inserted.[ScopeId],
   deleted.[IsArchivingOn],
   inserted.[IsArchivingOn],  
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
INTO @tblArchiveScopeRefData;

SELECT @ArchiveScopeRefDataInserted = COUNT(*) FROM @tblArchiveScopeRefData WHERE ActionType IN ( 'INSERT' );
SELECT @ArchiveScopeRefDataUpdated = COUNT(*) FROM @tblArchiveScopeRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @ArchiveScopeRefDataDeleted = COUNT(*) FROM @tblArchiveScopeRefData WHERE ActionType IN ( 'DELETE' )

IF (@ArchiveScopeRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ArchiveScopeRefDataInserted) + ' NEW RECORDS INSERTED INTO [archive].[tblArchiveScope] **'
	PRINT ''
END

IF (@ArchiveScopeRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ArchiveScopeRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [archive].[tblArchiveScope] **'
	PRINT ''
	SELECT * FROM @tblArchiveScopeRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF