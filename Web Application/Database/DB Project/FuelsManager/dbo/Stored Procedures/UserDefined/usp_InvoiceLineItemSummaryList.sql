

CREATE PROCEDURE [dbo].[usp_InvoiceLineItemSummaryList]
@InvoiceNumber NVARCHAR (50), @InvoiceType INT, @AccountCode NVARCHAR (50), @CostCenterCode NVARCHAR (50), @ShipToID NVARCHAR (100), @SupplierID NVARCHAR (100), @ProductID NVARCHAR (30), @LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @DeleteFlag BIT, @StartDate NVARCHAR (10), @EndDate NVARCHAR (10), @SortExpression NVARCHAR (MAX)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SubSQL1 nvarchar(max)

	--
	-- This part retrieves unique Payment/Recovery transactions. If transaction has more than 1 transaction line item, it retireves only one 
	-- of the line items satisfying the filter criteria. Joins transaction and one of the selected line item.
	--
	SET @SubSQL1 = 'SELECT  
			BB.TransID, 
			BB.TransactionLineItemGuid, 
			BB.Site AS SiteID, 
			BB.Product,
			BB.InvoiceNumber,
			BB.InvoiceLineNumber, 
			BB.AccountCode,
			BB.CostCentreCode, 
			BB.BatchNumber,
			BB.LookupTransTypeIndex,
			BB.AliasName,
			BB.TransDateTime,
			BB.InventoryDate,
			BB.Site,
			BB.SiteGuid,
			BB.PONumber as OrderNumber,
			BB.LegacyNumber as PaymentNumber,
			BB.ManagerID,
			BB.OwnerID,
			BB.ShipmentNumber,
			BB.ContactInfo,
			BB.ContactSurname,
			BB.ContactFirstName,
			BB.ManagerCompanyGuid,
			BB.OwnerCompanyGuid,
			BB.TransactionAliasGuid,
			BB.DeleteFlag,
			BB.LookupTransactionStatusIndex, 
			BB.SupplierID,
			BB.ShipperID,
			BB.Flag02,
			(SELECT DocumentNumber FROM tblTransactions WHERE TransID = BB.TransID) AS DocumentNumber
			FROM vw_InvoiceSummaryProduct BB 
			WHERE  DeleteFlag = 0 AND ISNULL(BB.TransactionLineItemGuid, ''00000000-0000-0000-0000-000000000000'') IN 
				(SELECT ISNULL(B.TransactionLineItemGuid, ''00000000-0000-0000-0000-000000000000'') FROM vw_InvoiceSummaryProduct B '
	/* Recovery - Get Ship-to info from an associated Sale transaction. */
	IF (@InvoiceType = 22) 
		BEGIN
		SET @SubSQL1 = @SubSQL1 + '
			LEFT JOIN vwInvoiceSummaryLinkedTransactions C ON B.TransID = C.OriginalTransID AND B.TransactionLineItemGuid = C.TransactionLineItemGuid 
			LEFT JOIN dbo.tblTransactions D ON D.TransID = C.LinkedTransID
			LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) ShipTo ON D.ShipToCompanyGuid = ShipTo.CompanyGuid OR D.ShipToCompanyGuid = ShipTo._MasterRecordGuid '

		END
	ELSE
	/* Payment - Get supplier information*/
		SET @SubSQL1 = @SubSQL1 + ' LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Supplier ON B.SupplierCompanyGuid = Supplier.CompanyGuid OR B.SupplierCompanyGuid = Supplier._MasterRecordGuid '

	SET @SubSQL1 = @SubSQL1 + ' WHERE BB.TransactionGuid = B.TransactionGuid  '
 
	--
	-- Part 1 filter criterias
	--
	IF (@StartDate <> '')
	 BEGIN
		SET @SubSQL1 = @SubSQL1 + ' AND (B.InventoryDate >= ''' + @StartDate + ''')'
	 END

	/* End Date */
	IF (@EndDate <> '')
	 BEGIN
		SET @SubSQL1 = @SubSQL1 + ' AND (B.InventoryDate <= ''' + @EndDate + ''')'
	 END

	/* Add Supplier */
	IF @SupplierID <> '' SET @SubSQL1 = @SubSQL1 + ' AND (B.SupplierID = ''' + @SupplierID + ''')'

	/* Add Invoice type (trans type ID) */
	IF ((@InvoiceType = 21) OR (@InvoiceType = 22))
	  BEGIN
		SET @SubSQL1 = @SubSQL1 + ' AND (B.LookupTransTypeIndex = ' + STR(@InvoiceType) + ')'
	  END
	ELSE
	  BEGIN
		SET @SubSQL1 = @SubSQL1 + ' AND (B.LookupTransTypeIndex = 21)'
	  END

	IF (@ProductID <> '') SET @SubSQL1 = @SubSQL1 + ' AND (B.Product = ''' + @ProductID + ''')'

	/* Add invoice number */
	IF @InvoiceNumber <> 'N/A' SET @SubSQL1 = @SubSQL1 + ' AND (B.InvoiceNumber LIKE ''' + @InvoiceNumber + '%'')'

	/* Add Account code */
	IF @AccountCode <> '' SET @SubSQL1 = @SubSQL1 + ' AND (B.AccountCode = ''' + @AccountCode + ''')'

	/* Add cost center code */
	IF @CostCenterCode <> '' SET @SubSQL1 = @SubSQL1 + ' AND (B.CostCentreCode = ''' + @CostCenterCode + ''')'
	/* Add Authorized Lists */
	IF @UserGuid IS NOT NULL 
		BEGIN 
			SET @SubSQL1 = @SubSQL1 + 
			' AND (
			(  (B.CarrierID  IN (Select * FROM @AuthorizedCompanies))
			OR (B.ShipperID  IN (Select * FROM @AuthorizedCompanies))
			OR (B.ManagerID  IN (Select * FROM @AuthorizedCompanies))
			OR (B.OwnerID    IN (Select * FROM @AuthorizedCompanies))
			OR (B.BillToID   IN (Select * FROM @AuthorizedCompanies)) '

			
			IF (@InvoiceType = 22)/*Recovery*/
				SET @SubSQL1 = @SubSQL1 + ' OR (D.ShipToID IN (Select * FROM @AuthorizedCompanies)) '
			ELSE/*Payment*/
				SET @SubSQL1 = @SubSQL1 + ' OR (B.SupplierID IN (Select * FROM @AuthorizedCompanies)) '

			SET @SubSQL1 = @SubSQL1 + '))'
		END
	SET @SubSQL1 = @SubSQL1 + ') '

	--
	-- Part2 retrieves aggregated columns such as quantity, product price, and etc.
	--
	DECLARE @SubSQL2 nvarchar(max)
	SET @SubSQL2 = '
			SELECT TOP 500
			A.TransID,
			A.TransactionLineItemGuid,
			(A.Tax1) AS Excise,
			(A.Tax2) AS GST,
			(A.GrossQuantity) AS GrossQuantity,
			(A.NetQuantity) AS NetQuantity,
			(A.ProductPrice) AS ProductPrice,
			((ProductPrice * GrossQuantity) + ISNULL(Tax1, 0) + ISNULL(Tax2, 0)) as TotalAmount
			FROM vw_InvoiceSummaryProduct A WHERE DeleteFlag = 0
				'

	DECLARE @TempStr nvarchar(max)

	SET @TempStr = 
	   'DECLARE @AuthorizedCompanies TABLE ([ID] [nvarchar] (100) NOT NULL); '
	SET @TempStr = @TempStr +
	   'INSERT INTO @AuthorizedCompanies SELECT * FROM udf_AuthorizedCompanies(''' + 
       CAST(@LoginSiteGuid AS VARCHAR(50)) + ''', ''' + CAST(@SiteGuid AS VARCHAR(50)) + ''', ''' + CAST(@UserGuid AS VARCHAR(50)) + '''); '



	SET @TempStr = @TempStr + '
			SELECT XXX.*, YYY.*,
			(SELECT MAX(D.ShipToID) FROM vwInvoiceSummaryLinkedTransactions C 
			LEFT JOIN dbo.tblTransactions D ON D.TransID = C.LinkedTransID          
			WHERE YYY.TransID = C.OriginalTransID)  AS ShipToID
			FROM (' + @SubSQL1 + ') XXX INNER JOIN (' + @SubSQL2 + ') YYY ON XXX.TransactionLineItemGuid = YYY.TransactionLineItemGuid '			


	/* Set the sort expression */
	IF @SortExpression <> '' SET @TempStr = @TempStr + ' ' + @SortExpression
--select @SubSQL1, @SubSQL2
--select @TempStr
	/* Execute the query */

	EXEC sp_executesql @TempStr
END



