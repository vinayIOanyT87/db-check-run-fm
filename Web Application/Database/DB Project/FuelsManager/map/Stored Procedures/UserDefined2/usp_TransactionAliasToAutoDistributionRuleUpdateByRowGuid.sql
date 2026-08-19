
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleUpdateByRowGuid] (
	@TransactionAliasToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblTransactionAliasToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, TransactionAliasGuid = @TransactionAliasGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		TransactionAliasToAutoDistributionRuleGuid = @TransactionAliasToAutoDistributionRuleGuid
END