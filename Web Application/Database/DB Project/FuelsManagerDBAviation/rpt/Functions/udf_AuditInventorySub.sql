CREATE Function [rpt].[udf_AuditInventorySub] 
(
	@Site UNIQUEIDENTIFIER,
	@Manager UNIQUEIDENTIFIER,
	@Owner UNIQUEIDENTIFIER,
	@Product UNIQUEIDENTIFIER,
	@BeginDate DATETIMEOFFSET(7),
	@EndDate DATETIMEOFFSET(7),
	@GrossNet BIT,
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER
)
RETURNS @TotalSum TABLE
(
						[OwnerID]			NVARCHAR(100),
						SiteID				NVARCHAR(100),
						InventoryDate		DATETIMEOFFSET(7),
						[Begin Inventory]	FLOAT,
						[Book Inventory]    FLOAT,
						[TransAmt]			FLOAT,
						[24 Hr]				FLOAT,
						[Adjustment]		FLOAT,
						[Bulk Issue]		FLOAT,
						[Defuel]			FLOAT,
						[Issue]				FLOAT,
						[Load Rack]			FLOAT,
						[LR Receipt]		FLOAT,
						[Receipt]			FLOAT,
						[Rotation]			FLOAT,
						[Transfer]			FLOAT

)		
AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[udf_AuditInventorySub] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Workhorse that retrieves the transaction records for the Audit Inventory Report
	-- Notes:
	-- 1. @Site: A single SiteGuid (not SiteGroups) to retrieve the transactions from.
	-- 2. @Manager: MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owner: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Product: ProductGuid for wich the list of transactions are filtered on.
	-- 5. @BeginDate: Lower bound date to collect transactions meeting criteria
	-- 6. @EndDate: Upper bound date to collect transactions meeting criteria
	-- 7. @GrossNet: Boolean indicated whether the Gross or Net quantity values are to be returned for the transaction list.
	-- 8. @SiteGuid: Identifies the site the report is being run from
	-- 9. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------

	/***************
		BEGIN
		Site List
	*****************/
    DECLARE @SiteList TABLE
	(
		SiteID NVARCHAR (100),
		SiteGuid UNIQUEIDENTIFIER,
		ManagerCompanyGuid UNIQUEIDENTIFIER,
		PriorCloseoutDate DATETIMEOFFSET(7),
		PriorCloseoutVolume FLOAT
	)

	INSERT INTO @SiteList
	SELECT b.ID, b.SiteGuid, NULL, '1901-01-01', 0
	FROM tblSites b 
	WHERE b.SiteGuid = @Site

	UPDATE @SiteList
	SET PriorCloseoutDate = b.CloseoutDate,
		PriorCloseoutVolume = CASE WHEN @GrossNet = 1 
			THEN dbo.udf_ConvertFromSIUnits(b.GrossBookInventory, tblSites.VolumeUnitIndex, tblSites.VolumeDecimalPlaces) 
			ELSE dbo.udf_ConvertFromSIUnits(b.NetBookInventory, tblSites.VolumeUnitIndex, tblSites.VolumeDecimalPlaces) 
		END,
		ManagerCompanyGuid = b.ManagerCompanyGuid
	FROM @SiteList a 
	INNER JOIN tblOwnerCloseout b ON a.SiteGuid = b.SiteGuid
	INNER JOIN tblSites ON a.SiteGuid = tblSites.SiteGuid
	WHERE b.ManagerCompanyGuid = @Manager AND 
    b.OwnerCompanyGuid = @Owner AND 
    b.ProductGuid = @Product AND 
    b.CloseoutDate = (
		SELECT MAX(CloseoutDate)
		FROM tblOwnerCloseout c 
		WHERE a.SiteGuid = c.SiteGuid AND 
        c.ManagerCompanyGuid = @Manager AND 
        c.OwnerCompanyGuid = @Owner AND 
        c.ProductGuid = @Product AND 
        c.CloseoutDate < @BeginDate
	)

	/***************
		END
		Site List
	*****************/

	/***************
		BEGIN
		Prior Closeout Date
	*****************/
	DECLARE @PriorCloseoutDate DATETIMEOFFSET(7)
	SET @PriorCloseoutDate = (SELECT MAX(PriorCloseoutDate) FROM @SiteList WHERE ManagerCompanyGuid = @Manager)
	SET @PriorCloseoutDate = ISNULL(@PriorCloseoutDate,'1901-01-01')

	/***************
		END
		Prior Closeout Date
	*****************/

	/***********
		BEGIN
		Ledger
	*************/
	DECLARE @Ledger TABLE 
	(
		OwnerID			  NVARCHAR(100),
		InventoryDate	  DATETIMEOFFSET(7),	
		AliasName		  NVARCHAR(32),
		SiteID			  NVARCHAR(60),
		GrossNet		  FLOAT						
	);

	INSERT INTO @Ledger
	SELECT OwnerID
		,InventoryDate
		,AliasName, SiteID
		,GrossNet = CASE WHEN @GrossNet = 1 
			THEN ISNULL(SUM(dbo.udf_ConvertFromSIUnits(GrossQuantity, VolumeUnitIndex, VolumeDecimalPlaces)), 0.0) 
			ELSE ISNULL(SUM(dbo.udf_ConvertFromSIUnits(NetQuantity, VolumeUnitIndex, VolumeDecimalPlaces)), 0.0) 
		END	   
	FROM
	(
		SELECT  t.OwnerID, t.InventoryDate, t.AliasName, t.[Site] AS SiteID, l.GrossQuantity, l.NetQuantity, tblSites.VolumeUnitIndex, tblSites.VolumeDecimalPlaces 
		FROM tblTransactionLineItems l  
		INNER JOIN tblTransactions t ON l.TransactionGuid = t.TransactionGuid
		INNER JOIN tblSites ON t.SiteGuid = tblSites.SiteGuid
		WHERE l.ProductGuid = @Product 
			AND (t.InventoryDate > @PriorCloseoutDate AND t.InventoryDate <= @EndDate)
			AND t.ManagerCompanyGuid = @Manager
			AND t.DeleteFlag = CAST(0 AS BIT)
			AND l.DeleteFlag = CAST(0 AS BIT)
			AND t.OwnerCompanyGuid = @Owner
			AND t.SiteGuid = @Site
			AND t.LookupTransTypeIndex NOT IN (7, 12, 14) -- T7_FillStand, T12 InventoryNotAffected, T14 PhysicalInventory
			AND EXISTS (SELECT *
				FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
				WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
				OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) 
	) AS transQuantity  
	GROUP BY OwnerID,SiteID,InventoryDate,AliasName
	ORDER BY OwnerID,SiteID,InventoryDate
	/***********
		END
		Ledger
	************/

	--build @DateList table lists each day FROM BeginDay to EndDay
	DECLARE @DateList TABLE
	( 
		InventoryDate DATETIMEOFFSET(7),
		[Book Inventory] FLOAT,
		Link INT DEFAULT 1 
	)

	DECLARE @BeginTemp DATE
	SET @BeginTemp = CAST(@BeginDate AS DATE)

	WHILE (@BeginTemp <= @EndDate) -- populate with each date
	BEGIN
		INSERT INTO @DateList(InventoryDate) VALUES (CAST(@BeginTemp AS DATETIMEOFFSET(7)))
		SET @BeginTemp = DATEADD(day,1,@BeginTemp)
	END

	/**********
		BEGIN
	Begining Book List  
	************/

	DECLARE @BeginTotTable TABLE 
	(
		OwnerID			  NVARCHAR(100),
		[BeginAmt]		  FLOAT
	);				
		
	INSERT INTO @BeginTotTable
	SELECT OwnerID
		,[BeginAmt] = ISNULL(SUM(GrossNet),0)
	FROM @Ledger
	WHERE (InventoryDate > @PriorCloseoutDate AND InventoryDate < @BeginDate) 
	GROUP BY OwnerID

	--CONTROLS IF STATEMENT THAT UPDATES [Begin Inventory]
	DECLARE @BeginTot FLOAT
	SET @BeginTot = (SELECT [BeginAmt] FROM @BeginTotTable)

	/**********
		END
	Begining Book List
	************/

	/**********
		BEGIN
	Alias Totals
	************/

	DECLARE @Totals TABLE 
	(
		OwnerID			  NVARCHAR(100),
		SiteID			  NVARCHAR(60),
		InventoryDate	  DATETIMEOFFSET(7),	
		[Begin Inventory] FLOAT,
		AliasName		  NVARCHAR(30),
		[TransAmt]		  FLOAT,
		[Book Inventory]  FLOAT,
		[SumTrans]		  FLOAT
	);		
					
	INSERT INTO @Totals
	SELECT OwnerID
		,SiteID
		,d.InventoryDate
		,0 AS [Begin Inventory]
		,AliasName
		,GrossNet AS [TransAmt]
		,0 AS [Book Inventory]
		,0 AS [SumTrans]				
	FROM @Ledger l
	RIGHT JOIN @DateList d ON
	l.InventoryDate = d.InventoryDate
	WHERE d.InventoryDate >= @BeginDate AND d.InventoryDate <= @EndDate
	ORDER BY OwnerID,SiteID,d.InventoryDate

	/********** 
		END
	Alias Totals
	************/

	/**********
		BEGIN
	Total Sums
	************/
				
	INSERT INTO @TotalSum
	SELECT 
		[OwnerID]
		,SiteID
		,d.InventoryDate
		,0 AS [Begin Inventory]
		,0 AS [Book Inventory]
		,ISNULL(SUM(GrossNet),0) AS [TransAmt]
		,0 AS [24 Hr] 
		,0 AS [Adjustment]
		,0 AS [Bulk Issue]
		,0 AS [Defuel]
		,0 AS [Issue]
		,0 AS [Load Rack]
		,0 AS [LR Receipt]	
		,0 AS [Receipt]	
		,0 AS [Rotation]
		,0 AS [Transfer]					
	FROM @Ledger l
	RIGHT JOIN @DateList d 
	ON l.InventoryDate = d.InventoryDate
	WHERE d.InventoryDate >= @BeginDate AND d.InventoryDate <= @EndDate
	GROUP BY [OwnerID],SiteID,d.InventoryDate
	ORDER BY d.InventoryDate

	/********** 
		END
	Total Sums
	************/

	/**************** 
		BEGIN
		UPDATE
	[Begin Inventory]
	******************/
	DECLARE @PriorCloseoutVolueVar float
	SET @PriorCloseoutVolueVar = 
	(
		SELECT ISNULL
		(
			(
				SELECT PriorCloseoutVolume 
				FROM @SiteList 
				WHERE ManagerCompanyGuid = @Manager
			)
			,0.00
		)
	)

	IF @BeginTot IS NULL
	BEGIN 
		UPDATE @TotalSum
		SET [Begin Inventory] = @PriorCloseoutVolueVar
		FROM @TotalSum 
		WHERE DATEPART(DAY,InventoryDate) = 1 
	END
	ELSE
	BEGIN
		UPDATE @TotalSum
		SET [Begin Inventory] = @BeginTot + @PriorCloseoutVolueVar
		FROM @TotalSum 
		WHERE DATEPART(DAY,InventoryDate) = 1 
	END

	/****************
		END
		UPDATE
	 [Begin Inventory]
	******************/

	/********** 
		BEGIN
	UPDATE Total Sums
	************/

	UPDATE @Totals
	SET SumTrans = b.TransAmt
	FROM @Totals a
	INNER JOIN @TotalSum b
	ON a.InventoryDate = b.InventoryDate

	UPDATE @TotalSum
	SET [24 Hr] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = '24 Hour Closeout'

	UPDATE @TotalSum
	SET [Adjustment] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Adjustment'

	UPDATE @TotalSum
	SET [Bulk Issue] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Bulk Issue'

	UPDATE @TotalSum
	SET [Defuel] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Defuel'

	UPDATE @TotalSum
	SET [Issue] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Issue'

	UPDATE @TotalSum
	SET [Load Rack] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Load Rack'

	UPDATE @TotalSum
	SET [LR Receipt] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'LR Receipt'

	UPDATE @TotalSum
	SET [Receipt] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Receipt'

	UPDATE @TotalSum
	SET [Rotation] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Rotation'

	UPDATE @TotalSum
	SET [Transfer] = t.[TransAmt] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Transfer'

	/********** 
		BEGIN
	UPDATE Total Sums
	************/

	/****************
		BEGIN
		Main Query
	******************/

	DECLARE @TotalsOwner  NVARCHAR(100)
	DECLARE @TotalSite NVARCHAR(100)
	DECLARE @TotalsInvDate DATETIMEOFFSET(7)
	DECLARE @TotalsBegin FLOAT
	DECLARE @TotalsTransAmt FLOAT
	DECLARE @TotalsBook FLOAT
	DECLARE @TempTotalDay FLOAT
	SET @TempTotalDay = (SELECT TOP 1 [Begin Inventory] FROM @TotalSum ORDER BY InventoryDate)
	SET @TempTotalDay = (SELECT ISNULL(@TempTotalDay ,0.00))

	DECLARE TotalsCursor CURSOR FAST_FORWARD FOR SELECT [OwnerID], InventoryDate, [Begin Inventory], TransAmt, [Book Inventory] FROM @TotalSum ORDER BY InventoryDate
	OPEN TotalsCursor
	FETCH NEXT FROM TotalsCursor INTO @TotalsOwner, @TotalsInvDate, @TotalsBegin, @TotalsTransAmt, @TotalsBook
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @TotalsTransAmt = (SELECT ISNULL(@TotalsTransAmt,0.00))
		SET @TempTotalDay = @TempTotalDay + @TotalsTransAmt
		UPDATE @TotalSum SET [Book Inventory] = (SELECT @TempTotalDay) WHERE InventoryDate = @TotalsInvDate 
		SET @TotalsBook = (SELECT ISNULL((SELECT TOP 1 [Book Inventory] FROM @TotalSum WHERE InventoryDate = @TotalsInvDate),0.00))
		UPDATE @TotalSum SET [Begin Inventory] = (SELECT @TotalsBook) WHERE InventoryDate = DATEADD(DAY,1,@TotalsInvDate)
		UPDATE @TotalSum SET [OwnerID] = (SELECT TOP 1 OwnerID FROM @Ledger)
		UPDATE @TotalSum SET [SiteID] = (SELECT TOP 1 SiteID FROM @Ledger)	
	
		FETCH NEXT FROM TotalsCursor INTO @TotalsOwner, @TotalsInvDate, @TotalsBegin, @TotalsTransAmt, @TotalsBook
	END

	CLOSE TotalsCursor
	DEALLOCATE TotalsCursor

	/****************
		BEGIN
		GROUPING 
	(Removes Owners that
	 have 0 Ledger for the
	 entire month)
	******************/

	DECLARE @GroupingTotalSums TABLE 
	(
		[OwnerID] NVARCHAR(100)
	);	

	INSERT INTO @GroupingTotalSums
	SELECT DISTINCT OwnerID
	FROM @TotalSum 
	WHERE ([Begin Inventory] <> 0 OR [Book Inventory] <> 0 OR [24 Hr] <> 0 OR [Adjustment]<> 0 OR
			[Bulk Issue] <> 0 OR [Defuel] <> 0 OR [Issue] <> 0 OR [Load Rack] <> 0 OR [LR Receipt] <> 0 OR 
			[Receipt] <> 0 OR [Rotation] <> 0 OR [Transfer]<> 0) AND OwnerID <> ''
	GROUP BY OwnerID
	ORDER BY OwnerID

	/****************
		END
		GROUPING 
	(Removes Owners that
	 have 0 Ledger for the
	 entire month)
	******************/
	DELETE FROM @TotalSum WHERE OwnerID NOT IN (SELECT OwnerID FROM @GroupingTotalSums)	
	RETURN;

	/****************
		END
		Main Query
	******************/

END