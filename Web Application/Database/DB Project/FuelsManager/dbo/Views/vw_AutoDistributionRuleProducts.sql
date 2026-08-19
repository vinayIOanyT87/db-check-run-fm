

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/14/2012
-- Description:	Select all products for all rules
-- ==================================================================================================================
CREATE VIEW [dbo].[vw_AutoDistributionRuleProducts]
AS
SELECT DISTINCT
	*
FROM
(
	(
		-- Products from product groups
		SELECT
			MAIN.*, PROD.[ProductGuid], PROD.ProductID
		FROM
			[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
			INNER JOIN [map].[tblProductGroupToAutoDistributionRule] RPGPMAP WITH (NOLOCK)
			ON MAIN.AutoDistributionRuleGuid = RPGPMAP.AutoDistributionRuleGuid

			INNER JOIN [map].[tblProductToProductGroup] PGPMAP WITH (NOLOCK)
			ON PGPMAP.[AssignedToApplicationStringGuid] = RPGPMAP.ProductGroupGuid

			INNER JOIN [dbo].[tblProducts] PROD WITH (NOLOCK)
			ON PROD.[ProductGuid] = PGPMAP.[ProductGuid]
	) UNION (
		-- Direct products
		SELECT
			MAIN.*, PROD.[ProductGuid], PROD.ProductID
		FROM
			[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
			INNER JOIN [map].[tblProductToAutoDistributionRule] PRODMAP WITH (NOLOCK)
			ON MAIN.AutoDistributionRuleGuid = PRODMAP.AutoDistributionRuleGuid

			INNER JOIN [dbo].[tblProducts] PROD WITH (NOLOCK)
			ON PROD.[ProductGuid] = PRODMAP.ProductGuid
	)
) RPROD