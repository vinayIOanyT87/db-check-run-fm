
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@OwnerToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@OwnerGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.OwnerToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.OwnerGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblOwnerToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@OwnerToAutoDistributionRuleGuid IS NULL) OR (@OwnerToAutoDistributionRuleGuid = MAIN.OwnerToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@OwnerGuid IS NULL) OR (@OwnerGuid = MAIN.OwnerGuid))

END