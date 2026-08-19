


CREATE VIEW [dbo].[vw_OrderSummary]
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
			B.UserData1,
			B.UserData2,
			B.UserData3,
			B.UserData4,
			B.UserData5,
			B.UserData6,
			B.UserData7,
			B.UserData8,
			B.UserData9,
			B.UserData10,
			B.UserData11,
			B.UserData12,
			B.UserData13,
			B.UserData14,
			B.UserData15,
			B.UserData16,
			B.UserData17,
			B.UserData18,
			B.UserData19,
			B.UserData20,
			B.UserData21,
			B.UserData22,
			B.UserData23,
			B.UserData24
	  FROM dbo.tblTransactions				A
	  LEFT JOIN dbo.tblTransactionUserData	B
	    ON A.TransactionGuid = B.TransactionGuid
	 WHERE A.LookupTransTypeIndex = 17;

