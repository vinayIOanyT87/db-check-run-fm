

CREATE PROCEDURE [dbo].[usp_GetOneDaysPhysicalInventorySummationSelect]
@SiteGuid UNIQUEIDENTIFIER, @InventoryDate DATE, @ManagerCompanyGuid UNIQUEIDENTIFIER, @ProductGuid UNIQUEIDENTIFIER, @VolumeFactor FLOAT, @VolumePrecision FLOAT, @MassFactor FLOAT, @MassPrecision FLOAT, @TankGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @EmptyIdentifier UNIQUEIDENTIFIER
	SET @EmptyIdentifier = CONVERT(UNIQUEIDENTIFIER, '00000000-0000-0000-0000-000000000000')

	IF (@TankGuid <> @EmptyIdentifier)
	BEGIN
		SELECT
			SUM(ROUND(l.GrossQuantity * @VolumeFactor, @VolumePrecision))		AS GrossQuantity,
			SUM(ROUND(l.NetQuantity   * @VolumeFactor, @VolumePrecision))		AS NetQuantity,
			SUM(ROUND(l.MassQuantity  * @MassFactor, @MassPrecision))		    AS MassQuantity,
			SUM(l.ProductPrice * ROUND(l.GrossQuantity * @VolumeFactor, @VolumePrecision))	AS GrossPrice,
			SUM(l.ProductPrice * ROUND(l.NetQuantity   * @VolumeFactor, @VolumePrecision))	AS NetPrice,
			SUM(l.ProductPrice * ROUND(l.MassQuantity   * @MassFactor, @MassPrecision))	AS MassPrice
			FROM tblTransactionLineItems l WITH(NOLOCK)INNER JOIN tblTransactions t WITH(NOLOCK) ON l.TransactionGuid = t.TransactionGuid
			WHERE t.SiteGuid   = @SiteGuid
			AND t.InventoryDate = @InventoryDate
			AND t.ManagerCompanyGuid  = @ManagerCompanyGuid
			AND l.ProductGuid  = @ProductGuid
			AND (l.StorageLocationTankGuid = @TankGuid OR @TankGuid IS NULL)
			AND t.DeleteFlag    = cast(0 AS bit)
			AND t.LookupTransTypeIndex   = 14
	END
	ELSE
	BEGIN
			SELECT
			SUM(ROUND(l.GrossQuantity * @VolumeFactor, @VolumePrecision))		AS GrossQuantity,
			SUM(ROUND(l.NetQuantity   * @VolumeFactor, @VolumePrecision))		AS NetQuantity,
			SUM(ROUND(l.MassQuantity  * @MassFactor, @MassPrecision))		    AS MassQuantity,
			SUM(l.ProductPrice * ROUND(l.GrossQuantity * @VolumeFactor, @VolumePrecision))	AS GrossPrice,
			SUM(l.ProductPrice * ROUND(l.NetQuantity   * @VolumeFactor, @VolumePrecision))	AS NetPrice,
			SUM(l.ProductPrice * ROUND(l.MassQuantity   * @MassFactor, @MassPrecision))	AS MassPrice
			FROM tblTransactionLineItems l WITH(NOLOCK)INNER JOIN tblTransactions t WITH(NOLOCK) ON l.TransactionGuid = t.TransactionGuid
			WHERE t.SiteGuid   = @SiteGuid
			AND t.InventoryDate = @InventoryDate
			AND t.ManagerCompanyGuid  = @ManagerCompanyGuid
			AND l.ProductGuid  = @ProductGuid
			AND t.DeleteFlag    = cast(0 AS bit)
			AND t.LookupTransTypeIndex   = 14
	END
END