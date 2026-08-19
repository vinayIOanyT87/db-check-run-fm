
/*

Select * from tblTransactions where TransID = 'e81e545f5cf84ebd8e36875040679bd7'
Select * from tblTransactionLineItems where TransIndex = 31

*/

USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6AirForceSALEFieldsToFMD8LineItems]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6AirForceSALEFieldsToFMD8LineItems]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6AirForceSALEFieldsToFMD8LineItems]
GO

USE [ConsolidatedDB]
GO

CREATE PROCEDURE [dbo].[Migrate_FMD6AirForceSALEFieldsToFMD8LineItems]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6AirForceSALEFieldsToFMD8LineItems ''

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


Set @fill = '?';
Set @blank = '';

Insert Into ConsolidatedDB.dbo.tblTransactionLineItems
(
SequenceID,
MeterStart,
MeterStop,
GrossQuantity,
Temperature,
Vcf,
Density,
Product,
ProductCode,
ProductType,
ProductPrice,
ProductIndex,
CLIN,
NetQuantity,
ContractNumber,
DestinationRegistrationID,
DestinationSerialNumber,
DestinationEquipmentType,
DestinationEquipmentModel,
DestinationCompanyEquipmentID,
DestinationEquipmentIndex,
DestinationCompartmentID,
DestinationCompartmentIndex,
SourceRegistrationID,
SourceSerialNumber,
SourceEquipmentType,
SourceEquipmentModel,
SourceCompanyEquipmentID,
SourceEquipmentIndex,
SourceCompartmentID,
SourceCompartmentIndex,
MeterFactor,
LineItemSequenceNumber,
BatchNumber,
DocumentNumber,
LineFill,
BottomVolume,
NetCapacity,
Customs,
TransactionStatus,
ArmNumber,
LineNumber,
OperatorID,
OperatorIndex,
TankStatus,
MeterStartDateTime,
MeterStopDateTime,
Pit,
RequestedDateTime,
DispatchedDateTime,
AcknowledgedDateTime,
OnLocationTime,
ValidationDateTime,
CompletionDateTime,
ReceiptVariance,
DifferentialPressure,
LoadRackVariance,
RequestedBy,
FreezePoint,
DeleteFlag,
StorageLocationID,
StorageLocationIndex,
MeterID,
AdditiveProfileID,
AdditiveProfileIndex,
CreatedBy,
CreatedDate,
UpdatedBy,
UpdatedDate,
PresetAmount,
EngineeringUnitsIndex,
CustomerProductName,
CustomerProductCode,
TransactionInventoryDate,
OrderLineReferenceID,
COAWaiver,
COANote,
COAID,
Quality,
Tax1,
Tax2,
Tax3,
Tax4,
Tax5,
--TransVersion,
LoadingLocationID,
LoadingLocationIndex,
ImproperAdditization,
BrokenBlend,
ContaminatePrompt,
CompartmentsPreviouslyLoaded,
CompartmentsEmpty,
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
OdometerHours,
EndDeliveryDate,
RequestedDeliveryDate,
InvoiceNumber,
InvoiceLineNumber,
AlternativeGrossVolume,
AlternativeNetVolume,
AlternativeUnits,
TankLevel,
TankLevelUnits,
Date01,
Date02,
Date03,
Date04,
NonDomesticPrice,
CurrencyUnit,
ExchangeRate,
QualityTestNumber,
Odometer,
DeliveryLocation,
Variance,
PartialFill,
TransIndex
)
Select
/* SequenceID */0,
/* MeterStart */NULL,
/* MeterStop */NULL,
/* GrossQuantity */ isnull(ta.GrossVolume,0) / 1000,
/* Temperature */ ta.Temperature,
/* Vcf */ ta.VCF,
/* Density */ ta.Density,
/* Product */ (Select top 1 isnull(tp.ProductID,'') from tblProducts tp where ProductCode = ta.ProductID),
/* ProductCode */ta.ProductID,
/* ProductType */'Component',
/* ProductPrice */NULL,
/* ProductIndex */(Select top 1 isnull(tp.ProductIndex,0) from tblProducts tp where ProductCode = ta.ProductID),
/* CLIN */@blank,
/* NetQuantity */ isnull(ta.NetVolume,0) / 1000,
/* ContractNumber */@blank,
/* DestinationRegistrationID */ tt.DestinationRegistrationID1,
/* DestinationSerialNumber */ tt.DestinationSerialNumber1,
/* DestinationEquipmentType */ tt.DestinationEquipmentType1,
/* DestinationEquipmentModel */ tt.DestinationEquipmentModel1,
/* DestinationCompanyEquipmentID */ tt.DestinationCompanyEquipmentID1,
/* DestinationEquipmentIndex */ tt.DestinationEquipmentIndex1,
/* DestinationCompartmentID */@blank,
/* DestinationCompartmentIndex */NULL,
/* SourceRegistrationID */ tt.SourceRegistrationID1,
/* SourceSerialNumber */tt.SourceSerialNumber1,
/* SourceEquipmentType */tt.SourceEquipmentType1,
/* SourceEquipmentModel */tt.SourceEquipmentModel1,
/* SourceCompanyEquipmentID */tt.SourceCompanyEquipmentID1,
/* SourceEquipmentIndex */tt.SourceEquipmentIndex1,
/* SourceCompartmentID */@blank,
/* SourceCompartmentIndex */NULL,
/* MeterFactor */NULL,
/* LineItemSequenceNumber */0,
/* BatchNumber */@blank,
/* DocumentNumber */@blank,
/* LineFill */NULL,
/* BottomVolume */NULL,
/* NetCapacity */NULL,
/* Customs */@blank,
/* TransactionStatus */0,
/* ArmNumber */NULL,
/* LineNumber */NULL,
/* OperatorID */tt.OperatorID,
/* OperatorIndex */tt.OperatorIndex,
/* TankStatus */@blank,
/* MeterStartDateTime */NULL,
/* MeterStopDateTime */NULL,
/* Pit */@blank,
/* RequestedDateTime */NULL,
/* DispatchedDateTime */NULL,
/* AcknowledgedDateTime */NULL,
/* OnLocationTime */NULL,
/* ValidationDateTime */NULL,
/* CompletionDateTime */NULL,
/* ReceiptVariance */NULL,
/* DifferentialPressure */NULL,
/* LoadRackVariance */NULL,
/* RequestedBy */@blank,
/* FreezePoint */NULL,
/* DeleteFlag */ta.DeleteFlag,
/* StorageLocationID */@blank,
/* StorageLocationIndex */NULL,
/* MeterID */@blank,
/* AdditiveProfileID */@blank,
/* AdditiveProfileIndex */NULL,
/* CreatedBy */tt.CreatedBy,
/* CreatedDate */tt.CreatedDate,
/* UpdatedBy */tt.UpdatedBy,
/* UpdatedDate */tt.UpdatedDate,
/* PresetAmount */NULL,
/* EngineeringUnitsIndex */0,
/* CustomerProductName */@blank,
/* CustomerProductCode */@blank,
/* TransactionInventoryDate */tt.InventoryDate,
/* OrderLineReferenceID */0,
/* COAWaiver */0,
/* COANote */@blank,
/* COAID */@blank,
/* Quality */1,
/* Tax1 */NULL,
/* Tax2 */NULL,
/* Tax3 */NULL,
/* Tax4 */NULL,
/* Tax5 */NULL,
--/* TransVersion */,
/* LoadingLocationID */@blank,
/* LoadingLocationIndex */NULL,
/* ImproperAdditization */0,
/* BrokenBlend */0,
/* ContaminatePrompt */0,
/* CompartmentsPreviouslyLoaded */0,
/* CompartmentsEmpty */0,
/* Flag01 */0,
/* Flag02 */0,
/* Flag03 */0,
/* Flag04 */0,
/* Flag05 */0,
/* Flag06 */0,
/* Number01 */NULL,
/* Number02 */NULL,
/* Number03 */NULL,
/* Number04 */NULL,
/* Number05 */NULL,
/* Number06 */NULL,
/* OdometerHours */NULL,
/* EndDeliveryDate */NULL,
/* RequestedDeliveryDate */NULL,
/* InvoiceNumber */@blank,
/* InvoiceLineNumber */@blank,
/* AlternativeGrossVolume */NULL,
/* AlternativeNetVolume */NULL,
/* AlternativeUnits */NULL,
/* TankLevel */NULL,
/* TankLevelUnits */NULL,
/* Date01 */NULL,
/* Date02 */NULL,
/* Date03 */NULL,
/* Date04 */NULL,
/* NonDomesticPrice */NULL,
/* CurrencyUnit */NULL,
/* ExchangeRate */NULL,
/* QualityTestNumber */@blank,
/* Odometer */NULL,
/* DeliveryLocation */@blank,
/* Variance */NULL,
/* PartialFill */NULL,
/* TransIndex */tt.TransIndex
from AccountingDB6.dbo.t_Acct_Tx5 ta
LEFT JOIN ConsolidatedDB.dbo.tblTransactions tt
ON tt.TransID = ta.TransactionID
where 
ta.SiteIndex = @siteIndex6 and tt.TransIndex is not null and ta.Alias = 'SALE' and
tt.TransIndex not in (Select TransIndex from ConsolidatedDB.dbo.tblTransactionUserData)