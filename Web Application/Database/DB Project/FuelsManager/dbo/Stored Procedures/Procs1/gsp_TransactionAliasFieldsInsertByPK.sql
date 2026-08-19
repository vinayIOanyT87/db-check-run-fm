CREATE PROCEDURE [dbo].[gsp_TransactionAliasFieldsInsertByPK]
(
		@TransactionAliasFieldGuid uniqueidentifier=NULL OUTPUT
	,	@AliasID int=NULL
	,	@DbName nvarchar(50)=NULL
	,	@DisplayOrder int=NULL
	,	@DisplayName nvarchar(50)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@Required bit=NULL
	,	@Virtual bit=NULL
	,	@LookupTransactionFieldTypeIndex int=NULL
	,	@TransactionAliasGuid uniqueidentifier=NULL
	,	@UserGroupGuid uniqueidentifier=NULL
	,	@DispatchField bit=NULL
	,	@ClearOnNew bit=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionAliasFieldsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5242767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionAliasFields]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionAliasFieldGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionAliasFields] 
		(
			[TransactionAliasFieldGuid]
		,	[AliasID]
		,	[DbName]
		,	[DisplayOrder]
		,	[DisplayName]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[Required]
		,	[Virtual]
		,	[LookupTransactionFieldTypeIndex]
		,	[TransactionAliasGuid]
		,	[UserGroupGuid]
		,	[DispatchField]
		,	[ClearOnNew]
		)
		VALUES
		(
			@TransactionAliasFieldGuid
		,	@AliasID
		,	@DbName
		,	@DisplayOrder
		,	@DisplayName
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@Required
		,	@Virtual
		,	@LookupTransactionFieldTypeIndex
		,	@TransactionAliasGuid
		,	@UserGroupGuid
		,	@DispatchField
		,	@ClearOnNew
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionAliasFields]           
		WHERE TransactionAliasFieldGuid=@TransactionAliasFieldGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionAliasFieldsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
