CREATE PROCEDURE [dbo].[usp_GetAssignedEquipmentByFuelCardIdentity]
@FuelCardGuid uniqueidentifier
AS
BEGIN
	SELECT [dbo].[tblSites].[SiteGroupFlag]
			,[dbo].[tblSites].[ID] AS SiteID
			,(SELECT EqTypeName FROM [dbo].[tblEquipmentTypes] WHERE [dbo].[tblEquipmentTypes].[EquipmentTypeGuid] = [dbo].[tblEquipment].[EquipmentTypeGuid]) AS EqTypeName
			,(SELECT LookupEquipmentTypeIndex FROM [dbo].[tblEquipmentTypes] WHERE [dbo].[tblEquipmentTypes].[EquipmentTypeGuid] = [dbo].[tblEquipment].[EquipmentTypeGuid]) AS LookupEquipmentTypeIndex
			,(SELECT ID FROM [dbo].[tblFuelCards] WHERE [dbo].[tblFuelCards].[FuelCardGuid] = [dbo].[tblEquipment].[FuelCardGuid]) AS FuelCardID
			,[dbo].[tblEquipment].*
		FROM [dbo].[tblEquipment]
			INNER JOIN [dbo].[tblSites]
				ON [dbo].[tblEquipment].[SiteGuid] = [dbo].[tblSites].[SiteGuid]
	WHERE [dbo].[tblEquipment].[FuelCardGuid] = @FuelCardGuid
	ORDER BY [dbo].[tblEquipment].[ID]
END
