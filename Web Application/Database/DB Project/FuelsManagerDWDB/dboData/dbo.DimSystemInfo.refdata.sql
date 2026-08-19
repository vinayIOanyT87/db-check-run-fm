--[dbo].[DimSystemInfo]
SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [dbo].[DimSystemInfo]'
PRINT ''

DECLARE @DimSystemInfoDataInserted bigint
DECLARE @DimSystemInfoDataUpdated bigint
DECLARE @DimSystemInfoDataDeleted bigint

SET @DimSystemInfoDataInserted = 0
SET @DimSystemInfoDataUpdated = 0
SET @DimSystemInfoDataDeleted = 0

DECLARE @DimSystemInfoData TABLE
(
	[ActionType] VARCHAR (50)
	,[OldSKey] INT
    ,[SKey] INT
    ,[OldFirstLoadDate] [datetimeoffset](7)
	,[FirstLoadDate] [datetimeoffset](7)
	,[OldLastLoadDate] [datetimeoffset](7)
	,[LastLoadDate] [datetimeoffset](7)
	,[LastLoadDateStr] [nvarchar](100)
	,[OldDataWarehouseVersion] [nvarchar](100)
	,[DataWarehouseVersion] [nvarchar](100)
	,[OldCDCActivationDate] [datetimeoffset](7)
	,[CDCActivationDate] [datetimeoffset](7)
);

DECLARE @DimSystemInfo TABLE
(
    [SKey] INT
    ,[FirstLoadDate] [datetimeoffset](7)
	,[LastLoadDate] [datetimeoffset](7)
	,[LastLoadDateStr] [nvarchar](100)
	,[ShowExtendedAttributes] [bit]
	,[DataWarehouseVersion] [nvarchar](100)
	,[CDCActivationDate] [datetimeoffset](7)
);

INSERT INTO @DimSystemInfo
(SKey, [FirstLoadDate], [LastLoadDate], [LastLoadDateStr], [DataWarehouseVersion], [CDCActivationDate])
VALUES 
(1, NULL, NULL, NULL, N'Version 1.00', NULL)

MERGE INTO [dbo].[DimSystemInfo] AS Target
USING 
(
	SELECT [SKey]
	, [FirstLoadDate]
	, [LastLoadDate]
	, [LastLoadDateStr]
	, [DataWarehouseVersion]
	, [CDCActivationDate]
	FROM @DimSystemInfo
) AS Source ([SKey], [FirstLoadDate], [LastLoadDate], [LastLoadDateStr], [DataWarehouseVersion], [CDCActivationDate])
ON (Target.[SKey] = Source.[SKey])
WHEN MATCHED AND (ISNULL(Target.[DataWarehouseVersion], '') <> ISNULL(Source.[DataWarehouseVersion], '')) THEN
	UPDATE SET [DataWarehouseVersion] = Source.[DataWarehouseVersion]
WHEN NOT MATCHED THEN
	INSERT ([SKey], [FirstLoadDate], [LastLoadDate], [LastLoadDateStr], [DataWarehouseVersion], [CDCActivationDate])
		VALUES (Source.[SKey], Source.[FirstLoadDate], source.[LastLoadDate], source.[LastLoadDateStr], source.[DataWarehouseVersion], source.[CDCActivationDate])
OUTPUT
	$action AS ActionType,
	deleted.[SKey],
	inserted.[SKey],
	deleted.[FirstLoadDate],
	inserted.[FirstLoadDate],
	deleted.[LastLoadDate],
	inserted.[LastLoadDate],
	inserted.[LastLoadDateStr],
	deleted.[DataWarehouseVersion],
	inserted.[DataWarehouseVersion],
	deleted.[CDCActivationDate],
	inserted.[CDCActivationDate]
INTO @DimSystemInfoData;

SELECT @DimSystemInfoDataInserted = COUNT(*) FROM @DimSystemInfoData WHERE ActionType IN ( 'INSERT' );
SELECT @DimSystemInfoDataUpdated = COUNT(*) FROM @DimSystemInfoData WHERE ActionType IN ( 'UPDATE' )
SELECT @DimSystemInfoDataDeleted = COUNT(*) FROM @DimSystemInfoData WHERE ActionType IN ( 'DELETE' )

IF (@DimSystemInfoDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @DimSystemInfoDataInserted) + ' NEW RECORDS INSERTED INTO [dbo].[DimSystemInfo] **'
	PRINT ''
END

IF (@DimSystemInfoDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @DimSystemInfoDataUpdated) + ' EXISTING RECORDS UPDATED IN [dbo].[DimSystemInfo] **'
	PRINT ''
	SELECT * FROM @DimSystemInfoData WHERE ActionType IN ( 'UPDATE' );
END