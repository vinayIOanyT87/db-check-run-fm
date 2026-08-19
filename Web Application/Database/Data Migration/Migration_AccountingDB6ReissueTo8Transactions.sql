USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AccountingDB6ReissueTo8Transactions]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6ReissueTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AccountingDB6ReissueTo8Transactions
GO

CREATE PROCEDURE [dbo].Migration_AccountingDB6ReissueTo8Transactions 

 /*=============================================
 Author:			A. Coker
 Create date:		3/12/2010
Description:		Migrating AccountingDB6.0 Receive to ConsolidatedDB8.0 Transactions
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_AccountingDB6ReissueTo8Transactions 2, null 

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
	ProductType int NULL,
	UnitOfIssue nvarchar(50)
)

INSERT INTO #TMP_PRODUCT_INDEX_MAP
SELECT DISTINCT P8.ProductID, P8.ProductCode, P8.ProductIndex AS ProductIndex8, 
	P6.ProductID, P8.ProductType, P6.UnitOfIssue
FROM [ConsolidatedDB6].[dbo].tblProducts P6 JOIN [ConsolidatedDB].[dbo].tblProducts P8 
ON P6.PRODUCTCODE = P8.ProductID 
JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M ON M.[Index]=P8.[ProductIndex] AND TypeID='Products'
	JOIN #TMPSITES s ON s.SiteIndex6 = P6.SiteIndex AND M.SiteIndex=s.SiteIndex8
WHERE P6.DeleteFlag = 0 AND --P6.SiteIndex = @SiteIndex6 AND M.SiteIndex = @SiteIndex8 AND 
P6.ProductIndex  <> -1

INSERT INTO #TMP_PRODUCT_INDEX_MAP
SELECT DISTINCT P8.ProductID, P8.ProductCode, P8.ProductIndex AS ProductIndex8, 
	P6.ProductID, P8.ProductType, P6.UnitOfIssue 
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
SET @AliasName			= 'Reissue'

SELECT @AliasID=[AliasID], @TransTypeID=TransTypeID  FROM [ConsolidatedDB].[dbo].tblTransactionAliases WHERE [AliasName] = @AliasName

CREATE TABLE #TMP_TRANSACTIONS(
	
	[TransID] [nvarchar](64) NOT NULL,
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
	[TransactionStatus] [int] NULL DEFAULT 11,
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
    [UserData1] NVarchar(120) NULL,
    [UserData2] NVarchar(120) NULL, 
    [UserData3] NVarchar(120) NULL, 
    [UserData4] NVarchar(120) NULL,  
    [UserData5] NVarchar(120) NULL, 
    [UserData6] NVarchar(120) NULL, 
    [UserData7] NVarchar(120) NULL, 
    [UserData8] NVarchar(120) NULL,
    [UserData9] NVarchar(120) NULL,
    [UserData10] NVarchar(120) NULL,
    [UserData11] NVarchar(120) NULL,
    [UserData12] NVarchar(120) NULL, 
    [UserData13] NVarchar(120) NULL, 
    [UserData14] NVarchar(120) NULL, 
    [UserData15] NVarchar(120) NULL, 
    [UserData16] NVarchar(120) NULL, 
    [UserData17] NVarchar(120) NULL, 
    [UserData18] NVarchar(120) NULL, 
    [UserData19] NVarchar(120) NULL, 
    [UserData20] NVarchar(120) NULL, 
    [UserData21] NVarchar(120) NULL, 
	[GrossQuantity] float,
	[ProductID] nvarchar(30),
	[ProductIndex] int,
	[ProductType] int,
	[ProductCode] nvarchar(15),
	
	[TransIndex] [bigint] IDENTITY(1,1) NOT NULL)
	

INSERT INTO #TMP_TRANSACTIONS(
	[TransID]
	,[AliasName]
	,[AliasIndex]
	,[TransTypeID]
	,[Site] 
	,[SiteIndex]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	,[TransVersion] 
	,[InventoryDate]
	,[TransDateTime]	
	
	,ShipToID
	,ShipToCode
	,ShipToIndex
	, Flag05
	,ErrorFlag 
	
	,DestinationRegistrationID1
	,BillToID
	,BillToCode
	,BillToIndex
	,ManagerID
	,ManagerCode
	,ManagerIndex
	,OwnerID
	,OwnerCode
	,OwnerIndex
	,[DeleteFlag]

	,UserData1
	,UserData3
	,UserData4
	,UserData5
	,UserData11
	,UserData12
	,UserData13
	,UserData14
	,UserData15
	,UserData16
	,UserData17
	,UserData19			
	,UserData20			
	,UserData21			

	,ConjoinTransID
	,SubType
	,GrossQuantity
	,ProductID
	,ProductCode
	,ProductIndex
	,ProductType
	,OriginApplication --Added by Eric Simmons on 4/5/2010
)
SELECT 
	C.[TransactionID]															AS [TransID]
	,@AliasName																	AS [AliasName]
	,@AliasID																	AS [AliasIndex]
	,@TransTypeID																AS [TransTypeID]
	,S.SiteID8																	AS [Site] 
	,S.SiteIndex8																AS [SiteIndex]
	,GETDATE()																	AS [CreatedDate]
	,'Varec'																		AS [CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]

	,@NextTransVersion														AS [TransVersion] 
	,CONVERT(Date,[TransactionDate]) 									AS	[InventoryDate]
	,[TransactionDate]														AS [TransDateTime]

	,C.[ToConsumer]															AS ShipToID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID=C.[ToConsumer] AND siteIndex8=s.SiteIndex8)	AS ShipToCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID=C.[ToConsumer] AND siteIndex8=s.SiteIndex8)	AS ShipToIndex
	--,C.[Flag1]									AS Flag01
	--,C.[Flag2]									AS Flag02
	,C.[SentToEnterpriseFlag]												AS Flag05
	,C.[ErrorFromEnterpriseFlag]											AS ErrorFlag

	,C.[ReceivingEqID]							AS DestinationRegistrationID1
	,C.[UserData1]								AS BillToID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID=C.[UserData1] AND S.SiteIndex8 = SiteIndex8)		AS BillToCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID=C.[UserData1] AND S.SiteIndex8 = SiteIndex8)	AS BillToIndex
	,'DESC'								AS ManagerID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND S.SiteIndex8 = SiteIndex8)				AS ManagerCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND S.SiteIndex8 = SiteIndex8)				AS ManagerIndex
	,'DESC'									AS OwnerID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND S.SiteIndex8 = SiteIndex8)				AS OwnerCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND S.SiteIndex8 = SiteIndex8)				AS OwnerIndex
	,C.[DeleteFlag]															AS [DeleteFlag]
	

	,P.[UnitOfIssue]															AS UserData1
	,C.[UserData2]																AS UserData3
	,C.[UserData22]															AS UserData4
	,C.[UserData4]																AS UserData5
	,C.[UserData11]															AS UserData11
	,C.[UserData12]															AS UserData12
	,C.[UserData13]															AS UserData13
	,C.[UserData14]															AS UserData14
	,C.[UserData15]															AS UserData15
	,C.[UserData18]															AS UserData16
	,C.[UserData17]															AS UserData17
	,C.[SubtypeCode1]															AS UserData19
	,C.[SubtypeCode2]															AS UserData20
	,C.[SubtypeCode3]															AS UserData21
	
	
	,NEWID()
	,'D'
	,-dbo.ConvertToSIUnits(ABS(Isnull(C.[GrossVolume],0)),42/*liters*/)	AS GrossQuantity
	
	,P.[ProductID]																AS	ProductID
	,P.[ProductCode]															AS	ProductCode
	,P.[ProductIndex8]														AS	ProductIndex
	,P.[ProductType]															AS	ProductType
	,1																				AS	OriginApplication --Added by Eric Simmons on 4/5/2010																
  FROM [AccountingDB6].[dbo].[t_Acct_Tx11] C LEFT JOIN #TMP_PRODUCT_INDEX_MAP P ON C.ProductID = P.ProductID6					
 	JOIN #TMPSITES s ON s.SiteIndex6 = C.SiteIndex
 WHERE --SiteIndex=@SiteIndex6 AND 
 C.Alias = 'REISSUE'
  AND transactiondate BETWEEN '1900-01-01' AND '2079-06-06'

