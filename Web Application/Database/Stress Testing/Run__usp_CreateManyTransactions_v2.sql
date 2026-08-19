/*************************************************************************** 
																									  
	FILE NAME:		Run__usp_CreateManyTransactions_v2.sql							  
																									  
	PURPOSE:			Runs "usp_CreateManyTransactions_v2.sql"						  
																									  
	Copyright (C) 1999-2009		Varec, Inc.												  
										Norcross, GA, USA    All Rights Reserved		  
																									  
	This file shall not be copied or reproduced in any form without the		  
	express written consent of Varec.													  
																									  
	MODIFICATION HISTORY																		  
																									  
		DATE:				BY:				VERSION:		REASON:							  
		===========		============	========		============================ 
		03-04-2009		L. Leonard		1				Creation.						  
		
		04-03-2009		I.Orndorff		2				- Removed all references to LoginSite. 
																- Set sites to the known sites with assigned entities. 
																- Set start date to '2009-01-01' 
																- Set @TotalTxToAdd to 6,132,000. 
	
		04-10-2009		L. Leonard		3				Two of the transaction tables 
																are clustered on a GUID. This 
																was felt to be ok, as they 
																were concocting a sequential 
																GUID in C#. Sadly, NEW_ID() 
																is not sequential, so we've 
																been clustered on a GUID. 
																Wrote a quick and dirty fnx 
																to generate a sequential GUID. 
	
		04-28-2009		L. Leonard		4				Added raiserror so you know
																that escape hatch is open.
																Added code to prevent orphaned
																transactions from other runs
																from colliding with new tx guids.
																Improved speed by moving a
																SELECT COUNT(*) outside the loop.
																Added the date to the pseudo-GUID
																to allow for overlapping
																program runs for the same sites.
																Improved progress reports to
																occur more often.

		04-30-2009		I.Orndorff		5				- Drop tables after run completes.

****************************************************************************/

/*
	To Do from Richard
	------------------
	Site Admin sould have no tx
	Set GrossQuant > NetQuant to random 
	Sales and issues and drect field are neg - adj can be either
for each product for site, we need a physical inventory tx type - do one per month per product per site
*/ 


-----------------------------------------------------------------------------
-- Setup.																						  
-----------------------------------------------------------------------------

USE [ConsolidatedDB]

SET NOCOUNT ON
SET ANSI_NULLS ON
SET XACT_ABORT ON
SET QUOTED_IDENTIFIER ON

DECLARE @TotalTxToAdd					INT
DECLARE @NumOfDaysArg					INT
DECLARE @StartDate						DATETIME

DECLARE @SiteCount						INT
DECLARE @RowsPerDayPerSiteArg			INT
DECLARE @StartDateArg					DATETIME
DECLARE @StopwatchStart					DATETIME
DECLARE @SiteIndexArg					INT
DECLARE @SiteName							NVARCHAR(50)
DECLARE @RowCountWhenRunStarted		INT
DECLARE @NominalTxToAdd					INT
DECLARE @RowsAddedThisRun				INT


----------------------------------------------------------------------------- 
-- Make sure that this database has the expected number of Sites; you may 
-- have been given the database. 
----------------------------------------------------------------------------- 

SELECT @SiteCount = COUNT(DISTINCT SiteIndex)
  FROM dbo.tblSites

IF @SiteCount < 70
BEGIN
	RAISERROR('Must have at least 70 Sites in the database to fulfill our contractural obligations!', 16, 1) WITH NOWAIT, LOG
	RETURN
END

----------------------------------------------------------------------------- 
-- Sometimes different runs populate these tables with ranges that eventually
-- overlap.  This prevents you from trying to insert a dupe in this table.
----------------------------------------------------------------------------- 

DELETE FROM tblTransactionLineItemUserData
 WHERE TransLineItemID NOT IN 
			(SELECT TransLineItemID FROM tblTransactionLineItems)


----------------------------------------------------------------------------- 
-- In case the last run was interrupted.												  
----------------------------------------------------------------------------- 

RAISERROR('Determining how many transactions have already been added...', 10, 1) WITH NOWAIT

SELECT @RowCountWhenRunStarted = COUNT(*)
  FROM dbo.tblTransactionWeightReadings
 WHERE CreatedBy = 'Owl'

PRINT CAST(@RowCountWhenRunStarted AS NVARCHAR) + ' transactions have already been added.'


----------------------------------------------------------------------------- 
-- THESE ARE THE ONLY USER SETTINGS!													  
----------------------------------------------------------------------------- 

