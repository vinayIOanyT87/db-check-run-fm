--******************************************************************************
-- The purpose of the Assign Companies To Sites stored procedure
-- is to redistribute Companies for load testing
--
-- Parameters:
--		N/A
--
-- Author: Richard R. Panachida
-- Date: 2010-08-16
--******************************************************************************
USE [ConsolidatedDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_AssignCompaniesToSites]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_AssignCompaniesToSites]
GO

CREATE PROCEDURE [dbo].[fm_AssignCompaniesToSites]
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

	SELECT ID AS CompanyID, CompanyIndex INTO #COMPANY_LIST
		FROM tblCompanies
		WHERE SiteIndex = @ParentSiteIndex

	SELECT SiteIndex, AssignedToIndex, AssignedIndex, [TYPE], ID, 'AutoGen' AS CreatedBy, GETDATE() AS CreatedDate,
		   'AutoGen' AS UpdatedBy, GETDATE() AS UpdatedDate
		INTO #COMPANY_ROLE_MAP
		FROM tblCompanyMap 
		WHERE SiteIndex = @ParentSiteIndex
		
	DECLARE @SiteID NVARCHAR(30)

	DECLARE @CompanyID NVARCHAR(50)
	DECLARE @CompanyIndex INT
	DECLARE @CompanyIndex_Cursor CURSOR 
	SET		@CompanyIndex_Cursor = CURSOR FOR SELECT CompanyIndex FROM #COMPANY_LIST

	OPEN	@CompanyIndex_Cursor
	FETCH NEXT FROM	@CompanyIndex_Cursor INTO @CompanyIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		SELECT @CompanyID = CompanyID
			FROM #COMPANY_LIST WHERE CompanyIndex = @CompanyIndex

		DECLARE @SiteIndex INT
		DECLARE @SiteIndex_Cursor CURSOR 
		SET		@SiteIndex_Cursor = CURSOR FOR SELECT SiteIndex FROM #SITE_LIST

		OPEN	@SiteIndex_Cursor
		FETCH NEXT FROM	@SiteIndex_Cursor INTO @SiteIndex

		WHILE (@@FETCH_STATUS = 0)
		BEGIN
			SELECT @SiteID = SiteID FROM #SITE_LIST WHERE SiteIndex = @SiteIndex

			PRINT 'Assigning Company "' + @CompanyID + '" to Site "' + @SiteID + '"'
			BEGIN TRY
				INSERT INTO tblEntityToSiteMap
					(TypeID, SiteIndex, [Index], CreatedDate, CreatedBy)
				VALUES
					('Companies', @SiteIndex, @CompanyIndex, GETDATE(), 'AutoGen')
			END TRY
			BEGIN CATCH
				-- Ignore
				PRINT 'Already assigned company "' + @CompanyID + '" to Site "' + @SiteID + '"'
			END CATCH
			
			BEGIN TRY
				UPDATE #COMPANY_ROLE_MAP SET SiteIndex = @SiteIndex
				
				INSERT INTO tblCompanyMap (SiteIndex, AssignedToIndex, AssignedIndex, [Type], ID, 
										   CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
					SELECT * FROM #COMPANY_ROLE_MAP WHERE AssignedIndex = @CompanyIndex
			END TRY
			BEGIN CATCH
				-- Ignore
				PRINT 'Could not assign company role'
			END CATCH
				
			FETCH NEXT FROM @SiteIndex_Cursor INTO @SiteIndex
		END
		CLOSE @SiteIndex_Cursor
		DEALLOCATE @SiteIndex_Cursor

		FETCH NEXT FROM @CompanyIndex_Cursor INTO @CompanyIndex
	END
	CLOSE @CompanyIndex_Cursor
	DEALLOCATE @CompanyIndex_Cursor

	PRINT ' '
	PRINT 'Completed'
	DROP TABLE #SITE_LIST
	DROP TABLE #COMPANY_LIST
	DROP TABLE #COMPANY_ROLE_MAP
END
