

-- ==================================================================================================================
-- Author:		Daniel Or
-- Update date:	10/26/2012
-- Description:	Select all managers/products combination for all rules (excluding blank manager or product)
-- ==================================================================================================================
CREATE VIEW [dbo].[vw_AutoDistributionRuleManagersProducts]
AS
SELECT
	MAIN.*, MGR.CompanyGuid, MGR.CompanyID, PROD.ProductGuid, PROD.ProductID
FROM
	[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
	LEFT JOIN [dbo].[vw_AutoDistributionRuleManagers] MGR
	ON MAIN.AutoDistributionRuleGuid = MGR.AutoDistributionRuleGuid
	LEFT JOIN [dbo].[vw_AutoDistributionRuleProducts] PROD
	ON MAIN.AutoDistributionRuleGuid = PROD.AutoDistributionRuleGuid