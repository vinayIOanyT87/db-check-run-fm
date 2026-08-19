DECLARE @StartDate DATETIME
SET @StartDate = '01/01/2017'


--hold list of trx to update
DECLARE @TrxTable TABLE
(
	SourceRegistrationID1 nvarchar(30),
	SiteGuid UNIQUEIDENTIFIER
)

--Get list of Trx to update
INSERT INTO @TrxTable 
SELECT Distinct T.SourceRegistrationID1, T.SiteGuid   
from tblTransactions T 
join tblTransactionLineItems TLI on TLI.TransactionGuid = T.TransactionGuid 
JOIN tblMeter M ON M.MeterID = T.SourceRegistrationID1  AND M.SiteGuid = T.SiteGuid  
JOIN map.tblMeterToEquipment MTE on MTE.MeterGuid = M.MeterGuid 
where T.InventoryDate >= @StartDate 
and T.SourceRegistrationID1 IS NOT NULL 
and TLI.MeterID IS NULL 

--update tblTransactionLineItems with meter information
UPDATE tblTransactionLineItems  
SET MeterGuid = M.MeterGuid, MeterID = T.SourceRegistrationID1 
FROM tblTransactionLineItems TLI 
JOIN tblTransactions T ON T.TransactionGuid = TLI.TransactionGuid 
JOIN tblMeter M ON M.MeterID = T.SourceRegistrationID1  AND M.SiteGuid = T.SiteGuid 
JOIN @TrxTable TT on TT.SourceRegistrationID1 = T.SourceRegistrationID1 AND TT.SiteGuid = T.SiteGuid 
WHERE T.InventoryDate >= @StartDate 


--update tblTransactions with Equipment information
UPDATE tblTransactions 
SET SourceRegistrationID1 = E.[ID], Source1EquipmentGuid = E.EquipmentGuid, 
SourceEquipmentType1 = ET.EqTypeName 
FROM tblTransactions T 
JOIN tblMeter M ON M.MeterID = T.SourceRegistrationID1  AND M.SiteGuid = T.SiteGuid 
JOIN map.tblMeterToEquipment  MTE on MTE.MeterGuid = M.MeterGuid 
JOIN tblEquipment E on E.EquipmentGuid = MTE.EquipmentGuid 
JOIN tblEquipmentTypes ET on ET.EquipmentTypeGuid = E.EquipmentTypeGuid  
JOIN @TrxTable TT on TT.SourceRegistrationID1 = T.SourceRegistrationID1 AND TT.SiteGuid = T.SiteGuid  
WHERE T.InventoryDate >= @StartDate 


--Get list of missing meters
SELECT DISTINCT T.[Site] AS [Site], T.SourceRegistrationID1 AS [Meter ID] 
from tblTransactions T 
LEFT JOIN tblMeter M on M.MeterID = T.SourceRegistrationID1 AND M.SiteGuid = T.SiteGuid 
WHERE T. InventoryDate >= @StartDate 
AND T.SourceRegistrationID1 IS NOT NULL  
AND (M.MeterID IS NULL OR M.SiteGuid IS NULL) 
order by T.[Site], T.SourceRegistrationID1