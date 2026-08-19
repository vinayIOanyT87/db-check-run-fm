CREATE PROCEDURE [dbo].[gsp_TransactionTransportLineItemsInsertByPK]
(
		@TransactionTransportLineItemGuid uniqueidentifier=NULL OUTPUT
	,	@TransportOrderNumber nvarchar(50)=NULL
	,	@TransVersion bigint=NULL
	,	@LocationName nvarchar(30)=NULL
	,	@Address1 nvarchar(60)=NULL
	,	@Address2 nvarchar(60)=NULL
	,	@City nvarchar(60)=NULL
	,	@State nvarchar(20)=NULL
	,	@Zip nvarchar(11)=NULL
	,	@POCName nvarchar(50)=NULL
	,	@POCPhone nvarchar(20)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@TransactionGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionTransportLineItemsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5822767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionTransportLineItems]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionTransportLineItemGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionTransportLineItems] 
		(
			[TransactionTransportLineItemGuid]
		,	[TransportOrderNumber]
		,	[TransVersion]
		,	[LocationName]
		,	[Address1]
		,	[Address2]
		,	[City]
		,	[State]
		,	[Zip]
		,	[POCName]
		,	[POCPhone]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[TransactionGuid]
		)
		VALUES
		(
			@TransactionTransportLineItemGuid
		,	@TransportOrderNumber
		,	@TransVersion
		,	@LocationName
		,	@Address1
		,	@Address2
		,	@City
		,	@State
		,	@Zip
		,	@POCName
		,	@POCPhone
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@TransactionGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionTransportLineItems]           
		WHERE TransactionTransportLineItemGuid=@TransactionTransportLineItemGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionTransportLineItemsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
