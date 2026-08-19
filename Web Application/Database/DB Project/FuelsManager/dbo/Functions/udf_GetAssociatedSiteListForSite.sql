CREATE FUNCTION [dbo].[udf_GetAssociatedSiteListForSite](
@sync_context_site_guid UNIQUEIDENTIFIER
)
RETURNS @tblSiteList TABLE
(
	[SiteGuid] [UNIQUEIDENTIFIER]
	,[OwnerSiteGuid] [UNIQUEIDENTIFIER]
)
AS
BEGIN

	-- Simplifies the Sync Stored Procedure Generation Templates
	--
	INSERT INTO @tblSiteList 
		SELECT [dbo].[tblSites].[SiteGuid],[dbo].[tblSites].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblSites]
		WHERE [dbo].[tblSites].[SiteGuid] = @sync_context_site_guid 
	RETURN;
END