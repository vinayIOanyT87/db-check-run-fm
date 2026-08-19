

CREATE PROCEDURE [dbo].[usp_SummaryJournalGross]
@BeginDate DATE, @EndDate DATE, @Manager NVARCHAR (1000), @Product NVARCHAR (1000), @SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @LoginSiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @VolumeUnits int
	SET @VolumeUnits = (SELECT dbo.tblSites.VolumeUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @VolumeDecimalPlaces int
	SET @VolumeDecimalPlaces = (SELECT dbo.tblSites.VolumeDecimalPlaces FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @SiteList TABLE (Site nvarchar (30))
	INSERT INTO @SiteList SELECT ID FROM dbo.tblSites, [map].[tblSiteToSite] WHERE ParentSiteGuid = @SiteGuid AND SiteGroupFlag = 0

	DECLARE @AuthorizedCompanies TABLE (CompanyID nvarchar(100))
	INSERT INTO @AuthorizedCompanies SELECT * FROM udf_AuthorizedCompanies(@LoginSiteGuid, @SiteGuid,@UserGuid)

	CREATE TABLE #OwnerList 
		 (Site nvarchar (30) NOT NULL, 
		  OwnerID nvarchar (100) NOT NULL, 
		  PriorCloseoutDate DATE NOT NULL, 
		  PriorGrossVolume float)
	ALTER TABLE #OwnerList WITH NOCHECK ADD 
		CONSTRAINT [PK_TempOwnerList] PRIMARY KEY CLUSTERED 
		(
			[Site],
			  [OwnerID]
		)  
	CREATE INDEX [IX_OwnerList_Site_OwnerID] ON #OwnerList([Site], [OwnerID]) 

	INSERT INTO #OwnerList 
	  SELECT DISTINCT Site, CompanyName AS OwnerID,'1901-01-01', 0
	  FROM @SiteList,udf_CompanyList(@LoginSiteGuid,@SiteGuid,1,0) 
	  WHERE CompanyName IN(SELECT * FROM @AuthorizedCompanies) 
	  ORDER BY Site, CompanyName

	UPDATE #OwnerList
		SET [PriorCloseoutDate] = b.[CloseoutDate],
			[PriorGrossVolume] = b.[GrossBookInventory]
	FROM #OwnerList a, dbo.tblOwnerCloseout b
	WHERE a.[OwnerID] = b.[OwnerName] 
			AND b.[ManagerName] = @Manager 
			AND b.[ProductName] = @Product 
			AND b.CloseoutDate = (SELECT MAX(CloseoutDate)
										FROM dbo.tblOwnerCloseout c
										WHERE b.Site = c.Site 
												 AND b.ManagerName = c.ManagerName 
												 AND b.ProductName = c.ProductName 
												 AND b.OwnerName = c.OwnerName 
												 AND c.CloseoutDate < @BeginDate) 
			AND a.Site = b.Site

	-- Determine if the site is a non-group site. If it is a non-group site, then the closeout date
	-- should be the same for every owner. To made the transaction query faster, the closeout date
	-- scalar value will be used. 
	DECLARE @CloseoutDate DATE
	DECLARE @SiteName nvarchar(30)
	DECLARE @IsSiteGroup bit
	SELECT @IsSiteGroup = SiteGroupFlag, @SiteName = [ID] FROM dbo.tblSites WHERE SiteGuid = @SiteGuid

	IF (@IsSiteGroup = 0)
	  BEGIN
		  SELECT @CloseoutDate = MAX(PriorCloseoutDate) FROM #OwnerList WHERE [Site] = @SiteName
	  END


	-- Get the Transaction Aliases
	DECLARE @AliasList TABLE
	(
		AliasName nvarchar (32),
		LookupTransTypeIndex INT,
		TransactionAliasGuid UNIQUEIDENTIFIER
	)

	INSERT INTO @AliasList SELECT * FROM udf_AliasList(@SiteGuid) WHERE LookupTransTypeIndex NOT IN(9,10,11,14,17) ORDER BY AliasName 

	-- Create the Summary Journal Table
	CREATE TABLE #SummaryJournal (OwnerID nvarchar (100), [Begin Inventory] float, [Book Inventory] float)
	DECLARE @AlterTableString nvarchar (2000)
	DECLARE @AliasName nvarchar (32)
	DECLARE @LookupTransTypeIndex int
	DECLARE @FirstTime bit
	DECLARE AliasCursor CURSOR FOR SELECT * FROM @AliasList 
	SET @FirstTime = 1

	OPEN AliasCursor
	FETCH NEXT FROM AliasCursor INTO @AliasName, @LookupTransTypeIndex

	SET @AliasName = '[' + @AliasName + ']'
	WHILE @@FETCH_STATUS = 0
	BEGIN
		 IF (@FirstTime = 1)
			BEGIN
			  SET @AlterTableString = 'ALTER TABLE #SummaryJournal ADD ' + @AliasName + ' float NULL'
				SET @FirstTime = 0
			END
		 ELSE
			BEGIN
			  SET @AlterTableString = @AlterTableString + ', ' + @AliasName + ' float NULL'
			END

		FETCH NEXT FROM AliasCursor INTO @AliasName, @LookupTransTypeIndex
		SET @AliasName = '[' + @AliasName + ']'
	END

	EXEC(@AlterTableString)
	CLOSE AliasCursor

	CREATE TABLE #TransactionTotals
	(
		AliasName nvarchar (32),
		LookupTransTypeIndex INT,
		OwnerID nvarchar (100),
		GrossQuantity [float],
		IsPrior [int]
	)

	DECLARE @TransactionTable TABLE
	(
		Site nvarchar(30),
		OwnerID nvarchar(100),
		AliasName nvarchar(32),
		LookupTransTypeIndex INT,
		TransID nvarchar(64),
		 IsPrior [int],
		 TransactionGuid UNIQUEIDENTIFIER
	)

	-- Check to see if the site is a non-site group. If so, then use scalar values for performance.
	IF (@IsSiteGroup = 0)
	  BEGIN
		-- Since the site is a non-group site, use the closeout date scalar value for better performance.
		 -- Get Transactions from Closeout Date to End Date
		INSERT INTO @TransactionTable
		SELECT a.Site, a.OwnerID, a.AliasName, a.LookupTransTypeIndex, a.TransID,
				(CASE
				 WHEN a.InventoryDate < @BeginDate
					THEN 1
					ELSE 0
				END) AS IsPrior,
				TransactionGuid
		 FROM dbo.tblTransactions a
		 WHERE a.LookupTransTypeIndex <> 14
			AND a.Site = @SiteName
			AND a.DeleteFlag = cast(0 AS bit)
			AND a.ManagerID = @Manager
			AND EXISTS (SELECT CompanyID 
						FROM @AuthorizedCompanies 
						WHERE CompanyID IN (a.CarrierID, a.ShipperID, a.ShipToID, a.SupplierID, a.ManagerID, a.OwnerID, a.BillToID))
			AND (a.InventoryDate > @CloseoutDate AND a.InventoryDate <= @EndDate)
	  END
	ELSE
	  BEGIN
		 -- Since the site is a group site, the performance will be degraded.
		-- Get Transactions from Closeout Date to End Date
		INSERT INTO @TransactionTable
		SELECT a.Site, a.OwnerID, a.AliasName, a.LookupTransTypeIndex, a.TransID,
				(CASE
				 WHEN a.InventoryDate < @BeginDate
					THEN 1
					ELSE 0
				END) AS IsPrior
		 FROM dbo.tblTransactions a LEFT OUTER JOIN #OwnerList ol ON a.Site = ol.Site AND a.OwnerID = ol.OwnerID
		 WHERE a.LookupTransTypeIndex <> 14
			AND a.Site IN (SELECT Site FROM @SiteList)
			AND a.DeleteFlag = cast(0 AS bit)
			AND a.ManagerID = @Manager
			AND EXISTS (SELECT CompanyID 
						FROM @AuthorizedCompanies 
						WHERE CompanyID IN (a.CarrierID, a.ShipperID, a.ShipToID, a.SupplierID, a.ManagerID, a.OwnerID, a.BillToID))
			AND (a.InventoryDate > ol.PriorCloseoutDate AND a.InventoryDate <= @EndDate)
	  END

	-- Totalize the Transactions from Begin Date to End Date
	INSERT INTO #TransactionTotals 
	SELECT t.AliasName, t.LookupTransTypeIndex, t.OwnerID,
			 IsNull(SUM(dbo.udf_ConvertFromSIUnits(l.GrossQuantity, @VolumeUnits, @VolumeDecimalPlaces)), 0.0) AS GrossQuantity,
			 t.IsPrior
	FROM dbo.tblTransactionLineItems l LEFT OUTER JOIN @TransactionTable t ON l.TransactionGuid = t.TransactionGuid
	WHERE l.Product = @Product
			AND t.IsPrior = 0
			AND l.TransactionGuid = t.TransactionGuid
			AND (l.TransactionInventoryDate BETWEEN @BeginDate AND @EndDate)
			AND (l.DeleteFlag = cast(0 AS bit))
	GROUP BY t.AliasName,  t.LookupTransTypeIndex, t.OwnerID, t.IsPrior
	ORDER BY t.AliasName,  t.OwnerID

	-- Totalize the Transactions from Closeout Date to Begin Date
	INSERT INTO #TransactionTotals 
	SELECT t.AliasName, t.LookupTransTypeIndex, t.OwnerID,
			 IsNull(SUM(dbo.udf_ConvertFromSIUnits(l.GrossQuantity, @VolumeUnits, @VolumeDecimalPlaces)), 0.0) AS GrossQuantity,
			 t.IsPrior
	FROM ((dbo.tblTransactionLineItems l LEFT OUTER JOIN @TransactionTable t ON l.TransactionGuid = t.TransactionGuid) 
			 LEFT OUTER JOIN #OwnerList ol ON ol.Site = t.Site AND ol.OwnerID = t.OwnerID)
	WHERE l.Product = @Product 
			AND t.IsPrior = 1
			AND l.TransactionGuid = t.TransactionGuid
			AND (l.TransactionInventoryDate > ol.PriorCloseoutDate AND l.TransactionInventoryDate < @BeginDate)
			AND (l.DeleteFlag = cast(0 AS bit))
	GROUP BY t.AliasName,  t.LookupTransTypeIndex, t.OwnerID, t.IsPrior
	ORDER BY t.AliasName,  t.OwnerID

	-- Initialize the Summary Journal
	DECLARE @AliasColumnName nvarchar (34)
	DECLARE @InitializeString nvarchar (1000)
	DECLARE @ValuesString nvarchar (1000)
	SELECT @InitializeString = 'INSERT INTO #SummaryJournal (OwnerID, [Begin Inventory], [Book Inventory]'
	SELECT @ValuesString = 'SELECT DISTINCT OwnerID, 0, 0 '
	OPEN AliasCursor
	FETCH NEXT FROM AliasCursor INTO @AliasName, @LookupTransTypeIndex
	SET @AliasColumnName = '[' + @AliasName + ']'
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT @InitializeString = @InitializeString + ',' + @AliasColumnName
		SELECT @ValuesString = @ValuesString + ',0 '
		FETCH NEXT FROM AliasCursor INTO @AliasName, @LookupTransTypeIndex
		SET @AliasColumnName = '[' + @AliasName + ']'
	END	 
	SELECT @InitializeString = @InitializeString + ')' + @ValuesString+'FROM #OwnerList'  
	CLOSE AliasCursor
	EXEC(@InitializeString)

	-- Update Begin Inventory with PriorGrossVolume from dbo.tblOwnerClosout
	UPDATE #SummaryJournal 
		SET [Begin Inventory] = IsNull((SELECT SUM(dbo.udf_ConvertFromSIUnits(PriorGrossVolume, @VolumeUnits, @VolumeDecimalPlaces))
											FROM #OwnerList b WHERE b.OwnerID = a.OwnerID), 0)
	FROM #SummaryJournal a

	-- Update Begin Inventory with Transactions prior to BeginDate
	UPDATE #SummaryJournal SET [Begin Inventory] = [Begin Inventory] +
		IsNull((SELECT SUM(IsNull(GrossQuantity, 0))
				  FROM #TransactionTotals b
				  WHERE b.OwnerID = a.OwnerID AND 
							b.IsPrior = 1 AND 
							b.LookupTransTypeIndex <> 12 AND 
							b.LookupTransTypeIndex <> 7), 0) FROM #SummaryJournal a

	DELETE FROM #TransactionTotals WHERE IsPrior = 1;

	-- Update Book Inventory
	UPDATE #SummaryJournal SET [Book Inventory] = [Begin Inventory]

	-- Update Transaction Totals
	DECLARE @UpdateString nvarchar (4000)
	OPEN AliasCursor
	FETCH NEXT FROM AliasCursor INTO @AliasName, @LookupTransTypeIndex
	SET @AliasColumnName = '[' + @AliasName + ']'
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF(@LookupTransTypeIndex = 5 OR @LookupTransTypeIndex = 6)
			SELECT @UpdateString = 'UPDATE #SummaryJournal SET ' + @AliasColumnName + ' = -GrossQuantity,
			[Book Inventory] = [Book Inventory] + GrossQuantity'
		ELSE IF ((@LookupTransTypeIndex <> 12) AND (@LookupTransTypeIndex <> 7))
			SELECT @UpdateString = 'UPDATE #SummaryJournal SET ' + @AliasColumnName + ' = GrossQuantity,
			[Book Inventory] = [Book Inventory] + GrossQuantity'
		ELSE
			SELECT @UpdateString = 'UPDATE #SummaryJournal SET ' + @AliasColumnName + ' = GrossQuantity'

		SELECT @UpdateString = @UpdateString + ' FROM #SummaryJournal a, #TransactionTotals b
		WHERE b.OwnerID = a.OwnerID
		AND AliasName = ''' + @AliasName + ''''

		EXEC(@UpdateString)


		FETCH NEXT FROM AliasCursor INTO @AliasName, @LookupTransTypeIndex
		SET @AliasColumnName = '[' + @AliasName + ']'
	END

	CLOSE AliasCursor

	-- Final Query
	SELECT * FROM #SummaryJournal ORDER BY OwnerID
END
