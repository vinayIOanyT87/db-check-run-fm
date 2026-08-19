/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionSignature]
 
	EXEC [staging].[usp_LoadTransactionSignature]
 
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionSignature]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadTransactionSignature]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the TransactionSignature records from staging into the tblTransactionSignature table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblTransactionSignature table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblTransactionSignature
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
		SELECT src.[TransactionSignatureGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionSignature src
		WHERE src.TransactionSignatureGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionSignature] tgt
			WHERE tgt.TransactionSignatureGuid = src.TransactionSignatureGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[TransactionSignatureGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionSignature src
		WHERE src.TransactionSignatureGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionSignature] tgt
			WHERE tgt.TransactionSignatureGuid = src.TransactionSignatureGuid
		)
 
 
		INSERT INTO [dbo].[tblTransactionSignature]
		(
		[Signature]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionSignatureGuid]
		, [TransactionGuid]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[Signature]
		, src.[CreatedBy]
		, src.[CreatedDate]
		, src.[UpdatedBy]
		, src.[UpdatedDate]
		, src.[TransactionSignatureGuid]
		, src.[TransactionGuid]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblTransactionSignature src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.TransactionSignatureGuid
		WHERE src.TransactionSignatureGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[Signature] = src.[Signature]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[TransactionSignatureGuid] = src.[TransactionSignatureGuid]
		, tgt.[TransactionGuid] = src.[TransactionGuid]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblTransactionSignature] tgt
		INNER JOIN staging.tblTransactionSignature src
		ON src.TransactionSignatureGuid = tgt.TransactionSignatureGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.TransactionSignatureGuid
		WHERE src.TransactionSignatureGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblTransactionSignature SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionSignature]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionSignature]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadTransactionSignature]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
