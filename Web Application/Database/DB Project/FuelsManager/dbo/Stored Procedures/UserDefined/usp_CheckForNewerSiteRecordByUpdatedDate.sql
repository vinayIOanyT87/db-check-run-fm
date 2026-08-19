CREATE PROCEDURE [dbo].[usp_CheckForNewerSiteRecordByUpdatedDate]
	@SiteGuid uniqueidentifier,
	@UpdatedDate datetimeoffset(7)
AS
BEGIN
	IF EXISTS (SELECT 1 FROM [dbo].[tblSites] WHERE SiteGuid = @SiteGuid)
	BEGIN
		SELECT CASE WHEN (ISNULL(UpdatedDate, CreatedDate) > @UpdatedDate) THEN 1 ELSE 0 END 'HasNewerVersion' FROM [dbo].[tblSites] WHERE SiteGuid = @SiteGuid
	END
	ELSE
	BEGIN
		SELECT 0 'HasNewerVersion'
	END
END