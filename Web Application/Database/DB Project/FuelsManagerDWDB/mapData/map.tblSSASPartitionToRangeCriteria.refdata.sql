--[map].[tblSSASPartitionToRangeCriteria]
SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [map].[tblSSASPartitionToRangeCriteria]'
PRINT ''

DECLARE @SSASPartitionToRangeCriteriaDataInserted bigint
DECLARE @SSASPartitionToRangeCriteriaDataUpdated bigint
DECLARE @SSASPartitionToRangeCriteriaDataDeleted bigint

SET @SSASPartitionToRangeCriteriaDataInserted = 0
SET @SSASPartitionToRangeCriteriaDataUpdated = 0
SET @SSASPartitionToRangeCriteriaDataDeleted = 0

DECLARE @tblSSASPartitionToRangeCriteriaData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldSKey] INT
    ,[SKey] INT
	, [OldDatabaseID] [varchar](50) NULL
	, [DatabaseID] [varchar](50) NULL
	, [OldCubeID] [varchar](50) NULL
	, [CubeID] [varchar](50) NULL
	, [OldMeasureGroupID] [varchar](50) NULL
	, [MeasureGroupID] [varchar](50) NULL
	, [OldPartitionID] [varchar](50) NULL
	, [PartitionID] [varchar](50) NULL
	, [OldPartitionName] [varchar](50) NULL
	, [PartitionName] [varchar](50) NULL
	, [OldLowerRange] [int] NULL
	, [LowerRange] [int] NULL
	, [OldUpperRange] [int] NULL
	, [UpperRange] [int] NULL
);

DECLARE @tblSSASPartitionToRangeCriteria TABLE
(
    [SKey] INT
    , [DatabaseID] [varchar](50) NULL
	, [CubeID] [varchar](50) NULL
	, [MeasureGroupID] [varchar](50) NULL
	, [PartitionID] [varchar](50) NULL
	, [PartitionName] [varchar](50) NULL
	, [LowerRange] [int] NULL
	, [UpperRange] [int] NULL
);

/*
INSERT INTO @tblSSASPartitionToRangeCriteria
(SKey, [DatabaseID], [CubeID], [MeasureGroupID], [PartitionID], [PartitionName], [LowerRange], [UpperRange])
VALUES 
*/

MERGE INTO [map].[tblSSASPartitionToRangeCriteria] AS Target
USING 
(
	SELECT [SKey], [DatabaseID], [CubeID], [MeasureGroupID], [PartitionID], [PartitionName], [LowerRange], [UpperRange]
	FROM @tblSSASPartitionToRangeCriteria
) AS Source ([SKey], [DatabaseID], [CubeID], [MeasureGroupID], [PartitionID], [PartitionName], [LowerRange], [UpperRange])
ON (Target.[SKey] = Source.[SKey])
WHEN MATCHED 
	AND (ISNULL(Target.[LowerRange], 0) <> ISNULL(Source.[LowerRange], 0)) 
	OR (ISNULL(Target.[UpperRange], 0) <> ISNULL(Source.[UpperRange], 0)) THEN
	UPDATE SET [LowerRange] = Source.[LowerRange], [UpperRange] = Source.[UpperRange]
WHEN NOT MATCHED THEN
	INSERT ([SKey], [DatabaseID], [CubeID], [MeasureGroupID], [PartitionID], [PartitionName], [LowerRange], [UpperRange])
		VALUES (Source.[SKey], Source.[DatabaseID], Source.[CubeID], Source.[MeasureGroupID], Source.[PartitionID], Source.[PartitionName], Source.[LowerRange], Source.[UpperRange])
OUTPUT
	$action AS ActionType,
	deleted.[SKey],
	inserted.[SKey],
	deleted.[DatabaseID],
	inserted.[DatabaseID],
	deleted.[CubeID],
	inserted.[CubeID],
	deleted.[MeasureGroupID],
	inserted.[MeasureGroupID],
	deleted.[PartitionID],
	inserted.[PartitionID],
	deleted.[PartitionName],
	inserted.[PartitionName],
	deleted.[LowerRange],
	inserted.[LowerRange],
	deleted.[UpperRange],
	inserted.[UpperRange]
INTO @tblSSASPartitionToRangeCriteriaData;


SELECT @SSASPartitionToRangeCriteriaDataInserted = COUNT(*) FROM @tblSSASPartitionToRangeCriteriaData WHERE ActionType IN ( 'INSERT' );
SELECT @SSASPartitionToRangeCriteriaDataUpdated = COUNT(*) FROM @tblSSASPartitionToRangeCriteriaData WHERE ActionType IN ( 'UPDATE' )
SELECT @SSASPartitionToRangeCriteriaDataDeleted = COUNT(*) FROM @tblSSASPartitionToRangeCriteriaData WHERE ActionType IN ( 'DELETE' )

IF (@SSASPartitionToRangeCriteriaDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SSASPartitionToRangeCriteriaDataInserted) + ' NEW RECORDS INSERTED INTO [map].[tblSSASPartitionToRangeCriteria] **'
	PRINT ''
END

IF (@SSASPartitionToRangeCriteriaDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @SSASPartitionToRangeCriteriaDataUpdated) + ' EXISTING RECORDS UPDATED IN [map].[tblSSASPartitionToRangeCriteria] **'
	PRINT ''
	SELECT * FROM @tblSSASPartitionToRangeCriteriaData WHERE ActionType IN ( 'UPDATE' );
END