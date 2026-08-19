/*
	DROP PROCEDURE [Staging].[usp_LoadExportResultDetails]
 
	EXEC [staging].[usp_LoadExportResultDetails]
 
*/
CREATE PROCEDURE [staging].[usp_LoadExportResultDetails]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadExportResultDetails]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the ExportResultDetails records from staging into the tblExportResultDetails table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblExportResultDetails table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblExportResultDetails
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
		SELECT src.[ExportResultDetailGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblExportResultDetails src
		WHERE src.ExportResultDetailGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblExportResultDetails] tgt
			WHERE tgt.ExportResultDetailGuid = src.ExportResultDetailGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[ExportResultDetailGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblExportResultDetails src
		WHERE src.ExportResultDetailGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblExportResultDetails] tgt
			WHERE tgt.ExportResultDetailGuid = src.ExportResultDetailGuid
		)
 
 
		INSERT INTO [dbo].[tblExportResultDetails]
		(
		[RecordID]
		, [Fail]
		, [TransVersion]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [Error]
		, [ExportResultDetailGuid]
		, [ExportResultGuid]
		, [InterfaceData01]
		, [InterfaceData02]
		, [InterfaceData03]
		, [InterfaceData04]
		, [InterfaceData05]
		, [InterfaceData06]
		, [InterfaceData07]
		, [InterfaceData08]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[RecordID]
		, src.[Fail]
		, src.[TransVersion]
		, src.[CreatedDate]
		, src.[CreatedBy]
		, src.[UpdatedDate]
		, src.[UpdatedBy]
		, src.[Error]
		, src.[ExportResultDetailGuid]
		, src.[ExportResultGuid]
		, src.[InterfaceData01]
		, src.[InterfaceData02]
		, src.[InterfaceData03]
		, src.[InterfaceData04]
		, src.[InterfaceData05]
		, src.[InterfaceData06]
		, src.[InterfaceData07]
		, src.[InterfaceData08]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblExportResultDetails src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.ExportResultDetailGuid
		WHERE src.ExportResultDetailGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[RecordID] = src.[RecordID]
		, tgt.[Fail] = src.[Fail]
		, tgt.[TransVersion] = src.[TransVersion]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[Error] = src.[Error]
		, tgt.[ExportResultDetailGuid] = src.[ExportResultDetailGuid]
		, tgt.[ExportResultGuid] = src.[ExportResultGuid]
		, tgt.[InterfaceData01] = src.[InterfaceData01]
		, tgt.[InterfaceData02] = src.[InterfaceData02]
		, tgt.[InterfaceData03] = src.[InterfaceData03]
		, tgt.[InterfaceData04] = src.[InterfaceData04]
		, tgt.[InterfaceData05] = src.[InterfaceData05]
		, tgt.[InterfaceData06] = src.[InterfaceData06]
		, tgt.[InterfaceData07] = src.[InterfaceData07]
		, tgt.[InterfaceData08] = src.[InterfaceData08]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblExportResultDetails] tgt
		INNER JOIN staging.tblExportResultDetails src
		ON src.ExportResultDetailGuid = tgt.ExportResultDetailGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.ExportResultDetailGuid
		WHERE src.ExportResultDetailGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblExportResultDetails SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblExportResultDetails]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblExportResultDetails]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadExportResultDetails]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
