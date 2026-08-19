CREATE FUNCTION dbo.udf_GetCloseoutTime(
@SiteGuid UNIQUEIDENTIFIER, 
@CurrentDateTime DateTimeOffset)
RETURNS TIME
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM tblSiteCloseoutTime  WHERE SiteGuid=@SiteGuid AND @CurrentDateTime < ExpirationDate)
	BEGIN
		RETURN NULL
	END

	DECLARE @CloseoutTime TIME 
	SELECT @CloseoutTime=CloseoutTime FROM tblSiteCloseoutTime  WHERE SiteGuid=@SiteGuid AND @CurrentDateTime >= EffectiveDate AND @CurrentDateTime < ExpirationDate
	IF @CloseoutTime IS NULL
	BEGIN
		SELECT @CloseoutTime=CloseoutTime FROM tblSites  WHERE SiteGuid=@SiteGuid
	END
	RETURN @CloseoutTime
END

