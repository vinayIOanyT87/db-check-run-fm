
CREATE PROCEDURE [dbo].[usp_GetChildLineItems]
	@parentTransactionLineItemGuid UNIQUEIDENTIFIER
AS
BEGIN

	-- Checked for linked line items before executing the queries below. The query can be very expensive even if no links exist.
	IF NOT EXISTS (SELECT * FROM tblTransactionLinks WHERE TransactionLineItemGuid = @parentTransactionLineItemGuid)
	BEGIN
		RETURN
	END

	DECLARE @StartSiteGuid UNIQUEIDENTIFIER
	SELECT @StartSiteGuid = b.SiteGuid FROM [dbo].tblTransactionLineItems a 
	INNER JOIN tblTransactions b ON b.TransactionGuid = a.TransactionGuid 
	WHERE a.TransactionLineItemGuid = @parentTransactionLineItemGuid;

	WITH TransAssociationsTopDown (
			ParentTransactionLineItemGuid, ChildTransactionLineItemGuid, LinkedTransID, GrossQuantity, LookupTransactionStatusIndex, Product, ProductPrice, 
			LookupQualityIndex, Tax1, Tax2, Tax3, LookupTransTypeIndex, CheckQuantity, CheckValue, CheckQtyTolerance, CheckValueTolerance, AggregateChildren, Tier)
		AS (
			-- Anchor member definition
			SELECT 
				l.TransactionLineItemGuid, l.TransactionLineItemGuid, l.LinkedTransID, li.GrossQuantity, li.LookupTransactionStatusIndex, li.Product, li.ProductPrice,
				li.LookupQualityIndex, li.Tax1, li.Tax2, li.Tax3, t.LookupTransTypeIndex, a.EnableTotalQuantityExceededWarning, a.EnableTotalValueExceededWarning,
				a.EnableQuantityToleranceExceededWarning, a.EnableValueToleranceExceededWarning, a.AggregateAssocTrans, 0 AS Tier
			FROM 
				[dbo].tblTransactionLinks l JOIN [dbo].tblTransactionLineItems li ON l.TransactionLineItemGuid = li.TransactionLineitemGuid
				JOIN [dbo].tblTransactions t ON li.TransactionGuid = t.TransactionGuid
				JOIN [dbo].tblTransactionAliases a ON t.TransactionAliasGuid = a._MasterRecordGuid
				INNER JOIN [erv].[udf_GetTransactionAliasRecordVersions](@StartSiteGuid) c ON c.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE 
				l.TransactionLineItemGuid = @parentTransactionLineItemGuid
			
			UNION ALL
			-- Recursive member definition
			SELECT
				ta.ChildTransactionLineItemGuid, l.TransactionLineItemGuid, l.LinkedTransID, li.GrossQuantity, li.LookupTransactionStatusIndex, li.Product, li.ProductPrice,
				li.LookupQualityIndex, li.Tax1, li.Tax2, li.Tax3, t.LookupTransTypeIndex, a.EnableTotalQuantityExceededWarning, a.EnableTotalValueExceededWarning,
				a.EnableQuantityToleranceExceededWarning, a.EnableValueToleranceExceededWarning, a.AggregateAssocTrans, Tier + 1
			FROM
				TransAssociationsTopDown ta JOIN [dbo].tblTransactionLinks l ON ta.ChildTransactionLineItemGuid = l.TransactionLinkGuid
				JOIN [dbo].tblTransactionLineItems li ON l.TransactionLineItemGuid = li.TransactionLineitemGuid
				JOIN [dbo].tblTransactions t ON li.TransactionGuid = t.TransactionGuid
				JOIN [dbo].tblTransactionAliases a ON t.TransactionAliasGuid = a._MasterRecordGuid
				INNER JOIN [erv].[udf_GetTransactionAliasRecordVersions](@StartSiteGuid) c ON c.TransactionAliasGuid = a.TransactionAliasGuid
			)
				
		SELECT * FROM TransAssociationsTopDown
END


