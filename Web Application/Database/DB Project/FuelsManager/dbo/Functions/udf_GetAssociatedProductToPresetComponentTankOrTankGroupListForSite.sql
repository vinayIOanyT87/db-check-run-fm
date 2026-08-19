CREATE FUNCTION [dbo].[udf_GetAssociatedProductToPresetComponentTankOrTankGroupListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProductToPresetComponentTankOrTankGroupList TABLE
(
	[ProductToPresetComponentTankOrTankGroupGuid] [uniqueidentifier]
	,[ProductGuid] [uniqueidentifier]
	,[AssignedToLoadArmGuid] [uniqueidentifier]
	,[TankGuid] [uniqueidentifier]
	,[TankGroupApplicationStringGuid] [uniqueidentifier]
	,[AssignedToMeterGuid] [uniqueidentifier]
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

	; WITH ProductToPresetComponentTankOrTankGroup_CTE ([ProductToPresetComponentTankOrTankGroupGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[TankGroupApplicationStringGuid],[AssignedToMeterGuid],[OwnerSiteGuid])
	AS (
		SELECT [map].[tblProductToPresetComponentTankOrTankGroup].[ProductToPresetComponentTankOrTankGroupGuid]
				,[map].[tblProductToPresetComponentTankOrTankGroup].[ProductGuid]
				,[map].[tblProductToPresetComponentTankOrTankGroup].[AssignedToLoadArmGuid]
				,[map].[tblProductToPresetComponentTankOrTankGroup].[TankGuid]
				,[map].[tblProductToPresetComponentTankOrTankGroup].[TankGroupApplicationStringGuid]
				,[map].[tblProductToPresetComponentTankOrTankGroup].[AssignedToMeterGuid]
				,data1.[OwnerSiteGuid]
			FROM [map].[tblProductToPresetComponentTankOrTankGroup]
				INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data1
					ON [map].[tblProductToPresetComponentTankOrTankGroup].[ProductGuid] = data1.[ProductGuid]
				INNER JOIN (SELECT [LoadArmGuid],[StationGuid],[OwnerSiteGuid] FROM @tblLoadArmList) data2
					ON [map].[tblProductToPresetComponentTankOrTankGroup].[AssignedToLoadArmGuid] = data2.[LoadArmGuid]
				LEFT JOIN [map].[tblProductToPresetComponentTankOrTankGroup] B
					ON [map].[tblProductToPresetComponentTankOrTankGroup].[ProductToPresetComponentTankOrTankGroupGuid] = B.ProductToPresetComponentTankOrTankGroupGuid
				LEFT JOIN (SELECT [TankGuid],[SiteGuid] 'OwnerSiteGuid' FROM [dbo].[tblTanks] WHERE [dbo].[tblTanks].[SiteGuid] = @sync_context_site_guid) data3
					ON B.[TankGuid] = data3.[TankGuid]
				LEFT JOIN [map].[tblProductToPresetComponentTankOrTankGroup] C
					ON [map].[tblProductToPresetComponentTankOrTankGroup].[ProductToPresetComponentTankOrTankGroupGuid] = C.ProductToPresetComponentTankOrTankGroupGuid
				LEFT JOIN (SELECT [TankGroupGuid],[SiteGuid] 'OwnerSiteGuid' FROM [dbo].[tblTankGroups] WHERE [dbo].[tblTankGroups].[SiteGuid] = @sync_context_site_guid) data4
					ON C.[TankGroupApplicationStringGuid] = data4.[TankGroupGuid]
				LEFT JOIN [map].[tblProductToPresetComponentTankOrTankGroup] D
					ON [map].[tblProductToPresetComponentTankOrTankGroup].[ProductToPresetComponentTankOrTankGroupGuid] = D.ProductToPresetComponentTankOrTankGroupGuid
				LEFT JOIN (SELECT [MeterGuid],[SiteGuid] 'OwnerSiteGuid' FROM [dbo].[tblMeter] WHERE [dbo].[tblMeter].[SiteGuid] = @sync_context_site_guid) data5
					ON D.[AssignedToMeterGuid] = data5.[MeterGuid]
			WHERE ([map].[tblProductToPresetComponentTankOrTankGroup].[TankGuid] IS NULL OR data3.[TankGuid] IS NOT NULL)
					AND ([map].[tblProductToPresetComponentTankOrTankGroup].[TankGroupApplicationStringGuid] IS NULL OR data4.[TankGroupGuid] IS NOT NULL)
					AND ([map].[tblProductToPresetComponentTankOrTankGroup].[AssignedToMeterGuid] IS NULL OR data5.[MeterGuid] IS NOT NULL)
	)
	INSERT INTO @tblProductToPresetComponentTankOrTankGroupList SELECT [ProductToPresetComponentTankOrTankGroupGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[TankGroupApplicationStringGuid],[AssignedToMeterGuid],[OwnerSiteGuid] FROM ProductToPresetComponentTankOrTankGroup_CTE

	RETURN;
END
