CREATE PROCEDURE [dbo].[gsp_SRMAdaptorInsertByPK]
(
		@SRMAdaptorGuid uniqueidentifier=NULL OUTPUT
	,	@SRMAdaptorName nvarchar(100)=NULL
	,	@IsEnabled bit=NULL
	,	@CustomWebApplicationPage nvarchar(100)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@DuplicateCount bigint=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SRMAdaptorInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4662767 -05:00
	-- Purpose: Insert into table [dbo].[tblSRMAdaptor]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SRMAdaptorGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSRMAdaptor] 
		(
			[SRMAdaptorGuid]
		,	[SRMAdaptorName]
		,	[IsEnabled]
		,	[CustomWebApplicationPage]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[DuplicateCount]
		)
		VALUES
		(
			@SRMAdaptorGuid
		,	@SRMAdaptorName
		,	@IsEnabled
		,	@CustomWebApplicationPage
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@DuplicateCount
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSRMAdaptor]           
		WHERE SRMAdaptorGuid=@SRMAdaptorGuid;
	
 
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
						+ 'Procedure Name: gsp_SRMAdaptorInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
