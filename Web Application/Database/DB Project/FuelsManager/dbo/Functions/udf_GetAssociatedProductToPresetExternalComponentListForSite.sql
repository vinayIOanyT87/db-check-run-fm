CREATE FUNCTION [dbo].[udf_GetAssociatedProductToPresetExternalComponentListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProductToPresetExternalComponentList TABLE
(
	[ProductToPresetExternalComponentGuid] [uniqueidentifier]
	,[ProductGuid] [uniqueidentifier]
	,[AssignedToLoadArmGuid] [uniqueidentifier]
	,[TankGuid] [uniqueidentifier]
	,[TankGroupApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	DECLARE @tblLoadArmList TABLE
	(
		[LoadArmGuid] [uniqueidentifier]
		,[StationGuid] [uniqueidentifier]
		,[OwnerSiteGuid] [uniqueidentifier]
	)

	; WITH LoadArm_CTE ([LoadArmGuid],[StationGuid],[OwnerSiteGuid])
	AS (
		SELECT [dbo].[tblLoadArms].[LoadArmGuid], [dbo].[tblLoadArms].[BayAStationGuid] 'StationGuid', data1.[OwnerSiteGuid]
			FROM [dbo].[tblLoadArms]
				LEFT JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedStationListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblLoadArms].[BayAStationGuid] = data1.[StationGuid]
			WHERE ([dbo].[tblLoadArms].[BayAStationGuid] IS NOT NULL AND data1.[StationGuid] IS NOT NULL)
		UNION
		SELECT [dbo].[tblLoadArms].[LoadArmGuid], [dbo].[tblLoadArms].[BayBStationGuid] 'StationGuid', data1.[OwnerSiteGuid]
			FROM [dbo].[tblLoadArms]
				INNER JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedStationListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblLoadArms].[BayBStationGuid] = data1.[StationGuid]
			WHERE ([dbo].[tblLoadArms].[BayBStationGuid] IS NOT NULL AND data1.[StationGuid] IS NOT NULL)
	)
	INSERT INTO @tblLoadArmList SELECT [LoadArmGuid],[StationGuid],[OwnerSiteGuid] FROM LoadArm_CTE

	; WITH ProductToPresetExternalComponent_CTE ([ProductToPresetExternalComponentGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[TankGroupApplicationStringGuid],[OwnerSiteGuid])
	AS (
		SELECT [map].[tblProductToPresetExternalComponent].[ProductToPresetExternalComponentGuid]
				,[map].[tblProductToPresetExternalComponent].[ProductGuid]
				,[map].[tblProductToPresetExternalComponent].[AssignedToLoadArmGuid]
				,[map].[tblProductToPresetExternalComponent].[TankGuid]
				,[map].[tblProductToPresetExternalComponent].[TankGroupApplicationStringGuid]
				,data1.[OwnerSiteGuid]
			FROM [map].[tblProductToPresetExternalComponent]
				INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data1
					ON [map].[tblProductToPresetExternalComponent].[ProductGuid] = data1.[ProductGuid]
				INNER JOIN (SELECT [LoadArmGuid],[StationGuid],[OwnerSiteGuid] FROM @tblLoadArmList) data2
					ON [map].[tblProductToPresetExternalComponent].[AssignedToLoadArmGuid] = data2.[LoadArmGuid]
				LEFT JOIN [map].[tblProductToPresetExternalComponent] B
					ON [map].[tblProductToPresetExternalComponent].[ProductToPresetExternalComponentGuid] = B.ProductToPresetExternalComponentGuid
				LEFT JOIN (SELECT [TankGuid],[SiteGuid] 'OwnerSiteGuid' FROM [dbo].[tblTanks] WHERE [dbo].[tblTanks].[SiteGuid] = @sync_context_site_guid) data3
					ON B.[TankGuid] = data3.[TankGuid]
				LEFT JOIN (SELECT [TankGroupGuid],[SiteGuid] 'OwnerSiteGuid' FROM [dbo].[tblTankGroups] WHERE [dbo].[tblTankGroups].[SiteGuid] = @sync_context_site_guid) data4
					ON B.[TankGroupApplicationStringGuid] = data4.[TankGroupGuid]
			WHERE ([map].[tblProductToPresetExternalComponent].[TankGuid] IS NULL OR data3.[TankGuid] IS NOT NULL)
				AND ([map].[tblProductToPresetExternalComponent].[TankGroupApplicationStringGuid] IS NULL OR data4.[TankGroupGuid] IS NOT NULL)
	)
	INSERT INTO @tblProductToPresetExternalComponentList SELECT [ProductToPresetExternalComponentGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[TankGroupApplicationStringGuid],[OwnerSiteGuid] FROM ProductToPresetExternalComponent_CTE

	RETURN;
END

