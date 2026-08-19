------------------------------------------------------------------------------------------------------
-- Stored Procedure: [dbo].[usp_EnumerateProductsAllSites] 
-- Author: Richard R. Panachida
-- Version/Date: 1.0.000 / 2022-04-07 
-- Purpose: Retrieve all Product records all Sites/SiteGroups.
-- 
-- Testing:
-- EXEC [dbo].[usp_EnumerateProductsAllSites]
------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_EnumerateProductsAllSites]
AS
BEGIN
	BEGIN TRY	

		SELECT P.ProductID
			   , P.ProductCode
			   , P.ProductGuid
			   , P._MasterRecordGuid
			   , EPTS.SiteGuid
		FROM tblProducts P
			INNER JOIN map.tblEntityProductToSite EPTS ON EPTS.ProductGuid = P._MasterRecordGuid AND EPTS.SiteGuid = P.SiteGuid

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
						+ 'Procedure Name: [dbo].usp_EnumerateProductsAllSites' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
