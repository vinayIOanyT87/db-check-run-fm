USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_AviationDB6ControlLogTo8Transactions]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6ControlLogTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_AviationDB6ControlLogTo8Transactions
GO

CREATE PROCEDURE [dbo].Migration_AviationDB6ControlLogTo8Transactions 

 /*=============================================
 Author:			A. Coker
 Create date:		3/12/2010
Description:		Migrating AviationDB6.0 Control Log to ConsolidatedDB8.0 Transactions
 Modification History:
	Date		by		Description
	
	04-08-2010  E. Simmons Updated to handle cancelled sale and defuel.  Upate all dates and times to UTC. Fixed mapping issues.
	
 =============================================*/
/*

EXEC Migration_AviationDB6ControlLogTo8Transactions 1, 'FP5518'

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarChar(MAX) = NULL

AS 
BEGIN

IF @IsBaseDB = 2
BEGIN
	RETURN
END

SET @SiteID = NULL

SELECT S8.[ID] AS SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex


/*	-- Left it commented out rather than delete so that it can be used when debugging.
	
BEGIN TRY
BEGIN TRANSACTION

*/
CREATE TABLE #TMP_OPERATORS
(
	SiteIndex int,
	PersonIndex int,
	PersonID NVarChar(50),
	OperatorID NVarchar(30)
)

INSERT INTO  #TMP_OPERATORS (SiteIndex, PersonIndex, OperatorID) 
SELECT M.SiteIndex AS SiteIndex8, PersonIndex, PersonID FROM [ConsolidatedDB].[dbo].[tblPersonnel] P 
JOIN [ConsolidatedDB].[dbo].[tblEntityToSiteMap] M ON P.PersonIndex=M.[Index] AND TypeID='Personnel'
	JOIN #TMPSITES s ON M.SiteIndex=s.SiteIndex8

	
UPDATE #TMP_OPERATORS SET PersonID = P.PersonID
FROM [ConsolidatedDB].[dbo].[tblPersonnel] P WHERE P.PersonIndex=#TMP_OPERATORS.PersonIndex

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
JOIN #TMPSITES S ON P6.SiteIndex = S.SiteIndex6 AND M.SiteIndex = S.SiteIndex8 
WHERE P6.DeleteFlag = 0 AND --P6.SiteIndex = @SiteIndex6 AND M.SiteIndex = @SiteIndex8 AND 
P6.ProductIndex  <> -1

INSERT INTO #TMP_PRODUCT_INDEX_MAP
SELECT P8.ProductID, P8.ProductCode, P8.ProductIndex AS ProductIndex8, 
		P6.ProductID, P6.ProductIndex AS ProductIndex6, P8.ProductType, P6.UnitOfIssue 
FROM [ConsolidatedDB6].[dbo].tblProducts P6 JOIN [ConsolidatedDB].[dbo].tblProducts P8 
ON P6.ProductIndex = P8.ProductIndex 
WHERE P8.ProductIndex = -1

SELECT S.SiteIndex8 AS SiteIndex8, CompanyIndex, [ID] AS CompanyID, Code AS CompanyCode INTO #TMP_COMPANIES 
FROM [ConsolidatedDB].[dbo].tblCompanies C JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M 
ON C.CompanyIndex = M.[Index] AND TypeID='Companies' , #TMPSITES s
WHERE M.SiteIndex = CASE WHEN @IsBaseDB = 0 THEN s.SiteIndex8 ELSE -1 END

SELECT S.SiteIndex8 AS SiteIndex8, E.[Index] AS EquipmentIndex, E.[ID] AS EquipmentID INTO #TMP_EQUIPMENT
FROM [ConsolidatedDB].[dbo].tblEquipment E JOIN  [ConsolidatedDB].[dbo].tblEntityToSiteMap M 
ON E.[Index] = M.[Index] AND TypeID='Equipment' 
JOIN #TMPSITES S ON M.SiteIndex = S.SiteIndex8 


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
DECLARE @AliasNameFillStand		nvarchar(30)
DECLARE @AliasNameRecirculation		nvarchar(30)
DECLARE @AliasNameRTB		nvarchar(30)
DECLARE @AliasNameSale		nvarchar(30);
Declare @AliasNameDefuel	nvarchar(30);
DECLARE @AliasIDFillStand		int
DECLARE @AliasIDRecirculation		int
DECLARE @AliasIDRTB		int
DECLARE @AliasIDSale	int
DECLARE @AliasIDDefuel	int
DECLARE @TransTypeIDFillStand	smallint
DECLARE @TransTypeIDRecirculation	smallint
DECLARE @TransTypeIDRTB	smallint
DECLARE @TransTypeIDSale smallint
DECLARE @TransTypeIDDefuel	smallint

SET @ManagerID			='DESC'																	
SET @OwnerID			='DESC'																	
SET @AliasNameFillStand				= 'Fillstand'
SET @AliasNameRecirculation			= 'Recirculation'
SET @AliasNameRTB					= 'Return to Bulk'
Set @AliasNameSale					= 'Sale'
Set @AliasNameDefuel				= 'Defuel'

