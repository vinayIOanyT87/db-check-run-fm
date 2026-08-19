
CREATE VIEW [dbo].[vw_InvoiceSummaryLinkedTransactions]
AS
SELECT tlk.OriginalTransID,
			 tlk.LinkedTransID,
			 t.ShipToID,
			 t.ShipToCompanyGuid,
			 t.SupplierID,
			 t.SupplierCompanyGuid,
			 TransactionLineItemGuid			AS LineItemIndex
	  FROM dbo.tblTransactionLinks				AS tlk
	  LEFT OUTER JOIN dbo.tblTransactions		AS t
	    ON tlk.LinkedTransID = t.TransID
	 WHERE t.LookupTransTypeIndex IN (5, 8);