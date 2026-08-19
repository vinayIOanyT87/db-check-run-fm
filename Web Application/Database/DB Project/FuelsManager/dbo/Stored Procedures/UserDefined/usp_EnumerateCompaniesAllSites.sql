------------------------------------------------------------------------------------------------------
-- Stored Procedure: [dbo].[usp_EnumerateCompaniesAllSites] 
-- Author: Richard R. Panachida
-- Version/Date: 1.0.000 / 2022-04-06
-- Purpose: Enumerate the Company records (minimum number of fields) that have been assigned to all Site/SiteGroup.
--
-- Testing:
-- EXEC [dbo].[usp_EnumerateCompaniesAllSites] 
------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_EnumerateCompaniesAllSites]
AS
BEGIN
	
	BEGIN TRY	
		SELECT C.ID
			 , C.CompanyGuid
			 , C._MasterRecordGuid
			 , ECTS.SiteGuid    
		FROM tblCompanies C 			
			INNER JOIN map.tblEntityCompanyToSite ECTS ON ECTS.CompanyGuid = C._MasterRecordGuid AND ECTS.SiteGuid = C.SiteGuid
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
						+ 'Procedure Name: [dbo].usp_EnumerateCompaniesAllSites' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END