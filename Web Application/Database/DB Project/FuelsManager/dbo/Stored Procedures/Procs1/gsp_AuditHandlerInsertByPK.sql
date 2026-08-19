CREATE PROCEDURE [dbo].[gsp_AuditHandlerInsertByPK]
(
		@TableName nvarchar(100)=NULL OUTPUT
	,	@TypeID nvarchar(50)=NULL
	,	@ParentTypeID nvarchar(50)=NULL
	,	@IDQuery nvarchar(max)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AuditHandlerInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0822767 -05:00
	-- Purpose: Insert into table [dbo].[tblAuditHandler]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		SET @TableName=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAuditHandler] 
		(
			[TableName]
		,	[TypeID]
		,	[ParentTypeID]
		,	[IDQuery]
		,	[CreatedDate]
		)
		VALUES
		(
			@TableName
		,	@TypeID
		,	@ParentTypeID
		,	@IDQuery
		,	@CreatedDate
		)
 
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
						+ 'Procedure Name: gsp_AuditHandlerInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
