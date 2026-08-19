


-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/14/2012
-- Description:	Select all managers for all rules
-- ==================================================================================================================
CREATE VIEW [dbo].[vw_AutoDistributionRuleManagers]
AS
SELECT DISTINCT
	*
FROM
(
	(
		-- Managers from manager groups
		SELECT
			MAIN.*, COMP.CompanyGuid, COMP.ID AS CompanyID
		FROM
			[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
			INNER JOIN [map].[tblManagerGroupToAutoDistributionRule] MGPMAP WITH (NOLOCK)
			ON MAIN.AutoDistributionRuleGuid = MGPMAP.AutoDistributionRuleGuid

			INNER JOIN [map].[tblCompanyCompanyToCompanyGroup] COMGRPMAP WITH (NOLOCK)
			ON COMGRPMAP.ApplicationStringGuid = MGPMAP.ManagerGroupGuid

			INNER JOIN [dbo].[tblCompanies] COMP WITH (NOLOCK)
			ON COMP.[CompanyGuid] = COMGRPMAP.[CompanyGuid]
			
			INNER JOIN map.tblCompanyToRole ROLE
			ON COMP.CompanyGuid = ROLE.CompanyGuid AND ROLE.LookupCompanyRoleIndex = 0 /* Manager Role */
	) UNION (
		-- Direct managers
		SELECT
			MAIN.*, COMP.CompanyGuid, COMP.ID AS CompanyID
		FROM
			[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
			INNER JOIN [map].[tblManagerToAutoDistributionRule] MGRMAP WITH (NOLOCK)
			ON MAIN.AutoDistributionRuleGuid = MGRMAP.AutoDistributionRuleGuid

			INNER JOIN [dbo].[tblCompanies] COMP WITH (NOLOCK)
			ON COMP.[CompanyGuid] = MGRMAP.[ManagerGuid]
	)
) RMGR