CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableSiteListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableSiteList TABLE
(
	[ProcessVariableSiteGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableSite_CTE ([ProcessVariableSiteGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableSite].[ProcessVariableSiteGuid],[dbo].[tblProcessVariableSite].[OPCConnectionGuid],[dbo].[tblProcessVariableSite].[MessageApplicationStringGuid],[dbo].[tblProcessVariableSite].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblProcessVariableSite]
				LEFT JOIN [dbo].[tblProcessVariableSite] B
					ON [dbo].[tblProcessVariableSite].[ProcessVariableSiteGuid] = B.[ProcessVariableSiteGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data1
					ON B.[MessageApplicationStringGuid] = data1.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableSite].[SiteGuid] = @sync_context_site_guid)
					AND ([dbo].[tblProcessVariableSite].[MessageApplicationStringGuid] IS NULL OR data1.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableSiteList SELECT [ProcessVariableSiteGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableSite_CTE

	RETURN;
END