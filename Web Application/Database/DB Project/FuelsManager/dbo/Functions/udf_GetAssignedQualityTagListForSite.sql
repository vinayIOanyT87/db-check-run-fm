CREATE FUNCTION [dbo].[udf_GetAssignedQualityTagListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblQualityTagList TABLE
(
	[QualityTagToSiteGuid] [uniqueidentifier]
	,[QualityTagGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblQualityTagList 
		SELECT [map].[tblEntityQualityTagToSite].[QualityTagToSiteGuid], [dbo].[tblQualityTags].[QualityTagGuid],[dbo].[tblQualityTags].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityQualityTagToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityQualityTagToSite]
			INNER JOIN [dbo].[tblQualityTags]
				ON [map].[tblEntityQualityTagToSite].[QualityTagGuid] = [dbo].[tblQualityTags].[QualityTagGuid]
		WHERE ([map].[tblEntityQualityTagToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END