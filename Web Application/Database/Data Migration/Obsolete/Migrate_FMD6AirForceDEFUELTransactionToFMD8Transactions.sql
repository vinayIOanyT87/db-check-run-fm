USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6CommonRequestToFMD8FuelCards]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6AirForceSALETransactionToFMD8Transactions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6AirForceSALETransactionToFMD8Transactions]
GO

USE [ConsolidatedDB]
GO

CREATE PROCEDURE [dbo].[Migrate_FMD6AirForceSALETransactionToFMD8Transactions]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6AirForceSALETransactionToFMD8Transactions ''

*/
@IsBaseDB bit,
@SiteID NVarChar(MAX) = NULL

AS 

IF NOT EXISTS(Select * from sys.databases where [name] = 'ConsolidatedDB6')
BEGIN
	Select 'ConsolidatedDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 ConsolidatedDB Database before running this stored procedure';
	return
END

IF NOT EXISTS(Select * from sys.databases where [name] = 'AccountingDB6')
BEGIN
	Select 'AccountingDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 Accounting Database before running this stored procedure';
	return
END



if(@IsBaseDB = 1)
BEGIN
	IF NOT EXISTS(Select * from sys.databases where [name] = 'AviationDB6')
	BEGIN
		Select 'AviationDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 Aviation Database before running this stored procedure';
		return
	END
	if((Select COUNT(SiteIndex) from tblSites) <> 2)
	BEGIN
		Select 'A base level site must have only two sites in the database.  The "SiteAdmin" site and the actual site.';
		return;
	END
	if(isnull(@SiteID,'') = '')
		BEGIN
		Select 'A site must be specified when running this script as a base level script.';
		return;
		End
	IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
		BEGIN
		Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
		return;
		END
END
ELSE
BEGIN
	
	if(isnull(@SiteID,'') = '')
		BEGIN
		Select 'A site must be specified when running this script as a base level script.';
		return;
		End
		
	if(isnull(@SiteID,'') <> '')
	BEGIN
		IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
			BEGIN
			Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
			return;
			END
	END
END

declare @siteIndex6 int;
declare @siteIndex8 int;

Set @siteIndex6 = (Select Min(Isnull(SiteIndex,0)) from ConsolidatedDB6.dbo.tblSites where SiteID = @SiteID);
Set @siteIndex8 = (Select Min(Isnull(SiteIndex,0)) from ConsolidatedDB.dbo.tblSites where ID = @SiteID);

declare @fill nvarchar(30);
declare @blank nvarchar(1);
declare @descCompanyName nvarchar(3);
declare @descCompanyCode nvarchar(3);
declare @descCompanyIndex int;
declare @transActionStatusComplete int;
declare @defaultTicketMode nvarchar(1);


Set @fill = '?';
Set @blank = '';
Set @descCompanyName = 'DESC';
Set @descCompanyCode = 'DESC';
Set @descCompanyIndex = (Select MIN(CompanyIndex) from tblCompanies where ID = @descCompanyName);
set @transActionStatusComplete = 0;
set @defaultTicketMode = '0';


