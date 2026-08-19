
CREATE TRIGGER [dbo].[trg_ins_tblTransactions_ForQueue] 
   ON [dbo].[tblTransactions]
   AFTER INSERT
AS 
BEGIN 
	SET NOCOUNT ON 

	-- run trigger at enterprise only
	IF ( SELECT dbo.udf_IsEnterprise()  ) = 1
	BEGIN

		IF EXISTS (SELECT 1 
					FROM Inserted i 
					WHERE i.LookupTransactionStatusIndex = 0 
					-- and one of the sales aliases
					AND i.AliasName IN ('Delivery Sale', 'Oil & Lube Sale', 'Retail Sale', 'Third Party Sale') )

		BEGIN
			
			-- add transaction to SAP queue if in complete status and is a sale 
			INSERT dbo.tblEnterpriseQueue( EnterpriseQueueGuid, SourceType, SourceID, 
				DateAdded, Priority, Status, DateUpdated, 
				CreatedBy, UpdatedBy, CreatedDate, UpdatedDate)
			SELECT NEWID(), 1, i.TransID,
				SYSDATETIMEOFFSET(), 1, 0, SYSDATETIMEOFFSET(), 
				'SAPInterface', 'SAPInterface', SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET()
			FROM Inserted i
			-- completed status
			WHERE i.LookupTransactionStatusIndex = 0 
			-- and one of the sales aliases
			AND i.AliasName IN ('Delivery Sale', 'Oil & Lube Sale', 'Retail Sale', 'Third Party Sale')
			AND NOT EXISTS (SELECT 1
							FROM dbo.tblEnterpriseQueue
						WHERE SourceID = i.TransID )
		END

	END					
END