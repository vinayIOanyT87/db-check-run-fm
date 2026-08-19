/*
	DROP PROCEDURE [staging].[usp_ValidateTransactionLoading]

	EXEC [staging].[usp_ValidateTransactionLoading]
	
*/
CREATE PROCEDURE [staging].[usp_ValidateTransactionLoading]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ValidateTransactionLoading]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Verifies that the transaction records in Staging can all be located in the target Archive table.
  -- Notes:
  -- 1. 
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
	
    IF 
	(
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionLineItems] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionLineItems]  b
				WHERE b.TransactionGuid = a.TransactionGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionLineItemUserData] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionLineItemUserData] b
				WHERE b.TransactionLineItemUserDataGuid = a.TransactionLineItemUserDataGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionLinks] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionLinks]  b
				WHERE b.TransactionLinkGuid = a.TransactionLinkGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionNotes] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionNotes]  b
				WHERE b.TransactionNoteGuid = a.TransactionNoteGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionPIDX] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionPIDX]  b
				WHERE b.TransactionPIDXGuid = a.TransactionPIDXGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactions] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactions]  b
				WHERE b.TransactionGuid = a.TransactionGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionSignature] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionSignature]  b
				WHERE b.TransactionSignatureGuid = a.TransactionSignatureGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionSubLineItems] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionSubLineItems]  b
				WHERE b.TransactionSubLineItemGuid = a.TransactionSubLineItemGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionTransportLineItems] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionTransportLineItems]  b
				WHERE b.TransactionTransportLineItemGuid = a.TransactionTransportLineItemGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionUserData] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionUserData]  b
				WHERE b.TransactionUserDataGuid = a.TransactionUserDataGuid
			)
		) > 0
		OR
		(
			SELECT COUNT(*) FROM [staging].[tblTransactionWeightReadings] a
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionWeightReadings]  b
				WHERE b.TransactionWeightReadingGuid = a.TransactionWeightReadingGuid
			)
		) > 0
	)	
	BEGIN
		RAISERROR('Transation loading validation failure. Not all the Transaction Staging records were loaded in the target Archive tables.',16,1); 
		RETURN;
	END

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
    + 'Procedure Name: [staging].[usp_ValidateTransactionLoading]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END