CREATE PROCEDURE  [dbo].[usp_AggDailyBOLByProduct]
AS  
BEGIN
-- =============================================
-- Author:	  Francisco Martin Manzano
-- Create date: 1/22/2014
-- Description: This stored procedure will populate the table tblAggProductByTransDay
--				tblAggProductByTransDay contains the SUM of the gross quantity by day by product (only component products).
--				the day is the UTC day, not the site calendar day, to avoid confusion with timezones.  
-- Modification History:
---	Date					By		Description
-- =============================================

SET NOCOUNT ON

-- Since this data is used to calculate the current values for the VRU tracking, if no VRU thresholds are configured we don't need to do anything
IF ( SELECT COUNT(*) FROM dbo.tblVRUThresholds ) = 0
BEGIN
	RETURN
END

DECLARE @starttime datetime, @endtime datetime
SELECT  @starttime = COALESCE( MAX( TransDate),'1/1/2000') 
							FROM tblAggProductByTransDay 
-- We don't want to close the previous day because we may still have transactions in progress so to be safe lets do the day before. 
-- No transaction should last more than 24 hrs in InProgress status.
SELECT @endtime = CAST( CAST( GETUTCDATE() - 2 AS date) as datetime)  

DECLARE @results TABLE 
( 	
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[TransDate] [date] NOT NULL,
	[ProductGuid] [uniqueidentifier] NOT NULL,
	[Total] [float] NOT NULL
) 
-- get all the products in the line items
INSERT @results( [SiteGuid], [TransDate], [ProductGuid], [Total] )
SELECT t.SiteGuid, CAST( t.TransDateTime AS DATE), tli.ProductGuid as ProductGuid, SUM( COALESCE( tli.GrossQuantity, 0)  )
FROM tblTransactions t (NOLOCK)
JOIN tblTransactionLineItems tli (NOLOCK)
ON t.TransactionGuid = tli.TransactionGuid
AND t.InventoryDate = tli.TransactionInventoryDate
JOIN tblproducts pm (NOLOCK)
ON pm.ProductGuid = tli.ProductGuid 
AND pm.LookupProductTypeIndex = 0 -- only component product
WHERE t.LookupTransTypeIndex = 5 -- BOLs
AND (t.ReversalType = 'O' or t.ReversalType IS NULL)  -- we only want original transactions, not updates or reverse/updates
AND t.TransDateTime BETWEEN @starttime AND @endtime
GROUP BY t.SiteGuid, CAST( t.TransDateTime AS DATE), tli.ProductGuid 

-- get all the products in the subline items
INSERT @results( [SiteGuid], [TransDate], [ProductGuid], [Total] )
SELECT t.SiteGuid, CAST(t.TransDateTime AS DATE), tsli.ProductGuid as ProductIndex, SUM( COALESCE( tsli.GrossQuantity, 0)  )
FROM tblTransactions t (NOLOCK)
JOIN tblTransactionSubLineItems tsli (NOLOCK)
ON t.TransactionGuid = tsli.TransactionGuid
AND t.InventoryDate = tsli.TransactionInventoryDate
JOIN tblproducts pm (NOLOCK)
ON pm.ProductGuid = tsli.ProductGuid 
AND pm.LookupProductTypeIndex = 0 -- only component product
WHERE t.LookupTransTypeIndex = 5 -- BOLs
AND (t.ReversalType = 'O' or t.ReversalType IS NULL) -- we only want original transactions, not updates or reverse/updates
AND t.TransDateTime BETWEEN @starttime AND @endtime
GROUP BY t.SiteGuid, CAST(t.TransDateTime AS DATE), tsli.ProductGuid

-- populate the agg table adding the values from lineitem and sublineitems
INSERT tblAggProductByTransDay( SiteGuid, TransDate, ProductGuid, Total )
SELECT [SiteGuid], [TransDate], [ProductGuid], SUM([Total]) 
FROM @results r
WHERE NOT EXISTS ( SELECT 1
					FROM tblAggProductByTransDay a
					WHERE r.SiteGuid = a.SiteGuid
					AND r.TransDate = a.TransDate
					AND r.ProductGuid = a.ProductGuid )
GROUP BY [SiteGuid], [TransDate], [ProductGuid]
ORDER BY SiteGuid, TransDate, ProductGuid

END