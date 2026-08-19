--******************************************************************************
-- The purpose of the Move Tx From Aviation To Ground Sites stored procedure
-- is to redistribute the aviation transaction to ground transactions for 
-- load testing
--
-- Parameters:
--		N/A
--
-- Author: Richard R. Panachida
-- Date: 2010-07-23
--******************************************************************************
USE [ConsolidatedDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_MoveTxFromAviationToGroundSites]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_MoveTxFromAviationToGroundSites]
GO

CREATE PROCEDURE [dbo].[fm_MoveTxFromAviationToGroundSites]
AS
SET NOCOUNT ON


BEGIN
	SELECT ID AS SiteID, SiteIndex, 0 AS SiteCount INTO #SITE_GROUND_LIST
		FROM tblSites
		WHERE ID LIKE ('%Ground%')

	SELECT TOP (1200000) TransIndex, AliasName, AliasIndex, [Site], SiteIndex INTO #MOVE_TX_ITEMS
		FROM tblTransactions
		WHERE AliasName LIKE ('%Aviation%')

	SELECT AliasName, AliasID INTO #ALIAS_ITEMS
		FROM tblTransactionAliases
		WHERE AliasName LIKE ('%Ground%')

	SELECT ProductID, ProductIndex, 0 AS ProductCount INTO #PRODUCT_ITEMS
		FROM tblProducts
		WHERE ProductID IN ('AUTOMOTIVE DIESEL FUEL (ADF)')

	DECLARE @Counter INT
	SET @Counter = 0

	DECLARE @ProductIndex INT
	DECLARE @ProductIndex_Cursor CURSOR 
	SET		@ProductIndex_Cursor = CURSOR FOR SELECT ProductIndex FROM #PRODUCT_ITEMS

	OPEN	@ProductIndex_Cursor
	FETCH NEXT FROM	@ProductIndex_Cursor INTO @ProductIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		UPDATE #PRODUCT_ITEMS SET ProductCount = @Counter
			WHERE ProductIndex = @ProductIndex
		SET @Counter = @Counter + 1
		FETCH NEXT FROM @ProductIndex_Cursor INTO @ProductIndex
	END
	CLOSE @ProductIndex_Cursor
	DEALLOCATE @ProductIndex_Cursor

	SET @Counter = 0
	DECLARE @SiteIndex INT
	DECLARE @SiteIndex_Cursor CURSOR 
	SET		@SiteIndex_Cursor = CURSOR FOR SELECT SiteIndex FROM #SITE_GROUND_LIST

	OPEN	@SiteIndex_Cursor
	FETCH NEXT FROM	@SiteIndex_Cursor INTO @SiteIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		UPDATE #SITE_GROUND_LIST SET SiteCount = @Counter
			WHERE SiteIndex = @SiteIndex
		SET @Counter = @Counter + 1
		FETCH NEXT FROM @SiteIndex_Cursor INTO @SiteIndex
	END
	CLOSE @SiteIndex_Cursor
	DEALLOCATE @SiteIndex_Cursor

	DECLARE @ProductID NVARCHAR(50)
	DECLARE @ProductCount INT
	DECLARE @MaxProductCount INT
	SELECT @MaxProductCount = MAX(ProductCount) FROM #PRODUCT_ITEMS
	SET @ProductCount = 0

	DECLARE @SiteID NVARCHAR(30)
	DECLARE @SiteCount INT
	DECLARE @MaxSiteCount INT
	SELECT @MaxSiteCount = MAX(SiteCount) FROM #SITE_GROUND_LIST
	SET @SiteCount = 0

	DECLARE @AliasName NVARCHAR(50)
	DECLARE @AliasIndex INT

	DECLARE @TransIndex INT
	DECLARE @TransIndex_Cursor CURSOR 
	SET		@TransIndex_Cursor = CURSOR FOR SELECT TransIndex FROM #MOVE_TX_ITEMS

	OPEN	@TransIndex_Cursor
	FETCH NEXT FROM	@TransIndex_Cursor INTO @TransIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		SELECT @AliasName = AliasName, @AliasIndex = AliasIndex
			FROM #MOVE_TX_ITEMS WHERE TransIndex = @TransIndex

		IF (@AliasName = 'Sale (Aviation)')
		BEGIN
			SET @AliasName = 'Sale (Ground)'
			SELECT @AliasIndex = AliasID FROM #ALIAS_ITEMS WHERE AliasName = @AliasName
		END
		ELSE IF (@AliasName = 'Issue (Aviation)')
		BEGIN
			SET @AliasName = 'Issue (Ground)'
			SELECT @AliasIndex = AliasID FROM #ALIAS_ITEMS WHERE AliasName = @AliasName
		END
		ELSE IF (@AliasName = 'Demand (Aviation)')
		BEGIN
			SET @AliasName = 'Demand (Ground)'
			SELECT @AliasIndex = AliasID FROM #ALIAS_ITEMS WHERE AliasName = @AliasName
		END

		SELECT @ProductID = ProductID, @ProductIndex = ProductIndex
			FROM #PRODUCT_ITEMS
			WHERE ProductCount = @ProductCount

		SELECT @SiteID = SiteID, @SiteIndex = SiteIndex
			FROM #SITE_GROUND_LIST
			WHERE SiteCount = @SiteCount

		PRINT 'Moving TransIndex = ' + CONVERT(NVARCHAR(30), @TransIndex) + ' to Site: '
			  + @SiteID + ', Alias: ' + @AliasName + ', and Product: ' + @ProductID
		BEGIN TRY
			BEGIN TRANSACTION
			UPDATE tblTransactions 
				SET AliasName = @AliasName, AliasIndex = @AliasIndex, [Site] = @SiteID, 
					SiteIndex = @SiteIndex
				WHERE TransIndex = @TransIndex

			UPDATE tblTransactionLineItems
				SET Product = @ProductID, ProductIndex = @ProductIndex
				WHERE TransIndex = @TransIndex
			COMMIT
		END TRY
		BEGIN CATCH
			ROLLBACK
			DECLARE @MSG NVARCHAR(100)    
			SET @MSG = ERROR_MESSAGE()    
	 		PRINT 'Failed - ' + @MSG
	 		RAISERROR  (@MSG, 0, 1)
		END CATCH

		SET @ProductCount = @ProductCount + 1
		IF (@ProductCount > @MaxProductCount)
		BEGIN
			SET @ProductCount = 0
		END

		SET @SiteCount = @SiteCount + 1
		IF (@SiteCount > @MaxSiteCount)
		BEGIN
			SET @SiteCount = 0
		END	

		FETCH NEXT FROM @TransIndex_Cursor INTO @TransIndex
	END
	CLOSE @TransIndex_Cursor
	DEALLOCATE @TransIndex_Cursor

	PRINT 'Completed'
	DROP TABLE #MOVE_TX_ITEMS
	DROP TABLE #SITE_GROUND_LIST
	DROP TABLE #ALIAS_ITEMS
	DROP TABLE #PRODUCT_ITEMS
END
