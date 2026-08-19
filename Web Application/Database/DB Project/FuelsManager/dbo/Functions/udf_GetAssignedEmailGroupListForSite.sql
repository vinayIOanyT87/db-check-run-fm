CREATE FUNCTION [dbo].[udf_GetAssignedEmailGroupListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblEmailGroupList TABLE
(
	[EmailGroupToSiteGuid] [uniqueidentifier]
	,[EmailGroupGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblEmailGroupList 
		SELECT [map].[tblEntityEmailGroupToSite].[EmailGroupToSiteGuid], [dbo].[tblEmailGroups].[EmailGroupGuid],[dbo].[tblEmailGroups].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityEmailGroupToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityEmailGroupToSite]
			INNER JOIN [dbo].[tblEmailGroups]
				ON [map].[tblEntityEmailGroupToSite].[EmailGroupGuid] = [dbo].[tblEmailGroups].[EmailGroupGuid]
		WHERE ([map].[tblEntityEmailGroupToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END