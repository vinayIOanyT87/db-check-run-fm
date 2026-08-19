USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[rpt_sp_ta_InventoryReconciliationSummarySub]    Script Date: 08/31/2012 11:55:10 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rpt_sp_ta_InventoryReconciliationSummarySub]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[rpt_sp_ta_InventoryReconciliationSummarySub]
GO

USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[rpt_sp_ta_InventoryReconciliationSummarySub]    Script Date: 08/31/2012 11:55:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[rpt_sp_ta_InventoryReconciliationSummarySub]
/*-- =============================================================================================
 Author:		Unknown
 Create date:	9/4/2007
 Version:		7.5.2.1
 Description:	
 Execution:
	Execute rpt_sp_ta_InventoryReconciliationSummarySub ''
												,''
												,'March 2010'
												,'3427 - CITGO Petroleum Corp'
												,'3003'
												,'1'
												,'1'
												,'2'
												,'Gross'
												,2
Modification History:
	Date	BY	Description
09-21-2007,		add SiteIndex filter at line 57 when retrieving views
10-05-2007,		change Variance formula to Physical - Book
11-05-2007,		change volumeUnit based on the product type
11-12-2007,		add daily variance percentage; add type 12 in transType filter 
09-08-2008,		add BookInventorySummaryGross and net in Inventory Summary Report
03-03-2009,		change BEGIN date to handle BEGIN data IS a closeout date
04-16-2009,	KF  Add WITH(NOLOCK) to tables.
12-8-2009,	KF	changed function AliasList to rpt_fn_ta_AliasList
02-23-2010	KF	Add Product to query.
4/15/2010	KF  New AliasName Supply Order. Need to exclude FROM report.
1/24/2011	KF	use DISTINCT Product FROM #TransactionTotals for #Summary to populate
				product name when there are no transactions.
8/31/2012	WK	Formatted and commented code.
11/15/2012	TH	Corrected determination of past closeouts to specify product
11/27/2012	TH	Branched rpt_sp_ta_InventoryReconciliationSummarySub from
				rpt_sp_ta_InventoryReconciliationSub, because change for Summary
				report caused performance issue on rpt_sp_ta_InventoryReconciliationSub
11/29/2012	TH	Copied changes to InventoryReconciliationSummary from v7.5.1 to v7.5.2
================================================================================================*/
( 
	@BeginDate DATETIME,
	@EndDate DATETIME,
	@Month	NVARCHAR(20),
	@Manager NVARCHAR(30),
	@Product NVARCHAR(30),
	@LoginSiteIndex INT,
	@SiteIndex INT,
	@UserIndex INT,
	@GrossNet NVARCHAR(10), -- the value is either Gross or Net
	@Header INT
)
AS

-- Build the View Header first with DataDictionary values and column names
DECLARE @StandardViewType TABLE 
( TypeIndex INT,
  ColumnName NVARCHAR(50),
  DictionaryKey NVARCHAR(100) COLLATE SQL_Latin1_General_CP1_CS_AS
)
INSERT INTO @StandardViewType 
VALUES(1,'BEGIN_INVENTORY' + @GrossNet,'Begin Inventory')
INSERT INTO @StandardViewType 
VALUES(2,'BOOK_INVENTORY' + @GrossNet,'Book Inventory')
INSERT INTO @StandardViewType 
VALUES(3,'INVENTORY_DATE' + @GrossNet,'Inventory Date')
INSERT INTO @StandardViewType 
VALUES(9,'VARIANCE' + @GrossNet,'Variance')	
INSERT INTO @StandardViewType 
VALUES(13,'TOTAL_VARIANCE' + @GrossNet, 'Total Variance')
INSERT INTO @StandardViewType 
VALUES(43,'TOTAL_PHYSICAL_INVENTORY' + @GrossNet,'Total Physical Inventory')
INSERT INTO @StandardViewType 
VALUES(61,'TOTAL_ACTIVITY' + @GrossNet,'Total Activity')		

