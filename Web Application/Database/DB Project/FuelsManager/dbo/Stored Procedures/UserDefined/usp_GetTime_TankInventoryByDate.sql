CREATE PROCEDURE [dbo].[usp_GetTime_TankInventoryByDate]    
 /*=============================================    
 Author:     John Gettel    
 Create date:        
 Description:  Support IM Tank Inventory By Date Report by returning either the site or server time based on parameters    
 Version:    12.0.0.0    
 Execution:      
   EXEC [dbo].[usp_GetTime_TankInventoryByDate]  'D1D07749-F0A6-4469-BAF0-ADD2CFC8D03A', 1   
    
  select siteguid, * from tblsites  
 Modification History:    
 Date    by  Description    
 11/20/2024  JLG  return either the site or the server time    
 03/21/2025  JLG   return time as datetimeoffset instead of datetime 
 
 /**************VIVIAN NOTES
 Create new procedures just for  Inventory By Date? Changes to this stored proc would also affect 
 Current Tank Inventory Report:	
Tank Change Report:  Date/Time Based Report
********************************************/
 =============================================*/     
    @SiteGuid UNIQUEIDENTIFIER,    
   @useSiteTime BIT    
AS    
BEGIN    
  IF @useSiteTime = 1 BEGIN    
 DECLARE @SiteTimezone AS nvarchar(50)    
 SET @SiteTimezone = (select Timezone from tblSites WHERE SiteGuid = @SiteGuid)    

SELECT DATEADD(MS, -3, CAST(CAST(SYSDATETIMEOFFSET() AS DATE) AS datetime) ) AT time zone @SiteTimezone AS [Time] 
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
  SET @ServerTimeZone = (@ReportTimeZoneOverride -1)   
 ELSE    
  SET @ServerTimeZone = @HostTimeZone    
 SELECT DATEADD(MS, -3, CAST(CAST(SYSDATETIMEOFFSET() AS date) AS datetime)) AT time zone @ServerTimezone AS [Time] 
  END    
END