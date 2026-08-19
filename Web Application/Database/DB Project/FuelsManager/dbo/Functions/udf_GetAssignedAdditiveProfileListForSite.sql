CREATE FUNCTION [dbo].[udf_GetAssignedAdditiveProfileListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAdditiveProfileList TABLE
(
	[AdditiveProfileToSiteGuid] [uniqueidentifier]
	,[AdditiveProfileGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblAdditiveProfileList 
		SELECT [map].[tblEntityAdditiveProfileToSite].[AdditiveProfileToSiteGuid], [dbo].[tblAdditiveProfiles].[AdditiveProfileGuid],[dbo].[tblAdditiveProfiles].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAdditiveProfileToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityAdditiveProfileToSite]
			INNER JOIN [dbo].[tblAdditiveProfiles]
				ON [map].[tblEntityAdditiveProfileToSite].[AdditiveProfileGuid] = [dbo].[tblAdditiveProfiles].[AdditiveProfileGuid]
		WHERE ([map].[tblEntityAdditiveProfileToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END