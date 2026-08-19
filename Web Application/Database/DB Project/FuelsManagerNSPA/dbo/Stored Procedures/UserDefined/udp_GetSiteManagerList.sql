CREATE PROCEDURE dbo.udp_GetSiteManagerList
(
	@SiteID	NVARCHAR(60)
)
AS
BEGIN
SET NOCOUNT ON

DECLARE @SiteList TABLE (ID NVARCHAR(60))
INSERT INTO @SiteList EXEC nspa_SitesFromSiteGroup @SiteID

DECLARE @SiteGuidList TABLE (SiteGuid UNIQUEIDENTIFIER)
INSERT INTO @SiteGuidList
SELECT	SiteGuid
FROM	tblSites
WHERE	ID IN (SELECT ID FROM @SiteList)

select * from @SiteGuidList

SELECT	DISTINCT c.ID AS ManagerID
FROM	tblCompanies c	INNER JOIN map.tblCompanyToRole m ON c.CompanyGuid = m.CompanyGuid
WHERE	m.SiteGuid IN (SELECT SiteGuid FROM @SiteGuidList)
AND		m.LookupCompanyRoleIndex = 0

END -- udp_GetSiteManagerList