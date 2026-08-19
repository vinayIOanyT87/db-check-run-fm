------------------------------------------------------------------------------------------------------
-- Stored Procedure: [map].[usp_GetCompanyToUserGroupMappingsByUserGroup] 
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13
-- Version/Date: 1.0.004 / 2022-03-11 (Modified by Richard R. Panachida)
-- Version/Date: 1.0.005 / 2022-03-22 (Modified by Richard R. Panachida)
-- Purpose: Retrieve the Company records that are tied to a given User Group and that has been assigned to a given Site/SiteGroup.
-- Notes:
-- 1. @AssignedToUserGroupGuid: UserGroup Guid that needs to be examined.
-- 2. @TargetSiteGuid: SiteGuid for which to run the query.
-- 4. This stored procedure replaces the CompanyMapClass.EnumerateByAssignedToGuidAndTypeSQL() inline SQL for the case where the Company Mapping Type is COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP, and the bInTransaction is false.
-- 5. This stored procedure must also work/be tested for the special case where the CompanyGuid in the mapping is NULL, which indicates a UserGroup mapping to ALL companies.
--
-- Testing:
-- EXEC [map].[usp_GetCompanyToUserGroupMappingsByUserGroup] '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000003'
-- EXEC [map].[usp_GetCompanyToUserGroupMappingsByUserGroup] '00000000-0000-0000-0000-000000000001', 'cc715445-0f51-4d62-8042-55fe0dd90461'
------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [map].[usp_GetCompanyToUserGroupMappingsByUserGroup]
(
	@TargetSiteGuid uniqueidentifier
	, @AssignedToUserGroupGuid uniqueidentifier
)
AS
BEGIN
	BEGIN TRY
		-- Get the Site Guid of the Group based on the Group Guid.
		DECLARE @GroupAssignSiteGuid uniqueidentifier
		SELECT @GroupAssignSiteGuid = SiteGuid FROM tblGroups WHERE GroupGuid = @AssignedToUserGroupGuid

		SELECT ctg.*, g.GroupID AS AssignedToID, c.ID AS AssignedID, c.LockedOut AS LockedOut
		FROM map.tblCompanyCompanyToUserGroup ctg
			INNER JOIN tblGroups g ON g.GroupGuid = ctg.GroupGuid
			LEFT OUTER JOIN map.tblEntityCompanyToSite ects ON ects.CompanyGuid = ctg.CompanyGuid AND ects.SiteGuid = ctg.SiteGuid
			LEFT OUTER JOIN tblCompanies c ON c.[_MasterRecordGuid] = ctg.CompanyGuid AND c.SiteGuid = ects.AssignedFromSiteGuid
		WHERE ctg.GroupGuid = @AssignedToUserGroupGuid
			AND ctg.SiteGuid = @TargetSiteGuid

	END TRY
	BEGIN CATCH
		DECLARE @_ErrMessage NVARCHAR(2048)
				, @_ErrNumber INT
				, @_ErrProcName NVARCHAR(126)
				, @_ErrLineNumber INT;
		SET @_ErrMessage = ERROR_MESSAGE();
		SET @_ErrNumber = ERROR_NUMBER();
		SET @_ErrProcName= ERROR_PROCEDURE();
		SET @_ErrLineNumber = ERROR_LINE();
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)
						+ 'Procedure Name: [map].usp_GetCompanyToUserGroupMappingsByUserGroup' + CHAR(13)+CHAR(10)
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
		RAISERROR(@_ErrMessage,18,1);
	END CATCH
END
