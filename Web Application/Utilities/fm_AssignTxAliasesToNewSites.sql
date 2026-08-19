--******************************************************************************
-- The purpose of the Assign Tx Aliases To new Sites stored procedure
-- is to redistribute aliases for load testing
--
-- Parameters:
--		N/A
--
-- Author: Richard R. Panachida
-- Date: 2010-08-17
--******************************************************************************
USE [ConsolidatedDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_AssignTxAliasesToNewSites]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_AssignTxAliasesToNewSites]
GO

CREATE PROCEDURE [dbo].[fm_AssignTxAliasesToNewSites]
AS
SET NOCOUNT ON

BEGIN
	SELECT ID AS SiteID, SiteIndex INTO #SITE_GROUND_LIST
		FROM tblSites
		WHERE ID LIKE ('%Site-sn%')

	SELECT AliasName, AliasID INTO #ALIAS_ITEMS
		FROM tblTransactionAliases
		WHERE TransTypeID NOT IN (18, 19, 20, 21, 22, 24)

	DECLARE @SiteID NVARCHAR(30)

	DECLARE @AliasName NVARCHAR(50)
	DECLARE @AliasIndex INT
	DECLARE @AliasIndex_Cursor CURSOR 
	SET		@AliasIndex_Cursor = CURSOR FOR SELECT AliasID FROM #ALIAS_ITEMS

	OPEN	@AliasIndex_Cursor
	FETCH NEXT FROM	@AliasIndex_Cursor INTO @AliasIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		SELECT @AliasName = AliasName
			FROM #ALIAS_ITEMS WHERE AliasID = @AliasIndex

		DECLARE @SiteIndex INT
		DECLARE @SiteIndex_Cursor CURSOR 
		SET		@SiteIndex_Cursor = CURSOR FOR SELECT SiteIndex FROM #SITE_GROUND_LIST

		OPEN	@SiteIndex_Cursor
		FETCH NEXT FROM	@SiteIndex_Cursor INTO @SiteIndex

		WHILE (@@FETCH_STATUS = 0)
		BEGIN
			SELECT @SiteID = SiteID FROM #SITE_GROUND_LIST WHERE SiteIndex = @SiteIndex

			PRINT 'Assigning alias "' + @AliasName + '" to Site "' + @SiteID + '"'
			BEGIN TRY
				INSERT INTO tblEntityToSiteMap
					(TypeID, SiteIndex, [Index], CreatedDate, CreatedBy)
				VALUES
					('Transaction Aliases', @SiteIndex, @AliasIndex, GETDATE(), 'AutoGen')
			END TRY
			BEGIN CATCH
				-- Ignore
				PRINT 'Already assigned alias "' + @AliasName + '" to Site "' + @SiteID + '"'
			END CATCH
			
			FETCH NEXT FROM @SiteIndex_Cursor INTO @SiteIndex
		END
		CLOSE @SiteIndex_Cursor
		DEALLOCATE @SiteIndex_Cursor

		FETCH NEXT FROM @AliasIndex_Cursor INTO @AliasIndex
	END
	CLOSE @AliasIndex_Cursor
	DEALLOCATE @AliasIndex_Cursor

	PRINT 'Completed'
	DROP TABLE #SITE_GROUND_LIST
	DROP TABLE #ALIAS_ITEMS
END
