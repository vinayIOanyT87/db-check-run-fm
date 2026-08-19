

CREATE FUNCTION [dbo].[udf_AuthorizedCompaniesGuid]
(@SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER = NULL)
RETURNS @Guid TABLE(CompanyGuid uniqueidentifier)
AS
BEGIN
	-- If @UserGuid = NULL OR user group is mapped to CompanyGuid=NULL in tblCompanyCompanyToUserGroup,
	-- then return all companies for the site
	IF @UserGuid IS NULL
	OR EXISTS (SELECT *
				FROM map.tblCompanyCompanyToUserGroup cm
				INNER JOIN map.tblUserToGroup ug ON ug.GroupGuid = cm.GroupGuid AND ug.SiteGuid = @SiteGuid
				WHERE ug.UserGuid = @UserGuid AND cm.CompanyGuid IS NULL)
	BEGIN
		INSERT INTO @Guid(CompanyGuid)
		SELECT NULL
	END
	ELSE
	BEGIN
		INSERT INTO @Guid(CompanyGuid)
		SELECT SRM._MasterRecordGuid 
		FROM erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM 
			INNER JOIN tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid
		WHERE EXISTS (SELECT cm.CompanyCompanyToUserGroupGuid 
			FROM map.tblCompanyCompanyToUserGroup cm
			INNER JOIN map.tblUserToGroup ug ON ug.GroupGuid = cm.GroupGuid AND ug.SiteGuid = @SiteGuid
			INNER JOIN tblCompanies c ON c.CompanyGuid = cm.CompanyGuid
			WHERE ug.UserGuid = @UserGuid AND c._MasterRecordGuid = SRM._MasterRecordGuid)
	END
	RETURN
END
