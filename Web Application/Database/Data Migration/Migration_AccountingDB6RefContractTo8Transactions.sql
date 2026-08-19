USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AccountingDB6RefContractTo8Transactions]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6RefContractTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AccountingDB6RefContractTo8Transactions
GO

CREATE PROCEDURE [dbo].Migration_AccountingDB6RefContractTo8Transactions 

 /*=============================================
 Author:			A. Coker
 Create date:		3/12/2010
Description:		Migrating AccountingDB6.0 Ref Contract to ConsolidatedDB8.0 Transactions
 Modification History:
	Date		by			Description
	4/11/2010	C. Knight	Migrate Notes, Buyer DoDAAC, Supp DoDAAC (Bug 13332)
 =============================================*/
/*

EXEC Migration_AccountingDB6RefContractTo8Transactions 1, 'FP5518'  

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarChar(MAX) = NULL

AS 
BEGIN


IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END

SELECT S8.[ID] AS SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex


	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	
*/
CREATE TABLE #TMP_PRODUCT_INDEX_MAP
(
	ProductID nvarchar(30) NULL,
	ProductCode nvarchar(15) NULL,
	ProductIndex8 int NULL,
	ProductID6 nvarchar(15) NULL,
	ProductIndex6 int NULL,
	ProductType int NULL,
	UnitOfIssue nvarchar(50)
)

INSERT INTO #TMP_PRODUCT_INDEX_MAP
SELECT P8.ProductID, P8.ProductCode, P8.ProductIndex AS ProductIndex8, 
		P6.ProductID, P6.ProductIndex AS ProductIndex6, P8.ProductType, P6.UnitOfIssue
FROM [ConsolidatedDB6].[dbo].tblProducts P6 JOIN [ConsolidatedDB].[dbo].tblProducts P8 
ON P6.PRODUCTCODE = P8.ProductID 
JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=P8.[ProductIndex] AND TypeID='Products'
	JOIN #TMPSITES s ON s.SiteIndex6 = P6.SiteIndex AND M.SiteIndex=s.SiteIndex8
WHERE P6.DeleteFlag = 0 AND-- P6.SiteIndex = @SiteIndex6 AND M.SiteIndex = @SiteIndex8 AND 
P6.ProductIndex  <> -1

INSERT INTO #TMP_PRODUCT_INDEX_MAP
SELECT P8.ProductID, P8.ProductCode, P8.ProductIndex AS ProductIndex8, 
		P6.ProductID, P6.ProductIndex AS ProductIndex6, P8.ProductType, P6.UnitOfIssue 
FROM [ConsolidatedDB6].[dbo].tblProducts P6 JOIN [ConsolidatedDB].[dbo].tblProducts P8 
ON P6.ProductIndex = P8.ProductIndex 
WHERE P8.ProductIndex = -1


SELECT S.SiteIndex8, CompanyIndex, [ID] AS CompanyID, Code AS CompanyCode INTO #TMP_COMPANIES 
FROM [ConsolidatedDB].[dbo].tblCompanies C JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M 
ON C.CompanyIndex = M.[Index] AND TypeID='Companies', #TMPSITES S 
WHERE M.SiteIndex = CASE WHEN @IsBaseDB = 0 THEN s.SiteIndex8 ELSE -1 END 

DECLARE @NextTransVersion int
SELECT @NextTransVersion=SequenceValue FROM dbo.tblAccountingSequences WHERE SequenceName = 'TransactionVersion';
IF @NextTransVersion IS NULL
BEGIN
	INSERT INTO dbo.tblAccountingSequences (SequenceName, SequenceValue)
	VALUES ('TransactionVersion', 1);
	SET @NextTransVersion = 1
END
UPDATE dbo.tblAccountingSequences SET SequenceValue =  SequenceValue + 1
	WHERE SequenceName = 'TransactionVersion';

DECLARE @ManagerID		nvarchar(30)
DECLARE @OwnerID		nvarchar(30)
DECLARE @AliasName		nvarchar(30)
DECLARE @AliasID		int
DECLARE @TransTypeID	smallint

SET @ManagerID			='DESC'																	
SET @OwnerID			='DESC'																	
SET @AliasName			= 'Contract'

SELECT @AliasID=[AliasID], @TransTypeID=TransTypeID  FROM [ConsolidatedDB].[dbo].tblTransactionAliases WHERE [AliasName] = @AliasName

