



/*
	EXEC [map].[usp_GetPersonnelToSiteByAssignedFromSiteGuid] '00000000-0000-0000-0000-000000000001'
*/

CREATE PROCEDURE [map].[usp_GetPersonnelToSiteByAssignedFromSiteGuid]
(
	@AssignedFromSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetPersonnelToSiteByAssignedFromSiteGuid]
	-- Author: Warren Gray
	-- Version/Date: 1.0.001 / 2013-12-20 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the PersonnelToSite assignments from a given sitegroup. 	
	-- Notes:
	-- 1. @AssignedFromSiteGuid: Guid of the AssignedFrom sitegroup for which the mapping tree search is to start. If not provided, the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF (@AssignedFromSiteGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END
	
		SELECT a.PersonnelToSiteGuid MappingGuid, a.PersonnelGuid EntityRecordGuid, d.PersonId EntityId, a.SiteGuid AssignedToSiteGuid, b.Id AssignedToSiteId, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a._RowVersion, a.AssignedFromSiteGuid , c.ID AssignedFromSiteId 
		FROM map.tblEntityPersonnelToSite a
		INNER JOIN tblSites b
		ON b.SiteGuid = a.SiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblPersonnel d
		ON d._MasterRecordGuid = a.PersonnelGuid
		AND d.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', a.PersonnelGuid, a.SiteGuid)
		WHERE a.AssignedFromSiteGuid = @AssignedFromSiteGuid
		AND a.SiteGuid <> @AssignedFromSiteGuid
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
						+ 'Procedure Name: map.usp_GetPersonnelToSiteByAssignedFromSiteGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
