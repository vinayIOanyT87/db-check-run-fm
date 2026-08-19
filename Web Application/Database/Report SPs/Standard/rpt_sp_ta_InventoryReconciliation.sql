USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[rpt_sp_ta_InventoryReconciliation]    Script Date: 08/31/2012 11:54:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rpt_sp_ta_InventoryReconciliation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[rpt_sp_ta_InventoryReconciliation]
GO

USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[rpt_sp_ta_InventoryReconciliation]    Script Date: 08/31/2012 11:54:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[rpt_sp_ta_InventoryReconciliation]
/**-- =============================================
 Author:		Kimberly Foote
 Create date:   4/16/2009
 Version:		7.5.3.1
 Description:	Used to pull <All> Products FROM fm_InventoryReconciliation
 Execution:

			EXECUTE rpt_sp_ta_InventoryReconciliation '','','June 2009','3427 - CITGO Petroleum Corp','9025','1','1','2'
			EXECUTE rpt_sp_ta_InventoryReconciliation '','','May 2009','3427 - CITGO Petroleum Corp','Regular','1','1','2'

			EXECUTE rpt_sp_ta_InventoryReconciliation '','','June 2009','3427 - CITGO Petroleum Corp','<All>','1','1','2'
			EXECUTE rpt_sp_ta_InventoryReconciliation '','','May 2009','3427 - CITGO Petroleum Corp','<All>','1','1','2'


 Modification History:
	Date		BY		Description
    5/15/09		kf		Only pull producttype 0 and 2
	2/24/2010	kf		SP now pulls ONLY products with ledgers FROM subreport.
	4/15/2010	kf		Version change due to change in sp subreport.
	1/24/2011	kf		Version change due to change in sp subreport.
	8/31/2012	WK		Formatted and commented all code. Added dynamic transaction columns for the temp table

============================================**/
( 
	@BeginDate DATETIME,
	@EndDate DATETIME,
	@Month	NVARCHAR(20),
	@Manager NVARCHAR(30),
	@Product NVARCHAR(30),
	@LoginSiteIndex INT,
	@SiteIndex INT,
	@UserIndex INT
)

AS

