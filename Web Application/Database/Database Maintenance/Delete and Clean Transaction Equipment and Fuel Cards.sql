--This script cleans the database. All transactions, Equipment, and Fuel Cards (except USARMY} are deleted. 

Use [ConsolidatedDB]
 

 DELETE from tblTransactionLinks;

 DELETE from tblTransactionPIDX;

 DELETE from tblTransactionTransportLineItems;

 DELETE from tblTransactionSubLineItems;  

 DELETE from tblTransactionLineItemUserData;

 DELETE from tblTransactionLineItems;

 DELETE from tblTransactionUserData;

 DELETE from tblTransactionNotes;

 DELETE from tblTransactionWeightReadings;

 DELETE from tblTransactionSignature;

 DELETE from tblTransactions;
 
 DELETE from tblOwnerCloseout;
 
 DELETE from tblCloseoutInventory;
 
 Delete from tblSessions;
 
 Update tblSequences Set SequenceValue = 1

 Update tblAccountingSequences Set SequenceValue = 1

 Delete from tblAlarmAndEventLog

 Delete from tblChangeLog
 
 Delete From tblChangesQueue

 Delete from tblAuditLog

go

--Use the following query to delete all equipment in a database:

delete from tblEquipment
delete from tblEntityToSiteMap where TypeID='Equipment'
go

--Use the following query to delete all fuel cards in a database except fuel card with ID of USARMY:

delete from tblFuelCards where ID <> 'USARMY'
delete from tblEntityToSiteMap where TypeID = 'FuelCard' AND [Index] NOT IN (SELECT FuelCardIndex from tblFuelCards WHERE ID='USARMY')
go

--Use the following query to delete all Equipment Types in a database except the 14 Standard Types:

delete from tblEquipmentTypes where EqTypeIndex > '14'
delete from tblEntityToSiteMap where TypeID = 'Equipment Type' AND [Index] NOT IN (SELECT EqTypeIndex from tblEquipmentTypes WHERE EqTypeIndex > '14')
go