Insert Into tblTransactions
(TransID,
AliasName,
AliasIndex,
TransTypeID,
SubType,
Site,
SiteIndex,
TransReferenceID,
InventoryDate,
ShipToID,
ShipToCode,
ShipToIndex,
SupplierID,
SupplierCode,
SupplierIndex,
CreatedDate,
CreatedBy,
RequestedDeliveryDate,
UpdatedDate,
UpdatedBy,
TransDateTime,
--TransVersion,
SCACCode,
CardNumber,
ShipmentNumber,
ShipperID,
ShipperCode,
ShipperIndex,
OwnerID,
OwnerCode,
OwnerIndex,
ManagerID,
ManagerCode,
ManagerIndex,
CarrierID,
CarrierCode,
CarrierIndex,
ConjoinTransID,
ReversedTransID,
LinkedDocumentNumber,
ReversalType,
PONumber,
TimeIn,
TimeOut,
TimeEnd,
RoutingID,
TicketSource,
LoadID,
TransactionStatus,
BillToID,
BillToCode,
BillToIndex,
DriverIdentificationNumber,
CreditAmount,
CardExpiration,
CardName,
CardType,
CashAmount,
RouteOriginationDate,
InternationalRouteIndicator,
PreviousRoutingID,
FinalStationIndex,
PreviousStationIndex,
NextStationIndex,
OriginStationIndex,
ShippingDocumentNumber,
DocumentNumber,
STD,
ETD,
STA,
ETA,
SFT,
FST,
EstimatedFuelingDuration,
DeleteFlag,
TicketMode,
DestinationRegistrationID1,
DestinationSerialNumber1,
DestinationEquipmentType1,
DestinationEquipmentModel1,
DestinationCompanyEquipmentID1,
DestinationEquipmentIndex1,
DestinationRegistrationID2,
DestinationSerialNumber2,
DestinationEquipmentType2,
DestinationEquipmentModel2,
DestinationCompanyEquipmentID2,
DestinationEquipmentIndex2,
DestinationRegistrationID3,
DestinationSerialNumber3,
DestinationEquipmentType3,
DestinationEquipmentModel3,
DestinationCompanyEquipmentID3,
DestinationEquipmentIndex3,
SourceRegistrationID1,
SourceSerialNumber1,
SourceEquipmentType1,
SourceEquipmentModel1,
SourceCompanyEquipmentID1,
SourceEquipmentIndex1,
SourceRegistrationID2,
SourceSerialNumber2,
SourceEquipmentType2,
SourceEquipmentModel2,
SourceCompanyEquipmentID2,
SourceEquipmentIndex2,
SourceRegistrationID3,
SourceSerialNumber3,
SourceEquipmentType3,
SourceEquipmentModel3,
SourceCompanyEquipmentID3,
SourceEquipmentIndex3,
OperatorID,
OperatorIndex,
EffectiveDate,
ExpirationDate,
ScheduledDate,
AutoComplete,
Flag01,
Flag02,
Flag03,
Flag04,
Flag05,
Flag06,
Number01,
Number02,
Number03,
Number04,
Number05,
Number06,
ContactFirstName,
ContactSurname,
Date01,
Date02,
Date03,
Date04,
LegacyNumber,
Country,
ContactInfo,
AssociatedDocNumber,
AssociatedCLIN,
SubmittedToAccounting,
OriginApplication,
FuelCardIndex,
FuelCardID,
AssociatedTransportOrderNumber,
RequestedDateTime,
DispatchedDateTime,
ErrorFlag)
Select
/* TransID */	ta.TransactionID, 
/* AliasName */ 'Sale', 
/* AliasIndex */ (Select top 1 AliasID from tblTransactionAliases where AliasName = 'Sale' and SiteIndex = -1), 
/* TransTypeID */ 6, 
/* SubType */ @blank, 
/* Site */ ta.Manager,
/* SiteIndex */ @siteIndex8,
/* TransReferenceID */ @blank,
/* InventoryDate */ CONVERT(date,ta.TransactionDate),
/* ShipToID */ ta.Consumer,
/* ShipToCode */ (Select top 1 Code from tblCompanies where ta.Consumer = ID and SiteIndex = -1),
/* ShipToIndex */ (Select top 1 CompanyIndex from tblCompanies where ta.Consumer = ID and SiteIndex = -1),
/* SupplierID */ @blank,
/* SupplierCode */ @blank,
/* SupplierIndex */ NULL,
/* CreatedDate */ ta.CreatedDate,
/* CreatedBy */ ta.CreatedBy,
/* RequestedDeliveryDate */ (Select top 1 REQUEST_TIME from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* UpdatedDate */ ta.UpdatedDate,
/* UpdatedBy */ ta.UpdatedBy,
/* TransDateTime */ ta.TransactionDate,
--/* TransVersion */ ,
/* SCACCode */ @blank,
/* CardNumber */ ta.UserData3,
/* ShipmentNumber */ @blank,
/* ShipperID */ ta.Manager,
/* ShipperCode */ (Select top 1 Code from tblCompanies where ta.Manager = ID and SiteIndex = -1),
/* ShipperIndex */ (Select top 1 CompanyIndex from tblCompanies where ta.Manager = ID and SiteIndex = -1),
/* OwnerID */ @descCompanyName,
/* OwnerCode */ @descCompanyCode,
/* OwnerIndex */ @descCompanyIndex,
/* ManagerID */ @descCompanyName,
/* ManagerCode */ @descCompanyCode,
/* ManagerIndex */ @descCompanyIndex,
/* CarrierID */ ta.Vendor,
/* CarrierCode */ (Select top 1 Code from tblCompanies where ta.Vendor = ID and SiteIndex = -1),
/* CarrierIndex */ (Select top 1CompanyIndex from tblCompanies where ta.Vendor = ID and SiteIndex = -1),
/* ConjoinTransID */ @fill,
/* ReversedTransID */ @fill,
/* LinkedDocumentNumber */ @fill,
/* ReversalType */ @fill,
/* PONumber */ @blank,
/* TimeIn */ (Select top 1 ARR_TIME from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* TimeOut */ (Select top 1 DEP_TIME from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* TimeEnd */ (Select top 1 STOP_TIME from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* RoutingID */ @blank,
/* TicketSource */ @blank,
/* LoadID */ @blank,
/* TransactionStatus */ @transActionStatusComplete,
/* BillToID */ ta.UserData1,
/* BillToCode */ (Select top 1 Code from tblCompanies where ta.UserData1 = ID and SiteIndex = -1),
/* BillToIndex */ (Select top 1 CompanyIndex from tblCompanies where ta.UserData1 = ID and SiteIndex = -1),
/* DriverIdentificationNumber */ @blank,
/* CreditAmount */ NULL,
/* CardExpiration */ NULL,
/* CardName */ @blank,
/* CardType */ @blank,
/* CashAmount */ NULL,
/* RouteOriginationDate */ NULL,
/* InternationalRouteIndicator */ 0,
/* PreviousRoutingID */ @blank,
/* FinalStationIndex */ NULL,
/* PreviousStationIndex */ NULL,
/* NextStationIndex */ NULL,
/* OriginStationIndex */ NULL,
/* ShippingDocumentNumber */ @blank,
/* DocumentNumber */ @fill,
/* STD */ NULL,
/* ETD */ NULL,
/* STA */ NULL,
/* ETA */ NULL,
/* SFT */ NULL,
/* FST */ (Select top 1 START_TIME from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* EstimatedFuelingDuration */ NULL,
/* DeleteFlag */ ta.DeleteFlag,
/* TicketMode */ @defaultTicketMode,
/* DestinationRegistrationID1 */ ta.ReceivingEqID,
/* DestinationSerialNumber1 */ @blank,
/* DestinationEquipmentType1 */ (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = ta.ReceivingEqID ),
/* DestinationEquipmentModel1 */ (Select top 1 Model from tblEquipment where ID = ta.ReceivingEqID),
/* DestinationCompanyEquipmentID1 */ @blank,
/* DestinationEquipmentIndex1 */ (Select top 1 EquipmentIndex from tblEquipment where ID = ta.ReceivingEqID),
/* DestinationRegistrationID2 */ @blank,
/* DestinationSerialNumber2 */ @blank,
/* DestinationEquipmentType2 */ @blank,
/* DestinationEquipmentModel2 */ @blank,
/* DestinationCompanyEquipmentID2 */ @blank,
/* DestinationEquipmentIndex2 */ NULL,
/* DestinationRegistrationID3 */ @blank,
/* DestinationSerialNumber3 */ @blank,
/* DestinationEquipmentType3 */ @blank,
/* DestinationEquipmentModel3 */ @blank,
/* DestinationCompanyEquipmentID3 */ @blank,
/* DestinationEquipmentIndex3 */ NULL,
/* SourceRegistrationID1 */ ta.EquipmentID,
/* SourceSerialNumber1 */ @blank,
/* SourceEquipmentType1 */ (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = ta.EquipmentID ),
/* SourceEquipmentModel1 */ (Select top 1 Model from tblEquipment where ID = ta.EquipmentID),
/* SourceCompanyEquipmentID1 */ @blank,
/* SourceEquipmentIndex1 */ (Select top 1 EquipmentIndex from tblEquipment where ID = ta.EquipmentID),
/* SourceRegistrationID2 */ @blank,
/* SourceSerialNumber2 */ @blank,
/* SourceEquipmentType2 */ @blank,
/* SourceEquipmentModel2 */ @blank,
/* SourceCompanyEquipmentID2 */ @blank,
/* SourceEquipmentIndex2 */ NULL,
/* SourceRegistrationID3 */ @blank,
/* SourceSerialNumber3 */ @blank,
/* SourceEquipmentType3 */ @blank,
/* SourceEquipmentModel3 */ @blank,
/* SourceCompanyEquipmentID3 */ @blank,
/* SourceEquipmentIndex3 */ NULL,
/* OperatorID */ (Select top 1 OPERATOR from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* OperatorIndex */ (Select top 1 OperatorIndex from tblOperators tblo where tblo.ID = (Select top 1 OPERATOR from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID)),
/* EffectiveDate */ NULL,
/* ExpirationDate */ NULL,
/* ScheduledDate */ NULL,
/* AutoComplete */ 0,
/* Flag01 */ ta.Flag1,
/* Flag02 */ ta.Flag2,
/* Flag03 */ 0,
/* Flag04 */ 0,
/* Flag05 */ ta.Flag7,
/* Flag06 */ 0,
/* Number01 */ NULL,
/* Number02 */ 0,
/* Number03 */ 0, --?????? Not Sure About this.
/* Number04 */ NULL,
/* Number05 */ NULL,
/* Number06 */ NULL,
/* ContactFirstName */ @blank,
/* ContactSurname */ (Select top 1 REQUESTED_BY from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* Date01 */ NULL,
/* Date02 */ NULL,
/* Date03 */ NULL,
/* Date04 */ NULL,
/* LegacyNumber */ @blank,
/* Country */ @blank,
/* ContactInfo */ @blank,
/* AssociatedDocNumber */ @blank,
/* AssociatedCLIN */ @blank,
/* SubmittedToAccounting */ NULL, --?????? Not Sure About This.
/* OriginApplication */ CASE SUBSTRING(ta.TransactionID,1,2) WHEN 'DP' THEN 3 ELSE 1 END,
/* FuelCardIndex */ (Select top 1 FuelCardIndex from tblEquipment te where te.ID = ta.ReceivingEqID),
/* FuelCardID */ (Select top 1 isnull(tfc.ID,@blank) from tblFuelCards tfc, tblEquipment te where te.FuelCardIndex = tfc.FuelCardIndex and te.ID = ta.ReceivingEqID),
/* AssociatedTransportOrderNumber */ @blank,
/* RequestedDateTime */ (Select top 1 REQUEST_TIME from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* DispatchedDateTime */ (Select top 1 DISP_TIME from AviationDB6.dbo.CONTROL_LOG cl where cl.TRANSACTION_ID = ta.TransactionID),
/* ErrorFlag */ 0
from AccountingDB6.dbo.t_Acct_Tx5 ta 
where ta.Alias = 'SALE' AND ta.SiteIndex = @siteIndex6 AND
Not exists(Select TransID from tblTransactions where TransID = ta.TransactionID);