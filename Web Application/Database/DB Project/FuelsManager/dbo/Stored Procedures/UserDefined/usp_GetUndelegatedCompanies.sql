/*
       EXEC [dbo].[usp_GetUndelegatedCompanies] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
*/

CREATE PROCEDURE [dbo].[usp_GetUndelegatedCompanies]    
(
       @TargetSiteGuid uniqueidentifier
)
       AS
       BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored Procedure: [dbo].[usp_GetUndelegatedCompanies]    
       -- Author: Hansraj Bapoo
       -- Version/Date: 1.0.003 / Script Date: 1/30/2013 5:38:24 PM
       -- Purpose: Retrieve, for a given target site/sitegroup, the Master Company records for which no child record versions have been created for any other site/sitegroup.
       -- Notes:
       -- 1. @TargetSiteGuid: Limit results to master Companys that are owned by this site/sitegroup only
       -- 2. The query returns only Master record Companies that are owned by the current Site and that have not been delegated to any other Site. 
	   -- 3. Since this query effectively excludes those Company records actively participating in Record Versioning, it implies that the AssignedFromSite can only be the owner site for the records in the resultset.
       ------------------------------------------------------------------------------------------------------
       BEGIN TRY   

			SELECT a.ID, a.CompanyGuid, a._MasterRecordGuid, a.SiteGuid, a.SiteGuid [AssignedFromSiteGuid], b.ID [AssignedFromSiteId]   
			FROM tblCompanies a
			INNER JOIN tblSites b
			ON b.SiteGuid = a.SiteGuid
			WHERE a.SiteGuid = @TargetSiteGuid
			AND a.CompanyGuid = a._MasterRecordGuid
			AND NOT EXISTS
			(SELECT * FROM tblCompanies c
			WHERE c._MasterRecordGuid = a._MasterRecordGuid
			AND c.CompanyGuid <> c._MasterRecordGuid)

       END TRY
       BEGIN CATCH  
              DECLARE       @_ErrMessage NVARCHAR(2048)      
                           , @_ErrNumber INT           
                           , @_ErrProcName NVARCHAR(126)           
                           , @_ErrLineNumber INT;            
              SET @_ErrMessage = ERROR_MESSAGE();        
              SET @_ErrNumber = ERROR_NUMBER();        
              SET @_ErrProcName= ERROR_PROCEDURE();        
              SET @_ErrLineNumber = ERROR_LINE();            
              SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
                                         + 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
                                         + 'Procedure Name: [dbo].usp_GetUndelegatedCompanies' + CHAR(13)+CHAR(10)                  
                                         + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
              RAISERROR(@_ErrMessage,18,1);      
       END CATCH    
       
END