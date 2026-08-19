


CREATE PROCEDURE [dbo].[usp_AllocationAmountLoaded]
@BeginDate DATE, @EndDate DATE, @ManagerID NVARCHAR (100), @OwnerID NVARCHAR (100), @ShipperID NVARCHAR (100), @BillToID NVARCHAR (100), @ShipToID NVARCHAR (100), @ItemGuid UNIQUEIDENTIFIER, @AllocationType TINYINT, @StationType TINYINT, @TransActionID NVARCHAR (50), @SiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @TransactionGuid UNIQUEIDENTIFIER
	DECLARE @VolumeUnits INT

	SET @TransactionGuid = (SELECT TransactionGuid
			FROM dbo.tblTransactions
		  WHERE TransID = @TransActionID)
	
	SET @VolumeUnits =
		(SELECT dbo.tblSites.VolumeUnitIndex
			FROM dbo.tblSites
		  WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @VolumeDecimalPlaces INT

	SET @VolumeDecimalPlaces =
		(SELECT dbo.tblSites.VolumeDecimalPlaces
			FROM dbo.tblSites
		  WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @Site nvarchar(30)

	SET @Site =
		(SELECT dbo.tblSites.ID
			FROM dbo.tblSites
		  WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @Products TABLE([ProductGuid] UNIQUEIDENTIFIER)
	
	-- Get Products in ProductGroup. 
	IF @AllocationType = 1
	BEGIN
		INSERT INTO @Products
		SELECT ProductGuid
		  FROM map.tblProductToProductGroup
		 WHERE AssignedToApplicationStringGuid = @ItemGuid
			
	END
	
	DECLARE @Transactions TABLE([TransID] [nvarchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, TransactionGuid UNIQUEIDENTIFIER null)

	INSERT INTO @Transactions
		SELECT TransID, TransactionGuid
		  FROM dbo.tblTransactions WITH(NOLOCK)
		 WHERE (tblTransactions.DeleteFlag IS NULL		OR dbo.tblTransactions.DeleteFlag   = 0)
			AND (tblTransactions.LookupTransTypeIndex    = 5		OR dbo.tblTransactions.LookupTransTypeIndex  = 6)
			AND (tblTransactions.Site           = @Site)
			AND (@ManagerID                     = ''		OR dbo.tblTransactions.ManagerID    = @ManagerID)
			AND (@OwnerID                       = ''		OR dbo.tblTransactions.OwnerID      = @OwnerID)
			AND (@ShipperID                     = ''		OR dbo.tblTransactions.ShipperID    = @ShipperID)
			AND (@BillToID                      = ''		OR dbo.tblTransactions.BillToID     = @BillToID)
			AND (@ShipToID                      = ''		OR dbo.tblTransactions.ShipToID     = @ShipToID)
			AND dbo.tblTransactions.InventoryDate  >= @BeginDate
			AND dbo.tblTransactions.InventoryDate   < @EndDate
			AND LookupTransactionStatusIndex              <> 7

	DECLARE @LineItems TABLE([NetQuantity] [float], [PresetAmount] [float], [LookupTransactionStatusIndex] [int], TransactionGuid UNIQUEIDENTIFIER null)

	INSERT INTO @LineItems
		SELECT - NetQuantity,
				 PresetAmount,
				 LookupTransactionStatusIndex,
				 TransactionGuid
		  FROM dbo.tblTransactionLineItems WITH(NOLOCK)
		 WHERE (tblTransactionLineItems.DeleteFlag IS NULL OR dbo.tblTransactionLineItems.DeleteFlag = 0)
			AND dbo.tblTransactionLineItems.LookupTransactionStatusIndex <> 7
			AND (@StationType <> 1	OR dbo.tblTransactionLineItems.LookupTransactionStatusIndex <> 9)
			AND (@TransactionGuid IS NULL OR dbo.tblTransactionLineItems.TransactionGuid <> @TransactionGuid)
			AND dbo.tblTransactionLineItems.TransactionGuid IN (SELECT TransactionGuid FROM @Transactions)
			AND (
					(@AllocationType = 0 AND dbo.tblTransactionLineItems.ProductGuid = @ItemGuid)
						OR
					(@AllocationType = 1 AND dbo.tblTransactionLineItems.ProductGuid IN (SELECT * FROM @Products))
						OR
					 @AllocationType = 2
				 )
				 
	IF @AllocationType = 0
		INSERT INTO @LineItems
			SELECT - NetQuantity,
					 PresetAmount,
					 LookupTransactionStatusIndex,
					 TransactionGuid
			  FROM dbo.tblTransactionSubLineItems WITH(NOLOCK)
			 WHERE (tblTransactionSubLineItems.DeleteFlag IS NULL OR dbo.tblTransactionSubLineItems.DeleteFlag         = 0)
				AND dbo.tblTransactionSubLineItems.LookupTransactionStatusIndex <> 7
				AND (@StationType <> 1 OR dbo.tblTransactionSubLineItems.LookupTransactionStatusIndex <> 9)
				AND dbo.tblTransactionSubLineItems.TransactionGuid IN (SELECT TransactionGuid FROM @Transactions)
				AND dbo.tblTransactionSubLineItems.ProductGuid = @ItemGuid

	SELECT SUM(dbo.udf_ConvertFromSIUnits(Volume, @VolumeUnits, @VolumeDecimalPlaces)) AS TotalVolume
	  FROM
		 (SELECT NetQuantity AS Volume
			 FROM @LineItems
			WHERE LookupTransactionStatusIndex = 0
				OR LookupTransactionStatusIndex = 11
				OR (
						LookupTransactionStatusIndex     <> 0
						AND LookupTransactionStatusIndex <> 11
						AND PresetAmount      <= NetQuantity
					)

			UNION ALL

			SELECT PresetAmount AS Volume
			  FROM @LineItems
			 WHERE LookupTransactionStatusIndex <> 0
				AND LookupTransactionStatusIndex <> 11
				AND PresetAmount       > NetQuantity
		 ) AS tbLineItems
END
