CREATE PROCEDURE [dbo].[gsp_ReportApprovalsInsertByPK]
(
		@ReportApprovalGuid uniqueidentifier=NULL OUTPUT
	,	@EventIndex bigint=NULL
	,	@ReportName nvarchar(75)=NULL
	,	@ParameterValue nvarchar(50)=NULL
	,	@MaximumRowVersionNumber bigint=NULL
	,	@NextApprovalUser udtUserID=NULL
	,	@NextApprovalEmail nvarchar(50)=NULL
	,	@ApprovalName nvarchar(50)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
    ,   @CompanyManagerGuid uniqueidentifier=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupReportApprovalStateIndex int=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ReportApprovalsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4102767 -05:00
	-- Purpose: Insert into table [dbo].[tblReportApprovals]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ReportApprovalGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblReportApprovals] 
		(
			[ReportApprovalGuid]
		,	[ReportName]
		,	[ParameterValue]
		,	[MaximumRowVersionNumber]
		,	[NextApprovalUser]
		,	[NextApprovalEmail]
		,	[ApprovalName]
		,	[CreatedDate]
		,	[CreatedBy]
        ,   [CompanyManagerGuid]
		,	[SiteGuid]
		,	[LookupReportApprovalStateIndex]
		)
		VALUES
		(
			@ReportApprovalGuid
		,	@ReportName
		,	@ParameterValue
		,	@MaximumRowVersionNumber
		,	@NextApprovalUser
		,	@NextApprovalEmail
		,	@ApprovalName
		,	@CreatedDate
		,	@CreatedBy
        ,   @CompanyManagerGuid
		,	@SiteGuid
		,	@LookupReportApprovalStateIndex
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblReportApprovals]           
		WHERE ReportApprovalGuid=@ReportApprovalGuid;
	
 
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
						+ 'Procedure Name: gsp_ReportApprovalsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
