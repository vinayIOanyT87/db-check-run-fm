--******************************************************************************
-- The purpose of the Assign Tx Aliases To Ground Sites stored procedure
-- is to redistribute aliases for load testing
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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_AssignTxAliasesToGroundSites]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_AssignTxAliasesToGroundSites]
GO

CREATE PROCEDURE [dbo].[fm_AssignTxAliasesToGroundSites]
AS
SET NOCOUNT ON

BEGIN
	SELECT ID AS SiteID, SiteIndex INTO #SITE_GROUND_LIST
		FROM tblSites
		WHERE ID LIKE ('%Ground%')

	SELECT AliasName, AliasID INTO #ALIAS_ITEMS
		FROM tblTransactionAliases
		WHERE AliasName NOT LIKE ('%Aviation%')


	DECLARE @SiteID NVARCHAR(30)
	DECLARE @MaxSiteCount INT
	SELECT @MaxSiteCount = MAX(SiteCount) FROM #SITE_GROUND_LIST

	DECLARE @AliasName NVARCHAR(50)
	DECLARE @AliasIndex INT
	DECLARE @AliasIndex_Cursor CURSOR 
	SET		@AliasIndex_Cursor = CURSOR FOR SELECT AliasIndex FROM #ALIAS_ITEMS

	OPEN	@AliasIndex_Cursor
	FETCH NEXT FROM	@AliasIndex_Cursor INTO @AliasIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		FETCH NEXT FROM @AliasIndex_Cursor INTO @AliasIndex
		SELECT @AliasName = AliasName
			FROM #ALIAS_ITEMS WHERE AliasIndex = @AliasIndex

		DECLARE @SiteIndex INT
		DECLARE @SiteIndex_Cursor CURSOR 
		SET		@SiteIndex_Cursor = CURSOR FOR SELECT SiteIndex FROM #SITE_GROUND_LIST

		OPEN	@SiteIndex_Cursor
		FETCH NEXT FROM	@SiteIndex_Cursor INTO @SiteIndex
		WHILE (@@FETCH_STATUS = 0)
		BEGIN
			FETCH NEXT FROM @SiteIndex_Cursor INTO @SiteIndex
			SELECT @SiteID = SiteID FROM #SITE_GROUND_LIST WHERE SiteIndex = @SiteIndex

			PRINT 'Assigning alias "' + @AliasName + '" to Site "' + @SiteID + '"'
			BEGIN TRY
				BEGIN TRANSACTION
					INSERT INTO tblEntityToSiteMap
						(TypeID, SiteIndex, [Index], CreatedDate, CreatedBy)
					VALUES
						('Transaction Aliases', @SiteIndex, @AliasIndex, GETDATE(), 'Administrator')
				COMMIT
			END TRY
			BEGIN CATCH
				-- Ignore
				PRINT 'Already assigned alias "' + @AliasName + '" to Site "' + @SiteID + '"'
			END CATCH
		END
		CLOSE @SiteIndex_Cursor
		DEALLOCATE @SiteIndex_Cursor

	END
	CLOSE @AliasIndex_Cursor
	DEALLOCATE @AliasIndex_Cursor

	PRINT 'Completed'
	DROP TABLE #SITE_GROUND_LIST
	DROP TABLE #ALIAS_ITEMS
END
