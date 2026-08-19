
CREATE PROCEDURE [dbo].[fm_ArchiveTransaction]
	@TransID NVARCHAR(64)	/* ID of the transaction record that is to be archived.*/
AS
BEGIN 
	IF EXISTS(SELECT * FROM FuelsManagerDBArchive.dbo.tblTransactions WHERE TransID = @TransID) 
		RETURN;

	DECLARE @TransactionGuid uniqueidentifier

	SELECT @TransactionGuid = TransactionGuid FROM FuelsManagerDB.dbo.tblTransactions WHERE TransID = @TransID 
	EXEC fm_InsertTransactionsToArchiveTables @TransID, @TransactionGuid

	DECLARE @assocTransID NVARCHAR(64);
	DECLARE AssociatedTransactionIDs_cursor CURSOR FOR
		SELECT linkedTransID FROM FuelsManagerDB.dbo.tblTransactionLinks WHERE @TransID IN (OriginalTransID, LinkedTransID) 
	
	OPEN AssociatedTransactionIDs_cursor 
	FETCH NEXT FROM AssociatedTransactionIDs_cursor INTO @assocTransID 
	WHILE @@FETCH_STATUS = 0 
	BEGIN 
		EXEC fm_ArchiveTransaction @assocTransID  
		FETCH NEXT FROM AssociatedTransactionIDs_cursor INTO @assocTransID 
	END 
	CLOSE AssociatedTransactionIDs_cursor 
	DEALLOCATE AssociatedTransactionIDs_cursor; 
	
	DELETE FROM FuelsManagerDB.dbo.tblTransactionLinks WHERE @TransID IN (OriginalTransID, LinkedTransID) 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionSublineItems 
			WHERE TransactionLineItemGuid IN (SELECT TransactionLineItemGuid FROM FuelsManagerDB.dbo.tblTransactionLineItems WHERE TransactionGuid = @TransactionGuid) 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionLineItemUserData 
			WHERE TransactionLineItemGuid IN (SELECT TransactionLineItemGuid FROM FuelsManagerDB.dbo.tblTransactionLineItems WHERE TransactionGuid = @TransactionGuid) 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionWeightReadings WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionNotes   WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionSignature  WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionUserData   WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionPIDX    WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionLineItems  WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactions    WHERE TransactionGuid = @TransactionGuid
END
