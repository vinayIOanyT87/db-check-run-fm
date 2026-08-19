CREATE PROCEDURE [dbo].[gsp_ListViewsInsertByPK]
(
		@ListViewGuid uniqueidentifier=NULL OUTPUT
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@ID nvarchar(50)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupListViewTypeIndex int=NULL
	,	@LookupListViewStandardTypeIndex int=NULL
	,	@LedgerAggregateColumnGuid uniqueidentifier=NULL
	,	@TransactionAliasGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ListViewsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2632767 -05:00
	-- Purpose: Insert into table [dbo].[tblListViews]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ListViewGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblListViews] 
		(
			[ListViewGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[ID]
		,	[SiteGuid]
		,	[LookupListViewTypeIndex]
		,	[LookupListViewStandardTypeIndex]
		,	[LedgerAggregateColumnGuid]
		,	[TransactionAliasGuid]
		)
		VALUES
		(
			@ListViewGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@ID
		,	@SiteGuid
		,	@LookupListViewTypeIndex
		,	@LookupListViewStandardTypeIndex
		,	@LedgerAggregateColumnGuid
		,	@TransactionAliasGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblListViews]           
		WHERE ListViewGuid=@ListViewGuid;
	
 
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
						+ 'Procedure Name: gsp_ListViewsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
