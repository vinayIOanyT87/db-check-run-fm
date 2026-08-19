CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringEmailAddressListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[EmailAddressToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityEmailAddressToSite].[EmailAddressToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityEmailAddressToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityEmailAddressToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityEmailAddressToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityEmailAddressToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END