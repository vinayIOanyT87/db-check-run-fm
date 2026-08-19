CREATE PROCEDURE dbo.usp_GetCloseoutDateTimeForTimezone(
@SiteGuid UNIQUEIDENTIFIER, 
@DateTimeOffset DateTimeOffset,
@useSiteTime Bit)
AS
BEGIN

	SELECT dbo.udf_GetCloseoutDateTimeForTimezone(@SiteGuid,@DateTimeOffset,@useSiteTime) AS CloseoutDateTime

END