-- Processing speed is about 1500 rows per minute on my desktop, 4500 on HAL. 
-- 2400 tx per day * 365 days per year * 7 years = 6132000 
-- 2400 tx per day * 365 days per year * 2 years = 1752000 
SET @NominalTxToAdd = 1752000 
SET @TotalTxToAdd   = @NominalTxToAdd - @RowCountWhenRunStarted
SET @StartDate      = '2009-04-17'


-- To abort processing at any time, just run this statement (in another window,
-- of course).
--	CREATE TABLE ##EscapeHatch (CheckedInEveryLoop INT)


-----------------------------------------------------------------------------
-- Determine how many rows per day per site to add.  We round up to 	  
-- make sure we INSERT *at least* that many. 										  
-----------------------------------------------------------------------------

PRINT CAST(@TotalTxToAdd AS NVARCHAR) + ' transactions need to be added.'

IF @TotalTxToAdd <= 0
BEGIN
	RAISERROR('@TotalTxToAdd must be positive!', 16, 1) WITH NOWAIT, LOG
	RETURN
END

SELECT @SiteCount = COUNT(*)
	FROM dbo.tblsites
	WHERE SiteIndex > -1
   AND SiteGroupFlag = 0

PRINT 'Found ' + CAST(@SiteCount AS NVARCHAR) + ' sites for ADF.'

--SET @NumOfDaysArg = 30
SET @NumOfDaysArg = @NominalTxToAdd / 2400	-- ADF spec: 2400 Tx per day, minus days already done.
IF @NumOfDaysArg = 0 SET @NumOfDaysArg = 1

PRINT '@NumOfDaysArg is ' + CAST(@NumOfDaysArg AS NVARCHAR)

IF @NumOfDaysArg <= 0
BEGIN
	RAISERROR('@NumOfDaysArg must be positive!', 16, 1) WITH NOWAIT, LOG
	RETURN
END

SET @RowsPerDayPerSiteArg = (@TotalTxToAdd / @NumOfDaysArg) / @SiteCount
PRINT '@RowsPerDayPerSiteArg is ' + CAST(@RowsPerDayPerSiteArg AS NVARCHAR)

WHILE @TotalTxToAdd > (@RowsPerDayPerSiteArg * @NumOfDaysArg) * @SiteCount
BEGIN
	SET @RowsPerDayPerSiteArg =  @RowsPerDayPerSiteArg + 1
	PRINT '@RowsPerDayPerSiteArg incremented to ' + CAST(@RowsPerDayPerSiteArg AS NVARCHAR)
END

PRINT 'To get ' + CAST(@TotalTxToAdd AS NVARCHAR) + ' rows, setting @RowsPerDayPerSiteArg to ' + CAST(@RowsPerDayPerSiteArg AS NVARCHAR) 


-----------------------------------------------------------------------------
-- Prepare the database.																	  
-----------------------------------------------------------------------------

-- Don't want these on!
RAISERROR('Disabling triggers ...', 10, 1) WITH NOWAIT
EXEC usp_EnableTriggersByItemName @sTriggerNameLike = 'TR_ChangeLog_',                       @bEnable = 0
EXEC usp_EnableTriggersByItemName @sTriggerNameLike = 'TRIG_UPD_tblTransactions',            @bEnable = 0
EXEC usp_EnableTriggersByItemName @sTriggerNameLike = 'TRIG_DELETE_tblTransactionLineItems', @bEnable = 0
EXEC usp_EnableTriggersByItemName @sTriggerNameLike = 'TRIG_INS_tblTransactionLineItems',    @bEnable = 0

-- Ensure that ERRORLOG files don't get huge. Also, our stuff is first.
RAISERROR('Running sp_cycle_errorlog ...', 10, 1) WITH NOWAIT
EXEC sp_cycle_errorlog

-- Do performance enhancements.
RAISERROR ('Updating statistics...', 10, 1) WITH NOWAIT
UPDATE STATISTICS dbo.tblTransactions						WITH INDEX
UPDATE STATISTICS dbo.tblTransactionLineItems			WITH INDEX
--UPDATE STATISTICS dbo.tblTransactionLineItemUserData	WITH INDEX
UPDATE STATISTICS dbo.tblTransactionUserData				WITH INDEX
UPDATE STATISTICS dbo.tblTransactionNotes					WITH INDEX
UPDATE STATISTICS dbo.tblTransactionWeightReadings		WITH INDEX


-----------------------------------------------------------------------------
-- Begin processing.																			  
-----------------------------------------------------------------------------

IF OBJECT_ID(N'tempdb..##EscapeHatch') IS NOT NULL
BEGIN
	DROP TABLE ##EscapeHatch