IF (@Product = '<All>')
	BEGIN

		-- Create base temp table
		CREATE TABLE #TransactionColumns(Test NVARCHAR(1))

		-- Define and assign local variables
		DECLARE @AlterTableString nvarchar (2000)
		DECLARE @ColumnOrder nvarchar (32)
		DECLARE @FirstTime INT
		DECLARE @Counter INT
		DECLARE @FieldCount INT

		SET @FirstTime = 1
		SET @Counter = 0

		-- Get a count of all fields configured for the Standard View and Inventory Reconciliation Type
		SET @FieldCount = ( SELECT COUNT([Index])
							FROM tblListViewFields
							WHERE ListViewIndex = (SELECT [Index] 
													FROM tblListViews 
													WHERE [Type] = 2
													AND TypeIndex = 4
													AND SiteIndex=@SiteIndex))
													
		-- Loop through count of transactions and add columns to temp table
		WHILE @Counter < @FieldCount
		BEGIN
			IF (@FirstTime = 1)
			  BEGIN
					SET @AlterTableString = 'ALTER TABLE #TransactionColumns ADD Column' + CAST(@Counter AS NVARCHAR(2)) + ' DATETIME NULL'
				 SET @FirstTime = 0
			  END
			ELSE
			  BEGIN
				   SET @AlterTableString = @AlterTableString + ', Column' + CAST(@Counter AS NVARCHAR(2)) + ' FLOAT NULL'
			  END

			   -- increment counter
			   SET @Counter = @Counter + 1
		END

		EXEC(@AlterTableString)

		-- Add fixed columns to temp table
		ALTER TABLE #TransactionColumns ADD TotalMovementGross FLOAT
											, TotalMovementNet FLOAT
											, DailyVarianceGrossPercentage FLOAT
											, DailyVarianceNetPercentage FLOAT
											, TotalVarianceGrossPercentage FLOAT
											, TotalVarianceNetPercentage FLOAT
											, ToleranceGrossResult FLOAT
											, ToleranceNetResult FLOAT
											, TotalActivityGross FLOAT
											, TotalActivityNet FLOAT
											, Product NVARCHAR(30)
		-- Remove place holder column									
		ALTER TABLE #TransactionColumns
		DROP COLUMN Test

		-- Clear table of any data
		DELETE #TransactionColumns

		-- DEBUGGING ***
		--SELECT *
		--FROM #TransactionColumns
		--RETURN;
		-- DEBUGGING ***

		DECLARE @ProductList TABLE(ProductID NVARCHAR(30))

		/*********
			Products
		**********/
		DECLARE @Products TABLE (ProductID NVARCHAR(30))

		INSERT INTO @Products	
					-- Get a list of all Components and Additives that don't inhibit accounting
					SELECT	ProductID
					FROM	tblProducts 
					WHERE ProductType IN (0,2) 
					AND (InhibitAccounting <> 1)
					AND	SiteIndex = @SiteIndex
		
		-- DEBUGGING ***
		--SELECT *
		--FROM @Products
		--RETURN;
		-- DEBUGGING ***
		
		/*****
		Main Query - This is for the <ALL> option
		*******/
		-- Loop through the @Products TABLE variable with all products: Components and Additives
		-- Run the InvReconSub stored procedure for each product
		DECLARE ProductCursor CURSOR FOR SELECT * FROM @Products 

		OPEN ProductCursor
		FETCH NEXT FROM ProductCursor INTO @Product

		WHILE @@FETCH_STATUS = 0
			BEGIN
				-- Insert values from InvReconSub stored procedure into #TransactionColumns temp table
				INSERT INTO #TransactionColumns 
					EXECUTE rpt_sp_ta_InventoryReconciliationSub @BeginDate
																,@EndDate
																,@Month
																,@Manager
																,@Product
																,@LoginSiteIndex
																,@SiteIndex
																,@UserIndex
																,'Net'
																,2
				
				-- Build a list of products that don't have all 0s OR NULLs for its columns
				INSERT INTO @ProductList 
							SELECT	@Product		
							FROM #TransactionColumns
							Where (Column1 <> 0 
									--OR Column2 <> NULL 
									--OR Column3 <> NULL 
									--OR Column4 <> NULL 
									--OR Column5 <> NULL 
									--OR Column6  <> NULL 
									--OR Column7 <> 0 
									--OR Column8 <> NULL 
									--OR Column9 <> NULL 
									--OR Column10 <> 0 
									--OR Column11 <> NULL 
									OR TotalMovementGross <> 0 
									OR TotalMovementNet <> 0 
									OR DailyVarianceGrossPercentage <> 0 
									OR DailyVarianceNetPercentage <> 0 
									OR TotalVarianceGrossPercentage <> 0 
									OR TotalVarianceNetPercentage <> 0 
									OR ToleranceGrossResult <> NULL 
									OR ToleranceNetResult <> NULL 
									OR TotalActivityGross <> 0 
									OR TotalActivityNet  <> 0)

				DELETE FROM #TransactionColumns

				FETCH NEXT FROM ProductCursor INTO @Product
			END
		CLOSE ProductCursor

		-- Cleanup resources
		DROP TABLE #TransactionColumns
					
	END	
ELSE
	-- Use the individual product passed in as a parameter
	BEGIN
		INSERT INTO @ProductList	
					SELECT	ProductID
					FROM	tblProducts 
					WHERE ProductType IN (0,2) 
					AND (InhibitAccounting <> 1)
					AND	SiteIndex = @SiteIndex 
					AND	ProductID = @Product
	END

-- Return list of Products
SELECT DISTINCT ProductID 
FROM @ProductList

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


GRANT EXECUTE ON dbo.rpt_sp_ta_InventoryReconciliation TO [public]
GO

