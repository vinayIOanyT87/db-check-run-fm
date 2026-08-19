

/* 
=============================================
Author: Ryan Hill
Create date: 4/24/12

Description:	

This function calculates and returns the transaction total for a specific meter

The Transaction total is the sum of all meter movements for transactions that occured on the specified inventory date.
Meter closeout transactions are not counted.
=============================================
*/
CREATE FUNCTION [dbo].[udf_MeterReconciliationCalculateTransactionTotal]
(
	@InventoryDate DATE, --the day to calculate the transaction total for
	@MeterGuid UNIQUEIDENTIFIER, --the meter to calculate the transaction total for
	@SiteGuid UNIQUEIDENTIFIER, --the site we are calculating the transaction total for
	@CloseoutTransactionAliasGuid UNIQUEIDENTIFIER --the transaction alias which is the meter closeout transaction
)
RETURNS FLOAT
AS
BEGIN
	DECLARE @TransactionTotal FLOAT

	--calculate the transaction total for transaction line items
	SET @TransactionTotal = (SELECT 
		ISNULL(SUM(dbo.udf_MeterReconciliationCalculateMeterTotal(tblMeter.RotatesBackwardsFlag, tblMeter.NumberOfDigits, MeterStart, MeterStop)), 0)
	FROM tblMeter INNER JOIN tblTransactionLineItems ON tblMeter.MeterID = tblTransactionLineItems.MeterID
	INNER JOIN tblTransactions ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid
	WHERE tblTransactions.InventoryDate = @InventoryDate
		AND tblTransactions.SiteGuid = @SiteGuid
		AND tblMeter.MeterGuid = @MeterGuid
		AND ( tblTransactions.ReversalType IS NULL OR tblTransactions.ReversalType = 'O' )
		AND MeterStop IS NOT NULL AND MeterStart IS NOT NULL -- if there's no meter start or stop, we can't calculate a total
		AND tblTransactions.TransactionAliasGuid <> @CloseoutTransactionAliasGuid --ignore meter closeout transactions
		AND (tblTransactionLineItems.DeleteFlag = 0 OR tblTransactionLineItems.DeleteFlag IS NULL)) --ignore deleted records

	--calculate the transaction total for transaction sub line items
	SET @TransactionTotal = @TransactionTotal + (SELECT 
		ISNULL(SUM(dbo.udf_MeterReconciliationCalculateMeterTotal(tblMeter.RotatesBackwardsFlag, tblMeter.NumberOfDigits, MeterStart, MeterStop)), 0)
	FROM tblMeter INNER JOIN tblTransactionSubLineItems ON tblMeter.MeterID = tblTransactionSubLineItems.MeterID
	INNER JOIN tblTransactions ON tblTransactionSubLineItems.TransactionGuid = tblTransactions.TransactionGuid
	WHERE tblTransactions.InventoryDate = @InventoryDate
		AND tblTransactions.SiteGuid = @SiteGuid
		AND tblMeter.MeterGuid = @MeterGuid
		AND ( tblTransactions.ReversalType IS NULL OR tblTransactions.ReversalType = 'O' )
		AND MeterStop IS NOT NULL AND MeterStart IS NOT NULL -- if there's no meter start or stop, we can't calculate a total
		AND tblTransactions.TransactionAliasGuid <> @CloseoutTransactionAliasGuid --ignore meter closeout transactions
		AND (tblTransactionSubLineItems.DeleteFlag = 0 OR tblTransactionSubLineItems.DeleteFlag IS NULL)) --ignore deleted records

	RETURN @TransactionTotal
END	

