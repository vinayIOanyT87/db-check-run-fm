


CREATE PROCEDURE [dbo].[usp_GetTotalProductVolume]
@SingleOwner BIT, @InventoryDate DATE, @Site NVARCHAR (30), @Product NVARCHAR (30), @Manager NVARCHAR (100), @LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER
AS
BEGIN
		SET NOCOUNT ON
		DECLARE @TotalGrossVolume FLOAT;
		DECLARE @TotalNetVolume FLOAT;
		DECLARE @GrossVolume FLOAT;
		DECLARE @NetVolume FLOAT;
		DECLARE @LastPhysicalDate DATE;
		DECLARE @CompanyTable TABLE(CompanyID NVARCHAR(100)) ;
		SET @LastPhysicalDate = @InventoryDate;

		INSERT INTO @CompanyTable
			SELECT *
			  FROM dbo.udf_AuthorizedCompanies(@LoginSiteGuid, @SiteGuid, @UserGuid) ;
		
		SET LOCK_TIMEOUT 25000
		
		DECLARE @AliasTable TABLE(AliasName NVARCHAR(30)) ;
		
		INSERT INTO @AliasTable
			SELECT AliasName
			  FROM dbo.udf_AliasList(@SiteGuid)
			 ORDER BY AliasName

		-- Get volume based on last physical inventory, if it is a single owner system, 
		-- or close out, if it is a multiple owner system. 
		IF @SingleOwner = 1
		BEGIN
			-- Get date of last physical inventory. 
			SELECT @LastPhysicalDate = MAX(inventorydate)
			  FROM dbo.tblTransactions							AS t
			  LEFT OUTER JOIN dbo.tblTransactionLineItems AS l
				 ON t.TransactionGuid     = l.TransactionGuid
			 WHERE t.ManagerID       = @Manager
				AND t.Site           = @Site
				AND t.InventoryDate <= @InventoryDate
				AND t.DeleteFlag     = CAST(0 AS BIT)
				AND t.LookupTransTypeIndex    = 14
				AND EXISTS
						(SELECT CompanyID
							FROM @CompanyTable
						  WHERE CompanyID IN (t.ShipToID, t.SupplierID, t.ShipperID, t.OwnerID, t.ManagerID, t.CarrierID, t.BillToID))
				AND t.AliasName IN
						(SELECT AliasName
							FROM @AliasTable)
				AND l.Product                  = @Product
				AND l.TransactionInventoryDate = t.InventoryDate
				AND ISNULL(l.LookupQualityIndex, 1)       = 1

			-- Get total volume of last physical inventory. 
			SELECT @TotalGrossVolume = SUM(ISNULL(GrossQuantity, 0)),
					 @TotalNetVolume   = SUM(ISNULL(NetQuantity, 0))
			  FROM dbo.tblTransactions							AS t
			  LEFT OUTER JOIN dbo.tblTransactionLineItems AS l
				 ON t.TransactionGuid    = l.TransactionGuid
			 WHERE t.ManagerID      = @Manager
				AND t.Site          = @Site
				AND t.InventoryDate = @LastPhysicalDate
				AND t.DeleteFlag    = CAST(0 AS BIT)
				AND t.LookupTransTypeIndex   = 14
				AND EXISTS
					(SELECT CompanyID
						FROM @CompanyTable
					  WHERE CompanyID IN (t.ShipToID, t.SupplierID, t.ShipperID, t.OwnerID, t.ManagerID, t.CarrierID, t.BillToID))
				AND t.AliasName IN
					 (SELECT AliasName
						 FROM @AliasTable)
				AND l.Product                  = @Product
				AND l.TransactionInventoryDate = t.InventoryDate
				AND ISNULL(l.LookupQualityIndex, 1)       = 1
		END
		ELSE
		BEGIN
			-- Get date of last close out. 
			SELECT @LastPhysicalDate = MAX(CloseoutDate)
			  FROM dbo.tblOwnerCloseout
			 WHERE CloseoutDate <= @InventoryDate
				AND ManagerName  = @Manager
				AND ProductName  = @Product
				AND Site         = @Site
				
			-- Get total volume of last close out
			SELECT @TotalGrossVolume = SUM(ISNULL(GrossBookInventory, 0)),
					 @TotalNetVolume   = SUM(ISNULL(NetBookInventory, 0))
			  FROM dbo.tblOwnerCloseout
			 WHERE CloseoutDate = @LastPhysicalDate
				AND ManagerName  = @Manager
				AND ProductName  = @Product
				AND Site         = @Site
		END
		
		-- Sum up all the transactions for quantity following last physical inventory, 
		-- if single owner system, or close out, if multiple owner system. 
		SELECT @GrossVolume = SUM(ISNULL(GrossQuantity, 0)),
				 @NetVolume   = SUM(ISNULL(NetQuantity, 0))
		 FROM
		 (
			(SELECT CONVERT(CHAR(10), t.InventoryDate, 111) AS InventoryDate,
					  t.AliasName,
					  l.GrossQuantity AS GrossQuantity,
					  l.ProductPrice,
					  l.NetQuantity AS NetQuantity,
					  t.Site,
					  LookupTransTypeIndex
				FROM dbo.tblTransactions t
				LEFT OUTER JOIN dbo.tblTransactionLineItems l
				  ON t.TransactionGuid = l.TransactionGuid
			  WHERE t.ManagerID      = @Manager
				 AND t.Site           = @Site
				 AND t.InventoryDate  <= @InventoryDate
				 AND t.InventoryDate  > @LastPhysicalDate
				 AND t.DeleteFlag     = CAST(0 AS BIT)
				 AND t.LookupTransTypeIndex    IN (1, 3, 5, 7, 8, 10, 13, 15)
				 AND EXISTS
						(SELECT CompanyID
							FROM @CompanyTable
						  WHERE CompanyID IN (t.ShipToID, t.SupplierID, t.ShipperID, t.OwnerID, t.ManagerID, t.CarrierID, t.BillToID))
				 AND t.AliasName IN
							(SELECT AliasName
								FROM @AliasTable)
				 AND l.Product                  = @Product
				 AND l.TransactionInventoryDate = t.InventoryDate
				 AND ISNULL(l.LookupQualityIndex, 1)       = 1
			)

			UNION ALL
			
			(SELECT CONVERT(CHAR(10), t.InventoryDate, 111) AS InventoryDate,
					  t.AliasName,
					  l.GrossQuantity AS GrossQuantity,
					  0.0             AS ProductPrice,
					  l.NetQuantity   AS NetQuantity,
					  t.Site,
					  LookupTransTypeIndex
			  FROM dbo.tblTransactions								AS t
			  LEFT OUTER JOIN dbo.tblTransactionSubLineItems AS l
			  ON t.TransactionGuid = l.TransactionGuid
			 WHERE ((@Manager    = '')
				 OR (@Manager IS NULL)
				 OR (t.ManagerID     = @Manager))
				AND t.Site           = @Site
				AND t.InventoryDate <= @InventoryDate
				AND t.InventoryDate  > @LastPhysicalDate
				AND t.DeleteFlag     = CAST(0 AS BIT)
				AND t.LookupTransTypeIndex   IN (1, 3, 5, 7, 8, 10, 13, 15)
				AND EXISTS
						(SELECT CompanyID
							FROM @CompanyTable
						  WHERE CompanyID IN (t.ShipToID, t.SupplierID, t.ShipperID, t.OwnerID, t.ManagerID, t.CarrierID, t.BillToID))
				AND t.AliasName IN
						(SELECT AliasName
							FROM @AliasTable)
				AND l.Product            = @Product
				AND ISNULL(l.LookupQualityIndex, 1) = 1)
			) AS UnionTable

			-- Return total reserves for given product and manager. 
			SELECT (ISNULL(@TotalGrossVolume, 0) + ISNULL(@GrossVolume, 0))	AS TotalGrossVolume,
					 (ISNULL(@TotalNetVolume,  0)  + ISNULL(@NetVolume,   0))	AS TotalNetVolume,
							 @LastPhysicalDate												AS LastPhysicalDate

	END