END

SET @StartDateArg = DATEADD(day, - @NumOfDaysArg, @StartDate)
PRINT '@StartDateArg is ' + CONVERT(NVARCHAR(50), @StartDateArg, 101)

SET @StopwatchStart = GETDATE()
PRINT 'Starting at ' + CAST(@StopwatchStart AS NVARCHAR)

SET @RowsAddedThisRun = 0

-- The where clause allows us to pick up at the site we left off at.
DECLARE TheCursor CURSOR FOR
	SELECT SiteIndex, ID
		FROM dbo.tblSites AS S
		WHERE SiteIndex > -1 AND SiteGroupFlag = 0
	

OPEN TheCursor

FETCH NEXT FROM TheCursor
 INTO @SiteIndexArg, @SiteName
 
WHILE @@FETCH_STATUS = 0
BEGIN
	IF OBJECT_ID(N'tempdb..##EscapeHatch') IS NOT NULL
	BEGIN
		RAISERROR ('''tempdb..##EscapeHatch detected''... Shutting down...', 10, 1) WITH NOWAIT, LOG
		BREAK
	END

/*
print @SiteIndexArg
print 'FFFFFFFF-FFFF-FFFF-00' + RIGHT(dbo.IntToHexString(63 /* @SiteIndexArg */ ), 2) + '-' + CAST(DATEPART(dayofyear, GETDATE()) AS CHAR(3)) + '%'
SELECT 'hello', *
	FROM dbo.tblTransactions
	  WHERE TransID LIKE 'FFFFFFFF-FFFF-FFFF-00' + RIGHT(dbo.IntToHexString(63 /* @SiteIndexArg */ ), 2) + '-' + CAST(DATEPART(dayofyear, GETDATE()) AS CHAR(3)) + '%'
		 AND SiteIndex = 63 --@SiteIndexArg
--break
*/
--	IF @SiteIndexArg IN (51, 63, 87, 115, 117, 125, 151, 179)
--	BEGIN
--		RAISERROR ('-----------------------------------------------------------', 10, 1) WITH NOWAIT, LOG
--		RAISERROR ('Site index %d (ID: %s) is being skipped because they are missing some kind of reference data', 10, 1, @SiteIndexArg, @SiteName) WITH NOWAIT, LOG
--	END
--	ELSE IF EXISTS (SELECT *
--							FROM dbo.tblTransactions
--						  WHERE TransID LIKE 'FFFFFFFF-FFFF-FFFF-00%-' + RIGHT(dbo.IntToHexString(@SiteIndexArg), 2) + '-' + CAST(DATEPART(dayofyear, GETDATE()) AS CHAR(3)) + '%'
--							 AND SiteIndex = @SiteIndexArg)
--	BEGIN
--		RAISERROR ('-----------------------------------------------------------', 10, 1) WITH NOWAIT, LOG
--		RAISERROR ('Site index %d (ID: %s) has already been processed', 10, 1, @SiteIndexArg, @SiteName) WITH NOWAIT, LOG
--	END
--	ELSE
	BEGIN
		DECLARE @sDateCurrent NVARCHAR(400)
		SET @sDateCurrent = CAST(GETDATE() AS NVARCHAR)
		RAISERROR ('-----------------------------------------------------------', 10, 1) WITH NOWAIT, LOG
		RAISERROR ('Processing site index %d (ID: %s) as %s', 10, 1, @SiteIndexArg, @SiteName, @sDateCurrent) WITH NOWAIT, LOG
		
		EXEC usp_CreateManyTransactions_v2
			@SiteIndex 			= @SiteIndexArg,				-- Run each of the Sites.
			@NumOfDays			= @NumOfDaysArg,				-- 7 years is 2556 days.
			@RowsPerDay			= @RowsPerDayPerSiteArg,	-- Test Case requires 2400.
			@bDelay 				= 0,								-- Unused.
			@StartDate 			= @StartDateArg,				-- First transaction date.
			@StartTime			= @StopwatchStart,			-- Measures performance.
			@RowsAlready		= @RowCountWhenRunStarted, -- Measures performance.
			@TotalRows			= @TotalTxToAdd,				-- Estimate completion.
			@RowsAddedThisRun = @RowsAddedThisRun

	END
	
	FETCH NEXT FROM TheCursor
	 INTO @SiteIndexArg, @SiteName
END


-----------------------------------------------------------------------------
-- Done.																							  
-----------------------------------------------------------------------------


-- Close and deallocate the cursor.
CLOSE TheCursor
DEALLOCATE TheCursor


-----------------------------------------------------------------------------
-- Display results.																			  
-----------------------------------------------------------------------------

EXEC dbo.tmp_usp_TimingReport @StopwatchStart, @TotalTxToAdd, @RowsAddedThisRun

SELECT CONVERT(NVARCHAR(50), InventoryDate, 101)	AS 'Date',
	    COUNT(*)												AS 'Rows Per Day'
  FROM dbo.tblTransactions
 WHERE CreatedBy = 'Owl'
 GROUP BY InventoryDate
 ORDER BY InventoryDate DESC

SELECT CONVERT(NVARCHAR(50), InventoryDate, 101)	AS 'Date',
		 MAX(AliasName)										AS 'Alias Name',
	    COUNT(*)												AS 'Rows Per Alias Per Day'
  FROM dbo.tblTransactions
 WHERE CreatedBy = 'Owl'  
 GROUP BY InventoryDate, AliasName 
 ORDER BY InventoryDate DESC

SELECT COUNT(*) AS 'tblTransactions Count'						FROM dbo.tblTransactions						WHERE CreatedBy = 'Owl'
SELECT COUNT(*) AS 'tblTransactionLineItems Count'				FROM dbo.tblTransactionLineItems				WHERE CreatedBy = 'Owl'
--SELECT COUNT(*) AS 'tblTransactionLineItemUserData Count'	FROM dbo.tblTransactionLineItemUserData	WHERE CreatedBy = 'Owl'
SELECT COUNT(*) AS 'tblTransactionUserData Count'				FROM dbo.tblTransactionUserData				WHERE CreatedBy = 'Owl'
SELECT COUNT(*) AS 'tblTransactionNotes Count'					FROM dbo.tblTransactionNotes					WHERE CreatedBy = 'Owl'
SELECT COUNT(*) AS 'tblTransactionWeightReadings Count'		FROM dbo.tblTransactionWeightReadings		WHERE CreatedBy = 'Owl'

-- Don't forget to turn these on!
RAISERROR('Enabling triggers ...', 10, 1) WITH NOWAIT
EXEC usp_EnableTriggersByItemName @sTriggerNameLike = 'TR_ChangeLog_',                       @bEnable = 1
EXEC usp_EnableTriggersByItemName @sTriggerNameLike = 'TRIG_UPD_tblTransactions',            @bEnable = 1
EXEC usp_EnableTriggersByItemName @sTriggerNameLike = 'TRIG_DELETE_tblTransactionLineItems', @bEnable = 1
EXEC usp_EnableTriggersByItemName @sTriggerNameLike = 'TRIG_INS_tblTransactionLineItems',    @bEnable = 1

RAISERROR(' ', 10, 1) WITH NOWAIT
RAISERROR('Processing complete.', 10, 1) WITH NOWAIT


--===========================================================================
--																									  
--									END OF EXECUTION											  
--																									  
--===========================================================================




-----------------------------------------------------------------------------
-- Manual cleanup.																			  
-----------------------------------------------------------------------------

-- This code will drop the "temp" tables when sure you're done with them.
-- You'll only need to run this if something goes wrong with the script.
--IF 1 = 0
BEGIN
	RAISERROR('Drop temp tables.', 10, 1) WITH NOWAIT
	DROP TABLE dbo.TBL_TEMP_TX_ALIASES
	DROP TABLE dbo.TBL_TEMP_EQUIP_REF_DATA
	DROP TABLE dbo.TBL_TEMP_PRODUCT_REF_DATA
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER
END

-- This code will DELETE ALL ROWS added by this script.
IF 1 = 0
BEGIN
	SET NOCOUNT OFF
	DECLARE @sCmd NVARCHAR(4000)
	DECLARE @sTableName sysname

	DECLARE TheCursor CURSOR FOR
		SELECT       'tblTransactionLineItems'
		UNION	SELECT 'tblTransactionUserData'
--		UNION	SELECT 'tblTransactionLineItemUserData'
		UNION	SELECT 'tblTransactionNotes'
		UNION	SELECT 'tblTransactionWeightReadings'
		UNION	SELECT 'tblTransactions'

	OPEN TheCursor
	FETCH NEXT FROM TheCursor INTO @sTableName
			
	WHILE @@FETCH_STATUS = 0
	BEGIN

		SET @sCmd = 'RAISERROR(''DELETE FROM ' + @sTableName + ''', 10, 1) WITH NOWAIT     ' +
				 		'DELETE FROM ' + @sTableName + ' WHERE CreatedBy = ''Owl'''

		PRINT @sCmd
		EXEC (@sCmd)
		
		FETCH NEXT FROM TheCursor INTO @sTableName
	END

	CLOSE TheCursor
	DEALLOCATE TheCursor
	SET NOCOUNT ON
