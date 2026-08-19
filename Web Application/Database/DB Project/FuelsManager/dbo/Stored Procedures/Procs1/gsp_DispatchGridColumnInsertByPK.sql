CREATE PROCEDURE [dbo].[gsp_DispatchGridColumnInsertByPK]
(
		@DispatchGridColumnGuid uniqueidentifier=NULL OUTPUT
	,	@DispatchGridGuid uniqueidentifier=NULL
	,	@DispatchGridID nvarchar(50)=NULL
	,	@LookupDispatchGridColumnTypeIndex int=NULL
	,	@ID nvarchar(50)=NULL
	,	@ColumnOrder int=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UserDataFieldTransactionAliasGuid uniqueidentifier=NULL
	,	@UserDataFieldTransactionAliasLineItemGuid uniqueidentifier=NULL
	,	@AliasName nvarchar(50)=NULL
	,	@UserDataNumber int=NULL
	,	@UserGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_DispatchGridColumnInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1372767 -05:00
	-- Purpose: Insert into table [dbo].[tblDispatchGridColumn]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @DispatchGridColumnGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblDispatchGridColumn] 
		(
			[DispatchGridColumnGuid]
		,	[DispatchGridGuid]
		,	[DispatchGridID]
		,	[LookupDispatchGridColumnTypeIndex]
		,	[ID]
		,	[ColumnOrder]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[UserDataFieldTransactionAliasGuid]
		,	[UserDataFieldTransactionAliasLineItemGuid]
		,	[AliasName]
		,	[UserDataNumber]
		,	[UserGuid]
		)
		VALUES
		(
			@DispatchGridColumnGuid
		,	@DispatchGridGuid
		,	@DispatchGridID
		,	@LookupDispatchGridColumnTypeIndex
		,	@ID
		,	@ColumnOrder
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@UserDataFieldTransactionAliasGuid
		,	@UserDataFieldTransactionAliasLineItemGuid
		,	@AliasName
		,	@UserDataNumber
		,	@UserGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblDispatchGridColumn]           
		WHERE DispatchGridColumnGuid=@DispatchGridColumnGuid;
	
 
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
						+ 'Procedure Name: gsp_DispatchGridColumnInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
