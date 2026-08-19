CREATE PROCEDURE [dbo].[usp_EnterpriseDequeueRecords]
	 @SourceType int, 
	 @MaxRecordCount int
AS

BEGIN

SET TRANSACTION ISOLATION LEVEL READ COMMITTED

;WITH cte (EnterpriseQueueGuid, TransID, ParentID, Level, SortPath)
AS(
    SELECT q.EnterpriseQueueGuid, t.TransID, cast( NULL as nvarchar(64)), 0,
        cast(row_number() over (partition by t.ReversedTransID order by t.ReversalType) as varbinary(max))
	FROM tblEnterpriseQueue q WITH (UPDLOCK, READPAST)
	JOIN tblTransactions t
	ON q.SourceID = t.TransID
    WHERE  NOT EXISTS ( SELECT 1 
						FROM tblEnterpriseQueue q2 WITH (UPDLOCK, READPAST)
						WHERE t.ReversedTransID = q2.SourceID 
						AND q2.Status = 0 AND SourceType = @SourceType)
						
    AND q.Status = 0 AND SourceType = @SourceType
    UNION ALL
    SELECT q.EnterpriseQueueGuid, t2.TransID, t2.ReversedTransID,  g2.Level + 1,
        g2.SortPath + cast(row_number() over (partition by t2.ReversedTransID order by t2.ReversalType) as binary(4))
	FROM tblEnterpriseQueue q WITH (UPDLOCK, READPAST)
	JOIN tblTransactions t2
	ON q.SourceID = t2.TransID
    JOIN cte g2 
    ON t2.ReversedTransID = g2.TransID
    WHERE q.Status = 0 AND SourceType = @SourceType

)

select TOP (@MaxRecordCount) q.* INTO #tblRecs
from cte c
JOIN tblEnterpriseQueue q WITH (UPDLOCK, READPAST)
ON c.EnterpriseQueueGuid = q.EnterpriseQueueGuid
order by SortPath


-- if we've found one, mark it as being processed
UPDATE a SET a.Status = 1, DateUpdated = getdate() 
FROM dbo.tblEnterpriseQueue a
INNER JOIN #tblRecs b on a.EnterpriseQueueGuid = b.EnterpriseQueueGuid

-- If we've got an item from the queue, return to whatever is going to process it
SELECT * FROM #tblRecs

DROP TABLE #tblRecs

END
GO


