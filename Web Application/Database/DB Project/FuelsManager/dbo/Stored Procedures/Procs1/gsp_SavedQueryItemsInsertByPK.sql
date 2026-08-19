CREATE PROCEDURE [dbo].[gsp_SavedQueryItemsInsertByPK]
(
		@SavedQueryItemGuid uniqueidentifier=NULL OUTPUT
	,	@QueryIndex int=NULL
	,	@FieldID nvarchar(30)=NULL
	,	@ModifierID int=NULL
	,	@Value nvarchar(50)=NULL
	,	@JoinTypeID int=NULL
	,	@ItemSequence int=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SavedQueryItemsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4202767 -05:00
	-- Purpose: Insert into table [dbo].[tblSavedQueryItems]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SavedQueryItemGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSavedQueryItems] 
		(
			[SavedQueryItemGuid]
		,	[QueryIndex]
		,	[FieldID]
		,	[ModifierID]
		,	[Value]
		,	[JoinTypeID]
		,	[ItemSequence]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		)
		VALUES
		(
			@SavedQueryItemGuid
		,	@QueryIndex
		,	@FieldID
		,	@ModifierID
		,	@Value
		,	@JoinTypeID
		,	@ItemSequence
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSavedQueryItems]           
		WHERE SavedQueryItemGuid=@SavedQueryItemGuid;
	
 
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
						+ 'Procedure Name: gsp_SavedQueryItemsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
