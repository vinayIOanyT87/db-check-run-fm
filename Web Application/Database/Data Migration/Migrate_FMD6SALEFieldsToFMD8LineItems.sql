
/*

Select * from tblTransactions where TransID = 'e81e545f5cf84ebd8e36875040679bd7'
Select * from tblTransactionLineItems where TransIndex = 31

*/

USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6SALEFieldsToFMD8LineItems]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6SALEFieldsToFMD8LineItems]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6SALEFieldsToFMD8LineItems]
GO


CREATE PROCEDURE [dbo].[Migrate_FMD6SALEFieldsToFMD8LineItems]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6SALEFieldsToFMD8LineItems 2,null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
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



if(@IsBaseDB <> 2)
BEGIN
	IF NOT EXISTS(Select * from sys.databases where [name] = 'AviationDB6')
	BEGIN
		Select 'AviationDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 Aviation Database before running this stored procedure';
		return
	END
	/*if((Select COUNT(SiteIndex) from tblSites) <> 2)
	BEGIN
		Select 'A base level site must have only two sites in the database.  The "SiteAdmin" site and the actual site.';
		return;
	END*/
	/*if(isnull(@SiteID,'') = '')
		BEGIN
		Select 'A site must be specified when running this script as a base level script.';
		return;
		End
	IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
		BEGIN
		Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
		return;
		END
		*/
END
/*ELSE
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
END*/

declare @aliasName6 nvarchar(50);
declare @aliasName8 nvarchar(50);
declare @count int
declare @transActionStatusComplete int;
set @transActionStatusComplete = 11;

Set @aliasName6 = 'SALE'
Set @aliasName8 = 'Sale'

declare @fill nvarchar(30);
declare @blank nvarchar(1);
declare @BaseSequenceID bigint;

Set @fill = '?';
Set @blank = '';

declare @SequenceName nvarchar(50);
Set @SequenceName = 'LineItemID'

IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END


SELECT S8.[ID] AS SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
AND ( Select isnull(COUNT(TransactionID),0) from AccountingDB6.dbo.t_Acct_Tx5 ta where ta.Alias = @aliasName6 AND ta.SiteIndex = S6.siteIndex) > 0
ORDER BY S6.SiteIndex



/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
BEGIN TRANSACTION

