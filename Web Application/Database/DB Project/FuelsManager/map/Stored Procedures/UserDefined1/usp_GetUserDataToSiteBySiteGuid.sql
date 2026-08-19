

/*
	EXEC [map].[usp_GetUserDataToSiteBySiteGuid] '00000000-0000-0000-0000-000000000001'
*/

CREATE PROCEDURE [map].[usp_GetUserDataToSiteBySiteGuid]
(
	@AssignedToSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetUserDataToSiteBySiteGuid]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the UserDataToSite assignments to a given site/sitegroup. 	
	-- Notes:
	-- 1. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup from which the mapping tree search is to start. If not provided, the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	-- 2. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 3. UserDataToSite mappings are not applied/managed on individual UserData records, but across all UserData records for a given sitegroup (@EntityRecordGuid)
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		IF (@AssignedToSiteGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END

		SELECT a.UserDataToSiteGuid MappingGuid, a.OwnerSiteGuid EntityRecordGuid, d.Id EntityId, a.MapToSiteGuid AssignedToSiteGuid, b.Id AssignedToSiteId, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a._RowVersion, a.AssignedFromSiteGuid , c.ID AssignedFromSiteId 
		FROM map.tblEntityUserDataToSite a
		INNER JOIN tblSites b
		ON b.SiteGuid = a.MapToSiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblSites d
		ON d.SiteGuid = a.OwnerSiteGuid
		WHERE a.MapToSiteGuid = @AssignedToSiteGuid
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
						+ 'Procedure Name: map.usp_GetUserDataToSiteBySiteGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
