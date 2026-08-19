CREATE PROCEDURE [dbo].[gsp_UserDataFieldProductInsertByPK]
(
		@UserDataFieldProductGuid uniqueidentifier=NULL OUTPUT
	,	@TransactionAliasGuid uniqueidentifier=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@Number tinyint=NULL
	,	@DisplayOrder int=NULL
	,	@DisplayName nvarchar(30)=NULL
	,	@LookupUserDataTypeIndex int=NULL
	,	@Required bit=NULL
	,	@UserGroupGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@DispatchField bit=NULL
	,	@ClearOnNew bit=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_UserDataFieldProductInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5992767 -05:00
	-- Purpose: Insert into table [dbo].[tblUserDataFieldProduct]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @UserDataFieldProductGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblUserDataFieldProduct] 
		(
			[UserDataFieldProductGuid]
		,	[TransactionAliasGuid]
		,	[SiteGuid]
		,	[Number]
		,	[DisplayOrder]
		,	[DisplayName]
		,	[LookupUserDataTypeIndex]
		,	[Required]
		,	[UserGroupGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[DispatchField]
		,	[ClearOnNew]
		)
		VALUES
		(
			@UserDataFieldProductGuid
		,	@TransactionAliasGuid
		,	@SiteGuid
		,	@Number
		,	@DisplayOrder
		,	@DisplayName
		,	@LookupUserDataTypeIndex
		,	@Required
		,	@UserGroupGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@DispatchField
		,	@ClearOnNew
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblUserDataFieldProduct]           
		WHERE UserDataFieldProductGuid=@UserDataFieldProductGuid;
	
 
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
						+ 'Procedure Name: gsp_UserDataFieldProductInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
