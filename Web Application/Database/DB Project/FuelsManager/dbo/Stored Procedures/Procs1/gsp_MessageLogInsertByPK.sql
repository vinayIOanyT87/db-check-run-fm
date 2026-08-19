CREATE PROCEDURE [dbo].[gsp_MessageLogInsertByPK]
(
		@MessageLogGuid uniqueidentifier=NULL OUTPUT
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CompanyGuid uniqueidentifier=NULL
	,	@MessageGuid uniqueidentifier=NULL
	,	@PersonnelGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_MessageLogInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2742767 -05:00
	-- Purpose: Insert into table [dbo].[tblMessageLog]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @MessageLogGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblMessageLog] 
		(
			[MessageLogGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[CompanyGuid]
		,	[MessageGuid]
		,	[PersonnelGuid]
		)
		VALUES
		(
			@MessageLogGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@CompanyGuid
		,	@MessageGuid
		,	@PersonnelGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblMessageLog]           
		WHERE MessageLogGuid=@MessageLogGuid;
	
 
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
						+ 'Procedure Name: gsp_MessageLogInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
