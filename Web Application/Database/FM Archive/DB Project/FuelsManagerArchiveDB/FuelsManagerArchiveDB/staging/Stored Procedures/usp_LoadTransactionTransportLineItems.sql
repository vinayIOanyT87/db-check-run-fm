/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionTransportLineItems]
 
	EXEC [staging].[usp_LoadTransactionTransportLineItems]
 
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionTransportLineItems]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadTransactionTransportLineItems]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the TransactionTransportLineItems records from staging into the tblTransactionTransportLineItems table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblTransactionTransportLineItems table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblTransactionTransportLineItems
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
		SELECT src.[TransactionTransportLineItemGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionTransportLineItems src
		WHERE src.TransactionTransportLineItemGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionTransportLineItems] tgt
			WHERE tgt.TransactionTransportLineItemGuid = src.TransactionTransportLineItemGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[TransactionTransportLineItemGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionTransportLineItems src
		WHERE src.TransactionTransportLineItemGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionTransportLineItems] tgt
			WHERE tgt.TransactionTransportLineItemGuid = src.TransactionTransportLineItemGuid
		)
 
 
		INSERT INTO [dbo].[tblTransactionTransportLineItems]
		(
		[TransportOrderNumber]
		, [TransVersion]
		, [LocationName]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [POCName]
		, [POCPhone]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionTransportLineItemGuid]
		, [TransactionGuid]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[TransportOrderNumber]
		, src.[TransVersion]
		, src.[LocationName]
		, src.[Address1]
		, src.[Address2]
		, src.[City]
		, src.[State]
		, src.[Zip]
		, src.[POCName]
		, src.[POCPhone]
		, src.[CreatedBy]
		, src.[CreatedDate]
		, src.[UpdatedBy]
		, src.[UpdatedDate]
		, src.[TransactionTransportLineItemGuid]
		, src.[TransactionGuid]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblTransactionTransportLineItems src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.TransactionTransportLineItemGuid
		WHERE src.TransactionTransportLineItemGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[TransportOrderNumber] = src.[TransportOrderNumber]
		, tgt.[TransVersion] = src.[TransVersion]
		, tgt.[LocationName] = src.[LocationName]
		, tgt.[Address1] = src.[Address1]
		, tgt.[Address2] = src.[Address2]
		, tgt.[City] = src.[City]
		, tgt.[State] = src.[State]
		, tgt.[Zip] = src.[Zip]
		, tgt.[POCName] = src.[POCName]
		, tgt.[POCPhone] = src.[POCPhone]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[TransactionTransportLineItemGuid] = src.[TransactionTransportLineItemGuid]
		, tgt.[TransactionGuid] = src.[TransactionGuid]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblTransactionTransportLineItems] tgt
		INNER JOIN staging.tblTransactionTransportLineItems src
		ON src.TransactionTransportLineItemGuid = tgt.TransactionTransportLineItemGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.TransactionTransportLineItemGuid
		WHERE src.TransactionTransportLineItemGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblTransactionTransportLineItems SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionTransportLineItems]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionTransportLineItems]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadTransactionTransportLineItems]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
