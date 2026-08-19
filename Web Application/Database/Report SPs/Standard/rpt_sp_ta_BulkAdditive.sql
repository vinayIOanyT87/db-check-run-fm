USE [ConsolidatedDB]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_BulkAdditive') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_BulkAdditive
GO




CREATE PROCEDURE [dbo].[rpt_sp_ta_BulkAdditive]

/*****************************************************************
Author:				Urvi Patel
Date:				6/15/2009
Version:			7.5.1.5
Execution:
			EXEC rpt_sp_ta_BulkAdditive '3/1/2010','3/31/2010',1,1,2,'1840 - CITGO Petroleum Corp','<All>'

Description

	Date			By		Description
	6/25/2009		UP		New
	10/1/2009		KF		Bug when there are more than one station.
							Add temp table to be updated by Bulk Meter Volume, Gross and Rate
							Also joined meterid on Bulk Station.
	10/28/2009		KF		Add order by Bulkstation,InventoryDate
	10/29/2009		KF		Add with(nolock) to tblTransactions
	12/08/2009		KF		changed function AliasList to rpt_fn_ta_AliasList
	2/9/2010		KF		Version changed due to change in report.
	3/8/2010		KF		AuthorizedCompanies replaced by rpt_fn_ta_AuthorizedCompanies
	3/16/2010		KF		Add InventoryDate to where clause for #Master table
	5/11/2010		KF		Version changed due to change in report
	12/14/2011		ADS		Added ISNULL on returned Gross_Total & BulkMeterVol columns to replace NULL by zero
*************************************************************************/

( 
	 @BeginDate datetime
	,@EndDate datetime
	,@LoginSiteIndex int
	,@SiteIndex int
	,@UserIndex int
	,@Manager nvarchar(30)
	,@BulkAdditive nvarchar(50)
)

AS

IF  @Manager = '<ALL>' SET @Manager = NULL
IF  @BulkAdditive = '<ALL>' SET @BulkAdditive = NULL

