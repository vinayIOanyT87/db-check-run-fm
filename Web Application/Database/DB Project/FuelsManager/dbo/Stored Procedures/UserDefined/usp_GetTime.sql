CREATE PROCEDURE [dbo].[usp_GetTime]
 /*=============================================
 Author:	 			John Gettel
 Create date: 			
 Description: 	Support Current Tank Inventory Report by returning either the site or server time based on parameters
 Version:				12.0.0.0
 Execution:		
			EXEC [dbo].[usp_GetTime] '{SITE GUID GOES HERE}', 1

 Modification History:
	Date				by		Description
	11/20/2024  JLG		return either the site or the server time
	03/21/2025  JLG   return time as datetimeoffset instead of datetime
 =============================================*/	
    @SiteGuid UNIQUEIDENTIFIER,
	  @useSiteTime BIT
AS
BEGIN
  IF @useSiteTime = 1 BEGIN
	DECLARE @SiteTimezone AS nvarchar(50)
	SET @SiteTimezone = (select Timezone from tblSites WHERE SiteGuid = @SiteGuid)
	SELECT SYSDATETIMEOFFSET() AT time zone @SiteTimezone AS [Time]
  END ELSE BEGIN
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
	SELECT SYSDATETIMEOFFSET() AT time zone @ServerTimezone AS [Time]
  END
END
GO