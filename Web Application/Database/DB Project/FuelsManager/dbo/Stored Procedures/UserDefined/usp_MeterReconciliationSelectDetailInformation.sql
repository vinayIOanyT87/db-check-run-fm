
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12
Description:
This procedure retrieves all transactions which occurred for a specific meter during the inventory date specified.

Transactions that are meter closeout transactions are ignored
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterReconciliationSelectDetailInformation]
	@InventoryDate DATE, --Required. The inventory date to view transactions for
	@MeterGuid UNIQUEIDENTIFIER, --Required. All transactions, except meter closeouts, for this meter will be returned
	@SiteGuid UNIQUEIDENTIFIER --Required. The site to get the transactions from
AS
BEGIN
	SET NOCOUNT ON

	--determine the transaction alias which is the meter closeout transaction
	DECLARE @CloseoutTransactionAliasGuid UNIQUEIDENTIFIER
	SELECT @CloseoutTransactionAliasGuid = a.MasterRecordGuid FROM [erv].[udf_GetTransactionAliasRecordVersions](@SiteGuid) a
	INNER JOIN tblTransactionAliases b
	ON b.TransactionAliasGuid = a.TransactionAliasGuid
	WHERE b.MeterCloseout = 1 AND b.LookupTransTypeIndex = 12

	--we'll be getting all transactions which use the meter matching @MeterGuid and aren't closeout transactions
	--transaction line items can use a meter, and so can transaction sub line items. So we use a UNION here to get both.
	SELECT * FROM (
		SELECT
			tblTransactions.TransID,
			tblTransactions.InventoryDate,
			tblTransactionLineItems.Product,
			MeterStart = tblTransactionLineItems.MeterStart,
			MeterStop = tblTransactionLineItems.MeterStop,
			MeterTotal = dbo.udf_MeterReconciliationCalculateMeterTotal(tblMeter.RotatesBackwardsFlag, tblMeter.NumberOfDigits, 
						tblTransactionLineItems.MeterStart, tblTransactionLineItems.MeterStop),
			Carrier = tblTransactions.CarrierID,
			StationID = tblTransactionLineItems.LoadingLocationID,
			TransactionAlias = tblTransactions.AliasName,
			FlightNumber = tblTransactions.RoutingID, 
			TicketNumber = tblTransactions.DocumentNumber, 
			tblMeter.NumberOfDigits,
			tblMeter.RotatesBackwardsFlag,
			tblTransactions.TransactionGuid,
			abs(dbo.udf_ConvertFromSIUnits(GrossQuantity,
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
																			tblSites.AdditiveVolumeDecimalPlaces))) as GrossVolume
		FROM tblMeter INNER JOIN tblTransactionLineItems 
			ON tblTransactionLineItems.MeterID = tblMeter.MeterID
		INNER JOIN tblTransactions 
			ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid
			AND ( tblTransactions.ReversalType IS NULL OR tblTransactions.ReversalType = 'O' )
		inner join erv.udf_GetProductRecordVersions(@SiteGuid) prv
			on prv.MasterRecordGuid = tblTransactionLineItems.ProductGuid
		INNER JOIN tblProducts
			ON tblProducts.ProductGuid = prv.ProductGuid -- get the site-specific product version for units and precision
		INNER JOIN tblSites
			on tblTransactions.SiteGuid = tblSites.SiteGuid
		INNER JOIN tblTransactionAliases
			on tblTransactions.TransactionAliasGuid = tblTransactionAliases._MasterRecordGuid
		WHERE tblMeter.MeterGuid = @MeterGuid
			AND MeterStart IS NOT NULL AND MeterStop IS NOT NULL
			AND tblMeter.SiteGuid = @SiteGuid
			AND tblTransactions.SiteGuid = @SiteGuid
			AND tblTransactions.InventoryDate = @InventoryDate
			AND tblTransactions.TransactionAliasGuid <> @CloseoutTransactionAliasGuid -- ignore meter closeout transactions
			AND (tblTransactionLineItems.DeleteFlag = 0 OR tblTransactionLineItems.DeleteFlag IS NULL)--ignore deleted transactions
		UNION ALL -- use a UNION ALL here since we don't want to de-dupe the result set
		SELECT
			tblTransactions.TransID,
			tblTransactions.InventoryDate,
			tblTransactionSubLineItems.Product,
			MeterStart = tblTransactionSubLineItems.MeterStart,
			MeterStop = tblTransactionSubLineItems.MeterStop,
			MeterTotal = dbo.udf_MeterReconciliationCalculateMeterTotal(tblMeter.RotatesBackwardsFlag, tblMeter.NumberOfDigits, 
						tblTransactionSubLineItems.MeterStart, tblTransactionSubLineItems.MeterStop),
			Carrier = tblTransactions.CarrierID,
			StationID = NULL,
			TransactionAlias = tblTransactions.AliasName,
			FlightNumber = tblTransactions.RoutingID,
			TicketNumber = tblTransactions.DocumentNumber,
			tblMeter.NumberOfDigits,
			tblMeter.RotatesBackwardsFlag,
			tblTransactions.TransactionGuid,
			abs(dbo.udf_ConvertFromSIUnits(GrossQuantity,
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
																			tblSites.AdditiveVolumeDecimalPlaces)))
		FROM tblMeter 
		INNER JOIN tblTransactionSubLineItems 
			ON tblTransactionSubLineItems.MeterID = tblMeter.MeterID
		INNER JOIN tblTransactions 
			ON tblTransactionSubLineItems.TransactionGuid = tblTransactions.TransactionGuid
			AND ( tblTransactions.ReversalType IS NULL OR tblTransactions.ReversalType = 'O' )
		inner join erv.udf_GetProductRecordVersions(@SiteGuid) prv
			on prv.MasterRecordGuid = tblTransactionSubLineItems.ProductGuid
		INNER JOIN tblProducts
			ON tblProducts.ProductGuid = prv.ProductGuid -- get the site-specific product version for units and precision
		INNER JOIN tblSites
			on tblTransactions.SiteGuid = tblSites.SiteGuid
		INNER JOIN tblTransactionAliases
			on tblTransactions.TransactionAliasGuid = tblTransactionAliases._MasterRecordGuid
		WHERE tblMeter.MeterGuid = @MeterGuid
			AND MeterStart IS NOT NULL AND MeterStop IS NOT NULL
			AND tblMeter.SiteGuid = @SiteGuid
			AND tblTransactions.SiteGuid = @SiteGuid
			AND tblTransactions.InventoryDate = @InventoryDate
			AND tblTransactions.TransactionAliasGuid <> @CloseoutTransactionAliasGuid -- ignore meter closeout transactions
			AND (tblTransactionSubLineItems.DeleteFlag = 0 OR tblTransactionSubLineItems.DeleteFlag IS NULL)--ignore deleted transactions
		) AS Results
	ORDER BY CASE WHEN RotatesBackwardsFlag = 0 THEN MeterStart END, CASE WHEN RotatesBackwardsFlag = 1 THEN MeterStart END DESC

END