-- DEBUGGING **
--SELECT *
--FROM @StandardViewType
--RETURN;
-- DEBUGGING **
	
	-- 	Build the ViewDefinition table with the column names for the view															
	SELECT	L.ColumnOrder
			, S1.ColumnName
			, S1.DictionaryKey
			, ISNULL(S1.DictionaryValue, S1.DictionaryKey) AS DictionaryValue
	INTO	#ViewDefinition
	FROM	dbo.tblListViewFields L WITH(NOLOCK) 
	RIGHT JOIN (SELECT S.*
						, D.[Value] AS DictionaryValue
				FROM @StandardViewType S 
				LEFT JOIN dbo.tblDataDictionaries D WITH(NOLOCK) ON S.DictionaryKey = D.[Key]
				) AS S1 ON S1.TypeIndex = L.TypeIndex
	WHERE	L.ListViewIndex = (	SELECT [Index] 
								FROM dbo.tblListViews WITH(NOLOCK) 
								WHERE [Type] = 2		--Standard View 
								AND TypeIndex = 4 
								AND SiteIndex=@SiteIndex -- Inventory Reconcilation View
								) 
	AND L.[Type] = 4

UNION

	SELECT  L.ColumnOrder
			, T.AliasName AS ColumnName -- 1 is TransAlias
			, '' AS DictionaryKey
			, T.AliasName AS DictionaryValue 
	FROM dbo.tblListViewFields L WITH(NOLOCK) 
	RIGHT JOIN dbo.tblTransactionAliases T WITH(NOLOCK) ON L.TypeIndex = T.AliasID
	WHERE L.ListViewIndex = (SELECT [Index] 
							FROM dbo.tblListViews WITH(NOLOCK) 
							WHERE [Type] = 2			--Standard View 
							AND TypeIndex = 4 
							AND SiteIndex = @SiteIndex	-- Inventory Reconcilation View
							) 
	AND L.[Type] = 1
	ORDER BY ColumnOrder

-- DEBUGGING **
--SELECT *
--FROM #ViewDefinition
--RETURN;
-- DEBUGGING **

IF @Header = 1	-- Gets all columns EXCEPT for last 3 columns on Inventory Reconciliation Report
				-- last 3 columns: Daily Tolerance, Total Movement, and Tolerance
	BEGIN
		DECLARE @TP NVARCHAR(1000)
		SELECT @TP = ''
		SELECT @TP = @TP + '['+ Convert(NVARCHAR(10),ColumnOrder)+'],'
		FROM	#ViewDefinition
		GROUP BY ColumnOrder

		SELECT @TP = LEFT(@TP, LEN(@TP) - 1)

		SELECT ColumnOrder
				, DictionaryValue
		INTO #TempV
		FROM #ViewDefinition

		SELECT @TP = 'SELECT * FROM #TempV PIVOT (MAX(DictionaryValue) FOR ColumnOrder IN ('+@TP+'))AS InfoPivot'
		
		EXEC(@TP)	
		DELETE #TempV
		DELETE #ViewDefinition
		
	END
