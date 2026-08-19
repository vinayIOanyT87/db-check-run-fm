/*
	DROP PROCEDURE [erv].[usp_PropagateCompanyUserGroupMappingsByUserGroup]

	EXEC [erv].[usp_PropagateCompanyUserGroupMappingsByUserGroup] '6045F07E-956F-43A6-B604-676679CFE91E', 'Tester'
	EXEC [erv].[usp_PropagateCompanyUserGroupMappingsByUserGroup] 'F94D0DAB-8C85-4A73-830E-A8168078B6AD', 'Admin'

*/

------------------------------------------------------------------------------------------------------
-- Stored Procedure: [erv].[usp_PropagateCompanyUserGroupMappingsByUserGroup] 
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Propagate the current Company-UserGroup mappings of a given UserGroup entity record down the site hierarchy, according to the rules established by the Company Field Level Control configurations.
-- This Stored Procedure is to be used to propagate the UserGroup-Company mapping changes down to all of the applicable company children record versions.
-- This Stored Procedure is meant to support Company-UserGroup mapping changes as initiated from the UserGroup side only.
-- Changes to the Company-UserGroup mappings that are initiated from the Company side are handled as part of the overall Company change propagation operation ([erv].[usp_PropagateCompanyRecordVersionBySegment]).
-- Notes:
-- 1. @SourceUserGroupGuid: Guid of the UserGroup record whose Company mappings need to be propagated down the site hierarchy. 
-- 2. @CreatedBy: Login user name of the FuelsManager user.
------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [erv].[usp_PropagateCompanyUserGroupMappingsByUserGroup]
(
	@SourceUserGroupGuid uniqueidentifier
	,@CreatedBy udtUserID = NULL
)
	AS
	BEGIN
	BEGIN TRY
		DECLARE @EmptyGuid uniqueidentifier
		SET @EmptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Company'

		DECLARE @ownerSiteGuid uniqueidentifier
		SELECT @ownerSiteGuid = SiteGuid FROM tblGroups 
		WHERE GroupGuid = @SourceUserGroupGuid

		IF (@ownerSiteGuid IS NULL)
		BEGIN
			RAISERROR('Cannot locate the source record for data propagation.',16,1); 
			RETURN;
		END


		-- Retrieve the Entity To Site hierarchy below the owner sitegroup of the entity record whose changes are to be propagated
		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			SiteGuid uniqueidentifier
			, SiteId nvarchar(30)
			, HierarchyLevel int
			, Processed bit
		);

		INSERT INTO @tblEntityToSiteHierarchy
		(SiteGuid, SiteId, HierarchyLevel, Processed)
		SELECT SiteGuid, SiteId, HierarchyLevel, 0
		FROM [erv].[udf_GetUserGroupToSiteHierarchyByEntityGuid](@SourceUserGroupGuid)
		WHERE HierarchyLevel > 0
		ORDER BY HierarchyLevel, SiteGuid

		-- Capture the Company.UserGroup FLC setting
		DECLARE @IsUserGroupsFieldVersionSpecific bit
		SET @IsUserGroupsFieldVersionSpecific = 0
		IF EXISTS
		(
			SELECT *
			FROM erv.tblEntitySegmentTemplate a					
			INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
			ON (b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid) 				
			AND (ISNULL(b.FilterValueGuid, @emptyGuid) = @emptyGuid)
			WHERE EntityTypeId = @EntityTypeId
			AND b.SiteGroupGuid = @ownerSiteGuid
			AND b.IsExternalAttribute = 1 
			AND b.TargetField = 'UserGroups'
			AND b.ForwardControlMode = 'VersionSpecific'
		)
		BEGIN
			SET @IsUserGroupsFieldVersionSpecific = 1
		END

		DECLARE @BeginTran BIT = 0
		IF (@@TRANCOUNT = 0)
		BEGIN
			BEGIN TRANSACTION --PropagateToChildRecordVersions
			SET @BeginTran = 1
		END

		--Delete the UserGroups mappings where UserGroup is no longer assigned
		DELETE cctug FROM [map].[tblCompanyCompanyToUserGroup] cctug
		WHERE cctug.GroupGuid = @SourceUserGroupGuid
		AND cctug.SiteGuid NOT IN (SELECT SiteGuid FROM map.tblEntityUserGroupToSite WHERE GroupGuid = @SourceUserGroupGuid)

		--For each of the master Companies that are tied to the source UserGroup, reset the Company-UserGroup mappings to that source UserGroup down the site hierarchy to match the Company-UserGroup mappings of each master Company at its owner site against the source UserGroup.
		IF (@IsUserGroupsFieldVersionSpecific = 0)
		BEGIN
			DELETE a FROM [map].[tblCompanyCompanyToUserGroup] a
			INNER JOIN @tblEntityToSiteHierarchy b
			ON b.SiteGuid = a.SiteGuid
			WHERE a.GroupGuid = @SourceUserGroupGuid
			AND a.CompanyGuid IS NULL
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyCompanyToUserGroup] c
				WHERE c.GroupGuid = a.GroupGuid
				AND c.SiteGuid = @ownerSiteGuid
				AND c.CompanyGuid IS NULL
			)

			DELETE a FROM [map].[tblCompanyCompanyToUserGroup] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.SiteGuid = a.SiteGuid
			WHERE a.GroupGuid = @SourceUserGroupGuid
			AND a.CompanyGuid IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyCompanyToUserGroup] d
				WHERE d.CompanyGuid = a.CompanyGuid
				AND d.GroupGuid = a.GroupGuid
				AND d.SiteGuid = @ownerSiteGuid
			)
			AND NOT EXISTS  --Protect mappings made to companies assigned to the mapping site from other that the UserGroup owner site, e.g. UserGroup mappings to companies that were created (not assigned down) at the site to which the target UserGroup was assigned down to.
			(
				SELECT * FROM dbo.tblCompanies e
				WHERE e.CompanyGuid = a.CompanyGuid
				AND e.SiteGuid <> @ownerSiteGuid
			)

			IF (EXISTS (SELECT * FROM [map].[tblCompanyCompanyToUserGroup] WHERE GroupGuid = @SourceUserGroupGuid AND SiteGuid = @ownerSiteGuid AND CompanyGuid IS NULL))
			BEGIN
				DELETE a FROM [map].[tblCompanyCompanyToUserGroup] a
				INNER JOIN @tblEntityToSiteHierarchy b
				ON b.SiteGuid = a.SiteGuid
				WHERE a.GroupGuid = @SourceUserGroupGuid
				AND a.CompanyGuid IS NOT NULL
			END

			DECLARE @CreatedDate datetimeoffset = SYSDATETIMEOFFSET()
			SET @CreatedBy = ISNULL(@CreatedBy, 'Admin')
			-- Insert a new Company-UserGroup child record mapping for each child site in the Entity To Site hierarchy where CompanyGuid is NULL
			INSERT INTO [map].[tblCompanyCompanyToUserGroup]
				([CompanyGuid], [GroupGuid], [SiteGuid], [ID], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
			SELECT a.[CompanyGuid], @SourceUserGroupGuid, b.[SiteGuid], '', @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy 
			FROM [map].[tblCompanyCompanyToUserGroup] a
			CROSS JOIN @tblEntityToSiteHierarchy b
			WHERE a.[SiteGuid] = @ownerSiteGuid
			AND a.[CompanyGuid] IS NULL
			AND a.[GroupGuid] = @SourceUserGroupGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyCompanyToUserGroup] cctug2 
				WHERE cctug2.[SiteGuid] = b.SiteGuid 
				AND cctug2.CompanyGuid IS NULL
				AND cctug2.GroupGuid = @SourceUserGroupGuid
			) 

			-- Insert a new Company-UserGroup child record mapping for each child site in the Entity To Site hierarchy where CompanyGuid is NOT NULL
			INSERT INTO [map].[tblCompanyCompanyToUserGroup]
				([CompanyGuid], [GroupGuid], [SiteGuid], [ID], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
			SELECT a.[CompanyGuid], @SourceUserGroupGuid, b.[SiteGuid], a.[ID], @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy 
			FROM [map].[tblCompanyCompanyToUserGroup] a
			CROSS JOIN @tblEntityToSiteHierarchy b
			INNER JOIN [map].[tblEntityCompanyToSite] ects 
			ON ects.[CompanyGuid] = a.[CompanyGuid] AND ects.SiteGuid = b.SiteGuid
			WHERE a.[SiteGuid] = @ownerSiteGuid
			AND a.[GroupGuid] = @SourceUserGroupGuid
			AND a.CompanyGuid IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyCompanyToUserGroup] c
				WHERE c.[SiteGuid] = b.SiteGuid 
				AND c.CompanyGuid = a.CompanyGuid 
				AND c.GroupGuid = @SourceUserGroupGuid
			) 
		END

		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --PropagateToChildRecordVersions
		END
	END TRY
	BEGIN CATCH
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --PropagateToChildRecordVersions
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
						+ 'Procedure Name: [erv].usp_PropagateCompanyUserGroupMappingsByUserGroup' + CHAR(13)+CHAR(10)
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
		RAISERROR(@_ErrMessage,18,1);
	END CATCH
END
