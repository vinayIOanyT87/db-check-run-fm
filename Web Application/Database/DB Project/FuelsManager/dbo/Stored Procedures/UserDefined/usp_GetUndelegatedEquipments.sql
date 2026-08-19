
/*
       EXEC [dbo].[usp_GetUndelegatedEquipments] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
	   EXEC [dbo].[usp_GetUndelegatedEquipments] '00000000-0000-0000-0000-000000000001'
*/

CREATE PROCEDURE [dbo].[usp_GetUndelegatedEquipments]    
(
       @TargetSiteGuid uniqueidentifier,
       @ExcludeCompartments bit = 0
)
       AS
       BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored Procedure: [dbo].[usp_GetUndelegatedEquipments]    
       -- Author: Hansraj Bapoo
       -- Version/Date: 1.0.003 / Script Date: 1/30/2013 5:38:24 PM
       -- Purpose: Retrieve, for a given target site/sitegroup, the Master Equipment records for which no child record versions have been created for any other site/sitegroup.
       -- Notes:
       -- 1. @TargetSiteGuid: Limit results to master Equipments that are owned by this site/sitegroup only
       -- 2. @ExcludeCompartments: 0: Cover Equipments of all Equipment Types, including Compartments. 1: Exclude Compartments from the query.
       -- 2. The query returns only Master record equipments that are owned by the current Site and that have not been delegated to any other Site. 
	   -- 3. Since this query effectively excludes those Equipment records actively participating in Record Versioning, it implies that the AssignedFromSite can only be the owner site for the records in the resultset.
       ------------------------------------------------------------------------------------------------------
       BEGIN TRY   

			SELECT a.ID, a.EquipmentGuid, a._MasterRecordGuid, a.SiteGuid, a.SiteGuid [AssignedFromSiteGuid], b.ID [AssignedFromSiteId]  
			FROM tblEquipment a
			INNER JOIN tblSites b
			ON b.SiteGuid = a.SiteGuid
			WHERE a.SiteGuid = @TargetSiteGuid
			AND a.EquipmentGuid = a._MasterRecordGuid
			AND NOT EXISTS
			(
                SELECT * FROM tblEquipment c
			    WHERE c._MasterRecordGuid = a._MasterRecordGuid
			    AND c.EquipmentGuid <> c._MasterRecordGuid
            )
            AND 
		    (
			    (@ExcludeCompartments = 0)
			    OR
			    ((@ExcludeCompartments = 1) AND (a.ParentEquipmentGuid IS NULL))
		    )

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
                                         + 'Procedure Name: [dbo].usp_GetUndelegatedEquipments' + CHAR(13)+CHAR(10)                  
                                         + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
              RAISERROR(@_ErrMessage,18,1);      
       END CATCH    
       
END