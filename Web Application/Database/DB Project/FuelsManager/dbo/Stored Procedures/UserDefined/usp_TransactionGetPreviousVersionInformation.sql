
CREATE PROCEDURE [dbo].[usp_TransactionGetPreviousVersionInformation]
(
	@TransactionGuids dbo.TransactionGuidListType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT
		tblTransactions.TransactionGuid,
		tblTransactions.DeleteFlag,
		tblTransactions.LookupTransactionStatusIndex,
		tblTransactions.TransVersion,
		tblTransactions.InventoryDate,
		HasWeightReadings = CASE WHEN EXISTS (SELECT * FROM tblTransactionWeightReadings 
								WHERE tblTransactionWeightReadings.TransactionGuid = tblTransactions.TransactionGuid AND HistoricalFlag = 0) THEN CAST(1 AS BIT) 
							ELSE CAST(0 AS BIT) 
							END
	FROM tblTransactions 
	INNER JOIN @TransactionGuids transactionGuids ON transactionGuids.TransactionGuid = tblTransactions.TransactionGuid
	WHERE ISNULL(tblTransactions.DeleteFlag, 0) <> 1 -- The save transactions processor considered deleted transactions to have no prior version. Checking the deleted flag mimics that functionality.
END 

