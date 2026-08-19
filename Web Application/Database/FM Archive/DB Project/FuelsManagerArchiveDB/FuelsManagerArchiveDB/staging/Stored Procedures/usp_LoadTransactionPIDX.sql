/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionPIDX]
 
	EXEC [staging].[usp_LoadTransactionPIDX]
 
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionPIDX]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadTransactionPIDX]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the TransactionPIDX records from staging into the tblTransactionPIDX table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblTransactionPIDX table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblTransactionPIDX
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
		SELECT src.[TransactionPIDXGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionPIDX src
		WHERE src.TransactionPIDXGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionPIDX] tgt
			WHERE tgt.TransactionPIDXGuid = src.TransactionPIDXGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[TransactionPIDXGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionPIDX src
		WHERE src.TransactionPIDXGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionPIDX] tgt
			WHERE tgt.TransactionPIDXGuid = src.TransactionPIDXGuid
		)
 
 
		INSERT INTO [dbo].[tblTransactionPIDX]
		(
		[AuthorizationNumber]
		, [SentFlag]
		, [DateSent]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [BrokenBlend]
		, [TransactionPIDXGuid]
		, [PIDXProfileGuid]
		, [TransactionGuid]
		, [CompanyPersonnelToShipToBillToGuid]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[AuthorizationNumber]
		, src.[SentFlag]
		, src.[DateSent]
		, src.[CreatedBy]
		, src.[CreatedDate]
		, src.[UpdatedBy]
		, src.[UpdatedDate]
		, src.[BrokenBlend]
		, src.[TransactionPIDXGuid]
		, src.[PIDXProfileGuid]
		, src.[TransactionGuid]
		, src.[CompanyPersonnelToShipToBillToGuid]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblTransactionPIDX src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.TransactionPIDXGuid
		WHERE src.TransactionPIDXGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[AuthorizationNumber] = src.[AuthorizationNumber]
		, tgt.[SentFlag] = src.[SentFlag]
		, tgt.[DateSent] = src.[DateSent]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[BrokenBlend] = src.[BrokenBlend]
		, tgt.[TransactionPIDXGuid] = src.[TransactionPIDXGuid]
		, tgt.[PIDXProfileGuid] = src.[PIDXProfileGuid]
		, tgt.[TransactionGuid] = src.[TransactionGuid]
		, tgt.[CompanyPersonnelToShipToBillToGuid] = src.[CompanyPersonnelToShipToBillToGuid]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblTransactionPIDX] tgt
		INNER JOIN staging.tblTransactionPIDX src
		ON src.TransactionPIDXGuid = tgt.TransactionPIDXGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.TransactionPIDXGuid
		WHERE src.TransactionPIDXGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblTransactionPIDX SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionPIDX]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionPIDX]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadTransactionPIDX]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
