

/*
	EXEC [map].[usp_GetCompanyMapRolesBySite] '00000000-0000-0000-0000-000000000001', 0, NULL, NULL, NULL
	EXEC [map].[usp_GetCompanyMapRolesBySite] '00000000-0000-0000-0000-000000000001', 1, NULL, NULL, NULL
	EXEC [map].[usp_GetCompanyMapRolesBySite] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', 0, NULL, NULL, NULL
	EXEC [map].[usp_GetCompanyMapRolesBySite] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', 1, NULL, NULL, NULL
	EXEC [map].[usp_GetCompanyMapRolesBySite] '46426312-E408-4AF8-85FD-338B622B32BF', 0, NULL, NULL, NULL
	EXEC [map].[usp_GetCompanyMapRolesBySite] '46426312-E408-4AF8-85FD-338B622B32BF', 0, '012D8DD3-E6FA-4B78-A81A-C84F1C360558', NULL, NULL
	EXEC [map].[usp_GetCompanyMapRolesBySite] '46426312-E408-4AF8-85FD-338B622B32BF', 0, NULL, '%02%', NULL
	EXEC [map].[usp_GetCompanyMapRolesBySite] '46426312-E408-4AF8-85FD-338B622B32BF', 0, NULL, null, 3

*/

CREATE PROCEDURE [map].[usp_GetCompanyMapRolesBySite]
(
	@TargetSiteGuid uniqueidentifier, @IncludeChildSites bit, @CompanyMasterRecordGuid uniqueidentifier, @FindString nvarchar(100), @RoleIndex int
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetCompanyMapRolesBySite] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Commpany Role Assignment mapping records for a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. @IncludeChildSites: 0: limit the query to the @TargetSiteGuid. 1: Extend the query to the complete children site hierarchy below @TargetSiteGuid.
	-- 3. @CompanyMasterRecordGuid: Limit results to Companies with a MasterRecordGuid of @CompanyMasterRecordGuid.
	-- 4. @FindString: Limit results to Companies that have an Id or a Name that contains the @FindString string.
	-- 5. @RoleIndex: Limit results to the Company Role that corresponds to the @RoleIndex number.
	-- 6. This stored procedure replaces the CompanyRoleMapClass.EnumerateByCriterionSQL inline SQL.
	-- 7. This Stored Procedure assumes that the set of CompanyRoles for a company are always cloned when a company is assigned to a lower site/sitegroup, irrespective of whether Company Record Versioning is turned ON or not.
	-- 8. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

	--Capture the FLC configuration for the Company Entity Type
	DECLARE @tblCompanyFLC TABLE
	(
		[EntitySegmentTemplateGuid] [uniqueidentifier],
		[SiteGroupGuid] [uniqueidentifier] NULL,
		[TargetField] nvarchar(100),
		[ForwardControlMode] nvarchar(20) NULL
	)		
	
	DECLARE @tblTargetSites TABLE
	(
		[SiteGuid] [uniqueidentifier]
		, SiteId nvarchar(30)
		, HierarchyLevel int
	)		

	DECLARE @tblTargetRecordVersions TABLE
	(
		[CompanyGuid] [uniqueidentifier],
		[MasterRecordGuid] [uniqueidentifier],
		[AssignedFromSiteGuid] [uniqueidentifier],
		[AssignedToSiteGuid] [uniqueidentifier]
	)		

	--Capture the target Sites for the query
	IF (@IncludeChildSites <> 1)
	BEGIN
		INSERT INTO @tblTargetSites
		(SiteGuid, SiteId, HierarchyLevel)
		SELECT SiteGuid, id, 0
		FROM tblSites
		WHERE SiteGuid = @TargetSiteGuid
	END
	ELSE
	BEGIN
		INSERT INTO @tblTargetSites
		(SiteGuid, SiteId, HierarchyLevel)
		SELECT SiteGuid, SiteId, HierarchyLevel 
		FROM [erv].[udf_GetSiteHierarchy](@TargetSiteGuid, 1)
		ORDER BY HierarchyLevel, SiteId
	END

	--Retrieve the complete company FLC configurations for all sites
	INSERT INTO @tblCompanyFLC
	(EntitySegmentTemplateGuid, SiteGroupGuid, TargetField, ForwardControlMode)
	SELECT g.EntitySegmentTemplateGuid, g. SiteGroupGuid, g.TargetField, g.ForWardControlMode FROM erv.tblEntityRecordVersioningFieldConfig g
	INNER JOIN erv.tblEntitySegmentTemplate h
	ON h.EntitySegmentTemplateGuid = g.EntitySegmentTemplateGuid
	WHERE h.EntityTypeId = 'Company'

	--Retrieve all the company record versions for the applicable sites
	DECLARE @siteGuidTemp uniqueidentifier
	DECLARE TargetRecordVersionsCursor CURSOR FOR 
		SELECT SiteGuid	FROM @tblTargetSites
	OPEN TargetRecordVersionsCursor

	FETCH NEXT FROM TargetRecordVersionsCursor INTO @siteGuidTemp

	WHILE @@FETCH_STATUS = 0
	BEGIN		
		INSERT INTO @tblTargetRecordVersions
		(CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
		SELECT CompanyGuid,MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM [erv].[udf_GetCompanyRecordVersions](@siteGuidTemp) a

		FETCH NEXT FROM TargetRecordVersionsCursor INTO @siteGuidTemp
	END 


	--Retrieve the Company Roles for the applicable companies
	SELECT a.LookupCompanyRoleIndex, a.CompanyGuid, c.ID AS CompanyID, c.Name AS CompanyName, c.Address1 AS CompanyAddress1, c.Address2 AS CompanyAddress2, 
	d.ID AS SiteID, a.SiteGuid, a.CreatedBy, a.CreatedDate,
	CASE 
		WHEN ((c.CompanyGuid = c._MasterRecordGuid) AND (e.AssignedFromSiteGuid = e.SiteGuid)) THEN 'VersionSpecific'
		WHEN f.ForwardControlMode IS NULL THEN 'ParentSpecific'
		ELSE f.ForwardControlMode
	END AS CompanyRolesFCM,
	CASE 
		WHEN g.TargetFieldCount IS NULL THEN 0
		WHEN (g.TargetFieldCount > 0) THEN 1
		ELSE 0
	END AS IsCompanyRecVerON
	FROM map.tblCompanyToRole a
	INNER JOIN @tblTargetRecordVersions b
	ON b.MasterRecordGuid = a.CompanyGuid
	AND b.AssignedToSiteGuid = a.SiteGuid
	INNER JOIN tblCompanies c
	ON c.CompanyGuid = b.CompanyGuid
	INNER JOIN tblSites d 
	ON d.SiteGuid = a.SiteGuid 
	INNER JOIN map.tblEntityCompanyToSite e
	ON e.CompanyGuid = c._MasterRecordGuid
	AND e.SiteGuid = a.SiteGuid		
	LEFT OUTER JOIN 
	(
		SELECT EntitySegmentTemplateGuid, SiteGroupGuid, TargetField, ForwardControlMode FROM @tblCompanyFLC
		WHERE TargetField = 'CompanyRoles'
	) f
	ON f.SiteGroupGuid = e.AssignedFromSiteGuid
	LEFT OUTER JOIN 
	(
		SELECT EntitySegmentTemplateGuid, SiteGroupGuid, COUNT(*) TargetFieldCount FROM @tblCompanyFLC
		WHERE ForwardControlMode = 'VersionSpecific'
		GROUP BY EntitySegmentTemplateGuid, SiteGroupGuid
	) g
	ON g.SiteGroupGuid = e.AssignedFromSiteGuid
	INNER JOIN @tblTargetSites h
	ON h.SiteGuid = e.SiteGuid
	WHERE ((@CompanyMasterRecordGuid IS NULL) OR (a.CompanyGuid = @CompanyMasterRecordGuid))
	AND 
	(
		(@FindString IS NULL) 
		OR 
		(c.ID LIKE(UPPER(@FindString)) OR c.Name LIKE(UPPER(@FindString)))
	)
	AND ((@RoleIndex IS NULL) OR (a.LookupCompanyRoleIndex = @RoleIndex))
	ORDER BY h.HierarchyLevel, SiteId, CompanyId, a.LookupCompanyRoleIndex

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
						+ 'Procedure Name: [map].usp_GetCompanyMapRolesBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END