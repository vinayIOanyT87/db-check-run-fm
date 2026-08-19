CREATE PROCEDURE [dbo].[gsp_ListViewFieldsInsertByPK]
(
		@ListViewFieldGuid uniqueidentifier=NULL OUTPUT
	,	@ColumnOrder int=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@ListViewID nvarchar(50)=NULL
	,	@LookupListViewFieldTypeIndex int=NULL
	,	@LookupStandardFieldTypeIndex int=NULL
	,	@ListViewGuid uniqueidentifier=NULL
	,	@TransactionAliasGuid uniqueidentifier=NULL
	,	@TransactionAliasFieldGuid uniqueidentifier=NULL
	,	@UserDataFieldTransactionAliasGuid uniqueidentifier=NULL
	,	@UserDataFieldTransactionAliasLineItemGuid uniqueidentifier=NULL
	,	@LedgerAggregateColumnGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ListViewFieldsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2602767 -05:00
	-- Purpose: Insert into table [dbo].[tblListViewFields]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ListViewFieldGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblListViewFields] 
		(
			[ListViewFieldGuid]
		,	[ColumnOrder]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[ListViewID]
		,	[LookupListViewFieldTypeIndex]
		,	[LookupStandardFieldTypeIndex]
		,	[ListViewGuid]
		,	[TransactionAliasGuid]
		,	[TransactionAliasFieldGuid]
		,	[UserDataFieldTransactionAliasGuid]
		,	[UserDataFieldTransactionAliasLineItemGuid]
		,	[LedgerAggregateColumnGuid]
		)
		VALUES
		(
			@ListViewFieldGuid
		,	@ColumnOrder
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@ListViewID
		,	@LookupListViewFieldTypeIndex
		,	@LookupStandardFieldTypeIndex
		,	@ListViewGuid
		,	@TransactionAliasGuid
		,	@TransactionAliasFieldGuid
		,	@UserDataFieldTransactionAliasGuid
		,	@UserDataFieldTransactionAliasLineItemGuid
		,	@LedgerAggregateColumnGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblListViewFields]           
		WHERE ListViewFieldGuid=@ListViewFieldGuid;
	
 
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
						+ 'Procedure Name: gsp_ListViewFieldsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
