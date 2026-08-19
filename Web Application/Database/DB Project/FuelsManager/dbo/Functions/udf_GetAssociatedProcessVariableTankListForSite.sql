
CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableTankListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableTankList TABLE
(
	[ProcessVariableTankGuid] [uniqueidentifier]
	,[TankGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableTank_CTE ([ProcessVariableTankGuid],[TankGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableTank].[ProcessVariableTankGuid],[dbo].[tblProcessVariableTank].[TankGuid],[dbo].[tblProcessVariableTank].[OPCConnectionGuid],[dbo].[tblProcessVariableTank].[MessageApplicationStringGuid],[dbo].[tblTanks].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblProcessVariableTank]
				INNER JOIN [dbo].[tblTanks] 
					ON [dbo].[tblProcessVariableTank].[TankGuid] = [dbo].[tblTanks].[TankGuid]
				LEFT JOIN [dbo].[tblProcessVariableTank] B
					ON [dbo].[tblProcessVariableTank].[ProcessVariableTankGuid] = B.[ProcessVariableTankGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE [dbo].[tblTanks].[SiteGuid] = @sync_context_site_guid
					AND ([dbo].[tblProcessVariableTank].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableTankList SELECT [ProcessVariableTankGuid],[TankGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableTank_CTE

	RETURN;
END