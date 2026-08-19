/*
	DROP PROCEDURE [Staging].[usp_LoadExportResults]
 
	EXEC [staging].[usp_LoadExportResults]
 
*/
CREATE PROCEDURE [staging].[usp_LoadExportResults]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadExportResults]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the ExportResults records from staging into the tblExportResults table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblExportResults table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblExportResults
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
		SELECT src.[ExportResultGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblExportResults src
		WHERE src.ExportResultGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblExportResults] tgt
			WHERE tgt.ExportResultGuid = src.ExportResultGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[ExportResultGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblExportResults src
		WHERE src.ExportResultGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblExportResults] tgt
			WHERE tgt.ExportResultGuid = src.ExportResultGuid
		)
 
 
		INSERT INTO [dbo].[tblExportResults]
		(
		[InterfaceName]
		, [TransVersion]
		, [FailedCount]
		, [SuccessCount]
		, [TransDateTime]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [BatchID]
		, [ExportResultGuid]
		, [SiteGuid]
		, [LookupExportResultTypeIndex]
		, [ArchiveFileName]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[InterfaceName]
		, src.[TransVersion]
		, src.[FailedCount]
		, src.[SuccessCount]
		, src.[TransDateTime]
		, src.[CreatedDate]
		, src.[CreatedBy]
		, src.[UpdatedDate]
		, src.[UpdatedBy]
		, src.[BatchID]
		, src.[ExportResultGuid]
		, src.[SiteGuid]
		, src.[LookupExportResultTypeIndex]
		, src.[ArchiveFileName]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblExportResults src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.ExportResultGuid
		WHERE src.ExportResultGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[InterfaceName] = src.[InterfaceName]
		, tgt.[TransVersion] = src.[TransVersion]
		, tgt.[FailedCount] = src.[FailedCount]
		, tgt.[SuccessCount] = src.[SuccessCount]
		, tgt.[TransDateTime] = src.[TransDateTime]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[BatchID] = src.[BatchID]
		, tgt.[ExportResultGuid] = src.[ExportResultGuid]
		, tgt.[SiteGuid] = src.[SiteGuid]
		, tgt.[LookupExportResultTypeIndex] = src.[LookupExportResultTypeIndex]
		, tgt.[ArchiveFileName] = src.[ArchiveFileName]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblExportResults] tgt
		INNER JOIN staging.tblExportResults src
		ON src.ExportResultGuid = tgt.ExportResultGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.ExportResultGuid
		WHERE src.ExportResultGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblExportResults SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblExportResults]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblExportResults]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadExportResults]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
