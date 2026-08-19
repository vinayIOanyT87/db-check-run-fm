
CREATE PROCEDURE [rpt].[usp_DsProductFromGuid]
@ProductGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_DsProductFromGuid] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-05-08 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Products based on Site ID for reports.
	-- Notes:
	-- 1. @SiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		SELECT ProductID FROM tblProducts WHERE ProductGuid = @ProductGuid
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
						+ 'Procedure Name: [rpt].[usp_DsProductFromGuid]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END