ELSE --@Header = 2 -- Transaction details
	BEGIN
		-- Get beginning and end of month for the month and year
		IF @Month IS NOT NULL AND @Month <> ''
			BEGIN
				SET @BeginDate = Convert(DATETIME,@Month)
				SET @EndDate = DateAdd(s,-1,DateAdd(mm,1,@BeginDate)) --last day of the month
				--PRINT 'Begin Date: ' + CAST(@BeginDate AS NVARCHAR(20))
				--PRINT 'End Date: ' + CAST(@EndDate AS NVARCHAR(20))
				--RETURN;
			END	
		
		-- Get Product Type for this product: 0,1,2,3
		DECLARE @ProductType INT
		SET @ProductType = (SELECT Top(1)ProductType 
							FROM dbo.tblProducts WITH(NOLOCK) 
							WHERE ProductID = @Product 
							AND SiteIndex =@LoginSiteIndex 
							)
		--PRINT 'Product Type: ' + CAST(@ProductType AS NVARCHAR(20))
		--RETURN;
		
		-- Get Volume Units for this product
		DECLARE @VolumeUnits INT
		SET @VolumeUnits = 
			CASE
				-- Additive products
				WHEN  @ProductType = 2 THEN (SELECT tblSites.AdditiveVolumeUnitIndex 
											FROM tblSites WITH(NOLOCK) 
											WHERE tblSites.SiteIndex = @LoginSiteIndex
											)
			ELSE
				-- Component products
				(SELECT tblSites.VolumeUnitIndex 
				FROM tblSites WITH(NOLOCK) 
				WHERE tblSites.SiteIndex = @LoginSiteIndex
				)
			END

		--PRINT 'Volume Units: ' + CAST(@VolumeUnits AS NVARCHAR(20))
		--RETURN;
		
		-- Get Volume Decimal places for this product
		DECLARE @VolumeDecimalPlaces INT
		SET @VolumeDecimalPlaces = 
			CASE
				-- Additive Products
				WHEN @ProductType =2 THEN (SELECT AdditiveVolumeDecimalPlaces 
											FROM tblSites WITH(NOLOCK) 
											WHERE tblSites.SiteIndex = @LoginSiteIndex
											)
			ELSE
				-- Component Products
				(SELECT tblSites.VolumeDecimalPlaces 
				FROM tblSites WITH(NOLOCK) 
				WHERE tblSites.SiteIndex = @LoginSiteIndex
				)
			END
		
		--PRINT 'Volume Decimal Places: ' + CAST(@VolumeDecimalPlaces AS NVARCHAR(20))
		--RETURN;
	
		-- Create a 0 variable with the correct amount of zeroes
		DECLARE @Zero FLOAT
		SET @Zero = ROUND(0.0,@VolumeDecimalPlaces)

		--PRINT 'Zero Variable with Places: ' + CAST(@Zero AS NVARCHAR(20))
		--RETURN;

		--Build Begining Inventory table
		DECLARE @SummarySheet TABLE
		(
			SiteIndex INT,
			INVENTORY_DATE DATETIME DEFAULT '1901-01-01',
			BEGIN_INVENTORYGross FLOAT DEFAULT 0.0,
			BEGIN_INVENTORYNet FLOAT DEFAULT 0.0,
			TOTAL_ACTIVITYGross FLOAT DEFAULT 0.0,
			TOTAL_ACTIVITYNet FLOAT DEFAULT 0.0,
			BOOK_INVENTORYGross FLOAT DEFAULT 0.0,
			BOOK_INVENTORYNet FLOAT DEFAULT 0.0,
			PHYSICAL_INVENTORYGross FLOAT DEFAULT 0.0,
			PHYSICAL_INVENTORYNet FLOAT DEFAULT 0.0,
			Link INT DEFAULT 1  -- this field is used to populate per day per owner later
		)

-- DEBUGGING ***
--SELECT *
--FROM @SummarySheet
--RETURN;
-- DEBUGGING ***

		-- Get the site and any children if it has any
		SELECT	ID AS [Site]
				, SiteIndex
		INTO	#Site
		FROM	tblSites WITH(NOLOCK) ,tblSiteToSiteMap WITH(NOLOCK) 
		WHERE	ParentSiteIndex = @SiteIndex 
		AND ChildSiteIndex = tblSites.SiteIndex --???
		AND SiteGroupFlag = 0

-- DEBUGGING ***
--SELECT *
--FROM #Site
--DROP TABLE #Site
--RETURN;
-- DEBUGGING ***

		--Get last closeout date that is less than the Begin Date parameter if it isn't NULL or ''
		DECLARE @BEGIN DATETIME 
		SELECT @BEGIN = (SELECT MAX(CloseoutDate)
						FROM dbo.tblCloseoutInventory WITH(NOLOCK) 
						WHERE [Site] = (SELECT ID 
										FROM tblSites WITH(NOLOCK) 
										WHERE SiteIndex = @SiteIndex
										)
						AND ManagerName = @Manager
						AND ProductName = @Product
						AND CloseoutDate < @BeginDate)

--PRINT 'Begin Variable: '  + CAST(ISNULL(@BEGIN,'1/1/2000') AS NVARCHAR(25))						
--RETURN;
		-- Assign a value to @Begin variable
		SELECT @BEGIN = 
			CASE 
				WHEN @BEGIN IS NULL THEN '1901-01-01'
				WHEN @BEGIN <= @BeginDate THEN @BEGIN
			END

--PRINT 'Begin Variable: ' + CAST(@BEGIN AS NVARCHAR(25))						
--RETURN;

		-- Get all Transaction Aliases configured in FuelsManager
		DECLARE @AliasList TABLE
		(
			AliasName NVARCHAR (30),
			TransTypeID INT
		)

		INSERT INTO @AliasList 
			SELECT AliasName
					, TransTypeID 
			FROM rpt_fn_ta_AliasList(@LoginSiteIndex,@SiteIndex) 
			--WHERE TransTypeID NOT IN(9,10,11,14,17) --14 IS Phyiscal Inventory
			ORDER BY AliasName 

