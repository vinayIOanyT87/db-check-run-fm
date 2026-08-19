CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableStationListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableStationList TABLE
(
	[ProcessVariableStationGuid] [uniqueidentifier]
	,[StationGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableStation_CTE ([ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableStation].[ProcessVariableStationGuid],[dbo].[tblProcessVariableStation].[StationGuid],[dbo].[tblProcessVariableStation].[OPCConnectionGuid],[dbo].[tblProcessVariableStation].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableStation]
				INNER JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedStationListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableStation].[StationGuid] = data1.[StationGuid]
				LEFT JOIN [dbo].[tblProcessVariableStation] B
					ON [dbo].[tblProcessVariableStation].[ProcessVariableStationGuid] = B.[ProcessVariableStationGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableStation].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableStationList SELECT [ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableStation_CTE

	RETURN;
END