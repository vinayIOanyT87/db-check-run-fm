CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringFootNoteListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[FootNoteToSiteGuid] [uniqueidentifier]
	,[ApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblApplicationStringList 
		SELECT [map].[tblEntityFootNoteToSite].[FootNoteToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityFootNoteToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityFootNoteToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityFootNoteToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityFootNoteToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END