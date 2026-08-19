/*
	EXEC [map].[usp_GetFuelCardTypeToSiteBySiteGuid] '00000000-0000-0000-0000-000000000001'
*/

CREATE PROCEDURE [map].[usp_GetFuelCardTypeToSiteBySiteGuid]
(
	@AssignedToSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetFuelCardTypeToSiteBySiteGuid]
	-- Author: George Peters
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the FuelCardTypeToSite assignmentsto a given site/sitegroup. 	
	-- Notes:
	-- 1. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup from which the mapping tree search is to start. If not provided, the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	-- 2. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF (@AssignedToSiteGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END
	
		SELECT a.FuelCardTypeToSiteGuid MappingGuid, a.ApplicationStringGuid EntityRecordGuid, d.Id EntityId, a.SiteGuid AssignedToSiteGuid, b.Id AssignedToSiteId, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a._RowVersion, a.AssignedFromSiteGuid , c.ID AssignedFromSiteId 
		FROM map.tblEntityFuelCardTypeToSite a
		INNER JOIN tblSites b
		ON b.SiteGuid = a.SiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblApplicationString d
		ON d.ApplicationStringGuid = a.ApplicationStringGuid
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
						+ 'Procedure Name: map.usp_GetFuelCardTypeToSiteBySiteGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END