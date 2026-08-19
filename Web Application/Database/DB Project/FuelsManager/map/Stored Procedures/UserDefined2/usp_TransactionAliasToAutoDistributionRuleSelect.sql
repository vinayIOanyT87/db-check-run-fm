
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@TransactionAliasToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@TransactionAliasGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.TransactionAliasToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.TransactionAliasGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblTransactionAliasToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@TransactionAliasToAutoDistributionRuleGuid IS NULL) OR (@TransactionAliasToAutoDistributionRuleGuid = MAIN.TransactionAliasToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@TransactionAliasGuid IS NULL) OR (@TransactionAliasGuid = MAIN.TransactionAliasGuid))

END