-- DEBUGGING ***
--SELECT *
--FROM @AliasList
--RETURN;
-- DEBUGGING ***

		-- Get Transactions FROM @BEGIN to @EndDate, NOT FROM @BeginDate
		SELECT	SiteIndex
				, InventoryDate
				, AliasName
				, TransTypeID
				, TransID
		INTO	#TransactionTable
		FROM	tblTransactions a WITH(NOLOCK) 
		WHERE	AliasName IN (SELECT AliasName 
								FROM @AliasLIst
								)-- WHERE TransTypeID NOT IN(9,10,11,14,17))
				AND SiteIndex IN (SELECT SiteIndex 
									FROM #Site
									)
				AND InventoryDate > =@BEGIN
				AND InventoryDate <=@EndDate
				AND ManagerID = @Manager
				AND a.DeleteFlag = CAST(0 AS BIT)

-- DEBUGGING ***
--SELECT *
--FROM #TransactionTable
--RETURN;
-- DEBUGGING ***
		-- Gets records from the BeginDate to the end of the month if BEGIN = BeginDate
		IF @BEGIN = @BeginDate  -- I need the BeginBook and BookInventory for the first day
			BEGIN
				INSERT INTO @SummarySheet
				SELECT	@SiteIndex
						, CloseoutDate
						, 0.0
						, 0.0
						, 0.0
						, 0.0
						, GrossBookInventory
						, NetBookInventory
						, GrossPhysicalInventory
						, NetPhysicalInventory
						, 1
				FROM dbo.tblCloseoutInventory WITH(NOLOCK) 
				WHERE [Site] = (SELECT [Site] 
								FROM #Site 
								WHERE SiteIndex = @SiteIndex
								)
				AND ManagerName = @Manager
				AND ProductName = @Product
				AND CloseoutDate = @BeginDate
				
				-- DEBUGGING ***
				--SELECT *
				--FROM @SummarySheet
				--RETURN;
				-- DEBUGGING ***

			END
		ELSE --Find the earliest Physical Inventory Date, if not exists, use the first transaction date as BEGIN date
			BEGIN

				SELECT Product, t.InventoryDate 
				INTO #PhysicalInventory
				FROM tblTransactions t
					INNER JOIN tblTransactionLineItems li ON t.TransID = li.TransID
				WHERE t.TransTypeID = 14
					AND InventoryDate < @BeginDate
				ORDER BY Product, InventoryDate
	
				SELECT @Begin = IsNull((SELECT Max(InventoryDate) 
								FROM #PhysicalInventory p
								WHERE p.Product = @Product),
								(SELECT Min(InventoryDate) 
								 FROM #PhysicalInventory p
								 WHERE p.Product = @Product )) --this may later than begindate??

				INSERT INTO @SummarySheet (SiteIndex, Inventory_Date) 
				VALUES(@SiteIndex,@BEGIN)
				DELETE #PhysicalInventory
				
				-- DEBUGGING ***
				--SELECT *
				--FROM @SummarySheet
				--RETURN;
				-- DEBUGGING ***
			END

		-- Totalize the Transactions FROM BEGIN to END Date in the 
		-- tblTransactionLineItem and tblTransactionSubLineItem tables
		SELECT	SiteIndex
				, InventoryDate As INVENTORY_DATE
				, AliasName
				, TransTypeID
				, Product
				, ISNULL(SUM(dbo.ConvertFromSIUnits(GrossQuantity,@VolumeUnits,@VolumeDecimalPlaces)),0.0) AS Gross
				, ISNULL(SUM(dbo.ConvertFromSIUnits(NetQuantity,@VolumeUnits,@VolumeDecimalPlaces)),0.0) AS Net
		INTO	#TransactionTotals 
		FROM	
		(		SELECT	t.SiteIndex
						, t.InventoryDate
						, t.AliasName
						, t.TransTypeID
						, l.Product
						, l.GrossQuantity
						, l.NetQuantity
				FROM tblTransactionLineItems l WITH(NOLOCK) 
				LEFT OUTER JOIN #TransactionTable t ON l.TransID = t.TransID
				WHERE	l.Product = @Product 
				AND		l.TransactionInventoryDate BETWEEN @BEGIN AND @EndDate 
				AND		l.TransID = t.TransID 
				AND		l.DeleteFlag = CAST(0 AS BIT)
			
			UNION ALL
			
				SELECT	t.SiteIndex
						, t.InventoryDate
						, t.AliasName
						, t.TransTypeID
						, l.Product
						, l.GrossQuantity
						, l.NetQuantity
				FROM tblTransactionSubLineItems l WITH(NOLOCK) 
				LEFT OUTER JOIN #TransactionTable t ON l.TransID = t.TransID
				WHERE	l.Product = @Product 
				AND		l.TransactionInventoryDate BETWEEN @BEGIN AND @EndDate 
				AND		l.TransID = t.TransID 
				AND		l.DeleteFlag = CAST(0 AS BIT)
		) AS UnionTable
		GROUP BY SiteIndex,InventoryDate,AliasName,TransTypeID,Product
		
