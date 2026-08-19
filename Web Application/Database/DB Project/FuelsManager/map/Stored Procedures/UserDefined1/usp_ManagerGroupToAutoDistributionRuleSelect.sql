
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ManagerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@ManagerGroupGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.ManagerGroupToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.ManagerGroupGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblManagerGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@ManagerGroupToAutoDistributionRuleGuid IS NULL) OR (@ManagerGroupToAutoDistributionRuleGuid = MAIN.ManagerGroupToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@ManagerGroupGuid IS NULL) OR (@ManagerGroupGuid = MAIN.ManagerGroupGuid))

END