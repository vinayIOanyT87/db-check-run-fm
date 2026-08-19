CREATE PROCEDURE [dbo].[usp_so_PopulateSiteNextNumbers]
	@SiteId NVARCHAR(30)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_so_PopulateSiteNextNumbers] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.0 / 2014-10-01 14:21:10.4470770 -04:00
	-- Purpose: Populate Sites Next Nubers on client after initial sync.
	-- Notes:
	-- 1. @SiteGuid: The site which has just completed initial sync
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		Declare @SiteGuid uniqueidentifier
		Set @SiteGuid = (Select SiteGuid from tblSites where ID = @SiteId)

		Declare @OrderEndNumber int
		SET @OrderEndNumber = (Select OrderEndNumber from tblSites where SiteGuid = @SiteGuid)
		Declare @ManualBOLEndNumber int
		SET @ManualBOLEndNumber = (Select ManualBOLEndNumber from tblSites where SiteGuid = @SiteGuid)
		Declare @TransactionEndNumber int
		SET @TransactionEndNumber = (Select TransactionEndNumber from tblSites where SiteGuid = @SiteGuid)
		Declare @InvoiceEndNumber int
		SET @InvoiceEndNumber = (Select TransactionEndNumber from tblSites where SiteGuid = @SiteGuid)

		Declare @OrderStartNumber int
		SET @OrderStartNumber = (Select OrderStartNumber from tblSites where SiteGuid = @SiteGuid)
		Declare @ManualBOLStartNumber int
		SET @ManualBOLStartNumber = (Select ManualBOLStartNumber from tblSites where SiteGuid = @SiteGuid)
		Declare @AutomaticBOLStartNumber int
		SET @AutomaticBOLStartNumber = (Select AutomaticBOLStartNumber from tblSites where SiteGuid = @SiteGuid)
		Declare @TransactionStartNumber int
		SET @TransactionStartNumber = (Select TransactionStartNumber from tblSites where SiteGuid = @SiteGuid)
		Declare @InvoiceStartNumber int
		SET @InvoiceStartNumber = (Select TransactionStartNumber from tblSites where SiteGuid = @SiteGuid)

		Declare @OrderEndNumberLen int
		SET @OrderEndNumberLen = LEN(@OrderEndNumber)
		Declare @ManualBOLEndNumberLen int
		SET @ManualBOLEndNumberLen = LEN(@ManualBOLEndNumber)
		Declare @TransactionEndNumberLen int
		SET @TransactionEndNumberLen = LEN(@TransactionEndNumber)
		Declare @InvoiceEndNumberLen int
		SET @InvoiceEndNumberLen = LEN(@InvoiceEndNumber)


		DECLARE @NumOrderTranactions int
		SET @NumOrderTranactions = 
		(
			SELECT COUNT(r.DocNum) FROM 
			(
				Select CASE when RIGHT(t.DocumentNumber,@OrderEndNumberLen - 1) not like '%[^0-9]%' then CAST(RIGHT(t.DocumentNumber,@OrderEndNumberLen - 1) AS INT) else @OrderStartNumber end AS DocNum from tblTransactions t
				inner join lookup.tblTransactionTypes tt
				ON t.LookupTransTypeIndex = tt.TransactionTypesIndex
				inner join lookup.tblTransactionOrigin o
				ON t.LookupOriginApplicationIndex = o.TransactionOriginIndex
				where t.SiteGuid = @SiteGuid and t.DocumentNumber IS NOT NULL 
				AND LEN(t.DocumentNumber) <> @OrderEndNumberLen 
				AND (tt.TransactionTypesCode = 'T17_Order' OR tt.TransactionTypesCode = 'T18_SupplyOrder') 
				AND (o.TransactionOriginCode = 'AdcUploadedAtBaseLevel'	OR o.TransactionOriginCode = 'BaseLevelTransaction')
			) r
			WHERE r.DocNum <= @OrderEndNumber 
		)

		DECLARE @NumManualBOLTranactions int
		SET @NumManualBOLTranactions = 
		(
			SELECT COUNT(r.DocNum) FROM 
			(
				Select CASE when RIGHT(t.DocumentNumber,@ManualBOLEndNumberLen - 1) not like '%[^0-9]%' then CAST(RIGHT(t.DocumentNumber,@ManualBOLEndNumberLen - 1) AS INT) else @ManualBOLStartNumber end AS DocNum from tblTransactions t
				inner join lookup.tblTransactionTypes tt
				ON t.LookupTransTypeIndex = tt.TransactionTypesIndex
				inner join lookup.tblTransactionOrigin o
				ON t.LookupOriginApplicationIndex = o.TransactionOriginIndex
				where t.SiteGuid = @SiteGuid and t.DocumentNumber IS NOT NULL 
				AND LEN(t.DocumentNumber) <> @ManualBOLEndNumberLen 
				AND (tt.TransactionTypesCode = 'T5_PrimaryDisbursement' OR tt.TransactionTypesCode = 'T6_SecondaryDisbursement' OR tt.TransactionTypesCode = 'T7_FillStand' OR tt.TransactionTypesCode = 'T8_ReceiptOR' OR tt.TransactionTypesCode = 'T23_StorageTransfer' OR tt.TransactionTypesCode = 'T25_Shipment') 
				AND (o.TransactionOriginCode = 'AdcUploadedAtBaseLevel'	OR o.TransactionOriginCode = 'BaseLevelTransaction')
			) r
			WHERE r.DocNum <= @ManualBOLEndNumber 
		)

		DECLARE @NumOtherTranactions int
		SET @NumOtherTranactions = 
		(
			SELECT COUNT(r.DocNum) FROM 
			(
				Select CASE when RIGHT(t.DocumentNumber,@TransactionEndNumberLen - 1) not like '%[^0-9]%' then CAST(RIGHT(t.DocumentNumber,@TransactionEndNumberLen - 1) AS INT) else @TransactionStartNumber end AS DocNum from tblTransactions t
				inner join lookup.tblTransactionTypes tt
				ON t.LookupTransTypeIndex = tt.TransactionTypesIndex
				inner join lookup.tblTransactionOrigin o
				ON t.LookupOriginApplicationIndex = o.TransactionOriginIndex
				where t.SiteGuid = @SiteGuid and t.DocumentNumber IS NOT NULL 
				AND LEN(t.DocumentNumber) <> @TransactionEndNumberLen 
				AND tt.TransactionTypesCode <> 'T5_PrimaryDisbursement' 
				AND tt.TransactionTypesCode <> 'T6_SecondaryDisbursement' 
				AND tt.TransactionTypesCode <> 'T7_FillStand' 
				AND tt.TransactionTypesCode <> 'T8_Receipt' 
				AND tt.TransactionTypesCode <> 'T17_Order' 
				AND tt.TransactionTypesCode <> 'T18_SupplyOrder'
				AND tt.TransactionTypesCode <> 'T22_AccountReceivableInvoice' 
				AND tt.TransactionTypesCode <> 'T23_StorageTransfer' 
				AND tt.TransactionTypesCode <> 'T25_Shipment' 
				AND (o.TransactionOriginCode = 'AdcUploadedAtBaseLevel'	OR o.TransactionOriginCode = 'BaseLevelTransaction')
			) r
			WHERE r.DocNum <= @TransactionEndNumber 
		)

		DECLARE @NumInvoiceTranactions int
		SET @NumInvoiceTranactions = 
		(
			SELECT COUNT(r.DocNum) FROM 
			(
				Select CASE when RIGHT(t.DocumentNumber,@InvoiceEndNumberLen - 1) not like '%[^0-9]%' then CAST(RIGHT(t.DocumentNumber,@InvoiceEndNumberLen - 1) AS INT) else @InvoiceStartNumber end AS DocNum from tblTransactions t
				inner join lookup.tblTransactionTypes tt
				ON t.LookupTransTypeIndex = tt.TransactionTypesIndex
				inner join lookup.tblTransactionOrigin o
				ON t.LookupOriginApplicationIndex = o.TransactionOriginIndex
				where t.SiteGuid = @SiteGuid and t.DocumentNumber IS NOT NULL 
				AND LEN(t.DocumentNumber) <> @InvoiceEndNumberLen 
				AND tt.TransactionTypesCode = 'T22_AccountReceivableInvoice'
				AND (o.TransactionOriginCode = 'AdcUploadedAtBaseLevel'	OR o.TransactionOriginCode = 'BaseLevelTransaction')
			) r
			WHERE r.DocNum <= @InvoiceEndNumber 
		)


		if @NumOrderTranactions > 0
		BEGIN
			DECLARE @OrderNextNum int
			Set @OrderNextNum = 
			( 
				SELECT TOP 1 r.DocNum FROM 
				(
					Select CASE when RIGHT(t.DocumentNumber,@OrderEndNumberLen - 1) not like '%[^0-9]%' then CAST(RIGHT(t.DocumentNumber,@OrderEndNumberLen - 1) AS INT) else @OrderStartNumber end AS DocNum from tblTransactions t
					inner join lookup.tblTransactionTypes tt
					ON t.LookupTransTypeIndex = tt.TransactionTypesIndex
					inner join lookup.tblTransactionOrigin o
					ON t.LookupOriginApplicationIndex = o.TransactionOriginIndex
					where t.SiteGuid = @SiteGuid and t.DocumentNumber IS NOT NULL 
					AND LEN(t.DocumentNumber) <> @OrderEndNumberLen 
					AND (tt.TransactionTypesCode = 'T17_Order' OR tt.TransactionTypesCode = 'T18_SupplyOrder') 
					AND (o.TransactionOriginCode = 'AdcUploadedAtBaseLevel'	OR o.TransactionOriginCode = 'BaseLevelTransaction')
				) r
				WHERE r.DocNum <= @OrderEndNumber 
				ORDER BY r.DocNum Desc
			)
			SET @OrderNextNum = @OrderNextNum + 1
			Update tblSites Set OrderNextNumber = @OrderNextNum, UpdatedDate = SYSDATETIMEOFFSET()
			WHERE SiteGuid = @SiteGuid
		END
		ELSE
		BEGIN
			Update tblSites Set OrderNextNumber = @OrderStartNumber, UpdatedDate = SYSDATETIMEOFFSET()
		END

		if @NumManualBOLTranactions > 0
		BEGIN
			DECLARE @ManualBOLNextNum int
			Set @ManualBOLNextNum = 
			( 
				SELECT TOP 1 r.DocNum FROM 
				(
					Select CASE when RIGHT(t.DocumentNumber,@ManualBOLEndNumberLen - 1) not like '%[^0-9]%' then CAST(RIGHT(t.DocumentNumber,@ManualBOLEndNumberLen - 1) AS INT) else @ManualBOLStartNumber end AS DocNum from tblTransactions t
					inner join lookup.tblTransactionTypes tt
					ON t.LookupTransTypeIndex = tt.TransactionTypesIndex
					inner join lookup.tblTransactionOrigin o
					ON t.LookupOriginApplicationIndex = o.TransactionOriginIndex
					where t.SiteGuid = @SiteGuid and t.DocumentNumber IS NOT NULL 
					AND LEN(t.DocumentNumber) <> @ManualBOLEndNumberLen 
					AND (tt.TransactionTypesCode = 'T5_PrimaryDisbursement' OR tt.TransactionTypesCode = 'T6_SecondaryDisbursement' OR tt.TransactionTypesCode = 'T7_FillStand' OR tt.TransactionTypesCode = 'T8_ReceiptOR' OR tt.TransactionTypesCode = 'T23_StorageTransfer' OR tt.TransactionTypesCode = 'T25_Shipment') 
					AND (o.TransactionOriginCode = 'AdcUploadedAtBaseLevel'	OR o.TransactionOriginCode = 'BaseLevelTransaction')

				) r
				WHERE r.DocNum <= @ManualBOLEndNumber 
				ORDER BY r.DocNum Desc
			)
			SET @ManualBOLNextNum = @ManualBOLNextNum + 1
			Update tblSites Set ManualBOLNextNumber = @ManualBOLNextNum, UpdatedDate = SYSDATETIMEOFFSET()
			WHERE SiteGuid = @SiteGuid
		END
		ELSE
		BEGIN
			Update tblSites Set ManualBOLNextNumber = @ManualBOLStartNumber, UpdatedDate = SYSDATETIMEOFFSET()
		END

		Update tblSites Set AutomaticBOLNextNumber = @AutomaticBOLStartNumber, UpdatedDate = SYSDATETIMEOFFSET()

		if @NumOtherTranactions > 0
		BEGIN
			DECLARE @OtherNextNum int
			Set @OtherNextNum = 
			( 
				SELECT TOP 1 r.DocNum FROM 
				(
					Select CASE when RIGHT(t.DocumentNumber,@TransactionEndNumberLen - 1) not like '%[^0-9]%' then CAST(RIGHT(t.DocumentNumber,@TransactionEndNumberLen - 1) AS INT) else @TransactionStartNumber end AS DocNum from tblTransactions t
					inner join lookup.tblTransactionTypes tt
					ON t.LookupTransTypeIndex = tt.TransactionTypesIndex
					inner join lookup.tblTransactionOrigin o
					ON t.LookupOriginApplicationIndex = o.TransactionOriginIndex
					where t.SiteGuid = @SiteGuid and t.DocumentNumber IS NOT NULL 
					AND LEN(t.DocumentNumber) <> @TransactionEndNumberLen 
					AND tt.TransactionTypesCode <> 'T5_PrimaryDisbursement' 
					AND tt.TransactionTypesCode <> 'T6_SecondaryDisbursement' 
					AND tt.TransactionTypesCode <> 'T7_FillStand' 
					AND tt.TransactionTypesCode <> 'T8_Receipt' 
					AND tt.TransactionTypesCode <> 'T17_Order' 
					AND tt.TransactionTypesCode <> 'T18_SupplyOrder'
					AND tt.TransactionTypesCode <> 'T22_AccountReceivableInvoice' 
					AND tt.TransactionTypesCode <> 'T23_StorageTransfer' 
					AND tt.TransactionTypesCode <> 'T25_Shipment' 
					AND (o.TransactionOriginCode = 'AdcUploadedAtBaseLevel'	OR o.TransactionOriginCode = 'BaseLevelTransaction')

				) r
				WHERE r.DocNum <= @TransactionEndNumber 
				ORDER BY r.DocNum Desc
			)
			SET @OtherNextNum = @OtherNextNum + 1
			Update tblSites Set TransactionNextNumber = @OtherNextNum, UpdatedDate = SYSDATETIMEOFFSET()
			WHERE SiteGuid = @SiteGuid
		END
		ELSE
		BEGIN
			Update tblSites Set TransactionNextNumber = @TransactionStartNumber, UpdatedDate = SYSDATETIMEOFFSET()
		END

		if @NumInvoiceTranactions > 0
		BEGIN
			DECLARE @InvoiceNextNum int
			Set @InvoiceNextNum = 
			( 
				SELECT TOP 1 r.DocNum FROM 
				(
					Select CASE when RIGHT(t.DocumentNumber,@InvoiceEndNumberLen - 1) not like '%[^0-9]%' then CAST(RIGHT(t.DocumentNumber,@InvoiceEndNumberLen - 1) AS INT) else @InvoiceStartNumber end AS DocNum from tblTransactions t
					inner join lookup.tblTransactionTypes tt
					ON t.LookupTransTypeIndex = tt.TransactionTypesIndex
					inner join lookup.tblTransactionOrigin o
					ON t.LookupOriginApplicationIndex = o.TransactionOriginIndex
					where t.SiteGuid = @SiteGuid and t.DocumentNumber IS NOT NULL 
					AND LEN(t.DocumentNumber) <> @InvoiceEndNumberLen 
					AND tt.TransactionTypesCode = 'T22_AccountReceivableInvoice'
					AND (o.TransactionOriginCode = 'AdcUploadedAtBaseLevel'	OR o.TransactionOriginCode = 'BaseLevelTransaction')

				) r
				WHERE r.DocNum <= @InvoiceEndNumber 
				ORDER BY r.DocNum Desc
			)
			SET @InvoiceNextNum = @InvoiceNextNum + 1
			Update tblSites Set InvoiceNextNumber = @InvoiceNextNum, UpdatedDate = SYSDATETIMEOFFSET()
			WHERE SiteGuid = @SiteGuid
		END
		ELSE
		BEGIN
			Update tblSites Set InvoiceNextNumber = @InvoiceStartNumber, UpdatedDate = SYSDATETIMEOFFSET()
		END

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
						+ 'Procedure Name: [dbo].[usp_so_PopulateSiteNextNumbers]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END