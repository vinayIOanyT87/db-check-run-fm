/*
	DROP PROCEDURE [Staging].[usp_LoadTransactionAttributes]

	EXEC [staging].[usp_LoadTransactionAttributes]
	
*/
CREATE PROCEDURE [staging].[usp_LoadTransactionAttributes]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [staging].[usp_LoadTransactionAttributes]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Update the dbo.DimTransactionAttributes table.
	-- Notes:
	-- 1. The DimTransactionAttributes is populated dynamically from the transaction data. It does not have a direct counter part in the OLTP database.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	

		DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
		DECLARE @shortDummyId varchar(4) = '<NA>'
		DECLARE @veryShortDummyId varchar(2) = 'NA'

		DECLARE @dummyDateSKey int = 19000101
		DECLARE @dummyDateTime datetimeoffset(7) = '1/1/1900'
		DECLARE @defaultTimeSKey int = 0
		DECLARE @defaultBitValue bit = 0


		-- Transaction Attributes for FactTransaction
		TRUNCATE TABLE staging.tblTransactionAttributes	
		
	
		INSERT INTO staging.tblTransactionAttributes
		(
			[DeleteFlag], 
			[ReversalType], 
			[SubType], 
			[TransactionStatusName], 
			[InvalidTerminalTime], 
			[GrossQuantitySign],
			[IsRecordDeleted],
			[FactTransactionSKey]
		)
		SELECT 
			a.[DeleteFlag], 
			a.[ReversalType], 
			a.[SubType], 
			a.[TransactionStatusName], 
			(
				CASE 
					WHEN (c.TransactionTypeCode IN ('T5_PrimaryDisbursement') 
							AND 
							(
								(a.TimeIn IS NULL OR a.TimeOut IS NULL)
								OR (a.TimeIn >= a.TimeOut)
								OR (a.Line_MeterStartDateTime IS NULL OR a.Line_MeterStopDateTime IS NULL)
								OR (a.Line_MeterStartDateTime >= a.Line_MeterStopDateTime)
								OR (a.TimeIn >= a.Line_MeterStartDateTime)
								OR (a.Line_MeterStopDateTime >= a.TimeOut)
							)

						) 
					THEN 1 ELSE 0 
				END
			) [InvalidTerminalTime],
			(
				CASE
					WHEN a.Line_GrossQuantitySI >= 0 THEN 'Positive'
					WHEN a.Line_GrossQuantitySI < 0 THEN 'Negative'
					ELSE @shortDummyId
				END
			) [GrossQuantitySign],
			a.[_IsRecordDeleted],
			a.[SKey]
		FROM FactTransaction a
		INNER JOIN (SELECT DISTINCT FactTransactionSKey FROM staging.tblEditedFactTransaction) AS b		
		ON b.FactTransactionSKey = a.SKey
		LEFT OUTER JOIN DimTransactionType c
		ON c.SKey = a.TransactionTypeSKey


		-- Transaction Attributes for FactTransactionSummary
		TRUNCATE TABLE staging.tblTransactionSummaryAttributes	
		
	
		INSERT INTO staging.tblTransactionSummaryAttributes
		(
			[DeleteFlag], 
			[ReversalType], 
			[SubType], 
			[TransactionStatusName], 
			[InvalidTerminalTime], 
			[IsRecordDeleted],
			[FactTransactionSummarySKey]
		)
		SELECT 
			a.[DeleteFlag], 
			a.[ReversalType], 
			a.[SubType], 
			a.[TransactionStatusName], 
			(
				CASE 
					WHEN 
					(
						c.TransactionTypeCode IN ('T5_PrimaryDisbursement') 
						AND 
						(
							(a.TimeIn IS NULL OR a.TimeOut IS NULL)
							OR (a.TimeIn >= a.TimeOut)
							OR (a.Line_MeterMinStartTime IS NULL OR a.Line_MeterMaxStopTime IS NULL)
							OR (a.Line_MeterMinStartTime >= a.Line_MeterMaxStopTime)
							OR (a.TimeIn >= a.Line_MeterMinStartTime)
							OR (a.Line_MeterMaxStopTime >= a.TimeOut)
						)
					) 
					THEN 1 ELSE 0 
				END
			) [InvalidTerminalTime],
			a.[_IsRecordDeleted],
			a.[SKey]
		FROM FactTransactionSummary a
		INNER JOIN (SELECT DISTINCT FactTransactionSummarySKey FROM staging.tblEditedFactTransactionSummary) AS b		
		ON b.FactTransactionSummarySKey = a.SKey
		LEFT OUTER JOIN DimTransactionType c
		ON c.SKey = a.TransactionTypeSKey




		--Update the DimTransactionAttributes table with any new attribute combination extracted from the current batch of Transaction data
		INSERT INTO dbo.DimTransactionAttributes
		(
			[DeleteFlag], 
			[ReversalType], 
			[SubType], 
			[TransactionStatusName], 
			[InvalidTerminalTime], 
			[GrossQuantitySign],
			[IsRecordDeleted],
			[_DeletedFlag], 
			[_RecordUpdatedDate]
		)
		SELECT x.DeleteFlag, x.ReversalType, x.SubType, x.[TransactionStatusName], x.[InvalidTerminalTime], x.[GrossQuantitySign], x.[IsRecordDeleted], 0, GetDate()
		FROM
		(
			SELECT ISNULL(a.DeleteFlag, 0) DeleteFlag, 
			ISNULL(a.ReversalType, @veryShortDummyId) ReversalType, 
			ISNULL(a.SubType, @dummyId) SubType,
			ISNULL(a.TransactionStatusName, @dummyId) TransactionStatusName,
			a.InvalidTerminalTime,
			a.GrossQuantitySign,
			a.IsRecordDeleted
			FROM staging.tblTransactionAttributes a
			GROUP BY a.DeleteFlag, a.ReversalType, a.SubType, a.TransactionStatusName, a.InvalidTerminalTime, a.GrossQuantitySign, a.IsRecordDeleted
			UNION
			SELECT ISNULL(a.DeleteFlag, 0) DeleteFlag, 
			ISNULL(a.ReversalType, @veryShortDummyId) ReversalType, 
			ISNULL(a.SubType, @dummyId) SubType,
			ISNULL(a.TransactionStatusName, @dummyId) TransactionStatusName,
			a.InvalidTerminalTime,
			@shortDummyId GrossQuantitySign,
			a.IsRecordDeleted
			FROM staging.tblTransactionSummaryAttributes a
			GROUP BY a.DeleteFlag, a.ReversalType, a.SubType, a.TransactionStatusName, a.InvalidTerminalTime, a.IsRecordDeleted
		) x
		WHERE NOT EXISTS
		(
			SELECT * FROM dbo.DimTransactionAttributes b
			WHERE b.DeleteFlag = x.DeleteFlag
			AND b.ReversalType = x.ReversalType
			AND b.SubType = x.SubType
			AND b.TransactionStatusName = x.TransactionStatusName
			AND b.InvalidTerminalTime = x.InvalidTerminalTime
			AND b.GrossQuantitySign = x.GrossQuantitySign
			AND b.IsRecordDeleted = x.IsRecordDeleted
		)
					
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [staging].[usp_LoadTransactionAttributes]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END