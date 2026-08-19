

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/17/2012
-- Description:	Select all Owners for all rules
-- ==================================================================================================================
CREATE VIEW [dbo].[vw_AutoDistributionRuleOwners]
AS
SELECT DISTINCT
	*
FROM
(
	(
		-- Owners from owner groups
		SELECT
			MAIN.*, COMP.CompanyGuid, COMP.ID AS CompanyID
		FROM
			[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
			INNER JOIN [map].[tblOwnerGroupToAutoDistributionRule] OGPMAP WITH (NOLOCK)
			ON MAIN.AutoDistributionRuleGuid = OGPMAP.AutoDistributionRuleGuid

			INNER JOIN [map].[tblCompanyCompanyToCompanyGroup] COMGRPMAP WITH (NOLOCK)
			ON COMGRPMAP.ApplicationStringGuid = OGPMAP.OwnerGroupGuid

			INNER JOIN [dbo].[tblCompanies] COMP WITH (NOLOCK)
			ON COMP.[CompanyGuid] = COMGRPMAP.[CompanyGuid]
			
			INNER JOIN map.tblCompanyToRole ROLE
			ON COMP.CompanyGuid = ROLE.CompanyGuid AND ROLE.LookupCompanyRoleIndex = 1 /* Owner Role */
	) UNION (
		-- Direct Owners
		SELECT
			MAIN.*, COMP.CompanyGuid, COMP.ID AS CompanyID
		FROM
			[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
			INNER JOIN [map].[tblOwnerToAutoDistributionRule] OWNERMAP WITH (NOLOCK)
			ON MAIN.AutoDistributionRuleGuid = OWNERMAP.AutoDistributionRuleGuid

			INNER JOIN [dbo].[tblCompanies] COMP WITH (NOLOCK)
			ON COMP.[CompanyGuid] = OWNERMAP.[OwnerGuid]
	)
) RMGR