-- DEBUGGING ***
--SELECT *
--FROM #TransactionTotals 
--RETURN;
-- DEBUGGING ***		
		
		--Build @DateList table lists each day FROM BeginDay to EndDay
		DECLARE @DateList TABLE( Inventory_Date DATETIME,Link INT DEFAULT 1 )

		DECLARE @BeginTemp DATETIME
		SET @BeginTemp = @BEGIN

		WHILE (@BeginTemp <= @EndDate) -- populate with each date
			BEGIN
				 INSERT INTO @DateList(Inventory_Date) 
				 VALUES (@BeginTemp)
				 SET @BeginTemp = DateAdd(day,1,@BeginTemp)
			END

-- DEBUGGING ***
--SELECT *
--FROM @DateList
--RETURN;
-- DEBUGGING ***

		-- Insert Transaction Totals in #Summary table
		SELECT	IDENTITY (INT,1,1) AS RecNo
				, S.SiteIndex
				, D.Inventory_Date AS INVENTORY_DATE
				, Product = (SELECT DISTINCT Product 
							FROM #TransactionTotals
							)
				, S.BEGIN_INVENTORYGross
				, S.BEGIN_INVENTORYNet
				, C.TOTAL_ACTIVITYGross
				, C.TOTAL_ACTIVITYNet
				, S.BOOK_INVENTORYGross
				, S.BOOK_INVENTORYNet
				, P.PHYSICAL_INVENTORYGross
				, P.PHYSICAL_INVENTORYNet
		INTO	#Summary
		FROM	((@DateList D 
					LEFT JOIN @SummarySheet S ON D.Link = S.Link)
					LEFT JOIN 
					(SELECT SiteIndex
							, INVENTORY_DATE
							, Product
							, SUM(Gross) AS TOTAL_ACTIVITYGross
							, SUM(Net) AS TOTAL_ACTIVITYNet
					 FROM	#TransactionTotals
					 WHERE  TransTypeID NOT IN(9,10,11,12,14,17,18) --Only calculate the type affect the book inventory -- kf 4/15/10 exclude Supply Order
					 GROUP BY SiteIndex, INVENTORY_DATE,Product 
					 ) AS C 
					 ON C.SiteIndex = S.SiteIndex AND C.INVENTORY_DATE = D.Inventory_Date
				)
				LEFT JOIN
				(SELECT SiteIndex
						, INVENTORY_DATE
						, SUM(Gross) AS PHYSICAL_INVENTORYGross
						, SUM(Net) AS PHYSICAL_INVENTORYNet
						FROM	#TransactionTotals
						WHERE  TransTypeID = 14 --Only calculate the type affect the book inventory
						GROUP BY SiteIndex, INVENTORY_DATE 
				) AS P ON P.SiteIndex = S.SiteIndex AND P.INVENTORY_DATE = D.Inventory_Date

