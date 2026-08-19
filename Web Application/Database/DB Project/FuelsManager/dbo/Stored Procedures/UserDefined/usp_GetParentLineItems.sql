
CREATE PROCEDURE [dbo].[usp_GetParentLineItems]
	@ChildTransactionLineItemGuid UNIQUEIDENTIFIER
AS
BEGIN

	-- Checked for linked line items before executing the queries below. The query can be very expensive even if no links exist.
	IF NOT EXISTS (SELECT * FROM tblTransactionLinks WHERE LinkedTransactionLineItemGuid = @ChildTransactionLineItemGuid)
	BEGIN
		RETURN
	END

	DECLARE @startSiteIndex uniqueidentifier
	SELECT @startSiteIndex = b.SiteGuid FROM [dbo].tblTransactionLineItems a 
	INNER JOIN tblTransactions b ON b.TransactionGuid = a.TransactionGuid 
	WHERE a.TransactionLineItemGuid = @ChildTransactionLineItemGuid;

	WITH TransAssociationsBottomUp (
		ChildTransactionLineItemGuid, ParentTransactionLineItemGuid, LinkedTransID, GrossQuantity, LookupTransactionStatusIndex, Product, ProductPrice, 
		LookupQualityIndex, Tax1, Tax2, Tax3, LookupTransTypeIndex, CheckQuantity, CheckValue, CheckQtyTolerance, CheckValueTolerance, AggregateChildren, Tier)
	AS (
		-- Anchor member definition
		SELECT 
			l.LinkedTransactionLineItemGuid, l.TransactionLineItemGuid,  l.OriginalTransID, li.GrossQuantity, li.LookupTransactionStatusIndex, li.Product, li.ProductPrice,
			li.LookupQualityIndex, li.Tax1, li.Tax2, li.Tax3, t.LookupTransTypeIndex, a.EnableTotalQuantityExceededWarning, a.EnableTotalValueExceededWarning,
			a.EnableQuantityToleranceExceededWarning, a.EnableValueToleranceExceededWarning, a.AggregateAssocTrans, 0 AS Tier
		FROM 
			dbo.tblTransactionLinks l 
			JOIN dbo.tblTransactionLineItems li ON l.TransactionLineItemGuid = li.TransactionLineItemGuid
			JOIN dbo.tblTransactions t ON li.TransactionGuid = t.TransactionGuid
			JOIN dbo.tblTransactionAliases a ON t.TransactionAliasGuid = a._MasterRecordGuid
			INNER JOIN erv.udf_GetTransactionAliasRecordVersions (@startSiteIndex) b ON b.TransactionAliasGuid = a.TransactionAliasGuid
		WHERE 
			l.LinkedTransactionLineItemGuid = @ChildTransactionLineItemGuid
		
		UNION ALL
		-- Recursive member definition
		SELECT
			ta.ParentTransactionLineItemGuid, l.TransactionLineItemGuid, l.OriginalTransID, li.GrossQuantity, li.LookupTransactionStatusIndex, li.Product, li.ProductPrice,
			li.LookupQualityIndex, li.Tax1, li.Tax2, li.Tax3, t.LookupTransTypeIndex, a.EnableTotalQuantityExceededWarning, a.EnableTotalValueExceededWarning,
			a.EnableQuantityToleranceExceededWarning, a.EnableValueToleranceExceededWarning, a.AggregateAssocTrans, Tier + 1
		FROM
			TransAssociationsBottomUp ta 
			JOIN dbo.tblTransactionLinks l ON ta.ParentTransactionLineItemGuid = l.LinkedTransactionLineItemGuid
			JOIN dbo.tblTransactionLineItems li ON l.TransactionLineItemGuid = li.TransactionLineItemGuid
			JOIN dbo.tblTransactions t ON li.TransactionGuid = t.TransactionGuid
			JOIN dbo.tblTransactionAliases a ON t.TransactionAliasGuid = a._MasterRecordGuid
			INNER JOIN erv.udf_GetTransactionAliasRecordVersions (@startSiteIndex) b ON b.TransactionAliasGuid = a.TransactionAliasGuid
	)

	SELECT * FROM TransAssociationsBottomUp
END

