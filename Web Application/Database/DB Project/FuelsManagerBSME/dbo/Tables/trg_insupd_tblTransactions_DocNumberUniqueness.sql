CREATE TRIGGER [dbo].[trg_insupd_tblTransactions_DocNumberUniqueness]
    ON [dbo].[tblTransactions]
    AFTER INSERT, UPDATE
    AS
    BEGIN
		-- This method will check for bulk inserts and updates as well as individual
		-- inserts and updates.
		IF (SELECT COUNT(*) 
			FROM tblTransactions t 
				INNER JOIN inserted i ON (t.SiteGuid = i.SiteGuid AND t.TransactionAliasGuid = i.TransactionAliasGuid
					AND t.DocumentNumber IS NOT NULL AND t.DocumentNumber <> ''
					AND i.DocumentNumber IS NOT NULL AND i.DocumentNumber <> ''
					AND t.DocumentNumber = i.DocumentNumber)
			WHERE t.TransID <> i.TransID
				AND (i.DeleteFlag IS NULL OR i.DeleteFlag = cast(0 as bit))
				AND (t.DeleteFlag IS NULL OR t.DeleteFlag = cast(0 as bit))
				AND (ISNULL(i.ReversalType,'O') = 'O' OR ISNULL(i.ReversalType,'') = '')
				AND (ISNULL(t.ReversalType,'O') = 'O' OR ISNULL(t.ReversalType,'') = '') 
				AND t.TransID <> ISNULL(i.ConjoinTransID,'')
				AND i.TransID <> ISNULL(t.ConjoinTransID,'')
				AND i.DocumentNumber IS NOT NULL) > 0
		BEGIN
			-- (error message, severity, state)
			ROLLBACK
			RAISERROR('Cannot insert/update duplicate DocumentNumber.', 16, 1)
			RETURN
		END
    END
