
CREATE PROCEDURE [dbo].[usp_SiteList]
@SiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @GroupFlag bit

	SELECT @GroupFlag = SiteGroupFlag
	FROM dbo.tblSites
	WHERE SiteGuid = @SiteGuid

	IF (@GroupFlag = 0)
	  BEGIN
		 SELECT ID, SiteGuid FROM dbo.tblSites WHERE SiteGuid = @SiteGuid
	  END
	ELSE
	  BEGIN
		 SELECT ID, SiteGuid 
         FROM dbo.tblSites 
         WHERE SiteGuid IN (SELECT ChildSiteGuid FROM [map].[tblSiteToSite] WHERE ParentSiteGuid = @SiteGuid) AND
               SiteGuid <> @SiteGuid
	  END
END