INSERT INTO #TMP_TRANSACTIONS(
	[TransID]
	,[AliasName]
	,[AliasIndex]
	,[TransTypeID]
	,[Site] 
	,[SiteIndex]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	,[TransVersion] 
	,[InventoryDate]
	,[TransDateTime]	
	
	,ShipToID
	,ShipToCode
	,ShipToIndex
	, Flag05
	,ErrorFlag 
	
	,DestinationRegistrationID1
	,BillToID
	,BillToCode
	,BillToIndex
	,ManagerID
	,ManagerCode
	,ManagerIndex
	,OwnerID
	,OwnerCode
	,OwnerIndex
	,[DeleteFlag]

	,UserData1
	,UserData3
	,UserData4
	,UserData5
	,UserData11
	,UserData12
	,UserData13
	,UserData14
	,UserData15
	,UserData16
	,UserData17
	,UserData19			
	,UserData20			
	,UserData21			

	,ConjoinTransID
	,SubType
	,GrossQuantity
	,ProductID
	,ProductCode
	,ProductIndex
	,ProductType
	,OriginApplication --Added by Eric Simmons on 4/5/2010
)
SELECT 
	(SELECT ConjoinTransID FROM #TMP_TRANSACTIONS x WHERE TransID=C.[TransactionID])	AS [TransID]
	,@AliasName																			AS [AliasName]
	,@AliasID																			AS [AliasIndex]
	,@TransTypeID																		AS [TransTypeID]
	,S.SiteID8																			AS [Site] 
	,S.SiteIndex8																		AS [SiteIndex]
	,GETDATE()																			AS [CreatedDate]
	,'Varec'																			AS [CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]

	,@NextTransVersion																	AS [TransVersion] 
	,CONVERT(Date,[TransactionDate])													AS	[InventoryDate]
	,[TransactionDate]																	AS [TransDateTime]

	,C.[ToConsumer]																		AS ShipToID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID=C.[ToConsumer] AND S.SiteIndex8=SiteIndex8)			AS ShipToCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID=C.[ToConsumer] AND siteIndex8=s.SiteIndex8)			AS ShipToIndex
	--,C.[Flag1]									AS Flag01
	--,C.[Flag2]									AS Flag02
	,C.[SentToEnterpriseFlag]															AS Flag05
	,C.[ErrorFromEnterpriseFlag]														AS ErrorFlag

	,C.[ReceivingEqID]																	AS DestinationRegistrationID1
	,C.[UserData1]																		AS BillToID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID=C.[UserData1] AND S.SiteIndex8=SiteIndex8)	AS BillToCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID=C.[UserData1] AND S.SiteIndex8=SiteIndex8)			AS BillToIndex
	,'DESC'																				AS ManagerID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND S.SiteIndex8=SiteIndex8)					AS ManagerCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND S.SiteIndex8=SiteIndex8)					AS ManagerIndex
	,'DESC'																				AS OwnerID
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND S.SiteIndex8=SiteIndex8)					AS OwnerCode
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND S.SiteIndex8=SiteIndex8)					AS OwnerIndex
	,C.[DeleteFlag]																		AS [DeleteFlag]
	

	,P.[UnitOfIssue]																	AS UserData1
	,C.[UserData2]																		AS UserData3
	,C.[UserData22]																		AS UserData4
	,C.[UserData4]																		AS UserData5
	,C.[UserData11]																		AS UserData11
	,C.[UserData12]																		AS UserData12
	,C.[UserData13]																		AS UserData13
	,C.[UserData14]																		AS UserData14
	,C.[UserData15]																		AS UserData15
	,C.[UserData18]																		AS UserData16
	,C.[UserData17]																		AS UserData17
	,C.[SubtypeCode1]																	AS UserData19
	,C.[SubtypeCode2]																	AS UserData20
	,C.[SubtypeCode3]																	AS UserData21
	
	
	,C.TransactionID
	,'C'
	,dbo.ConvertToSIUnits(ABS(Isnull(C.[GrossVolume],0)),42/*liters*/)									AS GrossQuantity
	
	,P.[ProductID]																		AS	ProductID
	,P.[ProductCode]																	AS	ProductCode
	,P.[ProductIndex8]																	AS	ProductIndex
	,P.[ProductType]																	AS	ProductType
	,1																					AS	OriginApplication --Added by Eric Simmons on 4/5/2010
  FROM [AccountingDB6].[dbo].[t_Acct_Tx11] C LEFT JOIN #TMP_PRODUCT_INDEX_MAP P ON C.ProductID = P.ProductID6					
	JOIN #TMPSITES s ON s.SiteIndex6 = C.SiteIndex
  WHERE --SiteIndex=@SiteIndex6 AND 
  C.Alias = 'REISSUE'
  AND transactiondate BETWEEN '1900-01-01' AND '2079-06-06'


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
      ,[SupplierIndex]
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
      ,[SupplierIndex]
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
      ,[UserData6]
      ,[UserData7]
      ,[UserData8]
      ,[UserData9]
      ,[UserData10]
      ,[UserData11]
      ,[UserData12]
      ,[UserData13]
      ,[UserData14]
      ,[UserData15]
      ,[UserData16]
      ,[UserData17]
      ,[UserData18]
      ,[UserData19]
      ,[UserData20]
      ,[UserData21]
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
      ,T.[UserData6]
      ,T.[UserData7]
      ,T.[UserData8]
      ,T.[UserData9]
      ,T.[UserData10]
      ,T.[UserData11]
      ,T.[UserData12]
      ,T.[UserData13]
      ,T.[UserData14]
      ,T.[UserData15]
      ,T.[UserData16]
      ,T.[UserData17]
      ,T.[UserData18]
      ,T.[UserData19]
      ,T.[UserData20]
      ,T.[UserData21]
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
	TransLineItemID bigint identity(1, 1), 
	TransIndex bigint, 
	TransactionInventoryDate datetime,
	[GrossQuantity] float,
	[NetQuantity] float,
	[ProductID] nvarchar(30),
	[ProductIndex] int,
	[ProductType] nvarchar(20),
	[ProductCode] nvarchar(15),
	CreatedBy nvarchar(100),
	CreatedDate datetime,
	UpdatedBy nvarchar(100),
	UpdatedDate datetime
	)
	
