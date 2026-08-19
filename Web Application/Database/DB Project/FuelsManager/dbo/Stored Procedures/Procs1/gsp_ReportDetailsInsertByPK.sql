CREATE PROCEDURE [dbo].[gsp_ReportDetailsInsertByPK]
(
		@ReportDetailGuid uniqueidentifier=NULL OUTPUT
	,	@ReportName nvarchar(60)=NULL
	,	@ReportDescription nvarchar(255)=NULL
	,	@ReportPath nvarchar(200)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@OrderNumber int=NULL
	,	@PrintOnlyFlag bit=NULL
	,	@PrimaryPrinterName nvarchar(100)=NULL
	,	@SecondaryPrinterName nvarchar(100)=NULL
	,	@PrintAtEndOfDay bit=NULL
	,	@PrintAtEndOfMonth bit=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@ReportGroupGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ReportDetailsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4122767 -05:00
	-- Purpose: Insert into table [dbo].[tblReportDetails]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ReportDetailGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblReportDetails] 
		(
			[ReportDetailGuid]
		,	[ReportName]
		,	[ReportDescription]
		,	[ReportPath]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[OrderNumber]
		,	[PrintOnlyFlag]
		,	[PrimaryPrinterName]
		,	[SecondaryPrinterName]
		,	[PrintAtEndOfDay]
		,	[PrintAtEndOfMonth]
		,	[SiteGuid]
		,	[ReportGroupGuid]
		)
		VALUES
		(
			@ReportDetailGuid
		,	@ReportName
		,	@ReportDescription
		,	@ReportPath
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@OrderNumber
		,	@PrintOnlyFlag
		,	@PrimaryPrinterName
		,	@SecondaryPrinterName
		,	@PrintAtEndOfDay
		,	@PrintAtEndOfMonth
		,	@SiteGuid
		,	@ReportGroupGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblReportDetails]           
		WHERE ReportDetailGuid=@ReportDetailGuid;
	
 
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
						+ 'Procedure Name: gsp_ReportDetailsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
