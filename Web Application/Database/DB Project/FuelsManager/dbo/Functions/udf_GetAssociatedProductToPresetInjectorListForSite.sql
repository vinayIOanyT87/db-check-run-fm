CREATE FUNCTION [dbo].[udf_GetAssociatedProductToPresetInjectorListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProductToPresetInjectorList TABLE
(
	[ProductToPresetInjectorGuid] [uniqueidentifier]
	,[ProductGuid] [uniqueidentifier]
	,[AssignedToLoadArmGuid] [uniqueidentifier]
	,[TankGuid] [uniqueidentifier]
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

	; WITH ProductToPresetInjector_CTE ([ProductToPresetInjectorGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid])
	AS (
		SELECT [map].[tblProductToPresetInjector].[ProductToPresetInjectorGuid]
				,[map].[tblProductToPresetInjector].[ProductGuid]
				,[map].[tblProductToPresetInjector].[AssignedToLoadArmGuid]
				,[map].[tblProductToPresetInjector].[TankGuid]
				,data1.[OwnerSiteGuid]
			FROM [map].[tblProductToPresetInjector]
				INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data1
					ON [map].[tblProductToPresetInjector].[ProductGuid] = data1.[ProductGuid]
				INNER JOIN (SELECT [LoadArmGuid],[StationGuid],[OwnerSiteGuid] FROM @tblLoadArmList) data2
					ON [map].[tblProductToPresetInjector].[AssignedToLoadArmGuid] = data2.[LoadArmGuid]
				LEFT JOIN [map].[tblProductToPresetInjector] B
					ON [map].[tblProductToPresetInjector].[ProductToPresetInjectorGuid] = B.ProductToPresetInjectorGuid
				LEFT JOIN (SELECT [TankGuid],[SiteGuid] 'OwnerSiteGuid' FROM [dbo].[tblTanks] WHERE [dbo].[tblTanks].[SiteGuid] = @sync_context_site_guid) data3
					ON B.[TankGuid] = data3.[TankGuid]
			WHERE ([map].[tblProductToPresetInjector].[TankGuid] IS NULL OR data3.[TankGuid] IS NOT NULL)
	)
	INSERT INTO @tblProductToPresetInjectorList SELECT [ProductToPresetInjectorGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid] FROM ProductToPresetInjector_CTE

	RETURN;
END

