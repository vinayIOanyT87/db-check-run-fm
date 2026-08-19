CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableStationOutputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableStationOutputPermissiveList TABLE
(
	[ProcessVariableStationGuid] [uniqueidentifier]
	,[StationGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableStationOutputPermissive_CTE ([ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableStationOutputPermissive].[ProcessVariableStationGuid],[dbo].[tblProcessVariableStationOutputPermissive].[StationGuid],[dbo].[tblProcessVariableStationOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableStationOutputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableStationOutputPermissive]
				INNER JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedStationListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableStationOutputPermissive].[StationGuid] = data1.[StationGuid]
				LEFT JOIN [dbo].[tblProcessVariableStationOutputPermissive] B
					ON [dbo].[tblProcessVariableStationOutputPermissive].[ProcessVariableStationGuid] = B.[ProcessVariableStationGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableStationOutputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableStationOutputPermissiveList SELECT [ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableStationOutputPermissive_CTE

	RETURN;
END