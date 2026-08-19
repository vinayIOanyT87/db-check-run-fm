CREATE FUNCTION dbo.udp_NSPA_ReconciliationException_MeterVariance
(
	@SiteID			NVARCHAR(60),
	@InventoryDate	DATETIME
)
RETURNS FLOAT
AS
BEGIN

DECLARE @MeterTable TABLE (SiteID NVARCHAR(60), ManagerID NVARCHAR(60), Product NVARCHAR(60), Throughput FLOAT)
DECLARE @GaugeTable TABLE (SiteID NVARCHAR(60), ManagerID NVARCHAR(60), Product NVARCHAR(60), Throughput FLOAT, Error BIT)

-- I feel the specs are wrong here, rather than the differences, it should be one minus the other. For example if meter is -100 and gauged is 100, then variance is 200 rather than 0.
-- But because I have no one to ask this question, I will assume the specs are correct. If it turns out it isn't, please address is part.
INSERT INTO @MeterTable SELECT * FROM dbo.udp_NSPA_MeterReconciliation_MeterThroughput(@SiteID, @InventoryDate)
INSERT INTO @GaugeTable SELECT * FROM dbo.udp_NSPA_MeterReconciliation_GaugedThroughput(@SiteID, @InventoryDate)

RETURN	(
			SELECT	SUM(ABS(ISNULL(g.Throughput, 0) - ISNULL(m.Throughput,0))) -- difference as per spec
			FROM	@MeterTable m FULL OUTER JOIN @GaugeTable g 
						ON	m.SiteID = g.SiteID
						AND	m.ManagerID = g.ManagerID
						AND	m.Product = g.Product
		)

END -- udp_NSPA_ReconciliationException_MeterVariance