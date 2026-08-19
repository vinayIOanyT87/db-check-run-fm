
/*
	EXEC [erv].[usp_GetFLCSiteHierarchy] NULL, 1
	EXEC [erv].[usp_GetFLCSiteHierarchy] NULL, 0
	EXEC [erv].[usp_GetFLCSiteHierarchy] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', 1
*/
CREATE PROCEDURE [erv].[usp_GetFLCSiteHierarchy]
(
	@SiteGuid uniqueidentifier,
	@SiteGroupsOnly bit = 1
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_GetFLCSiteHierarchy] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the site hierarchy tree for a given site group
	-- Notes:
	-- 1. If the SiteGuid parameter is null, then the site hierarchy under the first root node (item without a parent) is returned.
	-- 2. The site hierarchy is returned in the order of the hierarchy levels, with the top level first, and in the order of the site/sitegroup IDs.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		IF (@SiteGuid IS NULL)
		BEGIN
			--Retrieve the first available root node
			SET @SiteGuid = 
			(			
				SELECT TOP(1) a.ChildSiteGuid
				FROM map.tblSiteToSite a
				WHERE ((a.ParentSiteGuid IS NULL) OR (a.ChildSiteGuid = a.ParentSiteGuid))
				AND NOT EXISTS
				(
					SELECT * FROM map.tblSiteToSite b
					WHERE b.ChildSiteGuid = a.ChildSiteGuid
					AND b.ChildSiteGuid <> ISNULL(b.ParentSiteGuid, b.ChildSiteGuid)
				)			
			)
		END;
	
		WITH SiteHierarchy (SiteGuid, ParentSiteGuid, lvl)
		AS 
		(
			--Anchor
			SELECT TOP(1) ChildSiteGuid, ParentSiteGuid, 0
			FROM map.tblSiteToSite
			WHERE ChildSiteGuid = @SiteGuid -- node for which the tree structure underneath it needs to be retrieved.
			-- Recursive Call
			UNION ALL
			SELECT a.ChildSiteGuid, a.ParentSiteGuid, lvl + 1
			FROM map.tblSiteToSite a 
			INNER JOIN SiteHierarchy b
			ON a.ParentSiteGuid = b.SiteGuid
			AND a.ChildSiteGuid <> a.ParentSiteGuid
		)
		

		SELECT a.SiteGuid, b.id SiteId, MAX(a.lvl) HierarchyLevel
		FROM SiteHierarchy a
		INNER JOIN tblSites b
		ON b.SiteGuid = a.SiteGuid
		WHERE (((@SiteGroupsOnly = 1) AND (b.SiteGroupFlag = 1)) OR (@SiteGroupsOnly = 0) OR (@SiteGroupsOnly IS NULL))
		GROUP BY a.SiteGuid, b.ID
		ORDER BY HierarchyLevel, SiteId

				
		OPTION (MAXRECURSION 10000);
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
						+ 'Procedure Name: dbo.usp_GetFLCSiteHierarchy' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
GO