INSERT INTO #TMP_TRANSACTIONLINEITEMS (
	 TransIndex
	,[TransactionInventoryDate]
	,[GrossQuantity]	
	,[NetQuantity]	
	,[ProductID]		
	,[ProductIndex] 
	,[ProductType] 
	,[ProductCode] 
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
)
SELECT
	 TT.TransIndex
	,T.[InventoryDate]
	,T.[GrossQuantity] 
	,T.[GrossQuantity] 
	,T.[ProductID]
	,T.[ProductIndex] 
	,CASE T.[ProductType]
		WHEN 1 THEN 'Blend'
		WHEN 2 THEN 'Additive'
		WHEN 3 THEN 'Additize'
		ELSE 'Component'
		END 
	,T.[ProductCode] 
	,'Varec'
	,GETDATE()
	,T.UpdatedBy
	,T.UpdatedDate
	
FROM #TMP_TRANSACTIONS T JOIN [ConsolidatedDB].[dbo].tblTransactions TT ON T.TransID = TT.TransID


INSERT INTO [ConsolidatedDB].[dbo].[tblTransactionLineItems] (
	 TransLineItemID
	,TransIndex
	,TransVersion 
	,[SequenceID]
	,[deleteflag]
	,[TransactionStatus]
	,[EngineeringUnitsIndex]
	,[LineItemSequenceNumber]
	,[TransactionInventoryDate]	
	,[GrossQuantity]	
	,[NetQuantity]	
	,[Product]
	,[ProductIndex] 
	,[ProductType] 
	,[ProductCode] 
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
)
SELECT
	TransLineItemID+@NextTransLineItemID
	,TransIndex
	,@NextTransVersion 
	,0
	,0
	,0
	,0
	,0
	,[TransactionInventoryDate]	
	,[GrossQuantity] 
	,[GrossQuantity] 
	,[ProductID]
	,[ProductIndex] 
	,[ProductType] 
	,[ProductCode] 
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
FROM #TMP_TRANSACTIONLINEITEMS T 
--select * from [ConsolidatedDB].[dbo].[tblTransactionLineItems]
UPDATE dbo.tblAccountingSequences SET SequenceValue =  (SELECT SequenceValue +COUNT(*) FROM #TMP_TRANSACTIONLINEITEMS) 
	WHERE SequenceName = 'LineItemID';

DROP TABLE #TMP_TRANSACTIONLINEITEMS
DROP TABLE #TMP_TRANSACTIONS
DROP TABLE #TMP_PRODUCT_INDEX_MAP
DROP TABLE #TMPSITES

/*
FETCH NEXT FROM SiteIndexes_Cursor INTO @SiteID8, @SiteIndex6 , @SiteIndex8
END 
CLOSE SiteIndexes_Cursor 
DEALLOCATE SiteIndexes_Cursor; 
*/

/*	
IF @@TRANCOUNT > 0    
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
END