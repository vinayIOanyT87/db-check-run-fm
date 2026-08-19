
CREATE TRIGGER [dbo].[trg_insupd_tblTransactionLineItems_ForQueue] 
   ON [dbo].[tblTransactionLineItems]
   AFTER INSERT, UPDATE 
AS 
BEGIN 
	SET NOCOUNT ON 

	-- run trigger at enterprise only
	IF ( SELECT dbo.udf_IsEnterprise()  ) = 1
	BEGIN

		-- if we are working with delivery sale transactions
		IF EXISTS (SELECT 1 
					FROM Inserted i 
					JOIN tblTransactions t 
					ON t.TransactionGuid = i.TransactionGuid
					AND t.AliasName = 'Transportation Sale'
					-- completed status
					WHERE i.LookupTransactionStatusIndex = 0 )

		BEGIN
			
			DECLARE  @TransList TABLE ( TransID nvarchar(255) )

			-- add transaction to SAP queue if in complete status and is a sale 
			-- NOTE: we need to get a unique transID in case multiple line items are saved		
			INSERT  @TransList (TransID) 
			SELECT DISTINCT t.TransID
				FROM Inserted i 
				JOIN tblTransactions t 
				ON t.TransactionGuid = i.TransactionGuid
				AND t.AliasName = 'Transportation Sale'
				-- completed status
				WHERE i.LookupTransactionStatusIndex = 0
				-- all line items for the same transaction have to be in complete status
				AND NOT EXISTS ( SELECT 1
								 FROM tblTransactionLineItems tli
								 WHERE i.TransactionGuid = tli.TransactionGuid
								 AND tli.LookupTransactionStatusIndex <> 0 )
				AND NOT EXISTS (SELECT 1
								FROM dbo.tblEnterpriseQueue
								WHERE SourceID = t.TransID ) 
			
			-- add to the queue
			INSERT dbo.tblEnterpriseQueue( EnterpriseQueueGuid, SourceType, SourceID, 
				DateAdded, Priority, Status, DateUpdated, 
				CreatedBy, UpdatedBy, CreatedDate, UpdatedDate)
			SELECT NEWID(), 1, t.TransID,
				SYSDATETIMEOFFSET(), 1, 0, SYSDATETIMEOFFSET(), 
				'SAPInterface', 'SAPInterface', SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()
				FROM  @TransList t
					
			-- update transaction status to Pending
			UPDATE l
			SET LookupTransactionStatusIndex = 16			
			FROM  tblTransactions l
			JOIN @TransList tl
			ON l.TransID = tl.TransID	
					
		END

	END 					
END
