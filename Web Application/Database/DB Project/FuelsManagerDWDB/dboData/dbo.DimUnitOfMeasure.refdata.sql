--[dbo].[DimUnitOfMeasure]
SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [dbo].[DimUnitOfMeasure]'
PRINT ''

DECLARE @DimUnitOfMeasureDataInserted bigint
DECLARE @DimUnitOfMeasureDataUpdated bigint
DECLARE @DimUnitOfMeasureDataDeleted bigint

SET @DimUnitOfMeasureDataInserted = 0
SET @DimUnitOfMeasureDataUpdated = 0
SET @DimUnitOfMeasureDataDeleted = 0

DECLARE @DimUnitOfMeasureData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldSKey] INT
    ,[SKey] INT
    ,[OldUnitOfMeasureCode] [nvarchar](100)
	,[UnitOfMeasureCode] [nvarchar](100)
	,[OldUnitOfMeasureName] [nvarchar](100)
	,[UnitOfMeasureName] [nvarchar](100)
	,[OldDescription] [nvarchar](250)
	,[Description] [nvarchar](250)
	,[OldVolumeSIToUnitConvFactor] [float] NULL
	,[VolumeSIToUnitConvFactor] [float] NULL
	,[OldMassSIToUnitConvFactor] [float] NULL
	,[MassSIToUnitConvFactor] [float] NULL
);

DECLARE @DimUnitOfMeasure TABLE
(
    [SKey] INT
    ,[UnitOfMeasureCode] [nvarchar](100)
	,[UnitOfMeasureName] [nvarchar](100)
	,[Description] [nvarchar](250)
	,[VolumeSIToUnitConvFactor] [float] NULL
	,[MassSIToUnitConvFactor] [float] NULL
);

INSERT INTO @DimUnitOfMeasure
(SKey, [UnitOfMeasureCode], [UnitOfMeasureName], [Description], [VolumeSIToUnitConvFactor], [MassSIToUnitConvFactor])
VALUES 
(1, N'SI', 'Système international', NULL, 1, 1),
(2, N'Metric', 'Metric System', NULL, 1000, 1),
(3, N'Imperial', 'Imperial System', NULL, 264.172037284185, 2.20462247603796)

MERGE INTO [dbo].[DimUnitOfMeasure] AS Target
USING 
(
	SELECT [SKey]
	,[UnitOfMeasureCode]
	,[UnitOfMeasureName]
	,[Description]
	,[VolumeSIToUnitConvFactor]
	,[MassSIToUnitConvFactor]
	FROM @DimUnitOfMeasure
) AS Source ([SKey], [UnitOfMeasureCode], [UnitOfMeasureName], [Description], [VolumeSIToUnitConvFactor], [MassSIToUnitConvFactor])
ON (Target.[SKey] = Source.[SKey])
WHEN MATCHED AND (ISNULL(Target.[UnitOfMeasureCode], '') <> ISNULL(Source.[UnitOfMeasureCode], '')) THEN
	UPDATE SET [UnitOfMeasureName] = Source.[UnitOfMeasureName],
				[Description] = Source.[Description],
				[VolumeSIToUnitConvFactor] = Source.[VolumeSIToUnitConvFactor],
				[MassSIToUnitConvFactor] = Source.[MassSIToUnitConvFactor]
WHEN NOT MATCHED THEN
	INSERT ([SKey], [UnitOfMeasureCode], [UnitOfMeasureName], [Description], [VolumeSIToUnitConvFactor], [MassSIToUnitConvFactor])
		VALUES (Source.[SKey], Source.[UnitOfMeasureCode], source.[UnitOfMeasureName], source.[Description], source.[VolumeSIToUnitConvFactor], Source.[MassSIToUnitConvFactor])
OUTPUT
	$action AS ActionType,
	deleted.[SKey],
	inserted.[SKey],
	deleted.[UnitOfMeasureCode],
	inserted.[UnitOfMeasureCode],
	deleted.[UnitOfMeasureName],
	inserted.[UnitOfMeasureName],
	deleted.[Description],
	inserted.[Description],
	deleted.[VolumeSIToUnitConvFactor],
	inserted.[VolumeSIToUnitConvFactor],
	deleted.[MassSIToUnitConvFactor],
	inserted.[MassSIToUnitConvFactor]
INTO @DimUnitOfMeasureData;

SELECT @DimUnitOfMeasureDataInserted = COUNT(*) FROM @DimUnitOfMeasureData WHERE ActionType IN ( 'INSERT' );
SELECT @DimUnitOfMeasureDataUpdated = COUNT(*) FROM @DimUnitOfMeasureData WHERE ActionType IN ( 'UPDATE' )
SELECT @DimUnitOfMeasureDataDeleted = COUNT(*) FROM @DimUnitOfMeasureData WHERE ActionType IN ( 'DELETE' )

IF (@DimUnitOfMeasureDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @DimUnitOfMeasureDataInserted) + ' NEW RECORDS INSERTED INTO [dbo].[DimUnitOfMeasure] **'
	PRINT ''
END

IF (@DimUnitOfMeasureDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @DimUnitOfMeasureDataUpdated) + ' EXISTING RECORDS UPDATED IN [dbo].[DimUnitOfMeasure] **'
	PRINT ''
	SELECT * FROM @DimUnitOfMeasureData WHERE ActionType IN ( 'UPDATE' );
END