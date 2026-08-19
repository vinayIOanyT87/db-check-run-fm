
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleToSiteGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.AutoDistributionRuleToSiteGuid, MAIN.SiteGuid, MAIN.AutoDistributionRuleGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblEntityAutoDistributionRuleToSite] MAIN WITH (NOLOCK)
	WHERE
		((@AutoDistributionRuleToSiteGuid IS NULL) OR (@AutoDistributionRuleToSiteGuid = MAIN.AutoDistributionRuleToSiteGuid))
		AND ((@SiteGuid IS NULL) OR (@SiteGuid = MAIN.SiteGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))

END