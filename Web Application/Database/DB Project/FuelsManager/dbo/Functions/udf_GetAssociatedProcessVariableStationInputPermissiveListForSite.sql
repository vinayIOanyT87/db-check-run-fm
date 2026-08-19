CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableStationInputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableStationInputPermissiveList TABLE
(
	[ProcessVariableStationGuid] [uniqueidentifier]
	,[StationGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableStationInputPermissive_CTE ([ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableStationInputPermissive].[ProcessVariableStationGuid],[dbo].[tblProcessVariableStationInputPermissive].[StationGuid],[dbo].[tblProcessVariableStationInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableStationInputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableStationInputPermissive]
				INNER JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedStationListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableStationInputPermissive].[StationGuid] = data1.[StationGuid]
				LEFT JOIN [dbo].[tblProcessVariableStationInputPermissive] B
					ON [dbo].[tblProcessVariableStationInputPermissive].[ProcessVariableStationGuid] = B.[ProcessVariableStationGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableStationInputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableStationInputPermissiveList SELECT [ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableStationInputPermissive_CTE

	RETURN;
END