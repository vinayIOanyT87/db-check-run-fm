
CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableLoadArmInputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableLoadArmInputPermissiveList TABLE
(
	[ProcessVariableLoadArmGuid] [uniqueidentifier]
	,[LoadArmGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[StationGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	-- @selectedStation should be either StationA or StationB
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

	; WITH ProcessVariableLoadArmInputPermissive_CTE ([ProcessVariableLoadArmGuid],[LoadArmGuid],[OPCConnectionGuid],[StationGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableLoadArmInputPermissive].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[LoadArmGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[OPCConnectionGuid],data1.[StationGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableLoadArmInputPermissive]
				INNER JOIN (SELECT LoadArmGuid, StationGuid, OwnerSiteGuid FROM @tblLoadArmList) data1
					ON [dbo].[tblProcessVariableLoadArmInputPermissive].[LoadArmGuid] = data1.[LoadArmGuid]
	)
	INSERT INTO @tblProcessVariableLoadArmInputPermissiveList SELECT [ProcessVariableLoadArmGuid],[LoadArmGuid],[OPCConnectionGuid],[StationGuid],[OwnerSiteGuid] FROM ProcessVariableLoadArmInputPermissive_CTE

	RETURN;
END