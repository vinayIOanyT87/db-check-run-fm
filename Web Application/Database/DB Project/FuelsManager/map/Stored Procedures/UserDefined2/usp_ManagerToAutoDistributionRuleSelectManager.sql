



-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
 CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleSelectManager] (
 	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
 	@SiteGuid UNIQUEIDENTIFIER
 ) AS
 BEGIN
 	SELECT 
 		MAIN.ManagerToAutoDistributionRuleGuid,
 		MAIN.AutoDistributionRuleGuid,
 		MAIN.ManagerGuid,
 		ASSIGNED.*
 	FROM 
 		[map].[tblManagerToAutoDistributionRule] MAIN WITH (NOLOCK)
 		INNER JOIN 
 			(select ba.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) aa 
 			inner join tblCompanies ba on aa.CompanyGuid = ba.CompanyGuid) 
 			ASSIGNED 
 			On MAIN.ManagerGuid = ASSIGNED.CompanyGuid OR MAIN.ManagerGuid = ASSIGNED._MasterRecordGuid
 	WHERE
 		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)
 END