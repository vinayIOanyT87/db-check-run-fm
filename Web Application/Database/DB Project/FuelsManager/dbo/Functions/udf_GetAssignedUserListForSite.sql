CREATE FUNCTION [dbo].[udf_GetAssignedUserListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserList TABLE
(
	[UserToSiteGuid] [uniqueidentifier]
	,[UserGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblUserList 
		SELECT [map].[tblEntityUserToSite].[UserToSiteGuid], [dbo].[tblUsers].[UserGuid],[dbo].[tblUsers].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserToSite]
			INNER JOIN [dbo].[tblUsers]
				ON [map].[tblEntityUserToSite].[UserGuid] = [dbo].[tblUsers].[UserGuid]
		WHERE ([map].[tblEntityUserToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END