

CREATE VIEW [dbo].[vw_SupplyOrderSummaryProduct]
WITH SCHEMABINDING
AS
SELECT
			A.TransID						AS TransactionID,
			A.AliasName						AS TransactionAlias,
			A.LookupTransactionStatusIndex,
			A.TransDateTime				AS TransactionDate,
			A.InventoryDate,
			A.DocumentNumber,
			C.UserData1						AS ConfirmationNumber,
			A.PONumber,
			A.RequestedDeliveryDate		AS RequiredDeliveryDate,
			A.DeleteFlag,
			A.EffectiveDate				AS EstimatedDeliveryDateFrom,
			A.ExpirationDate				AS EstimatedDeliveryDateTo,
			A.Site,
			A.SiteGuid,
			A.LookupTransactionStatusIndex			AS TransStatus,
			A.SupplierID,
			A.ManagerID,
			A.OwnerID,
			A.BillToID,
			A.ShipperID,
			A.ShipToID,
			A.CarrierID,
			B.Product,
			B.TransactionLineItemGuid
	  FROM dbo.tblTransactions				A
	  JOIN dbo.tblTransactionLineItems	B
		 ON A.TransactionGuid = B.TransactionGuid
	  LEFT JOIN dbo.tblTransactionUserData	C
		 ON A.TransactionGuid = C.TransactionGuid
	 WHERE A.LookupTransTypeIndex = 18;

