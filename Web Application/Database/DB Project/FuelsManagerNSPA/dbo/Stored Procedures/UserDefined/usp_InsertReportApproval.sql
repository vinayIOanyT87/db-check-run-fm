CREATE PROCEDURE [dbo].[usp_InsertReportApproval] 
(
	@SiteGuid UniqueIdentifier,
	@ReportName NVarChar(75),
	@ParameterValue NVarChar(50),
	@MaximumRowVersionNumber BigInt,
	@LookupReportApprovalStateIndex Int,
	@NextApprovalUser NVarChar(50),
	@NextApprovalEmail NVarChar(50),
	@CreatedDate DateTimeOffset,
	@CreatedBy NVarChar(100),
	@ApprovalName NVarChar(50),
	@ReportApprovalGuid UniqueIdentifier,
    @CompanyManagerGuid UniqueIdentifier
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_InsertReportApproval] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Monthly Journal Report
	-- Notes:
	-- 1. @SiteGuid: Guid of the leaf site
	-- 2. @ReportName: The identifying associate report name
	-- 3. @ParameterValue: parameter value
	-- 4. @MaximumRowVersionNumber: checksum using against the list of transactions createing the report using rowversion
	-- 5. @LookupReportApprovalStateIndex: approval state identifier
	-- 6. @NextApprovalUser: Next approver
	-- 7. @NextApprovalEmail: Email of next approver
	-- 8. @CreatedDate: Date the record was created
	-- 9. @CreatedBy: The user who created the record
	-- 10. @ApprovalName: Approver name
	-- 11. @ReportApprovalGuid: Identifies the specific report approval by guid
	-- 12. @CompanyManagerGuid: Guid of managing company for site and products
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		INSERT INTO tblReportApprovals (
        SiteGuid, ReportName, ParameterValue, MaximumRowVersionNumber, LookupReportApprovalStateIndex, 
		NextApprovalUser, NextApprovalEmail, CreatedDate, CreatedBy, ApprovalName, ReportApprovalGuid, CompanyManagerGuid
		) VALUES (
        @SiteGuid, @ReportName, @ParameterValue, @MaximumRowVersionNumber, @LookupReportApprovalStateIndex, 
		@NextApprovalUser, @NextApprovalEmail, @CreatedDate, @CreatedBy, @ApprovalName, @ReportApprovalGuid, @CompanyManagerGuid
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
						+ 'Procedure Name: [dbo].[usp_InsertReportApproval] ' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END     