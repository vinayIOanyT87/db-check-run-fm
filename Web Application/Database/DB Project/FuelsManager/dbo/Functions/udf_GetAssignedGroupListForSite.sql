CREATE FUNCTION [dbo].[udf_GetAssignedGroupListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblGroupList TABLE
(
	[UserGroupToSiteGuid] [uniqueidentifier]
	,[UserGroupGuid] [uniqueidentifier]
	,[GroupGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblGroupList 
		SELECT [map].[tblEntityUserGroupToSite].[UserGroupToSiteGuid]
				,[dbo].[tblGroups].[GroupGuid] 'UserGroupGuid'
				,[dbo].[tblGroups].[GroupGuid]
				,[dbo].[tblGroups].[SiteGuid] 'OwnerSiteGuid'
				,[map].[tblEntityUserGroupToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserGroupToSite]
			INNER JOIN [dbo].[tblGroups]
				ON [map].[tblEntityUserGroupToSite].[GroupGuid] = [dbo].[tblGroups].[GroupGuid]
		WHERE ([map].[tblEntityUserGroupToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END