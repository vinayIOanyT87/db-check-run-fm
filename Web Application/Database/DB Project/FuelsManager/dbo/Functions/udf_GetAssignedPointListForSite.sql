CREATE FUNCTION [dbo].[udf_GetAssignedPointListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblPointList TABLE
(
	[PointToSiteGuid] [uniqueidentifier]
	,[PointGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblPointList 
		SELECT [dbo].[tblPoint].[PointGuid], [dbo].[tblPoint].[PointGuid],[dbo].[tblPoint].[SiteGuid] 'OwnerSiteGuid',[dbo].[tblPoint].[SiteGuid] 'AssignedToSiteGuid'
		FROM [dbo].[tblPoint]
		WHERE ([dbo].[tblPoint].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END