CREATE TABLE #TMP_TRANSACTIONS(
	[ContractIndex] int,
	[TransID] nvarchar(64) NOT NULL,
	[AliasName] [nvarchar](32) NOT NULL,
	[AliasIndex] [int] NULL,
	[TransTypeID] [smallint] NOT NULL,
	[SubType] [nvarchar](20) NULL,
	[Site] [nvarchar](30) NULL,
	[SiteIndex] [int] NULL,
	[TransReferenceID] [nvarchar](64) NULL,
	[InventoryDate] [smalldatetime] NULL,
	[ShipToID] [nvarchar](30) NULL,
	[ShipToCode] [nvarchar](10) NULL,
	[ShipToIndex] [int] NULL,
	[SupplierID] [nvarchar](30) NULL,
	[SupplierCode] [nvarchar](10) NULL,
	[SupplierIndex] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[RequestedDeliveryDate] [datetime] NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[TransDateTime] [datetime] NULL,
	[TransVersion] [bigint] NULL,
	[SCACCode] [nvarchar](4) NULL,
	[CardNumber] [nvarchar](30) NULL,
	[ShipmentNumber] [nvarchar](30) NULL,
	[ShipperID] [nvarchar](50) NULL,
	[ShipperCode] [nvarchar](10) NULL,
	[ShipperIndex] [int] NULL,
	[OwnerID] [nvarchar](30) NULL,
	[OwnerCode] [nvarchar](10) NULL,
	[OwnerIndex] [int] NULL,
	[ManagerID] [nvarchar](30) NULL,
	[ManagerCode] [nvarchar](10) NULL,
	[ManagerIndex] [int] NULL,
	[CarrierID] [nvarchar](30) NULL,
	[CarrierCode] [nvarchar](10) NULL,
	[CarrierIndex] [int] NULL,
	[ConjoinTransID] [nvarchar](64) NULL,
	[ReversedTransID] [nvarchar](64) NULL,
	[LinkedDocumentNumber] [nvarchar](64) NULL,
	[ReversalType] [nvarchar](2) DEFAULT '' NULL,
	[PONumber] [nvarchar](14) NULL,
	[TimeIn] [datetime] NULL,
	[TimeOut] [datetime] NULL,
	[TimeEnd] [datetime] NULL,
	[RoutingID] [nvarchar](30) NULL,
	[TicketSource] [nvarchar](20) NULL,
	[LoadID] [nvarchar](50) NULL,
	[TransactionStatus] [int] NULL DEFAULT 1, -- CHK 4/9/2010 - Contracts should be In Progress
	[BillToID] [nvarchar](30) NULL,
	[BillToCode] [nvarchar](10) NULL,
	[BillToIndex] [int] NULL,
	[DriverIdentificationNumber] [nvarchar](50) NULL,
	[CreditAmount] [float] NULL,
	[CardExpiration] [datetime] NULL,
	[CardName] [nvarchar](30) NULL,
	[CardType] [nvarchar](30) NULL,
	[CashAmount] [float] NULL,
	[RouteOriginationDate] [datetime] NULL,
	[InternationalRouteIndicator] [bit] NULL,
	[PreviousRoutingID] [nvarchar](30) NULL,
	[FinalStationIndex] [int] NULL,
	[PreviousStationIndex] [int] NULL,
	[NextStationIndex] [int] NULL,
	[OriginStationIndex] [int] NULL,
	[ShippingDocumentNumber] [nvarchar](30) NULL,
	[DocumentNumber] [nvarchar](30) NULL,
	[STD] [datetime] NULL,
	[ETD] [datetime] NULL,
	[STA] [datetime] NULL,
	[ETA] [datetime] NULL,
	[SFT] [datetime] NULL,
	[FST] [datetime] NULL,
	[EstimatedFuelingDuration] [int] NULL,
	[DeleteFlag] [bit] Default 0 NOT NULL,
	[TicketMode] [nvarchar](15) Default 0 NOT NULL,
	[DestinationRegistrationID1] [nvarchar](30) NULL,
	[DestinationSerialNumber1] [nvarchar](10) NULL,
	[DestinationEquipmentType1] [nvarchar](50) NULL,
	[DestinationEquipmentModel1] [nvarchar](20) NULL,
	[DestinationCompanyEquipmentID1] [nvarchar](30) NULL,
	[DestinationEquipmentIndex1] [int] NULL,
	[DestinationRegistrationID2] [nvarchar](30) NULL,
	[DestinationSerialNumber2] [nvarchar](10) NULL,
	[DestinationEquipmentType2] [nvarchar](50) NULL,
	[DestinationEquipmentModel2] [nvarchar](20) NULL,
	[DestinationCompanyEquipmentID2] [nvarchar](30) NULL,
	[DestinationEquipmentIndex2] [int] NULL,
	[DestinationRegistrationID3] [nvarchar](30) NULL,
	[DestinationSerialNumber3] [nvarchar](10) NULL,
	[DestinationEquipmentType3] [nvarchar](50) NULL,
	[DestinationEquipmentModel3] [nvarchar](20) NULL,
	[DestinationCompanyEquipmentID3] [nvarchar](30) NULL,
	[DestinationEquipmentIndex3] [int] NULL,
	[SourceRegistrationID1] [nvarchar](30) NULL,
	[SourceSerialNumber1] [nvarchar](10) NULL,
	[SourceEquipmentType1] [nvarchar](50) NULL,
	[SourceEquipmentModel1] [nvarchar](20) NULL,
	[SourceCompanyEquipmentID1] [nvarchar](30) NULL,
	[SourceEquipmentIndex1] [int] NULL,
	[SourceRegistrationID2] [nvarchar](30) NULL,
	[SourceSerialNumber2] [nvarchar](10) NULL,
	[SourceEquipmentType2] [nvarchar](50) NULL,
	[SourceEquipmentModel2] [nvarchar](20) NULL,
	[SourceCompanyEquipmentID2] [nvarchar](30) NULL,
	[SourceEquipmentIndex2] [int] NULL,
	[SourceRegistrationID3] [nvarchar](30) NULL,
	[SourceSerialNumber3] [nvarchar](10) NULL,
	[SourceEquipmentType3] [nvarchar](50) NULL,
	[SourceEquipmentModel3] [nvarchar](20) NULL,
	[SourceCompanyEquipmentID3] [nvarchar](30) NULL,
	[SourceEquipmentIndex3] [int] NULL,
	[OperatorID] [nvarchar](50) NULL,
	[OperatorIndex] [int] NULL,
	[EffectiveDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[ScheduledDate] [datetime] NULL,
	[AutoComplete] [bit] NULL,
	[Flag01] [bit] NULL,
	[Flag02] [bit] NULL,
	[Flag03] [bit] NULL,
	[Flag04] [bit] NULL,
	[Flag05] [bit] NULL,
	[Flag06] [bit] NULL,
	[Number01] [float] NULL,
	[Number02] [float] NULL,
	[Number03] [float] NULL,
	[Number04] [float] NULL,
	[Number05] [float] NULL,
	[Number06] [float] NULL,
	[ContactFirstName] [nvarchar](50) NULL,
	[ContactSurname] [nvarchar](50) NULL,
	[Date01] [datetime] NULL,
	[Date02] [datetime] NULL,
	[Date03] [datetime] NULL,
	[Date04] [datetime] NULL,
	[LegacyNumber] [nvarchar](50) NULL,
	[Country] [nvarchar](50) NULL,
	[ContactInfo] [nvarchar](50) NULL,
	[AssociatedDocNumber] [nvarchar](30) NULL,
	[AssociatedCLIN] [nvarchar](10) NULL,
	[SubmittedToAccounting] [bit] NULL,
	[OriginApplication] [int] NULL,
	[FuelCardIndex] [int] NULL,
	[FuelCardID] [nvarchar](50) NULL,
	[AssociatedTransportOrderNumber] [nvarchar](30) NULL,
	[RequestedDateTime] [datetime] NULL,
	[DispatchedDateTime] [datetime] NULL,
	[ErrorFlag] [bit] Default 0 NOT NULL,
	[AdditionalInformation] NVarchar(1000) NULL,
    [UserData1] NVarchar(120) NULL,
    [UserData2] NVarchar(120) NULL, 
    [UserData3] NVarchar(120) NULL, 
    [UserData4] NVarchar(120) NULL,  
    [UserData5] NVarchar(120) NULL, 
    [UserData7] NVarchar(120) NULL, 
    [UserData8] NVarchar(120) NULL,
    [UserData9] NVarchar(120) NULL,
    [UserData10] NVarchar(120) NULL,
    [UserData11] NVarchar(120) NULL,
    [UserData12] NVarchar(120) NULL, 
	
	[TransIndex] [bigint] IDENTITY(1,1) NOT NULL,
	[Notes] NVarchar (1000)			-- Added CHK 4/11/2010
	)
	

