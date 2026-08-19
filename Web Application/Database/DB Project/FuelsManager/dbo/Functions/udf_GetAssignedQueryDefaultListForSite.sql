CREATE FUNCTION [dbo].[udf_GetAssignedQueryDefaultListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblQueryDefaultList TABLE
(
	[QuerySettingToSiteGuid] [uniqueidentifier]
	,[QueryDefaultGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblQueryDefaultList
		SELECT [map].[tblEntityQuerySettingToSite].[QuerySettingToSiteGuid], [dbo].[tblQueryDefaults].[QueryDefaultGuid],[dbo].[tblQueryDefaults].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityQuerySettingToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityQuerySettingToSite]
			INNER JOIN [dbo].[tblQueryDefaults]
				ON [map].[tblEntityQuerySettingToSite].[SiteGuid] = [dbo].[tblQueryDefaults].[SiteGuid]
		WHERE ([map].[tblEntityQuerySettingToSite].[MapToSiteGuid] = @sync_context_site_guid)

	RETURN;
END