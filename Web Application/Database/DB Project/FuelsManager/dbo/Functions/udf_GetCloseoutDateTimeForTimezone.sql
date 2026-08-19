CREATE FUNCTION dbo.udf_GetCloseoutDateTimeforTimezone(
@SiteGuid UNIQUEIDENTIFIER, 
@Date DATE,
@UseSiteTime BIT)
RETURNS DATETIMEOFFSET
AS
BEGIN
	DECLARE @CloseoutDateTime DATETIMEOFFSET 
	DECLARE @CloseoutTime TIME = NULL
	DECLARE @LastExpirationDate DATETIMEOFFSET = (SELECT MAX(ExpirationDate) FROM tblSiteCloseoutTime  WHERE SiteGuid=@SiteGuid)
	DECLARE @SiteTimeZone as nvarchar(50)
    SET @SiteTimeZone = (select Timezone from tblSites WHERE SiteGuid = @SiteGuid)	
	
	DECLARE @DateTimeOffset DateTimeOffset = DATEADD(second, 59, DATEADD(minute, 59, DATEADD(hour, 23,CONVERT(DATETIME,@Date)))) AT TIME ZONE @SiteTimeZone
           
	IF @LastExpirationDate IS NULL OR @DateTimeOffset > @LastExpirationDate
	BEGIN
		SELECT @CloseoutTime = CloseoutTime FROM tblSites WHERE SiteGuid=@SiteGuid
	END
	ELSE
	BEGIN
		SELECT @CloseoutTime=CloseoutTime FROM tblSiteCloseoutTime  WHERE SiteGuid=@SiteGuid AND  @DateTimeOffset BETWEEN EffectiveDate AND ExpirationDate
	END
	IF @CloseoutTime IS NULL
	BEGIN
		SET @CloseoutTime = '23:59:59'
	END

	SET @CloseoutDateTime = DATEADD(hour, DATEPART(hour,@CloseoutTime), DATEADD(minute, DATEPART(minute,@CloseoutTime), DATEADD(second, DATEPART(second, @CloseoutTime),CONVERT(DATETIME, (CONVERT(DATE, @DateTimeOffset)))))) AT TIME ZONE @SiteTimeZone

	DECLARE @HostTimeZone VARCHAR(50)
	EXEC MASTER.dbo.xp_regread 'HKEY_LOCAL_MACHINE',
	'SYSTEM\CurrentControlSet\Control\TimeZoneInformation',
	'TimeZoneKeyName',@HostTimeZone OUT

	DECLARE @ReportTimeZoneOverride NVARCHAR(MAX)
	SET @ReportTimeZoneOverride = (SELECT SettingValue from tblConfigurationSetting WHERE SettingKey = 'ReportTimeZone')

	DECLARE @ServerTimeZone nvarchar(50)
	
    --Use Configuration Setting ReportTimeZone as override, otherwise use the server/host time
	IF EXISTS (SELECT * FROM sys.time_zone_info WHERE name = @ReportTimeZoneOverride)
		SET @ServerTimeZone = @ReportTimeZoneOverride
	ELSE
		SET @ServerTimeZone = @HostTimeZone

	IF @UseSiteTime = 1
	  SET @CloseoutDateTime =  @CloseoutDateTime AT TIME ZONE @SiteTimeZone
	ELSE
	  SET @CloseoutDateTime =  @CloseoutDateTime AT TIME ZONE @ServerTimeZone

	RETURN @CloseoutDateTime
END