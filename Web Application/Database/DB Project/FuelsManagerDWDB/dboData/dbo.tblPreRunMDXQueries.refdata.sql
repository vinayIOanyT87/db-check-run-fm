--[dbo].[tblPreRunMDXQueries]
SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [dbo].[tblPreRunMDXQueries]'
PRINT ''

DECLARE @PreRunMDXQueriesDataInserted bigint
DECLARE @PreRunMDXQueriesDataUpdated bigint
DECLARE @PreRunMDXQueriesDataDeleted bigint

SET @PreRunMDXQueriesDataInserted = 0
SET @PreRunMDXQueriesDataUpdated = 0
SET @PreRunMDXQueriesDataDeleted = 0

DECLARE @tblPreRunMDXQueriesData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldSKey] INT
    ,[SKey] INT
    ,[OldQueryDefinition] VARCHAR (MAX)
    ,[QueryDefinition] VARCHAR (MAX)    
	,[OldQueryDescription] VARCHAR (250)
    ,[QueryDescription] VARCHAR (250)
	,[OldPriorityLevel] INT
    ,[PriorityLevel] INT
	,[OldCreatedDate] DATETIMEOFFSET (7)
	,[CreatedDate] DATETIMEOFFSET (7)
    ,[OldCreatedBy] NVARCHAR (100)
    ,[CreatedBy] NVARCHAR (100)
    ,[OldUpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[OldUpdatedBy] NVARCHAR (100)
    ,[UpdatedBy] NVARCHAR (100)
);

DECLARE @tblPreRunMDXQueries TABLE
(
    [SKey] INT
    ,[QueryDefinition] VARCHAR (MAX)    
    ,[QueryDescription] VARCHAR (250)
    ,[PriorityLevel] INT
	,[CreatedDate] DATETIMEOFFSET (7)
    ,[CreatedBy] NVARCHAR (100)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedBy] NVARCHAR (100)
);

INSERT INTO @tblPreRunMDXQueries
(SKey, QueryDescription, QueryDefinition, PriorityLevel, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
VALUES 
(1, 'General Query', 'SELECT [Measures].[GrossQuantity] ON COLUMNS, NONEmpty([Inventory Date].[Date Hierarchy].[Calendar Year]) ON ROWS FROM [Fuels Manager DW]', 1, N'3/2/2020 9:21:09 AM +00:00', N'Administrator', N'3/2/2020 9:21:09 AM +00:00', N'Administrator')
,(2, 'General Query', 'SELECT NON EMPTY Hierarchize({DrilldownLevel({[Product].[Product Id].[All]},,,INCLUDE_CALC_MEMBERS)}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME,HIERARCHY_UNIQUE_NAME ON COLUMNS , NON EMPTY Hierarchize({DrilldownLevel({[Inventory Date].[Date Hierarchy].[All]},,,INCLUDE_CALC_MEMBERS)}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME,HIERARCHY_UNIQUE_NAME ON ROWS  FROM [Fuels Manager DW] WHERE ([Site].[Site Id].[All],[Measures].[GrossQuantity]) CELL PROPERTIES VALUE, FORMAT_STRING, LANGUAGE, BACK_COLOR, FORE_COLOR, FONT_FLAGS', 1, N'3/2/2020 9:21:09 AM +00:00', N'Administrator', N'3/2/2020 9:21:09 AM +00:00', N'Administrator')
,(3, 'Ledger Query', 'SELECT {[Measures].[GrossBeginBookInventory],[Measures].[ReceiptGrossQuantity],[Measures].[IssueGrossQuantity],[Measures].[DefuelGrossQuantity],[Measures].[GrossUnadjBookQuantity],[Measures].[GrossUnadjBookInventory],[Measures].[SiteGrossPhysicalInventory],[Measures].[GrossBookInventoryVariance],[Measures].[AdjustmentGrossQuantity],[Measures].[GrossBookInventoryToDate]} DIMENSION PROPERTIES PARENT_UNIQUE_NAME,HIERARCHY_UNIQUE_NAME ON COLUMNS , NON EMPTY Hierarchize(DrilldownMember({{DrilldownLevel({[Inventory Date].[Date Hierarchy].[All]},,,INCLUDE_CALC_MEMBERS)}}, {[Inventory Date].[Date Hierarchy].[Calendar Year].&[2020]},,,INCLUDE_CALC_MEMBERS)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME,HIERARCHY_UNIQUE_NAME,[Inventory Date].[Date Hierarchy].[Calendar Quarter].[Calendar Year] ON ROWS  FROM [Fuels Manager DW] WHERE ([Site].[Site Id].&[Baltimore],[Product].[Product Id].[All]) CELL PROPERTIES VALUE, FORMAT_STRING, LANGUAGE, BACK_COLOR, FORE_COLOR, FONT_FLAGS', 1, N'3/2/2020 9:21:09 AM +00:00', N'Administrator', N'3/2/2020 9:21:09 AM +00:00', N'Administrator')
MERGE INTO [dbo].[tblPreRunMDXQueries] AS Target
USING 
(
	SELECT [SKey]
	,[QueryDefinition]
    ,[QueryDescription]
    ,[PriorityLevel]
	,[CreatedDate]
    ,[CreatedBy]
    ,[UpdatedDate]
    ,[UpdatedBy]
	FROM @tblPreRunMDXQueries
) AS Source ([SKey], [QueryDefinition], [QueryDescription], [PriorityLevel], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[SKey] = Source.[SKey])
WHEN MATCHED AND (Target.[QueryDefinition] <> Source.[QueryDefinition]
				OR ISNULL(Target.[QueryDescription], '') <> ISNULL(Source.[QueryDescription], '')
				OR ISNULL(Target.[PriorityLevel], 0) <> ISNULL(Source.[PriorityLevel], 0)) THEN
	UPDATE SET [QueryDefinition] = Source.[QueryDefinition]
				, [QueryDescription] = Source.[QueryDescription]
				, [PriorityLevel] = Source.[PriorityLevel]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([SKey], [QueryDefinition], [QueryDescription], [PriorityLevel], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[SKey],Source.[QueryDefinition], Source.[QueryDescription], Source.[PriorityLevel], Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[SKey],
   inserted.[SKey],
   deleted.[QueryDefinition],
   inserted.[QueryDefinition],
   deleted.[QueryDescription],
   inserted.[QueryDescription],
   deleted.[PriorityLevel],
   inserted.[PriorityLevel],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblPreRunMDXQueriesData;

SELECT @PreRunMDXQueriesDataInserted = COUNT(*) FROM @tblPreRunMDXQueriesData WHERE ActionType IN ( 'INSERT' );
SELECT @PreRunMDXQueriesDataUpdated = COUNT(*) FROM @tblPreRunMDXQueriesData WHERE ActionType IN ( 'UPDATE' )
SELECT @PreRunMDXQueriesDataDeleted = COUNT(*) FROM @tblPreRunMDXQueriesData WHERE ActionType IN ( 'DELETE' )

IF (@PreRunMDXQueriesDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @PreRunMDXQueriesDataInserted) + ' NEW RECORDS INSERTED INTO [staging].[tblPreRunMDXQueries] **'
	PRINT ''
END

IF (@PreRunMDXQueriesDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @PreRunMDXQueriesDataUpdated) + ' EXISTING RECORDS UPDATED IN [staging].[tblPreRunMDXQueries] **'
	PRINT ''
	SELECT * FROM @tblPreRunMDXQueriesData WHERE ActionType IN ( 'UPDATE' );
END