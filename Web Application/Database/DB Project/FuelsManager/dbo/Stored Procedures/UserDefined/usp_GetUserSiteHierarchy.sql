/*
	DROP PROCEDURE [dbo].[usp_GetUserSiteHierarchy]

	EXEC [dbo].[usp_GetUserSiteHierarchy] '00000000-0000-0000-0000-000000000002', NULL
	EXEC [dbo].[usp_GetUserSiteHierarchy] '7708F581-E789-414E-83DC-5003B73D5F0B', NULL
	EXEC [dbo].[usp_GetUserSiteHierarchy] '7708F581-E789-414E-83DC-5003B73D5F0B', 'B6B0C108-C9F2-438F-9917-3189E471A3C2'
	
*/

CREATE PROCEDURE [dbo].[usp_GetUserSiteHierarchy]
(
	@UserGuid uniqueidentifier,
	@StartSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_GetUserSiteHierarchy] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the site hierarchy tree for a given user from a given SiteGuid down
	-- Notes:
	-- 1. If the StartSiteGuid parameter is null, then the full site hierarchy for the user is returned
	-- 2. The site hierarchy is returned in the order of the hierarchy levels, with the top level first, and in the order of the site/sitegroup IDs.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @tblFullSiteHierarchy table
		(
			SiteGuid uniqueidentifier,
			SiteId nvarchar(30),
			HierarchyLevel integer
		)
		
		INSERT INTO @tblFullSiteHierarchy
		EXEC [erv].[usp_GetFLCSiteHierarchy] @startSiteGuid, 0
		
		SELECT a.HierarchyLevel, c.SiteGuid, c.ID, c.AdministrativeLockDate, c. OperationalLockDate, c.SiteGroupFlag, c.Number 
		FROM @tblFullSiteHierarchy a
		INNER JOIN map.tblEntityUserToSite b
		ON b.SiteGuid = a.SiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.SiteGuid
		WHERE b.UserGuid = @UserGuid
		ORDER BY a.HierarchyLevel, c.ID
			
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
						+ 'Procedure Name: dbo.usp_GetUserSiteHierarchy' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END