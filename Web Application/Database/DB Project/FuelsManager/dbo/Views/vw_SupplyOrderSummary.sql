


CREATE VIEW [dbo].[vw_SupplyOrderSummary]
WITH SCHEMABINDING
AS
SELECT
			A.TransID					AS TransactionID,
			A.AliasName					AS TransactionAlias,
			A.LookupTransactionStatusIndex,
			A.TransDateTime			AS TransactionDate,
			A.InventoryDate,
			A.DocumentNumber,
			B.UserData1					AS ConfirmationNumber,
			A.PONumber,
			A.RequestedDeliveryDate	AS RequiredDeliveryDate,
			A.DeleteFlag,
			A.EffectiveDate			AS EstimatedDeliveryDateFrom,
			A.ExpirationDate			AS EstimatedDeliveryDateTo,
			A.Site,
			A.SiteGuid,
			A.LookupTransactionStatusIndex		AS TransStatus,
			A.SupplierID,
			A.ManagerID,
			A.OwnerID,
			A.BillToID,
			A.ShipperID,
			A.ShipToID,
			A.CarrierID,
			A.LookupTransTypeIndex
	  FROM dbo.tblTransactions				A
	  LEFT JOIN dbo.tblTransactionUserData	B
		 ON A.TransactionGuid = B.TransactionGuid
	 WHERE A.LookupTransTypeIndex = 18;