-- DEBUGGING ***
--SELECT *
--FROM #Summary
--RETURN;
-- DEBUGGING ***

		-- Replace all NULLs with 0 in #Summary table
		UPDATE #Summary 
		SET TOTAL_ACTIVITYGross = 0 
		WHERE TOTAL_ACTIVITYGross IS NULL
		UPDATE #Summary 
		SET TOTAL_ACTIVITYNet = 0 
		WHERE TOTAL_ACTIVITYNet IS NULL
		
		DECLARE @Temp TABLE 
		( RecNo INT,
		  GrossVolume FLOAT,
		  NetVolume FLOAT,
		  PhysicalGross FLOAT,
		  PhysicalNet FLOAT,	
		  MovementGross FLOAT,
		  MovementNet	FLOAT	
		)

		--Set Begining Inventory. 
		--A Physical Inventory will be NEXT Day's BEGIN Book, 
		-- if no physical inventory, END Book Inventory will be next day's BEGIN book 
		SET @BeginTemp = @BEGIN
		WHILE (@BeginTemp <= @EndDate)
		BEGIN
			UPDATE #Summary 
			SET BOOK_INVENTORYGross = BEGIN_INVENTORYGross + TOTAL_ACTIVITYGross
				, BOOK_INVENTORYNet = BEGIN_INVENTORYNet + TOTAL_ACTIVITYNet
			OUTPUT INSERTED.RecNo
					, INSERTED.BOOK_INVENTORYGross
					, INSERTED.BOOK_INVENTORYNet
					, INSERTED.PHYSICAL_INVENTORYGross
					, INSERTED.PHYSICAL_INVENTORYNet
					, 0.0
					, 0.0 
			INTO @Temp
			WHERE	INVENTORY_DATE = @BeginTemp
			
			-- increment BeginTemp date by 1 day
			-- set Physical Gross for next record
			-- set Physical Net for next record
			SET @BeginTemp  = DATEADD(DAY,1,@BeginTemp)
			UPDATE #Summary 
			SET BEGIN_INVENTORYGross = 
				CASE 
					WHEN T.PhysicalGross IS NULL THEN T.GrossVolume
					ELSE T.PhysicalGross
				END,
				BEGIN_INVENTORYNet = 
				CASE 
					WHEN T.PhysicalNet IS NULL THEN T.NetVolume
					ELSE T.PhysicalNet
				END
			FROM #Summary D 
			INNER JOIN @Temp T ON D.RecNo = T.RecNo + 1 
			AND D.INVENTORY_DATE = @BeginTemp

			DELETE FROM @Temp
		END

-- DEBUGGING ***
--SELECT *
--FROM @Temp
--RETURN;
-- DEBUGGING ***

		--Total movements = Type 5 and Type 6
		--Variance percentage = Total variance / Total movements

		--*********************This section builds the Inventory Reconcilation with Physical Inventory********
		--****************************************************************************************************
		SELECT	
				S.*
				, (PHYSICAL_INVENTORYGross - BOOK_INVENTORYGross) AS VARIANCEGross
				, (PHYSICAL_INVENTORYNet - BOOK_INVENTORYNet ) AS VARIANCENet
				, ISNULL(M.MovementGross,0.0) AS MovementGross
				, ISNULL(M.MovementNet,0.0) AS MovementNet
				, @Zero AS TOTAL_VARIANCEGross
				, @Zero AS TOTAL_VARIANCENet
				, @Zero AS TotalMovementGross
				, @Zero As TotalMovementNet
				, @Zero AS TOTAL_PHYSICAL_INVENTORYGross
				, @Zero AS TOTAL_PHYSICAL_INVENTORYNet
				, @Zero AS TotalVarianceGrossPercentage
				, @Zero AS TotalVarianceNetPercentage
				, @Zero AS ToleranceGrossResult -- result comparing the @Tolerance
				, @Zero As ToleranceNetResult
				, @Zero As DailyVarianceGrossPercentage
				, @Zero As DailyVarianceNetPercentage
		INTO	#InventoryReconcilation
		FROM   #Summary S 
		LEFT JOIN 
				(SELECT SiteIndex
						, INVENTORY_DATE
						, SUM(Gross) AS MovementGross
						, SUM(Net) AS MovementNet
				FROM	#TransactionTotals
				WHERE	TransTypeID IN (5,6) 
				AND INVENTORY_DATE >= @BeginDate
				GROUP BY SiteIndex, INVENTORY_DATE) AS M
				ON S.SiteIndex = M.SiteIndex AND S.INVENTORY_DATE = M.INVENTORY_DATE
		WHERE	S.INVENTORY_DATE >= @BeginDate

