SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [archive].[tblArchiveScopeToTable]'
PRINT ''

DECLARE @ArchiveScopeToTableRefDataInserted bigint
DECLARE @ArchiveScopeToTableRefDataUpdated bigint
DECLARE @ArchiveScopeToTableRefDataDeleted bigint

SET @ArchiveScopeToTableRefDataInserted = 0
SET @ArchiveScopeToTableRefDataUpdated = 0
SET @ArchiveScopeToTableRefDataDeleted = 0

DECLARE @tblArchiveScopeToTableRefData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldArchiveScopeToTableGuid] UNIQUEIDENTIFIER
	,[ArchiveScopeToTableGuid] UNIQUEIDENTIFIER
	,[OldArchiveScopeGuid] UNIQUEIDENTIFIER
	,[ArchiveScopeGuid] UNIQUEIDENTIFIER
    ,[OldSourceArchiveTable] NVARCHAR (100)
    ,[SourceArchiveTable] NVARCHAR (100)
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

DECLARE @tblArchiveScopeToTableData TABLE
(
    [ArchiveScopeToTableGuid] UNIQUEIDENTIFIER
	,[ArchiveScopeGuid] UNIQUEIDENTIFIER
	,[SourceArchiveTable] NVARCHAR (100)
    ,[CreatedDate] DATETIMEOFFSET (7)
    ,[CreatedBy] NVARCHAR (255)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedBy] NVARCHAR (255)	
);


INSERT INTO @tblArchiveScopeToTableData
(ArchiveScopeToTableGuid, ArchiveScopeGuid, SourceArchiveTable, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
VALUES
('A5739D5B-FBE1-49BD-ACA9-E9F378BD51B9', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactions]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('493C8C85-1730-4C5B-8E60-F4CD8132427F', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionLineItems]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('27070D7E-653D-4B00-A321-8EB983E7D540', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionLineItemUserData]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('18BB98AA-33D9-4D17-82FB-CEDF28648C1E', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionLinks]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('0C6981F8-1D33-4DDB-BE27-4B7B19F495AC', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionPIDX]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('C4D7A3C2-B8DF-4489-846F-C3466DF9C72F', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionNotes]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('5AF06AC3-64DF-4F12-8463-35600741F53D', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionSignature]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('20F3A9F2-BB4B-4358-92B2-2B00D677A5AC', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionSubLineItems]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('037C7BA0-33BA-422E-BAE5-25E9121B56E4', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionTransportLineItems]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('1E7430F2-6EA6-4B14-A1FC-603DFB75F2AB', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionUserData]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('DA2E01D3-90D6-4FD3-A788-5ACFB42631B9', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblTransactionWeightReadings]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')

,('FB33FFA2-D3FA-4DBF-A7B8-6393B95DB412', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblExportResults]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('95CD9B40-0F99-4701-B65D-65906892E831', '349ABE3C-4521-41FD-A4E0-1C86874137FD', N'[dbo].[tblExportResultDetails]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')

,('5CDA70CF-78C6-4759-BE2E-70EDFBEE4335', '345F252D-E5B1-4A6E-9E1A-FA74D5EA4C22', N'[dbo].[tblAlarmAndEventLog]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')
,('FCA9114E-2852-4A4A-AE6A-7B88D1406548', '75143700-5407-47C8-8C12-D17976B40F76', N'[dbo].[tblAuditLog]', N'6/15/2012 9:21:09 AM +00:00', N'Administrator', N'6/15/2012 9:21:09 AM +00:00', N'Administrator')


; MERGE INTO [archive].[tblArchiveScopeToTable] AS Target
USING 
(
	SELECT [ArchiveScopeToTableGuid], 
	[ArchiveScopeGuid],
	[SourceArchiveTable],
    [CreatedDate],
    [CreatedBy],
    [UpdatedDate],
    [UpdatedBy]
	FROM @tblArchiveScopeToTableData
) AS Source ([ArchiveScopeToTableGuid], [ArchiveScopeGuid], [SourceArchiveTable], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[ArchiveScopeToTableGuid] = Source.[ArchiveScopeToTableGuid])
WHEN MATCHED AND (Target.[ArchiveScopeGuid] <> Source.[ArchiveScopeGuid] 
					OR Target.[SourceArchiveTable] <> Source.[SourceArchiveTable]) THEN
	UPDATE SET [ArchiveScopeGuid] = Source.[ArchiveScopeGuid]
				, [SourceArchiveTable] = Source.[SourceArchiveTable]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([ArchiveScopeToTableGuid], [ArchiveScopeGuid], [SourceArchiveTable], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[ArchiveScopeToTableGuid], Source.[ArchiveScopeGuid],Source.[SourceArchiveTable],Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[ArchiveScopeToTableGuid],
   inserted.[ArchiveScopeToTableGuid],
   deleted.[ArchiveScopeGuid],
   inserted.[ArchiveScopeGuid],
   deleted.[SourceArchiveTable],
   inserted.[SourceArchiveTable],
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
INTO @tblArchiveScopetoTableRefData;

SELECT @ArchiveScopeToTableRefDataInserted = COUNT(*) FROM @tblArchiveScopeToTableRefData WHERE ActionType IN ( 'INSERT' );
SELECT @ArchiveScopeToTableRefDataUpdated = COUNT(*) FROM @tblArchiveScopeToTableRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @ArchiveScopeToTableRefDataDeleted = COUNT(*) FROM @tblArchiveScopeToTableRefData WHERE ActionType IN ( 'DELETE' )

IF (@ArchiveScopeToTableRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ArchiveScopeToTableRefDataInserted) + ' NEW RECORDS INSERTED INTO [archive].[tblArchiveScopeToTable] **'
	PRINT ''
END

IF (@ArchiveScopeToTableRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ArchiveScopeToTableRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [archive].[tblArchiveScopeToTable] **'
	PRINT ''
	SELECT * FROM @tblArchiveScopeToTableRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF