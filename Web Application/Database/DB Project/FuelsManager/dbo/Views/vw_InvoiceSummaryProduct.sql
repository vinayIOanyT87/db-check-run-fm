

CREATE VIEW [dbo].[vw_InvoiceSummaryProduct]
AS
SELECT		t.TransID,
			t.LookupTransTypeIndex,
			t.AliasName,
			t.TransDateTime,
			t.InventoryDate,
			t.Site,
			t.SiteGuid,
			t.PONumber,
			l.InvoiceNumber,
			l.InvoiceLineNumber,
			l.Tax1,
			l.Tax2,
			t.LegacyNumber,
			u.UserData13        AS AccountCode,
			u.UserData1         AS CostCentreCode,
			t.ManagerID,
			t.OwnerID,
			t.SupplierID,
			l.BatchNumber,
			t.ShipmentNumber,
			l.Product,
			l.GrossQuantity,
			l.NetQuantity,
			l.ProductPrice,
			t.ContactInfo,
			t.ContactSurname,
			t.ContactFirstName,
			t.SupplierCompanyGuid,
			t.ManagerCompanyGuid,
			t.OwnerCompanyGuid,
			t.TransactionAliasGuid,
			t.DeleteFlag,
			t.LookupTransactionStatusIndex,
			t.CarrierID,
			t.ShipperID,
			t.BillToID,
			l.TransactionLineItemGuid,
			t.Flag02,
			t.TransactionGuid,
			l.CreatedDate
	 FROM dbo.tblTransactions										AS t
	 LEFT OUTER JOIN dbo.tblTransactionLineItems				AS l
	   ON t.TransactionGuid = l.TransactionGuid
	 LEFT OUTER JOIN dbo.tblTransactionLineItemUserData	AS u
	   ON l.TransactionLineItemGuid = u.TransactionLineItemGuid
	WHERE t.LookupTransTypeIndex = 21
		OR t.LookupTransTypeIndex = 22;