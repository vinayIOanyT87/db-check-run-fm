CREATE PROCEDURE  [dbo].[usp_CalculateVRUtotals]
-- =============================================
-- Author:	  Francisco Martin Manzano
-- Create date: 1/22/2014
-- Description: This stored procedure will update the table tblVRUThresholds with the current transactions totals 
-- Modification History:
---	Date					By		Description
-- 3/12/2014				FJM		Remove calculation of the @checktime using the locatime since its supposed to be UTC
-- 5/1/2014					FJM		The script was not updating the schedule if there was only 1 threshold in the system 
-- 10/18/2018				CHK		Port forward to 9.x database
-- =============================================
AS  
BEGIN
SET NOCOUNT ON

DECLARE @checktime DATETIME
SELECT @checktime = GETUTCDATE()

DECLARE @limits TABLE 
(
	[ThresholdGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[Interval] decimal( 10, 2) NOT NULL,
	[IntervalType] [int] NOT NULL,
	[CheckDate] [datetimeoffset] NULL,
	[EndFirstPartDate] [datetimeoffset] NULL,
	[BeginSecondPartDate] [datetimeoffset] NULL
)

INSERT @limits ([ThresholdGuid], [SiteGuid], [Interval], [IntervalType], [CheckDate] )
SELECT [VRUThresholdGuid], 
[SiteGuid],
[Interval], 
[IntervalType], 
-- if we have a reset date and is greater than the reset time, then use the reset time to check
CASE WHEN COALESCE( ResetDate, '1900-01-01') > 
		-- interval type 0 - mins, 1 - hrs, 2 - days, 3 - months, 4 - years
			CASE WHEN intervaltype = 0 THEN DATEADD( MINUTE, - Interval,  @checktime) 
				 WHEN intervaltype = 1 THEN DATEADD( HOUR, - Interval,  @checktime) 
				 WHEN intervaltype = 2 THEN DATEADD( DAY, - Interval,  @checktime) 
				 WHEN intervaltype = 3 THEN DATEADD( MONTH, - Interval,  @checktime) 
				 WHEN intervaltype = 4 THEN DATEADD( YEAR, - Interval,  @checktime) 
			END	 
	THEN COALESCE( ResetDate, '1900-01-01')
	ELSE CASE WHEN intervaltype = 0 THEN DATEADD( MINUTE, - Interval,  @checktime) 
				 WHEN intervaltype = 1 THEN DATEADD( HOUR, - Interval,  @checktime) 
				 WHEN intervaltype = 2 THEN DATEADD( DAY, - Interval,  @checktime) 
				 WHEN intervaltype = 3 THEN DATEADD( MONTH, - Interval,  @checktime) 
				 WHEN intervaltype = 4 THEN DATEADD( YEAR, - Interval,  @checktime) 
			 END		
	END 
FROM tblVRUThresholds  
WHERE enabled = 1

DECLARE @maxAggDate date
SELECT @maxAggDate = MAX( transdate)
FROM dbo.tblAggProductByTransDay

IF @maxAggDate > @checktime
	SET @maxAggDate = @checktime - 1
ELSE IF @maxAggDate IS NULL
	SET @maxAggDate = '1/1/1900'


UPDATE @limits
SET [EndFirstPartDate] = CASE WHEN @checktime >  TODATETIMEOFFSET(CAST(DATEADD(day, 2, CheckDate) as date), DATEPART(TZOFFSET, CheckDate))
							 THEN TODATETIMEOFFSET(CAST(DATEADD(day, 1, CheckDate) as date), DATEPART(TZOFFSET, CheckDate)) 
							ELSE CheckDate END,
[BeginSecondPartDate] =  CASE WHEN @maxAggDate < TODATETIMEOFFSET(CAST(DATEADD(day, -1, @CheckTime) as date), DATEPART(TZOFFSET, CheckDate))
							THEN CASE WHEN @maxAggDate > CheckDate
										THEN @maxAggDate
								 ELSE CheckDate END
							ELSE CASE WHEN TODATETIMEOFFSET(CAST(DATEADD(day, -1, @CheckTime) as date), DATEPART(TZOFFSET, CheckDate)) > CheckDate
										THEN TODATETIMEOFFSET(CAST(DATEADD(day, -1, @CheckTime) as date), DATEPART(TZOFFSET, CheckDate))
								 ELSE CheckDate END
							END
							
	
DECLARE @results TABLE 
( 	[ThresholdGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[ProductGuid] [uniqueidentifier] NOT NULL,
	[Total] [float] NOT NULL
) 

-- we need to do 3 steps:
-- get the transactions with a transdatetime between the start datetime and midnight of the first day in the range
-- get the transactions between 2 days in the past and the end of the range
-- get the results from the agg between the next day of the start of the range and 2 days before the end of the range
-- ie.  range between 2/3/2012 10:12:34 AM and 4/3/2012 10:12:34 AM 
--			get transactions between    2/3/2012 10:12:34 AM and 2/4/2012 00:00:00 
--			get transactions between    4/2/2012 00:00:00 and 4/3/2012 10:12:34 AM 
--			get the agg between 2/4/2012 and 4/2/2012

SELECT l.[ThresholdGuid], t.SiteGuid, t.TransactionGuid, t.InventoryDate
INTO #TransactionList
FROM tblTransactions t
JOIN @limits l
ON (t.TransDateTime BETWEEN l.CheckDate AND l.EndFirstPartDate
OR t.TransDateTime BETWEEN  l.BeginSecondPartDate AND @checktime)
AND l.SiteGuid = t.SiteGuid
WHERE t.LookupTransTypeIndex = 5 -- BOLs
AND (t.ReversalType = 'O' or t.ReversalType IS NULL)

INSERT @results( [ThresholdGuid], [SiteGuid], [ProductGuid], [Total] )
SELECT t.[ThresholdGuid], t.[SiteGuid], tli.ProductGuid as ProductGuid, SUM( COALESCE( tli.GrossQuantity, 0)  )
FROM #TransactionList t
INNER JOIN tblTransactionLineItems tli
ON t.TransactionGuid = tli.TransactionGuid
AND t.InventoryDate = tli.TransactionInventoryDate
JOIN map.tblProductToVruTrackingConfig pm
ON pm.ProductGuid = tli.ProductGuid 
GROUP BY t.[ThresholdGuid], t.[SiteGuid], tli.ProductGuid 

INSERT @results( [ThresholdGuid], [SiteGuid], [ProductGuid], [Total] )
SELECT t.[ThresholdGuid], t.[SiteGuid], tsli.ProductGuid as ProductGuid, SUM( COALESCE( tsli.GrossQuantity, 0)  )
FROM #TransactionList t
JOIN tblTransactionSubLineItems tsli
ON t.TransactionGuid = tsli.TransactionGuid
AND t.InventoryDate = tsli.TransactionInventoryDate
JOIN map.tblProductToVruTrackingConfig pm
ON pm.ProductGuid = tsli.ProductGuid 
GROUP BY t.[ThresholdGuid], [SiteGuid], tsli.ProductGuid 

DROP TABLE #TransactionList

INSERT @results( [ThresholdGuid], [SiteGuid], [ProductGuid], [Total] )
SELECT l.[ThresholdGuid], l.[SiteGuid], pm.[ProductGuid], SUM([Total])
FROM tblAggProductByTransDay a
JOIN @limits l
ON a.TransDate BETWEEN l.EndFirstPartDate AND DATEADD(day, -1, l.BeginSecondPartDate)
AND l.EndFirstPartDate <> l.BeginSecondPartDate
AND l.SiteGuid = a.SiteGuid
JOIN map.tblProductToVruTrackingConfig pm
ON pm.ProductGuid = a.ProductGuid
GROUP BY l.[ThresholdGuid],  l.[SiteGuid], pm.[ProductGuid]

UPDATE  vl
SET CurrentValue = ( SELECT STR( COALESCE( -SUM([Total]), 0 ), 15, 5 ) as CurrentValue
					FROM @results r
					WHERE vl.[VRUThresholdGuid] = r.[ThresholdGuid]
					AND r.SiteGuid = vl.SiteGuid),
LastCalculationDate = SYSDATETIMEOFFSET()					
FROM [tblVRUThresholds] vl

-- update the schedule based on the tolerance
DECLARE @interval int
SET @interval = 30
IF (SELECT COUNT(*) FROM tblVRUThresholds ) > 0 
BEGIN 
	SELECT @interval = MIN( CASE WHEN Limit = 0 THEN 1
		 WHEN CurrentValue * 100.00 / (Limit - Limit * ( Tolerance / 100.0 )) > 90 THEN 1 
		 WHEN CurrentValue * 100.00 / (Limit - Limit * ( Tolerance / 100.0 )) > 80 THEN 10
		 WHEN CurrentValue * 100.00 / (Limit - Limit * ( Tolerance / 100.0 )) > 70 THEN 15
		 WHEN CurrentValue * 100.00 / (Limit - Limit * ( Tolerance / 100.0 )) > 60 THEN 20
		 WHEN CurrentValue * 100.00 / (Limit - Limit * ( Tolerance / 100.0 )) > 50 THEN 25 
		 ELSE 30 END )
	FROM tblVRUThresholds
END

EXEC msdb..sp_update_schedule @name = 'VRU_calculation_schedule', @freq_subday_interval = @interval 

END