CREATE FUNCTION [dbo].[udf_CompanyList]
(@LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @Role INT, @HasAllItem INT)
RETURNS TABLE 
AS
RETURN 
    SELECT '<All>' CompanyName,
				 '<All>' CompanyCode
		 WHERE @HasAllItem = 1

		UNION

		SELECT c.ID CompanyName,
				 c.Code CompanyCode
		  FROM (select ba.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) aa inner join tblCompanies ba on aa.CompanyGuid = ba.CompanyGuid) c
		  inner join [map].[tblCompanyToRole] b On b.CompanyGuid = c.CompanyGuid
		  WHERE b.LookupCompanyRoleIndex = @Role