
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ManagerToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@ManagerGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.ManagerToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.ManagerGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblManagerToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@ManagerToAutoDistributionRuleGuid IS NULL) OR (@ManagerToAutoDistributionRuleGuid = MAIN.ManagerToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@ManagerGuid IS NULL) OR (@ManagerGuid = MAIN.ManagerGuid))

END