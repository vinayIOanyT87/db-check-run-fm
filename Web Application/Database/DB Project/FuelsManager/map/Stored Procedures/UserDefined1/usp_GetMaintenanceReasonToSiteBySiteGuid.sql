/*
	EXEC [map].[usp_GetMaintenanceReasonToSiteBySiteGuid] '00000000-0000-0000-0000-000000000001'
*/

CREATE PROCEDURE [map].[usp_GetMaintenanceReasonToSiteBySiteGuid]
(
	@AssignedToSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetMaintenanceReasonToSiteBySiteGuid]
	-- Author: Richard R. Panachida
	-- Version/Date: 1.0.003 / 2015-02-24
	-- Purpose: Retrieves the MaintenanceReasonToSite assignments to a given site/sitegroup. 	
	-- Notes:
	-- 1. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup from which the mapping tree search is to start. If not provided, 
	--    the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	-- 2. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF (@AssignedToSiteGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END
	
		SELECT a.MaintenanceReasonToSiteGuid MappingGuid, 
			   a.MaintenanceReasonGuid EntityRecordGuid, 
			   d.ID EntityId, 
			   a.SiteGuid AssignedToSiteGuid, 
			   b.ID AssignedToSiteId, 
			   a.CreatedDate, 
			   a.CreatedBy, 
			   a.UpdatedDate, 
			   a.UpdatedBy, 
			   a._RowVersion, 
			   a.AssignedFromSiteGuid , 
			   c.ID AssignedFromSiteId 
		FROM map.tblEntityMaintenanceReasonToSite a INNER JOIN tblSites b ON b.SiteGuid = a.SiteGuid
		     INNER JOIN tblSites c ON c.SiteGuid = a.AssignedFromSiteGuid
		     INNER JOIN tblMaintenanceReasons d ON d.MaintenanceReasonGuid = a.MaintenanceReasonGuid
		WHERE a.SiteGuid = @AssignedToSiteGuid
		ORDER BY EntityId

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
						+ 'Procedure Name: map.usp_GetMaintenanceReasonToSiteBySiteGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
