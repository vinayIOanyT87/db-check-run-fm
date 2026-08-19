CREATE FUNCTION dbo.udf_GetEffectiveCloseoutDateTime(
@SiteGuid UNIQUEIDENTIFIER, 
@DateTimeOffset DATETIMEOFFSET)
RETURNS DATETIMEOFFSET
AS
BEGIN
	DECLARE @CloseoutDateTime DATETIMEOFFSET 
	DECLARE @CloseoutTime TIME = NULL
	DECLARE @LastExpirationDate DATETIMEOFFSET = (SELECT MAX(ExpirationDate) FROM tblSiteCloseoutTime  WHERE SiteGuid=@SiteGuid)

	IF @LastExpirationDate IS NULL OR @DateTimeOffset >= @LastExpirationDate
	BEGIN
		SELECT @CloseoutTime = CloseoutTime FROM tblSites WHERE SiteGuid=@SiteGuid
	END
	ELSE
	BEGIN
		SELECT @CloseoutTime=CloseoutTime FROM tblSiteCloseoutTime  WHERE SiteGuid=@SiteGuid AND  @DateTimeOffset >= EffectiveDate AND @DateTimeOffset < ExpirationDate
	END
	IF @CloseoutTime IS NULL
	BEGIN
		RETURN NULL
	END

	SET @CloseoutDateTime = DATEADD(hour, DATEPART(hour,@CloseoutTime), DATEADD(minute, DATEPART(minute,@CloseoutTime), DATEADD(second, DATEPART(second, @CloseoutTime),CONVERT(DATETIME,(CONVERT(DATE, @DateTimeOffset))))))

	DECLARE @SiteTimeZone as nvarchar(50)
    SET @SiteTimeZone = (select Timezone from tblSites WHERE SiteGuid = @SiteGuid)
	SET @CloseoutDateTime =  CONVERT(DATETIME, @CloseoutDateTime)  AT TIME ZONE @SiteTimeZone
	RETURN @CloseoutDateTime
END