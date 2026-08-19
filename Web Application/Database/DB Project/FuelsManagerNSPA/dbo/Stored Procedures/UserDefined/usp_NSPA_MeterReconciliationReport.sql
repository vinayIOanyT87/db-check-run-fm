CREATE PROCEDURE dbo.usp_NSPA_MeterReconciliationReport (
	@ViewingSiteID NVARCHAR(60)
	, @InventoryDate DATETIME
	)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @VolumeUnits INT

	SET @VolumeUnits = (
			SELECT tblSites.VolumeUnitIndex
			FROM tblSites
			WHERE tblSites.ID = @ViewingSiteID
			)

	DECLARE @VolumeDecimalPlaces INT

	SET @VolumeDecimalPlaces = (
			SELECT tblSites.VolumeDecimalPlaces
			FROM tblSites
			WHERE tblSites.ID = @ViewingSiteID
			)

	DECLARE @SiteTable TABLE (SiteID NVARCHAR(60))

	INSERT INTO @SiteTable
	SELECT *
	FROM dbo.udp_SitesFromSiteGroup(@ViewingSiteID)

	DECLARE @ResultTable TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, MeterThroughput FLOAT
		, GaugedThroughput FLOAT
		, GaugedVariance FLOAT
		, TransThroughput FLOAT
		, TransVariance FLOAT
		, AllowableVariance FLOAT
		,
		-- status to be calculated by report
		GaugedError BIT
		)
	DECLARE @TransTable TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, Throughput FLOAT
		)
	DECLARE @GaugedTable TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, Throughput FLOAT
		, Error BIT -- when there is a missing physical inventory in the previous day
		)
	DECLARE @MeterTable TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, Throughput FLOAT
		)

	-- get the three types of throughputs
	INSERT INTO @MeterTable
	SELECT *
	FROM dbo.udp_NSPA_MeterReconciliation_MeterThroughput(@ViewingSiteID, @InventoryDate)

	INSERT INTO @GaugedTable
	SELECT *
	FROM dbo.udp_NSPA_MeterReconciliation_GaugedThroughput(@ViewingSiteID, @InventoryDate)

	INSERT INTO @TransTable
	SELECT *
	FROM dbo.udp_NSPA_MeterReconciliation_TransThroughput(@ViewingSiteID, @InventoryDate)

	-- populate site, manager and product information in result table
	DECLARE @RecordTable TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		)

	INSERT INTO @RecordTable
	SELECT SiteID
		, ManagerID
		, Product
	FROM @MeterTable

	INSERT INTO @RecordTable
	SELECT SiteID
		, ManagerID
		, Product
	FROM @GaugedTable

	INSERT INTO @RecordTable
	SELECT SiteID
		, ManagerID
		, Product
	FROM @TransTable

	INSERT INTO @ResultTable
	SELECT DISTINCT *
		, 0
		, 0
		, 0
		, 0
		, 0
		, 0
		, 0
	FROM @RecordTable

	-- populate throughput values in result table
	UPDATE r
	SET MeterThroughput = ISNULL((
				SELECT TOP 1 Throughput
				FROM @MeterTable
				WHERE SiteID = r.SiteID
					AND ManagerID = r.ManagerID
					AND Product = r.Product
				), 0)
		, GaugedThroughput = ISNULL((
				SELECT TOP 1 Throughput
				FROM @GaugedTable
				WHERE SiteID = r.SiteID
					AND ManagerID = r.ManagerID
					AND Product = r.Product
					AND Error = 0
				), 0)
		, TransThroughput = ISNULL((
				SELECT TOP 1 Throughput
				FROM @TransTable
				WHERE SiteID = r.SiteID
					AND ManagerID = r.ManagerID
					AND Product = r.Product
				), 0)
		, GaugedError = ISNULL((
				SELECT 1
				FROM @GaugedTable
				WHERE SiteID = r.SiteID
					AND ManagerID = r.ManagerID
					AND Product = r.Product
					AND Error = 1
				), 0)
	FROM @ResultTable r

	-- set allowable variance
	UPDATE r
	SET r.AllowableVariance = (p.VarianceTolerance / 100) * r.MeterThroughput -- variance tolerance is a percent, so divide it by 100
	FROM @ResultTable r
	INNER JOIN tblProducts p
		ON r.Product = p.ProductID

	-- adjust for site precision
	UPDATE @ResultTable
	SET GaugedThroughput = dbo.udf_ConvertFromSIUnits(GaugedThroughput, @VolumeUnits, @VolumeDecimalPlaces)
		, TransThroughput = dbo.udf_ConvertFromSIUnits(TransThroughput, @VolumeUnits, @VolumeDecimalPlaces)
		, AllowableVariance = dbo.udf_ConvertFromSIUnits(AllowableVariance, @VolumeUnits, @VolumeDecimalPlaces)
		, MeterThroughput = dbo.udf_ConvertFromSIUnits(MeterThroughput, @VolumeUnits, @VolumeDecimalPlaces)

	-- calculate variances
	UPDATE @ResultTable
	SET GaugedVariance = (
			CASE GaugedError
				WHEN 1
					THEN 0
				ELSE MeterThroughput - GaugedThroughput
				END
			)
		, TransVariance = MeterThroughput - TransThroughput

	-- return results to report
	SELECT *
	FROM @ResultTable
END
