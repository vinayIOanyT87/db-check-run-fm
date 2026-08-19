
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ProductGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@ProductGroupGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.ProductGroupToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.ProductGroupGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblProductGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@ProductGroupToAutoDistributionRuleGuid IS NULL) OR (@ProductGroupToAutoDistributionRuleGuid = MAIN.ProductGroupToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@ProductGroupGuid IS NULL) OR (@ProductGroupGuid = MAIN.ProductGroupGuid))

END