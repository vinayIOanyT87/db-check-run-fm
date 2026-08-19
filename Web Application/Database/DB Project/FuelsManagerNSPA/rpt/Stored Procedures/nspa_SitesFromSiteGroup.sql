CREATE PROCEDURE [rpt].[nspa_SitesFromSiteGroup]
(
	@SiteID	NVARCHAR(60)
)
AS
BEGIN
SET NOCOUNT ON

IF (SELECT SiteGroupFlag FROM tblSites WHERE ID = @SiteID) = 1
	SELECT ID AS SiteID FROM tblSites WHERE SiteGroupFlag = 0 AND [Enabled] = 1

ELSE
	SELECT @SiteID AS SiteID

END -- nspa_SitesFromSiteGroup