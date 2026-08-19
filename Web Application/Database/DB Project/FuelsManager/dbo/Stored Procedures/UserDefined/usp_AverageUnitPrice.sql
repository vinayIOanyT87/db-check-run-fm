

CREATE PROCEDURE [dbo].[usp_AverageUnitPrice]
@SiteGuid UNIQUEIDENTIFIER, @StartingDate DATE, @endDate DATE, @SupplierCompanyGuid UNIQUEIDENTIFIER, @ProductGuid UNIQUEIDENTIFIER
AS
SET NOCOUNT ON

	DECLARE @ReceiptTable TABLE (GrossQuantity float, NetQuantity float, GrossQuantityUnitPrice float, NetQuantityUnitPrice float)
	DECLARE @AverageTable TABLE (AverageGrossQuantity float, AverageNetQuantity float, 
								AverageGrossQuantityUnitPrice float, AverageNetQuantityUnitPrice float)

	IF (@SupplierCompanyGuid IS NOT NULL)
	BEGIN
		INSERT INTO @ReceiptTable
		SELECT l.GrossQuantity, l.NetQuantity, (l.GrossQuantity * l.ProductPrice), (l.NetQuantity * l.ProductPrice)
		FROM dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
		WHERE l.ProductGuid = @ProductGuid AND
			l.LookupQualityIndex = 1 AND -- usable
			l.DeleteFlag <> 1 AND
			l.LookupTransactionStatusIndex IN (0, 12) AND -- Completed
			t.InventoryDate BETWEEN @StartingDate AND @endDate AND
			t.SiteGuid = @SiteGuid AND
			t.LookupTransTypeIndex IN (8)
	END
	ELSE
	BEGIN
		INSERT INTO @ReceiptTable
		SELECT l.GrossQuantity, l.NetQuantity, (l.GrossQuantity * l.ProductPrice), (l.NetQuantity * l.ProductPrice)
		FROM dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
		WHERE t.SupplierCompanyGuid = @SupplierCompanyGuid AND
			l.ProductGuid = @ProductGuid AND
			l.LookupQualityIndex = 1 AND -- usable
			l.DeleteFlag <> 1 AND
			l.LookupTransactionStatusIndex IN (0, 12) AND -- Completed
			t.InventoryDate BETWEEN @StartingDate AND @endDate AND
			t.SiteGuid = @SiteGuid AND
			t.LookupTransTypeIndex IN (8)
	END

	INSERT INTO @AverageTable
	SELECT SUM(GrossQuantity), SUM(NetQuantity), SUM(GrossQuantityUnitPrice), SUM(NetQuantityUnitPrice)
	FROM @ReceiptTable

	SELECT 
		ISNULL((CASE AverageGrossQuantity WHEN 0 THEN 0 ELSE (AverageGrossQuantityUnitPrice / AverageGrossQuantity) END ), 0) AS AverageGrossUnitPrice, 
		ISNULL((CASE AverageNetQuantity WHEN 0 THEN 0 ELSE (AverageNetQuantityUnitPrice / AverageNetQuantity) END ), 0) AS AverageNetUnitPrice

	FROM @AverageTable