END

-- This code will shrink the mdf and ldf files - SLOW.
-- Only run this when your disk is getting full.
IF 1 = 0
BEGIN
	RAISERROR('Truncate the log files.', 10, 1) WITH NOWAIT
	USE [ConsolidatedDB]	CHECKPOINT
	USE Tempdb	CHECKPOINT
	USE [ConsolidatedDB]
	
	RAISERROR('Note that tempdb''s log can swell for odd reasons which a checkpoint.', 10, 1) WITH NOWAIT
	RAISERROR('won''t fix,', 10, 1) WITH NOWAIT
	DBCC SHRINKDATABASE (Tempdb, /* nTargetPercentLeft */ 1, TRUNCATEONLY /* , NOTRUNCATE */ )

	RAISERROR('Generally SHRINKDATABASE should be avoided, but this a special time.', 10, 1) WITH NOWAIT
	RAISERROR('A big, largely empty database will perform much better than a small,', 10, 1) WITH NOWAIT
	RAISERROR('"lean" one.  The NOTRUNCATE keeps any empty space reclaimed inside ',  10, 1) WITH NOWAIT
	RAISERROR('the database.  This may take a while.',  10, 1)										  WITH NOWAIT
	DBCC SHRINKDATABASE ([ConsolidatedDB], /* nTargetPercentLeft */ 1, TRUNCATEONLY /* , NOTRUNCATE */ )
