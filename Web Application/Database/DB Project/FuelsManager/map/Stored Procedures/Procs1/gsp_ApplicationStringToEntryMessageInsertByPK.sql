CREATE PROCEDURE [map].[gsp_ApplicationStringToEntryMessageInsertByPK]
(
		@ApplicationStringToEntryMessageGuid uniqueidentifier=NULL OUTPUT
	,	@ApplicationStringGuid uniqueidentifier=NULL
	,	@ProductGroupApplicationStringGuid uniqueidentifier=NULL
	,	@Sequence int=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_ApplicationStringToEntryMessageInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6382767 -05:00
	-- Purpose: Insert into table [map].[tblApplicationStringToEntryMessage]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ApplicationStringToEntryMessageGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblApplicationStringToEntryMessage] 
		(
			[ApplicationStringToEntryMessageGuid]
		,	[ApplicationStringGuid]
		,	[ProductGroupApplicationStringGuid]
		,	[Sequence]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@ApplicationStringToEntryMessageGuid
		,	@ApplicationStringGuid
		,	@ProductGroupApplicationStringGuid
		,	@Sequence
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblApplicationStringToEntryMessage]           
		WHERE ApplicationStringToEntryMessageGuid=@ApplicationStringToEntryMessageGuid;
	
 
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
						+ 'Procedure Name: gsp_ApplicationStringToEntryMessageInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
