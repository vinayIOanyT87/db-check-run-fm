
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/10/2012
-- Description:	Validate the given rule is the only defaultEOM for the Mgr/Prod combination
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleFindDuplicateDefaultEOM] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN	
	
	SELECT
		OTHERS.AutoDistributionRuleGuid, OTHERS.RuleID,
		OTHERS.CompanyGuid, OTHERS.CompanyID, 
		OTHERS.ProductGuid, OTHERS.ProductID
		
	FROM
		[dbo].[vw_AutoDistributionRuleManagersProducts] ME
		INNER JOIN
		( 
			SELECT OTHERSTMP.*
			FROM
				[dbo].[udf_AutoDistributionRuleSelectRulesBySite](@SelectedSiteGuid,@LoginSiteGuid) MAIN
				INNER JOIN [dbo].[vw_AutoDistributionRuleManagersProducts] OTHERSTMP
				ON MAIN.AutoDistributionRuleGuid = OTHERSTMP.AutoDistributionRuleGuid
					AND MAIN.DefaultEOM = 1 AND MAIN.RuleEnabled = 1
				
		) OTHERS
		ON ME.CompanyGuid = OTHERS.CompanyGuid AND Me.ProductGuid = OTHERS.ProductGuid
	WHERE
		ME.AutoDistributionRuleGuid = @AutoDistributionRuleGuid
		AND ME.DefaultEOM = 1 AND ME.RuleEnabled = 1
		AND OTHERS.AutoDistributionRuleGuid <> @AutoDistributionRuleGuid
	GROUP BY
		OTHERS.AutoDistributionRuleGuid, OTHERS.RuleID,
		OTHERS.CompanyGuid, OTHERS.CompanyID, 
		OTHERS.ProductGuid, OTHERS.ProductID
	ORDER BY
		OTHERS.CompanyID, OTHERS.ProductID
END