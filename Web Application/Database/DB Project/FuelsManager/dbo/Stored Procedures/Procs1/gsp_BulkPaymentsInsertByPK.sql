CREATE PROCEDURE [dbo].[gsp_BulkPaymentsInsertByPK]
(
		@BulkPaymentGuid uniqueidentifier=NULL OUTPUT
	,	@Site nvarchar(60)=NULL
	,	@Section nvarchar(60)=NULL
	,	@PaymentType nvarchar(60)=NULL
	,	@ForeignRate float=NULL
	,	@ForeignUnit nvarchar(60)=NULL
	,	@RomanNumber nvarchar(60)=NULL
	,	@DiscountRate float=NULL
	,	@PaymentDueDate datetimeoffset(7)=NULL
	,	@TransactionDate datetimeoffset(7)=NULL
	,	@Supplier nvarchar(60)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_BulkPaymentsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0902767 -05:00
	-- Purpose: Insert into table [dbo].[tblBulkPayments]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @BulkPaymentGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblBulkPayments] 
		(
			[BulkPaymentGuid]
		,	[Site]
		,	[Section]
		,	[PaymentType]
		,	[ForeignRate]
		,	[ForeignUnit]
		,	[RomanNumber]
		,	[DiscountRate]
		,	[PaymentDueDate]
		,	[TransactionDate]
		,	[Supplier]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		)
		VALUES
		(
			@BulkPaymentGuid
		,	@Site
		,	@Section
		,	@PaymentType
		,	@ForeignRate
		,	@ForeignUnit
		,	@RomanNumber
		,	@DiscountRate
		,	@PaymentDueDate
		,	@TransactionDate
		,	@Supplier
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblBulkPayments]           
		WHERE BulkPaymentGuid=@BulkPaymentGuid;
	
 
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
						+ 'Procedure Name: gsp_BulkPaymentsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
