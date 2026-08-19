CREATE PROCEDURE [dbo].[gsp_SRMAdaptorFilterInsertByPK]
(
		@SRMAdaptorFilterGuid uniqueidentifier=NULL OUTPUT
	,	@SRMAdaptorGuid uniqueidentifier=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@SRMAdaptorFilterTypeCode tinyint=NULL
	,	@FilterValue nvarchar(100)=NULL
	,	@IsEnabled bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SRMAdaptorFilterInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4672767 -05:00
	-- Purpose: Insert into table [dbo].[tblSRMAdaptorFilter]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SRMAdaptorFilterGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSRMAdaptorFilter] 
		(
			[SRMAdaptorFilterGuid]
		,	[SRMAdaptorGuid]
		,	[SiteGuid]
		,	[SRMAdaptorFilterTypeCode]
		,	[FilterValue]
		,	[IsEnabled]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@SRMAdaptorFilterGuid
		,	@SRMAdaptorGuid
		,	@SiteGuid
		,	@SRMAdaptorFilterTypeCode
		,	@FilterValue
		,	@IsEnabled
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSRMAdaptorFilter]           
		WHERE SRMAdaptorFilterGuid=@SRMAdaptorFilterGuid;
	
 
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
						+ 'Procedure Name: gsp_SRMAdaptorFilterInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
