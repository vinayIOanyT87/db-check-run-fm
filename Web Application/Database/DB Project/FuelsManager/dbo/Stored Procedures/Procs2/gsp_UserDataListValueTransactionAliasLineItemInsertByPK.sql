CREATE PROCEDURE [dbo].[gsp_UserDataListValueTransactionAliasLineItemInsertByPK]
(
		@UserDataListValueTransactionAliasLineItemGuid uniqueidentifier=NULL OUTPUT
	,	@UserDataFieldTransactionAliasLineItemGuid uniqueidentifier=NULL
	,	@Value nvarchar(120)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_UserDataListValueTransactionAliasLineItemInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6202767 -05:00
	-- Purpose: Insert into table [dbo].[tblUserDataListValueTransactionAliasLineItem]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @UserDataListValueTransactionAliasLineItemGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblUserDataListValueTransactionAliasLineItem] 
		(
			[UserDataListValueTransactionAliasLineItemGuid]
		,	[UserDataFieldTransactionAliasLineItemGuid]
		,	[Value]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@UserDataListValueTransactionAliasLineItemGuid
		,	@UserDataFieldTransactionAliasLineItemGuid
		,	@Value
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblUserDataListValueTransactionAliasLineItem]           
		WHERE UserDataListValueTransactionAliasLineItemGuid=@UserDataListValueTransactionAliasLineItemGuid;
	
 
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
						+ 'Procedure Name: gsp_UserDataListValueTransactionAliasLineItemInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
