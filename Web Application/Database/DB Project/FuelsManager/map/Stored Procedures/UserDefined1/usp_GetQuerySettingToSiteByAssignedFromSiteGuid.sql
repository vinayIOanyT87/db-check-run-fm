

/*
	EXEC [map].[usp_GetQuerySettingToSiteByAssignedFromSiteGuid] '00000000-0000-0000-0000-000000000001'
*/

CREATE PROCEDURE [map].[usp_GetQuerySettingToSiteByAssignedFromSiteGuid]
(
	@AssignedFromSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetQuerySettingToSiteByAssignedFromSiteGuid]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the QuerySettingToSite assignments from a given sitegroup. 	
	-- Notes:
	-- 1. @AssignedFromSiteGuid: Guid of the AssignedFrom sitegroup from which the mapping is to be queried.	
	-- 2. QuerySettingToSite mappings are not applied/managed on individual QuerySetting records, but across all QuerySetting records for a given sitegroup (@EntityRecordGuid)
	-- 3. This operation helps verify if a QuerySetting mapping to a given sitegroup is valid. If a QuerySetting mapping, for a given EntityRecordGuid/OwnerSiteGuid (e.g. SGZ) 
	--    has already been assigned from a given sitegroup (e.g. SGZ), then assigning a QuerySetting mapping to the same sitegroup from a higher level sitegroup (e.g. SGY) will 
	--    now provide more than one EntityRecordGuid/OwnerSiteGuid that can be assigned from the lower level sitegroup (SGZ), thus violating the principle of the Entity-to-site 
	--    mapping for entities that are mapped as a whole, which dictates that there can only be one EntityRecordGuid/OwnerSiteGuid available for mapping from any sitegroup.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT a.QuerySettingToSiteGuid MappingGuid, a.SiteGuid EntityRecordGuid, d.Id EntityId, a.MapToSiteGuid AssignedToSiteGuid, b.Id AssignedToSiteId, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a._RowVersion, a.AssignedFromSiteGuid , c.ID AssignedFromSiteId 
		FROM map.tblEntityQuerySettingToSite a
		INNER JOIN tblSites b
		ON b.SiteGuid = a.MapToSiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblSites d
		ON d.SiteGuid = a.SiteGuid
		WHERE a.AssignedFromSiteGuid = @AssignedFromSiteGuid
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
						+ 'Procedure Name: map.usp_GetQuerySettingToSiteByAssignedFromSiteGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
