CREATE PROCEDURE [dbo].[gsp_ExportRequestInsertByPK]
(
		@ExportRequestGuid uniqueidentifier=NULL OUTPUT
	,	@RequestID nvarchar(200)=NULL
	,	@InterfaceID nvarchar(200)=NULL
	,	@OwnerCode nvarchar(10)=NULL
	,	@UploadStagingFolder nvarchar(200)=NULL
	,	@ArchiveFolder nvarchar(200)=NULL
	,	@ConnectionInfo nvarchar(max)=NULL
	,	@SendingCompanyCode nvarchar(50)=NULL
	,	@SendViaFTP bit=NULL
	,	@SendSecure bit=NULL
	,	@CompanyNames nvarchar(max)=NULL
	,	@LatestRowVersion bigint=NULL
	,	@LastExportTime datetimeoffset(7)=NULL
	,	@ExportFrequency int=NULL
	,	@BaselineDate datetimeoffset(7)=NULL
	,	@ExcludeEmptyFiles bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
	,   @SendMethod INT=0
	,	@WebServicePluginType NVARCHAR(100)=NULL
	,	@WebServiceConfiguration NVARCHAR(512)=NULL
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ExportRequestInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2002767 -05:00
	-- Purpose: Insert into table [dbo].[tblExportRequest]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ExportRequestGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblExportRequest] 
		(
			[ExportRequestGuid]
		,	[RequestID]
		,	[InterfaceID]
		,	[OwnerCode]
		,	[UploadStagingFolder]
		,	[ArchiveFolder]
		,	[ConnectionInfo]
		,	[SendingCompanyCode]
		,	[SendViaFTP]
		,	[SendSecure]
		,	[CompanyNames]
		,	[LatestRowVersion]
		,	[LastExportTime]
		,	[ExportFrequency]
		,	[BaselineDate]
		,	[ExcludeEmptyFiles]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SendMethod]
		,	[WebServicePluginType]
		,	[WebServiceConfiguration]
		)
		VALUES
		(
			@ExportRequestGuid
		,	@RequestID
		,	@InterfaceID
		,	@OwnerCode
		,	@UploadStagingFolder
		,	@ArchiveFolder
		,	@ConnectionInfo
		,	@SendingCompanyCode
		,	@SendViaFTP
		,	@SendSecure
		,	@CompanyNames
		,	@LatestRowVersion
		,	@LastExportTime
		,	@ExportFrequency
		,	@BaselineDate
		,	@ExcludeEmptyFiles
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SendMethod
		,	@WebServicePluginType
		,	@WebServiceConfiguration
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblExportRequest]           
		WHERE ExportRequestGuid=@ExportRequestGuid;
	
 
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
						+ 'Procedure Name: gsp_ExportRequestInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
