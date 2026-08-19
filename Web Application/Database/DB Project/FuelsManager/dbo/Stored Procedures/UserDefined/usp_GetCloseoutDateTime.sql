CREATE PROCEDURE dbo.usp_GetCloseoutDateTime(
@SiteGuid UNIQUEIDENTIFIER, 
@DateTimeOffset DateTimeOffset)
AS
BEGIN

	SELECT dbo.udf_GetCloseoutDateTime(@SiteGuid,@DateTimeOffset) AS CloseoutDateTime

END