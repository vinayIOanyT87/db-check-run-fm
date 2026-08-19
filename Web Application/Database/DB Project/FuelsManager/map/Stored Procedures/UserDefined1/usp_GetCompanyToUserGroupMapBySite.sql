
CREATE PROCEDURE [map].[usp_GetCompanyToUserGroupMapBySite]
(
	@TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetCompanyToUserGroupMapBySite] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the Company-To-UserGroup mappings for a given Site/Sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		

	SELECT ccug.*, 
	groups.GroupID AS AssignedToID, 
	c1.ID AS AssignedID, 
	c1.LockedOut AS LockedOut 
	FROM map.tblCompanyCompanyToUserGroup  ccug 
	INNER JOIN erv.udf_GetCompanyRecordVersions(@TargetSiteGuid) erv  
	ON erv.MasterRecordGuid = ccug.CompanyGuid 
	INNER JOIN tblGroups groups 
	ON groups.GroupGuid = ccug.GroupGuid 
	INNER JOIN map.tblEntityUserGroupToSite eugs 
	ON groups.GroupGuid = eugs.GroupGuid 
	LEFT JOIN tblCompanies c1 
	ON c1.CompanyGuid = ccug.CompanyGuid 
	WHERE eugs.SiteGuid = @TargetSiteGuid
	AND ccug.SiteGuid = @TargetSiteGuid
	ORDER BY ccug.CompanyGuid	

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
						+ 'Procedure Name: [map].usp_GetCompanyToUserGroupMapBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     



