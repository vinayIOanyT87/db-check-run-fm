USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6ADJUSTTransactionToFMD8Transactions]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6ADJUSTTransactionToFMD8Transactions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6ADJUSTTransactionToFMD8Transactions]
GO

CREATE PROCEDURE [dbo].[Migrate_FMD6ADJUSTTransactionToFMD8Transactions]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6ADJUSTTransactionToFMD8Transactions 2,null

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

declare @siteIndex6 int;
declare @siteIndex8 int;
declare @count int
declare @aliasName6 nvarchar(50);
declare @aliasName8 nvarchar(50);

Set @aliasName6 = 'ADJUST'
Set @aliasName8 = 'Physical Inventory'


declare @fill nvarchar(30);
declare @blank nvarchar(1);
declare @descCompanyName nvarchar(4);
declare @descCompanyCode nvarchar(3);
declare @descCompanyIndex int;
declare @transActionStatusComplete int;
declare @defaultTicketMode nvarchar(1);


Set @fill = '?';
Set @blank = '';
Set @descCompanyName = 'DESC';
Set @descCompanyCode = 'DOD';
Set @descCompanyIndex = (Select MIN(CompanyIndex) from tblCompanies where ID = @descCompanyName);
set @transActionStatusComplete = 11;
set @defaultTicketMode = '0';

declare @SequenceName nvarchar(50);
declare @BaseSequenceID bigint;
Set @SequenceName = 'TransactionVersion'


IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END



