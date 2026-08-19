/*
	DROP PROCEDURE [map].[usp_GetUserPermissionForSiteHierarchy]

	EXEC [map].[usp_GetUserPermissionForSiteHierarchy] '00000000-0000-0000-0000-000000000002', NULL
	EXEC [map].[usp_GetUserPermissionForSiteHierarchy] '919F57C8-7001-4D63-B09C-C51BF22408D9', NULL
	EXEC [map].[usp_GetUserPermissionForSiteHierarchy] '919F57C8-7001-4D63-B09C-C51BF22408D9', 'B6B0C108-C9F2-438F-9917-3189E471A3C2'
	
*/

CREATE PROCEDURE [map].[usp_GetUserPermissionForSiteHierarchy]
(
	@UserGuid uniqueidentifier,
	@StartSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetUserPermissionForSiteHierarchy] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the User Permissions for a given user, from a given Site down the Site Hierarchy.
	-- Notes:
	-- 1. @UserGuid: USerGuid of the User for which to retrieve the permissions data.
	-- 2. @StartSiteGuid: Guid of the site from which the retrieve permissions data down the site hierarchy. If the StartSiteGuid parameter is null, then the UserPermissions for the full site hierarchy for the user is returned
	-- 3. The site hierarchy is returned in the order of the hierarchy levels, with the top level first, and in the order of the site/sitegroup IDs.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @tblFullSiteHierarchy table
		(
			SiteGuid uniqueidentifier,
			SiteId nvarchar(30),
			HierarchyLevel int
		)
		
		INSERT INTO @tblFullSiteHierarchy
		EXEC [erv].[usp_GetFLCSiteHierarchy] @startSiteGuid, 0
		
		SELECT b.UserGuid, a.HierarchyLevel, c.SiteGuid, c.ID SiteId, e.GroupGuid, e.GroupID, e.GroupDescription, g.RightGuid, g.RightIndex, g.RightName, g.RightCode, g.RightDescription 
		FROM @tblFullSiteHierarchy a
		INNER JOIN map.tblEntityUserToSite b
		ON b.SiteGuid = a.SiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.SiteGuid
		INNER JOIN map.tblUserToGroup d
		ON d.UserGuid = b.UserGuid
		AND d.SiteGuid = b.SiteGuid
		INNER JOIN tblGroups e
		ON e.GroupGuid = d.GroupGuid
		INNER JOIN map.tblGroupToRight f
		ON f.GroupGuid = d.GroupGuid
		INNER JOIN lookup.tblRight g
		ON g.RightIndex = f.LookupRightIndex
		WHERE b.UserGuid = @UserGuid
		ORDER BY a.HierarchyLevel, c.ID, e.GroupID, g.RightName
			
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
						+ 'Procedure Name: map.usp_GetUserPermissionForSiteHierarchy' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END