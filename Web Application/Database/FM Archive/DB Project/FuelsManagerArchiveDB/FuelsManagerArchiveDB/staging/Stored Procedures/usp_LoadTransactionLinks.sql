/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionLinks]
 
	EXEC [staging].[usp_LoadTransactionLinks]
 
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionLinks]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadTransactionLinks]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the TransactionLinks records from staging into the tblTransactionLinks table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblTransactionLinks table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblTransactionLinks
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
		SELECT src.[TransactionLinkGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionLinks src
		WHERE src.TransactionLinkGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionLinks] tgt
			WHERE tgt.TransactionLinkGuid = src.TransactionLinkGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[TransactionLinkGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionLinks src
		WHERE src.TransactionLinkGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionLinks] tgt
			WHERE tgt.TransactionLinkGuid = src.TransactionLinkGuid
		)
 
 
		INSERT INTO [dbo].[tblTransactionLinks]
		(
		[OriginalTransID]
		, [LinkedTransID]
		, [Level]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionLinkGuid]
		, [SiteGuid]
		, [LinkedTransactionLineItemGuid]
		, [TransactionLineItemGuid]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[OriginalTransID]
		, src.[LinkedTransID]
		, src.[Level]
		, src.[CreatedBy]
		, src.[CreatedDate]
		, src.[UpdatedBy]
		, src.[UpdatedDate]
		, src.[TransactionLinkGuid]
		, src.[SiteGuid]
		, src.[LinkedTransactionLineItemGuid]
		, src.[TransactionLineItemGuid]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblTransactionLinks src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.TransactionLinkGuid
		WHERE src.TransactionLinkGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[OriginalTransID] = src.[OriginalTransID]
		, tgt.[LinkedTransID] = src.[LinkedTransID]
		, tgt.[Level] = src.[Level]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[TransactionLinkGuid] = src.[TransactionLinkGuid]
		, tgt.[SiteGuid] = src.[SiteGuid]
		, tgt.[LinkedTransactionLineItemGuid] = src.[LinkedTransactionLineItemGuid]
		, tgt.[TransactionLineItemGuid] = src.[TransactionLineItemGuid]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblTransactionLinks] tgt
		INNER JOIN staging.tblTransactionLinks src
		ON src.TransactionLinkGuid = tgt.TransactionLinkGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.TransactionLinkGuid
		WHERE src.TransactionLinkGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblTransactionLinks SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionLinks]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionLinks]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadTransactionLinks]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