SELECT S8.[ID] AS SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1 AND
( Select isnull(COUNT(TransactionID),0) from AccountingDB6.dbo.t_Acct_Tx14 ta where ta.Alias = @aliasName6 AND ta.SiteIndex = S6.siteIndex) > 0
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

	Create Table #TransVersionTable
	(
		[TransVersion] [bigint] IDENTITY(1,1) NOT NULL,
		[TransID] nvarchar(64) NOT NULL
	)

	Insert Into #TransVersionTable
	(TransID)
	Select TransactionID
	from AccountingDB6.dbo.t_Acct_Tx14 ta WHERE
	--ta.SiteIndex = @siteIndex6 and 
	ta.Alias = @aliasName6


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
	TransVersion,
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
	/* AliasName */ @aliasName8, 
	/* AliasIndex */ (Select top 1 AliasID from tblTransactionAliases where AliasName = @aliasName8), 
	/* TransTypeID */ (Select top 1 TransTypeID from tblTransactionAliases where AliasName = @aliasName8),  
	/* SubType */ @blank, 
	/* Site */ ta.Manager,
	/* SiteIndex */ s.siteIndex8,
	/* TransReferenceID */ @blank,
	/* InventoryDate */ CONVERT(date,ta.TransactionDate),
	/* ShipToID */ @blank,
	/* ShipToCode */ @blank,
	/* ShipToIndex */ NULL,
	/* SupplierID */ @blank,
	/* SupplierCode */ @blank,
	/* SupplierIndex */ NULL,
	/* CreatedDate */ dbo.GetUTCTime(s.siteIndex8,ta.CreatedDate),
	/* CreatedBy */ ta.CreatedBy,
	/* RequestedDeliveryDate */ NULL,
	/* UpdatedDate */ dbo.GetUTCTime(s.siteIndex8,ta.UpdatedDate),
	/* UpdatedBy */ ta.UpdatedBy,
	/* TransDateTime */ dbo.GetUTCTime(s.siteIndex8,ta.TransactionDate),
	/* TransVersion */ @BaseSequenceID + tvt.TransVersion,
	/* SCACCode */ @blank,
	/* CardNumber */ @blank,
	/* ShipmentNumber */ @blank,
	/* ShipperID */ @blank,
	/* ShipperCode */ @blank,
	/* ShipperIndex */ NULL,
	/* OwnerID */ @descCompanyName,
	/* OwnerCode */ @descCompanyCode,
	/* OwnerIndex */ @descCompanyIndex,
	/* ManagerID */ @descCompanyName,
	/* ManagerCode */ @descCompanyCode,
	/* ManagerIndex */ @descCompanyIndex,
	/* CarrierID */ @blank,
	/* CarrierCode */ @blank,
	/* CarrierIndex */ NULL,
	/* ConjoinTransID */ @blank,
	/* ReversedTransID */ @blank,
	/* LinkedDocumentNumber */ @blank,
	/* ReversalType */ @blank,
	/* PONumber */ @blank,
	/* TimeIn */ NULL,
	/* TimeOut */ NULL,
	/* TimeEnd */ NULL,
	/* RoutingID */ @blank,
	/* TicketSource */ @blank,
	/* LoadID */ @blank,
	/* TransactionStatus */ @transActionStatusComplete,
	/* BillToID */ @blank,
	/* BillToCode */ @blank,
	/* BillToIndex */ NULL,
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
	/* DocumentNumber */ @blank,
	/* STD */ NULL,
	/* ETD */ NULL,
	/* STA */ NULL,
	/* ETA */ NULL,
	/* SFT */ NULL,
	/* FST */ NULL,
	/* EstimatedFuelingDuration */ NULL,
	/* DeleteFlag */ ta.DeleteFlag,
	/* TicketMode */ @defaultTicketMode,
	/* DestinationRegistrationID1 */ @blank,
	/* DestinationSerialNumber1 */ @blank,
	/* DestinationEquipmentType1 */ @blank,
	/* DestinationEquipmentModel1 */ @blank,
	/* DestinationCompanyEquipmentID1 */ @blank,
	/* DestinationEquipmentIndex1 */ NULL,
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
	/* SourceRegistrationID1 */ @blank,
	/* SourceSerialNumber1 */ @blank,
	/* SourceEquipmentType1 */ @blank,
	/* SourceEquipmentModel1 */ @blank,
	/* SourceCompanyEquipmentID1 */ @blank,
	/* SourceEquipmentIndex1 */ NULL,
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
	/* OperatorID */ @blank,
	/* OperatorIndex */ NULL,
	/* EffectiveDate */ NULL,
	/* ExpirationDate */ NULL,
	/* ScheduledDate */ NULL,
	/* AutoComplete */ 0,
	/* Flag01 */ ta.Flag1,
	/* Flag02 */ ta.Flag2,
	/* Flag03 */ 0,
	/* Flag04 */ 0,
	/* Flag05 */ ta.SentToEnterpriseFlag,
	/* Flag06 */ 0,
	/* Number01 */ NULL,
	/* Number02 */ 0,
	/* Number03 */ 0, --?????? Not Sure About this.
	/* Number04 */ NULL,
	/* Number05 */ NULL,
	/* Number06 */ NULL,
	/* ContactFirstName */ @blank,
	/* ContactSurname */ @blank,
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
	/* FuelCardIndex */ NULL,
	/* FuelCardID */ @blank,
	/* AssociatedTransportOrderNumber */ @blank,
	/* RequestedDateTime */ NULL,
	/* DispatchedDateTime */ NULL,
	/* ErrorFlag */ ta.ErrorFromEnterpriseFlag
	from AccountingDB6.dbo.t_Acct_Tx14 ta
	JOIN #TMPSITES s ON s.SiteIndex6 = ta.SiteIndex
	, #TransVersionTable tvt
	where ta.Alias = @aliasName6 AND
	ta.TransactionDate between '1900/1/1' and '2079/06/06 23:59:59' AND 
	tvt.TransID = ta.TransactionID --AND
	--ta.SiteIndex = @siteIndex6;

--select * from tblTransactions where aliasname=@aliasname8 order by transid

	drop table #TMPSITES;

	declare @lineItemCount int;
	Set @lineItemCount = (Select COUNT(TransID) from #TransVersionTable);

	if(@lineItemCount > 0)
	BEGIN
		UPDATE dbo.tblAccountingSequences SET SequenceValue = @BaseSequenceID + (Select MAX(TransVersion) from #TransVersionTable) WHERE SequenceName = @SequenceName;
	END
	
	drop table #TransVersionTable;

	

/* IF @@TRANCOUNT > 0    
BEGIN     
--	ROLLBACK TRANSACTION;     
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