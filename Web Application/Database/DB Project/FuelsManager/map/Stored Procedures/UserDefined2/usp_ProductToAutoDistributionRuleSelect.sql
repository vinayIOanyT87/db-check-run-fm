
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ProductToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@ProductGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.ProductToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.ProductGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblProductToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@ProductToAutoDistributionRuleGuid IS NULL) OR (@ProductToAutoDistributionRuleGuid = MAIN.ProductToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@ProductGuid IS NULL) OR (@ProductGuid = MAIN.ProductGuid))

END