CREATE FUNCTION dbo.NSPA_SitesFromSiteGroup
(
	@SiteID	NVARCHAR(60)
)
RETURNS @ResultTable TABLE (SiteID NVARCHAR(60))
AS
BEGIN

IF (SELECT SiteGroupFlag FROM tblSites WHERE ID = @SiteID) = 1
	INSERT INTO @ResultTable
	SELECT ID AS SiteID FROM tblSites WHERE SiteGroupFlag = 0 AND [Enabled] = 1

ELSE
	INSERT INTO @ResultTable
	SELECT @SiteID AS SiteID

RETURN

END -- udp_SitesFromSiteGroup