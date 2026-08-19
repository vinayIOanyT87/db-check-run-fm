CREATE PROCEDURE [dbo].[usp_EnumerateReportApproval] 
(
	@SiteGuid UniqueIdentifier,
	@ReportName NVarChar(75),
	@ParameterValue NVarChar(50),
    @CompanyManagerGuid UniqueIdentifier
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_EnumerateReportApproval] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Monthly Journal Report
	-- Notes:
	-- 1. @SiteGuid: Guid of the leaf site
	-- 2. @ReportName: The identifying associate report name
	-- 3. @ParameterValue: parameter value
	-- 4. @CompanyManagerGuid: Guid of managing company for site and products
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT * FROM tblReportApprovals
		WHERE SiteGuid = @SiteGuid
		AND ParameterValue = @ParameterValue
		AND ReportName = @ReportName
        AND CompanyManagerGuid = @CompanyManagerGuid
		ORDER BY CreatedDate

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
						+ 'Procedure Name: [dbo].[usp_EnumerateReportApproval] ' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END     