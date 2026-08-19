CREATE PROCEDURE [dbo].[gsp_UserDataFieldEquipmentInsertByPK]
(
		@UserDataFieldEquipmentGuid uniqueidentifier=NULL OUTPUT
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
	-- Stored procedure: [dbo].[gsp_UserDataFieldEquipmentInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5922767 -05:00
	-- Purpose: Insert into table [dbo].[tblUserDataFieldEquipment]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @UserDataFieldEquipmentGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblUserDataFieldEquipment] 
		(
			[UserDataFieldEquipmentGuid]
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
			@UserDataFieldEquipmentGuid
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
		FROM [dbo].[tblUserDataFieldEquipment]           
		WHERE UserDataFieldEquipmentGuid=@UserDataFieldEquipmentGuid;
	
 
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
						+ 'Procedure Name: gsp_UserDataFieldEquipmentInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
