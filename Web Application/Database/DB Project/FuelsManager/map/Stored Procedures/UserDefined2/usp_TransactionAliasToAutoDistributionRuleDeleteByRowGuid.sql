
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByRowGuid]
	@TransactionAliasToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblTransactionAliasToAutoDistributionRule]
			WHERE [TransactionAliasToAutoDistributionRuleGuid] = @TransactionAliasToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of TransactionAliasToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblTransactionAliasToAutoDistributionRule] 
	WHERE [TransactionAliasToAutoDistributionRuleGuid] = @TransactionAliasToAutoDistributionRuleGuid; 
END