CREATE FUNCTION dbo.udf_GetCloseoutDateTime(
@SiteGuid UNIQUEIDENTIFIER, 
@Date DATE)
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

	SET @CloseoutDateTime = DATEADD(hour, DATEPART(hour,@CloseoutTime), DATEADD(minute, DATEPART(minute,@CloseoutTime), DATEADD(second, DATEPART(second, @CloseoutTime),CONVERT(DATETIME, (CONVERT(DATE, @DateTimeOffset))))))


	SET @CloseoutDateTime =  CONVERT(DATETIME, @CloseoutDateTime)  AT TIME ZONE @SiteTimeZone
	RETURN @CloseoutDateTime
END