
CREATE FUNCTION [dbo].[udf_IsUserPastInactivityPeriod]
(@UserGuid UNIQUEIDENTIFIER)
RETURNS bit
AS
BEGIN
	DECLARE @ReturnValue int
	DECLARE @InactivityPeriod int
	DECLARE @LastLoginDate datetimeoffset(7)

	SET @ReturnValue = 0;

	SELECT TOP 1 @InactivityPeriod = s.InactivityDisablePeriod
				,@LastLoginDate = u.LastLoginDate
		FROM dbo.tblSites s
			INNER JOIN dbo.tblUsers u
				ON s.[SiteGuid] = u.[SiteGuid]
			WHERE u.[UserGuid] = @UserGuid

	IF (@InactivityPeriod IS NOT NULL AND @InactivityPeriod > 0)
	BEGIN
		IF (@LastLoginDate IS NOT NULL AND DATEADD(DAY, @InactivityPeriod, @LastLoginDate) < SYSDATETIMEOFFSET())
		BEGIN
			SET @ReturnValue = 1;
		END
	END
		
	RETURN @ReturnValue
END