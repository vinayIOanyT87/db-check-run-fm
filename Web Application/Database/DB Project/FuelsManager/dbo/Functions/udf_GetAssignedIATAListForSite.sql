CREATE FUNCTION [dbo].[udf_GetAssignedIATAListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblIATAList TABLE
(
	[IATACodeToSiteGuid] [uniqueidentifier]
	,[IATAGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblIATAList 
		SELECT [map].[tblEntityIATACodeToSite].[IATACodeToSiteGuid], [dbo].[tblIATA].[IATAGuid],[dbo].[tblIATA].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityIATACodeToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityIATACodeToSite]
			INNER JOIN [dbo].[tblIATA]
				ON [map].[tblEntityIATACodeToSite].[IATAGuid] = [dbo].[tblIATA].[IATAGuid]
		WHERE ([map].[tblEntityIATACodeToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END