-- DEBUGGING ***
--SELECT *
--FROM #InventoryReconcilation
--RETURN;
-- DEBUGGING ***
			
		--Calculate the TotalVariance, TotalMovement, Total Physicl Inventory
		SET @BeginTemp = @BeginDate
		UPDATE #InventoryReconcilation
		SET TOTAL_VARIANCEGross = ISNULL(VARIANCEGross,0.0)
			, TOTAL_VARIANCENet = ISNULL(VARIANCENet,0.0)
			, TOTAL_PHYSICAL_INVENTORYGross = ISNULL(PHYSICAL_INVENTORYGross,0.0)
			, TOTAL_PHYSICAL_INVENTORYNet = ISNULL(PHYSICAL_INVENTORYNet,0.0)
			, TotalMovementGross = ISNULL(MovementGross,0.0)
			, TotalMovementNet = ISNULL(MovementNet,0.0)
		OUTPUT	INSERTED.RecNo
				, INSERTED.TOTAL_VARIANCEGross
				, INSERTED.TOTAL_VARIANCENet
				, INSERTED.TOTAL_PHYSICAL_INVENTORYGross
				, INSERTED.TOTAL_PHYSICAL_INVENTORYNet
				, INSERTED.TotalMovementGross
				, INSERTED.TotalMovementNet 
		INTO @Temp
		WHERE Inventory_Date = @BeginTemp

-- DEBUGGING ***
--SELECT *
--FROM @Temp
--RETURN;
-- DEBUGGING ***

		-- increment BeginTemp date by 1 day
		SET @BeginTemp  = DateAdd(day,1,@BeginTemp)

		-- update TotalVarianceGross,TotalVarianceNet,TotalPhysicalGross, TotalPhysicaNet
		WHILE (@BeginTemp <=@EndDate)
		BEGIN
			UPDATE #InventoryReconcilation 
			SET TOTAL_VARIANCEGross = ISNULL(VARIANCEGross,0.0) + T.GrossVolume
				, TOTAL_VARIANCENet = ISNULL(VARIANCENet,0.0) + T.NetVolume
				, TOTAL_PHYSICAL_INVENTORYGross = ISNULL(PHYSICAL_INVENTORYGross,0.0)+T.PhysicalGross
				, TOTAL_PHYSICAL_INVENTORYNet = ISNULL(PHYSICAL_INVENTORYNet,0.0)+T.PhysicalNet
				, TotalMovementGross = D.MovementGross + T.MovementGross
				, TotalMovementNet = D.MovementNet + T.MovementNet
			OUTPUT INSERTED.RecNo
					, INSERTED.TOTAL_VARIANCEGross
					, INSERTED.TOTAL_VARIANCENet
					, INSERTED.TOTAL_PHYSICAL_INVENTORYGross
					, INSERTED.TOTAL_PHYSICAL_INVENTORYNet
					, INSERTED.TotalMovementGross
					, INSERTED.TotalMovementNet 
			INTO @Temp
			FROM #InventoryReconcilation D 
			INNER JOIN @Temp T ON D.RecNo = T.RecNo + 1 
			AND D.Inventory_Date = @BeginTemp

			SET @BeginTemp  = DateAdd(day,1,@BeginTemp)
		END
		DELETE FROM @Temp
		
-- DEBUGGING ***
--SELECT *
--FROM #InventoryReconcilation
--RETURN;
-- DEBUGGING ***		

		-- update DailyVarianceGross
		UPDATE #InventoryReconcilation
		SET DailyVarianceGrossPercentage =
			CASE 
				WHEN VARIANCEGross IS NULL THEN NULL
				ELSE ABS(VARIANCEGross/MovementGross)
			END
		WHERE MovementGross <> 0 
		
		-- update DailyVarianceNet
		UPDATE #InventoryReconcilation
		SET DailyVarianceNetPercentage = 
			CASE 
				WHEN VARIANCENet IS NULL THEN NULL
				ELSE ABS(VARIANCENet/MovementNet)
			END
		WHERE MovementNet <> 0 

		--Calculate the tolerance
		DECLARE @Tolerance FLOAT
		SET @Tolerance =ISNULL((SELECT VarianceTolerance 
								FROM dbo.tblProducts WITH(NOLOCK) 
								WHERE ProductID = @Product 
								AND SiteIndex = @SiteIndex
								),0.0)

		-- update TotalVarianceGross%
		UPDATE #InventoryReconcilation
		SET TotalVarianceGrossPercentage =
			CASE 
				WHEN VARIANCEGross IS NULL THEN NULL
				ELSE ABS(TOTAL_VARIANCEGross/TotalMovementGross)
			END
		WHERE TotalMovementGross <> 0 

		-- update TotalVarianceNet%
		UPDATE #InventoryReconcilation
		SET TotalVarianceNetPercentage = 
			CASE 
				WHEN VARIANCENet IS NULL THEN NULL
				ELSE ABS(TOTAL_VARIANCENet/TotalMovementNet)
			END
		WHERE TotalMovementNet <> 0 

		-- update ToleranceGross and ToleranceNet
		UPDATE #InventoryReconcilation
		SET ToleranceGrossResult = 
			CASE 
				WHEN VARIANCEGross IS NULL THEN NULL 
				ELSE TotalVarianceGrossPercentage - @Tolerance / 100
			END,
			ToleranceNetResult = 
			CASE
				WHEN VARIANCENet IS NULL THEN NULL
				ELSE TotalVarianceNetPercentage - @Tolerance / 100
			END

