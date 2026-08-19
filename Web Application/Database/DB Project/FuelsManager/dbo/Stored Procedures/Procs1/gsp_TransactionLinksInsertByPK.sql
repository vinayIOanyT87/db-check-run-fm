CREATE PROCEDURE [dbo].[gsp_TransactionLinksInsertByPK]
(
		@TransactionLinkGuid uniqueidentifier=NULL OUTPUT
	,	@OriginalTransID nvarchar(64)=NULL
	,	@LinkedTransID nvarchar(64)=NULL
	,	@Level int=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LinkedTransactionLineItemGuid uniqueidentifier=NULL
	,	@TransactionLineItemGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionLinksInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5462767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionLinks]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionLinkGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionLinks] 
		(
			[TransactionLinkGuid]
		,	[OriginalTransID]
		,	[LinkedTransID]
		,	[Level]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[SiteGuid]
		,	[LinkedTransactionLineItemGuid]
		,	[TransactionLineItemGuid]
		)
		VALUES
		(
			@TransactionLinkGuid
		,	@OriginalTransID
		,	@LinkedTransID
		,	@Level
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@SiteGuid
		,	@LinkedTransactionLineItemGuid
		,	@TransactionLineItemGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionLinks]           
		WHERE TransactionLinkGuid=@TransactionLinkGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionLinksInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
