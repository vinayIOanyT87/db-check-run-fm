CREATE PROCEDURE dbo.NSPA_ConsumptionReport_Filterable (
	@ViewingSiteID NVARCHAR(60)
	, @SiteID NVARCHAR(60)
	, @Period DATETIME
	, @EntityType INT -- 0 category, 1 bill-to
	)
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON

		DECLARE @VolumeUnits INT
		DECLARE @VolumeDecimalPlaces INT

		SELECT @VolumeUnits = tblSites.VolumeUnitIndex
			, @VolumeDecimalPlaces = tblSites.VolumeDecimalPlaces
		FROM tblSites
		WHERE tblSites.ID = @ViewingSiteID

		DECLARE @ResultTable TABLE (
			EntityName NVARCHAR(60)
			, ProductName NVARCHAR(60)
			, NetQuantity FLOAT
			, ProductAggregate FLOAT -- total, for aiding sorting (faster done on sp)
			, Top5 BIT
			)
		DECLARE @SalesTable TABLE (
			BillToID NVARCHAR(60)
			, Product NVARCHAR(60)
			, EquipmentType NVARCHAR(60)
			, NetQuantity FLOAT
			)
		DECLARE @Top7ProductTable TABLE (
			ProductName NVARCHAR(60)
			, NetQuantity FLOAT
		)

		INSERT INTO @SalesTable
		SELECT t.BillToID
			, l.Product
			, t.DestinationEquipmentType1
			, dbo.udf_ConvertFromSIUnits(ISNULL(l.NetQuantity, 0), @VolumeUnits, @VolumeDecimalPlaces)
		FROM tblTransactions t
		INNER JOIN tblTransactionLineItems l
			ON t.TransactionGuid = l.TransactionGuid
		WHERE ISNULL(t.DeleteFlag, 0) = 0
			AND [Site] = @SiteID
			AND MONTH(t.InventoryDate) = MONTH(@Period)
			AND YEAR(t.InventoryDate) = YEAR(@Period)
			AND AliasName IN ('Retail Sale', 'Delivery Sale', 'Third Party Sale')
			AND (t.ReversalType IS NULL OR t.ReversalType IN ('', 'U'))

		IF @EntityType = 1 -- bill to
		BEGIN
			INSERT INTO @SalesTable
			SELECT ''
				, p.ProductId
				, ''
				, 0
			FROM (
				SELECT ProductID
				FROM vw_ProductGroupProducts
				WHERE ProductGroupID = 'Fuel Products'
					AND ProductID NOT IN (
						SELECT Product
						FROM @SalesTable
						)
				) p
				ORDER BY p.ProductID

			INSERT INTO @Top7ProductTable
			SELECT TOP 7 Product, SUM(NetQuantity)
					FROM @SalesTable
					GROUP BY Product
					ORDER BY SUM(NetQuantity)

			INSERT INTO @ResultTable
			SELECT BillToId
				, Product
				, SUM(NetQuantity)
				, 0
				, 0
			FROM @SalesTable
			WHERE BillToID IS NOT NULL
				AND LEN(BillToID) > 0
				AND Product IN (
					SELECT Product 
					FROM @Top7ProductTable
					)
			GROUP BY BillToId
				, Product

		END
		ELSE IF @EntityType = 0 -- category
		BEGIN

			INSERT INTO @ResultTable
			SELECT 'Aviation'
				, Product
				, SUM(NetQuantity)
				, 0
				, 0
			FROM @SalesTable
			WHERE EquipmentType IN ('Aircraft', 'Aviation')
			GROUP BY Product

			INSERT INTO @ResultTable
			SELECT 'Bulk'
				, Product
				, SUM(NetQuantity)
				, 0
				, 0
			FROM @SalesTable
			WHERE EquipmentType IN ('Tanker', 'Trailer', 'Railcar', 'Barge', 'Bulk')
			GROUP BY Product

			INSERT INTO @ResultTable
			SELECT 'Vehicle'
				, Product
				, SUM(NetQuantity)
				, 0
				, 0
			FROM @SalesTable
			WHERE EquipmentType = 'Vehicle'
			GROUP BY Product

			INSERT INTO @ResultTable
			SELECT 'Jerry Can'
				, Product
				, SUM(NetQuantity)
				, 0
				, 0
			FROM @SalesTable
			WHERE EquipmentType = 'Container'
			GROUP BY Product

			INSERT INTO @ResultTable
			SELECT 'Infrastructure'
				, Product
				, SUM(NetQuantity)
				, 0
				, 0
			FROM @SalesTable
			WHERE EquipmentType = 'Infrastructure'
			GROUP BY Product
		END

		UPDATE r
		SET ProductAggregate = ISNULL((
					SELECT SUM(NetQuantity)
					FROM @ResultTable
					WHERE ProductName = r.ProductName
					), 0)
		FROM @ResultTable r

		-- only keep the top 7 aggregates
		DECLARE @AggregateTable TABLE (
			ProductAggregate FLOAT
			, Product NVARCHAR(60)
			)

		INSERT INTO @AggregateTable
		SELECT DISTINCT TOP 7 ProductAggregate
			, ProductName
		FROM @ResultTable
		ORDER BY ProductAggregate DESC

		-- only graph the top 5 aggregates
		DECLARE @Top5Table TABLE (
			EntityName NVARCHAR(60)
			, TotalNetQuantity FLOAT
			)

		INSERT INTO @Top5Table
		SELECT TOP 5 EntityName
			, SUM(NetQuantity)
		FROM @ResultTable
		GROUP BY EntityName
		ORDER BY 2

		UPDATE @ResultTable
		SET Top5 = 1
		WHERE EntityName IN (
				SELECT EntityName
				FROM @Top5Table
				)

		UPDATE @ResultTable
		SET EntityName='NO_ENTITY_NAME'
		WHERE EntityName IS NULL OR EntityName = ''

		SELECT *
		FROM @ResultTable
		WHERE ProductName IN (
				SELECT Product
				FROM @AggregateTable
				)
		ORDER BY ProductAggregate DESC
	END TRY

	BEGIN CATCH
		DECLARE @_ErrMessage NVARCHAR(2048)
			, @_ErrNumber INT
			, @_ErrProcName NVARCHAR(126)
			, @_ErrLineNumber INT;

		SET @_ErrMessage = ERROR_MESSAGE();
		SET @_ErrNumber = ERROR_NUMBER();
		SET @_ErrProcName = ERROR_PROCEDURE();
		SET @_ErrLineNumber = ERROR_LINE();
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10) + 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13) + CHAR(10) + 'Procedure Name: [dbo].[NSPA_ConsumptionReport_Filterable] ' + CHAR(13) + CHAR(10) + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)), '') + CHAR(13) + CHAR(10);

		RAISERROR (
				@_ErrMessage
				, 16
				, 1
				);
	END CATCH
END