INSERT INTO #TMP_TRANSACTIONS(
	[TransID]
	,[ContractIndex]
	,[AliasName]
	,[AliasIndex]
	,[TransTypeID]
	,[Site] 
	,[SiteIndex]
	,[InventoryDate]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	
	,[TransVersion] 
	
	,ManagerID
	,ManagerCode
	,ManagerIndex
	,OwnerID
	,OwnerCode
	,OwnerIndex
	,[DocumentNumber]
	,[ContactFirstName]
	,[ContactSurname]
	,[Date01]
	,[Date02]
	,[Flag05]
	,[ErrorFlag] 
	,Deleteflag

	,[ContactInfo]
	,[AdditionalInformation] 
	,[UserData1]
	,[UserData2] 
	,[UserData3] 
	,[UserData4]  
	,[UserData7] 
	,[UserData8]
	,[UserData9]
	,[UserData10]
	,[UserData11]
	,[UserData12]
	,OriginApplication --Added by Eric Simmons on 4/5/2010 
	,[ShipToID]		-- Added CHK 4/11/2010
	,[ShipToCode]	-- Addec CHK 4/11/2010
	,[ShipToIndex]	-- Addec CHK 4/11/2010
	,[BillToID]		-- Added CHK 4/11/2010
	,[BillToCode]	-- Addec CHK 4/11/2010
	,[BillToIndex]	-- Addec CHK 4/11/2010
	,[Notes]		-- Addec CHK 4/11/2010
)
SELECT 
		NewID()
	  ,C.[ContractIndex]
      ,@AliasName											AS [AliasName]
      ,@AliasID											AS [AliasIndex]
      ,@TransTypeID										AS [TransTypeID]
      ,S.SiteID8											AS [Site] 
      ,S.SiteIndex8										AS [SiteIndex]
      ,CONVERT(Date, CreatedDate)					AS [InventoryDate]
      ,GETDATE()											AS [CreatedDate]
      ,'Varec'												AS [CreatedBy]
	  ,[UpdatedDate]
	  ,[UpdatedBy]
	,@NextTransVersion									AS [TransVersion] 
	,@ManagerID												AS ManagerID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND siteIndex8=s.SiteIndex8	) AS ManagerCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND siteIndex8=s.SiteIndex8	) AS ManagerIndex
	,@OwnerID AS OwnerID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND siteIndex8=s.SiteIndex8	) AS OwnerCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND siteIndex8=s.SiteIndex8	) AS OwnerIndex
      ,C.[ContractID] AS [DocumentNumber] 
      ,C.[CustomerName] AS [ContactFirstName] 
      ,C.[PointOfContact] AS [ContactSurname] 
      ,C.[datefrom] AS [Date01]
      ,C.[dateto] AS [Date02]
	,0														AS Flag05
	,0														AS ErrorFlag
  	,C.Deleteflag										AS Deleteflag
    
      ,C.[Phone] AS [ContactInfo] 
		,C.[AdditionalInfo]									AS [AdditionalInformation]
      ,C.[Address3]											AS [UserData1]
      ,C.[EMail]											AS [UserData2] 
      ,C.[Rid]												AS [UserData3] 
      ,C.[AttentionTo]										AS [UserData4]  
      ,C.[RICFrom]											AS [UserData7] 
      ,C.[Address1]											AS [UserData8]
      ,C.[Address2]											AS [UserData9]
      ,C.[City]												AS [UserData10]
      ,C.[State]											AS [UserData11]
      ,C.[Zip]												AS [UserData12]
      ,1													AS	OriginApplication --Added by Eric Simmons on 4/5/2010 
	,LTRIM(RTRIM(C.[BuyerAccount]))						AS ShipToID -- Added CHK 4.11.2010
	,(Select top 1 LTRIM(RTRIM(Code)) from tblCompanies where  LTRIM(RTRIM(C.[BuyerAccount])) =  LTRIM(RTRIM(ID))) AS ShipToCode
	,(Select top 1 CompanyIndex from tblCompanies where  LTRIM(RTRIM(C.[BuyerAccount])) =  LTRIM(RTRIM(ID))) AS ShipToIndex
	,LTRIM(RTRIM(C.[sub]))						AS BillToID -- Added CHK 4.11.2010
	,(Select top 1 LTRIM(RTRIM(Code)) from tblCompanies where  LTRIM(RTRIM(C.[sub])) =  LTRIM(RTRIM(ID))) AS BillToCode
	,(Select top 1 CompanyIndex from tblCompanies where  LTRIM(RTRIM(C.[sub])) =  LTRIM(RTRIM(ID))) AS BillToIndex
	,SubString(Replace(Replace(Replace(isnull(C.[Notes],''),';',''),':',''),'''',''),1,1000)								As Notes	-- Added CHK 4.11.2010
  
  FROM [AccountingDB6].[dbo].[t_Acct_Contracts] C 
	JOIN #TMPSITES s ON s.SiteIndex6 = C.SiteIndex
  WHERE C.ContractType = 'C' 

  
DROP TABLE #TMP_COMPANIES
INSERT INTO [ConsolidatedDB].[dbo].[tblTransactions](
	   [TransID]
      ,[AliasName]
      ,[AliasIndex]
      ,[TransTypeID]
      ,[SubType]
      ,[Site]
      ,[SiteIndex]
      ,[TransReferenceID]
      ,[InventoryDate]
      ,[ShipToID]
      ,[ShipToCode]
      ,[ShipToIndex]
      ,[SupplierID]
      ,[SupplierCode]
      ,[SupplierIndex]/**/
      ,[CreatedDate]
      ,[CreatedBy]
      ,[RequestedDeliveryDate]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[TransDateTime]
      ,[TransVersion]
      ,[SCACCode]
      ,[CardNumber]
      ,[ShipmentNumber]
      ,[ShipperID]
      ,[ShipperCode]
      ,[ShipperIndex]
      ,[OwnerID]
      ,[OwnerCode]
      ,[OwnerIndex]
      ,[ManagerID]
      ,[ManagerCode]
      ,[ManagerIndex]
      ,[CarrierID]
      ,[CarrierCode]
      ,[CarrierIndex]
      ,[ConjoinTransID]
      ,[ReversedTransID]
      ,[LinkedDocumentNumber]
      ,[ReversalType]
      ,[PONumber]
      ,[TimeIn]
      ,[TimeOut]
      ,[TimeEnd]
      ,[RoutingID]
      ,[TicketSource]
      ,[LoadID]
      ,[TransactionStatus]
      ,[BillToID]
      ,[BillToCode]
      ,[BillToIndex]
      ,[DriverIdentificationNumber]
      ,[CreditAmount]
      ,[CardExpiration]
      ,[CardName]
      ,[CardType]
      ,[CashAmount]
      ,[RouteOriginationDate]
      ,[InternationalRouteIndicator]
      ,[PreviousRoutingID]
      ,[FinalStationIndex]
      ,[PreviousStationIndex]
      ,[NextStationIndex]
      ,[OriginStationIndex]
      ,[ShippingDocumentNumber]
      ,[DocumentNumber]
      ,[STD]
      ,[ETD]
      ,[STA]
      ,[ETA]
      ,[SFT]
      ,[FST]
      ,[EstimatedFuelingDuration]
      ,[DeleteFlag]
      ,[TicketMode]
      ,[DestinationRegistrationID1]
      ,[DestinationSerialNumber1]
      ,[DestinationEquipmentType1]
      ,[DestinationEquipmentModel1]
      ,[DestinationCompanyEquipmentID1]
      ,[DestinationEquipmentIndex1]
      ,[DestinationRegistrationID2]
      ,[DestinationSerialNumber2]
      ,[DestinationEquipmentType2]
      ,[DestinationEquipmentModel2]
      ,[DestinationCompanyEquipmentID2]
      ,[DestinationEquipmentIndex2]
      ,[DestinationRegistrationID3]
      ,[DestinationSerialNumber3]
      ,[DestinationEquipmentType3]
      ,[DestinationEquipmentModel3]
      ,[DestinationCompanyEquipmentID3]
      ,[DestinationEquipmentIndex3]
      ,[SourceRegistrationID1]
      ,[SourceSerialNumber1]
      ,[SourceEquipmentType1]
      ,[SourceEquipmentModel1]
      ,[SourceCompanyEquipmentID1]
      ,[SourceEquipmentIndex1]
      ,[SourceRegistrationID2]
      ,[SourceSerialNumber2]
      ,[SourceEquipmentType2]
      ,[SourceEquipmentModel2]
      ,[SourceCompanyEquipmentID2]
      ,[SourceEquipmentIndex2]
      ,[SourceRegistrationID3]
      ,[SourceSerialNumber3]
      ,[SourceEquipmentType3]
      ,[SourceEquipmentModel3]
      ,[SourceCompanyEquipmentID3]
      ,[SourceEquipmentIndex3]
      ,[OperatorID]
      ,[OperatorIndex]
      ,[EffectiveDate]
      ,[ExpirationDate]
      ,[ScheduledDate]
      ,[AutoComplete]
      ,[Flag01]
      ,[Flag02]
      ,[Flag03]
      ,[Flag04]
      ,[Flag05]
      ,[Flag06]
      ,[Number01]
      ,[Number02]
      ,[Number03]
      ,[Number04]
      ,[Number05]
      ,[Number06]
      ,[ContactFirstName]
      ,[ContactSurname]
      ,[Date01]
      ,[Date02]
      ,[Date03]
      ,[Date04]
      ,[LegacyNumber]
      ,[Country]
      ,[ContactInfo]
      ,[AssociatedDocNumber]
      ,[AssociatedCLIN]
      ,[SubmittedToAccounting]
      ,[OriginApplication]
      ,[FuelCardIndex]
      ,[FuelCardID]
      ,[AssociatedTransportOrderNumber]
      ,[RequestedDateTime]
      ,[DispatchedDateTime]
      ,[ErrorFlag]/**/
)
SELECT
	  [TransID]
      ,[AliasName]
      ,[AliasIndex]
      ,[TransTypeID]
      ,[SubType]
      ,[Site]
      ,[SiteIndex]
      ,[TransReferenceID]
      ,[InventoryDate]
      ,[ShipToID]
      ,[ShipToCode]
      ,[ShipToIndex]
      ,[SupplierID]
      ,[SupplierCode]
      ,[SupplierIndex]/**/
      ,[CreatedDate]
      ,[CreatedBy]
      ,[RequestedDeliveryDate]
      ,[UpdatedDate]
      ,[UpdatedBy]
      ,[TransDateTime]
      ,[TransVersion]
      ,[SCACCode]
      ,[CardNumber]
      ,[ShipmentNumber]
      ,[ShipperID]
      ,[ShipperCode]
      ,[ShipperIndex]
      ,[OwnerID]
      ,[OwnerCode]
      ,[OwnerIndex]
      ,[ManagerID]
      ,[ManagerCode]
      ,[ManagerIndex]
      ,[CarrierID]
      ,[CarrierCode]
      ,[CarrierIndex]
      ,[ConjoinTransID]
      ,[ReversedTransID]
      ,[LinkedDocumentNumber]
      ,[ReversalType]
      ,[PONumber]
      ,[TimeIn]
      ,[TimeOut]
      ,[TimeEnd]
      ,[RoutingID]
      ,[TicketSource]
      ,[LoadID]
      ,[TransactionStatus]
      ,[BillToID]
      ,[BillToCode]
      ,[BillToIndex]
      ,[DriverIdentificationNumber]
      ,[CreditAmount]
      ,[CardExpiration]
      ,[CardName]
      ,[CardType]
      ,[CashAmount]
      ,[RouteOriginationDate]
      ,[InternationalRouteIndicator]
      ,[PreviousRoutingID]
      ,[FinalStationIndex]
      ,[PreviousStationIndex]
      ,[NextStationIndex]
      ,[OriginStationIndex]
      ,[ShippingDocumentNumber]
      ,[DocumentNumber]
      ,[STD]
      ,[ETD]
      ,[STA]
      ,[ETA]
      ,[SFT]
      ,[FST]
      ,[EstimatedFuelingDuration]
      ,[DeleteFlag]
      ,[TicketMode]
      ,[DestinationRegistrationID1]
      ,[DestinationSerialNumber1]
      ,[DestinationEquipmentType1]
      ,[DestinationEquipmentModel1]
      ,[DestinationCompanyEquipmentID1]
      ,[DestinationEquipmentIndex1]
      ,[DestinationRegistrationID2]
      ,[DestinationSerialNumber2]
      ,[DestinationEquipmentType2]
      ,[DestinationEquipmentModel2]
      ,[DestinationCompanyEquipmentID2]
      ,[DestinationEquipmentIndex2]
      ,[DestinationRegistrationID3]
      ,[DestinationSerialNumber3]
      ,[DestinationEquipmentType3]
      ,[DestinationEquipmentModel3]
      ,[DestinationCompanyEquipmentID3]
      ,[DestinationEquipmentIndex3]
      ,[SourceRegistrationID1]
      ,[SourceSerialNumber1]
      ,[SourceEquipmentType1]
      ,[SourceEquipmentModel1]
      ,[SourceCompanyEquipmentID1]
      ,[SourceEquipmentIndex1]
      ,[SourceRegistrationID2]
      ,[SourceSerialNumber2]
      ,[SourceEquipmentType2]
      ,[SourceEquipmentModel2]
      ,[SourceCompanyEquipmentID2]
      ,[SourceEquipmentIndex2]
      ,[SourceRegistrationID3]
      ,[SourceSerialNumber3]
      ,[SourceEquipmentType3]
      ,[SourceEquipmentModel3]
      ,[SourceCompanyEquipmentID3]
      ,[SourceEquipmentIndex3]
      ,[OperatorID]
      ,[OperatorIndex]
      ,[EffectiveDate]
      ,[ExpirationDate]
      ,[ScheduledDate]
      ,[AutoComplete]
      ,[Flag01]
      ,[Flag02]
      ,[Flag03]
      ,[Flag04]
      ,[Flag05]
      ,[Flag06]
      ,[Number01]
      ,[Number02]
      ,[Number03]
      ,[Number04]
      ,[Number05]
      ,[Number06]
      ,[ContactFirstName]
      ,[ContactSurname]
      ,[Date01]
      ,[Date02]
      ,[Date03]
      ,[Date04]
      ,[LegacyNumber]
      ,[Country]
      ,[ContactInfo]
      ,[AssociatedDocNumber]
      ,[AssociatedCLIN]
      ,[SubmittedToAccounting]
      ,[OriginApplication]
      ,[FuelCardIndex]
      ,[FuelCardID]
      ,[AssociatedTransportOrderNumber]
      ,[RequestedDateTime]
      ,[DispatchedDateTime]
      ,[ErrorFlag]/**/
  FROM #TMP_TRANSACTIONS T 
  
  INSERT INTO [ConsolidatedDB].[dbo].[tblTransactionUserData](
	   [UserData1]
      ,[UserData2]
      ,[UserData3]
      ,[UserData4]
      ,[UserData5]
      ,[UserData7]
      ,[UserData8]
      ,[UserData9]
      ,[UserData10]
      ,[UserData11]
      ,[UserData12]
      ,[CreatedBy]
      ,[CreatedDate]
      ,[UpdatedBy]
      ,[UpdatedDate]
      ,[TransIndex]
      )
  SELECT
	   T.[UserData1]
      ,T.[UserData2]
      ,T.[UserData3]
      ,T.[UserData4]
      ,T.[UserData5]
      ,T.[UserData7]
      ,T.[UserData8]
      ,T.[UserData9]
      ,T.[UserData10]
      ,T.[UserData11]
      ,T.[UserData12]
      ,T.[CreatedBy]
      ,T.[CreatedDate]
      ,T.[UpdatedBy]
      ,T.[UpdatedDate]
      ,TT.[TransIndex]
  FROM  #TMP_TRANSACTIONS T JOIN [ConsolidatedDB].[dbo].tblTransactions TT ON T.TransID = TT.TransID

DECLARE @NextTransLineItemID int
SELECT @NextTransLineItemID=SequenceValue FROM dbo.tblAccountingSequences WHERE SequenceName = 'LineItemID';
IF @NextTransLineItemID IS NULL
BEGIN
	INSERT INTO dbo.tblAccountingSequences (SequenceName, SequenceValue)
	VALUES ('LineItemID', 1);
	SET @NextTransLineItemID = 1
END

CREATE TABLE #TMP_TRANSACTIONLINEITEMS (
	TransLineItemID		bigint identity(1, 1), 
	TransIndex			bigint, 
	CreatedBy			nvarchar(100),
	CreatedDate			datetime,
	UpdatedBy			nvarchar(100),
	UpdatedDate			datetime,
	UserData1			nvarchar(60),
	[ProductID] nvarchar(30),
	[ProductIndex] int,
	[ProductType] nvarchar(20),
	[ProductCode] nvarchar(15),
	[BatchNumber] nvarchar(20),
	[ProductPrice]		float NULL,
	[CLIN]				nvarchar(10) NULL,
	[Sequence]          int
	)
	
INSERT INTO #TMP_TRANSACTIONLINEITEMS (
	TransIndex		
	,CreatedBy		
	,CreatedDate		
	,UpdatedBy		
	,UpdatedDate		
	,UserData1		
	,[ProductIndex]	
	,[ProductID]		
	,[ProductCode]	
	,[ProductType]	
	,[ProductPrice]	
	,[CLIN]			
	,[Sequence]
)
SELECT
	TT.TransIndex
	,'Varec'
	,GETDATE()
	,T.UpdatedBy
	,T.UpdatedDate
	,cast (A.AmountAuthorized as nvarchar(60))
	,P.[ProductIndex8]			
	,P.[ProductID]				
	,P.[ProductCode]				
	,CASE P.[ProductType]
		WHEN 1 THEN 'Blend'
		WHEN 2 THEN 'Additive'
		WHEN 3 THEN 'Additize'
		ELSE 'Component'
		END 
	,A.[UnitPrice]	
	,A.[ClinID] 
	,A.ClinIndex - (select MIN(cl.clinindex) from [AccountingDB6].[dbo].t_Acct_ContractLineItems cl where cl.ContractIndex = A.ContractIndex) + 1			
FROM [AccountingDB6].[dbo].t_Acct_ContractLineItems A 
JOIN #TMP_TRANSACTIONS T ON A.[ContractIndex] = T.[ContractIndex] -- CHK 4/9/2010 Fix from A.ContactIndex to A.ContractIndex - Don' rely on Intellisense!
JOIN [ConsolidatedDB].[dbo].tblTransactions TT ON T.[TransID] = TT.[TransID]
LEFT JOIN #TMP_PRODUCT_INDEX_MAP P ON A.ProductIndex = P.ProductIndex6

DROP TABLE #TMP_PRODUCT_INDEX_MAP

INSERT INTO [ConsolidatedDB].[dbo].[tblTransactionLineItems] (
	 TransLineItemID
	,TransIndex		
	,TransVersion 
	,[SequenceID]
	,[deleteflag]
	,[TransactionStatus]
	,[EngineeringUnitsIndex]
	,[LineItemSequenceNumber]
	,CreatedBy		
	,CreatedDate		
	,UpdatedBy		
	,UpdatedDate		
	,[Product]		
	,[ProductCode]	
	,[ProductType]	
	,[ProductPrice]	
	,[ProductIndex]	
	,[CLIN]			
)
SELECT
	TransLineItemID+@NextTransLineItemID
	,TransIndex		
	,@NextTransVersion 
	,Sequence
	,0
	,0
	,0
	,0
	,CreatedBy		
	,CreatedDate		
	,UpdatedBy		
	,UpdatedDate		
	,[ProductID]		
	,[ProductCode]	
	,[ProductType]	
	,[ProductPrice]	
	,[ProductIndex]	
	,[CLIN]			
FROM #TMP_TRANSACTIONLINEITEMS T 

--select * from [ConsolidatedDB].[dbo].[tblTransactionLineItems]
UPDATE dbo.tblAccountingSequences SET SequenceValue =  (SELECT SequenceValue +COUNT(*) FROM #TMP_TRANSACTIONLINEITEMS) 
	WHERE SequenceName = 'LineItemID';
	
INSERT INTO [ConsolidatedDB].[dbo].[tblTransactionLineItemUserData] (
	TransLineItemID,
	[UserData1],
	CreatedBy,
	CreatedDate,
	UpdatedBy,
	UpdatedDate
)
SELECT 
	TT.TransLineItemID,
	T.[UserData1],
	T.CreatedBy,
	T.CreatedDate,
	T.UpdatedBy,
	T.UpdatedDate
FROM #TMP_TRANSACTIONLINEITEMS T  JOIN [ConsolidatedDB].[dbo].tblTransactionLineItems TT 
ON T.TransLineItemID+@NextTransLineItemID = TT.TransLineItemID

INSERT INTO [ConsolidatedDB].[dbo].[tblTransactionTransportLineItems] (
	 [TransportOrderNumber]
	,[TransVersion]
	,[LocationName]
	,[Address1]
	,[Address2]
	,[City]
	,[State]
	,[Zip]
	,[POCName]
	,[POCPhone]
	,[CreatedBy]
	,[CreatedDate]
	,[UpdatedBy]
	,[UpdatedDate]
	,[TransIndex]
      )
SELECT 
	R.[rtord] 
	,NULL
	,LEFT(R.frmname, 30) 
	,R.frmadd1 
	,R.frmadd2 
	,LEFT(R.frmcity, 20) 
	,R.frmstate 
	,R.frmzip 
	,R.frmPOCName 
	,R.frmPOCPhone 
	,'Varec'
	,GETDATE()
	,R.UpdatedBy
	,R.UpdatedDate 
	,T.[TransIndex]
FROM #TMP_TRANSACTIONS TT  JOIN [AccountingDB6].[dbo].t_Acct_Routing R ON TT.[ContractIndex] = R.[ContractIndex]
JOIN [ConsolidatedDB].[dbo].[tblTransactions] T ON T.TransID=TT.TransID

  INSERT INTO [ConsolidatedDB].[dbo].[tblTransactionNotes](
		[AdditionalInformation]
      ,[TransIndex]
      ,[Notes]
      )
  SELECT 
 		T.[AdditionalInformation]
		,TT.[TransIndex]
		,T.[Notes]
  FROM  #TMP_TRANSACTIONS T JOIN [ConsolidatedDB].[dbo].tblTransactions TT ON T.TransID = TT.TransID

DROP TABLE #TMP_TRANSACTIONLINEITEMS

DROP TABLE #TMP_TRANSACTIONS
DROP TABLE #TMPSITES




	/*
IF @@TRANCOUNT > 0    
BEGIN     
--	  ROLLBACK TRANSACTION;   
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
END