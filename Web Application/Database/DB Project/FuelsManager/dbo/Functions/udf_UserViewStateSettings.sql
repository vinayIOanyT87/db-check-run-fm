CREATE FUNCTION [dbo].[udf_UserViewStateSettings](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserList TABLE
(
	[UserViewStateSettingGuid] [uniqueidentifier]
	,[UserGuid] [uniqueidentifier]
	,[UserToSiteGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblUserList 
		SELECT uvss.UserViewStateSettingGuid, uvss.[UserGuid], [map].[tblEntityUserToSite].[UserToSiteGuid], uvss.[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserToSite]
			INNER JOIN [dbo].[tblUsers]
				ON [map].[tblEntityUserToSite].[UserGuid] = [dbo].[tblUsers].[UserGuid]
				JOIN [dbo].[tblUserViewStateSettings] uvss
				ON [map].[tblEntityUserToSite].[UserGuid] = uvss.[UserGuid]
				AND uvss.[SiteGuid] = @sync_context_site_guid
		WHERE ([map].[tblEntityUserToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END