*/	
	UPDATE dbo.tblAccountingSequences SET SequenceValue = SequenceValue + 1 WHERE SequenceName = @SequenceName;
	IF @@ROWCOUNT = 0
		INSERT INTO dbo.tblAccountingSequences (SequenceName, SequenceValue)
		VALUES (@SequenceName, 1);
	Set @BaseSequenceID = (SELECT top 1 Isnull(SequenceValue,0) FROM dbo.tblAccountingSequences WHERE SequenceName = @SequenceName)

	Create Table #LineItemIDTable
	(
		[LineItemIndex] [bigint] IDENTITY(1,1) NOT NULL,
		[TransIndex] [bigint] NOT NULL
	)

	Insert Into #LineItemIDTable
	(TransIndex)
	Select TransIndex
	from AccountingDB6.dbo.t_Acct_Tx5 ta
	JOIN #TMPSITES s ON ta.SiteIndex = s.SiteIndex6
	LEFT JOIN ConsolidatedDB.dbo.tblTransactions tt
	ON tt.TransID = ta.TransactionID
	where 
	--ta.SiteIndex = @siteIndex6 and 
	tt.TransIndex is not null and ta.Alias = @aliasName6 	


	CREATE TABLE #TMP_PRODUCT_INDEX_MAP
	(
		ProductID nvarchar(30) NULL,
		ProductCode nvarchar(15) NULL,
		ProductIndex8 int NULL,
		ProductID6 nvarchar(30) NULL,
		ProductIndex6 int NULL,
		ProductType int NULL,
		UnitOfIssue nvarchar(50)
	)

	INSERT INTO #TMP_PRODUCT_INDEX_MAP
	SELECT P8.ProductID, P8.ProductCode, P8.ProductIndex AS ProductIndex8, 
				P6.ProductID ,P6.ProductIndex AS ProductIndex6, P8.ProductType, P6.UnitOfIssue
	FROM [ConsolidatedDB6].[dbo].tblProducts P6 JOIN [ConsolidatedDB].[dbo].tblProducts P8 
	ON P6.PRODUCTCODE = P8.ProductID 
	JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=P8.[ProductIndex] AND TypeID='Products'
	JOIN #TMPSITES s ON P6.SiteIndex = s.SiteIndex6 AND M.SiteIndex = s.SiteIndex8
	WHERE P6.DeleteFlag = 0 AND 
	--P6.SiteIndex = @SiteIndex6 AND 
	--M.SiteIndex = @SiteIndex8 AND 
	P6.ProductIndex  <> -1

	INSERT INTO #TMP_PRODUCT_INDEX_MAP
	SELECT P8.ProductID, P8.ProductCode, P8.ProductIndex AS ProductIndex8, 
				P6.ProductID ,P6.ProductIndex AS ProductIndex6, P8.ProductType, P6.UnitOfIssue 
	FROM [ConsolidatedDB6].[dbo].tblProducts P6 JOIN [ConsolidatedDB].[dbo].tblProducts P8 
	ON P6.ProductIndex = P8.ProductIndex 
	WHERE P8.ProductIndex = -1
	
	Insert Into ConsolidatedDB.dbo.tblTransactionLineItems
	(
	TransLineItemID,
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
	TransVersion,
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
	/* TransLineItemID */ @BaseSequenceID + lit.LineItemIndex,
	/* SequenceID */0,
	/* MeterStart */NULL,
	/* MeterStop */NULL,
	/* GrossQuantity */ -1 * (abs(isnull(ta.GrossVolume,0)) / 1000.0),
	/* Temperature */ ta.Temperature,
	/* Vcf */ ta.VCF,
	/* Density */ ta.Density,
	/* Product */ isnull(tp.ProductID,''),
	/* ProductCode */ta.ProductID,
	/* ProductType */'Component',
	/* ProductPrice */NULL,
	/* ProductIndex */isnull(tp.ProductIndex8,0),
	/* CLIN */@blank,
	/* NetQuantity */ -1 * (abs(isnull(ta.GrossVolume,0)) / 1000.0),
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
	/* TransactionStatus */@transActionStatusComplete,
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
	/* TransVersion */tt.TransVersion,
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
	JOIN #TMPSITES s ON ta.SiteIndex = s.SiteIndex6
	LEFT JOIN ConsolidatedDB.dbo.tblTransactions tt
	INNER JOIN #LineItemIDTable lit on lit.TransIndex = tt.TransIndex
	ON tt.TransID = ta.TransactionID 
	LEFT JOIN (SELECT DISTINCT ProductIndex8, ProductID, ProductID6 FROM #TMP_PRODUCT_INDEX_MAP) tp on tp.ProductID6 = ta.ProductID
	where 
	lit.TransIndex = tt.TransIndex AND
	--ta.SiteIndex = @siteIndex6 and 
	tt.TransIndex is not null and ta.Alias = @aliasName6;

	declare @lineItemCount int;
	Set @lineItemCount = (Select COUNT(TransIndex) from #LineItemIDTable);

	if(@lineItemCount > 0)
	BEGIN
		UPDATE dbo.tblAccountingSequences SET SequenceValue = @BaseSequenceID + (Select MAX(LineItemIndex) from #LineItemIDTable) WHERE SequenceName = @SequenceName;
	END

	drop table #LineItemIDTable;
	drop table #TMP_PRODUCT_INDEX_MAP;

	drop table #TMPSITES;
	
	
	--select l.* from ConsolidatedDB.dbo.tblTransactionLineItems l JOIN ConsolidatedDB.dbo.tblTransactions t ON l.TransIndex=t.transIndex and aliasname=@AliasName8 order by transLineItemID
	
	
	
	
	

/*	
IF @@TRANCOUNT > 0    
BEGIN     
--	 	ROLLBACK TRANSACTION; 
   
	COMMIT TRANSACTION  
END   
 
END TRY

BEGIN CATCH
IF @@TRANCOUNT > 0    
BEGIN     
	ROLLBACK TRANSACTION; 
	--SELECT  'ERROR: ' + ISNULL(@MSG,'Unknown Error')  as [Status]; 
	DECLARE @MSG nvarchar(MAX)
	SET @MSG = ERROR_MESSAGE()    
	RAISERROR  (@MSG,0,1)  
END  
END CATCH
*/