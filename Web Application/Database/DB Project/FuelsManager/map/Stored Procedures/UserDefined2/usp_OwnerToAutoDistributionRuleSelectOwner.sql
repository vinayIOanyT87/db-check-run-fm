
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
 CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleSelectOwner] (
 	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
 	@SiteGuid UNIQUEIDENTIFIER
 ) AS
 BEGIN
 	SELECT 
 		MAIN.OwnerToAutoDistributionRuleGuid,
 		MAIN.AutoDistributionRuleGuid,
 		MAIN.OwnerGuid,
 		ASSIGNED.*
 	FROM 
 		[map].[tblOwnerToAutoDistributionRule] MAIN WITH (NOLOCK)
 		INNER JOIN
 		(select ba.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) aa 
 		inner join tblCompanies ba on aa.CompanyGuid = ba.CompanyGuid) 
 		ASSIGNED 
 		On MAIN.OwnerGuid = ASSIGNED.CompanyGuid OR MAIN.OwnerGuid = ASSIGNED._MasterRecordGuid
 
 	WHERE
 		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)
 
 END