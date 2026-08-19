
CREATE PROCEDURE [dbo].[usp_SupplyOrderSummaryList]
@AliasName NVARCHAR (200), @ManagerID NVARCHAR (100), @OwnerID NVARCHAR (100), @Product NVARCHAR (100), @ShipperID NVARCHAR (100), @SupplierID NVARCHAR (100), @Status INT, @LookupTransTypeIndex SMALLINT, @LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @MoreWhereClause NVARCHAR (MAX)
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @TempStr nvarchar(max)

	SET @TempStr = 'DECLARE @AuthorizedCompanies TABLE (
		[ID] [nvarchar] (100) NOT NULL
	);

	INSERT INTO @AuthorizedCompanies SELECT * FROM udf_AuthorizedCompanies('''+CAST(@LoginSiteGuid AS VARCHAR(50))+''','''+CAST(@SiteGuid AS VARCHAR(50))+''','
	
	IF @UserGuid IS NOT NULL
		SET @TempStr = @TempStr + '''' + CAST(@UserGuid AS VARCHAR(50)) +''')'
	ELSE
		SET @TempStr = @TempStr + 'NULL'+')'

	IF ( @Product = '' )
	BEGIN
		SET @TempStr = @TempStr + '
		SELECT TOP 500
				A.TransactionID,
				A.TransactionAlias,
				A.LookupTransactionStatusIndex,
				A.TransactionDate,
				A.InventoryDate,
				A.DocumentNumber,
				A.ConfirmationNumber,
				A.PONumber,
				A.RequiredDeliveryDate,
				A.DeleteFlag,
				A.EstimatedDeliveryDateFrom,
				A.EstimatedDeliveryDateTo,
				A.Site,
				A.TransStatus,
				A.SupplierID, A.ManagerID, A.OwnerID, A.BillToID,
				A.ShipperID, A.ShipToID, A.CarrierID
			FROM vw_SupplyOrderSummary A
		'

	END 
	ELSE BEGIN
		SET @TempStr = @TempStr + '
			SELECT TOP 500
				A.TransactionID,
				A.TransactionAlias,
				A.LookupTransactionStatusIndex,
				A.TransactionDate,
				A.InventoryDate,
				A.DocumentNumber,
				A.ConfirmationNumber,
				A.PONumber,
				A.RequiredDeliveryDate,
				A.DeleteFlag,
				A.EstimatedDeliveryDateFrom,
				A.EstimatedDeliveryDateTo,
				A.Site,
				A.TransStatus,
				A.SupplierID, A.ManagerID, A.OwnerID, A.BillToID,
				A.ShipperID, A.ShipToID, A.CarrierID
			FROM vw_SupplyOrderSummaryProduct A
		'
	END 

	SET @TempStr = @TempStr + ' WHERE 1=1 '

	IF ( @Product <> '' )
	  BEGIN
		  SET @TempStr = @TempStr + ' AND (A.Product = '''+ @Product + ''')'
	  END

	/* Add Alias */
	IF @AliasName <> '' SET @TempStr = @TempStr + ' AND (A.TransactionAlias = '''+ @AliasName + ''')'

	/* Add Manager */
	IF @ManagerID <> '' SET @TempStr = @TempStr + ' AND (A.ManagerID = ''' + @ManagerID + ''')'

	/* Add Owner */
	IF @OwnerID <> '' SET @TempStr = @TempStr + ' AND (A.OwnerID = ''' + @OwnerID + ''')'

	/* Add Shipper */
	IF @ShipperID <> '' SET @TempStr = @TempStr + ' AND (A.ShipperID = ''' + @ShipperID + ''')'

	/* Add Supplier */
	IF @SupplierID <> '' SET @TempStr = @TempStr + ' AND (A.SupplierID = ''' + @SupplierID + ''')'

	/* Add Transaction Status */
	IF @Status <> -1 SET @TempStr = @TempStr + ' AND (A.LookupTransactionStatusIndex = ' + STR(@Status) + ')'

	/* Add Authorized Lists */
	IF @UserGuid IS NOT NULL SET @TempStr = @TempStr + ' AND (
			((A.ShipperID IN (Select * FROM @AuthorizedCompanies))
			OR (A.SupplierID IN (Select * FROM @AuthorizedCompanies))
			OR (A.ManagerID IN (Select * FROM @AuthorizedCompanies))
			OR (A.OwnerID IN (Select * FROM @AuthorizedCompanies))))
		'

	SET @TempStr = @TempStr + ' ' + @MoreWhereClause
	EXEC sp_executesql @TempStr
END