-- DEBUGGING ***
--SELECT *
--FROM #InventoryReconcilation
--RETURN;
-- DEBUGGING ***		

		---********************This section builds the columnn in the desired display order******************
		---**************************************************************************************************
		-- Build a string for Pivot table
		DECLARE @pivotValues NVARCHAR(2000)
		DECLARE @sqlString NVARCHAR(2000)
		SELECT @pivotValues = ''
		SELECT @sqlString = ''

		--build all aliasNames INTO one string - Gross volume first
		SELECT @pivotValues = @pivotValues + '[' + AliasName + '],'
		FROM @AliasList
		GROUP BY AliasName 

		SELECT @pivotValues = LEFT(@pivotValues, LEN(@pivotValues) - 1)  --remove the last comma

		SELECT @sqlString = 'SELECT * FROM (SELECT SiteIndex, INVENTORY_DATE,AliasName,'+@GrossNet+' FROM #TransactionTotals) AS NewTable PIVOT (SUM('+@GrossNet+') FOR AliasName IN (' + @pivotValues + ')) AS InfoPivot' 
																															  
		SELECT @sqlString = 'SELECT P.*, 
							I.SiteIndex AS SiteIndex1,
							Product,
							I.INVENTORY_DATE AS INVENTORY_DATE'+@GrossNet+','+
							'BEGIN_INVENTORYGross, BEGIN_INVENTORYNet,BOOK_INVENTORYGross,BOOK_INVENTORYNet,
							 TOTAL_ACTIVITYGross,TOTAL_ACTIVITYNet,
							 VARIANCEGross,VARIANCENet,TOTAL_VARIANCEGross,TOTAL_VARIANCENet,
							TotalMovementGross,TotalMovementNet,TOTAL_PHYSICAL_INVENTORYGross,TOTAL_PHYSICAL_INVENTORYNet,
							DailyVarianceGrossPercentage,DailyVarianceNetPercentage,TotalVarianceGrossPercentage,TotalVarianceNetPercentage,
							ToleranceGrossResult, ToleranceNetResult
		 FROM #InventoryReconcilation I LEFT JOIN ('+@sqlString+' ) AS P ON P.SiteIndex=I.SiteIndex AND P.INVENTORY_DATE=I.INVENTORY_DATE'

		---***************************Build InventoryReconcilation View using the user's View Definition*********
		DECLARE @sql NVARCHAR(2000)
		SET @sql = ''
		SELECT @sql = @sql + '['+ColumnName + '] AS Column'+Convert(NVARCHAR(3),ColumnOrder) + ','
		FROM #ViewDefinition
		Group BY ColumnOrder, ColumnName

		SELECT @sql = LEFT(@sql, LEN(@sql) - 1)  --remove the last comma

		SELECT @sql = 'SELECT '+@sql + ',TotalMovementGross,TotalMovementNet,DailyVarianceGrossPercentage,DailyVarianceNetPercentage,TotalVarianceGrossPercentage,TotalVarianceNetPercentage,
							ToleranceGrossResult, ToleranceNetResult,TOTAL_ACTIVITYGross AS TotalActivityGross,TOTAL_ACTIVITYNet AS TotalActivityNet,Product FROM ('+@sqlString+') AS T'
	--	PRINT @sql
		
		EXEC(@sql)

		DELETE #Site
		DELETE #TransactionTable
		DELETE #TransactionTotals
		DELETE #Summary
		DELETE #InventoryReconcilation
		DELETE #ViewDefinition
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_InventoryReconciliationSummarySub TO [public]
GO
