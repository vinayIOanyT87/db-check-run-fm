CREATE PROCEDURE [dbo].[usp_GetAllEquipmentForFuelCardSitesByFuelCardIdentity]
@fuelCardGuid uniqueidentifier
AS
	BEGIN
	DECLARE @siteGuid uniqueidentifier

	DECLARE @fuelCardSiteList TABLE
	(
		[AssignedToSiteGuid] [uniqueidentifier]
		,[SiteID] [nvarchar](30)
	)

	DECLARE @equipmentList TABLE
	(
		[SiteGroupFlag] [bit]
		,[SiteID] [nvarchar](30)
		,[EquipmentGuid] [uniqueidentifier]
	)

	INSERT INTO @fuelCardSiteList SELECT AssignedToSiteGuid, SiteID FROM [dbo].[udf_GetSiteListForFuelCard](@fuelCardGuid)

	DECLARE FuelCardSiteCursor CURSOR FOR
		SELECT AssignedToSiteGuid FROM @fuelCardSiteList

	OPEN FuelCardSiteCursor
	FETCH NEXT FROM FuelCardSiteCursor INTO @siteGuid

	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO @equipmentList 
			SELECT [dbo].[tblSites].[SiteGroupFlag], [dbo].[tblSites].[ID], list.[EquipmentGuid] 
				FROM [dbo].[udf_GetAssignedEquipmentListForSite](@siteGuid) list
					INNER JOIN [dbo].[tblSites]
						ON [dbo].[tblSites].[SiteGuid] = list.[OwnerSiteGuid]
					WHERE list.[AssignedToSiteGuid] = list.[OwnerSiteGuid]

		FETCH NEXT FROM FuelCardSiteCursor INTO @siteGuid
	END

	CLOSE FuelCardSiteCursor
	DEALLOCATE FuelCardSiteCursor

	SELECT list.[SiteGroupFlag]
			,list.[SiteID] AS SiteID
			,(SELECT EqTypeName FROM [dbo].[tblEquipmentTypes] WHERE [dbo].[tblEquipmentTypes].[EquipmentTypeGuid] = [dbo].[tblEquipment].[EquipmentTypeGuid]) AS EqTypeName
			,(SELECT LookupEquipmentTypeIndex FROM [dbo].[tblEquipmentTypes] WHERE [dbo].[tblEquipmentTypes].[EquipmentTypeGuid] = [dbo].[tblEquipment].[EquipmentTypeGuid]) AS LookupEquipmentTypeIndex
			,(SELECT ID FROM [dbo].[tblFuelCards] WHERE [dbo].[tblFuelCards].[FuelCardGuid] = [dbo].[tblEquipment].[FuelCardGuid]) AS FuelCardID
			,[dbo].[tblEquipment].*
		FROM @equipmentList list
			INNER JOIN [dbo].[tblEquipment]
				ON list.[EquipmentGuid] = [dbo].[tblEquipment].[EquipmentGuid]
	ORDER BY [dbo].[tblEquipment].[ID]
END