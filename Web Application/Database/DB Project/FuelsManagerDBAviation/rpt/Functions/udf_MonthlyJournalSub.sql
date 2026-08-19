CREATE FUNCTION [rpt].[udf_MonthlyJournalSub] 
(
	@Site UNIQUEIDENTIFIER,
	@Manager UNIQUEIDENTIFIER,
	@Owner UNIQUEIDENTIFIER,
	@Product UNIQUEIDENTIFIER,
	@BeginDate DATETIMEOFFSET(7),
	@EndDate DATETIMEOFFSET(7),
	@GrossNet BIT,
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER,
	@RemoveOwnersWithNoActivity BIT, 
	@EnterpriseStatus BIT
)
RETURNS @TotalSum TABLE
(
						[OwnerID]			NVARCHAR(100),
						InventoryDate		DATETIMEOFFSET(7),
						[Begin Inventory]	FLOAT,
						[Book Inventory]    FLOAT,
						[TransAmt]			FLOAT,
						[DetailTrans]		FLOAT,
						[24 Hr]				FLOAT,
						[Adjustment]		FLOAT,
						[Bulk Issue]		FLOAT,
						[Defuel]			FLOAT,
						[Issue]				FLOAT,
						[Load Rack]			FLOAT,
						[LR Receipt]		FLOAT,
						[Receipt]			FLOAT,
						[Rotation]			FLOAT,
						[Transfer]			FLOAT,
						[VolumeUnitIndex]   INT,
						[VolumeDecimalPlaces] TINYINT,
						[ProductID]			NVARCHAR(30)

)		
AS 
BEGIN

	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_MonthlyJournalSub] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Monthly Journal Report
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Manager: Company MasterRecordGuid assigned the role of manager that the transactions list has the manager for itself to be included in the results
	-- 3. @Owner: Company MasterRecordGuid assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Product: MasterRecordGuid for the product that is associated with the transaction list for the report.
	-- 5. @BeginDate: Lower bound date to collect transactions meeting criteria
	-- 6. @EndDate: Upper bound date to collect transactions meeting criteria
	-- 7. @GrossNet: Bit indicating whether to return the Gross or Net quantity values in the transaction list.
	-- 8. @SiteGuid: Identifies the site the report is being run from
	-- 9. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------

	Declare @ProductID nvarchar(30)

	Set @ProductID = (Select top 1 ProductID from tblProducts where ProductGuid = @Product);

	/***************
		BEGIN
		Site List
	*****************/
	DECLARE @SiteList TABLE
	(
		Site NVARCHAR (30),
		SiteGuid UNIQUEIDENTIFIER,
		ManagerCompanyGuid UNIQUEIDENTIFIER,
		PriorCloseoutDate DATETIMEOFFSET(7),
		PriorCloseoutVolume FLOAT
	)

	-- Only insert sites that are managed by the manager whose guid was passed in.
	-- If there are no matching sites, exit the function with an empty table.
	INSERT INTO @SiteList
	SELECT s.ID, s.SiteGuid, @Manager, '1901-01-01', 0
	FROM tblSites s  
	WHERE s.SiteGuid = @Site and s.SiteGroupFlag = 0

	DECLARE @SiteGroupLevelVolumeUnitIndex INT
	DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

	SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
		@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
	FROM tblSites 
	WHERE SiteGuid = @SiteGuid

	UPDATE @SiteList
	SET PriorCloseoutDate = b.CloseoutDate,
		PriorCloseoutVolume = CASE WHEN @GrossNet = 1 
			THEN dbo.udf_ConvertFromSIUnits(b.GrossBookInventory, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) 
			ELSE dbo.udf_ConvertFromSIUnits(b.NetBookInventory, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)
		END,
		ManagerCompanyGuid = b.ManagerCompanyGuid
	FROM @SiteList a
	INNER JOIN tblOwnerCloseout b ON a.SiteGuid = b.SiteGuid
	WHERE b.ManagerCompanyGuid = @Manager  
		AND b.OwnerCompanyGuid = @Owner  
		AND b.ProductGuid = @Product  
		AND b.CloseoutDate = 
		(
			SELECT MAX(CloseoutDate)
			FROM tblOwnerCloseout c  
			WHERE a.SiteGuid = c.SiteGuid  
				AND c.ManagerCompanyGuid = @Manager  
				AND c.OwnerCompanyGuid = @Owner  
				AND c.ProductGuid = @Product  
				AND c.CloseoutDate < @BeginDate
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

	DECLARE @Ledger TABLE (
		OwnerID			  NVARCHAR(100),
		InventoryDate	  DATETIMEOFFSET(7),	
		AliasName		  NVARCHAR(30),
		[Site]			  NVARCHAR(30),
		GrossNet		  FLOAT,
		VolumeUnitIndex	  INT,
		VolumeDecimalPlaces TINYINT						
	);

	INSERT INTO @Ledger
	SELECT 
		OwnerID
		,InventoryDate
		,AliasName, [Site]
		,GrossNet = CASE WHEN @GrossNet = 1 
			THEN ISNULL(SUM(dbo.udf_ConvertFromSIUnits(GrossQuantity, VolumeUnitIndex, VolumeDecimalPlaces)), 0.0) 
			ELSE ISNULL(SUM(dbo.udf_ConvertFromSIUnits(NetQuantity, VolumeUnitIndex, VolumeDecimalPlaces)), 0.0) 
		END,
		VolumeUnitIndex,
		VolumeDecimalPlaces
	FROM
	(
		SELECT t.OwnerID, 
			t.InventoryDate, 
			t.AliasName, 
			t.Site, 
			l.GrossQuantity, 
			l.NetQuantity,
			@SiteGroupLevelVolumeUnitIndex AS VolumeUnitIndex, 
			@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces 
		FROM tblTransactionLineItems l  
		INNER JOIN tblTransactions t ON l.TransactionGuid = t.TransactionGuid
		INNER JOIN @SiteList sites ON sites.SiteGuid = t.SiteGuid 
		WHERE l.ProductGuid = @Product 
			AND (t.InventoryDate > @PriorCloseoutDate AND t.InventoryDate < @EndDate)
			AND t.ManagerCompanyGuid = @Manager
			AND t.DeleteFlag = CAST(0 AS BIT)
			AND l.DeleteFlag = CAST(0 AS BIT)
			AND t.OwnerCompanyGuid = @Owner
			AND t.LookupTransTypeIndex NOT IN (14) -- T14_PhysicalInventory
			-- TFS #71982 - Monthly and Summary Journal Reports not Filtering on User Group Assignments
			-- Bryan Ponnwitz - 3/24/2017
			AND EXISTS (SELECT *
						FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
						WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
						OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid))
			-- End TFS #71982
			AND t.LookupTransactionStatusIndex IN (select s.TransactionStatusIndex from [rpt].[udf_GetEnterpriseStatusTable](@EnterpriseStatus) s) 
	) as test   

	GROUP BY OwnerID,InventoryDate,AliasName,[Site],VolumeUnitIndex,VolumeDecimalPlaces
	ORDER BY OwnerID,InventoryDate

	--If no records exists in the @Ledger Table, then insert at least one 
	if(not exists(Select top 1 * from @Ledger))
		BEGIN
			INSERT INTO @Ledger				 
			SELECT  (Select ID from tblCompanies where CompanyGuid = @Owner), 
					@BeginDate, 
					'Issue', 
					(Select ID from tblSites where SiteGuid = @Site), 
					0.0, 
					@SiteGroupLevelVolumeUnitIndex, 
					@SiteGroupLevelVolumeDecimalPlaces
		END

	/***********
		END
		Ledger
	************/

	--build @DateList table lists each day from BeginDay to EndDay
	DECLARE @DateList TABLE
	( 
		InventoryDate DATETIMEOFFSET(7),
		[Book Inventory] FLOAT,
		Link int DEFAULT 1 
	)

	DECLARE @BeginTemp DATE
	SET @BeginTemp = CAST(@BeginDate AS DATE)
	WHILE (@BeginTemp < @EndDate) -- populate with each date
	BEGIN
		INSERT INTO @DateList(InventoryDate) VALUES (CAST(@BeginTemp AS DATETIMEOFFSET(7)))
		SET @BeginTemp = DATEADD(DAY, 1, @BeginTemp)
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
	SELECT 
		OwnerID
		,[BeginAmt] = ISNULL(SUM(GrossNet),0)
	FROM @Ledger
	WHERE  (InventoryDate > @PriorCloseoutDate AND InventoryDate < @BeginDate) 
		AND [AliasName] NOT IN('24 Hour Closeout','Load Rack','Rotation') 
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
	DECLARE @Totals TABLE (
		OwnerID			  NVARCHAR(100),
		[Site]			  NVARCHAR(30),
		InventoryDate	  DATETIMEOFFSET(7),	
		[Begin Inventory] FLOAT,
		AliasName		  NVARCHAR(30),
		[TransAmt]		  FLOAT,
		[DetailTrans]	  FLOAT,
		[Book Inventory]  FLOAT,
		[SumTrans]		  FLOAT,
		[VolumeUnitIndex] INT,
		[VolumeDecimalPlaces] TINYINT	);			
				
	INSERT INTO @Totals
	SELECT 
		OwnerID
		,[Site]
		,d.InventoryDate
		,0 AS [Begin Inventory]
		,AliasName
		,GrossNet AS [TransAmt]
		,GrossNet AS [DetailTrans]
		,0 AS [Book Inventory]
		,0 AS [SumTrans]
		,@SiteGroupLevelVolumeUnitIndex AS VolumeUnitIndex, 
		@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
	FROM @Ledger l
	RIGHT JOIN @DateList d 
	ON l.InventoryDate = d.InventoryDate
	WHERE d.InventoryDate >= @BeginDate AND d.InventoryDate < @EndDate
	ORDER BY OwnerID,[Site],d.InventoryDate

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
		,d.InventoryDate
		,0 AS  [Begin Inventory]
		,0 AS  [Book Inventory]
		,0 AS  [TransAmt]
		,ISNULL(SUM(GrossNet), 0) AS  [DetailTrans]
		,0 AS  [24 Hr] 
		,0 AS  [Adjustment]
		,0 AS  [Bulk Issue]
		,0 AS  [Defuel]
		,0 AS  [Issue]
		,0 AS  [Load Rack]
		,0 AS  [LR Receipt]	
		,0 AS  [Receipt]	
		,0 AS  [Rotation]
		,0 AS  [Transfer]
		,@SiteGroupLevelVolumeUnitIndex AS VolumeUnitIndex, 
		@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
		,@ProductID	
	FROM @Ledger l  
	RIGHT JOIN  @DateList d ON
	l.InventoryDate = d.InventoryDate
	WHERE d.InventoryDate >= @BeginDate AND d.InventoryDate < @EndDate
	GROUP BY [OwnerID],[Site],d.InventoryDate,VolumeUnitIndex,VolumeDecimalPlaces
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
	DECLARE @PriorCloseoutVolueVar FLOAT
	SET @PriorCloseoutVolueVar = 
	(
		SELECT ISNULL
		(
			(SELECT PriorCloseoutVolume FROM @SiteList WHERE ManagerCompanyGuid = @Manager)
			,0.00
		)
	)

	IF @BeginTot IS NULL
		BEGIN 
			UPDATE @TotalSum
			SET [Begin Inventory] = @PriorCloseoutVolueVar
			FROM @TotalSum 
			WHERE DATEPART(day,InventoryDate) = 1 
		END
	ELSE
		BEGIN
			UPDATE @TotalSum
			SET [Begin Inventory] = @BeginTot + @PriorCloseoutVolueVar
			FROM @TotalSum 
			WHERE DATEPART(day,InventoryDate) = 1 
		END

	/****************
		END
		UPDATE
	 [Begin Inventory]
	******************/

	/********** 
		BEGIN
	Temp for filtering out the 
	excluded trans for Begin and Book
	Inventory
	************/
	DECLARE @ExTrans TABLE 
	(
		InventoryDate		DATETIMEOFFSET(7),
		[TransAmt]			FLOAT
	);		

	INSERT INTO @ExTrans 
	SELECT s.InventoryDate, ISNULL(SUM(t.TransAmt),0) AS TransAmt
	FROM @TotalSum s
	JOIN @Totals t 
	ON s.InventoryDate = t.InventoryDate
	WHERE [AliasName] NOT IN ('24 Hour Closeout','Load Rack','Rotation')
	GROUP BY s.InventoryDate
	ORDER BY s.InventoryDate

	/********** 
		END
	Temp for filtering out the 
	excluded trans for Begin and Book
	Inventory
	************/

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
	SET TransAmt = isnull(et.TransAmt,0)
	FROM @TotalSum b
	INNER JOIN @ExTrans et
	ON b.InventoryDate = et.InventoryDate

	UPDATE @TotalSum
	SET [24 Hr] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = '24 Hour Closeout'

	UPDATE @TotalSum
	SET [Adjustment] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Adjustment'

	UPDATE @TotalSum
	SET [Bulk Issue] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Bulk Issue'

	UPDATE @TotalSum
	SET [Defuel] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Defuel'

	UPDATE @TotalSum
	SET [Issue] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Issue'

	UPDATE @TotalSum
	SET [Load Rack] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Load Rack'

	UPDATE @TotalSum
	SET [LR Receipt] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'LR Receipt'

	UPDATE @TotalSum
	SET [Receipt] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Receipt'

	UPDATE @TotalSum
	SET [Rotation] = t.[DetailTrans] 
	FROM @TotalSum b
	INNER JOIN @Totals t
	ON b.InventoryDate = t.InventoryDate
	WHERE t.AliasName = 'Rotation'

	UPDATE @TotalSum
	SET [Transfer] = t.[DetailTrans] 
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
	DECLARE @TotalsInvDate DATETIMEOFFSET(7)
	DECLARE @TotalsBegin FLOAT
	DECLARE @TotalsTransAmt FLOAT
	DECLARE @TotalsBook FLOAT
	DECLARE @TempTotalDay FLOAT
	SET @TempTotalDay = (SELECT TOP 1 [Begin Inventory] FROM @TotalSum ORDER BY InventoryDate)
	SET @TempTotalDay = (SELECT ISNULL(@TempTotalDay, 0.00))

	DECLARE TotalsCursor CURSOR FAST_FORWARD FOR SELECT [OwnerID], InventoryDate, [Begin Inventory], TransAmt, [Book Inventory] FROM @TotalSum ORDER BY InventoryDate
	OPEN TotalsCursor
	FETCH NEXT FROM TotalsCursor INTO @TotalsOwner, @TotalsInvDate, @TotalsBegin, @TotalsTransAmt, @TotalsBook

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @TotalsTransAmt = (SELECT ISNULL(@TotalsTransAmt,0.00))
		SET @TempTotalDay = @TempTotalDay + @TotalsTransAmt

		UPDATE @TotalSum SET [Book Inventory] = (SELECT @TempTotalDay) WHERE InventoryDate = @TotalsInvDate 

		SET @TotalsBook = (SELECT ISNULL((SELECT TOP 1 [Book Inventory] FROM @TotalSum WHERE InventoryDate = @TotalsInvDate),0.00))

		UPDATE @TotalSum SET [Begin Inventory] = (SELECT @TotalsBook) WHERE InventoryDate = DATEADD(day,1,@TotalsInvDate)
		UPDATE @TotalSum SET [OwnerID] = (SELECT TOP 1 OwnerID FROM @Ledger)

		FETCH NEXT FROM TotalsCursor INTO @TotalsOwner, @TotalsInvDate, @TotalsBegin, @TotalsTransAmt, @TotalsBook
	END

	CLOSE TotalsCursor
	DEALLOCATE TotalsCursor

	if(@RemoveOwnersWithNoActivity = 1)
		BEGIN
		/****************
			BEGIN
			GROUPING 
		(Removes Owners that
		 have 0 Ledger for the
		 entire month)
		******************/

		Declare @GroupingTotalSums TABLE 
		(
			[OwnerID] NVARCHAR(100)
		);	

		INSERT INTO @GroupingTotalSums
		SELECT DISTINCT OwnerID
		FROM @TotalSum 
		WHERE 
		(
			[Begin Inventory] <> 0 OR [Book Inventory] <> 0 OR [24 Hr] <> 0 OR [Adjustment]<> 0 or
			[Bulk Issue] <> 0 OR [Defuel] <> 0 OR [Issue] <> 0 OR [Load Rack] <> 0 OR [LR Receipt] <> 0 OR 
			[Receipt] <> 0 OR [Rotation] <> 0 OR [Transfer]<> 0
		) 
		AND OwnerID <> ''
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
		END
	RETURN;

	/****************
		END
		Main Query
	******************/
END