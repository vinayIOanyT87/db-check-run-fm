
CREATE PROCEDURE [dbo].[fm_ArchiveTransactions]
		@start_date datetime,
		@end_date datetime
AS
BEGIN 

	CREATE TABLE #TransToBeArchived (TransID NVARCHAR(64) NOT NULL, InventoryDate smalldatetime, SiteGuid uniqueidentifier, ProductGuid uniqueidentifier,TransTypeID smallint);
	/*Populate #TransToBeArchived */
	EXEC fm_GetTransactionsToBeArchived @start_date, @end_date 
	/* Cursor for all archivable transactions. */
	DECLARE transactionIDs_cursor CURSOR FOR SELECT TransID, InventoryDate FROM #TransToBeArchived  

	BEGIN TRY
		DECLARE @TransID NVARCHAR(64);
		DECLARE @inventoryDate DateTime;
		DECLARE @ArchivedTransCount int 
		
		Set @ArchivedTransCount = 0
		/*
		 Archive one FM transactions per SQL transaction.
		*/
		OPEN transactionIDs_cursor;
		FETCH NEXT FROM transactionIDs_cursor INTO @TransID, @inventoryDate;
		WHILE @@FETCH_STATUS = 0  
		BEGIN   
		--	INSERT INTO #MSG (LogTime , Status , Info )
		--	(SELECT  GetDate(), 'Info', 'Archiving transaction ' + @TransID + '. InventoryDate is ' + 
		--	CONVERT(nvarchar, @inventoryDate,101));
			BEGIN TRANSACTION;
			BEGIN TRY 
				EXEC fm_ArchiveTransaction @TransID 
				IF @@TRANCOUNT > 0 
				BEGIN
					COMMIT TRANSACTION 
					SET @ArchivedTransCount = @ArchivedTransCount + 1
				END
			END TRY
			BEGIN CATCH
				IF @@TRANCOUNT > 0
				BEGIN
					ROLLBACK TRANSACTION;
					-- INSERT INTO #MSG (LogTime , Status , Info )
					(SELECT  GetDate(), 'Error' AS Status, 'Failed to archive transaction ' + @TransID + '. ' + ISNULL(ERROR_MESSAGE(),'Unknown') AS Info);
				END   
			END CATCH      
			FETCH NEXT FROM transactionIDs_cursor INTO @TransID, @inventoryDate;  
		END 
		-- insert into #MSG (LogTime , Status , Info )
		(SELECT  GetDate(), 'Success' AS Status, 'Archived transaction count = ' + CAST(@ArchivedTransCount AS nvarchar) + '. ' AS Info);
	END TRY 
	BEGIN CATCH
		-- INSERT INTO #MSG (LogTime , Status , Info ) 
		(SELECT  GetDate(), 'Error' AS Status, 'Failed to archive Accounting tables. ' +  ERROR_MESSAGE() AS Info); 
	END CATCH
	BEGIN TRY
		CLOSE transactionIDs_cursor; 
		DEALLOCATE transactionIDs_cursor; 
	END TRY 
	BEGIN CATCH 
	END CATCH 
	--SELECT * FROM #MSG order by line;
END
