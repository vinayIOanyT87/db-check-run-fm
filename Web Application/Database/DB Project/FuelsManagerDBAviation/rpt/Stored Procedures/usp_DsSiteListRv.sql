CREATE PROCEDURE [rpt].[usp_DsSiteListRv]
@SiteGuid UNIQUEIDENTIFIER,
@UserGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_DsSiteListRv] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-05-08 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Site ID's (non-group) based on SiteGuid and UserGuid.
	-- Notes:
	-- 1. @SiteGuid: Limit results to Sites that are below this site/sitegroup in the siteHeirarchy
	-- 2. @UserGuid: Limit results to Sites to only those that the user has been assigned to.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		SELECT a.SiteGuid, a.SiteId, a.HierarchyLevel 
		FROM erv.udf_GetSiteHierarchy(@SiteGuid, 1) a
		INNER JOIN map.tblEntityUserToSite c
		ON a.SiteGuid = c.SiteGuid
		INNER JOIN tblSites b
		ON a.SiteGuid = b.SiteGuid
		WHERE c.UserGuid = @UserGuid AND b.SiteGroupFlag = 0 AND b.Enabled = 1
		ORDER BY a.HierarchyLevel, a.SiteId
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
						+ 'Procedure Name: [rpt].[usp_DsSiteListRv]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END