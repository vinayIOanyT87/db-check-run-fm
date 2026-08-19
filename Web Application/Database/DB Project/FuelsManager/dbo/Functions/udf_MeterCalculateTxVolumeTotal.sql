

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
CREATE FUNCTION [dbo].[udf_MeterCalculateTxVolumeTotal]
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
		ISNULL(ABS(SUM(dbo.udf_ConvertFromSIUnits(tblTransactionLineItems.GrossQuantity,
												dbo.udf_GetVolumeUnitsIndex(tblProducts.LookupProductTypeIndex,
																			tblProducts.VolumeUnitIndex,
																			tblTransactionAliases.VolumeUnitIndex,
																			tblSites.VolumeUnitIndex,
																			tblTransactionAliases.AdditiveVolumeUnitIndex,
																			tblSites.AdditiveVolumeUnitIndex),
												dbo.udf_GetVolumeDecimalPlaces(tblProducts.LookupProductTypeIndex,
																			tblProducts.VolumeDecimalPlaces,
																			tblTransactionAliases.VolumeDecimalPlaces,
																			tblSites.VolumeDecimalPlaces,
																			tblTransactionAliases.AdditiveVolumeDecimalPlaces,
																			tblSites.AdditiveVolumeDecimalPlaces)))),
				0)
	FROM tblMeter 
		INNER JOIN tblTransactionLineItems ON tblMeter.MeterID = tblTransactionLineItems.MeterID
		INNER JOIN tblTransactions ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid
		INNER JOIN tblSites on tblSites.SiteGuid = tblTransactions.SiteGuid
		INNER JOIN tblTransactionAliases on tblTransactionAliases.TransactionAliasGuid = tblTransactions.TransactionAliasGuid
		INNER JOIN tblProducts on tblProducts.ProductGuid = tblTransactionLineItems.ProductGuid
	WHERE tblTransactions.InventoryDate = @InventoryDate
		AND tblTransactions.SiteGuid = @SiteGuid
		AND tblMeter.MeterGuid = @MeterGuid
		AND ( tblTransactions.ReversalType IS NULL OR tblTransactions.ReversalType = 'O' )
		AND tblTransactions.TransactionAliasGuid <> @CloseoutTransactionAliasGuid --ignore meter closeout transactions
		AND (tblTransactionLineItems.DeleteFlag = 0 OR tblTransactionLineItems.DeleteFlag IS NULL)) --ignore deleted records

	--calculate the transaction total for transaction sub line items
	SET @TransactionTotal = @TransactionTotal + (SELECT 
		ISNULL(SUM(tblTransactionSubLineItems.GrossQuantity), 0)
	FROM tblMeter INNER JOIN tblTransactionSubLineItems ON tblMeter.MeterID = tblTransactionSubLineItems.MeterID
	INNER JOIN tblTransactions ON tblTransactionSubLineItems.TransactionGuid = tblTransactions.TransactionGuid
	WHERE tblTransactions.InventoryDate = @InventoryDate
		AND tblTransactions.SiteGuid = @SiteGuid
		AND tblMeter.MeterGuid = @MeterGuid
		AND ( tblTransactions.ReversalType IS NULL OR tblTransactions.ReversalType = 'O' )
		AND tblTransactions.TransactionAliasGuid <> @CloseoutTransactionAliasGuid --ignore meter closeout transactions
		AND (tblTransactionSubLineItems.DeleteFlag = 0 OR tblTransactionSubLineItems.DeleteFlag IS NULL)) --ignore deleted records

	RETURN @TransactionTotal
END	