END


-- This will delete all transactions of ours for a given SiteIndex.
-- Handy for when it breaks in middle of a site's processing.
IF 1 = 0
BEGIN
	DROP TABLE #tmpLAL

	SELECT TransID
	  INTO #tmpLAL
	  FROM dbo.tbltransactions
	 WHERE SiteIndex IN (81, 83)									-- <------------------
		AND CreatedBy = 'Owl'
	   AND DATEPART(dayofyear, CreatedDate) = DATEPART(dayofyear, GETDATE())

	ALTER TABLE #tmpLAL WITH CHECK
	  ADD CONSTRAINT PK_#tmpLAL PRIMARY KEY CLUSTERED (TransID)
	 WITH (PAD_INDEX = ON, FILLFACTOR = 100)

	SELECT COUNT(*) FROM #tmpLAL

	BEGIN TRAN
	RAISERROR('Deleting from tblTransactionLineItems', 10, 1)
	DELETE FROM dbo.tblTransactionLineItems			WHERE TransID IN (SELECT TransID FROM #tmpLAL)

	RAISERROR('Deleting from tblTransactionNotes', 10, 1)
	DELETE FROM dbo.tblTransactionNotes					WHERE TransID IN (SELECT TransID FROM #tmpLAL)

	RAISERROR('Deleting from tblTransactionUserData', 10, 1)
	DELETE FROM dbo.tblTransactionUserData				WHERE TransID IN (SELECT TransID FROM #tmpLAL)

	RAISERROR('Deleting from tblTransactionWeightReadings', 10, 1)
	DELETE FROM dbo.tblTransactionWeightReadings		WHERE TransID IN (SELECT TransID FROM #tmpLAL)

	RAISERROR('Deleting from tblTransactionLineItemUserData', 10, 1)
	DELETE FROM dbo.tblTransactionLineItemUserData
			 FROM dbo.tblTransactionLineItems
	WHERE tblTransactionLineItemUserData.TransLineItemID NOT IN (SELECT TransLineItemID FROM tblTransactionLineItems)

	WHILE 1=1
	BEGIN
		RAISERROR('Deleting from tbltransactions', 10, 1)
		UPDATE STATISTICS dbo.tbltransactions(IX_tblTransactions_InventoryDate)
		DELETE TOP (16000) FROM dbo.tbltransactions	WHERE TransID IN (SELECT TransID  FROM  #tmpLAL)
		IF 0 = @@ROWCOUNT  BREAK
		CHECKPOINT
		--WAITFOR DELAY '00:00:02'
	END

	COMMIT -- rollback
END


-----------------------------------------------------------------------------
-- Disable the trigger on this table first!
-----------------------------------------------------------------------------

IF 1 = 0
BEGIN
	RAISERROR('Truncate the tblChangeLog table.', 10, 1) WITH NOWAIT
	TRUNCATE TABLE dbo.tblChangeLog
END


-----------------------------------------------------------------------------
--										END OF FILE												  
-----------------------------------------------------------------------------
