CREATE PROCEDURE [dbo].[usp_GetSiteTime]
	@SiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @TargetTimezone AS nvarchar(50)
	SET @TargetTimezone = (select Timezone from tblSites WHERE SiteGuid = @SiteGuid)
	SELECT SYSDATETIMEOFFSET() AT time zone @TargetTimezone AS SiteTime
END

GO