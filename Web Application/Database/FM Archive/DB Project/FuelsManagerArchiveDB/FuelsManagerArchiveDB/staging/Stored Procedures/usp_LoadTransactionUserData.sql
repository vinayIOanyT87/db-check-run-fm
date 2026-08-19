/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionUserData]
 
	EXEC [staging].[usp_LoadTransactionUserData]
 
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionUserData]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadTransactionUserData]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the TransactionUserData records from staging into the tblTransactionUserData table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblTransactionUserData table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblTransactionUserData
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
		SELECT src.[TransactionUserDataGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionUserData src
		WHERE src.TransactionUserDataGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionUserData] tgt
			WHERE tgt.TransactionUserDataGuid = src.TransactionUserDataGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[TransactionUserDataGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionUserData src
		WHERE src.TransactionUserDataGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionUserData] tgt
			WHERE tgt.TransactionUserDataGuid = src.TransactionUserDataGuid
		)
 
 
		INSERT INTO [dbo].[tblTransactionUserData]
		(
		[UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [UserData9]
		, [UserData10]
		, [UserData11]
		, [UserData12]
		, [UserData13]
		, [UserData14]
		, [UserData15]
		, [UserData16]
		, [UserData17]
		, [UserData18]
		, [UserData19]
		, [UserData20]
		, [UserData21]
		, [UserData22]
		, [UserData23]
		, [UserData24]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionUserDataGuid]
		, [TransactionGuid]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[UserData1]
		, src.[UserData2]
		, src.[UserData3]
		, src.[UserData4]
		, src.[UserData5]
		, src.[UserData6]
		, src.[UserData7]
		, src.[UserData8]
		, src.[UserData9]
		, src.[UserData10]
		, src.[UserData11]
		, src.[UserData12]
		, src.[UserData13]
		, src.[UserData14]
		, src.[UserData15]
		, src.[UserData16]
		, src.[UserData17]
		, src.[UserData18]
		, src.[UserData19]
		, src.[UserData20]
		, src.[UserData21]
		, src.[UserData22]
		, src.[UserData23]
		, src.[UserData24]
		, src.[CreatedBy]
		, src.[CreatedDate]
		, src.[UpdatedBy]
		, src.[UpdatedDate]
		, src.[TransactionUserDataGuid]
		, src.[TransactionGuid]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblTransactionUserData src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.TransactionUserDataGuid
		WHERE src.TransactionUserDataGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[UserData1] = src.[UserData1]
		, tgt.[UserData2] = src.[UserData2]
		, tgt.[UserData3] = src.[UserData3]
		, tgt.[UserData4] = src.[UserData4]
		, tgt.[UserData5] = src.[UserData5]
		, tgt.[UserData6] = src.[UserData6]
		, tgt.[UserData7] = src.[UserData7]
		, tgt.[UserData8] = src.[UserData8]
		, tgt.[UserData9] = src.[UserData9]
		, tgt.[UserData10] = src.[UserData10]
		, tgt.[UserData11] = src.[UserData11]
		, tgt.[UserData12] = src.[UserData12]
		, tgt.[UserData13] = src.[UserData13]
		, tgt.[UserData14] = src.[UserData14]
		, tgt.[UserData15] = src.[UserData15]
		, tgt.[UserData16] = src.[UserData16]
		, tgt.[UserData17] = src.[UserData17]
		, tgt.[UserData18] = src.[UserData18]
		, tgt.[UserData19] = src.[UserData19]
		, tgt.[UserData20] = src.[UserData20]
		, tgt.[UserData21] = src.[UserData21]
		, tgt.[UserData22] = src.[UserData22]
		, tgt.[UserData23] = src.[UserData23]
		, tgt.[UserData24] = src.[UserData24]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[TransactionUserDataGuid] = src.[TransactionUserDataGuid]
		, tgt.[TransactionGuid] = src.[TransactionGuid]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblTransactionUserData] tgt
		INNER JOIN staging.tblTransactionUserData src
		ON src.TransactionUserDataGuid = tgt.TransactionUserDataGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.TransactionUserDataGuid
		WHERE src.TransactionUserDataGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblTransactionUserData SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionUserData]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionUserData]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadTransactionUserData]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
