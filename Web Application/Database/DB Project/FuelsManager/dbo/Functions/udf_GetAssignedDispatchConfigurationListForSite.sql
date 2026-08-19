CREATE FUNCTION [dbo].[udf_GetAssignedDispatchConfigurationListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblDispatchConfigurationList TABLE
(
	[DispatchConfigurationToSiteGuid] [uniqueidentifier]
	,[DispatchConfigurationGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblDispatchConfigurationList 
		SELECT [map].[tblEntityDispatchConfigurationToSite].[DispatchConfigurationToSiteGuid], [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid],[dbo].[tblDispatchConfiguration].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityDispatchConfigurationToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityDispatchConfigurationToSite]
			INNER JOIN [dbo].[tblDispatchConfiguration]
				ON [map].[tblEntityDispatchConfigurationToSite].[DispatchConfigurationGuid] = [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid]
		WHERE ([map].[tblEntityDispatchConfigurationToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END