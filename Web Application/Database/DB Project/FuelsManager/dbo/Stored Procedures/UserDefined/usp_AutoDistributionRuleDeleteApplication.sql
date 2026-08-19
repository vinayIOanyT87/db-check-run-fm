
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete(cascade) a record from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteApplication] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@RowVersion TIMESTAMP = NULL
) AS
BEGIN
	EXEC [map].[usp_AutoDistributionRuleToSiteDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_ManagerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_OwnerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_ProductGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_ProductToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC usp_AutoDistributionRuleDeleteByRowGuid @AutoDistributionRuleGuid
END