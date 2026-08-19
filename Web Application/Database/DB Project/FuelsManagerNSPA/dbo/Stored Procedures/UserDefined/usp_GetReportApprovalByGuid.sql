CREATE PROCEDURE [dbo].[usp_GetReportApprovalByGuid] 
(
	@ReportApprovalGuid UNIQUEIDENTIFIER,
	@InTransaction BIT
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetReportApprovalByGuid] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Monthly Journal Report
	-- Notes:
	-- 1. @ReportApprovalGuid: Identifies the specific report approval by guid
	-- 2. @InTransaction: Identifies whether this is being used in a transaction and needs to be locked
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

	IF(@InTransaction = 1)
		BEGIN
			SELECT * FROM tblReportApprovals WITH(UPDLOCK) WHERE ReportApprovalGuid = @ReportApprovalGuid
		END
	ELSE
		BEGIN
			SELECT * FROM tblReportApprovals WHERE ReportApprovalGuid = @ReportApprovalGuid
		END

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
						+ 'Procedure Name: [dbo].[usp_GetReportApprovalByGuid] ' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END     