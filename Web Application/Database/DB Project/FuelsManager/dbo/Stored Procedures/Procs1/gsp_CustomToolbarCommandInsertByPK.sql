CREATE PROCEDURE [dbo].[gsp_CustomToolbarCommandInsertByPK]
(
		@CustomToolbarCommandGuid uniqueidentifier=NULL OUTPUT
	,	@CustomToolbarGuid uniqueidentifier=NULL
	,	@CustomToolbarID nvarchar(50)=NULL
	,	@LookupCustomToolbarCommandTypeIndex int=NULL
	,	@TransactionAliasGuid uniqueidentifier=NULL
	,	@ID nvarchar(50)=NULL
	,	@ColumnOrder int=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_CustomToolbarCommandInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1232767 -05:00
	-- Purpose: Insert into table [dbo].[tblCustomToolbarCommand]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @CustomToolbarCommandGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblCustomToolbarCommand] 
		(
			[CustomToolbarCommandGuid]
		,	[CustomToolbarGuid]
		,	[CustomToolbarID]
		,	[LookupCustomToolbarCommandTypeIndex]
		,	[TransactionAliasGuid]
		,	[ID]
		,	[ColumnOrder]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@CustomToolbarCommandGuid
		,	@CustomToolbarGuid
		,	@CustomToolbarID
		,	@LookupCustomToolbarCommandTypeIndex
		,	@TransactionAliasGuid
		,	@ID
		,	@ColumnOrder
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblCustomToolbarCommand]           
		WHERE CustomToolbarCommandGuid=@CustomToolbarCommandGuid;
	
 
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
						+ 'Procedure Name: gsp_CustomToolbarCommandInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
