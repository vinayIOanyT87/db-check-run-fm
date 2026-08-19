/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionSubLineItems]
 
	EXEC [staging].[usp_LoadTransactionSubLineItems]
 
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionSubLineItems]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadTransactionSubLineItems]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the TransactionSubLineItems records from staging into the tblTransactionSubLineItems table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblTransactionSubLineItems table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblTransactionSubLineItems
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
		SELECT src.[TransactionSubLineItemGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionSubLineItems src
		WHERE src.TransactionSubLineItemGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionSubLineItems] tgt
			WHERE tgt.TransactionSubLineItemGuid = src.TransactionSubLineItemGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex], [ParentRecordGuid])
		SELECT src.[TransactionSubLineItemGuid], src.[SourceClusterIdx], src.[TransactionGuid]
		FROM staging.tblTransactionSubLineItems src
		WHERE src.TransactionSubLineItemGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionSubLineItems] tgt
			WHERE tgt.TransactionSubLineItemGuid = src.TransactionSubLineItemGuid
		)
 
 
		INSERT INTO [dbo].[tblTransactionSubLineItems]
		(
		[SequenceID]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [GrossQuantity]
		, [NetQuantity]
		, [Vcf]
		, [Density]
		, [Temperature]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [BatchNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [TankStatus]
		, [MeterFactor]
		, [MeterStart]
		, [MeterStop]
		, [MeterStopDateTime]
		, [MeterStartDateTime]
		, [FreezePoint]
		, [DifferentialPressure]
		, [DosageRate]
		, [DeleteFlag]
		, [PresetAmount]
		, [StorageLocationID]
		, [MeterID]
		, [COAID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionInventoryDate]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [TransactionSubLineItemGuid]
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [TransactionLineItemGuid]
		, [ProductGuid]
		, [TransactionGuid]
		, [StorageLocationTankGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [InventoryDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[SequenceID]
		, src.[Product]
		, src.[ProductCode]
		, src.[ProductType]
		, src.[GrossQuantity]
		, src.[NetQuantity]
		, src.[Vcf]
		, src.[Density]
		, src.[Temperature]
		, src.[Customs]
		, src.[ArmNumber]
		, src.[LineNumber]
		, src.[BatchNumber]
		, src.[LineFill]
		, src.[BottomVolume]
		, src.[NetCapacity]
		, src.[TankStatus]
		, src.[MeterFactor]
		, src.[MeterStart]
		, src.[MeterStop]
		, src.[MeterStopDateTime]
		, src.[MeterStartDateTime]
		, src.[FreezePoint]
		, src.[DifferentialPressure]
		, src.[DosageRate]
		, src.[DeleteFlag]
		, src.[PresetAmount]
		, src.[StorageLocationID]
		, src.[MeterID]
		, src.[COAID]
		, src.[CreatedBy]
		, src.[CreatedDate]
		, src.[UpdatedBy]
		, src.[UpdatedDate]
		, src.[TransactionInventoryDate]
		, src.[Tax1]
		, src.[Tax2]
		, src.[Tax3]
		, src.[Tax4]
		, src.[Tax5]
		, src.[TransVersion]
		, src.[ImproperAdditization]
		, src.[BrokenBlend]
		, src.[Flag01]
		, src.[Flag02]
		, src.[Flag03]
		, src.[Flag04]
		, src.[Flag05]
		, src.[Flag06]
		, src.[Number01]
		, src.[Number02]
		, src.[Number03]
		, src.[Number04]
		, src.[Number05]
		, src.[Number06]
		, src.[Date01]
		, src.[Date02]
		, src.[Date03]
		, src.[Date04]
		, src.[MassQuantity]
		, src.[NetManualValueFlag]
		, src.[MassManualValueFlag]
		, src.[GrossManualValueFlag]
		, src.[VcfManualValueFlag]
		, src.[TransactionSubLineItemGuid]
		, src.[LookupTransactionStatusIndex]
		, src.[LookupQualityIndex]
		, src.[TransactionLineItemGuid]
		, src.[ProductGuid]
		, src.[TransactionGuid]
		, src.[StorageLocationTankGuid]
		, src.[MeterGuid]
		, src.[PackageManualValueFlag]
		, src.[CleanLineItem]
		, src.[CleanLineDeductItem]
		, src.[CleanLineDeductQuantity]
		, src.[CleanLinePackQuantity]
		, src.[InventoryDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblTransactionSubLineItems src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.TransactionSubLineItemGuid
		WHERE src.TransactionSubLineItemGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[SequenceID] = src.[SequenceID]
		, tgt.[Product] = src.[Product]
		, tgt.[ProductCode] = src.[ProductCode]
		, tgt.[ProductType] = src.[ProductType]
		, tgt.[GrossQuantity] = src.[GrossQuantity]
		, tgt.[NetQuantity] = src.[NetQuantity]
		, tgt.[Vcf] = src.[Vcf]
		, tgt.[Density] = src.[Density]
		, tgt.[Temperature] = src.[Temperature]
		, tgt.[Customs] = src.[Customs]
		, tgt.[ArmNumber] = src.[ArmNumber]
		, tgt.[LineNumber] = src.[LineNumber]
		, tgt.[BatchNumber] = src.[BatchNumber]
		, tgt.[LineFill] = src.[LineFill]
		, tgt.[BottomVolume] = src.[BottomVolume]
		, tgt.[NetCapacity] = src.[NetCapacity]
		, tgt.[TankStatus] = src.[TankStatus]
		, tgt.[MeterFactor] = src.[MeterFactor]
		, tgt.[MeterStart] = src.[MeterStart]
		, tgt.[MeterStop] = src.[MeterStop]
		, tgt.[MeterStopDateTime] = src.[MeterStopDateTime]
		, tgt.[MeterStartDateTime] = src.[MeterStartDateTime]
		, tgt.[FreezePoint] = src.[FreezePoint]
		, tgt.[DifferentialPressure] = src.[DifferentialPressure]
		, tgt.[DosageRate] = src.[DosageRate]
		, tgt.[DeleteFlag] = src.[DeleteFlag]
		, tgt.[PresetAmount] = src.[PresetAmount]
		, tgt.[StorageLocationID] = src.[StorageLocationID]
		, tgt.[MeterID] = src.[MeterID]
		, tgt.[COAID] = src.[COAID]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[TransactionInventoryDate] = src.[TransactionInventoryDate]
		, tgt.[Tax1] = src.[Tax1]
		, tgt.[Tax2] = src.[Tax2]
		, tgt.[Tax3] = src.[Tax3]
		, tgt.[Tax4] = src.[Tax4]
		, tgt.[Tax5] = src.[Tax5]
		, tgt.[TransVersion] = src.[TransVersion]
		, tgt.[ImproperAdditization] = src.[ImproperAdditization]
		, tgt.[BrokenBlend] = src.[BrokenBlend]
		, tgt.[Flag01] = src.[Flag01]
		, tgt.[Flag02] = src.[Flag02]
		, tgt.[Flag03] = src.[Flag03]
		, tgt.[Flag04] = src.[Flag04]
		, tgt.[Flag05] = src.[Flag05]
		, tgt.[Flag06] = src.[Flag06]
		, tgt.[Number01] = src.[Number01]
		, tgt.[Number02] = src.[Number02]
		, tgt.[Number03] = src.[Number03]
		, tgt.[Number04] = src.[Number04]
		, tgt.[Number05] = src.[Number05]
		, tgt.[Number06] = src.[Number06]
		, tgt.[Date01] = src.[Date01]
		, tgt.[Date02] = src.[Date02]
		, tgt.[Date03] = src.[Date03]
		, tgt.[Date04] = src.[Date04]
		, tgt.[MassQuantity] = src.[MassQuantity]
		, tgt.[NetManualValueFlag] = src.[NetManualValueFlag]
		, tgt.[MassManualValueFlag] = src.[MassManualValueFlag]
		, tgt.[GrossManualValueFlag] = src.[GrossManualValueFlag]
		, tgt.[VcfManualValueFlag] = src.[VcfManualValueFlag]
		, tgt.[TransactionSubLineItemGuid] = src.[TransactionSubLineItemGuid]
		, tgt.[LookupTransactionStatusIndex] = src.[LookupTransactionStatusIndex]
		, tgt.[LookupQualityIndex] = src.[LookupQualityIndex]
		, tgt.[TransactionLineItemGuid] = src.[TransactionLineItemGuid]
		, tgt.[ProductGuid] = src.[ProductGuid]
		, tgt.[TransactionGuid] = src.[TransactionGuid]
		, tgt.[StorageLocationTankGuid] = src.[StorageLocationTankGuid]
		, tgt.[MeterGuid] = src.[MeterGuid]
		, tgt.[PackageManualValueFlag] = src.[PackageManualValueFlag]
		, tgt.[CleanLineItem] = src.[CleanLineItem]
		, tgt.[CleanLineDeductItem] = src.[CleanLineDeductItem]
		, tgt.[CleanLineDeductQuantity] = src.[CleanLineDeductQuantity]
		, tgt.[CleanLinePackQuantity] = src.[CleanLinePackQuantity]
		, tgt.[InventoryDateKey] = src.[InventoryDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblTransactionSubLineItems] tgt
		INNER JOIN staging.tblTransactionSubLineItems src
		ON src.TransactionSubLineItemGuid = tgt.TransactionSubLineItemGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.TransactionSubLineItemGuid
		WHERE src.TransactionSubLineItemGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblTransactionSubLineItems SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionSubLineItems]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblTransactionSubLineItems]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadTransactionSubLineItems]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
