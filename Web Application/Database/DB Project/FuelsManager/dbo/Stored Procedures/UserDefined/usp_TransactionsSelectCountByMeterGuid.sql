
/*
=============================================
Author: Ryan Hill
Create date: 4/30/12
Description:

Get a count of the number of transactions which use a particular meter
=============================================
*/
CREATE PROCEDURE [dbo].[usp_TransactionsSelectCountByMeterGuid]
(
	@MeterGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT OFF

	SELECT COUNT(*) 
	FROM ( 
		SELECT tblTransactions.TransactionGuid
		FROM tblTransactions 
		INNER JOIN tblTransactionLineItems ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid
		WHERE tblTransactionLineItems.MeterGuid = @MeterGuid
		UNION  
		SELECT tblTransactions.TransactionGuid
		FROM tblTransactions 
		INNER JOIN tblTransactionSubLineItems ON tblTransactionSubLineItems.TransactionGuid = tblTransactions.TransactionGuid 
		WHERE tblTransactionSubLineItems.MeterGuid = @MeterGuid
	) AS Results
	
END