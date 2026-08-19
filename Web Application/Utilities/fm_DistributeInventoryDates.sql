DECLARE @date_from SMALLDATETIME, @date_to SMALLDATETIME, @random_date SMALLDATETIME, @transIndex BIGINT

SET @date_from = '2003-07-01'
SET @date_to = '2010-06-30'

DECLARE transactionCursor CURSOR FOR SELECT TransIndex FROM tblTransactions WHERE TransTypeID=14 AND InventoryDate < @date_to
OPEN transactionCursor

FETCH NEXT FROM transactionCursor INTO @transIndex

DECLARE @randomSite nvarchar(32)
DECLARE @randomSiteIndex int

WHILE @@FETCH_STATUS = 0
BEGIN

SET @randomSiteIndex =  (SELECT TOP(1) SiteIndex FROM tblSites ORDER BY NewID())
SET @randomSite = (SELECT [ID] FROM tblSites WHERE SiteIndex = @randomSiteIndex)
 
SET @random_date = (@date_from + (ABS(CAST(CAST(NewID() AS BINARY(8))AS INT)) % CAST((@date_to - @date_from) AS INT)))

UPDATE tblTransactions SET InventoryDate=@random_date
 --,SiteIndex=@randomSiteIndex
 --,[Site]=@randomSite 
 WHERE TransIndex=@transIndex

FETCH NEXT FROM transactionCursor INTO @transIndex

END

CLOSE transactionCursor
DEALLOCATE transactionCursor





