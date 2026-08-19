

-- ==================================================================================================================
-- Author:		Daniel Or
-- Updated date:	7/30/2013
-- Description:	Select record(s) from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@RuleID NVARCHAR(50) = NULL
) AS
BEGIN
	SELECT 
		MAIN.AutoDistributionRuleGuid, MAIN.SiteGuid, MAIN.RuleID, 
		MAIN.RuleDescription, MAIN.RuleEnabled, MAIN.DefaultEOM, MAIN.TransactionAliasGuid, 
		MAIN.DefaultReasonCodeGuid, MAIN.DefaultNotes, MAIN.CreatedDate, MAIN.CreatedBy, 
		MAIN.UpdatedDate, MAIN.UpdatedBy, MAIN._RowVersion
	FROM 
		[dbo].[udf_AutoDistributionRuleSelectRulesBySite](@SelectedSiteGuid,@LoginSiteGuid) MAIN 		
	WHERE
		((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@RuleID IS NULL) OR (@RuleID = MAIN.RuleID))
	ORDER BY
		MAIN.RuleID

END
