CREATE FUNCTION dbo.udp_NSPA_MeterReconciliation_MeterThroughput (
	@SiteID NVARCHAR(60)
	, @InventoryDate DATETIME
)
RETURNS @ResultTable TABLE (
	SiteID NVARCHAR(60)
	, ManagerID NVARCHAR(60)
	, Product NVARCHAR(60)
	, Throughput FLOAT
)
AS
BEGIN
	DECLARE @SiteTable TABLE (SiteID NVARCHAR(60))

	INSERT INTO @SiteTable
	SELECT *
	FROM dbo.udp_SitesFromSiteGroup(@SiteID)

	DECLARE @ReceiptData TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, MeterStop FLOAT
		, MeterStart FLOAT
)
	DECLARE @IssueData TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, MeterStop FLOAT
		, MeterStart FLOAT
)
	DECLARE @RecirculationData TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, Recirculation FLOAT
		)

	INSERT INTO @RecirculationData
	SELECT t.[Site]
		, t.ManagerID
		, l.Product
		, SUM(dbo.udf_ConvertToSIUnits(ISNULL(l.MeterStop, 0), s.VolumeUnitIndex)) - SUM(dbo.udf_ConvertToSIUnits(ISNULL(l.MeterStart, 0), s.VolumeUnitIndex))
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems l
		ON t.TransactionGuid = l.TransactionGuid
	INNER JOIN tblSites s
		ON t.SiteGuid = s.SiteGuid
	WHERE t.DeleteFlag = 0
		AND t.AliasName = 'Meter Rotation'
		AND t.[Site] IN (
			SELECT *
			FROM @SiteTable
)
		AND t.InventoryDate = @InventoryDate
	GROUP BY t.[Site]
		, t.ManagerID
		, l.Product

	INSERT INTO @ReceiptData
	SELECT t.[Site]
		, t.ManagerID
		, l.Product
		,
		-- meter values are stored at site UOM, need to convert it to SI
		SUM(dbo.udf_ConvertToSIUnits(ISNULL(l.MeterStop, 0), s.VolumeUnitIndex))
		, SUM(dbo.udf_ConvertToSIUnits(ISNULL(l.MeterStart, 0), s.VolumeUnitIndex))
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems l
		ON t.TransactionGuid = l.TransactionGuid
	INNER JOIN tblSites s
		ON t.SiteGuid = s.SiteGuid
	WHERE t.DeleteFlag = 0
		AND t.AliasName IN ('Receipt', 'Defuel')
		AND t.[Site] IN (
			SELECT *
			FROM @SiteTable
			)
		AND t.InventoryDate = @InventoryDate
	GROUP BY t.[Site]
		, t.ManagerID
		, l.Product

	INSERT INTO @IssueData
	SELECT t.[Site]
		, t.ManagerID
		, l.Product
		,
		-- meter values are stored at site UOM, need to convert it to SI
		SUM(dbo.udf_ConvertToSIUnits(ISNULL(l.MeterStop, 0), s.VolumeUnitIndex))
		, SUM(dbo.udf_ConvertToSIUnits(ISNULL(l.MeterStart, 0), s.VolumeUnitIndex))
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems l
		ON t.TransactionGuid = l.TransactionGuid
	INNER JOIN tblSites s
		ON t.SiteGuid = s.SiteGuid
	WHERE t.DeleteFlag = 0
		AND t.AliasName IN ('Retail Sale', 'Delivery Sale', 'Third Party Sale', 'Shipment')
		AND t.[Site] IN (
			SELECT *
			FROM @SiteTable
			)
		AND t.InventoryDate = @InventoryDate
	GROUP BY t.[Site]
		, t.ManagerID
		, l.Product

-- Add receipt meter transaction values to the result table
INSERT INTO @ResultTable
	SELECT m.[SiteID]
		, m.ManagerID
		, m.Product
		, ABS(SUM(m.MeterStop - m.MeterStart - ISNULL(r.Recirculation, 0)))
	FROM @ReceiptData m
	LEFT OUTER JOIN @RecirculationData r
		ON m.SiteID = r.SiteID
			AND m.ManagerID = r.ManagerID
			AND m.Product = r.Product
	GROUP BY m.SiteID
		, m.ManagerID
		, m.Product

-- Update the result table by subtracting issue meter transaction values. If there are no results for a product/manager/site already because there were no receipt meter transactions, 
-- add the issue meter throughput as a negative value (receipt meter throughput = 0 - issue meter throughput value) 
; WITH mergeTarget AS (SELECT SiteID
						,ManagerID
						,Product
						,Throughput FROM @ResultTable )
MERGE mergeTarget
USING (
		SELECT issueData.SiteID
			, issueData.ManagerID
			, issueData.Product
			, ABS(SUM(issueData.MeterStop - issueData.MeterStart - ISNULL(r.Recirculation, 0))) AS IssueThroughput
		FROM @IssueData issueData
		LEFT OUTER JOIN @RecirculationData r
			ON issueData.SiteID = r.SiteID
				AND issueData.ManagerID = r.ManagerID
				AND issueData.Product = r.Product
		GROUP BY issueData.SiteID
			, issueData.ManagerID
			, issueData.Product
		) AS mergeSource
		ON mergeSource.SiteID = mergeTarget.SiteID
			AND mergeSource.ManagerID = mergeTarget.ManagerID
			AND mergeSource.Product = mergeTarget.Product
	WHEN MATCHED
		THEN
			UPDATE
			SET Throughput = Throughput - mergeSource.IssueThroughput
	WHEN NOT MATCHED
		THEN
			INSERT (
				SiteID
				, ManagerID
				, Product
				, Throughput
)
			VALUES (
				mergeSource.SiteID
				, mergeSource.ManagerID
				, mergeSource.Product
				, mergeSource.IssueThroughput * - 1
);

RETURN
END