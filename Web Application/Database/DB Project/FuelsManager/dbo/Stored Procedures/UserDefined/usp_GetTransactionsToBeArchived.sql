CREATE Procedure [dbo].[usp_GetTransactionsToBeArchived]
(
	@BeginDate DateTime,
	@EndDate DateTime
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetTransactionsToBeArchived] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @BeginDate: Beginning of date range to archive records.
	-- 2. @EndDate: End of date range to archive records.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	/* Step 1 : Find transactions that are old enough to become candidates for archiving. */
	INSERT INTO #TransToBeArchived (TransID, InventoryDate, SiteGuid, ProductGuid, TransTypeID)
	(SELECT t.TransID, t.InventoryDate, t.SiteGuid, l.ProductGuid, t.LookupTransTypeIndex FROM
		FuelsManagerDB.dbo.tblTransactions t LEFT JOIN FuelsManagerDB.dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid WHERE 
		t.InventoryDate BETWEEN @BeginDate AND @EndDate); 
	/* Step 2 : Filter out transactions that are not closed out. These can not be archived. Supply Order, Payment and Recovery are 
		excluded from close out.*/
	/*DELETE FROM #TransToBeArchived WHERE 
		InventoryDate > ISNULL((SELECT Max(CloseOutDate) FROM FuelsManagerDB.dbo.tblCloseoutInventory c WHERE 
		#TransToBeArchived.SiteGuid = c.SiteGuid AND c.ProductGuid = #TransToBeArchived.ProductGuid), '1/1/1900') AND 
		#TransToBeArchived.TransTypeID <> 18 AND -- Exclude from close out check: Bulk Purchase Order and Fuel Order 
		#TransToBeArchived.TransTypeID <> 21 AND -- Payment 
		#TransToBeArchived.TransTypeID <> 22 ;   -- Recovery 
	*/

	/* Step 3 : Filter out transactions that are associated to other transactions that are not yet selected as candidates for archiving.*/
	CREATE TABLE #AssociatedTransactionsNotReady (TransID NVARCHAR(64) NOT NULL); 
	WITH X (TransID1, TransID2 ) 
	AS ( 
		SELECT OriginalTransID, LinkedTransID FROM tblTransactionLinks
		UNION ALL 
		SELECT OriginalTransID, LinkedTransID FROM X JOIN tblTransactionLinks ON TransID1 = LinkedTransID 
	) 

	INSERT INTO #AssociatedTransactionsNotReady (TransID) 
	(SELECT TransID2 FROM X WHERE TransID1 NOT IN (SELECT TransID FROM  #TransToBeArchived) );
	WITH X (TransID1, TransID2 ) 
	AS ( 
		SELECT OriginalTransID, LinkedTransID FROM tblTransactionLinks
		UNION ALL 
		SELECT OriginalTransID, LinkedTransID FROM X JOIN tblTransactionLinks ON TransID1 = LinkedTransID 
	) 

	INSERT INTO #AssociatedTransactionsNotReady (TransID)
	(SELECT TransID1 FROM X WHERE TransID2 NOT IN (SELECT TransID FROM  #TransToBeArchived) );

	DELETE FROM #TransToBeArchived WHERE TransID IN (SELECT TransID FROM #AssociatedTransactionsNotReady);
	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_GetTransactionsToBeArchived' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     