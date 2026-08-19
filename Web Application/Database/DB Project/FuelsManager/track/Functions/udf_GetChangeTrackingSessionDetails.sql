CREATE FUNCTION [track].[udf_GetChangeTrackingSessionDetails]()
RETURNS @tTrackingSessionContext TABLE
(
	[ContextName] [nvarchar](100),
	[BypassTrackingFlags] [int],
	[BypassReason] [nvarchar](512)
)
AS
BEGIN
	-- BypassTrackingFlags: Bypass Insert Change Tracking = 0x01
	--						Bypass Update Change Tracking = 0x02
	--						Bypass Delete Change Tracking = 0x04
	--
	-- Bypass Insert and Updates: 0x01 & 0x02
	--

	-- If SyncEnabled is FALSE in tblConfigurationSettings then we bypass all triggers
	--
	IF EXISTS (SELECT 1 FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'SyncEnabled' AND SettingValue = '0')
	BEGIN
		INSERT INTO @tTrackingSessionContext SELECT 'SYNCDISABLED', 0x07, 'SyncEnabled is set to False in tblConfigurationSetting'
	END
	ELSE
	BEGIN
		INSERT INTO @tTrackingSessionContext SELECT ContextName
													, BypassTrackingFlags
													, BypassReason 
												FROM [track].[tblChangeTrackingSession]
												WHERE SqlServerSessionID = @@SPID;
	END
	RETURN;
END