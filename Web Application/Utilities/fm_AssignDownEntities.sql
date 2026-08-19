--******************************************************************************
-- The purpose of the Assign Down Entities stored procedure is to assign down
-- the company and equipment entities from the owner site to all the other 
-- sites.  The assumption is that the entity owner site has all the other
-- sites as children sites.
--
-- Parameters:
--		@EntityOwnerSite  - This is the owner site of the entity.
--
-- Author: Richard R. Panachida
-- Date: 2010-07-20
--******************************************************************************
USE [ConsolidatedDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_AssignDownEntities]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_AssignDownEntities]
GO

CREATE PROCEDURE [dbo].[fm_AssignDownEntities]
(
	@EntityOwnerSite  VARCHAR(50)
)
AS
SET NOCOUNT ON

BEGIN
	IF ((@EntityOwnerSite IS NULL) OR (@EntityOwnerSite = ''))
	BEGIN
		DECLARE @MSG VARCHAR(100)    
		SET @MSG = 'ERROR: Must have an Entity Owner Site.'    
		PRINT @MSG
		RAISERROR  (@MSG, 0, 1)
	END

	DECLARE @OwnerSiteIndex int
	SELECT @OwnerSiteIndex = SiteIndex FROM tblSites WHERE [ID] = @EntityOwnerSite

	DECLARE @SiteID NVARCHAR(30)
	DECLARE @SiteIndex INT
	DECLARE @SiteIndex_Cursor CURSOR 
	SET		@SiteIndex_Cursor = CURSOR FOR SELECT SiteIndex FROM tblSites
										   WHERE SiteIndex <> @OwnerSiteIndex AND SiteIndex <> -1

	OPEN	@SiteIndex_Cursor
	FETCH NEXT FROM	@SiteIndex_Cursor INTO @SiteIndex
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		SELECT @SiteID = ID FROM tblSites WHERE SiteIndex = @SiteIndex

		--*******************************************************************************
		-- Loop through the list of companies to be assigned down to the sites
		--*******************************************************************************
		DECLARE @CompanyCount INT
		DECLARE @CompanyID NVARCHAR(30)
		DECLARE @CompanyIndex INT
		DECLARE @CompanyIndex_Cursor CURSOR 
		SET		@CompanyIndex_Cursor = CURSOR FOR SELECT CompanyIndex FROM tblCompanies
												  WHERE SiteIndex = @OwnerSiteIndex

		OPEN	@CompanyIndex_Cursor
		FETCH NEXT FROM	@CompanyIndex_Cursor INTO @CompanyIndex
		WHILE (@@FETCH_STATUS = 0)
		BEGIN
			SELECT @CompanyID = ID
				FROM tblCompanies WHERE CompanyIndex = @CompanyIndex

			SELECT @CompanyCount = COUNT(*) FROM tblEntityToSiteMap
				WHERE TypeID = 'Companies' 
					  AND SiteIndex = @SiteIndex
					  AND [Index] = @CompanyIndex

			IF (@CompanyCount = 0)
			BEGIN
				PRINT 'Assigning company "' + @CompanyID + '" to site "' + @SiteID + '".'
				INSERT INTO tblEntityToSiteMap (TypeID, SiteIndex, [Index], CreatedDate, CreatedBy)
					VALUES ('Companies', @SiteIndex, @CompanyIndex, GETDATE(), 'Administrator')
			END
			FETCH NEXT FROM @CompanyIndex_Cursor INTO @CompanyIndex
		END
		CLOSE @CompanyIndex_Cursor
		DEALLOCATE @CompanyIndex_Cursor

		--*******************************************************************************
		-- Loop through the list of equipment to be assigned down to the sites
		--*******************************************************************************
		DECLARE @EquipmentCount INT
		DECLARE @EquipmentID NVARCHAR(30)
		DECLARE @EquipmentIndex INT
		DECLARE @EquipmentIndex_Cursor CURSOR 
		SET		@EquipmentIndex_Cursor = CURSOR FOR SELECT [Index] FROM tblEquipment
												  WHERE SiteIndex = @OwnerSiteIndex

		OPEN	@EquipmentIndex_Cursor
		FETCH NEXT FROM	@EquipmentIndex_Cursor INTO @EquipmentIndex
		WHILE (@@FETCH_STATUS = 0)
		BEGIN
			SELECT @EquipmentID = ID
				FROM tblEquipment WHERE EquipmentIndex = @EquipmentIndex

			SELECT @EquipmentCount = COUNT(*) FROM tblEntityToSiteMap
				WHERE TypeID = 'Equipment' 
					  AND SiteIndex = @SiteIndex
					  AND [Index] = @EquipmentIndex

			IF (@EquipmentCount = 0)
			BEGIN
				PRINT 'Assigning equipment "' + @EquipmentID + '" to site "' + @SiteID + '".'
				INSERT INTO tblEntityToSiteMap (TypeID, SiteIndex, [Index], CreatedDate, CreatedBy)
					VALUES ('Equipment', @SiteIndex, @EquipmentIndex, GETDATE(), 'Administrator')
			END
			FETCH NEXT FROM @EquipmentIndex_Cursor INTO @EquipmentIndex
		END
		CLOSE @EquipmentIndex_Cursor
		DEALLOCATE @EquipmentIndex_Cursor

		FETCH NEXT FROM @SiteIndex_Cursor INTO @SiteIndex
	END
	CLOSE @SiteIndex_Cursor
	DEALLOCATE @SiteIndex_Cursor
END
