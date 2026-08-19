CREATE PROCEDURE [dbo].[gsp_ExportResultsInsertByPK]
(
		@ExportResultGuid uniqueidentifier=NULL OUTPUT
	,	@InterfaceName nvarchar(150)=NULL
	,	@TransVersion bigint=NULL
	,	@FailedCount int=NULL
	,	@SuccessCount int=NULL
	,	@TransDateTime datetimeoffset(7)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@BatchID nvarchar(64)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupExportResultTypeIndex int=NULL
	,	@ArchiveFileName nvarchar(150)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ExportResultsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2192767 -05:00
	-- Purpose: Insert into table [dbo].[tblExportResults]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ExportResultGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblExportResults] 
		(
			[ExportResultGuid]
		,	[InterfaceName]
		,	[TransVersion]
		,	[FailedCount]
		,	[SuccessCount]
		,	[TransDateTime]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[BatchID]
		,	[SiteGuid]
		,	[LookupExportResultTypeIndex]
		,	[ArchiveFileName]
		)
		VALUES
		(
			@ExportResultGuid
		,	@InterfaceName
		,	@TransVersion
		,	@FailedCount
		,	@SuccessCount
		,	@TransDateTime
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@BatchID
		,	@SiteGuid
		,	@LookupExportResultTypeIndex
		,	@ArchiveFileName
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblExportResults]           
		WHERE ExportResultGuid=@ExportResultGuid;
	
 
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
						+ 'Procedure Name: gsp_ExportResultsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
