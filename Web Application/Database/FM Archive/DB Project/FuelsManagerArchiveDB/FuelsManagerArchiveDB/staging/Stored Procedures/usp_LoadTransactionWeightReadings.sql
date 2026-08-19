/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionWeightReadings]
 
	EXEC [staging].[usp_LoadTransactionWeightReadings]
 
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionWeightReadings]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadTransactionWeightReadings]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the TransactionWeightReadings records from staging into the tblTransactionWeightReadings table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblTransactionWeightReadings table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblTransactionWeightReadings
				WHERE IsProcessed = 0
			) = 0)
		BEGIN
			RETURN
		END
 
	DECLARE @tblInsertedRecords TABLE
	(
		[SKey] [int] IDENTITY(1,1) NOT NULL,
		[RecordGuid] [uniqueidentifier] NOT NULL,
		[RecordIndex] [bigint] NOT NULL,
		[ParentRecordGuid] [uniqueidentifier] NULL
	)
 
	DECLARE @tblUpdatedRecords TABLE
	(
		[SKey] [int] IDENTITY(1,1) NOT NULL,
		[RecordGuid] [uniqueidentifier] NOT NULL,
		[RecordIndex] [bigint] NOT NULL,
		[ParentRecordGuid] [uniqueidentifier] NULL
	)
 
		INSERT INTO @tblUpdatedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[TransactionWeightReadingGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionWeightReadings src
		WHERE src.TransactionWeightReadingGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionWeightReadings] tgt
			WHERE tgt.TransactionWeightReadingGuid = src.TransactionWeightReadingGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[TransactionWeightReadingGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionWeightReadings src
		WHERE src.TransactionWeightReadingGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionWeightReadings] tgt
			WHERE tgt.TransactionWeightReadingGuid = src.TransactionWeightReadingGuid
		)
 
 
		INSERT INTO [dbo].[tblTransactionWeightReadings]
		(
		[CompartmentID]
		, [BeginQuantityValue]
		, [RequestedQuantityValue]
		, [FinalQuantityValue]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransVersion]
		, [TransactionWeightReadingGuid]
		, [TransactionGuid]
		, [FuelsManagerVersionNumber]
		, [SourceVersionNumber]
		, [HistoricalFlag]
		, [VolumetricTopOffFlag]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[CompartmentID]
		, src.[BeginQuantityValue]
		, src.[RequestedQuantityValue]
		, src.[FinalQuantityValue]
		, src.[CreatedBy]
		, src.[CreatedDate]
		, src.[UpdatedBy]
		, src.[UpdatedDate]
		, src.[TransVersion]
		, src.[TransactionWeightReadingGuid]
		, src.[TransactionGuid]
		, src.[FuelsManagerVersionNumber]
		, src.[SourceVersionNumber]
		, src.[HistoricalFlag]
		, src.[VolumetricTopOffFlag]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblTransactionWeightReadings src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.TransactionWeightReadingGuid
		WHERE src.TransactionWeightReadingGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[CompartmentID] = src.[CompartmentID]
		, tgt.[BeginQuantityValue] = src.[BeginQuantityValue]
		, tgt.[RequestedQuantityValue] = src.[RequestedQuantityValue]
		, tgt.[FinalQuantityValue] = src.[FinalQuantityValue]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[TransVersion] = src.[TransVersion]
		, tgt.[TransactionWeightReadingGuid] = src.[TransactionWeightReadingGuid]
		, tgt.[TransactionGuid] = src.[TransactionGuid]
		, tgt.[FuelsManagerVersionNumber] = src.[FuelsManagerVersionNumber]
		, tgt.[SourceVersionNumber] = src.[SourceVersionNumber]
		, tgt.[HistoricalFlag] = src.[HistoricalFlag]
		, tgt.[VolumetricTopOffFlag] = src.[VolumetricTopOffFlag]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblTransactionWeightReadings] tgt
		INNER JOIN staging.tblTransactionWeightReadings src
		ON src.TransactionWeightReadingGuid = tgt.TransactionWeightReadingGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.TransactionWeightReadingGuid
		WHERE src.TransactionWeightReadingGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblTransactionWeightReadings SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionWeightReadings]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionWeightReadings]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblUpdatedRecords
 
	END TRY
	BEGIN CATCH
		DECLARE @_ErrMessage nvarchar(2048),
		@_ErrNumber int,
		@_ErrProcName nvarchar(126),
		@_ErrLineNumber int;
		SET @_ErrMessage = ERROR_MESSAGE();
		SET @_ErrNumber = ERROR_NUMBER();
		SET @_ErrProcName = ERROR_PROCEDURE();
		SET @_ErrLineNumber = ERROR_LINE();
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
		+ 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
		+ 'Procedure Name: [staging].[usp_LoadTransactionWeightReadings]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
