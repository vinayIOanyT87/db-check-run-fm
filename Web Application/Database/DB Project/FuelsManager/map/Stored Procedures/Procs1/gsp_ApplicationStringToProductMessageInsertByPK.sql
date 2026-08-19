CREATE PROCEDURE [map].[gsp_ApplicationStringToProductMessageInsertByPK]
(
		@ApplicationStringToProductMessageGuid uniqueidentifier=NULL OUTPUT
	,	@ApplicationStringGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
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
	-- Stored procedure: [map].[gsp_ApplicationStringToProductMessageInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6482767 -05:00
	-- Purpose: Insert into table [map].[tblApplicationStringToProductMessage]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ApplicationStringToProductMessageGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblApplicationStringToProductMessage] 
		(
			[ApplicationStringToProductMessageGuid]
		,	[ApplicationStringGuid]
		,	[ProductGuid]
		,	[Sequence]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@ApplicationStringToProductMessageGuid
		,	@ApplicationStringGuid
		,	@ProductGuid
		,	@Sequence
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblApplicationStringToProductMessage]           
		WHERE ApplicationStringToProductMessageGuid=@ApplicationStringToProductMessageGuid;
	
 
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
						+ 'Procedure Name: gsp_ApplicationStringToProductMessageInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
