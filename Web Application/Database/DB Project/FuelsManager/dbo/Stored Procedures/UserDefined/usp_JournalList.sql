

CREATE PROCEDURE [dbo].[usp_JournalList]
@BeginDate DATE, @EndDate DATE, @Manager NVARCHAR (100), @Owner NVARCHAR (100), @Product NVARCHAR (30), @LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @Gross BIT
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @VolumeUnits INT

	SET @VolumeUnits =
		(SELECT dbo.tblSites.VolumeUnitIndex
			FROM dbo.tblSites
		  WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @VolumeDecimalPlaces INT

	SET @VolumeDecimalPlaces =
		(SELECT dbo.tblSites.VolumeDecimalPlaces
			FROM dbo.tblSites
		  WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @AuthorizedCompanies TABLE(Company NVARCHAR(100))

	INSERT INTO @AuthorizedCompanies
		SELECT *
		  FROM udf_AuthorizedCompanies(@LoginSiteGuid, @SiteGuid, @UserGuid)

	DECLARE @SiteList TABLE
	(
		Site NVARCHAR(50),
		PriorCloseoutDate DATE,
		PriorCloseoutGrossVolume FLOAT,
		PriorCloseoutNetVolume FLOAT
	)

	INSERT INTO @SiteList
		SELECT dbo.tblSites.ID,
				 '1901-01-01',
				 0,
				 0
		  FROM [map].[tblSiteToSite],
				 dbo.tblSites
		 WHERE ParentSiteGuid        = @SiteGuid
			AND ChildSiteGuid         = dbo.tblSites.SiteGuid
			AND dbo.tblSites.SiteGroupFlag = 0

	UPDATE @SiteList
	   SET PriorCloseoutDate        = tOC1.CloseoutDate,
			 PriorCloseoutGrossVolume = tOC1.GrossBookInventory,
			 PriorCloseoutNetVolume   = tOC1.NetBookInventory
	  FROM @SiteList			AS tSL,
			 dbo.tblOwnerCloseout AS tOC1
	 WHERE tSL.Site          = tOC1.Site
		AND tOC1.ManagerName  = @Manager
		AND tOC1.OwnerName    = @Owner
		AND tOC1.ProductName  = @Product
		AND tOC1.CloseoutDate =
				(SELECT MAX(CloseoutDate)
					FROM dbo.tblOwnerCloseout AS tOC2
				  WHERE tSL.Site          = tOC2.Site
					 AND tOC2.ManagerName  = @Manager
					 AND tOC2.OwnerName    = @Owner
					 AND tOC2.ProductName  = @Product
					 AND tOC2.CloseoutDate < @BeginDate)

	-- Get the Transaction Aliases. 
	DECLARE @AliasList TABLE(AliasName NVARCHAR(30), LookupTransTypeIndex INT)

	INSERT INTO @AliasList
		SELECT AliasName,
				 LookupTransTypeIndex
		  FROM dbo.udf_AliasList(@SiteGuid)
		 WHERE LookupTransTypeIndex NOT IN (7, 9, 10, 11, 14, 17)
		 ORDER BY AliasName

	-- Create the Journal Table Name. 
	DECLARE @TableName NVARCHAR(40)
	SELECT @TableName = 'Journal_' + CONVERT(NVARCHAR, GETDATE(), 126)

	SET @TableName = (SELECT REPLACE(@TableName, '-', ''))
	SET @TableName = (SELECT REPLACE(@TableName, ':', ''))
	SET @TableName = (SELECT REPLACE(@TableName, '.', ''))
	
	-- Create the Journal Table. 
	DECLARE @CreateJournalString NVARCHAR(2000)
	SET @CreateJournalString = 'CREATE TABLE ' + @TableName + ' ([Inventory Date] DATE, [Begin Inventory] FLOAT, [Book Inventory] FLOAT'

	DECLARE @InitializationString NVARCHAR(1000)
	SET @InitializationString = ' DECLARE @InventoryDate DATE' + ' SELECT @InventoryDate = ''' + CONVERT(NVARCHAR, @BeginDate, 101) + '''' + ' DECLARE @EndDate DATE' + ' SELECT @EndDate = ''' + CONVERT(NVARCHAR, @EndDate, 101) + '''' + ' WHILE (@InventoryDate < @EndDate)' + ' BEGIN' + '  INSERT INTO ' + @TableName + ' ([Inventory Date], [Begin Inventory], [Book Inventory]'
	
	DECLARE @InitialValuesString NVARCHAR(1000)
	SET @InitialValuesString = '@InventoryDate,0,0'

	DECLARE AliasCursor CURSOR FOR
		SELECT * FROM @AliasList

	DECLARE @AliasName NVARCHAR(32)
	DECLARE @LookupTransTypeIndex INT

	OPEN AliasCursor

	FETCH NEXT
	 FROM AliasCursor
	 INTO @AliasName,
			@LookupTransTypeIndex

	SET @AliasName = '[' + @AliasName + ']'
	
	WHILE @@FETCH_STATUS = 0	-- Success. 
	BEGIN
		SELECT @CreateJournalString  = @CreateJournalString  + ',' + @AliasName + ' FLOAT'
		SELECT @InitializationString = @InitializationString + ',' + @AliasName
		SELECT @InitialValuesString  = @InitialValuesString  + ',0.0'
		
		FETCH NEXT
		 FROM AliasCursor
		 INTO @AliasName,
				@LookupTransTypeIndex

		SET @AliasName = '[' + @AliasName + ']'
	END

	SET @InitializationString = @InitializationString + ') VALUES (' + @InitialValuesString + ')' + ' SELECT @InventoryDate = DATEADD(day,1,@InventoryDate)' + ' END'
	SET @CreateJournalString  = @CreateJournalString  + ')'

	EXEC(@CreateJournalString)
	EXEC(@InitializationString)
	CLOSE AliasCursor

	-- Update the Journal  
	DECLARE SiteCursor CURSOR FOR
		SELECT * FROM @SiteList
		
	DECLARE @Site NVARCHAR(50)
	DECLARE @PriorCloseoutDate DATE
	DECLARE @PriorCloseoutGrossVolume FLOAT
	DECLARE @PriorCloseoutNetVolume FLOAT
	DECLARE @LedgerList TABLE(InventoryDate DATE, AliasName NVARCHAR(32), GrossQuantity FLOAT, NetQuantity FLOAT)
	DECLARE @LedgerInventoryDate DATE
	DECLARE @LedgerAliasName NVARCHAR(32)
	DECLARE @LedgerGrossQuantity FLOAT DECLARE @LedgerNetQuantity FLOAT

	DECLARE LedgerCursor CURSOR FOR
		SELECT * FROM @LedgerList
		
	-- Iterate for each Site. 
	DECLARE @UpdateString NVARCHAR(1000)

	OPEN SiteCursor

	FETCH NEXT
	 FROM SiteCursor
	 INTO @Site,
			@PriorCloseoutDate,
			@PriorCloseoutGrossVolume,
			@PriorCloseoutNetVolume

	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @InventoryDate DATE
		SELECT @InventoryDate = @PriorCloseoutDate

		DECLARE @SiteBookInventory FLOAT
		
		IF (@Gross = 1)
			SELECT @SiteBookInventory = dbo.udf_ConvertFromSIUnits(@PriorCloseoutGrossVolume, @VolumeUnits, @VolumeDecimalPlaces)
		ELSE
			SELECT @SiteBookInventory = dbo.udf_ConvertFromSIUnits(@PriorCloseoutNetVolume, @VolumeUnits, @VolumeDecimalPlaces)

		INSERT INTO @LedgerList
			SELECT InventoryDate,
					 AliasName,
					 SUM(dbo.udf_ConvertFromSIUnits(GrossQuantity, @VolumeUnits, @VolumeDecimalPlaces)),
					 SUM(dbo.udf_ConvertFromSIUnits(NetQuantity,   @VolumeUnits, @VolumeDecimalPlaces))
			  FROM
			  (
					(SELECT t.InventoryDate,
							  t.AliasName,
							  l.GrossQuantity,
							  l.NetQuantity
					   FROM dbo.tblTransactionLineItems		AS l
						LEFT OUTER JOIN dbo.tblTransactions	AS t
						  ON l.TransactionGuid        = t.TransactionGuid
					  WHERE l.Product        = @Product
						 AND l.TransactionGuid        = t.TransactionGuid
						 AND t.Site           = @Site
						 AND t.ManagerID      = @Manager
						 AND t.InventoryDate <= @EndDate
						 AND t.InventoryDate  > @PriorCloseoutDate
						 AND t.DeleteFlag     = CAST(0 AS BIT)
						 AND t.OwnerID        = @Owner
						 AND EXISTS
								(SELECT Company
									FROM @AuthorizedCompanies
								  WHERE Company IN (t.ShipToID, t.SupplierID, t.ShipperID, t.OwnerID, t.ManagerID, t.CarrierID, t.BillToID))
						 AND t.AliasName IN
								(SELECT AliasName
									FROM @AliasList)
					)

					UNION ALL
		
					(SELECT InventoryDate,
							  t.AliasName,
							  l.GrossQuantity,
							  l.NetQuantity
						FROM dbo.tblTransactionSubLineItems AS l
						LEFT OUTER JOIN dbo.tblTransactions AS t
						  ON l.TransactionGuid       = t.TransactionGuid
					  WHERE l.Product        = @Product
						 AND l.TransactionGuid     = t.TransactionGuid
						 AND t.Site           = @Site
						 AND t.ManagerID      = @Manager
						 AND t.InventoryDate <= @EndDate
						 AND t.InventoryDate  > @PriorCloseoutDate
						 AND t.DeleteFlag     = CAST(0 AS BIT)
						 AND t.OwnerID        = @Owner
						 AND EXISTS
								(SELECT Company
									FROM @AuthorizedCompanies
								  WHERE Company IN (t.ShipToID, t.SupplierID, t.ShipperID, t.OwnerID, t.ManagerID, t.CarrierID, t.BillToID))
						 AND t.AliasName IN
								 (SELECT AliasName
									 FROM @AliasList)
						)
					) AS UNIONTABLE
					
			 GROUP BY InventoryDate,
						 AliasName
			 ORDER BY InventoryDate,
						 AliasName

		OPEN LedgerCursor

		FETCH NEXT
		 FROM LedgerCursor
		 INTO @LedgerInventoryDate,
				@LedgerAliasName,
				@LedgerGrossQuantity,
				@LedgerNetQuantity
		
		SET @InventoryDate = ISNULL(@LedgerInventoryDate, @EndDate)
		
		WHILE(@InventoryDate < @EndDate)
		BEGIN
			IF(@InventoryDate >= @BeginDate)
			BEGIN
				SET @UpdateString = 'UPDATE ' + @TableName + ' SET [Begin Inventory] = [Begin Inventory]+' + STR(@SiteBookInventory) + ', [Book Inventory] = [Book Inventory]+' + STR(@SiteBookInventory) + '  WHERE [Inventory Date] = ''' + CONVERT(NVARCHAR, @InventoryDate, 101) + ''''
				EXEC(@UpdateString)
			END
			
			OPEN AliasCursor
			
			FETCH NEXT
			 FROM AliasCursor
			 INTO @AliasName,
					@LookupTransTypeIndex

			WHILE @@FETCH_STATUS = 0
			BEGIN
				IF (@InventoryDate = @LedgerInventoryDate AND @AliasName = @LedgerAliasName)
					BEGIN
						DECLARE @AliasInventory FLOAT

						IF (@Gross = 1)
							SELECT @AliasInventory = @LedgerGrossQuantity
						ELSE
							SELECT @AliasInventory = @LedgerNetQuantity
							
						IF (@InventoryDate >= @BeginDate)
						BEGIN
							SET @AliasName   = '[' + @AliasName + ']'

							SET @UpdateString = 'UPDATE ' + @TableName + ' SET ' + @AliasName + ' = ' + @AliasName + '+' + STR(@AliasInventory) + '  WHERE [Inventory Date] = ''' + CONVERT(NVARCHAR, @InventoryDate, 101) + ''''
							EXEC(@UpdateString)

							IF(@LookupTransTypeIndex <> 12)
							BEGIN
								SET @UpdateString = 'UPDATE ' + @TableName + ' SET [Book Inventory] = [Book Inventory]+' + STR(@AliasInventory) + '  WHERE [Inventory Date] = ''' + CONVERT(NVARCHAR, @InventoryDate, 101) + ''''
								EXEC(@UpdateString)
							END
						END

						IF (@LookupTransTypeIndex <> 12)
							SELECT @SiteBookInventory = @SiteBookInventory + @AliasInventory
							
						FETCH NEXT
						 FROM LedgerCursor
						 INTO @LedgerInventoryDate,
								@LedgerAliasName,
								@LedgerGrossQuantity,
								@LedgerNetQuantity
					END

				FETCH NEXT
				 FROM AliasCursor
				 INTO @AliasName,
						@LookupTransTypeIndex
			END

			CLOSE AliasCursor

			SELECT @InventoryDate = DATEADD(DAY, 1, @InventoryDate)
		END

		CLOSE LedgerCursor

		DELETE FROM @LedgerList
		 FETCH NEXT
		  FROM SiteCursor
		  INTO @Site,
				 @PriorCloseoutDate,
				 @PriorCloseoutGrossVolume,
				 @PriorCloseoutNetVolume
	END
	
	CLOSE SiteCursor
	DEALLOCATE SiteCursor
	
	-- Final Query. 
	DECLARE @QueryString NVARCHAR(100)
	SET @QueryString = 'SELECT * FROM ' + @TableName
	EXEC(@QueryString)
	
	-- Drop the Journal. 
	DECLARE @DropString NVARCHAR(100)
	SET @DropString = 'DROP TABLE ' + @TableName
	EXEC(@DropString)
END
