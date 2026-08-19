

CREATE VIEW [dbo].[vw_OrderSummaryProduct]
WITH SCHEMABINDING
AS
SELECT
			A.TransID				AS TransactionID,
			A.AliasName				AS TransactionAlias,
			A.LookupTransactionStatusIndex,
			A.TransDateTime		AS TransactionDate,
			A.InventoryDate,
			A.SupplierID, 
			A.ManagerID, 
			A.OwnerID, 
			A.BillToID,  
			A.ShipperID, 
			A.ShipToID, 
			A.CarrierID,
			A.DocumentNumber,
			A.PONumber,
			A.ScheduledDate,
			A.DeleteFlag,
			A.EffectiveDate,
			A.ExpirationDate,
			A.Site,
			A.SiteGuid,
			A.LookupTransactionStatusIndex	AS TransStatus,
			A.BillToCompanyGuid,
			A.ShipToCompanyGuid,
			A.CarrierCompanyGuid,
			A.LookupTransTypeIndex,
			A.ETA,
			A.RequestedDeliveryDate,
			A.ShipmentNumber,
			A.OperatorID,
			A.DestinationRegistrationID1,
			A.DestinationRegistrationID2,
			A.DestinationRegistrationID3,
			B.Product,
			B.TransactionLineItemGuid,
			C.UserData1,
			C.UserData2,
			C.UserData3,
			C.UserData4,
			C.UserData5,
			C.UserData6,
			C.UserData7,
			C.UserData8,
			C.UserData9,
			C.UserData10,
			C.UserData11,
			C.UserData12,
			C.UserData13,
			C.UserData14,
			C.UserData15,
			C.UserData16,
			C.UserData17,
			C.UserData18,
			C.UserData19,
			C.UserData20,
			C.UserData21,
			C.UserData22,
			C.UserData23,
			C.UserData24
	  FROM dbo.tblTransactions				A
	  JOIN dbo.tblTransactionLineItems	B
	    ON A.TransactionGuid = B.TransactionGuid
	  LEFT JOIN dbo.tblTransactionUserData	C
	    ON A.TransactionGuid = C.TransactionGuid
	 WHERE A.LookupTransTypeIndex = 17;