SELECT top 1 @AliasIDFillStand=[AliasID], @TransTypeIDFillStand=TransTypeID  FROM [ConsolidatedDB].[dbo].tblTransactionAliases WHERE [AliasName] = @AliasNameFillStand
SELECT top 1 @AliasIDRecirculation=[AliasID], @TransTypeIDRecirculation=TransTypeID  FROM [ConsolidatedDB].[dbo].tblTransactionAliases WHERE [AliasName] = @AliasNameRecirculation
SELECT top 1 @AliasIDRTB=[AliasID], @TransTypeIDRTB=TransTypeID  FROM [ConsolidatedDB].[dbo].tblTransactionAliases WHERE [AliasName] = @AliasNameRTB
SELECT top 1 @AliasIDSale=[AliasID], @TransTypeIDSale=TransTypeID  FROM [ConsolidatedDB].[dbo].tblTransactionAliases WHERE [AliasName] = @AliasNameSale
SELECT top 1 @AliasIDDefuel=[AliasID], @TransTypeIDDefuel=TransTypeID  FROM [ConsolidatedDB].[dbo].tblTransactionAliases WHERE [AliasName] = @AliasNameDefuel

SELECT 
	[TRansaction_ID]									AS [TransID]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'RECIRC'
		THEN @AliasNameRecirculation
		WHEN 'FILL STAND'	
		THEN @AliasNameFillStand
		WHEN 'RETURN TO BULK'
		THEN @AliasNameRTB
		WHEN 'REFUEL'
		THEN @AliasNameSale
		WHEN 'DEFUEL'
		THEN @AliasNameDefuel
		ELSE [REQUEST_TYPE]
	END													AS [AliasName]	--fillstand, RTB, Recirculation	 
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'RECIRC'
		THEN @AliasIDRecirculation
		WHEN 'FILL STAND'	
		THEN @AliasIDFillStand
		WHEN 'RETURN TO BULK'
		THEN @AliasIDRTB
		WHEN 'REFUEL'
		THEN @AliasIDSale
		WHEN 'DEFUEL'
		THEN @AliasIDDefuel
	END													AS [AliasIndex]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'RECIRC'
		THEN @TransTypeIDRecirculation
		WHEN 'FILL STAND'	
		THEN @TransTypeIDFillStand
		WHEN 'RETURN TO BULK'
		THEN @TransTypeIDRTB
		WHEN 'REFUEL'
		THEN @TransTypeIDSale
		WHEN 'DEFUEL'
		THEN @TransTypeIDDefuel
	END													AS [TransTypeID]
	,'' AS [SubType]
	, S.SiteID8											AS [Site]
	, S.SiteIndex8										AS [SiteIndex] 
	,''													AS [TransReferenceID]
	,CONVERT(Date, Isnull(C.INVENTORY_DATE,C.TRANSACTION_DATE)) AS [InventoryDate]		--fillstand, RTB, Recirculation
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN C.CUSTOMER_ID
		WHEN 'DEFUEL'
		THEN C.CUSTOMER_ID
		ELSE ''
	END													AS [ShipToID]			
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN Isnull((Select top 1 Code from tblCompanies where [ID] = C.CUSTOMER_ID),'')
		WHEN 'DEFUEL'
		THEN Isnull((Select top 1 Code from tblCompanies where [ID] = C.CUSTOMER_ID),'')
		ELSE ''
	END																AS [ShipToCode]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN (Select top 1 CompanyIndex from tblCompanies where [ID] = C.CUSTOMER_ID)
		WHEN 'DEFUEL'
		THEN (Select top 1 CompanyIndex from tblCompanies where [ID] = C.CUSTOMER_ID)
		ELSE NULL
	END																AS [ShipToIndex]
	,''																AS [SupplierID]
	,''																AS [SupplierCode]
	,NULL																AS [SupplierIndex]
	,dbo.GetUTCTime(S.SiteIndex8,GetDate())					AS [CreatedDate]
	,'Varec'															AS [CreatedBy] 
	,CONVERT(date,C.REQUEST_TIME)								AS [RequestedDeliveryDate]
	,dbo.GetUTCTime(S.SiteIndex8,C.[UpdatedDate])			AS [UpdatedDate]
	,LEFT(C.[UpdatedBy],30)										AS [UpdatedBy]
	,dbo.GetUTCTime(S.SiteIndex8,C.[Transaction_Date])	AS [TransDateTime]
	,@NextTransVersion											AS [TransVersion]
	,''																AS [SCACCode]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN C.[USERDATA_4] 
		WHEN 'DEFUEL'
		THEN C.[USERDATA_4]
		WHEN 'RECIRC' 
		THEN C.[USERDATA_4]	
		ELSE ''
		END															AS [CardNumber]			
	,''																AS [ShipmentNumber]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN Isnull(C.SELLERS_ID,'')
		WHEN 'DEFUEL'
		THEN Isnull(C.SELLERS_ID,'')
		ELSE ''
	END																AS [ShipperID]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN Isnull((Select top 1 Code from tblCompanies where [ID] = C.SELLERS_ID),'')
		WHEN 'DEFUEL'
		THEN Isnull((Select top 1 Code from tblCompanies where [ID] = C.SELLERS_ID),'')
		ELSE ''
	END																AS [ShipperCode]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN (Select top 1 CompanyIndex from tblCompanies where [ID] = C.SELLERS_ID)
		WHEN 'DEFUEL'
		THEN (Select top 1 CompanyIndex from tblCompanies where [ID] = C.SELLERS_ID)
		ELSE NULL
	END																AS [ShipperIndex]
	,@OwnerID														AS [OwnerID]
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND siteIndex8=s.SiteIndex8	) AS [OwnerCode]
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND siteIndex8=s.SiteIndex8	) AS [OwnerIndex]
	,@ManagerID														AS [ManagerID]
	,(SELECT CompanyCode FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND siteIndex8=s.SiteIndex8	) AS [ManagerCode]
	,(SELECT CompanyIndex FROM #TMP_COMPANIES WHERE CompanyID='DESC' AND siteIndex8=s.SiteIndex8	) AS [ManagerIndex]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN Isnull(C.VENDOR,'')
		WHEN 'DEFUEL'
		THEN Isnull(C.VENDOR,'')
		ELSE ''
	END																AS [CarrierID]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN Isnull((Select top 1 Code from tblCompanies where [ID] = C.VENDOR),'')
		WHEN 'DEFUEL'
		THEN Isnull((Select top 1 Code from tblCompanies where [ID] = C.VENDOR),'')
		ELSE ''
	END																AS [CarrierCode]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN (Select top 1 CompanyIndex from tblCompanies where [ID] = C.VENDOR)
		WHEN 'DEFUEL'
		THEN (Select top 1 CompanyIndex from tblCompanies where [ID] = C.VENDOR)
		ELSE NULL
	END																AS [CarrierIndex]
	,''																AS [ConjoinTransID]
	,''																AS [ReversedTransID]
	,''																AS [LinkedDocumentNumber]
	,''																AS [ReversalType]
	,''																AS [PONumber]
	,CASE   UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND'
		THEN dbo.GetUTCTime(S.SiteIndex8,[ARR_TIME])
		WHEN 'RETURN TO BULK'
		THEN dbo.GetUTCTime(S.SiteIndex8,[ARR_TIME])
		WHEN 'REFUEL'
		THEN dbo.GetUTCTime(S.SiteIndex8,[ARR_TIME])
		WHEN 'DEFUEL'
		THEN dbo.GetUTCTime(S.SiteIndex8,[ARR_TIME])
		ELSE NULL
		END															AS [TimeIn]					--fillstand, RTB,
	,CASE  UPPER(REQUEST_TYPE)
		WHEN 'FILL STAND'
		THEN dbo.GetUTCTime(S.SiteIndex8,[DEP_TIME])
		WHEN 'RETURN TO BULK'
		THEN dbo.GetUTCTime(S.SiteIndex8,[DEP_TIME])
		WHEN 'REFUEL'
		THEN dbo.GetUTCTime(S.SiteIndex8,[DEP_TIME])
		WHEN 'DEFUEL'
		THEN dbo.GetUTCTime(S.SiteIndex8,[DEP_TIME])
		ELSE NULL
		END															AS [TimeOut]				--fillstand, RTB, 
	,CASE  UPPER(REQUEST_TYPE)
		WHEN 'FILL STAND'
		THEN dbo.GetUTCTime(S.SiteIndex8,[STOP_TIME])
		WHEN 'RETURN TO BULK'
		THEN dbo.GetUTCTime(S.SiteIndex8,[STOP_TIME])
		WHEN 'REFUEL'
		THEN dbo.GetUTCTime(S.SiteIndex8,[STOP_TIME])
		WHEN 'DEFUEL'
		THEN dbo.GetUTCTime(S.SiteIndex8,[STOP_TIME])
		ELSE NULL
		END															AS [TimeEnd]				--fillstand, RTB,
	,''																AS [RoutingID]
	,''																AS [TicketSource]
	,''																AS [LoadID]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND'  
		THEN (CASE C.[CANCELLED] WHEN 1 THEN 7 ELSE 0 END)
		WHEN 'RETURN TO BULK'  
		THEN (CASE C.[CANCELLED] WHEN 1 THEN 7 ELSE 0 END)
		WHEN 'RECIRC'
		THEN (CASE C.[STATUS] WHEN 'Completed' THEN 0 ELSE 1 END)
		WHEN 'REFUEL'  
		THEN (CASE C.[CANCELLED] WHEN 1 THEN 7 ELSE 0 END)
		WHEN 'DEFUEL'  
		THEN (CASE C.[CANCELLED] WHEN 1 THEN 7 ELSE 0 END)
		ELSE 0
		END															AS [TransactionStatus]		--fillstand, RTB, Recirc
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN Isnull(C.SUB_ACCOUNT_NUM,'')
		WHEN 'DEFUEL'
		THEN Isnull(C.SUB_ACCOUNT_NUM,'')
		ELSE ''
	END																AS [BillToID]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN Isnull((Select top 1 Code from tblCompanies where [ID] = C.SUB_ACCOUNT_NUM),'')
		WHEN 'DEFUEL'
		THEN Isnull((Select top 1 Code from tblCompanies where [ID] = C.SUB_ACCOUNT_NUM),'')
		ELSE ''
	END																AS [BillToCode]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN (Select top 1 CompanyIndex from tblCompanies where [ID] = C.SUB_ACCOUNT_NUM)
		WHEN 'DEFUEL'
		THEN (Select top 1 CompanyIndex from tblCompanies where [ID] = C.SUB_ACCOUNT_NUM)
		ELSE NULL
	END																AS [BillToIndex]
	,''																AS [DriverIdentificationNumber]
	,NULL																AS [CreditAmount]
	,NULL																AS [CardExpiration]
	,''																AS [CardName]
	,''																AS [CardType]
	,NULL																AS [CashAmount]
	,NULL																AS [RouteOriginationDate]
	,NULL																AS [InternationalRouteIndicator]
	,''																AS [PreviousRoutingID]
	,NULL																AS [FinalStationIndex]
	,NULL																AS [PreviousStationIndex]
	,NULL																AS [NextStationIndex]
	,NULL																AS [OriginStationIndex]
	,''																AS [ShippingDocumentNumber]
	,''																AS [DocumentNumber]
	,NULL																AS [STD]
	,NULL																AS [ETD]
	,NULL																AS [STA]
	,NULL																AS [ETA]
	,NULL																AS [SFT]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND'
		THEN dbo.GetUTCTime(S.SiteIndex8,C.[START_TIME])	
		WHEN 'RETURN TO BULK'
		THEN dbo.GetUTCTime(S.SiteIndex8,C.[START_TIME])	
		WHEN 'REFUEL'
		THEN dbo.GetUTCTime(S.SiteIndex8,[START_TIME])
		WHEN 'DEFUEL'
		THEN dbo.GetUTCTime(S.SiteIndex8,[START_TIME])
		ELSE NULL
		END															AS [FST]							--fillstand, RTB, 
	,NULL																AS [EstimatedFuelingDuration]
	,0																	AS [TicketMode]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND' 
			THEN isnull(C.[VEHICLE_ID],'')
		WHEN 'RECIRC'
			THEN isnull(C.[VEHICLE_ID],'')
		WHEN 'REFUEL'
			THEN isnull(C.[AIRCRAFT_ID],'') 
		WHEN 'DEFUEL'
			THEN isnull(C.[VEHICLE_ID],'')
		ELSE ''
		END															AS [DestinationRegistrationID1]		--fillstand, recirculation
	,NULL																AS [DestinationSerialNumber1]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND' 
			THEN (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'RECIRC'
			THEN (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'REFUEL'
			THEN (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.AIRCRAFT_ID AND te.SiteIndex = S.SiteIndex8 ) 
		WHEN 'DEFUEL'
			THEN (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		END													AS [DestinationEquipmentType1]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND' 
			THEN (Select top 1 tet.Model from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'RECIRC'
			THEN (Select top 1 tet.Model from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'REFUEL'
			THEN (Select top 1 tet.Model from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.AIRCRAFT_ID AND te.SiteIndex = S.SiteIndex8 ) 
		WHEN 'DEFUEL'
			THEN (Select top 1 tet.Model from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		END													AS [DestinationEquipmentModel1]
	,NULL												AS [DestinationCompanyEquipmentID1]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND' 
			THEN (Select top 1 te.[Index] from tblEquipment te where te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'RECIRC'
			THEN (Select top 1 te.[Index] from tblEquipment te where te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'REFUEL'
			THEN (Select top 1 te.[Index] from tblEquipment te where te.ID = C.AIRCRAFT_ID AND te.SiteIndex = S.SiteIndex8 ) 
		WHEN 'DEFUEL'
			THEN (Select top 1 te.[Index] from tblEquipment te where te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		END												AS [DestinationEquipmentIndex1]
	,NULL												AS [DestinationRegistrationID2]
	,NULL												AS [DestinationSerialNumber2]
	,NULL												AS [DestinationEquipmentType2]
	,NULL												AS [DestinationEquipmentModel2]
	,NULL												AS [DestinationCompanyEquipmentID2]
	,NULL												AS [DestinationEquipmentIndex2]
	,NULL												AS [DestinationRegistrationID3]
	,NULL												AS [DestinationSerialNumber3]
	,NULL												AS [DestinationEquipmentType3]
	,NULL												AS [DestinationEquipmentModel3]
	,NULL												AS [DestinationCompanyEquipmentID3]
	,NULL												AS [DestinationEquipmentIndex3]
	,CASE UPPER(C.REQUEST_TYPE) 
		WHEN 'FILL STAND' 
			THEN isnull(C.[LOCATION],'')
		WHEN 'RECIRC'
			THEN isnull(C.[VEHICLE_ID],'')
		WHEN 'REFUEL'
			THEN isnull(C.[VEHICLE_ID],'') 
		WHEN 'DEFUEL'
			THEN isnull(C.[AIRCRAFT_ID],'')
		ELSE ''
		END												AS [SourceRegistrationID1]		--fillstand, recirculation
	,NULL												AS [SourceSerialNumber1]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND' 
			THEN (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.LOCATION AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'RECIRC'
			THEN (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'REFUEL'
			THEN (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 ) 
		WHEN 'DEFUEL'
			THEN (Select top 1 EqTypeName from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.AIRCRAFT_ID AND te.SiteIndex = S.SiteIndex8 )
		END													AS [SourceEquipmentType1]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND' 
			THEN (Select top 1 tet.Model from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.LOCATION AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'RECIRC'
			THEN (Select top 1 tet.Model from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'REFUEL'
			THEN (Select top 1 tet.Model from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 ) 
		WHEN 'DEFUEL'
			THEN (Select top 1 tet.Model from tblEquipment te,tblEquipmentTypes tet where te.EqTypeIndex = tet.EqTypeIndex and te.ID = C.AIRCRAFT_ID AND te.SiteIndex = S.SiteIndex8 )
		END													AS [SourceEquipmentModel1]
	,NULL												AS [SourceCompanyEquipmentID1]
	,CASE UPPER(REQUEST_TYPE) 
		WHEN 'FILL STAND' 
			THEN (Select top 1 te.[Index] from tblEquipment te where te.ID = C.LOCATION AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'RECIRC'
			THEN (Select top 1 te.[Index] from tblEquipment te where te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 )
		WHEN 'REFUEL'
			THEN (Select top 1 te.[Index] from tblEquipment te where te.ID = C.VEHICLE_ID AND te.SiteIndex = S.SiteIndex8 ) 
		WHEN 'DEFUEL'
			THEN (Select top 1 te.[Index] from tblEquipment te where te.ID = C.AIRCRAFT_ID AND te.SiteIndex = S.SiteIndex8 )
		END												AS [SourceEquipmentIndex1]
	,NULL												AS [SourceRegistrationID2]
	,NULL												AS [SourceSerialNumber2]
	,NULL												AS [SourceEquipmentType2]
	,NULL												AS [SourceEquipmentModel2]
	,NULL												AS [SourceCompanyEquipmentID2]
	,NULL												AS [SourceEquipmentIndex2]
	,NULL												AS [SourceRegistrationID3]
	,NULL												AS [SourceSerialNumber3]
	,NULL												AS [SourceEquipmentType3]
	,NULL												AS [SourceEquipmentModel3]
	,NULL												AS [SourceCompanyEquipmentID3]
	,NULL												AS [SourceEquipmentIndex3]
	,C.OPERATOR											AS [OperatorID]							--fillstand, RTB, 
	,O.PersonIndex 										AS [OperatorIndex]
	,NULL												AS [EffectiveDate]
	,NULL												AS [ExpirationDate]
	,NULL												AS [ScheduledDate]
	,NULL												AS [AutoComplete]
	,NULL												AS [Flag01]								
	,NULL												AS [Flag02]								
	,NULL												AS [Flag03]
	,C.FLAG_1											AS [Flag04]								--Special Fuel Flag
	,0													AS [Flag05]								
	,NULL												AS [Flag06]
	,NULL												AS [Number01]
	,0													AS [Number02]
	,convert(float,CASE ISNUMERIC(C.USERDATA_8) WHEN 0 THEN NULL ELSE C.USERDATA_8 END)	AS [Number03]
	,CASE 
		WHEN   
			UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN convert(float,CASE ISNUMERIC(C.SUBTYPE_4_DESC) WHEN 0 THEN NULL ELSE C.USERDATA_8 END)
			ELSE NULL 
		END												AS [Number04]
	,NULL												AS [Number05]
	,NULL												AS [Number06]
	,NULL												AS [ContactFirstName]
	,NULL												AS [ContactSurname]			
	,NULL												AS [Date01]
	,NULL												AS [Date02]
	,NULL												AS [Date03]
	,NULL												AS [Date04]
	,NULL												AS [LegacyNumber]
	,NULL												AS [Country]
	,NULL												AS [ContactInfo]
	,NULL												AS [AssociatedDocNumber]
	,NULL												AS [AssociatedCLIN]
	,0													AS [SubmittedToAccounting]
	,3													AS [OriginApplication]
	,(SELECT top 1 FuelCardIndex from tblFuelCards where ID = C.USERDATA_7) AS [FuelCardIndex]
	,C.USERDATA_7												AS [FuelCardID]
	,NULL												AS [AssociatedTransportOrderNumber]
	,dbo.GetUTCTime(S.SiteIndex8,REQUEST_TIME)			AS [RequestedDateTime]				
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'FILL STAND' 
			OR UPPER(REQUEST_TYPE) = 'RETURN TO BULK'
			OR UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN dbo.GetUTCTime(S.siteIndex8,[DISP_TIME])
			ELSE NULL 
		END												AS [DispatchedDateTime]				--fillstand, RTB, REFUEL, DEFUEL
	,0													AS [ErrorFlag]
	,Deleteflag											AS [DeleteFlag]
	,NULL												AS [UserData1]
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'RECIRC'
		THEN	C.[SUBTYPE_4_DESC]
		ELSE '' 	
		END												AS [UserData2]  --Recirc
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'REFUEL'
		THEN	C.USERDATA_2 
		WHEN   UPPER(REQUEST_TYPE) = 'DEFUEL'
		THEN	C.USERDATA_2	
		ELSE ''
		END												AS [UserData3]
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'RECIRC'
		OR UPPER(REQUEST_TYPE) = 'REFUEL'
		OR UPPER(REQUEST_TYPE) = 'DEFUEL'
		THEN	C.[USERDATA_3]
		ELSE ''	
		END												AS [UserData4]	--Recirc
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN C.[USERDATA_1] 
		WHEN 'DEFUEL'
		THEN C.[USERDATA_1]	
		ELSE ''
		END												AS [UserData5]  
	,NULL												AS [UserData6]  
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'FILL STAND' 
			OR UPPER(REQUEST_TYPE) = 'RETURN TO BULK'
			OR UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL' 
			THEN C.[LOCATION] 
		END												AS [UserData7]	--Fillstand/RTB
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'FILL STAND' 
			OR UPPER(REQUEST_TYPE) = 'RETURN TO BULK' 
			OR UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN C.[RADIO_NUM] 
			ELSE ''
		END												AS [UserData8]  --Fillstand/RTB
	,NULL												AS [UserData9]
	,CASE 
		WHEN    
			UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN convert(nvarchar(20),dbo.ConvertToSIUnits(C.[VARIANCE],46/*gallons*/)) 
			ELSE ''
		END												AS [UserData10]
	,CASE 
		WHEN    
			UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN (Select top 1 Address1 from ConsolidatedDB6.dbo.tblContacts where ContactIndex = C.CONTACT_INDEX and ContactIndex is not null)	
		ELSE ''
		END AS [UserData11]
	,CASE 
		WHEN    
			UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN (Select top 1 Address2 from ConsolidatedDB6.dbo.tblContacts where ContactIndex = C.CONTACT_INDEX and ContactIndex is not null)	
		ELSE ''
		END AS [UserData12]
	,CASE 
		WHEN UPPER(REQUEST_TYPE) = 'RECIRC'
		OR UPPER(REQUEST_TYPE) = 'REFUEL'
		OR UPPER(REQUEST_TYPE) = 'DEFUEL' 
			THEN C.[UserData_5] 
			ELSE ''	
		END												AS [UserData13] --RECIRC
	,CASE 
		WHEN UPPER(REQUEST_TYPE) = 'RECIRC' 
		OR UPPER(REQUEST_TYPE) = 'REFUEL'
		OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN C.[UserData_6]
		ELSE ''	 
		END
													AS [UserData14] --RECIRC
	,CASE 
		WHEN    
			UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN (Select top 1 City from ConsolidatedDB6.dbo.tblContacts where ContactIndex = C.CONTACT_INDEX and ContactIndex is not null)	
		ELSE ''
		END												AS [UserData15]
	,NULL												AS [UserData16] 
	,CASE 
		WHEN    
			UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN (Select top 1 State from ConsolidatedDB6.dbo.tblContacts where ContactIndex = C.CONTACT_INDEX and ContactIndex is not null)	
		ELSE ''
		END													AS [UserData17]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN isnull(C.AIRCRAFT_REF_CODE,'') 
		WHEN 'DEFUEL'
		THEN isnull(C.AIRCRAFT_REF_CODE,'')
		ELSE ''
		END												AS [UserData18] 
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'REFUEL'
		THEN	C.[SUBTYPE_1_DESC] 
		WHEN   UPPER(REQUEST_TYPE) = 'DEFUEL'
		THEN	C.[SUBTYPE_1_DESC]	
		ELSE ''
		END												AS [UserData19] 
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'REFUEL'
		THEN	C.[SUBTYPE_2_DESC] 
		WHEN   UPPER(REQUEST_TYPE) = 'DEFUEL'
		THEN	C.[SUBTYPE_2_DESC]
		ELSE ''	
		END										AS [UserData20] 
	,CASE 
		WHEN   UPPER(REQUEST_TYPE) = 'REFUEL'
		THEN	C.[SUBTYPE_3_DESC] 
		WHEN   UPPER(REQUEST_TYPE) = 'DEFUEL'
		THEN	C.[SUBTYPE_3_DESC]	
		ELSE ''
		END										AS [UserData21] 
	,CASE 
		WHEN    
			UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN (Select top 1 PostalCode from ConsolidatedDB6.dbo.tblContacts where ContactIndex = C.CONTACT_INDEX and ContactIndex is not null)	
		ELSE ''
		END													AS [UserData22]
	,CASE UPPER(REQUEST_TYPE)
		WHEN 'REFUEL'
		THEN C.USERDATA_7 
		WHEN 'DEFUEL'
		THEN C.USERDATA_7
		ELSE ''
		END													AS [UserData23]  
	,CASE 
		WHEN    
			UPPER(REQUEST_TYPE) = 'REFUEL'
			OR UPPER(REQUEST_TYPE) = 'DEFUEL'
			THEN (Select top 1 Fax from ConsolidatedDB6.dbo.tblContacts where ContactIndex = C.CONTACT_INDEX and ContactIndex is not null)	
		ELSE ''
		END													AS [UserData24]
	,CASE WHEN UPPER(REQUEST_TYPE)='FILL STAND' 
	THEN -dbo.ConvertToSIUnits(ABS(ISNULL(C.[QUANTITY],0)),46/*gallons*/)
	WHEN UPPER(REQUEST_TYPE)='REFUEL' 
	THEN -dbo.ConvertToSIUnits(ABS(ISNULL(C.[QUANTITY],0)),46/*gallons*/)	
	ELSE dbo.ConvertToSIUnits(ABS(ISNULL(C.[QUANTITY],0)),46/*gallons*/)
	END												AS [GrossQuantity]
	,CASE WHEN UPPER(REQUEST_TYPE)='FILL STAND' 
	THEN -dbo.ConvertToSIUnits(ABS(ISNULL(C.[QUANTITY],0)),46/*gallons*/)
	WHEN UPPER(REQUEST_TYPE)='REFUEL' 
	THEN -dbo.ConvertToSIUnits(ABS(ISNULL(C.[QUANTITY],0)),46/*gallons*/)	
	ELSE dbo.ConvertToSIUnits(ABS(ISNULL(C.[QUANTITY],0)),46/*gallons*/)
	END												AS [NetQuantity]
	,dbo.ConvertToSIUnits(C.[VARIANCE],46/*gallons*/)	AS [Variance]
	,P.[ProductID]										AS [ProductID]
	,P.[ProductCode]									AS [ProductCode]
	,P.[ProductIndex8]									AS [ProductIndex]
	,P.[ProductType]									AS [ProductType]
	,C.[START_TIME]										AS [MeterStart]
	,C.[STOP_TIME]										AS [MeterStop]
	,CASE 
		WHEN UPPER(REQUEST_TYPE) = 'RECIRC' 
			THEN C.[LOCATION] 
		END												AS [StorageLocationID]
	
INTO #TMP_TRANS_NEED_CREATING
  FROM [AviationDB6].[dbo].[CONTROL_LOG] C LEFT JOIN #TMP_PRODUCT_INDEX_MAP P ON C.PRODUCT_INDEX = P.ProductIndex6
 	JOIN #TMPSITES s ON MANAGER = S.SiteID8
 LEFT JOIN #TMP_OPERATORS O ON C.EMPLOYEE_ID  = O.OperatorID  AND S.SiteIndex8=O.SiteIndex
  --LEFT JOIN #TMP_EQUIPMENT SE ON SE.EquipmentID = C.VEHICLE_ID
  --LEFT JOIN #TMP_EQUIPMENT DE ON DE.EquipmentID = C.AIRCRAFT_ID
  WHERE --MANAGER = S.SiteID8
  --AND  
  (
	(UPPER(REQUEST_TYPE) IN ('FILL STAND', 'RETURN TO BULK', 'RECIRC')) OR 
	(UPPER(REQUEST_TYPE) IN ('REFUEL', 'DEFUEL') AND C.CANCELLED = 1)
  )
  AND TRANSACTION_DATE BETWEEN '1900-01-01' AND '2079-06-06'

DROP TABLE #TMP_COMPANIES
DROP TABLE #TMP_PRODUCT_INDEX_MAP
DROP TABLE #TMP_OPERATORS
DROP TABLE #TMP_EQUIPMENT

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
	  LEFT([TransID],64)
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
	  ,3 AS	[OriginApplication] --Added by Eric Simmons on 4/5/2010
      ,[FuelCardIndex]
      ,[FuelCardID]
      ,[AssociatedTransportOrderNumber]
      ,[RequestedDateTime]
      ,[DispatchedDateTime]
      ,[ErrorFlag]/**/
  FROM #TMP_TRANS_NEED_CREATING T 

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
      ,[UserData22]
      ,[UserData23]
      ,[UserData24]
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
      ,T.[UserData22]
      ,T.[UserData23]
      ,T.[UserData24]
      ,T.[CreatedBy]
      ,T.[CreatedDate]
      ,T.[UpdatedBy]
      ,T.[UpdatedDate]
      ,TT.[TransIndex]
  FROM  #TMP_TRANS_NEED_CREATING T JOIN [ConsolidatedDB].[dbo].tblTransactions TT ON T.TransID = TT.TransID

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
	[TransactionStatus] int,
	[GrossQuantity] float,
	[NetQuantity] float,
	[Variance] float,
	[ProductID] nvarchar(30),
	[ProductIndex] int,
	[ProductType] nvarchar(20),
	[ProductCode] nvarchar(15),
	[StorageLocationID] nvarchar(50), 
	CreatedBy nvarchar(100),
	CreatedDate datetime,
	UpdatedBy nvarchar(100),
	UpdatedDate datetime,
	OperatorID nvarchar(50),
	OperatorIndex int,
	DestinationRegistrationID nvarchar(30),
	DestinationSerialNumber nvarchar(10),
	DestinationEquipmentType nvarchar(50),
	DestinationEquipmentModel nvarchar(20),
	DestinationCompanyEquipmentID nvarchar(30),
	DestinationEquipmentIndex int,
	DestinationCompartmentID nvarchar(50),
	DestinationCompartmentIndex int,
	SourceRegistrationID nvarchar(30),
	SourceSerialNumber nvarchar(10),
	SourceEquipmentType nvarchar(50),
	SourceEquipmentModel nvarchar(20),
	SourceCompanyEquipmentID nvarchar(30),
	SourceEquipmentIndex int,
	SourceCompartmentID nvarchar(30),
	SourceCompartmentIndex int
	)
	
INSERT INTO #TMP_TRANSACTIONLINEITEMS (
	 TransIndex
	,[TransactionInventoryDate]
	,[TransactionStatus]
	,[GrossQuantity]	
	,[NetQuantity]	
	,[Variance]	
	,[ProductID]		
	,[ProductIndex] 
	,[ProductType] 
	,[ProductCode] 
	,[StorageLocationID]
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
	,OperatorID
	,OperatorIndex
	/* DestinationRegistrationID */ ,DestinationRegistrationID
	/* DestinationSerialNumber */ ,DestinationSerialNumber
	/* DestinationEquipmentType */ ,DestinationEquipmentType
	/* DestinationEquipmentModel */ ,DestinationEquipmentModel
	/* DestinationCompanyEquipmentID */ ,DestinationCompanyEquipmentID
	/* DestinationEquipmentIndex */ ,DestinationEquipmentIndex
	/* DestinationCompartmentID */,DestinationCompartmentID
	/* DestinationCompartmentIndex */,DestinationCompartmentIndex
	/* SourceRegistrationID */ ,SourceRegistrationID
	/* SourceSerialNumber */,SourceSerialNumber
	/* SourceEquipmentType */,SourceEquipmentType
	/* SourceEquipmentModel */,SourceEquipmentModel
	/* SourceCompanyEquipmentID */,SourceCompanyEquipmentID
	/* SourceEquipmentIndex */,SourceEquipmentIndex
	/* SourceCompartmentID */,SourceCompartmentID
	/* SourceCompartmentIndex */,SourceCompartmentIndex
)
SELECT
	 TT.TransIndex
	,T.[InventoryDate]
	,T.[TransactionStatus] 
	,T.[GrossQuantity] 
	,T.[NetQuantity] 
	,T.[Variance] 
	,T.[ProductID]
	,T.[ProductIndex] 
	,CASE T.[ProductType]
		WHEN 1 THEN 'Blend'
		WHEN 2 THEN 'Additive'
		WHEN 3 THEN 'Additize'
		ELSE 'Component'
		END 
	,T.[ProductCode] 
	,T.[StorageLocationID]
	,'Varec'
	,dbo.GetUTCTime(tt.siteIndex,GETDATE())
	,T.UpdatedBy
	,T.UpdatedDate
	,T.OperatorID
	,T.OperatorIndex
	/* DestinationRegistrationID */ ,TT.DestinationRegistrationID1
	/* DestinationSerialNumber */ ,TT.DestinationSerialNumber1
	/* DestinationEquipmentType */ ,TT.DestinationEquipmentType1
	/* DestinationEquipmentModel */ ,TT.DestinationEquipmentModel1
	/* DestinationCompanyEquipmentID */ ,TT.DestinationCompanyEquipmentID1
	/* DestinationEquipmentIndex */ ,TT.DestinationEquipmentIndex1
	/* DestinationCompartmentID */,''
	/* DestinationCompartmentIndex */,NULL
	/* SourceRegistrationID */ ,TT.SourceRegistrationID1
	/* SourceSerialNumber */,TT.SourceSerialNumber1
	/* SourceEquipmentType */,TT.SourceEquipmentType1
	/* SourceEquipmentModel */,TT.SourceEquipmentModel1
	/* SourceCompanyEquipmentID */,TT.SourceCompanyEquipmentID1
	/* SourceEquipmentIndex */,TT.SourceEquipmentIndex1
	/* SourceCompartmentID */,''
	/* SourceCompartmentIndex */,NULL
	
FROM #TMP_TRANS_NEED_CREATING T JOIN [ConsolidatedDB].[dbo].tblTransactions TT ON T.TransID = TT.TransID


INSERT INTO [ConsolidatedDB].[dbo].[tblTransactionLineItems] (
	 TransLineItemID
	,TransIndex
	,TransVersion 
	,[SequenceID]
	,[deleteflag]
	,[EngineeringUnitsIndex]
	,[LineItemSequenceNumber]
	,[TransactionInventoryDate]	
	,[TransactionStatus]
	,[GrossQuantity]	
	,[NetQuantity]	
	,[Variance]			
	,[Product]
	,[ProductIndex] 
	,[ProductType] 
	,[ProductCode] 
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
	,OperatorID
	,OperatorIndex
	/* DestinationRegistrationID */ ,DestinationRegistrationID
	/* DestinationSerialNumber */ ,DestinationSerialNumber
	/* DestinationEquipmentType */ ,DestinationEquipmentType
	/* DestinationEquipmentModel */ ,DestinationEquipmentModel
	/* DestinationCompanyEquipmentID */ ,DestinationCompanyEquipmentID
	/* DestinationEquipmentIndex */ ,DestinationEquipmentIndex
	/* DestinationCompartmentID */,DestinationCompartmentID
	/* DestinationCompartmentIndex */,DestinationCompartmentIndex
	/* SourceRegistrationID */ ,SourceRegistrationID
	/* SourceSerialNumber */,SourceSerialNumber
	/* SourceEquipmentType */,SourceEquipmentType
	/* SourceEquipmentModel */,SourceEquipmentModel
	/* SourceCompanyEquipmentID */,SourceCompanyEquipmentID
	/* SourceEquipmentIndex */,SourceEquipmentIndex
	/* SourceCompartmentID */,SourceCompartmentID
	/* SourceCompartmentIndex */,SourceCompartmentIndex
)
SELECT
	TransLineItemID+@NextTransLineItemID
	,TransIndex
	,@NextTransVersion 
	,0
	,0
	,0
	,0
	,[TransactionInventoryDate]	
	,[TransactionStatus] 
	,[GrossQuantity] 
	,[GrossQuantity] 
	,[Variance]
	,[ProductID]
	,[ProductIndex] 
	,[ProductType] 
	,[ProductCode] 
	,CreatedBy
	,CreatedDate
	,UpdatedBy
	,UpdatedDate
	,OperatorID
	,OperatorIndex
	/* DestinationRegistrationID */ ,DestinationRegistrationID
	/* DestinationSerialNumber */ ,DestinationSerialNumber
	/* DestinationEquipmentType */ ,DestinationEquipmentType
	/* DestinationEquipmentModel */ ,DestinationEquipmentModel
	/* DestinationCompanyEquipmentID */ ,DestinationCompanyEquipmentID
	/* DestinationEquipmentIndex */ ,DestinationEquipmentIndex
	/* DestinationCompartmentID */,DestinationCompartmentID
	/* DestinationCompartmentIndex */,DestinationCompartmentIndex
	/* SourceRegistrationID */ ,SourceRegistrationID
	/* SourceSerialNumber */,SourceSerialNumber
	/* SourceEquipmentType */,SourceEquipmentType
	/* SourceEquipmentModel */,SourceEquipmentModel
	/* SourceCompanyEquipmentID */,SourceCompanyEquipmentID
	/* SourceEquipmentIndex */,SourceEquipmentIndex
	/* SourceCompartmentID */,SourceCompartmentID
	/* SourceCompartmentIndex */,SourceCompartmentIndex
FROM #TMP_TRANSACTIONLINEITEMS T 
--select * from [ConsolidatedDB].[dbo].[tblTransactionLineItems]
UPDATE dbo.tblAccountingSequences SET SequenceValue =  (SELECT SequenceValue +COUNT(*) FROM #TMP_TRANSACTIONLINEITEMS) 
	WHERE SequenceName = 'LineItemID';

DROP TABLE #TMP_TRANSACTIONLINEITEMS

DROP TABLE #TMP_TRANS_NEED_CREATING
DROP TABLE #TMPSITES





	/*
IF @@TRANCOUNT > 0    
BEGIN     
--	 ROLLBACK TRANSACTION; 
   
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