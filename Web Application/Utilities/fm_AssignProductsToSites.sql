--******************************************************************************
-- The purpose of the Assign Product To Sites stored procedure
-- is to redistribute Products for load testing
--
-- Parameters:
--		N/A
--
-- Author: Richard R. Panachida
-- Date: 2010-08-13
--******************************************************************************
USE [ConsolidatedDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_AssignProductsToSites]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_AssignProductsToSites]
GO

CREATE PROCEDURE [dbo].[fm_AssignProductsToSites]
(
	@ParentSiteID NVARCHAR(30)
)
AS
SET NOCOUNT ON

BEGIN
	DECLARE @ParentSiteIndex INT
	SELECT @ParentSiteIndex = SiteIndex FROM tblSites WHERE ID = @ParentSiteID

	SELECT ID AS SiteID, SiteIndex INTO #SITE_LIST
		FROM tblSites
		WHERE SiteIndex <> @ParentSiteIndex

	SELECT ProductID, ProductIndex INTO #PRODUCT_LIST
		FROM tblProducts
		WHERE SiteIndex = @ParentSiteIndex

	DECLARE @SiteID NVARCHAR(30)

	DECLARE @ProductID NVARCHAR(50)
	DECLARE @ProductIndex INT
	DECLARE @ProductIndex_Cursor CURSOR 
	SET		@ProductIndex_Cursor = CURSOR FOR SELECT ProductIndex FROM #PRODUCT_LIST

	OPEN	@ProductIndex_Cursor
	FETCH NEXT FROM	@ProductIndex_Cursor INTO @ProductIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		SELECT @ProductID = ProductID
			FROM #PRODUCT_LIST WHERE ProductIndex = @ProductIndex

		DECLARE @SiteIndex INT
		DECLARE @SiteIndex_Cursor CURSOR 
		SET		@SiteIndex_Cursor = CURSOR FOR SELECT SiteIndex FROM #SITE_LIST

		OPEN	@SiteIndex_Cursor
		FETCH NEXT FROM	@SiteIndex_Cursor INTO @SiteIndex

		WHILE (@@FETCH_STATUS = 0)
		BEGIN
			SELECT @SiteID = SiteID FROM #SITE_LIST WHERE SiteIndex = @SiteIndex

			PRINT 'Assigning Product "' + @ProductID + '" to Site "' + @SiteID + '"'
			BEGIN TRY
				INSERT INTO tblEntityToSiteMap
					(TypeID, SiteIndex, [Index], CreatedDate, CreatedBy)
				VALUES
					('Products', @SiteIndex, @ProductIndex, GETDATE(), 'AutoGen')
			END TRY
			BEGIN CATCH
				-- Ignore
				PRINT 'Already assigned product "' + @ProductID + '" to Site "' + @SiteID + '"'
			END CATCH
			
			FETCH NEXT FROM @SiteIndex_Cursor INTO @SiteIndex
		END
		CLOSE @SiteIndex_Cursor
		DEALLOCATE @SiteIndex_Cursor

		FETCH NEXT FROM @ProductIndex_Cursor INTO @ProductIndex
	END
	CLOSE @ProductIndex_Cursor
	DEALLOCATE @ProductIndex_Cursor

	PRINT 'Completed'
	DROP TABLE #SITE_LIST
	DROP TABLE #PRODUCT_LIST
END