DECLARE @VolumeUnits int
SET @VolumeUnits = (SELECT VolumeUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @AdditiveVolumeUnits int
SET @AdditiveVolumeUnits = (SELECT AdditiveVolumeUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @VolumeDecimalPlaces int
SET @VolumeDecimalPlaces = (SELECT VolumeDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @AdditiveVolumeDecimalPlaces int
SET @AdditiveVolumeDecimalPlaces = (SELECT AdditiveVolumeDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @EndInventoryDate datetime
SET	@EndInventoryDate = @EndDate  -- preserve the EndDate as InventoryDate first

SET @EndDate = DateAdd(second,-1,DateAdd(day,1,@EndDate))

-- Get the Authorized Companies
DECLARE @AuthorizedCompanies TABLE (CompanyID nvarchar(30))
INSERT INTO @AuthorizedCompanies SELECT ID FROM rpt_fn_ta_AuthorizedCompanies(@LoginSiteIndex, @SiteIndex, @UserIndex)

SELECT	ID AS Site,
		SiteIndex
INTO	#Site
FROM	tblSites with(nolock) ,tblSiteToSiteMap with(nolock)  
WHERE	ParentSiteIndex = @SiteIndex 
		AND ChildSiteIndex = tblSites.SiteIndex 
		AND SiteGroupFlag = 0

-- Get the Transaction Aliases --shall we filter here though???
DECLARE @AliasList TABLE
(
	AliasName nvarchar (30),
	TransTypeID int
)

INSERT INTO @AliasList 
SELECT * 
FROM rpt_fn_ta_AliasList(@LoginSiteIndex,@SiteIndex) 
ORDER BY AliasName 

--SELECT * from @AliasList

-- Cursor to convert csv values into normalized form from Station table(ArmsServiced)

CREATE TABLE #tmp_split_stations ([SpiltIndex] int,Arms_Split_ID varchar(100))

DECLARE @delimiter char

SET @delimiter = ','

DECLARE @Spilt_Index int
DECLARE @ArmsService_Split_ID varchar(20)
DECLARE @ArmsServiced nvarchar(200)

DECLARE StationCursor CURSOR FOR
SELECT [Index],ArmsServiced FROM dbo.tblStations WHERE Type = 7 AND [Index] = ISNULL(@BulkAdditive,[Index])

OPEN StationCursor

FETCH NEXT FROM StationCursor INTO @Spilt_Index,@ArmsServiced
WHILE @@fetch_status<>-1
BEGIN

	INSERT INTO #tmp_split_stations([SpiltIndex],Arms_Split_ID)
	SELECT @Spilt_Index,tmp.array_value
	FROM dbo.rpt_fn_ta_BulkAdditive_ParseArray(@ArmsServiced,@delimiter) as tmp

	FETCH NEXT FROM StationCursor INTO @Spilt_Index,@ArmsServiced 
END
CLOSE StationCursor
DEALLOCATE StationCursor


SELECT [Index] AS StationIndex, ID AS 'Station', Type, AssociatedTankIndex,ArmsServiced, [Index] AS SpiltIndex,Arms_Split_ID
INTO #SplitStation
FROM dbo.tblStations s with(nolock)
	JOIN #tmp_split_stations tmpstation On
			 s.[Index] = tmpstation.[SpiltIndex]

--SELECT * FROM #SplitStation

-- MeterID from SublineItemMeterID

SELECT	l.TransLineItemID AS SubLineLineItemID,
		t.SiteIndex SubLineSiteIndex, 
		t.InventoryDate AS SubLineInventoryDate,
		t.TransID AS SubLineTransID,
		l.MeterID AS SublineMeterID,
		IsNull(-1*dbo.ConvertFromSIUnits(l.GrossQuantity,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS SublineGross,
		IsNull(-1*dbo.ConvertFromSIUnits(l.NetQuantity,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS SublineNet,
		IsNull(dbo.ConvertFromSIUnits(l.PresetAmount,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Preset--,
		,s.Station AS SublineStation
INTO #BOLTransactionsSubline
FROM (tblTransactionSubLineItems l with(nolock) 
			LEFT OUTER JOIN tblTransactionLineItems I with(nolock) ON 
							l.TransLineItemID = I.TransLineItemID AND 
							l.TransID = I.TransID)
			LEFT OUTER JOIN tblTransactions t with(nolock) ON 
							l.TransID = t.TransID
		    JOIN #SplitStation s ON 
							s.Arms_Split_ID = l.meterID
			--s.associatedtankindex = l.storagelocationindex and 
WHERE	
			S.Type = 7 AND s.StationIndex = isnull(@BulkAdditive,s.StationIndex) 
			--l.TransactionInventoryDate BETWEEN @BeginDate AND @EndDate AND
		AND l.TransID = t.TransID 
		AND	l.DeleteFlag = cast(0 as bit) 
		AND	t.AliasName IN (SELECT AliasName FROM @AliasLIst)
		AND t.SiteIndex IN (SELECT SiteIndex FROM #Site)
		AND ((dbo.GetLocalTime(@SiteIndex,t.TransDateTime) > =@BeginDate
			 AND dbo.GetLocalTime(@SiteIndex,t.TransDateTime) <=@EndDate
			 AND InventoryDate>=@BeginDate) -- cross midnight trans can have inventory date earlier than transaction datetime
			 OR InventoryDate Between @BeginDate AND @EndInventoryDate
			)
		AND ManagerID = isnull(@Manager,ManagerID)
		AND TransTypeID IN (3,4,5,6) -- trans use meters
	    AND (ReversalType IS NULL OR ReversalType ='O') -- add code to take out Reversed Transactions
		AND EXISTS (SELECT CompanyID 
		            FROM @AuthorizedCompanies 
			        WHERE CompanyID IN (t.CarrierID, t.ShipperID, t.ShipToID, t.SupplierID, t.ManagerID, t.OwnerID, t.BillToID)) 
		AND t.DeleteFlag = cast(0 as bit)

--Select * from #BOLTransactionsSubline
-- Check MeterID matching LineItem with Subline

SELECT	l.TransLineItemID AS ID,
		t.SiteIndex, 
		t.InventoryDate,
		t.TransID,
		l.MeterID,
		IsNull(-1*dbo.ConvertFromSIUnits(l.GrossQuantity,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Gross,
		IsNull(-1*dbo.ConvertFromSIUnits(l.NetQuantity,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Net,
		IsNull(dbo.ConvertFromSIUnits(l.PresetAmount,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Preset--,
		,s.Station 
INTO #BOLTransactionsLineItem
FROM tblTransactionLineItems l with(nolock) 
		LEFT OUTER JOIN tblTransactions t with(nolock) ON 
					l.TransID = t.TransID
		JOIN #SplitStation s ON 
					s.Arms_Split_ID = l.meterID
		LEFT JOIN #BOLTransactionsSubline tmpsubline ON 
					l.TransID = tmpsubline.SubLineTransID AND 
					l.TransLineItemID = tmpsubline.SubLineLineItemID AND 
					l.MeterID <> tmpsubline.SublineMeterID
		--s.associatedtankindex = l.storagelocationindex and 
WHERE		S.Type = 7 
		AND s.StationIndex = isnull(@BulkAdditive,s.StationIndex)
			--l.TransactionInventoryDate BETWEEN @BeginDate AND @EndDate AND
		AND	l.TransID = t.TransID 
		AND	l.DeleteFlag = cast(0 as bit) 
		AND	t.AliasName IN (SELECT AliasName FROM @AliasLIst)
		AND t.SiteIndex IN (SELECT SiteIndex FROM #Site)
		AND ((dbo.GetLocalTime(@SiteIndex,t.TransDateTime) > =@BeginDate
			 AND dbo.GetLocalTime(@SiteIndex,t.TransDateTime) <=@EndDate
			 AND InventoryDate>=@BeginDate) -- cross midnight trans can have inventory date earlier than transaction datetime
			 OR InventoryDate Between @BeginDate AND @EndInventoryDate
			)
		AND ManagerID = isnull(@Manager,ManagerID)
		AND TransTypeID IN (3,4,5,6) -- trans use meters
	    AND (ReversalType IS NULL OR ReversalType ='O') -- add code to take out Reversed Transactions
		AND EXISTS (SELECT CompanyID 
		            FROM @AuthorizedCompanies 
			        WHERE CompanyID IN (t.CarrierID, t.ShipperID, t.ShipToID, t.SupplierID, t.ManagerID, t.OwnerID, t.BillToID)) 
		AND t.DeleteFlag = cast(0 as bit)
		
SELECT	SubLineLineItemID AS LineItemID,
		SubLineSiteIndex AS SiteIndex, 
		SubLineInventoryDate AS InventoryDate,
		SubLineTransID AS TransID,
		SublineMeterID AS MeterID,
		SublineGross AS Gross,
		SublineNet AS Net,
		Preset AS Preset,
		SublineStation AS Station
INTO #AllTrans
FROM #BOLTransactionsSubline

UNION ALL

SELECT	ID,
		SiteIndex, 
		InventoryDate,
		TransID,
		MeterID,
		Gross,
		Net,
		Preset,
		Station
FROM #BOLTransactionsLineItem

--SELECT * FROM #AllTrans order by inventorydate, meterid

--select * from #BOLTransactionsSubline

--Select * from #Transactions
SELECT	SiteIndex, 
		InventoryDate,
		MeterID,
		sum(Gross) AS Gross_Total,
		sum(Net) AS Net_Total,
		Station
INTO #Sum
FROM #AllTrans
GROUP BY SiteIndex,InventoryDate,Station, MeterID

--select * from #Sum


-- MeterCLoseout Transactions

SELECT	SiteIndex, 
		InventoryDate,
		AliasName,
		TransTypeID,
		a.TransID,
		a.DocumentNumber,
		ManagerID
INTO	#TransactionTable
FROM	tblTransactions a with(nolock) 
WHERE	AliasName IN (SELECT AliasName FROM @AliasLIst)
		AND SiteIndex IN (SELECT SiteIndex FROM #Site)
		AND ((dbo.GetLocalTime(@SiteIndex,TransDateTime) > =@BeginDate and
			  dbo.GetLocalTime(@SiteIndex,TransDateTime) <=@EndDate and
			  InventoryDate>=@BeginDate) -- cross midnight trans can have inventory date earlier than transaction datetime
			or InventoryDate Between @BeginDate AND @EndInventoryDate)
		AND ManagerID = isnull(@Manager,ManagerID)
		--AND TransTypeID IN (3,4,5,6) -- trans use meters												
		AND TransTypeID IN (12)																			
	    AND (ReversalType IS NULL OR ReversalType ='O') -- add code to take out Reversed Transactions
		AND EXISTS (SELECT CompanyID 
		            FROM @AuthorizedCompanies 
			        WHERE CompanyID IN (a.CarrierID, a.ShipperID, a.ShipToID, a.SupplierID, a.ManagerID, a.OwnerID, a.BillToID)) 
		AND a.DeleteFlag = cast(0 as bit)

--SELECT * from #transactiontable


SELECT	t.SiteIndex, 
		InventoryDate,
		AliasName,
		TransTypeID,
		t.DocumentNumber,
		l.Product,
		l.MeterID,
		l.MeterStart,
		l.MeterStop,
		IsNull(dbo.ConvertFromSIUnits(l.PresetAmount,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Preset,
		l.storagelocationid ,
		l.ProductType,
		s.ArmsServiced,
		s.[ID] AS BulkStation,
		ManagerID
		
INTO #Transactions

FROM tblTransactionLineItems l with(nolock)
					Left Outer Join #TransactionTable t on
										l.TransID = t.TransID
							   Join tbltanks tank with(nolock) on
										l.storagelocationindex = tank.tankindex and
										l.productindex = tank.productindex
							   Join dbo.tblStations s with(nolock) on
										l.storagelocationindex = s.Associatedtankindex and
										s.id = l.meterid

WHERE	 S.Type = 7  AND s.[Index] = isnull(@BulkAdditive,s.[Index]) AND  ManagerID = isnull(@Manager,ManagerID) AND
		l.TransID = t.TransID AND
		l.DeleteFlag = cast(0 as bit)
										

--SELECT * FROM #Transactions
--  Get PreviousDay Transactions


SELECT	SiteIndex, 
		InventoryDate,dbo.GetLocalTime(@SiteIndex,TransDateTime)AS TransDateTime,
		AliasName,
		TransTypeID,
		a.TransID,
		a.DocumentNumber,
		ManagerID
INTO	#PreviousDayTransactionTable
FROM	tblTransactions a with(nolock) 
WHERE	AliasName IN (SELECT AliasName FROM @AliasLIst)
		AND SiteIndex IN (SELECT SiteIndex FROM #Site)
		AND ((dbo.GetLocalTime(@SiteIndex,TransDateTime) > = (@BeginDate - 1) and
			  dbo.GetLocalTime(@SiteIndex,TransDateTime) <= @EndDate and InventoryDate>= (@BeginDate - 1)
             ) -- cross midnight trans can have inventory date earlier than transaction datetime
				or InventoryDate Between (@BeginDate - 1) AND @EndInventoryDate
			)
		AND ManagerID = isnull(@Manager,ManagerID)
		--AND TransTypeID IN (3,4,5,6) -- trans use meters												
		AND TransTypeID IN (12)																			
	    AND (ReversalType IS NULL OR ReversalType ='O') -- add code to take out Reversed Transactions
		AND EXISTS (SELECT CompanyID 
		            FROM @AuthorizedCompanies 
			        WHERE CompanyID IN (a.CarrierID, a.ShipperID, a.ShipToID, a.SupplierID, a.ManagerID, a.OwnerID, a.BillToID)) 
		AND a.DeleteFlag = cast(0 as bit)

--SELECT * from #transactiontable


SELECT	t.SiteIndex, 
		InventoryDate,
		AliasName,
		TransTypeID,
		t.DocumentNumber,
		l.Product,
		l.MeterID,
		l.MeterStart,
		l.MeterStop,
		IsNull(dbo.ConvertFromSIUnits(l.PresetAmount,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Preset,
		l.storagelocationid ,
		l.ProductType,
		s.ArmsServiced,
		s.[ID] AS BulkStation,
		ManagerID
		
INTO #PreviousDayTransactions
FROM tblTransactionLineItems l with(nolock)
				Left Outer Join #PreviousDayTransactionTable t On 
										l.TransID = t.TransID
						   Join tbltanks tank with(nolock) on
										l.storagelocationindex = tank.tankindex and
										l.productindex = tank.productindex
						  Join dbo.tblStations s with(nolock) on
										l.storagelocationindex = s.Associatedtankindex and
										s.id = l.meterid

WHERE	 S.Type = 7  AND s.[Index] = isnull(@BulkAdditive,s.[Index]) AND  ManagerID = isnull(@Manager,ManagerID) AND
		l.TransID = t.TransID AND
		l.DeleteFlag = cast(0 as bit)

SELECT Identity (int,1,1) AS RecNo,
		T.*,
		0 AS PreviousMeterStop,
		0 AS MeterSkip,
		convert(float, 0) AS NewMeterStart
INTO	#TransTemp 
FROM #Transactions T
--
--SELECT * FROM #Transactions  ---BREAKS
--
SELECT Identity (int,1,1) AS RecNo,
		P.*,
		0 AS PreviousMeterStop,
		0 AS MeterSkip--,
		--convert(float, 0) AS NewMeterStart
INTO	#PreviousDayTransTemp 
FROM #PreviousDayTransactions P

UPDATE  #TransTemp
SET MeterStart = Case when p.MeterStop is null then 0
					ELSE p.MeterStop END
FROM #TransTemp T
LEFT JOIN #PreviousDayTransTemp P
ON t.InventoryDate = p.InventoryDate + 1
	AND T.MeterID = P.MeterID 
	--AND T.RecNo = P.RecNO+1

--SELECT * FROM #TransTemp --mulitply

CREATE TABLE #Master(

		SiteIndex			int, 
		InventoryDate		Datetime,
		MeterID				nvarchar(100),
		MeterStart			float,
		MeterStop			float,
		Preset				float,
		storagelocationid	nvarchar(100),
		Gross_Total			float,
		ArmsServiced		nvarchar(200),
		BulkStation			nvarchar(100),
		ManagerID			nvarchar(60),
		BulkMeterVol		nvarchar(200),
		Rate				nvarchar(200)
)

INSERT INTO #Master

Select
		 t.SiteIndex
		,t.InventoryDate
		,t.MeterID	
		,t.MeterStart
		,t.MeterStop
		,t.Preset
		,t.storagelocationid
		,0 as Gross_Total	
		,t.ArmsServiced
		,t.BulkStation
		,t.ManagerID
		,0 as BulkMeterVol	
		,0 as Rate
FROM #TransTemp t


Update #Master
Set Gross_Total = (Select sum(s.Gross_Total)
					 from #Sum s 
					where s.Station = m.BulkStation and
						  s.InventoryDate = m.InventoryDate)
	From #Master m


Update #Master
Set BulkMeterVol = m.MeterStop - m.MeterStart
	from #Master m 


Update #Master
Set Rate = Case when m.Gross_Total is null or m.Gross_Total = 0 Then 0
				ELSE ((m.MeterStop - m.MeterStart)/ Gross_Total)*1000 END
	from #Master m 


/********
	MAIN QUERY
*********/

--Select * from #Master order by BulkStation, InventoryDate
SELECT	SiteIndex
	,	InventoryDate
	,	MeterID
	,	MeterStart
	,	MeterStop
	,	Preset
	,	storagelocationid
	,	ISNULL(Gross_Total,0) AS Gross_Total
	,	ArmsServiced
	,	BulkStation
	,	ManagerID
	,	ISNULL(BulkMeterVol,0) AS BulkMeterVol
	,	Rate

from #Master order by BulkStation, InventoryDate



DELETE #Site
DELETE #PreviousDayTransactions
DELETE #Transactions
DELETE #PreviousDayTransTemp
DELETE #TransTemp
DELETE #tmp_split_stations
DELETE #SplitStation
DELETE #BOLTransactionsSubline
DELETE #BOLTransactionsLineItem
DELETE #AllTrans
DELETE #Sum
DELETE #PreviousDayTransactionTable
DELETE #TransactionTable
DELETE #Master

--END








GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_BulkAdditive TO [public]
GO
