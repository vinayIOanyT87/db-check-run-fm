
/*
       EXEC [dbo].[usp_GetUndelegatedTransactionAliases] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
*/

CREATE PROCEDURE [dbo].[usp_GetUndelegatedTransactionAliases]    
(
       @TargetSiteGuid uniqueidentifier
)
       AS
       BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored Procedure: [dbo].[usp_GetUndelegatedTransactionAliases]    
       -- Author: Hansraj Bapoo
       -- Version/Date: 1.0.003 / Script Date: 1/30/2013 5:38:24 PM
       -- Purpose: Retrieve, for a given target site/sitegroup, the Master TransactionAlias records for which no child record versions have been created for any other site/sitegroup.
       -- Notes:
       -- 1. @TargetSiteGuid: Limit results to master TransactionAliases that are owned by this site/sitegroup only
       -- 2. The query returns only Master record TransactionAliases that are owned by the current Site and that have not been delegated to any other Site. 
	   -- 3. Since this query effectively excludes those TransactionAlias records actively participating in Record Versioning, it implies that the AssignedFromSite can only be the owner site for the records in the resultset.
       ------------------------------------------------------------------------------------------------------
       BEGIN TRY   

			SELECT a.AliasName Id, a.TransactionAliasGuid, a._MasterRecordGuid, a.SiteGuid, a.SiteGuid [AssignedFromSiteGuid], b.ID [AssignedFromSiteId]   
			FROM tblTransactionAliases a
			INNER JOIN tblSites b
			ON b.SiteGuid = a.SiteGuid
			WHERE a.SiteGuid = @TargetSiteGuid
			AND a.TransactionAliasGuid = a._MasterRecordGuid
			AND NOT EXISTS
			(SELECT * FROM tblTransactionAliases c
			WHERE c._MasterRecordGuid = a._MasterRecordGuid
			AND c.TransactionAliasGuid <> c._MasterRecordGuid)

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
                                         + 'Procedure Name: [dbo].usp_GetUndelegatedTransactionAliases' + CHAR(13)+CHAR(10)                  
                                         + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
              RAISERROR(@_ErrMessage,18,1);      
       END CATCH    
       
END